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
using System.Diagnostics;
using System.Runtime.CompilerServices;
using TUGraz.VectoCommon.BusAuxiliaries;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.Declaration;

[assembly: InternalsVisibleTo("VECTO3GUI2020")]

namespace TUGraz.VectoCore.InputData.Impl
{
	public class CycleInputData : ICycleData
	{
		public string Name { get; internal set; }

		public TableData CycleData { get; internal set; }
	}

	public class LookAheadCoastingInputData : ILookaheadCoastingInputData
	{
		public bool Enabled { get; internal set; }

		//public MeterPerSquareSecond Deceleration { get; internal set; }

		public MeterPerSecond MinSpeed { get; internal set; }

		public double CoastingDecisionFactorOffset { get; internal set; }
		public double CoastingDecisionFactorScaling { get; internal set; }
		public double LookaheadDistanceFactor { get; internal set; }
		public TableData CoastingDecisionFactorTargetSpeedLookup { get; internal set; }
		public TableData CoastingDecisionFactorVelocityDropLookup { get; internal set; }
	}

	public class OverSpeedInputData : IOverSpeedEngineeringInputData
	{
		public bool Enabled { get; internal set; }

		public MeterPerSecond MinSpeed { get; internal set; }

		public MeterPerSecond OverSpeed { get; internal set; }

	}

	public class DriverAccelerationInputData : IDriverAccelerationData
	{
		#region Implementation of IDriverAccelerationData

		public TableData AccelerationCurve { get; internal set; }

		#endregion
	}


	public class TransmissionInputData : ITransmissionInputData
	{
		public int Gear { get; internal set; }

		public double Ratio { get; internal set; }

		public TableData LossMap { get; internal set; }

		public double Efficiency { get; internal set; }

		public NewtonMeter MaxTorque { get; internal set; }

		public PerSecond MaxInputSpeed { get; internal set; }

		public TableData ShiftPolygon { get; internal set; }
		public DataSource DataSource { get; internal set; }
	}

	public class AxleInputData : IAxleEngineeringInputData
	{
		public bool TwinTyres { get; internal set; }

		public bool Steered { get; internal set; }

		public AxleType AxleType { get; internal set; }

		ITyreDeclarationInputData IAxleDeclarationInputData.Tyre => Tyre;

		public ITyreEngineeringInputData Tyre { get; internal set; }


		public double AxleWeightShare { get; internal set; }

		public DataSource DataSource { get; internal set; }
	}

