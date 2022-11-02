using System.Collections.ObjectModel;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using VECTO3GUI.Util;

namespace VECTO3GUI.ViewModel.Interfaces
{
	public interface IEngineViewModel : IComponentViewModel, ICommonComponentParameters
	{
		IEngineDeclarationInputData ModelData { get; }

		
		CubicMeter Displacement { get; set; }
		PerSecond IdlingSpeed { get; set; }
		PerSecond RatedSpeed { get; set; }
		Watt RatedPower { get; set; }
		NewtonMeter MaxEngineTorque { get; set; }
		double WHTCUrban { get; set; }
		double WHTCRural { get; set; }
		double WHTCMotorway { get; set; }
		double BFColdHot { get; set; }
		double CFRegPer { get; set; }
		double CFNCV { get; set; }
		FuelType FuelType { get; set; }
		AllowedEntry<FuelType>[] AllowedFuelTypes { get; }
		ObservableCollection<FuelConsumptionEntry> FCMap { get; }
		ObservableCollection<FullLoadEntry> FullLoadCurve { get; }
		double EngineeringCF { get; set; }
	}
}
