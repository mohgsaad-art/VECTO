using System;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.Reader.Impl;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.Utils.Ninject;

namespace TUGraz.VectoCore.InputData.Reader
{
	public interface IInternalRunDataFactoryFactory
	{
		IVectoRunDataFactory CreateSingleBusRunDataFactory(VehicleTypeAndArchitectureStringHelperRundata.VehicleClassification vehicleClassification, 
			ISingleBusInputDataProvider dataProvider, 
			IDeclarationReport report);

		IVectoRunDataFactory CreateDeclarationRunDataFactory(VehicleTypeAndArchitectureStringHelperRundata.VehicleClassification vehicleClassification,
			IDeclarationInputDataProvider dataProvider,
			IDeclarationReport report);

		IVectoRunDataFactory CreateDeclarationCompletedBusRunDataFactory(VehicleTypeAndArchitectureStringHelperRundata.VehicleClassification vehicleClassification,
			IMultistageVIFInputData dataProvider,
			IDeclarationReport report);

		IVectoRunDataFactory CreateDeclarationVTPRunDataFactory(VehicleCategory vehicleCategory, IVTPDeclarationInputDataProvider ivtpProvider, IVTPReport report);
		
		IVectoRunDataFactory CreateEngineOnlyRunDataFactory(string name, IEngineeringInputDataProvider dataProvider);

		IVectoRunDataFactory CreateEngineeringRunDataFactory(string name, IEngineeringInputDataProvider dataProvider);

		IVectoRunDataFactory CreateEngineeringVTPRunDataFactory(VehicleCategory vehicleCategory, IVTPEngineeringInputDataProvider ivtpProvider);
	}


	public class VectoRunDataFactoryFactory : IVectoRunDataFactoryFactory
	{
		private readonly IInternalRunDataFactoryFactory _internalFactory;

		public VectoRunDataFactoryFactory(IInternalRunDataFactoryFactory internalFactory)
		{
			_internalFactory = internalFactory;
		}

		/// <summary>
		/// Creates a VectoRunDataFactory based on the type of inputDataProvider
		/// </summary>
		/// <param name="inputDataProvider"></param>
		/// <param name="report"></param>
		/// <param name="vtpReport"></param>
		/// <param name="missionFilter"></param>
		/// <returns></returns>
		public IVectoRunDataFactory CreateDeclarationRunDataFactory(IInputDataProvider inputDataProvider,
			IDeclarationReport report, IVTPReport vtpReport)
		{
			if (inputDataProvider == null)
				throw new ArgumentNullException(nameof(inputDataProvider));

			switch (inputDataProvider)
			{
				case IVTPDeclarationInputDataProvider vtpProvider:
					return CreateRunDataReader(vtpProvider, vtpReport);
				case ISingleBusInputDataProvider singleBusProvider:
					return CreateRunDataReader(singleBusProvider, report);
				case IDeclarationInputDataProvider declDataProvider:
					return CreateRunDataReader(declDataProvider, report);
				case IMultistageVIFInputData multistageVifInputData:
					return CreateRunDataReader(multistageVifInputData, report);
				default:
					break;
			}
			throw new VectoException("Unknown InputData for Declaration Mode!", ErrorCodes.VECTORUNDATAFACTORY_UNKNOWN_INPUT);
		}


		private IVectoRunDataFactory CreateRunDataReader(IMultistageVIFInputData multiStepVifInputData, IDeclarationReport report)
		{
			if (multiStepVifInputData.VehicleInputData == null) {
				return _internalFactory.CreateDeclarationCompletedBusRunDataFactory(
					new VehicleTypeAndArchitectureStringHelperRundata.VehicleClassification(
						multiStepVifInputData), multiStepVifInputData, report);
			}
			else {
				return new DeclarationModeMultistageBusVectoRunDataFactory(multiStepVifInputData, report);
			}
		}

