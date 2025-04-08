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

Imports System.Collections.Generic
Imports System.ComponentModel.DataAnnotations
Imports System.IO
Imports System.Linq
Imports System.Xml
Imports Ninject
Imports TUGraz.VECTO.Input_Files
Imports TUGraz.VectoCommon.BusAuxiliaries
Imports TUGraz.VectoCommon.InputData
Imports TUGraz.VectoCommon.Models
Imports TUGraz.VectoCommon.Utils
Imports TUGraz.VectoCore
Imports TUGraz.VectoCore.InputData.FileIO.JSON
Imports TUGraz.VectoCore.InputData.Impl
Imports TUGraz.VectoCore.InputData.Reader.DataObjectAdapter
Imports TUGraz.VectoCore.InputData.Reader.DataObjectAdapter.SimulationComponents
Imports TUGraz.VectoCore.Models.Declaration
Imports TUGraz.VectoCore.Models.SimulationComponent.Data
Imports TUGraz.VectoCore.Models.SimulationComponent.Impl
Imports TUGraz.VectoCore.Utils
Imports TUGraz.VectoCore.Utils.Ninject
Imports DeclarationDataAdapterHeavyLorry = TUGraz.VectoCore.InputData.Reader.DataObjectAdapter.HeavyLorry.DeclarationDataAdapterHeavyLorry

