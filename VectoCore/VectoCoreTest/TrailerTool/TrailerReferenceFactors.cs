using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using Ninject;
using NUnit.Framework;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCore.InputData.FileIO.XML;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.OutputData.FileIO;
using TUGraz.VectoCore.Tests.Helper.TrailerTool;

namespace TUGraz.VectoCore.Tests.TrailerTool
{

	public class ResultManager
	{

		public class SimulationResult
		{
			public int group;
			public string group_annex1;
			public string towingGroup;
			public TrailerReferenceRatios CO2Ref;
			public TrailerReferenceRatios CO2Standard;
			public TrailerReferenceRatios RefFactor;
		}


		public class StandardResult
		{
			public string XmlCode;
			public TrailerReferenceRatios StandardResults;
		}

		public class ReferenceResult
		{
			public string group_annex1;
			public TrailerReferenceRatios CO2Ref;
		}

		private Dictionary<int, SimulationResult> _vehicleGroupResults = new Dictionary<int, SimulationResult>();
		private Dictionary<string, ReferenceResult> _referenceResults = new Dictionary<string, ReferenceResult>();
		private Dictionary<string, StandardResult> _standardResults = new Dictionary<string, StandardResult>();
		public ResultManager(IList<int> groupsToSimulate)
		{
			foreach (var i in groupsToSimulate) {
				_vehicleGroupResults.Add(i, new SimulationResult() {
					@group = i,
					
				});
			}
			SetTowingVehicleAndAnnex1Groups(new TrailerSegments());
		}

		private void SetTowingVehicleAndAnnex1Groups(TrailerSegments segments)
		{
			foreach (var group in _vehicleGroupResults) {
				var row = segments.GetRow(group.Key.ToString());
				group.Value.towingGroup = row["Generic_vehicle_group"].ToString();
				group.Value.group_annex1 = row.Field<string>(CSVNames.TrailerReferenceFactors_Vehicle_group_annex1);
			}
		}



		public SimulationResult GetSimulationResult(int group)
		{
			return _vehicleGroupResults[group];
		}

		public void AddReferenceResult(ReferenceResult referenceResult, int vehicleGroup)
		{
			var groupAnnex = _vehicleGroupResults[vehicleGroup].group_annex1;
			foreach (var simulationResult in _vehicleGroupResults.Values.Where(simRes => simRes.group_annex1 == groupAnnex)) {
				simulationResult.CO2Ref = referenceResult.CO2Ref;
			}

			if (!_referenceResults.ContainsKey(groupAnnex)) {
				_referenceResults.Add(groupAnnex, referenceResult);
			}
		}

		public bool GetReferenceResult(int group, out ReferenceResult referenceResult)
		{
			return _referenceResults.TryGetValue(_vehicleGroupResults[group].group_annex1, out referenceResult);
		}

		public void AddStandardSimulationResult(string xmlCode, StandardResult standardResult)
		{
			_standardResults[xmlCode] = standardResult;
		}

		public bool GetStandardResult(string xmlCode, out StandardResult standardResult)
		{
			return _standardResults.TryGetValue(xmlCode, out standardResult);
		}

		public IList<SimulationResult> GetSimulationsResultsByAnnexGroup()
		{
			return _vehicleGroupResults.Values.GroupBy(res => res.group_annex1).Select(group => group.First()).ToList();
		}
	}
	public class TrailerReferenceFactors
	{
		protected IXMLInputDataReader XMLInputReader;
		private IKernel _kernel;

