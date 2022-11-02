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
using TUGraz.VectoCore.InputData.FileIO.XML.Engineering.Interfaces;
using TUGraz.VectoCore.InputData.FileIO.XML.Engineering.Reader;

namespace TUGraz.VectoCore.InputData.FileIO.XML.Engineering.NinjectModules
{
	public class XMLEngineeringReaderTestOverrides : NinjectModule
	{
		#region Overrides of NinjectModule

		public override void Load()
		{
			// testing derived xml data types
			Bind<IXMLEngineData>().To<XMLEngineeringEngineDataProviderV10TEST>()
								.Named(XMLEngineeringEngineDataProviderV10TEST.QUALIFIED_XSD_TYPE);
			Bind<IXMLTyreData>().To<XMLTyreEngineeringDataProviderV10TEST>().Named(XMLTyreEngineeringDataProviderV10TEST.QUALIFIED_XSD_TYPE);

			Bind<IXMLAxleEngineeringData>().To<XMLAxleEngineeringDataV10TEST>()
											.Named(XMLAxleEngineeringDataV10TEST.QUALIFIED_XSD_TYPE);
			Bind<IXMLComponentsReader>().To<XMLComponentsEngineeringReaderV10TEST>()
										.Named(XMLComponentsEngineeringReaderV10TEST.QUALIFIED_XSD_TYPE_COMPONENTS);

			Bind<IXMLAxlesReader>().To<XMLComponentsEngineeringReaderV10TEST>()
									.Named(XMLComponentsEngineeringReaderV10TEST.QUALIFIED_XSD_TYPE_AXLES);
			Bind<IXMLAxleReader>().To<XMLComponentsEngineeringReaderV10TEST>()
								.Named(XMLComponentsEngineeringReaderV10TEST.QUALIFIED_XSD_TYPE_AXLE);
			Bind<IXMLGearboxReader>().To<XMLComponentsEngineeringReaderV10TEST>()
									.Named(XMLComponentsEngineeringReaderV10TEST.QUALIFIED_XSD_TYPE_GEARBOX);
			Bind<IXMLAuxiliaryReader>().To<XMLComponentsEngineeringReaderV10TEST>()
										.Named(XMLComponentsEngineeringReaderV10TEST.QUALIFIED_XSD_TYPE_AUXILIARY);

		}

		#endregion
	}
}
