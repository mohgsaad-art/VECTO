using Ninject.Modules;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.DataProvider;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Interfaces;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Reader;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Reader.Impl;

namespace TUGraz.VectoCore.InputData.FileIO.XML.Declaration.NinjectModules
{
	public class XMLDeclarationInputDataV27InjectModule : NinjectModule
	{
		#region Overrides of NinjectModule

		public override void Load()
		{
			Bind<IXMLDeclarationTrailerInputData>().To<XMLDeclarationInputDataProviderV27>().Named(
				XMLDeclarationInputDataProviderV27.QUALIFIED_XSD_TYPE);

			Bind<IXMLDeclarationTrailerJobInputData>().To<XMLDeclarationJobInputDataProviderV27>().Named(
				XMLDeclarationJobInputDataProviderV27.QUALIFIED_XSD_TYPE);

			Bind<IXMLTrailerDeclarationVehicleData>().To<XMLDeclarationVehicleDataProviderV27>().Named(
				XMLDeclarationVehicleDataProviderV27.QUALIFIED_XSD_TYPE);

			Bind<IXMLAxlesDeclarationInputData>().To<XMLDeclarationAxlesDataProviderV27>().Named(
				XMLDeclarationAxlesDataProviderV27.QUALIFIED_XSD_TYPE);

			Bind<IXMLAxleDeclarationInputData>().To<XMLDeclarationAxleDataProviderV27>().Named(
				XMLDeclarationAxleDataProviderV27.QUALIFIED_XSD_TYPE);

			Bind<ITransmissionInputData>().To<XMLGearDataV10>().Named(
				XMLGearDataV10.QUALIFIED_XSD_TYPE);


			// ---------------------------------------------------------------------------------------

			Bind<IXMLDeclarationTrailerInputDataReader>().To<XMLDeclarationInputReaderV27>()
				.Named(XMLDeclarationInputReaderV27.QUALIFIED_XSD_TYPE);

			Bind<IXMLJobTrailerDataReader>().To<XMLJobDataReaderV27>()
				.Named(XMLJobDataReaderV27.QUALIFIED_XSD_TYPE);

			Bind<IXMLAxlesReader>().To<XMLComponentReaderV27>()
				.Named(XMLComponentReaderV27.AXLES_READER_QUALIFIED_XSD_TYPE);

			Bind<IXMLAxleReader>().To<XMLComponentReaderV27>()
				.Named(XMLComponentReaderV27.AXLE_READER_QUALIFIED_XSD_TYPE);

		}

		#endregion
	}
}