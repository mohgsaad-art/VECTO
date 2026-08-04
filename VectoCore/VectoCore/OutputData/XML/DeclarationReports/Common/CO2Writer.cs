using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.OutputData.XML.DeclarationReports.Common
{
    public abstract class CO2WriterBase : AbstractResultWriter, ICO2Writer
    {
        protected CO2WriterBase(ICommonResultsWriterFactory factory, XNamespace ns) : base(factory, ns) { }

        public virtual XElement[] GetElements(IResultEntry entry)
		{
			if (entry.Status == VectoRun.Status.PrimaryBusSimulationIgnore) {
				return GetCO2ResultEntries(entry.CO2Total, entry.Distance, entry.Payload, entry.CargoVolume, entry.PassengerCount)
					.Select(x => new XElement(TNS + XMLNames.Report_Results_CO2,
						new ConvertedSI(double.NaN, x.Value.Units).ValueAsUnit()))
					.ToArray();
			}
			return GetCO2ResultEntries(entry.CO2Total, entry.Distance, entry.Payload, entry.CargoVolume, entry.PassengerCount)
				.Select(x => new XElement(TNS + XMLNames.Report_Results_CO2, x.GetElement()))
				.ToArray();
		}

        public virtual XElement[] GetElements(IWeightedResult entry)
		{
			if (entry.Status == VectoRun.Status.PrimaryBusSimulationIgnore) {
				return GetCO2ResultEntries(entry.CO2PerMeter, entry.Payload, entry.CargoVolume, entry.PassengerCount)
					.Select(x => new XElement(TNS + XMLNames.Report_Results_CO2,
						new ConvertedSI(double.NaN, x.Value.Units).ValueAsUnit()))
					.ToArray();
            }

			return GetCO2ResultEntries(entry.CO2PerMeter, entry.Payload, entry.CargoVolume, entry.PassengerCount)
				.Select(x => new XElement(TNS + XMLNames.Report_Results_CO2, x.GetElement()))
				.ToArray();
		}

		protected abstract IList<FormattedReportValue> GetCO2ResultEntries(
			Kilogram co2,
			Meter distance,
			Kilogram payload,
			CubicMeter volume,
			double? passengers);


		protected abstract IList<FormattedReportValue> GetCO2ResultEntries(
			KilogramPerMeter co2PerMeter,
			Kilogram payload,
			CubicMeter volume,
			double? passengers);
	}

	public class LorryCO2Writer : CO2WriterBase
    {
        public LorryCO2Writer(ICommonResultsWriterFactory factory, XNamespace ns) : base(factory, ns) { }

		protected override IList<FormattedReportValue> GetCO2ResultEntries(Kilogram CO2Total, Meter distance, Kilogram payload, CubicMeter volume, double? passengers)
		{
			var retVal = new List<FormattedReportValue>() {
				new FormattedReportValue((CO2Total / distance).ConvertToGrammPerKiloMeter()),
				new FormattedReportValue((CO2Total / distance / payload).ConvertToGrammPerTonKilometer()),
			};

			if (volume.IsGreater(0))
			{
				retVal.Add(new FormattedReportValue((CO2Total / distance / volume).ConvertToGrammPerCubicMeterKiloMeter()));
			}

			return retVal;
		}

		protected override IList<FormattedReportValue> GetCO2ResultEntries(KilogramPerMeter CO2PerMeter, Kilogram payload, CubicMeter volume, double? passengers)
		{
			var retVal = new List<FormattedReportValue>() {
				new FormattedReportValue(CO2PerMeter.ConvertToGrammPerKiloMeter()),
				new FormattedReportValue((CO2PerMeter / payload).ConvertToGrammPerTonKilometer()),
			};

			if (volume.IsGreater(0)) {
				retVal.Add(new FormattedReportValue((CO2PerMeter / volume).ConvertToGrammPerCubicMeterKiloMeter()));
			}

			return retVal;
		}
	}

	public class LorrySummaryCO2Writer : CO2WriterBase
	{
		public LorrySummaryCO2Writer(ICommonResultsWriterFactory factory, XNamespace ns) : base(factory, ns) { }

		protected override IList<FormattedReportValue> GetCO2ResultEntries(Kilogram CO2Total, Meter distance, Kilogram payload, CubicMeter volume, double? passengers)
		{
			if (distance.IsEqual(0))
			{
				// in some testcases only a single cycle is simulated which has a weighting of 0. consider this to generate a valid report
				return new List<FormattedReportValue>() { new FormattedReportValue((CO2Total / 1.SI<Meter>()).ConvertToGrammPerKiloMeter()), };
			}

			var retVal = new List<FormattedReportValue>() {
				new FormattedReportValue((CO2Total / distance).ConvertToGrammPerKiloMeter()),
				new FormattedReportValue((CO2Total / distance / payload).ConvertToGrammPerTonKilometer(), FormattedReportValue.Format2Decimal),
			};

			if (volume.IsGreater(0))
			{
				retVal.Add(new FormattedReportValue((CO2Total / distance / volume).ConvertToGrammPerCubicMeterKiloMeter()));
			}

			return retVal;
		}

		protected override IList<FormattedReportValue> GetCO2ResultEntries(KilogramPerMeter CO2PerMeter, Kilogram payload, CubicMeter volume, double? passengers)
		{
            var retVal = new List<FormattedReportValue>() {
				new FormattedReportValue(CO2PerMeter.ConvertToGrammPerKiloMeter()),
				new FormattedReportValue((CO2PerMeter / payload).ConvertToGrammPerTonKilometer(), FormattedReportValue.Format2Decimal),
			};

			if (volume.IsGreater(0)) {
				retVal.Add(new FormattedReportValue((CO2PerMeter / volume).ConvertToGrammPerCubicMeterKiloMeter()));
			}

			return retVal;
		}
	}

	public abstract class BusCO2WriterBase : CO2WriterBase
	{
		public BusCO2WriterBase(ICommonResultsWriterFactory factory, XNamespace ns) : base(factory, ns) { }

        public override XElement[] GetElements(IResultEntry entry)
		{
			if (entry.Status == VectoRun.Status.PrimaryBusSimulationIgnore) {
				return GetCO2ResultEntries(entry.CO2Total, entry.Distance, entry.Payload, entry.CargoVolume, entry.PassengerCount)
					.Select(x => new XElement(TNS + XMLNames.Report_Results_CO2,
						new ConvertedSI(double.NaN, x.Value.Units).ValueAsUnit()))
					.ToArray();
			}

			var retval = new List<XElement>();
			retval.AddRange(GetCO2ResultEntries(entry.CO2Total, entry.Distance, entry.Payload, entry.CargoVolume, entry.PassengerCount)
				.Select(x => new XElement(TNS + XMLNames.Report_Results_CO2, x.GetElement())));

			if (!entry.OVCMode.IsOneOf(OvcHevMode.ChargeDepleting, OvcHevMode.ChargeSustaining) 
				&& entry.FuelData.All(f => f.FuelType.IsHydrogenFuel())
				&& (entry.VectoRunData.VehicleData.VehicleCategory != VehicleCategory.HeavyBusPrimaryVehicle)
				) 
			{
				var tmp = _factory.GetFuelConsumptionBus(_factory, TNS);
				retval.Add(new XElement(TNS + XMLNames.Report_ResultEntry_FCZEVAuxHeater,
						new XAttribute(XMLNames.Report_Results_Fuel_Type_Attr, entry.AuxHeaterFuel.FuelType.ToXMLFormat()),
						tmp?.GetFuelConsumptionEntries(entry.ZEV_FuelConsumption_AuxHtr, entry.AuxHeaterFuel, entry.Distance,
							entry.Payload, entry.CargoVolume, entry.PassengerCount).Select(x =>
							new XElement(TNS + XMLNames.Report_Results_FuelConsumption, new FormattedReportValue(x).GetElement()))
					));
				retval.Add(new XElement(TNS + XMLNames.Report_ResultEntry_CO2ZEVAuxHeater,
						GetCO2ResultEntries(entry.ZEV_CO2, entry.Distance, entry.Payload, entry.CargoVolume, entry.PassengerCount)
							.Select(x => new XElement(TNS + XMLNames.Report_Results_CO2, x.GetElement()))
					));
			}
			return retval.ToArray();
		}

		public override XElement[] GetElements(IWeightedResult entry)
		{
			if (entry.Status == VectoRun.Status.PrimaryBusSimulationIgnore) {
				return GetCO2ResultEntries(entry.CO2PerMeter, entry.Payload, entry.CargoVolume, entry.PassengerCount)
					.Select(x => new XElement(TNS + XMLNames.Report_Results_CO2,
						new ConvertedSI(double.NaN, x.Value.Units).ValueAsUnit()))
					.ToArray();
			}

			var retVal = new List<XElement>();
			retVal.AddRange(GetCO2ResultEntries(entry.CO2PerMeter, entry.Payload, entry.CargoVolume, entry.PassengerCount)
				.Select(x => new XElement(TNS + XMLNames.Report_Results_CO2, x.GetElement())));
			
			if (entry.FuelConsumption.Keys.All(x => x.FuelType.IsHydrogenFuel()) && (entry.AuxHeaterFuel != null)) {
				var tmp = _factory.GetFuelConsumptionBus(_factory, TNS);
				retVal.Add(new XElement(TNS + XMLNames.Report_ResultEntry_FCZEVAuxHeater,
					new XAttribute(XMLNames.Report_Results_Fuel_Type_Attr, entry.AuxHeaterFuel.FuelType.ToXMLFormat()),
					tmp?.GetFuelConsumptionEntries(entry.ZEV_FuelConsumption_AuxHtr, entry.AuxHeaterFuel,
						entry.Payload, entry.CargoVolume, entry.PassengerCount).Select(x =>
						new XElement(TNS + XMLNames.Report_Results_FuelConsumption, new FormattedReportValue(x).GetElement()))
				));
				retVal.Add(new XElement(TNS + XMLNames.Report_ResultEntry_CO2ZEVAuxHeater,
					GetCO2ResultEntries(entry.ZEV_CO2, entry.Payload, entry.CargoVolume, entry.PassengerCount)
						.Select(x => new XElement(TNS + XMLNames.Report_Results_CO2, x.GetElement()))
				));
            }
			return retVal.ToArray();
		}
    }

	public class BusCO2Writer : BusCO2WriterBase
    {
        public BusCO2Writer(ICommonResultsWriterFactory factory, XNamespace ns) : base(factory, ns) { }

		protected override IList<FormattedReportValue> GetCO2ResultEntries(Kilogram CO2Total, Meter distance, Kilogram payload, CubicMeter volume, double? passengers)
		{
			return new[] {
				new FormattedReportValue((CO2Total / distance).ConvertToGrammPerKiloMeter()),
				new FormattedReportValue((CO2Total / distance / passengers.Value).ConvertToGrammPerPassengerKilometer()),
			};
		}

		protected override IList<FormattedReportValue> GetCO2ResultEntries(KilogramPerMeter CO2PerMeter, Kilogram payload,
			CubicMeter volume, double? passengers)
		{
			return new[] {
				new FormattedReportValue(CO2PerMeter.ConvertToGrammPerKiloMeter()),
				new FormattedReportValue((CO2PerMeter / passengers.Value).ConvertToGrammPerPassengerKilometer()),
			};
		}
	}

	public class BusSummaryCO2Writer : BusCO2WriterBase
    {
		public BusSummaryCO2Writer(ICommonResultsWriterFactory factory, XNamespace ns) : base(factory, ns) { }

		protected override IList<FormattedReportValue> GetCO2ResultEntries(Kilogram CO2Total, Meter distance, Kilogram payload, CubicMeter volume, double? passengers)
		{
			return new[] {
				new FormattedReportValue((CO2Total / distance).ConvertToGrammPerKiloMeter()),
				new FormattedReportValue((CO2Total / distance / passengers.Value).ConvertToGrammPerPassengerKilometer(), FormattedReportValue.Format2Decimal),
			};
		}

		protected override IList<FormattedReportValue> GetCO2ResultEntries(KilogramPerMeter CO2PerMeter, Kilogram payload, CubicMeter volume, double? passengers)
		{
			return new[] {
				new FormattedReportValue(CO2PerMeter.ConvertToGrammPerKiloMeter()),
				new FormattedReportValue((CO2PerMeter / passengers.Value).ConvertToGrammPerPassengerKilometer(), FormattedReportValue.Format2Decimal),
			};
		}
	}

    public class BusPEVCO2Writer : BusCO2Writer
    {
        public BusPEVCO2Writer(ICommonResultsWriterFactory factory, XNamespace ns) : base(factory, ns) { }

        #region Implementation of ICO2Writer

        public override XElement[] GetElements(IResultEntry entry)
        {
            if (entry.AuxHeaterFuel == null) {
                return null;
            }

			if (entry.Status == VectoRun.Status.PrimaryBusSimulationIgnore) {
				GetCO2ResultEntries(entry.CO2Total, entry.Distance, entry.Payload, entry.CargoVolume, entry.PassengerCount)
					.Select(x => new XElement(TNS + XMLNames.Report_Results_CO2,
							   new ConvertedSI(double.NaN, x.Value.Units).ValueAsUnit()))
					.ToArray();
			}

            var tmp = _factory.GetFuelConsumptionBus(_factory, TNS);
			return new[] {
				new XElement(TNS + XMLNames.Report_ResultEntry_FCZEVAuxHeater,
					new XAttribute(XMLNames.Report_Results_Fuel_Type_Attr, entry.AuxHeaterFuel.FuelType.ToXMLFormat()),
					tmp?.GetFuelConsumptionEntries(entry.ZEV_FuelConsumption_AuxHtr, entry.AuxHeaterFuel, entry.Distance,
						entry.Payload, entry.CargoVolume, entry.PassengerCount).Select(x =>
						new XElement(TNS + XMLNames.Report_Results_FuelConsumption, new FormattedReportValue(x).GetElement()))
				),
				new XElement(TNS + XMLNames.Report_ResultEntry_CO2ZEVAuxHeater,
					GetCO2ResultEntries(entry.ZEV_CO2, entry.Distance, entry.Payload, entry.CargoVolume, entry.PassengerCount)
						.Select(x => new XElement(TNS + XMLNames.Report_Results_CO2, x.GetElement()))
				)
			};
		}

        public override XElement[] GetElements(IWeightedResult entry)
        {
            if (entry.AuxHeaterFuel == null) {
                return null;
            }

            var tmp = _factory.GetFuelConsumptionBus(_factory, TNS) as BusFuelConsumptionWriter;
            return new[] {
                new XElement(TNS + XMLNames.Report_ResultEntry_FCZEVAuxHeater,
                    new XAttribute(XMLNames.Report_Results_Fuel_Type_Attr, entry.AuxHeaterFuel.FuelType.ToXMLFormat()),
                    tmp?.GetFuelConsumptionEntries(entry.ZEV_FuelConsumption_AuxHtr, entry.AuxHeaterFuel,
                        entry.Payload, entry.CargoVolume, entry.PassengerCount).Select(x =>
                        new XElement(TNS + XMLNames.Report_Results_FuelConsumption, new FormattedReportValue(x).GetElement()))
                ),
                new XElement(TNS + XMLNames.Report_ResultEntry_CO2ZEVAuxHeater,
                    GetCO2ResultEntries(entry.ZEV_CO2, entry.Payload, entry.CargoVolume, entry.PassengerCount)
						.Select(x => new XElement(TNS + XMLNames.Report_Results_CO2, x.GetElement()))
                )
            };
        }

        #endregion

    }

	public class BusPEVSummaryCO2Writer : BusPEVCO2Writer
	{
		public BusPEVSummaryCO2Writer(ICommonResultsWriterFactory factory, XNamespace ns) : base(factory, ns) { }
	}
}