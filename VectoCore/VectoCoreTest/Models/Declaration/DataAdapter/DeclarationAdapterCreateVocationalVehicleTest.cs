using System.IO;
using System.Xml;
using System.Xml.XPath;
using Ninject;
using NUnit.Framework;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCore.InputData.FileIO.XML;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.Models.Simulation.Impl.SimulatorFactory;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.OutputData.FileIO;
using TUGraz.VectoCore.OutputData.XML;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.Tests.Models.Declaration.DataAdapter
{
	[TestFixture]
	[Parallelizable(ParallelScope.All)]
	public class DeclarationAdapterCreateVocationalVehicleTest
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

		[TestCase(@"Rigid Truck_4x2_vehicle-class-1_EURO6_2018.xml"),
		TestCase(@"Rigid Truck_4x2_vehicle-class-2_EURO6_2018.xml"),
		TestCase(@"Rigid Truck_4x2_vehicle-class-3_EURO6_2018.xml"),
		TestCase(@"Rigid Truck_6x4_vehicle-class-11_EURO6_2018.xml"),
		TestCase(@"Rigid Truck_8x4_vehicle-class-16_EURO6_2018.xml"),
		TestCase(@"Tractor_6x4_vehicle-class-12_EURO6_2018.xml"),

			]
		public void TestCreateDeclarationVocationalVehicle(string jobfile)
		{
			var reader = XmlReader.Create(Path.Combine(@"TestData/XML/XMLReaderDeclaration/GroupTest", jobfile));

			var doc = new XmlDocument();
			doc.Load(reader);
			var nav = doc.CreateNavigator();
			var manager = new XmlNamespaceManager(nav.NameTable);
			var helper = new XPathHelper(ExecutionMode.Declaration);
			helper.AddNamespaces(manager);

			var vocational = nav.SelectSingleNode(XMLHelper.QueryLocalName(XMLNames.Component_Vehicle, XMLNames.Vehicle_VocationalVehicle));
			vocational.SetValue(true.ToString().ToLower());
			var modified = XmlReader.Create(new StringReader(nav.OuterXml));
			
			var dataProvider = xmlInputReader.CreateDeclaration(modified);

			var writer = new FileOutputWriter(jobfile);
			var xmlReport = new XMLDeclarationReport(writer);
			var sumData = new SummaryDataContainer(null);
			var jobContainer = new JobContainer(sumData);

			var runsFactory = SimulatorFactory.CreateSimulatorFactory(ExecutionMode.Declaration, dataProvider, null, xmlReport);
			runsFactory.WriteModalResults = false;
			runsFactory.Validate = false;
			jobContainer.AddRuns(runsFactory);

			// no need to run the simulation, we only check whether the meta-data is correct, no results are considered
			//jobContainer.Execute();
			//jobContainer.WaitFinished();
			xmlReport.DoWriteReport();

			var manufacturerReport = xmlReport.FullReport;

			Assert.IsFalse(XmlConvert.ToBoolean(manufacturerReport.XPathSelectElement(XMLHelper.QueryLocalName(XMLNames.Vehicle_VocationalVehicle))?.Value ?? ""));
		}
	}
}
