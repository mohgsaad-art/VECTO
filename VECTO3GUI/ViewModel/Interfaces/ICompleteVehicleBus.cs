using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TUGraz.VectoCommon.BusAuxiliaries;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;

namespace VECTO3GUI.ViewModel.Interfaces
{
	public interface ICompleteVehicleBus
	{
		string Manufacturer { get; set; }
		string ManufacturerAddress { get; set; }
		string Model { get; set; }
		string VIN { get; set; }
		DateTime Date { get; set; }
		LegislativeClass LegislativeClass { get; set; }
		RegistrationClass RegisteredClass { get; set; }
		VehicleCode VehicleCode { get; set; }
		Kilogram CurbMassChassis { get; set; }
		Kilogram TechnicalPermissibleMaximumLadenMass { get; set; }
		TankSystem? NgTankSystem { get; set; }
		int NumberOfPassengersLowerDeck { get; set; }
		int NumberOfPassengersUpperDeck { get; set; }
		bool LowEntry { get; set; }
		Meter HeightIntegratedBody { get; set; }
		Meter VehicleLength { get; set; }
		Meter VehicleWidth { get; set; }
		Meter EntranceHeight { get; set; }

		#region CompleteBusDataProviderV26

		ConsumerTechnology DoorDriveTechnology { get; set; }
		
		#endregion

		Dictionary<string,string> XmlNamesToPropertyMapping { get; }
	}
}
