using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json.Linq;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.Declaration;

namespace TUGraz.VectoCore.InputData.FileIO.JSON
{
	public class JSONTCUDataV1 : JSONFile, IGearshiftEngineeringInputData
	{
		public JSONTCUDataV1(JObject json, string filename, bool tolerateMissing) : base(json, filename, tolerateMissing) { }



		#region Implementation of IGearshiftEngineeringInputData

		public virtual Second MinTimeBetweenGearshift =>
			Body[JsonKeys.Gearbox_ShiftTime] == null
				? DeclarationData.Gearbox.MinTimeBetweenGearshifts
				: Body.GetEx<double>(JsonKeys.Gearbox_ShiftTime).SI<Second>();

		public virtual double TorqueReserve =>
			Body[JsonKeys.Gearbox_TorqueReserve] == null ?
				DeclarationData.GearboxTCU.TorqueReserve
				:Body.GetEx<double>(JsonKeys.Gearbox_TorqueReserve) / 100.0;

		public Second DownshiftAfterUpshiftDelay =>
			Body["DownshiftAfterUpshiftDelay"] == null
				? DeclarationData.Gearbox.DownshiftAfterUpshiftDelay
				: Body.GetEx<double>("DownshiftAfterUpshiftDelay").SI<Second>();

		public Second UpshiftAfterDownshiftDelay =>
			Body["UpshiftAfterDownshiftDelay"] == null
				? DeclarationData.Gearbox.UpshiftAfterDownshiftDelay
				: Body.GetEx<double>("UpshiftAfterDownshiftDelay").SI<Second>();

		public MeterPerSquareSecond UpshiftMinAcceleration =>
			Body["UpshiftMinAcceleration"] == null
				? DeclarationData.Gearbox.UpshiftMinAcceleration
				: Body.GetEx<double>("UpshiftMinAcceleration").SI<MeterPerSquareSecond>();

		public MeterPerSquareSecond CLUpshiftMinAcceleration =>
			Body[JsonKeys.Gearbox_TorqueConverter] != null &&
			Body[JsonKeys.Gearbox_TorqueConverter]["CLUpshiftMinAcceleration"] != null
				? Body.GetEx(JsonKeys.Gearbox_TorqueConverter).GetEx<double>("CLUpshiftMinAcceleration").SI<MeterPerSquareSecond>()
				: UpshiftMinAcceleration;

		public MeterPerSquareSecond CCUpshiftMinAcceleration =>
			Body[JsonKeys.Gearbox_TorqueConverter] != null &&
			Body[JsonKeys.Gearbox_TorqueConverter]["CCUpshiftMinAcceleration"] != null
				? Body.GetEx(JsonKeys.Gearbox_TorqueConverter).GetEx<double>("CCUpshiftMinAcceleration").SI<MeterPerSquareSecond>()
				: UpshiftMinAcceleration;

		public virtual double StartTorqueReserve =>
			Body[JsonKeys.Gearbox_StartTorqueReserve] == null
				? DeclarationData.GearboxTCU.TorqueReserveStart
				: Body.GetEx<double>(JsonKeys.Gearbox_StartTorqueReserve) / 100.0;

		public MeterPerSecond StartSpeed => Body.GetValueOrDefault<double>(JsonKeys.Gearbox_StartSpeed)?.KMPHtoMeterPerSecond() ?? DeclarationData.GearboxTCU.StartSpeed;

		public MeterPerSquareSecond StartAcceleration => Body.GetValueOrDefault<double>(JsonKeys.Gearbox_StartAcceleration)?.SI<MeterPerSquareSecond>() ?? DeclarationData.GearboxTCU.StartAcceleration;

		public Second GearResidenceTime => Body.GetValueOrDefault<double>("GearResidenceTime")?.SI<Second>();

		public double? DnT99LHMin1 => Body.GetValueOrDefault<double>("Dn_Tq99L_high_min_1");

		public double? DnT99LHMin2 => Body.GetValueOrDefault<double>("Dn_Tq99L_high_min_2");

		public int? AllowedGearRangeUp => Body.GetValueOrDefault<int>("GearRangeUp");

		public int? AllowedGearRangeDown => Body.GetValueOrDefault<int>("GearRangeDown");

		public Second LookBackInterval => Body.GetValueOrDefault<double>("LookBackDriver")?.SI<Second>();

		public Watt AvgCardanPowerThresholdPropulsion => Body.GetValueOrDefault<double>("P_card_avg_threshold")?.SI<Watt>();

		public Watt CurrCardanPowerThresholdPropulsion => Body.GetValueOrDefault<double>("P_card_curr_threshold")?.SI<Watt>();

		public double? TargetSpeedDeviationFactor => Body.GetValueOrDefault<double>("Diff_curr_targ_vel");

		public double? EngineSpeedHighDriveOffFactor => Body.GetValueOrDefault<double>("EngineSpeedHighDriveOffFactor");

		public double? RatingFactorCurrentGear => Body.GetValueOrDefault<double>("Rating_current_gear");

		public TableData AccelerationReserveLookup
		{
			get {
				try {
					return ReadTableData(Body["AccelerationReserveLookup"]?.ToString(), "AccelerationReserveLookup", false);
				} catch (Exception) {
					if (!TolerateMissing) {
						throw;
					}

					return new TableData(
						Path.Combine(BasePath, Body["AccelerationReserveLookup"]?.ToString() ?? "") + MissingFileSuffix,
						DataSourceType.Missing);
				}
			}
		}

