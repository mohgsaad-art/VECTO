using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.DataProvider;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.ManufacturerReport.ManufacturerReport_0_9.ManufacturerReportXMLTypeWriter;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.OutputData.XML.DeclarationReports.VehicleInformationFile.VehicleInformationFile_0_1.Components
{
	public class VIFIepcType : AbstractVIFXmlType, IXmlTypeWriter
	{
		public VIFIepcType(IVIFReportFactory vifFactory) : base(vifFactory) { }

		#region Implementation of IXmlTypeWriter

		public XElement GetElement(IDeclarationInputDataProvider inputData)
		{
			var iepc = inputData.JobInputData.Vehicle.Components.IEPC;
			if (iepc == null)
				return null;

			var xmlIepc = iepc as AbstractCommonComponentType;
			if (xmlIepc == null) {
				throw new VectoException("IEPC requires input to be in XML format");
			}

			var certificationMethod = xmlIepc.XMLSource
				.SelectSingleNode(XMLHelper.QueryLocalName(XMLNames.Component_CertificationMethod))?.InnerText;
			
            return new XElement(_vif + XMLNames.Component_IEPC,
					new XElement(_vif + XMLNames.ComponentDataWrapper,
						new XAttribute(XNamespace.Xmlns + "vif1.0", _vif10.NamespaceName),
                        new XAttribute(_xsi + XMLNames.XSIType, "vif1.0:IEPCDataDeclarationType"),
						new XElement(_vif + XMLNames.Component_Manufacturer, iepc.Manufacturer),
						new XElement(_vif + XMLNames.Component_Model, iepc.Model),
						new XElement(_vif + XMLNames.Component_CertificationNumber, iepc.CertificationNumber),
						new XElement(_vif + XMLNames.Component_Date, XmlConvert.ToString(iepc.Date, XmlDateTimeSerializationMode.Utc)),
						new XElement(_vif + XMLNames.Component_AppVersion, iepc.AppVersion),
						new XElement(_vif + XMLNames.ElectricMachine_ElectricMachineType, iepc.ElectricMachineType.ToString()),
						new XElement(_vif10 + XMLNames.Component_CertificationMethod, certificationMethod),
						new XElement(_vif + XMLNames.ElectricMachine_R85RatedPower, iepc.R85RatedPower.ToXMLFormat(0)),
						new XElement(_vif + XMLNames.ElectricMachine_RotationalInertia, iepc.Inertia.ToXMLFormat(2)),
						new XElement(_vif10 + XMLNames.IEPC_DifferentialIncluded, iepc.DifferentialIncluded),
						new XElement(_vif10 + XMLNames.IEPC_DesignTypeWheelMotor, iepc.DesignTypeWheelMotor),
						iepc.NrOfDesignTypeWheelMotorMeasured == null
							? null
							: new XElement(_vif10 + XMLNames.IEPC_NrOfDesignTypeWheelMotorMeasured, iepc.NrOfDesignTypeWheelMotorMeasured.Value),
						GetGears(iepc.Gears),
						GetVoltageLevels(iepc.VoltageLevels),
						GetDragCurves(iepc.DragCurves),
						GetConditioning(iepc.Conditioning),
                        new XElement(_vif10 + XMLNames.IEPC_DisengagementClutch, iepc.DisengagementClutch)
                    )
			);
		}
		
		#endregion


		private XElement GetGears(IList<IGearEntry> gearsData)
		{
			var gears = new List<XElement>();
			
			foreach (var gearEntry in gearsData) {

				var currentGear = new XElement(_vif + XMLNames.Gear_EntryName,
					new XAttribute("number", gearEntry.GearNumber.ToString()),
					new XElement(_vif + XMLNames.GearRatio_Ratio, gearEntry.Ratio.ToXMLFormat(3)),
					gearEntry.MaxOutputShaftTorque == null 
						? null
						: new XElement(_vif + XMLNames.Gear_MaxOutputShaftTorque, gearEntry.MaxOutputShaftTorque.ToXMLFormat(0)),

					gearEntry.MaxOutputShaftSpeed == null 
						? null 
						: new XElement(_vif + XMLNames.Gear_MaxOutputShaftSpeed, gearEntry.MaxOutputShaftSpeed.AsRPM.ToXMLFormat(0))
					);
				gears.Add(currentGear);
			}
			
			return new XElement(_vif10 + XMLNames.Gearbox_Gears, 
				new XAttribute(_xsi + XMLNames.XSIType, "IEPCGearsDeclarationType"),
				gears);
		}


		private IList<XElement> GetVoltageLevels(IList<IElectricMotorVoltageLevel> voltageData)
		{
			var voltageLevels = new List<XElement>();

			foreach (var voltageEntry in voltageData) {

				var voltage = new XElement(_vif10 + XMLNames.ElectricMachine_VoltageLevel,
					voltageEntry.VoltageLevel == null 
						? null 
						: new XElement(_vif10 + XMLNames.VoltageLevel_Voltage, voltageEntry.VoltageLevel.ToXMLFormat(0)),
					new XElement(_vif10 + XMLNames.ElectricMachine_ContinuousTorque, voltageEntry.ContinuousTorque.ToXMLFormat(2)),
					new XElement(_vif10 + XMLNames.ElectricMachine_TestSpeedContinuousTorque, voltageEntry.ContinuousTorqueSpeed.AsRPM.ToXMLFormat(2)),
					new XElement(_vif10 + XMLNames.ElectricMachine_OverloadTorque, voltageEntry.OverloadTorque.ToXMLFormat(2)),
					new XElement(_vif10 + XMLNames.ElectricMachine_TestSpeedOverloadTorque, voltageEntry.OverloadTestSpeed.AsRPM.ToXMLFormat(2)),
					new XElement(_vif10 + XMLNames.ElectricMachine_OverloadDuration, voltageEntry.OverloadTime.ToXMLFormat(2)),
					voltageEntry.FullLoadCurve.Select(x => GetMaxTorqueCurve(x))
					//GetPowerMap(voltageEntry.PowerMap)
                );

				voltageLevels.Add(voltage);
			}

			return voltageLevels;
		}

        private XElement GetMaxTorqueCurve(IElectricMotorLoadCurve curve)
		{
			var maxTorqueCurveEntries = new List<XElement>();

			for (int r = 0; r < curve.LoadCurve.Rows.Count; r++) {
				var row = curve.LoadCurve.Rows[r];
				var outShaftSpeed = row[XMLNames.MaxTorqueCurve_OutShaftSpeed];
				var maxTorque = row[XMLNames.MaxTorqueCurve_MaxTorque];
				var minTorque = row[XMLNames.MaxTorqueCurve_MinTorque];
				
				var element = new XElement(_v26 + XMLNames.MaxTorqueCurve_Entry,
					new XAttribute(XMLNames.MaxTorqueCurve_OutShaftSpeed, outShaftSpeed),
					new XAttribute(XMLNames.MaxTorqueCurve_MaxTorque, maxTorque),
					new XAttribute(XMLNames.MaxTorqueCurve_MinTorque, minTorque)
				);

				maxTorqueCurveEntries.Add(element);
			}
            
            return (curve.Gear > 0)
				? new XElement(_vif10 + XMLNames.MaxTorqueCurve, new XAttribute(XMLNames.MaxTorqueCurve_attr_gear, curve.Gear), maxTorqueCurveEntries)
				: new XElement(_vif10 + XMLNames.MaxTorqueCurve, maxTorqueCurveEntries);
		}
		
		private IList<XElement> GetDragCurves(IList<IDragCurve> dragCurves)
		{
			var result = new List<XElement>();
			
			foreach (var dragCurve in dragCurves) {
				var entries = new List<XElement>();
				for (int r = 0; r < dragCurve.DragCurve.Rows.Count; r++) {
					var outShaftSpeed = dragCurve.DragCurve.Rows[r][XMLNames.DragCurve_OutShaftSpeed].ToString().ToDouble();
					var dragTorque = dragCurve.DragCurve.Rows[r][XMLNames.DragCurve_DragTorque].ToString().ToDouble();

					var entry = new XElement(_vif10 + XMLNames.DragCurve_Entry,
						new XAttribute(XMLNames.DragCurve_OutShaftSpeed, outShaftSpeed.ToXMLFormat(2)),
						new XAttribute(XMLNames.DragCurve_DragTorque, dragTorque.ToXMLFormat(2)));

					entries.Add(entry);
				}

				result.Add(new  XElement(_vif10 + XMLNames.DragCurve, 
					dragCurves.Count == 1 && !dragCurve.Gear.HasValue ? null : new XAttribute(XMLNames.DragCurve_Gear, dragCurve.Gear.Value),
					entries));
			}

			return result;
		}

		private XElement GetConditioning(DataTable iepcData)
		{
			if (iepcData == null)
				return null;

			var entries = new List<XElement>();
			for (int r = 0; r < iepcData.Rows.Count; r++) {

				var coolantTempInLet = iepcData.Rows[r][XMLNames.Conditioning_CoolantTempInlet];
				var coolingPower = iepcData.Rows[r][XMLNames.Conditioning_CoolingPower];

				entries.Add(new XElement(_vif10 + XMLNames.Conditioning_Entry, 
					new XAttribute(XMLNames.Conditioning_CoolantTempInlet, coolantTempInLet),
					new XAttribute(XMLNames.Conditioning_CoolingPower, coolingPower)));
			}
			
			return new XElement(_vif10 + XMLNames.Conditioning, entries);
		}
	}
}
