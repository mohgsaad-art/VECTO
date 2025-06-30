
Imports System.Collections.Generic
Imports System.ComponentModel.DataAnnotations
Imports System.IO
Imports System.Linq
Imports TUGraz.VECTO.Input_Files
Imports TUGraz.VectoCommon.Exceptions
Imports TUGraz.VectoCommon.InputData
Imports TUGraz.VectoCommon.Models
Imports TUGraz.VectoCommon.Utils
Imports TUGraz.VectoCore.Models.SimulationComponent.Data.ElectricComponents.Battery
Imports TUGraz.VectoCore.OutputData.FileIO
Imports TUGraz.VectoCore.Utils

<CustomValidation(GetType(Battery), "ValidateBattery")>
Public Class Battery
    Implements IBatteryPackEngineeringInputData

    Private _filePath As String
    Private _myPath As String
    Public ModelName As String
    Public BatCapacity As Double
    Private _socCurvePath As SubPath
    Private _riCurvePath As SubPath
    Private _maxCurrentPath As SubPath
    Public BatMinSoc As Double
    Public BatMaxSoc As Double
    
    Public Sub New()
        _myPath = ""
        _filePath = ""

        _socCurvePath = New SubPath
        _riCurvePath = New SubPath()
        _maxCurrentPath = new SubPath()

        SetDefault()
    End Sub

    Private Sub SetDefault()
        ModelName = "Undefined"
        BatCapacity = 0
        BatMinSoc = 0
        BatMaxSoc = 0

        _riCurvePath.Clear()
        _socCurvePath.Clear()
        _maxCurrentPath.Clear()
    End Sub

    Public Function SaveFile() As Boolean

        Dim validationResults As IList(Of ValidationResult) =
                Validate(If(Cfg.DeclMode, ExecutionMode.Declaration, ExecutionMode.Engineering), VectoSimulationJobType.BatteryElectricVehicle, Nothing, Nothing, False)

        If validationResults.Count > 0 Then
            Dim messages As IEnumerable(Of String) =
                    validationResults.Select(Function(r) r.ErrorMessage + String.Join(", ", r.MemberNames.Distinct()))
            MsgBox("Invalid input." + Environment.NewLine + String.Join(Environment.NewLine, messages), MsgBoxStyle.OkOnly,
                   "Failed to save battery")
            Return False
        End If

        Try
            Dim writer As JSONFileWriter = New JSONFileWriter()
            writer.SaveBattery(Me, _filePath, Cfg.DeclMode)

        Catch ex As Exception
            MsgBox("Failed to write Battery file: " + ex.Message)
            Return False
        End Try
        Return True
    End Function

    Public Shared Function ValidateBattery(battery As Battery, validationContext As ValidationContext) As ValidationResult
        Dim batterData As BatteryData

        Dim modeService As VectoValidationModeServiceContainer =
                TryCast(validationContext.GetService(GetType(VectoValidationModeServiceContainer)),
                        VectoValidationModeServiceContainer)
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

    Public Property PathSoCCurve(Optional ByVal original As Boolean = False) As String
        Get
            If original Then
                Return _socCurvePath.OriginalPath
            Else
                Return _socCurvePath.FullPath
            End If
        End Get
        Set(ByVal value As String)
            _socCurvePath.Init(_myPath, value)
        End Set
    End Property

    Public Property PathRiCurve(Optional ByVal original As Boolean = False) As String
        Get
            If original Then
                Return _riCurvePath.OriginalPath
            Else
                Return _riCurvePath.FullPath
            End If
        End Get
        Set(ByVal value As String)
            _riCurvePath.Init(_myPath, value)
        End Set
    End Property

    Public Property PathMaxCurrentCurve(Optional ByVal original As Boolean = False) As String
        Get
            If original Then
                Return _maxCurrentPath.OriginalPath
            Else
                Return _maxCurrentPath.FullPath
            End If
        End Get
        Set(ByVal value As String)
            _maxCurrentPath.Init(_myPath, value)
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

    Public ReadOnly Property MinSOC As Double? Implements IBatteryPackDeclarationInputData.MinSOC
        Get
            Return BatMinSoc / 100.0
        End Get
    End Property

    Public ReadOnly Property MaxSOC As Double? Implements IBatteryPackDeclarationInputData.MaxSOC
        Get
            Return BatMaxSoc / 100.0
        End Get
    End Property

    Public ReadOnly Property DeteriorationPerformanceRatio As Double? Implements IBatteryPackDeclarationInputData.DeteriorationPerformanceRatio
        Get
            Return Nothing
        End Get
    End Property

    Public ReadOnly Property BatteryType As BatteryType Implements IBatteryPackDeclarationInputData.BatteryType

    Public ReadOnly Property Capacity As AmpereSecond Implements IBatteryPackDeclarationInputData.Capacity
        Get
            Return BatCapacity.SI(Unit.SI.Ampere.Hour).Cast(Of AmpereSecond)
        End Get
    End Property

    Public Property ConnectorsSubsystemsIncluded As Boolean? Implements IBatteryPackDeclarationInputData.ConnectorsSubsystemsIncluded
    Public Property JunctionboxIncluded As Boolean? Implements IBatteryPackDeclarationInputData.JunctionboxIncluded
    Public Property TestingTemperature As Kelvin Implements IBatteryPackDeclarationInputData.TestingTemperature

    Public ReadOnly Property InternalResistanceCurve As TableData Implements IBatteryPackDeclarationInputData.InternalResistanceCurve
        Get
            If Not File.Exists(_riCurvePath.FullPath) Then _
                Throw New VectoException("Ri Curve is missing or invalid")
            Return VectoCSVFile.Read(_riCurvePath.FullPath)
        End Get
    End Property

    Public ReadOnly Property VoltageCurve As TableData Implements IBatteryPackDeclarationInputData.VoltageCurve
        Get
            If Not File.Exists(_socCurvePath.FullPath) Then _
                Throw New VectoException("SoC Curve is missing or invalid")
            Return VectoCSVFile.Read(_socCurvePath.FullPath)
        End Get
    End Property

    
    Public ReadOnly Property MaxCurrentMap As TableData Implements IBatteryPackDeclarationInputData.MaxCurrentMap
        Get
            If Not File.Exists(_maxCurrentPath.FullPath) Then _
                Throw New VectoException("Max Current Map is missing or invalid")
            Return VectoCSVFile.Read(_maxCurrentPath.FullPath)
        End Get
    End Property

    Public ReadOnly Property StorageType As REESSType Implements IREESSPackInputData.StorageType
    get
        Return REESSType.Battery
    End Get
    End Property
End Class