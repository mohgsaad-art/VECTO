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
using System.IO;
using System.Linq;
using System.Reflection.Emit;
using System.Xml;
using System.Xml.XPath;
using Ninject;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.InputData.Reader.ComponentData;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.OutputData.FileIO;
using TUGraz.VectoCore.Tests.Utils;
using TUGraz.VectoCore.Utils;
using NUnit.Framework;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCore.InputData.FileIO.XML;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Reader.Impl;
using TUGraz.VectoCore.InputData.Reader.DataObjectAdapter;
using TUGraz.VectoCore.InputData.Reader.DataObjectAdapter.HeavyLorry;
using TUGraz.VectoCore.Models.Declaration.Auxiliaries;
using TUGraz.VectoCore.Models.Simulation.Impl.SimulatorFactory;
using XmlDocumentType = TUGraz.VectoCore.Utils.XmlDocumentType;

namespace TUGraz.VectoCore.Tests.XML
{
	[TestFixture]
	[Parallelizable(ParallelScope.All)]
	public class XMLDeclarationInputTest
	{
		const string SampleVehicleDecl = "TestData/XML/XMLReaderDeclaration/vecto_vehicle-sample.xml";
		const string SampleVehicleDeclNoAirdrag = "TestData/XML/XMLReaderDeclaration/vecto_vehicle-sample_noAirdrag.xml";
		const string SampleVehicleFullDecl = "TestData/XML/XMLReaderDeclaration/vecto_vehicle-sample_FULL.xml";
		const string SampleVehicleFullDeclUpdated = "TestData/XML/XMLReaderDeclaration/vecto_vehicle-sample_FULL_updated.xml";
		const string SampleVehicleFullDeclExempted = "TestData/XML/XMLReaderDeclaration/vecto_vehicle-sample_exempted.xml";

		const string SampleVehicleFullDeclCertificationOptions =
			"TestData/XML/XMLReaderDeclaration/vecto_vehicle-sample_certificationOptions.xml";

		protected IXMLInputDataReader xmlInputReader;
		private IKernel _kernel;

		[OneTimeSetUp]
		public void RunBeforeAnyTests()
		{
			Directory.SetCurrentDirectory(TestContext.CurrentContext.TestDirectory);

			_kernel = new StandardKernel(new VectoNinjectModule());
			xmlInputReader = _kernel.Get<IXMLInputDataReader>();
		}

		[TestCase]
		public void TestXMLInputDecl()
		{
			var reader = XmlReader.Create(SampleVehicleDecl);

			var inputDataProvider = xmlInputReader.CreateDeclaration(reader);

			var engineDataProvider = inputDataProvider.JobInputData.Vehicle.Components.EngineInputData;

			Assert.IsTrue(engineDataProvider.SavedInDeclarationMode);

			Assert.AreEqual("Generic 40t Long Haul Truck Engine", engineDataProvider.Model);
			Assert.AreEqual(0.012730, engineDataProvider.Displacement.Value());
			Assert.AreEqual(1.0097, engineDataProvider.EngineModes.First().Fuels.First().WHTCUrban);
			//AssertHelper.Exception<VectoException>(() => { var tmp = engineDataProvider.Inertia; });

			var fcMapTable = engineDataProvider.EngineModes.First().Fuels.First().FuelConsumptionMap;
			Assert.AreEqual(112, fcMapTable.Rows.Count);
			Assert.AreEqual("engine speed", fcMapTable.Columns[0].Caption);
			Assert.AreEqual("torque", fcMapTable.Columns[1].Caption);
			Assert.AreEqual("fuel consumption", fcMapTable.Columns[2].Caption);

			Assert.AreEqual("560.00", fcMapTable.Rows[0][0]);
			var fcMap = FuelConsumptionMapReader.Create(fcMapTable);
			Assert.AreEqual(1256.SI(Unit.SI.Gramm.Per.Hour).Value(),
				fcMap.GetFuelConsumption(0.SI<NewtonMeter>(), 560.RPMtoRad(), false).Value.Value());

			var fldTable = engineDataProvider.EngineModes.First().FullLoadCurve;
			Assert.AreEqual(10, fldTable.Rows.Count);
			Assert.AreEqual("engine speed", fldTable.Columns[0].Caption);
			Assert.AreEqual("full load torque", fldTable.Columns[1].Caption);
			Assert.AreEqual("motoring torque", fldTable.Columns[2].Caption);
			var fldMap = FullLoadCurveReader.Create(fldTable, true);
		}

		[TestCase()]
		public void TestEngineFuelTypes()
		{
			var fuelTypes = GetEnumOptions("FuelTypeType", "1.0");
			foreach (var fuel in fuelTypes) {
				if (!(fuel.EndsWith("CI") || fuel.EndsWith("PI"))) {
					// new fuel labels end either with CI or PI, others are for backward compatibility. separate testcase
					continue;
				}

				var reader = XmlReader.Create(SampleVehicleDecl);

				var doc = new XmlDocument();
				doc.Load(reader);
				var nav = doc.CreateNavigator();
				var manager = new XmlNamespaceManager(nav.NameTable);
				var helper = new XPathHelper(ExecutionMode.Declaration);
				helper.AddNamespaces(manager);

				var engineFuelType = nav.SelectSingleNode(helper.QueryAbs(
						helper.NSPrefix(XMLNames.VectoInputDeclaration,
							Constants.XML.RootNSPrefix),
						XMLNames.Component_Vehicle,
						XMLNames.Vehicle_Components,
						XMLNames.Component_Engine, XMLNames.ComponentDataWrapper, XMLNames.Engine_FuelType),
					manager);
				engineFuelType.SetValue(fuel);
				var modified = XmlReader.Create(new StringReader(nav.OuterXml));

				var inputDataProvider = xmlInputReader.CreateDeclaration(modified);
				var fuelTyle = inputDataProvider.JobInputData.Vehicle.Components.EngineInputData.EngineModes.First().Fuels.First().FuelType;
				Assert.AreEqual(fuel, fuelTyle.ToXMLFormat());
				var tankSystem = fuelTyle == FuelType.NGPI || fuelTyle == FuelType.NGCI ? TankSystem.Liquefied : (TankSystem?)null;
				Assert.NotNull(DeclarationData.FuelData.Lookup(fuelTyle, tankSystem));
			}
		}

		
		[TestCase]
		public void TestXMLInputGbx()
		{
			var reader = XmlReader.Create(SampleVehicleDecl);

			var inputDataProvider = xmlInputReader.CreateDeclaration(reader);
			var gearboxDataProvider = inputDataProvider.JobInputData.Vehicle.Components.GearboxInputData;

			Assert.AreEqual("Generic 40t Long Haul Truck Gearbox", gearboxDataProvider.Model);
			Assert.AreEqual(GearboxType.AMT, gearboxDataProvider.Type);
			var gears = gearboxDataProvider.Gears;
			Assert.AreEqual(12, gears.Count);

			Assert.AreEqual(1, gears.First().Gear);
			Assert.AreEqual(14.93, gears.First().Ratio);
			Assert.AreEqual("0.00", gears.First().LossMap.Rows[0][0]);
			Assert.AreEqual("-350.00", gears.First().LossMap.Rows[0][1]);
			Assert.AreEqual("12.06", gears.First().LossMap.Rows[0][2]);

			var lossMap = TransmissionLossMapReader.Create(gears.First().LossMap, gears.First().Ratio,
				gears.First().Gear.ToString());

			Assert.AreEqual(5000, gears.First().MaxTorque.Value());
		}

