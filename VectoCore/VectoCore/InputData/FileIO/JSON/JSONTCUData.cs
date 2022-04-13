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
				DeclarationData.GearboxTCU.TorqueReserveAMT
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

		public double? RatingFactorCurrentGear => Body.GetValueOrDefault<double>("Rating_current_gear");

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

		public double? EffshiftAccelerationFactorAT
		{
			get
			{
				if (Body["EffshiftAccelerationFactorAT"] == null) {
					return null;
				}

				return Body.GetEx<double>("EffshiftAccelerationFactorAT");
			}
		}

		public IList<double> LoadStageThresholdsUp
		{
			get {
				return (Body["LoadStageThresholdsUp"]?.ToString())?.Split(';').Select(x => x.ToDouble(0)).ToList();
			}
		}

		public IList<double> LoadStageThresholdsDown
		{
			get {
				return (Body["LoadStageThresholdsDown"]?.ToString())?.Split(';').Select(x => x.ToDouble(0)).ToList();
			}
		}

		#endregion

	}
}
