using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Diagnostics;
using System.Drawing.Text;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;
using Ninject;
using NUnit.Framework;
using NUnit.Framework.Interfaces;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.FileIO.XML;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.InputData.Reader.DataObjectAdapter;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.OutputData.FileIO;
using TUGraz.VectoCore.Tests.Helper.TrailerTool;
using TUGraz.VectoCore.Utils;
using XmlDocumentType = System.Xml.XmlDocumentType;

namespace TUGraz.VectoCore.Tests.XML
{

	public class ResultManager
	{

		public class SimulationResult
		{
			public int group;
			public string towingGroup;
			public bool referenceSimulated;
			public bool success;
			public string error;
			public TrailerReferenceRatios CO2Ref;
			public TrailerReferenceRatios CO2Standard;
			public TrailerReferenceRatios RefFactor;
		}

		public class StandardResult
		{
			public string XmlCode;
			public TrailerReferenceRatios StandardResults;
		}

		private Dictionary<int, SimulationResult> _vehicleGroupResults = new Dictionary<int, SimulationResult>();
		private Dictionary<string, StandardResult> _standardResults = new Dictionary<string, StandardResult>();
		public ResultManager(IList<int> groupsToSimulate)
		{
			foreach (var i in groupsToSimulate) {
				_vehicleGroupResults.Add(i, new SimulationResult() {
					group = i,
				});
			}
		}

		public void SetTowingVehicleGroups(TrailerSegments segments)
		{
			foreach (var group in _vehicleGroupResults) {
				var row = segments.GetRow(group.Key.ToString());
				group.Value.towingGroup = row["Generic_vehicle_group"].ToString();
			}
		}

		public SimulationResult GetSimulationResult(int group)
		{
			return _vehicleGroupResults[group];
		}

		public void AddStandardSimulationResult(string xmlCode, StandardResult standardResult)
		{
			_standardResults[xmlCode] = standardResult;

		}

		public bool GetStandardResult(string xmlCode, out StandardResult standardResult)
		{
			return _standardResults.TryGetValue(xmlCode, out standardResult);
		}


	}
	public class TrailerReferenceFactors
	{
		protected IXMLInputDataReader XMLInputReader;
		private IKernel _kernel;

		private const string SampleTrailer = @"TestData\Trailer\trailerSample.xml";

		private IDeclarationTrailerInputDataProvider inputDataProvider;
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


		private string _logFile = "log.txt";
		[SetUp]
		public void RunBeforeAnyTests()
		{
			if (File.Exists(_logFile))
			{
				File.Delete(_logFile);
			}
			Directory.SetCurrentDirectory(TestContext.CurrentContext.TestDirectory);
			Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture; //fixes double parsing
			_kernel = new StandardKernel(new VectoNinjectModule());
			XMLInputReader = _kernel.Get<IXMLInputDataReader>();

			//var reader = XmlReader.Create(SampleTrailer);
			//inputDataProvider = XMLInputReader.CreateDeclarationTrailer(reader);
		}

		private void SimulateReferenceTrailer(ResultManager groupManager, IList<int> vehicleGroups)
		{
			
			foreach(var group in vehicleGroups){
				var simResult = groupManager.GetSimulationResult(group); //holds the result of the simulation
				var writer = File.AppendText(_logFile);
				writer.Write(($"Simulating reference trailer for group {group}"));
				writer.Flush();
				writer.Close();

				try {
					var xDoc = XDocumentCreator.CreateXDocument(group);

					var fileWriter = new FileOutputWriter(SampleTrailer);

					var sumWriter = new SummaryDataContainer(fileWriter);
					var jobContainer = new JobContainer(sumWriter);

					var dataProvider = XMLInputReader.CreateDeclarationTrailer(xDoc.CreateReader());
					var runsFactory = new SimulatorFactory(ExecutionMode.Declaration, dataProvider, fileWriter) {
						ModalResults1Hz = false,
						WriteModalResults = false,
						ActualModalData = false,
						Validate = false,
					};
					

					jobContainer.AddRuns(runsFactory, (vectoRunData) => vectoRunData.ModFileSuffix.Contains("_genericTrailer"));
					
					jobContainer.Execute(true); //I guess the synchronization fix is not implemented in the trailerTool
					jobContainer.WaitFinished();

					var vsum = fileWriter.SumFileName;
					var dt = GetDataTableFromCsv(vsum);

					_vsumCO2ColumnIndex = 118;
				
					simResult.CO2Ref.LongHaulLow = parseInvariant(dt.Rows[0][_vsumCO2ColumnIndex]);
					simResult.CO2Ref.LongHaulRef = parseInvariant(dt.Rows[1][_vsumCO2ColumnIndex]);

					simResult.CO2Ref.RegionalDeliveryLow = parseInvariant(dt.Rows[2][_vsumCO2ColumnIndex]);

					simResult.CO2Ref.RegionalDeliveryRef = parseInvariant(dt.Rows[3][_vsumCO2ColumnIndex]);

					if (dt.Rows.Count > 4) {
						simResult.CO2Ref.UrbanDeliveryLow = parseInvariant(dt.Rows[4][_vsumCO2ColumnIndex]);
						simResult.CO2Ref.UrbanDeliveryRef = parseInvariant(dt.Rows[5][_vsumCO2ColumnIndex]);
					}



					simResult.success = true;
				} catch (Exception ex) {
					TestContext.WriteLine(ex.Message);
					TestContext.WriteLine($"Error in group{group}");
					ToCSV(CreateDataTable(groupManager, vehicleGroups), "TestOutput.csv");
					throw ex;
					Assert.Fail($"group{group}");
					var result = groupManager.GetSimulationResult(group);
					simResult.success = false;
					simResult.error = ex.Message;
				}
				simResult.referenceSimulated = true;
			}
		}

