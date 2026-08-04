using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using Ninject;
using NUnit.Framework;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCore.InputData.FileIO.XML;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Ninject;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.Common;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.CustomerInformationFile.CustomerInformationFile_0_9;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.CustomerInformationFile.CustomerInformationFile_0_9.ResultWriter;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.ManufacturerReport;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.ManufacturerReport.ManufacturerReport_0_9.ManufacturerReport;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.ManufacturerReport.ManufacturerReport_0_9.ManufacturerReportXMLTypeWriter;
using TUGraz.VectoCore.Utils;
using TUGraz.VectoCore.Utils.Ninject;
using XmlDocumentType = TUGraz.VectoCore.Utils.XmlDocumentType;

namespace TUGraz.VectoCore.Tests.XML.Reports
{
    
	
	public class MRF_CIF_WriterTestBase
	{
		protected string outputBasePath; //@"C:\Users\Harry\source\vecto\mrf_report_0_9";
		protected ISimulatorFactory _simulatorFactory;
		protected IOutputDataWriter _outputWriter;
		protected StandardKernel _kernel;
		protected IXMLInputDataReader _xmlReader;
		protected IManufacturerReportFactory _mrfFactory;
		protected ICustomerInformationFileFactory _cifFactory;

		protected const string ConventionalHeavyLorry =
			@"TestData/XML/XMLReaderDeclaration/SchemaVersion2.4/Distributed/HeavyLorry/Conventional_heavyLorry_AMT.xml";
		protected const string HEV_Px_HeavyLorry =
			@"TestData/XML/XMLReaderDeclaration/SchemaVersion2.4/Distributed/HeavyLorry/HEV_heavyLorry_AMT_Px_IHPC.xml";
		protected const string HEV_S2_HeavyLorry = 
			@"TestData/XML/XMLReaderDeclaration/SchemaVersion2.4/Distributed/HeavyLorry/HEV-S_heavyLorry_AMT_S2.xml";
		protected const string HEV_S3_HeavyLorry = 
			@"TestData/XML/XMLReaderDeclaration/SchemaVersion2.4/Distributed/HeavyLorry/HEV-S_heavyLorry_S3.xml";
		protected const string HEV_S4_HeavyLorry = 
			@"TestData/XML/XMLReaderDeclaration/SchemaVersion2.4/Distributed/HeavyLorry/HEV-S_heavyLorry_S4.xml";
		protected const string HEV_IEPC_S_HeavyLorry = 
			@"TestData/XML/XMLReaderDeclaration/SchemaVersion2.4/Distributed/HeavyLorry/HEV-S_heavyLorry_IEPC-S.xml";
		protected const string HEV_IEPC_S_NoAxlegear_HeavyLorry = 
			@"TestData/XML/XMLReaderDeclaration/SchemaVersion2.4/Distributed/HeavyLorry/HEV-S_heavyLorry_IEPC-S_NoAxlegear.xml";
		protected const string PEV_E2_HeavyLorry = 
			@"TestData/XML/XMLReaderDeclaration/SchemaVersion2.4/Distributed/HeavyLorry/PEV_heavyLorry_AMT_E2.xml";
		protected const string PEV_E3_HeavyLorry = 
			@"TestData/XML/XMLReaderDeclaration/SchemaVersion2.4/Distributed/HeavyLorry/PEV_heavyLorry_E3.xml";
		protected const string PEV_E4_HeavyLorry = 
			@"TestData/XML/XMLReaderDeclaration/SchemaVersion2.4/Distributed/HeavyLorry/PEV_heavyLorry_E4.xml";
		protected const string PEV_IEPC_HeavyLorry =
			@"TestData/XML/XMLReaderDeclaration/SchemaVersion2.4/Distributed/HeavyLorry/IEPC_heavyLorry.xml";
		protected const string PEV_IEPC_NoAxlegear_HeavyLorry = 
			@"TestData/XML/XMLReaderDeclaration/SchemaVersion2.4/Distributed/HeavyLorry/IEPC_heavyLorry_NoAxlegear.xml";

		protected const string Exempted_HeavyLorry =
			@"TestData/XML/XMLReaderDeclaration/SchemaVersion2.4/Distributed/ExemptedVehicles/exempted_heavyLorry.xml";

