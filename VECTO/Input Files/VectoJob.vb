' Copyright 2017 European Union.
' Licensed under the EUPL (the 'Licence');
'
' * You may not use this work except in compliance with the Licence.
' * You may obtain a copy of the Licence at: http://ec.europa.eu/idabc/eupl
' * Unless required by applicable law or agreed to in writing,
'   software distributed under the Licence is distributed on an "AS IS" basis,
'   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
'
' See the LICENSE.txt for the specific language governing permissions and limitations.
'Option Infer On
'Option Explicit On

Imports System.Collections.Generic
Imports System.ComponentModel.DataAnnotations
Imports System.IO
Imports System.Linq
Imports System.Xml.Linq
Imports TUGraz.VECTO.Input_Files
Imports TUGraz.VectoCommon.Exceptions
Imports TUGraz.VectoCommon.InputData
Imports TUGraz.VectoCommon.Models
Imports TUGraz.VectoCommon.Utils
Imports TUGraz.VectoCore.InputData.FileIO.JSON
Imports TUGraz.VectoCore.InputData.Impl
Imports TUGraz.VectoCore.InputData.Reader.Impl
Imports TUGraz.VectoCore.Models.Declaration
Imports TUGraz.VectoCore.Models.Simulation.Data
Imports TUGraz.VectoCore.Utils

<CustomValidation(GetType(VectoJob), "ValidateJob")>
Public Class VectoJob
    Implements IEngineeringInputDataProvider, IDeclarationInputDataProvider, IEngineeringJobInputData,
                IDeclarationJobInputData, IDriverEngineeringInputData, IDriverDeclarationInputData, IAuxiliariesEngineeringInputData, IAuxiliaryEngineeringInputData,
                IAuxiliariesDeclarationInputData, IJSONVehicleComponents, IEngineStopStartEngineeringInputData, IEcoRollEngineeringInputData, IPCCEngineeringInputData


    
    Private _sFilePath As String
    Private _myPath As String

    'Input parameters
    Private ReadOnly _vehicleFile As SubPath
    Private ReadOnly _engineFile As SubPath
    Private ReadOnly _gearboxFile As SubPath
    Private ReadOnly _tcuFile As SubPath
    Private ReadOnly _hcuFile As SubPath
    Private ReadOnly _busAuxFile As SubPath

    Private ReadOnly _lacDfTargetSpeedFile As SubPath
    Private ReadOnly _lacDfVelocityDropFile as SubPath
    Private ReadOnly _ptoCycleWhileDriveFile as SubPath

    Private _startStop As Boolean
    Public StartStopDelay As Double

    public UseBusAux as Boolean

    Private ReadOnly _driverAccelerationFile As SubPath

    'Alle Nebenverbraucher die in der Veh-Datei UND im Zyklus definiert sind

    Public ReadOnly CycleFiles As List(Of SubPath)

    'Public EngineOnly As Boolean

    Public VMin As Double
    Public LookAheadOn As Boolean
    Public OverSpeedOn As Boolean
    Public OverSpeed As Double
   

    Public LookAheadMinSpeed As Double
    Public EngineStopStartActivationThreshold As Double
    public EngineOffTimeLimit As double
    public EngineStStUtilityFactor As Double
    public EngineStStUtilityFactorDriving as Double

    Public EcoRollMinSpeed As double
    Public EcoRollUnderspeedThreshold As Double
    Public EcoRollActivationDelay as double
    public EcoRollMaxAcceleration as Double

    Public PCCEnableSpeedVal As Double
    Public PCCMinSpeed As Double
    Public PCCPrevewiDistance1 As Double
    Public PCCPreviewDistance2 As Double
    Public PCCUnderspeed As Double
    Public PCCOverspeedUseCase3 As Double
    Private _accelerationUpperLimit As MeterPerSquareSecond
    Public AuxElPadd As Double
    Public AuxPwrDrivingICEOff As Double
    Public AuxPwrStandstillICEOff As Double

    Public AuxEntries As Dictionary(Of String, AuxEntry )

    'Private _vehicleInputData As JSONComponentInputData
    'Private _engineInputData As JSONComponentInputData
    'Private _gearboxInputData As JSONComponentInputData

    Public Class AuxEntry
        Public Type As AuxiliaryType
        Public ReadOnly Path As SubPath
        Public ReadOnly TechnologyList As List(Of String)

        Public Sub New()
            Path = New SubPath
            TechnologyList = New List(Of String)()
        End Sub
    End Class

    Public Sub New()

        _myPath = ""
        _sFilePath = ""

        _vehicleFile = New SubPath
        _engineFile = New SubPath
        _gearboxFile = New SubPath
        _tcuFile = New SubPath
        _hcuFile = New SubPath()
        _busAuxFile = new SubPath()
        _lacDfTargetSpeedFile = New SubPath()
        _lacDfVelocityDropFile = New SubPath()
        _ptoCycleWhileDriveFile = new SubPath()

        _driverAccelerationFile = New SubPath

        CycleFiles = New List(Of SubPath)

        AuxEntries = new Dictionary(Of String,AuxEntry)
    End Sub

    Public Function SaveFile() As Boolean
        dim emPos As PowertrainPosition? = Nothing
        if (IEngineeringJobInputData_Vehicle?.VehicleType <> VectoSimulationJobType.ConventionalVehicle) then
            if (IEngineeringJobInputData_Vehicle.VehicleType <> VectoSimulationJobType.IEPC_E) Then
                emPos =  IEngineeringJobInputData_Vehicle?.Components.ElectricMachines?.Entries.FirstOrDefault()?.Position
            End If
        end if

        Dim validationResults As IList(Of ValidationResult) =
                Validate(If(Cfg.DeclMode, ExecutionMode.Declaration, ExecutionMode.Engineering), JobType, emPos, Nothing, False)

        If validationResults.Count > 0 Then
            Dim messages As IEnumerable(Of String) =
                    validationResults.Select(Function(r) r.ErrorMessage + String.Join(", ", r.MemberNames.Distinct()))
            MsgBox("Invalid input." + Environment.NewLine + String.Join(Environment.NewLine, messages), MsgBoxStyle.OkOnly,
                    "Failed to save Vecto Job")
            Return False
        End If

        Try
            Dim writer As JSONFileWriter = JSONFileWriter.Instance
            writer.SaveJob(Me, _sFilePath, cfg.DeclMode)
        Catch ex As Exception
            MsgBox("Failed to save Job file: " + ex.Message)
            Return False
        End Try
        Return True
    End Function

    'This Sub reads those Input-files that do not have their own class, etc.


