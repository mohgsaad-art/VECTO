using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Xml.Linq;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.CustomerInformationFile.CustomerInformationFile_0_9.ResultWriter;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.OutputData.XML.DeclarationReports.Common
{
    public abstract class NonOVCTotalWriterBase : AbstractResultGroupWriter
    {
        protected NonOVCTotalWriterBase(ICommonResultsWriterFactory factory, XNamespace ns) : base(factory, ns) { }

        #region Overrides of AbstractResultWriter

        public override XElement GetElement(IResultEntry entry)
		{
			var fcWriter = FuelConsumptionWriter;
			return new XElement(TNS + XMLNames.Report_ResultEntry_Total,
				VehiclePerformanceWriter.GetElement(entry),
				fcWriter != null
					? entry.FuelData.Select(f =>
						fcWriter.GetElement(entry, entry.FuelConsumptionFinal(f.FuelType)))
					: null,
				ElectricEnergyConsumptionWriter?.GetElement(entry),
				CO2Writer?.GetElements(entry),
				ElectricRangeWriter?.GetElements(entry),
				HydrogenRangeWriter?.GetElements(entry)
			);
		}

        #endregion

        protected abstract IResultGroupWriter VehiclePerformanceWriter { get; }

        protected abstract IFuelConsumptionWriter FuelConsumptionWriter { get; }

        protected abstract IElectricEnergyConsumptionWriter ElectricEnergyConsumptionWriter { get; }

        protected abstract ICO2Writer CO2Writer { get; }

        protected abstract IElectricRangeWriter ElectricRangeWriter { get; }

        protected virtual IHydrogenRangeWriter HydrogenRangeWriter=> _factory.GetHydrogenRangeWriter(_factory, TNS);
		
    }

    public class LorryConvTotalWriter : NonOVCTotalWriterBase
    {
        public LorryConvTotalWriter(ICommonResultsWriterFactory factory, XNamespace ns) : base(factory, ns) { }


        #region Overrides of NonOVCTotalWriterBase

        protected override IResultGroupWriter VehiclePerformanceWriter => _factory.GetVehiclePerformanceLorry(_factory, TNS);

        protected override IFuelConsumptionWriter FuelConsumptionWriter => _factory.GetFuelConsumptionLorry(_factory, TNS);

        protected override IElectricEnergyConsumptionWriter ElectricEnergyConsumptionWriter => null;

        protected override ICO2Writer CO2Writer => _factory.GetCO2ResultLorry(_factory, TNS);

        protected override IElectricRangeWriter ElectricRangeWriter => null;

		#endregion
	}

    public class LorryHEVNonOVCTotalWriter : NonOVCTotalWriterBase
    {
        public LorryHEVNonOVCTotalWriter(ICommonResultsWriterFactory factory, XNamespace ns) : base(factory, ns) { }


        #region Overrides of NonOVCTotalWriterBase

        protected override IResultGroupWriter VehiclePerformanceWriter => _factory.GetVehiclePerformanceLorry(_factory, TNS);

        protected override IFuelConsumptionWriter FuelConsumptionWriter => _factory.GetFuelConsumptionLorry(_factory, TNS);

        protected override IElectricEnergyConsumptionWriter ElectricEnergyConsumptionWriter => null;

        protected override ICO2Writer CO2Writer => _factory.GetCO2ResultLorry(_factory, TNS);

        protected override IElectricRangeWriter ElectricRangeWriter => null;

		#endregion
    }

    public class LorryPEVTotalWriter : NonOVCTotalWriterBase
    {
        public LorryPEVTotalWriter(ICommonResultsWriterFactory factory, XNamespace ns) : base(factory, ns) { }


        #region Overrides of NonOVCTotalWriterBase

        protected override IResultGroupWriter VehiclePerformanceWriter => _factory.GetVehiclePerformancePEVLorry(_factory, TNS);

		protected override IFuelConsumptionWriter FuelConsumptionWriter => null;
	

        protected override IElectricEnergyConsumptionWriter ElectricEnergyConsumptionWriter => _factory.GetElectricEnergyConsumptionLorry(_factory, TNS);

        protected override ICO2Writer CO2Writer => null;

        protected override IElectricRangeWriter ElectricRangeWriter => _factory.GetElectricRangeWriter(_factory, TNS);

        #endregion
    }

    public abstract class OVCTotalWriterBase : AbstractResultGroupWriter
    {
        protected OVCTotalWriterBase(ICommonResultsWriterFactory factory, XNamespace ns) : base(factory, ns) { }

        #region Overrides of AbstractResultWriter

        [ExcludeFromCodeCoverage()] // OVC writing class is never used for writing non-OVC
        public override XElement GetElement(IResultEntry entry)
        {
            throw new NotImplementedException();
        }

        public override XElement GetElement(IOVCResultEntry entry)
        {
            var total = entry.Weighted;
            return new XElement(TNS + XMLNames.Report_ResultEntry_Total,
                _factory.GetVehiclePerformanceBus(_factory, TNS).GetElement(entry),
                GetFuelConsumption(entry),
                GetElectricConsumption(entry),
                GetCO2(entry),
                _factory.GetElectricRangeWriter(_factory, TNS).GetElements(total),
				HydrogenRangeWriter?.GetElements(total),
                new XElement(TNS + "UtilityFactor", entry.Weighted.Status == VectoRun.Status.PrimaryBusSimulationIgnore ? double.NaN.ToString() : total.UtilityFactor.ToXMLFormat(3))
            );
        }

        protected abstract XElement[] GetFuelConsumption(IOVCResultEntry entry);

        #endregion

        protected abstract XElement GetElectricConsumption(IOVCResultEntry entry);

        protected abstract XElement[] GetCO2(IOVCResultEntry entry);

		protected virtual IHydrogenRangeWriter HydrogenRangeWriter => _factory.GetHydrogenRangeWriter(_factory, TNS);

    }

    public class LorryHEVOVCTotalWriter : OVCTotalWriterBase
    {
        public LorryHEVOVCTotalWriter(ICommonResultsWriterFactory factory, XNamespace ns) : base(factory, ns) { }

        #region Overrides of OVCSummaryWriterBase

        protected override XElement[] GetFuelConsumption(IOVCResultEntry entry)
        {
            return _factory.GetFuelConsumptionLorry(_factory, TNS).GetElements(entry.Weighted);
        }

        protected override XElement GetElectricConsumption(IOVCResultEntry entry)
        {
            return _factory.GetElectricEnergyConsumptionLorry(_factory, TNS).GetElement(entry.Weighted);
        }

        protected override XElement[] GetCO2(IOVCResultEntry entry)
        {
            return _factory.GetCO2ResultLorry(_factory, TNS).GetElements(entry.Weighted);
        }

        #endregion
    }


	public class NoTotalWriter : AbstractResultGroupWriter
    {
		public NoTotalWriter(ICommonResultsWriterFactory factory, XNamespace ns) : base(factory, ns) { }

		public override XElement GetElement(IResultEntry entry)
		{
			return null;
		}

	}

    // ---- bus

    public class BusConvTotalWriter : NonOVCTotalWriterBase
    {
        public BusConvTotalWriter(ICommonResultsWriterFactory factory, XNamespace ns) : base(factory, ns) { }

        #region Overrides of NonOVCTotalWriterBase

        protected override IResultGroupWriter VehiclePerformanceWriter => _factory.GetVehiclePerformanceBus(_factory, TNS);

        protected override IFuelConsumptionWriter FuelConsumptionWriter => _factory.GetFuelConsumptionBus(_factory, TNS);
        protected override IElectricEnergyConsumptionWriter ElectricEnergyConsumptionWriter => null;
        protected override ICO2Writer CO2Writer => _factory.GetCO2ResultBus(_factory, TNS);
        protected override IElectricRangeWriter ElectricRangeWriter => null;
		
        #endregion
    }
    
    public class BusHEVNonOVCTotalWriter : NonOVCTotalWriterBase
    {
        public BusHEVNonOVCTotalWriter(ICommonResultsWriterFactory factory, XNamespace ns) : base(factory, ns) { }

        #region Overrides of NonOVCTotalWriterBase

        protected override IResultGroupWriter VehiclePerformanceWriter => _factory.GetVehiclePerformanceBus(_factory, TNS);
        protected override IFuelConsumptionWriter FuelConsumptionWriter => _factory.GetFuelConsumptionBus(_factory, TNS);
        protected override IElectricEnergyConsumptionWriter ElectricEnergyConsumptionWriter => null;
        protected override ICO2Writer CO2Writer => _factory.GetCO2ResultBus(_factory, TNS);
        protected override IElectricRangeWriter ElectricRangeWriter => null;
		
        #endregion
    }

    public class BusPEVTotalWriter : NonOVCTotalWriterBase
    {
        public BusPEVTotalWriter(ICommonResultsWriterFactory factory, XNamespace ns) : base(factory, ns) { }

        #region Overrides of NonOVCTotalWriterBase

        protected override IResultGroupWriter VehiclePerformanceWriter => _factory.GetVehiclePerformancePEVBus(_factory, TNS);
		//protected override IFuelConsumptionWriter FuelConsumptionWriter => _factory.GetZEVFuelConsumptionBus(_factory, TNS);
		protected override IFuelConsumptionWriter FuelConsumptionWriter => null;
        protected override IElectricEnergyConsumptionWriter ElectricEnergyConsumptionWriter => _factory.GetElectricEnergyConsumptionBus(_factory, TNS);
        protected override ICO2Writer CO2Writer => _factory.GetCO2ResultPEVBus(_factory, TNS);
        protected override IElectricRangeWriter ElectricRangeWriter => _factory.GetElectricRangeWriter(_factory, TNS);
		
        #endregion
    }

    public class BusOVCTotalWriter : OVCTotalWriterBase
    {
        public BusOVCTotalWriter(ICommonResultsWriterFactory factory, XNamespace ns) : base(factory, ns) { }

        #region Overrides of OVCSummaryWriterBase

        protected override XElement[] GetFuelConsumption(IOVCResultEntry entry)
        {
            return _factory.GetFuelConsumptionBus(_factory, TNS).GetElements(entry.Weighted);
		}

        protected override XElement GetElectricConsumption(IOVCResultEntry entry)
        {
            return _factory.GetElectricEnergyConsumptionBus(_factory, TNS).GetElement(entry.Weighted);
        }

        protected override XElement[] GetCO2(IOVCResultEntry entry)
        {
            return _factory.GetCO2ResultBus(_factory, TNS).GetElements(entry.Weighted);
        }

        #endregion
    }
}