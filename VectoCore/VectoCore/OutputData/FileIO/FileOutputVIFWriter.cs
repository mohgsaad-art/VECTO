using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Linq;


namespace TUGraz.VectoCore.OutputData.FileIO
{
	public class FileOutputVIFWriter : FileOutputWriter
	{
		//private string _jobFile;
		//private readonly int _numberOfManufacturingStages;
		

		public FileOutputVIFWriter(string jobFile, int numberOfManufacturingStages) : 
			base(jobFile, numberOfManufacturingStages)
		{
			//_jobFile = RemoveExistingVIFEndingPrefix(jobFile);
			//_numberOfManufacturingStages = numberOfManufacturingStages;
		}

		public override void WriteReport(ReportType type, XDocument data)
		{
			base.WriteReport(type, data);
			return;

			//var fileName = (string)null;
			//switch (type) {
			//	case ReportType.DeclarationReportMultistageVehicleXML:
			//		fileName = XMLMultistageReportFileName;
			//		break;
			//	default:
			//		base.WriteReport(type, data);
			//		break;
			//}

			//if (fileName == null)
			//	return;

			//using (var writer = new FileStream(fileName, FileMode.Create))
			//{
			//	using (var xmlWriter = new XmlTextWriter(writer, Encoding.UTF8))
			//	{
			//		xmlWriter.Formatting = Formatting.Indented;
			//		data.WriteTo(xmlWriter);
			//		xmlWriter.Flush();
			//		xmlWriter.Close();
			//	}
			//}

		}
	}
}
