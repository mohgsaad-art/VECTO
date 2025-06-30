using Ninject;
using NUnit.Framework;
using TUGraz.Vecto.UnitTests.Utils.MockComponents;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore;
using TUGraz.VectoCore.InputData.Reader.ComponentData;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.Simulation.DataBus;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.Tests.Utils;
using Assert = NUnit.Framework.Assert;

namespace TUGraz.Vecto.UnitTests.TestCases.SummaryData;

public class SummaryDataContainerTests
{
	private StandardKernel _kernel;

	[OneTimeSetUp]
	public void Setup()
	{
		_kernel = new StandardKernel(new VectoNinjectModule());
	}

    [TestCase]
    public void TestSumCalcFixedTime()
    {
        var writer = new MemorySumDataWriter();
        var sumWriter = new SummaryDataContainer(writer);
		var rundata = GetDummyRundata();

        var modData = GetModalDataContainer(rundata);
		sumWriter.AddAuxiliary("FAN");
        sumWriter.CreateColumns(SummaryDataContainer.VehilceColumns);
        sumWriter.CreateColumns(SummaryDataContainer.BrakeColumns);
        sumWriter.UpdateTableColumns(rundata.EngineData);

        for (var i = 0; i < 500; i++) {
            modData[ModalResultField.simulationInterval] = 1.SI<Second>();
            modData[ModalResultField.n_ice_avg] = 600.RPMtoRad();
            modData[ModalResultField.v_act] = 1.SI<MeterPerSecond>(); //20.KMPHtoMeterPerSecond();
            modData[ModalResultField.drivingBehavior] = DrivingBehavior.Driving;
            modData[ModalResultField.time] = i.SI<Second>();
            modData[ModalResultField.dist] = i.SI<Meter>();
            modData["FAN"] = 3000.SI<Watt>();
            modData[ModalResultField.P_air] = 3000.SI<Watt>();
            modData[ModalResultField.P_roll] = 3000.SI<Watt>();
            modData[ModalResultField.P_slope] = 3000.SI<Watt>();
            modData[ModalResultField.P_aux_mech] = 3000.SI<Watt>();
            modData[ModalResultField.P_brake_loss] = 3000.SI<Watt>();

            modData[ModalResultField.FCMap] = 1e-4.SI<KilogramPerSecond>();
            modData[ModalResultField.FCFinal] = 1e-4.SI<KilogramPerSecond>();
            modData[ModalResultField.ICEOn] = false;
            modData[ModalResultField.altitude] = 0.SI<Meter>();
            modData[ModalResultField.acc] = 0.SI<MeterPerSquareSecond>();
            modData[ModalResultField.P_ice_out] = (i % 2 == 0 ? 1 : -1) * 3000.SI<Watt>();

            modData[ModalResultField.P_ice_fcmap] = 0.SI<Watt>();
            modData[ModalResultField.P_veh_inertia] = 0.SI<Watt>();
            modData[ModalResultField.P_wheel_inertia] = 0.SI<Watt>();

            modData.CommitSimulationStep();
        }

        sumWriter.Write(modData, rundata);

        modData.Finish(VectoRun.Status.Success);
        sumWriter.Finish();

		var sumData = writer.WrittenData.Rows[0];

        // duration: 500s, distance: 500m
        Assert.AreEqual(500, modData.Duration.Value());
        Assert.AreEqual(500, modData.Distance.Value());

        // 3kW * 500s => to kWh
        Assert.AreEqual(500.0 * 3000.0 / 1000 / 3600, (ConvertedSI)sumData["E_air [kWh]"], 1e-3);
        Assert.AreEqual(500.0 * 3000.0 / 1000 / 3600, (ConvertedSI)sumData["E_aux_FAN [kWh]"], 1e-3);
        Assert.AreEqual(500.0 * 3000.0 / 1000 / 3600, (ConvertedSI)sumData["E_roll [kWh]"], 1e-3);
        Assert.AreEqual(500.0 * 3000.0 / 1000 / 3600, (ConvertedSI)sumData["E_grad [kWh]"], 1e-3);
        Assert.AreEqual(500.0 * 3000.0 / 1000 / 3600, (ConvertedSI)sumData["E_aux_sum [kWh]"], 1e-3);
        Assert.AreEqual(500.0 * 3000.0 / 1000 / 3600, (ConvertedSI)sumData["E_brake [kWh]"], 1e-3);

        // 500s * 1e-4 kg/s = 0.05kg  => 0.05kg / 500 => to g/h
        Assert.AreEqual((500.0 * 1e-4) * 1000 * 3600 / 500.0, (ConvertedSI)sumData["FC-Map [g/h]"], 1e-3);
        //// 500s * 1e-4 kg/s = 0.05kg => 0.05kg / 500m => to g/km
        Assert.AreEqual((500.0 * 1e-4) * 1000 * 1000 / 500, (ConvertedSI)sumData["FC-Map [g/km]"], 1e-3);
    }

	
	[TestCase]
    public void TestSumCalcVariableTime()
    {
        var writer = new MemorySumDataWriter();
        var sumWriter = new SummaryDataContainer(writer);
		var rundata = GetDummyRundata();
        var modData = GetModalDataContainer(rundata);
        
		sumWriter.AddAuxiliary("FAN");
        sumWriter.CreateColumns(SummaryDataContainer.VehilceColumns);
        sumWriter.CreateColumns(SummaryDataContainer.BrakeColumns);
        sumWriter.UpdateTableColumns(rundata.EngineData);

        var timeSteps = new[]
        { 0.5.SI<Second>(), 0.3.SI<Second>(), 1.2.SI<Second>(), 12.SI<Second>(), 0.1.SI<Second>() };
        var powerDemand = new[]
        { 1000.SI<Watt>(), 1500.SI<Watt>(), 2000.SI<Watt>(), 2500.SI<Watt>(), 3000.SI<Watt>() };

        for (var i = 0; i < 500; i++) {
            modData[ModalResultField.simulationInterval] = timeSteps[i % timeSteps.Length];
            modData[ModalResultField.time] = i.SI<Second>();
            modData[ModalResultField.dist] = i.SI<Meter>();
            modData[ModalResultField.n_ice_avg] = 600.RPMtoRad();
            modData[ModalResultField.v_act] = 20.KMPHtoMeterPerSecond();
            modData[ModalResultField.drivingBehavior] = DrivingBehavior.Driving;
            modData["FAN"] = powerDemand[i % powerDemand.Length];
            modData[ModalResultField.P_air] = powerDemand[i % powerDemand.Length];
            modData[ModalResultField.P_roll] = powerDemand[i % powerDemand.Length];
            modData[ModalResultField.P_slope] = powerDemand[i % powerDemand.Length];
            modData[ModalResultField.P_aux_mech] = powerDemand[i % powerDemand.Length];
            modData[ModalResultField.P_brake_loss] = powerDemand[i % powerDemand.Length];

            modData[ModalResultField.altitude] = 0.SI<Meter>();
            modData[ModalResultField.acc] = 0.SI<MeterPerSquareSecond>();
            modData[ModalResultField.P_ice_out] = (i % 2 == 0 ? 1 : -1) * powerDemand[i % powerDemand.Length];

            modData[ModalResultField.P_ice_fcmap] = 0.SI<Watt>();
            modData[ModalResultField.FCFinal] = 0.SI<KilogramPerSecond>();
            modData[ModalResultField.ICEOn] = false;
            modData.CommitSimulationStep();
        }

        sumWriter.Write(modData, rundata);

        modData.Finish(VectoRun.Status.Success);
        sumWriter.Finish();

		var sumData = writer.WrittenData.Rows[0];

        // sum(dt * p) => to kWh
        Assert.AreEqual(0.934722222, (ConvertedSI)sumData["E_air [kWh]"], 1e-3);
        Assert.AreEqual(0.934722222, (ConvertedSI)sumData["E_aux_FAN [kWh]"], 1e-3);
        Assert.AreEqual(0.934722222, (ConvertedSI)sumData["E_roll [kWh]"], 1e-3);
        Assert.AreEqual(0.934722222, (ConvertedSI)sumData["E_grad [kWh]"], 1e-3);
        Assert.AreEqual(0.934722222, (ConvertedSI)sumData["E_aux_sum [kWh]"], 1e-3);
        Assert.AreEqual(0.934722222, (ConvertedSI)sumData["E_brake [kWh]"], 1e-3);
    }

