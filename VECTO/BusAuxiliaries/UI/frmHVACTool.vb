Imports System.Collections.Generic
Imports System.Windows.Forms
Imports System.ComponentModel
Imports System.Drawing
Imports System.Globalization
Imports System.IO
Imports TUGraz.VectoCommon.Utils
Imports TUGraz.VectoCore.InputData.FileIO.JSON
Imports TUGraz.VectoCore.Models.BusAuxiliaries.DownstreamModules.Impl.HVAC
Imports TUGraz.VectoCore.Models.BusAuxiliaries.Util
Imports TUGraz.VectoCore.Models.Declaration
Imports TUGraz.VectoCore.OutputData.FileIO

Public Class frmHVACTool
	'Fields
	Private captureDiagnostics As Boolean
	Private busDatabasePath As String
	Private ahsmFilePath As String
	'Private buses As IBusDatabase
	Private ssmTOOL As SSMTOOL
	Private TabColors As Dictionary(Of TabPage, Color) = New Dictionary(Of TabPage, Color)()
	Private DefaultCategories As String() = {"Cooling", "Heating", "Insulation", "Ventiliation"}
	Private vectoFile As String = String.Empty
	Private UserHitCancel As Boolean = False
	Private UserHitSave As Boolean = False
	Private IsAddingBus As Boolean = False
	Private IsUpdatingBus As Boolean = False
	'Private busesList As BindingList(Of IBus)

	'Helpers
	Public Sub UpdateButtonText()

		If txtIndex.Text = String.Empty Then
			btnUpdate.Text = "Add"
		Else
			btnUpdate.Text = "Update"
		End If
	End Sub

	Private Function ValidateSSMTOOLFileName(filename As String) As Boolean

		Dim message As String = String.Empty

		If Not FilePathUtils.ValidateFilePath(filename, ".ahsm", message) Then
			MessageBox.Show(message)
		End If

		Return True
	End Function

	Private Sub BindGrid()

		'Dim _
		'	gvTechListBinding As _
		'		New BindingList(Of ISSMTechnology)(
		'			ssmTOOL.TechList.TechLines.OrderBy(Function(o) o.Category).ThenBy(Function(t) t.BenefitName).ToList())
		'Me.gvTechBenefitLines.DataSource = gvTechListBinding
	End Sub

	Private Function GetCategories() As List(Of String)

        'If Not ssmTOOL Is Nothing AndAlso Not ssmTOOL.TechList Is Nothing AndAlso ssmTOOL.TechList.TechLines.Count > 0 Then

        '	'Fuse Lists          
        '	Dim fusedList As List(Of String) = DefaultCategories.ToList()

        '	For Each s As String In ssmTOOL.TechList.TechLines.Select(Function(sel) sel.Category)

        '		If Not fusedList.Contains(s) Then
        '			fusedList.Add(s)
        '		End If

        '	Next

        '	Return fusedList.OrderBy(Function(o) o.ToString()).ToList()

        'Else

        Return New List(Of String)(DefaultCategories)

        'End If
    End Function

	'Constructors
	Public Sub New(busDatabasePath As String, ahsmFilePath As String, vectoFilePath As String,
					Optional useDefaults As Boolean = False)

		' This call is required by the designer.
		InitializeComponent()

		vectoFile = vectoFilePath

		'Add any initialization after the InitializeComponent() call.
		Me.busDatabasePath = busDatabasePath
		Me.ahsmFilePath = ahsmFilePath

		ssmTOOL = New SSMTOOL(SSMInputData.ReadFile(ahsmFilePath, Nothing, DeclarationData.BusAuxiliaries.DefaultEnvironmentalConditions)) ' , New HVACConstants, False, useDefaults)
		'originalssmTOOL = New SSMTOOL(SSMInputData.ReadFile(ahsmFilePath, Nothing, DeclarationData.BusAuxiliaries.DefaultEnvironmentalConditions))  ' ahsmFilePath, New HVACConstants, False, useDefaults)

		'If IO.File.Exists(ahsmFilePath) Then
		'	If ssmTOOL.Load(ahsmFilePath) AndAlso originalssmTOOL.Load(ahsmFilePath) Then
		'		Timer1.Enabled = True
		'	Else
		'		MessageBox.Show(
		'			"The file format for the Steady State Model (.AHSM) was corrupted or is an alpha version. Please refer to the documentation or help to discover more.")
		'		Timer1.Enabled = False
		'	End If
		'Else
			Timer1.Enabled = True
		'End If

		'setupBuses()
		setupControls()
		setupBindings()
	End Sub

	'Setup Methods
	'Private Sub setupBuses()

	'	'Setup Buses
	'	buses = New BusDatabase()
	'	If Not buses.Initialise(busDatabasePath) Then
	'		MessageBox.Show("Problems initialising the Bus Database, some buses may not appear")
	'	End If

	'	busesList = New BindingList(Of IBus)(buses.GetBuses(String.Empty, True))

	'	cboBuses.DataSource = busesList
	'	cboBuses.DisplayMember = "Model"
	'End Sub

	Private Sub setupControls()

		'gvTechBenefitLines
		gvTechBenefitLines.AutoGenerateColumns = False

		Dim cIndex As Integer

		'Column - Category
		cIndex = gvTechBenefitLines.Columns.Add("Category", "Category")
		gvTechBenefitLines.Columns(cIndex).DataPropertyName = "Category"
		gvTechBenefitLines.Columns(cIndex).Width = 70
		gvTechBenefitLines.Columns(cIndex).ReadOnly = True
		gvTechBenefitLines.Columns(cIndex).HeaderCell.Style.Alignment = DataGridViewContentAlignment.TopCenter
		gvTechBenefitLines.Columns(cIndex).HeaderCell.Style.Padding = New Padding(1, 2, 1, 1)

		'Column - BenefitName
		cIndex = gvTechBenefitLines.Columns.Add("BenefitName", "BenefitName")
		gvTechBenefitLines.Columns(cIndex).DataPropertyName = "BenefitName"
		gvTechBenefitLines.Columns(cIndex).Width = 330
		gvTechBenefitLines.Columns(cIndex).ReadOnly = True
		gvTechBenefitLines.Columns(cIndex).HeaderCell.Style.Alignment = DataGridViewContentAlignment.TopCenter
		gvTechBenefitLines.Columns(cIndex).HeaderCell.Style.Padding = New Padding(1, 2, 1, 1)

		'Column - H
		cIndex = gvTechBenefitLines.Columns.Add("H", "H")
		gvTechBenefitLines.Columns(cIndex).DataPropertyName = "H"
		gvTechBenefitLines.Columns(cIndex).Width = 60
		gvTechBenefitLines.Columns(cIndex).ReadOnly = True
		gvTechBenefitLines.Columns(cIndex).HeaderCell.Style.Alignment = DataGridViewContentAlignment.TopCenter
		gvTechBenefitLines.Columns(cIndex).HeaderCell.Style.Padding = New Padding(1, 2, 1, 1)

		'Column - VH
		cIndex = gvTechBenefitLines.Columns.Add("VH", "VH")
		gvTechBenefitLines.Columns(cIndex).DataPropertyName = "VH"
		gvTechBenefitLines.Columns(cIndex).Width = 60
		gvTechBenefitLines.Columns(cIndex).ReadOnly = True
		gvTechBenefitLines.Columns(cIndex).HeaderCell.Style.Alignment = DataGridViewContentAlignment.TopCenter
		gvTechBenefitLines.Columns(cIndex).HeaderCell.Style.Padding = New Padding(1, 2, 1, 1)


		'Column - VV
		cIndex = gvTechBenefitLines.Columns.Add("VV", "VV")
		gvTechBenefitLines.Columns(cIndex).DataPropertyName = "VV"
		gvTechBenefitLines.Columns(cIndex).Width = 60
		gvTechBenefitLines.Columns(cIndex).ReadOnly = True
		gvTechBenefitLines.Columns(cIndex).HeaderCell.Style.Alignment = DataGridViewContentAlignment.TopCenter
		gvTechBenefitLines.Columns(cIndex).HeaderCell.Style.Padding = New Padding(1, 2, 1, 1)

		'Column - VC
		cIndex = gvTechBenefitLines.Columns.Add("VC", "VC")
		gvTechBenefitLines.Columns(cIndex).DataPropertyName = "VC"
		gvTechBenefitLines.Columns(cIndex).Width = 60
		gvTechBenefitLines.Columns(cIndex).ReadOnly = True
		gvTechBenefitLines.Columns(cIndex).HeaderCell.Style.Alignment = DataGridViewContentAlignment.TopCenter
		gvTechBenefitLines.Columns(cIndex).HeaderCell.Style.Padding = New Padding(1, 2, 1, 1)

		'Column - C
		cIndex = gvTechBenefitLines.Columns.Add("C", "C")
		gvTechBenefitLines.Columns(cIndex).DataPropertyName = "C"
		gvTechBenefitLines.Columns(cIndex).Width = 60
		gvTechBenefitLines.Columns(cIndex).ReadOnly = True
		gvTechBenefitLines.Columns(cIndex).HeaderCell.Style.Alignment = DataGridViewContentAlignment.TopCenter
		gvTechBenefitLines.Columns(cIndex).HeaderCell.Style.Padding = New Padding(1, 2, 1, 1)

		'Column - OnVehicle

		Dim onV As New DataGridViewCheckBoxColumn()

		cIndex = gvTechBenefitLines.Columns.Add(onV)
		gvTechBenefitLines.Columns(cIndex).Name = "OnVehicle"
		gvTechBenefitLines.Columns(cIndex).DataPropertyName = "OnVehicle"
		gvTechBenefitLines.Columns(cIndex).Width = 60
		gvTechBenefitLines.Columns(cIndex).ReadOnly = True
		gvTechBenefitLines.Columns(cIndex).HeaderCell.Style.Alignment = DataGridViewContentAlignment.TopCenter
		gvTechBenefitLines.Columns(cIndex).HeaderCell.Style.Padding = New Padding(1, 2, 1, 1)


		Dim deleteColumn As New DeleteColumn
		With deleteColumn
			.HeaderText = ""
			.ToolTipText = "Delete this row"
			.Name = "Delete"
			.Width = 20
			.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
		End With
		gvTechBenefitLines.Columns.Add(deleteColumn)

		'Techlist Edit Panel
		cboCategory.DataSource = GetCategories()
		cboUnits.DataSource = {"Fraction"}
		cboLineType.DataSource = {"Normal", "ActiveVentilation"}

		'txtEC_EnvironmentConditionsFilePath.Tag = ssmTOOL.SSMInputs.EnvironmentalConditions.EnviromentalConditions_BatchFile
		'txtEC_EnvironmentConditionsFilePath.Text = GetRelativePath(ssmTOOL.SSMInputs.EnvironmentalConditions.EnviromentalConditions_BatchFile,
		'													Path.GetDirectoryName(vectoFile))
		txtEC_EnvironmentConditionsFilePath.ReadOnly = True
		btnEnvironmentConditionsSource.Enabled = False
		btnOpenAenv.Enabled = False

		btnNewBus.Tag = "New"
		btnEditBus.Tag = "Edit"

		BusParamGroupEdit.Location = New Drawing.Point(30, 51)
	End Sub

	Private Sub setupBindings()

		UpdateButtonText()

		'TechBenefitLines
		BindGrid()

		'Bus Parameterisation
		txtBusModel.DataBindings.Add("Text", ssmTOOL.SSMInputs, "BP_BusModel", False, DataSourceUpdateMode.OnPropertyChanged)
		txtRegisteredPassengers.DataBindings.Add("Text", ssmTOOL.SSMInputs, "BP_NumberOfPassengers", False,
												DataSourceUpdateMode.OnPropertyChanged)
		cmbBusFloorType.DataBindings.Add("Text", ssmTOOL.SSMInputs, "BP_BusFloorType", False,
										DataSourceUpdateMode.OnPropertyChanged)
		chkIsDoubleDecker.DataBindings.Add("Checked", ssmTOOL.SSMInputs, "BP_DoubleDecker", False,
											DataSourceUpdateMode.OnPropertyChanged)
	    dim bLength As Binding = New Binding("Text", ssmTOOL.SSMInputs, "BP_BusLength")
	    AddHandler bLength.Parse, New ConvertEventHandler(AddressOf TextToSI(of Meter))
	    AddHandler blength.Format , New ConvertEventHandler(AddressOf SIToText)
        txtBusLength.DataBindings.Add(bLength)

	    dim bWidth As Binding = New Binding("Text", ssmTOOL.SSMInputs, "BP_BusWidth")
	    AddHandler bWidth.Parse, New ConvertEventHandler(AddressOf TextToSI(of Meter))
	    AddHandler bWidth.Format , New ConvertEventHandler(AddressOf SIToText)
        txtBusWidth.DataBindings.Add(bWidth)

        dim bHeight As Binding = New Binding("Text", ssmTOOL.SSMInputs, "BP_BusHeight")
	    AddHandler bHeight.Parse, New ConvertEventHandler(AddressOf TextToSI(of Meter))
	    AddHandler bHeight.Format , New ConvertEventHandler(AddressOf SIToText)
		txtBusHeight.DataBindings.Add(bHeight)

	    dim bFloorSurface As Binding = New Binding("Text", ssmTOOL.SSMInputs, "BP_BusFloorSurfaceArea", False,
                                                  DataSourceUpdateMode.OnPropertyChanged)
	    AddHandler bFloorSurface.Parse, New ConvertEventHandler(AddressOf TextToSI(of SquareMeter))
	    AddHandler bFloorSurface.Format , New ConvertEventHandler(AddressOf SIToText)
		txtBusFloorSurfaceArea.DataBindings.Add(bFloorSurface)

	    dim bWindowSF As Binding = New Binding("Text", ssmTOOL.SSMInputs, "BP_BusWindowSurface")
	    AddHandler bWindowSF.Parse, New ConvertEventHandler(AddressOf TextToSI(of SquareMeter))
	    AddHandler bWindowSF.Format , New ConvertEventHandler(AddressOf SIToText)
		txtBusWindowSurfaceArea.DataBindings.Add(bWindowSF)

	    dim bBusSF As Binding = New Binding("Text", ssmTOOL.SSMInputs, "BP_BusSurfaceArea")
	    AddHandler bBusSF.Parse, New ConvertEventHandler(AddressOf TextToSI(of SquareMeter))
	    AddHandler bBusSF.Format , New ConvertEventHandler(AddressOf SIToText)
		txtBusSurfaceArea.DataBindings.Add(bBusSF)

	    dim bBusVol As Binding = New Binding("Text", ssmTOOL.SSMInputs, "BP_BusVolume")
	    AddHandler bBusVol.Parse, New ConvertEventHandler(AddressOf TextToSI(of CubicMeter))
	    AddHandler bBusVol.Format , New ConvertEventHandler(AddressOf SIToText)
		txtBusVolume.DataBindings.Add(bBusVol)

		'Boundary Conditions
		txtBC_GFactor.DataBindings.Add("Text", ssmTOOL.SSMInputs, "BC_GFactor")
		txtBC_SolarClouding.DataBindings.Add("Text", ssmTOOL.SSMInputs, "BC_SolarClouding")

	    dim bPassHeat As Binding = New Binding("Text", ssmTOOL.SSMInputs, "BC_HeatPerPassengerIntoCabinW")
	    AddHandler bPassHeat.Parse, New ConvertEventHandler(AddressOf TextToSI(of Watt))
	    AddHandler bPassHeat.Format , New ConvertEventHandler(AddressOf SIToText)
		txtBC_HeatPerPassengerIntoCabinW.DataBindings.Add(bPassHeat)

	    dim bPassBT As Binding = New Binding("Text", ssmTOOL.SSMInputs, "BC_PassengerBoundaryTemperature")
	    AddHandler bPassBT.Parse, New ConvertEventHandler(AddressOf TextToSIKelvin)
	    AddHandler bPassBT.Format , New ConvertEventHandler(AddressOf SIKelvinToText)
		txtBC_PassengerBoundaryTemperature.DataBindings.Add(bPassBT)

	    dim bPassLowFloor As Binding = New Binding("Text", ssmTOOL.SSMInputs, "BC_PassengerDensityLowFloor")
	    AddHandler bPassLowFloor.Parse, New ConvertEventHandler(AddressOf TextToSI(of PerSquareMeter))
	    AddHandler bPassLowFloor.Format , New ConvertEventHandler(AddressOf SIToText)
		txtBC_PassengerDensityLowFloor.DataBindings.Add(bPassLowFloor)

	    dim bPassSemi As Binding = New Binding("Text", ssmTOOL.SSMInputs, "BC_PassengerDensitySemiLowFloor")
	    AddHandler bPassSemi.Parse, New ConvertEventHandler(AddressOf TextToSI(of PerSquareMeter))
	    AddHandler bPassSemi.Format , New ConvertEventHandler(AddressOf SIToText)
		txtBC_PassengerDensitySemiLowFloor.DataBindings.Add(bPassSemi)

	    dim bPassRaised As Binding = New Binding("Text", ssmTOOL.SSMInputs, "BC_PassengerDensityRaisedFloor")
	    AddHandler bPassRaised.Parse, New ConvertEventHandler(AddressOf TextToSI(of PerSquareMeter))
	    AddHandler bPassRaised.Format , New ConvertEventHandler(AddressOf SIToText)
		txtBC_PassengerDensityRaisedFloor.DataBindings.Add(bPassRaised)

	   
		txtBC_CalculatedPassengerNumber.DataBindings.Add("Text", ssmTOOL.SSMInputs, "BC_CalculatedPassengerNumber")

	    dim bUVal As Binding = New Binding("Text", ssmTOOL.SSMInputs, "BC_UValues")
	    AddHandler bUVal.Parse, New ConvertEventHandler(AddressOf TextToSI(of WattPerKelvinSquareMeter))
	    AddHandler bUVal.Format , New ConvertEventHandler(AddressOf SIToText)
		txtBC_UValues.DataBindings.Add(bUVal)

	    dim bPassBdT As Binding = New Binding("Text", ssmTOOL.SSMInputs, "BC_HeatingBoundaryTemperature")
	    AddHandler bPassBdT.Parse, New ConvertEventHandler(AddressOf TextToSIKelvin)
	    AddHandler bPassBdT.Format , New ConvertEventHandler(AddressOf SIKelvinToText)
		txtBC_HeatingBoundaryTemperature.DataBindings.Add(bPassBdT)

	    dim bBdTCool As Binding = New Binding("Text", ssmTOOL.SSMInputs, "BC_CoolingBoundaryTemperature")
	    AddHandler bBdTCool.Parse, New ConvertEventHandler(AddressOf TextToSIKelvin)
	    AddHandler bBdTCool.Format , New ConvertEventHandler(AddressOf SIKelvinToText)
		txtBC_CoolingBoundaryTemperature.DataBindings.Add(bBdTCool)

	    dim bCoolingOff As Binding = New Binding("Text", ssmTOOL.SSMInputs, "BC_TemperatureCoolingTurnsOff")
	    AddHandler bCoolingOff.Parse, New ConvertEventHandler(AddressOf TextToSIKelvin)
	    AddHandler bCoolingOff.Format , New ConvertEventHandler(AddressOf SIKelvinToText)
		txtBC_TemperatureCoolingOff.DataBindings.Add(bCoolingOff)

	    dim bVentPerHour As Binding = New Binding("Text", ssmTOOL.SSMInputs, "BC_HighVentilation")
	    AddHandler bVentPerHour.Parse, New ConvertEventHandler(AddressOf TextToSIPerHour)
	    AddHandler bVentPerHour.Format , New ConvertEventHandler(AddressOf SIPerHourToText)
		txtBC_HighVentilation.DataBindings.Add(bVentPerHour)

	    dim bVentLPerHour As Binding = New Binding("Text", ssmTOOL.SSMInputs, "BC_lowVentilation")
	    AddHandler bVentLPerHour.Parse, New ConvertEventHandler(AddressOf TextToSIPerHour)
	    AddHandler bVentLPerHour.Format , New ConvertEventHandler(AddressOf SIPerHourToText)
		txtBC_lowVentilation.DataBindings.Add(bVentLPerHour)

	    dim bHigh As Binding = New Binding("Text", ssmTOOL.SSMInputs, "BC_High")
	    AddHandler bHigh.Parse, New ConvertEventHandler(AddressOf TextToSICubicMeterPerHour)
	    AddHandler bHigh.Format , New ConvertEventHandler(AddressOf SICubicMeterPerHourToText)
		txtBC_High.DataBindings.Add(bHigh)

	    dim bLow As Binding = New Binding("Text", ssmTOOL.SSMInputs, "BC_Low")
	    AddHandler bLow.Parse, New ConvertEventHandler(AddressOf TextToSICubicMeterPerHour)
	    AddHandler bLow.Format , New ConvertEventHandler(AddressOf SICubicMeterPerHourToText)
		txtBC_Low.DataBindings.Add(bLow)

	    dim bHiP As Binding = New Binding("Text", ssmTOOL.SSMInputs, "BC_HighVentPower")
	    AddHandler bHiP.Parse, New ConvertEventHandler(AddressOf TextToSI(of Watt))
	    AddHandler bHiP.Format , New ConvertEventHandler(AddressOf SIToText)
		txtBC_HighVentPowerW.DataBindings.Add(bHiP)

	    dim bLoP As Binding = New Binding("Text", ssmTOOL.SSMInputs, "BC_LowVentPower")
	    AddHandler bLoP.Parse, New ConvertEventHandler(AddressOf TextToSI(of Watt))
	    AddHandler bLoP.Format , New ConvertEventHandler(AddressOf SIToText)
		txtBC_LowVentPowerW.DataBindings.Add(bLoP)

	    dim bSpecPwr As Binding = New Binding("Text", ssmTOOL.SSMInputs, "BC_SpecificVentilationPower")
	    AddHandler bSpecPwr.Parse, New ConvertEventHandler(AddressOf TextToSIWattHourPerCubicMeter)
	    AddHandler bSpecPwr.Format , New ConvertEventHandler(AddressOf SIWattHourPerCubicMeterToText)
		txtBC_SpecificVentilationPower.DataBindings.Add(bSpecPwr)

		txtBC_AuxHeaterEfficiency.DataBindings.Add("Text", ssmTOOL.SSMInputs, "BC_AuxHeaterEfficiency")

	    dim bGCV As Binding = New Binding("Text", ssmTOOL.SSMInputs, "BC_GCVDieselOrHeatingOil")
	    AddHandler bGCV.Parse, New ConvertEventHandler(AddressOf TextToSIkWhkg)
	    AddHandler bGCV.Format , New ConvertEventHandler(AddressOf SIWhkgToText)
		txtBC_GCVDieselOrHeatingOil.DataBindings.Add(bGCV)

	    dim bWnd As Binding = New Binding("Text", ssmTOOL.SSMInputs, "BC_WindowAreaPerUnitBusLength")
	    AddHandler bWnd.Parse, New ConvertEventHandler(AddressOf TextToSI(of SquareMeterPerMeter))
	    AddHandler bWnd.Format , New ConvertEventHandler(AddressOf SIToText)
		txtBC_WindowAreaPerUnitBusLength.DataBindings.Add(bWnd)

	    dim bFrReW As Binding = New Binding("Text", ssmTOOL.SSMInputs, "BC_FrontRearWindowArea")
	    AddHandler bFrReW.Parse, New ConvertEventHandler(AddressOf TextToSI(of SquareMeter))
	    AddHandler bFrReW.Format , New ConvertEventHandler(AddressOf SIToText)
		txtBC_FrontRearWindowArea.DataBindings.Add(bFrReW)

	    dim bTDiff As Binding = New Binding("Text", ssmTOOL.SSMInputs, "BC_MaxTemperatureDeltaForLowFloorBusses")
	    AddHandler bTDiff.Parse, New ConvertEventHandler(AddressOf TextToSI(of Kelvin))
	    AddHandler bTDiff.Format , New ConvertEventHandler(AddressOf SIToText)
		txtBC_MaxTemperatureDeltaForLowFloorBusses.DataBindings.Add(bTDiff)

	    txtBC_MaxPossibleBenefitFromTechnologyList.DataBindings.Add("Text", ssmTOOL.SSMInputs,
																	"BC_MaxPossibleBenefitFromTechnologyList")

		'EnviromentalConditions	        		
	    dim bEnvT As Binding = New Binding("Text", ssmTOOL.SSMInputs, "EC_EnviromentalTemperature")
	    AddHandler bEnvT.Parse, New ConvertEventHandler(AddressOf TextToSIKelvin)
	    AddHandler bEnvT.Format , New ConvertEventHandler(AddressOf SIKelvinToText)
		txtEC_EnviromentalTemperature.DataBindings.Add(bEnvT)

	    dim bSolar As Binding = New Binding("Text", ssmTOOL.SSMInputs, "EC_Solar")
	    AddHandler bSolar.Parse, New ConvertEventHandler(AddressOf TextToSI(of WattPerSquareMeter))
	    AddHandler bSolar.Format , New ConvertEventHandler(AddressOf SIToText)
		txtEC_Solar.DataBindings.Add(bSolar)

		chkEC_BatchMode.DataBindings.Add("Checked", ssmTOOL.SSMInputs, "EC_EnviromentalConditions_BatchEnabled", False,
										DataSourceUpdateMode.OnPropertyChanged)

		'AC-system	
		cboAC_CompressorType.DataBindings.Add("Text", ssmTOOL.SSMInputs, "AC_CompressorType", False,
											DataSourceUpdateMode.OnPropertyChanged)
		txtAC_CompressorType.DataBindings.Add("Text", ssmTOOL.SSMInputs, "AC_CompressorTypeDerived")

	    dim bAcPwr As Binding = New Binding("Text", ssmTOOL.SSMInputs, "AC_CompressorCapacitykW")
	    AddHandler bAcPwr.Parse, New ConvertEventHandler(AddressOf TextToSIKiloWatt)
	    AddHandler bAcPwr.Format , New ConvertEventHandler(AddressOf SIKiloWattToText)
		txtAC_CompressorCapacitykW.DataBindings.Add(bAcPwr)

		txtAC_COP.DataBindings.Add("Text", ssmTOOL.SSMInputs, "AC_COP")

		'Ventilation	
		chkVEN_VentilationOnDuringHeating.DataBindings.Add("Checked", ssmTOOL.SSMInputs, "VEN_VentilationOnDuringHeating",
															False, DataSourceUpdateMode.OnPropertyChanged)
		chkVEN_VentilationWhenBothHeatingAndACInactive.DataBindings.Add("Checked", ssmTOOL.SSMInputs,
																		"VEN_VentilationWhenBothHeatingAndACInactive", False, DataSourceUpdateMode.OnPropertyChanged)
		chkVEN_VentilationDuringAC.DataBindings.Add("Checked", ssmTOOL.SSMInputs, "VEN_VentilationDuringAC", False,
													DataSourceUpdateMode.OnPropertyChanged)
		cboVEN_VentilationFlowSettingWhenHeatingAndACInactive.DataBindings.Add("Text", ssmTOOL.SSMInputs,
																				"VEN_VentilationFlowSettingWhenHeatingAndACInactive")
		cboVEN_VentilationDuringHeating.DataBindings.Add("Text", ssmTOOL.SSMInputs, "VEN_VentilationDuringHeating")
		cboVEN_VentilationDuringCooling.DataBindings.Add("Text", ssmTOOL.SSMInputs, "VEN_VentilationDuringCooling")

		'Aux. Heater  
	    dim bHtr As Binding = New Binding("Text", ssmTOOL.SSMInputs, "AH_FuelFiredHeaterkW")
	    AddHandler bHtr.Parse, New ConvertEventHandler(AddressOf TextToSIKiloWatt)
	    AddHandler bHtr.Format , New ConvertEventHandler(AddressOf SIKiloWattToText)
		txtAH_FuelFiredHeaterkW.DataBindings.Add(bHtr)

		txtAH_FuelEnergyHeatToCoolant.DataBindings.Add("Text", ssmTOOL.SSMInputs, "AH_FuelEnergyToHeatToCoolant")
		txtAH_CoolantHeatToAirCabinHeater.DataBindings.Add("Text", ssmTOOL.SSMInputs,
															"AH_CoolantHeatTransferredToAirCabinHeater")
	End Sub

	'GeneralInputControlEvents
	'Private Sub cboBuses_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cboBuses.SelectedIndexChanged

	'	If cboBuses.SelectedIndex > 0 Then

	'		Dim bus As IBus = DirectCast(cboBuses.SelectedItem, IBus)

	'		ssmTOOL.SSMInputs.BP_BusModel = bus.Model
	'		ssmTOOL.SSMInputs.BP_NumberOfPassengers = bus.RegisteredPassengers
	'		ssmTOOL.SSMInputs.BP_BusFloorType = bus.FloorType
	'		ssmTOOL.SSMInputs.BP_DoubleDecker = bus.IsDoubleDecker
	'		ssmTOOL.SSMInputs.BP_BusLength = bus.LengthInMetres
	'		ssmTOOL.SSMInputs.BP_BusWidth = bus.WidthInMetres
	'		ssmTOOL.SSMInputs.BP_BusHeight = bus.HeightInMetres

	'		txtBusModel.Text = bus.Model
	'		txtRegisteredPassengers.Text = bus.RegisteredPassengers.ToString()
	'		cmbBusFloorType.Text = bus.FloorType
	'		txtBusLength.Text = bus.LengthInMetres.ToString()
	'		txtBusWidth.Text = bus.WidthInMetres.ToString()
	'		txtBusHeight.Text = bus.HeightInMetres.ToString()
	'		chkIsDoubleDecker.Checked = bus.IsDoubleDecker

	'		txtBusFloorSurfaceArea.Text = ssmTOOL.SSMInputs.BP_BusFloorSurfaceArea.ToString()
	'		txtBusWindowSurfaceArea.Text = ssmTOOL.SSMInputs.BP_BusWindowSurface.ToString()
	'		txtBusSurfaceArea.Text = ssmTOOL.SSMInputs.BP_BusSurfaceArea.ToString()
	'		txtBusVolume.Text = ssmTOOL.SSMInputs.BP_BusVolume.ToString()

	'		btnEditBus.Enabled = True

	'	Else
	'		btnEditBus.Enabled = False
	'	End If
	'End Sub

	'Validators
	Public Sub Validating_GeneralInputsBP(sender As Object, e As CancelEventArgs) _
		Handles txtRegisteredPassengers.Validating, txtBusWidth.Validating, txtBusHeight.Validating, txtBusLength.Validating,
				txtBusModel.Validating

		e.Cancel = Not Validate_GeneralInputsBP()
	End Sub

	Public Sub Validating_GeneralInputsBC(sender As Object, e As CancelEventArgs) _
		Handles txtBC_GFactor.Validating, txtBC_SpecificVentilationPower.Validating,
				txtBC_MaxTemperatureDeltaForLowFloorBusses.Validating, txtBC_MaxPossibleBenefitFromTechnologyList.Validating,
				txtBC_lowVentilation.Validating, txtBC_HighVentilation.Validating, txtBC_HeatingBoundaryTemperature.Validating,
				txtBC_GCVDieselOrHeatingOil.Validating, txtBC_CoolingBoundaryTemperature.Validating,
				txtBC_AuxHeaterEfficiency.Validating, txtBC_PassengerBoundaryTemperature.Validating

		e.Cancel = Not Validate_GeneralInputsBC()
	End Sub

	Public Sub Validating_GeneralInputsOther(sender As Object, e As CancelEventArgs) _
		Handles txtEC_Solar.Validating, txtEC_EnviromentalTemperature.Validating, txtAH_FuelFiredHeaterkW.Validating,
				txtAC_CompressorCapacitykW.Validating, txtEC_EnvironmentConditionsFilePath.Validating,
				txtAH_FuelEnergyHeatToCoolant.Validating, txtAH_CoolantHeatToAirCabinHeater.Validating

		e.Cancel = Not Validate_GeneralInputsOther()
	End Sub

	Public Function Validate_GeneralInputsBP() As Boolean

		Dim result As Boolean = True

		'BUS PARAMETERISATION
		'********************

		'txtBusModel
		If txtBusModel.Text.Trim.Length = 0 Then
			ErrorProvider1.SetError(txtBusModel, "Please enter a bus model")
			result = False
		Else
			ErrorProvider1.SetError(txtBusModel, String.Empty)
		End If


		'txtRegisteredPassengers
		If Not IsNumberGreaterThanOne(txtRegisteredPassengers.Text) Then
			ErrorProvider1.SetError(txtRegisteredPassengers,
									"Please enter an integer of one or higher ( Bus : Number of Passengers )")
			result = False
		Else
			ErrorProvider1.SetError(txtRegisteredPassengers, String.Empty)
		End If

		'txtBusWidth
		If Not IsPostiveNumber(txtBusWidth.Text) Then
			ErrorProvider1.SetError(txtBusWidth, "Please enter a positive number ( BusWidth : linear metres )")
			result = False
		Else
			ErrorProvider1.SetError(txtBusWidth, String.Empty)
		End If

		'txtBusHeight
		If Not IsPostiveNumber(txtBusHeight.Text) Then
			ErrorProvider1.SetError(txtBusHeight, "Please enter a positive number ( BusHeight : linear metres )")
			result = False
		Else
			ErrorProvider1.SetError(txtBusHeight, String.Empty)
		End If

		'txtBusLength
		If Not IsPostiveNumber(txtBusLength.Text) Then
			ErrorProvider1.SetError(txtBusLength, "Please enter a positive number ( BusLength : linear metres )")
			result = False
		Else
			ErrorProvider1.SetError(txtBusLength, String.Empty)
		End If

		'Set Tab Color
		UpdateTabStatus("tabGeneralInputsBP", result)

		Return result
	End Function

	Public Function Validate_GeneralInputsBPEdit() As Boolean

		Dim result As Boolean = True

		'BUS PARAMETERISATION
		'********************

		'txtBusModel
		If txtEditBusModel.Text.Trim.Length = 0 Then
			ErrorProvider1.SetError(txtEditBusModel, "Please enter a bus model")
			result = False
		Else
			ErrorProvider1.SetError(txtEditBusModel, String.Empty)
		End If

		'cmbEditFloorType
		If cmbEditFloorType.Text.Trim.Length = 0 Then
			ErrorProvider1.SetError(cmbEditFloorType, "Please select a floor type")
			result = False
		Else
			ErrorProvider1.SetError(cmbEditFloorType, String.Empty)
		End If

		'cmbEditEngineType
		If cmbEditEngineType.Text.Trim.Length = 0 Then
			ErrorProvider1.SetError(cmbEditEngineType, "Please select an engine type")
			result = False
		Else
			ErrorProvider1.SetError(cmbEditEngineType, String.Empty)
		End If

		'txtRegisteredPassengers
		If Not IsNumberGreaterThanOne(txtEditBusPassengers.Text) Then
			ErrorProvider1.SetError(txtEditBusPassengers,
									"Please enter an integer of one or higher ( Bus : Number of Passengers )")
			result = False
		Else
			ErrorProvider1.SetError(txtEditBusPassengers, String.Empty)
		End If

		'txtBusLength
		If Not IsPostiveNumber(txtEditBusLength.Text) Then
			ErrorProvider1.SetError(txtEditBusLength, "Please enter a positive number ( Buslength : linear metres )")
			result = False
		Else
			ErrorProvider1.SetError(txtEditBusLength, String.Empty)
		End If

		'txtBusWidth
		If Not IsPostiveNumber(txtEditBusWidth.Text) Then
			ErrorProvider1.SetError(txtEditBusWidth, "Please enter a positive number ( BusWidth : linear metres )")
			result = False
		Else
			ErrorProvider1.SetError(txtEditBusWidth, String.Empty)
		End If

		'txtBusHeight
		If Not IsPostiveNumber(txtEditBusHeight.Text) Then
			ErrorProvider1.SetError(txtEditBusHeight, "Please enter a positive number ( BusHeight : linear metres )")
			result = False
		Else
			ErrorProvider1.SetError(txtEditBusHeight, String.Empty)
		End If

		'Set Tab Color
		UpdateTabStatus("tabGeneralInputsBP", result)

		Return result
	End Function

	Public Function Validate_GeneralInputsBC() As Boolean

		Dim result As Boolean = True

		'BOUNDARY CONDITIONS
		'*******************

		'txtBC_GFactor		
		IsTextBoxNumber(txtBC_GFactor, "Please enter a number ( GFactor )", result)
		'BC_SolarClouding				        : Calculated    
		'BC_HeatPerPassengerIntoCabinW	        : Calculated             
		'txtBC_PassengerBoundaryTemperature    
		IsTextBoxNumber(txtBC_PassengerBoundaryTemperature, "Please enter a number ( Passenger Boundary Temperature )", result)
		'txtBC_PassengerDensityLowFloor         : Calculated  
		'txtBC_PassengerDensitySemiLowFloor	    : Calculated 
		'txtBC_PassengerDensityRaisedFloor	    : Calculated 
		'txtBC_CalculatedPassengerNumber	    : Calculated          
		'txtBC_UValues                          : Calculated                            
		'txtBC_HeatingBoundaryTemperature	
		IsTextBoxNumber(txtBC_HeatingBoundaryTemperature, "Please enter a number ( Heating Boundary Temperature )", result)
		'txtBC_CoolingBoundaryTemperature 
		IsTextBoxNumber(txtBC_CoolingBoundaryTemperature, "Please enter a number ( Cooling Boundary Temperature )", result)
		'txtBC_HighVentilation    
		IsTextBoxNumber(txtBC_HighVentilation, "Please enter a number ( High Ventilation )", result)
		'txtBC_lowVentilation	
		IsTextBoxNumber(txtBC_lowVentilation, "Please enter a number ( Low Ventilation )", result)
		'txtBC_High             : Calculated                                     
		'txtBC_Low	            : Calculated                                
		'txtBC_HighVentPowerW   : Calculated                  
		'txtBC_LowVentPowerW    : Calculated                           
		'txtBC_SpecificVentilationPower 
		IsTextBoxNumber(txtBC_SpecificVentilationPower, "Please enter a number ( Specific Ventilation Power )", result)
		'txtBC_COP	
		IsTextBoxNumber(txtAC_COP, "Please enter a number ( COP )", result)
		'txtBC_AuxHeaterEfficiency		
		IsTextBoxNumber(txtBC_AuxHeaterEfficiency, "Please enter a number ( Aux Heater Efficiency )", result)
		'txtBC_GCVDieselOrHeatingOil   
		IsTextBoxNumber(txtBC_GCVDieselOrHeatingOil, "Please enter a number ( GCV Diesel Or Heating Oil )", result)
		'txtBC_WindowAreaPerUnitBusLength	     : Calculated 
		'txtBC_FrontRearWindowArea               : Calculated                      
		'txtBC_MaxTemperatureDeltaForLowFloorBusses
		IsTextBoxNumber(txtBC_MaxTemperatureDeltaForLowFloorBusses,
						"Please enter a number ( Max Temp Delta For Low Floor Busses )", result)
		'txtBC_MaxPossibleBenefitFromTechnologyList
		IsTextBoxNumber(txtBC_MaxPossibleBenefitFromTechnologyList,
						"Please enter a number ( Max Benefit From Technology List )", result)

		'Set Tab Color
		UpdateTabStatus("tabGeneralInputsBC", result)

		Return result
	End Function

	Public Function Validate_GeneralInputsOther() As Boolean

		Dim result As Boolean = True

		If (Not chkEC_BatchMode.Checked) Then
			'EnviromentalConditions				
			IsTextBoxNumber(txtEC_EnviromentalTemperature, "Please enter a number (Environmental Temperature)", result)
			'txtEC_Solar   	                                  
			IsTextBoxNumber(txtEC_Solar, "Please enter a number (Solar)", result)
		End If


		''AC-system				                                     
		'chkAC_InCabinRoomAC_System	     : Selection                  
		'cboAC_CompressorType			 : Selection                
		'txtAC_CompressorCapacitykW	 
		IsTextBoxNumber(txtAC_CompressorCapacitykW, "Please enter a number ( Compressor Capacity )", result)

		''Ventilation				
		'chkVEN_VentilationOnDuringHeating				          : Selection
		'chkVEN_VentilationWhenBothHeatingAndACInactive		      : Selection
		'chkVEN_VentilationDuringAC			                      : Selection
		'cboVEN_VentilationFlowSettingWhenHeatingAndACInactive    : Selection
		'cboVEN_VentilationDuringHeating			              : Selection
		'cboVEN_VentilationDuringCooling				          : Selection

		''Aux. Heater				
		'txtAH_FuelFiredHeaterkW  
		IsTextBoxNumber(txtAH_FuelFiredHeaterkW, "Please enter a number ( Fuel fired heater )", result)
		IsTextBoxNumber(txtAH_FuelEnergyHeatToCoolant, "Please enter a number ( Fuel Energy Heat To Coolant )", result)
		IsTextBoxNumber(txtAH_CoolantHeatToAirCabinHeater,
						"Please enter a number ( Coolant Heat Transfered To Air CabinHeater )", result)

		Try
			'Dim environmentalConditionsMap As IEnvironmentalConditionsMap =
			'		New EnvironmentalConditionsMap(CType(txtEC_EnvironmentConditionsFilePath.Tag, String), Path.GetDirectoryName(vectoFile))
			ErrorProvider1.SetError(txtEC_EnvironmentConditionsFilePath, String.Empty)
			'ssmTOOL.SSMInputs.EnvironmentalConditions.EnviromentalConditions_BatchFile = CType(txtEC_EnvironmentConditionsFilePath.Tag, String)
		Catch ex As Exception
			ErrorProvider1.SetError(txtEC_EnvironmentConditionsFilePath,
									"Error : The environment conditions file is invalid or cannot be found, please select a valid aenv file.")
			result = False
		End Try

		'Set Tab Color
		UpdateTabStatus("tabGeneralInputsOther", result)

		Return result
	End Function

	Public Sub Validating_TechLineEdit(sender As Object, e As CancelEventArgs) _
