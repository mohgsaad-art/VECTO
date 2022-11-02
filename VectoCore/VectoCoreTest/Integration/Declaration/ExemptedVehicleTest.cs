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
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.XPath;
using Ninject;
using NUnit.Framework;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.InputData.FileIO.XML;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.Models.Simulation.Impl.SimulatorFactory;
using TUGraz.VectoCore.OutputData.FileIO;
using TUGraz.VectoCore.Tests.Models.Simulation;
using TUGraz.VectoCore.Tests.Utils;
using TUGraz.VectoCore.Utils;
using XmlDocumentType = TUGraz.VectoCore.Utils.XmlDocumentType;

namespace TUGraz.VectoCore.Tests.Integration
{
	[TestFixture]
	[Parallelizable(ParallelScope.All)]
	public class ExemptedVehicleTest
	{
		const string ExemptedVehicle = @"Testdata\Integration\DeclarationMode\ExemptedVehicle\vecto_vehicle-sample_exempted.xml";
		const string ExemptedVehicleNoHEV = @"Testdata\Integration\DeclarationMode\ExemptedVehicle\vecto_vehicle-sample_exempted_nonHEV.xml";
		const string ExemptedVehicleV2 = @"Testdata\Integration\DeclarationMode\ExemptedVehicle\vecto_vehicle-sample_exempted_v2.xml";
		const string ExemptedVehicleV2NoHEV = @"Testdata\Integration\DeclarationMode\ExemptedVehicle\vecto_vehicle-sample_exempted_v2_nonHEV.xml";

		const string ExemptedPrimaryBus = @"TestData\XML\XMLReaderDeclaration\SchemaVersion2.4\exempted_primary_heavyBus.xml";

		protected IXMLInputDataReader xmlInputReader;
		private IKernel _kernel;

		[OneTimeSetUp]
		public void RunBeforeAnyTests()
		{
			Directory.SetCurrentDirectory(TestContext.CurrentContext.TestDirectory);

			_kernel = new StandardKernel(new VectoNinjectModule());
			xmlInputReader = _kernel.Get<IXMLInputDataReader>();
		}

		[TestCase(ExemptedVehicle, 1),
		TestCase(ExemptedVehicleNoHEV, 1),
		TestCase(ExemptedVehicleV2, 1),
		TestCase(ExemptedVehicleV2NoHEV, 1)
		]
		public void TestSimulationExemptedVehicle(string filename, int numRuns)
		{
			var writer = new FileOutputWriter(filename);

			var customerFile = writer.XMLCustomerReportName;
			var manufactuerFile = writer.XMLFullReportName;
			var monitoringFile = writer.XMLMonitoringReportName;
			if (File.Exists(customerFile)) {
				File.Delete(customerFile);
			}
			if (File.Exists(manufactuerFile)) {
				File.Delete(manufactuerFile);
			}
			if (File.Exists(monitoringFile)) {
				File.Delete(monitoringFile);
			}

			var inputData = xmlInputReader.CreateDeclaration(filename);

			var factory = SimulatorFactory.CreateSimulatorFactory(ExecutionMode.Declaration, inputData, writer);
			factory.WriteModalResults = true;
			factory.ActualModalData = true;
			var jobContainer = new JobContainer(new MockSumWriter());
			jobContainer.AddRuns(factory);
			Assert.That(jobContainer.Runs.Count, Is.EqualTo(numRuns));

			jobContainer.Execute();
			jobContainer.WaitFinished();
			var progress = jobContainer.GetProgress();
			Assert.IsTrue(progress.All(r => r.Value.Success), string.Concat(progress.Select(r => r.Value.Error)));

			Assert.IsTrue(File.Exists(manufactuerFile));
			Assert.IsTrue(File.Exists(customerFile));

			var validator = new XMLValidator(XmlReader.Create(manufactuerFile));
			Assert.IsTrue(validator.ValidateXML(XmlDocumentType.ManufacturerReport), validator.ValidationError);

			var val2 = new XMLValidator(XmlReader.Create(customerFile));
			Assert.IsTrue(val2.ValidateXML(XmlDocumentType.CustomerReport), val2.ValidationError);

			//var val3 = new XMLValidator(XmlReader.Create(monitoringFile));
			//Assert.IsTrue(val3.ValidateXML(XmlDocumentType.MonitoringReport), val3.ValidationError);

		}

