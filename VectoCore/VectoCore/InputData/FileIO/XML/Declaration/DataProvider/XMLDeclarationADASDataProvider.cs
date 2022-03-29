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

using System.Xml;
using System.Xml.Linq;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Interfaces;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.InputData.FileIO.XML.Declaration.DataProvider
{
	public class XMLDeclarationADASDataProviderV10 : AbstractCommonComponentType,
		IXMLAdvancedDriverAssistantSystemDeclarationInputData
	{
		public static XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V10;

		public const string XSD_TYPE = "ADASType";

		public static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

		public XMLDeclarationADASDataProviderV10(IXMLDeclarationVehicleData vehicle, XmlNode componentNode, string sourceFile)
			: base(componentNode, sourceFile)
		{
			SourceType = DataSourceType.XMLFile;
		}

		#region Overrides of AbstractXMLResource

		protected override XNamespace SchemaNamespace
		{
			get { return NAMESPACE_URI; }
		}

		protected override DataSourceType SourceType { get; }

		#endregion

		#region Implementation of IAdvancedDriverAssistantSystemDeclarationInputData

		public virtual bool EngineStopStart
		{
			get { return XmlConvert.ToBoolean(GetString(XMLNames.Vehicle_ADAS_EngineStopStart)); }
		}

		public virtual EcoRollType EcoRoll
		{
			get {
				return EcorollTypeHelper.Get(
					XmlConvert.ToBoolean(GetString(XMLNames.Vehicle_ADAS_EcoRollWithoutEngineStop)),
					XmlConvert.ToBoolean(GetString(XMLNames.Vehicle_ADAS_EcoRollWithEngineStopStart)));
			}
		}

		public virtual PredictiveCruiseControlType PredictiveCruiseControl
		{
			get { return PredictiveCruiseControlTypeHelper.Parse(GetString(XMLNames.Vehicle_ADAS_PCC)); }
		}

		public virtual bool? ATEcoRollReleaseLockupClutch => null;
		#endregion
	}


	// ---------------------------------------------------------------------------------------

	public class XMLDeclarationADASDataProviderV21 : XMLDeclarationADASDataProviderV10
	{
		/*
		 * ADAS are added in version 2.1
		 */

		public new static readonly XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V21;

		public new const string XSD_TYPE = "AdvancedDriverAssistantSystemsType";

		public new static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

		public XMLDeclarationADASDataProviderV21(IXMLDeclarationVehicleData vehicle, XmlNode componentNode, string sourceFile)
			: base(vehicle, componentNode, sourceFile) { }

		protected override XNamespace SchemaNamespace
		{
			get { return NAMESPACE_URI; }
		}
	}
}
