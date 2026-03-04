using System;
using System.Xml.Linq;
using System.Xml;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCore.Utils;
using Ninject;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Factory;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Interfaces;
using System.Xml.Schema;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.Models;

namespace TUGraz.VectoCore.InputData.FileIO.XML.Declaration.DataProvider
{
    public abstract class XMLDeclarationAxlePowertrainDataProvider : AbstractCommonComponentType, IXMLAxlePowertrainDeclarationInputData, IComponentInputData
    {
        protected IRetarderInputData _retarderInputData;
        protected IAngledriveInputData _angledriveInputData;
        protected IXMLDeclarationVehicleData Vehicle;
        protected ElectricMachineEntry<IElectricMotorDeclarationInputData> _electricMotor;
        protected IGearboxDeclarationInputData _gearboxInputData;
        protected ITorqueConverterDeclarationInputData _torqueConverterInputData;
        protected IAxleGearInputData _axlegearInputData;
        protected IIEPCDeclarationInputData _iepcDeclarationInputData;

        protected XMLDeclarationAxlePowertrainDataProvider(IXMLDeclarationVehicleData vehicle, XmlNode node, string source) : base(node, source)
        {
            SourceType = DataSourceType.XMLEmbedded;
            Vehicle = vehicle;
        }

        protected override XNamespace SchemaNamespace => XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V27;

        public static XNamespace NAMESPACE_URI => XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V27;

        [Inject]
        public virtual IDeclarationInjectFactory Factory { protected get; set; }

        protected override DataSourceType SourceType { get; }

        public AxleNumber AxleNumber => new AxleNumber(int.Parse(GetAttribute(BaseNode, "axleNumber")));

        public virtual ArchitectureID Architecture => ArchitectureID.UNKNOWN;

        public virtual IAngledriveInputData AngledriveInputData =>
            _angledriveInputData ?? (_angledriveInputData = CreateComponent(XMLNames.Component_Angledrive, AngledriveCreator, true));

        public virtual IAxleGearInputData AxleGearInputData => _axlegearInputData ?? 
            (_axlegearInputData = ElementExists(XMLNames.Component_Axlegear) ? CreateComponent(XMLNames.Component_Axlegear, AxlegearCreator) : null);

        public virtual ElectricMachineEntry<IElectricMotorDeclarationInputData> ElectricMotor => 
            _electricMotor ?? (_electricMotor = ElementExists(XMLNames.Component_ElectricMachine) ? GetElectricMotor() : null);

        public virtual IGearboxDeclarationInputData GearboxInputData => _gearboxInputData ?? 
            (_gearboxInputData = ElementExists(XMLNames.Component_Gearbox) ? CreateComponent(XMLNames.Component_Gearbox, GearboxCreator) : null);

        public virtual IIEPCDeclarationInputData IEPCInputData => _iepcDeclarationInputData ??
            (_iepcDeclarationInputData = ElementExists(XMLNames.IEPC_Component) ? CreateComponent(XMLNames.IEPC_Component, IEPCCreator) : null);

        public virtual IPTOTransmissionInputData PTOTransmissionInputData => Vehicle.GetPTOTransmissionInputData(AxleNumber);

        public virtual IRetarderInputData RetarderInputData => 
            _retarderInputData ?? (_retarderInputData = CreateComponent(XMLNames.Component_Retarder, RetarderCreator, true));

        public virtual ITorqueConverterDeclarationInputData TorqueConverterInputData => _torqueConverterInputData ?? 
            (_torqueConverterInputData = ElementExists(XMLNames.Component_TorqueConverter) 
                ? CreateComponent(XMLNames.Component_TorqueConverter, TorqueConverterCreator) 
                : null);

        protected virtual ElectricMachineEntry<IElectricMotorDeclarationInputData> GetElectricMotor()
        {
            return CreateComponent(XMLNames.Component_ElectricMachine, ElectricMachineCreator);
        }

        protected virtual IIEPCDeclarationInputData IEPCCreator(string version, XmlNode componentNode, string sourceFile)
        {
            return Factory.CreateIEPCData(version, Vehicle, componentNode, sourceFile);
        }

        protected virtual IAxleGearInputData AxlegearCreator(string version, XmlNode componentNode, string sourceFile)
        {
            return Factory.CreateAxlegearData(version, Vehicle, componentNode, sourceFile);
        }

        protected virtual ITorqueConverterDeclarationInputData TorqueConverterCreator(string version, XmlNode componentNode, string sourceFile)
        {
            if (version == null)
            {
                return new XMLDeclarationTorqueConverterDataProviderV10(Vehicle, componentNode, sourceFile);
            }

            return Factory.CreateTorqueconverterData(version, Vehicle, componentNode, sourceFile);
        }

        protected virtual IGearboxDeclarationInputData GearboxCreator(string version, XmlNode componentNode, string sourceFile)
        {
            var gbx = Factory.CreateGearboxData(version, Vehicle, componentNode, sourceFile);
            gbx.Reader = Factory.CreateGearboxReader(version, Vehicle, componentNode);
            return gbx;
        }

