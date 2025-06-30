using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Xml.Linq;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Resources;

namespace TUGraz.VectoCore.OutputData.XML.DeclarationReports.Common
{
	public abstract class AbstractResultWriter
	{

		protected static readonly XNamespace xsi = "http://www.w3.org/2001/XMLSchema-instance";

		protected ICommonResultsWriterFactory _factory;

		protected AbstractResultWriter(ICommonResultsWriterFactory factory, XNamespace ns)
		{
			_factory = factory;
			TNS = ns;
		}

		protected XNamespace TNS { get; }


		protected virtual XElement GetPrimaryBusSubGroupElement(IResultEntry entry)
		{
			if (entry.VehicleClass.IsCompletedBus())
			{
                //busSubGroup 

				if (!entry.PrimaryVehicleClass.HasValue || !entry.PrimaryVehicleClass.Value.IsPrimaryBus())
				{
					throw new VectoException($"Expected Primary Bus Class");
				}



                var primarySubGroup = entry.PrimaryVehicleClass.Value;
			
				return new XElement(TNS + XMLNames.Report_Results_PrimaryVehicleSubgroup, primarySubGroup.ToXML());
			}

			return null;
		}
	}


	public abstract class ResultWriterBase : AbstractResultGroupWriter
	{
		protected ResultWriterBase(ICommonResultsWriterFactory factory, XNamespace ns) : base(factory, ns) { }

		#region Overrides of AbstractResultGroupWriter

		public override XElement GetElement(IResultEntry entry)
		{
			return new XElement(TNS + XMLNames.Report_Result_Result,
				new XAttribute(XMLNames.Report_Result_Status_Attr, XMLNames.Report_Results_Status_Success_Val),
				new XAttribute(xsi + XMLNames.XSIType, ResultXMLType),
				_factory.GetSuccessMissionWriter(_factory, TNS).GetElement(entry),
				SimulationParameterWriter.GetElement(entry),
				GetPrimaryBusSubGroupElement(entry),
				ResultTotalWriter.GetElement(entry)
			);
		}
		#endregion

		public abstract string ResultXMLType { get; }

		public abstract IResultGroupWriter SimulationParameterWriter { get; }
		public abstract IResultGroupWriter ResultTotalWriter { get; }
	}

	public class LorryConvResultWriter : ResultWriterBase
	{

		public LorryConvResultWriter(ICommonResultsWriterFactory factory, XNamespace ns) : base(factory, ns) { }

		public override string ResultXMLType => "ResultSuccessConventionalType";
		public override IResultGroupWriter SimulationParameterWriter => _factory.GetLorrySimulationParameterWriter(_factory, TNS);
		public override IResultGroupWriter ResultTotalWriter => _factory.GetLorryConvTotalWriter(_factory, TNS);


	}

	public class LorryHEVNonOVCResultWriter : ResultWriterBase
	{

		public LorryHEVNonOVCResultWriter(ICommonResultsWriterFactory factory, XNamespace ns) : base(factory, ns) { }

		public override string ResultXMLType => "ResultSuccessNonOVCHEVType";
		public override IResultGroupWriter SimulationParameterWriter => _factory.GetLorrySimulationParameterWriter(_factory, TNS);
		public override IResultGroupWriter ResultTotalWriter => _factory.GetLorryHEVNonOVCTotalWriter(_factory, TNS);


	}

	
	public class LorryFCHVNonOVCResultWriter : ResultWriterBase
    {

		public LorryFCHVNonOVCResultWriter(ICommonResultsWriterFactory factory, XNamespace ns) : base(factory, ns) { }

		public override XElement GetElement(IResultEntry entry)
		{
			return new XElement(TNS + XMLNames.Report_Result_Result,
				new XAttribute(XMLNames.Report_Result_Status_Attr, XMLNames.Report_Results_Status_Success_Val),
				new XAttribute(xsi + XMLNames.XSIType, ResultXMLType),
				_factory.GetSuccessMissionWriter(_factory, TNS).GetElement(entry),
				SimulationParameterWriter.GetElement(entry),
				GetPrimaryBusSubGroupElement(entry),
				_factory.GetLorryFCHVOVCResultWriterChargeSustaining(_factory, TNS).GetElement(entry)
				//ResultTotalWriter.GetElement(entry)
			);
		}