		protected const string Conventional_PrimaryBus = 
			@"TestData/XML/XMLReaderDeclaration/SchemaVersion2.4/Distributed/PrimaryBus/Conventional_primaryBus_AMT.xml";
		protected const string HEV_Px_IHPC_PrimaryBus =
			@"TestData/XML/XMLReaderDeclaration/SchemaVersion2.4/Distributed/PrimaryBus/HEV_primaryBus_AMT_Px.xml";
		protected const string HEV_S2_PrimaryBus = 
			@"TestData/XML/XMLReaderDeclaration/SchemaVersion2.4/Distributed/PrimaryBus/HEV-S_primaryBus_AMT_S2.xml";
		protected const string HEV_S3_PrimaryBus = 
			@"TestData/XML/XMLReaderDeclaration/SchemaVersion2.4/Distributed/PrimaryBus/HEV-S_primaryBus_S3.xml";
		protected const string HEV_S4_PrimaryBus = 
			@"TestData/XML/XMLReaderDeclaration/SchemaVersion2.4/Distributed/PrimaryBus/HEV-S_primaryBus_S4.xml";
		protected const string HEV_IEPC_S_PrimaryBus =
			@"TestData/XML/XMLReaderDeclaration/SchemaVersion2.4/Distributed/PrimaryBus/HEV-S_primaryBus_IEPC-S.xml";
		protected const string PEV_E2_PrimaryBus = 
			@"TestData/XML/XMLReaderDeclaration/SchemaVersion2.4/Distributed/PrimaryBus/PEV_primaryBus_AMT_E2.xml";
		protected const string PEV_E3_PrimaryBus = 
			@"TestData/XML/XMLReaderDeclaration/SchemaVersion2.4/Distributed/PrimaryBus/PEV_primaryBus_E3.xml";
		protected const string PEV_E4_PrimaryBus = 
			@"TestData/XML/XMLReaderDeclaration/SchemaVersion2.4/Distributed/PrimaryBus/PEV_primaryBus_E4.xml";
		protected const string PEV_IEPC_PrimaryBus =
			@"TestData/XML/XMLReaderDeclaration/SchemaVersion2.4/Distributed/PrimaryBus/IEPC_primaryBus.xml";

		protected const string Exempted_PrimaryBus =
			@"TestData/XML/XMLReaderDeclaration/SchemaVersion2.4/Distributed/ExemptedVehicles/exempted_primaryBus.xml";


		protected const string Conventional_CompletedBus = @"TestData/XML/XMLReaderDeclaration/SchemaVersionMultistage.0.1/vecto_multistage_conventional_final_vif.VIF_Report_1.xml";

		protected const string Exempted_CompletedBus = @"TestData/XML/XMLReaderDeclaration/SchemaVersionMultistage.0.1/exempted_completed.VIF_Report_2.xml";
		//@"TestData/XML/XMLReaderDeclaration/SchemaVersion2.4/Distributed/ExemptedVehicles/exempted_completedBus_input_full.xml";

		protected const string HEVCompletedBus = @"TestData/XML/XMLReaderDeclaration/SchemaVersion2.4/Distributed/CompletedBus/HEVCompletedBus_3.xml";
		 
		protected const string PEVCompletedBus = @"TestData/XML/XMLReaderDeclaration/SchemaVersion2.4/Distributed/CompletedBus/PEV_completedBus_3.xml";

		public static bool ValidateAndPrint(XDocument document, XmlDocumentType documentType)
		{
			var error = false;

			try {
				var stream = new MemoryStream();
				var writer = new XmlTextWriter(stream, Encoding.UTF8);
				document.WriteTo(writer);
				writer.Flush();
				stream.Flush();
				stream.Seek(0, SeekOrigin.Begin);
				var validator = new XMLValidator(new XmlTextReader(stream));
				error = !validator.ValidateXML(documentType);
				if (error) {
					TestContext.WriteLine(validator.ValidationError);
				}
			} finally {
				TestContext.WriteLine(document);
			}
			return !error;
		}