'Handles txtSemiLowFloorV.Validating, txtSemiLowFloorH.Validating, txtSemiLowFloorC.Validating, txtRaisedFloorV.Validating, txtRaisedFloorH.Validating, txtRaisedFloorC.Validating, txtLowFloorV.Validating, txtLowFloorH.Validating, txtLowFloorC.Validating, txtBenefitName.Validating, chkOnVehicle.Validating, chkActiveVV.Validating, chkActiveVH.Validating, chkActiveVC.Validating, cboUnits.Validating, cboLineType.Validating, cboCategory.Validating

		e.Cancel = Not Validate_TechLineEdit()
	End Sub

	Public Function Validate_TechLineEdit() As Boolean

		Dim result As Boolean = True

		IsEmptyString(cboCategory.Text, cboCategory, "Please enter a valid category", result)
		IsEmptyString(txtBenefitName.Text, txtBenefitName, "Please enter a valid Benefit Name", result)
		IsEmptyString(cboUnits.Text, cboUnits, "Please enter valid units", result)
		IsEmptyString(cboLineType.Text, cboLineType, "Please enter a valid line type", result)
		IsTextBoxNumber(txtLowFloorH, "Please enter a valid number for this floor variable", result)
		IsTextBoxNumber(txtLowFloorV, "Please enter a valid number for this floor variable", result)
		IsTextBoxNumber(txtLowFloorC, "Please enter a valid number for this floor variable", result)
		IsTextBoxNumber(txtSemiLowFloorH, "Please enter a valid number for this floor variable", result)
		IsTextBoxNumber(txtSemiLowFloorV, "Please enter a valid number for this floor variable", result)
		IsTextBoxNumber(txtSemiLowFloorC, "Please enter a valid number for this floor variable", result)
		IsTextBoxNumber(txtRaisedFloorH, "Please enter a valid number for this floor variable", result)
		IsTextBoxNumber(txtRaisedFloorV, "Please enter a valid number for this floor variable", result)
		IsTextBoxNumber(txtRaisedFloorC, "Please enter a valid number for this floor variable", result)


		Return result
	End Function

	'Validation Helpers
	Private Sub IsTextBoxNumber(control As TextBox, errorProviderMessage As String, ByRef result As Boolean)

		If Not IsNumeric(control.Text) Then
			ErrorProvider1.SetError(control, errorProviderMessage)
			result = False
		Else
			ErrorProvider1.SetError(control, String.Empty)

		End If
	End Sub

	Private Sub IsEmptyString(text As String, control As Control, errorProviderMessage As String, ByRef result As Boolean)

		If String.IsNullOrEmpty(text) Then
			ErrorProvider1.SetError(control, errorProviderMessage)
			result = False
		Else
			ErrorProvider1.SetError(control, String.Empty)

		End If
	End Sub

	Private Function IsPostiveInteger(ByVal test As String) As Boolean

		'Is this numeric sanity check.
		If Not IsNumeric(test) Then Return False

		Dim number As Integer

		If Not Integer.TryParse(test, number) Then Return False

		If number <= 0 Then Return False


		Return True
	End Function

	Private Function IsPostiveNumber(ByVal test As String) As Boolean

		'Is this numeric sanity check.
		If Not IsNumeric(test) Then Return False

		Dim number As Double

		If Not Double.TryParse(test, number) Then Return False

		If number <= 0 Then Return False


		Return True
	End Function

	Private Function IsNumberGreaterThanOne(ByVal test As String) As Boolean

		'Is this numeric sanity check.
		If Not IsNumeric(test) Then Return False

		Dim number As Double

		If Not Double.TryParse(test, number) Then Return False

		If number <= 1 Then Return False


		Return True
	End Function

	Private Function IsZeroOrPostiveNumber(ByVal test As String) As Boolean

		'Is this numeric sanity check.
		If Not IsNumeric(test) Then Return False

		Dim number As Double

		If Not Double.TryParse(test, number) Then Return False

		If number < 0 Then Return False


		Return True
	End Function

	Private Function IsNumberBetweenZeroandOne(test As String) As Boolean

		'Is this numeric sanity check.
		If Not IsNumeric(test) Then Return False

		Dim number As Double

		If Not Double.TryParse(test, number) Then Return False

		If number < 0 OrElse number > 1 Then Return False

		Return True
	End Function

	Private Function IsIntegerZeroOrPositiveNumber(test As String) As Boolean

		'Is this numeric sanity check.
		If Not IsNumeric(test) Then Return False

		'if not integer then return false

		Dim number As Integer

		If Not Integer.TryParse(test, number) Then Return False

		If number < 0 Then Return False

		Return True
	End Function

	Private Function ValidateAll() As Boolean

		Return Not IsAddingBus AndAlso
				Not IsUpdatingBus AndAlso
				Validate_GeneralInputsBC() AndAlso
				Validate_GeneralInputsBP() AndAlso
				Validate_GeneralInputsOther()
	End Function

	'Tab Colors
	Private Sub UpdateTabStatus(pageName As String, resultGood As Boolean)

		Dim page As TabPage = tabMain.TabPages(pageName)

		If Not resultGood Then

			SetTabHeader(page, Color.Red)

		Else
			SetTabHeader(page, Control.DefaultBackColor)

		End If
	End Sub

	Private Sub SetTabHeader(page As TabPage, color As Color)

		TabColors(page) = color
		tabMain.Invalidate()
	End Sub

	Private Sub tabMain_DrawItem(sender As Object, e As DrawItemEventArgs)

		Dim br As Brush = New SolidBrush(TabColors(tabMain.TabPages(e.Index)))

		Using (br)

			e.Graphics.FillRectangle(br, e.Bounds)
			Dim sz As SizeF = e.Graphics.MeasureString(tabMain.TabPages(e.Index).Text, e.Font)
			e.Graphics.DrawString(tabMain.TabPages(e.Index).Text, e.Font, Brushes.Black,
								e.Bounds.Left + (e.Bounds.Width - sz.Width)/2, e.Bounds.Top + (e.Bounds.Height - sz.Height)/2 + 1)

			Dim rect As Rectangle = e.Bounds
			rect.Offset(- 1, - 1)
			rect.Inflate(1, 1)


		End Using
	End Sub

	Private Sub EnsureBinding()
		With tabMain
			Dim lastSelectedTabIndex As Integer = .SelectedIndex
			If lastSelectedTabIndex < 0 OrElse lastSelectedTabIndex > .TabCount Then lastSelectedTabIndex = 0
			For currentTab As Integer = 0 To .TabCount - 1
				.SelectedIndex = currentTab
			Next
			.SelectedIndex = 0
		End With
	End Sub

	'Form/Control Events
	Private Sub frmHVACTool_Load(sender As Object, e As EventArgs) Handles MyBase.Load

		'Required for OwnerDraw, this is required in order to color the tabs when a validation error occurs to draw
		'The attention of the user to the fact that attention is required on a particlar tab.
		TabColors.Add(tabGeneralInputsBP, Control.DefaultBackColor)
		TabColors.Add(tabGeneralInputsBC, Control.DefaultBackColor)
		TabColors.Add(tabGeneralInputsOther, Control.DefaultBackColor)
		TabColors.Add(tabTechBenefits, Control.DefaultBackColor)
		TabColors.Add(tabDiagnostics, Control.DefaultBackColor)

		EnsureBinding()

		'Additional atatched events
		'For Tab Coloring, this is the place where the background will get filled on the tab when attention is required.
		AddHandler tabMain.DrawItem, New System.Windows.Forms.DrawItemEventHandler(AddressOf tabMain_DrawItem)

		gvTechBenefitLines.ClearSelection()

		Dim r As DialogResult = Me.DialogResult
	End Sub

	Private Sub frmHVACTool_FormClosing(sender As Object, e As FormClosingEventArgs) Handles MyBase.FormClosing


		Dim result As DialogResult

		'If UserHitCancel then bail
		If UserHitCancel Then
			DialogResult = Windows.Forms.DialogResult.Cancel
			UserHitCancel = False
			Return
		End If

		'UserHitSave
		If UserHitSave Then
			DialogResult = Windows.Forms.DialogResult.Cancel
			If Not BusAuxWriter.SaveSSMConfig(ssmTOOL.SSMInputs, ahsmFilePath) Then
				MessageBox.Show("Unable to save file, aborting.")
				e.Cancel = True
			End If
			DialogResult = Windows.Forms.DialogResult.OK
			UserHitSave = False
			Return
		End If


		'This must be a close box event. If nothing changed, then bail, otherwise ask user if they wanna save
		'If Not ssmTOOL.IsEqualTo(originalssmTOOL) Then

		'	result =
		'		(MessageBox.Show("Would you like to save changes before closing?", "Save Changes", MessageBoxButtons.YesNoCancel,
		'						MessageBoxIcon.Question))


		'	Select Case result

		'		Case DialogResult.Yes
		'			'save 

		'			If Not BusAuxWriter.SaveSSMConfig(ssmTOOL.SSMInputs, ahsmFilePath) Then
		'				e.Cancel = True
		'			End If

		'		Case DialogResult.No
		'			'just allow the form to close
		'			'without saving
		'			Me.DialogResult = Windows.Forms.DialogResult.Cancel


		'		Case DialogResult.Cancel
		'			'cancel the close
		'			e.Cancel = True
		'			Me.DialogResult = Windows.Forms.DialogResult.Cancel


		'	End Select

		'End If

		UserHitCancel = False
		UserHitSave = False
	End Sub

	'Grid Events
	Private Sub gvTechBenefitLines_DoubleClick(sender As Object, e As EventArgs) Handles gvTechBenefitLines.DoubleClick

		If gvTechBenefitLines.SelectedCells.Count < 1 Then Return


		Dim row As Integer = gvTechBenefitLines.SelectedCells(0).OwningRow.Index

		Dim benefitName, category As String
		benefitName = CType(gvTechBenefitLines.Rows(row).Cells("BenefitName").Value, String)
		category = CType(gvTechBenefitLines.Rows(row).Cells("Category").Value, String)


		'editTechLine = ssmTOOL.TechList.TechLines.First(Function(f) f.BenefitName = benefitName AndAlso f.Category = category)

		FillTechLineEditPanel(row)

		UpdateButtonText()
	End Sub

	Private Sub gvTechBenefitLines_CellClick(sender As Object, e As DataGridViewCellEventArgs) _
		Handles gvTechBenefitLines.CellClick

		If e.ColumnIndex < 0 OrElse e.RowIndex < 0 Then Return


		If _
			gvTechBenefitLines.Columns(e.ColumnIndex).Name = "Delete" OrElse
			gvTechBenefitLines.Columns(e.ColumnIndex).Name = "OnVehicle" Then

			Dim benefit As String = CType(gvTechBenefitLines.Rows(e.RowIndex).Cells(1).Value, String)
			Dim category As String = CType(gvTechBenefitLines.Rows(e.RowIndex).Cells(0).Value, String)
			Dim feedback As String = String.Empty


			Select Case gvTechBenefitLines.Columns(e.ColumnIndex).Name


				Case "Delete"
					Dim dr As DialogResult = MessageBox.Show($"Do you want to delete benefit '{benefit}' ?", "",
															MessageBoxButtons.YesNo)
					If dr = Windows.Forms.DialogResult.Yes Then
						'If ssmTOOL.TechList.Delete(New TechListBenefitLine With {.BenefitName = benefit, .Category = category}, feedback) _
						'	Then
						'	BindGrid()
						'End If
					End If

				Case "OnVehicle"
					Dim onVehicle As Boolean = Not CType(gvTechBenefitLines.Rows(e.RowIndex).Cells(e.ColumnIndex).Value, Boolean)

					'Dim fi As ITechListBenefitLine = ssmTOOL.TechList.Find(category, benefit)
					'fi.OnVehicle = onVehicle
					' ssmTOOL.TechList.TechLines.First( Function(x)  x.BenefitName= benefit AndAlso x.Category=category).OnVehicle=onVehicle  
					' BindGrid 
					gvTechBenefitLines.Refresh()


			End Select


		End If
	End Sub

	Private Sub gvTechBenefitLines_CurrentCellDirtyStateChanged(sender As Object, e As EventArgs) _
		Handles gvTechBenefitLines.CurrentCellDirtyStateChanged


		If gvTechBenefitLines.IsCurrentCellDirty Then
			gvTechBenefitLines.CommitEdit(DataGridViewDataErrorContexts.Commit)
		End If
	End Sub

	'Button Event Handlers
	Private Sub btnUpdate_Click(sender As Object, e As EventArgs) Handles btnUpdate.Click

		Dim feedback As String = String.Empty

		If Not Validate_TechLineEdit() Then Return

		If txtIndex.Text.Trim.Length = 0 Then
			'This is an Add
			'If Not ssmTOOL.TechList.Add(GetTechLineFromPanel(), feedback) Then
			'	MessageBox.Show(feedback)
			'Else


				BindGrid()

				'find new row
				'Dim ol As List(Of ISSMTechnology) =
				'		ssmTOOL.TechList.TechLines.OrderBy(Function(x) x.Category).ThenBy(Function(tb) tb.BenefitName).ToList()
				'Dim item As ISSMTechnology =
				'		ol.First(
				'			Function(x) _
				'					x.Category = GetTechLineFromPanel().Category AndAlso x.BenefitName = GetTechLineFromPanel().BenefitName)
				'Dim idx As Integer = ol.IndexOf(item)

				'gvTechBenefitLines.FirstDisplayedScrollingRowIndex = idx

				cboCategory.DataSource = GetCategories()

				UpdateButtonText()

			'End If

		Else
			'This is an update
			'If Not ssmTOOL.TechList.Modify(editTechLine, GetTechLineFromPanel(), feedback) Then
			'	MessageBox.Show(feedback)
			'Else
			'	gvTechBenefitLines.Refresh()
			'	ClearEditPanel()
			'	UpdateButtonText()

			'End If

		End If
	End Sub

	Private Sub btnSave_Click(sender As Object, e As EventArgs) Handles btnSave.Click

		If Not ValidateAll() Then Return

		UserHitSave = True

		Me.DialogResult = Windows.Forms.DialogResult.OK
		Me.Close()
	End Sub

	Private Sub btnClearForm_Click(sender As Object, e As EventArgs) Handles btnClearForm.Click

		ClearEditPanel()
		UpdateButtonText()
	End Sub

	Private Sub btnCancel_Click(sender As Object, e As EventArgs) Handles btnCancel.Click

		UserHitCancel = True
		Me.Close()
	End Sub

	Private Sub btnNewBus_Click(sender As Object, e As EventArgs) Handles btnNewBus.Click

		If "New".Equals(btnNewBus.Tag) Then

			IsAddingBus = True

			'cboBuses.Enabled = False
			btnUpdateBusDatabase.Enabled = False
			btnEditBus.Enabled = False
			btnCancelBus.Enabled = True

			btnNewBus.Text = "Add"
			btnNewBus.Tag = "Add"

			BusParamGroupModel.Visible = False
			BusParamGroupEdit.Visible = True

		ElseIf "Add".Equals(btnNewBus.Tag) Then

			If Validate_GeneralInputsBPEdit() Then
				'Dim newBus As IBus = New Bus(busesList.Count + 1, txtEditBusModel.Text, cmbEditFloorType.Text,
				'							cmbEditEngineType.Text, Double.Parse(txtEditBusLength.Text, CultureInfo.InvariantCulture),
				'							Double.Parse(txtEditBusWidth.Text, CultureInfo.InvariantCulture),
				'							Double.Parse(txtEditBusHeight.Text, CultureInfo.InvariantCulture),
				'							Convert.ToInt32(txtEditBusPassengers.Text), chkEditIsDoubleDecker.Checked)

				'buses.AddBus(newBus)
				'busesList.Add(newBus)

				'cboBuses.Enabled = True
				btnUpdateBusDatabase.Enabled = True
				btnEditBus.Enabled = True
				btnCancelBus.Enabled = False

				BusParamGroupModel.Visible = True
				BusParamGroupEdit.Visible = False

				btnNewBus.Text = "New"
				btnNewBus.Tag = "New"

				'cboBuses.SelectedIndex = busesList.Count - 1

				IsAddingBus = False
			End If

		End If
	End Sub

	Private Sub btnUpdateBusDatabase_Click(sender As Object, e As EventArgs) Handles btnUpdateBusDatabase.Click

		'If buses.Save(busDatabasePath) Then
		'	MessageBox.Show("Buses database file saved successfully")
		'Else
			MessageBox.Show("Failed to save buses database file")
		'End If
	End Sub

	'Private Sub btnEditBus_Click(sender As Object, e As EventArgs) Handles btnEditBus.Click

	'	If cboBuses.SelectedIndex > 0 Then

	'		If "Edit".Equals(btnEditBus.Tag) Then

	'			IsUpdatingBus = True

	'			cboBuses.Enabled = False
	'			btnUpdateBusDatabase.Enabled = False
	'			btnNewBus.Enabled = False
	'			btnCancelBus.Enabled = True

	'			btnEditBus.Text = "Save"
	'			btnEditBus.Tag = "Save"

	'			BusParamGroupModel.Visible = False
	'			BusParamGroupEdit.Visible = True

	'			Dim bus As IBus = DirectCast(cboBuses.SelectedItem, IBus)

	'			txtEditBusModel.Text = bus.Model
	'			cmbEditFloorType.Text = bus.FloorType
	'			cmbEditEngineType.Text = bus.EngineType
	'			txtEditBusLength.Text = String.Format(CultureInfo.InvariantCulture, "{0}", bus.LengthInMetres)
	'			txtEditBusWidth.Text = String.Format(CultureInfo.InvariantCulture, "{0}", bus.WidthInMetres)
	'			txtEditBusHeight.Text = String.Format(CultureInfo.InvariantCulture, "{0}", bus.HeightInMetres)
	'			txtEditBusPassengers.Text = String.Format(CultureInfo.InvariantCulture, "{0}", bus.RegisteredPassengers)

	'		ElseIf "Save".Equals(btnEditBus.Tag) Then

	'			If Validate_GeneralInputsBPEdit() Then

	'				Dim bus As IBus = DirectCast(cboBuses.SelectedItem, IBus)

	'				bus.Model = txtEditBusModel.Text
	'				bus.FloorType = cmbEditFloorType.Text
	'				bus.EngineType = cmbEditEngineType.Text
	'				bus.LengthInMetres = Double.Parse(txtEditBusLength.Text, CultureInfo.InvariantCulture)
	'				bus.WidthInMetres = Double.Parse(txtEditBusWidth.Text, CultureInfo.InvariantCulture)
	'				bus.HeightInMetres = Double.Parse(txtEditBusHeight.Text, CultureInfo.InvariantCulture)
	'				bus.RegisteredPassengers = Integer.Parse(txtEditBusPassengers.Text, CultureInfo.InvariantCulture)
	'				bus.IsDoubleDecker = chkEditIsDoubleDecker.Checked

	'				buses.UpdateBus(bus.Id, bus)

	'				cboBuses.Enabled = True
	'				btnUpdateBusDatabase.Enabled = True
	'				btnNewBus.Enabled = True
	'				btnCancelBus.Enabled = False

	'				BusParamGroupModel.Visible = True
	'				BusParamGroupEdit.Visible = False

	'				btnEditBus.Text = "Edit"
	'				btnEditBus.Tag = "Edit"

	'				Dim currentIndex As Integer = cboBuses.SelectedIndex
	'				cboBuses.SelectedIndex = 0
	'				cboBuses.SelectedIndex = currentIndex

	'				IsUpdatingBus = False
	'			End If

	'		End If

	'	End If
	'End Sub

	'Private Sub btnCancelBus_Click(sender As Object, e As EventArgs) Handles btnCancelBus.Click

	'	IsUpdatingBus = False
	'	IsAddingBus = False

	'	cboBuses.Enabled = True
	'	btnUpdateBusDatabase.Enabled = True
	'	btnNewBus.Enabled = True
	'	btnCancelBus.Enabled = False

	'	If cboBuses.SelectedIndex > 0 Then
	'		btnEditBus.Enabled = True
	'	End If

	'	btnEditBus.Text = "Edit"
	'	btnEditBus.Tag = "Edit"

	'	btnNewBus.Text = "New"
	'	btnNewBus.Tag = "New"

	'	txtEditBusModel.Text = String.Empty
	'	cmbEditFloorType.Text = String.Empty
	'	cmbEditEngineType.Text = String.Empty
	'	txtEditBusLength.Text = String.Empty
	'	txtEditBusWidth.Text = String.Empty
	'	txtEditBusHeight.Text = String.Empty
	'	txtEditBusPassengers.Text = String.Empty
	'	chkEditIsDoubleDecker.Checked = False

	'	BusParamGroupModel.Visible = True
	'	BusParamGroupEdit.Visible = False
	'End Sub

	'TechList Helpers
	Private Sub FillTechLineEditPanel(index As Integer)

		Dim techline As SSMTechnology
		Dim benefitName, category As Object
		benefitName = gvTechBenefitLines.Rows(index).Cells("BenefitName").Value
		category = gvTechBenefitLines.Rows(index).Cells("Category").Value

		'techline =
		'	ssmTOOL.TechList.TechLines.First(Function(f) f.BenefitName.Equals(benefitName) AndAlso f.Category.Equals(category))

		txtIndex.Text = index.ToString()
		cboCategory.Text = techline.Category
		txtBenefitName.Text = techline.BenefitName
		'cboLineType.Text = If(techline.LineType = 0, "Normal", "ActiveVentilation")
		txtLowFloorH.Text = String.Format(CultureInfo.InvariantCulture, "{0}", techline.LowFloorH)
		txtLowFloorV.Text = String.Format(CultureInfo.InvariantCulture, "{0}", techline.LowFloorV)
		txtLowFloorC.Text = String.Format(CultureInfo.InvariantCulture, "{0}", techline.LowFloorC)
		txtSemiLowFloorH.Text = String.Format(CultureInfo.InvariantCulture, "{0}", techline.SemiLowFloorH)
		txtSemiLowFloorV.Text = String.Format(CultureInfo.InvariantCulture, "{0}", techline.SemiLowFloorV)
		txtSemiLowFloorC.Text = String.Format(CultureInfo.InvariantCulture, "{0}", techline.SemiLowFloorC)
		txtRaisedFloorH.Text = String.Format(CultureInfo.InvariantCulture, "{0}", techline.RaisedFloorH)
		txtRaisedFloorV.Text = String.Format(CultureInfo.InvariantCulture, "{0}", techline.RaisedFloorV)
		txtRaisedFloorC.Text = String.Format(CultureInfo.InvariantCulture, "{0}", techline.RaisedFloorC)
		chkActiveVH.Checked = techline.ActiveVH
		chkActiveVV.Checked = techline.ActiveVV
		chkActiveVC.Checked = techline.ActiveVC
		'chkOnVehicle.Checked = techline.OnVehicle
	End Sub

	Private Function GetTechLineFromPanel() As SSMTechnology

		Dim tl As SSMTechnology = New SSMTechnology()
	    'tl.BusFloorType = ssmTOOL.SSMInputs.BusParameters.BusFloorType

		tl.Category = StrConv(cboCategory.Text, vbProperCase)
		tl.BenefitName = txtBenefitName.Text
		'tl.LineType = If(cboLineType.Text = "Normal", TechLineType.Normal, TechLineType.HVCActiveSelection)
		tl.LowFloorH = Double.Parse(txtLowFloorH.Text, CultureInfo.InvariantCulture)
		tl.LowFloorV = Double.Parse(txtLowFloorV.Text, CultureInfo.InvariantCulture)
		tl.LowFloorC = Double.Parse(txtLowFloorC.Text, CultureInfo.InvariantCulture)
		tl.SemiLowFloorH = Double.Parse(txtSemiLowFloorH.Text, CultureInfo.InvariantCulture)
		tl.SemiLowFloorV = Double.Parse(txtSemiLowFloorV.Text, CultureInfo.InvariantCulture)
		tl.SemiLowFloorC = Double.Parse(txtSemiLowFloorC.Text, CultureInfo.InvariantCulture)
		tl.RaisedFloorH = Double.Parse(txtRaisedFloorH.Text, CultureInfo.InvariantCulture)
		tl.RaisedFloorV = Double.Parse(txtRaisedFloorV.Text, CultureInfo.InvariantCulture)
		tl.RaisedFloorC = Double.Parse(txtRaisedFloorC.Text, CultureInfo.InvariantCulture)
		tl.ActiveVH = chkActiveVH.Checked
		tl.ActiveVV = chkActiveVV.Checked
		tl.ActiveVC = chkActiveVC.Checked
		'tl.OnVehicle = chkOnVehicle.Checked


		Return tl
	End Function

	Private Sub ClearEditPanel()

		txtIndex.Text = String.Empty
		cboCategory.SelectedIndex = 0
		txtBenefitName.Text = String.Empty
		cboUnits.SelectedIndex = 0
		cboLineType.SelectedIndex = 0
		txtLowFloorH.Text = String.Empty
		txtLowFloorV.Text = String.Empty
		txtLowFloorC.Text = String.Empty
		txtSemiLowFloorH.Text = String.Empty
		txtSemiLowFloorV.Text = String.Empty
		txtSemiLowFloorC.Text = String.Empty
		txtRaisedFloorH.Text = String.Empty
		txtRaisedFloorV.Text = String.Empty
		txtRaisedFloorC.Text = String.Empty
		chkActiveVH.Checked = False
		chkActiveVV.Checked = False
		chkActiveVC.Checked = False
		chkOnVehicle.Checked = False
	End Sub

	'Tab Events
	Private Sub tabMain_SelectedIndexChanged(sender As Object, e As EventArgs) Handles tabMain.SelectedIndexChanged

		If tabMain.SelectedIndex = 4 Then
			captureDiagnostics = True
		End If
	End Sub

	'Timer Events
	Private Sub Timer1_Tick(sender As Object, e As EventArgs) Handles Timer1.Tick

		'Enables the user to instantly see the results of the configuration changes
		'on the top right hand side of the form where it displays the main outputs.
		'The same information is also displayed in the Diagnostics tab where staging
		'results are also available, this is mainly used for testing but could also
		'be used as supporting documentation.

		Try

			If Not ssmTOOL Is Nothing Then

				txtBasElectrical.Text = ssmTOOL.ElectricalWBase.Value().ToString("F4", CultureInfo.InvariantCulture)
				txtBaseMechanical.Text = ssmTOOL.MechanicalWBase.Value().ToString("F4", CultureInfo.InvariantCulture)

				txtAdjElectrical.Text = ssmTOOL.ElectricalWAdjusted.Value().ToString("F4", CultureInfo.InvariantCulture)
				txtAdjMechanical.Text = ssmTOOL.MechanicalWBaseAdjusted.Value().ToString("F4", CultureInfo.InvariantCulture)

				If captureDiagnostics Then

					txtDiagnostics.Text = ssmTOOL.ToString()

					captureDiagnostics = False

				End If

			End If

		Catch Ex As SystemException

			MessageBox.Show("An unexpected error occured during the timer click recalculation.")

		End Try
	End Sub

	Private Sub btnEnvironmentConditionsSource_Click(sender As Object, e As EventArgs) _
		Handles btnEnvironmentConditionsSource.Click

		Dim ecFileBrowser As New FileBrowser("AAUXEnv", True, False)

		ecFileBrowser.Extensions = New String() {"aenv"}

		If ecFileBrowser.OpenDialog(path.GetDirectoryName(vectoFile)) Then

			txtEC_EnvironmentConditionsFilePath.Tag = ecFileBrowser.Files(0)
			txtEC_EnvironmentConditionsFilePath.Text = GetRelativePath(ecFileBrowser.Files(0), path.GetDirectoryName(vectoFile))

			txtEC_EnvironmentConditionsFilePath.Focus()
			txtAH_FuelFiredHeaterkW.Focus()

		End If
	End Sub

	Private Sub chkEC_BatchMode_CheckedChanged(sender As Object, e As EventArgs) Handles chkEC_BatchMode.CheckedChanged

		If (chkEC_BatchMode.Checked) Then
			txtEC_EnviromentalTemperature.ReadOnly = True
			txtEC_Solar.ReadOnly = True
			txtEC_EnvironmentConditionsFilePath.ReadOnly = False
			btnEnvironmentConditionsSource.Enabled = True
			btnOpenAenv.Enabled = True
		Else
			txtEC_EnviromentalTemperature.ReadOnly = False
			txtEC_Solar.ReadOnly = False
			txtEC_EnvironmentConditionsFilePath.ReadOnly = True
			btnEnvironmentConditionsSource.Enabled = False
			btnOpenAenv.Enabled = False
		End If
	End Sub

	' File Helpers
	Private cmFilesList As String()

	Private Sub btnOpenECDB_Click(sender As Object, e As EventArgs) Handles btnOpenAenv.Click
		OpenFiles(path.Combine(Me.txtEC_EnvironmentConditionsFilePath.Text, path.GetDirectoryName(vectoFile)))
	End Sub

	Public Function FileOpenAlt(ByVal file As String) As Boolean
		Dim PSI As New ProcessStartInfo

		If Not IO.File.Exists(file) Then Return False

		PSI.FileName = "notepad.exe"
		PSI.Arguments = ChrW(34) & file & ChrW(34)
		Try
			Process.Start(PSI)
			Return True
		Catch ex As Exception
			Return False
		End Try
	End Function

	Private Sub OpenFiles(ParamArray files() As String)

		If files.Length = 0 Then Exit Sub

		cmFilesList = files

		CMFiles.Show(Cursor.Position)
	End Sub

	Private Sub OpenWithToolStripMenuItem_Click(sender As System.Object, e As System.EventArgs) _
		Handles OpenWithToolStripMenuItem.Click
		If Not FileOpenAlt(cmFilesList(0)) Then MsgBox("Failed to open file!")
	End Sub

	Private Sub ShowInFolderToolStripMenuItem_Click(sender As System.Object, e As System.EventArgs) _
		Handles ShowInFolderToolStripMenuItem.Click
		If IO.File.Exists(cmFilesList(0)) Then
			Try
				System.Diagnostics.Process.Start("explorer", "/select,""" & cmFilesList(0) & "")
			Catch ex As Exception
				MsgBox("Failed to open file!")
			End Try
		Else
			MsgBox("File not found!")
		End If
	End Sub
End Class
