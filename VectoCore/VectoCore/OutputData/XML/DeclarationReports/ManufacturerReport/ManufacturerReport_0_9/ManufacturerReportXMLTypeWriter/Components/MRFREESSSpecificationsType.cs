using System.Linq;
using System.Xml.Linq;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.Reader.ComponentData;
using TUGraz.VectoCore.InputData.Reader.DataObjectAdapter.SimulationComponents;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.OutputData.XML.DeclarationReports.ManufacturerReport.ManufacturerReport_0_9.ManufacturerReportXMLTypeWriter.Components
{
    internal class MrfreessSpecificationsTypeWriter : AbstractMrfXmlType, IXmlTypeWriter
	{
		public MrfreessSpecificationsTypeWriter(IManufacturerReportFactory mrfFactory) : base(mrfFactory) { }

		#region Overrides of AbstractMrfXmlType

		public XElement GetElement(IDeclarationInputDataProvider inputData)
		{
			var vehicle = GetVehicle(inputData);
			var reess = inputData.JobInputData.Vehicle.Components.ElectricStorage;
			var reessElements = reess.ElectricStorageElements;

			var result = new XElement(_mrf + "REESSSpecifications");

			if (reessElements.Any(x => x.REESSPack.StorageType == REESSType.Battery)) {
				var eletricStorageAdapter = new ElectricStorageAdapter();
				var batData = eletricStorageAdapter.CreateBatteryData(reess, vehicle.VehicleType, vehicle.OVC, vehicle.BatteryOnlyMode);
				foreach (var entry in batData.Batteries.OrderBy(x => x.Item1)) {
					var batteryPackInput = entry.Item2.InputData;
					var battery = batteryPackInput.REESSPack as IBatteryPackDeclarationInputData;
					var batUsableCap = entry.Item2.UseableStoredEnergy;
					var batTotalCap = entry.Item2.TotalStoredEnergy;

					result.Add(new XElement(_mrf + XMLNames.ElectricEnergyStorage_Battery,
							new XAttribute("stringId", entry.Item1),
							new XElement(_mrf + XMLNames.Component_Model, battery.Model),
							new XElement(_mrf + XMLNames.Component_CertificationNumber, battery.CertificationNumber),
							new XElement(_mrf + XMLNames.DI_Signature_Reference_DigestValue, battery.DigestValue?.DigestValue ?? ""),
							new XElement(_mrf + XMLNames.Component_CertificationMethod, battery.CertificationMethod.ToXMLFormat()),
							new XElement(_mrf + XMLNames.BusAux_ElectricSystem_NominalVoltage, BatterySOCReader.Create(battery.VoltageCurve).Lookup(0.5).ToXMLFormat(0)),
							new XElement(_mrf + "TotalStorageCapacity", batTotalCap.ValueAsUnit("kWh", 0)),
							new XElement(_mrf + "TotalUsableCapacityInSimulation", batUsableCap.ValueAsUnit("kWh", 0)),
                            (battery.DeteriorationPerformanceRatio != null) ? new XElement(_mrf + "DeteriorationPerformanceRatio", battery.DeteriorationPerformanceRatio) : null,
							(battery.MinSOC != null) ? new XElement(_mrf + "SOCmin", battery.MinSOC) : null,
                            (battery.MaxSOC != null) ? new XElement(_mrf + "SOCmax", battery.MaxSOC) : null
                        )
					);
				}

			} else {
				foreach (var electricStorage in reessElements.Where(x => x.REESSPack.StorageType == REESSType.SuperCap)) {
					var superCap = electricStorage.REESSPack as ISuperCapDeclarationInputData;
					result.Add(new XElement(_mrf + XMLNames.ElectricEnergyStorage_Capacitor,
							new XElement(_mrf + XMLNames.Component_Model, superCap.Model),
							new XElement(_mrf + XMLNames.Component_CertificationNumber, superCap.CertificationNumber),
							new XElement(_mrf + XMLNames.DI_Signature_Reference_DigestValue,
								superCap.DigestValue?.DigestValue ?? ""),
                            new XElement(_mrf + XMLNames.Component_CertificationMethod, superCap.CertificationMethod.ToXMLFormat()),
                            new XElement(_mrf + XMLNames.Capacitor_Capacitance, superCap.Capacity.ToXMLFormat()),
							new XElement(_mrf + XMLNames.Capacitor_MinVoltage, superCap.MinVoltage.ToXMLFormat(2)),
							new XElement(_mrf + XMLNames.Capacitor_MaxVoltage, superCap.MaxVoltage.ToXMLFormat(2))
						)
					);
				}
			}
			return result;
		}

		#endregion
	}
}
