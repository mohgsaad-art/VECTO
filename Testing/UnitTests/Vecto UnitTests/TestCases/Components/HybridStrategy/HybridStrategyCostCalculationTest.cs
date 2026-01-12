using Moq;
using NUnit.Framework;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.Reader.ComponentData;
using TUGraz.VectoCore.Models.Connector.Ports.Impl;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.Simulation.DataBus;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Engine;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Gearbox;
using TUGraz.VectoCore.Models.SimulationComponent.Impl;
using TUGraz.VectoCore.Models.SimulationComponent.Strategies;

namespace TUGraz.Vecto.UnitTests.TestCases.Components.HybridStrategy;

public class HybridStrategyCostCalculationTest
{

	
	public class HybridStrategyCalculateCost : AbstractHybridStrategy
	{
		public HybridStrategyCalculateCost(VectoRunData runData, IVehicleContainer vehicleContainer) : base(runData, vehicleContainer) { }

		#region Overrides of AbstractHybridStrategy

		protected override IResponse RequestDryRun(Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity,
			GearshiftPosition nextGear, HybridStrategyResponse cfg)
		{
			throw new NotImplementedException();
		}

		protected override void CheckGearshiftLimits(HybridResultEntry tmp, IResponse resp)
		{
			// do nothing here for now... (used in cost calculation!)
		}

		#endregion

		public void CostCalculation(IResponse resp, Second dt, HybridResultEntry tmp, bool allowIceOff, bool dryRun)
		{
			CalculateCosts(resp, dt, tmp, allowIceOff, dryRun);
		}
	}

	public record CostCalculationData
	{
		public ModelParams ModelParams { get; } = new ModelParams();

		public PowertrainInfo PowertrainInfo { get; } = new PowertrainInfo();

		public ResponseValues Response { get; } = new ResponseValues();
		
		public Second dt { get; } = 1.SI<Second>();
    }

	public record ModelParams
	{
		public PerSecond EngineIdleSpeed { get; } = 600.RPMtoRad();
		public KilogramSquareMeter Inertia { get; } = 2.5.SI<KilogramSquareMeter>();
		public Second EngineStartTime { get; } = 2.SI<Second>();
		public Second HybridStrategyMinIceOnTime { get; } = 10.SI<Second>();
		public int GearshiftParamsAllowedGearRange { get; } = 2;
		public uint GearboxNumGears { get; } = 6;
	}

	public record PowertrainInfo
	{
		public bool CombustionEngineOn { get; } = true;
		public PerSecond EngineN95hSpeed { get; } = 2200.RPMtoRad();
		public DrivingAction DrivingAction { get; } = DrivingAction.Accelerate;
		public WattSecond BatteryStoredEnergy { get; } = (600.SI<Volt>() * 90.SI<AmpereSecond>()).Cast<WattSecond>();
		public double BatterySoC { get; } = 0.5;
		public GearshiftPosition GearboxCurrentGear { get; } = new GearshiftPosition(3);
    }

	public record ResponseValues
	{
		public GearshiftPosition ResponseGear { get; } = new GearshiftPosition(3);
		public NewtonMeter EngineDynamicFullLoadTorque { get; } = 1500.SI<NewtonMeter>();
		public Watt EngineDynamicFulloadPower { get; } = 160000.SI<Watt>();
		public Watt EngineDragPower { get; } = 5000.SI<Watt>();
		public NewtonMeter EngineTotalTorqueDemand { get; } = 800.SI<NewtonMeter>();
		public PerSecond EngineSpeed { get; } = 1200.RPMtoRad();
		public Watt REESSPowerDemand { get; } = 40000.SI<Watt>();
		public Watt REESSResponsePowerDemand { get; } = 38000.SI<Watt>();
		public Watt REESSResponseMaxDischargePower { get; } = 500000.SI<Watt>();
		public Watt REESSResponseMaxChargePower { get; } = 500000.SI<Watt>();
    }

	[TestCase()]
	public void TestCostCalculation()
	{
		var testData = new CostCalculationData();

		var container = CreateContainer(testData.PowertrainInfo);

		var runData = GetVectoRunData(testData.ModelParams);

		var dut = new HybridStrategyCalculateCost(runData, container.Object);

		var response = CreateResponse(testData.Response);
		var dt = testData.dt;
		var entry = new HybridResultEntry();
		dut.CostCalculation(response.Object, dt, entry, false, false);

		
	}

