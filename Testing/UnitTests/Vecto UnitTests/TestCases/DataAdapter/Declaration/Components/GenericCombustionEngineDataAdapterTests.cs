using Moq;
using NUnit.Framework;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.Reader.ComponentData;
using TUGraz.VectoCore.InputData.Reader.DataObjectAdapter.SimulationComponents;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Tests.Utils;

namespace TUGraz.Vecto.UnitTests.TestCases.DataAdapter.Declaration.Components;

public class GenericCombustionEngineDataAdapterTests
{

	[TestCase()]
	public void TestGenericCombustionEngine_Conventional_SingleFuel_Diesel()
	{
		var dao = new GenericCombustionEngineComponentDataAdapter();

		var mockPrimaryVehicle = GetPrimaryVehicleConventional(FuelType.DieselCI);
		var mission = new Mission() {
			MissionType = MissionType.Urban,
		};
		var iceData = dao.CreateEngineData(mockPrimaryVehicle.Object, 0, mission);

		Assert.NotNull(iceData);
		Assert.AreEqual(1, iceData.Fuels.Count);
		Assert.AreEqual(FuelType.DieselCI, iceData.Fuels.First().FuelData.FuelType);
		Assert.AreEqual(9, iceData.FullLoadCurves.Count);

		var expectedFld = FullLoadCurveReader.Create(InputDataHelper.InputDataAsTableData(ICE_FLD_HDR, ICE_FLD_Data));
		foreach (var (gear, fld) in iceData.FullLoadCurves) {
			foreach (var expectedEntry in expectedFld.FullLoadEntries) {
				Assert.AreEqual(expectedEntry.TorqueFullLoad.Value(), fld.FullLoadStationaryTorque(expectedEntry.EngineSpeed).Value(), 1e-6);
				Assert.AreEqual(expectedEntry.TorqueDrag.Value(), fld.DragLoadStationaryTorque(expectedEntry.EngineSpeed).Value(), 1e-6);
			}
        }

		foreach (var eFc in ExpectedFC_Diesel) {
			var fc = iceData.Fuels.First().ConsumptionMap.GetFuelConsumption(eFc[1].SI<NewtonMeter>(), eFc[0].RPMtoRad(), false);
			Assert.AreEqual(eFc[2], fc.Value.ConvertToGrammPerHour().Value, 1e-3);
		}

		Assert.AreEqual(1.05, iceData.Fuels.First().WHTCUrban);
		Assert.AreEqual(1.02, iceData.Fuels.First().WHTCRural);
		Assert.AreEqual(1.0, iceData.Fuels.First().WHTCMotorway);
		Assert.AreEqual(1.005, iceData.Fuels.First().ColdHotCorrectionFactor);
		Assert.AreEqual(1.0, iceData.Fuels.First().CorrectionFactorRegPer);
	}

	[TestCase()]
	public void TestGenericCombustionEngine_Conventional_GbxLimits_SingleFuel_Diesel()
	{
		var dao = new GenericCombustionEngineComponentDataAdapter();

		var mockPrimaryVehicle = GetPrimaryVehicleConventional(FuelType.DieselCI);

		var tqLimits = new[] { -1, -1, -1, -1, 2000, 2000, 2000, -1 }.Select((x, idx) => {
			var limit = new Mock<ITorqueLimitInputData>();
			limit.Setup(l => l.Gear).Returns(idx + 1);
			limit.Setup(l => l.MaxTorque).Returns(x.SI<NewtonMeter>());
			return limit.Object;
		}).Where(x => x.MaxTorque >= 0).ToList();
		mockPrimaryVehicle.Setup(v => v.TorqueLimits).Returns(tqLimits);
		var mission = new Mission() {
			MissionType = MissionType.Urban,
		};
		var iceData = dao.CreateEngineData(mockPrimaryVehicle.Object, 0, mission);

		Assert.NotNull(iceData);
		Assert.AreEqual(1, iceData.Fuels.Count);
		Assert.AreEqual(FuelType.DieselCI, iceData.Fuels.First().FuelData.FuelType);
		Assert.AreEqual(9, iceData.FullLoadCurves.Count);

        var expectedFld = FullLoadCurveReader.Create(InputDataHelper.InputDataAsTableData(ICE_FLD_HDR, ICE_FLD_Data));
		foreach (var (gear, fld) in iceData.FullLoadCurves) {
			foreach (var expectedEntry in expectedFld.FullLoadEntries) {
				var maxTq = tqLimits.FirstOrDefault(x => x.Gear == gear)?.MaxTorque;
				Assert.AreEqual(VectoMath.Min(maxTq, expectedEntry.TorqueFullLoad).Value(), fld.FullLoadStationaryTorque(expectedEntry.EngineSpeed).Value(), 1e-6);
				Assert.AreEqual(expectedEntry.TorqueDrag.Value(), fld.DragLoadStationaryTorque(expectedEntry.EngineSpeed).Value(), 1e-6);
			}
		}

		foreach (var eFc in ExpectedFC_Diesel) {
			var fc = iceData.Fuels.First().ConsumptionMap.GetFuelConsumption(eFc[1].SI<NewtonMeter>(), eFc[0].RPMtoRad(), false);
			Assert.AreEqual(eFc[2], fc.Value.ConvertToGrammPerHour().Value, 1e-3);
		}

		Assert.AreEqual(1.05, iceData.Fuels.First().WHTCUrban);
		Assert.AreEqual(1.02, iceData.Fuels.First().WHTCRural);
		Assert.AreEqual(1.0, iceData.Fuels.First().WHTCMotorway);
		Assert.AreEqual(1.005, iceData.Fuels.First().ColdHotCorrectionFactor);
		Assert.AreEqual(1.0, iceData.Fuels.First().CorrectionFactorRegPer);
	}

