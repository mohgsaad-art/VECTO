using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using FmiWrapper_Net;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.Simulation.DataBus;
using TUGraz.VectoCore.Models.SimulationComponent.Impl;

namespace TUGraz.VectoCore.Utils
{
    public class FMUConnector
    {
        const Double MinValue = -2.2251E+307;
        private string ModelDescriptionGuid { get; set; }
        private string ModelIdentifier { get; set; }
        private Dictionary<string, uint> InputDict { get; set; }
        private Dictionary<string, uint> OutputDict { get; set; }
        private FmuInstance Instance { get; set; }

        public uint InitGearNextGear = 0;
        private decimal Time { get; set; }
        private string FmuDLL { get; set; }
        public void InitFMU(string fmuPath)
        {
            Time = 0;
            string fmuDLL = LoadAndCopyFMUArchive(fmuPath);
            FmuDLL = fmuDLL;
            FmuInstance fmu_instance = new FmuInstance(fmuDLL);
            
            fmu_instance.Log += Fmu_instance_Log;
            fmu_instance.StepFinished += Fmu_step_finished;

            fmu_instance.Instantiate(ModelIdentifier,
                  Fmi2Type.fmi2CoSimulation,
                  ModelDescriptionGuid,
                  "file:///C:\\Users\\Work\\Programming\\vectosimulinkshiftstrategymodel\\AMTShiftStrategyModel_grt_fmi_rtw\\AMTShiftStrategyModel\\resources",
                  false,
                  true);

            
            Fmi2Status experiment_status = fmu_instance.SetupExperiment(false, 0, 0, false, 10);
            //Console.WriteLine("Setup Experiment status: " + experiment_status);

            Fmi2Status initalization_mode_status = fmu_instance.EnterInitializationMode();
            //Console.WriteLine("Initalization mode status: " + initalization_mode_status);
            fmu_instance.ExitInitializationMode();

            Instance = fmu_instance;

        }

        ~FMUConnector()
        {
            //File.Delete(FmuDLL);
        }


        public void Terminate()
        {
            Instance.Terminate();
        }
        public uint Step(int function_, Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity, NewtonMeter inTorque, PerSecond inAngularVelocity, uint gear, Second lastShiftTime, IDataBus DataBus, Gearbox gearbox)
        {
            //LogShiftData.LogFunctionDataToExcel(function_, absTime, dt, outTorque, outAngularVelocity, inTorque, inAngularVelocity, gear, lastShiftTime, DataBus, gearbox);
            //set Inputs
            if (function_ != 4)
                StepSetInput(function_, absTime, dt, outTorque, outAngularVelocity, inTorque, inAngularVelocity, gear, lastShiftTime, DataBus, gearbox);

            //doStep
            Fmi2Status step_status = Instance.DoStep((double)Time, 0.1, true);

            //getOutput
            uint[] getOutputName = { OutputDict["OutShiftRequired"] };
            double[] getOutput = new double[getOutputName.Length];
            Instance.GetReal(getOutputName, getOutput);

            uint[] getOutShiftRequiredName = { OutputDict["OutShiftRequired"] };
            double[] getOutShiftRequired = new double[getOutShiftRequiredName.Length];
            Instance.GetReal(getOutShiftRequiredName, getOutShiftRequired);

            uint[] getOutNextGearName = { OutputDict["OutNextGear"] };
            double[] getOutNextGear = new double[getOutNextGearName.Length];
            Instance.GetReal(getOutNextGearName, getOutNextGear);

            switch(function_)
            {
                case 1: 
                    return (uint)getOutShiftRequired[0];
                case 2:
                    return (uint)getOutNextGear[0];
                case 3:
                    if ((int)getOutShiftRequired[0] != -1)
                    {
                        InitGearNextGear = (uint)getOutShiftRequired[0];

                    }
                    return (uint)getOutNextGear[0];
                default:
                    break;
            }
            Time = Time + 0.1m;
            if (getOutput.Length > 0 && getOutput[0] > 0)
                return (uint)getOutput[0];
            else
                return 0;
        }

