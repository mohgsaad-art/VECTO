using System;
using System.Collections.Generic;
using System.Linq;
using TUGraz.VectoCommon.BusAuxiliaries;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.InputData.Reader.ComponentData;
using TUGraz.VectoCore.InputData.Reader.DataObjectAdapter;
using TUGraz.VectoCore.InputData.Reader.DataObjectAdapter.PrimaryBus;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Impl;
using TUGraz.VectoCore.OutputData;

namespace TUGraz.VectoCore.InputData.Reader.Impl
{
    internal class DeclarationVTPModeVectoRunDataFactoryHeavyBusPrimary : AbstractVTPModeVectoRunDataFactory
    {
        private IPrimaryBusDeclarationDataAdapter _dao;

        protected readonly IInputDataProvider InputDataProvider;

        public DeclarationVTPModeVectoRunDataFactoryHeavyBusPrimary(
            IVTPDeclarationInputDataProvider ivtpProvider, IVTPReport report) : base(ivtpProvider.JobInputData, report)
        {
            InputDataProvider = ivtpProvider;
        }

        protected DeclarationVTPModeVectoRunDataFactoryHeavyBusPrimary(IInputDataProvider inputProvider, IVTPReport report) : 
            base((inputProvider as IVTPEngineeringInputDataProvider).JobInputData, report) 
        {
            InputDataProvider = inputProvider;
        }


        #region Implementation of IVectoRunDataFactory

        public override IInputDataProvider DataProvider => InputDataProvider;

        protected IPrimaryBusDeclarationDataAdapter DataAdapter => _dao ?? (_dao = new DeclarationDataAdapterPrimaryBus.Conventional());

        #endregion

		protected override IDeclarationDataAdapter Dao => DataAdapter;

		protected override void Initialize()
        {
            var vehicle = JobInputData.Vehicle;
            Segment = DeclarationData.PrimaryBusSegments.Lookup(
                vehicle.VehicleCategory, vehicle.AxleConfiguration, vehicle.Articulated);

            Driverdata = DataAdapter.CreateBusDriverData(Segment, vehicle.VehicleType, vehicle.ArchitectureID, vehicle.Components.BusAuxiliaries.PneumaticSupply.CompressorDrive);
            var tempVehicle = DataAdapter.CreateVehicleData(
                vehicle, Segment, Segment.Missions.First(),
                Segment.Missions.First().Loadings.First(), _allowVocational);
            tempVehicle.VehicleClass = JobInputData.ManufacturerReportInputData.VehicleClass;
            tempVehicle.VehicleCode = JobInputData.ManufacturerReportInputData.VehicleCode;

            var vtpMission = tempVehicle.VehicleCode.GetFloorType() == FloorType.LowFloor
                ? DeclarationData.VTPMode.SelectedMissionLowFloorBus
                : DeclarationData.VTPMode.SelectedMissionHighFloorBus;
            AirdragData = DataAdapter.CreateAirdragData(
                vehicle.Components.AirdragInputData,
                Segment.Missions.First(), Segment);
            EngineData = DataAdapter.CreateEngineData(
                vehicle, vehicle.Components.EngineInputData.EngineModes.First(),
                new Mission() { MissionType = vtpMission });
            AxlegearData = JobInputData.Vehicle.Components.GearboxInputData.DifferentialIncluded
                ? DataAdapter.CreateDummyAxleGearData(JobInputData.Vehicle.Components.GearboxInputData)
                : DataAdapter.CreateAxleGearData(vehicle.Components.AxleGearInputData);
            AngledriveData = DataAdapter.CreateAngledriveData(vehicle.Components.AngledriveInputData);

			var vectoRun = new VectoRunData()
			{
				EngineData = EngineData,
				AxleGearData = AxlegearData,
				VehicleData = tempVehicle,
				Cycle = VTPCycle
			};
			
			GearboxData = DataAdapter.CreateGearboxData(vehicle, vectoRun, null);
			RetarderData = DataAdapter.CreateRetarderData(vehicle.Components.RetarderInputData, vehicle.ArchitectureID, vehicle.Components.IEPC);

            //PTOTransmissionData =
            //    DataAdapter.CreatePTOTransmissionData(vehicle.Components.PTOTransmissionInputData);

            GearshiftData = DataAdapter.CreateGearshiftData(
				AxlegearData.AxleGear.Ratio * (AngledriveData?.Angledrive.Ratio ?? 1.0), 
				EngineData.IdleSpeed, 
				vehicle.Components.GearboxInputData.Type,
				vehicle.Components.GearboxInputData.Gears.Count);

            AuxVTP = CreateVTPAuxData(vehicle);
        }

