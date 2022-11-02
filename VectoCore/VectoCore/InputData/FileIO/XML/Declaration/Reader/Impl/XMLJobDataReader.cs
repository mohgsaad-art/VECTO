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
using Ninject;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.DataProvider;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Factory;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Interfaces;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Reader.Impl
{
	public class XMLJobDataReaderV10 : AbstractComponentReader, IXMLJobDataReader
	{
		public static readonly XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V10;

		public const string XSD_TYPE = "VectoDeclarationJobType";

		public static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);


		protected IXMLDeclarationJobInputData JobData;
		protected XmlNode JobNode;
		protected IVehicleDeclarationInputData _vehicle;

		[Inject]
		public IDeclarationInjectFactory Factory { protected get; set; }

		public XMLJobDataReaderV10(IXMLDeclarationJobInputData jobData, XmlNode jobNode) : base(
			jobData, jobNode)
		{
			JobNode = jobNode;
			JobData = jobData;
		}

		#region Implementation of IXMLJobDataReader

		public virtual IVehicleDeclarationInputData CreateVehicle => _vehicle ?? (_vehicle = CreateComponent(XMLNames.Component_Vehicle, VehicleCreator));

		#endregion

		protected virtual IVehicleDeclarationInputData VehicleCreator(string version, XmlNode vehicleNode, string sourceFile)
		{
			var vehicle = Factory.CreateVehicleData(version, JobData, vehicleNode, sourceFile);

			vehicle.ComponentReader = GetReader(vehicle, vehicle.ComponentNode, Factory.CreateComponentReader);  	
			vehicle.ADASReader =  GetReader(vehicle, vehicle.ADASNode, Factory.CreateADASReader);
			vehicle.PTOReader = GetReader(vehicle, vehicle.PTONode, Factory.CreatePTOReader); 

			return vehicle;
		}

	}

	// ---------------------------------------------------------------------------------------

	public class XMLJobDataReaderV20 : XMLJobDataReaderV10
	{
		public new static readonly XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V20;

		public new const string XSD_TYPE = "VectoDeclarationJobType";

		public new static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

		public XMLJobDataReaderV20(IXMLDeclarationJobInputData jobData, XmlNode jobNode) : base(
			jobData, jobNode) { }

		protected override IVehicleDeclarationInputData VehicleCreator(string version, XmlNode vehicleNode, string sourceFile)
		{
			var vehicle = Factory.CreateVehicleData(version, JobData, vehicleNode, sourceFile);

			vehicle.ComponentReader = GetReader(vehicle, vehicle.ComponentNode, Factory.CreateComponentReader);
			vehicle.ADASReader = vehicle.ADASNode == null ? null : GetReader(vehicle, vehicle.ADASNode, Factory.CreateADASReader); //null;
			vehicle.PTOReader = vehicle.PTONode == null ? null : GetReader(vehicle, vehicle.PTONode, Factory.CreatePTOReader);

			return vehicle;
		}
	}

	// ---------------------------------------------------------------------------------------

	public class XMLJobDataReaderV21 : XMLJobDataReaderV10
	{
		public new static readonly XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V21;

		public new const string XSD_TYPE = "VectoDeclarationJobType";

		public new static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

		public XMLJobDataReaderV21(IXMLDeclarationJobInputData jobData, XmlNode jobNode) : base(
			jobData, jobNode)
		{ }

		protected override IVehicleDeclarationInputData VehicleCreator(string version, XmlNode vehicleNode, string sourceFile)
		{
			var vehicle = Factory.CreateVehicleData(version, JobData, vehicleNode, sourceFile);

			vehicle.ComponentReader = GetReader(vehicle, vehicle.ComponentNode, Factory.CreateComponentReader);
			vehicle.ADASReader = GetReader(vehicle, vehicle.ADASNode, Factory.CreateADASReader);
			vehicle.PTOReader = GetReader(vehicle, vehicle.PTONode, Factory.CreatePTOReader);

			return vehicle;
		}
	}
	

	// ---------------------------------------------------------------------------------------
	
	public class XMLJobDataMultistagePrimaryVehicleReaderV01 : AbstractComponentReader, IXMLJobDataReader
	{
		public static readonly XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_MULTISTAGE_BUS_VEHICLE_NAMESPACE_VO1;

		public const string XSD_TYPE = "VehiclePIFType";

		public static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);


		[Inject]
		public IDeclarationInjectFactory Factory { protected get; set; }

		protected IVehicleDeclarationInputData _vehicle;

		private readonly IXMLPrimaryVehicleBusJobInputData _primaryBusJobData;

		public XMLJobDataMultistagePrimaryVehicleReaderV01(IXMLPrimaryVehicleBusJobInputData busJobData, XmlNode jobNode)
			: base(busJobData, jobNode)
		{
			_primaryBusJobData = busJobData;
		}

		public IVehicleDeclarationInputData CreateVehicle => _vehicle ?? (_vehicle = CreateComponent(XMLNames.Component_Vehicle, VehicleCreator));

		protected IVehicleDeclarationInputData VehicleCreator(string version, XmlNode vehicleNode, string sourceFile)
		{
			var vehicle = Factory.CreatePrimaryVehicleData(version, _primaryBusJobData, vehicleNode, sourceFile);
			vehicle.ComponentReader = GetReader(vehicle, vehicle.ComponentNode, Factory.CreateComponentReader);
			vehicle.ADASReader = GetReader(vehicle, vehicle.ADASNode, Factory.CreateADASReader);

			return vehicle;
		}
	}

	// ---------------------------------------------------------------------------------------

	public class XMLJobDataMultistageExemptedPrimaryVehicleReaderV01 : XMLJobDataMultistagePrimaryVehicleReaderV01
	{
		public new static readonly XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_MULTISTAGE_BUS_VEHICLE_NAMESPACE_VO1;

		public new const string XSD_TYPE = "VehicleExemptedPrimaryBusType";

		public new static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

		public XMLJobDataMultistageExemptedPrimaryVehicleReaderV01(IXMLPrimaryVehicleBusJobInputData busJobData, XmlNode jobNode) : base(busJobData, jobNode) { }
	}
}
