using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.Reader.ComponentData;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Engine;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Gearbox;
using TUGraz.VectoCore.Tests.Utils;

namespace TUGraz.Vecto.UnitTests.TestCases.Components.HybridStrategy;

public static class GearListHelpers
{
	public static int GetDistance(this GearList gearList, GearshiftPosition a, GearshiftPosition b)
	{
		GearshiftPosition? instanceA = null;
		GearshiftPosition? instanceB = null;
		
		foreach (var gear in gearList) {
			if (gear.Equals(a)) {
				if (instanceA != null) {
					throw new Exception("Duplicate");
				}
				instanceA = gear;
			}

			if (gear.Equals(b)) {
				if (instanceB != null) {
					throw new Exception("Duplicate");
				}
				instanceB = gear;
			}
		}

		if (instanceA == null) {
			throw new Exception($"Gear {a} not found");
		}

		if (instanceB == null) {
			throw new Exception($"Gear {b} not found");
		}

		return gearList.Distance(instanceA, instanceB);
	}	

}

public partial class HybridStrategyBase
{
	
	
	
	
	#region RunData

	protected VectoRunData GetRunData(PowertrainPosition p, GearboxType gbxType)
	{
		var runData = new VectoRunData();

		runData.ElectricMachinesSinglePwt = GetElectricMachinesData(p);
		runData.HybridStrategyParameters = GetHybridStrategyParameters();
		runData.GearboxSinglePwt = GetGearboxData(gbxType);
		runData.EngineData = GetEngineData();
		runData.GearshiftParametersSinglePwt = GetGearshiftParameters();
		runData.Cycle = GetCycle();
		runData.ElectricAuxDemand = 0.SI<Watt>();
		runData.VehicleData = GetVehicleData();
		runData.AxleGearSinglePwt = new() {
			AxleGear = new() { Ratio = 2.64 }
		};

		var lorryStartStop = DeclarationData.Driver.GetEngineStopStartLorry();
		runData.DriverData = new() {
			EngineStopStart = new() {
				EngineOffStandStillActivationDelay = lorryStartStop.ActivationDelay
			}
		};
		return runData;
	}

	private VehicleData GetVehicleData()
	{
		return new() {
			ADAS = new() {
				EngineStopStart = true
			},
			DynamicTyreRadius = 0.4922.SI<Meter>(),
		};
	}

	private IDrivingCycleData GetCycle()
	{
		var cycleType = CycleType.DistanceBased;
		var cycle = new DrivingCycleData() {
			CycleType = cycleType,
			Entries = new List<DrivingCycleData.DrivingCycleEntry>() {
				new DrivingCycleData.DrivingCycleEntry() {
					Distance = 0.SI<Meter>(),
					RoadGradient = 0.SI<Radian>()
				}
			},
		};
		return cycle;
	}

	private ShiftStrategyParameters GetGearshiftParameters()
	{
		return new ShiftStrategyParameters() {
			AllowedGearRangeFC = 2,
			TimeBetweenGearshifts = 5.SI<Second>(),
			DownshiftAfterUpshiftDelay = 6.SI<Second>(),
			UpshiftAfterDownshiftDelay = 6.SI<Second>(),
			RatioEarlyDownshiftFC = 9.09,
			RatioEarlyUpshiftFC = 9.09,
			StartSpeed = DeclarationData.GearboxTCU.StartSpeed,
		};
	}

	private CombustionEngineData GetEngineData()
	{
		var engineData = new CombustionEngineData();
		engineData.IdleSpeed = 600.RPMtoRad();
		engineData.Inertia = 5.SI<KilogramSquareMeter>();
		engineData.EngineStartTime = 1.SI<Second>();
		engineData.FullLoadCurves = GetFullLoadCurves();
		engineData.Fuels = GetFuels();
		return engineData;
	}

	private List<CombustionEngineFuelData> GetFuels()
	{
		List<CombustionEngineFuelData> fuels = new List<CombustionEngineFuelData>() {
			new CombustionEngineFuelData() {
				ColdHotCorrectionFactor = 1,
				ConsumptionMap = FuelConsumptionMapReader.Create(InputDataHelper.InputDataAsTableData(
					EngineFcMapHdr,
					EngineFcMapData)),
				FuelData = FuelData.Diesel,
			}
		};
		return fuels;
	}

