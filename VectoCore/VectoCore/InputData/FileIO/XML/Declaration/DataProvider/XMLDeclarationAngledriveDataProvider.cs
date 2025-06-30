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
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Interfaces;
using TUGraz.VectoCore.OutputData.XML;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.InputData.FileIO.XML.Declaration.DataProvider
{
	public class XMLDeclarationAngledriveDataProviderV10 : AbstractCommonComponentType, IXMLAngledriveInputData
	{
		public static XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V10;

		public const string XSD_TYPE = "AngledriveDataDeclarationType";

		public static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

        public static readonly string AXLE_NUMBER_VERSION = $"{XSD_TYPE}:AxleNumberVersion";

        protected IXMLDeclarationVehicleData Vehicle;
		private int? _axleNumber;

		public XMLDeclarationAngledriveDataProviderV10(
			IXMLDeclarationVehicleData vehicle, XmlNode componentNode, string sourceFile) :
			base(componentNode, sourceFile)
		{
			SourceType = DataSourceType.XMLFile;
			Vehicle = vehicle;
		}

        public XMLDeclarationAngledriveDataProviderV10(
            int axleNumber, IXMLDeclarationVehicleData vehicle, XmlNode componentNode, string sourceFile) :
            base(componentNode, sourceFile)
        {
			_axleNumber = axleNumber;
            SourceType = DataSourceType.XMLFile;
            Vehicle = vehicle;
        }

        #region Implementation of IAngledriveInputData

        public virtual AngledriveType Type => Vehicle.GetAngledriveType(AxleNumber.Value);

		public virtual double Ratio => GetDouble(XMLNames.AngleDrive_Ratio);

        private int? AxleNumber => 
			_axleNumber ?? (_axleNumber = int.Parse(GetAttribute(BaseNode?.ParentNode, "axleNumber") ?? $"{Constants.NOT_IN_AXLE_POWERTRAIN}"));

        public virtual TableData LossMap =>
			ReadTableData(
				XMLNames.AngleDrive_TorqueLossMap, XMLNames.Angledrive_LossMap_Entry,
				AttributeMappings.TransmissionLossmapMapping);

		public virtual double Efficiency => throw new VectoException("Efficiency not supported in Declaration Mode!");

		#endregion

		#region Overrides of AbstractXMLResource

		protected override XNamespace SchemaNamespace => NAMESPACE_URI;

		protected override DataSourceType SourceType { get; }

		#endregion
	}

	// ---------------------------------------------------------------------------------------

	public class XMLDeclarationAngledriveDataProviderV20 : XMLDeclarationAngledriveDataProviderV10
	{
		public new static XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V20;

		//public new const string XSD_TYPE = "AngledriveComponentDeclarationType";

		public new static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

		static XMLDeclarationAngledriveDataProviderV20()
		{
			NAMESPACE_URI = XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V20;
		}

		public XMLDeclarationAngledriveDataProviderV20(
			IXMLDeclarationVehicleData vehicle, XmlNode componentNode, string sourceFile) : base(
			vehicle, componentNode, sourceFile) { }

		protected override XNamespace SchemaNamespace => NAMESPACE_URI;
	}

	// ---------------------------------------------------------------------------------------

	public class XMLDeclarationMultistagePrimaryVehicleBusAngledriveDataProviderV01 : XMLDeclarationAngledriveDataProviderV10
	{
		public new static XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_MULTISTAGE_BUS_VEHICLE_NAMESPACE_VO1;

		public new const string XSD_TYPE = "AngledriveDataVIFType";

		public new static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);
		
		public XMLDeclarationMultistagePrimaryVehicleBusAngledriveDataProviderV01(
			IXMLDeclarationVehicleData vehicle, XmlNode componentNode, string sourceFile)
			: base(vehicle, componentNode, sourceFile) { }

		#region Overrides of AbstractCommonComponentType

		public override string CertificationNumber => GetString(XMLNames.Component_CertificationNumber, required: false);

		#endregion


		protected override XNamespace SchemaNamespace => NAMESPACE_URI;
	}
}