<CustomValidation(GetType(Vehicle), "ValidateVehicle")>
Public Class Vehicle
	Implements IVehicleEngineeringInputData, IVehicleDeclarationInputData, IRetarderInputData, IPTOTransmissionInputData,
				IAngledriveInputData, IAirdragEngineeringInputData, IAdvancedDriverAssistantSystemDeclarationInputData, IAdvancedDriverAssistantSystemsEngineering,
				IVehicleComponentsEngineering, IVehicleComponentsDeclaration, IAxlesEngineeringInputData, IAxlesDeclarationInputData, IVehicleInMotionChargingEngineering, IVehicleInMotionChargingDeclaration

	Private _filePath As String
	Private _path As String

	Public Mass As Double
	Public Loading As Double

	Public CdA0 As Double

	Public CrossWindCorrectionMode As CrossWindCorrectionMode
	Public ReadOnly CrossWindCorrectionFile As SubPath

	<ValidateObject> Public RetarderType As RetarderType
	Public RetarderRatio As Double = 0
	Public ReadOnly RetarderLossMapFile As SubPath
	Public ReadOnly EmTorqueLimitsFile As SubPath
	Public ReadOnly PropulsionTorqueFile As SubPath

	Public DynamicTyreRadius As Double
	Public ReadOnly Axles As List(Of AxleInputData)


	Public VehicleCategory As VehicleCategory
	Public MassExtra As Double
	Public MassMax As Double
	Public AxleConfiguration As AxleConfiguration
	Public SavedInDeclMode As Boolean

	Public AngledriveType As AngledriveType
	Public AngledriveRatio As Double
	Public ReadOnly AngledriveLossMapFile As SubPath

	Public PtoType As String
	Public ReadOnly PtoLossMap As SubPath
	Public ReadOnly PtoCycleStandstill As SubPath
	Public ReadOnly EPtoCycleStandstill As SubPath

	Public ReadOnly PtoCycleDriving As SubPath
	Public torqueLimitsList As List(Of ITorqueLimitInputData)
	Public VehicleidlingSpeed As PerSecond
	Public legClass As LegislativeClass
	Public VehicleHeight As Double

	Public EcoRolltype As EcoRollType
	Public PCC As PredictiveCruiseControlType
	Public EngineStop As Boolean

	Public VehicleTankSystem As TankSystem?

	Public ReadOnly ElectricMotorFile As SubPath
	Public ReadOnly GenSetEMFile As SubPath

	Public ReadOnly ReessPacks As List(Of Tuple(Of String, Integer, Integer))


	Public ReadOnly FuelCellComponents As List(Of Tuple(Of String, Integer))

	Public ElectricMotorPosition As PowertrainPosition
	Public ElectricMotorCount As Integer
	Public ElectricMotorRatio As Double
	'Public ElectricMotorMechEff As Double
	Public ElectricMotorMechLossMap As SubPath
	Public GenSetMechLossMap As SubPath

	Public GenSetPosition As PowertrainPosition
	Public GenSetCount As Integer
	Public GenSetRatio As Double
	'Public ElectricMotorMechEff As Double
	Public GenSetLossMap As SubPath

	Public GearDuringPTODrive As UInteger?
	Public EngineSpeedDuringPTODrive As PerSecond
	Public ElectricMotorPerGearRatios As Double()
	Public IEPCFile As SubPath


	Public Sub New()
		_path = ""
		_filePath = ""
		CrossWindCorrectionFile = New SubPath

		RetarderLossMapFile = New SubPath
		AngledriveLossMapFile = New SubPath()
		EmTorqueLimitsFile = New SubPath()
		PropulsionTorqueFile = New SubPath()
		IEPCFile = New SubPath()

		Axles = New List(Of AxleInputData)
		torqueLimitsList = New List(Of ITorqueLimitInputData)
		ReessPacks = New List(Of Tuple(Of String, Integer, Integer))
		FuelCellComponents = New List(Of Tuple(Of String, Integer))
		PtoLossMap = New SubPath()
		PtoCycleStandstill = New SubPath()
		EPtoCycleStandstill = New SubPath()
		PtoCycleDriving = New SubPath()
		ElectricMotorFile = New SubPath()
		ElectricMotorMechLossMap = New SubPath()
		GenSetEMFile = New SubPath()
		GenSetMechLossMap = New SubPath()

		SetDefault()
	End Sub


	' ReSharper disable once UnusedMember.Global  -- used for Validation
	Public Shared Function ValidateVehicle(vehicle As Vehicle, validationContext As ValidationContext) As ValidationResult

		Dim vehicleData As VehicleData
		Dim airdragData As AirdragData
		Dim retarderData As RetarderData
		Dim ptoData As PTOData
		Dim angledriveData As AngledriveData

		Dim modeService As VectoValidationModeServiceContainer =
				TryCast(validationContext.GetService(GetType(VectoValidationModeServiceContainer)),
						VectoValidationModeServiceContainer)
		Dim mode As ExecutionMode = If(modeService Is Nothing, ExecutionMode.Declaration, modeService.Mode)
		Dim emsCycle As Boolean = (modeService IsNot Nothing) AndAlso modeService.IsEMSCycle
		Dim gbxType As GearboxType? = If(modeService Is Nothing, Nothing, modeService.GearboxType)
		Dim jobType As VectoSimulationJobType = If(modeService Is Nothing, VectoSimulationJobType.ConventionalVehicle, modeService.JobType)
		Dim emPos = If(modeService Is Nothing, modeService.EMPowertrainPosition, PowertrainPosition.HybridPositionNotSet)

		Try
			If mode = ExecutionMode.Declaration Then

				'Dim doa As ILorryDeclarationDataAdapter = CType(_kernel.Value.Get(Of IDeclarationDataAdapterFactory).CreateDataAdapter(New VehicleTypeAndArchitectureStringHelperRundata.VehicleClassification(vehicle)), ILorryDeclarationDataAdapter)
				Dim segment As Segment = DeclarationData.TruckSegments.Lookup(vehicle.VehicleCategory, vehicle.AxleConfiguration,
																		vehicle.GrossVehicleMassRating, vehicle.CurbMassChassis, False)
				vehicleData = New LorryVehicleDataAdapter().CreateVehicleData(vehicle, segment, segment.Missions.First(),
													segment.Missions.First().Loadings.First(), True)
				airdragData = New AirdragDataAdapter().CreateAirdragData(vehicle, vehicle.InMotionChargingDecl, segment.Missions.First(), segment,OvcHevMode.NotApplicable, 0)
				retarderData = New RetarderDataAdapter().CreateRetarderData(vehicle, vehicle.ArchitectureID, vehicle.Components?.IEPC)
				angledriveData = New AngledriveDataAdapter().CreateAngledriveData(vehicle)
				ptoData = New PTODataAdapterLorry().CreatePTOTransmissionData(vehicle, vehicle.Components.GearboxInputData)
			Else
				Dim doa As EngineeringDataAdapter = New EngineeringDataAdapter()
				vehicleData = doa.CreateVehicleData(vehicle)
				airdragData = doa.CreateAirdragData(vehicle, vehicle, If(vehicle.InMotionCharging.Enabled, vehicle.InMotionCharging.ShareIMCAvailabilityTotalMission * 0.5, 0))
				retarderData = doa.CreateRetarderData(vehicle, emPos)
				angledriveData = doa.CreateAngledriveData(vehicle)
				ptoData = doa.CreatePTOTransmissionData(vehicle)
			End If

			Dim result As IList(Of ValidationResult) =
					vehicleData.Validate(If(Cfg.DeclMode, ExecutionMode.Declaration, ExecutionMode.Engineering), jobType, emPos, gbxType, emsCycle)
			If result.Any() Then
				Return _
					New ValidationResult("Vehicle Configuration is invalid. ",
										result.Select(Function(r) r.ErrorMessage + String.Join(Environment.NewLine, r.MemberNames)).ToList())
			End If

			result = airdragData.Validate(If(Cfg.DeclMode, ExecutionMode.Declaration, ExecutionMode.Engineering), jobType, emPos, gbxType, emsCycle)
			If result.Any() Then
				Return _
					New ValidationResult("Airdrag Configuration is invalid. ",
										result.Select(Function(r) r.ErrorMessage + String.Join(Environment.NewLine, r.MemberNames)).ToList())
			End If

			result = retarderData.Validate(If(Cfg.DeclMode, ExecutionMode.Declaration, ExecutionMode.Engineering), jobType, emPos, gbxType, emsCycle)
			If result.Any() Then
				Return _
					New ValidationResult("Retarder Configuration is invalid. ",
										result.Select(Function(r) r.ErrorMessage + String.Join(Environment.NewLine, r.MemberNames)).ToList())
			End If

			If vehicle.AngledriveType = AngledriveType.SeparateAngledrive Then
				result = angledriveData.Validate(If(Cfg.DeclMode, ExecutionMode.Declaration, ExecutionMode.Engineering), jobType, emPos, gbxType, emsCycle)
				If result.Any() Then
					Return _
						New ValidationResult("AngleDrive Configuration is invalid. ",
											result.Select(Function(r) r.ErrorMessage + String.Join(Environment.NewLine, r.MemberNames)).ToList())
				End If
			End If

			If Not vehicle.PTOTransmissionType = "None" Then
				result = ptoData.Validate(If(Cfg.DeclMode, ExecutionMode.Declaration, ExecutionMode.Engineering), jobType, emPos, gbxType, emsCycle)
				If result.Any() Then
					Return _
						New ValidationResult("PTO Configuration is invalid. ",
											result.Select(Function(r) r.ErrorMessage + String.Join(Environment.NewLine, r.MemberNames)).ToList())
				End If
			End If

			Return ValidationResult.Success

		Catch ex As Exception
			Return New ValidationResult(ex.Message)
		End Try
	End Function

	Private Sub SetDefault()
		Mass = 0
		MassExtra = 0
		Loading = 0
		CdA0 = 0
		CrossWindCorrectionFile.Clear()
		CrossWindCorrectionMode = CrossWindCorrectionMode.NoCorrection

		DynamicTyreRadius = 0

		RetarderType = RetarderType.None
		RetarderRatio = 1
		RetarderLossMapFile.Clear()
		AngledriveLossMapFile.Clear()
		EmTorqueLimitsFile.Clear()
		PropulsionTorqueFile.Clear()

		AngledriveType = AngledriveType.None
		AngledriveLossMapFile.Clear()
		AngledriveRatio = 1

		PtoType = PTOTransmission.NoPTO
		PtoLossMap.Clear()
		PtoCycleStandstill.Clear()
		EPtoCycleStandstill.Clear()
		PtoCycleDriving.Clear()

		Axles.Clear()
		VehicleCategory = VehicleCategory.RigidTruck
		MassMax = 0
		AxleConfiguration = AxleConfiguration.AxleConfig_4x2

		ElectricMotorFile.Clear()
		ElectricMotorMechLossMap.Clear()

		GenSetEMFile.Clear()
		GenSetMechLossMap.Clear()
		'IMC
		IMCEnabled = False
		IMCDeltaCdxA = 0.SI(of SquareMeter)
		IMCOnMotorwayOnly = False
		ShareIMCAvailabilityTotalMission = 0

		SavedInDeclMode = False
	End Sub


	Public Function SaveFile() As Boolean
		SavedInDeclMode = Cfg.DeclMode

		Dim validationResults As IList(Of ValidationResult) =
				Validate(If(Cfg.DeclMode, ExecutionMode.Declaration, ExecutionMode.Engineering), VehicleType, ElectricMotorPosition, Nothing, False)

		If validationResults.Count > 0 Then
			Dim messages As IEnumerable(Of String) =
					validationResults.Select(Function(r) r.ErrorMessage + String.Join(", ", r.MemberNames.Distinct()))
			MsgBox("Invalid input." + Environment.NewLine + String.Join(Environment.NewLine, messages), MsgBoxStyle.OkOnly,
					"Failed to save vehicle")
			Return False
		End If

		Try
			Dim writer As JSONFileWriter = JSONFileWriter.Instance
			writer.SaveVehicle(Me, Me, Me, Me, Me, _filePath, Cfg.DeclMode)
		Catch ex As Exception
			MsgBox("Failed to save Vehicle file: " + ex.Message)
			Return False
		End Try
		Return True
	End Function


#Region "Properties"


	Public Property FilePath() As String
		Get
			Return _filePath
		End Get
		Set(value As String)
			_filePath = value
			If _filePath = "" Then
				_path = ""
			Else
				_path = Path.GetDirectoryName(_filePath) & "\"
			End If
		End Set
	End Property