        protected virtual ElectricMachineEntry<IElectricMotorDeclarationInputData> ElectricMachineCreator(string version,
            XmlNode componentNode, string sourcefile)
        {
            if (componentNode == null)
            {
                return null;
            }

            var electricMachine = Factory.CreateElectricMachinesData(version, Vehicle, componentNode, sourcefile);
            electricMachine.ElectricMachineSystemReader = Factory.CreateElectricMotorReader(version, Vehicle, componentNode, sourcefile);
            return electricMachine.Entries[0];
        }

        protected virtual T CreateComponent<T>(
            string component, Func<string, XmlNode, string, T> componentCreator, bool createDummy = false)
        {
            var componentNode = GetComponentNode(component);
            var dataNode =
                componentNode?.SelectSingleNode($"./*[local-name()='{XMLNames.ComponentDataWrapper}']");
            if (componentNode != null)
            {
                var type = (dataNode ?? componentNode).SchemaInfo.SchemaType;
                var version = XMLHelper.GetXsdType(type);
                if (string.IsNullOrWhiteSpace(version))
                {
                    version = XMLHelper.GetVersionFromNamespaceUri(((dataNode ?? componentNode).SchemaInfo.SchemaType?.Parent as XmlSchemaElement)?.QualifiedName.Namespace);
                }
                return componentCreator(version, componentNode, SourceFile);
            }

            if (createDummy)
            {
                try
                {
                    return componentCreator(null, null, null);
                }
                catch (Exception e)
                {
                    throw new VectoException("failed to create dummy instance for component {0}", e, component);
                }
            }

            throw new VectoException("Component {0} not found!", component);
        }

        protected virtual XmlNode GetComponentNode(string component)
        {
            var componentNode = BaseNode.LocalName == component
                ? BaseNode
                : BaseNode.SelectSingleNode(XMLHelper.QueryLocalName(component));

            return componentNode;
        }

        protected virtual IRetarderInputData RetarderCreator(string version, XmlNode componentNode, string sourceFile)
        {
            if (version == null)
            {
                return Factory.CreateRetarderData(XMLDeclarationRetarderDataProviderV10.AXLE_NUMBER_VERSION, AxleNumber, Vehicle, componentNode, sourceFile);
            }

            return Factory.CreateRetarderData(version, Vehicle, componentNode, sourceFile);
        }

        protected virtual IAngledriveInputData AngledriveCreator(string version, XmlNode componentNode, string sourceFile)
        {
            if (version == null)
            {
                return Factory.CreateAngledriveData(XMLDeclarationAngledriveDataProviderV10.AXLE_NUMBER_VERSION, AxleNumber, Vehicle, componentNode, sourceFile);
            }

            return Factory.CreateAngledriveData(version, Vehicle, componentNode, sourceFile);
        }
    }

    public abstract class XMLDeclarationMultistage_AxlePowertrain_DataProviderV11 : XMLDeclarationAxlePowertrainDataProvider
    {
        protected override XNamespace SchemaNamespace => XMLDefinitions.DECLARATION_MULTISTAGE_BUS_VEHICLE_NAMESPACE_V11;

        public new static XNamespace NAMESPACE_URI => XMLDefinitions.DECLARATION_MULTISTAGE_BUS_VEHICLE_NAMESPACE_V11;

        public XMLDeclarationMultistage_AxlePowertrain_DataProviderV11(IXMLDeclarationVehicleData vehicle, XmlNode componentNode, string sourceFile)
            : base(vehicle, componentNode, sourceFile)
        {}

        public override IGearboxDeclarationInputData GearboxInputData => _gearboxInputData ??
            (_gearboxInputData = ElementExists("Transmission") ? CreateComponent("Transmission", GearboxCreator) : null);
    }

    public class XMLDeclarationMultistage_AxlePowertrain_EM2_DataProviderV11 : XMLDeclarationMultistage_AxlePowertrain_DataProviderV11
    {
        public static string QUALIFIED_XSD_TYPE => XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, "xEV_EM2_Powertrain_VIFType");

        public XMLDeclarationMultistage_AxlePowertrain_EM2_DataProviderV11(IXMLDeclarationVehicleData vehicle, XmlNode componentNode, string sourceFile)
            : base(vehicle, componentNode, sourceFile)
        {}