		public static void Validate(XDocument document, XmlDocumentType documentType)
		{
			var error = false;

			var stream = new MemoryStream();
			var writer = new XmlTextWriter(stream, Encoding.UTF8);
			document.WriteTo(writer);
			writer.Flush();
			stream.Flush();
			stream.Seek(0, SeekOrigin.Begin);
			var validator = new XMLValidator(new XmlTextReader(stream));
			error = !validator.ValidateXML(documentType);
			if (error) {
				TestContext.WriteLine(validator.ValidationError);
				Assert.Fail($"XML Validation failed {documentType}");
			}
		}


		protected bool WriteToDisk(string basePath, string fileName, XDocument xDocument)
		{
			TestContext.WriteLine($"{basePath},{fileName}");
			if (!fileName.EndsWith(".xml"))
			{
				fileName = fileName + ".xml";
			}

			if (!Directory.Exists(basePath)) {
				Directory.CreateDirectory(basePath);
			}
			var destPath = Path.Combine(basePath, fileName);
			using (var writer = new XmlTextWriter(destPath, Encoding.UTF8) { Formatting = Formatting.Indented })
			{
				try
				{
					xDocument.WriteTo(writer);
				}
				catch (Exception ex)
				{
					TestContext.WriteLine(ex);
					return false;
				}
			}

			return true;
		}

		[OneTimeSetUp]
		public void OneTimeSetup()
		{
			_kernel = new StandardKernel(
				new VectoNinjectModule()
			);
			outputBasePath = Path.Combine(TestContext.CurrentContext.TestDirectory, "XMLReports_0.9");

			// create a binding just for the tests to write dummy results (exempted). Vehicle category is set to Unknown
			_kernel.Bind<IResultsWriter>().To<MRFResultsWriter.ExemptedVehicle>()
				.Named(new VehicleTypeAndArchitectureStringHelperResults().GetName(XmlDocumentType.ManufacturerReport,
					"Unknown", VectoSimulationJobTypeHelper.Conventional, false));
			_kernel.Bind<IResultsWriter>().To<CIFResultsWriter.ExemptedVehicle>().Named(
				new VehicleTypeAndArchitectureStringHelperResults().GetName(XmlDocumentType.CustomerReport,
					"Unknown", VectoSimulationJobTypeHelper.Conventional, false));
			_kernel.Bind<IResultsWriter>().To<CIFResultsWriter.ExemptedVehicle>().Named(
				new VehicleTypeAndArchitectureStringHelperResults().GetName(XmlDocumentType.CustomerReport,
					"Unknown", VectoSimulationJobTypeHelper.Conventional, false, true));
		}

		[SetUp]
		public void SetUp()
		{
			//_outputWriter = new FileOutputWriter()
			_xmlReader = _kernel.Get<IXMLInputDataReader>();
			_mrfFactory = _kernel.Get<IManufacturerReportFactory>();
			_cifFactory = _kernel.Get<ICustomerInformationFileFactory>();

			
		}
	}

    
	[TestFixture]
    public class ManufacturerReportWriterTest : MRF_CIF_WriterTestBase
	{


		private IXMLManufacturerReport GetReport(string fileName,
			out IDeclarationInputDataProvider dataProvider)
		{
			Assert.IsFalse(string.IsNullOrEmpty(fileName));
			dataProvider = _xmlReader.CreateDeclaration(fileName);
			

			dataProvider.JobInputData.Vehicle.VehicleCategory.GetVehicleType(); // HEV/PEV - Sx/Px
			var ihpc = (dataProvider.JobInputData.Vehicle.Components?.ElectricMachines?.Entries)?.Count(electric =>
				electric.ElectricMachine.IHPCType != "None") > 0;
			var iepc = (dataProvider.JobInputData.Vehicle.Components?.IEPC != null);

			var report = _mrfFactory.GetManufacturerReport(
				dataProvider.JobInputData.Vehicle.VehicleCategory,
				dataProvider.JobInputData.JobType,
				dataProvider.JobInputData.Vehicle.ArchitectureID,
                dataProvider.JobInputData.Vehicle.ExemptedVehicle,
				iepc,
				ihpc);

			return report;
		}