		[Category("LongRunning")]
		[TestCase]
		public void TestXMLInputAxlG()
		{
			var reader = XmlReader.Create(SampleVehicleDecl);

			var inputDataProvider = xmlInputReader.CreateDeclaration(reader);
			var axlegearDataProvider = inputDataProvider.JobInputData.Vehicle.Components.AxleGearInputData;

			Assert.AreEqual("Generic 40t Long Haul Truck AxleGear", axlegearDataProvider.Model);

			var lossMapData = axlegearDataProvider.LossMap;
			Assert.AreEqual(2.59, axlegearDataProvider.Ratio);
			Assert.AreEqual("0.00", lossMapData.Rows[0][0]);
			Assert.AreEqual("-5000.00", lossMapData.Rows[0][1]);
			Assert.AreEqual("115.00", lossMapData.Rows[0][2]);

			var lossMap = TransmissionLossMapReader.Create(lossMapData, axlegearDataProvider.Ratio, "AxleGear");
			Assert.IsNotNull(lossMap);

			AssertHelper.Exception<VectoException>(() => {
				var tmp = axlegearDataProvider.Efficiency;
			});
		}

		[TestCase]
		public void TestXMLInputRetarder()
		{
			var reader = XmlReader.Create(SampleVehicleDecl);

			var inputDataProvider = xmlInputReader.CreateDeclaration(reader);
			var retarderDataProvider = inputDataProvider.JobInputData.Vehicle.Components.RetarderInputData;

			Assert.AreEqual("Generic Retarder", retarderDataProvider.Model);

			var lossMapData = retarderDataProvider.LossMap;

			Assert.AreEqual(RetarderType.TransmissionOutputRetarder, retarderDataProvider.Type);

			Assert.AreEqual("0.00", lossMapData.Rows[0][0]);
			Assert.AreEqual("10.00", lossMapData.Rows[0][1]);

			var lossMap = RetarderLossMapReader.Create(lossMapData);
			Assert.IsNotNull(lossMap);
		}

		[Category("LongRunning")]
		[TestCase]
		public void TestXMLInputAxleWheels()
		{
			var reader = XmlReader.Create(SampleVehicleDecl);

			var inputDataProvider = xmlInputReader.CreateDeclaration(reader);
			var vehicleDataProvider = inputDataProvider.JobInputData.Vehicle;

			var axles = vehicleDataProvider.Components.AxleWheels.AxlesDeclaration;

			var tyre = axles[0].Tyre;
			Assert.AreEqual("315/70 R22.5", tyre.Dimension);
			Assert.AreEqual(0.0055, tyre.RollResistanceCoefficient);
			Assert.AreEqual(31300, tyre.TyreTestLoad.Value());
			Assert.AreEqual("e12*0815/8051*2017/05E0000*00", tyre.CertificationNumber);

			tyre = axles[1].Tyre;
			Assert.AreEqual("315/70 R22.5", tyre.Dimension);
			Assert.AreEqual(0.0063, tyre.RollResistanceCoefficient);
			Assert.AreEqual(31300, tyre.TyreTestLoad.Value());
			Assert.AreEqual("e12*0815/8051*2017/05E0000*00", tyre.CertificationNumber);
		}

		[TestCase]
		public void TestXMLInputAxleWheelsDuplicates()
		{
			var reader = XmlReader.Create(SampleVehicleDecl);

			var doc = new XmlDocument();
			doc.Load(reader);
			var nav = doc.CreateNavigator();
			var manager = new XmlNamespaceManager(nav.NameTable);
			var helper = new XPathHelper(ExecutionMode.Declaration);
			helper.AddNamespaces(manager);

			var firstAxle = nav.SelectSingleNode(helper.QueryAbs(
				helper.NSPrefix(XMLNames.VectoInputDeclaration, Constants.XML.RootNSPrefix),
				XMLNames.Component_Vehicle,
				XMLNames.Vehicle_Components,
				XMLNames.Component_AxleWheels,
				XMLNames.ComponentDataWrapper,
				XMLNames.AxleWheels_Axles,
				helper.QueryConstraint(XMLNames.AxleWheels_Axles_Axle, "1", null, string.Empty)
			), manager);
			firstAxle.MoveToAttribute(XMLNames.AxleWheels_Axles_Axle_AxleNumber_Attr, string.Empty);
			firstAxle.SetTypedValue(2);


			var modified = XmlReader.Create(new StringReader(nav.OuterXml));

			var inputDataProvider = xmlInputReader.CreateDeclaration(modified);
			var vehicleDataProvider = inputDataProvider.JobInputData.Vehicle;

			AssertHelper.Exception<VectoException>(() => {
				var axles = vehicleDataProvider.Components.AxleWheels.AxlesDeclaration;
			});
		}

		[TestCase]
		public void TestXMLInputAxleWheelsAxleNumTooLow()
		{
			var reader = XmlReader.Create(SampleVehicleDecl);

			var doc = new XmlDocument();
			doc.Load(reader);
			var nav = doc.CreateNavigator();
			var manager = new XmlNamespaceManager(nav.NameTable);
			var helper = new XPathHelper(ExecutionMode.Declaration);
			helper.AddNamespaces(manager);

			var firstAxle = nav.SelectSingleNode(helper.QueryAbs(
				helper.NSPrefix(XMLNames.VectoInputDeclaration, Constants.XML.RootNSPrefix),
				XMLNames.Component_Vehicle,
				XMLNames.Vehicle_Components,
				XMLNames.Component_AxleWheels,
				XMLNames.ComponentDataWrapper,
				XMLNames.AxleWheels_Axles,
				helper.QueryConstraint(XMLNames.AxleWheels_Axles_Axle, "1", null, string.Empty)
			), manager);
			firstAxle.MoveToAttribute(XMLNames.AxleWheels_Axles_Axle_AxleNumber_Attr, string.Empty);
			firstAxle.SetTypedValue(0);


			var modified = XmlReader.Create(new StringReader(nav.OuterXml));

			AssertHelper.Exception<VectoException>(
				() => {
					var inputDataProvider = xmlInputReader.CreateDeclaration(modified);
				});
		}

