using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.WindowsAPICodePack.Dialogs;
using OpenFileDialog = System.Windows.Forms.OpenFileDialog;
using SaveFileDialog = Microsoft.Win32.SaveFileDialog;


namespace VECTOTrailer.Helper
{
	public static class FileDialogHelper
	{
		public const string XMLExtension = ".xml";
		public const string JobFileExtension = ".vecto";

		public const string XMLFilter = "XML Files (*.xml)|*.xml|All Files (*.*)|*.*";
		public const string JobFilter = "XML Files |*.xml|Vecto Files (*.vecto)|*.vecto|All Files (*.*)|*.*";

		private static Dictionary<string, string> _selectLastDirectory = new Dictionary<string, string>();
		private static Dictionary<string, string> _saveLastDirectory = new Dictionary<string, string>();


		public static string[] ShowSelectFilesDialog(bool multiselect, string filter = XMLFilter, string initialDirectory = null)
		{
			if(!_selectLastDirectory.TryGetValue(filter, out var prevDirectory))
			{
				_selectLastDirectory.Add(filter, null);
			}

			using (var openFileDialog = new OpenFileDialog())
			{
				openFileDialog.InitialDirectory = initialDirectory ?? prevDirectory;
				openFileDialog.Multiselect = multiselect;
				openFileDialog.Filter = filter;
				var result = openFileDialog.ShowDialog();

				if (result == DialogResult.OK) {
					var fileNames = openFileDialog.FileNames;
					_selectLastDirectory[filter] = Path.GetDirectoryName(fileNames.First());
					return fileNames;
				}
			}

			return null;
		}
		
		public static string ShowSelectDirectoryDialog(string initialDirectory = null)
		{
			using (var dialog = new CommonOpenFileDialog())
			{
				dialog.InitialDirectory = initialDirectory;
				dialog.IsFolderPicker = true;

				var result = dialog.ShowDialog();
				if (result == CommonFileDialogResult.Ok)
				{
					return dialog.FileName;
				}
			}

			return null;
		}

		public static string SaveXmlFileToDialog(string initialDirectory = null)
		{
			return SaveToDialog(initialDirectory, XMLFilter);
		}

		public static string SaveJobFileToDialog(string initialDirectory = null)
		{
			return SaveToDialog(initialDirectory, JobFilter);
		}

		private static string SaveToDialog(string initialDirectory, string filter)
		{
			var saveFileDialog = new SaveFileDialog
			{
				Filter = filter
			};

			if (!_saveLastDirectory.TryGetValue(filter, out var prevDirectory)) {
				_saveLastDirectory.Add(filter,null);
			}

			if (initialDirectory != null) {
				saveFileDialog.InitialDirectory = initialDirectory ?? null;
			}

			if (saveFileDialog.ShowDialog() == true) {
				var fileName = saveFileDialog.FileName;
				_saveLastDirectory[filter] = Path.GetFullPath(fileName);
				return fileName;
			} else {
				return null;
			}
		}
	}
}
