using System.Diagnostics;
using Moq;
using NUnit.Framework;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.Impl;
using TUGraz.VectoCore.InputData.Reader.ComponentData;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Engine;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Gearbox;
using TUGraz.VectoCore.Tests.Utils;
using TUGraz.VectoCore.Utils;
using Assert = NUnit.Framework.Assert;

namespace TUGraz.Vecto.UnitTests.TestCases.ModelValidationTests;

public class ValidationModeTests
{
	[TestCase]
	public void ValidationModeVehicleDataTest()
	{
		var vehicleData = new VehicleData {
			AxleConfiguration = AxleConfiguration.AxleConfig_4x2,
			AirDensity = DeclarationData.AirDensity,
			CurbMass = 7500.SI<Kilogram>(),
			DynamicTyreRadius = 0.5.SI<Meter>(),
			//CurbWeigthExtra = 0.SI<Kilogram>(),
			Loading = 12000.SI<Kilogram>(),
			GrossVehicleMass = 16000.SI<Kilogram>(),
			TrailerGrossVehicleMass = 0.SI<Kilogram>(),
			AxleData = new List<Axle> {
				new Axle {
					AxleType = AxleType.VehicleNonDriven,
					AxleWeightShare = 0.4,
					Inertia = 0.5.SI<KilogramSquareMeter>(),
					RollResistanceCoefficient = 0.00555,
					TyreTestLoad = 33000.SI<Newton>()
				},
				new Axle {
					AxleType = AxleType.VehicleNonDriven,
					AxleWeightShare = 0.6,
					Inertia = 0.5.SI<KilogramSquareMeter>(),
					RollResistanceCoefficient = 0.00555,
					TyreTestLoad = 33000.SI<Newton>()
				},
			}
		};
		var result = vehicleData.Validate(ExecutionMode.Engineering, VectoSimulationJobType.ConventionalVehicle, null, null, false);
		Assert.IsTrue(!result.Any(), "validation should have succeded but failed." + string.Concat(result));
		// Clear of History -> Normally happens inside the SimulatorFactory
		ValidationHelper.ClearValHistory();
		result = vehicleData.Validate(ExecutionMode.Declaration, VectoSimulationJobType.ConventionalVehicle, null, null, false);
		Assert.IsTrue(result.Any(), "validation should have failed, but succeeded." + string.Concat(result));
	}

