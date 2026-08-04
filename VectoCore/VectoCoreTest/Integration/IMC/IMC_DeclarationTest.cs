using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Xml;
using Moq;
using Ninject;
using NUnit.Framework;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCore.InputData.FileIO.JSON;
using TUGraz.VectoCore.InputData.FileIO.XML;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.DataProvider;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.DataProvider.v24;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Interfaces;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.Ninject;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.OutputData.FileIO;
using TUGraz.VectoCore.Tests.Utils;
using TUGraz.VectoCore.Tests.Utils.Ninject;

namespace TUGraz.VectoCore.Tests.Integration.IMC;

public class IMC_DeclarationTest
{
	private ThreadLocal<StandardKernel> _kernel;
	//private IXMLInputDataReader _xmlReader;


	public const string PEV_IMC_Grp5_all = "TestData/IMC/Declaration/BEV_E2_Group5_2030/BEV_Group5_IMC.vecto";
	public const string HEV_IMC_Grp5_all = "TestData/IMC/Declaration/P2_PHEV_Group5_2030/P2_Group5_IMC.vecto";

	public const string PEV_Grp5_IMC_XML = "TestData/Integration/IMC/Group5_ PEV_E4.xml";

	public const string PEV_PrimaryBus_IMC = "TestData/Integration/IMC/PEV_primaryBus_AMT_E2.xml";
	public const string PEV_CompleteBus_IMC = "TestData/Integration/IMC/PEV_completedBus_2.xml";

	public const string HEV_PrimaryBus_IMC = "TestData/Integration/IMC/PrimaryCoach_P2_HEV_AMT_OVC.xml";
	public const string HEV_CompleteBus_IMC = "TestData/Integration/IMC/HEV_completedBus_2.xml";

    public StandardKernel Kernel {
		get => _kernel.Value;
	}

	[OneTimeSetUp]
	public void OneTimeSetup()
	{
		_kernel = new ThreadLocal<StandardKernel>(() => new StandardKernel(new VectoNinjectModule()));
		//_xmlReader = Kernel.Get<IXMLInputDataReader>();
	}


    [TestCase(PEV_IMC_Grp5_all, MissionType.RegionalDelivery, LoadingType.ReferenceLoad, TestName = "PEV IMC All Declaration Grp5 RD RL")]
	public void RunJob_PEV_IMC_Declaration(string jobFile, MissionType? mission = null, LoadingType? loading = null)
	{
		if (mission != null && loading != null) {
			Kernel.Rebind<IMissionFilter>().To<TestMissionFilter>().InSingletonScope();
			var missionFilter = Kernel.Get<IMissionFilter>() as TestMissionFilter;
			missionFilter.SetMissions((mission.Value, loading.Value));
		}
        var inputProvider = JSONInputDataFactory.ReadJsonJob(jobFile);
		var writer = new FileOutputWriter(jobFile);
		var factory = Kernel.Get<ISimulatorFactoryFactory>().Factory(ExecutionMode.Declaration, inputProvider, writer, null, null);
		factory.Validate = false;
		factory.WriteModalResults = true;
		factory.SumData = new SummaryDataContainer(writer);
		var sumData = new SummaryDataContainer(writer);
        var jobContainer = new JobContainer(sumData);
		jobContainer.AddRuns(factory);

		jobContainer.Execute(false);
		jobContainer.WaitFinished();
		Assert.IsTrue(jobContainer.GetProgress().All(x => x.Value.Success));
    }

