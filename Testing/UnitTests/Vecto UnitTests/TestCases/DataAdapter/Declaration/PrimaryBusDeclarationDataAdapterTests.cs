using NUnit.Framework;
using System.Reflection;
using TUGraz.VectoCore.InputData.Reader.DataObjectAdapter.PrimaryBus;
using TUGraz.VectoCore.InputData.Reader.DataObjectAdapter.SimulationComponents;
using TUGraz.VectoCore.InputData.Reader.DataObjectAdapter.SimulationComponents.AuxiliaryDataAdapter;
using TUGraz.VectoCore.InputData.Reader.DataObjectAdapter.SimulationComponents.StrategyDataAdapter;

namespace TUGraz.Vecto.UnitTests.TestCases.DataAdapter.Declaration;

public class PrimaryBusDeclarationDataAdapterTests
{
	private static readonly Dictionary<string, Type?> CompDataAdapter_Conventional = new() {
		{"EngineDataAdapter", typeof(CombustionEngineComponentDataAdapter)},
		{"GearboxDataAdapter", typeof(GearboxDataAdapter)},
        {"AxleGearDataAdapter", typeof(AxleGearDataAdapter)},
        {"RetarderDataAdapter", typeof(RetarderDataAdapter)},
        {"AirdragDataAdapter", typeof(AirdragDataAdapter)},
        {"AngledriveDataAdapter", typeof(AngledriveDataAdapter)},
        {"VehicleDataAdapter", typeof(PrimaryBusVehicleDataAdapter)},
		{"AuxDataAdapter", typeof(PrimaryBusAuxiliaryDataAdapter)},
		{"ElectricMachinesDataAdapter", null},
		{"HybridStrategyDataAdapter", null},
		{"DriverDataAdapter", typeof(PrimaryBusDriverDataAdapter)}
	};

	private static readonly Dictionary<string, Type?> CompDataAdapter_ParallelHybrid = new() {
		{"EngineDataAdapter", typeof(CombustionEngineComponentDataAdapter)},
		{"GearboxDataAdapter", typeof(GearboxDataAdapter)},
		{"AxleGearDataAdapter", typeof(AxleGearDataAdapter)},
		{"RetarderDataAdapter", typeof(RetarderDataAdapter)},
		{"AirdragDataAdapter", typeof(AirdragDataAdapter)},
		{"AngledriveDataAdapter", typeof(AngledriveDataAdapter)},
		{"VehicleDataAdapter", typeof(PrimaryBusVehicleDataAdapter_HEV)},
		{"AuxDataAdapter", typeof(PrimaryBusAuxiliaryDataAdapter)},
		{"ElectricMachinesDataAdapter", typeof(ElectricMachinesDataAdapter)},
		{"HybridStrategyDataAdapter", typeof(ParallelHybridStrategyParameterDataAdapter)},
		{"DriverDataAdapter", typeof(PrimaryBusDriverDataAdapter)}
	};

	private static readonly Dictionary<string, Type?> CompDataAdapter_SerialHybrid_S2 = new() {
		{"EngineDataAdapter", typeof(CombustionEngineComponentDataAdapter)},
		{"GearboxDataAdapter", typeof(GearboxDataAdapter)},
		{"AxleGearDataAdapter", typeof(AxleGearDataAdapter)},
		{"RetarderDataAdapter", typeof(RetarderDataAdapter)},
		{"AirdragDataAdapter", typeof(AirdragDataAdapter)},
		{"AngledriveDataAdapter", typeof(AngledriveDataAdapter)},
		{"VehicleDataAdapter", typeof(PrimaryBusVehicleDataAdapter_HEV)},
		{"AuxDataAdapter", typeof(PrimaryBusAuxiliaryDataAdapter)},
		{"ElectricMachinesDataAdapter", typeof(ElectricMachinesDataAdapter)},
		{"HybridStrategyDataAdapter", typeof(SerialHybridStrategyParameterDataAdapter)},
		{"DriverDataAdapter", typeof(PrimaryBusDriverDataAdapter)}
	};

