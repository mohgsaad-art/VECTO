using System;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;

namespace VECTO3GUI2020.ViewModel.Interfaces.JobEdit.Vehicle.Components
{
    public interface ICommonComponentViewModel : IComponentInputData
    {
		new string Manufacturer { get; set; }
		new string Model { get; set; }

		new DateTime Date { get; set; }

		new string CertificationNumber { get; set; }
		new CertificationMethod CertificationMethod { get; set; }
		new bool SavedInDeclarationMode { get; set; }

		new DigestData DigestValue { get; set; }

		new DataSource DataSource { get; set; }

		new string AppVersion { get; }
	}

}
