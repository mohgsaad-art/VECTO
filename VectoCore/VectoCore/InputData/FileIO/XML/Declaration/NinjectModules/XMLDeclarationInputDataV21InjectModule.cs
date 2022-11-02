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
	public class XMLDeclarationInputDataV21InjectModule : NinjectModule
	{
		#region Overrides of NinjectModule

		public override void Load()
		{
			Bind<IXMLDeclarationVehicleData>().To<XMLDeclarationVehicleDataProviderV21>().Named(
				XMLDeclarationVehicleDataProviderV21.QUALIFIED_XSD_TYPE);

			Bind<IXMLDeclarationVehicleData>().To<XMLDeclarationExemptedVehicleDataProviderV22>()
											.Named(XMLDeclarationExemptedVehicleDataProviderV22.QUALIFIED_XSD_TYPE);

			Bind<IXMLEngineDeclarationInputData>().To<XMLDeclarationEngineDataProviderV21>().Named(
				XMLDeclarationEngineDataProviderV21.QUALIFIED_XSD_TYPE);

			Bind<IXMLAdvancedDriverAssistantSystemDeclarationInputData>()
				.To<XMLDeclarationADASDataProviderV21>().Named(
					XMLDeclarationADASDataProviderV21.QUALIFIED_XSD_TYPE);

			// ---------------------------------------------------------------------------------------

			Bind<IXMLJobDataReader>().To<XMLJobDataReaderV21>()
									.Named(XMLJobDataReaderV21.QUALIFIED_XSD_TYPE);

			Bind<IXMLADASReader>().To<XMLADASReaderV21>()
								.Named(XMLADASReaderV21.QUALIFIED_XSD_TYPE);
		}

		#endregion
	}
}
