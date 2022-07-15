using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using Ninject;
using NUnit.Framework;
using NUnit.Framework.Internal;
using TUGraz.VectoCommon.BusAuxiliaries;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCore.InputData.FileIO.JSON;
using TUGraz.VectoCore.InputData.FileIO.XML;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.OutputData.FileIO;
using TUGraz.VectoCore.Tests.Utils;

namespace TUGraz.VectoCore.Tests.Integration.CompletedBus
{
	[TestFixture]
	public class CompletedBusSanityCheckTests
	{
		public const string CompletedFile32 = @"TestData\Integration\Buses\FactorMethod\vecto_vehicle-completed_heavyBus_41.xml";
		public const string CompletedFile33b1 = @"TestData\Integration\Buses\FactorMethod\vecto_vehicle-completed_heavyBus_42.xml";
		public const string  PifFile_33_34 = @"TestData\Integration\Buses\FactorMethod\primary_heavyBus group42_SmartPS.RSLT_PIF.xml";

        [OneTimeSetUp]
		public void RunBeforeAnyTests()
		{
			Directory.SetCurrentDirectory(TestContext.CurrentContext.TestDirectory);
		}

        [TestCase(CompletedFile33b1, BusHVACSystemConfiguration.Configuration8, false, TestName = "CompletedBus AirDistribution sanitycheck error, grp 33b1 cfg 8/false"),
		TestCase(CompletedFile33b1, BusHVACSystemConfiguration.Configuration9, false, TestName = "CompletedBus AirDistribution sanitycheck error, grp 33b1 cfg 9/false"),
		TestCase(CompletedFile33b1, BusHVACSystemConfiguration.Configuration10, false, TestName = "CompletedBus AirDistribution sanitycheck error, grp 33b1 cfg 10/false"),

		TestCase(CompletedFile32, BusHVACSystemConfiguration.Configuration1, false, TestName = "CompletedBus AirDistribution sanitycheck error, grp 34b cfg 1/false"),
		TestCase(CompletedFile32, BusHVACSystemConfiguration.Configuration2, false, TestName = "CompletedBus AirDistribution sanitycheck error, grp 34b cfg 2/false"),
		TestCase(CompletedFile32, BusHVACSystemConfiguration.Configuration3, false, TestName = "CompletedBus AirDistribution sanitycheck error, grp 34b cfg 3/false"),
		TestCase(CompletedFile32, BusHVACSystemConfiguration.Configuration4, false, TestName = "CompletedBus AirDistribution sanitycheck error, grp 34b cfg 4/false"),
		TestCase(CompletedFile32, BusHVACSystemConfiguration.Configuration5, false, TestName = "CompletedBus AirDistribution sanitycheck error, grp 34b cfg 5/false"),
		TestCase(CompletedFile32, BusHVACSystemConfiguration.Configuration6, false, TestName = "CompletedBus AirDistribution sanitycheck error, grp 34b cfg 6/false"),
		TestCase(CompletedFile32, BusHVACSystemConfiguration.Configuration7, false, TestName = "CompletedBus AirDistribution sanitycheck error, grp 34b cfg 7/false"),
		TestCase(CompletedFile32, BusHVACSystemConfiguration.Configuration8, false, TestName = "CompletedBus AirDistribution sanitycheck error, grp 34b cfg 8/false"),
		TestCase(CompletedFile32, BusHVACSystemConfiguration.Configuration9, false, TestName = "CompletedBus AirDistribution sanitycheck error, grp 34b cfg 9/false"),
		TestCase(CompletedFile32, BusHVACSystemConfiguration.Configuration10, false, TestName = "CompletedBus AirDistribution sanitycheck error, grp 34b cfg 10/false"),
        ]
        public void TestSeparateAirDistributionDuctsError(string completedJob, BusHVACSystemConfiguration hvacConfig, bool separateDucts)
		{
			var modified = GetModifiedXML(completedJob, hvacConfig, separateDucts);

			var writer = new FileOutputWriter("SanityCheckTest");
			var inputData = new MockCompletedBusInputData(XmlReader.Create(PifFile_33_34), modified);

			var factory = new SimulatorFactory(ExecutionMode.Declaration, inputData, writer)
			{
				WriteModalResults = true,
				Validate = false
			};

			AssertHelper.Exception<VectoException>(() => {
				var runs = factory.DataReader.NextRun().ToList();}, messageContains: "Input parameter 'separate air distribution ducts' has to be set to 'true' for vehicle group ");
        }

