using System.IO;
using System.Linq;
using Ninject;
using NUnit.Framework;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCore.InputData.FileIO.JSON;
using TUGraz.VectoCore.InputData.FileIO.XML;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.Models.Simulation.Impl.SimulatorFactory;
using TUGraz.VectoCore.Ninject;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.OutputData.FileIO;

namespace TUGraz.VectoCore.Tests.Integration.H2;


[TestFixture]
public class H2_ICE_Test
{
	private StandardKernel _kernel;

	[OneTimeSetUp]
	public void OneTimeSetup()
	{
		Directory.SetCurrentDirectory(TestContext.CurrentContext.TestDirectory);
        _kernel = new StandardKernel(new VectoNinjectModule()) { };
	}

	[TestCase]
	public void RunJob_H2_ICE_Group5_Engineering()
	{
		var jobFile = "TestData/H2_ICE/Group5_Tractor_4x2_H2-ICE/Class5_Tractor_H2ICE_ENG.vecto";
		var inputProvider = JSONInputDataFactory.ReadJsonJob(jobFile);
		var writer = new FileOutputWriter(jobFile);
		var factory = _kernel.Get<ISimulatorFactoryFactory>().Factory(ExecutionMode.Engineering, inputProvider, writer, null, null, false);
        factory.Validate = false;
		factory.WriteModalResults = true;
		factory.SumData = new SummaryDataContainer(writer);
		var run = factory.SimulationRuns().ToArray()[0];
		var modData = ((ModalDataContainer)((VehicleContainer)run.GetContainer()).ModData).Data;

		run.Run();
		Assert.IsTrue(run.FinishedWithoutErrors);
	}

	[TestCase]
	public void RunJob_H2_ICE_Group5_Declaration()
	{
		var jobFile = "TestData/H2_ICE/Group5_Conv_ES_Standard_H2_ICE.xml";
		var inputProvider = _kernel.Get<IXMLInputDataReader>().Create(jobFile);
		var writer = new FileOutputWriter(jobFile);
		var factory = _kernel.Get<ISimulatorFactoryFactory>().Factory(ExecutionMode.Declaration, inputProvider, writer, null, null, false);
        factory.Validate = false;
		factory.WriteModalResults = true;
		factory.SumData = new SummaryDataContainer(writer);
		var run = factory.SimulationRuns().ToArray()[0];
		var modData = ((ModalDataContainer)((VehicleContainer)run.GetContainer()).ModData).Data;

		run.Run();
		Assert.IsTrue(run.FinishedWithoutErrors);
	}

}