	[TestCase(PEV_Grp5_IMC_XML, IMCTechnology.OverheadPantograph, MissionType.RegionalDelivery, LoadingType.ReferenceLoad, TestName = "PEV XML IMC Pantograph Declaration Grp5 RD RL")]
	public void RunJob_PEV_IMC_Declaration_XML(string jobFile, IMCTechnology imcTech, MissionType? mission = null, LoadingType? loading = null)
	{
		if (mission != null && loading != null) {
			Kernel.Rebind<IMissionFilter>().To<TestMissionFilter>().InSingletonScope();
			var missionFilter = Kernel.Get<IMissionFilter>() as TestMissionFilter;
			missionFilter.SetMissions((mission.Value, loading.Value));
		}

		Kernel.UpdateBinding<IXMLDeclarationVehicleData>(XMLDeclarationPevHeavyLorryDataProviderV24.QUALIFIED_XSD_TYPE,
			ctx => {
				var imcInputData = new Mock<XMLDeclarationPevHeavyLorryDataProviderV24>(
					(IXMLDeclarationJobInputData)ctx.Parameters.ToList()[0].GetValue(ctx, ctx.Request.Target),
					(XmlNode)ctx.Parameters.ToList()[1].GetValue(ctx, ctx.Request.Target),
					(string)ctx.Parameters.ToList()[2].GetValue(ctx, ctx.Request.Target)) { CallBase = true };
				imcInputData.Setup(v => v.InMotionCharging).Returns(() => new XMLIMCData() {
					Technology = imcTech,
				});
				return imcInputData.Object;
			});

		var _xmlReader = Kernel.Get<IXMLInputDataReader>();
        var inputProvider = _xmlReader.CreateDeclaration(jobFile);
		var writer = new FileOutputWriter(jobFile);
		var factory = Kernel.Get<ISimulatorFactoryFactory>().Factory(ExecutionMode.Declaration, inputProvider, writer, null, null);
		factory.Validate = false;
		factory.WriteModalResults = true;
		factory.SumData = new SummaryDataContainer(writer);
		var sumData = new SummaryDataContainer(writer);
		var jobContainer = new JobContainer(sumData);
		jobContainer.AddRuns(factory);

		jobContainer.Execute(false);
		jobContainer.WaitFinished();
		Assert.IsTrue(jobContainer.GetProgress().All(x => x.Value.Success));
	}

    [TestCase(HEV_IMC_Grp5_all, MissionType.RegionalDelivery, LoadingType.ReferenceLoad, TestName = "P-HEV IMC Declaration Grp5 RD RL")]
	public void RunJob_PHEV_IMC_Declaration(string jobFile, MissionType? mission = null, LoadingType? loading = null)
	{
		if (mission != null && loading != null) {
			Kernel.Rebind<IMissionFilter>().To<TestMissionFilter>().InSingletonScope();
			var missionFilter = Kernel.Get<IMissionFilter>() as TestMissionFilter;
			missionFilter.SetMissions((mission.Value, loading.Value));
        }

		var inputProvider = JSONInputDataFactory.ReadJsonJob(jobFile);
		var writer = new FileOutputWriter(jobFile);
		var factory = Kernel.Get<ISimulatorFactoryFactory>().Factory(ExecutionMode.Declaration, inputProvider, writer, null, null);
		//SimulatorFactory.CreateSimulatorFactory(ExecutionMode.Declaration, inputProvider, writer);
		factory.Validate = false;
		factory.WriteModalResults = true;

		var sumData = new SummaryDataContainer(writer);

		var jobContainer = new JobContainer(sumData);
		jobContainer.AddRuns(factory);

		jobContainer.Execute(false);
		jobContainer.WaitFinished();
		Assert.IsTrue(jobContainer.GetProgress().All(x => x.Value.Success));
	}


	[TestCase(PEV_PrimaryBus_IMC, IMCTechnology.OverheadPantograph)]
	[TestCase(PEV_PrimaryBus_IMC, IMCTechnology.GroundRail)]
    public void RunJob_PrimaryBus(string jobFile, IMCTechnology imcTech, MissionType? mission = null, LoadingType? loading = null)
	{
		if (mission != null && loading != null) {
			Kernel.Rebind<IMissionFilter>().To<TestMissionFilter>().InSingletonScope();
			var missionFilter = Kernel.Get<IMissionFilter>() as TestMissionFilter;
			missionFilter.SetMissions((mission.Value, loading.Value));
		}

		Kernel.UpdateBinding<IXMLDeclarationVehicleData>(XMLDeclarationPevPrimaryBusDataProviderV24.QUALIFIED_XSD_TYPE,
			ctx => {
				var imcInputData = new Mock<XMLDeclarationPevPrimaryBusDataProviderV24>(
					(IXMLDeclarationJobInputData)ctx.Parameters.ToList()[0].GetValue(ctx, ctx.Request.Target),
					(XmlNode)ctx.Parameters.ToList()[1].GetValue(ctx, ctx.Request.Target),
					(string)ctx.Parameters.ToList()[2].GetValue(ctx, ctx.Request.Target)) { CallBase = true };
				imcInputData.Setup(v => v.InMotionCharging).Returns(() => new XMLIMCData() {
					Technology = imcTech,
				});
				return imcInputData.Object;
			});

		var _xmlReader = Kernel.Get<IXMLInputDataReader>();
        var writer = new FileOutputWriter(Path.Combine(Path.GetDirectoryName(jobFile), "tmp", Path.GetFileName(jobFile)));
        var inputData = _xmlReader.CreateDeclaration(jobFile);
		var factory = Kernel.Get<ISimulatorFactoryFactory>().Factory(ExecutionMode.Declaration, inputData, writer, null, null, false);

		factory.WriteModalResults = true;
		var sumData = new SummaryDataContainer(writer);

		var jobContainer = new JobContainer(sumData);
		jobContainer.AddRuns(factory);

		jobContainer.Execute(false);
		jobContainer.WaitFinished();
		Assert.IsTrue(jobContainer.GetProgress().All(x => x.Value.Success));
    }

