using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TUGraz.VectoCommon.BusAuxiliaries;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCommon.Utils;
using VECTOTrailer.ViewModel.Interfaces;

namespace VECTOTrailer.Model.TempDataObject
{
	public class TrailerComponentData : ICompleteTrailer, ITempDataObject<ICompleteTrailerViewModel>
	{
		protected const int NotSelected = -1;

		#region ICompleteVehicleBus Interface

		public string Manufacturer { get; set; }
		public string ManufacturerAddress { get; set; }
		public string Model { get; set; }
		public string VIN { get; set; }
		public DateTime Date { get; set; }
		public LegislativeClass LegislativeClass { get; set; }
		/*public RegistrationClass RegisteredClass { get; set; }
		public VehicleCode VehicleCode { get; set; }
		public Kilogram CurbMassChassis { get; set; }
		public Kilogram TechnicalPermissibleMaximumLadenMass { get; set; }
		public TankSystem? NgTankSystem { get; set; }
		public int NumberOfPassengersLowerDeck { get; set; }
		public int NumberOfPassengersUpperDeck { get; set; }
		public bool LowEntry { get; set; }
		public Meter HeightIntegratedBody { get; set; }
		public Meter VehicleLength { get; set; }
		public Meter VehicleWidth { get; set; }
		public Meter EntranceHeight { get; set; }
		public ConsumerTechnology DoorDriveTechnology { get; set; }*/
		public Meter ExternalLengthOfTheBody { get; set; }
		public Meter ExternalWidthOfTheBody { get; set; }
		public Meter ExternalHeightOfTheBody { get; set; }
		public Meter TotalHeightOfTheTrailer { get; set; }
		public Meter PositionXFirstAxle { get; set; }
		public Meter PositionXLastAxle { get; set; }
		public Meter InternalHeightOfTheBody { get; set; }
		public CubicMeter CargoVolume { get; set; }
		public int NumberOfAxles { get; set; }
		public TypeTrailer TrailerType { get; set; }
		public BodyWorkCode BodyCode { get; set; }
		public AeroFeatureTechnologies Technologies { get; set; }
		public Kilogram CurbMassTrailer { get; set; }
		public Kilogram TPLMTotalTrailer { get; set; }
		public Kilogram TPLMAxleAssembly { get; set; }
		public Dictionary<string, string> XmlNamesToPropertyMapping { get;  private set; }

		#endregion

		public TrailerComponentData(ICompleteTrailerViewModel viewModel, bool defaultValues)
		{
			if(defaultValues)
				ClearValues(viewModel);
			SetXmlNamesToPropertyMapping();
		}

		public TrailerComponentData(ICompleteTrailerViewModel viewModel)
		{
			SetCurrentValues(viewModel);
			SetXmlNamesToPropertyMapping();
		}

		public void UpdateCurrentValues(ICompleteTrailerViewModel viewModel)
		{
			SetCurrentValues(viewModel);
		}

		public void ResetToComponentValues(ICompleteTrailerViewModel viewModel)
		{
			viewModel.Manufacturer = Manufacturer;
			viewModel.ManufacturerAddress = ManufacturerAddress;
			viewModel.Model = Model;
			viewModel.VIN = VIN;
			viewModel.Date = Date;
			viewModel.LegislativeClass = LegislativeClass;
			/*viewModel.RegisteredClass = RegisteredClass;
			viewModel.VehicleCode = VehicleCode;
			viewModel.CurbMassChassis = CurbMassChassis;
			viewModel.TechnicalPermissibleMaximumLadenMass = TechnicalPermissibleMaximumLadenMass;
			viewModel.NgTankSystem = NgTankSystem;
			viewModel.NumberOfPassengersLowerDeck = NumberOfPassengersLowerDeck;
			viewModel.NumberOfPassengersUpperDeck = NumberOfPassengersUpperDeck;
			viewModel.LowEntry = LowEntry;
			viewModel.HeightIntegratedBody = HeightIntegratedBody;
			viewModel.VehicleLength = VehicleLength;
			viewModel.VehicleWidth = VehicleWidth;
			viewModel.EntranceHeight = EntranceHeight;
			viewModel.DoorDriveTechnology = DoorDriveTechnology;*/

			viewModel.ExternalHeightOfTheBody = ExternalHeightOfTheBody;
			viewModel.ExternalLengthOfTheBody = ExternalLengthOfTheBody;
			viewModel.ExternalWidthOfTheBody = ExternalWidthOfTheBody;
			viewModel.TotalHeightOfTheTrailer = TotalHeightOfTheTrailer;
			viewModel.PositionXFirstAxle = PositionXFirstAxle;
			viewModel.PositionXLastAxle = PositionXFirstAxle;
			viewModel.InternalHeightOfTheBody = InternalHeightOfTheBody;
			viewModel.CargoVolume = CargoVolume;
			viewModel.NumberOfAxles = NumberOfAxles;
			viewModel.TrailerType = TrailerType;
			viewModel.BodyCode = BodyCode;
			viewModel.Technologies = Technologies;
			viewModel.CurbMassTrailer = CurbMassTrailer;
			viewModel.TPLMTotalTrailer = TPLMTotalTrailer;
			viewModel.TPLMAxleAssembly = TPLMAxleAssembly;
		}

