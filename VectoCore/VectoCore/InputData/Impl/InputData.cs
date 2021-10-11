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
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;

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

		ITyreDeclarationInputData IAxleDeclarationInputData.Tyre
		{
			get { return Tyre; }
		}

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

		public string Date { get; internal set; }

		public CertificationMethod CertificationMethod { get; internal set; }

		public string CertificationNumber { get; internal set; }

		public DigestData DigestValue { get; internal set; }

		public string Dimension { get; internal set; }

		public double RollResistanceCoefficient { get; internal set; }

		public Newton TyreTestLoad { get; internal set; }

		public KilogramSquareMeter Inertia { get; internal set; }

		public Meter DynamicTyreRadius { get; }
	}

	public class AuxiliaryDataInputData : IAuxiliaryEngineeringInputData, IAuxiliaryDeclarationInputData
	{
		public AuxiliaryDataInputData()
		{
			AuxiliaryType = AuxiliaryDemandType.Mapping;
			ConstantPowerDemand = 0.SI<Watt>();
		}

		public AuxiliaryDemandType AuxiliaryType { get; internal set; }

		public string ID { get; internal set; }

		public AuxiliaryType Type { get; set; }

		public IList<string> Technology { get; set; }

		public double TransmissionRatio { get; internal set; }

		public double EfficiencyToEngine { get; internal set; }

		public double EfficiencyToSupply { get; internal set; }

		public TableData DemandMap { get; internal set; }

		public Watt ConstantPowerDemand { get; internal set; }

		public DataSource DataSource { get; internal set; }
	}

	public class TorqueLimitInputData : ITorqueLimitInputData
	{
		public int Gear { get; internal set; }
		public NewtonMeter MaxTorque { get; internal set; }
	}
}