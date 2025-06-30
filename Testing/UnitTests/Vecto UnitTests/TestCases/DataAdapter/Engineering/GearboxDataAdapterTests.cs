using Moq;
using NUnit.Framework;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.Reader.ComponentData;
using TUGraz.VectoCore.InputData.Reader.DataObjectAdapter;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Engine;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Gearbox;
using TUGraz.VectoCore.Tests.Utils;
using TUGraz.VectoCore.Utils;
using Assert = NUnit.Framework.Assert;

namespace TUGraz.Vecto.UnitTests.TestCases.DataAdapter.Engineering;

public class GearboxDataAdapterTests
{
	[TestCase]
	public void TestGearboxDataReadTest()
	{
		var input = GetMockInputData();

		var dao = new EngineeringDataAdapter();
		var axleData = dao.CreateAxleGearData(GetMockAxlegearInputdata());
		Assert.AreEqual(3.240355, axleData.AxleGear.Ratio, 0.0001);

		var gbxData = dao.CreateGearboxData(input, GetDummyRunData(input.JobInputData.Vehicle.Components.GearboxInputData));
		Assert.AreEqual(GearboxType.AMT, gbxData.Type);
		Assert.AreEqual(1.0, gbxData.TractionInterruption.Value(), 0.0001);
		Assert.AreEqual(8, gbxData.Gears.Count);

		Assert.AreEqual(1.0, gbxData.Gears[7].Ratio, 0.0001);

		Assert.AreEqual(-400, gbxData.Gears[1].ShiftPolygon.Downshift[0].Torque.Value(), 0.0001);
		Assert.AreEqual(560.RPMtoRad().Value(), gbxData.Gears[1].ShiftPolygon.Downshift[0].AngularSpeed.Value(), 0.0001);
		Assert.AreEqual(1289.RPMtoRad().Value(), gbxData.Gears[1].ShiftPolygon.Upshift[0].AngularSpeed.Value(), 0.0001);

		Assert.AreEqual(200.RPMtoRad().Value(), gbxData.Gears[1].LossMap[26].InputSpeed.Value(), 0.0001);
		Assert.AreEqual(-350, gbxData.Gears[1].LossMap[35].InputTorque.Value(), 0.0001);
		Assert.AreEqual(13.072, gbxData.Gears[1].LossMap[35].TorqueLoss.Value(), 0.0001);
	}

    [TestCase]
    public void ReadGearboxSerialTC()
    {
       // var inputProvider = JSONInputDataFactory.ReadGearbox(@"TestData/Components/AT_GBX/GearboxSerial.vgbx");

		var inputProvider = GetMockGbxInputData(GearboxType.ATSerial, VehicleCategory.Tractor, 1);
        var tcInputProvider = GetMockTcInputData(GearboxType.ATSerial);

        var ratios = new[] { 3.4, 1.9, 1.42, 1.0, 0.7, 0.62 };
        Assert.AreEqual(ratios.Length, inputProvider.Gears.Count);
        for (int i = 0; i < ratios.Length; i++) {
            Assert.AreEqual(ratios[i], inputProvider.Gears[i].Ratio);
        }

		var inputData = GetMockInputData(inputProvider, tcInputProvider);
		var runData = GetDummyVectoRunData(VehicleCategory.Tractor);
        var gbxData = new EngineeringDataAdapter().CreateGearboxData(
            inputData,
			runData);

        //inputProvider,
        //MockSimulationDataFactory.CreateEngineDataFromFile(@"TestData/Components/AT_GBX/Engine.veng", 0),
        //(IGearshiftEngineeringInputData)inputProvider, 2.1,
        //0.5.SI<Meter>(), VehicleCategory.RigidTruck, (ITorqueConverterEngineeringInputData)inputProvider, null, null);
        Assert.AreEqual(ratios.Length, gbxData.Gears.Count);

        Assert.IsTrue(gbxData.Gears[1].HasLockedGear);
        Assert.IsTrue(gbxData.Gears[1].HasTorqueConverter);
        Assert.IsTrue(gbxData.Gears[2].HasLockedGear);
        Assert.IsFalse(gbxData.Gears[2].HasTorqueConverter);
        Assert.IsTrue(gbxData.Gears[3].HasLockedGear);
        Assert.IsFalse(gbxData.Gears[3].HasTorqueConverter);

        var gear = gbxData.Gears[1];
        Assert.AreEqual(gear.Ratio, gear.TorqueConverterRatio);
    }

	

	[TestCase]
    public void ReadGearboxPowersplitTC()
    {
        // var inputProvider = JSONInputDataFactory.ReadGearbox(@"TestData/Components/AT_GBX/GearboxPowerSplit.vgbx");
		var inputProvider = GetMockGbxInputData(GearboxType.ATPowerSplit, VehicleCategory.Tractor, 1);
		var tcInputProvider = GetMockTcInputData(GearboxType.ATSerial);

        var ratios = new[] { 1.35, 1.0, 0.73 };
        Assert.AreEqual(ratios.Length, inputProvider.Gears.Count);
        for (int i = 0; i < ratios.Length; i++) {
            Assert.AreEqual(ratios[i], inputProvider.Gears[i].Ratio);
        }

		var inputData = GetMockInputData(inputProvider, tcInputProvider);
		var runData = GetDummyVectoRunData(VehicleCategory.Tractor);
        var gbxData = new EngineeringDataAdapter().CreateGearboxData(
            inputData, runData);

        //inputProvider,
        //MockSimulationDataFactory.CreateEngineDataFromFile(@"TestData/Components/AT_GBX/Engine.veng", 0),
        //(IGearshiftEngineeringInputData)inputProvider, 2.1,
        //0.5.SI<Meter>(), VehicleCategory.RigidTruck, (ITorqueConverterEngineeringInputData)inputProvider, null, null);
        Assert.AreEqual(ratios.Length, gbxData.Gears.Count);

        Assert.IsTrue(gbxData.Gears[1].HasLockedGear);
        Assert.IsTrue(gbxData.Gears[1].HasTorqueConverter);
        Assert.IsTrue(gbxData.Gears[2].HasLockedGear);
        Assert.IsFalse(gbxData.Gears[2].HasTorqueConverter);
        Assert.IsTrue(gbxData.Gears[3].HasLockedGear);
        Assert.IsFalse(gbxData.Gears[3].HasTorqueConverter);

        Assert.AreEqual(1, gbxData.Gears[1].TorqueConverterRatio);
    }

	

