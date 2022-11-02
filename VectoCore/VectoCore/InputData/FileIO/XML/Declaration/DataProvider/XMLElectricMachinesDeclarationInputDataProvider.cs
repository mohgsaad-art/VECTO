using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Interfaces;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Reader;
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

		private IXMLDeclarationVehicleData _vehicle;
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

		private List<ElectricMachineEntry<IElectricMotorDeclarationInputData>> GetEntries()
		{
			var machineEntry = new ElectricMachineEntry<IElectricMotorDeclarationInputData> {
				Position = PowertrainPositionHelper.Parse(((AbstractXMLVehicleDataProviderV24)_vehicle).PowertrainPositionPrefix,
							GetString(XMLNames.ElectricMachine_PowertrainPosition)),
				Count = XmlConvert.ToInt32(GetString(XMLNames.ElectricMachine_Count)),
				ElectricMachine = ElectricMachineSystemReader.CreateElectricMachineSystem(GetNode(XMLNames.ElectricMachineSystem)),
			};

			if (ElementExists("ADC"))
				machineEntry.ADC = ElectricMachineSystemReader.ADCInputData;

			if(ElementExists(XMLNames.ElectricMachine_P2_5GearRatios))
				SetGearRatios(machineEntry);
			
			return new List<ElectricMachineEntry<IElectricMotorDeclarationInputData>>{machineEntry};
		}

		private void SetGearRatios(ElectricMachineEntry<IElectricMotorDeclarationInputData> machineEntry)
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

		public TableData LossMap => ReadTableData(XMLNames.ADC_TorqueLossMap, XMLNames.ADC_TorqueLossMap_Entry,
			new Dictionary<string, string> {
				{ XMLNames.ADC_TorqueLossMap_InputSpeed, XMLNames.ADC_TorqueLossMap_InputSpeed },
				{ XMLNames.ADC_TorqueLossMap_InputTorque, XMLNames.ADC_TorqueLossMap_InputTorque },
				{ XMLNames.ADC_TorqueLossMap_TorqueLoss, XMLNames.ADC_TorqueLossMap_TorqueLoss },
			});

		#endregion
	} }
