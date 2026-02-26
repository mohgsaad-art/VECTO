using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using TUGraz.VectoCommon.BusAuxiliaries;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.OutputData.ModDataPostprocessing;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.OutputData.XML.DeclarationReports.Common
{
    public abstract class FuelConsumptionWriterBase : AbstractResultWriter, IFuelConsumptionWriter
    {
        protected FuelConsumptionWriterBase(ICommonResultsWriterFactory factory, XNamespace ns) : base(factory, ns) { }

		protected virtual string FCElementName { get; } = XMLNames.Report_Results_FuelConsumption;

        #region Implementation of IFuelConsumptionWriter

        public XElement GetElement(IResultEntry entry, IFuelConsumptionCorrection fc)
        {
			if (entry.Status == VectoRun.Status.PrimaryBusSimulationIgnore) {
				return GetElementIgnore(
					fc?.TotalFuelConsumptionCorrected ?? 0.SI<Kilogram>(),
					fc?.Fuel ?? FuelData.H2,
					entry.Distance,
					entry.Payload,
					entry.CargoVolume,
					entry.PassengerCount);
			}

			bool isZeroConsumptionEntry = (fc == null || fc.Fuel.FuelType == FuelType.H2FC) && entry.OVCMode == OvcHevMode.ChargeDepleting;
			return GetElement(
				isZeroConsumptionEntry ? 0.SI<Kilogram>() : fc.TotalFuelConsumptionCorrected,
				entry.VectoRunData.JobType.IsFCHV() ? FuelData.H2 : fc.Fuel,
				entry.Distance,
				entry.Payload,
				entry.CargoVolume,
				entry.PassengerCount);
        }

		public XElement[] GetElements(IWeightedResult entry)
		{
			List<XElement> fcElements = new List<XElement>();
			foreach (var fcEntry in entry.FuelConsumptionPerMeter)
			{
				XElement element = entry.Status == VectoRun.Status.PrimaryBusSimulationIgnore
					? GetElementIgnore(fcEntry.Value, fcEntry.Key, entry.Payload, entry.CargoVolume, entry.PassengerCount)
					: GetElement(fcEntry.Value, fcEntry.Key, entry.Payload, entry.CargoVolume, entry.PassengerCount);
				fcElements.Add(element);
			}

			return fcElements.ToArray();
		}

		protected virtual XElement GetElement(Kilogram consumption, IFuelProperties fuel, Meter distance, Kilogram payLoad, CubicMeter cargoVolume, double? passengerCount)
		{
			return new XElement(TNS + XMLNames.Report_Results_Fuel,
				new XAttribute(XMLNames.Report_Results_Fuel_Type_Attr, fuel.FuelType.ToXMLFormat()),
				GetFuelConsumptionEntries(consumption, fuel, distance, payLoad, cargoVolume, passengerCount).Select(x =>
					new XElement(TNS + FCElementName, new FormattedReportValue(x).GetElement()))
			);
		}

		protected virtual XElement GetElementIgnore(Kilogram consumption, IFuelProperties fuel, Meter distance, Kilogram payLoad, CubicMeter cargoVolume, double? passengerCount)
		{
			return new XElement(TNS + XMLNames.Report_Results_Fuel,
				new XAttribute(XMLNames.Report_Results_Fuel_Type_Attr, fuel.FuelType.ToXMLFormat()),
				GetFuelConsumptionEntries(consumption, fuel, distance, payLoad, cargoVolume, passengerCount).Select(x =>
					new XElement(TNS + FCElementName,
						new FormattedReportValue(new ConvertedSI(double.NaN, x.Units)).GetElement()))
			);
		}


		protected virtual XElement GetElement(KilogramPerMeter consumption, IFuelProperties fuel, Kilogram payLoad, CubicMeter cargoVolume, double? passengerCount)
		{
			return new XElement(TNS + XMLNames.Report_Results_Fuel,
				new XAttribute(XMLNames.Report_Results_Fuel_Type_Attr, fuel.FuelType.ToXMLFormat()),
				GetFuelConsumptionEntries(consumption, fuel, payLoad, cargoVolume, passengerCount).Select(x =>
					new XElement(TNS + FCElementName, new FormattedReportValue(x).GetElement()))
			);
		}

		protected virtual XElement GetElementIgnore(KilogramPerMeter consumption, IFuelProperties fuel, Kilogram payLoad, CubicMeter cargoVolume, double? passengerCount)
		{
			return new XElement(TNS + XMLNames.Report_Results_Fuel,
				new XAttribute(XMLNames.Report_Results_Fuel_Type_Attr, fuel.FuelType.ToXMLFormat()),
				GetFuelConsumptionEntries(consumption, fuel, payLoad, cargoVolume, passengerCount).Select(x =>
					new XElement(TNS + FCElementName,
						new FormattedReportValue(new ConvertedSI(double.NaN, x.Units)).GetElement()))
			);
		}
		#endregion

		public abstract IList<ConvertedSI> GetFuelConsumptionEntries(
			Kilogram fc,
			IFuelProperties fuel,
			Meter distance,
			Kilogram payload,
			CubicMeter volume,
			double? passenger);

		public abstract IList<ConvertedSI> GetFuelConsumptionEntries(
			KilogramPerMeter fcPerMeter,
			IFuelProperties fuel,
			Kilogram payload,
			CubicMeter volume,
			double? passenger);
	}

    public class LorryFuelConsumptionWriter : FuelConsumptionWriterBase
    {
        public LorryFuelConsumptionWriter(ICommonResultsWriterFactory factory, XNamespace ns) : base(factory, ns) { }


        #region Overrides of FuelConsumptionWriterBase

        public override IList<ConvertedSI> GetFuelConsumptionEntries(Kilogram fc, IFuelProperties fuel, Meter distance, Kilogram payload, CubicMeter volume, double? passenger)
        {
			if (distance.IsEqual(0)) {
                // in some testcases only a single cycle is simulated which has a weighting of 0. consider this to generate a valid report
				return new List<ConvertedSI>() { (fc / 1.SI<Meter>()).ConvertToGrammPerKiloMeter(),};
			}

            if (fc == null) {
                return new List<ConvertedSI>();
            }

            var retVal = new List<ConvertedSI> {
                (fc / distance).ConvertToGrammPerKiloMeter(),
                (fc / distance /payload).ConvertToGrammPerTonKilometer()};
            if (volume.IsGreater(0)) {
				retVal.Add((fc / distance / volume).ConvertToGrammPerCubicMeterKiloMeter());
			}

			retVal.AddRange(new [] {
                (fc * fuel.LowerHeatingValueVecto / distance).ConvertToMegaJoulePerKilometer(),
                (fc * fuel.LowerHeatingValueVecto / distance / payload).ConvertToMegaJoulePerTonKiloMeter(),
			});
			if (volume.IsGreater(0)) {
                retVal.Add((fc * fuel.LowerHeatingValueVecto / distance / volume).ConvertToMegaJoulePerCubicMeterKiloMeter());
			}

            if (fuel.FuelDensity != null) {
                retVal.AddRange(new[] {
                    (fc / fuel.FuelDensity / distance).ConvertToLiterPer100KiloMeter(),
                    (fc / fuel.FuelDensity / distance / payload).ConvertToLiterPerTonKiloMeter(),
				});
				if (volume.IsGreater(0)) {
					retVal.Add((fc / fuel.FuelDensity / distance / volume).ConvertToLiterPerCubicMeterKiloMeter());
				}
            }

            return retVal;
        }

		public override IList<ConvertedSI> GetFuelConsumptionEntries(KilogramPerMeter fcPerMeter, IFuelProperties fuel, Kilogram payload, CubicMeter volume, double? passenger)
		{
			if (fcPerMeter == null)
			{
				return new List<ConvertedSI>();
			}

			var retVal = new List<ConvertedSI> {
				(fcPerMeter).ConvertToGrammPerKiloMeter(),
				(fcPerMeter /payload).ConvertToGrammPerTonKilometer()};
			if (volume.IsGreater(0))
			{
				retVal.Add((fcPerMeter / volume).ConvertToGrammPerCubicMeterKiloMeter());
			}

			JoulePerMeter fcPerMeterlowerHeatingValue = (fcPerMeter.Value() * fuel.LowerHeatingValueVecto.Value()).SI<JoulePerMeter>();
			retVal.AddRange(new[] {
				(fcPerMeterlowerHeatingValue).ConvertToMegaJoulePerKilometer(),
				(fcPerMeterlowerHeatingValue / payload).ConvertToMegaJoulePerTonKiloMeter(),
			});

			if (volume.IsGreater(0))
			{
				retVal.Add((fcPerMeterlowerHeatingValue / volume).ConvertToMegaJoulePerCubicMeterKiloMeter());
			}

			if (fuel.FuelDensity != null)
			{
				CubicMeterPerMeter fcPerMeterDensity = (fcPerMeter.Value() / fuel.FuelDensity.Value()).SI<CubicMeterPerMeter>();
				retVal.AddRange(new[] {
					(fcPerMeterDensity).ConvertToLiterPer100KiloMeter(),
					(fcPerMeterDensity / payload).ConvertToLiterPerTonKiloMeter(),
				});

				if (volume.IsGreater(0))
				{
					retVal.Add((fcPerMeterDensity / volume).ConvertToLiterPerCubicMeterKiloMeter());
				}
			}

			return retVal;
		}

		#endregion
	}

    public class BusFuelConsumptionWriter : FuelConsumptionWriterBase
    {
        public BusFuelConsumptionWriter(ICommonResultsWriterFactory factory, XNamespace ns) : base(factory, ns) { }


        #region Overrides of FuelConsumptionWriterBase

        public override IList<ConvertedSI> GetFuelConsumptionEntries(Kilogram fc, IFuelProperties fuel,
            Meter distance, Kilogram payload, CubicMeter volume, double? passenger)
        {
            var retVal = new List<ConvertedSI> {
                (fc / distance).ConvertToGrammPerKiloMeter(),
                (fc / distance / passenger.Value).ConvertToGrammPerPassengerKilometer(),

                (fc * fuel.LowerHeatingValueVecto / distance).ConvertToMegaJoulePerKilometer(),
                (fc * fuel.LowerHeatingValueVecto / distance / passenger.Value)
                .ConvertToMegaJoulePerPassengerKilometer(),
            };

            if (fuel.FuelDensity != null) {
                retVal.AddRange(new[] {
                    (fc / fuel.FuelDensity / distance).ConvertToLiterPer100KiloMeter(),
                    (fc / fuel.FuelDensity / distance / passenger.Value).ConvertToLiterPerPassengerKiloMeter(),
                });
            }

            return retVal;
        }

		public override IList<ConvertedSI> GetFuelConsumptionEntries(KilogramPerMeter fcPerMeter, IFuelProperties fuel, Kilogram payload, CubicMeter volume, double? passenger)
		{
			JoulePerMeter fcPerMeterLowerHeatingValue = (fcPerMeter.Value() * fuel.LowerHeatingValueVecto.Value()).SI<JoulePerMeter>();

			var retVal = new List<ConvertedSI> {
				(fcPerMeter).ConvertToGrammPerKiloMeter(),
				(fcPerMeter / passenger.Value).ConvertToGrammPerPassengerKilometer(),
				(fcPerMeterLowerHeatingValue).ConvertToMegaJoulePerKilometer(),
				(fcPerMeterLowerHeatingValue / passenger.Value)
				.ConvertToMegaJoulePerPassengerKilometer(),
			};

			if (fuel.FuelDensity != null)
			{
				CubicMeterPerMeter fcPerMeterDensity = (fcPerMeter.Value() / fuel.FuelDensity.Value()).SI<CubicMeterPerMeter>();
				retVal.AddRange(new[] {
					(fcPerMeterDensity).ConvertToLiterPer100KiloMeter(),
					(fcPerMeterDensity / passenger.Value).ConvertToLiterPerPassengerKiloMeter(),
				});
			}

			return retVal;
		}

		#endregion

	}
}