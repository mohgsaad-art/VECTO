using Ninject.Extensions.Factory;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCore.InputData;
using TUGraz.VectoCore.InputData.Reader;
using TUGraz.VectoCore.InputData.Reader.Impl;
using TUGraz.VectoCore.InputData.Reader.Impl.DeclarationMode.CompletedBusRunDataFactory;
using TUGraz.VectoCore.InputData.Reader.Impl.DeclarationMode.HeavyLorryRunDataFactory;
using TUGraz.VectoCore.InputData.Reader.Impl.DeclarationMode.PrimaryBusRunDataFactory;
using TUGraz.VectoCore.InputData.Reader.Impl.DeclarationMode.SingleBus;
using TUGraz.VectoCore.Utils.Ninject;

namespace TUGraz.VectoCore.Ninject.PowertrainComponents
{
	public class VectoRunDataFactoryNinjectModule : AbstractNinjectModule
	{

		private VehicleTypeAndArchitectureStringHelperRundata _vehicleStringHelper = new VehicleTypeAndArchitectureStringHelperRundata();

		private UseFirstArgumentStringHelper _useFirstArgHelper = new UseFirstArgumentStringHelper();

		#region Overrides of NinjectModule

		public override void Load()
		{
			Bind<IVectoRunDataFactoryFactory>().To<VectoRunDataFactoryFactory>().InSingletonScope();


			Bind<IInternalRunDataFactoryFactory>().ToFactory(
				() => new CombineArgumentsToNameInstanceProvider(
					new CombineArgumentsToNameInstanceProvider.MethodSettings() {
						combineToNameDelegate = _vehicleStringHelper.CreateName,
						methods = new[] {
							typeof(IInternalRunDataFactoryFactory)
								.GetMethod(nameof(IInternalRunDataFactoryFactory
									.CreateDeclarationRunDataFactory)),
							typeof(IInternalRunDataFactoryFactory)
								.GetMethod(nameof(IInternalRunDataFactoryFactory
									.CreateDeclarationCompletedBusRunDataFactory)),
							typeof(IInternalRunDataFactoryFactory)
								.GetMethod(nameof(IInternalRunDataFactoryFactory
									.CreateSingleBusRunDataFactory)),
						},
						skipArguments = 1,
						takeArguments = 1,
					},
					new CombineArgumentsToNameInstanceProvider.MethodSettings() {
						combineToNameDelegate = _useFirstArgHelper.CreateName,
						methods = new[] {
							typeof(IInternalRunDataFactoryFactory)
								.GetMethod(nameof(IInternalRunDataFactoryFactory.CreateEngineeringRunDataFactory)),
							typeof(IInternalRunDataFactoryFactory).GetMethod(nameof(IInternalRunDataFactoryFactory
								.CreateEngineOnlyRunDataFactory))
						},
						skipArguments = 1,
						takeArguments = 1,
					},
					new CombineArgumentsToNameInstanceProvider.MethodSettings() {
						combineToNameDelegate = CreateVtpRunDataFactoryNameDecl,
						methods = new[] {
							typeof(IInternalRunDataFactoryFactory)
								.GetMethod(nameof(IInternalRunDataFactoryFactory.CreateDeclarationVTPRunDataFactory)),
						},
						skipArguments = 1,
						takeArguments = 1,
					},
					new CombineArgumentsToNameInstanceProvider.MethodSettings() {
						combineToNameDelegate = CreateVtpRunDataFactoryNameEng,
						methods = new[] {
							typeof(IInternalRunDataFactoryFactory)
								.GetMethod(nameof(IInternalRunDataFactoryFactory.CreateEngineeringVTPRunDataFactory)),
						},
						skipArguments = 1,
						takeArguments = 1,
					}
				)).InSingletonScope();

			#region EngineeringMode

			Bind<IVectoRunDataFactory>().To<EngineeringModeVectoRunDataFactory>().Named(EngineeringModeVectoRunDataFactory.Name);
			Bind<IVectoRunDataFactory>().To<EngineOnlyVectoRunDataFactory>().Named(EngineOnlyVectoRunDataFactory.Name);

			// the actual vehicle category is currently not relevant, the only differentiation is lorry vs bus
            Bind<IVectoRunDataFactory>().To<EngineeringVTPModeVectoRunDataFactoryLorries>().Named(VtpRunDataFactoryName(ExecutionMode.Engineering, VehicleCategory.RigidTruck));
			Bind<IVectoRunDataFactory>().To<EngineeringVTPModeVectoRunDataFactoryHeavyBusPrimary>().Named(VtpRunDataFactoryName(ExecutionMode.Engineering, VehicleCategory.HeavyBusPrimaryVehicle));

            #endregion

            #region Lorries


            Bind<IVectoRunDataFactory>().To<DeclarationModeHeavyLorryRunDataFactory.Conventional>().Named(
				_vehicleStringHelper.GetName(VehicleCategoryHelper.Lorry, VectoSimulationJobType.ConventionalVehicle));

			Bind<IVectoRunDataFactory>().To<DeclarationModeHeavyLorryRunDataFactory.HEV_S2>().Named(
				_vehicleStringHelper.GetName(VehicleCategoryHelper.Lorry, VectoSimulationJobType.SerialHybridVehicle,
					ArchitectureID.S2));

			Bind<IVectoRunDataFactory>().To<DeclarationModeHeavyLorryRunDataFactory.HEV_S3>().Named(
				_vehicleStringHelper.GetName(VehicleCategoryHelper.Lorry, VectoSimulationJobType.SerialHybridVehicle,
					ArchitectureID.S3));

			Bind<IVectoRunDataFactory>().To<DeclarationModeHeavyLorryRunDataFactory.HEV_S4>().Named(
				_vehicleStringHelper.GetName(VehicleCategoryHelper.Lorry, VectoSimulationJobType.SerialHybridVehicle,
					ArchitectureID.S4));

			Bind<IVectoRunDataFactory>().To<DeclarationModeHeavyLorryRunDataFactory.HEV_S_IEPC>().Named(
				_vehicleStringHelper.GetName(VehicleCategoryHelper.Lorry, VectoSimulationJobType.IEPC_S,
					ArchitectureID.S_IEPC));

			Bind<IVectoRunDataFactory>().To<DeclarationModeHeavyLorryRunDataFactory.HEV_F2>().Named(
				_vehicleStringHelper.GetName(VehicleCategoryHelper.Lorry, VectoSimulationJobType.FCHV,
					ArchitectureID.F2));

			Bind<IVectoRunDataFactory>().To<DeclarationModeHeavyLorryRunDataFactory.HEV_F3>().Named(
				_vehicleStringHelper.GetName(VehicleCategoryHelper.Lorry, VectoSimulationJobType.FCHV,
					ArchitectureID.F3));

			Bind<IVectoRunDataFactory>().To<DeclarationModeHeavyLorryRunDataFactory.HEV_F4>().Named(
				_vehicleStringHelper.GetName(VehicleCategoryHelper.Lorry, VectoSimulationJobType.FCHV,
					ArchitectureID.F4));

			Bind<IVectoRunDataFactory>().To<DeclarationModeHeavyLorryRunDataFactory.HEV_F_IEPC>().Named(
				_vehicleStringHelper.GetName(VehicleCategoryHelper.Lorry, VectoSimulationJobType.FCHV_IEPC,
				ArchitectureID.F_IEPC));

			Bind<IVectoRunDataFactory>().To<DeclarationModeHeavyLorryRunDataFactory.MultipleFCHV>().Named(
				_vehicleStringHelper.GetName(VehicleCategoryHelper.Lorry, VectoSimulationJobType.Multiple_FCHV, ArchitectureID.F2));

            Bind<IVectoRunDataFactory>().To<DeclarationModeHeavyLorryRunDataFactory.MultipleFCHV>().Named(
                _vehicleStringHelper.GetName(VehicleCategoryHelper.Lorry, VectoSimulationJobType.Multiple_FCHV, ArchitectureID.F3));

            Bind<IVectoRunDataFactory>().To<DeclarationModeHeavyLorryRunDataFactory.MultipleFCHV>().Named(
                _vehicleStringHelper.GetName(VehicleCategoryHelper.Lorry, VectoSimulationJobType.Multiple_FCHV, ArchitectureID.F4));

            Bind<IVectoRunDataFactory>().To<DeclarationModeHeavyLorryRunDataFactory.MultipleFCHV>().Named(
                _vehicleStringHelper.GetName(VehicleCategoryHelper.Lorry, VectoSimulationJobType.Multiple_FCHV, ArchitectureID.F_IEPC));

            Bind<IVectoRunDataFactory>().To<DeclarationModeHeavyLorryRunDataFactory.MultiplePEV>().Named(
                _vehicleStringHelper.GetName(VehicleCategoryHelper.Lorry, VectoSimulationJobType.Multiple_PEV, ArchitectureID.E2));
            
			Bind<IVectoRunDataFactory>().To<DeclarationModeHeavyLorryRunDataFactory.MultiplePEV>().Named(
                _vehicleStringHelper.GetName(VehicleCategoryHelper.Lorry, VectoSimulationJobType.Multiple_PEV, ArchitectureID.E3));

            Bind<IVectoRunDataFactory>().To<DeclarationModeHeavyLorryRunDataFactory.MultiplePEV>().Named(
                _vehicleStringHelper.GetName(VehicleCategoryHelper.Lorry, VectoSimulationJobType.Multiple_PEV, ArchitectureID.E4));

            Bind<IVectoRunDataFactory>().To<DeclarationModeHeavyLorryRunDataFactory.MultiplePEV>().Named(
                _vehicleStringHelper.GetName(VehicleCategoryHelper.Lorry, VectoSimulationJobType.Multiple_PEV, ArchitectureID.E_IEPC));

            Bind<IVectoRunDataFactory>().To<DeclarationModeHeavyLorryRunDataFactory.MultipleSHEV>().Named(
                _vehicleStringHelper.GetName(VehicleCategoryHelper.Lorry, VectoSimulationJobType.Multiple_SHEV, ArchitectureID.S2));

            Bind<IVectoRunDataFactory>().To<DeclarationModeHeavyLorryRunDataFactory.MultipleSHEV>().Named(
                _vehicleStringHelper.GetName(VehicleCategoryHelper.Lorry, VectoSimulationJobType.Multiple_SHEV, ArchitectureID.S3));

            Bind<IVectoRunDataFactory>().To<DeclarationModeHeavyLorryRunDataFactory.MultipleSHEV>().Named(
                _vehicleStringHelper.GetName(VehicleCategoryHelper.Lorry, VectoSimulationJobType.Multiple_SHEV, ArchitectureID.S4));

            Bind<IVectoRunDataFactory>().To<DeclarationModeHeavyLorryRunDataFactory.MultipleSHEV>().Named(
                _vehicleStringHelper.GetName(VehicleCategoryHelper.Lorry, VectoSimulationJobType.Multiple_SHEV, ArchitectureID.S_IEPC));

            Bind<IVectoRunDataFactory>().To<DeclarationModeHeavyLorryRunDataFactory.HEV_P1>().Named(
				_vehicleStringHelper.GetName(VehicleCategoryHelper.Lorry, VectoSimulationJobType.ParallelHybridVehicle,
					ArchitectureID.P1));

			Bind<IVectoRunDataFactory>().To<DeclarationModeHeavyLorryRunDataFactory.HEV_P2>().Named(
				_vehicleStringHelper.GetName(VehicleCategoryHelper.Lorry, VectoSimulationJobType.ParallelHybridVehicle,
					ArchitectureID.P2));

			Bind<IVectoRunDataFactory>().To<DeclarationModeHeavyLorryRunDataFactory.HEV_P2_5>().Named(
				_vehicleStringHelper.GetName(VehicleCategoryHelper.Lorry, VectoSimulationJobType.ParallelHybridVehicle,
					ArchitectureID.P2_5));

			Bind<IVectoRunDataFactory>().To<DeclarationModeHeavyLorryRunDataFactory.HEV_P3>().Named(
				_vehicleStringHelper.GetName(VehicleCategoryHelper.Lorry, VectoSimulationJobType.ParallelHybridVehicle,
					ArchitectureID.P3));

			Bind<IVectoRunDataFactory>().To<DeclarationModeHeavyLorryRunDataFactory.HEV_P4>().Named(
				_vehicleStringHelper.GetName(VehicleCategoryHelper.Lorry, VectoSimulationJobType.ParallelHybridVehicle,
					ArchitectureID.P4));

			Bind<IVectoRunDataFactory>().To<DeclarationModeHeavyLorryRunDataFactory.HEV_P_IHPC>().Named(
				_vehicleStringHelper.GetName(VehicleCategoryHelper.Lorry, VectoSimulationJobType.IHPC,
					ArchitectureID.P2));

			Bind<IVectoRunDataFactory>().To<DeclarationModeHeavyLorryRunDataFactory.PEV_E2>().Named(
				_vehicleStringHelper.GetName(VehicleCategoryHelper.Lorry, VectoSimulationJobType.BatteryElectricVehicle,
					ArchitectureID.E2));

			Bind<IVectoRunDataFactory>().To<DeclarationModeHeavyLorryRunDataFactory.PEV_E3>().Named(
				_vehicleStringHelper.GetName(VehicleCategoryHelper.Lorry, VectoSimulationJobType.BatteryElectricVehicle,
					ArchitectureID.E3));

			Bind<IVectoRunDataFactory>().To<DeclarationModeHeavyLorryRunDataFactory.PEV_E4>().Named(
				_vehicleStringHelper.GetName(VehicleCategoryHelper.Lorry, VectoSimulationJobType.BatteryElectricVehicle,
					ArchitectureID.E4));

			Bind<IVectoRunDataFactory>().To<DeclarationModeHeavyLorryRunDataFactory.PEV_E_IEPC>().Named(
				_vehicleStringHelper.GetName(VehicleCategoryHelper.Lorry, VectoSimulationJobType.IEPC_E,
					ArchitectureID.E_IEPC));

			Bind<IVectoRunDataFactory>().To<DeclarationModeHeavyLorryRunDataFactory.Exempted>().Named(
				_vehicleStringHelper.GetName(VehicleCategoryHelper.Lorry, exempted: true));
			#endregion

			#region PrimaryBus
			Bind<IVectoRunDataFactory>().To<DeclarationModePrimaryBusRunDataFactory.Conventional>().Named(
				_vehicleStringHelper.GetName(VehicleCategoryHelper.PrimaryBus, VectoSimulationJobType.ConventionalVehicle));

			Bind<IVectoRunDataFactory>().To<DeclarationModePrimaryBusRunDataFactory.HEV_S2>().Named(
				_vehicleStringHelper.GetName(VehicleCategoryHelper.PrimaryBus, VectoSimulationJobType.SerialHybridVehicle,
					ArchitectureID.S2));

			Bind<IVectoRunDataFactory>().To<DeclarationModePrimaryBusRunDataFactory.HEV_S3>().Named(
				_vehicleStringHelper.GetName(VehicleCategoryHelper.PrimaryBus, VectoSimulationJobType.SerialHybridVehicle,
					ArchitectureID.S3));

			Bind<IVectoRunDataFactory>().To<DeclarationModePrimaryBusRunDataFactory.HEV_S4>().Named(
				_vehicleStringHelper.GetName(VehicleCategoryHelper.PrimaryBus, VectoSimulationJobType.SerialHybridVehicle,
					ArchitectureID.S4));
			Bind<IVectoRunDataFactory>().To<DeclarationModePrimaryBusRunDataFactory.HEV_S_IEPC>().Named(
				_vehicleStringHelper.GetName(VehicleCategoryHelper.PrimaryBus, VectoSimulationJobType.IEPC_S,
					ArchitectureID.S_IEPC));

			Bind<IVectoRunDataFactory>().To<DeclarationModePrimaryBusRunDataFactory.HEV_F2>().Named(
				_vehicleStringHelper.GetName(VehicleCategoryHelper.PrimaryBus, VectoSimulationJobType.FCHV,
					ArchitectureID.F2));

			Bind<IVectoRunDataFactory>().To<DeclarationModePrimaryBusRunDataFactory.HEV_F3>().Named(
				_vehicleStringHelper.GetName(VehicleCategoryHelper.PrimaryBus, VectoSimulationJobType.FCHV,
					ArchitectureID.F3));

			Bind<IVectoRunDataFactory>().To<DeclarationModePrimaryBusRunDataFactory.HEV_F4>().Named(
				_vehicleStringHelper.GetName(VehicleCategoryHelper.PrimaryBus, VectoSimulationJobType.FCHV,
					ArchitectureID.F4));
			Bind<IVectoRunDataFactory>().To<DeclarationModePrimaryBusRunDataFactory.HEV_F_IEPC>().Named(
				_vehicleStringHelper.GetName(VehicleCategoryHelper.PrimaryBus, VectoSimulationJobType.FCHV_IEPC,
					ArchitectureID.F_IEPC));

			Bind<IVectoRunDataFactory>().To<DeclarationModePrimaryBusRunDataFactory.MultiplePEV>().Named(
                _vehicleStringHelper.GetName(VehicleCategoryHelper.PrimaryBus, VectoSimulationJobType.Multiple_PEV, ArchitectureID.E2));

            Bind<IVectoRunDataFactory>().To<DeclarationModePrimaryBusRunDataFactory.MultiplePEV>().Named(
                _vehicleStringHelper.GetName(VehicleCategoryHelper.PrimaryBus, VectoSimulationJobType.Multiple_PEV, ArchitectureID.E3));

            Bind<IVectoRunDataFactory>().To<DeclarationModePrimaryBusRunDataFactory.MultiplePEV>().Named(
                _vehicleStringHelper.GetName(VehicleCategoryHelper.PrimaryBus, VectoSimulationJobType.Multiple_PEV, ArchitectureID.E4));

            Bind<IVectoRunDataFactory>().To<DeclarationModePrimaryBusRunDataFactory.MultiplePEV>().Named(
                _vehicleStringHelper.GetName(VehicleCategoryHelper.PrimaryBus, VectoSimulationJobType.Multiple_PEV, ArchitectureID.E_IEPC));

            Bind<IVectoRunDataFactory>().To<DeclarationModePrimaryBusRunDataFactory.MultipleFCHV>().Named(
                _vehicleStringHelper.GetName(VehicleCategoryHelper.PrimaryBus, VectoSimulationJobType.Multiple_FCHV, ArchitectureID.F2));

            Bind<IVectoRunDataFactory>().To<DeclarationModePrimaryBusRunDataFactory.MultipleFCHV>().Named(
                _vehicleStringHelper.GetName(VehicleCategoryHelper.PrimaryBus, VectoSimulationJobType.Multiple_FCHV, ArchitectureID.F3));

            Bind<IVectoRunDataFactory>().To<DeclarationModePrimaryBusRunDataFactory.MultipleFCHV>().Named(
                _vehicleStringHelper.GetName(VehicleCategoryHelper.PrimaryBus, VectoSimulationJobType.Multiple_FCHV, ArchitectureID.F4));

            Bind<IVectoRunDataFactory>().To<DeclarationModePrimaryBusRunDataFactory.MultipleFCHV>().Named(
                _vehicleStringHelper.GetName(VehicleCategoryHelper.PrimaryBus, VectoSimulationJobType.Multiple_FCHV, ArchitectureID.F_IEPC));

            Bind<IVectoRunDataFactory>().To<DeclarationModePrimaryBusRunDataFactory.MultipleSHEV>().Named(
                _vehicleStringHelper.GetName(VehicleCategoryHelper.PrimaryBus, VectoSimulationJobType.Multiple_SHEV, ArchitectureID.S2));

            Bind<IVectoRunDataFactory>().To<DeclarationModePrimaryBusRunDataFactory.MultipleSHEV>().Named(
                _vehicleStringHelper.GetName(VehicleCategoryHelper.PrimaryBus, VectoSimulationJobType.Multiple_SHEV, ArchitectureID.S3));

            Bind<IVectoRunDataFactory>().To<DeclarationModePrimaryBusRunDataFactory.MultipleSHEV>().Named(
                _vehicleStringHelper.GetName(VehicleCategoryHelper.PrimaryBus, VectoSimulationJobType.Multiple_SHEV, ArchitectureID.S4));

            Bind<IVectoRunDataFactory>().To<DeclarationModePrimaryBusRunDataFactory.MultipleSHEV>().Named(
                _vehicleStringHelper.GetName(VehicleCategoryHelper.PrimaryBus, VectoSimulationJobType.Multiple_SHEV, ArchitectureID.S_IEPC));

            Bind<IVectoRunDataFactory>().To<DeclarationModePrimaryBusRunDataFactory.HEV_P1>().Named(
				_vehicleStringHelper.GetName(VehicleCategoryHelper.PrimaryBus, VectoSimulationJobType.ParallelHybridVehicle,
					ArchitectureID.P1));

			Bind<IVectoRunDataFactory>().To<DeclarationModePrimaryBusRunDataFactory.HEV_P2>().Named(
				_vehicleStringHelper.GetName(VehicleCategoryHelper.PrimaryBus, VectoSimulationJobType.ParallelHybridVehicle,
					ArchitectureID.P2));

			Bind<IVectoRunDataFactory>().To<DeclarationModePrimaryBusRunDataFactory.HEV_P2_5>().Named(
				_vehicleStringHelper.GetName(VehicleCategoryHelper.PrimaryBus, VectoSimulationJobType.ParallelHybridVehicle,
					ArchitectureID.P2_5));

			Bind<IVectoRunDataFactory>().To<DeclarationModePrimaryBusRunDataFactory.HEV_P3>().Named(
				_vehicleStringHelper.GetName(VehicleCategoryHelper.PrimaryBus, VectoSimulationJobType.ParallelHybridVehicle,
					ArchitectureID.P3));

			Bind<IVectoRunDataFactory>().To<DeclarationModePrimaryBusRunDataFactory.HEV_P4>().Named(
				_vehicleStringHelper.GetName(VehicleCategoryHelper.PrimaryBus, VectoSimulationJobType.ParallelHybridVehicle,
					ArchitectureID.P4));

			Bind<IVectoRunDataFactory>().To<DeclarationModePrimaryBusRunDataFactory.HEV_P_IHPC>().Named(
				_vehicleStringHelper.GetName(VehicleCategoryHelper.PrimaryBus, VectoSimulationJobType.IHPC,
					ArchitectureID.P2));

			Bind<IVectoRunDataFactory>().To<DeclarationModePrimaryBusRunDataFactory.PEV_E2>().Named(
				_vehicleStringHelper.GetName(VehicleCategoryHelper.PrimaryBus, VectoSimulationJobType.BatteryElectricVehicle,
					ArchitectureID.E2));
			Bind<IVectoRunDataFactory>().To<DeclarationModePrimaryBusRunDataFactory.PEV_E3>().Named(
				_vehicleStringHelper.GetName(VehicleCategoryHelper.PrimaryBus, VectoSimulationJobType.BatteryElectricVehicle,
					ArchitectureID.E3));
			Bind<IVectoRunDataFactory>().To<DeclarationModePrimaryBusRunDataFactory.PEV_E4>().Named(
				_vehicleStringHelper.GetName(VehicleCategoryHelper.PrimaryBus, VectoSimulationJobType.BatteryElectricVehicle,
					ArchitectureID.E4));

			Bind<IVectoRunDataFactory>().To<DeclarationModePrimaryBusRunDataFactory.PEV_E_IEPC>().Named(
				_vehicleStringHelper.GetName(VehicleCategoryHelper.PrimaryBus, VectoSimulationJobType.IEPC_E,
					ArchitectureID.E_IEPC));

			Bind<IVectoRunDataFactory>().To<DeclarationModePrimaryBusRunDataFactory.Exempted>().Named(
				_vehicleStringHelper.GetName(VehicleCategoryHelper.PrimaryBus, true));
			#endregion


			#region CompletedBus
			Bind<IVectoRunDataFactory>().To<DeclarationModeCompletedBusRunDataFactory.Conventional>().Named(
				_vehicleStringHelper.GetName(VehicleCategoryHelper.CompletedBus, VectoSimulationJobType.ConventionalVehicle,
					ArchitectureID.UNKNOWN));
			Bind<IVectoRunDataFactory>().To<DeclarationModeCompletedBusRunDataFactory.HEV_S2>().Named(
				_vehicleStringHelper.GetName(VehicleCategoryHelper.CompletedBus, VectoSimulationJobType.SerialHybridVehicle,
					ArchitectureID.S2));
			Bind<IVectoRunDataFactory>().To<DeclarationModeCompletedBusRunDataFactory.HEV_S3>().Named(
				_vehicleStringHelper.GetName(VehicleCategoryHelper.CompletedBus, VectoSimulationJobType.SerialHybridVehicle,
					ArchitectureID.S3));
			Bind<IVectoRunDataFactory>().To<DeclarationModeCompletedBusRunDataFactory.HEV_S4>().Named(
				_vehicleStringHelper.GetName(VehicleCategoryHelper.CompletedBus, VectoSimulationJobType.SerialHybridVehicle,
					ArchitectureID.S4));
			Bind<IVectoRunDataFactory>().To<DeclarationModeCompletedBusRunDataFactory.HEV_S_IEPC>().Named(
				_vehicleStringHelper.GetName(VehicleCategoryHelper.CompletedBus, VectoSimulationJobType.IEPC_S,
					ArchitectureID.S_IEPC));

			Bind<IVectoRunDataFactory>().To<DeclarationModeCompletedBusRunDataFactory.HEV_P1>().Named(
				_vehicleStringHelper.GetName(VehicleCategoryHelper.CompletedBus, VectoSimulationJobType.ParallelHybridVehicle,
					ArchitectureID.P1));
			Bind<IVectoRunDataFactory>().To<DeclarationModeCompletedBusRunDataFactory.HEV_P2>().Named(
				_vehicleStringHelper.GetName(VehicleCategoryHelper.CompletedBus, VectoSimulationJobType.ParallelHybridVehicle,
					ArchitectureID.P2));
			Bind<IVectoRunDataFactory>().To<DeclarationModeCompletedBusRunDataFactory.HEV_P2_5>().Named(
				_vehicleStringHelper.GetName(VehicleCategoryHelper.CompletedBus, VectoSimulationJobType.ParallelHybridVehicle,
					ArchitectureID.P2_5));
			Bind<IVectoRunDataFactory>().To<DeclarationModeCompletedBusRunDataFactory.HEV_P3>().Named(
				_vehicleStringHelper.GetName(VehicleCategoryHelper.CompletedBus, VectoSimulationJobType.ParallelHybridVehicle,
					ArchitectureID.P3));
			Bind<IVectoRunDataFactory>().To<DeclarationModeCompletedBusRunDataFactory.HEV_P4>().Named(
				_vehicleStringHelper.GetName(VehicleCategoryHelper.CompletedBus, VectoSimulationJobType.ParallelHybridVehicle,
					ArchitectureID.P4));

			Bind<IVectoRunDataFactory>().To<DeclarationModeCompletedBusRunDataFactory.HEV_P_IHPC>().Named(
				_vehicleStringHelper.GetName(VehicleCategoryHelper.CompletedBus, VectoSimulationJobType.IHPC,
					ArchitectureID.P2));

            Bind<IVectoRunDataFactory>().To<DeclarationModeCompletedBusRunDataFactory.PEV_E2>().Named(
				_vehicleStringHelper.GetName(VehicleCategoryHelper.CompletedBus, VectoSimulationJobType.BatteryElectricVehicle,
					ArchitectureID.E2));
			Bind<IVectoRunDataFactory>().To<DeclarationModeCompletedBusRunDataFactory.PEV_E3>().Named(
				_vehicleStringHelper.GetName(VehicleCategoryHelper.CompletedBus, VectoSimulationJobType.BatteryElectricVehicle,
					ArchitectureID.E3));
			Bind<IVectoRunDataFactory>().To<DeclarationModeCompletedBusRunDataFactory.PEV_E4>().Named(
				_vehicleStringHelper.GetName(VehicleCategoryHelper.CompletedBus, VectoSimulationJobType.BatteryElectricVehicle,
					ArchitectureID.E4));
			Bind<IVectoRunDataFactory>().To<DeclarationModeCompletedBusRunDataFactory.PEV_E_IEPC>().Named(
				_vehicleStringHelper.GetName(VehicleCategoryHelper.CompletedBus, VectoSimulationJobType.IEPC_E,
					ArchitectureID.E_IEPC));

            Bind<IVectoRunDataFactory>().To<DeclarationModeCompletedBusRunDataFactory.FCHV_F2>().Named(
                _vehicleStringHelper.GetName(VehicleCategoryHelper.CompletedBus, VectoSimulationJobType.FCHV,
                    ArchitectureID.F2));
            Bind<IVectoRunDataFactory>().To<DeclarationModeCompletedBusRunDataFactory.FCHV_F3>().Named(
                _vehicleStringHelper.GetName(VehicleCategoryHelper.CompletedBus, VectoSimulationJobType.FCHV,
                    ArchitectureID.F3));
            Bind<IVectoRunDataFactory>().To<DeclarationModeCompletedBusRunDataFactory.FCHV_F4>().Named(
                _vehicleStringHelper.GetName(VehicleCategoryHelper.CompletedBus, VectoSimulationJobType.FCHV,
                    ArchitectureID.F4));
            Bind<IVectoRunDataFactory>().To<DeclarationModeCompletedBusRunDataFactory.FCHV_IEPC>().Named(
                _vehicleStringHelper.GetName(VehicleCategoryHelper.CompletedBus, VectoSimulationJobType.FCHV_IEPC,
                    ArchitectureID.F_IEPC));

            Bind<IVectoRunDataFactory>().To<DeclarationModeCompletedBusRunDataFactory.MultiplePEV>().Named(
                _vehicleStringHelper.GetName(VehicleCategoryHelper.CompletedBus, VectoSimulationJobType.Multiple_PEV,
                    ArchitectureID.E2));

            Bind<IVectoRunDataFactory>().To<DeclarationModeCompletedBusRunDataFactory.MultiplePEV>().Named(
                _vehicleStringHelper.GetName(VehicleCategoryHelper.CompletedBus, VectoSimulationJobType.Multiple_PEV,
                    ArchitectureID.E3));

            Bind<IVectoRunDataFactory>().To<DeclarationModeCompletedBusRunDataFactory.MultiplePEV>().Named(
                _vehicleStringHelper.GetName(VehicleCategoryHelper.CompletedBus, VectoSimulationJobType.Multiple_PEV,
                    ArchitectureID.E4));

            Bind<IVectoRunDataFactory>().To<DeclarationModeCompletedBusRunDataFactory.MultiplePEV>().Named(
                _vehicleStringHelper.GetName(VehicleCategoryHelper.CompletedBus, VectoSimulationJobType.Multiple_PEV,
                    ArchitectureID.E_IEPC));

            Bind<IVectoRunDataFactory>().To<DeclarationModeCompletedBusRunDataFactory.MultipleFCHV>().Named(
                _vehicleStringHelper.GetName(VehicleCategoryHelper.CompletedBus, VectoSimulationJobType.Multiple_FCHV,
                    ArchitectureID.F2));

            Bind<IVectoRunDataFactory>().To<DeclarationModeCompletedBusRunDataFactory.MultipleFCHV>().Named(
                _vehicleStringHelper.GetName(VehicleCategoryHelper.CompletedBus, VectoSimulationJobType.Multiple_FCHV,
                    ArchitectureID.F3));

            Bind<IVectoRunDataFactory>().To<DeclarationModeCompletedBusRunDataFactory.MultipleFCHV>().Named(
                _vehicleStringHelper.GetName(VehicleCategoryHelper.CompletedBus, VectoSimulationJobType.Multiple_FCHV,
                    ArchitectureID.F4));

            Bind<IVectoRunDataFactory>().To<DeclarationModeCompletedBusRunDataFactory.MultipleFCHV>().Named(
                _vehicleStringHelper.GetName(VehicleCategoryHelper.CompletedBus, VectoSimulationJobType.Multiple_FCHV,
                    ArchitectureID.F_IEPC));

            Bind<IVectoRunDataFactory>().To<DeclarationModeCompletedBusRunDataFactory.MultipleSHEV>().Named(
                _vehicleStringHelper.GetName(VehicleCategoryHelper.CompletedBus, VectoSimulationJobType.Multiple_SHEV,
                    ArchitectureID.S2));

            Bind<IVectoRunDataFactory>().To<DeclarationModeCompletedBusRunDataFactory.MultipleSHEV>().Named(
                _vehicleStringHelper.GetName(VehicleCategoryHelper.CompletedBus, VectoSimulationJobType.Multiple_SHEV,
                    ArchitectureID.S3));

            Bind<IVectoRunDataFactory>().To<DeclarationModeCompletedBusRunDataFactory.MultipleSHEV>().Named(
                _vehicleStringHelper.GetName(VehicleCategoryHelper.CompletedBus, VectoSimulationJobType.Multiple_SHEV,
                    ArchitectureID.S4));

            Bind<IVectoRunDataFactory>().To<DeclarationModeCompletedBusRunDataFactory.MultipleSHEV>().Named(
                _vehicleStringHelper.GetName(VehicleCategoryHelper.CompletedBus, VectoSimulationJobType.Multiple_SHEV,
                    ArchitectureID.S_IEPC));

            Bind<IVectoRunDataFactory>().To<DeclarationModeCompletedBusRunDataFactory.Exempted>().Named(
				_vehicleStringHelper.GetName(VehicleCategoryHelper.CompletedBus, true));
			#endregion

			#region SingleBus
			Bind<IVectoRunDataFactory>().To<DeclarationModeSingleBusRunDataFactory.Conventional>().Named(
				_vehicleStringHelper.GetSingleBusName(VectoSimulationJobType.ConventionalVehicle,
					ArchitectureID.UNKNOWN ));
			Bind<IVectoRunDataFactory>().To<DeclarationModeSingleBusRunDataFactory.HEV_S2>().Named(
				_vehicleStringHelper.GetSingleBusName(VectoSimulationJobType.SerialHybridVehicle,
					ArchitectureID.S2));
			Bind<IVectoRunDataFactory>().To<DeclarationModeSingleBusRunDataFactory.HEV_S3>().Named(
				_vehicleStringHelper.GetSingleBusName(VectoSimulationJobType.SerialHybridVehicle,
					ArchitectureID.S3));
			Bind<IVectoRunDataFactory>().To<DeclarationModeSingleBusRunDataFactory.HEV_S4>().Named(
				_vehicleStringHelper.GetSingleBusName(VectoSimulationJobType.SerialHybridVehicle,
					ArchitectureID.S4));
			Bind<IVectoRunDataFactory>().To<DeclarationModeSingleBusRunDataFactory.HEV_S_IEPC>().Named(
				_vehicleStringHelper.GetSingleBusName(VectoSimulationJobType.IEPC_S,
					ArchitectureID.S_IEPC));
			
			Bind<IVectoRunDataFactory>().To<DeclarationModeSingleBusRunDataFactory.HEV_P1>().Named(
				_vehicleStringHelper.GetSingleBusName(VectoSimulationJobType.ParallelHybridVehicle,
					ArchitectureID.P1));
			Bind<IVectoRunDataFactory>().To<DeclarationModeSingleBusRunDataFactory.HEV_P2>().Named(
				_vehicleStringHelper.GetSingleBusName(VectoSimulationJobType.ParallelHybridVehicle,
					ArchitectureID.P2));
			Bind<IVectoRunDataFactory>().To<DeclarationModeSingleBusRunDataFactory.HEV_P2_5>().Named(
				_vehicleStringHelper.GetSingleBusName(VectoSimulationJobType.ParallelHybridVehicle,
					ArchitectureID.P2_5));
			Bind<IVectoRunDataFactory>().To<DeclarationModeSingleBusRunDataFactory.HEV_P3>().Named(
				_vehicleStringHelper.GetSingleBusName(VectoSimulationJobType.ParallelHybridVehicle,
					ArchitectureID.P3));
			Bind<IVectoRunDataFactory>().To<DeclarationModeSingleBusRunDataFactory.HEV_P4>().Named(
				_vehicleStringHelper.GetSingleBusName(VectoSimulationJobType.ParallelHybridVehicle,
					ArchitectureID.P4));
			Bind<IVectoRunDataFactory>().To<DeclarationModeSingleBusRunDataFactory.HEV_P_IHPC>().Named(
				_vehicleStringHelper.GetSingleBusName(VectoSimulationJobType.IHPC,
					ArchitectureID.P2));

            Bind<IVectoRunDataFactory>().To<DeclarationModeSingleBusRunDataFactory.PEV_E2>().Named(
				_vehicleStringHelper.GetSingleBusName(VectoSimulationJobType.BatteryElectricVehicle,
					ArchitectureID.E2));
			Bind<IVectoRunDataFactory>().To<DeclarationModeSingleBusRunDataFactory.PEV_E3>().Named(
				_vehicleStringHelper.GetSingleBusName(VectoSimulationJobType.BatteryElectricVehicle,
					ArchitectureID.E3));
			Bind<IVectoRunDataFactory>().To<DeclarationModeSingleBusRunDataFactory.PEV_E4>().Named(
				_vehicleStringHelper.GetSingleBusName(VectoSimulationJobType.BatteryElectricVehicle,
					ArchitectureID.E4));
			Bind<IVectoRunDataFactory>().To<DeclarationModeSingleBusRunDataFactory.PEV_E_IEPC>().Named(
				_vehicleStringHelper.GetSingleBusName(VectoSimulationJobType.IEPC_E,
					ArchitectureID.E_IEPC));

            Bind<IVectoRunDataFactory>().To<DeclarationModeSingleBusRunDataFactory.FCHV_F2>().Named(
                _vehicleStringHelper.GetSingleBusName(VectoSimulationJobType.FCHV,
					ArchitectureID.F2));
            Bind<IVectoRunDataFactory>().To<DeclarationModeSingleBusRunDataFactory.FCHV_F3>().Named(
                _vehicleStringHelper.GetSingleBusName(VectoSimulationJobType.FCHV,
                    ArchitectureID.F3));
            Bind<IVectoRunDataFactory>().To<DeclarationModeSingleBusRunDataFactory.FCHV_F4>().Named(
                _vehicleStringHelper.GetSingleBusName(VectoSimulationJobType.FCHV,
                    ArchitectureID.F4));
            Bind<IVectoRunDataFactory>().To<DeclarationModeSingleBusRunDataFactory.FCHV_IEPC>().Named(
                _vehicleStringHelper.GetSingleBusName(VectoSimulationJobType.FCHV_IEPC,
                    ArchitectureID.F_IEPC));

            Bind<IVectoRunDataFactory>().To<DeclarationModeSingleBusRunDataFactory.MultiplePEV>().Named(
                _vehicleStringHelper.GetSingleBusName(VectoSimulationJobType.Multiple_PEV,
                    ArchitectureID.E2));
            Bind<IVectoRunDataFactory>().To<DeclarationModeSingleBusRunDataFactory.MultiplePEV>().Named(
                _vehicleStringHelper.GetSingleBusName(VectoSimulationJobType.Multiple_PEV,
                    ArchitectureID.E3));
            Bind<IVectoRunDataFactory>().To<DeclarationModeSingleBusRunDataFactory.MultiplePEV>().Named(
                _vehicleStringHelper.GetSingleBusName(VectoSimulationJobType.Multiple_PEV,
                    ArchitectureID.E4));
            Bind<IVectoRunDataFactory>().To<DeclarationModeSingleBusRunDataFactory.MultiplePEV>().Named(
                _vehicleStringHelper.GetSingleBusName(VectoSimulationJobType.Multiple_PEV,
                    ArchitectureID.E_IEPC));

            Bind<IVectoRunDataFactory>().To<DeclarationModeSingleBusRunDataFactory.MultipleFCHV>().Named(
                _vehicleStringHelper.GetSingleBusName(VectoSimulationJobType.Multiple_FCHV,
                    ArchitectureID.F2));
            Bind<IVectoRunDataFactory>().To<DeclarationModeSingleBusRunDataFactory.MultipleFCHV>().Named(
                _vehicleStringHelper.GetSingleBusName(VectoSimulationJobType.Multiple_FCHV,
                    ArchitectureID.F3));
            Bind<IVectoRunDataFactory>().To<DeclarationModeSingleBusRunDataFactory.MultipleFCHV>().Named(
                _vehicleStringHelper.GetSingleBusName(VectoSimulationJobType.Multiple_FCHV,
                    ArchitectureID.F4));
            Bind<IVectoRunDataFactory>().To<DeclarationModeSingleBusRunDataFactory.MultipleFCHV>().Named(
                _vehicleStringHelper.GetSingleBusName(VectoSimulationJobType.Multiple_FCHV,
                    ArchitectureID.F_IEPC));

            Bind<IVectoRunDataFactory>().To<DeclarationModeSingleBusRunDataFactory.MultipleSHEV>().Named(
                _vehicleStringHelper.GetSingleBusName(VectoSimulationJobType.Multiple_SHEV,
                    ArchitectureID.S2));
            Bind<IVectoRunDataFactory>().To<DeclarationModeSingleBusRunDataFactory.MultipleSHEV>().Named(
                _vehicleStringHelper.GetSingleBusName(VectoSimulationJobType.Multiple_SHEV,
                    ArchitectureID.S3));
            Bind<IVectoRunDataFactory>().To<DeclarationModeSingleBusRunDataFactory.MultipleSHEV>().Named(
                _vehicleStringHelper.GetSingleBusName(VectoSimulationJobType.Multiple_SHEV,
                    ArchitectureID.S4));
            Bind<IVectoRunDataFactory>().To<DeclarationModeSingleBusRunDataFactory.MultipleSHEV>().Named(
                _vehicleStringHelper.GetSingleBusName(VectoSimulationJobType.Multiple_SHEV,
                    ArchitectureID.S_IEPC));

            Bind<IVectoRunDataFactory>().To<DeclarationModeSingleBusRunDataFactory.Exempted>().Named(
				_vehicleStringHelper.GetSingleBusName(VectoSimulationJobType.ConventionalVehicle,
					ArchitectureID.UNKNOWN, true));

			#endregion

			#region VTP

			// the actual vehicle category is currently not relevant, the only differentiation is lorry vs bus

			Bind<IVectoRunDataFactory>().To<DeclarationVTPModeVectoRunDataFactoryLorries>()
				.Named(VtpRunDataFactoryName(ExecutionMode.Declaration, VehicleCategory.RigidTruck));

			Bind<IVectoRunDataFactory>().To<DeclarationVTPModeVectoRunDataFactoryHeavyBusPrimary>()
				.Named(VtpRunDataFactoryName(ExecutionMode.Declaration, VehicleCategory.HeavyBusPrimaryVehicle));

			#endregion
		}

		private string CreateVtpRunDataFactoryNameDecl(object[] arguments)
		{
			if (arguments.Length == 1 && arguments[0] is VehicleCategory vcat) {
				return VtpRunDataFactoryName(ExecutionMode.Declaration, vcat);
			}

			throw new VectoException("Unknown argument for VTP RunDataFactory Name");
		}

		private string CreateVtpRunDataFactoryNameEng(object[] arguments)
		{
			if (arguments.Length == 1 && arguments[0] is VehicleCategory vcat) {
				return VtpRunDataFactoryName(ExecutionMode.Engineering, vcat);
			}

			throw new VectoException("Unknown argument for VTP RunDataFactory Name");
		}

        private string VtpRunDataFactoryName(ExecutionMode mode, VehicleCategory vcat)
		{
			return $"{mode}_{(vcat.IsLorry() ? "VTPLorry" : "VTPBus")}";
        }

		#endregion



	}

}