using System.Xml.Linq;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCore.InputData.FileIO.XML;
using TUGraz.VectoCore.Utils;
using VECTO3GUI2020.ViewModel.Implementation.Common;
using VECTO3GUI2020.ViewModel.Interfaces;
using VECTO3GUI2020.ViewModel.Interfaces.Document;
using VECTO3GUI2020.ViewModel.Interfaces.JobEdit;

namespace VECTO3GUI2020.ViewModel.Implementation.Document
{
    public class DeclarationJobViewModel : ViewModelBase, IJobViewModel
    {
        #region Implementation of IDocumentViewModel
        public string DocumentName { get; }

        public XmlDocumentType? DocumentType { get; }

		public string DocumentTypeName => DocumentType?.GetName();

		public DataSource DataSource => _jobEditViewModel.DataSource;


		private IJobEditViewModel _jobEditViewModel;
        public IJobEditViewModel JobEditViewModel { get => _jobEditViewModel; }
        public IEditViewModel EditViewModel
		{
			get => _jobEditViewModel as IEditViewModel;
		}

		public bool Selected
		{
			get => _selected && CanBeSimulated;
			set => SetProperty(ref _selected, value);
		}

		public bool CanBeSimulated
		{
			get => false;
			set => throw new System.NotImplementedException();
		}

		public IAdditionalJobInfoViewModel AdditionalJobInfoVm
		{
			get => throw new System.NotImplementedException();
			set => throw new System.NotImplementedException();
		}

		#endregion
        #region Members

        public IXMLInputDataReader _xMLInputDataReader;
		private bool _selected;

		#endregion


        public DeclarationJobViewModel(XmlDocumentType xmlDocumentType, string sourcefile, IXMLInputDataReader xMLInputDataReader,
            IJobEditViewModelFactory jobEditViewModelFactory)
        {

            _xMLInputDataReader = xMLInputDataReader;
            DocumentType = xmlDocumentType;
            

            var xmlInputDataProvider = _xMLInputDataReader.Create(sourcefile);
			_jobEditViewModel = jobEditViewModelFactory.CreateJobEditViewModel(xmlInputDataProvider);

			_xMLInputDataReader.Create(sourcefile);
			DocumentName = _jobEditViewModel.Name;
        }
	}
}
