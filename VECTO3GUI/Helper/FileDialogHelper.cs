using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.WindowsAPICodePack.Dialogs;
using OpenFileDialog = System.Windows.Forms.OpenFileDialog;
using SaveFileDialog = Microsoft.Win32.SaveFileDialog;


namespace VECTO3GUI.Helper
{
	public static class FileDialogHelper
	{
		public const string XMLExtension = ".xml";
		public const string JobFileExtension = ".vecto";

		public const string XMLFilter = "XML Files (*.xml)|*.xml|All Files (*.*)|*.*";
		public const string JobFilter = "Vecto Files (*.vecto)|*.vecto|All Files (*.*)|*.*";

		public static string[] ShowSelectFilesDialog(bool multiselect)
		{
			return ShowSelectFilesDialog(multiselect, XMLFilter);
		}
		
		public static string[] ShowSelectFilesDialog(bool multiselect, string filter = XMLFilter, string initialDirectory = null)
		{
			using (var openFileDialog = new OpenFileDialog())
			{
				openFileDialog.InitialDirectory = initialDirectory;
				openFileDialog.Multiselect = multiselect;
				openFileDialog.Filter = filter;
				var result = openFileDialog.ShowDialog();

				if (result == DialogResult.OK)
				{
					return openFileDialog.FileNames;
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

			if (initialDirectory != null)
				saveFileDialog.InitialDirectory = initialDirectory;

			return saveFileDialog.ShowDialog() == true ? saveFileDialog.FileName : null;
		}
	}
}
