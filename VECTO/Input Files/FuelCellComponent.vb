Imports System.ComponentModel.DataAnnotations
Imports System.IO
Imports TUGraz.VectoCommon.Exceptions
Imports TUGraz.VectoCommon.InputData
Imports TUGraz.VectoCommon.Models
Imports TUGraz.VectoCommon.Utils
Imports TUGraz.VectoCore.Utils

Public Class FuelCellComponent
    Implements IFuelCellComponentEngineeringInputData

    Private _filePath As String
    Private _basePath As String

    Public Property FilePath As String
        Get
            Return _filePath
        End Get
        Set(ByVal value As String)
            _filePath = value
            If _filePath = "" Then
                _basePath = ""
            Else
                _basePath = Path.GetDirectoryName(_filePath)
            End If
        End Set
    End Property

    Public ReadOnly Property DataSource As DataSource Implements IComponentInputData.DataSource
        Get
            Dim retVal = New DataSource()
            retVal.SourceType = DataSourceType.JSONFile
            retVal.SourceFile = _filePath
            Return retVal
        End Get
    End Property

    Public ReadOnly Property SavedInDeclarationMode As Boolean Implements IComponentInputData.SavedInDeclarationMode

    Public Property Manufacturer As String Implements IComponentInputData.Manufacturer

    Public Property Model As String Implements IComponentInputData.Model

    Public ReadOnly Property [Date] As Date Implements IComponentInputData.[Date]

    Public ReadOnly Property AppVersion As String Implements IComponentInputData.AppVersion

    Public ReadOnly Property CertificationMethod As CertificationMethod Implements IComponentInputData.CertificationMethod
        Get
            Throw New NotImplementedException
        End Get
    End Property

    Public ReadOnly Property CertificationNumber As String Implements IComponentInputData.CertificationNumber
        Get
            Throw New NotImplementedException
        End Get
    End Property

    Public ReadOnly Property DigestValue As DigestData Implements IComponentInputData.DigestValue
        Get
            Throw New NotImplementedException
        End Get
    End Property

    Public ReadOnly Property MassFlowMap As TableData Implements IFuelCellComponentEngineeringInputData.MassFlowMap
        Get
            If Not File.Exists(MassFlowMapFile) Then _
                Throw New VectoException("Mass flow map file is missing or invalid")
            Return VectoCSVFile.Read(MassFlowMapFile)
        End Get
    End Property

    Public Property MassFlowMapFile As String

    Public Property MaxElectricPower As Watt Implements IFuelCellComponentEngineeringInputData.MaxElectricPower

    Public Property MinElectricPower As Watt Implements IFuelCellComponentEngineeringInputData.MinElectricPower

    Public Function SaveFile() As Boolean
        Dim validationResults As IList(Of ValidationResult) = Validate(If(Cfg.DeclMode, ExecutionMode.Declaration, ExecutionMode.Engineering),
                                                                       VectoSimulationJobType.BatteryElectricVehicle, Nothing, Nothing, False)

        If validationResults.Count > 0 Then
            Dim messages As IEnumerable(Of String) = validationResults.Select(Function(r) r.ErrorMessage + String.Join(", ", r.MemberNames.Distinct()))
            MsgBox("Invalid input." + Environment.NewLine + String.Join(Environment.NewLine, messages), MsgBoxStyle.OkOnly,
                   "Failed to save fuel cell component")
            Return False
        End If

        Try
            Dim writer As JSONFileWriter = New JSONFileWriter()
            writer.SaveFuelCellComponent(Me, _filePath, False)
        Catch ex As Exception
            MsgBox("Failed to write fuel cell component file: " + ex.Message)
            Return False
        End Try
        Return True






    End Function
End Class