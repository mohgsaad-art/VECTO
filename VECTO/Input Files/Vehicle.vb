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
Imports TUGraz.VECTO.Input_Files
Imports TUGraz.VectoCommon.InputData
Imports TUGraz.VectoCommon.Models
Imports TUGraz.VectoCommon.Utils
Imports TUGraz.VectoCore.InputData.FileIO.JSON
Imports TUGraz.VectoCore.InputData.Impl
Imports TUGraz.VectoCore.InputData.Reader.DataObjectAdapter
Imports TUGraz.VectoCore.Models.Declaration
Imports TUGraz.VectoCore.Models.SimulationComponent.Data
Imports TUGraz.VectoCore.Models.SimulationComponent.Impl
Imports TUGraz.VectoCore.Utils

<CustomValidation(GetType(Vehicle), "ValidateVehicle")>
Public Class Vehicle
	Implements IVehicleEngineeringInputData, IVehicleDeclarationInputData, IRetarderInputData, IPTOTransmissionInputData, 
				IAngledriveInputData, IAirdragEngineeringInputData, IAdvancedDriverAssistantSystemDeclarationInputData, IAdvancedDriverAssistantSystemsEngineering,
				IVehicleComponentsEngineering, IVehicleComponentsDeclaration, IAxlesEngineeringInputData, IAxlesDeclarationInputData

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
    Public ReadOnly PtoCycleDriving As SubPath
	Public torqueLimitsList As List(Of ITorqueLimitInputData)
	Public VehicleidlingSpeed As PerSecond
	Public legClass As LegislativeClass
	Public VehicleHeight As Double

    public EcoRolltype as EcoRollType
    public PCC as PredictiveCruiseControlType
    public EngineStop as Boolean

    public VehicleTankSystem as TankSystem?

    public GearDuringPTODrive As UInteger?
    Public EngineSpeedDuringPTODrive As PerSecond

    Public Sub New()
		_path = ""
		_filePath = ""
		CrossWindCorrectionFile = New SubPath

		RetarderLossMapFile = New SubPath
		AngledriveLossMapFile = New SubPath()

		Axles = New List(Of AxleInputData)
		torqueLimitsList = New List(Of ITorqueLimitInputData)
		PtoLossMap = New SubPath()
		PtoCycleStandstill = New SubPath()
        PtoCycleDriving = new SubPath()
		SetDefault()
	End Sub


	' ReSharper disable once UnusedMember.Global  -- used for Validation
	Public Shared Function ValidateVehicle(vehicle As Vehicle, validationContext As ValidationContext) As ValidationResult

		Dim vehicleData As VehicleData
		Dim airdragData As AirdragData
		Dim retarderData As RetarderData
		Dim ptoData As PTOData = Nothing
		Dim angledriveData As AngledriveData

		Dim modeService As VectoValidationModeServiceContainer =
				TryCast(validationContext.GetService(GetType(VectoValidationModeServiceContainer)), 
						VectoValidationModeServiceContainer)
		Dim mode As ExecutionMode = If(modeService Is Nothing, ExecutionMode.Declaration, modeService.Mode)
		Dim emsCycle As Boolean = (modeService IsNot Nothing) AndAlso modeService.IsEMSCycle
		Dim gbxType As GearboxType? = If(modeService Is Nothing, Nothing, modeService.GearboxType)

		Try
			If mode = ExecutionMode.Declaration Then
				Dim doa As DeclarationDataAdapter = New DeclarationDataAdapter()
				Dim segment As Segment = DeclarationData.Segments.Lookup(vehicle.VehicleCategory, vehicle.AxleConfiguration,
																		vehicle.GrossVehicleMassRating, vehicle.CurbMassChassis, false)
				vehicleData = doa.CreateVehicleData(vehicle, segment.Missions.First(),
													segment.Missions.First().Loadings.First().Value, true)
				airdragData = doa.CreateAirdragData(vehicle, segment.Missions.First(), segment)
				retarderData = doa.CreateRetarderData(vehicle)
				angledriveData = doa.CreateAngledriveData(vehicle)
				ptoData = doa.CreatePTOTransmissionData(vehicle)
			Else
				Dim doa As EngineeringDataAdapter = New EngineeringDataAdapter()
				vehicleData = doa.CreateVehicleData(vehicle)
				airdragData = doa.CreateAirdragData(vehicle, vehicle)
				retarderData = doa.CreateRetarderData(vehicle)
				angledriveData = doa.CreateAngledriveData(vehicle)
				ptoData = doa.CreatePTOTransmissionData(vehicle)
			End If

			Dim result As IList(Of ValidationResult) =
					vehicleData.Validate(If(Cfg.DeclMode, ExecutionMode.Declaration, ExecutionMode.Engineering), gbxType, emsCycle)
			If result.Any() Then
				Return _
					New ValidationResult("Vehicle Configuration is invalid. ",
										result.Select(Function(r) r.ErrorMessage + String.Join(Environment.NewLine, r.MemberNames)).ToList())
			End If

			result = airdragData.Validate(If(Cfg.DeclMode, ExecutionMode.Declaration, ExecutionMode.Engineering), gbxType,
										emsCycle)
			If result.Any() Then
				Return _
					New ValidationResult("Airdrag Configuration is invalid. ",
										result.Select(Function(r) r.ErrorMessage + String.Join(Environment.NewLine, r.MemberNames)).ToList())
			End If

			result = retarderData.Validate(If(Cfg.DeclMode, ExecutionMode.Declaration, ExecutionMode.Engineering), gbxType,
											emsCycle)
			If result.Any() Then
				Return _
					New ValidationResult("Retarder Configuration is invalid. ",
										result.Select(Function(r) r.ErrorMessage + String.Join(Environment.NewLine, r.MemberNames)).ToList())
			End If

			If vehicle.AngledriveType = AngledriveType.SeparateAngledrive Then
				result = angledriveData.Validate(If(Cfg.DeclMode, ExecutionMode.Declaration, ExecutionMode.Engineering), gbxType,
												emsCycle)
				If result.Any() Then
					Return _
						New ValidationResult("AngleDrive Configuration is invalid. ",
											result.Select(Function(r) r.ErrorMessage + String.Join(Environment.NewLine, r.MemberNames)).ToList())
				End If
			End If

			If Not vehicle.PTOTransmissionType = "None" Then
				result = ptoData.Validate(If(Cfg.DeclMode, ExecutionMode.Declaration, ExecutionMode.Engineering), gbxType, emsCycle)
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

		AngledriveType = AngledriveType.None
		AngledriveLossMapFile.Clear()
		AngledriveRatio = 1

		PtoType = PTOTransmission.NoPTO
		PtoLossMap.Clear()
		PtoCycleStandstill.Clear()
	    PtoCycleDriving.Clear()

		Axles.Clear()
		VehicleCategory = VehicleCategory.RigidTruck
		MassMax = 0
		AxleConfiguration = AxleConfiguration.AxleConfig_4x2

		SavedInDeclMode = False
	End Sub


	Public Function SaveFile() As Boolean
		SavedInDeclMode = Cfg.DeclMode

		Dim validationResults As IList(Of ValidationResult) =
				Validate(If(Cfg.DeclMode, ExecutionMode.Declaration, ExecutionMode.Engineering), Nothing, False)

		If validationResults.Count > 0 Then
			Dim messages As IEnumerable(Of String) =
					validationResults.Select(Function(r) r.ErrorMessage + String.Join(", ", r.MemberNames.Distinct()))
			MsgBox("Invalid input." + Environment.NewLine + String.Join(Environment.NewLine, messages), MsgBoxStyle.OkOnly,
					"Failed to save vehicle")
			Return False
		End If

		Try
			Dim writer As JSONFileWriter = JSONFileWriter.Instance
			writer.SaveVehicle(Me, Me, Me, Me, Me, _filePath, cfg.DeclMode)
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
			Dim retVal As DataSource =  New DataSource() 
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


	Public ReadOnly Property [Date] As String Implements IComponentInputData.[Date]
		Get
			Return Now.ToUniversalTime().ToString("o")
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
    get
            Return ""
    End Get
    End Property

    Public ReadOnly Property ExemptedVehicle As Boolean Implements IVehicleDeclarationInputData.ExemptedVehicle
	get
			Return false
	End Get
	End Property

	Public ReadOnly Property VIN As String Implements IVehicleDeclarationInputData.VIN
		Get
			Return TUGraz.VectoCore.Configuration.Constants.NOT_AVAILABLE
		End Get
	End Property

	Public ReadOnly Property LegislativeClass As LegislativeClass Implements IVehicleEngineeringInputData.LegislativeClass
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

	Public ReadOnly Property Height As Meter Implements IVehicleEngineeringInputData.Height
		Get
			Return VehicleHeight.SI(Of Meter)()
		End Get
	End Property

	Public ReadOnly Property IVehicleEngineeringInputData_Components As IVehicleComponentsEngineering Implements IVehicleEngineeringInputData.Components
    get
            Return me
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
    get
            return nothing
    End Get
    End Property

    Public ReadOnly Property AuxiliaryInputData As IAuxiliariesEngineeringInputData Implements IVehicleComponentsEngineering.AuxiliaryInputData
	get
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
	get
			Return Me
	End Get
	End Property

	Public ReadOnly Property AxleWheels As IAxlesEngineeringInputData Implements IVehicleComponentsEngineering.AxleWheels
	get
			Return me
	End Get
	End Property

	Public ReadOnly Property VocationalVehicle As Boolean Implements IVehicleDeclarationInputData.VocationalVehicle
	get
			Return DeclarationData.Vehicle.VocationalVehicleDefault
	End Get
	End Property

	Public ReadOnly Property SleeperCab As Boolean? Implements IVehicleDeclarationInputData.SleeperCab
	get
			Return DeclarationData.Vehicle.SleeperCabDefault
	End Get
	End Property

	Public ReadOnly Property TankSystem As TankSystem? Implements IVehicleDeclarationInputData.TankSystem
	get
			Return vehicleTankSystem
	End Get
	End Property

	Public ReadOnly Property IVehicleEngineeringInputData_ADAS As IAdvancedDriverAssistantSystemsEngineering Implements IVehicleEngineeringInputData.ADAS
	get
			Return me
	End Get
	End Property

	Public ReadOnly Property ADAS As IAdvancedDriverAssistantSystemDeclarationInputData Implements IVehicleDeclarationInputData.ADAS
	get
			return Me
	End Get
	End Property

    Public ReadOnly Property PTO_DriveGear As GearshiftPosition Implements IVehicleEngineeringInputData.PTO_DriveGear
    get
            return  If(gearDuringPTODrive.HasValue, new GearshiftPosition(GearDuringPTODrive.Value), Nothing)
    End Get
    End Property

    Public ReadOnly Property PTO_DriveEngineSpeed As PerSecond Implements IVehicleEngineeringInputData.PTO_DriveEngineSpeed
    get
            Return EngineSpeedDuringPTODrive
    End Get
    End Property

    Public ReadOnly Property ZeroEmissionVehicle As Boolean Implements IVehicleDeclarationInputData.ZeroEmissionVehicle
	get
			Return DeclarationData.Vehicle.ZeroEmissionVehicleDefault
	End Get
	End Property

	Public ReadOnly Property HybridElectricHDV As Boolean Implements IVehicleDeclarationInputData.HybridElectricHDV
	get
			return DeclarationData.Vehicle.HybridElectricHDVDefault
	End Get
	End Property

	Public ReadOnly Property DualFuelVehicle As Boolean Implements IVehicleDeclarationInputData.DualFuelVehicle
	get
			return DeclarationData.Vehicle.DualFuelVehicleDefault
	End Get
	End Property

	Public ReadOnly Property MaxNetPower1 As Watt Implements IVehicleDeclarationInputData.MaxNetPower1
	get
			Return Nothing
	End Get
	End Property

	Public ReadOnly Property MaxNetPower2 As Watt Implements IVehicleDeclarationInputData.MaxNetPower2
	get
			Return Nothing
	End Get
	End Property

	Public ReadOnly Property Components As IVehicleComponentsDeclaration Implements IVehicleDeclarationInputData.Components
	get
			Return Me
	End Get
	End Property

	Public ReadOnly Property EngineStopStart As Boolean Implements IAdvancedDriverAssistantSystemDeclarationInputData.EngineStopStart
	get
			return EngineStop
	End Get
	End Property

	Public ReadOnly Property EcoRoll As EcoRollType Implements IAdvancedDriverAssistantSystemDeclarationInputData.EcoRoll
	get
			return EcoRolltype
	End Get
	End Property

	
	Public ReadOnly Property PredictiveCruiseControl As PredictiveCruiseControlType Implements IAdvancedDriverAssistantSystemDeclarationInputData.PredictiveCruiseControl
	get
			Return PCC
	End Get
	End Property

    Public ReadOnly Property ATEcoRollReleaseLockupClutch As Boolean? Implements IAdvancedDriverAssistantSystemDeclarationInputData.ATEcoRollReleaseLockupClutch
        Get
            Return EcoRollReleaseLockupClutch
        End Get
    End Property

    Public Property EcoRollReleaseLockupClutch As Boolean

    Public ReadOnly Property NumSteeredAxles As Integer? Implements IAxlesDeclarationInputData.NumSteeredAxles
        get
            return nothing
        End Get
    End Property

    Public ReadOnly Property IAxlesEngineeringInputData_DataSource As DataSource Implements IAxlesEngineeringInputData.DataSource
	get
		Return New DataSource() With {.SourceType = DataSourceType.JSONFile}
	End Get
	End Property

    Public ReadOnly Property IAdvancedDriverAssistantSystemsEngineering_DataSource As DataSource Implements IAdvancedDriverAssistantSystemsEngineering.DataSource
End Class
