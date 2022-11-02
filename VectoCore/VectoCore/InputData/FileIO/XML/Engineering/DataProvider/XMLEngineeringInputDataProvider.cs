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
using System.Xml;
using System.Xml.Linq;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCore.InputData.FileIO.XML.Common;
using TUGraz.VectoCore.InputData.FileIO.XML.Engineering.Interfaces;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.InputData.FileIO.XML.Engineering.DataProvider
{
	internal class XMLEngineeringInputDataProviderV07 : AbstractXMLResource, IXMLEngineeringInputData
	{
		public static readonly XNamespace NAMESPACE_URI = XMLDefinitions.ENGINEERING_DEFINITONS_NAMESPACE_V07;

		public const string XSD_TYPE = "VectoJobEngineeringType";

		public static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI, XSD_TYPE);

		internal XmlDocument Document;

		protected IEngineeringJobInputData JobData;
		protected IDriverEngineeringInputData DriverData;

		public IXMLEngineeringInputReader Reader { protected get; set; }

		public XMLEngineeringInputDataProviderV07(XmlDocument xmldoc, string fileName) : base(
			xmldoc.DocumentElement, fileName)
		{
			Document = xmldoc;

			// check for reference elements inside the vehicle or the vehicle is referenced itself
			var refNodes = Document.DocumentElement?.SelectNodes(
				String.Format("//*[local-name()='{0}']//*[local-name()='{1}' and @{2}]|/*/*[local-name()='{1}' and @{2}]", XMLNames.Component_Vehicle, XMLNames.ExternalResource, XMLNames.ExtResource_File_Attr));
			if (refNodes != null && refNodes.Count > 0 && fileName == null) {
				throw new VectoException("XML input data with file references can not be read via stream!");
			}

			SourceType = DataSourceType.XMLFile;
		}


		#region Implementation of IEngineeringInputDataProvider

		public virtual IEngineeringJobInputData JobInputData => JobData ?? (JobData = Reader.JobData);

		public virtual IDriverEngineeringInputData DriverInputData => DriverData ?? (DriverData = Reader.DriverModel);

		//public virtual IGearshiftEngineeringInputData GearshiftInputData { get { return DriverInputData.GearshiftInputData; } }

		#endregion




		#region Overrides of AbstractXMLResource

		protected override XNamespace SchemaNamespace => NAMESPACE_URI;
		protected override DataSourceType SourceType { get; }

		#endregion
	}

	internal class XMLEngineeringInputDataProviderV10 : XMLEngineeringInputDataProviderV07
	{
		public new static readonly XNamespace NAMESPACE_URI = XMLDefinitions.ENGINEERING_DEFINITONS_NAMESPACE_V10;

		public new const string XSD_TYPE = "VectoJobEngineeringType";

		public new static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI, XSD_TYPE);


		public XMLEngineeringInputDataProviderV10(XmlDocument xmldoc, string fileName) : base(xmldoc, fileName) { }

		protected override XNamespace SchemaNamespace => NAMESPACE_URI;
	}
}
