using System;
using Ninject.Extensions.Factory;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCore.OutputData.ModDataPostprocessing;
using TUGraz.VectoCore.OutputData.ModDataPostprocessing.Impl;
using TUGraz.VectoCore.Utils.Ninject;

namespace TUGraz.VectoCore.Ninject
{

	public class PostProcessingNinjectModule : AbstractNinjectModule
	{
		#region Overrides of NinjectModule

		public override void Load()
		{
			var nameHelper = new PostprocessorBindingNameHelper();

			//Bind<IModalDataPostProcessorFactory>().ToFactory(() => new UseFirstArgumentAsInstanceProvider());
			Bind<IModalDataPostProcessorFactory>().ToFactory(() => new CombineArgumentsToNameInstanceProvider(
				new CombineArgumentsToNameInstanceProvider.MethodSettings() {
					combineToNameDelegate = nameHelper.CreatePostprocessorName,
					skipArguments = 2,
					takeArguments = 2,
					methods = new [] {
						typeof(IModalDataPostProcessorFactory).GetMethod(nameof(IModalDataPostProcessorFactory.GetPostProcessor))
					}
				}));

			Bind<IModalDataPostProcessor>().To<BatteryElectricPostprocessingCorrection>()
				.Named(nameHelper.PostprocessorName(VectoSimulationJobType.BatteryElectricVehicle, false));
			Bind<IModalDataPostProcessor>().To<BatteryElectricPostprocessingCorrection>()
				.Named(nameHelper.PostprocessorName(VectoSimulationJobType.IEPC_E, false));
			Bind<IModalDataPostProcessor>().To<BatteryElectricPostprocessingCorrection>()
				.Named(nameHelper.PostprocessorName(VectoSimulationJobType.Multiple_FCHV, false));
            Bind<IModalDataPostProcessor>().To<BatteryElectricPostprocessingCorrection>()
                .Named(nameHelper.PostprocessorName(VectoSimulationJobType.Multiple_PEV, false));
            Bind<IModalDataPostProcessor>().To<BatteryElectricPostprocessingCorrection>()
                .Named(nameHelper.PostprocessorName(VectoSimulationJobType.Multiple_SHEV, false));
            
            Bind<IModalDataPostProcessor>().To<SerialHybridModalDataPostprocessingCorrection>()
				.Named(nameHelper.PostprocessorName(VectoSimulationJobType.SerialHybridVehicle, false));
			Bind<IModalDataPostProcessor>().To<SerialHybridModalDataPostprocessingCorrection>()
				.Named(nameHelper.PostprocessorName(VectoSimulationJobType.IEPC_S, false));

			Bind<IModalDataPostProcessor>().To<FCHVPostProcessingCorrection>()
				.Named(nameHelper.PostprocessorName(VectoSimulationJobType.FCHV, false));
			Bind<IModalDataPostProcessor>().To<FCHVPostProcessingCorrection>()
				.Named(nameHelper.PostprocessorName(VectoSimulationJobType.FCHV_IEPC, false));

			Bind<IModalDataPostProcessor>().To<ParallelHybridModalDataPostprocessingCorrection>()
				.Named(nameHelper.PostprocessorName(VectoSimulationJobType.ParallelHybridVehicle, false));
			Bind<IModalDataPostProcessor>().To<ParallelHybridModalDataPostprocessingCorrection>()
				.Named(nameHelper.PostprocessorName(VectoSimulationJobType.IHPC, false));

			Bind<IModalDataPostProcessor>().To<EngineOnlyPostprocessingCorrection>()
				.Named(nameHelper.PostprocessorName(VectoSimulationJobType.EngineOnlySimulation, false));

			Bind<IModalDataPostProcessor>().To<ConventionalModalDataPostprocessingCorrection>()
				.Named(nameHelper.PostprocessorName(VectoSimulationJobType.ConventionalVehicle, false));


			Bind<IModalDataPostProcessor>().To<BatteryOnlyHybridModalDataPostprocessingCorrection>()
				.Named(nameHelper.PostprocessorName(VectoSimulationJobType.SerialHybridVehicle, true));
			Bind<IModalDataPostProcessor>().To<BatteryOnlyHybridModalDataPostprocessingCorrection>()
				.Named(nameHelper.PostprocessorName(VectoSimulationJobType.IEPC_S, true));
			Bind<IModalDataPostProcessor>().To<BatteryOnlyHybridModalDataPostprocessingCorrection>()
				.Named(nameHelper.PostprocessorName(VectoSimulationJobType.ParallelHybridVehicle, true));
			Bind<IModalDataPostProcessor>().To<BatteryOnlyHybridModalDataPostprocessingCorrection>()
				.Named(nameHelper.PostprocessorName(VectoSimulationJobType.IHPC, true));

            Bind<IModalDataPostProcessor>().To<FCHVPostProcessingCorrection>()
                .Named(nameHelper.PostprocessorName(VectoSimulationJobType.FCHV, true));
            Bind<IModalDataPostProcessor>().To<FCHVPostProcessingCorrection>()
                .Named(nameHelper.PostprocessorName(VectoSimulationJobType.FCHV_IEPC, true));

			Bind<IModalDataPostProcessor>().To<BatteryElectricPostprocessingCorrection>()
				.Named(nameHelper.PostprocessorName(VectoSimulationJobType.Multiple_FCHV, true));
			Bind<IModalDataPostProcessor>().To<BatteryOnlyHybridModalDataPostprocessingCorrection>()
				.Named(nameHelper.PostprocessorName(VectoSimulationJobType.Multiple_SHEV, true));
        }

		public class PostprocessorBindingNameHelper : NinjectBindingNameHelperBase
		{
			public string CreatePostprocessorName(object[] arguments) => CheckArguments<VectoSimulationJobType, bool>(arguments, PostprocessorName);

			public string PostprocessorName(VectoSimulationJobType p1, bool p2)
			{
				return $"{p1.ToString()}{(p2 ? "_BO" : "")}";
			}
        }

		#endregion
    }
}