#End Region

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

	Public ReadOnly Property Model As String Implements IComponentInputData.Model
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

	Public ReadOnly Property IVehicleDeclarationInputData_VehicleCategory As VehicleCategory _
		Implements IVehicleDeclarationInputData.VehicleCategory
		Get
			Return VehicleCategory
		End Get
	End Property

	Public ReadOnly Property IVehicleDeclarationInputData_AxleConfiguration As AxleConfiguration _
		Implements IVehicleDeclarationInputData.AxleConfiguration
		Get
			Return AxleConfiguration
		End Get
	End Property

	Public ReadOnly Property Identifier As String Implements IVehicleDeclarationInputData.Identifier
		Get
			Return ""
		End Get
	End Property

	Public ReadOnly Property ExemptedVehicle As Boolean Implements IVehicleDeclarationInputData.ExemptedVehicle
		Get
			Return False
		End Get
	End Property

	Public ReadOnly Property VIN As String Implements IVehicleDeclarationInputData.VIN
		Get
			Return TUGraz.VectoCore.Configuration.Constants.NOT_AVAILABLE
		End Get
	End Property

	Public ReadOnly Property LegislativeClass As LegislativeClass? Implements IVehicleEngineeringInputData.LegislativeClass
		Get
			Return legClass
		End Get
	End Property

	Public ReadOnly Property CurbMassChassis As Kilogram Implements IVehicleDeclarationInputData.CurbMassChassis
		Get
			Return Mass.SI(Of Kilogram)()
		End Get
	End Property

	Public ReadOnly Property GrossVehicleMassRating As Kilogram _
		Implements IVehicleDeclarationInputData.GrossVehicleMassRating
		Get
			Return MassMax.SI(Unit.SI.Ton).Cast(Of Kilogram)()
		End Get
	End Property

	Public ReadOnly Property TorqueLimits As IList(Of ITorqueLimitInputData) _
		Implements IVehicleDeclarationInputData.TorqueLimits
		Get
			Return torqueLimitsList
		End Get
	End Property

	Public ReadOnly Property ManufacturerAddress As String Implements IVehicleDeclarationInputData.ManufacturerAddress
		Get
			Return TUGraz.VectoCore.Configuration.Constants.NOT_AVAILABLE
		End Get
	End Property

	Public ReadOnly Property EngineIdleSpeed As PerSecond Implements IVehicleDeclarationInputData.EngineIdleSpeed
		Get
			Return VehicleidlingSpeed
		End Get
	End Property

	Public ReadOnly Property AirDragArea As SquareMeter Implements IAirdragEngineeringInputData.AirDragArea
		Get
			Return If(Double.IsNaN(CdA0), Nothing, CdA0.SI(Of SquareMeter)())
		End Get
	End Property

	Public ReadOnly Property TransferredAirDragArea As SquareMeter Implements IAirdragDeclarationInputData.TransferredAirDragArea
	Public ReadOnly Property AirDragArea_0 As SquareMeter Implements IAirdragDeclarationInputData.AirDragArea_0
	Public ReadOnly Property IAirdragDeclarationInputData_XMLSource As XmlNode Implements IAirdragDeclarationInputData.XMLSource

	Public ReadOnly Property IVehicleEngineeringInputData_Axles As IList(Of IAxleEngineeringInputData) _
		Implements IAxlesEngineeringInputData.AxlesEngineering
		Get
			Return Axles.Cast(Of IAxleEngineeringInputData)().ToList()
		End Get
	End Property

	Public ReadOnly Property IVehicleDeclarationInputData_Axles As IList(Of IAxleDeclarationInputData) _
		Implements IAxlesDeclarationInputData.AxlesDeclaration
		Get
			Return Axles.Cast(Of IAxleDeclarationInputData)().ToList()
		End Get
	End Property


	Public ReadOnly Property CurbMassExtra As Kilogram Implements IVehicleEngineeringInputData.CurbMassExtra
		Get
			Return MassExtra.SI(Of Kilogram)()
		End Get
	End Property

	Public ReadOnly Property IVehicleDeclarationInputData_Height As Meter Implements IVehicleDeclarationInputData.Height

	Public ReadOnly Property Articulated As Boolean Implements IVehicleDeclarationInputData.Articulated

	Public ReadOnly Property Height As Meter Implements IVehicleEngineeringInputData.Height
		Get
			Return VehicleHeight.SI(Of Meter)()
		End Get
	End Property

	'    Public ReadOnly Property ElectricMotorTorqueLimits As TableData Implements IVehicleEngineeringInputData.ElectricMotorTorqueLimits
	'	get
	'		If (String.IsNullOrWhiteSpace(EmTorqueLimitsFile.FullPath))
	'			return Nothing
	'		End If
	'		Return VectoCSVFile.Read(EmTorqueLimitsFile.FullPath)
	'	End Get
	'    End Property
	Public ReadOnly Property ElectricMotorTorqueLimits As IDictionary(Of PowertrainPosition, IList(Of Tuple(Of Volt, TableData))) Implements IVehicleDeclarationInputData.ElectricMotorTorqueLimits

	Public ReadOnly Property BoostingLimitations As TableData Implements IVehicleDeclarationInputData.BoostingLimitations
		Get
			If (String.IsNullOrWhiteSpace(PropulsionTorqueFile.FullPath)) Then
				Return Nothing
			End If
			Return VectoCSVFile.Read(PropulsionTorqueFile.FullPath)
		End Get
	End Property

	Public ReadOnly Property Length As Meter Implements IVehicleDeclarationInputData.Length
	Public ReadOnly Property Width As Meter Implements IVehicleDeclarationInputData.Width
	Public ReadOnly Property EntranceHeight As Meter Implements IVehicleDeclarationInputData.EntranceHeight
	Public ReadOnly Property DoorDriveTechnology As ConsumerTechnology? Implements IVehicleDeclarationInputData.DoorDriveTechnology
	Public ReadOnly Property VehicleDeclarationType As VehicleDeclarationType Implements IVehicleDeclarationInputData.VehicleDeclarationType

	Public ReadOnly Property IVehicleEngineeringInputData_Components As IVehicleComponentsEngineering Implements IVehicleEngineeringInputData.Components
		Get
			Return Me
		End Get
	End Property

	Public ReadOnly Property CrosswindCorrectionMap As TableData _
		Implements IAirdragEngineeringInputData.CrosswindCorrectionMap
		Get
			Return VectoCSVFile.Read(CrossWindCorrectionFile.FullPath)
		End Get
	End Property

	Public ReadOnly Property IVehicleEngineeringInputData_CrossWindCorrectionMode As CrossWindCorrectionMode _
		Implements IAirdragEngineeringInputData.CrossWindCorrectionMode
		Get
			Return CrossWindCorrectionMode
		End Get
	End Property

	Public ReadOnly Property IVehicleEngineeringInputData_DynamicTyreRadius As Meter _
		Implements IVehicleEngineeringInputData.DynamicTyreRadius
		Get
			Return DynamicTyreRadius.SI(Unit.SI.Milli.Meter).Cast(Of Meter)()
		End Get
	End Property

	Public ReadOnly Property IVehicleEngineeringInputData_Loading As Kilogram _
		Implements IVehicleEngineeringInputData.Loading
		Get
			Return Loading.SI(Of Kilogram)()
		End Get
	End Property


	Public ReadOnly Property Type As RetarderType Implements IRetarderInputData.Type
		Get
			Return RetarderType
		End Get
	End Property

	Public ReadOnly Property IRetarderInputData_Ratio As Double Implements IRetarderInputData.Ratio
		Get
			Return RetarderRatio
		End Get
	End Property

	Public ReadOnly Property Ratio As Double Implements IAngledriveInputData.Ratio
		Get
			Return AngledriveRatio
		End Get
	End Property

	Public ReadOnly Property IRetarderInputData_LossMap As TableData Implements IRetarderInputData.LossMap
		Get
			Return VectoCSVFile.Read(RetarderLossMapFile.FullPath)
		End Get
	End Property

	Public ReadOnly Property AngledriveInputDataType As AngledriveType Implements IAngledriveInputData.Type
		Get
			Return AngledriveType
		End Get
	End Property


	Public ReadOnly Property LossMap As TableData Implements IAngledriveInputData.LossMap
		Get
			Return VectoCSVFile.Read(AngledriveLossMapFile.FullPath)
		End Get
	End Property


	Public ReadOnly Property Efficiency As Double Implements IAngledriveInputData.Efficiency
		Get
			Return If(IsNumeric(AngledriveLossMapFile.OriginalPath), AngledriveLossMapFile.OriginalPath.ToDouble(), -1.0)
		End Get
	End Property

