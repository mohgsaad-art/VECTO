using Moq;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData;
using TUGraz.VectoCore.InputData.FileIO.JSON;
using TUGraz.VectoCore.InputData.FileIO.XML;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.Simulation.DataBus;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.Models.Simulation.Impl.SimulatorFactory;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.OutputData.XML;

namespace TUGraz.Vecto.IntegrationTests.Utils.DummyRun;

public class DummyRunDeclarationSimulatorFactory : SimulatorFactoryDeclaration
{
    protected ISimplePowertrainBuilder SimplePowertrainBuilder { get; private set; }

    public DummyRunDeclarationSimulatorFactory(IInputDataProvider dataProvider, IOutputDataWriter writer,
        IDeclarationReport declarationReport, IVTPReport vtpReport, bool validate,
        // the following parameters are injected
        IXMLInputDataReader xmlInputDataReader, ISimulatorFactoryFactory simulatorFactoryFactory,
        IXMLDeclarationReportFactory xmlDeclarationReportFactory, IVectoRunDataFactoryFactory runDataFactoryFactory,
        IPowertrainBuilder ptBuilder, ISimplePowertrainBuilder simplePtBuilder, IModalDataFactory modDataFactory)
        : base(dataProvider, writer, declarationReport, vtpReport, validate, xmlInputDataReader,
            simulatorFactoryFactory, xmlDeclarationReportFactory, runDataFactoryFactory, ptBuilder, modDataFactory)
    {
        SimplePowertrainBuilder = simplePtBuilder;
        CheckInputData(dataProvider);
    }

    public DummyRunDeclarationSimulatorFactory(IInputDataProvider dataProvider,
        IOutputDataWriter writer, bool validate,
        // the following parameters are injected
        IXMLInputDataReader xmlInputDataReader,
        ISimulatorFactoryFactory simulatorFactoryFactory,
        IXMLDeclarationReportFactory xmlDeclarationReportFactory,
        IVectoRunDataFactoryFactory runDataFactoryFactory, IPowertrainBuilder ptBuilder, IModalDataFactory modDataFactory)
        : base(dataProvider, writer, validate,
            xmlInputDataReader, simulatorFactoryFactory, xmlDeclarationReportFactory, runDataFactoryFactory,
            ptBuilder, modDataFactory)
    {
        CheckInputData(dataProvider);
    }

    private void CheckInputData(IInputDataProvider dataProvider)
    {
        if (dataProvider is JSONFile json && !(dataProvider is JSONInputDataV10_PrimaryAndStageInputBus || dataProvider is JSONInputDataCompletedBusFactorMethodV7))
        {
            throw new VectoException($"JSON input data is not supported");
        }
    }

    protected override IVectoRun GetExemptedRun(VectoRunData data)
    {

        if (data.Report != null)
        {
            data.Report.PrepareResult(data);
        }
        return new DummyRunExemptedRun(new ExemptedVehicleContainer(data, null, null, SimplePowertrainBuilder), modData =>
        {
            if (data.Report != null)
            {
                data.Report.AddResult(data, modData);
            }
        });
    }

    protected override IVectoRun GetNonExemptedRun(VectoRunData data, int current, ref bool warning1Hz, ref bool firstRun)
    {
        var addReportResult = PrepareReport(data);
        var modContainer = ModDataFactory.CreateModDataContainer(
            //new ModalDataContainer(
            data, ReportWriter,
            _mode == ExecutionMode.Declaration ? addReportResult : null,
            null);

        var container = PowertrainBuilder.Build(data, modContainer, null);
        var mock = Mock.Get(container);
        var milage = new Mock<IMileageCounter>();
        milage.Setup(m => m.Distance).Returns(0.SI<Meter>());
        var vi = new Mock<IVehicleInfo>();
        vi.Setup(m => m.VehicleSpeed).Returns(0.KMPHtoMeterPerSecond());
        mock.Setup(c => c.MileageCounter).Returns(milage.Object);
        mock.Setup(c => c.VehicleInfo).Returns(vi.Object);
        return new DummyRunNonExemptedRun(mock.Object, new DummyRunPostMortemAnalyzer());

    }
    protected new static Action<IModalDataContainer> PrepareReport(VectoRunData data)
    {
        if (data.Report != null)
        {
            data.Report.PrepareResult(data);
        }
        Action<IModalDataContainer> addReportResult = modData =>
        {
            data?.Report?.AddResult(data, modData);
        };

        return addReportResult;
    }

}