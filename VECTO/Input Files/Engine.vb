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
Imports TUGraz.VectoCore.InputData.Reader.DataObjectAdapter
Imports TUGraz.VectoCore.InputData.Reader.DataObjectAdapter.SimulationComponents
Imports TUGraz.VectoCore.Models.Declaration
Imports TUGraz.VectoCore.Models.SimulationComponent.Data
Imports TUGraz.VectoCore.Models.SimulationComponent.Impl
Imports TUGraz.VectoCore.OutputData.FileIO
Imports TUGraz.VectoCore.Utils
Imports DeclarationDataAdapterHeavyLorry = TUGraz.VectoCore.InputData.Reader.DataObjectAdapter.HeavyLorry.DeclarationDataAdapterHeavyLorry


''' <summary>
''' Engine input file
''' </summary>
''' <remarks></remarks>
<CustomValidation(GetType(Engine), "ValidateEngine")>
Public Class Engine
	Implements IEngineEngineeringInputData, IEngineDeclarationInputData, IEngineModeDeclarationInputData, IEngineModeEngineeringInputData

	''' <summary>
	''' Current format version
	''' </summary>
	''' <remarks></remarks>
	Private Const FormatVersion As Short = 3

	''' <summary>
	''' Engine description (model, type, etc.). Saved in input file.
	''' </summary>
	''' <remarks></remarks>
	Public ModelName As String

	''' <summary>
	''' Engine displacement [ccm]. Saved in input file.
	''' </summary>
	''' <remarks></remarks>
	Public Displacement As Double

	''' <summary>
	''' Idling speed [1/min]. Saved in input file.
	''' </summary>
	''' <remarks></remarks>
	Public IdleSpeed As Double

	''' <summary>
	''' Rotational inertia including flywheel [kgm²]. Saved in input file. Overwritten by generic value in Declaration mode.
	''' </summary>
	''' <remarks></remarks>
	Public EngineInertia As Double

	''' <summary>
	''' List of full load/motoring curve files (.vfld)
	''' </summary>
	''' <remarks></remarks>
	Private ReadOnly _fullLoadCurvePath As SubPath



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



	Public ratedPowerInput As Watt
	Public ratedSpeedInput As PerSecond
	Public maxTorqueInput As NewtonMeter

	Public WHRTypeInput As WHRType


	Public PrimaryEngineFuel As EngineFuel
	Public SecondaryEngineFuel As EngineFuel

	Public ElectricalWHRData As WHRData
	Public MechanicalWHRData As WHRData

	Public DualFuelInput As Boolean


	''' <summary>
	''' New instance. Initialise
	''' </summary>
	''' <remarks></remarks>
	Public Sub New()
		_myPath = ""
		_filePath = ""

		_fullLoadCurvePath = New SubPath

		PrimaryEngineFuel = New EngineFuel(Me)
		SecondaryEngineFuel = New EngineFuel(Me)
		SetDefault()
	End Sub

	''' <summary>
	''' Set default values
	''' </summary>
	''' <remarks></remarks>
	Private Sub SetDefault()
		ModelName = "Undefined"
		Displacement = 0
		IdleSpeed = 0
		EngineInertia = 0


		_fullLoadCurvePath.Clear()

	End Sub

	''' <summary>
	''' </summary>
	''' <returns>True if successful.</returns>
	''' <remarks></remarks>
	Public Function SaveFile() As Boolean

		Dim validationResults As IList(Of ValidationResult) =
				Validate(If(Cfg.DeclMode, ExecutionMode.Declaration, ExecutionMode.Engineering), VectoSimulationJobType.ConventionalVehicle, Nothing, Nothing, False)

		If validationResults.Count > 0 Then
			Dim messages As IEnumerable(Of String) =
					validationResults.Select(Function(r) r.ErrorMessage + String.Join(", ", r.MemberNames.Distinct()))
			MsgBox("Invalid input." + Environment.NewLine + String.Join(Environment.NewLine, messages), MsgBoxStyle.OkOnly,
					"Failed to save engine")
			Return False
		End If

		Try
			Dim writer As JSONFileWriter = New JSONFileWriter()
			writer.SaveEngine(Me, _filePath, Cfg.DeclMode)

		Catch ex As Exception
			MsgBox("Failed to write Engine file: " + ex.Message)
			Return False
		End Try
		Return True
	End Function


	''' <summary>
	''' </summary>
	''' <value></value>
	''' <returns>Full filepath</returns>
	''' <remarks></remarks>
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


	Public Property PathFld(Optional ByVal original As Boolean = False) As String
		Get
			If original Then
				Return _fullLoadCurvePath.OriginalPath
			Else
				Return _fullLoadCurvePath.FullPath
			End If
		End Get
		Set(ByVal value As String)
			_fullLoadCurvePath.Init(_myPath, value)
		End Set
	End Property




	' ReSharper disable once UnusedMember.Global  -- used for Validation
	Public Shared Function ValidateEngine(engine As Engine, validationContext As ValidationContext) As ValidationResult
		Dim engineData As CombustionEngineData


		Dim modeService As VectoValidationModeServiceContainer =
				TryCast(validationContext.GetService(GetType(VectoValidationModeServiceContainer)),
						VectoValidationModeServiceContainer)
		Dim mode As ExecutionMode = If(modeService Is Nothing, ExecutionMode.Declaration, modeService.Mode)
		Dim emsCycle As Boolean = (modeService IsNot Nothing) AndAlso modeService.IsEMSCycle
		Dim gbxType As GearboxType? = If(modeService Is Nothing, GearboxType.MT, modeService.GearboxType)
		Dim jobType As VectoSimulationJobType = If(modeService Is Nothing, VectoSimulationJobType.ConventionalVehicle, modeService.JobType)
		Dim emPos As PowertrainPosition? = If(modeService Is Nothing, PowertrainPosition.HybridPositionNotSet, modeService.EMPowertrainPosition)

		Try
			If mode = ExecutionMode.Declaration Then
				Dim doa As DeclarationDataAdapterHeavyLorry = New DeclarationDataAdapterHeavyLorry()
				Dim dummyGearboxData As IGearboxDeclarationInputData = New Gearbox() With {
						.Type = GearboxType.AMT,
						.MaxTorque = New List(Of String),
						.GearRatios = New List(Of Double)()
						}
				Dim dummyVehicle As IVehicleDeclarationInputData = New DummyVehicle() With {
					.GearboxInputData = dummyGearboxData,
					.EngineInputData = engine,
					.TankSystem = TankSystem.Compressed
				}
				engineData = New CombustionEngineComponentDataAdapter().CreateEngineData(dummyVehicle, engine.EngineModes.First(), New Mission() With {.MissionType = MissionType.LongHaul})
			Else
				Dim doa As EngineeringDataAdapter = New EngineeringDataAdapter()
				Dim dummyVehicle As IVehicleEngineeringInputData = New DummyVehicle() With {
						.IVehicleComponentsEngineering_EngineInputData = engine,
						.TankSystem = TankSystem.Compressed
						}
				engineData = doa.CreateEngineData(dummyVehicle, CType(engine.EngineModes.First(), IEngineModeEngineeringInputData))
			End If

			Dim result As IList(Of ValidationResult) =
					engineData.Validate(If(Cfg.DeclMode, ExecutionMode.Declaration, ExecutionMode.Engineering), jobType, emPos, gbxType, emsCycle)

			If Not result.Any() Then Return ValidationResult.Success

			Return New ValidationResult("Engine Configuration is invalid. ",
										result.Select(Function(r) r.ErrorMessage + String.Join(Environment.NewLine, r.MemberNames)).ToList())
		Catch ex As Exception
			Return New ValidationResult(ex.Message)
		End Try
	End Function

#Region "IInputData"

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

	Public ReadOnly Property IEngineDeclarationInputData_Displacement As CubicMeter _
		Implements IEngineDeclarationInputData.Displacement
		Get
			Return (Displacement / 1000.0 / 1000.0).SI(Of CubicMeter)()
		End Get
	End Property

	Public ReadOnly Property IEngineModeDeclarationInputData_IdleSpeed As PerSecond _
		Implements IEngineModeDeclarationInputData.IdleSpeed
		Get
			Return IdleSpeed.RPMtoRad()
		End Get
	End Property



	Public ReadOnly Property FullLoadCurve As TableData Implements IEngineModeDeclarationInputData.FullLoadCurve
		Get
			If Not File.Exists(_fullLoadCurvePath.FullPath) Then _
				Throw New VectoException("Full-Load Curve is missing or invalid")
			Return VectoCSVFile.Read(_fullLoadCurvePath.FullPath)
		End Get
	End Property

	Public ReadOnly Property IEngineModeEngineeringInputData_Fuels As IList(Of IEngineFuelEngineeringInputData) Implements IEngineModeEngineeringInputData.Fuels
		Get
			Dim retval As List(Of IEngineFuelEngineeringInputData) = New List(Of IEngineFuelEngineeringInputData)({PrimaryEngineFuel})
			If (DualFuelInput) Then
				retval.Add(SecondaryEngineFuel)
			End If
			Return retval
		End Get
	End Property

	Public ReadOnly Property Fuels As IList(Of IEngineFuelDeclarationInputData) Implements IEngineModeDeclarationInputData.Fuels
		Get
			Dim retval As List(Of IEngineFuelDeclarationInputData) = New List(Of IEngineFuelDeclarationInputData)({PrimaryEngineFuel})
			If (DualFuelInput) Then
				retval.Add(SecondaryEngineFuel)
			End If
			Return retval
		End Get
	End Property

	Public ReadOnly Property WasteHeatRecoveryDataElectrical As IWHRData Implements IEngineModeDeclarationInputData.WasteHeatRecoveryDataElectrical
		Get
			Return ElectricalWHRData
		End Get
	End Property

	Public ReadOnly Property WasteHeatRecoveryDataMechanical As IWHRData Implements IEngineModeDeclarationInputData.WasteHeatRecoveryDataMechanical
		Get
			Return MechanicalWHRData
		End Get
	End Property

	Public ReadOnly Property RatedPowerDeclared As Watt Implements IEngineDeclarationInputData.RatedPowerDeclared
		Get
			Return ratedPowerInput
		End Get
	End Property

	Public ReadOnly Property RatedSpeedDeclared As PerSecond Implements IEngineDeclarationInputData.RatedSpeedDeclared
		Get
			Return ratedSpeedInput
		End Get
	End Property

	Public ReadOnly Property MaxTorqueDeclared As NewtonMeter Implements IEngineDeclarationInputData.MaxTorqueDeclared
		Get
			Return maxTorqueInput
		End Get
	End Property

	Public ReadOnly Property IEngineEngineeringInputData_EngineModes As IList(Of IEngineModeEngineeringInputData) Implements IEngineEngineeringInputData.EngineModes
		Get
			Return New List(Of IEngineModeEngineeringInputData)({Me})
		End Get
	End Property

	Public ReadOnly Property EngineModes As IList(Of IEngineModeDeclarationInputData) Implements IEngineDeclarationInputData.EngineModes
		Get
			Return New List(Of IEngineModeDeclarationInputData)({Me})
		End Get
	End Property

	Public ReadOnly Property WHRType As WHRType Implements IEngineDeclarationInputData.WHRType
		Get
			Return WHRTypeInput
		End Get
	End Property

	Public ReadOnly Property Inertia As KilogramSquareMeter Implements IEngineEngineeringInputData.Inertia
		Get
			Return EngineInertia.SI(Of KilogramSquareMeter)()
		End Get
	End Property


	Public ReadOnly Property EngineStartTime As Second Implements IEngineEngineeringInputData.EngineStartTime
		Get
			Return Nothing
		End Get
	End Property

#End Region


End Class

Public Class WHRData

	Implements IWHRData

	Public WHRUrbanInput As Double
	Public WHRRuralInput As Double
	Public WHRMotorwayInput As Double
	Public WHRColdHotInput As Double
	Public WHRRegPerInput As Double
	Public WHREngineeringInput As Double

	Protected EngineData As Engine

	Public Sub New(engineDatar As Engine)
		EngineData = engineDatar
	End Sub

	Public ReadOnly Property UrbanCorrectionFactor As Double Implements IWHRData.UrbanCorrectionFactor
		Get
			Return WHRUrbanInput
		End Get
	End Property
	Public ReadOnly Property RuralCorrectionFactor As Double Implements IWHRData.RuralCorrectionFactor
		Get
			Return WHRRuralInput
		End Get
	End Property
	Public ReadOnly Property MotorwayCorrectionFactor As Double Implements IWHRData.MotorwayCorrectionFactor
		Get
			Return WHRMotorwayInput
		End Get
	End Property
	Public ReadOnly Property BFColdHot As Double Implements IWHRData.BFColdHot
		Get
			Return WHRColdHotInput
		End Get
	End Property
	Public ReadOnly Property CFRegPer As Double Implements IWHRData.CFRegPer
		Get
			Return WHRRegPerInput
		End Get
	End Property

	Public ReadOnly Property EngineeringCorrectionFactor As Double Implements IWHRData.EngineeringCorrectionFactor
		Get
			Return WHREngineeringInput
		End Get
	End Property

	Public ReadOnly Property GeneratedPower As TableData Implements IWHRData.GeneratedPower
		Get
			If Not File.Exists(EngineData.PrimaryEngineFuel._fuelConsumptionMapPath.FullPath) Then _
				Throw New VectoException("FuelConsumptionMap is missing or invalid")
			Return VectoCSVFile.Read(EngineData.PrimaryEngineFuel._fuelConsumptionMapPath.FullPath)
		End Get
	End Property

End Class

Public Class EngineFuel
	Implements IEngineFuelDeclarationInputData, IEngineFuelEngineeringInputData

	''' <summary>
	''' WHTC Urban test results. Saved in input file.
	''' </summary>
	''' <remarks></remarks>
	Public WHTCUrbanInput As Double

	''' <summary>
	''' WHTC Rural test results. Saved in input file.
	''' </summary>
	''' <remarks></remarks>
	Public WHTCRuralInput As Double

	''' <summary>
	''' WHTC Motorway test results. Saved in input file.
	''' </summary>
	''' <remarks></remarks>
	Public WHTCMotorwayInput As Double

	Public WHTCEngineeringInput As Double

	''' <summary>
	''' Path to fuel consumption map
	''' </summary>
	''' <remarks></remarks>

	Friend ReadOnly _fuelConsumptionMapPath As SubPath


    Public ColdHotBalancingFactorInput As Double
    Public correctionFactorRegPerInput As Double
	Public FuelTypeInput As FuelType
	Private engineData As Engine

	Public Sub New(engine As Engine)

		engineData = engine
		_fuelConsumptionMapPath = New SubPath

		SetDefault()
	End Sub

	''' <summary>
	''' Set default values
	''' </summary>
	''' <remarks></remarks>
	Private Sub SetDefault()
		_fuelConsumptionMapPath.Clear()

		WHTCUrbanInput = 0
		WHTCRuralInput = 0
		WHTCMotorwayInput = 0
		WHTCEngineeringInput = 1
	End Sub

	Public ReadOnly Property WHTCMotorway As Double Implements IEngineFuelDeclarationInputData.WHTCMotorway
		Get
			Return WHTCMotorwayInput
		End Get
	End Property

	Public ReadOnly Property WHTCRural As Double Implements IEngineFuelDeclarationInputData.WHTCRural
		Get
			Return WHTCRuralInput
		End Get
	End Property

	Public ReadOnly Property WHTCUrban As Double Implements IEngineFuelDeclarationInputData.WHTCUrban
		Get
			Return WHTCUrbanInput
		End Get
	End Property

	Public ReadOnly Property ColdHotBalancingFactor As Double Implements IEngineFuelDeclarationInputData.ColdHotBalancingFactor
		Get
			Return ColdHotBalancingFactorInput
		End Get
	End Property

    Public ReadOnly Property CorrectionFactorRegPer As Double Implements IEngineFuelDeclarationInputData.CorrectionFactorRegPer
        Get
            Return correctionFactorRegPerInput
        End Get
    End Property

    Public ReadOnly Property FuelType As FuelType Implements IEngineFuelDeclarationInputData.FuelType
		Get
			Return FuelTypeInput
		End Get
	End Property

	''' <summary>
	''' Get or set file path (cSubPath) to FC map (.vmap)
	''' </summary>
	''' <param name="original">True= (relative) file path as saved in file; False= full file path</param>
	''' <value></value>
	''' <returns>Relative or absolute file path to FC map</returns>
	''' <remarks></remarks>
	Public Property PathMap(Optional ByVal original As Boolean = False) As String
		Get
			If original Then
				Return _fuelConsumptionMapPath.OriginalPath
			Else
				Return _fuelConsumptionMapPath.FullPath
			End If
		End Get
		Set(ByVal value As String)
			_fuelConsumptionMapPath.Init(engineData._myPath, value)
		End Set
	End Property

	Public ReadOnly Property FuelConsumptionMap As TableData Implements IEngineFuelDeclarationInputData.FuelConsumptionMap
		Get
			If Not File.Exists(_fuelConsumptionMapPath.FullPath) Then _
				Throw New VectoException("FuelConsumptionMap is missing or invalid")
			Return VectoCSVFile.Read(_fuelConsumptionMapPath.FullPath)
		End Get
	End Property

	Public ReadOnly Property WHTCEngineering As Double Implements IEngineFuelEngineeringInputData.WHTCEngineering
		Get
			Return WHTCEngineeringInput
		End Get
	End Property
