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
Imports System.Collections.Generic
Imports System.ComponentModel.DataAnnotations
Imports System.IO
Imports System.Linq
Imports System.Xml
Imports TUGraz.VECTO.Input_Files
Imports TUGraz.VectoCommon.BusAuxiliaries
Imports TUGraz.VectoCommon.Exceptions
Imports TUGraz.VectoCommon.InputData
Imports TUGraz.VectoCommon.Models
Imports TUGraz.VectoCommon.Utils
Imports TUGraz.VectoCore.InputData.FileIO.JSON
Imports TUGraz.VectoCore.InputData.Impl
Imports TUGraz.VectoCore.InputData.Reader.ComponentData
Imports TUGraz.VectoCore.InputData.Reader.DataObjectAdapter
Imports TUGraz.VectoCore.InputData.Reader.DataObjectAdapter.SimulationComponents
Imports TUGraz.VectoCore.Models.Declaration
Imports TUGraz.VectoCore.Models.Simulation.Data
Imports TUGraz.VectoCore.Models.SimulationComponent.Data
Imports TUGraz.VectoCore.Models.SimulationComponent.Data.Engine
Imports TUGraz.VectoCore.Models.SimulationComponent.Impl
Imports TUGraz.VectoCore.OutputData.FileIO
Imports TUGraz.VectoCore.Utils
Imports DeclarationDataAdapterHeavyLorry = TUGraz.VectoCore.InputData.Reader.DataObjectAdapter.HeavyLorry.DeclarationDataAdapterHeavyLorry

