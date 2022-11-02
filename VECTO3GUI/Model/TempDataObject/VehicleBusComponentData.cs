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
using VECTO3GUI.ViewModel.Interfaces;

namespace VECTO3GUI.Model.TempDataObject
{
	public class VehicleBusComponentData : ICompleteVehicleBus, ITempDataObject<ICompleteVehicleBusViewModel>
	{
		protected const int NotSelected = -1;

		#region ICompleteVehicleBus Interface

		public string Manufacturer { get; set; }
		public string ManufacturerAddress { get; set; }
		public string Model { get; set; }
		public string VIN { get; set; }
		public DateTime Date { get; set; }
		public LegislativeClass LegislativeClass { get; set; }
		public RegistrationClass RegisteredClass { get; set; }
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
		public ConsumerTechnology DoorDriveTechnology { get; set; }
		public Dictionary<string, string> XmlNamesToPropertyMapping { get;  private set; }

		#endregion

		public VehicleBusComponentData(ICompleteVehicleBusViewModel viewModel, bool defaultValues)
		{
			if(defaultValues)
				ClearValues(viewModel);
			SetXmlNamesToPropertyMapping();
		}

		public VehicleBusComponentData(ICompleteVehicleBusViewModel viewModel)
		{
			SetCurrentValues(viewModel);
			SetXmlNamesToPropertyMapping();
		}

		public void UpdateCurrentValues(ICompleteVehicleBusViewModel viewModel)
		{
			SetCurrentValues(viewModel);
		}

		public void ResetToComponentValues(ICompleteVehicleBusViewModel viewModel)
		{
			viewModel.Manufacturer = Manufacturer;
			viewModel.ManufacturerAddress = ManufacturerAddress;
			viewModel.Model = Model;
			viewModel.VIN = VIN;
			viewModel.Date = Date;
			viewModel.LegislativeClass = LegislativeClass;
			viewModel.RegisteredClass = RegisteredClass;
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
			viewModel.DoorDriveTechnology = DoorDriveTechnology;
		}

		public void ClearValues(ICompleteVehicleBusViewModel viewModel)
		{
			viewModel.Manufacturer = default(string);
			viewModel.ManufacturerAddress = default(string);
			viewModel.Model = default(string);
			viewModel.VIN = default(string);
			viewModel.Date = default(DateTime);
			viewModel.LegislativeClass = (LegislativeClass)NotSelected;
			viewModel.RegisteredClass = (RegistrationClass)NotSelected;
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
			viewModel.DoorDriveTechnology = (ConsumerTechnology)NotSelected;
		}

		
		private void SetCurrentValues(ICompleteVehicleBusViewModel vehicleBus)
		{
			Manufacturer = vehicleBus.Manufacturer;
			ManufacturerAddress = vehicleBus.ManufacturerAddress;
			Model = vehicleBus.Model;
			VIN = vehicleBus.VIN;
			Date = DateTime.UtcNow;//Set DateTime UTC of current save
			LegislativeClass = vehicleBus.LegislativeClass;
			RegisteredClass = vehicleBus.RegisteredClass;
			VehicleCode = vehicleBus.VehicleCode;
			CurbMassChassis = vehicleBus.CurbMassChassis;
			TechnicalPermissibleMaximumLadenMass = vehicleBus.TechnicalPermissibleMaximumLadenMass;
			NgTankSystem = vehicleBus.NgTankSystem ;
			NumberOfPassengersLowerDeck = vehicleBus.NumberOfPassengersLowerDeck;
			NumberOfPassengersUpperDeck = vehicleBus.NumberOfPassengersUpperDeck;
			LowEntry = vehicleBus.LowEntry;
			HeightIntegratedBody = vehicleBus.HeightIntegratedBody;
			VehicleLength = vehicleBus.VehicleLength;
			VehicleWidth = vehicleBus.VehicleWidth;
			EntranceHeight = vehicleBus.EntranceHeight;
			DoorDriveTechnology = vehicleBus.DoorDriveTechnology;
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
				{XMLNames.Vehicle_RegisteredClass, nameof(RegisteredClass)},
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
				{XMLNames.BusAux_PneumaticSystem_DoorDriveTechnology, nameof(DoorDriveTechnology)}
			};
		}


	}
}
