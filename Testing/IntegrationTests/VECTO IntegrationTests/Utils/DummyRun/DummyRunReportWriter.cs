using System.Data;
using System.Xml.Linq;
using Newtonsoft.Json;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.OutputData.FileIO;

namespace TUGraz.Vecto.IntegrationTests.Utils.DummyRun;

public class DummyRunReportWriter : IOutputDataWriter
{
    private readonly FileOutputWriter _fileWriter;

    public XDocument XMLManufacturerReport => ManufacturerReports.FirstOrDefault();

    public IReadOnlyList<XDocument> XMLManufacturerReports => ManufacturerReports;

    public List<XDocument> ManufacturerReports { get; } = new List<XDocument>();

    public XDocument XMLCustomerReport { get; internal set; }

    public XDocument XMLMultistageReport { get; internal set; }

    public XDocument XMLMonitoringReport => MonitoringReports.FirstOrDefault();

    public IReadOnlyList<XDocument> XMLMonitoringReports => MonitoringReports;

    public List<XDocument> MonitoringReports { get; } = new List<XDocument>();

    public DummyRunReportWriter(string jobFile)
    {
        _fileWriter = new FileOutputWriter(jobFile);
    }

    protected DummyRunReportWriter(string jobFile, int numberOfManufacturingStages)
    {
        _fileWriter = new FileOutputVIFWriter(jobFile, numberOfManufacturingStages);
    }


    #region Implementation of IModalDataWriter

    public void WriteModData(int jobRunId, string runName, string cycleName, string runSuffix, DataTable modData)
    {
        // not used for now
    }

    #endregion

    #region Implementation of IReportWriter

    public void WriteReport(ReportType type, XDocument data)
    {
        switch (type)
        {
            case ReportType.DeclarationReportManufacturerXML:
                ManufacturerReports.Add(data);
                break;
            case ReportType.DeclarationReportCustomerXML:
                XMLCustomerReport = data;
                break;
            case ReportType.DeclarationReportMonitoringXML:
                MonitoringReports.Add(data);
                break;
            case ReportType.DeclarationReportPrimaryVehicleXML:
                XMLMultistageReport = data;
                break;
            case ReportType.DeclarationReportMultistageVehicleXML:
                XMLMultistageReport = data;
                break;
            default:
                throw new ArgumentOutOfRangeException("ReportType", type, null);
        }
    }

    public void WriteReport(ReportType type, Stream data)
    {
        throw new NotImplementedException("not supported");
    }

    public IDictionary<ReportType, string> GetWrittenFiles()
    {
        throw new NotImplementedException();
    }

    public int NumberOfManufacturingStages { get; set; }


    public XDocument MultistageXmlReport => XMLMultistageReport;

    #endregion

    #region Implementation of ISummaryWriter

    public void WriteSumData(DataTable sortedAndFilteredTable)
    {
        throw new NotImplementedException();
    }

    #endregion

    public void WriteAllReports()
    {
        if (XMLCustomerReport != null)
        {
            _fileWriter.WriteReport(ReportType.DeclarationReportCustomerXML, XMLCustomerReport);
        }

        if (XMLManufacturerReports.Count > 0)
        {
            if (XMLManufacturerReports.Count == 2)
            {
                var tmpWriter = new TempFileOutputWriter(this, ReportType.DeclarationReportManufacturerXML);
                tmpWriter.WriteReport(ReportType.DeclarationReportManufacturerXML, XMLManufacturerReports.First());
                _fileWriter.WriteReport(ReportType.DeclarationReportManufacturerXML, XMLManufacturerReports.Last());
            }
            else if (XMLManufacturerReports.Count == 1)
            {
                _fileWriter.WriteReport(ReportType.DeclarationReportManufacturerXML, XMLManufacturerReport);
            }
            else
            {
                throw new NotImplementedException("Unexpected number of manufacturer reports");
            }
        }

        if (XMLMonitoringReports.Count > 0)
        {
            if (XMLMonitoringReports.Count == 2)
            {
                var tmpWriter = new TempFileOutputWriter(this, ReportType.DeclarationReportMonitoringXML);
                tmpWriter.WriteReport(ReportType.DeclarationReportMonitoringXML, XMLMonitoringReports.First());
                _fileWriter.WriteReport(ReportType.DeclarationReportMonitoringXML, XMLMonitoringReports.Last());
            }
            else if (XMLMonitoringReports.Count == 1)
            {
                _fileWriter.WriteReport(ReportType.DeclarationReportMonitoringXML, XMLManufacturerReport);
            }
            else
            {
                throw new NotImplementedException("Unexpected number of monitoring reports");
            }
        }

        if (XMLMultistageReport != null)
        {
            _fileWriter.WriteReport(ReportType.DeclarationReportPrimaryVehicleXML, XMLMultistageReport);
        }

    }

    public string WriteJSONJobCompleted(string vif, string completeBusInput, string subDirectory)
    {
        subDirectory = Path.Combine("MockupReports", subDirectory, "Input");

        var header = new Dictionary<string, object>() {
            { "FileVersion", 7 }
        };
        var body = new Dictionary<string, object>() {
            { "PrimaryVehicleResults", Path.GetRelativePath(subDirectory, Path.GetFullPath(vif)) },
            { "CompletedVehicle", Path.GetRelativePath(subDirectory, Path.GetFullPath(completeBusInput)) },
            { "RunSimulation", true}
        };
        var json = new Dictionary<string, object>() {
            {"Header", header},
            {"Body", body}
        };

        Directory.CreateDirectory(Path.GetFullPath(subDirectory));
        var path = Path.Combine(Path.Combine(Path.GetFullPath(subDirectory)), "completedJob.vecto");
        var str = JsonConvert.SerializeObject(json, Formatting.Indented);
        File.WriteAllText(path, str);
        return path;

    }

    #region Implementation of IOutputDataWriter

    public string JobFile => _fileWriter.JobFile;
    public string XMLCustomerReportName => _fileWriter.XMLCustomerReportName;
    public string XMLFullReportName => _fileWriter.XMLFullReportName;
    public string XMLPrimaryVehicleReportName => _fileWriter.XMLPrimaryVehicleReportName;
    public string XMLMonitoringReportName => _fileWriter.XMLMonitoringReportName;

    #endregion
}