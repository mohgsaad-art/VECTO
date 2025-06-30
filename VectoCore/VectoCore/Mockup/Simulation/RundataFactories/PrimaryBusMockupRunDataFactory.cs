using System;
using System.Collections.Generic;
using System.Linq;
using TUGraz.VectoCommon.BusAuxiliaries;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Interfaces;
using TUGraz.VectoCore.InputData.Reader.DataObjectAdapter;
using TUGraz.VectoCore.InputData.Reader.Impl;
using TUGraz.VectoCore.InputData.Reader.Impl.DeclarationMode.PrimaryBusRunDataFactory;
using TUGraz.VectoCore.Models.BusAuxiliaries;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Gearbox;
using TUGraz.VectoCore.OutputData;

namespace TUGraz.VectoCore.Mockup.Simulation.RundataFactories
{
    public class PrimaryBusMockupRunDataFactory :  DeclarationModePrimaryBusRunDataFactory.Conventional
    {
        public PrimaryBusMockupRunDataFactory(IDeclarationInputDataProvider dataProvider,
			IDeclarationReport report,
			IPrimaryBusDeclarationDataAdapter declarationDataAdapter, IDeclarationCycleFactory cycleFactory,
			IMissionFilter missionFilter, IPowertrainBuilder ptBuilder) :
            base(dataProvider, report, declarationDataAdapter, cycleFactory, missionFilter, ptBuilder)
        { }

        #region Overrides of AbstractDeclarationVectoRunDataFactory

		protected override IEnumerable<VectoRunData> GetNextRun()
		{
			if (InputDataProvider.JobInputData.Vehicle.VehicleCategory == VehicleCategory.HeavyBusPrimaryVehicle) {
				if (InputDataProvider.JobInputData.Vehicle.ExemptedVehicle) {
					yield return CreateVectoRunData(null,
						new KeyValuePair<LoadingType, Tuple<Kilogram, double?>>(), 0);
				} else {
					foreach (var vectoRunData in VectoRunDataHeavyBusPrimary()) {
						yield return vectoRunData;
					}
				}
			}

			foreach (var entry in new List<VectoRunData>()) {
				yield return entry;
			}
		}

