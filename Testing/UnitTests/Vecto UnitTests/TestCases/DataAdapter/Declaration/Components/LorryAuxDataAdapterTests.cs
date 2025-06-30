using Moq;
using NUnit.Framework;
using TUGraz.Vecto.UnitTests.Utils;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.InputData.Reader.DataObjectAdapter.SimulationComponents.AuxiliaryDataAdapter;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation.Data;
using Assert = NUnit.Framework.Assert;

namespace TUGraz.Vecto.UnitTests.TestCases.DataAdapter.Declaration.Components;

public class LorryAuxDataAdapterTests
{
    [TestCase("Small", false, false, TestName = "S_HEV PneumaticSystemTest Pass 1")]
    [TestCase("Small + AMS + elec. driven", true, false, TestName = "S_HEV PneumaticSystemTest Pass 2")]
    [TestCase("Small", false, false, TestName = "S_HEV PneumaticSystemTest Pass 3")]
    public void SHEVPneumaticSystemTest(string psTechnology, bool fullyElectric, bool fail)
    {
        var dataAdapter = new HeavyLorryAuxiliaryDataAdapter();

        var auxInput = CreateAuxInput(psTechnology, "Default", "Standard technology", "Crankshaft mounted - Electronically controlled visco clutch", "Electric driven pump");

        var auxData = dataAdapter.CreateAuxiliaryData(auxInput.Object, null, MissionType.LongHaul, VehicleClass.Class2,
            8.SI<Meter>(), 1, VectoSimulationJobType.SerialHybridVehicle);

        var ps = auxData.Single(data => data.ID == Constants.Auxiliaries.IDs.PneumaticSystem);

        Assert.That(ps.ConnectToREESS == fullyElectric);
        Assert.That(ps.IsFullyElectric == fullyElectric);
    }

    [TestCase("Small + AMS", "", true, 0.0f, true, TestName = "PEV PneumaticSystemTest Fail 2")]
    [TestCase("Small + ESS", "", true, 0.0f, true, TestName = "PEV PneumaticSystemTest Fail 3")]
    [TestCase("Small + AMS + elec. driven", "Electrically driven - Electronically controlled", true, 0.0f, true, TestName = "PEV PneumaticSystemTest Fail 1")]
    [TestCase("Small + AMS + elec. driven", "", true, 675, false, TestName = "PEV PneumaticSystemTest Pass 1")]
    public void PEVPneumaticSystemTest(string psTechnology, string fanTech, bool fullyElectric, double expectedElectric,
        bool fail)
    {
        var dataAdapter = new HeavyLorryPEVAuxiliaryDataAdapter();

        var auxInput = CreateAuxInput(psTechnology, "Default", "Standard technology", fanTech, "Electric driven pump");
        var auxData = GetAuxData(fail, dataAdapter, auxInput, VectoSimulationJobType.BatteryElectricVehicle, 1, 8.SI<Meter>(), VehicleClass.Class2, MissionType.LongHaul);

        var ps = auxData.Single(data => data.ID == Constants.Auxiliaries.IDs.PneumaticSystem);

        Assert.That(ps.ConnectToREESS == true);
        Assert.That(ps.IsFullyElectric == fullyElectric);
        Assert.That(ps.PowerDemandDataBusFunc == null);
        Assert.That(ps.PowerDemandMechCycleFunc == null);
        Assert.That(ps.PowerDemandElectric.IsEqual(expectedElectric));
    }

    [TestCase("Small", false, 1400, false, TestName = "Conventional PneumaticSystemTest Pass 1")]
    [TestCase("Small + AMS + elec. driven", true, 357, false, TestName = "Conventional PneumaticSystemTest Pass 2")]
    [TestCase("Small + AMS + elec. xyz", true, 357, true, TestName = "Conventional PneumaticSystemTest Fail 1")]
    public void ConventionalPneumaticSystemTest(string psTechnology, bool fullyElectric, double expectedMechanic, bool fail)
    {
        var dataAdapter = new HeavyLorryAuxiliaryDataAdapter();

        var auxInput = CreateAuxInput(psTechnology, "Default", "Standard technology", "Crankshaft mounted - Electronically controlled visco clutch", "Electric driven pump");
        var auxData = GetAuxData(fail, dataAdapter, auxInput, VectoSimulationJobType.ConventionalVehicle, 1, 8.SI<Meter>(), VehicleClass.Class2, MissionType.LongHaul);

        var ps = auxData.Single(data => data.ID == Constants.Auxiliaries.IDs.PneumaticSystem);

        Assert.That(ps.ConnectToREESS == false);
        Assert.That(ps.IsFullyElectric == fullyElectric);
        Assert.That(ps.PowerDemandDataBusFunc == null);
        Assert.That(ps.PowerDemandMechCycleFunc == null);
        Assert.That(ps.PowerDemandMech.IsEqual(expectedMechanic), "wrong value");
    }

