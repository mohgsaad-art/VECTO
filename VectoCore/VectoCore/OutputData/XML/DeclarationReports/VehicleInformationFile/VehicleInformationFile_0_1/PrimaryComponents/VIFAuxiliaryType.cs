using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using TUGraz.VectoCommon.BusAuxiliaries;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.Impl;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.ManufacturerReport.ManufacturerReport_0_9.ManufacturerReportXMLTypeWriter;

namespace TUGraz.VectoCore.OutputData.XML.DeclarationReports.VehicleInformationFile.VehicleInformationFile_0_1.Components
{
	public class VIFAuxiliaryType : AbstractVIFXmlType, IXmlTypeWriter
	{
		public VIFAuxiliaryType(IVIFReportFactory vifFactory) : base(vifFactory) { }

		#region Implementation of IXmlTypeWriter

		public virtual XElement GetElement(IDeclarationInputDataProvider inputData)
		{
			var aux = inputData.JobInputData.Vehicle.Components.BusAuxiliaries;
			if (aux == null)
				return null;

			return new XElement(_vif + XMLNames.Component_Auxiliaries,
					new XElement(_vif + XMLNames.ComponentDataWrapper,
					new XAttribute(_xsi + XMLNames.XSIType, "AUX_Conventional_PrimaryBusType"),
					new XElement(_vif + XMLNames.BusAux_Fan,
						new XElement(_vif + XMLNames.Auxiliaries_Auxiliary_Technology, aux.FanTechnology)),
					GetSteeringPumpElement(aux.SteeringPumpTechnology),
					GetElectricSystem(aux.ElectricSupply),
					GetPneumaticSystem(aux.PneumaticSupply, aux.PneumaticConsumers),
					GetHvac(aux.HVACAux)
					));
		}

		protected virtual XElement GetSteeringPumpElement(IList<string> steeringPumps)
		{
			var result = new List<XElement>();
			for (int i = 0; i < steeringPumps.Count; i++)
			{
				result.Add(new XElement(_vif + XMLNames.BusAux_Technology,
					new XAttribute("axleNumber", (i + 1).ToString()), steeringPumps[i]));
			}

			return new XElement(_vif + XMLNames.BusAux_SteeringPump, result);
		}
		

		protected virtual XElement GetElectricSystem(IElectricSupplyDeclarationData electricSupply)
		{
			var alternatorTech = new XElement(_vif + XMLNames.Bus_AlternatorTechnology, electricSupply.AlternatorTechnology.ToXMLFormat());

			List<XElement> smartAlternators = null;
			List<XElement> auxBattery = null;
			List<XElement> auxCapacitor = null;

			if (electricSupply.Alternators?.Any() == true)
			{
				smartAlternators = new List<XElement>();

				foreach (var alternator in electricSupply.Alternators)
				{
					smartAlternators.Add(new XElement(_vif + XMLNames.BusAux_ElectricSystem_SmartAlternator,
									new XElement(_vif + XMLNames.BusAux_ElectricSystem_RatedCurrent, alternator.RatedCurrent.Value()),
									new XElement(_vif + XMLNames.BusAux_ElectricSystem_RatedRatedVoltage, alternator.RatedVoltage.Value())));
				}
			}

			if (electricSupply.ElectricStorage?.Any() == true)
			{
				auxBattery = new List<XElement>();
				auxCapacitor = new List<XElement>();

				foreach (var electricStorage in electricSupply.ElectricStorage)
				{
					if (electricStorage is BusAuxBatteryInputData battery)
					{
						auxBattery.Add(new XElement(_vif + XMLNames.BusAux_ElectricSystem_Battery,
							new XElement(_vif + XMLNames.BusAux_ElectricSystem_BatteryTechnology, battery.Technology),
							new XElement(_vif + XMLNames.BusAux_ElectricSystem_RatedCapacity, battery.Capacity.AsAmpHour),
							new XElement(_vif + XMLNames.BusAux_ElectricSystem_NominalVoltage, battery.Voltage.Value())));
					}
					else if (electricStorage is BusAuxCapacitorInputData capacitor)
					{
						auxCapacitor.Add(new XElement(_vif + XMLNames.BusAux_ElectricSystem_Capacitor,
							new XElement(_vif + XMLNames.BusAux_ElectricSystem_CapacitorTechnology, capacitor.Technology),
							new XElement(_vif + XMLNames.BusAux_ElectricSystem_RatedCapacitance, capacitor.Capacity.Value()),
							new XElement(_vif + XMLNames.BusAux_ElectricSystem_RatedVoltage, capacitor.Voltage.Value())));
					}
				}

				auxBattery = auxBattery.Any() ? auxBattery : null;
				auxCapacitor = auxCapacitor.Any() ? auxCapacitor : null;
			}

			return new XElement(_vif + XMLNames.BusAux_ElectricSystem,
						alternatorTech,
						smartAlternators,
						auxBattery,
						auxCapacitor,
						new XElement(_vif + XMLNames.BusAux_ElectricSystem_SupplyFromHEVPossible, electricSupply.ESSupplyFromHEVREESS)
			);
		}
		

