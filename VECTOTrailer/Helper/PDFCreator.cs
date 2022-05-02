using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;
using CefSharp;
using CefSharp.OffScreen;

namespace VECTOTrailer.Helper
{
	public class PdfCreator : IPdfCreator
	{
		private CefSettings _cefSettings;
		private bool _initialized;

		public bool Initialized => _initialized;
		private bool _initError = false;

		private string _error = null;
		public PdfCreator()
		{
			try {
				CefRuntime.LoadCefSharpCoreRuntimeAnyCpu();
				CefRuntime.SubscribeAnyCpuAssemblyResolver();
				CreateSettings();
			}
			catch (Exception ex)
			{
				Debug.WriteLine("Cef Settings could not be created, possibly due to missing c++ redistributable");
				_error = ex.Message;
				_initError = true;
			}

		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public void CreateSettings()
		{
			_cefSettings = new CefSettings()
			{
				CachePath = "",
				LogFile = LogFileHelper.GetFileNameWithBasePath("cefSharpDebug.log"),
				LogSeverity = LogSeverity.Disable,
			};

		}


		public bool CreatePDFFromXml(string pathToXml, string outputPath)
		{
			if (!_initialized) {
				return false;
			}
			var browser = new ChromiumWebBrowser(Path.GetFullPath(pathToXml));
			browser.WaitForInitialLoadAsync().Wait();
			var printTask = browser.PrintToPdfAsync(outputPath, new PdfPrintSettings() {
				HeaderFooterEnabled = true,
			});
			printTask.Wait();
			return printTask.Result;
		}


		public bool Init(out string error)
		{
			error = null;
			if (!_initError) {
				try
				{
					_initialized = Cef.Initialize(_cefSettings, performDependencyCheck:true ); //Could be done async
					return _initialized;
				}
				catch (Exception ex)
				{
					Debug.WriteLine(ex.Message);
					error = $"{ex.Message} \n {ex.InnerException?.Message}";
					return false;
				}
			} else {
				_initialized = false;
				error = _error;
				return false;
			}
			
		}

		public void ShutDown()
		{
			{
				if (_initialized) {
					Cef.Shutdown();
				}
			}
		}
	}

	public interface IPdfCreator
	{
		bool CreatePDFFromXml(string pathToXml, string outputPath);
		bool Init(out string error);
		void ShutDown();

		bool Initialized { get; }
	}
}