    [TestCase("Electrically driven - Electronically controlled", true, false, TestName = "HEV CoolingFan Pass 1")]
    [TestCase("Crankshaft mounted - On/off clutch", false, false, TestName = "HEV CoolingFan Pass 2")]
    public void HEVCoolingFanTest(string fanTech, bool isFullyElectric, bool fail)
    {
        var dataAdapter = new HeavyLorryAuxiliaryDataAdapter();
        var auxInputData = CreateAuxInput("Small + ESS", "Default", "Standard technology",
            fanTech, "Electric driven pump");
        var auxData = GetAuxData(fail, dataAdapter, auxInputData, VectoSimulationJobType.SerialHybridVehicle, 1,
            8.SI<Meter>(), VehicleClass.Class2, MissionType.LongHaul);

        var fan = auxData.Single(data => data.ID == Constants.Auxiliaries.IDs.Fan);

        Assert.That(isFullyElectric == fan.IsFullyElectric);
        Assert.That(isFullyElectric == fan.ConnectToREESS);
        Assert.That(fan.PowerDemandElectric == fan.PowerDemandMech * DeclarationData.AlternatorEfficiency);
        Assert.That(fan.PowerDemandDataBusFunc == null);
        Assert.That(fan.PowerDemandMechCycleFunc == null);
    }

    [TestCase("Electrically driven - Electronically controlled", true, false, TestName = "Conv CoolingFan Pass 1")]
    [TestCase("Crankshaft mounted - On/off clutch", false, false, TestName = "Conv CoolingFan Pass 2")]
    public void ConventionalCoolingFanTest(string fanTech, bool isFullyElectric, bool fail)
    {
        var dataAdapter = new HeavyLorryAuxiliaryDataAdapter();
        var auxInputData = CreateAuxInput("Small + ESS", "Default", "Standard technology",
            fanTech, "Electric driven pump");
        var auxData = GetAuxData(fail, dataAdapter, auxInputData, VectoSimulationJobType.ConventionalVehicle, 1,
            8.SI<Meter>(), VehicleClass.Class2, MissionType.LongHaul);

        var fan = auxData.Single(data => data.ID == Constants.Auxiliaries.IDs.Fan);

        Assert.That(isFullyElectric == fan.IsFullyElectric);
        Assert.IsFalse(fan.ConnectToREESS);
        Assert.That(fan.PowerDemandElectric == fan.PowerDemandMech * DeclarationData.AlternatorEfficiency);
        Assert.IsNull(fan.PowerDemandDataBusFunc);
        Assert.IsNull(fan.PowerDemandMechCycleFunc);
    }

    [TestCase("Standard technology", false)]
    [TestCase("asdf", true)]
    public void ConventionalESTest(string esTechnology, bool fail)
    {
        var dataAdapter = new HeavyLorryAuxiliaryDataAdapter();
        var auxInput = CreateAuxInput("Small", "Default", esTechnology, "Crankshaft mounted - On/off clutch",
            "Fixed displacement");
        var auxData = GetAuxData(fail, dataAdapter, auxInput, VectoSimulationJobType.ConventionalVehicle, 1,
            4.SI<Meter>(), VehicleClass.Class2, MissionType.LongHaul);
        var es = auxData.Single(data => data.ID == Constants.Auxiliaries.IDs.ElectricSystem);
        Assert.IsFalse(es.ConnectToREESS);
    }

