using System;
using System.Xml.Linq;
using Ninject.Extensions.Factory;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCore;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.DataProvider.v24;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.DataProvider.v27;
using TUGraz.VectoCore.Models.SimulationComponent;
using TUGraz.VectoCore.Ninject;
using TUGraz.VectoCore.Utils;
using TUGraz.VectoCore.Utils.Ninject;
using VECTO3GUI2020.ViewModel.Interfaces.JobEdit.Vehicle;
using VECTO3GUI2020.ViewModel.MultiStage.Implementation;

namespace VECTO3GUI2020.Ninject.Factories
{

	public interface IVehicleViewModelFactory
	{
		/// <summary>
		///
		/// </summary>
		/// <param name="consolidatedInputData">null if an exisiting step input should be used</param>
		/// <param name="stepInputData">null if a new step inputviewmodel should be created</param>
		/// <param name="exempted"></param>
		/// <returns></returns>
		IVehicleViewModel CreateVehicleViewModel(IVehicleDeclarationInputData consolidatedVehicleData, IVehicleDeclarationInputData vehicleInput);

		IVehicleViewModel CreateNewVehicleViewModel(CompletedBusArchitecture arch);
	}

	public class VehicleViewModelFactoryModule : AbstractNinjectModule
	{
		#region Overrides of NinjectModule

		private const string scopeName = nameof(IVehicleViewModelFactory);
		public override void Load()
		{
			Bind<IVehicleViewModelFactory>().ToFactory(() => new CombineArgumentsToNameInstanceProvider(new[] {
				///From consolidated data
				new CombineArgumentsToNameInstanceProvider.MethodSettings() {
					combineToNameDelegate = (args) => {
						if (args.Length >= 2) {
							//Step inputviewmodel with consolidate vehicle data from previous steps
							if (args[0] is IVehicleDeclarationInputData consolidatedVehicle) {
								return ArchName(consolidatedVehicle.VehicleType, consolidatedVehicle.ExemptedVehicle).ToString();
							}

							//Standalone step input
							if (args[1] is IVehicleDeclarationInputData vehicle) {
								return CombineToName(vehicle.DataSource.TypeVersion, vehicle.DataSource.Type);
							}
						}
						throw new ArgumentException($"arg[0] must be {nameof(IVehicleDeclarationInputData)}");
					},
					methods = new []{typeof(IVehicleViewModelFactory).GetMethod(nameof(IVehicleViewModelFactory.CreateVehicleViewModel))},
					takeArguments = 2,
					skipArguments = 0,
				},
				new CombineArgumentsToNameInstanceProvider.MethodSettings() {
					methods = new[]{typeof(IVehicleViewModelFactory).GetMethod(nameof(IVehicleViewModelFactory.CreateNewVehicleViewModel))},
					combineToNameDelegate = (args) => {
						if (args.Length >= 1 && args[0] is CompletedBusArchitecture arch) {
							return arch.ToString();
						}
						throw new ArgumentException($"arg[0] must be {nameof(CompletedBusArchitecture)}");
                    },
					skipArguments = 1,
					takeArguments = 1
				}
			})).Named(scopeName);

            // Empty vehicle view models
			// Create depending on jobtype
			AddVehicleViewModelBinding<InterimStageConventionalBusVehicleViewModel>(VectoSimulationJobType.ConventionalVehicle);
			AddVehicleViewModelBinding<InterimStageHevBusVehicleViewModel>(VectoSimulationJobType.ParallelHybridVehicle); // One for hev is enough map to same name
			AddVehicleViewModelBinding<InterimStagePevBusVehicleViewModel>(VectoSimulationJobType.BatteryElectricVehicle);
			AddVehicleViewModelBinding<InterimStageFCHVBusVehicleViewModel>(VectoSimulationJobType.FCHV);
			AddVehicleViewModelBinding<InterimStageIEPCBusVehicleViewModel>(VectoSimulationJobType.IEPC_E);
			AddVehicleViewModelBinding<InterimStageExemptedBusVehicleViewModel>(VectoSimulationJobType.EngineOnlySimulation, true);

            ///Vehicle ViewModels for existing files: v2.4
            AddVehicleViewModelBinding<InterimStageConventionalBusVehicleViewModel>(XMLDeclarationConventionalCompletedBusDataProviderV24.NAMESPACE_URI, XMLDeclarationConventionalCompletedBusDataProviderV24.XSD_TYPE);
			AddVehicleViewModelBinding<InterimStageHevBusVehicleViewModel>(XMLDeclarationHevCompletedBusDataProviderV24.NAMESPACE_URI, XMLDeclarationHevCompletedBusDataProviderV24.XSD_TYPE);
			AddVehicleViewModelBinding<InterimStagePevBusVehicleViewModel>(XMLDeclarationPEVCompletedBusDataProviderV24.NAMESPACE_URI, XMLDeclarationPEVCompletedBusDataProviderV24.XSD_TYPE);
			AddVehicleViewModelBinding<InterimStageIEPCBusVehicleViewModel>(XMLDeclarationIepcCompletedBusDataProviderV24.NAMESPACE_URI, XMLDeclarationIepcCompletedBusDataProviderV24.XSD_TYPE);
            AddVehicleViewModelBinding<InterimStageExemptedBusVehicleViewModel>(XMLDeclarationExemptedCompletedBusDataProviderV24.NAMESPACE_URI, XMLDeclarationExemptedCompletedBusDataProviderV24.XSD_TYPE);

			///Vehicle ViewModels for existing files: v2.7
			AddVehicleViewModelBinding<InterimStageConventionalBusVehicleViewModel>(XMLDeclaration_Conventional_CompletedBus_DataProviderV27.NAMESPACE_URI, XMLDeclaration_Conventional_CompletedBus_DataProviderV27.XSD_TYPE);
			AddVehicleViewModelBinding<InterimStageHevBusVehicleViewModel>(XMLDeclaration_HEV_CompletedBus_DataProviderV27.NAMESPACE_URI, XMLDeclaration_HEV_CompletedBus_DataProviderV27.XSD_TYPE);
			AddVehicleViewModelBinding<InterimStagePevBusVehicleViewModel>(XMLDeclaration_PEV_CompletedBus_DataProviderV27.NAMESPACE_URI, XMLDeclaration_PEV_CompletedBus_DataProviderV27.XSD_TYPE);
			AddVehicleViewModelBinding<InterimStageFCHVBusVehicleViewModel>(XMLDeclaration_FCHV_CompletedBus_DataProviderV27.NAMESPACE_URI, XMLDeclaration_FCHV_CompletedBus_DataProviderV27.XSD_TYPE);
			AddVehicleViewModelBinding<InterimStageExemptedBusVehicleViewModel>(XMLDeclaration_Exempted_CompletedBus_DataProviderV27.NAMESPACE_URI, XMLDeclaration_Exempted_CompletedBus_DataProviderV27.XSD_TYPE);

		}

