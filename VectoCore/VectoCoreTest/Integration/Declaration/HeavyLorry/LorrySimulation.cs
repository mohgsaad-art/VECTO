

//#define singlethreaded

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Threading;
using System.Xml.Linq;
using System.Xml.Schema;
using Moq;
using Ninject;
using NUnit.Framework;
using NUnit.Framework.Constraints;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.InputData.FileIO.JSON;
using TUGraz.VectoCore.InputData.FileIO.XML;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration;
using TUGraz.VectoCore.InputData.Reader.ComponentData;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.Models.Simulation.Impl.SimulatorFactory;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Impl;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.OutputData.FileIO;
using TUGraz.VectoCore.OutputData.XML;
using TUGraz.VectoCore.Tests.Models.Simulation;
using TUGraz.VectoCore.Tests.TestUtils;
using TUGraz.VectoCore.Tests.Utils;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.Tests.Integration.Declaration.HeavyLorry;



[TestFixture]
//[Parallelizable(ParallelScope.Children)]
public class LorrySimulation
{
	private enum PTOState
	{
		VehicleDriving, // speed >= 0
		VehicleStopped, // speed == 0, pto might be activated
		PTOActive, // Pto is active 
	}

	private const string BASE_DIR = @"TestData/Integration/DeclarationMode/2nd_AmendmDeclMode/";
	private const string Group5_HEV_P2_OVC = @"HeavyLorry/P-HEV/Group5_HEV_P2_ovc.xml";
	private const string Group5_HEV_P2_SuperCap = @"HeavyLorry/P-HEV/Group5_HEV_P2_supercap.xml";
	private const string Group5_HEV_P3_OVC = @"HeavyLorry/P-HEV/Group5_HEV_P3_ovc.xml";
	private const string Group5_HEV_P4_OVC = @"HeavyLorry/P-HEV/Group5_HEV_P4_ovc.xml";
	private const string Group5_HEV_P2_5_OVC = @"HeavyLorry/P-HEV/Group5_HEV_P2_5_ovc.xml";
	private const string Group5_HEV_S2_OVC = @"HeavyLorry/S-HEV/Group2_HEV_S2_ovc.xml";
	private const string Group5_HEV_S3_OVC = @"HeavyLorry/S-HEV/Group2_HEV_S3_ovc.xml";
	private const string Group5_HEV_S4_OVC = @"HeavyLorry/S-HEV/Group2_HEV_S4_ovc.xml";
	private const string Group5_HEV_S_IEPC = @"HeavyLorry/S-HEV/Group2_HEV_IEPC_S.xml";
	private const string Group5_HEV_S_IEPC_pto = @"HeavyLorry/S-HEV/Group2_HEV_IEPC_S_pto.xml";
	private const string Group5_HEV_S_IEPC_ovc = @"HeavyLorry/S-HEV/Group2_HEV_IEPC_S_ovc.xml";

	private const string Group5_PEV_E3 = @"HeavyLorry/PEV/Group5_ PEV_E3_ES_Standard.xml";
	private const string Group2_HEV_IEPC_S_StdVal = @"HeavyLorry/S-HEV/Group2_HEV_IEPC_S_standard_values.xml";
	private const string ConventionalHeavyLorry = @"HeavyLorry/Conventional/Group5_Conv_ES_Standard.xml";


	public StandardKernel Kernel
	{
		get => _kernel.Value;
	}

	private IXMLInputDataReader _xmlReader;
	private XmlSchemaSet _cifSchema = XMLValidator.GetXMLSchema(XmlDocumentType.CustomerReport);
	private XmlSchemaSet _mrfSchema = XMLValidator.GetXMLSchema(XmlDocumentType.ManufacturerReport);
	private ThreadLocal<StandardKernel> _kernel;


	[OneTimeSetUp]
	public void OneTimeSetup()
	{
		_kernel = new ThreadLocal<StandardKernel>(() => new StandardKernel(new VectoNinjectModule()));
		_xmlReader = Kernel.Get<IXMLInputDataReader>();
	}

	private TestMissionFilter TestMissionFilter()
	{
		Kernel.Rebind<IMissionFilter>().To<TestMissionFilter>().InSingletonScope();
		var missionFilter = Kernel.Get<IMissionFilter>() as TestMissionFilter;
		Assert.NotNull(missionFilter);
		return missionFilter;
	}

	private TestDeclarationCycleFactoryStartPoint StartPointCycleFactory()
	{
		Kernel.Rebind<IDeclarationCycleFactory>().To<TestDeclarationCycleFactoryStartPoint>().InSingletonScope();
		var missionFilter = Kernel.Get<IDeclarationCycleFactory>() as TestDeclarationCycleFactoryStartPoint;
		Assert.NotNull(missionFilter);
		return missionFilter;
	}



	//Conventional
	[TestCase(ConventionalHeavyLorry, TestName = "Stefan_Conv")]
	//S-HEV
	[TestCase(Group5_HEV_S2_OVC)]
	[TestCase(@"HeavyLorry/S-HEV/Group2_HEV_S3_stefan.xml", TestName = "Stefan_S2")]
	//PEV
	[TestCase(Group5_PEV_E3)]
	[TestCase(@"HeavyLorry/PEV/PEV_heavyLorry_APT_E2.xml")]
	//P-HEV
	[TestCase(Group5_HEV_P2_OVC)]
	//P-HEV with SuperCap 0 Ohm R_i
	[TestCase(Group5_HEV_P2_SuperCap, TestName = "P2 HEV SuperCap")]
	public void HeavyLorrySimulationTest(string jobFile)
	{
#if singlethreaded
		RunFullSimulation(jobFile, false);
#else
		RunFullSimulation(jobFile, true, false);
#endif
	}

	//S-HEV
	[TestCase(@"MediumLorry/S-HEV/Group2_HEV_S2.xml")]


	//P-HEV
	[TestCase(@"MediumLorry/P-HEV/Group5_HEV_P3_ovc.xml")]

	//PEV
	[TestCase(@"MediumLorry/PEV/Group5_ PEV_E3_ES_Standard.xml")]
	public void MediumLorrySimulationTest(string jobFile)
	{
		RunFullSimulation(jobFile, true, false);
	}

	[TestCase(@"HeavyLorry/Exempted/exempted_heavy_lorry.xml")]
	public void ExemptedTest(string jobFile)
	{
		RunFullSimulation(jobFile, true);
	}

	public void RunFullSimulation(string jobFile, bool multiThreaded = true, bool disableIterativeRuns = false)
	{
		var filePath = Path.Combine(BASE_DIR, jobFile);
		var runsFactory = GetSimulatorFactory(filePath, out var dataProvider, out var fileWriter,
			out var summaryDataContainer);
		if (disableIterativeRuns) {
			DisableIterativeRuns(runsFactory);
		}

		runsFactory.WriteModalResults = true;
		var jobContainer = new JobContainer(summaryDataContainer) { };
		jobContainer.AddRuns(runsFactory);
		PrintRuns(jobContainer, null);

		jobContainer.Execute(multiThreaded);

		if (multiThreaded) {
			jobContainer.WaitFinished();
		} else {
			while (!jobContainer.AllCompleted) { }

			;
		}


		Assert.IsTrue(jobContainer.AllCompleted);
		Assert.IsTrue(jobContainer.Runs.TrueForAll(runEntry => runEntry.Success));
		PrintRuns(jobContainer, fileWriter);
		PrintFiles(fileWriter);

		Assert.AreEqual(0, GetResultCount(jobContainer.Runs.First().Run.GetContainer().RunData.Report),
			"_resultCount must be zero after simulation");

		var mrfPath = fileWriter.GetWrittenFiles()[ReportType.DeclarationReportManufacturerXML];
		var cifPath = fileWriter.GetWrittenFiles()[ReportType.DeclarationReportCustomerXML];
		XDocument.Load(mrfPath).Validate(_mrfSchema, (sender, args) => Assert.Fail(args.Message));
		XDocument.Load(cifPath).Validate(_cifSchema, (sender, args) => Assert.Fail(args.Message));

		VSUM_order_test(fileWriter.SumFileName, jobContainer.Runs.First().Run.GetContainer().RunData);
	}

	private ISimulatorFactory GetSimulatorFactory(string filePath, out IDeclarationInputDataProvider dataProvider,
		out FileOutputWriter fileWriter, out SummaryDataContainer sumWriter)
	{
		dataProvider = _xmlReader.CreateDeclaration(filePath);
		fileWriter = new FileOutputWriter(filePath);

		var runsFactory = SimulatorFactory.CreateSimulatorFactory(ExecutionMode.Declaration, dataProvider, fileWriter);
		sumWriter = new SummaryDataContainer(fileWriter);
		runsFactory.SumData = sumWriter;
		return runsFactory;
	}