		protected virtual XElement GetPneumaticSystem(IPneumaticSupplyDeclarationData pSupply, IPneumaticConsumersDeclarationData pConsumer)
		{

			return new XElement(_vif + XMLNames.BusAux_PneumaticSystem,
				new XElement(_vif + XMLNames.Bus_SizeOfAirSupply, pSupply.CompressorSize),
				new XElement(_vif + XMLNames.CompressorDrive, pSupply.CompressorDrive.GetLabel()),
				new XElement(_vif + XMLNames.Vehicle_Clutch, pSupply.Clutch),
				new XElement(_vif + XMLNames.Bus_CompressorRatio, pSupply.Ratio.ToXMLFormat(3)),
				new XElement(_vif + XMLNames.Bus_SmartCompressionSystem, pSupply.SmartAirCompression),
				new XElement(_vif + XMLNames.Bus_SmartRegenerationSystem, pSupply.SmartRegeneration),
				new XElement(_vif + XMLNames.Bus_AirsuspensionControl, GetXMLAirsuspensionControl(pConsumer.AirsuspensionControl)),
				new XElement(_vif + XMLNames.BusAux_PneumaticSystem_SCRReagentDosing, pConsumer.AdBlueDosing == ConsumerTechnology.Pneumatically)
			);
		}

		protected string GetXMLAirsuspensionControl(ConsumerTechnology airsuspensionControl)
		{
			switch (airsuspensionControl)
			{
				case ConsumerTechnology.Electrically:
					return "electronically";
				case ConsumerTechnology.Mechanically:
					return "mechanically";
				default:
					throw new VectoException("Unknown AirsuspensionControl!", ErrorCodes.AUXILIARIES_UNKNOWN_TECHNOLOGY);
			}
		}


		protected virtual XElement GetHvac(IHVACBusAuxiliariesDeclarationData hvac)
		{
			return new XElement(new XElement(_vif + XMLNames.BusAux_HVAC,
				new XElement(_vif + XMLNames.Bus_AdjustableCoolantThermostat, hvac.AdjustableCoolantThermostat),
				new XElement(_vif + XMLNames.Bus_EngineWasteGasHeatExchanger, hvac.EngineWasteGasHeatExchanger)));
		}

		#endregion
	}

	public class VIFAuxiliaryHevSType : VIFAuxiliaryType
	{
		public VIFAuxiliaryHevSType(IVIFReportFactory vifFactory) : base(vifFactory) { }


		#region Overrides of VIFAuxiliaryType

		public override XElement GetElement(IDeclarationInputDataProvider inputData)
		{
			var aux = inputData.JobInputData.Vehicle.Components.BusAuxiliaries;
			if (aux == null)
				return null;

			return new XElement(_vif + XMLNames.Component_Auxiliaries,
				new XElement(_vif + XMLNames.ComponentDataWrapper,
					new XAttribute(_xsi + XMLNames.XSIType, "AUX_HEV-S_PrimaryBusType"),
					new XElement(_vif + XMLNames.BusAux_Fan,
						new XElement(
							_vif + XMLNames.Auxiliaries_Auxiliary_Technology,
							aux.FanTechnology)),
					GetSteeringPumpElement(aux.SteeringPumpTechnology),
					GetElectricSystem(aux.ElectricSupply),
					GetPneumaticSystem(aux.PneumaticSupply, aux.PneumaticConsumers),
					GetHvac(aux.HVACAux)
				));
		}


