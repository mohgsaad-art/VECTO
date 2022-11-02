using TUGraz.VectoCore.Utils;

namespace VECTO3GUI2020.ViewModel.Interfaces.Document
{
    public interface IDocumentViewModelFactory
    {
		/*
	public enum XmlDocumentType
	{
		DeclarationJobData = 1 << 1,
		PrimaryVehicleBusOutputData = 1 << 2,
		DeclarationComponentData = 1 << 3,
		EngineeringJobData = 1 << 4,
		EngineeringComponentData = 1 << 5,
		ManufacturerReport = 1 << 6,
		CustomerReport = 1 << 7,
		MonitoringReport = 1 << 8,
		VTPReport = 1 << 9,
		DeclarationTrailerJobData = 1 << 10,
	}

		*/
		IDocumentViewModel CreateDocumentViewModel(XmlDocumentType xmlDocumentType, string sourcefile);


    }
}
