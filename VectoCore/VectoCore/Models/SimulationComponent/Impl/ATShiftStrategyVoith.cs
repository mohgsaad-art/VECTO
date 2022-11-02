using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.Simulation.DataBus;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Engine;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.Models.SimulationComponent.Impl
{
	public class ATShiftStrategyVoith : ATShiftStrategy
	{
		

		protected ShiftStrategyParameters shiftParameters;

		protected internal Dictionary<int, ShiftLineSet> UpshiftLines = new Dictionary<int, ShiftLineSet>();
		protected internal Dictionary<int, ShiftLineSet> DownshiftLines = new Dictionary<int, ShiftLineSet>();
		protected Kilogram MinMass;
		protected Kilogram MaxMass;
		protected EngineFullLoadCurve FullLoadCurve;
		protected MeterPerSquareSecond _accMin = 0.SI<MeterPerSquareSecond>();
		private MeterPerSquareSecond _accMax = 0.SI<MeterPerSquareSecond>();
		private int _loadStage;
		private List<SchmittTrigger> LoadStageSteps;
		private Radian roadGradient;
		private MeterPerSquareSecond driverAcceleration;
		private bool dualTCTransmission;

		public ATShiftStrategyVoith(IVehicleContainer dataBus) : base(dataBus)
		{
			var data = dataBus.RunData;
			if (data.EngineData == null) {
				return;
			}
			shiftParameters = data.GearshiftParameters;
			InitializeShiftLines(shiftParameters.GearshiftLines);
			LoadStageSteps = new List<SchmittTrigger>();
			foreach (var entry in shiftParameters.LoadstageThresholds) {
				LoadStageSteps.Add(new SchmittTrigger(entry));
			}

			MinMass = data.VehicleData.MinimumVehicleMass;
			MaxMass = data.VehicleData.MaximumVehicleMass;
			FullLoadCurve = data.EngineData.FullLoadCurves[0];

			dualTCTransmission = GearboxModelData.Gears[1].HasTorqueConverter && GearboxModelData.Gears[2].HasTorqueConverter;
		}

		private void InitializeShiftLines(TableData lines)
		{
			var slopeDh = VectoMath.InclinationToAngle(ShiftLineSet.DownhillSlope / 100.0);
			var slopeLevel = VectoMath.InclinationToAngle(0);
			var slopeUh = VectoMath.InclinationToAngle(ShiftLineSet.UphillSlope / 100.0);

			foreach (DataRow row in lines.Rows) {
				var shift = row[ShiftLinesColumns.Shift].ToString().Split('-');
				var g1 = shift[0].ToInt();
				var g2 = shift[1].ToInt();
				bool upshift;
				if (g1 + 1 == g2) {
					// upshift
					upshift = true;
				} else if (g2 + 1 == g1) {
					// downshift
					upshift = false;
				} else {
					throw new VectoException("invalid shift entry: {0}-{1}", g1, g2);
				}

				var loadStage = row[ShiftLinesColumns.LoadStage].ToString().ToInt();

				var nDhAmaxLower = row.Field<string>(ShiftLinesColumns.nDhAmax).ToDouble().RPMtoRad();
				var nLevelAmaxLower = row.Field<string>(ShiftLinesColumns.nLevelAmax).ToDouble()
										.RPMtoRad();
				var nUhAmaxLower = row.Field<string>(ShiftLinesColumns.nUhAmax).ToDouble().RPMtoRad();

				var nDhAminLower = GetAlternativeIfEmpty(row, ShiftLinesColumns.nDhAmin, ShiftLinesColumns.nDhAmax)
					.RPMtoRad();
				var nLevelAminLower = GetAlternativeIfEmpty(
					row, ShiftLinesColumns.nLevelAmin, ShiftLinesColumns.nLevelAmax).RPMtoRad();
				var nUhAminLower = GetAlternativeIfEmpty(row, ShiftLinesColumns.nUhAmin, ShiftLinesColumns.nUhAmax)
					.RPMtoRad();

				ShiftLineSet shiftLineSet;
				if (upshift) {
					if (!UpshiftLines.ContainsKey(g1)) {
						UpshiftLines[g1] = new ShiftLineSet();
					}
					shiftLineSet = UpshiftLines[g1];
					if (shiftLineSet.LoadStages.ContainsKey(loadStage)) {
						throw new VectoException(
							"Gearshift entries for upshift {0}-{1} load stage {2} already defined!", g1, g2, loadStage);
					}
				} else {
					if (!DownshiftLines.ContainsKey(g1)) {
						DownshiftLines[g1] = new ShiftLineSet();
					}
					shiftLineSet = DownshiftLines[g1];
					if (shiftLineSet.LoadStages.ContainsKey(loadStage)) {
						throw new VectoException(
							"Gearshift entries for downshift {0}-{1} load stage {2} already defined!", g1, g2, loadStage);
					}
				}

				var entry = new ShiftLines();

				entry.entriesAMin.Add(Tuple.Create(slopeDh, nDhAminLower));
				entry.entriesAMin.Add(Tuple.Create(slopeLevel, nLevelAminLower));
				entry.entriesAMin.Add(Tuple.Create(slopeUh, nUhAminLower));

				entry.entriesAMax.Add(Tuple.Create(slopeDh, nDhAmaxLower));
				entry.entriesAMax.Add(Tuple.Create(slopeLevel, nLevelAmaxLower));
				entry.entriesAMax.Add(Tuple.Create(slopeUh, nUhAmaxLower));

				shiftLineSet.LoadStages[loadStage] = entry;
			}
		}

		private double GetAlternativeIfEmpty(DataRow row, string col1, string col2)
		{
			return string.IsNullOrWhiteSpace(row[col1].ToString())
				? row.Field<string>(col2).ToDouble()
				: row.Field<string>(col1).ToDouble();
		}

		public new static string Name
		{
			get { return "AT - Voith"; }
		}


		protected Watt EstimateAccelerrationPower(PerSecond gbxOutSpeed, NewtonMeter gbxOutTorque)
		{
			var vehicleSpeed = DataBus.VehicleInfo.VehicleSpeed;
			var avgSlope =
			((DataBus.DrivingCycleInfo.CycleLookAhead(Constants.SimulationSettings.GearboxLookaheadForAccelerationEstimation).Altitude -
			DataBus.DrivingCycleInfo.Altitude) / Constants.SimulationSettings.GearboxLookaheadForAccelerationEstimation).Value().SI<Radian>();

			var airDragLoss = DataBus.VehicleInfo.AirDragResistance(vehicleSpeed, vehicleSpeed) * DataBus.VehicleInfo.VehicleSpeed;
			var rollResistanceLoss = DataBus.VehicleInfo.RollingResistance(avgSlope) * DataBus.VehicleInfo.VehicleSpeed;

			//DataBus.GearboxLoss();
			var slopeLoss = DataBus.VehicleInfo.SlopeResistance(avgSlope) * DataBus.VehicleInfo.VehicleSpeed;
			var axleLoss = DataBus.AxlegearInfo.AxlegearLoss();

			return gbxOutSpeed * gbxOutTorque - axleLoss - airDragLoss - rollResistanceLoss - slopeLoss;
		}

		#region Overrides of ATShiftStrategy

		public override void Request(Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity)
		{
			driverAcceleration = DataBus.DriverInfo.DriverAcceleration;
			roadGradient = DataBus.DrivingCycleInfo.RoadGradient;
			base.Request(absTime, dt, outTorque, outAngularVelocity);
		}

		public override bool ShiftRequired(
			Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity, NewtonMeter inTorque,
			PerSecond inAngularVelocity, GearshiftPosition gear, Second lastShiftTime, IResponse response)
		{
			var accPower = EstimateAccelerrationPower(outAngularVelocity, outTorque);

			_accMin = (accPower / DataBus.VehicleInfo.VehicleSpeed / (MaxMass + DataBus.WheelsInfo.ReducedMassWheels)).Cast<MeterPerSquareSecond>();
			_accMax = (accPower / DataBus.VehicleInfo.VehicleSpeed / (MinMass + DataBus.WheelsInfo.ReducedMassWheels)).Cast<MeterPerSquareSecond>();

			//var engineLoadPercent = inTorque / FullLoadCurve.FullLoadStationaryTorque(inAngularVelocity);
			var engineLoadPercent = inTorque / response.Engine.DynamicFullLoadTorque;
			_loadStage = GetLoadStage(engineLoadPercent);

			return base.ShiftRequired(
				absTime, dt, outTorque, outAngularVelocity, inTorque, inAngularVelocity, gear, lastShiftTime, response);
		}

		private int GetLoadStage(double engineLoadPercent)
		{
			var sum = 1;
			foreach (var entry in LoadStageSteps) {
				sum += entry.GetOutput(engineLoadPercent * 100);
			}

			return sum;
		}


		protected override bool CheckUpshift(Second absTime, Second dt, NewtonMeter outTorque,
			PerSecond outAngularVelocity, NewtonMeter inTorque, PerSecond inAngularVelocity, GearshiftPosition gear,
			Second lastShiftTime, IResponse response)
		{
			var shiftTimeReached = (absTime - lastShiftTime).IsGreaterOrEqual(GearshiftParams.TimeBetweenGearshifts);
			if (!shiftTimeReached) {
				return false;
			}

			var currentGear = GearboxModelData.Gears[gear.Gear];
			if (dualTCTransmission && gear.Gear == 1) {
				// UPSHIFT - Special rule for 1C -> 2C
				if (!_gearbox.TorqueConverterLocked && GearboxModelData.Gears.ContainsKey(gear.Gear + 1) &&
					GearboxModelData.Gears[gear.Gear + 1].HasTorqueConverter && outAngularVelocity.IsGreater(0)) {
					var result = CheckUpshiftTcTc(absTime, dt, outTorque, outAngularVelocity, inTorque,
						inAngularVelocity, gear, currentGear, response);
					if (result.HasValue) {
						return result.Value;
					}
				}
			} else {


				if (gear.Gear >= GearboxModelData.Gears.Keys.Max()) {
					return false;
				}

				var nextGear = _gearbox.TorqueConverterLocked ? gear.Gear + 1 : gear.Gear;

				var gearIdx = (int)gear.Gear;
				if (_gearbox.TorqueConverterLocked) {
					gearIdx += 1;
				}


				var shiftSpeed = UpshiftLines[gearIdx].LookupShiftSpeed(
					_loadStage, DataBus.DrivingCycleInfo.RoadGradient, DataBus.DriverInfo.DriverAcceleration, _accMin,
					_accMax);
				var shiftSpeedGbxOut = shiftSpeed / GearboxModelData.Gears[nextGear].Ratio;
				if (outAngularVelocity > shiftSpeedGbxOut) {
					Upshift(absTime, gear);
					return true;
				}
			}

			return false;
		}


		protected override bool CheckDownshift(Second absTime, Second dt, NewtonMeter outTorque,
			PerSecond outAngularVelocity, NewtonMeter inTorque, PerSecond inAngularVelocity, GearshiftPosition gear,
			Second lastShiftTime, IResponse response)
		{
			var shiftTimeReached = (absTime - lastShiftTime).IsGreaterOrEqual(GearshiftParams.TimeBetweenGearshifts);
			if (!shiftTimeReached) {
				return false;
			}

			if (gear.Gear == 1) {
				return false;
			}

			var gearIdx = (int)gear.Gear;
			if (_gearbox.TorqueConverterLocked) {
				gearIdx += 1;
			}

			var shiftSpeed = DownshiftLines[gearIdx].LookupShiftSpeed(
				_loadStage, DataBus.DrivingCycleInfo.RoadGradient, DataBus.DriverInfo.DriverAcceleration,
				-0.4.SI<MeterPerSquareSecond>(),
				-0.2.SI<MeterPerSquareSecond>());
			if (inAngularVelocity < shiftSpeed) {
				Downshift(absTime, gear);
				return true;
			}

			return false;
		}

		#region Overrides of BaseShiftStrategy

		public override void WriteModalResults(IModalDataContainer container)
		{
			container.SetDataValue("loadStage", _loadStage);
			container.SetDataValue("accMin", _accMin.Value());
			container.SetDataValue("accMax", _accMax.Value());

			if (_loadStage == 0 || roadGradient == null) {
				container.SetDataValue("S12_UPS", 0);
				container.SetDataValue("S23_UPS", 0);
				container.SetDataValue("S34_UPS", 0);
			} else {
				container.SetDataValue(
					"S12_UPS", UpshiftLines[1].LookupShiftSpeed(_loadStage, roadGradient, driverAcceleration, _accMin, _accMax).AsRPM);
				container.SetDataValue(
					"S23_UPS", UpshiftLines[2].LookupShiftSpeed(_loadStage, roadGradient, driverAcceleration, _accMin, _accMax).AsRPM);
				container.SetDataValue(
					"S34_UPS", UpshiftLines[3].LookupShiftSpeed(_loadStage, roadGradient, driverAcceleration, _accMin, _accMax).AsRPM);
			}
			base.WriteModalResults(container);
		}

		#endregion

		#endregion

		protected class ShiftLinesColumns
		{
			public const string Shift = "shift";
			public const string LoadStage = "loadstage";

			public const string nDhAmin = "n_dh_amin";
			public const string nLevelAmin = "n_level_amin";
			public const string nUhAmin = "n_uh_amin";

			public const string nDhAmax = "n_dh_amax";
			public const string nLevelAmax = "n_level_amax";
			public const string nUhAmax = "n_uh_amax";
		}
	}

	
}
