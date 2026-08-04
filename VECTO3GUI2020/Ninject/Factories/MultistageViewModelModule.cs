using System;
using System.Diagnostics;
using Ninject.Extensions.Factory;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCore;
using TUGraz.VectoCore.Ninject;
using TUGraz.VectoCore.Utils.Ninject;
using VECTO3GUI2020.ViewModel.MultiStage.Implementation;

namespace VECTO3GUI2020.Ninject.Factories
{
	public interface IMultistepComponentViewModelFactory
	{
		IMultistageAuxiliariesViewModel CreateMultistepBusAuxViewModel(CompletedBusArchitecture arch, IBusAuxiliariesDeclarationData consolidatedInputData);
		IMultistageAuxiliariesViewModel CreateNewMultistepBusAuxViewModel(CompletedBusArchitecture arch);
	}


	public class MultistepComponentViewModelFactory : AbstractNinjectModule
	{
		private string scopeName = "MultistepComponent";
		#region Overrides of NinjectModule

		public override void Load()
		{
			Bind<IMultistepComponentViewModelFactory>().ToFactory(() => new CombineArgumentsToNameInstanceProvider(new[] {
				new CombineArgumentsToNameInstanceProvider.MethodSettings() {
					methods = new[] {
						typeof(IMultistepComponentViewModelFactory).GetMethod(nameof(IMultistepComponentViewModelFactory.CreateNewMultistepBusAuxViewModel)),
						typeof(IMultistepComponentViewModelFactory).GetMethod(nameof(IMultistepComponentViewModelFactory.CreateMultistepBusAuxViewModel))
					},
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
			AddBinding<IMultistageAuxiliariesViewModel, MultistageAuxiliariesViewModel_Conventional>(CompletedBusArchitecture.Conventional);
			AddBinding<IMultistageAuxiliariesViewModel, MultistageAuxiliariesViewModel_xEV>(CompletedBusArchitecture.HEV);
			AddBinding<IMultistageAuxiliariesViewModel, MultistageAuxiliariesViewModel_xEV>(CompletedBusArchitecture.PEV);
			AddBinding<IMultistageAuxiliariesViewModel, MultistageAuxiliariesViewModel_xEV>(CompletedBusArchitecture.IEPC);
			AddBinding<IMultistageAuxiliariesViewModel, MultistageAuxiliariesViewModel_xEV>(CompletedBusArchitecture.FCHV);
		}

		[DebuggerStepThrough]
		public string GetName(CompletedBusArchitecture arch)
		{
			return arch.ToString();
		}
		#endregion


		public void AddBinding<TInterface, TConcrete>(CompletedBusArchitecture arch)
			where TConcrete : class, TInterface
		{
			Bind<TInterface>().To<TConcrete>().WhenParentNamed(scopeName).Named(GetName(arch));
		}

	}

}
