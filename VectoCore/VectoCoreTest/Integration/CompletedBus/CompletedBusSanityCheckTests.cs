using System;
using System.Collections.Generic;
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
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.DataProvider;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.Models.Simulation.Impl.SimulatorFactory;
using TUGraz.VectoCore.OutputData.FileIO;
using TUGraz.VectoCore.Tests.Models.Simulation;
using TUGraz.VectoCore.Tests.Utils;

namespace TUGraz.VectoCore.Tests.Integration.CompletedBus
{
	[TestFixture]
	[Parallelizable(ParallelScope.All)]
	public class CompletedBusSanityCheckTests
	{
		private IXMLInputDataReader _xmlInputReader;
		public const string CompletedFile32 = @"TestData/Integration/Buses/FactorMethod/vecto_vehicle-completed_heavyBus_41.xml";
		//public const string CompletedFile33b1 = @"TestData/Integration/Buses/FactorMethod/CompletedHeavyBus_33b1.RSLT_VIF.xml";
        public const string CompletedFile33b1 = @"TestData/Integration/Buses/FactorMethod/vecto_vehicle-completed_heavyBus_42.xml";
        public const string  PifFile_33_34 = @"TestData/Integration/Buses/FactorMethod/VIF/primary_heavyBus group42_SmartPS.RSLT_VIF.xml";

        [OneTimeSetUp]
		public void RunBeforeAnyTests()
		{
			Directory.SetCurrentDirectory(TestContext.CurrentContext.TestDirectory);
			var kernel = new StandardKernel(new VectoNinjectModule());
			_xmlInputReader = kernel.Get<IXMLInputDataReader>();
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
			Category(Definitions.TESTCASE_MIGRATED)
        ]
        public void TestSeparateAirDistributionDuctsError(string completedJob, BusHVACSystemConfiguration hvacConfig, bool separateDucts)
		{
			var modified = GetModifiedXML(PifFile_33_34, completedJob, hvacConfig, separateDucts);

			var writer = new FileOutputWriter("SanityCheckTest");

			//var vifDataProvider = _xmlInputReader.Create(modified);
			//var inputData = new XMLDeclarationVIFInputData(modified  as IMultistageBusInputDataProvider, null);
			//var inputData = new MockCompletedBusInputData(XmlReader.Create(PifFile_33_34), modified);
			//var inputData = _xmlInputReader.CreateDeclaration(modified);

			var factory = SimulatorFactory.CreateSimulatorFactory(ExecutionMode.Declaration, new XMLDeclarationVIFInputData(modified, null),  writer);
			factory.WriteModalResults = true;
			factory.Validate = false;

			AssertHelper.Exception<VectoException>(() => {
				var runs = factory.RunDataFactory.NextRun().ToList();}, messageContains: "Input parameter 'separate air distribution ducts' has to be set to 'true' for vehicle group ");
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
		Category(Definitions.TESTCASE_MIGRATED)
		]
        public void TestSeparateAirDistributionDuctsOk(string completedJob, BusHVACSystemConfiguration hvacConfig, bool separateDucts)
        {
            var modified = GetModifiedXML(PifFile_33_34, completedJob, hvacConfig, separateDucts);

            var writer = new FileOutputWriter("SanityCheckTest");
            //var inputData = new MockCompletedBusInputData(XmlReader.Create(PifFile_33_34), modified);
			//var inputData = new MockCompletedBusInputData(modified);

			var factory = SimulatorFactory.CreateSimulatorFactory(ExecutionMode.Declaration, new XMLDeclarationVIFInputData(modified, null), writer);
			factory.WriteModalResults = true;
			factory.Validate = false;

			//AssertHelper.Exception<VectoException>(() => {
                var runs = factory.RunDataFactory.NextRun().ToList();
            //}, messageContains: "Input parameter 'separate air distribution ducts' has to be set to 'true' for vehicle group ");
        }