		[Category("LongRunning")]
		[TestCase]
		public void TestXMLInputAxleWheelsAxleNumTooHigh()
		{
			var reader = XmlReader.Create(SampleVehicleDecl);

			var doc = new XmlDocument();
			doc.Load(reader);
			var nav = doc.CreateNavigator();
			var manager = new XmlNamespaceManager(nav.NameTable);
			var helper = new XPathHelper(ExecutionMode.Declaration);
			helper.AddNamespaces(manager);

			var firstAxle = nav.SelectSingleNode(helper.QueryAbs(
				helper.NSPrefix(XMLNames.VectoInputDeclaration, Constants.XML.RootNSPrefix),
				XMLNames.Component_Vehicle,
				XMLNames.Vehicle_Components,
				XMLNames.Component_AxleWheels,
				XMLNames.ComponentDataWrapper,
				XMLNames.AxleWheels_Axles,
				helper.QueryConstraint(XMLNames.AxleWheels_Axles_Axle, "1", null, string.Empty)
			), manager);
			firstAxle.MoveToAttribute(XMLNames.AxleWheels_Axles_Axle_AxleNumber_Attr, string.Empty);
			firstAxle.SetTypedValue(3);


			var modified = XmlReader.Create(new StringReader(nav.OuterXml));

			var inputDataProvider = xmlInputReader.CreateDeclaration(modified);
			var vehicleDataProvider = inputDataProvider.JobInputData.Vehicle;

			AssertHelper.Exception<VectoException>(() => {
				var axles = vehicleDataProvider.Components.AxleWheels.AxlesDeclaration;
			});
		}

		[TestCase]
		public void TestXMLInputAuxiliaries()
		{
			var reader = XmlReader.Create(SampleVehicleDecl);

			var inputDataProvider = xmlInputReader.CreateDeclaration(reader);
			var auxDataProvider = inputDataProvider.JobInputData.Vehicle.Components.AuxiliaryInputData;

			var aux = auxDataProvider.Auxiliaries;
			var aux1 = aux[0];

			Assert.AreEqual(AuxiliaryType.Fan, aux1.Type);
			Assert.AreEqual("Hydraulic driven - Constant displacement pump", aux1.Technology.First());

			var aux3 = aux[2];
			Assert.AreEqual(AuxiliaryType.ElectricSystem, aux3.Type);
			Assert.AreEqual("Standard technology - LED headlights, all", aux3.Technology.First());
		}


		[TestCase]
		public void TestVehicleInput()
		{
			var reader = XmlReader.Create(SampleVehicleDecl);

			var inputDataProvider = xmlInputReader.CreateDeclaration(reader);

			var vehicleDataProvider = inputDataProvider.JobInputData.Vehicle;

			Assert.AreEqual(VehicleCategory.Tractor, vehicleDataProvider.VehicleCategory);
			Assert.AreEqual(AxleConfiguration.AxleConfig_4x2, vehicleDataProvider.AxleConfiguration);

			Assert.AreEqual(7100.0, vehicleDataProvider.CurbMassChassis.Value());
			Assert.AreEqual(40000.0, vehicleDataProvider.GrossVehicleMassRating.Value());
			Assert.AreEqual(6.34, inputDataProvider.JobInputData.Vehicle.Components.AirdragInputData.AirDragArea.Value());

			Assert.AreEqual(1.0, inputDataProvider.JobInputData.Vehicle.Components.RetarderInputData.Ratio);
		}

		[TestCase]
		public void TestVehicleInputNoAirdrag()
		{
			var reader = XmlReader.Create(SampleVehicleDeclNoAirdrag);

			var inputDataProvider = xmlInputReader.CreateDeclaration(reader);

			Assert.IsNull(inputDataProvider.JobInputData.Vehicle.Components.AirdragInputData.AirDragArea);
		}

		[Category("LongRunning")]
		[TestCase]
		public void TestXMLPowertrainGeneration()
		{
			var reader = XmlReader.Create(SampleVehicleDecl);

			var fileWriter = new FileOutputWriter("foo");
			var sumWriter = new FileOutputWriter("vecto_vehicle-sample_xml");
			var sumData = new SummaryDataContainer(sumWriter);
			var jobContainer = new JobContainer(sumData);
			var dataProvider = xmlInputReader.CreateDeclaration(reader);

			var runsFactory = SimulatorFactory.CreateSimulatorFactory(ExecutionMode.Declaration, dataProvider, fileWriter);
			runsFactory.WriteModalResults = true;

			jobContainer.AddRuns(runsFactory);

			Assert.AreEqual(10, jobContainer.Runs.Count);
		}

		[TestCase]
		public void TestFullFeaturedXMEngineering_TorqueConverter()
		{
			var reader = XmlReader.Create(SampleVehicleFullDecl);

			var inputDataProvider = xmlInputReader.CreateDeclaration(reader);

			var tcDataProvider = inputDataProvider.JobInputData.Vehicle.Components.TorqueConverterInputData;


			Assert.AreEqual(3, tcDataProvider.TCData.Rows.Count);
			Assert.AreEqual("300.00", tcDataProvider.TCData.Rows[0][2]);
			Assert.AreEqual("0.90", tcDataProvider.TCData.Rows[2][1]);
		}

		[TestCase]
		public void TestFullFeaturedXMLDeclaration_AngleDrive()
		{
			var reader = XmlReader.Create(SampleVehicleFullDecl);

			var inputDataProvider = xmlInputReader.CreateDeclaration(reader);


			var angledriveDataProvider = inputDataProvider.JobInputData.Vehicle.Components.AngledriveInputData;

			Assert.AreEqual(2.345, angledriveDataProvider.Ratio);
			Assert.AreEqual(6, angledriveDataProvider.LossMap.Rows.Count);
			Assert.AreEqual("-10000.00", angledriveDataProvider.LossMap.Rows[0][1]);

			AssertHelper.Exception<VectoException>(() => {
				var tmp = angledriveDataProvider.Efficiency;
			});
		}

