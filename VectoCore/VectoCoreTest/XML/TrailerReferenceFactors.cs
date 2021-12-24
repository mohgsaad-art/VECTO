using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
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


		public void RunBeforeAnyTests()
		{
			Directory.SetCurrentDirectory(TestContext.CurrentContext.TestDirectory);

			_kernel = new StandardKernel(new VectoNinjectModule());
			XMLInputReader = _kernel.Get<IXMLInputDataReader>();

			//var reader = XmlReader.Create(SampleTrailer);
			//inputDataProvider = XMLInputReader.CreateDeclarationTrailer(reader);
		}

		public void RunTrailerDeclarationJob()
		{
			var rfDt = new DataTable();
			rfDt.Columns.Add("Vehicle Group");
			rfDt.Columns.Add("CO2 Standard LongHaul [g/km]");
			rfDt.Columns.Add("CO2 Standard Regional Delivery [g/km]");
			rfDt.Columns.Add("CO2 Standard Urban Delivery [g/km]");
			rfDt.Columns.Add("CO2 Reference LongHaul [g/km]");
			rfDt.Columns.Add("CO2 Reference Regional Delivery [g/km]");
			rfDt.Columns.Add("CO2 Reference Urban Delivery [g/km]");
			rfDt.Columns.Add("Ref-Factor LongHaul");
			rfDt.Columns.Add("Ref-Factor Regional Delivery");
			rfDt.Columns.Add("Ref-Factor Urban Delivery");

			System.Environment.SetEnvironmentVariable("referenceFactorRun", "true");

			var trailerList = DeclarationData.TrailerSegments.GetActiveVehicleGroups();
			foreach (var group in trailerList) {

				System.Environment.SetEnvironmentVariable("vehicleGroup", group.ToString());

				var fileWriter = new FileOutputWriter(SampleTrailer);
				var sumWriter = new SummaryDataContainer(fileWriter);
				var jobContainer = new JobContainer(sumWriter);
				var dataProvider = XMLInputReader.CreateDeclarationTrailer(XmlReader.Create(SampleTrailer));
				var runsFactory = new SimulatorFactory(ExecutionMode.Declaration, dataProvider, fileWriter) {
					ModalResults1Hz = false,
					WriteModalResults = false,
					ActualModalData = false,
					Validate = false,
				};
				jobContainer.AddRuns(runsFactory);
				jobContainer.Execute();
				jobContainer.WaitFinished();
				var vsum = fileWriter.SumFileName;

				//CO2 Standard LongHaul [g/km]
				//CO2 Standard Regional Delivery[g/km]
				//CO2 Standard Urban Delivery[g/km]


				var mrf = XDocument.Load(fileWriter.XMLFullReportName);

				double.TryParse(mrf.Descendants().Where(x => x.Name.ToString().Contains("Result")).Skip(1).Descendants()
						.Where(x => x.Name.ToString().Contains("CO2"))?.FirstOrDefault()?.Value,
					out var longHaulLowLoadCo2);

				double.TryParse(mrf.Descendants().Where(x => x.Name.ToString().Contains("Result")).Skip(1).Skip(1).Descendants()
						.Where(x => x.Name.ToString().Contains("CO2"))?.FirstOrDefault()?.Value,
					out var longHaulRefLoadCo2);

				var stlongHaulCo2 = longHaulLowLoadCo2 + "/" + longHaulRefLoadCo2;

				double.TryParse(mrf.Descendants().Where(x => x.Name.ToString().Contains("Result")).Skip(2).Skip(1).Descendants()
						.Where(x => x.Name.ToString().Contains("CO2"))?.FirstOrDefault()?.Value,
					out var regionalDeliveryLowLoadCo2);

				double.TryParse(mrf.Descendants().Where(x => x.Name.ToString().Contains("Result")).Skip(3).Skip(1).Descendants()
						.Where(x => x.Name.ToString().Contains("CO2"))?.FirstOrDefault()?.Value,
					out var regionalDeliveryRefLoadCo2);

				var stRegionalDeliveryCo2 = regionalDeliveryLowLoadCo2 + "/" + regionalDeliveryRefLoadCo2;

				double.TryParse(mrf.Descendants().Where(x => x.Name.ToString().Contains("Result")).Skip(4).Skip(1).Descendants()
						.Where(x => x.Name.ToString().Contains("CO2"))?.FirstOrDefault()?.Value,
					out var urbanDeliveryLowLoadCo2);

				double.TryParse(mrf.Descendants().Where(x => x.Name.ToString().Contains("Result")).Skip(5).Skip(1).Descendants()
						.Where(x => x.Name.ToString().Contains("CO2"))?.FirstOrDefault()?.Value,
					out var urbanDeliveryRefLoadCo2);

				var stUrbanDeliveryCo2 = urbanDeliveryLowLoadCo2 + "/" + urbanDeliveryRefLoadCo2;

				//Read data vsum file
				//CO2 Reference LongHaul [g/km]
				//CO2 Reference Regional Delivery [g/km]
				//CO2 Reference Urban Delivery [g/km]

				var dt = GetDataTableFromCsv(vsum);

				var refLongHaulCo2 = dt.Rows[1][114] + "/" + dt.Rows[3][114];

				var refRegionalDeliveryCo2 = dt.Rows[5][114] + "/" + dt.Rows[7][114];

				var refUrbanDeliveryCo2 = dt.Rows[9][114] + "/" + dt.Rows[11][114];

				//Ref-Factor LongHaul  
				//Ref-Factor Regional Delivery 
				//Ref-Factor Urban Delivery 

				double.TryParse(mrf.Descendants().Where(x => x.Name.ToString().Contains("Result")).Skip(1).Descendants()
						.Where(x => x.Name.ToString().Contains("EfficiencyRatio"))?.FirstOrDefault()?.Value,
					out var longHaulLowLoadEf);

				double.TryParse(mrf.Descendants().Where(x => x.Name.ToString().Contains("Result")).Skip(1).Skip(1).Descendants()
						.Where(x => x.Name.ToString().Contains("EfficiencyRatio"))?.FirstOrDefault()?.Value,
					out var longHaulRefLoadEf);

				var longHaulReferenceFactor = (1 / longHaulLowLoadEf) + "/"+ (1 / longHaulRefLoadEf);

				double.TryParse(mrf.Descendants().Where(x => x.Name.ToString().Contains("Result")).Skip(2).Skip(1).Descendants()
						.Where(x => x.Name.ToString().Contains("EfficiencyRatio"))?.FirstOrDefault()?.Value,
					out var regionalDeliveryLowLoadEf);

				double.TryParse(mrf.Descendants().Where(x => x.Name.ToString().Contains("Result")).Skip(3).Skip(1).Descendants()
						.Where(x => x.Name.ToString().Contains("EfficiencyRatio"))?.FirstOrDefault()?.Value,
					out var regionalDeliveryRefLoadEf);

				var regionalDeliveryReferenceFactor = (1 / regionalDeliveryLowLoadEf) + "/" + (1 / regionalDeliveryRefLoadEf);

				double.TryParse(mrf.Descendants().Where(x => x.Name.ToString().Contains("Result")).Skip(4).Skip(1).Descendants()
						.Where(x => x.Name.ToString().Contains("EfficiencyRatio"))?.FirstOrDefault()?.Value,
					out var urbanDeliveryLowLoadEf);

				double.TryParse(mrf.Descendants().Where(x => x.Name.ToString().Contains("Result")).Skip(5).Skip(1).Descendants()
						.Where(x => x.Name.ToString().Contains("EfficiencyRatio"))?.FirstOrDefault()?.Value,
					out var urbanDeliveryRefLoadEf);

				var urbanDeliveryReferenceFactor = (1 / urbanDeliveryLowLoadEf) + "/" + (1 / urbanDeliveryRefLoadEf);

				//Add columns to DataTable

				var dr = rfDt.NewRow(); 
				var rowArray = new object[] { group.ToString(),
					stlongHaulCo2, stRegionalDeliveryCo2, stUrbanDeliveryCo2,
					refLongHaulCo2, refRegionalDeliveryCo2, refUrbanDeliveryCo2,
					longHaulReferenceFactor, regionalDeliveryReferenceFactor, urbanDeliveryReferenceFactor
				};
				dr.ItemArray = rowArray;
				rfDt.Rows.Add(dr);
				//break;
			}
			ToCSV(rfDt, @"TestData\Trailer\ReferenceFactors.csv");
		}

		private static void ToCSV(DataTable dtDataTable, string strFilePath)
		{
			var sw = new StreamWriter(strFilePath, false);
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
			RunBeforeAnyTests();
			RunTrailerDeclarationJob();
		}

	}
}