        public override IResultGroupWriter SimulationParameterWriter => _factory.GetLorrySimulationParameterWriter(_factory, TNS);
		public override IResultGroupWriter ResultTotalWriter => _factory.GetLorryFCHVNonOVCTotalWriter(_factory, TNS);

		public override string ResultXMLType => "ResultSuccessFCHVType";
    }

	public class LorryFCHVOVCResultWriter : ResultWriterBase
    {

		public LorryFCHVOVCResultWriter(ICommonResultsWriterFactory factory, XNamespace ns) : base(factory, ns) { }

		#region Implementation of IResultGroupWriter

		public override XElement GetElement(IResultEntry entry)
		{
            throw new NotImplementedException();
        }

		public override XElement GetElement(IOVCResultEntry entry)
		{
			return new XElement(TNS + XMLNames.Report_Result_Result,
				new XAttribute(XMLNames.Report_Result_Status_Attr, XMLNames.Report_Results_Status_Success_Val),
				new XAttribute(xsi + XMLNames.XSIType, ResultXMLType),
				_factory.GetSuccessMissionWriter(_factory, TNS).GetElement(entry.ChargeDepletingResult),
				_factory.GetLorrySimulationParameterWriter(_factory, TNS).GetElement(entry.ChargeDepletingResult),
				_factory.GetLorryFCHVOVCResultWriterChargeDepleting(_factory, TNS).GetElement(entry.ChargeDepletingResult),
				_factory.GetLorryFCHVOVCResultWriterChargeSustaining(_factory, TNS).GetElement(entry.ChargeSustainingResult)
			);
		}

		#endregion

		public override IResultGroupWriter SimulationParameterWriter { get; }
		public override IResultGroupWriter ResultTotalWriter { get; }

		public override string ResultXMLType => "ResultSuccessFCHVType";
    }

    public class LorryPEVResultWriter : ResultWriterBase
	{

		public LorryPEVResultWriter(ICommonResultsWriterFactory factory, XNamespace ns) : base(factory, ns) { }

		public override string ResultXMLType => "ResultSuccessPEVType";
		public override IResultGroupWriter SimulationParameterWriter => _factory.GetLorrySimulationParameterWriter(_factory, TNS);
		public override IResultGroupWriter ResultTotalWriter => _factory.GetLorryPEVTotalWriter(_factory, TNS);
	}

	public class LorryHEVOVCResultWriter : AbstractResultGroupWriter
	{

		public LorryHEVOVCResultWriter(ICommonResultsWriterFactory factory, XNamespace ns) : base(factory, ns) { }

        #region Implementation of IResultGroupWriter

		[ExcludeFromCodeCoverage()] // OVC writing class is never used for writing non-OVC
		public override XElement GetElement(IResultEntry entry)
		{
			throw new NotImplementedException();
		}

		public override XElement GetElement(IOVCResultEntry entry)
		{
			return new XElement(TNS + XMLNames.Report_Result_Result,
				new XAttribute(XMLNames.Report_Result_Status_Attr, XMLNames.Report_Results_Status_Success_Val),
				new XAttribute(xsi + XMLNames.XSIType, ResultXMLType),
				_factory.GetSuccessMissionWriter(_factory, TNS).GetElement(entry.ChargeDepletingResult),
				_factory.GetLorrySimulationParameterWriter(_factory, TNS).GetElement(entry.ChargeDepletingResult),
				_factory.GetLorryHEVOVCResultWriterChargeDepleting(_factory, TNS).GetElement(entry.ChargeDepletingResult),
				_factory.GetLorryHEVOVCResultWriterChargeSustaining(_factory, TNS).GetElement(entry.ChargeSustainingResult),
				_factory.GetLorryHEVOVCTotalWriter(_factory, TNS).GetElement(entry)
			);
		}

		#endregion