		private IXMLManufacturerReport GetCompletedBusReport(string fileName,
			out IMultistepBusInputDataProvider dataProvider)
		{
			Assert.IsFalse(string.IsNullOrEmpty(fileName));
			dataProvider = _xmlReader.CreateDeclaration(fileName) as IMultistepBusInputDataProvider;
			Assert.NotNull(dataProvider);
			var arch = dataProvider.JobInputData.PrimaryVehicle.Vehicle.ArchitectureID;

			dataProvider.JobInputData.PrimaryVehicle.Vehicle.VehicleCategory.GetVehicleType();// HEV/PEV - Sx/Px


			var ihpc = (dataProvider.JobInputData.PrimaryVehicle.Vehicle.Components.ElectricMachines?.Entries)?.Count(electric => electric.ElectricMachine.IHPCType != "None") > 0;
			var iepc = (dataProvider.JobInputData.PrimaryVehicle.Vehicle.Components.IEPC != null);
			
			var report = _mrfFactory.GetManufacturerReport(
				dataProvider.JobInputData.ConsolidateManufacturingStage.Vehicle.VehicleCategory,
				dataProvider.JobInputData.JobType,
				dataProvider.JobInputData.PrimaryVehicle.Vehicle.ArchitectureID,
                dataProvider.JobInputData.PrimaryVehicle.Vehicle.ExemptedVehicle,
				iepc,
				ihpc);

			return report;
		}


		
		[TestCase(ConventionalHeavyLorry)]
		public async Task ConventionalLorryMRFTest(string fileName)
		{
			var report = GetReport(fileName, out var dataProvider) as ConventionalLorryManufacturerReport;

			report.Initialize(GetRunData(dataProvider));
			report.GenerateReport();

			Assert.IsTrue(ValidateAndPrint(report.Report, XmlDocumentType.ManufacturerReport));
            Assert.IsTrue(WriteToDisk(outputBasePath, TestContext.CurrentContext.Test.MethodName, report.Report));
        }

		private VectoRunData GetRunData(IDeclarationInputDataProvider dataProvider)
		{
			//var vehicleMock = new Mock<IVehicleDeclarationInputData>();
			//vehicleMock.Setup(x => x.o)
			return new VectoRunData() {
				InputData = dataProvider,
				VehicleData = new VehicleData() {
					OffVehicleCharging = false
				}
			};
		}

		[TestCase(HEV_Px_HeavyLorry)]
		public async Task HEV_Px_LorryMRFTest(string fileName)
		{
			var report = GetReport(fileName, out var dataProvider) as HEV_Px_IHPC_LorryManufacturerReport;
			Assert.NotNull(report);
			report.Initialize(GetRunData(dataProvider));
			report.GenerateReport();

			Assert.IsTrue(ValidateAndPrint(report.Report, XmlDocumentType.ManufacturerReport));
			Assert.IsTrue(WriteToDisk(outputBasePath, TestContext.CurrentContext.Test.MethodName, report.Report));
		}

		[TestCase(HEV_S2_HeavyLorry)]
		public async Task HEV_S2_LorryMRFTest(string fileName)
		{
			var report = GetReport(fileName, out var dataProvider) as HEV_S2_LorryManufacturerReport;
			Assert.NotNull(report);
			report.Initialize(GetRunData(dataProvider));
			report.GenerateReport();

			Assert.IsTrue(ValidateAndPrint(report.Report, XmlDocumentType.ManufacturerReport));
			Assert.IsTrue(WriteToDisk(outputBasePath, TestContext.CurrentContext.Test.MethodName, report.Report));
		}

		[TestCase(HEV_S3_HeavyLorry)]
		public async Task HEV_S3_LorryMRFTest(string fileName)
		{
			var report = GetReport(fileName, out var dataProvider) as HEV_S3_LorryManufacturerReport;
			Assert.NotNull(report);
			report.Initialize(GetRunData(dataProvider));
			report.GenerateReport();

			Assert.IsTrue(ValidateAndPrint(report.Report, XmlDocumentType.ManufacturerReport));
			Assert.IsTrue(WriteToDisk(outputBasePath, TestContext.CurrentContext.Test.MethodName, report.Report));
		}

