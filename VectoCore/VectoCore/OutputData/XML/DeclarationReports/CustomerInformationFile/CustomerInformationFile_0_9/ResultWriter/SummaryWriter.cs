using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.Common;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.OutputData.XML.DeclarationReports.CustomerInformationFile.CustomerInformationFile_0_9.ResultWriter
{
    public abstract class SummaryWriterBase : AbstractResultWriter, IReportResultsSummaryWriter
	{
		private MissionType[] vocationalMissions = new[] { MissionType.Construction, MissionType.MunicipalUtility };

		protected SummaryWriterBase(ICIFResultsWriterFactory factory, XNamespace ns) : base(factory, ns) { }

		#region Implementation of ICifSummaryWriter

		public XElement[] GetElement(IList<IResultEntry> entries)
		{
			var weighted = DeclarationData.CalculateWeightedSummary(entries.Where(e => !vocationalMissions.Contains(e.Mission)).ToList());
			var weightedVocationals = DeclarationData.CalculateWeightedSummary(entries.Where(e => vocationalMissions.Contains(e.Mission)).ToList());
			return DoGetElement(weighted, weightedVocationals);
		}

		public XElement[] GetElement(IList<IOVCResultEntry> entries)
		{
			var weighted = DeclarationData.CalculateWeightedSummary(entries.Where(e => !vocationalMissions.Contains(e.ChargeDepletingResult.Mission)).ToList());
			var weightedVocationals = DeclarationData.CalculateWeightedSummary(entries.Where(e => vocationalMissions.Contains(e.ChargeDepletingResult.Mission)).ToList());
			return DoGetElement(weighted, weightedVocationals);
		}

		protected virtual XElement[] DoGetElement(IWeightedResult weighted, IWeightedResult weightedVocationals)
		{
			bool isVocational = false;
			List<XElement> results = new List<XElement>();
			foreach (IWeightedResult weightedResult in new[] { weighted, weightedVocationals })
			{
				if (weightedResult == null)
				{
					results.Add(null);
					continue;
				}

				XElement element = new XElement(TNS + XMLNames.Report_Results_Summary,
					new XAttribute(xsi + XMLNames.XSIType, ResultSummaryXMLType),
					new XElement(TNS + XMLNames.XMLCustomerReportIsVocational, isVocational),
					GetSummary(weightedResult),
					FuelConsumptionWriter != null
						? weightedResult.FuelConsumption.Select(x =>
							FuelConsumptionWriter.GetElements(weightedResult))
						: null,
					ElectricEnergyConsumptionWriter?.GetElement(weightedResult),
					CO2Writer?.GetElements(weightedResult),
					ElectricRangeWriter?.GetElements(weightedResult),
					HydrogenRangeWriter?.GetElements(weightedResult)
				);

				results.Add(element);
				isVocational = !isVocational;
			}

			return results.ToArray();
		}

		public abstract string ResultSummaryXMLType { get; }


		protected abstract XElement[] GetSummary(IWeightedResult weighted);

		protected abstract IFuelConsumptionWriter FuelConsumptionWriter { get; }

		protected abstract IElectricEnergyConsumptionWriter ElectricEnergyConsumptionWriter { get; }

		protected abstract ICO2Writer CO2Writer { get; }

		protected abstract IElectricRangeWriter ElectricRangeWriter { get; }

		protected virtual IHydrogenRangeWriter HydrogenRangeWriter => _factory.GetHydrogenRangeWriter(_factory, TNS);

        #endregion
    }

	

	public abstract class LorrySummaryWriterBase : SummaryWriterBase
	{
		protected LorrySummaryWriterBase(ICIFResultsWriterFactory factory, XNamespace ns) : base(factory, ns) { }

		protected override XElement[] GetSummary(IWeightedResult weighted)
		{
			return new[] {
				new XElement(TNS + "AveragePayload", XMLHelper.ValueAsUnit(weighted.Payload, "t", 3))
			};
		}
	}

	public class LorryConvSummaryWriter : LorrySummaryWriterBase
	{
		public LorryConvSummaryWriter(ICIFResultsWriterFactory factory, XNamespace ns) : base(factory, ns) { }

		public override string ResultSummaryXMLType => "ResultSummaryConventionalType";
		protected override IFuelConsumptionWriter FuelConsumptionWriter => _factory.GetFuelConsumptionLorry(_factory, TNS);
		protected override IElectricEnergyConsumptionWriter ElectricEnergyConsumptionWriter => null;
		protected override ICO2Writer CO2Writer => _factory.GetCO2SummaryResultLorry(_factory, TNS);
		protected override IElectricRangeWriter ElectricRangeWriter => null;
		
	}

	public class LorryHEVNonOVCSummaryWriter : LorrySummaryWriterBase
	{
		public LorryHEVNonOVCSummaryWriter(ICIFResultsWriterFactory factory, XNamespace ns) : base(factory, ns) { }

		public override string ResultSummaryXMLType => "ResultSummaryNonOVCHEVType";
		protected override IFuelConsumptionWriter FuelConsumptionWriter => _factory.GetFuelConsumptionLorry(_factory, TNS);
		protected override IElectricEnergyConsumptionWriter ElectricEnergyConsumptionWriter => null;
		protected override ICO2Writer CO2Writer => _factory.GetCO2SummaryResultLorry(_factory, TNS);
		protected override IElectricRangeWriter ElectricRangeWriter => null;
		
	}

	public class LorryHEVOVCSummaryWriter : LorrySummaryWriterBase
	{
		public LorryHEVOVCSummaryWriter(ICIFResultsWriterFactory factory, XNamespace ns) : base(factory, ns) { }

		#region Overrides of CifSummaryWriterBase

		public override string ResultSummaryXMLType => "ResultSummaryOVCHEVType";
		protected override IFuelConsumptionWriter FuelConsumptionWriter => _factory.GetFuelConsumptionLorry(_factory, TNS);
		protected override IElectricEnergyConsumptionWriter ElectricEnergyConsumptionWriter => _factory.GetElectricEnergyConsumptionLorry(_factory, TNS);
		protected override ICO2Writer CO2Writer => _factory.GetCO2SummaryResultLorry(_factory, TNS);
		protected override IElectricRangeWriter ElectricRangeWriter => _factory.GetElectricRangeWriter(_factory, TNS);

		#endregion
	}

	public class LorryFCHVNonOVCSummaryWriter : LorrySummaryWriterBase
	{
		public LorryFCHVNonOVCSummaryWriter(ICIFResultsWriterFactory factory, XNamespace ns) : base(factory, ns) { }

		#region Overrides of CifSummaryWriterBase

		public override string ResultSummaryXMLType => "ResultSummaryFCHVType";
		protected override IFuelConsumptionWriter FuelConsumptionWriter => _factory.GetFuelConsumptionLorry(_factory, TNS);
		protected override IElectricEnergyConsumptionWriter ElectricEnergyConsumptionWriter => null;
		protected override ICO2Writer CO2Writer => _factory.GetCO2SummaryResultLorry(_factory, TNS);
		protected override IElectricRangeWriter ElectricRangeWriter => null;

		#endregion

    }

	public class LorryFCHVOVCSummaryWriter : AbstractResultWriter, IReportResultsSummaryWriter
    {
		public LorryFCHVOVCSummaryWriter(ICommonResultsWriterFactory factory, XNamespace ns) : base(factory, ns) { }

        #region Implementation of IReportResultsSummaryWriter

        public virtual XElement[] GetElement(IList<IResultEntry> entries)
		{
			return new[] {
				new XElement(TNS + XMLNames.Report_Results_Summary,
					new XAttribute(xsi + XMLNames.XSIType, "ResultSummaryFCHVType"),
					new XElement(TNS + "Info", "Not applicable for OVC-FCHV"))
			};

		}

		public XElement[] GetElement(IList<IOVCResultEntry> entries)
		{
			return new[] {
				new XElement(TNS + XMLNames.Report_Results_Summary,
					new XAttribute(xsi + XMLNames.XSIType, "ResultSummaryFCHVType"),
					new XElement(TNS + "Info", "Not applicable for OVC-FCHV"))
			};
        }

		#endregion

	}


	public class LorryPEVSummaryWriter : LorrySummaryWriterBase
	{
		public LorryPEVSummaryWriter(ICIFResultsWriterFactory factory, XNamespace ns) : base(factory, ns) { }

		public override string ResultSummaryXMLType => "ResultSummaryPEVType";
		protected override IFuelConsumptionWriter FuelConsumptionWriter => null;
		protected override IElectricEnergyConsumptionWriter ElectricEnergyConsumptionWriter => _factory.GetElectricEnergyConsumptionLorry(_factory, TNS);
		protected override ICO2Writer CO2Writer => null;
		protected override IElectricRangeWriter ElectricRangeWriter => _factory.GetElectricRangeWriter(_factory, TNS);

	}

	public class LorryPEVNonOVCSummaryWriter : LorryPEVSummaryWriter
	{
		public LorryPEVNonOVCSummaryWriter(ICIFResultsWriterFactory factory, XNamespace ns) : base(factory, ns) { }

		public override string ResultSummaryXMLType => "ResultSummaryPEVNonOVCType";
	}

	// ---- bus

	public abstract class BusSummaryWriterBase : SummaryWriterBase
	{
		protected BusSummaryWriterBase(ICIFResultsWriterFactory factory, XNamespace ns) : base(factory, ns) { }

		protected override XElement[] GetSummary(IWeightedResult weighted)
		{
			return new[] {
				new XElement(TNS + "AveragePassengerCount", weighted.PassengerCount.Value.ToXMLFormat(2))
			};
		}
	}

	public class BusConvSummaryWriter : BusSummaryWriterBase
	{
		public BusConvSummaryWriter(ICIFResultsWriterFactory factory, XNamespace ns) : base(factory, ns) { }

		#region Overrides of CifSummaryWriterBase

		public override string ResultSummaryXMLType => "ResultSummaryConventionalType";
		protected override IFuelConsumptionWriter FuelConsumptionWriter => _factory.GetFuelConsumptionBus(_factory, TNS);
		protected override IElectricEnergyConsumptionWriter ElectricEnergyConsumptionWriter => null;
		protected override ICO2Writer CO2Writer => _factory.GetCO2SummaryResultBus(_factory, TNS);
		protected override IElectricRangeWriter ElectricRangeWriter => null;

		#endregion
	}

	public class BusHEVNonOVCSummaryWriter : BusSummaryWriterBase
	{
		public BusHEVNonOVCSummaryWriter(ICIFResultsWriterFactory factory, XNamespace ns) : base(factory, ns) { }

		#region Overrides of CifSummaryWriterBase

		public override string ResultSummaryXMLType => "ResultSummaryNonOVCHEVType";
		protected override IFuelConsumptionWriter FuelConsumptionWriter => _factory.GetFuelConsumptionBus(_factory, TNS);
		protected override IElectricEnergyConsumptionWriter ElectricEnergyConsumptionWriter => null;
		protected override ICO2Writer CO2Writer => _factory.GetCO2SummaryResultBus(_factory, TNS);
		protected override IElectricRangeWriter ElectricRangeWriter => null;

		#endregion
	}

	public class BusHEVOVCSummaryWriter : BusSummaryWriterBase
	{
		public BusHEVOVCSummaryWriter(ICIFResultsWriterFactory factory, XNamespace ns) : base(factory, ns) { }


		#region Overrides of CifSummaryWriterBase

		public override string ResultSummaryXMLType => "ResultSummaryOVCHEVType";
		protected override IFuelConsumptionWriter FuelConsumptionWriter => _factory.GetFuelConsumptionBus(_factory, TNS);

		protected override IElectricEnergyConsumptionWriter ElectricEnergyConsumptionWriter => _factory.GetElectricEnergyConsumptionBus(_factory, TNS);
		protected override ICO2Writer CO2Writer => _factory.GetCO2SummaryResultBus(_factory, TNS);
		protected override IElectricRangeWriter ElectricRangeWriter => _factory.GetElectricRangeWriter(_factory, TNS);

		#endregion
	}

	public class BusFCHVNonOVCSummaryWriter : BusSummaryWriterBase
    {
		public BusFCHVNonOVCSummaryWriter(ICIFResultsWriterFactory factory, XNamespace ns) : base(factory, ns) { }

		#region Overrides of CifSummaryWriterBase

		public override string ResultSummaryXMLType => "ResultSummaryFCHVType";
		protected override IFuelConsumptionWriter FuelConsumptionWriter => _factory.GetFuelConsumptionBus(_factory, TNS);
		protected override IElectricEnergyConsumptionWriter ElectricEnergyConsumptionWriter => null;
		protected override ICO2Writer CO2Writer => _factory.GetCO2SummaryResultBus(_factory, TNS);
		protected override IElectricRangeWriter ElectricRangeWriter => null;

		#endregion

	}

	public class BusFCHVOVCSummaryWriter : AbstractResultWriter, IReportResultsSummaryWriter
	{
		public BusFCHVOVCSummaryWriter(ICommonResultsWriterFactory factory, XNamespace ns) : base(factory, ns) { }

		#region Implementation of IReportResultsSummaryWriter

		public virtual XElement[] GetElement(IList<IResultEntry> entries)
		{
			return new[] {
				new XElement(TNS + XMLNames.Report_Results_Summary,
					new XAttribute(xsi + XMLNames.XSIType, "ResultSummaryFCHVType"),
					new XElement(TNS + "Info", "Not applicable for OVC-FCHV"))
			};

		}

		public XElement[] GetElement(IList<IOVCResultEntry> entries)
		{
			return new[] {
				new XElement(TNS + XMLNames.Report_Results_Summary,
					new XAttribute(xsi + XMLNames.XSIType, "ResultSummaryFCHVType"),
					new XElement(TNS + "Info", "Not applicable for OVC-FCHV"))
			};
		}

		#endregion

	}

    public class BusPEVSummaryWriter : BusSummaryWriterBase
	{
		public BusPEVSummaryWriter(ICIFResultsWriterFactory factory, XNamespace ns) : base(factory, ns) { }

		#region Overrides of CifSummaryWriterBase

		public override string ResultSummaryXMLType => "ResultSummaryPEVType";
		protected override IFuelConsumptionWriter FuelConsumptionWriter => null;

		protected override IElectricEnergyConsumptionWriter ElectricEnergyConsumptionWriter => _factory.GetElectricEnergyConsumptionBus(_factory, TNS);
		protected override ICO2Writer CO2Writer => _factory.GetCO2SummaryResultPEVBus(_factory, TNS);
		protected override IElectricRangeWriter ElectricRangeWriter => _factory.GetElectricRangeWriter(_factory, TNS);

		#endregion
	}

}