	[TestCase(Group5_HEV_S2_OVC)]
	public void VMODUnitTest(string jobFile)
	{
		var columnsWithoutUnit = new List<ModalResultField> {
			ModalResultField.ICEOn,
			ModalResultField.EM_Off_,
			ModalResultField.HybridStrategyScore,
			ModalResultField.HybridStrategySolution,
			ModalResultField.HybridStrategyState,
		}.Select(m => m.GetCaption()).Concat(new List<string>() {
			"DriverAction",
			"EcoRollConditionsMet",
			"PCCSegment",
			"PCCState",
		});


		//	.ToHashSet();

		GetEmptySumAndModData(jobFile, out var sumDataContainer, out var run, out var modData, out var sumData);

		var columnsWithoutUnitHashSet = new HashSet<string>();
		foreach (var col in columnsWithoutUnit) {
			var emPositions = run.GetContainer().PowertrainInfo.ElectricMotorPositions;
			foreach (var emPosition in emPositions) {
				columnsWithoutUnitHashSet.Add(string.Format(col, emPosition.GetLabel()));
			}
		}


		List<string> columnHeaders = new List<string>();
		foreach (DataColumn modColumn in modData.Columns) {
			columnHeaders.Add(modColumn.Caption);
		}


		var unitColumnHeaders = columnHeaders.Where(h => !columnsWithoutUnitHashSet.Contains(h));
		foreach (string header in unitColumnHeaders) {
			TestContext.WriteLine(header);
		}

		foreach (string header in unitColumnHeaders) {
			var regex = new Regex(@"\[\S+\]");
			Assert.AreEqual(1, regex.Matches(header).Count(), $"missing unit {header}");
		}


		var sumCols = new List<string>();
		TestContext.WriteLine("\n -----------------------SumFile captions----------------------------- \n");
		foreach (DataColumn col in sumDataContainer.Table.Columns) {
			var caption = col.Caption;
			sumCols.Add(caption);
			TestContext.WriteLine(caption);
		}

		foreach (string sumHeader in sumCols) {
			var regex = new Regex(@"\[\S+\]");
			Assert.IsTrue(regex.Matches(sumHeader).Count() <= 1, $"double units {sumHeader}");
		}
	}

	#region VSUM_HELPER

	private void GetEmptySumAndModData(string jobFile, out SummaryDataContainer sumDataContainer, out IVectoRun run,
		out TableData modData, out TableData sumData)
	{
		var filePath = Path.Combine(BASE_DIR, jobFile);
		sumDataContainer = null;
		var jobContainer =
			GetJobContainer(jobFile, null, out var fileWriter, out var runs, out sumDataContainer, false);
		//sumDataContainer.Finish();
		run = runs.First();
		sumData = null;
		jobContainer.AddRun(run);

		run.GetContainer().ModalData[ModalResultField.time] = 1.SI<Second>(); //fake duration for run
		run.GetContainer().ModalData[ModalResultField.Gear] = 2;
		run.GetContainer().ModalData.CommitSimulationStep();
		//run.GetContainer().FinishSingleSimulationRun(null);
		var modFileName = fileWriter.GetModDataFileName(run.RunName, run.CycleName, run.RunSuffix);

		var sumFileName = fileWriter.SumFileName;
		Assert.IsTrue(File.Exists(modFileName));
		Assert.IsTrue(File.Exists(sumFileName));
		modData = VectoCSVFile.Read(modFileName, false, true);
	}

	public void AssertVSUMElectricMotorFields(string vsumFileName, VectoRunData runData)
	{
		var sumData = VectoCSVFile.Read(vsumFileName, false, true);
		var em_positions = runData.ElectricMachinesData.Select(em => em.Item1);
		foreach (var em_pos in runData.ElectricMachinesData.Select(e => e.Item1)) {
			Tuple<string, Type>[] columns;
			if (em_pos == PowertrainPosition.IEPC) {
				columns = SummaryDataContainer.IEPCColumns;
			} else {
				columns = SummaryDataContainer.ElectricMotorColumns;
			}

			foreach (var (s, type) in columns) {
				var colName = string.Format(s, em_pos.GetLabel());
				Assert.IsTrue(sumData.Columns.Contains(colName), $"{colName} missing in sum file");
			}
		}
	}

	public void VSUM_order_test(string fileName, VectoRunData runData)
	{
		if (runData.Exempted) {
			return;
		}


		List<string> GbxTimeShareFields()
		{

			var gbxTimeShareFields = new List<string> { };
			if (runData.GearboxData?.Gears != null && runData.GearboxData.Gears.Count > 0) {
				for (var i = 0; i <= runData.GearboxData.Gears.Count; i++) {
					gbxTimeShareFields.Add(string.Format(SumDataFields.TIME_SHARE_PER_GEAR_FORMAT, i));
				}

			}

			return gbxTimeShareFields;
		}

		#region local helper

		void AssertColumnNotPresent(TableData tableData, List<string> notPresent)
		{
			foreach (var name in notPresent) {
				Assert.IsFalse(tableData.Columns.Contains(name), name);
			}
		}

		void AssertOrder(TableData tableData, List<string> ordered) { }

		void SearchForPattern(TableData tableData, List<string> pattern)
		{
			if (pattern.Count == 0) {
				return;
			}

			var comparePattern = false;
			using (var enumerator = pattern.GetEnumerator()) {
				enumerator.MoveNext();
				foreach (DataColumn column in tableData.Columns) {
					if (!comparePattern && column.ColumnName == enumerator.Current) {
						comparePattern = true;
					}

					if (comparePattern) {
						TestContext.Write(column.ColumnName + "|" + enumerator.Current);
						if (column.ColumnName != enumerator.Current) {
							TestContext.WriteLine("X");
							Assert.Fail($"expected {enumerator.Current} got {column.ColumnName}");
						}

						TestContext.WriteLine("OK");
						if (!enumerator.MoveNext()) {
							return;
						}
					}
				}

				Assert.Fail($"Reached end of table searching for {enumerator.Current}");
			}

		}

		#endregion

		var sumData = VectoCSVFile.Read(fileName, false, true);
		var fcFields = new List<string>() {
			//FUEL
			SumDataFields.FCMAP_H,
			SumDataFields.FCMAP_KM,
			SumDataFields.FCNCVC_H,
			SumDataFields.FCNCVC_KM,
			SumDataFields.FCWHTCC_H,
			SumDataFields.FCWHTCC_KM,
			SumDataFields.FCESS_H,
			SumDataFields.FCESS_KM,
			SumDataFields.FCESS_H_CORR,
			SumDataFields.FCESS_KM_CORR,
			SumDataFields.FC_BusAux_PS_CORR_H,
			SumDataFields.FC_BusAux_PS_CORR_KM,
			SumDataFields.FC_BusAux_ES_CORR_H,
			SumDataFields.FC_BusAux_ES_CORR_KM,
			SumDataFields.FCWHR_H_CORR,
			SumDataFields.FCWHR_KM_CORR,
			SumDataFields.FC_HEV_SOC_H,
			SumDataFields.FC_HEV_SOC_KM,
			SumDataFields.FC_HEV_SOC_CORR_H,
			SumDataFields.FC_HEV_SOC_CORR_KM,
			SumDataFields.FC_AUXHTR_H,
			SumDataFields.FC_AUXHTR_KM,
			SumDataFields.FC_AUXHTR_H_CORR,
			SumDataFields.FC_AUXHTR_KM_CORR,
			SumDataFields.FCFINAL_H,
			SumDataFields.FCFINAL_KM,
			SumDataFields.FCFINAL_LITERPER100KM,
			SumDataFields.FCFINAL_LITERPER100TKM,
			SumDataFields.FCFINAL_LiterPer100M3KM,
			SumDataFields.FCFINAL_LiterPer100PassengerKM,
			SumDataFields.SPECIFIC_FC,
			SumDataFields.K_VEHLINE,
			SumDataFields.K_ENGLINE,


		};
		var CO2fields = new List<string> {
			SumDataFields.CO2_KM,
			SumDataFields.CO2_TKM,
			SumDataFields.CO2_M3KM,
		};
		var EC_el = new List<string> {
			SumDataFields.EC_el_SOC,
			SumDataFields.EC_el_SOC_corr,
			SumDataFields.EC_el_final,
			SumDataFields.EC_el_final_KM,
			SumDataFields.EC_el_final_TKM,
			SumDataFields.EC_el_final_M3KM,



		};
		var p_hev_fields = new List<string> {
			SumDataFields.f_equiv
		};
		var REESS_fields = new List<string> {
			SumDataFields.REESS_StartSoC,
			SumDataFields.REESS_EndSoC,
			SumDataFields.REESS_DeltaEnergy,

			SumDataFields.REESS_MinSoC,
			SumDataFields.REESS_MaxSoC,

			SumDataFields.E_REESS_LOSS,
			SumDataFields.E_REESS_T_chg,
			SumDataFields.E_REESS_T_dischg,
			SumDataFields.E_REESS_int_chg,
			SumDataFields.E_REESS_int_dischg,
		};


		if (runData.JobType == VectoSimulationJobType.ConventionalVehicle) {
			var gbxTimeShareFields = GbxTimeShareFields();
			SearchForPattern(sumData, gbxTimeShareFields);

			if (runData.EngineData.Fuels.Count > 1) {
				foreach (var fuel in runData.EngineData.Fuels.Select(f => f.FuelData.FuelType.GetLabel())) {
					SearchForPattern(sumData,
						new List<string>(fcFields.Select(fc => string.Format(fc, fuel)).Concat(CO2fields)));
				}
			} else {
				SearchForPattern(sumData,
					new List<string>(fcFields.Select(fc => string.Format(fc, "")).Concat(CO2fields)));
			}

		}

		if (runData.JobType is VectoSimulationJobType.BatteryElectricVehicle or VectoSimulationJobType.IEPC_E) {
			//PEV CHECKS
			AssertColumnNotPresent(sumData, CO2fields);
			AssertColumnNotPresent(sumData, fcFields);

			SearchForPattern(sumData, EC_el);
			SearchForPattern(sumData, REESS_fields);
			SearchForPattern(sumData, GbxTimeShareFields());
		}

		if (runData.JobType is VectoSimulationJobType.SerialHybridVehicle or VectoSimulationJobType.IEPC_S) {
			if (runData.EngineData.Fuels.Count > 1) {
				foreach (var fuel in runData.EngineData.Fuels.Select(f => f.FuelData.FuelType.GetLabel())) {
					SearchForPattern(sumData,
						new List<string>(fcFields.Select(fc => string.Format(fc, fuel)).Concat(CO2fields)));
				}
			} else {
				SearchForPattern(sumData,
					new List<string>(fcFields.Select(fc => string.Format(fc, "")).Concat(CO2fields)));
			}

			SearchForPattern(sumData, EC_el);
			SearchForPattern(sumData, REESS_fields);
			SearchForPattern(sumData, GbxTimeShareFields());
		}


		if (runData.JobType is VectoSimulationJobType.ParallelHybridVehicle or VectoSimulationJobType.IHPC) {
			if (runData.EngineData.Fuels.Count > 1) {
				foreach (var fuel in runData.EngineData.Fuels.Select(f => f.FuelData.FuelType.GetLabel())) {
					SearchForPattern(sumData,
						new List<string>(fcFields.Select(fc => string.Format(fc, fuel)).Concat(CO2fields)));
				}
			} else {
				SearchForPattern(sumData,
					new List<string>(fcFields.Select(fc => string.Format(fc, "")).Concat(CO2fields)));
			}

			SearchForPattern(sumData, EC_el);
			SearchForPattern(sumData, p_hev_fields);
			SearchForPattern(sumData, REESS_fields);
			SearchForPattern(sumData, GbxTimeShareFields());
		}
	}

