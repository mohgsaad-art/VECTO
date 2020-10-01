using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TUGraz.VectoCommon.BusAuxiliaries;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;

namespace VECTOTrailer.ViewModel.Interfaces
{
	public interface ICompleteTrailer
	{
		string Manufacturer { get; set; }
		string ManufacturerAddress { get; set; }
		string Model { get; set; }
		string VIN { get; set; }
		DateTime Date { get; set; }
		LegislativeClass LegislativeClass { get; set; }
		/*RegistrationClass RegisteredClass { get; set; }
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
		
		#endregion*/

		#region CompleteBusDataProviderV27

		Meter ExternalLengthOfTheBody { get; set; }
		Meter ExternalWidthOfTheBody { get; set; }
		Meter ExternalHeightOfTheBody { get; set; }
		Meter TotalHeightOfTheTrailer { get; set; }
		Meter PositionXFirstAxle { get; set; }
		Meter PositionXLastAxle { get; set; }
		Meter InternalHeightOfTheBody { get; set; }
		CubicMeter CargoVolume { get; set; }
		int NumberOfAxles { get; set; }
		TypeTrailer TrailerType { get; set; }
		BodyWorkCode BodyCode { get; set; }
		AeroFeatureTechnologies Technologies { get; set; }
		Kilogram CurbMassTrailer { get; set; }
		Kilogram TPLMTotalTrailer { get; set; }
		Kilogram TPLMAxleAssembly { get; set; }

		#endregion

		Dictionary<string,string> XmlNamesToPropertyMapping { get; }
	}
}
