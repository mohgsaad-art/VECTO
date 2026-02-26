using System.IO;
using System.Linq;
using Ninject;
using NUnit.Framework;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCore.InputData.FileIO.JSON;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.Models.Simulation.Impl.SimulatorFactory;
using TUGraz.VectoCore.Ninject;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.OutputData.FileIO;

namespace TUGraz.VectoCore.Tests.Integration.IMC;

public class IMC_EngineeringTest
{
    private StandardKernel _kernel;

    private const string PEV_IMC_Grp5_HW = "TestData/IMC/BEV_E2_Group5_2030LH_rl_Electr_Gear/BEV_Group5LH_rl_Electr_Gear_IMC-HW.vecto";
	private const string PEV_IMC_Grp5_all = "TestData/IMC/BEV_E2_Group5_2030LH_rl_Electr_Gear/BEV_Group5LH_rl_Electr_Gear_IMC-all.vecto";

	private const string PHEV_IMC_Grp5_HW = "TestData/IMC/P2_PHEV_Group5_2030LH_rl_ENG/P2_Group5LH_rl_IMC-HW.vecto";
	private const string PHEV_IMC_Grp5_all = "TestData/IMC/P2_PHEV_Group5_2030LH_rl_ENG/P2_Group5LH_rl_IMC-all.vecto";

    [OneTimeSetUp]
    public void RunBeforeAnyTests()
    {
        _kernel = new StandardKernel(new VectoNinjectModule());
    }

    [TestCase(PHEV_IMC_Grp5_HW, 0, TestName = "P-HEV IMC HW Engineering Grp5 LH")]
	[TestCase(PHEV_IMC_Grp5_HW, 1, TestName = "P-HEV IMC HW Engineering Grp5 RD")]
	[TestCase(PHEV_IMC_Grp5_all, 1, TestName = "P-HEV IMC All Engineering Grp5 RD")]
    public void RunJob_PHEV_IMC_Engineering(string jobFile, int runIdx)
    { 
        var inputProvider = JSONInputDataFactory.ReadJsonJob(jobFile);
        var writer = new FileOutputWriter(jobFile);
        var factory = _kernel.Get<ISimulatorFactoryFactory>().Factory(ExecutionMode.Engineering, inputProvider, writer, null, null, false);
        factory.Validate = false;
        factory.WriteModalResults = true;
        factory.SumData = new SummaryDataContainer(writer);
        var run = factory.SimulationRuns().ToArray()[runIdx];
        var modData = ((ModalDataContainer)((VehicleContainer)run.GetContainer()).ModData).Data;

        run.Run();
        Assert.IsTrue(run.FinishedWithoutErrors);
    }

	[TestCase (PEV_IMC_Grp5_HW, 0, TestName = "PEV IMC HW Engineering Grp5 LH")]
	[TestCase(PEV_IMC_Grp5_HW, 1, TestName = "PEV IMC HW Engineering Grp5 RD")]
	[TestCase(PEV_IMC_Grp5_all, 1, TestName = "PEV IMC All Engineering Grp5 RD")]
	public void RunJob_PEV_IMC_Engineering(string jobFile, int runIdx)
	{
		var inputProvider = JSONInputDataFactory.ReadJsonJob(jobFile);
		var writer = new FileOutputWriter(jobFile);
		var factory = _kernel.Get<ISimulatorFactoryFactory>().Factory(ExecutionMode.Engineering, inputProvider, writer, null, null, false);
        factory.Validate = false;
		factory.WriteModalResults = true;
		factory.SumData = new SummaryDataContainer(writer);

		var run = factory.SimulationRuns().ToArray()[runIdx];
		var modData = ((ModalDataContainer)((VehicleContainer)run.GetContainer()).ModData).Data;

		run.Run();
		Assert.IsTrue(run.FinishedWithoutErrors);
	}
}