<CustomValidation(GetType(Gearbox), "ValidateGearbox")>
Public Class Gearbox
    Implements IGearboxEngineeringInputData, IGearboxDeclarationInputData, IAxleGearInputData,
               ITorqueConverterEngineeringInputData, ITorqueConverterDeclarationInputData,
               IGearshiftEngineeringInputData

    Private _myPath As String
    Private _filePath As String

    Public ModelName As String
    Public GbxInertia As Double
    Public TracIntrSi As Double

    Public GearRatios As List(Of Double)
    Public GearLossmaps As List(Of SubPath)

    'Gear shift polygons
    Public GearshiftFiles As List(Of SubPath)

    Public MaxTorque As List(Of String)
    Public MaxSpeed As List(Of String)

    Public TorqueResv As Double
    'Public SkipGears As Boolean
    Public ShiftTime As Double
    Public TorqueResvStart As Double
    Public StartSpeed As Double
    Public StartAcc As Double
    'Public ShiftInside As Boolean

    Public Type As GearboxType

    'Torque Converter Input
    Public TorqueConverterReferenceRpm As Double
    Private ReadOnly _torqueConverterFile As New SubPath
    Public TorqueConverterInertia As Double
    Public TorqueConverterShiftPolygonFile As String
    Public TCLUpshiftMinAcceleration As Double
    Public TCCUpshiftMinAcceleration As Double

    Public UpshiftMinAcceleration As Double
    Public DownshiftAfterUpshift As Double
    Public UpshiftAfterDownshift As Double
    Public TorqueConverterMaxSpeed As Double

    Public PSShiftTime As Double


    Public Sub New()
        _myPath = ""
        _filePath = ""
        SetDefault()
    End Sub

    Private Sub SetDefault()

        ModelName = ""
        GbxInertia = 0
        TracIntrSi = 0

        GearRatios = New List(Of Double)
        GearLossmaps = New List(Of SubPath)
        GearshiftFiles = New List(Of SubPath)
        MaxTorque = New List(Of String)
        MaxSpeed = New List(Of String)

        TorqueResv = 0
        'SkipGears = False
        ShiftTime = 0
        TorqueResvStart = 0
        StartSpeed = 0
        StartAcc = 0
        'ShiftInside = False

        Type = GearboxType.MT

        TorqueConverterReferenceRpm = 0
        _torqueConverterFile.Clear()

        TorqueConverterInertia = 0
    End Sub

    Public Function SaveFile() As Boolean

        Dim validationResults As IList(Of ValidationResult) =
                Validate(If(Cfg.DeclMode, ExecutionMode.Declaration, ExecutionMode.Engineering), VectoSimulationJobType.ConventionalVehicle, Nothing, Type, False)

        If validationResults.Count > 0 Then
            Dim messages As IEnumerable(Of String) =
                    validationResults.Select(Function(r) r.ErrorMessage + String.Join(", ", r.MemberNames.Distinct()))
            MsgBox("Invalid input." + Environment.NewLine + String.Join("; ", messages), MsgBoxStyle.OkOnly,
                   "Failed to save gearbox")
            Return False
        End If

        Try
            Dim writer As JSONFileWriter = JSONFileWriter.Instance
            writer.SaveGearbox(Me, Me, Me, Me, _filePath, Cfg.DeclMode)
        Catch ex As Exception
            MsgBox("failed to write Gearbox file: " + ex.Message)
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

    Public Property GearLossMap(ByVal gearNr As Integer, Optional ByVal original As Boolean = False) As String
        Get
            If original Then
                Return GearLossmaps(gearNr).OriginalPath
            Else
                Return GearLossmaps(gearNr).FullPath
            End If
        End Get
        Set(ByVal value As String)
            GearLossmaps(gearNr).Init(_myPath, value)
        End Set
    End Property

    Public Property ShiftPolygonFile(ByVal gearNr As Integer, Optional ByVal original As Boolean = False) As String
        Get
            If original Then
                Return GearshiftFiles(gearNr).OriginalPath
            Else
                Return GearshiftFiles(gearNr).FullPath
            End If
        End Get
        Set(value As String)
            GearshiftFiles(gearNr).Init(_myPath, value)
        End Set
    End Property

    Public Property TorqueConverterFile(Optional ByVal original As Boolean = False) As String
        Get
            If original Then
                Return _torqueConverterFile.OriginalPath
            Else
                Return _torqueConverterFile.FullPath
            End If
        End Get
        Set(value As String)
            _torqueConverterFile.Init(_myPath, value)
        End Set
    End Property


    ' ReSharper disable once UnusedMember.Global -- used by Validation
    Public Shared Function ValidateGearbox(gearbox As Gearbox, validationContext As ValidationContext) _
        As ValidationResult
        Dim modeService As VectoValidationModeServiceContainer =
                TryCast(validationContext.GetService(GetType(VectoValidationModeServiceContainer)),
                        VectoValidationModeServiceContainer)
        Dim mode As ExecutionMode = If(modeService Is Nothing, ExecutionMode.Declaration, modeService.Mode)
        Dim emsCycle As Boolean = (modeService IsNot Nothing) AndAlso modeService.IsEMSCycle
        Dim jobType As VectoSimulationJobType = If(modeService Is Nothing, VectoSimulationJobType.ConventionalVehicle, modeService.JobType)
        Dim emPos As PowertrainPosition? = If(modeService Is Nothing, PowertrainPosition.HybridPositionNotSet, modeService.EMPowertrainPosition)


        Dim axlegearData As AxleGearData
        Dim gearboxData As GearboxData

        Try
            'Dim vectoJob As VectoJob = New VectoJob() With {.FilePath = VectoJobForm.VECTOfile}
            Dim vectoFile As String = VectoJobForm.VectoFile
            Dim inputData As IEngineeringInputDataProvider =
                    TryCast(JSONInputDataFactory.ReadComponentData(vectoFile),
                            IEngineeringInputDataProvider)
            'Dim vehicle As IVehicleEngineeringInputData = inputData.VehicleInputData
            Dim engine As CombustionEngineData
            Dim vehiclecategory As VehicleCategory
            Dim rdyn As Meter = 0.5.SI(Of Meter)()
            Try
                vehiclecategory = inputData.JobInputData.Vehicle.VehicleCategory
            Catch ex As Exception
                vehiclecategory = VehicleCategory.RigidTruck
            End Try
            If mode = ExecutionMode.Declaration Then
                Dim doa As DeclarationDataAdapterHeavyLorry = New DeclarationDataAdapterHeavyLorry()

                Try
                    engine = New CombustionEngineComponentDataAdapter().CreateEngineData(inputData.JobInputData.Vehicle,
                                                  inputData.JobInputData.Vehicle.Components.EngineInputData.EngineModes.
                                                     First(), New Mission() With {.MissionType = MissionType.LongHaul})
                Catch
                    engine = GetDefaultEngine(gearbox.Gears)
                End Try

                axlegearData = New AxleGearDataAdapter().CreateAxleGearData(gearbox)

                dim supportedGearboxTypes As GearboxType() = New GearboxType(){ GearboxType.AMT, GearboxType.ATSerial, GearboxType.ATPowerSplit, GearboxType.MT, GearboxType.APTN, GearboxType.IEPC, GearboxType.IHPC}
                'Select Case jobType
                '    Case VectoSimulationJobType.ConventionalVehicle:
                '        supportedGearboxTypes = DeclarationDataAdapterHeavyLorry.Conventional.SupportsGearboxTypes
                '        Case VectoSimulationJobType.ParallelHybridVehicle
                '            supportedGearboxTypes = DeclarationDataAdapterHeavyLorry.ParallelHybrid.SupportsGearboxTypes
                'End Select

                if gearbox.Type = GearboxType.IEPC Then
                    if (gearbox.Gears.Count > 0) then
                        throw new VectoSimulationException("No gears are allowed for IEPC gearbox.")
                    End If
                    gearboxData = new GearboxData() With{ .Inertia = 0.SI(of KilogramSquareMeter), .TractionInterruption = 0.SI(of Second)} 
                else
                    gearboxData = New GearboxDataAdapter(New TorqueConverterDataAdapter()).CreateGearboxData(
                        New MockVehicleInputData() _
                                                           With {
                                                           .Components =
                                                           New MockComponents() _
                                                           With {.GearboxInputData = gearbox,
                                                           .TorqueConverterInputData = gearbox}},
                        New VectoRunData() _
                                                           With {.AxleGearData = axlegearData, .EngineData = engine,
                                                           .VehicleData =
                                                           New VehicleData() _
                                                           With {.DynamicTyreRadius = rdyn,
                                                           .VehicleCategory = vehiclecategory}},
                        Nothing, supportedGearboxTypes)
                End If
            Else

                Dim doa As EngineeringDataAdapter = New EngineeringDataAdapter()
                Try
                    engine = doa.CreateEngineData(inputData.JobInputData.Vehicle,
                                                  inputData.JobInputData.Vehicle.Components.EngineInputData.EngineModes.
                                                     First())
                Catch
                    engine = GetDefaultEngine(gearbox.Gears)
                End Try

                axlegearData = doa.CreateAxleGearData(gearbox)
                gearboxData = doa.CreateGearboxData(New MockEngineeringInputData() With {
                                                       .DriverInputData =
                                                       New MockDriverInputData() With {.GearshiftInputData = gearbox },
                                                       .JobInputData =
                                                       New MockJobInputData() _
                                                       With { _
                                                       .IEngineeringJobInputData_Vehicle =
                                                       New MockEngineeringVehicle() _
                                                       With { .GearboxInputData = gearbox,
                                                       .TorqueConverterInputData = gearbox,
                                                       .VehicleType = jobType }
                                                       }},
                                                    New VectoRunData() _
                                                       With {.AxleGearData = axlegearData, .EngineData = engine,
                                                       .VehicleData =
                                                       New VehicleData() _
                                                       With { .DynamicTyreRadius = rdyn,
                                                       .VehicleCategory = vehiclecategory}})
                'gearbox, engine, gearbox, axlegearData.AxleGear.Ratio, rdyn,
                '                                vehiclecategory, gearbox, Nothing, Nothing)
            End If

            Dim result As IList(Of ValidationResult) =
                    gearboxData.Validate(If(Cfg.DeclMode, ExecutionMode.Declaration, ExecutionMode.Engineering), jobType, emPos, gearbox.Type, emsCycle)
            If result.Any() Then
                Return _
                    New ValidationResult("Gearbox Configuration is invalid. ",
                                         result.Select(
                                             Function(r) _
                                                          r.ErrorMessage +
                                                          String.Join(Environment.NewLine, r.MemberNames)).ToList())
            End If

            result = axlegearData.Validate(If(Cfg.DeclMode, ExecutionMode.Declaration, ExecutionMode.Engineering), jobType, emPos, gearbox.Type, emsCycle)
            If result.Any() Then
                Return _
                    New ValidationResult("Axlegear Configuration is invalid. ",
                                         result.Select(
                                             Function(r) _
                                                          r.ErrorMessage +
                                                          String.Join(Environment.NewLine, r.MemberNames)).ToList())
            End If

            Return ValidationResult.Success

        Catch ex As Exception
            Return New ValidationResult(ex.Message)
        End Try
    End Function

    Private Shared Function GetDefaultEngine(gears As IList(Of ITransmissionInputData)) As CombustionEngineData
        Dim fldData As MemoryStream = New MemoryStream()
        Dim writer As StreamWriter = New StreamWriter(fldData)
        writer.WriteLine("engine speed, full load torque, motoring torque")
        writer.WriteLine(" 500, 2000, -500")
        writer.WriteLine("2500, 2000, -500")
        writer.WriteLine("3000,    0, -500")
        writer.Flush()
        fldData.Seek(0, SeekOrigin.Begin)
        Dim retVal As CombustionEngineData = New CombustionEngineData() With {
                .IdleSpeed = 600.RPMtoRad()
                }

        Dim fldCurve As EngineFullLoadCurve = FullLoadCurveReader.Create(VectoCSVFile.ReadStream(fldData))
        Dim fullLoadCurves As Dictionary(Of UInteger, EngineFullLoadCurve) =
                New Dictionary(Of UInteger, EngineFullLoadCurve)()
        fullLoadCurves(0) = fldCurve
        fullLoadCurves(0).EngineData = retVal
        For i As Integer = 0 To gears.Count - 1
            fullLoadCurves(CType(i + 1, UInteger)) =
                AbstractSimulationDataAdapter.IntersectFullLoadCurves(fullLoadCurves(0),
                                                                      gears(i).MaxTorque)
        Next
        retVal.FullLoadCurves = fullLoadCurves
        Return retVal
    End Function


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
            ' Just for the interface. Value is not available in GUI yet.
            Return TUGraz.VectoCore.Configuration.Constants.NOT_AVAILABLE
        End Get
    End Property


    Public ReadOnly Property [Date] As DateTime Implements IComponentInputData.[Date]
        Get
            Return Now.ToUniversalTime()
        End Get
    End Property

    Public ReadOnly Property AppVersion As String Implements IComponentInputData.AppVersion
        get
            Return "VECTO-GUI"
        End Get
    End Property

    Public ReadOnly Property CertificationMethod As CertificationMethod _
        Implements IComponentInputData.CertificationMethod
        Get
            Return CertificationMethod.NotCertified
        End Get
    End Property

    Public ReadOnly Property CertificationNumber As String Implements IComponentInputData.CertificationNumber
        Get
            ' Just for the interface. Value is not available in GUI yet.
            Return TUGraz.VectoCore.Configuration.Constants.NOT_AVAILABLE
        End Get
    End Property

    Public ReadOnly Property DigestValue As DigestData Implements IComponentInputData.DigestValue
        Get
            Return Nothing
        End Get
    End Property

    Public ReadOnly Property Model As String Implements IComponentInputData.Model
        Get
            Return ModelName
        End Get
    End Property

    Public ReadOnly Property IGearboxDeclarationInputData_Type As GearboxType _
        Implements IGearboxDeclarationInputData.Type
        Get
            Return Type
        End Get
    End Property

    Public ReadOnly Property Gears As IList(Of ITransmissionInputData) Implements IGearboxDeclarationInputData.Gears
        Get
            Dim ls As IList(Of ITransmissionInputData) = New List(Of ITransmissionInputData)
            Dim i As Integer
            For i = 1 To GearRatios.Count - 1
                Dim gearDict As New TransmissionInputData With {
                        .Ratio = GearRatios(i),
                        .Gear = i
                        }
                If File.Exists(GearshiftFiles(i).FullPath) Then
                    gearDict.ShiftPolygon = VectoCSVFile.Read(GearshiftFiles(i).FullPath)
                End If
                If Not String.IsNullOrWhiteSpace(MaxTorque(i)) AndAlso IsNumeric(MaxTorque(i)) Then
                    gearDict.MaxTorque = MaxTorque(i).ToDouble().SI (Of NewtonMeter)()
                End If
                If Not String.IsNullOrWhiteSpace(MaxSpeed(i)) AndAlso IsNumeric(MaxSpeed(i)) Then
                    gearDict.MaxInputSpeed = MaxSpeed(i).ToDouble().RPMtoRad()
                End If
                If IsNumeric(GearLossMap(i, True)) Then
                    gearDict.Efficiency = GearLossMap(i, True).ToDouble()
                Else
                    gearDict.LossMap = VectoCSVFile.Read(GearLossmaps(i).FullPath)
                End If

                ls.Add(gearDict)
            Next
            Return ls
        End Get
    End Property

    Public ReadOnly Property DifferentialIncluded As Boolean Implements IGearboxDeclarationInputData.DifferentialIncluded
    Public ReadOnly Property AxlegearRatio As Double Implements IGearboxDeclarationInputData.AxlegearRatio

    Public ReadOnly Property ReferenceRPM As PerSecond Implements ITorqueConverterEngineeringInputData.ReferenceRPM
        Get
            Return TorqueConverterReferenceRpm.RPMtoRad()
        End Get
    End Property

    Public ReadOnly Property ITorqueConverterEngineeringInputData_Inertia As KilogramSquareMeter _
        Implements ITorqueConverterEngineeringInputData.Inertia
        Get
            Return TorqueConverterInertia.SI (Of KilogramSquareMeter)()
        End Get
    End Property

    Public ReadOnly Property Inertia As KilogramSquareMeter Implements IGearboxEngineeringInputData.Inertia
        Get
            Return GbxInertia.SI (Of KilogramSquareMeter)()
        End Get
    End Property

    Public ReadOnly Property ShiftPolygon As TableData Implements ITorqueConverterEngineeringInputData.ShiftPolygon
        Get
            If Not File.Exists(Path.Combine(_myPath, TorqueConverterShiftPolygonFile)) Then Return Nothing
            Return VectoCSVFile.Read(Path.Combine(_myPath, TorqueConverterShiftPolygonFile))
        End Get
    End Property

    Public ReadOnly Property MaxInputSpeed As PerSecond Implements ITorqueConverterEngineeringInputData.MaxInputSpeed
        Get
            Return TorqueConverterMaxSpeed.RPMtoRad()
        End Get
    End Property

    Public ReadOnly Property CLUpshiftMinAcceleration As MeterPerSquareSecond _
        Implements IGearshiftEngineeringInputData.CLUpshiftMinAcceleration
        Get
            Return TCLUpshiftMinAcceleration.SI (Of MeterPerSquareSecond)()
        End Get
    End Property

    Public ReadOnly Property CCUpshiftMinAcceleration As MeterPerSquareSecond _
        Implements IGearshiftEngineeringInputData.CCUpshiftMinAcceleration
        Get
            Return TCCUpshiftMinAcceleration.SI (Of MeterPerSquareSecond)()
        End Get
    End Property


    Public ReadOnly Property TractionInterruption As Second Implements IGearboxEngineeringInputData.TractionInterruption
        Get
            Return TracIntrSi.SI (Of Second)()
        End Get
    End Property


    Public ReadOnly Property TorqueReserve As Double Implements IGearshiftEngineeringInputData.TorqueReserve
        Get
            Return TorqueResv/100
        End Get
    End Property

    Public ReadOnly Property StartAcceleration As MeterPerSquareSecond _
        Implements IGearshiftEngineeringInputData.StartAcceleration
        Get
            Return StartAcc.SI (Of MeterPerSquareSecond)()
        End Get
    End Property

    Public ReadOnly Property StartTorqueReserve As Double Implements IGearshiftEngineeringInputData.StartTorqueReserve
        Get
            Return TorqueResvStart/100
        End Get
    End Property

    Public ReadOnly Property DownshiftAferUpshiftDelay As Second _
        Implements IGearshiftEngineeringInputData.DownshiftAfterUpshiftDelay
        Get
            Return DownshiftAfterUpshift.SI (Of Second)()
        End Get
    End Property

    Public ReadOnly Property UpshiftAfterDownshiftDelay As Second _
        Implements IGearshiftEngineeringInputData.UpshiftAfterDownshiftDelay
        Get
            Return UpshiftAfterDownshift.SI (Of Second)()
        End Get
    End Property

    Public Overridable ReadOnly Property GearResidenceTime As Second _
        Implements IGearshiftEngineeringInputData.GearResidenceTime
        Get
            Return Nothing
        End Get
    End Property

    Public Overridable ReadOnly Property DnT99LHMin1 As Double? Implements IGearshiftEngineeringInputData.DnT99LHMin1
        Get
            Return Nothing
        End Get
    End Property

    Public Overridable ReadOnly Property DnT99LHMin2 As Double? Implements IGearshiftEngineeringInputData.DnT99LHMin2
        Get
            Return Nothing
        End Get
    End Property

    Public Overridable ReadOnly Property AllowedGearRangeUp As Integer? _
        Implements IGearshiftEngineeringInputData.AllowedGearRangeUp
        Get
            Return Nothing
        End Get
    End Property

    Public Overridable ReadOnly Property AllowedGearRangeDown As Integer? _
        Implements IGearshiftEngineeringInputData.AllowedGearRangeDown
        Get
            Return Nothing
        End Get
    End Property

    Public Overridable ReadOnly Property LookBackInterval As Second _
        Implements IGearshiftEngineeringInputData.LookBackInterval
        Get
            Return Nothing
        End Get
    End Property

    Public Overridable ReadOnly Property AvgCardanPowerThresholdPropulsion As Watt _
        Implements IGearshiftEngineeringInputData.AvgCardanPowerThresholdPropulsion
        Get
            Return Nothing
        End Get
    End Property

    Public Overridable ReadOnly Property CurrCardanPowerThresholdPropulsion As Watt _
        Implements IGearshiftEngineeringInputData.CurrCardanPowerThresholdPropulsion
        Get
            Return Nothing
        End Get
    End Property

    Public Overridable ReadOnly Property TargetSpeedDeviationFactor As Double? _
        Implements IGearshiftEngineeringInputData.TargetSpeedDeviationFactor
        Get
            Return Nothing
        End Get
    End Property

    Public Overridable ReadOnly Property EngineSpeedHighDriveOffFactor As Double? _
        Implements IGearshiftEngineeringInputData.EngineSpeedHighDriveOffFactor
        Get
            Return Nothing
        End Get
    End Property

    Public Overridable ReadOnly Property RatingFactorCurrentGear As Double? _
        Implements IGearshiftEngineeringInputData.RatingFactorCurrentGear
        Get
            Return Nothing
        End Get
    End Property

    Public Overridable ReadOnly Property AccelerationReserveLookup As TableData _
        Implements IGearshiftEngineeringInputData.AccelerationReserveLookup
        Get
            Return Nothing
        End Get
    End Property

    Public Overridable ReadOnly Property ShareTorque99L As TableData _
        Implements IGearshiftEngineeringInputData.ShareTorque99L
        Get
            Return Nothing
        End Get
    End Property

    Public Overridable ReadOnly Property PredictionDurationLookup As TableData _
        Implements IGearshiftEngineeringInputData.PredictionDurationLookup
        Get
            Return Nothing
        End Get
    End Property

    Public Overridable ReadOnly Property ShareIdleLow As TableData _
        Implements IGearshiftEngineeringInputData.ShareIdleLow
        Get
            Return Nothing
        End Get
    End Property

    Public Overridable ReadOnly Property ShareEngineHigh As TableData _
        Implements IGearshiftEngineeringInputData.ShareEngineHigh
        Get
            Return Nothing
        End Get
    End Property

    Public Overridable ReadOnly Property Source As String Implements IGearshiftEngineeringInputData.Source
        Get
            Return Nothing
        End Get
    End Property

    Public Overridable ReadOnly Property DriverAccelerationLookBackInterval As Second _
        Implements IGearshiftEngineeringInputData.DriverAccelerationLookBackInterval
        Get
            Return Nothing
        End Get
    End Property

    Public Overridable ReadOnly Property DriverAccelerationThresholdLow As MeterPerSquareSecond _
        Implements IGearshiftEngineeringInputData.DriverAccelerationThresholdLow
        Get
            Return Nothing
        End Get
    End Property

    Public Overridable ReadOnly Property RatioEarlyUpshiftFC As Double? _
        Implements IGearshiftEngineeringInputData.RatioEarlyUpshiftFC
        Get
            Return Nothing
        End Get
    End Property

    Public Overridable ReadOnly Property RatioEarlyDownshiftFC As Double? _
        Implements IGearshiftEngineeringInputData.RatioEarlyDownshiftFC
        Get
            Return Nothing
        End Get
    End Property

    Public ReadOnly Property AllowedGearRangeFC As Integer? Implements IGearshiftEngineeringInputData.AllowedGearRangeFC
        get
            Return Nothing
        End Get
    End Property

    Public ReadOnly Property VeloictyDropFactor As Double? Implements IGearshiftEngineeringInputData.VeloictyDropFactor
        Get
            Return Nothing
        End Get
    End Property

    Public ReadOnly Property AccelerationFactor As Double? Implements IGearshiftEngineeringInputData.AccelerationFactor
        Get
            Return Nothing
        End Get
    End Property

    public readonly Property MinEngineSpeedPostUpshift as PerSecond _
        Implements IGearshiftEngineeringInputData.MinEngineSpeedPostUpshift
        get
            Return Nothing
        End Get
    End Property

    Public ReadOnly Property ATLookAheadTime As Second Implements IGearshiftEngineeringInputData.ATLookAheadTime
        get
            return Nothing
        End Get
    End Property

    Public ReadOnly Property ShiftSpeedsTCToLocked As Double()() _
        Implements IGearshiftEngineeringInputData.ShiftSpeedsTCToLocked
        get
            return Nothing
        End Get
    End Property

    Public ReadOnly Property PEV_TargetSpeedBrakeNorm As Double? Implements IGearshiftEngineeringInputData.PEV_TargetSpeedBrakeNorm
    get
        Return Nothing
    End Get
    End Property

    Public ReadOnly Property PEV_DownshiftSpeedFactor As Double? Implements IGearshiftEngineeringInputData.PEV_DownshiftSpeedFactor
        get
            return nothing
        End Get
    End Property

    Public ReadOnly Property PEV_DeRatingDownshiftSpeedFactor As Double? Implements IGearshiftEngineeringInputData.PEV_DeRatingDownshiftSpeedFactor
    get
        return Nothing
    End Get
    End Property

    Public ReadOnly Property PEV_DownshiftMinSpeedFactor As Double? Implements IGearshiftEngineeringInputData.PEV_DownshiftMinSpeedFactor
    get
        Return Nothing
    End Get
    End Property

    Public Overridable ReadOnly Property LoadStageShiftLines As TableData _
        Implements IGearshiftEngineeringInputData.LoadStageShiftLines
        Get
            Return Nothing
        End Get
    End Property

    Public Overridable ReadOnly Property LoadStageThresholdsUp As IList(Of Double) _
        Implements IGearshiftEngineeringInputData.LoadStageThresholdsUp
        Get
            Return Nothing
        End Get
    End Property

    Public Overridable ReadOnly Property LoadStageThresholdsDown As IList(Of Double) _
        Implements IGearshiftEngineeringInputData.LoadStageThresholdsDown
        Get
            Return Nothing
        End Get
    End Property

    Public ReadOnly Property PowershiftShiftTime As Second Implements IGearboxEngineeringInputData.PowershiftShiftTime
        Get
            Return PSShiftTime.SI (Of Second)()
        End Get
    End Property


    Public ReadOnly Property IGearboxEngineeringInputData_UpshiftMinAcceleration As MeterPerSquareSecond _
        Implements IGearshiftEngineeringInputData.UpshiftMinAcceleration
        Get
            Return UpshiftMinAcceleration.SI (Of MeterPerSquareSecond)()
        End Get
    End Property


    Public ReadOnly Property IGearboxEngineeringInputData_StartSpeed As MeterPerSecond _
        Implements IGearshiftEngineeringInputData.StartSpeed
        Get
            Return StartSpeed.SI (Of MeterPerSecond)()
        End Get
    End Property

    Public ReadOnly Property MinTimeBetweenGearshift As Second _
        Implements IGearshiftEngineeringInputData.MinTimeBetweenGearshift
        Get
            Return ShiftTime.SI (Of Second)()
        End Get
    End Property

    Public ReadOnly Property TCData As TableData Implements ITorqueConverterDeclarationInputData.TCData
        Get
            If Not File.Exists(_torqueConverterFile.FullPath) Then Return Nothing
            Return VectoCSVFile.Read(_torqueConverterFile.FullPath)
        End Get
    End Property


    Public ReadOnly Property Ratio As Double Implements IAxleGearInputData.Ratio
        Get
            Return GearRatios(0)
        End Get
    End Property

    Public ReadOnly Property LossMap As TableData Implements IAxleGearInputData.LossMap
        Get
            If Not File.Exists(GearLossmaps(0).FullPath) Then Return Nothing
            Return VectoCSVFile.Read(GearLossmaps(0).FullPath)
        End Get
    End Property

    Public ReadOnly Property Efficiency As Double Implements IAxleGearInputData.Efficiency
        Get
            Return GearLossMap(0, True).ToDouble(0)
        End Get
    End Property

    Public ReadOnly Property LineType As AxleLineType Implements IAxleGearInputData.LineType
        Get
            Return AxleLineType.SinglePortalAxle
        End Get
    End Property
