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
using System.Diagnostics;
using System.Linq;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Gearbox;
using TUGraz.VectoCore.Models.SimulationComponent.Impl;

namespace TUGraz.VectoCore.Models.Connector.Ports.Impl
{
	public abstract class AbstractResponse : IResponse
	{
		public AbstractResponse(object source)
		{
			Source = source;
			Driver = new DriverResponse();
			Engine = new EngineResponse();
			Clutch = new ClutchResponse();
			Gearbox = new GearboxResponse();
			Axlegear = new AxlegearResponse();
			Angledrive = new AngledriveResponse();
			Wheels = new WheelsResponse();
			Vehicle = new VehicleResponse();
			Brakes = new BrakesResponse();
			ElectricMotor = new ElectricMotorResponse();
			//ElectricSystem = new
			TorqueConverter = new TorqueConverterResponse();
			HybridController = new HybridControllerResponse();
		}

		public AbstractResponse(object source, IResponse subResponse)
		{
			Source = source;
			Driver = subResponse.Driver;
			Engine = subResponse.Engine;
			Clutch = subResponse.Clutch;
			Gearbox = subResponse.Gearbox;
			Axlegear = subResponse.Axlegear;
			Angledrive = subResponse.Angledrive;
			Wheels = subResponse.Wheels;
			Vehicle = subResponse.Vehicle;
			Brakes = subResponse.Brakes;
			ElectricMotor = subResponse.ElectricMotor;
			ElectricSystem = subResponse.ElectricSystem;
			TorqueConverter = subResponse.TorqueConverter;
			HybridController = subResponse.HybridController;
		}

		public Second AbsTime { get; set; }
		public Second SimulationInterval { get; set; }
		public Meter SimulationDistance { get; set; }

		public DriverResponse Driver { get; }

		public EngineResponse Engine { get; }

		public ClutchResponse Clutch { get; }

		public GearboxResponse Gearbox { get; }

		public TorqueConverterResponse TorqueConverter { get; }

		public AxlegearResponse Axlegear { get; }

		public AngledriveResponse Angledrive { get; }
		public WheelsResponse Wheels { get; }
		public VehicleResponse Vehicle { get; }
		public BrakesResponse Brakes { get; }
		public ElectricMotorResponse ElectricMotor { get; }

		public object Source { get; }

		public IElectricSystemResponse ElectricSystem { get; set; }

		public HybridControllerResponse HybridController { get; set; }
		
		//public override string ToString()
		//{
		//	var t = GetType();
		//	return $"{t.Name}{{{t.GetProperties().Select(p => $"{p.Name}: {p.GetValue(this)}").Join()}}}";
		//}
	}

	/// <summary>
	/// Response when the Cycle is finished.
	/// </summary>
	[DebuggerDisplay("CycleFinished")]
	public class ResponseCycleFinished : AbstractResponse {
		public ResponseCycleFinished(object source) : base(source) { }
	}

	/// <summary>
	/// Response when a request was successful.
	/// </summary>
	[DebuggerDisplay("Success({AbsTime,nq}, {Driver.OperatingPoint,nq})")]
	public class ResponseSuccess : AbstractResponse {
		public ResponseSuccess(object source) : base(source) { }
	}

	[DebuggerDisplay("BatteryEmpty")]
	public class ResponseBatteryEmpty : AbstractResponse
	{
		public ResponseBatteryEmpty(object source, IElectricSystemResponse electricSupplyResponse) : base(source) { }
	}

	/// <summary>
	/// Response when the request resulted in an engine or gearbox overload. 
	/// </summary>
	[DebuggerDisplay("Overload({Delta,nq})")]
	public class ResponseOverload : AbstractResponse
	{
		public Watt Delta { get; set; }
		public ResponseOverload(object source) : base(source) { }
	}

	/// <summary>
	/// Response when the request resulted in an engine under-load. 
	/// </summary>
	[DebuggerDisplay("Underload({Delta,nq})")]
	public class ResponseUnderload : AbstractResponse
	{
		public Watt Delta { get; set; }
		public ResponseUnderload(object source) : base(source) { }
	}

	/// <summary>
	/// Response when the Speed Limit was exceeded.
	/// </summary>
	[DebuggerDisplay("SpeedLimitExceeded")]
	public class ResponseSpeedLimitExceeded : AbstractResponse
	{
		public ResponseSpeedLimitExceeded(object source) : base(source) { }
	}

	/// <summary>
	/// Response when the request should have another time interval.
	/// </summary>
	[DebuggerDisplay("FailTimeInterval({DeltaT,nq})")]
	public class ResponseFailTimeInterval : AbstractResponse
	{
		public ResponseFailTimeInterval(object source) : base(source) { }

		public Second DeltaT { get; set; }
	}

	[DebuggerDisplay("DistanceExceeded({MaxDistance,nq})")]
	public class ResponseDrivingCycleDistanceExceeded : AbstractResponse
	{
		public ResponseDrivingCycleDistanceExceeded(object source) : base(source) { }
		public Meter MaxDistance { get; set; }
	}

	[DebuggerDisplay("DryRun({DeltaFullLoad,nq}, {DeltaDragLoad,nq}, {DeltaEngineSpeed,nq})")]
	public class ResponseDryRun : AbstractResponse
	{
		public ResponseDryRun(object source) : base(source) { }

		public ResponseDryRun(object source, IResponse subResponse) : base(source, subResponse) { }

		public Watt DeltaFullLoad { get; set; }

		public NewtonMeter DeltaFullLoadTorque { get; set; }
		public Watt DeltaDragLoad { get; set; }

		public NewtonMeter DeltaDragLoadTorque { get; set; }

		public PerSecond DeltaEngineSpeed { get; set; }
	}

	[DebuggerDisplay("GearShift")]
	internal class ResponseGearShift : AbstractResponse
	{
		public ResponseGearShift(object source) : base(source) { }

		public ResponseGearShift(object source, IResponse subResponse) : base(source, subResponse) { }
	}

	[DebuggerDisplay("DifferentGearEngaged")]
	internal class ResponseDifferentGearEngaged : AbstractResponse
	{
		public ResponseDifferentGearEngaged(object source) : base(source) { }
	}

	[DebuggerDisplay("InvalidOperatingPoint")]
	internal class ResponseInvalidOperatingPoint : AbstractResponse
	{
		public ResponseInvalidOperatingPoint(object source) : base(source) { }
	}

	/*
		internal class ResponseEngineSpeedTooLow : ResponseDryRun {}
	*/

	[DebuggerDisplay("EngineSpeedTooHigh({DeltaEngineSpeed,nq})")]
	internal class ResponseEngineSpeedTooHigh : AbstractResponse
	{
		public ResponseEngineSpeedTooHigh(object source) : base(source) { }

		public PerSecond DeltaEngineSpeed { get; set; }
	}
}