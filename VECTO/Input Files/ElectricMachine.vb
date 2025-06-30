Imports System.Collections.Generic
Imports System.ComponentModel.DataAnnotations
Imports System.IO
Imports System.Linq
Imports TUGraz.VECTO.Input_Files
Imports TUGraz.VectoCommon.Exceptions
Imports TUGraz.VectoCommon.InputData
Imports TUGraz.VectoCommon.Models
Imports TUGraz.VectoCommon.Utils
Imports TUGraz.VectoCore.InputData.FileIO.JSON
Imports TUGraz.VectoCore.InputData.Impl
Imports TUGraz.VectoCore.Models.SimulationComponent.Data
Imports TUGraz.VectoCore.OutputData.FileIO
Imports TUGraz.VectoCore.Utils

<CustomValidation(GetType(ElectricMachine), "ValidateEngine")>
Public Class ElectricMachine
    Implements IElectricMotorEngineeringInputData

    public VoltageLevelLow As Double
    Private ReadOnly _fullLoadCurvePathLow As SubPath
    Private ReadOnly _dragCurvePath As SubPath
    Private ReadOnly _efficiencyMapLow As SubPath

    public VoltageLevelHigh as Double
    Private ReadOnly _fullLoadCurvePathHi As SubPath
    Private ReadOnly _efficiencyMapHi As SubPath

    ''' <summary>
    ''' Directory of engine file. Defined in FilePath property (Set)
    ''' </summary>
    ''' <remarks></remarks>
    Friend _myPath As String

    ''' <summary>
    ''' Full file path. Needs to be defined via FilePath property before calling ReadFile or SaveFile.
    ''' </summary>
    ''' <remarks></remarks>
    Private _filePath As String

    Public ModelName As String
    Public MotorInertia As Double
    Public PeakPowerTimeLo As Double
    Public ContTqLo As Double
    Public RatedSpeedLo As Double
    Public OvlTqLo As Double
    Public OvlSpeedLo As Double

    Public PeakPowerTimeHi As Double
    Public ContTqHi As Double
    Public RatedSpeedHi As Double
    Public OvlTqHi As Double
    Public OvlSpeedHi As Double

    ''' <summary>
    ''' New instance. Initialise
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub New()
        _myPath = ""
        _filePath = ""

        _fullLoadCurvePathLow = New SubPath
        _dragCurvePath = New SubPath()
        _efficiencyMapLow = New SubPath()

        _fullLoadCurvePathHi = New SubPath
        _efficiencyMapHi = New SubPath()

        SetDefault()
    End Sub

    Private Sub SetDefault()
        ModelName = "Undefined"
        MotorInertia = 0


        _fullLoadCurvePathLow.Clear()
        _fullLoadCurvePathHi.Clear()

    End Sub

    Public Function SaveFile() As Boolean

        Dim validationResults As IList(Of ValidationResult) =
                Validate(If(Cfg.DeclMode, ExecutionMode.Declaration, ExecutionMode.Engineering), VectoSimulationJobType.BatteryElectricVehicle, PowertrainPosition.HybridPositionNotSet, Nothing, False)

        If validationResults.Count > 0 Then
            Dim messages As IEnumerable(Of String) =
                    validationResults.Select(Function(r) r.ErrorMessage + String.Join(", ", r.MemberNames.Distinct()))
            MsgBox("Invalid input." + Environment.NewLine + String.Join(Environment.NewLine, messages), MsgBoxStyle.OkOnly,
                   "Failed to save engine")
            Return False
        End If

        Try
            Dim writer As JSONFileWriter = New JSONFileWriter()
            writer.SaveElectricMotor(Me, _filePath, Cfg.DeclMode)

        Catch ex As Exception
            MsgBox("Failed to write Engine file: " + ex.Message)
            Return False
        End Try
        Return True
    End Function


    Public Shared Function ValidateEngine(engine As ElectricMachine, validationContext As ValidationContext) As ValidationResult
        Dim engineData As ElectricMotorData


        Dim modeService As VectoValidationModeServiceContainer =
                TryCast(validationContext.GetService(GetType(VectoValidationModeServiceContainer)),
                        VectoValidationModeServiceContainer)
        Dim mode As ExecutionMode = If(modeService Is Nothing, ExecutionMode.Declaration, modeService.Mode)
        Dim emsCycle As Boolean = (modeService IsNot Nothing) AndAlso modeService.IsEMSCycle
        Dim gbxType As GearboxType? = If(modeService Is Nothing, GearboxType.MT, modeService.GearboxType)

        Try
            'If mode = ExecutionMode.Declaration Then
            '    Dim doa As DeclarationDataAdapterHeavyLorry = New DeclarationDataAdapterHeavyLorry()

            '    'engineData = doa.create(DummyVehicle, engine.EngineModes.First(), New Mission() With {.MissionType = MissionType.LongHaul})
            'Else
            '    Dim doa As EngineeringDataAdapter = New EngineeringDataAdapter()
            '    Dim dummyVehicle As IVehicleEngineeringInputData = New DummyVehicle() With {
            '            .IVehicleComponentsEngineering_EngineInputData = engine
            '            }
            '    engineData = doa.CreateElectricMachines(engine)
            'End If

            'Dim result As IList(Of ValidationResult) =
            '        engineData.Validate(If(Cfg.DeclMode, ExecutionMode.Declaration, ExecutionMode.Engineering), gbxType, emsCycle)

            'If Not result.Any() Then Return ValidationResult.Success

            'Return New ValidationResult("Engine Configuration is invalid. ",
            '                            result.Select(Function(r) r.ErrorMessage + String.Join(Environment.NewLine, r.MemberNames)).ToList())
            Return ValidationResult.Success
        Catch ex As Exception
            Return New ValidationResult(ex.Message)
        End Try
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

    Public ReadOnly Property DataSource As DataSource Implements IComponentInputData.DataSource
        Get
            Dim retVal As DataSource = New DataSource()
            retVal.SourceType = DataSourceType.JSONFile
            retVal.SourceFile = FilePath
            Return retVal
        End Get
    End Property

    Public ReadOnly Property SavedInDeclarationMode As Boolean Implements IComponentInputData.SavedInDeclarationMode
        Get
            Return Cfg.DeclMode
        End Get
    End Property

    Public ReadOnly Property Manufacturer As String Implements IComponentInputData.Manufacturer
        Get

        End Get
    End Property

    Public ReadOnly Property Model As String Implements IComponentInputData.Model
        Get
            Return ModelName
        End Get
    End Property

    Public ReadOnly Property [Date] As Date Implements IComponentInputData.[Date]
        Get
            Return Now.ToUniversalTime()
        End Get
    End Property

    Public ReadOnly Property AppVersion As String Implements IComponentInputData.AppVersion
        Get
            Return "VECTO-GUI"
        End Get
    End Property

    Public ReadOnly Property CertificationMethod As CertificationMethod Implements IComponentInputData.CertificationMethod
        Get
            Return CertificationMethod.NotCertified
        End Get
    End Property
    Public ReadOnly Property CertificationNumber As String Implements IComponentInputData.CertificationNumber
        Get
            Return TUGraz.VectoCore.Configuration.Constants.NOT_AVAILABLE
        End Get
    End Property
    Public ReadOnly Property DigestValue As DigestData Implements IComponentInputData.DigestValue
        Get
            Return Nothing
        End Get
    End Property

    protected ReadOnly Property FullLoadCurveLow As TableData 
        Get
            If Not File.Exists(_fullLoadCurvePathLow.FullPath) Then _
                Throw New VectoException("Full-Load Curve is missing or invalid")
            Return VectoCSVFile.Read(_fullLoadCurvePathLow.FullPath)
        End Get
    End Property
    public ReadOnly Property DragCurve As TableData Implements IElectricMotorDeclarationInputData.DragCurve
        Get
            If Not File.Exists(_dragCurvePath.FullPath) Then _
                Throw New VectoException("Drag Curve is missing or invalid")
            Return VectoCSVFile.Read(_dragCurvePath.FullPath)
        End Get
    End Property
    protected ReadOnly Property EfficiencyMapLow As TableData
        Get
            If Not File.Exists(_efficiencyMapLow.FullPath) Then _
                Throw New VectoException("Drag Curve is missing or invalid")
            Return VectoCSVFile.Read(_efficiencyMapLow.FullPath)
        End Get
    End Property
    protected ReadOnly Property FullLoadCurveHi As TableData 
        Get
            If Not File.Exists(_fullLoadCurvePathHi.FullPath) Then _
                Throw New VectoException("Full-Load Curve is missing or invalid")
            Return VectoCSVFile.Read(_fullLoadCurvePathHi.FullPath)
        End Get
                    End Property
   
    protected ReadOnly Property PowerMapHi As TableData
        Get
            If Not File.Exists(_efficiencyMapHi.FullPath) Then _
                Throw New VectoException("Drag Curve is missing or invalid")
            Return VectoCSVFile.Read(_efficiencyMapHi.FullPath)
        End Get
    End Property

    Public ReadOnly Property VoltageLevels As IList(Of IElectricMotorVoltageLevel) Implements IElectricMotorDeclarationInputData.VoltageLevels
    Get
            Return New List(Of IElectricMotorVoltageLevel) From {
                New ElectricMotorVoltageLevel() With {
                    .VoltageLevel = VoltageLevelLow.SI(Of Volt),
                    .ContinuousTorque = ContTqLo.SI(Of NewtonMeter),
                    .ContinuousTorqueSpeed = RatedSpeedLo.RPMtoRad(),
                    .OverloadTorque = OvlTqLo.SI(Of NewtonMeter),
                    .OverloadTestSpeed = OvlSpeedLo.RPMtoRad(),
                    .OverloadTime = PeakPowerTimeLo.SI(Of Second),
                    .PowerMap = New List(Of IElectricMotorPowerMap) From {New JSONElectricMotorPowerMap With {.PowerMap = EfficiencyMapLow, .Gear = 0}},
                    .FullLoadCurve = New List(Of IElectricMotorLoadCurve) From {New ElectricMotorLoadCurve With {.LoadCurve = FullLoadCurveLow, .Gear = 0}}},
                New ElectricMotorVoltageLevel() With {
                    .VoltageLevel = VoltageLevelHigh.SI(Of Volt),
                    .ContinuousTorque = ContTqHi.SI(Of NewtonMeter),
                    .ContinuousTorqueSpeed = RatedSpeedHi.RPMtoRad(),
                    .OverloadTorque = OvlTqHi.SI(Of NewtonMeter),
                    .OverloadTestSpeed = OvlSpeedHi.RPMtoRad(),
                    .OverloadTime = PeakPowerTimeHi.SI(Of Second),
                    .PowerMap = New List(Of IElectricMotorPowerMap) From {New JSONElectricMotorPowerMap With {.PowerMap = EfficiencyMapLow, .Gear = 0}},
                    .FullLoadCurve = New List(Of IElectricMotorLoadCurve) From {New ElectricMotorLoadCurve With {.LoadCurve = FullLoadCurveLow, .Gear = 0}}}
                }
        End Get
    End Property

    Public Property ElectricMachineType As ElectricMachineType Implements IElectricMotorDeclarationInputData.ElectricMachineType
    Public Property R85RatedPower As Watt Implements IElectricMotorDeclarationInputData.R85RatedPower

    Public ReadOnly Property Inertia As KilogramSquareMeter Implements IElectricMotorDeclarationInputData.Inertia
        Get
            Return MotorInertia.SI(Of KilogramSquareMeter)
        End Get
    End Property

    
    Public ReadOnly Property DcDcConverterIncluded As Boolean Implements IElectricMotorDeclarationInputData.DcDcConverterIncluded
    Public ReadOnly Property IHPCType As String Implements IElectricMotorDeclarationInputData.IHPCType

    

    Public ReadOnly Property Conditioning As TableData Implements IElectricMotorDeclarationInputData.Conditioning
    Public Property OverloadRecoveryFactor As Double Implements IElectricMotorEngineeringInputData.OverloadRecoveryFactor

    'Public ReadOnly Property OverloadTime As Second Implements IElectricMotorDeclarationInputData.OverloadTime
    '    Get
    '        Return PeakPowerTime.SI(Of Second)
    '    End Get
    'End Property
    'Public ReadOnly Property ContinuousTorqueSpeed As PerSecond Implements IElectricMotorDeclarationInputData.ContinuousTorqueSpeed
    '    get
    '        Return RatedSpeed.RPMtoRad()
    '    End Get
    'End Property
    'Public ReadOnly Property ContinuousTorque As NewtonMeter Implements IElectricMotorDeclarationInputData.ContinuousTorque
    'get
    '    Return ContTq.si(of NewtonMeter)
    'End Get
    'End Property

    'Public ReadOnly Property OverloadTorque As NewtonMeter Implements IElectricMotorDeclarationInputData.OverloadTorque
    'get
    '    Return OvlTq.SI(of NewtonMeter)
    'End Get
    'End Property

    'Public ReadOnly Property OverloadTestSpeed As PerSecond Implements IElectricMotorDeclarationInputData.OverloadTestSpeed
    '    Get
    '        Return OvlSpeed.RPMtoRad()
    '    End Get
    'End Property

    Public Property PathMaxTorqueLow(Optional ByVal original As Boolean = False) As String
        Get
            If original Then
                Return _fullLoadCurvePathLow.OriginalPath
            Else
                Return _fullLoadCurvePathLow.FullPath
            End If
        End Get
        Set(ByVal value As String)
            _fullLoadCurvePathLow.Init(_myPath, value)
        End Set
    End Property

    Public Property PathDrag(Optional ByVal original As Boolean = False) As String
        Get
            If original Then
                Return _dragCurvePath.OriginalPath
            Else
                Return _dragCurvePath.FullPath
            End If
        End Get
        Set(ByVal value As String)
            _dragCurvePath.Init(_myPath, value)
        End Set
    End Property

    Public Property PathMapLow(Optional ByVal original As Boolean = False) As String
        Get
            If original Then
                Return _efficiencyMapLow.OriginalPath
            Else
                Return _efficiencyMapLow.FullPath
            End If
        End Get
        Set(ByVal value As String)
            _efficiencyMapLow.Init(_myPath, value)
        End Set
    End Property

    Public Property PathMaxTorqueHi(Optional ByVal original As Boolean = False) As String
        Get
            If original Then
                Return _fullLoadCurvePathHi.OriginalPath
            Else
                Return _fullLoadCurvePathHi.FullPath
            End If
        End Get
        Set(ByVal value As String)
            _fullLoadCurvePathHi.Init(_myPath, value)
        End Set
    End Property


    Public Property PathMapHi(Optional ByVal original As Boolean = False) As String
        Get
            If original Then
                Return _efficiencyMapHi.OriginalPath
            Else
                Return _efficiencyMapHi.FullPath
            End If
        End Get
        Set(ByVal value As String)
            _efficiencyMapHi.Init(_myPath, value)
        End Set
    End Property

End Class