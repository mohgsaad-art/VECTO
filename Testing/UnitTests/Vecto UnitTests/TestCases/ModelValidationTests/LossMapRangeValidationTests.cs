using System.ComponentModel.DataAnnotations;
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
using Assert = NUnit.Framework.Assert;

namespace TUGraz.Vecto.UnitTests.TestCases.ModelValidationTests;

public class LossMapRangeValidationTests
{
    /// <summary>
    /// VECTO-173
    /// </summary>
    [TestCase]
    public void LossMapValid()
    {
        var gearboxData = CreateGearboxData();
        var engineData = GetDummyEngineData(gearboxData.Gears.Count);
        var axleGearData = CreateAxleGearData();
		var runData = GetVectoRunData(gearboxData, engineData, axleGearData);

        var result = VectoRunData.ValidateRunData(runData, new ValidationContext(runData));
        Assert.IsTrue(ValidationResult.Success == result);
        Assert.IsTrue(runData.IsValid(), GetValidationMessage(runData));
		
    }

	private static string GetValidationMessage(VectoRunData runData)
	{
		return runData.Validate(runData.ExecutionMode, runData.JobType, null, runData.GearboxData?.Type, false).Select(x => x.ErrorMessage).Join();
	}


	/// <summary>
    /// VECTO-173
    /// </summary>
    [TestCase]
    public void LossMapInvalidAxle()
    {
        var gearboxData = CreateGearboxData();
		var engineData = GetDummyEngineData(gearboxData.Gears.Count);
        var axleGearData = CreateAxleGearData();
		axleGearData.AxleGear.LossMap =
			TransmissionLossMapReader.Create(InputDataHelper.InputDataAsTableData(LossMapHdr, LossMapDataLimited), 2.59,
				"AxleGear");

		var runData = GetVectoRunData(gearboxData, engineData, axleGearData);
		var result = VectoRunData.ValidateRunData(runData, new ValidationContext(runData));
        Assert.IsFalse(runData.IsValid(), GetValidationMessage(runData));
		Assert.IsNotNull(result.ErrorMessage);
		Assert.IsTrue(result.ErrorMessage.Contains("Interpolation of AxleGear-LossMap failed"), result.ErrorMessage);
    }

    /// <summary>
    /// VECTO-173
    /// </summary>
    [TestCase]
    public void LossMapLimited()
    {
        var gearboxData = CreateGearboxData();
		gearboxData.Gears.Where(x => x.Value.Ratio.Equals(1.0)).FirstOrDefault().Value.LossMap =
			TransmissionLossMapReader.Create(InputDataHelper.InputDataAsTableData(LossMapHdr, LossMapDataLimited), 1.0,
				"Gear 1");

		var engineData = GetDummyEngineData(gearboxData.Gears.Count);
        var axleGearData = CreateAxleGearData();
        var runData = GetVectoRunData(gearboxData, engineData, axleGearData);
		var result = VectoRunData.ValidateRunData(runData, new ValidationContext(runData));
        Assert.IsFalse(ValidationResult.Success == result);
		Assert.IsNotNull(result.ErrorMessage);
		Assert.IsTrue(result.ErrorMessage.Contains("Interpolation of Gear-12-LossMap failed"), result.ErrorMessage);
    }

	

	/// <summary>
    /// VECTO-173
    /// </summary>
    [TestCase]
    public void LossMapAxleLossMapMissing()
    {
        var gearboxData = CreateGearboxData();
		var engineData = GetDummyEngineData(gearboxData.Gears.Count);
		var runData = GetVectoRunData(gearboxData, engineData, null);

        var result = VectoRunData.ValidateRunData(runData, new ValidationContext(runData));
        Assert.IsFalse(ValidationResult.Success == result);
        Assert.IsFalse(runData.IsValid(), GetValidationMessage(runData));
		Assert.IsNotNull(result.ErrorMessage);
		Assert.IsTrue(result.ErrorMessage.Contains("Axlegear data is required for conventional and parallel hybrid vehicles!"), result.ErrorMessage);
    }

