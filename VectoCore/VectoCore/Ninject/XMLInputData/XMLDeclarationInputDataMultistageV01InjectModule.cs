using Ninject.Modules;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.DataProvider;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Interfaces;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Reader;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Reader.Impl;

namespace TUGraz.VectoCore.Ninject.XMLInputData
{
	public class XMLDeclarationInputDataMultistageV01InjectModule : NinjectModule
	{
		public override void Load()
		{
			Bind<IXMLMultistageInputDataProvider>().To<XMLDeclarationInputDataProviderMultistageV01>()
				.Named(XMLDeclarationInputDataProviderMultistageV01.QUALIFIED_XSD_TYPE);

			Bind<IXMLDeclarationMultistageJobInputData>().To<XMLDeclarationMultistageJobInputDataV01>()
				.Named(XMLDeclarationMultistageJobInputDataV01.QUALIFIED_XSD_TYPE);

			// -----------------------------------

			Bind<IXMLDeclarationMultistageVehicleInputDataReader>().To<XMLDeclarationMultistageInputReaderV01>()
				.Named(XMLDeclarationMultistageInputReaderV01.QUALIFIED_XSD_TYPE);

			Bind<IXMLMultistageJobReader>().To<XMLMultistageJobReaderV01>()
				.Named(XMLMultistageJobReaderV01.QUALIFIED_XSD_TYPE);

			// -----------------------------------

			Bind<IXMLPrimaryVehicleBusInputData>().To<XMLDeclarationMultistagePrimaryVehicleInputDataV01>()
				.Named(XMLDeclarationMultistagePrimaryVehicleInputDataV01.QUALIFIED_XSD_TYPE);

			Bind<IXMLBusAuxiliariesDeclarationData>().To<XMLPrimaryBusAuxiliaries_Conventional_DataProviderV01>()
				.Named(XMLPrimaryBusAuxiliaries_Conventional_DataProviderV01.QUALIFIED_XSD_TYPE);

			Bind<IXMLBusAuxiliariesDeclarationData>().To<XMLPrimaryBusAuxiliaries_HEV_P_DataProviderV01>()
				.Named(XMLPrimaryBusAuxiliaries_HEV_P_DataProviderV01.QUALIFIED_XSD_TYPE);

			Bind<IXMLBusAuxiliariesDeclarationData>().To<XMLPrimaryBusAuxiliaries_HEV_S_DataProviderV01>()
				.Named(XMLPrimaryBusAuxiliaries_HEV_S_DataProviderV01.QUALIFIED_XSD_TYPE);

			Bind<IXMLBusAuxiliariesDeclarationData>().To<XMLPrimaryBusAuxiliaries_PEV_DataProviderV01>()
				.Named(XMLPrimaryBusAuxiliaries_PEV_DataProviderV01.QUALIFIED_XSD_TYPE);

			Bind<IXMLBusAuxiliariesDeclarationData>().To<XMLPrimaryBusAuxiliaries_IEPC_DataProviderV01>()
				.Named(XMLPrimaryBusAuxiliaries_IEPC_DataProviderV01.QUALIFIED_XSD_TYPE);

			Bind<IXMLDeclarationPrimaryVehicleBusInputDataReader>().To<XMLMultistagePrimaryVehicleReaderV01>()
				.Named(XMLMultistagePrimaryVehicleReaderV01.QUALIFIED_XSD_TYPE);

			Bind<IXMLPrimaryVehicleBusJobInputData>().To<XMLDeclarationMultistage_Conventional_PrimaryVehicleBusJobInputDataProviderV01>()
				.Named(XMLDeclarationMultistage_Conventional_PrimaryVehicleBusJobInputDataProviderV01.QUALIFIED_XSD_TYPE);

			Bind<IXMLPrimaryVehicleBusJobInputData>().To<XMLDeclarationMultistage_HEV_Px_PrimaryVehicleBusJobInputDataProviderV01>()
				.Named(XMLDeclarationMultistage_HEV_Px_PrimaryVehicleBusJobInputDataProviderV01.QUALIFIED_XSD_TYPE);

			Bind<IXMLPrimaryVehicleBusJobInputData>().To<XMLDeclarationMultistage_HEV_Sx_PrimaryVehicleBusJobInputDataProviderV01>()
				.Named(XMLDeclarationMultistage_HEV_Sx_PrimaryVehicleBusJobInputDataProviderV01.QUALIFIED_XSD_TYPE);

			Bind<IXMLPrimaryVehicleBusJobInputData>().To<XMLDeclarationMultistage_HEV_IEPC_S_PrimaryVehicleBusJobInputDataProviderV01>()
				.Named(XMLDeclarationMultistage_HEV_IEPC_S_PrimaryVehicleBusJobInputDataProviderV01.QUALIFIED_XSD_TYPE);

			Bind<IXMLPrimaryVehicleBusJobInputData>().To<XMLDeclarationMultistage_PEV_Ex_PrimaryVehicleBusJobInputDataProviderV01>()
				.Named(XMLDeclarationMultistage_PEV_Ex_PrimaryVehicleBusJobInputDataProviderV01.QUALIFIED_XSD_TYPE);

			Bind<IXMLPrimaryVehicleBusJobInputData>().To<XMLDeclarationMultistage_PEV_IEPC_PrimaryVehicleBusJobInputDataProviderV01>()
				.Named(XMLDeclarationMultistage_PEV_IEPC_PrimaryVehicleBusJobInputDataProviderV01.QUALIFIED_XSD_TYPE);

			Bind<IXMLPrimaryVehicleBusJobInputData>().To<XMLDeclarationMultistageExemptedPrimaryVehicleBusJobInputDataProviderV01>()
				.Named(XMLDeclarationMultistageExemptedPrimaryVehicleBusJobInputDataProviderV01.QUALIFIED_XSD_TYPE);

			Bind<IXMLJobDataReader>().To<XMLJobDataMultistage_Conventional_PrimaryVehicleReaderV01>()
				.Named(XMLJobDataMultistage_Conventional_PrimaryVehicleReaderV01.QUALIFIED_XSD_TYPE);

			Bind<IXMLJobDataReader>().To<XMLJobDataMultistage_HEV_Px_PrimaryVehicleReaderV01>()
				.Named(XMLJobDataMultistage_HEV_Px_PrimaryVehicleReaderV01.QUALIFIED_XSD_TYPE);

			Bind<IXMLJobDataReader>().To<XMLJobDataMultistage_HEV_Sx_PrimaryVehicleReaderV01>()
				.Named(XMLJobDataMultistage_HEV_Sx_PrimaryVehicleReaderV01.QUALIFIED_XSD_TYPE);

			Bind<IXMLJobDataReader>().To<XMLJobDataMultistage_HEV_IEPC_S_PrimaryVehicleReaderV01>()
				.Named(XMLJobDataMultistage_HEV_IEPC_S_PrimaryVehicleReaderV01.QUALIFIED_XSD_TYPE);

			Bind<IXMLJobDataReader>().To<XMLJobDataMultistage_PEV_Ex_PrimaryVehicleReaderV01>()
				.Named(XMLJobDataMultistage_PEV_Ex_PrimaryVehicleReaderV01.QUALIFIED_XSD_TYPE);

			Bind<IXMLJobDataReader>().To<XMLJobDataMultistage_PEV_IEPC_PrimaryVehicleReaderV01>()
				.Named(XMLJobDataMultistage_PEV_IEPC_PrimaryVehicleReaderV01.QUALIFIED_XSD_TYPE);

			Bind<IXMLJobDataReader>().To<XMLJobDataMultistageExemptedPrimaryVehicleReaderV01>()
				.Named(XMLJobDataMultistageExemptedPrimaryVehicleReaderV01.QUALIFIED_XSD_TYPE);

			Bind<IXMLDeclarationVehicleData>().To<XMLDeclarationMultistage_Conventional_PrimaryVehicleBusDataProviderV01>()
				.Named(XMLDeclarationMultistage_Conventional_PrimaryVehicleBusDataProviderV01.QUALIFIED_XSD_TYPE);

			Bind<IXMLDeclarationVehicleData>().To<XMLDeclarationMultistage_HEV_Px_PrimaryVehicleBusDataProviderV01>()
				.Named(XMLDeclarationMultistage_HEV_Px_PrimaryVehicleBusDataProviderV01.QUALIFIED_XSD_TYPE);

			Bind<IXMLDeclarationVehicleData>().To<XMLDeclarationMultistage_HEV_Sx_PrimaryVehicleBusDataProviderV01>()
				.Named(XMLDeclarationMultistage_HEV_Sx_PrimaryVehicleBusDataProviderV01.QUALIFIED_XSD_TYPE);

			Bind<IXMLDeclarationVehicleData>().To<XMLDeclarationMultistage_HEV_IEPC_S_PrimaryVehicleBusDataProviderV01>()
				.Named(XMLDeclarationMultistage_HEV_IEPC_S_PrimaryVehicleBusDataProviderV01.QUALIFIED_XSD_TYPE);

			Bind<IXMLDeclarationVehicleData>().To<XMLDeclarationMultistage_PEV_Ex_PrimaryVehicleBusDataProviderV01>()
				.Named(XMLDeclarationMultistage_PEV_Ex_PrimaryVehicleBusDataProviderV01.QUALIFIED_XSD_TYPE);

			Bind<IXMLDeclarationVehicleData>().To<XMLDeclarationMultistage_PEV_IEPC_PrimaryVehicleBusDataProviderV01>()
				.Named(XMLDeclarationMultistage_PEV_IEPC_PrimaryVehicleBusDataProviderV01.QUALIFIED_XSD_TYPE);

			Bind<IXMLDeclarationVehicleData>().To<XMLDeclarationMultistageExemptedPrimaryVehicleBusDataProviderV01>()
				.Named(XMLDeclarationMultistageExemptedPrimaryVehicleBusDataProviderV01.QUALIFIED_XSD_TYPE);

			// -----------------------------------

			Bind<IXMLComponentReader>().To<XMLMultistagePrimaryVehicleBus_Conventional_ComponentReaderV01>()
				.Named(XMLMultistagePrimaryVehicleBus_Conventional_ComponentReaderV01.QUALIFIED_XSD_TYPE);

			Bind<IXMLComponentReader>().To<XMLMultistagePrimaryVehicleBus_HEV_Px_ComponentReaderV01>()
				.Named(XMLMultistagePrimaryVehicleBus_HEV_Px_ComponentReaderV01.QUALIFIED_XSD_TYPE);

			Bind<IXMLComponentReader>().To<XMLMultistagePrimaryVehicleBus_HEV_S2_ComponentReaderV01>()
				.Named(XMLMultistagePrimaryVehicleBus_HEV_S2_ComponentReaderV01.QUALIFIED_XSD_TYPE);

			Bind<IXMLComponentReader>().To<XMLMultistagePrimaryVehicleBus_HEV_S3_ComponentReaderV01>()
				.Named(XMLMultistagePrimaryVehicleBus_HEV_S3_ComponentReaderV01.QUALIFIED_XSD_TYPE);

			Bind<IXMLComponentReader>().To<XMLMultistagePrimaryVehicleBus_HEV_S4_ComponentReaderV01>()
				.Named(XMLMultistagePrimaryVehicleBus_HEV_S4_ComponentReaderV01.QUALIFIED_XSD_TYPE);

			Bind<IXMLComponentReader>().To<XMLMultistagePrimaryVehicleBus_HEV_IEPC_S_ComponentReaderV01>()
				.Named(XMLMultistagePrimaryVehicleBus_HEV_IEPC_S_ComponentReaderV01.QUALIFIED_XSD_TYPE);

			Bind<IXMLComponentReader>().To<XMLMultistagePrimaryVehicleBus_PEV_E2_ComponentReaderV01>()
				.Named(XMLMultistagePrimaryVehicleBus_PEV_E2_ComponentReaderV01.QUALIFIED_XSD_TYPE);

			Bind<IXMLComponentReader>().To<XMLMultistagePrimaryVehicleBus_PEV_E3_ComponentReaderV01>()
				.Named(XMLMultistagePrimaryVehicleBus_PEV_E3_ComponentReaderV01.QUALIFIED_XSD_TYPE);

			Bind<IXMLComponentReader>().To<XMLMultistagePrimaryVehicleBus_PEV_E4_ComponentReaderV01>()
				.Named(XMLMultistagePrimaryVehicleBus_PEV_E4_ComponentReaderV01.QUALIFIED_XSD_TYPE);

			Bind<IXMLComponentReader>().To<XMLMultistagePrimaryVehicleBus_PEV_IEPC_ComponentReaderV01>()
				.Named(XMLMultistagePrimaryVehicleBus_PEV_IEPC_ComponentReaderV01.QUALIFIED_XSD_TYPE);

			Bind<IXMLGearboxReader>().To<XMLMultistagePrimaryVehicleBus_Conventional_ComponentReaderV01>()
				.Named(XMLMultistagePrimaryVehicleBus_Conventional_ComponentReaderV01.GEARBOX_READER_QUALIFIED_XSD_TYPE);

			Bind<IXMLAxlesReader>().To<XMLMultistagePrimaryVehicleBus_Conventional_ComponentReaderV01>()
				.Named(XMLMultistagePrimaryVehicleBus_Conventional_ComponentReaderV01.AXLES_READER_QUALIFIED_XSD_TYPE);

			Bind<IXMLElectricMachineSystemReader>().To<XMLElectricMachineSystemReaderV01>().Named(XMLElectricMachineSystemReaderV01.QUALIFIED_XSD_TYPE);
			Bind<IXMLElectricMachineSystemReader>().To<XMLElectricMachineSystemReaderV01>().Named(XMLElectricMachineSystemReaderV01.QUALIFIED_GEN_XSD_TYPE);

			Bind<IXMLElectricStorageSystemDeclarationInputData>().To<XMLElectricStorageSystemDeclarationInputDataV01>()
				.Named(XMLElectricStorageSystemDeclarationInputDataV01.QUALIFIED_XSD_TYPE);

			Bind<IXMLREESSReader>().To<XMLREESSReaderV01>().Named(XMLREESSReaderV01.QUALIFIED_XSD_TYPE);

			//Bind<IXMLBatteryPackDeclarationInputData>().To<XMLBatteryPackDeclarationDeclarationInputDataV01>()
			//	.Named(XMLBatteryPackDeclarationDeclarationInputDataV01.QUALIFIED_XSD_TYPE);

			Bind<IXMLBatteryPackDeclarationInputData>().To<XMLBatteryPackDeclarationInputDataMeasuredV01>()
				.Named(XMLBatteryPackDeclarationInputDataMeasuredV01.QUALIFIED_XSD_TYPE);

			Bind<IXMLBatteryPackDeclarationInputData>().To<XMLBatteryPackDeclarationInputDataStandardV01>()
				.Named(XMLBatteryPackDeclarationInputDataStandardV01.QUALIFIED_XSD_TYPE);

			Bind<IXMLSuperCapDeclarationInputData>().To<XMLSuperCapDeclarationInputDataV01>()
				.Named(XMLSuperCapDeclarationInputDataV01.QUALIFIED_XSD_TYPE);

			//Bind<IXMLADCDeclarationInputData>().To<XMLADCDeclarationInputDataV23>()
			//	.Named(XMLADCDeclarationInputDataV23.QUALIFIED_XSD_TYPE);



			Bind<IXMLVehicleComponentsDeclaration>()
				.To<XMLDeclarationComponentsMultistagePrimaryVehicleBus_Conventional_DataProviderV01>()
				.Named(XMLDeclarationComponentsMultistagePrimaryVehicleBus_Conventional_DataProviderV01.QUALIFIED_XSD_TYPE);

			Bind<IXMLVehicleComponentsDeclaration>()
				.To<XMLDeclarationComponentsMultistagePrimaryVehicleBus_HEV_Px_DataProviderV01>()
				.Named(XMLDeclarationComponentsMultistagePrimaryVehicleBus_HEV_Px_DataProviderV01.QUALIFIED_XSD_TYPE);

			Bind<IXMLVehicleComponentsDeclaration>()
				.To<XMLDeclarationComponentsMultistagePrimaryVehicleBus_HEV_S2_DataProviderV01>()
				.Named(XMLDeclarationComponentsMultistagePrimaryVehicleBus_HEV_S2_DataProviderV01.QUALIFIED_XSD_TYPE);

			Bind<IXMLVehicleComponentsDeclaration>()
				.To<XMLDeclarationComponentsMultistagePrimaryVehicleBus_HEV_S3_DataProviderV01>()
				.Named(XMLDeclarationComponentsMultistagePrimaryVehicleBus_HEV_S3_DataProviderV01.QUALIFIED_XSD_TYPE);

			Bind<IXMLVehicleComponentsDeclaration>()
				.To<XMLDeclarationComponentsMultistagePrimaryVehicleBus_HEV_S4_DataProviderV01>()
				.Named(XMLDeclarationComponentsMultistagePrimaryVehicleBus_HEV_S4_DataProviderV01.QUALIFIED_XSD_TYPE);

			Bind<IXMLVehicleComponentsDeclaration>()
				.To<XMLDeclarationComponentsMultistagePrimaryVehicleBus_HEV_IEPC_S_DataProviderV01>()
				.Named(XMLDeclarationComponentsMultistagePrimaryVehicleBus_HEV_IEPC_S_DataProviderV01.QUALIFIED_XSD_TYPE);

			Bind<IXMLVehicleComponentsDeclaration>()
				.To<XMLDeclarationComponentsMultistagePrimaryVehicleBus_PEV_E2_DataProviderV01>()
				.Named(XMLDeclarationComponentsMultistagePrimaryVehicleBus_PEV_E2_DataProviderV01.QUALIFIED_XSD_TYPE);

			Bind<IXMLVehicleComponentsDeclaration>()
				.To<XMLDeclarationComponentsMultistagePrimaryVehicleBus_PEV_E3_DataProviderV01>()
				.Named(XMLDeclarationComponentsMultistagePrimaryVehicleBus_PEV_E3_DataProviderV01.QUALIFIED_XSD_TYPE);

			Bind<IXMLVehicleComponentsDeclaration>()
				.To<XMLDeclarationComponentsMultistagePrimaryVehicleBus_PEV_E4_DataProviderV01>()
				.Named(XMLDeclarationComponentsMultistagePrimaryVehicleBus_PEV_E4_DataProviderV01.QUALIFIED_XSD_TYPE);

			Bind<IXMLVehicleComponentsDeclaration>()
				.To<XMLDeclarationComponentsMultistagePrimaryVehicleBus_PEV_IEPC_DataProviderV01>()
				.Named(XMLDeclarationComponentsMultistagePrimaryVehicleBus_PEV_IEPC_DataProviderV01.QUALIFIED_XSD_TYPE);

			// -----------------------------------

			Bind<IXMLEngineDeclarationInputData>().To<XMLDeclarationMultistagePrimaryVehicleBusEngineDataProviderV01>()
				.Named(XMLDeclarationMultistagePrimaryVehicleBusEngineDataProviderV01.QUALIFIED_XSD_TYPE);

			Bind<IXMLGearboxDeclarationInputData>().To<XMLDeclarationMultistagePrimaryVehicleBusGearboxDataProviderV01>()
				.Named(XMLDeclarationMultistagePrimaryVehicleBusGearboxDataProviderV01.QUALIFIED_XSD_TYPE);

			Bind<IXMLGearData>().To<XMLMultistagePrimaryVehicleBusTransmissionDataV01>()
				.Named(XMLMultistagePrimaryVehicleBusTransmissionDataV01.QUALIFIED_XSD_TYPE);

			Bind<IXMLAngledriveInputData>().To<XMLDeclarationMultistagePrimaryVehicleBusAngledriveDataProviderV01>()
				.Named(XMLDeclarationMultistagePrimaryVehicleBusAngledriveDataProviderV01.QUALIFIED_XSD_TYPE);

			Bind<IXMLTorqueConverterDeclarationInputData>()
				.To<XMLDeclarationMultistagePrimaryTorqueConverterDataProviderV01>()
				.Named(XMLDeclarationMultistagePrimaryTorqueConverterDataProviderV01.QUALIFIED_XSD_TYPE);

			Bind<IXMLAxleGearInputData>().To<XMLDeclarationMultistagePrimaryVehicleBusAxlegearDataProviderV01>().Named(
				XMLDeclarationMultistagePrimaryVehicleBusAxlegearDataProviderV01.QUALIFIED_XSD_TYPE);

			Bind<IXMLAxlesDeclarationInputData>().To<XMLDeclarationAxlesDataProviderV01>().Named(
				XMLDeclarationAxlesDataProviderV01.QUALIFIED_XSD_TYPE);

			Bind<IXMLApplicationInformationData>().To<XMLDeclarationMultistagePrimaryVehicleBusApplicationInformationDataProviderV01>()
				.Named(XMLDeclarationMultistagePrimaryVehicleBusApplicationInformationDataProviderV01.QUALIFIED_XSD_TYPE);

			Bind<IXMLResultsInputData>().To<XMLDeclarationMultistagePrimaryVehicleBusResultsInputDataProviderV01>()
				.Named(XMLDeclarationMultistagePrimaryVehicleBusResultsInputDataProviderV01.QUALIFIED_XSD_TYPE);

			Bind<IXMLElectricMachinesDeclarationInputData>().To<XMLDeclarationElectricMachinesDataProviderV01>().Named(
				XMLDeclarationElectricMachinesDataProviderV01.QUALIFIED_XSD_TYPE);

			Bind<IXMLElectricMachinesDeclarationInputData>().To<XMLDeclarationElectricMachinesDataProviderV01>().Named(
				XMLDeclarationElectricMachinesDataProviderV01.QUALIFIED_GEN_XSD_TYPE);

			Bind<IXMLElectricMotorDeclarationInputData>().To<XMLElectricMotorDeclarationInputDataProviderV01>()
				.Named(XMLElectricMotorDeclarationInputDataProviderV01.QUALIFIED_XSD_TYPE);

			Bind<IXMLIEPCInputData>().To<XMLElectricMotorIEPCIInputDataProviderV01>()
				.Named(XMLElectricMotorIEPCIInputDataProviderV01.QUALIFIED_XSD_TYPE);

			Bind<IXMLADCDeclarationInputData>().To<XMLADCDeclarationInputDataV01>()
				.Named(XMLADCDeclarationInputDataV01.QUALIFIED_XSD_TYPE);

			// -----------------------------------

			Bind<IXMLMultistageEntryInputDataProvider>().To<XMLDeclarationMultistageTypeInputDataV01>()
				.Named(XMLDeclarationMultistageTypeInputDataV01.QUALIFIED_XSD_TYPE);

			Bind<IXMLMultistageReader>().To<XMLMultistageEntryReaderV01>()
				.Named(XMLMultistageEntryReaderV01.QUALIFIED_XSD_TYPE);
		}

	}
}