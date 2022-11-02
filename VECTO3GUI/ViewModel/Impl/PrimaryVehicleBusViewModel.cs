using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ninject;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.DataProvider;
using TUGraz.VectoCore.Utils;
using VECTO3GUI.Util;
using VECTO3GUI.ViewModel.Adapter;
using VECTO3GUI.ViewModel.Impl;
using VECTO3GUI.ViewModel.Interfaces;

namespace VECTO3GUI.ViewModel.Impl
{
	public class PrimaryVehicleBusViewModel : AbstractViewModel, IPrimaryVehicleBusViewModel
	{

		#region Members

		private string _manufacturer;
		private string _manufacturerAddress;
		private string _model;
		private string _vin;
		private DateTime _date;
		private VehicleCategory _vehicleCategory;
		private AxleConfiguration _axleConfiguration;
		private bool _articulated;
		private Kilogram _techniclaPermisibleMaximumLadenMass;
		private PerSecond _idlingSpeed;
		private RetarderType _retarderType;
		private double _retarderRatio;
		private AngledriveType _angledriveType;
		private bool _zeroEmissionVehicle;

		private bool _engineStopStart;
		private EcoRollType _ecoRollType;
		private PredictiveCruiseControlType _predictiveCruiseControl;

		private IList<ITorqueLimitInputData> _torqueLimits;

		#endregion

		#region Properties

		[Inject]
		public IAdapterFactory AdapterFactory { set; protected get; }

		public IVehicleDeclarationInputData Vehicle { get; set; }
		public DigestData ResultDataHash { get; set; }
		public IResultsInputData ResultsInputData { get; set; }
		public IApplicationInformation ApplicationInformation { get; set; }
		public DigestData ManufacturerHash { get; set; }
		
		public string Manufacturer
		{
			get { return _manufacturer; }
			set { SetProperty(ref _manufacturer, value); }
		}
		public string ManufacturerAddress
		{
			get { return _manufacturerAddress; }
			set { SetProperty(ref _manufacturerAddress, value); }
		}
		public string Model
		{
			get { return _model; }
			set { SetProperty(ref _model, value); }
		}
		public string VIN
		{
			get { return _vin; }
			set { SetProperty(ref _vin, value); }
		}
		public DateTime Date
		{
			get { return _date; }
			set { SetProperty(ref _date, value); }
		}
		public VehicleCategory VehicleCategory
		{
			get { return _vehicleCategory; }
			set { SetProperty(ref _vehicleCategory, value); }
		}
		public AxleConfiguration AxleConfiguration
		{
			get { return _axleConfiguration; }
			set { SetProperty(ref _axleConfiguration, value); }
		}
		public bool Articulated
		{
			get { return _articulated; }
			set { SetProperty(ref _articulated, value); }
		}
		public Kilogram TechnicalPermissibleMaximumLadenMass
		{
			get { return _techniclaPermisibleMaximumLadenMass; }
			set { SetProperty(ref _techniclaPermisibleMaximumLadenMass, value); }
		}
		public PerSecond IdlingSpeed
		{
			get { return _idlingSpeed; }
			set { SetProperty(ref _idlingSpeed, value); }
		}
		public RetarderType RetarderType
		{
			get { return _retarderType; }
			set { SetProperty(ref _retarderType, value); }
		}
		public double RetarderRatio
		{
			get { return _retarderRatio; }
			set { SetProperty(ref _retarderRatio, value); }
		}
		public AngledriveType AngledriveType
		{
			get { return _angledriveType; }
			set { SetProperty(ref _angledriveType, value); }
		}
		public bool ZeroEmissionVehicle
		{
			get { return _zeroEmissionVehicle; }
			set { SetProperty(ref _zeroEmissionVehicle, value); }
		}

		public bool EngineStopStart
		{
			get { return _engineStopStart;}
			set { SetProperty(ref _engineStopStart, value); }
		}
		public EcoRollType EcoRoll
		{
			get { return _ecoRollType;}
			set { SetProperty(ref _ecoRollType, value); }
		}
		public PredictiveCruiseControlType PredictiveCruiseControl
		{
			get { return _predictiveCruiseControl;}
			set { SetProperty(ref _predictiveCruiseControl, value); }
		}


