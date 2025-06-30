using NUnit.Framework;
using System.Reflection;
using TUGraz.VectoCore.InputData.Reader.DataObjectAdapter.CompletedBus.Specific;
using TUGraz.VectoCore.InputData.Reader.DataObjectAdapter.SimulationComponents;
using TUGraz.VectoCore.InputData.Reader.DataObjectAdapter.SimulationComponents.AuxiliaryDataAdapter;
using TUGraz.VectoCore.InputData.Reader.DataObjectAdapter.SimulationComponents.StrategyDataAdapter;

namespace TUGraz.Vecto.UnitTests.TestCases.DataAdapter.Declaration;

public class CompletedBusSpecificDataAdapterTests
{
    private static readonly Dictionary<string, Type?> CompDataAdapter_Conventional = new() {
        {"AirdragDataAdapter", typeof(CompletedBusSpecificAirdragDataAdapter)},
        {"VehicleDataAdapter", typeof(CompletedBusSpecificVehicleDataAdapter)},
        {"AuxDataAdapter", typeof(SpecificCompletedBusAuxiliaryDataAdapter)},
    };

    private static readonly Dictionary<string, Type?> CompDataAdapter_ParallelHybrid = new() {
        {"AirdragDataAdapter", typeof(CompletedBusSpecificAirdragDataAdapter)},
        {"VehicleDataAdapter", typeof(CompletedBusSpecificVehicleDataAdapter)},
        {"AuxDataAdapter", typeof(SpecificCompletedBusAuxiliaryDataAdapter)},
    };

    private static readonly Dictionary<string, Type?> CompDataAdapter_SerialHybrid_S2 = new() {
        {"AirdragDataAdapter", typeof(CompletedBusSpecificAirdragDataAdapter)},
        {"VehicleDataAdapter", typeof(CompletedBusSpecificVehicleDataAdapter)},
        {"AuxDataAdapter", typeof(SpecificCompletedBusAuxiliaryDataAdapter)},
    };

    private static readonly Dictionary<string, Type?> CompDataAdapter_SerialHybrid_S_IEPC = new() {
        {"AirdragDataAdapter", typeof(CompletedBusSpecificAirdragDataAdapter)},
        {"VehicleDataAdapter", typeof(CompletedBusSpecificVehicleDataAdapter)},
        {"AuxDataAdapter", typeof(SpecificCompletedBusAuxiliaryDataAdapter)},
    };

    private static readonly Dictionary<string, Type?> CompDataAdapter_SerialHybrid_Sx = new() {
        {"AirdragDataAdapter", typeof(CompletedBusSpecificAirdragDataAdapter)},
        {"VehicleDataAdapter", typeof(CompletedBusSpecificVehicleDataAdapter)},
        {"AuxDataAdapter", typeof(SpecificCompletedBusAuxiliaryDataAdapter)},
    };

	private static readonly Dictionary<string, Type?> CompDataAdapter_PEV_E2 = new() {
        {"AirdragDataAdapter", typeof(CompletedBusSpecificAirdragDataAdapter)},
        {"VehicleDataAdapter", typeof(CompletedBusSpecificVehicleDataAdapter)},
        {"AuxDataAdapter", typeof(SpecificCompletedPEVBusAuxiliaryDataAdapter)},
    };


    private static readonly Dictionary<string, Type?> CompDataAdapter_PEV_E_IEPC = new() {
        {"AirdragDataAdapter", typeof(CompletedBusSpecificAirdragDataAdapter)},
        {"VehicleDataAdapter", typeof(CompletedBusSpecificVehicleDataAdapter)},
        {"AuxDataAdapter", typeof(SpecificCompletedPEVBusAuxiliaryDataAdapter)},
    };


    private static readonly Dictionary<string, Type?> CompDataAdapter_PEV_Ex = new() {
        {"AirdragDataAdapter", typeof(CompletedBusSpecificAirdragDataAdapter)},
        {"VehicleDataAdapter", typeof(CompletedBusSpecificVehicleDataAdapter)},
        {"AuxDataAdapter", typeof(SpecificCompletedPEVBusAuxiliaryDataAdapter)},
    };

    private static readonly Dictionary<string, Type?> CompDataAdapter_Exempted = new() {
        {"AirdragDataAdapter", null},
        {"VehicleDataAdapter", typeof(ExemptedCompletedBusSpecificVehicleDataAdapter)},
        {"AuxDataAdapter", null},
    };

    private static readonly Dictionary<Type, Dictionary<string, Type?>> ComponentDataAdapterMapping = new() {
            { typeof(DeclarationDataAdapterSpecificCompletedBus.Conventional), CompDataAdapter_Conventional },
            { typeof(DeclarationDataAdapterSpecificCompletedBus.HEV_P1), CompDataAdapter_ParallelHybrid },
            { typeof(DeclarationDataAdapterSpecificCompletedBus.HEV_P2), CompDataAdapter_ParallelHybrid },
            { typeof(DeclarationDataAdapterSpecificCompletedBus.HEV_P2_5), CompDataAdapter_ParallelHybrid },
            { typeof(DeclarationDataAdapterSpecificCompletedBus.HEV_P3), CompDataAdapter_ParallelHybrid },
            { typeof(DeclarationDataAdapterSpecificCompletedBus.HEV_P4), CompDataAdapter_ParallelHybrid },
            { typeof(DeclarationDataAdapterSpecificCompletedBus.HEV_S2), CompDataAdapter_SerialHybrid_S2 },
            { typeof(DeclarationDataAdapterSpecificCompletedBus.HEV_S3), CompDataAdapter_SerialHybrid_Sx },
            { typeof(DeclarationDataAdapterSpecificCompletedBus.HEV_S4), CompDataAdapter_SerialHybrid_Sx },
            { typeof(DeclarationDataAdapterSpecificCompletedBus.HEV_S_IEPC), CompDataAdapter_SerialHybrid_S_IEPC },
            { typeof(DeclarationDataAdapterSpecificCompletedBus.PEV_E2), CompDataAdapter_PEV_E2},
            { typeof(DeclarationDataAdapterSpecificCompletedBus.PEV_E3), CompDataAdapter_PEV_Ex },
            { typeof(DeclarationDataAdapterSpecificCompletedBus.PEV_E4), CompDataAdapter_PEV_Ex },
            { typeof(DeclarationDataAdapterSpecificCompletedBus.PEV_E_IEPC), CompDataAdapter_PEV_E_IEPC },
            { typeof(DeclarationDataAdapterSpecificCompletedBus.Exempted), CompDataAdapter_Exempted },

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