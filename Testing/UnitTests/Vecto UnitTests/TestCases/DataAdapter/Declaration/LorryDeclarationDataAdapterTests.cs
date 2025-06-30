using NUnit.Framework;
using System.Reflection;
using TUGraz.VectoCore.InputData.Reader.DataObjectAdapter.HeavyLorry;
using TUGraz.VectoCore.InputData.Reader.DataObjectAdapter.SimulationComponents;
using TUGraz.VectoCore.InputData.Reader.DataObjectAdapter.SimulationComponents.AuxiliaryDataAdapter;
using TUGraz.VectoCore.InputData.Reader.DataObjectAdapter.SimulationComponents.StrategyDataAdapter;

namespace TUGraz.Vecto.UnitTests.TestCases.DataAdapter.Declaration;

public class LorryDeclarationDataAdapterTests
{
	
	private static readonly Dictionary<string, Type?> CompDataAdapter_Conventional = new() {
		{"EngineDataAdapter", typeof(CombustionEngineComponentDataAdapter)},
		{"GearboxDataAdapter", typeof(GearboxDataAdapter)},
		{"AxleGearDataAdapter", typeof(AxleGearDataAdapter)},
		{"RetarderDataAdapter", typeof(RetarderDataAdapter)},
		{"AirdragDataAdapter", typeof(AirdragDataAdapter)},
		{"AngleDriveDataAdapter", typeof(AngledriveDataAdapter)},
		{"VehicleDataAdapter", typeof(LorryVehicleDataAdapter)},
		{"AuxDataAdapter", typeof(HeavyLorryAuxiliaryDataAdapter)},
		{"PtoDataAdapter", typeof(PTODataAdapterLorry)},
		{"ElectricMachinesDataAdapter", null},
		{"HybridStrategyDataAdapter", null},
		{"DriverDataAdapter", typeof(LorryDriverDataAdapter)}
    };

	private static readonly Dictionary<string, Type?> CompDataAdapter_ParallelHybrid = new() {
		{"EngineDataAdapter", typeof(CombustionEngineComponentDataAdapter)},
		{"GearboxDataAdapter", typeof(GearboxDataAdapter)},
		{"AxleGearDataAdapter", typeof(AxleGearDataAdapter)},
		{"RetarderDataAdapter", typeof(RetarderDataAdapter)},
		{"AirdragDataAdapter", typeof(AirdragDataAdapter)},
		{"AngleDriveDataAdapter", typeof(AngledriveDataAdapter)},
		{"VehicleDataAdapter", typeof(LorryVehicleDataAdapter)},
		{"AuxDataAdapter", typeof(HeavyLorryAuxiliaryDataAdapter)},
		{"PtoDataAdapter", typeof(PTODataAdapterLorry)},
		{"ElectricMachinesDataAdapter", typeof(ElectricMachinesDataAdapter)},
		{"HybridStrategyDataAdapter", typeof(ParallelHybridStrategyParameterDataAdapter)},
		{"DriverDataAdapter", typeof(LorryDriverDataAdapter)}
	};

	private static readonly Dictionary<string, Type?> CompDataAdapter_SeriallHybrid_S2 = new() {
		{"EngineDataAdapter", typeof(CombustionEngineComponentDataAdapter)},
		{"GearboxDataAdapter", typeof(GearboxDataAdapter)},
		{"AxleGearDataAdapter", typeof(AxleGearDataAdapter)},
		{"RetarderDataAdapter", typeof(RetarderDataAdapter)},
		{"AirdragDataAdapter", typeof(AirdragDataAdapter)},
		{"AngleDriveDataAdapter", typeof(AngledriveDataAdapter)},
		{"VehicleDataAdapter", typeof(LorryVehicleDataAdapter)},
		{"AuxDataAdapter", typeof(HeavyLorryAuxiliaryDataAdapter)},
		{"PtoDataAdapter", typeof(PTODataAdapterLorry)},
		{"ElectricMachinesDataAdapter", typeof(ElectricMachinesDataAdapter)},
		{"HybridStrategyDataAdapter", typeof(SerialHybridStrategyParameterDataAdapter)},
		{"DriverDataAdapter", typeof(LorryDriverDataAdapter)}
	};