    [TestCase("Standard technology", false)]
    [TestCase("asdf", true)]
    public void PHEVElectricSystemTest(string esTechnology, bool fail)
    {
        var dataAdapter = new HeavyLorryAuxiliaryDataAdapter();
        var auxInput = CreateAuxInput("Small", "Default", esTechnology, "Crankshaft mounted - On/off clutch",
            "Fixed displacement");
        var auxData = GetAuxData(fail, dataAdapter, auxInput, VectoSimulationJobType.ParallelHybridVehicle, 1,
            4.SI<Meter>(), VehicleClass.Class2, MissionType.LongHaul);
        var es = auxData.Single(data => data.ID == Constants.Auxiliaries.IDs.ElectricSystem);
        Assert.IsFalse(es.ConnectToREESS);
    }

    [TestCase("Standard technology", false)]
    [TestCase("asdf", true)]
    public void SHEVElectricSystemTest(string esTechnology, bool fail)
    {
        var dataAdapter = new HeavyLorryAuxiliaryDataAdapter();
        var auxInput = CreateAuxInput("Small", "Default", esTechnology, "Crankshaft mounted - On/off clutch",
            "Electric driven pump");
        var auxData = GetAuxData(fail, dataAdapter, auxInput, VectoSimulationJobType.SerialHybridVehicle, 1,
            4.SI<Meter>(), VehicleClass.Class2, MissionType.LongHaul);
        var es = auxData.Single(data => data.ID == Constants.Auxiliaries.IDs.ElectricSystem);
        Assert.IsTrue(es.ConnectToREESS);
    }

    [TestCase("Standard technology", false)]
    [TestCase("asdf", true)]
    public void PEVElectricSystemTest(string esTechnology, bool fail)
    {
        var dataAdapter = new HeavyLorryPEVAuxiliaryDataAdapter();
        var auxInput = CreateAuxInput("Small + elec. driven", "Default", esTechnology, "",
            "Electric driven pump");
        var auxData = GetAuxData(fail, dataAdapter, auxInput, VectoSimulationJobType.BatteryElectricVehicle, 1,
            4.SI<Meter>(), VehicleClass.Class2, MissionType.LongHaul);
        var es = auxData.Single(data => data.ID == Constants.Auxiliaries.IDs.ElectricSystem);
        Assert.IsTrue(es.ConnectToREESS);
    }

    [Test]
    public void ConventionalHVACTest([Values(VectoSimulationJobType.ConventionalVehicle)] VectoSimulationJobType jobType)
    {
        var dataAdapter = new HeavyLorryAuxiliaryDataAdapter();
        var auxInput = CreateAuxInput("Small + elec. driven", "Default", "Standard technology", "Electrically driven - Electronically controlled", "Electric driven pump");
        var auxData = GetAuxData(false, dataAdapter, auxInput, VectoSimulationJobType.ConventionalVehicle, 1, 4.SI<Meter>(), VehicleClass.Class2, MissionType.LongHaul);

        var es = auxData.Single(data => data.ID == Constants.Auxiliaries.IDs.HeatingVentilationAirCondition);
        Assert.IsFalse(es.ConnectToREESS, "Connected to REESS");
    }

    [Test]
    public void PEVHVACTest([Values(VectoSimulationJobType.BatteryElectricVehicle, VectoSimulationJobType.IEPC_E)] VectoSimulationJobType jobType)
    {
        var dataAdapter = new HeavyLorryPEVAuxiliaryDataAdapter();
        var auxInput = CreateAuxInput("Small + elec. driven", "Default", "Standard technology", "", "Electric driven pump");
        var auxData = GetAuxData(false, dataAdapter, auxInput, VectoSimulationJobType.BatteryElectricVehicle, 1, 4.SI<Meter>(), VehicleClass.Class2, MissionType.LongHaul);

        var es = auxData.Single(data => data.ID == Constants.Auxiliaries.IDs.HeatingVentilationAirCondition);
        Assert.IsTrue(es.ConnectToREESS);
    }