        private void StepSetInput(int function_, Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity, NewtonMeter inTorque, PerSecond inAngularVelocity, uint gear, Second lastShiftTime, IDataBus DataBus, Gearbox gearbox)
        {
            var StrategyType = function_;
            SetInput(new uint[] { InputDict["StrategyType"] }, new double[] { StrategyType });
            var InAbsTime = absTime.Value();
            SetInput(new uint[] { InputDict["InAbsTime"] }, new double[] { InAbsTime });
            var InDt = dt.Value();
            SetInput(new uint[] { InputDict["InDt"] }, new double[] { InDt });
            var InOutTorque = outTorque.Value();
            SetInput(new uint[] { InputDict["InOutTorque"] }, new double[] { InOutTorque });
            var InInTorque = inTorque.Value();
            SetInput(new uint[] { InputDict["InInTorque"] }, new double[] { InInTorque });
            var InOutAngularVelocity = outAngularVelocity.AsRPM;
            SetInput(new uint[] { InputDict["InOutAngularVelocity"] }, new double[] { InOutAngularVelocity });
            var InInAngularVelocity = inAngularVelocity.AsRPM;
            SetInput(new uint[] { InputDict["InInAngularVelocity"] }, new double[] { InInAngularVelocity });
            var InGear = gear;
            SetInput(new uint[] { InputDict["InGear"] }, new double[] { InGear });
            var InLastShifttime = (lastShiftTime.Value() == -double.MaxValue.SI().Value()) ? MinValue : lastShiftTime.Value();
            SetInput(new uint[] { InputDict["InLastShifttime"] }, new double[] { InLastShifttime });

            var TCLocked = DataBus.TCLocked ? 1 : 0;
            SetInput(new uint[] { InputDict["TCLocked"] }, new double[] { TCLocked });
            var StartSpeed = DataBus.StartSpeed.Value();
            SetInput(new uint[] { InputDict["StartSpeed"] }, new double[] { StartSpeed });
            var StartAcceleration = DataBus.StartAcceleration.Value();
            SetInput(new uint[] { InputDict["StartAcceleration"] }, new double[] { StartAcceleration });
            var LastShift = (DataBus.LastShift.Value() == -double.MaxValue.SI().Value()) ? MinValue : DataBus.LastShift.Value();
            SetInput(new uint[] { InputDict["LastShift"] }, new double[] { LastShift });
            var TractionInterruption = DataBus.TractionInterruption.Value();
            SetInput(new uint[] { InputDict["TractionInterruption"] }, new double[] { TractionInterruption });
            var NumGears = DataBus.NumGears;
            SetInput(new uint[] { InputDict["NumGears"] }, new double[] { NumGears });
            var AxlegearLoss = DataBus.AxlegearLoss().Value();
            SetInput(new uint[] { InputDict["AxlegearLoss"] }, new double[] { AxlegearLoss });
            var EngineSpeed = (DataBus.EngineSpeed != null) ? DataBus.EngineSpeed.AsRPM : 0;
            SetInput(new uint[] { InputDict["EngineSpeed"] }, new double[] { EngineSpeed });
            var EngineTorque = (DataBus.EngineTorque != null) ? DataBus.EngineTorque.Value() : 0;
            SetInput(new uint[] { InputDict["EngineTorque"] }, new double[] { EngineTorque });
            var EngineIdleSpeed = (DataBus.EngineIdleSpeed != null) ? DataBus.EngineIdleSpeed.AsRPM : 0;
            SetInput(new uint[] { InputDict["EngineIdleSpeed"] }, new double[] { EngineIdleSpeed });
            var EngineRatedSpeed = (DataBus.EngineRatedSpeed != null) ? DataBus.EngineRatedSpeed.AsRPM : 0;
            SetInput(new uint[] { InputDict["EngineRatedSpeed"] }, new double[] { EngineRatedSpeed });
            var EngineN95hSpeed = (DataBus.EngineN95hSpeed != null) ? DataBus.EngineN95hSpeed.AsRPM : 0;
            SetInput(new uint[] { InputDict["EngineN95hSpeed"] }, new double[] { EngineN95hSpeed });
            var EngineN80hSpeed = (DataBus.EngineN80hSpeed != null) ? DataBus.EngineN80hSpeed.AsRPM : 0;
            SetInput(new uint[] { InputDict["EngineN80hSpeed"] }, new double[] { EngineN80hSpeed });
            var VehicleSpeed = (DataBus.VehicleSpeed != null) ? DataBus.VehicleSpeed.Value() : 0;
            SetInput(new uint[] { InputDict["VehicleSpeed"] }, new double[] { VehicleSpeed });
            var VehicleStopped = DataBus.VehicleStopped ? 1 : 0;
            SetInput(new uint[] { InputDict["VehicleStopped"] }, new double[] { VehicleStopped });
            var VehicleMass = (DataBus.VehicleMass != null) ? DataBus.VehicleMass.Value() : 0;
            SetInput(new uint[] { InputDict["VehicleMass"] }, new double[] { VehicleMass });
            var VehicleLoading = (DataBus.VehicleLoading != null) ? DataBus.VehicleLoading.Value() : 0;
            SetInput(new uint[] { InputDict["VehicleLoading"] }, new double[] { VehicleLoading });
            var TotalMass = (DataBus.TotalMass != null) ? DataBus.TotalMass.Value() : 0;
            SetInput(new uint[] { InputDict["TotalMass"] }, new double[] { TotalMass });
            var CargoVolume = (DataBus.CargoVolume != null) ? DataBus.CargoVolume.Value() : 0;
            SetInput(new uint[] { InputDict["CargoVolume"] }, new double[] { CargoVolume });
            var MaxVehicleSpeed = (DataBus.MaxVehicleSpeed != null) ? DataBus.MaxVehicleSpeed.Value() : 0;
            SetInput(new uint[] { InputDict["MaxVehicleSpeed"] }, new double[] { MaxVehicleSpeed });
            var Distance = (DataBus.Distance != null) ? DataBus.Distance.Value() : 0;
            SetInput(new uint[] { InputDict["Distance"] }, new double[] { Distance });
            var BrakePower = (DataBus.BrakePower != null) ? DataBus.BrakePower.Value() : 0;
            SetInput(new uint[] { InputDict["BrakePower"] }, new double[] { BrakePower });
            var ReducedMassWheels = (DataBus.ReducedMassWheels != null) ? DataBus.ReducedMassWheels.Value() : 0;
            SetInput(new uint[] { InputDict["ReducedMassWheels"] }, new double[] { ReducedMassWheels });
            var DrivingBehavior = (int)DataBus.DriverBehavior;
            SetInput(new uint[] { InputDict["DrivingBehavior"] }, new double[] { DrivingBehavior });
            var DrivingAction = (int)DataBus.DrivingAction;
            SetInput(new uint[] { InputDict["DrivingAction"] }, new double[] { DrivingAction });
            var DriverAcceleration = DataBus.DriverAcceleration.Value();
            SetInput(new uint[] { InputDict["DriverAcceleration"] }, new double[] { DriverAcceleration });
            var PTOActive = DataBus.PTOActive ? 1 : 0;
            SetInput(new uint[] { InputDict["PTOActive"] }, new double[] { PTOActive });
            var Altitude = (DataBus.Altitude != null) ? DataBus.Altitude.Value() : 0;
            SetInput(new uint[] { InputDict["Altitude"] }, new double[] { Altitude });
            var RoadGradient = (DataBus.RoadGradient != null) ? DataBus.RoadGradient.Value() : 0;
            SetInput(new uint[] { InputDict["RoadGradient"] }, new double[] { RoadGradient });
            var CycleStartDistance = (DataBus.CycleStartDistance != null) ? DataBus.CycleStartDistance.Value() : 0;
            SetInput(new uint[] { InputDict["CycleStartDistance"] }, new double[] { CycleStartDistance });
            var InLastUpshift = (gearbox.LastUpshift.Value() == -double.MaxValue.SI().Value()) ? MinValue : gearbox.LastUpshift.Value();
            SetInput(new uint[] { InputDict["InLastUpshift"] }, new double[] { InLastUpshift });
            var InLastDownshift = (gearbox.LastDownshift.Value() == -double.MaxValue.SI().Value()) ? MinValue : gearbox.LastDownshift.Value();
            SetInput(new uint[] { InputDict["InLastDownshift"] }, new double[] { InLastDownshift });
            var GearboxPreviousStateInTorque = (gearbox.PreviousState.InTorque != null) ? gearbox.PreviousState.InTorque.Value() : 0;
            SetInput(new uint[] { InputDict["GearboxPreviousStateInTorque"] }, new double[] { GearboxPreviousStateInTorque });
            var GearboxPreviousStateInAngularVelocity = (gearbox.PreviousState.InAngularVelocity != null) ? gearbox.PreviousState.InAngularVelocity.AsRPM : 0;
            SetInput(new uint[] { InputDict["GearboxPreviousStateInAngularVelocity"] }, new double[] { GearboxPreviousStateInAngularVelocity });
            var GearboxPreviousStateOutTorque = (gearbox.PreviousState.OutTorque != null) ? gearbox.PreviousState.OutTorque.Value() : 0;
            SetInput(new uint[] { InputDict["GearboxPreviousStateOutTorque"] }, new double[] { GearboxPreviousStateOutTorque });
            var GearboxPreviousStateOutAngularVelocity = (gearbox.PreviousState.OutAngularVelocity != null) ? gearbox.PreviousState.OutAngularVelocity.AsRPM : 0;
            SetInput(new uint[] { InputDict["GearboxPreviousStateOutAngularVelocity"] }, new double[] { GearboxPreviousStateOutAngularVelocity });
            var EnginePreviousStateEnginePower = (DataBus.PreviousStateEnginePower != null) ? DataBus.PreviousStateEnginePower.Value() : 0;
            SetInput(new uint[] { InputDict["EnginePreviousStateEnginePower"] }, new double[] { EnginePreviousStateEnginePower });
            var EnginePreviousStateEngineSpeed = (DataBus.EngineSpeed != null) ? DataBus.EngineSpeed.AsRPM : 0;
            SetInput(new uint[] { InputDict["EnginePreviousStateEngineSpeed"] }, new double[] { EnginePreviousStateEngineSpeed });
            var EnginePreviousStateEngineTorque = (DataBus.EngineTorque != null) ? DataBus.EngineTorque.Value() : 0;
            SetInput(new uint[] { InputDict["EnginePreviousStateEngineTorque"] }, new double[] { EnginePreviousStateEngineTorque });
            var EnginePreviousStateEngineTorqueOut = (DataBus.PreviousStateEngineTorqueOut != null) ? DataBus.PreviousStateEngineTorqueOut.Value() : 0;
            SetInput(new uint[] { InputDict["EnginePreviousStateEngineTorqueOut"] }, new double[] { EnginePreviousStateEngineTorqueOut });
            var EnginePreviousStateFullDragTorque = (DataBus.PreviousStateFullDragTorque != null) ? DataBus.PreviousStateFullDragTorque.Value() : 0;
            SetInput(new uint[] { InputDict["EnginePreviousStateFullDragTorque"] }, new double[] { EnginePreviousStateFullDragTorque });
            var EnginePreviousStateInertiaTorqueLoss = (DataBus.PreviousStateInertiaTorqueLoss != null) ? DataBus.PreviousStateInertiaTorqueLoss.Value() : 0;
            SetInput(new uint[] { InputDict["EnginePreviousStateInertiaTorqueLoss"] }, new double[] { EnginePreviousStateInertiaTorqueLoss });
            var EnginePreviousStateOperationMode = (int)DataBus.PreviousStateOperationMode;
            SetInput(new uint[] { InputDict["EnginePreviousStateOperationMode"] }, new double[] { EnginePreviousStateOperationMode });
            var EnginePreviousStateStationaryFullLoadTorque = (DataBus.PreviousStateStationaryFullLoadTorque != null) ? DataBus.PreviousStateStationaryFullLoadTorque.Value() : 0;
            SetInput(new uint[] { InputDict["EnginePreviousStateStationaryFullLoadTorque"] }, new double[] { EnginePreviousStateStationaryFullLoadTorque });
            var EnginePreviousStatedt = (DataBus.PreviousStateDt != null) ? DataBus.PreviousStateDt.Value() : 0;
            SetInput(new uint[] { InputDict["EnginePreviousStatedt"] }, new double[] { EnginePreviousStatedt });
            //string jsonName = "VectoJsonModelData.json";

            //SetInput(new uint[] { InputDict["jsonName"] }, new double[] { 86, 101, 99, 116, 111, 74, 115, 111, 110, 77, 111, 100, 101, 108, 68, 97, 116, 97, 46, 106, 115, 111, 110 });
        }