End Class


Public Class DummyVehicle
	Implements IVehicleDeclarationInputData, IVehicleComponentsDeclaration, IVehicleEngineeringInputData, IVehicleComponentsEngineering

	Public Property DataSource As DataSource Implements IComponentInputData.DataSource
	Public Property SavedInDeclarationMode As Boolean Implements IComponentInputData.SavedInDeclarationMode
	Public Property Manufacturer As String Implements IComponentInputData.Manufacturer
	Public Property Model As String Implements IComponentInputData.Model
	Public Property [Date] As DateTime Implements IComponentInputData.[Date]
	Public Property SimulationToolLicenseNumber As String Implements IVehicleDeclarationInputData.SimulationToolLicenseNumber
	Public Property VehicleMonitoringData As String Implements IVehicleDeclarationInputData.VehicleMonitoringData
	Public Property H2StorageUsableCapacity As Kilogram Implements IVehicleDeclarationInputData.H2StorageUsableCapacity
	Public Property HydrogenStorageTechnology As HydrogenStorageTechnology? Implements IVehicleDeclarationInputData.HydrogenStorageTechnology
	Public Property BatteryOnlyMode As Boolean Implements IVehicleDeclarationInputData.BatteryOnlyMode
	Public Property DynamicChargingTechnology As DynamicChargingTechnology Implements IVehicleDeclarationInputData.DynamicChargingTechnology
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
	Public ReadOnly Property TorqueLimits As IList(Of ITorqueLimitInputData) Implements IVehicleDeclarationInputData.TorqueLimits
		Get
			Return New List(Of ITorqueLimitInputData)()
		End Get
	End Property
	Public Property ManufacturerAddress As String Implements IVehicleDeclarationInputData.ManufacturerAddress
	Public Property EngineIdleSpeed As PerSecond Implements IVehicleDeclarationInputData.EngineIdleSpeed
	Public Property VocationalVehicle As Boolean Implements IVehicleDeclarationInputData.VocationalVehicle
	Public Property SleeperCab As Boolean? Implements IVehicleDeclarationInputData.SleeperCab
	Public ReadOnly Property AirdragModifiedMultistep As Boolean? Implements IVehicleDeclarationInputData.AirdragModifiedMultistep
	Public Property TankSystem As TankSystem? Implements IVehicleDeclarationInputData.TankSystem
	Public Property IVehicleEngineeringInputData_ADAS As IAdvancedDriverAssistantSystemsEngineering Implements IVehicleEngineeringInputData.ADAS
	Public ReadOnly Property IVehicleEngineeringInputData_Components As IVehicleComponentsEngineering Implements IVehicleEngineeringInputData.Components
		Get
			Return Me
		End Get
	End Property
	Public Property ADAS As IAdvancedDriverAssistantSystemDeclarationInputData Implements IVehicleDeclarationInputData.ADAS
    Public ReadOnly Property IVehicleDeclarationInputData_InMotionCharging As IVehicleInMotionChargingDeclaration Implements IVehicleDeclarationInputData.InMotionCharging
    Public ReadOnly Property InitialSOC As Double Implements IVehicleEngineeringInputData.InitialSOC
    Public ReadOnly Property InMotionCharging As IVehicleInMotionChargingEngineering Implements IVehicleEngineeringInputData.InMotionCharging
    Public ReadOnly Property VehicleType As VectoSimulationJobType Implements IVehicleEngineeringInputData.VehicleType
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
    Public Property CurbMassExtra As Kilogram Implements IVehicleEngineeringInputData.CurbMassExtra
    Public Property Loading As Kilogram Implements IVehicleEngineeringInputData.Loading
    Public Property DynamicTyreRadius As Meter Implements IVehicleEngineeringInputData.DynamicTyreRadius
    Public Property Height As Meter Implements IVehicleEngineeringInputData.Height
    Public ReadOnly Property LowEntry As Boolean? Implements IVehicleDeclarationInputData.LowEntry
	Public ReadOnly Property ElectricMotorTorqueLimits As IDictionary(Of EMPlacement, IList(Of Tuple(Of Volt, TableData))) Implements IVehicleDeclarationInputData.ElectricMotorTorqueLimits
	Public ReadOnly Property BoostingLimitations As TableData Implements IVehicleDeclarationInputData.BoostingLimitations
    Public ReadOnly Property Articulated As Boolean Implements IVehicleDeclarationInputData.Articulated
    Public ReadOnly Property IVehicleDeclarationInputData_Height As Meter Implements IVehicleDeclarationInputData.Height
    Public ReadOnly Property Length As Meter Implements IVehicleDeclarationInputData.Length
    Public ReadOnly Property Width As Meter Implements IVehicleDeclarationInputData.Width
    Public ReadOnly Property EntranceHeight As Meter Implements IVehicleDeclarationInputData.EntranceHeight
    Public ReadOnly Property DoorDriveTechnology As ConsumerTechnology? Implements IVehicleDeclarationInputData.DoorDriveTechnology
    Public ReadOnly Property VehicleDeclarationType As VehicleDeclarationType Implements IVehicleDeclarationInputData.VehicleDeclarationType

    Public ReadOnly Property Components As IVehicleComponentsDeclaration Implements IVehicleDeclarationInputData.Components
        Get
            Return Me
        End Get
    End Property

    Public ReadOnly Property XMLSource As XmlNode Implements IVehicleDeclarationInputData.XMLSource
    Public ReadOnly Property VehicleTypeApprovalNumber As String Implements IVehicleDeclarationInputData.VehicleTypeApprovalNumber
	Public ReadOnly Property ArchitectureID As ArchitectureID Implements IVehicleDeclarationInputData.ArchitectureID
	Public ReadOnly Property ArchitectureIDPwt2 As ArchitectureID Implements IVehicleDeclarationInputData.ArchitectureIDPwt2
	Public ReadOnly Property OVC As Boolean Implements IVehicleDeclarationInputData.OVC
    Public ReadOnly Property MaxChargingPower As Watt Implements IVehicleDeclarationInputData.MaxChargingPower
	Public ReadOnly Property IVehicleDeclarationInputData_VehicleType As VectoSimulationJobType Implements IVehicleDeclarationInputData.VehicleType

	Public ReadOnly Property AxlePowertrainEngineeringInputData As IList(Of IAxlePowertrainEngineeringInputData) Implements IVehicleComponentsEngineering.AxlePowertrainEngineeringInputData

	Public Property AirdragInputData As IAirdragDeclarationInputData Implements IVehicleComponentsDeclaration.AirdragInputData
	Public Property IVehicleComponentsEngineering_GearboxInputData As IGearboxEngineeringInputData Implements IVehicleComponentsEngineering.GearboxInputData
	Public Property IVehicleComponentsEngineering_AirdragInputData As IAirdragEngineeringInputData Implements IVehicleComponentsEngineering.AirdragInputData
	Public Property GearboxInputData As IGearboxDeclarationInputData Implements IVehicleComponentsDeclaration.GearboxInputData
	Public Property IVehicleComponentsEngineering_TorqueConverterInputData As ITorqueConverterEngineeringInputData Implements IVehicleComponentsEngineering.TorqueConverterInputData
	Public Property TorqueConverterInputData As ITorqueConverterDeclarationInputData Implements IVehicleComponentsDeclaration.TorqueConverterInputData
	Public Property IVehicleComponentsEngineering_AxleGearInputData As IAxleGearInputData Implements IVehicleComponentsEngineering.AxleGearInputData
	Public Property AxleGearInputData As IAxleGearInputData Implements IVehicleComponentsDeclaration.AxleGearInputData
	Public Property IVehicleComponentsEngineering_AngledriveInputData As IAngledriveInputData Implements IVehicleComponentsEngineering.AngledriveInputData
	Public Property AngledriveInputData As IAngledriveInputData Implements IVehicleComponentsDeclaration.AngledriveInputData
	Public Property IVehicleComponentsEngineering_EngineInputData As IEngineEngineeringInputData Implements IVehicleComponentsEngineering.EngineInputData
	Public Property EngineInputData As IEngineDeclarationInputData Implements IVehicleComponentsDeclaration.EngineInputData
	Public Property IVehicleComponentsEngineering_AuxiliaryInputData As IAuxiliariesEngineeringInputData Implements IVehicleComponentsEngineering.AuxiliaryInputData
	Public Property AuxiliaryInputData As IAuxiliariesDeclarationInputData Implements IVehicleComponentsDeclaration.AuxiliaryInputData
	Public Property IVehicleComponentsEngineering_RetarderInputData As IRetarderInputData Implements IVehicleComponentsEngineering.RetarderInputData
	Public Property RetarderInputData As IRetarderInputData Implements IVehicleComponentsDeclaration.RetarderInputData
	Public Property IVehicleComponentsEngineering_PTOTransmissionInputData As IPTOTransmissionInputData Implements IVehicleComponentsEngineering.PTOTransmissionInputData
	Public Property PTOTransmissionInputData As IPTOTransmissionInputData Implements IVehicleComponentsDeclaration.PTOTransmissionInputData
	Public Property IVehicleComponentsEngineering_AxleWheels As IAxlesEngineeringInputData Implements IVehicleComponentsEngineering.AxleWheels
	Public Property AxleWheels As IAxlesDeclarationInputData Implements IVehicleComponentsDeclaration.AxleWheels
	Public ReadOnly Property IVehicleComponentsEngineering_ElectricStorage As IElectricStorageSystemEngineeringInputData Implements IVehicleComponentsEngineering.ElectricStorage
	Public ReadOnly Property BusAuxiliaries As IBusAuxiliariesDeclarationData Implements IVehicleComponentsDeclaration.BusAuxiliaries
	Public ReadOnly Property ElectricStorage As IElectricStorageSystemDeclarationInputData Implements IVehicleComponentsDeclaration.ElectricStorage
	Public ReadOnly Property IVehicleComponentsEngineering_ElectricMachines As IElectricMachinesEngineeringInputData Implements IVehicleComponentsEngineering.ElectricMachines
	Public ReadOnly Property ElectricMachines As IElectricMachinesDeclarationInputData Implements IVehicleComponentsDeclaration.ElectricMachines
	Public ReadOnly Property IEPCEngineeringInputData As IIEPCEngineeringInputData Implements IVehicleComponentsEngineering.IEPCEngineeringInputData
	Public ReadOnly Property FuelCellSystemInputData As IFuelCellSystemEngineeringInputData Implements IVehicleComponentsEngineering.FuelCellSystemInputData
    Public ReadOnly Property IEPC As IIEPCDeclarationInputData Implements IVehicleComponentsDeclaration.IEPC
	Public ReadOnly Property FuelCellSystem As IFuelCellSystemDeclarationInputData Implements IVehicleComponentsDeclaration.FuelCellSystem
	Public ReadOnly Property AxlePowertrainInputData As IList(Of IAxlePowertrainDeclarationInputData) Implements IVehicleComponentsDeclaration.AxlePowertrainInputData
	Public ReadOnly Property Generator As ElectricMachineEntry(Of IElectricMotorDeclarationInputData) Implements IVehicleComponentsDeclaration.Generator
End Class


