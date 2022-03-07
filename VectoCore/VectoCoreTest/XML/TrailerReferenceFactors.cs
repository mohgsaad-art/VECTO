using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Xml;
using System.Xml.Linq;
using Ninject;
using NUnit.Framework;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.FileIO.XML;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.InputData.Reader.DataObjectAdapter;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.OutputData.FileIO;
using TUGraz.VectoCore.Utils;
using XmlDocumentType = System.Xml.XmlDocumentType;

namespace TUGraz.VectoCore.Tests.XML
{

	public class TrailerReferenceFactors
	{
		protected IXMLInputDataReader XMLInputReader;
		private IKernel _kernel;

		private const string SampleTrailer = @"TestData\Trailer\trailerSample.xml";

		private IDeclarationTrailerInputDataProvider inputDataProvider;
		private int _vsumCO2ColumnIndex;


		public void RunBeforeAnyTests()
		{
			Directory.SetCurrentDirectory(TestContext.CurrentContext.TestDirectory);
			Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture; //fixes double parsing
			_kernel = new StandardKernel(new VectoNinjectModule());
			XMLInputReader = _kernel.Get<IXMLInputDataReader>();

			//var reader = XmlReader.Create(SampleTrailer);
			//inputDataProvider = XMLInputReader.CreateDeclarationTrailer(reader);
		}

		public void RunTrailerDeclarationJob(bool serializeVectoRunData)
		{
			var rfDt = new DataTable();
			rfDt.Columns.Add("Vehicle_Group");
			rfDt.Columns.Add("CO2 Standard LongHaul [g/km]");
			rfDt.Columns.Add("CO2 Standard Regional Delivery [g/km]");
			rfDt.Columns.Add("CO2 Standard Urban Delivery [g/km]");
			rfDt.Columns.Add("CO2 Reference LongHaul [g/km]");
			rfDt.Columns.Add("CO2 Reference Regional Delivery [g/km]");
			rfDt.Columns.Add("CO2 Reference Urban Delivery [g/km]");
			rfDt.Columns.Add("Ref-Factor_LongHaul");
			rfDt.Columns.Add("Ref-Factor_Regional_Delivery");
			rfDt.Columns.Add("Ref-Factor_Urban_Delivery");

			System.Environment.SetEnvironmentVariable("referenceFactorRun", "true");

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
	}
}
