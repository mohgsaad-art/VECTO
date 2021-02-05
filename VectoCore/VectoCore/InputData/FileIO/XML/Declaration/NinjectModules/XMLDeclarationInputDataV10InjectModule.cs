using Ninject.Modules;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.DataProvider;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Interfaces;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Reader;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Reader.Impl;

namespace TUGraz.VectoCore.InputData.FileIO.XML.Declaration.NinjectModules
{
	public class XMLDeclarationInputDataV10InjectModule : NinjectModule
	{
		#region Overrides of NinjectModule

		public override void Load()
		{
			Bind<IXMLDeclarationInputData>().To<XMLDeclarationInputDataProviderV10>().Named(
				XMLDeclarationInputDataProviderV10.QUALIFIED_XSD_TYPE);

			Bind<IXMLDeclarationJobInputData>().To<XMLDeclarationJobInputDataProviderV10>().Named(
				XMLDeclarationJobInputDataProviderV10.QUALIFIED_XSD_TYPE);

			Bind<IXMLDeclarationVehicleData>().To<XMLDeclarationVehicleDataProviderV10>().Named(
				XMLDeclarationVehicleDataProviderV10.QUALIFIED_XSD_TYPE);

			Bind<IXMLVehicleComponentsDeclaration>().To<XMLDeclarationComponentsDataProviderV10>().Named(
				XMLDeclarationComponentsDataProviderV10.QUALIFIED_XSD_TYPE);

			Bind<IXMLAirdragDeclarationInputData>().To<XMLDeclarationAirdragDataProviderV10>().Named(
				XMLDeclarationAirdragDataProviderV10.QUALIFIED_XSD_TYPE);

			Bind<IXMLAngledriveInputData>().To<XMLDeclarationAngledriveDataProviderV10>().Named(
				XMLDeclarationAngledriveDataProviderV10.QUALIFIED_XSD_TYPE);

			Bind<IXMLAxleGearInputData>().To<XMLDeclarationAxlegearDataProviderV10>().Named(
				XMLDeclarationAxlegearDataProviderV10.QUALIFIED_XSD_TYPE);

			Bind<IXMLEngineDeclarationInputData>().To<XMLDeclarationEngineDataProviderV10>().Named(
				XMLDeclarationEngineDataProviderV10.QUALIFIED_XSD_TYPE);

			Bind<IXMLRetarderInputData>().To<XMLDeclarationRetarderDataProviderV10>().Named(
				XMLDeclarationRetarderDataProviderV10.QUALIFIED_XSD_TYPE);

			Bind<IXMLGearboxDeclarationInputData>().To<XMLDeclarationGearboxDataProviderV10>()
				.Named(XMLDeclarationGearboxDataProviderV10.QUALIFIED_XSD_TYPE);

			Bind<IXMLGearData>().To<XMLGearDataV10>().Named(XMLGearDataV10.QUALIFIED_XSD_TYPE);

			Bind<IXMLTorqueConverterDeclarationInputData>().To<XMLDeclarationTorqueConverterDataProviderV10>().Named(
				XMLDeclarationTorqueConverterDataProviderV10.QUALIFIED_XSD_TYPE);

			Bind<IXMLAxlesDeclarationInputData>().To<XMLDeclarationAxlesDataProviderV10>().Named(
				XMLDeclarationAxlesDataProviderV10.QUALIFIED_XSD_TYPE);

			Bind<IXMLAxleDeclarationInputData>().To<XMLDeclarationAxleDataProviderV10>().Named(
				XMLDeclarationAxleDataProviderV10.QUALIFIED_XSD_TYPE);

			Bind<IXMLTyreDeclarationInputData>().To<XMLDeclarationTyreDataProviderV10>().Named(
				XMLDeclarationTyreDataProviderV10.QUALIFIED_XSD_TYPE);

			Bind<IXMLAuxiliariesDeclarationInputData>().To<XMLDeclarationAuxiliariesDataProviderV10>().Named(
				XMLDeclarationAuxiliariesDataProviderV10.QUALIFIED_XSD_TYPE);

			Bind<IXMLAuxiliaryDeclarationInputData>().To<XMLAuxiliaryDeclarationDataProviderV10>().Named(
				XMLAuxiliaryDeclarationDataProviderV10.QUALIFIED_XSD_TYPE);

			Bind<IXMLPTOTransmissionInputData>().To<XMLDeclarationPTODataProviderV10>().Named(
				XMLDeclarationPTODataProviderV10.QUALIFIED_XSD_TYPE);

			Bind<IXMLAdvancedDriverAssistantSystemDeclarationInputData>()
				.To<XMLDeclarationADASDataProviderV10>().Named(
					XMLDeclarationADASDataProviderV10.QUALIFIED_XSD_TYPE);

            Bind<IXMLComponentInputData>().To<XMLDeclarationComponentInputDataProviderV10>().Named(
                XMLDeclarationComponentInputDataProviderV10.QUALIFIED_XSD_TYPE);

            // ---------------------------------------------------------------------------------------

            Bind<IXMLDeclarationInputDataReader>().To<XMLDeclarationInputReaderV10>()
												.Named(XMLDeclarationInputReaderV10.QUALIFIED_XSD_TYPE);

			Bind<IXMLJobDataReader>().To<XMLJobDataReaderV10>()
									.Named(XMLJobDataReaderV10.QUALIFIED_XSD_TYPE);

			Bind<IXMLComponentReader>().To<XMLComponentReaderV10>()
										.Named(XMLComponentReaderV10.QUALIFIED_XSD_TYPE);

			Bind<IXMLGearboxReader>().To<XMLComponentReaderV10>().Named(XMLComponentReaderV10.GEARBOX_READER_QUALIFIED_XSD_TYPE);

			Bind<IXMLPTOReader>().To<XMLPTOReaderV10>()
								.Named(XMLPTOReaderV10.QUALIFIED_XSD_TYPE);

			Bind<IXMLADASReader>().To<XMLADASReaderV10>()
								.Named(XMLADASReaderV10.QUALIFIED_XSD_TYPE);

			Bind<IXMLAxlesReader>().To<XMLComponentReaderV10>().Named(XMLComponentReaderV10.AXLES_READER_QUALIFIED_XSD_TYPE);

			Bind<IXMLAxleReader>().To<XMLComponentReaderV10>().Named(XMLComponentReaderV10.AXLE_READER_QUALIFIED_XSD_TYPE);

			Bind<IXMLAuxiliaryReader>().To<XMLComponentReaderV10>()
										.Named(XMLComponentReaderV10.AUXILIARIES_READER_QUALIFIED_XSD_TYPE);

		}

		#endregion
	}
}