		[TestCase(HEV_S4_HeavyLorry)]
		public async Task HEV_S4_LorryMRFTest(string fileName)
		{
			var report = GetReport(fileName, out var dataProvider) as HEV_S4_LorryManufacturerReport;
			Assert.NotNull(report);
			report.Initialize(GetRunData(dataProvider));
			report.GenerateReport();

			Assert.IsTrue(ValidateAndPrint(report.Report, XmlDocumentType.ManufacturerReport));
			Assert.IsTrue(WriteToDisk(outputBasePath, TestContext.CurrentContext.Test.MethodName, report.Report));
		}

		[TestCase(HEV_IEPC_S_HeavyLorry)]
		public async Task HEV_IEPC_S_LorryMRFTest(string fileName)
		{
			var report = GetReport(fileName, out var dataProvider) as HEV_IEPC_S_LorryManufacturerReport;
			Assert.NotNull(report);
			report.Initialize(GetRunData(dataProvider));
			report.GenerateReport();

			Assert.IsTrue(ValidateAndPrint(report.Report, XmlDocumentType.ManufacturerReport));
			Assert.IsTrue(WriteToDisk(outputBasePath, TestContext.CurrentContext.Test.MethodName, report.Report));
		}

		[TestCase(PEV_E2_HeavyLorry)]
		public async Task PEV_E2_LorryMRFTest(string fileName)
		{
			var report = GetReport(fileName, out var dataProvider) as PEV_E2_LorryManufacturerReport;
			Assert.NotNull(report);
			report.Initialize(GetRunData(dataProvider));
			report.GenerateReport();

			Assert.IsTrue(ValidateAndPrint(report.Report, XmlDocumentType.ManufacturerReport));
			Assert.IsTrue(WriteToDisk(outputBasePath, TestContext.CurrentContext.Test.MethodName, report.Report));
		}

		[TestCase(PEV_E3_HeavyLorry)]
		public async Task PEV_E3_LorryMRFTest(string fileName)
		{

			var report = GetReport(fileName, out var dataProvider) as PEV_E3_LorryManufacturerReport;
			Assert.NotNull(report);
			report.Initialize(GetRunData(dataProvider));
			report.GenerateReport();

			Assert.IsTrue(ValidateAndPrint(report.Report, XmlDocumentType.ManufacturerReport));
			Assert.IsTrue(WriteToDisk(outputBasePath, TestContext.CurrentContext.Test.MethodName, report.Report));
		}

		[TestCase(PEV_E4_HeavyLorry)]
		public async Task PEV_E4_LorryMRFTest(string fileName)
		{
			var report = GetReport(fileName, out var dataProvider) as PEV_E4_LorryManufacturerReport;
			Assert.NotNull(report);
			report.Initialize(GetRunData(dataProvider));
			report.GenerateReport();

			Assert.IsTrue(ValidateAndPrint(report.Report, XmlDocumentType.ManufacturerReport));
			Assert.IsTrue(WriteToDisk(outputBasePath, TestContext.CurrentContext.Test.MethodName, report.Report));
		}

		[TestCase(PEV_IEPC_HeavyLorry)]
		public async Task PEV_IEPC_LorryMRFTest(string fileName)
		{
			var report = GetReport(fileName, out var dataProvider) as PEV_IEPC_LorryManufacturerReport;
			Assert.NotNull(report);
			report.Initialize(GetRunData(dataProvider));
			report.GenerateReport();

			Assert.IsTrue(ValidateAndPrint(report.Report, XmlDocumentType.ManufacturerReport));
			Assert.IsTrue(WriteToDisk(outputBasePath, TestContext.CurrentContext.Test.MethodName, report.Report));
		}

        [TestCase(Exempted_HeavyLorry, TestName="ExemptedHeavyLorry_MRF")]
		public void Exempted_LorryMRFTest(string fileName)
		{
			var report = GetReport(fileName, out var dataProvider); 
			Assert.NotNull(report as Exempted_LorryManufacturerReport);
			report.Initialize(GetRunData(dataProvider));
			report.GenerateReport();

			Assert.IsTrue(ValidateAndPrint(report.Report, XmlDocumentType.ManufacturerReport));
			Assert.IsTrue(WriteToDisk(outputBasePath, TestContext.CurrentContext.Test.MethodName, report.Report));
		}


