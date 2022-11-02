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
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.Connector.Ports;
using TUGraz.VectoCore.Models.Connector.Ports.Impl;
using TUGraz.VectoCore.Models.Simulation.DataBus;

namespace TUGraz.VectoCore.Tests.Utils
{
	public class MockTnOutPort : ITnOutPort, IEngineInfo
	{
		protected static readonly LoggingObject Log = LogManager.GetLogger(typeof(MockTnOutPort).FullName);

		public Second AbsTime;
		public Second Dt;
		public NewtonMeter Torque;
		public PerSecond AngularVelocity;

		public IResponse Request(Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity,
			bool dryRun = false)
		{
			AbsTime = absTime;
			Dt = dt;
			Torque = outTorque;
			AngularVelocity = outAngularVelocity;
			Log.Debug("Request: absTime: {0}, dt: {1}, torque: {2}, angularVelocity: {3}", absTime, dt, outTorque,
				outAngularVelocity);

			if (dryRun) {
				return new ResponseDryRun(this) {
					Gearbox = {
						PowerRequest = outTorque * outAngularVelocity,
					},
					Engine = {
						PowerRequest = outTorque * outAngularVelocity,
						EngineSpeed = outAngularVelocity,
						DynamicFullLoadPower = (outTorque + 2300.SI<NewtonMeter>()) * outAngularVelocity,
					},
					Clutch = {
						PowerRequest = outTorque * outAngularVelocity,
					},
					DeltaFullLoad = (outTorque - 2300.SI<NewtonMeter>()) * outAngularVelocity,
					DeltaDragLoad = (outTorque - -100.SI<NewtonMeter>()) * outAngularVelocity
				};
			}

			return new ResponseSuccess(this) {
				Gearbox = {
					PowerRequest = outTorque * outAngularVelocity,
				},
				Engine = {
					PowerRequest = outTorque * outAngularVelocity,
					EngineSpeed = outAngularVelocity,
				},
				Clutch = {
					PowerRequest = outTorque * outAngularVelocity,
				},
			};
		}

		public IResponse Initialize(NewtonMeter outTorque, PerSecond outAngularVelocity)
		{
			return new ResponseSuccess(this) {
				Clutch = {
					PowerRequest = outTorque * (outAngularVelocity ?? 0.SI<PerSecond>()),
				},
				Engine = {
					PowerRequest = outTorque * (outAngularVelocity ?? 0.SI<PerSecond>()),
					EngineSpeed = outAngularVelocity,
				}
			};
		}

		public void DoCommitSimulationStep()
		{
			AbsTime = null;
			Dt = null;
			Torque = null;
			AngularVelocity = null;
		}

		public PerSecond EngineSpeed => AngularVelocity;

		public NewtonMeter EngineTorque => Torque;

		public Watt EngineStationaryFullPower(PerSecond angularSpeed)
		{
			return 2300.SI<NewtonMeter>() * angularSpeed;
		}

		public Watt EngineDynamicFullLoadPower(PerSecond avgEngineSpeed, Second dt)
		{
			throw new NotImplementedException();
		}

		public Watt EngineDragPower(PerSecond angularSpeed)
		{
			return -1000.SI<NewtonMeter>() * angularSpeed;
		}

		public Watt EngineAuxDemand(PerSecond avgEngineSpeed, Second dt)
		{
			throw new NotImplementedException();
		}

		public PerSecond EngineIdleSpeed => 560.RPMtoRad();

		public PerSecond EngineRatedSpeed =>
			// just a test value. not real.
			1600.SI<PerSecond>();

		public PerSecond EngineN95hSpeed { get; set; }
		public PerSecond EngineN80hSpeed { get; set; }

		public bool EngineOn => throw new NotImplementedException();
	}

	public class MockDrivingCycleOutPort : LoggingObject, IDrivingCycleOutPort
	{
		public Second AbsTime;
		public Meter Ds;
		public Second Dt;
		public MeterPerSecond Velocity;
		public Radian Gradient;

		public IResponse Request(Second absTime, Meter ds, MeterPerSecond targetVelocity, Radian gradient)
		{
			AbsTime = absTime;
			Ds = ds;
			Velocity = targetVelocity;
			Gradient = gradient;
			Log.Debug("Request: absTime: {0}, ds: {1}, velocity: {2}, gradient: {3}", absTime, ds, targetVelocity, gradient);
			return new ResponseSuccess(this);
		}

		public IResponse Request(Second absTime, Second dt, MeterPerSecond targetVelocity, Radian gradient)
		{
			AbsTime = absTime;
			Dt = dt;
			Velocity = targetVelocity;
			Gradient = gradient;
			Log.Debug("Request: absTime: {0}, ds: {1}, velocity: {2}, gradient: {3}", absTime, dt, targetVelocity, gradient);
			return new ResponseSuccess(this);
		}

		public IResponse Initialize(MeterPerSecond vehicleSpeed, Radian roadGradient)
		{
			throw new NotImplementedException();
		}

		public IResponse Initialize(MeterPerSecond vehicleSpeed, Radian roadGradient, MeterPerSquareSecond startAcceleration)
		{
			throw new NotImplementedException();
		}
	}

	public class MockFvOutPort : LoggingObject, IFvOutPort
	{
		public Second AbsTime { get; set; }
		public Second Dt { get; set; }
		public Newton Force { get; set; }
		public MeterPerSecond Velocity { get; set; }

		public IResponse Request(Second absTime, Second dt, Newton force, MeterPerSecond velocity, bool dryRun = false)
		{
			AbsTime = absTime;
			Dt = dt;
			Force = force;
			Velocity = velocity;
			Log.Debug("Request: abstime: {0}, dt: {1}, force: {2}, velocity: {3}", absTime, dt, force, velocity);
			return new ResponseSuccess(this);
		}

		public IResponse Initialize(Newton vehicleForce, MeterPerSecond vehicleSpeed)
		{
			return new ResponseSuccess(this);
		}
	}
}