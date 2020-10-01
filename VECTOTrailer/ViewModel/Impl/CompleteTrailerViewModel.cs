using System;
using System.Collections.Generic;
using System.Linq;
using Castle.Core.Internal;
using TUGraz.VectoCommon.BusAuxiliaries;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using VECTOTrailer.Model.TempDataObject;
using VECTOTrailer.Util;
using VECTOTrailer.ViewModel.Interfaces;

namespace VECTOTrailer.ViewModel.Impl
{
	public class CompleteTrailerViewModel : AbstractComponentViewModel, ICompleteTrailerViewModel
	{
		#region Members

		private IVehicleDeclarationInputData _vehicle;
		private TrailerComponentData _componentData;

		private string _manufacturer;
		private string _manufacturerAddress;
		private string _model;
		private string _vin;
		private DateTime _date;
		private LegislativeClass _legislativeClass;
		private RegistrationClass _registeredClass;
		private VehicleCode _vehicleCode;
		private Kilogram _curbMassChassis;
		private Kilogram _technicalPermissibleMaximumLadenMass;
		private TankSystem? _ngTankSystem;
		private int _numberOfPassengersLowerDeck;
		private int _numberOfPassengersUpperDeck;
		private bool _lowEntry;
		private Meter _heightIntegratedBody;
		private Meter _vehicleLength;
		private Meter _vehicleWidth;
		private Meter _entranceHeight;
		private ConsumerTechnology _doorDriveTechnology;
		private Meter _externalLengthOfTheBody;
		private Meter _externalWidthOfTheBody;
		private Meter _externalHeightOfTheBody;
		private Meter _totalHeightOfTheTrailer;
		private Meter _positionXFirstAxle;
		private Meter _positionXLastAxle;
		private Meter _internalHeightOfTheBody;
		private CubicMeter _cargoVolume;
		private int _numberOfAxles;
		private TypeTrailer _trailerType;
		private BodyWorkCode _bodyCode;
		private AeroFeatureTechnologies _technologies;
		private Kilogram _curbMassTrailer;
		private Kilogram _tPLMTotalTrailer;
		private Kilogram _tPLMAxleAssembly;


		#endregion

		#region ICompleteTrailerViewModel

		public string Manufacturer
		{
			get { return _manufacturer; }
			set
			{
				if (!SetProperty(ref _manufacturer, value))
					return;
				IsDataChanged(_manufacturer, _componentData);
			}
		}
		public string ManufacturerAddress
		{
			get { return _manufacturerAddress; }
			set
			{
				if (!SetProperty(ref _manufacturerAddress, value))
					return;
				IsDataChanged(_manufacturerAddress, _componentData);
			}
		}
		public string Model
		{
			get { return _model; }
			set
			{
				if (!SetProperty(ref _model, value))
					return;

				IsDataChanged(_model, _componentData);
			}
		}
		public string VIN
		{
			get { return _vin; }
			set
			{
				if (!SetProperty(ref _vin, value))
					return;
				IsDataChanged(_vin, _componentData);
			}
		}
		public DateTime Date
		{
			get { return _date; }
			set
			{
				if (!SetProperty(ref _date, value))
					return;
				IsDataChanged(_date, _componentData);
			}
		}
		public LegislativeClass LegislativeClass
		{
			get { return _legislativeClass; }
			set
			{
				if (!SetProperty(ref _legislativeClass, value))
					return;
				IsDataChanged(_legislativeClass, _componentData);
			}
		}
		public RegistrationClass RegisteredClass
		{
			get { return _registeredClass; }
			set
			{
				if (!SetProperty(ref _registeredClass, value))
					return;
				IsDataChanged(_registeredClass, _componentData);
			}
		}
		public VehicleCode VehicleCode
		{
			get { return _vehicleCode; }
			set
			{
				if (!SetProperty(ref _vehicleCode, value))
					return;
				IsDataChanged(_vehicleCode, _componentData);
			}
		}
		public Kilogram CurbMassChassis
		{
			get { return _curbMassChassis; }
			set
			{
				if (!SetProperty(ref _curbMassChassis, value))
					return;
				IsDataChanged(_curbMassChassis, _componentData);
			}
		}
		public Kilogram TechnicalPermissibleMaximumLadenMass
		{
			get { return _technicalPermissibleMaximumLadenMass; }
			set
			{
				if (!SetProperty(ref _technicalPermissibleMaximumLadenMass, value))
					return;
				IsDataChanged(_technicalPermissibleMaximumLadenMass, _componentData);
			}
		}
		public TankSystem? NgTankSystem
		{
			get { return _ngTankSystem; }
			set
			{
				if (!SetProperty(ref _ngTankSystem, value))
					return;
				IsDataChanged(_ngTankSystem, _componentData);
			}
		}
		public int NumberOfPassengersLowerDeck
		{
			get { return _numberOfPassengersLowerDeck; }
			set
			{
				if (!SetProperty(ref _numberOfPassengersLowerDeck, value))
					return;
				IsDataChanged(_numberOfPassengersLowerDeck, _componentData);
			}
		}
		public int NumberOfPassengersUpperDeck
		{
			get { return _numberOfPassengersUpperDeck; }
			set
			{
				if (!SetProperty(ref _numberOfPassengersUpperDeck, value))
					return;
				IsDataChanged(_numberOfPassengersUpperDeck, _componentData);
			}
		}