		protected virtual string ResultXMLType => "ResultSuccessOVCHEVType";
	}

	public class LorryHEVOVCChargeDepletingWriter : AbstractResultGroupWriter
	{
		public LorryHEVOVCChargeDepletingWriter(ICommonResultsWriterFactory factory, XNamespace ns) : base(factory, ns) { }

		#region Overrides of AbstractResultWriter

		public override XElement GetElement(IResultEntry entry)
		{
			return new XElement(TNS + XMLNames.Report_Results_OVCMode,
				new XAttribute(XMLNames.Results_Report_OVCModeAttr, XMLNames.Results_Report_OVCModeAttr_ChargeDepleting),
				_factory.GetVehiclePerformanceLorry(_factory, TNS).GetElement(entry),
				entry.FuelData.Select(f =>
					_factory.GetFuelConsumptionLorry(_factory, TNS).GetElement(entry, entry.FuelConsumptionFinal(f.FuelType))),
				_factory.GetElectricEnergyConsumptionLorry(_factory, TNS).GetElement(entry),
				_factory.GetCO2ResultLorry(_factory, TNS).GetElements(entry)
			);
		}


		#endregion
	}

	public class LorryHEVOVCChargeSustainingWriter : AbstractResultGroupWriter
	{
		public LorryHEVOVCChargeSustainingWriter(ICommonResultsWriterFactory factory, XNamespace ns) : base(factory, ns) { }

		#region Overrides of AbstractResultWriter

		public override XElement GetElement(IResultEntry entry)
		{
			return new XElement(TNS + XMLNames.Report_Results_OVCMode,
				new XAttribute(XMLNames.Results_Report_OVCModeAttr, XMLNames.Results_Report_OVCModeAttr_ChargeSustaining),
				_factory.GetVehiclePerformanceLorry(_factory, TNS).GetElement(entry),
				entry.FuelData.Select(f =>
					_factory.GetFuelConsumptionLorry(_factory, TNS).GetElement(entry, entry.FuelConsumptionFinal(f.FuelType))),
				_factory.GetCO2ResultLorry(_factory, TNS).GetElements(entry)
			);
		}

		#endregion
	}

	public class LorryFCHVOVCChargeDepletingWriter : AbstractResultGroupWriter
	{
		public LorryFCHVOVCChargeDepletingWriter(ICommonResultsWriterFactory factory, XNamespace ns) : base(factory, ns) { }

		#region Overrides of AbstractResultGroupWriter

		public override XElement GetElement(IResultEntry entry)
		{
			return new XElement(TNS + XMLNames.Report_Results_OVCMode,
				new XAttribute(XMLNames.Results_Report_OVCModeAttr, XMLNames.Results_Report_OVCModeAttr_ChargeDepleting),
				_factory.GetVehiclePerformanceFCHVLorry(_factory, TNS).GetElement(entry),
				entry.FuelData.Select(f =>
					_factory.GetFuelConsumptionLorry(_factory, TNS).GetElement(entry, entry.FuelConsumptionFinal(f.FuelType))),
				_factory.GetElectricEnergyConsumptionLorry(_factory, TNS).GetElement(entry),
                _factory.GetHydrogenRangeWriter(_factory, TNS).GetElements(entry)
			);
        }

		#endregion
	}

	public class LorryFCHVOVCChargeSustainingWriter : AbstractResultGroupWriter
	{
		public LorryFCHVOVCChargeSustainingWriter(ICommonResultsWriterFactory factory, XNamespace ns) : base(factory, ns) { }

		#region Overrides of AbstractResultGroupWriter

		public override XElement GetElement(IResultEntry entry)
		{
			return new XElement(TNS + XMLNames.Report_Results_OVCMode,
				new XAttribute(XMLNames.Results_Report_OVCModeAttr, XMLNames.Results_Report_OVCModeAttr_ChargeSustaining),
				_factory.GetVehiclePerformanceFCHVLorry(_factory, TNS).GetElement(entry),
				entry.FuelData.Select(f =>
					_factory.GetFuelConsumptionLorry(_factory, TNS).GetElement(entry, entry.FuelConsumptionFinal(f.FuelType))),
				_factory.GetHydrogenRangeWriter(_factory, TNS).GetElements(entry)
			);
        }

