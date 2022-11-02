using System;
using System.Collections.Generic;
using System.Linq;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.Models.Connector.Ports.Impl;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.Simulation.DataBus;
using TUGraz.VectoCore.Models.SimulationComponent.Strategies;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.Models.SimulationComponent.Impl
{
	public class VelocitySpeedGearshiftPreprocessor : ISimulationPreprocessor
	{
		protected readonly Second TractionInterruption;
		protected readonly VelocityRollingLookup VehicleVelocityDropLookup;
		protected SimplePowertrainContainer Container;

		public VelocitySpeedGearshiftPreprocessor(
			VelocityRollingLookup velocityDropData, Second tracktionInterruption, SimplePowertrainContainer simpleContainer,
			int minGradient = -24, int maxGradient = 24, int gradientStep = 2)
		{
			Container = simpleContainer;
			VehicleVelocityDropLookup = velocityDropData;
			TractionInterruption = tracktionInterruption;
			MinGradient = minGradient;
			MaxGradient = maxGradient;
			GradientStep = gradientStep;

			var speeds = Enumerable.Range(1, 12).Select(x => (x * 10).KMPHtoMeterPerSecond()).ToList();
			speeds.Insert(0, 5.KMPHtoMeterPerSecond());
			Speeds = speeds;
		}


		#region Implementation of ISimulationPreprocessor

		public void RunPreprocessing()
		{
			VehicleVelocityDropLookup.Data = IterateVehicleSpeedAndGradient();
		}

		#endregion

		public int MinGradient { get; set; }

		public int MaxGradient { get; set; }
		public int GradientStep { get; set; }

		protected Entry[] IterateVehicleSpeedAndGradient()
		{
			var container = Container;
			var vehicle = container?.VehicleInfo as Vehicle;

			if (vehicle == null) {
				throw new VectoException("no vehicle found...");
			}

			var gearbox = container.GearboxInfo as Gearbox;
			if (gearbox == null) {
				throw new VectoException("no gearbox found...");
			}

			var modData = container.ModalData as ModalDataContainer;
			var runData = container.RunData;
			var ratio = 1.0 / runData.VehicleData.DynamicTyreRadius *
						(runData.AxleGearData?.AxleGear.Ratio ?? 1.0) * // alxlegear may be null for certain IEPC configurations
						(runData.AngledriveData?.Angledrive.Ratio ?? 1.0);

			var tmp = new List<Entry>();
			//(Container.DriverInfo as MockDriver).DriverBehavior = DrivingBehavior.Coasting;
			var maxSpeed = GetVehicleMaxSpeed(runData);
			foreach (var speed in Speeds) {
				if (speed > maxSpeed) {
					continue;
				}

				var targetEngineSpeed = GetMotorTargetSpeed(runData);
				var gearForSpeed = runData.GearboxData.GearList
					.Where(x => !x.TorqueConverterLocked.HasValue || x.TorqueConverterLocked.Value)
					.Select(x => new {
						Gear = x,
						SpeedDiff = Math.Abs((speed * ratio * runData.GearboxData.Gears[x.Gear].Ratio -
											targetEngineSpeed).Value())
					})
					.OrderBy(x => x.SpeedDiff).FirstOrDefault()?.Gear ?? new GearshiftPosition(0);
				//var gearForSpeed = runData.GearboxData.Gears.OrderBy(x => Math.Abs((speed * ratio * x.Value.Ratio - targetEngineSpeed).Value()))
				//	.FirstOrDefault().Key;
				if (!gearForSpeed.Engaged) {
					continue;
				}

				for (var grad = MinGradient; grad <= MaxGradient; grad += GradientStep) {
					var gradient = VectoMath.InclinationToAngle(grad / 100.0);
					gearbox.Disengaged = false;
					gearbox.Gear = gearForSpeed;
					vehicle.Initialize(speed, gradient);
					gearbox.Gear = new GearshiftPosition(0);
					gearbox.Disengaged = true;
					gearbox.EngageTime = 100.SI<Second>();
					gearbox._nextGear = gearForSpeed;

					var vehicleSpeed = SimulateRollingVehicle(vehicle, gradient, container);
					modData?.Reset();
					tmp.Add(new Entry { StartVelocity = speed, Gradient = gradient, EndVelocity = vehicleSpeed });
				}
			}

			return tmp.ToArray();
		}

		protected virtual PerSecond GetMotorTargetSpeed(VectoRunData runData)
		{
			return 0.5 * (runData.EngineData.FullLoadCurves[0].RatedSpeed - runData.EngineData.IdleSpeed) + runData.EngineData.IdleSpeed;
		}

		protected virtual MeterPerSecond GetVehicleMaxSpeed(VectoRunData runData)
		{
			var axleGearData = runData.AxleGearData;
			var angledriveData = runData.AngledriveData;
			var hasAngleDrive = angledriveData != null && angledriveData.Angledrive != null;
			var angledriveRatio = hasAngleDrive && angledriveData.Type == AngledriveType.SeparateAngledrive
				? angledriveData.Angledrive.Ratio
				: 1.0;
			var axlegearRatio = axleGearData?.AxleGear.Ratio ?? 1.0;
			var dynamicTyreRadius = runData.VehicleData != null ? runData.VehicleData.DynamicTyreRadius : 0.0.SI<Meter>();

			var vehicleMaxSpeed = GetMaxMotorspeed(runData) /
					runData.GearboxData.Gears[runData.GearboxData.Gears.Keys.Max()].Ratio / axlegearRatio /
					angledriveRatio * dynamicTyreRadius;
			
			var maxSpeed = VectoMath.Min(
				vehicleMaxSpeed,
				(runData.VehicleDesignSpeed ?? 90.KMPHtoMeterPerSecond()) +
				(runData.DriverData?.OverSpeed?.OverSpeed ?? 0.KMPHtoMeterPerSecond()));
			return maxSpeed;
		}

		protected virtual PerSecond GetMaxMotorspeed(VectoRunData runData)
		{
			return runData.EngineData.FullLoadCurves[0].N95hSpeed;
		}

		public IList<MeterPerSecond> Speeds { get; }

		protected MeterPerSecond SimulateRollingVehicle(Vehicle vehicle, Radian gradient, IVehicleContainer container)
		{
			var simulationInterval = TractionInterruption;

			if (simulationInterval.IsEqual(0)) {
				return vehicle.VehicleSpeed;
			}

			var acceleration = 0.SI<MeterPerSquareSecond>();
			var absTime = 0.SI<Second>();
			var initialResponse = vehicle.Request(absTime, simulationInterval, acceleration, gradient);
			var delta = initialResponse.Gearbox.PowerRequest;
			try {
				var time = absTime;
				acceleration = SearchAlgorithm.Search(
					acceleration, delta, Constants.SimulationSettings.OperatingPointInitialSearchIntervalAccelerating,
					getYValue: response => {
						var r = (ResponseDryRun)response;
						return r.Gearbox.PowerRequest;
					},
					evaluateFunction: acc => {
						var response = vehicle.Request(time, simulationInterval, acc, gradient, true);
						response.Driver.Acceleration = acc;
						return response;
					},
					criterion: response => {
						var r = (ResponseDryRun)response;
						return r.Gearbox.PowerRequest.Value() * 100;
					},
					abortCriterion: (response, cnt) => {
						var r = (ResponseDryRun)response;
						return r != null && (vehicle.VehicleSpeed + r.Driver.Acceleration * simulationInterval) < 0.KMPHtoMeterPerSecond();
					},
					searcher: this
				);
				var step = vehicle.Request(absTime, simulationInterval, acceleration, gradient);
				if (!(step is ResponseSuccess)) {
					throw new VectoSimulationException("failed to find acceleration for rolling");
				}

				absTime += simulationInterval;
			} catch (VectoSearchAbortedException) {
				return 0.KMPHtoMeterPerSecond();
			}

			container.CommitSimulationStep(absTime, simulationInterval);

			return vehicle.VehicleSpeed;
		}

		public class Entry
		{
			public MeterPerSecond StartVelocity;
			public Radian Gradient;
			public MeterPerSecond EndVelocity;
		}
	}

	public class VelocitySpeedGearshiftPreprocessorE2 : VelocitySpeedGearshiftPreprocessor
	{
		public VelocitySpeedGearshiftPreprocessorE2(VelocityRollingLookup velocityDropData,
			Second tracktionInterruption, SimplePowertrainContainer simpleContainer, int minGradient = -24,
			int maxGradient = 24, int gradientStep = 2) : base(velocityDropData, tracktionInterruption, simpleContainer,
			minGradient, maxGradient, gradientStep) { }

		protected override PerSecond GetMotorTargetSpeed(VectoRunData runData)
		{
			var em = runData.ElectricMachinesData
				.FirstOrDefault(x => x.Item1 == PowertrainPosition.BatteryElectricE2 || x.Item1 == PowertrainPosition.IEPC);
			if (em == null) {
				throw new VectoException("E2 EM required for PEV E2 GearshiftPreprocessing");
			}
			return 0.5 * em.Item2.EfficiencyData.VoltageLevels.First().FullLoadCurve
				.MaxSpeed / em.Item2.RatioADC;
		}

		protected override PerSecond GetMaxMotorspeed(VectoRunData runData)
		{
			var em = runData.ElectricMachinesData
				.FirstOrDefault(x => x.Item1 == PowertrainPosition.BatteryElectricE2 || x.Item1 == PowertrainPosition.IEPC);
			if (em == null) {
				throw new VectoException("E2 EM required for PEV E2 GearshiftPreprocessing");
			}
			return em.Item2.EfficiencyData.VoltageLevels.First().FullLoadCurve.MaxSpeed / em.Item2.RatioADC;
        }
	}
}
