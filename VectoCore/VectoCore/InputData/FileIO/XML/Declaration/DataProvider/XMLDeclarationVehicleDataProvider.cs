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
using TUGraz.VectoCommon.BusAuxiliaries;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Interfaces;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Reader;
using TUGraz.VectoCore.InputData.Impl;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.InputData.FileIO.XML.Declaration.DataProvider
{
	public class XMLDeclarationVehicleDataProviderV10 : AbstractCommonComponentType, IXMLDeclarationVehicleData
	{
		public static readonly XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V10;

		public const string XSD_TYPE = "VehicleDeclarationType";

		public static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

		protected IVehicleComponentsDeclaration _components;
		protected IPTOTransmissionInputData _ptoData;
		protected XmlElement _componentNode;
		protected XmlElement _ptoNode;
		protected XmlElement _adasNode;


		public XMLDeclarationVehicleDataProviderV10(IXMLDeclarationJobInputData jobData, XmlNode xmlNode, string sourceFile)
			: base(xmlNode, sourceFile)
		{
			Job = jobData;
			SourceType = DataSourceType.XMLEmbedded;
		}


		public virtual XmlElement ComponentNode
		{
			get {
				if (ExemptedVehicle) {
					return null;
				}

				return _componentNode ?? (_componentNode = GetNode(XMLNames.Vehicle_Components) as XmlElement);
			}
		}

		public virtual IXMLComponentReader ComponentReader { protected get; set; }

		public virtual XmlElement PTONode
		{
			get {
				if (ExemptedVehicle) {
					return null;
				}

				return _ptoNode ?? (_ptoNode = GetNode(XMLNames.Vehicle_PTO) as XmlElement);
			}
		}

		public virtual IXMLPTOReader PTOReader { protected get; set; }

		public virtual XmlElement ADASNode => _adasNode ?? (_adasNode = GetNode(XMLNames.Vehicle_ADAS, required: false) as XmlElement);

		public virtual IXMLADASReader ADASReader { protected get; set; }

		public virtual IXMLDeclarationJobInputData Job { get; }

		public virtual string Identifier => GetAttribute(BaseNode, XMLNames.Component_ID_Attr);

		public virtual bool ExemptedVehicle => ElementExists(XMLNames.Vehicle_HybridElectricHDV) && ElementExists(XMLNames.Vehicle_DualFuelVehicle);

		public virtual string VIN => GetString(XMLNames.Vehicle_VIN);

		public virtual LegislativeClass? LegislativeClass => GetString(XMLNames.Vehicle_LegislativeClass).ParseEnum<LegislativeClass>();
			//get { return GetString("LegislativeCategory").ParseEnum<LegislativeClass>(); }

		public virtual VehicleCategory VehicleCategory
		{
			get {
				var val = GetString(XMLNames.Vehicle_VehicleCategory);
				if ("Rigid Lorry".Equals(val, StringComparison.InvariantCultureIgnoreCase)) {
					return VehicleCategory.RigidTruck;
				}

				return val.ParseEnum<VehicleCategory>();
			}
		}

		public virtual Kilogram CurbMassChassis => GetDouble(XMLNames.Vehicle_CurbMassChassis).SI<Kilogram>();


		public virtual Kilogram GrossVehicleMassRating => GetDouble(XMLNames.Vehicle_GrossVehicleMass).SI<Kilogram>();
			//get { return GetDouble(XMLNames.Vehicle_TPMLM).SI<Kilogram>(); }

		public virtual IList<ITorqueLimitInputData> TorqueLimits
		{
			get {
				var retVal = new List<ITorqueLimitInputData>();
				var limits = GetNodes(new[] { XMLNames.Vehicle_TorqueLimits, XMLNames.Vehicle_TorqueLimits_Entry });
				foreach (XmlNode current in limits) {
					if (current.Attributes != null) {
						retVal.Add(
							new TorqueLimitInputData() {
								Gear = GetAttribute(current, XMLNames.Vehicle_TorqueLimits_Entry_Gear_Attr).ToInt(),
								MaxTorque = GetAttribute(current, XMLNames.Vehicle_TorqueLimits_Entry_MaxTorque_Attr)
									.ToDouble().SI<NewtonMeter>()
							});
					}
				}

				return retVal;
			}
		}

		public virtual AxleConfiguration AxleConfiguration => AxleConfigurationHelper.Parse(GetString(XMLNames.Vehicle_AxleConfiguration));

		public virtual string ManufacturerAddress => GetString(XMLNames.Component_ManufacturerAddress);

		public virtual PerSecond EngineIdleSpeed => GetDouble(XMLNames.Vehicle_IdlingSpeed).RPMtoRad();

		public virtual double RetarderRatio => GetDouble(XMLNames.Vehicle_RetarderRatio);

		public virtual IPTOTransmissionInputData PTOTransmissionInputData => _ptoData ?? (_ptoData = PTOReader.PTOInputData);

		public virtual RetarderType RetarderType => RetarderTypeHelper.Parse(GetString(XMLNames.Vehicle_RetarderType));
		
		public virtual AngledriveType AngledriveType => GetString(XMLNames.Vehicle_AngledriveType).ParseEnum<AngledriveType>();

		public virtual bool VocationalVehicle => XmlConvert.ToBoolean(GetString(XMLNames.Vehicle_VocationalVehicle));

		public virtual bool? SleeperCab => ElementExists(XMLNames.Vehicle_SleeperCab)
			? XmlConvert.ToBoolean(GetString(XMLNames.Vehicle_SleeperCab))
			: (bool?)null;

		public virtual bool? AirdragModifiedMultistep { get; }

		public virtual TankSystem? TankSystem =>
			ElementExists(XMLNames.Vehicle_NgTankSystem)
				? EnumHelper.ParseEnum<TankSystem>(GetString(XMLNames.Vehicle_NgTankSystem))
				: (TankSystem?)null;

		public virtual IAdvancedDriverAssistantSystemDeclarationInputData ADAS => ADASReader.ADASInputData;

		public virtual bool ZeroEmissionVehicle => XmlConvert.ToBoolean(GetString(XMLNames.Vehicle_ZeroEmissionVehicle));

		public virtual bool HybridElectricHDV => XmlConvert.ToBoolean(GetString(XMLNames.Vehicle_HybridElectricHDV));

		public virtual bool DualFuelVehicle => XmlConvert.ToBoolean(GetString(XMLNames.Vehicle_DualFuelVehicle));

		public virtual Watt MaxNetPower1 =>
			ElementExists(XMLNames.Vehicle_MaxNetPower1)
				? GetDouble(XMLNames.Vehicle_MaxNetPower1).SI<Watt>()
				: null;

		public virtual Watt MaxNetPower2 =>
			ElementExists(XMLNames.Vehicle_MaxNetPower2)
				? GetDouble(XMLNames.Vehicle_MaxNetPower2).SI<Watt>()
				: null;

		public virtual string ExemptedTechnology => null;

		public virtual RegistrationClass? RegisteredClass => RegistrationClass.unknown;

		public virtual int? NumberPassengerSeatsUpperDeck => 0;

		public virtual int? NumberPassengerSeatsLowerDeck => 0;

		public virtual int? NumberPassengersStandingLowerDeck => 0;

		public virtual int? NumberPassengersStandingUpperDeck => 0;

		public virtual CubicMeter CargoVolume => 
			ElementExists(XMLNames.Vehicle_CargoVolume) 
				? GetDouble(XMLNames.Vehicle_CargoVolume).SI<CubicMeter>() : null;

		public virtual VehicleCode? VehicleCode => VectoCommon.Models.VehicleCode.NOT_APPLICABLE;

		public virtual bool? LowEntry => false;

		public virtual bool Articulated => false;

		public virtual Meter Height => null;

		public virtual Meter Length => null;

		public virtual Meter Width => null;

		public virtual Meter EntranceHeight => null;

		public virtual ConsumerTechnology? DoorDriveTechnology => ConsumerTechnology.Unknown;
		public virtual VehicleDeclarationType VehicleDeclarationType { get; }
		public virtual Dictionary<PowertrainPosition, List<Tuple<Volt, TableData>>> ElectricMotorTorqueLimits => null;
		public virtual TableData BoostingLimitations => null;


		public virtual IVehicleComponentsDeclaration Components => _components ?? (_components = ComponentReader.ComponentInputData);
		
		public virtual string VehicleTypeApprovalNumber => ElementExists(XMLNames.VehicleTypeApprovalNumber) ? GetString(XMLNames.Vehicle_TypeApprovalNumber) : null;
		public virtual ArchitectureID ArchitectureID => ArchitectureIDHelper.Parse(GetString(XMLNames.Vehicle_ArchitectureID));
		public virtual bool OvcHev => GetBool(XMLNames.Vehicle_OvcHev);
		public virtual Watt MaxChargingPower => ElementExists(XMLNames.Vehicle_MaxChargingPower) ?
			XmlConvert.ToInt32(GetString(XMLNames.Vehicle_MaxChargingPower)).SI<Watt>() : null;

		#region Implementation of IAdvancedDriverAssistantSystemDeclarationInputData

		#endregion

		#region Overrides of AbstractXMLResource

		protected override XNamespace SchemaNamespace => NAMESPACE_URI;

		protected override DataSourceType SourceType { get; }

		#endregion
	}

	// ---------------------------------------------------------------------------------------

	public class XMLDeclarationVehicleDataProviderV20 : XMLDeclarationVehicleDataProviderV10
	{
		/*
		 * use default values for new parameters introduced in 2019/318 (amendment of 2017/2400
		 */

		public new static readonly XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V20;

		public new const string XSD_TYPE = "VehicleDeclarationType";

		public new static readonly string QUALIFIED_XSD_TYPE =
			XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

		public XMLDeclarationVehicleDataProviderV20(IXMLDeclarationJobInputData jobData, XmlNode xmlNode, string sourceFile) :
			base(jobData, xmlNode, sourceFile) { }

		protected override XNamespace SchemaNamespace => NAMESPACE_URI;

		public override bool VocationalVehicle => false;

		public override bool? SleeperCab => true;

		public override TankSystem? TankSystem => VectoCommon.InputData.TankSystem.Compressed;

		public override IAdvancedDriverAssistantSystemDeclarationInputData ADAS => new ADASDefaultValues();

		public override bool ZeroEmissionVehicle => false;

		public override bool HybridElectricHDV => false;

		public override bool DualFuelVehicle => false;

		public override Watt MaxNetPower1 => null;

		public override Watt MaxNetPower2 => null;

		public class ADASDefaultValues : IAdvancedDriverAssistantSystemDeclarationInputData
		{
			#region Implementation of IAdvancedDriverAssistantSystemDeclarationInputData

			public bool EngineStopStart => false;

			public EcoRollType EcoRoll => EcoRollType.None;

			public PredictiveCruiseControlType PredictiveCruiseControl => PredictiveCruiseControlType.None;

			public bool? ATEcoRollReleaseLockupClutch => null;

			public XmlNode XMLSource => null;

			#endregion
		}
	}

	// ---------------------------------------------------------------------------------------

	public class XMLDeclarationVehicleDataProviderV21 : XMLDeclarationVehicleDataProviderV10
	{
		/*
		 * added new parameters introduced in 2019/318 (amendment of 2017/2400) (already implemented in version 1.0)
		 */

		public new static readonly XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V21;

		public new const string XSD_TYPE = "VehicleDeclarationType";

		public new static readonly string QUALIFIED_XSD_TYPE =
			XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);


		public XMLDeclarationVehicleDataProviderV21(IXMLDeclarationJobInputData jobData, XmlNode xmlNode, string sourceFile) :
			base(jobData, xmlNode, sourceFile) { }

		#region Overrides of XMLDeclarationVehicleDataProviderV10

		#endregion

		public override VehicleCategory VehicleCategory
		{
			get {
				var val = GetString(XMLNames.Vehicle_VehicleCategory);
				if ("Rigid Lorry".Equals(val, StringComparison.InvariantCultureIgnoreCase)) {
					return VehicleCategory.RigidTruck;
				}

				return val.ParseEnum<VehicleCategory>();
			}
		}

		protected override XNamespace SchemaNamespace => NAMESPACE_URI;
	}

	// ---------------------------------------------------------------------------------------

	public class XMLDeclarationExemptedVehicleDataProviderV22 : XMLDeclarationVehicleDataProviderV20
	{
		public new static readonly XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V21;

		public new const string XSD_TYPE = "ExemptedVehicleDeclarationType";

		public new static readonly string QUALIFIED_XSD_TYPE =
			XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

		public XMLDeclarationExemptedVehicleDataProviderV22(
			IXMLDeclarationJobInputData jobData, XmlNode xmlNode, string sourceFile) : base(jobData, xmlNode, sourceFile)
		{
			SourceType = DataSourceType.XMLEmbedded;
		}

		#region Overrides of AbstractXMLResource

		protected override XNamespace SchemaNamespace => NAMESPACE_URI;

		protected override DataSourceType SourceType { get; }

		#endregion

		#region Implementation of IVehicleDeclarationInputData

		public override bool ExemptedVehicle => true;

		public override AxleConfiguration AxleConfiguration => AxleConfiguration.AxleConfig_Undefined;

		public override IList<ITorqueLimitInputData> TorqueLimits => new List<ITorqueLimitInputData>();

		public override PerSecond EngineIdleSpeed => null;

		public override bool VocationalVehicle => false;

		public override bool? SleeperCab => null;

		public override TankSystem? TankSystem => null;

		public override IAdvancedDriverAssistantSystemDeclarationInputData ADAS => null;

		public override bool ZeroEmissionVehicle => XmlConvert.ToBoolean(GetString(XMLNames.Vehicle_ZeroEmissionVehicle));

		public override bool HybridElectricHDV => XmlConvert.ToBoolean(GetString(XMLNames.Vehicle_HybridElectricHDV));

		public override bool DualFuelVehicle => XmlConvert.ToBoolean(GetString(XMLNames.Vehicle_DualFuelVehicle));

		public override Watt MaxNetPower1 => GetDouble(XMLNames.Vehicle_MaxNetPower1).SI<Watt>();

		public override Watt MaxNetPower2 => GetDouble(XMLNames.Vehicle_MaxNetPower2).SI<Watt>();

		public override IVehicleComponentsDeclaration Components => null;

		#endregion

		#region Implementation of IXMLDeclarationVehicleData

		public override XmlElement ComponentNode => null;

		public override XmlElement PTONode => null;

		public override XmlElement ADASNode => null;

		public override AngledriveType AngledriveType => AngledriveType.None;

		public override RetarderType RetarderType => RetarderType.None;

		public override double RetarderRatio => 0;

		public override IPTOTransmissionInputData PTOTransmissionInputData => null;

		#endregion
	}


	// ---------------------------------------------------------------------------------------

	public class XMLDeclarationExemptedVehicleDataProviderV221 : XMLDeclarationExemptedVehicleDataProviderV22
	{
		public new static readonly XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V221;

		public new const string XSD_TYPE = "ExemptedVehicleDeclarationType";

		public new static readonly string QUALIFIED_XSD_TYPE =
			XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

		public XMLDeclarationExemptedVehicleDataProviderV221(
			IXMLDeclarationJobInputData jobData, XmlNode xmlNode, string sourceFile) : base(jobData, xmlNode,
			sourceFile)
		{
		}

		public override bool? SleeperCab => ElementExists(XMLNames.Vehicle_SleeperCab) ? XmlConvert.ToBoolean(GetString(XMLNames.Vehicle_SleeperCab)) : (bool?)null;

		public override AxleConfiguration AxleConfiguration =>
			ElementExists(XMLNames.Vehicle_AxleConfiguration)
				? AxleConfigurationHelper.Parse(GetString(XMLNames.Vehicle_AxleConfiguration))
				: AxleConfiguration.AxleConfig_Undefined;
	}

	// ---------------------------------------------------------------------------------------


	public class XMLDeclarationMultistagePrimaryVehicleBusDataProviderV01 : AbstractCommonComponentType, IXMLDeclarationVehicleData
	{

		public static readonly XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_MULTISTAGE_BUS_VEHICLE_NAMESPACE_VO1;

		public const string XSD_TYPE = "VehiclePIFType";

		public static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

		protected IXMLPrimaryVehicleBusJobInputData BusJobData;
		private XmlElement _adasNode;
		private IAdvancedDriverAssistantSystemDeclarationInputData _adas;
		private XmlElement _componentNode;
		private IVehicleComponentsDeclaration _components;
		
		public XMLDeclarationMultistagePrimaryVehicleBusDataProviderV01(
			IXMLPrimaryVehicleBusJobInputData busJobData, XmlNode xmlNode, string sourceFile)
			: base(xmlNode, sourceFile)
		{
			BusJobData = busJobData;
		}

		#region Overrides of AbstractCommonComponentType

		public override string Manufacturer => GetString(XMLNames.ManufacturerPrimaryVehicle);

		public string ManufacturerAddress => GetString(XMLNames.ManufacturerAddressPrimaryVehicle);

		#endregion


		#region IXMLDeclarationVehicleData interface

		public string VIN => GetString(XMLNames.Vehicle_VIN);

		public virtual LegislativeClass? LegislativeClass => GetString(XMLNames.Vehicle_LegislativeCategory)?.ParseEnum<LegislativeClass>();

		public virtual VehicleCategory VehicleCategory => VehicleCategoryHelper.Parse(GetString("ChassisConfiguration"));

		public virtual AxleConfiguration AxleConfiguration => AxleConfigurationHelper.Parse(GetString(XMLNames.Vehicle_AxleConfiguration));

		//TechnicalPermissibleMaximumLadenMass
		public virtual Kilogram GrossVehicleMassRating => GetDouble(XMLNames.TPMLM).SI<Kilogram>();

		//IdlingSpeed
		public virtual PerSecond EngineIdleSpeed => GetDouble(XMLNames.Engine_IdlingSpeed).RPMtoRad();

		
		public virtual RetarderType RetarderType => RetarderTypeHelper.Parse(GetString(XMLNames.Vehicle_RetarderType));

		public virtual double RetarderRatio => GetDouble(XMLNames.Vehicle_RetarderRatio);

		
		public virtual AngledriveType AngledriveType => GetString(XMLNames.Vehicle_AngledriveType).ParseEnum<AngledriveType>();

		
		public virtual bool ZeroEmissionVehicle => GetBool(XMLNames.Vehicle_ZeroEmissionVehicle);

		
		public virtual XmlElement ADASNode => _adasNode ?? (_adasNode = GetNode(XMLNames.Vehicle_ADAS, required: false) as XmlElement);

		public virtual IXMLADASReader ADASReader { get; set; }

		
		public virtual IAdvancedDriverAssistantSystemDeclarationInputData ADAS => ExemptedVehicle ? null : (_adas ?? (_adas = ADASReader?.ADASInputData));


		public virtual IList<ITorqueLimitInputData> TorqueLimits => ReadTorqueLimits();

		public virtual XmlElement ComponentNode
		{
			get
			{
				if (ExemptedVehicle)
				{
					return null;
				}

				return _componentNode ?? (_componentNode = GetNode(XMLNames.Vehicle_Components) as XmlElement);
			}
		}

		public virtual IXMLComponentReader ComponentReader { get; set; }

		public virtual Meter EntranceHeight { get; }

		public virtual ConsumerTechnology? DoorDriveTechnology => ConsumerTechnology.Unknown;

		public virtual VehicleDeclarationType VehicleDeclarationType { get; }
		public Dictionary<PowertrainPosition, List<Tuple<Volt, TableData>>> ElectricMotorTorqueLimits { get; }
		public TableData BoostingLimitations { get; }

		public virtual string VehicleTypeApprovalNumber { get; }


		public virtual IVehicleComponentsDeclaration Components => _components ?? (_components = ComponentReader.ComponentInputData);
		public ArchitectureID ArchitectureID { get; }
		public bool OvcHev { get; }
		public Watt MaxChargingPower { get; }


		#region  Non seeded Properties

		public string Identifier { get; }
		public virtual bool ExemptedVehicle { get; }
		public int? NumberPassengerSeatsUpperDeck { get; }
		public int? NumberPassengerSeatsLowerDeck { get; }
		public int? NumberPassengersStandingLowerDeck { get; }
		public int? NumberPassengersStandingUpperDeck { get; }

		public CubicMeter CargoVolume => 0.SI<CubicMeter>();
		public Kilogram CurbMassChassis { get; }
		public bool VocationalVehicle { get; }
		public bool? SleeperCab { get; }
		public bool? AirdragModifiedMultistep { get; }
		public TankSystem? TankSystem { get; }

		public bool HybridElectricHDV { get; }
		public bool DualFuelVehicle { get; }
		public virtual Watt MaxNetPower1 { get; }
		public Watt MaxNetPower2 { get; }
		public virtual string ExemptedTechnology { get; }
		public RegistrationClass? RegisteredClass { get; }
		public VehicleCode? VehicleCode { get; }
		public bool? LowEntry { get; }
		public bool Articulated => GetBool(XMLNames.Vehicle_Articulated);
		public Meter Height { get; }
		public Meter Length { get; }
		public Meter Width { get; }

		public XmlElement PTONode { get; }
		public IXMLPTOReader PTOReader { get; set; }
		public IPTOTransmissionInputData PTOTransmissionInputData { get; }

		#endregion

		#endregion

		#region Overrides of AbstractXMLResource

		protected override XNamespace SchemaNamespace => NAMESPACE_URI;

		protected override DataSourceType SourceType => DataSourceType.XMLFile;

		#endregion


		private IList<ITorqueLimitInputData> ReadTorqueLimits()
		{
			var torqueLimits = new List<ITorqueLimitInputData>();
			var limits = GetNodes(new[] { XMLNames.Vehicle_TorqueLimits, XMLNames.Vehicle_TorqueLimits_Entry });
			foreach (XmlNode current in limits)
			{
				if (current.Attributes != null)
				{
					torqueLimits.Add(
						new TorqueLimitInputData()
						{
							Gear = GetAttribute(current, XMLNames.Vehicle_TorqueLimits_Entry_Gear_Attr).ToInt(),
							MaxTorque = GetAttribute(current, XMLNames.Vehicle_TorqueLimits_Entry_MaxTorque_Attr)
								.ToDouble().SI<NewtonMeter>()
						});
				}
			}

			return torqueLimits;
		}
	}

	// ---------------------------------------------------------------------------------------

	public class XMLDeclarationMultistageExemptedPrimaryVehicleBusDataProviderV01 : XMLDeclarationMultistagePrimaryVehicleBusDataProviderV01
	{

		public new static readonly XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_MULTISTAGE_BUS_VEHICLE_NAMESPACE_VO1;

		public new const string XSD_TYPE = "VehicleExemptedPrimaryBusType";

		public new static readonly string QUALIFIED_XSD_TYPE =
			XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

		public XMLDeclarationMultistageExemptedPrimaryVehicleBusDataProviderV01(IXMLPrimaryVehicleBusJobInputData busJobData, XmlNode xmlNode, string sourceFile) : base(busJobData, xmlNode, sourceFile) { }

		#region Overrides of XMLDeclarationMultistagePrimaryVehicleBusDataProviderV01

		public override XmlElement ComponentNode => null;

		public override IVehicleComponentsDeclaration Components => null;

		public override bool ExemptedVehicle => true;

		public override Watt MaxNetPower1 => GetDouble("SumNetPower").SI<Watt>();


		#endregion
	}

	// ---------------------------------------------------------------------------------------


	public abstract class AbstractXMLVehicleDataProviderV24 : XMLDeclarationVehicleDataProviderV20
	{
		public virtual string PowertrainPositionPrefix => "P";

		protected IAdvancedDriverAssistantSystemDeclarationInputData _adas;

		protected AbstractXMLVehicleDataProviderV24(IXMLDeclarationJobInputData jobData, XmlNode xmlNode, string sourceFile) 
			: base(jobData, xmlNode, sourceFile) { }
		

		#region Overrides of XMLDeclarationVehicleDataProviderV10

		public override bool ZeroEmissionVehicle => GetBool(XMLNames.Vehicle_ZeroEmissionVehicle);

		public override bool VocationalVehicle => GetBool(XMLNames.Vehicle_VocationalVehicle);

		public override VehicleCategory VehicleCategory =>
			VehicleCategoryHelper.Parse(GetString(XMLNames.ChassisConfiguration));

		public override Kilogram CurbMassChassis => GetDouble(XMLNames.CorrectedActualMass).SI<Kilogram>();

		public override Kilogram GrossVehicleMassRating => GetDouble(XMLNames.TPMLM).SI<Kilogram>();

		public override bool Articulated => GetBool(XMLNames.Vehicle_Articulated);

		public override LegislativeClass? LegislativeClass => GetString(XMLNames.Vehicle_LegislativeCategory).ParseEnum<LegislativeClass>();
		//get { return GetString("LegislativeCategory").ParseEnum<LegislativeClass>(); }

		public override TankSystem? TankSystem => ElementExists(XMLNames.Vehicle_NgTankSystem)
			? GetString(XMLNames.Vehicle_NgTankSystem).ParseEnum<TankSystem>()
			: (TankSystem?) null;
		#endregion

		#region Overrides of XMLDeclarationVehicleDataProviderV20

		public override bool? SleeperCab => GetBool(XMLNames.Vehicle_SleeperCab);

		public override IAdvancedDriverAssistantSystemDeclarationInputData ADAS {
			get {
				if (ADASNode == null)
					return null;
				return _adas ?? (_adas = ADASReader.ADASInputData);
			}
		}

		#endregion

		public override Dictionary<PowertrainPosition, List<Tuple<Volt, TableData>>> ElectricMotorTorqueLimits
			=> ElementExists(XMLNames.ElectricMotorTorqueLimits) ? ReadElectricMotorTorqueLimits() : null;
		
		private Dictionary<PowertrainPosition, List<Tuple<Volt, TableData>>> ReadElectricMotorTorqueLimits()
		{
			var torqueLimitNodes = GetNodes(XMLNames.ElectricMotorTorqueLimits);
			var motorTorqueLimits = new Dictionary<PowertrainPosition, List<Tuple<Volt, TableData>>>();

			foreach (XmlNode torqueLimitNode in torqueLimitNodes)
			{
				var electricMachineNodes = GetNodes(XMLNames.ElectricMotorTorqueLimit_ElectricMachine, torqueLimitNode);
				if (electricMachineNodes == null || electricMachineNodes.Count == 0)
					return null;
				
				foreach (XmlNode electricMachineNode in electricMachineNodes)
				{
					var powertrainPosition =
						PowertrainPositionHelper.Parse(PowertrainPositionPrefix, GetString(XMLNames.ElectricMachine_Position, electricMachineNode));

					if (!motorTorqueLimits.ContainsKey(powertrainPosition))
						motorTorqueLimits.Add(powertrainPosition, new List<Tuple<Volt, TableData>>());

					var voltageLevelNodes = GetNodes(XMLNames.ElectricMachine_VoltageLevel, electricMachineNode);
					foreach (XmlNode voltageLevelNode in voltageLevelNodes)
					{
						var voltageLevel = ReadVoltageLevelNode(voltageLevelNode);
						motorTorqueLimits[powertrainPosition].Add(voltageLevel);
					}
				}
			}

			return motorTorqueLimits.Count == 0 ? null : motorTorqueLimits;
		}

		private Tuple<Volt, TableData> ReadVoltageLevelNode(XmlNode voltageLevelNode)
		{
			var voltage = GetString(XMLNames.VoltageLevel_Voltage, voltageLevelNode).ToDouble().SI<Volt>();
			var entries = voltageLevelNode.SelectNodes(XMLHelper.QueryLocalName(XMLNames.MaxTorqueCurve, XMLNames.MaxTorqueCurve_Entry));
			var mapping = new Dictionary<string, string> {
							{ XMLNames.MaxTorqueCurve_OutShaftSpeed, XMLNames.MaxTorqueCurve_OutShaftSpeed},
							{ XMLNames.MaxTorqueCurve_MaxTorque, XMLNames.MaxTorqueCurve_MaxTorque },
							{ XMLNames.MaxTorqueCurve_MinTorque, XMLNames.MaxTorqueCurve_MinTorque}
						};
			var maxTorqueCurve = XMLHelper.ReadTableData(mapping, entries);
			return new Tuple<Volt, TableData>(voltage, maxTorqueCurve);
		}
	}


	// ---------------------------------------------------------------------------------------


	
}