		[TestCase]
		public void TestVehicleInputData()
		{
			var reader = XmlReader.Create(SampleVehicleFullDecl);

			var inputDataProvider = xmlInputReader.CreateDeclaration(reader);
			var vehicleDataProvider = inputDataProvider.JobInputData.Vehicle;

			Assert.AreEqual(VehicleCategory.Tractor, vehicleDataProvider.VehicleCategory);
			Assert.AreEqual(AxleConfiguration.AxleConfig_4x2, vehicleDataProvider.AxleConfiguration);

			Assert.AreEqual(7100.0, vehicleDataProvider.CurbMassChassis.Value());
			Assert.AreEqual(40000.0, vehicleDataProvider.GrossVehicleMassRating.Value());
			Assert.AreEqual(6.34, inputDataProvider.JobInputData.Vehicle.Components.AirdragInputData.AirDragArea.Value());

			Assert.AreEqual(1.0, inputDataProvider.JobInputData.Vehicle.Components.RetarderInputData.Ratio);
		}

		[TestCase]
		public void TestFullFeaturedXMLDeclaration_TorqueLimits()
		{
			var reader = XmlReader.Create(SampleVehicleFullDecl);

			var inputDataProvider = xmlInputReader.CreateDeclaration(reader);
			var vehicleDataProvider = inputDataProvider.JobInputData.Vehicle;

			var torqueLimits = vehicleDataProvider.TorqueLimits.OrderBy(x => x.Gear).ToList();
			Assert.AreEqual(3, torqueLimits.Count);
			Assert.AreEqual(1, torqueLimits[0].Gear);
			Assert.AreEqual(2500, torqueLimits[0].MaxTorque.Value());
			Assert.AreEqual(12, torqueLimits[2].Gear);
		}

		[TestCase]
		public void TestFullFeaturedXMLDeclaration_GbxTorqueLimits()
		{
			var reader = XmlReader.Create(SampleVehicleFullDecl);

			var inputDataProvider = xmlInputReader.CreateDeclaration(reader);
			var gearboxDataProvider = inputDataProvider.JobInputData.Vehicle.Components.GearboxInputData;
			var gears = gearboxDataProvider.Gears;

			Assert.AreEqual(12, gears.Count);
			Assert.AreEqual(1900, gears[0].MaxTorque.Value());
			Assert.AreEqual(1900, gears[1].MaxTorque.Value());
			Assert.IsNull(gears[11].MaxTorque);
		}

		[TestCase]
		public void TestFullFeaturedXMLDeclaration_GbxSpeedLimits()
		{
			var reader = XmlReader.Create(SampleVehicleFullDecl);

			var inputDataProvider = xmlInputReader.CreateDeclaration(reader);
			var gearboxDataProvider = inputDataProvider.JobInputData.Vehicle.Components.GearboxInputData;
			var gears = gearboxDataProvider.Gears;

			Assert.AreEqual(12, gears.Count);
			Assert.AreEqual(2000, gears[0].MaxInputSpeed.AsRPM, 1e-6);
			Assert.AreEqual(2000, gears[1].MaxInputSpeed.AsRPM, 1e-6);
			Assert.IsNull(gears[11].MaxInputSpeed);
		}

		[TestCase]
		public void TestElementNotAvailable()
		{
			var reader = XmlReader.Create(SampleVehicleDecl);

			var doc = new XmlDocument();
			doc.Load(reader);
			var nav = doc.CreateNavigator();
			var manager = new XmlNamespaceManager(nav.NameTable);
			var helper = new XPathHelper(ExecutionMode.Declaration);
			helper.AddNamespaces(manager);

			var retarderRatio = nav.SelectSingleNode(helper.QueryAbs(
				helper.NSPrefix(XMLNames.VectoInputDeclaration, Constants.XML.RootNSPrefix),
				XMLNames.Component_Vehicle,
				XMLNames.Vehicle_RetarderRatio), manager);
			retarderRatio.DeleteSelf();

			var modified = XmlReader.Create(new StringReader(nav.OuterXml));

			var inputDataProvider = xmlInputReader.CreateDeclaration(modified);

			AssertHelper.Exception<VectoException>(() => {
				var tmp = inputDataProvider.JobInputData.Vehicle.Components.RetarderInputData.Ratio;
			});
		}

		[TestCase]
		public void TestRetarderTypeNone()
		{
			var reader = XmlReader.Create(SampleVehicleDecl);

			var doc = new XmlDocument();
			doc.Load(reader);
			var nav = doc.CreateNavigator();
			var manager = new XmlNamespaceManager(nav.NameTable);
			var helper = new XPathHelper(ExecutionMode.Declaration);
			helper.AddNamespaces(manager);

			var retarderType = nav.SelectSingleNode(helper.QueryAbs(
				helper.NSPrefix(XMLNames.VectoInputDeclaration, Constants.XML.RootNSPrefix),
				XMLNames.Component_Vehicle,
				XMLNames.Vehicle_RetarderType), manager);
			retarderType.SetValue("None");

			var retarder = nav.SelectSingleNode(helper.QueryAbs(
				helper.NSPrefix(XMLNames.VectoInputDeclaration, Constants.XML.RootNSPrefix),
				XMLNames.Component_Vehicle,
				XMLNames.Vehicle_Components,
				XMLNames.Component_Retarder), manager);
			retarder.DeleteSelf();

			var modified = XmlReader.Create(new StringReader(nav.OuterXml));

			var inputDataProvider = xmlInputReader.CreateDeclaration(modified);

			var factory = SimulatorFactory.CreateSimulatorFactory(ExecutionMode.Declaration, inputDataProvider, new FileOutputWriter("dummy"));
			var jobContainer = new JobContainer(null);
			jobContainer.AddRuns(factory);
			jobContainer.Execute();
		}

		[TestCase]
		public void TestRetarderTypes()
		{
			var retarderTypes = GetEnumOptions("RetarderTypeType", "1.0");
			Assert.IsTrue(retarderTypes.Length > 0);

			foreach (var retarderType in retarderTypes) {
				var reader = XmlReader.Create(SampleVehicleDecl);

				var doc = new XmlDocument();
				doc.Load(reader);
				var nav = doc.CreateNavigator();
				var manager = new XmlNamespaceManager(nav.NameTable);
				var helper = new XPathHelper(ExecutionMode.Declaration);
				helper.AddNamespaces(manager);

				var xmlRetarderType = nav.SelectSingleNode(helper.QueryAbs(
					helper.NSPrefix(XMLNames.VectoInputDeclaration, Constants.XML.RootNSPrefix),
					XMLNames.Component_Vehicle,
					XMLNames.Vehicle_RetarderType), manager);
				xmlRetarderType.SetValue(retarderType);

				var modified = XmlReader.Create(new StringReader(nav.OuterXml));

				var inputDataProvider = xmlInputReader.CreateDeclaration(modified);

				Assert.AreEqual(retarderType, inputDataProvider.JobInputData.Vehicle.Components.RetarderInputData.Type.ToXMLFormat());
			}
		}