        protected override void InitializeReport()
        {
            var vehicle = JobInputData.Vehicle;
            var tempVehicle = DataAdapter.CreateVehicleData(
                vehicle, Segment, Segment.Missions.First(),
                Segment.Missions.First().Loadings.First(), _allowVocational);
            tempVehicle.VehicleClass = JobInputData.ManufacturerReportInputData.VehicleClass;
            tempVehicle.VehicleCode = JobInputData.ManufacturerReportInputData.VehicleCode;
            var powertrainConfig = new VectoRunData()
            {
                VehicleData = tempVehicle,
                AirdragData = AirdragData,
                EngineData = EngineData,
                GearboxData = GearboxData,
                AxleGearData = AxlegearData,
                Retarder = RetarderData,
                Aux = GetAuxiliaryData(Segment.Missions.First().MissionType),
            };
            //powertrainConfig.VehicleData.VehicleClass = Segment.VehicleClass;
            Report.InputDataHash = JobInputData.VectoJobHash;
            Report.ManufacturerRecord = JobInputData.ManufacturerReportInputData;
            Report.ManufacturerRecordHash = JobInputData.VectoManufacturerReportHash;
            var fuels = JobInputData.Vehicle.Components.EngineInputData.EngineModes.Select(
                                        x => x.Fuels.Select(f => DeclarationData.FuelData.Lookup(f.FuelType, JobInputData.Vehicle.TankSystem))
                                            .ToList())
                                    .ToList();
            Report.InitializeReport(powertrainConfig);
        }

        protected virtual List<VectoRunData.AuxData> CreateVTPAuxData(IVehicleDeclarationInputData vehicle)
        {
            // used to fill VECTO RunData for VTP mission.

            var retVal = new List<VectoRunData.AuxData>();

            var electricEfficiency =
                Constants.BusAuxiliaries.ElectricSystem.AlternatorGearEfficiency *
                DeclarationData.BusAuxiliaries.AlternatorTechnologies.Lookup("default");

            // TODO: vehicle length from MRF

            var spPowerDemand = DeclarationData.SteeringPumpBus.LookupMechanicalPowerDemand(
                                    MissionType.VerificationTest, JobInputData.Vehicle.Components.BusAuxiliaries.SteeringPumpTechnology,
                                    JobInputData.ManufacturerReportInputData.VehicleLength)
                                +
                                DeclarationData.SteeringPumpBus.LookupElectricalPowerDemand(
                                    MissionType.VerificationTest, JobInputData.Vehicle.Components.BusAuxiliaries.SteeringPumpTechnology,
                                    JobInputData.ManufacturerReportInputData.VehicleLength) / electricEfficiency;

            retVal.Add(
                new VectoRunData.AuxData()
                {
                    DemandType = AuxiliaryDemandType.Constant,
                    Technology = JobInputData.Vehicle.Components.BusAuxiliaries.SteeringPumpTechnology,
                    ID = Constants.Auxiliaries.IDs.SteeringPump,
                    PowerDemandMech = spPowerDemand
                });

            retVal.Add(
                new VectoRunData.AuxData()
                {
                    DemandType = AuxiliaryDemandType.Constant,
                    Technology = new List<string>() { "default" },
                    ID = Constants.Auxiliaries.IDs.ElectricSystem,
                    PowerDemandMech = Constants.BusAuxiliaries.ElectricSystem.PowernetVoltage * 32.4.SI<Ampere>() / electricEfficiency
                });
            retVal.Add(new VectoRunData.AuxData()
            {
                DemandType = AuxiliaryDemandType.Constant,
                Technology = new List<string>() { "default" },
                ID = Constants.Auxiliaries.IDs.HeatingVentilationAirCondition,
                PowerDemandMech = 350.SI<Watt>()
            });

            var busAux = vehicle.Components.BusAuxiliaries;
            var psCompressor = DeclarationData.BusAuxiliaries.GetCompressorMap(busAux.PneumaticSupply);
            retVal.Add(new VectoRunData.AuxData()
            {
                DemandType = AuxiliaryDemandType.Direct,
                Technology = new List<string>() { busAux.PneumaticSupply.CompressorSize + " / " + busAux.PneumaticSupply.Clutch },
                ID = Constants.Auxiliaries.IDs.PneumaticSystem,
                PowerDemandMechCycleFunc = cycleEntry =>
                {
                    var cmp = psCompressor.Interpolate(cycleEntry.EngineSpeed * busAux.PneumaticSupply.Ratio);
                    return cycleEntry.VTPPSCompressorActive ? cmp.PowerOn : cmp.PowerOff;
                }
            });

            var fanData = GetFanData();
            var engineFan = new EngineFanAuxiliary(fanData.FanCoefficients, fanData.FanDiameter);
            retVal.Add(new VectoRunData.AuxData()
            {
                DemandType = AuxiliaryDemandType.Direct,
                Technology = new List<string>() { "default" },
                ID = Constants.Auxiliaries.IDs.Fan,
                PowerDemandMechCycleFunc = cycleEntry => engineFan.PowerDemand(cycleEntry)
            });

            return retVal;
        }