		public void ClearValues(ICompleteTrailerViewModel viewModel)
		{
			viewModel.Manufacturer = default(string);
			viewModel.ManufacturerAddress = default(string);
			viewModel.Model = default(string);
			viewModel.VIN = default(string);
			viewModel.Date = default(DateTime);
			viewModel.LegislativeClass = (LegislativeClass)NotSelected;
			/*viewModel.RegisteredClass = (RegistrationClass)NotSelected;
			viewModel.VehicleCode = (VehicleCode)NotSelected;
			viewModel.CurbMassChassis = default(Kilogram);
			viewModel.TechnicalPermissibleMaximumLadenMass = default(Kilogram);
			viewModel.NgTankSystem = (TankSystem)NotSelected;
			viewModel.NumberOfPassengersLowerDeck = default(int);
			viewModel.NumberOfPassengersUpperDeck = default(int);
			viewModel.LowEntry = LowEntry;
			viewModel.HeightIntegratedBody = default(Meter);
			viewModel.VehicleLength = default(Meter);
			viewModel.VehicleWidth = default(Meter);
			viewModel.EntranceHeight = default(Meter);
			viewModel.DoorDriveTechnology = (ConsumerTechnology)NotSelected;*/

			viewModel.ExternalHeightOfTheBody = default(Meter);
			viewModel.ExternalLengthOfTheBody = default(Meter);
			viewModel.ExternalWidthOfTheBody = default(Meter);
			viewModel.TotalHeightOfTheTrailer = default(Meter);
			viewModel.PositionXFirstAxle = default(Meter);
			viewModel.PositionXLastAxle = default(Meter);
			viewModel.InternalHeightOfTheBody = default(Meter);
			viewModel.CargoVolume = default(CubicMeter);
			viewModel.NumberOfAxles = default(int);
			viewModel.TrailerType = default(TypeTrailer);
			viewModel.BodyCode = default(BodyWorkCode);
			viewModel.Technologies = default(AeroFeatureTechnologies);
			viewModel.CurbMassTrailer = default(Kilogram);
			viewModel.TPLMTotalTrailer = default(Kilogram);
			viewModel.TPLMAxleAssembly = default(Kilogram);
		}

		
		private void SetCurrentValues(ICompleteTrailerViewModel trailer)
		{
			Manufacturer = trailer.Manufacturer;
			ManufacturerAddress = trailer.ManufacturerAddress;
			Model = trailer.Model;
			VIN = trailer.VIN;
			Date = DateTime.UtcNow;//Set DateTime UTC of current save
			LegislativeClass = trailer.LegislativeClass;
			/*RegisteredClass = trailer.RegisteredClass;
			VehicleCode = trailer.VehicleCode;
			CurbMassChassis = trailer.CurbMassChassis;
			TechnicalPermissibleMaximumLadenMass = trailer.TechnicalPermissibleMaximumLadenMass;
			NgTankSystem = trailer.NgTankSystem ;
			NumberOfPassengersLowerDeck = trailer.NumberOfPassengersLowerDeck;
			NumberOfPassengersUpperDeck = trailer.NumberOfPassengersUpperDeck;
			LowEntry = trailer.LowEntry;
			HeightIntegratedBody = trailer.HeightIntegratedBody;
			VehicleLength = trailer.VehicleLength;
			VehicleWidth = trailer.VehicleWidth;
			EntranceHeight = vehicleBus.EntranceHeight;
			DoorDriveTechnology = vehicleBus.DoorDriveTechnology;*/

			ExternalHeightOfTheBody = trailer.ExternalHeightOfTheBody;
			ExternalLengthOfTheBody = trailer.ExternalLengthOfTheBody;
			ExternalWidthOfTheBody = trailer.ExternalWidthOfTheBody;
			TotalHeightOfTheTrailer = trailer.TotalHeightOfTheTrailer;
			PositionXFirstAxle = trailer.PositionXFirstAxle;
			PositionXLastAxle = trailer.PositionXLastAxle;
			InternalHeightOfTheBody = trailer.InternalHeightOfTheBody;
			CargoVolume = trailer.CargoVolume;
			NumberOfAxles = trailer.NumberOfAxles;
			TrailerType = trailer.TrailerType;
			BodyCode = trailer.BodyCode;
			Technologies = trailer.Technologies;
			CurbMassTrailer = trailer.CurbMassTrailer;
			TPLMTotalTrailer = trailer.TPLMTotalTrailer;
			TPLMAxleAssembly = trailer.TPLMAxleAssembly;
		}

