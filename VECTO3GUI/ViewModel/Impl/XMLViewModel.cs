using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Utils;

namespace VECTO3GUI.ViewModel.Impl
{
	public class XMLViewModel :  ObservableObject
	{
		#region Constant

		private const string XMLSyntaxHighlight = "XML";

		#endregion

		#region Members

		private string _textContent;
		private IHighlightingDefinition _syntaxHighlightType;
		private bool _showLineNumbers;

		private string _filePath;

		#endregion

		#region Properties
		
		public string TextContent
		{
			get { return _textContent; }
			set { SetProperty(ref _textContent, value); }
		}

		public IHighlightingDefinition SyntaxHighlightType
		{
			get { return _syntaxHighlightType; }
			set { SetProperty(ref _syntaxHighlightType, value); }
		}

		public bool ShowLineNumbers
		{
			get { return _showLineNumbers; }
			set { SetProperty(ref _showLineNumbers, value); }
		}

		public string FileName { get; private set; }

		#endregion
		
		public XMLViewModel(string filePath)
		{
			_filePath = filePath;
			InitXMLViewer();
		}
		
		private void InitXMLViewer()
		{
			SyntaxHighlightType = HighlightingManager.Instance.GetDefinition(XMLSyntaxHighlight);
			ShowLineNumbers = true;
			ReadFile();
		}

		private void ReadFile()
		{
			if (!File.Exists(_filePath))
				TextContent = null;

			FileName = Path.GetFileName(_filePath);

			using (var fileStream = new FileStream(_filePath, FileMode.Open, FileAccess.Read, FileShare.Read)) {
				TextContent =  FileReader.ReadFileContent(fileStream, Encoding.UTF8);
			}
		}

		public ICommand CloseWindowCommand
		{
			get { return null; }
		}
	}
}
