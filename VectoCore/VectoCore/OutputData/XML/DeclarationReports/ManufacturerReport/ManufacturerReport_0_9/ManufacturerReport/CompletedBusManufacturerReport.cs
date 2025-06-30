using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using TUGraz.VectoCommon.BusAuxiliaries;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.Models.SimulationComponent.Data.ElectricComponents.Battery;
using TUGraz.VectoCore.OutputData.ModDataPostprocessing;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.Common;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.ManufacturerReport.ManufacturerReport_0_9.ManufacturerReportXMLTypeWriter;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.OutputData.XML.DeclarationReports.ManufacturerReport.ManufacturerReport_0_9.ManufacturerReport
{
	internal abstract class CompletedBusManufacturerReportBase : AbstractManufacturerReport, IXMLManufacturerReportCompletedBus
	{
		private bool _allSuccess = true;
		
		public CompletedBusManufacturerReportBase(IManufacturerReportFactory MRFReportFactory, IResultsWriterFactory resultFactory) : base(MRFReportFactory, resultFactory) { }


		public override void Initialize(VectoRunData modelData)
		{
			InitializeVehicleData(modelData.InputData);
			_ovc = modelData.VehicleData.OffVehicleCharging;
			var inputData = modelData.InputData as IMultistepBusInputDataProvider;
			Input = inputData.JobInputData.PrimaryVehicle.Vehicle;
			if (inputData == null) {
				throw new VectoException("CompletedBus ManrufacturersRecordFile requires MultistepBusInputData");
			}
			Results = _resultFactory.GetMRFResultsWriter(modelData.InputData, modelData.VehicleData.VehicleCategory.GetVehicleType(),
				modelData.JobType, modelData.VehicleData.OffVehicleCharging, modelData.Exempted);
			InputDataIntegrity = new XElement(Namespace + XMLNames.Report_InputDataSignature,
				inputData.JobInputData.ConsolidateManufacturingStage.Signature == null
					? XMLHelper.CreateDummySig(_di)
					: inputData.JobInputData.ConsolidateManufacturingStage.Signature.ToXML(_di));
		}

		#region Implementation of IXMLManufacturerReportCompletedBus


		public virtual void WriteResult(IResultEntry genericResult,
			IResultEntry specificResult, IResult primaryResult,Func<IResultEntry, IResultEntry, IResult, IResultEntry> getCompletedResult)
		{
			_allSuccess &= genericResult.Status == VectoRun.Status.Success;
			_allSuccess &= specificResult.Status == VectoRun.Status.Success;

			_results.Add(getCompletedResult(genericResult, specificResult, primaryResult));
			return;


			//var result = new CompletedBusResult {
			//	Status = VectoRun.Status.Success,
			//};
			//result.Status = genericResult.Status != VectoRun.Status.Success ? genericResult.Status : result.Status;
			//result.Status = specificResult.Status != VectoRun.Status.Success ? specificResult.Status : result.Status;
			//result.OVCMode = specificResult.OVCMode;
			//if (genericResult.OVCMode != specificResult.OVCMode) {
			//	throw new VectoException($"generic OVC Mode must be equal to specific OVC Mode! but was gen_ovc = {genericResult.OVCMode} != {specificResult.OVCMode} = spec_ovc");
			//}
			//result.BatteryData = specificResult.BatteryData;
			//result.Mission = genericResult.Mission;
			//result.Distance = genericResult.Distance;
			//result.TotalVehicleMass = specificResult.TotalVehicleMass;
			//result.Payload = specificResult.Payload;
			//result.PassengerCount = specificResult.PassengerCount;
			//result.VehicleClass = primaryResult.VehicleGroup;


			/////Factor for each fuel
			///// Factor for electric Energy Consumption

			////TODO: Add primary bus group to writer 


			////TODO: 
			//var combinedResults = (genericResult, specificResult);
			////var speedFactor = CalculateFactor(combinedResults, r => r.AverageSpeed);
			////result.AverageSpeed = primaryResult.
	
			////Info not available in Primary Results -> no factor method
			//result.AverageSpeed = specificResult.AverageSpeed;
			//result.AverageDrivingSpeed = specificResult.AverageDrivingSpeed;
			//result.MinSpeed = specificResult.MinSpeed;
			//result.MaxSpeed = specificResult.MaxSpeed;



			//result.MaxDeceleration = specificResult.MaxDeceleration;
			//result.MaxAcceleration = specificResult.MaxAcceleration;
			//result.FullLoadPercentage = specificResult.FullLoadPercentage;
			//result.GearshiftCount = specificResult.GearshiftCount;
			//result.EngineSpeedDrivingAvg = specificResult.EngineSpeedDrivingAvg;
			//result.EngineSpeedDrivingMin = specificResult.EngineSpeedDrivingMin;
			//result.EngineSpeedDrivingMax = specificResult.EngineSpeedDrivingMax;
			//result.AverageGearboxEfficiency = specificResult.AverageGearboxEfficiency;
			//result.AverageAxlegearEfficiency = specificResult.AverageAxlegearEfficiency;



			////Fuels
			//result.FuelData = specificResult.FuelData;
			//var co2Sum = 0.SI<Kilogram>();
			
			//foreach (var fuel in genericResult.CorrectedFinalFuelConsumption.Keys) {
				
			//	var fuelFactor = CalculateFactor(combinedResults, r => r.FuelConsumptionFinal(fuel).TotalFuelConsumptionCorrected);
			//	var completedFuelConsumption =
			//		fuelFactor * (primaryResult.EnergyConsumption[fuel] * specificResult.Distance);
   //             var fuelConsumption = new CompletedBusFuelConsumption() {
			//		Fuel = specificResult.FuelData.Single(f => f.FuelType == fuel),
			//		EnergyDemand = completedFuelConsumption,
			//	};
			//	co2Sum += fuelConsumption.TotalFuelConsumptionCorrected * fuelConsumption.Fuel.CO2PerFuelWeight;
			//	result.CorrectedFinalFuelConsumption.Add(fuel, fuelConsumption);
			//}
			//result.CO2Total = co2Sum;


   //         result.ElectricEnergyConsumption = null;
			
			//if (!(primaryResult.ElectricEnergyConsumption?.IsEqual(0) ?? true)){
			//	var electricEnergyFactor = CalculateFactor(combinedResults,
			//		r => r.ElectricEnergyConsumption);
			//	result.ElectricEnergyConsumption =
			//		electricEnergyFactor * primaryResult.ElectricEnergyConsumption * specificResult.Distance;

   //         }



			//if (specificResult.ZEV_FuelConsumption_AuxHtr?.IsGreaterOrEqual(0) ?? false) {
			//	result.ZEV_FuelConsumption_AuxHtr = specificResult.ZEV_FuelConsumption_AuxHtr;
			//	var auxHeaterFuel = specificResult.AuxHeaterFuel;

			//	result.AuxHeaterFuel = auxHeaterFuel;
			//	result.FuelData.Add(auxHeaterFuel);

			//	result.ZEV_CO2 = result.ZEV_FuelConsumption_AuxHtr * auxHeaterFuel.CO2PerFuelWeight;
			//}




			//if (genericResult.VectoRunData.JobType.IsOneOf(VectoSimulationJobType.BatteryElectricVehicle,
			//		VectoSimulationJobType.IEPC_E)) {
			//	var elRanges = DeclarationData.CalculateElectricRangesPEVCompletedBus(batteryData: result.BatteryData,
			//		result.ElectricEnergyConsumption, result.Distance);

			//	result.EquivalentAllElectricRange = elRanges.EquivalentAllElectricRange;
			//	result.ActualChargeDepletingRange = elRanges.ActualChargeDepletingRange;
			//	result.ZeroCO2EmissionsRange = elRanges.ZeroCO2EmissionsRange;
			//}

			
   //         _results.Add(result);
        }
		#endregion
	}

	internal class Conventional_CompletedBusManufacturerReport : CompletedBusManufacturerReportBase
    {
		public Conventional_CompletedBusManufacturerReport(IManufacturerReportFactory MRFReportFactory, IResultsWriterFactory resultFactory) : base(MRFReportFactory, resultFactory) { }

		#region Overrides of AbstractManufacturerReport

		public override string OutputDataType => "ConventionalCompletedBusManufacturerOutputDataType";



		protected override void InitializeVehicleData(IDeclarationInputDataProvider inputData)
		{
			Vehicle = _mRFReportFactory.GetConventional_CompletedBusVehicleType().GetElement(inputData);
			//GenerateReport(OutputDataType);

		}

		#endregion
	}

	internal class HEV_CompletedBusManufacturerReport : CompletedBusManufacturerReportBase
	{
		public HEV_CompletedBusManufacturerReport(IManufacturerReportFactory MRFReportFactory, IResultsWriterFactory resultFactory) : base(MRFReportFactory, resultFactory) { }

		#region Overrides of AbstractManufacturerReport

		public override string OutputDataType => "HEVCompletedBusManufacturerOutputDataType";

		protected override void InitializeVehicleData(IDeclarationInputDataProvider inputData)
		{
			Vehicle = _mRFReportFactory.GetHEV_CompletedBusVehicleType().GetElement(inputData);
			//GenerateReport("HEVCompletedBusManufacturerOutputDataType");
		}

		#endregion
	}

	internal class PEV_CompletedBusManufacturerReport : CompletedBusManufacturerReportBase
	{
		public PEV_CompletedBusManufacturerReport(IManufacturerReportFactory MRFReportFactory, IResultsWriterFactory resultFactory) : base(MRFReportFactory, resultFactory) { }

		#region Overrides of AbstractManufacturerReport

		public override string OutputDataType => "PEVCompletedBusManufacturerOutputDataType";

		protected override void InitializeVehicleData(IDeclarationInputDataProvider inputData)
		{
			Vehicle = _mRFReportFactory.GetPEV_CompletedBusVehicleType().GetElement(inputData);
			//GenerateReport(OutputDataType);
		}

		#endregion
	}

	internal class Exempted_CompletedBusManufacturerReport : CompletedBusManufacturerReportBase
	{
		public Exempted_CompletedBusManufacturerReport(IManufacturerReportFactory MRFReportFactory, IResultsWriterFactory resultFactory) : base(MRFReportFactory, resultFactory) { }

		#region Overrides of AbstractManufacturerReport

		public override string OutputDataType => "ExemptedCompletedBusManufacturerOutputDataType";

		protected override void InitializeVehicleData(IDeclarationInputDataProvider inputData)
		{
			Vehicle = _mRFReportFactory.GetExempted_CompletedBusVehicleType().GetElement(inputData);
			//GenerateReport(OutputDataType);
		}

		#endregion
	}
}
