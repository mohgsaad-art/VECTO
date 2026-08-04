using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Xml.Linq;
using System.Xml.XPath;
using Ninject;
using NUnit.Framework;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.InputData.FileIO.XML;
using TUGraz.VectoCore.Models.Connector.Ports.Impl;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.Ninject;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.OutputData.FileIO;
using TUGraz.VectoCore.Tests.Utils;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.Tests.Integration.Declaration.PrimaryBus;

public class PrimaryBusIgnoreResult
{
	private const string BASE_DIR = @"TestData/Integration/DeclarationMode/2nd_AmendmDeclMode/";

	private ThreadLocal<StandardKernel> _kernel;

	private StandardKernel Kernel => _kernel.Value;

	private IXMLInputDataReader _xmlReader;

    [OneTimeSetUp]
	public void OneTimeSetup()
	{
		_kernel = new ThreadLocal<StandardKernel>(() => new StandardKernel(new VectoNinjectModule()));
		_xmlReader = Kernel.Get<IXMLInputDataReader>();
	}

    [SetUp]
	public void Setup()
	{
		Kernel.Rebind<IDeclarationCycleFactory>().To<TestDeclarationCycleFactoryVariant>().InSingletonScope();
		var cycleFactory = Kernel.Get<IDeclarationCycleFactory>() as TestDeclarationCycleFactoryVariant;
		cycleFactory.Variant = "Short_1";

	}

	private TestMissionFilter TestMissionFilter()
	{
		Kernel.Rebind<IMissionFilter>().To<TestMissionFilter>().InSingletonScope();
		var missionFilter = Kernel.Get<IMissionFilter>() as TestMissionFilter;
		Assert.NotNull(missionFilter);
		return missionFilter;
	}

    [
		TestCase(@"PrimaryBus/Conventional/primary_heavyBus group41_nonSmart.xml", 0, TestName = "2nd Amendment PrimaryBus IgnoreCycleAbort Conventional"),
		TestCase(@"PrimaryBus/P-HEV/PrimaryCoach_P2_HEV_Base_AMT.xml", 0, TestName = "2nd Amendment PrimaryBus IgnoreCycleAbort Coach P-HEV P2 Base AMT"),
		TestCase(@"PrimaryBus/P-HEV/PrimaryCoach_P2_HEV_AMT_OVC.xml", 0, TestName = "2nd Amendment PrimaryBus IgnoreCycleAbort Coach P-HEV P2 AMT OVC"),
		TestCase(@"FactorMethod/IEPC/P31_32_IEPC_EDP.xml", 0, TestName = "2nd Amendment PrimaryBus IgnoreCycleAbort FM IEPC EDP"),
		TestCase(@"FactorMethod/IHPC/P31_32_IHPC_nonSmartES_mechAux.xml", 0, TestName = "2nd Amendment PrimaryBus IgnoreCycleAbort FM IHPC nonSmartES_mechAux"),
		TestCase(@"PrimaryBus/PEV/PEV_primaryBus_AMT_E2.xml", 0, TestName = "2nd Amendment PrimaryBus IgnoreCycleAbort PEV E2"),
		TestCase(@"PrimaryBus/PEV/PrimaryCityBus_IEPC_Base.xml", 0, TestName = "2nd Amendment PrimaryBus IgnoreCycleAbort CityBus PEV IEPC Base"),
		TestCase(@"PrimaryBus/S-HEV/PrimaryCoach_S2_Base_AMT.xml", 0, TestName = "2nd Amendment PrimaryBus IgnoreCycleAbort Coach S-HEV S2 Base"),
		TestCase(@"PrimaryBus/S-HEV/PrimaryCityBus_IEPC-S_Base.xml", 0, TestName = "2nd Amendment PrimaryBus IgnoreCycleAbort CityBus S-HEV IEPC Base"),
	]
    public void PrimaryBusIgnoreCycleAbortTest(string jobFile, int runIdx)
	{
		TestMissionFilter().SetMissions((MissionType.Interurban, LoadingType.ReferenceLoad));
		RunSimulationPrimary(jobFile, runIdx);
	}

	public void RunSimulationPrimary(string jobFile, int runIdx, params Action<VectoRunData>[] runDataModifier)
	{
		RunSimulationPrimary(jobFile, r => r.IterativeRunStrategy.Enabled = false);
	}

