using Ninject.Extensions.Factory;
using TUGraz.VectoCore;
using TUGraz.VectoCore.Ninject;
using VECTO3GUI2020.Ninject.Util;
using VECTO3GUI2020.ViewModel.Interfaces.JobEdit;
using VECTO3GUI2020.ViewModel.MultiStage.Interfaces;

namespace VECTO3GUI2020.Ninject.Factories
{
    public class FactoryModule : AbstractNinjectModule
    {
        public override void Load()
        {
            LoadModule<DocumentViewModelFactoryModule>();
            LoadModule<ComponentViewModelFactoryModule>();
            LoadModule<VehicleViewModelFactoryModule>();
            LoadModule<XMLWriterFactoryModule>();
			LoadModule<MultistepComponentViewModelFactory>();

            //Bind<IXMLWriterFactory>().ToFactory(() => new UseFirstArgumentTypeAsNameInstanceProvider(fallback: true));
            //Bind<IMultiStageViewModelFactory>().ToFactory(() => new UseFirstArgumentAsNameInstanceProvider(skipFirstArgument: true, fallback: true));

            Bind<IJobEditViewModelFactory>().ToFactory(() => new UseFirstArgumentTypeAsNameInstanceProvider());






            Bind<IMultiStageViewModelFactory>().To<MultiStageViewModelFactory>().
                InSingletonScope();
            Bind<IMultiStageViewModelFactoryDefaultInstanceProvider>().
                ToFactory();
            //Bind<IMultiStageViewModelFactoryTypeAsNameInstanceProvider>().
            //	ToFactory(() => new UseFirstArgumentTypeAsNameInstanceProvider());
            Bind<IMultistageViewModelFactoryFirstParameterAsNameInstanceProvider>().ToFactory(() =>
                new UseFirstArgumentAsNameInstanceProvider(skipFirstArgument: false));

        }
    }
}
