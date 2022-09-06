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

using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using TUGraz.IVT.VectoXML;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.FileIO.XML.Engineering.Interfaces;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.InputData.FileIO.XML.Engineering.DataProvider
{
	internal class XMLEngineeringEngineDataProviderV07 : AbstractEngineeringXMLComponentDataProvider,
		IXMLEngineData, IEngineModeEngineeringInputData, IEngineFuelEngineeringInputData
	{
		public static readonly XNamespace NAMESPACE_URI = XMLDefinitions.ENGINEERING_DEFINITONS_NAMESPACE_V07;

		public const string XSD_TYPE = "EngineDataEngineeringType";

		public static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

		protected IList<IEngineModeEngineeringInputData> _modes;

		public XMLEngineeringEngineDataProviderV07(
			IXMLEngineeringVehicleData vehicle,
			XmlNode vehicleNode, string fsBasePath)
			: base(vehicle, vehicleNode, fsBasePath)
		{
			SourceType = (vehicle as IXMLResource).DataSource.SourceFile == fsBasePath
				? DataSourceType.XMLEmbedded
				: DataSourceType.XMLFile;
		}

		
		public XMLEngineeringEngineDataProviderV07(XmlNode node, string sourceFile) : base(null, node, sourceFile) { }

		public virtual CubicMeter Displacement
		{
			get { return GetDouble(XMLNames.Engine_Displacement).SI(Unit.SI.Cubic.Centi.Meter).Cast<CubicMeter>(); }
		}

		public virtual PerSecond IdleSpeed
		{
			get { return GetDouble(XMLNames.Engine_IdlingSpeed).RPMtoRad(); }
		}

		public virtual double WHTCEngineering
		{
			get { return GetDouble(XMLNames.Engine_WHTCEngineering); }
		}
		IList<IEngineModeEngineeringInputData> IEngineEngineeringInputData.EngineModes => _modes ?? (_modes = ReadEngineModes());

		public virtual Second EngineStartTime => null;


		public virtual double WHTCMotorway
		{
			get { throw new VectoException("Property not available in Engineering Mode"); }
		}

		public virtual double WHTCRural
		{
			get { throw new VectoException("Property not available in Engineering Mode"); }
		}

		public virtual double WHTCUrban
		{
			get { throw new VectoException("Property not available in Engineering Mode"); }
		}

		public virtual double ColdHotBalancingFactor
		{
			get { throw new VectoException("Property not available in Engineering Mode"); }
		}

		public virtual double CorrectionFactorRegPer
		{
			get {
				return 1;

				//GetDoubleElementValue(XMLNames.Engine_CorrectionFactor_RegPer); 
			}
		}

		public virtual double NetCalorificValueCorrectionFactor
		{
			get { return 1; }
		}

		public virtual FuelType FuelType
		{
			get {
				return FuelType.DieselCI; //GetElementValue(XMLNames.Engine_FuelType).ParseEnum<FuelType>();
			}
		}

		public virtual TableData FuelConsumptionMap
		{
			get {
				return XMLHelper.ReadEntriesOrResource(
					BaseNode, DataSource.SourcePath, XMLNames.Engine_FuelConsumptionMap, XMLNames.Engine_FuelConsumptionMap_Entry,
					AttributeMappings.FuelConsumptionMapMapping);
			}
		}

		public virtual TableData FullLoadCurve
		{
			get {
				return XMLHelper.ReadEntriesOrResource(
					BaseNode, DataSource.SourcePath, XMLNames.Engine_FullLoadAndDragCurve, XMLNames.Engine_FullLoadCurve_Entry,
					AttributeMappings.EngineFullLoadCurveMapping);
			}
		}

		IList<IEngineFuelEngineeringInputData> IEngineModeEngineeringInputData.Fuels {
			get { return new[] { this }.Cast<IEngineFuelEngineeringInputData>().ToList(); }
		}

		public virtual IList<IEngineFuelDeclarationInputData> Fuels {
			get { return new[] { this }.Cast<IEngineFuelDeclarationInputData>().ToList(); }
		}

		public virtual IWHRData WasteHeatRecoveryDataElectrical => null;


		public virtual IWHRData WasteHeatRecoveryDataMechanical => null;
		public virtual Watt RatedPowerDeclared
		{
			get {
				return null; //GetDoubleElementValue(XMLNames.Engine_RatedPower).SI<Watt>(); 
			}
		}

		public virtual PerSecond RatedSpeedDeclared
		{
			get {
				return null; //GetDoubleElementValue(XMLNames.Engine_RatedSpeed).RPMtoRad(); 
			}
		}

		public virtual NewtonMeter MaxTorqueDeclared
		{
			get {
				return null; //GetDoubleElementValue(XMLNames.Engine_MaxTorque).SI<NewtonMeter>(); 
			}
		}

		public virtual IList<IEngineModeDeclarationInputData> EngineModes => (_modes ?? (_modes = ReadEngineModes())).Cast<IEngineModeDeclarationInputData>().ToList();

		protected virtual IList<IEngineModeEngineeringInputData> ReadEngineModes()
		{
			return new IEngineModeEngineeringInputData[] { this };
		}

		public virtual WHRType WHRType => WHRType.None;

		public virtual KilogramSquareMeter Inertia
		{
			get { return GetString(XMLNames.Engine_Inertia, required: false)?.ToDouble().SI<KilogramSquareMeter>(); }
		}

		#region Overrides of AbstractXMLResource

		protected override XNamespace SchemaNamespace
		{
			get { return NAMESPACE_URI; }
		}

		protected override DataSourceType SourceType { get; }

		#endregion
	}

	internal class XMLEngineeringEngineDataProviderV10 : XMLEngineeringEngineDataProviderV07
	{
		public new const string NAMESPACE_URI = XMLDefinitions.ENGINEERING_DEFINITONS_NAMESPACE_V10;

		//public new const string XSD_TYPE = "EngineDataEngineeringType";

		public new static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI, XSD_TYPE);

		public XMLEngineeringEngineDataProviderV10(
			XMLEngineeringVehicleDataProviderV07 vehicle, XmlNode vehicleNode, string fsBasePath) : base(
			vehicle, vehicleNode, fsBasePath) { }

		// engine-only constructor
		public XMLEngineeringEngineDataProviderV10(XmlNode node, string sourceFile) : base(node, sourceFile) { }

		public override double WHTCEngineering
		{
			get { return GetDouble(XMLNames.Engine_FCCorrection, 1.0); }
		}

		#region Overrides of XMLEngineeringEngineDataProviderV07

		#region Overrides of XMLEngineeringEngineDataProviderV07

		protected override XNamespace SchemaNamespace
		{
			get { return NAMESPACE_URI; }
		}

		#endregion

		#endregion
	}

	// this class is just for testing reading derived XML datatypes
	internal class XMLEngineeringEngineDataProviderV10TEST : XMLEngineeringEngineDataProviderV10
	{
		public new static readonly XNamespace NAMESPACE_URI = XMLDefinitions.ENGINEERING_DEFINITONS_NAMESPACE_V10_TEST;

		public new const string XSD_TYPE = "EngineDataEngineeringType";

		public new static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI, XSD_TYPE);

		public XMLEngineeringEngineDataProviderV10TEST(
			XMLEngineeringVehicleDataProviderV07 vehicle, XmlNode vehicleNode, string fsBasePath) : base(
			vehicle, vehicleNode, fsBasePath) { }

		// engine-only constructor
		public XMLEngineeringEngineDataProviderV10TEST(XmlNode node, string sourceFile) : base(node, sourceFile) { }

		#region Overrides of XMLEngineeringEngineDataProviderV07

		public override PerSecond RatedSpeedDeclared
		{
			get { return GetString(XMLNames.Engine_RatedSpeed).Replace("rpm", "").ToDouble(0).RPMtoRad(); }
		}

		protected override XNamespace SchemaNamespace
		{
			get { return NAMESPACE_URI; }
		}

		#endregion
	}
}
