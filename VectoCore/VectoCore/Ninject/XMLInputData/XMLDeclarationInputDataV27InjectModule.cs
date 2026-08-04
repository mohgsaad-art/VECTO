using Ninject.Modules;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.DataProvider;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.DataProvider.v27;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Interfaces;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Reader;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Reader.Impl;

namespace TUGraz.VectoCore.Ninject.XMLInputData
{
    public class XMLDeclarationInputDataV27InjectModule : NinjectModule
    {
        public override void Load()
        {
            // Job
            Bind<IXMLDeclarationInputData>().To<XMLDeclarationInputDataProviderV27>().Named(XMLDeclarationInputDataProviderV27.QUALIFIED_XSD_TYPE);
            Bind<IXMLDeclarationInputDataReader>().To<XMLDeclarationInputReaderV27>().Named(XMLDeclarationInputReaderV27.QUALIFIED_XSD_TYPE);
            Bind<IXMLDeclarationJobInputData>().To<XMLDeclarationJobInputDataProviderV27>().Named(XMLDeclarationJobInputDataProviderV27.QUALIFIED_XSD_TYPE);
            Bind<IXMLJobDataReader>().To<XMLJobDataReaderV27>().Named(XMLJobDataReaderV27.QUALIFIED_XSD_TYPE);

            // Vehicle - Heavy Lorry
            Bind<IXMLDeclarationVehicleData>().To<XMLDeclaration_Exempted_HeavyLorry_DataProviderV27>()
                .Named(XMLDeclaration_Exempted_HeavyLorry_DataProviderV27.QUALIFIED_XSD_TYPE);

            Bind<IXMLDeclarationVehicleData>().To<XMLDeclaration_Conventional_HeavyLorry_DataProviderV27>()
                .Named(XMLDeclaration_Conventional_HeavyLorry_DataProviderV27.QUALIFIED_XSD_TYPE);

            Bind<IXMLDeclarationVehicleData>().To<XMLDeclaration_PHEV_HeavyLorry_DataProviderV27>()
                .Named(XMLDeclaration_PHEV_HeavyLorry_DataProviderV27.QUALIFIED_XSD_TYPE);

            Bind<IXMLDeclarationVehicleData>().To<XMLDeclaration_PEV_HeavyLorry_DataProviderV27>()
                .Named(XMLDeclaration_PEV_HeavyLorry_DataProviderV27.QUALIFIED_XSD_TYPE);

            Bind<IXMLDeclarationVehicleData>().To<XMLDeclaration_PEV_IEPC_HeavyLorry_DataProviderV27>()
                .Named(XMLDeclaration_PEV_IEPC_HeavyLorry_DataProviderV27.QUALIFIED_XSD_TYPE);

            Bind<IXMLDeclarationVehicleData>().To<XMLDeclaration_SHEV_HeavyLorry_DataProviderV27>()
                .Named(XMLDeclaration_SHEV_HeavyLorry_DataProviderV27.QUALIFIED_XSD_TYPE);

            Bind<IXMLDeclarationVehicleData>().To<XMLDeclaration_SHEV_IEPC_HeavyLorry_DataProviderV27>()
                .Named(XMLDeclaration_SHEV_IEPC_HeavyLorry_DataProviderV27.QUALIFIED_XSD_TYPE);

            Bind<IXMLDeclarationVehicleData>().To<XMLDeclaration_FCHV_HeavyLorry_DataProviderV27>()
                .Named(XMLDeclaration_FCHV_HeavyLorry_DataProviderV27.QUALIFIED_XSD_TYPE);

            Bind<IXMLDeclarationVehicleData>().To<XMLDeclaration_FCHV_IEPC_HeavyLorry_DataProviderV27>()
                .Named(XMLDeclaration_FCHV_IEPC_HeavyLorry_DataProviderV27.QUALIFIED_XSD_TYPE);

            Bind<IXMLDeclarationVehicleData>().To<XMLDeclaration_Multiple_FCHV_HeavyLorry_DataProviderV27>()
                .Named(XMLDeclaration_Multiple_FCHV_HeavyLorry_DataProviderV27.QUALIFIED_XSD_TYPE);

            Bind<IXMLDeclarationVehicleData>().To<XMLDeclaration_Multiple_PEV_HeavyLorry_DataProviderV27>()
                .Named(XMLDeclaration_Multiple_PEV_HeavyLorry_DataProviderV27.QUALIFIED_XSD_TYPE);

            Bind<IXMLDeclarationVehicleData>().To<XMLDeclaration_Multiple_SHEV_HeavyLorry_DataProviderV27>()
                .Named(XMLDeclaration_Multiple_SHEV_HeavyLorry_DataProviderV27.QUALIFIED_XSD_TYPE);

            // Vehicle - Medium Lorry
            Bind<IXMLDeclarationVehicleData>().To<XMLDeclaration_Exempted_MediumLorry_DataProviderV27>()
                .Named(XMLDeclaration_Exempted_MediumLorry_DataProviderV27.QUALIFIED_XSD_TYPE);

            Bind<IXMLDeclarationVehicleData>().To<XMLDeclaration_Conventional_MediumLorry_DataProviderV27>()
                .Named(XMLDeclaration_Conventional_MediumLorry_DataProviderV27.QUALIFIED_XSD_TYPE);

            Bind<IXMLDeclarationVehicleData>().To<XMLDeclaration_PHEV_MediumLorry_DataProviderV27>()
                .Named(XMLDeclaration_PHEV_MediumLorry_DataProviderV27.QUALIFIED_XSD_TYPE);

            Bind<IXMLDeclarationVehicleData>().To<XMLDeclaration_PEV_MediumLorry_DataProviderV27>()
                .Named(XMLDeclaration_PEV_MediumLorry_DataProviderV27.QUALIFIED_XSD_TYPE);

            Bind<IXMLDeclarationVehicleData>().To<XMLDeclaration_PEV_IEPC_MediumLorry_DataProviderV27>()
                .Named(XMLDeclaration_PEV_IEPC_MediumLorry_DataProviderV27.QUALIFIED_XSD_TYPE);

            Bind<IXMLDeclarationVehicleData>().To<XMLDeclaration_SHEV_MediumLorry_DataProviderV27>()
                .Named(XMLDeclaration_SHEV_MediumLorry_DataProviderV27.QUALIFIED_XSD_TYPE);

            Bind<IXMLDeclarationVehicleData>().To<XMLDeclaration_SHEV_IEPC_MediumLorry_DataProviderV27>()
                .Named(XMLDeclaration_SHEV_IEPC_MediumLorry_DataProviderV27.QUALIFIED_XSD_TYPE);

            Bind<IXMLDeclarationVehicleData>().To<XMLDeclaration_FCHV_MediumLorry_DataProviderV27>()
                .Named(XMLDeclaration_FCHV_MediumLorry_DataProviderV27.QUALIFIED_XSD_TYPE);

            Bind<IXMLDeclarationVehicleData>().To<XMLDeclaration_FCHV_IEPC_MediumLorry_DataProviderV27>()
                .Named(XMLDeclaration_FCHV_IEPC_MediumLorry_DataProviderV27.QUALIFIED_XSD_TYPE);

            // Vehicle - Primary Bus
            Bind<IXMLDeclarationVehicleData>().To<XMLDeclaration_Exempted_PrimaryBus_DataProviderV27>()
                .Named(XMLDeclaration_Exempted_PrimaryBus_DataProviderV27.QUALIFIED_XSD_TYPE);

            Bind<IXMLDeclarationVehicleData>().To<XMLDeclaration_Conventional_PrimaryBus_DataProviderV27>()
                .Named(XMLDeclaration_Conventional_PrimaryBus_DataProviderV27.QUALIFIED_XSD_TYPE);

            Bind<IXMLDeclarationVehicleData>().To<XMLDeclaration_PHEV_PrimaryBus_DataProviderV27>()
                .Named(XMLDeclaration_PHEV_PrimaryBus_DataProviderV27.QUALIFIED_XSD_TYPE);

            Bind<IXMLDeclarationVehicleData>().To<XMLDeclaration_PEV_PrimaryBus_DataProviderV27>()
                .Named(XMLDeclaration_PEV_PrimaryBus_DataProviderV27.QUALIFIED_XSD_TYPE);

            Bind<IXMLDeclarationVehicleData>().To<XMLDeclaration_PEV_IEPC_PrimaryBus_DataProviderV27>()
                .Named(XMLDeclaration_PEV_IEPC_PrimaryBus_DataProviderV27.QUALIFIED_XSD_TYPE);

            Bind<IXMLDeclarationVehicleData>().To<XMLDeclaration_SHEV_PrimaryBus_DataProviderV27>()
                .Named(XMLDeclaration_SHEV_PrimaryBus_DataProviderV27.QUALIFIED_XSD_TYPE);

            Bind<IXMLDeclarationVehicleData>().To<XMLDeclaration_SHEV_IEPC_PrimaryBus_DataProviderV27>()
                .Named(XMLDeclaration_SHEV_IEPC_PrimaryBus_DataProviderV27.QUALIFIED_XSD_TYPE);

            Bind<IXMLDeclarationVehicleData>().To<XMLDeclaration_FCHV_PrimaryBus_DataProviderV27>()
                .Named(XMLDeclaration_FCHV_PrimaryBus_DataProviderV27.QUALIFIED_XSD_TYPE);

            Bind<IXMLDeclarationVehicleData>().To<XMLDeclaration_FCHV_IEPC_PrimaryBus_DataProviderV27>()
                .Named(XMLDeclaration_FCHV_IEPC_PrimaryBus_DataProviderV27.QUALIFIED_XSD_TYPE);

            Bind<IXMLDeclarationVehicleData>().To<XMLDeclaration_Multiple_FCHV_PrimaryBus_DataProviderV27>()
                .Named(XMLDeclaration_Multiple_FCHV_PrimaryBus_DataProviderV27.QUALIFIED_XSD_TYPE);

            Bind<IXMLDeclarationVehicleData>().To<XMLDeclaration_Multiple_PEV_PrimaryBus_DataProviderV27>()
                .Named(XMLDeclaration_Multiple_PEV_PrimaryBus_DataProviderV27.QUALIFIED_XSD_TYPE);

            Bind<IXMLDeclarationVehicleData>().To<XMLDeclaration_Multiple_SHEV_PrimaryBus_DataProviderV27>()
                .Named(XMLDeclaration_Multiple_SHEV_PrimaryBus_DataProviderV27.QUALIFIED_XSD_TYPE);

            // Vehicle - Completed Bus
            Bind<IXMLDeclarationVehicleData>().To<XMLDeclaration_Conventional_CompletedBus_DataProviderV27>()
                .Named(XMLDeclaration_Conventional_CompletedBus_DataProviderV27.QUALIFIED_XSD_TYPE);

            Bind<IXMLDeclarationVehicleData>().To<XMLDeclaration_Exempted_CompletedBus_DataProviderV27>()
               .Named(XMLDeclaration_Exempted_CompletedBus_DataProviderV27.QUALIFIED_XSD_TYPE);

            Bind<IXMLDeclarationVehicleData>().To<XMLDeclaration_HEV_CompletedBus_DataProviderV27>()
                .Named(XMLDeclaration_HEV_CompletedBus_DataProviderV27.QUALIFIED_XSD_TYPE);

            Bind<IXMLDeclarationVehicleData>().To<XMLDeclaration_PEV_CompletedBus_DataProviderV27>()
                .Named(XMLDeclaration_PEV_CompletedBus_DataProviderV27.QUALIFIED_XSD_TYPE);

            Bind<IXMLDeclarationVehicleData>().To<XMLDeclaration_FCHV_CompletedBus_DataProviderV27>()
                .Named(XMLDeclaration_FCHV_CompletedBus_DataProviderV27.QUALIFIED_XSD_TYPE);

            // ComponentDataProvider - Lorry
            Bind<IXMLVehicleComponentsDeclaration>().To<XMLDeclaration_Conventional_Lorry_ComponentDataProviderV27>()
                .Named(XMLDeclaration_Conventional_Lorry_ComponentDataProviderV27.QUALIFIED_XSD_TYPE);

            Bind<IXMLVehicleComponentsDeclaration>().To<XMLDeclaration_PHEV_Lorry_ComponentDataProviderV27>()
                .Named(XMLDeclaration_PHEV_Lorry_ComponentDataProviderV27.QUALIFIED_XSD_TYPE);

            Bind<IXMLVehicleComponentsDeclaration>().To<XMLDeclaration_PEV_Lorry_ComponentDataProviderV27>()
                .Named(XMLDeclaration_PEV_Lorry_ComponentDataProviderV27.QUALIFIED_E2_XSD_TYPE);

            Bind<IXMLVehicleComponentsDeclaration>().To<XMLDeclaration_PEV_Lorry_ComponentDataProviderV27>()
                .Named(XMLDeclaration_PEV_Lorry_ComponentDataProviderV27.QUALIFIED_E3_XSD_TYPE);

            Bind<IXMLVehicleComponentsDeclaration>().To<XMLDeclaration_PEV_Lorry_ComponentDataProviderV27>()
                .Named(XMLDeclaration_PEV_Lorry_ComponentDataProviderV27.QUALIFIED_E4_XSD_TYPE);

            Bind<IXMLVehicleComponentsDeclaration>().To<XMLDeclaration_PEV_IEPC_Lorry_ComponentDataProviderV27>()
                .Named(XMLDeclaration_PEV_IEPC_Lorry_ComponentDataProviderV27.QUALIFIED_XSD_TYPE);

            Bind<IXMLVehicleComponentsDeclaration>().To<XMLDeclaration_SHEV_Lorry_ComponentDataProviderV27>()
                .Named(XMLDeclaration_SHEV_Lorry_ComponentDataProviderV27.QUALIFIED_S2_XSD_TYPE);

            Bind<IXMLVehicleComponentsDeclaration>().To<XMLDeclaration_SHEV_Lorry_ComponentDataProviderV27>()
               .Named(XMLDeclaration_SHEV_Lorry_ComponentDataProviderV27.QUALIFIED_S3_XSD_TYPE);

            Bind<IXMLVehicleComponentsDeclaration>().To<XMLDeclaration_SHEV_Lorry_ComponentDataProviderV27>()
                .Named(XMLDeclaration_SHEV_Lorry_ComponentDataProviderV27.QUALIFIED_S4_XSD_TYPE);

            Bind<IXMLVehicleComponentsDeclaration>().To<XMLDeclaration_SHEV_IEPC_Lorry_ComponentDataProviderV27>()
                .Named(XMLDeclaration_SHEV_IEPC_Lorry_ComponentDataProviderV27.QUALIFIED_XSD_TYPE);

            Bind<IXMLVehicleComponentsDeclaration>().To<XMLDeclaration_FCHV_Lorry_ComponentDataProviderV27>()
                .Named(XMLDeclaration_FCHV_Lorry_ComponentDataProviderV27.QUALIFIED_F2_XSD_TYPE);

            Bind<IXMLVehicleComponentsDeclaration>().To<XMLDeclaration_FCHV_Lorry_ComponentDataProviderV27>()
                .Named(XMLDeclaration_FCHV_Lorry_ComponentDataProviderV27.QUALIFIED_F3_XSD_TYPE);

            Bind<IXMLVehicleComponentsDeclaration>().To<XMLDeclaration_FCHV_Lorry_ComponentDataProviderV27>()
                .Named(XMLDeclaration_FCHV_Lorry_ComponentDataProviderV27.QUALIFIED_F4_XSD_TYPE);

            Bind<IXMLVehicleComponentsDeclaration>().To<XMLDeclaration_FCHV_IEPC_Lorry_ComponentDataProviderV27>()
                .Named(XMLDeclaration_FCHV_IEPC_Lorry_ComponentDataProviderV27.QUALIFIED_XSD_TYPE);

            Bind<IXMLVehicleComponentsDeclaration>().To<XMLDeclaration_Multiple_FCHV_Lorry_ComponentDataProviderV27>()
                .Named(XMLDeclaration_Multiple_FCHV_Lorry_ComponentDataProviderV27.QUALIFIED_XSD_TYPE);

            Bind<IXMLVehicleComponentsDeclaration>().To<XMLDeclaration_Multiple_PEV_Lorry_ComponentDataProviderV27>()
                .Named(XMLDeclaration_Multiple_PEV_Lorry_ComponentDataProviderV27.QUALIFIED_XSD_TYPE);

            Bind<IXMLVehicleComponentsDeclaration>().To<XMLDeclaration_Multiple_SHEV_Lorry_ComponentDataProviderV27>()
                .Named(XMLDeclaration_Multiple_SHEV_Lorry_ComponentDataProviderV27.QUALIFIED_XSD_TYPE);

            // ComponentDataProvider - Primary Bus
            Bind<IXMLVehicleComponentsDeclaration>().To<XMLDeclaration_Conventional_PrimaryBus_ComponentDataProviderV27>()
                .Named(XMLDeclaration_Conventional_PrimaryBus_ComponentDataProviderV27.QUALIFIED_XSD_TYPE);

            Bind<IXMLVehicleComponentsDeclaration>().To<XMLDeclaration_PHEV_PrimaryBus_ComponentDataProviderV27>()
                .Named(XMLDeclaration_PHEV_PrimaryBus_ComponentDataProviderV27.QUALIFIED_XSD_TYPE);

            Bind<IXMLVehicleComponentsDeclaration>().To<XMLDeclaration_PEV_PrimaryBus_ComponentDataProviderV27>()
                .Named(XMLDeclaration_PEV_PrimaryBus_ComponentDataProviderV27.QUALIFIED_E2_XSD_TYPE);

            Bind<IXMLVehicleComponentsDeclaration>().To<XMLDeclaration_PEV_PrimaryBus_ComponentDataProviderV27>()
                .Named(XMLDeclaration_PEV_PrimaryBus_ComponentDataProviderV27.QUALIFIED_E3_XSD_TYPE);

            Bind<IXMLVehicleComponentsDeclaration>().To<XMLDeclaration_PEV_PrimaryBus_ComponentDataProviderV27>()
                .Named(XMLDeclaration_PEV_PrimaryBus_ComponentDataProviderV27.QUALIFIED_E4_XSD_TYPE);

            Bind<IXMLVehicleComponentsDeclaration>().To<XMLDeclaration_PEV_IEPC_PrimaryBus_ComponentDataProviderV27>()
                .Named(XMLDeclaration_PEV_IEPC_PrimaryBus_ComponentDataProviderV27.QUALIFIED_XSD_TYPE);

            Bind<IXMLVehicleComponentsDeclaration>().To<XMLDeclaration_FCHV_PrimaryBus_ComponentDataProviderV27>()
                .Named(XMLDeclaration_FCHV_PrimaryBus_ComponentDataProviderV27.QUALIFIED_HEV_F2_XSD_TYPE);

            Bind<IXMLVehicleComponentsDeclaration>().To<XMLDeclaration_FCHV_PrimaryBus_ComponentDataProviderV27>()
                .Named(XMLDeclaration_FCHV_PrimaryBus_ComponentDataProviderV27.QUALIFIED_HEV_F3_XSD_TYPE);

            Bind<IXMLVehicleComponentsDeclaration>().To<XMLDeclaration_FCHV_PrimaryBus_ComponentDataProviderV27>()
                .Named(XMLDeclaration_FCHV_PrimaryBus_ComponentDataProviderV27.QUALIFIED_HEV_F4_XSD_TYPE);

            Bind<IXMLVehicleComponentsDeclaration>().To<XMLDeclaration_FCHV_IEPC_PrimaryBus_ComponentDataProviderV27>()
                .Named(XMLDeclaration_FCHV_IEPC_PrimaryBus_ComponentDataProviderV27.QUALIFIED_XSD_TYPE);

            Bind<IXMLVehicleComponentsDeclaration>().To<XMLDeclaration_SHEV_PrimaryBus_ComponentDataProviderV27>()
                .Named(XMLDeclaration_SHEV_PrimaryBus_ComponentDataProviderV27.QUALIFIED_XSD_TYPE_S2);

            Bind<IXMLVehicleComponentsDeclaration>().To<XMLDeclaration_SHEV_PrimaryBus_ComponentDataProviderV27>()
                .Named(XMLDeclaration_SHEV_PrimaryBus_ComponentDataProviderV27.QUALIFIED_XSD_TYPE_S3);
            
            Bind<IXMLVehicleComponentsDeclaration>().To<XMLDeclaration_SHEV_PrimaryBus_ComponentDataProviderV27>()
                .Named(XMLDeclaration_SHEV_PrimaryBus_ComponentDataProviderV27.QUALIFIED_XSD_TYPE_S4);

            Bind<IXMLVehicleComponentsDeclaration>().To<XMLDeclaration_SHEV_IEPC_PrimaryBus_ComponentDataProviderV27>()
                .Named(XMLDeclaration_SHEV_IEPC_PrimaryBus_ComponentDataProviderV27.QUALIFIED_XSD_TYPE);

            Bind<IXMLVehicleComponentsDeclaration>().To<XMLDeclaration_Multiple_FCHV_PrimaryBus_ComponentDataProviderV27>()
                .Named(XMLDeclaration_Multiple_FCHV_PrimaryBus_ComponentDataProviderV27.QUALIFIED_XSD_TYPE);

            Bind<IXMLVehicleComponentsDeclaration>().To<XMLDeclaration_Multiple_PEV_PrimaryBus_ComponentDataProviderV27>()
                .Named(XMLDeclaration_Multiple_PEV_PrimaryBus_ComponentDataProviderV27.QUALIFIED_XSD_TYPE);

            Bind<IXMLVehicleComponentsDeclaration>().To<XMLDeclaration_Multiple_SHEV_PrimaryBus_ComponentDataProviderV27>()
                .Named(XMLDeclaration_Multiple_SHEV_PrimaryBus_ComponentDataProviderV27.QUALIFIED_XSD_TYPE);

            // ComponentDataProvider - Completed Bus
            Bind<IXMLVehicleComponentsDeclaration>().To<XMLDeclaration_CompletedBus_ComponentDataProviderV27>()
                .Named(XMLDeclaration_CompletedBus_ComponentDataProviderV27.QUALIFIED_XSD_TYPE_CONVENTIONAL);

            Bind<IXMLVehicleComponentsDeclaration>().To<XMLDeclaration_CompletedBus_ComponentDataProviderV27>()
                .Named(XMLDeclaration_CompletedBus_ComponentDataProviderV27.QUALIFIED_XSD_TYPE_xEV);

            // ComponentReader - Lorry
            Bind<IXMLComponentReader>().To<XML_Conventional_Lorry_ComponentReaderV27>().Named(XML_Conventional_Lorry_ComponentReaderV27.QUALIFIED_XSD_TYPE);
            Bind<IXMLComponentReader>().To<XML_PHEV_Lorry_ComponentReaderV27>().Named(XML_PHEV_Lorry_ComponentReaderV27.QUALIFIED_XSD_TYPE);
            Bind<IXMLComponentReader>().To<XML_PEV_E2_Lorry_ComponentReaderV27>().Named(XML_PEV_E2_Lorry_ComponentReaderV27.QUALIFIED_XSD_TYPE);
            Bind<IXMLComponentReader>().To<XML_PEV_E3_Lorry_ComponentReaderV27>().Named(XML_PEV_E3_Lorry_ComponentReaderV27.QUALIFIED_XSD_TYPE);
            Bind<IXMLComponentReader>().To<XML_PEV_E4_Lorry_ComponentReaderV27>().Named(XML_PEV_E4_Lorry_ComponentReaderV27.QUALIFIED_XSD_TYPE);
            Bind<IXMLComponentReader>().To<XML_PEV_IEPC_Lorry_ComponentReaderV27>().Named(XML_PEV_IEPC_Lorry_ComponentReaderV27.QUALIFIED_XSD_TYPE);
            Bind<IXMLComponentReader>().To<XML_SHEV_S2_Lorry_ComponentReaderV27>().Named(XML_SHEV_S2_Lorry_ComponentReaderV27.QUALIFIED_XSD_TYPE);
            Bind<IXMLComponentReader>().To<XML_SHEV_S3_Lorry_ComponentReaderV27>().Named(XML_SHEV_S3_Lorry_ComponentReaderV27.QUALIFIED_XSD_TYPE);
            Bind<IXMLComponentReader>().To<XML_SHEV_S4_Lorry_ComponentReaderV27>().Named(XML_SHEV_S4_Lorry_ComponentReaderV27.QUALIFIED_XSD_TYPE);
            Bind<IXMLComponentReader>().To<XML_SHEV_IEPC_Lorry_ComponentReaderV27>().Named(XML_SHEV_IEPC_Lorry_ComponentReaderV27.QUALIFIED_XSD_TYPE);
            Bind<IXMLComponentReader>().To<XML_FCHV_F2_Lorry_ComponentReaderV27>().Named(XML_FCHV_F2_Lorry_ComponentReaderV27.QUALIFIED_XSD_TYPE);
            Bind<IXMLComponentReader>().To<XML_FCHV_F3_Lorry_ComponentReaderV27>().Named(XML_FCHV_F3_Lorry_ComponentReaderV27.QUALIFIED_XSD_TYPE);
            Bind<IXMLComponentReader>().To<XML_FCHV_F4_Lorry_ComponentReaderV27>().Named(XML_FCHV_F4_Lorry_ComponentReaderV27.QUALIFIED_XSD_TYPE);
            Bind<IXMLComponentReader>().To<XML_FCHV_IEPC_Lorry_ComponentReaderV27>().Named(XML_FCHV_IEPC_Lorry_ComponentReaderV27.QUALIFIED_XSD_TYPE);
            Bind<IXMLComponentReader>().To<XML_Multiple_FCHV_Lorry_ComponentReaderV27>().Named(XML_Multiple_FCHV_Lorry_ComponentReaderV27.QUALIFIED_XSD_TYPE);
            Bind<IXMLComponentReader>().To<XML_Multiple_PEV_Lorry_ComponentReaderV27>().Named(XML_Multiple_PEV_Lorry_ComponentReaderV27.QUALIFIED_XSD_TYPE);
            Bind<IXMLComponentReader>().To<XML_Multiple_SHEV_Lorry_ComponentReaderV27>().Named(XML_Multiple_SHEV_Lorry_ComponentReaderV27.QUALIFIED_XSD_TYPE);

            // ComponentReader - Primary Bus
            Bind<IXMLComponentReader>().To<XML_Conventional_PrimaryBus_ComponentReaderV27>().Named(XML_Conventional_PrimaryBus_ComponentReaderV27.QUALIFIED_XSD_TYPE);
            Bind<IXMLComponentReader>().To<XML_PHEV_PrimaryBus_ComponentReaderV27>().Named(XML_PHEV_PrimaryBus_ComponentReaderV27.QUALIFIED_XSD_TYPE);
            Bind<IXMLComponentReader>().To<XML_PEV_E2_PrimaryBus_ComponentReaderV27>().Named(XML_PEV_E2_PrimaryBus_ComponentReaderV27.QUALIFIED_XSD_TYPE);
            Bind<IXMLComponentReader>().To<XML_PEV_E3_PrimaryBus_ComponentReaderV27>().Named(XML_PEV_E3_PrimaryBus_ComponentReaderV27.QUALIFIED_XSD_TYPE);
            Bind<IXMLComponentReader>().To<XML_PEV_E4_PrimaryBus_ComponentReaderV27>().Named(XML_PEV_E4_PrimaryBus_ComponentReaderV27.QUALIFIED_XSD_TYPE);
            Bind<IXMLComponentReader>().To<XML_PEV_IEPC_PrimaryBus_ComponentReaderV27>().Named(XML_PEV_IEPC_PrimaryBus_ComponentReaderV27.QUALIFIED_XSD_TYPE);
            Bind<IXMLComponentReader>().To<XML_FCHV_F2_PrimaryBus_ComponentReaderV27>().Named(XML_FCHV_F2_PrimaryBus_ComponentReaderV27.QUALIFIED_XSD_TYPE);
            Bind<IXMLComponentReader>().To<XML_FCHV_F3_PrimaryBus_ComponentReaderV27>().Named(XML_FCHV_F3_PrimaryBus_ComponentReaderV27.QUALIFIED_XSD_TYPE);
            Bind<IXMLComponentReader>().To<XML_FCHV_F4_PrimaryBus_ComponentReaderV27>().Named(XML_FCHV_F4_PrimaryBus_ComponentReaderV27.QUALIFIED_XSD_TYPE);
            Bind<IXMLComponentReader>().To<XML_FCHV_IEPC_PrimaryBus_ComponentReaderV27>().Named(XML_FCHV_IEPC_PrimaryBus_ComponentReaderV27.QUALIFIED_XSD_TYPE);
            Bind<IXMLComponentReader>().To<XML_SHEV_S2_PrimaryBus_ComponentReaderV27>().Named(XML_SHEV_S2_PrimaryBus_ComponentReaderV27.QUALIFIED_XSD_TYPE);
            Bind<IXMLComponentReader>().To<XML_SHEV_S3_PrimaryBus_ComponentReaderV27>().Named(XML_SHEV_S3_PrimaryBus_ComponentReaderV27.QUALIFIED_XSD_TYPE);
            Bind<IXMLComponentReader>().To<XML_SHEV_S4_PrimaryBus_ComponentReaderV27>().Named(XML_SHEV_S4_PrimaryBus_ComponentReaderV27.QUALIFIED_XSD_TYPE);
            Bind<IXMLComponentReader>().To<XML_SHEV_IEPC_PrimaryBus_ComponentReaderV27>().Named(XML_SHEV_IEPC_PrimaryBus_ComponentReaderV27.QUALIFIED_XSD_TYPE);
            Bind<IXMLComponentReader>().To<XML_Multiple_FCHV_PrimaryBus_ComponentReaderV27>().Named(XML_Multiple_FCHV_PrimaryBus_ComponentReaderV27.QUALIFIED_XSD_TYPE);
            Bind<IXMLComponentReader>().To<XML_Multiple_PEV_PrimaryBus_ComponentReaderV27>().Named(XML_Multiple_PEV_PrimaryBus_ComponentReaderV27.QUALIFIED_XSD_TYPE);
            Bind<IXMLComponentReader>().To<XML_Multiple_SHEV_PrimaryBus_ComponentReaderV27>().Named(XML_Multiple_SHEV_PrimaryBus_ComponentReaderV27.QUALIFIED_XSD_TYPE);

            // ComponentReader - Completed Bus
            Bind<IXMLComponentReader>().To<XML_CompletedBus_ComponentReaderV27>().Named(XML_CompletedBus_ComponentReaderV27.QUALIFIED_XSD_TYPE_CONVENTIONAL);
            Bind<IXMLComponentReader>().To<XML_CompletedBus_ComponentReaderV27>().Named(XML_CompletedBus_ComponentReaderV27.QUALIFIED_XSD_TYPE_xEV);

            // Axles
            Bind<IXMLAxleDeclarationInputData>().To<XMLDeclarationAxleDataProviderV27>().Named(XMLDeclarationAxleDataProviderV27.QUALIFIED_XSD_TYPE);
            Bind<IXMLAxleReader>().To<XMLComponentReaderV27>().Named(XMLComponentReaderV27.AXLE_READER_QUALIFIED_XSD_TYPE);
            Bind<IXMLAxlesDeclarationInputData>().To<XMLDeclarationAxlesDataProviderV27>().Named(XMLDeclarationAxlesDataProviderV27.QUALIFIED_XSD_TYPE);
            Bind<IXMLAxlesReader>().To<XMLComponentReaderV27>().Named(XMLComponentReaderV27.AXLES_READER_QUALIFIED_XSD_TYPE);

            // Battery
            Bind<IXMLElectricStorageSystemDeclarationInputData>().To<XMLElectricStorageSystemDeclarationInputDataV27>()
                .Named(XMLElectricStorageSystemDeclarationInputDataV27.QUALIFIED_XSD_TYPE);

            Bind<IXMLREESSReader>().To<XMLREESSReaderV27>().Named(XMLREESSReaderV27.QUALIFIED_XSD_TYPE);

            // ADAS
            Bind<IXMLADASReader>().To<XMLADASReaderV27>().Named(XMLADASReaderV27.QUALIFIED_XSD_TYPE_CONVENTIONAL);
            Bind<IXMLADASReader>().To<XMLADASReaderV27>().Named(XMLADASReaderV27.QUALIFIED_XSD_TYPE_HEV);
            Bind<IXMLADASReader>().To<XMLADASReaderV27>().Named(XMLADASReaderV27.QUALIFIED_XSD_TYPE_PEV);

            Bind<IXMLAdvancedDriverAssistantSystemDeclarationInputData>()
                .To<XMLDeclarationADASDataConventionalProviderV27>().Named(XMLDeclarationADASDataConventionalProviderV27.QUALIFIED_XSD_TYPE);

            Bind<IXMLAdvancedDriverAssistantSystemDeclarationInputData>()
                .To<XMLDeclarationADASDataHEVProviderV27>().Named(XMLDeclarationADASDataHEVProviderV27.QUALIFIED_XSD_TYPE);

            Bind<IXMLAdvancedDriverAssistantSystemDeclarationInputData>()
                .To<XMLDeclarationADASDataPEVProviderV27>().Named(XMLDeclarationADASDataPEVProviderV27.QUALIFIED_XSD_TYPE);

            // Electric Machines
            Bind<IXMLElectricMachinesDeclarationInputData>().To<XMLElectricMachinesDeclarationInputDataProviderV27>()
                .Named(XMLElectricMachinesDeclarationInputDataProviderV27.QUALIFIED_XSD_TYPE);

            Bind<IXMLElectricMachinesDeclarationInputData>().To<XMLElectricMachinesDeclarationInputDataProviderV27>()
                .Named(XMLElectricMachinesDeclarationInputDataProviderV27.QUALIFIED_GEN_XSD_TYPE);

            Bind<IXMLElectricMachineSystemReader>().To<XMLElectricMachineSystemReaderV27>().Named(XMLElectricMachineSystemReaderV27.QUALIFIED_XSD_TYPE);
            Bind<IXMLElectricMachineSystemReader>().To<XMLElectricMachineSystemReaderV27>().Named(XMLElectricMachineSystemReaderV27.QUALIFIED_GEN_XSD_TYPE);

            // PTO
            Bind<IXMLPTOReader>().To<XMLPTOReaderV27>().Named(XMLPTOReaderV27.QUALIFIED_XSD_TYPE);
            Bind<IXMLPTOReader>().To<XMLMultiplePTOReaderV27>().Named(XMLMultiplePTOReaderV27.QUALIFIED_XSD_TYPE);
            Bind<IXMLPTOTransmissionInputData>().To<XMLDeclarationPTODataProviderV27>().Named(XMLDeclarationPTODataProviderV27.QUALIFIED_XSD_TYPE);
            Bind<IXMLPTOTransmissionInputData>().To<XMLDeclarationMultiplePTODataProviderV27>().Named(XMLDeclarationMultiplePTODataProviderV27.QUALIFIED_XSD_TYPE);

            // Auxiliaries
            Bind<IXMLAuxiliariesDeclarationInputData>().To<XMLDeclarationAuxiliaries_Lorry_DataProviderV27>().Named(
                XMLDeclarationAuxiliaries_Lorry_DataProviderV27.QUALIFIED_XSD_TYPE_CONVENTIONAL);

            Bind<IXMLAuxiliariesDeclarationInputData>().To<XMLDeclarationAuxiliaries_Lorry_DataProviderV27>().Named(
                XMLDeclarationAuxiliaries_Lorry_DataProviderV27.QUALIFIED_XSD_TYPE_PHEV);

            Bind<IXMLAuxiliariesDeclarationInputData>().To<XMLDeclarationAuxiliaries_Lorry_DataProviderV27>().Named(
                XMLDeclarationAuxiliaries_Lorry_DataProviderV27.QUALIFIED_XSD_TYPE_SHEV);

            Bind<IXMLAuxiliariesDeclarationInputData>().To<XMLDeclarationAuxiliaries_Lorry_DataProviderV27>().Named(
                XMLDeclarationAuxiliaries_Lorry_DataProviderV27.QUALIFIED_XSD_TYPE_PEV);

            Bind<IXMLAuxiliariesDeclarationInputData>().To<XMLDeclarationAuxiliaries_Lorry_DataProviderV27>().Named(
                XMLDeclarationAuxiliaries_Lorry_DataProviderV27.QUALIFIED_XSD_TYPE_FCHV);

            Bind<IXMLBusAuxiliariesDeclarationData>().To<XMLDeclarationAuxiliaries_Conventional_PrimaryBus_DataProviderV27>()
                .Named(XMLDeclarationAuxiliaries_Conventional_PrimaryBus_DataProviderV27.QUALIFIED_XSD_TYPE);

            Bind<IXMLBusAuxiliariesDeclarationData>().To<XMLDeclarationAuxiliaries_PHEV_PrimaryBus_DataProviderV27>()
                .Named(XMLDeclarationAuxiliaries_PHEV_PrimaryBus_DataProviderV27.QUALIFIED_XSD_TYPE);

            Bind<IXMLBusAuxiliariesDeclarationData>().To<XMLDeclarationAuxiliaries_PEV_PrimaryBus_DataProviderV27>()
                .Named(XMLDeclarationAuxiliaries_PEV_PrimaryBus_DataProviderV27.QUALIFIED_XSD_TYPE);

            Bind<IXMLBusAuxiliariesDeclarationData>().To<XMLDeclarationAuxiliaries_SHEV_PrimaryBus_DataProviderV27>()
                .Named(XMLDeclarationAuxiliaries_SHEV_PrimaryBus_DataProviderV27.QUALIFIED_XSD_TYPE);

            Bind<IXMLBusAuxiliariesDeclarationData>().To<XMLDeclarationAuxiliaries_FCHV_PrimaryBus_DataProviderV27>()
                .Named(XMLDeclarationAuxiliaries_FCHV_PrimaryBus_DataProviderV27.QUALIFIED_XSD_TYPE);

            Bind<IXMLBusAuxiliariesDeclarationData>().To<XMLDeclarationAuxiliaries_CompletedBus_DataProviderV27>()
                .Named(XMLDeclarationAuxiliaries_CompletedBus_DataProviderV27.QUALIFIED_XSD_TYPE_CONVENTIONAL);

            Bind<IXMLBusAuxiliariesDeclarationData>().To<XMLDeclarationAuxiliaries_CompletedBus_DataProviderV27>()
                .Named(XMLDeclarationAuxiliaries_CompletedBus_DataProviderV27.QUALIFIED_XSD_TYPE_xEV);

            Bind<IXMLAuxiliaryDeclarationInputData>().To<XMLDeclarationAuxiliaryLorryDataProviderV27>().Named(XMLDeclarationAuxiliaryLorryDataProviderV27.QUALIFIED_XSD_TYPE_CONVENTIONAL);
            Bind<IXMLAuxiliaryDeclarationInputData>().To<XMLDeclarationAuxiliaryLorryDataProviderV27>().Named(XMLDeclarationAuxiliaryLorryDataProviderV27.QUALIFIED_XSD_TYPE_PHEV);
            Bind<IXMLAuxiliaryDeclarationInputData>().To<XMLDeclarationAuxiliaryLorryDataProviderV27>().Named(XMLDeclarationAuxiliaryLorryDataProviderV27.QUALIFIED_XSD_TYPE_SHEV);
            Bind<IXMLAuxiliaryDeclarationInputData>().To<XMLDeclarationAuxiliaryLorryDataProviderV27>().Named(XMLDeclarationAuxiliaryLorryDataProviderV27.QUALIFIED_XSD_TYPE_PEV);
            Bind<IXMLAuxiliaryDeclarationInputData>().To<XMLDeclarationAuxiliaryLorryDataProviderV27>().Named(XMLDeclarationAuxiliaryLorryDataProviderV27.QUALIFIED_XSD_TYPE_FCHV);

            Bind<IXMLAuxiliaryReader>().To<XMLLorryComponentReaderV27>().Named(XMLLorryComponentReaderV27.AUXILIARIES_CONVENTIONAL_QUALIFIED_XSD_TYPE);
            Bind<IXMLAuxiliaryReader>().To<XMLLorryComponentReaderV27>().Named(XMLLorryComponentReaderV27.AUXILIARIES_PHEV_QUALIFIED_XSD_TYPE);
            Bind<IXMLAuxiliaryReader>().To<XMLLorryComponentReaderV27>().Named(XMLLorryComponentReaderV27.AUXILIARIES_SHEV_QUALIFIED_XSD_TYPE);
            Bind<IXMLAuxiliaryReader>().To<XMLLorryComponentReaderV27>().Named(XMLLorryComponentReaderV27.AUXILIARIES_PEV_QUALIFIED_XSD_TYPE);
            Bind<IXMLAuxiliaryReader>().To<XMLLorryComponentReaderV27>().Named(XMLLorryComponentReaderV27.AUXILIARIES_FCHV_QUALIFIED_XSD_TYPE);

            // Air Drag
            Bind<IXMLAirdragDeclarationInputData>().To<XMLDeclarationAirdragDataProviderV27>().Named(
                XMLDeclarationAirdragDataProviderV27.QUALIFIED_XSD_TYPE);

            // Fuel Cell
            Bind<IXMLFuelCellDeclarationInputData>().To<XMLFuelCellDeclarationInputDataProviderV27>().Named(XMLFuelCellDeclarationInputDataProviderV27.QUALIFIED_XSD_TYPE);
			
            Bind<IXMLFuelCellSystemDeclarationInputData>().To<XMLFuelCellSystemDeclarationInputDataProviderV27>()
                .Named(XMLFuelCellSystemDeclarationInputDataProviderV27.QUALIFIED_XSD_TYPE);

            // Axle Powertrains
            Bind<IXMLAxlePowertrainDeclarationInputData>().To<XMLDeclaration_AxlePowertrain_EM2_DataProviderV27>()
                .Named(XMLDeclaration_AxlePowertrain_EM2_DataProviderV27.QUALIFIED_XSD_TYPE);

            Bind<IXMLAxlePowertrainDeclarationInputData>().To<XMLDeclaration_AxlePowertrain_EM3_DataProviderV27>()
                .Named(XMLDeclaration_AxlePowertrain_EM3_DataProviderV27.QUALIFIED_XSD_TYPE);
            
            Bind<IXMLAxlePowertrainDeclarationInputData>().To<XMLDeclaration_AxlePowertrain_EM4_DataProviderV27>()
                .Named(XMLDeclaration_AxlePowertrain_EM4_DataProviderV27.QUALIFIED_XSD_TYPE);
            
            Bind<IXMLAxlePowertrainDeclarationInputData>().To<XMLDeclaration_AxlePowertrain_IEPC_DataProviderV27>()
                .Named(XMLDeclaration_AxlePowertrain_IEPC_DataProviderV27.QUALIFIED_XSD_TYPE);

            // Monitoring Data
            Bind<IXMLMonitoringReader>().To<XMLMonitoringReaderV27>().Named(XMLMonitoringReaderV27.QUALIFIED_XSD_TYPE_EXEMPTED);
            Bind<IXMLMonitoringReader>().To<XMLMonitoringReaderV27>().Named(XMLMonitoringReaderV27.QUALIFIED_XSD_TYPE_CONVENTIONAL);
            Bind<IXMLMonitoringReader>().To<XMLMonitoringReaderV27>().Named(XMLMonitoringReaderV27.QUALIFIED_XSD_TYPE_COMPLETED);
            Bind<IXMLMonitoringReader>().To<XMLMonitoringReaderV27>().Named(XMLMonitoringReaderV27.QUALIFIED_XSD_TYPE_PHEV);
            Bind<IXMLMonitoringReader>().To<XMLMonitoringReaderV27>().Named(XMLMonitoringReaderV27.QUALIFIED_XSD_TYPE_SHEV_S2);
            Bind<IXMLMonitoringReader>().To<XMLMonitoringReaderV27>().Named(XMLMonitoringReaderV27.QUALIFIED_XSD_TYPE_SHEV_S3);
            Bind<IXMLMonitoringReader>().To<XMLMonitoringReaderV27>().Named(XMLMonitoringReaderV27.QUALIFIED_XSD_TYPE_SHEV_S4);
            Bind<IXMLMonitoringReader>().To<XMLMonitoringReaderV27>().Named(XMLMonitoringReaderV27.QUALIFIED_XSD_TYPE_SHEV_IEPC);
            Bind<IXMLMonitoringReader>().To<XMLMonitoringReaderV27>().Named(XMLMonitoringReaderV27.QUALIFIED_XSD_TYPE_PEV_E2);
            Bind<IXMLMonitoringReader>().To<XMLMonitoringReaderV27>().Named(XMLMonitoringReaderV27.QUALIFIED_XSD_TYPE_PEV_E3);
            Bind<IXMLMonitoringReader>().To<XMLMonitoringReaderV27>().Named(XMLMonitoringReaderV27.QUALIFIED_XSD_TYPE_PEV_E4);
            Bind<IXMLMonitoringReader>().To<XMLMonitoringReaderV27>().Named(XMLMonitoringReaderV27.QUALIFIED_XSD_TYPE_PEV_IEPC);
            Bind<IXMLMonitoringReader>().To<XMLMonitoringReaderV27>().Named(XMLMonitoringReaderV27.QUALIFIED_XSD_TYPE_FCHV_F2);
            Bind<IXMLMonitoringReader>().To<XMLMonitoringReaderV27>().Named(XMLMonitoringReaderV27.QUALIFIED_XSD_TYPE_FCHV_F3);
            Bind<IXMLMonitoringReader>().To<XMLMonitoringReaderV27>().Named(XMLMonitoringReaderV27.QUALIFIED_XSD_TYPE_FCHV_F4);
            Bind<IXMLMonitoringReader>().To<XMLMonitoringReaderV27>().Named(XMLMonitoringReaderV27.QUALIFIED_XSD_TYPE_FCHV_IEPC);
            Bind<IXMLMonitoringReader>().To<XMLMonitoringReaderV27>().Named(XMLMonitoringReaderV27.QUALIFIED_XSD_TYPE_MULTIPLE_FCHV);
            Bind<IXMLMonitoringReader>().To<XMLMonitoringReaderV27>().Named(XMLMonitoringReaderV27.QUALIFIED_XSD_TYPE_MULTIPLE_PEV);
            Bind<IXMLMonitoringReader>().To<XMLMonitoringReaderV27>().Named(XMLMonitoringReaderV27.QUALIFIED_XSD_TYPE_MULTIPLE_SHEV);
        }
    }
}