End Class

Public Class MockEngineeringVehicle
    Implements IVehicleEngineeringInputData, IVehicleComponentsEngineering

    Public Property DataSource As DataSource Implements IComponentInputData.DataSource
    Public Property SavedInDeclarationMode As Boolean Implements IComponentInputData.SavedInDeclarationMode
    Public Property Manufacturer As String Implements IComponentInputData.Manufacturer
    Public Property Model As String Implements IComponentInputData.Model
    Public Property SimulationToolLicenseNumber As String Implements IVehicleDeclarationInputData.SimulationToolLicenseNumber
    Public Property VehicleMonitoringData As String Implements IVehicleDeclarationInputData.VehicleMonitoringData
    Public Property H2StorageUsableCapacity As Kilogram Implements IVehicleDeclarationInputData.H2StorageUsableCapacity
    Public Property HydrogenStorageTechnology As HydrogenStorageTechnology? Implements IVehicleDeclarationInputData.HydrogenStorageTechnology
    Public Property BatteryOnlyMode As Boolean Implements IVehicleDeclarationInputData.BatteryOnlyMode
    Public Property DynamicChargingTechnology As DynamicChargingTechnology Implements IVehicleDeclarationInputData.DynamicChargingTechnology
    Public Property [Date] As DateTime Implements IComponentInputData.[Date]
    Public ReadOnly Property AppVersion As String Implements IComponentInputData.AppVersion
    Public Property CertificationMethod As CertificationMethod Implements IComponentInputData.CertificationMethod
    Public Property CertificationNumber As String Implements IComponentInputData.CertificationNumber
    Public Property DigestValue As DigestData Implements IComponentInputData.DigestValue
    Public Property Identifier As String Implements IVehicleDeclarationInputData.Identifier
    Public Property ExemptedVehicle As Boolean Implements IVehicleDeclarationInputData.ExemptedVehicle
    Public Property VIN As String Implements IVehicleDeclarationInputData.VIN
    Public Property LegislativeClass As LegislativeClass? Implements IVehicleDeclarationInputData.LegislativeClass
    Public Property VehicleCategory As VehicleCategory Implements IVehicleDeclarationInputData.VehicleCategory
    Public Property AxleConfiguration As AxleConfiguration Implements IVehicleDeclarationInputData.AxleConfiguration
    Public Property CurbMassChassis As Kilogram Implements IVehicleDeclarationInputData.CurbMassChassis
    Public Property GrossVehicleMassRating As Kilogram Implements IVehicleDeclarationInputData.GrossVehicleMassRating
    Public Property TorqueLimits As IList(Of ITorqueLimitInputData) Implements IVehicleDeclarationInputData.TorqueLimits
    Public Property ManufacturerAddress As String Implements IVehicleDeclarationInputData.ManufacturerAddress
    Public Property EngineIdleSpeed As PerSecond Implements IVehicleDeclarationInputData.EngineIdleSpeed
    Public Property VocationalVehicle As Boolean Implements IVehicleDeclarationInputData.VocationalVehicle
    Public Property SleeperCab As Boolean? Implements IVehicleDeclarationInputData.SleeperCab
    Public ReadOnly Property AirdragModifiedMultistep As Boolean? Implements IVehicleDeclarationInputData.AirdragModifiedMultistep
    Public Property TankSystem As TankSystem? Implements IVehicleDeclarationInputData.TankSystem

    Public Property IVehicleEngineeringInputData_ADAS As IAdvancedDriverAssistantSystemsEngineering _
        Implements IVehicleEngineeringInputData.ADAS

    Public ReadOnly Property IVehicleEngineeringInputData_Components As IVehicleComponentsEngineering _
        Implements IVehicleEngineeringInputData.Components
        Get
            Return Me
        End Get
    End Property

    Public Property ADAS As IAdvancedDriverAssistantSystemDeclarationInputData _
        Implements IVehicleDeclarationInputData.ADAS

    Public ReadOnly Property AxlePowertrainEngineeringInputData As IList(Of IAxlePowertrainEngineeringInputData) Implements IVehicleComponentsEngineering.AxlePowertrainEngineeringInputData

    Public ReadOnly Property IVehicleDeclarationInputData_InMotionCharging As IVehicleInMotionChargingDeclaration Implements IVehicleDeclarationInputData.InMotionCharging

    Public ReadOnly Property InitialSOC As Double Implements IVehicleEngineeringInputData.InitialSOC
    Public ReadOnly Property InMotionCharging As IVehicleInMotionChargingEngineering Implements IVehicleEngineeringInputData.InMotionCharging
    Public Property VehicleType As VectoSimulationJobType Implements IVehicleEngineeringInputData.VehicleType
    Public ReadOnly Property PTO_DriveGear As GearshiftPosition Implements IVehicleEngineeringInputData.PTO_DriveGear
    Public ReadOnly Property PTO_DriveEngineSpeed As PerSecond Implements IVehicleEngineeringInputData.PTO_DriveEngineSpeed

    Public Property ZeroEmissionVehicle As Boolean Implements IVehicleDeclarationInputData.ZeroEmissionVehicle
    Public Property HybridElectricHDV As Boolean Implements IVehicleDeclarationInputData.HybridElectricHDV
    Public Property DualFuelVehicle As Boolean Implements IVehicleDeclarationInputData.DualFuelVehicle
    Public Property MaxNetPower1 As Watt Implements IVehicleDeclarationInputData.MaxNetPower1
    Public ReadOnly Property ExemptedTechnology As String Implements IVehicleDeclarationInputData.ExemptedTechnology
    Public ReadOnly Property RegisteredClass As RegistrationClass? Implements IVehicleDeclarationInputData.RegisteredClass
    Public ReadOnly Property NumberPassengerSeatsUpperDeck As Integer? Implements IVehicleDeclarationInputData.NumberPassengerSeatsUpperDeck
    Public ReadOnly Property NumberPassengerSeatsLowerDeck As Integer? Implements IVehicleDeclarationInputData.NumberPassengerSeatsLowerDeck
    Public ReadOnly Property NumberPassengersStandingLowerDeck As Integer? Implements IVehicleDeclarationInputData.NumberPassengersStandingLowerDeck
    Public ReadOnly Property NumberPassengersStandingUpperDeck As Integer? Implements IVehicleDeclarationInputData.NumberPassengersStandingUpperDeck
    Public ReadOnly Property CargoVolume As CubicMeter Implements IVehicleDeclarationInputData.CargoVolume
    Public ReadOnly Property VehicleCode As VehicleCode? Implements IVehicleDeclarationInputData.VehicleCode
    Public ReadOnly Property LowEntry As Boolean? Implements IVehicleDeclarationInputData.LowEntry
    Public ReadOnly Property Articulated As Boolean Implements IVehicleDeclarationInputData.Articulated
    Public ReadOnly Property IVehicleDeclarationInputData_Height As Meter Implements IVehicleDeclarationInputData.Height
    Public Property CurbMassExtra As Kilogram Implements IVehicleEngineeringInputData.CurbMassExtra
    Public Property Loading As Kilogram Implements IVehicleEngineeringInputData.Loading
    Public Property DynamicTyreRadius As Meter Implements IVehicleEngineeringInputData.DynamicTyreRadius
    Public Property Height As Meter Implements IVehicleEngineeringInputData.Height
    Public ReadOnly Property ElectricMotorTorqueLimits As IDictionary(Of EMPlacement, IList(Of Tuple(Of Volt, TableData))) Implements IVehicleDeclarationInputData.ElectricMotorTorqueLimits
    Public ReadOnly Property BoostingLimitations As TableData Implements IVehicleDeclarationInputData.BoostingLimitations
    Public ReadOnly Property Length As Meter Implements IVehicleDeclarationInputData.Length
    Public ReadOnly Property Width As Meter Implements IVehicleDeclarationInputData.Width
    Public ReadOnly Property EntranceHeight As Meter Implements IVehicleDeclarationInputData.EntranceHeight
    Public ReadOnly Property DoorDriveTechnology As ConsumerTechnology? Implements IVehicleDeclarationInputData.DoorDriveTechnology
    Public ReadOnly Property VehicleDeclarationType As VehicleDeclarationType Implements IVehicleDeclarationInputData.VehicleDeclarationType


    Public Property Components As IVehicleComponentsDeclaration Implements IVehicleDeclarationInputData.Components
    Public ReadOnly Property XMLSource As XmlNode Implements IVehicleDeclarationInputData.XMLSource
    Public ReadOnly Property VehicleTypeApprovalNumber As String Implements IVehicleDeclarationInputData.VehicleTypeApprovalNumber
    Public ReadOnly Property ArchitectureID As ArchitectureID Implements IVehicleDeclarationInputData.ArchitectureID
    Public ReadOnly Property ArchitectureIDPwt2 As ArchitectureID Implements IVehicleDeclarationInputData.ArchitectureIDPwt2
    Public ReadOnly Property OVC As Boolean Implements IVehicleDeclarationInputData.OVC
    Public ReadOnly Property MaxChargingPower As Watt Implements IVehicleDeclarationInputData.MaxChargingPower
    Public ReadOnly Property IVehicleDeclarationInputData_VehicleType As VectoSimulationJobType Implements IVehicleDeclarationInputData.VehicleType

    Public Property AirdragInputData As IAirdragEngineeringInputData _
        Implements IVehicleComponentsEngineering.AirdragInputData

    Public Property GearboxInputData As IGearboxEngineeringInputData _
        Implements IVehicleComponentsEngineering.GearboxInputData

    Public Property TorqueConverterInputData As ITorqueConverterEngineeringInputData _
        Implements IVehicleComponentsEngineering.TorqueConverterInputData

    Public Property AxleGearInputData As IAxleGearInputData Implements IVehicleComponentsEngineering.AxleGearInputData

    Public Property AngledriveInputData As IAngledriveInputData _
        Implements IVehicleComponentsEngineering.AngledriveInputData

    Public Property EngineInputData As IEngineEngineeringInputData _
        Implements IVehicleComponentsEngineering.EngineInputData

    Public Property AuxiliaryInputData As IAuxiliariesEngineeringInputData _
        Implements IVehicleComponentsEngineering.AuxiliaryInputData

    Public Property RetarderInputData As IRetarderInputData Implements IVehicleComponentsEngineering.RetarderInputData

    Public Property PTOTransmissionInputData As IPTOTransmissionInputData _
        Implements IVehicleComponentsEngineering.PTOTransmissionInputData

    Public Property AxleWheels As IAxlesEngineeringInputData Implements IVehicleComponentsEngineering.AxleWheels
    Public ReadOnly Property ElectricStorage As IElectricStorageSystemEngineeringInputData Implements IVehicleComponentsEngineering.ElectricStorage
    Public ReadOnly Property ElectricMachines As IElectricMachinesEngineeringInputData Implements IVehicleComponentsEngineering.ElectricMachines
    Public ReadOnly Property IEPCEngineeringInputData As IIEPCEngineeringInputData Implements IVehicleComponentsEngineering.IEPCEngineeringInputData
    Public ReadOnly Property FuelCellSystemInputData As IFuelCellSystemEngineeringInputData Implements IVehicleComponentsEngineering.FuelCellSystemInputData