	[TestCase]
    public void ReadGearboxDualTCTruck()
    {
        // var inputProvider = JSONInputDataFactory.ReadGearbox(@"TestData/Components/AT_GBX/GearboxSerialDualTC.vgbx");
		var inputProvider = GetMockGbxInputData(GearboxType.ATSerial, VehicleCategory.Tractor, 2);
		var tcInputProvider = GetMockTcInputData(GearboxType.ATSerial);

        var ratios = new[] { 4.35, 2.4, 1.8, 1.3, 1.0 };
        Assert.AreEqual(ratios.Length, inputProvider.Gears.Count);
        for (int i = 0; i < ratios.Length; i++) {
            Assert.AreEqual(ratios[i], inputProvider.Gears[i].Ratio);
        }

		var inputData = GetMockInputData(inputProvider, tcInputProvider);
		var runData = GetDummyVectoRunData(VehicleCategory.Tractor);

        var gbxData = new EngineeringDataAdapter().CreateGearboxData(
           inputData, runData);
        //inputProvider,
        //MockSimulationDataFactory.CreateEngineDataFromFile(@"TestData/Components/AT_GBX/Engine.veng", 0),
        //(IGearshiftEngineeringInputData)inputProvider, 2.1,
        //0.5.SI<Meter>(), VehicleCategory.RigidTruck, (ITorqueConverterEngineeringInputData)inputProvider, null, null);
        Assert.AreEqual(ratios.Length, gbxData.Gears.Count);

        Assert.IsFalse(gbxData.Gears[1].HasLockedGear);
        Assert.IsTrue(gbxData.Gears[1].HasTorqueConverter);
        Assert.IsTrue(gbxData.Gears[2].HasLockedGear);
        Assert.IsTrue(gbxData.Gears[2].HasTorqueConverter);
        Assert.IsTrue(gbxData.Gears[3].HasLockedGear);
        Assert.IsFalse(gbxData.Gears[3].HasTorqueConverter);


        var gear = gbxData.Gears[2];
        Assert.AreEqual(gear.Ratio, gear.TorqueConverterRatio);
    }

    [TestCase]
    public void ReadGearboxSingleTCBus()
    {
        //var inputProvider = JSONInputDataFactory.ReadGearbox(@"TestData/Components/AT_GBX/GearboxSerialDualTC.vgbx");
		var inputProvider = GetMockGbxInputData(GearboxType.ATSerial, VehicleCategory.HeavyBusPrimaryVehicle, 1);
		var tcInputProvider = GetMockTcInputData(GearboxType.ATSerial);

        var ratios = new[] { 4.35, 2.4, 1.8, 1.3, 1.0 };
        Assert.AreEqual(ratios.Length, inputProvider.Gears.Count);
        for (int i = 0; i < ratios.Length; i++) {
            Assert.AreEqual(ratios[i], inputProvider.Gears[i].Ratio);
        }

		var inputData = GetMockInputData(inputProvider, tcInputProvider);
		var runData = GetDummyVectoRunData(VehicleCategory.HeavyBusPrimaryVehicle);
        var gbxData = new EngineeringDataAdapter().CreateGearboxData(
            inputData, runData);
        //inputProvider,
        //MockSimulationDataFactory.CreateEngineDataFromFile(@"TestData/Components/AT_GBX/Engine.veng", 0),
        //(IGearshiftEngineeringInputData)inputProvider, 2.1,
        //0.5.SI<Meter>(), VehicleCategory.InterurbanBus, (ITorqueConverterEngineeringInputData)inputProvider, null, null);
        Assert.AreEqual(ratios.Length, gbxData.Gears.Count);

        Assert.IsTrue(gbxData.Gears[1].HasLockedGear);
        Assert.IsTrue(gbxData.Gears[1].HasTorqueConverter);
        Assert.IsTrue(gbxData.Gears[2].HasLockedGear);
        Assert.IsFalse(gbxData.Gears[2].HasTorqueConverter);
        Assert.IsTrue(gbxData.Gears[3].HasLockedGear);
        Assert.IsFalse(gbxData.Gears[3].HasTorqueConverter);


        var gear = gbxData.Gears[1];
        Assert.AreEqual(gear.Ratio, gear.TorqueConverterRatio);
    }

    [TestCase]
    public void ReadGearboxDualTCBus()
    {
        //var inputProvider = JSONInputDataFactory.ReadGearbox(@"TestData/Components/AT_GBX/GearboxSerialDualTCBus.vgbx");
		var inputProvider = GetMockGbxInputData(GearboxType.ATSerial, VehicleCategory.HeavyBusPrimaryVehicle, 2);
		var tcInputProvider = GetMockTcInputData(GearboxType.ATSerial);

        var ratios = new[] { 4.58, 2.4, 1.8, 1.3, 1.0 };
        Assert.AreEqual(ratios.Length, inputProvider.Gears.Count);
        for (int i = 0; i < ratios.Length; i++) {
            Assert.AreEqual(ratios[i], inputProvider.Gears[i].Ratio);
        }

		var inputData = GetMockInputData(inputProvider, tcInputProvider);
		var runData = GetDummyVectoRunData(VehicleCategory.HeavyBusPrimaryVehicle);
        var gbxData = new EngineeringDataAdapter().CreateGearboxData(
            inputData, runData);

        //inputProvider,
        //MockSimulationDataFactory.CreateEngineDataFromFile(@"TestData/Components/AT_GBX/Engine.veng", 0),
        //(IGearshiftEngineeringInputData)inputProvider, 2.1,
        //0.5.SI<Meter>(), VehicleCategory.InterurbanBus, (ITorqueConverterEngineeringInputData)inputProvider, null, null);
        Assert.AreEqual(ratios.Length, gbxData.Gears.Count);

        Assert.IsFalse(gbxData.Gears[1].HasLockedGear);
        Assert.IsTrue(gbxData.Gears[1].HasTorqueConverter);
        Assert.IsTrue(gbxData.Gears[2].HasLockedGear);
        Assert.IsTrue(gbxData.Gears[2].HasTorqueConverter);
        Assert.IsTrue(gbxData.Gears[3].HasLockedGear);
        Assert.IsFalse(gbxData.Gears[3].HasTorqueConverter);


        var gear = gbxData.Gears[2];
        Assert.AreEqual(gear.Ratio, gear.TorqueConverterRatio);
    }


