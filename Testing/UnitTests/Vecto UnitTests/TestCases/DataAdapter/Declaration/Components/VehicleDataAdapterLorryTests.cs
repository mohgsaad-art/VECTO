using Moq;
using Ninject;
using NUnit.Framework;
using TUGraz.Vecto.UnitTests.Utils.DataAdapter;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore;
using TUGraz.VectoCore.InputData;
using TUGraz.VectoCore.InputData.Reader.DataObjectAdapter;
using TUGraz.VectoCore.InputData.Reader.DataObjectAdapter.SimulationComponents;
using TUGraz.VectoCore.InputData.Reader.Impl.DeclarationMode.HeavyLorryRunDataFactory;
using TUGraz.VectoCore.Models.Declaration;
using Assert = NUnit.Framework.Assert;
using CollectionAssert = NUnit.Framework.CollectionAssert;

namespace TUGraz.Vecto.UnitTests.TestCases.DataAdapter.Declaration.Components;

public class VehicleDataAdapterLorryTests
{
    private StandardKernel _kernel;

    [OneTimeSetUp]
    public void RunBeforeAnyTests()
    {
        _kernel = new StandardKernel(new VectoNinjectModule());

        _kernel.Rebind<ILorryDeclarationDataAdapter>().To<VehicleOnlyDeclarationDataAdapterHeavyLorryConventional>()
         .WhenInjectedExactlyInto<DeclarationModeHeavyLorryRunDataFactory.Conventional>();
    }

    [TestCase,
    Category("LongRunning"),
    Ignore("LongRunning")]
    public void Declaration_WheelsForT1_Class2()
    {
        //var dataProvider =
        //	JSONInputDataFactory.ReadJsonJob(@"TestData/Jobs/12t Delivery Truck.vecto") as IDeclarationInputDataProvider;
        //var dataReader = new DeclarationModeHeavyLorryRunDataFactory.Conventional(dataProvider, null, new DeclarationDataAdapterHeavyLorry.Conventional(), _kernel.Get<IDeclarationCycleFactory>(), _kernel.Get<IMissionFilter>());

        var dataProvider = GetMockJobInputData(VehicleClass.Class2);
        var runDataFactoryFactory = _kernel.Get<IVectoRunDataFactoryFactory>();
        var runDataFactory = runDataFactoryFactory.CreateDeclarationRunDataFactory(dataProvider, null, null);

        var runs = runDataFactory.NextRun().ToList();
        Assert.AreEqual(6, runs.Count);
        var withT1 = new[] { 6.0, 6.0, 4.5, 4.5 };

        CollectionAssert.AreEqual(withT1, runs[0].VehicleData.AxleData.Select(a => a.Inertia.Value()));
        CollectionAssert.AreEqual(withT1, runs[1].VehicleData.AxleData.Select(a => a.Inertia.Value()));

        var bodyOnly = new[] { 6.0, 6.0 };

        CollectionAssert.AreEqual(bodyOnly, runs[2].VehicleData.AxleData.Select(a => a.Inertia.Value()));
        CollectionAssert.AreEqual(bodyOnly, runs[3].VehicleData.AxleData.Select(a => a.Inertia.Value()));

        CollectionAssert.AreEqual(bodyOnly, runs[4].VehicleData.AxleData.Select(a => a.Inertia.Value()));
        CollectionAssert.AreEqual(bodyOnly, runs[5].VehicleData.AxleData.Select(a => a.Inertia.Value()));
    }

