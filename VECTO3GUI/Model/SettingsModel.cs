using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace VECTO3GUI.Model
{
	public class SettingsModel
	{
		public SettingsModel()
		{
			SetupDefaultPaths();
		}
		
		public string SavePathFolder
		{
			get { return Properties.Settings.Default.SavePathFolder; }
			set
			{
				Properties.Settings.Default.SavePathFolder = value;
				Properties.Settings.Default.Save();
			}
		}

		public string XmlFilePathFolder
		{
			get { return Properties.Settings.Default.XMLFilesPathFolder; }
			set
			{
				Properties.Settings.Default.XMLFilesPathFolder = value;
				Properties.Settings.Default.Save();
			}
		}

		private void SetupDefaultPaths()
		{
			if (XmlFilePathFolder == string.Empty)
			{
				var location = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
				var defaultPath = location; //+
					//@"\..\..\..\VectoCore\VectoCoreTest\TestData\XML\XMLReaderDeclaration\SchemaVersion2.6_Buses";
				XmlFilePathFolder = Path.GetFullPath(new Uri(defaultPath).LocalPath);
			}

			if (SavePathFolder == string.Empty)
			{
				SavePathFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
			}
		}
	}
}