	private static readonly Dictionary<string, Type?> CompDataAdapter_SerialHybrid_S_IEPC = new() {
		{"EngineDataAdapter", typeof(CombustionEngineComponentDataAdapter)},
		{"GearboxDataAdapter", typeof(IEPCGearboxDataAdapter)},
		{"AxleGearDataAdapter", typeof(AxleGearDataAdapter)},
		{"RetarderDataAdapter", typeof(RetarderDataAdapter)},
		{"AirdragDataAdapter", typeof(AirdragDataAdapter)},
		{"AngledriveDataAdapter", typeof(AngledriveDataAdapter)},
		{"VehicleDataAdapter", typeof(PrimaryBusVehicleDataAdapter_HEV)},
		{"AuxDataAdapter", typeof(PrimaryBusAuxiliaryDataAdapter)},
		{"ElectricMachinesDataAdapter", typeof(ElectricMachinesDataAdapter)},
		{"HybridStrategyDataAdapter", typeof(SerialHybridStrategyParameterDataAdapter)},
		{"DriverDataAdapter", typeof(PrimaryBusDriverDataAdapter)}
	};

	private static readonly Dictionary<string, Type?> CompDataAdapter_SerialHybrid_Sx = new() {
		{"EngineDataAdapter", typeof(CombustionEngineComponentDataAdapter)},
		{"GearboxDataAdapter", null},
		{"AxleGearDataAdapter", typeof(AxleGearDataAdapter)},
		{"RetarderDataAdapter", typeof(RetarderDataAdapter)},
		{"AirdragDataAdapter", typeof(AirdragDataAdapter)},
		{"AngledriveDataAdapter", typeof(AngledriveDataAdapter)},
		{"VehicleDataAdapter", typeof(PrimaryBusVehicleDataAdapter_HEV)},
		{"AuxDataAdapter", typeof(PrimaryBusAuxiliaryDataAdapter)},
		{"ElectricMachinesDataAdapter", typeof(ElectricMachinesDataAdapter)},
		{"HybridStrategyDataAdapter", typeof(SerialHybridStrategyParameterDataAdapter)},
		{"DriverDataAdapter", typeof(PrimaryBusDriverDataAdapter)}
	};

	private static readonly Dictionary<string, Type?> CompDataAdapter_PEV_E2 = new() {
		{"EngineDataAdapter", null},
		{"GearboxDataAdapter", typeof(GearboxDataAdapter)},
		{"AxleGearDataAdapter", typeof(AxleGearDataAdapter)},
		{"RetarderDataAdapter", typeof(RetarderDataAdapter)},
		{"AirdragDataAdapter", typeof(AirdragDataAdapter)},
		{"AngledriveDataAdapter", typeof(AngledriveDataAdapter)},
		{"VehicleDataAdapter", typeof(PrimaryBusVehicleDataAdapter_PEV)},
		{"AuxDataAdapter", typeof(PrimaryBusAuxiliaryDataAdapter)},
		{"ElectricMachinesDataAdapter", typeof(ElectricMachinesDataAdapter)},
		{"HybridStrategyDataAdapter", null},
		{"DriverDataAdapter", typeof(PrimaryBusDriverDataAdapter)}
	};


	private static readonly Dictionary<string, Type?> CompDataAdapter_PEV_E_IEPC = new() {
		{"EngineDataAdapter", null},
		{"GearboxDataAdapter", typeof(IEPCGearboxDataAdapter)},
		{"AxleGearDataAdapter", typeof(AxleGearDataAdapter)},
		{"RetarderDataAdapter", typeof(RetarderDataAdapter)},
		{"AirdragDataAdapter", typeof(AirdragDataAdapter)},
		{"AngledriveDataAdapter", typeof(AngledriveDataAdapter)},
		{"VehicleDataAdapter", typeof(PrimaryBusVehicleDataAdapter_PEV)},
		{"AuxDataAdapter", typeof(PrimaryBusAuxiliaryDataAdapter)},
		{"ElectricMachinesDataAdapter", typeof(ElectricMachinesDataAdapter)},
		{"HybridStrategyDataAdapter", null},
		{"DriverDataAdapter", typeof(PrimaryBusDriverDataAdapter)}
	};


