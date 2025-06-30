using System.Xml.Linq;
using TUGraz.VectoCore.Models.Simulation.Data;

namespace TUGraz.VectoCore.OutputData.XML.DeclarationReports.VehicleInformationFile
{
	public interface IXMLVehicleInformationFile
	{
		void Initialize(VectoRunData modelData);
		void WriteResult(IResultEntry result);
		void GenerateReport(XElement fullReportHash);
		XDocument Report { get; }
	}
}