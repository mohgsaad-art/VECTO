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

using System;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCore.InputData.FileIO.XML.Common;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Interfaces;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.InputData.FileIO.XML.Declaration.DataProvider
{
	public class XMLDeclarationPTODataProviderV10 : AbstractXMLType, IXMLPTOTransmissionInputData
	{
		public static readonly XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V10;

		public const string XSD_TYPE = "PTOType";

		public static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

		public XMLDeclarationPTODataProviderV10(
			IXMLDeclarationVehicleData vehicle, XmlNode componentNode, string sourceFile) : base(componentNode) { }


		#region Implementation of IPTOTransmissionInputData

		public virtual string PTOTransmissionType
		{
			get {
				var shaftGearWheels = GetString(XMLNames.Vehicle_PTO_ShaftsGearWheels);
				if ("none".Equals(shaftGearWheels, StringComparison.InvariantCultureIgnoreCase)) {
					return "None";
				}
				if ("only one engaged gearwheel above oil level".Equals(
					shaftGearWheels, StringComparison.CurrentCultureIgnoreCase)) {
					return "only one engaged gearwheel above oil level";
				}

				var otherElements = GetString(XMLNames.Vehicle_PTO_OtherElements);
				var ptoTech = $"{shaftGearWheels} - {otherElements}";
				if (DeclarationData.PTOTransmission.GetTechnologies().Contains(ptoTech)) {
					return ptoTech;
				}

				throw new VectoException("PTO Technology {0} invalid!", ptoTech);
			}
		}

		public virtual TableData PTOLossMap => null;

		public virtual TableData PTOCycleDuringStop => null;

		public TableData PTOCycleWhileDriving => null;
		
		#endregion
	}

	// ---------------------------------------------------------------------------------------

	public class XMLDeclarationPTODataProviderV20 : XMLDeclarationPTODataProviderV10
	{
		public new static readonly XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V20;

		public new const string XSD_TYPE = "PTOType";

		public new static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);
		
		public XMLDeclarationPTODataProviderV20(IXMLDeclarationVehicleData vehicle, XmlNode componentNode, string sourceFile)
			: base(vehicle, componentNode, sourceFile) { }

	}

	// ---------------------------------------------------------------------------------------

	public class XMLDeclarationPTODataProviderV24 : XMLDeclarationPTODataProviderV10
	{
		public new static readonly XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V24;

		public new const string XSD_TYPE = "PTOType";

		public new static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

		public XMLDeclarationPTODataProviderV24(IXMLDeclarationVehicleData vehicle, XmlNode componentNode, string sourceFile)
			: base(vehicle, componentNode, sourceFile) { }


	}
}
