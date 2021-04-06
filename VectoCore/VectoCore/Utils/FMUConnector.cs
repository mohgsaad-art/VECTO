using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using FmiWrapper_Net;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.Simulation.DataBus;
using TUGraz.VectoCore.Models.SimulationComponent.Impl;
using TUGraz.VectoCore.Models.Connector.Ports.Impl;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.Models.Simulation.Data;

namespace TUGraz.VectoCore.Utils
{
    public class FMUConnector
    {
        static bool firstLine = false;

        static string header = "Time;ICE_MaxTorque_Nm;torqueDemandPwt_Nm;AuxCons_kW;abilEssDchPower_kW;abilEssRegenPower_kW;reqMotorBrakeTorque_Nm;SOC;abilMotorTorque_Nm;abilMotorPower_kW;motorSpeed_rpm;abilRegenPower_kW;abilRegenTorque_Nm;EM_speed_rpm;motorTorque_Nm;VehicleSpeed";

        const Double MinValue = -2.2251E+307;
        private string ModelDescriptionGuid { get; set; }
        private string ModelIdentifier { get; set; }
        private Dictionary<string, uint> InputDict { get; set; }
        private Dictionary<string, uint> OutputDict { get; set; }
        private FmuInstance Instance { get; set; }

        public uint InitGearNextGear = 0;
        private decimal Time { get; set; }
        public void InitFMU(string fmuPath)
        {
            Time = 0;
            string fmuDLL = LoadAndCopyFMUArchive(fmuPath);

            FmuInstance fmu_instance = new FmuInstance(fmuDLL);

            fmu_instance.Log += Fmu_instance_Log;
            fmu_instance.StepFinished += Fmu_step_finished;

            fmu_instance.Instantiate(ModelIdentifier,
                  Fmi2Type.fmi2CoSimulation,
                  ModelDescriptionGuid,
                  "",
                  false,
                  true);
            Fmi2Status experiment_status = fmu_instance.SetupExperiment(false, 0, 0, false, 10);
            //Console.WriteLine("Setup Experiment status: " + experiment_status);

            Fmi2Status initalization_mode_status = fmu_instance.EnterInitializationMode();
            //Console.WriteLine("Initalization mode status: " + initalization_mode_status);
            fmu_instance.ExitInitializationMode();

            Instance = fmu_instance;
        }



        public double Step(Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity, ResponseDryRun response, IDataBus DataBus, out double EMTorque, out double ICETorque)
        {
            //set Inputs
            StepSetInput(absTime, dt, outTorque, outAngularVelocity, response, DataBus);

            //doStep
            Fmi2Status step_status = Instance.DoStep((double)Time, 0.1, true);

            //getOutput
            uint[] getEMTorqueOutput = { OutputDict["EM_Torq"] };
            double[] getOutEM = new double[getEMTorqueOutput.Length];
            Instance.GetReal(getEMTorqueOutput, getOutEM);

            uint[] getICETorqueOutput = { OutputDict["ICE_Torq"] };
            double[] getOutICE = new double[getICETorqueOutput.Length];
            Instance.GetReal(getICETorqueOutput, getOutICE);

            EMTorque = getOutEM[0];
            ICETorque = getOutICE[0];
            return getOutEM[0];
        }

