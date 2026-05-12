using System;
using System.Collections.Generic;
using System.Data;
using System.Xml;
using System.Xml.Linq;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.Reader.ComponentData;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.ManufacturerReport.ManufacturerReport_0_9.ManufacturerReportXMLTypeWriter;

namespace TUGraz.VectoCore.OutputData.XML.DeclarationReports.VehicleInformationFile.VehicleInformationFile_0_1.Components
{
	public  class VIFElectricEnergyStorageType : AbstractVIFXmlType, IXmlTypeWriter
	{
		public VIFElectricEnergyStorageType(IVIFReportFactory vifFactory) : base(vifFactory) { }

		#region Implementation of IXmlTypeWriter

		public XElement GetElement(IDeclarationInputDataProvider inputData)
		{
			var electricStorages = inputData.JobInputData.Vehicle.Components.ElectricStorage.ElectricStorageElements;
			if (electricStorages == null)
				return null;

			var result = new List<XElement>();
			
			foreach (var entry in electricStorages) {
				switch (entry.REESSPack.StorageType) {
					case REESSType.SuperCap:
						result.Add(GetCapacitor(entry));
						break;
					case REESSType.Battery:
						result.Add(GetBattery(entry));
						break;
				}
			}
			
			return new XElement(_vif + XMLNames.Component_ElectricEnergyStorage, result);
		}

		#endregion

		private XElement GetCapacitor(IElectricStorageDeclarationInputData reess)
		{
			if(reess  == null)
				return null;

			var supercap = reess.REESSPack as ISuperCapDeclarationInputData;
			if (supercap == null) {
				throw new VectoException("Electric energy storage of type 'Capacitor' requires capacitor component", ErrorCodes.REESS_SUPERCAP_MISSING);
			}

			return new XElement(_vif + XMLNames.ElectricEnergyStorage_Capacitor,
					new XElement(_vif + XMLNames.ComponentDataWrapper,
						new XAttribute(_xsi + XMLNames.XSIType, "CapacitorSystemDataType"),
					new XElement(_vif + XMLNames.Component_Manufacturer, supercap.Manufacturer),
					new XElement(_vif + XMLNames.Component_Model, supercap.Model),
					new XElement(_vif + XMLNames.Component_CertificationMethod, supercap.CertificationMethod.ToXMLFormat()),
					supercap.CertificationMethod == CertificationMethod.StandardValues 
						? null 
						: new XElement(_vif + XMLNames.Report_Component_CertificationNumber, supercap.CertificationNumber), 
					new XElement(_vif + XMLNames.Component_Date, XmlConvert.ToString(supercap.Date, XmlDateTimeSerializationMode.Utc)),
					new XElement(_vif + XMLNames.Component_AppVersion, supercap.AppVersion),
					new XElement(_vif + XMLNames.Capacitor_Capacitance, supercap.Capacity.ToXMLFormat(2)),
					new XElement(_vif + XMLNames.Capacitor_MinVoltage, supercap.MinVoltage.ToXMLFormat(2)),
					new XElement(_vif + XMLNames.Capacitor_MaxVoltage, supercap.MaxVoltage.ToXMLFormat(2)),
					new XElement(_vif + XMLNames.Capacitor_MaxChargingCurrent, supercap.MaxCurrentCharge.ToXMLFormat(2)),
					new XElement(_vif + XMLNames.Capacitor_MaxDischargingCurrent, supercap.MaxCurrentDischarge.ToXMLFormat(2)),
						supercap.TestingTemperature == null 
						? null 
						: new XElement(_vif + XMLNames.REESS_TestingTemperature, supercap.TestingTemperature.AsDegCelsius.ToXMLFormat(0))
					));
		}


		private  XElement GetBattery(IElectricStorageDeclarationInputData reess)
		{
			if (reess == null)
				return null;
			var battery = reess.REESSPack as IBatteryPackDeclarationInputData;
			if (battery == null) {
				throw new VectoException("Electric energy storage of type 'Battery' requires battery component", ErrorCodes.REESS_BATTERY_MISSING);
			}


			var result = new XElement(_vif + XMLNames.ElectricEnergyStorage_Battery,
				new XElement(_vif + XMLNames.Battery_StringID, reess.StringId),
				new XElement(_vif + "REESS", GetReess(battery)),
				battery.MinSOC.HasValue ? new XElement(_vif + XMLNames.Battery_SOCmin, Math.Round(battery.MinSOC.Value * 100, MidpointRounding.AwayFromZero)) : null,
				battery.MaxSOC.HasValue ? new XElement(_vif + XMLNames.Battery_SOCmax, Math.Round(battery.MaxSOC.Value * 100, MidpointRounding.AwayFromZero)) : null,
				battery.DeteriorationPerformanceRatio.HasValue 
					? new XElement(_vif + "DeteriorationPerformanceRatio", Math.Round(battery.DeteriorationPerformanceRatio.Value * 100, MidpointRounding.AwayFromZero)) 
					: null
			);
			
			return result;
		}


