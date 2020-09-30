using System;

namespace VECTOTrailer.ViewModel.Interfaces {
	public interface ICommonComponentParameters
	{
		string Manufacturer { get; set; }
		string Model { get; set; }
		string CertificationNumber { get; set; }
		DateTime? Date { get; set; }
	}
}