#End Region

	Public ReadOnly Property PTOTransmissionType As String Implements IPTOTransmissionInputData.PTOTransmissionType
		Get
			Return PtoType
		End Get
	End Property

	Public ReadOnly Property PTOCycleDuringStop As TableData Implements IPTOTransmissionInputData.PTOCycleDuringStop
		Get
			If String.IsNullOrWhiteSpace(PtoCycleStandstill.FullPath) Then
				Return Nothing
			End If
			Return VectoCSVFile.Read(PtoCycleStandstill.FullPath)
		End Get
	End Property

	Public ReadOnly Property EPTOCycleDuringStop As TableData Implements IPTOTransmissionInputData.EPTOCycleDuringStop
		Get
			If String.IsNullOrWhiteSpace(EPtoCycleStandstill.FullPath) Then
				Return Nothing
			End If
			Return VectoCSVFile.Read(EPtoCycleStandstill.FullPath)
		End Get
	End Property

	Public ReadOnly Property IPTOTransmissionInputData_PTOLossMap As TableData _
		Implements IPTOTransmissionInputData.PTOLossMap
		Get
			If String.IsNullOrWhiteSpace(PtoLossMap.FullPath) Then
				Return Nothing
			End If
			Return VectoCSVFile.Read(PtoLossMap.FullPath)
		End Get
	End Property

	Public ReadOnly Property PTOCycleWhileDriving As TableData _
		Implements IPTOTransmissionInputData.PTOCycleWhileDriving
		Get
			If String.IsNullOrWhiteSpace(PtoCycleDriving.FullPath) Then
				Return Nothing
			End If
			Return VectoCSVFile.Read(PtoCycleDriving.FullPath)
		End Get
	End Property


	Public ReadOnly Property IDeclarationInputDataProvider_AirdragInputData As IAirdragDeclarationInputData _
		Implements IVehicleComponentsDeclaration.AirdragInputData
		Get
			Return AirdragInputData
		End Get
	End Property

	Public ReadOnly Property AirdragInputData As IAirdragEngineeringInputData _
		Implements IVehicleComponentsEngineering.AirdragInputData
		Get
			Return Me
		End Get
	End Property

	Public ReadOnly Property IDeclarationInputDataProvider_GearboxInputData As IGearboxDeclarationInputData _
		Implements IVehicleComponentsDeclaration.GearboxInputData
		Get
			Return Nothing
			'If Not File.Exists(_gearboxFile.FullPath) Then Return Nothing
			'Return New JSONComponentInputData(_gearboxFile.FullPath).JobInputData.Vehicle.GearboxInputData
		End Get
	End Property

	Public ReadOnly Property GearboxInputData As IGearboxEngineeringInputData _
		Implements IVehicleComponentsEngineering.GearboxInputData
		Get
			Return Nothing
			'If Not File.Exists(_gearboxFile.FullPath) Then Return Nothing
			'Return New JSONComponentInputData(_gearboxFile.FullPath).JobInputData.Vehicle.GearboxInputData
		End Get
	End Property

	Public ReadOnly Property AxlePowertrainEngineeringInputData As IList(Of IAxlePowertrainEngineeringInputData) _
		Implements IVehicleComponentsEngineering.AxlePowertrainEngineeringInputData
		Get
			Return Nothing
		End Get
	End Property

	Public ReadOnly Property IDeclarationInputDataProvider_TorqueConverterInputData As ITorqueConverterDeclarationInputData _
		Implements IVehicleComponentsDeclaration.TorqueConverterInputData
		Get
			Return Nothing
			'If Not File.Exists(_gearboxFile.FullPath) Then Return Nothing
			'Return New JSONComponentInputData(_gearboxFile.FullPath).JobInputData.Vehicle.TorqueConverterInputData
		End Get
	End Property

	Public ReadOnly Property TorqueConverterInputData As ITorqueConverterEngineeringInputData _
		Implements IVehicleComponentsEngineering.TorqueConverterInputData
		Get
			Return Nothing
			'If Not File.Exists(_gearboxFile.FullPath) Then Return Nothing
			'Return New JSONComponentInputData(_gearboxFile.FullPath).JobInputData.Vehicle.TorqueConverterInputData
		End Get
	End Property

	Public ReadOnly Property IDeclarationInputDataProvider_AxleGearInputData As IAxleGearInputData _
		Implements IVehicleComponentsDeclaration.AxleGearInputData
		Get
			Return Nothing
			'If Not File.Exists(_gearboxFile.FullPath) Then Return Nothing
			'Return New JSONComponentInputData(_gearboxFile.FullPath).JobInputData.Vehicle.AxleGearInputData
		End Get
	End Property

	Public ReadOnly Property AxleGearInputData As IAxleGearInputData _
		Implements IVehicleComponentsEngineering.AxleGearInputData
		Get
			Return Nothing
			'If Not File.Exists(_gearboxFile.FullPath) Then Return Nothing
			'Return New JSONComponentInputData(_gearboxFile.FullPath).JobInputData.Vehicle.AxleGearInputData
		End Get
	End Property

	Public ReadOnly Property DeclarationInputDataProviderAngledriveInputData As IAngledriveInputData _
		Implements IVehicleComponentsDeclaration.AngledriveInputData
		Get
			Return Me
		End Get
	End Property

	Public ReadOnly Property AngledriveInputData As IAngledriveInputData _
		Implements IVehicleComponentsEngineering.AngledriveInputData
		Get
			Return Me
		End Get
	End Property

	Public ReadOnly Property IDeclarationInputDataProvider_EngineInputData As IEngineDeclarationInputData _
		Implements IVehicleComponentsDeclaration.EngineInputData
		Get
			Return Nothing
			'If Not File.Exists(_engineFile.FullPath) Then Return Nothing
			'Return New JSONComponentInputData(_engineFile.FullPath).JobInputData.Vehicle.EngineInputData
		End Get
	End Property

	Public ReadOnly Property EngineInputData As IEngineEngineeringInputData _
		Implements IVehicleComponentsEngineering.EngineInputData
		Get
			Return Nothing
			'If Not File.Exists(_engineFile.FullPath) Then Return Nothing
			'Return New JSONComponentInputData(_engineFile.FullPath).JobInputData.Vehicle.EngineInputData
		End Get
	End Property

	Public ReadOnly Property IVehicleComponentsDeclaration_AuxiliaryInputData As IAuxiliariesDeclarationInputData Implements IVehicleComponentsDeclaration.AuxiliaryInputData
		Get
			Return Nothing
		End Get
	End Property

	Public ReadOnly Property AuxiliaryInputData As IAuxiliariesEngineeringInputData Implements IVehicleComponentsEngineering.AuxiliaryInputData
		Get
			Return Nothing
		End Get
	End Property


	Public ReadOnly Property IDeclarationInputDataProvider_RetarderInputData As IRetarderInputData _
		Implements IVehicleComponentsDeclaration.RetarderInputData
		Get
			Return Me
		End Get
	End Property

	Public ReadOnly Property RetarderInputData As IRetarderInputData _
		Implements IVehicleComponentsEngineering.RetarderInputData
		Get
			Return Me
		End Get
	End Property


	'Public ReadOnly Property DriverInputData As IDriverEngineeringInputData _
	'	Implements IEngineeringInputDataProvider.DriverInputData
	'	Get
	'		Return Nothing
	'	End Get
	'End Property

	Public ReadOnly Property IDeclarationInputDataProvider_PTOTransmissionInputData As IPTOTransmissionInputData _
		Implements IVehicleComponentsDeclaration.PTOTransmissionInputData
		Get
			Return Me
		End Get
	End Property

	Public ReadOnly Property PTOTransmissionInputData As IPTOTransmissionInputData _
		Implements IVehicleComponentsEngineering.PTOTransmissionInputData
		Get
			Return Me
		End Get
	End Property

	Public ReadOnly Property IVehicleComponentsDeclaration_AxleWheels As IAxlesDeclarationInputData Implements IVehicleComponentsDeclaration.AxleWheels
		Get
			Return Me
		End Get
	End Property

	Public ReadOnly Property AxleWheels As IAxlesEngineeringInputData Implements IVehicleComponentsEngineering.AxleWheels
		Get
			Return Me
		End Get
	End Property

	Public ReadOnly Property ElectricStorage As IElectricStorageSystemEngineeringInputData Implements IVehicleComponentsEngineering.ElectricStorage
		Get
			Return New ElectricStorageSystemWrapper(Me)
		End Get
	End Property

	Public ReadOnly Property IVehicleComponentsDeclaration_ElectricMachines As IElectricMachinesDeclarationInputData Implements IVehicleComponentsDeclaration.ElectricMachines

	Public ReadOnly Property IVehicleComponentsDeclaration_ElectricStorage As IElectricStorageSystemDeclarationInputData Implements IVehicleComponentsDeclaration.ElectricStorage
	Public ReadOnly Property ElectricMachines As IElectricMachinesEngineeringInputData Implements IVehicleComponentsEngineering.ElectricMachines
		Get
			Return New ElectricMachinesWrapper(Me)
		End Get
	End Property

	Public ReadOnly Property IEPCEngineeringInputData As IIEPCEngineeringInputData Implements IVehicleComponentsEngineering.IEPCEngineeringInputData
		Get
			Return New IEPCWrapper(Me)
		End Get
	End Property

	Public ReadOnly Property FuelCellSystemInputData As IFuelCellSystemEngineeringInputData Implements IVehicleComponentsEngineering.FuelCellSystemInputData
		Get
			Return New FuelCellSystemWrapper(Me)
		End Get
	End Property

	Public ReadOnly Property IEPC As IIEPCDeclarationInputData Implements IVehicleComponentsDeclaration.IEPC
		Get
			Return IEPCEngineeringInputData
		End Get
	End Property

	Public ReadOnly Property BusAuxiliaries As IBusAuxiliariesDeclarationData Implements IVehicleComponentsDeclaration.BusAuxiliaries

	Public ReadOnly Property VocationalVehicle As Boolean Implements IVehicleDeclarationInputData.VocationalVehicle
		Get
			Return DeclarationData.Vehicle.VocationalVehicleDefault
		End Get
	End Property

	Public ReadOnly Property SleeperCab As Boolean? Implements IVehicleDeclarationInputData.SleeperCab
		Get
			Return DeclarationData.Vehicle.SleeperCabDefault
		End Get
	End Property

	Public ReadOnly Property AirdragModifiedMultistep As Boolean? Implements IVehicleDeclarationInputData.AirdragModifiedMultistep

	Public ReadOnly Property TankSystem As TankSystem? Implements IVehicleDeclarationInputData.TankSystem
		Get
			Return VehicleTankSystem
		End Get
	End Property

	Public ReadOnly Property IVehicleEngineeringInputData_ADAS As IAdvancedDriverAssistantSystemsEngineering Implements IVehicleEngineeringInputData.ADAS
		Get
			Return Me
		End Get
	End Property

	Public ReadOnly Property ADAS As IAdvancedDriverAssistantSystemDeclarationInputData Implements IVehicleDeclarationInputData.ADAS
		Get
			Return Me
		End Get
	End Property

	Public ReadOnly Property PTO_DriveGear As GearshiftPosition Implements IVehicleEngineeringInputData.PTO_DriveGear
		Get
			Return If(GearDuringPTODrive.HasValue, New GearshiftPosition(GearDuringPTODrive.Value), Nothing)
		End Get
	End Property

	Public Property InitialSOC As Double Implements IVehicleEngineeringInputData.InitialSOC
	Public Property VehicleType As VectoSimulationJobType Implements IVehicleEngineeringInputData.VehicleType


	Public ReadOnly Property PTO_DriveEngineSpeed As PerSecond Implements IVehicleEngineeringInputData.PTO_DriveEngineSpeed
		Get
			Return EngineSpeedDuringPTODrive
		End Get
	End Property

	Public ReadOnly Property ZeroEmissionVehicle As Boolean Implements IVehicleDeclarationInputData.ZeroEmissionVehicle
		Get
			Return DeclarationData.Vehicle.ZeroEmissionVehicleDefault
		End Get
	End Property

	Public ReadOnly Property HybridElectricHDV As Boolean Implements IVehicleDeclarationInputData.HybridElectricHDV
		Get
			Return DeclarationData.Vehicle.HybridElectricHDVDefault
		End Get
	End Property

	Public ReadOnly Property DualFuelVehicle As Boolean Implements IVehicleDeclarationInputData.DualFuelVehicle
		Get
			Return DeclarationData.Vehicle.DualFuelVehicleDefault
		End Get
	End Property

	Public ReadOnly Property MaxNetPower1 As Watt Implements IVehicleDeclarationInputData.MaxNetPower1
		Get
			Return Nothing
		End Get
	End Property

	Public ReadOnly Property ExemptedTechnology As String Implements IVehicleDeclarationInputData.ExemptedTechnology

	Public ReadOnly Property RegisteredClass As RegistrationClass? Implements IVehicleDeclarationInputData.RegisteredClass
	Public ReadOnly Property NumberPassengerSeatsUpperDeck As Integer? Implements IVehicleDeclarationInputData.NumberPassengerSeatsUpperDeck
	Public ReadOnly Property NumberPassengerSeatsLowerDeck As Integer? Implements IVehicleDeclarationInputData.NumberPassengerSeatsLowerDeck
	Public ReadOnly Property NumberPassengersStandingLowerDeck As Integer? Implements IVehicleDeclarationInputData.NumberPassengersStandingLowerDeck
	Public ReadOnly Property NumberPassengersStandingUpperDeck As Integer? Implements IVehicleDeclarationInputData.NumberPassengersStandingUpperDeck
	Public ReadOnly Property CargoVolume As CubicMeter Implements IVehicleDeclarationInputData.CargoVolume
	Public ReadOnly Property VehicleCode As VehicleCode? Implements IVehicleDeclarationInputData.VehicleCode
	Public ReadOnly Property LowEntry As Boolean? Implements IVehicleDeclarationInputData.LowEntry

	Public ReadOnly Property Components As IVehicleComponentsDeclaration Implements IVehicleDeclarationInputData.Components
		Get
			Return Me
		End Get
	End Property

	Public ReadOnly Property IVehicleDeclarationInputData_XMLSource As XmlNode Implements IVehicleDeclarationInputData.XMLSource

	Public ReadOnly Property EngineStopStart As Boolean Implements IAdvancedDriverAssistantSystemDeclarationInputData.EngineStopStart
		Get
			Return EngineStop
		End Get
	End Property

	Public ReadOnly Property EcoRoll As EcoRollType Implements IAdvancedDriverAssistantSystemDeclarationInputData.EcoRoll
		Get
			Return EcoRolltype
		End Get
	End Property


	Public ReadOnly Property PredictiveCruiseControl As PredictiveCruiseControlType Implements IAdvancedDriverAssistantSystemDeclarationInputData.PredictiveCruiseControl
		Get
			Return PCC
		End Get
	End Property

	Public ReadOnly Property ATEcoRollReleaseLockupClutch As Boolean? Implements IAdvancedDriverAssistantSystemDeclarationInputData.ATEcoRollReleaseLockupClutch
		Get
			Return EcoRollReleaseLockupClutch
		End Get
	End Property

	Public ReadOnly Property NumSteeredAxles As Integer? Implements IAxlesDeclarationInputData.NumSteeredAxles
		Get
			Return Nothing
		End Get
	End Property
	Public ReadOnly Property XMLSource As XmlNode Implements IAdvancedDriverAssistantSystemDeclarationInputData.XMLSource
	Public ReadOnly Property VehicleTypeApprovalNumber As String Implements IVehicleDeclarationInputData.VehicleTypeApprovalNumber
	Public ReadOnly Property ArchitectureID As ArchitectureID Implements IVehicleDeclarationInputData.ArchitectureID
	Public Property OvcHev As Boolean Implements IVehicleDeclarationInputData.OvcHev
	Public Property MaxChargingPower As Watt Implements IVehicleDeclarationInputData.MaxChargingPower
	Public ReadOnly Property IVehicleDeclarationInputData_VehicleType As VectoSimulationJobType Implements IVehicleDeclarationInputData.VehicleType

	Public ReadOnly Property IAdvancedDriverAssistantSystemsEngineering_DataSource As DataSource Implements IAdvancedDriverAssistantSystemsEngineering.DataSource
		Get
			Return New DataSource() With {.SourceType = DataSourceType.JSONFile}
		End Get
	End Property


	Public ReadOnly Property IAxlesEngineeringInputData_DataSource As DataSource Implements IAxlesEngineeringInputData.DataSource
		Get
			Return New DataSource() With {.SourceType = DataSourceType.JSONFile}
		End Get
	End Property

	Public Property EcoRollReleaseLockupClutch As Boolean

	Public ReadOnly Property IAxlesDeclarationInputData_XMLSource As XmlNode Implements IAxlesDeclarationInputData.XMLSource

	Public ReadOnly Property InMotionCharging As IVehicleInMotionChargingEngineering Implements IVehicleEngineeringInputData.InMotionCharging
		Get
			Return Me
		End Get
	End Property

    Public ReadOnly Property InMotionChargingDecl As IVehicleInMotionChargingDeclaration Implements IVehicleDeclarationInputData.InMotionCharging
        Get
            Return Me
        End Get
    End Property


	Public Property IMCEnabled As Boolean Implements IVehicleInMotionChargingEngineering.Enabled
	Public Property ShareIMCAvailabilityTotalMission As Double Implements IVehicleInMotionChargingEngineering.ShareIMCAvailabilityTotalMission
	Public Property IMCDeltaCdxA As SquareMeter Implements IVehicleInMotionChargingEngineering.DeltaCdxA
	Public Property IMCOnMotorwayOnly As Boolean Implements IVehicleInMotionChargingEngineering.IMCOnMotorwayOnly
   
    Public Property IMCDeclarationTechnology As IMCTechnology Implements IVehicleInMotionChargingDeclaration.Technology
