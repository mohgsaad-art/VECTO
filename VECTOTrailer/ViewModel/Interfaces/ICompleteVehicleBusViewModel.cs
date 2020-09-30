using System;
using TUGraz.VectoCommon.BusAuxiliaries;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using VECTOTrailer.Util;

namespace VECTOTrailer.ViewModel.Interfaces
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