	public class TyreInputData : ITyreEngineeringInputData
	{
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design",
			"CA1065:DoNotRaiseExceptionsInUnexpectedLocations")]
		public DataSource DataSource { get; internal set; }

		public string Source { get; internal set; }

		public bool SavedInDeclarationMode { get; internal set; }

		public string Manufacturer { get; internal set; }

		public string Model { get; internal set; }

		public DateTime Date { get; internal set; }
		public string AppVersion { get; internal set; }

		public CertificationMethod CertificationMethod { get; internal set; }

		public string CertificationNumber { get; internal set; }

		public DigestData DigestValue { get; internal set; }

		public string Dimension { get; internal set; }

		public double RollResistanceCoefficient { get; internal set; }

		public Newton TyreTestLoad { get; internal set; }
		public string FuelEfficiencyClass { get; internal set; }

		public KilogramSquareMeter Inertia { get; internal set; }

		public Meter DynamicTyreRadius { get; internal set; }
	}

	public class DeclarationAuxiliaryDataInputData : IAuxiliaryDeclarationInputData
	{
		public DeclarationAuxiliaryDataInputData()
		{
			AuxiliaryType = AuxiliaryDemandType.Constant;
		}

		public AuxiliaryDemandType AuxiliaryType { get; internal set; }

		public string ID { get; internal set; }

		public AuxiliaryType Type { get; set; }

		public IList<string> Technology { get; set; }


	}

	public class EngineeringAuxiliaryDataInputData : IAuxiliaryEngineeringInputData
	{
		public EngineeringAuxiliaryDataInputData()
		{
			AuxiliaryType = AuxiliaryDemandType.Constant;
			ConstantPowerDemand = 0.SI<Watt>();
		}

		public AuxiliaryDemandType AuxiliaryType { get; internal set; }

		public string ID { get; internal set; }

		public Watt ConstantPowerDemand { get; internal set; }
		public Watt PowerDemandICEOffDriving { get; internal set; }
		public Watt PowerDemandICEOffStandstill { get; internal set; }
		public Watt ElectricPowerDemand { get; internal set; }
	}


	public class TorqueLimitInputData : ITorqueLimitInputData
	{
		public int Gear { get; internal set; }
		public NewtonMeter MaxTorque { get; internal set; }
	}

	public class AlternatorInputData : IAlternatorDeclarationInputData
	{
		public AlternatorInputData(Volt ratedVoltage, Ampere ratedCurrent)
		{
			RatedCurrent = ratedCurrent;
			RatedVoltage = ratedVoltage;

		}

		#region Implementation of IAlternatorDeclarationInputData
		
		public Ampere RatedCurrent { get; }
		public Volt RatedVoltage { get; }

		#endregion
	}

	public class BusAuxBatteryInputData : IBusAuxElectricStorageDeclarationInputData
	{
		public BusAuxBatteryInputData(string technology, Volt voltage, AmpereSecond ratedCapacity)
		{
			Technology = technology;
			Voltage = voltage;
			Capacity = ratedCapacity;
		}

		public Volt Voltage { get; set; }

		public AmpereSecond Capacity { get; set; }

		#region Implementation of IBusAuxElectricStorageDeclarationInputData

		public string Technology { get; }

		public WattSecond ElectricStorageCapacity => Capacity * Voltage;

		#endregion
	}

	public class BusAuxCapacitorInputData : IBusAuxElectricStorageDeclarationInputData
	{
		public BusAuxCapacitorInputData(string technology, Volt voltage, Farad ratedCapacity)
		{
			Technology = technology;
			Voltage = voltage;
			Capacity = ratedCapacity;
		}

		public Volt Voltage { get; set; }

		public Farad Capacity { get; set; }

		#region Implementation of IBusAuxElectricStorageDeclarationInputData

		public string Technology { get; }
		public WattSecond ElectricStorageCapacity => Capacity * Voltage * Voltage / 2.0;

		#endregion
	}



	public class ResultInputData : IResultsInputData
	{
		public string Status { get; internal set; }

		public IList<IResult> Results { get; internal set; }
	}
	
	[DebuggerDisplay("{ResultStatus} | {VehicleGroup} {Mission}")]
	public class Result : IResult
	{
		public string ResultStatus { get; internal set; }
		public VehicleClass VehicleGroup { get; internal set; }
		public MissionType Mission { get; internal set; }
		public ISimulationParameter SimulationParameter { get; internal set; }
		public Dictionary<FuelType, JoulePerMeter> EnergyConsumption { get; set; }
		public Dictionary<string, double> CO2 { get; set; }
	}

	public class SimulationParameter : ISimulationParameter
	{
		public Kilogram TotalVehicleMass { get; internal set; }
		public Kilogram Payload { get; internal set; }
		public double PassengerCount { get; internal set; }
		public string FuelMode { get; internal set; }
	}

	public class ApplicationInformation : IApplicationInformation
	{
		public string SimulationToolVersion { get; internal set; }
		public DateTime Date { get; internal set; }
	}

	public class ElectricMotorVoltageLevel : IElectricMotorVoltageLevel
	{
		#region Implementation of IElectricMotorVoltageLevel

		public Volt VoltageLevel { get; internal set; }
		public NewtonMeter ContinuousTorque { get; internal set; }
		public PerSecond ContinuousTorqueSpeed { get; internal set; }
		public NewtonMeter OverloadTorque { get; internal set; }
		public PerSecond OverloadTestSpeed { get; internal set; }
		public Second OverloadTime { get; internal set; }
		public TableData FullLoadCurve { get; internal set; }
		//public TableData EfficiencyMap { get; internal set; }
		public IList<IElectricMotorPowerMap> PowerMap { get; internal set; }

		#endregion
	}
}