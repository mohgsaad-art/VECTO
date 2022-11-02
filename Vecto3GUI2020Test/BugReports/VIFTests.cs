using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ninject;
using NUnit.Framework;
using NUnit.Framework.Internal;
using VECTO3GUI2020.ViewModel.Implementation;
using VECTO3GUI2020.ViewModel.Interfaces;

namespace Vecto3GUI2020Test.BugReports
{
	[TestFixture]
	public class VifTests : ViewModelTestBase
	{
		public const string primaryDecimalTestFile = "PrimaryDecimal/primary_heavyBus group41_nonSmart_rounded_decimals.xml";

		
		[TestCase(VifTests.primaryDecimalTestFile, TestName="PneumaticsCompressorDrive")]
		public async Task CreateAndLoadVifWithWrongDecimalCount(string fileName)
		{
            //Load JobFile 
			var jobListViewModel = _kernel.Get<IJobListViewModel>() as JobListViewModel;
			await jobListViewModel.AddJobAsync(GetTestDataPath(fileName: fileName));
			Assert.AreEqual(1, jobListViewModel.Jobs.Count);
			jobListViewModel.Jobs[0].Selected = true;

			//Start Simulation
			TestContext.Write("Starting simulation ... ");
			Stopwatch stop = Stopwatch.StartNew();
			await jobListViewModel.RunSimulationExecute();
			stop.Stop();
			TestContext.WriteLine($"Done! ({stop.Elapsed.TotalSeconds}s)");


			var vifName = fileName.Replace(".xml", ".RSLT_VIF.xml");
			TestContext.WriteLine($"Trying to add {vifName} to JobList");
			await jobListViewModel.AddJobAsync(GetTestDataPath(fileName: vifName));
			Assert.AreEqual(2, jobListViewModel.Jobs.Count);

			foreach (var documentViewModel in jobListViewModel.Jobs) {
				TestContext.WriteLine($"{documentViewModel.DataSource.SourcePath}");
			}

			return;
		}





	}
}