	[TestCase(PEV_PrimaryBus_IMC, PEV_CompleteBus_IMC, IMCTechnology.None, IMCTechnology.None, TestName = "PEV A - IMC Complete Bus None/None")]
	[TestCase(PEV_PrimaryBus_IMC, PEV_CompleteBus_IMC, IMCTechnology.None, IMCTechnology.OverheadPantograph, TestName = "PEV B - IMC Complete Bus None/Pantograph")]
	[TestCase(PEV_PrimaryBus_IMC, PEV_CompleteBus_IMC, IMCTechnology.OverheadPantograph, IMCTechnology.OverheadPantograph, TestName = "PEV C - IMC Pantograph/Pantograph")]
	[TestCase(PEV_PrimaryBus_IMC, PEV_CompleteBus_IMC, IMCTechnology.OverheadPantograph, IMCTechnology.None, TestName = "PEV D - IMC Pantograph/None")]

	[TestCase(PEV_PrimaryBus_IMC, PEV_CompleteBus_IMC, IMCTechnology.None, IMCTechnology.GroundRail, TestName = "PEV E - IMC Complete Bus None/GroundRail")]
	[TestCase(PEV_PrimaryBus_IMC, PEV_CompleteBus_IMC, IMCTechnology.GroundRail, IMCTechnology.GroundRail, TestName = "PEV F - IMC GroundRail/GroundRail")]
	[TestCase(PEV_PrimaryBus_IMC, PEV_CompleteBus_IMC, IMCTechnology.GroundRail, IMCTechnology.None, TestName = "PEV G - IMC GroundRail/None")]
	public void RunJob_CompleteBus_PEV(string primaryJob, string completeJob, IMCTechnology primaryTech,
		IMCTechnology completeTech, MissionType? mission = null, LoadingType? loading = null)
	{
		// update bindings to create a mock object that provides the imc technology values for primary vehicle, VIF, and complete vehicle
		Kernel.UpdateBinding<IXMLDeclarationVehicleData>(XMLDeclarationPevPrimaryBusDataProviderV24.QUALIFIED_XSD_TYPE, 
			ctx => {
				var imcInputData = new Mock<XMLDeclarationPevPrimaryBusDataProviderV24>(
					(IXMLDeclarationJobInputData)ctx.Parameters.ToList()[0].GetValue(ctx, ctx.Request.Target),
					(XmlNode)ctx.Parameters.ToList()[1].GetValue(ctx, ctx.Request.Target),
					(string)ctx.Parameters.ToList()[2].GetValue(ctx, ctx.Request.Target)) { CallBase = true };
				imcInputData.Setup(v => v.InMotionCharging).Returns(() => new XMLIMCData() {
					Technology = primaryTech,
				});
				Console.WriteLine($"primaryBus Provider: {imcInputData.Object.InMotionCharging.Technology.ToString()}");
				return imcInputData.Object;
			});
		Kernel.UpdateBinding<IXMLDeclarationVehicleData>(XMLDeclarationMultistage_PEV_Ex_PrimaryVehicleBusDataProviderV01.QUALIFIED_XSD_TYPE, ctx => {
			var imcInputData = new Mock<XMLDeclarationMultistage_PEV_Ex_PrimaryVehicleBusDataProviderV01>(
				(IXMLPrimaryVehicleBusJobInputData)ctx.Parameters.ToList()[0].GetValue(ctx, ctx.Request.Target),
				(XmlNode)ctx.Parameters.ToList()[1].GetValue(ctx, ctx.Request.Target), 
				(string)ctx.Parameters.ToList()[2].GetValue(ctx, ctx.Request.Target)) {CallBase = true};
			imcInputData.Setup(v => v.InMotionCharging).Returns(() => new XMLIMCData() {
				Technology = primaryTech,
			});
			Console.WriteLine($"Multistage Provider: {imcInputData.Object.InMotionCharging.Technology.ToString()}");
            return imcInputData.Object;
		});
		Kernel.UpdateBinding<IXMLDeclarationVehicleData>(XMLDeclarationPEVCompletedBusDataProviderV24.QUALIFIED_XSD_TYPE, ctx => {
			var imcInputData = new Mock<XMLDeclarationPEVCompletedBusDataProviderV24>(
				(IXMLDeclarationJobInputData)ctx.Parameters.ToList()[0].GetValue(ctx, ctx.Request.Target),
				(XmlNode)ctx.Parameters.ToList()[1].GetValue(ctx, ctx.Request.Target),
				(string)ctx.Parameters.ToList()[2].GetValue(ctx, ctx.Request.Target)) { CallBase = true };
			imcInputData.Setup(v => v.InMotionCharging).Returns(() => new XMLIMCData() {
				Technology = completeTech,
			});
			Console.WriteLine($"completed Bus Provider: {imcInputData.Object.InMotionCharging.Technology.ToString()}");
			return imcInputData.Object;
		});
        // ---------
		if (mission != null && loading != null) {
			Kernel.Rebind<IMissionFilter>().To<TestMissionFilter>().InSingletonScope();
			var missionFilter = Kernel.Get<IMissionFilter>() as TestMissionFilter;
			missionFilter.SetMissions((mission.Value, loading.Value));
		}

        var testname = TestContext.CurrentContext.Test.Name.Substring(0, 4);
        var outFolder = Path.Combine(Path.GetDirectoryName(primaryJob), $"out_{testname}_{primaryTech.ToString()}_{completeTech.ToString()}");
		if (!Directory.Exists(outFolder)) {
			Directory.CreateDirectory(outFolder);
		}

		var _xmlReader = Kernel.Get<IXMLInputDataReader>();
        var writer = new FileOutputWriter(Path.Combine(outFolder, Path.GetFileName(primaryJob)));
		var primary = _xmlReader.CreateDeclaration(primaryJob);
		var complete = _xmlReader.CreateDeclaration(completeJob);

		var primaryVehicleMock = Mock.Get(primary.JobInputData.Vehicle);
		var completeVehicleMock = Mock.Get(complete.JobInputData.Vehicle);

        //Console.WriteLine($"Primary IMC: {primary.JobInputData.Vehicle.InMotionCharging.Technology.ToString()}");
        Console.WriteLine($"Complete IMC: {complete.JobInputData.Vehicle.InMotionCharging.Technology.ToString()}");

        var inputData = new MockCompletedBusInputDataProvider(primary, complete, true);
		inputData.XMLInputReader = _xmlReader;
		var factory = Kernel.Get<ISimulatorFactoryFactory>().Factory(ExecutionMode.Declaration, inputData, writer, null, null, false);

		factory.WriteModalResults = true;
		factory.SerializeVectoRunData = true;
        var sumData = new SummaryDataContainer(writer);

		var jobContainer = new JobContainer(sumData);
		jobContainer.AddRuns(factory);

		jobContainer.Execute(false);
		jobContainer.WaitFinished();

		var runs = jobContainer.Runs;

		primaryVehicleMock.Verify(p => p.InMotionCharging, Times.AtLeastOnce);
		completeVehicleMock.Verify(c => c.InMotionCharging, Times.AtLeastOnce);

        Assert.IsTrue(jobContainer.GetProgress().All(x => x.Value.Success));
    }

