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
using System.Data;
using System.IO;
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
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.Models.Simulation.Impl.SimulatorFactory;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.OutputData.FileIO;
using TUGraz.VectoCore.Tests.Models.Simulation;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.Tests.Integration.Declaration
{
	[TestFixture()]
	[Parallelizable(ParallelScope.All)]
	public class NaturalGasVehicles
	{
		const string Class5NG = @"Testdata\Integration\DeclarationMode\Class5_NG\Tractor_4x2_vehicle-class-5_EURO6_2018.xml";

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
		 TestCase(Class5NG, 2, TankSystem.Liquefied, 249.8, 691.93, TestName = "Class5 LNG 2"),
		 TestCase(Class5NG, 2, TankSystem.Compressed, 255.5, 687.35, TestName = "Class5 CNG 2"),
		TestCase(Class5NG, 6, TankSystem.Liquefied, 253.2, 701.46, TestName = "Class5 LNG 6"),
		TestCase(Class5NG, 6, TankSystem.Compressed, 259.0, 696.81, TestName = "Class5 CNG 6"),
			]
		public void NaturalGasTankSystemTest(string filename, int runIdx, TankSystem tankSystem, double expectedFc, double expectedCo2)
		{
			var reader = XmlReader.Create(filename);

			var doc = new XmlDocument();
			doc.Load(reader);
			var nav = doc.CreateNavigator();
			var manager = new XmlNamespaceManager(nav.NameTable);
			var helper = new XPathHelper(ExecutionMode.Declaration);
			helper.AddNamespaces(manager);

			XNamespace ns = Constants.XML.VectoDeclarationDefinitionsNS;
			var tankSystemNode = nav.SelectSingleNode(helper.QueryAbs(
													helper.NSPrefix(XMLNames.VectoInputDeclaration,
																	Constants.XML.RootNSPrefix),
													XMLNames.Component_Vehicle,
													XMLNames.Vehicle_NgTankSystem),
												manager);
			tankSystemNode.SetValue(tankSystem.ToString());
			var modified = XmlReader.Create(new StringReader(nav.OuterXml));

			var writer = new FileOutputWriter(filename); // new MockDeclarationWriter(filename);
			var inputData = xmlInputReader.CreateDeclaration(modified);
			var factory = SimulatorFactory.CreateSimulatorFactory(ExecutionMode.Declaration, inputData, writer);
			factory.WriteModalResults = true;
			factory.ActualModalData = true;
			var jobContainer = new JobContainer(new MockSumWriter());

			jobContainer.AddRuns(factory);

			jobContainer.Execute();
			jobContainer.WaitFinished();
			var manufacturerReport = XDocument.Load(XmlReader.Create(writer.XMLFullReportName)); // writer.GetReport(ReportType.DeclarationReportManufacturerXML);

			var fuelTypeResultNodes = manufacturerReport.XPathSelectElements("//*[local-name()='Results']//*[local-name()='FuelType']");
			foreach (var node in fuelTypeResultNodes) {
				Assert.AreEqual(tankSystem == TankSystem.Liquefied ? "LNG PI" : "CNG PI", node.Value);
			}

			var fcNode = manufacturerReport.XPathSelectElement(
				$"//*[local-name()='Results']/*[local-name()='Result'][{runIdx}]//*[local-name()='FuelConsumption' and @unit='g/km']");
			var co2Node = manufacturerReport.XPathSelectElement(
				$"//*[local-name()='Results']/*[local-name()='Result'][{runIdx}]//*[local-name()='CO2' and @unit='g/km']");

			Console.WriteLine("fc: {0}  co2: {1}", fcNode.Value, co2Node.Value);

			Assert.AreEqual(expectedFc, fcNode.Value.ToDouble(), 0.1);
			Assert.AreEqual(expectedCo2, co2Node.Value.ToDouble(), 0.1);
		}
	}

	public class MockDeclarationWriter : IOutputDataWriter
	{
		public DataTable SumData { get; private set; }

		private readonly Dictionary<ReportType, XDocument> _reports = new Dictionary<ReportType, XDocument>();

		

		public MockDeclarationWriter(string filename)
		{
			JobFile = filename;
		}

		public XDocument GetReport(ReportType type)
		{
			return _reports[type];
		}

		#region Implementation of IModalDataWriter

		public void WriteModData(int jobRunId, string runName, string cycleName, string runSuffix, DataTable modData)
		{
			
		}

		#endregion

		#region Implementation of IReportWriter

		public void WriteReport(ReportType type, XDocument data)
		{
			_reports[type] = data;
		}

		public void WriteReport(ReportType type, Stream data)
		{
		}

		public IDictionary<ReportType, string> GetWrittenFiles()
		{
			throw new NotImplementedException();
		}

		public int NumberOfManufacturingStages
		{
			set => throw new NotImplementedException();
		}

		public XDocument MultistageXmlReport { get; }

		#endregion

		#region Implementation of ISummaryWriter

		public void WriteSumData(DataTable sortedAndFilteredTable)
		{
			SumData = sortedAndFilteredTable;
		}

		#endregion

		#region Implementation of IOutputDataWriter

		public string JobFile
		{
			get;
			private set;
		}

		#endregion
	}
}
