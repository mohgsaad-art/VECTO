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
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCommon.InputData;
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
			var axleRatio = (RunData.AxleGearData != null) ? RunData.AxleGearData.AxleGear.Ratio : 1;

			/* For BEVs, ratioADC must participate in the calculation of the wheel angular velocity. */
			var emData = (RunData.ElectricMachinesData.Count > 0) ? RunData.ElectricMachinesData.First().Item2 : null;
			var ratioADC = (RunData.JobType == VectoSimulationJobType.BatteryElectricVehicle) ? emData.RatioADC : 1;
					
			foreach (var entry in Data.Entries) {
				entry.WheelAngularVelocity = entry.AngularVelocity / (axleRatio * gearRatios[entry.Gear] * ratioADC);

				entry.VehicleTargetSpeed = entry.WheelAngularVelocity * RunData.VehicleData.DynamicTyreRadius;
				
				entry.Torque = !entry.WheelAngularVelocity.IsEqual(0) 
					? entry.PWheel / entry.WheelAngularVelocity 
					: 0.SI<NewtonMeter>();
			}
		}

		public override IResponse Initialize()
		{
			if ((RunData.JobType == VectoSimulationJobType.BatteryElectricVehicle) && (DataBus.GearboxCtl != null)) {
				DataBus.GearboxCtl.GearShiftTriggered -= GearShiftTriggered;
				DataBus.GearboxCtl.GearShiftTriggered += GearShiftTriggered;
            }

			if (FirstRun) {
				InitializeCycleData();
			   
			}
			var first = Data.Entries[0];
			AbsTime = first.Time;
			var response = NextComponent.Initialize(first.Torque, first.WheelAngularVelocity);
			response.AbsTime = AbsTime;
			return response;
		}

		private void GearShiftTriggered()
        {
			/* Set driving action to roll, on gear change trigger, in order to replicate distance-based mode driver signals. */

			if (DrivingAction == DrivingAction.Accelerate) {
				DriverBehavior = DrivingBehavior.Driving;	
				DrivingAction = DrivingAction.Roll;
			}
        }

		public override IResponse Request(Second absTime, Second dt)
		{
			if (CycleIterator.LastEntry && CycleIterator.RightSample.Time == absTime) {
				return new ResponseCycleFinished(this);
			}

			// interval exceeded
			if (CycleIterator.RightSample != null && (absTime + dt).IsGreater(CycleIterator.RightSample.Time)) {
				return new ResponseFailTimeInterval(this) {
					AbsTime = absTime,
					DeltaT = CycleIterator.RightSample.Time - absTime
				};
			}

			DetermineDriverAction();

			return DoHandleRequest(absTime, dt, CycleIterator.LeftSample.WheelAngularVelocity);
		}

		protected override void DoWriteModalResults(Second time, Second simulationInterval, IModalDataContainer container)
		{
			container[ModalResultField.P_wheel_in] = CycleIterator.LeftSample.PWheel;
			container.SetDataValue("DriverAction", (int) DataBus.DriverInfo.DrivingAction);
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
		
		public DrivingBehavior DriverBehavior { get; internal set; } = DrivingBehavior.Driving;
		
		public DrivingAction DrivingAction { get; private set; } = DrivingAction.Accelerate;

		public MeterPerSquareSecond DriverAcceleration => 0.SI<MeterPerSquareSecond>();
		public PCCStates PCCState => PCCStates.OutsideSegment;
		public MeterPerSecond NextBrakeTriggerSpeed => 0.SI<MeterPerSecond>();

		private void DetermineDriverAction()
		{
			if (RunData.JobType == VectoSimulationJobType.BatteryElectricVehicle) {
				DetermineDriverActionForBEV();
            }
        }

        private void DetermineDriverActionForBEV()
        {
			if (VehicleStopped) {
				DrivingAction = DrivingAction.Halt;
				DriverBehavior = DrivingBehavior.Halted;
			}
			else if ((CycleIterator.LeftSample.PWheel.Value() < 0) && (DrivingAction != DrivingAction.Roll)) {
				DrivingAction = DrivingAction.Brake;
				DriverBehavior = DrivingBehavior.Braking;
			}
			else {
				DrivingAction = DataBus.GearboxInfo.GearEngaged(DataBus.AbsTime) ? DrivingAction.Accelerate : DrivingAction.Roll;
				DriverBehavior = DrivingBehavior.Driving;
			}
		}

        #endregion
    }
}