#Region "Properties"


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

    Public Property PathEng(Optional ByVal original As Boolean = False) As String
        Get
            If original Then
                Return _engineFile.OriginalPath
            Else
                Return _engineFile.FullPath
            End If
        End Get
        Set(value As String)
            _engineFile.Init(_myPath, value)
        End Set
    End Property

    Public Property PathGbx(Optional ByVal original As Boolean = False) As String
        Get
            If original Then
                Return _gearboxFile.OriginalPath
            Else
                Return _gearboxFile.FullPath
            End If
        End Get
        Set(value As String)
            _gearboxFile.Init(_myPath, value)
        End Set
    End Property

    Public Property PathBusAux(Optional ByVal original As Boolean = False) As String
        Get
            If original Then
                Return _busAuxFile.OriginalPath
            Else
                Return _busAuxFile.FullPath
            End If
        End Get
        Set(value As String)
            _busAuxFile.Init(_myPath, value)
        End Set
    End Property

    Public Property PathShiftParams(Optional ByVal original As Boolean = False) As String
        Get
            If original Then
                Return _tcuFile.OriginalPath
            Else
                Return _tcuFile.FullPath
            End If
        End Get
        Set(value As String)
            _tcuFile.Init(_myPath, value)
        End Set
    End Property

    Public Property PathHybridStrategyParams(Optional ByVal original As Boolean = False) As String
        Get
            If original Then
                Return _hcuFile.OriginalPath
            Else
                Return _hcuFile.FullPath
            End If
        End Get
        Set(value As String)
            _hcuFile.Init(_myPath, value)
        End Set
    End Property


    Public ReadOnly Property IDriverDeclarationInputData_SavedInDeclarationMode As Boolean _
        Implements IDriverDeclarationInputData.SavedInDeclarationMode
        Get
            Return Cfg.DeclMode
        End Get
    End Property

    'Public Property StartStop As Boolean
    '	Get
    '		Return _startStop
    '	End Get
    '	Set(value As Boolean)
    '		_startStop = value
    '	End Set
    'End Property

    Public ReadOnly Property OverSpeedData As IOverSpeedEngineeringInputData _
        Implements IDriverEngineeringInputData.OverSpeedData
        Get
           Return New OverSpeedInputData() With {
                .Enabled = OverSpeedOn,
                .MinSpeed = VMin.KMPHtoMeterPerSecond(),
                .OverSpeed = OverSpeed.KMPHtoMeterPerSecond()
                }
        End Get
    End Property


    Public ReadOnly Property AccelerationCurve As IDriverAccelerationData Implements IDriverEngineeringInputData.AccelerationCurve
        Get
            If String.IsNullOrWhiteSpace(_driverAccelerationFile.FullPath) Then Return Nothing
            If Not File.Exists(_driverAccelerationFile.FullPath) Then
                Try
                    Dim cycleDataRes As Stream =
                            RessourceHelper.ReadStream(
                                DeclarationData.DeclarationDataResourcePrefix + ".VACC." + _driverAccelerationFile.OriginalPath +
                                VectoCore.Configuration.Constants.FileExtensions.DriverAccelerationCurve)
                    Return  New DriverAccelerationInputData() With{ .AccelerationCurve =
                        VectoCSVFile.ReadStream(cycleDataRes,
                                                source:=DeclarationData.DeclarationDataResourcePrefix + ".VACC." + _driverAccelerationFile.OriginalPath +
                                                        VectoCore.Configuration.Constants.FileExtensions.DriverAccelerationCurve)
                    }
                Catch ex As Exception
                    Return Nothing
                End Try
            End If
            Return New DriverAccelerationInputData() With{ .AccelerationCurve = VectoCSVFile.Read(_driverAccelerationFile.FullPath) }
        End Get
    End Property

    Public ReadOnly Property Lookahead As ILookaheadCoastingInputData Implements IDriverEngineeringInputData.Lookahead
        Get
            Dim lacTargetLookup As TableData =
                    If(File.Exists(LacDfTargetSpeedFile), VectoCSVFile.Read(LacDfTargetSpeedFile), Nothing)
            Dim lacVdropLookup As TableData =
                    If(File.Exists(LacDfVelocityDropFile), VectoCSVFile.Read(LacDfVelocityDropFile), Nothing)
            Return New LookAheadCoastingInputData With {
                .Enabled = LookAheadOn,
                .MinSpeed = LookAheadMinSpeed.KMPHtoMeterPerSecond(),
                .CoastingDecisionFactorScaling = LacDfScale,
                .CoastingDecisionFactorOffset = LacDfOffset,
                .LookaheadDistanceFactor = LacPreviewFactor,
                .CoastingDecisionFactorTargetSpeedLookup = lacTargetLookup,
                .CoastingDecisionFactorVelocityDropLookup = lacVdropLookup
                }
        End Get
    End Property

    Public ReadOnly Property IDriverEngineeringInputData_GearshiftInputData As IGearshiftEngineeringInputData Implements IDriverEngineeringInputData.GearshiftInputData
        Get
            return new JSONComponentInputData(_tcuFile.FullPath, Me).DriverInputData.GearshiftInputData
        End Get
    End Property

    Public ReadOnly Property EngineStopStartData As IEngineStopStartEngineeringInputData Implements IDriverEngineeringInputData.EngineStopStartData
        Get
            Return me
        End Get
    End Property

    Public ReadOnly Property EcoRollData As IEcoRollEngineeringInputData Implements IDriverEngineeringInputData.EcoRollData
    Get
            Return me
    End Get
    End Property

    Public ReadOnly Property PCCData As IPCCEngineeringInputData Implements IDriverEngineeringInputData.PCCData
    get
            return me
    End Get
    End Property

    Public ReadOnly Property PCCEnabledSpeed As MeterPerSecond Implements IPCCEngineeringInputData.PCCEnabledSpeed
    get
            return PCCEnableSpeedVal.KMPHtoMeterPerSecond()
    End Get
    End Property
    Public ReadOnly Property IPCCEngineeringInputData_MinSpeed As MeterPerSecond Implements IPCCEngineeringInputData.MinSpeed
    get
            return PCCMinSpeed.KMPHtoMeterPerSecond()
    End Get
    End Property

    Public ReadOnly Property MinSpeed As MeterPerSecond Implements IEcoRollEngineeringInputData.MinSpeed
    get
            Return EcoRollMinSpeed.KMPHtoMeterPerSecond()
    End Get
    End Property

    Public ReadOnly Property PreviewDistanceUseCase1 As Meter Implements IPCCEngineeringInputData.PreviewDistanceUseCase1
    get
            return PCCPrevewiDistance1.SI(of Meter)
    End Get
    End Property
    Public ReadOnly Property PreviewDistanceUseCase2 As Meter Implements IPCCEngineeringInputData.PreviewDistanceUseCase2
    get
            return PCCPreviewDistance2.SI(of Meter)
    End Get
    End Property
    Public ReadOnly Property Underspeed As MeterPerSecond Implements IPCCEngineeringInputData.Underspeed
    get
            return PCCUnderspeed.KMPHtoMeterPerSecond()
    End Get
    End Property
    Public ReadOnly Property OverspeedUseCase3 As MeterPerSecond Implements IPCCEngineeringInputData.OverspeedUseCase3
    get
            Return PCCOverspeedUseCase3.KMPHtoMeterPerSecond()
    End Get
    End Property

    Public ReadOnly Property IEcoRollEngineeringInputData_ActivationDelay As Second Implements IEcoRollEngineeringInputData.ActivationDelay
    get
            Return EcoRollActivationDelay.SI(Of Second)()
    End Get
    End Property

    Public ReadOnly Property ActivationDelay As Second Implements IEngineStopStartEngineeringInputData.ActivationDelay
        Get
            return EngineStopStartActivationThreshold.SI(Of Second)()
        End Get
    End Property

    Public ReadOnly Property UnderspeedThreshold As MeterPerSecond Implements IEcoRollEngineeringInputData.UnderspeedThreshold
    get
            Return EcoRollUnderspeedThreshold.KMPHtoMeterPerSecond()
    End Get
    End Property

    Public ReadOnly Property AccelerationUpperLimit As MeterPerSquareSecond Implements IEcoRollEngineeringInputData.AccelerationUpperLimit
        Get
            Return EcoRollMaxAcceleration.SI(of MeterPerSquareSecond)
        End Get
    End Property

    Public ReadOnly Property MaxEngineOffTimespan As Second Implements IEngineStopStartEngineeringInputData.MaxEngineOffTimespan
        Get
            Return EngineOffTimeLimit.SI(Of Second)()
        End Get
    End Property

    Public ReadOnly Property UtilityFactorStandstill As Double Implements IEngineStopStartEngineeringInputData.UtilityFactorStandstill
        Get
            Return EngineStStUtilityFactor
        End Get
    End Property

    Public ReadOnly Property UtilityFactorDriving As Double Implements IEngineStopStartEngineeringInputData.UtilityFactorDriving
    get
        Return EngineStStUtilityFactorDriving
    End Get
    End Property

    Public Property DesMaxFile(Optional ByVal original As Boolean = False) As String
        Get
            If original Then
                Return _driverAccelerationFile.OriginalPath
            Else
                Return _driverAccelerationFile.FullPath
            End If
        End Get
        Set(value As String)
            _driverAccelerationFile.Init(_myPath, value)
        End Set
    End Property

    Public Property LacPreviewFactor As Double
    Public Property LacDfOffset As Double
    Public Property LacDfScale As Double
    Public Property LacDfTargetSpeedFile(Optional ByVal original As Boolean = false) As String
        Get
            If original Then
                Return _lacDfTargetSpeedFile.OriginalPath
                Else 
                return _lacDfTargetSpeedFile.FullPath
            End If
        End Get
        Set(value As String)
            _lacDfTargetSpeedFile.Init(_myPath, value)
        End Set
    End Property
    Public Property LacDfVelocityDropFile(optional ByVal original As Boolean = false) As String
        Get
            If original Then
                Return _lacDfVelocityDropFile.OriginalPath
            Else 
                return _lacDfVelocityDropFile.FullPath
            End If
        End Get
        Set(value As String)
            _lacDfVelocityDropFile.Init(_myPath, value)
        End Set
    End Property