End Class

Public Class IEPCWrapper
	Implements IIEPCEngineeringInputData

	Private _vehicle As Vehicle

	Public Sub New(vehicle As Vehicle)
		_vehicle = vehicle
	End Sub

	Public ReadOnly Property DataSource As DataSource Implements IComponentInputData.DataSource
		Get
			Dim retVal As DataSource = New DataSource()
			retVal.SourceType = DataSourceType.JSONFile
			retVal.SourceFile = _vehicle.IEPCFile.FullPath
			Return retVal
		End Get
	End Property
	Public ReadOnly Property SavedInDeclarationMode As Boolean Implements IComponentInputData.SavedInDeclarationMode
	Public ReadOnly Property Manufacturer As String Implements IComponentInputData.Manufacturer
	Public ReadOnly Property Model As String Implements IComponentInputData.Model
	Public ReadOnly Property [Date] As Date Implements IComponentInputData.[Date]
	Public ReadOnly Property AppVersion As String Implements IComponentInputData.AppVersion
	Public ReadOnly Property CertificationMethod As CertificationMethod Implements IComponentInputData.CertificationMethod
	Public ReadOnly Property CertificationNumber As String Implements IComponentInputData.CertificationNumber
	Public ReadOnly Property DigestValue As DigestData Implements IComponentInputData.DigestValue
	Public ReadOnly Property ElectricMachineType As ElectricMachineType Implements IIEPCDeclarationInputData.ElectricMachineType
	Public ReadOnly Property R85RatedPower As Watt Implements IIEPCDeclarationInputData.R85RatedPower
    Public ReadOnly Property TotalRatedPowerCalculated As Watt Implements IIEPCDeclarationInputData.TotalRatedPowerCalculated
    Public ReadOnly Property Inertia As KilogramSquareMeter Implements IIEPCDeclarationInputData.Inertia
	Public ReadOnly Property DifferentialIncluded As Boolean Implements IIEPCDeclarationInputData.DifferentialIncluded
	Public ReadOnly Property DesignTypeWheelMotor As Boolean Implements IIEPCDeclarationInputData.DesignTypeWheelMotor
	Public ReadOnly Property DisengagementClutch As Boolean Implements IIEPCDeclarationInputData.DisengagementClutch
	Public ReadOnly Property NrOfDesignTypeWheelMotorMeasured As Integer? Implements IIEPCDeclarationInputData.NrOfDesignTypeWheelMotorMeasured
	Public ReadOnly Property Gears As IList(Of IGearEntry) Implements IIEPCDeclarationInputData.Gears
	Public ReadOnly Property VoltageLevels As IList(Of IElectricMotorVoltageLevel) Implements IIEPCDeclarationInputData.VoltageLevels
	Public ReadOnly Property DragCurves As IList(Of IDragCurve) Implements IIEPCDeclarationInputData.DragCurves
	Public ReadOnly Property Conditioning As TableData Implements IIEPCDeclarationInputData.Conditioning
	Public ReadOnly Property OverloadRecoveryFactor As Double Implements IIEPCEngineeringInputData.OverloadRecoveryFactor
