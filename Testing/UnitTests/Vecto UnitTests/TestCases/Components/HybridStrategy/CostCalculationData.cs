using System.Text.RegularExpressions;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.SimulationComponent.Impl;

namespace TUGraz.Vecto.UnitTests.TestCases.Components.HybridStrategy;

public record CostCalculationData
{
	public ModelParams ModelParams { get; init; } = new ModelParams();

	public PowertrainInfo PowertrainInfo { get; init; } = new PowertrainInfo();

	public ResponseValues Response { get; init; } = new ResponseValues();

	public ExpectedValues Expected { get; init; } = new ExpectedValues();

	public Second AbsTime { get; init; } = 0.SI<Second>();

	public Second dt { get; init; } = 1.SI<Second>();
	public bool AllowICEOff { get; init; } = false;
	public bool DryRun { get; init; } = false;
}

public record ModelParams
{
	public PerSecond EngineIdleSpeed { get; init; } = 600.RPMtoRad();
	public KilogramSquareMeter Inertia { get; init; } = 2.5.SI<KilogramSquareMeter>();
	public Second EngineStartTime { get; init; } = 2.SI<Second>();
	public Second HybridStrategyMinIceOnTime { get; init; } = 10.SI<Second>();
	public int GearshiftParamsAllowedGearRange { get; init; } = 2;
	public uint GearboxNumGears { get; init; } = 6;
	public KilogramPerSecond FuelConsumption { get; init; }
	public WattSecond BatteryDischargeEnergyThreshold { get; set; }
}

public record PowertrainInfo
{
	public bool CombustionEngineOn { get; init; } = true;
	public PerSecond EngineSpeed { get; init; }
	public PerSecond EngineN95hSpeed { get; init; } = 2200.RPMtoRad();
	public DrivingAction DrivingAction { get; init; } = DrivingAction.Accelerate;
	public WattSecond BatteryStoredEnergy { get; init; } = (600.SI<Volt>() * 90.SI<AmpereSecond>()).Cast<WattSecond>();
	public double BatterySoC { get; init; } = 0.5;
	public uint GearboxCurrentGear { get; init; } = 3;
	public bool PreviousStateGearboxEngaged { get; set; }
}

public record ResponseValues
{
	public uint ResponseGear { get; init; } = 3;
	public NewtonMeter EngineTorqueOutDemand { get; init; }
	public NewtonMeter EngineDynamicFullLoadTorque { get; init; } = 1500.SI<NewtonMeter>();
	public Watt EngineDynamicFulloadPower { get; init; } = 160000.SI<Watt>();
	public Watt EngineDragPower { get; init; } = 5000.SI<Watt>();
	public NewtonMeter EngineTotalTorqueDemand { get; init; } = 800.SI<NewtonMeter>();
	public PerSecond EngineSpeed { get; init; } = 1200.RPMtoRad();
	public Watt REESSPowerDemand { get; init; } = 40000.SI<Watt>();
	public Watt REESSResponsePowerDemand { get; init; } = 38000.SI<Watt>();
	public Watt REESSResponseMaxDischargePower { get; init; } = 500000.SI<Watt>();
	public Watt REESSResponseMaxChargePower { get; init; } = 500000.SI<Watt>();
}

public record ExpectedValues
{
	public double BatCosts { get; init; }
	public double FuelCosts { get; init; }
	public double Cost { get; init; }
	public HybridConfigurationIgnoreReason IngoreReason { get; init; }
};


public static class CostCalculationDataExtension
{
	public static string HEADER = new string[] {
		"AbsTime", "dt", "AllowICEOff", "DryRun",
        "EngineIdleSpeed", "Inertia", "EngineStartTime", "HybridStrategyMinIceOnTime", "GearshiftParamsAllowedGearRange", "GearboxNumGears", "FuelConsumption", "BatteryDischargeEnergyThreshold",
        "CombustionEngineOn", "EngineSpeed", "EngineN95hSpeed", "DrivingAction", "BatteryStoredEnergy", "BatterySoC", "GearboxCurrentGear", "PreviousStateGearboxEngaged",
        "ResponseGear", "EngineTorqueOutDemand", "EngineDynamicFullLoadTorque", "EngineDynamicFulloadPower", "EngineDragPower", "EngineTotalTorqueDemand", "EngineSpeed", 
		"REESSPowerDemand", "REESSResponsePowerDemand","REESSResponseMaxDischargePower", "REESSResponseMaxChargePower",
        "BatCosts", "FuelCosts", "Cost", "IngoreReason",
    }.Join("; ");