    [TestCase(HEV_PrimaryBus_IMC, HEV_CompleteBus_IMC, IMCTechnology.None, IMCTechnology.None, TestName = "HEV A - IMC Complete Bus None/None")]
    [TestCase(HEV_PrimaryBus_IMC, HEV_CompleteBus_IMC, IMCTechnology.None, IMCTechnology.OverheadPantograph, TestName = "HEV B - IMC Complete Bus None/Pantograph")]
    [TestCase(HEV_PrimaryBus_IMC, HEV_CompleteBus_IMC, IMCTechnology.OverheadPantograph, IMCTechnology.OverheadPantograph, TestName = "HEV C - IMC Pantograph/Pantograph")]
    [TestCase(HEV_PrimaryBus_IMC, HEV_CompleteBus_IMC, IMCTechnology.OverheadPantograph, IMCTechnology.None, TestName = "HEV D - IMC Pantograph/None")]

    [TestCase(HEV_PrimaryBus_IMC, HEV_CompleteBus_IMC, IMCTechnology.None, IMCTechnology.GroundRail, TestName = "HEV E - IMC Complete Bus None/GroundRail")]
    [TestCase(HEV_PrimaryBus_IMC, HEV_CompleteBus_IMC, IMCTechnology.GroundRail, IMCTechnology.GroundRail, TestName = "HEV F - IMC GroundRail/GroundRail")]
    [TestCase(HEV_PrimaryBus_IMC, HEV_CompleteBus_IMC, IMCTechnology.GroundRail, IMCTechnology.None, TestName = "HEV G - IMC GroundRail/None")]
    public void RunJob_CompleteBus_HEV(string primaryJob, string completeJob, IMCTechnology primaryTech,
        IMCTechnology completeTech, MissionType? mission = null, LoadingType? loading = null)
    {
        // update bindings to create a mock object that provides the imc technology values for primary vehicle, VIF, and complete vehicle
        Kernel.UpdateBinding<IXMLDeclarationVehicleData>(XMLDeclarationHevPxPrimaryBusDataProviderV24.QUALIFIED_XSD_TYPE,
            ctx => {
                var imcInputData = new Mock<XMLDeclarationHevPxPrimaryBusDataProviderV24>(
                    (IXMLDeclarationJobInputData)ctx.Parameters.ToList()[0].GetValue(ctx, ctx.Request.Target),
                    (XmlNode)ctx.Parameters.ToList()[1].GetValue(ctx, ctx.Request.Target),
                    (string)ctx.Parameters.ToList()[2].GetValue(ctx, ctx.Request.Target)) { CallBase = true };
                imcInputData.Setup(v => v.InMotionCharging).Returns(() => new XMLIMCData() {
                    Technology = primaryTech,
                });
                Console.WriteLine($"primaryBus Provider: {imcInputData.Object.InMotionCharging.Technology.ToString()}");
                return imcInputData.Object;
            });
        Kernel.UpdateBinding<IXMLDeclarationVehicleData>(XMLDeclarationMultistage_HEV_Px_PrimaryVehicleBusDataProviderV01.QUALIFIED_XSD_TYPE, ctx => {
            var imcInputData = new Mock<XMLDeclarationMultistage_HEV_Px_PrimaryVehicleBusDataProviderV01>(
                (IXMLPrimaryVehicleBusJobInputData)ctx.Parameters.ToList()[0].GetValue(ctx, ctx.Request.Target),
                (XmlNode)ctx.Parameters.ToList()[1].GetValue(ctx, ctx.Request.Target),
                (string)ctx.Parameters.ToList()[2].GetValue(ctx, ctx.Request.Target)) { CallBase = true };
            imcInputData.Setup(v => v.InMotionCharging).Returns(() => new XMLIMCData() {
                Technology = primaryTech,
            });
            Console.WriteLine($"Multistage Provider: {imcInputData.Object.InMotionCharging.Technology.ToString()}");
            return imcInputData.Object;
        });
        Kernel.UpdateBinding<IXMLDeclarationVehicleData>(XMLDeclarationHevCompletedBusDataProviderV24.QUALIFIED_XSD_TYPE, ctx => {
            var imcInputData = new Mock<XMLDeclarationHevCompletedBusDataProviderV24>(
                (IXMLDeclarationJobInputData)ctx.Parameters.ToList()[0].GetValue(ctx, ctx.Request.Target),
                (XmlNode)ctx.Parameters.ToList()[1].GetValue(ctx, ctx.Request.Target),
                (string)ctx.Parameters.ToList()[2].GetValue(ctx, ctx.Request.Target)) { CallBase = true };
            imcInputData.Setup(v => v.InMotionCharging).Returns(() => new XMLIMCData() {
                Technology = completeTech,
            });
            Console.WriteLine($"completed Bus Provider: {imcInputData.Object.InMotionCharging.Technology.ToString()}");
            return imcInputData.Object;
        });
        // ---------
        if (mission != null && loading != null) {
            Kernel.Rebind<IMissionFilter>().To<TestMissionFilter>().InSingletonScope();
            var missionFilter = Kernel.Get<IMissionFilter>() as TestMissionFilter;
            missionFilter.SetMissions((mission.Value, loading.Value));
        }

        var testname = TestContext.CurrentContext.Test.Name.Substring(0, 4);
        var outFolder = Path.Combine(Path.GetDirectoryName(primaryJob), $"out_{testname}_{primaryTech.ToString()}_{completeTech.ToString()}");
        if (!Directory.Exists(outFolder)) {
            Directory.CreateDirectory(outFolder);
        }

        var _xmlReader = Kernel.Get<IXMLInputDataReader>();
        var writer = new FileOutputWriter(Path.Combine(outFolder, Path.GetFileName(primaryJob)));
        var primary = _xmlReader.CreateDeclaration(primaryJob);
        var complete = _xmlReader.CreateDeclaration(completeJob);

		var primaryVehicleMock = Mock.Get(primary.JobInputData.Vehicle);
		var completeVehicleMock = Mock.Get(complete.JobInputData.Vehicle);

        //Console.WriteLine($"Primary IMC: {primary.JobInputData.Vehicle.InMotionCharging.Technology.ToString()}");
        Console.WriteLine($"Complete IMC: {complete.JobInputData.Vehicle.InMotionCharging.Technology.ToString()}");

        var inputData = new MockCompletedBusInputDataProvider(primary, complete, true);
        inputData.XMLInputReader = _xmlReader;
        var factory = Kernel.Get<ISimulatorFactoryFactory>().Factory(ExecutionMode.Declaration, inputData, writer, null, null, false);

        factory.WriteModalResults = true;
        factory.SerializeVectoRunData = true;
        var sumData = new SummaryDataContainer(writer);

        var jobContainer = new JobContainer(sumData);
        jobContainer.AddRuns(factory);

        jobContainer.Execute(false);
        jobContainer.WaitFinished();

        var runs = jobContainer.Runs;

		primaryVehicleMock.Verify(p => p.InMotionCharging, Times.AtLeastOnce);
		completeVehicleMock.Verify(c => c.InMotionCharging, Times.AtLeastOnce);

        Assert.IsTrue(jobContainer.GetProgress().All(x => x.Value.Success));
    }

