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

using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;

namespace TUGraz.VectoCommon.InputData
{
	public interface IEngineeringJobInputData : IDeclarationJobInputData
	{
		new IVehicleEngineeringInputData Vehicle { get; }


		IHybridStrategyParameters HybridStrategyParameters { get; }

		/// <summary>
		/// P008  Cycles
		/// cf. VECTO Input Parameters.xlsx
		/// </summary>
		IList<ICycleData> Cycles { get; }

		IEngineEngineeringInputData EngineOnly { get; }
	}


	public interface IHybridStrategyParameters
	{
		double EquivalenceFactorDischarge { get; }

		double EquivalenceFactorCharge { get; }

		double MinSoC { get; }

		double MaxSoC { get; }

		double TargetSoC { get; }

		string Source { get;  }
		Second MinimumICEOnTime { get; }
		Second AuxBufferTime { get; }
		Second AuxBufferChargeTime { get; }
		double ICEStartPenaltyFactor { get; }

		double CostFactorSOCExpponent { get; }

		// serial hybrid only: factor applied to the max propulsion power which the genset needs to provide in the optimal operating point
		double GensetMinOptPowerFactor { get; }
	}

	public interface IVehicleEngineeringInputData : IVehicleDeclarationInputData
	{
		/// <summary>
		/// P039  Curb Weight Extra Trailer/Body
		/// cf. VECTO Input Parameters.xlsx
		/// </summary>
		Kilogram CurbMassExtra { get; }

		/// <summary>
		/// P040  Loading
		/// cf. VECTO Input Parameters.xlsx
		/// </summary>
		Kilogram Loading { get; }

		/// <summary>
		/// P049
		/// cf. VECTO Input Parameters.xlsx
		/// </summary>
		Meter DynamicTyreRadius { get; }

        Meter Height { get; }


		new IVehicleComponentsEngineering Components { get; }

		new IAdvancedDriverAssistantSystemsEngineering ADAS { get; }

		double InitialSOC { get; }

		new IVehicleInMotionChargingEngineering InMotionCharging { get; }

        // input parameters for road sweeper use case


        VectoSimulationJobType VehicleType { get; }
		GearshiftPosition PTO_DriveGear { get; }

		PerSecond PTO_DriveEngineSpeed { get; }
	}

	public interface IAdvancedDriverAssistantSystemsEngineering : IAdvancedDriverAssistantSystemDeclarationInputData
	{
		DataSource DataSource { get; }
	}

	public interface IVehicleInMotionChargingEngineering : IVehicleInMotionChargingDeclaration
    {
		bool Enabled { get; }
		double ShareIMCAvailabilityTotalMission { get; }
		SquareMeter DeltaCdxA { get; }
    }

	public interface IVehicleComponentsEngineering
	{
		IAirdragEngineeringInputData AirdragInputData { get; }

		IGearboxEngineeringInputData GearboxInputData { get; }

		ITorqueConverterEngineeringInputData TorqueConverterInputData { get; }

		IAxleGearInputData AxleGearInputData { get; }

		IAngledriveInputData AngledriveInputData { get; }

		IEngineEngineeringInputData EngineInputData { get; }

		IAuxiliariesEngineeringInputData AuxiliaryInputData { get; }

		IRetarderInputData RetarderInputData { get; }

		IPTOTransmissionInputData PTOTransmissionInputData { get; }

		/// <summary>
		/// parameters for every axle
		/// P044, P045, P046, P047, P048, P108
		/// cf. VECTO Input Parameters.xlsx
		/// </summary>
		IAxlesEngineeringInputData AxleWheels { get; }

		//IBusAuxiliariesEngineeringData BusAuxiliaries { get; }

		IElectricStorageSystemEngineeringInputData ElectricStorage { get; }

		IElectricMachinesEngineeringInputData ElectricMachines { get; }

		IIEPCEngineeringInputData IEPCEngineeringInputData { get; }

