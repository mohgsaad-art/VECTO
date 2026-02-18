using System.Text;
using Moq;
using Ninject;
using NUnit.Framework;
using TUGraz.Vecto.UnitTests.Utils;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.FileIO.JSON;
using TUGraz.VectoCore.InputData.FileIO.XML;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.Models.SimulationComponent;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Ninject;
using TUGraz.VectoCore.Ninject.PowertrainComponents;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.OutputData.FileIO;

namespace TUGraz.Vecto.UnitTests.TestCases.Components.HybridStrategy;

public class HybridStrategyCostCalculationExtractTestdata
{
	public interface ICostCalculationDataWriter
	{
		void WriteCalculationData(CostCalculationData calcData);
		void Finish();
	}

	public class CostCalculationDataWriter : ICostCalculationDataWriter
	{
		public CostCalculationDataWriter()
		{
			var outFile = "out.txt";
			if (TestContext.CurrentContext.Test.Arguments.Length > 0 &&
				TestContext.CurrentContext.Test.Arguments.First() is string jobName) {
				outFile = Path.GetFileNameWithoutExtension(jobName) + "_calcData.csv";
			}

			OutStream = new StreamWriter(new FileStream(outFile, FileMode.Create), Encoding.UTF8);
			OutStream.WriteLine(CostCalculationDataExtension.HEADER);
		}

		public StreamWriter OutStream { get; set; }


		#region Implementation of IICostCalculationDataWriter

		public void WriteCalculationData(CostCalculationData calcData)
		{
			OutStream.WriteLine(calcData.GetString());
		}

		public void Finish()
		{
			OutStream.Flush();
			OutStream.Close();
		}

		#endregion
	}

	public class HybridStrategyTestData : VectoCore.Models.SimulationComponent.Strategies.HybridStrategy
	{
		public HybridStrategyTestData(VectoRunData runData, IVehicleContainer container) : base(runData, container)
		{
			//Writer = writer;
		}

		[Inject]
		protected ICostCalculationDataWriter Writer { get; set; }

		protected override void CalculateCosts(IResponse resp, Second dt, HybridResultEntry tmp, bool allowIceOff,
			bool dryRun)
		{
			base.CalculateCosts(resp, dt, tmp, allowIceOff, dryRun);

			//if (!dryRun) {
			//	return;}

			StoreCalculationResults(resp, dt, tmp, allowIceOff, dryRun);
		}

		private void StoreCalculationResults(IResponse resp, Second dt, HybridResultEntry tmp, bool allowIceOff,
			bool dryRun)
		{
			var calcData = new CostCalculationData() {
				AbsTime = DataBus.AbsTime,
				dt = dt,
				AllowICEOff = allowIceOff,
				DryRun = dryRun,
				ModelParams = new ModelParams() {
					EngineIdleSpeed = ModelData.EngineData.IdleSpeed,
					Inertia = ModelData.EngineData.Inertia,
					EngineStartTime = ModelData.EngineData.EngineStartTime,
					HybridStrategyMinIceOnTime = ModelData.HybridStrategyParameters.MinICEOnTime,
					GearshiftParamsAllowedGearRange = ModelData.GearshiftParametersSinglePwt.AllowedGearRangeFC,
					GearboxNumGears = (uint)ModelData.GearboxSinglePwt.Gears.Count,
					FuelConsumption = ModelData.EngineData.Fuels.First().ConsumptionMap.GetFuelConsumptionValue(resp.Engine.TotalTorqueDemand, resp.Engine.EngineSpeed),
					BatteryDischargeEnergyThreshold = BatteryDischargeEnergyThreshold,
				},
				PowertrainInfo = new PowertrainInfo() {
					CombustionEngineOn = DataBus.EngineCtl.CombustionEngineOn,
					EngineN95hSpeed = DataBus.EngineInfo.EngineN95hSpeed,
					EngineSpeed = DataBus.EngineInfo.EngineSpeed,
					DrivingAction = DataBus.DriverInfo.DrivingAction,
					BatteryStoredEnergy = DataBus.BatteryInfo.StoredEnergy,
					BatterySoC = DataBus.BatteryInfo.StateOfCharge,
					GearboxCurrentGear = DataBus.GearboxesInfo.First().Gear.Gear,
					PreviousStateGearboxEngaged = PreviousState.GearboxEngaged,
                },
				Response = new ResponseValues() {
					ResponseGear = resp.Gearbox.Gear.Gear,
					EngineTorqueOutDemand = resp.Engine.TorqueOutDemand,
					EngineDynamicFullLoadTorque = resp.Engine.DynamicFullLoadTorque,
					EngineDynamicFulloadPower = resp.Engine.DynamicFullLoadPower,
					EngineDragPower = resp.Engine.DragPower,
					EngineTotalTorqueDemand = resp.Engine.TotalTorqueDemand,
					EngineSpeed = resp.Engine.EngineSpeed,
					REESSPowerDemand = resp.ElectricSystem.RESSPowerDemand,
					REESSResponsePowerDemand = resp.ElectricSystem.RESSResponse.PowerDemand,
					REESSResponseMaxChargePower = resp.ElectricSystem.RESSResponse.MaxChargePower,
					REESSResponseMaxDischargePower = resp.ElectricSystem.RESSResponse.MaxDischargePower,
				},
				Expected = new ExpectedValues() {
					BatCosts = tmp.BatCosts,
					FuelCosts = tmp.FuelCosts,
					Cost = tmp.Cost,
					IngoreReason = tmp.IgnoreReason,
				}
			};

			Writer?.WriteCalculationData(calcData);
		}

