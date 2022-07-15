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
using Castle.Components.DictionaryAdapter.Xml;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.FileIO.XML.Common;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Interfaces;
using TUGraz.VectoCore.Utils;
using TUGraz.VectoHashing;
using XmlDocumentType = System.Xml.XmlDocumentType;

namespace TUGraz.VectoCore.InputData.FileIO.XML.Declaration
{
	public class XMLDeclarationInputDataProviderV10 : AbstractXMLResource, IXMLDeclarationInputData
	{
		// "urn:tugraz:ivt:VectoAPI:DeclarationDefinitions:v1.0:VectoDeclarationJobType"
		public static readonly XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V10;

		public const string XSD_TYPE = "VectoDeclarationJobType";

		public static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

		protected readonly XmlDocument Document;
		protected IDeclarationJobInputData JobData;

		public XMLDeclarationInputDataProviderV10(XmlDocument xmlDoc, string fileName) : base(
			xmlDoc.DocumentElement, fileName)
		{
			Document = xmlDoc;
			SourceType = DataSourceType.XMLFile;

			var h = VectoHash.Load(xmlDoc);
			XMLHash = h.ComputeXmlHash();
		}

		#region Overrides of AbstractXMLResource

		protected override XNamespace SchemaNamespace
		{
			get { return NAMESPACE_URI; }
		}

		protected override DataSourceType SourceType { get; }

		#endregion

		#region Implementation of IXMLDeclarationInputData

		public virtual IXMLDeclarationInputDataReader Reader { protected get; set; }

		#endregion


		public virtual IDeclarationJobInputData JobInputData
		{
			get { return JobData ?? (JobData = Reader.JobData); }
		}

		public virtual IPrimaryVehicleInformationInputDataProvider PrimaryVehicleData { get { return null; } }


		public void ValidateComponentHashes()
		{
			VectoHash.ValidateJob(Document);
		}

		public virtual XElement XMLHash { get; private set; }
	}

	// ---------------------------------------------------------------------------------------

	public class XMLDeclarationInputDataProviderV20 : XMLDeclarationInputDataProviderV10
	{
		public new static XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V20;

		public new const string XSD_TYPE = "VectoDeclarationJobType";

		public new static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

		public XMLDeclarationInputDataProviderV20(XmlDocument xmlDoc, string fileName) : base(xmlDoc, fileName) { }


	}

	// ---------------------------------------------------------------------------------------

	public class XMLDeclarationInputDataProviderV27 :  AbstractXMLResource, IXMLDeclarationTrailerInputData
	{
		public static XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V20;

		public const string XSD_TYPE = "VectoDeclarationJobType";

		public static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

		protected readonly XmlDocument Document;

		protected IDeclarationTrailerJobInputData JobData;
		public XMLDeclarationInputDataProviderV27(XmlDocument xmlDoc, string fileName) : base(
			xmlDoc.DocumentElement, fileName)
		{
			Document = xmlDoc;
			SourceType = DataSourceType.XMLFile;

			var h = VectoHash.Load(xmlDoc);
			XMLHash = h.ComputeXmlHash();
		}
		protected override XNamespace SchemaNamespace { get { return NAMESPACE_URI; } }
		protected override DataSourceType SourceType { get; }
		public IDeclarationTrailerJobInputData JobInputData => JobData ?? (JobData = Reader.JobData);
		public void ValidateComponentHashes()
		{
			VectoHash.ValidateJob(Document);
		}

		public XElement XMLHash { get; private set; }
		public IXMLDeclarationTrailerInputDataReader Reader { protected get; set; }

	}

	// ---------------------------------------------------------------------------------------


	public class XMLPrimaryVehicleBusInputDataV01 : AbstractXMLResource, IXMLPrimaryVehicleBusInputData
	{
		public static readonly XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_PRIMARY_BUS_VEHICLE_NAMESPACE;

		public const string XSD_TYPE = "PrimaryVehicleHeavyBusType";

		public static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);
		
		protected readonly XmlDocument Document;
		protected IDeclarationJobInputData JobData;

		private IVehicleDeclarationInputData _vehicle;
		private IApplicationInformation _applicationInformation;
		private IResultsInputData _resultsInputData;
		

		public XMLPrimaryVehicleBusInputDataV01(XmlDocument xmlDoc, string fileName) : base(xmlDoc, fileName)
		{
			Document = xmlDoc;
			SourceType = DataSourceType.XMLFile;

			var h = VectoHash.Load(xmlDoc);
			XMLHash = h.ComputeXmlHash();
		}


		#region IPrimaryVehicleInputDataProvider interface

		public IVehicleDeclarationInputData Vehicle
		{
			get { return _vehicle ?? (_vehicle = Reader.JobData.Vehicle); }
		}

		public DigestData PrimaryVehicleInputDataHash
		{
			get { return Reader.GetDigestData(GetNode("InputDataSignature")); }
		}

		public DigestData ManufacturerRecordHash
		{
			get { return Reader.GetDigestData(GetNode("ManufacturerRecordSignature")); }
		}

		public IApplicationInformation ApplicationInformation
		{
			get { return _applicationInformation ?? (_applicationInformation = Reader.ApplicationInformation); }
		}

		public IResult GetResult(VehicleClass vehicleClass, MissionType mission, string fuelMode, Kilogram payload)
		{
			return ResultsInputData.Results.FirstOrDefault(
				x => x.VehicleGroup == vehicleClass &&
					(x.SimulationParameter.Payload - payload).IsEqual(0, 1) && x.Mission == mission &&
					x.SimulationParameter.FuelMode.Equals(fuelMode, StringComparison.InvariantCultureIgnoreCase));
		}

		public XElement XMLHash { get; }

		public IResultsInputData ResultsInputData
		{
			get { return _resultsInputData ?? (_resultsInputData = Reader.ResultsInputData); }
		}

		#endregion



		#region IXMLPrimaryVehicleBusInputData interface 

		public IXMLDeclarationPrimaryVehicleBusInputDataReader Reader { protected get; set; }

		public XmlNode ResultsNode
		{
			get { return GetNode(XMLNames.Report_Results); }
		}

		public XmlNode ApplicationInformationNode
		{
			get { return GetNode(XMLNames.Tag_ApplicationInformation); }
		}

		#endregion


		#region AbstractXMLResource class

		protected override XNamespace SchemaNamespace
		{
			get { return NAMESPACE_URI; }
		}

		public override DataSource DataSource
		{
			get { return new DataSource { SourceFile = SourceFile, SourceVersion = "", SourceType = SourceType }; }
		}

		protected override DataSourceType SourceType { get; }

		#endregion

		public virtual IDeclarationJobInputData JobInputData
		{
			get { return JobData ?? (JobData = Reader.JobData); }
		}
		
	}





}
