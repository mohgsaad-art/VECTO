using NLog.Fluent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using TUGraz.VectoCommon.BusAuxiliaries;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.Models.Declaration;

namespace TUGraz.VectoCore.OutputData.XML.DeclarationReports.ManufacturerReport.ManufacturerReport_0_9.ManufacturerReportXMLTypeWriter.Components
{


	public interface IMRFBusAuxiliariesType
	{
		XElement GetElement(IBusAuxiliariesDeclarationData auxData);
	}




	internal class MRFPrimaryBusAuxType_Conventional : AbstractMrfXmlType, IMRFBusAuxiliariesType
	{
		public MRFPrimaryBusAuxType_Conventional(IManufacturerReportFactory mrfFactory) : base(mrfFactory) { }


		#region Implementation of MRFBusAuxiliariesType

		public XElement GetElement(IBusAuxiliariesDeclarationData auxData)
		{
			var steeringPumpData = auxData.SteeringPumpTechnology;

			return new XElement(_mrf + XMLNames.Component_Auxiliaries,
				new XElement(_mrf + "CoolingFanTechnology",
					auxData.FanTechnology),
				steeringPumpData.Select(x => new XElement(_mrf + "SteeringPumpTechnology", x)),


				_mrfFactory.GetPrimaryBusElectricSystemType_Conventional_HEV().GetElement(auxData),
				_mrfFactory.GetPrimaryBusPneumaticSystemType_Conventional_HEV_Px().GetElement(auxData),
				_mrfFactory.GetPrimaryBusHVACSystemType_Conventional_HEV().GetElement(auxData)
			);
		}

		#endregion
	}

	internal class MRFPrimaryBusAuxType_HEV_P : AbstractMrfXmlType, IMRFBusAuxiliariesType
	{
		public MRFPrimaryBusAuxType_HEV_P(IManufacturerReportFactory mrfFactory) : base(mrfFactory) { }

		#region Implementation of IMRFBusAuxiliariesType

		public XElement GetElement(IBusAuxiliariesDeclarationData auxData)
		{
			var steeringPumpData = auxData.SteeringPumpTechnology;

			return new XElement(_mrf + XMLNames.Component_Auxiliaries,
				new XElement(_mrf + "CoolingFanTechnology",
					auxData.FanTechnology),
				steeringPumpData.Select(x => new XElement(_mrf + "SteeringPumpTechnology", x)),
				_mrfFactory.GetPrimaryBusElectricSystemType_Conventional_HEV().GetElement(auxData),
				_mrfFactory.GetPrimaryBusPneumaticSystemType_Conventional_HEV_Px().GetElement(auxData),
				_mrfFactory.GetPrimaryBusHVACSystemType_Conventional_HEV().GetElement(auxData)
			);
		}

		#endregion
	}

	internal class MRFPrimaryBusAuxType_HEV_S : AbstractMrfXmlType, IMRFBusAuxiliariesType
	{
		public MRFPrimaryBusAuxType_HEV_S(IManufacturerReportFactory mrfFactory) : base(mrfFactory) { }

		#region Implementation of IMRFBusAuxiliariesType

		public XElement GetElement(IBusAuxiliariesDeclarationData auxData)
		{
			var steeringPumpData = auxData.SteeringPumpTechnology;

			return new XElement(_mrf + XMLNames.Component_Auxiliaries,
				new XElement(_mrf + "CoolingFanTechnology", auxData.FanTechnology),
				steeringPumpData.Select(x => new XElement(_mrf + "SteeringPumpTechnology", x)),
				_mrfFactory.GetPrimaryBusElectricSystemType_Conventional_HEV().GetElement(auxData),

				_mrfFactory.GetPrimaryBusPneumaticSystemType_HEV_S().GetElement(auxData),
				_mrfFactory.GetPrimaryBusHVACSystemType_Conventional_HEV().GetElement(auxData)
			);
		}

		#endregion
	}

	internal class MRFPrimaryBusAuxType_HEV_F : AbstractMrfXmlType, IMRFBusAuxiliariesType
	{
		public MRFPrimaryBusAuxType_HEV_F(IManufacturerReportFactory mrfFactory) : base(mrfFactory) { }

		#region Implementation of IMRFBusAuxiliariesType