		[TestCase(ExemptedVehicle, true, true, true, "Invalid input: ZE-HDV and DualFuelVehicle are mutually exclusive!"),
			TestCase(ExemptedVehicle, true, false, true, "Invalid input: ZE-HDV and DualFuelVehicle are mutually exclusive!"),
			TestCase(ExemptedVehicle, false, false, false, "Invalid input: at least one option of ZE-HDV, He-HDV, and DualFuelVehicle has to be set for an exempted vehicle!")]
		public void TestInvalidExemptedCombination(string filename, bool zeroEmission, bool hybrid, bool dualFuel, string exMsg)
		{
			var writer = new FileOutputWriter(InputDataHelper.GetRandomFilename(filename));

			var customerFile = writer.XMLCustomerReportName;
			var manufactuerFile = writer.XMLFullReportName;
			var monitoringFile = writer.XMLMonitoringReportName;
			if (File.Exists(customerFile)) {
				File.Delete(customerFile);
			}
			if (File.Exists(manufactuerFile)) {
				File.Delete(manufactuerFile);
			}
			if (File.Exists(monitoringFile)) {
				File.Delete(monitoringFile);
			}

			var reader = XmlReader.Create(filename);

			var doc = new XmlDocument();
			doc.Load(reader);
			var nav = doc.CreateNavigator();

			SetExemptedParameters(nav, zeroEmission, hybrid, dualFuel);

			var modified = XmlReader.Create(new StringReader(nav.OuterXml));

			var inputData = xmlInputReader.CreateDeclaration(modified);
			
			var factory = SimulatorFactory.CreateSimulatorFactory(ExecutionMode.Declaration, inputData, writer);
			factory.WriteModalResults = true;
			factory.ActualModalData = true;
			var jobContainer = new JobContainer(new MockSumWriter());

			jobContainer.AddRuns(factory);

			AssertHelper.Exception<VectoException>(
				() => {
					jobContainer.Runs[0].Run.Run();
				},
				messageContains: exMsg);
			Assert.IsFalse(File.Exists(customerFile));
			Assert.IsFalse(File.Exists(manufactuerFile));
			Assert.IsFalse(File.Exists(monitoringFile));
		}

		


		[TestCase(ExemptedVehicle, null, 10000),
		TestCase(ExemptedVehicle, 100000, null),
		TestCase(ExemptedVehicle, null, null)]
		public void TestHybridExemptedRequiresMaxNetPower(string filename, double? maxNetPower1, double? maxNetPower2)
		{
			var writer = new FileOutputWriter(InputDataHelper.GetRandomFilename(filename));

			var customerFile = writer.XMLCustomerReportName;
			var manufactuerFile = writer.XMLFullReportName;
			var monitoringFile = writer.XMLMonitoringReportName;
			if (File.Exists(customerFile)) {
				File.Delete(customerFile);
			}
			if (File.Exists(manufactuerFile)) {
				File.Delete(manufactuerFile);
			}
			if (File.Exists(monitoringFile)) {
				File.Delete(monitoringFile);
			}

			var reader = XmlReader.Create(filename);

			var doc = new XmlDocument();
			doc.Load(reader);
			var nav = doc.CreateNavigator();
			var manager = new XmlNamespaceManager(nav.NameTable);
			var helper = new XPathHelper(ExecutionMode.Declaration);
			helper.AddNamespaces(manager);

			SetExemptedParameters(nav, false, true, false);
			var maxPower1 = nav.SelectSingleNode(helper.QueryAbs(
													helper.NSPrefix(XMLNames.VectoInputDeclaration,
																	Constants.XML.RootNSPrefix),
													XMLNames.Component_Vehicle,
													XMLNames.Vehicle_MaxNetPower1),
												manager);
			if (maxNetPower1.HasValue) {
				maxPower1.SetValue(maxNetPower1.Value.ToXMLFormat(0));
			} else {
				maxPower1.DeleteSelf();
			}
			var maxPower2 = nav.SelectSingleNode(helper.QueryAbs(
													helper.NSPrefix(XMLNames.VectoInputDeclaration,
																	Constants.XML.RootNSPrefix),
													XMLNames.Component_Vehicle,
													XMLNames.Vehicle_MaxNetPower2),
												manager);
			if (maxNetPower2.HasValue) {
				maxPower2.SetValue(maxNetPower2.Value.ToXMLFormat(0));
			} else {
				maxPower2.DeleteSelf();
			}

			var modified = XmlReader.Create(new StringReader(nav.OuterXml));

			var inputData = xmlInputReader.CreateDeclaration(modified);
			
			var factory = SimulatorFactory.CreateSimulatorFactory(ExecutionMode.Declaration, inputData, writer);
			factory.WriteModalResults = true;
			factory.ActualModalData = true;
			var jobContainer = new JobContainer(new MockSumWriter());

			jobContainer.AddRuns(factory);

			AssertHelper.Exception<VectoException>(
				() => {
					jobContainer.Runs[0].Run.Run();
				},
				messageContains: "For He-HDV both MaxNetPower1 and MaxNetPower2 have to be provided!");
			Assert.IsFalse(File.Exists(customerFile));
			Assert.IsFalse(File.Exists(manufactuerFile));
			Assert.IsFalse(File.Exists(monitoringFile));
		}



