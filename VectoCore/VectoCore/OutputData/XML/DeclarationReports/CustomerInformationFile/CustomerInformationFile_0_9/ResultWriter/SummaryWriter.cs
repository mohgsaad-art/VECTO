using System;
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
		private MissionType[] vocationalMissions = { MissionType.Construction, MissionType.MunicipalUtility };

		private static readonly MissionType[] _ehcMissions = { MissionType.LongHaulEMS, MissionType.RegionalDelivery };

		private static readonly Dictionary<VehicleClass, WeightingGroup> _ehcWeightingGroups = new Dictionary<VehicleClass, WeightingGroup>
		{
			{ VehicleClass.Class11, WeightingGroup.Group11EHC },
			{ VehicleClass.Class12, WeightingGroup.Group12EHC },
			{ VehicleClass.Class16, WeightingGroup.Group16EHC },
		};

		protected SummaryWriterBase(ICIFResultsWriterFactory factory, XNamespace ns) : base(factory, ns) { }

		#region Implementation of ICifSummaryWriter

		public XElement[] GetElement(IList<IResultEntry> entries)
		{
			var weighted = DeclarationData.CalculateWeightedSummary(entries.Where(e => !vocationalMissions.Contains(e.Mission) && !_ehcMissions.Contains(e.Mission)).ToList());
			var weightedVocationals = DeclarationData.CalculateWeightedSummary(entries.Where(e => vocationalMissions.Contains(e.Mission)).ToList());

			// For Group 16 vehicles normal weights is null. The report needs both always.
			bool isClass16 = entries.All(e => e.VehicleClass == VehicleClass.Class16);
			weighted = isClass16 && weighted == null ? weightedVocationals : weighted;

			IWeightedResult weightedEHC = GetEHCWeightedResult(entries);

			return DoGetElement(weighted, weightedVocationals, weightedEHC);
		}

		public XElement[] GetElement(IList<IOVCResultEntry> entries)
		{
			var weighted = DeclarationData.CalculateWeightedSummary(entries.Where(e => !vocationalMissions.Contains(e.ChargeDepletingResult.Mission) && !_ehcMissions.Contains(e.ChargeDepletingResult.Mission)).ToList());
			var weightedVocationals = DeclarationData.CalculateWeightedSummary(entries.Where(e => vocationalMissions.Contains(e.ChargeDepletingResult.Mission)).ToList());

			// For Group 16 vehicles normal weights is null. The report needs both always.
			bool isClass16 = entries.All(e => e.ChargeDepletingResult.VehicleClass == VehicleClass.Class16);
			weighted = isClass16 && weighted == null ? weightedVocationals : weighted;

			var weightedEHC = GetEHCWeightedResult(
				entries.Where(e => _ehcMissions.Contains(e.ChargeDepletingResult.Mission))
					.Select(e => e.ChargeDepletingResult)
					.ToList());

			return DoGetElement(weighted, weightedVocationals, weightedEHC);
		}

		protected virtual XElement[] DoGetElement(IWeightedResult weighted, IWeightedResult weightedVocationals, IWeightedResult weightedEHC = null)
		{
			bool isVocational = false;
			List<XElement> results = new List<XElement>();
			foreach (IWeightedResult weightedResult in new[] { weighted, weightedVocationals, weightedEHC })
			{
				if (weightedResult == null)
				{
					results.Add(null);
					isVocational = !isVocational;
					continue;
				}

				XElement element = new XElement(TNS + XMLNames.Report_Results_Summary,
					new XAttribute(xsi + XMLNames.XSIType, ResultSummaryXMLType),
					new XElement(TNS + XMLNames.XMLCustomerReportIsVocational, isVocational),
					GetSummary(weightedResult),
					FuelConsumptionWriter?.GetElements(weightedResult),
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

		private IWeightedResult GetEHCWeightedResult(IList<IResultEntry> ehcCandidates)
		{
			IWeightedResult weightedEHC = null;
			var ehcEntries = ehcCandidates.Where(e => _ehcMissions.Contains(e.Mission)).ToList();
			if (ehcEntries.Any() && _ehcWeightingGroups.TryGetValue(ehcEntries.First().VehicleClass, out var ehcGroup))
			{
				var ehcFactors = DeclarationData.WeightingFactors.Lookup(ehcGroup);
				weightedEHC = DeclarationData.CalculateWeightedSummary(ehcEntries, ehcFactors);
				weightedEHC = ApplyEHCCO2Interpolation(ehcEntries, weightedEHC, ehcFactors);
			}

			return weightedEHC;
		}

		/// <summary>
		/// Overrides the CO2PerMeter and Payload in the EHC weighted result.
		/// Declared payloads (x-axis) are fetched from all SegmentTable rows matching the vehicle class,
		/// finding the row whose LongHaulEMS declared payloads match the simulation entries.
		/// CO2 is computed via linear interpolation: CO2 = m*(X - x1) + y1 where m = (y0-y1)/(x0-x1).
		/// The EHC weighted payload and CO2 are computed from SegmentTable declared values.
		/// </summary>
		private static IWeightedResult ApplyEHCCO2Interpolation(
			IList<IResultEntry> ehcEntries,
			IWeightedResult weightedEHC,
			IDictionary<Tuple<MissionType, LoadingType>, double> ehcFactors)
		{
			if (weightedEHC == null) return null;

			var lel = ehcEntries.FirstOrDefault(e => e.Mission == MissionType.LongHaulEMS && e.LoadingType == LoadingType.LowLoading);
			var ler = ehcEntries.FirstOrDefault(e => e.Mission == MissionType.LongHaulEMS && e.LoadingType == LoadingType.ReferenceLoad);
			var rdl = ehcEntries.FirstOrDefault(e => e.Mission == MissionType.RegionalDelivery && e.LoadingType == LoadingType.LowLoading);
			var rdr = ehcEntries.FirstOrDefault(e => e.Mission == MissionType.RegionalDelivery && e.LoadingType == LoadingType.ReferenceLoad);

			var weightFactors = new Dictionary<string, double>
			{
				{ "rdl", ehcFactors[Tuple.Create(MissionType.RegionalDelivery, LoadingType.LowLoading)] },
				{ "rdr", ehcFactors[Tuple.Create(MissionType.RegionalDelivery, LoadingType.ReferenceLoad)] },
				{ "lel", ehcFactors[Tuple.Create(MissionType.LongHaulEMS, LoadingType.LowLoading)] },
				{ "ler", ehcFactors[Tuple.Create(MissionType.LongHaulEMS, LoadingType.ReferenceLoad)] }
			};

			if (lel == null || ler == null || lel.Distance.IsEqual(0) || ler.Distance.IsEqual(0))
				return weightedEHC;

			// Fetch EHC SegmentTable row for this vehicle class to get declared payloads
			var ehcSegment = DeclarationData.TruckSegments.GetEHCSegment(lel.VehicleClass);
			if (!ehcSegment.Found)
				return weightedEHC;

			var ehcPayload = ehcSegment.Missions.First(m => m.MissionType == MissionType.LongHaulEMS).RefLoad.Cast<Kilogram>();

			var x0 = lel.Payload;
			var x1 = ler.Payload;

			if ((x1 - x0).IsEqual(0))
				return weightedEHC;

			var fcPerMeter = new Dictionary<IFuelProperties, KilogramPerMeter>();
			var fcTotal = new Dictionary<IFuelProperties, Kilogram>();

			foreach (var fuel in lel.FuelData)
			{
				var y0 = lel.FuelConsumptionFinal(fuel.FuelType)?.FC_FINAL_KM;
				var y1 = ler.FuelConsumptionFinal(fuel.FuelType)?.FC_FINAL_KM;

				if (y0 == null || y1 == null) continue;

				var ehcFC = VectoMath.Interpolate(
					x0.Value(),
					x1.Value(),
					y0.Value(),
					y1.Value(),
					ehcPayload.Value()).SI<KilogramPerMeter>();

				var weightedFCPerKm = GetEHCWeightedValue(
					ehcSegment,
					rdl?.FuelConsumptionFinal(fuel.FuelType)?.FC_FINAL_KM.Value(),
					rdr?.FuelConsumptionFinal(fuel.FuelType)?.FC_FINAL_KM.Value(),
					lel.FuelConsumptionFinal(fuel.FuelType).FC_FINAL_KM.Value(),
					ehcFC.Value(),
					weightFactors)?.SI<KilogramPerMeter>();

				var rdlFcTotal = rdl == null ? null : rdl.FuelConsumptionFinal(fuel.FuelType)?.FC_FINAL_KM * rdl.Distance;
				var rdrFcTotal = rdr == null ? null : rdr.FuelConsumptionFinal(fuel.FuelType)?.FC_FINAL_KM * rdr.Distance;
				var weightedFCTotal = GetEHCWeightedValue(
					ehcSegment,
					rdlFcTotal?.Value(),
					rdrFcTotal?.Value(),
					(lel.FuelConsumptionFinal(fuel.FuelType).FC_FINAL_KM * lel.Distance).Value(),
					(ehcFC * ler.Distance).Value(),
					weightFactors)?.SI<Kilogram>();

				if (weightedFCPerKm != null) fcPerMeter[fuel] = weightedFCPerKm;
				if (weightedFCTotal != null) fcTotal[fuel] = weightedFCTotal;
			}

			var result = (WeightedResult)weightedEHC;
			if (fcPerMeter.Any())
				result.CO2PerMeter = fcPerMeter.Select((k) => k.Value * k.Key.CO2PerFuelWeight).Sum();
			result.FuelConsumptionPerMeter = fcPerMeter;
			result.FuelConsumption = fcTotal;
			result.Payload = GetEHCWeightedValue(
				ehcSegment,
				rdl?.Payload?.Value(),
				rdr?.Payload?.Value(),
				lel?.Payload?.Value(),
				ehcPayload.Value(),
				weightFactors)?.SI<Kilogram>();
			return result;
		}

		private static double? GetEHCWeightedValue(
			Segment ehcSegment,
			double? rdlValue,
			double? rdrValue,
			double? lelValue,
			double? lerValue,
			Dictionary<string, double> weights)
		{
			var rdEmsMission = ehcSegment.Missions.FirstOrDefault(m => m.MissionType == MissionType.RegionalDelivery);
			if (rdEmsMission != null && rdlValue != null && rdrValue != null)
			{
				return weights["rdl"] * rdlValue + weights["rdr"] * rdrValue + weights["lel"] * lelValue + weights["ler"] * lerValue;
			}
			else
			{
				return weights["lel"] * lelValue + weights["ler"] * lerValue;
			}
		}
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