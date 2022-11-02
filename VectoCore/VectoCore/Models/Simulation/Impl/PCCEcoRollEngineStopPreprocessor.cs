using System.Collections.Generic;
using System.Linq;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.Models.Connector.Ports.Impl;
using TUGraz.VectoCore.Models.SimulationComponent.Impl;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.Models.Simulation.Impl
{
	public class PCCEcoRollEngineStopPreprocessor : ISimulationPreprocessor
	{
		protected SimplePowertrainContainer Container;
		private MeterPerSecond MaxSpeed;
		private MeterPerSecond MinSpeed;
		private Dictionary<MeterPerSecond, Radian> SlopeData;

		public PCCEcoRollEngineStopPreprocessor(
			SimplePowertrainContainer simpleContainer, Dictionary<MeterPerSecond, Radian> slopeData, MeterPerSecond minSpeed,
			MeterPerSecond maxSpeed)
		{
			Container = simpleContainer;
			MinSpeed = minSpeed;
			MaxSpeed = maxSpeed;
			SlopeData = slopeData;
			SpeedStep = 5.KMPHtoMeterPerSecond();
		}

		#region Implementation of ISimulationPreprocessor

		public void RunPreprocessing()
		{
			if (!(Container?.VehicleInfo is Vehicle vehicle)) {
				throw new VectoException("no vehicle found...");
			}

			switch (Container.GearboxInfo) {
				case Gearbox gearbox:
					RunPreprocessingAMTGearbox(gearbox, vehicle);
					return;
				case ATGearbox atGearbox:
					RunPreprocessingATGearbox(atGearbox, vehicle);
					return;
				case null when !Container.HasGearbox:
					RunPreprocessingNoGearbox(vehicle);
					return;
				default:
					throw new VectoException("no valid gearbox found...");
			}
		}

		private void RunPreprocessingATGearbox(ATGearbox gearbox, Vehicle vehicle)
		{
			var modData = Container.ModalData as ModalDataContainer;
			SlopeData.Clear();

			for (var speed = MinSpeed; speed <= MaxSpeed + SpeedStep; speed += SpeedStep) {
				var gear = FindLowestGearForSpeed(speed);
				gearbox.Gear = gear;
				gearbox.DisengageGearbox = true;
				vehicle.Initialize(speed, 0.SI<Radian>());
				var slope = SearchSlope(vehicle, Container);
				modData?.Reset();
				SlopeData[speed] = slope;
			}
		}

		private void RunPreprocessingNoGearbox(Vehicle vehicle)
		{
			var modData = Container.ModalData as ModalDataContainer;
			SlopeData.Clear();

			for (var speed = MinSpeed; speed <= MaxSpeed + SpeedStep; speed += SpeedStep) {
				vehicle.Initialize(speed, 0.SI<Radian>());
				var slope = SearchSlope(vehicle, Container);
				modData?.Reset();
				SlopeData[speed] = slope;
			}
		}

		private void RunPreprocessingAMTGearbox(Gearbox gearbox, Vehicle vehicle)
		{
			var modData = Container.ModalData as ModalDataContainer;
			SlopeData.Clear();

			for (var speed = MinSpeed; speed <= MaxSpeed + SpeedStep; speed += SpeedStep) {
				var gear = FindLowestGearForSpeed(speed);
				gearbox.Gear = gear;
				gearbox.DisengageGearbox = true;
				gearbox._nextGear = gear;
				vehicle.Initialize(speed, 0.SI<Radian>());
				var slope = SearchSlope(vehicle, Container);
				modData?.Reset();
				SlopeData[speed] = slope;
			}
		}

		private GearshiftPosition FindLowestGearForSpeed(MeterPerSecond speed)
		{
			var data = Container.RunData;
			var ratio = data.AxleGearData.AxleGear.Ratio * (data.AngledriveData?.Angledrive.Ratio ?? 1.0) /
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

		private Radian SearchSlope(Vehicle vehicle, SimplePowertrainContainer container)
		{
			var simulationInterval = Constants.SimulationSettings.TargetTimeInterval;
			var acceleration = 0.SI<MeterPerSquareSecond>();
			var absTime = 0.SI<Second>();
			var gradient = 0.SI<Radian>();

			foreach (var motor in container.ElectricMotors.Values) {
				if ((motor as ElectricMotor).Control is DummyElectricMotorControl emCtl) {
					emCtl.EmTorque = null;
				}
			}

			var initialResponse = vehicle.Request(absTime, simulationInterval, acceleration, gradient);
			var delta = initialResponse.Gearbox?.PowerRequest ?? initialResponse.ElectricMotor?.TotalTorqueDemand * initialResponse.ElectricMotor?.AvgDrivetrainSpeed;

			try {
				gradient = SearchAlgorithm.Search(
					gradient, delta, 0.1.SI<Radian>(),
					getYValue: response => {
						var r = (ResponseDryRun)response;
						return r.Gearbox?.PowerRequest ?? r.ElectricMotor?.TotalTorqueDemand * r.ElectricMotor?.AvgDrivetrainSpeed;
					},
					evaluateFunction: grad => vehicle.Request(absTime, simulationInterval, acceleration, grad, true),
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