    [TestCase,
    Category("LongRunning"),
    Ignore("LongRunning")]
    public void Declaration_WheelsForT2_Class4()
    {
        //var dataProvider =
        //    JSONInputDataFactory.ReadJsonJob(
        //        @"TestData/Jobs/Class4_40t_Long_Haul_Truck.vecto") as IDeclarationInputDataProvider;
        //var dataReader = new DeclarationModeHeavyLorryRunDataFactory.Conventional(dataProvider, null, new DeclarationDataAdapterHeavyLorry.Conventional(), _kernel.Get<IDeclarationCycleFactory>(), _kernel.Get<IMissionFilter>());

        var dataProvider = GetMockJobInputData(VehicleClass.Class4);
        var runDataFactoryFactory = _kernel.Get<IVectoRunDataFactoryFactory>();
        var runDataFactory = runDataFactoryFactory.CreateDeclarationRunDataFactory(dataProvider, null, null);

        var runs = runDataFactory.NextRun().ToList();
        Assert.AreEqual(8, runs.Count);
        var withT1 = new[] { 14.9, 14.9, 19.2, 19.2 };
        CollectionAssert.AreEqual(withT1, runs[0].VehicleData.AxleData.Select(a => a.Inertia.Value()));
        CollectionAssert.AreEqual(withT1, runs[1].VehicleData.AxleData.Select(a => a.Inertia.Value()));

        var bodyOnly = new[] { 14.9, 14.9 };
        CollectionAssert.AreEqual(bodyOnly, runs[2].VehicleData.AxleData.Select(a => a.Inertia.Value()));
        CollectionAssert.AreEqual(bodyOnly, runs[3].VehicleData.AxleData.Select(a => a.Inertia.Value()));

        CollectionAssert.AreEqual(bodyOnly, runs[4].VehicleData.AxleData.Select(a => a.Inertia.Value()));
        CollectionAssert.AreEqual(bodyOnly, runs[5].VehicleData.AxleData.Select(a => a.Inertia.Value()));

        CollectionAssert.AreEqual(bodyOnly, runs[6].VehicleData.AxleData.Select(a => a.Inertia.Value()));
        CollectionAssert.AreEqual(bodyOnly, runs[7].VehicleData.AxleData.Select(a => a.Inertia.Value()));
    }

    [TestCase,
    Category("LongRunning"),
    Ignore("LongRunning")]
    public void Declaration_WheelsForDefault_Class5()
    {
        //var dataProvider =
        //    JSONInputDataFactory.ReadJsonJob(@"TestData/Jobs/40t_Long_Haul_Truck.vecto") as IDeclarationInputDataProvider;
        //var dataReader = new DeclarationModeHeavyLorryRunDataFactory.Conventional(dataProvider, null, new DeclarationDataAdapterHeavyLorry.Conventional(), _kernel.Get<IDeclarationCycleFactory>(), _kernel.Get<IMissionFilter>());

        var dataProvider = GetMockJobInputData(VehicleClass.Class5);
        var runDataFactoryFactory = _kernel.Get<IVectoRunDataFactoryFactory>();
        var runDataFactory = runDataFactoryFactory.CreateDeclarationRunDataFactory(dataProvider, null, null);

        var runs = runDataFactory.NextRun().ToList();

        Assert.AreEqual(VehicleClass.Class5, runs[0].VehicleData.VehicleClass);
        Assert.AreEqual(10, runs.Count);

        //var bodyOnly = new[] { 14.9, 14.9 };
        var withST1 = new[] { 14.9, 14.9, 19.2, 19.2, 19.2 };
        var withSTT1 = new[] { 14.9, 14.9, 19.2, 19.2, 19.2 };
        var withST1andT2 = new[] { 14.9, 14.9, 19.2, 19.2, 19.2, 19.2, 19.2 };

        CollectionAssert.AreEqual(withST1, runs[0].VehicleData.AxleData.Select(a => a.Inertia.Value()));
        CollectionAssert.AreEqual(withST1, runs[1].VehicleData.AxleData.Select(a => a.Inertia.Value()));

        CollectionAssert.AreEqual(withST1andT2, runs[2].VehicleData.AxleData.Select(a => a.Inertia.Value()));
        CollectionAssert.AreEqual(withST1andT2, runs[3].VehicleData.AxleData.Select(a => a.Inertia.Value()));

        CollectionAssert.AreEqual(withST1, runs[4].VehicleData.AxleData.Select(a => a.Inertia.Value()));
        CollectionAssert.AreEqual(withST1, runs[5].VehicleData.AxleData.Select(a => a.Inertia.Value()));

        CollectionAssert.AreEqual(withST1andT2, runs[6].VehicleData.AxleData.Select(a => a.Inertia.Value()));
        CollectionAssert.AreEqual(withST1andT2, runs[7].VehicleData.AxleData.Select(a => a.Inertia.Value()));

        CollectionAssert.AreEqual(withSTT1, runs[8].VehicleData.AxleData.Select(a => a.Inertia.Value()));
        CollectionAssert.AreEqual(withSTT1, runs[9].VehicleData.AxleData.Select(a => a.Inertia.Value()));
    }

