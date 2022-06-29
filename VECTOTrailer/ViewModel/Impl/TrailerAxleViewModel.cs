using System.Xml.Linq;

namespace VECTOTrailer.ViewModel.Impl
{

	public class TrailerAxleViewModel : AbstractComponentViewModel
	{

		private bool _twinTyres;
		public bool TwinTyres
		{
			get => _twinTyres;
			set => SetProperty(ref _twinTyres, value);
		}

		private bool _liftable;
		public bool Liftable
		{
			get => _liftable;
			set => SetProperty(ref _liftable, value);
		}

		private bool _steered;
		public bool Steered
		{
			get => _steered;
			set => SetProperty(ref _steered, value);
		}

		private string _tyreInfo;
		public string TyreInfo
		{
			get => _tyreInfo;
			set => SetProperty(ref _tyreInfo, value);
		}

		private string _tyreXmlPath;
		public string TyreXmlPath
		{
			get => _tyreXmlPath;
			set => SetProperty(ref _tyreXmlPath, value);
		}

		private XDocument _sourceDoc;
		/// <summary>
		/// Only used for tyres that are embedded in the trailer xml
		/// </summary>
		public XDocument SourceDoc
		{
			get => _sourceDoc;
			set => SetProperty(ref _sourceDoc, value);
		}
	}
}