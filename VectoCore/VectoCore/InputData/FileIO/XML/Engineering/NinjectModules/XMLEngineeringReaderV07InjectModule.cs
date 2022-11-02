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
using TUGraz.VectoCore.InputData.FileIO.XML.Engineering.DataProvider;
using TUGraz.VectoCore.InputData.FileIO.XML.Engineering.Impl;
using TUGraz.VectoCore.InputData.FileIO.XML.Engineering.Interfaces;
using TUGraz.VectoCore.InputData.FileIO.XML.Engineering.Reader;
using TUGraz.VectoCore.InputData.FileIO.XML.Engineering.Reader.Impl;

namespace TUGraz.VectoCore.InputData.FileIO.XML.Engineering.NinjectModules
{
	public class XMLEngineeringReaderV07InjectModule : NinjectModule
	{
		#region Overrides of NinjectModule

		public override void Load()
		{
			Bind<IXMLEngineeringInputData>().To<XMLEngineeringInputDataProviderV07>()
											.Named(XMLEngineeringInputDataProviderV07.QUALIFIED_XSD_TYPE);
			Bind<IXMLEngineeringInputReader>().To<XMLEngineeringInputReaderV07>()
											.Named(XMLEngineeringInputReaderV07.QUALIFIED_XSD_TYPE);

			Bind<IXMLEngineeringJobInputData>().To<XMLEngineeringJobInputDataProviderV07>()
												.Named(XMLEngineeringJobInputDataProviderV07.QUALIFIED_XSD_TYPE);

			Bind<IXMLJobDataReader>().To<XMLJobDataReaderV07>()
									.Named(XMLJobDataReaderV07.QUALIFIED_XSD_TYPE);

			Bind<IXMLEngineeringDriverData>().To<XMLEngineeringDriverDataProviderV07>()
											.Named(XMLEngineeringDriverDataProviderV07.QUALIFIED_XSD_TYPE);

			Bind<IXMLDriverDataReader>().To<XMLDriverDataReaderV07>()
										.Named(XMLDriverDataReaderV07.QUALIFIED_XSD_TYPE);

			Bind<IXMLEngineeringGearshiftData>().To<XMLEngineeringGearshiftDataV07>()
												.Named(XMLEngineeringGearshiftDataV07.QUALIFIED_XSD_TYPE);

			Bind<IXMLCyclesDataProvider>().To<XMLCyclesDataProviderV07>()
										.Named(XMLCyclesDataProviderV07.QUALIFIED_XSD_TYPE);

			Bind<IXMLEngineeringVehicleData>().To<XMLEngineeringVehicleDataProviderV07>()
											.Named(XMLEngineeringVehicleDataProviderV07.QUALIFIED_XSD_TYPE);
			Bind<IXMLComponentsReader>().To<XMLComponentsEngineeringReaderV07>()
										.Named(XMLComponentsEngineeringReaderV07.QUALIFIED_XSD_TYPE_COMPONENTS);

			Bind<IXMLAxlesReader>().To<XMLComponentsEngineeringReaderV07>()
										.Named(XMLComponentsEngineeringReaderV07.QUALIFIED_XSD_TYPE_AXLES);
			Bind<IXMLAxleReader>().To<XMLComponentsEngineeringReaderV07>()
										.Named(XMLComponentsEngineeringReaderV07.QUALIFIED_XSD_TYPE_AXLE);
			Bind<IXMLGearboxReader>().To<XMLComponentsEngineeringReaderV07>()
										.Named(XMLComponentsEngineeringReaderV07.QUALIFIED_XSD_TYPE_GEARBOX);
			Bind<IXMLAuxiliaryReader>().To<XMLComponentsEngineeringReaderV07>()
										.Named(XMLComponentsEngineeringReaderV07.QUALIFIED_XSD_TYPE_AUXILIARY);


			Bind<IXMLEngineeringVehicleComponentsData>().To<XMLEngineeringVehicleComponentsDataProviderV07>()
														.Named(
															XMLEngineeringVehicleComponentsDataProviderV07.QUALIFIED_XSD_TYPE);

			Bind<IXMLAxleEngineeringData>().To<XMLAxleEngineeringDataV07>().Named(
				XMLAxleEngineeringDataV07.QUALIFIED_XSD_TYPE);
			Bind<IXMLGearData>().To<XMLGearDataV07>().Named(XMLGearDataV07.QUALIFIED_XSD_TYPE);
			Bind<IXMLAuxiliaryData>().To<XMLAuxiliaryEngineeringDataV07>().Named(
				XMLAuxiliaryEngineeringDataV07.QUALIFIED_XSD_TYPE);
			Bind<IXMLAxlegearData>().To<XMLEngineeringAxlegearDataProviderV07>()
									.Named(XMLEngineeringAxlegearDataProviderV07.QUALIFIED_XSD_TYPE);
			Bind<IXMLAngledriveData>().To<XMLEngineeringAngledriveDataProviderV07>()
									.Named(XMLEngineeringAngledriveDataProviderV07.QUALIFIED_XSD_TYPE);
			Bind<IXMLEngineData>().To<XMLEngineeringEngineDataProviderV07>()
								.Named(XMLEngineeringEngineDataProviderV07.QUALIFIED_XSD_TYPE);
			Bind<IXMLRetarderData>().To<XMLEngineeringRetarderDataProviderV07>()
									.Named(XMLEngineeringRetarderDataProviderV07.QUALIFIED_XSD_TYPE);
			Bind<IXMLAuxiliairesData>().To<XMLEngineeringAuxiliariesDataProviderV07>()
										.Named(XMLEngineeringAuxiliariesDataProviderV07.QUALIFIED_XSD_TYPE);
			Bind<IXMLGearboxData>().To<XMLEngineeringGearboxDataProviderV07>()
									.Named(XMLEngineeringGearboxDataProviderV07.QUALIFIED_XSD_TYPE);
			Bind<IXMLAirdragData>().To<XMLEngineeringAirdragDataProviderV07>()
									.Named(XMLEngineeringAirdragDataProviderV07.QUALIFIED_XSD_TYPE);
			Bind<IXMLTorqueconverterData>().To<XMLEngineeringTorqueConverterDataProviderV07>()
											.Named(XMLEngineeringTorqueConverterDataProviderV07.QUALIFIED_XSD_TYPE);
			Bind<IXMLAxlesData>().To<XMLEngineeringAxlesDataProviderV07>()
								.Named(XMLEngineeringAxlesDataProviderV07.QUALIFIED_XSD_TYPE);

			Bind<IXMLLookaheadData>().To<XMLEngineeringDriverLookAheadV07>().Named(
				XMLEngineeringDriverLookAheadV07.QUALIFIED_XSD_TYPE);

			Bind<IXMLOverspeedData>().To<XMLEngineeringOverspeedV07>().Named(
				XMLEngineeringOverspeedV07.QUALIFIED_XSD_TYPE);

			Bind<IXMLDriverAcceleration>().To<XMLDriverAccelerationV07>()
										.Named(XMLDriverAccelerationV07.QUALIFIED_XSD_TYPE);
		}

		#endregion
	}
}
