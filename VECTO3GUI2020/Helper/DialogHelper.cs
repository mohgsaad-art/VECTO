using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Forms;
using VECTO3GUI2020.Properties;
using MessageBox = System.Windows.MessageBox;

namespace VECTO3GUI2020.Helper
{
    public class DialogHelper : IDialogHelper
	{
		private readonly string _defaultInitialDirectory = Settings.Default.DefaultFilePath;

		#region File and Folder Dialogs
		public const string XmlFilter = "XML Files (*.xml)|*.xml";
		public const string JsonFilter = "JSON Files (*.json)|*.json";
		public const string VectoJobFilter = "Vecto Files (*.vecto)|*.vecto";
		public const string XmlAndVectoJobFilter = "Vecto Files (*.xml, *.vecto)|*.xml;*.vecto";

		private Dictionary<string, string> lastUsedLoadDirectories = new Dictionary<string, string>();
		private Dictionary<string, string> lastUsedSaveDirectories = new Dictionary<string, string>();

		private string lastUsedDirectoryFolderPicker = null;
		private string[] OpenFilesDialog(string filter, string initialDirectory, bool multiselect)
		{
			if (initialDirectory == null) {
				initialDirectory = LookUpLastDir(lastUsedLoadDirectories, filter);
			}


			
			using (OpenFileDialog fd = new OpenFileDialog {
				InitialDirectory = initialDirectory ?? _defaultInitialDirectory,
				Multiselect = multiselect,
				Filter = filter,
				RestoreDirectory = true
			}) {
				var result = fd.ShowDialog();
				if (result == DialogResult.OK) {
					lastUsedLoadDirectories[filter] = Path.GetDirectoryName(fd.FileName);
					return fd.FileNames;
				}
			}


			return null;
		}
		public string SaveToDialog(string initialDirectory, string filter)
		{
			if (initialDirectory == null) {
				initialDirectory = LookUpLastDir(lastUsedSaveDirectories, filter);
			}
			using (var saveFileDialog = new SaveFileDialog
			{
				Filter = filter,
				InitialDirectory = initialDirectory ?? _defaultInitialDirectory,
				AddExtension = true
			}) {
				var result = saveFileDialog.ShowDialog();
				{
					if (result == DialogResult.OK) {
						lastUsedSaveDirectories[filter] = Path.GetDirectoryName(saveFileDialog.FileName);
						return saveFileDialog.FileName;
					}
				}

				return null;
			}
		}

		private static string LookUpLastDir(Dictionary<string, string> dict, string filter)
		{
			string lastUsedDirectory = null;
			if (dict.TryGetValue(filter, out lastUsedDirectory)) {
				return lastUsedDirectory;
			} else {
				return Settings.Default.DefaultFilePath;
			}
		}

		public string OpenFileDialog(string filter = "All files (*.*)|*.*", string initialDirectory = null)
		{
			return OpenFilesDialog(filter, initialDirectory)?[0];
		}

		public string[] OpenFilesDialog(string filter, string initialDirectory)
		{
			return OpenFilesDialog(filter, initialDirectory, true);
		}

		public string OpenXMLFileDialog()
		{
			return OpenXMLFileDialog(null);
		}



		public string[] OpenXMLFilesDialog(string initialDirectory)
		{
			return OpenFilesDialog(XmlFilter, initialDirectory);
		}

		public string OpenXMLFileDialog(string initialDirectory)
		{
			return OpenFilesDialog(XmlFilter, initialDirectory, false)?[0];
		}

		public string OpenJsonFileDialog(string initialDirectory)
		{
			return OpenFilesDialog(VectoJobFilter, initialDirectory, false)?[0];
		}

		public string OpenXMLAndVectoFileDialog(string initialDirectory)
		{
			return OpenFilesDialog(XmlAndVectoJobFilter, initialDirectory, false)?[0];
		}


