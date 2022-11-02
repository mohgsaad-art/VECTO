using System.Collections.Generic;
using System.Xml.Linq;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation.Data;

namespace TUGraz.VectoCore.OutputData.XML.DeclarationReports.VehicleInformationFile
{
	public interface IXMLPrimaryVehicleReport
	{
		void Initialize(VectoRunData modelData, List<List<FuelData.Entry>> fuelModes);
		void WriteResult(XMLDeclarationReport.ResultEntry result);
		void GenerateReport(XElement fullReportHash);
		XDocument Report { get; }
	}
}