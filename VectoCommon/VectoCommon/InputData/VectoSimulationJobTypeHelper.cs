using System;
using TUGraz.VectoCommon.Utils;

namespace TUGraz.VectoCommon.InputData
{
	public static class VectoSimulationJobTypeHelper
	{
		public const string Conventional = "Conventional";
		public const string Hybrid = "Hybrid";
		public const string PureElectric = "PureElectric";
		public const string FuelCell = "FuelCell";

		public static bool IsBatteryElectric(this VectoSimulationJobType jobType)
		{
			return jobType == VectoSimulationJobType.BatteryElectricVehicle || jobType == VectoSimulationJobType.IEPC_E || jobType == VectoSimulationJobType.Multiple_PEV;
		}

		public static bool IsIEPC(this VectoSimulationJobType jobType)
		{
			return jobType == VectoSimulationJobType.IEPC_S || jobType == VectoSimulationJobType.IEPC_E || jobType == VectoSimulationJobType.FCHV_IEPC;
		}

		public static bool IsFCHV(this VectoSimulationJobType jobType)
		{
			return jobType == VectoSimulationJobType.FCHV || jobType == VectoSimulationJobType.FCHV_IEPC || jobType == VectoSimulationJobType.Multiple_FCHV;
		}

		public static bool IsMultiplePowertrains(this VectoSimulationJobType jobType)
		{
			return jobType.IsOneOf(VectoSimulationJobType.Multiple_SHEV, VectoSimulationJobType.Multiple_FCHV, VectoSimulationJobType.Multiple_PEV);
		}

		public static string GetPowertrainArchitectureType(this VectoSimulationJobType jobType)
		{
			switch (jobType) {
				case VectoSimulationJobType.EngineOnlySimulation:
				case VectoSimulationJobType.ConventionalVehicle:
					return Conventional;
				case VectoSimulationJobType.ParallelHybridVehicle:
				case VectoSimulationJobType.SerialHybridVehicle:
				case VectoSimulationJobType.IHPC:
				case VectoSimulationJobType.IEPC_S:
				case VectoSimulationJobType.Multiple_SHEV:
					return Hybrid;
				case VectoSimulationJobType.BatteryElectricVehicle:
				case VectoSimulationJobType.IEPC_E:
				case VectoSimulationJobType.Multiple_PEV:
					return PureElectric;
				case VectoSimulationJobType.FCHV:
				case VectoSimulationJobType.FCHV_IEPC:
				case VectoSimulationJobType.Multiple_FCHV:
					return FuelCell;
				default:
					throw new ArgumentOutOfRangeException(nameof(jobType), jobType, null);
			}
		}

		public static ArchitectureID GetArchitectureID(this VectoSimulationJobType jobType, PowertrainPosition em)
		{
			switch (jobType) {
				case VectoSimulationJobType.ConventionalVehicle:
				case VectoSimulationJobType.EngineOnlySimulation:
					return ArchitectureID.UNKNOWN;
				case VectoSimulationJobType.ParallelHybridVehicle:
					return GetPHEVArchitectureId(em);

				case VectoSimulationJobType.SerialHybridVehicle:
				case VectoSimulationJobType.Multiple_SHEV when (em != PowertrainPosition.IEPC):
					return GetSHEVArchitecureID(em);

				case VectoSimulationJobType.BatteryElectricVehicle:
				case VectoSimulationJobType.Multiple_PEV when (em != PowertrainPosition.IEPC):
					return GetPEVArchId(emPos: em);

				case VectoSimulationJobType.FCHV:
				case VectoSimulationJobType.Multiple_FCHV when (em != PowertrainPosition.IEPC):
					return GetFCHVArchId(emPos: em);

				case VectoSimulationJobType.IEPC_E:
				case VectoSimulationJobType.IEPC_S:
				case VectoSimulationJobType.FCHV_IEPC:
				case VectoSimulationJobType.Multiple_SHEV when (em == PowertrainPosition.IEPC):
				case VectoSimulationJobType.Multiple_FCHV when (em == PowertrainPosition.IEPC):
				case VectoSimulationJobType.Multiple_PEV when (em == PowertrainPosition.IEPC):
					return GetIepcArchitectureId(jobType, em);

				case VectoSimulationJobType.IHPC:
					return ArchitectureID.P2;
				default:
					throw new ArgumentOutOfRangeException(nameof(jobType), jobType, null);
			}
		}

