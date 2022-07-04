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

using System.Linq;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.Connector.Ports.Impl;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.Simulation.DataBus;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.OutputData;

namespace TUGraz.VectoCore.Models.SimulationComponent.Impl
{
	/// <summary>
	/// Driving Cycle for the PWheel driving cycle.
	/// </summary>
	public class PWheelCycle : PowertrainDrivingCycle, IDriverInfo, IVehicleInfo
	{
		protected bool FirstRun = true;
		protected readonly VectoRunData RunData;

		/// <summary>
		/// Initializes a new instance of the <see cref="PWheelCycle"/> class.
		/// </summary>
		/// <param name="container">The container.</param>
		/// <param name="cycle">The cycle.</param>
		public PWheelCycle(IVehicleContainer container, IDrivingCycleData cycle) : base(container, cycle)
		{
			RunData = container.RunData;
		}

		protected virtual void InitializeCycleData()
		{
			FirstRun = false;
			var gearRatios = (RunData.GearboxData != null) 
				? RunData.GearboxData.Gears.ToDictionary(g => g.Key, g => g.Value.Ratio)
				: new System.Collections.Generic.Dictionary<uint, double>() { { 0, 1 } };

			// just to ensure that null-gear has ratio 1
			gearRatios[0] = 1;
			var axleRatio = RunData.AxleGearData.AxleGear.Ratio;

			var emData = RunData.ElectricMachinesData.First().Item2;

			foreach (var entry in Data.Entries)
			{
				if (RunData.JobType == VectoCommon.InputData.VectoSimulationJobType.BatteryElectricVehicle)
				{
					entry.WheelAngularVelocity = entry.AngularVelocity / (axleRatio * gearRatios[entry.Gear] * emData.RatioADC);
				}
				else
				{
					entry.WheelAngularVelocity = entry.AngularVelocity / (axleRatio * gearRatios[entry.Gear]);
				}

				if (entry.WheelAngularVelocity.Value() == 0)
				{
					entry.Torque = NewtonMeter.Create(0);
				}
				else
				{
					entry.Torque = entry.PWheel / entry.WheelAngularVelocity;
				}
			}
		}

		public override IResponse Initialize()
		{
			if (FirstRun) {
				InitializeCycleData();
			   
			}
			var first = Data.Entries[0];
			AbsTime = first.Time;
			var response = NextComponent.Initialize(first.Torque, first.WheelAngularVelocity);
			response.AbsTime = AbsTime;
			return response;
		}

		public override IResponse Request(Second absTime, Second dt)
		{
			if (CycleIterator.LastEntry && CycleIterator.RightSample.Time == absTime) {
				return new ResponseCycleFinished(this);
			}

			SetDriverAction();
			// interval exceeded
			if (CycleIterator.RightSample != null && (absTime + dt).IsGreater(CycleIterator.RightSample.Time)) {
				return new ResponseFailTimeInterval(this) {
					AbsTime = absTime,
					DeltaT = CycleIterator.RightSample.Time - absTime
				};
			}

			return DoHandleRequest(absTime, dt, CycleIterator.LeftSample.WheelAngularVelocity);
		}

		protected override void DoWriteModalResults(Second time, Second simulationInterval, IModalDataContainer container)
		{
			container[ModalResultField.P_wheel_in] = CycleIterator.LeftSample.PWheel;
			base.DoWriteModalResults(time, simulationInterval, container);
		}
		#region IDriverInfo

		public MeterPerSecond VehicleSpeed { get; private set; }

		/// <summary>
		/// True if the angularVelocity at the wheels is 0.
		/// </summary>
		public virtual bool VehicleStopped => CycleIterator.LeftSample.WheelAngularVelocity.IsEqual(0);

		public Kilogram VehicleMass => RunData.VehicleData.TotalCurbMass;

		public Kilogram VehicleLoading => RunData.VehicleData.Loading;

		public Kilogram TotalMass => RunData.VehicleData.TotalVehicleMass;

		public CubicMeter CargoVolume => RunData.VehicleData.CargoVolume;

		public Newton AirDragResistance(MeterPerSecond previousVelocity, MeterPerSecond nextVelocity)
		{
			throw new System.NotImplementedException();
		}

		public Newton RollingResistance(Radian gradient)
		{
			throw new System.NotImplementedException();
		}

		public Newton SlopeResistance(Radian gradient)
		{
			throw new System.NotImplementedException();
		}

		public MeterPerSecond MaxVehicleSpeed => null;
		public MeterPerSquareSecond DriverAcceleration => 0.SI<MeterPerSquareSecond>();
		public PCCStates PCCState => PCCStates.OutsideSegment;
		public MeterPerSecond NextBrakeTriggerSpeed => 0.SI<MeterPerSecond>();
		public DrivingBehavior DriverBehavior { get; internal set; }
		public DrivingAction DrivingAction { get; private set; }

		private PerSquareSecond GetCurrentAcceleration()
		{
			var targetSpeed = CycleIterator.RightSample.AngularVelocity;
			if (targetSpeed.Value().IsEqual(0, 0.5))
			{
				targetSpeed = PerSecond.Create(0);
			}
			var deltaV = targetSpeed - CycleIterator.LeftSample.AngularVelocity;
			var deltaT = CycleIterator.RightSample.Time - AbsTime;
			if (deltaT.IsSmaller(0))
			{
				throw new VectoSimulationException("deltaT is smaller than zero");
			}

			return (deltaV / deltaT);
		}
		private void SetDriverAction()
		{
			var acceleration = GetCurrentAcceleration();
			if (VehicleStopped)
			{
				DrivingAction = DrivingAction.Halt;
				DriverBehavior = DrivingBehavior.Halted;
			}
			else if (acceleration.Value() < 0)
			{
				DrivingAction = DrivingAction.Brake;
				DriverBehavior = DrivingBehavior.Braking;
			}
			else
			{
				DrivingAction = DrivingAction.Accelerate;
				DriverBehavior = DrivingBehavior.Accelerating;
			}
		}

		#endregion
	}
}