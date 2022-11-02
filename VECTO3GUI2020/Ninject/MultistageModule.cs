using Ninject.Extensions.Factory;
using Ninject.Modules;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration;
using VECTO3GUI2020.Model.Multistage;
using VECTO3GUI2020.ViewModel.Implementation.Common;
using VECTO3GUI2020.ViewModel.Interfaces.Common;
using VECTO3GUI2020.ViewModel.Interfaces.Document;
using VECTO3GUI2020.ViewModel.Interfaces.JobEdit.Vehicle;
using VECTO3GUI2020.ViewModel.MultiStage.Implementation;
using VECTO3GUI2020.ViewModel.MultiStage.Interfaces;

namespace VECTO3GUI2020.Ninject
{
    public class MultistageModule : NinjectModule
    {
		public override void Load()
		{

			Bind<IViewModelBase>().To<NewMultiStageJobViewModel>().
				NamedLikeFactoryMethod((IMultiStageViewModelFactory f) => f.GetNewMultistageJobViewModel());

			Bind<IMultiStageJobViewModel>().To<MultiStageJobViewModel_v0_1>().
				NamedLikeFactoryMethod((IMultiStageViewModelFactory f) => f.GetMultiStageJobViewModel(null));

			Bind<IVehicleViewModel>().To<InterimStageBusVehicleViewModel_v2_8>().
				NamedLikeFactoryMethod((IMultiStageViewModelFactory f)=>f.GetInterimStageVehicleViewModel());

			Bind<IManufacturingStageViewModel>().To<ManufacturingStageViewModel_v0_1>().
				NamedLikeFactoryMethod((IMultiStageViewModelFactory f) => f.GetManufacturingStageViewModel(null, false));

			Bind<IMultistageAirdragViewModel>().To<MultistageAirdragViewModel>().
				NamedLikeFactoryMethod((IMultiStageViewModelFactory f) => f.GetMultistageAirdragViewModel());

			Bind<IMultistageAuxiliariesViewModel>().To<MultistageAuxiliariesViewModel>().
				NamedLikeFactoryMethod((IMultiStageViewModelFactory f) => f.GetAuxiliariesViewModel(null));

			Bind<IMultistageDependencies>().To<MultistageLazyDependencies>();

			Bind<ICreateVifViewModel>().To<CreateVifViewModel>().
				NamedLikeFactoryMethod((IMultiStageViewModelFactory f) => f.GetCreateNewVifViewModel());

			Bind<IDocumentViewModel>().To<StageInputViewModel>()
				.Named(typeof(XMLDeclarationInputDataProviderV20).ToString());

			Bind<IDocumentViewModel>().To<StageInputViewModel>()
				.NamedLikeFactoryMethod((IMultiStageViewModelFactory f) => f.GetStageInputViewModel(default(bool)));

			Bind<IAdditionalJobInfoViewModel>().To<AdditionalJobInfoViewModelMultiStage>()
				.WhenInjectedInto(typeof(IMultiStageJobViewModel));

			Bind<IAdditionalJobInfoViewModel>().To<AdditionalJobInfoViewModelNewVif>()
				.WhenInjectedInto(typeof(ICreateVifViewModel));

			Bind<IAdditionalJobInfoViewModel>().To<AdditionalJobInfoViewModelStageInput>()
				.WhenInjectedInto(typeof(IStageViewModelBase));

			Bind<JSONJob>().ToSelf();
		}
	}
}