        public override ArchitectureID Architecture => (Vehicle.Components.FuelCellSystem != null)
            ? ArchitectureID.F2
            : (Vehicle.Components.EngineInputData != null)
                ? ArchitectureID.S2
                : ArchitectureID.E2;
    }

    public class XMLDeclarationMultistage_AxlePowertrain_EM3_DataProviderV11 : XMLDeclarationMultistage_AxlePowertrain_DataProviderV11
    {
        public static string QUALIFIED_XSD_TYPE => XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, "xEV_EM3_Powertrain_VIFType");

        public XMLDeclarationMultistage_AxlePowertrain_EM3_DataProviderV11(IXMLDeclarationVehicleData vehicle, XmlNode componentNode, string sourceFile)
            : base(vehicle, componentNode, sourceFile)
        {}

        public override ArchitectureID Architecture => (Vehicle.Components.FuelCellSystem != null)
            ? ArchitectureID.F3
            : (Vehicle.Components.EngineInputData != null)
                ? ArchitectureID.S3
                : ArchitectureID.E3;
    }

    public class XMLDeclarationMultistage_AxlePowertrain_EM4_DataProviderV11 : XMLDeclarationMultistage_AxlePowertrain_DataProviderV11
    {
        public static string QUALIFIED_XSD_TYPE => XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, "xEV_EM4_Powertrain_VIFType");

        public XMLDeclarationMultistage_AxlePowertrain_EM4_DataProviderV11(IXMLDeclarationVehicleData vehicle, XmlNode componentNode, string sourceFile)
            : base(vehicle, componentNode, sourceFile)
        { }

        public override ArchitectureID Architecture => (Vehicle.Components.FuelCellSystem != null)
            ? ArchitectureID.F4
            : (Vehicle.Components.EngineInputData != null)
                ? ArchitectureID.S4
                : ArchitectureID.E4;
    }

    public class XMLDeclarationMultistage_AxlePowertrain_IEPC_DataProviderV11 : XMLDeclarationMultistage_AxlePowertrain_DataProviderV11
    {
        public static string QUALIFIED_XSD_TYPE => XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, "xEV_IEPC_Powertrain_VIFType");

        public XMLDeclarationMultistage_AxlePowertrain_IEPC_DataProviderV11(IXMLDeclarationVehicleData vehicle, XmlNode componentNode, string sourceFile)
            : base(vehicle, componentNode, sourceFile)
        { }

        public override ArchitectureID Architecture => (Vehicle.Components.FuelCellSystem != null)
            ? ArchitectureID.F_IEPC
            : (Vehicle.Components.EngineInputData != null)
                ? ArchitectureID.S_IEPC
                : ArchitectureID.E_IEPC;
    }

    public class XMLDeclaration_AxlePowertrain_EM2_DataProviderV27 : XMLDeclarationAxlePowertrainDataProvider
    {
        public static string QUALIFIED_XSD_TYPE => XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, "xEV_EM2_Powertrain_Type");

        public XMLDeclaration_AxlePowertrain_EM2_DataProviderV27(IXMLDeclarationVehicleData vehicle, XmlNode componentNode, string sourceFile)
            : base(vehicle, componentNode, sourceFile)
        {
        }

        public override ArchitectureID Architecture => (Vehicle.Components.FuelCellSystem != null) 
            ? ArchitectureID.F2 
            : (Vehicle.Components.EngineInputData != null) 
                ? ArchitectureID.S2 
                : ArchitectureID.E2;
    }

    public class XMLDeclaration_AxlePowertrain_EM3_DataProviderV27 : XMLDeclarationAxlePowertrainDataProvider
    {
        public static string QUALIFIED_XSD_TYPE => XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, "xEV_EM3_Powertrain_Type");

        public XMLDeclaration_AxlePowertrain_EM3_DataProviderV27(IXMLDeclarationVehicleData vehicle, XmlNode componentNode, string sourceFile)
            : base(vehicle, componentNode, sourceFile)
        {
        }

        public override ArchitectureID Architecture => (Vehicle.Components.FuelCellSystem != null)
            ? ArchitectureID.F3
            : (Vehicle.Components.EngineInputData != null)
                ? ArchitectureID.S3
                : ArchitectureID.E3;
    }

    public class XMLDeclaration_AxlePowertrain_EM4_DataProviderV27 : XMLDeclarationAxlePowertrainDataProvider
    {
        public static string QUALIFIED_XSD_TYPE => XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, "xEV_EM4_Powertrain_Type");

        public XMLDeclaration_AxlePowertrain_EM4_DataProviderV27(IXMLDeclarationVehicleData vehicle, XmlNode componentNode, string sourceFile)
            : base(vehicle, componentNode, sourceFile)
        {
        }

        public override ArchitectureID Architecture => (Vehicle.Components.FuelCellSystem != null)
            ? ArchitectureID.F4
            : (Vehicle.Components.EngineInputData != null)
                ? ArchitectureID.S4
                : ArchitectureID.E4;
    }

    public class XMLDeclaration_AxlePowertrain_IEPC_DataProviderV27 : XMLDeclarationAxlePowertrainDataProvider
    {
        public static string QUALIFIED_XSD_TYPE => XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, "xEV_IEPC_Powertrain_Type");

        public XMLDeclaration_AxlePowertrain_IEPC_DataProviderV27(IXMLDeclarationVehicleData vehicle, XmlNode componentNode, string sourceFile)
            : base(vehicle, componentNode, sourceFile)
        {
        }

        public override ArchitectureID Architecture => (Vehicle.Components.FuelCellSystem != null)
            ? ArchitectureID.F_IEPC
            : (Vehicle.Components.EngineInputData != null)
                ? ArchitectureID.S_IEPC
                : ArchitectureID.E_IEPC;
    }

    public interface IXMLAxlePowertrainDeclarationInputData : IAxlePowertrainDeclarationInputData, IXMLResource
    { }
}
