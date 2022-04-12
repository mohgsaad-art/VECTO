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
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms.VisualStyles;
using System.Xml;
using System.Xml.Linq;
using TUGraz.VectoCommon.BusAuxiliaries;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.FileIO.XML.Common;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Factory;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Interfaces;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Reader;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Reader.Impl;
using TUGraz.VectoCore.InputData.Impl;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Utils;
using TrailerTypeHelper = TUGraz.VectoCommon.Models.TrailerTypeHelper;

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
			get
			{
				if (ExemptedVehicle)
				{
					return null;
				}

				return _componentNode ?? (_componentNode = GetNode(XMLNames.Vehicle_Components) as XmlElement);
			}
		}

		public virtual IXMLComponentReader ComponentReader { protected get; set; }

		public virtual XmlElement PTONode
		{
			get
			{
				if (ExemptedVehicle)
				{
					return null;
				}

				return _ptoNode ?? (_ptoNode = GetNode(XMLNames.Vehicle_PTO) as XmlElement);
			}
		}

		public virtual IXMLPTOReader PTOReader { protected get; set; }

		public virtual XmlElement ADASNode
		{
			get { return _adasNode ?? (_adasNode = GetNode(XMLNames.Vehicle_ADAS, required: false) as XmlElement); }
		}

		public virtual IXMLADASReader ADASReader { protected get; set; }

		public virtual IXMLDeclarationJobInputData Job { get; }

		public virtual string Identifier
		{
			get { return GetAttribute(BaseNode, XMLNames.Component_ID_Attr); }
		}

		public virtual bool ExemptedVehicle
		{
			get { return ElementExists(XMLNames.Vehicle_HybridElectricHDV) && ElementExists(XMLNames.Vehicle_DualFuelVehicle); }
		}

		public virtual string VIN
		{
			get { return GetString(XMLNames.Vehicle_VIN); }
		}

		public virtual LegislativeClass LegislativeClass
		{
			get { return GetString(XMLNames.Vehicle_LegislativeClass).ParseEnum<LegislativeClass>(); }
		}

		public virtual VehicleCategory VehicleCategory
		{
			get
			{
				var val = GetString(XMLNames.Vehicle_VehicleCategory);
				if ("Rigid Lorry".Equals(val, StringComparison.InvariantCultureIgnoreCase))
				{
					return VehicleCategory.RigidTruck;
				}

				return val.ParseEnum<VehicleCategory>();
			}
		}

		public virtual Kilogram CurbMassChassis
		{
			get { return GetDouble(XMLNames.Vehicle_CurbMassChassis).SI<Kilogram>(); }
		}


		public virtual Kilogram GrossVehicleMassRating
		{
			get { return GetDouble(XMLNames.Vehicle_GrossVehicleMass).SI<Kilogram>(); }
		}

		public virtual IList<ITorqueLimitInputData> TorqueLimits
		{
			get
			{
				var retVal = new List<ITorqueLimitInputData>();
				var limits = GetNodes(new[] { XMLNames.Vehicle_TorqueLimits, XMLNames.Vehicle_TorqueLimits_Entry });
				foreach (XmlNode current in limits)
				{
					if (current.Attributes != null)
					{
						retVal.Add(
							new TorqueLimitInputData()
							{
								Gear = GetAttribute(current, XMLNames.Vehicle_TorqueLimits_Entry_Gear_Attr).ToInt(),
								MaxTorque = GetAttribute(current, XMLNames.Vehicle_TorqueLimits_Entry_MaxTorque_Attr)
									.ToDouble().SI<NewtonMeter>()
							});
					}
				}

				return retVal;
			}
		}

		public virtual AxleConfiguration AxleConfiguration
		{
			get { return AxleConfigurationHelper.Parse(GetString(XMLNames.Vehicle_AxleConfiguration)); }
		}

		public virtual string ManufacturerAddress
		{
			get { return GetString(XMLNames.Component_ManufacturerAddress); }
		}

		public virtual PerSecond EngineIdleSpeed
		{
			get { return GetDouble(XMLNames.Vehicle_IdlingSpeed).RPMtoRad(); }
		}

		public virtual double RetarderRatio
		{
			get { return GetDouble(XMLNames.Vehicle_RetarderRatio); }
		}

		public virtual IPTOTransmissionInputData PTOTransmissionInputData
		{
			get { return _ptoData ?? (_ptoData = PTOReader.PTOInputData); }
		}

		public virtual RetarderType RetarderType
		{
			get
			{
				var value = GetString(XMLNames.Vehicle_RetarderType); //.ParseEnum<RetarderType>(); 
				switch (value)
				{
					case "None": return RetarderType.None;
					case "Losses included in Gearbox": return RetarderType.LossesIncludedInTransmission;
					case "Engine Retarder": return RetarderType.EngineRetarder;
					case "Transmission Input Retarder": return RetarderType.TransmissionInputRetarder;
					case "Transmission Output Retarder": return RetarderType.TransmissionOutputRetarder;
				}

				throw new ArgumentOutOfRangeException("RetarderType", value);
			}
		}

		public virtual AngledriveType AngledriveType
		{
			get { return GetString(XMLNames.Vehicle_AngledriveType).ParseEnum<AngledriveType>(); }
		}

		public virtual bool VocationalVehicle
		{
			get { return XmlConvert.ToBoolean(GetString(XMLNames.Vehicle_VocationalVehicle)); }
		}

		public virtual bool SleeperCab
		{
			get { return XmlConvert.ToBoolean(GetString(XMLNames.Vehicle_SleeperCab)); }
		}

		public virtual TankSystem? TankSystem
		{
			get
			{
				return ElementExists(XMLNames.Vehicle_NgTankSystem)
					? EnumHelper.ParseEnum<TankSystem>(GetString(XMLNames.Vehicle_NgTankSystem))
					: (TankSystem?)null;
			}
		}

		public virtual IAdvancedDriverAssistantSystemDeclarationInputData ADAS
		{
			get { return ADASReader.ADASInputData; }
		}

		public virtual bool ZeroEmissionVehicle
		{
			get { return XmlConvert.ToBoolean(GetString(XMLNames.Vehicle_ZeroEmissionVehicle)); }
		}

		public virtual bool HybridElectricHDV
		{
			get { return XmlConvert.ToBoolean(GetString(XMLNames.Vehicle_HybridElectricHDV)); }
		}

		public virtual bool DualFuelVehicle
		{
			get { return XmlConvert.ToBoolean(GetString(XMLNames.Vehicle_DualFuelVehicle)); }
		}

		public virtual Watt MaxNetPower1
		{
			get
			{
				return ElementExists(XMLNames.Vehicle_MaxNetPower1)
					? GetDouble(XMLNames.Vehicle_MaxNetPower1).SI<Watt>()
					: null;
			}
		}

		public virtual Watt MaxNetPower2
		{
			get
			{
				return ElementExists(XMLNames.Vehicle_MaxNetPower2)
					? GetDouble(XMLNames.Vehicle_MaxNetPower2).SI<Watt>()
					: null;
			}
		}

		public virtual RegistrationClass RegisteredClass
		{
			get { return RegistrationClass.unknown; }
		}

		public virtual int NumberOfPassengersUpperDeck
		{
			get { return 0; }
		}

		public virtual int NumberOfPassengersLowerDeck
		{
			get { return 0; }
		}

		public virtual CubicMeter CargoVolume
		{
			get { return 0.SI<CubicMeter>(); }
		}

		public virtual VehicleCode VehicleCode
		{
			get { return VehicleCode.NOT_APPLICABLE; }
		}

		public virtual bool LowEntry
		{
			get { return false; }
		}

		public virtual bool Articulated
		{
			get { return false; }
		}

		public virtual Meter Height
		{
			get { return null; }
		}

		public virtual Meter Length
		{
			get { return null; }
		}

		public virtual Meter Width
		{
			get { return null; }
		}

		public virtual Meter EntranceHeight
		{
			get { return null; }
		}

		public virtual ConsumerTechnology DoorDriveTechnology { get { return ConsumerTechnology.Unknown; } }

		public virtual IVehicleComponentsDeclaration Components
		{
			get { return _components ?? (_components = ComponentReader.ComponentInputData); }
		}

		#region Implementation of IAdvancedDriverAssistantSystemDeclarationInputData

		#endregion

		#region Overrides of AbstractXMLResource

		protected override XNamespace SchemaNamespace
		{
			get { return NAMESPACE_URI; }
		}

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
			base(jobData, xmlNode, sourceFile)
		{ }

		protected override XNamespace SchemaNamespace
		{
			get { return NAMESPACE_URI; }
		}

		public override bool VocationalVehicle
		{
			get { return false; }
		}

		public override bool SleeperCab
		{
			get { return true; }
		}

		public override TankSystem? TankSystem
		{
			get { return VectoCommon.InputData.TankSystem.Compressed; }
		}

		public override IAdvancedDriverAssistantSystemDeclarationInputData ADAS
		{
			get { return new ADASDefaultValues(); }
		}

		public override bool ZeroEmissionVehicle
		{
			get { return false; }
		}

		public override bool HybridElectricHDV
		{
			get { return false; }
		}

		public override bool DualFuelVehicle
		{
			get { return false; }
		}

		public override Watt MaxNetPower1
		{
			get { return null; }
		}

		public override Watt MaxNetPower2
		{
			get { return null; }
		}

		public class ADASDefaultValues : IAdvancedDriverAssistantSystemDeclarationInputData
		{
			#region Implementation of IAdvancedDriverAssistantSystemDeclarationInputData

			public bool EngineStopStart
			{
				get { return false; }
			}

			public EcoRollType EcoRoll
			{
				get { return EcoRollType.None; }
			}

			public PredictiveCruiseControlType PredictiveCruiseControl
			{
				get { return PredictiveCruiseControlType.None; }
			}

			public bool? ATEcoRollReleaseLockupClutch
			{
				get { return null; }
			}

			public XmlNode XMLSource
			{
				get { return null; }
			}

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
			base(jobData, xmlNode, sourceFile)
		{ }

		#region Overrides of XMLDeclarationVehicleDataProviderV10

		#endregion

		public override VehicleCategory VehicleCategory
		{
			get
			{
				var val = GetString(XMLNames.Vehicle_VehicleCategory);
				if ("Rigid Lorry".Equals(val, StringComparison.InvariantCultureIgnoreCase))
				{
					return VehicleCategory.RigidTruck;
				}

				return val.ParseEnum<VehicleCategory>();
			}
		}

		protected override XNamespace SchemaNamespace
		{
			get { return NAMESPACE_URI; }
		}
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

		protected override XNamespace SchemaNamespace
		{
			get { return NAMESPACE_URI; }
		}

		protected override DataSourceType SourceType { get; }

		#endregion

		#region Implementation of IVehicleDeclarationInputData

		public override bool ExemptedVehicle
		{
			get { return true; }
		}

		public override AxleConfiguration AxleConfiguration
		{
			get { return AxleConfiguration.AxleConfig_Undefined; }
		}

		public override IList<ITorqueLimitInputData> TorqueLimits
		{
			get { return new List<ITorqueLimitInputData>(); }
		}

		public override PerSecond EngineIdleSpeed
		{
			get { return null; }
		}

		public override bool VocationalVehicle
		{
			get { return false; }
		}

		public override bool SleeperCab
		{
			get { return false; }
		}

		public override TankSystem? TankSystem
		{
			get { return null; }
		}

		public override IAdvancedDriverAssistantSystemDeclarationInputData ADAS
		{
			get { return null; }
		}

		public override bool ZeroEmissionVehicle
		{
			get { return XmlConvert.ToBoolean(GetString(XMLNames.Vehicle_ZeroEmissionVehicle)); }
		}

		public override bool HybridElectricHDV
		{
			get { return XmlConvert.ToBoolean(GetString(XMLNames.Vehicle_HybridElectricHDV)); }
		}

		public override bool DualFuelVehicle
		{
			get { return XmlConvert.ToBoolean(GetString(XMLNames.Vehicle_DualFuelVehicle)); }
		}

		public override Watt MaxNetPower1
		{
			get { return GetDouble(XMLNames.Vehicle_MaxNetPower1).SI<Watt>(); }
		}

		public override Watt MaxNetPower2
		{
			get { return GetDouble(XMLNames.Vehicle_MaxNetPower2).SI<Watt>(); }
		}

		public override IVehicleComponentsDeclaration Components
		{
			get { return null; }
		}

		#endregion

		#region Implementation of IXMLDeclarationVehicleData

		public override XmlElement ComponentNode
		{
			get { return null; }
		}

		public override XmlElement PTONode
		{
			get { return null; }
		}

		public override XmlElement ADASNode
		{
			get { return null; }
		}

		public override AngledriveType AngledriveType
		{
			get { return AngledriveType.None; }
		}

		public override RetarderType RetarderType
		{
			get { return RetarderType.None; }
		}

		public override double RetarderRatio
		{
			get { return 0; }
		}

		public override IPTOTransmissionInputData PTOTransmissionInputData
		{
			get { return null; }
		}

		#endregion
	}

	// ---------------------------------------------------------------------------------------


	public class XMLDeclarationPrimaryBusVehicleDataProviderV26 : XMLDeclarationVehicleDataProviderV20
	{
		public new static readonly XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V26;

		public new const string XSD_TYPE = "PrimaryVehicleDeclarationType";

		public new static readonly string QUALIFIED_XSD_TYPE =
			XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

		public XMLDeclarationPrimaryBusVehicleDataProviderV26(
			IXMLDeclarationJobInputData jobData, XmlNode xmlNode, string sourceFile) : base(jobData, xmlNode, sourceFile)
		{
			SourceType = DataSourceType.XMLEmbedded;

		}


		#region Overrides of XMLDeclarationVehicleDataProviderV10

		public override bool SleeperCab
		{
			get { return false; }
		}

		public override IAdvancedDriverAssistantSystemDeclarationInputData ADAS
		{
			get { return ADASReader.ADASInputData; }
		}

		public override XmlElement PTONode
		{
			get { return null; }
		}

		#region Overrides of XMLDeclarationVehicleDataProviderV10

		public override LegislativeClass LegislativeClass
		{
			get { return LegislativeClass.M3; }
		}

		#endregion

		public override IPTOTransmissionInputData PTOTransmissionInputData
		{
			get { return null; }
		}

		public override VehicleCategory VehicleCategory
		{
			get { return VehicleCategory.HeavyBusPrimaryVehicle; }
		}

		public override bool Articulated
		{
			get { return GetBool(XMLNames.Vehicle_Articulated); }
		}

		public override Kilogram CurbMassChassis
		{
			get { return null; }
		}

		public override Kilogram GrossVehicleMassRating
		{
			get { return GetDouble(XMLNames.Vehicle_TPMLM).SI<Kilogram>(); }
		}

		public override Meter EntranceHeight
		{
			get { return null; }
		}

		#endregion

		#region Overrides of AbstractXMLResource

		protected override XNamespace SchemaNamespace
		{
			get { return NAMESPACE_URI; }
		}

		protected override DataSourceType SourceType { get; }

		#endregion
	}

	public class XMLDeclarationMediumLorryVehicleDataProviderV26 : XMLDeclarationVehicleDataProviderV20
	{
		public new static readonly XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V26;

		public new const string XSD_TYPE = "VehicleMediumLorryDeclarationType";

		public new static readonly string QUALIFIED_XSD_TYPE =
			XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

		public XMLDeclarationMediumLorryVehicleDataProviderV26(
			IXMLDeclarationJobInputData jobData, XmlNode xmlNode, string sourceFile) : base(jobData, xmlNode, sourceFile) { }


		#region Overrides of XMLDeclarationVehicleDataProviderV10

		public override bool SleeperCab
		{
			get { return false; }
		}

		public override bool VocationalVehicle
		{
			get { return false; }
		}

		public override IPTOTransmissionInputData PTOTransmissionInputData
		{
			get { return null; }
		}

		public override XmlElement PTONode
		{
			get { return null; }
		}

		public override Kilogram GrossVehicleMassRating
		{
			get { return GetDouble(XMLNames.Vehicle_TPMLM).SI<Kilogram>(); }
		}

		public override CubicMeter CargoVolume
		{
			get
			{
				if (VehicleCategory == VehicleCategory.Van && !ElementExists(XMLNames.Vehicle_CargoVolume))
				{
					throw new VectoException("Medium lorries with type Van require the input parameter cargo volume!");
				}
				return ElementExists(XMLNames.Vehicle_CargoVolume) ? GetDouble(XMLNames.Vehicle_CargoVolume).SI<CubicMeter>()
					: 0.SI<CubicMeter>();
			}
		}

		#endregion

		#region Overrides of AbstractXMLResource

		protected override XNamespace SchemaNamespace
		{
			get { return NAMESPACE_URI; }
		}

		protected override DataSourceType SourceType { get; }

		#endregion
	}

	public class XMLDeclarationCompletedBusDataProviderV26 : XMLDeclarationVehicleDataProviderV20
	{
		public new static readonly XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V26;

		public new const string XSD_TYPE = "CompletedVehicleDeclarationType";

		public new static readonly string QUALIFIED_XSD_TYPE =
			XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

		public XMLDeclarationCompletedBusDataProviderV26(
			IXMLDeclarationJobInputData jobData, XmlNode xmlNode, string sourceFile) : base(jobData, xmlNode, sourceFile)
		{
			SourceType = DataSourceType.XMLEmbedded;
		}


		#region Overrides of XMLDeclarationVehicleDataProviderV10

		public override VehicleCategory VehicleCategory
		{
			get { return VehicleCategory.HeavyBusCompletedVehicle; }
		}

		public override RegistrationClass RegisteredClass
		{
			get { return RegistrationClassHelper.Parse(GetString(XMLNames.Vehicle_RegisteredClass)).First(); }
		}

		public override VehicleCode VehicleCode
		{
			get { return GetString(XMLNames.Vehicle_VehicleCode).ParseEnum<VehicleCode>(); }
		}

		//TechnicalPermissibleMaximumLadenMass
		public override Kilogram GrossVehicleMassRating
		{
			get { return GetDouble(XMLNames.TPMLM).SI<Kilogram>(); }
		}

		public override TankSystem? TankSystem
		{
			get
			{
				return ElementExists(XMLNames.Vehicle_NgTankSystem)
						? EnumHelper.ParseEnum<TankSystem>(GetString(XMLNames.Vehicle_NgTankSystem))
						: (TankSystem?)null;
			}
		}

		public override int NumberOfPassengersLowerDeck
		{
			get
			{
				var node = GetNode(XMLNames.Bus_LowerDeck);
				return XmlConvert.ToInt32(node.InnerText);
			}
		}

		public override int NumberOfPassengersUpperDeck
		{
			get
			{
				var node = GetNode(XMLNames.Bus_UpperDeck);
				return XmlConvert.ToInt32(node.InnerText);
			}
		}

		//HeightIntegratedBody
		public override Meter Height
		{
			get { return GetDouble(XMLNames.Bus_HeighIntegratedBody).SI<Meter>(); }
		}

		//VehicleLength
		public override Meter Length
		{
			get { return GetDouble(XMLNames.Bus_VehicleLength).SI<Meter>(); }
		}

		//VehicleWidth
		public override Meter Width
		{
			get { return GetDouble(XMLNames.Bus_VehicleWidth).SI<Meter>(); }
		}

		public override XmlElement PTONode
		{
			get { return null; }
		}

		#endregion


		public override bool LowEntry
		{
			get { return GetBool(XMLNames.Bus_LowEntry); }
		}

		public override Meter EntranceHeight
		{
			get { return GetDouble(XMLNames.Bus_EntranceHeight).SI<Meter>(); }
		}

		public override ConsumerTechnology DoorDriveTechnology
		{
			get { return ConsumerTechnologyHelper.Parse(GetString(XMLNames.BusAux_PneumaticSystem_DoorDriveTechnology)); }
		}


		#region Overrides of AbstractXMLResource

		protected override XNamespace SchemaNamespace
		{
			get { return NAMESPACE_URI; }
		}

		protected override DataSourceType SourceType { get; }

		#endregion
	}


	// ---------------------------------------------------------------------------------------

	public class XMLDeclarationVehicleDataProviderV27 : AbstractCommonComponentType, IXMLTrailerDeclarationVehicleData
	{
		public static readonly XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V27;

		public const string XSD_TYPE = "VehicleDeclarationType";

		public static readonly string QUALIFIED_XSD_TYPE =
			XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

		protected IVehicleComponentsDeclaration _components;
		protected IAxlesDeclarationInputData _axles;
		protected XmlNode _axlesNode;
		protected XmlElement _componentNode;

		private CertifiedAeroDeviceReader _certifiedAeroDevice = null;
		private CertifiedAeroDeviceReader CertifiedAeroDevice
		{
			get => _certifiedAeroDevice ?? (_certifiedAeroDevice = ElementExists(XMLNames.Trailer_CertifiedAeroDevice)
				? new CertifiedAeroDeviceReader(GetNode(XMLNames.Trailer_CertifiedAeroDevice))
				: null);
		}

		public XMLDeclarationVehicleDataProviderV27(IXMLDeclarationTrailerJobInputData jobData, XmlNode xmlNode,
			string sourceFile) : base(xmlNode, sourceFile)
		{

			SourceFile = sourceFile;
			Job = jobData;
			SourceType = DataSourceType.XMLEmbedded;
		}

		public string CommercialName { get; set; }
		public virtual XmlNode AxleWheelsNode => _axlesNode ?? (_axlesNode = GetNode(new[] { XMLNames.Component_AxleWheels, XMLNames.ComponentDataWrapper }, required: false));

		public XmlElement ComponentNode => _componentNode ?? (_componentNode = GetNode(XMLNames.Vehicle_Components) as XmlElement);

		public virtual IXMLComponentReader ComponentReader { protected get; set; }

		public IXMLTrailerAxlesReader AxlesReader { get; set; }

		public virtual IXMLDeclarationTrailerJobInputData Job { get; }

		public IList<ITrailerAxleDeclarationInputData> Axles => ReadAxles();

		public string TrailerModel => GetString(XMLNames.Trailer_Model);

		public string Identifier => GetAttribute(BaseNode, XMLNames.Component_ID_Attr);

		public string ManufacturerAddress => GetString(XMLNames.Component_ManufacturerAddress);

		public string VIN => GetString(XMLNames.Vehicle_VIN);

		public string LegislativeCategory => GetString(XMLNames.Trailer_LegislativeCategory);

		public NumberOfTrailerAxles NumberOfAxles => NumberOfTrailerAxlesHelper.Parse(GetString(XMLNames.Trailer_AxleCount));

		public TypeTrailer TrailerType => TrailerTypeHelper.Parse(GetString(XMLNames.Trailer_TrailerType));

		public BodyWorkCode BodyworkCode => BodyWorkCodeHelper.Parse(GetString(XMLNames.Trailer_BodyCode));

		public Kilogram MassInRunningOrder => GetDouble(XMLNames.Trailer_MassInRunningOrder).SI<Kilogram>();

		public Kilogram TPMLMTotalTrailer => GetDouble(XMLNames.Trailer_TPMLMTotalTrailer).SI<Kilogram>();

		public Kilogram TPMLMAxleAssembly => ElementExists(XMLNames.Trailer_TPLMAxleAssembly)
			? GetDouble(XMLNames.Trailer_TPLMAxleAssembly).SI<Kilogram>()
			: null;

		//public Kilogram TPMLMAxleAssembly => TrailerType != TypeTrailer.DB ? GetDouble(XMLNames.Trailer_TPLMAxleAssembly).SI<Kilogram>() : 0.SI<Kilogram>();

		public Meter ExternalBodyLength => GetDouble(XMLNames.Trailer_ExternalLengthBody).SI<Meter>();

		public Meter ExternalBodyWidth => GetDouble(XMLNames.Trailer_ExternalWidthBody).SI<Meter>();

		public Meter ExternalBodyHeight => GetDouble(XMLNames.Trailer_ExternalHeightBody).SI<Meter>();

		public Meter TotalTrailerHeight => GetDouble(XMLNames.Trailer_TotalHeightTrailer).SI<Meter>();

		public Meter LengthFromFrontToFirstAxle => GetDouble(XMLNames.Trailer_LengthFromFrontToFirstAxle).SI<Meter>();

		public Meter LengthBetweenCentersOfAxles => GetDouble(XMLNames.Trailer_LengthBetweenCentersOfAxles).SI<Meter>();

		public bool VolumeOrientation => GetBool(XMLNames.Trailer_VolumeOrientation);

		public CubicMeter CargoVolume => GetDouble(XMLNames.Trailer_CargoVolume).SI<CubicMeter>();

		public TrailerCouplingPoint TrailerCouplingPoint => ElementExists(XMLNames.Trailer_TrailerCouplingPoint) 
			? TrailerCouplingPointHelper.Parse(GetString(XMLNames.Trailer_TrailerCouplingPoint))
			: TrailerCouplingPoint.Unknown;

		public string CertifiedAeroDeviceApplicableVehicleGroup => GetNodes(XMLNames.Trailer_AeroDevice_ApplicableVehicleGroup).Count > 0
			? GetString(XMLNames.Trailer_AeroDevice_ApplicableVehicleGroup)
			: null;
		//public double YawAngle0 => GetNodes(XMLNames.Trailer_YawAngle0).Count > 0 ? GetDouble(XMLNames.Trailer_YawAngle0) : 0;
		//public double YawAngle3 => GetNodes(XMLNames.Trailer_YawAngle3).Count > 0 ? GetDouble(XMLNames.Trailer_YawAngle3) : 0;
		//public double YawAngle6 => GetNodes(XMLNames.Trailer_YawAngle6).Count > 0 ? GetDouble(XMLNames.Trailer_YawAngle6) : 0;
		//public double YawAngle9 => GetNodes(XMLNames.Trailer_YawAngle9).Count > 0 ? GetDouble(XMLNames.Trailer_YawAngle9) : 0;
		public double YawAngle0 => CertifiedAeroDevice?.YawAngle0 ?? 0;
		public double YawAngle3 => CertifiedAeroDevice?.YawAngle3 ?? 0;
		public double YawAngle6 => CertifiedAeroDevice?.YawAngle6 ?? 0;
		public double YawAngle9 => CertifiedAeroDevice?.YawAngle9 ?? 0;



		public IList<AeroFeatureTechnology> AeroFeatureTechnologies => ReadAeroFeatures();

		public string CertifiedAeroCertificationNumber => CertifiedAeroDevice?.CertificationNumber;
		public string CertifiedAeroSignatureXml => CertifiedAeroDevice?.SignatureXml;

		public IVehicleComponentsDeclaration Components => _components ?? (_components = ComponentReader.ComponentInputData);

		protected override XNamespace SchemaNamespace => NAMESPACE_URI;

		protected override DataSourceType SourceType { get; }

		protected IList<ITrailerAxleDeclarationInputData> ReadAxles()
		{
			var axles = new List<ITrailerAxleDeclarationInputData>();

			var trailerAxles = GetNodes(new[] { XMLNames.AxleWheels_Axles_Axle });

			foreach (XmlNode current in trailerAxles)
			{
				if (current != null)
				{
					// MQ: 2020-10-23 Never invoke a constructor directly when working with dependency injection. In such cases all injected members (in this case the Factory) are not set!
					//var axle = new XMLDeclarationAxleDataProviderV27(this, AxleWheelsNode, SourceFile) {
					//Reader = new XMLComponentReaderV27(this, AxleWheelsNode)
					//};
					var axle = AxlesReader.CreateAxle(current);

					axles.Add(axle);
				}
			}

			return axles;
		}

		

		protected IList<AeroFeatureTechnology> ReadAeroFeatures()
		{
			var aeroFeatureTechnologies = new List<AeroFeatureTechnology>();
			var technologies = GetNodes(new[] { XMLNames.Trailer_AeroFeatureTechnologies });
			foreach (XmlNode current in technologies)
			{

				if (current == null)
					continue;

				var technology = AeroFeatureTechnologiesHelper.Parse(current.InnerText);
				if ((technology != AeroFeatureTechnology.None)) {
					aeroFeatureTechnologies.Add(technology);
				}
				//if (current.InnerText == AeroFeatureTechnology.SideSkirtsShort.GetLabel())
				//	aeroFeatureTechnologies.Add(AeroFeatureTechnology.SideSkirtsShort);
				//else if (current.InnerText == AeroFeatureTechnology.SideSkirtsLong.GetLabel())
				//	aeroFeatureTechnologies.Add(AeroFeatureTechnology.SideSkirtsLong);
				//else if (current.InnerText == AeroFeatureTechnology.BoatTailShort.GetLabel())
				//	aeroFeatureTechnologies.Add(AeroFeatureTechnology.BoatTailShort);
				//else if (current.InnerText == AeroFeatureTechnology.BoatTailLong.GetLabel())
				//	aeroFeatureTechnologies.Add(AeroFeatureTechnology.BoatTailLong);
			}

			return aeroFeatureTechnologies;
		}

		private class CertifiedAeroDeviceReader : AbstractXMLType
		{
			public CertifiedAeroDeviceReader(XmlNode certifiedDeviceNode) : base(certifiedDeviceNode) {


			}
			public double YawAngle0
			{
				get
				{
					var nodes = this.GetNodes(XMLNames.Trailer_YawAngle0);
					return GetNodes(XMLNames.Trailer_CertifiedAeroReduction).Count > 0 ? GetDouble(XMLNames.Trailer_YawAngle0) : 0;
				}
			}

			public double YawAngle3
			{
				get
				{
					return GetNodes(XMLNames.Trailer_YawAngle3).Count > 0 ? GetDouble(XMLNames.Trailer_YawAngle3) : 0;
				}
			}

			public double YawAngle6 => this.GetNodes(XMLNames.Trailer_YawAngle6).Count > 0 ? GetDouble(XMLNames.Trailer_YawAngle6) : 0;
			public double YawAngle9 => this.GetNodes(XMLNames.Trailer_YawAngle9).Count > 0 ? GetDouble(XMLNames.Trailer_YawAngle9) : 0;

			public string CertificationNumber => ElementExists(XMLNames.Component_CertificationNumber)
				? GetString(XMLNames.Component_CertificationNumber): null;

			public string SignatureXml =>
				ElementExists(XMLNames.DI_Signature) ? GetNode(XMLNames.DI_Signature).OuterXml : null;

			public string CertifiedAeroDeviceApplicableVehicleGroup => GetNodes(XMLNames.Trailer_AeroDevice_ApplicableVehicleGroup).Count > 0
				? GetString(XMLNames.Trailer_AeroDevice_ApplicableVehicleGroup)
				: null;
		}
	}

	// ---------------------------------------------------------------------------------------
	public class XMLDeclarationPrimaryVehicleBusDataProviderV01 : AbstractCommonComponentType, IXMLDeclarationVehicleData
	{
		public static readonly XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_PRIMARY_BUS_VEHICLE_URI_V01;

		public const string XSD_TYPE = "VehiclePIFType";

		public static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

		protected IXMLPrimaryVehicleBusJobInputData BusJobData;
		private XmlElement _adasNode;
		private IAdvancedDriverAssistantSystemDeclarationInputData _adas;
		private XmlElement _componentNode;
		private IVehicleComponentsDeclaration _components;


		public XMLDeclarationPrimaryVehicleBusDataProviderV01(
			IXMLPrimaryVehicleBusJobInputData busJobData, XmlNode xmlNode, string sourceFile)
			: base(xmlNode, sourceFile)
		{
			BusJobData = busJobData;
		}

		#region Overrides of AbstractCommonComponentType

		public override string Manufacturer
		{
			get { return GetString(XMLNames.ManufacturerPrimaryVehicle); }
		}

		public string ManufacturerAddress
		{
			get { return GetString(XMLNames.ManufacturerAddressPrimaryVehicle); }
		}

		#endregion


		#region IXMLDeclarationVehicleData interface

		public string VIN
		{
			get { return GetString(XMLNames.Vehicle_VIN); }
		}

		public VehicleCategory VehicleCategory
		{
			get { return VehicleCategoryHelper.Parse(GetString(XMLNames.Vehicle_VehicleCategory)); }
		}

		public AxleConfiguration AxleConfiguration
		{
			get { return AxleConfigurationHelper.Parse(GetString(XMLNames.Vehicle_AxleConfiguration)); }
		}

		//TechnicalPermissibleMaximumLadenMass
		public Kilogram GrossVehicleMassRating
		{
			get { return GetDouble(XMLNames.TPMLM).SI<Kilogram>(); }
		}

		//IdlingSpeed
		public PerSecond EngineIdleSpeed
		{
			get { return GetDouble(XMLNames.Engine_IdlingSpeed).RPMtoRad(); }
		}

		public RetarderType RetarderType
		{
			get { return GetString(XMLNames.Vehicle_RetarderType).ParseEnum<RetarderType>(); }
		}

		public double RetarderRatio
		{
			get { return GetDouble(XMLNames.Vehicle_RetarderRatio); }
		}

		public AngledriveType AngledriveType
		{
			get { return GetString(XMLNames.Vehicle_AngledriveType).ParseEnum<AngledriveType>(); }
		}

		public bool ZeroEmissionVehicle
		{
			get { return GetBool(XMLNames.Vehicle_ZeroEmissionVehicle); }
		}

		public XmlElement ADASNode
		{
			get { return _adasNode ?? (_adasNode = GetNode(XMLNames.Vehicle_ADAS, required: false) as XmlElement); }
		}

		public IXMLADASReader ADASReader { get; set; }

		public IAdvancedDriverAssistantSystemDeclarationInputData ADAS
		{
			get { return _adas ?? (_adas = ADASReader.ADASInputData); }
		}


		public IList<ITorqueLimitInputData> TorqueLimits
		{
			get { return ReadTorqueLimits(); }
		}

		public XmlElement ComponentNode
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

		public IXMLComponentReader ComponentReader { get; set; }

		public Meter EntranceHeight { get; }

		public virtual ConsumerTechnology DoorDriveTechnology
		{
			get { return ConsumerTechnology.Unknown; }
		}

		public IVehicleComponentsDeclaration Components
		{
			get { return _components ?? (_components = ComponentReader.ComponentInputData); }
		}


		#region  Non seeded Properties

		public string Identifier { get; }
		public bool ExemptedVehicle { get; }
		public LegislativeClass LegislativeClass { get; }
		public int NumberOfPassengersUpperDeck { get; }
		public int NumberOfPassengersLowerDeck { get; }
		public CubicMeter CargoVolume
		{
			get { return 0.SI<CubicMeter>(); }
		}
		public Kilogram CurbMassChassis { get; }
		public bool VocationalVehicle { get; }
		public bool SleeperCab { get; }
		public TankSystem? TankSystem { get; }

		public bool HybridElectricHDV { get; }
		public bool DualFuelVehicle { get; }
		public Watt MaxNetPower1 { get; }
		public Watt MaxNetPower2 { get; }
		public RegistrationClass RegisteredClass { get; }
		public VehicleCode VehicleCode { get; }
		public bool LowEntry { get; }
		public bool Articulated { get { return GetBool(XMLNames.Vehicle_Articulated); } }
		public Meter Height { get; }
		public Meter Length { get; }
		public Meter Width { get; }

		public XmlElement PTONode { get; }
		public IXMLPTOReader PTOReader { get; set; }
		public IPTOTransmissionInputData PTOTransmissionInputData { get; }

		#endregion

		#endregion

		#region Overrides of AbstractXMLResource

		protected override XNamespace SchemaNamespace
		{
			get { return NAMESPACE_URI; }
		}

		protected override DataSourceType SourceType
		{
			get { return DataSourceType.XMLFile; }
		}

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
}