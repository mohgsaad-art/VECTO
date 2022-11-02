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
	public class XMLEngineeringReaderV10InjectModule : NinjectModule
	{
		#region Overrides of NinjectModule

		public override void Load()
		{
			Bind<IXMLEngineeringInputData>().To<XMLEngineeringInputDataProviderV10>()
											.Named(XMLEngineeringInputDataProviderV10.QUALIFIED_XSD_TYPE);
			Bind<IXMLEngineeringInputReader>().To<XMLEngineeringInputReaderV10>()
											.Named(XMLEngineeringInputReaderV10.QUALIFIED_XSD_TYPE);

			Bind<IXMLEngineeringJobInputData>().To<XMLEngineeringJobInputDataProviderV10>()
												.Named(XMLEngineeringJobInputDataProviderV10.QUALIFIED_XSD_TYPE);

			Bind<IXMLJobDataReader>().To<XMLJobDataReaderV10>()
									.Named(XMLJobDataReaderV10.QUALIFIED_XSD_TYPE);

			Bind<IXMLEngineeringDriverData>().To<XMLEngineeringDriverDataProviderV10>()
											.Named(XMLEngineeringDriverDataProviderV10.QUALIFIED_XSD_TYPE);

			Bind<IXMLDriverDataReader>().To<XMLDriverDataReaderV10>()
										.Named(XMLDriverDataReaderV10.QUALIFIED_XSD_TYPE);

			Bind<IXMLEngineeringGearshiftData>().To<XMLEngineeringGearshiftDataV10>()
												.Named(XMLEngineeringGearshiftDataV10.QUALIFIED_XSD_TYPE);

			Bind<IXMLCyclesDataProvider>().To<XMLCyclesDataProviderV10>()
										.Named(XMLCyclesDataProviderV10.QUALIFIED_XSD_TYPE);

			Bind<IXMLEngineeringVehicleData>().To<XMLEngineeringVehicleDataProviderV10>()
											.Named(XMLEngineeringVehicleDataProviderV10.QUALIFIED_XSD_TYPE);
			Bind<IXMLComponentsReader>().To<XMLComponentsEngineeringReaderV10>()
										.Named(XMLComponentsEngineeringReaderV10.QUALIFIED_XSD_TYPE_COMPONENTS);
			Bind<IXMLAxlesReader>().To<XMLComponentsEngineeringReaderV10>()
									.Named(XMLComponentsEngineeringReaderV10.QUALIFIED_XSD_TYPE_AXLES);
			Bind<IXMLAxleReader>().To<XMLComponentsEngineeringReaderV10>()
								.Named(XMLComponentsEngineeringReaderV10.QUALIFIED_XSD_TYPE_AXLE);
			Bind<IXMLGearboxReader>().To<XMLComponentsEngineeringReaderV10>()
									.Named(XMLComponentsEngineeringReaderV10.QUALIFIED_XSD_TYPE_GEARBOX);
			Bind<IXMLAuxiliaryReader>().To<XMLComponentsEngineeringReaderV10>()
										.Named(XMLComponentsEngineeringReaderV10.QUALIFIED_XSD_TYPE_AUXILIARY);


			Bind<IXMLEngineeringVehicleComponentsData>().To<XMLEngineeringVehicleComponentsDataProviderV10>()
														.Named(
															XMLEngineeringVehicleComponentsDataProviderV10.QUALIFIED_XSD_TYPE);

			Bind<IXMLAxleEngineeringData>().To<XMLAxleEngineeringDataV10>().Named(
				XMLAxleEngineeringDataV10.QUALIFIED_XSD_TYPE);
			Bind<IXMLGearData>().To<XMLGearDataV10>().Named(XMLGearDataV10.QUALIFIED_XSD_TYPE);
			Bind<IXMLAuxiliaryData>().To<XMLAuxiliaryEngineeringDataV10>().Named(
				XMLAuxiliaryEngineeringDataV10.QUALIFIED_XSD_TYPE);
			Bind<IXMLAxlegearData>().To<XMLEngineeringAxlegearDataProviderV10>()
									.Named(XMLEngineeringAxlegearDataProviderV10.QUALIFIED_XSD_TYPE);
			Bind<IXMLAngledriveData>().To<XMLEngineeringAngledriveDataProviderV10>()
									.Named(XMLEngineeringAngledriveDataProviderV10.QUALIFIED_XSD_TYPE);
			Bind<IXMLEngineData>().To<XMLEngineeringEngineDataProviderV10>()
								.Named(XMLEngineeringEngineDataProviderV10.QUALIFIED_XSD_TYPE);
			Bind<IXMLRetarderData>().To<XMLEngineeringRetarderDataProviderV10>()
									.Named(XMLEngineeringRetarderDataProviderV10.QUALIFIED_XSD_TYPE);
			Bind<IXMLAuxiliairesData>().To<XMLEngineeringAuxiliariesDataProviderV10>()
										.Named(XMLEngineeringAuxiliariesDataProviderV10.QUALIFIED_XSD_TYPE);
			Bind<IXMLGearboxData>().To<XMLEngineeringGearboxDataProviderV10>()
									.Named(XMLEngineeringGearboxDataProviderV10.QUALIFIED_XSD_TYPE);
			Bind<IXMLAirdragData>().To<XMLEngineeringAirdragDataProviderV10>()
									.Named(XMLEngineeringAirdragDataProviderV10.QUALIFIED_XSD_TYPE);
			Bind<IXMLTorqueconverterData>().To<XMLEngineeringTorqueConverterDataProviderV10>()
											.Named(XMLEngineeringTorqueConverterDataProviderV10.QUALIFIED_XSD_TYPE);
			Bind<IXMLAxlesData>().To<XMLEngineeringAxlesDataProviderV10>()
								.Named(XMLEngineeringAxlesDataProviderV10.QUALIFIED_XSD_TYPE);
			Bind<IXMLTyreData>().To<XMLTyreEngineeringDataProviderV10>().Named(
				XMLTyreEngineeringDataProviderV10.QUALIFIED_XSD_TYPE);

			Bind<IXMLLookaheadData>().To<XMLEngineeringDriverLookAheadV10>().Named(
				XMLEngineeringDriverLookAheadV10.QUALIFIED_XSD_TYPE);

			Bind<IXMLOverspeedData>().To<XMLEngineeringOverspeedV10>().Named(
				XMLEngineeringOverspeedV10.QUALIFIED_XSD_TYPE);

			Bind<IXMLDriverAcceleration>().To<XMLDriverAccelerationV10>()
										.Named(XMLDriverAccelerationV10.QUALIFIED_XSD_TYPE);

			Bind<IXMLEngineeringEngineStopStartData>().To<XMLEngineeringEngineStopStartDataProviderV10>()
													.Named(XMLEngineeringEngineStopStartDataProviderV10.QUALIFIED_XSD_TYPE);

			Bind<IXMLEngineeringEcoRollData>().To<XMLEngineeringEcoRollDataProviderV10>()
											.Named(XMLEngineeringEcoRollDataProviderV10.QUALIFIED_XSD_TYPE);
			Bind<IXMLEngineeringPCCInputData>().To<XMLEngineeringPCCInputDataProviderV10>()
												.Named(XMLEngineeringPCCInputDataProviderV10.QUALIFIED_XSD_TYPE);
		}

		#endregion
	}

	public class XMLEngineeringReaderV11InjectModule : NinjectModule
	{
		public override void Load()
		{
			Bind<IXMLEngineData>().To<XMLEngineeringEngineDataProviderV11>()
								.Named(XMLEngineeringEngineDataProviderV11.QUALIFIED_XSD_TYPE);

		}
	}
}