		public bool LowEntry
		{
			get { return _lowEntry; }
			set
			{
				if (!SetProperty(ref _lowEntry, value))
					return;
				IsDataChanged(_lowEntry, _componentData);
			}
		}

		public Meter HeightIntegratedBody
		{
			get { return _heightIntegratedBody; }
			set
			{
				if (!SetProperty(ref _heightIntegratedBody, value))
					return;
				IsDataChanged(_heightIntegratedBody, _componentData);
			}
		}
		public Meter VehicleLength
		{
			get { return _vehicleLength; }
			set
			{
				if (!SetProperty(ref _vehicleLength, value))
					return;
				IsDataChanged(_vehicleLength, _componentData);
			}
		}
		public Meter VehicleWidth
		{
			get { return _vehicleWidth; }
			set
			{
				if (!SetProperty(ref _vehicleWidth, value))
					return;
				IsDataChanged(_vehicleWidth, _componentData);
			}
		}
		public Meter EntranceHeight
		{
			get { return _entranceHeight; }
			set
			{
				if (!SetProperty(ref _entranceHeight, value))
					return;
				IsDataChanged(_entranceHeight, _componentData);
			}
		}
		public ConsumerTechnology DoorDriveTechnology
		{
			get { return _doorDriveTechnology; }
			set
			{
				if (!SetProperty(ref _doorDriveTechnology, value))
					return;
				IsDataChanged(_doorDriveTechnology, _componentData);
			}
		}

		public Meter ExternalLengthOfTheBody
		{
			get { return _externalLengthOfTheBody; }
			set
			{
				if (!SetProperty(ref _externalLengthOfTheBody, value))
					return;
				IsDataChanged(_externalLengthOfTheBody, _componentData);
			}
		}

		public Meter ExternalWidthOfTheBody
		{
			get { return _externalWidthOfTheBody; }
			set
			{
				if (!SetProperty(ref _externalWidthOfTheBody, value))
					return;
				IsDataChanged(_externalWidthOfTheBody, _componentData);
			}
		}

		public Meter ExternalHeightOfTheBody
		{
			get { return _externalHeightOfTheBody; }
			set
			{
				if (!SetProperty(ref _externalHeightOfTheBody, value))
					return;
				IsDataChanged(_externalHeightOfTheBody, _componentData);
			}
		}

		public Meter TotalHeightOfTheTrailer
		{
			get { return _totalHeightOfTheTrailer; }
			set
			{
				if (!SetProperty(ref _totalHeightOfTheTrailer, value))
					return;
				IsDataChanged(_totalHeightOfTheTrailer, _componentData);
			}
		}

		public Meter PositionXFirstAxle
		{
			get { return _positionXFirstAxle; }
			set
			{
				if (!SetProperty(ref _positionXFirstAxle, value))
					return;
				IsDataChanged(_positionXFirstAxle, _componentData);
			}
		}

		public Meter PositionXLastAxle
		{
			get { return _positionXLastAxle; }
			set
			{
				if (!SetProperty(ref _positionXLastAxle, value))
					return;
				IsDataChanged(_positionXLastAxle, _componentData);
			}
		}

		public Meter InternalHeightOfTheBody
		{
			get { return _internalHeightOfTheBody; }
			set
			{
				if (!SetProperty(ref _internalHeightOfTheBody, value))
					return;
				IsDataChanged(_internalHeightOfTheBody, _componentData);
			}
		}

		public CubicMeter CargoVolume
		{
			get { return _cargoVolume; }
			set
			{
				if (!SetProperty(ref _cargoVolume, value))
					return;
				IsDataChanged(_cargoVolume, _componentData);
			}
		}

		public int NumberOfAxles
		{
			get { return _numberOfAxles; }
			set
			{
				if (!SetProperty(ref _numberOfAxles, value))
					return;
				IsDataChanged(_numberOfAxles, _componentData);
			}
		}

		public TypeTrailer TrailerType
		{
			get { return _trailerType; }
			set
			{
				if (!SetProperty(ref _trailerType, value))
					return;
				IsDataChanged(_trailerType, _componentData);
			}
		}

		public BodyWorkCode BodyCode
		{
			get { return _bodyCode; }
			set
			{
				if (!SetProperty(ref _bodyCode, value))
					return;
				IsDataChanged(_bodyCode, _componentData);
			}
		}
		public AeroFeatureTechnologies Technologies
		{
			get { return _technologies; }
			set
			{
				if (!SetProperty(ref _technologies, value))
					return;
				IsDataChanged(_technologies, _componentData);
			}
		}

