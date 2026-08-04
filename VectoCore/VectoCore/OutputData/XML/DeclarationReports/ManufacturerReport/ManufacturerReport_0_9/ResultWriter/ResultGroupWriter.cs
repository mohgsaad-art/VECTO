using System;
using System.Linq;
using System.Xml.Linq;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.Common;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.OutputData.XML.DeclarationReports.ManufacturerReport.ManufacturerReport_0_9.
	ResultWriter
{

	public class MRFResultSuccessMissionWriter : AbstractResultWriter, IResultSequenceWriter
	{
		public MRFResultSuccessMissionWriter(ICommonResultsWriterFactory factory, XNamespace ns) : base(factory, ns) { }

		#region Overrides of AbstractResultWriter

		public virtual XElement[] GetElement(IResultEntry entry)
		{
			return new[] {
				new XElement(TNS + XMLNames.Report_Result_Mission, entry.Mission.ToXMLFormat()) ,
				new XElement(TNS + XMLNames.Report_ResultEntry_Distance, entry.Distance.ConvertToKiloMeter().ValueAsUnit(decimals:1))
			};
		}

		public XElement[] GetElement(IOVCResultEntry entry)
		{
			return null;
		}

		#endregion
	}

	public class ResultSimulationParameterMRFBusWriter : AbstractResultGroupWriter
	{
		public ResultSimulationParameterMRFBusWriter(ICommonResultsWriterFactory factory, XNamespace ns) : base(factory, ns) { }

		#region Overrides of AbstractResultWriter

		public override XElement GetElement(IResultEntry entry)
		{
			return new XElement(TNS + XMLNames.Report_ResultEntry_SimulationParameters,
				new XElement(TNS + XMLNames.Report_ResultEntry_TotalVehicleMass,
					entry.TotalVehicleMass.ValueAsUnit(XMLNames.Unit_kg)),
				new XElement(TNS + XMLNames.Report_Result_Payload,
					entry.Payload.ValueAsUnit(XMLNames.Unit_kg)),
				new XElement(TNS + XMLNames.Report_Result_PassengerCount,
					(entry.PassengerCount ?? double.NaN).ToXMLFormat(2))
			);
		}

		#endregion
	}

	public class VehiclePerformanceMRFWriter : AbstractResultGroupWriter
	{
		public VehiclePerformanceMRFWriter(ICommonResultsWriterFactory factory, XNamespace ns) : base(factory, ns) { }

		#region Overrides of AbstractResultGroupWriter

		public override XElement GetElement(IResultEntry entry)
		{
			if (entry.Status == VectoRun.Status.PrimaryBusSimulationIgnore) {
				return new XElement(TNS + XMLNames.Report_ResultEntry_VehiclePerformance,
					new XElement(TNS + XMLNames.Report_ResultEntry_AverageSpeed, new ConvertedSI(double.NaN, XMLNames.Unit_kmph).ValueAsUnit()),
					new XElement(TNS + XMLNames.Report_ResultEntry_AvgDrivingSpeed, new ConvertedSI(double.NaN, XMLNames.Unit_kmph).ValueAsUnit()),
					new XElement(TNS + XMLNames.Report_ResultEntry_MinSpeed, new ConvertedSI(double.NaN, XMLNames.Unit_kmph).ValueAsUnit()),
					new XElement(TNS + XMLNames.Report_ResultEntry_MaxSpeed, new ConvertedSI(double.NaN, XMLNames.Unit_kmph).ValueAsUnit()),
					new XElement(TNS + XMLNames.Report_ResultEntry_MaxDeceleration, new ConvertedSI(double.NaN, XMLNames.Unit_mps2).ValueAsUnit()),
					new XElement(TNS + XMLNames.Report_ResultEntry_MaxAcceleration, new ConvertedSI(double.NaN, XMLNames.Unit_mps2).ValueAsUnit()),
					new XElement(TNS + XMLNames.Report_ResultEntry_FullLoadDrivingtimePercentage, 0),
					new XElement(TNS + XMLNames.Report_ResultEntry_GearshiftCount, 0),
					new XElement(TNS + XMLNames.Report_ResultEntry_EngineSpeedDriving,
						new XElement(TNS + XMLNames.Report_ResultEntry_EngineSpeedDriving_Min, new ConvertedSI(double.NaN, XMLNames.Unit_RPM).ValueAsUnit()),
						new XElement(TNS + XMLNames.Report_ResultEntry_EngineSpeedDriving_Avg, new ConvertedSI(double.NaN, XMLNames.Unit_RPM).ValueAsUnit()),
						new XElement(TNS + XMLNames.Report_ResultEntry_EngineSpeedDriving_Max, new ConvertedSI(double.NaN, XMLNames.Unit_RPM).ValueAsUnit())
					),
					new XElement(TNS + XMLNames.Report_Results_AverageGearboxEfficiency, new ConvertedSI(double.NaN, XMLNames.UnitPercent).ValueAsUnit()),
					new XElement(TNS + XMLNames.Report_Results_AverageAxlegearEfficiency, new ConvertedSI(double.NaN, XMLNames.UnitPercent).ValueAsUnit())
				);
			}
			return new XElement(TNS + XMLNames.Report_ResultEntry_VehiclePerformance,
				new XElement(TNS + XMLNames.Report_ResultEntry_AverageSpeed, entry.AverageSpeed.ValueAsUnit(XMLNames.Unit_kmph, 1)),
				new XElement(TNS + XMLNames.Report_ResultEntry_AvgDrivingSpeed, entry.AverageDrivingSpeed.ValueAsUnit(XMLNames.Unit_kmph, 1)),
				new XElement(TNS + XMLNames.Report_ResultEntry_MinSpeed, entry.MinSpeed.ValueAsUnit(XMLNames.Unit_kmph, 1)),
				new XElement(TNS + XMLNames.Report_ResultEntry_MaxSpeed, entry.MaxSpeed.ValueAsUnit(XMLNames.Unit_kmph, 1)),
				new XElement(TNS + XMLNames.Report_ResultEntry_MaxDeceleration, entry.MaxDeceleration.ValueAsUnit(XMLNames.Unit_mps2, 2)),
				new XElement(TNS + XMLNames.Report_ResultEntry_MaxAcceleration, entry.MaxAcceleration.ValueAsUnit(XMLNames.Unit_mps2, 2)),
				new XElement(TNS + XMLNames.Report_ResultEntry_FullLoadDrivingtimePercentage, entry.FullLoadPercentage.ToXMLFormat(2)),
				(entry.GearshiftCount.Count > 0)
					? entry.GearshiftCount
						.Select(x => new XElement(
							TNS + XMLNames.Report_ResultEntry_GearshiftCount,
							(x.Key != Constants.NOT_IN_AXLE_POWERTRAIN) ? new XAttribute("axleNumber", x.Key) : null,
							x.Value.ToXMLFormat(0)))
						.ToArray()
					: new[] { new XElement(TNS + XMLNames.Report_ResultEntry_GearshiftCount, ((double)0).ToXMLFormat(0)) },
				new XElement(TNS + XMLNames.Report_ResultEntry_EngineSpeedDriving,
					new XElement(TNS + XMLNames.Report_ResultEntry_EngineSpeedDriving_Min, entry.EngineSpeedDrivingMin.ValueAsUnit(XMLNames.Unit_RPM, 1)),
					new XElement(TNS + XMLNames.Report_ResultEntry_EngineSpeedDriving_Avg, entry.EngineSpeedDrivingAvg.ValueAsUnit(XMLNames.Unit_RPM, 1)),
					new XElement(TNS + XMLNames.Report_ResultEntry_EngineSpeedDriving_Max, entry.EngineSpeedDrivingMax.ValueAsUnit(XMLNames.Unit_RPM, 1))
				),
                (entry.AverageGearboxEfficiency.Count() > 0)
					? entry.AverageGearboxEfficiency
						.Select(x => new XElement(
							TNS + XMLNames.Report_Results_AverageGearboxEfficiency,
							(x.Key != Constants.NOT_IN_AXLE_POWERTRAIN) ? new XAttribute("axleNumber", x.Key) : null,
							x.Value.ValueAsUnit(XMLNames.UnitPercent, 2)))
						.ToArray()
                    : new[] { new XElement(TNS + XMLNames.Report_Results_AverageGearboxEfficiency, double.NaN.ValueAsUnit(XMLNames.UnitPercent, 2)) },
                (entry.AverageAxlegearEfficiency.Count() > 0)
                    ? entry.AverageAxlegearEfficiency
                        .Select(x => new XElement(
                            TNS + XMLNames.Report_Results_AverageAxlegearEfficiency,
                            (x.Key != Constants.NOT_IN_AXLE_POWERTRAIN) ? new XAttribute("axleNumber", x.Key) : null,
                            x.Value.ValueAsUnit(XMLNames.UnitPercent, 2)))
                        .ToArray()
                    : new[] { new XElement(TNS + XMLNames.Report_Results_AverageAxlegearEfficiency, double.NaN.ValueAsUnit(XMLNames.UnitPercent, 2)) }
			);
		}

		public override XElement GetElement(IOVCResultEntry entry)
		{
			var weighted = entry.Weighted;
			if (weighted.Status == VectoRun.Status.PrimaryBusSimulationIgnore) {
				return new XElement(TNS + XMLNames.Report_ResultEntry_VehiclePerformance,
					new XElement(TNS + XMLNames.Report_ResultEntry_AverageSpeed, new ConvertedSI(double.NaN, XMLNames.Unit_kmph).ValueAsUnit()),
					new XElement(TNS + XMLNames.Report_ResultEntry_AvgDrivingSpeed, new ConvertedSI(double.NaN, XMLNames.Unit_kmph).ValueAsUnit())
				);
            }
			return new XElement(TNS + XMLNames.Report_ResultEntry_VehiclePerformance,
				new XElement(TNS + XMLNames.Report_ResultEntry_AverageSpeed, weighted.AverageSpeed.ValueAsUnit("km/h", 1)),
				new XElement(TNS + XMLNames.Report_ResultEntry_AvgDrivingSpeed, weighted.AverageDrivingSpeed.ValueAsUnit("km/h", 1))
			);
		}

		#endregion
	}

	public class VehiclePerformancePEVMRFWriter : AbstractResultGroupWriter
	{
		public VehiclePerformancePEVMRFWriter(ICommonResultsWriterFactory factory, XNamespace ns) : base(factory, ns) { }

		#region Overrides of AbstractResultGroupWriter

		public override XElement GetElement(IResultEntry entry)
		{
			if (entry.Status == VectoRun.Status.PrimaryBusSimulationIgnore) {
				return new XElement(TNS + XMLNames.Report_ResultEntry_VehiclePerformance,
					new XElement(TNS + XMLNames.Report_ResultEntry_AverageSpeed, new ConvertedSI(double.NaN, XMLNames.Unit_kmph).ValueAsUnit()),
					new XElement(TNS + XMLNames.Report_ResultEntry_AvgDrivingSpeed, new ConvertedSI(double.NaN, XMLNames.Unit_kmph).ValueAsUnit()),
					new XElement(TNS + XMLNames.Report_ResultEntry_MinSpeed, new ConvertedSI(double.NaN, XMLNames.Unit_kmph).ValueAsUnit()),
					new XElement(TNS + XMLNames.Report_ResultEntry_MaxSpeed, new ConvertedSI(double.NaN, XMLNames.Unit_kmph).ValueAsUnit()),
					new XElement(TNS + XMLNames.Report_ResultEntry_MaxDeceleration, new ConvertedSI(double.NaN, XMLNames.Unit_mps2).ValueAsUnit()),
					new XElement(TNS + XMLNames.Report_ResultEntry_MaxAcceleration, new ConvertedSI(double.NaN, XMLNames.Unit_mps2).ValueAsUnit()),
					new XElement(TNS + XMLNames.Report_ResultEntry_FullLoadDrivingtimePercentage, 0),
					new XElement(TNS + XMLNames.Report_ResultEntry_GearshiftCount, 0),
					new XElement(TNS + XMLNames.Report_Results_AverageGearboxEfficiency, new ConvertedSI(double.NaN, XMLNames.UnitPercent).ValueAsUnit()),
					new XElement(TNS + XMLNames.Report_Results_AverageAxlegearEfficiency, new ConvertedSI(double.NaN, XMLNames.UnitPercent).ValueAsUnit())
				);
			}
            return new XElement(TNS + XMLNames.Report_ResultEntry_VehiclePerformance,
				new XElement(TNS + XMLNames.Report_ResultEntry_AverageSpeed, entry.AverageSpeed.ValueAsUnit(XMLNames.Unit_kmph, 1)),
				new XElement(TNS + XMLNames.Report_ResultEntry_AvgDrivingSpeed, entry.AverageDrivingSpeed.ValueAsUnit(XMLNames.Unit_kmph, 1)),
				new XElement(TNS + XMLNames.Report_ResultEntry_MinSpeed, entry.MinSpeed.ValueAsUnit(XMLNames.Unit_kmph, 1)),
				new XElement(TNS + XMLNames.Report_ResultEntry_MaxSpeed, entry.MaxSpeed.ValueAsUnit(XMLNames.Unit_kmph, 1)),
				new XElement(TNS + XMLNames.Report_ResultEntry_MaxDeceleration, entry.MaxDeceleration.ValueAsUnit(XMLNames.Unit_mps2, 2)),
				new XElement(TNS + XMLNames.Report_ResultEntry_MaxAcceleration, entry.MaxAcceleration.ValueAsUnit(XMLNames.Unit_mps2, 2)),
				new XElement(TNS + XMLNames.Report_ResultEntry_FullLoadDrivingtimePercentage, entry.FullLoadPercentage.ToXMLFormat(2)),
                (entry.GearshiftCount.Count > 0)
                    ? entry.GearshiftCount
						.Select(x => new XElement(
                            TNS + XMLNames.Report_ResultEntry_GearshiftCount,
                            (x.Key != Constants.NOT_IN_AXLE_POWERTRAIN) ? new XAttribute("axleNumber", x.Key) : null,
                            x.Value.ToXMLFormat(0)))
                        .ToArray()
                    : new[] { new XElement(TNS + XMLNames.Report_ResultEntry_GearshiftCount, ((double)0).ToXMLFormat(0)) },
                (entry.AverageGearboxEfficiency.Count() > 0)
                    ? entry.AverageGearboxEfficiency
                        .Select(x => new XElement(
                            TNS + XMLNames.Report_Results_AverageGearboxEfficiency,
                            (x.Key != Constants.NOT_IN_AXLE_POWERTRAIN) ? new XAttribute("axleNumber", x.Key) : null,
                            x.Value.ValueAsUnit(XMLNames.UnitPercent, 2)))
                        .ToArray()
                    : new[] { new XElement(TNS + XMLNames.Report_Results_AverageGearboxEfficiency, double.NaN.ValueAsUnit(XMLNames.UnitPercent, 2)) },
                (entry.AverageAxlegearEfficiency.Count() > 0)
                    ? entry.AverageAxlegearEfficiency
                        .Select(x => new XElement(
                            TNS + XMLNames.Report_Results_AverageAxlegearEfficiency,
                            (x.Key != Constants.NOT_IN_AXLE_POWERTRAIN) ? new XAttribute("axleNumber", x.Key) : null,
                            x.Value.ValueAsUnit(XMLNames.UnitPercent, 2)))
                        .ToArray()
                    : new[] { new XElement(TNS + XMLNames.Report_Results_AverageAxlegearEfficiency, double.NaN.ValueAsUnit(XMLNames.UnitPercent, 2)) }
			);
		}

		public override XElement GetElement(IOVCResultEntry entry)
		{
			var weighted = entry.Weighted;
			if (weighted.Status == VectoRun.Status.PrimaryBusSimulationIgnore) {
				return new XElement(TNS + XMLNames.Report_ResultEntry_VehiclePerformance,
					new XElement(TNS + XMLNames.Report_ResultEntry_AverageSpeed, new ConvertedSI(double.NaN, XMLNames.Unit_kmph).ValueAsUnit()),
					new XElement(TNS + XMLNames.Report_ResultEntry_AvgDrivingSpeed, new ConvertedSI(double.NaN, XMLNames.Unit_kmph).ValueAsUnit())
				);
			}
            return new XElement(TNS + XMLNames.Report_ResultEntry_VehiclePerformance,
				new XElement(TNS + XMLNames.Report_ResultEntry_AverageSpeed, weighted.AverageSpeed.ValueAsUnit(XMLNames.Unit_kmph, 1)),
				new XElement(TNS + XMLNames.Report_ResultEntry_AvgDrivingSpeed, weighted.AverageDrivingSpeed.ValueAsUnit(XMLNames.Unit_kmph, 1))
			);
		}

		#endregion
	}

    public class VehiclePerformanceFCHVMRFWriter : VehiclePerformancePEVMRFWriter
    {
        public VehiclePerformanceFCHVMRFWriter(ICommonResultsWriterFactory factory, XNamespace ns) : base(factory, ns) { }

    }

    public class MRFErrorDetailsWriter : AbstractResultWriter, IResultSequenceWriter
	{
		public MRFErrorDetailsWriter(ICommonResultsWriterFactory factory, XNamespace ns) : base(factory, ns) { }

		#region Implementation of IResultSequenceWriter

		public XElement[] GetElement(IResultEntry entry)
		{
			return new[] {
				new XElement(TNS + XMLNames.Report_Vehicle_VehicleGroup, entry.VehicleClass.ToXML()),
				new XElement(TNS + XMLNames.Report_Results_Error, entry.Error),
				new XElement(TNS + XMLNames.Report_Results_ErrorDetails, entry.StackTrace)
			};
		}

		public XElement[] GetElement(IOVCResultEntry entry)
		{
			throw new NotImplementedException();
		}

		#endregion
	}
}