		public TableData ShareTorque99L
		{
			get {
				try {
					return ReadTableData(Body["ShareTorque99L"]?.ToString(), "ShareTorque99L", false);
				} catch (Exception) {
					if (!TolerateMissing) {
						throw;
					}

					return new TableData(
						Path.Combine(BasePath, Body["ShareTorque99L"]?.ToString() ?? "") + MissingFileSuffix,
						DataSourceType.Missing);
				}
			}
		}

		public TableData PredictionDurationLookup
		{
			get {
				try {
					return ReadTableData(Body["PredictionDurationLookup"]?.ToString(), "PredictionDurationLookup", false);
				} catch (Exception) {
					if (!TolerateMissing) {
						throw;
					}

					return new TableData(
						Path.Combine(BasePath, Body["PredictionDurationLookup"]?.ToString() ?? "") + MissingFileSuffix,
						DataSourceType.Missing);
				}
			}
		}

		public TableData ShareIdleLow
		{
			get {
				try {
					return ReadTableData(Body["ShareIdleLow"]?.ToString(), "ShareIdleLow", false);
				} catch (Exception) {
					if (!TolerateMissing) {
						throw;
					}

					return new TableData(
						Path.Combine(BasePath, Body["ShareIdleLow"]?.ToString() ?? "") + MissingFileSuffix,
						DataSourceType.Missing);
				}
			}
		}

		public TableData ShareEngineHigh
		{
			get {
				try {
					return ReadTableData(Body["ShareEngineHigh"]?.ToString(), "ShareEngineHigh", false);
				} catch (Exception) {
					if (!TolerateMissing) {
						throw;
					}

					return new TableData(
						Path.Combine(BasePath, Body["ShareEngineHigh"]?.ToString() ?? "") + MissingFileSuffix,
						DataSourceType.Missing);
				}
			}
		}

		public Second DriverAccelerationLookBackInterval => Body.GetValueOrDefault<double>("DriverAccelerationLookBackInterval")?.SI<Second>();
		public MeterPerSquareSecond DriverAccelerationThresholdLow => Body.GetValueOrDefault<double>("DriverAccelerationThresholdLow")?.SI<MeterPerSquareSecond>();

		public double? RatioEarlyUpshiftFC => Body.GetValueOrDefault<double>("RatioEarlyUpshiftFC");

		public double? RatioEarlyDownshiftFC => Body.GetValueOrDefault<double>("RatioEarlyDownshiftFC");

		public int? AllowedGearRangeFC
		{
			get {
				if (Body["AllowedGearRangeFC"] == null) {
					return null;
				}

				return Body.GetEx<int>("AllowedGearRangeFC");
			}
		}

		public double? VeloictyDropFactor
		{
			get {
				if (Body["VelocityDropFactor"] == null) {
					return null;
				}

				return Body.GetEx<double>("VelocityDropFactor");
			}
		}

		public double? AccelerationFactor
		{
			get {
				if (Body["AccelerationFactorNP98h"] == null) {
					return null;
				}

				return Body.GetEx<double>("AccelerationFactorNP98h");
			}
		}

		public PerSecond MinEngineSpeedPostUpshift
		{
			get {
				if (Body["MinEngineSpeedPostUpshift"] == null) {
					return null;
				}

				return Body.GetEx<double>("MinEngineSpeedPostUpshift").RPMtoRad();
			}
		}

		public Second ATLookAheadTime
		{
			get {
				if (Body["ATLookAheadTime"] == null) {
					return null;
				}

				return Body.GetEx<double>("ATLookAheadTime").SI<Second>();
			}
		}

		public double[][] ShiftSpeedsTCToLocked
		{
			get {
				if (Body["ShiftSpeedsTCLockup"] == null) {
					return null;
				}

				var retVal = new List<double[]>();
				foreach (var entry in Body["ShiftSpeedsTCLockup"]) {
					if (!(entry is JArray)) {
						throw new VectoException("Array expected");
					}

					retVal.Add(entry.Select(shiftSpeed => shiftSpeed.Value<double>()).ToArray());
				}

				return retVal.ToArray();
			}
		}

		public double? PEV_TargetSpeedBrakeNorm
		{
			get
			{
				if (Body["PEV_TargetSpeedBrakeNorm"] != null) {
					return Body.GetEx<double>("PEV_TargetSpeedBrakeNorm");
				}

				return null;
			}
		}

		public double? PEV_DownshiftSpeedFactor
		{
			get
			{
				if (Body["PEV_DownshiftSpeedFactor"] != null) {
					return Body.GetEx<double>("PEV_DownshiftSpeedFactor");
				}

				return null;
			}
		}

		public double? PEV_DeRatingDownshiftSpeedFactor
		{
			get
			{
				if (Body["PEV_DeRatingDownshiftSpeedFactor"] != null) {
					return Body.GetEx<double>("PEV_DeRatingDownshiftSpeedFactor");
				}

				return null;
			}
		}

		public double? PEV_DownshiftMinSpeedFactor
		{
			get
			{
				if (Body["PEV_DownshiftMinSpeedFactor"] != null) {
					return Body.GetEx<double>("PEV_DownshiftMinSpeedFactor");
				}

				return null;
			}
		}

		public TableData LoadStageShiftLines
		{
			get {
				try {
					return ReadTableData(Body["LoadStageShiftLines"]?.ToString(), "LoadStageShiftLines", false);
				} catch (Exception) {
					return null;
				}
			}
		}

		public IList<double> LoadStageThresholdsUp
		{
			get {
				return (Body["LoadStageThresoldsUp"]?.ToString())?.Split(';').Select(x => x.ToDouble(0)).ToList();
			}
		}

		public IList<double> LoadStageThresholdsDown
		{
			get {
				return (Body["LoadStageThresoldsDown"]?.ToString())?.Split(';').Select(x => x.ToDouble(0)).ToList();
			}
		}

		#endregion

	}
}
