using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using Newtonsoft.Json.Linq;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.DataProvider;
using TUGraz.VectoCore.InputData.Impl;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.InputData.FileIO.JSON 
{
	public class JSONElectricMotorV5 : JSONElectricMotorV4
	{
		public JSONElectricMotorV5(JObject data, string filename, bool tolerateMissing = false) : base(data, filename, tolerateMissing) { }

		#region Overrides of JSONElectricMotorV1


		protected override IList<IElectricMotorVoltageLevel> ReadVoltageLevels()
		{
			return Body["VoltageLevels"].Select(entry => new ElectricMotorVoltageLevel() {
				VoltageLevel = entry.GetEx<double>("Voltage").SI<Volt>(),
				ContinuousTorque = entry.GetEx<double>("ContinuousTorque").SI<NewtonMeter>(),
				ContinuousTorqueSpeed = entry.GetEx<double>("ContinuousTorqueSpeed").RPMtoRad(),
				OverloadTorque = entry.GetEx<double>("OverloadTorque").SI<NewtonMeter>(),
				OverloadTestSpeed = entry.GetEx<double>("OverloadTorqueSpeed").RPMtoRad(),
				OverloadTime = entry.GetValueOrDefault<double>("OverloadTime")?.SI<Second>() ?? 0.SI<Second>(),
				PowerMap = ReadPowermap(entry),
				// DragCurve = ReadTableData(entry.GetEx<string>("DragCurve"), "ElectricMotor DragCurve"),
				FullLoadCurve = ReadTableData(entry.GetEx<string>("FullLoadCurve"), "ElectricMotor FullLoadCurve")
			}).Cast<IElectricMotorVoltageLevel>().ToList();
		}

		protected virtual IList<IElectricMotorPowerMap> ReadPowermap(JToken entry)
		{
			//if (entry.)
			var powermap = entry["EfficiencyMap"];

			if (!powermap.HasValues) {
				return new List<IElectricMotorPowerMap>() {
					new JSONElectricMotorPowerMap() {
						Gear = 0,
						PowerMap = ReadTableData(entry.GetEx<string>("EfficiencyMap"), "ElectricMotor Map")
					}
				};
			}

			return powermap.Select(x => new JSONElectricMotorPowerMap() {
				Gear = (((JProperty) x).Name).ToInt(),
				PowerMap = ReadTableData((x as JProperty).Value.Value<string>(), "ElectricMotor Map")
			}).Cast<IElectricMotorPowerMap>().ToList();
		}

		#endregion
	}

	// -----------------------------

	public class JSONElectricMotorPowerMap : IElectricMotorPowerMap
	{
		#region Implementation of IElectricMotorPowerMap

		public int Gear { get; set; }
		public TableData PowerMap { get; set; }

		#endregion
	}

	public class JSONElectricMotorV4 : JSONElectricMotorV3
	{
		public JSONElectricMotorV4(JObject data, string filename, bool tolerateMissing = false) : base(data, filename, tolerateMissing) { }

		public override TableData DragCurve => ReadTableData(Body.GetEx<string>("DragCurve"), "ElectricMotor DragCurve");

	}

	public class JSONElectricMotorV3 : JSONElectricMotorV2
	{
		public JSONElectricMotorV3(JObject data, string filename, bool tolerateMissing = false) : base(data, filename, tolerateMissing) { }

		#region Overrides of JSONElectricMotorV1


		protected override IList<IElectricMotorVoltageLevel> ReadVoltageLevels()
		{
			return Body["VoltageLevels"].Select(entry => new ElectricMotorVoltageLevel() {
				VoltageLevel = entry.GetEx<double>("Voltage").SI<Volt>(),
				ContinuousTorque = Body.GetEx<double>("ContinuousTorque").SI<NewtonMeter>(),
				ContinuousTorqueSpeed = Body.GetEx<double>("ContinuousTorqueSpeed").RPMtoRad(),
				OverloadTorque = Body.GetEx<double>("OverloadTorque").SI<NewtonMeter>(),
				OverloadTestSpeed = Body.GetEx<double>("OverloadTorqueSpeed").RPMtoRad(),
				OverloadTime = Body.GetValueOrDefault<double>("OverloadTime")?.SI<Second>() ?? 0.SI<Second>(),
				PowerMap = new List<IElectricMotorPowerMap>() {
					new JSONElectricMotorPowerMap() {
						Gear = 0, 
						PowerMap = ReadTableData(entry.GetEx<string>("EfficiencyMap"), "ElectricMotor Map")
					}
				},
				// DragCurve = ReadTableData(entry.GetEx<string>("DragCurve"), "ElectricMotor DragCurve"),
				FullLoadCurve = ReadTableData(entry.GetEx<string>("FullLoadCurve"), "ElectricMotor FullLoadCurve")
			}).Cast<IElectricMotorVoltageLevel>().ToList();
		}

		public override TableData DragCurve=> Body["VoltageLevels"].Select(x => ReadTableData(x.GetEx<string>("DragCurve"), "ElectricMotor DragCurve")).First();
		

		#endregion
	}

	// -----------------------------

	public class JSONElectricMotorV2 : JSONElectricMotorV1
	{
		public JSONElectricMotorV2(JObject data, string filename, bool tolerateMissing = false) : base(data, filename, tolerateMissing) { }

		protected override IList<IElectricMotorVoltageLevel> ReadVoltageLevels()
		{
			return new List<IElectricMotorVoltageLevel>() {
				new ElectricMotorVoltageLevel() {
					VoltageLevel = 0.SI<Volt>(),
					ContinuousTorque = Body.GetEx<double>("ContinuousTorque").SI<NewtonMeter>(),
					ContinuousTorqueSpeed = Body.GetEx<double>("ContinuousTorqueSpeed").RPMtoRad(),
					OverloadTorque = Body.GetEx<double>("OverloadTorque").SI<NewtonMeter>(),
					OverloadTestSpeed = Body.GetEx<double>("OverloadTorqueSpeed").RPMtoRad(),
					OverloadTime = Body.GetValueOrDefault<double>("OverloadTime")?.SI<Second>() ?? 0.SI<Second>(),
					PowerMap = new List<IElectricMotorPowerMap>() {
						new JSONElectricMotorPowerMap() {
							Gear = 0,
							PowerMap = ReadTableData(Body.GetEx<string>("EfficiencyMap"), "ElectricMotor Map")
						}
					},// DragCurve = ReadTableData(Body.GetEx<string>("DragCurve"), "ElectricMotor DragCurve"),
					FullLoadCurve = ReadTableData(Body.GetEx<string>("FullLoadCurve"), "ElectricMotor FullLoadCurve")
				},
				new ElectricMotorVoltageLevel() {
					VoltageLevel = 1e9.SI<Volt>(),
					ContinuousTorque = Body.GetEx<double>("ContinuousTorque").SI<NewtonMeter>(),
					ContinuousTorqueSpeed = Body.GetEx<double>("ContinuousTorqueSpeed").RPMtoRad(),
					OverloadTorque = Body.GetEx<double>("OverloadTorque").SI<NewtonMeter>(),
					OverloadTestSpeed = Body.GetEx<double>("OverloadTorqueSpeed").RPMtoRad(),
					OverloadTime = Body.GetValueOrDefault<double>("OverloadTime")?.SI<Second>() ?? 0.SI<Second>(),
					PowerMap = new List<IElectricMotorPowerMap>() {
						new JSONElectricMotorPowerMap() {
							Gear = 0,
							PowerMap = ReadTableData(Body.GetEx<string>("EfficiencyMap"), "ElectricMotor Map")
						}
					},
					// DragCurve = ReadTableData(Body.GetEx<string>("DragCurve"), "ElectricMotor DragCurve"),
					FullLoadCurve = ReadTableData(Body.GetEx<string>("FullLoadCurve"), "ElectricMotor FullLoadCurve")
				},
			};
		}

	}


	public class JSONElectricMotorV1 : JSONFile, IElectricMotorEngineeringInputData
	{
		private IList<IElectricMotorVoltageLevel> _voltageLevels;
		public JSONElectricMotorV1(JObject data, string filename, bool tolerateMissing = false) : base(data, filename, tolerateMissing) { }
		public virtual string Manufacturer => Constants.NOT_AVAILABLE;

		public virtual string Model => Body.GetEx<string>("Model");
		public virtual IList<IElectricMotorVoltageLevel> VoltageLevels => _voltageLevels ?? (_voltageLevels = ReadVoltageLevels());
		public virtual DateTime Date => DateTime.MinValue;

		public CertificationMethod CertificationMethod => CertificationMethod.NotCertified;

		public string CertificationNumber => Constants.NOT_AVAILABLE;

		protected virtual IList<IElectricMotorVoltageLevel> ReadVoltageLevels()
		{
			return new List<IElectricMotorVoltageLevel>() {
				new ElectricMotorVoltageLevel() {
					VoltageLevel = 0.SI<Volt>(),
					ContinuousTorque = (Body.GetValueOrDefault<double>("ContinuousPower")?.SI<Watt>() ?? 1e12.SI<Watt>()) / (Body.GetValueOrDefault<double>("ContinuousPowerSpeed")?.RPMtoRad() ?? 1.SI<PerSecond>()),
					ContinuousTorqueSpeed = Body.GetValueOrDefault<double>("ContinuousPowerSpeed")?.RPMtoRad() ?? 0.RPMtoRad(),
					OverloadTorque = null,
					OverloadTestSpeed = null,
					OverloadTime = Body.GetValueOrDefault<double>("OverloadTime")?.SI<Second>() ?? 0.SI<Second>(),
					PowerMap = new List<IElectricMotorPowerMap>() {
						new JSONElectricMotorPowerMap() {
							Gear = 0,
							PowerMap = ReadTableData(Body.GetEx<string>("EfficiencyMap"), "ElectricMotor Map")
						}
					},
					// DragCurve = ReadTableData(Body.GetEx<string>("DragCurve"), "ElectricMotor DragCurve"),
					FullLoadCurve = ReadTableData(Body.GetEx<string>("FullLoadCurve"), "ElectricMotor FullLoadCurve")
				},
				new ElectricMotorVoltageLevel() {
					VoltageLevel = 1e9.SI<Volt>(),
					ContinuousTorque = (Body.GetValueOrDefault<double>("ContinuousPower")?.SI<Watt>() ?? 1e12.SI<Watt>()) / (Body.GetValueOrDefault<double>("ContinuousPowerSpeed")?.RPMtoRad() ?? 1.SI<PerSecond>()),
					ContinuousTorqueSpeed = Body.GetValueOrDefault<double>("ContinuousPowerSpeed")?.RPMtoRad() ?? 0.RPMtoRad(),
					OverloadTorque = null,
					OverloadTestSpeed = null,
					OverloadTime = Body.GetValueOrDefault<double>("OverloadTime")?.SI<Second>() ?? 0.SI<Second>(),
					PowerMap = new List<IElectricMotorPowerMap>() {
						new JSONElectricMotorPowerMap() {
							Gear = 0,
							PowerMap = ReadTableData(Body.GetEx<string>("EfficiencyMap"), "ElectricMotor Map")
						}
					},
					// DragCurve = ReadTableData(Body.GetEx<string>("DragCurve"), "ElectricMotor DragCurve"),
					FullLoadCurve = ReadTableData(Body.GetEx<string>("FullLoadCurve"), "ElectricMotor FullLoadCurve")
				},
			};
		}

		public DigestData DigestValue => null;

		//public virtual TableData FullLoadCurve => ReadTableData(Body.GetEx<string>("FullLoadCurve"), "ElectricMotor FullLoadCurve");

		public virtual TableData DragCurve => ReadTableData(Body.GetEx<string>("DragCurve"), "ElectricMotor DragCurve");

		public virtual TableData Conditioning =>
			ReadTableData(Body.GetEx<string>("Conditioning"), "ElectricMotor Conditioning", false);


		public ElectricMachineType ElectricMachineType { get; }
		public Watt R85RatedPower => null;
		public virtual KilogramSquareMeter Inertia => Body.GetEx<double>("Inertia").SI<KilogramSquareMeter>();

		//public virtual Joule OverloadBuffer => Body.GetValueOrDefault<double>("ThermalOverloadBuffer")?.SI(Unit.SI.Mega.Joule).Cast<Joule>() ?? 1e18.SI<Joule>();

		public string IHPCType => null;
		public virtual double OverloadRecoveryFactor => Body.GetValueOrDefault<double>("ThermalOverloadRecoveryFactor") ?? 0.9;

		public bool DcDcConverterIncluded { get; }
	}
}