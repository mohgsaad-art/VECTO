using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.OutputData.ModDataPostprocessing;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.OutputData.XML.DeclarationReports.Common
{

    public interface ICommonResultsWriterFactory
    {
        IResultGroupWriter GetLorryConvSuccessResultWriter(ICommonResultsWriterFactory factory, XNamespace ns);
        IResultGroupWriter GetLorryHEVNonOVCSuccessResultWriter(ICommonResultsWriterFactory factory, XNamespace ns);
        IResultGroupWriter GetLorryHEVOVCSuccessResultWriter(ICommonResultsWriterFactory factory, XNamespace ns);
		IResultGroupWriter GetLorryFCHVNonOVCSuccessResultWriter(ICommonResultsWriterFactory factory, XNamespace ns);
		IResultGroupWriter GetLorryFCHVOVCSuccessResultWriter(ICommonResultsWriterFactory factory, XNamespace ns);
		IResultGroupWriter GetLorryPEVSuccessResultWriter(ICommonResultsWriterFactory factory, XNamespace ns);
        IResultGroupWriter GetLorryPEVNonOVCSuccessResultWriter(ICommonResultsWriterFactory factory, XNamespace ns);

        IResultGroupWriter GetLorryErrorResultWriter(ICommonResultsWriterFactory factory, XNamespace ns);

        IResultGroupWriter GetBusConvSuccessResultWriter(ICommonResultsWriterFactory factory, XNamespace ns);
        IResultGroupWriter GetBusHEVNonOVCSuccessResultWriter(ICommonResultsWriterFactory factory, XNamespace ns);
        IResultGroupWriter GetBusHEVOVCSuccessResultWriter(ICommonResultsWriterFactory factory, XNamespace ns);
		IResultGroupWriter GetBusFCHVNonOVCSuccessResultWriter(ICommonResultsWriterFactory factory, XNamespace ns);
		IResultGroupWriter GetBusFCHVOVCSuccessResultWriter(ICommonResultsWriterFactory factory, XNamespace ns);
		IResultGroupWriter GetBusPEVSuccessResultWriter(ICommonResultsWriterFactory factory, XNamespace ns);

        IResultGroupWriter GetBusErrorResultWriter(ICommonResultsWriterFactory factory, XNamespace ns);

        IResultSequenceWriter GetSuccessMissionWriter(ICommonResultsWriterFactory factory, XNamespace ns);
		IResultSequenceWriter GetErrorMissionWriter(ICommonResultsWriterFactory factory, XNamespace ns);
        IResultGroupWriter GetLorrySimulationParameterWriter(ICommonResultsWriterFactory factory, XNamespace ns);
		IResultGroupWriter GetErrorSimulationParameterWriter(ICommonResultsWriterFactory factory, XNamespace ns);

        IResultGroupWriter GetLorryConvTotalWriter(ICommonResultsWriterFactory factory, XNamespace ns);
        IResultGroupWriter GetLorryHEVNonOVCTotalWriter(ICommonResultsWriterFactory factory, XNamespace ns);
        IResultGroupWriter GetLorryHEVOVCResultWriterChargeDepleting(ICommonResultsWriterFactory factory, XNamespace ns);
        IResultGroupWriter GetLorryHEVOVCResultWriterChargeSustaining(ICommonResultsWriterFactory factory, XNamespace ns);
        IResultGroupWriter GetLorryHEVOVCTotalWriter(ICommonResultsWriterFactory factory, XNamespace ns);
		IResultGroupWriter GetLorryFCHVNonOVCTotalWriter(ICommonResultsWriterFactory factory, XNamespace ns);
		IResultGroupWriter GetLorryFCHVOVCResultWriterChargeDepleting(ICommonResultsWriterFactory factory, XNamespace ns);
		IResultGroupWriter GetLorryFCHVOVCResultWriterChargeSustaining(ICommonResultsWriterFactory factory, XNamespace ns);
        IResultGroupWriter GetLorryPEVTotalWriter(ICommonResultsWriterFactory factory, XNamespace ns);

		IResultGroupWriter GetVehiclePerformanceLorry(ICommonResultsWriterFactory factory, XNamespace ns);

		IResultGroupWriter GetVehiclePerformancePEVLorry(ICommonResultsWriterFactory factory, XNamespace ns);
		IResultGroupWriter GetVehiclePerformanceFCHVLorry(ICommonResultsWriterFactory factory, XNamespace ns);

        IResultGroupWriter GetBusConvTotalWriter(ICommonResultsWriterFactory factory, XNamespace ns);
        IResultGroupWriter GetBusHEVNonOVCTotalWriter(ICommonResultsWriterFactory factory, XNamespace ns);
        IResultGroupWriter GetBusHEVOVCResultWriterChargeDepleting(ICommonResultsWriterFactory factory, XNamespace ns);
        IResultGroupWriter GetBusHEVOVCResultWriterChargeSustaining(ICommonResultsWriterFactory factory, XNamespace ns);
		IResultGroupWriter GetBusFCHVOVCResultWriterChargeDepleting(ICommonResultsWriterFactory factory, XNamespace ns);
		IResultGroupWriter GetBusFCHVOVCResultWriterChargeSustaining(ICommonResultsWriterFactory factory, XNamespace ns);

        IResultGroupWriter GetBusHEVOVCTotalWriter(ICommonResultsWriterFactory factory, XNamespace ns);
        IResultGroupWriter GetBusPEVTotalWriter(ICommonResultsWriterFactory factory, XNamespace ns);

		IResultGroupWriter GetVehiclePerformanceBus(ICommonResultsWriterFactory factory, XNamespace ns);
		IResultGroupWriter GetVehiclePerformancePEVBus(ICommonResultsWriterFactory factory, XNamespace ns);
		IResultGroupWriter GetVehiclePerformanceFCHVBus(ICommonResultsWriterFactory factory, XNamespace ns);

        IFuelConsumptionWriter GetFuelConsumptionLorry(ICommonResultsWriterFactory factory, XNamespace ns);
        IElectricEnergyConsumptionWriter GetElectricEnergyConsumptionLorry(ICommonResultsWriterFactory factory, XNamespace ns);
        ICO2Writer GetCO2ResultLorry(ICommonResultsWriterFactory factory, XNamespace ns);
		ICO2Writer GetCO2SummaryResultLorry(ICommonResultsWriterFactory factory, XNamespace ns);

        IReportResultsSummaryWriter GetLorryConvSummaryWriter(ICommonResultsWriterFactory factory, XNamespace ns);
        IReportResultsSummaryWriter GetLorryHEVNonOVCSummaryWriter(ICommonResultsWriterFactory factory, XNamespace ns);
        IReportResultsSummaryWriter GetLorryHEVOVCSummaryWriter(ICommonResultsWriterFactory factory, XNamespace ns);
		IReportResultsSummaryWriter GetLorryFCHVNonOVCSummaryWriter(ICommonResultsWriterFactory factory, XNamespace ns);
		IReportResultsSummaryWriter GetLorryFCHVOVCSummaryWriter(ICommonResultsWriterFactory factory, XNamespace ns);
        IReportResultsSummaryWriter GetLorryPEVSummaryWriter(ICommonResultsWriterFactory factory, XNamespace ns);
        IReportResultsSummaryWriter GetLorryPEVNonOVCSummaryWriter(ICommonResultsWriterFactory factory, XNamespace ns);

        IReportResultsSummaryWriter GetBusConvSummaryWriter(ICommonResultsWriterFactory factory, XNamespace ns);
        IReportResultsSummaryWriter GetBusHEVNonOVCSummaryWriter(ICommonResultsWriterFactory factory, XNamespace ns);
        IReportResultsSummaryWriter GetBusHEVOVCSummaryWriter(ICommonResultsWriterFactory factory, XNamespace ns);
		IReportResultsSummaryWriter GetBusFCHVNonOVCSummaryWriter(ICommonResultsWriterFactory factory, XNamespace ns);
		IReportResultsSummaryWriter GetBusFCHVOVCSummaryWriter(ICommonResultsWriterFactory factory, XNamespace ns);
        IReportResultsSummaryWriter GetBusPEVSummaryWriter(ICommonResultsWriterFactory factory, XNamespace ns);

        IResultGroupWriter GetBusSimulationParameterWriter(ICommonResultsWriterFactory factory, XNamespace ns);

        IFuelConsumptionWriter GetFuelConsumptionBus(ICommonResultsWriterFactory factory, XNamespace ns);

		IElectricEnergyConsumptionWriter GetElectricEnergyConsumptionBus(ICommonResultsWriterFactory factory, XNamespace ns);
        ICO2Writer GetCO2ResultBus(ICommonResultsWriterFactory factory, XNamespace ns);
        ICO2Writer GetCO2ResultPEVBus(ICommonResultsWriterFactory factory, XNamespace ns);
		ICO2Writer GetCO2SummaryResultBus(ICommonResultsWriterFactory factory, XNamespace ns);
		ICO2Writer GetCO2SummaryResultPEVBus(ICommonResultsWriterFactory factory, XNamespace ns);



        IElectricRangeWriter GetElectricRangeWriter(ICommonResultsWriterFactory factory, XNamespace ns);

		IHydrogenRangeWriter GetHydrogenRangeWriter(ICommonResultsWriterFactory factory, XNamespace ns);

        IResultSequenceWriter GetErrorDetailsWriter(ICommonResultsWriterFactory factory, XNamespace ns);
	}

	public interface IResultsWriter
	{
		XElement GenerateResults(List<IResultEntry> results);
	}

	public interface IResultGroupWriter
	{
		XElement GetElement(IResultEntry entry);

		XElement GetElement(IOVCResultEntry entry);
	}

	public interface IResultSequenceWriter
	{
		XElement[] GetElement(IResultEntry entry);

		XElement[] GetElement(IOVCResultEntry entry);
	}

	public interface IFuelConsumptionWriter
	{
		XElement GetElement(IResultEntry entry, IFuelConsumptionCorrection fuelConsumptionCorrection);
		
		XElement[] GetElements(IWeightedResult entry);

		IList<ConvertedSI> GetFuelConsumptionEntries(
			Kilogram fc,
			IFuelProperties fuel,
			Meter distance,
			Kilogram payload,
			CubicMeter volume,
			double? passenger);

		IList<ConvertedSI> GetFuelConsumptionEntries(
			KilogramPerMeter fcPerMeter,
			IFuelProperties fuel,
			Kilogram payload,
			CubicMeter volume,
			double? passenger);
	}

	public interface IElectricEnergyConsumptionWriter
	{
		XElement GetElement(IResultEntry entry);

		XElement GetElement(IWeightedResult weighted);
	}

	public interface ICO2Writer
	{
		XElement[] GetElements(IResultEntry entry);

		XElement[] GetElements(IWeightedResult entry);
	}

	public interface IReportResultsSummaryWriter
	{
		XElement[] GetElement(IList<IResultEntry> entries);

		XElement[] GetElement(IList<IOVCResultEntry> entries);
	}

	public interface IElectricRangeWriter
	{
		XElement[] GetElements(IResultEntry weightedResult);

		XElement[] GetElements(IWeightedResult weightedResult);

	}

	public interface IHydrogenRangeWriter
	{
		XElement[] GetElements(IResultEntry weightedResult);

		XElement[] GetElements(IWeightedResult weightedResult);

	}
}