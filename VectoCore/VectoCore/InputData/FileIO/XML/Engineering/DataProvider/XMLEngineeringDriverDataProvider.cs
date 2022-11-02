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
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCore.InputData.FileIO.XML.Engineering.Interfaces;
using TUGraz.VectoCore.InputData.FileIO.XML.Engineering.Reader;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.InputData.FileIO.XML.Engineering.DataProvider
{
	internal class XMLEngineeringDriverDataProviderV07 : AbstractCommonComponentType, IXMLEngineeringDriverData
	{
		public static readonly XNamespace NAMESPACE_URI = XMLDefinitions.ENGINEERING_DEFINITONS_NAMESPACE_V07;

		public const string XSD_TYPE = "DriverModelType";

		public static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI, XSD_TYPE);

		private ILookaheadCoastingInputData _lookahead;
		private IOverSpeedEngineeringInputData _overspeed;
		private IXMLDriverAcceleration _accCurve;
		private IGearshiftEngineeringInputData _shiftParameters;

		public XMLEngineeringDriverDataProviderV07(
			IXMLEngineeringInputData inputData,
			XmlNode driverDataNode, string fsBasePath)
			: base(driverDataNode, fsBasePath)
		{
			SourceType = (inputData as IXMLResource).DataSource.SourceFile == fsBasePath
				? DataSourceType.XMLEmbedded
				: DataSourceType.XMLFile;
		}

		public IXMLDriverDataReader Reader { protected get; set; }

		public virtual IDriverAccelerationData AccelerationCurve => (_accCurve ?? (_accCurve = Reader.AccelerationCurveData)).AccelerationCurve;

		public virtual ILookaheadCoastingInputData Lookahead => _lookahead ?? (_lookahead = Reader.LookAheadData);

		public virtual IGearshiftEngineeringInputData GearshiftInputData => _shiftParameters ?? (_shiftParameters = Reader.ShiftParameters);

		public virtual IEngineStopStartEngineeringInputData EngineStopStartData => null;


		public virtual IEcoRollEngineeringInputData EcoRollData => null;

		public virtual IPCCEngineeringInputData PCCData => null;

		public virtual IOverSpeedEngineeringInputData OverSpeedData => _overspeed ?? (_overspeed = Reader.OverspeedData);

		#region Overrides of AbstractXMLResource

		protected override XNamespace SchemaNamespace => NAMESPACE_URI;

		protected override DataSourceType SourceType { get; }

		#endregion
	}

	internal class XMLEngineeringDriverDataProviderV10 : XMLEngineeringDriverDataProviderV07
	{
		public new static readonly XNamespace NAMESPACE_URI = XMLDefinitions.ENGINEERING_DEFINITONS_NAMESPACE_V10;

		public new const string XSD_TYPE = "DriverModelEngineeringType";

		public new static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI, XSD_TYPE);

		protected IEngineStopStartEngineeringInputData _engineStopStart;

		protected IEcoRollEngineeringInputData _ecoRollData;

		protected IPCCEngineeringInputData _pccData;


		public XMLEngineeringDriverDataProviderV10(
			IXMLEngineeringInputData inputData, XmlNode driverDataNode, string fsBasePath) : base(
			inputData, driverDataNode, fsBasePath) { }


		protected override XNamespace SchemaNamespace => NAMESPACE_URI;

		public override IEngineStopStartEngineeringInputData EngineStopStartData => _engineStopStart ?? (_engineStopStart = Reader.EngineStopStartData);

		public override IEcoRollEngineeringInputData EcoRollData => _ecoRollData ?? (_ecoRollData = Reader.EcoRollData);


		public override IPCCEngineeringInputData PCCData => _pccData ?? (_pccData = Reader.PCCData);
	}

	
}