		public string OpenFolderDialog(string initialDirectory = null)
		{

			if (initialDirectory == null) {
				initialDirectory = lastUsedDirectoryFolderPicker;
			}
			using (var dialog = new FolderBrowserDialog()) {
				dialog.SelectedPath = initialDirectory;

				var result = dialog.ShowDialog();
				if (result == DialogResult.OK) {
					lastUsedDirectoryFolderPicker = Path.GetDirectoryName(dialog.SelectedPath);
					return dialog.SelectedPath;
				}
			}
			return null;
		}

		#endregion

		#region Messagebox

		public MessageBoxResult ShowMessageBox(string messageBoxText,
			string caption,
			MessageBoxButton button,
			MessageBoxImage icon)
		{
			return MessageBox.Show(messageBoxText, caption, button, icon);
		}

		public MessageBoxResult ShowMessageBox(string messageBoxText, string caption)
		{
			return MessageBox.Show(messageBoxText, caption);
		}




		public string SaveToXMLDialog(string initialDirectory)
		{
			return SaveToDialog(initialDirectory, XmlFilter);
		}

		public string SaveToVectoJobDialog(string initialDirectory)
		{
			return SaveToDialog(initialDirectory, VectoJobFilter);
		}

		public MessageBoxResult ShowErrorMessage(string errorMessage, string caption)
		{
			return ShowMessageBox(errorMessage, caption, MessageBoxButton.OK, MessageBoxImage.Error);
		}

		public MessageBoxResult ShowErrorMessage(string errorMessage)
		{
			return ShowErrorMessage(errorMessage, "Error");
		}


		#endregion
	}

	public interface IDialogHelper
	{
		/// <summary>
		/// Opens a dialog to open a file
		/// </summary>
		/// <param name="filter"></param>
		/// <param name="initialDirectory">If no directory is specified the location of the assembly is used</param>
		/// <returns></returns>
		string OpenFileDialog(string filter = "All files (*.*)|*.*", string initialDirectory = null);

		/// <summary>
		/// Opens a dialog to open files
		/// </summary>
		/// <param name="filter"></param>
		/// <param name="initialDirectory">If no directory is specified the location of the assembly is used</param>
		/// <returns></returns>
		string[] OpenFilesDialog(string filter = "All files (*.*|*.*", string initialDirectory = null);

		/// <summary>
		/// Opens a dialog to open a XML-file
		/// </summary>
		/// <param name="initialDirectory">If no directory is specified the location of the assembly is used</param>
		/// <returns></returns>
		string OpenXMLFileDialog(string initialDirectory);

		string OpenXMLFileDialog();


		/// <summary>
		/// Opens a dialog to open XML-files
		/// </summary>
		/// <param name="initialDirectory">If no directory is specified the location of the assembly is used</param>
		/// <returns></returns>
		string[] OpenXMLFilesDialog(string initialDirectory = null);

		/// <summary>
		/// Opens a dialog to pick a folder, if no initialdirectory is specified the location of the assembly is used
		/// </summary>
		/// <param name="initialDirectory"></param>
		/// <returns></returns>
		string OpenFolderDialog(string initialDirectory = null);
		string OpenJsonFileDialog(string initialDirectory = null);
		string SaveToDialog(string initialDirectory = null, string filter = "All files (*.*|*.*");
		string SaveToXMLDialog(string initialDirectory = null);
		string SaveToVectoJobDialog(string initialDirectory = null);

		string OpenXMLAndVectoFileDialog(string initialDirectory = null);


		/// <summary>
		/// Displays a messagebox
		/// </summary>
		/// <param name="messageBoxText"></param>
		/// <param name="caption"></param>
		/// <param name="button"></param>
		/// <param name="icon"></param>
		/// <returns></returns>
		MessageBoxResult ShowMessageBox(string messageBoxText,
			string caption,
			MessageBoxButton button,
			MessageBoxImage icon);

		/// <summary>
		/// Displays a messagebox
		/// </summary>
		/// <param name="messageBoxText"></param>
		/// <param name="caption"></param>
		/// <returns></returns>
		MessageBoxResult ShowMessageBox(string messageBoxText, string caption);

		MessageBoxResult ShowErrorMessage(string errorMessage, string caption);
		MessageBoxResult ShowErrorMessage(string errorMessage);

	}
}
