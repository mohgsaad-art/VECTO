using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Interfaces;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Reader;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.InputData.FileIO.XML.Declaration.DataProvider
{
	public class XMLElectricStorageSystemDeclarationInputData : AbstractCommonComponentType,
		IXMLElectricStorageSystemDeclarationInputData
	{
		public static readonly XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V24;
		public const string XSD_TYPE = "ElectricEnergyStorageType";
		public static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

		private IList<IElectricStorageDeclarationInputData> _electricStorageElements;
		private IXMLDeclarationVehicleData _vehicle;
		
		public XMLElectricStorageSystemDeclarationInputData(
			IXMLDeclarationVehicleData vehicle, XmlNode componentNode, string sourceFile)
			: base(componentNode, sourceFile)
		{
			_vehicle = vehicle;
			SourceType = DataSourceType.XMLEmbedded;
		}

		#region Implementation of IElectricStorageSystemDeclarationInputData

		public virtual IList<IElectricStorageDeclarationInputData> ElectricStorageElements =>
			_electricStorageElements ?? (_electricStorageElements = GetElectricStorages());

		#endregion
		
		private IList<IElectricStorageDeclarationInputData> GetElectricStorages()
		{
			var electricStorages = new List<IElectricStorageDeclarationInputData>();
			if (ElementExists(XMLNames.ElectricEnergyStorage_Capacitor)) {
				var capacitor = GetNode(XMLNames.ElectricEnergyStorage_Capacitor);
				electricStorages.Add(new XMLElectricStorageDeclaration {
							REESSPack = StorageTypeReader.CreateREESSInputData(capacitor, REESSType.SuperCap)
				});
			}else if (ElementExists(XMLNames.ElectricEnergyStorage_Battery)) {
				var batteries = GetNodes(XMLNames.ElectricEnergyStorage_Battery);
				foreach (XmlNode battery in batteries)
				{
					electricStorages.Add(new XMLElectricStorageDeclaration {
							REESSPack = StorageTypeReader.CreateREESSInputData(battery, REESSType.Battery),
							StringId = XmlConvert.ToInt32(GetString(XMLNames.Battery_StringID, battery))
					});
				}
			}
		
			return electricStorages.Any() ? electricStorages : null;
		}

		#region Implementation of IXMLElectricStorageSystemDeclarationInputData

		public virtual IXMLREESSReader StorageTypeReader { protected get; set; }

		#endregion
		
		#region Implementation of IXMLResource

		protected override XNamespace SchemaNamespace => NAMESPACE_URI;
		protected override DataSourceType SourceType { get; }

		#endregion
		
		public class XMLElectricStorageDeclaration : IElectricStorageDeclarationInputData
		{
			#region Implementation of IElectricStorageDeclarationInputData
			public IREESSPackInputData REESSPack { get; set; }
			public int Count { get; set; }
			public int StringId { get; set; }

			#endregion
		}
	}

	// ---------------------------------------------------------------------------------------

	public class XMLBatteryPackDeclarationDeclarationInputData : AbstractCommonComponentType,
		IXMLBatteryPackDeclarationInputData
	{
		public static readonly XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V24;
		public const string XSD_TYPE = "REESSBatteryType";
		public static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

		public XMLBatteryPackDeclarationDeclarationInputData(XmlNode componentNode, string sourceFile) 
			: base(componentNode, sourceFile)
		{
			SourceType = DataSourceType.XMLEmbedded;
		}
		
		#region Implementation of IREESSPackInputData

		public REESSType StorageType => REESSType.Battery;

		#endregion

		#region Implementation of IBatteryPackDeclarationInputData

		public virtual double? MinSOC  => 
			ElementExists(XMLNames.Battery_SOCmin) ? GetDouble(XMLNames.Battery_SOCmin) : (double?)null;

		public virtual double? MaxSOC => 
			ElementExists(XMLNames.Battery_SOCmax) ? GetDouble(XMLNames.Battery_SOCmax) : (double?)null;

		public virtual BatteryType BatteryType => GetString(XMLNames.REESS_BatteryType).ParseEnum<BatteryType>();
		public virtual AmpereSecond Capacity => GetDouble(XMLNames.REESS_RatedCapacity).SI(Unit.SI.Ampere.Hour).Cast<AmpereSecond>();
		public virtual bool ConnectorsSubsystemsIncluded => GetBool(XMLNames.REESS_ConnectorsSubsystemsIncluded);
		public virtual bool JunctionboxIncluded => GetBool(XMLNames.REESS_JunctionboxIncluded);

		public virtual Kelvin TestingTemperature =>
				ElementExists(XMLNames.REESS_TestingTemperature)
					? GetDouble(XMLNames.REESS_TestingTemperature).DegCelsiusToKelvin() : null;

		public virtual TableData InternalResistanceCurve => ReadTableData(XMLNames.REESS_InternalResistanceCurve, XMLNames.REESS_MapEntry,
			new Dictionary<string, string> {
				{XMLNames.REESS_InternalResistanceCurve_SoC, XMLNames.REESS_InternalResistanceCurve_SoC},
				{XMLNames.REESS_InternalResistanceCurve_R2, XMLNames.REESS_InternalResistanceCurve_R2},
				{XMLNames.REESS_InternalResistanceCurve_R10, XMLNames.REESS_InternalResistanceCurve_R10},
				{XMLNames.REESS_InternalResistanceCurve_R20, XMLNames.REESS_InternalResistanceCurve_R20},
				{XMLNames.REESS_InternalResistanceCurve_R120, XMLNames.REESS_InternalResistanceCurve_R120}
			});
		
		public virtual TableData VoltageCurve => ReadTableData(XMLNames.REESS_OCV, XMLNames.REESS_MapEntry, 
			new Dictionary<string, string> {
				{XMLNames.REESS_OCV_SoC, XMLNames.REESS_OCV_SoC},
				{XMLNames.REESS_OCV_OCV, XMLNames.REESS_OCV_OCV}
		});
		
		public virtual TableData MaxCurrentMap => ReadTableData(XMLNames.REESS_CurrentLimits, XMLNames.REESS_MapEntry, 
			new Dictionary<string, string> {
				{XMLNames.REESS_CurrentLimits_SoC, XMLNames.REESS_CurrentLimits_SoC},
				{XMLNames.REESS_CurrentLimits_MaxChargingCurrent, XMLNames.REESS_CurrentLimits_MaxChargingCurrent},
				{XMLNames.REESS_CurrentLimits_MaxDischargingCurrent, XMLNames.REESS_CurrentLimits_MaxDischargingCurrent}
		});

		#endregion

		#region Overrides of AbstractXMLResource

		protected override XNamespace SchemaNamespace => NAMESPACE_URI;
		protected override DataSourceType SourceType { get; }

		#endregion
	}
	
	// ---------------------------------------------------------------------------------------
	
	public class XMLSuperCapDeclarationInputData : AbstractCommonComponentType, IXMLSuperCapDeclarationInputData
	{
		public static readonly XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V24;
		public const string XSD_TYPE = "REESSCapacitorType";
		public static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);
		
		public XMLSuperCapDeclarationInputData(XmlNode componentNode, string sourceFile) : base(componentNode, sourceFile)
		{
			SourceType = DataSourceType.XMLEmbedded;
		}
		
		#region Implementation of IREESSPackInputData

		public REESSType StorageType => REESSType.SuperCap;

		#endregion

		#region Implementation of ISuperCapDeclarationInputData

		public Farad Capacity => GetDouble(XMLNames.Capacitor_Capacitance).SI<Farad>();
		public Ohm InternalResistance => GetDouble(XMLNames.Capacitor_InternalResistance).SI<Ohm>() / 1000;
		public Volt MinVoltage => GetDouble(XMLNames.Capacitor_MinVoltage).SI<Volt>();
		public Volt MaxVoltage => GetDouble(XMLNames.Capacitor_MaxVoltage).SI<Volt>();
		public Ampere MaxCurrentCharge => GetDouble(XMLNames.Capacitor_MaxChargingCurrent).SI<Ampere>();
		public Ampere MaxCurrentDischarge => GetDouble(XMLNames.Capacitor_MaxDischargingCurrent).SI<Ampere>();

		public Kelvin TestingTemperature => 
			ElementExists(XMLNames.REESS_TestingTemperature)
				? GetDouble(XMLNames.REESS_TestingTemperature).DegCelsiusToKelvin() : null;
		
		#endregion
		
		#region Overrides of AbstractXMLResource

		protected override XNamespace SchemaNamespace => NAMESPACE_URI;
		protected override DataSourceType SourceType { get; }

		#endregion
	}
}