        private void StepSetInput(Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity, ResponseDryRun response, IDataBus DataBus)
        {

            /*var InAbsTime = absTime.Value();
            SetInput(new uint[] { InputDict["InAbsTime"] }, new double[] { InAbsTime });
            var InDt = dt.Value();
            SetInput(new uint[] { InputDict["InDt"] }, new double[] { InDt });
            var InOutTorque = outTorque.Value();
            SetInput(new uint[] { InputDict["InOutTorque"] }, new double[] { InOutTorque });

            var InOutAngularVelocity = outAngularVelocity.AsRPM;
            SetInput(new uint[] { InputDict["InOutAngularVelocity"] }, new double[] { InOutAngularVelocity });
            */
            
            var InertiaTorqueLoss = (DataBus.EngineInfo.EngineOn ? response.Engine.InertiaTorqueLoss.Value() : 0);

            var ICE_MaxTorque_Nm = (response.Engine.DynamicFullLoadTorque != null) ? (response.Engine.DynamicFullLoadTorque.Value() - InertiaTorqueLoss) : 0;
            //var ICE_MaxTorque_Nm = (response.Engine.DynamicFullLoadTorque != null) ? (response.Engine.DynamicFullLoadTorque).Value() : 0;
            SetInput(new uint[] { InputDict["ICE_MaxTorque_Nm"] }, new double[] { ICE_MaxTorque_Nm });

            var torqueDemandPwt_Nm = (response.Gearbox.InTorque != null) ? response.Gearbox.InTorque.Value() : 0;
            SetInput(new uint[] { InputDict["torqueDemandPwt_Nm"] }, new double[] { torqueDemandPwt_Nm });

            var AuxCons_kW = (response.Engine.AuxiliariesPowerDemand != null) ? response.Engine.AuxiliariesPowerDemand.Value() : 0;
            SetInput(new uint[] { InputDict["AuxCons_kW"] }, new double[] { AuxCons_kW });

            var abilEssDchPower_kW = (response.ElectricSystem != null && response.ElectricSystem.RESSResponse.MaxDischargePower != null) ? response.ElectricSystem.RESSResponse.MaxDischargePower.Value() / 1000 : 0;
            SetInput(new uint[] { InputDict["abilEssDchPower_kW"] }, new double[] { abilEssDchPower_kW });

            var abilEssRegenPower_kW = (response.ElectricSystem != null && response.ElectricSystem.RESSResponse.MaxChargePower != null) ? response.ElectricSystem.RESSResponse.MaxChargePower.Value() / 1000 : 0;
            SetInput(new uint[] { InputDict["abilEssRegenPower_kW"] }, new double[] { abilEssRegenPower_kW });

            var reqMotorBrakeTorque_Nm = (response.Brakes.BrakeTorque != null) ? response.Brakes.BrakeTorque.Value() : 0;
            SetInput(new uint[] { InputDict["reqMotorBrakeTorque_Nm"] }, new double[] { reqMotorBrakeTorque_Nm });

            var SOC = (response.ElectricSystem != null && response.ElectricSystem.RESSResponse != null) ? response.ElectricSystem.RESSResponse.StateOfCharge * 100 : 0 ;
            SetInput(new uint[] { InputDict["SOC"] }, new double[] { SOC });

            var abilMotorTorque_Nm = (response.ElectricMotor.MaxDriveTorque != null) ? response.ElectricMotor.MaxDriveTorque.Value() : 0;
            SetInput(new uint[] { InputDict["abilMotorTorque_Nm"] }, new double[] { abilMotorTorque_Nm });

            var abilMotorPower_kW = ((((response.ElectricMotor.MaxDriveTorque ?? 0.SI<NewtonMeter>())) * response.ElectricMotor.AngularVelocity).Value()) / 1000;
            SetInput(new uint[] { InputDict["abilMotorPower_kW"] }, new double[] { abilMotorPower_kW });

            var motorSpeed_rpm = (response.Engine.EngineSpeed != null) ? response.Engine.EngineSpeed.Value() : 0;
            SetInput(new uint[] { InputDict["motorSpeed_rpm"] }, new double[] { motorSpeed_rpm });

            var abilRegenPower_kW = ((((response.ElectricMotor.MaxRecuperationTorqueEm ?? 0.SI<NewtonMeter>())) * response.ElectricMotor.AngularVelocity).Value()) / 1000;
            SetInput(new uint[] { InputDict["abilRegenPower_kW"] }, new double[] { abilRegenPower_kW });

            var abilRegenTorque_Nm = (response.ElectricMotor.MaxRecuperationTorqueEmMap != null) ? response.ElectricMotor.MaxRecuperationTorqueEm.Value() : 0;
            SetInput(new uint[] { InputDict["abilRegenTorque_Nm"] }, new double[] { abilRegenTorque_Nm });

            var EM_speed_rpm = (response.ElectricMotor.AngularVelocity != null) ? response.ElectricMotor.EmSpeed.AsRPM : 0;
            SetInput(new uint[] { InputDict["EM_speed_rpm"] }, new double[] { EM_speed_rpm });

            var motorTorque_Nm = ((DataBus.ElectricMotorInfo(VectoCommon.InputData.PowertrainPosition.HybridP2) as ElectricMotor).CurrentState.EmTorqueMap != null) ? (DataBus.ElectricMotorInfo(VectoCommon.InputData.PowertrainPosition.HybridP2) as ElectricMotor).CurrentState.EmTorqueMap.Value() : 0;
            SetInput(new uint[] { InputDict["motorTorque_Nm"] }, new double[] { motorTorque_Nm });

            var VehicleSpeed_kmh = (DataBus.VehicleInfo.VehicleSpeed != null) ? DataBus.VehicleInfo.VehicleSpeed.AsKmph : 0;
            SetInput(new uint[] { InputDict["VehicleSpeed"] }, new double[] { VehicleSpeed_kmh });

            LogShiftDataToExcel(absTime, dt, outTorque, outAngularVelocity, response, ICE_MaxTorque_Nm, torqueDemandPwt_Nm, AuxCons_kW, abilEssDchPower_kW, abilEssRegenPower_kW, reqMotorBrakeTorque_Nm,
                    SOC, abilMotorTorque_Nm, abilMotorPower_kW, motorSpeed_rpm, abilRegenPower_kW, abilRegenTorque_Nm, EM_speed_rpm, motorTorque_Nm, VehicleSpeed_kmh);
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


                using (var doc = modelDescription.Open())
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
                        int fileCounter = 1;
                        string copyToPath = Path.Combine(tempPath, ModelIdentifier + ".dll");
                        while (File.Exists(copyToPath))
                        {
                            copyToPath = Path.Combine(tempPath, ModelIdentifier + fileCounter + ".dll");
                            fileCounter++;
                        }
                        using (var target = File.Create(copyToPath))
                        {
                            dll.CopyTo(target);
                            return copyToPath;
                        }
                    }
                }

            }
        }

        public static void LogShiftDataToExcel(Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity, ResponseDryRun response, double iCE_MaxTorque_Nm, 
            double torqueDemandPwt_Nm, double auxCons_kW, double abilEssDchPower_kW, double abilEssRegenPower_kW, double reqMotorBrakeTorque_Nm, double sOC, 
            double abilMotorTorque_Nm, double abilMotorPower_kW, double motorSpeed_rpm, double abilRegenPower_kW, double abilRegenTorque_Nm, double eM_speed_rpm, 
            double motorTorque_Nm, double vehicleSpeed_kmh)
        {
            using (var w = new StreamWriter(@"C:\Users\Work\Workspace\Data\Test_Volvo.csv", true, Encoding.UTF8))
            {
                if (!firstLine)
                {
                    w.WriteLine(header);
                    firstLine = true;
                }
                //var function = function_;

                var Time = absTime.Value();

                //var EngageTime = DataBus.EngageTime?.Value();

                var line = string.Format("{0}; {1}; {2}; {3}; {4}; {5}; {6}; {7}; {8}; {9}; {10}; {11}; {12}; {13}; {14}; {15}",
                    Time, iCE_MaxTorque_Nm, torqueDemandPwt_Nm, auxCons_kW, abilEssDchPower_kW, abilEssRegenPower_kW, reqMotorBrakeTorque_Nm,
                    sOC, abilMotorTorque_Nm, abilMotorPower_kW, motorSpeed_rpm, abilRegenPower_kW, abilRegenTorque_Nm, eM_speed_rpm, motorTorque_Nm, vehicleSpeed_kmh);
                w.WriteLine(line);
                w.Flush();
            }
        }
    }
}
