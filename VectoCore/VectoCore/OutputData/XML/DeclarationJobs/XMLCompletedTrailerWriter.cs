using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using Castle.Core.Internal;


namespace TUGraz.VectoCore.OutputData.XML.DeclarationJobs
{
	public class XMLCompletedTrailerWriter
	{
		private XmlWriterSettings _xmlWriterSettings;
		private StringBuilder _stringBuilder;


		public XMLCompletedTrailerWriter()
		{
			_xmlWriterSettings = new XmlWriterSettings{Indent = true};
			_stringBuilder = new StringBuilder();
		}
		
		public bool WriteCompletedTrailerXml(string filePath, XDocument xmlDocument)
		{
			if (filePath.IsNullOrEmpty())
				return false;
			
			using (var xmlWriter = XmlWriter.Create(_stringBuilder, _xmlWriterSettings))
			{
				xmlDocument.WriteTo(xmlWriter);
				xmlWriter.Flush();
			}

			xmlDocument.Save(filePath, SaveOptions.OmitDuplicateNamespaces);
			return true;
		}
	}
}