		private const string SampleTrailer = @"TestData\Trailer\trailerSample.xml";
		private int _vsumCO2ColumnIndex;
		private static string _vehicleGroup = "Vehicle_Group";
		private string _co2StandardLonghaulGKm = "CO2 Standard LongHaul [g/km]";
		private string _co2StandardRegionalDeliveryGKm = "CO2 Standard Regional Delivery [g/km]";
		private string _co2StandardUrbanDeliveryGKm = "CO2 Standard Urban Delivery [g/km]";
		private string _co2ReferenceLonghaulGKm = "CO2 Reference LongHaul [g/km]";
		private string _co2ReferenceRegionalDeliveryGKm = "CO2 Reference Regional Delivery [g/km]";
		private string _co2ReferenceUrbanDeliveryGKm = "CO2 Reference Urban Delivery [g/km]";
		private string _refFactorLonghaul = "Ref-Factor_LongHaul";
		private string _refFactorRegionalDelivery = "Ref-Factor_Regional_Delivery";
		private string _refFactorUrbanDelivery = "Ref-Factor_Urban_Delivery";
		private bool _writeModalResults = false;
		private bool _actualModalData = false;
		private bool _serializeVectoRunData = false;

		[SetUp]
		public void RunBeforeAnyTests()
		{
			Directory.SetCurrentDirectory(TestContext.CurrentContext.TestDirectory);
			Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture; //fixes double parsing
			_kernel = new StandardKernel(new VectoNinjectModule());
			XMLInputReader = _kernel.Get<IXMLInputDataReader>();
		}

		private void SimulateReferenceTrailers(ResultManager groupManager, IList<int> vehicleGroups)
		{
			
			foreach(var group in vehicleGroups) {
				ResultManager.ReferenceResult refResult;
				if (groupManager.GetReferenceResult(group, out refResult)) {
					groupManager.AddReferenceResult(refResult, group);
					continue;
				} else {
					refResult = new ResultManager.ReferenceResult();
				}
				//var simResult = groupManager.GetSimulationResult(group); //holds the result of the simulation
				

				try {
					var xDoc = XDocumentCreator.CreateXDocument(group);

					var fileWriter = new FileOutputWriter(SampleTrailer);

					var sumWriter = new SummaryDataContainer(fileWriter);
					var jobContainer = new JobContainer(sumWriter);

					var dataProvider = XMLInputReader.CreateDeclarationTrailer(xDoc.CreateReader());
					var runsFactory = new SimulatorFactory(ExecutionMode.Declaration, dataProvider, fileWriter) {
						ModalResults1Hz = false,
						WriteModalResults = _writeModalResults,
						ActualModalData = _actualModalData,
						SerializeVectoRunData = _serializeVectoRunData,
						Validate = false,
					};
					

					jobContainer.AddRuns(runsFactory, (vectoRunData) => vectoRunData.ModFileSuffix.Contains("_referenceTrailer"));
					
					jobContainer.Execute(true); //I guess the synchronization fix is not implemented in the trailerTool
					jobContainer.WaitFinished();

					var vsum = fileWriter.SumFileName;
					var dt = GetDataTableFromCsv(vsum);

					_vsumCO2ColumnIndex = 118;
				
					refResult.CO2Ref.LongHaulLow = parseInvariant(dt.Rows[0][_vsumCO2ColumnIndex]);
					refResult.CO2Ref.LongHaulRef = parseInvariant(dt.Rows[1][_vsumCO2ColumnIndex]);

					refResult.CO2Ref.RegionalDeliveryLow = parseInvariant(dt.Rows[2][_vsumCO2ColumnIndex]);

					refResult.CO2Ref.RegionalDeliveryRef = parseInvariant(dt.Rows[3][_vsumCO2ColumnIndex]);

					if (dt.Rows.Count > 4) {
						refResult.CO2Ref.UrbanDeliveryLow = parseInvariant(dt.Rows[4][_vsumCO2ColumnIndex]);
						refResult.CO2Ref.UrbanDeliveryRef = parseInvariant(dt.Rows[5][_vsumCO2ColumnIndex]);
					}

					groupManager.AddReferenceResult(refResult, group);
				} catch (Exception ex) {
					TestContext.WriteLine(ex.Message);
					TestContext.WriteLine($"Error in group{group}");
					ToCSV(CreateDataTable(groupManager, vehicleGroups), "crashResultRefFactor.csv");
					throw ex;
				}
			}
		}