		public double parseInvariant(object stringFromCsv)
		{
			return double.Parse(stringFromCsv.ToString(), NumberFormatInfo.InvariantInfo);
		}

		public void RunTrailerDeclarationJob(bool serializeVectoRunData)
		{
			var rfDt = new DataTable();
			rfDt.Columns.Add(_vehicleGroup);
			rfDt.Columns.Add(_co2StandardLonghaulGKm);
			rfDt.Columns.Add(_co2StandardRegionalDeliveryGKm);
			rfDt.Columns.Add(_co2StandardUrbanDeliveryGKm);
			rfDt.Columns.Add(_co2ReferenceLonghaulGKm);
			rfDt.Columns.Add(_co2ReferenceRegionalDeliveryGKm);
			rfDt.Columns.Add(_co2ReferenceUrbanDeliveryGKm);
			rfDt.Columns.Add(_refFactorLonghaul);
			rfDt.Columns.Add(_refFactorRegionalDelivery);
			rfDt.Columns.Add(_refFactorUrbanDelivery);

			//System.Environment.SetEnvironmentVariable("referenceFactorRun", "true");

			//vehicle group is determined via
			//private DataRow GetTrailerDataRow(
			//	NumberOfTrailerAxles numberOfAxles, TypeTrailer typeTrailer, BodyWorkCode bodyWorkCode, bool volumeOrientation, Kilogram tpmlmAxleAssembly)
			//reference trailer is determined via vehiclegroup;


			var trailerList = DeclarationData.TrailerSegments.GetActiveVehicleGroups();

			trailerList = new List<int> { 31101 };//, 31102, 31201, 31301 };

			foreach (var group in trailerList) {

				System.Environment.SetEnvironmentVariable("vehicleGroup", group.ToString());

				try {
					var fileWriter = new FileOutputWriter(SampleTrailer);
					if (serializeVectoRunData) {
						var groupDir = Path.Combine(Path.GetDirectoryName(SampleTrailer), group.ToString());
						Directory.CreateDirectory(groupDir);
						var groupSamplePath = Path.Combine(groupDir + "\\dummy.xml"); //no real xml only used for the filewriter to use a different output path
						fileWriter = new FileOutputWriter(groupSamplePath);
					}
					
					var sumWriter = new SummaryDataContainer(fileWriter);
					var jobContainer = new JobContainer(sumWriter);
					var dataProvider = XMLInputReader.CreateDeclarationTrailer(XmlReader.Create(SampleTrailer));
					var runsFactory = new SimulatorFactory(ExecutionMode.Declaration, dataProvider, fileWriter) {
						ModalResults1Hz = false,
						WriteModalResults = false,
						ActualModalData = false,
						Validate = false,
						SerializeVectoRunData = serializeVectoRunData,
					};
					
					
					jobContainer.AddRuns(runsFactory);
					jobContainer.Execute();
					jobContainer.WaitFinished();

					var vsum = fileWriter.SumFileName;

					//CO2 Standard LongHaul [g/km]
					//CO2 Standard Regional Delivery[g/km]
					//CO2 Standard Urban Delivery[g/km]

					var mrf = XDocument.Load(fileWriter.XMLFullReportName);
					bool doubleParseSuccess = true;
					doubleParseSuccess &= double.TryParse(mrf.Descendants().Where(x => x.Name.ToString().Contains("Result")).Skip(1)
							.Descendants()
							.Where(x => x.Name.ToString().Contains("CO2"))?.FirstOrDefault()?.Value, 
						NumberStyles.AllowDecimalPoint, 
						NumberFormatInfo.InvariantInfo, 
						out var longHaulLowLoadCo2);

					doubleParseSuccess &= double.TryParse(mrf.Descendants().Where(x => x.Name.ToString().Contains("Result")).Skip(1).Skip(1)
							.Descendants()
							.Where(x => x.Name.ToString().Contains("CO2"))?.FirstOrDefault()?.Value,
						out var longHaulRefLoadCo2);

					var stlongHaulCo2 = longHaulLowLoadCo2 + "/" + longHaulRefLoadCo2;

					doubleParseSuccess &= double.TryParse(mrf.Descendants().Where(x => x.Name.ToString().Contains("Result")).Skip(2).Skip(1)
							.Descendants()
							.Where(x => x.Name.ToString().Contains("CO2"))?.FirstOrDefault()?.Value,
						out var regionalDeliveryLowLoadCo2);

					doubleParseSuccess &= double.TryParse(mrf.Descendants().Where(x => x.Name.ToString().Contains("Result")).Skip(3).Skip(1)
							.Descendants()
							.Where(x => x.Name.ToString().Contains("CO2"))?.FirstOrDefault()?.Value,
						out var regionalDeliveryRefLoadCo2);

					var stRegionalDeliveryCo2 = regionalDeliveryLowLoadCo2 + "/" + regionalDeliveryRefLoadCo2;

					doubleParseSuccess &= double.TryParse(mrf.Descendants().Where(x => x.Name.ToString().Contains("Result")).Skip(4).Skip(1)
							.Descendants()
							.Where(x => x.Name.ToString().Contains("CO2"))?.FirstOrDefault()?.Value,
						out var urbanDeliveryLowLoadCo2);

					doubleParseSuccess &= double.TryParse(mrf.Descendants().Where(x => x.Name.ToString().Contains("Result")).Skip(5).Skip(1)
							.Descendants()
							.Where(x => x.Name.ToString().Contains("CO2"))?.FirstOrDefault()?.Value,
						out var urbanDeliveryRefLoadCo2);

					var stUrbanDeliveryCo2 = urbanDeliveryLowLoadCo2 + "/" + urbanDeliveryRefLoadCo2;

					//Read data vsum file
					//CO2 Reference LongHaul [g/km]
					//CO2 Reference Regional Delivery [g/km]
					//CO2 Reference Urban Delivery [g/km]

					var dt = GetDataTableFromCsv(vsum);

					_vsumCO2ColumnIndex = 118;
					var refLongHaulCo2 = dt.Rows[1][_vsumCO2ColumnIndex] + "/" + dt.Rows[3][_vsumCO2ColumnIndex];

					var refRegionalDeliveryCo2 = dt.Rows[5][_vsumCO2ColumnIndex] + "/" + dt.Rows[7][_vsumCO2ColumnIndex];

					var refUrbanDeliveryCo2 = dt.Rows.Count > 10 ? dt.Rows[9][_vsumCO2ColumnIndex] + "/" + dt.Rows[11][_vsumCO2ColumnIndex] : "0/0";

					//Ref-Factor LongHaul  
					//Ref-Factor Regional Delivery 
					//Ref-Factor Urban Delivery 

					
					doubleParseSuccess &= double.TryParse(mrf.Descendants().Where(x => x.Name.ToString().Contains("Result")).Skip(1)
							.Descendants()
							.Where(x => x.Name.ToString().Contains("EfficiencyRatio"))?.FirstOrDefault()?.Value,
						out var longHaulLowLoadEf);

					doubleParseSuccess &= double.TryParse(mrf.Descendants().Where(x => x.Name.ToString().Contains("Result")).Skip(1).Skip(1)
							.Descendants()
							.Where(x => x.Name.ToString().Contains("EfficiencyRatio"))?.FirstOrDefault()?.Value,
						out var longHaulRefLoadEf);

					var longHaulReferenceFactor = (1 / longHaulLowLoadEf) + "/" + (1 / longHaulRefLoadEf);

					doubleParseSuccess &= double.TryParse(mrf.Descendants().Where(x => x.Name.ToString().Contains("Result")).Skip(2).Skip(1)
							.Descendants()
							.Where(x => x.Name.ToString().Contains("EfficiencyRatio"))?.FirstOrDefault()?.Value,
						out var regionalDeliveryLowLoadEf);

					doubleParseSuccess &= double.TryParse(mrf.Descendants().Where(x => x.Name.ToString().Contains("Result")).Skip(3).Skip(1)
							.Descendants()
							.Where(x => x.Name.ToString().Contains("EfficiencyRatio"))?.FirstOrDefault()?.Value,
						out var regionalDeliveryRefLoadEf);

					var regionalDeliveryReferenceFactor =
						(1 / regionalDeliveryLowLoadEf) + "/" + (1 / regionalDeliveryRefLoadEf);

					doubleParseSuccess &= double.TryParse(mrf.Descendants().Where(x => x.Name.ToString().Contains("Result")).Skip(4).Skip(1)
							.Descendants()
							.Where(x => x.Name.ToString().Contains("EfficiencyRatio"))?.FirstOrDefault()?.Value,
						out var urbanDeliveryLowLoadEf);

					doubleParseSuccess &= double.TryParse(mrf.Descendants().Where(x => x.Name.ToString().Contains("Result")).Skip(5).Skip(1)
							.Descendants()
							.Where(x => x.Name.ToString().Contains("EfficiencyRatio"))?.FirstOrDefault()?.Value,
						out var urbanDeliveryRefLoadEf);

					var urbanDeliveryReferenceFactor  = urbanDeliveryLowLoadEf != 0 && urbanDeliveryRefLoadEf != 0 ?
						(1 / urbanDeliveryLowLoadEf) + "/" + (1 / urbanDeliveryRefLoadEf) : "0/0";

					//Add columns to DataTable
					if (!doubleParseSuccess) {
						TestContext.WriteLine("parsing error");
					}
					//Assert.IsTrue(doubleParseSuccess, "double parsing failed");

					var dr = rfDt.NewRow();
					var rowArray = new object[] {
						group.ToString(),
						stlongHaulCo2, stRegionalDeliveryCo2, stUrbanDeliveryCo2,
						refLongHaulCo2, refRegionalDeliveryCo2, refUrbanDeliveryCo2,
						longHaulReferenceFactor, regionalDeliveryReferenceFactor, urbanDeliveryReferenceFactor
					};
					dr.ItemArray = rowArray;
					rfDt.Rows.Add(dr);
					//break;

				} catch (Exception ex) {
					TestContext.WriteLine(ex.Message);
				}
			}
			ToCSV(rfDt, @"TestData\Trailer\reference_factors.csv");
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

		[TestCase(false, TestName="Create reference factors")]
		[TestCase(true, TestName="Create reference factors (serialize output)")]
		public void CreateReferenceFactors(bool serializeOutput)
		{
			RunBeforeAnyTests();
			RunTrailerDeclarationJob(serializeOutput);
		}


		[TestCase(false)]
		public void CreateReferenceFactorsNew(bool serializeOutput)
		{
			var vehicleGroups = DeclarationData.TrailerSegments.GetActiveVehicleGroups();

			vehicleGroups = new List<int> { 11101, 11102, 11201, 11301, 11302, 11401, 11402, 11501, 11502, 15101, 15301, 15401, 15402, 15501, 15502, 21111 };
			vehicleGroups  = new List<int> { 23101};
			var groupManager = new ResultManager(vehicleGroups);
			SimulateReferenceTrailer(groupManager, vehicleGroups);
			ToCSV(CreateDataTable(groupManager, vehicleGroups), "referenceResults.csv");
			SimulateStandardTrailer(groupManager, vehicleGroups);
			ToCSV(CreateDataTable(groupManager, vehicleGroups), "standardAndReferenceResults.csv");
			CalculateReferenceFactors(groupManager,vehicleGroups);
			ToCSV(CreateDataTable(groupManager,vehicleGroups), "referenceFactors.csv");

		}


		private void CalculateReferenceFactors(ResultManager manager, IList<int> vehicleGroups)
		{
			foreach (var vehicleGroup in vehicleGroups) {
				var simResult = manager.GetSimulationResult(vehicleGroup);

				simResult.RefFactor.RegionalDeliveryLow = simResult.CO2Ref.RegionalDeliveryLow / simResult.CO2Standard.RegionalDeliveryLow;
				simResult.RefFactor.RegionalDeliveryRef = simResult.CO2Ref.RegionalDeliveryRef / simResult.CO2Standard.RegionalDeliveryRef;

				

				simResult.RefFactor.LongHaulLow = simResult.CO2Ref.LongHaulLow / simResult.CO2Standard.LongHaulLow;
				simResult.RefFactor.LongHaulRef = simResult.CO2Ref.LongHaulRef / simResult.CO2Standard.LongHaulRef;

				if (simResult.towingGroup != "9-LH") {
					simResult.RefFactor.UrbanDeliveryLow = simResult.CO2Ref.UrbanDeliveryLow / simResult.CO2Standard.UrbanDeliveryLow;
					simResult.RefFactor.UrbanDeliveryRef = simResult.CO2Ref.UrbanDeliveryRef / simResult.CO2Standard.UrbanDeliveryRef;
				}
			}
		}

		private DataTable CreateDataTable(ResultManager groupManager, IList<int> vehicleGroups)
		{
			var rfDt = new DataTable();

			rfDt.Columns.Add(_vehicleGroup);
			rfDt.Columns.Add(_co2StandardLonghaulGKm);
			rfDt.Columns.Add(_co2StandardRegionalDeliveryGKm);
			rfDt.Columns.Add(_co2StandardUrbanDeliveryGKm);
			rfDt.Columns.Add(_co2ReferenceLonghaulGKm);
			rfDt.Columns.Add(_co2ReferenceRegionalDeliveryGKm);
			rfDt.Columns.Add(_co2ReferenceUrbanDeliveryGKm);
			rfDt.Columns.Add(_refFactorLonghaul);
			rfDt.Columns.Add(_refFactorRegionalDelivery);
			rfDt.Columns.Add(_refFactorUrbanDelivery);


			foreach (var group in vehicleGroups) {
				var result = groupManager.GetSimulationResult(group);
				var row = rfDt.NewRow();
				row[_vehicleGroup] = group;
				row[_co2ReferenceLonghaulGKm] = $"{result.CO2Ref.LongHaulLow}/{result.CO2Ref.LongHaulRef}";
				row[_co2ReferenceRegionalDeliveryGKm] = $"{result.CO2Ref.RegionalDeliveryLow}/{result.CO2Ref.RegionalDeliveryRef}";
				row[_co2ReferenceUrbanDeliveryGKm] = $"{result.CO2Ref.UrbanDeliveryLow}/{result.CO2Ref.UrbanDeliveryRef}";

				row[_co2StandardLonghaulGKm] = $"{result.CO2Standard.LongHaulLow}/{result.CO2Standard.LongHaulRef}";
				row[_co2StandardRegionalDeliveryGKm] = $"{result.CO2Standard.RegionalDeliveryLow}/{result.CO2Standard.RegionalDeliveryRef}";
				row[_co2StandardUrbanDeliveryGKm] = $"{result.CO2Standard.UrbanDeliveryLow}/{result.CO2Standard.UrbanDeliveryRef}";

				row[_refFactorLonghaul] = $"{result.RefFactor.LongHaulLow}/{result.RefFactor.LongHaulRef}";
				row[_refFactorRegionalDelivery] = $"{result.RefFactor.RegionalDeliveryLow}/{result.RefFactor.RegionalDeliveryRef}";
				row[_refFactorUrbanDelivery] = $"{result.RefFactor.UrbanDeliveryLow}/{result.RefFactor.UrbanDeliveryRef}";
				
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




				var xDoc = XDocumentCreator.GetTowingVehicleXml(genericTowingVehicleCode, out var path); //doestn work with xDoc

				//
				var fileWriter = new FileOutputWriter(path);

				var sumWriter = new SummaryDataContainer(fileWriter);
				var jobContainer = new JobContainer(sumWriter);

				var dataProvider = XMLInputReader.CreateDeclaration(path);
				var runsFactory = new SimulatorFactory(ExecutionMode.Declaration, dataProvider, fileWriter)
				{
					ModalResults1Hz = false,
					WriteModalResults = false,
					ActualModalData = false,
					Validate = false,
				};


				jobContainer.AddRuns(runsFactory, (vectoRunData) => missionTypes.Contains(vectoRunData.Mission.MissionType));
				jobContainer.Execute(false); //I guess the synchronization fix is not implemented in the trailerTool
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



		/// <summary>
		/// This test checks the calculation of a tpmlmaxle assembly value based on tpmlm_min and tpmlm_max wich can be found in the trailerSegmentationTable.csv
		/// </summary>
		/// <param name="min"></param>
		/// <param name="max"></param>
		/// <param name="expected"></param>

		[TestCase(8, 9, 9999)]
		[TestCase(0,18.001, 18)]
		[TestCase(18,9999,9999)]
		[TestCase(13.5, 9999, 9999)]
		[TestCase(8, 13.501, 13)]
		public void TPMLMCalcTest(double min, double max, int expected)
		{
			Assert.AreEqual(expected, XDocumentCreator.GetMaxIntBetween(min, max));
		}
	}



}