		#endregion
	}

	// ----- bus

    public class BusConvMRFResultWriter : ResultWriterBase
	{
		public BusConvMRFResultWriter(ICommonResultsWriterFactory factory, XNamespace ns) : base(factory, ns) { }

		#region Overrides of CIFResultWriterBase

		public override string ResultXMLType => "ResultSuccessConventionalType";
		public override IResultGroupWriter SimulationParameterWriter => _factory.GetBusSimulationParameterWriter(_factory, TNS);
		public override IResultGroupWriter ResultTotalWriter => _factory.GetBusConvTotalWriter(_factory, TNS);

		#endregion
	}

	public class BusConvCIFResultWriter : BusConvMRFResultWriter
    {
		public BusConvCIFResultWriter(ICommonResultsWriterFactory factory, XNamespace ns) : base(factory, ns) { }

		protected override XElement GetPrimaryBusSubGroupElement(IResultEntry entry)
		{
			return null;
		}
	}

    public class BusHEVNonOVCMRFResultWriter : ResultWriterBase
	{
		public BusHEVNonOVCMRFResultWriter(ICommonResultsWriterFactory factory, XNamespace ns) : base(factory, ns) { }

		#region Overrides of CIFResultWriterBase

		public override string ResultXMLType => "ResultSuccessNonOVCHEVType";
		public override IResultGroupWriter SimulationParameterWriter => _factory.GetBusSimulationParameterWriter(_factory, TNS);
		public override IResultGroupWriter ResultTotalWriter => _factory.GetBusHEVNonOVCTotalWriter(_factory, TNS);

		#endregion
	}

	public class BusHEVNonOVCCIFResultWriter : BusHEVNonOVCMRFResultWriter
    {
		public BusHEVNonOVCCIFResultWriter(ICommonResultsWriterFactory factory, XNamespace ns) : base(factory, ns) { }

		protected override XElement GetPrimaryBusSubGroupElement(IResultEntry entry)
		{
			return null;
		}
    }

	public class BusPEVMRFResultWriter : ResultWriterBase
	{
		public BusPEVMRFResultWriter(ICommonResultsWriterFactory factory, XNamespace ns) : base(factory, ns) { }

		#region Overrides of CIFResultWriterBase

		public override string ResultXMLType => "ResultSuccessPEVType";
		public override IResultGroupWriter SimulationParameterWriter => _factory.GetBusSimulationParameterWriter(_factory, TNS);
		public override IResultGroupWriter ResultTotalWriter => _factory.GetBusPEVTotalWriter(_factory, TNS);

		#endregion
	}

	public class BusPEVCIFResultWriter : BusPEVMRFResultWriter
    {
		public BusPEVCIFResultWriter(ICommonResultsWriterFactory factory, XNamespace ns) : base(factory, ns) { }

		protected override XElement GetPrimaryBusSubGroupElement(IResultEntry entry)
		{
			return null;
		}
    }

	public class BusHEVOVCMRFResultWriter : AbstractResultGroupWriter
	{

		public BusHEVOVCMRFResultWriter(ICommonResultsWriterFactory factory, XNamespace ns) : base(factory, ns) { }

        #region Implementation of IResultGroupWriter

		[ExcludeFromCodeCoverage()] // OVC writing class is never used for writing non-OVC
		public override XElement GetElement(IResultEntry entry)
		{
			throw new NotImplementedException();
		}