		[TestCase(Conventional_PrimaryBus)]
		public void ConventionalPrimaryBusTest(string fileName)
		{
			var report = GetReport(fileName, out var dataProvider) as Conventional_PrimaryBus_ManufacturerReport;
			Assert.NotNull(report);
			report.Initialize(GetRunData(dataProvider));
            report.GenerateReport();

			Assert.IsTrue(ValidateAndPrint(report.Report, XmlDocumentType.ManufacturerReport));
			Assert.IsTrue(WriteToDisk(outputBasePath, TestContext.CurrentContext.Test.MethodName, report.Report));
		}

		[TestCase(HEV_Px_IHPC_PrimaryBus)]
		public void HEV_Px_IHPC_PrimaryBusTest(string fileName)
		{
			var report = GetReport(fileName, out var dataProvider) as HEV_Px_IHPC_PrimaryBus_ManufacturerReport;
			Assert.NotNull(report);
			report.Initialize(GetRunData(dataProvider));
			report.GenerateReport();

			Assert.IsTrue(ValidateAndPrint(report.Report, XmlDocumentType.ManufacturerReport));
			Assert.IsTrue(WriteToDisk(outputBasePath, TestContext.CurrentContext.Test.MethodName, report.Report));
		}




		[TestCase(HEV_S2_PrimaryBus)]
		public void HEV_S2_PrimaryBusTest(string fileName)
		{
			var report = GetReport(fileName, out var dataProvider) as HEV_S2_PrimaryBus_ManufacturerReport;
			Assert.NotNull(report);
			report.Initialize(GetRunData(dataProvider));
			report.GenerateReport();

			Assert.IsTrue(ValidateAndPrint(report.Report, XmlDocumentType.ManufacturerReport));
			Assert.IsTrue(WriteToDisk(outputBasePath, TestContext.CurrentContext.Test.MethodName, report.Report));
		}


		[TestCase(HEV_S3_PrimaryBus)]
		public void HEV_S3_PrimaryBusTest(string fileName)
		{
			var report = GetReport(fileName, out var dataProvider) as HEV_S3_PrimaryBus_ManufacturerReport;
			Assert.NotNull(report);
			report.Initialize(GetRunData(dataProvider));
			report.GenerateReport();

			Assert.IsTrue(ValidateAndPrint(report.Report, XmlDocumentType.ManufacturerReport));
			Assert.IsTrue(WriteToDisk(outputBasePath, TestContext.CurrentContext.Test.MethodName, report.Report));
		}

		[TestCase(HEV_S4_PrimaryBus)]
		public void HEV_S4_PrimaryBusTest(string fileName)
		{
			var report = GetReport(fileName, out var dataProvider) as HEV_S4_PrimaryBus_ManufacturerReport;
			Assert.NotNull(report);
			report.Initialize(GetRunData(dataProvider));
			report.GenerateReport();

			Assert.IsTrue(ValidateAndPrint(report.Report, XmlDocumentType.ManufacturerReport));
			Assert.IsTrue(WriteToDisk(outputBasePath, TestContext.CurrentContext.Test.MethodName, report.Report));
		}

		[TestCase(HEV_IEPC_S_PrimaryBus)]
		public void HEV_IEPC_S_PrimaryBusTest(string fileName)
		{
			var report = GetReport(fileName, out var dataProvider) as HEV_IEPC_S_PrimaryBus_ManufacturerReport;
			Assert.NotNull(report);
			report.Initialize(GetRunData(dataProvider));
			report.GenerateReport();

			Assert.IsTrue(ValidateAndPrint(report.Report, XmlDocumentType.ManufacturerReport));
			Assert.IsTrue(WriteToDisk(outputBasePath, TestContext.CurrentContext.Test.MethodName, report.Report));
		}



