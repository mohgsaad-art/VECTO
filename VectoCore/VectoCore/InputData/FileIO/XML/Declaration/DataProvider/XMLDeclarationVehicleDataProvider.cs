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
			get {
				var val = GetString(XMLNames.Vehicle_VehicleCategory);
				if ("Rigid Lorry".Equals(val, StringComparison.InvariantCultureIgnoreCase)) {
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

		public virtual AxleConfiguration AxleConfiguration
		{
			get
			{
				return ElementExists(XMLNames.Vehicle_AxleConfiguration)
					? AxleConfigurationHelper.Parse(GetString(XMLNames.Vehicle_AxleConfiguration))
					: AxleConfiguration.AxleConfig_Undefined;
			}
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
			get {
				var value = GetString(XMLNames.Vehicle_RetarderType); //.ParseEnum<RetarderType>(); 
				switch (value) {
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

		public virtual bool? SleeperCab
		{
			get
			{
				return ElementExists(XMLNames.Vehicle_SleeperCab)
					? XmlConvert.ToBoolean(GetString(XMLNames.Vehicle_SleeperCab))
					: (bool?)null;
			}
		}

		public virtual TankSystem? TankSystem
		{
			get {
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
			get {
				return ElementExists(XMLNames.Vehicle_MaxNetPower1)
					? GetDouble(XMLNames.Vehicle_MaxNetPower1).SI<Watt>()
					: null;
			}
		}

		public virtual Watt MaxNetPower2
		{
			get {
				return ElementExists(XMLNames.Vehicle_MaxNetPower2)
					? GetDouble(XMLNames.Vehicle_MaxNetPower2).SI<Watt>()
					: null;
			}
		}

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
			base(jobData, xmlNode, sourceFile) { }

		protected override XNamespace SchemaNamespace
		{
			get { return NAMESPACE_URI; }
		}

		public override bool VocationalVehicle
		{
			get { return false; }
		}

		public override bool? SleeperCab
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

			public bool? ATEcoRollReleaseLockupClutch => null;

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

		protected override XNamespace SchemaNamespace
		{
			get { return NAMESPACE_URI; }
		}
	}

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

		public override bool? SleeperCab
		{
			get { return null; }
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
			get { return ElementExists(XMLNames.Vehicle_MaxNetPower1) ? GetDouble(XMLNames.Vehicle_MaxNetPower1).SI<Watt>() : null; }
		}

		public override Watt MaxNetPower2
		{
			get { return ElementExists(XMLNames.Vehicle_MaxNetPower2) ? GetDouble(XMLNames.Vehicle_MaxNetPower2).SI<Watt>() : null; }
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

		public override bool? SleeperCab
		{
			get
			{
				return ElementExists(XMLNames.Vehicle_SleeperCab) ? XmlConvert.ToBoolean(GetString(XMLNames.Vehicle_SleeperCab)) : (bool?)null; 
			}
		}

		public override AxleConfiguration AxleConfiguration
		{
			get
			{
				return ElementExists(XMLNames.Vehicle_AxleConfiguration)
					? AxleConfigurationHelper.Parse(GetString(XMLNames.Vehicle_AxleConfiguration))
					: AxleConfiguration.AxleConfig_Undefined;
			}
		}
	}
}
