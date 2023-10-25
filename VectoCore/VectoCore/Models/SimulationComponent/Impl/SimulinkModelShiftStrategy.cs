using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.Models.Connector.Ports.Impl;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.Simulation.DataBus;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Utils;
using FmiWrapper_Net;


namespace TUGraz.VectoCore.Models.SimulationComponent.Impl
{
    public class SimulinkModelShiftStrategy : ShiftStrategy
    {
        private FMUConnector Connector;
        protected uint _nextGear;

        public SimulinkModelShiftStrategy(VectoRunData runData, IDataBus dataBus) : base(runData.GearboxData, dataBus)
        {
            //Initialize FMU file?
            string FMUPath = @"C:\Users\Work\Programming\vectosimulinkshiftstrategymodel\AMTShiftStrategyModel.fmu";
            Connector = new FMUConnector();
            Connector.InitFMU(FMUPath);
        }

        ~SimulinkModelShiftStrategy()
        {
           Connector.Step(4, (0.0).SI<Second>(), (0.0).SI<Second>(), (0.0).SI<NewtonMeter>(), (0.0).SI<PerSecond>(), (0.0).SI<NewtonMeter>(), (0.0).SI<PerSecond>(), 0, (0.0).SI<Second>(), DataBus, _gearbox);
           Connector.Terminate();
        }
        public override GearInfo NextGear
        {
            get { return new GearInfo(_nextGear, false); }
        }

        public override void Disengage(Second absTime, Second dt, NewtonMeter outTorque, PerSecond outEngineSpeed)
        {
        }

        public override uint Engage(Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity)
        {
            _nextGear = Connector.Step(2, absTime, dt, outTorque, outAngularVelocity, (0.0).SI<NewtonMeter>(), (0.0).SI<PerSecond>(), 0, (0.0).SI<Second>(), DataBus, _gearbox);
            return _nextGear;
        }


        public override uint InitGear(Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity)
        {
           
            _nextGear = Connector.Step(3, absTime, dt, outTorque, outAngularVelocity, (0.0).SI<NewtonMeter>(), (0.0).SI<PerSecond>(), 0, (0.0).SI<Second>(), DataBus, _gearbox);
            uint tempNextGear = _nextGear;
            if (Connector.InitGearNextGear != 0)
            {
                _nextGear = Connector.InitGearNextGear;
                Connector.InitGearNextGear = 0;
            }
            return tempNextGear;
        }

        public override bool ShiftRequired(Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity, NewtonMeter inTorque, PerSecond inAngularVelocity, uint gear, Second lastShiftTime)
        {
            uint ShiftRequiredValue = Connector.Step(1, absTime, dt, outTorque, outAngularVelocity, inTorque, inAngularVelocity, gear, lastShiftTime, DataBus, _gearbox);
            bool shiftRequired = ShiftRequiredValue == 0 ? false : true;
            return shiftRequired;
        }
    }
}
