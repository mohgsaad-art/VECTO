using System;
using System.Collections.Generic;
using System.Linq;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Configuration;
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
		
		protected Exception InitException;

		public IVTPReport Report;
		protected ShiftStrategyParameters GearshiftData;

		
		protected abstract IDeclarationDataAdapter Dao { get; }

		
		protected AbstractVTPModeVectoRunDataFactory(IVTPDeclarationJobInputData job, IVTPReport report)
		{
			JobInputData = job;
			Report = report;
			_allowVocational = true;
			try {
				Initialize();
				if (Report != null) {
					InitializeReport();
				}
			} catch (Exception e) {
				InitException = e;
			}
		}

		protected abstract void Initialize();

		protected virtual void InitializeReport()
		{
			var powertrainConfig = new VectoRunData() {
				VehicleData =
					Dao.CreateVehicleData(
						JobInputData.Vehicle, Segment, Segment.Missions.First(),
						Segment.Missions.First().Loadings.First(), _allowVocational),
				AirdragData = AirdragData,
				EngineData = EngineData,
				GearboxData = GearboxData,
				AxleGearData = AxlegearData,
				Retarder = RetarderData,
				Aux = GetAuxiliaryData(Segment.Missions.First().MissionType),
			};
			powertrainConfig.VehicleData.VehicleClass = Segment.VehicleClass;
			Report.InputDataHash = JobInputData.VectoJobHash;
			Report.ManufacturerRecord = JobInputData.ManufacturerReportInputData;
			Report.ManufacturerRecordHash = JobInputData.VectoManufacturerReportHash;
			var fuels = JobInputData.Vehicle.Components.EngineInputData.EngineModes.Select(
										x => x.Fuels.Select(f => DeclarationData.FuelData.Lookup(f.FuelType, JobInputData.Vehicle.TankSystem))
											.ToList())
									.ToList();
			Report.InitializeReport(powertrainConfig, fuels);
		}

		protected abstract IEnumerable<VectoRunData.AuxData> GetAuxiliaryData(MissionType missionType);


		#region Implementation of IVectoRunDataFactory

		public abstract IEnumerable<VectoRunData> NextRun();

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
				ElectricMachinesData = new List<Tuple<PowertrainPosition, ElectricMotorData>>(),
                GearboxData = GearboxData,
				GearshiftParameters = GearshiftData,
				AxleGearData = AxlegearData,
				AngledriveData = AngledriveData,
				VehicleData = Dao.CreateVehicleData(
					JobInputData.Vehicle, segment, mission,
					new KeyValuePair<LoadingType, Tuple<Kilogram, double?>>(LoadingType.ReferenceLoad, loading), _allowVocational),
				AirdragData = AirdragData,
				DriverData = null,
				BusAuxiliaries = null,
				Retarder = RetarderData,
				PTO = PTOTransmissionData,
				Report = Report,
			};
		}

		

	}
}