		private void SetXmlNamesToPropertyMapping()
		{
			XmlNamesToPropertyMapping = new Dictionary<string, string> {
				{XMLNames.Component_Manufacturer, nameof(Manufacturer)},
				{XMLNames.Component_ManufacturerAddress, nameof(ManufacturerAddress)},
				{XMLNames.Component_Model, nameof(Model)},
				{XMLNames.Vehicle_VIN, nameof(VIN)},
				{XMLNames.Component_Date, nameof(Date)},
				{XMLNames.Vehicle_LegislativeClass, nameof(LegislativeClass)},
				/*{XMLNames.Vehicle_RegisteredClass, nameof(RegisteredClass)},
				{XMLNames.Vehicle_VehicleCode, nameof(VehicleCode)},
				{XMLNames.Vehicle_CurbMassChassis, nameof(CurbMassChassis)},
				{XMLNames.TPMLM, nameof(TechnicalPermissibleMaximumLadenMass)},
				{XMLNames.Vehicle_NgTankSystem, nameof(NgTankSystem)},
				{XMLNames.Bus_LowerDeck, nameof(NumberOfPassengersLowerDeck)},
				{XMLNames.Bus_UpperDeck, nameof(NumberOfPassengersUpperDeck)},
				{XMLNames.Bus_LowEntry, nameof(LowEntry)},
				{XMLNames.Bus_HeighIntegratedBody, nameof(HeightIntegratedBody)},
				{XMLNames.Bus_VehicleLength, nameof(VehicleLength)},
				{XMLNames.Bus_VehicleWidth, nameof(VehicleWidth)},
				{XMLNames.Bus_EntranceHeight, nameof(EntranceHeight)},
				{XMLNames.BusAux_PneumaticSystem_DoorDriveTechnology, nameof(DoorDriveTechnology)}*/

				{XMLNames.Trailer_ExternalHeightBody, nameof(ExternalHeightOfTheBody)},
				{XMLNames.Trailer_ExternalLenghtBody, nameof(ExternalLengthOfTheBody)},
				{XMLNames.Trailer_ExternalWidthBody, nameof(ExternalWidthOfTheBody)},
				{XMLNames.Trailer_TotalHeightTrailer, nameof(TotalHeightOfTheTrailer)},
				{XMLNames.Trailer_PositionXFirstAxle, nameof(PositionXFirstAxle)},
				{XMLNames.Trailer_PositionXLastAxle, nameof(PositionXLastAxle)},
				{XMLNames.Trailer_InternalHeightBody, nameof(InternalHeightOfTheBody)},
				{XMLNames.Trailer_CargoVolume, nameof(CargoVolume)},
				{XMLNames.Trailer_AxleCount, nameof(NumberOfAxles)},
				{XMLNames.Trailer_TrailerType, nameof(TrailerType)},
				{XMLNames.Trailer_BodyCode, nameof(BodyCode)},
				{XMLNames.Trailer_AeroFeatureTechnologies, nameof(Technologies)},
				{XMLNames.Trailer_CurbMass, nameof(CurbMassTrailer)},
				{XMLNames.Trailer_TPMLMTotalTrailer, nameof(TPLMTotalTrailer)},
				{XMLNames.Trailer_TPLMAxleAssembly, nameof(TPLMAxleAssembly)},
			};
		}


	}
}