    [TestCase()]
    public void TestMaxMassInMunicipalCycle()
    {
        var doa = new LorryVehicleDataAdapter();

        var segment = DeclarationData.TruckSegments.Lookup(VehicleCategory.RigidTruck, AxleConfiguration.AxleConfig_4x2,
            18000.SI<Kilogram>(), 9300.SI<Kilogram>(), true);
        var mission = segment.Missions.First(m => m.MissionType == MissionType.MunicipalUtility);
        var loading = mission.Loadings.First(l => l.Key.Equals(LoadingType.ReferenceLoad));

        var vehicleInputData = GetMockVehicleInputData();

        var muRefLoadData = doa.CreateVehicleData(vehicleInputData, segment, mission, loading, true);

        Assert.AreEqual(2700, muRefLoadData.Loading.Value());
        Assert.AreEqual(6000, muRefLoadData.BodyAndTrailerMass.Value());
        Assert.AreEqual(18000, muRefLoadData.TotalVehicleMass.Value());

    }

    private IVehicleDeclarationInputData GetMockVehicleInputData()
    {
        var veh = new Mock<IVehicleDeclarationInputData>();
        var comp = new Mock<IVehicleComponentsDeclaration>();
        var eng = new Mock<IEngineDeclarationInputData>();
        var gbx = new Mock<IGearboxDeclarationInputData>();
        var axlWhls = new Mock<IAxlesDeclarationInputData>();
        var axlWhl1 = new Mock<IAxleDeclarationInputData>();
        var axlWhl2 = new Mock<IAxleDeclarationInputData>();
        var tyre = new Mock<ITyreDeclarationInputData>();
        var adas = new Mock<IAdvancedDriverAssistantSystemDeclarationInputData>();

        veh.Setup(v => v.ADAS).Returns(adas.Object);
        veh.Setup(v => v.Components).Returns(comp.Object);
        comp.Setup(c => c.EngineInputData).Returns(eng.Object);
        comp.Setup(c => c.AxleWheels).Returns(axlWhls.Object);
        comp.Setup(c => c.GearboxInputData).Returns(gbx.Object);

        veh.Setup(v => v.ExemptedVehicle).Returns(false);
        veh.Setup(v => v.ArchitectureID).Returns(ArchitectureID.UNKNOWN);
        veh.Setup(v => v.VehicleType).Returns(VectoSimulationJobType.ConventionalVehicle);
        veh.Setup(v => v.LegislativeClass).Returns(LegislativeClass.M3);
        veh.Setup(v => v.OVC).Returns(false);

        adas.Setup(a => a.EngineStopStart).Returns(false);
        adas.Setup(a => a.PredictiveCruiseControl).Returns(PredictiveCruiseControlType.None);
        adas.Setup(a => a.EcoRoll).Returns(EcoRollType.None);

        axlWhls.Setup(a => a.AxlesDeclaration).Returns(new List<IAxleDeclarationInputData> { axlWhl1.Object, axlWhl2.Object });
        axlWhl1.Setup(a => a.AxleType).Returns(AxleType.VehicleNonDriven);
        axlWhl1.Setup(a => a.Steered).Returns(true);
        axlWhl1.Setup(a => a.TwinTyres).Returns(false);
        axlWhl1.Setup(a => a.Tyre).Returns(tyre.Object);

        axlWhl2.Setup(a => a.AxleType).Returns(AxleType.VehicleDriven);
        axlWhl2.Setup(a => a.Steered).Returns(false);
        axlWhl2.Setup(a => a.TwinTyres).Returns(true);
        axlWhl2.Setup(a => a.Tyre).Returns(tyre.Object);


        veh.Setup(v => v.VehicleCategory).Returns(VehicleCategory.RigidTruck);
        veh.Setup(v => v.AxleConfiguration).Returns(AxleConfiguration.AxleConfig_4x2);
        veh.Setup(v => v.CurbMassChassis).Returns(9300.SI<Kilogram>());
        veh.Setup(v => v.GrossVehicleMassRating).Returns(18000.SI<Kilogram>());
        tyre.Setup(t => t.Dimension).Returns("315/70 R22.5");

        return veh.Object;
    }

