using CefSharp.OffScreen;

namespace VECTOTrailer.Helper
{
	public class PdfCreator : IPdfCreator
	{
		#region Implementation of IPdfCreator

		private CefSettings _cefSettings;

		public PdfCreator()
		{
			_cefSettings = new CefSettings() {
				
			}
		}
		public bool CreatePDFFromXml(string pathToXml, string outputPath)
		{
			throw new System.NotImplementedException();
		}

		#endregion
	}

	public interface IPdfCreator
	{
		bool CreatePDFFromXml(string pathToXml, string outputPath);
	}
}