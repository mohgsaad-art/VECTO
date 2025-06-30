Imports System.Collections.Generic
Imports System.ComponentModel.DataAnnotations
Imports System.IO
Imports System.Linq
Imports TUGraz.VectoCommon.InputData
Imports TUGraz.VectoCommon.Models
Imports TUGraz.VectoCommon.Utils
Imports TUGraz.VectoCore.OutputData.FileIO

Public Class SuperCap 
    Implements ISuperCapEngineeringInputData

    Private _filePath As String
    Private _myPath As String
    Public ModelName As String
    Public Cap As Double
    Public Ri As Double
    Public MinV As Double
    Public MaxV As Double

    Public MaxChgCurrent As Double
    Public MaxDischgCurrent As Double


    Public Sub New()
        _myPath = ""
        _filePath = ""

        SetDefault()
    End Sub

    Private Sub SetDefault()
        ModelName = "Undefined"
        Cap = 0
        Ri = 0
        MinV = 0
        MaxV = 0
    End Sub

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
            Return String.Empty
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
    Public ReadOnly Property Capacity As Farad Implements ISuperCapDeclarationInputData.Capacity
        get
            Return cap.SI(of Farad)
        End Get
    End Property
    Public ReadOnly Property InternalResistance As Ohm Implements ISuperCapDeclarationInputData.InternalResistance
        get
            Return ri.SI(of Ohm)
        End Get
    End Property
    Public ReadOnly Property MinVoltage As Volt Implements ISuperCapDeclarationInputData.MinVoltage
        get
            Return minv.SI(of Volt)
        End Get
    End Property
    Public ReadOnly Property MaxVoltage As Volt Implements ISuperCapDeclarationInputData.MaxVoltage
        get
            Return MaxV.SI(of Volt)
        End Get
    End Property

    Public ReadOnly Property MaxCurrentCharge As Ampere Implements ISuperCapDeclarationInputData.MaxCurrentCharge
    get
        Return MaxChgCurrent.SI(of Ampere)
    End Get
    End Property
    Public ReadOnly Property MaxCurrentDischarge As Ampere Implements ISuperCapDeclarationInputData.MaxCurrentDischarge
        Get
            Return MaxDischgCurrent.SI(Of Ampere)
        End Get
    End Property

    Public Property TestingTemperature As Kelvin Implements ISuperCapDeclarationInputData.TestingTemperature

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
            writer.SaveSuperCap(Me, _filePath, Cfg.DeclMode)

        Catch ex As Exception
            MsgBox("Failed to write Battery file: " + ex.Message)
            Return False
        End Try
        Return True
    End Function

    Public ReadOnly Property StorageType As REESSType Implements IREESSPackInputData.StorageType
        get
            Return REESSType.SuperCap
        End Get
    End Property


End Class