    private IInputDataProvider GetMockJobInputData(VehicleClass vehicleClass)
    {
        var mock = new Mock<IDeclarationInputDataProvider>();
        var job = new Mock<IDeclarationJobInputData>();
        var veh = new Mock<IVehicleDeclarationInputData>();
        var comp = new Mock<IVehicleComponentsDeclaration>();
        var eng = new Mock<IEngineDeclarationInputData>();
        var gbx = new Mock<IGearboxDeclarationInputData>();
        var axlWhls = new Mock<IAxlesDeclarationInputData>();
        var axlWhl1 = new Mock<IAxleDeclarationInputData>();
        var axlWhl2 = new Mock<IAxleDeclarationInputData>();
        var tyre = new Mock<ITyreDeclarationInputData>();
        var adas = new Mock<IAdvancedDriverAssistantSystemDeclarationInputData>();

        mock.Setup(i => i.JobInputData).Returns(job.Object);
        job.Setup(j => j.Vehicle).Returns(veh.Object);
        veh.Setup(v => v.ADAS).Returns(adas.Object);
        veh.Setup(v => v.Components).Returns(comp.Object);
        comp.Setup(c => c.EngineInputData).Returns(eng.Object);
        comp.Setup(c => c.AxleWheels).Returns(axlWhls.Object);
        comp.Setup(c => c.GearboxInputData).Returns(gbx.Object);

        veh.Setup(v => v.ExemptedVehicle).Returns(false);
        veh.Setup(v => v.ArchitectureID).Returns(ArchitectureID.UNKNOWN);
        veh.Setup(v => v.VehicleType).Returns(VectoSimulationJobType.ConventionalVehicle);
        veh.Setup(v => v.LegislativeClass).Returns(LegislativeClass.M3);
        veh.Setup(v => v.OVC).Returns(false);

        adas.Setup(a => a.EngineStopStart).Returns(false);
        adas.Setup(a => a.PredictiveCruiseControl).Returns(PredictiveCruiseControlType.None);
        adas.Setup(a => a.EcoRoll).Returns(EcoRollType.None);

        gbx.Setup(g => g.Type).Returns(GearboxType.AMT);
        gbx.Setup(g => g.Gears).Returns(new List<ITransmissionInputData>());

        var mode = new Mock<IEngineModeDeclarationInputData>();
        var fuel = new Mock<IEngineFuelDeclarationInputData>();
        fuel.Setup(f => f.FuelType).Returns(FuelType.DieselCI);
        mode.Setup(m => m.Fuels).Returns(new List<IEngineFuelDeclarationInputData>() { fuel.Object });
        eng.Setup(e => e.EngineModes).Returns(new List<IEngineModeDeclarationInputData>() {
            mode.Object
        });
        eng.Setup(e => e.RatedPowerDeclared).Returns(320000.SI<Watt>());

        axlWhls.Setup(a => a.AxlesDeclaration).Returns(new List<IAxleDeclarationInputData> { axlWhl1.Object, axlWhl2.Object });
        axlWhl1.Setup(a => a.AxleType).Returns(AxleType.VehicleNonDriven);
        axlWhl1.Setup(a => a.Steered).Returns(true);
        axlWhl1.Setup(a => a.TwinTyres).Returns(false);
        axlWhl1.Setup(a => a.Tyre).Returns(tyre.Object);

        axlWhl2.Setup(a => a.AxleType).Returns(AxleType.VehicleDriven);
        axlWhl2.Setup(a => a.Steered).Returns(false);
        axlWhl2.Setup(a => a.TwinTyres).Returns(true);
        axlWhl2.Setup(a => a.Tyre).Returns(tyre.Object);

        switch (vehicleClass)
        {
            case VehicleClass.Class2:
                veh.Setup(v => v.VehicleCategory).Returns(VehicleCategory.RigidTruck);
                veh.Setup(v => v.AxleConfiguration).Returns(AxleConfiguration.AxleConfig_4x2);
                veh.Setup(v => v.CurbMassChassis).Returns(5850.SI<Kilogram>());
                veh.Setup(v => v.GrossVehicleMassRating).Returns(11900.SI<Kilogram>());
                tyre.Setup(t => t.Dimension).Returns("245/70 R19.5");
                break;
            case VehicleClass.Class4:
                veh.Setup(v => v.VehicleCategory).Returns(VehicleCategory.RigidTruck);
                veh.Setup(v => v.AxleConfiguration).Returns(AxleConfiguration.AxleConfig_4x2);
                veh.Setup(v => v.CurbMassChassis).Returns(7100.SI<Kilogram>());
                veh.Setup(v => v.GrossVehicleMassRating).Returns(40000.SI<Kilogram>());
                tyre.Setup(t => t.Dimension).Returns("315/70 R22.5");

                break;
            case VehicleClass.Class5:
                veh.Setup(v => v.VehicleCategory).Returns(VehicleCategory.Tractor);
                veh.Setup(v => v.AxleConfiguration).Returns(AxleConfiguration.AxleConfig_4x2);
                veh.Setup(v => v.CurbMassChassis).Returns(7100.SI<Kilogram>());
                veh.Setup(v => v.GrossVehicleMassRating).Returns(40000.SI<Kilogram>());
                tyre.Setup(t => t.Dimension).Returns("315/70 R22.5");

                break;
        }

        return mock.Object;
    }
}