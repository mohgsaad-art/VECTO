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
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.Simulation.DataBus;
using TUGraz.VectoCore.OutputData;

namespace TUGraz.VectoCore.Models.SimulationComponent.Impl
{
    public interface IResetableVectoSimulationComponent
    {
        void Reset(IVehicleContainer vehicleContainer);
    }

    /// <summary>
    /// Base class for all vecto simulation components.
    /// </summary>
    public abstract class VectoSimulationComponent : LoggingObject, IUpdateable
    {
        [NonSerialized] protected IDataBus DataBus;

        /// <summary>
        /// Constructor. Registers the component in the cockpit.
        /// </summary>
        /// <param name="dataBus">The vehicle container</param>
        protected VectoSimulationComponent(IVehicleContainer dataBus)
        {
            DataBus = dataBus;

            // if a component doesn't want to be registered in DataBus, it supplies null to the constructor
            // (mk 2016-08-31: currently the only example is PTOCycleController, in order to not interfere with the real DrivingCycle)
            if (dataBus != null) {
                dataBus.AddComponent(this);
            }
        }

        public virtual void CommitSimulationStep(Second time, Second simulationInterval, IModalDataContainer container)
        {
            if (container != null) {
                DoWriteModalResults(time, simulationInterval, container);
            }
            DoCommitSimulationStep(time, simulationInterval);
        }

        protected abstract void DoWriteModalResults(Second time, Second simulationInterval, IModalDataContainer container);

        /// <summary>
        /// Commits the simulation step.
        /// Writes the moddata into the data writer.
        /// Commits the internal state of the object if needed.
        /// </summary>
        /// <param name="time"></param>
        /// <param name="simulationInterval"></param>
        protected abstract void DoCommitSimulationStep(Second time, Second simulationInterval);

        public virtual bool UpdateFrom(object other)
        {
            if (!DataBus.IsTestPowertrain) {
                throw new VectoException("Only components in a testpowertrain are allowed to be updated!");
            }

            return DoUpdateFrom(other);
        }

        protected abstract bool DoUpdateFrom(object other);

    }



    public abstract class StatefulVectoSimulationComponent<TStateType> : VectoSimulationComponent where TStateType : new()
    {
        protected internal TStateType CurrentState;
        protected internal TStateType PreviousState;

        protected StatefulVectoSimulationComponent(IVehicleContainer container)
            : base(container)
        {
            CurrentState = new TStateType();
            PreviousState = new TStateType();
        }

        protected void AdvanceState()
        {
            PreviousState = CurrentState;
            CurrentState = new TStateType();
        }
    }

    public abstract class StatefulProviderComponent<TStateType, TProviderOutPort, TProviderInPort, TOutPort> :
        StatefulVectoSimulationComponent<TStateType>
        where TStateType : new()
        where TProviderOutPort : class
        where TProviderInPort : class
    {
        protected TOutPort NextComponent;

        protected StatefulProviderComponent(IVehicleContainer container) : base(container) { }

        public TProviderOutPort OutPort()
        {
            return this as TProviderOutPort;
        }

        public TProviderInPort InPort()
        {
            return this as TProviderInPort;
        }

        public virtual void Connect(TOutPort other)
        {
            NextComponent = other;
        }

        protected override void DoCommitSimulationStep(Second time, Second simulationInterval)
        {
            AdvanceState();
        }
    }

    public class SimpleComponentState
    {
        public NewtonMeter OutTorque = 0.SI<NewtonMeter>();
        public NewtonMeter InTorque = 0.SI<NewtonMeter>();

        public PerSecond OutAngularVelocity = 0.SI<PerSecond>();
        public PerSecond InAngularVelocity = 0.SI<PerSecond>();

        public void SetState(NewtonMeter inTorque, PerSecond inAngularVelocity, NewtonMeter outTorque,
            PerSecond outAngularVelocity)
        {
            InTorque = inTorque;
            InAngularVelocity = inAngularVelocity;
            OutTorque = outTorque;
            OutAngularVelocity = outAngularVelocity;
        }

        public SimpleComponentState Clone() => (SimpleComponentState)MemberwiseClone();
    }
}