		IFuelCellSystemEngineeringInputData FuelCellSystemInputData { get; }

		IList<IAxlePowertrainEngineeringInputData> AxlePowertrainEngineeringInputData { get; }
	}

	public interface IAxlePowertrainEngineeringInputData
	{
		int AxleNumber { get; }

		VectoSimulationJobType Type { get; }

		IGearboxEngineeringInputData GearboxInputData { get; }

		IAxleGearInputData AxleGearInputData { get; }

		ITorqueConverterEngineeringInputData TorqueConverterInputData { get; }

		IAngledriveInputData AngledriveInputData { get; }

		IRetarderInputData RetarderInputData { get; }

		IPTOTransmissionInputData PTOTransmissionInputData { get; }

		IGearshiftEngineeringInputData GearshiftInputData { get; }

		ElectricMachineEntry<IElectricMotorEngineeringInputData> ElectricMotor { get; }
	}

	public interface IAxlesEngineeringInputData
	{
		/// <summary>
		/// parameters for every axle
		/// P044, P045, P046, P047, P048, P108
		/// cf. VECTO Input Parameters.xlsx
		/// </summary>
		IList<IAxleEngineeringInputData> AxlesEngineering { get; }

		DataSource DataSource { get; }
	}

	public interface IAirdragEngineeringInputData : IAirdragDeclarationInputData
	{
		/// <summary>
		/// P050 - Cross Wind Correction Mode
		/// cf. VECTO Input Parameters.xlsx
		/// </summary>
		CrossWindCorrectionMode CrossWindCorrectionMode { get; }

		/// <summary>
		/// P051
		/// P055, P056
		/// cf. VECTO Input Parameters.xlsx
		/// </summary>
		TableData CrosswindCorrectionMap { get; }
	}

	public interface IPTOTransmissionInputData
	{
		/// <summary>
		/// The transmission type for the constant pto transmission losses.
		/// </summary>
		string PTOTransmissionType { get; }

		/// <summary>
		/// The PTO Loss map for idling losses of the "consumer" part.
		/// </summary>
		TableData PTOLossMap { get; }

		TableData PTOCycleDuringStop { get; }

		TableData EPTOCycleDuringStop { get; }

		TableData PTOCycleWhileDriving { get; }

        int AxleNumber { get; }
    }

	public interface IAxleEngineeringInputData : IAxleDeclarationInputData
	{
		/// <summary>
		/// P044 (0 - 1)
		/// cf. VECTO Input Parameters.xlsx
		/// </summary>
		double AxleWeightShare { get; }

		new ITyreEngineeringInputData Tyre { get; }
	}

	public interface ITyreEngineeringInputData : ITyreDeclarationInputData
	{
		/// <summary>
		/// P048
		/// cf. VECTO Input Parameters.xlsx
		/// </summary>
		KilogramSquareMeter Inertia { get; }

		Meter DynamicTyreRadius { get; }
	}

	public interface IGearboxEngineeringInputData : IGearboxDeclarationInputData
	{
		/// <summary>
		/// P080
		/// cf. VECTO Input Parameters.xlsx
		/// </summary>
		KilogramSquareMeter Inertia { get; }

		/// <summary>
		/// P081
		/// cf. VECTO Input Parameters.xlsx
		/// </summary>
		Second TractionInterruption { get; }

		Second PowershiftShiftTime { get; }
	}

	public interface IGearshiftEngineeringInputData : ITorqueConverterEngineeringShiftParameterInputData, IDriverModelData
	{
		/// <summary>
		/// P086
		/// cf. VECTO Input Parameters.xlsx
		/// </summary>
		Second MinTimeBetweenGearshift { get; }

		///// <summary>
		///// P083
		///// cf. VECTO Input Parameters.xlsx
		///// </summary>
		//bool EarlyShiftUp { get; }

