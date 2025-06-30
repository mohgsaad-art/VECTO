using Ninject;
using NUnit.Framework;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.OutputData;
using Assert = NUnit.Framework.Assert;

namespace TUGraz.Vecto.UnitTests.TestCases.Components.ModDataContainer;

public class ModalDataContainerTests
{
	private StandardKernel _kernel;

	[OneTimeSetUp]
	public void Setup()
	{
		_kernel = new StandardKernel(new VectoNinjectModule());
	}

	[TestCase(80, 0),
	TestCase(80, -0.1),
	TestCase(10, 0.1)]
	public void ModDataDistanceCalculationTest(double initialSpeedVal, double accVal)
	{
		var rundata = new VectoRunData() {
			JobName = "sumDataTest",
			Cycle = new DrivingCycleData() {
				CycleType = CycleType.DistanceBased
			}
		};
		var modData = _kernel.Get<IModalDataFactory>().CreateModDataContainer(rundata, null, null, null) as ModalDataContainer;
		Assert.IsNotNull(modData);
		modData.Data.CreateColumns(ModalResults.DistanceCycleSignals);
		modData.Data.CreateColumns(ModalResults.DriverSignals);
		var initalSpeed = initialSpeedVal.KMPHtoMeterPerSecond();
		var speed = initalSpeed;
		var dist = 0.SI<Meter>();
		var dt = 0.5.SI<Second>();
		var acc = accVal.SI<MeterPerSquareSecond>();
		for (var i = 0; i < 100; i++) {
			modData[ModalResultField.v_act] = speed;
			modData[ModalResultField.simulationInterval] = dt;
			modData[ModalResultField.acc] = acc;
			dist += speed * dt + acc * dt * dt / 2.0;
			speed += acc * dt;
			modData[ModalResultField.dist] = dist;
			modData.CommitSimulationStep();
		}

		// distance = 80km/h * 50s + acc/2 * 50s * 50s
		var totalTime = 50.SI<Second>();
		var expected = initalSpeed * totalTime + acc / 2.0 * totalTime * totalTime;

		Assert.AreEqual(expected.Value(), modData.Distance.Value(), 1e-6);
	}
}