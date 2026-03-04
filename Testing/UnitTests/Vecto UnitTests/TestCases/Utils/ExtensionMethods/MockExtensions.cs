using Moq;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.Simulation.DataBus;
using TUGraz.VectoCore.Models.Simulation.Impl;

namespace TUGraz.Vecto.UnitTests.TestCases.Utils.ExtensionMethods;

public static class MockExtensions
{
    public static Mock<TMocked> WithDefaultAxleNumber<TMocked>(this Mock<TMocked> mock) where TMocked : class, IHasAxleNumber
    { 
        return mock.WithAxleNumber(AxleNumber.NOT_IN_AXLE_POWERTRAIN);
    }
    public static Mock<TMocked> WithAxleNumber<TMocked>(this Mock<TMocked> mock, AxleNumber axleNumber) where TMocked : class, IHasAxleNumber
    {
        mock.Setup(m => m.AxleNumber).Returns(axleNumber);
        return mock;
    }

    // public static Mock<> WithPowertrainDict(this Mock<ISimpleVehicleContainer> container)
    // {
    //     var dict = new Dictionary<AxleNumber, IPowertrainContainer>();
    //     container.Setup(p => p.Powertrains).Returns(() => dict.AsReadOnly());
    //     container.Setup(p => p.AddPowertrainContainer(It.IsAny<IPowertrainContainer>()))
    //         .Callback((IPowertrainContainer p) => dict.Add(p.AxleNumber, p));
    //     
    //     return container;
    // }
    
    public static Mock<ISimpleVehicleContainer> WithPowertrainDict(this Mock<ISimpleVehicleContainer> container)
    {
        var dict = new Dictionary<AxleNumber, IPowertrainContainer>();
        container.Setup(p => p.Powertrains).Returns(() => dict.AsReadOnly());
        container.Setup(p => p.SimplePowertrains).Returns(() => dict.Values.Cast<ISimplePowertrainContainer>()
            .ToDictionary(p => p.AxleNumber, p => p)
            .AsReadOnly());
        container.Setup(p => p.AddPowertrainContainer(It.IsAny<IPowertrainContainer>()))
            .Callback((IPowertrainContainer p) => dict.Add(p.AxleNumber, p));
        
        return container;
    }
    
    public static Mock<IVehicleContainer> WithPowertrainDict(this Mock<IVehicleContainer> container)
    {
        var dict = new Dictionary<AxleNumber, IPowertrainContainer>();
        container.Setup(p => p.Powertrains).Returns(() => dict.AsReadOnly());
        container.Setup(p => p.AddPowertrainContainer(It.IsAny<IPowertrainContainer>()))
            .Callback((IPowertrainContainer p) =>
            {
                dict.Add(p.AxleNumber, p);
            });
        
        return container;
    }
}