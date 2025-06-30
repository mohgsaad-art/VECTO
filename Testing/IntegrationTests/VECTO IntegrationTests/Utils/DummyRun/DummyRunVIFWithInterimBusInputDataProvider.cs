using System.Xml;
using System.Xml.Linq;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.FileIO.XML;

namespace TUGraz.Vecto.IntegrationTests.Utils.DummyRun;

public class DummyRunVIFWithInterimBusInputDataProvider : IMultistageVIFInputData
{
    protected IXMLInputDataReader XMLInputReader { set; get; }

    //private XmlReader vifReader;
    private XmlReader stageInput;
    private IVehicleDeclarationInputData _vehicleInput;
    private IMultistepBusInputDataProvider _stageInput;
    private bool _simulateResultingVif;

    internal DummyRunVIFWithInterimBusInputDataProvider(XDocument vifXml, XmlReader multistageInput, IXMLInputDataReader xmlReader)
    {
        //vifReader = vifXml;
        VIF = vifXml;
        //vifXml.
        stageInput = multistageInput;
        XMLInputReader = xmlReader;
    }

    internal DummyRunVIFWithInterimBusInputDataProvider(XDocument vifXml, XmlReader multistageInput, IXMLInputDataReader xmlReader, bool runSimulation) : this(vifXml, multistageInput, xmlReader)
    {
        _simulateResultingVif = runSimulation;
    }


    public bool IsComplete => MultistageJobInputData.JobInputData.InputComplete;

    public IList<string> InvalidEntries => MultistageJobInputData.JobInputData.InvalidEntries;


    #region Implementation of IInputDataProvider

    public DataSource DataSource { get; }

    #endregion

    #region Implementation of IMultistageVIFInputData

    public IVehicleDeclarationInputData VehicleInputData => stageInput == null
        ? null
        : _vehicleInput ??
        (_vehicleInput = ReadVehicle());

    private IVehicleDeclarationInputData ReadVehicle()
    {
        return XMLInputReader.CreateDeclaration(stageInput).JobInputData.Vehicle;
    }

    public IMultistepBusInputDataProvider MultistageJobInputData => _stageInput ??
                                                                    (_stageInput =
                                                                        XMLInputReader.CreateDeclaration(
                                                                                XmlReader.Create(VIF.ToString().ToStream())) as
                                                                            IMultistepBusInputDataProvider);

    public bool SimulateResultingVIF => _simulateResultingVif;

    public XDocument VIF { get; }

    #endregion
}