		public double parseInvariant(object stringFromCsv)
		{
			return double.Parse(stringFromCsv.ToString(), NumberFormatInfo.InvariantInfo);
		}


		private static void ToCSV(DataTable dtDataTable, string strFilePath)
		{
			var sw = new StreamWriter(strFilePath, false, new System.Text.UTF8Encoding(true));
			//headers    
			for (var i = 0; i < dtDataTable.Columns.Count; i++)
			{
				sw.Write(dtDataTable.Columns[i]);
				if (i < dtDataTable.Columns.Count - 1)
				{
					sw.Write(",");
				}
			}
			sw.Write(sw.NewLine);
			foreach (DataRow dr in dtDataTable.Rows)
			{
				for (var i = 0; i < dtDataTable.Columns.Count; i++)
				{
					if (!Convert.IsDBNull(dr[i]))
					{
						var value = dr[i].ToString();
						if (value.Contains(','))
						{
							value = String.Format("\"{0}\"", value);
							sw.Write(value);
						}
						else
						{
							sw.Write(dr[i].ToString());
						}
					}
					if (i < dtDataTable.Columns.Count - 1)
					{
						sw.Write(",");
					}
				}
				sw.Write(sw.NewLine);
			}
			sw.Close();
		}

		private static DataTable GetDataTableFromCsv(string path)
		{
			var dtCsv = new DataTable();

			using (var sr = new StreamReader(path))
			{
				while (!sr.EndOfStream)
				{
					var fulltext = sr.ReadToEnd().ToString();
					var rows = fulltext.Split('\n'); 
					for (var i = 1; i < rows.Count() - 2; i++)
					{
						var rowValues = rows[i].Split(',');  
						{
							if (i == 1)
							{
								for (var j = 0; j < rowValues.Count()-1; j++)
								{
									dtCsv.Columns.Add(rowValues[j]);
								}
							}
							else
							{
								var dr = dtCsv.NewRow();
								for (var k = 0; k < 120; k++)
								{
									dr[k] = rowValues[k].ToString();
								}
								dtCsv.Rows.Add(dr);
							}
						}
					}
				}
			}

			return dtCsv;
		}


		[TestCase]
		
		public void CreateReferenceFactors()
		{
			var vehicleGroups = DeclarationData.TrailerSegments.GetActiveVehicleGroups();

			//vehicleGroups = new System.Collections.Generic.List<int>() { 11101,11301 };


			var groupManager = new ResultManager(vehicleGroups);

	
			SimulateReferenceTrailers(groupManager, vehicleGroups);
			ToCSV(CreateDataTable(groupManager, vehicleGroups), "referenceResults.csv");
			SimulateStandardTrailer(groupManager, vehicleGroups);
			ToCSV(CreateDataTable(groupManager, vehicleGroups), "standardAndReferenceResults.csv");
			CalculateReferenceFactors(groupManager,vehicleGroups);
			ToCSV(CreateDataTable(groupManager,vehicleGroups), "referenceFactors.csv");

		}

		[TestCase(33102, false)]
		[TestCase(23101, true)]
		[TestCase(11101, true)]
		public void SimulateTrailer(int group, bool writeAdditionalFiles)
		{
			_writeModalResults = writeAdditionalFiles;
			_actualModalData = writeAdditionalFiles;
			_serializeVectoRunData = writeAdditionalFiles;

			var vehicleGroups = new List<int>();
			vehicleGroups.Add(group);
			var groupManager = new ResultManager(vehicleGroups);

			SimulateReferenceTrailers(groupManager, vehicleGroups);
			//ToCSV(CreateDataTable(groupManager, vehicleGroups), "referenceResults.csv");
			SimulateStandardTrailer(groupManager, vehicleGroups);
			//ToCSV(CreateDataTable(groupManager, vehicleGroups), "standardAndReferenceResults.csv");
			//CalculateReferenceFactors(groupManager, vehicleGroups);
			//ToCSV(CreateDataTable(groupManager, vehicleGroups), "referenceFactors.csv");
		}