End Class




Public Class ElectricStorageSystemWrapper
	Implements IElectricStorageSystemEngineeringInputData

	Private _vehicle As Vehicle

	Public Sub New(vehicle As Vehicle)
		_vehicle = vehicle
	End Sub

	Public ReadOnly Property ElectricStorageElements As IList(Of IElectricStorageDeclarationInputData) Implements IElectricStorageSystemDeclarationInputData.ElectricStorageElements
		Get
			Return _vehicle.ReessPacks.Select(Function(x) New ElectricStorageWrapper(x, GetPath(_vehicle.FilePath))).Cast(Of IElectricStorageDeclarationInputData).ToList()
		End Get
	End Property
	Public ReadOnly Property IElectricStorageSystemEngineeringInputData_ElectricStorageElements As IList(Of IElectricStorageEngineeringInputData) Implements IElectricStorageSystemEngineeringInputData.ElectricStorageElements
		Get
			Return _vehicle.ReessPacks.Select(Function(x) New ElectricStorageWrapper(x, GetPath(_vehicle.FilePath))).Cast(Of IElectricStorageEngineeringInputData).ToList()
		End Get
	End Property
End Class

Public Class ElectricStorageWrapper
	Implements IElectricStorageEngineeringInputData, IBatteryPackEngineeringInputData

	Public Property BatteryFile As SubPath

	Public Sub New(veh As Tuple(Of String, Integer, Integer), filePath As String)
		Count = veh.Item2
		StringId = veh.Item3
		BatteryFile = New SubPath
		BatteryFile.Init(filePath, veh.Item1)
	End Sub



	Public ReadOnly Property REESSPack As IREESSPackInputData Implements IElectricStorageEngineeringInputData.REESSPack
		Get
			Return Me
		End Get
	End Property

	Public ReadOnly Property Count As Integer Implements IElectricStorageEngineeringInputData.Count


	Public ReadOnly Property StringId As Integer Implements IElectricStorageDeclarationInputData.StringId

	Public ReadOnly Property DataSource As DataSource Implements IComponentInputData.DataSource
		Get
			Dim retVal As DataSource = New DataSource()
			retVal.SourceType = DataSourceType.JSONFile
			retVal.SourceFile = BatteryFile.FullPath
			Return retVal
		End Get
	End Property
	Public ReadOnly Property SavedInDeclarationMode As Boolean Implements IComponentInputData.SavedInDeclarationMode
	Public ReadOnly Property Manufacturer As String Implements IComponentInputData.Manufacturer
	Public ReadOnly Property Model As String Implements IComponentInputData.Model
	Public ReadOnly Property [Date] As Date Implements IComponentInputData.[Date]
	Public ReadOnly Property AppVersion As String Implements IComponentInputData.AppVersion
	Public ReadOnly Property CertificationMethod As CertificationMethod Implements IComponentInputData.CertificationMethod
	Public ReadOnly Property CertificationNumber As String Implements IComponentInputData.CertificationNumber
	Public ReadOnly Property DigestValue As DigestData Implements IComponentInputData.DigestValue
	Public ReadOnly Property MinSOC As Double? Implements IBatteryPackDeclarationInputData.MinSOC
	Public ReadOnly Property MaxSOC As Double? Implements IBatteryPackDeclarationInputData.MaxSOC
	Public ReadOnly Property DeteriorationPerformanceRatio As Double? Implements IBatteryPackDeclarationInputData.DeteriorationPerformanceRatio
	Public ReadOnly Property BatteryType As BatteryType Implements IBatteryPackDeclarationInputData.BatteryType
	Public ReadOnly Property Capacity As AmpereSecond Implements IBatteryPackDeclarationInputData.Capacity
	Public ReadOnly Property ConnectorsSubsystemsIncluded As Boolean? Implements IBatteryPackDeclarationInputData.ConnectorsSubsystemsIncluded
	Public ReadOnly Property JunctionboxIncluded As Boolean? Implements IBatteryPackDeclarationInputData.JunctionboxIncluded
	Public ReadOnly Property TestingTemperature As Kelvin Implements IBatteryPackDeclarationInputData.TestingTemperature
	Public ReadOnly Property InternalResistanceCurve As TableData Implements IBatteryPackDeclarationInputData.InternalResistanceCurve
	Public ReadOnly Property VoltageCurve As TableData Implements IBatteryPackDeclarationInputData.VoltageCurve
	Public ReadOnly Property MaxCurrentMap As TableData Implements IBatteryPackDeclarationInputData.MaxCurrentMap
	Public ReadOnly Property StorageType As REESSType Implements IREESSPackInputData.StorageType
