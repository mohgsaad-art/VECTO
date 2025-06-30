#if CERTIFICATION_RELEASE || RELEASE_CANDIDATE
#define PROHIBIT_OLD_XML
#endif

using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Interfaces;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Reader;
using TUGraz.VectoCore.InputData.Reader.ComponentData;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.SimulationComponent.Data.ElectricComponents.Battery;
using TUGraz.VectoCore.OutputData.XML;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.InputData.FileIO.XML.Declaration.DataProvider
{
	public class XMLElectricStorageSystemDeclarationInputDataV24 : AbstractCommonComponentType,
		IXMLElectricStorageSystemDeclarationInputData
	{
		public static readonly XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V24;
		public const string XSD_TYPE = "ElectricEnergyStorageType";
		public static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

		private IList<IElectricStorageDeclarationInputData> _electricStorageElements;
		protected IXMLDeclarationVehicleData _vehicle;
		
		public XMLElectricStorageSystemDeclarationInputDataV24(
			IXMLDeclarationVehicleData vehicle, XmlNode componentNode, string sourceFile, bool obsolete = true)
			: base(componentNode, sourceFile)
		{
            _vehicle = vehicle;
			//SourceType = DataSourceType.XMLEmbedded;
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
							REESSPack = StorageTypeReader.CreateREESSInputData(capacitor, REESSType.SuperCap),
							Count = 1,
							StringId = 1,
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

                CheckVehicleBatteryData(electricStorages);
            }

			return electricStorages.Any() ? electricStorages : null;
		}

		protected virtual void CheckVehicleBatteryData(IList<IElectricStorageDeclarationInputData> electricStorages) { }

		#region Implementation of IXMLElectricStorageSystemDeclarationInputData

		public virtual IXMLREESSReader StorageTypeReader { protected get; set; }

		#endregion
		
		#region Implementation of IXMLResource

		protected override XNamespace SchemaNamespace => NAMESPACE_URI;
		protected override DataSourceType SourceType => DataSourceType.XMLEmbedded;

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

	public class XMLElectricStorageSystemDeclarationInputDataV27 : XMLElectricStorageSystemDeclarationInputDataV24
    {
		public static new readonly XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V27;
		public static new readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

		public XMLElectricStorageSystemDeclarationInputDataV27(IXMLDeclarationVehicleData vehicle, XmlNode componentNode, string sourceFile)
			: base(vehicle, componentNode, sourceFile, obsolete: false)
		{}

        protected override void CheckVehicleBatteryData(IList<IElectricStorageDeclarationInputData> electricStorages)
        {
            if (_vehicle.ArchitectureID.IsBatteryElectricVehicle() || _vehicle.ArchitectureID.IsFuelCellVehicle() || (_vehicle.HybridElectricHDV && _vehicle.OVC))
            {
                var batteries = electricStorages.Where(x => x.REESSPack.StorageType == REESSType.Battery);

                if (batteries.Any(x => (x.REESSPack as IBatteryPackDeclarationInputData).MinSOC == null))
                {
                    throw new VectoException("Battery SOCmin is undefined");
                }

                if (batteries.Any(x => (x.REESSPack as IBatteryPackDeclarationInputData).MaxSOC == null))
                {
                    throw new VectoException("Battery SOCmax is undefined");
                }

                if (batteries.Any(x => (x.REESSPack as IBatteryPackDeclarationInputData).DeteriorationPerformanceRatio == null))
                {
                    //Disabled this check because DeteriorationPerformanceRatio is not being used yet by OEMS.
                    //throw new VectoException("Battery DeteriorationPerformanceRatio is undefined");
                }
            }
        }
    }

    // ---------------------------------------------------------------------------------------

    public abstract class AbstractBatteryPackDeclarationInputDataProvider : AbstractCommonComponentType, IXMLBatteryPackDeclarationInputData
	{
		protected AbstractBatteryPackDeclarationInputDataProvider(XmlNode node, string source) : base(node, source) { }
		protected abstract XNamespace NamespaceURI { get; }

		#region Implementation of IREESSPackInputData

		public REESSType StorageType => REESSType.Battery;



		#endregion


		protected TableData _correctedInternalResistanceCurve = null;
		protected TableData _correctedMaxCurrentMap = null;

		#region Implementation of IBatteryPackDeclarationInputData

        public virtual double? MinSOC
		{
			get
			{
				// ancestor-or-self::*[local-name()='Battery']/*[local-name()='SOCmin']
				var node = BaseNode.SelectSingleNode($"ancestor-or-self::*[local-name()='{XMLNames.ElectricEnergyStorage_Battery}']/*[local-name()='{XMLNames.Battery_SOCmin}']");
				return node != null
					? node.InnerText.ToDouble() / 100
					: (double?)null;
			}
		}

		public virtual double? MaxSOC
		{
			get
			{
				// ancestor-or-self::*[local-name()='Battery']/*[local-name()='SOCmin']
				var node = BaseNode.SelectSingleNode($"ancestor-or-self::*[local-name()='{XMLNames.ElectricEnergyStorage_Battery}']/*[local-name()='{XMLNames.Battery_SOCmax}']");
				return node != null
					? node.InnerText.ToDouble() / 100
					: (double?)null;
            }
		}

        public virtual double? DeteriorationPerformanceRatio
        {
            get
            {
                // ancestor-or-self::*[local-name()='Battery']/*[local-name()='DeteriorationPerformanceRatio']
                var node = BaseNode.SelectSingleNode($"ancestor-or-self::*[local-name()='{XMLNames.ElectricEnergyStorage_Battery}']/*[local-name()='DeteriorationPerformanceRatio']");
                return node != null
                    ? node.InnerText.ToDouble() / 100
                    : (double?)null;
            }
        }

        public virtual BatteryType BatteryType => GetString(XMLNames.REESS_BatteryType).ParseEnum<BatteryType>();
		public virtual AmpereSecond Capacity => GetDouble(XMLNames.REESS_RatedCapacity).SI(Unit.SI.Ampere.Hour).Cast<AmpereSecond>();

		public virtual bool? ConnectorsSubsystemsIncluded => CertificationMethod == CertificationMethod.StandardValues
			? (bool?)null
			: GetBool(XMLNames.REESS_ConnectorsSubsystemsIncluded);

		public virtual bool? JunctionboxIncluded => CertificationMethod == CertificationMethod.StandardValues
			? (bool?)null
			: GetBool(XMLNames.REESS_JunctionboxIncluded);

		public virtual Kelvin TestingTemperature => CertificationMethod != CertificationMethod.StandardValues
			? GetDouble(XMLNames.REESS_TestingTemperature).DegCelsiusToKelvin()
			: null;

		public virtual TableData InternalResistanceCurve => _correctedInternalResistanceCurve ??
															(_correctedInternalResistanceCurve =
																GetInternalResistanceCurve());



		public virtual TableData VoltageCurve => ReadTableData(XMLNames.REESS_OCV, XMLNames.REESS_MapEntry,
			AttributeMappings.VoltageMap);

		public virtual TableData MaxCurrentMap =>
			_correctedMaxCurrentMap ?? (_correctedMaxCurrentMap = GetMaxCurrentMap());

		protected virtual TableData GetMaxCurrentMap()
		{
			return ReadTableData(XMLNames.REESS_CurrentLimits, XMLNames.REESS_MapEntry,
				AttributeMappings.MaxCurrentMap);
        }

		#endregion
		protected virtual TableData GetInternalResistanceCurve()
		{
			return ReadTableData(XMLNames.REESS_InternalResistanceCurve, XMLNames.REESS_MapEntry,
				AttributeMappings.InternalResistanceMap);
		}


		#region Overrides of AbstractXMLResource

        protected override XNamespace SchemaNamespace => NamespaceURI;
		protected override DataSourceType SourceType => DataSourceType.XMLFile;

		#endregion
	}


    public class XMLBatteryPackDeclarationInputDataMeasuredV23 : AbstractBatteryPackDeclarationInputDataProvider
	{
		public static readonly XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V23;
		public const string XSD_TYPE = "BatterySystemDataType";
		public static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

		public XMLBatteryPackDeclarationInputDataMeasuredV23(XmlNode componentNode, string sourceFile) : base(componentNode, sourceFile) { }

		#region Overrides of AbstractBatteryPackDeclarationInputDataProvider

		protected override XNamespace NamespaceURI => NAMESPACE_URI;

		#endregion
    }

	public class XMLBatteryPackDeclarationInputDataStandardV23 : AbstractBatteryPackDeclarationInputDataProvider
    {
		public static readonly XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V23;
		public const string XSD_TYPE = "BatterySystemStandardValuesDataType";
		public static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

		public XMLBatteryPackDeclarationInputDataStandardV23(XmlNode componentNode, string sourceFile) : base(componentNode, sourceFile) 
		{
#if PROHIBIT_OLD_XML
			throw new VectoException($"{XSD_TYPE} v2.3 is no longer supported. Use newer version instead.");
#endif
		}

		#region Overrides of AbstractBatteryPackDeclarationInputDataProvider

		protected override XNamespace NamespaceURI => NAMESPACE_URI;

        protected override TableData GetInternalResistanceCurve()
		{
			var corrected = base.GetInternalResistanceCurve();
			var socMap = BatterySOCReader.Create(VoltageCurve);
			var vNom = socMap.Lookup(0.5);

			Volt u_cell;
			switch (BatteryType)
			{
				case BatteryType.HPBS:
					u_cell = 3.3.SI<Volt>();
					break;
				case BatteryType.HEBS:
					u_cell = 3.7.SI<Volt>();
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}

			var dcir_corr = vNom / u_cell;


			var nrCols = corrected.Columns.Count;
			//mOhm
			foreach (DataRow row in corrected.Rows) {
				for (var i = 1; i < nrCols; i++) {
					var uncorr = row.ParseDouble(i);
					row[i] = (uncorr * dcir_corr.Value()).ToXMLFormat(2);
				}
			}

			return corrected;
		}

		protected override TableData GetMaxCurrentMap()
		{
			var entries = base.GetMaxCurrentMap();
			if (entries.Rows.Count != 2) {
				throw new VectoException("exactly 2 entries for the max current map are expected for standard values");
			}

			var maxChargeCurrentRow = entries.AsEnumerable()
				.FirstOrDefault(x => x.Field<string>(XMLNames.REESS_CurrentLimits_SoC).ToInt() == 0);
			var maxDischargeCurrentRow = entries.AsEnumerable()
				.FirstOrDefault(x => x.Field<string>(XMLNames.REESS_CurrentLimits_SoC).ToInt() == 100);
			if (maxChargeCurrentRow == null) {
				throw new VectoException("no max current entry for SoC 0% found in input data!");
			}
			if (maxDischargeCurrentRow == null) {
				throw new VectoException("no max current entry for SoC 100% found in input data!");
			}

            var maxChargeCurrent = maxChargeCurrentRow.Field<string>(BatteryMaxCurrentReader.Fields.MaxChargeCurrent).ToDouble() * 0.9;
			var maxDischargeCurrent = maxDischargeCurrentRow.Field<string>(BatteryMaxCurrentReader.Fields.MaxDischargeCurrent).ToDouble() * 5;
			var newMap = new string[] {
				$"{BatteryMaxCurrentReader.Fields.StateOfCharge}, {BatteryMaxCurrentReader.Fields.MaxChargeCurrent}, {BatteryMaxCurrentReader.Fields.MaxDischargeCurrent}",
				$"  0, {maxChargeCurrent.ToXMLFormat(2)}, 0",
				$" 30, {maxChargeCurrent.ToXMLFormat(2)}, {maxDischargeCurrent.ToXMLFormat(2)}",
				$" 80, {maxChargeCurrent.ToXMLFormat(2)}, {maxDischargeCurrent.ToXMLFormat(2)}",
				$"100, 0, {maxDischargeCurrent.ToXMLFormat(2)}"
			};
			return VectoCSVFile.ReadStream(newMap.Join(Environment.NewLine).ToStream());
		}

		#endregion
	}

    public class XMLBatteryPackDeclarationInputDataMeasuredV26 : XMLBatteryPackDeclarationInputDataMeasuredV23
    {
        public static new readonly XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V26;
        public static new readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

        public XMLBatteryPackDeclarationInputDataMeasuredV26(XmlNode componentNode, string sourceFile) : base(componentNode, sourceFile) { }
    }

    // ---------------------------------------------------------------------------------------

    public class XMLBatteryPackDeclarationInputDataStandardV26 : AbstractBatteryPackDeclarationInputDataProvider
	{
        public static readonly XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V26;
        public const string XSD_TYPE = "BatterySystemStandardValuesDataType";
        public static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

		private SpecificResistances _specificResistances;

        public XMLBatteryPackDeclarationInputDataStandardV26(XmlNode componentNode, string sourceFile) : base(componentNode, sourceFile) 
		{
			_specificResistances = new SpecificResistances();
        }

        protected override XNamespace NamespaceURI => NAMESPACE_URI;

        protected override TableData GetMaxCurrentMap()
        {
            var entries = base.GetMaxCurrentMap();

			var requiredEntrySoCs = new List<int>() { 0, 30, 80, 100 };

			foreach (var soc in requiredEntrySoCs)
			{
                if (!entries.AsEnumerable().Any(z => z.Field<string>(XMLNames.REESS_CurrentLimits_SoC).ToInt() == soc))
				{
					throw new VectoException($"Battery does not contain Current Limit entry for SoC: {soc}");
                }
            }

			return entries;
        }

        protected override TableData GetInternalResistanceCurve()
        {
			const int SoC = 50;
			
			var mapSoC = BatterySOCReader.Create(VoltageCurve);
			if (!mapSoC.ContainsSoC(SoC / 100.0))
			{
				throw new VectoException($"Battery SoC map does not contain entry with SoC: {SoC}");
			}

            var vNom = mapSoC.Lookup(SoC / 100.0);
			var minCells = vNom / 4.0.SI<Volt>();
			var maxCells = vNom / 3.0.SI<Volt>();

            var resistances = base.GetInternalResistanceCurve();
            var specificResistanceData = _specificResistances.Lookup(BatteryType);

            //var rowSoC = resistances.AsEnumerable().First();
			foreach (DataRow rowSoC in resistances.AsEnumerable()) {
				var soc = rowSoC.ParseDouble(0);
				for (var i = 1; i < resistances.Columns.Count; i++) {
					var resistance = rowSoC.ParseDouble(i).SI(Unit.SI.Milli.Ohm).Cast<Ohm>();
					var resistanceName = resistances.Columns[i].ColumnName;

					var specificResistance = specificResistanceData.GetValue(resistanceName);
					var cellResistance = specificResistance / Capacity;

					var minResistance = cellResistance * minCells.Value();
					var maxResistance = cellResistance * maxCells.Value();

					if ((resistance < minResistance) || (resistance > maxResistance)) {
						Log.Warn(
							$@"Battery {resistanceName}: {resistance.AsMilliOhm}, for SoC: {soc}, out of range [{minResistance.AsMilliOhm.ToString("N2")}, {maxResistance.AsMilliOhm.ToString("N2")}]");
					}
				}
			}

			return resistances;
        }
    }

	public class XMLBatteryPackDeclarationInputDataMeasuredV01 : AbstractBatteryPackDeclarationInputDataProvider
	{
		public static readonly XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_MULTISTAGE_BUS_VEHICLE_NAMESPACE_VO1;
		public const string XSD_TYPE = "BatterySystemDataType";
		public static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

		public XMLBatteryPackDeclarationInputDataMeasuredV01(XmlNode componentNode, string sourceFile) : base(componentNode, sourceFile) { }

		#region Overrides of AbstractBatteryPackDeclarationInputDataProvider

		protected override XNamespace NamespaceURI => NAMESPACE_URI;

		#endregion
	}

	public class XMLBatteryPackDeclarationInputDataStandardV01 : AbstractBatteryPackDeclarationInputDataProvider
	{
		public static readonly XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_MULTISTAGE_BUS_VEHICLE_NAMESPACE_VO1;
		public const string XSD_TYPE = "BatterySystemStandardValuesDataType";
		public static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

		public XMLBatteryPackDeclarationInputDataStandardV01(XmlNode componentNode, string sourceFile) : base(componentNode, sourceFile) { }

		#region Overrides of AbstractBatteryPackDeclarationInputDataProvider

		protected override XNamespace NamespaceURI => NAMESPACE_URI;

		#endregion
	}

    // ---------------------------------------------------------------------------------------

    public class XMLSuperCapDeclarationInputDataV23 : AbstractCommonComponentType, IXMLSuperCapDeclarationInputData
	{
		public static readonly XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V23;
		public const string XSD_TYPE = "CapacitorSystemDataType";
		public static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);
		
		public XMLSuperCapDeclarationInputDataV23(XmlNode componentNode, string sourceFile, bool disallowed = true) : base(componentNode, sourceFile)
		{
#if PROHIBIT_OLD_XML
			if (disallowed && CertificationMethod == CertificationMethod.StandardValues)
			{
				throw new VectoException($"Capacitor v2.3 is no longer supported. Use newer version instead.");
			}
#endif
		}
		
		#region Implementation of IREESSPackInputData

		public REESSType StorageType => REESSType.SuperCap;

		#endregion

		#region Implementation of ISuperCapDeclarationInputData

		public virtual Farad Capacity => GetDouble(XMLNames.Capacitor_Capacitance).SI<Farad>();
		public virtual Ohm InternalResistance
		{
			get
			{
				var value = GetDouble(XMLNames.Capacitor_InternalResistance).SI<Ohm>();
				if (value.IsEqual(0)) {
					value = DeclarationData.SuperCapMinInternalResistance;
				}

				if (CertificationMethod == CertificationMethod.StandardValues) {
					var estCellCnt = MaxVoltage / DeclarationData.SuperCapReferenceVoltage;
					value = value / estCellCnt * DeclarationData.SuperCapInternalResistanceStdValuesCorrection;
					if (value < DeclarationData.SuperCapMinInternalResistance) {
						value = DeclarationData.SuperCapMinInternalResistance;
					}
				}
				return value;
			}
		}

		public virtual Volt MinVoltage => GetDouble(XMLNames.Capacitor_MinVoltage).SI<Volt>();
		public virtual Volt MaxVoltage => GetDouble(XMLNames.Capacitor_MaxVoltage).SI<Volt>();
		public virtual Ampere MaxCurrentCharge => GetDouble(XMLNames.Capacitor_MaxChargingCurrent).SI<Ampere>();
		public virtual Ampere MaxCurrentDischarge => GetDouble(XMLNames.Capacitor_MaxDischargingCurrent).SI<Ampere>();

		public virtual Kelvin TestingTemperature => 
			ElementExists(XMLNames.REESS_TestingTemperature)
				? GetDouble(XMLNames.REESS_TestingTemperature).DegCelsiusToKelvin() : null;
		
		#endregion
		
		#region Overrides of AbstractXMLResource

		protected override XNamespace SchemaNamespace => NAMESPACE_URI;
		protected override DataSourceType SourceType => DataSourceType.XMLEmbedded;

		#endregion
	}

	// ---------------------------------------------------------------------------------------

    public class XMLSuperCapDeclarationInputDataV26 : XMLSuperCapDeclarationInputDataV23
	{
        public static new readonly XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V26;
        public const string XSD_TYPE = "CapacitorSystemDataType";
        public static new readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

        public XMLSuperCapDeclarationInputDataV26(XmlNode componentNode, string sourceFile) : base(componentNode, sourceFile, false)
        {}

        public override Ohm InternalResistance => GetDouble(XMLNames.Capacitor_InternalResistance).SI(Unit.SI.Milli.Ohm).Cast<Ohm>();
    }

    // ---------------------------------------------------------------------------------------

	public class XMLSuperCapDeclarationInputDataV01 : XMLSuperCapDeclarationInputDataV23
	{
		public static readonly XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_MULTISTAGE_BUS_VEHICLE_NAMESPACE_VO1;
		public const string XSD_TYPE = "CapacitorSystemDataType";

		public static readonly string QUALIFIED_XSD_TYPE =
			XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

		public XMLSuperCapDeclarationInputDataV01(XmlNode componentNode, string sourceFile) : base(componentNode, sourceFile, false) { }

		#region Overrides of XMLSuperCapDeclarationInputDataV24

		public override Ohm InternalResistance => null;

		#endregion
	}

	//===================

	public class XMLElectricStorageSystemDeclarationInputDataV01 : XMLElectricStorageSystemDeclarationInputDataV24
	{
		public static readonly XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_MULTISTAGE_BUS_VEHICLE_NAMESPACE_VO1;
		public const string XSD_TYPE = "ElectricEnergyStorageType";
		public static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);


		public XMLElectricStorageSystemDeclarationInputDataV01(IXMLDeclarationVehicleData vehicle, XmlNode componentNode, string sourceFile) : base(vehicle, componentNode, sourceFile) { }
	}
}
