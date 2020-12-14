using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.Simulation.DataBus;

namespace TUGraz.VectoCore.Models.SimulationComponent.Impl
{
    public static class LogShiftData
    {
        static bool firstLine = false;
        const Double MinValue = -2.2251E+307;
        public static void LogShiftDataToExcel(Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity,
            NewtonMeter inTorque, PerSecond inAngularVelocity, uint gear, Second lastShiftTime, IDataBus DataBus, Gearbox gearbox)
        {
            using (var w = new StreamWriter(@"C:\Users\Work\Workspace\Data\Test.csv", true, Encoding.UTF8))
            {
                if(!firstLine)
                {
                    string header = "Time;InAbsTime;InDt;InOutTorque;InInTorque;InOutAngularVelocity;InInAngularVelocity;InGear;InLastShifttime;Gear;TCLocked;StartSpeed;StartAcceleration;LastShift;TractionInterruption;NumGears;AxlegearLoss;EngineSpeed;EngineTorque;EngineIdleSpeed;EngineRatedSpeed;EngineN95hSpeed;EngineN80hSpeed;VehicleSpeed;VehicleStopped;VehicleMass;VehicleLoading;TotalMass;CargoVolume;MaxVehicleSpeed;Distance;BrakePower;ReducedMassWheels;DrivingBehavior;DrivingAction;DriverAcceleration;PTOActive;Altitude;RoadGradient;CycleStartDistance;InLastUpshift;InLastDownshift;GearboxPreviousStateInTorque;GearboxPreviousStateInAngularVelocity;GearboxPreviousStateOutTorque;GearboxPreviousStateOutAngularVelocity;EnginePreviousStateEnginePower;EnginePreviousStateEngineSpeed;EnginePreviousStateEngineTorque;EnginePreviousStateEngineTorqueOut;EnginePreviousStateFullDragTorque;EnginePreviousStateInertiaTorqueLoss;EnginePreviousStateOperationMode;EnginePreviousStateStationaryFullLoadTorque;EnginePreviousStatedt";
                    w.WriteLine(header);
                    firstLine = true;
                }
                //var function = function_;
                var Time = 0;
                var InAbsTime = absTime.Value();
                var InDt = dt.Value();
                var InOutTorque = outTorque.Value();
                var InInTorque = inTorque.Value();
                var InOutAngularVelocity = outAngularVelocity.AsRPM;
                var InInAngularVelocity = inAngularVelocity.AsRPM;
                var InGear = gear;
                var InLastShifttime = (lastShiftTime.Value() == -double.MaxValue.SI().Value()) ? MinValue : lastShiftTime.Value();
                var Gear = gear;
                var TCLocked = DataBus.TCLocked ? 1 : 0;
                var StartSpeed = DataBus.StartSpeed.Value();
                var StartAcceleration = DataBus.StartAcceleration.Value();
                var LastShift = (DataBus.LastShift.Value() == -double.MaxValue.SI().Value()) ? MinValue : DataBus.LastShift.Value();
                var TractionInterruption = DataBus.TractionInterruption.Value();
                var NumGears = DataBus.NumGears;
                var AxlegearLoss = DataBus.AxlegearLoss().Value();
                var EngineSpeed = DataBus.EngineSpeed.AsRPM;
                var EngineTorque = DataBus.EngineTorque.Value(); ;
                var EngineIdleSpeed = DataBus.EngineIdleSpeed.AsRPM;
                var EngineRatedSpeed = DataBus.EngineRatedSpeed.AsRPM;
                var EngineN95hSpeed = DataBus.EngineN95hSpeed.AsRPM;
                var EngineN80hSpeed = DataBus.EngineN80hSpeed.AsRPM;
                var VehicleSpeed = DataBus.VehicleSpeed.Value();
                var VehicleStopped = DataBus.VehicleStopped ? 1 : 0;
                var VehicleMass = DataBus.VehicleMass.Value();
                var VehicleLoading = DataBus.VehicleLoading.Value();
                var TotalMass = DataBus.TotalMass.Value();
                var CargoVolume = DataBus.CargoVolume.Value();
                var MaxVehicleSpeed = DataBus.MaxVehicleSpeed.Value();
                var Distance = DataBus.Distance.Value();
                var BrakePower = DataBus.BrakePower.Value();
                var ReducedMassWheels = DataBus.ReducedMassWheels.Value();
                var DrivingBehavior = (int) DataBus.DriverBehavior;
                var DrivingAction = (int) DataBus.DrivingAction;
                var DriverAcceleration = DataBus.DriverAcceleration.Value();
                var PTOActive = DataBus.PTOActive ? 1 : 0;
                var Altitude = DataBus.Altitude.Value();
                var RoadGradient = DataBus.RoadGradient.Value();
                var CycleStartDistance = DataBus.CycleStartDistance.Value();
                var InLastUpshift = (gearbox.LastUpshift.Value() == -double.MaxValue.SI().Value()) ? MinValue : gearbox.LastUpshift.Value();
                var InLastDownshift = (gearbox.LastDownshift.Value() == -double.MaxValue.SI().Value()) ? MinValue: gearbox.LastDownshift.Value();
                var GearboxPreviousStateInTorque = gearbox.PreviousState.InTorque.Value();
                var GearboxPreviousStateInAngularVelocity = gearbox.PreviousState.InAngularVelocity.AsRPM;
                var GearboxPreviousStateOutTorque = gearbox.PreviousState.OutTorque.Value();
                var GearboxPreviousStateOutAngularVelocity = gearbox.PreviousState.OutAngularVelocity.AsRPM;
                var EnginePreviousStateEnginePower = DataBus.PreviousStateEnginePower.Value();
                var EnginePreviousStateEngineSpeed = DataBus.EngineSpeed.AsRPM;
                var EnginePreviousStateEngineTorque = DataBus.EngineTorque.Value();
                var EnginePreviousStateEngineTorqueOut = DataBus.PreviousStateEngineTorqueOut.Value();
                var EnginePreviousStateFullDragTorque = DataBus.PreviousStateFullDragTorque.Value();
                var EnginePreviousStateInertiaTorqueLoss = DataBus.PreviousStateInertiaTorqueLoss.Value();
                var EnginePreviousStateOperationMode = (int) DataBus.PreviousStateOperationMode;
                var EnginePreviousStateStationaryFullLoadTorque = DataBus.PreviousStateStationaryFullLoadTorque.Value();
                var EnginePreviousStatedt = DataBus.PreviousStateDt.Value();
                //var EngageTime = DataBus.EngageTime?.Value();

                var line = string.Format("{0}; {1}; {2}; {3}; {4}; {5}; {6}; {7}; {8}; {9}; {10}; {11}; {12}; {13}; {14}; {15}; {16}; {17}; {18}; {19}; {20}; {21}; {22}; {23}; {24}; {25}; {26}; {27}; {28}; {29}; {30}; {31}; {32}; {33}; {34}; {35}; {36}; {37}; {38}; {39}; {40}; {41}; {42}; {43}; {44}; {45}; {46}; {47}; {48}; {49}; {50}; {51}; {52}; {53}; {54}",
                    Time, InAbsTime, InDt, InOutTorque, InInTorque, InOutAngularVelocity, InInAngularVelocity, InGear, InLastShifttime, Gear,
                    TCLocked, StartSpeed, StartAcceleration, LastShift, TractionInterruption, NumGears, AxlegearLoss, EngineSpeed, EngineTorque,
                    EngineIdleSpeed, EngineRatedSpeed, EngineN95hSpeed, EngineN80hSpeed, VehicleSpeed, VehicleStopped, VehicleMass, VehicleLoading, TotalMass, CargoVolume,
                    MaxVehicleSpeed, Distance, BrakePower, ReducedMassWheels, DrivingBehavior, DrivingAction, DriverAcceleration, PTOActive, Altitude, RoadGradient, CycleStartDistance,
                    InLastUpshift, InLastDownshift, GearboxPreviousStateInTorque, GearboxPreviousStateInAngularVelocity, GearboxPreviousStateOutTorque, GearboxPreviousStateOutAngularVelocity, EnginePreviousStateEnginePower, EnginePreviousStateEngineSpeed, EnginePreviousStateEngineTorque, EnginePreviousStateEngineTorqueOut,
                    EnginePreviousStateFullDragTorque, EnginePreviousStateInertiaTorqueLoss, EnginePreviousStateOperationMode, EnginePreviousStateStationaryFullLoadTorque, EnginePreviousStatedt);
                w.WriteLine(line);
                w.Flush();
            }
        }
    }
}
