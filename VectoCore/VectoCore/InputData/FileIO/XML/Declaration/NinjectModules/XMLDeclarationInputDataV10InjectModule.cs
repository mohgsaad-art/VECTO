/*
* This file is part of VECTO.
*
* Copyright © 2012-2019 European Union
*
* Developed by Graz University of Technology,
*              Institute of Internal Combustion Engines and Thermodynamics,
*              Institute of Technical Informatics
*
* VECTO is licensed under the EUPL, Version 1.1 or - as soon they will be approved
* by the European Commission - subsequent versions of the EUPL (the "Licence");
* You may not use VECTO except in compliance with the Licence.
* You may obtain a copy of the Licence at:
*
* https://joinup.ec.europa.eu/community/eupl/og_page/eupl
*
* Unless required by applicable law or agreed to in writing, VECTO
* distributed under the Licence is distributed on an "AS IS" basis,
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
* See the Licence for the specific language governing permissions and
* limitations under the Licence.
*
* Authors:
*   Stefan Hausberger, hausberger@ivt.tugraz.at, IVT, Graz University of Technology
*   Christian Kreiner, christian.kreiner@tugraz.at, ITI, Graz University of Technology
*   Michael Krisper, michael.krisper@tugraz.at, ITI, Graz University of Technology
*   Raphael Luz, luz@ivt.tugraz.at, IVT, Graz University of Technology
*   Markus Quaritsch, markus.quaritsch@tugraz.at, IVT, Graz University of Technology
*   Martin Rexeis, rexeis@ivt.tugraz.at, IVT, Graz University of Technology
*/

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