		/// <summary>
		/// P085
		/// cf. VECTO Input Parameters.xlsx
		/// </summary>
		double TorqueReserve { get; }

		/// <summary>
		/// P087
		/// cf. VECTO Input Parameters.xlsx
		/// </summary>
		MeterPerSecond StartSpeed { get; }

		/// <summary>
		/// P088
		/// cf. VECTO Input Parameters.xlsx
		/// </summary>
		MeterPerSquareSecond StartAcceleration { get; }

		/// <summary>
		/// P089
		/// [%] (0-1)
		/// cf. VECTO Input Parameters.xlsx
		/// </summary>
		double StartTorqueReserve { get; }

		///// <summary>
		///// P084
		///// cf. VECTO Input Parameters.xlsx
		///// </summary>
		//bool SkipGears { get; }

		Second DownshiftAfterUpshiftDelay { get; }

		Second UpshiftAfterDownshiftDelay { get; }

		MeterPerSquareSecond UpshiftMinAcceleration { get; }

		// ACEA/Scania GS Parameters
		Second GearResidenceTime { get; }
		double? DnT99LHMin1 { get; }
		double? DnT99LHMin2 { get; }
		int? AllowedGearRangeUp { get; }
		int? AllowedGearRangeDown { get; }
		Second LookBackInterval { get; }
		Watt AvgCardanPowerThresholdPropulsion { get; }
		Watt CurrCardanPowerThresholdPropulsion { get; }
		double? TargetSpeedDeviationFactor { get; }
		double? EngineSpeedHighDriveOffFactor { get; }
		double? RatingFactorCurrentGear { get; }
		TableData AccelerationReserveLookup { get; }
		TableData ShareTorque99L { get; }
		TableData PredictionDurationLookup { get; }
		TableData ShareIdleLow { get; }
		TableData ShareEngineHigh { get; }
		string Source { get; }
		Second DriverAccelerationLookBackInterval { get; }
		MeterPerSquareSecond DriverAccelerationThresholdLow { get; }

		// FC-Based GS parameters
		double? RatioEarlyUpshiftFC { get; }
		double? RatioEarlyDownshiftFC { get; }

		int? AllowedGearRangeFC { get; }

		double? VeloictyDropFactor { get; }

		double? AccelerationFactor { get; }

		// Voith GS Parameters
		TableData LoadStageShiftLines { get; }
		IList<double> LoadStageThresholdsUp { get; }
		IList<double> LoadStageThresholdsDown { get; }
		PerSecond MinEngineSpeedPostUpshift { get; }

		Second ATLookAheadTime { get; }
		double[][] ShiftSpeedsTCToLocked { get; }

		double? PEV_TargetSpeedBrakeNorm { get; }

		double? PEV_DownshiftSpeedFactor { get; }
		double? PEV_DeRatingDownshiftSpeedFactor { get; }
		double? PEV_DownshiftMinSpeedFactor { get; }
	}

	public interface ITorqueConverterEngineeringShiftParameterInputData
	{
		/// <summary>
		/// Min Acceleration after C->L upshifts.
		/// </summary>
		MeterPerSquareSecond CLUpshiftMinAcceleration { get; }

		/// <summary>
		/// Min Acceleration after C->C upshifts.
		/// </summary>
		MeterPerSquareSecond CCUpshiftMinAcceleration { get; }
	}

	public interface ITorqueConverterEngineeringInputData : ITorqueConverterDeclarationInputData
	{
		///// <summary>
		///// P090
		///// cf. VECTO Input Parameters.xlsx
		///// </summary>
		//bool Enabled { get; }   // deprecated

		/// <summary>
		/// P092
		/// cf. VECTO Input Parameters.xlsx
		/// </summary>
		// ReSharper disable once InconsistentNaming
		PerSecond ReferenceRPM { get; }

		/// <summary>
		/// P127
		/// cf. VECTO Input Parameters.xlsx
		/// </summary>
		KilogramSquareMeter Inertia { get; }

