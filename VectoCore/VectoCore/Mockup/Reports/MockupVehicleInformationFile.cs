using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.XPath;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.OutputData.XML;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.ManufacturerReport.ManufacturerReport_0_9;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.VehicleInformationFile;

namespace TUGraz.VectoMockup.Reports
{
	internal class MockupInterimVehicleInformationFile : IXMLMultistepIntermediateReport
	{
		private readonly IXMLMultistepIntermediateReport _intermediateVifImplementation;

		public MockupInterimVehicleInformationFile(IXMLMultistepIntermediateReport vifImplementation)
		{
			_intermediateVifImplementation = vifImplementation;
		}

		#region Implementation of IXMLMultistepIntermediateReport

		public void Initialize(VectoRunData modelData)
		{
			_intermediateVifImplementation.Initialize(modelData);
		}

		public XDocument Report => _intermediateVifImplementation.Report;

		public void GenerateReport()
		{
			_intermediateVifImplementation.GenerateReport();
		}

		#endregion

	}


	internal class MockupPrimaryVehicleInformationFile : IXMLVehicleInformationFile
    {
		
		private readonly IXMLVehicleInformationFile _vehicleInformationFileImplementation;
		private VectoRunData _modelData;

		public MockupPrimaryVehicleInformationFile(IXMLVehicleInformationFile vehicleInformationFileImplementation)
		{
			_vehicleInformationFileImplementation = vehicleInformationFileImplementation;
		}


		#region Implementation of IXMLPrimaryVehicleReport

		public void Initialize(VectoRunData modelData)
		{
			_vehicleInformationFileImplementation.Initialize(modelData);
			_modelData = modelData;
		}

		public void WriteResult(IResultEntry result)
		{
			_vehicleInformationFileImplementation.WriteResult(result);
		}

		#endregion

		#region Implementation of IXMLMockupReport

		public void GenerateReport(XElement fullReportHash)
		{ 
			_vehicleInformationFileImplementation.GenerateReport(fullReportHash);
		}

		public XDocument Report
		{
			get
			{
				var report = _vehicleInformationFileImplementation.Report;
				return report;
			}
		}

		#endregion
	}
}
