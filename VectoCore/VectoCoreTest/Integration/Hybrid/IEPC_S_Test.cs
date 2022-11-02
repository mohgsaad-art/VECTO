using System.IO;
using System.Linq;
using NUnit.Framework;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCore.InputData.FileIO.JSON;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.Models.Simulation.Impl.SimulatorFactory;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.OutputData.FileIO;
using TUGraz.VectoCore.Tests.Utils;

namespace TUGraz.VectoCore.Tests.Integration.BatteryElectric
{
	[TestFixture]
	[Parallelizable(ParallelScope.All)]
	public class IEPC_S_Test
	{

		protected const string IEPC_S_Gbx3Speed = @"TestData\Hybrids\GenericIEPC-S\IEPC-S_Gbx3Speed\IEPC-S_ENG_Gbx3.vecto";

		protected const string IEPC_S_Gbx3Speed_drag = @"TestData\Hybrids\GenericIEPC-S\IEPC-S_Gbx3Speed\IEPC-S_ENG_Gbx3_drag.vecto";

		protected const string IEPC_S_Gbx3SpeedAxle = @"TestData\Hybrids\GenericIEPC-S\IEPC-S_Gbx3Speed+Axle\IEPC-S_ENG_Gbx3Axl.vecto";

		protected const string IEPC_S_Gbx3SpeedWhl1 = @"TestData\Hybrids\GenericIEPC-S\IEPC-S_Gbx3Speed-Whl1\IEPC-S_ENG_Gbx3Whl1.vecto";

		protected const string IEPC_S_Gbx3SpeedWhl2 = @"TestData\Hybrids\GenericIEPC-S\IEPC-S_Gbx3Speed-Whl2\IEPC-S_ENG_Gbx3Whl2.vecto";

		protected const string IEPC_S_Gbx1Speed = @"TestData\Hybrids\GenericIEPC-S\IEPC-S_Gbx1Speed\IEPC-S_ENG_Gbx1.vecto";

		protected const string IEPC_S_Gbx1SpeedAxl = @"TestData\Hybrids\GenericIEPC-S\IEPC-S_Gbx1Speed+Axle\IEPC-S_ENG_Gbx1Axl.vecto";

		protected const string IEPC_S_Gbx1SpeedWhl1 = @"TestData\Hybrids\GenericIEPC-S\IEPC-S_Gbx1Speed-Whl1\IEPC-S_ENG_Gbx1Whl1.vecto";

		protected const string IEPC_S_Gbx1SpeedWhl2 = @"TestData\Hybrids\GenericIEPC-S\IEPC-S_Gbx1Speed-Whl2\IEPC-S_ENG_Gbx1Whl2.vecto";


		[OneTimeSetUp]
		public void RunBeforeAnyTests()
		{
			Directory.SetCurrentDirectory(TestContext.CurrentContext.TestDirectory);
		}


		private GraphWriter GetGraphWriter(ModalResultField[] yFields)
		{
			var graphWriter = new GraphWriter();
			//#if TRACE
			graphWriter.Enable();

			//#else
			//graphWriter.Disable();
			//#endif


			var Yfields = new[] {
				ModalResultField.v_act, ModalResultField.altitude, ModalResultField.acc, ModalResultField.Gear,
				ModalResultField.REESSStateOfCharge,
			}.Concat(yFields).ToArray();
			graphWriter.Xfields = new[] { ModalResultField.dist };
			graphWriter.Yfields = yFields;
			graphWriter.Series1Label = "BEV";
			graphWriter.PlotIgnitionState = true;

			return graphWriter;
		}