    public void RunSimulationPrimary(string jobFile, params Action<VectoRunData>[] runDataModifier)
    {
		var filePath = Path.Combine(BASE_DIR, jobFile);
		var outDir = Path.Combine(Path.GetDirectoryName(filePath), "IgnoreCycleAbort");
		if (!Directory.Exists(outDir)) {
			Directory.CreateDirectory(outDir);
		}
		
        var dataProvider = _xmlReader.CreateDeclaration(filePath);
        var fileWriter = new FileOutputWriter(Path.Combine(outDir, Path.GetFileName(jobFile)));
        var sumData = new SummaryDataContainer(fileWriter);
        var simFactory = Kernel.Get<ISimulatorFactoryFactory>();
        var runsFactory = simFactory.Factory(ExecutionMode.Declaration, dataProvider, fileWriter, null, null);
        runsFactory.WriteModalResults = false;
        runsFactory.SerializeVectoRunData = false;
		runsFactory.Validate = false;
        runsFactory.SumData = sumData;
        var jobContainer = new JobContainer(sumData) { };
        //var jobContainer = new JobContainer(new MockSumWriter()) { };
        var runs = runsFactory.SimulationRuns().ToList();
        foreach (var vectoRun in runs) {
            foreach (var action in runDataModifier) {
                action(vectoRun.GetContainer().RunData);
            }
        }

		var abortedRunIdx = runs.FindIndex(x =>
			x.GetContainer().RunData.Mission.BusParameter.DoubleDecker &&
			x.GetContainer().RunData.Loading == LoadingType.ReferenceLoad &&
			x.GetContainer().RunData.Mission.MissionType == MissionType.Interurban);
		if (abortedRunIdx < 0) {
			throw new Exception("No such simulation run found!");
		}
		runs[abortedRunIdx] = new AbortingSimulationRun(runs[abortedRunIdx]);

        foreach (var run in runs) {
            jobContainer.AddRun(run);
        }

        jobContainer.Execute();
        jobContainer.WaitFinished();
        Assert.IsTrue(jobContainer.AllCompleted);
        Assert.IsTrue(jobContainer.Runs.TrueForAll(runEntry => runEntry.Success));
		PrintFiles(fileWriter);

		AssertResultsAreNaN(fileWriter);
		
	}

	private void AssertResultsAreNaN(FileOutputWriter xmlFile)
	{
		var vifFileName = xmlFile.XMLPrimaryVehicleReportName;
		var vifXML = XDocument.Load(vifFileName);

		var vifResults = vifXML.XPathSelectElements(".//*[local-name() = 'Result']");
		var ignoredVifResult = vifResults.FirstOrDefault(x => {
			var subGroup = x.XPathSelectElement("./*[local-name()='PrimaryVehicleSubgroup']");
			var cycle = x.XPathSelectElement("./*[local-name()='Mission']");

            return subGroup.Value.EndsWith("DD") && cycle.Value.Equals("Interurban");
		});

		Assert.NotNull(ignoredVifResult);
		Assert.AreEqual("success", ignoredVifResult.Attributes().First(a => a.Name.LocalName.Equals("status")).Value);

		var mrfFileName = xmlFile.XMLFullReportName;
		var mrfXML = XDocument.Load(mrfFileName);
		var mrfResults = mrfXML.XPathSelectElements(".//*[local-name() = 'Result']");
		var cycleVif = ignoredVifResult.XPathSelectElement("./*[local-name()='Mission']");
		var massVif = ignoredVifResult.XPathSelectElement(".//*[local-name()='TotalVehicleMass']");
		var ignoredMrfResult = mrfResults.FirstOrDefault(x => {
			var cycle = x.XPathSelectElement("./*[local-name()='Mission']");
			var mass = x.XPathSelectElement(".//*[local-name()='TotalVehicleMass']");
			return cycle.Value.Equals(cycleVif.Value) && mass.Value.Equals(massVif.Value);
		});

		Assert.NotNull(ignoredMrfResult);
		Assert.AreEqual("success", ignoredMrfResult.Attributes().First(a => a.Name.LocalName.Equals("status")).Value);

        var notANumberElements = new[] {
			"FuelConsumption", "CO2", "EnergyConsumption", "UtilityFactor", "ActualChargeDepletingRange", "EquivalentAllElectricRange",
			"ZeroCO2EmissionsRange",
			"AverageSpeed", "AverageDrivingSpeed", "MinSpeed", "MaxSpeed", "MaxDeceleration", "MaxAcceleration"
		};
		foreach (var node in new[] {ignoredVifResult, ignoredMrfResult}) {
			var values = new List<Tuple<string, string, string>>();
			var hasOVCModes = node.XPathSelectElements(".//*[local-name() = 'OVCMode']").Any();
			foreach (var elementName in notANumberElements) {
				var detailedSelector = hasOVCModes
					? " and ((ancestor-or-self::*[local-name() = 'OVCMode' and @type = 'charge depleting']) or ancestor-or-self::*[local-name()='Total'])"
                    : "";
				var queryString = $".//*[local-name() = '{elementName}'{detailedSelector}]";
				var result = node.XPathSelectElements(queryString);
				foreach (var entry in result) {
					//Assert.AreEqual(entry.Value, "NaN", $"elment ${elementName} expected to be NaN");
					var unit = entry.Attributes().FirstOrDefault(a => a.Name == "unit")?.Value;
					values.Add(Tuple.Create(elementName, unit ?? "", entry.Value));
				}
			}

			Assert.IsNotEmpty(values);
			foreach (var entry in values) {
				Assert.AreEqual("NaN", entry.Item3, $"element {entry.Item1} {entry.Item2} exptected to be NaN, {entry.Item3} found");
			}
		}
		
	}