	#endregion

	[TestCase(@"HeavyLorry/S-HEV/Group2_HEV_S4_invalid_pto.xml")]
	[TestCase(@"HeavyLorry/PEV/PEV_heavyLorry_E3_pto_transmission_invalid.xml")]
	public void PTOWithoutTransmissionTest(string jobFile)
	{
		SummaryDataContainer sumDataContainer;
		var exception = Assert.Throws<VectoException>(() =>
			GetJobContainer(jobFile, null, out var fileWriter, out var runs, out sumDataContainer));
		TestContext.WriteLine(exception.Message);
	}


	[TestCase(@"HeavyLorry/P-HEV/Group5_HEV_P2_supercap.xml", 10)]
	[TestCase(@"HeavyLorry/P-HEV/Group5_HEV_P1_supercap.xml", 10)]
	public void PHEV_SuperCap(string jobFile, int nrRuns)
	{
		var jobContainer = GetJobContainer(jobFile, nrRuns, out var fileWriter, out var runs, out var sumDataContainer);
		Assert.AreEqual(0, runs.Count(r =>
			r.GetContainer().RunData.OVCMode is OvcHevMode.ChargeDepleting or OvcHevMode.NotApplicable));

		var run = runs.First(r => {
			var rd = r.GetContainer().RunData;
			return rd.Mission.MissionType == MissionType.UrbanDelivery;
		});

		jobContainer.AddRun(run);
		var modData = ((ModalDataContainer)((VehicleContainer)run.GetContainer()).ModData).Data;
		jobContainer.Execute(false);
		WaitAndAssertSuccess(jobContainer, fileWriter);

	}


	[TestCase(@"HeavyLorry/S-HEV/Group2_HEV_S2_supercap.xml", 6)]
	public void SHEV_SuperCap(string jobFile, int nrRuns)
	{
		var jobContainer = GetJobContainer(jobFile, nrRuns, out var fileWriter, out var runs, out var sumDataContainer);
		Assert.AreEqual(0, runs.Count(r =>
			r.GetContainer().RunData.OVCMode is OvcHevMode.ChargeDepleting or OvcHevMode.NotApplicable));

		var run = runs.First(r => {
			var rd = r.GetContainer().RunData;
			return rd.Mission.MissionType == MissionType.UrbanDelivery;
		});

		jobContainer.AddRun(run);
		var modData = ((ModalDataContainer)((VehicleContainer)run.GetContainer()).ModData).Data;
		jobContainer.Execute(false);
		WaitAndAssertSuccess(jobContainer, fileWriter);
	}

	[TestCase(Group5_HEV_P2_OVC, 20)]
	[TestCase(Group5_HEV_P3_OVC, 20)]
	[TestCase(Group5_HEV_P4_OVC, 20)]
	[TestCase(Group5_HEV_P2_5_OVC, 20)]
	[TestCase(@"MediumLorry/P-HEV/Group5_HEV_P3_ovc.xml", 8, MissionType.UrbanDelivery, LoadingType.ReferenceLoad)]
	public void PHEV_ChargeSustainingIt(string jobFile, int nrRuns, MissionType missionType = MissionType.UrbanDelivery,
		LoadingType loadingType = LoadingType.ReferenceLoad)
	{
		var jobContainer = GetJobContainer(jobFile, nrRuns, out var fileWriter, out var runs, out var sumDataContainer);

		Assert.AreEqual(0, runs.Count(r => r.GetContainer().RunData.OVCMode == OvcHevMode.NotApplicable));

		runs = runs.Where(run => {
			var rd = run.GetContainer().RunData;
			return rd.OVCMode == OvcHevMode.ChargeSustaining &&
					rd.Mission.MissionType == MissionType.UrbanDelivery && rd.Loading == loadingType;
		}).ToList();

		jobContainer.AddRun(runs.Single());
		var modData = ((ModalDataContainer)((VehicleContainer)runs.Single().GetContainer()).ModData).Data;

		jobContainer.Execute(false);
		WaitAndAssertSuccess(jobContainer, fileWriter);
	}

	[TestCase(Group5_HEV_P2_OVC)]
	public void PHEV_CD_CS_weighted(string jobFile)
	{
		var jobContainer = GetJobContainer(jobFile, null, out var fileWriter, out var runs, out var sumDataContainer);

		Assert.AreEqual(0, runs.Count(r => r.GetContainer().RunData.OVCMode == OvcHevMode.NotApplicable));

		runs = runs.Where(run => {
			var rd = run.GetContainer().RunData;
			return rd.OVCMode is OvcHevMode.ChargeSustaining or OvcHevMode.ChargeDepleting &&
					rd.Mission.MissionType == MissionType.RegionalDelivery && rd.Loading == LoadingType.ReferenceLoad;
		}).ToList();

		Assert.AreEqual(2, runs.Count);

		foreach (var run in runs) {
			jobContainer.AddRun(run);
		}

		SetResultCountInReport(2, runs.First().GetContainer().RunData.Report);


		jobContainer.Execute(false);
		WaitAndAssertSuccess(jobContainer, fileWriter);
		AssertVSUMElectricMotorFields(fileWriter.SumFileName, runs.First().GetContainer().RunData);

	}