	[TestCase()]
    public void TestGenericCombustionEngine_Conventional_DisabledGear_SingleFuel_Diesel()
    {
        var dao = new GenericCombustionEngineComponentDataAdapter();

        var mockPrimaryVehicle = GetPrimaryVehicleConventional(FuelType.DieselCI);

        var tqLimits = new[] { -1, -1, -1, -1, -1, -1, 2000, 0 }.Select((x, idx) => {
            var limit = new Mock<ITorqueLimitInputData>();
            limit.Setup(l => l.Gear).Returns(idx + 1);
            limit.Setup(l => l.MaxTorque).Returns(x.SI<NewtonMeter>());
            return limit.Object;
        }).Where(x => x.MaxTorque >= 0).ToList();
        mockPrimaryVehicle.Setup(v => v.TorqueLimits).Returns(tqLimits);
        var mission = new Mission() {
            MissionType = MissionType.Urban,
        };
        var iceData = dao.CreateEngineData(mockPrimaryVehicle.Object, 0, mission);

        Assert.NotNull(iceData);
        Assert.AreEqual(1, iceData.Fuels.Count);
        Assert.AreEqual(FuelType.DieselCI, iceData.Fuels.First().FuelData.FuelType);
		Assert.AreEqual(8, iceData.FullLoadCurves.Count);

        var expectedFld = FullLoadCurveReader.Create(InputDataHelper.InputDataAsTableData(ICE_FLD_HDR, ICE_FLD_Data));
        foreach (var (gear, fld) in iceData.FullLoadCurves) {
            foreach (var expectedEntry in expectedFld.FullLoadEntries) {
                var maxTq = tqLimits.FirstOrDefault(x => x.Gear == gear)?.MaxTorque;
                Assert.AreEqual(VectoMath.Min(maxTq, expectedEntry.TorqueFullLoad).Value(), fld.FullLoadStationaryTorque(expectedEntry.EngineSpeed).Value(), 1e-6);
                Assert.AreEqual(expectedEntry.TorqueDrag.Value(), fld.DragLoadStationaryTorque(expectedEntry.EngineSpeed).Value(), 1e-6);
            }
        }

        foreach (var eFc in ExpectedFC_Diesel) {
            var fc = iceData.Fuels.First().ConsumptionMap.GetFuelConsumption(eFc[1].SI<NewtonMeter>(), eFc[0].RPMtoRad(), false);
            Assert.AreEqual(eFc[2], fc.Value.ConvertToGrammPerHour().Value, 1e-3);
        }

        Assert.AreEqual(1.05, iceData.Fuels.First().WHTCUrban);
        Assert.AreEqual(1.02, iceData.Fuels.First().WHTCRural);
        Assert.AreEqual(1.0, iceData.Fuels.First().WHTCMotorway);
        Assert.AreEqual(1.005, iceData.Fuels.First().ColdHotCorrectionFactor);
        Assert.AreEqual(1.0, iceData.Fuels.First().CorrectionFactorRegPer);
    }


    [TestCase()]
	public void TestGenericCombustionEngine_Conventional_SingleFuel_LPGPI()
	{
		var dao = new GenericCombustionEngineComponentDataAdapter();

		var mockPrimaryVehicle = GetPrimaryVehicleConventional(FuelType.LPGPI);
		var mission = new Mission() {
			MissionType = MissionType.Urban,
		};
		var iceData = dao.CreateEngineData(mockPrimaryVehicle.Object, 0, mission);

		Assert.NotNull(iceData);
		Assert.AreEqual(1, iceData.Fuels.Count);
		// for non-Diesel fuels NGPI is always used
		Assert.AreEqual(FuelType.NGPI, iceData.Fuels.First().FuelData.FuelType);

		var expectedFld = FullLoadCurveReader.Create(InputDataHelper.InputDataAsTableData(ICE_FLD_HDR, ICE_FLD_Data));
		foreach (var (gear, fld) in iceData.FullLoadCurves) {
			foreach (var expectedEntry in expectedFld.FullLoadEntries) {
				Assert.AreEqual(expectedEntry.TorqueFullLoad.Value(), fld.FullLoadStationaryTorque(expectedEntry.EngineSpeed).Value(), 1e-6);
				Assert.AreEqual(expectedEntry.TorqueDrag.Value(), fld.DragLoadStationaryTorque(expectedEntry.EngineSpeed).Value(), 1e-6);
			}
		}

		foreach (var eFc in ExpectedFC_NonDiesel) {
			var fc = iceData.Fuels.First().ConsumptionMap.GetFuelConsumption(eFc[1].SI<NewtonMeter>(), eFc[0].RPMtoRad(), false);
			Assert.AreEqual(eFc[2], fc.Value.ConvertToGrammPerHour().Value, 1e-3);
		}

		Assert.AreEqual(1.05, iceData.Fuels.First().WHTCUrban);
		Assert.AreEqual(1.02, iceData.Fuels.First().WHTCRural);
		Assert.AreEqual(1.0, iceData.Fuels.First().WHTCMotorway);
		Assert.AreEqual(1.005, iceData.Fuels.First().ColdHotCorrectionFactor);
		Assert.AreEqual(1.0, iceData.Fuels.First().CorrectionFactorRegPer);
    }

