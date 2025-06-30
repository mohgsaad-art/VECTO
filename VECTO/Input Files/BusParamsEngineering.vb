Imports System.Collections.Generic
Imports System.ComponentModel.DataAnnotations
Imports System.IO
Imports System.Linq
Imports TUGraz.VECTO.Input_Files
Imports TUGraz.VectoCommon.Exceptions
Imports TUGraz.VectoCommon.InputData
Imports TUGraz.VectoCommon.Models
Imports TUGraz.VectoCommon.Utils
Imports TUGraz.VectoCore.OutputData.FileIO
Imports TUGraz.VectoCore.Utils

Public Class BusAuxEngineeringParams
    Implements IBusAuxiliariesEngineeringData, 
               IBusAuxPneumaticSystemEngineeringData, 
               IBusAuxElectricSystemEngineeringData,
               IBusAuxHVACData

    Private _filePath As String
    Private _myPath As String

    public  CurrentDemandEngineOn as double
    public  CurrentDemandEngineOffDriving as double
    public  CurrentDemandEngineOffStandstill as double

    public  AlternatorEfficiency as double
    public  MaxAlternatorPower as double
    public  ElectricStorageCapacity as double
    public AlternatorType As AlternatorType

    public CompressorMap as SubPath
    public AverageAirDemand as Double
    public GearRatio As Double
    public SmartCompression as Boolean

    public ElectricPowerDemand as Double
    public MechanicalPowerDemand As Double
    public AuxHeaterPower As Double
    public AverageHeatingDemand As Double
    Public DCDCEfficiency As Double
    Public SupplyESFromHEVREESS As Boolean


    Public Sub New()
        _myPath = ""
        _filePath = ""

        CompressorMap = new SubPath()


        SetDefault()
    End Sub

    Private Sub SetDefault()
        CompressorMap.Clear()
    End Sub

    Public Function SaveFile() As Boolean

        Dim validationResults As IList(Of ValidationResult) =
                Validate(If(Cfg.DeclMode, ExecutionMode.Declaration, ExecutionMode.Engineering), VectoSimulationJobType.ParallelHybridVehicle, Nothing, Nothing, False)

        If validationResults.Count > 0 Then
            Dim messages As IEnumerable(Of String) =
                    validationResults.Select(Function(r) r.ErrorMessage + String.Join(", ", r.MemberNames.Distinct()))
            MsgBox("Invalid input." + Environment.NewLine + String.Join(Environment.NewLine, messages), MsgBoxStyle.OkOnly,
                   "Failed to save strategy parameters")
            Return False
        End If

        Try
            Dim writer As JSONFileWriter = New JSONFileWriter()
            writer.SaveBusAuxEngineeringParameters(Me, _filePath, Cfg.DeclMode)

        Catch ex As Exception
            MsgBox("Failed to write auxiliary parameters file: " + ex.Message)
            Return False
        End Try
        Return True
    End Function

    Public Property FilePath() As String
        Get
            Return _filePath
        End Get
        Set(ByVal value As String)
            _filePath = value
            If _filePath = "" Then
                _myPath = ""
            Else
                _myPath = Path.GetDirectoryName(_filePath) & "\"
            End If
        End Set
    End Property

    Public Property PathCompressorMap(Optional ByVal original As Boolean = False) As String
        Get
            If original Then
                Return CompressorMap.OriginalPath
            Else
                Return CompressorMap.FullPath
            End If
        End Get
        Set(ByVal value As String)
            CompressorMap.Init(_myPath, value)
        End Set
    End Property

    Public ReadOnly Property DataSource As DataSource Implements IBusAuxiliariesEngineeringData.DataSource
    get
        Return new DataSource() With{ .SourceFile = FilePath }
    End Get
    End Property

    Public ReadOnly Property PneumaticSystem As IBusAuxPneumaticSystemEngineeringData Implements IBusAuxiliariesEngineeringData.PneumaticSystem
    get
        return me
    End Get
    End Property

    Public ReadOnly Property ElectricSystem As IBusAuxElectricSystemEngineeringData Implements IBusAuxiliariesEngineeringData.ElectricSystem
    get
        Return me
    End Get
    End Property

    Public ReadOnly Property HVACData As IBusAuxHVACData Implements IBusAuxiliariesEngineeringData.HVACData
    get
        Return me
    End Get
    End Property

    Public ReadOnly Property PS_CompressorMap As TableData Implements IBusAuxPneumaticSystemEngineeringData.CompressorMap
    Get
            If JobType = VectoSimulationJobType.BatteryElectricVehicle OrElse JobType = VectoSimulationJobType.IEPC_E OrElse JobType = VectoSimulationJobType.FCHV OrElse JobType = VectoSimulationJobType.FCHV_IEPC Then
                Return Nothing
            End If
            If Not file.Exists(CompressorMap.FullPath) Then
             Throw new VectoException("Compressor Map is missing or invalid")               
        End If
        Return VectoCSVFile.Read(CompressorMap.FullPath)
    End Get
    End Property
    Public ReadOnly Property PS_AverageAirConsumed As NormLiterPerSecond Implements IBusAuxPneumaticSystemEngineeringData.AverageAirConsumed
    get
        Return AverageAirDemand.SI(Of NormLiterPerSecond)
    End Get
    End Property

    Public ReadOnly Property PS_SmartAirCompression As Boolean Implements IBusAuxPneumaticSystemEngineeringData.SmartAirCompression
    get
            Return SmartCompression
    End Get
    End Property

    Public ReadOnly Property PS_GearRatio As Double Implements IBusAuxPneumaticSystemEngineeringData.GearRatio
    get
            Return GearRatio
    End Get
    End Property

    Public ReadOnly Property ES_AlternatorEfficiency As Double Implements IBusAuxElectricSystemEngineeringData.AlternatorEfficiency
    get
            Return AlternatorEfficiency
    End Get
    End Property

    Public ReadOnly Property ES_CurrentDemand As Ampere Implements IBusAuxElectricSystemEngineeringData.CurrentDemand
    get
            return CurrentDemandEngineOn.SI(of Ampere)
    End Get
    End Property

    Public ReadOnly Property ES_MaxAlternatorPower As Watt Implements IBusAuxElectricSystemEngineeringData.MaxAlternatorPower
    get
        Return MaxAlternatorPower.SI(of Watt)
    End Get
    End Property

    Public ReadOnly Property ES_ElectricStorageCapacity As WattSecond Implements IBusAuxElectricSystemEngineeringData.ElectricStorageCapacity
    get
        Return ElectricStorageCapacity.SI(Unit.SI.Watt.Hour).Cast(Of WattSecond)
    End Get
    End Property

    Public ReadOnly Property ES_CurrentDemandEngineOffStandstill As Ampere Implements IBusAuxElectricSystemEngineeringData.CurrentDemandEngineOffStandstill
    get
            Return CurrentDemandEngineOffStandstill.SI(of Ampere)
    End Get
    End Property

    Public ReadOnly Property ES_CurrentDemandEngineOffDriving As Ampere Implements IBusAuxElectricSystemEngineeringData.CurrentDemandEngineOffDriving
    get
            Return CurrentDemandEngineOffDriving.SI(of Ampere)
    End Get
    End Property

    Public ReadOnly Property HVAC_ElectricalPowerDemand As Watt Implements IBusAuxHVACData.ElectricalPowerDemand
    get
            Return ElectricPowerDemand.SI(of Watt)
    End Get
    End Property

    Public ReadOnly Property HVAC_MechanicalPowerDemand As Watt Implements IBusAuxHVACData.MechanicalPowerDemand
    get
            Return MechanicalPowerDemand.SI(of Watt)
    End Get
    End Property

    Public ReadOnly Property HVAC_AverageHeatingDemand As Joule Implements IBusAuxHVACData.AverageHeatingDemand
    get
            Return AverageHeatingDemand.SI(unit.SI.Mega.Joule).Cast(of Joule)
    End Get
    End Property
    Public ReadOnly Property HVAC_AuxHeaterPower As Watt Implements IBusAuxHVACData.AuxHeaterPower
    get
        Return AuxHeaterPower.SI(of Watt)
    End Get
    End Property

    Public ReadOnly Property DCDCConverterEfficiency As Double Implements IBusAuxElectricSystemEngineeringData.DCDCConverterEfficiency
    get
        Return DCDCEfficiency
    End Get
    End Property

    Public ReadOnly Property ESSupplyFromHEVREESS As Boolean Implements IBusAuxElectricSystemEngineeringData.ESSupplyFromHEVREESS
    get
        Return SupplyESFromHEVREESS
    End Get
    End Property

    Public Property ElectricStorageEfficiency As Double Implements IBusAuxElectricSystemEngineeringData.ElectricStorageEfficiency

    Public ReadOnly Property IBusAuxElectricSystemEngineeringData_AlternatorType As AlternatorType Implements IBusAuxElectricSystemEngineeringData.AlternatorType
    get
        Return AlternatorType
    End Get
    End Property

    Public Property JobType As VectoSimulationJobType

End Class