		TableData ShiftPolygon { get; }

		PerSecond MaxInputSpeed { get; }
	}

	public interface IEngineEngineeringInputData : IEngineDeclarationInputData
	{
		/// <summary>
		/// P062
		/// cf. VECTO Input Parameters.xlsx
		/// </summary>
		KilogramSquareMeter Inertia { get; }

		new IList<IEngineModeEngineeringInputData> EngineModes { get; }

		Second EngineStartTime { get; }
	}

	public interface IEngineModeEngineeringInputData : IEngineModeDeclarationInputData
	{
		new IList<IEngineFuelEngineeringInputData> Fuels { get; }
	}

	public interface IEngineFuelEngineeringInputData : IEngineFuelDeclarationInputData
	{
		/// <summary>
		/// P170
		/// </summary>
		double WHTCEngineering { get; }
	}

	public interface IAuxiliariesEngineeringInputData
	{
		IAuxiliaryEngineeringInputData Auxiliaries { get; }

		IBusAuxiliariesEngineeringData BusAuxiliariesData { get; }

	}

	public interface IBusAuxiliariesEngineeringData
	{
		DataSource DataSource { get; }

		IBusAuxPneumaticSystemEngineeringData PneumaticSystem { get; }

		IBusAuxElectricSystemEngineeringData ElectricSystem { get; }

		IBusAuxHVACData HVACData { get; }
	}

	public interface IBusAuxPneumaticSystemEngineeringData
	{
		TableData CompressorMap { get; }

		NormLiterPerSecond AverageAirConsumed { get; }

		bool SmartAirCompression { get; }

		double GearRatio { get; }
	}

	public interface IBusAuxElectricSystemEngineeringData
	{
		double AlternatorEfficiency { get; }

		double DCDCConverterEfficiency { get; }

		Ampere CurrentDemand { get; }

		Ampere CurrentDemandEngineOffDriving { get; }

		Ampere CurrentDemandEngineOffStandstill { get; }

		AlternatorType AlternatorType { get; }

		WattSecond ElectricStorageCapacity { get; }

		Watt MaxAlternatorPower { get; }

		bool ESSupplyFromHEVREESS { get; }
		double ElectricStorageEfficiency { get; }
	}

	public interface IBusAuxHVACData
	{
		Watt ElectricalPowerDemand { get; }

		Watt MechanicalPowerDemand { get; }

		Joule AverageHeatingDemand { get; }

		Watt AuxHeaterPower { get; }
	}

	public interface IElectricMotorEngineeringInputData : IElectricMotorDeclarationInputData
	{
		double OverloadRecoveryFactor { get; }
	}

	public interface IElectricMachinesEngineeringInputData : IElectricMachinesDeclarationInputData
	{
		new IList<ElectricMachineEntry<IElectricMotorEngineeringInputData>> Entries { get; }
	}


	public interface IIEPCEngineeringInputData : IIEPCDeclarationInputData
	{
		double OverloadRecoveryFactor { get; }
	}

	public interface IElectricStorageEngineeringInputData : IElectricStorageDeclarationInputData
	{

	}

	public interface IElectricStorageSystemEngineeringInputData  : IElectricStorageSystemDeclarationInputData
	{
		new IList<IElectricStorageEngineeringInputData> ElectricStorageElements { get; }
	}

	public interface IBatteryPackEngineeringInputData : IBatteryPackDeclarationInputData
	{
	}

	public interface ISuperCapEngineeringInputData : ISuperCapDeclarationInputData
	{
	}

	public interface IFuelCellSystemEngineeringInputData
	{
		IList<FuelCellStringEntry<IFuelCellComponentEngineeringInputData>> FuelCellStrings { get; }

