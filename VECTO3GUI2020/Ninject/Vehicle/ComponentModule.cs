
using Ninject.Extensions.Factory;
using Ninject.Modules;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCore.Models.Declaration;
using VECTO3GUI2020.Model.Interfaces;
using VECTO3GUI2020.Ninject.Util;
using VECTO3GUI2020.Util;
using VECTO3GUI2020.ViewModel.Implementation.JobEdit.Vehicle;
using VECTO3GUI2020.ViewModel.Implementation.JobEdit.Vehicle.Components;
using VECTO3GUI2020.ViewModel.Interfaces.JobEdit;
using VECTO3GUI2020.ViewModel.Interfaces.JobEdit.Vehicle;
using VECTO3GUI2020.ViewModel.Interfaces.JobEdit.Vehicle.Components;
using VECTO3GUI2020.ViewModel.MultiStage.Implementation;

namespace VECTO3GUI2020.Ninject.Vehicle
{
    public class ComponentModule : NinjectModule
    {
        public override void Load()
        {

			#region IComponentViewModelFactory



			Bind<IVehicleViewModel>().To<VehicleViewModel_v1_0>().Named(VehicleViewModel_v1_0.VERSION);
			Bind<IVehicleViewModel>().To<VehicleViewModel_v2_0>().Named(VehicleViewModel_v2_0.VERSION);


	        
			Bind<IVehicleViewModel>().To<InterimStageBusVehicleViewModel_v2_8>().Named(InterimStageBusVehicleViewModel_v2_8.VERSION);
			Bind<IVehicleViewModel>().To<InterimStageBusVehicleViewModel_v2_8>().Named(InterimStageBusVehicleViewModel_v2_8.VERSION_EXEMPTED);


            Bind<IComponentViewModel>().To<EngineViewModel_v1_0>().Named(EngineViewModel_v1_0.VERSION);
            Bind<IComponentViewModel>().To<EngineViewModel_v2_0>().Named(EngineViewModel_v2_0.VERSION);

            Bind<IComponentViewModel>().To<AirDragViewModel_v1_0>().Named(AirDragViewModel_v1_0.VERSION);
            Bind<IComponentViewModel>().To<AirDragViewModel_v2_0>().Named(AirDragViewModel_v2_0.VERSION);
            Bind<IComponentViewModel>().To<AirDragViewModel_v2_4>().Named(AirDragViewModel_v2_4.VERSION);

            Bind<IComponentViewModel>().To<AxleWheelsViewModel_v1_0>().Named(AxleWheelsViewModel_v1_0.VERSION);
            Bind<IComponentViewModel>().To<AxleWheelsViewModel_v2_0>().Named(AxleWheelsViewModel_v2_0.VERSION);

            Bind<IComponentViewModel>().To<AxleGearViewModel_v1_0>().Named(AxleGearViewModel_v1_0.VERSION);
            Bind<IComponentViewModel>().To<AxleGearViewModel_v2_0>().Named(AxleGearViewModel_v2_0.VERSION);

            Bind<IComponentViewModel>().To<RetarderViewModel_v1_0>().Named(RetarderViewModel_v1_0.VERSION);
            Bind<IComponentViewModel>().To<RetarderViewModel_v2_0>().Named(RetarderViewModel_v2_0.VERSION);

            Bind<IComponentViewModel>().To<GearboxViewModel_v1_0>().Named(GearboxViewModel_v1_0.VERSION);
            Bind<IComponentViewModel>().To<GearboxViewModel_v2_0>().Named(GearboxViewModel_v2_0.VERSION);

            Bind<IComponentViewModel>().To<AuxiliariesViewModel_v1_0>().Named(AuxiliariesViewModel_v1_0.VERSION);
            Bind<IComponentViewModel>().To<AuxiliariesViewModel_v2_0>().Named(AuxiliariesViewModel_v2_0.VERSION);

            Bind<IComponentViewModel>().To<AxleViewModel_v1_0>().Named(AxleViewModel_v1_0.VERSION);
            Bind<IComponentViewModel>().To<AxleViewModel_v2_0>().Named(AxleViewModel_v2_0.VERSION);

            Bind<IComponentViewModel>().To<TyreViewModel_v1_0>().Named(TyreViewModel_v1_0.VERSION);
            Bind<IComponentViewModel>().To<TyreViewModel_v2_0>().Named(TyreViewModel_v2_0.VERSION);
            Bind<IComponentViewModel>().To<TyreViewModel_v2_2>().Named(TyreViewModel_v2_2.VERSION);
            Bind<IComponentViewModel>().To<TyreViewModel_v2_3>().Named(TyreViewModel_v2_3.VERSION);

            Bind<IComponentViewModel>().To<GearViewModel_v1_0>().Named(GearViewModel_v1_0.VERSION);
            Bind<IComponentViewModel>().To<GearViewModel_v2_0>().Named(GearViewModel_v2_0.VERSION);

            Bind<IComponentViewModel>().To<PTOViewModel_V1_0>().Named(PTOViewModel_V1_0.VERSION);
            Bind<IComponentViewModel>().To<PTOViewModel_V2_0>().Named(PTOViewModel_V2_0.VERSION);

            Bind<IComponentViewModel>().To<AngleDriveViewModel_v1_0>().Named(AngleDriveViewModel_v1_0.VERSION);
            Bind<IComponentViewModel>().To<AngleDriveViewModel_v2_0>().Named(AngleDriveViewModel_v2_0.VERSION);

            Bind<IComponentViewModel>().To<AuxiliaryViewModel_v1_0>().Named(AuxiliaryViewModel_v1_0.VERSION);
            Bind<IComponentViewModel>().To<AuxiliaryViewModel_v2_0>().Named(AuxiliaryViewModel_v2_0.VERSION);
            Bind<IComponentViewModel>().To<AuxiliaryViewModel_v2_3>().Named(AuxiliaryViewModel_v2_3.VERSION);

            Bind<IAdasViewModel>().To<ADASViewModel_v1_0>().Named(ADASViewModel_v1_0.VERSION);
            Bind<IAdasViewModel>().To<ADASViewModel_v2_1>().Named(ADASViewModel_v2_1.VERSION);
            //Bind<IAdasViewModel>().To<ADASViewModel_v2>().Named(ADASViewModel_v2_3.VERSION);
            Bind<IAdasViewModel>().To<ADASViewModel_v2_1>(); //Default ADAS ViewModel if no matching binding is available;

            Bind<IEngineModeViewModel>().To<EngineModeViewModelSingleFuel>()
                .Named(EngineModeViewModelSingleFuel.VERSION);

			Bind<IEngineFuelViewModel>().To<EngineFuelViewModel>();


			Bind<IComponentViewModel>().To<TorqueConverterViewModel_v1_0>()
                .Named(TorqueConverterViewModel_v1_0.VERSION);
            Bind<IComponentViewModel>().To<TorqueConverterViewModel_v2_0>()
                .Named(TorqueConverterViewModel_v2_0.VERSION);


            Bind<IComponentsViewModel>().To<ComponentsViewModel_v1_0>().Named(ComponentsViewModel_v1_0.VERSION);
            Bind<IComponentsViewModel>().To<ComponentsViewModel_v2_0>().Named(ComponentsViewModel_v2_0.VERSION);

            Bind<ICommonComponentViewModel>().To<CommonComponentViewModel>();
            #endregion



            #region AuxiliaryModelFactory
            Bind<IAuxiliaryModelFactory>().ToFactory(() => new UseFirstArgumentAsNameInstanceProvider());

            Bind<IDeclarationAuxiliaryTable>().To<ElectricSystem>().
                Named(AuxiliaryType.ElectricSystem.ToString());

            Bind<IDeclarationAuxiliaryTable>().To<Fan>().
                Named(AuxiliaryType.Fan.ToString());

            Bind<IDeclarationAuxiliaryTable>().To<HeatingVentilationAirConditioning>().
                Named(AuxiliaryType.HVAC.ToString());

            Bind<IDeclarationAuxiliaryTable>().To<PneumaticSystem>().
                Named(AuxiliaryType.PneumaticSystem.ToString());

            Bind<IDeclarationAuxiliaryTable>().To<SteeringPump>().
                Named(AuxiliaryType.SteeringPump.ToString());

            #endregion
		}
    }
}