		public XElement GetElement(IBusAuxiliariesDeclarationData auxData)
		{
			var steeringPumpData = auxData.SteeringPumpTechnology;


			return new XElement(_mrf + XMLNames.Component_Auxiliaries,
				steeringPumpData.Select(x => new XElement(_mrf + "SteeringPumpTechnology", x)),
				_mrfFactory.GetPrimaryBusHVACSystemType_FCHV().GetElement(auxData),

				_mrfFactory.GetPrimaryBusPneumaticSystemType_HEV_F().GetElement(auxData),
				_mrfFactory.GetPrimaryBusHVACSystemType_Conventional_HEV().GetElement(auxData)
			);
		}

		#endregion
	}

	internal class MRFPrimaryBusAuxType_PEV : AbstractMrfXmlType, IMRFBusAuxiliariesType
	{
		public MRFPrimaryBusAuxType_PEV(IManufacturerReportFactory mrfFactory) : base(mrfFactory) { }

		#region Implementation of IMRFBusAuxiliariesType

		public XElement GetElement(IBusAuxiliariesDeclarationData auxData)
		{
			var steeringPumpData = auxData.SteeringPumpTechnology;

			return new XElement(_mrf + XMLNames.Component_Auxiliaries,
				steeringPumpData.Select(x => new XElement(_mrf + "SteeringPumpTechnology", x)),
				_mrfFactory.GetPrimaryBusElectricSystemType_PEV().GetElement(auxData),
				_mrfFactory.GetPrimaryBusPneumaticSystemType_PEV_IEPC().GetElement(auxData)
			);
		}

		#endregion
	}



	public class MRFPrimaryBusHVACSystemType_Conventional_HEV : AbstractMrfXmlType, IMRFBusAuxiliariesType
	{
		public MRFPrimaryBusHVACSystemType_Conventional_HEV(IManufacturerReportFactory mrfFactory) : base(mrfFactory) { }

		#region Implementation of IMRFBusAuxiliariesType

		public XElement GetElement(IBusAuxiliariesDeclarationData auxData)
		{
			var hvac = auxData.HVACAux;
			return new XElement(_mrf + "HVACSystem",
				new XElement(_mrf + XMLNames.Bus_AdjustableCoolantThermostat, hvac.AdjustableCoolantThermostat),
				new XElement(_mrf + XMLNames.Bus_EngineWasteGasHeatExchanger, hvac.EngineWasteGasHeatExchanger));
		}

		#endregion
	}

	internal class MRFConventionalCompletedBusAuxType : AbstractMrfXmlType, IMRFBusAuxiliariesType
	{
		public MRFConventionalCompletedBusAuxType(IManufacturerReportFactory mrfFactory) : base(mrfFactory) { }

		#region Implementation of IMRFBusAuxiliariesType

		public XElement GetElement(IBusAuxiliariesDeclarationData auxData)
		{
			var result = new XElement(_mrf + XMLNames.Component_Auxiliaries,
				_mrfFactory.GetConventionalCompletedBusElectricSystemType().GetElement(auxData),
				_mrfFactory.GetConventionalCompletedBus_HVACSystemType().GetElement(auxData)
			);
			return result;
		}

		#endregion
	}

	internal class MRFHEVCompletedBusAuxType : AbstractMrfXmlType, IMRFBusAuxiliariesType
	{
		public MRFHEVCompletedBusAuxType(IManufacturerReportFactory mrfFactory) : base(mrfFactory) { }

		#region Implementation of IMRFBusAuxiliariesType

		public XElement GetElement(IBusAuxiliariesDeclarationData auxData)
		{
			var result = new XElement(_mrf + XMLNames.Component_Auxiliaries,
				_mrfFactory.GetHEVCompletedBusElectricSystemType().GetElement(auxData),
				_mrfFactory.GetHEVCompletedBus_HVACSystemType().GetElement(auxData)
            );
			return result;
		}

		#endregion
	}

	internal class MRFPEVCompletedBusAuxType : AbstractMrfXmlType, IMRFBusAuxiliariesType
	{
		public MRFPEVCompletedBusAuxType(IManufacturerReportFactory mrfFactory) : base(mrfFactory) { }

		#region Implementation of IMRFBusAuxiliariesType

