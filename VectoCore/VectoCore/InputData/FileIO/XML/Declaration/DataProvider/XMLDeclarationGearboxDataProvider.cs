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
using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Interfaces;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Reader;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.InputData.FileIO.XML.Declaration.DataProvider
{
	public class XMLDeclarationGearboxDataProviderV10 : AbstractCommonComponentType, IXMLGearboxDeclarationInputData
	{
		public static readonly XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V10;

		public const string XSD_TYPE = "GearboxDataDeclarationType";

		public static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);


		protected ITorqueConverterDeclarationInputData _torqueConverter;
		protected IList<ITransmissionInputData> _gears;
		protected IXMLDeclarationVehicleData _vehicle;

		public XMLDeclarationGearboxDataProviderV10(
			IXMLDeclarationVehicleData vehicle, XmlNode componentNode, string sourceFile) :
			base(componentNode, sourceFile)
		{
			SourceType = DataSourceType.XMLEmbedded;
			_vehicle = vehicle;
		}


		#region Overrides of AbstractXMLResource

		protected override XNamespace SchemaNamespace => NAMESPACE_URI;

		protected override DataSourceType SourceType { get; }

		#endregion

		#region Implementation of IGearboxDeclarationInputData

		public virtual GearboxType Type
		{
			get {
				var value = GetString(XMLNames.Gearbox_TransmissionType);
				switch (value) {
					case "MT":
					case "SMT": return GearboxType.MT;
					case "AMT": return GearboxType.AMT;
					case "APT-S":
					case "AT - Serial": return GearboxType.ATSerial;
					case "APT-N":
					case "APT-P":
					case "AT - PowerSplit": return GearboxType.ATPowerSplit;
				}

				throw new ArgumentOutOfRangeException("GearboxType", value);
			}
		}

		public virtual IList<ITransmissionInputData> Gears
		{
			get {
				if (_gears != null) {
					return _gears;
				}

				_gears = new List<ITransmissionInputData>();

				var gearNodes = GetNodes(new[] { XMLNames.Gearbox_Gears, XMLNames.Gearbox_Gears_Gear });
				if (gearNodes != null) {
					foreach (XmlNode gearNode in gearNodes) {
						_gears.Add(Reader.CreateGear(gearNode));
					}
				}

				return _gears;
			}
		}

		public virtual bool DifferentialIncluded => false;

		public virtual double AxlegearRatio => double.NaN;

		//public virtual ITorqueConverterDeclarationInputData TorqueConverter
		//{
		//	get { return _vehicle.Components.TorqueConverterInputData; }
		//}

		#endregion

		#region Implementation of IXMLGearboxDeclarationInputData

		public IXMLGearboxReader Reader { protected get; set; }

		#endregion
	}

	// ---------------------------------------------------------------------------------------

	public class XMLDeclarationGearboxDataProviderV20 : XMLDeclarationGearboxDataProviderV10
	{
		public new static readonly XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V20;

		public new static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

		public XMLDeclarationGearboxDataProviderV20(
			IXMLDeclarationVehicleData vehicle, XmlNode componentNode, string sourceFile) : base(
			vehicle, componentNode, sourceFile) { }

		protected override XNamespace SchemaNamespace => NAMESPACE_URI;
	}

	// ---------------------------------------------------------------------------------------

	//public class XMLDeclarationGearboxDataProviderV26 : XMLDeclarationGearboxDataProviderV10
	//{
	//	public new static readonly XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V210_JOBS;

	//	public new static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

	//	public XMLDeclarationGearboxDataProviderV26(
	//		IXMLDeclarationVehicleData vehicle, XmlNode componentNode, string sourceFile) : base(
	//		vehicle, componentNode, sourceFile)
	//	{ }

	//	#region Overrides of XMLDeclarationGearboxDataProviderV10

	//	public override bool DifferentialIncluded => GetBool(XMLNames.Gearbox_DifferentialIncluded);

	//	public override double AxlegearRatio => DifferentialIncluded ? GetDouble(XMLNames.Gearbox_AxlegearRatio) : double.NaN;

	//	#endregion
		
	//	protected override XNamespace SchemaNamespace => NAMESPACE_URI;
	//}
	
	// ---------------------------------------------------------------------------------------

	public class XMLDeclarationMultistagePrimaryVehicleBusGearboxDataProviderV01 : XMLDeclarationGearboxDataProviderV10
	{
		public new static readonly XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_MULTISTAGE_BUS_VEHICLE_NAMESPACE_VO1;

		public new const string XSD_TYPE = "TransmissionDataPIFType";

		public new static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

		public XMLDeclarationMultistagePrimaryVehicleBusGearboxDataProviderV01(IXMLDeclarationVehicleData vehicle,
			XmlNode componentNode, string sourceFile) : base(vehicle, componentNode, sourceFile) { }
		
		#region Overrides of AbstractCommonComponentType

		public override string CertificationNumber => GetString(XMLNames.Component_CertificationNumber, required: false);

		#endregion
		
		protected override XNamespace SchemaNamespace => NAMESPACE_URI;
	}

	// ---------------------------------------------------------------------------------------

	public class XMLDeclarationGearboxDataProviderV23 : XMLDeclarationGearboxDataProviderV10
	{
		public new static readonly XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V23;
		public new const string XSD_TYPE = "GearboxDataDeclarationType";
		public new static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

		public XMLDeclarationGearboxDataProviderV23(IXMLDeclarationVehicleData vehicle, XmlNode componentNode, string sourceFile) 
			: base(vehicle, componentNode, sourceFile) { }
	}
}