	private Dictionary<uint, EngineFullLoadCurve> GetFullLoadCurves()
	{
		var fld = new EngineFullLoadCurve(new List<EngineFullLoadCurve.FullLoadCurveEntry>() {
			new() {
				EngineSpeed = 600.RPMtoRad(),
				TorqueDrag = -20.SI<NewtonMeter>(),
				TorqueFullLoad = 800.SI<NewtonMeter>()
			},
			new() {
				EngineSpeed = 2000.RPMtoRad(),
				TorqueDrag = -20.SI<NewtonMeter>(),
				TorqueFullLoad = 800.SI<NewtonMeter>()
			}
		}, DeclarationData.PT1);

		return new Dictionary<uint, EngineFullLoadCurve>() {
			{ 0, fld }
		};
	}

	private GearboxData GetGearboxData(GearboxType gbxType)
	{
		var gearboxData = new GearboxData();

		gearboxData.Type = gbxType;
		gearboxData.TractionInterruption = gearboxData.Type.AutomaticTransmission() ? 0.SI<Second>() : 1.SI<Second>();

		var gearRatios = new double[] {
			14.93,
			11.64,
			9.02,
			7.04,
			5.64,
			4.4,
			3.39,
			2.65,
			2.05,
			1.6,
			1.28,
			1
		};
		gearboxData.Gears = new Dictionary<uint, GearData>() { };


		
		foreach (var (r, i) in gearRatios.Select((value, index) => (value, (uint)index + 1))) {
			// var shiftPolygon = 	DeclarationData.Gearbox.ComputeShiftPolygon(
			// 	gbxType, i, engine?.FullLoadCurves[i + 1],
			// 	gearsInput, engine,
			// 	axlegearRatio, dynamicTyreRadius, runData.ElectricMachinesData?.FirstOrDefault()?.Item2);
			gearboxData.Gears[i] = new GearData() {
				Ratio = r,
				LossMap = TransmissionLossMapReader.Create(0.96, r, $"Gear {i}"),
				// ShiftPolygon = 
			};
			
		}

		return gearboxData;
	}

	private HybridStrategyParameters GetHybridStrategyParameters()
	{
		var parameters = new HybridStrategyParameters() {
			MinICEOnTime = 10.SI<Second>(),
			AuxReserveTime = 0.SI<Second>(),
		};
		return parameters;
	}

	private IList<Tuple<PowertrainPosition, ElectricMotorData>> GetElectricMachinesData(PowertrainPosition pos)
	{
		Assert.True(pos.IsParallelHybrid());
		var electricMotorData = new ElectricMotorData();

		return new List<Tuple<PowertrainPosition, ElectricMotorData>>() {
			new(pos, electricMotorData)
		};
	}

	#region maps

	private const string EngineFcMapHdr = "engine speed [rpm], torque [Nm], fuel consumption [g/h]";

