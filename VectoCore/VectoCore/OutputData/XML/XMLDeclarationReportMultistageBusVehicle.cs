using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.VehicleInformationFile;

namespace TUGraz.VectoCore.OutputData.XML
{
	public class XMLDeclarationReportMultistageBusVehicle : XMLDeclarationReport
	{
		private IXMLMultistageReport _multistageBusReport;

		public XMLDeclarationReportMultistageBusVehicle(IReportWriter writer)
			: base(writer)
		{
			//_multistageBusReport = new XMLMultistageBusReport();
		}
		
		public override void InitializeReport(VectoRunData modelData, List<List<FuelData.Entry>> fuelModes)
		{
			_multistageBusReport =
				modelData.Exempted ? new XMLMultistageExemptedBusReport() : new XMLMultistageBusReport();
			_multistageBusReport.Initialize(modelData);
		}
		
		protected override void GenerateReports()
		{
			_multistageBusReport.GenerateReport();
		}
		
		protected override void OutputReports()
		{
			Writer.WriteReport(ReportType.DeclarationReportMultistageVehicleXML, _multistageBusReport.Report);
		}
		
		protected override void DoStoreResult(ResultEntry entry, VectoRunData runData, IModalDataContainer modData)
		{
			throw new NotSupportedException();
		}
		protected override void WriteResult(ResultEntry result)
		{
			throw new NotSupportedException();
		}
	}
}
