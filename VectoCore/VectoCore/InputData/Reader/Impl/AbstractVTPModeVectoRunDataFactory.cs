using System;
using System.Collections.Generic;
using System.Linq;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.Reader.ComponentData;
using TUGraz.VectoCore.InputData.Reader.DataObjectAdapter;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.OutputData;

namespace TUGraz.VectoCore.InputData.Reader.Impl {
	internal abstract class AbstractVTPModeVectoRunDataFactory : IVectoRunDataFactory
	{
		protected IVTPDeclarationJobInputData JobInputData;
		protected DriverData Driverdata;
		protected AirdragData AirdragData;
		protected CombustionEngineData EngineData;
		protected AxleGearData AxlegearData;
		protected AngledriveData AngledriveData;
		protected GearboxData GearboxData;
		protected RetarderData RetarderData;
		protected PTOData PTOTransmissionData;
		protected List<VectoRunData.AuxData> AuxVTP;

		protected Segment Segment;
		protected bool _allowVocational;
		private DrivingCycleProxy _VTPCycle;
		
		public IVTPReport Report;
		protected ShiftStrategyParameters GearshiftData;
				
		protected abstract IDeclarationDataAdapter Dao { get; }

		protected AbstractVTPModeVectoRunDataFactory(IVTPDeclarationJobInputData job, IVTPReport report)
		{
			JobInputData = job;
			Report = report;
			_allowVocational = true;
		}

		protected abstract void Initialize();

		protected virtual void InitializeReport()
		{
			var airDragData = JobInputData.Vehicle.VehicleCategory.IsBus() ?
				JobInputData.CompletedVIFInputData.AirDragData :
				AirdragData;

			var powertrainConfig = new VectoRunData()
			{
				VehicleData = Dao.CreateVehicleData(
					JobInputData.Vehicle,
					Segment,
					Segment.Missions.First(),
					Segment.Missions.First().Loadings.First(),
					_allowVocational),
				AirdragData = AirdragData,
				EngineData = EngineData,
				GearboxSinglePwt = GearboxData,
				AxleGearSinglePwt = AxlegearData,
				RetarderSinglePwt = RetarderData,
				Aux = GetAuxiliaryData(Segment.Missions.First().MissionType),
				Cycle = VTPCycle,
			};

			powertrainConfig.VehicleData.VehicleClass = Segment.VehicleClass;
			Report.InputDataHash = JobInputData.VectoJobHash;
			Report.ManufacturerRecord = JobInputData.ManufacturerReportInputData;
			Report.ManufacturerRecordHash = JobInputData.VectoManufacturerReportHash;
			Report.CustomerFileHash = JobInputData.VectoCustomerFileHash;
			Report.OBFCMDeclarationInputData = JobInputData.OBFCMDeclarationInputData;
			Report.InitializeReport(powertrainConfig);
		}

		protected abstract IEnumerable<VectoRunData.AuxData> GetAuxiliaryData(MissionType missionType);

		protected DrivingCycleProxy VTPCycle => _VTPCycle ?? ( _VTPCycle = GetVTPCycle());

		private DrivingCycleProxy GetVTPCycle()
		{
			var vtpCycle = JobInputData.Cycles.FirstOrDefault();
            if (vtpCycle == null) {
                throw new VectoException("no VTP-Cycle provided!");
            }
            var drivingCycle = DrivingCycleDataReader.ReadFromDataTable(vtpCycle.CycleData, vtpCycle.Name, false);

			var cycle =  new DrivingCycleProxy(drivingCycle, vtpCycle.Name);

			if (cycle.CycleType != CycleType.VTP) {
				throw new VectoException("first cycle is not a VTP cycle!");
			}

			return cycle;
		}

		#region Implementation of IVectoRunDataFactory

		public virtual IEnumerable<VectoRunData> NextRun()
		{
			Initialize();
			if (Report != null) {
				InitializeReport();
			}

			return GetNextRun();
        }

		protected abstract IEnumerable<VectoRunData> GetNextRun();

        public abstract IInputDataProvider DataProvider { get; }

        public IVehicleDeclarationInputData CompletedVehicle { get; set; }


        #endregion


        protected virtual AuxFanData GetFanData()
		{
			return new AuxFanData() {
				FanCoefficients = DeclarationData.VTPMode.FanParameters,
				FanDiameter = JobInputData.FanDiameter,
			};
		}

		protected double GetMileagecorrectionFactor(Meter mileage)
		{
			if (mileage > DeclarationData.VTPMode.RunInThreshold) {
				return 1;
			}

			return DeclarationData.VTPMode.EvolutionCoefficient + (1 - DeclarationData.VTPMode.EvolutionCoefficient) * mileage /
					DeclarationData.VTPMode.RunInThreshold;
		}