        private List<Fmi2Status> setCurrentStepInput(int step, List<string> headerList, List<List<String>> datalist, Dictionary<string, uint> inputDict)
        {

            List<Fmi2Status> statusList = new List<Fmi2Status>();
            int counter = 0;
            foreach (string column in headerList)
            {
                statusList.Add(SetInput(new uint[] { inputDict[column] }, new double[] { double.Parse(datalist[counter].ToArray()[step]) }));
                counter++;
            }

            return statusList;
        }

        private Fmi2Status SetInput(uint[] valueReference, double[] data)
        {
            return Instance.SetReal(valueReference, data);
        }

        private Fmi2Status SetOutput(uint[] valueReference, double[] data)
        {
            return Instance.SetReal(valueReference, data);
        }


        private void Fmu_instance_Log(string instanceName, Fmi2Status status, string category, string message)
        {
            //Console.WriteLine(message);
        }

        private void Fmu_step_finished(Fmi2Status status)
        {
            //Console.WriteLine(status);
        }

        private void ReadInputOutputVariables(XmlDocument doc)
        {
            InputDict = new Dictionary<string, uint>();
            OutputDict = new Dictionary<string, uint>();
            var ModelVariables = doc.SelectSingleNode("/fmiModelDescription/ModelVariables");

            foreach (XmlNode node in ModelVariables.ChildNodes)
            {
                if (node.Attributes != null)
                {
                    if (node.Attributes["causality"].Value == "input")
                    {
                        InputDict.Add(node.Attributes["name"].Value, uint.Parse(node.Attributes["valueReference"].Value));
                    }
                    else if (node.Attributes["causality"].Value == "output")
                    {
                        OutputDict.Add(node.Attributes["name"].Value, uint.Parse(node.Attributes["valueReference"].Value));
                    }
                }
            }
        }


