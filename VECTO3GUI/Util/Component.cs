using TUGraz.VectoCommon.Resources;

namespace VECTO3GUI.Util {
	public enum Component
	{
		Vehicle = 1,
		PrimaryBusVehicle,
		Engine,
		Gearbox,
		TorqueConverter,
		Retarder,
		Angledrive,
		Axlegear,
		PTO,
		Airdrag,
		Axles,
		Auxiliaries,
		BusAuxiliaries,
		Cycle,
		CompleteBusVehicle
	}

	public static class ComponentHelper
	{
		public static string GetLabel(this Component component)
		{
			switch (component)
			{
				case Component.Vehicle:
				case Component.PrimaryBusVehicle:
				case Component.CompleteBusVehicle:
					return XMLNames.Component_Vehicle;
				case Component.Engine:
					return XMLNames.Component_Engine;
				case Component.Gearbox:
					return XMLNames.Component_Gearbox;
				case Component.TorqueConverter:
					return XMLNames.Component_TorqueConverter;
				case Component.Retarder:
					return XMLNames.Component_Retarder;
				case Component.Angledrive:
					return XMLNames.Component_Angledrive;
				case Component.Axlegear:
					return XMLNames.Component_Axlegear;
				case Component.PTO:
					return XMLNames.Vehicle_PTO;
				case Component.Airdrag:
					return XMLNames.Component_AirDrag;
				case Component.Axles:
					return XMLNames.AxleWheels_Axles_Axle;
				case Component.Auxiliaries:
				case Component.BusAuxiliaries:
					return XMLNames.Component_Auxiliaries;
				case Component.Cycle:
					return nameof(Component.Cycle);
			}

			return string.Empty;
		}
	}
}