using System;
using System.Linq;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.Reader.ComponentData;
using TUGraz.VectoCore.InputData.Reader.DataObjectAdapter.SimulationComponents.Interfaces;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data;

namespace TUGraz.VectoCore.InputData.Reader.DataObjectAdapter.SimulationComponents
{
    public class RetarderDataAdapter : IRetarderDataAdapter
	{
		public RetarderData CreateRetarderData(IRetarderInputData retarder, ArchitectureID architecture,
			IIEPCDeclarationInputData iepcInputData)
		{
			return SetCommonRetarderData(retarder, architecture, iepcInputData);
		}


		private bool TypeValid(RetarderType type, ArchitectureID archId,
			IIEPCDeclarationInputData iepc, out string errorMsg)
		{
			var valid = true;
			errorMsg = "";
			if (archId == ArchitectureID.UNKNOWN) {
				//Conventional vehicle
				return true;

			}

			switch (type) {
				case RetarderType.None:
					valid = true;
					break;
				case RetarderType.TransmissionInputRetarder:
					valid = archId.IsParallelHybridVehicle() || archId.IsOneOf(ArchitectureID.S2, ArchitectureID.E2, ArchitectureID.F2);
					break;
				case RetarderType.TransmissionOutputRetarder:
					valid = archId.IsParallelHybridVehicle() ||
							archId.IsOneOf(ArchitectureID.P_IHPC, ArchitectureID.S2, ArchitectureID.E2, ArchitectureID.F2);
					break;
				case RetarderType.EngineRetarder:
					valid = archId.IsParallelHybridVehicle() || archId.IsOneOf(ArchitectureID.P_IHPC);
					break;
				case RetarderType.LossesIncludedInTransmission:
					valid = archId.IsParallelHybridVehicle() ||
							archId.IsOneOf(ArchitectureID.P_IHPC, ArchitectureID.S2, ArchitectureID.S_IEPC, ArchitectureID.E2, ArchitectureID.F2) ||
							(archId.IsOneOf(ArchitectureID.E_IEPC, ArchitectureID.F_IEPC) && !iepc.DesignTypeWheelMotor);
					break;
				case RetarderType.AxlegearInputRetarder:
					valid = archId.IsOneOf(ArchitectureID.E3, ArchitectureID.S3, ArchitectureID.F3, ArchitectureID.S_IEPC) ||
							(archId.IsOneOf(ArchitectureID.E_IEPC, ArchitectureID.F_IEPC) && !iepc.DifferentialIncluded &&
							!iepc.DesignTypeWheelMotor);
					break;
				default:
					throw new ArgumentOutOfRangeException(nameof(type), type, null);
			}


			if (!valid) {
				errorMsg =
					$"Invalid retardertype ({type}) for architecture ({archId}";
				if (archId == ArchitectureID.E_IEPC) {
					errorMsg += $" Differential {(iepc.DifferentialIncluded? "": "not")} included\n";
					errorMsg += iepc.DesignTypeWheelMotor ? "Design type wheel motor" : "";
				}

				errorMsg += ")\n";
			}




			return valid;
		}

		private void SetRatioAndLossMap(IRetarderInputData inputData, RetarderData retarderData)
		{
			switch (inputData.Type) {
				case RetarderType.TransmissionInputRetarder:
				case RetarderType.TransmissionOutputRetarder:
				case RetarderType.AxlegearInputRetarder:
					retarderData.LossMap = RetarderLossMapReader.Create(inputData.LossMap);
					retarderData.Ratio = inputData.Ratio;
					break;
                case RetarderType.LossesIncludedInTransmission:
				case RetarderType.EngineRetarder:
				case RetarderType.None:
					retarderData.Ratio = 1.0;
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		internal RetarderData SetCommonRetarderData(IRetarderInputData retarderInputData,
			ArchitectureID architecture, IIEPCDeclarationInputData iepcDeclarationInputData)
		{
			if (!TypeValid(retarderInputData.Type, architecture, iepcDeclarationInputData, out var errorMsg)) {
				throw new VectoException("Error while Reading Retarder Data: {0}", errorMsg);
            }


			var retarder = new RetarderData { Type = retarderInputData?.Type ?? RetarderType.None };
			SetRatioAndLossMap(retarderInputData, retarder);

			if (retarder.Type.IsDedicatedComponent())
			{
					retarder.SavedInDeclarationMode = retarderInputData.SavedInDeclarationMode;
					retarder.Manufacturer = retarderInputData.Manufacturer;
					retarder.ModelName = retarderInputData.Model;
					retarder.Date = retarderInputData.Date;
					retarder.CertificationMethod = retarderInputData.CertificationMethod;
					retarder.CertificationNumber = retarderInputData.CertificationNumber;
					retarder.DigestValueInput = retarderInputData.DigestValue != null ? retarderInputData.DigestValue.DigestValue : "";
			}

			return retarder;
		}
		
	}

	public class GenericRetarderDataAdapter : IGenericRetarderDataAdapter
	{
		private readonly GenericBusRetarderData _genericRetarderData = new GenericBusRetarderData();

		public RetarderData CreateGenericRetarderData(IRetarderInputData retarder, VectoRunData vehicleData)
		{
			bool isBatteryElectric =
				vehicleData.JobType.IsOneOf(VectoSimulationJobType.BatteryElectricVehicle,
					VectoSimulationJobType.IEPC_E,
					VectoSimulationJobType.SerialHybridVehicle,
					VectoSimulationJobType.IEPC_S,
					VectoSimulationJobType.FCHV,
					VectoSimulationJobType.FCHV_IEPC,
					VectoSimulationJobType.Multiple_FCHV,
					VectoSimulationJobType.Multiple_PEV,
					VectoSimulationJobType.Multiple_SHEV);

			PerSecond maxMotorSpeed = isBatteryElectric
				? vehicleData.ElectricMachinesSinglePwt.First(x => x.Item1 != PowertrainPosition.GEN).Item2.EfficiencyData.MaxSpeed
				: vehicleData.EngineData.FullLoadCurves[0].MaxSpeed;

			double maxGbxRatio = vehicleData.GearboxSinglePwt?.Gears[(uint)vehicleData.GearboxSinglePwt.Gears.Count].Ratio ?? 1;
			double combinedRatios = isBatteryElectric
				? maxGbxRatio * vehicleData.ElectricMachinesSinglePwt.First(x => x.Item1 != PowertrainPosition.GEN).Item2.RatioADC
				: maxGbxRatio;

			return _genericRetarderData.CreateGenericBusRetarderData(retarder, maxMotorSpeed, combinedRatios);
        }

		public RetarderData CreateRetarderData(IRetarderInputData retarder, ArchitectureID architecture,
			IIEPCDeclarationInputData iepcInputData)
		{
			return _genericRetarderData.CreateGenericBusRetarderData(retarder);
		}
	}
}