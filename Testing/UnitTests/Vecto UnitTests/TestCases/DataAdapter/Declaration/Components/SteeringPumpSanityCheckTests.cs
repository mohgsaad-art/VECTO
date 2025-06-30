using Moq;
using NUnit.Framework;
using TUGraz.Vecto.UnitTests.Utils;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.Reader.DataObjectAdapter.SimulationComponents.AuxiliaryDataAdapter;

namespace TUGraz.Vecto.UnitTests.TestCases.DataAdapter.Declaration.Components;

public class SteeringPumpSanityCheckTests
{
    [
        TestCase(1, "Fixed displacement"),
        TestCase(2, "Fixed displacement", "Dual displacement"),
        TestCase(2, "Full electric steering gear", "Electric driven pump"),
        TestCase(1, "Full electric steering gear"),
    ]
    public void TestCorrectNumberSteeredAxlesLorry(int numStreeredAxles, params string[] steeringPumpTechnologies)
    {
        var mission = MissionType.RegionalDelivery;
        var hdvClass = VehicleClass.Class9;

        var doa = new HeavyLorryAuxiliaryDataAdapter();
        var auxInputData = GetLorryAuxInputData(steeringPumpTechnologies);
        var aux = doa.CreateAuxiliaryData(auxInputData, null, mission, hdvClass, null, numStreeredAxles,
            VectoSimulationJobType.ConventionalVehicle);
        Assert.IsNotNull(aux);
    }

    [
        TestCase(1, "Fixed displacement"),
        TestCase(2, "Fixed displacement", "Dual displacement"),
        TestCase(2, "Full electric steering gear", "Electric driven pump"),
        TestCase(1, "Full electric steering gear")
    ]
    public void TestCorrectNumberSteeredAxlesBus(int numStreeredAxles, params string[] steeringPumpTechnologies)
    {
        var mission = MissionType.Urban;
        var hdvClass = VehicleClass.Class9;

        var doa = new PrimaryBusAuxiliaryDataAdapter();
        var busAuxInputData = GetBusAuxInputData(steeringPumpTechnologies);
        var aux = doa.CreateAuxiliaryData(null, busAuxInputData, mission, hdvClass, 12.SI<Meter>(), numStreeredAxles,
            VectoSimulationJobType.ConventionalVehicle);
        Assert.IsNotNull(aux);
    }

    [
        TestCase(2, "Fixed displacement"),
        TestCase(1, "Fixed displacement", "Dual displacement"),
        TestCase(1, "Full electric steering gear", "Electric driven pump"),
        TestCase(1, "Fixed displacement", "Dual displacement", "Full electric steering gear"),
    ]
    public void TestWrongNumberSteeredAxlesLorry(int numStreeredAxles, params string[] steeringPumpTechnologies)
    {
        var mission = MissionType.RegionalDelivery;
        var hdvClass = VehicleClass.Class9;

        var doa = new HeavyLorryAuxiliaryDataAdapter();
        var auxInputData = GetLorryAuxInputData(steeringPumpTechnologies);
        AssertHelper.Exception<VectoException>(() =>
        {
            var aux = doa.CreateAuxiliaryData(auxInputData, null, mission, hdvClass, null, numStreeredAxles,
                VectoSimulationJobType.ConventionalVehicle);
        }, messageContains: $"Number of steering pump technologies does not match number of steered axles ({numStreeredAxles}, {steeringPumpTechnologies.Length})");
    }

    [
        TestCase(2, "Fixed displacement"),
        TestCase(1, "Fixed displacement", "Dual displacement"),
        TestCase(1, "Full electric steering gear", "Electric driven pump"),
        TestCase(1, "Fixed displacement", "Dual displacement", "Full electric steering gear"),
    ]
    public void TestWrongNumberSteeredAxlesBus(int numStreeredAxles, params string[] steeringPumpTechnologies)
    {
        var mission = MissionType.Urban;
        var hdvClass = VehicleClass.Class9;

        var doa = new PrimaryBusAuxiliaryDataAdapter();
        var busAuxInputData = GetBusAuxInputData(steeringPumpTechnologies);
        AssertHelper.Exception<VectoException>(() =>
        {
            doa.CreateAuxiliaryData(null, busAuxInputData, mission, hdvClass, 12.SI<Meter>(), numStreeredAxles,
                VectoSimulationJobType.ConventionalVehicle);

        }, messageContains: $"Number of steering pump technologies does not match number of steered axles ({numStreeredAxles}, {steeringPumpTechnologies.Length})");

    }

    private IAuxiliariesDeclarationInputData GetLorryAuxInputData(string[] steeringPumpTechnologies)
    {
        var aux = new Mock<IAuxiliariesDeclarationInputData>();

        var fan = new Mock<IAuxiliaryDeclarationInputData>();
        fan.Setup(f => f.Type).Returns(AuxiliaryType.Fan);
        fan.Setup(f => f.Technology).Returns(new[] { "Crankshaft mounted - Discrete step clutch" });
        var sp = new Mock<IAuxiliaryDeclarationInputData>();
        sp.Setup(f => f.Type).Returns(AuxiliaryType.SteeringPump);
        sp.Setup(s => s.Technology).Returns(steeringPumpTechnologies);
        var ps = new Mock<IAuxiliaryDeclarationInputData>();
        ps.Setup(f => f.Type).Returns(AuxiliaryType.PneumaticSystem);
        ps.Setup(s => s.Technology).Returns(new[] { "Small + ESS" });
        var es = new Mock<IAuxiliaryDeclarationInputData>();
        es.Setup(f => f.Type).Returns(AuxiliaryType.ElectricSystem);
        es.Setup(s => s.Technology).Returns(new[] { "Standard technology" });
        var hvac = new Mock<IAuxiliaryDeclarationInputData>();
        hvac.Setup(f => f.Type).Returns(AuxiliaryType.HVAC);
        hvac.Setup(s => s.Technology).Returns(new[] { "Default" });

        aux.Setup(a => a.Auxiliaries).Returns(new List<IAuxiliaryDeclarationInputData>() { fan.Object, sp.Object, es.Object, ps.Object, hvac.Object });

        return aux.Object;
    }

    private IBusAuxiliariesDeclarationData GetBusAuxInputData(string[] steeringPumpTechnologies)
    {
        var aux = new Mock<IBusAuxiliariesDeclarationData>();

        aux.Setup(a => a.SteeringPumpTechnology).Returns(steeringPumpTechnologies);
        aux.Setup(f => f.FanTechnology).Returns("Crankshaft mounted - Discrete step clutch");

        return aux.Object;
    }
}