	[TestCase(Group5_HEV_P2_OVC, 20)]
	[TestCase(Group5_HEV_P3_OVC, 20)]
	[TestCase(Group5_HEV_P4_OVC, 20)]
	[TestCase(Group5_HEV_P2_5_OVC, 20)]
	[TestCase(@"HeavyLorry/P-HEV/Group5_HEV_IHPC.xml", 20)]
	[TestCase(@"HeavyLorry/P-HEV/Group5_HEV_P2_OVC_stefan.xml", 20, MissionType.UrbanDelivery, LoadingType.LowLoading)]
	[TestCase(@"HeavyLorry/P-HEV/Group5_HEV_P3_OVC_stefan.xml", 20)]
	public void PHEV_ChargeDepleting(string jobFile, int nrRuns, MissionType missionType = MissionType.UrbanDelivery,
		LoadingType loadingType = LoadingType.ReferenceLoad)
	{
		var jobContainer = GetJobContainer(jobFile, nrRuns, out var fileWriter, out var runs, out var sumDataContainer);


		Assert.AreEqual(0, runs.Count(r => r.GetContainer().RunData.OVCMode == OvcHevMode.NotApplicable));

		runs = runs.Where(run => {
			var rd = run.GetContainer().RunData;
			return rd.OVCMode == OvcHevMode.ChargeDepleting &&
					rd.Mission.MissionType == missionType && rd.Loading == loadingType;
		}).ToList();

		jobContainer.AddRun(runs.Single());
		var modData = ((ModalDataContainer)((VehicleContainer)runs.Single().GetContainer()).ModData).Data;
		jobContainer.Execute(false);
		WaitAndAssertSuccess(jobContainer, fileWriter);
		AssertVSUMElectricMotorFields(fileWriter.SumFileName, runs.First().GetContainer().RunData);
	}

	[TestCase(Group5_HEV_S2_OVC, 12)]
	[TestCase(Group5_HEV_S3_OVC, 12)]
	[TestCase(Group5_HEV_S4_OVC, 12)]
	[TestCase(Group5_HEV_S_IEPC_ovc, 12)]
	//[TestCase(@"HeavyLorry/S-HEV/Group2_HEV_S4_ovc.xml",12)]
	public void SHEV_ChargeDepleting(string jobFile, int nrRuns)
	{
		SummaryDataContainer sumDataContainer;
		var jobContainer = GetJobContainer(jobFile, nrRuns, out var fileWriter, out var runs, out sumDataContainer);

		Assert.AreEqual(runs.Count(r => r.GetContainer().RunData.OVCMode == OvcHevMode.ChargeDepleting),
			runs.Count(r => r.GetContainer().RunData.OVCMode == OvcHevMode.ChargeSustaining));
		Assert.AreEqual(0, runs.Count(r => r.GetContainer().RunData.OVCMode == OvcHevMode.NotApplicable));


		runs = runs.Where(run => {
			var rd = run.GetContainer().RunData;
			return rd.OVCMode == OvcHevMode.ChargeDepleting &&
					rd.Mission.MissionType == MissionType.UrbanDelivery && rd.Loading == LoadingType.ReferenceLoad;
		}).ToList();

		jobContainer.AddRun(runs.Single());

		Assert.AreEqual(1, jobContainer.Runs.Count);

		var modData = ((ModalDataContainer)((VehicleContainer)runs.Single().GetContainer()).ModData).Data;
		jobContainer.Execute(false);
		WaitAndAssertSuccess(jobContainer, fileWriter);


		var soc = modData.Rows[0].Field<Scalar>(ModalResultField.REESSStateOfCharge.GetName()); //init soc
		foreach (DataRow modDataRow in modData.Rows) {
			Assert.AreEqual(soc, modDataRow.Field<Scalar>(ModalResultField.REESSStateOfCharge.GetName()));
			Assert.IsFalse(modDataRow.Field<bool>(ModalResultField.ICEOn.GetName()));
			AssertSHEV_PEV_Conditioning(modDataRow, runs.Single());
		}

		Assert.IsTrue(modData.Rows.Count > 0);
		AssertVSUMElectricMotorFields(fileWriter.SumFileName, runs.First().GetContainer().RunData);
	}

	[TestCase(Group5_HEV_S2_OVC, 12)]
	[TestCase(Group5_HEV_S3_OVC, 12)]
	[TestCase(@"HeavyLorry/S-HEV/Group2_HEV_S4.xml", 6)]
	[TestCase(Group5_HEV_S_IEPC, 6)]
	[TestCase(Group2_HEV_IEPC_S_StdVal, 6)]
	[TestCase(@"HeavyLorry/S-HEV/Group2_HEV_S2.xml", 6)]
	public void SHEV_ChargeSustaining(string jobFile, int nrRuns)
	{
		SummaryDataContainer sumDataContainer;
		var jobContainer = GetJobContainer(jobFile, nrRuns, out var fileWriter, out var runs, out sumDataContainer);

		if (runs.First().GetContainer().RunData.VehicleData.OffVehicleCharging) {
			Assert.AreEqual(runs.Count(r => r.GetContainer().RunData.OVCMode == OvcHevMode.ChargeDepleting),
				runs.Count(r => r.GetContainer().RunData.OVCMode == OvcHevMode.ChargeSustaining));
		}

		Assert.AreEqual(0, runs.Count(r => r.GetContainer().RunData.OVCMode == OvcHevMode.NotApplicable));

		runs = runs.Where(run => {
			var rd = run.GetContainer().RunData;
			return rd.OVCMode == OvcHevMode.ChargeSustaining &&
					rd.Mission.MissionType == MissionType.UrbanDelivery && rd.Loading == LoadingType.ReferenceLoad;
		}).ToList();

		jobContainer.AddRun(runs.Single());

		Assert.AreEqual(1, jobContainer.Runs.Count);

		var modData = ((ModalDataContainer)((VehicleContainer)runs.Single().GetContainer()).ModData).Data;
		jobContainer.Execute(false);
		WaitAndAssertSuccess(jobContainer, fileWriter);


		var soc = modData.Rows[0].Field<Scalar>(ModalResultField.REESSStateOfCharge.GetName());
		foreach (DataRow modDataRow in modData.Rows) {
			AssertSHEV_PEV_Conditioning(modDataRow, runs.Single());
		}

		Assert.IsTrue(modData.Rows.Count > 0);
	}

	[TestCase(@"HeavyLorry/S-HEV/Group2_HEV_S2_pto_transmission.xml", 6)]
	public void SHEV_PTO_Transmission(string jobFile, int nrRuns)
	{
		SummaryDataContainer sumDataContainer;
		var jobContainer = GetJobContainer(jobFile, nrRuns, out var fileWriter, out var runs, out sumDataContainer);

		Assert.AreEqual(0, runs.Count(r => r.GetContainer().RunData.OVCMode == OvcHevMode.NotApplicable));
		Assert.AreEqual(6, runs.Count(r => r.GetContainer().RunData.OVCMode == OvcHevMode.ChargeSustaining));
		Assert.AreEqual(0, runs.Count(r => r.GetContainer().RunData.OVCMode == OvcHevMode.ChargeDepleting));

		runs = runs.Where(run => {
			var rd = run.GetContainer().RunData;
			return rd.Mission.MissionType == MissionType.LongHaul && rd.Loading == LoadingType.ReferenceLoad;
		}).ToList();

		jobContainer.AddRun(runs.Single());

		Assert.AreEqual(1, jobContainer.Runs.Count);

		var modDataContainer = ((ModalDataContainer)((VehicleContainer)runs.Single().GetContainer()).ModData);
		var modData = modDataContainer.Data;
		var aux = modDataContainer.Auxiliaries.ToDictionary(entry => entry.Key, entry => entry.Value);


		jobContainer.Execute(false);
		WaitAndAssertSuccess(jobContainer, fileWriter);


		modDataContainer.Data = modData;
		modDataContainer.Auxiliaries = aux;
		//modDataContainer.GetValues<double>(ModalResultField



		var ptoTransm = modData.Rows[1][modDataContainer.Auxiliaries[Constants.Auxiliaries.IDs.PTOTransmission]];
		var ptoTransmCaption = modDataContainer.Auxiliaries[Constants.Auxiliaries.IDs.PTOTransmission];
		Assert.IsNotNull(ptoTransm);
		//modData.
		foreach (DataRow row in modData.Rows) {
			if (row.Field<uint>(ModalResultField.Gear.GetShortCaption()) != 0) {
				Assert.IsTrue(row.Field<Watt>(ptoTransmCaption).Value().IsGreater(0));
			}
		}

		Assert.IsTrue(modData.Rows.Count > 0);
	}

