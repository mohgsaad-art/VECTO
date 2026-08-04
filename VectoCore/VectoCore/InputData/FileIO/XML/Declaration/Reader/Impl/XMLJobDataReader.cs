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

using System.Linq;
using System.Xml;
using System.Xml.Linq;
using Ninject;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Resources;
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

		public XMLJobDataReaderV10(IXMLDeclarationJobInputData jobData, XmlNode jobNode, bool allowDeprecated) : base(
			jobData, jobNode)
		{
			JobNode = jobNode;
			JobData = jobData;
			AllowDeprecated = allowDeprecated;
		}

		#region Implementation of IXMLJobDataReader

		public bool AllowDeprecated { get; protected set; }

		public virtual IVehicleDeclarationInputData CreateVehicle => _vehicle ?? (_vehicle = CreateComponent(XMLNames.Component_Vehicle, VehicleCreator));

		#endregion

		protected virtual IVehicleDeclarationInputData VehicleCreator(string version, XmlNode vehicleNode, string sourceFile)
		{
			var vehicle = Factory.CreateVehicleData(version, JobData, vehicleNode, sourceFile, AllowDeprecated);

			vehicle.ComponentReader = GetReader(vehicle, vehicle.ComponentNode, Factory.CreateComponentReader);
			vehicle.ComponentReader = GetReader(vehicle, vehicle.ComponentNode, Factory.CreateComponentReader);
			vehicle.ADASReader =  GetReader(vehicle, vehicle.ADASNode, Factory.CreateADASReader);
			vehicle.PTOReader = GetReader(vehicle, vehicle.PTONode, Factory.CreatePTOReader);
			vehicle.PTOReader = GetReader(vehicle, vehicle.PTONode, Factory.CreatePTOReader);

			return vehicle;
		}

		protected void CheckH2Properties(IVehicleDeclarationInputData vehicle)
		{
            if (vehicle.Components?.EngineInputData?.EngineModes.Any(x => x.Fuels.Any(y => y.FuelType.IsHydrogenFuel())) ?? false)
            {
                if (vehicle.H2StorageUsableCapacity == null)
                {
                    throw new VectoException("Vehicle with hydrogen-powered engine must declare H2StorageUsableCapacity.");
                }

				if (vehicle.HydrogenStorageTechnology == null)
				{
                    throw new VectoException("Vehicle with hydrogen-powered engine must declare HydrogenStorageTechnology.");
                }
            }
        }
    }

	// ---------------------------------------------------------------------------------------

	public class XMLJobDataReaderV20 : XMLJobDataReaderV10
	{
		public new static readonly XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V20;

		public new const string XSD_TYPE = "VectoDeclarationJobType";

		public new static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

		public XMLJobDataReaderV20(IXMLDeclarationJobInputData jobData, XmlNode jobNode, bool allowDeprecated) : base(
			jobData, jobNode, allowDeprecated) { }

		protected override IVehicleDeclarationInputData VehicleCreator(string version, XmlNode vehicleNode, string sourceFile)
		{
			var vehicle = Factory.CreateVehicleData(version, JobData, vehicleNode, sourceFile, AllowDeprecated);


			vehicle.ComponentReader = GetReader(vehicle, vehicle.ComponentNode, Factory.CreateComponentReader);
			vehicle.ADASReader = vehicle.ADASNode == null ? null : GetReader(vehicle, vehicle.ADASNode, Factory.CreateADASReader); //null;
			vehicle.PTOReader = vehicle.PTONode == null ? null : GetReader(vehicle, vehicle.PTONode, Factory.CreatePTOReader);
            vehicle.MonitoringReader = (vehicle.MonitoringNode == null) ? null : GetReader(vehicle, vehicle.MonitoringNode, Factory.CreateMonitoringReader);

			CheckH2Properties(vehicle);

            return vehicle;
		}
	}

	// ---------------------------------------------------------------------------------------

	public class XMLJobDataReaderV21 : XMLJobDataReaderV10
	{
		public new static readonly XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V21;

		public new const string XSD_TYPE = "VectoDeclarationJobType";

		public new static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

		public XMLJobDataReaderV21(IXMLDeclarationJobInputData jobData, XmlNode jobNode, bool allowDeprecated) : base(
			jobData, jobNode, allowDeprecated)
		{ }

		protected override IVehicleDeclarationInputData VehicleCreator(string version, XmlNode vehicleNode, string sourceFile)
		{
			var vehicle = Factory.CreateVehicleData(version, JobData, vehicleNode, sourceFile, AllowDeprecated);

			vehicle.ComponentReader = GetReader(vehicle, vehicle.ComponentNode, Factory.CreateComponentReader);
			vehicle.ADASReader = GetReader(vehicle, vehicle.ADASNode, Factory.CreateADASReader);
			vehicle.PTOReader = GetReader(vehicle, vehicle.PTONode, Factory.CreatePTOReader);

            CheckH2Properties(vehicle);

            return vehicle;
		}
	}


	// ---------------------------------------------------------------------------------------

	public class XMLJobDataReaderV27 : XMLJobDataReaderV10
	{
		public new static readonly XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V27;

		public new const string XSD_TYPE = "VectoDeclarationJobType";

		public new static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

		public XMLJobDataReaderV27(IXMLDeclarationJobInputData jobData, XmlNode jobNode, bool allowDeprecated) : base(
			jobData, jobNode, allowDeprecated)
		{ }

		protected override IVehicleDeclarationInputData VehicleCreator(string version, XmlNode vehicleNode, string sourceFile)
		{
			var vehicle = Factory.CreateVehicleData(version, JobData, vehicleNode, sourceFile, AllowDeprecated);

			vehicle.ComponentReader = GetReader(vehicle, vehicle.ComponentNode, Factory.CreateComponentReader);
			vehicle.ADASReader = vehicle.ADASNode == null ? null : GetReader(vehicle, vehicle.ADASNode, Factory.CreateADASReader); //null;
			vehicle.PTOReader = vehicle.PTONode == null ? null : GetReader(vehicle, vehicle.PTONode, Factory.CreatePTOReader);
			vehicle.MonitoringReader = (vehicle.MonitoringNode == null) ? null : GetReader(vehicle, vehicle.MonitoringNode, Factory.CreateMonitoringReader);

			CheckH2Properties(vehicle);

            return vehicle;
		}
    }

	// ---------------------------------------------------------------------------------------


	public class XMLJobDataMultistage_Conventional_PrimaryVehicleReaderV01 : AbstractComponentReader, IXMLJobDataReader
	{
		public static readonly XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_MULTISTAGE_BUS_VEHICLE_NAMESPACE_VO1;

		public const string XSD_TYPE = "ConventionalVehicleVIFType";

		public static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);


		[Inject]
		public IDeclarationInjectFactory Factory { protected get; set; }

		protected IVehicleDeclarationInputData _vehicle;

		private readonly IXMLPrimaryVehicleBusJobInputData _primaryBusJobData;

		public XMLJobDataMultistage_Conventional_PrimaryVehicleReaderV01(IXMLPrimaryVehicleBusJobInputData busJobData, XmlNode jobNode)
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

            CheckH2Properties(vehicle);

            return vehicle;
		}

        protected void CheckH2Properties(IVehicleDeclarationInputData vehicle)
        {
            if (vehicle.Components?.EngineInputData?.EngineModes.Any(x => x.Fuels.Any(y => y.FuelType.IsHydrogenFuel())) ?? false)
            {
                if (vehicle.H2StorageUsableCapacity == null)
                {
                    throw new VectoException("Vehicle with hydrogen-powered engine must declare H2StorageUsableCapacity.");
                }

                if (vehicle.HydrogenStorageTechnology == null)
                {
                    throw new VectoException("Vehicle with hydrogen-powered engine must declare HydrogenStorageTechnology.");
                }
            }
        }
    }

    public class XMLJobDataMultistage_Conventional_PrimaryVehicleReaderV11 : XMLJobDataMultistage_Conventional_PrimaryVehicleReaderV01
	{
        public new static readonly XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_MULTISTAGE_BUS_VEHICLE_NAMESPACE_V11;

        public new static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

		public XMLJobDataMultistage_Conventional_PrimaryVehicleReaderV11(IXMLPrimaryVehicleBusJobInputData busJobData, XmlNode jobNode)
			: base(busJobData, jobNode)
		{ }
    }

    // ---------------------------------------------------------------------------------------

    public class XMLJobDataMultistage_HEV_Px_PrimaryVehicleReaderV01 : XMLJobDataMultistage_Conventional_PrimaryVehicleReaderV01
	{
		public new static readonly XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_MULTISTAGE_BUS_VEHICLE_NAMESPACE_VO1;

		public new const string XSD_TYPE = "HEV-Px_VehicleVIFType";

		public new static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

		public XMLJobDataMultistage_HEV_Px_PrimaryVehicleReaderV01(IXMLPrimaryVehicleBusJobInputData busJobData, XmlNode jobNode)
			: base(busJobData, jobNode)
		{ }
	}

    public class XMLJobDataMultistage_HEV_Px_PrimaryVehicleReaderV11 : XMLJobDataMultistage_HEV_Px_PrimaryVehicleReaderV01
	{
        public new static readonly XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_MULTISTAGE_BUS_VEHICLE_NAMESPACE_V11;

        public new static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

        public XMLJobDataMultistage_HEV_Px_PrimaryVehicleReaderV11(IXMLPrimaryVehicleBusJobInputData busJobData, XmlNode jobNode)
            : base(busJobData, jobNode)
        { }
    }

    // ---------------------------------------------------------------------------------------

    public class XMLJobDataMultistage_HEV_Sx_PrimaryVehicleReaderV01 : XMLJobDataMultistage_Conventional_PrimaryVehicleReaderV01
	{
		public new static readonly XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_MULTISTAGE_BUS_VEHICLE_NAMESPACE_VO1;

		public new const string XSD_TYPE = "HEV-Sx_VehicleVIFType";

		public new static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

		public XMLJobDataMultistage_HEV_Sx_PrimaryVehicleReaderV01(IXMLPrimaryVehicleBusJobInputData busJobData, XmlNode jobNode)
			: base(busJobData, jobNode)
		{ }
	}

	public class XMLJobDataMultistage_HEV_Sx_PrimaryVehicleReaderV11 : XMLJobDataMultistage_HEV_Sx_PrimaryVehicleReaderV01
	{
        public new static readonly XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_MULTISTAGE_BUS_VEHICLE_NAMESPACE_V11;

        public new static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

        public XMLJobDataMultistage_HEV_Sx_PrimaryVehicleReaderV11(IXMLPrimaryVehicleBusJobInputData busJobData, XmlNode jobNode)
            : base(busJobData, jobNode)
        { }
    }

	// ---------------------------------------------------------------------------------------

	public class XMLJobDataMultistage_HEV_IEPC_S_PrimaryVehicleReaderV01 : XMLJobDataMultistage_Conventional_PrimaryVehicleReaderV01
	{
		public new static readonly XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_MULTISTAGE_BUS_VEHICLE_NAMESPACE_VO1;

		public new const string XSD_TYPE = "HEV-IEPC-S_VehicleVIFType";

		public new static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

		public XMLJobDataMultistage_HEV_IEPC_S_PrimaryVehicleReaderV01(IXMLPrimaryVehicleBusJobInputData busJobData, XmlNode jobNode)
			: base(busJobData, jobNode)
		{ }
	}

	public class XMLJobDataMultistage_HEV_IEPC_S_PrimaryVehicleReaderV11 : XMLJobDataMultistage_HEV_IEPC_S_PrimaryVehicleReaderV01
	{
        public new static readonly XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_MULTISTAGE_BUS_VEHICLE_NAMESPACE_V11;

        public new static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

        public XMLJobDataMultistage_HEV_IEPC_S_PrimaryVehicleReaderV11(IXMLPrimaryVehicleBusJobInputData busJobData, XmlNode jobNode)
            : base(busJobData, jobNode)
        { }
    }

    // ---------------------------------------------------------------------------------------

    public class XMLJobDataMultistage_PEV_Ex_PrimaryVehicleReaderV01 : XMLJobDataMultistage_Conventional_PrimaryVehicleReaderV01
	{
		public new static readonly XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_MULTISTAGE_BUS_VEHICLE_NAMESPACE_VO1;

		public new const string XSD_TYPE = "PEV_Ex_VehicleVIFType";

		public new static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

		public XMLJobDataMultistage_PEV_Ex_PrimaryVehicleReaderV01(IXMLPrimaryVehicleBusJobInputData busJobData, XmlNode jobNode)
			: base(busJobData, jobNode)
		{ }
	}

	public class XMLJobDataMultistage_PEV_Ex_PrimaryVehicleReaderV11 : XMLJobDataMultistage_PEV_Ex_PrimaryVehicleReaderV01
	{
        public new static readonly XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_MULTISTAGE_BUS_VEHICLE_NAMESPACE_V11;

        public new static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

        public XMLJobDataMultistage_PEV_Ex_PrimaryVehicleReaderV11(IXMLPrimaryVehicleBusJobInputData busJobData, XmlNode jobNode)
            : base(busJobData, jobNode)
        { }
    }

	// ---------------------------------------------------------------------------------------

	public class XMLJobDataMultistage_PEV_IEPC_PrimaryVehicleReaderV01 : XMLJobDataMultistage_Conventional_PrimaryVehicleReaderV01
	{
		public new static readonly XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_MULTISTAGE_BUS_VEHICLE_NAMESPACE_VO1;

		public new const string XSD_TYPE = "IEPC_VehicleVIFType";

		public new static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

		public XMLJobDataMultistage_PEV_IEPC_PrimaryVehicleReaderV01(IXMLPrimaryVehicleBusJobInputData busJobData, XmlNode jobNode)
			: base(busJobData, jobNode)
		{ }
	}

	public class XMLJobDataMultistage_PEV_IEPC_PrimaryVehicleReaderV11 : XMLJobDataMultistage_PEV_IEPC_PrimaryVehicleReaderV01
	{
        public new static readonly XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_MULTISTAGE_BUS_VEHICLE_NAMESPACE_V11;

        public new static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

		public XMLJobDataMultistage_PEV_IEPC_PrimaryVehicleReaderV11(IXMLPrimaryVehicleBusJobInputData busJobData, XmlNode jobNode)
			: base(busJobData, jobNode)
		{ }
    }

    // ---------------------------------------------------------------------------------------

    public class XMLJobDataMultistage_FCHV_Fx_PrimaryVehicleReaderV11 : XMLJobDataMultistage_Conventional_PrimaryVehicleReaderV01
    {
        public new static readonly XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_MULTISTAGE_BUS_VEHICLE_NAMESPACE_V11;

        public new const string XSD_TYPE = "FCHV_Fx_VehicleVIFType";

        public new static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

        public XMLJobDataMultistage_FCHV_Fx_PrimaryVehicleReaderV11(IXMLPrimaryVehicleBusJobInputData busJobData, XmlNode jobNode)
            : base(busJobData, jobNode)
        { }
    }

    public class XMLJobDataMultistage_FCHV_IEPC_PrimaryVehicleReaderV11 : XMLJobDataMultistage_Conventional_PrimaryVehicleReaderV01
    {
        public new static readonly XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_MULTISTAGE_BUS_VEHICLE_NAMESPACE_V11;

        public new const string XSD_TYPE = "FCHV_IEPC_VehicleVIFType";

        public new static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

        public XMLJobDataMultistage_FCHV_IEPC_PrimaryVehicleReaderV11(IXMLPrimaryVehicleBusJobInputData busJobData, XmlNode jobNode)
            : base(busJobData, jobNode)
        { }
    }

	// ---------------------------------------------------------------------------------------

    public class XMLJobDataMultistage_Multiple_FCHV_PrimaryVehicleReaderV11 : XMLJobDataMultistage_Conventional_PrimaryVehicleReaderV01
	{
        public new static readonly XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_MULTISTAGE_BUS_VEHICLE_NAMESPACE_V11;

        public new const string XSD_TYPE = "Multiple_FCHV_VehicleType";

        public new static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

        public XMLJobDataMultistage_Multiple_FCHV_PrimaryVehicleReaderV11(IXMLPrimaryVehicleBusJobInputData busJobData, XmlNode jobNode)
            : base(busJobData, jobNode)
        { }
    }

	public class XMLJobDataMultistage_Multiple_PEV_PrimaryVehicleReaderV11 : XMLJobDataMultistage_Conventional_PrimaryVehicleReaderV01
	{
        public new static readonly XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_MULTISTAGE_BUS_VEHICLE_NAMESPACE_V11;

        public new const string XSD_TYPE = "Multiple_PEV_VehicleType";

        public new static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

        public XMLJobDataMultistage_Multiple_PEV_PrimaryVehicleReaderV11(IXMLPrimaryVehicleBusJobInputData busJobData, XmlNode jobNode)
            : base(busJobData, jobNode)
        { }
    }

	public class XMLJobDataMultistage_Multiple_SHEV_PrimaryVehicleReaderV11 : XMLJobDataMultistage_Conventional_PrimaryVehicleReaderV01
    {
        public new static readonly XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_MULTISTAGE_BUS_VEHICLE_NAMESPACE_V11;

        public new const string XSD_TYPE = "Multiple_SHEV_VehicleType";

        public new static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

        public XMLJobDataMultistage_Multiple_SHEV_PrimaryVehicleReaderV11(IXMLPrimaryVehicleBusJobInputData busJobData, XmlNode jobNode)
            : base(busJobData, jobNode)
        { }
    }

    // ---------------------------------------------------------------------------------------

    public class XMLJobDataMultistageExemptedPrimaryVehicleReaderV01 : XMLJobDataMultistage_Conventional_PrimaryVehicleReaderV01
	{
		public new static readonly XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_MULTISTAGE_BUS_VEHICLE_NAMESPACE_VO1;

		public new const string XSD_TYPE = "Exempted_VehicleVIFType";

		public new static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

		public XMLJobDataMultistageExemptedPrimaryVehicleReaderV01(IXMLPrimaryVehicleBusJobInputData busJobData, XmlNode jobNode) : base(busJobData, jobNode) { }
	}

	public class XMLJobDataMultistageExemptedPrimaryVehicleReaderV11 : XMLJobDataMultistageExemptedPrimaryVehicleReaderV01
	{
        public new static readonly XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_MULTISTAGE_BUS_VEHICLE_NAMESPACE_V11;

        public new static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

        public XMLJobDataMultistageExemptedPrimaryVehicleReaderV11(IXMLPrimaryVehicleBusJobInputData busJobData, XmlNode jobNode) : base(busJobData, jobNode) { }
    }

}