		public IList<ITorqueLimitInputData> TorqueLimits
		{
			get { return _torqueLimits; }
			set { SetProperty(ref _torqueLimits, value); }
		}

		public AllowedEntry<VehicleCategory>[] AllowedVehicleCategories { get; set; }
		public AllowedEntry<AxleConfiguration>[] AllowedAxleConfigurations { get; set; }
		public AllowedEntry<RetarderType>[] AllowedRetarderTypes { get; set; }
		public AllowedEntry<AngledriveType>[] AllowedAngledriveTypes { get; set; }
		public AllowedEntry<EcoRollType>[] AllowedEcoRollTypes { get; set; }
		public AllowedEntry<PredictiveCruiseControlType>[] AllowedPredictiveCruiseControl { get; set; }

		#endregion



		public PrimaryVehicleBusViewModel()
		{
			
		}






		private void SetVehicleData(IVehicleDeclarationInputData vehicle)
		{
			Manufacturer = vehicle.Manufacturer;
			Model = vehicle.Model;
			ManufacturerAddress = vehicle.ManufacturerAddress;
			VIN = vehicle.VIN;
			Date = vehicle.Date;
			VehicleCategory = vehicle.VehicleCategory;
			AxleConfiguration = vehicle.AxleConfiguration;
			Articulated = vehicle.Articulated;
			TechnicalPermissibleMaximumLadenMass = vehicle.GrossVehicleMassRating;
			IdlingSpeed = vehicle.EngineIdleSpeed;

			RetarderType = ((XMLDeclarationPrimaryVehicleBusDataProviderV01)vehicle).RetarderType;
			RetarderRatio = ((XMLDeclarationPrimaryVehicleBusDataProviderV01)vehicle).RetarderRatio;
			AngledriveType = ((XMLDeclarationPrimaryVehicleBusDataProviderV01)vehicle).AngledriveType;

			ZeroEmissionVehicle = vehicle.ZeroEmissionVehicle;
			EngineStopStart = vehicle.ADAS.EngineStopStart;
			EcoRoll = vehicle.ADAS.EcoRoll;
			PredictiveCruiseControl = vehicle.ADAS.PredictiveCruiseControl;

			TorqueLimits = vehicle.TorqueLimits;
			
			SetAllowedEntries();
		}


		private void SetAllowedEntries()
		{
			AllowedVehicleCategories = Enum.GetValues(typeof(VehicleCategory)).Cast<VehicleCategory>()
				.Select(vc => AllowedEntry.Create(vc, vc.GetLabel())).ToArray();

			AllowedAxleConfigurations = Enum.GetValues(typeof(AxleConfiguration)).Cast<AxleConfiguration>()
				.Select(ac => AllowedEntry.Create(ac, ac.GetName())).ToArray();

			AllowedRetarderTypes = Enum.GetValues(typeof(RetarderType)).Cast<RetarderType>()
				.Select(rt => AllowedEntry.Create(rt, rt.GetLabel())).ToArray();

			AllowedAngledriveTypes = Enum.GetValues(typeof(AngledriveType)).Cast<AngledriveType>()
				.Select(at => AllowedEntry.Create(at, at.GetLabel())).ToArray();

			AllowedEcoRollTypes = Enum.GetValues(typeof(EcoRollType)).Cast<EcoRollType>()
				.Select(ert => AllowedEntry.Create(ert, ert.GetName())).ToArray();

			AllowedPredictiveCruiseControl = Enum.GetValues(typeof(PredictiveCruiseControlType))
				.Cast<PredictiveCruiseControlType>()
				.Select(pcc => AllowedEntry.Create(pcc, pcc.GetName())).ToArray();
		}


		protected override void InputDataChanged()
		{
			var inputData = JobViewModel.InputDataProvider as IPrimaryVehicleInformationInputDataProvider;
			SetVehicleData(inputData?.Vehicle);
		}


	}
}