		[TestCase(PEV_E2_PrimaryBus)]
		public void PEV_E2_PrimaryBusTest(string fileName)
		{
			var report = GetReport(fileName, out var dataProvider) as PEV_E2_PrimaryBus_ManufacturerReport;
			Assert.NotNull(report);
			report.Initialize(GetRunData(dataProvider));
			report.GenerateReport();

			Assert.IsTrue(ValidateAndPrint(report.Report, XmlDocumentType.ManufacturerReport));
			Assert.IsTrue(WriteToDisk(outputBasePath, TestContext.CurrentContext.Test.MethodName, report.Report));
		}


		[TestCase(PEV_E3_PrimaryBus)]
		public void PEV_E3_PrimaryBusTest(string fileName)
		{
			var report = GetReport(fileName, out var dataProvider) as PEV_E3_PrimaryBus_ManufacturerReport;
			Assert.NotNull(report);
			report.Initialize(GetRunData(dataProvider));
			report.GenerateReport();

			Assert.IsTrue(ValidateAndPrint(report.Report, XmlDocumentType.ManufacturerReport));
			Assert.IsTrue(WriteToDisk(outputBasePath, TestContext.CurrentContext.Test.MethodName, report.Report));
		}

		[TestCase(PEV_E4_PrimaryBus)]
		public void PEV_E4_PrimaryBusTest(string fileName)
		{
			var report = GetReport(fileName, out var dataProvider) as PEV_E4_PrimaryBus_ManufacturerReport;
			Assert.NotNull(report);
			report.Initialize(GetRunData(dataProvider));
			report.GenerateReport();

			Assert.IsTrue(ValidateAndPrint(report.Report, XmlDocumentType.ManufacturerReport));
			Assert.IsTrue(WriteToDisk(outputBasePath, TestContext.CurrentContext.Test.MethodName, report.Report));
		}


		[TestCase(PEV_IEPC_PrimaryBus)]
		public void PEV_IEPC_PrimaryBusTest(string fileName)
		{
			var report = GetReport(fileName, out var dataProvider) as PEV_IEPC_PrimaryBus_ManufacturerReport;
			Assert.NotNull(report);
			report.Initialize(GetRunData(dataProvider));
			report.GenerateReport();

			report.GenerateReport();

			Assert.IsTrue(ValidateAndPrint(report.Report, XmlDocumentType.ManufacturerReport));
			Assert.IsTrue(WriteToDisk(outputBasePath, TestContext.CurrentContext.Test.MethodName, report.Report));
		}

		[TestCase(Exempted_PrimaryBus)]
		public void ExemptedPrimaryBusTest(string fileName)
		{
			var report = GetReport(fileName, out var dataProvider) as Exempted_PrimaryBus_ManufacturerReport;
			Assert.NotNull(report);
			report.Initialize(GetRunData(dataProvider));

			report.GenerateReport();

			Assert.IsTrue(ValidateAndPrint(report.Report, XmlDocumentType.ManufacturerReport));
			Assert.IsTrue(WriteToDisk(outputBasePath, TestContext.CurrentContext.Test.MethodName, report.Report));
		}

		[TestCase(Conventional_CompletedBus)]
        public void Conventional_CompletedBusTest(string fileName)
        {
			var report = GetCompletedBusReport(fileName, out var dataProvider) as Conventional_CompletedBusManufacturerReport;
			Assert.NotNull(report);
            report.Initialize(GetRunData(dataProvider));
			report.GenerateReport();

			Assert.IsTrue(ValidateAndPrint(report.Report, XmlDocumentType.ManufacturerReport));
			Assert.IsTrue(WriteToDisk(outputBasePath, TestContext.CurrentContext.Test.MethodName, report.Report));
		}

		[Ignore("No testfile")]
		[TestCase("")]
		public void HEV_CompletedBusTest(string fileName)
		{
			Assert.IsFalse(string.IsNullOrEmpty(fileName));

		}
		[Ignore("No testfile")]
		[TestCase("")]
		public void PEV_CompletedBusTest(string fileName)
		{
			Assert.IsFalse(string.IsNullOrEmpty(fileName));
		}

		[Ignore("No testfile")]
		[TestCase("")]
		public void Exempted_CompletedBusTest(string fileName)
		{
			Assert.IsFalse(string.IsNullOrEmpty(fileName));
		}

	}
}