        Meter MaxWindowSize { get; }
    }
	public class FuelCellStringEntry<T> where T : class, IFuelCellComponentEngineeringInputData //Generic to reuse for declaration?
	{
		public int Count { get; set; }
		public T FuelCellComponent { get; set; }
	}
    public interface IFuelCellComponentEngineeringInputData : IComponentInputData
	{
		TableData MassFlowMap { get; }
		Watt MaxElectricPower { get; }

		Watt MinElectricPower { get; }
    }

	public interface IDriverModelData { }

	public interface IDriverAccelerationData : IDriverModelData
	{
		TableData AccelerationCurve { get; }
	}


	public interface IDriverEngineeringInputData : IDriverDeclarationInputData
	{
		//new IStartStopEngineeringInputData StartStop { get; }

		IOverSpeedEngineeringInputData OverSpeedData { get; }

		/// <summary>
		/// P009; P033, P034, P035
		/// cf. VECTO Input Parameters.xlsx
		/// </summary>
		IDriverAccelerationData AccelerationCurve { get; }

		ILookaheadCoastingInputData Lookahead { get; }

		IGearshiftEngineeringInputData GearshiftInputData { get; }

		IEngineStopStartEngineeringInputData EngineStopStartData { get; }

		IEcoRollEngineeringInputData EcoRollData { get; }
		IPCCEngineeringInputData PCCData { get; }
	}

	public interface IEcoRollEngineeringInputData
	{
		MeterPerSecond MinSpeed { get; }

		Second ActivationDelay { get; }

		MeterPerSecond UnderspeedThreshold { get; }

		MeterPerSquareSecond AccelerationUpperLimit { get; }
	}

	public interface IPCCEngineeringInputData
	{
		MeterPerSecond PCCEnabledSpeed { get; }

		MeterPerSecond MinSpeed { get; }

		Meter PreviewDistanceUseCase1 { get; }

		Meter PreviewDistanceUseCase2 { get; }

		MeterPerSecond Underspeed { get; }

		MeterPerSecond OverspeedUseCase3 { get; }
	}

	public interface IOverSpeedEngineeringInputData : IOverSpeedEcoRollDeclarationInputData
	{
		/// <summary>
		/// P016
		/// cf. VECTO Input Parameters.xlsx
		/// </summary>
		MeterPerSecond MinSpeed { get; }

		/// <summary>
		/// P017
		/// cf. VECTO Input Parameters.xlsx
		/// </summary>
		MeterPerSecond OverSpeed { get; }

	}

	public interface ILookaheadCoastingInputData : IDriverModelData
	{
		/// <summary>
		/// P019
		/// cf. VECTO Input Parameters.xlsx
		/// </summary>
		bool Enabled { get; }

		/// <summary>
		/// P020
		/// cf. VECTO Input Parameters.xlsx
		/// </summary>
		//MeterPerSquareSecond Deceleration { get; }
		/// <summary>
		/// P021
		/// cf. VECTO Input Parameters.xlsx
		/// </summary>
		MeterPerSecond MinSpeed { get; }

		double CoastingDecisionFactorOffset { get; }

		double CoastingDecisionFactorScaling { get; }

		double LookaheadDistanceFactor { get; }

		TableData CoastingDecisionFactorTargetSpeedLookup { get; }

		TableData CoastingDecisionFactorVelocityDropLookup { get; }
	}

	public interface IEngineStopStartEngineeringInputData
	{
		Second ActivationDelay { get; }
		Second MaxEngineOffTimespan { get; }
		double UtilityFactorStandstill { get; }
		double UtilityFactorDriving { get;}
	}

	public interface IAuxiliaryEngineeringInputData
	{

		/// <summary>
		/// P178
		/// additional constant auxiliary load, similar to Padd; not specified in the cycle but as auxiliary
		/// </summary>
		Watt ConstantPowerDemand { get; }

		Watt PowerDemandICEOffDriving { get; }

		Watt PowerDemandICEOffStandstill { get; }


		Watt ElectricPowerDemand { get; }
	}

}
