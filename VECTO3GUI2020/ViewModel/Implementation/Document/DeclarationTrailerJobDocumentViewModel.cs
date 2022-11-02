using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCore.InputData.FileIO.XML;
using TUGraz.VectoCore.Utils;
using VECTO3GUI2020.ViewModel.Implementation.Common;
using VECTO3GUI2020.ViewModel.Interfaces;
using VECTO3GUI2020.ViewModel.Interfaces.Document;
using VECTO3GUI2020.ViewModel.Interfaces.JobEdit;

namespace VECTO3GUI2020.ViewModel.Implementation.Document
{
    public class DeclarationTrailerJobDocumentViewModel : IJobViewModel
    {
        private string _name = "";
        public string DocumentName { get => _name; }

        public XmlDocumentType? DocumentType { get; }

		public string DocumentTypeName => DocumentType?.GetName();

        public DataSource DataSource => throw new System.NotImplementedException();

		public string SourceFile { get; }

        public IJobEditViewModel EditViewModel { get => _jobEditViewModel; }

		public bool Selected
		{
			get => throw new System.NotImplementedException();
			set => throw new System.NotImplementedException();
		}

		public bool CanBeSimulated
		{
			get => throw new System.NotImplementedException();
			set => throw new System.NotImplementedException();
		}

		public IAdditionalJobInfoViewModel AdditionalJobInfoVm
		{
			get => throw new System.NotImplementedException();
			set => throw new System.NotImplementedException();
		}

		IEditViewModel IDocumentViewModel.EditViewModel => throw new System.NotImplementedException();

        private IXMLInputDataReader _xMLInputDataReader;

        private IJobEditViewModel _jobEditViewModel;


        public DeclarationTrailerJobDocumentViewModel(XmlDocumentType xmlDocumentType, string sourcefile, IXMLInputDataReader xMLInputDataReader,
           IJobEditViewModelFactory jobEditViewModelFactory)
        {
            
            DocumentType = xmlDocumentType;
            _xMLInputDataReader = xMLInputDataReader;
            
            SourceFile = sourcefile;


            var xml_input_data_provider = _xMLInputDataReader.Create(sourcefile);
            _jobEditViewModel = jobEditViewModelFactory.CreateJobEditViewModel(xml_input_data_provider);
        }
    }
}