#End Region


    ' ReSharper disable once UnusedMember.Global -- used by Validation
    Public Shared Function ValidateJob(vectoJob As VectoJob, validationContext As ValidationContext) As ValidationResult
        Dim modeService As VectoValidationModeServiceContainer =
                TryCast(validationContext.GetService(GetType(VectoValidationModeServiceContainer)),
                        VectoValidationModeServiceContainer)
        Dim mode As ExecutionMode = If(modeService Is Nothing, ExecutionMode.Declaration, modeService.Mode)

        If mode = ExecutionMode.Engineering AndAlso vectoJob.JobType = VectoSimulationJobType.EngineOnlySimulation Then
            Return ValidateEngineOnlyJob(vectoJob, mode)
        End If

        Return ValidateVehicleJob(vectoJob, mode)
    End Function

    Private Shared Function ValidateEngineOnlyJob(vectoJob As VectoJob, executionMode As ExecutionMode) As ValidationResult
        Dim result As IList(Of ValidationResult) = New List(Of ValidationResult)

        'vectoJob._engineInputData = New JSONComponentInputData(vectoJob._engineFile.FullPath)

        If vectoJob.JobInputData.EngineOnly Is Nothing Then _
            result.Add(New ValidationResult("Engine File is missing or invalid"))
        If result.Any() Then
            Return _
                New ValidationResult("Vecto Job Configuration is invalid. ", result.Select(Function(r) r.ErrorMessage).ToList())
        End If

        Return ValidationResult.Success
    End Function

    Private Shared Function ValidateVehicleJob(vectoJob As VectoJob, mode As ExecutionMode) As ValidationResult

        Dim jobData As VectoRunData

        Dim result As IList(Of ValidationResult) = New List(Of ValidationResult)

        Dim vehicleInputData As IVehicleEngineeringInputData = vectoJob.JobInputData.Vehicle
        If vehicleInputData Is Nothing Then
            result.Add(New ValidationResult("Vehicle File is missing or invalid"))
            Return New ValidationResult("Vecto Job Configuration is invalid. ", result.Select(Function(r) r.ErrorMessage).ToList())
        End If

        Dim engineInputData As IEngineDeclarationInputData = vectoJob.JobInputData.Vehicle.Components.EngineInputData
        Dim gearboxInputData As IGearboxDeclarationInputData = vectoJob.Vehicle.Components.GearboxInputData
        Dim gearshiftInputData As IGearshiftEngineeringInputData = vectoJob.DriverInputData.GearshiftInputData

        If (vehicleInputData.VehicleType <> vectoJob.JobType) Then
            result.Add(New ValidationResult($"Vehicle type ""{vehicleInputData.VehicleType}"" differs from job type ""{vectoJob.JobType}""."))
        End If
        If vectoJob.JobType.IsOneOf(VectoSimulationJobType.BatteryElectricVehicle, VectoSimulationJobType.ParallelHybridVehicle, VectoSimulationJobType.SerialHybridVehicle) _
           AndAlso (vehicleInputData.Components.ElectricMachines Is Nothing OrElse vehicleInputData.Components.ElectricMachines.Entries.Count = 0) Then _
            result.Add(New ValidationResult("Electric machine is missing in vehicle"))
        If Not (vectoJob.JobType = VectoSimulationJobType.BatteryElectricVehicle OrElse vectoJob.JobType = VectoSimulationJobType.IEPC_E) AndAlso engineInputData Is Nothing Then _
            result.Add(New ValidationResult("Engine File is missing or invalid"))
        If (vectoJob.JobType = VectoSimulationJobType.ConventionalVehicle OrElse vectoJob.JobType = VectoSimulationJobType.ParallelHybridVehicle) _
             AndAlso gearboxInputData Is Nothing Then _
            result.Add(New ValidationResult("Gearbox File is missing or invalid"))
        If (mode = ExecutionMode.Engineering andalso vectoJob.JobType = VectoSimulationJobType.ConventionalVehicle OrElse vectoJob.JobType = VectoSimulationJobType.ParallelHybridVehicle) _
             AndAlso gearshiftInputData Is Nothing Then _
            result.Add(New ValidationResult("Gearshift File is missing or invalid"))

        If result.Any() Then
            Return _
                New ValidationResult("Vecto Job Configuration is invalid. ", result.Select(Function(r) r.ErrorMessage).ToList())
        End If
        Try
            If mode = ExecutionMode.Declaration Then
                If Not vehicleInputData.SavedInDeclarationMode Then
                    result.Add(New ValidationResult("Vehicle File is not in Declaration Mode"))
                End If
                If Not engineInputData.SavedInDeclarationMode Then
                    result.Add(New ValidationResult("Engine File is not in Declaration Mode"))
                End If
                If Not gearboxInputData.SavedInDeclarationMode Then
                    result.Add(New ValidationResult("Gearbox File is not in Declaration Mode"))
                End If
                If result.Any() Then
                    Return _
                        New ValidationResult("Vecto Job Configuration is invalid. ", result.Select(Function(r) r.ErrorMessage).ToList())
                End If

                Dim dataFactory As DeclarationModeTruckVectoRunDataFactory = New DeclarationModeTruckVectoRunDataFactory(vectoJob, Nothing)

                jobData = dataFactory.NextRun().First()
            Else
                If vehicleInputData.SavedInDeclarationMode Then
                    result.Add(New ValidationResult("Vehicle File is not in Engineering Mode"))
                End If
                If Not (vectoJob.JobType = VectoSimulationJobType.BatteryElectricVehicle OrElse vectoJob.JobType = VectoSimulationJobType.IEPC_E) AndAlso engineInputData.SavedInDeclarationMode Then
                    result.Add(New ValidationResult("Engine File is not in Engineering Mode"))
                End If
                If Not vectoJob.JobType = VectoSimulationJobType.BatteryElectricVehicle _ 
                    AndAlso gearboxInputData IsNot Nothing AndAlso gearboxInputData.SavedInDeclarationMode Then
                    result.Add(New ValidationResult("Gearbox File is not in Engineering Mode"))
                End If
                If vectoJob.CycleFiles.Count = 0 Then
                    result.Add(New ValidationResult("At least one cycle must be defined."))
                End If
                If result.Any() Then
                    Return _
                        New ValidationResult("Vecto Job Configuration is invalid. ", result.Select(Function(r) r.ErrorMessage).ToList())
                End If
                Dim dataFactory As EngineeringModeVectoRunDataFactory = New EngineeringModeVectoRunDataFactory(vectoJob)
                jobData = dataFactory.NextRun().FirstOrDefault()
                If jobData Is Nothing Then
                    Return New ValidationResult("No cycles selected in Vecto Job.", result.Select(Function(r) r.ErrorMessage).ToList())
                End If
            End If

            dim emPos As PowertrainPosition? = Nothing
            if (vehicleInputData?.VehicleType <> VectoSimulationJobType.ConventionalVehicle) then
                if (vehicleInputData.VehicleType <> VectoSimulationJobType.IEPC_E) Then
                    emPos =  vehicleInputData?.Components.ElectricMachines?.Entries.FirstOrDefault()?.Position
                End If
            end if
            result = jobData.Validate(If(Cfg.DeclMode, ExecutionMode.Declaration, ExecutionMode.Engineering), vehicleInputData.VehicleType, empos, If(jobData.GearboxData?.Type, GearboxType.NoGearbox), False)
            If result.Any() Then
                Return _
                    New ValidationResult("Vecto Job Configuration is invalid. ", result.Select(Function(r) r.ErrorMessage).ToList())
            End If


            Return ValidationResult.Success

        Catch ex As Exception
            Return New ValidationResult(ex.Message)
            'Finally
            '	vectoJob._vehicleInputData = Nothing
            '	vectoJob._engineInputData = Nothing
            '	vectoJob._gearboxInputData = Nothing
        End Try
    End Function


