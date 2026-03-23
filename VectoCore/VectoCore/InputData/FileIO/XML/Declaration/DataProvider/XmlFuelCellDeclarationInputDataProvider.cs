using Ninject;
using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Factory;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Interfaces;
using TUGraz.VectoCore.InputData.Impl;
using TUGraz.VectoCore.Models.GenericModelData;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.InputData.FileIO.XML.Declaration.DataProvider
{
	public abstract class XMLFuelCellSystemDeclarationInputDataProvider : AbstractCommonComponentType, IXMLFuelCellSystemDeclarationInputData, IComponentInputData
	{
		protected XMLFuelCellSystemDeclarationInputDataProvider(XmlNode node, string source)
			: base(node, source)
		{
			SourceType = DataSourceType.XMLEmbedded;
		}

		[Inject]
		public virtual IDeclarationInjectFactory Factory { protected get; set; }

		protected override DataSourceType SourceType { get; }

		public virtual int Count => Convert.ToInt32(GetDouble(XMLNames.FuelCell_Count));

		public virtual Watt MinPower => ElementExists(XMLNames.FuelCell_MinPower) ? GetDouble(XMLNames.FuelCell_MinPower).SI(Unit.SI.Watt).Cast<Watt>() : null;

		public virtual Watt MaxPower => ElementExists(XMLNames.FuelCell_MaxPower) ? GetDouble(XMLNames.FuelCell_MaxPower).SI(Unit.SI.Watt).Cast<Watt>() : null;

		public virtual List<IFuelCellModuleDeclarationInputData> FuelCellModules => CreateFuelCellModule();

		public virtual List<IFuelCellModuleDeclarationInputData> CreateFuelCellModule()
		{
			var modules = new List<IFuelCellModuleDeclarationInputData>();

			var xmlFuelCellModules = GetNodes(XMLNames.FuelCell_Module);
			foreach (XmlNode moduleNode in xmlFuelCellModules)
			{
				var componentNode = GetNode(XMLNames.FuelCell_Cell, moduleNode);
				var dataNode = GetNode(XMLNames.ComponentDataWrapper, componentNode);
				var version = XMLHelper.GetXsdType(dataNode.SchemaInfo.SchemaType);

				string sourceFile = null;
				IFuelCellDeclarationInputData fuelCellData = Factory.CreateFuelCellInputData(version, componentNode, sourceFile);
				var cellModule = new FuelCellModule()
				{
					Count = Count,
					FuelCell = fuelCellData,
					MinPower = MinPower,
					MaxPower = MaxPower,
				};

				modules.Add(cellModule);
			}

			return modules;
		}
	}

	public abstract class XMLFuelCellDeclarationInputDataProvider : AbstractCommonComponentType, IComponentInputData, IXMLFuelCellDeclarationInputData
	{
		protected XMLFuelCellDeclarationInputDataProvider(XmlNode componentNode, string sourceFile)
			: base(componentNode, sourceFile)
		{
			SourceType = DataSourceType.XMLEmbedded;
		}

		protected override DataSourceType SourceType { get; }

		public virtual Watt FCSRatedPower => ElementExists(XMLNames.FuelCell_FCSRatedPower) ?
			GetDouble(XMLNames.FuelCell_FCSRatedPower).SI(Unit.SI.Kilo.Watt).Cast<Watt>() : null;

		public virtual TableData FuelCellPowerOutputConsumptionMap => ReadOutputPowerConsumptionMap();

		protected virtual TableData ReadOutputPowerConsumptionMap()
		{
			return ReadTableData(
				XMLNames.FuelCell_PowerOutputConsumptionMap,
				XMLNames.FuelCell_ConsumptionEntry,
				new Dictionary<string, string>
				{
					{ XMLNames.FuelCell_PowerOutput, XMLNames.FuelCell_PowerOutput },
					{ XMLNames.FuelCell_Consumption, XMLNames.FuelCell_Consumption }
				});
		}
	}

	public class XMLFuelCellDeclarationInputDataProviderV26 : XMLFuelCellDeclarationInputDataProvider
	{
		public static readonly XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V26;
		public const string XSD_TYPE = "FuelCellDataDeclarationType";
		public static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

		public XMLFuelCellDeclarationInputDataProviderV26(XmlNode componentNode, string sourceFile)
			: base(componentNode, sourceFile)
		{
		}

		protected override XNamespace SchemaNamespace => NAMESPACE_URI;
	}

	public class XMLFuelCellDeclarationInputDataProviderV27 : XMLFuelCellDeclarationInputDataProvider
	{
		public static readonly XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V27;
		public const string XSD_TYPE = "FuelCellDataDeclarationType";
		public static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

		public XMLFuelCellDeclarationInputDataProviderV27(XmlNode componentNode, string sourceFile)
			: base(componentNode, sourceFile)
		{
		}

		protected override XNamespace SchemaNamespace => NAMESPACE_URI;
	}

	public class XMLFuelCellSystemDeclarationInputDataProviderV26 : XMLFuelCellSystemDeclarationInputDataProvider, IXMLFuelCellSystemDeclarationInputData
	{
		public XMLFuelCellSystemDeclarationInputDataProviderV26(XmlNode componentNode, string sourceFile)
			: base(componentNode, sourceFile)
		{
		}

		protected override XNamespace SchemaNamespace => XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V26;

		public static XNamespace NAMESPACE_URI => XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V26;

		public static string QUALIFIED_XSD_TYPE => XMLHelper.CombineNamespace(
			NAMESPACE_URI.NamespaceName,
			"FuelCellSystemDeclarationType");
	}

	public class XMLFuelCellSystemDeclarationInputDataProviderV27 : XMLFuelCellSystemDeclarationInputDataProviderV26
	{
		public XMLFuelCellSystemDeclarationInputDataProviderV27(XmlNode componentNode, string sourceFile)
			: base(componentNode, sourceFile)
		{
		}

		protected override XNamespace SchemaNamespace => XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V27;

		public static new XNamespace NAMESPACE_URI => XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V27;

		public static new string QUALIFIED_XSD_TYPE => XMLHelper.CombineNamespace(
			NAMESPACE_URI.NamespaceName,
			"FuelCellSystemDeclarationType");
	}

	public class XMLDeclarationMultistagePrimaryVehicleBusFuelCellDataProviderV11 : XMLFuelCellSystemDeclarationInputDataProviderV27
	{
        public XMLDeclarationMultistagePrimaryVehicleBusFuelCellDataProviderV11(XmlNode componentNode, string sourceFile)
            : base(componentNode, sourceFile)
        {}

        public new static readonly XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_MULTISTAGE_BUS_VEHICLE_NAMESPACE_V11;

        public static new string QUALIFIED_XSD_TYPE => XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, "FuelCellSystemType");

        public override List<IFuelCellModuleDeclarationInputData> CreateFuelCellModule()
        {
            var modules = new List<IFuelCellModuleDeclarationInputData>();

            var xmlFuelCellModules = GetNodes(XMLNames.FuelCell_Module);
            foreach (XmlNode moduleNode in xmlFuelCellModules)
            {
                var componentNode = GetNode("Data", moduleNode);

                var fuelCellData = new FuelCellInputData();

				fuelCellData.Manufacturer = GetString("Manufacturer");
				fuelCellData.Model = GetString("Model");
				fuelCellData.CertificationMethod = GetString("CertificationMethod").ParseEnum<CertificationMethod>();
				fuelCellData.CertificationNumber = ElementExists("CertificationNumber") ? GetString("CertificationNumber") : null;
				fuelCellData.AppVersion = GetString("AppVersion");
				fuelCellData.FCSRatedPower = GetDouble("FCSRatedPower").SI<Watt>();
				fuelCellData.FuelCellPowerOutputConsumptionMap = GenericBusFuelCellData.CreateFuelCellPowerOutputMap(fuelCellData.FCSRatedPower);

                var cellModule = new FuelCellModule()
                {
                    Count = Count,
                    FuelCell = fuelCellData,
                    MinPower = MinPower,
                    MaxPower = MaxPower,
                };

                modules.Add(cellModule);
            }

            return modules;
        }
    }

}
