/*
* This file is part of VECTO.
*
* Copyright © 2012-2019 European Union
*
* Developed by Graz University of Technology,
*              Institute of Internal Combustion Engines and Thermodynamics,
*              Institute of Technical Informatics
*
* VECTO is licensed under the EUPL, Version 1.1 or - as soon they will be approved
* by the European Commission - subsequent versions of the EUPL (the "Licence");
* You may not use VECTO except in compliance with the Licence.
* You may obtain a copy of the Licence at:
*
* https://joinup.ec.europa.eu/community/eupl/og_page/eupl
*
* Unless required by applicable law or agreed to in writing, VECTO
* distributed under the Licence is distributed on an "AS IS" basis,
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
* See the Licence for the specific language governing permissions and
* limitations under the Licence.
*
* Authors:
*   Stefan Hausberger, hausberger@ivt.tugraz.at, IVT, Graz University of Technology
*   Christian Kreiner, christian.kreiner@tugraz.at, ITI, Graz University of Technology
*   Michael Krisper, michael.krisper@tugraz.at, ITI, Graz University of Technology
*   Raphael Luz, luz@ivt.tugraz.at, IVT, Graz University of Technology
*   Markus Quaritsch, markus.quaritsch@tugraz.at, IVT, Graz University of Technology
*   Martin Rexeis, rexeis@ivt.tugraz.at, IVT, Graz University of Technology
*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json.Linq;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.InputData.Impl;
using TUGraz.VectoCore.Models.Declaration;

namespace TUGraz.VectoCore.InputData.FileIO.JSON
{
	public class JSONGearboxDataV6 : JSONGearboxDataV5
	{
		private IList<ITransmissionInputData> resultGears;

		public JSONGearboxDataV6(JObject data, string filename, bool tolerateMissing = false)
			: base(data, filename, tolerateMissing) {}

		public override GearboxType Type => Body.GetEx<string>(JsonKeys.Gearbox_GearboxType).ParseEnum<GearboxType>();

		public override IList<ITransmissionInputData> Gears
		{
			get {
				if (resultGears == null) { 
					resultGears = new List<ITransmissionInputData>();
					var gears = Body.GetEx(JsonKeys.Gearbox_Gears);
					for (var i = 1; i < gears.Count(); i++) {
						var gear = gears[i];

						resultGears.Add(CreateGear(i, gear));
					}
				}
				return resultGears;
			}
		}

		public override TableData ShiftPolygon
		{
			get {
				if (Body[JsonKeys.Gearbox_TorqueConverter] == null || Body[JsonKeys.Gearbox_TorqueConverter]["ShiftPolygon"] == null) {
					return null;
				}
				var shiftpolygonFile = Body.GetEx(JsonKeys.Gearbox_TorqueConverter)
					.GetEx<string>("ShiftPolygon");
				try {
					return ReadTableData(shiftpolygonFile, "TorqueConverter Shift Polygon");
				} catch (Exception) {
					if (TolerateMissing) {
						return new TableData(Path.Combine(BasePath, shiftpolygonFile) + MissingFileSuffix, DataSourceType.Missing);
					}
				}
				return null;
			}
		}

		public override double AxlegearRatio => Ratio;
	}

	/// <summary>
	///		Represents the Data containing all parameters of the gearbox
	/// </summary>
	/// {
	///  "Header": {
	///    "CreatedBy": "Raphael Luz IVT TU-Graz (85407225-fc3f-48a8-acda-c84a05df6837)",
	///    "Date": "29.07.2014 16:59:17",
	///    "AppVersion": "2.0.4-beta",
	///    "FileVersion": 4
	///  },
	///  "Body": {
	///    "SavedInDeclMode": false,
	///    "ModelName": "Generic 24t Coach",
	///		"GearboxType": "AMT",
	///    "Gears": [
	///      {
	///        "Ratio": 3.240355,
	///        "LossMap": "Axle.vtlm"
	///      },
	///      {
	///        "Ratio": 6.38,
	///        "LossMap": "Indirect GearData.vtlm",
	///      },
	///		...
	///		]
	/// }
	// ReSharper disable once InconsistentNaming
	public class JSONGearboxDataV5 : JSONFile, IGearboxEngineeringInputData, IAxleGearInputData,
		ITorqueConverterEngineeringInputData, IGearshiftEngineeringInputData
	{

		public JSONGearboxDataV5(JObject data, string filename, bool tolerateMissing = false)
			: base(data, filename, tolerateMissing) {}

		#region IAxleGearInputData

		public virtual double Ratio
		{
			[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design",
				"CA1065:DoNotRaiseExceptionsInUnexpectedLocations")]
			get {
				var gears = Body.GetEx(JsonKeys.Gearbox_Gears);
				if (!gears.Any()) {
					throw new VectoSimulationException("At least one Gear-Entry must be defined in Gearbox!");
				}
				return gears[0].GetEx<double>(JsonKeys.Gearbox_Gear_Ratio);
			}
		}

		public TableData LossMap
		{
			[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design",
				"CA1065:DoNotRaiseExceptionsInUnexpectedLocations")]
			get {
				var gears = Body.GetEx(JsonKeys.Gearbox_Gears);
				if (!gears.Any()) {
					throw new VectoSimulationException("At least one Gear-Entry must be defined in Gearbox!");
				}
				var lossMap = gears[0][JsonKeys.Gearbox_Gear_LossMapFile];
				if (lossMap != null) {
					try {
						return ReadTableData(gears[0][JsonKeys.Gearbox_Gear_LossMapFile].Value<string>(), "AxleGear");
					} catch (Exception) {
						if (!TolerateMissing) {
							throw;
						}
						return new TableData(Path.Combine(BasePath, lossMap.Value<string>()) + MissingFileSuffix, DataSourceType.Missing);
					}
				}
				return null;
			}
		}

		public double Efficiency
		{
			[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design",
				"CA1065:DoNotRaiseExceptionsInUnexpectedLocations")]
			get {
				var gears = Body.GetEx(JsonKeys.Gearbox_Gears);
				if (!gears.Any()) {
					throw new VectoSimulationException("At least one Gear-Entry must be defined in Gearbox!");
				}
				return gears[0].GetEx<double>(JsonKeys.Gearbox_Gear_Efficiency);
			}
		}

		public AxleLineType LineType => AxleLineType.SingleReductionAxle;

		#endregion

		#region IGearboxInputData

		public virtual GearboxType Type
		{
			get {
				//return Body.GetEx<string>(JsonKeys.Gearbox_GearboxType).ParseEnum<GearboxType>(); 
				var typeString = Body.GetEx<string>(JsonKeys.Gearbox_GearboxType);
				if (!"AT".Equals(typeString)) {
					return typeString.ParseEnum<GearboxType>();
				}
				var gearRatio = Body.GetEx(JsonKeys.Gearbox_Gears)[1].GetEx<double>(JsonKeys.Gearbox_Gear_Ratio);
				var gearEfficiency = Body.GetEx(JsonKeys.Gearbox_Gears)[1][JsonKeys.Gearbox_Gear_Efficiency].Value<double>();
				if (gearRatio.IsEqual(1) && gearEfficiency.IsEqual(1)) {
					return GearboxType.ATPowerSplit;
				}
				return GearboxType.ATSerial;
			}
		}

		public virtual KilogramSquareMeter Inertia => Body.GetEx<double>(JsonKeys.Gearbox_Inertia).SI<KilogramSquareMeter>();

		public virtual TableData ShiftPolygon
		{
			get {
				try {
					return ReadTableData(Body.GetEx(JsonKeys.Gearbox_Gears)[1].GetEx<string>("ShiftPolygon"),
						"TorqueConverter Shift Polygon", false);
				} catch (Exception) {
					if (!TolerateMissing) {
						throw;
					}
					return
						new TableData(Path.Combine(BasePath, Body[JsonKeys.Gearbox_Gears][1]["ShiftPolygon"].ToString()) +
									MissingFileSuffix, DataSourceType.Missing);
				}
			}
		}

		public Second TractionInterruption => Body.GetEx<double>(JsonKeys.Gearbox_TractionInterruption).SI<Second>();

		public virtual IList<ITransmissionInputData> Gears
		{
			get {
				var resultGears = new List<ITransmissionInputData>();
				var gears = Body.GetEx(JsonKeys.Gearbox_Gears);
				var gearNr = 1;
				for (var i = 1; i < gears.Count(); i++, gearNr++) {
					var gear = gears[i];
					var torqueConverter = gear.GetEx<bool>(JsonKeys.Gearbox_Gear_TCactive);

					if (torqueConverter) {
						resultGears.Add(gears[i + 1].GetEx<bool>(JsonKeys.Gearbox_Gear_TCactive)
							? CreateGear(gearNr, gear)
							: CreateTorqueConverterGear(gearNr, gear, gears[++i]));
					} else {
						resultGears.Add(CreateGear(gearNr, gear));
					}
				}
				return resultGears;
			}
		}

		public bool DifferentialIncluded => false;
		public virtual double AxlegearRatio => double.NaN;

		private TransmissionInputData CreateTorqueConverterGear(int gearNr, JToken gear, JToken nextGear)
		{
			var ratio = gear.GetEx<double>(JsonKeys.Gearbox_Gear_Ratio);
			var efficiency = gear[JsonKeys.Gearbox_Gear_Efficiency] != null
				? gear[JsonKeys.Gearbox_Gear_Efficiency].Value<double>()
				: double.NaN;
			var nextEfficiency = nextGear[JsonKeys.Gearbox_Gear_Efficiency] != null
				? nextGear[JsonKeys.Gearbox_Gear_Efficiency].Value<double>()
				: double.NaN;
			var nextRatio = nextGear.GetEx<double>(JsonKeys.Gearbox_Gear_Ratio);

			if (!((ratio.IsEqual(1) && efficiency.IsEqual(1)) || (!ratio.IsEqual(1) && ratio.IsEqual(nextRatio)))) {
				throw new VectoException(
					"TorqueConverter gear either has to have a ratio of 1 and efficiency of 1, or the ratios of the torque converter gear and the locked gear have to be the same");
			}
			return new TransmissionInputData {
				Gear = gearNr,
				Ratio = nextRatio,
				LossMap = nextGear[JsonKeys.Gearbox_Gear_LossMapFile] != null
					? ReadTableData(nextGear.GetEx<string>(JsonKeys.Gearbox_Gear_LossMapFile),
						$"Gear {gearNr} LossMap")
					: null,
				Efficiency = nextEfficiency,
				MaxTorque = gear["MaxTorque"]?.Value<double>().SI<NewtonMeter>(),
				ShiftPolygon = ReadTableData(gear.GetEx<string>(JsonKeys.Gearbox_Gear_ShiftPolygonFile),
					$"Gear {gearNr} shiftPolygon", false),
			};
		}

		protected TransmissionInputData CreateGear(int gearNumber, JToken gear)
		{
			var retVal = new TransmissionInputData {
				Gear = gearNumber,
				Ratio = gear.GetEx<double>(JsonKeys.Gearbox_Gear_Ratio),
				MaxTorque =
					gear["MaxTorque"] != null && !string.IsNullOrEmpty(gear["MaxTorque"].ToString())
						? gear["MaxTorque"].Value<double>().SI<NewtonMeter>()
						: null,
				MaxInputSpeed = gear["MaxSpeed"] != null && !string.IsNullOrWhiteSpace(gear["MaxSpeed"].ToString())
					? gear["MaxSpeed"].Value<double>().RPMtoRad()
					: null
			};
			var lossMap = gear[JsonKeys.Gearbox_Gear_LossMapFile];
			if (lossMap != null) {
				try {
					retVal.LossMap = ReadTableData(gear.GetEx<string>(JsonKeys.Gearbox_Gear_LossMapFile),
						$"Gear {gearNumber} LossMap");
				} catch (Exception) {
					if (!TolerateMissing) {
						throw;
					}
					retVal.LossMap = new TableData(Path.Combine(BasePath, lossMap.ToString()) + MissingFileSuffix,
						DataSourceType.Missing);
				}
			} else {
				retVal.Efficiency = gear[JsonKeys.Gearbox_Gear_Efficiency] != null
					? gear[JsonKeys.Gearbox_Gear_Efficiency].Value<double>()
					: double.NaN;
			}
			var shiftPolygonFile = gear[JsonKeys.Gearbox_Gear_ShiftPolygonFile];
			if (shiftPolygonFile != null && !string.IsNullOrWhiteSpace(shiftPolygonFile.Value<string>())) {
				try {
					retVal.ShiftPolygon = ReadTableData(gear.GetEx<string>(JsonKeys.Gearbox_Gear_ShiftPolygonFile),
						$"Gear {gearNumber} shiftPolygon");
				} catch (Exception) {
					retVal.ShiftPolygon = new TableData(Path.Combine(BasePath, shiftPolygonFile.Value<string>()) + MissingFileSuffix,
						DataSourceType.Missing);
				}
			}
			//retVal.ShiftPolygon = gear[JsonKeys.Gearbox_Gear_ShiftPolygonFile] != null
			//	? ReadTableData(gear.GetEx<string>(JsonKeys.Gearbox_Gear_ShiftPolygonFile),
			//		string.Format("Gear {0} shiftPolygon", gearNumber), false)
			//	: null;
			return retVal;
		}

		public virtual Second MinTimeBetweenGearshift => Body.GetEx<double>(JsonKeys.Gearbox_ShiftTime).SI<Second>();

		public virtual double TorqueReserve => Body.GetEx<double>(JsonKeys.Gearbox_TorqueReserve) / 100.0;

		public virtual MeterPerSecond StartSpeed => Body.GetEx<double>(JsonKeys.Gearbox_StartSpeed).SI<MeterPerSecond>();

		public virtual MeterPerSquareSecond StartAcceleration => Body.GetEx<double>(JsonKeys.Gearbox_StartAcceleration).SI<MeterPerSquareSecond>();

		public virtual double StartTorqueReserve => Body.GetEx<double>(JsonKeys.Gearbox_StartTorqueReserve) / 100.0;


		public virtual ITorqueConverterEngineeringInputData TorqueConverter => this;

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

		public Second GearResidenceTime => null;
		public double? DnT99LHMin1 => null;
		public double? DnT99LHMin2 => null;
		public int? AllowedGearRangeUp => null;
		public int? AllowedGearRangeDown => null;
		public Second LookBackInterval => null;
		public Watt AvgCardanPowerThresholdPropulsion => null;
		public Watt CurrCardanPowerThresholdPropulsion => null;
		public double? TargetSpeedDeviationFactor => null;
		public double? EngineSpeedHighDriveOffFactor => null;
		public double? RatingFactorCurrentGear => null;
		public TableData AccelerationReserveLookup => null;
		public TableData ShareTorque99L => null;
		public TableData PredictionDurationLookup => null;
		public TableData ShareIdleLow => null;
		public TableData ShareEngineHigh => null;
		public Second DriverAccelerationLookBackInterval => null;
		public MeterPerSquareSecond DriverAccelerationThresholdLow => null;
		public double? RatioEarlyUpshiftFC => null;
		public double? RatioEarlyDownshiftFC => null;
		public int? AllowedGearRangeFC => null;

		public PerSecond MinEngineSpeedPostUpshift => null;
		public Second ATLookAheadTime => null;
		public double[][] ShiftSpeedsTCToLocked => null;
		public double? PEV_TargetSpeedBrakeNorm => null;
		public double? PEV_DownshiftSpeedFactor => null;
		public double? PEV_DeRatingDownshiftSpeedFactor => null;
		public double? PEV_DownshiftMinSpeedFactor => null;

		public double? VeloictyDropFactor => null;

		public double? AccelerationFactor => null;

		public TableData LoadStageShiftLines => null;
		public IList<double> LoadStageThresholdsUp => null;
		public IList<double> LoadStageThresholdsDown => null;

		public Second PowershiftShiftTime =>
			Body["PowershiftShiftTime"] == null
				? Constants.DefaultPowerShiftTime
				: Body.GetEx<double>("PowershiftShiftTime").SI<Second>();

		#endregion

		#region ITorqueConverterInputData

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

		public virtual PerSecond ReferenceRPM =>
			Body[JsonKeys.Gearbox_TorqueConverter] != null &&
			Body[JsonKeys.Gearbox_TorqueConverter][JsonKeys.Gearbox_TorqueConverter_ReferenceRPM] != null
				? Body.GetEx(JsonKeys.Gearbox_TorqueConverter)
					.GetEx<double>(JsonKeys.Gearbox_TorqueConverter_ReferenceRPM)
					.RPMtoRad()
				: null;

		public PerSecond MaxInputSpeed =>
			Body[JsonKeys.Gearbox_TorqueConverter] != null &&
			Body[JsonKeys.Gearbox_TorqueConverter]["MaxTCSpeed"] != null
				? Body.GetEx(JsonKeys.Gearbox_TorqueConverter).GetEx<double>("MaxTCSpeed").RPMtoRad()
				: 5000.RPMtoRad();

		public virtual TableData TCData
		{
			get {
				if (Body[JsonKeys.Gearbox_TorqueConverter] == null ||
					Body[JsonKeys.Gearbox_TorqueConverter][JsonKeys.Gearbox_TorqueConverter_TCMap] == null) {
					return null;
				}
				var tcFile = Body.GetEx(JsonKeys.Gearbox_TorqueConverter).GetEx<string>(JsonKeys.Gearbox_TorqueConverter_TCMap);
				try {
					return ReadTableData(tcFile,
						"TorqueConverter Data");
				} catch (Exception) {
					if (TolerateMissing) {
						return new TableData(Path.Combine(BasePath, tcFile) + MissingFileSuffix, DataSourceType.Missing);
					}
				}
				return null;
			}
		}

		KilogramSquareMeter ITorqueConverterEngineeringInputData.Inertia =>
			Body[JsonKeys.Gearbox_TorqueConverter] != null &&
			Body[JsonKeys.Gearbox_TorqueConverter][JsonKeys.Gearbox_TorqueConverter_Inertia] != null
				? Body.GetEx(JsonKeys.Gearbox_TorqueConverter)
					.GetEx<double>(JsonKeys.Gearbox_TorqueConverter_Inertia)
					.SI<KilogramSquareMeter>()
				: null;

		#endregion

		public string Manufacturer => Constants.NOT_AVAILABLE;

		public string Model => Body.GetEx<string>(JsonKeys.Gearbox_ModelName);

		public DateTime Date => DateTime.MinValue;

		public CertificationMethod CertificationMethod => CertificationMethod.NotCertified;

		public string CertificationNumber => Constants.NOT_AVAILABLE;

		public DigestData DigestValue => null;
	}
}