#Region "IInputData"

    Public ReadOnly Property JobInputData As IEngineeringJobInputData Implements IEngineeringInputDataProvider.JobInputData
        Get
            Return Me
        End Get
    End Property

    Public ReadOnly Property PrimaryVehicleData As IPrimaryVehicleInformationInputDataProvider Implements IDeclarationInputDataProvider.PrimaryVehicleData

    Public ReadOnly Property IDeclarationInputDataProvider_JobInputData As IDeclarationJobInputData _
        Implements IDeclarationInputDataProvider.JobInputData
        Get
            Return Me
        End Get
    End Property

    Public ReadOnly Property DriverInputData As IDriverEngineeringInputData _
        Implements IEngineeringInputDataProvider.DriverInputData
        Get
            Return Me
        End Get
    End Property

    'Public ReadOnly Property GearshiftInputData As IGearshiftEngineeringInputData Implements IDriverEngineeringInputData.GearshiftInputData
    '    get
    '        Return TryCast( New JSONComponentInputData(_gearboxFile.FullPath, Me).JobInputData.Vehicle.Components.GearboxInputData, IGearshiftEngineeringInputData)
    '    End Get
    'End Property


    Public ReadOnly Property XMLHash As XElement Implements IDeclarationInputDataProvider.XMLHash
        Get
            Return Nothing
        End Get
    End Property


    Public ReadOnly Property SavedInDeclarationMode As Boolean Implements IDeclarationJobInputData.SavedInDeclarationMode
        Get
            Return Cfg.DeclMode
        End Get
    End Property


    Public ReadOnly Property IEngineeringJobInputData_Vehicle As IVehicleEngineeringInputData _
        Implements IEngineeringJobInputData.Vehicle
        Get
            If Not File.Exists(_vehicleFile.FullPath) Then Return Nothing
            Return New JSONComponentInputData(_vehicleFile.FullPath, Me).JobInputData.Vehicle
        End Get
    End Property

    Public ReadOnly Property Vehicle As IVehicleDeclarationInputData Implements IDeclarationJobInputData.Vehicle
        Get
            If Not File.Exists(_vehicleFile.FullPath) Then Return Nothing
            Return New JSONComponentInputData(_vehicleFile.FullPath, Me).JobInputData.Vehicle
        End Get
    End Property

    Public ReadOnly Property HybridStrategyParameters As IHybridStrategyParameters Implements IEngineeringJobInputData.HybridStrategyParameters
        Get
            If Not File.Exists(_hcuFile.FullPath) Then Return Nothing
            Return New JSONComponentInputData(_hcuFile.FullPath, Me).JobInputData.HybridStrategyParameters
        End Get
    End Property

    Public ReadOnly Property Cycles As IList(Of ICycleData) Implements IEngineeringJobInputData.Cycles
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
                                                    cycleFile.OriginalPath + TUGraz.VectoCore.Configuration.Constants.FileExtensions.CycleFile
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

    Public Property JobType As VectoSimulationJobType Implements IEngineeringJobInputData.JobType


 
    Public ReadOnly Property IEngineeringJobInputData_EngineOnly As IEngineEngineeringInputData Implements IEngineeringJobInputData.EngineOnly
        Get
            If Not File.Exists(_engineFile.FullPath) Then Return Nothing
            Return New JSONComponentInputData(_engineFile.FullPath, Me).JobInputData.Vehicle.Components.EngineInputData
        End Get
    End Property

    Public ReadOnly Property JobName As String Implements IDeclarationJobInputData.JobName
        Get
            Return Path.GetFileNameWithoutExtension(FilePath)
        End Get
    End Property

    
    Public Property AuxPwrICEOn As Double

    Public ReadOnly Property IAuxiliariesDeclarationInputData_SavedInDeclarationMode As Boolean _
        Implements IAuxiliariesDeclarationInputData.SavedInDeclarationMode
        Get
            Return Cfg.DeclMode
        End Get
    End Property

    Public ReadOnly Property Auxiliaries As IAuxiliaryEngineeringInputData _
        Implements IAuxiliariesEngineeringInputData.Auxiliaries
        Get
            Return  Me 'AuxData().Cast(Of IAuxiliariesEngineeringInputData).ToList()
        End Get
    End Property

    Public ReadOnly Property BusAuxiliariesData As IBusAuxiliariesEngineeringData Implements IAuxiliariesEngineeringInputData.BusAuxiliariesData
    get
        If (not UseBusAux) Then
            Return Nothing
        End If
        Return New JSONComponentInputData(_busAuxFile.FullPath, Me).JobInputData.Vehicle.Components.AuxiliaryInputData.BusAuxiliariesData
    End Get
    End Property


    Public ReadOnly Property IAuxiliariesDeclarationInputData_Auxiliaries As IList(Of IAuxiliaryDeclarationInputData) _
        Implements IAuxiliariesDeclarationInputData.Auxiliaries
        Get
            Return AuxData().Cast(Of IAuxiliaryDeclarationInputData).ToList()
        End Get
    End Property

    Protected Function AuxData() As IList(Of DeclarationAuxiliaryDataInputData)

        Dim retVal As List(Of DeclarationAuxiliaryDataInputData) = New List(Of DeclarationAuxiliaryDataInputData)
        For Each entry As KeyValuePair(Of string, AuxEntry) In AuxEntries
            retVal.Add(New DeclarationAuxiliaryDataInputData() With{ .ID = entry.Key, .Technology = entry.Value.TechnologyList, .Type = AuxiliaryTypeHelper.ParseKey(entry.Key)})
        Next
        Return retVal
    End Function