		[TestCase]
		public void TestAxleConfigurationTypes()
		{
			var axleConfigurations = GetEnumOptions("VehicleAxleConfigurationDeclarationType", "1.0");
			foreach (var axleConfiguration in axleConfigurations) {
				var reader = XmlReader.Create(SampleVehicleDecl);

				var doc = new XmlDocument();
				doc.Load(reader);
				var nav = doc.CreateNavigator();
				var manager = new XmlNamespaceManager(nav.NameTable);
				var helper = new XPathHelper(ExecutionMode.Declaration);
				helper.AddNamespaces(manager);

				var xmlAxleConf = nav.SelectSingleNode(helper.QueryAbs(
					helper.NSPrefix(XMLNames.VectoInputDeclaration, Constants.XML.RootNSPrefix),
					XMLNames.Component_Vehicle,
					XMLNames.Vehicle_AxleConfiguration), manager);
				xmlAxleConf.SetValue(axleConfiguration);

				var modified = XmlReader.Create(new StringReader(nav.OuterXml));

				var inputDataProvider = xmlInputReader.CreateDeclaration(modified);

				Assert.AreEqual(axleConfiguration, inputDataProvider.JobInputData.Vehicle.AxleConfiguration.GetName());
			}
		}

		[TestCase]
		public void TestVehicleCategoryTypes()
		{
			var vehicleCategories = GetEnumOptions("VehicleCategoryDeclarationType", "1.0");
			var allowedCategories = DeclarationData.TruckSegments.GetVehicleCategories();
			foreach (var vehicleCategory in vehicleCategories) {
				if (vehicleCategory.Equals("Rigid Truck")) {
					continue; // Rigid Truck has been renamed to Rigid Lorry. The XML contains this entry for backward compatibility (separate testcase)
				}
				var reader = XmlReader.Create(SampleVehicleDecl);

				var doc = new XmlDocument();
				doc.Load(reader);
				var nav = doc.CreateNavigator();
				var manager = new XmlNamespaceManager(nav.NameTable);
				var helper = new XPathHelper(ExecutionMode.Declaration);
				helper.AddNamespaces(manager);

				var xmlRetarderType = nav.SelectSingleNode(helper.QueryAbs(
					helper.NSPrefix(XMLNames.VectoInputDeclaration, Constants.XML.RootNSPrefix),
					XMLNames.Component_Vehicle,
					XMLNames.Vehicle_VehicleCategory), manager);
				xmlRetarderType.SetValue(vehicleCategory);

				var modified = XmlReader.Create(new StringReader(nav.OuterXml));

				var inputDataProvider = xmlInputReader.CreateDeclaration(modified);

				var vehCategory = inputDataProvider.JobInputData.Vehicle.VehicleCategory;
				Assert.AreEqual(vehicleCategory, vehCategory.ToXMLFormat());
				Assert.IsTrue(allowedCategories.Contains(vehCategory));
			}
		}


		[TestCase]
		public void TestWheelsTypes()
		{
			var wheelDimensions = GetEnumOptions("TyreDimensionType", "1.0");
			foreach (var wheelDimension in wheelDimensions) {
				var reader = XmlReader.Create(SampleVehicleDecl);

				var doc = new XmlDocument();
				doc.Load(reader);
				var nav = doc.CreateNavigator();
				var manager = new XmlNamespaceManager(nav.NameTable);
				var helper = new XPathHelper(ExecutionMode.Declaration);
				helper.AddNamespaces(manager);

				var tyredimensionNode = nav.SelectSingleNode(helper.QueryAbs(
						helper.NSPrefix(XMLNames.VectoInputDeclaration,
							Constants.XML.RootNSPrefix),
						XMLNames.Component_Vehicle, XMLNames.Vehicle_Components, XMLNames.Component_AxleWheels,
						XMLNames.ComponentDataWrapper,
						XMLNames.AxleWheels_Axles, XMLNames.AxleWheels_Axles_Axle, XMLNames.AxleWheels_Axles_Axle_Tyre,
						XMLNames.ComponentDataWrapper, XMLNames.AxleWheels_Axles_Axle_Dimension),
					manager);
				tyredimensionNode.SetValue(wheelDimension);

				var modified = XmlReader.Create(new StringReader(nav.OuterXml));

				var inputDataProvider = xmlInputReader.CreateDeclaration(modified);

				var tyreDimension = inputDataProvider.JobInputData.Vehicle.Components.AxleWheels.AxlesDeclaration.First().Tyre.Dimension;
				Assert.AreEqual(wheelDimension, tyreDimension);
				Assert.IsTrue(DeclarationData.Wheels.GetWheelsDimensions().Contains(tyreDimension), "Unknown tyre dimension {0}",
					tyreDimension);
			}
		}

		public const string SampleTyreXML = @"TestData/XML/XMLReaderDeclaration/SchemaVersion2.5/TyreSample.xml";

		[TestCase(),
		Category(Definitions.TESTCASE_MIGRATED)]
		public void TestWheelsSupportedInXML()
		{
			var tyreDimensions = DeclarationData.Wheels.GetWheelsDimensions();
			foreach (var tyreDimension in tyreDimensions) {
				var reader = XmlReader.Create(SampleTyreXML);
				var doc = new XmlDocument();
				doc.Load(reader);
				var nav = doc.CreateNavigator();
				var manager = new XmlNamespaceManager(nav.NameTable);
				var helper = new XPathHelper(ExecutionMode.Declaration);
				helper.AddNamespaces(manager);
				var tyredimensionNode = nav.SelectSingleNode(XMLHelper.QueryLocalName(XMLNames.AxleWheels_Axles_Axle_Dimension));
				tyredimensionNode.SetValue(tyreDimension);

				var modified = XmlReader.Create(new StringReader(nav.OuterXml));
				var validator = new XMLValidator(modified, null, XMLValidator.CallBackExceptionOnError);
				var valid = validator.ValidateXML(XmlDocumentType.DeclarationComponentData);
				Assert.IsTrue(valid, $"error validating XML with dimension {tyreDimension}");
			}
		}

