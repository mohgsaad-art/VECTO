using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.Declaration;
using VECTO3GUI.Util;

namespace VECTO3GUI.ViewModel.Interfaces {
	public interface IVehicleViewModel : IComponentViewModel
	{
		IVehicleDeclarationInputData ModelData { get; }

		string Manufacturer { get; set; }
		string ManufacturerAddress { get; set; }
		string Model { get; set; }
		string VIN { get; set; }
		DateTime Date { get; set; }
		LegislativeClass LegislativeClass { get; set; }
		VehicleCategory VehicleCategory { get; set; }
		AxleConfiguration AxleConfiguration { get; set; }
		VehicleClass? VehicleClass { get; }
		Kilogram CurbMassChassis { get; set; }
		Kilogram GrossVehicleMass { get; set; }
		PerSecond IdlingSpeed { get; set; }
		RetarderType RetarderType { get; set; }
		double RetarderRatio { get; set; }
		AngledriveType AngledriveType { get; set; }
		string PTOTechnology { get; set; }	
		AllowedEntry<string>[] AllowedPTOTechnologies { get; } 
		AllowedEntry<LegislativeClass>[] AllowedLegislativeClasses { get; }
		AllowedEntry<VehicleCategory>[] AllowedVehicleCategories { get; }
		AllowedEntry<AxleConfiguration>[] AllowedAxleConfigurations { get; }
		AllowedEntry<RetarderType>[] AllowedRetarderTypes { get; }
		AllowedEntry<AngledriveType>[] AllowedAngledriveTypes { get; }
		bool HasDedicatedRetarder { get; }
		ObservableCollection<TorqueEntry> TorqueLimits { get; }

		ICommand AddTorqueLimit { get; }
		ICommand RemoveTorqueLimit { get; }
	}

	
}