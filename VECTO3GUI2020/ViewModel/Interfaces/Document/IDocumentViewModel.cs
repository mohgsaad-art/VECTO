using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCore.Utils;
using VECTO3GUI2020.ViewModel.Implementation.Common;
using VECTO3GUI2020.ViewModel.Interfaces.JobEdit;

namespace VECTO3GUI2020.ViewModel.Interfaces.Document
{
    public interface IDocumentViewModel
    {
        string DocumentName { get; }
        XmlDocumentType? DocumentType { get; }

        string DocumentTypeName { get; }
		DataSource DataSource { get; }
		IEditViewModel EditViewModel { get; }
		bool Selected { get; set; }

        bool CanBeSimulated { get; set; }
		IAdditionalJobInfoViewModel AdditionalJobInfoVm { get; set; }
	}
}
