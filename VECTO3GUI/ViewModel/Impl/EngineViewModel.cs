using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.Reader;
using TUGraz.VectoCore.InputData.Reader.ComponentData;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Engine;
using TUGraz.VectoCore.Utils;
using VECTO3GUI.Util;
using VECTO3GUI.ViewModel.Adapter.Declaration;
using VECTO3GUI.ViewModel.Interfaces;

namespace VECTO3GUI.ViewModel.Impl
{
	public class EngineViewModel : AbstractComponentViewModel, IEngineViewModel
	{
		private string _manufacturer;
		private string _model;
		private string _certificationNumber;
		private DateTime? _date;
		private CubicMeter _displacement;
		private PerSecond _idlingSpeed;
		private PerSecond _ratedSpeed;
		private Watt _ratedPower;
		private NewtonMeter _maxEngineTorque;
		private double _whtcUrban;
		private double _whctRural;
		private double _whtcMotorway;
		private double _bfColdHot;
		private double _cfRegPer;
		private double _cfncv;
		private FuelType _fuelType;
		private double _engineeringCf;

		public EngineViewModel()
		{
			FCMap = new ObservableCollection<FuelConsumptionEntry>();
			FullLoadCurve = new ObservableCollection<FullLoadEntry>();
		}

		#region Implementation of ICommonComponentParameters

		public virtual string Manufacturer
		{
			get { return _manufacturer; }
			set { SetProperty(ref _manufacturer, value); }
		}

		public virtual string Model
		{
			get { return _model; }
			set { SetProperty(ref _model, value); }
		}

		public virtual string CertificationNumber
		{
			get { return _certificationNumber; }
			set { SetProperty(ref _certificationNumber, value); }
		}

		public virtual DateTime? Date
		{
			get { return _date; }
			set { SetProperty(ref _date, value); }
		}

		#endregion



		#region Implementation of IEditComponentEngineViewModel

		public CubicMeter Displacement
		{
			get { return _displacement; }
			set { SetProperty(ref _displacement, value); }
		}

		public PerSecond IdlingSpeed
		{
			get { return _idlingSpeed; }
			set { SetProperty(ref _idlingSpeed, value); }
		}

		public PerSecond RatedSpeed
		{
			get { return _ratedSpeed; }
			set { SetProperty(ref _ratedSpeed, value); }
		}

		public Watt RatedPower
		{
			get { return _ratedPower; }
			set { SetProperty(ref _ratedPower, value); }
		}

		public NewtonMeter MaxEngineTorque
		{
			get { return _maxEngineTorque; }
			set { SetProperty(ref _maxEngineTorque, value); }
		}

		public double WHTCUrban
		{
			get { return _whtcUrban; }
			set { SetProperty(ref _whtcUrban, value); }
		}

		public double WHTCRural
		{
			get { return _whctRural; }
			set { SetProperty(ref _whctRural, value); }
		}

		public double WHTCMotorway
		{
			get { return _whtcMotorway; }
			set { SetProperty(ref _whtcMotorway, value); }
		}

		public double BFColdHot
		{
			get { return _bfColdHot; }
			set { SetProperty(ref _bfColdHot, value); }
		}

		public double CFRegPer
		{
			get { return _cfRegPer; }
			set { SetProperty(ref _cfRegPer, value); }
		}

		public double CFNCV
		{
			get { return _cfncv; }
			set { SetProperty(ref _cfncv, value); }
		}

		public FuelType FuelType
		{
			get { return _fuelType; }
			set { SetProperty(ref _fuelType, value); }
		}

		public AllowedEntry<FuelType>[] AllowedFuelTypes
		{
			get
			{
				var types = //DeclarationMode ? :
					Enum.GetValues(typeof(FuelType)).Cast<FuelType>();
				return types.Select(ft => AllowedEntry.Create(ft, ft.GetLabel())).ToArray();
			}
		}

		public ObservableCollection<FuelConsumptionEntry> FCMap { get; }
		public ObservableCollection<FullLoadEntry> FullLoadCurve { get; }

		public double EngineeringCF
		{
			get { return _engineeringCf; }
			set { SetProperty(ref _engineeringCf, value); }
		}

		public IEngineDeclarationInputData ModelData
		{
			get { return AdapterFactory.EngineDeclarationAdapter(this); }
		}

		#endregion

		#region Overrides of AbstractEditComponentViewModel

		protected override void InputDataChanged()
		{
			JobViewModel.InputDataProvider.Switch()
						.If<IDeclarationInputDataProvider>(
							d =>
							{
								if (d.JobInputData.SavedInDeclarationMode)
								{
									// there are input data provider implementing both interfaces...
									LoadValues(d.JobInputData.Vehicle.Components.EngineInputData);
								}
							})
						.If<IEngineeringInputDataProvider>(
							e => LoadValues(
								e.JobInputData.JobType == VectoSimulationJobType.EngineOnlySimulation ? e.JobInputData.EngineOnly : e.JobInputData.Vehicle.Components.EngineInputData));
		}

		private void LoadValues(IEngineDeclarationInputData engine)
		{
			Manufacturer = engine.Manufacturer;
			Model = engine.Model;
			CertificationNumber = engine.CertificationNumber;
			DateTime date;
			//ToDo
			//var success = DateTime.TryParse(engine.Date, out date);
			//Date = success ? date : (DateTime?)null;
			Displacement = engine.Displacement;
			//ToDo
			//IdlingSpeed = engine.IdleSpeed;
			RatedSpeed = engine.RatedSpeedDeclared;
			RatedPower = engine.RatedPowerDeclared;
			MaxEngineTorque = engine.MaxTorqueDeclared;

			//ToDo
			//WHTCUrban = engine.WHTCUrban;
			//WHTCRural = engine.WHTCRural;
			//WHTCMotorway = engine.WHTCMotorway;
			//BFColdHot = engine.ColdHotBalancingFactor;
			//CFRegPer = engine.CorrectionFactorRegPer;
			//CFNCV = engine.CorrectionFactorNCV;
			//FuelType = engine.FuelType;
			//FullLoadCurve.Clear();
			//foreach (var entry in FullLoadCurveReader.Create(engine.FullLoadCurve, DeclarationMode).Entries) {
			//	FullLoadCurve.Add(new FullLoadEntry(entry));
			//}

			//FCMap.Clear();
			//foreach (var entry in FuelConsumptionMapReader.Create(engine.FuelConsumptionMap).Entries) {
			//	FCMap.Add(new FuelConsumptionEntry(entry));
			//}
		}

		private void LoadValues(IEngineEngineeringInputData engine)
		{
			LoadValues(engine as IEngineDeclarationInputData);
			//ToDo
			//EngineeringCF = engine.WHTCEngineering;
		}

		#endregion
	}
}