		private void CalculateReferenceFactors(ResultManager manager, IList<int> vehicleGroups)
		{
			foreach (var vehicleGroup in vehicleGroups) {
				var simResult = manager.GetSimulationResult(vehicleGroup);

				simResult.RefFactor.RegionalDeliveryLow = simResult.CO2Ref.RegionalDeliveryLow / simResult.CO2Standard.RegionalDeliveryLow;
				simResult.RefFactor.RegionalDeliveryRef = simResult.CO2Ref.RegionalDeliveryRef / simResult.CO2Standard.RegionalDeliveryRef;

				

				simResult.RefFactor.LongHaulLow = simResult.CO2Ref.LongHaulLow / simResult.CO2Standard.LongHaulLow;
				simResult.RefFactor.LongHaulRef = simResult.CO2Ref.LongHaulRef / simResult.CO2Standard.LongHaulRef;

				if (simResult.towingGroup == "9-LH") {
					
				}else{
					simResult.RefFactor.UrbanDeliveryLow = simResult.CO2Ref.UrbanDeliveryLow / simResult.CO2Standard.UrbanDeliveryLow;
					simResult.RefFactor.UrbanDeliveryRef = simResult.CO2Ref.UrbanDeliveryRef / simResult.CO2Standard.UrbanDeliveryRef;
				}
			}
		}

		private DataTable CreateDataTable(ResultManager groupManager, IList<int> vehicleGroups)
		{
			var rfDt = new DataTable();

			//rfDt.Columns.Add(_vehicleGroup);
			rfDt.Columns.Add(CSVNames.TrailerReferenceFactors_Vehicle_group_annex1);
			rfDt.Columns.Add(_co2StandardLonghaulGKm);
			rfDt.Columns.Add(_co2StandardRegionalDeliveryGKm);
			rfDt.Columns.Add(_co2StandardUrbanDeliveryGKm);
			rfDt.Columns.Add(_co2ReferenceLonghaulGKm);
			rfDt.Columns.Add(_co2ReferenceRegionalDeliveryGKm);
			rfDt.Columns.Add(_co2ReferenceUrbanDeliveryGKm);
			rfDt.Columns.Add(_refFactorLonghaul);
			rfDt.Columns.Add(_refFactorRegionalDelivery);
			rfDt.Columns.Add(_refFactorUrbanDelivery);


			foreach (var result in groupManager.GetSimulationsResultsByAnnexGroup().OrderBy(result => result.group_annex1)) {
				var row = rfDt.NewRow();
				//row[_vehicleGroup] = group;
				row[CSVNames.TrailerReferenceFactors_Vehicle_group_annex1] = result.group_annex1;
				row[_co2ReferenceLonghaulGKm] = $"{result.CO2Ref.LongHaulLow}/{result.CO2Ref.LongHaulRef}";
				row[_co2ReferenceRegionalDeliveryGKm] = $"{result.CO2Ref.RegionalDeliveryLow}/{result.CO2Ref.RegionalDeliveryRef}";
				row[_co2ReferenceUrbanDeliveryGKm] = $"{result.CO2Ref.UrbanDeliveryLow}/{result.CO2Ref.UrbanDeliveryRef}";

				row[_co2StandardLonghaulGKm] = $"{result.CO2Standard.LongHaulLow}/{result.CO2Standard.LongHaulRef}";
				row[_co2StandardRegionalDeliveryGKm] = $"{result.CO2Standard.RegionalDeliveryLow}/{result.CO2Standard.RegionalDeliveryRef}";
				if (result.towingGroup == "9-LH") {
					row[_co2StandardUrbanDeliveryGKm] = "---";
				} else {
					row[_co2StandardUrbanDeliveryGKm] = $"{result.CO2Standard.UrbanDeliveryLow}/{result.CO2Standard.UrbanDeliveryRef}";
				}
				


				row[_refFactorLonghaul] = $"{result.RefFactor.LongHaulLow}/{result.RefFactor.LongHaulRef}";
				row[_refFactorRegionalDelivery] = $"{result.RefFactor.RegionalDeliveryLow}/{result.RefFactor.RegionalDeliveryRef}";
				if(result.towingGroup == "9-LH")
                {
					row[_refFactorUrbanDelivery] = "---";
                }
                else
                {
					row[_refFactorUrbanDelivery] = $"{result.RefFactor.UrbanDeliveryLow}/{result.RefFactor.UrbanDeliveryRef}";
				}
				
				
				rfDt.Rows.Add(row);
			}
			
			return rfDt;
		}

