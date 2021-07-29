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
using System.Net.NetworkInformation;
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
	}

	public interface IComponentInputData
	{
		DataSource DataSource { get; }
		
		bool SavedInDeclarationMode { get; }

		string Manufacturer { get; }

		string Model { get; }

		string Date { get; }

		CertificationMethod CertificationMethod { get; }

		string CertificationNumber { get; }

		DigestData DigestValue { get; }
	}

	public class DataSource
	{
		public DataSourceType SourceType { get; set; }

		public string SourceFile { get; set; }

		public string SourceVersion { get; set; }

		public string SourcePath
		{
			get { return SourceFile != null ? Path.GetDirectoryName(Path.GetFullPath(SourceFile)) : null; }
		}
	}

	public interface IVehicleDeclarationInputData : IComponentInputData
	{
		string Identifier { get; }

		bool ExemptedVehicle { get; }

		string VIN { get; }

		LegislativeClass LegislativeClass { get; }

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

		TankSystem? TankSystem { get; }

		IAdvancedDriverAssistantSystemDeclarationInputData ADAS { get; }

		// fields for exempted vehicles

		bool ZeroEmissionVehicle { get; }

		bool HybridElectricHDV { get; }

		bool DualFuelVehicle { get; }

		Watt MaxNetPower1 { get; }

		Watt MaxNetPower2 { get; }

		// components

		IVehicleComponentsDeclaration Components { get; }

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

	}

	public interface IAxlesDeclarationInputData
	{
		/// <summary>
		/// parameters for every axle
		/// P044, P045, P046, P047, P048, P108
		/// cf. VECTO Input Parameters.xlsx
		/// </summary>
		IList<IAxleDeclarationInputData> AxlesDeclaration { get; }
	}

	public interface IAdvancedDriverAssistantSystemDeclarationInputData
	{
		bool EngineStopStart { get; }

		EcoRollType EcoRoll { get; }

		PredictiveCruiseControlType PredictiveCruiseControl { get; }
	}

	public enum PredictiveCruiseControlType
	{
		None,
		Option_1_2,
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
			return pcc.ToString().Replace(Prefix, "").Replace(SeparatorEnum, SeparatorXML);
		}

		public static string GetName(this PredictiveCruiseControlType pcc)
		{
			return pcc.ToString().Replace(Prefix, Prefix.Replace(SeparatorEnum, " ")).Replace(SeparatorEnum, "&");
		}
	}

	public enum EcoRollType
	{
		None,
		WithoutEngineStop,
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
	}

	public enum TankSystem
	{
		Liquefied,
		Compressed
	}

	public interface IAirdragDeclarationInputData : IComponentInputData
	{
		/// <summary>
		/// P146, P147  DragCoefficient * Cross Section Area - Rigid
		/// cf. VECTO Input Parameters.xlsx
		/// </summary>
		SquareMeter AirDragArea { get; } // without trailer
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

		/// <summary>
		/// P063
		/// cf. VECTO Input Parameters.xlsx
		/// </summary>
		PerSecond IdleSpeed { get; }

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

		FuelType FuelType { get; }

		/// <summary>
		/// P067
		/// P072, P073, P074
		/// cf. VECTO Input Parameters.xlsx
		/// engine speed in rpm, torque in NM, fuel consumption in g/h
		/// </summary>
		TableData FuelConsumptionMap { get; }

		/// <summary>
		/// P144
		/// P068, P069, P70, P71
		/// cf. VECTO Input Parameters.xlsx
		/// </summary>
		TableData FullLoadCurve { get; }

		Watt RatedPowerDeclared { get; }

		PerSecond RatedSpeedDeclared { get; }

		NewtonMeter MaxTorqueDeclared { get; }
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
		DriverMode Mode { get; }
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

	public interface ITorqueLimitInputData
	{
		int Gear { get; }

		NewtonMeter MaxTorque { get; }
	}
}
