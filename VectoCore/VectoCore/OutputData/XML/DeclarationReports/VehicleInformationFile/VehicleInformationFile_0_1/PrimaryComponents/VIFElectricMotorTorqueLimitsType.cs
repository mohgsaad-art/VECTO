using System;
using System.Collections.Generic;
using System.Xml.Linq;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.ManufacturerReport.ManufacturerReport_0_9.ManufacturerReportXMLTypeWriter;

namespace TUGraz.VectoCore.OutputData.XML.DeclarationReports.VehicleInformationFile.VehicleInformationFile_0_1.Components
{
	public class VIFElectricMotorTorqueLimitsType : AbstractVIFXmlType, IXmlTypeWriter
	{
		public VIFElectricMotorTorqueLimitsType(IVIFReportFactory vifFactory) : base(vifFactory) { }

		#region Implementation of IXmlTypeWriter

		public XElement GetElement(IDeclarationInputDataProvider inputData)
		{
			var elTorqueLimits = inputData.JobInputData.Vehicle.ElectricMotorTorqueLimits;
			if (elTorqueLimits == null || elTorqueLimits.Count == 0)
				return null;

			var vehicle = inputData.JobInputData.Vehicle;
			if (vehicle.Components.AxlePowertrainInputData?.Count > 0)
			{
				return GetMultiplePowertrainsEMTorqueLimits(vehicle);
            }

			var electricMachine = new List<XElement>();
			foreach (var entry in elTorqueLimits) {
				electricMachine.Add(new XElement(_v27 + XMLNames.Component_ElectricMachine,
					new XElement(_v27 + XMLNames.ElectricMachine_Position, entry.Key.Position.ToXmlFormat()),
					GetVoltageLevels(entry.Value)));
			}

			var xmlType = inputData.JobInputData.Vehicle.VehicleType == VectoSimulationJobType.SerialHybridVehicle
				? "MultipleElectricMachineTorqueLimitsType"
                : "ElectricMachineTorqueLimitsType";

			return new XElement(_vif + XMLNames.ElectricMotorTorqueLimits,
				new XAttribute(_xsi + XMLNames.XSIType, xmlType),
				new XAttribute("xmlns", _v27),
				electricMachine);
		}

		private XElement GetMultiplePowertrainsEMTorqueLimits(IVehicleDeclarationInputData vehicle)
		{
            var result = new List<XElement>();

            foreach (var entry in vehicle.ElectricMotorTorqueLimits)
            {
                result.Add(new XElement(_v27 + XMLNames.Component_ElectricMachine,
					(entry.Key.AxleNumber != AxleNumber.NOT_IN_AXLE_POWERTRAIN) ? new XAttribute("axleNumber", entry.Key.AxleNumber) : null,
					new XElement(_v27 + XMLNames.ElectricMachine_Position, entry.Key.Position.ToXmlFormat()),
					GetVoltageLevels(entry.Value)));
            }

            return new XElement(_vif + "ElectricMotorTorqueLimits", result);
        }

        private IList<XElement> GetVoltageLevels(IList<Tuple<Volt, TableData>> voltageLevels)
		{
			var voltageEntries = new List<XElement>();
			foreach (var voltageLevel in voltageLevels) 
			{
				var voltage = voltageLevel.Item1;
				var maxTorqueCurveEntries = new List<XElement>();
				for (int i = 0; i < voltageLevel.Item2.Rows.Count; i++) {

					var row = voltageLevel.Item2.Rows[i];
					var outShaftSpeed = row[XMLNames.MaxTorqueCurve_OutShaftSpeed];
					var maxTorque = row[XMLNames.MaxTorqueCurve_MaxTorque];
					var minTorque = row[XMLNames.MaxTorqueCurve_MinTorque];

					var element = new XElement(_v27 + XMLNames.MaxTorqueCurve_Entry,
						new XAttribute(XMLNames.MaxTorqueCurve_OutShaftSpeed, outShaftSpeed),
						new XAttribute(XMLNames.MaxTorqueCurve_MaxTorque, maxTorque),
						new XAttribute(XMLNames.MaxTorqueCurve_MinTorque, minTorque)
					);

					maxTorqueCurveEntries.Add(element);
				}
				
				voltageEntries.Add( new XElement(_v27 + XMLNames.ElectricMachine_VoltageLevel, 
					new XElement(_v27 + XMLNames.VoltageLevel_Voltage, voltage.ToXMLFormat(0)),
					new XElement(_v27 + XMLNames.MaxTorqueCurve, maxTorqueCurveEntries)));

			}

			return voltageEntries;
		}

		#endregion
	}
}