	[TestCase(@"HeavyLorry/PEV/PEV_heavyLorry_AMT_E2.xml", 6)]
	[TestCase(Group5_PEV_E3, 10)]
	[TestCase(@"HeavyLorry/PEV/Group5_ PEV_E4.xml", 10)]
	[TestCase(@"HeavyLorry/PEV/PEV_heavyLorry_E4_standardValues.xml", 6)]
	[TestCase(@"HeavyLorry/PEV/Group5_ PEV_IEPC_E.xml", 10)]
	[TestCase(@"HeavyLorry/PEV/PEV_heavyLorry_APT_E2.xml", null)]
	public void PEV(string jobFile, int? nrRuns)
	{
		SummaryDataContainer sumDataContainer;
		var jobContainer = GetJobContainer(jobFile, nrRuns, out var fileWriter, out var runs, out sumDataContainer,
			out var inputProvider);

		Assert.IsTrue(runs.All(run => run.GetContainer().RunData.OVCMode == OvcHevMode.NotApplicable));
		//Assert.AreEqual(0, runs.Count(r => r.GetContainer().RunData.OVCMode == OvcHevMode.NotApplicable));

		Assert.That(runs.All(run => {
			var rd = run.GetContainer().RunData;
			// PEV with APT-S or APT-P transmission are simulated as APT-N
			return rd.VehicleData.InputData.Components.GearboxInputData == null ||
					rd.GearboxData.Type.IsOneOf(GearboxType.AMT, GearboxType.APTN);
		}));

		var run = runs.Single(run => {
			var rd = run.GetContainer().RunData;
			return rd.Mission.MissionType == MissionType.UrbanDelivery && rd.Loading == LoadingType.ReferenceLoad;
		});
		jobContainer.AddRun(run);

		AssertPevAUXConnectToREESS(run.GetContainer().RunData);
		CheckPEVHVACInRunData(run.GetContainer().RunData, inputProvider.JobInputData.Vehicle);


		Assert.AreEqual(1, jobContainer.Runs.Count);

		var modData = ((ModalDataContainer)((VehicleContainer)run.GetContainer()).ModData).Data;
		jobContainer.Execute(false);
		WaitAndAssertSuccess(jobContainer, fileWriter);


		var soc = modData.Rows[0].Field<Scalar>(ModalResultField.REESSStateOfCharge.GetName()); //init soc
		foreach (DataRow modDataRow in modData.Rows) {
			Assert.AreEqual(soc, modDataRow.Field<Scalar>(ModalResultField.REESSStateOfCharge.GetName()));
			//Assert.IsFalse(modDataRow.Field<bool>(ModalResultField.ICEOn.GetName()));
			if (!run.GetContainer().RunData.JobType
					.IsOneOf(VectoSimulationJobType.IEPC_S, VectoSimulationJobType.IEPC_E)) {
				AssertSHEV_PEV_Conditioning(modDataRow, run);
			}
		}

		AssertVSUMElectricMotorFields(fileWriter.SumFileName, runs.First().GetContainer().RunData);
		Assert.IsTrue(modData.Rows.Count > 0);
		//foreach (var vectoRun in runs.Where(r => r != run))
		//{
		//	var rd = vectoRun.GetContainer().RunData;
		//	rd.Report.AddResult(rd, modData);
		//}

		Assert.IsTrue(modData.Rows.Count > 0);
		CheckPEVHVACInModData(run.GetContainer().RunData, modData);

	}

	[TestCase(@"HeavyLorry/PEV/Group5_ PEV_IEPC_E-EffCorrection.xml", 413.75, -1396.825, -61756.61)]
	[TestCase(@"HeavyLorry/PEV/Group5_ PEV_IEPC_E-EffCorrection.xml", 827.50, 161.085, 13679.79)]
	[Category(Definitions.TESTCASE_MIGRATED)]
    public void TestIEPC_EfficiencyCorrection(string jobFile, double rpm, double tq, double expectedPel)
	{
		var jobContainer = GetJobContainer(jobFile, null, out var fileWriter, out var runs, out var sumDataContainer,
			out var inputProvider);
		var run = runs.First().GetContainer().RunData;

		var iepc = run.ElectricMachinesData.First().Item2 as IEPCElectricMotorData;
		Assert.IsNotNull(iepc);
		var voltageLevel = iepc.EfficiencyData.VoltageLevels.First() as IEPCVoltageLevelData;
		Assert.IsNotNull(voltageLevel);
		var map = voltageLevel.EfficiencyMaps[1];

		var entry = map.Entries.First(x => x.Torque.IsEqual(tq, 0.1) && x.MotorSpeed.AsRPM.IsEqual(rpm, 0.1));

		var input = run.InputData.JobInputData.Vehicle.Components.IEPC
			.VoltageLevels.First().PowerMap.First();
		var ratio = run.InputData.JobInputData.Vehicle.Components.IEPC.Gears.First().Ratio;

        var inputRow = input.PowerMap.AsEnumerable().First(r => r.ParseDouble(IEPCMapReader.Fields.MotorSpeed).IsEqual(rpm / ratio, 0.1) &&
																r.ParseDouble(IEPCMapReader.Fields.Torque).IsEqual(-tq * ratio, 0.1));
		var inputPwrEl = inputRow.ParseDouble(IEPCMapReader.Fields.PowerElectrical);

		// check that in the input the efficiency is greater than 1
		Assert.IsTrue(-inputPwrEl > expectedPel);
		if (tq < 0) {
			// propulsion
			Assert.IsTrue(rpm.RPMtoRad() * -tq.SI<NewtonMeter>() / inputPwrEl > 1);
		} else {
			// recuperation
			Assert.IsTrue(inputPwrEl / (rpm.RPMtoRad() * -tq.SI<NewtonMeter>()) > 1);
		}

        // calculate electric power as the raw map contains a virtual 'torque loss' of the EM.
        var elPower = entry.MotorSpeed * entry.Torque + entry.PowerElectrical.Value().SI<NewtonMeter>() * entry.MotorSpeed;

		// < 0 means propulsion, hence the electric power needs to be 'more negative'
		Assert.IsTrue(entry.MotorSpeed * entry.Torque > elPower);
		
        Assert.AreEqual(expectedPel, elPower.Value(), 0.1);
	}

	[TestCase(@"HeavyLorry/P-HEV/Group5_HEV_P2_EM-EffCorrection.xml", 25, -1050, -2804.993)]
	[TestCase(@"HeavyLorry/P-HEV/Group5_HEV_P2_EM-EffCorrection.xml", 255, 1050, 27477.94)]
	[Category(Definitions.TESTCASE_MIGRATED)]
	public void TestEM_EfficiencyCorrection(string jobFile, double rpm, double tq, double expectedPel)
	{
		var jobContainer = GetJobContainer(jobFile, null, out var fileWriter, out var runs, out var sumDataContainer,
			out var inputProvider);
		var run = runs.First().GetContainer().RunData;

		var em = run.ElectricMachinesData.First().Item2;
		Assert.IsNotNull(em);
		var voltageLevel = em.EfficiencyData.VoltageLevels.First();
		Assert.IsNotNull(voltageLevel);
		var map = voltageLevel.EfficiencyMap;

		var entry = map.Entries.First(x => x.Torque.IsEqual(tq, 0.1) && x.MotorSpeed.AsRPM.IsEqual(rpm, 0.1));

		var input = run.InputData.JobInputData.Vehicle.Components.ElectricMachines.Entries.First().ElectricMachine
			.VoltageLevels.First().PowerMap.First();
		var inputRow = input.PowerMap.AsEnumerable().First(r => r.ParseDouble(ElectricMotorMapReader.Fields.MotorSpeed).IsEqual(rpm, 0.1) &&
																r.ParseDouble(ElectricMotorMapReader.Fields.Torque).IsEqual(-tq, 0.1));
		var inputPwrEl = inputRow.ParseDouble(ElectricMotorMapReader.Fields.PowerElectrical);

		// check that in the input the efficiency is greater than 1
		Assert.IsTrue(-inputPwrEl > expectedPel);
		if (tq < 0) {
			// propulsion
			Assert.IsTrue(rpm.RPMtoRad() * -tq.SI<NewtonMeter>() / inputPwrEl > 1);
		} else {
			// recuperation
			Assert.IsTrue(inputPwrEl / (rpm.RPMtoRad() * -tq.SI<NewtonMeter>())  > 1);
		}

        // calculate electric power as the raw map contains a virtual 'torque loss' of the EM.
        var elPower = entry.MotorSpeed * entry.Torque + entry.PowerElectrical.Value().SI<NewtonMeter>() * entry.MotorSpeed;

		// < 0 means propulsion, hence the electric power needs to be 'more negative'
		Assert.IsTrue(entry.MotorSpeed * entry.Torque > elPower);
		
		Assert.AreEqual(expectedPel, elPower.Value(), 0.1);
	}