	public static string GetString(this CostCalculationData calcData)
	{
		var reqParams = new object[] { calcData.AbsTime.Value(), calcData.dt.Value(), calcData.AllowICEOff ? 1 : 0, calcData.DryRun ? 1 : 0 }.Join("; ");
		var model = calcData.ModelParams;
		var modelParams = new object[] {
			model.EngineIdleSpeed.AsRPM, model.Inertia.Value(), model.EngineStartTime.Value(), model.HybridStrategyMinIceOnTime.Value(),
			model.GearshiftParamsAllowedGearRange, model.GearboxNumGears, model.FuelConsumption?.ConvertToGrammPerHour().Value, model.BatteryDischargeEnergyThreshold.Value()
		}.Join("; ");
		var pt = calcData.PowertrainInfo;
		var powertrainParams = new object[] {
			pt.CombustionEngineOn ? 1 : 0, pt.EngineSpeed.AsRPM, pt.EngineN95hSpeed.AsRPM, pt.DrivingAction, pt.BatteryStoredEnergy.Value(), pt.BatterySoC,
			pt.GearboxCurrentGear, pt.PreviousStateGearboxEngaged ? 1 : 0,
		}.Join("; ");
		var resp = calcData.Response;
		var respParams = new object[] {
			resp.ResponseGear, resp.EngineTorqueOutDemand.Value(), resp.EngineDynamicFullLoadTorque.Value(), resp.EngineDynamicFulloadPower.Value(),
			resp.EngineDragPower.Value(),
			resp.EngineTotalTorqueDemand.Value(), resp.EngineSpeed.AsRPM, resp.REESSPowerDemand.Value(), resp.REESSResponsePowerDemand.Value(),
			resp.REESSResponseMaxDischargePower.Value(), resp.REESSResponseMaxChargePower.Value()
		}.Join("; ");
		var expected = calcData.Expected;
		var expectedParams = new object[]
			{ expected.BatCosts, expected.FuelCosts, expected.Cost, expected.IngoreReason }.Join("; ");
		var line = new[] { reqParams, modelParams, powertrainParams, respParams, expectedParams }.Join("; ");
		return line;
	}

	public static CostCalculationData ParseString(string str)
	{
		var parts = str.Split(';');
		const int numColumns = 35;
		if (parts.Length != numColumns) {
			throw new Exception($"Invalid number of columns. expected {numColumns}, got {parts.Length}");
		}

		var i = 0;
		var data = new CostCalculationData()
		{
			AbsTime = parts[i++].ToDouble().SI<Second>(),
			dt = parts[i++].ToDouble().SI<Second>(),
			AllowICEOff = parts[i++].ToBoolean(),
			DryRun = parts[i++].ToBoolean(),
			ModelParams = new ModelParams() {
				EngineIdleSpeed = parts[i++].ToDouble().RPMtoRad(),
				Inertia = parts[i++].ToDouble().SI<KilogramSquareMeter>(),
				EngineStartTime = parts[i++].ToDouble().SI<Second>(),
				HybridStrategyMinIceOnTime = parts[i++].ToDouble().SI<Second>(),
				GearshiftParamsAllowedGearRange = parts[i++].ToInt(),
				GearboxNumGears = (uint)parts[i++].ToInt() ,
				FuelConsumption = string.IsNullOrWhiteSpace(parts[i++]) ? null : parts[i-1].ToDouble().SI(Unit.SI.Gramm.Per.Hour).Cast<KilogramPerSecond>(),
				BatteryDischargeEnergyThreshold = parts[i++].ToDouble().SI<WattSecond>()
			},
			PowertrainInfo = new PowertrainInfo() {
				CombustionEngineOn = parts[i++].ToBoolean(),
				EngineSpeed = parts[i++].ToDouble().RPMtoRad(),
				EngineN95hSpeed = parts[i++].ToDouble().RPMtoRad(),
				DrivingAction = parts[i++].ParseEnum<DrivingAction>(),
				BatteryStoredEnergy = parts[i++].ToDouble().SI<WattSecond>(),
				BatterySoC = parts[i++].ToDouble(),
				GearboxCurrentGear = (uint)parts[i++].ToInt(),
				PreviousStateGearboxEngaged = parts[i++].ToBoolean(),
			}, 
			Response = new ResponseValues() {
				ResponseGear = (uint)parts[i++].ToInt(),
				EngineTorqueOutDemand = parts[i++].ToDouble().SI<NewtonMeter>(),
				EngineDynamicFullLoadTorque = parts[i++].ToDouble().SI<NewtonMeter>(),
				EngineDynamicFulloadPower = parts[i++].ToDouble().SI<Watt>(),
				EngineDragPower = parts[i++].ToDouble().SI<Watt>(),
				EngineTotalTorqueDemand = parts[i++].ToDouble().SI<NewtonMeter>(),
				EngineSpeed = parts[i++].ToDouble().RPMtoRad(),
				REESSPowerDemand = parts[i++].ToDouble().SI<Watt>(),
				REESSResponsePowerDemand = parts[i++].ToDouble().SI<Watt>(),
				REESSResponseMaxDischargePower = parts[i++].ToDouble().SI<Watt>(),
				REESSResponseMaxChargePower = parts[i++].ToDouble().SI<Watt>()
			},
			Expected = new ExpectedValues() {
				BatCosts = parts[i++].ToDouble(),
				FuelCosts = parts[i++].ToDouble(),
				Cost = parts[i++].ToDouble(),
				IngoreReason = parts[i++].ParseEnumFields<HybridConfigurationIgnoreReason>()
			}
		};

		return data;
	}

	
}

public static class EnumHelperUnitTest
{
	public static T ParseEnumFields<T>(this string s, bool ignoreCase = true) =>
		(T)Enum.Parse(typeof(T),
			s.StartsWith("-")
				? Regex.Replace(s, "[^,a-zA-Z0-9_-]", "")
				: Regex.Replace(s, "[^,a-zA-Z0-9_]", ""), ignoreCase);

	public static T ParseEnumFields<T>(this object o, bool ignoreCase = true) =>
		o is string s ? ParseEnumFields<T>(s, ignoreCase) : (T)o;


}