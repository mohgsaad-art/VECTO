using Ninject.Modules;
using TUGraz.VectoCore.InputData.FileIO.JSON;
using TUGraz.VectoCore.Utils;
using VECTO3GUI2020.ViewModel.Implementation.Document;
using VECTO3GUI2020.ViewModel.Interfaces.Document;
using VECTO3GUI2020.ViewModel.MultiStage.Implementation;

namespace VECTO3GUI2020.Ninject
{
    public class DocumentModule : NinjectModule
    {
        public override void Load()
        {

            Bind<IDocumentViewModel>().To<DeclarationJobViewModel>().Named(XmlDocumentType.DeclarationJobData.ToString());
			Bind<IDocumentViewModel>().To<MultiStageJobViewModel_v0_1>()
				.Named(XmlDocumentType.MultistepOutputData.ToString());

			Bind<IDocumentViewModel>().To<CreateVifViewModel>()
				.Named(typeof(JSONInputDataV10_PrimaryAndStageInputBus).ToString());

			//Bind<IDocumentViewModel>().To<MultistageJobViewModel>().Named(XmlDocumentType.MultistageOutputData.ToString());
			//Bind<IDocumentViewModel>().To<DeclarationTrailerJobDocumentViewModel>().Named(XmlDocumentType.DeclarationTrailerJobData.ToString());

		}
    }
}