        [
		TestCase(CompletedFile33b1, BusHVACSystemConfiguration.Configuration8, true, TestName = "CompletedBus AirDistribution sanitycheck OK, grp 33b1 cfg 8/true"),
		TestCase(CompletedFile33b1, BusHVACSystemConfiguration.Configuration9, true, TestName = "CompletedBus AirDistribution sanitycheck OK, grp 33b1 cfg 9/true"),
		TestCase(CompletedFile33b1, BusHVACSystemConfiguration.Configuration10, true, TestName = "CompletedBus AirDistribution sanitycheck OK, grp 33b1 cfg 10/true"),

		TestCase(CompletedFile32, BusHVACSystemConfiguration.Configuration1, true, TestName = "CompletedBus AirDistribution sanitycheck OK, grp 34b cfg 1/true"),
		TestCase(CompletedFile32, BusHVACSystemConfiguration.Configuration2, true, TestName = "CompletedBus AirDistribution sanitycheck OK, grp 34b cfg 2/true"),
		TestCase(CompletedFile32, BusHVACSystemConfiguration.Configuration3, true, TestName = "CompletedBus AirDistribution sanitycheck OK, grp 34b cfg 3/true"),
		TestCase(CompletedFile32, BusHVACSystemConfiguration.Configuration4, true, TestName = "CompletedBus AirDistribution sanitycheck OK, grp 34b cfg 4/true"),
		TestCase(CompletedFile32, BusHVACSystemConfiguration.Configuration5, true, TestName = "CompletedBus AirDistribution sanitycheck OK, grp 34b cfg 5/true"),
		TestCase(CompletedFile32, BusHVACSystemConfiguration.Configuration6, true, TestName = "CompletedBus AirDistribution sanitycheck OK, grp 34b cfg 6/true"),
		TestCase(CompletedFile32, BusHVACSystemConfiguration.Configuration7, true, TestName = "CompletedBus AirDistribution sanitycheck OK, grp 34b cfg 7/true"),
		TestCase(CompletedFile32, BusHVACSystemConfiguration.Configuration8, true, TestName = "CompletedBus AirDistribution sanitycheck OK, grp 34b cfg 8/true"),
		TestCase(CompletedFile32, BusHVACSystemConfiguration.Configuration9, true, TestName = "CompletedBus AirDistribution sanitycheck OK, grp 34b cfg 9/true"),
		TestCase(CompletedFile32, BusHVACSystemConfiguration.Configuration10, true, TestName = "CompletedBus AirDistribution sanitycheck OK, grp 34b cfg 10/true"),

		TestCase(CompletedFile33b1, BusHVACSystemConfiguration.Configuration1, false, TestName = "CompletedBus AirDistribution sanitycheck OK, grp 33b1 cfg 1/false"),
		TestCase(CompletedFile33b1, BusHVACSystemConfiguration.Configuration2, false, TestName = "CompletedBus AirDistribution sanitycheck OK, grp 33b1 cfg 2/false"),
		TestCase(CompletedFile33b1, BusHVACSystemConfiguration.Configuration3, false, TestName = "CompletedBus AirDistribution sanitycheck OK, grp 33b1 cfg 3/false"),
		TestCase(CompletedFile33b1, BusHVACSystemConfiguration.Configuration4, false, TestName = "CompletedBus AirDistribution sanitycheck OK, grp 33b1 cfg 4/false"),
		TestCase(CompletedFile33b1, BusHVACSystemConfiguration.Configuration5, false, TestName = "CompletedBus AirDistribution sanitycheck OK, grp 33b1 cfg 5/false"),
		TestCase(CompletedFile33b1, BusHVACSystemConfiguration.Configuration6, false, TestName = "CompletedBus AirDistribution sanitycheck OK, grp 33b1 cfg 6/false"),
		TestCase(CompletedFile33b1, BusHVACSystemConfiguration.Configuration7, false, TestName = "CompletedBus AirDistribution sanitycheck OK, grp 33b1 cfg 7/false"),

		TestCase(CompletedFile33b1, BusHVACSystemConfiguration.Configuration1, true, TestName = "CompletedBus AirDistribution sanitycheck OK, grp 33b1 cfg 1/true"),
		TestCase(CompletedFile33b1, BusHVACSystemConfiguration.Configuration2, true, TestName = "CompletedBus AirDistribution sanitycheck OK, grp 33b1 cfg 2/true"),
		TestCase(CompletedFile33b1, BusHVACSystemConfiguration.Configuration3, true, TestName = "CompletedBus AirDistribution sanitycheck OK, grp 33b1 cfg 3/true"),
		TestCase(CompletedFile33b1, BusHVACSystemConfiguration.Configuration4, true, TestName = "CompletedBus AirDistribution sanitycheck OK, grp 33b1 cfg 4/true"),
		TestCase(CompletedFile33b1, BusHVACSystemConfiguration.Configuration5, true, TestName = "CompletedBus AirDistribution sanitycheck OK, grp 33b1 cfg 5/true"),
		TestCase(CompletedFile33b1, BusHVACSystemConfiguration.Configuration6, true, TestName = "CompletedBus AirDistribution sanitycheck OK, grp 33b1 cfg 6/true"),
		TestCase(CompletedFile33b1, BusHVACSystemConfiguration.Configuration7, true, TestName = "CompletedBus AirDistribution sanitycheck OK, grp 33b1 cfg 7/true"),
		]
        public void TestSeparateAirDistributionDuctsOk(string completedJob, BusHVACSystemConfiguration hvacConfig, bool separateDucts)
        {
            var modified = GetModifiedXML(completedJob, hvacConfig, separateDucts);

            var writer = new FileOutputWriter("SanityCheckTest");
            var inputData = new MockCompletedBusInputData(XmlReader.Create(PifFile_33_34), modified);

            var factory = new SimulatorFactory(ExecutionMode.Declaration, inputData, writer)
            {
                WriteModalResults = true,
                Validate = false
            };

            //AssertHelper.Exception<VectoException>(() => {
                var runs = factory.DataReader.NextRun().ToList();
            //}, messageContains: "Input parameter 'separate air distribution ducts' has to be set to 'true' for vehicle group ");
        }

