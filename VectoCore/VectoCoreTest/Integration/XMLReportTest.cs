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

using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using Ninject;
using NUnit.Framework;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.InputData.FileIO.XML;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.Models.Simulation.Impl.SimulatorFactory;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.OutputData.FileIO;
using TUGraz.VectoCore.OutputData.XML;
using TUGraz.VectoCore.Tests.Utils;
using TUGraz.VectoCore.Tests.XML;
using TUGraz.VectoCore.Utils;
using TUGraz.VectoHashing;
using XmlDocumentType = TUGraz.VectoCore.Utils.XmlDocumentType;


namespace TUGraz.VectoCore.Tests.Integration
{
	[TestFixture]
	[Parallelizable(ParallelScope.All)]
	public class XMLReportTest
	{
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
		public void TestXMLReportMetaInformation()
		{
			var jobfile = @"Testdata\XML\XMLReaderDeclaration\vecto_vehicle-sample.xml";
			var dataProvider = xmlInputReader.CreateDeclaration(jobfile);
			var writer = new FileOutputWriter(InputDataHelper.GetRandomFilename(jobfile));
			var xmlReport = new XMLDeclarationReport(writer);
			var sumData = new SummaryDataContainer(writer);
			var jobContainer = new JobContainer(sumData);

			if (File.Exists(writer.SumFileName)) {
				File.Delete(writer.SumFileName);
			}

			var runsFactory = SimulatorFactory.CreateSimulatorFactory(ExecutionMode.Declaration, dataProvider, writer,
				xmlReport, validate: false);
			runsFactory.WriteModalResults = false;
			//var runsFactory = new SimulatorFactory(ExecutionMode.Declaration, dataProvider, writer, xmlReport) {
			//	WriteModalResults = false,
			//	Validate = false,
			//};
			jobContainer.AddRuns(runsFactory);

			// no need to run the simulation, we only check whether the meta-data is correct, no results are considered
			//jobContainer.Execute();
			//jobContainer.WaitFinished();
			xmlReport.DoWriteReport();

			var manufacturerReport = xmlReport.FullReport;

			Assert.AreEqual(5, manufacturerReport.XPathSelectElement("//*[local-name()='VehicleGroup']")?.Value.ToInt());

			Assert.IsFalse(XmlConvert.ToBoolean(manufacturerReport.XPathSelectElement("//*[local-name()='PTO']").Value));

			var reportWheels = manufacturerReport.XPathSelectElements("//*[local-name()='TyreCertificationNumber']").ToList();
			Assert.AreEqual(dataProvider.JobInputData.Vehicle.Components.AxleWheels.AxlesDeclaration.Count, reportWheels.Count);

			var i = 0;
			foreach (var axleDeclarationInputData in dataProvider.JobInputData.Vehicle.Components.AxleWheels.AxlesDeclaration) {
				Assert.AreEqual(axleDeclarationInputData.Tyre.CertificationNumber, reportWheels[i++].Value);
			}

			var digestWheels = manufacturerReport.XPathSelectElements("//*[local-name()='Axle']/*[local-name()='DigestValue']").ToArray();
			Assert.NotNull(digestWheels);
			Assert.AreEqual(2, digestWheels.Count());
			foreach (var digestWheel in digestWheels) {
				Assert.IsFalse(string.IsNullOrWhiteSpace(digestWheel.Value));
			}
		}