	private IGearboxEngineeringInputData GetMockGbxInputData(GearboxType gbxType, VehicleCategory vehicleCategory, int numTcGears)
	{
		var gbx = new Mock<IGearboxEngineeringInputData>();
		double[] ratios = {};
		switch (gbxType) {
            case GearboxType.ATSerial:
				
				if (vehicleCategory.IsBus()) {
					ratios = numTcGears == 1 ? new[] { 4.35, 2.4, 1.8, 1.3, 1.0 } : new[] { 4.58, 2.4, 1.8, 1.3, 1.0 };
				} else {
					ratios = numTcGears == 1 ? new[] { 3.4, 1.9, 1.42, 1.0, 0.7, 0.62 } : new[] { 4.35, 2.4, 1.8, 1.3, 1.0 };
                }
				break;
            case GearboxType.ATPowerSplit:
				ratios = new[] { 1.35, 1.0, 0.73 };
				break;
        }

		gbx.Setup(g => g.Type).Returns(gbxType);
		var gears = new List<ITransmissionInputData>();
		foreach (var ratio in ratios) {
			var gear = new Mock<ITransmissionInputData>();
			gear.Setup(g => g.Ratio).Returns(ratio);
			gear.Setup(g => g.Efficiency).Returns(0.98);
			gear.Setup(g => g.ShiftPolygon).Returns(InputDataHelper.InputDataAsTableData(ShiftHdr, ShiftData));
            gears.Add(gear.Object);
		}
		gbx.Setup(g => g.Gears).Returns(gears);
        return gbx.Object;
	}

    const string ShiftHdr = "engine torque [Nm],downshift rpm [1/min],upshift rpm [1/min]";

	private readonly string[] ShiftData = new[] {
		"-200,700,800",
		"0,700,800",
		"3000,700,800",
	};

	private ITorqueConverterEngineeringInputData GetMockTcInputData(GearboxType atSerial)
	{
		var tc = new Mock<ITorqueConverterEngineeringInputData>();
		tc.Setup(t => t.ReferenceRPM).Returns(1000.RPMtoRad());
		string[] tcData = null;
		switch (atSerial) {
            case GearboxType.ATSerial:
				tcData = TcDataSerial;
				break;
            case GearboxType.ATPowerSplit:
				tcData = TcDataPS;
                break;
		}

		tc.Setup(t => t.TCData).Returns(InputDataHelper.InputDataAsTableData(TcHdr, tcData));
		
		//tc.Setup(t => t.Inertia).Returns(0.SI<KilogramSquareMeter>());
        return tc.Object;
	}

    const string TcHdr = "Speed Ratio, Torque Ratio,MP1000";

	private static string[] TcDataSerial = new[] {
		"0.0,1.80,377.80",
		"0.1,1.71,365.21",
		"0.2,1.61,352.62",
		"0.3,1.52,340.02",
		"0.4,1.42,327.43",
		"0.5,1.33,314.84",
		"0.6,1.23,302.24",
		"0.7,1.14,264.46",
		"0.8,1.04,226.68",
		"0.9,1.02,188.90",
	};

	private static string[] TcDataPS = new[] {
		"0.0,  4.5, 700",
		"0.1,  3.5, 640",
		"0.2,  2.7, 560",
		"0.3,  2.2, 460",
		"0.4,  1.6, 350",
		"0.5,  1.2, 250",
		"0.6,  0.9, 160",
		"0.74, 0.9,   1",
	};

    private VectoRunData GetDummyVectoRunData(VehicleCategory vehCategory)
	{
		return new VectoRunData() {
			EngineData = new CombustionEngineData() {
				Displacement = 7700.SI(Unit.SI.Cubic.Centi.Meter).Cast<CubicMeter>(),
				IdleSpeed = 600.RPMtoRad(),
				Inertia = 3.8.SI<KilogramSquareMeter>(),
				FullLoadCurves = new Dictionary<uint, EngineFullLoadCurve>() {
					{ 0, FullLoadCurveReader.Create(InputDataHelper.InputDataAsTableData(EngineFldHdr, EngineFldData)) }
				}
			},
			VehicleData = new VehicleData() {
				VehicleCategory = vehCategory,
				DynamicTyreRadius = 0.5.SI<Meter>()
			},
			AxleGearData = new AxleGearData() { AxleGear = new TransmissionData() { Ratio = 2.1 } }
		};

	}

    private IEngineeringInputDataProvider GetMockInputData(IGearboxEngineeringInputData? gbxInput = null,
		ITorqueConverterEngineeringInputData? tcInput = null)
    {
        var input = new Mock<IEngineeringInputDataProvider>();
		var job = new Mock<IEngineeringJobInputData>();
		input.Setup(i => i.JobInputData).Returns(job.Object);
		var components = new Mock<IVehicleComponentsEngineering>();
		var driver = new Mock<IDriverEngineeringInputData>();
		input.Setup(i => i.DriverInputData).Returns(driver.Object);
		//var gs = new Mock<IGearshiftEngineeringInputData>();
		//driver.Setup(d => d.GearshiftInputData).Returns(gs.Object);

		var vehicle = new Mock<IVehicleEngineeringInputData>();
		job.Setup(j => j.Vehicle).Returns(vehicle.Object);
        vehicle.Setup(i => i.Components).Returns(components.Object);
        var gbx = gbxInput ?? GetMockGearboxInputdata();
        components.Setup(c => c.GearboxInputData).Returns(gbx);
        var axl = GetMockAxlegearInputdata();
        components.Setup(c => c.AxleGearInputData).Returns(axl);

		if (tcInput != null) {
			components.Setup(c => c.TorqueConverterInputData).Returns(tcInput);
			var gs = new Mock<IGearshiftEngineeringInputData>();
			driver.Setup(d => d.GearshiftInputData).Returns(gs.Object);
			gs.Setup(s => s.CLUpshiftMinAcceleration).Returns(DeclarationData.TorqueConverter.CLUpshiftMinAcceleration);
			gs.Setup(s => s.CCUpshiftMinAcceleration).Returns(DeclarationData.TorqueConverter.CCUpshiftMinAcceleration);
		}

        return input.Object;
    }

    private IAxleGearInputData GetMockAxlegearInputdata()
    {
        var lossMap = InputDataHelper.InputDataAsTableData(AxlMapHdr, AxlMapData);
        var axl = new Mock<IAxleGearInputData>();
        axl.Setup(a => a.Ratio).Returns(3.240355);
        axl.Setup(a => a.LossMap).Returns(lossMap);

        return axl.Object;
    }

