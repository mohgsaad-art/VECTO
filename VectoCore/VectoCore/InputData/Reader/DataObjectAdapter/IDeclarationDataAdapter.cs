using System;
using System.Collections.Generic;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.SimulationComponent;
using TUGraz.VectoCore.Models.SimulationComponent.Data;

namespace TUGraz.VectoCore.InputData.Reader.DataObjectAdapter {
	public interface IDeclarationDataAdapter
	{
		DriverData CreateDriverData();
		VehicleData CreateVehicleData(IVehicleDeclarationInputData vehicle, Segment segment, Mission mission, KeyValuePair<LoadingType, Tuple<Kilogram, double?>> loading, bool allowVocational);
		AirdragData CreateAirdragData(IAirdragDeclarationInputData airdragData, Mission mission, Segment segment);
		AxleGearData CreateAxleGearData(IAxleGearInputData axlegearData);
		AngledriveData CreateAngledriveData(IAngledriveInputData angledriveData);
		CombustionEngineData CreateEngineData(IVehicleDeclarationInputData vehicle, IEngineModeDeclarationInputData engineMode, Mission mission);

		GearboxData CreateGearboxData(
			IVehicleDeclarationInputData inputData, VectoRunData runData,
			IShiftPolygonCalculator shiftPolygonCalc);

		ShiftStrategyParameters CreateGearshiftData(GearboxData gbx, double axleRatio, PerSecond engineIdlingSpeed);

		RetarderData CreateRetarderData(IRetarderInputData retarderData);
		PTOData CreatePTOTransmissionData(IPTOTransmissionInputData ptoData);
		IList<VectoRunData.AuxData> CreateAuxiliaryData(IAuxiliariesDeclarationInputData auxData, IBusAuxiliariesDeclarationData busAuxData, MissionType missionType, VehicleClass vehicleClass, Meter vehicleLength, int? numSteeredAxles);
		AxleGearData CreateDummyAxleGearData(IGearboxDeclarationInputData gbxData);
	}
}