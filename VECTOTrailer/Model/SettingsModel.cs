using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using VECTOTrailer.Helper;

namespace VECTOTrailer.Model
{
	public class SettingsModel
	{
		private readonly ITrailerToolSettings _settings;

		public SettingsModel(ITrailerToolSettings settings)
		{
			_settings = settings;
			SetupDefaultPaths();
		}
		

		public string XmlFilePathFolder
		{
			get { return _settings.GeneralSettings.XMLFilesPathFolder; }
			set
			{
				_settings.GeneralSettings.XMLFilesPathFolder = value;
				_settings.GeneralSettings.Save();
			}
		}

		private void SetupDefaultPaths()
		{
			if (XmlFilePathFolder == string.Empty)
			{
				var location = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
				var defaultPath = location; //+
				XmlFilePathFolder = Path.GetFullPath(new Uri(defaultPath).LocalPath);
			}
		}
	}
}