		#endregion
		private void AddVehicleViewModelBinding<TConcrete>(XNamespace ns, string type) where TConcrete : IVehicleViewModel
		{
			Bind<IVehicleViewModel>().To<TConcrete>().WhenAnyAncestorNamed(scopeName).Named(CombineToName(ns, type));
		}

		private void AddVehicleViewModelBinding<TConcrete>(VectoSimulationJobType jobType, bool exempted = false) where TConcrete : IVehicleViewModel
		{
			Bind<IVehicleViewModel>().To<TConcrete>().WhenAnyAncestorNamed(scopeName).Named(ArchName(jobType, exempted).ToString());
		}


		public static CompletedBusArchitecture ArchName(VectoSimulationJobType jobType, bool exempted = false)
		{
			if (exempted) {
				return CompletedBusArchitecture.Exempted;
			}
			switch(jobType){
				case VectoSimulationJobType.ConventionalVehicle:
					return CompletedBusArchitecture.Conventional;
				case VectoSimulationJobType.ParallelHybridVehicle:
				case VectoSimulationJobType.SerialHybridVehicle:
				case VectoSimulationJobType.IHPC:
                    return CompletedBusArchitecture.HEV;
				case VectoSimulationJobType.BatteryElectricVehicle:
					return CompletedBusArchitecture.PEV;
				case VectoSimulationJobType.IEPC_E:
				case VectoSimulationJobType.IEPC_S:
					return CompletedBusArchitecture.IEPC;
				case VectoSimulationJobType.FCHV:
				case VectoSimulationJobType.FCHV_IEPC:
					return CompletedBusArchitecture.FCHV;
				default:
					throw new ArgumentOutOfRangeException(nameof(jobType), jobType, null);
			}
		}
        public static string CombineToName(XNamespace ns, string type)
		{
			return XMLHelper.CombineNamespace(ns, type);
		}
	}





}