		[
		TestCase("285/55R16C"), // invalid space
		//TestCase("285/55  R16C"), // allowed, as xs:token already combines multiple whitespaces
		TestCase("85/55 R16C"), // invalid section width
		TestCase("285/1231 R16C"), // invalid aspect ratio width
		TestCase("285/55 X16C"), // invalid construction type
		TestCase("85/55 R16112C"), // invalid rim diameter
		TestCase("85/55 R16x"), // invalid suffix
		TestCase("1.0001 R12"), // 
		TestCase("9 R12111"), // invalid rim diameter
		TestCase("9 R12x"), // invalid suffix
		TestCase("9R12"), // invalid space
		//TestCase("9  R12"), // allowed, as xs:token already combines multiple whitespaces
		Category(Definitions.TESTCASE_MIGRATED)
		]
		public void TestInvalidWheelsDimensionString(string dim)
		{
			var reader = XmlReader.Create(SampleTyreXML);
			var doc = new XmlDocument();
			doc.Load(reader);
			var nav = doc.CreateNavigator();
			var manager = new XmlNamespaceManager(nav.NameTable);
			var helper = new XPathHelper(ExecutionMode.Declaration);
			helper.AddNamespaces(manager);
			var tyredimensionNode = nav.SelectSingleNode(XMLHelper.QueryLocalName(XMLNames.AxleWheels_Axles_Axle_Dimension));
			tyredimensionNode.SetValue(dim);

			var modified = XmlReader.Create(new StringReader(nav.OuterXml));
			var validator = new XMLValidator(modified, null, XMLValidator.CallBackExceptionOnError);
			AssertHelper.Exception<VectoException>(() => validator.ValidateXML(XmlDocumentType.DeclarationComponentData), messageContains: "Validation error:");
		}


		[TestCase]
		public void TestPTOTypeTypes()
		{
			var ptoGearWheels = GetEnumOptions("PTOShaftsGearWheelsType", "1.0");
			var ptoOthers = GetEnumOptions("PTOOtherElementsType", "1.0");

			foreach (var ptoGearWheel in ptoGearWheels) {
				foreach (var ptoOther in ptoOthers) {
					if (ptoGearWheel == "none" || ptoGearWheel == "only one engaged gearwheel above oil level") {
						if (ptoOther != "none") {
							continue;
						}
					} else {
						if (ptoOther == "none") {
							continue;
						}
					}
					var reader = XmlReader.Create(SampleVehicleDecl);

					var doc = new XmlDocument();
					doc.Load(reader);
					var nav = doc.CreateNavigator();
					var manager = new XmlNamespaceManager(nav.NameTable);
					var helper = new XPathHelper(ExecutionMode.Declaration);
					helper.AddNamespaces(manager);

					var ptoGearWheelsNode = nav.SelectSingleNode(helper.QueryAbs(
							helper.NSPrefix(XMLNames.VectoInputDeclaration,
								Constants.XML.RootNSPrefix),
							XMLNames.Component_Vehicle,
							XMLNames.Vehicle_PTO,
							XMLNames.Vehicle_PTO_ShaftsGearWheels),
						manager);
					ptoGearWheelsNode.SetValue(ptoGearWheel);
					var ptoOtherNode = nav.SelectSingleNode(helper.QueryAbs(
							helper.NSPrefix(XMLNames.VectoInputDeclaration,
								Constants.XML.RootNSPrefix),
							XMLNames.Component_Vehicle,
							XMLNames.Vehicle_PTO,
							XMLNames.Vehicle_PTO_OtherElements),
						manager);
					ptoOtherNode.SetValue(ptoOther);

					var modified = XmlReader.Create(new StringReader(nav.OuterXml));

					var inputDataProvider = xmlInputReader.CreateDeclaration(modified);

					if (ptoGearWheel == "none") {
						Assert.AreEqual("None",
							inputDataProvider.JobInputData.Vehicle.Components.PTOTransmissionInputData.PTOTransmissionType);
					} else if (ptoGearWheel == "only one engaged gearwheel above oil level") {
						Assert.AreEqual(ptoGearWheel,
							inputDataProvider.JobInputData.Vehicle.Components.PTOTransmissionInputData.PTOTransmissionType);
					} else {
						Assert.AreEqual($"{ptoGearWheel} - {ptoOther}",
							inputDataProvider.JobInputData.Vehicle.Components.PTOTransmissionInputData.PTOTransmissionType);
					}
					Assert.NotNull(DeclarationData.PTOTransmission.Lookup(inputDataProvider.JobInputData.Vehicle.Components
						.PTOTransmissionInputData
						.PTOTransmissionType));
				}
			}
		}

		[TestCase]
		public void TestAngledriveTypes()
		{
			var angledriveTypes = GetEnumOptions("AngledriveTypeType", "1.0");
			foreach (var angleDrive in angledriveTypes) {
				var reader = XmlReader.Create(SampleVehicleDecl);

				var doc = new XmlDocument();
				doc.Load(reader);
				var nav = doc.CreateNavigator();
				var manager = new XmlNamespaceManager(nav.NameTable);
				var helper = new XPathHelper(ExecutionMode.Declaration);
				helper.AddNamespaces(manager);

				var angledriveNode = nav.SelectSingleNode(helper.QueryAbs(
						helper.NSPrefix(XMLNames.VectoInputDeclaration,
							Constants.XML.RootNSPrefix),
						XMLNames.Component_Vehicle,
						XMLNames.Vehicle_AngledriveType),
					manager);
				angledriveNode.SetValue(angleDrive);

				var modified = XmlReader.Create(new StringReader(nav.OuterXml));

				var inputDataProvider = xmlInputReader.CreateDeclaration(modified);

				Assert.AreEqual(angleDrive, inputDataProvider.JobInputData.Vehicle.Components.AngledriveInputData.Type.ToXMLFormat());
			}
		}

		[TestCase]
		public void TestGearboxTypes()
		{
			var gearboxTypes = GetEnumOptions("GearboxTransmissionTypeType", "1.0");
			foreach (var gearboxType in gearboxTypes) {
				var reader = XmlReader.Create(SampleVehicleDecl);

				var doc = new XmlDocument();
				doc.Load(reader);
				var nav = doc.CreateNavigator();
				var manager = new XmlNamespaceManager(nav.NameTable);
				var helper = new XPathHelper(ExecutionMode.Declaration);
				helper.AddNamespaces(manager);

				var xmlRetarderType = nav.SelectSingleNode(helper.QueryAbs(
						helper.NSPrefix(XMLNames.VectoInputDeclaration,
							Constants.XML.RootNSPrefix),
						XMLNames.Component_Vehicle,
						XMLNames.Vehicle_Components,
						XMLNames.Component_Gearbox,
						XMLNames.ComponentDataWrapper,
						XMLNames.Gearbox_TransmissionType),
					manager);
				xmlRetarderType.SetValue(gearboxType);

				var modified = XmlReader.Create(new StringReader(nav.OuterXml));

				var inputDataProvider = xmlInputReader.CreateDeclaration(modified);

				var gbxType = inputDataProvider.JobInputData.Vehicle.Components.GearboxInputData.Type;
				Assert.AreEqual(gearboxType, gbxType.ToXMLFormat());
				Assert.IsTrue(DeclarationDataAdapterHeavyLorry.Conventional.SupportsGearboxTypes.Contains(gbxType));
			}
		}