    [TestCase()]
	public void TestGenericCombustionEngine_Conventional_DualFuel()
	{
		var dao = new GenericCombustionEngineComponentDataAdapter();

		var mockPrimaryVehicle = GetPrimaryVehicleConventional(FuelType.DieselCI, FuelType.NGCI);
		var mission = new Mission() {
			MissionType = MissionType.Urban,
		};
		var iceData = dao.CreateEngineData(mockPrimaryVehicle.Object, 0, mission);

		Assert.NotNull(iceData);
		Assert.AreEqual(1, iceData.Fuels.Count); // dual fuel is simulated as single-fuel with Diesel
		Assert.AreEqual(FuelType.DieselCI, iceData.Fuels.First().FuelData.FuelType);

		var expectedFld = FullLoadCurveReader.Create(InputDataHelper.InputDataAsTableData(ICE_FLD_HDR, ICE_FLD_Data));
		foreach (var (gear, fld) in iceData.FullLoadCurves) {
			foreach (var expectedEntry in expectedFld.FullLoadEntries) {
				Assert.AreEqual(expectedEntry.TorqueFullLoad.Value(), fld.FullLoadStationaryTorque(expectedEntry.EngineSpeed).Value(), 1e-6);
				Assert.AreEqual(expectedEntry.TorqueDrag.Value(), fld.DragLoadStationaryTorque(expectedEntry.EngineSpeed).Value(), 1e-6);
			}
		}

		foreach (var eFc in ExpectedFC_Diesel) {
			var fc = iceData.Fuels.First().ConsumptionMap.GetFuelConsumption(eFc[1].SI<NewtonMeter>(), eFc[0].RPMtoRad(), false);
			Assert.AreEqual(eFc[2], fc.Value.ConvertToGrammPerHour().Value, 1e-3);
		}

		Assert.AreEqual(1.05, iceData.Fuels.First().WHTCUrban);
		Assert.AreEqual(1.02, iceData.Fuels.First().WHTCRural);
		Assert.AreEqual(1.0, iceData.Fuels.First().WHTCMotorway);
		Assert.AreEqual(1.005, iceData.Fuels.First().ColdHotCorrectionFactor);
		Assert.AreEqual(1.0, iceData.Fuels.First().CorrectionFactorRegPer);
    }

    [TestCase()] // this testcase ensures that in case of an SerialHybrid the combustion engine data is correctly set
	public void TestGenericCombustionEngine_SerialHybrid_SingleFuel_Diesel()
	{
		var dao = new GenericCombustionEngineComponentDataAdapter();

		var mockPrimaryVehicle = GetPrimaryVehicle_SerialHybrid(FuelType.DieselCI);
		var mission = new Mission() {
			MissionType = MissionType.Urban,
		};
		var iceData = dao.CreateEngineData(mockPrimaryVehicle.Object, 0, mission);

		Assert.NotNull(iceData);
		Assert.AreEqual(FuelType.DieselCI, iceData.Fuels.First().FuelData.FuelType);
	}

	

	[TestCase()] // this testcase ensures that in case of an IEPC-S the combustion engine data is correctly set
	public void TestGenericCombustionEngine_IEPC_S_SingleFuel_Diesel()
	{
		var dao = new GenericCombustionEngineComponentDataAdapter();

		var mockPrimaryVehicle = GetPrimaryVehicle_IEPC_S(FuelType.DieselCI);
		var mission = new Mission() {
			MissionType = MissionType.Urban,
		};
		var iceData = dao.CreateEngineData(mockPrimaryVehicle.Object, 0, mission);

		Assert.NotNull(iceData);
		Assert.AreEqual(FuelType.DieselCI, iceData.Fuels.First().FuelData.FuelType);
	}

	

	private Mock<IVehicleDeclarationInputData> GetPrimaryVehicleConventional(params FuelType[] fuelType)
	{
		var vehicle = new Mock<IVehicleDeclarationInputData>();
		var comp = new Mock<IVehicleComponentsDeclaration>();
		var eng = GetMockCombustionEngine(fuelType);
		var gbx = GetMockGearbox();


		vehicle.Setup(v => v.Components).Returns(comp.Object);
		comp.Setup(c => c.EngineInputData).Returns(eng.Object);
		comp.Setup(c => c.GearboxInputData).Returns(gbx.Object);

		vehicle.Setup(v => v.TorqueLimits).Returns(new List<ITorqueLimitInputData>());

		return vehicle;
	}

	private static Mock<IEngineDeclarationInputData> GetMockCombustionEngine(params FuelType[] fuelTypes)
	{
		var mode = new Mock<IEngineModeDeclarationInputData>();
		var eng = new Mock<IEngineDeclarationInputData>();
		eng.Setup(e => e.EngineModes).Returns(new[] { mode.Object }.ToList());
		mode.Setup(m => m.IdleSpeed).Returns(600.RPMtoRad());
		eng.Setup(e => e.Displacement).Returns(7.7.SI(Unit.SI.Liter).Cast<CubicMeter>());
		eng.Setup(e => e.RatedPowerDeclared).Returns(220000.SI<Watt>());
		eng.Setup(e => e.RatedSpeedDeclared).Returns(1400.RPMtoRad());
		eng.Setup(e => e.MaxTorqueDeclared).Returns(200.SI<NewtonMeter>());

		mode.Setup(m => m.FullLoadCurve).Returns(InputDataHelper.InputDataAsTableData(ICE_FLD_HDR, ICE_FLD_Data));
		mode.Setup(m => m.Fuels).Returns(fuelTypes.Select(x => {
			var r = new Mock<IEngineFuelDeclarationInputData>();
			r.Setup(y => y.FuelType).Returns(x);
			r.Setup(y => y.WHTCMotorway).Returns(1.0);
			r.Setup(y => y.WHTCRural).Returns(1.1);
			r.Setup(y => y.WHTCUrban).Returns(1.2);
			return r.Object;
		}).ToList());
		return eng;
	}