		public Kilogram CurbMassTrailer
		{
			get { return _curbMassTrailer; }
			set
			{
				if (!SetProperty(ref _curbMassTrailer, value))
					return;
				IsDataChanged(_curbMassTrailer, _componentData);
			}
		}
		public Kilogram TPLMTotalTrailer
		{
			get { return _tPLMTotalTrailer; }
			set
			{
				if (!SetProperty(ref _tPLMTotalTrailer, value))
					return;
				IsDataChanged(_tPLMTotalTrailer, _componentData);
			}
		}
		public Kilogram TPLMAxleAssembly
		{
			get { return _tPLMAxleAssembly; }
			set
			{
				if (!SetProperty(ref _tPLMAxleAssembly, value))
					return;
				IsDataChanged(_tPLMAxleAssembly, _componentData);
			}
		}

		public Dictionary<string, string> XmlNamesToPropertyMapping { get; private set; }

		public AllowedEntry<LegislativeClass>[] AllowedLegislativeClasses { get; private set; }
		public AllowedEntry<TypeTrailer>[] AllowedTrailerTypes { get; private set; }
		public AllowedEntry<BodyWorkCode>[] AllowedBodyCodes { get; private set; }
		public AllowedEntry<AeroFeatureTechnologies>[] AllowedTechnologies { get; private set; }

		#endregion


		#region Set XML Data

		protected override void InputDataChanged()
		{
			var inputData = JobViewModel.InputDataProvider as IDeclarationInputDataProvider;
			_vehicle = inputData?.JobInputData.Vehicle;
			SetVehicleValues(_vehicle);
			SetAllowedEntries();
			XmlNamesToPropertyMapping = _componentData.XmlNamesToPropertyMapping;

		}

		private void SetVehicleValues(IVehicleDeclarationInputData vehicle)
		{
			if (vehicle == null)
			{
				_componentData = new TrailerComponentData(this, true);
			}
			else
			{
				Manufacturer = vehicle.Manufacturer;
				ManufacturerAddress = vehicle.ManufacturerAddress;
				Model = vehicle.Model;
				VIN = vehicle.VIN;
				Date = vehicle.Date;
				LegislativeClass = vehicle.LegislativeClass;
				RegisteredClass = vehicle.RegisteredClass;
				VehicleCode = vehicle.VehicleCode;
				CurbMassChassis = vehicle.CurbMassChassis;
				TechnicalPermissibleMaximumLadenMass = vehicle.GrossVehicleMassRating;
				NgTankSystem = vehicle.TankSystem;
				NumberOfPassengersLowerDeck = vehicle.NumberOfPassengersLowerDeck;
				NumberOfPassengersUpperDeck = vehicle.NumberOfPassengersUpperDeck;
				LowEntry = vehicle.LowEntry;
				HeightIntegratedBody = vehicle.Height;
				VehicleLength = vehicle.Length;
				VehicleWidth = vehicle.Width;
				EntranceHeight = vehicle.EntranceHeight;
				DoorDriveTechnology = vehicle.DoorDriveTechnology;
				_componentData = new TrailerComponentData(this);
			}

			ClearChangedProperties();
		}

		private void SetAllowedEntries()
		{

			AllowedLegislativeClasses = new[] { LegislativeClass.M3 }
				.Select(lc => AllowedEntry.Create(lc, lc.GetLabel())).ToArray();

			AllowedTrailerTypes = EnumHelper.GetValues<TypeTrailer>().Where(x => x != TypeTrailer.Unknown)
				.Select(vc => AllowedEntry.Create(vc, vc.GetLabel())).ToArray();

			AllowedBodyCodes = EnumHelper.GetValues<BodyWorkCode>().Where(x => x != BodyWorkCode.Unknown)
				.Select(vc => AllowedEntry.Create(vc, vc.GetLabel())).ToArray();

			AllowedTechnologies = EnumHelper.GetValues<AeroFeatureTechnologies>().Where(x => x != AeroFeatureTechnologies.Unknown)
				.Select(vc => AllowedEntry.Create(vc, vc.GetLabel())).ToArray();
		}

		#endregion

		public override void ResetComponentData()
		{
			_componentData.ResetToComponentValues(this);
		}

		public override object CommitComponentData()
		{
			_componentData.UpdateCurrentValues(this);
			ClearChangedProperties();
			return _componentData;
		}

		public override void ShowValidationErrors(Dictionary<string, string> errors)
		{
			if (errors.IsNullOrEmpty())
				return;

			foreach (var error in errors)
			{
				string propertyName;
				if (XmlNamesToPropertyMapping.TryGetValue(error.Key, out propertyName))
					AddPropertyError(propertyName, error.Value);
			}
		}

		public override void RemoveValidationErrors(Dictionary<string, string> errors)
		{
			if (errors.IsNullOrEmpty())
				return;

			foreach (var error in errors)
			{
				string propertyName;
				if (XmlNamesToPropertyMapping.TryGetValue(error.Key, out propertyName))
					RemovePropertyError(propertyName);
			}
		}

	}

}
