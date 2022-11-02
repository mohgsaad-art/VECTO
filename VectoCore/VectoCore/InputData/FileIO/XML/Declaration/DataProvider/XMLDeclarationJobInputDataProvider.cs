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
using TUGraz.VectoCore.InputData.FileIO.XML.Common;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Interfaces;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Reader;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.InputData.FileIO.XML.Declaration.DataProvider
{
	public class XMLDeclarationJobInputDataProviderV10 : AbstractXMLResource, IXMLDeclarationJobInputData
	{
		public static readonly XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V10;

		public const string XSD_TYPE = "VectoDeclarationJobType";

		public static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

		protected IVehicleDeclarationInputData _vehicle;

		public XMLDeclarationJobInputDataProviderV10(XmlNode node, IXMLDeclarationInputData inputProvider, string fileName) :
			base(node, fileName)
		{
			InputData = inputProvider;
		}

		#region Overrides of AbstractXMLResource

		protected override XNamespace SchemaNamespace => NAMESPACE_URI;

		protected override DataSourceType SourceType => DataSourceType.XMLFile;

		#endregion

		#region Implementation of IDeclarationJobInputData

		public virtual bool SavedInDeclarationMode => true;

		public virtual IVehicleDeclarationInputData Vehicle => _vehicle ?? (_vehicle = Reader.CreateVehicle);

		public virtual string JobName => Vehicle.Identifier;

		public VectoSimulationJobType JobType => VectoSimulationJobType.ConventionalVehicle;

		#endregion

		#region Implementation of IXMLDeclarationJobInputData

		public virtual IXMLJobDataReader Reader { protected get; set; }
		public virtual IXMLDeclarationInputData InputData { get; }

		#endregion
	}

	// ---------------------------------------------------------------------------------------

	public class XMLDeclarationJobInputDataProviderV20 : XMLDeclarationJobInputDataProviderV10
	{
		public new static readonly XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V20;

		public new const string XSD_TYPE = "VectoDeclarationJobType";

		public new static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

		public XMLDeclarationJobInputDataProviderV20(XmlNode node, IXMLDeclarationInputData inputProvider, string fileName) :
			base(node, inputProvider, fileName) { }

		protected override XNamespace SchemaNamespace => NAMESPACE_URI;
	}
	
	// ---------------------------------------------------------------------------------------

	public class XMLDeclarationMultistagePrimaryVehicleBusJobInputDataProviderV01 : AbstractXMLResource,
		IXMLPrimaryVehicleBusJobInputData
	{
		public static readonly XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_MULTISTAGE_BUS_VEHICLE_NAMESPACE_VO1;

		public const string XSD_TYPE = "VehiclePIFType";

		public static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

		protected IVehicleDeclarationInputData _vehicle;

		public XMLDeclarationMultistagePrimaryVehicleBusJobInputDataProviderV01(XmlNode node, IXMLPrimaryVehicleBusInputData inputProvider,
			string fileName) : base(node, fileName)
		{
			InputData = inputProvider;
		}
		protected override XNamespace SchemaNamespace => NAMESPACE_URI;

		public IVehicleDeclarationInputData Vehicle => _vehicle ?? (_vehicle = Reader.CreateVehicle);

		protected override DataSourceType SourceType { get; }
		public bool SavedInDeclarationMode { get; }
		public string JobName { get; }
		
		public VectoSimulationJobType JobType => VectoSimulationJobType.ConventionalVehicle;
		public IXMLJobDataReader Reader { protected get; set; }
		public IXMLPrimaryVehicleBusInputData InputData { get; }
	}

	// ---------------------------------------------------------------------------------------

	public class XMLDeclarationMultistageExemptedPrimaryVehicleBusJobInputDataProviderV01 :
			XMLDeclarationMultistagePrimaryVehicleBusJobInputDataProviderV01
	{
		public new static readonly XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_MULTISTAGE_BUS_VEHICLE_NAMESPACE_VO1;

		public new const string XSD_TYPE = "VehicleExemptedPrimaryBusType";

		public new static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);


		public XMLDeclarationMultistageExemptedPrimaryVehicleBusJobInputDataProviderV01(XmlNode node, IXMLPrimaryVehicleBusInputData inputProvider, string fileName) : base(node, inputProvider, fileName) { }
	}
}