    private IGearboxEngineeringInputData GetMockGearboxInputdata()
    {
        var gbx = new Mock<IGearboxEngineeringInputData>();
        
        var gearRatios = new double[] { 6.38, 4.63, 3.44, 2.59, 1.86, 1.35, 1.0, 0.76 };
        var gears = gearRatios.Select((i, idx) => {
            var g = new Mock<ITransmissionInputData>();
            g.Setup(x => x.Ratio).Returns(i);
            g.Setup(x => x.Gear).Returns(idx + 1);
            var lossMap = !i.IsEqual(1.0) ? LossMapIndirect : LossMapDirect;
            g.Setup(x => x.LossMap)
                .Returns(InputDataHelper.InputDataAsTableData(LossMapHdr, lossMap));
			g.Setup(x => x.ShiftPolygon)
				.Returns(InputDataHelper.InputDataAsTableData(ShiftPolyHdr, ShiftPolyData));
            return g.Object;
        }).ToList();

		gbx.Setup(g => g.TractionInterruption).Returns(1.SI<Second>());
        gbx.Setup(g => g.Type).Returns(GearboxType.AMT);
        gbx.Setup(g => g.Gears).Returns(gears);

        return gbx.Object;
    }

    private static VectoRunData GetDummyRunData(IGearboxDeclarationInputData gbxData)
    {
        var fld = new[] {
            "560        ,680      ,-149      ,0.6",
            "600        ,882      ,-148      ,0.6",
            "800        ,1491      ,-149      ,0.6",
            "1000       ,2100      ,-160      ,0.6",
            "1200       ,2200      ,-179      ,0.6",
            "1400       ,2300      ,-203      ,0.6",
            "1600       ,2179      ,-235      ,0.49",
            "1800       ,2057      ,-264      ,0.25",
            "2000       ,1652      ,-301      ,0.25",
            "2100       ,1500      ,-320      ,0.25",
            };
        var engineData = new CombustionEngineData() {
            IdleSpeed = 560.RPMtoRad(),
            Inertia = 0.SI<KilogramSquareMeter>(),
            EngineStartTime = 1.SI<Second>(),
        };
        var fullLoadCurves = new Dictionary<uint, EngineFullLoadCurve>();
        fullLoadCurves[0] = FullLoadCurveReader.Create(
            VectoCSVFile.ReadStream(
                InputDataHelper.InputDataAsStream("engine speed [1/min],full load torque [Nm],motoring torque [Nm],PT1 [s]",
                    fld)));
        fullLoadCurves[0].EngineData = engineData;
        foreach (var gears in gbxData.Gears) {
            fullLoadCurves[(uint)gears.Gear] = fullLoadCurves[0];
        }
        engineData.FullLoadCurves = fullLoadCurves;
        return new VectoRunData() {
            VehicleData = new VehicleData() {
                DynamicTyreRadius = 0.492.SI<Meter>(),
            },
            AxleGearData = new AxleGearData() {
                AxleGear = new GearData() {
                    Ratio = 2.64
                }
            },
            EngineData = engineData,
            GearshiftParameters = new ShiftStrategyParameters() {
                StartSpeed = 2.SI<MeterPerSecond>(),
                StartAcceleration = DeclarationData.GearboxTCU.StartAcceleration,
                TimeBetweenGearshifts = DeclarationData.Gearbox.MinTimeBetweenGearshifts,
                DownshiftAfterUpshiftDelay = DeclarationData.Gearbox.DownshiftAfterUpshiftDelay,
                UpshiftAfterDownshiftDelay = DeclarationData.Gearbox.UpshiftAfterDownshiftDelay,
                UpshiftMinAcceleration = DeclarationData.Gearbox.UpshiftMinAcceleration,
            },
        };
    }

	private const string EngineFldHdr = "n in rpm,M_FL in Nm,M_frict in Nm,PT1 in s";

	private static string[] EngineFldData = new[] {
		"575,570,-12,0.21",
		"800,834,-16,0.47",
		"1000,1068,-24,0.58",
		"1200,1198,-33,0.53",
		"1400,1198,-44,0.46",
		"1600,1198,-56,0.35",
		"1800,1122,-67,0.20",
		"2000,1036,-82,0.11",
		"2100,995,-89,0.11",
		"2200,952,-97,0.11",
		"2400,813,-119,0.11",
		"2500,709,-134,0.11",
		"2600,0,-148,0.11",
	};

	public const string LossMapHdr = "Input Speed [rpm],Input Torque [Nm],Torque Loss [Nm]";

