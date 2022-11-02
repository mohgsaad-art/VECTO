using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace TUGraz.VectoCore.OutputData.XML.DeclarationJobs
{
	public class XMLCompletedBusWriter
	{
		private XmlWriterSettings _xmlWriterSettings;
		private StringBuilder _stringBuilder;


		public XMLCompletedBusWriter()
		{
			_xmlWriterSettings = new XmlWriterSettings{Indent = true};
			_stringBuilder = new StringBuilder();
		}
		
		public bool WriteCompletedBusXml(string filePath, XDocument xmlDocument)
		{
			if (string.IsNullOrEmpty(filePath))
				return false;
			
			using (var xmlWriter = XmlWriter.Create(_stringBuilder, _xmlWriterSettings))
			{
				xmlDocument.WriteTo(xmlWriter);
				xmlWriter.Flush();
			}

			xmlDocument.Save(filePath);
			return true;
		}
	}
}