		public static bool HasEngine(this VectoSimulationJobType jobType)
		{
			switch (jobType) {
				case VectoSimulationJobType.ConventionalVehicle:
				case VectoSimulationJobType.ParallelHybridVehicle:
				case VectoSimulationJobType.SerialHybridVehicle:
				case VectoSimulationJobType.EngineOnlySimulation:
				case VectoSimulationJobType.IHPC:
				case VectoSimulationJobType.IEPC_S:
				case VectoSimulationJobType.Multiple_SHEV:
					return true;
				case VectoSimulationJobType.FCHV:
				case VectoSimulationJobType.FCHV_IEPC:
				case VectoSimulationJobType.BatteryElectricVehicle:
				case VectoSimulationJobType.IEPC_E:
				case VectoSimulationJobType.Multiple_FCHV:
				case VectoSimulationJobType.Multiple_PEV:
					return false;
				default:
					throw new ArgumentOutOfRangeException(nameof(jobType), jobType, null);
			}
		}

		private static ArchitectureID GetIepcArchitectureId(VectoSimulationJobType jobType, PowertrainPosition em)
		{
			if (em != PowertrainPosition.IEPC) {
				throw new ArgumentException(nameof(em));
			}

			switch (jobType) {
				case VectoSimulationJobType.IEPC_E:
                case VectoSimulationJobType.Multiple_PEV:
                    return ArchitectureID.E_IEPC;
				case VectoSimulationJobType.IEPC_S:
                case VectoSimulationJobType.Multiple_SHEV:
                    return ArchitectureID.S_IEPC;
                case VectoSimulationJobType.Multiple_FCHV:
                case VectoSimulationJobType.FCHV_IEPC:
					return ArchitectureID.F_IEPC;
				default:
					throw new ArgumentException(nameof(jobType));
			}
		}

		private static ArchitectureID GetPHEVArchitectureId(PowertrainPosition emPos)
		{
			switch (emPos) {
				case PowertrainPosition.HybridP1:
					return ArchitectureID.P1;
				case PowertrainPosition.HybridP2:
					return ArchitectureID.P2;
				case PowertrainPosition.HybridP2_5:
					return ArchitectureID.P2_5;
				case PowertrainPosition.HybridP3:
					return ArchitectureID.P3;
				case PowertrainPosition.HybridP4:
					return ArchitectureID.P4;
				default:
					throw new ArgumentOutOfRangeException(nameof(emPos));
			}
		}

		private static ArchitectureID GetSHEVArchitecureID(PowertrainPosition emPos)
		{
			switch (emPos) {
				case PowertrainPosition.BatteryElectricE4:
					return ArchitectureID.S4;
				case PowertrainPosition.BatteryElectricE3:
					return ArchitectureID.S3;
				case PowertrainPosition.BatteryElectricE2:
					return ArchitectureID.S2;
				default:
					throw new ArgumentOutOfRangeException(nameof(emPos));
			}
		}

		private static ArchitectureID GetPEVArchId(PowertrainPosition emPos)
		{
			switch (emPos) {
				case PowertrainPosition.BatteryElectricE4:
					return ArchitectureID.E4;
				case PowertrainPosition.BatteryElectricE3:
					return ArchitectureID.E3;
				case PowertrainPosition.BatteryElectricE2:
					return ArchitectureID.E2;
				default:
					throw new ArgumentOutOfRangeException(nameof(emPos));
			}
		}

		private static ArchitectureID GetFCHVArchId(PowertrainPosition emPos)
		{
			switch (emPos)
			{
				case PowertrainPosition.BatteryElectricE4:
					return ArchitectureID.F4;
				case PowertrainPosition.BatteryElectricE3:
					return ArchitectureID.F3;
				case PowertrainPosition.BatteryElectricE2:
					return ArchitectureID.F2;
				default:
					throw new ArgumentOutOfRangeException(nameof(emPos));
			}
		}
	}
}