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
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using Ninject;
using NUnit.Framework;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.FileIO.XML;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.Models.Simulation.Impl.SimulatorFactory;
using TUGraz.VectoCore.Ninject;
using TUGraz.VectoCore.OutputData.FileIO;
using TUGraz.VectoCore.Tests.Models.Simulation;
using TUGraz.VectoCore.Utils;
using XmlDocumentType = TUGraz.VectoCore.Utils.XmlDocumentType;

namespace TUGraz.VectoCore.Tests.Integration
{
	[TestFixture]
	[Parallelizable(ParallelScope.All)]
	public class ExemptedVehicleTest
	{
		//const string ExemptedVehicle = @"TestData/Integration/DeclarationMode/ExemptedVehicle/vecto_vehicle-sample_exempted.xml";
		//const string ExemptedVehicleNoHEV = @"TestData/Integration/DeclarationMode/ExemptedVehicle/vecto_vehicle-sample_exempted_nonHEV.xml";
		const string ExemptedVehicleV24 = @"TestData/Integration/DeclarationMode/ExemptedVehicle/vecto_vehicle-sample_exempted_v24.xml";
		const string ExemptedVehicleV24NoHEV = @"TestData/Integration/DeclarationMode/ExemptedVehicle/vecto_vehicle-sample_exempted_v24_nonHEV.xml";
		const string ExemptedVehicleV24InvisibleChar = @"TestData/Integration/DeclarationMode/ExemptedVehicle/vecto_vehicle-sample_exempted_v24_mupltiplePowertrains_invisibleChar.xml";

        const string ExemptedPrimaryBus = @"TestData/XML/XMLReaderDeclaration/SchemaVersion2.4/exempted_primary_heavyBus.xml";

		protected IXMLInputDataReader xmlInputReader;
		private IKernel _kernel;

		[OneTimeSetUp]
		public void RunBeforeAnyTests()
		{
			Directory.SetCurrentDirectory(TestContext.CurrentContext.TestDirectory);

			_kernel = new StandardKernel(new VectoNinjectModule());
			xmlInputReader = _kernel.Get<IXMLInputDataReader>();
		}

		[
		TestCase(ExemptedVehicleV24, 1),
		TestCase(ExemptedVehicleV24NoHEV, 1),
		TestCase(ExemptedVehicleV24InvisibleChar, 1),

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

			var factory = _kernel.Get<ISimulatorFactoryFactory>().Factory(ExecutionMode.Declaration, inputData, writer, null, null, false);
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

			var cif = XDocument.Load(XmlReader.Create(customerFile));
			var techNode = cif.XPathSelectElement("//*[local-name()='VehicleTechnologyExempted']");
			Assert.NotNull(techNode);
			var match = Regex.Match(techNode.Value, "^[a-zA-Z0-9 ]+$");
			Assert.IsTrue(match.Success);

			var mrf = XDocument.Load(XmlReader.Create(manufactuerFile));
			techNode = mrf.XPathSelectElement("//*[local-name()='VehicleTechnologyExempted']");
			Assert.NotNull(techNode);
			match = Regex.Match(techNode.Value, "^[a-zA-Z0-9 ]+$");
			Assert.IsTrue(match.Success);

            //var val3 = new XMLValidator(XmlReader.Create(monitoringFile));
            //Assert.IsTrue(val3.ValidateXML(XmlDocumentType.MonitoringReport), val3.ValidationError);

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

			var factory = _kernel.Get<ISimulatorFactoryFactory>().Factory(ExecutionMode.Declaration, inputData, writer, null, null, false);
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
