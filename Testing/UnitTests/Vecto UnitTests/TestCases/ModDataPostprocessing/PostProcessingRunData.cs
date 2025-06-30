using Moq;
using TUGraz.VectoCommon.BusAuxiliaries;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.InputData.Reader.ComponentData;
using TUGraz.VectoCore.Models.BusAuxiliaries;
using TUGraz.VectoCore.Models.BusAuxiliaries.DownstreamModules.Impl.Electrics;
using TUGraz.VectoCore.Models.BusAuxiliaries.DownstreamModules.Impl.Pneumatics;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Tests.Utils;

namespace TUGraz.Vecto.UnitTests.TestCases.ModDataPostprocessing;

static internal class PostProcessingRunData
{
	public const double dcdc_efficiency = 0.926;
	public const PowertrainPosition emPos = PowertrainPosition.HybridP2;

    public const double UF_ESS_Driving = 0.821;
	public const double UF_ESS_Standstill = 0.753;
	public const double busAuxAlternatorEff = 0.753;

    public static VectoRunData GetRunData(bool withBusAux = false, bool smartCompressor = false, AlternatorType alternatorType = AlternatorType.Conventional)
	{
		var fcMapHeader = "engine speed [rpm],torque [Nm],fuel consumption [g/h]";
		var fcMapEntries = new[] {
			"600,-107,0",
			"600,0,1042",
			"600,916,10963",
			"2000,-215,0",
			"2000,0,6519",
			"2000,966,40437"
		};

		var retVal = new VectoRunData() {
			DriverData = new DriverData() {
				EngineStopStart = new DriverData.EngineStopStartData() {
					UtilityFactorDriving = UF_ESS_Driving,
					UtilityFactorStandstill = UF_ESS_Standstill,
				}
			},
			EngineData = new CombustionEngineData() {
				IdleSpeed = 600.RPMtoRad(),
				Fuels = new List<CombustionEngineFuelData>() {
					new CombustionEngineFuelData() {
						FuelData = FuelData.Diesel,
						FuelConsumptionCorrectionFactor = 1.012,
						ConsumptionMap = FuelConsumptionMapReader.ReadFromStream(InputDataHelper.InputDataAsStream(fcMapHeader, fcMapEntries))
					}
				}
			},
			Cycle = new DrivingCycleData() {
				CycleType = CycleType.DistanceBased
			}
		};

		if (withBusAux) {
			retVal.BusAuxiliaries = new AuxiliaryConfig() {
				ElectricalUserInputsConfig = new ElectricsUserInputsConfig() {
					ConnectESToREESS = true,
					AlternatorMap = new SimpleAlternator(busAuxAlternatorEff),
					AlternatorType = alternatorType,
					AlternatorGearEfficiency = 1,
					DCDCEfficiency = dcdc_efficiency,
				},
				PneumaticAuxiliariesConfig = CreatePneumaticAuxConfig(0.7.SI<NormLiterPerSecond>()),
				PneumaticUserInputsConfig = CreatePneumaticUserInputsConfig(smartCompressor),
				Actuations = new Actuations() {
					Braking = 0,
					Kneeling = 0,
					ParkBrakeAndDoors = 0,
					CycleTime = 1.SI<Second>()
				},
				VehicleData = new VehicleData() {
					CurbMass = 14000.SI<Kilogram>()
				}
			};
			retVal.ElectricMachinesData = new List<Tuple<PowertrainPosition, ElectricMotorData>>() {
				Tuple.Create(emPos, new ElectricMotorData() { Overload = new OverloadData() {OverloadBuffer = 0.SI<Joule>()}})
			};
		}

		return retVal;
	}

	static IPneumaticsConsumersDemand CreatePneumaticAuxConfig(NormLiterPerSecond averageAirDemand)
	{
		return new PneumaticsConsumersDemand() {
			AdBlueInjection = 0.SI<NormLiterPerSecond>(),
			AirControlledSuspension = averageAirDemand,
			Braking = 0.SI<NormLiterPerKilogram>(),
			BreakingWithKneeling = 0.SI<NormLiterPerKilogramMeter>(),
			DeadVolBlowOuts = 0.SI<PerSecond>(),
			DeadVolume = 0.SI<NormLiter>(),
			NonSmartRegenFractionTotalAirDemand = 0,
			SmartRegenFractionTotalAirDemand = 0,
			OverrunUtilisationForCompressionFraction =
				Constants.BusAuxiliaries.PneumaticConsumersDemands.OverrunUtilisationForCompressionFraction,
			DoorOpening = 0.SI<NormLiter>(),
			StopBrakeActuation = 0.SI<NormLiterPerKilogram>(),
		};
	}

	static PneumaticUserInputsConfig CreatePneumaticUserInputsConfig(bool smartCompressor)
	{
		var mock = new Mock<IPneumaticSupplyDeclarationData>();
		mock.Setup(x => x.CompressorDrive).Returns(CompressorDrive.mechanically);
		mock.Setup(x => x.CompressorSize).Returns("Medium Supply 2-stage");
		mock.Setup(x => x.Clutch).Returns("visco");

		return new PneumaticUserInputsConfig() {
			CompressorMap =
				DeclarationData.BusAuxiliaries.GetCompressorMap(mock.Object),
			CompressorGearEfficiency = Constants.BusAuxiliaries.PneumaticUserConfig.CompressorGearEfficiency,
			CompressorGearRatio = 1.0,
			SmartAirCompression = smartCompressor,
			SmartRegeneration = false,
			KneelingHeight = 0.SI<Meter>(),
			AirSuspensionControl = ConsumerTechnology.Pneumatically,
			AdBlueDosing = ConsumerTechnology.Electrically,
			Doors = ConsumerTechnology.Electrically
		};
	}
}