using System;
using System.Xml.Linq;
using Ninject.Extensions.Factory;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCore;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.DataProvider;
using TUGraz.VectoCore.Ninject;
using TUGraz.VectoCore.Utils;
using TUGraz.VectoCore.Utils.Ninject;
using VECTO3GUI2020.ViewModel.Implementation.JobEdit.Vehicle.Components;
using VECTO3GUI2020.ViewModel.Interfaces.JobEdit.Vehicle.Components;

namespace VECTO3GUI2020.Ninject.Factories
{
	
	public interface IComponentViewModelFactoryInternal
	{
		IComponentViewModel CreateComponentViewModel(DataSource source, object inputData);
		ICommonComponentViewModel CreateCommonComponentViewModel(object inputData);


	}


    public class ComponentViewModelFactoryModule : AbstractNinjectModule
	{
		private const string scopeName = nameof(IComponentViewModelFactory);
		private const string common = "common";

		#region Overrides of NinjectModule

		public override void Load()
		{
			Bind<IComponentViewModelFactory>().To<ComponentViewModelFactory>();
            Bind<IComponentViewModelFactoryInternal>().ToFactory(() => new CombineArgumentsToNameInstanceProvider(new[] {
				new CombineArgumentsToNameInstanceProvider.MethodSettings() {
					methods = new []{typeof(IComponentViewModelFactoryInternal).GetMethod(nameof(IComponentViewModelFactoryInternal.CreateComponentViewModel))},
					combineToNameDelegate = CombineToNameDelegate,
					skipArguments = 1,
					takeArguments = 1,
				},
				new CombineArgumentsToNameInstanceProvider.MethodSettings() {
					methods = new []{typeof(IComponentViewModelFactoryInternal).GetMethod(nameof(IComponentViewModelFactoryInternal.CreateCommonComponentViewModel))},
					combineToNameDelegate = arguments => common,
					skipArguments = 0,
					takeArguments = 0,
				}
			})).Named(scopeName);

			Bind<ICommonComponentViewModel>().To<CommonComponentViewModel>().WhenAnyAncestorNamed(scopeName).Named(common);
			AddInternalComponentBinding<AirDragViewModel_v1_0>(XMLDeclarationAirdragDataProviderV10.NAMESPACE_URI, XMLDeclarationAirdragDataProviderV10.XSD_TYPE);
			AddInternalComponentBinding<AirDragViewModel_v2_0>(XMLDeclarationAirdragDataProviderV20.NAMESPACE_URI, XMLDeclarationAirdragDataProviderV20.XSD_TYPE);
			AddInternalComponentBinding<AirDragViewModel_v2_4>(XMLDeclarationAirdragDataProviderV24.NAMESPACE_URI, XMLDeclarationAirdragDataProviderV24.XSD_TYPE);
            AddInternalComponentBinding<AirDragViewModel_v2_7>(XMLDeclarationAirdragDataProviderV27.NAMESPACE_URI, XMLDeclarationAirdragDataProviderV27.XSD_TYPE);
        }

		private void AddInternalComponentBinding<TConcrete>(XNamespace ns, string type) where TConcrete : IComponentViewModel
		{
			Bind<IComponentViewModel>().To<TConcrete>().WhenAnyAncestorNamed(scopeName).Named(CombineToName(ns, type));
		}

		private string CombineToNameDelegate(object[] arguments)
		{
			if (arguments.Length >= 1 && arguments[0] is DataSource dataSource) {
				XNamespace ns = dataSource.TypeVersion;
				var type = dataSource.Type;
				return CombineToName(ns, xsdType: type);
			}

			throw new ArgumentException($"First argument must be of type {typeof(DataSource)}");

		}

		private string CombineToName(XNamespace ns, string xsdType)
		{
			return XMLHelper.CombineNamespace(ns, xsdType);
		}
		#endregion
	}
}
