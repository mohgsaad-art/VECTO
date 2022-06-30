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
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.Connector.Ports;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.Simulation.DataBus;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Gearbox;

namespace TUGraz.VectoCore.Models.SimulationComponent.Impl
{
	public abstract class AbstractGearbox<TStateType> :
		StatefulProviderComponent<TStateType, ITnOutPort, ITnInPort, ITnOutPort>, ITnOutPort, ITnInPort, IGearbox
		where TStateType : GearboxState, new()
	{
		/// <summary>
		/// The data and settings for the gearbox.
		/// </summary>
		[Required, ValidateObject] internal readonly GearboxData ModelData;

		protected GearshiftPosition _gear;

		public event Action GearShiftTriggered;

		protected AbstractGearbox(IVehicleContainer container) : base(container)
		{
			ModelData = container.RunData.GearboxData;
			LastShift = -double.MaxValue.SI<Second>();
        }

        protected void InvokeGearShiftTriggered()
        { 
			GearShiftTriggered?.Invoke();
		}


        #region ITnOutPort

        public abstract IResponse Request(Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity,
			bool dryRun = false);

		public abstract IResponse Initialize(NewtonMeter outTorque, PerSecond outAngularVelocity);

		#endregion

		#region IGearboxCockpit

		public GearboxType GearboxType => ModelData.Type;

		/// <summary>
		/// The current gear.
		/// </summary>
		public virtual GearshiftPosition Gear
		{
			get => _gear;
			protected internal set => _gear = value;
		}

		public abstract bool TCLocked { get; }

		//[DebuggerHidden]
		//public MeterPerSecond StartSpeed
		//{
		//	get { return ModelData.StartSpeed; }
		//}

		//[DebuggerHidden]
		//public MeterPerSquareSecond StartAcceleration
		//{
		//	get { return ModelData.StartAcceleration; }
		//}

		public Watt GearboxLoss()
		{
			var ratio = ModelData.Gears[PreviousState.Gear.Gear].HasLockedGear
				? ModelData.Gears[PreviousState.Gear.Gear].Ratio
				: ModelData.Gears[PreviousState.Gear.Gear].TorqueConverterRatio;

			return (PreviousState.TransmissionTorqueLoss +
					PreviousState.InertiaTorqueLossOut) / ratio * PreviousState.InAngularVelocity;
		}

		public virtual Second LastShift { get; protected internal set; }

		public abstract Second LastUpshift { get; protected internal set; }

		public abstract Second LastDownshift { get; protected internal set; }

		public GearData GetGearData(uint gear)
		{
			return ModelData.Gears[gear];
		}

		public abstract GearshiftPosition NextGear { get; }

		public virtual Second TractionInterruption => ModelData.TractionInterruption;

		public uint NumGears => (uint)ModelData.Gears.Count;

		#endregion

		public abstract bool GearEngaged(Second absTime);
		public bool RequestAfterGearshift { get; set; }

		protected bool ConsiderShiftLosses(GearshiftPosition nextGear, NewtonMeter torqueOut)
		{
			if (ModelData.Type.ManualTransmission()) {
				return false;
			}
			if (torqueOut.IsSmaller(0)) {
				return false;
			}
			if (nextGear.Gear == 0) {
				return false;
			}
			if (ModelData.Gears[2].HasTorqueConverter) {
				return nextGear.TorqueConverterLocked.HasValue && nextGear.TorqueConverterLocked.Value; // || nextGear.Gear == 2;
			}
			return nextGear.TorqueConverterLocked.HasValue && nextGear.TorqueConverterLocked.Value;
		}

		protected internal WattSecond ComputeShiftLosses(NewtonMeter outTorque, PerSecond outAngularVelocity, GearshiftPosition gear)
		{
			var ratio = ModelData.Gears[gear.Gear].Ratio;
			if (double.IsNaN(ratio)) {
				ratio = ModelData.Gears[gear.Gear].TorqueConverterRatio;
			}
			var torqueGbxIn = outTorque / ratio;
			var deltaClutchSpeed = (DataBus.EngineInfo.EngineSpeed - PreviousState.OutAngularVelocity * ratio) / 2;
			var shiftLossEnergy = torqueGbxIn * deltaClutchSpeed * ModelData.PowershiftShiftTime;

			return shiftLossEnergy.Abs();
		}

		#region Implementation of IGearboxControl

		public abstract bool DisengageGearbox { get; set; }
		public abstract void TriggerGearshift(Second absTime, Second dt);

		#endregion
	}

	public class GearboxState : SimpleComponentState
	{
		public NewtonMeter InertiaTorqueLossOut = 0.SI<NewtonMeter>();
		public NewtonMeter TransmissionTorqueLoss = 0.SI<NewtonMeter>();
		public GearshiftPosition Gear;
		public TransmissionLossMap.LossMapResult TorqueLossResult;
		public DrivingBehavior DrivingBehavior;

		public new GearboxState Clone() => (GearboxState)base.Clone();
	}
}