		protected override XElement GetElectricSystem(IElectricSupplyDeclarationData electricSupply)
		{
			return new XElement(_vif + XMLNames.BusAux_ElectricSystem,
				new XElement(_vif + XMLNames.BusAux_ElectricSystem_AlternatorTechnology,
					electricSupply.AlternatorTechnology.ToXMLFormat()),
				new XElement(_vif + XMLNames.BusAux_ElectricSystem_SupplyFromHEVPossible,
					electricSupply.ESSupplyFromHEVREESS));
		}


		protected override XElement GetPneumaticSystem(IPneumaticSupplyDeclarationData pSupply, IPneumaticConsumersDeclarationData pConsumer)
		{
			return new XElement(_vif + XMLNames.BusAux_PneumaticSystem,
				new XElement(_vif + XMLNames.Bus_SizeOfAirSupply, pSupply.CompressorSize),
				new XElement(_vif + XMLNames.CompressorDrive, pSupply.CompressorDrive.GetLabel()),
				new XElement(_vif + XMLNames.Vehicle_Clutch, pSupply.Clutch),
				new XElement(_vif + XMLNames.Bus_CompressorRatio, pSupply.Ratio.ToXMLFormat(3)),
				new XElement(_vif + XMLNames.Bus_SmartRegenerationSystem, pSupply.SmartRegeneration),
				new XElement(_vif + XMLNames.Bus_AirsuspensionControl, GetXMLAirsuspensionControl(pConsumer.AirsuspensionControl)),
				new XElement(_vif + XMLNames.BusAux_PneumaticSystem_SCRReagentDosing, pConsumer.AdBlueDosing == ConsumerTechnology.Pneumatically)
			);
		}

		#endregion
	}

	public class VIFAuxiliaryIEPC_SType : VIFAuxiliaryType
	{
		public VIFAuxiliaryIEPC_SType(IVIFReportFactory vifFactory) : base(vifFactory) { }


		#region Overrides of VIFAuxiliaryType

		public override XElement GetElement(IDeclarationInputDataProvider inputData)
		{
			var aux = inputData.JobInputData.Vehicle.Components.BusAuxiliaries;
			if (aux == null)
				return null;

			return new XElement(_vif + XMLNames.Component_Auxiliaries,
				new XElement(_vif + XMLNames.ComponentDataWrapper,
					new XAttribute(_xsi + XMLNames.XSIType, "AUX_HEV-S_PrimaryBusType"),
					new XElement(_vif + XMLNames.BusAux_Fan,
						new XElement(_vif + XMLNames.Auxiliaries_Auxiliary_Technology, aux.FanTechnology)),
					GetSteeringPumpElement(aux.SteeringPumpTechnology),
					GetElectricSystem(aux.ElectricSupply),
					GetPneumaticSystem(aux.PneumaticSupply, aux.PneumaticConsumers),
					GetHvac(aux.HVACAux)
				));
		}


		protected override XElement GetElectricSystem(IElectricSupplyDeclarationData electricSupply)
		{
			return new XElement(_vif + XMLNames.BusAux_ElectricSystem,
				new XElement(_vif + XMLNames.BusAux_ElectricSystem_AlternatorTechnology,
					electricSupply.AlternatorTechnology.ToXMLFormat()),
				new XElement(_vif + XMLNames.BusAux_ElectricSystem_SupplyFromHEVPossible,
					electricSupply.ESSupplyFromHEVREESS));
		}