        private static XmlReader GetModifiedXML(string completedJob, BusHVACSystemConfiguration hvacConfig, bool separateDucts)
		{
			var completedXML = new XmlDocument();
			completedXML.Load(completedJob);

			var hvacCfgNode = completedXML.SelectSingleNode("//*[local-name()='SystemConfiguration']");
			hvacCfgNode.InnerText = hvacConfig.GetXmlFormat();

			var airDuctsNode = completedXML.SelectSingleNode("//*[local-name()='SeparateAirDistributionDucts']");
			airDuctsNode.InnerText = XmlConvert.ToString(separateDucts);
			var modified = XmlReader.Create(new StringReader(completedXML.OuterXml));
			return modified;
		}
	}

	public class MockCompletedBusInputData : IInputDataProvider, IDeclarationInputDataProvider, IDeclarationJobInputData
	{
		public MockCompletedBusInputData(XmlReader pif, XmlReader completed)
		{
			var kernel = new StandardKernel(new VectoNinjectModule());
			var _xmlInputReader = kernel.Get<IXMLInputDataReader>();


			Vehicle = _xmlInputReader.CreateDeclaration(completed).JobInputData.Vehicle;
			PrimaryVehicleData = (_xmlInputReader.Create(pif) as IPrimaryVehicleInformationInputDataProvider);
			JobName = Vehicle.VIN;
		}

		public IDeclarationJobInputData JobInputData
		{
			get { return this; }
		}
		public IPrimaryVehicleInformationInputDataProvider PrimaryVehicleData { get; }
		public void ValidateComponentHashes()
		{
			//throw new System.NotImplementedException();
		}

		public XElement XMLHash { get; }

		public bool SavedInDeclarationMode
		{
			get { return true; }
		}
		public IVehicleDeclarationInputData Vehicle { get; }

		public string JobName { get; }
		public string ShiftStrategy { get; }

		public DataSource DataSource { get; }
	}
}