using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Linq;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.Models.Connector.Ports;
using TUGraz.VectoCore.Models.Connector.Ports.Impl;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.Models.SimulationComponent.Impl
{
	/// <summary>
	/// Velocity Speed Gearshift Preprocessor for Single Powertrain Vehicles
	/// </summary>
	public class VelocitySpeedGearshiftPreprocessor : ISimulationPreprocessor
	{
		protected readonly Second TractionInterruption;
		protected readonly VelocityRollingLookup VehicleVelocityDropLookup;
		protected ITestPowertrain TestPowertrain;

		public VelocitySpeedGearshiftPreprocessor(
			VelocityRollingLookup velocityDropData, Second tracktionInterruption, ITestPowertrain testPowertrain, ISimplePowertrainContainer powertrainContainer,
			int minGradient = -24, int maxGradient = 24, int gradientStep = 2)
		{
			TestPowertrain = testPowertrain;
			SimplePowertrainContainer = powertrainContainer;
			VehicleVelocityDropLookup = velocityDropData;
			TractionInterruption = tracktionInterruption;
			MinGradient = minGradient;
			MaxGradient = maxGradient;
			GradientStep = gradientStep;

			var speeds = Enumerable.Range(1, 12).Select(x => (x * 10).KMPHtoMeterPerSecond()).ToList();
			speeds.Insert(0, 5.KMPHtoMeterPerSecond());
			Speeds = speeds;
		}

		public ISimplePowertrainContainer SimplePowertrainContainer { get; }


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
			var container = TestPowertrain.Container;
			var runData = container.RunData;
			var powertrainData = runData.AxlePowertrainsDict[SimplePowertrainContainer.AxleNumber];
			var vehicle = container?.VehicleInfo as IVehicle;
			var vehiclePort = vehicle as IDriverDemandOutPort;

            if (vehicle == null || vehiclePort == null) {
				throw new VectoException("no vehicle found...", ErrorCodes.VEHICLE_MISSING);
			}

			var gearbox = SimplePowertrainContainer.Gearbox;
            
			if (gearbox == null) {
				throw new VectoException("no gearbox found...", ErrorCodes.VEHICLE_MISSING_GEARBOX);
			}

			var modData = container.ModalData as ModalDataContainer;
			var ratio = 1.0 / runData.VehicleData.DynamicTyreRadius *
						(powertrainData.AxleGearData?.AxleGear.Ratio ?? 1.0) * // alxlegear may be null for certain IEPC configurations
						(powertrainData.AngledriveData?.Angledrive.Ratio ?? 1.0);

			var tmp = new List<Entry>();
			//(Container.DriverInfo as MockDriver).DriverBehavior = DrivingBehavior.Coasting;
			var maxSpeed = GetVehicleMaxSpeed(runData);

			foreach (var speed in Speeds) {
				if (speed > maxSpeed) {
					continue;
				}

				var targetEngineSpeed = GetMotorTargetSpeed(runData);
				var gearForSpeed = powertrainData.GearboxData.GearList
					.Where(x => !x.TorqueConverterLocked.HasValue || x.TorqueConverterLocked.Value)
					.Select(x => new {
						Gear = x,
						SpeedDiff = Math.Abs((speed * ratio * powertrainData.GearboxData.Gears[x.Gear].Ratio -
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
					gearbox.SetDisengaged = false;
					gearbox.SetGear = gearForSpeed;
					vehiclePort.Initialize(speed, gradient);
					gearbox.SetGear = new GearshiftPosition(0);
					gearbox.SetDisengaged = true;
					gearbox.SetEngageTime = 100.SI<Second>();
					gearbox.SetNextGear = gearForSpeed;

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
			var powertrainData = runData.AxlePowertrainsDict[SimplePowertrainContainer.AxleNumber];
			var gbxData = powertrainData.GearboxData;
			var axleGearData = powertrainData.AxleGearData;
			var angledriveData = powertrainData.AngledriveData;
			var hasAngleDrive = angledriveData != null && angledriveData.Angledrive != null;
			var angledriveRatio = hasAngleDrive && angledriveData.Type == AngledriveType.SeparateAngledrive
				? angledriveData.Angledrive.Ratio
				: 1.0;
			var axlegearRatio = axleGearData?.AxleGear.Ratio ?? 1.0;
			var dynamicTyreRadius = runData.VehicleData != null ? runData.VehicleData.DynamicTyreRadius : 0.0.SI<Meter>();

			var vehicleMaxSpeed = GetMaxMotorspeed(runData) /
					gbxData.Gears[gbxData.Gears.Keys.Max()].Ratio / axlegearRatio /
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

		protected MeterPerSecond SimulateRollingVehicle(IVehicle vehicle, Radian gradient, IVehicleContainer container)
		{
			var simulationInterval = TractionInterruption;

			if (simulationInterval.IsEqual(0)) {
				return vehicle.VehicleSpeed;
			}

			var vehiclePort = vehicle as IDriverDemandOutPort;

			var acceleration = 0.SI<MeterPerSquareSecond>();
			var absTime = 0.SI<Second>();
			var initialResponse = vehiclePort.Request(absTime, simulationInterval, acceleration, gradient, false);
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
						var response = vehiclePort.Request(time, simulationInterval, acc, gradient, true);
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
				var step = vehiclePort.Request(absTime, simulationInterval, acceleration, gradient, false);
				if (!(step is ResponseSuccess) && !(step is ResponseEngineSpeedTooHigh)) {
					throw new VectoSimulationException("failed to find acceleration for rolling", ErrorCodes.SPEEDPREPROCESSOR_ROLLING_CANNOT_FIND_ACCELERATION);
				}

				absTime += simulationInterval;
			} catch (VectoException ex) when (ex.GetErrorCode() == ErrorCodes.SEARCH_ABORTED) {
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
			Second tracktionInterruption, ITestPowertrain testPowertrain, ISimplePowertrainContainer simplePowertrainContainer, int minGradient = -24,
			int maxGradient = 24, int gradientStep = 2) : base(velocityDropData, tracktionInterruption, testPowertrain, simplePowertrainContainer,
			minGradient, maxGradient, gradientStep) { }

		protected override PerSecond GetMotorTargetSpeed(VectoRunData runData)
		{
			var em = GetEMPos(runData);
			if (em == null) {
				throw new VectoException("E2 EM required for PEV E2 GearshiftPreprocessing", ErrorCodes.VEHICLE_MISSING_ELECTRICMOTOR);
			}

            var voltageLevel = em.Item2.EfficiencyData.VoltageLevels.First();

            return 0.5 * (voltageLevel.FullLoadCurve?.MaxSpeed ?? (voltageLevel as IEPCVoltageLevelData).FullLoadCurves.Min(x => x.Value.MaxSpeed)) / em.Item2.RatioADC;
		}

		protected override PerSecond GetMaxMotorspeed(VectoRunData runData)
		{
			var em = GetEMPos(runData);
			if (em == null) {
				throw new VectoException("E2 EM required for PEV E2 GearshiftPreprocessing", ErrorCodes.VEHICLE_MISSING_ELECTRICMOTOR);
			}

			var voltageLevel = em.Item2.EfficiencyData.VoltageLevels.First();
            return (voltageLevel.FullLoadCurve?.MaxSpeed ?? (voltageLevel as IEPCVoltageLevelData).FullLoadCurves.Min(x => x.Value.MaxSpeed)) / em.Item2.RatioADC;
        }
		
		private Tuple<PowertrainPosition, ElectricMotorData> GetEMPos(VectoRunData runData)
		{
			var axleNumber = SimplePowertrainContainer.AxleNumber;
			
			var electricMachines = runData.AxlePowertrainsDict[axleNumber].ElectricMachinesData;
			
			if (runData.JobType.IsOneOf(VectoSimulationJobType.ParallelHybridVehicle, VectoSimulationJobType.IHPC) && runData.BatteryOnlyHybridMode) {
				return electricMachines
					.FirstOrDefault(x => x.Item1.IsOneOf(PowertrainPosition.HybridP2, PowertrainPosition.HybridP2_5, PowertrainPosition.IHPC));
            }
			return electricMachines
				.FirstOrDefault(x => x.Item1 == PowertrainPosition.BatteryElectricE2 || x.Item1 == PowertrainPosition.IEPC);
        }
	}
}
