using System;
using TUGraz.VectoCommon.BusAuxiliaries;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using VECTO3GUI.Util;

namespace VECTO3GUI.ViewModel.Interfaces
{
	public interface ICompleteVehicleBusViewModel : ICompleteVehicleBus,  IComponentViewModel 
	{
		AllowedEntry<LegislativeClass>[] AllowedLegislativeClasses { get; }
		AllowedEntry<VehicleCode>[] AllowedVehicleCodes { get; }
		AllowedEntry<ConsumerTechnology>[] AllowedDoorDriveTechnologies { get; }
		AllowedEntry<RegistrationClass>[] AllowedRegisteredClasses { get; }
		AllowedEntry<TankSystem?>[] AllowedTankSystems { get; }
	}
}