		protected override XElement GetPneumaticSystem(IPneumaticSupplyDeclarationData pSupply, IPneumaticConsumersDeclarationData pConsumer)
		{
			return new XElement(_vif + XMLNames.BusAux_PneumaticSystem,
				new XElement(_vif + XMLNames.Bus_SizeOfAirSupply, pSupply.CompressorSize),
				new XElement(_vif + XMLNames.CompressorDrive, pSupply.CompressorDrive.GetLabel()),
				new XElement(_vif + XMLNames.Vehicle_Clutch, pSupply.Clutch),
				new XElement(_vif + XMLNames.Bus_CompressorRatio, pSupply.Ratio.ToXMLFormat(3)),
				new XElement(_vif + XMLNames.Bus_SmartRegenerationSystem, pSupply.SmartRegeneration),
				new XElement(_vif + XMLNames.Bus_AirsuspensionControl, GetXMLAirsuspensionControl(pConsumer.AirsuspensionControl)),
				new XElement(_vif + XMLNames.BusAux_PneumaticSystem_SCRReagentDosing, pConsumer.AdBlueDosing == ConsumerTechnology.Pneumatically)
			);
		}

		#endregion
	}

	public class VIFAuxiliaryHevPType : VIFAuxiliaryType
	{
		public VIFAuxiliaryHevPType(IVIFReportFactory vifFactory) : base(vifFactory) { }


		#region Overrides of VIFAuxiliaryHevSType

		public override XElement GetElement(IDeclarationInputDataProvider inputData)
		{

			var aux = inputData.JobInputData.Vehicle.Components.BusAuxiliaries;
			if (aux == null)
				return null;

			return new XElement(_vif + XMLNames.Component_Auxiliaries,
				new XElement(_vif + XMLNames.ComponentDataWrapper,
					new XAttribute(_xsi + XMLNames.XSIType, "AUX_HEV-P_PrimaryBusType"),
					new XElement(_vif + XMLNames.BusAux_Fan,
						new XElement(_vif + XMLNames.Auxiliaries_Auxiliary_Technology, aux.FanTechnology)),
					GetSteeringPumpElement(aux.SteeringPumpTechnology),
					GetElectricSystem(aux.ElectricSupply),
					GetPneumaticSystem(aux.PneumaticSupply, aux.PneumaticConsumers),
					GetHvac(aux.HVACAux)
				));

		}

		#endregion
		
		#region Overrides of VIFAuxiliaryHevSType

		protected override XElement GetPneumaticSystem(IPneumaticSupplyDeclarationData pSupply, IPneumaticConsumersDeclarationData pConsumer)
		{
			return new XElement(_vif + XMLNames.BusAux_PneumaticSystem,
				new XElement(_vif + XMLNames.Bus_SizeOfAirSupply, pSupply.CompressorSize),
				new XElement(_vif + XMLNames.CompressorDrive, pSupply.CompressorDrive.GetLabel()),
				new XElement(_vif + XMLNames.BusAux_Clutch, pSupply.Clutch),
				new XElement(_vif + XMLNames.Bus_CompressorRatio, pSupply.Ratio.ToXMLFormat(3)),
				new XElement(_vif + XMLNames.BusAux_PneumaticSystem_SmartcompressionSystem, pSupply.SmartAirCompression),
				new XElement(_vif + XMLNames.BusAux_PneumaticSystem_SmartRegenerationSystem, pSupply.SmartRegeneration),
				new XElement(_vif + XMLNames.Bus_AirsuspensionControl, GetXMLAirsuspensionControl(pConsumer.AirsuspensionControl)),
				new XElement(_vif + XMLNames.BusAux_PneumaticSystem_SCRReagentDosing, pConsumer.AdBlueDosing == ConsumerTechnology.Pneumatically)
			);
		}
		
		#endregion
	}

	public class VIFAuxiliaryIEPCType : VIFAuxiliaryType
	{
		public VIFAuxiliaryIEPCType(IVIFReportFactory vifFactory) : base(vifFactory) { }

		#region Implementation of IXmlTypeWriter

		public override XElement GetElement(IDeclarationInputDataProvider inputData)
		{
			var aux = inputData.JobInputData.Vehicle.Components.BusAuxiliaries;
			if (aux == null)
				return null;

			return new XElement(_vif + XMLNames.Component_Auxiliaries,
				new XElement(_vif + XMLNames.ComponentDataWrapper,
					new XAttribute(_xsi + XMLNames.XSIType, "AUX_IEPC_PrimaryBusType"),
					GetSteeringPumpElement(aux.SteeringPumpTechnology),
					GetPneumaticSystem(aux.PneumaticSupply, aux.PneumaticConsumers),
					GetHvac(aux.HVACAux)
				));
		}