		[TestCase(IEPC_S_Gbx3Speed, 0, TestName = "IEPC-S Gbx 3speed Job LH"),
		TestCase(IEPC_S_Gbx3Speed, 1, TestName = "IEPC-S Gbx 3speed Job Coach"),
		TestCase(IEPC_S_Gbx3Speed, 2, TestName = "IEPC-S Gbx 3speed Job CO"),
		TestCase(IEPC_S_Gbx3Speed, 3, TestName = "IEPC-S Gbx 3speed Job HU"),
		TestCase(IEPC_S_Gbx3Speed, 4, TestName = "IEPC-S Gbx 3speed Job IU"),
		TestCase(IEPC_S_Gbx3Speed, 5, TestName = "IEPC-S Gbx 3speed Job RD"),
		TestCase(IEPC_S_Gbx3Speed, 6, TestName = "IEPC-S Gbx 3speed Job SU"),
		TestCase(IEPC_S_Gbx3Speed, 7, TestName = "IEPC-S Gbx 3speed Job U"),
		TestCase(IEPC_S_Gbx3Speed, 8, TestName = "IEPC-S Gbx 3speed Job UD"),

		TestCase(IEPC_S_Gbx3Speed_drag, 0, TestName = "IEPC-S Gbx 3speed sep dragCurves Job LH"),
		TestCase(IEPC_S_Gbx3Speed_drag, 1, TestName = "IEPC-S Gbx 3speed sep dragCurves Job Coach"),
		TestCase(IEPC_S_Gbx3Speed_drag, 2, TestName = "IEPC-S Gbx 3speed sep dragCurves Job CO"),
		TestCase(IEPC_S_Gbx3Speed_drag, 3, TestName = "IEPC-S Gbx 3speed sep dragCurves Job HU"),
		TestCase(IEPC_S_Gbx3Speed_drag, 4, TestName = "IEPC-S Gbx 3speed sep dragCurves Job IU"),
		TestCase(IEPC_S_Gbx3Speed_drag, 5, TestName = "IEPC-S Gbx 3speed sep dragCurves Job RD"),
		TestCase(IEPC_S_Gbx3Speed_drag, 6, TestName = "IEPC-S Gbx 3speed sep dragCurves Job SU"),
		TestCase(IEPC_S_Gbx3Speed_drag, 7, TestName = "IEPC-S Gbx 3speed sep dragCurves Job U"),
		TestCase(IEPC_S_Gbx3Speed_drag, 8, TestName = "IEPC-S Gbx 3speed sep dragCurves Job UD"),

		TestCase(IEPC_S_Gbx3SpeedAxle, 0, TestName = "IEPC-S Gbx 3speed + Axle Job LH"),
		TestCase(IEPC_S_Gbx3SpeedAxle, 1, TestName = "IEPC-S Gbx 3speed + Axle Job Coach"),
		TestCase(IEPC_S_Gbx3SpeedAxle, 2, TestName = "IEPC-S Gbx 3speed + Axle Job CO"),
		TestCase(IEPC_S_Gbx3SpeedAxle, 3, TestName = "IEPC-S Gbx 3speed + Axle Job HU"),
		TestCase(IEPC_S_Gbx3SpeedAxle, 4, TestName = "IEPC-S Gbx 3speed + Axle Job IU"),
		TestCase(IEPC_S_Gbx3SpeedAxle, 5, TestName = "IEPC-S Gbx 3speed + Axle Job RD"),
		TestCase(IEPC_S_Gbx3SpeedAxle, 6, TestName = "IEPC-S Gbx 3speed + Axle Job SU"),
		TestCase(IEPC_S_Gbx3SpeedAxle, 7, TestName = "IEPC-S Gbx 3speed + Axle Job U"),
		TestCase(IEPC_S_Gbx3SpeedAxle, 8, TestName = "IEPC-S Gbx 3speed + Axle Job UD"),

		TestCase(IEPC_S_Gbx3SpeedWhl1, 0, TestName = "IEPC-S Gbx 3speed DTWheel-1 Job LH"),
		TestCase(IEPC_S_Gbx3SpeedWhl1, 1, TestName = "IEPC-S Gbx 3speed DTWheel-1 Job Coach"),
		TestCase(IEPC_S_Gbx3SpeedWhl1, 2, TestName = "IEPC-S Gbx 3speed DTWheel-1 Job CO"),
		TestCase(IEPC_S_Gbx3SpeedWhl1, 3, TestName = "IEPC-S Gbx 3speed DTWheel-1 Job HU"),
		TestCase(IEPC_S_Gbx3SpeedWhl1, 4, TestName = "IEPC-S Gbx 3speed DTWheel-1 Job IU"),
		TestCase(IEPC_S_Gbx3SpeedWhl1, 5, TestName = "IEPC-S Gbx 3speed DTWheel-1 Job RD"),
		TestCase(IEPC_S_Gbx3SpeedWhl1, 6, TestName = "IEPC-S Gbx 3speed DTWheel-1 Job SU"),
		TestCase(IEPC_S_Gbx3SpeedWhl1, 7, TestName = "IEPC-S Gbx 3speed DTWheel-1 Job U"),
		TestCase(IEPC_S_Gbx3SpeedWhl1, 8, TestName = "IEPC-S Gbx 3speed DTWheel-1 Job UD"),

		TestCase(IEPC_S_Gbx3SpeedWhl2, 0, TestName = "IEPC-S Gbx 3speed DTWheel-2 Job LH"),
		TestCase(IEPC_S_Gbx3SpeedWhl2, 1, TestName = "IEPC-S Gbx 3speed DTWheel-2 Job Coach"),
		TestCase(IEPC_S_Gbx3SpeedWhl2, 2, TestName = "IEPC-S Gbx 3speed DTWheel-2 Job CO"),
		TestCase(IEPC_S_Gbx3SpeedWhl2, 3, TestName = "IEPC-S Gbx 3speed DTWheel-2 Job HU"),
		TestCase(IEPC_S_Gbx3SpeedWhl2, 4, TestName = "IEPC-S Gbx 3speed DTWheel-2 Job IU"),
		TestCase(IEPC_S_Gbx3SpeedWhl2, 5, TestName = "IEPC-S Gbx 3speed DTWheel-2 Job RD"),
		TestCase(IEPC_S_Gbx3SpeedWhl2, 6, TestName = "IEPC-S Gbx 3speed DTWheel-2 Job SU"),
		TestCase(IEPC_S_Gbx3SpeedWhl2, 7, TestName = "IEPC-S Gbx 3speed DTWheel-2 Job U"),
		TestCase(IEPC_S_Gbx3SpeedWhl2, 8, TestName = "IEPC-S Gbx 3speed DTWheel-2 Job UD"),

		TestCase(IEPC_S_Gbx1Speed, 0, TestName = "IEPC-S Gbx 1speed Job LH"),
		TestCase(IEPC_S_Gbx1Speed, 1, TestName = "IEPC-S Gbx 1speed Job Coach"),
		TestCase(IEPC_S_Gbx1Speed, 2, TestName = "IEPC-S Gbx 1speed Job CO"),
		TestCase(IEPC_S_Gbx1Speed, 3, TestName = "IEPC-S Gbx 1speed Job HU"),
		TestCase(IEPC_S_Gbx1Speed, 4, TestName = "IEPC-S Gbx 1speed Job IU"),
		TestCase(IEPC_S_Gbx1Speed, 5, TestName = "IEPC-S Gbx 1speed Job RD"),
		TestCase(IEPC_S_Gbx1Speed, 6, TestName = "IEPC-S Gbx 1speed Job SU"),
		TestCase(IEPC_S_Gbx1Speed, 7, TestName = "IEPC-S Gbx 1speed Job U"),
		TestCase(IEPC_S_Gbx1Speed, 8, TestName = "IEPC-S Gbx 1speed Job UD"),

		TestCase(IEPC_S_Gbx1SpeedAxl, 0, TestName = "IEPC-S Gbx 1speed + Axle Job LH"),
		TestCase(IEPC_S_Gbx1SpeedAxl, 1, TestName = "IEPC-S Gbx 1speed + Axle Job Coach"),
		TestCase(IEPC_S_Gbx1SpeedAxl, 2, TestName = "IEPC-S Gbx 1speed + Axle Job CO"),
		TestCase(IEPC_S_Gbx1SpeedAxl, 3, TestName = "IEPC-S Gbx 1speed + Axle Job HU"),
		TestCase(IEPC_S_Gbx1SpeedAxl, 4, TestName = "IEPC-S Gbx 1speed + Axle Job IU"),
		TestCase(IEPC_S_Gbx1SpeedAxl, 5, TestName = "IEPC-S Gbx 1speed + Axle Job RD"),
		TestCase(IEPC_S_Gbx1SpeedAxl, 6, TestName = "IEPC-S Gbx 1speed + Axle Job SU"),
		TestCase(IEPC_S_Gbx1SpeedAxl, 7, TestName = "IEPC-S Gbx 1speed + Axle Job U"),
		TestCase(IEPC_S_Gbx1SpeedAxl, 8, TestName = "IEPC-S Gbx 1speed + Axle Job UD"),

		TestCase(IEPC_S_Gbx1SpeedWhl1, 0, TestName = "IEPC-S Gbx 1speed DTWheel-1 Job LH"),
		TestCase(IEPC_S_Gbx1SpeedWhl1, 1, TestName = "IEPC-S Gbx 1speed DTWheel-1 Job Coach"),
		TestCase(IEPC_S_Gbx1SpeedWhl1, 2, TestName = "IEPC-S Gbx 1speed DTWheel-1 Job CO"),
		TestCase(IEPC_S_Gbx1SpeedWhl1, 3, TestName = "IEPC-S Gbx 1speed DTWheel-1 Job HU"),
		TestCase(IEPC_S_Gbx1SpeedWhl1, 4, TestName = "IEPC-S Gbx 1speed DTWheel-1 Job IU"),
		TestCase(IEPC_S_Gbx1SpeedWhl1, 5, TestName = "IEPC-S Gbx 1speed DTWheel-1 Job RD"),
		TestCase(IEPC_S_Gbx1SpeedWhl1, 6, TestName = "IEPC-S Gbx 1speed DTWheel-1 Job SU"),
		TestCase(IEPC_S_Gbx1SpeedWhl1, 7, TestName = "IEPC-S Gbx 1speed DTWheel-1 Job U"),
		TestCase(IEPC_S_Gbx1SpeedWhl1, 8, TestName = "IEPC-S Gbx 1speed DTWheel-1 Job UD"),

		TestCase(IEPC_S_Gbx1SpeedWhl2, 0, TestName = "IEPC-S Gbx 1speed DTWheel-2 Job LH"),
		TestCase(IEPC_S_Gbx1SpeedWhl2, 1, TestName = "IEPC-S Gbx 1speed DTWheel-2 Job Coach"),
		TestCase(IEPC_S_Gbx1SpeedWhl2, 2, TestName = "IEPC-S Gbx 1speed DTWheel-2 Job CO"),
		TestCase(IEPC_S_Gbx1SpeedWhl2, 3, TestName = "IEPC-S Gbx 1speed DTWheel-2 Job HU"),
		TestCase(IEPC_S_Gbx1SpeedWhl2, 4, TestName = "IEPC-S Gbx 1speed DTWheel-2 Job IU"),
		TestCase(IEPC_S_Gbx1SpeedWhl2, 5, TestName = "IEPC-S Gbx 1speed DTWheel-2 Job RD"),
		TestCase(IEPC_S_Gbx1SpeedWhl2, 6, TestName = "IEPC-S Gbx 1speed DTWheel-2 Job SU"),
		TestCase(IEPC_S_Gbx1SpeedWhl2, 7, TestName = "IEPC-S Gbx 1speed DTWheel-2 Job U"),
		TestCase(IEPC_S_Gbx1SpeedWhl2, 8, TestName = "IEPC-S Gbx 1speed DTWheel-2 Job UD"),
		]
		public void IEPCRunJob(string jobFile, int cycleIdx)
		{
			var inputProvider = JSONInputDataFactory.ReadJsonJob(jobFile);

			var writer = new FileOutputWriter(jobFile);
			var factory = SimulatorFactory.CreateSimulatorFactory(ExecutionMode.Engineering, inputProvider, writer);
			factory.Validate = false;
			factory.WriteModalResults = true;
			

			var sumContainer = new SummaryDataContainer(writer);
			var jobContainer = new JobContainer(sumContainer);

			factory.SumData = sumContainer;
			
			if (cycleIdx < 0) {
				jobContainer.AddRuns(factory);
				jobContainer.Execute();
				jobContainer.WaitFinished();
				Assert.IsTrue(jobContainer.GetProgress().All(x => x.Value.Success));
			} else {
				var run = factory.SimulationRuns().ToArray()[cycleIdx];

				Assert.NotNull(run);

				var pt = run.GetContainer();

				Assert.NotNull(pt);

				run.Run();
				Assert.IsTrue(run.FinishedWithoutErrors);
			}
		}

	}
}