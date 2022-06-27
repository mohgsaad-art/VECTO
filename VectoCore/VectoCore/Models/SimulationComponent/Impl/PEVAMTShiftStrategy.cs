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
using TUGraz.VectoCore.Models.SimulationComponent.Strategies;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.Models.SimulationComponent.Impl
{

	public class PEVAMTShiftStrategy : LoggingObject, IShiftStrategy
	{
		protected IDataBus DataBus;
		protected readonly GearboxData GearboxModelData;

		protected Gearbox _gearbox;
		protected GearshiftPosition _nextGear;

		private ShiftStrategyParameters shiftStrategyParameters;
		protected readonly VelocityRollingLookup VelocityDropData = new VelocityRollingLookup();
		private SimplePowertrainContainer TestContainer;
		private Gearbox TestContainerGbx;
		private Battery TestContainerBattery;
		private BatterySystem TestContainerBatterySystem;
		private SuperCap TestContainerSuperCap;
		private ElectricMotor TestContainerElectricMotor;

		private VoltageLevelData VoltageLevels;
		private SI TransmissionRatio;
		private ShiftStrategyParameters GearshiftParams;
		private GearList GearList;
		private Dictionary<uint, ShiftPolygon> DeRatedShiftpolygons;
		private SimpleCharger TestContainerElectricSystemCharger;
		private double EMRatio;

		protected PowertrainPosition EMPos;

		public static string Name => "AMT - EffShift (BEV)";


		protected bool DriveOffStandstill { get; set; }

		public PEVAMTShiftStrategy(IVehicleContainer dataBus) : this(dataBus, false)
		{
			if (dataBus.RunData.VehicleData == null) {
				return;
			}

			EMPos = dataBus.RunData.ElectricMachinesData.FirstOrDefault(x =>
				x.Item1 == PowertrainPosition.BatteryElectricE2 || x.Item1 == PowertrainPosition.IEPC)?.Item1 ?? PowertrainPosition.HybridPositionNotSet;
			SetupVelocityDropPreprocessor(dataBus);
		}

		protected PEVAMTShiftStrategy(IVehicleContainer dataBus, bool dummy)
		{
			DataBus = dataBus;
			var runData = dataBus.RunData;
			shiftStrategyParameters = runData.GearshiftParameters;
			if (runData.VehicleData == null) {
				return;
			}
			EMPos = runData.ElectricMachinesData.FirstOrDefault(x =>
				x.Item1 == PowertrainPosition.BatteryElectricE2 || x.Item1 == PowertrainPosition.IEPC)?.Item1 ?? PowertrainPosition.HybridPositionNotSet;

			GearboxModelData = runData.GearboxData;
			GearshiftParams = runData.GearshiftParameters;
			GearList = GearboxModelData.GearList;
			MaxStartGear = GearList.Reverse().First();

			VoltageLevels = runData.ElectricMachinesData
				.FirstOrDefault(x => x.Item1 == EMPos)?.Item2.EfficiencyData;

			TransmissionRatio = (runData.AxleGearData?.AxleGear.Ratio ?? 1.0) *  // axlegeardata may be null for certain IEPC configurations
								(runData.AngledriveData?.Angledrive.Ratio ?? 1.0) /
								runData.VehicleData.DynamicTyreRadius;

			if (shiftStrategyParameters == null) {
				throw new VectoException("Parameters for shift strategy missing!");
			}

			var em = runData.ElectricMachinesData.First(x => x.Item1 == EMPos).Item2;
			EMRatio = em.RatioADC;
			DeRatedShiftpolygons = CalculateDeratedShiftLines(em,
				runData.GearboxData.InputData.Gears, runData.VehicleData.DynamicTyreRadius,
				runData.AxleGearData?.AxleGear.Ratio ?? 1.0, runData.GearboxData.Type);
		}

		protected void SetupVelocityDropPreprocessor(IVehicleContainer dataBus)
		{
			var runData = dataBus.RunData;
			// MQ: 2019-11-29 - fuel used here has no effect as this is the modDatacontainer for the test-powertrain only!
			TestContainer = new SimplePowertrainContainer(runData);
			PowertrainBuilder.BuildSimplePowertrainElectric(runData, TestContainer);
			TestContainerGbx = TestContainer.GearboxCtl as Gearbox;
			TestContainerBattery = TestContainer.BatteryInfo as Battery;
			TestContainerBatterySystem = TestContainer.BatteryInfo as BatterySystem;
			TestContainerSuperCap = TestContainer.BatteryInfo as SuperCap;
			TestContainerElectricSystemCharger = (TestContainer.ElectricSystemInfo as ElectricSystem)?.Charger.FirstOrDefault(x => x is SimpleCharger) as SimpleCharger ;
			TestContainerElectricMotor =
				TestContainer.ElectricMotorInfo(EMPos) as ElectricMotor;
			if (TestContainerGbx == null) {
				throw new VectoException("Unknown gearboxtype: {0}", TestContainer.GearboxCtl.GetType().FullName);
			}

			// register pre-processors
			var maxG = runData.Cycle.Entries.Max(x => Math.Abs(x.RoadGradientPercent.Value())) + 1;
			var grad = Convert.ToInt32(maxG / 2) * 2;
			if (grad == 0) {
				grad = 2;
			}

			dataBus.AddPreprocessor(
				new VelocitySpeedGearshiftPreprocessorE2(VelocityDropData, runData.GearboxData.TractionInterruption, TestContainer, -grad, grad, 2));
		}

		#region Implementation of IShiftPolygonCalculator

		public ShiftPolygon ComputeDeclarationShiftPolygon(GearboxType gearboxType, int i, EngineFullLoadCurve engineDataFullLoadCurve,
			IList<ITransmissionInputData> gearboxGears, CombustionEngineData engineData, double axlegearRatio, Meter dynamicTyreRadius,
			ElectricMotorData electricMotorData = null)
		{
			if (electricMotorData == null) {
				throw new VectoException("ElectricMotorData is required to calculate Shift Polygon!");
			}
			var emFld = electricMotorData.EfficiencyData.VoltageLevels.First().FullLoadCurve;
			return ComputeDeclarationShiftPolygon(i, gearboxGears, axlegearRatio, dynamicTyreRadius, electricMotorData,
				shiftStrategyParameters.PEV_DownshiftSpeedFactor.LimitTo(0, 1) * emFld.RatedSpeed, shiftStrategyParameters.PEV_DownshiftMinSpeedFactor * emFld.RatedSpeed);
		}


		public ShiftPolygon ComputeDeclarationShiftPolygon(int i,
			IList<ITransmissionInputData> gearboxGears, double axlegearRatio,
			Meter dynamicTyreRadius,
			ElectricMotorData electricMotorData, PerSecond downshiftMaxSpeed, PerSecond downshiftMinSpeed)
		{
			return DeclarationData.Gearbox.ComputeElectricMotorShiftPolygon(i,
				electricMotorData.EfficiencyData.VoltageLevels.First().FullLoadCurve, electricMotorData.RatioADC,
				gearboxGears, axlegearRatio, dynamicTyreRadius, downshiftMaxSpeed, downshiftMinSpeed);
		}

		#endregion

		protected internal Dictionary<uint, ShiftPolygon> CalculateDeratedShiftLines(ElectricMotorData em,
			IList<ITransmissionInputData> gearData, Meter rDyn, double axleGearRatio, GearboxType gearboxType)
		{
			var retVal = new Dictionary<uint, ShiftPolygon>();
			for (var i = 0u; i < gearData.Count; i++) {
				var emFld = em.EfficiencyData.VoltageLevels.First().FullLoadCurve;
				var contTqFld = new ElectricMotorFullLoadCurve(new List<ElectricMotorFullLoadCurve.FullLoadEntry>() {
					new ElectricMotorFullLoadCurve.FullLoadEntry() {
						MotorSpeed = 0.RPMtoRad(),
						FullDriveTorque = -em.Overload.ContinuousTorque,
						FullGenerationTorque = em.Overload.ContinuousTorque
					},
					new ElectricMotorFullLoadCurve.FullLoadEntry() {
						MotorSpeed = 1.1 * emFld.MaxSpeed,
						FullDriveTorque = -em.Overload.ContinuousTorque,
						FullGenerationTorque = em.Overload.ContinuousTorque
					}
				});
				var limitedFld = AbstractSimulationDataAdapter.IntersectEMFullLoadCurves(emFld, contTqFld);
				var limitedEm = new ElectricMotorData() {
					EfficiencyData = new VoltageLevelData() {
						VoltageLevels = new List<ElectricMotorVoltageLevelData>() {
							new ElectricMotorVoltageLevelData() {
								FullLoadCurve = limitedFld
							}
						}
					}
				};

				var shiftPolygon = ComputeDeclarationShiftPolygon((int)i,
					gearData, axleGearRatio,
					rDyn, limitedEm, shiftStrategyParameters.PEV_DeRatedDownshiftSpeedFactor * emFld.RatedSpeed,
					shiftStrategyParameters.PEV_DownshiftMinSpeedFactor * emFld.RatedSpeed);
				retVal[i + 1] = shiftPolygon;
			}

			return retVal;
		}

		#region Implementation of IShiftStrategy

		public bool ShiftRequired(Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity, NewtonMeter inTorque,
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
			if (DriveOffStandstill && response.ElectricMotor.AngularVelocity.IsGreater(VoltageLevels.VoltageLevels.First().FullLoadCurve.NP80low)) {
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

			//if ((ModelData.Gears[_nextGear].Ratio * outAngularVelocity - DataBus.EngineIdleSpeed) /
			//	(DataBus.EngineRatedSpeed - DataBus.EngineIdleSpeed) <
			//	Constants.SimulationSettings.ClutchClosingSpeedNorm && _nextGear > 1) {
			//	_nextGear--;
			//}

			return _nextGear != gear;
		}

		private GearshiftPosition CheckUpshift(Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity, NewtonMeter inTorque, PerSecond inAngularVelocity, GearshiftPosition currentGear, IResponse response)
		{
			// if the driver's intention is _not_ to accelerate or drive along then don't upshift
			if (DataBus.DriverInfo.DriverBehavior != DrivingBehavior.Accelerating && DataBus.DriverInfo.DriverBehavior != DrivingBehavior.Driving) {
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
			// upshift
			if (IsAboveUpShiftCurve(currentGear, inTorque, inAngularVelocity, r.ElectricMotor.DeRatingActive)) {
				nextGear = GearList.Successor(currentGear);

				while (GearList.HasSuccessor(nextGear)) {
					// check skip gears
					nextGear = GearList.Successor(nextGear);
					var response = RequestDryRunWithGear(absTime, dt, outTorque, outAngularVelocity, nextGear);

					inAngularVelocity = response.ElectricMotor.AngularVelocity;
					inTorque = response.ElectricMotor.PowerRequest / inAngularVelocity;

					var maxTorque = VectoMath.Min(-response.ElectricMotor.MaxDriveTorque,
						!nextGear.Equals(GearList.First())
							? GearboxModelData.Gears[nextGear.Gear].ShiftPolygon
								.InterpolateDownshift(response.Engine.EngineSpeed)
							: double.MaxValue.SI<NewtonMeter>());
					var reserve = 1 - inTorque / maxTorque;

					if (reserve >= 0 /*ModelData.TorqueReserve */ &&
						IsAboveDownShiftCurve(nextGear, inTorque, inAngularVelocity, r.ElectricMotor.DeRatingActive)) {
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

			var vDrop = DataBus.VehicleInfo.VehicleSpeed - estimatedVelocityPostShift;
			var vehicleSpeedPostShift =
				DataBus.VehicleInfo.VehicleSpeed - vDrop * shiftStrategyParameters.VelocityDropFactor;

			var totalTransmissionRatio =
				DataBus.ElectricMotorInfo(EMPos).ElectricMotorSpeed /
				DataBus.VehicleInfo.VehicleSpeed;

			var results = new List<Tuple<GearshiftPosition, double>>();
			foreach (var tryNextGear in GearList.IterateGears(GearList.Successor(currentGear),
				GearList.Successor(currentGear, (uint)shiftStrategyParameters.AllowedGearRangeFC))) {
				var response = RequestDryRunWithGear(absTime, dt, outTorque, outAngularVelocity, tryNextGear);

				var inAngularVelocity = GearboxModelData.Gears[tryNextGear.Gear].Ratio * outAngularVelocity;
				var inTorque = response.ElectricMotor.PowerRequest / inAngularVelocity;

				if (IsBelowDownShiftCurve(tryNextGear, inTorque, inAngularVelocity, resp.ElectricMotor.DeRatingActive)) {
					continue;
				}

				var estimatedEngineSpeed = vehicleSpeedPostShift * (totalTransmissionRatio /
						GearboxModelData.Gears[currentGear.Gear].Ratio * GearboxModelData.Gears[tryNextGear.Gear].Ratio);
				if (estimatedEngineSpeed.IsSmaller(shiftStrategyParameters.MinEngineSpeedPostUpshift)) {
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
				? 1 / shiftStrategyParameters.RatingFactorCurrentGear
				: shiftStrategyParameters.RatingFactorCurrentGear;

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

			if (response.ElectricMotor.DeRatingActive) {

			}
			// check with shiftline
			if (response.ElectricMotor.TorqueRequestEmMap != null && IsBelowDownShiftCurve(currentGear,
				response.ElectricMotor.TorqueRequestEmMap,
				response.ElectricMotor.AngularVelocity, response.ElectricMotor.DeRatingActive)) {

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
					var resp = RequestDryRunWithGear(absTime, dt, outTorque, outAngularVelocity, nextGear);

					inAngularVelocity = resp.ElectricMotor.AngularVelocity;
					inTorque = resp.ElectricMotor.PowerRequest / resp.ElectricMotor.AvgDrivetrainSpeed;

					if (IsAboveUpShiftCurve(nextGear, inTorque, inAngularVelocity, resp.ElectricMotor.DeRatingActive)) {
						nextGear = GearList.Successor(nextGear);
						break;
					}

					var maxTorque = VectoMath.Min(-resp.ElectricMotor.MaxDriveTorque,
						!nextGear.Equals(GearList.First())
							? GearboxModelData.Gears[nextGear.Gear].ShiftPolygon
								.InterpolateDownshift(resp.Engine.EngineSpeed)
							: double.MaxValue.SI<NewtonMeter>());
					var reserve = 1 - inTorque / maxTorque;

					if (reserve >= 0 /*ModelData.TorqueReserve */ &&
						IsBelowDownShiftCurve(nextGear, inTorque, inAngularVelocity, resp.ElectricMotor.DeRatingActive)) {
						continue;
					}

					
					nextGear = GearList.Successor(nextGear);
					break;
				}

				return nextGear;
			}

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
			var gbxOutSpeed = response.Engine.EngineSpeed /
							GearboxModelData.Gears[tmpGear.Gear].Ratio;
			var firstGear = GearList.Predecessor(currentGear, 1);
			var lastGear = GearList.Predecessor(currentGear, (uint)GearshiftParams.AllowedGearRangeFC);
			foreach (var gear in GearList.IterateGears(firstGear, lastGear)) {
				var ratio = gear.IsLockedGear()
					? GearboxModelData.Gears[gear.Gear].Ratio
					: GearboxModelData.Gears[gear.Gear].TorqueConverterRatio;
				candidates[gear] = gbxOutSpeed * ratio;
			}

			var ratedSpeed = VoltageLevels.VoltageLevels.First().FullLoadCurve.RatedSpeed;
			var maxSpeedNorm = VoltageLevels.MaxSpeed / ratedSpeed;
			var targetMotor = (shiftStrategyParameters.PEV_TargetSpeedBrakeNorm * (maxSpeedNorm - 1) + 1) * ratedSpeed;

			if (candidates.Any(x => x.Value > targetMotor && x.Value < VoltageLevels.MaxSpeed)) {
				var best = candidates.Where(x => x.Value > targetMotor && x.Value < VoltageLevels.MaxSpeed)
					.OrderBy(x => x.Value).First();
				return best.Key;
			}

			if (candidates.Any(x => x.Value < VoltageLevels.MaxSpeed))
				return candidates.Where(x => x.Value < VoltageLevels.MaxSpeed).MaxBy(x => x.Value).Key;
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

			var results = new List<Tuple<GearshiftPosition, double>>();
			foreach (var tryNextGear in GearList.IterateGears(GearList.Predecessor(currentGear),
				GearList.Predecessor(currentGear, (uint)shiftStrategyParameters.AllowedGearRangeFC))) {

				var response = RequestDryRunWithGear(absTime, dt, outTorque, outAngularVelocity, tryNextGear);
				var inAngularVelocity = GearboxModelData.Gears[tryNextGear.Gear].Ratio * outAngularVelocity;
				var inTorque = response.ElectricMotor.PowerRequest / inAngularVelocity;

				if (IsAboveUpShiftCurve(tryNextGear, inTorque, inAngularVelocity, response.ElectricMotor.DeRatingActive)) {
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
				? 1 / shiftStrategyParameters.RatingFactorCurrentGear
				: shiftStrategyParameters.RatingFactorCurrentGear;

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
			TestContainerGbx.Disengaged = false;
			TestContainerGbx.Gear = tryNextGear;

			TestContainerBattery?.Initialize(DataBus.BatteryInfo.StateOfCharge);
			//TestContainerBatterySystem?.Initialize(DataBus.BatteryInfo.StateOfCharge);
			//TestContainerSuperCap?.Initialize(DataBus.BatteryInfo.StateOfCharge);
			if (TestContainerBattery != null) {
				TestContainerBattery.PreviousState.PulseDuration =
					(DataBus.BatteryInfo as Battery).PreviousState.PulseDuration;
			}
			if (TestContainerBatterySystem != null) {
				var batSystem = DataBus.BatteryInfo as BatterySystem;
				foreach (var bsKey in batSystem.Batteries.Keys) {
					for (var i = 0; i < batSystem.Batteries[bsKey].Batteries.Count; i++) {
						TestContainerBatterySystem.Batteries[bsKey].Batteries[i]
							.Initialize(batSystem.Batteries[bsKey].Batteries[i].StateOfCharge);
					}
				}
				TestContainerBatterySystem.PreviousState.PulseDuration =
					(DataBus.BatteryInfo as BatterySystem).PreviousState.PulseDuration;
			}
			TestContainerSuperCap?.Initialize(DataBus.BatteryInfo.StateOfCharge);

			TestContainerElectricSystemCharger?.UpdateFrom(DataBus.ElectricSystemInfo.ChargePower);


			//var pos = ModelData.ElectricMachinesData.FirstOrDefault().Item1;
			TestContainerElectricMotor.ThermalBuffer =
				(DataBus.ElectricMotorInfo(EMPos) as ElectricMotor).ThermalBuffer;
			TestContainerElectricMotor.DeRatingActive =
				(DataBus.ElectricMotorInfo(EMPos) as ElectricMotor).DeRatingActive;

			TestContainer.GearboxOutPort.Initialize(outTorque, outAngularVelocity);
			var response = (ResponseDryRun)TestContainer.GearboxOutPort.Request(
				0.SI<Second>(), dt, outTorque, outAngularVelocity, true);
			LogEnabled = true;
			return response;
		}


		public GearshiftPosition InitGear(Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity)
		{
			if (DataBus.VehicleInfo.VehicleSpeed.IsEqual(0)) {
				return InitStartGear(absTime, outTorque, outAngularVelocity);
			}

			var emE2 = DataBus.ElectricMotorInfo(EMPos) as ElectricMotor;
			if (emE2 is null) {
				throw new VectoException("PEV Shift Strategy requires electric motor at position E2");
			}

			var emCtl = emE2.Control;
			emE2.Control = new PEVInitControl(DataBus as IVehicleContainer);
			foreach (var gear in GearList.Reverse()) {
				//for (var gear = (uint)GearboxModelData.Gears.Count; gear > 1; gear--) {
				var response = _gearbox.Initialize(absTime, gear, outTorque, outAngularVelocity);

				var inAngularSpeed = outAngularVelocity * GearboxModelData.Gears[gear.Gear].Ratio;
				var inTorque = response.ElectricMotor.PowerRequest / inAngularSpeed;

				// if in shift curve and torque reserve is provided: return the current gear
				if (!IsBelowDownShiftCurve(gear, inTorque, inAngularSpeed, false) && !IsAboveUpShiftCurve(gear, inTorque, inAngularSpeed, false)) {
					_nextGear = gear;
					return gear;
				}
			}
			emE2.Control = emCtl;
			// fallback: return first gear
			_nextGear = GearList.First();
			return _nextGear;
		}

		private GearshiftPosition InitStartGear(Second absTime, NewtonMeter outTorque, PerSecond outAngularVelocity)
		{
			DriveOffStandstill = true;

			var emSpeeds = new Dictionary<GearshiftPosition, Tuple<PerSecond, PerSecond, double>>();


			var emE2 = DataBus.ElectricMotorInfo(EMPos) as ElectricMotor;
			if (emE2 is null) {
				throw new VectoException("PEV Shift Strategy requires electric motor at position E2");
			}

			var emCtl = emE2.Control;
			emE2.Control = new PEVInitControl(DataBus as IVehicleContainer);
			foreach (var gear in GearList.Reverse()) {
				//for (var gear = (uint)GearboxModelData.Gears.Count; gear >= 1; gear--) {
				var inAngularSpeed = outAngularVelocity * GearboxModelData.Gears[gear.Gear].Ratio;

				var ratedSpeed = VoltageLevels.MaxSpeed * 0.9;
				if (inAngularSpeed > ratedSpeed || inAngularSpeed.IsEqual(0)) {
					continue;
				}

				var response = _gearbox.Initialize(absTime, gear, outTorque, outAngularVelocity);

				var fullLoadPower = -(response.ElectricMotor.MaxDriveTorque * response.ElectricMotor.AngularVelocity);
				//.DynamicFullLoadPower; //EnginePowerRequest - response.DeltaFullLoad;
				var reserve = 1 - (response.ElectricMotor.TorqueRequestEmMap ?? 0.SI<NewtonMeter>()) / response.ElectricMotor.MaxDriveTorqueEM;

				var isBelowDownshift = gear.Gear > 1 &&
										IsBelowDownshiftCurve(GearboxModelData.Gears[gear.Gear].ShiftPolygon, response.ElectricMotor.TorqueRequest,
											response.ElectricMotor.AngularVelocity);

				if (reserve >= GearshiftParams.StartTorqueReserve && !isBelowDownshift) {
					//_nextGear = gear;
					//return gear;
					emSpeeds[gear] = Tuple.Create(response.ElectricMotor.AngularVelocity,
						(GearshiftParams.StartSpeed * TransmissionRatio * GearboxModelData.Gears[gear.Gear].Ratio)
						.Cast<PerSecond>(), (response.ElectricMotor.ElectricMotorPowerMech / response.ElectricSystem.RESSPowerDemand).Value());
				}
			}

			emE2.Control = emCtl;

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
				return DeRatedShiftpolygons[gear.Gear];
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
					DataBus.ElectricMotorInfo(EMPos).MaxSpeed));
		}

		public void Disengage(Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity) { }

		public IGearbox Gearbox {
			get => _gearbox;
			set {
				var myGearbox = value as Gearbox;
				if (myGearbox == null) {
					throw new VectoException("This shift strategy can't handle gearbox of type {0}", value.GetType());
				}
				_gearbox = myGearbox;
			}
		}

		public GearshiftPosition NextGear => _nextGear;
		public bool CheckGearshiftRequired { get; protected set; }
		public GearshiftPosition MaxStartGear { get; }
		public void Request(Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity) { }

		public void WriteModalResults(IModalDataContainer container) { }


		#endregion

		private class PEVInitControl : IElectricMotorControl
		{
			protected IDataBus DataBus;
			protected ElectricMotorData ElectricMotorData;

			protected readonly GearboxData GearboxModelData;

			public PEVInitControl(IVehicleContainer dataBus)
			{
				DataBus = dataBus;
				ElectricMotorData = dataBus.RunData.ElectricMachinesData
					.First(x => x.Item1 == PowertrainPosition.BatteryElectricE2 || x.Item1 == PowertrainPosition.IEPC).Item2;
				GearboxModelData = dataBus.RunData.GearboxData;
			}

			#region Implementation of IElectricMotorControl

			public NewtonMeter MechanicalAssistPower(Second absTime, Second dt, NewtonMeter outTorque, PerSecond prevOutAngularVelocity,
				PerSecond currOutAngularVelocity, NewtonMeter maxDriveTorque, NewtonMeter maxRecuperationTorque,
				PowertrainPosition position, bool dryRun)
			{
				if (!DataBus.GearboxInfo.GearEngaged(absTime) && DataBus.DriverInfo.DrivingAction == DrivingAction.Roll) {
					var avgSpeed = (prevOutAngularVelocity + currOutAngularVelocity) / 2;
					var inertiaTorqueLoss = avgSpeed.IsEqual(0)
						? 0.SI<NewtonMeter>()
						: Formulas.InertiaPower(currOutAngularVelocity, prevOutAngularVelocity, ElectricMotorData.Inertia, dt) / avgSpeed;
					//var dragTorque = ElectricMotorData.DragCurve.Lookup()
					return (-inertiaTorqueLoss); //.LimitTo(maxDriveTorque, maxRecuperationTorque);
				}
				if (DataBus.DriverInfo.DrivingAction == DrivingAction.Coast ||
					DataBus.DriverInfo.DrivingAction == DrivingAction.Roll) {
					return null;
				}

				if (DataBus.DriverInfo.DrivingAction != DrivingAction.Halt && DataBus.VehicleInfo.VehicleSpeed.IsSmallerOrEqual(GearboxModelData.DisengageWhenHaltingSpeed) && outTorque.IsSmaller(0)) {
					return null;
				}

				if (maxDriveTorque == null) {
					return null;
				}

				return (-outTorque).LimitTo(maxDriveTorque, maxRecuperationTorque ?? VectoMath.Max(maxDriveTorque, 0.SI<NewtonMeter>()));
			}

			#endregion
		}
	}
}