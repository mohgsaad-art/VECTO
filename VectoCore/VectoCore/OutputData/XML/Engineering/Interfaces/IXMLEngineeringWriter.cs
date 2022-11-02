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

using System.Xml.Linq;
using TUGraz.VectoCommon.InputData;

namespace TUGraz.VectoCore.OutputData.XML.Engineering.Interfaces {
	public interface IXMLEngineeringWriter
	{
		XNamespace RegisterNamespace(string namespaceUri);

		string GetNSPrefix(string xmlns);

		XDocument Write(IEngineeringInputDataProvider inputData);

		XDocument Write(IInputDataProvider inputData);

		XDocument WriteComponent<T>(T componentInputData) where T : class, IComponentInputData;

		WriterConfiguration Configuration { get; set; }

		string GetFilename<T>(T componentData, string suffix = null) where T:IComponentInputData;

		string RemoveInvalidFileCharacters(string filename);
	}

	public class WriterConfiguration
	{
		public bool SingleFile { get; set; }
		public string BasePath { get; set; }
	}
}