    public static readonly string[] LossMapIndirect = new[] {
        "0,-650,18.06",
        "0,-850,22.06",
        "0,-1050,26.06",
        "0,-1250,30.06",
        "0,-1450,34.06",
        "0,-1650,38.06",
        "0,-1850,42.06",
        "0,-2050,46.06",
        "0,-2250,50.06",
        "0,-2450,54.06",
        "0,-350,12.06",
        "0,-150,8.06",
        "0,50,6.06",
        "0,250,10.06",
        "0,450,14.06",
        "0,650,18.06",
        "0,850,22.06",
        "0,1050,26.06",
        "0,1250,30.06",
        "0,1450,34.06",
        "0,1650,38.06",
        "0,1850,42.06",
        "0,2050,46.06",
        "0,2250,50.06",
        "0,2450,54.06",
        "200,-650,19.072",
        "200,-850,23.072",
        "200,-1050,27.072",
        "200,-1250,31.072",
        "200,-1450,35.072",
        "200,-1650,39.072",
        "200,-1850,43.072",
        "200,-2050,47.072",
        "200,-2250,51.072",
        "200,-2450,55.072",
        "200,-350,13.072",
        "200,-150,9.072",
        "200,50,7.072",
        "200,250,11.072",
        "200,450,15.072",
        "200,650,19.072",
        "200,850,23.072",
        "200,1050,27.072",
        "200,1250,31.072",
        "200,1450,35.072",
        "200,1650,39.072",
        "200,1850,43.072",
        "200,2050,47.072",
        "200,2250,51.072",
        "200,2450,55.072",
        "400,-650,20.084",
        "400,-850,24.084",
        "400,-1050,28.084",
        "400,-1250,32.084",
        "400,-1450,36.084",
        "400,-1650,40.084",
        "400,-1850,44.084",
        "400,-2050,48.084",
        "400,-2250,52.084",
        "400,-2450,56.084",
        "400,-350,14.084",
        "400,-150,10.084",
        "400,50,8.084",
        "400,250,12.084",
        "400,450,16.084",
        "400,650,20.084",
        "400,850,24.084",
        "400,1050,28.084",
        "400,1250,32.084",
        "400,1450,36.084",
        "400,1650,40.084",
        "400,1850,44.084",
        "400,2050,48.084",
        "400,2250,52.084",
        "400,2450,56.084",
        "600,-650,21.096",
        "600,-850,25.096",
        "600,-1050,29.096",
        "600,-1250,33.096",
        "600,-1450,37.096",
        "600,-1650,41.096",
        "600,-1850,45.096",
        "600,-2050,49.096",
        "600,-2250,53.096",
        "600,-350,15.096",
        "600,-150,11.096",
        "600,50,9.096",
        "600,250,13.096",
        "600,450,17.096",
        "600,650,21.096",
        "600,850,25.096",
        "600,1050,29.096",
        "600,1250,33.096",
        "600,1450,37.096",
        "600,1650,41.096",
        "600,1850,45.096",
        "600,2050,49.096",
        "600,2250,53.096",
        "600,2450,57.096",
        "800,-650,22.108",
        "800,-850,26.108",
        "800,-1050,30.108",
        "800,-1250,34.108",
        "800,-1450,38.108",
        "800,-1650,42.108",
        "800,-1850,46.108",
        "800,-2050,50.108",
        "800,-2250,54.108",
        "800,-2450,58.108",
        "800,-350,16.108",
        "800,-150,12.108",
        "800,50,10.108",
        "800,250,14.108",
        "800,450,18.108",
        "800,650,22.108",
        "800,850,26.108",
        "800,1050,30.108",
        "800,1250,34.108",
        "800,1450,38.108",
        "800,1650,42.108",
        "800,1850,46.108",
        "800,2050,50.108",
        "800,2250,54.108",
        "800,2450,58.108",
        "1000,-650,23.12",
        "1000,-850,27.12",
        "1000,-1050,31.12",
        "1000,-1250,35.12",
        "1000,-1450,39.12",
        "1000,-1650,43.12",
        "1000,-1850,47.12",
        "1000,-2050,51.12",
        "1000,-2250,55.12",
        "1000,-2450,59.12",
        "1000,-350,17.12",
        "1000,-150,13.12",
        "1000,50,11.12",
        "1000,250,15.12",
        "1000,450,19.12",
        "1000,650,23.12",
        "1000,850,27.12",
        "1000,1050,31.12",
        "1000,1250,35.12",
        "1000,1450,39.12",
        "1000,1650,43.12",
        "1000,1850,47.12",
        "1000,2050,51.12",
        "1000,2250,55.12",
        "1000,2450,59.12",
        "1200,-650,24.132",
        "1200,-850,28.132",
        "1200,-1050,32.132",
        "1200,-1250,36.132",
        "1200,-1450,40.132",
        "1200,-1650,44.132",
        "1200,-1850,48.132",
        "1200,-2050,52.132",
        "1200,-2250,56.132",
        "1200,-2450,60.132",
        "1200,-350,18.132",
        "1200,-150,14.132",
        "1200,50,12.132",
        "1200,250,16.132",
        "1200,450,20.132",
        "1200,650,24.132",
        "1200,850,28.132",
        "1200,1050,32.132",
        "1200,1250,36.132",
        "1200,1450,40.132",
        "1200,1650,44.132",
        "1200,1850,48.132",
        "1200,2050,52.132",
        "1200,2250,56.132",
        "1200,2450,60.132",
        "1400,-650,25.144",
        "1400,-850,29.144",
        "1400,-1050,33.144",
        "1400,-1250,37.144",
        "1400,-1450,41.144",
        "1400,-1650,45.144",
        "1400,-1850,49.144",
        "1400,-2050,53.144",
        "1400,-2250,57.144",
        "1400,-2450,61.144",
        "1400,-350,19.144",
        "1400,-150,15.144",
        "1400,50,13.144",
        "1400,250,17.144",
        "1400,450,21.144",
        "1400,650,25.144",
        "1400,850,29.144",
        "1400,1050,33.144",
        "1400,1250,37.144",
        "1400,1450,41.144",
        "1400,1650,45.144",
        "1400,1850,49.144",
        "1400,2050,53.144",
        "1400,2250,57.144",
        "1400,2450,61.144",
        "1600,-650,26.156",
        "1600,-850,30.156",
        "1600,-1050,34.156",
        "1600,-1250,38.156",
        "1600,-1450,42.156",
        "1600,-1650,46.156",
        "1600,-1850,50.156",
        "1600,-2050,54.156",
        "1600,-2250,58.156",
        "1600,-2450,62.156",
        "1600,-350,20.156",
        "1600,-150,16.156",
        "1600,50,14.156",
        "1600,250,18.156",
        "1600,450,22.156",
        "1600,650,26.156",
        "1600,850,30.156",
        "1600,1050,34.156",
        "1600,1250,38.156",
        "1600,1450,42.156",
        "1600,1650,46.156",
        "1600,1850,50.156",
        "1600,2050,54.156",
        "1600,2250,58.156",
        "1600,2450,62.156",
        "1800,-650,27.168",
        "1800,-850,31.168",
        "1800,-1050,35.168",
        "1800,-1250,39.168",
        "1800,-1450,43.168",
        "1800,-1650,47.168",
        "1800,-1850,51.168",
        "1800,-2050,55.168",
        "1800,-2250,59.168",
        "1800,-2450,63.168",
        "1800,-350,21.168",
        "1800,-150,17.168",
        "1800,50,15.168",
        "1800,250,19.168",
        "1800,450,23.168",
        "1800,650,27.168",
        "1800,850,31.168",
        "1800,1050,35.168",
        "1800,1250,39.168",
        "1800,1450,43.168",
        "1800,1650,47.168",
        "1800,1850,51.168",
        "1800,2050,55.168",
        "1800,2250,59.168",
        "1800,2450,63.168",
        "2000,-650,28.18",
        "2000,-850,32.18",
        "2000,-1050,36.18",
        "2000,-1250,40.18",
        "2000,-1450,44.18",
        "2000,-1650,48.18",
        "2000,-1850,52.18",
        "2000,-2050,56.18",
        "2000,-2250,60.18",
        "2000,-2450,64.18",
        "2000,-350,22.18",
        "2000,-150,18.18",
        "2000,50,16.18",
        "2000,250,20.18",
        "2000,450,24.18",
        "2000,650,28.18",
        "2000,850,32.18",
        "2000,1050,36.18",
        "2000,1250,40.18",
        "2000,1450,44.18",
        "2000,1650,48.18",
        "2000,1850,52.18",
        "2000,2050,56.18",
        "2000,2250,60.18",
        "2000,2450,64.18",
        "3000,-650,28.18",
        "3000,-850,32.18",
        "3000,-1050,36.18",
        "3000,-1250,40.18",
        "3000,-1450,44.18",
        "3000,-1650,48.18",
        "3000,-1850,52.18",
        "3000,-2050,56.18",
        "3000,-2250,60.18",
        "3000,-2450,64.18",
        "3000,-350,22.18",
        "3000,-150,18.18",
        "3000,50,16.18",
        "3000,250,20.18",
        "3000,450,24.18",
        "3000,650,28.18",
        "3000,850,32.18",
        "3000,1050,36.18",
        "3000,1250,40.18",
        "3000,1450,44.18",
        "3000,1650,48.18",
        "3000,1850,52.18",
        "3000,2050,56.18",
        "3000,2250,60.18",
        "3000,2450,64.18",
    };