End Class

Public Class FuelCellSystemWrapper
	Implements IFuelCellSystemEngineeringInputData

	Protected Vehicle As Vehicle
	Public Sub New(veh As Vehicle)
		Vehicle = veh
	End Sub

	Public ReadOnly Property MaxWindowSize As Meter Implements IFuelCellSystemEngineeringInputData.MaxWindowSize
		Get
			If (Vehicle.VehicleType <> VectoSimulationJobType.FCHV AndAlso Vehicle.VehicleType <> VectoSimulationJobType.FCHV_IEPC) Then
				Return Nothing
			End If

			Return Vehicle.FuelCellSystemInputData.MaxWindowSize
		End Get
	End Property

	Public ReadOnly Property FuelCellStrings As IList(Of FuelCellStringEntry(Of IFuelCellComponentEngineeringInputData)) Implements IFuelCellSystemEngineeringInputData.FuelCellStrings
		Get
			If (Vehicle.VehicleType <> VectoSimulationJobType.FCHV AndAlso Vehicle.VehicleType <> VectoSimulationJobType.FCHV_IEPC) Then
				Return Nothing
			End If
			Dim retVal As List(Of FuelCellStringEntry(Of IFuelCellComponentEngineeringInputData)) = New List(Of FuelCellStringEntry(Of IFuelCellComponentEngineeringInputData))

			retVal = Vehicle.FuelCellComponents.Select(
				Function(x) _
												 New FuelCellStringEntry(Of IFuelCellComponentEngineeringInputData) _
												 With {.Count = x.Item2,
												 .FuelCellComponent =
												 JSONInputDataFactory.ReadFuelCellComponentEngineeringInputData(
													 Path.Combine(Path.GetDirectoryName(Vehicle.FilePath), x.Item1), False)}).ToList()


			Return retVal
		End Get
	End Property
End Class