		[TestCase]
		public void TestPTOInputNone()
		{
			var reader = XmlReader.Create(SampleVehicleDecl);

			var inputDataProvider = xmlInputReader.CreateDeclaration(reader);
			var ptoDataProvider = inputDataProvider.JobInputData.Vehicle.Components.PTOTransmissionInputData;

			Assert.AreEqual("None", ptoDataProvider.PTOTransmissionType);
		}

		[TestCase]
		public void TestPTOInput()
		{
			var reader = XmlReader.Create(SampleVehicleFullDecl);

			var inputDataProvider = xmlInputReader.CreateDeclaration(reader);
			var ptoDataProvider = inputDataProvider.JobInputData.Vehicle.Components.PTOTransmissionInputData;
			var lookup = DeclarationData.PTOTransmission.Lookup(ptoDataProvider.PTOTransmissionType);

			Assert.AreEqual("only the drive shaft of the PTO - multi-disc clutch", ptoDataProvider.PTOTransmissionType);
			Assert.AreEqual(350, lookup.PowerDemand.Value());
		}

		[TestCase]
		public void TestCertificationMethodInput()
		{
			var reader = XmlReader.Create(SampleVehicleFullDeclCertificationOptions);

			var inputDataProvider = xmlInputReader.CreateDeclaration(reader);

			Assert.AreEqual(CertificationMethod.Option2,
				inputDataProvider.JobInputData.Vehicle.Components.GearboxInputData.CertificationMethod);
			Assert.AreEqual(CertificationMethod.Measured,
				inputDataProvider.JobInputData.Vehicle.Components.AxleGearInputData.CertificationMethod);
			Assert.AreEqual(CertificationMethod.Measured,
				inputDataProvider.JobInputData.Vehicle.Components.RetarderInputData.CertificationMethod);
			Assert.AreEqual(CertificationMethod.Measured,
				inputDataProvider.JobInputData.Vehicle.Components.AirdragInputData.CertificationMethod);
		}

		[TestCase]
		public void TestAuxFanTechTypes()
		{
			TestAuxTech(AuxiliaryType.Fan, GetEnumOptions("AuxFanTechnologyType", "1.0"), DeclarationData.Fan);
		}

		[TestCase]
		public void TestAuxElectricSystemTechTypes()
		{
			TestAuxTech(AuxiliaryType.ElectricSystem, GetEnumOptions("AuxESTechnologyType", "1.0"),
				DeclarationData.ElectricSystem);
		}

		[TestCase]
		public void TestAuxSteeringPumpTechTypes()
		{
			TestAuxTech(AuxiliaryType.SteeringPump, GetEnumOptions("AuxSPTechnologyType", "1.0"), DeclarationData.SteeringPump);
		}

		[TestCase]
		public void TestAuxPneumaticSystemTechTypes()
		{
			TestAuxTech(AuxiliaryType.PneumaticSystem, GetEnumOptions("AuxPSTechnologyType", "1.0"),
				DeclarationData.PneumaticSystem);
		}

		[TestCase]
		public void TestAuxHVACTechTypes()
		{
			TestAuxTech(AuxiliaryType.HVAC, GetEnumOptions("AuxHVACTechnologyType", "1.0"),
				DeclarationData.HeatingVentilationAirConditioning);
		}

		[TestCase(SampleVehicleDecl, false),
			TestCase(SampleVehicleFullDecl, false),
			TestCase(SampleVehicleFullDeclUpdated, false),
			TestCase(SampleVehicleFullDeclExempted, true)]
		public void TestReadingExemptedVehicles(string file, bool expectedExempted)
		{
			var reader = XmlReader.Create(file);

			var inputDataProvider = xmlInputReader.CreateDeclaration(reader);
			var vehicle = inputDataProvider.JobInputData.Vehicle;
			Assert.AreEqual(expectedExempted, vehicle.ExemptedVehicle);
		}

		[TestCase(SampleVehicleFullDeclExempted, true, true, true, 30000, 20000)]
		public void TestReadingExemptedParameters(
			string file, bool dualfuel, bool elHDV, bool zeroEmission, double maxNetPower1, double maxNetPower2)
		{
			var reader = XmlReader.Create(file);

			var inputDataProvider = xmlInputReader.CreateDeclaration(reader);
			var vehicle = inputDataProvider.JobInputData.Vehicle;

			Assert.IsTrue(vehicle.ExemptedVehicle);
			Assert.AreEqual(elHDV, vehicle.HybridElectricHDV);
			Assert.AreEqual(zeroEmission, vehicle.ZeroEmissionVehicle);
			Assert.AreEqual(dualfuel, vehicle.DualFuelVehicle);
			Assert.AreEqual(maxNetPower1, vehicle.MaxNetPower1.Value());

		}

		[TestCase(SampleVehicleFullDeclUpdated, true, false, true)]
		public void TestReadingNewVehicleParameters(string file, bool vocational, bool sleeperCab, bool zeroEmission)
		{
			var reader = XmlReader.Create(file);

			var inputDataProvider = xmlInputReader.CreateDeclaration(reader);
			var vehicle = inputDataProvider.JobInputData.Vehicle;

			Assert.IsFalse(vehicle.ExemptedVehicle);

			Assert.AreEqual(vocational, vehicle.VocationalVehicle);
			Assert.AreEqual(sleeperCab, vehicle.SleeperCab);
			Assert.AreEqual(zeroEmission, vehicle.ZeroEmissionVehicle);
		}

		[TestCase(SampleVehicleDecl, false, false, false, PredictiveCruiseControlType.None),
		TestCase(SampleVehicleFullDeclUpdated, true, false, true, PredictiveCruiseControlType.Option_1_2)]
		public void TestReadingAdasParameters(
			string file, bool engineStopStart, bool ecoRollWithout, bool ecoRollWith, PredictiveCruiseControlType pcc)
		{
			var reader = XmlReader.Create(file);

			var inputDataProvider = xmlInputReader.CreateDeclaration(reader);
			var vehicle = inputDataProvider.JobInputData.Vehicle;
			var adas = vehicle.ADAS;

			Assert.IsFalse(vehicle.ExemptedVehicle);

			Assert.AreEqual(engineStopStart, adas.EngineStopStart);
			Assert.AreEqual(ecoRollWith, adas.EcoRoll.WithEngineStop());
			Assert.AreEqual(ecoRollWithout, adas.EcoRoll.WithoutEngineStop());
			Assert.AreEqual(pcc, adas.PredictiveCruiseControl);
		}

		