		private static void SetExemptedParameters(XPathNavigator nav, bool zeroEmission, bool hybrid, bool dualFuel)
		{
			var manager = new XmlNamespaceManager(nav.NameTable);
			var helper = new XPathHelper(ExecutionMode.Declaration);
			helper.AddNamespaces(manager);

			var zeNode = nav.SelectSingleNode(
				helper.QueryAbs(
					helper.NSPrefix(
						XMLNames.VectoInputDeclaration,
						Constants.XML.RootNSPrefix),
					XMLNames.Component_Vehicle,
					XMLNames.Vehicle_ZeroEmissionVehicle),
				manager);
			zeNode.SetValue(zeroEmission.ToString().ToLowerInvariant());

			var dualfuelNode = nav.SelectSingleNode(
				helper.QueryAbs(
					helper.NSPrefix(
						XMLNames.VectoInputDeclaration,
						Constants.XML.RootNSPrefix),
					XMLNames.Component_Vehicle,
					XMLNames.Vehicle_DualFuelVehicle),
				manager);
			dualfuelNode.SetValue(dualFuel.ToString().ToLowerInvariant());

			var hybridNode = nav.SelectSingleNode(
				helper.QueryAbs(
					helper.NSPrefix(
						XMLNames.VectoInputDeclaration,
						Constants.XML.RootNSPrefix),
					XMLNames.Component_Vehicle,
					XMLNames.Vehicle_HybridElectricHDV),
				manager);
			hybridNode.SetValue(hybrid.ToString().ToLowerInvariant());
		}

		[TestCase(ExemptedPrimaryBus, 1)]
		public void TestSimulationExemptedPrimaryBusVehicle(string filename, int numRuns)
		{
			var writer = new FileOutputWriter(filename);

			var primaryReportFile = writer.XMLPrimaryVehicleReportName;
			var manufactuerFile = writer.XMLFullReportName;
			var monitoringFile = writer.XMLMonitoringReportName;
			if (File.Exists(primaryReportFile)) {
				File.Delete(primaryReportFile);
			}
			if (File.Exists(manufactuerFile)) {
				File.Delete(manufactuerFile);
			}
			if (File.Exists(monitoringFile)) {
				File.Delete(monitoringFile);
			}

			var inputData = xmlInputReader.CreateDeclaration(filename);

			var factory = SimulatorFactory.CreateSimulatorFactory(ExecutionMode.Declaration, inputData, writer);
			factory.WriteModalResults = true;
			factory.ActualModalData = true;
			var jobContainer = new JobContainer(new MockSumWriter());

			var runs = factory.SimulationRuns().ToList();
			Assert.AreEqual(numRuns, runs.Count);
			foreach (var run in runs) {
				jobContainer.AddRun(run);
			}
			//jobContainer.AddRuns(factory);

			jobContainer.Execute();
			jobContainer.WaitFinished();
			var progress = jobContainer.GetProgress();
			Assert.IsTrue(progress.All(r => r.Value.Success), string.Concat<Exception>(progress.Select(r => r.Value.Error)));

			Assert.IsTrue(File.Exists(manufactuerFile));
			Assert.IsTrue(File.Exists(primaryReportFile));

			var validator = new XMLValidator(XmlReader.Create(manufactuerFile));
			Assert.IsTrue(validator.ValidateXML(XmlDocumentType.ManufacturerReport), validator.ValidationError);

			var val2 = new XMLValidator(XmlReader.Create(primaryReportFile));
			Assert.IsTrue(val2.ValidateXML(XmlDocumentType.MultistepOutputData), val2.ValidationError);

			//var val3 = new XMLValidator(XmlReader.Create(monitoringFile));
			//Assert.IsTrue(val3.ValidateXML(XmlDocumentType.MonitoringReport), val3.ValidationError);

		}
	}
}