    [Test]
    public void HEVHVACTest([Values(VectoSimulationJobType.SerialHybridVehicle, VectoSimulationJobType.ParallelHybridVehicle, VectoSimulationJobType.IEPC_S, VectoSimulationJobType.IHPC)] VectoSimulationJobType jobType)
    {
        var dataAdapter = new HeavyLorryAuxiliaryDataAdapter();
        var auxInput = CreateAuxInput("Small + elec. driven", "Default", "Standard technology", "Electrically driven - Electronically controlled", "Electric driven pump");
        var auxData = GetAuxData(false, dataAdapter, auxInput, VectoSimulationJobType.SerialHybridVehicle, 1, 4.SI<Meter>(), VehicleClass.Class2, MissionType.LongHaul);

        var es = auxData.Single(data => data.ID == Constants.Auxiliaries.IDs.HeatingVentilationAirCondition);
        Assert.IsTrue(es.ConnectToREESS);
    }

    [TestCase(2, false, "Fixed displacement", "Electric driven pump", TestName = "Conventional SteeringPump PASS mixed technologies")]
    [TestCase(2, false, "Fixed displacement", "Fixed displacement", TestName = "Conventional SteeringPump PASS")]
    [TestCase(1, true, "Fixed displacement", "Electric driven pump", TestName = "Conventional SteeringPump Fail")]
    public void ConventionalSteeringPumpTest(int steeredAxles, bool fail, string sp1 = null, string sp2 = null, string sp3 = null, string sp4 = null)
    {
        var dataAdapter = new HeavyLorryAuxiliaryDataAdapter();
        var auxInput = CreateAuxInput("Small + elec. driven", "Default", "Standard technology", "Electrically driven - Electronically controlled", new[] {
            sp1,
            sp2,
            sp3,
            sp4
        });

        var auxData = GetAuxData(fail, dataAdapter, auxInput, VectoSimulationJobType.ConventionalVehicle, steeredAxles, 4.SI<Meter>(), VehicleClass.Class2, MissionType.LongHaul);

        var sp = auxData.Single(data => data.ID == Constants.Auxiliaries.IDs.SteeringPump);
        Assert.IsFalse(auxData.Any(data => data.ID == Constants.Auxiliaries.IDs.SteeringPump_el && data.ConnectToREESS == true), "conventional has el. steering pump");


        Assert.IsFalse(sp.ConnectToREESS, "Connected to REESS");
    }

    [TestCase(2, false, "Electric driven pump", "Full electric steering gear", TestName = "SHEV SteeringPump Pass")]
    [TestCase(2, true, "Fixed displacement", "Full electric steering gear", TestName = "SHEV SteeringPump Fail_mixed")]
    public void SHEVSteeringPumpTest(int steeredAxles, bool fail, string sp1 = null, string sp2 = null, string sp3 = null, string sp4 = null)
    {
        var dataAdapter = new HeavyLorryAuxiliaryDataAdapter();
        var auxInput = CreateAuxInput("Small + elec. driven", "Default", "Standard technology", "Electrically driven - Electronically controlled", new[] {
            sp1,
            sp2,
            sp3,
            sp4
        });

        var auxData = GetAuxData(fail, dataAdapter, auxInput, VectoSimulationJobType.SerialHybridVehicle, steeredAxles, 4.SI<Meter>(), VehicleClass.Class2, MissionType.LongHaul);

        var sp = auxData.Single(data => data.ID == Constants.Auxiliaries.IDs.SteeringPump);
        Assert.IsFalse(auxData.Any(data => data.ID is Constants.Auxiliaries.IDs.SteeringPump_el or Constants.Auxiliaries.IDs.SteeringPump && data.ConnectToREESS == false), "STP not connected to REESS");


        Assert.IsTrue(sp.ConnectToREESS, "not connected to REESS");
    }