		public override XElement GetElement(IOVCResultEntry entry)
		{
			var primarySubGroup = entry.ChargeDepletingResult.VehicleClass;
			
			return new XElement(TNS + XMLNames.Report_Result_Result,
				new XAttribute(XMLNames.Report_Result_Status_Attr, XMLNames.Report_Results_Status_Success_Val),
				new XAttribute(xsi + XMLNames.XSIType, ResultXMLType),
				_factory.GetSuccessMissionWriter(_factory, TNS).GetElement(entry.ChargeDepletingResult),
				_factory.GetBusSimulationParameterWriter(_factory, TNS).GetElement(entry.ChargeDepletingResult),
				GetPrimaryBusSubGroupElement(entry.ChargeSustainingResult),
				_factory.GetBusHEVOVCResultWriterChargeDepleting(_factory, TNS).GetElement(entry.ChargeDepletingResult),
				_factory.GetBusHEVOVCResultWriterChargeSustaining(_factory, TNS).GetElement(entry.ChargeSustainingResult),
				_factory.GetBusHEVOVCTotalWriter(_factory, TNS).GetElement(entry)
			);
		}

		#endregion

		protected virtual string ResultXMLType => "ResultSuccessOVCHEVType";
	}

	public class BusHEVOVCCIFResultWriter : BusHEVOVCMRFResultWriter
    {

		public BusHEVOVCCIFResultWriter(ICommonResultsWriterFactory factory, XNamespace ns) : base(factory, ns) { }

		protected override XElement GetPrimaryBusSubGroupElement(IResultEntry entry)
		{
			return null;
		}

    }

	public class BusOVCChargeDepletingWriter : AbstractResultGroupWriter
	{
		public BusOVCChargeDepletingWriter(ICommonResultsWriterFactory factory, XNamespace ns) : base(factory, ns) { }

		#region Overrides of AbstractResultWriter

		public override XElement GetElement(IResultEntry entry)
		{
			return new XElement(TNS + XMLNames.Report_Results_OVCMode,
				new XAttribute(XMLNames.Results_Report_OVCModeAttr, XMLNames.Results_Report_OVCModeAttr_ChargeDepleting),
				_factory.GetVehiclePerformanceBus(_factory, TNS).GetElement(entry),
				entry.FuelData.Select(f =>
					_factory.GetFuelConsumptionBus(_factory, TNS).GetElement(entry, entry.FuelConsumptionFinal(f.FuelType))),
				_factory.GetElectricEnergyConsumptionBus(_factory, TNS).GetElement(entry),
				_factory.GetCO2ResultBus(_factory, TNS).GetElements(entry)
			);
		}
		#endregion
	}

	public class BusOVCChargeSustainingWriter : AbstractResultGroupWriter
	{
		public BusOVCChargeSustainingWriter(ICommonResultsWriterFactory factory, XNamespace ns) : base(factory, ns) { }

		#region Overrides of AbstractResultWriter

		public override XElement GetElement(IResultEntry entry)
		{
			return new XElement(TNS + XMLNames.Report_Results_OVCMode,
				new XAttribute(XMLNames.Results_Report_OVCModeAttr, XMLNames.Results_Report_OVCModeAttr_ChargeSustaining),
				_factory.GetVehiclePerformanceBus(_factory, TNS).GetElement(entry),
				//new XElement(TNS + XMLNames.Report_ResultEntry_AverageSpeed, XMLHelper.ValueAsUnit(entry.AverageSpeed, XMLNames.Unit_kmph, 1)),
				entry.FuelData.Select(f =>
					_factory.GetFuelConsumptionBus(_factory, TNS).GetElement(entry, entry.FuelConsumptionFinal(f.FuelType))),
				_factory.GetCO2ResultBus(_factory, TNS).GetElements(entry)
			);
		}

		#endregion
	}

    public class BusFCHVNonOVCResultWriter : ResultWriterBase
    {

        public BusFCHVNonOVCResultWriter(ICommonResultsWriterFactory factory, XNamespace ns) : base(factory, ns) { }

        public override XElement GetElement(IResultEntry entry)
        {
            return new XElement(TNS + XMLNames.Report_Result_Result,
                new XAttribute(XMLNames.Report_Result_Status_Attr, XMLNames.Report_Results_Status_Success_Val),
                new XAttribute(xsi + XMLNames.XSIType, ResultXMLType),
                _factory.GetSuccessMissionWriter(_factory, TNS).GetElement(entry),
                SimulationParameterWriter.GetElement(entry),
                GetPrimaryBusSubGroupElement(entry),
                _factory.GetBusFCHVOVCResultWriterChargeSustaining(_factory, TNS).GetElement(entry)
            //ResultTotalWriter.GetElement(entry)
            );
        }

