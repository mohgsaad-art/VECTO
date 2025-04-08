using System;
using System.IO;
using Newtonsoft.Json.Linq;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.InputData.Reader.ComponentData;

namespace TUGraz.VectoCore.InputData.FileIO.JSON
{
	public class JSONBatteryV1 : JSONFile, IBatteryPackEngineeringInputData, ISuperCapEngineeringInputData
	{
		public JSONBatteryV1(JObject data, string filename, bool tolerateMissing = false) : base(data, filename,
			tolerateMissing) { }

		public string Manufacturer => Constants.NOT_AVAILABLE;

		public string Model => Body.GetEx<string>("Model");

		public DateTime Date => DateTime.MinValue;

		public CertificationMethod CertificationMethod => CertificationMethod.NotCertified;

		public string CertificationNumber => Constants.NOT_AVAILABLE;

		public DigestData DigestValue => null;

		public double? MinSOC => Body.GetEx<double>("SOC_min") / 100.0;

		public double? MaxSOC => Body.GetEx<double>("SOC_max") / 100.0;

		public double? DeteriorationPerformanceRatio => null;

        public BatteryType BatteryType { get; }

		AmpereSecond IBatteryPackDeclarationInputData.Capacity => Body.GetEx<double>("Capacity").SI(Unit.SI.Ampere.Hour).Cast<AmpereSecond>();
		public bool? ConnectorsSubsystemsIncluded => Body.ContainsKey("ConnectorsSubsystemsIncluded") && Body.GetEx<bool>("ConnectorsSubsystemsIncluded");
		public bool? JunctionboxIncluded => Body.ContainsKey("JunctionboxIncluded") && Body.GetEx<bool>("JunctionboxIncluded");

		public Kelvin TestingTemperature => Body.ContainsKey("TestingTemperature")
			? Body.GetEx<double>("TestingTemperature").DegCelsiusToKelvin()
			: 20.DegCelsiusToKelvin();

		Farad ISuperCapDeclarationInputData.Capacity => Body.GetEx<double>("Capacity").SI<Farad>();

		public Ohm InternalResistance => Body.GetEx<double>("InternalResistance").SI<Ohm>();

		public Volt MinVoltage => Body.GetEx<double>("U_min").SI<Volt>();

		public Volt MaxVoltage => Body.GetEx<double>("U_max").SI<Volt>();

		public Ampere MaxCurrentCharge => Math.Abs(Body.GetEx<double>("I_maxCharge")).SI<Ampere>();
		public Ampere MaxCurrentDischarge => Math.Abs(Body.GetEx<double>("I_maxDischarge")).SI<Ampere>();

		public TableData InternalResistanceCurve
		{
			get
			{
				try {
					return ReadTableData(Body.GetEx<string>("InternalResistanceCurve"), "InternalResistanceCurve");
				} catch (Exception) {
					if (!TolerateMissing) {
						throw;
					}

					return
						new TableData(
							Path.Combine(BasePath, Body["InternalResistanceCurve"].ToString()) + MissingFileSuffix,
							DataSourceType.Missing);
				}
			}
		}

		public TableData VoltageCurve
		{
			get
			{
				try {
					return ReadTableData(Body.GetEx<string>("SoCCurve"), "SoC Curve");
				} catch (Exception) {
					if (!TolerateMissing) {
						throw;
					}

					return
						new TableData(
							Path.Combine(BasePath, Body["SoCCurve"].ToString()) + MissingFileSuffix,
							DataSourceType.Missing);
				}
			}
		}

		public TableData MaxCurrentMap
		{
			get
			{
				try {
					return ReadTableData(Body.GetEx<string>("MaxCurrentMap"), "Max Current Map");
				} catch (Exception) {
					if (!TolerateMissing) {
						throw;
					}

					return
						new TableData(
							Body["MaxCurrentMap"] == null ? MissingFileSuffix : Path.Combine(BasePath, Body["MaxCurrentMap"].ToString()) + MissingFileSuffix,
							DataSourceType.Missing);
				}
			}
		}


		public REESSType StorageType => Body["REESSType"] == null ? REESSType.Battery : Body.GetEx<string>("REESSType").ParseEnum<REESSType>();
		//public int StreamNumber { get; }
	}
}