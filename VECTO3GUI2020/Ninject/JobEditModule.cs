using Ninject.Modules;
using VECTO3GUI2020.ViewModel.Implementation.JobEdit;
using VECTO3GUI2020.ViewModel.Interfaces.JobEdit;

namespace VECTO3GUI2020.Ninject
{
    public class JobEditModule : NinjectModule
    {
        public override void Load()
        {

            Bind<IJobEditViewModel>().To<DeclarationJobEditViewModel_v1_0>().Named(DeclarationJobEditViewModel_v1_0.VERSION);
            Bind<IJobEditViewModel>().To<DeclarationJobEditViewModel_v2_0>().Named(DeclarationJobEditViewModel_v2_0.VERSION);

        }

    }
}