	private static Mock<IGearboxDeclarationInputData> GetMockGearbox()
	{
		var gbx = new Mock<IGearboxDeclarationInputData>();
		var gearRatios = new double[] {
			6.38, 4.63, 3.44, 2.59, 1.86, 1.35, 1.0, 0.76
		};
		var gears = gearRatios.Select((x, idx) => {
			var gear = new Mock<ITransmissionInputData>();
			gear.Setup(g => g.Ratio).Returns(x);
			gear.Setup(g => g.Gear).Returns(idx + 1);
			return gear.Object;
		}).ToList();
		gbx.Setup(g => g.Type).Returns(GearboxType.AMT);
		gbx.Setup(g => g.Gears).Returns(gears);
		return gbx;
	}

	private Mock<IVehicleDeclarationInputData> GetPrimaryVehicle_SerialHybrid(FuelType fuelType)
	{
		var vehicle = new Mock<IVehicleDeclarationInputData>();
		var comp = new Mock<IVehicleComponentsDeclaration>();
		var eng = GetMockCombustionEngine(fuelType);
		var gbx = GetMockGearbox();


		vehicle.Setup(v => v.Components).Returns(comp.Object);
		comp.Setup(c => c.EngineInputData).Returns(eng.Object);
		comp.Setup(c => c.GearboxInputData).Returns(gbx.Object);

		vehicle.Setup(v => v.TorqueLimits).Returns(new List<ITorqueLimitInputData>());
		vehicle.Setup(v => v.VehicleType).Returns(VectoSimulationJobType.SerialHybridVehicle);
		return vehicle;
    }

	private Mock<IVehicleDeclarationInputData> GetPrimaryVehicle_IEPC_S(FuelType fuelType)
	{
		var vehicle = new Mock<IVehicleDeclarationInputData>();
		var comp = new Mock<IVehicleComponentsDeclaration>();
		var eng = GetMockCombustionEngine(fuelType);

		vehicle.Setup(v => v.Components).Returns(comp.Object);
		comp.Setup(c => c.EngineInputData).Returns(eng.Object);
		
		vehicle.Setup(v => v.TorqueLimits).Returns(new List<ITorqueLimitInputData>());
		vehicle.Setup(v => v.VehicleType).Returns(VectoSimulationJobType.SerialHybridVehicle);

        return vehicle;
    }

	public const string ICE_FLD_HDR = "n [U/min],Mfull [Nm],Mdrag [Nm],<PT1> [s]";

	public static readonly string[] ICE_FLD_Data = new[] {
		"560,1180,-149,0.6",
		"600,1282,-148,0.6",
		"799.9999999,1791,-149,0.6",
		"1000,2300,-160,0.6",
		"1200,2300,-179,0.6",
		"1400,2300,-203,0.6",
		"1599.999999,2079,-235,0.49",
		"1800,1857,-264,0.25",
		"2000.000001,1352,-301,0.25",
		"2100,1100,-320,0.25",
	};