	private static readonly Dictionary<string, Type?> CompDataAdapter_SeriallHybrid_S_IEPC = new() {
		{"EngineDataAdapter", typeof(CombustionEngineComponentDataAdapter)},
		{"GearboxDataAdapter", typeof(IEPCGearboxDataAdapter)},
		{"AxleGearDataAdapter", typeof(AxleGearDataAdapter)},
		{"RetarderDataAdapter", typeof(RetarderDataAdapter)},
		{"AirdragDataAdapter", typeof(AirdragDataAdapter)},
		{"AngleDriveDataAdapter", typeof(AngledriveDataAdapter)},
		{"VehicleDataAdapter", typeof(LorryVehicleDataAdapter)},
		{"AuxDataAdapter", typeof(HeavyLorryAuxiliaryDataAdapter)},
		{"PtoDataAdapter", typeof(PTODataAdapterLorry)},
		{"ElectricMachinesDataAdapter", typeof(ElectricMachinesDataAdapter)},
		{"HybridStrategyDataAdapter", typeof(SerialHybridStrategyParameterDataAdapter)},
		{"DriverDataAdapter", typeof(LorryDriverDataAdapter)}
	};


    private static readonly Dictionary<string, Type?> CompDataAdapter_SeriallHybrid_Sx = new() {
		{"EngineDataAdapter", typeof(CombustionEngineComponentDataAdapter)},
		{"GearboxDataAdapter", null},
		{"AxleGearDataAdapter", typeof(AxleGearDataAdapter)},
		{"RetarderDataAdapter", typeof(RetarderDataAdapter)},
		{"AirdragDataAdapter", typeof(AirdragDataAdapter)},
		{"AngleDriveDataAdapter", typeof(AngledriveDataAdapter)},
		{"VehicleDataAdapter", typeof(LorryVehicleDataAdapter)},
		{"AuxDataAdapter", typeof(HeavyLorryAuxiliaryDataAdapter)},
		{"PtoDataAdapter", typeof(PTODataAdapterLorry)},
		{"ElectricMachinesDataAdapter", typeof(ElectricMachinesDataAdapter)},
		{"HybridStrategyDataAdapter", typeof(SerialHybridStrategyParameterDataAdapter)},
		{"DriverDataAdapter", typeof(LorryDriverDataAdapter)}
	};

	private static readonly Dictionary<string, Type?> CompDataAdapter_SeriallHybrid_E2 = new() {
		{"EngineDataAdapter", null},
		{"GearboxDataAdapter", typeof(GearboxDataAdapter)},
		{"AxleGearDataAdapter", typeof(AxleGearDataAdapter)},
		{"RetarderDataAdapter", typeof(RetarderDataAdapter)},
		{"AirdragDataAdapter", typeof(AirdragDataAdapter)},
		{"AngleDriveDataAdapter", typeof(AngledriveDataAdapter)},
		{"VehicleDataAdapter", typeof(LorryVehicleDataAdapter)},
		{"AuxDataAdapter", typeof(HeavyLorryAuxiliaryDataAdapter)},
		{"PtoDataAdapter", typeof(PTODataAdapterLorry)},
		{"ElectricMachinesDataAdapter", typeof(ElectricMachinesDataAdapter)},
		{"HybridStrategyDataAdapter", null},
	};

	private static readonly Dictionary<string, Type?> CompDataAdapter_SeriallHybrid_E_IEPC = new() {
		{"EngineDataAdapter", null},
		{"GearboxDataAdapter", typeof(IEPCGearboxDataAdapter)},
		{"AxleGearDataAdapter", typeof(AxleGearDataAdapter)},
		{"RetarderDataAdapter", typeof(RetarderDataAdapter)},
		{"AirdragDataAdapter", typeof(AirdragDataAdapter)},
		{"AngleDriveDataAdapter", typeof(AngledriveDataAdapter)},
		{"VehicleDataAdapter", typeof(LorryVehicleDataAdapter)},
		{"AuxDataAdapter", typeof(HeavyLorryAuxiliaryDataAdapter)},
		{"PtoDataAdapter", typeof(PTODataAdapterLorry)},
		{"ElectricMachinesDataAdapter", typeof(ElectricMachinesDataAdapter)},
		{"HybridStrategyDataAdapter", null},
		{"DriverDataAdapter", typeof(LorryDriverDataAdapter)}
	};

	private static readonly Dictionary<string, Type?> CompDataAdapter_SeriallHybrid_Ex = new() {
		{"EngineDataAdapter", null},
		{"GearboxDataAdapter", null},
		{"AxleGearDataAdapter", typeof(AxleGearDataAdapter)},
		{"RetarderDataAdapter", typeof(RetarderDataAdapter)},
		{"AirdragDataAdapter", typeof(AirdragDataAdapter)},
		{"AngleDriveDataAdapter", typeof(AngledriveDataAdapter)},
		{"VehicleDataAdapter", typeof(LorryVehicleDataAdapter)},
		{"AuxDataAdapter", typeof(HeavyLorryAuxiliaryDataAdapter)},
		{"PtoDataAdapter", typeof(PTODataAdapterLorry)},
		{"ElectricMachinesDataAdapter", typeof(ElectricMachinesDataAdapter)},
		{"HybridStrategyDataAdapter", null},
		{"DriverDataAdapter", typeof(LorryDriverDataAdapter)}
	};