    /// <summary>
    /// VECTO-173
    /// </summary>
    [TestCase]
    public void LossMapGearLossMapMissing()
    {
		var engineData = GetDummyEngineData(0);
        var axleGearData = CreateAxleGearData();

		var runData = GetVectoRunData(null, engineData, axleGearData);

        var context = new ValidationContext(runData);
        //context.InitializeServiceProvider(type =>
        //    new VectoValidationModeServiceContainer(ExecutionMode.Declaration, VectoSimulationJobType.ConventionalVehicle, PowertrainPosition.HybridPositionNotSet, GearboxType.AMT, false));

        var result = VectoRunData.ValidateRunData(runData, context);
        Assert.IsFalse(ValidationResult.Success == result);
        Assert.IsFalse(runData.IsValid(), GetValidationMessage(runData));
		Assert.IsNotNull(result.ErrorMessage);
		Assert.IsTrue(result.ErrorMessage.Contains("Gearbox data is required for conventional and parallel hybrid vehicles!"), result.ErrorMessage);
    }

	private static GearboxData CreateGearboxData()
	{
		var ratios = new[] { 14.93, 11.64, 9.02, 7.04, 5.64, 4.4, 3.39, 2.65, 2.05, 1.6, 1.28, 1.0 };
		var gearboxInput = new Mock<IGearboxDeclarationInputData>();
		var gears = new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 }
			.Select(x => new TransmissionInputData() { Gear = x }).Cast<ITransmissionInputData>().ToList();
		gearboxInput.Setup(g => g.Gears).Returns(gears);
		return new GearboxData {
			Gears = ratios.Select((ratio, i) =>
					Tuple.Create((uint)i,
						new GearData {
							//								MaxTorque = 2300.SI<NewtonMeter>(),
							LossMap = TransmissionLossMapReader.Create(ratio.IsEqual(1.0) ? 0.98 : 0.96, ratio,
								$"Gear {i}"),
							Ratio = ratio,
							ShiftPolygon = ShiftPolygonReader.Create(InputDataHelper.InputDataAsTableData(ShiftPolyHdr, ShiftPolyData))
						}))
				.ToDictionary(k => k.Item1 + 1, v => v.Item2),
			InputData = gearboxInput.Object,
			Inertia = 0.SI<KilogramSquareMeter>(),
			TractionInterruption = 1.SI<Second>()
		};
	}

	private static AxleGearData CreateAxleGearData()
	{
		const double ratio = 2.59;
		return new AxleGearData {
			AxleGear = new GearData {
				Ratio = ratio,
				LossMap = TransmissionLossMapReader.Create(0.96, ratio, "AxleGear")
			}
		};
	}


	private CombustionEngineData GetDummyEngineData(int gearsCount)
	{
		var retVal = new CombustionEngineData() {
			IdleSpeed = 560.RPMtoRad(),
			Inertia = 5.1.SI<KilogramSquareMeter>(),
			FullLoadCurves = new Dictionary<uint, EngineFullLoadCurve>(),
			Displacement = 5.5.SI(Unit.SI.Liter).Cast<CubicMeter>(),
			EngineStartTime = 1.SI<Second>(),
			Fuels = new List<CombustionEngineFuelData>() { new CombustionEngineFuelData() {
				ColdHotCorrectionFactor = 1.0,
				ConsumptionMap = FuelConsumptionMapReader.Create(InputDataHelper.InputDataAsTableData(FcMapHdr, FcMapData)),
				CorrectionFactorRegPer = 1.0,
				FuelConsumptionCorrectionFactor = 1.0,
				FuelData = FuelData.Diesel,
				WHTCMotorway = 1.0,
				WHTCRural = 1.0,
				WHTCUrban = 1.0,
			}}
		};
		for (var i = 0u; i <= gearsCount; i++) {
			retVal.FullLoadCurves[i] =
				FullLoadCurveReader.Create(InputDataHelper.InputDataAsTableData(EngineFldHdr, EngineFldData));
		}
		return retVal;
	}

	private static VectoRunData GetVectoRunData(GearboxData gearboxData, CombustionEngineData engineData, AxleGearData axleGearData)
	{
		var mockVehicleInputData = new Mock<IVehicleDeclarationInputData>();
		var vehicleData = new VehicleData {
			DynamicTyreRadius = 0.85.SI<Meter>(),
			Loading = 0.SI<Kilogram>(),
			CurbMass = 2000.SI<Kilogram>(),
			GrossVehicleMass = 7500.SI<Kilogram>(),
			TrailerGrossVehicleMass = 0.SI<Kilogram>(),
			AirDensity = DeclarationData.AirDensity,
			AxleConfiguration = AxleConfiguration.AxleConfig_4x2,
			AxleData =
				new List<Axle> {
					new Axle {
						TwinTyres = false,
						AxleType = AxleType.VehicleNonDriven,
						AxleWeightShare = 0.5,
						TyreTestLoad = 50000.SI<Newton>(),
						Inertia = 10.SI<KilogramSquareMeter>(),
						RollResistanceCoefficient = 0.0065,
						WheelsDimension = "225/75 R17.5"
                    },
					new Axle {
						TwinTyres = true,
						AxleType = AxleType.VehicleDriven,
						AxleWeightShare = 0.5,
						TyreTestLoad = 50000.SI<Newton>(),
						Inertia = 10.SI<KilogramSquareMeter>(),
						RollResistanceCoefficient = 0.0065,
						WheelsDimension = "225/75 R17.5"
					}
                },
			InputData = mockVehicleInputData.Object
		};
		var runData = new VectoRunData {
			JobType = VectoSimulationJobType.ConventionalVehicle,
			GearboxData = gearboxData, EngineData = engineData, AxleGearData = axleGearData, VehicleData = vehicleData,
			JobName = "ValidationJob",
			Cycle = DrivingCycleDataReader.ReadFromDataTable(InputDataHelper.InputDataAsTableData(CycleHdr, CycleData), "DummyCycle", false)
		};
		return runData;
	}

    const string ShiftPolyHdr = "M [Nm],nDown [rpm],nUp [rpm]";

	private static readonly string[] ShiftPolyData = new[] {
		"-400,560,1289",
		"759,560,1289",
		"1252,742,1289",
		"2372,1155,1942",
	};

    const string LossMapHdr = "Input Speed [rpm],Input Torque [Nm],Torque Loss [Nm]";

	private static readonly string[] LossMapDataLimited = new[] {
		"0,-2500,77.5",
		"0,-500,47.5",
		"0,500,47.5",
		"0,15500,272.5",
		"200,-2500,77.5",
		"200,-500,47.5",
		"200,500,47.5",
		"200,15500,272.5",
	};

	const string EngineFldHdr = "engine speed [1/min],full load torque [Nm],motoring torque [Nm],PT1 [s]";

	private static readonly string[] EngineFldData = new[] {
		"560,1180,-149,0.6",
		"600,1282,-148,0.6",
		"799.9999999,1791,-149,0.6",
		"1000,2300,-160,0.6",
		"1200,2300,-179,0.6",
		"1400,2300,-203,0.6",
		"1599.999999,2079,-235,0.49",
		"1800,1857,-264,0.25",
		"2000.000001,1352,-301,0.25",
		"2100,1100,-320,0.25",
	};

	const string FcMapHdr = "engine speed [1/min],torque [Nm],fuel consumption [g/h]";

	private static readonly string[] FcMapData = new[] {
		"560,-149,0",
		"560,0,1256",
		"560,2300,12869",
		"2300,-320,0",
		"2300,0,10470",
		"2300,2300,50653",
	};

	private const string CycleHdr = "<s>,<v>,<grad>,<stop>,HW";

	private static readonly string[] CycleData = new[] {
		"0,0,-0.982323,1,1",
		"1,83,-0.982389,0,1",
		"2,83,-0.982455,0,1",
		"3,83,-0.982521,0,1",
		"4,83,-0.982587,0,1",
		"5,83,-0.982653,0,1",
		"6,83,-0.982719,0,1",
		"7,83,-0.982785,0,1",
		"8,83,-0.982851,0,1",
		"9,83,-0.982917,0,1",
		"10,83,-0.982983,0,1",
		"11,83,-0.983049,0,1",
		"12,85,-0.983115,0,1",
		"13,85,-0.983181,0,1",
		"14,85,-0.983247,0,1",
		"15,85,-0.983313,0,1",
		"16,85,-0.983379,0,1",
	};
}