		protected virtual VectoRunData CreateVectoRunData(Segment segment, Mission mission, Tuple<Kilogram, double?> loading)
		{
			return new VectoRunData {
				JobName = JobInputData.Vehicle.VIN,
				EngineData = EngineData,
				ElectricMachinesSinglePwt = new List<Tuple<PowertrainPosition, ElectricMotorData>>(),
                GearboxSinglePwt = GearboxData,
				GearshiftParametersSinglePwt = GearshiftData,
				AxleGearSinglePwt = AxlegearData,
				AngledriveSinglePwt = AngledriveData,
				VehicleData = Dao.CreateVehicleData(
					JobInputData.Vehicle, segment, mission,
					new KeyValuePair<LoadingType, Tuple<Kilogram, double?>>(LoadingType.ReferenceLoad, loading), _allowVocational),
				AirdragData = AirdragData,
				DriverData = null,
				BusAuxiliaries = null,
				RetarderSinglePwt = RetarderData,
				PTOSinglePwt = PTOTransmissionData,
				Report = Report,
				WheelEndData = Dao.CreateWheelEndData(segment.VehicleClass, JobInputData.Vehicle)
			};
		}
	}

	internal class VTPOBFCMData
	{
		private IDrivingCycleData _cycleData;
		private readonly FuelType[] AvailableFuels;

		private Dictionary<FuelType, Liter> _measuredConsumptionVolume = null;
		private Dictionary<FuelType, Kilogram> _measuredConsumptionMass = null;
		private Dictionary<FuelType, Liter> _cumulativeFuelConsumptionVolume = null;
		private Dictionary<FuelType, Kilogram> _cumulativeFuelConsumptionMass = null;

		private bool isOBFCMMassAvailable = false;
		private bool isOBFCMMileageAvailable = false;
		private bool isOBFCMFuelConsumptionMassFlowAvailable = false;
		private bool isOBFCMFuelConsumptionVolumeFlowAvailable = false;

		public VTPOBFCMData(VectoRunData runData)
		{
			_cycleData = runData?.Cycle ?? throw new ArgumentNullException(nameof(runData));
			AvailableFuels = _cycleData.Entries.First().Fuelconsumption.Keys.ToArray();

			var firstEntry = _cycleData.Entries.First();
			isOBFCMMassAvailable = firstEntry.OBFCMMass != null;
			isOBFCMMileageAvailable = firstEntry.OBFCMMileage != null;
			isOBFCMFuelConsumptionMassFlowAvailable = firstEntry.OBFCMFuelConsumptionMassFlow.First().Value != null;
			isOBFCMFuelConsumptionVolumeFlowAvailable = firstEntry.OBFCMFuelConsumptionVolumeFlow.First().Value != null;
		}

		public bool IsOBFCM => isOBFCMMassAvailable
			|| isOBFCMMileageAvailable
			|| isOBFCMFuelConsumptionMassFlowAvailable
			|| isOBFCMFuelConsumptionVolumeFlowAvailable;

		public Meter StartMileage => _cycleData.Entries.First()?.OBFCMMileage ?? null;

		public Meter EndMileage => _cycleData.Entries.Last()?.OBFCMMileage ?? null;

		public Kilogram AverageMass
		{
			get
			{
				if (!isOBFCMMassAvailable)
				{
					return null;
				}

				return _cycleData.Entries.Sum(e => e.OBFCMMass) / (_cycleData.Entries.Count);
			}
		}

		public Dictionary<FuelType, Liter> CumulativeFuelConsumptionVolume
		{
			get
			{
				if (_cumulativeFuelConsumptionVolume != null)
				{
					return _cumulativeFuelConsumptionVolume;
				}

				if (!isOBFCMFuelConsumptionVolumeFlowAvailable)
				{
					return null;
				}

				return _cumulativeFuelConsumptionVolume = GetFuelConsumptionByVolume();
			}
		}

		public Dictionary<FuelType, Kilogram> CumulativeFuelConsumptionMass
		{
			get
			{
				if(_cumulativeFuelConsumptionMass != null)
				{
					return _cumulativeFuelConsumptionMass;
				}

				if (!isOBFCMFuelConsumptionMassFlowAvailable)
				{
					return null;
				}

				return _cumulativeFuelConsumptionMass = GetFuelConsumptionByMass();
			}
		}

		public Kilogram TotalCumulativeFuelConsumptionMass
		{
			get
			{
				if (!isOBFCMFuelConsumptionMassFlowAvailable)
				{
					return null;
				}

				return CumulativeFuelConsumptionMass.Select(fc => fc.Value).Sum();
			}
		}

		public Liter TotalCumulativeFuelConsumptionVolume
		{
			get
			{
				if (!isOBFCMFuelConsumptionVolumeFlowAvailable)
				{
					return null;
				}

				return CumulativeFuelConsumptionVolume.Select(fc => fc.Value).Sum();
			}
		}

