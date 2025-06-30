using System.Xml;
using Ninject.Modules;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.DataProvider;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.DataProvider.v24;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Interfaces;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Reader;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Reader.Impl;

namespace TUGraz.VectoCore.InputData.FileIO.XML.Declaration.NinjectModules
{
	public class XMLDeclarationInputDataV24InjectModule : NinjectModule
	{
		#region Overrides of NinjectModlue

		public override void Load()
		{

			Bind<IXMLVehicleComponentsDeclaration>().To<XMLDeclarationComponentsDataProviderV24_Lorry>().Named(
				XMLDeclarationComponentsDataProviderV24_Lorry.QUALIFIED_XSD_TYPE);

			Bind<IXMLDeclarationVehicleData>().To<XMLDeclarationConventionalCompletedBusDataProviderV24>()
				.Named(XMLDeclarationConventionalCompletedBusDataProviderV24.QUALIFIED_XSD_TYPE);
			Bind<IXMLDeclarationVehicleData>().To<XMLDeclarationHevCompletedBusDataProviderV24>()
				.Named(XMLDeclarationHevCompletedBusDataProviderV24.QUALIFIED_XSD_TYPE);
			Bind<IXMLDeclarationVehicleData>().To<XMLDeclarationPEVCompletedBusDataProviderV24>()
				.Named(XMLDeclarationPEVCompletedBusDataProviderV24.QUALIFIED_XSD_TYPE);
			Bind<IXMLDeclarationVehicleData>().To<XMLDeclarationIepcCompletedBusDataProviderV24>()
				.Named(XMLDeclarationIepcCompletedBusDataProviderV24.QUALIFIED_XSD_TYPE);

			Bind<IXMLDeclarationVehicleData>().To<XMLDeclarationExemptedCompletedBusDataProviderV24>()
				.Named(XMLDeclarationExemptedCompletedBusDataProviderV24.QUALIFIED_XSD_TYPE);

			Bind<IXMLDeclarationVehicleData>().To<XMLDeclarationConventionalHeavyLorryDataProviderV24>()
				.Named(XMLDeclarationConventionalHeavyLorryDataProviderV24.QUALIFIED_XSD_TYPE);
			
			Bind<IXMLDeclarationVehicleData>().To<XMLDeclarationHevPxHeavyLorryDataProviderV24>()
				.Named(XMLDeclarationHevPxHeavyLorryDataProviderV24.QUALIFIED_XSD_TYPE);

			Bind<IXMLDeclarationVehicleData>().To<XMLDeclarationHevSxHeavyLorryDataProviderV24>()
				.Named(XMLDeclarationHevSxHeavyLorryDataProviderV24.QUALIFIED_XSD_TYPE);

			Bind<IXMLDeclarationVehicleData>().To<XMLDeclarationHevSxMediumLorryDataProviderV24>()
				.Named(XMLDeclarationHevSxMediumLorryDataProviderV24.QUALIFIED_XSD_TYPE);

			Bind<IXMLDeclarationVehicleData>().To<XMLDeclarationHevPxMediumLorryDataProviderV24>()
				.Named(XMLDeclarationHevPxMediumLorryDataProviderV24.QUALIFIED_XSD_TYPE);
			
			Bind<IXMLDeclarationVehicleData>().To<XMLDeclarationHevPxPrimaryBusDataProviderV24>()
				.Named(XMLDeclarationHevPxPrimaryBusDataProviderV24.QUALIFIED_XSD_TYPE);

			Bind<IXMLDeclarationVehicleData>().To<XMLDeclarationHevSxPrimaryBusDataProviderV24>()
				.Named(XMLDeclarationHevSxPrimaryBusDataProviderV24.QUALIFIED_XSD_TYPE);

			Bind<IXMLDeclarationVehicleData>().To<XMLDeclarationHevIEPCSHeavyLorryDataProviderV24>()
				.Named(XMLDeclarationHevIEPCSHeavyLorryDataProviderV24.QUALIFIED_XSD_TYPE);

			Bind<IXMLDeclarationVehicleData>().To<XMLDeclarationHeviepcsMediumLorryDataProviderV24>()
				.Named(XMLDeclarationHeviepcsMediumLorryDataProviderV24.QUALIFIED_XSD_TYPE);

			Bind<IXMLDeclarationVehicleData>().To<XMLDeclarationHeviepcsPrimaryBusDataProviderV24>()
				.Named(XMLDeclarationHeviepcsPrimaryBusDataProviderV24.QUALIFIED_XSD_TYPE);

			Bind<IXMLDeclarationVehicleData>().To<XMLDeclarationPevHeavyLorryDataProviderV24>()
				.Named(XMLDeclarationPevHeavyLorryDataProviderV24.QUALIFIED_XSD_TYPE);

			Bind<IXMLDeclarationVehicleData>().To<XMLDeclarationPevMediumLorryExDataProviderV24>()
				.Named(XMLDeclarationPevMediumLorryExDataProviderV24.QUALIFIED_XSD_TYPE);

			Bind<IXMLDeclarationVehicleData>().To<XMLDeclarationPevPrimaryBusDataProviderV24>()
				.Named(XMLDeclarationPevPrimaryBusDataProviderV24.QUALIFIED_XSD_TYPE);

			Bind<IXMLDeclarationVehicleData>().To<XMLDeclarationIEPCHeavyLorryDataProviderV24>()
				.Named(XMLDeclarationIEPCHeavyLorryDataProviderV24.QUALIFIED_XSD_TYPE);

			Bind<IXMLDeclarationVehicleData>().To<XMLDeclarationIepcMediumLorryDataProviderV24>()
				.Named(XMLDeclarationIepcMediumLorryDataProviderV24.QUALIFIED_XSD_TYPE);

			Bind<IXMLDeclarationVehicleData>().To<XMLDeclarationIepcPrimaryBusDataProviderV24>()
				.Named(XMLDeclarationIepcPrimaryBusDataProviderV24.QUALIFIED_XSD_TYPE);


			Bind<IXMLVehicleComponentsDeclaration>().To<XMLDeclarationCompletedBusComponentsDataProviderV24>()
				.Named(XMLDeclarationCompletedBusComponentsDataProviderV24.QUALIFIED_XSD_TYPE_CONVENTIONAL);
			Bind<IXMLVehicleComponentsDeclaration>().To<XMLDeclarationCompletedBusComponentsDataProviderV24>()
				.Named(XMLDeclarationCompletedBusComponentsDataProviderV24.QUALIFIED_XSD_TYPE_xEV);

			Bind<IXMLVehicleComponentsDeclaration>().To<XMLDeclarationHEVPxLorryComponentsDataProviderV24>()
				.Named(XMLDeclarationHEVPxLorryComponentsDataProviderV24.QUALIFIED_XSD_TYPE);

			Bind<IXMLVehicleComponentsDeclaration>().To<XMLDeclarationHevs2LorryComponentsDataProviderV24>()
				.Named(XMLDeclarationHevs2LorryComponentsDataProviderV24.QUALIFIED_XSD_TYPE);

			Bind<IXMLVehicleComponentsDeclaration>().To<XMLDeclarationHEVSXLorryComponentsDataProviderV24>()
				.Named(XMLDeclarationHEVSXLorryComponentsDataProviderV24.QUALIFIED_HEV_S3_XSD_TYPE);

			Bind<IXMLVehicleComponentsDeclaration>().To<XMLDeclarationHEVSXLorryComponentsDataProviderV24>()
				.Named(XMLDeclarationHEVSXLorryComponentsDataProviderV24.QUALIFIED_HEV_S4_XSD_TYPE);

			Bind<IXMLVehicleComponentsDeclaration>().To<XMLDeclarationPrimaryBusHEVPxComponentsDataProviderV24>()
				.Named(XMLDeclarationPrimaryBusHEVPxComponentsDataProviderV24.QUALIFIED_XSD_TYPE);

			Bind<IXMLVehicleComponentsDeclaration>().To<XMLDeclarationPrimaryBusHEVS2ComponentDataProviderV24>()
				.Named(XMLDeclarationPrimaryBusHEVS2ComponentDataProviderV24.QUALIFIED_XSD_TYPE);

			Bind<IXMLVehicleComponentsDeclaration>().To<XMLDeclarationPrimaryBusHEVSxComponentDataProviderV24>()
				.Named(XMLDeclarationPrimaryBusHEVSxComponentDataProviderV24.QUALIFIED_HEV_S3_XSD_TYPE);

			Bind<IXMLVehicleComponentsDeclaration>().To<XMLDeclarationPrimaryBusHEVSxComponentDataProviderV24>()
				.Named(XMLDeclarationPrimaryBusHEVSxComponentDataProviderV24.QUALIFIED_HEV_S4_XSD_TYPE);

			Bind<IXMLVehicleComponentsDeclaration>().To<XMLDeclarationHeavyLorryHEVIEPCSComponentDataV24>()
				.Named(XMLDeclarationHeavyLorryHEVIEPCSComponentDataV24.QUALIFIED_XSD_TYPE);

			Bind<IXMLVehicleComponentsDeclaration>().To<XMLDeclarationPrimaryBusHEVIEPCSComponentDataV24>()
				.Named(XMLDeclarationPrimaryBusHEVIEPCSComponentDataV24.QUALIFIED_XSD_TYPE);

			Bind<IXMLVehicleComponentsDeclaration>().To<XMLDeclarationHeavyLorryPEVE2ComponentDataV24>()
				.Named(XMLDeclarationHeavyLorryPEVE2ComponentDataV24.QUALIFIED_XSD_TYPE);

			Bind<IXMLVehicleComponentsDeclaration>().To<XMLDeclarationHeavyLorryPevExComponentDataV24>()
				.Named(XMLDeclarationHeavyLorryPevExComponentDataV24.QUALIFIED_XSD_TYPE);

			Bind<IXMLVehicleComponentsDeclaration>().To<XMLDeclarationHeavyLorryPevExComponentDataV24>()
				.Named(XMLDeclarationHeavyLorryPevExComponentDataV24.QUALIFIED_XSD_PEV_E4_TYPE);

			Bind<IXMLVehicleComponentsDeclaration>().To<XMLDeclarationPrimaryBusPEVE2ComponentDataV24>()
				.Named(XMLDeclarationPrimaryBusPEVE2ComponentDataV24.QUALIFIED_XSD_TYPE);

			Bind<IXMLVehicleComponentsDeclaration>().To<XMLDeclarationPrimaryBusPevExComponentDataV24>()
				.Named(XMLDeclarationPrimaryBusPevExComponentDataV24.QUALIFIED_XSD_TYPE);

			Bind<IXMLVehicleComponentsDeclaration>().To<XMLDeclarationPrimaryBusPevExComponentDataV24>()
				.Named(XMLDeclarationPrimaryBusPevExComponentDataV24.QUALIFIED_XSD_PEV_E4_TYPE);

			Bind<IXMLVehicleComponentsDeclaration>().To<XMLDeclarationIEPCHeavyLorryComponentDataV24>()
				.Named(XMLDeclarationIEPCHeavyLorryComponentDataV24.QUALIFIED_XSD_TYPE);

			Bind<IXMLVehicleComponentsDeclaration>().To<XMLDeclarationIEPCPrimaryBusComponentDataV24>()
				.Named(XMLDeclarationIEPCPrimaryBusComponentDataV24.QUALIFIED_XSD_TYPE);

			Bind<IXMLComponentReader>().To<XMLComponentReaderV24_CompletedBus>().Named(XMLComponentReaderV24_CompletedBus.QUALIFIED_XSD_TYPE_CONVENTIONAL);
			Bind<IXMLComponentReader>().To<XMLComponentReaderV24_CompletedBus>().Named(XMLComponentReaderV24_CompletedBus.QUALIFIED_XSD_TYPE_xEV);

			Bind<IXMLComponentReader>().To<XMLComponentReaderV24_HEV_PxHeavyLorry>()
				.Named(XMLComponentReaderV24_HEV_PxHeavyLorry.QUALIFIED_XSD_TYPE);

			Bind<IXMLComponentReader>().To<XMLComponentReaderV24_HEV_PxHeavyLorry>()
				.Named(XMLComponentReaderV24_HEV_PxHeavyLorry.QUALIFIED_XSD_HEV_Px_TYPE);

			Bind<IXMLComponentReader>().To<XMLPrimaryBusHEVPxDeclarationComponentReaderV201>()
				.Named(XMLPrimaryBusHEVPxDeclarationComponentReaderV201.QUALIFIED_XSD_TYPE);

			Bind<IXMLComponentReader>().To<XMLPrimaryBusHEVS2DeclarationComponentReaderV201>()
				.Named(XMLPrimaryBusHEVS2DeclarationComponentReaderV201.QUALIFIED_XSD_TYPE);

			Bind<IXMLComponentReader>().To<XMLPrimaryBusHEVS3DeclarationComponentReaderV201>()
				.Named(XMLPrimaryBusHEVS3DeclarationComponentReaderV201.QUALIFIED_XSD_TYPE);

			Bind<IXMLComponentReader>().To<XMLPrimaryBusHEVS4DeclarationComponentReaderV201>()
				.Named(XMLPrimaryBusHEVS4DeclarationComponentReaderV201.QUALIFIED_XSD_TYPE);

			Bind<IXMLComponentReader>().To<XMLHeavyLorryHEVS2DeclartionComponentReaderV201>().Named(XMLHeavyLorryHEVS2DeclartionComponentReaderV201.QUALIFIED_XSD_TYPE);
			
			Bind<IXMLComponentReader>().To<XMLHeavyLorryHEVS3DeclarationComponentReaderV201>().Named(XMLHeavyLorryHEVS3DeclarationComponentReaderV201.QUALIFIED_XSD_TYPE);

			Bind<IXMLComponentReader>().To<XMLHeavyLorryHEVS4DeclarationComponentReaderV201>().Named(XMLHeavyLorryHEVS4DeclarationComponentReaderV201.QUALIFIED_XSD_TYPE);
			
			Bind<IXMLComponentReader>().To<XMLHeavyLorryHEVIEPCSDeclarationComponentReaderV201>()
				.Named(XMLHeavyLorryHEVIEPCSDeclarationComponentReaderV201.QUALIFIED_XSD_TYPE);

			Bind<IXMLComponentReader>().To<XMLPrimaryBusHEVIEPCSDeclarationComponentReaderV201>()
				.Named(XMLPrimaryBusHEVIEPCSDeclarationComponentReaderV201.QUALIFIED_XSD_TYPE);

			Bind<IXMLComponentReader>().To<XMLPEVHeavyLorryE2DeclarationComponentReaderV201>()
				.Named(XMLPEVHeavyLorryE2DeclarationComponentReaderV201.QUALIFIED_XSD_TYPE);

			Bind<IXMLComponentReader>().To<XMLPEVHeavyLorryE3DeclarationComponentReaderV201>()
				.Named(XMLPEVHeavyLorryE3DeclarationComponentReaderV201.QUALIFIED_XSD_TYPE);

			Bind<IXMLComponentReader>().To<XMLPEVHeavyLorryE4DeclarationComponentReaderV201>()
				.Named(XMLPEVHeavyLorryE4DeclarationComponentReaderV201.QUALIFIED_XSD_TYPE);

			Bind<IXMLComponentReader>().To<XMLPEVPrimaryBusE2DeclarationComponentReaderV201>()
				.Named(XMLPEVPrimaryBusE2DeclarationComponentReaderV201.QUALIFIED_XSD_TYPE);

			Bind<IXMLComponentReader>().To<XMLPEVPrimaryBusE3DeclarationComponentReaderV201>()
				.Named(XMLPEVPrimaryBusE3DeclarationComponentReaderV201.QUALIFIED_XSD_TYPE);

			Bind<IXMLComponentReader>().To<XMLPEVPrimaryBusE4DeclarationComponentReaderV201>()
				.Named(XMLPEVPrimaryBusE4DeclarationComponentReaderV201.QUALIFIED_XSD_TYPE);

			Bind<IXMLComponentReader>().To<XMLIEPCHeavyLorryComponentReaderV201>()
				.Named(XMLIEPCHeavyLorryComponentReaderV201.QUALIFIED_XSD_TYPE);

			Bind<IXMLComponentReader>().To<XMLIEPCPrimaryBusComponentReaderV201>()
				.Named(XMLIEPCPrimaryBusComponentReaderV201.QUALIFIED_XSD_TYPE);

			Bind<IXMLGearboxReader>().To<XMLGearboxDeclarationComponentReaderV201>()
				.Named(XMLGearboxDeclarationComponentReaderV201.GEARBOX_READER_QUALIFIED_XSD_TYPE);

			Bind<IXMLAuxiliaryReader>().To<XMLAuxiliaryDeclarationComponentReaderV201>()
				.Named(XMLAuxiliaryDeclarationComponentReaderV201.AUXILIARIES_READER_QUALIFIED_XSD_TYPE);

			Bind<IXMLAuxiliaryReader>().To<XMLAuxiliaryDeclarationComponentReaderV201>()
				.Named(XMLAuxiliaryDeclarationComponentReaderV201.AUXILIARIES_READER_IEPC_PRIMARY_QUALIFIED_XSD_TYPE);

			Bind<IXMLElectricMachineSystemReader>().To<XMLElectricMachineSystemReaderV24>().Named(XMLElectricMachineSystemReaderV24.QUALIFIED_XSD_TYPE);
			Bind<IXMLElectricMachineSystemReader>().To<XMLElectricMachineSystemReaderV24>().Named(XMLElectricMachineSystemReaderV24.QUALIFIED_GEN_XSD_TYPE);

			Bind<IXMLREESSReader>().To<XMLREESSReaderV24>().Named(XMLREESSReaderV24.QUALIFIED_XSD_TYPE);

			Bind<IXMLPTOReader>().To<XMLPTOReaderV24>().Named(XMLPTOReaderV24.QUALIFIED_XSD_TYPE);

			Bind<IXMLBusAuxiliariesDeclarationData>().To<XMLDeclarationCompletedBusAuxiliariesDataProviderV24>()
				.Named(XMLDeclarationCompletedBusAuxiliariesDataProviderV24.QUALIFIED_XSD_TYPE_CONVENTIONAL);
			Bind<IXMLBusAuxiliariesDeclarationData>().To<XMLDeclarationCompletedBusAuxiliariesDataProviderV24>()
				.Named(XMLDeclarationCompletedBusAuxiliariesDataProviderV24.QUALIFIED_XSD_TYPE_xEV);

			Bind<IXMLAirdragDeclarationInputData>().To<XMLDeclarationAirdragDataProviderV24>().Named(
				XMLDeclarationAirdragDataProviderV24.QUALIFIED_XSD_TYPE);

			Bind<IXMLAuxiliariesDeclarationInputData>().To<XMLDeclarationAuxiliariesDataProviderV24_Lorry>().Named(
				XMLDeclarationAuxiliariesDataProviderV24_Lorry.QUALIFIED_XSD_TYPE);

			Bind<IXMLAuxiliariesDeclarationInputData>().To<XMLDeclarationAuxiliariesDataProviderV24_Lorry>().Named(
				XMLDeclarationAuxiliariesDataProviderV24_Lorry.QUALIFIED_XSD_HEV_P_TYPE);

			Bind<IXMLAuxiliariesDeclarationInputData>().To<XMLDeclarationAuxiliariesDataProviderV24_Lorry>().Named(
				XMLDeclarationAuxiliariesDataProviderV24_Lorry.QUALIFIED_XSD_HEV_S_TYPE);

			Bind<IXMLAuxiliariesDeclarationInputData>().To<XMLDeclarationAuxiliariesDataProviderV24_Lorry>().Named(
				XMLDeclarationAuxiliariesDataProviderV24_Lorry.QUALIFIED_XSD_PEV_TYPE);
			
			Bind<IXMLAuxiliariesDeclarationInputData>().To<XMLDeclarationAuxiliariesDataProviderV24_Lorry>().Named(
				XMLDeclarationAuxiliariesDataProviderV24_Lorry.QUALIFIED_XSD_IEPC_TYPE);

			Bind<IXMLAuxiliaryDeclarationInputData>().To<XMLAuxiliaryDeclarationDataProviderV24_Lorry>()
				.Named(XMLAuxiliaryDeclarationDataProviderV24_Lorry.QUALIFIED_XSD_TYPE);

			Bind<IXMLAuxiliaryDeclarationInputData>().To<XMLAuxiliaryDeclarationDataProviderV24_Lorry>()
				.Named(XMLAuxiliaryDeclarationDataProviderV24_Lorry.QUALIFIED_XSD_HEV_P_TYPE);
			
			Bind<IXMLAuxiliaryDeclarationInputData>().To<XMLAuxiliaryDeclarationDataProviderV24_Lorry>()
				.Named(XMLAuxiliaryDeclarationDataProviderV24_Lorry.QUALIFIED_XSD_HEV_S_TYPE);

			Bind<IXMLAuxiliaryDeclarationInputData>().To<XMLAuxiliaryDeclarationDataProviderV24_Lorry>()
				.Named(XMLAuxiliaryDeclarationDataProviderV24_Lorry.QUALIFIED_XSD_PEV_E2_TYPE);

			Bind<IXMLAuxiliaryDeclarationInputData>().To<XMLAuxiliaryDeclarationDataProviderV24_Lorry>()
				.Named(XMLAuxiliaryDeclarationDataProviderV24_Lorry.QUALIFIED_XSD_IEPC_TYPE);

			Bind<IXMLAdvancedDriverAssistantSystemDeclarationInputData>()
				.To<XMLDeclarationADASDataConventionalProviderV24>().Named(XMLDeclarationADASDataConventionalProviderV24.QUALIFIED_XSD_TYPE);
			Bind<IXMLAdvancedDriverAssistantSystemDeclarationInputData>()
				.To<XMLDeclarationADASDataHEVProviderV24>().Named(XMLDeclarationADASDataHEVProviderV24.QUALIFIED_XSD_TYPE);
			Bind<IXMLAdvancedDriverAssistantSystemDeclarationInputData>()
				.To<XMLDeclarationADASDataPEVProviderV24>().Named(XMLDeclarationADASDataPEVProviderV24.QUALIFIED_XSD_TYPE);
			Bind<IXMLAdvancedDriverAssistantSystemDeclarationInputData>()
				.To<XMLDeclarationADASDataIEPCProviderV24>().Named(XMLDeclarationADASDataIEPCProviderV24.QUALIFIED_XSD_TYPE);

			Bind<IXMLPTOTransmissionInputData>()
				.To<XMLDeclarationPTODataProviderV24>().Named(XMLDeclarationPTODataProviderV24.QUALIFIED_XSD_TYPE);

			Bind<IXMLDeclarationVehicleData>().To<XMLDeclarationConventionalPrimaryBusVehicleDataProviderV24>()
				.Named(XMLDeclarationConventionalPrimaryBusVehicleDataProviderV24.QUALIFIED_XSD_TYPE);

			Bind<IXMLDeclarationVehicleData>().To<XMLDeclarationExemptedHeavyLorryDataProviderV24>()
				.Named(XMLDeclarationExemptedHeavyLorryDataProviderV24.QUALIFIED_XSD_TYPE);

			Bind<IXMLDeclarationVehicleData>().To<XMLDeclarationExemptedMediumLorryDataProviderV24>()
				.Named(XMLDeclarationExemptedMediumLorryDataProviderV24.QUALIFIED_XSD_TYPE);

			Bind<IXMLDeclarationVehicleData>().To<XMLDeclarationExemptedPrimaryBusDataProviderV24>()
				.Named(XMLDeclarationExemptedPrimaryBusDataProviderV24.QUALIFIED_XSD_TYPE);


			Bind<IXMLVehicleComponentsDeclaration>().To<XMLDeclarationPrimaryBusComponentsDataProviderV24>()
				.Named(XMLDeclarationPrimaryBusComponentsDataProviderV24.QUALIFIED_XSD_TYPE);

			Bind<IXMLBusAuxiliariesDeclarationData>().To<XMLDeclarationPrimaryBusAuxiliariesConventionalDataProviderV24>()
				.Named(XMLDeclarationPrimaryBusAuxiliariesConventionalDataProviderV24.QUALIFIED_XSD_TYPE);

			Bind<IXMLBusAuxiliariesDeclarationData>().To<XMLDeclarationPrimaryBusAuxiliariesHEVPDataProviderV24>()
				.Named(XMLDeclarationPrimaryBusAuxiliariesHEVPDataProviderV24.QUALIFIED_XSD_TYPE);

			Bind<IXMLBusAuxiliariesDeclarationData>().To<XMLDeclarationPrimaryBusAuxiliariesHEVSDataProviderV24>()
				.Named(XMLDeclarationPrimaryBusAuxiliariesHEVSDataProviderV24.QUALIFIED_XSD_TYPE);

			Bind<IXMLBusAuxiliariesDeclarationData>().To<XMLDeclarationPrimaryBusAuxiliariesPEVDataProviderV24>()
				.Named(XMLDeclarationPrimaryBusAuxiliariesPEVDataProviderV24.QUALIFIED_XSD_TYPE);

			Bind<IXMLBusAuxiliariesDeclarationData>().To<XMLDeclarationPrimaryBusAuxiliariesIEPCDataProviderV24>()
				.Named(XMLDeclarationPrimaryBusAuxiliariesIEPCDataProviderV24.QUALIFIED_XSD_TYPE);

			Bind<IXMLDeclarationVehicleData>().To<XMLDeclarationConventionalMediumLorryVehicleDataProviderV24>()
				.Named(XMLDeclarationConventionalMediumLorryVehicleDataProviderV24.QUALIFIED_XSD_TYPE);

			Bind<IXMLElectricMachinesDeclarationInputData>().To<XMLElectricMachinesDeclarationInputDataProvider>()
				.Named(XMLElectricMachinesDeclarationInputDataProvider.QUALIFIED_XSD_TYPE);

			Bind<IXMLElectricMachinesDeclarationInputData>().To<XMLElectricMachinesDeclarationInputDataProvider>()
				.Named(XMLElectricMachinesDeclarationInputDataProvider.QUALIFIED_GEN_XSD_TYPE);

			Bind<IXMLElectricMotorDeclarationInputData>().To<XMLElectricMotorDeclarationInputDataProviderV23>()
				.Named(XMLElectricMotorDeclarationInputDataProviderV23.QUALIFIED_XSD_TYPE);
			Bind<IComponentInputData>().ToConstructor<XMLElectricMotorDeclarationInputDataProviderV23>((syntax) => new XMLElectricMotorDeclarationInputDataProviderV23(syntax.Inject<XmlNode>(), syntax.Inject<string>()))
				.Named(XMLElectricMotorDeclarationInputDataProviderV23.QUALIFIED_XSD_TYPE);

            Bind<IXMLElectricMotorDeclarationInputData>().To<XMLElectricMotorSystemStandardDeclarationInputDataProviderV23>()
				.Named(XMLElectricMotorSystemStandardDeclarationInputDataProviderV23.QUALIFIED_XSD_TYPE);
			
			Bind<IXMLElectricMotorDeclarationInputData>().To<XMLElectricMotorIhpcDeclarationInputDataProviderV23>()
				.Named(XMLElectricMotorIhpcDeclarationInputDataProviderV23.QUALIFIED_XSD_TYPE);

			Bind<IXMLIEPCInputData>().To<XMLElectricMotorIEPCIInputDataProviderV23>()
				.Named(XMLElectricMotorIEPCIInputDataProviderV23.QUALIFIED_XSD_TYPE);

			Bind<IXMLIEPCInputData>().To<XMLElectricMotorIEPCStandardInputDataProviderV23>()
				.Named(XMLElectricMotorIEPCStandardInputDataProviderV23.QUALIFIED_XSD_TYPE);
			
			Bind<IXMLElectricStorageSystemDeclarationInputData>().To<XMLElectricStorageSystemDeclarationInputDataV24>()
				.Named(XMLElectricStorageSystemDeclarationInputDataV24.QUALIFIED_XSD_TYPE);
			
			//Bind<IXMLBatteryPackDeclarationInputData>().To<XMLBatteryPackDeclarationDeclarationInputDataV24>()
			//	.Named(XMLBatteryPackDeclarationDeclarationInputDataV24.QUALIFIED_XSD_TYPE);

			Bind<IXMLBatteryPackDeclarationInputData>().To<XMLBatteryPackDeclarationInputDataMeasuredV23>()
				.Named(XMLBatteryPackDeclarationInputDataMeasuredV23.QUALIFIED_XSD_TYPE);
			Bind<IXMLBatteryPackDeclarationInputData>().To<XMLBatteryPackDeclarationInputDataStandardV23>()
				.Named(XMLBatteryPackDeclarationInputDataStandardV23.QUALIFIED_XSD_TYPE);

            Bind<IXMLSuperCapDeclarationInputData>().To<XMLSuperCapDeclarationInputDataV23>()
				.Named(XMLSuperCapDeclarationInputDataV23.QUALIFIED_XSD_TYPE);

			Bind<IXMLADCDeclarationInputData>().To<XMLADCDeclarationInputDataV23>()
				.Named(XMLADCDeclarationInputDataV23.QUALIFIED_XSD_TYPE);
			
			Bind<IXMLAuxiliaryReader>().To<XMLComponentReaderV24_Lorry>()
				.Named(XMLComponentReaderV24_Lorry.AUXILIARIES_READER_QUALIFIED_XSD_TYPE);

			Bind<IXMLAuxiliaryReader>().To<XMLComponentReaderV24_Lorry>()
				.Named(XMLComponentReaderV24_Lorry.AUXILIARIES_READER_HEV_P_QUALIFIED_XSD_TYPE);

			Bind<IXMLAuxiliaryReader>().To<XMLComponentReaderV24_Lorry>()
				.Named(XMLComponentReaderV24_Lorry.AUXILIARIES_READER_HEV_S2_QUALIFIED_XSD_TYPE);

			Bind<IXMLAuxiliaryReader>().To<XMLComponentReaderV24_Lorry>()
				.Named(XMLComponentReaderV24_Lorry.AUXILIARIES_READER_PEV_E2_QUALIFIED_XSD_TYPE);

			Bind<IXMLGearboxDeclarationInputData>().To<XMLDeclarationGearboxDataProviderV23>()
				.Named(XMLDeclarationGearboxDataProviderV23.QUALIFIED_XSD_TYPE);

			Bind<IXMLComponentReader>()
				.To<XMLComponentReaderV24_PrimaryBus>().Named(XMLComponentReaderV24_PrimaryBus.QUALIFIED_XSD_TYPE);

			Bind<IXMLADASReader>()
				.To<XMLADASReaderV24>().Named(XMLADASReaderV24.QUALIFIED_XSD_TYPE_CONVENTIONAL);
			Bind<IXMLADASReader>()
				.To<XMLADASReaderV24>().Named(XMLADASReaderV24.QUALIFIED_XSD_TYPE_HEV);
			Bind<IXMLADASReader>()
				.To<XMLADASReaderV24>().Named(XMLADASReaderV24.QUALIFIED_XSD_TYPE_PEV);
			Bind<IXMLADASReader>()
				.To<XMLADASReaderV24>().Named(XMLADASReaderV24.QUALIFIED_XSD_TYPE_IEPC);

		}

		#endregion


	}
}