        protected override IEnumerable<VectoRunData.AuxData> GetAuxiliaryData(MissionType missionType)
        {
            // used for initializing XML report,
            return AuxVTP;
        }

        protected override AuxFanData GetFanData()
        {
            return new AuxFanData()
            {
                FanCoefficients = DeclarationData.VTPMode.FanParameters.Concat(JobInputData.FanPowerCoefficents.Skip(3).Take(1)).ToArray(),
                FanDiameter = JobInputData.FanDiameter,
            };
        }

        public override IEnumerable<VectoRunData> NextRun()
        {
            if (InitException != null)
            {
                throw InitException;
            }


            // simulate the Measured cycle
            var vtpCycle = JobInputData.Cycles.FirstOrDefault();
            if (vtpCycle == null)
            {
                throw new VectoException("no VTP-Cycle provided!");
            }

            var drivingCycle = DrivingCycleDataReader.ReadFromDataTable(vtpCycle.CycleData, vtpCycle.Name, false);

            // Loading is not relevant as we use P_wheel
            var vtpRunData = CreateVectoRunData(Segment, Segment.Missions.First(), Tuple.Create<Kilogram, double?>(0.SI<Kilogram>(), null));
            vtpRunData.Cycle = new DrivingCycleProxy(drivingCycle, vtpCycle.Name);
            vtpRunData.Aux = AuxVTP;
            vtpRunData.FanDataVTP = GetFanData();
            vtpRunData.ExecutionMode = ExecutionMode.Declaration;
            vtpRunData.SimulationType = SimulationType.VerificationTest;
            vtpRunData.Mission = new Mission()
            {
                MissionType = MissionType.VerificationTest
            };
            vtpRunData.VehicleData.VehicleClass = JobInputData.ManufacturerReportInputData.VehicleClass; //Segment.VehicleClass;
            vtpRunData.VehicleData.VehicleCode = JobInputData.ManufacturerReportInputData.VehicleCode;
            vtpRunData.VehicleData.LegislativeClass = JobInputData.Vehicle.LegislativeClass;

            //var ncvStd = DeclarationData.FuelData.Lookup(JobInputData.Vehicle.Components.EngineInputData.FuelType).LowerHeatingValueVecto;
            //var ncvCorrection = ncvStd / JobInputData.NetCalorificValueTestFuel;
            var mileageCorrection = GetMileagecorrectionFactor(JobInputData.Mileage);
			var correctionFactors = JobInputData.FuelNCVs.ToDictionary(
				keySelector: f => f.Type, 
				elementSelector: f => (f.NCV / DeclarationData.FuelData.Lookup(
					f.Type, 
					JobInputData.Vehicle.TankSystem).LowerHeatingValueVecto).Value() * mileageCorrection);
            
            vtpRunData.VTPData = new VTPData() {
				CorrectionFactors = correctionFactors,
				FuelNCVs = JobInputData.FuelNCVs
            };

            vtpRunData.DriverData = Driverdata;
			vtpRunData.TorqueDriftLeftWheel = JobInputData.TorqueDriftLeftWheel;
			vtpRunData.TorqueDriftRightWheel = JobInputData.TorqueDriftRightWheel;

            yield return vtpRunData;
        }


    }
}