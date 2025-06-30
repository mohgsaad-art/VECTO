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

using System.Diagnostics;
using System.Linq;
using TUGraz.VectoCommon.Utils;

namespace TUGraz.VectoCommon.Models
{
	public abstract class AbstractComponentResponse
	{
		public Watt PowerRequest { get; set; }

		public override string ToString()
		{
			var t = GetType();
			return $"{t.Name}{{{t.GetProperties().Select(p => $"{p.Name}: {p.GetValue(this)}").Join()}}}";
		}
	}

	public abstract class AbstractPowertrainComponentResponse : AbstractComponentResponse
	{

	}


	public class DriverResponse : AbstractComponentResponse
	{
		public MeterPerSquareSecond Acceleration { get; set; }
		public OperatingPoint OperatingPoint { get; set; }

	}

	[DebuggerDisplay("n_ice: {EngineSpeed?.AsRPM}; T_out: {TorqueOutDemand}; T_ice: {TotalTorqueDemand}; T_full_dyn: {DynamicFullLoadTorque}; P_full_dyn: {DynamicFullLoadPower}; P_drag: {DragPower}; P_aux: {AuxiliariesPowerDemand}")]

	public class EngineResponse : AbstractPowertrainComponentResponse
	{
		public bool EngineOn { get; set; }
		public PerSecond EngineSpeed { get; set; }

		public NewtonMeter TorqueOutDemand { get; set; }
		public NewtonMeter TotalTorqueDemand { get; set; }
		public NewtonMeter DynamicFullLoadTorque { get; set; }

		public NewtonMeter StationaryFullLoadTorque { get; set; }

		public Watt DynamicFullLoadPower { get; set; }
		public Watt DragPower { get; set; }

		public NewtonMeter DragTorque { get; set; }

		public Watt AuxiliariesPowerDemand { get; set; }
	}



	[DebuggerDisplay("P_out: {PowerRequest}")]
	public class ClutchResponse : AbstractPowertrainComponentResponse
	{
		public PerSecond OutputSpeed { get; set; }
	}

	[DebuggerDisplay("P_out: {PowerRequest}")]
	public class GearboxResponse : AbstractPowertrainComponentResponse
	{
		public PerSecond InputSpeed { get; set; }

		public NewtonMeter InputTorque { get; set; }

		public GearshiftPosition Gear { get; set; }

		public PerSecond OutputSpeed { get; set; }

		public NewtonMeter OutputTorque { get; set; }
	}

	public class TorqueConverterResponse : AbstractPowertrainComponentResponse
	{
		public TorqueConverterOperatingPoint TorqueConverterOperatingPoint { get; set; }

		public NewtonMeter TorqueConverterTorqueDemand { get; set; }

	}

	[DebuggerDisplay("P_out: {PowerRequest}; T_card: {CardanTorque}")]
	public class AxlegearResponse : AbstractPowertrainComponentResponse
	{
		public NewtonMeter CardanTorque { get; set; }
		
		public NewtonMeter OutputTorque { get; set; }

		public PerSecond OutputSpeed { get; set; }
	}

	[DebuggerDisplay("P_out: {PowerRequest}")]
	public class AngledriveResponse : AbstractPowertrainComponentResponse
	{
		public NewtonMeter OutputTorque { get; set; }

		public PerSecond OutputSpeed { get; set; }

	}

	[DebuggerDisplay("P_out: {PowerRequest}")]
	public class WheelsResponse : AbstractPowertrainComponentResponse { }

	[DebuggerDisplay("v_veh: {VehicleSpeed}")]

	public class VehicleResponse : AbstractComponentResponse
	{
		public MeterPerSecond VehicleSpeed { get; set; }
	}

	[DebuggerDisplay("P_brake: {BrakePower}")]

	public class BrakesResponse : AbstractComponentResponse
	{
		public Watt BrakePower { get; set; }
	}

	[DebuggerDisplay("P_em_mech: {ElectricMotorPowerMech}")]

	public class ElectricMotorResponse : AbstractComponentResponse
	{
		public Watt ElectricMotorPowerMech { get; set; }

		public NewtonMeter MaxDriveTorque { get; set; }

		public NewtonMeter MaxRecuperationTorque { get; set; }

		public PerSecond AngularVelocity { get; set; }

		public SIBase<Watt> InertiaPowerDemand { get; set; }
		public NewtonMeter TotalTorqueDemand { get; set; }
		public NewtonMeter TorqueRequest { get; set; }
		public NewtonMeter InertiaTorque { get; set; }
		public PerSecond AvgDrivetrainSpeed { get; set; }
		public NewtonMeter MaxDriveTorqueEM { get; set; }
		public NewtonMeter MaxRecuperationTorqueEM { get; set; }
		public NewtonMeter TorqueRequestEmMap { get; set; }
		public bool DeRatingActive { get; set; }
	}


	/// <summary>
	/// The Interface for a Response. Carries over result data to higher components.
	/// </summary>
	public interface IResponse
	{
		object Source { get; }

		Second AbsTime { get; set; }
		Meter SimulationDistance { get; set; }
		Second SimulationInterval { get; set; }

		DriverResponse Driver { get; }

		EngineResponse Engine { get; }

		ClutchResponse Clutch { get; }

		GearboxResponse Gearbox { get; }

		TorqueConverterResponse TorqueConverter { get; }

		AxlegearResponse Axlegear { get; }

		AngledriveResponse Angledrive { get; }
		WheelsResponse Wheels { get; }

		VehicleResponse Vehicle { get; }

		BrakesResponse Brakes { get; }

		ElectricMotorResponse ElectricMotor { get; }

		IElectricSystemResponse ElectricSystem { get; set; }

		HybridControllerResponse HybridController { get; set; }
	}

	public class HybridControllerResponse : AbstractComponentResponse
	{
		public HybridStrategyResponse StrategySettings { get; set; }
	}

	public interface IRESSResponse
	{
		Second AbsTime { get; set; }

		Second SimulationInterval { get; set; }

		Watt MaxChargePower { get; set; }

		Watt MaxDischargePower { get; set; }

		Watt PowerDemand { get; set; }

		Watt LossPower { get; set; }

		double StateOfCharge { get; set; }

		Volt InternalVoltage { get; set; }

		object Source { get; }
	}

	public interface IElectricSystemResponse
	{
		IRESSResponse RESSResponse { get; set; }

        Watt MaxNominalFCRatedPower { get; set; }

        Watt AuxPower { get; set; }

		Watt ConsumerPower { get; set; }

		Watt ChargingPower { get; set; }

		Watt MaxPowerDrive { get; }

		Watt MaxPowerDrag { get; }

		Watt RESSPowerDemand { get; set; }

		object Source { get; }
	}
}