        public override IResultGroupWriter SimulationParameterWriter => _factory.GetLorrySimulationParameterWriter(_factory, TNS);
        public override IResultGroupWriter ResultTotalWriter => _factory.GetLorryFCHVNonOVCTotalWriter(_factory, TNS);

        public override string ResultXMLType => "ResultSuccessFCHVType";
    }

    public class BusFCHVOVCResultWriter : ResultWriterBase
    {

        public BusFCHVOVCResultWriter(ICommonResultsWriterFactory factory, XNamespace ns) : base(factory, ns) { }

        #region Implementation of IResultGroupWriter

        public override XElement GetElement(IResultEntry entry)
        {
            throw new NotImplementedException();
        }

        public override XElement GetElement(IOVCResultEntry entry)
        {
            return new XElement(TNS + XMLNames.Report_Result_Result,
                new XAttribute(XMLNames.Report_Result_Status_Attr, XMLNames.Report_Results_Status_Success_Val),
                new XAttribute(xsi + XMLNames.XSIType, ResultXMLType),
                _factory.GetSuccessMissionWriter(_factory, TNS).GetElement(entry.ChargeDepletingResult),
                _factory.GetBusSimulationParameterWriter(_factory, TNS).GetElement(entry.ChargeDepletingResult),
                _factory.GetBusFCHVOVCResultWriterChargeDepleting(_factory, TNS).GetElement(entry.ChargeDepletingResult),
                _factory.GetBusFCHVOVCResultWriterChargeSustaining(_factory, TNS).GetElement(entry.ChargeSustainingResult)
            );
        }

        #endregion

        public override IResultGroupWriter SimulationParameterWriter { get; }
        public override IResultGroupWriter ResultTotalWriter { get; }

        public override string ResultXMLType => "ResultSuccessFCHVType";
    }


	public class BusFCHVOVCChargeDepletingWriter : AbstractResultGroupWriter
	{
		public BusFCHVOVCChargeDepletingWriter(ICommonResultsWriterFactory factory, XNamespace ns) : base(factory, ns) { }

		#region Overrides of AbstractResultGroupWriter

		public override XElement GetElement(IResultEntry entry)
		{
			return new XElement(TNS + XMLNames.Report_Results_OVCMode,
				new XAttribute(XMLNames.Results_Report_OVCModeAttr, XMLNames.Results_Report_OVCModeAttr_ChargeDepleting),
				_factory.GetVehiclePerformanceFCHVBus(_factory, TNS).GetElement(entry),
				entry.FuelData.Select(f =>
					_factory.GetFuelConsumptionBus(_factory, TNS).GetElement(entry, entry.FuelConsumptionFinal(f.FuelType))),
				_factory.GetElectricEnergyConsumptionBus(_factory, TNS).GetElement(entry),
				_factory.GetHydrogenRangeWriter(_factory, TNS).GetElements(entry)
			);
		}

		#endregion
	}

	public class BusFCHVOVCChargeSustainingWriter : AbstractResultGroupWriter
	{
		public BusFCHVOVCChargeSustainingWriter(ICommonResultsWriterFactory factory, XNamespace ns) : base(factory, ns) { }

		#region Overrides of AbstractResultGroupWriter

		public override XElement GetElement(IResultEntry entry)
		{
			return new XElement(TNS + XMLNames.Report_Results_OVCMode,
				new XAttribute(XMLNames.Results_Report_OVCModeAttr, XMLNames.Results_Report_OVCModeAttr_ChargeSustaining),
				_factory.GetVehiclePerformanceFCHVBus(_factory, TNS).GetElement(entry),
				entry.FuelData.Select(f =>
					_factory.GetFuelConsumptionBus(_factory, TNS).GetElement(entry, entry.FuelConsumptionFinal(f.FuelType))),
				_factory.GetHydrogenRangeWriter(_factory, TNS).GetElements(entry)
			);
		}

		#endregion
	}
}