		private void SimulateStandardTrailer(ResultManager groupManager, List<int> vehicleGroups)
		{
			HashSet<MissionType> missionTypes = new HashSet<MissionType>() {
				MissionType.RegionalDelivery,
				MissionType.UrbanDelivery,
				MissionType.LongHaul
			};

			var trailerSegments = new TrailerSegments();
			foreach (var group in vehicleGroups) {
				var row = trailerSegments.GetRow(group.ToString());
				var genericTowingVehicleCode = row["Generic_code"].ToString();
				ResultManager.StandardResult standardResult;
				if (groupManager.GetStandardResult(genericTowingVehicleCode, out standardResult)) {
					groupManager.GetSimulationResult(group).CO2Standard = standardResult.StandardResults;
					continue;
				} else {
					standardResult = new ResultManager.StandardResult() {
						XmlCode = genericTowingVehicleCode
					};
				}




				XDocumentCreator.GetTowingVehicleXml(genericTowingVehicleCode, out var path); //doestn work with xDoc

				//
				var fileWriter = new FileOutputWriter(path);

				var sumWriter = new SummaryDataContainer(fileWriter);
				var jobContainer = new JobContainer(sumWriter);

				var dataProvider = XMLInputReader.CreateDeclaration(path);
				var runsFactory = new SimulatorFactory(ExecutionMode.Declaration, dataProvider, fileWriter)
				{
					ModalResults1Hz = false,
					WriteModalResults = _writeModalResults,
					ActualModalData = _actualModalData,
					SerializeVectoRunData = _serializeVectoRunData,
					Validate = false,
				};


				jobContainer.AddRuns(runsFactory, (vectoRunData) => missionTypes.Contains(vectoRunData.Mission.MissionType));
				jobContainer.Execute(true); //I guess the synchronization fix is not implemented in the trailerTool
				jobContainer.WaitFinished();

				var vsum = fileWriter.SumFileName;
				var dt = GetDataTableFromCsv(vsum);

				_vsumCO2ColumnIndex = 118;

				ResultManager.StandardResult simResult = new ResultManager.StandardResult() {
					XmlCode = genericTowingVehicleCode
				};

				simResult.StandardResults.LongHaulLow = parseInvariant(dt.Rows[0][_vsumCO2ColumnIndex]);
				simResult.StandardResults.LongHaulRef = parseInvariant(dt.Rows[1][_vsumCO2ColumnIndex]);

				simResult.StandardResults.RegionalDeliveryLow = parseInvariant(dt.Rows[2][_vsumCO2ColumnIndex]);

				simResult.StandardResults.RegionalDeliveryRef = parseInvariant(dt.Rows[3][_vsumCO2ColumnIndex]);

				if (dt.Rows.Count > 4)
				{
					simResult.StandardResults.UrbanDeliveryLow = parseInvariant(dt.Rows[4][_vsumCO2ColumnIndex]);
					simResult.StandardResults.UrbanDeliveryRef = parseInvariant(dt.Rows[5][_vsumCO2ColumnIndex]);
				}

				groupManager.GetSimulationResult(group).CO2Standard = simResult.StandardResults;
				groupManager.AddStandardSimulationResult(genericTowingVehicleCode, simResult);
			}
		}
	}
}