		[TestCase(@"TestData\XML\XMLReaderDeclaration\GroupTest\Rigid Truck_4x2_vehicle-class-1_EURO6_2018.xml"),
		//TestCase(@"TestData\XML\XMLReaderDeclaration\GroupTest\Rigid Truck_4x2_vehicle-class-2_EURO6_2018.xml"),
		//TestCase(@"TestData\XML\XMLReaderDeclaration\GroupTest\Rigid Truck_4x2_vehicle-class-3_EURO6_2018.xml"),
		//TestCase(@"TestData\XML\XMLReaderDeclaration\GroupTest\Rigid Truck_4x2_vehicle-class-4_EURO6_2018.xml"),
		TestCase(@"TestData\XML\XMLReaderDeclaration\GroupTest\Tractor_4x2_vehicle-class-5_EURO6_2018.xml"),
		//TestCase(@"TestData\XML\XMLReaderDeclaration\GroupTest\Rigid Truck_6x2_vehicle-class-9_EURO6_2018.xml"),
		//TestCase(@"TestData\XML\XMLReaderDeclaration\GroupTest\Tractor_6x2_vehicle-class-10_EURO6_2018.xml"),
		//TestCase(@"TestData\XML\XMLReaderDeclaration\GroupTest\Rigid Truck_6x4_vehicle-class-11_EURO6_2018.xml"),
		//TestCase(@"TestData\XML\XMLReaderDeclaration\GroupTest\Tractor_6x4_vehicle-class-12_EURO6_2018.xml"),
		TestCase(@"TestData\XML\XMLReaderDeclaration\GroupTest\Rigid Truck_8x4_vehicle-class-16_EURO6_2018.xml")]
        public void TestXMLSummaryReportExists(string jobfile)
        {
			var dataProvider = xmlInputReader.CreateDeclaration(jobfile);
            var writer = new FileOutputWriter(jobfile);
            var xmlReport = new XMLDeclarationReport(writer);
            var sumData = new SummaryDataContainer(writer);
            var jobContainer = new JobContainer(sumData);

			var runsFactory = SimulatorFactory.CreateSimulatorFactory(ExecutionMode.Declaration, dataProvider, writer,
				xmlReport, validate: false);
			runsFactory.WriteModalResults = false;
			//var runsFactory = new SimulatorFactory(ExecutionMode.Declaration, dataProvider, writer, xmlReport)
   //         {
   //             WriteModalResults = false,
   //             Validate = false,
   //         };
            jobContainer.AddRuns(runsFactory);

            // no need to run the simulation, we only check whether the meta-data is correct, no results are considered
            jobContainer.Execute();
            jobContainer.WaitFinished();
            xmlReport.DoWriteReport();

			var customerReport = xmlReport.CustomerReport;

            //check if the customerReport contains the summary XML-Element
			Assert.AreNotEqual(null,customerReport.XPathSelectElement("//*[local-name()='Summary']"));
		}

		[TestCase(@"Testdata\XML\XMLReaderDeclaration\vecto_vehicle-sample.xml"),
		 TestCase(@"TestData\Integration\DeclarationMode\ExemptedVehicle\vecto_vehicle-sample_exempted.xml")]
		public void TestValidationXMLReports(string jobfile)
		{
			var dataProvider = xmlInputReader.CreateDeclaration(jobfile);
			var writer = new FileOutputWriter(InputDataHelper.GetRandomFilename(jobfile));
			var xmlReport = new XMLDeclarationReport(writer);
			var sumData = new SummaryDataContainer(writer);
			var jobContainer = new JobContainer(sumData);

			if (File.Exists(writer.SumFileName)) {
				File.Delete(writer.SumFileName);
			}

			var runsFactory = SimulatorFactory.CreateSimulatorFactory(ExecutionMode.Declaration, dataProvider, writer,
				xmlReport, validate: false);
			runsFactory.WriteModalResults = false;
			//var runsFactory = new SimulatorFactory(ExecutionMode.Declaration, dataProvider, writer, xmlReport) {
			//	WriteModalResults = false,
			//	Validate = false,
			//};
			jobContainer.AddRuns(runsFactory);

			jobContainer.Execute();
			jobContainer.WaitFinished();

			var mrfValidator = GetValidator(xmlReport.FullReport);
			mrfValidator.ValidateXML(XmlDocumentType.DeclarationComponentData | XmlDocumentType.DeclarationJobData | XmlDocumentType.CustomerReport | XmlDocumentType.ManufacturerReport);

			var cifValidator = GetValidator(xmlReport.CustomerReport);
			cifValidator.ValidateXML(XmlDocumentType.DeclarationComponentData | XmlDocumentType.DeclarationJobData | XmlDocumentType.CustomerReport | XmlDocumentType.ManufacturerReport);

			//var monitoringValidator = GetValidator(xmlReport.MonitoringReport);
			//monitoringValidator.ValidateXML(XmlDocumentType.DeclarationComponentData | XmlDocumentType.DeclarationJobData | XmlDocumentType.CustomerReport | XmlDocumentType.ManufacturerReport);
		}

