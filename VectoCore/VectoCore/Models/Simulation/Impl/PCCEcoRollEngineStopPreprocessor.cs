using System.Collections.Generic;
using System.Linq;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.Models.Connector.Ports.Impl;
using TUGraz.VectoCore.Models.Simulation.DataBus;
using TUGraz.VectoCore.Models.SimulationComponent;
using TUGraz.VectoCore.Models.SimulationComponent.Impl;
using TUGraz.VectoCore.Models.SimulationComponent.Impl.Gearbox;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.Models.Simulation.Impl
{
    public class PCCEcoRollEngineStopPreprocessor : ISimulationPreprocessor
	{
		protected ITestPowertrain TestPowertrain;
		private MeterPerSecond MaxSpeed;
		private MeterPerSecond MinSpeed;
		private Dictionary<MeterPerSecond, Radian> SlopeData;

		public PCCEcoRollEngineStopPreprocessor(
			ITestPowertrain testPowertrain, Dictionary<MeterPerSecond, Radian> slopeData, MeterPerSecond minSpeed,
			MeterPerSecond maxSpeed)
		{
			TestPowertrain = testPowertrain;
			MinSpeed = minSpeed;
			MaxSpeed = maxSpeed;
			SlopeData = slopeData;
			SpeedStep = 5.KMPHtoMeterPerSecond();
		}

		#region Implementation of ISimulationPreprocessor

		public void RunPreprocessing()
		{
			if (TestPowertrain.Container?.VehicleInfo == null) {
				throw new VectoException("no vehicle found...");
			}

			if (TestPowertrain.Vehicle == null) {
				throw new VectoException("Vehicle not applicable for PCC Preprocessor");
			}

			switch (TestPowertrain.Container.GearboxInfo) {
				case IAMTGearbox _:
				case IEPCGearboxMultipleGears _:
                    RunPreprocessingAMTGearbox();
					return;
				case IAPTGearbox _:
					RunPreprocessingATGearbox();
					return;
				case null when !TestPowertrain.Container.HasGearbox:
				case IGearboxInfo _ when !TestPowertrain.Container.HasGearbox:
					RunPreprocessingNoGearbox();
					return;
				default:
					throw new VectoException("no valid gearbox found...");
			}
		}

		private void RunPreprocessingATGearbox()
		{
			var modData = TestPowertrain.Container.ModalData;
			SlopeData.Clear();

			for (var speed = MinSpeed; speed <= MaxSpeed + SpeedStep; speed += SpeedStep) {
				var gear = FindLowestGearForSpeed(speed);
				TestPowertrain.Gearbox.SetGear = gear;
				TestPowertrain.Gearbox.SetDisengageGearbox = true;
				TestPowertrain.Vehicle.Initialize(speed, 0.SI<Radian>());
				var slope = SearchSlope();
				modData?.Reset();
				SlopeData[speed] = slope;
			}
		}

		private void RunPreprocessingNoGearbox()
		{
			var modData = TestPowertrain.Container.ModalData;
			SlopeData.Clear();

			for (var speed = MinSpeed; speed <= MaxSpeed + SpeedStep; speed += SpeedStep) {
				TestPowertrain.Vehicle.Initialize(speed, 0.SI<Radian>());
				var slope = SearchSlope();
				modData?.Reset();
				SlopeData[speed] = slope;
			}
		}

		private void RunPreprocessingAMTGearbox()
		{
			var modData = TestPowertrain.Container.ModalData;
			SlopeData.Clear();

			for (var speed = MinSpeed; speed <= MaxSpeed + SpeedStep; speed += SpeedStep) {
				var gear = FindLowestGearForSpeed(speed);
				TestPowertrain.Gearbox.SetGear = gear;
				TestPowertrain.Gearbox.SetDisengageGearbox = true;
				TestPowertrain.Gearbox.SetNextGear = gear;
				TestPowertrain.Vehicle.Initialize(speed, 0.SI<Radian>());
				var slope = SearchSlope();
				modData?.Reset();
				SlopeData[speed] = slope;
			}
		}

		private GearshiftPosition FindLowestGearForSpeed(MeterPerSecond speed)
		{
			var data = TestPowertrain.Container.RunData;
			var ratio = (data.AxleGearData?.AxleGear.Ratio ?? 1.0 ) * (data.AngledriveData?.Angledrive.Ratio ?? 1.0) /
						data.VehicleData.DynamicTyreRadius;
			var possible = new List<GearshiftPosition>();
			foreach (var gear in data.GearboxData.GearList) {
				if (gear.TorqueConverterLocked.HasValue && !gear.TorqueConverterLocked.Value) {
					continue;
				}

				var n = speed * ratio * data.GearboxData.Gears[gear.Gear].Ratio;

				possible.Add(n < (data.EngineData?.IdleSpeed ?? 0.SI<PerSecond>()) ? new GearshiftPosition(0) : gear);
			}

			var selected = possible.MaxBy(x => x.Gear);
			return selected;
		}

		private Radian SearchSlope()
		{
			var simulationInterval = Constants.SimulationSettings.TargetTimeInterval;
			var acceleration = 0.SI<MeterPerSquareSecond>();
			var absTime = 0.SI<Second>();
			var gradient = 0.SI<Radian>();

			foreach (var motor in TestPowertrain.ElectricMotors.Values) {
				if (motor.Control is ITestPowertrainElectricMotorControl emCtl) {
					emCtl.EmOff = true;
				}
			}

			var initialResponse = TestPowertrain.Vehicle.Request(absTime, simulationInterval, acceleration, gradient, false);
			var delta = initialResponse.Gearbox?.PowerRequest ?? initialResponse.ElectricMotor?.TotalTorqueDemand * initialResponse.ElectricMotor?.AvgDrivetrainSpeed;

			try {
				gradient = SearchAlgorithm.Search(
					gradient, delta, 0.1.SI<Radian>(),
					getYValue: response => {
						var r = (ResponseDryRun)response;
						return r.Gearbox?.PowerRequest ?? r.ElectricMotor?.TotalTorqueDemand * r.ElectricMotor?.AvgDrivetrainSpeed;
					},
					evaluateFunction: grad => TestPowertrain.Vehicle.Request(absTime, simulationInterval, acceleration, grad, true),
					criterion: response => {
						var r = (ResponseDryRun)response;
						return (r.Gearbox?.PowerRequest ?? r.ElectricMotor?.TotalTorqueDemand * r.ElectricMotor?.AvgDrivetrainSpeed).Value();
					},
					searcher: this
				);
			} catch (VectoSearchAbortedException) {
				return gradient;
			}

			return gradient;
		}

		public MeterPerSecond SpeedStep { get; set; }

		#endregion
	}

	public class PCCSegments
	{
		public PCCSegments()
		{
			Segments = new List<PCCSegment>();
			CurrentIdx = 0;
		}

		public void MoveNext()
		{
			CurrentIdx = CurrentIdx + 1;
			if (CurrentIdx >= Count) {
				CurrentIdx = Count - 1;
			}
		}

		public int CurrentIdx { get; private set; }

		public int Count => Segments.Count;

		public PCCSegment Current => Segments.Any() ? Segments[CurrentIdx] : null;

		public List<PCCSegment> Segments { get; }
	}

	public class PCCSegment
	{
		public Meter StartDistance { get; set; }
		public Meter DistanceAtLowestSpeed { get; set; }
		public Meter EndDistance { get; set; }
		public MeterPerSecond TargetSpeed { get; set; }
		public Meter Altitude { get; set; }
		public Joule EnergyAtLowestSpeed { get; set; }
		public Joule EnergyAtEnd { get; set; }
	}
}