		#endregion
		

		#region Overrides of VIFAuxiliaryType

		protected override XElement GetPneumaticSystem(IPneumaticSupplyDeclarationData pSupply, IPneumaticConsumersDeclarationData pConsumer)
		{
			return new XElement(_vif + XMLNames.BusAux_PneumaticSystem,
				new XElement(_vif + XMLNames.CompressorDrive, pSupply.CompressorDrive.GetLabel()),
				new XElement(_vif + XMLNames.Bus_SmartRegenerationSystem, pSupply.SmartRegeneration),
				new XElement(_vif + XMLNames.Bus_AirsuspensionControl, GetXMLAirsuspensionControl(pConsumer.AirsuspensionControl))
			);
		}

		protected override XElement GetHvac(IHVACBusAuxiliariesDeclarationData hvac)
		{
			return new XElement(new XElement(_vif + XMLNames.BusAux_HVAC,
				new XElement(_vif + XMLNames.Bus_AdjustableCoolantThermostat, hvac.AdjustableCoolantThermostat)));
		}
		
		#endregion
	}
	
	public class VIFAuxiliaryPEVType : VIFAuxiliaryIEPCType
	{
		public VIFAuxiliaryPEVType(IVIFReportFactory vifFactory) : base(vifFactory) { }


		#region Overrides of VIFAuxiliaryType

		public override XElement GetElement(IDeclarationInputDataProvider inputData)
		{

			var aux = inputData.JobInputData.Vehicle.Components.BusAuxiliaries;
			if (aux == null)
				return null;

			return new XElement(_vif + XMLNames.Component_Auxiliaries,
				new XElement(_vif + XMLNames.ComponentDataWrapper,
					new XAttribute(_xsi + XMLNames.XSIType, "AUX_PEV_PrimaryBusType"),
					GetSteeringPumpElement(aux.SteeringPumpTechnology),
					GetPneumaticSystem(aux.PneumaticSupply, aux.PneumaticConsumers),
					GetHvac(aux.HVACAux)
				));
		}

		#endregion


	}

	public class VIFAuxiliaryFCHVType : VIFAuxiliaryType
	{
		public VIFAuxiliaryFCHVType(IVIFReportFactory vifFactory) : base(vifFactory) { }


		#region Overrides of VIFAuxiliaryType

		public override XElement GetElement(IDeclarationInputDataProvider inputData)
		{
			var aux = inputData.JobInputData.Vehicle.Components.BusAuxiliaries;
			if (aux == null)
				return null;

			return new XElement(_vif + XMLNames.Component_Auxiliaries,
				new XElement(_vif + XMLNames.ComponentDataWrapper,
					GetSteeringPumpElement(aux.SteeringPumpTechnology),
                    GetPneumaticSystem(aux.PneumaticSupply, aux.PneumaticConsumers),
                    GetHvac(aux.HVACAux)
                ));
		}

		protected override XElement GetPneumaticSystem(IPneumaticSupplyDeclarationData pSupply, IPneumaticConsumersDeclarationData pConsumer)
		{
			return new XElement(_vif + XMLNames.BusAux_PneumaticSystem,
                new XElement(_vif + XMLNames.CompressorDrive, pSupply.CompressorDrive.GetLabel()),
                new XElement(_vif + XMLNames.Bus_SmartRegenerationSystem, pSupply.SmartRegeneration),
                new XElement(_vif + XMLNames.Bus_AirsuspensionControl, GetXMLAirsuspensionControl(pConsumer.AirsuspensionControl)),
                new XElement(_vif + XMLNames.BusAux_PneumaticSystem_SCRReagentDosing, pConsumer.AdBlueDosing == ConsumerTechnology.Pneumatically)
			);
		}

        protected override XElement GetHvac(IHVACBusAuxiliariesDeclarationData hvac)
        {
            return new XElement(new XElement(_vif + XMLNames.BusAux_HVAC,
                new XElement(_vif + XMLNames.Bus_AdjustableCoolantThermostat, hvac.AdjustableCoolantThermostat)));
        }

        #endregion
    }

}