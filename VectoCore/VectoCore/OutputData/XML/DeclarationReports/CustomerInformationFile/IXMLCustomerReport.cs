using System.Collections.Generic;
using System.Xml.Linq;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation.Data;

namespace TUGraz.VectoCore.OutputData.XML.DeclarationReports.CustomerInformationFile
{
	public interface IXMLCustomerReport
	{
		void Initialize(VectoRunData modelData, List<List<FuelData.Entry>> fuelModes);
		XDocument Report { get; }
		void WriteResult(XMLDeclarationReport.ResultEntry resultValue);
		void GenerateReport(XElement resultSignature);
	}
}