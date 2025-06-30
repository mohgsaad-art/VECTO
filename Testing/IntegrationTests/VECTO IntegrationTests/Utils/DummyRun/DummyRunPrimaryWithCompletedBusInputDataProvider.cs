using System.Xml;
using Ninject;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCore.InputData.FileIO.XML;

namespace TUGraz.Vecto.IntegrationTests.Utils.DummyRun;

internal class DummyRunPrimaryWithCompletedBusInputDataProvider : IMultistagePrimaryAndStageInputDataProvider
{
    protected IXMLInputDataReader XMLInputReader { set; get; }

    private XmlReader _primaryVehicleReader;
    private XmlReader completedVehicleReader;
    private IDeclarationInputDataProvider _primaryVehicle;
    private IVehicleDeclarationInputData _completedVehicle;
    private bool _simulateResultingVif;


    internal DummyRunPrimaryWithCompletedBusInputDataProvider(XmlReader primaryVehicle, XmlReader completedVehicleData, IXMLInputDataReader xmlReader)
    {
        _primaryVehicleReader = primaryVehicle;
        completedVehicleReader = completedVehicleData;
        XMLInputReader = xmlReader;
        _simulateResultingVif = false;

    }
    internal DummyRunPrimaryWithCompletedBusInputDataProvider(XmlReader primaryVehicle, XmlReader completedVehicleData, IXMLInputDataReader xmlreader, bool runSimulation) : this(primaryVehicle, completedVehicleData, xmlreader)
    {
        _simulateResultingVif = runSimulation;
    }

    #region Implementation of IInputDataProvider

    public DataSource DataSource { get; }

    #endregion

    #region Implementation of IMultistagePrimaryAndStageInputDataProvider

    public IDeclarationInputDataProvider PrimaryVehicle =>
        _primaryVehicle ?? (_primaryVehicle = XMLInputReader.CreateDeclaration(_primaryVehicleReader));

    public IVehicleDeclarationInputData StageInputData => _completedVehicle ?? (_completedVehicle =
        XMLInputReader.CreateDeclaration(completedVehicleReader).JobInputData.Vehicle);

    public bool SimulateResultingVIF => _simulateResultingVif;

    public bool? Completed => null;

    #endregion
}