    [TestCase(2, true, false, false, "Electric driven pump", "Full electric steering gear", TestName = "PHEV SteeringPump Electric")]
    [TestCase(2, true, true, false, "Fixed displacement", "Full electric steering gear", TestName = "PHEV SteeringPump Mixed")]
    [TestCase(2, false, true, false, "Fixed displacement", "Dual displacement", TestName = "PHEV SteeringPump Mechanical")]
    public void PHEVSteeringPumpTest(int steeredAxles, bool hasElectric, bool hasMechanic, bool fail, string sp1 = null, string sp2 = null, string sp3 = null, string sp4 = null)
    {
        var dataAdapter = new HeavyLorryAuxiliaryDataAdapter();
        var auxInput = CreateAuxInput("Small + elec. driven", "Default", "Standard technology", "Electrically driven - Electronically controlled", new[] {
            sp1,
            sp2,
            sp3,
            sp4
        });

        var auxData = GetAuxData(fail, dataAdapter, auxInput, VectoSimulationJobType.ParallelHybridVehicle, steeredAxles, 4.SI<Meter>(), VehicleClass.Class2, MissionType.LongHaul);

        VectoRunData.AuxData sp = null;
        if (hasElectric && !hasMechanic)
        {
            sp = auxData.Single(data =>
                data.ID is Constants.Auxiliaries.IDs.SteeringPump or Constants.Auxiliaries.IDs.SteeringPump_el);

            Assert.IsTrue(sp.ConnectToREESS);
            Assert.Pass();
        }

        if (hasElectric && hasMechanic)
        {
            var spS = auxData.Where(data => data.ID is Constants.Auxiliaries.IDs.SteeringPump);
            Assert.AreEqual(2, spS.Count());

            Assert.IsTrue(spS.All(sp => sp.ConnectToREESS == sp.IsFullyElectric));
            Assert.Pass();
        }

        if (!hasElectric && hasMechanic)
        {
            sp = auxData.Single(data =>
                data.ID is Constants.Auxiliaries.IDs.SteeringPump);

            Assert.IsFalse(sp.ConnectToREESS);
            Assert.Pass();
        }

        Assert.IsFalse(sp.ConnectToREESS, "not connected to REESS");
    }

    [TestCase(2, false, "Electric driven pump", "Full electric steering gear", TestName = "PEV SteeringPump Pass")]
    [TestCase(2, true, "Fixed displacement", "Full electric steering gear", TestName = "PEV SteeringPump Fail_mixed")]
    public void PEVSteeringPumpTest(int steeredAxles, bool fail, string sp1 = null, string sp2 = null, string sp3 = null, string sp4 = null)
    {
        var dataAdapter = new HeavyLorryPEVAuxiliaryDataAdapter();
        var auxInput = CreateAuxInput("Small + elec. driven", "Default", "Standard technology", "", new[] {
            sp1,
            sp2,
            sp3,
            sp4
        });

        var auxData = GetAuxData(fail, dataAdapter, auxInput, VectoSimulationJobType.BatteryElectricVehicle, steeredAxles, 4.SI<Meter>(), VehicleClass.Class2, MissionType.LongHaul);

        var es = auxData.Single(data => data.ID == Constants.Auxiliaries.IDs.SteeringPump);
        Assert.IsTrue(es.ConnectToREESS, "not connected to REESS");
    }


    [TestCase(VectoSimulationJobType.BatteryElectricVehicle,
            new[] { "Full electric steering gear" },
            "Vacuum pump + elec. driven",
            true,
            2,
            TestName = "PEVAuxFailSteeredAxle")]
    [TestCase(VectoSimulationJobType.BatteryElectricVehicle,
            new[] { "Full electric steering gear" },
            "Vacuum pump + elec. driven",
            false,
            TestName = "PEVAuxPass")]
    [TestCase(VectoSimulationJobType.BatteryElectricVehicle,
            new[] { "Full electric steering gear" },
            "Medium Supply 1-stage + mech. clutch + AMS",
            true,
            TestName = "PEVAuxFailPS")]
    [TestCase(VectoSimulationJobType.SerialHybridVehicle,
            new[] { "Full electric steering gear" },
            "Vacuum pump + elec. driven",
            false,
            TestName = "S-HEV Aux Pass")]
    [TestCase(VectoSimulationJobType.ParallelHybridVehicle,
            new[] { "Full electric steering gear" },
            "Vacuum pump + elec. driven",
            false,
            TestName = "PHEVAuxPass")]
    public void HeavyLorryPEVAuxiliaryDataAdapterFailTest(VectoSimulationJobType jobType, string[] spTechnologies,
            string psTechnology, bool fail, int? steeredAxles = null)
    {
        var dataAdapter = new HeavyLorryPEVAuxiliaryDataAdapter();

        var auxData = new Mock<IAuxiliariesDeclarationInputData>();

        Mock<IAuxiliaryDeclarationInputData> steeringSystem = new Mock<IAuxiliaryDeclarationInputData>();
        foreach (var spTechnoly in spTechnologies)
        {
            steeringSystem.SetType(AuxiliaryType.SteeringPump)
                .AddTechnology(spTechnoly);
        }

        var hvac = new Mock<IAuxiliaryDeclarationInputData>()
            .SetType(AuxiliaryType.HVAC)
            .AddTechnology("Default");
        var pneumatic = new Mock<IAuxiliaryDeclarationInputData>()
            .SetType(AuxiliaryType.PneumaticSystem)
            .AddTechnology(psTechnology);
        var elSystem = new Mock<IAuxiliaryDeclarationInputData>()
            .SetType(AuxiliaryType.ElectricSystem)
            .AddTechnology("Standard technology");




        auxData.AddAuxiliaries(steeringSystem.Object, hvac.Object, pneumatic.Object, elSystem.Object);
        try
        {
            dataAdapter.CreateAuxiliaryData(auxData.Object, null, MissionType.LongHaul, VehicleClass.Class12,
                4.SI<Meter>(), steeredAxles ?? 1, VectoSimulationJobType.BatteryElectricVehicle);
        }
        catch (Exception ex)
        {
            if (fail)
            {
                Assert.Pass($"Expected Exception {ex.Message}");
            }
            else
            {
                throw new Exception("Exception occured", ex);
            }
        }
    }