    public static readonly string[] LossMapDirect = new[] {
        "0,-650,8.31",
        "0,-850,9.31",
        "0,-1050,10.31",
        "0,-1250,11.31",
        "0,-1450,12.31",
        "0,-1650,13.31",
        "0,-1850,14.31",
        "0,-2050,15.31",
        "0,-2250,16.31",
        "0,-2450,17.31",
        "0,-350,6.81",
        "0,-150,5.81",
        "0,50,5.31",
        "0,250,6.31",
        "0,450,7.31",
        "0,650,8.31",
        "0,850,9.31",
        "0,1050,10.31",
        "0,1250,11.31",
        "0,1450,12.31",
        "0,1650,13.31",
        "0,1850,14.31",
        "0,2050,15.31",
        "0,2250,16.31",
        "0,2450,17.31",
        "200,-650,9.322",
        "200,-850,10.322",
        "200,-1050,11.322",
        "200,-1250,12.322",
        "200,-1450,13.322",
        "200,-1650,14.322",
        "200,-1850,15.322",
        "200,-2050,16.322",
        "200,-2250,17.322",
        "200,-2450,18.322",
        "200,-350,7.822",
        "200,-150,6.822",
        "200,50,6.322",
        "200,250,7.322",
        "200,450,8.322",
        "200,650,9.322",
        "200,850,10.322",
        "200,1050,11.322",
        "200,1250,12.322",
        "200,1450,13.322",
        "200,1650,14.322",
        "200,1850,15.322",
        "200,2050,16.322",
        "200,2250,17.322",
        "200,2450,18.322",
        "400,-650,10.334",
        "400,-850,11.334",
        "400,-1050,12.334",
        "400,-1250,13.334",
        "400,-1450,14.334",
        "400,-1650,15.334",
        "400,-1850,16.334",
        "400,-2050,17.334",
        "400,-2250,18.334",
        "400,-2450,19.334",
        "400,-350,8.834",
        "400,-150,7.834",
        "400,50,7.334",
        "400,250,8.334",
        "400,450,9.334",
        "400,650,10.334",
        "400,850,11.334",
        "400,1050,12.334",
        "400,1250,13.334",
        "400,1450,14.334",
        "400,1650,15.334",
        "400,1850,16.334",
        "400,2050,17.334",
        "400,2250,18.334",
        "400,2450,19.334",
        "600,-650,11.346",
        "600,-850,12.346",
        "600,-1050,13.346",
        "600,-1250,14.346",
        "600,-1450,15.346",
        "600,-1650,16.346",
        "600,-1850,17.346",
        "600,-2050,18.346",
        "600,-2250,19.346",
        "600,-2450,20.346",
        "600,-350,9.846",
        "600,-150,8.846",
        "600,50,8.346",
        "600,250,9.346",
        "600,450,10.346",
        "600,650,11.346",
        "600,850,12.346",
        "600,1050,13.346",
        "600,1250,14.346",
        "600,1450,15.346",
        "600,1650,16.346",
        "600,1850,17.346",
        "600,2050,18.346",
        "600,2250,19.346",
        "600,2450,20.346",
        "800,-650,12.358",
        "800,-850,13.358",
        "800,-1050,14.358",
        "800,-1250,15.358",
        "800,-1450,16.358",
        "800,-1650,17.358",
        "800,-1850,18.358",
        "800,-2050,19.358",
        "800,-2250,20.358",
        "800,-2450,21.358",
        "800,-350,10.858",
        "800,-150,9.858",
        "800,50,9.358",
        "800,250,10.358",
        "800,450,11.358",
        "800,650,12.358",
        "800,850,13.358",
        "800,1050,14.358",
        "800,1250,15.358",
        "800,1450,16.358",
        "800,1650,17.358",
        "800,1850,18.358",
        "800,2050,19.358",
        "800,2250,20.358",
        "800,2450,21.358",
        "1000,-650,13.37",
        "1000,-850,14.37",
        "1000,-1050,15.37",
        "1000,-1250,16.37",
        "1000,-1450,17.37",
        "1000,-1650,18.37",
        "1000,-1850,19.37",
        "1000,-2050,20.37",
        "1000,-2250,21.37",
        "1000,-2450,22.37",
        "1000,-350,11.87",
        "1000,-150,10.87",
        "1000,50,10.37",
        "1000,250,11.37",
        "1000,450,12.37",
        "1000,650,13.37",
        "1000,850,14.37",
        "1000,1050,15.37",
        "1000,1250,16.37",
        "1000,1450,17.37",
        "1000,1650,18.37",
        "1000,1850,19.37",
        "1000,2050,20.37",
        "1000,2250,21.37",
        "1000,2450,22.37",
        "1200,-650,14.382",
        "1200,-850,15.382",
        "1200,-1050,16.382",
        "1200,-1250,17.382",
        "1200,-1450,18.382",
        "1200,-1650,19.382",
        "1200,-1850,20.382",
        "1200,-2050,21.382",
        "1200,-2250,22.382",
        "1200,-2450,23.382",
        "1200,-350,12.882",
        "1200,-150,11.882",
        "1200,50,11.382",
        "1200,250,12.382",
        "1200,450,13.382",
        "1200,650,14.382",
        "1200,850,15.382",
        "1200,1050,16.382",
        "1200,1250,17.382",
        "1200,1450,18.382",
        "1200,1650,19.382",
        "1200,1850,20.382",
        "1200,2050,21.382",
        "1200,2250,22.382",
        "1200,2450,23.382",
        "1400,-650,15.394",
        "1400,-850,16.394",
        "1400,-1050,17.394",
        "1400,-1250,18.394",
        "1400,-1450,19.394",
        "1400,-1650,20.394",
        "1400,-1850,21.394",
        "1400,-2050,22.394",
        "1400,-2250,23.394",
        "1400,-350,13.894",
        "1400,-150,12.894",
        "1400,50,12.394",
        "1400,250,13.394",
        "1400,450,14.394",
        "1400,650,15.394",
        "1400,850,16.394",
        "1400,1050,17.394",
        "1400,1250,18.394",
        "1400,1450,19.394",
        "1400,1650,20.394",
        "1400,1850,21.394",
        "1400,2050,22.394",
        "1400,2250,23.394",
        "1400,2450,24.394",
        "1600,-650,16.406",
        "1600,-850,17.406",
        "1600,-1050,18.406",
        "1600,-1250,19.406",
        "1600,-1450,20.406",
        "1600,-1650,21.406",
        "1600,-1850,22.406",
        "1600,-2050,23.406",
        "1600,-2250,24.406",
        "1600,-2450,25.406",
        "1600,-350,14.906",
        "1600,-150,13.906",
        "1600,50,13.406",
        "1600,250,14.406",
        "1600,450,15.406",
        "1600,650,16.406",
        "1600,850,17.406",
        "1600,1050,18.406",
        "1600,1250,19.406",
        "1600,1450,20.406",
        "1600,1650,21.406",
        "1600,1850,22.406",
        "1600,2050,23.406",
        "1600,2250,24.406",
        "1600,2450,25.406",
        "1800,-650,17.418",
        "1800,-850,18.418",
        "1800,-1050,19.418",
        "1800,-1250,20.418",
        "1800,-1450,21.418",
        "1800,-1650,22.418",
        "1800,-1850,23.418",
        "1800,-2050,24.418",
        "1800,-2250,25.418",
        "1800,-2450,26.418",
        "1800,-350,15.918",
        "1800,-150,14.918",
        "1800,50,14.418",
        "1800,250,15.418",
        "1800,450,16.418",
        "1800,650,17.418",
        "1800,850,18.418",
        "1800,1050,19.418",
        "1800,1250,20.418",
        "1800,1450,21.418",
        "1800,1650,22.418",
        "1800,1850,23.418",
        "1800,2050,24.418",
        "1800,2250,25.418",
        "1800,2450,26.418",
        "2000,-650,18.43",
        "2000,-850,19.43",
        "2000,-1050,20.43",
        "2000,-1250,21.43",
        "2000,-1450,22.43",
        "2000,-1650,23.43",
        "2000,-1850,24.43",
        "2000,-2050,25.43",
        "2000,-2250,26.43",
        "2000,-350,16.93",
        "2000,-150,15.93",
        "2000,50,15.43",
        "2000,250,16.43",
        "2000,450,17.43",
        "2000,650,18.43",
        "2000,850,19.43",
        "2000,1050,20.43",
        "2000,1250,21.43",
        "2000,1450,22.43",
        "2000,1650,23.43",
        "2000,1850,24.43",
        "2000,2050,25.43",
        "2000,2250,26.43",
        "3000,-650,18.43",
        "3000,-850,19.43",
        "3000,-1050,20.43",
        "3000,-1250,21.43",
        "3000,-1450,22.43",
        "3000,-1650,23.43",
        "3000,-1850,24.43",
        "3000,-2050,25.43",
        "3000,-2250,26.43",
        "3000,-2450,27.43",
        "3000,-350,16.93",
        "3000,-150,15.93",
        "3000,50,15.43",
        "3000,250,16.43",
        "3000,450,17.43",
        "3000,650,18.43",
        "3000,850,19.43",
        "3000,1050,20.43",
        "3000,1250,21.43",
        "3000,1450,22.43",
        "3000,1650,23.43",
        "3000,1850,24.43",
        "3000,2050,25.43",
        "3000,2250,26.43",
        "3000,2450,27.43",
    };