    private void CheckPEVHVACInRunData(VectoRunData rd, IVehicleDeclarationInputData vehicle)
	{
		var hvacInput = vehicle.Components.AuxiliaryInputData.Auxiliaries
			.First(a => a.Type == AuxiliaryType.HVAC);
		var ignoredTechnology = "None";
		var technologies = DeclarationData.HeatingVentilationAirConditioning.GetTechnologies();
		Assert.That(technologies.Contains(ignoredTechnology));

		if (hvacInput.Technology.Contains(ignoredTechnology)) {
			//assert no hvac power 
			Assert.Fail("Create testfiles");
		} else {
			//assert hvac power
			var hvacAux = rd.Aux.First(a => a.ID == Constants.Auxiliaries.IDs.HeatingVentilationAirCondition);
			Assert.IsTrue(hvacAux.ConnectToREESS);
			Assert.AreNotEqual(0.SI<Watt>(), hvacAux.PowerDemandElectric);
		}
	}

	private void AssertPevAUXConnectToREESS(VectoRunData rd)
	{
		foreach (var aux in rd.Aux) {
			Assert.IsTrue(aux.ConnectToREESS);
		}
	}

	private void CheckPEVHVACInModData(VectoRunData rd, ModalResults modalResults)
	{
		foreach (DataRow row in modalResults.Rows) {
			foreach (var auxData in rd.Aux) {
				switch (auxData.ID) {
					case Constants.Auxiliaries.IDs.HeatingVentilationAirCondition:
						var hvac = row[string.Format(ModalResultField.P_aux_el_.GetCaption(), auxData.ID)] as Watt;
						Assert.AreEqual(auxData.PowerDemandElectric, hvac);
						break;
					default:
						break;
				}

			}
		}


	}

	[TestCase(@"HeavyLorry/S-HEV/Group2_HEV_S2_pto_transmission.xml", @"HeavyLorry/S-HEV/Group2_HEV_S2.xml")]
	[Ignore("not ready, maybe this test doesn't make any sense")]
	public void PEVPtoTransmission(string withPTOTransmission, string withoutPTOTransmission)
	{
		var ptoJobContainer =
			GetJobContainer(withPTOTransmission, null, out var fileWriterPto, out var runsPto, out _, false);
		var jobContainer =
			GetJobContainer(withoutPTOTransmission, null, out var fileWriter, out var runs, out _, false);
		runsPto.ForEach(r => Assert.NotNull(r.GetContainer().RunData.PTO));
		runs.ForEach(r => Assert.Null(r.GetContainer().RunData.PTO));
		//var ptoPath = Path.Combine(BASE_DIR, withoutPTOTransmission);
		//var noPtoPath = Path.Combine(BASE_DIR, withoutPTOTransmission);
		//var ptoXDoc = XDocument.Load(ptoPath);
		//var noPtoxDoc = XDocument.Load(ptoPath);


		var ptoRun = runs.Single(run => {
			var rd = run.GetContainer().RunData;
			return rd.Mission.MissionType == MissionType.UrbanDelivery && rd.Loading == LoadingType.ReferenceLoad;
		});
		ptoJobContainer.AddRun(ptoRun);


		var run = runs.Single(run => {
			var rd = run.GetContainer().RunData;
			return rd.Mission.MissionType == MissionType.UrbanDelivery && rd.Loading == LoadingType.ReferenceLoad;
		});
		jobContainer.AddRun(run);
		var ptoMod =
			VectoCSVFile.Read(fileWriterPto.GetModDataFileName(ptoRun.RunName, ptoRun.CycleName, ptoRun.RunSuffix));
		var mod = VectoCSVFile.Read(fileWriter.GetModDataFileName(run.RunName, run.CycleName, run.RunSuffix));




		Assert.Inconclusive();
	}



	[TestCase(@"HeavyLorry/PEV/PEV_heavyLorry_E3_supercap_invalid.xml", TestName = "SuperCap used in PEV")]
	[TestCase(@"HeavyLorry/P-HEV/Group5_HEV_P2_ovc_supercap_invalid.xml", TestName = "SuperCap used in OVC P-HEV")]
	[TestCase(@"HeavyLorry/S-HEV/Group2_HEV_S2_ovc_supercap_invalid.xml", TestName = "SuperCap used in OVC S-HEV")]
	public void SuperCapFailTest(string jobFile)
	{
		var ex = Assert.Throws<VectoException>(() => {
			GetJobContainer(jobFile, null, out var fileWriter, out var runs, out var sumDataContainer,
				true);
		});

		Assert.IsTrue(ex!.Message.Contains("Super cap"));
	}

	//public void EPTOUpdateFromTest()
	//{
	//	var originalPTOCycleController = new PTOCycleController()
	//}


	[TestCase(@"HeavyLorry/PEV/PEV_heavyLorry_E4_pto.xml", 8)]
	[TestCase(@"HeavyLorry/PEV/Group5_ PEV_IEPC_E_pto.xml", 8)]
	[TestCase(@"HeavyLorry/S-HEV/Group2_HEV_S3_pto.xml", 8)]
	[TestCase(@"HeavyLorry/S-HEV/Group2_HEV_IEPC_S_pto.xml", 8)]
	[TestCase(@"HeavyLorry/S-HEV/Group2_HEV_S2_supercap_epto.xml", 8)]
	public void EPTO(string jobFile, int nrRuns)
	{
		//"HeavyLorry/S-HEV/Group2_HEV_S2_supercap_epto.xml"
		///Charge is 0 in TestPowertrain but 45,9 kW in real powertrain
		/// SM state of serial hybrid strategy in breaking phase was Break_S1 -> selects Accelerate S1 as new accelerate phase -> GEN is on but supercap is (almost full) 

		var jobContainer = GetJobContainer(jobFile, nrRuns, out var fileWriter, out var runs, out var sumDataContainer,
			true);
		Assert.IsTrue(runs.Any(run => run.GetContainer().RunData.Mission.MissionType == MissionType.MunicipalUtility));
		var run = runs.First(run => run.GetContainer().RunData.Mission.MissionType == MissionType.MunicipalUtility);
		jobContainer.AddRun(run);

		var modData = ((ModalDataContainer)((VehicleContainer)run.GetContainer()).ModData).Data;
		jobContainer.Execute();

		var ptoCol = "P_aux_PTO_CONSUM_el [kW]";
		var speedCol = "v_act";
		var timeCol = "time";

		WaitAndAssertSuccess(jobContainer, fileWriter);
		var soc = modData.Rows[0].Field<Scalar>(ModalResultField.REESSStateOfCharge.GetName()); //init soc
		var ptoState = PTOState.VehicleDriving;


		var eptoCSV =
			VectoCSVFile.ReadStream(RessourceHelper.ReadStream(DeclarationData.PTO.DefaultE_PTOActivationCycle));
		var eptoCurve = new LinearCurve();
		foreach (DataRow row in eptoCSV.Rows) {
			eptoCurve.AddPoint(row.ParseDouble(0), row.ParseDouble(1) * 1000);
		}

		double startTime = 0;
		for (var i = 0; i < modData.Rows.Count - 1; i++) {
			switch (ptoState) {
				case PTOState.VehicleDriving:
					if (modData.Rows[i].Field<MeterPerSecond>(speedCol).Value() == 0) {
						ptoState = PTOState.VehicleStopped;
					}

					Assert.AreEqual(0, modData.Rows[i].Field<Watt>(ptoCol).Value());
					break;
				case PTOState.VehicleStopped:
					if (modData.Rows[i].Field<MeterPerSecond>(speedCol).Value() > 0) {
						ptoState = PTOState.VehicleDriving;
						continue;
					}

					if (modData.Rows[i].Field<Watt>(ptoCol).Value() > 0) {
						ptoState = PTOState.PTOActive;
						startTime = modData.Rows[i].Field<Second>(timeCol).Value();
						Assert.AreEqual(eptoCurve.Lookup(0), modData.Rows[i].Field<Watt>(ptoCol).Value(), 1E-6);
					}

					break;
				case PTOState.PTOActive:


					if (modData.Rows[i].Field<Watt>(ptoCol).Value() == 0) {
						ptoState = PTOState.VehicleStopped;
						continue;
					}

					Assert.AreEqual(eptoCurve.Lookup(modData.Rows[i].Field<Second>(timeCol).Value() - startTime),
						modData.Rows[i].Field<Watt>(ptoCol).Value(), 1E-6);

					break;
				default:
					throw new ArgumentOutOfRangeException();
			}




		}
	}

