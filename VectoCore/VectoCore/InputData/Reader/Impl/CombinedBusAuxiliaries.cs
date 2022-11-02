using System.Collections.Generic;
using System.Linq;
using System.Xml;
using TUGraz.VectoCommon.BusAuxiliaries;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Utils;

namespace TUGraz.VectoCore.InputData.Reader.Impl {
	//public class CombinedBusAuxiliaries : IBusAuxiliariesDeclarationData, IElectricSupplyDeclarationData, IPneumaticSupplyDeclarationData, IElectricConsumersDeclarationData, IPneumaticConsumersDeclarationData, IHVACBusAuxiliariesDeclarationData
	//{
	//	private IBusAuxiliariesDeclarationData CompletedAux;
	//	private IBusAuxiliariesDeclarationData PrimaryAux;

	//	public CombinedBusAuxiliaries(IBusAuxiliariesDeclarationData primary, IBusAuxiliariesDeclarationData completed)
	//	{
	//		PrimaryAux = primary;
	//		CompletedAux = completed;
	//	}

	//	#region Implementation of IBusAuxiliariesDeclarationData

	//	public XmlNode XMLSource { get { return null; } }
	//	public string FanTechnology { get { return PrimaryAux.FanTechnology; } }
	//	public IList<string> SteeringPumpTechnology { get { return PrimaryAux.SteeringPumpTechnology; } }
	//	public IElectricSupplyDeclarationData ElectricSupply { get { return this; } }
	//	public IElectricConsumersDeclarationData ElectricConsumers { get { return this; } }
	//	public IPneumaticSupplyDeclarationData PneumaticSupply { get { return this; } }
	//	public IPneumaticConsumersDeclarationData PneumaticConsumers { get { return this; } }
	//	public IHVACBusAuxiliariesDeclarationData HVACAux { get { return this; } }

	//	#endregion

	//	#region Implementation of IElectricSupplyDeclarationData

	//	public IList<IAlternatorDeclarationInputData> Alternators
	//	{
	//		get { return PrimaryAux.ElectricSupply.Alternators.Concat(CompletedAux.ElectricSupply.Alternators).ToList(); }
	//	}

	//	public bool SmartElectrics { get; }
	//	public Watt MaxAlternatorPower { get; }
	//	public WattSecond ElectricStorageCapacity { get; }

	//	#endregion

	//	#region Implementation of IPneumaticSupplyDeclarationData

	//	public CompressorDrive CompressorDrive { get; }
	//	public string Clutch { get; }
	//	public double Ratio { get; }
	//	public string CompressorSize { get; }
	//	public bool SmartAirCompression { get; }
	//	public bool SmartRegeneration { get; }

	//	#endregion

	//	#region Implementation of IElectricConsumersDeclarationData

	//	public bool? InteriorLightsLED { get; }
	//	public bool? DayrunninglightsLED { get; }
	//	public bool? PositionlightsLED { get; }
	//	public bool? HeadlightsLED { get; }
	//	public bool? BrakelightsLED { get; }

	//	#endregion

	//	#region Implementation of IPneumaticConsumersDeclarationData

	//	public ConsumerTechnology AirsuspensionControl { get; }
	//	public ConsumerTechnology AdBlueDosing { get; }
	//	public ConsumerTechnology DoorDriveTechnology { get; }

	//	#endregion

	//	#region Implementation of IHVACBusAuxiliariesDeclarationData

	//	public BusHVACSystemConfiguration? SystemConfiguration { get; }
	//	public HeatPumpType? HeatPumpTypeDriverCompartment { get; }
	//	public HeatPumpMode? HeatPumpModeDriverCompartment { get; }
	//	public HeatPumpType? HeatPumpTypePassengerCompartment { get; }
	//	public HeatPumpMode? HeatPumpModePassengerCompartment { get; }
	//	public Watt AuxHeaterPower { get; }
	//	public bool? DoubleGlazing { get; }
	//	public bool HeatPump { get; }
	//	public bool? OtherHeatingTechnology { get; }
	//	public bool? AdjustableCoolantThermostat { get; }
	//	public bool? AdjustableAuxiliaryHeater { get; }
	//	public bool EngineWasteGasHeatExchanger { get; }
	//	public bool? SeparateAirDistributionDucts { get; }
	//	public bool? WaterElectricHeater { get; }
	//	public bool? AirElectricHeater { get; }

	//	#endregion
	//}
}