        private string LoadAndCopyFMUArchive(string path)
        {
            using (var stream = File.OpenRead(path))
            {
                var fmuArchive = new ZipArchive(stream, ZipArchiveMode.Read);
                var modelDescription = fmuArchive.GetEntry("modelDescription.xml");

                
                using(var doc = modelDescription.Open())
                {
                    var docXML = new XmlDocument();
                    docXML.Load(doc);

                    ModelDescriptionGuid = docXML.SelectSingleNode("/fmiModelDescription").Attributes["guid"].Value;
                    ModelIdentifier = docXML.SelectSingleNode("/fmiModelDescription/CoSimulation").Attributes["modelIdentifier"].InnerText;

                    ReadInputOutputVariables(docXML);
                    var fmuDLL = fmuArchive.GetEntry("binaries/win64/" + ModelIdentifier + ".dll");

                    string tempPath = Path.GetTempPath();

                    using (var dll = fmuDLL.Open())
                    {
                        
                        //int fileCounter = 1;
                        string copyToPath = Path.Combine(tempPath, ModelIdentifier + ".dll");
                        //while(File.Exists(copyToPath))
                        //{
                        //    copyToPath = Path.Combine(tempPath, ModelIdentifier + fileCounter + ".dll");
                        //    fileCounter++;
                        //}

                        using (var md5 = MD5.Create())
                        {
                            if(File.Exists(copyToPath))
                            {
                                using (var stream2 = File.OpenRead(copyToPath))
                                {
                                    byte[] hash = md5.ComputeHash(dll);
                                    byte[] hash2 = md5.ComputeHash(stream2);
                                    bool hashCompare = true;
                                    for (int i = 0; i < hash.Length; i++)
                                    {
                                        if (hash[i] != hash2[i])
                                            hashCompare = false;
                                    }
                                    if(!hashCompare)
                                    {
                                        stream2.Close();
                                        using (var target = File.Create(copyToPath))
                                        {
                                            dll.CopyTo(target);
                                        }
                                    } 
                                }
                            }
                            else
                            {
                                using (var target = File.Create(copyToPath))
                                {
                                    dll.CopyTo(target);
                                }
                            }
                            
                            return copyToPath;
                        }
                            //File.Copy(path, copyToPath);
                    }
                }
                
            }
        }
    }
}