		private IVectoRunDataFactory CreateRunDataReader(IDeclarationInputDataProvider declDataProvider, IDeclarationReport report)
		{
			//TODO: encapsulate arguments into object
			var vehicle = declDataProvider.JobInputData.Vehicle;
			try {
				return _internalFactory.CreateDeclarationRunDataFactory(
					new VehicleTypeAndArchitectureStringHelperRundata.VehicleClassification(vehicle), declDataProvider,
					report);
				//var ihpc = (vehicle.Components?.ElectricMachines?.Entries)?.Count(electric => electric.ElectricMachine.IHPCType != "None")  > 0;
				//var iepc = (vehicle.Components?.IEPC != null);
				//return _internalFactory.CreateDeclarationRunDataFactory(declDataProvider.JobInputData.Vehicle.VehicleCategory, 
				//	declDataProvider.JobInputData.JobType,
				//	declDataProvider.JobInputData.Vehicle.ArchitectureID,
				//	declDataProvider.JobInputData.Vehicle.ExemptedVehicle, iepc, ihpc, declDataProvider, report);
			} catch (Exception ex) {
				throw new Exception(
					$"Could not create RunDataFactory for Vehicle Category {declDataProvider.JobInputData.Vehicle.VehicleCategory} {declDataProvider.JobInputData.Vehicle.ArchitectureID} {declDataProvider.JobInputData.JobType}", ex);
			}
			
		}

		private IVectoRunDataFactory CreateRunDataReader(IVTPDeclarationInputDataProvider vtpProvider, IDeclarationReport report)
		{
			var vtpReport = CastReport<IVTPReport>(report);

			try {
				return _internalFactory.CreateDeclarationVTPRunDataFactory(vtpProvider.JobInputData.Vehicle.VehicleCategory
                    , vtpProvider, vtpReport);
			} catch (Exception ex) {
				throw new Exception(
					$"Could not create RunDataFactory for Vehicle Category {vtpProvider.JobInputData.Vehicle.VehicleCategory}", ex);
			}

		}

		private IVectoRunDataFactory CreateRunDataReader(ISingleBusInputDataProvider singleBusProvider, IDeclarationReport report)
		{

			return _internalFactory.CreateSingleBusRunDataFactory(new VehicleTypeAndArchitectureStringHelperRundata.VehicleClassification(singleBusProvider), singleBusProvider, report);
		}




		public IVectoRunDataFactory CreateEngineeringRunDataFactory(IInputDataProvider inputDataProvider)
		{
			if (inputDataProvider == null)
				throw new ArgumentNullException(nameof(inputDataProvider));

			switch (inputDataProvider) {
				case IVTPEngineeringInputDataProvider vtpProvider:
					return _internalFactory.CreateEngineeringVTPRunDataFactory(
						vtpProvider.JobInputData.Vehicle.VehicleCategory, vtpProvider);
                //case IVTPEngineeringInputDataProvider vtpProvider when vtpProvider.JobInputData.Vehicle.VehicleCategory.IsLorry():
                //	return new EngineeringVTPModeVectoRunDataFactoryLorries(vtpProvider);
                //case IVTPEngineeringInputDataProvider vtpProvider when vtpProvider.JobInputData.Vehicle.VehicleCategory.IsBus():
                //	return new EngineeringVTPModeVectoRunDataFactoryHeavyBusPrimary(vtpProvider);
                case IEngineeringInputDataProvider engDataProvider when engDataProvider.JobInputData.JobType == VectoSimulationJobType.EngineOnlySimulation:
					return _internalFactory.CreateEngineOnlyRunDataFactory(EngineOnlyVectoRunDataFactory.Name, engDataProvider);
				case IEngineeringInputDataProvider engDataProvider:
					return _internalFactory.CreateEngineeringRunDataFactory(EngineeringModeVectoRunDataFactory.Name, engDataProvider);
				default:
					throw new VectoException("Unknown InputData for Engineering Mode!", ErrorCodes.VECTORUNDATAFACTORY_UNKNOWN_INPUT);
			}

        }

		private T CastReport<T>(object report)
		{
			if (report is T castedReport) {
				return castedReport;
			} else {
				throw new VectoException("Error creating VectoRunDataFactory - wrong ReportType", ErrorCodes.VECTORUNDATAFACTORY_BAD_REPORT_TYPE);
			}
		}
	}
}