	private static readonly string[] EngineFcMapData = new[] {
		"500,-235.5,0",
		"500,-135.5,0",
		"500,0,1355",
		"500,213.4,3412.291",
		"500,426.8,5830.1",
		"500,640.2,8316.426",
		"500,853.6,10439.87",
		"500,1067,12823.69",
		"500,1188,14228.79",
		"500,1401.4,16628.66",
		"600,-238,0",
		"600,-138,0",
		"600,0,1355",
		"600,213.4,3412.291",
		"600,426.8,5830.1",
		"600,640.2,8316.426",
		"600,853.6,10439.87",
		"600,1067,12823.69",
		"600,1188,14228.79",
		"600,1401.4,16628.66",
		"751,-241.775,0",
		"751,-141.775,0",
		"750.9,0,1649.255",
		"750.9,213.4,4157.795",
		"750.9,426.8,7149.494",
		"750.9,640.2,10037.08",
		"750.9,853.6,12957.07",
		"750.9,1067,16055.22",
		"750.9,1280.4,19231.36",
		"750.9,1493.8,22400.17",
		"750.9,1544.879,23213.92",
		"751,1758.279,26392.93",
		"902,-247.59,0",
		"902,-147.59,0",
		"901.8,0,2210.735",
		"901.8,213.4,5204.867",
		"901.8,426.8,8515.462",
		"901.8,640.2,11804.75",
		"901.8,853.6,15410.55",
		"901.8,1067,19081.7",
		"901.8,1280.4,22742.96",
		"901.8,1493.8,26543.87",
		"901.8,1707.2,30534.68",
		"901.8,1901.757,34352.75",
		"902,2115.157,38403.27",
		"1053,-255.445,0",
		"1053,-155.445,0",
		"1052.7,0,2768.035",
		"1052.7,213.4,6228.407",
		"1052.7,426.8,9836.041",
		"1052.7,640.2,13624.5",
		"1052.7,853.6,17854.95",
		"1052.7,1067,22072.71",
		"1052.7,1280.4,26161.13",
		"1052.7,1493.8,30525.55",
		"1052.7,1707.2,35019.18",
		"1052.7,1920.6,39913.3",
		"1052.7,2134,45438.16",
		"1053,2347.4,50542.53",
		"1204,-265.44,0",
		"1203.6,0,3086.704",
		"1203.6,213.4,6943.027",
		"1203.6,426.8,11040.37",
		"1203.6,640.2,15504.65",
		"1203.6,853.6,20335.89",
		"1203.6,1067,25176.6",
		"1203.6,1280.4,29782.22",
		"1203.6,1493.8,34642.24",
		"1203.6,1707.2,39786.14",
		"1203.6,1920.6,45254.8",
		"1203.6,2134,51129.03",
		"1204,2347.4,56732.88",
		"1367,-283.37,0",
		"1367,-183.37,0",
		"1367.1,0,3845.344",
		"1367.1,213.4,7981.742",
		"1367.1,426.8,12796.69",
		"1367.1,640.2,17789.2",
		"1367.1,853.6,22854.21",
		"1367.1,1067,28302.84",
		"1367.1,1280.4,33739.91",
		"1367.1,1493.8,39393.87",
		"1367.1,1707.2,45836.33",
		"1367.1,1920.6,52078.71",
		"1367.1,2134,58296.41",
		"1367,2347.4,64530.56",
		"1490,-300.5,0",
		"1490,-200.5,0",
		"1489.6,0,4373.424",
		"1489.6,213.4,8861.484",
		"1489.6,426.8,14090.86",
		"1489.6,640.2,19518.29",
		"1489.6,853.6,25092.8",
		"1489.6,1067,30873.69",
		"1489.6,1280.4,36865.42",
		"1489.6,1493.8,43095.57",
		"1489.6,1707.2,50249.81",
		"1489.6,1920.6,57035.25",
		"1489.6,2041.712,60609.5",
		"1490,2255.112,67311.83",
		"1612,-318.62,0",
		"1612,-218.62,0",
		"1612.2,0,4904.015",
		"1612.2,213.4,9810.482",
		"1612.2,426.8,15403.9",
		"1612.2,640.2,21301.35",
		"1612.2,853.6,27492.32",
		"1612.2,1067,33580.96",
		"1612.2,1280.4,40114.61",
		"1612.2,1493.8,46914.77",
		"1612.2,1707.2,54666.14",
		"1612.2,1915.434,61862.91",
		"1612,2128.834,69491.99",
		"1735,-335.225,0",
		"1735,-235.225,0",
		"1734.7,0,5586.953",
		"1734.7,213.4,11041.15",
		"1734.7,426.8,16949.24",
		"1734.7,640.2,23500.23",
		"1734.7,853.6,30159.59",
		"1734.7,1067,36741.18",
		"1734.7,1280.4,43923.85",
		"1734.7,1493.8,51295.21",
		"1734.7,1707.2,59469.31",
		"1734.7,1789.259,62731.31",
		"1735,2002.659,70935.23",
		"1857,-353.69,0",
		"1857,-253.69,0",
		"1857.3,0,6673.839",
		"1857.3,213.4,12518.56",
		"1857.3,426.8,18687.88",
		"1857.3,640.2,25652.39",
		"1857.3,853.6,33003.08",
		"1857.3,1067,40438.09",
		"1857.3,1280.4,48117.52",
		"1857.3,1493.8,55848.59",
		"1857.3,1587.631,59434.17",
		"1857,1801.031,67215.39",
		"1957,-370.69,0",
		"1957,-270.69,0",
		"1957.3,0,6673.839",
		"1957.3,213.4,12518.56",
		"1957.3,426.8,18687.88",
		"1957.3,640.2,25652.39",
		"1957.3,853.6,33003.08",
		"1957.3,1067,40438.09",
		"1957.3,1280.4,48117.52",
		"1957.3,1493.8,55848.59",
		"1957.3,1587.631,59434.17",
		"1957,1801.031,67215.39",
	};

	#endregion maps

	#endregion
}