	private static Mock<IVehicleContainer> CreateContainer(PowertrainInfo powertrainInfo)
	{
		var container = new Mock<IVehicleContainer>();
		var ptBuilder = new Mock<ISimplePowertrainBuilder>();
		var driver = new Mock<IDriverInfo>();
		var engine = new Mock<IEngineInfo>();
		var engineCtl = new Mock<IEngineControl>();
		var battery = new Mock<IRESSInfo>();
		var gearbox = new Mock<IGearboxInfo>();
		
		container.Setup(x => x.SimplePowertrainBuilder).Returns(ptBuilder.Object);
		container.Setup(x => x.DriverInfo).Returns(driver.Object);
		container.Setup(x => x.EngineInfo).Returns(engine.Object);
		container.Setup(x => x.EngineCtl).Returns(engineCtl.Object);
		container.Setup(x => x.BatteryInfo).Returns(battery.Object);
		container.Setup(x => x.GearboxesInfo).Returns(new List<IGearboxInfo>() {
			gearbox.Object
		});

		driver.Setup(x => x.DrivingAction).Returns(powertrainInfo.DrivingAction);
		engine.Setup(x => x.EngineN95hSpeed).Returns(powertrainInfo.EngineN95hSpeed);
		engineCtl.Setup(x => x.CombustionEngineOn).Returns(powertrainInfo.CombustionEngineOn);
		battery.Setup(x => x.StoredEnergy).Returns(powertrainInfo.BatteryStoredEnergy);
		battery.Setup(x => x.StateOfCharge).Returns(powertrainInfo.BatterySoC);
		gearbox.Setup(x => x.Gear).Returns(powertrainInfo.GearboxCurrentGear);

        return container;
	}

	private Mock<IResponse> CreateResponse(ResponseValues responseValues)
	{
		var response = new Mock<IResponse>();
		response.Setup(x => x.Gearbox).Returns(new GearboxResponse() {
			Gear = responseValues.ResponseGear
		});
		response.Setup(x => x.Engine).Returns(new EngineResponse() {
			DynamicFullLoadTorque = responseValues.EngineDynamicFullLoadTorque,
			DynamicFullLoadPower = responseValues.EngineDynamicFulloadPower,
			TotalTorqueDemand = responseValues.EngineTotalTorqueDemand,
			DragPower = responseValues.EngineDragPower,
			EngineSpeed = responseValues.EngineSpeed
		});
		response.Setup(x => x.ElectricSystem).Returns(new ElectricSystemResponseSuccess(this) {
			RESSPowerDemand = responseValues.REESSPowerDemand,
			RESSResponse = new RESSResponseSuccess(this) {
				PowerDemand = responseValues.REESSResponsePowerDemand,
				MaxDischargePower = responseValues.REESSResponseMaxDischargePower,
				MaxChargePower = responseValues.REESSResponseMaxChargePower
			}
		});
		return response;
	}

	private VectoRunData GetVectoRunData(ModelParams modelParams)
	{
		var runData = new VectoRunData();

		var fcMap = new Mock<IFuelConsumptionMap>();
		fcMap.Setup(x => x.GetFuelConsumptionValue(It.IsAny<NewtonMeter>(), It.IsAny<PerSecond>()))
			.Returns( 0.2.SI<KilogramPerSecond>());

		runData.ElectricMachinesSinglePwt = new List<Tuple<PowertrainPosition, ElectricMotorData>>() {
			Tuple.Create(PowertrainPosition.HybridP2, new ElectricMotorData())
		};
		runData.EngineData = new CombustionEngineData() {
			IdleSpeed = modelParams.EngineIdleSpeed,
			Inertia = modelParams.Inertia,
			Fuels = new List<CombustionEngineFuelData>() {
				new CombustionEngineFuelData() {
					FuelData = FuelData.Diesel,
					ConsumptionMap = fcMap.Object
				}
			},
			FullLoadCurves = new Dictionary<uint, EngineFullLoadCurve>() {
				{0, new EngineFullLoadCurve(new List<EngineFullLoadCurve.FullLoadCurveEntry>() {
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
				}, DeclarationData.PT1)}

			},
			EngineStartTime = modelParams.EngineStartTime
		};
		runData.GearshiftParametersSinglePwt = new ShiftStrategyParameters() {
			AllowedGearRangeFC = modelParams.GearshiftParamsAllowedGearRange
		};

		runData.GearboxSinglePwt = new GearboxData() {
			Gears = new Dictionary<uint, GearData>()
		};
		for (var i = 1u; i <= modelParams.GearboxNumGears; i++) {
			runData.GearboxSinglePwt.Gears[i] = new GearData();
		}
		runData.HybridStrategyParameters = new HybridStrategyParameters() {
			MinICEOnTime = modelParams.HybridStrategyMinIceOnTime
		};

		return runData;
	}
}