    private static IList<VectoRunData.AuxData> GetAuxData(bool fail, HeavyLorryAuxiliaryDataAdapter dataAdapter, Mock<IAuxiliariesDeclarationInputData> auxInput, VectoSimulationJobType vectoSimulationJobType, int numSteeredAxles, Meter vehicleLength, VehicleClass vehicleClass, MissionType missionType)
    {
        IList<VectoRunData.AuxData> auxData = null;
        try
        {
            auxData = dataAdapter.CreateAuxiliaryData(auxInput.Object, null, missionType, vehicleClass,
                vehicleLength, numSteeredAxles, vectoSimulationJobType);
        }
        catch (VectoException ex)
        {
            if (fail)
            {
                Assert.Pass(ex.Message);
            }
            else
            {
                Assert.Fail(ex.Message, "Unexpected exception");
            }
        }
        Assert.IsFalse(fail, "Expected fail");
        return auxData;
    }


    private static Mock<IAuxiliariesDeclarationInputData> CreateAuxInput(string psTechnology, string hvacTechnology, string elSystemTechnology, string fanTech, params string[] spTechnologies)
    {
        Assert.IsTrue(spTechnologies.Length >= 1);
        var auxInput = new Mock<IAuxiliariesDeclarationInputData>();

        Mock<IAuxiliaryDeclarationInputData> steeringSystem = new Mock<IAuxiliaryDeclarationInputData>();
        foreach (var spTechnoly in spTechnologies)
        {
            if (spTechnoly.IsNullOrEmpty())
            {
                continue;

            }
            steeringSystem.SetType(AuxiliaryType.SteeringPump)
                .AddTechnology(spTechnoly);
        }

        var hvac = new Mock<IAuxiliaryDeclarationInputData>()
            .SetType(AuxiliaryType.HVAC)
            .AddTechnology(hvacTechnology);
        var pneumatic = new Mock<IAuxiliaryDeclarationInputData>()
            .SetType(AuxiliaryType.PneumaticSystem)
            .AddTechnology(psTechnology);
        var elSystem = new Mock<IAuxiliaryDeclarationInputData>()
            .SetType(AuxiliaryType.ElectricSystem)
            .AddTechnology(elSystemTechnology);

        Mock<IAuxiliaryDeclarationInputData> fan = null;
        if (!fanTech.IsNullOrEmpty())
        {
            fan = new Mock<IAuxiliaryDeclarationInputData>().SetType(AuxiliaryType.Fan).AddTechnology(fanTech);
            auxInput.AddAuxiliary(fan.Object);
        }

        auxInput.AddAuxiliaries(steeringSystem.Object, hvac.Object, pneumatic.Object, elSystem.Object);
        return auxInput;
    }
}