		public XElement GetElement(IBusAuxiliariesDeclarationData auxData)
		{
			var result = new XElement(_mrf + XMLNames.Component_Auxiliaries,
				_mrfFactory.GetPEVCompletedBusElectricSystemType().GetElement(auxData),
                _mrfFactory.GetPEVCompletedBus_HVACSystemType().GetElement(auxData)
            );
			return result;
		}

		#endregion
	}

	internal class MRFConventionalCompletedBusElectricSystemType : AbstractMrfXmlType, IMRFBusAuxiliariesType
	{
		public MRFConventionalCompletedBusElectricSystemType(IManufacturerReportFactory mrfFactory) : base(mrfFactory) { }

		#region Implementation of IMRFBusAuxiliariesType

		public virtual XElement GetElement(IBusAuxiliariesDeclarationData auxData)
		{
			return new XElement(_mrf + XMLNames.BusAux_ElectricSystem,
				new XElement(_mrf + "DayRunningLightsLED", auxData.ElectricConsumers.DayrunninglightsLED),
				new XElement(_mrf + "HeadLightsLED", auxData.ElectricConsumers.HeadlightsLED),
				new XElement(_mrf + "PositionLightsLED", auxData.ElectricConsumers.PositionlightsLED),
				new XElement(_mrf + "BrakeLightsLED", auxData.ElectricConsumers.BrakelightsLED),
				new XElement(_mrf + "InteriorLightsLED", auxData.ElectricConsumers.InteriorLightsLED));
		}

		#endregion
	}

	internal class MRFHEVCompletedBusElectricSystemType : MRFConventionalCompletedBusElectricSystemType
	{
		public MRFHEVCompletedBusElectricSystemType(IManufacturerReportFactory mrfFactory) : base(mrfFactory) { }

	}

	internal class MRFPEVCompletedBusElectricSystemType : MRFConventionalCompletedBusElectricSystemType
	{
		public MRFPEVCompletedBusElectricSystemType(IManufacturerReportFactory mrfFactory) : base(mrfFactory) { }

	}

	internal class MRFConventionalCompletedBus_HVACSystemType : AbstractMrfXmlType, IMRFBusAuxiliariesType
	{
		public MRFConventionalCompletedBus_HVACSystemType(IManufacturerReportFactory mrfFactory) : base(mrfFactory) { }

		#region Implementation of IMRFBusAuxiliariesType

		public XElement GetElement(IBusAuxiliariesDeclarationData auxData)
		{
			return new XElement(_mrf + "HVACSystem",
				_mrfFactory.GetCompletedBus_HVACSystemGroup().GetElements(auxData));
		}

		#endregion
	}

	internal class MRFHEVCompletedBus_HVACSystemType : AbstractMrfXmlType, IMRFBusAuxiliariesType
	{
		public MRFHEVCompletedBus_HVACSystemType(IManufacturerReportFactory mrfFactory) : base(mrfFactory) { }

		#region Implementation of IMRFBusAuxiliariesType

		public virtual XElement GetElement(IBusAuxiliariesDeclarationData auxData)
		{
			return new XElement(_mrf + "HVACSystem",
				_mrfFactory.GetCompletedBus_HVACSystemGroup().GetElements(auxData),
				_mrfFactory.GetCompletedBus_xEVHVACSystemGroup().GetElements(auxData));
		}

		#endregion
	}

	internal class MRFPEVCompletedBus_HVACSystemType : MRFHEVCompletedBus_HVACSystemType
	{
		public MRFPEVCompletedBus_HVACSystemType(IManufacturerReportFactory mrfFactory) : base(mrfFactory) { }

		
	}

	internal class MRFPrimaryBusElectricSystemType_Conventional_HEV : AbstractMrfXmlType, IMRFBusAuxiliariesType
	{
		public MRFPrimaryBusElectricSystemType_Conventional_HEV(IManufacturerReportFactory mrfFactory) : base(mrfFactory) { }


		#region Implementation of IMRFBusAuxiliariesType

