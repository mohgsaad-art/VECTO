using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using Ninject;
using TUGraz.Vecto.IntegrationTests.Utils.DummyRun;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore;
using TUGraz.VectoCore.InputData.FileIO.XML;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Utils;
using XmlDocumentType = TUGraz.VectoCore.Utils.XmlDocumentType;

namespace TUGraz.Vecto.IntegrationTests.TestCases.Declaration.SimulationRuns.FullReportTests;

public class FullReportTestsBase
{
    private IKernel _vectoKernel;
	protected ISimulatorFactoryFactory _simFactoryFactory;
	protected IXMLInputDataReader _inputDataReader;

    // #####
    // #####
    // #####
    protected bool WRITE_REPORTS_TO_FILESYSTEM { get; set; }
    protected bool WRITE_REPORTS_TO_OUTPUT { get; set; }


    protected const string BasePath = @"TestData/XML/v2.4/MockupJobData/";

	protected void SetupNinject()
	{
		_vectoKernel = new StandardKernel(
			new VectoNinjectModule(),
			new DummyRunNinjectModule()
		);

		_simFactoryFactory = _vectoKernel.Get<ISimulatorFactoryFactory>();
		Assert.NotNull(_simFactoryFactory);
		_inputDataReader = _vectoKernel.Get<IXMLInputDataReader>();
		Assert.NotNull(_inputDataReader);
	}


    public bool CheckElementExists(string name, XDocument xDoc)
    {
        return xDoc.XPathSelectElements($"//*[local-name()='{name}']").FirstOrDefault() != null;
    }

	public void CheckElementCount(string name, XDocument xDoc, int count)
	{
		Assert.AreEqual(count, xDoc.XPathSelectElements($"//*[local-name()='{name}']").Count(), $"element count of '{name}' mismatch");
	}

	protected void AssertElementValue(XDocument xdoc, string expectedValue, params string[] xPath)
	{
		var query = "./" + string.Join("/",
			xPath.Where(x => x != null).Select(x => $"/*[local-name()='{x}']").ToArray());
		var node = xdoc.XPathSelectElement(query);
		Assert.AreEqual(expectedValue, node.Value);
	}

	protected IEnumerable<XElement> GetElements(XDocument xdoc, params string[] xPath)
	{
		var query = "./" + string.Join("/",
			xPath.Where(x => x != null).Select(x => $"/*[local-name()='{x}']").ToArray());
        return xdoc.XPathSelectElements(query);
    }

    public bool ValidateAndPrint(XDocument document, XmlDocumentType documentType)
    {
        var error = false;

        try
        {
            var stream = new MemoryStream();
            var writer = new XmlTextWriter(stream, Encoding.UTF8);
            document.WriteTo(writer);
            writer.Flush();
            stream.Flush();
            stream.Seek(0, SeekOrigin.Begin);
            var validator = new XMLValidator(new XmlTextReader(stream));
            error = !validator.ValidateXML(documentType);
            if (error)
            {
                TestContext.WriteLine(validator.ValidationError);
            }
        }
        finally
        {
            if (WRITE_REPORTS_TO_OUTPUT)
            {
                TestContext.WriteLine(document);
            }
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
        if (error)
        {
            TestContext.WriteLine(validator.ValidationError);
            Assert.Fail($"XML Validation failed {documentType}");
        }
    }

    private bool CheckReport(bool shouldExist, string filename, XDocument report, XmlDocumentType docType)
    {
        var fail = false;
        if (WRITE_REPORTS_TO_FILESYSTEM)
        {
            if (shouldExist)
            {
                if (File.Exists(filename))
                {
                    Validate(XDocument.Load(filename), docType);
                    TestContext.WriteLine($"{filename} exists and is valid.");
                }
                else
                {
                    TestContext.WriteLine($"{filename} missing!");
                }
            }
            else
            {
                if (File.Exists(filename))
                {
                    fail = true;
                    TestContext.WriteLine($"{filename} should not exist!");
                }
            }
        }

        if (shouldExist)
        {
            Validate(report, docType);
            TestContext.WriteLine($"{docType} exists and is valid.");
        }
        else
        {
            if (report != null)
            {
                TestContext.WriteLine($"{docType} should not be set!");
                fail = true;
            }
        }

        return fail;
    }

	protected void CheckReportExists(DummyRunReportWriter fileWriter,
            bool MrfShouldExist = true,
            bool CifShouldExist = true,
            bool VifShouldExist = false,
            bool PrimaryMrfShouldExist = false,
            bool PrimaryReportShouldExist = false)
    {
        var fail = false;

        fail |= CheckReport(CifShouldExist, fileWriter.XMLCustomerReportName, fileWriter.XMLCustomerReport,
            XmlDocumentType.CustomerReport);
        fail |= CheckReport(MrfShouldExist, fileWriter.XMLFullReportName, fileWriter.XMLManufacturerReport,
            XmlDocumentType.ManufacturerReport);
        fail |= CheckReport(VifShouldExist, fileWriter.XMLPrimaryVehicleReportName, fileWriter.XMLMultistageReport,
            XmlDocumentType.MultistepOutputData);

		if (fail) {
            Assert.Fail();
        }

    }

	protected void CheckElementTypeNameContains(XDocument xDoc, string elementName, params string[] expectedType)
	{
		var xmlDoc = new XmlDocument();
		xmlDoc.Load(xDoc.CreateReader());
		var validator = new XMLValidator(xmlDoc);
		validator.ValidateXML(XmlDocumentType.MultistepOutputData);

		var vehicleNodes = xmlDoc.SelectNodes($"//*[local-name()='{elementName}']");
		foreach (XmlNode vehicleNode in vehicleNodes) {
			var typeName = vehicleNode?.SchemaInfo?.SchemaType?.Name ?? "";
			var contains = expectedType.Select(x => typeName.Contains(x, StringComparison.InvariantCultureIgnoreCase));
			Assert.IsTrue(contains.Any(x => x), $"{typeName} -- {expectedType.Join()}");
		}
	}

    protected void Clearfiles(DummyRunReportWriter fileWriter)
    {
        IList<string> filesToBeCleared = new List<string>() {
            fileWriter.XMLPrimaryVehicleReportName,
            fileWriter.XMLFullReportName,
            fileWriter.XMLCustomerReportName,
            fileWriter.XMLMonitoringReportName
        };
        foreach (var fileName in filesToBeCleared)
        {
            if (File.Exists(fileName))
            {
                File.Delete(fileName);
            }
        }
    }

	protected string[] CopyInputFile(params string[] fileNames)
    {
        if (!WRITE_REPORTS_TO_FILESYSTEM)
        {
            return new string[] { };
        }
        var subDirectory = Path.Combine("MockupReports", TestContext.CurrentContext.Test.Name, "Input");
        Directory.CreateDirectory(Path.GetFullPath(subDirectory));
        var retVal = new List<string>();
        foreach (var file in fileNames)
        {
            var output = Path.Combine(subDirectory, Path.GetFileName(file));
            File.Copy(file, output, true);
            retVal.Add(output);
        }

        return retVal.ToArray();
    }

    public DummyRunReportWriter GetReportWriter(string subDirectory, string originalFilePath)
    {
        subDirectory = Path.Combine("MockupReports", subDirectory);
        Directory.CreateDirectory(Path.GetFullPath(subDirectory));
        var path = Path.Combine(Path.Combine(Path.GetFullPath(subDirectory)), Path.GetFileName(originalFilePath));
        return new DummyRunReportWriter(path);
    }
}