        private IMultistepBusInputDataProvider GetModifiedXML(string vifPrimary, string completedJob, BusHVACSystemConfiguration? hvacConfig, bool separateDucts)
		{
			var vifDataProvider = _xmlInputReader.Create(XmlReader.Create(vifPrimary));
			//var completeDataProvider = _xmlInputReader.CreateDeclaration(comple);

			var completedXML = new XmlDocument();
			completedXML.Load(completedJob);

			var hvacCfgNode = completedXML.SelectSingleNode("//*[local-name()='SystemConfiguration']");
			hvacCfgNode.InnerText = hvacConfig.ToXmlFormat();

			var (hpDriver, hpPassenger) = GetHeatpumps(hvacConfig);
			var hpDriverCoolingNode =
				completedXML.SelectSingleNode(
					"//*[local-name()='HeatPumpTypeDriverCompartment']/*[local-name()='Cooling']");
			hpDriverCoolingNode.InnerText = hpDriver.ToXML();
			var hpPassengerCoolingNode =
				completedXML.SelectSingleNode(
					"//*[local-name()='HeatPumpTypePassengerCompartment']/*[local-name()='Cooling']");
			hpPassengerCoolingNode.InnerText = hpPassenger.ToXML();

			var airDuctsNode = completedXML.SelectSingleNode("//*[local-name()='SeparateAirDistributionDucts']");
			airDuctsNode.InnerText = XmlConvert.ToString(separateDucts);
			var modified = XmlReader.Create(new StringReader(completedXML.OuterXml));

			var completeDataProvider = _xmlInputReader.CreateDeclaration(modified);

			var inputData = new XMLDeclarationVIFInputData(vifDataProvider as IMultistepBusInputDataProvider, completeDataProvider.JobInputData.Vehicle);

			var filename = Guid.NewGuid().ToString().Substring(0, 20);
			var writer = new FileOutputVIFWriter(filename, 0);

			var factory = SimulatorFactory.CreateSimulatorFactory(ExecutionMode.Declaration, inputData, writer);
			var jobContainer = new JobContainer(new MockSumWriter());
			jobContainer.AddRuns(factory);
			jobContainer.Execute();
			jobContainer.WaitFinished();

			var completedVifXML = new XmlDocument();
			completedVifXML.Load(writer.XMLMultistageReportFileName);

			var completedVif = _xmlInputReader.CreateDeclaration(XmlReader.Create(new StringReader(completedVifXML.OuterXml)));
			File.Delete(writer.XMLMultistageReportFileName);

			return completedVif as IMultistepBusInputDataProvider;
		}

		private (HeatPumpType, HeatPumpType) GetHeatpumps(BusHVACSystemConfiguration? hvacConfig)
		{
			var mapping = new Dictionary<BusHVACSystemConfiguration, Tuple<HeatPumpType, HeatPumpType>>() {
				{ BusHVACSystemConfiguration.Configuration1, Tuple.Create(HeatPumpType.none, HeatPumpType.none) },
				{ BusHVACSystemConfiguration.Configuration2, Tuple.Create(HeatPumpType.non_R_744_2_stage, HeatPumpType.none) },
				{ BusHVACSystemConfiguration.Configuration3, Tuple.Create(HeatPumpType.none, HeatPumpType.none) },
				{ BusHVACSystemConfiguration.Configuration4, Tuple.Create(HeatPumpType.non_R_744_2_stage, HeatPumpType.none) },
				{ BusHVACSystemConfiguration.Configuration5, Tuple.Create(HeatPumpType.none, HeatPumpType.non_R_744_3_stage) },
				{ BusHVACSystemConfiguration.Configuration6, Tuple.Create(HeatPumpType.none, HeatPumpType.non_R_744_3_stage) },
				{ BusHVACSystemConfiguration.Configuration7, Tuple.Create(HeatPumpType.non_R_744_2_stage, HeatPumpType.non_R_744_3_stage) },
				{ BusHVACSystemConfiguration.Configuration8, Tuple.Create(HeatPumpType.none, HeatPumpType.non_R_744_3_stage) },
				{ BusHVACSystemConfiguration.Configuration9, Tuple.Create(HeatPumpType.non_R_744_2_stage, HeatPumpType.non_R_744_3_stage) },
				{ BusHVACSystemConfiguration.Configuration10, Tuple.Create(HeatPumpType.none, HeatPumpType.non_R_744_3_stage) },
			};
			if (!hvacConfig.HasValue || !mapping.ContainsKey(hvacConfig.Value)) {
				throw new VectoException("invalid hvac configuration");
			}
			var entry = mapping[hvacConfig.Value];
			return (entry.Item1, entry.Item2);
		}
	}

	public class MockCompletedBusInputData : IInputDataProvider, IMultistepBusInputDataProvider
	{
		private IMultistepBusInputDataProvider input;
	
		public MockCompletedBusInputData(XmlReader vif)
		{
			var kernel = new StandardKernel(new VectoNinjectModule());
			var _xmlInputReader = kernel.Get<IXMLInputDataReader>();


			input = _xmlInputReader.CreateDeclaration(vif) as IMultistepBusInputDataProvider;
			
			//JobName = Vehicle.VIN;
		}

		public IDeclarationMultistageJobInputData JobInputData => input.JobInputData;


		IDeclarationJobInputData IDeclarationInputDataProvider.JobInputData => null;

		public IPrimaryVehicleInformationInputDataProvider PrimaryVehicleData => input.PrimaryVehicleData;
		public XElement XMLHash { get; }

		public bool SavedInDeclarationMode => true;

		public IPrimaryVehicleInformationInputDataProvider PrimaryVehicle => PrimaryVehicleData;

		public IList<IManufacturingStageInputData> ManufacturingStages => input.JobInputData.ManufacturingStages;

		public IManufacturingStageInputData ConsolidateManufacturingStage => input.JobInputData.ConsolidateManufacturingStage;

		public VectoSimulationJobType JobType => VectoSimulationJobType.ConventionalVehicle;

		public bool InputComplete { get; }

		public DataSource DataSource { get; }
	}
}