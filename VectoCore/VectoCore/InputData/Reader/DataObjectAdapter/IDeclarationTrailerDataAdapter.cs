using System;
using System.Collections.Generic;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Interfaces;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.SimulationComponent;
using TUGraz.VectoCore.Models.SimulationComponent.Data;

namespace TUGraz.VectoCore.InputData.Reader.DataObjectAdapter {
	public interface IDeclarationTrailerDataAdapter
	{
		DriverData CreateDriverData();
		VehicleData CreateVehicleData(ITrailerDeclarationInputData vehicle, Segment segment, Mission mission, KeyValuePair<LoadingType, Tuple<Kilogram, double?>> loading);
		AirdragData CreateAirdragData(IAirdragDeclarationInputData airdragData, Mission mission, Segment segment);
		AxleGearData CreateAxleGearData(IAxleGearInputData axlegearData);
		AngledriveData CreateAngledriveData(IAngledriveInputData angledriveData);
		CombustionEngineData CreateEngineData(IEngineDeclarationInputData engine, IGearboxDeclarationInputData gearbox,
			IEngineModeDeclarationInputData mode, Mission mission);

		GearboxData CreateGearboxData(IGearboxDeclarationInputData gearbox, IVehicleComponentsDeclaration components, VectoRunData runData, IShiftPolygonCalculator shiftPolygonCalc);

		ShiftStrategyParameters CreateGearshiftData(GearboxData gbx, double axleRatio, PerSecond engineIdlingSpeed);

		RetarderData CreateRetarderData(IRetarderInputData retarderData);
		PTOData CreatePTOTransmissionData(IPTOTransmissionInputData ptoData);
		IList<VectoRunData.AuxData> CreateAuxiliaryData(IAuxiliariesDeclarationInputData auxData, IBusAuxiliariesDeclarationData busAuxData, MissionType missionType, VehicleClass vehicleClass, Meter vehicleLength);
		AxleGearData CreateDummyAxleGearData(IGearboxDeclarationInputData gbxData);
	}
}