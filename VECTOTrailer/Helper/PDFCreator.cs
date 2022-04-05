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

		public PdfCreator()
		{
			_cefSettings = new CefSettings() {
				CachePath = "",
			};
		}

		public bool CreatePDFFromXml(string pathToXml, string outputPath)
		{
			var browser = new ChromiumWebBrowser(Path.GetFullPath(pathToXml));
			browser.WaitForInitialLoadAsync().Wait();
			var printTask = browser.PrintToPdfAsync(outputPath);
			printTask.Wait();
			return printTask.Result;
		}


		public bool Init()
		{
			try {
				_initialized = Cef.Initialize(_cefSettings); //Could be done async
				return _initialized;
			} catch (Exception ex) {
				Debug.WriteLine(ex.Message);
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
		bool Init();
		void ShutDown();

		bool Initialized { get; }
	}
}