	private static readonly double[][] ExpectedFC_Diesel = new[] {
		new[] { 499.4, -250.5200, 0 },
		new[] { 499.4, -150.5100, 0 },
		new[] { 499.4, 0.0000, 1461.11 },
		new[] { 499.4, 219.2100, 3543.86 },
		new[] { 499.4, 438.4300, 5945.48 },
		new[] { 499.4, 657.6400, 8597.34 },
		new[] { 499.4, 876.8500, 10723.729999999998 },
		new[] { 499.4, 1096.0700, 13072.84 },
		new[] { 499.4, 1315.2800, 15597.93 },
		new[] { 499.4, 1534.4900, 18123.69 },
		new[] { 499.4, 1753.7100, 20649.44 },
		new[] { 499.4, 1972.9200, 23175.2 },
		new[] { 499.4, 2192.1300, 25700.96 },
		new[] { 499.4, 3288.2000, 38551.44 },
		new[] { 600, -248.0000, 0 },
		new[] { 600, -148.0000, 0 },
		new[] { 600, 0.0000, 1461.11 },
		new[] { 600, 217.2100, 3543.86 },
		new[] { 600, 434.4300, 5945.48 },
		new[] { 600, 651.6400, 8597.34 },
		new[] { 600, 868.8500, 10723.729999999998 },
		new[] { 600, 1086.0700, 13072.84 },
		new[] { 600, 1303.2800, 15597.93 },
		new[] { 600, 1520.4900, 18123.69 },
		new[] { 600, 1737.7000, 20649.44 },
		new[] { 600, 1954.9200, 23175.2 },
		new[] { 600, 2172.1300, 25700.96 },
		new[] { 600, 3258.2000, 38551.44 },
		new[] { 756.03, -248.7800, 0 },
		new[] { 756.03, -148.7800, 0 },
		new[] { 756.03, 0.0000, 1774.25 },
		new[] { 756.03, 215.1600, 4293.959999999999 },
		new[] { 756.03, 430.3300, 7339.64 },
		new[] { 756.03, 645.4900, 10267.719999999998 },
		new[] { 756.03, 860.6500, 13233.62 },
		new[] { 756.03, 1075.8200, 16294.569999999998 },
		new[] { 756.03, 1290.9800, 19532.589999999997 },
		new[] { 756.03, 1506.1400, 22748.69 },
		new[] { 756.03, 1721.3100, 26104.64 },
		new[] { 756.03, 1936.4700, 29423.459999999995 },
		new[] { 756.03, 2151.6400, 32747.159999999996 },
		new[] { 756.03, 3227.4500, 49120.729999999996 },
		new[] { 912.07, -255.1600, 0 },
		new[] { 912.07, -155.1600, 0 },
		new[] { 912.07, 0.0000, 2364.5599999999995 },
		new[] { 912.07, 213.8200, 5379.239999999999 },
		new[] { 912.07, 427.6300, 8760.059999999998 },
		new[] { 912.07, 641.4500, 12037.219999999998 },
		new[] { 912.07, 855.2600, 15665.070000000002 },
		new[] { 912.07, 1069.0800, 19376.549999999996 },
		new[] { 912.07, 1282.8900, 23104.4 },
		new[] { 912.07, 1496.7100, 26903.08 },
		new[] { 912.07, 1710.5200, 30859.659999999996 },
		new[] { 912.07, 1924.3400, 34904.71 },
		new[] { 912.07, 2138.1500, 39211.59999999999 },
		new[] { 912.07, 3207.2300, 58817.399999999994 },
		new[] { 1068.1, -266.4700, 0 },
		new[] { 1068.1, -166.4700, 0 },
		new[] { 1068.1, 0.0000, 2972.49 },
		new[] { 1068.1, 212.8600, 6473.02 },
		new[] { 1068.1, 425.7200, 10117.25 },
		new[] { 1068.1, 638.5800, 13873.76 },
		new[] { 1068.1, 851.4400, 18094.789999999997 },
		new[] { 1068.1, 1064.3000, 22416.009999999995 },
		new[] { 1068.1, 1277.1700, 26581.71 },
		new[] { 1068.1, 1490.0300, 30892.16999999999 },
		new[] { 1068.1, 1702.8900, 35355.62999999999 },
		new[] { 1068.1, 1915.7500, 40104.08999999999 },
		new[] { 1068.1, 2128.6100, 45227.9 },
		new[] { 1068.1, 3192.9100, 67841.86 },
		new[] { 1224.1000000000001, -281.8900, 0 },
		new[] { 1224.1000000000001, -181.8900, 0 },
		new[] { 1224.1000000000001, 0.0000, 3301.97 },
		new[] { 1224.1000000000001, 212.1500, 7201.929999999999 },
		new[] { 1224.1000000000001, 424.3000, 11305.79 },
		new[] { 1224.1000000000001, 636.4500, 15743.28 },
		new[] { 1224.1000000000001, 848.6000, 20609.92 },
		new[] { 1224.1000000000001, 1060.7500, 25521.589999999997 },
		new[] { 1224.1000000000001, 1272.9000, 30266.389999999992 },
		new[] { 1224.1000000000001, 1485.0500, 35056.74 },
		new[] { 1224.1000000000001, 1697.2000, 40084.369999999995 },
		new[] { 1224.1000000000001, 1909.3500, 45562.56 },
		new[] { 1224.1000000000001, 2121.5000, 50927.5 },
		new[] { 1224.1000000000001, 3182.2500, 76391.25 },
		new[] { 1395.56, -302.4700, 0 },
		new[] { 1395.56, -202.4700, 0 },
		new[] { 1395.56, 0.0000, 4108.069999999999 },
		new[] { 1395.56, 211.5500, 8272.439999999999 },
		new[] { 1395.56, 423.1000, 13101.32 },
		new[] { 1395.56, 634.6600, 18152.57 },
		new[] { 1395.56, 846.2100, 23279.16 },
		new[] { 1395.56, 1057.7600, 28653.009999999995 },
		new[] { 1395.56, 1269.3100, 34235.35 },
		new[] { 1395.56, 1480.8600, 39831.37 },
		new[] { 1395.56, 1692.4200, 46006.67999999999 },
		new[] { 1395.56, 1903.9700, 52561.719999999994 },
		new[] { 1395.56, 2115.5200, 58444.46 },
		new[] { 1395.56, 3173.2800, 87666.69 },
		new[] { 1526.3499999999997, -323.2200, 0 },
		new[] { 1526.3499999999997, -223.2200, 0 },
		new[] { 1526.3499999999997, 0.0000, 4691.6 },
		new[] { 1526.3499999999997, 211.1900, 9204.07 },
		new[] { 1526.3499999999997, 422.3700, 14476.549999999997 },
		new[] { 1526.3499999999997, 633.5600, 19957.94 },
		new[] { 1526.3499999999997, 844.7400, 25593.19 },
		new[] { 1526.3499999999997, 1055.9300, 31371.539999999994 },
		new[] { 1526.3499999999997, 1267.1200, 37417.74999999999 },
		new[] { 1526.3499999999997, 1478.3000, 43634.799999999996 },
		new[] { 1526.3499999999997, 1689.4900, 50533.65 },
		new[] { 1526.3499999999997, 1900.6800, 57687.659999999996 },
		new[] { 1526.3499999999997, 2111.8600, 64273.76999999999 },
		new[] { 1526.3499999999997, 3167.7900, 96410.65999999999 },
		new[] { 1657.2299999999996, -343.3000, 0 },
		new[] { 1657.2299999999996, -243.3000, 0 },
		new[] { 1657.2299999999996, 0.0000, 5268.99 },
		new[] { 1657.2299999999996, 210.8800, 10214.629999999997 },
		new[] { 1657.2299999999996, 421.7600, 15869.059999999998 },
		new[] { 1657.2299999999996, 632.6300, 21778.57 },
		new[] { 1657.2299999999996, 843.5100, 28077.679999999997 },
		new[] { 1657.2299999999996, 1054.3900, 34281.77 },
		new[] { 1657.2299999999996, 1265.2700, 40727.95 },
		new[] { 1657.2299999999996, 1476.1500, 47567.03 },
		new[] { 1657.2299999999996, 1687.0200, 55094.41 },
		new[] { 1657.2299999999996, 1897.9000, 62745.34999999999 },
		new[] { 1657.2299999999996, 2108.7800, 70172.88999999998 },
		new[] { 1657.2299999999996, 3163.1700, 105259.33 },
		new[] { 1788.12, -362.2800, 0 },
		new[] { 1788.12, -262.2800, 0 },
		new[] { 1788.12, 0.0000, 6019.48 },
		new[] { 1788.12, 210.6100, 11552.74 },
		new[] { 1788.12, 421.2300, 17504.28 },
		new[] { 1788.12, 631.8400, 24034.199999999997 },
		new[] { 1788.12, 842.4600, 30988.279999999995 },
		new[] { 1788.12, 1053.0700, 37536.60999999999 },
		new[] { 1788.12, 1263.6900, 44681.91 },
		new[] { 1788.12, 1474.3000, 52146.399999999994 },
		new[] { 1788.12, 1684.9200, 60090.88999999999 },
		new[] { 1788.12, 1895.5300, 68349.96 },
		new[] { 1788.12, 2106.1500, 76491.32999999999 },
		new[] { 1788.12, 3159.2200, 114736.98999999999 },
		new[] { 1918.91, -386.0000, 0 },
		new[] { 1918.91, -286.0000, 0 },
		new[] { 1918.91, 0.0000, 7214.909999999999 },
		new[] { 1918.91, 210.3900, 13162.519999999999 },
		new[] { 1918.91, 420.7800, 19366.88 },
		new[] { 1918.91, 631.1600, 26310.379999999997 },
		new[] { 1918.91, 841.5500, 33887.78 },
		new[] { 1918.91, 1051.9400, 41340.68 },
		new[] { 1918.91, 1262.3300, 49136.52 },
		new[] { 1918.91, 1472.7100, 57148.50999999999 },
		new[] { 1918.91, 1683.1000, 65418.63 },
		new[] { 1918.91, 1893.4900, 73982.72 },
		new[] { 1918.91, 2103.8800, 82503.21 },
		new[] { 1918.91, 3155.8100, 123754.80999999998 },
		new[] { 2049.7, -410.4400, 0 },
		new[] { 2049.7, -310.4400, 0 },
		new[] { 2049.7, 0.0000, 8410.34 },
		new[] { 2049.7, 210.1900, 14772.299999999997 },
		new[] { 2049.7, 420.3800, 21229.47 },
		new[] { 2049.7, 630.5700, 28586.559999999998 },
		new[] { 2049.7, 840.7600, 36787.28 },
		new[] { 2049.7, 1050.9500, 45144.76 },
		new[] { 2049.7, 1261.1400, 53591.13 },
		new[] { 2049.7, 1471.3300, 62150.62 },
		new[] { 2049.7, 1681.5200, 70746.37 },
		new[] { 2049.7, 1891.7100, 79615.47999999998 },
		new[] { 2049.7, 2101.9000, 88515.09 },
		new[] { 2049.7, 3152.8400, 132772.63 },
		new[] { 2180.49, -435.2900, 0 },
		new[] { 2180.49, -335.2900, 0 },
		new[] { 2180.49, 0.0000, 9605.77 },
		new[] { 2180.49, 210.0200, 16382.079999999996 },
		new[] { 2180.49, 420.0300, 23092.069999999996 },
		new[] { 2180.49, 630.0500, 30862.750000000004 },
		new[] { 2180.49, 840.0600, 39686.76999999999 },
		new[] { 2180.49, 1050.0800, 48948.829999999994 },
		new[] { 2180.49, 1260.0900, 58045.74999999999 },
		new[] { 2180.49, 1470.1100, 67152.73 },
		new[] { 2180.49, 1680.1200, 76074.11 },
		new[] { 2180.49, 1890.1400, 85248.25 },
		new[] { 2180.49, 2100.1500, 94526.96999999999 },
		new[] { 2180.49, 3150.2300, 141790.46 },
	};