#End Region

    Public ReadOnly Property Gearbox As IGearboxEngineeringInputData Implements IJSONVehicleComponents.Gearbox
        Get
            Return New JSONComponentInputData(_gearboxFile.FullPath, Me).JobInputData.Vehicle.Components.GearboxInputData
        End Get
    End Property

    Public ReadOnly Property TorqueConverter As ITorqueConverterEngineeringInputData Implements IJSONVehicleComponents.TorqueConverter
        Get
            Return New JSONComponentInputData(_gearboxFile.FullPath, Me).JobInputData.Vehicle.Components.TorqueConverterInputData
        End Get

    End Property

    Public ReadOnly Property AxleGear As IAxleGearInputData Implements IJSONVehicleComponents.AxleGear
        Get
            Return New JSONComponentInputData(_gearboxFile.FullPath, Me).JobInputData.Vehicle.Components.AxleGearInputData
        End Get

    End Property

    Public ReadOnly Property Engine As IEngineEngineeringInputData Implements IJSONVehicleComponents.Engine
        Get
            Return New JSONComponentInputData(_engineFile.FullPath, Me).JobInputData.Vehicle.Components.EngineInputData
        End Get
    End Property

    Public ReadOnly Property DeclarationAuxiliaries As IAuxiliariesDeclarationInputData Implements IJSONVehicleComponents.DeclarationAuxiliaries
        Get
            Return Me
        End Get
    End Property

    Public ReadOnly Property EngineeringAuxiliaries As IAuxiliariesEngineeringInputData Implements IJSONVehicleComponents.EngineeringAuxiliaries
        Get
            Return Me
        End Get
    End Property

    Public ReadOnly Property DataSource As DataSource Implements IInputDataProvider.DataSource
        Get
            Dim retVal As DataSource =  New DataSource() 
            retVal.SourceType = DataSourceType.JSONFile
            retVal.SourceFile = FilePath
            Return retVal
        End Get
    End Property

    Public ReadOnly Property ConstantPowerDemand As Watt Implements IAuxiliaryEngineeringInputData.ConstantPowerDemand
    get
        Return AuxPwrICEOn.SI(of Watt)
    End Get
    End Property
    Public ReadOnly Property PowerDemandICEOffDriving As Watt Implements IAuxiliaryEngineeringInputData.PowerDemandICEOffDriving
    get
        Return AuxPwrDrivingICEOff.SI(of Watt)
    End Get
    End Property
    Public ReadOnly Property PowerDemandICEOffStandstill As Watt Implements IAuxiliaryEngineeringInputData.PowerDemandICEOffStandstill
    get
        Return AuxPwrStandstillICEOff.SI(Of Watt)
    End Get
    End Property
    Public ReadOnly Property ElectricPowerDemand As Watt Implements IAuxiliaryEngineeringInputData.ElectricPowerDemand
    get
            Return AuxElPadd.SI(of Watt)
    End Get
    End Property


End Class


