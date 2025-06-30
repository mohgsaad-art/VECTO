using Moq;
using NUnit.Framework;
using TUGraz.Vecto.UnitTests.Utils;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.Reader.DataObjectAdapter.SimulationComponents;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.SimulationComponent;
using TUGraz.VectoCore.Models.SimulationComponent.Data;

namespace TUGraz.Vecto.UnitTests.TestCases.DataAdapter.Declaration.Components;

public class GearboxDataAdapterTests
{

    [TestCase()]
    public void Gearbox_LessThanTwoGearsException()
    {
        var dataAdapter = new GearboxDataAdapter(null);
        var gbxTypes = new[] { GearboxType.AMT };

        var inputData = GetMockInputData();
        var runData = GetDummyRunData();
        var shiftPolygonCalc = new Mock<IShiftPolygonCalculator>();
        AssertHelper.Exception<VectoException>(
            //() => MockSimulationDataFactory.CreateGearboxDataFromFile(wrongFile, EngineDataFile),
            () => dataAdapter.CreateGearboxData(inputData, runData, shiftPolygonCalc.Object, gbxTypes),
            "At least one Gear-Entry must be defined in Gearbox!");
    }



    private VectoRunData GetDummyRunData()
    {
        return new VectoRunData()
        {
            VehicleData = new VehicleData()
            {
                DynamicTyreRadius = 0.5.SI<Meter>(),
            }
        };
    }

    private IVehicleDeclarationInputData GetMockInputData()
    {
        var input = new Mock<IVehicleDeclarationInputData>();
        var components = new Mock<IVehicleComponentsDeclaration>();
        var gbx = new Mock<IGearboxDeclarationInputData>();
        //var vehicle = new Mock<IVehicleDeclarationInputData>();

        //var gearRatios = new double[] { 1.0 };
        //var gears = gearRatios.Select((i, idx) => {
        //	var g = new Mock<ITransmissionInputData>();
        //	g.Setup(x => x.Ratio).Returns(i);
        //	g.Setup(x => x.Gear).Returns(idx + 1);
        //	g.Setup(x => x.LossMap)
        //		.Returns(VectoCSVFile.ReadStream(InputDataHelper.InputDataAsStream(LossMapHdr, LossMapData)));
        //	return g.Object;
        //}).ToList();

        input.Setup(i => i.Components).Returns(components.Object);
        components.Setup(c => c.GearboxInputData).Returns(gbx.Object);
        gbx.Setup(g => g.Type).Returns(GearboxType.AMT);
        gbx.Setup(g => g.Gears).Returns(new List<ITransmissionInputData>());
        return input.Object;
    }

    public const string LossMapHdr = "Input Speed [rpm],Input Torque [Nm],Torque Loss [Nm]";

    public readonly string[] LossMapData = new[] {
        "0,0,0",
        "0,-100000,100",
        "0,100000,100",
        "5000,-100000,100",
        "5000,100000,100",
    };



}