using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Interfaces;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Reader;
using TUGraz.VectoCore.InputData.Reader.ComponentData;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.InputData.FileIO.XML.Declaration.DataProvider
{
	public class XMLElectricMachinesDeclarationInputDataProvider : AbstractCommonComponentType, IXMLElectricMachinesDeclarationInputData
	{
		public static readonly XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V24;
		public const string XSD_TYPE = "ElectricMachineType";
		public const string XSD_GEN_TYPE = "ElectricMachineGENType";
		public static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);
		public static readonly string QUALIFIED_GEN_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_GEN_TYPE);

		protected IXMLDeclarationVehicleData _vehicle;
		private IList<ElectricMachineEntry<IElectricMotorDeclarationInputData>> _entries;

		public XMLElectricMachinesDeclarationInputDataProvider(
			IXMLDeclarationVehicleData vehicle, XmlNode componentNode, string sourceFile) 
			: base(componentNode, sourceFile)
		{
			_vehicle = vehicle;
			SourceType = DataSourceType.XMLEmbedded;
		}

		#region Implementation of IElectricMachinesDeclarationInputData

		public virtual IList<ElectricMachineEntry<IElectricMotorDeclarationInputData>> Entries => 
			_entries ?? (_entries = GetEntries());

		#endregion

		protected virtual List<ElectricMachineEntry<IElectricMotorDeclarationInputData>> GetEntries()
		{
			var machineEntry = new ElectricMachineEntry<IElectricMotorDeclarationInputData> {
				Position = PowertrainPositionHelper.Parse(((AbstractXMLVehicleDataProviderV24)_vehicle).PowertrainPositionPrefix,
							GetString(XMLNames.ElectricMachine_PowertrainPosition)),
				Count = XmlConvert.ToInt32(GetString(XMLNames.ElectricMachine_Count)),
				ElectricMachine = ElectricMachineSystemReader.CreateElectricMachineSystem(GetNode(XMLNames.ElectricMachineSystem)),
			};

			if (ElementExists("ADC")) {
				machineEntry.ADC = ElectricMachineSystemReader.ADCInputData;
				
			}

			machineEntry.MechanicalTransmissionEfficiency = double.NaN;


			if (ElementExists(XMLNames.ElectricMachine_P2_5GearRatios)) {
				SetGearRatios(machineEntry);
			}
			
			
			return new List<ElectricMachineEntry<IElectricMotorDeclarationInputData>>{machineEntry};
		}

		protected void SetGearRatios(ElectricMachineEntry<IElectricMotorDeclarationInputData> machineEntry)
		{ 
			var gearRatios = GetNode(XMLNames.ElectricMachine_P2_5GearRatios, null, false);
			if (gearRatios != null) {
				var gears = GetNodes(XMLNames.GearRatio_Ratio, gearRatios);
				if (gears is null || gears.Count == 0)
					return;
				
				machineEntry.RatioPerGear = new double[gears.Count];
				for (int i = 0; i < gears.Count; i++) {
					machineEntry.RatioPerGear[i] = XmlConvert.ToDouble(gears[i].InnerText);
				}
			}
		}

		



		#region Implementation of IXMLElectricMachinesDeclarationInputData

		public IXMLElectricMachineSystemReader ElectricMachineSystemReader { protected get; set; }
	
		#endregion
		
		#region Overrides of AbstractXMLResource

		protected override XNamespace SchemaNamespace => NAMESPACE_URI;
		protected override DataSourceType SourceType { get; }

		#endregion
	}

	public class XMLElectricMachinesDeclarationInputDataProviderV27 : XMLElectricMachinesDeclarationInputDataProvider
	{
        public static new readonly XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V27;
        public static new readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);
        public static new readonly string QUALIFIED_GEN_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_GEN_TYPE);

        public XMLElectricMachinesDeclarationInputDataProviderV27(
            IXMLDeclarationVehicleData vehicle, XmlNode componentNode, string sourceFile)
            : base(vehicle, componentNode, sourceFile)
        {}

    }

	public class XMLElectricMachinesDeclarationData : IElectricMachinesDeclarationInputData
	{
		private readonly IList<ElectricMachineEntry<IElectricMotorDeclarationInputData>> _entries;

		public XMLElectricMachinesDeclarationData(
			IList<ElectricMachineEntry<IElectricMotorDeclarationInputData>> entries)
		{
			_entries = entries;
		}

		#region Implementation of IElectricMachinesDeclarationInputData

		public IList<ElectricMachineEntry<IElectricMotorDeclarationInputData>> Entries => _entries;

		#endregion
	}

	public class XMLADCDeclarationInputDataV23 : AbstractCommonComponentType, IXMLADCDeclarationInputData
	{
		public static readonly XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V23;
		public const string XSD_TYPE = "ADCDataDeclarationType";
		public static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);
		
		public XMLADCDeclarationInputDataV23(XmlNode componentNode, string sourceFile)
			: base(componentNode, sourceFile)
		{
			SourceType = DataSourceType.XMLEmbedded;
		}

		#region Overrides of AbstractXMLResource

		protected override XNamespace SchemaNamespace => NAMESPACE_URI;
		protected override DataSourceType SourceType { get; }

		#endregion

		#region Implementation of IADCDeclarationInputData

		public double Ratio => GetDouble(XMLNames.ADC_Ratio);

		public virtual TableData LossMap => ReadTableData(XMLNames.ADC_TorqueLossMap, XMLNames.ADC_TorqueLossMap_Entry,
			new Dictionary<string, string> {
				{ TransmissionLossMapReader.Fields.InputSpeed, XMLNames.ADC_TorqueLossMap_InputSpeed },
				{ TransmissionLossMapReader.Fields.InputTorque, XMLNames.ADC_TorqueLossMap_InputTorque },
				{ TransmissionLossMapReader.Fields.TorqeLoss, XMLNames.ADC_TorqueLossMap_TorqueLoss },
			});

		#endregion
	}

	public class XMLDeclarationElectricMachinesDataProviderV01 : XMLElectricMachinesDeclarationInputDataProvider
	{
		public new static readonly XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_MULTISTAGE_BUS_VEHICLE_NAMESPACE_VO1;

		public new const string XSD_TYPE = "ElectricMachineType";
		public new const string XSD_GEN_TYPE = "ElectricMachineGENType";

		public new static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);
		public new static readonly string QUALIFIED_GEN_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_GEN_TYPE);

		public XMLDeclarationElectricMachinesDataProviderV01(IXMLDeclarationVehicleData vehicle, XmlNode componentNode, string sourceFile) : base(vehicle, componentNode, sourceFile) { }

		protected override List<ElectricMachineEntry<IElectricMotorDeclarationInputData>> GetEntries()
		{
			var machineEntry = new ElectricMachineEntry<IElectricMotorDeclarationInputData> {
				Position = PowertrainPositionHelper.Parse(((XMLDeclarationMultistage_Conventional_PrimaryVehicleBusDataProviderV01)_vehicle).PowertrainPositionPrefix,
														GetString(XMLNames.ElectricMachine_PowertrainPosition)),
				Count = XmlConvert.ToInt32(GetString(XMLNames.ElectricMachine_Count)),
				ElectricMachine = ElectricMachineSystemReader.CreateElectricMachineSystem(GetNode(XMLNames.ElectricMachineSystem)),
			};

			if (ElementExists("ADC"))
				machineEntry.ADC = ElectricMachineSystemReader.ADCInputData;

			if (ElementExists(XMLNames.ElectricMachine_P2_5GearRatios))
				SetGearRatios(machineEntry);

			return new List<ElectricMachineEntry<IElectricMotorDeclarationInputData>> { machineEntry };
		}
	}

    public class XMLDeclarationElectricMachinesDataProviderV11 : XMLDeclarationElectricMachinesDataProviderV01
	{
        public new static readonly XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_MULTISTAGE_BUS_VEHICLE_NAMESPACE_V11;

        public new static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);
        public new static readonly string QUALIFIED_GEN_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_GEN_TYPE);

        public XMLDeclarationElectricMachinesDataProviderV11(IXMLDeclarationVehicleData vehicle, XmlNode componentNode, string sourceFile) : 
			base(vehicle, componentNode, sourceFile) 
		{ }
    }

    public class XMLADCDeclarationInputDataV01 : XMLADCDeclarationInputDataV23
	{
		public new static readonly XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_MULTISTAGE_BUS_VEHICLE_NAMESPACE_VO1;

        public new const string XSD_TYPE = "ADCDataDeclarationType";
		public new static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);
		public XMLADCDeclarationInputDataV01(XmlNode componentNode, string sourceFile) : base(componentNode, sourceFile) { }


		public override TableData LossMap => null;
	}

	public class XMLADCDeclarationInputDataV11 : XMLADCDeclarationInputDataV01
	{
        public new static readonly XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_MULTISTAGE_BUS_VEHICLE_NAMESPACE_V11;

        public new static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

        public XMLADCDeclarationInputDataV11(XmlNode componentNode, string sourceFile) : base(componentNode, sourceFile) { }
    }

}