	public class AbortingSimulationRun : DistanceRun
	{
		private readonly IVectoRun _run;

		public AbortingSimulationRun(IVectoRun run) : base(run.GetContainer(), null, new IgnoreSimulationRun())
		{
			_run = run;
		}

        protected override IResponse DoSimulationStep()
        {
            IterationStatistics.StartIteration();

            // estimate distance to be traveled within the next TargetTimeInterval
            var ds = Container.VehicleInfo.VehicleSpeed.IsEqual(0.KMPHtoMeterPerSecond(), 0.01.SI<MeterPerSecond>())
                ? Constants.SimulationSettings.DriveOffDistance
                : VectoMath.Max(
                Constants.SimulationSettings.TargetTimeInterval * Container.VehicleInfo.VehicleSpeed,
                    Constants.SimulationSettings.DriveOffDistance);

            var loopCount = 0;
            IResponse response;
            var debug = new DebugData();
            do {
                IterationStatistics.Increment(this, "Iterations");

                Container.Brakes.BrakePower = 0.SI<Watt>();
                response = CyclePort.Request(AbsTime, ds);
                switch (response) {
                    case ResponseSuccess r:
                        dt = r.SimulationInterval;
                        break;
                    case ResponseDrivingCycleDistanceExceeded r:
                        if (r.MaxDistance.IsSmallerOrEqual(0)) {
                            throw new VectoSimulationException("DistanceExceeded, MaxDistance is invalid: {0}", r.MaxDistance);
                        }
                        ds = r.MaxDistance;
                        break;
                    case ResponseCycleFinished _:
                        FinishedWithoutErrors = true;
                        Log.Info("========= Driving Cycle Finished");
                        break;
                    case ResponseBatteryEmpty _:
                        FinishedWithoutErrors = true;
                        Log.Info("========= REESS empty");
                        break;
                    default:
                        throw new VectoException("DistanceRun got an unexpected response: {0}", response);
                }
                if (loopCount++ > Constants.SimulationSettings.MaximumIterationCountForSimulationStep) {
                    throw new VectoSimulationException("Maximum iteration count for a single simulation interval reached! Aborting!");
                }

				if (AbsTime > 100.SI<Second>()) {
					throw new VectoException("Intentionally aborting the simulation run...");
				}
                debug.Add($"[DR.DST-{loopCount}]", response);
            } while (!(response is ResponseSuccess || response is ResponseCycleFinished || response is ResponseBatteryEmpty));

            IterationStatistics.Increment(this, "Distance", Container.MileageCounter.Distance.Value());
            IterationStatistics.Increment(this, "Time", AbsTime.Value());
            IterationStatistics.FinishIteration(AbsTime);
            response.AbsTime = AbsTime;
            return response;
        }
    }

	public class IgnoreSimulationRun : IPostMortemAnalyzer
	{
		#region Implementation of IPostMortemAnalyzer

		public bool AbortSimulation(IVehicleContainer container, Exception ex)
		{
            // do not abort the simulation but ignore the result
			container.RunStatus = VectoRun.Status.PrimaryBusSimulationIgnore;
            return false;
		}

		#endregion
	}

	private void PrintFiles(FileOutputWriter fileWriter)
	{
		foreach (var keyValuePair in fileWriter.GetWrittenFiles()) {
			TestContext.WriteLine($"{keyValuePair.Key} written to {keyValuePair.Value}");
			TestContext.AddTestAttachment(keyValuePair.Value, keyValuePair.Key.ToString());
		}
	}
}