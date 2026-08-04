using Ninject.Modules;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.DataProvider;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Interfaces;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Reader;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Reader.Impl;

namespace TUGraz.VectoCore.Ninject.XMLInputData
{
    public class XMLDeclarationInputDataMultistageV11InjectModule : NinjectModule
    {
        public override void Load()
        {
            Bind<IXMLMultistageInputDataProvider>().To<XMLDeclarationInputDataProviderMultistageV11>()
                .Named(XMLDeclarationInputDataProviderMultistageV11.QUALIFIED_XSD_TYPE);

            Bind<IXMLDeclarationMultistageJobInputData>().To<XMLDeclarationMultistageJobInputDataV11>()
                .Named(XMLDeclarationMultistageJobInputDataV11.QUALIFIED_XSD_TYPE);

            // -----------------------------------

            Bind<IXMLDeclarationMultistageVehicleInputDataReader>().To<XMLDeclarationMultistageInputReaderV11>()
                .Named(XMLDeclarationMultistageInputReaderV11.QUALIFIED_XSD_TYPE);

            Bind<IXMLMultistageJobReader>().To<XMLMultistageJobReaderV11>()
                .Named(XMLMultistageJobReaderV11.QUALIFIED_XSD_TYPE);

            // -----------------------------------

            Bind<IXMLPrimaryVehicleBusInputData>().To<XMLDeclarationMultistagePrimaryVehicleInputDataV11>()
                .Named(XMLDeclarationMultistagePrimaryVehicleInputDataV11.QUALIFIED_XSD_TYPE);

            Bind<IXMLBusAuxiliariesDeclarationData>().To<XMLPrimaryBusAuxiliaries_Conventional_DataProviderV11>()
                .Named(XMLPrimaryBusAuxiliaries_Conventional_DataProviderV11.QUALIFIED_XSD_TYPE);

            Bind<IXMLBusAuxiliariesDeclarationData>().To<XMLPrimaryBusAuxiliaries_HEV_P_DataProviderV11>()
                .Named(XMLPrimaryBusAuxiliaries_HEV_P_DataProviderV11.QUALIFIED_XSD_TYPE);

            Bind<IXMLBusAuxiliariesDeclarationData>().To<XMLPrimaryBusAuxiliaries_HEV_S_DataProviderV11>()
                .Named(XMLPrimaryBusAuxiliaries_HEV_S_DataProviderV11.QUALIFIED_XSD_TYPE);

            Bind<IXMLBusAuxiliariesDeclarationData>().To<XMLPrimaryBusAuxiliaries_PEV_DataProviderV11>()
                .Named(XMLPrimaryBusAuxiliaries_PEV_DataProviderV11.QUALIFIED_XSD_TYPE);

            Bind<IXMLBusAuxiliariesDeclarationData>().To<XMLPrimaryBusAuxiliaries_IEPC_DataProviderV11>()
                .Named(XMLPrimaryBusAuxiliaries_IEPC_DataProviderV11.QUALIFIED_XSD_TYPE);

            Bind<IXMLBusAuxiliariesDeclarationData>().To<XMLPrimaryBusAuxiliaries_FCHV_DataProviderV11>()
                .Named(XMLPrimaryBusAuxiliaries_FCHV_DataProviderV11.QUALIFIED_XSD_TYPE);

            Bind<IXMLDeclarationPrimaryVehicleBusInputDataReader>().To<XMLMultistagePrimaryVehicleReaderV11>()
                .Named(XMLMultistagePrimaryVehicleReaderV11.QUALIFIED_XSD_TYPE);

            Bind<IXMLPrimaryVehicleBusJobInputData>().To<XMLDeclarationMultistage_Conventional_PrimaryVehicleBusJobInputDataProviderV11>()
                .Named(XMLDeclarationMultistage_Conventional_PrimaryVehicleBusJobInputDataProviderV11.QUALIFIED_XSD_TYPE);

            Bind<IXMLPrimaryVehicleBusJobInputData>().To<XMLDeclarationMultistage_HEV_Px_PrimaryVehicleBusJobInputDataProviderV11>()
                .Named(XMLDeclarationMultistage_HEV_Px_PrimaryVehicleBusJobInputDataProviderV11.QUALIFIED_XSD_TYPE);

            Bind<IXMLPrimaryVehicleBusJobInputData>().To<XMLDeclarationMultistage_HEV_Sx_PrimaryVehicleBusJobInputDataProviderV11>()
                .Named(XMLDeclarationMultistage_HEV_Sx_PrimaryVehicleBusJobInputDataProviderV11.QUALIFIED_XSD_TYPE);

            Bind<IXMLPrimaryVehicleBusJobInputData>().To<XMLDeclarationMultistage_HEV_IEPC_S_PrimaryVehicleBusJobInputDataProviderV11>()
                .Named(XMLDeclarationMultistage_HEV_IEPC_S_PrimaryVehicleBusJobInputDataProviderV11.QUALIFIED_XSD_TYPE);

            Bind<IXMLPrimaryVehicleBusJobInputData>().To<XMLDeclarationMultistage_PEV_Ex_PrimaryVehicleBusJobInputDataProviderV11>()
                .Named(XMLDeclarationMultistage_PEV_Ex_PrimaryVehicleBusJobInputDataProviderV11.QUALIFIED_XSD_TYPE);

            Bind<IXMLPrimaryVehicleBusJobInputData>().To<XMLDeclarationMultistage_PEV_IEPC_PrimaryVehicleBusJobInputDataProviderV11>()
                .Named(XMLDeclarationMultistage_PEV_IEPC_PrimaryVehicleBusJobInputDataProviderV11.QUALIFIED_XSD_TYPE);

            Bind<IXMLPrimaryVehicleBusJobInputData>().To<XMLDeclarationMultistage_FCHV_Fx_PrimaryVehicleBusJobInputDataProviderV11>()
                .Named(XMLDeclarationMultistage_FCHV_Fx_PrimaryVehicleBusJobInputDataProviderV11.QUALIFIED_XSD_TYPE);

            Bind<IXMLPrimaryVehicleBusJobInputData>().To<XMLDeclarationMultistage_FCHV_IEPC_PrimaryVehicleBusJobInputDataProviderV11>()
                .Named(XMLDeclarationMultistage_FCHV_IEPC_PrimaryVehicleBusJobInputDataProviderV11.QUALIFIED_XSD_TYPE);

            Bind<IXMLPrimaryVehicleBusJobInputData>().To<XMLDeclarationMultistage_Multiple_FCHV_PrimaryVehicleBusJobInputDataProviderV11>()
                .Named(XMLDeclarationMultistage_Multiple_FCHV_PrimaryVehicleBusJobInputDataProviderV11.QUALIFIED_XSD_TYPE);

            Bind<IXMLPrimaryVehicleBusJobInputData>().To<XMLDeclarationMultistage_Multiple_PEV_PrimaryVehicleBusJobInputDataProviderV11>()
                .Named(XMLDeclarationMultistage_Multiple_PEV_PrimaryVehicleBusJobInputDataProviderV11.QUALIFIED_XSD_TYPE);

            Bind<IXMLPrimaryVehicleBusJobInputData>().To<XMLDeclarationMultistage_Multiple_SHEV_PrimaryVehicleBusJobInputDataProviderV11>()
                .Named(XMLDeclarationMultistage_Multiple_SHEV_PrimaryVehicleBusJobInputDataProviderV11.QUALIFIED_XSD_TYPE);

            Bind<IXMLPrimaryVehicleBusJobInputData>().To<XMLDeclarationMultistageExemptedPrimaryVehicleBusJobInputDataProviderV11>()
                .Named(XMLDeclarationMultistageExemptedPrimaryVehicleBusJobInputDataProviderV11.QUALIFIED_XSD_TYPE);

            Bind<IXMLJobDataReader>().To<XMLJobDataMultistage_Conventional_PrimaryVehicleReaderV11>()
                .Named(XMLJobDataMultistage_Conventional_PrimaryVehicleReaderV11.QUALIFIED_XSD_TYPE);

            Bind<IXMLJobDataReader>().To<XMLJobDataMultistage_HEV_Px_PrimaryVehicleReaderV11>()
                .Named(XMLJobDataMultistage_HEV_Px_PrimaryVehicleReaderV11.QUALIFIED_XSD_TYPE);

            Bind<IXMLJobDataReader>().To<XMLJobDataMultistage_HEV_Sx_PrimaryVehicleReaderV11>()
                .Named(XMLJobDataMultistage_HEV_Sx_PrimaryVehicleReaderV11.QUALIFIED_XSD_TYPE);

            Bind<IXMLJobDataReader>().To<XMLJobDataMultistage_HEV_IEPC_S_PrimaryVehicleReaderV11>()
                .Named(XMLJobDataMultistage_HEV_IEPC_S_PrimaryVehicleReaderV11.QUALIFIED_XSD_TYPE);

            Bind<IXMLJobDataReader>().To<XMLJobDataMultistage_PEV_Ex_PrimaryVehicleReaderV11>()
                .Named(XMLJobDataMultistage_PEV_Ex_PrimaryVehicleReaderV11.QUALIFIED_XSD_TYPE);

            Bind<IXMLJobDataReader>().To<XMLJobDataMultistage_PEV_IEPC_PrimaryVehicleReaderV11>()
                .Named(XMLJobDataMultistage_PEV_IEPC_PrimaryVehicleReaderV11.QUALIFIED_XSD_TYPE);

            Bind<IXMLJobDataReader>().To<XMLJobDataMultistage_FCHV_Fx_PrimaryVehicleReaderV11>()
                .Named(XMLJobDataMultistage_FCHV_Fx_PrimaryVehicleReaderV11.QUALIFIED_XSD_TYPE);

            Bind<IXMLJobDataReader>().To<XMLJobDataMultistage_FCHV_IEPC_PrimaryVehicleReaderV11>()
                .Named(XMLJobDataMultistage_FCHV_IEPC_PrimaryVehicleReaderV11.QUALIFIED_XSD_TYPE);

            Bind<IXMLJobDataReader>().To<XMLJobDataMultistage_Multiple_FCHV_PrimaryVehicleReaderV11>()
                .Named(XMLJobDataMultistage_Multiple_FCHV_PrimaryVehicleReaderV11.QUALIFIED_XSD_TYPE);

            Bind<IXMLJobDataReader>().To<XMLJobDataMultistage_Multiple_PEV_PrimaryVehicleReaderV11>()
                .Named(XMLJobDataMultistage_Multiple_PEV_PrimaryVehicleReaderV11.QUALIFIED_XSD_TYPE);

            Bind<IXMLJobDataReader>().To<XMLJobDataMultistage_Multiple_SHEV_PrimaryVehicleReaderV11>()
                .Named(XMLJobDataMultistage_Multiple_SHEV_PrimaryVehicleReaderV11.QUALIFIED_XSD_TYPE);

            Bind<IXMLJobDataReader>().To<XMLJobDataMultistageExemptedPrimaryVehicleReaderV11>()
                .Named(XMLJobDataMultistageExemptedPrimaryVehicleReaderV11.QUALIFIED_XSD_TYPE);

            Bind<IXMLDeclarationVehicleData>().To<XMLDeclarationMultistage_Conventional_PrimaryVehicleBusDataProviderV11>()
                .Named(XMLDeclarationMultistage_Conventional_PrimaryVehicleBusDataProviderV11.QUALIFIED_XSD_TYPE);

            Bind<IXMLDeclarationVehicleData>().To<XMLDeclarationMultistage_HEV_Px_PrimaryVehicleBusDataProviderV11>()
                .Named(XMLDeclarationMultistage_HEV_Px_PrimaryVehicleBusDataProviderV11.QUALIFIED_XSD_TYPE);

            Bind<IXMLDeclarationVehicleData>().To<XMLDeclarationMultistage_HEV_Sx_PrimaryVehicleBusDataProviderV11>()
                .Named(XMLDeclarationMultistage_HEV_Sx_PrimaryVehicleBusDataProviderV11.QUALIFIED_XSD_TYPE);

            Bind<IXMLDeclarationVehicleData>().To<XMLDeclarationMultistage_HEV_IEPC_S_PrimaryVehicleBusDataProviderV11>()
                .Named(XMLDeclarationMultistage_HEV_IEPC_S_PrimaryVehicleBusDataProviderV11.QUALIFIED_XSD_TYPE);

            Bind<IXMLDeclarationVehicleData>().To<XMLDeclarationMultistage_PEV_Ex_PrimaryVehicleBusDataProviderV11>()
                .Named(XMLDeclarationMultistage_PEV_Ex_PrimaryVehicleBusDataProviderV11.QUALIFIED_XSD_TYPE);

            Bind<IXMLDeclarationVehicleData>().To<XMLDeclarationMultistage_PEV_IEPC_PrimaryVehicleBusDataProviderV11>()
                .Named(XMLDeclarationMultistage_PEV_IEPC_PrimaryVehicleBusDataProviderV11.QUALIFIED_XSD_TYPE);

            Bind<IXMLDeclarationVehicleData>().To<XMLDeclarationMultistage_FCHV_Fx_PrimaryVehicleBusDataProviderV11>()
                .Named(XMLDeclarationMultistage_FCHV_Fx_PrimaryVehicleBusDataProviderV11.QUALIFIED_XSD_TYPE);

            Bind<IXMLDeclarationVehicleData>().To<XMLDeclarationMultistage_FCHV_IEPC_PrimaryVehicleBusDataProviderV11>()
                .Named(XMLDeclarationMultistage_FCHV_IEPC_PrimaryVehicleBusDataProviderV11.QUALIFIED_XSD_TYPE);

            Bind<IXMLDeclarationVehicleData>().To<XMLDeclarationMultistage_Multiple_FCHV_PrimaryVehicleBusDataProviderV11>()
                .Named(XMLDeclarationMultistage_Multiple_FCHV_PrimaryVehicleBusDataProviderV11.QUALIFIED_XSD_TYPE);

            Bind<IXMLDeclarationVehicleData>().To<XMLDeclarationMultistage_Multiple_PEV_PrimaryVehicleBusDataProviderV11>()
                .Named(XMLDeclarationMultistage_Multiple_PEV_PrimaryVehicleBusDataProviderV11.QUALIFIED_XSD_TYPE);

            Bind<IXMLDeclarationVehicleData>().To<XMLDeclarationMultistage_Multiple_SHEV_PrimaryVehicleBusDataProviderV11>()
                .Named(XMLDeclarationMultistage_Multiple_SHEV_PrimaryVehicleBusDataProviderV11.QUALIFIED_XSD_TYPE);

            Bind<IXMLDeclarationVehicleData>().To<XMLDeclarationMultistageExemptedPrimaryVehicleBusDataProviderV11>()
                .Named(XMLDeclarationMultistageExemptedPrimaryVehicleBusDataProviderV11.QUALIFIED_XSD_TYPE);

            // -----------------------------------

            Bind<IXMLComponentReader>().To<XMLMultistagePrimaryVehicleBus_Conventional_ComponentReaderV11>()
                .Named(XMLMultistagePrimaryVehicleBus_Conventional_ComponentReaderV11.QUALIFIED_XSD_TYPE);

            Bind<IXMLComponentReader>().To<XMLMultistagePrimaryVehicleBus_HEV_Px_ComponentReaderV11>()
                .Named(XMLMultistagePrimaryVehicleBus_HEV_Px_ComponentReaderV11.QUALIFIED_XSD_TYPE);

            Bind<IXMLComponentReader>().To<XMLMultistagePrimaryVehicleBus_HEV_S2_ComponentReaderV11>()
                .Named(XMLMultistagePrimaryVehicleBus_HEV_S2_ComponentReaderV11.QUALIFIED_XSD_TYPE);

            Bind<IXMLComponentReader>().To<XMLMultistagePrimaryVehicleBus_HEV_S3_ComponentReaderV11>()
                .Named(XMLMultistagePrimaryVehicleBus_HEV_S3_ComponentReaderV11.QUALIFIED_XSD_TYPE);

            Bind<IXMLComponentReader>().To<XMLMultistagePrimaryVehicleBus_HEV_S4_ComponentReaderV11>()
                .Named(XMLMultistagePrimaryVehicleBus_HEV_S4_ComponentReaderV11.QUALIFIED_XSD_TYPE);

            Bind<IXMLComponentReader>().To<XMLMultistagePrimaryVehicleBus_HEV_IEPC_S_ComponentReaderV11>()
                .Named(XMLMultistagePrimaryVehicleBus_HEV_IEPC_S_ComponentReaderV11.QUALIFIED_XSD_TYPE);

            Bind<IXMLComponentReader>().To<XMLMultistagePrimaryVehicleBus_PEV_E2_ComponentReaderV11>()
                .Named(XMLMultistagePrimaryVehicleBus_PEV_E2_ComponentReaderV11.QUALIFIED_XSD_TYPE);

            Bind<IXMLComponentReader>().To<XMLMultistagePrimaryVehicleBus_PEV_E3_ComponentReaderV11>()
                .Named(XMLMultistagePrimaryVehicleBus_PEV_E3_ComponentReaderV11.QUALIFIED_XSD_TYPE);

            Bind<IXMLComponentReader>().To<XMLMultistagePrimaryVehicleBus_PEV_E4_ComponentReaderV11>()
                .Named(XMLMultistagePrimaryVehicleBus_PEV_E4_ComponentReaderV11.QUALIFIED_XSD_TYPE);

            Bind<IXMLComponentReader>().To<XMLMultistagePrimaryVehicleBus_PEV_IEPC_ComponentReaderV11>()
                .Named(XMLMultistagePrimaryVehicleBus_PEV_IEPC_ComponentReaderV11.QUALIFIED_XSD_TYPE);

            Bind<IXMLComponentReader>().To<XMLMultistagePrimaryVehicleBus_FCHV_F2_ComponentReaderV11>()
                .Named(XMLMultistagePrimaryVehicleBus_FCHV_F2_ComponentReaderV11.QUALIFIED_XSD_TYPE);

            Bind<IXMLComponentReader>().To<XMLMultistagePrimaryVehicleBus_FCHV_F3_ComponentReaderV11>()
                .Named(XMLMultistagePrimaryVehicleBus_FCHV_F3_ComponentReaderV11.QUALIFIED_XSD_TYPE);

            Bind<IXMLComponentReader>().To<XMLMultistagePrimaryVehicleBus_FCHV_F4_ComponentReaderV11>()
                .Named(XMLMultistagePrimaryVehicleBus_FCHV_F4_ComponentReaderV11.QUALIFIED_XSD_TYPE);

            Bind<IXMLComponentReader>().To<XMLMultistagePrimaryVehicleBus_FCHV_IEPC_ComponentReaderV11>()
                .Named(XMLMultistagePrimaryVehicleBus_FCHV_IEPC_ComponentReaderV11.QUALIFIED_XSD_TYPE);

            Bind<IXMLComponentReader>().To<XMLMultistagePrimaryVehicleBus_Multiple_FCHV_ComponentReaderV11>()
                .Named(XMLMultistagePrimaryVehicleBus_Multiple_FCHV_ComponentReaderV11.QUALIFIED_XSD_TYPE);

            Bind<IXMLComponentReader>().To<XMLMultistagePrimaryVehicleBus_Multiple_PEV_ComponentReaderV11>()
                .Named(XMLMultistagePrimaryVehicleBus_Multiple_PEV_ComponentReaderV11.QUALIFIED_XSD_TYPE);

            Bind<IXMLComponentReader>().To<XMLMultistagePrimaryVehicleBus_Multiple_SHEV_ComponentReaderV11>()
                .Named(XMLMultistagePrimaryVehicleBus_Multiple_SHEV_ComponentReaderV11.QUALIFIED_XSD_TYPE);

            Bind<IXMLGearboxReader>().To<XMLMultistagePrimaryVehicleBus_Conventional_ComponentReaderV11>()
                .Named(XMLMultistagePrimaryVehicleBus_Conventional_ComponentReaderV11.GEARBOX_READER_QUALIFIED_XSD_TYPE);

            Bind<IXMLAxlesReader>().To<XMLMultistagePrimaryVehicleBus_Conventional_ComponentReaderV11>()
                .Named(XMLMultistagePrimaryVehicleBus_Conventional_ComponentReaderV11.AXLES_READER_QUALIFIED_XSD_TYPE);

            Bind<IXMLElectricMachineSystemReader>().To<XMLElectricMachineSystemReaderV11>().Named(XMLElectricMachineSystemReaderV11.QUALIFIED_XSD_TYPE);
            Bind<IXMLElectricMachineSystemReader>().To<XMLElectricMachineSystemReaderV11>().Named(XMLElectricMachineSystemReaderV11.QUALIFIED_GEN_XSD_TYPE);

            Bind<IXMLElectricStorageSystemDeclarationInputData>().To<XMLElectricStorageSystemDeclarationInputDataV11>()
                .Named(XMLElectricStorageSystemDeclarationInputDataV11.QUALIFIED_XSD_TYPE);

            Bind<IXMLREESSReader>().To<XMLREESSReaderV11>().Named(XMLREESSReaderV11.QUALIFIED_XSD_TYPE);

            Bind<IXMLBatteryPackDeclarationInputData>().To<XMLBatteryPackDeclarationInputDataMeasuredV11>()
                .Named(XMLBatteryPackDeclarationInputDataMeasuredV11.QUALIFIED_XSD_TYPE);

            Bind<IXMLBatteryPackDeclarationInputData>().To<XMLBatteryPackDeclarationInputDataStandardV11>()
                .Named(XMLBatteryPackDeclarationInputDataStandardV11.QUALIFIED_XSD_TYPE);

            Bind<IXMLSuperCapDeclarationInputData>().To<XMLSuperCapDeclarationInputDataV11>()
                .Named(XMLSuperCapDeclarationInputDataV11.QUALIFIED_XSD_TYPE);

            Bind<IXMLVehicleComponentsDeclaration>()
                .To<XMLDeclarationComponentsMultistagePrimaryVehicleBus_Conventional_DataProviderV11>()
                .Named(XMLDeclarationComponentsMultistagePrimaryVehicleBus_Conventional_DataProviderV11.QUALIFIED_XSD_TYPE);

            Bind<IXMLVehicleComponentsDeclaration>()
                .To<XMLDeclarationComponentsMultistagePrimaryVehicleBus_HEV_Px_DataProviderV11>()
                .Named(XMLDeclarationComponentsMultistagePrimaryVehicleBus_HEV_Px_DataProviderV11.QUALIFIED_XSD_TYPE);

            Bind<IXMLVehicleComponentsDeclaration>()
                .To<XMLDeclarationComponentsMultistagePrimaryVehicleBus_HEV_S2_DataProviderV11>()
                .Named(XMLDeclarationComponentsMultistagePrimaryVehicleBus_HEV_S2_DataProviderV11.QUALIFIED_XSD_TYPE);

            Bind<IXMLVehicleComponentsDeclaration>()
               .To<XMLDeclarationComponentsMultistagePrimaryVehicleBus_HEV_S3_DataProviderV11>()
               .Named(XMLDeclarationComponentsMultistagePrimaryVehicleBus_HEV_S3_DataProviderV11.QUALIFIED_XSD_TYPE);

            Bind<IXMLVehicleComponentsDeclaration>()
               .To<XMLDeclarationComponentsMultistagePrimaryVehicleBus_HEV_S4_DataProviderV11>()
               .Named(XMLDeclarationComponentsMultistagePrimaryVehicleBus_HEV_S4_DataProviderV11.QUALIFIED_XSD_TYPE);

            Bind<IXMLVehicleComponentsDeclaration>()
                .To<XMLDeclarationComponentsMultistagePrimaryVehicleBus_HEV_IEPC_S_DataProviderV11>()
                .Named(XMLDeclarationComponentsMultistagePrimaryVehicleBus_HEV_IEPC_S_DataProviderV11.QUALIFIED_XSD_TYPE);

            Bind<IXMLVehicleComponentsDeclaration>()
                .To<XMLDeclarationComponentsMultistagePrimaryVehicleBus_PEV_E2_DataProviderV11>()
                .Named(XMLDeclarationComponentsMultistagePrimaryVehicleBus_PEV_E2_DataProviderV11.QUALIFIED_XSD_TYPE);

            Bind<IXMLVehicleComponentsDeclaration>()
                .To<XMLDeclarationComponentsMultistagePrimaryVehicleBus_PEV_E3_DataProviderV11>()
                .Named(XMLDeclarationComponentsMultistagePrimaryVehicleBus_PEV_E3_DataProviderV11.QUALIFIED_XSD_TYPE);

            Bind<IXMLVehicleComponentsDeclaration>()
                .To<XMLDeclarationComponentsMultistagePrimaryVehicleBus_PEV_E4_DataProviderV11>()
                .Named(XMLDeclarationComponentsMultistagePrimaryVehicleBus_PEV_E4_DataProviderV11.QUALIFIED_XSD_TYPE);

            Bind<IXMLVehicleComponentsDeclaration>()
                .To<XMLDeclarationComponentsMultistagePrimaryVehicleBus_PEV_IEPC_DataProviderV11>()
                .Named(XMLDeclarationComponentsMultistagePrimaryVehicleBus_PEV_IEPC_DataProviderV11.QUALIFIED_XSD_TYPE);

            Bind<IXMLVehicleComponentsDeclaration>()
                .To<XMLDeclarationComponentsMultistagePrimaryVehicleBus_FCHV_F2_DataProviderV11>()
                .Named(XMLDeclarationComponentsMultistagePrimaryVehicleBus_FCHV_F2_DataProviderV11.QUALIFIED_XSD_TYPE);

            Bind<IXMLVehicleComponentsDeclaration>()
                .To<XMLDeclarationComponentsMultistagePrimaryVehicleBus_FCHV_F3_DataProviderV11>()
                .Named(XMLDeclarationComponentsMultistagePrimaryVehicleBus_FCHV_F3_DataProviderV11.QUALIFIED_XSD_TYPE);

            Bind<IXMLVehicleComponentsDeclaration>()
                .To<XMLDeclarationComponentsMultistagePrimaryVehicleBus_FCHV_F4_DataProviderV11>()
                .Named(XMLDeclarationComponentsMultistagePrimaryVehicleBus_FCHV_F4_DataProviderV11.QUALIFIED_XSD_TYPE);

            Bind<IXMLVehicleComponentsDeclaration>()
               .To<XMLDeclarationComponentsMultistagePrimaryVehicleBus_FCHV_IEPC_DataProviderV11>()
               .Named(XMLDeclarationComponentsMultistagePrimaryVehicleBus_FCHV_IEPC_DataProviderV11.QUALIFIED_XSD_TYPE);

            Bind<IXMLVehicleComponentsDeclaration>()
               .To<XMLDeclarationComponentsMultistagePrimaryVehicleBus_Multiple_FCHV_DataProviderV11>()
               .Named(XMLDeclarationComponentsMultistagePrimaryVehicleBus_Multiple_FCHV_DataProviderV11.QUALIFIED_XSD_TYPE);

            Bind<IXMLVehicleComponentsDeclaration>()
               .To<XMLDeclarationComponentsMultistagePrimaryVehicleBus_Multiple_PEV_DataProviderV11>()
               .Named(XMLDeclarationComponentsMultistagePrimaryVehicleBus_Multiple_PEV_DataProviderV11.QUALIFIED_XSD_TYPE);

            Bind<IXMLVehicleComponentsDeclaration>()
               .To<XMLDeclarationComponentsMultistagePrimaryVehicleBus_Multiple_SHEV_DataProviderV11>()
               .Named(XMLDeclarationComponentsMultistagePrimaryVehicleBus_Multiple_SHEV_DataProviderV11.QUALIFIED_XSD_TYPE);

            // -----------------------------------

            Bind<IXMLFuelCellSystemDeclarationInputData>().To<XMLDeclarationMultistagePrimaryVehicleBusFuelCellDataProviderV11>()
                .Named(XMLDeclarationMultistagePrimaryVehicleBusFuelCellDataProviderV11.QUALIFIED_XSD_TYPE);

            Bind<IXMLEngineDeclarationInputData>().To<XMLDeclarationMultistagePrimaryVehicleBusEngineDataProviderV11>()
                .Named(XMLDeclarationMultistagePrimaryVehicleBusEngineDataProviderV11.QUALIFIED_XSD_TYPE);

            Bind<IXMLGearboxDeclarationInputData>().To<XMLDeclarationMultistagePrimaryVehicleBusGearboxDataProviderV11>()
                .Named(XMLDeclarationMultistagePrimaryVehicleBusGearboxDataProviderV11.QUALIFIED_XSD_TYPE);

            Bind<IXMLGearData>().To<XMLMultistagePrimaryVehicleBusTransmissionDataV11>()
                .Named(XMLMultistagePrimaryVehicleBusTransmissionDataV11.QUALIFIED_XSD_TYPE);

            Bind<IXMLAngledriveInputData>().To<XMLDeclarationMultistagePrimaryVehicleBusAngledriveDataProviderV11>()
                .Named(XMLDeclarationMultistagePrimaryVehicleBusAngledriveDataProviderV11.QUALIFIED_XSD_TYPE);

            Bind<IXMLTorqueConverterDeclarationInputData>()
                .To<XMLDeclarationMultistagePrimaryTorqueConverterDataProviderV11>()
                .Named(XMLDeclarationMultistagePrimaryTorqueConverterDataProviderV11.QUALIFIED_XSD_TYPE);

            Bind<IXMLAxleGearInputData>().To<XMLDeclarationMultistagePrimaryVehicleBusAxlegearDataProviderV11>().Named(
                XMLDeclarationMultistagePrimaryVehicleBusAxlegearDataProviderV11.QUALIFIED_XSD_TYPE);

            Bind<IXMLAxlesDeclarationInputData>().To<XMLDeclarationAxlesDataProviderV11>().Named(
                XMLDeclarationAxlesDataProviderV11.QUALIFIED_XSD_TYPE);

            Bind<IXMLApplicationInformationData>().To<XMLDeclarationMultistagePrimaryVehicleBusApplicationInformationDataProviderV11>()
                .Named(XMLDeclarationMultistagePrimaryVehicleBusApplicationInformationDataProviderV11.QUALIFIED_XSD_TYPE);

            Bind<IXMLResultsInputData>().To<XMLDeclarationMultistagePrimaryVehicleBusResultsInputDataProviderV11>()
                .Named(XMLDeclarationMultistagePrimaryVehicleBusResultsInputDataProviderV11.QUALIFIED_XSD_TYPE);

            Bind<IXMLElectricMachinesDeclarationInputData>().To<XMLDeclarationElectricMachinesDataProviderV11>().Named(
                XMLDeclarationElectricMachinesDataProviderV11.QUALIFIED_XSD_TYPE);

            Bind<IXMLElectricMachinesDeclarationInputData>().To<XMLDeclarationElectricMachinesDataProviderV11>().Named(
                XMLDeclarationElectricMachinesDataProviderV11.QUALIFIED_GEN_XSD_TYPE);

            Bind<IXMLElectricMotorDeclarationInputData>().To<XMLElectricMotorDeclarationInputDataProviderV11>()
                .Named(XMLElectricMotorDeclarationInputDataProviderV11.QUALIFIED_XSD_TYPE);

            Bind<IXMLIEPCInputData>().To<XMLElectricMotorIEPCIInputDataProviderV11>()
                .Named(XMLElectricMotorIEPCIInputDataProviderV11.QUALIFIED_XSD_TYPE);

            Bind<IXMLADCDeclarationInputData>().To<XMLADCDeclarationInputDataV11>()
                .Named(XMLADCDeclarationInputDataV11.QUALIFIED_XSD_TYPE);

            // -----------------------------------

            Bind<IXMLMultistageEntryInputDataProvider>().To<XMLDeclarationMultistageTypeInputDataV11>()
                .Named(XMLDeclarationMultistageTypeInputDataV11.QUALIFIED_XSD_TYPE);

            Bind<IXMLMultistageReader>().To<XMLMultistageEntryReaderV11>()
                .Named(XMLMultistageEntryReaderV11.QUALIFIED_XSD_TYPE);

            // ------------------------------------

            Bind<IXMLRetarderInputData>().To<XMLDeclarationMultistageRetarderDataProviderV11>()
                .Named(XMLDeclarationMultistageRetarderDataProviderV11.QUALIFIED_XSD_TYPE);

            // Axle Powertrains
            Bind<IXMLAxlePowertrainDeclarationInputData>().To<XMLDeclarationMultistage_AxlePowertrain_EM2_DataProviderV11>()
                .Named(XMLDeclarationMultistage_AxlePowertrain_EM2_DataProviderV11.QUALIFIED_XSD_TYPE);

            Bind<IXMLAxlePowertrainDeclarationInputData>().To<XMLDeclarationMultistage_AxlePowertrain_EM3_DataProviderV11>()
                .Named(XMLDeclarationMultistage_AxlePowertrain_EM3_DataProviderV11.QUALIFIED_XSD_TYPE);

            Bind<IXMLAxlePowertrainDeclarationInputData>().To<XMLDeclarationMultistage_AxlePowertrain_EM4_DataProviderV11>()
                .Named(XMLDeclarationMultistage_AxlePowertrain_EM4_DataProviderV11.QUALIFIED_XSD_TYPE);

            Bind<IXMLAxlePowertrainDeclarationInputData>().To<XMLDeclarationMultistage_AxlePowertrain_IEPC_DataProviderV11>()
                .Named(XMLDeclarationMultistage_AxlePowertrain_IEPC_DataProviderV11.QUALIFIED_XSD_TYPE);
        }
    }
}
