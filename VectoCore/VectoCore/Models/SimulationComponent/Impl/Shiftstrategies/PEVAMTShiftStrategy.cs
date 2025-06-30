using System;
using System.Collections.Generic;
using System.Linq;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.InputData.Reader.DataObjectAdapter;
using TUGraz.VectoCore.Models.Connector.Ports.Impl;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.Simulation.DataBus;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data.ElectricMotor;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Engine;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Gearbox;
using TUGraz.VectoCore.Models.SimulationComponent.Impl.Shiftstrategies.ShiftPolygonCalc;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.Models.SimulationComponent.Impl.Shiftstrategies
{
    public class PEVAMTShiftStrategy : LoggingObject, IShiftStrategy
	{

		public const string Name = "AMT - EffShift (BEV)";
		
		protected IVehicleContainer DataBus;

		protected readonly GearboxData GearboxModelData;

		protected IGearbox _gearbox;
		protected GearshiftPosition _nextGear;

		private readonly ShiftStrategyParameters _shiftStrategyParameters;

		private VoltageLevelData VoltageLevels;
		private SI TransmissionRatio;
		private ShiftStrategyParameters GearshiftParams;
		private GearList GearList;
		
		private double EMRatio;

		protected PowertrainPosition EMPos;

		protected bool DriveOffStandstill { get; set; }

		protected ITestPowertrain TestPowertrain;

		public PEVAMTShiftStrategy(IVehicleContainer container) : this(container, false)
		{
			if (container.RunData.VehicleData == null) {
				return;
			}

            SetupVelocityDropPreprocessor(container.SimplePowertrainBuilder);
		}

		public VelocityRollingLookup VelocityDropData { get; } = new VelocityRollingLookup();

		// this constructor is called by derived classes and the public constructor. performs common initialization
		protected PEVAMTShiftStrategy(IVehicleContainer dataBus, bool dummy)
		{
			DataBus = dataBus;
			var runData = dataBus.RunData;
			_shiftStrategyParameters = runData.GearshiftParameters;

			if (runData.VehicleData == null) {
				return;
			}
			var emPos = runData.ElectricMachinesData.FirstOrDefault(x =>
				x.Item1 == PowertrainPosition.BatteryElectricE2 || x.Item1 == PowertrainPosition.IEPC)?.Item1; // ?? PowertrainPosition.HybridPositionNotSet;
			if (!emPos.HasValue) {
				throw new VectoException("PEV Shift Strategy requires electric motor at position E2");
			}
			EMPos = emPos.Value;
            GearboxModelData = runData.GearboxData;
			GearshiftParams = runData.GearshiftParameters;
			GearList = GearboxModelData.GearList;
			MaxStartGear = GearList.Reverse().First();

			VoltageLevels = runData.ElectricMachinesData
				.FirstOrDefault(x => x.Item1 == EMPos)?.Item2.EfficiencyData;

			TransmissionRatio = (runData.AxleGearData?.AxleGear.Ratio ?? 1.0) *  // axlegeardata may be null for certain IEPC configurations
								(runData.AngledriveData?.Angledrive.Ratio ?? 1.0) /
								runData.VehicleData.DynamicTyreRadius;

			if (_shiftStrategyParameters == null) {
				throw new VectoException("Parameters for shift strategy missing!");
			}

			var em = runData.ElectricMachinesData.First(x => x.Item1 == EMPos).Item2;
			EMRatio = em.RatioADC;

            // create testcontainer
			var powertrainBuilder = dataBus.SimplePowertrainBuilder;
			TestPowertrain = powertrainBuilder.CreateTestPowertrain(DataBus, true, VectoSimulationJobType.BatteryElectricVehicle);

			foreach (var motor in TestPowertrain.ElectricMotors.Values) {
				if (motor.Control is ITestPowertrainElectricMotorControl emCtl) {
					emCtl.EmOff = false; //Make sure em is switched on
				}
			}
		}

		protected void SetupVelocityDropPreprocessor(ISimplePowertrainBuilder powertrainBuilder)
		{
			// register pre-processors
			var maxG = DataBus.RunData.Cycle.Entries.Max(x => Math.Abs(x.RoadGradientPercent.Value())) + 1;
			var grad = Convert.ToInt32(maxG / 2) * 2;
			if (grad == 0) {
				grad = 2;
			}

			var testPowertrain = powertrainBuilder.CreateTestPowertrain(DataBus, false, VectoSimulationJobType.BatteryElectricVehicle);
			DataBus.AddPreprocessor(
				new VelocitySpeedGearshiftPreprocessorE2(VelocityDropData, DataBus.RunData.GearboxData.TractionInterruption, testPowertrain, -grad, grad, 2));
		}

		#region Implementation of IShiftStrategy

		public virtual bool ShiftRequired(Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity, NewtonMeter inTorque,
			PerSecond inAngularVelocity, GearshiftPosition gear, Second lastShiftTime, IResponse response)
		{
			CheckGearshiftRequired = true;
			var retVal = DoCheckShiftRequired(absTime, dt, outTorque, outAngularVelocity, inTorque, inAngularVelocity,
				gear, lastShiftTime, response);
			CheckGearshiftRequired = false;
			return retVal;
		}

		private bool DoCheckShiftRequired(Second absTime, Second dt, NewtonMeter outTorque, 
			PerSecond outAngularVelocity, NewtonMeter inTorque, PerSecond inAngularVelocity, 
			GearshiftPosition gear, Second lastShiftTime, IResponse response)
		{
			// no shift when vehicle stands
			if (DataBus.VehicleInfo.VehicleStopped) {
				return false;
			}

			if (DriveOffStandstill &&
				DataBus.VehicleInfo.VehicleSpeed.IsGreaterOrEqual(DataBus.DrivingCycleInfo.TargetSpeed)) {
				DriveOffStandstill = false;
			}

			var voltageLevel = VoltageLevels.VoltageLevels.First();

            if (DriveOffStandstill && response.ElectricMotor.AngularVelocity.IsGreater(voltageLevel.FullLoadCurve?.NP80low ?? (voltageLevel as IEPCVoltageLevelData).FullLoadCurves[gear.Gear].NP80low)) {
				DriveOffStandstill = false;
			}

			if (DriveOffStandstill && response.ElectricMotor.TorqueRequestEmMap != null &&
				response.ElectricMotor.TorqueRequestEmMap.IsEqual(response.ElectricMotor.MaxDriveTorqueEM)) {
				DriveOffStandstill = false;
			}

			// emergency shift to not stall the engine ------------------------
			while (GearList.HasSuccessor(_nextGear) &&
					SpeedTooHighForEngine(_nextGear, inAngularVelocity / GearboxModelData.Gears[gear.Gear].Ratio)) {
				_nextGear = GearList.Successor(_nextGear);
			}
			if (_nextGear != gear) {
				return true;
			}
			if (DriveOffStandstill) {
				return false;
			}

			// normal shift when all requirements are fullfilled ------------------
			var minimumShiftTimePassed = (lastShiftTime + GearshiftParams.TimeBetweenGearshifts).IsSmallerOrEqual(absTime);
			if (!minimumShiftTimePassed) {
				return false;
			}

			_nextGear = CheckDownshift(absTime, dt, outTorque, outAngularVelocity, inTorque, inAngularVelocity, gear, response);
			if (_nextGear != gear) {
				return true;
			}

			_nextGear = CheckUpshift(absTime, dt, outTorque, outAngularVelocity, inTorque, inAngularVelocity, gear, response);

			return _nextGear != gear;
		}

		private GearshiftPosition CheckUpshift(Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity, NewtonMeter inTorque, PerSecond inAngularVelocity, GearshiftPosition currentGear, IResponse response)
		{
			// if the driver's intention is _not_ to accelerate or drive along then don't upshift
			if (DataBus.DriverInfo.DriverBehavior != DrivingBehavior.Accelerating && DataBus.DriverInfo.DriverBehavior != DrivingBehavior.Driving) {
				return currentGear;
			}

			if (DataBus.ElectricMotorInfo(EMPos).EmOff) {
				return currentGear;
			}
			if ((absTime - _gearbox.LastDownshift).IsSmaller(GearshiftParams.UpshiftAfterDownshiftDelay)) {
				return currentGear;
			}
			var nextGear = DoCheckUpshift(absTime, dt, outTorque, outAngularVelocity, inTorque, inAngularVelocity, currentGear, response);
			if (!nextGear.Equals(currentGear)) {
				return nextGear;
			}

			// early up shift to higher gear ---------------------------------------
			if (GearList.HasSuccessor(currentGear)) {
				nextGear = CheckEarlyUpshift(absTime, dt, outTorque, outAngularVelocity, currentGear, response);
			}

			return nextGear;
		}

		protected virtual GearshiftPosition DoCheckUpshift(Second absTime, Second dt, NewtonMeter outTorque,
			PerSecond outAngularVelocity, NewtonMeter inTorque, PerSecond inAngularVelocity,
			GearshiftPosition currentGear, IResponse r)
		{
			var nextGear = currentGear;
			var derated = r.ElectricMotor.DeRatingActive;
			// upshift
			if (IsAboveUpShiftCurve(currentGear, inTorque, inAngularVelocity, derated)) {
				
				inTorque = null;
				inAngularVelocity = null;
				r = null;
				
				nextGear = GearList.Successor(currentGear);
				while (GearList.HasSuccessor(nextGear)) {
					// check skip gears
					nextGear = GearList.Successor(nextGear);
					var nextResponse = RequestDryRunWithGear(absTime, dt, outTorque, outAngularVelocity, nextGear);

					var nextInAngularVelocity = nextResponse.Gearbox.InputSpeed;
					var nextInTorque = nextResponse.Gearbox.InputTorque;
					
					var maxTorque = VectoMath.Min(
						-nextResponse.ElectricMotor.MaxDriveTorque, //ADC considered
						!nextGear.Equals(GearList.First())
							? GearboxModelData.Gears[nextGear.Gear].ShiftPolygon.InterpolateDownshift(nextInAngularVelocity)
							: double.MaxValue.SI<NewtonMeter>()
						);
					
					var reserve = 1 - nextInTorque / maxTorque;

					if (reserve >= 0 /*ModelData.TorqueReserve */ &&
						IsAboveDownShiftCurve(nextGear, nextInTorque, nextInAngularVelocity, derated)) {
						continue;
					}

					nextGear = GearList.Predecessor(nextGear);
					break;
				}
			}

			return nextGear;
		}

		protected virtual GearshiftPosition CheckEarlyUpshift(Second absTime, Second dt, NewtonMeter outTorque,
			PerSecond outAngularVelocity, GearshiftPosition currentGear, IResponse resp)
		{
			var estimatedVelocityPostShift = VelocityDropData.Interpolate(DataBus.VehicleInfo.VehicleSpeed,
				DataBus.DrivingCycleInfo.RoadGradient ?? 0.SI<Radian>());
			if (!estimatedVelocityPostShift.IsGreater(DeclarationData.GearboxTCU.MIN_SPEED_AFTER_TRACTION_INTERRUPTION)
			) {
				return currentGear;
			}
			if (_shiftStrategyParameters.AllowedGearRangeFC == 0) {
				return currentGear;
			}
			
			var vDrop = DataBus.VehicleInfo.VehicleSpeed - estimatedVelocityPostShift;
			var vehicleSpeedPostShift =
				DataBus.VehicleInfo.VehicleSpeed - vDrop * _shiftStrategyParameters.VelocityDropFactor;

			var totalTransmissionRatio =
				DataBus.ElectricMotorInfo(EMPos).ElectricMotorSpeed /
				DataBus.VehicleInfo.VehicleSpeed;

			var results = new List<Tuple<GearshiftPosition, double>>();
			foreach (var tryNextGear in GearList.IterateGears(GearList.Successor(currentGear),
				GearList.Successor(currentGear, (uint)_shiftStrategyParameters.AllowedGearRangeFC))) {
				var response = RequestDryRunWithGear(absTime, dt, outTorque, outAngularVelocity, tryNextGear);

				var inAngularVelocity = GearboxModelData.Gears[tryNextGear.Gear].Ratio * outAngularVelocity;
				var inTorque = response.ElectricMotor.PowerRequest / inAngularVelocity;

				if (IsBelowDownShiftCurve(tryNextGear, inTorque, inAngularVelocity, resp.ElectricMotor.DeRatingActive)) {
					continue;
				}

				var estimatedEngineSpeed = vehicleSpeedPostShift * (totalTransmissionRatio /
						GearboxModelData.Gears[currentGear.Gear].Ratio * GearboxModelData.Gears[tryNextGear.Gear].Ratio);
				if (estimatedEngineSpeed.IsSmaller(_shiftStrategyParameters.MinEngineSpeedPostUpshift)) {
					continue;
				}

				var fullLoadPower = -response.ElectricMotor.MaxDriveTorque * response.ElectricMotor.AngularVelocity;
				var reserve = 1 - response.ElectricMotor.PowerRequest / fullLoadPower;
				if (reserve < 0) {
					continue;
				}

				var fcNext = GetFCRating(response);
				results.Add(Tuple.Create(tryNextGear, fcNext));

			}

			if (results.Count == 0) {
				return currentGear;
			}

			var responseCurrent = RequestDryRunWithGear(absTime, dt, outTorque, outAngularVelocity, currentGear);
			var fcCurrent = GetFCRating(responseCurrent);

			var minFc = results.MaxBy(x => x.Item2);

			var ratingFactor = outTorque < 0
				? 1 / _shiftStrategyParameters.RatingFactorCurrentGear
				: _shiftStrategyParameters.RatingFactorCurrentGear;

			if (minFc.Item2.IsGreater(fcCurrent * ratingFactor)) {
				return minFc.Item1;
			}

			return currentGear;
		}


		private GearshiftPosition CheckDownshift(Second absTime, Second dt, NewtonMeter outTorque,
			PerSecond outAngularVelocity, NewtonMeter inTorque, PerSecond inAngularVelocity,
			GearshiftPosition currentGear, IResponse response)
		{
			if ((absTime - _gearbox.LastUpshift).IsSmaller(GearshiftParams.DownshiftAfterUpshiftDelay)) {
				return currentGear;
			}

			if (!GearList.HasPredecessor(currentGear)) {
				return currentGear;
			}
			var derated = response.ElectricMotor.DeRatingActive;
			// check with shiftline
			if (IsBelowDownShiftCurve(currentGear, inTorque, inAngularVelocity, derated)) {
				inTorque = null;
				inAngularVelocity = null;
				
				
				 if (DataBus.DriverInfo.DriverBehavior == DrivingBehavior.Braking) {
				 	var brakingGear = SelectBrakingGear(currentGear, response);
				 	return brakingGear;
				 }

				var nextGear = GearList.Predecessor(currentGear);
				if (SpeedTooHighForEngine(nextGear, outAngularVelocity)) {
					return currentGear;
				}

				while (GearList.HasPredecessor(nextGear)) {
					// check skip gears
					nextGear = GearList.Predecessor(nextGear);
					var nextReponse = RequestDryRunWithGear(absTime, dt, outTorque, outAngularVelocity, nextGear);

					var nextInAngularVelocity = nextReponse.Gearbox.InputSpeed;
					var nextInTorque = nextReponse.Gearbox.InputTorque;
					
					if (IsAboveUpShiftCurve(nextGear, nextInTorque, nextInAngularVelocity, derated)) {
						nextGear = GearList.Successor(nextGear);
						break;
					}
					
					var maxDriveTorque = nextReponse.ElectricMotor.MaxDriveTorque; //Already on drivetrain side
					
					var isFirstGear = nextGear.Equals(GearList.First());
					
					var maxTorque = VectoMath.Min(
					(maxDriveTorque == null
								? null
								: -maxDriveTorque
					),
					(!isFirstGear 
								? GearboxModelData.Gears[nextGear.Gear].ShiftPolygon.InterpolateDownshift(nextInAngularVelocity)
								: double.MaxValue.SI<NewtonMeter>()
					));
					var reserve = 1 - nextInTorque / maxTorque;

					if (reserve >= 0 /*ModelData.TorqueReserve */ &&
						IsBelowDownShiftCurve(nextGear, nextInTorque, nextInAngularVelocity, derated)) {
						continue;
					}

					
					nextGear = GearList.Successor(nextGear);
					break;
				}

				return nextGear;
			}
			//Should not matter if calculated on gbx or em side.
			if (response.ElectricMotor.TorqueRequestEmMap != null 
				&& response.ElectricMotor.MaxRecuperationTorqueEM != null 
				&& response.ElectricMotor.TorqueRequestEmMap.IsEqual(
				response.ElectricMotor.MaxRecuperationTorqueEM,
				response.ElectricMotor.MaxRecuperationTorqueEM * 0.1)) {
				// no early downshift when close to max recuperation line
				return currentGear;
			}

			// check early downshift
			return CheckEarlyDownshift(absTime, dt, outTorque, outAngularVelocity, currentGear, response);
		}

		private GearshiftPosition SelectBrakingGear(GearshiftPosition currentGear, IResponse response)
		{   
			var tmpGear = new GearshiftPosition(currentGear.Gear, currentGear.TorqueConverterLocked);
			var candidates = new Dictionary<GearshiftPosition, PerSecond>();
			
			var gbxOutSpeed = response.Gearbox.InputSpeed /
							GearboxModelData.Gears[tmpGear.Gear].Ratio;
			
			
			var firstGear = GearList.Predecessor(currentGear, 1);
			var lastGear = GearList.Predecessor(currentGear, (uint)GearshiftParams.AllowedGearRangeFC);

			var electricMotor = DataBus.GetElectricMotors().First(x => x.Position != PowertrainPosition.GEN);
			var maxEmSpeedDt = electricMotor.MaxSpeedDt; //Drivetrain
			
			foreach (var gear in GearList.IterateGears(firstGear, lastGear)) {
				var ratio = gear.IsLockedGear()
					? GearboxModelData.Gears[gear.Gear].Ratio
					: GearboxModelData.Gears[gear.Gear].TorqueConverterRatio;
				var gbxInSpeed = gbxOutSpeed * ratio;
				if (GearboxModelData.Gears[gear.Gear].MaxSpeed != null && gbxInSpeed.IsGreater(GearboxModelData.Gears[gear.Gear].MaxSpeed)) {
					continue;
				}

				if (gbxInSpeed.IsGreater(maxEmSpeedDt)) {
					continue;
				}
                candidates[gear] = gbxInSpeed;
			}

			if (!candidates.Any()) {
				return tmpGear;
			}

			var curve = VoltageLevels.VoltageLevels.First().FullLoadCurve ?? (VoltageLevels.VoltageLevels.First() as IEPCVoltageLevelData).FullLoadCurves[currentGear.Gear];
            var ratedSpeed = curve.RatedSpeed;
			var maxSpeedNorm = VoltageLevels.MaxSpeed / ratedSpeed;
			var targetMotor = (_shiftStrategyParameters.PEV_TargetSpeedBrakeNorm * (maxSpeedNorm - 1) + 1) * ratedSpeed;

			if (candidates.Any(x => x.Value > targetMotor && x.Value < maxEmSpeedDt)) {
				var best = candidates.Where(x => x.Value > targetMotor && x.Value < maxEmSpeedDt)
					.OrderBy(x => x.Value).First();
				return best.Key;
			}

			if (candidates.Any(x => x.Value < maxEmSpeedDt))
				return candidates.Where(x => x.Value < maxEmSpeedDt).MaxBy(x => x.Value).Key;
			else {
				return candidates.MaxBy(x => x.Value).Key;
			}
		}

		protected virtual GearshiftPosition CheckEarlyDownshift(Second absTime, Second dt, NewtonMeter outTorque,
			PerSecond outAngularVelocity, GearshiftPosition currentGear, IResponse resp)
		{
			var estimatedVelocityPostShift = VelocityDropData.Interpolate(DataBus.VehicleInfo.VehicleSpeed,
				DataBus.DrivingCycleInfo.RoadGradient ?? 0.SI<Radian>());
			if (!estimatedVelocityPostShift.IsGreater(DeclarationData.GearboxTCU.MIN_SPEED_AFTER_TRACTION_INTERRUPTION)
			) {
				return currentGear;
			}

			if (_shiftStrategyParameters.AllowedGearRangeFC == 0) {
				return currentGear;
			}

			var results = new List<Tuple<GearshiftPosition, double>>();
			foreach (var tryNextGear in GearList.IterateGears(GearList.Predecessor(currentGear),
				GearList.Predecessor(currentGear, (uint)_shiftStrategyParameters.AllowedGearRangeFC))) {

				var response = RequestDryRunWithGear(absTime, dt, outTorque, outAngularVelocity, tryNextGear);
				// var inAngularVelocity = GearboxModelData.Gears[tryNextGear.Gear].Ratio * outAngularVelocity;
				// var inTorque = response.ElectricMotor.PowerRequest / inAngularVelocity;

				var inAngularVelocity = response.Gearbox.InputSpeed;
				var inTorque = response.Gearbox.InputTorque;
				
				if (IsAboveUpShiftCurve(tryNextGear, inTorque, inAngularVelocity, response.ElectricMotor.DeRatingActive)) {
					continue;
				}

				if (GearboxModelData.Gears[tryNextGear.Gear].MaxSpeed != null && inAngularVelocity.IsGreater(GearboxModelData.Gears[tryNextGear.Gear].MaxSpeed)) {
					continue;
				}

				var fcNext = GetFCRating(response);
				results.Add(Tuple.Create(tryNextGear, fcNext));
			}

			if (results.Count == 0) {
				return currentGear;
			}

			var responseCurrent = RequestDryRunWithGear(absTime, dt, outTorque, outAngularVelocity, currentGear);
			var fcCurrent = GetFCRating(responseCurrent);
			var minFc = results.MinBy(x => x.Item2);
			var ratingFactor = outTorque < 0
				? 1 / _shiftStrategyParameters.RatingFactorCurrentGear
				: _shiftStrategyParameters.RatingFactorCurrentGear;

			if (minFc.Item2.IsGreater(fcCurrent * ratingFactor)) {
				return minFc.Item1;
			}

			return currentGear;
		}


		protected double GetFCRating(ResponseDryRun response)//PerSecond engineSpeed, NewtonMeter tqCurrent)
		{
			var currentGear = response.Gearbox.Gear;
			if (currentGear.Gear == 0)
			{
				return 0;
			}
			// there's no power if the gear is 0.
			var maxGenTorque = VectoMath.Min(GearboxModelData.Gears[currentGear.Gear].MaxTorque, response.ElectricMotor.MaxRecuperationTorque);
			var maxDriveTorque = GearboxModelData.Gears[currentGear.Gear].MaxTorque != null
				? VectoMath.Max(-GearboxModelData.Gears[currentGear.Gear].MaxTorque, response.ElectricMotor.MaxDriveTorque)
				: response.ElectricMotor.MaxDriveTorque;

			var tqCurrent = (-response.ElectricMotor.TorqueRequest); // / response.ElectricMotor.AngularVelocity);
			if (!tqCurrent.IsBetween(maxDriveTorque, maxGenTorque)) {
				return double.NaN;
			}
			var engineSpeed = response.ElectricMotor.AngularVelocity;


			var fcCurRes = VoltageLevels.LookupElectricPower(DataBus.BatteryInfo.InternalVoltage, engineSpeed, tqCurrent / EMRatio, currentGear, true);
			if (fcCurRes.Extrapolated) {
				Log.Warn(
					"EffShift Strategy: Extrapolation of power consumption for current gear! n: {0}, Tq: {1}",
					engineSpeed, tqCurrent);
			}
			return fcCurRes.ElectricalPower.Value();
		}

		protected ResponseDryRun RequestDryRunWithGear(
			Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity, GearshiftPosition tryNextGear)
		{
			LogEnabled = false; 

			TestPowertrain.UpdateComponents();
			
			TestPowertrain.Gearbox.SetDisengaged = false;
			TestPowertrain.Gearbox.SetGear = tryNextGear;

			TestPowertrain.Container.GearboxOutPort.Initialize(outTorque, outAngularVelocity);
			var response = (ResponseDryRun)TestPowertrain.Container.GearboxOutPort.Request(
				0.SI<Second>(), dt, outTorque, outAngularVelocity, true);
			LogEnabled = true;
			return response;
		}


		public GearshiftPosition InitGear(Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity)
		{
			if (DataBus.VehicleInfo.VehicleSpeed.IsEqual(0)) {
				return InitStartGear(absTime, outTorque, outAngularVelocity);
			}

			foreach (var gear in GearList.Reverse()) {
                TestPowertrain.UpdateComponents();
                TestPowertrain.Gearbox.SetGear = gear;
                TestPowertrain.Gearbox.SetNextGear = gear;

                var response = TestPowertrain.Gearbox.Initialize(outTorque, outAngularVelocity);
                response = TestPowertrain.Gearbox.Request(absTime,
                    Constants.SimulationSettings.MeasuredSpeedTargetTimeInterval, outTorque, outAngularVelocity,
                    true);

    //             var inAngularSpeed = outAngularVelocity * GearboxModelData.Gears[gear.Gear].Ratio;
				// var inTorque = response.Gearbox.PowerRequest / inAngularSpeed;

				var inAngularSpeed = response.Gearbox.InputSpeed;
				var inTorque = response.Gearbox.InputTorque;
				
				// if in shift curve and torque reserve is provided: return the current gear
				if (!IsBelowDownShiftCurve(gear, inTorque, inAngularSpeed, false) && !IsAboveUpShiftCurve(gear, inTorque, inAngularSpeed, false)) {
					_nextGear = gear;
					return gear;
				}
			}
			// fallback: return first gear
			_nextGear = GearList.First();
			return _nextGear;
		}

		private GearshiftPosition InitStartGear(Second absTime, NewtonMeter outTorque, PerSecond outAngularVelocity)
		{
			DriveOffStandstill = true;

			var emSpeeds = new Dictionary<GearshiftPosition, Tuple<PerSecond, PerSecond, double>>();
			
			foreach (var gear in GearList.Reverse()) {
				var inAngularSpeed = outAngularVelocity * GearboxModelData.Gears[gear.Gear].Ratio;

				var ratedSpeed = VoltageLevels.MaxSpeed * 0.9;
				if (inAngularSpeed > ratedSpeed || inAngularSpeed.IsEqual(0)) {
					continue;
				}

                //var response = _gearbox.Initialize(absTime, gear, outTorque, outAngularVelocity);
                TestPowertrain.UpdateComponents();
                TestPowertrain.Gearbox.SetGear = gear;
                TestPowertrain.Gearbox.SetNextGear = gear;

                var response = TestPowertrain.Gearbox.Initialize(outTorque, outAngularVelocity);
                response = TestPowertrain.Gearbox.Request(absTime,
                    Constants.SimulationSettings.MeasuredSpeedTargetTimeInterval, outTorque, outAngularVelocity,
                    true);

                var fullLoadPower = -(response.ElectricMotor.MaxDriveTorque * response.ElectricMotor.AngularVelocity);
				//.DynamicFullLoadPower; //EnginePowerRequest - response.DeltaFullLoad;
				var reserve = 1 - (response.ElectricMotor.TorqueRequestEmMap ?? 0.SI<NewtonMeter>()) / response.ElectricMotor.MaxDriveTorqueEM;

				var isBelowDownshift = gear.Gear > 1 &&
										IsBelowDownshiftCurve(GearboxModelData.Gears[gear.Gear].ShiftPolygon, -response.ElectricMotor.TorqueRequestEmMap,
											response.ElectricMotor.AngularVelocity);

				if (reserve >= GearshiftParams.StartTorqueReserve && !isBelowDownshift) {
					//_nextGear = gear;
					//return gear;
					emSpeeds[gear] = Tuple.Create(response.ElectricMotor.AngularVelocity,
						(GearshiftParams.StartSpeed * TransmissionRatio * GearboxModelData.Gears[gear.Gear].Ratio)
						.Cast<PerSecond>(), 
						!response.ElectricSystem.RESSPowerDemand.IsEqual(0) 
							? (response.ElectricMotor.ElectricMotorPowerMech / response.ElectricSystem.RESSPowerDemand).Value()
							: 0
						);
				}
			}

			if (emSpeeds.Any()) {
				var optimum = emSpeeds.MaxBy(x => x.Key.Gear); //x => VectoMath.Abs(x.Value.Item2 - FullLoadCurve.MaxSpeed * 0.5));
				_nextGear = optimum.Key;
				return _nextGear;
			}
			_nextGear = GearList.First();
			return _nextGear;
		}

		


		protected bool IsBelowDownShiftCurve(GearshiftPosition gear, NewtonMeter inTorque, PerSecond inEngineSpeed,
			bool deRatingActive)
		{
			if (!GearList.HasPredecessor(gear)) {
				return false;
			}

			var shiftPolygon = GetShiftpolygon(gear, deRatingActive);
			return IsBelowDownshiftCurve(shiftPolygon, inTorque, inEngineSpeed);
		}

		

		protected bool IsAboveDownShiftCurve(GearshiftPosition gear, NewtonMeter inTorque, PerSecond inEngineSpeed,
			bool deRatingActive)
		{
			if (!GearList.HasPredecessor(gear)) {
				return true;
			}
			var shiftPolygon = GetShiftpolygon(gear, deRatingActive);

			return IsAboveDownshiftCurve(shiftPolygon, inTorque, inEngineSpeed);
		}

		protected bool IsAboveUpShiftCurve(GearshiftPosition gear, NewtonMeter inTorque, PerSecond inEngineSpeed,
			bool deRatingActive)
		{
			if (!GearList.HasSuccessor(gear)) {
				return false;
			}

			var shiftPolygon = GetShiftpolygon(gear, deRatingActive);
			return shiftPolygon.IsAboveUpshiftCurve(inTorque, inEngineSpeed);
		}

		private ShiftPolygon GetShiftpolygon(GearshiftPosition gear, bool deRatingActive)
		{
			if (deRatingActive) {
				return GearboxModelData.Gears[gear.Gear].DeRatedEmShiftPolygon;
				//return DeRatedShiftpolygons[gear.Gear];
			}
			return GearboxModelData.Gears[gear.Gear].ShiftPolygon;
		}

		protected bool IsBelowDownshiftCurve(ShiftPolygon shiftPolygon, NewtonMeter emTorque, PerSecond emSpeed)
		{
			foreach (var entry in shiftPolygon.Downshift.Pairwise()) {
				if (!emTorque.IsBetween(entry.Item1.Torque, entry.Item2.Torque)) {
					continue;
				}

				if (ShiftPolygon.IsLeftOf(emSpeed, emTorque, entry)) {

					return true;
				}
			}

			return false;
		}

		protected bool IsAboveDownshiftCurve(ShiftPolygon shiftPolygon, NewtonMeter emTorque, PerSecond emSpeed)
		{
			foreach (var entry in shiftPolygon.Downshift.Pairwise()) {
				if (!emTorque.IsBetween(entry.Item1.Torque, entry.Item2.Torque)) {
					continue;
				}

				if (ShiftPolygon.IsRightOf(emSpeed, emTorque, entry)) {
					return true;
				}
			}

			return false;
		}

		public GearshiftPosition Engage(Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity)
		{
			while (GearList.HasSuccessor(_nextGear) && SpeedTooHighForEngine(_nextGear, outAngularVelocity)) {
				_nextGear = GearList.Successor(_nextGear);
			}

			return _nextGear;
		}

		protected bool SpeedTooHighForEngine(GearshiftPosition gear, PerSecond outAngularSpeed)
		{
			return
				(outAngularSpeed * GearboxModelData.Gears[gear.Gear].Ratio).IsGreaterOrEqual(VectoMath.Min(GearboxModelData.Gears[gear.Gear].MaxSpeed,
					DataBus.ElectricMotorInfo(EMPos).MaxSpeedDt));
		}

		public void Disengage(Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity) { }

		public IGearbox Gearbox {
			get => _gearbox;
			set {
				if (!(value is IPEVGearbox || value is IIEPCGearbox || value is IAPTNGearbox)) {
					throw new VectoException("This shift strategy can't handle gearbox of type {0}", value.GetType());
				}
				_gearbox = value;
			}
		}

		public GearshiftPosition NextGear => _nextGear;
		public bool CheckGearshiftRequired { get; protected set; }
		public GearshiftPosition MaxStartGear { get; }
		public void Request(Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity) { }

		public void WriteModalResults(IModalDataContainer container) { }


		#endregion

	}
}