    public class MockCompletedBusInputDataProvider : IMultistagePrimaryAndStageInputDataProvider
	{
		public IXMLInputDataReader XMLInputReader { set; get; }

		private IDeclarationInputDataProvider _primaryVehicle;
		private IVehicleDeclarationInputData _completedVehicle;
        private bool _simulateResultingVif;


		internal MockCompletedBusInputDataProvider(IInputDataProvider primaryVehicle, IDeclarationInputDataProvider completedVehicleData)
		{
			_primaryVehicle = primaryVehicle as IDeclarationInputDataProvider;
			_completedVehicle = completedVehicleData.JobInputData.Vehicle;
			_simulateResultingVif = false;

		}
		internal MockCompletedBusInputDataProvider(IInputDataProvider primaryVehicle, IDeclarationInputDataProvider completedVehicleData, bool runSimulation) : this(primaryVehicle, completedVehicleData)
		{
			_simulateResultingVif = runSimulation;
		}

		#region Implementation of IInputDataProvider

		public DataSource DataSource { get; }

        #endregion

        #region Implementation of IMultistagePrimaryAndStageInputDataProvider

        public IDeclarationInputDataProvider PrimaryVehicle =>
    _primaryVehicle;

        public IVehicleDeclarationInputData StageInputData => _completedVehicle;

        public bool SimulateResultingVIF => _simulateResultingVif;

		public bool? Completed => null;

		#endregion
	}
}