End Class

Public Class MockJobInputData
    Implements IEngineeringJobInputData
    Public Property SavedInDeclarationMode As Boolean Implements IDeclarationJobInputData.SavedInDeclarationMode

    Public Property IEngineeringJobInputData_Vehicle As IVehicleEngineeringInputData _
        Implements IEngineeringJobInputData.Vehicle

    Public Property Vehicle As IVehicleDeclarationInputData Implements IDeclarationJobInputData.Vehicle
    Public ReadOnly Property HybridStrategyParameters As IHybridStrategyParameters Implements IEngineeringJobInputData.HybridStrategyParameters
    Public Property Cycles As IList(Of ICycleData) Implements IEngineeringJobInputData.Cycles
    Public Property JobType As VectoSimulationJobType Implements IEngineeringJobInputData.JobType
    Public Property EngineOnly As IEngineEngineeringInputData Implements IEngineeringJobInputData.EngineOnly
    Public Property JobName As String Implements IDeclarationJobInputData.JobName
End Class

Public Class MockDriverInputData
    Implements IDriverEngineeringInputData
    Public Property SavedInDeclarationMode As Boolean Implements IDriverDeclarationInputData.SavedInDeclarationMode
    Public Property OverSpeedData As IOverSpeedEngineeringInputData Implements IDriverEngineeringInputData.OverSpeedData

    Public Property AccelerationCurve As IDriverAccelerationData _
        Implements IDriverEngineeringInputData.AccelerationCurve

    Public Property Lookahead As ILookaheadCoastingInputData Implements IDriverEngineeringInputData.Lookahead

    Public Property GearshiftInputData As IGearshiftEngineeringInputData _
        Implements IDriverEngineeringInputData.GearshiftInputData

    Public Property EngineStopStartData As IEngineStopStartEngineeringInputData _
        Implements IDriverEngineeringInputData.EngineStopStartData

    Public Property EcoRollData As IEcoRollEngineeringInputData Implements IDriverEngineeringInputData.EcoRollData
    Public Property PCCData As IPCCEngineeringInputData Implements IDriverEngineeringInputData.PCCData
End Class

Public Class MockEngineeringInputData
    Implements IEngineeringInputDataProvider
    Public Property DataSource As DataSource Implements IInputDataProvider.DataSource
    Public Property JobInputData As IEngineeringJobInputData Implements IEngineeringInputDataProvider.JobInputData

    Public Property DriverInputData As IDriverEngineeringInputData _
        Implements IEngineeringInputDataProvider.DriverInputData
End Class