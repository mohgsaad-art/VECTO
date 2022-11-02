using System;
using System.Collections.Generic;
using System.Xml.Linq;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.CustomerInformationFile;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.ManufacturerReport;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.VehicleInformationFile;

namespace TUGraz.VectoCore.OutputData.XML {
	public class XMLDeclarationReportPrimaryVehicle : XMLDeclarationReport
	{
		protected IXMLPrimaryVehicleReport PrimaryReport;


		public XMLDeclarationReportPrimaryVehicle(IReportWriter writer, bool writePIF = false) : base(writer)
		{
		}

		public override XDocument CustomerReport => null;

		public override XDocument PrimaryVehicleReport => PrimaryReport?.Report;


		#region Overrides of XMLDeclarationReport

		protected override void InstantiateReports(VectoRunData modelData)
		{
			if (modelData.Exempted) {
				ManufacturerRpt = new XMLManufacturerReportExeptedPrimaryBus();
				CustomerRpt = new XMLCustomerReportExemptedPrimaryBus();
				PrimaryReport = new XMLExemptedPrimaryBusVehicleReport();

			} else {
				ManufacturerRpt = new XMLManufacturerReportPrimaryBus();
				CustomerRpt = new XMLCustomerReport();
				PrimaryReport = new XMLPrimaryBusVehicleReport();
			}


		}

		public override void InitializeReport(VectoRunData modelData, List<List<FuelData.Entry>> fuelModes)
		{
			base.InitializeReport(modelData, fuelModes);
			PrimaryReport.Initialize(modelData,fuelModes);
		}



		protected override void WriteResult(ResultEntry result)
		{
			base.WriteResult(result);
			PrimaryReport.WriteResult(result);
		}

		protected override void GenerateReports()
		{
			ManufacturerRpt.GenerateReport();
			var fullReportHash = GetSignature(ManufacturerRpt.Report);
			CustomerRpt.GenerateReport(fullReportHash);
			PrimaryReport.GenerateReport(fullReportHash);
		}

	

		protected override void OutputReports()
		{
			Writer.WriteReport(ReportType.DeclarationReportManufacturerXML, ManufacturerRpt.Report);
			Writer.WriteReport(ReportType.DeclarationReportPrimaryVehicleXML, PrimaryReport.Report);
		}

		#endregion
	}
}