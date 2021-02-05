using Ninject.Modules;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.DataProvider;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Interfaces;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Reader;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Reader.Impl;

namespace TUGraz.VectoCore.InputData.FileIO.XML.Declaration.NinjectModules
{
	public class XMLDeclarationInputDataV20InjectModule : NinjectModule
	{
		#region Overrides of NinjectModule

		public override void Load()
		{
			Bind<IXMLDeclarationInputData>().To<XMLDeclarationInputDataProviderV20>().Named(
				XMLDeclarationInputDataProviderV20.QUALIFIED_XSD_TYPE);

			Bind<IXMLDeclarationJobInputData>().To<XMLDeclarationJobInputDataProviderV20>().Named(
				XMLDeclarationJobInputDataProviderV20.QUALIFIED_XSD_TYPE);

			Bind<IXMLDeclarationVehicleData>().To<XMLDeclarationVehicleDataProviderV20>().Named(
				XMLDeclarationVehicleDataProviderV20.QUALIFIED_XSD_TYPE);

			Bind<IXMLVehicleComponentsDeclaration>().To<XMLDeclarationComponentsDataProviderV20>().Named(
				XMLDeclarationComponentsDataProviderV20.QUALIFIED_XSD_TYPE);

			Bind<IXMLAirdragDeclarationInputData>().To<XMLDeclarationAirdragDataProviderV20>().Named(
				XMLDeclarationAirdragDataProviderV20.QUALIFIED_XSD_TYPE);

			Bind<IXMLAngledriveInputData>().To<XMLDeclarationAngledriveDataProviderV20>().Named(
				XMLDeclarationAngledriveDataProviderV20.QUALIFIED_XSD_TYPE);

			Bind<IXMLAxleGearInputData>().To<XMLDeclarationAxlegearDataProviderV20>().Named(
				XMLDeclarationAxlegearDataProviderV20.QUALIFIED_XSD_TYPE);

			Bind<IXMLEngineDeclarationInputData>().To<XMLDeclarationEngineDataProviderV20>().Named(
				XMLDeclarationEngineDataProviderV20.QUALIFIED_XSD_TYPE);

			Bind<IXMLRetarderInputData>().To<XMLDeclarationRetarderDataProviderV20>().Named(
				XMLDeclarationRetarderDataProviderV20.QUALIFIED_XSD_TYPE);

			Bind<IXMLGearboxDeclarationInputData>().To<XMLDeclarationGearboxDataProviderV20>()
													.Named(XMLDeclarationGearboxDataProviderV20.QUALIFIED_XSD_TYPE);

			Bind<IXMLGearData>().To<XMLGearDataV20>().Named(XMLGearDataV20.QUALIFIED_XSD_TYPE);

			Bind<IXMLTorqueConverterDeclarationInputData>().To<XMLDeclarationTorqueConverterDataProviderV20>().Named(
				XMLDeclarationTorqueConverterDataProviderV20.QUALIFIED_XSD_TYPE);

			Bind<IXMLAxlesDeclarationInputData>().To<XMLDeclarationAxlesDataProviderV20>().Named(
				XMLDeclarationAxlesDataProviderV20.QUALIFIED_XSD_TYPE);

			Bind<IXMLAxleDeclarationInputData>().To<XMLDeclarationAxleDataProviderV20>().Named(
				XMLDeclarationAxleDataProviderV20.QUALIFIED_XSD_TYPE);

			Bind<IXMLTyreDeclarationInputData>().To<XMLDeclarationTyreDataProviderV20>().Named(
				XMLDeclarationTyreDataProviderV20.QUALIFIED_XSD_TYPE);

			Bind<IXMLAuxiliariesDeclarationInputData>().To<XMLDeclarationAuxiliariesDataProviderV20>().Named(
				XMLDeclarationAuxiliariesDataProviderV20.QUALIFIED_XSD_TYPE);

			Bind<IXMLAuxiliaryDeclarationInputData>().To<XMLAuxiliaryDeclarationDataProviderV20>().Named(
				XMLAuxiliaryDeclarationDataProviderV20.QUALIFIED_XSD_TYPE);

			Bind<IXMLPTOTransmissionInputData>().To<XMLDeclarationPTODataProviderV20>().Named(
				XMLDeclarationPTODataProviderV20.QUALIFIED_XSD_TYPE);


            Bind<IXMLComponentInputData>().To<XMLDeclarationComponentInputDataProviderV20>().Named(
                XMLDeclarationComponentInputDataProviderV20.QUALIFIED_XSD_TYPE);

            // ---------------------------------------------------------------------------------------

            Bind<IXMLDeclarationInputDataReader>().To<XMLDeclarationInputReaderV20>()
												.Named(XMLDeclarationInputReaderV20.QUALIFIED_XSD_TYPE);

			Bind<IXMLJobDataReader>().To<XMLJobDataReaderV20>()
									.Named(XMLJobDataReaderV20.QUALIFIED_XSD_TYPE);

			Bind<IXMLComponentReader>().To<XMLComponentReaderV20>()
										.Named(XMLComponentReaderV20.QUALIFIED_XSD_TYPE);

			Bind<IXMLPTOReader>().To<XMLPTOReaderV20>()
								.Named(XMLPTOReaderV20.QUALIFIED_XSD_TYPE);

			Bind<IXMLADASReader>().To<XMLADASReaderV20>()
								.Named(XMLADASReaderV20.QUALIFIED_XSD_TYPE);


			Bind<IXMLGearboxReader>().To<XMLComponentReaderV20>().Named(XMLComponentReaderV20.GEARBOX_READER_QUALIFIED_XSD_TYPE);

			Bind<IXMLAxlesReader>().To<XMLComponentReaderV20>().Named(XMLComponentReaderV20.AXLES_READER_QUALIFIED_XSD_TYPE);

			Bind<IXMLAxleReader>().To<XMLComponentReaderV20>().Named(XMLComponentReaderV20.AXLE_READER_QUALIFIED_XSD_TYPE);

			Bind<IXMLAuxiliaryReader>().To<XMLComponentReaderV20>()
										.Named(XMLComponentReaderV20.AUXILIARIES_READER_QUALIFIED_XSD_TYPE);
		}

		#endregion
	}
}