		public XElement GetElement(IBusAuxiliariesDeclarationData auxData)
		{

			var maxAlternatorPower = auxData.ElectricSupply.GetMaxAlternatorPower();
			var electricStorageCapacity = DeclarationData.BusAuxiliaries.CalculateBatteryCapacity(
				auxData.ElectricSupply.ElectricStorage) ?? 0.SI<WattSecond>();
			return new XElement(_mrf + XMLNames.BusAux_ElectricSystem,
				new XElement(_mrf + XMLNames.BusAux_ElectricSystem_AlternatorTechnology,
					auxData.ElectricSupply.AlternatorTechnology.ToXMLFormat()),
				(auxData.ElectricSupply.AlternatorTechnology == AlternatorType.Smart
					? new XElement(_mrf + "MaxAlternatorPower", maxAlternatorPower.ToXMLFormat(0))
					: null),

				auxData.ElectricSupply.AlternatorTechnology == AlternatorType.Smart
					? new XElement(_mrf + "ElectricStorageCapacity", electricStorageCapacity.ToXMLFormat())
					: null);
		}

		#endregion
	}

	internal class MRFPrimaryBusElectricSystemType_PEV : AbstractMrfXmlType, IMRFBusAuxiliariesType
	{
		public MRFPrimaryBusElectricSystemType_PEV(IManufacturerReportFactory mrfFactory) : base(mrfFactory) { }


		#region Implementation of IMRFBusAuxiliariesType

		public XElement GetElement(IBusAuxiliariesDeclarationData auxData)
		{
			var electricStorageCapacity =
				DeclarationData.BusAuxiliaries.CalculateBatteryCapacity(auxData.ElectricSupply.ElectricStorage);
			return new XElement(_mrf + XMLNames.BusAux_ElectricSystem,
				auxData.ElectricSupply.ElectricStorage == null || electricStorageCapacity == null ? null :  new XElement(_mrf + "ElectricStorageCapacity", electricStorageCapacity.ToXMLFormat()));
		}

		#endregion
	}


	internal class MRFPrimaryBusPneumaticSystemType_Conventional_Hev_Px : AbstractMrfXmlType, IMRFBusAuxiliariesType
	{
		public MRFPrimaryBusPneumaticSystemType_Conventional_Hev_Px(IManufacturerReportFactory mrfFactory) : base(mrfFactory) { }

		#region Implementation of IMRFBusAuxiliariesType

		public virtual XElement GetElement(IBusAuxiliariesDeclarationData auxData)
		{
			return new XElement(_mrf + XMLNames.BusAux_PneumaticSystem,
				new XElement(_mrf + XMLNames.Auxiliaries_Auxiliary_Technology, GetPneumaticSystemTechnology(auxData)),
				new XElement(_mrf + XMLNames.Bus_CompressorRatio, auxData.PneumaticSupply.Ratio.ToXMLFormat(3)),

				new XElement(_mrf + XMLNames.BusAux_PneumaticSystem_SmartcompressionSystem, auxData.PneumaticSupply.SmartAirCompression),
				new XElement(_mrf + XMLNames.BusAux_PneumaticSystem_SmartRegenerationSystem, auxData.PneumaticSupply.SmartRegeneration),
				new XElement(_mrf + XMLNames.BusAux_PneumaticSystem_AirsuspensionControl, auxData.PneumaticConsumers.AirsuspensionControl.ToXMLFormat()),
				new XElement(_mrf + "ReagentDosing", auxData.PneumaticConsumers.AdBlueDosing == ConsumerTechnology.Pneumatically)
			);
		}

		protected virtual string GetPneumaticSystemTechnology(IBusAuxiliariesDeclarationData auxData)
		{
			return $"{auxData.PneumaticSupply.CompressorSize}, {auxData.PneumaticSupply.CompressorDrive.GetLabel()}, clutch: {auxData.PneumaticSupply.Clutch}";
		}

		#endregion
	}

	internal class MRFPrimaryBusPneumaticSystemType_HEV_S : MRFPrimaryBusPneumaticSystemType_Conventional_Hev_Px
	{
		public MRFPrimaryBusPneumaticSystemType_HEV_S(IManufacturerReportFactory mrfFactory) : base(mrfFactory) { }

		#region Implementation of IMRFBusAuxiliariesType

