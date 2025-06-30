using System.Collections.Generic;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.Models.Connector.Ports.Impl;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.Simulation.DataBus;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.OutputData;

namespace TUGraz.VectoCore.Models.SimulationComponent.Impl
{
    public class DCDCConverter : StatefulVectoSimulationComponent<DCDCConverter.State>, IDCDCConverter, IUpdateable
    {
        public double Efficiency { get; protected set; }

        public DCDCConverter(IVehicleContainer container, double efficiency) : base(container)
        {
            Efficiency = efficiency;
            PreviousState.ConsumedEnergy = 0.SI<WattSecond>();
            CurrentState.ConsumedEnergy = 0.SI<WattSecond>();
            PreviousState.stateCount = 0;
            CurrentState.stateCount = 1;
        }


        #region Implementation of IElectricAuxPort

        public Watt Initialize()
        {
            PreviousState.ConsumedEnergy = 0.SI<WattSecond>();
            CurrentState.ConsumedEnergy = 0.SI<WattSecond>();
            _electricConsumers.ForEach(aux => aux.Initialize());
            return 0.SI<Watt>();
        }

        public Watt PowerDemand(Second absTime, Second dt, bool dryRun)
        {
            var electricConsumersPower =
                _electricConsumers.Sum(aux => aux.PowerDemand(absTime, dt, dryRun)).DefaultIfNull(0);
            var energyDemand = PreviousState.ConsumedEnergy + electricConsumersPower * dt;


            var efficiency = energyDemand > 0 ? 1 / Efficiency : Efficiency;

            var powerDemand = energyDemand / dt * efficiency;



            var dischargeEnergy = -DataBus.BatteryInfo.MaxDischargePower(dt) * dt;
            var chargeEnergy = -DataBus.BatteryInfo.MaxChargePower(dt) * dt;


            if (!dryRun) {
                CurrentState.ElectricAuxPower = electricConsumersPower;
            }

            if ((powerDemand * dt).IsBetween(chargeEnergy, dischargeEnergy)) {
                return powerDemand;
            }

            if (!dryRun) {
                CurrentState.MissingEnergy = energyDemand;
            }

            return 0.SI<Watt>();
        }

        #endregion



        #region Overrides of VectoSimulationComponent

        protected override void DoWriteModalResults(Second time, Second simulationInterval, IModalDataContainer container)
        {
            if (CurrentState.MissingEnergy.IsEqual(0)) {
                var consumedEnergy =
                    PreviousState.ConsumedEnergy / simulationInterval + CurrentState.ElectricAuxPower;
                container[ModalResultField.P_DCDC_In] =
                    consumedEnergy / Efficiency;

                container[ModalResultField.P_DCDC_Out] =
                    consumedEnergy;
                container[ModalResultField.P_DCDC_missing] = 0.SI<Watt>();


            } else {
                container[ModalResultField.P_DCDC_In] = 0.SI<Watt>();
                container[ModalResultField.P_DCDC_Out] = 0.SI<Watt>();
                container[ModalResultField.P_DCDC_missing] = CurrentState.MissingEnergy / simulationInterval;

            }
        }

        protected override void DoCommitSimulationStep(Second time, Second simulationInterval)
        {
            var prevState = CurrentState.stateCount;
            AdvanceState();
            CurrentState.stateCount = ++prevState;
        }

        #endregion

        public void ConsumerEnergy(WattSecond electricConsumerEnergy, bool dryRun)
        {
            if (!dryRun) {
                CurrentState.ConsumedEnergy += electricConsumerEnergy;
            }
        }

        public class State
        {
            public State()
            {
                MissingEnergy = 0.SI<WattSecond>();
                ConsumedEnergy = 0.SI<WattSecond>();

                ElectricAuxPower = 0.SI<Watt>();
                simInterval = 0.SI<Second>();
            }

            public Watt ElectricAuxPower { get; set; }

            public WattSecond ConsumedEnergy {
                get;
                set;
            }

            public WattSecond MissingEnergy { get; set; }

            //Debug
            public ulong stateCount { get; set; }

            public Second simInterval { get; set; }



            //

            public State Clone() => (State)MemberwiseClone();
        }

        #region Implementation of IUpdateable
        protected override bool DoUpdateFrom(object other)
        {
            if (other is DCDCConverter d) {
                PreviousState = d.PreviousState.Clone();
                return true;
            }
            return false;
        }
        #endregion

        #region Implementation of IElectricAuxConnector

        private List<IElectricAuxPort> _electricConsumers = new List<IElectricAuxPort>();

        public void Connect(IElectricAuxPort aux)
        {
            _electricConsumers.Add(aux);
        }

        #endregion
    }
}