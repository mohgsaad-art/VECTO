
Imports System.Collections.Generic
Imports System.ComponentModel.DataAnnotations
Imports System.IO
Imports System.Linq
Imports Ninject
Imports TUGraz.VECTO.Input_Files
Imports TUGraz.VectoCommon.Exceptions
Imports TUGraz.VectoCommon.Hashing
Imports TUGraz.VectoCommon.InputData
Imports TUGraz.VectoCommon.Models
Imports TUGraz.VectoCommon.Utils
Imports TUGraz.VectoCore
Imports TUGraz.VectoCore.InputData.FileIO.XML
Imports TUGraz.VectoCore.InputData.Impl
Imports TUGraz.VectoCore.Models.Declaration
Imports TUGraz.VectoCore.OutputData.FileIO
Imports TUGraz.VectoCore.Utils
Imports TUGraz.VectoHashing

<CustomValidation(GetType(VectoVTPJob), "ValidateJob")>
Public Class VectoVTPJob
    Implements IVTPEngineeringInputDataProvider, IVTPEngineeringJobInputData, IVTPDeclarationInputDataProvider,
               IVTPDeclarationJobInputData, IManufacturerReport, ICompletedVIF

    Private _sFilePath As String
    Private _myPath As String

    Private ReadOnly _vehicleFile As SubPath
    Private ReadOnly _manufacturerRecord As SubPath
    Private ReadOnly _completedVIF As SubPath

    Public ReadOnly CycleFiles As List(Of SubPath)
    Public FanCoefficients As Double()
    Private _fanDiameter As Meter
    Private _fuelNCVData As List(Of IFuelNCVData)

    Private _xmlInputReader As IXMLInputDataReader

    Public Sub New()
        CycleFiles = New List(Of SubPath)
        _vehicleFile = New SubPath
        _manufacturerRecord = New SubPath()
        _completedVIF = New SubPath()
        _fuelNCVData = New List(Of IFuelNCVData)

        Dim kernel As IKernel = New StandardKernel(New VectoNinjectModule)
        _xmlInputReader = kernel.Get(Of IXMLInputDataReader)
    End Sub

    Public Property FilePath As String
        Get
            Return _sFilePath
        End Get
        Set(value As String)
            _sFilePath = value
            If _sFilePath = "" Then
                _myPath = ""
            Else
                _myPath = Path.GetDirectoryName(_sFilePath) & "\"
            End If
        End Set
    End Property


    Public Property PathVeh(Optional ByVal original As Boolean = False) As String
        Get
            If original Then
                Return _vehicleFile.OriginalPath
            Else
                Return _vehicleFile.FullPath
            End If
        End Get
        Set(value As String)
            _vehicleFile.Init(_myPath, value)
        End Set
    End Property


    Public Function SaveFile() As Boolean
        Dim validationResults As IList(Of ValidationResult) =
                Validate(ExecutionMode.Declaration, VectoSimulationJobType.ConventionalVehicle, Nothing, Nothing, False)

        If validationResults.Count > 0 Then
            Dim messages As IEnumerable(Of String) =
                    validationResults.Select(Function(r) r.ErrorMessage + String.Join(", ", r.MemberNames.Distinct()))
            MsgBox("Invalid input." + Environment.NewLine + String.Join(Environment.NewLine, messages),
                   MsgBoxStyle.OkOnly,
                   "Failed to save Vecto Job")
            Return False
        End If

        Try
            Dim writer As JSONFileWriter = JSONFileWriter.Instance
            If Cfg.DeclMode Then
                writer.SaveJob(CType(Me, IVTPDeclarationInputDataProvider), _sFilePath, Cfg.DeclMode)
            Else
                writer.SaveJob(CType(Me, IVTPEngineeringInputDataProvider), _sFilePath, Cfg.DeclMode)
            End If
        Catch ex As Exception
            MsgBox("Failed to save Job file: " + ex.Message)
            Return False
        End Try
        Return True
    End Function

    ' ReSharper disable once UnusedMember.Global -- used by Validation
    Public Shared Function ValidateJob(vectoJob As VectoVTPJob, validationContext As ValidationContext) _
        As ValidationResult
        Dim modeService As VectoValidationModeServiceContainer =
                TryCast(validationContext.GetService(GetType(VectoValidationModeServiceContainer)),
                        VectoValidationModeServiceContainer)
        Dim mode As ExecutionMode = If(modeService Is Nothing, ExecutionMode.Declaration, modeService.Mode)

        Return ValidateVehicleJob(vectoJob, mode)
    End Function

    Private Shared Function ValidateVehicleJob(vectoJob As VectoVTPJob, mode As ExecutionMode) As ValidationResult

        ' TODO!!
        Return ValidationResult.Success
    End Function

    Public ReadOnly Property Vehicle As IVehicleDeclarationInputData Implements IVTPEngineeringJobInputData.Vehicle
        Get
            If Not File.Exists(_vehicleFile.FullPath) Then Return Nothing
            'Return New JSONComponentInputData(_vehicleFile.FullPath).JobInputData.Vehicle
            Return _xmlInputReader.CreateDeclaration(_vehicleFile.FullPath, True).JobInputData.Vehicle
        End Get
    End Property

    Public ReadOnly Property IVTPDeclarationJobInputData_ManufacturerReportInputData As IManufacturerReport Implements IVTPDeclarationJobInputData.ManufacturerReportInputData
        Get
            Return Me
        End Get
    End Property

    Public ReadOnly Property IVTPDeclarationJobInputData_CompletedVIFInputData As ICompletedVIF Implements IVTPDeclarationJobInputData.CompletedVIFInputData
        Get
            Return Me
        End Get
    End Property

    Public ReadOnly Property VectoJobHash As IVectoHash Implements IVTPDeclarationJobInputData.VectoJobHash

    Public ReadOnly Property VectoManufacturerReportHash As IVectoHash Implements IVTPDeclarationJobInputData.VectoManufacturerReportHash
    Public Property Mileage As Meter Implements IVTPDeclarationJobInputData.Mileage

    Public ReadOnly Property Cycles As IList(Of ICycleData) Implements IVTPEngineeringJobInputData.Cycles
        Get
            Dim retVal As ICycleData() = New ICycleData(CycleFiles.Count - 1) {}
            Dim i As Integer = 0
            For Each cycleFile As SubPath In CycleFiles
                Dim cycleData As TableData
                If (File.Exists(cycleFile.FullPath)) Then
                    cycleData = VectoCSVFile.Read(cycleFile.FullPath)
                Else
                    Try
                        Dim resourceName As String = DeclarationData.DeclarationDataResourcePrefix + ".MissionCycles." +
                                                     cycleFile.OriginalPath +
                                                     TUGraz.VectoCore.Configuration.Constants.FileExtensions.CycleFile
                        Dim cycleDataRes As Stream = RessourceHelper.ReadStream(resourceName)
                        cycleData = VectoCSVFile.ReadStream(cycleDataRes, source:=resourceName)
                    Catch ex As Exception
                        Throw New VectoException("Driving Cycle could not be read: " + cycleFile.OriginalPath)
                    End Try
                End If
                retVal(i) = New CycleInputData With {
                    .Name = Path.GetFileNameWithoutExtension(cycleFile.FullPath),
                    .CycleData = cycleData
                    }
                i += 1
            Next
            Return retVal
        End Get
    End Property

    Public ReadOnly Property FuelNCVs As IList(Of IFuelNCVData) _
        Implements IVTPEngineeringJobInputData.FuelNCVs
        Get
            Return _fuelNCVData
        End Get
    End Property

    Public Property TorqueDriftLeftWheel As NewtonMeter Implements IVTPEngineeringJobInputData.TorqueDriftLeftWheel

    Public Property TorqueDriftRightWheel As NewtonMeter Implements IVTPEngineeringJobInputData.TorqueDriftRightWheel

    Public ReadOnly Property FanPowerCoefficents As IEnumerable(Of Double) _
        Implements IVTPEngineeringJobInputData.FanPowerCoefficents
        Get
            Return FanCoefficients
        End Get
    End Property

    Public ReadOnly Property SavedInDeclarationMode As Boolean _
        Implements IVTPEngineeringJobInputData.SavedInDeclarationMode
        Get
            Return False
        End Get
    End Property

    Public Property FanDiameter As Meter Implements IVTPEngineeringJobInputData.FanDiameter
        Get
            Return _fanDiameter
        End Get
        Set
            _fanDiameter = Value
        End Set
    End Property

    Public ReadOnly Property JobInputData As IVTPEngineeringJobInputData _
        Implements IVTPEngineeringInputDataProvider.JobInputData
        Get
            Return Me
        End Get
    End Property


    Public ReadOnly Property IVTPDeclarationInputDataProvider_JobInputData As IVTPDeclarationJobInputData _
        Implements IVTPDeclarationInputDataProvider.JobInputData
        Get
            Return Me
        End Get
    End Property

    Public Property ManufacturerRecord(Optional ByVal original As Boolean = False) As String
        Get
            If original Then
                Return _manufacturerRecord.OriginalPath
            Else
                Return _manufacturerRecord.FullPath
            End If
        End Get
        Set(value As String)
            _manufacturerRecord.Init(_myPath, value)
        End Set
    End Property

    Public Property CompletedVIF(Optional ByVal original As Boolean = False) As String
        Get
            If original Then
                Return _completedVIF.OriginalPath
            Else
                Return _completedVIF.FullPath
            End If
        End Get
        Set(value As String)
            _completedVIF.Init(_myPath, value)
        End Set
    End Property

    Public ReadOnly Property Source As String Implements IManufacturerReport.Source
        Get
            Return _manufacturerRecord.FullPath
        End Get
    End Property

    Public ReadOnly Property Results As IResultsInputData Implements IManufacturerReport.Results

    Public ReadOnly Property ComponentDigests As IDictionary(Of VectoComponents, IList(Of String)) Implements IManufacturerReport.ComponentDigests
    Public ReadOnly Property JobDigest As DigestData Implements IManufacturerReport.JobDigest

    Public Sub ValidateSimulationToolVersion() Implements IManufacturerReport.ValidateSimulationToolVersion

    End Sub

    Public Sub ValidateHash() Implements IManufacturerReport.ValidateHash

    End Sub

    Public ReadOnly Property DataSource As DataSource Implements IInputDataProvider.DataSource
        Get
            Dim retVal As DataSource = New DataSource()
            retVal.SourceType = DataSourceType.JSONFile
            retVal.SourceFile = FilePath
            Return retVal
        End Get
    End Property

    Private ReadOnly Property ICompletedVIF_Source As String Implements ICompletedVIF.Source
        Get
            Return _completedVIF.FullPath
        End Get
    End Property

    Public ReadOnly Property VehicleLength As Meter Implements ICompletedVIF.VehicleLength
        Get
            Throw New NotImplementedException()
        End Get
    End Property

    Public ReadOnly Property BodyworkCode As VehicleCode Implements ICompletedVIF.BodyworkCode
        Get
            Throw New NotImplementedException()
        End Get
    End Property
End Class
