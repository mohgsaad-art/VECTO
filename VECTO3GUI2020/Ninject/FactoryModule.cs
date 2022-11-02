using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ninject.Extensions.Factory;
using Ninject.Modules;
using VECTO3GUI2020.Ninject.Util;
using VECTO3GUI2020.Util.XML.Interfaces;
using VECTO3GUI2020.ViewModel.Interfaces.Document;
using VECTO3GUI2020.ViewModel.Interfaces.JobEdit;
using VECTO3GUI2020.ViewModel.Interfaces.JobEdit.Vehicle.Components;
using VECTO3GUI2020.ViewModel.MultiStage.Interfaces;

namespace VECTO3GUI2020.Ninject
{
    public class FactoryModule : NinjectModule
    {
		public override void Load()
		{
			Bind<IXMLWriterFactory>().ToFactory(() => new UseFirstArgumentTypeAsNameInstanceProvider(fallback:true));
			//Bind<IMultiStageViewModelFactory>().ToFactory(() => new UseFirstArgumentAsNameInstanceProvider(skipFirstArgument: true, fallback: true));
			
			Bind<IJobEditViewModelFactory>().ToFactory(() => new UseFirstArgumentTypeAsNameInstanceProvider());
			Bind<IDocumentViewModelFactory>().ToFactory(() => new UseFirstArgumentAsNameInstanceProvider(false));
			Bind<IComponentViewModelFactory>().ToFactory(
				() => new UseFirstArgumentTypeAsNameInstanceProvider(true));


			Bind<IMultiStageViewModelFactory>().To<MultiStageViewModelFactory>().
				InSingletonScope();
			Bind<IMultiStageViewModelFactoryDefaultInstanceProvider>().
				ToFactory();
			Bind<IMultiStageViewModelFactoryTypeAsNameInstanceProvider>().
				ToFactory(() => new UseFirstArgumentTypeAsNameInstanceProvider());
			Bind<IMultistageViewModelFactoryFirstParameterAsNameInstanceProvider>().ToFactory(() =>
				new UseFirstArgumentAsNameInstanceProvider(skipFirstArgument: false));

		}
	}
}
