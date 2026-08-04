using System;
using System.Collections.Generic;
using System.Linq;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.OutputData;
using static TUGraz.VectoCommon.InputData.VectoSimulationJobType;

namespace TUGraz.VectoCore.InputData.Reader.Impl
{
    public abstract class AbstractDeclarationVectoRunDataFactory : LoggingObject, IVectoRunDataFactory
	{
		protected readonly IDeclarationInputDataProvider InputDataProvider;

		protected IDeclarationReport Report;
		
		protected Segment _segment;

        protected bool _allowVocational;

        private DriverData _driverdata;

		
		protected IDeclarationCycleFactory CycleFactory { get; }

		protected IPowertrainBuilder PowertrainBuilder { get; }

		protected virtual IVehicleDeclarationInputData Vehicle => InputDataProvider.JobInputData.Vehicle;

        protected AbstractDeclarationVectoRunDataFactory(IDeclarationInputDataProvider dataProvider,
			IDeclarationReport report,
			IDeclarationCycleFactory cycleFactory, IMissionFilter missionFilter,
			bool checkJobType, IPowertrainBuilder ptBuilder)
		{
			CycleFactory = cycleFactory;
			PowertrainBuilder = ptBuilder;
			InputDataProvider = dataProvider;
			MissionFilter = missionFilter;
			if (checkJobType) {
				if (dataProvider.JobInputData.JobType.IsOneOf(BatteryElectricVehicle, ParallelHybridVehicle, SerialHybridVehicle))
				{
					throw new VectoSimulationException("Electric and Hybrid Vehicles are not supported in Declaration Mode. Aborting Simulation.");
				}
			}
           
            Report = report;

			_allowVocational = true;
		}

		protected IMissionFilter MissionFilter { get; }

		protected DriverData DriverData => _driverdata ??(_driverdata= CreateDriverData(_segment));
		protected abstract DriverData CreateDriverData(Segment segment);

		public virtual IEnumerable<VectoRunData> NextRun()
		{
		
			Initialize();
			if (Report != null) {
				InitializeReport();
			}

			return GetNextRun();
		}

        public IVehicleDeclarationInputData CompletedVehicle { get; set; }

        public IInputDataProvider DataProvider => InputDataProvider;

		protected abstract IEnumerable<VectoRunData> GetNextRun();

		protected abstract void Initialize();

		protected abstract VectoRunData CreateVectoRunData(Mission mission,
			KeyValuePair<LoadingType, Tuple<Kilogram, double?>> loading,
			int? modeIdx = null,
			OvcHevMode ovcMode = OvcHevMode.NotApplicable);


		protected virtual void InitializeReport()
		{
			var powertrainConfig = GetPowertrainConfigForReportInit();
			Report.InitializeReport(powertrainConfig);
		}

		protected abstract VectoRunData GetPowertrainConfigForReportInit();

		/// <summary>
		/// Super caps are not allowed for ovc hevs or pevs
		/// </summary>
		protected virtual void CheckSuperCap(IVehicleDeclarationInputData vehicle)
		{
			if (vehicle.VehicleType == VectoSimulationJobType.BatteryElectricVehicle || vehicle.OVC) {
				if (vehicle.Components.ElectricStorage.ElectricStorageElements.Any(e =>
						e.REESSPack.StorageType == REESSType.SuperCap)) {
					throw new VectoException("Super caps are not allowed for OVC-HEVs or PEVs");
				}
			}

			if (vehicle.Components.ElectricStorage?.ElectricStorageElements == null) {
				return;
			}

			var hasSuperCap = vehicle.Components.ElectricStorage.ElectricStorageElements.Any(e =>
				e.REESSPack.StorageType == REESSType.SuperCap);
			var hasBattery = vehicle.Components.ElectricStorage.ElectricStorageElements.Any(e =>
				e.REESSPack.StorageType == REESSType.Battery);

			if (hasSuperCap && hasBattery) {
				//Already handled by XML Schema
				throw new VectoException("Super caps AND batteries are not supported");
			}
		}
    }
}