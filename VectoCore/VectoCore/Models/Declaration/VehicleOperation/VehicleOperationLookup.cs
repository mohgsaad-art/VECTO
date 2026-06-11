using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;

namespace TUGraz.VectoCore.Models.Declaration.VehicleOperation
{
	public interface IVehicleOperationLookup
	{
		VehicleOperationLookup.VehicleOperationData LookupVehicleOperation(VehicleClass hdvClass, MissionType mission);
	}
	public class VehicleOperationLookup : IVehicleOperationLookup
	{

		private MileageLookup _mileageLookup = new MileageLookup();

		private StationaryChargingDurationPerEventLookup _chargingDurationLookup =
			new StationaryChargingDurationPerEventLookup();

		private StationaryChargingPowerFromInfrastructureLookup _chargingPowerFromInfrastructureLookup =
			new StationaryChargingPowerFromInfrastructureLookup();

		private StationaryChargingEventsPerDayLookup _numberOfChargingEventsLookup =
			new StationaryChargingEventsPerDayLookup();

		private RealWorldUsageFactors _realWorldUsageFactors = new RealWorldUsageFactors();

		private ShareInMotionChargingInfrastructureLookup _inMotionChargingShare = new ShareInMotionChargingInfrastructureLookup();


		public VehicleOperationData LookupVehicleOperation(VehicleClass hdvClass, MissionType mission)
		{
			return new VehicleOperationData() {
				Mileage = _mileageLookup.Lookup(hdvClass, mission),
				StationaryChargingMaxPwrInfrastructure = _chargingPowerFromInfrastructureLookup.Lookup(hdvClass, mission),
				StationaryChargingDuringMission_AvgDurationPerEvent = _chargingDurationLookup.Lookup(hdvClass, mission),
				StationaryChargingDuringMission_NbrEvents = _numberOfChargingEventsLookup.Lookup(hdvClass, mission),
				RealWorldUsageFactors = _realWorldUsageFactors.Lookup(hdvClass),
				ShareInMotionCharging = _inMotionChargingShare.Lookup(hdvClass, mission),
			};
		}

		public class VehicleOperationData
		{
			public MileageLookup.MileageEntry Mileage { get; internal set; }

			public Watt StationaryChargingMaxPwrInfrastructure { get; internal set; }

			public Second StationaryChargingDuringMission_AvgDurationPerEvent { get; internal set; }

			public double StationaryChargingDuringMission_NbrEvents { get; internal set; }

			public RealWorldUsageFactors.Entry RealWorldUsageFactors { get; internal set; }

			public ShareInMotionChargingInfrastructureLookup.Entry ShareInMotionCharging { get; internal set; }
		}
	}
}