	private static readonly Dictionary<string, Type?> CompDataAdapter_Exempted = new() {
		{"EngineDataAdapter", null},
		{"GearboxDataAdapter", null},
		{"AxleGearDataAdapter", null},
		{"RetarderDataAdapter", null},
		{"AirdragDataAdapter", null},
		{"AngleDriveDataAdapter", null},
		{"VehicleDataAdapter", typeof(ExemptedLorryVehicleDataAdapter)},
		{"AuxDataAdapter", null},
		{"PtoDataAdapter", null},
		{"ElectricMachinesDataAdapter", null},
		{"HybridStrategyDataAdapter", null},
		{"DriverDataAdapter", typeof(LorryDriverDataAdapter)}
	};

    private static readonly Dictionary<Type, Dictionary<string, Type?>> ComponentDataAdapterMapping = new() {
			{ typeof(DeclarationDataAdapterHeavyLorry.Conventional), CompDataAdapter_Conventional },
			{ typeof(DeclarationDataAdapterHeavyLorry.HEV_P1), CompDataAdapter_ParallelHybrid },
			{ typeof(DeclarationDataAdapterHeavyLorry.HEV_P2), CompDataAdapter_ParallelHybrid },
			{ typeof(DeclarationDataAdapterHeavyLorry.HEV_P2_5), CompDataAdapter_ParallelHybrid },
			{ typeof(DeclarationDataAdapterHeavyLorry.HEV_P3), CompDataAdapter_ParallelHybrid },
			{ typeof(DeclarationDataAdapterHeavyLorry.HEV_P4), CompDataAdapter_ParallelHybrid },
			{ typeof(DeclarationDataAdapterHeavyLorry.HEV_S2), CompDataAdapter_SeriallHybrid_S2 },
			{ typeof(DeclarationDataAdapterHeavyLorry.HEV_S3), CompDataAdapter_SeriallHybrid_Sx },
			{ typeof(DeclarationDataAdapterHeavyLorry.HEV_S4), CompDataAdapter_SeriallHybrid_Sx },
			{ typeof(DeclarationDataAdapterHeavyLorry.HEV_S_IEPC), CompDataAdapter_SeriallHybrid_S_IEPC },
			{ typeof(DeclarationDataAdapterHeavyLorry.PEV_E2), CompDataAdapter_SeriallHybrid_E2},
			{ typeof(DeclarationDataAdapterHeavyLorry.PEV_E3), CompDataAdapter_SeriallHybrid_Ex },
			{ typeof(DeclarationDataAdapterHeavyLorry.PEV_E4), CompDataAdapter_SeriallHybrid_Ex },
			{ typeof(DeclarationDataAdapterHeavyLorry.PEV_E_IEPC), CompDataAdapter_SeriallHybrid_E_IEPC },
			{ typeof(DeclarationDataAdapterHeavyLorry.Exempted), CompDataAdapter_Exempted },
		};

	[TestCaseSource(nameof(GetArchitectures))]
	public void TestComponentDataAdapterLorry(Type dataAdapterType)
	{
		var dao = Activator.CreateInstance(dataAdapterType);
		Assert.IsNotNull(dao);
		Assert.IsInstanceOf(dataAdapterType, dao);

		var componentDataAdapter = GetExpectedComponentDataAdapters(dataAdapterType);

		foreach (var (component, expectedType) in componentDataAdapter) {
			try {
				var componentDao = GetDataAdapter(dao, component);
				if (expectedType == null) {
					Assert.IsNull(componentDao);
				} else {
					Assert.IsNotNull(componentDao);
					Assert.IsInstanceOf(expectedType, componentDao);
				}
			} catch (Exception ex) {
				if (expectedType != null) {
					throw;
				}
			}
		}
	}

	public static IEnumerable<Type> GetArchitectures()
	{
		return ComponentDataAdapterMapping.Keys;
	}

	private Dictionary<string, Type?> GetExpectedComponentDataAdapters(Type dataAdapterType)
	{
		if (!ComponentDataAdapterMapping.ContainsKey(dataAdapterType)) {
			throw new Exception($"No mapping for data adapter {dataAdapterType.Name} defined");
		}

		return ComponentDataAdapterMapping[dataAdapterType];
	}

	private object? GetDataAdapter(object dao, string adapterName)
	{
		return dao.GetType().GetProperty(adapterName, BindingFlags.NonPublic | BindingFlags.Instance)?.GetValue(dao, null);

	}
}