    /// <summary>
    /// VECTO-107 Check valid range of input parameters
    /// </summary>
    [TestCase]
    public void ValidationModeVectoRunDataTest()
    {
        var engineData = new CombustionEngineData {
            FullLoadCurves =
                new Dictionary<uint, EngineFullLoadCurve>() {
                        { 0, FullLoadCurveReader.Create(InputDataHelper.InputDataAsTableData(EngineFldHeader, EngineFldData)) },
                        { 1, FullLoadCurveReader.Create(InputDataHelper.InputDataAsTableData(EngineFldHeader, EngineFldData)) },
                },
            IdleSpeed = 560.RPMtoRad(),
            Displacement = 5.5.SI(Unit.SI.Liter).Cast<CubicMeter>(),
            Inertia = 3.SI<KilogramSquareMeter>(),
            Fuels = new List<CombustionEngineFuelData>() {
                new CombustionEngineFuelData() {
                    FuelConsumptionCorrectionFactor = 1.0,
                    FuelData = FuelData.Diesel,
                    ConsumptionMap = new FuelConsumptionMap(new DelaunayMap("Dummy"))
				}
			},
            EngineStartTime = 1.SI<Second>(),
        };

		var gbxInput = new Mock<IGearboxDeclarationInputData>();
		gbxInput.Setup(g => g.Gears).Returns(new List<ITransmissionInputData>() {
			new TransmissionInputData() {
				Gear = 1
			},
			new TransmissionInputData() {
				Gear = 2
			}
		});
        var gearboxData = new GearboxData() {
            Inertia = 0.SI<KilogramSquareMeter>(),
            TractionInterruption = 1.SI<Second>(),
            InputData = gbxInput.Object
		};
        gearboxData.Gears[1] = new GearData {
            LossMap = TransmissionLossMapReader.Create(0.98, 1, "1"),
            Ratio = 2,
		};
		gearboxData.Gears[1] = new GearData {
			LossMap = TransmissionLossMapReader.Create(0.98, 1, "2"),
			Ratio = 1,
		};

        var axleGearData = new AxleGearData {
            AxleGear = new GearData {
                Ratio = 1,
                LossMap = TransmissionLossMapReader.Create(0.98, 1, "1"),
            }
        };
        var vehicleData = new VehicleData {
            AxleConfiguration = AxleConfiguration.AxleConfig_4x2,
            AirDensity = DeclarationData.AirDensity,
            CurbMass = 7500.SI<Kilogram>(),
            DynamicTyreRadius = 0.5.SI<Meter>(),
            //CurbWeigthExtra = 0.SI<Kilogram>(),
            Loading = 12000.SI<Kilogram>(),
            GrossVehicleMass = 16000.SI<Kilogram>(),
            TrailerGrossVehicleMass = 0.SI<Kilogram>(),
            AxleData = new List<Axle> {
                    new Axle {
                        AxleType = AxleType.VehicleNonDriven,
                        AxleWeightShare = 0.4,
                        Inertia = 0.5.SI<KilogramSquareMeter>(),
                        RollResistanceCoefficient = 0.00555,
                        TyreTestLoad = 33000.SI<Newton>(),
                        WheelsDimension = "225/70 R17.5",
                    },
                    new Axle {
                        AxleType = AxleType.VehicleNonDriven,
                        AxleWeightShare = 0.6,
                        Inertia = 0.5.SI<KilogramSquareMeter>(),
                        RollResistanceCoefficient = 0.00555,
                        TyreTestLoad = 33000.SI<Newton>(),
                        WheelsDimension = "225/70 R17.5"
                    },
                },
            InputData = new Mock<IVehicleDeclarationInputData>().Object
        };

		var runData = new VectoRunData {
            JobName = "Validation Job",
            JobRunId = 0,
			VehicleData = vehicleData,
            AirdragData = new AirdragData() {
                CrossWindCorrectionMode = CrossWindCorrectionMode.NoCorrection,
                CrossWindCorrectionCurve =
                    new CrosswindCorrectionCdxALookup(5.SI<SquareMeter>(), 0.SI<SquareMeter>(),
                        CrossWindCorrectionCurveReader.GetNoCorrectionCurve(5.SI<SquareMeter>()),
                        CrossWindCorrectionMode.NoCorrection)
            },
            GearboxData = gearboxData,
            EngineData = engineData,
            AxleGearData = axleGearData,
            Cycle = new DrivingCycleData() {
                Entries = new List<DrivingCycleData.DrivingCycleEntry>() {
                    new DrivingCycleData.DrivingCycleEntry() {
                        Time = 0.SI<Second>()
					}
				}
			}
        };
		
        var stopwatch = new Stopwatch();
		stopwatch.Start();
        var results = runData.Validate(ExecutionMode.Declaration, VectoSimulationJobType.ConventionalVehicle, null, null, false);
		stopwatch.Stop();
		Console.WriteLine(stopwatch.Elapsed + " " + stopwatch.ElapsedMilliseconds);
        Assert.IsTrue(results.Any(), "Validation should have failed, but succeded.");

        ValidationHelper.ClearValHistory();
        results = runData.Validate(ExecutionMode.Engineering, VectoSimulationJobType.ConventionalVehicle, null, null, false);
        Assert.IsTrue(!results.Any());
    }

    const string EngineFldHeader = "engine speed [1/min],full load torque [Nm],motoring torque [Nm],PT1 [s]";

	private static readonly string[] EngineFldData = new string[] {
		"600,586,-44,0.60",
		"800,755,-54,0.60",
		"1000,883,-62,0.60",
		"1200,899,-74,0.60",
		"1300,899,-80,0.60",
		"1400,899,-87,0.60",
		"1500,899,-92,0.60",
		"1600,899,-97,0.60",
		"1700,890,-100,0.60",
		"1800,881,-103,0.60",
		"1900,867,-107,0.60",
		"2000,853,-111,0.43",
		"2150,811,-118,0.29",
		"2200,802,-125,0.25",
		"2300,755,-130,0.25",
		"2400,705,-135,0.25",
		"2500,644,-140,0.25",
		"2600,479,-145,0.25",
		"2700,0,-149,0.25",
	};

    //const string GearLossMapHeader = ""
}