    const string ShiftPolyHdr = "M [Nm],nDown [rpm],nUp [rpm]";

	private static readonly string[] ShiftPolyData = new[] {
		"-400,560,1289",
		"759,560,1289",
		"1252,742,1289",
		"2372,1155,1942",
	};

    const string AxlMapHdr = "Input Speed [rpm],Input Torque [Nm],Torque Loss [Nm] # this is a comment";

    private readonly static string[] AxlMapData = new string[] {
        "# rpm, Nm, Nm",
        "# this is a comment",
        "0,-2500,77.5  # this is a comment",
        "0,-1500,62.5",
        "0,-500,47.5",
        "0,500,47.5",
        "0,1500,62.5",
        "0,2500,77.5",
        "0,3500,92.5",
        "0,4500,107.5",
        "# this is a comment",
        "0,5500,122.5",
        "0,6500,137.5",
        "0,7500,152.5",
        "0,8500,167.5",
        "0,9500,182.5",
        "0,10500,197.5",
        "0,11500,212.5",
        "0,12500,227.5",
        "0,13500,242.5",
        "0,14500,257.5",
        "0,15500,272.5",
        "200,-2500,77.5",
        "200,-1500,62.5",
        "200,-500,47.5",
        "200,500,47.5",
        "200,1500,62.5",
        "200,2500,77.5",
        "200,3500,92.5",
        "200,4500,107.5",
        "200,5500,122.5",
        "200,6500,137.5",
        "200,7500,152.5",
        "200,8500,167.5",
        "200,9500,182.5",
        "200,10500,197.5",
        "200,11500,212.5",
        "200,12500,227.5",
        "200,13500,242.5",
        "200,14500,257.5",
        "200,15500,272.5",
        "400,-2500,77.5",
        "400,-1500,62.5",
        "400,-500,47.5",
        "400,500,47.5",
        "400,1500,62.5",
        "400,2500,77.5",
        "400,3500,92.5",
        "400,4500,107.5",
        "400,5500,122.5",
        "400,6500,137.5",
        "400,7500,152.5",
        "400,8500,167.5",
        "400,9500,182.5",
        "400,10500,197.5",
        "400,11500,212.5",
        "400,12500,227.5",
        "400,13500,242.5",
        "400,14500,257.5",
        "400,15500,272.5",
        "600,-2500,77.5",
        "600,-1500,62.5",
        "600,-500,47.5",
        "600,500,47.5",
        "600,1500,62.5",
        "600,2500,77.5",
        "600,3500,92.5",
        "600,4500,107.5",
        "600,5500,122.5",
        "600,6500,137.5",
        "600,7500,152.5",
        "600,8500,167.5",
        "600,9500,182.5",
        "600,10500,197.5",
        "600,11500,212.5",
        "600,12500,227.5",
        "600,13500,242.5",
        "600,14500,257.5",
        "600,15500,272.5",
        "800,-2500,77.5",
        "800,-1500,62.5",
        "800,-500,47.5",
        "800,500,47.5",
        "800,1500,62.5",
        "800,2500,77.5",
        "800,3500,92.5",
        "800,4500,107.5",
        "800,5500,122.5",
        "800,6500,137.5",
        "800,7500,152.5",
        "800,8500,167.5",
        "800,9500,182.5",
        "800,10500,197.5",
        "800,11500,212.5",
        "800,12500,227.5",
        "800,13500,242.5",
        "800,14500,257.5",
        "800,15500,272.5",
        "1000,-2500,77.5",
        "1000,-1500,62.5",
        "1000,-500,47.5",
        "1000,500,47.5",
        "1000,1500,62.5",
        "1000,2500,77.5",
        "1000,3500,92.5",
        "1000,4500,107.5",
        "1000,5500,122.5",
        "1000,6500,137.5",
        "1000,7500,152.5",
        "1000,8500,167.5",
        "1000,9500,182.5",
        "1000,10500,197.5",
        "1000,11500,212.5",
        "1000,12500,227.5",
        "1000,13500,242.5",
        "1000,14500,257.5",
        "1000,15500,272.5",
        "1200,-2500,77.5",
        "1200,-1500,62.5",
        "1200,-500,47.5",
        "1200,500,47.5",
        "1200,1500,62.5",
        "1200,2500,77.5",
        "1200,3500,92.5",
        "1200,4500,107.5",
        "1200,5500,122.5",
        "1200,6500,137.5",
        "1200,7500,152.5",
        "1200,8500,167.5",
        "1200,9500,182.5",
        "1200,10500,197.5",
        "1200,11500,212.5",
        "1200,12500,227.5",
        "1200,13500,242.5",
        "1200,14500,257.5",
        "1200,15500,272.5",
        "1400,-2500,77.5",
        "1400,-1500,62.5",
        "1400,-500,47.5",
        "1400,500,47.5",
        "1400,1500,62.5",
        "1400,2500,77.5",
        "1400,3500,92.5",
        "1400,4500,107.5",
        "1400,5500,122.5",
        "1400,6500,137.5",
        "1400,7500,152.5",
        "1400,8500,167.5",
        "1400,9500,182.5",
        "1400,10500,197.5",
        "1400,11500,212.5",
        "1400,12500,227.5",
        "1400,13500,242.5",
        "1400,14500,257.5",
        "1400,15500,272.5",
        "1600,-2500,77.5",
        "1600,-1500,62.5",
        "1600,-500,47.5",
        "1600,500,47.5",
        "1600,1500,62.5",
        "1600,2500,77.5",
        "1600,3500,92.5",
        "1600,4500,107.5",
        "1600,5500,122.5",
        "1600,6500,137.5",
        "1600,7500,152.5",
        "1600,8500,167.5",
        "1600,9500,182.5",
        "1600,10500,197.5",
        "1600,11500,212.5",
        "1600,12500,227.5",
        "1600,13500,242.5",
        "1600,14500,257.5",
        "1600,15500,272.5",
        "1800,-2500,77.5",
        "1800,-1500,62.5",
        "1800,-500,47.5",
        "1800,500,47.5",
        "1800,1500,62.5",
        "1800,2500,77.5",
        "1800,3500,92.5",
        "1800,4500,107.5",
        "1800,5500,122.5",
        "1800,6500,137.5",
        "1800,7500,152.5",
        "1800,8500,167.5",
        "1800,9500,182.5",
        "1800,10500,197.5",
        "1800,11500,212.5",
        "1800,12500,227.5",
        "1800,13500,242.5",
        "1800,14500,257.5",
        "1800,15500,272.5",
        "2000,-2500,77.5",
        "2000,-1500,62.5",
        "2000,-500,47.5",
        "2000,500,47.5",
        "2000,1500,62.5",
        "2000,2500,77.5",
        "2000,3500,92.5",
        "2000,4500,107.5",
        "2000,5500,122.5",
        "2000,6500,137.5",
        "2000,7500,152.5",
        "2000,8500,167.5",
        "2000,9500,182.5",
        "2000,10500,197.5",
        "2000,11500,212.5",
        "2000,12500,227.5",
        "2000,13500,242.5",
        "2000,14500,257.5",
        "2000,15500,272.5",
        "2200,-2500,77.5",
        "2200,-1500,62.5",
        "2200,-500,47.5",
        "2200,500,47.5",
        "2200,1500,62.5",
        "2200,2500,77.5",
        "2200,3500,92.5",
        "2200,4500,107.5",
        "2200,5500,122.5",
        "2200,6500,137.5",
        "2200,7500,152.5",
        "2200,8500,167.5",
        "2200,9500,182.5",
        "2200,10500,197.5",
        "2200,11500,212.5",
        "2200,12500,227.5",
        "2200,13500,242.5",
        "2200,14500,257.5",
        "2200,15500,272.5",
        "2400,-2500,77.5",
        "2400,-1500,62.5",
        "2400,-500,47.5",
        "2400,500,47.5",
        "2400,1500,62.5",
        "2400,2500,77.5",
        "2400,3500,92.5",
        "2400,4500,107.5",
        "2400,5500,122.5",
        "2400,6500,137.5",
        "2400,7500,152.5",
        "2400,8500,167.5",
        "2400,9500,182.5",
        "2400,10500,197.5",
        "2400,11500,212.5",
        "2400,12500,227.5",
        "2400,13500,242.5",
        "2400,14500,257.5",
        "2400,15500,272.5",
        "2600,-2500,77.5",
        "2600,-1500,62.5",
        "2600,-500,47.5",
        "2600,500,47.5",
        "2600,1500,62.5",
        "2600,2500,77.5",
        "2600,3500,92.5",
        "2600,4500,107.5",
        "2600,5500,122.5",
        "2600,6500,137.5",
        "2600,7500,152.5",
        "2600,8500,167.5",
        "2600,9500,182.5",
        "2600,10500,197.5",
        "2600,11500,212.5",
        "2600,12500,227.5",
        "2600,13500,242.5",
        "2600,14500,257.5",
        "2600,15500,272.5",
        "3600,-2500,77.5",
        "3600,-1500,62.5",
        "3600,-500,47.5",
        "3600,500,47.5",
        "3600,1500,62.5",
        "3600,2500,77.5",
        "3600,3500,92.5",
        "3600,4500,107.5",
        "3600,5500,122.5",
        "3600,6500,137.5",
        "3600,7500,152.5",
        "3600,8500,167.5",
        "3600,9500,182.5",
        "3600,10500,197.5",
        "3600,11500,212.5",
        "3600,12500,227.5",
        "3600,13500,242.5",
        "3600,14500,257.5",
        "3600,15500,272.5",
    };
}