		#region Overrides of AbstractHybridStrategy

		public override void CommitSimulationStep(Second time, Second simulationInterval)
		{
			base.CommitSimulationStep(time, simulationInterval);
			if (DataBus.GetCycleOutPort().Progress.Equals(1.0)) {
				Writer?.Finish();
				Writer = null;
			}
		}

		#endregion
	}

	private IKernel _vectoKernel;
	protected ISimulatorFactoryFactory _simFactoryFactory;
	protected IXMLInputDataReader _inputDataReader;

    protected void SetupNinject()
	{
		_vectoKernel = new StandardKernel(
			new VectoNinjectModule()
		);
		
		var _realPowertrain = new ComponentBindingNameHelper("RealPowertrain");

        _vectoKernel.Rebind<IHybridControlStrategy>().To<HybridStrategyTestData>()
			.Named(_realPowertrain.HybridStrategyName(VectoSimulationJobType.ParallelHybridVehicle, CycleType.DistanceBased, false, false));
		_vectoKernel.Bind<IHybridControlStrategy>().To<HybridStrategyTestData>()
			.Named(_realPowertrain.HybridStrategyName(VectoSimulationJobType.IHPC, CycleType.DistanceBased, false, false));

		_vectoKernel.Bind<ICostCalculationDataWriter>().To<CostCalculationDataWriter>();

		_simFactoryFactory = _vectoKernel.Get<ISimulatorFactoryFactory>();
		Assert.NotNull(_simFactoryFactory);
		_inputDataReader = _vectoKernel.Get<IXMLInputDataReader>();
		Assert.NotNull(_inputDataReader);
    }

	[OneTimeSetUp]
	public void OneTimeSetup()
	{
		// update all necessary bindings so that no simulation is performed
		SetupNinject();
	}

	
    protected const string ParallelHybridLorry = "Generic Vehicles/Declaration Mode/xEV XML Jobs/Lorries/Group5_HEV_P2_non-ovc.xml";

	protected const string SuperCapLorry = "Generic Vehicles/Declaration Mode/xEV XML Jobs/Lorries/Group5_HEV_P2_supercap.xml";

	protected const string Group5P2_EM1 = "Generic Vehicles/Declaration Mode/P_HEV/Group5_P2/P2 Group 5_EMLossMap_1.vecto";

    [TestCase(ParallelHybridLorry, MissionType.LongHaul, LoadingType.ReferenceLoad)]
	[TestCase(SuperCapLorry, MissionType.RegionalDelivery, LoadingType.ReferenceLoad)]
	[TestCase(Group5P2_EM1, MissionType.RegionalDelivery, LoadingType.ReferenceLoad)]
	[Ignore("Testcase used for generating testdata")]
	public void HybridStrategyGenerateCostCalcResultDataTest(string jobFile, MissionType mission, LoadingType loading)
	{
		var fileName = Path.Combine(ProjectSource.ProjectDirectory(), jobFile);
		_vectoKernel.Rebind<IMissionFilter>().ToMethod(ctx => {
			var mock = new Mock<IMissionFilter>();
			mock.Setup(x => x.Run(It.IsAny<MissionType>(), It.IsAny<LoadingType>())).Returns(
				(MissionType m, LoadingType l) => m == mission && l == loading);
			return mock.Object;
		}).InSingletonScope();

        //var inputProvider = _inputDataReader.CreateDeclaration(fileName);
		var inputProvider = Path.GetExtension(fileName) == ".xml"
			? _inputDataReader.CreateDeclaration(fileName)
			: JSONInputDataFactory.ReadJsonJob(fileName);
        var reportWriter = new FileOutputWriter(fileName);
		var sumWriter = new SummaryDataContainer(null);
		var jobContainer = new JobContainer(sumWriter);

		var _simulatorFactory =
			_simFactoryFactory.Factory(ExecutionMode.Declaration, inputProvider, reportWriter, null, null, true);

		jobContainer.AddRuns(_simulatorFactory);
		jobContainer.Execute(false);
		jobContainer.WaitFinished();
    }
}
