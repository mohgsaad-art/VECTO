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
using System.Xml;
using Ninject;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.InputData.FileIO.XML.Engineering.DataProvider;
using TUGraz.VectoCore.InputData.FileIO.XML.Engineering.Factory;
using TUGraz.VectoCore.InputData.FileIO.XML.Engineering.Interfaces;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.InputData.FileIO.XML.Engineering.Reader.Impl {
	internal class XMLDriverDataReaderV07 : IXMLDriverDataReader
	{
		public const string NAMESPACE_URI = XMLDefinitions.ENGINEERING_DEFINITONS_NAMESPACE_V07;

		public const string XSD_TYPE = "DriverModelType";

		public static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI, XSD_TYPE);

		protected XmlNode DriverDataNode;
		protected IXMLEngineeringDriverData DriverData;

		[Inject]
		public IEngineeringInjectFactory Factory { protected get; set; }

		public XMLDriverDataReaderV07(IXMLEngineeringDriverData driverData, XmlNode driverDataNode)
		{
			DriverData = driverData;
			DriverDataNode = driverDataNode;
		}

		public ILookaheadCoastingInputData LookAheadData
		{
			get {
				return CreateData(
					XMLNames.DriverModel_LookAheadCoasting, (version, node) => Factory.CreateLookAheadData(version, DriverData, node));
			}
		}

		public IOverSpeedEngineeringInputData OverspeedData
		{
			get {
				return CreateData(
					XMLNames.DriverModel_Overspeed, (version, node) => Factory.CreateOverspeedData(version, DriverData, node));
			}
		}

		
		public IEcoRollEngineeringInputData EcoRollData
		{
			get {
				return CreateData(
					XMLNames.DriverModel_EcoRoll,
					(version, node) => version == null ? null : Factory.CreateEcoRollData(version, DriverData, node), false);
			}
		}

		public IPCCEngineeringInputData PCCData
		{
			get {
				return CreateData(
					"PCCParameters",
					(version, node) => version == null ? null : Factory.CreatePCCData(version, DriverData, node), false);
			}
		}

		public IXMLDriverAcceleration AccelerationCurveData
		{
			get {
				return CreateData(
					XMLNames.DriverModel_DriverAccelerationCurve,
					(version, node) => version == null
						? DefaultAccelerationCurve()
						: Factory.CreateAccelerationCurveData(version, DriverData, node), false);
			}
		}

		public IGearshiftEngineeringInputData ShiftParameters =>
			CreateData(
				XMLNames.DriverModel_ShiftStrategyParameters, ShiftParametersCreator, false);

		public IEngineStopStartEngineeringInputData EngineStopStartData
		{
			get {
				return CreateData(
					XMLNames.DriverModel_EngineStopStartParameters, 
					(version, node) => version == null ? null : Factory.CreateEngineStopStartData(version, DriverData, node), false);
			}
		}

		private IGearshiftEngineeringInputData ShiftParametersCreator(string version, XmlNode node)
		{
			if (version == null) {
				return new XMLEngineeringGearshiftDataV07(null);
			}
			return Factory.CreateShiftParametersData(version, node);
		}

		protected virtual T CreateData<T>(string elementName, Func<string, XmlNode, T> creator, bool required = true)
		{
			var node = GetNode(elementName, required);
			if (!required && node == null) {
				try {
					return creator(null, null);
				} catch (Exception e) {
					throw new VectoException("Failed to create dummy data provider", e);
				}
			}

			var version = XMLHelper.GetXsdType(node.SchemaInfo.SchemaType);
			try {
				return creator(version, node);
			} catch (Exception e) {
				throw new VectoException("Unsupported XML Version! Node: {0} Version: {1}", e, node.LocalName, version);
			}
		}

		
		protected virtual XmlNode GetNode(string elementName, bool required = true)
		{
			var retVal =
				DriverDataNode.SelectSingleNode(XMLHelper.QueryLocalName(elementName));
			if (required && retVal == null) {
				throw new VectoException("Element {0} not found!", elementName);
			}

			return retVal;
		}

		private static IXMLDriverAcceleration DefaultAccelerationCurve()
		{
			try {
				var resourceName = DeclarationData.DeclarationDataResourcePrefix + ".VACC.Truck" +
									Constants.FileExtensions.DriverAccelerationCurve;
				return new XMLDriverAccelerationV07(
					VectoCSVFile.ReadStream(RessourceHelper.ReadStream(resourceName), source: resourceName));
			} catch (Exception e) {
				throw new VectoException("Failed to read Driver Acceleration Curve: " + e.Message, e);
			}
		}
	}
	
	internal class XMLDriverDataReaderV10 : XMLDriverDataReaderV07
	{
		public new const string NAMESPACE_URI = XMLDefinitions.ENGINEERING_DEFINITONS_NAMESPACE_V10;

		public new const string XSD_TYPE = "DriverModelEngineeringType";

		public new static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI, XSD_TYPE);


		public XMLDriverDataReaderV10(IXMLEngineeringDriverData driverData, XmlNode driverDataNode) : base(driverData, driverDataNode) { }
	}

}