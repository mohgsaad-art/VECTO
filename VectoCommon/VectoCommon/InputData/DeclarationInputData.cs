/*
* This file is part of VECTO.
*
* Copyright © 2012-2019 European Union
*
* Developed by Graz University of Technology,
*              Institute of Internal Combustion Engines and Thermodynamics,
*              Institute of Technical Informatics
*
* VECTO is licensed under the EUPL, Version 1.1 or - as soon they will be approved
* by the European Commission - subsequent versions of the EUPL (the "Licence");
* You may not use VECTO except in compliance with the Licence.
* You may obtain a copy of the Licence at:
*
* https://joinup.ec.europa.eu/community/eupl/og_page/eupl
*
* Unless required by applicable law or agreed to in writing, VECTO
* distributed under the Licence is distributed on an "AS IS" basis,
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
* See the Licence for the specific language governing permissions and
* limitations under the Licence.
*
* Authors:
*   Stefan Hausberger, hausberger@ivt.tugraz.at, IVT, Graz University of Technology
*   Christian Kreiner, christian.kreiner@tugraz.at, ITI, Graz University of Technology
*   Michael Krisper, michael.krisper@tugraz.at, ITI, Graz University of Technology
*   Raphael Luz, luz@ivt.tugraz.at, IVT, Graz University of Technology
*   Markus Quaritsch, markus.quaritsch@tugraz.at, IVT, Graz University of Technology
*   Martin Rexeis, rexeis@ivt.tugraz.at, IVT, Graz University of Technology
*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Xml;
using TUGraz.VectoCommon.BusAuxiliaries;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;


namespace TUGraz.VectoCommon.InputData
{
	public interface IDeclarationJobInputData
	{
		bool SavedInDeclarationMode { get; }

		IVehicleDeclarationInputData Vehicle { get; }

		string JobName { get; }

		VectoSimulationJobType JobType { get; }
	}

	public interface IComponentInputData
	{
		DataSource DataSource { get; }

		bool SavedInDeclarationMode { get; }

		string Manufacturer { get; }

		string Model { get; }

		DateTime Date { get; }

		String AppVersion { get; }

		CertificationMethod CertificationMethod { get; }

		string CertificationNumber { get; }

		DigestData DigestValue { get; }
	}

	public class DataSource
	{
		public DataSourceType SourceType { get; set; }

		public string SourceFile { get; set; }

		/// <summary>
		/// returns the version of the namespace uri
		/// </summary>
		public string SourceVersion { get; set; }

		/// <summary>
		/// In case of components loaded from XML files, this is the name of the XSD Type
		/// </summary>
		public string Type { get; set; }

		public string TypeVersion { get; set; }

		public string SourcePath => SourceFile != null ? Path.GetDirectoryName(Path.GetFullPath(SourceFile)) : null;
	}

	public interface IVehicleDeclarationInputData : IComponentInputData
	{
		string Identifier { get; }

		bool ExemptedVehicle { get; }

		string VIN { get; }

		LegislativeClass? LegislativeClass { get; }

		/// <summary>
		/// P036
		/// cf. VECTO Input Parameters.xlsx
		/// </summary>
		VehicleCategory VehicleCategory { get; }

		/// <summary>
		/// P037
		/// cf. VECTO Input Parameters.xlsx
		/// </summary>
		AxleConfiguration AxleConfiguration { get; }

		/// <summary>
		/// P038  Curb Weight Vehicle
		/// cf. VECTO Input Parameters.xlsx
		/// </summary>
		Kilogram CurbMassChassis { get; }

		/// <summary>
		/// P041  Max. vehicle weight
		/// cf. VECTO Input Parameters.xlsx
		/// </summary>
		Kilogram GrossVehicleMassRating { get; }

		///// <summary>
		///// P117  Powered axle tyres/rims
		///// cf. VECTO Input Parameters.xlsx
		///// </summary>
		//string Rim { get; }  // deprecated

		/// <summary>
		/// P196, P197  TorqueLimits: Gear [-], MaxTorque [Nm]
		/// cf. VECTO Input Parameters.xlsx
		/// </summary>
		IList<ITorqueLimitInputData> TorqueLimits { get; }

		/// <summary>
		/// parameters for every axle
		/// P044, P045, P046, P047, P048, P108
		/// cf. VECTO Input Parameters.xlsx
		/// </summary>

		string ManufacturerAddress { get; }

		PerSecond EngineIdleSpeed { get; }

		// new (optional) input fields

		bool VocationalVehicle { get; }

		bool? SleeperCab { get; }

		bool? AirdragModifiedMultistep { get; }

        Kilogram H2StorageUsableCapacity { get; }

        HydrogenStorageTechnology? HydrogenStorageTechnology { get; }

        TankSystem? TankSystem { get; }

		IAdvancedDriverAssistantSystemDeclarationInputData ADAS { get; }

		IVehicleInMotionChargingDeclaration InMotionCharging { get; }

        // fields for exempted vehicles

        bool ZeroEmissionVehicle { get; }

		bool HybridElectricHDV { get; }

        bool DualFuelVehicle { get; }

		/// <summary>
		/// SumNetPower in 2nd amendment
		/// </summary>
        Watt MaxNetPower1 { get; }

		string ExemptedTechnology { get; }

		// --- end

		RegistrationClass? RegisteredClass { get; }

		int? NumberPassengerSeatsUpperDeck { get; }

		int? NumberPassengerSeatsLowerDeck { get; }

		int? NumberPassengersStandingLowerDeck { get; }

		int? NumberPassengersStandingUpperDeck { get; }

		// only used for medium lorries type VAN
		CubicMeter CargoVolume { get; }

		VehicleCode? VehicleCode { get; }

		bool? LowEntry { get; }

		bool Articulated { get; }

		Meter Height { get; }

		Meter Length { get; }

		Meter Width { get; }

		Meter EntranceHeight { get; }

		ConsumerTechnology? DoorDriveTechnology { get; }

		VehicleDeclarationType VehicleDeclarationType { get; }

		IDictionary<EMPlacement, IList<Tuple<Volt, TableData>>> ElectricMotorTorqueLimits { get; }

		TableData BoostingLimitations { get; }

		// components

		IVehicleComponentsDeclaration Components { get; }
		XmlNode XMLSource { get; }

		string VehicleTypeApprovalNumber { get; }

		ArchitectureID ArchitectureID { get; }

        ArchitectureID ArchitectureIDPwt2 { get; }

        bool OVC { get; }

		bool BatteryOnlyMode { get; }

        DynamicChargingTechnology DynamicChargingTechnology {  get; }

        Watt MaxChargingPower { get; }

		VectoSimulationJobType VehicleType { get; }

		string SimulationToolLicenseNumber { get; }

        string VehicleMonitoringData { get; }
    }

	public interface IVehicleInMotionChargingDeclaration
    {
		IMCTechnology Technology { get; }
	}


    public interface IVehicleComponentsDeclaration
	{
		IAirdragDeclarationInputData AirdragInputData { get; }

		IGearboxDeclarationInputData GearboxInputData { get; }

		ITorqueConverterDeclarationInputData TorqueConverterInputData { get; }

		IAxleGearInputData AxleGearInputData { get; }

		IAngledriveInputData AngledriveInputData { get; }

		IEngineDeclarationInputData EngineInputData { get; }

		IAuxiliariesDeclarationInputData AuxiliaryInputData { get; }

		IRetarderInputData RetarderInputData { get; }

		IPTOTransmissionInputData PTOTransmissionInputData { get; }

		IAxlesDeclarationInputData AxleWheels { get; }

		IBusAuxiliariesDeclarationData BusAuxiliaries { get; }

		IElectricStorageSystemDeclarationInputData ElectricStorage { get; }

		IElectricMachinesDeclarationInputData ElectricMachines { get; }

		IIEPCDeclarationInputData IEPC { get; }

		IFuelCellSystemDeclarationInputData FuelCellSystem { get; }

        IList<IAxlePowertrainDeclarationInputData> AxlePowertrainInputData { get; }

        ElectricMachineEntry<IElectricMotorDeclarationInputData> Generator { get; }
    }

	public interface IAxlePowertrainDeclarationInputData
	{
        int AxleNumber { get; }

        ArchitectureID Architecture { get; }

        IGearboxDeclarationInputData GearboxInputData { get; }

        IAxleGearInputData AxleGearInputData { get; }

        ITorqueConverterDeclarationInputData TorqueConverterInputData { get; }

        IAngledriveInputData AngledriveInputData { get; }

        IRetarderInputData RetarderInputData { get; }

        IPTOTransmissionInputData PTOTransmissionInputData { get; }

        ElectricMachineEntry<IElectricMotorDeclarationInputData> ElectricMotor { get; }

        IIEPCDeclarationInputData IEPCInputData { get; }
    }

	public static class ComponentsHelper{
		/// <summary>
		/// Returns the gearbox type of Gearbox- or IEPC-Component
		/// </summary>
		/// <param name=""></param>
		public static GearboxType? GetGearboxType(this IVehicleComponentsDeclaration components)
		{
			return components?.GearboxInputData?.Type ?? (components?.IEPC != null ? new GearboxType?(GearboxType.IEPC) : null);
		}
	}

	public interface IAxlesDeclarationInputData
	{
		/// <summary>
		/// parameters for every axle
		/// P044, P045, P046, P047, P048, P108
		/// cf. VECTO Input Parameters.xlsx
		/// </summary>
		IList<IAxleDeclarationInputData> AxlesDeclaration { get; }

		int? NumSteeredAxles { get; }

		XmlNode XMLSource { get; }
	}

	public interface IAdvancedDriverAssistantSystemDeclarationInputData
	{
		bool EngineStopStart { get; }

		EcoRollType EcoRoll { get; }

		PredictiveCruiseControlType PredictiveCruiseControl { get; }

		bool? ATEcoRollReleaseLockupClutch { get; }
		XmlNode XMLSource { get; }
	}

	public enum PredictiveCruiseControlType
	{
		[GuiLabel("None")]
		None,
		[GuiLabel("Option I + II")]
		Option_1_2,
		[GuiLabel("Option I + II + III")]
		Option_1_2_3
	}

	public static class PredictiveCruiseControlTypeHelper
	{
		public const string Prefix = "Option_";
		public const string SeparatorXML = ",";
		public const string SeparatorEnum = "_";

		public static PredictiveCruiseControlType Parse(string value)
		{
			if (PredictiveCruiseControlType.None.ToString().Equals(value, StringComparison.InvariantCultureIgnoreCase)) {
				return PredictiveCruiseControlType.None;
			}

			return (Prefix + value.Replace(SeparatorXML, SeparatorEnum)).ParseEnum<PredictiveCruiseControlType>();
		}

		public static string ToXMLFormat(this PredictiveCruiseControlType pcc)
		{
			return pcc.ToString().ToLowerInvariant().Replace(Prefix.ToLowerInvariant(), "").Replace(SeparatorEnum, SeparatorXML);
		}

		public static string GetName(this PredictiveCruiseControlType pcc)
		{
			return pcc.ToString().Replace(Prefix, Prefix.Replace(SeparatorEnum, " ")).Replace(SeparatorEnum, " & ");
		}
	}

	public enum EcoRollType
	{
		[GuiLabel("None")]
		None,
		[GuiLabel("Without Engine Stop")]
		WithoutEngineStop,
		[GuiLabel("With Engine Stop")]
		WithEngineStop
	}

	public static class EcorollTypeHelper
	{
		public static EcoRollType Get(bool ecoRollWithoutEngineStop, bool ecoRollWithEngineStop)
		{
			if (ecoRollWithEngineStop && ecoRollWithoutEngineStop) {
				throw new VectoException("invalid combination for EcoRoll");
			}

			if (ecoRollWithoutEngineStop) {
				return EcoRollType.WithoutEngineStop;
			}

			if (ecoRollWithEngineStop) {
				return EcoRollType.WithEngineStop;
			}

			return EcoRollType.None;
		}

		public static EcoRollType Parse(string ecoRoll)
		{
			return ecoRoll.ParseEnum<EcoRollType>();
		}

		public static bool WithoutEngineStop(this EcoRollType ecoRoll)
		{
			return ecoRoll == EcoRollType.WithoutEngineStop;
		}

		public static bool WithEngineStop(this EcoRollType ecoRoll)
		{
			return ecoRoll == EcoRollType.WithEngineStop;
		}

		public static string GetName(this EcoRollType ecoRoll)
		{
			switch (ecoRoll) {
				case EcoRollType.None: return "None";
				case EcoRollType.WithoutEngineStop: return "without engine stop";
				case EcoRollType.WithEngineStop: return "with engine stop";
				default: throw new ArgumentOutOfRangeException(nameof(ecoRoll), ecoRoll, null);
			}
		}

		public static string ToXMLFormat(this EcoRollType ecoRoll)
		{
			return GetName(ecoRoll).ToLowerInvariant();
		}
	}

	public enum TankSystem
	{
		None,
		Liquefied,
		Compressed
	}

	public static class TankSystemHelper
	{
		public static TankSystem? Parse(string parse)
		{
			switch (parse) {
				case nameof(TankSystem.Liquefied):
					return TankSystem.Liquefied;
				case nameof(TankSystem.Compressed):
					return TankSystem.Compressed;
				default:
					return null;
			}
		}
	}

	public interface IAirdragDeclarationInputData : IComponentInputData
	{
		/// <summary>
		/// P146, P147  DragCoefficient * Cross Section Area - Rigid
		/// cf. VECTO Input Parameters.xlsx
		/// </summary>
		SquareMeter AirDragArea { get; } // without trailer
		SquareMeter TransferredAirDragArea { get; } // P246

		SquareMeter AirDragArea_0 { get; } // P245

        SquareMeter DeltaCdxA_CFD { get; } //P561

        SquareMeter DeltaCdxA_declared { get; } //P563

        SquareMeter DeltaTransferredCdxA { get; } //P564

        string LicenseNumberCFDMethod { get; } //P562

        XmlNode XMLSource { get; }

	}

	public interface IRetarderInputData : IComponentInputData
	{
		/// <summary>
		/// P052
		/// cf. VECTO Input Parameters.xlsx
		/// </summary>
		RetarderType Type { get; }

		/// <summary>
		/// P053
		/// cf. VECTO Input Parameters.xlsx
		/// </summary>
		double Ratio { get; }

		/// <summary>
		/// P054
		/// P057, P058
		/// cf. VECTO Input Parameters.xlsx
		/// </summary>
		TableData LossMap { get; }
	}

	public interface IAngledriveInputData : IComponentInputData
	{
		/// <summary>
		/// P180
		/// </summary>
		AngledriveType Type { get; }

		/// <summary>
		/// P176
		/// </summary>
		double Ratio { get; }

		/// <summary>
		/// P173, P174, P175
		/// </summary>
		TableData LossMap { get; }

		/// <summary>
		/// P177
		/// </summary>
		double Efficiency { get; }
	}

	public interface IAxleDeclarationInputData
	{
		/// <summary>
		/// P045
		/// cf. VECTO Input Parameters.xlsx
		/// </summary>
		bool TwinTyres { get; }

		AxleType AxleType { get; }

		ITyreDeclarationInputData Tyre { get; }

		DataSource DataSource { get; }

        bool Steered { get; }

		NewtonMeter WheelEndFriction { get; }
	}

	public interface ITyreDeclarationInputData : IComponentInputData
	{
		/// <summary>
		/// P108
		/// cf. VECTO Input Parameters.xlsx
		/// </summary>
		string Dimension { get; }

		/// <summary>
		/// P046
		/// cf. VECTO Input Parameters.xlsx
		/// </summary>
		double RollResistanceCoefficient { get; }

		/// <summary>
		/// P047
		/// cf. VECTO Input Parameters.xlsx
		/// </summary>
		Newton TyreTestLoad { get; }

		string FuelEfficiencyClass { get; }
	}

	public interface IGearboxDeclarationInputData : IComponentInputData
	{
		/// <summary>
		/// P076
		/// cf. VECTO Input Parameters.xlsx
		/// </summary>
		GearboxType Type { get; }

		/// <summary>
		/// P078, P079, P077, P082, P145 (for every gear)
		/// cf. VECTO Input Parameters.xlsx
		/// </summary>
		IList<ITransmissionInputData> Gears { get; }

		bool DifferentialIncluded { get; }
		double AxlegearRatio { get; }
	}


	public interface ITransmissionInputData
	{
		int Gear { get; }

		/// <summary>
		/// P078
		/// cf. VECTO Input Parameters.xlsx
		/// </summary>
		double Ratio { get; }

		/// <summary>
		/// P079
		/// P096, P097, P098
		/// cf. VECTO Input Parameters.xlsx
		/// </summary>
		TableData LossMap { get; }

		/// <summary>
		/// P079
		/// cf. VECTO Input Parameters.xlsx
		/// </summary>
		double Efficiency { get; }


		/// <summary>
		/// P157
		/// </summary>
		NewtonMeter MaxTorque { get; }

		PerSecond MaxInputSpeed { get; }

		/// <summary>
		/// P082
		/// P093, P094, P095
		/// cf. VECTO Input Parameters.xlsx
		/// </summary>
		TableData ShiftPolygon { get; }

		DataSource DataSource { get; }
	}

	public interface IAxleGearInputData : IComponentInputData
	{
		/// <summary>
		/// P078
		/// cf. VECTO Input Parameters.xlsx
		/// </summary>
		double Ratio { get; }

		/// <summary>
		/// P079
		/// P096, P097, P098
		/// cf. VECTO Input Parameters.xlsx
		/// </summary>
		TableData LossMap { get; }

		/// <summary>
		/// P079
		/// cf. VECTO Input Parameters.xlsx
		/// </summary>
		double Efficiency { get; }

		AxleLineType LineType { get; }
	}

	public interface ITorqueConverterDeclarationInputData : IComponentInputData
	{
		/// <summary>
		/// P091
		/// P099, P100, P101
		/// cf. VECTO Input Parameters.xlsx
		/// </summary>
		// ReSharper disable once InconsistentNaming
		TableData TCData { get; }
	}

	public interface IEngineDeclarationInputData : IComponentInputData
	{
		/// <summary>
		/// P061
		/// cf. VECTO Input Parameters.xlsx
		/// </summary>
		CubicMeter Displacement { get; }

		Watt RatedPowerDeclared { get; }

		PerSecond RatedSpeedDeclared { get; }

		NewtonMeter MaxTorqueDeclared { get; }

		IList<IEngineModeDeclarationInputData> EngineModes { get; }

		WHRType WHRType { get; }
	}

	public interface IEngineModeDeclarationInputData
	{
		/// <summary>
		/// P063
		/// cf. VECTO Input Parameters.xlsx
		/// </summary>
		PerSecond IdleSpeed { get; }

		/// <summary>
		/// P144
		/// P068, P069, P70, P71
		/// cf. VECTO Input Parameters.xlsx
		/// </summary>
		TableData FullLoadCurve { get; }

		IList<IEngineFuelDeclarationInputData> Fuels { get; }

		IWHRData WasteHeatRecoveryDataElectrical { get; }

		IWHRData WasteHeatRecoveryDataMechanical { get; }
	}

	public interface IWHRData
	{
		double UrbanCorrectionFactor { get; }

		double RuralCorrectionFactor { get; }

		double MotorwayCorrectionFactor { get; }

		double BFColdHot { get; }

		double CFRegPer { get; }

		double EngineeringCorrectionFactor { get; }

		TableData GeneratedPower { get; }
	}

	public interface IEngineFuelDeclarationInputData
	{
		FuelType FuelType { get; }

		/// <summary>
		/// P111
		/// cf. VECTO Input Parameters.xlsx
		/// </summary>
		double WHTCMotorway { get; }

		/// <summary>
		/// P110
		/// cf. VECTO Input Parameters.xlsx
		/// </summary>
		double WHTCRural { get; }

		/// <summary>
		/// P109
		/// cf. VECTO Input Parameters.xlsx
		/// </summary>
		double WHTCUrban { get; }

		/// <summary>
		/// P159
		/// </summary>
		double ColdHotBalancingFactor { get; }

		double CorrectionFactorRegPer { get; }

		/// <summary>
		/// P067
		/// P072, P073, P074
		/// cf. VECTO Input Parameters.xlsx
		/// engine speed in rpm, torque in NM, fuel consumption in g/h
		/// </summary>
		TableData FuelConsumptionMap { get; }
	}


	public interface IAuxiliariesDeclarationInputData
	{
		bool SavedInDeclarationMode { get; }

		IList<IAuxiliaryDeclarationInputData> Auxiliaries { get; }
	}

	public interface ICycleData
	{
		string Name { get; }

		/// <summary>
		/// P028, P029, P030, P031, P032, P119, P120, P121, P122, P123, P124, P125, P126
		/// cf. VECTO Input Parameters.xlsx
		/// </summary>
		TableData CycleData { get; }
	}

	public interface IFuelNCVData
	{
		FuelType Type { get; }

		JoulePerKilogramm NCV { get; }
    }

	public interface IDriverDeclarationInputData : IDriverModelData
	{
		bool SavedInDeclarationMode { get; }
	}

	public interface IOverSpeedEcoRollDeclarationInputData : IDriverModelData
	{
		/// <summary>
		/// P015
		/// cf. VECTO Input Parameters.xlsx
		/// </summary>
		bool Enabled { get; }
	}

	public interface IAuxiliaryDeclarationInputData
	{

		/// <summary>
		/// P005  Aux-Type
		/// cf. VECTO Input Parameters.xlsx
		/// </summary>
		AuxiliaryType Type { get; }

		/// <summary>
		/// P118  Aux-Technology
		/// cf. VECTO Input Parameters.xlsx
		/// </summary>
		IList<string> Technology { get; }
	}

	public interface IFuelCellSystemDeclarationInputData
	{
		List<IFuelCellModuleDeclarationInputData> FuelCellModules { get; }
	}

	public interface IFuelCellModuleDeclarationInputData
	{
		int Count { get; }

		Watt MaxPower { get; }

		Watt MinPower { get; }

		IFuelCellDeclarationInputData FuelCell { get; }
	}

	public interface IFuelCellDeclarationInputData : IComponentInputData
	{
		Watt FCSRatedPower { get; }

		TableData FuelCellPowerOutputConsumptionMap { get; }
	}


	public interface IElectricMotorDeclarationInputData : IComponentInputData
	{
		ElectricMachineType ElectricMachineType { get; }
		Watt R85RatedPower { get; }
		KilogramSquareMeter Inertia { get; } //RotationalInertia


		bool DcDcConverterIncluded { get; }

		string IHPCType { get; }

		IList<IElectricMotorVoltageLevel> VoltageLevels { get; }

		TableData DragCurve { get; }

		TableData Conditioning { get; }

		//double OverloadRecoveryFactor { get; }
	}

	public static class IElectricMotorInputDataHelper
	{
		public static bool IsIHPC(this IElectricMotorDeclarationInputData em)
		{
			return ((!em.IHPCType.IsNullOrEmpty()) && (em.IHPCType != "None"));
		}
	}

	public interface IElectricMotorVoltageLevel
	{
		Volt VoltageLevel { get; }

		NewtonMeter ContinuousTorque { get; }

		PerSecond ContinuousTorqueSpeed { get; } //TestSpeedContinuousTorque
		NewtonMeter OverloadTorque { get; }

		PerSecond OverloadTestSpeed { get; } //TestSpeedOverloadTorque
		/// <summary>
		/// OverloadDuration
		/// </summary>
		Second OverloadTime { get; }
        /// <summary>
        /// MaxTorqueCurve
        /// </summary>
        IList<IElectricMotorLoadCurve> FullLoadCurve { get; }

		IList<IElectricMotorPowerMap> PowerMap { get; }
	}

	public interface IElectricMotorPowerMap
	{
		int Gear { get; }
		/// <summary>
		/// P_el must be in W!
		/// </summary>
		TableData PowerMap { get; }
	}

	public interface IElectricMotorLoadCurve
	{
        int Gear { get; }

		TableData LoadCurve { get; }
    }

	public interface IElectricMachinesDeclarationInputData
	{
		IList<ElectricMachineEntry<IElectricMotorDeclarationInputData>> Entries { get; }
	}

	//public interface IElectricMachineEntry<T>
	//{
	//	T ElectricMachine { get; set; }

	//	int Count { get; set; }

	//	PowertrainPosition Position { get; set; }

	//	double RatioADC { get; set; }

	//	double[] RatioPerGear { get; set; }

	//	double MechanicalTransmissionEfficiency { get; set; }

	//	TableData MechanicalTransmissionLossMap { get; set; }

	//	IADCDeclarationInputData ADC { get; set; }
	//}


	public class ElectricMachineEntry<T> where T : class, IElectricMotorDeclarationInputData
	{
		public T ElectricMachine { get; set; }

		public int Count { get; set; }

		public PowertrainPosition Position { get; set; }

		private double? _ratioADC = null;

		private TableData _lossMapADC = null;

		/// <summary>
		/// If not overridden RatioADC == ADC?.Ratio ?? 1;
		/// Can only be overridden when ADC == null;
		/// </summary>
		public double RatioADC {
			get
			{
				//Engineering mode sets RatioADC, decl mode sets ADC
				if (_ratioADC.HasValue && ADC == null) {
					return _ratioADC.Value;
				} else {
					return ADC?.Ratio ?? 1;
				}
			}
			set => _ratioADC = value;
		}

		public double[] RatioPerGear { get; set; }

		public double MechanicalTransmissionEfficiency { get; set; }

		public TableData MechanicalTransmissionLossMap
		{
			get
			{
				if (_lossMapADC != null && ADC == null) {
					return _lossMapADC;
				} else {
					return ADC?.LossMap;
				}
			}
			set
			{
				_lossMapADC = value;
			}
		}

		public IADCDeclarationInputData ADC {get; set; }
	}

    public interface IADCDeclarationInputData : IComponentInputData
	{
		/// <summary>
		/// P176
		/// </summary>
		double Ratio { get; }

		/// <summary>
		/// P173, P174, P175
		/// </summary>
		TableData LossMap { get; }
	}


	public interface IIEPCDeclarationInputData : IComponentInputData
	{
		ElectricMachineType ElectricMachineType { get; }

		Watt R85RatedPower { get; }

		Watt TotalRatedPowerCalculated { get; }

		KilogramSquareMeter Inertia { get; } //RotationalInertia

		bool DifferentialIncluded { get; }

		bool DesignTypeWheelMotor { get; }

		int? NrOfDesignTypeWheelMotorMeasured { get; }

		IList<IGearEntry> Gears { get; }

		IList<IElectricMotorVoltageLevel> VoltageLevels { get; }

		IList<IDragCurve> DragCurves { get; }

		TableData Conditioning { get; }

		bool DisengagementClutch { get; }
    }

	public interface IDragCurve
	{
		int? Gear { get; }

		TableData DragCurve { get; }
	}


	public interface IGearEntry
	{
		int GearNumber { get; }

		double Ratio { get; }
		NewtonMeter MaxOutputShaftTorque { get; }

		PerSecond MaxOutputShaftSpeed { get; }
	}

	public interface IElectricStorageSystemDeclarationInputData
	{
		IList<IElectricStorageDeclarationInputData> ElectricStorageElements { get; }
	}

	public interface IElectricStorageDeclarationInputData
	{
		IREESSPackInputData REESSPack { get; }

		int Count { get; }

		int StringId { get; }
	}

	public enum REESSType
	{
		Battery,
		SuperCap
	}

	public interface IREESSPackInputData : IComponentInputData
	{
		REESSType StorageType { get; }

	}

	public interface IBatteryPackDeclarationInputData : IREESSPackInputData
	{
		double? MinSOC { get; }

		double? MaxSOC { get; }

		double? DeteriorationPerformanceRatio { get; }

        BatteryType BatteryType { get; }

		AmpereSecond Capacity { get; }

		bool? ConnectorsSubsystemsIncluded { get; }

		bool? JunctionboxIncluded { get; }

		Kelvin TestingTemperature { get; }

		TableData InternalResistanceCurve { get; }

		TableData VoltageCurve { get; }

		TableData MaxCurrentMap { get; }
	}

	public interface ISuperCapDeclarationInputData : IREESSPackInputData
	{
		Farad Capacity { get; }

		Ohm InternalResistance { get; }

		Volt MinVoltage { get; }

		Volt MaxVoltage { get; }

		Ampere MaxCurrentCharge { get; }

		Ampere MaxCurrentDischarge { get; }

		Kelvin TestingTemperature { get; }
	}


	public interface ITorqueLimitInputData
	{
		int Gear { get; }

		NewtonMeter MaxTorque { get; }
	}

	public interface IBusAuxiliariesDeclarationData
	{
		DataSource DataSource { get; }

		string FanTechnology { get; }

		IList<string> SteeringPumpTechnology { get; }

		IElectricSupplyDeclarationData ElectricSupply { get; }

		IElectricConsumersDeclarationData ElectricConsumers { get; }

		IPneumaticSupplyDeclarationData PneumaticSupply { get; }

		IPneumaticConsumersDeclarationData PneumaticConsumers { get; }

		IHVACBusAuxiliariesDeclarationData HVACAux { get; }
	}

	public interface IElectricSupplyDeclarationData
	{
		AlternatorType AlternatorTechnology { get; }

		IList<IAlternatorDeclarationInputData> Alternators { get; }

		bool ESSupplyFromHEVREESS { get; }

		IList<IBusAuxElectricStorageDeclarationInputData> ElectricStorage { get; }
	}

	public static class ElectricSupplyDeclarationDataHelper
	{
		public static Watt GetMaxAlternatorPower(this IElectricSupplyDeclarationData electricSupply)
		{
			return electricSupply.Alternators?.Select(alt => alt.RatedCurrent * alt.RatedVoltage)?
				.Max();
		}
	}

	public interface IElectricConsumersDeclarationData
	{
		bool? InteriorLightsLED { get; }

		bool? DayrunninglightsLED { get; }

		bool? PositionlightsLED { get; }

		bool? HeadlightsLED { get; }

		bool? BrakelightsLED { get; }
	}

	public interface IAlternatorDeclarationInputData
	{
		Ampere RatedCurrent { get; }

		Volt RatedVoltage { get; }
	}

	public interface IBusAuxElectricStorageDeclarationInputData
	{
		string Technology { get; }

	}




	public interface IPneumaticSupplyDeclarationData
	{
		CompressorDrive CompressorDrive { get; }
		string Clutch { get; }
		double Ratio { get; }

		string CompressorSize { get; }

		bool SmartAirCompression { get; }

		bool SmartRegeneration { get; }
	}

	public interface IPneumaticConsumersDeclarationData
	{
		ConsumerTechnology AirsuspensionControl { get; }
		ConsumerTechnology AdBlueDosing { get; }
	}

	public interface IHVACBusAuxiliariesDeclarationData
	{
		BusHVACSystemConfiguration? SystemConfiguration { get; }

		HeatPumpType? HeatPumpTypeCoolingDriverCompartment { get; }

		HeatPumpType? HeatPumpTypeHeatingDriverCompartment { get; }

		HeatPumpType? HeatPumpTypeCoolingPassengerCompartment { get; }

		HeatPumpType? HeatPumpTypeHeatingPassengerCompartment { get; }

		Watt AuxHeaterPower { get; }

		bool? DoubleGlazing { get; }

		bool? AdjustableAuxiliaryHeater { get; }

		bool? SeparateAirDistributionDucts { get; }

		bool? WaterElectricHeater { get; }

		bool? AirElectricHeater { get; }

		bool? OtherHeatingTechnology { get; }

		bool? AdjustableCoolantThermostat { get; }

		bool EngineWasteGasHeatExchanger { get; }
	}


	public interface IResultsInputData
	{
		string Status { get; }

		IList<IResult> Results { get; }
	}

	public enum ResultStatus
	{
		Success,
		Error,
		PrimaryRunIgnored
	}

    public interface IResult
	{
		ResultStatus ResultStatus { get; }

		VehicleClass VehicleGroup { get; }

		MissionType Mission { get; }

		ISimulationParameter SimulationParameter { get; }

		Dictionary<FuelType, JoulePerMeter> EnergyConsumption { get; }
		JoulePerMeter ElectricEnergyConsumption { get; }

		/// <summary>
		/// Dictionary <string unit, double value>
		/// </summary>
        Dictionary<string, double> CO2 { get; }

		OvcHevMode OvcMode { get; }
	}

	public interface ISimulationParameter
	{
		Kilogram TotalVehicleMass { get; }
		Kilogram Payload { get; }
		double PassengerCount { get; }
		//string FuelMode { get; }
	}


	public interface IApplicationInformation
	{
		string SimulationToolVersion { get; }
		DateTime Date { get; }
	}

	public interface IManufacturingStageInputData
	{
		DigestData HashPreviousStep { get; }
		int StepCount { get; }

		IVehicleDeclarationInputData Vehicle { get; }

		IApplicationInformation ApplicationInformation { get; }

		DigestData Signature { get; }

	}

	public enum VehicleDeclarationType
	{
		[GuiLabel("Interim")]
		interim,
		[GuiLabel("Final")]
		final
	}

	public static class VehicleDeclarationTypeHelper
	{
		public static VehicleDeclarationType Parse(string parse)
		{
			switch (parse)
			{
				case nameof(VehicleDeclarationType.interim):
					return VehicleDeclarationType.interim;
				case nameof(VehicleDeclarationType.final):
					return VehicleDeclarationType.final;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}
		public static string GetLabel(this VehicleDeclarationType type)
		{
			switch (type)
			{
				case VehicleDeclarationType.final: return nameof(VehicleDeclarationType.final);
				case VehicleDeclarationType.interim:
					return nameof(VehicleDeclarationType.interim);
				default: return null;
			}
		}
	}

	public enum CompressorDrive
	{
		[GuiLabel("Electrically")]
		electrically,
		[GuiLabel("Mechanically")]
		mechanically
	}

	public static class CompressorDriveHelper
	{
		public static CompressorDrive Parse(string parse)
		{
			switch (parse)
			{
				case nameof(CompressorDrive.electrically):
					return CompressorDrive.electrically;
				case nameof(CompressorDrive.mechanically):
					return CompressorDrive.mechanically;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		public static string GetLabel(this CompressorDrive type)
		{
			switch (type)
			{
				case CompressorDrive.electrically: return nameof(CompressorDrive.electrically);
				case CompressorDrive.mechanically: return nameof(CompressorDrive.mechanically);
				default: return null;
			}
		}
	}


	public enum ElectricMachineType
	{
		ASM,
		ESM,
		PSM,
		RM
	}

	public static class ElectricMachineTypeHelper
	{
		public static string GetLabel(this ElectricMachineType type)
		{
			return type.ToString();
		}
	}

	public enum BatteryType
	{
		HPBS,
		HEBS
	}

	public enum ArchitectureID
	{
		UNKNOWN,
		E2,
		E3,
		E4,
		E_IEPC,
		P1,
		P2,
		P2_5,
		P3,
		P4,
		P_IHPC,
		S2,
		S3,
		S4,
		S_IEPC,
		F2,
		F3,
		F4,
		F_IEPC
	}


	public static class ArchitectureIDHelper
	{
		private const string E_IEPC_ID = "E-IEPC";
		private const string P2_5_ID = "P2.5";
		private const string S_IEPC_ID = "S-IEPC";
		private const string F_IEPC_ID = "F-IEPC";

		public static ArchitectureID Parse(string parse)
		{
			switch (parse)
			{
				case nameof(ArchitectureID.E2):
				case nameof(ArchitectureID.E3):
				case nameof(ArchitectureID.E4):
				case nameof(ArchitectureID.P1):
				case nameof(ArchitectureID.P2):
				case nameof(ArchitectureID.P3):
				case nameof(ArchitectureID.P4):
				case nameof(ArchitectureID.S2):
				case nameof(ArchitectureID.S3):
				case nameof(ArchitectureID.S4):
				case nameof(ArchitectureID.F2):
				case nameof(ArchitectureID.F3):
				case nameof(ArchitectureID.F4):
					return parse.ParseEnum<ArchitectureID>();
				case E_IEPC_ID:
					return ArchitectureID.E_IEPC;
				case P2_5_ID:
					return ArchitectureID.P2_5;
				case S_IEPC_ID:
					return ArchitectureID.S_IEPC;
				case F_IEPC_ID:
					return ArchitectureID.F_IEPC;
				default:
					throw new ArgumentOutOfRangeException($"{nameof(ArchitectureID)}");
			}
		}

		public static string GetLabel(this ArchitectureID type)
		{
			switch (type) {
				case ArchitectureID.E_IEPC:
					return E_IEPC_ID;
				case ArchitectureID.P2_5:
					return P2_5_ID;
				case ArchitectureID.S_IEPC:
					return S_IEPC_ID;
				case ArchitectureID.F_IEPC:
					return F_IEPC_ID;
				default:
					return type.ToString();
			}
		}

		public static bool IsBatteryElectricVehicle(this ArchitectureID type)
		{
			switch (type) {
				case ArchitectureID.E2:
				case ArchitectureID.E3:
				case ArchitectureID.E4:
				case ArchitectureID.E_IEPC:
					return true;
				default: return false;
			}
		}

		public static bool IsHybridVehicle(this ArchitectureID type)
		{
			return IsSerialHybridVehicle(type) || IsParallelHybridVehicle(type);
		}

		public static bool IsParallelHybridVehicle(this ArchitectureID type)
		{
			switch (type) {
				case ArchitectureID.P1:
				case ArchitectureID.P2:
				case ArchitectureID.P2_5:
				case ArchitectureID.P3:
				case ArchitectureID.P4:
				//case ArchitectureID.P_IHPC:
					return true;
				default:
					return false;
			}
		}

		public static bool IsSerialHybridVehicle(this ArchitectureID type)
		{
			switch (type) {
				case ArchitectureID.S2:
				case ArchitectureID.S3:
				case ArchitectureID.S4:
				case ArchitectureID.S_IEPC:
					return true;
				default:
					return false;
			}
		}

		public static bool IsFuelCellVehicle(this ArchitectureID type)
		{
			switch (type)
			{
				case ArchitectureID.F2:
				case ArchitectureID.F3:
				case ArchitectureID.F4:
				case ArchitectureID.F_IEPC:
					return true;
				default:
					return false;
			}
		}
	}
}