	[Test, TestCaseSource(nameof(GetJsonJobs))]
	public void JSONDeclarationSmokeTest(string path)
	{
		RunJsonJob(path, ExecutionMode.Declaration);
	}

	private void RunJsonJob(string path, ExecutionMode executionMode)
	{
		var writeReports = true;
		var inputData = JSONInputDataFactory.ReadJsonJob(path, false);
		if (inputData is IDeclarationInputDataProvider decl) {
			if (decl.JobInputData.Vehicle.VehicleCategory.IsBus()) {
				Assert.Ignore("Bus");
			}

		}

		var fileWriter = new FileOutputWriter(path);
		var runsFactory = SimulatorFactory.CreateSimulatorFactory(executionMode, inputData, fileWriter,
			writeReports ? null : new NullDeclarationReport()); //, writeReports ? null : new NullDeclarationReport());
		DisableIterativeRuns(runsFactory);
		runsFactory.WriteModalResults = true;
		var sumWriter = new SummaryDataContainer(fileWriter); //new MockSumWriter();

		var jobContainer = new JobContainer(sumWriter);
		runsFactory.SumData = sumWriter;
		//var sumDataContainer = sumWriter;
		var runs = runsFactory.SimulationRuns();
		var simulatedRun =
			runs.FirstOrDefault(r => r.GetContainer().RunData.Mission?.MissionType == MissionType.RegionalDelivery,
				runs.First());


		jobContainer.AddRun(simulatedRun);
		if (writeReports) {
			SetResultCountInReport(1, simulatedRun.GetContainer().RunData.Report);
		}

		jobContainer.Execute(true);
		WaitAndAssertSuccess(jobContainer, fileWriter);
	}


	private void SetResultCountInReport(int count, IDeclarationReport report)
	{
		if (report == null) {
			return; //also used in engineering mode
		}

		if (report is XMLDeclarationReport rep09) {


			GetField("_resultCount", rep09.GetType()).SetValue(rep09, count);
			return;
		}

		Assert.Fail("Reflection failed");

	}

	private int GetResultCount(IDeclarationReport report)
	{

		if (report is XMLDeclarationReport rep09) {


			return (int)GetField("_resultCount", rep09.GetType()).GetValue(rep09);

		}

		return int.MinValue;
	}

	private FieldInfo GetField(string name, Type type)
	{
		bool found = false;
		while (!found) {
			FieldInfo[] fields = type.GetFields(
				BindingFlags.NonPublic |
				BindingFlags.Instance);
			var field = fields.FirstOrDefault(f => f.Name == name);
			if (field == null) {
				type = type.BaseType;
				if (type == null) {
					Assert.Fail("Field not found");
				}
			} else {
				return field;
			}
		}

		return null;

	}




	public void DisableIterativeRuns(ISimulatorFactory factory)
	{
		factory.ModifyRunData = (rd) => rd.IterativeRunStrategy.Enabled = false;
	}

	[Test, TestCaseSource(nameof(GetJsonJobs))]
	[Ignore("Just for comparison")]
	public void JSONEngineering(string path)
	{
		RunJsonJob(path, ExecutionMode.Engineering);
	}

	public static string[] GetJsonJobs()
	{
		return GetFiles("JSON", "*.vecto").ToArray();
	}

	public static string[] GetHeavyLorryJobs()
	{
		return GetFiles("HeavyLorry", "*.xml").ToArray();
	}

	public static string[] GetMediumLorryJobs()
	{
		return GetFiles("MediumLorry", "*.xml").ToArray();
	}

	private static List<string> GetFiles(string path, string searchPattern)
	{
		var dirPath = Path.Combine(BASE_DIR, path);
		List<string> vectoJobs = new List<string>();
		foreach (var fileName in Directory.EnumerateFiles(dirPath, searchPattern, SearchOption.AllDirectories)) {
			vectoJobs.Add(fileName);
		}

		return vectoJobs;
	}


	[Test, TestCaseSource(nameof(GetHeavyLorryJobs)),
	Description("Just a little helper to check that all files are heavy lorries")]
	public void FilesAreHeavyLorry(string file)
	{
		IDeclarationInputDataProvider dataProvider = null;
		try {
			dataProvider = _xmlReader.CreateDeclaration(file);

		} catch (Exception ex) {
			if (ex.Message.Contains("unknown")) {
				Assert.Ignore(ex.Message);
			} else {
				Assert.Fail(ex.Message);
			}

		}

		Assert.That(dataProvider.JobInputData.Vehicle.VehicleCategory.IsLorry());
		var segment = GetSegment(dataProvider.JobInputData.Vehicle);
		Assert.That(segment.VehicleClass.IsHeavyLorry());
	}

	[Test, TestCaseSource(nameof(GetMediumLorryJobs)),
	Description("Just a little helper to check that all files are medium lorries")]
	public void FilesAreMediumLorry(string file)
	{
		IDeclarationInputDataProvider dataProvider = null;
		try {
			dataProvider = _xmlReader.CreateDeclaration(file);

		} catch (Exception ex) {
			if (ex.Message.Contains("unknown")) {
				Assert.Ignore(ex.Message);
			} else {
				Assert.Fail(ex.Message);
			}

		}

		Assert.That(dataProvider.JobInputData.Vehicle.VehicleCategory.IsLorry());
		var segment = GetSegment(dataProvider.JobInputData.Vehicle);
		Assert.That(segment.VehicleClass.IsMediumLorry());
	}

	protected Segment GetSegment(IVehicleDeclarationInputData vehicle)
	{
		var batteryElectric = vehicle.VehicleType.IsOneOf(VectoSimulationJobType.BatteryElectricVehicle,
			VectoSimulationJobType.IEPC_E);
		var ng = vehicle.Components?.EngineInputData?.EngineModes.Any(e =>
			e.Fuels.Any(f => f.FuelType.IsOneOf(FuelType.LPGPI, FuelType.NGCI, FuelType.NGPI))) ?? false;
		var ovcHev = vehicle.OVC;
		Segment segment;
		try {
			segment = DeclarationData.TruckSegments.Lookup(
				vehicle.VehicleCategory, batteryElectric, vehicle.AxleConfiguration, vehicle.GrossVehicleMassRating,
				vehicle.CurbMassChassis,
				vehicle.VocationalVehicle, ng, ovcHev);
		} catch (VectoException) {
			segment = DeclarationData.TruckSegments.Lookup(
				vehicle.VehicleCategory, batteryElectric, vehicle.AxleConfiguration, vehicle.GrossVehicleMassRating,
				vehicle.CurbMassChassis,
				false, ng, ovcHev);
		}

		if (!segment.Found) {
			throw new VectoException(
				"no segment found for vehicle configuration: vehicle category: {0}, axle configuration: {1}, GVMR: {2}",
				vehicle.VehicleCategory, vehicle.AxleConfiguration,
				vehicle.GrossVehicleMassRating);
		}

		return segment;
	}

	private void WaitAndAssertSuccess(JobContainer jobContainer, FileOutputWriter fileWriter)
	{
		jobContainer.WaitFinished();
		Assert.IsTrue(jobContainer.AllCompleted);
		Assert.IsTrue(jobContainer.Runs.TrueForAll(runEntry => runEntry.Success));
		PrintRuns(jobContainer, fileWriter);
		PrintFiles(fileWriter);
	}


	private JobContainer GetJobContainer(string jobFile, int? nrRuns, out FileOutputWriter fileWriter,
		out List<IVectoRun> runs, out SummaryDataContainer sumDataContainer, bool writeReports = true)
	{
		return GetJobContainer(jobFile, nrRuns, out fileWriter, out runs, out sumDataContainer, out _, writeReports);
	}