		[TestCase(SampleVehicleFullDeclUpdated)]
		public void TestRigidTruckIsReadAsRigidLorry(string file)
		{
			var reader = XmlReader.Create(file);

			var doc = new XmlDocument();
			doc.Load(reader);
			var nav = doc.CreateNavigator();
			var manager = new XmlNamespaceManager(nav.NameTable);
			var helper = new XPathHelper(ExecutionMode.Declaration);
			helper.AddNamespaces(manager);

			var vehicleCategoryNode = nav.SelectSingleNode(helper.QueryAbs(
															helper.NSPrefix(XMLNames.VectoInputDeclaration, Constants.XML.RootNSPrefix),
															XMLNames.Component_Vehicle,
															XMLNames.Vehicle_VehicleCategory), manager);
			vehicleCategoryNode.SetValue("Rigid Truck");

			var modified = XmlReader.Create(new StringReader(nav.OuterXml));

			var inputDataProvider = xmlInputReader.CreateDeclaration(modified);

			var vehCategory = inputDataProvider.JobInputData.Vehicle.VehicleCategory;

			Assert.AreEqual(VehicleCategory.RigidTruck, vehCategory);
		}

		[TestCase(SampleVehicleFullDecl, "LPG", FuelType.LPGPI),
			TestCase(SampleVehicleDecl, "NG", FuelType.NGPI)]
		public void TestFuelTypesLNGandNGBackwardCompatibility(string file, string value, FuelType expectedFuelType)
		{
			var reader = XmlReader.Create(file);

			var doc = new XmlDocument();
			doc.Load(reader);
			var nav = doc.CreateNavigator();
			var manager = new XmlNamespaceManager(nav.NameTable);
			var helper = new XPathHelper(ExecutionMode.Declaration);
			helper.AddNamespaces(manager);

			var fuelTypeNode = nav.SelectSingleNode(
				helper.QueryAbs(
					helper.NSPrefix(
						XMLNames.VectoInputDeclaration,
						Constants.XML.RootNSPrefix),
					XMLNames.Component_Vehicle, XMLNames.Vehicle_Components, XMLNames.Component_Engine,
					XMLNames.ComponentDataWrapper,
					XMLNames.Engine_FuelType),
				manager);
			
			fuelTypeNode.SetValue(value);

			var modified = XmlReader.Create(new StringReader(nav.OuterXml));

			var inputDataProvider = xmlInputReader.CreateDeclaration(modified);

			var fuelType = inputDataProvider.JobInputData.Vehicle.Components.EngineInputData.EngineModes.First().Fuels.First().FuelType;

			Assert.AreEqual(expectedFuelType, fuelType);
		}

		public static string[] GetEnumOptions(string xmlType, string schemaVersion)
		{
			Stream resource;
			var schemaFile = $"VectoDeclarationDefinitions{"." + schemaVersion}.xsd";
			try {
				resource = RessourceHelper.LoadResourceAsStream(RessourceHelper.ResourceType.XMLSchema, schemaFile);
			} catch (Exception e) {
				throw new Exception($"Unknown XML schema! version: {schemaVersion}, xsd: {schemaFile}", e);
			}
			var reader = new XPathDocument(resource);
			var nav = reader.CreateNavigator();
			var nodes = nav.Select(
				$"//*[local-name()='simpleType' and @name='{xmlType}']//*[local-name()='enumeration']/@value");
			var retVal = new List<string>();
			foreach (var node in nodes) {
				retVal.Add(node.ToString());
			}
			return retVal.ToArray();
		}

		private void TestAuxTech(AuxiliaryType aux, string[] techs, IDeclarationAuxiliaryTable auxLookup)
		{
			foreach (var tech in techs) {
				var reader = XmlReader.Create(SampleVehicleDecl);

				var doc = new XmlDocument();
				doc.Load(reader);
				var nav = doc.CreateNavigator();
				var manager = new XmlNamespaceManager(nav.NameTable);
				var helper = new XPathHelper(ExecutionMode.Declaration);
				helper.AddNamespaces(manager);

				var technology = nav.SelectSingleNode(helper.QueryAbs(
						helper.NSPrefix(XMLNames.VectoInputDeclaration,
							Constants.XML.RootNSPrefix),
						XMLNames.Component_Vehicle,
						XMLNames.Vehicle_Components,
						XMLNames.Component_Auxiliaries, XMLNames.ComponentDataWrapper, aux.ToString(),
						XMLNames.Auxiliaries_Auxiliary_Technology),
					manager);
				technology.SetValue(tech);
				var modified = XmlReader.Create(new StringReader(nav.OuterXml));

				var inputDataProvider = xmlInputReader.CreateDeclaration(modified);
				var techInput = inputDataProvider.JobInputData.Vehicle.Components.AuxiliaryInputData.Auxiliaries.Where(x => x.Type == aux)
					.First().Technology.First();
				Assert.AreEqual(tech, techInput);

				Assert.IsTrue(auxLookup.GetTechnologies().Contains(techInput), "technology '{0}' for aux type '{1}' not known!",
					techInput, aux);
			}
		}

		[TestCase(@"TestData/XML/XMLReaderDeclaration/SchemaVersion2.3/vecto_vehicle-fullElectricSP.xml")]
		public void TestReadingNewSteeringPumpTechnologies(string file)
		{
			var reader = XmlReader.Create(file);

			var inputDataProvider = xmlInputReader.CreateDeclaration(reader);
			var vehicle = inputDataProvider.JobInputData.Vehicle;

			var steeringPump = vehicle.Components.AuxiliaryInputData.Auxiliaries.Where(x => x.Type == AuxiliaryType.SteeringPump)
									.FirstOrDefault();
			Assert.NotNull(steeringPump);

			Assert.AreEqual("Full electric steering gear", steeringPump.Technology[0]);
			Assert.AreEqual("Electric driven pump", steeringPump.Technology[1]);

			Assert.AreEqual(
				616.2, DeclarationData.SteeringPump.Lookup(MissionType.LongHaul, VehicleClass.Class5, steeringPump.Technology).electricPumps.Value());
		}
	}
}