		private static XMLValidator GetValidator(XDocument xmlReport)
		{
			var mrfStream = new MemoryStream();
			var mrfWriter = new XmlTextWriter(mrfStream, Encoding.UTF8);
			xmlReport.WriteTo(mrfWriter);
			mrfWriter.Flush();
			mrfStream.Flush();
			mrfStream.Seek(0, SeekOrigin.Begin);
			return new XMLValidator(new XmlTextReader(mrfStream));
		}

		[Category("LongRunning")]
		[TestCase()]
		public void TestXMLReportPTO()
		{
			var ptoGearWheels = XMLDeclarationInputTest.GetEnumOptions("PTOShaftsGearWheelsType", "1.0");
			var ptoOthers = XMLDeclarationInputTest.GetEnumOptions("PTOOtherElementsType", "1.0");

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

					var jobfile = @"Testdata\XML\XMLReaderDeclaration\vecto_vehicle-sample.xml";

					var doc = new XmlDocument();
					doc.Load(XmlReader.Create(jobfile));
					var nav = doc.CreateNavigator();
					var manager = new XmlNamespaceManager(nav.NameTable);
					var helper = new XPathHelper(ExecutionMode.Declaration);
					helper.AddNamespaces(manager);
					var ptoGearWheelsNode = nav.SelectSingleNode(
						helper.QueryAbs(
							helper.NSPrefix(
								XMLNames.VectoInputDeclaration,
								Constants.XML.RootNSPrefix),
							XMLNames.Component_Vehicle,
							XMLNames.Vehicle_PTO,
							XMLNames.Vehicle_PTO_ShaftsGearWheels),
						manager);
					ptoGearWheelsNode.SetValue(ptoGearWheel);
					var ptoOtherNode = nav.SelectSingleNode(
						helper.QueryAbs(
							helper.NSPrefix(
								XMLNames.VectoInputDeclaration,
								Constants.XML.RootNSPrefix),
							XMLNames.Component_Vehicle,
							XMLNames.Vehicle_PTO,
							XMLNames.Vehicle_PTO_OtherElements),
						manager);
					ptoOtherNode.SetValue(ptoOther);

					var modified = XmlReader.Create(new StringReader(nav.OuterXml));

					var writer = new FileOutputWriter(InputDataHelper.GetRandomFilename(jobfile));
					var xmlReport = new XMLDeclarationReport(writer);
					var sumData = new SummaryDataContainer(writer);
					var jobContainer = new JobContainer(sumData);

					var dataProvider = xmlInputReader.CreateDeclaration(modified);

					var runsFactory = SimulatorFactory.CreateSimulatorFactory(ExecutionMode.Declaration, dataProvider, writer,
						xmlReport, validate: false);
					runsFactory.WriteModalResults = false;
					//var runsFactory = new SimulatorFactory(ExecutionMode.Declaration, dataProvider, writer, xmlReport) {
					//	WriteModalResults = false,
					//	Validate = false,
					//};
					jobContainer.AddRuns(runsFactory);

					xmlReport.DoWriteReport();

					var manufacturerReport = xmlReport.FullReport;

					Assert.AreEqual(
						ptoGearWheel != "none",
						XmlConvert.ToBoolean(manufacturerReport.XPathSelectElement("//*[local-name()='PTO']").Value),
						"PTO Type: {0} {1}", ptoGearWheel, ptoOther);

				}
			}
		}


		[TestCase]
		public void TestXMLReportCorrectHashes()
		{
			var jobfile = @"Testdata\XML\XMLReaderDeclaration\vecto_vehicle-sample.xml";
			var dataProvider = xmlInputReader.CreateDeclaration(jobfile);
			var writer = new FileOutputWriter(InputDataHelper.GetRandomFilename(jobfile));
			var xmlReport = new XMLDeclarationReport(writer);
			var sumData = new SummaryDataContainer(writer);
			var jobContainer = new JobContainer(sumData);

			if (File.Exists(writer.SumFileName)) {
				File.Delete(writer.SumFileName);
			}

			var runsFactory = SimulatorFactory.CreateSimulatorFactory(ExecutionMode.Declaration, dataProvider, writer,
				xmlReport, validate: false);
			runsFactory.WriteModalResults = false;
			//var runsFactory = new SimulatorFactory(ExecutionMode.Declaration, dataProvider, writer, xmlReport) {
			//	WriteModalResults = false,
			//	Validate = false,
			//};
			jobContainer.AddRuns(runsFactory);

			// no need to run the simulation, we only check whether the meta-data is correct, no results are considered
			//jobContainer.Execute();
			//jobContainer.WaitFinished();
			xmlReport.DoWriteReport();

			
			var inputHash = VectoHash.Load(jobfile);

			var mrfDigestData = new DigestData(xmlReport.FullReport.Document.XPathSelectElement("//*[local-name()='InputDataSignature']"));
			var mrfInputDigest = inputHash.ComputeHash(mrfDigestData.CanonicalizationMethods, mrfDigestData.DigestMethod);

			Assert.AreEqual(mrfInputDigest, mrfDigestData.DigestValue);

			var cifDigestData = new DigestData(xmlReport.CustomerReport.Document.XPathSelectElement("//*[local-name()='InputDataSignature']"));
			var cifInputDigest = inputHash.ComputeHash(cifDigestData.CanonicalizationMethods, cifDigestData.DigestMethod);

			Assert.AreEqual(cifInputDigest, cifDigestData.DigestValue);

			var mrfHash = VectoHash.Load(writer.XMLFullReportName);
			var mrfCifDigestData = new DigestData(xmlReport.CustomerReport.Document.XPathSelectElement("//*[local-name()='ResultDataSignature']"));
			var mrfCifDigest = mrfHash.ComputeHash(mrfCifDigestData.CanonicalizationMethods, mrfCifDigestData.DigestMethod);

			Assert.AreEqual(mrfCifDigest, mrfCifDigestData.DigestValue);

		}

		[TestCase]
		public void TestXMLPrimaryVehicleReportTest()
		{
			var jobfile = @"Testdata\XML\XMLReaderDeclaration\SchemaVersion2.4/vecto_vehicle-primary_heavyBus-sample.xml";
			var dataProvider = xmlInputReader.CreateDeclaration(jobfile);
			var writer = new FileOutputWriter(jobfile);
			var xmlReport = new XMLDeclarationReport(writer);
			var sumData = new SummaryDataContainer(writer);
			var jobContainer = new JobContainer(sumData);

			if (File.Exists(writer.SumFileName)) {
				File.Delete(writer.SumFileName);
			}

			var runsFactory = SimulatorFactory.CreateSimulatorFactory(ExecutionMode.Declaration, dataProvider, writer,
				xmlReport, validate: false);
			runsFactory.WriteModalResults = false;
			//var runsFactory = new SimulatorFactory(ExecutionMode.Declaration, dataProvider, writer, xmlReport) {
			//	WriteModalResults = false,
			//	Validate = false,
			//};
			jobContainer.AddRuns(runsFactory);

			// no need to run the simulation, we only check whether the meta-data is correct, no results are considered
			//jobContainer.Execute();
			//jobContainer.WaitFinished();
			xmlReport.DoWriteReport();
		}
	}
}