	private static readonly double[][] ExpectedFC_NonDiesel = new[] {
		new[] { 499.4, -250.5200, 0 },
		new[] { 499.4, -150.5100, 0 },
		new[] { 499.4, 0.0000, 2245.11 },
		new[] { 499.4, 365.3600, 8280.589999999998 },
		new[] { 499.4, 730.7100, 12479.98 },
		new[] { 499.4, 1096.0700, 17996.84 },
		new[] { 499.4, 1461.4200, 23265.58 },
		new[] { 499.4, 1826.7800, 25643.37 },
		new[] { 499.4, 2192.1300, 30347.72 },
		new[] { 499.4, 2557.4900, 35052.06 },
		new[] { 499.4, 2922.8500, 39756.41 },
		new[] { 499.4, 3288.2000, 44460.76 },
		new[] { 499.4, 3653.5600, 49165.11 },
		new[] { 499.4, 5480.3400, 73747.66 },
		new[] { 649.19, -248.2500, 0 },
		new[] { 649.19, -148.2500, 0 },
		new[] { 649.19, 0.0000, 3129.16 },
		new[] { 649.19, 350.8000, 10000.819999999998 },
		new[] { 649.19, 701.6100, 14756.479999999998 },
		new[] { 649.19, 1052.4100, 20916.24 },
		new[] { 649.19, 1403.2100, 27154.38 },
		new[] { 649.19, 1754.0200, 30070.659999999996 },
		new[] { 649.19, 2104.8200, 35702.38 },
		new[] { 649.19, 2455.6200, 41334.1 },
		new[] { 649.19, 2806.4300, 46965.81 },
		new[] { 649.19, 3157.2300, 52597.52999999999 },
		new[] { 649.19, 3508.0400, 58229.25 },
		new[] { 649.19, 5262.0500, 87343.87 },
		new[] { 798.99, -248.9900, 0 },
		new[] { 798.99, -148.9900, 0 },
		new[] { 798.99, 0.0000, 3865.5400000000004 },
		new[] { 798.99, 341.7100, 11414.839999999998 },
		new[] { 798.99, 683.4200, 18317.83 },
		new[] { 798.99, 1025.1200, 24323.54 },
		new[] { 798.99, 1366.8300, 29723.75 },
		new[] { 798.99, 1708.5400, 36280.24 },
		new[] { 798.99, 2050.2500, 42814.79 },
		new[] { 798.99, 2391.9500, 49349.35 },
		new[] { 798.99, 2733.6600, 55883.899999999994 },
		new[] { 798.99, 3075.3700, 62418.45999999999 },
		new[] { 798.99, 3417.0800, 68953.02 },
		new[] { 798.99, 5125.6200, 103429.51999999999 },
		new[] { 948.78, -257.1800, 0 },
		new[] { 948.78, -157.1800, 0 },
		new[] { 948.78, 0.0000, 3940.7499999999995 },
		new[] { 948.78, 335.4800, 12292.859999999999 },
		new[] { 948.78, 670.9700, 19602.2 },
		new[] { 948.78, 1006.4500, 27171.29 },
		new[] { 948.78, 1341.9400, 33272.06 },
		new[] { 948.78, 1677.4200, 39164.009999999995 },
		new[] { 948.78, 2012.9000, 46846.20999999999 },
		new[] { 948.78, 2348.3900, 54473.439999999995 },
		new[] { 948.78, 2683.8700, 62100.659999999996 },
		new[] { 948.78, 3019.3600, 69727.89 },
		new[] { 948.78, 3354.8400, 77355.11 },
		new[] { 948.78, 5032.2600, 116032.66999999998 },
		new[] { 1098.54, -269.3600, 0 },
		new[] { 1098.54, -169.3600, 0 },
		new[] { 1098.54, 0.0000, 3685.62 },
		new[] { 1098.54, 330.9600, 12721.3 },
		new[] { 1098.54, 661.9200, 20432.8 },
		new[] { 1098.54, 992.8800, 28529.909999999996 },
		new[] { 1098.54, 1323.8300, 36765.09999999999 },
		new[] { 1098.54, 1654.7900, 43543.95 },
		new[] { 1098.54, 1985.7500, 49761 },
		new[] { 1098.54, 2316.7100, 57996.81999999999 },
		new[] { 1098.54, 2647.6700, 66232.64 },
		new[] { 1098.54, 2978.6300, 74468.46 },
		new[] { 1098.54, 3309.5800, 82704.27999999998 },
		new[] { 1098.54, 4964.3800, 124056.42999999998 },
		new[] { 1263.14, -286.5800, 0 },
		new[] { 1263.14, -186.5800, 0 },
		new[] { 1263.14, 0.0000, 4330.539999999999 },
		new[] { 1263.14, 327.2200, 14633.129999999997 },
		new[] { 1263.14, 654.4400, 23555.82 },
		new[] { 1263.14, 981.6700, 32737.169999999995 },
		new[] { 1263.14, 1308.8900, 42257.03 },
		new[] { 1263.14, 1636.1100, 49879.19 },
		new[] { 1263.14, 1963.3300, 56690.23999999999 },
		new[] { 1263.14, 2290.5600, 65861.23 },
		new[] { 1263.14, 2617.7800, 75032.22 },
		new[] { 1263.14, 2945.0000, 84203.20999999999 },
		new[] { 1263.14, 3272.2200, 93374.19999999998 },
		new[] { 1263.14, 4908.3300, 140061.29999999996 },
		new[] { 1388.7, -301.6400, 0 },
		new[] { 1388.7, -201.6400, 0 },
		new[] { 1388.7, 0.0000, 5063.51 },
		new[] { 1388.7, 324.9700, 16309.159999999998 },
		new[] { 1388.7, 649.9400, 26109.799999999996 },
		new[] { 1388.7, 974.9000, 36129.69999999999 },
		new[] { 1388.7, 1299.8700, 46705.46 },
		new[] { 1388.7, 1624.8400, 54833.73 },
		new[] { 1388.7, 1949.8100, 62022.01 },
		new[] { 1388.7, 2274.7700, 71833.35999999999 },
		new[] { 1388.7, 2599.7400, 81644.71 },
		new[] { 1388.7, 2924.7100, 91456.04999999999 },
		new[] { 1388.7, 3249.6800, 101267.4 },
		new[] { 1388.7, 4874.5200, 151901.10999999996 },
		new[] { 1514.3499999999997, -321.3000, 0 },
		new[] { 1514.3499999999997, -221.3000, 0 },
		new[] { 1514.3499999999997, 0.0000, 5689.4 },
		new[] { 1514.3499999999997, 323.0900, 18124.66 },
		new[] { 1514.3499999999997, 646.1700, 28847.149999999998 },
		new[] { 1514.3499999999997, 969.2600, 39797.96 },
		new[] { 1514.3499999999997, 1292.3400, 51452.31999999999 },
		new[] { 1514.3499999999997, 1615.4300, 60578.53999999999 },
		new[] { 1514.3499999999997, 1938.5200, 68337.86999999998 },
		new[] { 1514.3499999999997, 2261.6000, 79085.98999999999 },
		new[] { 1514.3499999999997, 2584.6900, 89834.09999999999 },
		new[] { 1514.3499999999997, 2907.7700, 100582.21 },
		new[] { 1514.3499999999997, 3230.8600, 111330.32 },
		new[] { 1514.3499999999997, 4846.2900, 166995.48 },
		new[] { 1639.99, -340.8000, 0 },
		new[] { 1639.99, -240.8000, 0 },
		new[] { 1639.99, 0.0000, 6521.979999999999 },
		new[] { 1639.99, 321.4900, 19952.05 },
		new[] { 1639.99, 642.9800, 31522.26 },
		new[] { 1639.99, 964.4800, 43594.55 },
		new[] { 1639.99, 1285.9700, 56102.81 },
		new[] { 1639.99, 1607.4600, 65738.62999999999 },
		new[] { 1639.99, 1928.9500, 74210.63 },
		new[] { 1639.99, 2250.4500, 85824.67 },
		new[] { 1639.99, 2571.9400, 97438.71 },
		new[] { 1639.99, 2893.4300, 109052.74 },
		new[] { 1639.99, 3214.9200, 120666.77999999998 },
		new[] { 1639.99, 4822.3900, 181000.17 },
		new[] { 1765.55, -359.0000, 0 },
		new[] { 1765.55, -259.0000, 0 },
		new[] { 1765.55, 0.0000, 7891.089999999999 },
		new[] { 1765.55, 320.1300, 22022.72 },
		new[] { 1765.55, 640.2500, 34497.11 },
		new[] { 1765.55, 960.3800, 47499.61 },
		new[] { 1765.55, 1280.5100, 60528.52999999999 },
		new[] { 1765.55, 1600.6300, 70488.44999999998 },
		new[] { 1765.55, 1920.7600, 79534.97 },
		new[] { 1765.55, 2240.8900, 91996.67 },
		new[] { 1765.55, 2561.0100, 104458.38 },
		new[] { 1765.55, 2881.1400, 116920.08 },
		new[] { 1765.55, 3201.2700, 129381.78 },
		new[] { 1765.55, 4801.9000, 194072.66 },
		new[] { 1891.2, -380.8700, 0 },
		new[] { 1891.2, -280.8700, 0 },
		new[] { 1891.2, 0.0000, 9260.21 },
		new[] { 1891.2, 318.9400, 24093.38 },
		new[] { 1891.2, 637.8800, 37471.97 },
		new[] { 1891.2, 956.8200, 51404.659999999996 },
		new[] { 1891.2, 1275.7700, 64954.23999999999 },
		new[] { 1891.2, 1594.7100, 75238.28 },
		new[] { 1891.2, 1913.6500, 84859.32 },
		new[] { 1891.2, 2232.5900, 98168.68 },
		new[] { 1891.2, 2551.5300, 111478.03999999998 },
		new[] { 1891.2, 2870.4700, 124787.40999999999 },
		new[] { 1891.2, 3189.4100, 138096.77 },
		new[] { 1891.2, 4784.1200, 207145.15999999997 },
		new[] { 2016.85, -404.2000, 0 },
		new[] { 2016.85, -304.2000, 0 },
		new[] { 2016.85, 0.0000, 10629.329999999998 },
		new[] { 2016.85, 317.9000, 26164.039999999997 },
		new[] { 2016.85, 635.8100, 40446.81999999999 },
		new[] { 2016.85, 953.7100, 55309.719999999994 },
		new[] { 2016.85, 1271.6200, 69379.96 },
		new[] { 2016.85, 1589.5200, 79988.1 },
		new[] { 2016.85, 1907.4200, 90183.65999999999 },
		new[] { 2016.85, 2225.3300, 104340.69 },
		new[] { 2016.85, 2543.2300, 118497.70999999998 },
		new[] { 2016.85, 2861.1300, 132654.74 },
		new[] { 2016.85, 3179.0400, 146811.76999999996 },
		new[] { 2016.85, 4768.5600, 220217.65999999997 },
		new[] { 2142.5, -428.0800, 0 },
		new[] { 2142.5, -328.0800, 0 },
		new[] { 2142.5, 0.0000, 10671.829999999998 },
		new[] { 2142.5, 316.9900, 28228.559999999998 },
		new[] { 2142.5, 633.9800, 43929.49 },
		new[] { 2142.5, 950.9600, 60805.41999999999 },
		new[] { 2142.5, 1267.9500, 75828.77999999998 },
		new[] { 2142.5, 1584.9400, 84152.33 },
		new[] { 2142.5, 1901.9300, 98263.27 },
		new[] { 2142.5, 2218.9200, 110512.68999999999 },
		new[] { 2142.5, 2535.9000, 125517.37999999999 },
		new[] { 2142.5, 2852.8900, 140522.07999999996 },
		new[] { 2142.5, 3169.8800, 155526.77 },
		new[] { 2142.5, 4754.8200, 233290.14999999997 },
		new[] { 2268.14, -451.9500, 0 },
		new[] { 2268.14, -351.9500, 0 },
		new[] { 2268.14, 0.0000, 13719.989999999998 },
		new[] { 2268.14, 316.1700, 30158.419999999995 },
		new[] { 2268.14, 632.3500, 47346.21 },
		new[] { 2268.14, 948.5200, 64595.58999999999 },
		new[] { 2268.14, 1264.6900, 75749.42 },
		new[] { 2268.14, 1580.8700, 88760.61999999998 },
		new[] { 2268.14, 1897.0400, 118431.40999999999 },
		new[] { 2268.14, 2213.2200, 127292.39999999998 },
		new[] { 2268.14, 2529.3900, 144585.88 },
		new[] { 2268.14, 2845.5600, 161879.36 },
		new[] { 2268.14, 3161.7400, 179172.84 },
		new[] { 2268.14, 4742.6000, 268759.25 },
	};

}