		public Dictionary<FuelType, Liter> MeasuredConsumptionVolume
		{
			get
			{
				if (_measuredConsumptionVolume == null)
				{
					_measuredConsumptionVolume = CalculateMeasuredConsumptionLiter();
				}

				return _measuredConsumptionVolume;
			}
		}

		public Dictionary<FuelType, Kilogram> MeasuredConsumptionMass
		{
			get
			{
				if (_measuredConsumptionMass == null)
				{
					_measuredConsumptionMass = CalculateMeasuredConsumptionKilogram();
				}

				return _measuredConsumptionMass;
			}
		}

		public Liter TotalMeasuredConsumptionVolume
		{
			get
			{
				if (_measuredConsumptionVolume == null)
				{
					_measuredConsumptionVolume = CalculateMeasuredConsumptionLiter();
				}

				if(_measuredConsumptionVolume.Count != AvailableFuels.Count())
				{
					return null;
				}

				return _measuredConsumptionVolume.Select(fc => fc.Value).Sum();
			}
		}

		public Kilogram TotalMeasuredConsumptionMass
		{
			get
			{
				if (_measuredConsumptionMass == null)
				{
					_measuredConsumptionMass = CalculateMeasuredConsumptionKilogram();
				}

				return _measuredConsumptionMass.Select(fC => fC.Value).Sum();
			}
		}

		private Dictionary<FuelType, Kilogram> CalculateMeasuredConsumptionKilogram()
		{
			var entries = _cycleData.Entries;
			var resultFC = new Dictionary<FuelType, Kilogram>();
			for (int i = 0; i < entries.Count; i++)
			{
				var dt = i == 0 ? entries[i].Time : entries[i].Time - entries[i - 1].Time;
				foreach (var fuelType in entries[i].Fuelconsumption.Keys)
				{
					if (!resultFC.ContainsKey(fuelType))
					{
						resultFC.Add(fuelType, 0.SI<Kilogram>());
					}

					resultFC[fuelType] += entries[i].Fuelconsumption[fuelType] * dt;
				}
			}

			return resultFC;
		}

		private Dictionary<FuelType, Liter> CalculateMeasuredConsumptionLiter()
		{
			if(_measuredConsumptionMass == null)
			{
				_measuredConsumptionMass = CalculateMeasuredConsumptionKilogram();
			}

			var resultFC = new Dictionary<FuelType, Liter>();
			foreach(var fuelType in _measuredConsumptionMass.Keys)
			{
				if(fuelType.Density() != null)
				{
					resultFC.Add(fuelType, (_measuredConsumptionMass[fuelType] / fuelType.Density()).Cast<Liter>());
				}
			}

			return resultFC;
		}

		private Dictionary<FuelType, Liter> GetFuelConsumptionByVolume()
		{
			var entries = _cycleData.Entries;
			Dictionary<FuelType, Liter> consumptionByFuel = new Dictionary<FuelType, Liter>();
			for (int i = 0; i < entries.Count; i++)
			{
				var dt = i == 0 ? entries[i].Time : entries[i].Time - entries[i - 1].Time;
				foreach (var fuelType in entries[i].OBFCMFuelConsumptionVolumeFlow.Keys)
				{
					if (!consumptionByFuel.ContainsKey(fuelType))
					{
						consumptionByFuel.Add(fuelType, 0.SI<Liter>());
					}

					consumptionByFuel[fuelType] += entries[i].OBFCMFuelConsumptionVolumeFlow[fuelType] * dt;
				}
			}

			return consumptionByFuel;
		}

		private Dictionary<FuelType, Kilogram> GetFuelConsumptionByMass()
		{
			var entries = _cycleData.Entries;
			Dictionary<FuelType, Kilogram> consumptionByFuel = new Dictionary<FuelType, Kilogram>();
			for (int i = 0; i < entries.Count; i++)
			{
				var dt = i == 0 ? entries[i].Time : entries[i].Time - entries[i - 1].Time;
				foreach (var fuelType in entries[i].OBFCMFuelConsumptionMassFlow.Keys)
				{
					if (!consumptionByFuel.ContainsKey(fuelType))
					{
						consumptionByFuel.Add(fuelType, 0.SI<Kilogram>());
					}

					consumptionByFuel[fuelType] += entries[i].OBFCMFuelConsumptionMassFlow[fuelType] * dt;
				}
			}

			return consumptionByFuel;
		}
	}

	public static class FuelDataExtensions
	{
		public static KilogramPerCubicMeter Density(this FuelType fuelType)
		{
			return fuelType.IsNaturalGas()
				? FuelData.Instance().Lookup(fuelType, TankSystem.Compressed).FuelDensity
				: FuelData.Instance().Lookup(fuelType).FuelDensity;
		}
	}
}