	private ModalDataContainer GetModalDataContainer(VectoRunData rundata)
	{
		var modData = _kernel.Get<IModalDataFactory>().CreateModDataContainer(rundata, null, null, null) as ModalDataContainer;
		Assert.IsNotNull(modData);
		modData.Data.CreateColumns(ModalResults.DistanceCycleSignals);
		modData.Data.CreateCombustionEngineColumns(rundata);
		modData.Data.CreateColumns(ModalResults.VehicleSignals);
		modData.Data.CreateColumns(ModalResults.BrakeSignals);
		modData.Data.CreateColumns(ModalResults.DriverSignals);
		modData.Data.CreateColumns(ModalResults.WheelSignals);
		modData.AddAuxiliary("FAN");
		return modData;
	}

	private static VectoRunData GetDummyRundata()
	{
		return new VectoRunData() {
			JobName = "AuxWriteModFileSumFile",
			EngineData = new CombustionEngineData() {
				Fuels = new[] {new CombustionEngineFuelData {
					FuelData = FuelData.Diesel,
					ConsumptionMap = FuelConsumptionMapReader.Create(InputDataHelper.InputDataAsTableData("",
						new[] {
							"600,-45,0",
							"600,0,767",
							"600,100,1759",
							"800,-55,0",
							"800,0,951",
							"800,100,2346",
						}))
				}}.ToList(),
				IdleSpeed = 600.RPMtoRad(),
				RatedPowerDeclared = 300000.SI<Watt>(),
				RatedSpeedDeclared = 2000.RPMtoRad(),
				Displacement = 7.SI(Unit.SI.Liter).Cast<CubicMeter>()
			},
			ElectricMachinesData = new List<Tuple<PowertrainPosition, ElectricMotorData>>(),
			Cycle = new DrivingCycleData() {
				Name = "MockCycle",
				CycleType = CycleType.DistanceBased
			},
			DriverData = new DriverData() {
				EngineStopStart = new DriverData.EngineStopStartData()
			},
			Aux = new List<VectoRunData.AuxData>() {
				new() {
					ID = "FAN",
					PowerDemandMech = 3000.SI<Watt>(),
					Technology = new List<string>() {"FanTech"}
				}
			}
		};
	}

}