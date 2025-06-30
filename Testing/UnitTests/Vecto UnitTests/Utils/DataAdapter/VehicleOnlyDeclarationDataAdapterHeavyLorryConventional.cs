using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.Reader.DataObjectAdapter.HeavyLorry;
using TUGraz.VectoCore.InputData.Reader.DataObjectAdapter.SimulationComponents.Interfaces;
using TUGraz.VectoCore.InputData.Reader.DataObjectAdapter.SimulationComponents.StrategyDataAdapter;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.SimulationComponent;
using TUGraz.VectoCore.Models.SimulationComponent.Data;

namespace TUGraz.Vecto.UnitTests.Utils.DataAdapter;

public class VehicleOnlyDeclarationDataAdapterHeavyLorryConventional : DeclarationDataAdapterHeavyLorry.Conventional
{
	protected override IAuxiliaryDataAdapter AuxDataAdapter { get; } = new NullAuxiliaryDataAdapter();
	protected override IHybridStrategyDataAdapter HybridStrategyDataAdapter => throw new NotImplementedException();
	protected override IPTODataAdapter PtoDataAdapter { get; } = new NullPtoDataAdapterLorry();

	protected override IGearboxDataAdapter GearboxDataAdapter { get; } = new NullGearboxDataAdapter();

	
	protected override IAxleGearDataAdapter AxleGearDataAdapter { get; } = new NullAxlegearDataAdapter();
	
	protected override IRetarderDataAdapter RetarderDataAdapter { get; } = new NullRetarderDataAdapter();
	
	protected override IEngineDataAdapter EngineDataAdapter { get; } = new NullCombustionEngineDataAdapter();

}

public class NullAuxiliaryDataAdapter : IAuxiliaryDataAdapter
{
	#region Implementation of IAuxiliaryDataAdapter

	public IList<VectoRunData.AuxData> CreateAuxiliaryData(IAuxiliariesDeclarationInputData auxInputData, IBusAuxiliariesDeclarationData busAuxData,
		MissionType mission, VehicleClass hvdClass, Meter vehicleLength, int? numSteeredAxles,
		VectoSimulationJobType jobType)
	{
		return new List<VectoRunData.AuxData>();
	}

	#endregion
}

public class NullPtoDataAdapterLorry : IPTODataAdapter
{
	#region Implementation of IPTODataAdapter

	public PTOData CreatePTOTransmissionData(IPTOTransmissionInputData pto, IGearboxDeclarationInputData gbx)
	{
		return null;
	}

	public PTOData CreateDefaultPTOData(IPTOTransmissionInputData pto, IGearboxDeclarationInputData gbx)
	{
		return null;
	}

	#endregion
}

public class NullCombustionEngineDataAdapter : IEngineDataAdapter
{
	#region Implementation of IEngineDataAdapter

	public CombustionEngineData CreateEngineData(IVehicleDeclarationInputData primaryVehicle, IEngineModeDeclarationInputData mode,
		Mission mission)
	{
		return new CombustionEngineData() {

		};
	}

	public CombustionEngineData CreateEngineData(IVehicleDeclarationInputData primaryVehicle, int modeIdx, Mission mission)
	{
		return null;
	}

	#endregion
}

public class NullGearboxDataAdapter : IGearboxDataAdapter
{
	#region Implementation of IGearboxDataAdapter

	public GearboxData CreateGearboxData(IVehicleDeclarationInputData inputData, VectoRunData runData,
		IShiftPolygonCalculator shiftPolygonCalculator, GearboxType[] supportedGearboxTypes)
	{
		return null;
	}

	public ShiftStrategyParameters CreateGearshiftData(double axleRatio, PerSecond engineIdlingSpeed, GearboxType gearboxType,
		int gearsCount)
	{
		return null;
	}

	#endregion
}

public class NullAxlegearDataAdapter : IAxleGearDataAdapter
{
	#region Implementation of IAxleGearDataAdapter

	public AxleGearData CreateAxleGearData(IAxleGearInputData data)
	{
		return null;
	}

	public AxleGearData CreateDummyAxleGearData(IGearboxDeclarationInputData gbxData)
	{
		return null;
	}

	#endregion
}

public class NullRetarderDataAdapter : IRetarderDataAdapter
{
	#region Implementation of IRetarderDataAdapter

	public RetarderData CreateRetarderData(IRetarderInputData retarder, ArchitectureID architecture,
		IIEPCDeclarationInputData iepcInputData)
	{
		return null;
	}

	#endregion
}
