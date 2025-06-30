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
using System.Xml;
using System.Xml.Linq;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.FileIO.XML.Common;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Interfaces;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.OutputData.XML;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.InputData.FileIO.XML.Declaration.DataProvider
{
	public class XMLDeclarationEngineDataProviderV10 : AbstractCommonComponentType, IXMLEngineDeclarationInputData
	{
		public static readonly XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V10;

		public const string XSD_TYPE = "EngineDataDeclarationType";

		public static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

		protected List<IEngineModeDeclarationInputData> _engineModes;

		public XMLDeclarationEngineDataProviderV10(
			IXMLDeclarationVehicleData vehicle, XmlNode componentNode, string sourceFile) :
			base(componentNode, sourceFile)
		{
			SourceType = DataSourceType.XMLFile;
		}

		#region Implementation of IEngineDeclarationInputData

		public virtual CubicMeter Displacement => GetDouble(XMLNames.Engine_Displacement).SI(Unit.SI.Cubic.Centi.Meter).Cast<CubicMeter>();

		public virtual Watt RatedPowerDeclared => GetDouble(XMLNames.Engine_RatedPower).SI<Watt>();

		public virtual PerSecond RatedSpeedDeclared => GetDouble(XMLNames.Engine_RatedSpeed).RPMtoRad();

		public virtual NewtonMeter MaxTorqueDeclared => GetDouble(XMLNames.Engine_MaxTorque).SI<NewtonMeter>();

		public virtual IList<IEngineModeDeclarationInputData> EngineModes =>
			_engineModes ??
			(_engineModes = new List<IEngineModeDeclarationInputData>() { new XMLSingleFuelEngineMode(BaseNode) });

		public virtual WHRType WHRType => WHRType.None;


		public class XMLSingleFuelEngineMode : AbstractXMLType, IEngineModeDeclarationInputData
		{
			protected IList<IEngineFuelDeclarationInputData> FuelsList;

			public XMLSingleFuelEngineMode(XmlNode baseNode) : base(baseNode) { }

			public virtual PerSecond IdleSpeed => GetDouble(XMLNames.Engine_IdlingSpeed).RPMtoRad();

			public virtual TableData FullLoadCurve =>
				ReadTableData(
					XMLNames.Engine_FullLoadAndDragCurve, XMLNames.Engine_FullLoadCurve_Entry,
					AttributeMappings.EngineFullLoadCurveMapping);

			public virtual IList<IEngineFuelDeclarationInputData> Fuels =>
				FuelsList ??
				(FuelsList = new List<IEngineFuelDeclarationInputData>() { new XMLSingleFuelEngineFuel(BaseNode) });

			public virtual IWHRData WasteHeatRecoveryDataElectrical => null;

			public virtual IWHRData WasteHeatRecoveryDataMechanical => null;
		}

		public class XMLSingleFuelEngineFuel : AbstractXMLType, IEngineFuelDeclarationInputData
		{
			public XMLSingleFuelEngineFuel(XmlNode baseNode) : base(baseNode) { }

			public virtual FuelType FuelType
			{
				get {
					var value = GetString(XMLNames.Engine_FuelType).Replace(" ", "");
					if ("LPG".Equals(value, StringComparison.InvariantCultureIgnoreCase)) {
						return FuelType.LPGPI;
					}
					if ("NG".Equals(value, StringComparison.InvariantCultureIgnoreCase)) {
						return FuelType.NGPI;
					}

					return value.ParseEnum<FuelType>();
				}
			}

			public virtual TableData FuelConsumptionMap =>
				ReadTableData(
					XMLNames.Engine_FuelConsumptionMap, XMLNames.Engine_FuelConsumptionMap_Entry,
					AttributeMappings.FuelConsumptionMapMapping);

			public virtual double WHTCMotorway => GetDouble(XMLNames.Engine_WHTCMotorway);

			public virtual double WHTCRural => GetDouble(XMLNames.Engine_WHTCRural);

			public virtual double WHTCUrban => GetDouble(XMLNames.Engine_WHTCUrban);

			public virtual double ColdHotBalancingFactor => GetDouble(XMLNames.Engine_ColdHotBalancingFactor);

			public virtual double CorrectionFactorRegPer => GetDouble(XMLNames.Engine_CorrectionFactor_RegPer);
		}

		#endregion

		#region Overrides of AbstractXMLResource

		protected override XNamespace SchemaNamespace => NAMESPACE_URI;

		protected override DataSourceType SourceType { get; }

		#endregion
	}


	// ---------------------------------------------------------------------------------------

	public class XMLDeclarationEngineDataProviderV20 : XMLDeclarationEngineDataProviderV10
	{
		//public new static readonly XNamespace
		public new static readonly XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V20;

		//public new const string XSD_TYPE = "EngineComponentDeclarationType";

		public new static readonly string QUALIFIED_XSD_TYPE =
			XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

		public XMLDeclarationEngineDataProviderV20(
			IXMLDeclarationVehicleData vehicle, XmlNode componentNode, string sourceFile) : base(
			vehicle, componentNode, sourceFile)
		{ }

		protected override XNamespace SchemaNamespace => NAMESPACE_URI;
	}

	// ---------------------------------------------------------------------------------------

	public class XMLDeclarationEngineDataProviderV21 : XMLDeclarationEngineDataProviderV20
	{
		/*
		 * harmonize fuel-type paramenter in Regulation 2019/318 (amendment of 2017/2400)
		 */

		public new static readonly XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V21;

		//public new const string XSD_TYPE = "EngineComponentDeclarationType";

		public new static readonly string QUALIFIED_XSD_TYPE =
			XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

		public XMLDeclarationEngineDataProviderV21(
			IXMLDeclarationVehicleData vehicle, XmlNode componentNode, string sourceFile) : base(
			vehicle, componentNode, sourceFile) { }

		protected override XNamespace SchemaNamespace => NAMESPACE_URI;
	}

	// ---------------------------------------------------------------------------------------

	public class XMLDeclarationEngineDataProviderV23 : XMLDeclarationEngineDataProviderV20
	{
		/*
		 * Support for dual-fuel engines (in different operating modes - either single fuel or dual fuel)
		 */

		public new static readonly XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V23;

		public new static readonly string QUALIFIED_XSD_TYPE =
			XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

		public XMLDeclarationEngineDataProviderV23(
			IXMLDeclarationVehicleData vehicle, XmlNode componentNode, string sourceFile) : base(
			vehicle, componentNode, sourceFile) { }

		protected override XNamespace SchemaNamespace => NAMESPACE_URI;

		#region Overrides of XMLDeclarationEngineDataProviderV10

		public override IList<IEngineModeDeclarationInputData> EngineModes
		{
			get {
				return _engineModes ?? (_engineModes = GetNodes(XMLNames.Engine_FuelModes)
							.Cast<XmlNode>().Select(x => new XMLDualFuelEngineMode(x, this)).Cast<IEngineModeDeclarationInputData>().ToList());
			}
		}


		public override WHRType WHRType
		{
			get {
				var retVal = WHRType.None;
				if (XmlConvert.ToBoolean(GetString(XMLNames.Engine_WHR_MechanicalOutputICE))) {
					retVal |= WHRType.MechanicalOutputICE;
				}
				if (XmlConvert.ToBoolean(GetString(XMLNames.Engine_WHR_MechanicalOutputIDrivetrain))) {
					retVal |= WHRType.MechanicalOutputDrivetrain;
				}
				if (XmlConvert.ToBoolean(GetString(XMLNames.Engine_WHR_ElectricalOutput))) {
					retVal |= WHRType.ElectricalOutput;
				}

				return retVal;
			}
		}

		#endregion

		public class XMLDualFuelEngineMode : XMLSingleFuelEngineMode
		{
			protected IXMLEngineDeclarationInputData Engine;
			
			protected IWHRData WHRDataEl;
			protected IWHRData WHRDataMech;

            public XMLDualFuelEngineMode(XmlNode baseNode,
				IXMLEngineDeclarationInputData engine) : base(baseNode)
			{
				Engine = engine;
			}


			#region Overrides of XMLSingleFuelEngineMode

			public override IList<IEngineFuelDeclarationInputData> Fuels
			{
				get {
					return FuelsList ?? (FuelsList = GetNodes(XMLNames.Engine_FuelModes_Fuel)
								.Cast<XmlNode>().Select(x => new XMLDualFuelEngineFuel(x))
								.Cast<IEngineFuelDeclarationInputData>().ToList());
				}
			}

			public override IWHRData WasteHeatRecoveryDataElectrical
			{
				get {
					if (!Engine.WHRType.IsElectrical()) {
						return null;
					}
					return WHRDataEl ?? (WHRDataEl = ReadWHRData(
								GetNodes(
									new[] {
										XMLNames.Engine_WHRCorrectionFactors,
										XMLNames.Engine_WHRCorrectionFactors_Electrical
									}, GetNode(XMLNames.Engine_FuelModes_Fuel)),
								XMLNames.Engine_FuelConsumptionMap_WHRElPower_Attr)
							);
				}
			}

			public override IWHRData WasteHeatRecoveryDataMechanical
			{
				get {
					if (!Engine.WHRType.IsMechanical()) {
						return null;
					}
					return WHRDataMech ?? (WHRDataMech = ReadWHRData(
								GetNodes(
									new[] {
										XMLNames.Engine_WHRCorrectionFactors,
										XMLNames.Engine_WHRCorrectionFactors_Mechanical
									}, GetNode(XMLNames.Engine_FuelModes_Fuel)),
								XMLNames.Engine_FuelConsumptionMap_WHRMechPower_Attr));
				}
			}

			#endregion

			protected virtual IWHRData ReadWHRData(XmlNodeList correctionFactorNodes, string fcMapAttr)
			{
				var whrPwrNodes = GetNodes(
						new[] {
							XMLNames.Engine_FuelConsumptionMap, XMLNames.Engine_FuelConsumptionMap_Entry
						}, GetNode(XMLNames.Engine_FuelModes_Fuel))
					.Cast<XmlNode>().All(x => x.Attributes?[fcMapAttr] != null);
				if (correctionFactorNodes.Count > 0) {
					if (!whrPwrNodes) {
						throw new VectoXMLException("WHR correction factors provided but {0} missing for some entries.", fcMapAttr);

					}
					//return new XMLDeclarationWHRData();
				}

				if (correctionFactorNodes.Count > 1) {
					throw new VectoXMLException("WHRData (correction factors) can only be defined for one fuel!");
				}

				if (whrPwrNodes) {
					if (correctionFactorNodes.Count == 0) {
						throw new VectoXMLException("WHR electric power provided but no correction factors found.");
					}
					//return new XMLDeclarationWHRData();
				}

				var fuelNodes = GetNodes(XMLNames.Engine_FuelModes_Fuel);
				XmlNode whrFuelNode = null;
				if (fuelNodes.Count > 1) {
					for (var i = 0; i < fuelNodes.Count; i++) {
						var fuel = fuelNodes[i];
						if (GetNodes(XMLNames.Engine_FuelConsumptionMap_Entry, fuel).Cast<XmlNode>()
																					.Any(x => x.Attributes?[fcMapAttr] != null)) {
							if (whrFuelNode != null) {
								throw new VectoException("WHRData ({0}) can only be defined for one fuel!", fcMapAttr);
							}

							whrFuelNode = fuel;
						}
					}
				} else {
					whrFuelNode = fuelNodes[0];
				}

				if (GetNodes(new[] { XMLNames.Engine_FuelConsumptionMap, XMLNames.Engine_FuelConsumptionMap_Entry }, whrFuelNode)
					.Cast<XmlNode>().Any(x => x.Attributes?[fcMapAttr] == null)) {
					var missing = GetNodes(
							new[] { XMLNames.Engine_FuelConsumptionMap, XMLNames.Engine_FuelConsumptionMap_Entry }, whrFuelNode)
						.Cast<XmlNode>().Where(x => x.Attributes?[fcMapAttr] == null);
					throw new VectoException("WHRData has to be provided for every entry in the FC-Map! {0}",
						missing.Select(x => $"n: {x.Attributes?[XMLNames.Engine_FuelConsumptionMap_EngineSpeed_Attr]?.Value}, " +
											$"T: {x.Attributes?[XMLNames.Engine_FuelConsumptionMap_Torque_Attr]?.Value}").Join("; "));
				}

				if (correctionFactorNodes[0].ParentNode.ParentNode != whrFuelNode) {
					throw new VectoException("Correction Factors and WHR-Map have to be defined for the same fuel!");
				}

				return new XMLDeclarationWHRData(whrFuelNode, correctionFactorNodes[0]);
			}
		}

		public class XMLDualFuelEngineFuel : XMLSingleFuelEngineFuel
		{
			public XMLDualFuelEngineFuel(XmlNode baseNode) : base(baseNode) { }

			#region Overrides of XMLSingleFuelEngineFuel

			public override FuelType FuelType => GetAttribute(BaseNode, "type").ParseEnum<FuelType>();

			#endregion
		}

		public class XMLDeclarationWHRData : AbstractXMLType, IWHRData
		{
			protected TableData WHRPower;
			protected XmlNode CorrectionFactorNode;

			public XMLDeclarationWHRData(XmlNode whrFuelNode, XmlNode correctionFactorNode) : base(whrFuelNode)
			{
				CorrectionFactorNode = correctionFactorNode;
			}

			public XMLDeclarationWHRData() : base(null) { }

			#region Implementation of IWHRData

			public double UrbanCorrectionFactor => GetNode(XMLNames.Engine_WHRCorrectionFactors_Urban, CorrectionFactorNode)?.InnerText.ToDouble() ?? 1;

			public double RuralCorrectionFactor => GetNode(XMLNames.Engine_WHRCorrectionFactors_Rural, CorrectionFactorNode)?.InnerText.ToDouble() ?? 1;

			public double MotorwayCorrectionFactor => GetNode(XMLNames.Engine_WHRCorrectionFactors_Motorway, CorrectionFactorNode)?.InnerText.ToDouble() ?? 1;

			public double BFColdHot => GetNode(XMLNames.Engine_WHRCorrectionFactors_BFColdHot, CorrectionFactorNode)?.InnerText.ToDouble() ?? 1;

			public double CFRegPer => GetNode(XMLNames.Engine_WHRCorrectionFactors_CFRegPer, CorrectionFactorNode)?.InnerText.ToDouble() ?? 1;

			public double EngineeringCorrectionFactor => 1.0;

			public TableData GeneratedPower =>
				WHRPower ?? (WHRPower = BaseNode == null
					? null
					: ReadTableData(
						XMLNames.Engine_FuelConsumptionMap, XMLNames.Engine_FuelConsumptionMap_Entry,
						AttributeMappings.WHRPowerMapMapping));

			#endregion
		}
	}

    // ---------------------------------------------------------------------------------------

    public class XMLDeclarationEngineDataProviderV26 : XMLDeclarationEngineDataProviderV23
    {
        public new static readonly XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V26;

        public new static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

        public XMLDeclarationEngineDataProviderV26(IXMLDeclarationVehicleData vehicle, XmlNode componentNode, string sourceFile) : base(vehicle, componentNode, sourceFile) 
		{ }

    }

	public class XMLDeclarationMultistagePrimaryVehicleBusEngineDataProviderV01 : XMLDeclarationEngineDataProviderV23
	{
		public new static readonly XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_MULTISTAGE_BUS_VEHICLE_NAMESPACE_VO1;

		public new const string XSD_TYPE = "EngineDataVIFType";

		public new static readonly string QUALIFIED_XSD_TYPE =
			XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

		public XMLDeclarationMultistagePrimaryVehicleBusEngineDataProviderV01(IXMLDeclarationVehicleData vehicle, XmlNode componentNode,
			string sourceFile) : base(vehicle, componentNode, sourceFile) { }

		public override IList<IEngineModeDeclarationInputData> EngineModes =>
			_engineModes ??
			(_engineModes = new List<IEngineModeDeclarationInputData>() { new XMLDeclarationEngineDataProviderV10.XMLSingleFuelEngineMode(BaseNode) });

		protected override XNamespace SchemaNamespace => NAMESPACE_URI;
	}
}