		private XElement GetReess(IBatteryPackDeclarationInputData battery)
		{
			var stdValues = battery.CertificationMethod == CertificationMethod.StandardValues;
			return new XElement(_vif + XMLNames.ComponentDataWrapper,
				new XAttribute(_xsi + XMLNames.XSIType, stdValues ? "BatterySystemStandardValuesDataType" : "BatterySystemDataType"),
				new XElement(_vif + XMLNames.Component_Manufacturer, battery.Manufacturer),
				new XElement(_vif + XMLNames.Component_Model, battery.Model),
				new XElement(_vif + XMLNames.Component_CertificationMethod, battery.CertificationMethod.ToXMLFormat()),
				battery.CertificationMethod == CertificationMethod.StandardValues 
					? null 
					: new XElement(_vif + XMLNames.Report_Component_CertificationNumber, battery.CertificationNumber),
				new XElement(_vif + XMLNames.Component_Date,
					XmlConvert.ToString(battery.Date, XmlDateTimeSerializationMode.Utc)),
				new XElement(_vif + XMLNames.Component_AppVersion, battery.AppVersion),
				new XElement(_vif + XMLNames.REESS_BatteryType, battery.BatteryType.ToString()),
				new XElement(_vif + XMLNames.REESS_RatedCapacity, battery.Capacity.AsAmpHour.ToXMLFormat(2)),
				stdValues ? null : new XElement(_vif + XMLNames.REESS_ConnectorsSubsystemsIncluded, battery.ConnectorsSubsystemsIncluded),
				stdValues ? null : new XElement(_vif + XMLNames.REESS_JunctionboxIncluded, battery.JunctionboxIncluded),
				stdValues ? null : new XElement(_vif + XMLNames.REESS_TestingTemperature, battery.TestingTemperature.AsDegCelsius.ToXMLFormat(0)),
				GetOcv(battery.VoltageCurve),
				GetCurrentLimits(battery.MaxCurrentMap)
			);

		}


		private XElement GetOcv(DataTable voltageCurve)
		{
			var entries = new List<XElement>();

			foreach (DataRow row in voltageCurve.Rows) {
				var soc = row[BatterySOCReader.Fields.StateOfCharge];
				var ocv = row[BatterySOCReader.Fields.BatteryVoltage].ToString().ToDouble();

				entries.Add(new XElement(_vif + XMLNames.REESS_MapEntry,
					new XAttribute(XMLNames.REESS_OCV_SoC, soc),
					new XAttribute(XMLNames.REESS_OCV_OCV, ocv.ToXMLFormat(2))));
			}
			return new XElement(_vif + XMLNames.REESS_OCV, entries);
		}
		
		private XElement GetCurrentLimits(DataTable batteryMaxCurrentMap)
		{
			var entries = new List<XElement>();

			foreach (DataRow row in batteryMaxCurrentMap.Rows)
			{
				var soc = row[BatteryMaxCurrentReader.Fields.StateOfCharge];
				var maxChargingCurrent = row[BatteryMaxCurrentReader.Fields.MaxChargeCurrent].ToString().ToDouble();
				var maxDischargingCurrent = row[BatteryMaxCurrentReader.Fields.MaxDischargeCurrent].ToString().ToDouble();

				entries.Add(new XElement(_vif + XMLNames.REESS_MapEntry,
					new XAttribute(XMLNames.REESS_OCV_SoC, soc),
					new XAttribute(XMLNames.REESS_CurrentLimits_MaxChargingCurrent, maxChargingCurrent.ToXMLFormat(2)),
					new XAttribute(XMLNames.REESS_CurrentLimits_MaxDischargingCurrent, maxDischargingCurrent.ToXMLFormat(2))
				));
			}

			return new XElement(_vif + XMLNames.REESS_CurrentLimits, entries);
		}
	}
}
