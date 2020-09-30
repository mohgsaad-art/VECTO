using System;
using System.Collections.Generic;
using VECTOTrailer.ViewModel.Interfaces;
using VECTOTrailer.ViewModel.Interfaces;

namespace VECTOTrailer.Util
{
	public static class ViewModelFactory
	{

		public static Dictionary<Component, Type> ComponentViewModelMapping = new Dictionary<Component, Type>()
		{
			{ Component.Vehicle, typeof(IVehicleViewModel)},
			{ Component.CompleteBusVehicle, typeof(ICompleteVehicleBusViewModel)},
			{ Component.PrimaryBusVehicle, typeof(IPrimaryVehicleBusViewModel) },
			{ Component.Engine ,typeof(IEngineViewModel) },
			{ Component.Gearbox ,typeof(IGearboxViewModel) },
			{ Component.TorqueConverter ,typeof(ITorqueConverterViewModel) },
			{ Component.Retarder ,typeof(IRetarderViewModel) },
			{ Component.Angledrive ,typeof(IAngledriveViewModel) },
			{ Component.Axlegear ,typeof(IAxlegearViewModel) },
			{ Component.Auxiliaries, typeof(IAuxiliariesViewModel) },
			{ Component.Airdrag ,typeof(IAirdragViewModel) },
			{ Component.Cycle ,typeof(ICyclesViewModel) },
			{ Component.Axles, typeof(IAxlesViewModel) }

		};

		//public static IComponentViewModel CreateViewModel(IKernel kernel, Component component, DeclarationJobViewModel jobEditViewModel)
		//{
		//	throw new System.NotImplementedException();
		//}
	}
}
