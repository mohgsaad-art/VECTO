using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.Impl;
using TUGraz.VectoCore.Configuration;

namespace TUGraz.VectoCore.InputData.FileIO.JSON
{
	public class JSONIEPCData : JSONFile, IIEPCEngineeringInputData
	{
		private readonly JObject _header;

		public JSONIEPCData(JObject data, string filename, bool tolerateMissing = false)
			: base(data, filename, tolerateMissing)
		{
			_header = (JObject)data.GetEx(JsonKeys.JsonHeader);
		}

		#region Overrides of JSONFile

		public override string AppVersion => _header.GetEx<string>(JsonKeys.AppVersion);
	
		#endregion
		
		#region Implementation of IComponentInputData
		
		public string Manufacturer => Constants.NOT_AVAILABLE;
		public string Model => Body.GetEx<string>(JsonKeys.Component_Model);

		public DateTime Date => DateTime.MinValue;

		public CertificationMethod CertificationMethod => CertificationMethod.NotCertified;
		public string CertificationNumber => Constants.NOT_AVAILABLE;
		public DigestData DigestValue => null;

		#endregion

		#region Implementation of IIEPCDeclarationInputData

		public ElectricMachineType ElectricMachineType { get; }
		public Watt R85RatedPower => null;
		public KilogramSquareMeter Inertia => Body.GetEx<double>(JsonKeys.IEPC_Inertia).SI<KilogramSquareMeter>();
		public bool DifferentialIncluded => Body.GetEx<bool>(JsonKeys.IEPC_DifferentialIncluded);
		public bool DesignTypeWheelMotor => Body.GetEx<bool>(JsonKeys.IEPC_DesignTypeWheelMotor);
		public int? NrOfDesignTypeWheelMotorMeasured => Body.GetEx<int?>(JsonKeys.IEPC_NrOfDesignTypeWheelMotorMeasured);
		public IList<IGearEntry> Gears => ReadGearEntries(Body[JsonKeys.Gearbox_Gears ]);
		public IList<IElectricMotorVoltageLevel> VoltageLevels => ReadVoltageLevel(Body[JsonKeys.IEPC_VoltageLevels]);
		public IList<IDragCurve> DragCurves => ReadDragCurve(Body[JsonKeys.IEPC_DragCurves]);
		public TableData Conditioning => null;

		#endregion

		#region Implementation of IIEPCEngineeringInputData

		public double OverloadRecoveryFactor => Body.GetEx<double>(JsonKeys.IEPC_ThermalOverloadRecoveryFactor);

		#endregion


		protected virtual IList<IGearEntry> ReadGearEntries(JToken gears)
		{
			var gearData = new List<IGearEntry>();
			var gearNumber = 1;
			foreach (var gear in gears) {
				gearData.Add(new GearEntry {
					GearNumber = gearNumber,
					Ratio = gear.GetEx<double>(JsonKeys.Gearbox_Gear_Ratio),
					MaxOutputShaftTorque = gear[JsonKeys.Gearbox_Gear_MaxOutShaftTorque] == null ? null : gear.GetEx<double>(JsonKeys.Gearbox_Gear_MaxOutShaftTorque).SI<NewtonMeter>(),
					MaxOutputShaftSpeed = gear[JsonKeys.Gearbox_Gear_MaxOutShaftSpeed] == null ? null : gear.GetEx<double>(JsonKeys.Gearbox_Gear_MaxOutShaftSpeed).SI<PerSecond>()
				});

				gearNumber++;
			}
			return gearData;
		}

		protected virtual IList<IElectricMotorVoltageLevel> ReadVoltageLevel(JToken voltageLevels)
		{
			var voltageLevelData = new List<IElectricMotorVoltageLevel>();
			foreach (var voltageLevel in voltageLevels) {
				voltageLevelData.Add(new ElectricMotorVoltageLevel {

					VoltageLevel = voltageLevel.GetEx<double>(JsonKeys.IEPC_Voltage).SI<Volt>(),
					ContinuousTorque = voltageLevel.GetEx<double>(JsonKeys.IEPC_ContinuousTorque).SI<NewtonMeter>(),
					ContinuousTorqueSpeed = voltageLevel.GetEx<double>(JsonKeys.IEPC_ContinuousTorqueSpeed).RPMtoRad(),
					OverloadTorque = voltageLevel.GetEx<double>(JsonKeys.IEPC_OverloadTorque).SI<NewtonMeter>(),
					OverloadTestSpeed = voltageLevel.GetEx<double>(JsonKeys.IEPC_OverloadTorqueSpeed).RPMtoRad(),
					OverloadTime = voltageLevel.GetValueOrDefault<double>(JsonKeys.IEPC_OverloadTime)?.SI<Second>() ?? 0.SI<Second>(),
					FullLoadCurve = ReadTableData(voltageLevel.GetEx<string>(JsonKeys.IEPC_FullLoadCurve), "ElectricMotor FullLoadCurve"),
					PowerMap = ReadPowerMap(voltageLevel[JsonKeys.IEPC_PowerMaps])
				});
			}

			return voltageLevelData;
		}
		
		protected virtual IList<IElectricMotorPowerMap> ReadPowerMap(JToken powerMapEntry)
		{
			var powerMaps = new List<IElectricMotorPowerMap>();

			foreach (var powerMap in powerMapEntry) {
				var key = ((JProperty)powerMap).Name;
				var value = ((JProperty)powerMap).Value.Value<string>();

				powerMaps.Add(new JSONElectricMotorPowerMap {
					Gear = Convert.ToInt32(key),
					PowerMap = ReadTableData(value, "ElectricMotor Power Map")
				});
			}

			return powerMaps;
		}

		protected virtual IList<IDragCurve> ReadDragCurve(JToken dragCurveEntry)
		{
			var dragCurves = new List<IDragCurve>();

			foreach (var dragCurve in dragCurveEntry) {
				
				var key = ((JProperty)dragCurve).Name;
				var value = ((JProperty)dragCurve).Value.Value<string>();

				dragCurves.Add(new DragCurveEntry {
					Gear = Convert.ToInt32(key),
					DragCurve = ReadTableData(value, "ElectricMotor Drag Curve")
				});
			}
			
			return dragCurves;
		}
	}
	
	public class GearEntry : IGearEntry
	{
		#region Implementation of IGearEntry

		public int GearNumber { get; internal set; }
		public double Ratio { get; internal set; }
		public NewtonMeter MaxOutputShaftTorque { get; internal set; }
		public PerSecond MaxOutputShaftSpeed { get; internal set; }

		#endregion
	}

	public class DragCurveEntry : IDragCurve
	{
		#region Implementation of IDragCurve

		public int? Gear { get; internal set; }
		public TableData DragCurve { get; internal set; }

		#endregion
	}
}