Public Class ElectricMachinesWrapper
	Implements IElectricMachinesEngineeringInputData ', IElectricMotorEngineeringInputData

	Protected Vehicle As Vehicle

	Public Sub New(veh As Vehicle)
		Vehicle = veh
	End Sub


	Public ReadOnly Property Entries As IList(Of ElectricMachineEntry(Of IElectricMotorDeclarationInputData)) Implements IElectricMachinesDeclarationInputData.Entries
		Get
			Dim retval As IList(Of ElectricMachineEntry(Of IElectricMotorDeclarationInputData)) = New List(Of ElectricMachineEntry(Of IElectricMotorDeclarationInputData))
			If (Vehicle.VehicleType = VectoSimulationJobType.BatteryElectricVehicle OrElse 
                Vehicle.VehicleType = VectoSimulationJobType.ParallelHybridVehicle OrElse 
                Vehicle.VehicleType = VectoSimulationJobType.SerialHybridVehicle OrElse 
                Vehicle.VehicleType = VectoSimulationJobType.IHPC OrElse
                Vehicle.VehicleType = VectoSimulationJobType.FCHV) Then 

				retval.Add(New ElectricMachineEntry(Of IElectricMotorDeclarationInputData) With {
					.ElectricMachine = new ElectricMachineWrapper(Vehicle.ElectricMotorFile),
					.MechanicalTransmissionEfficiency = If(IsNumeric(Vehicle.ElectricMotorMechLossMap.OriginalPath), Vehicle.ElectricMotorMechLossMap.OriginalPath.ToDouble(), double.NaN), 
					.MechanicalTransmissionLossMap = If(IsNumeric(Vehicle.ElectricMotorMechLossMap.OriginalPath), Nothing, VectoCSVFile.Read(Vehicle.ElectricMotorMechLossMap.FullPath)),
					.Position = Vehicle.ElectricMotorPosition, 
					.RatioADC = Vehicle.ElectricMotorRatio, 
					.RatioPerGear = vehicle.ElectricMotorPerGearRatios,
					.Count = Vehicle.ElectricMotorCount})
			End If

			if (Vehicle.VehicleType = VectoSimulationJobType.SerialHybridVehicle OrElse Vehicle.VehicleType = VectoSimulationJobType.IEPC_S) Then
				retval.Add(New ElectricMachineEntry(Of IElectricMotorDeclarationInputData) With {
							  .ElectricMachine = new ElectricMachineWrapper(Vehicle.GenSetEMFile),
							  .MechanicalTransmissionEfficiency = If(IsNumeric(Vehicle.GenSetMechLossMap.OriginalPath), Vehicle.GenSetMechLossMap.OriginalPath.ToDouble(), double.NaN), 
							  .MechanicalTransmissionLossMap = If(IsNumeric(Vehicle.GenSetMechLossMap.OriginalPath), Nothing,VectoCSVFile.Read(Vehicle.GenSetMechLossMap.FullPath)),
							  .Position = PowertrainPosition.GEN, 
							  .RatioADC = Vehicle.GenSetRatio, 
							  .Count = Vehicle.GenSetCount})
			End If
			Return retval
		End Get
	End Property
	Public ReadOnly Property IElectricMachinesEngineeringInputData_Entries As IList(Of ElectricMachineEntry(Of IElectricMotorEngineeringInputData)) Implements IElectricMachinesEngineeringInputData.Entries
		Get
			Dim retval As IList(Of ElectricMachineEntry(Of IElectricMotorEngineeringInputData)) =  New List(Of ElectricMachineEntry(Of IElectricMotorEngineeringInputData))
			
			If (Vehicle.VehicleType = VectoSimulationJobType.BatteryElectricVehicle OrElse 
                Vehicle.VehicleType = VectoSimulationJobType.ParallelHybridVehicle OrElse 
                Vehicle.VehicleType = VectoSimulationJobType.SerialHybridVehicle OrElse 
                Vehicle.VehicleType = VectoSimulationJobType.IHPC OrElse 
                Vehicle.VehicleType = VectoSimulationJobType.FCHV) Then 

				retval.Add(New ElectricMachineEntry(Of IElectricMotorEngineeringInputData) With {
					.ElectricMachine = new ElectricMachineWrapper(Vehicle.ElectricMotorFile),
					.MechanicalTransmissionEfficiency = If(IsNumeric(Vehicle.ElectricMotorMechLossMap.OriginalPath), Vehicle.ElectricMotorMechLossMap.OriginalPath.ToDouble(), double.NaN), 
					.MechanicalTransmissionLossMap = If(IsNumeric(Vehicle.ElectricMotorMechLossMap.OriginalPath) OrElse String.IsNullOrWhiteSpace(Vehicle.ElectricMotorMechLossMap.OriginalPath), Nothing, VectoCSVFile.Read(Vehicle.ElectricMotorMechLossMap.FullPath)),
					.Position = Vehicle.ElectricMotorPosition, 
					.RatioADC = Vehicle.ElectricMotorRatio, 
					.RatioPerGear = Vehicle.ElectricMotorPerGearRatios,
					.Count = Vehicle.ElectricMotorCount})
			End If

			if (Vehicle.VehicleType = VectoSimulationJobType.SerialHybridVehicle  OrElse Vehicle.VehicleType = VectoSimulationJobType.IEPC_S) Then
				retval.Add(New ElectricMachineEntry(Of IElectricMotorEngineeringInputData) With {
							  .ElectricMachine = new ElectricMachineWrapper(Vehicle.GenSetEMFile),
							  .MechanicalTransmissionEfficiency = If(IsNumeric(Vehicle.GenSetMechLossMap.OriginalPath), Vehicle.GenSetMechLossMap.OriginalPath.ToDouble(), double.NaN), 
							  .MechanicalTransmissionLossMap = If(IsNumeric(Vehicle.GenSetMechLossMap.OriginalPath), Nothing,VectoCSVFile.Read(Vehicle.GenSetMechLossMap.FullPath)),
							  .Position = PowertrainPosition.GEN, 
							  .RatioADC = Vehicle.GenSetRatio, 
							  .Count = Vehicle.GenSetCount})
			End If
			Return retval
		End Get
	End Property

	
End Class

Public Class ElectricMachineWrapper
	Implements  IElectricMotorEngineeringInputData

	Private EMFile As SubPath

	Public Sub New(em As SubPath)
		EMFile = em
	End Sub

	Public ReadOnly Property DataSource As DataSource Implements IComponentInputData.DataSource
		Get
			Return New DataSource() With {
								.SourceFile = EMFile.FullPath}
		End Get
	End Property
	Public ReadOnly Property SavedInDeclarationMode As Boolean Implements IComponentInputData.SavedInDeclarationMode
	Public ReadOnly Property Manufacturer As String Implements IComponentInputData.Manufacturer
	Public ReadOnly Property Model As String Implements IComponentInputData.Model
	Public ReadOnly Property [Date] As Date Implements IComponentInputData.[Date]
	Public ReadOnly Property AppVersion As String Implements IComponentInputData.AppVersion
	Public ReadOnly Property CertificationMethod As CertificationMethod Implements IComponentInputData.CertificationMethod
	Public ReadOnly Property CertificationNumber As String Implements IComponentInputData.CertificationNumber
	Public ReadOnly Property DigestValue As DigestData Implements IComponentInputData.DigestValue

	Public ReadOnly Property VoltageLevels As IList(Of IElectricMotorVoltageLevel) Implements IElectricMotorDeclarationInputData.VoltageLevels
	Public ReadOnly Property ElectricMachineType As ElectricMachineType Implements IElectricMotorDeclarationInputData.ElectricMachineType
	Public ReadOnly Property R85RatedPower As Watt Implements IElectricMotorDeclarationInputData.R85RatedPower
	Public ReadOnly Property Inertia As KilogramSquareMeter Implements IElectricMotorDeclarationInputData.Inertia
	Public ReadOnly Property DcDcConverterIncluded As Boolean Implements IElectricMotorDeclarationInputData.DcDcConverterIncluded
	Public ReadOnly Property IHPCType As String Implements IElectricMotorDeclarationInputData.IHPCType
	Public ReadOnly Property DragCurve As TableData Implements IElectricMotorDeclarationInputData.DragCurve
	Public ReadOnly Property Conditioning As TableData Implements IElectricMotorDeclarationInputData.Conditioning
	Public ReadOnly Property OverloadRecoveryFactor As Double Implements IElectricMotorEngineeringInputData.OverloadRecoveryFactor

End Class