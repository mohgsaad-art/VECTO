using Ninject.Extensions.Factory;
using TUGraz.VectoCore.Models.SimulationComponent;
using TUGraz.VectoCore.Models.SimulationComponent.Impl;
using TUGraz.VectoCore.Models.SimulationComponent.Impl.Shiftstrategies;
using TUGraz.VectoCore.Models.SimulationComponent.Impl.Shiftstrategies.ShiftPolygonCalc;
using TUGraz.VectoCore.Utils.Ninject;

namespace TUGraz.VectoCore.InputData.Reader.DataObjectAdapter
{
    public class ShiftStrategyNinjectModule : AbstractNinjectModule
	{
		public override void Load()
		{
			Bind<IShiftStrategyFactory>().To<ShiftStrategyFactory>().InSingletonScope();
			Bind<IInternalShiftStrategyFactory>().ToFactory(() => new UseFirstArgumentAsInstanceProvider()).InSingletonScope();
			
			
			Bind<IShiftStrategy>().To<MTShiftStrategy>().Named(MTShiftStrategy.Name);
			Bind<IShiftPolygonCalculator>().To<AMTShiftStrategyPolygonCalculator>().Named(MTShiftStrategy.Name);
			
			Bind<IShiftStrategy>().To<AMTShiftStrategyOptimized>().Named(AMTShiftStrategyOptimized.Name);
			Bind<IShiftPolygonCalculator>().To<AMTShiftStrategyOptimizedPolygonCalculator>().Named(AMTShiftStrategyOptimized.Name);

			Bind<IShiftStrategy>().To<ATShiftStrategyOptimized>().Named(ATShiftStrategyOptimized.Name);
			Bind<IShiftPolygonCalculator>().To<ATShiftStrategyOptimizedPolygonCalculator>().Named(ATShiftStrategyOptimized.Name);

            Bind<IShiftStrategy>().To<PEVAMTShiftStrategy>().Named(PEVAMTShiftStrategy.Name);
			Bind<IShiftPolygonCalculator>().To<PEVAMTShiftStrategyPolygonCreator>().Named(PEVAMTShiftStrategy.Name);
			
			Bind<IShiftStrategy>().To<APTNShiftStrategy>().Named(APTNShiftStrategy.Name);
			Bind<IShiftPolygonCalculator>().To<PEVAMTShiftStrategyPolygonCreator>().Named(APTNShiftStrategy.Name);
        }
	}
}