		public override XElement GetElement(IBusAuxiliariesDeclarationData auxData)
		{
			return new XElement(_mrf + XMLNames.BusAux_PneumaticSystem,
				new XElement(_mrf + XMLNames.Auxiliaries_Auxiliary_Technology, GetPneumaticSystemTechnology(auxData)),
				new XElement(_mrf + XMLNames.Bus_CompressorRatio, auxData.PneumaticSupply.Ratio.ToXMLFormat(3)),
				new XElement(_mrf + XMLNames.BusAux_PneumaticSystem_SmartRegenerationSystem, auxData.PneumaticSupply.SmartRegeneration),
				new XElement(_mrf + XMLNames.BusAux_PneumaticSystem_AirsuspensionControl, auxData.PneumaticConsumers.AirsuspensionControl.ToXMLFormat()),
				new XElement(_mrf + "ReagentDosing", auxData.PneumaticConsumers.AdBlueDosing == ConsumerTechnology.Pneumatically)
			);
		}

		#endregion
	}

	internal class MRFPrimaryBusPneumaticSystemType_PEV_IEPC : MRFPrimaryBusPneumaticSystemType_Conventional_Hev_Px
	{
		public MRFPrimaryBusPneumaticSystemType_PEV_IEPC(IManufacturerReportFactory mrfFactory) : base(mrfFactory) { }

		#region Implementation of IMRFBusAuxiliariesType

		public override XElement GetElement(IBusAuxiliariesDeclarationData auxData)
		{
			return new XElement(_mrf + XMLNames.BusAux_PneumaticSystem,
				new XElement(_mrf + XMLNames.Auxiliaries_Auxiliary_Technology, auxData.PneumaticSupply.CompressorDrive.ToString()),
				new XElement(_mrf + XMLNames.BusAux_PneumaticSystem_SmartRegenerationSystem, auxData.PneumaticSupply.SmartRegeneration),
				new XElement(_mrf + XMLNames.BusAux_PneumaticSystem_AirsuspensionControl, auxData.PneumaticConsumers.AirsuspensionControl.ToXMLFormat())
			);
		}

		#endregion
	}

	internal class MRFPrimaryBusPneumaticSystemType_HEV_F : MRFPrimaryBusPneumaticSystemType_Conventional_Hev_Px
	{
		public MRFPrimaryBusPneumaticSystemType_HEV_F(IManufacturerReportFactory mrfFactory) : base(mrfFactory) { }

		#region Implementation of IMRFBusAuxiliariesType

		public override XElement GetElement(IBusAuxiliariesDeclarationData auxData)
		{
			return new XElement(_mrf + XMLNames.BusAux_PneumaticSystem,
				new XElement(_mrf + XMLNames.Auxiliaries_Auxiliary_Technology, GetPneumaticSystemTechnology(auxData)),
				new XElement(_mrf + XMLNames.Bus_CompressorRatio, auxData.PneumaticSupply.Ratio.ToXMLFormat(3)),
				new XElement(_mrf + XMLNames.BusAux_PneumaticSystem_SmartRegenerationSystem, auxData.PneumaticSupply.SmartRegeneration),
				new XElement(_mrf + XMLNames.BusAux_PneumaticSystem_AirsuspensionControl, auxData.PneumaticConsumers.AirsuspensionControl.ToXMLFormat()),
				new XElement(_mrf + "ReagentDosing", auxData.PneumaticConsumers.AdBlueDosing == ConsumerTechnology.Pneumatically)
			);
		}

		#endregion
	}

	internal class MRFPrimaryBusElectricSystemType_FCHV : AbstractMrfXmlType, IMRFBusAuxiliariesType
	{
		public MRFPrimaryBusElectricSystemType_FCHV(IManufacturerReportFactory mrfFactory) : base(mrfFactory) { }


		#region Implementation of IMRFBusAuxiliariesType

		public XElement GetElement(IBusAuxiliariesDeclarationData auxData)
		{
			var maxAlternatorPower = auxData.ElectricSupply.GetMaxAlternatorPower();
			var electricStorageCapacity = DeclarationData.BusAuxiliaries.CalculateBatteryCapacity(
				auxData.ElectricSupply.ElectricStorage) ?? 0.SI<WattSecond>();
			return new XElement(_mrf + XMLNames.BusAux_ElectricSystem,
				new XElement(_mrf + XMLNames.BusAux_ElectricSystem_AlternatorTechnology,
					AlternatorType.None.ToXMLFormat()));
		}

		#endregion
	}
}