	[MethodImpl(MethodImplOptions.Synchronized)]
	private JobContainer GetJobContainer(string jobFile, int? nrRuns, out FileOutputWriter fileWriter,
		out List<IVectoRun> runs, out SummaryDataContainer sumDataContainer,
		out IDeclarationInputDataProvider inputData,
		bool writeReports = true)
	{
		var filePath = Path.Combine(BASE_DIR, jobFile);
		Assert.IsTrue(File.Exists(filePath), "Testfile not found: " + filePath);
		var dataProvider = _xmlReader.CreateDeclaration(filePath);
		inputData = dataProvider;
		fileWriter = new FileOutputWriter(filePath);
		var runsFactory = SimulatorFactory.CreateSimulatorFactory(ExecutionMode.Declaration, dataProvider, fileWriter,
			writeReports ? null : new NullDeclarationReport());
		//runsFactory.ActualModalData = true;
		runsFactory.SerializeVectoRunData = true;
		runsFactory.WriteModalResults = true;
		var sumWriter = new SummaryDataContainer(fileWriter);

		var jobContainer = new JobContainer(sumWriter);
		runsFactory.SumData = sumWriter;
		sumDataContainer = sumWriter;
		runs = runsFactory.SimulationRuns().ToList();

		if (nrRuns.HasValue) {
			Assert.AreEqual(nrRuns, runs.Count,
				"Cycles: \n" + string.Join("\n", runs.Select(run => run.CycleName + "_" + run.RunSuffix)));
		}

		TestContext.WriteLine(string.Join("\n", runs.Select(r => r.CycleName + "_" + r.RunSuffix)));

		if (dataProvider.JobInputData.Vehicle.OVC) {
			Assert.AreEqual(runs.Count(r => r.GetContainer().RunData.OVCMode == OvcHevMode.ChargeDepleting),
				runs.Count(r => r.GetContainer().RunData.OVCMode == OvcHevMode.ChargeSustaining));
		}

		return jobContainer;
	}






	private void PrintRuns(JobContainer jobContainer, FileOutputWriter fileWriter = null)
	{
		foreach (var keyValuePair in jobContainer.GetProgress()) {
			TestContext.WriteLine(
				$"{keyValuePair.Key}: {keyValuePair.Value.CycleName} {keyValuePair.Value.RunName} {keyValuePair.Value.Error?.Message}");
			//if (fileWriter != null && keyValuePair.Value.Success) {
			//	TestContext.AddTestAttachment(fileWriter.GetModDataFileName(keyValuePair.Value.RunName, keyValuePair.Value.CycleName, keyValuePair.Value.RunSuffix), keyValuePair.Value.RunName);
			//         }

		}
	}

	private void PrintFiles(FileOutputWriter fileWriter)
	{
		//if (fileWriter.GetWrittenFiles().Count == 0) {
		//	Assert.Fail("No files written\n");
		//}
		foreach (var keyValuePair in fileWriter.GetWrittenFiles()) {
			TestContext.WriteLine($"{keyValuePair.Key} written to {keyValuePair.Value}");
			TestContext.AddTestAttachment(keyValuePair.Value, keyValuePair.Key.ToString());
		}
	}

	//runs.First().GetContainer().PowertrainInfo.ElectricMotorPositions;
	public void AssertSHEV_PEV_Conditioning(DataRow modDataRow, IVectoRun run)
	{
		var electricMotorPositions = run.GetContainer().PowertrainInfo.ElectricMotorPositions;
		var position = electricMotorPositions.Single(e => e != PowertrainPosition.GEN);

		var hasGen = electricMotorPositions.Any(e => e == PowertrainPosition.GEN);

		var idx = modDataRow.Table.Rows.IndexOf(modDataRow);
		if (idx - 1 < 0) {
			return;
		}

		var prevRow = modDataRow.Table.Rows[idx - 1];

		var condData = run.GetContainer().RunData.Aux.Single(aux => aux.ID == Constants.Auxiliaries.IDs.Cond);
		Assert.IsTrue(condData.IsFullyElectric);
		Assert.IsTrue(condData.ConnectToREESS);
		var time = modDataRow.Field<Second>(ModalResultField.time.GetName());
		//modDataRow.Table.Rows[]
		if (EMOn(prevRow, position) || (hasGen && EMOn(prevRow, PowertrainPosition.GEN))) {
			var cond = DeclarationData.Conditioning.LookupPowerDemand(
				run.GetContainer().RunData.VehicleData.VehicleClass,
				run.GetContainer().RunData.JobType,
				run.GetContainer().RunData.Mission.MissionType);
			var condMod = modDataRow.Field<Watt>("P_aux_COND_el [kW]");
			Assert.IsTrue(cond.IsEqual(condMod), $"expected {cond} got {condMod} at {time}");
		} else {
			var condMod = modDataRow.Field<Watt>("P_aux_COND_el [kW]");
			Assert.IsTrue(0.SI<Watt>().IsEqual(condMod), $"expected {0} got {condMod} at {time}");
		}
	}

	private static bool EMOn(DataRow prevRow, PowertrainPosition position)
	{
		if (position == PowertrainPosition.IEPC) {
			return prevRow.Field<Scalar>(string.Format(ModalResultField.IEPC_Off_.GetCaption(), position.GetLabel())) ==
					0.SI<Scalar>();
		}

		return prevRow.Field<Scalar>(string.Format(ModalResultField.EM_Off_.GetCaption(), position.GetLabel())) ==
				0.SI<Scalar>();
	}


	//private static bool EPTOTestPowertrain()
	//{
	//	IVehicleContainer powertrain = null;// = PowertrainBuilder.Build();
	//	var testPowertrain = PowertrainBuilder.BuildSimpleSerialHybridPowertrain();

	//	VectoRunData runData = null;
	//	SimplePowertrainContainer testContainer = null; new SimplePowertrainContainer(runData);
	//	if (runData.JobType == VectoSimulationJobType.IEPC_S)
	//	{
	//		PowertrainBuilder.BuildSimpleIEPCHybridPowertrain(runData, testContainer);
	//	}
	//	else
	//	{
	//		PowertrainBuilder.BuildSimpleSerialHybridPowertrain(runData, testContainer);
	//	}

	//	var response = testContainer.GetCycleOutPort().Request(0.SI<Second>(), 2.SI<Meter>());
	//	response.ElectricSystem.AuxPower


	//}	[TestCase(@"HeavyLorry/P-HEV/Group5_HEV_IHPC.xml", 20)]

	[TestCase(@"HeavyLorry/P-HEV/Group5_HEV_IHPC.xml",
		MissionType.UrbanDelivery,
		LoadingType.ReferenceLoad,
		8503,
		0.01,
		TestName = "Group5_HEV_IHPC.xml, Urbandelivery selected section")]

	public void HeavyLorryCycleSection(string jobFile, MissionType cycle, LoadingType loading,
		double startDistance_m, double f_equiv)
	{


		var missionFilter = TestMissionFilter();
		missionFilter.SetMissions((cycle, loading));

		var cycleFactory = StartPointCycleFactory();
		cycleFactory.SetStartPoint(startDistance_m.SI<Meter>());
		RunSimulation(jobFile, data => data.UpdateFequiv(f_equiv));
	}

	public void RunSimulation(string jobFile, params Action<VectoRunData>[] runDataModifier)
	{
		var filePath = Path.Combine(BASE_DIR, jobFile);
		TestContext.Progress.WriteLine(filePath);
		var dataProvider = _xmlReader.CreateDeclaration(filePath);
		var fileWriter = new FileOutputWriter(filePath);
		var simFactory = Kernel.Get<ISimulatorFactoryFactory>();
		var runsFactory = simFactory.Factory(ExecutionMode.Declaration, dataProvider, fileWriter, null, null);
		runsFactory.WriteModalResults = true;
		//runsFactory.SerializeVectoRunData = true;
		var jobContainer = new JobContainer(new SummaryDataContainer(fileWriter)) { };
		//var jobContainer = new JobContainer(new MockSumWriter()) { };
		var runs = runsFactory.SimulationRuns().ToList();
		foreach (var vectoRun in runs) {
			foreach (var action in runDataModifier) {
				action(vectoRun.GetContainer().RunData);
			}
		}

		foreach (var run in runs) {
			jobContainer.AddRun(run);
		}

		PrintRuns(jobContainer, null);

		jobContainer.Execute();
		jobContainer.WaitFinished();
		PrintRuns(jobContainer, fileWriter);
		PrintFiles(fileWriter);
        Assert.IsTrue(jobContainer.AllCompleted);
		Assert.IsTrue(jobContainer.Runs.TrueForAll(runEntry => runEntry.Success));


        return;
	}
}