	private static readonly Dictionary<string, Type?> CompDataAdapter_PEV_Ex = new() {
		{"EngineDataAdapter", null},
		{"GearboxDataAdapter", null},
		{"AxleGearDataAdapter", typeof(AxleGearDataAdapter)},
		{"RetarderDataAdapter", typeof(RetarderDataAdapter)},
		{"AirdragDataAdapter", typeof(AirdragDataAdapter)},
		{"AngledriveDataAdapter", typeof(AngledriveDataAdapter)},
		{"VehicleDataAdapter", typeof(PrimaryBusVehicleDataAdapter_PEV)},
		{"AuxDataAdapter", typeof(PrimaryBusAuxiliaryDataAdapter)},
		{"ElectricMachinesDataAdapter", typeof(ElectricMachinesDataAdapter)},
		{"HybridStrategyDataAdapter", null},
		{"DriverDataAdapter", typeof(PrimaryBusDriverDataAdapter)}
	};

	private static readonly Dictionary<string, Type?> CompDataAdapter_Exempted = new() {
		{"EngineDataAdapter", null},
		{"GearboxDataAdapter", null},
		{"AxleGearDataAdapter", null},
		{"RetarderDataAdapter", null},
		{"AirdragDataAdapter", null},
		{"AngledriveDataAdapter", null},
		{"VehicleDataAdapter", typeof(ExemptedPrimaryBusVehicleDataAdapter)},
		{"AuxDataAdapter", null},
		{"ElectricMachinesDataAdapter", null},
		{"HybridStrategyDataAdapter", null},
		{"DriverDataAdapter", typeof(PrimaryBusDriverDataAdapter)}
	};

    private static readonly Dictionary<Type, Dictionary<string, Type?>> ComponentDataAdapterMapping = new() {
            { typeof(DeclarationDataAdapterPrimaryBus.Conventional), CompDataAdapter_Conventional },
            { typeof(DeclarationDataAdapterPrimaryBus.HEV_P1), CompDataAdapter_ParallelHybrid },
            { typeof(DeclarationDataAdapterPrimaryBus.HEV_P2), CompDataAdapter_ParallelHybrid },
            { typeof(DeclarationDataAdapterPrimaryBus.HEV_P2_5), CompDataAdapter_ParallelHybrid },
            { typeof(DeclarationDataAdapterPrimaryBus.HEV_P3), CompDataAdapter_ParallelHybrid },
            { typeof(DeclarationDataAdapterPrimaryBus.HEV_P4), CompDataAdapter_ParallelHybrid },
            { typeof(DeclarationDataAdapterPrimaryBus.HEV_S2), CompDataAdapter_SerialHybrid_S2 },
            { typeof(DeclarationDataAdapterPrimaryBus.HEV_S3), CompDataAdapter_SerialHybrid_Sx },
            { typeof(DeclarationDataAdapterPrimaryBus.HEV_S4), CompDataAdapter_SerialHybrid_Sx },
            { typeof(DeclarationDataAdapterPrimaryBus.HEV_S_IEPC), CompDataAdapter_SerialHybrid_S_IEPC },
            { typeof(DeclarationDataAdapterPrimaryBus.PEV_E2), CompDataAdapter_PEV_E2},
            { typeof(DeclarationDataAdapterPrimaryBus.PEV_E3), CompDataAdapter_PEV_Ex },
            { typeof(DeclarationDataAdapterPrimaryBus.PEV_E4), CompDataAdapter_PEV_Ex },
            { typeof(DeclarationDataAdapterPrimaryBus.PEV_E_IEPC), CompDataAdapter_PEV_E_IEPC },
            { typeof(DeclarationDataAdapterPrimaryBus.Exempted), CompDataAdapter_Exempted },
           
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
                    Assert.IsNotNull(componentDao, component);
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