		private IEnumerable<VectoRunData> VectoRunDataHeavyBusPrimary()
		{
			switch (InputDataProvider.JobInputData.JobType) {
				case VectoSimulationJobType.ConventionalVehicle:
				case VectoSimulationJobType.ParallelHybridVehicle:
				case VectoSimulationJobType.SerialHybridVehicle:
                case VectoSimulationJobType.IHPC:
                case VectoSimulationJobType.IEPC_S:
					return VectoRunDataConventionalHeavyBusPrimaryNonExempted();
                case VectoSimulationJobType.IEPC_E:
				case VectoSimulationJobType.BatteryElectricVehicle:
                case VectoSimulationJobType.FCHV:
                case VectoSimulationJobType.FCHV_IEPC:
                case VectoSimulationJobType.Multiple_FCHV:
                case VectoSimulationJobType.Multiple_PEV:
                case VectoSimulationJobType.Multiple_SHEV:
                    return VectoRunDataBatteryElectricHeavyBusPrimaryNonExempted();
				case VectoSimulationJobType.EngineOnlySimulation:
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
			return VectoRunDataConventionalHeavyBusPrimaryNonExempted();
		}

		private IEnumerable<VectoRunData> VectoRunDataBatteryElectricHeavyBusPrimaryNonExempted()
		{
			var vehicle = InputDataProvider.JobInputData.Vehicle;
			foreach (var mission in _segment.Missions) {
				foreach (var loading in mission.Loadings) {
					var simulationRunData = CreateVectoRunData(mission, loading, 0);
					if (simulationRunData == null) {
						continue;
					}
					yield return simulationRunData;
				}

			}
		}

		private IEnumerable<VectoRunData> VectoRunDataConventionalHeavyBusPrimaryNonExempted()
		{
			var vehicle = InputDataProvider.JobInputData.Vehicle;
			var engine = vehicle.Components.EngineInputData;
			var engineModes = engine.EngineModes;

			for (var modeIdx = 0; modeIdx < engineModes.Count; modeIdx++) {
				foreach (var mission in _segment.Missions) {
					foreach (var loading in mission.Loadings) {
						var simulationRunData = CreateVectoRunData(mission, loading, modeIdx);
						if (simulationRunData == null) {
							continue;
						}
						yield return simulationRunData;
					}
				}
			}
		}

        protected override void Initialize()
        {
            _segment = GetSegment();
        }

        protected override void InitializeReport()
        {
            if (InputDataProvider.JobInputData.JobType == VectoSimulationJobType.ConventionalVehicle)
            {
                base.InitializeReport();
                return;
            }

            VectoRunData powertrainConfig;
            var vehicle = InputDataProvider.JobInputData.Vehicle;
            if (vehicle.ExemptedVehicle)
            {
                powertrainConfig = CreateVectoRunData(null,
                    new KeyValuePair<LoadingType, Tuple<Kilogram, double?>>(), 0);
            }
            else
            {
                powertrainConfig = _segment.Missions.Select(
                        mission => CreateVectoRunData(mission, mission.Loadings.First(), 0))
                    .FirstOrDefault(x => x != null);
            }

            Report.InitializeReport(powertrainConfig);
        }

        #region Overrides of DeclarationModePrimaryBusVectoRunDataFactory

        protected override VectoRunData CreateVectoRunData(Mission mission,
			KeyValuePair<LoadingType, Tuple<Kilogram, double?>> loading,
			int? modeIdx,
			OvcHevMode ovcMode = OvcHevMode.NotApplicable)
        {

            VectoRunData runData;
            if (InputDataProvider.JobInputData.Vehicle.ExemptedVehicle)
            {
				runData = new VectoRunData
				{
					Exempted = true,
					Report = Report,
					Mission = new Mission() { MissionType = MissionType.ExemptedMission },
					VehicleData = CreateExemptedMockupVehicleData(Vehicle, _segment),
					InputDataHash = InputDataProvider.XMLHash
				};
				runData.VehicleData.InputData = Vehicle;
            }
            else
            {
				var cycle = CycleFactory.GetDeclarationCycle(mission);

                if (Vehicle.VehicleType.IsMultiplePowertrains())
                {
                    runData = new VectoRunData() 
                    {
                        Loading = loading.Key,
                        Cycle = new DrivingCycleProxy(cycle, mission.MissionType.ToString()),
                        ExecutionMode = ExecutionMode.Declaration,
                        Report = Report,
                        Mission = mission,
                        SimulationType = SimulationType.DistanceCycle,
                        VehicleData = CreateMockupVehicleData(Vehicle, _segment, loading),
                        EngineData = CreateMockupEngineData(Vehicle, modeIdx),
                        BusAuxiliaries = CreateMockupBusAux(Vehicle),
                        InputDataHash = InputDataProvider.XMLHash,
                        AxlePowertrainsData = new List<AxlePowertrainData>()
                    };

                    foreach (var axlePt in Vehicle.Components.AxlePowertrainInputData)
                    {
                        var axlePowertrainData = new AxlePowertrainData()
                        {
                            AxleNumber = axlePt.AxleNumber,
                            Retarder = CreateMockupRetarder(axlePt),
                            AxleGearData = CreateMockupAxleGearData(axlePt.AxleGearInputData),
                            GearboxData = CreateMockupGearboxData(axlePt.GearboxInputData),
                            AngledriveData = CreateMockupAngleDriveData(axlePt.AngledriveInputData),
                        };
                        
                        runData.AxlePowertrainsData.Add(axlePowertrainData);
                    }
                }
                else
                {
                    runData = new VectoRunData()
                    {
                        Loading = loading.Key,
                        Cycle = new DrivingCycleProxy(cycle, mission.MissionType.ToString()),
                        ExecutionMode = ExecutionMode.Declaration,
                        Report = Report,
                        Mission = mission,
                        SimulationType = SimulationType.DistanceCycle,
                        VehicleData = CreateMockupVehicleData(Vehicle, _segment, loading),
                        Retarder = CreateMockupRetarder(Vehicle),
                        AxleGearData = CreateMockupAxleGearData(Vehicle.Components.AxleGearInputData),
                        GearboxData = CreateMockupGearboxData(Vehicle.Components.GearboxInputData),
                        AngledriveData = CreateMockupAngleDriveData(Vehicle.Components.AngledriveInputData),
                        EngineData = CreateMockupEngineData(Vehicle, modeIdx),
                        BusAuxiliaries = CreateMockupBusAux(Vehicle),
                        InputDataHash = InputDataProvider.XMLHash,
                    };
                }
            }

            runData.InputData = InputDataProvider;

            return runData;
        }


		#endregion

        #endregion
        public static IAuxiliaryConfig CreateMockupBusAux(IVehicleDeclarationInputData vehicle)
        {
            return new AuxiliaryConfig()
            {
                InputData = vehicle.Components.BusAuxiliaries,

            };
        }

		public static AirdragData CreateMockupAirdragData(IVehicleDeclarationInputData vehicle)
		{
			var airdrag = vehicle.Components.AirdragInputData;
			if (airdrag == null) {
				return new AirdragData() {
					CertificationMethod = CertificationMethod.StandardValues,
					DeclaredAirdragArea = 0.SI<SquareMeter>(), // dummy value -- no used at all
				};
			}
			return new AirdragData() {
				CertificationMethod = airdrag.CertificationMethod,
				CertificationNumber = airdrag.CertificationNumber,
				DeclaredAirdragAreaInput = airdrag.AirDragArea,
			};
		}

        public static RetarderData CreateMockupRetarder(IVehicleDeclarationInputData vehicle)
        {
            var xmlVehicle = vehicle as IXMLDeclarationVehicleData;
            return new RetarderData()
            {
                Type = xmlVehicle.GetRetarderType(),

                Ratio = xmlVehicle.GetRetarderType().IsDedicatedComponent() ? xmlVehicle.GetRetarderRatio() : 0,
            };
        }

        public static RetarderData CreateMockupRetarder(IAxlePowertrainDeclarationInputData axlePt)
        {
            var type = axlePt.RetarderInputData?.Type ?? RetarderType.None;

            return new RetarderData()
            {
                Type = type,

                Ratio = type.IsDedicatedComponent() ? axlePt.RetarderInputData.Ratio : 0,
            };
        }

        public static AngledriveData CreateMockupAngleDriveData(IAngledriveInputData angledrive)
        {
            if (angledrive == null || angledrive.Type != AngledriveType.SeparateAngledrive)
            {
                return null;
            }

			var angleDriveData = new AngledriveData {
				InputData = angledrive,
				Type = angledrive.Type,
                Angledrive = new TransmissionData() { Ratio = angledrive.Ratio },
                Manufacturer = angledrive.Manufacturer,
                ModelName = angledrive.Model,
                CertificationNumber = angledrive.CertificationNumber,
                Date = angledrive.Date
            };

			return angleDriveData;
		}

        public static AxleGearData CreateMockupAxleGearData(IAxleGearInputData axlegear)
        {
            if (axlegear == null)
            {
                return null;
            }

            return new AxleGearData()
            {
                InputData = axlegear,
				Manufacturer = axlegear.Manufacturer,
				ModelName = axlegear.Model,
				CertificationNumber = axlegear.CertificationNumber,
				Date = axlegear.Date,
                LineType = axlegear.LineType,
                AxleGear = new TransmissionData()
                {
                    Ratio = axlegear.Ratio,
				}
            };
        }

        public static GearboxData CreateMockupGearboxData(IGearboxDeclarationInputData gearbox)
        {
            if (gearbox == null)
            {
                return null;
            }

			var gears = new Dictionary<uint, GearData>();
            
            foreach (var gearInputData in gearbox.Gears) {
				gears.Add((uint)gearInputData.Gear, new GearData() {
                    Ratio = gearInputData.Ratio,
                    MaxTorque = gearInputData.MaxTorque,
                    MaxSpeed = gearInputData.MaxInputSpeed,
				});
			}
				
			return new GearboxData()
            {
                InputData = gearbox,
                Type = gearbox.Type,
				Manufacturer = gearbox.Manufacturer,
				ModelName = gearbox.Model,
				CertificationNumber = gearbox.CertificationNumber,
				Date = gearbox.Date,
                Gears = gears
			};
        }


        public static CombustionEngineData CreateMockupEngineData(IVehicleDeclarationInputData vehicleData, int? modeIdx, TankSystem? tankSystem = null)
		{

			var engine = vehicleData.Components.EngineInputData;
            if (engine == null || modeIdx == null)
            {
                return null;
            }

            var engineModes = engine.EngineModes;
            var engineMode = engineModes[modeIdx.Value];
            var fuels = new List<CombustionEngineFuelData>();
            foreach (var fuel in engineMode.Fuels)
            {
                fuels.Add(new CombustionEngineFuelData()
                {
                    FuelData = DeclarationData.FuelData.Lookup(fuel.FuelType, tankSystem ?? vehicleData.TankSystem)
                });
            }

			var componentData = vehicleData.Components.EngineInputData;
            return new CombustionEngineData()
            {
                Fuels = fuels,
                RatedPowerDeclared = vehicleData.Components.EngineInputData.RatedPowerDeclared,
                IdleSpeed = vehicleData.EngineIdleSpeed,
                InputData = vehicleData.Components.EngineInputData,
                WHRType = vehicleData.Components.EngineInputData.WHRType,
                RatedSpeedDeclared = engine.RatedSpeedDeclared,
                Displacement = engine.Displacement,

                Manufacturer = componentData.Manufacturer,
                ModelName = componentData.Model,
                CertificationNumber = componentData.CertificationNumber,
                Date = componentData.Date,


            };
        }

		private VehicleData CreateExemptedMockupVehicleData(IVehicleDeclarationInputData vehicleData, Segment segment)
		{
			return new VehicleData() {
				InputData = vehicleData,
				SleeperCab = vehicleData.SleeperCab,
				//Loading = loading.Value.Item1,
				VehicleClass = segment.VehicleClass,
				OffVehicleCharging = vehicleData.OVC,
				VehicleCategory = vehicleData.VehicleCategory,
				ZeroEmissionVehicle = vehicleData.ZeroEmissionVehicle,
				//ADAS = CreateMockupAdasData(vehicleData),

				Manufacturer = vehicleData.Manufacturer,
				ManufacturerAddress = vehicleData.ManufacturerAddress,
				ModelName = vehicleData.Model,
				VIN = vehicleData.VIN,
				LegislativeClass = vehicleData.LegislativeClass,
				AxleConfiguration = vehicleData.AxleConfiguration,
				Date = vehicleData.Date,

			};
		}

        public static VehicleData CreateMockupVehicleData(IVehicleDeclarationInputData vehicleData, Segment segment,
			KeyValuePair<LoadingType, Tuple<Kilogram, double?>> loading)
        {
            return new VehicleData()
            {

                InputData = vehicleData,
                SleeperCab = vehicleData.SleeperCab,
                Loading = loading.Value.Item1,
                VehicleClass = segment.VehicleClass,
                OffVehicleCharging = vehicleData.OVC,
                VehicleCategory = vehicleData.VehicleCategory,
                ZeroEmissionVehicle = vehicleData.ZeroEmissionVehicle,
                ADAS = CreateMockupAdasData(vehicleData),
                
                Manufacturer = vehicleData.Manufacturer,
                ManufacturerAddress = vehicleData.ManufacturerAddress,
                ModelName = vehicleData.Model,
                VIN = vehicleData.VIN,
                LegislativeClass = vehicleData.LegislativeClass,
                AxleConfiguration = vehicleData.AxleConfiguration,
                Date = vehicleData.Date,
			};
        }

        public static VehicleData.ADASData CreateMockupAdasData(IVehicleDeclarationInputData vehicleData)
        {
            var adas = vehicleData.ADAS;
            return new VehicleData.ADASData()
            {
                EcoRoll = adas.EcoRoll,
                EngineStopStart = adas.EngineStopStart,
                InputData = adas,
                PredictiveCruiseControl = adas.PredictiveCruiseControl,
            };
        }
    }
}
