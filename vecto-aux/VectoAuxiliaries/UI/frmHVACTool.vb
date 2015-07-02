Imports System.Windows.Forms
Imports VectoAuxiliaries.Hvac
Imports System.ComponentModel
Imports System.Drawing

Public Class frmHVACTool

    'Fields
    Private captureDiagnostics As Boolean
    Private busDatabasePath As String
    Private ahsmFilePath As String
    Private buses As IBusDatabase
    Private ssmTOOL As SSMTOOL
    Private originalssmTOOL As SSMTOOL
    Private TabColors As Dictionary(Of TabPage, Color) = New Dictionary(Of TabPage, Color)()
    Private editTechLine As ITechListBenefitLine = New TechListBenefitLine(Nothing)
    Private gvTechListBinding As BindingList(Of ITechListBenefitLine)
    Private DefaultCategories As String() = {"Cooling", "Heating", "Insulation", "Ventiliation"}
    Private vectoFile As String = String.Empty
    Private UserHitCancel As Boolean = False
    Private UserHitSave As Boolean = False
    Private IsAddingBus As Boolean = False
    Private IsUpdatingBus As Boolean = False
    Private busesList As BindingList(Of IBus)

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

        Dim gvTechListBinding As New BindingList(Of ITechListBenefitLine)(ssmTOOL.TechList.TechLines.OrderBy(Function(o) o.Category).ThenBy(Function(t) t.BenefitName).ToList())
        Me.gvTechBenefitLines.DataSource = gvTechListBinding

    End Sub
    Private Function GetCategories() As List(Of String)

        If Not ssmTOOL Is Nothing AndAlso Not ssmTOOL.TechList Is Nothing AndAlso ssmTOOL.TechList.TechLines.Count > 0 Then

            'Fuse Lists          
            Dim fusedList As List(Of String) = DefaultCategories.ToList()

            For Each s As String In ssmTOOL.TechList.TechLines.Select(Function(sel) sel.Category)

                If Not fusedList.Contains(s) Then
                    fusedList.Add(s)
                End If

            Next

            Return fusedList.OrderBy(Function(o) o.ToString()).ToList()

        Else

            Return New List(Of String)(DefaultCategories)

        End If


    End Function

    'Constructors
    Public Sub New(busDatabasePath As String, ahsmFilePath As String, vectoFilePath As String, Optional useDefaults As Boolean = False)

        ' This call is required by the designer.
        InitializeComponent()

        vectoFile = vectoFilePath

        'Add any initialization after the InitializeComponent() call.


        Me.busDatabasePath = busDatabasePath
        Me.ahsmFilePath = ahsmFilePath



        ssmTOOL = New SSMTOOL(ahsmFilePath, useDefaults)
        originalssmTOOL = New SSMTOOL(ahsmFilePath, useDefaults)

        If ssmTOOL.Load(ahsmFilePath) AndAlso originalssmTOOL.Load(ahsmFilePath) Then
            Timer1.Enabled = True
        Else

            MessageBox.Show("The file format for the Steady State Model (.AHSM) was corrupted or is an alpha version. Please refer to the documentation or help to discover more.")
            Timer1.Enabled = False

        End If

        setupBuses()
        setupControls()
        setupBindings()


    End Sub

    'Setup Methods
    Private Sub setupBuses()

        'Setup Buses
        buses = New BusDatabase()
        If Not buses.Initialise(busDatabasePath) Then
            MessageBox.Show("Problems initialising the Bus Database, some buses may not appear")
        End If

        busesList = New BindingList(Of IBus)(buses.GetBuses(String.Empty, True))

        cboBuses.DataSource = busesList
        cboBuses.DisplayMember = "Model"
    End Sub
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

        txtEC_EnvironmentConditionsFilePath.Tag = ssmTOOL.GenInputs.EC_EnviromentalConditions_BatchFile
        txtEC_EnvironmentConditionsFilePath.Text = fFileWoDir(ssmTOOL.GenInputs.EC_EnviromentalConditions_BatchFile, fPATH(vectoFile))
        txtEC_EnvironmentConditionsFilePath.ReadOnly = True
        btnEnvironmentConditionsSource.Enabled = False
        btnOpenAenv.Enabled = False

        btnNewBus.Tag = "New"
        btnEditBus.Tag = "Edit"

        BusParamGroupEdit.Location = New Point(30, 51)
    End Sub
    Private Sub setupBindings()

        UpdateButtonText()

        'TechBenefitLines
        BindGrid()

        'Bus Parameterisation
        txtBusModel.DataBindings.Add("Text", ssmTOOL.GenInputs, "BP_BusModel", False, DataSourceUpdateMode.OnPropertyChanged)
        txtRegisteredPassengers.DataBindings.Add("Text", ssmTOOL.GenInputs, "BP_NumberOfPassengers", False, DataSourceUpdateMode.OnPropertyChanged)
        cmbBusFloorType.DataBindings.Add("Text", ssmTOOL.GenInputs, "BP_BusFloorType", False, DataSourceUpdateMode.OnPropertyChanged)
        chkIsDoubleDecker.DataBindings.Add("Checked", ssmTOOL.GenInputs, "BP_DoubleDecker", False, DataSourceUpdateMode.OnPropertyChanged)
        txtBusLength.DataBindings.Add("Text", ssmTOOL.GenInputs, "BP_BusLength")
        txtBusWidth.DataBindings.Add("Text", ssmTOOL.GenInputs, "BP_BusWidth")
        txtBusHeight.DataBindings.Add("Text", ssmTOOL.GenInputs, "BP_BusHeight")

        txtBusFloorSurfaceArea.DataBindings.Add("Text", ssmTOOL.GenInputs, "BP_BusFloorSurfaceArea", False, DataSourceUpdateMode.OnPropertyChanged)
        txtBusWindowSurfaceArea.DataBindings.Add("Text", ssmTOOL.GenInputs, "BP_BusWindowSurface")
        txtBusSurfaceArea.DataBindings.Add("Text", ssmTOOL.GenInputs, "BP_BusSurfaceAreaM2")
        txtBusVolume.DataBindings.Add("Text", ssmTOOL.GenInputs, "BP_BusVolume")

        'Boundary Conditions
        txtBC_GFactor.DataBindings.Add("Text", ssmTool.genInputs, "BC_GFactor")
        txtBC_SolarClouding.DataBindings.Add("Text", ssmTool.genInputs, "BC_SolarClouding")
        txtBC_HeatPerPassengerIntoCabinW.DataBindings.Add("Text", ssmTool.genInputs, "BC_HeatPerPassengerIntoCabinW")
        txtBC_PassengerBoundaryTemperature.DataBindings.Add("Text", ssmTool.genInputs, "BC_PassengerBoundaryTemperature")
        txtBC_PassengerDensityLowFloor.DataBindings.Add("Text", ssmTool.genInputs, "BC_PassengerDensityLowFloor")
        txtBC_PassengerDensitySemiLowFloor.DataBindings.Add("Text", ssmTool.genInputs, "BC_PassengerDensitySemiLowFloor")
        txtBC_PassengerDensityRaisedFloor.DataBindings.Add("Text", ssmTool.genInputs, "BC_PassengerDensityRaisedFloor")
        txtBC_CalculatedPassengerNumber.DataBindings.Add("Text", ssmTool.genInputs, "BC_CalculatedPassengerNumber")
        txtBC_UValues.DataBindings.Add("Text", ssmTool.genInputs, "BC_UValues")
        txtBC_HeatingBoundaryTemperature.DataBindings.Add("Text", ssmTool.genInputs, "BC_HeatingBoundaryTemperature")
        txtBC_CoolingBoundaryTemperature.DataBindings.Add("Text", ssmTool.genInputs, "BC_CoolingBoundaryTemperature")
        txtBC_HighVentilation.DataBindings.Add("Text", ssmTool.genInputs, "BC_HighVentilation")
        txtBC_lowVentilation.DataBindings.Add("Text", ssmTool.genInputs, "BC_lowVentilation")
        txtBC_High.DataBindings.Add("Text", ssmTool.genInputs, "BC_High")
        txtBC_Low.DataBindings.Add("Text", ssmTool.genInputs, "BC_Low")
        txtBC_HighVentPowerW.DataBindings.Add("Text", ssmTool.genInputs, "BC_HighVentPowerW")
        txtBC_LowVentPowerW.DataBindings.Add("Text", ssmTool.genInputs, "BC_LowVentPowerW")
        txtBC_SpecificVentilationPower.DataBindings.Add("Text", ssmTool.genInputs, "BC_SpecificVentilationPower")
        txtBC_COP.DataBindings.Add("Text", ssmTool.genInputs, "BC_COP")
        txtBC_AuxHeaterEfficiency.DataBindings.Add("Text", ssmTool.genInputs, "BC_AuxHeaterEfficiency")
        txtBC_GCVDieselOrHeatingOil.DataBindings.Add("Text", ssmTool.genInputs, "BC_GCVDieselOrHeatingOil")
        txtBC_VolumicMassDieselOrHeatingOil.DataBindings.Add("Text", ssmTool.genInputs, "BC_VolumicMassDieselOrHeatingOil")
        txtBC_WindowAreaPerUnitBusLength.DataBindings.Add("Text", ssmTool.genInputs, "BC_WindowAreaPerUnitBusLength")
        txtBC_FrontRearWindowArea.DataBindings.Add("Text", ssmTool.genInputs, "BC_FrontRearWindowArea")
        txtBC_MaxTemperatureDeltaForLowFloorBusses.DataBindings.Add("Text", ssmTool.genInputs, "BC_MaxTemperatureDeltaForLowFloorBusses")
        txtBC_MaxPossibleBenefitFromTechnologyList.DataBindings.Add("Text", ssmTOOL.GenInputs, "BC_MaxPossibleBenefitFromTechnologyList")

        'General Inputs Other   
        'EnviromentalConditions	        		
        txtEC_EnviromentalTemperature.DataBindings.Add("Text", ssmTool.genInputs, "EC_EnviromentalTemperature")
        txtEC_Solar.DataBindings.Add("Text", ssmTOOL.GenInputs, "EC_Solar")
        chkEC_BatchMode.DataBindings.Add("Checked", ssmTOOL.GenInputs, "EC_EnviromentalConditions_BatchEnabled", False, DataSourceUpdateMode.OnPropertyChanged)

        'AC-system	
        chkAC_InCabinRoomAC_System.DataBindings.Add("Checked", ssmTool.genInputs, "AC_InCabinRoomAC_System", False, DataSourceUpdateMode.OnPropertyChanged)
        cboAC_CompressorType.DataBindings.Add("Text", ssmTool.genInputs, "AC_CompressorType", False, DataSourceUpdateMode.OnPropertyChanged)
        txtAC_CompressorCapacitykW.DataBindings.Add("Text", ssmTool.genInputs, "AC_CompressorCapacitykW")

        'Ventilation	
        chkVEN_VentilationOnDuringHeating.DataBindings.Add("Checked", ssmTool.genInputs, "VEN_VentilationOnDuringHeating", False, DataSourceUpdateMode.OnPropertyChanged)
        chkVEN_VentilationWhenBothHeatingAndACInactive.DataBindings.Add("Checked", ssmTool.genInputs, "VEN_VentilationWhenBothHeatingAndACInactive", False, DataSourceUpdateMode.OnPropertyChanged)
        chkVEN_VentilationDuringAC.DataBindings.Add("Checked", ssmTool.genInputs, "VEN_VentilationDuringAC", False, DataSourceUpdateMode.OnPropertyChanged)
        cboVEN_VentilationFlowSettingWhenHeatingAndACInactive.DataBindings.Add("Text", ssmTool.genInputs, "VEN_VentilationFlowSettingWhenHeatingAndACInactive")
        cboVEN_VentilationDuringHeating.DataBindings.Add("Text", ssmTool.genInputs, "VEN_VentilationDuringHeating")
        cboVEN_VentilationDuringCooling.DataBindings.Add("Text", ssmTool.genInputs, "VEN_VentilationDuringCooling")

        'Aux. Heater  
        txtAH_FuelFiredHeaterkW.DataBindings.Add("Text", ssmTOOL.GenInputs, "AH_FuelFiredHeaterkW")


    End Sub

    'GeneralInputControlEvents
    Private Sub cboBuses_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cboBuses.SelectedIndexChanged

        If cboBuses.SelectedIndex > 0 Then

            Dim bus As IBus = DirectCast(cboBuses.SelectedItem, IBus)

            ssmTOOL.GenInputs.BP_BusModel = bus.Model
            ssmTOOL.GenInputs.BP_NumberOfPassengers = bus.RegisteredPassengers
            ssmTOOL.GenInputs.BP_BusFloorType = bus.FloorType
            ssmTOOL.GenInputs.BP_DoubleDecker = bus.IsDoubleDecker
            ssmTOOL.GenInputs.BP_BusLength = bus.LengthInMetres
            ssmTOOL.GenInputs.BP_BusWidth = bus.WidthInMetres
            ssmTOOL.GenInputs.BP_BusHeight = bus.HeightInMetres

            txtBusModel.Text = bus.Model
            txtRegisteredPassengers.Text = bus.RegisteredPassengers
            cmbBusFloorType.Text = bus.FloorType
            txtBusLength.Text = bus.LengthInMetres
            txtBusWidth.Text = bus.WidthInMetres
            txtBusHeight.Text = bus.HeightInMetres
            chkIsDoubleDecker.Checked = bus.IsDoubleDecker

            txtBusFloorSurfaceArea.Text = ssmTOOL.GenInputs.BP_BusFloorSurfaceArea
            txtBusWindowSurfaceArea.Text = ssmTOOL.GenInputs.BP_BusWindowSurface
            txtBusSurfaceArea.Text = ssmTOOL.GenInputs.BP_BusSurfaceAreaM2
            txtBusVolume.Text = ssmTOOL.GenInputs.BP_BusVolume

            btnEditBus.Enabled = True

        Else
            btnEditBus.Enabled = False
        End If


    End Sub

    'Validators
    Public Sub Validating_GeneralInputsBP(sender As Object, e As CancelEventArgs) Handles txtRegisteredPassengers.Validating, txtBusWidth.Validating, txtBusHeight.Validating, txtBusLength.Validating, txtBusModel.Validating

        e.Cancel = Not Validate_GeneralInputsBP()

    End Sub
    Public Sub Validating_GeneralInputsBC(sender As Object, e As CancelEventArgs) Handles txtBC_GFactor.Validating, txtBC_VolumicMassDieselOrHeatingOil.Validating, txtBC_SpecificVentilationPower.Validating, txtBC_PassengerDensitySemiLowFloor.Validating, txtBC_PassengerDensityRaisedFloor.Validating, txtBC_PassengerDensityLowFloor.Validating, txtBC_MaxTemperatureDeltaForLowFloorBusses.Validating, txtBC_MaxPossibleBenefitFromTechnologyList.Validating, txtBC_lowVentilation.Validating, txtBC_HighVentilation.Validating, txtBC_HeatingBoundaryTemperature.Validating, txtBC_GCVDieselOrHeatingOil.Validating, txtBC_COP.Validating, txtBC_CoolingBoundaryTemperature.Validating, txtBC_AuxHeaterEfficiency.Validating, txtBC_PassengerBoundaryTemperature.Validating

        e.Cancel = Not Validate_GeneralInputsBC()

    End Sub
    Public Sub Validating_GeneralInputsOther(sender As Object, e As CancelEventArgs) Handles txtEC_Solar.Validating, txtEC_EnviromentalTemperature.Validating, txtAH_FuelFiredHeaterkW.Validating, txtAC_CompressorCapacitykW.Validating, txtEC_EnvironmentConditionsFilePath.Validating

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
            ErrorProvider1.SetError(txtRegisteredPassengers, "Please enter an integer of one or higher ( Bus : Number of Passengers )")
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
            ErrorProvider1.SetError(txtEditBusPassengers, "Please enter an integer of one or higher ( Bus : Number of Passengers )")
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
        'BC_SolarClouding				      : Calculated    
        'BC_HeatPerPassengerIntoCabinW	      : Calculated             
        'txtBC_PassengerBoundaryTemperature    
        IsTextBoxNumber(txtBC_PassengerBoundaryTemperature, "Please enter a number ( Passenger Boundary Temperature )", result)
        'txtBC_PassengerDensityLowFloor   
        IsTextBoxNumber(txtBC_PassengerDensityLowFloor, "Please enter a number ( Passenger Density Low Floor )", result)
        'txtBC_PassengerDensitySemiLowFloor	 
        IsTextBoxNumber(txtBC_PassengerDensitySemiLowFloor, "Please enter a number ( Passenger Density Semi Low Floor )", result)
        'txtBC_PassengerDensityRaisedFloor	
        IsTextBoxNumber(txtBC_PassengerDensityRaisedFloor, "Please enter a number ( Passenger Density Raised Floor )", result)
        'txtBC_CalculatedPassengerNumber	: Calculated          
        'txtBC_UValues                      : Calculated                            
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
        IsTextBoxNumber(txtBC_COP, "Please enter a number ( COP )", result)
        'txtBC_AuxHeaterEfficiency		
        IsTextBoxNumber(txtBC_AuxHeaterEfficiency, "Please enter a number ( Aux Heater Efficiency )", result)
        'txtBC_GCVDieselOrHeatingOil   
        IsTextBoxNumber(txtBC_GCVDieselOrHeatingOil, "Please enter a number ( GCV Diesel Or Heating Oil )", result)
        'txtBC_VolumicMassDieselOrHeatingOil	
        IsTextBoxNumber(txtBC_VolumicMassDieselOrHeatingOil, "Please enter a number ( Volumic Mass Diesel Or Heating Oil )", result)
        'txtBC_WindowAreaPerUnitBusLength	     : Calculated 
        'txtBC_FrontRearWindowArea               : Calculated                      
        'txtBC_MaxTemperatureDeltaForLowFloorBusses
        IsTextBoxNumber(txtBC_MaxTemperatureDeltaForLowFloorBusses, "Please enter a number ( Max Temp Delta For Low Floor Busses )", result)
        'txtBC_MaxPossibleBenefitFromTechnologyList
        IsTextBoxNumber(txtBC_MaxPossibleBenefitFromTechnologyList, "Please enter a number ( Max Benefit From Technology List )", result)

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

        Try
            Dim environmentalConditionsMap As IEnvironmentalConditionsMap = New EnvironmentalConditionsMap(txtEC_EnvironmentConditionsFilePath.Tag)
            ErrorProvider1.SetError(txtEC_EnvironmentConditionsFilePath, String.Empty)
            ssmTOOL.GenInputs.EC_EnviromentalConditions_BatchFile = txtEC_EnvironmentConditionsFilePath.Tag
        Catch ex As Exception
            ErrorProvider1.SetError(txtEC_EnvironmentConditionsFilePath, "Error : The environment conditions file is invalid or cannot be found, please select a valid aenv file.")
            result = False
        End Try

        'Set Tab Color
        UpdateTabStatus("tabGeneralInputsOther", result)

        Return result

    End Function
    Public Sub Validating_TechLineEdit(sender As Object, e As CancelEventArgs) 'Handles txtSemiLowFloorV.Validating, txtSemiLowFloorH.Validating, txtSemiLowFloorC.Validating, txtRaisedFloorV.Validating, txtRaisedFloorH.Validating, txtRaisedFloorC.Validating, txtLowFloorV.Validating, txtLowFloorH.Validating, txtLowFloorC.Validating, txtBenefitName.Validating, chkOnVehicle.Validating, chkActiveVV.Validating, chkActiveVH.Validating, chkActiveVC.Validating, cboUnits.Validating, cboLineType.Validating, cboCategory.Validating

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

        Dim number As Single

        If Not Integer.TryParse(test, number) Then Return False

        If number <= 0 Then Return False


        Return True

    End Function
    Private Function IsPostiveNumber(ByVal test As String) As Boolean

        'Is this numeric sanity check.
        If Not IsNumeric(test) Then Return False

        Dim number As Single

        If Not Double.TryParse(test, number) Then Return False

        If number <= 0 Then Return False


        Return True

    End Function
    Private Function IsNumberGreaterThanOne(ByVal test As String) As Boolean

        'Is this numeric sanity check.
        If Not IsNumeric(test) Then Return False

        Dim number As Single

        If Not Double.TryParse(test, number) Then Return False

        If number <= 1 Then Return False


        Return True

    End Function
    Private Function IsZeroOrPostiveNumber(ByVal test As String) As Boolean

        'Is this numeric sanity check.
        If Not IsNumeric(test) Then Return False

        Dim number As Single

        If Not Double.TryParse(test, number) Then Return False

        If number < 0 Then Return False


        Return True

    End Function
    Private Function IsNumberBetweenZeroandOne(test As String) As Boolean

        'Is this numeric sanity check.
        If Not IsNumeric(test) Then Return False

        Dim number As Single

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
            e.Graphics.DrawString(tabMain.TabPages(e.Index).Text, e.Font, Brushes.Black, e.Bounds.Left + (e.Bounds.Width - sz.Width) / 2, e.Bounds.Top + (e.Bounds.Height - sz.Height) / 2 + 1)

            Dim rect As Rectangle = e.Bounds
            rect.Offset(-1, -1)
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
            If Not ssmTOOL.Save(ahsmFilePath) Then
                MessageBox.Show("Unable to save file, aborting.")
                e.Cancel = True
            End If
            DialogResult = Windows.Forms.DialogResult.OK
            UserHitSave = False
            Return
        End If


        'This must be a close box event. If nothing changed, then bail, otherwise ask user if they wanna save
        If Not ssmTOOL.IsEqualTo(originalssmTOOL) Then

            result = (MessageBox.Show("Would you like to save changes before closing?", "Save Changes", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question))


            Select Case result

                Case DialogResult.Yes
                    'save 

                    If Not ssmTOOL.Save(ahsmFilePath) Then
                        e.Cancel = True
                    End If

                Case DialogResult.No
                    'just allow the form to close
                    'without saving
                    Me.DialogResult = Windows.Forms.DialogResult.Cancel



                Case DialogResult.Cancel
                    'cancel the close
                    e.Cancel = True
                    Me.DialogResult = Windows.Forms.DialogResult.Cancel


            End Select

        End If

        UserHitCancel = False
        UserHitSave = False

    End Sub

    'Grid Events
    Private Sub gvTechBenefitLines_DoubleClick(sender As Object, e As EventArgs) Handles gvTechBenefitLines.DoubleClick

        If gvTechBenefitLines.SelectedCells.Count < 1 Then Return


        Dim row As Integer = gvTechBenefitLines.SelectedCells(0).OwningRow.Index

        Dim benefitName, category, units As String
        benefitName = gvTechBenefitLines.Rows(row).Cells("BenefitName").Value
        category = gvTechBenefitLines.Rows(row).Cells("Category").Value


        editTechLine = ssmTOOL.TechList.TechLines.First(Function(f) f.BenefitName = benefitName AndAlso f.Category = category)

        If editTechLine.Units.ToLower = "kw" Then

            ClearEditPanel()
            MessageBox.Show("KW Unit types not supported, any KW Units in list are for test purposes only")
            Return

        End If


        FillTechLineEditPanel(row)

        UpdateButtonText()



    End Sub
    Private Sub gvTechBenefitLines_CellClick(sender As Object, e As DataGridViewCellEventArgs) Handles gvTechBenefitLines.CellClick

        If e.ColumnIndex < 0 OrElse e.RowIndex < 0 Then Return


        If gvTechBenefitLines.Columns(e.ColumnIndex).Name = "Delete" OrElse gvTechBenefitLines.Columns(e.ColumnIndex).Name = "OnVehicle" Then

            Dim benefit As String = gvTechBenefitLines.Rows(e.RowIndex).Cells(1).Value
            Dim category As String = gvTechBenefitLines.Rows(e.RowIndex).Cells(0).Value
            Dim feedback As String = String.Empty


            Select Case gvTechBenefitLines.Columns(e.ColumnIndex).Name


                Case "Delete"
                    Dim dr As DialogResult = MessageBox.Show(String.Format("Do you want to delete benefit '{0}' ?", benefit), "", MessageBoxButtons.YesNo)
                    If dr = Windows.Forms.DialogResult.Yes Then
                        If ssmTOOL.TechList.Delete(New TechListBenefitLine With {.BenefitName = benefit, .Category = category}, feedback) Then
                            BindGrid()
                        End If
                    End If

                Case "OnVehicle"
                    Dim onVehicle As Boolean = Not gvTechBenefitLines.Rows(e.RowIndex).Cells(e.ColumnIndex).Value

                    Dim fi As TechListBenefitLine = ssmTOOL.TechList.TechLines.Find(Function(f) (f.Category = category) AndAlso f.BenefitName = benefit)
                    fi.OnVehicle = onVehicle
                    ' ssmTOOL.TechList.TechLines.First( Function(x)  x.BenefitName= benefit AndAlso x.Category=category).OnVehicle=onVehicle  
                    ' BindGrid 
                    gvTechBenefitLines.Refresh()



            End Select



        End If

    End Sub
    Private Sub gvTechBenefitLines_CurrentCellDirtyStateChanged(sender As Object, e As EventArgs) Handles gvTechBenefitLines.CurrentCellDirtyStateChanged



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
            If Not ssmTOOL.TechList.Add(GetTechLineFromPanel(), feedback) Then
                MessageBox.Show(feedback)
            Else


                BindGrid()

                'find new row
                Dim ol As List(Of ITechListBenefitLine) = ssmTOOL.TechList.TechLines.OrderBy(Function(x) x.Category).ThenBy(Function(tb) tb.BenefitName).ToList()
                Dim item As ITechListBenefitLine = ol.First(Function(x) x.Category = GetTechLineFromPanel().Category AndAlso x.BenefitName = GetTechLineFromPanel().BenefitName)
                Dim idx As Integer = ol.IndexOf(item)

                gvTechBenefitLines.FirstDisplayedScrollingRowIndex = idx

                cboCategory.DataSource = GetCategories()

                UpdateButtonText()

            End If

        Else
            'This is an update
            If Not ssmTOOL.TechList.Modify(editTechLine, GetTechLineFromPanel(), feedback) Then
                MessageBox.Show(feedback)
            Else
                gvTechBenefitLines.Refresh()
                ClearEditPanel()
                UpdateButtonText()

            End If

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

        If btnNewBus.Tag = "New" Then

            IsAddingBus = True

            cboBuses.Enabled = False
            btnUpdateBusDatabase.Enabled = False
            btnEditBus.Enabled = False
            btnCancelBus.Enabled = True

            btnNewBus.Text = "Add"
            btnNewBus.Tag = "Add"

            BusParamGroupModel.Visible = False
            BusParamGroupEdit.Visible = True

        ElseIf btnNewBus.Tag = "Add" Then

            If Validate_GeneralInputsBPEdit() Then
                Dim newBus As IBus = New Bus(busesList.Count + 1, txtEditBusModel.Text, cmbEditFloorType.Text, cmbEditEngineType.Text, txtEditBusLength.Text, txtEditBusWidth.Text, txtEditBusHeight.Text, Convert.ToInt32(txtEditBusPassengers.Text), chkEditIsDoubleDecker.Checked)

                buses.AddBus(newBus)
                busesList.Add(newBus)

                cboBuses.Enabled = True
                btnUpdateBusDatabase.Enabled = True
                btnEditBus.Enabled = True
                btnCancelBus.Enabled = False

                BusParamGroupModel.Visible = True
                BusParamGroupEdit.Visible = False

                btnNewBus.Text = "New"
                btnNewBus.Tag = "New"

                cboBuses.SelectedIndex = busesList.Count - 1

                IsAddingBus = False
            End If

        End If
    End Sub
    Private Sub btnUpdateBusDatabase_Click(sender As Object, e As EventArgs) Handles btnUpdateBusDatabase.Click

        If buses.Save(busDatabasePath) Then
            MessageBox.Show("Buses database file saved successfully")
        Else
            MessageBox.Show("Failed to save buses database file")
        End If

    End Sub
    Private Sub btnEditBus_Click(sender As Object, e As EventArgs) Handles btnEditBus.Click

        If cboBuses.SelectedIndex > 0 Then

            If btnEditBus.Tag = "Edit" Then

                IsUpdatingBus = True

                cboBuses.Enabled = False
                btnUpdateBusDatabase.Enabled = False
                btnNewBus.Enabled = False
                btnCancelBus.Enabled = True

                btnEditBus.Text = "Save"
                btnEditBus.Tag = "Save"

                BusParamGroupModel.Visible = False
                BusParamGroupEdit.Visible = True

                Dim bus As IBus = DirectCast(cboBuses.SelectedItem, IBus)

                txtEditBusModel.Text = bus.Model
                cmbEditFloorType.Text = bus.FloorType
                cmbEditEngineType.Text = bus.EngineType
                txtEditBusLength.Text = bus.LengthInMetres
                txtEditBusWidth.Text = bus.WidthInMetres
                txtEditBusHeight.Text = bus.HeightInMetres
                txtEditBusPassengers.Text = bus.RegisteredPassengers

            ElseIf btnEditBus.Tag = "Save" Then

                If Validate_GeneralInputsBPEdit() Then

                    Dim bus As IBus = DirectCast(cboBuses.SelectedItem, IBus)

                    bus.Model = txtEditBusModel.Text
                    bus.FloorType = cmbEditFloorType.Text
                    bus.EngineType = cmbEditEngineType.Text
                    bus.LengthInMetres = txtEditBusLength.Text
                    bus.WidthInMetres = txtEditBusWidth.Text
                    bus.HeightInMetres = txtEditBusHeight.Text
                    bus.RegisteredPassengers = txtEditBusPassengers.Text
                    bus.IsDoubleDecker = chkEditIsDoubleDecker.Checked

                    buses.UpdateBus(bus.Id, bus)

                    cboBuses.Enabled = True
                    btnUpdateBusDatabase.Enabled = True
                    btnNewBus.Enabled = True
                    btnCancelBus.Enabled = False

                    BusParamGroupModel.Visible = True
                    BusParamGroupEdit.Visible = False

                    btnEditBus.Text = "Edit"
                    btnEditBus.Tag = "Edit"

                    Dim currentIndex As Integer = cboBuses.SelectedIndex
                    cboBuses.SelectedIndex = 0
                    cboBuses.SelectedIndex = currentIndex

                    IsUpdatingBus = False
                End If

            End If

        End If

    End Sub
    Private Sub btnCancelBus_Click(sender As Object, e As EventArgs) Handles btnCancelBus.Click

        IsUpdatingBus = False
        IsAddingBus = False

        cboBuses.Enabled = True
        btnUpdateBusDatabase.Enabled = True
        btnNewBus.Enabled = True
        btnCancelBus.Enabled = False

        If cboBuses.SelectedIndex > 0 Then
            btnEditBus.Enabled = True
        End If

        btnEditBus.Text = "Edit"
        btnEditBus.Tag = "Edit"

        btnNewBus.Text = "New"
        btnNewBus.Tag = "New"

        txtEditBusModel.Text = String.Empty
        cmbEditFloorType.Text = String.Empty
        cmbEditEngineType.Text = String.Empty
        txtEditBusLength.Text = String.Empty
        txtEditBusWidth.Text = String.Empty
        txtEditBusHeight.Text = String.Empty
        txtEditBusPassengers.Text = String.Empty
        chkEditIsDoubleDecker.Checked = False

        BusParamGroupModel.Visible = True
        BusParamGroupEdit.Visible = False

    End Sub

    'TechList Helpers
    Private Sub FillTechLineEditPanel(index As Integer)

        Dim techline As ITechListBenefitLine
        Dim benefitName, category As String
        benefitName = gvTechBenefitLines.Rows(index).Cells("BenefitName").Value
        category = gvTechBenefitLines.Rows(index).Cells("Category").Value

        techline = ssmTOOL.TechList.TechLines.First(Function(f) f.BenefitName = benefitName AndAlso f.Category = category)

        txtIndex.Text = index
        cboCategory.Text = techline.Category
        txtBenefitName.Text = techline.BenefitName
        cboUnits.Text = techline.Units
        cboLineType.Text = If(techline.LineType = 0, "Normal", "ActiveVentilation")
        txtLowFloorH.Text = techline.LowFloorH
        txtLowFloorV.Text = techline.LowFloorV
        txtLowFloorC.Text = techline.LowFloorC
        txtSemiLowFloorH.Text = techline.SemiLowFloorH
        txtSemiLowFloorV.Text = techline.SemiLowFloorV
        txtSemiLowFloorC.Text = techline.SemiLowFloorC
        txtRaisedFloorH.Text = techline.RaisedFloorH
        txtRaisedFloorV.Text = techline.RaisedFloorV
        txtRaisedFloorC.Text = techline.RaisedFloorC
        chkActiveVH.Checked = techline.ActiveVH
        chkActiveVV.Checked = techline.ActiveVV
        chkActiveVC.Checked = techline.ActiveVC
        chkOnVehicle.Checked = techline.OnVehicle



    End Sub
    Private Function GetTechLineFromPanel() As ITechListBenefitLine

        Dim tl As ITechListBenefitLine = New TechListBenefitLine(ssmTOOL.GenInputs)


        tl.Category = StrConv(cboCategory.Text, vbProperCase)
        tl.BenefitName = txtBenefitName.Text
        tl.Units = cboUnits.Text
        tl.LineType = If(cboLineType.Text = "Normal", 0, 3)
        tl.LowFloorH = txtLowFloorH.Text
        tl.LowFloorV = txtLowFloorV.Text
        tl.LowFloorC = txtLowFloorC.Text
        tl.SemiLowFloorH = txtSemiLowFloorH.Text
        tl.SemiLowFloorV = txtSemiLowFloorV.Text
        tl.SemiLowFloorC = txtSemiLowFloorC.Text
        tl.RaisedFloorH = txtRaisedFloorH.Text
        tl.RaisedFloorV = txtRaisedFloorV.Text
        tl.RaisedFloorC = txtRaisedFloorC.Text
        tl.ActiveVH = chkActiveVH.Checked
        tl.ActiveVV = chkActiveVV.Checked
        tl.ActiveVC = chkActiveVC.Checked
        tl.OnVehicle = chkOnVehicle.Checked


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

                txtBasElectrical.Text = ssmTOOL.ElectricalWBase
                txtBaseMechanical.Text = ssmTOOL.MechanicalWBase

                txtAdjElectrical.Text = ssmTOOL.ElectricalWAdjusted
                txtAdjMechanical.Text = ssmTOOL.MechanicalWBaseAdjusted

                If captureDiagnostics Then

                    txtDiagnostics.Text = ssmTOOL.ToString()

                    captureDiagnostics = False

                End If

            End If

        Catch Ex As SystemException

            MessageBox.Show("An unexpected error occured during the timer click recalculation.")

        End Try






    End Sub

    Private Sub btnEnvironmentConditionsSource_Click(sender As Object, e As EventArgs) Handles btnEnvironmentConditionsSource.Click

        Dim ecFileBrowser As New cFileBrowser(True, False)

        ecFileBrowser.Extensions = New String() {"aenv"}

        If ecFileBrowser.OpenDialog(fPATH(vectoFile)) Then

            txtEC_EnvironmentConditionsFilePath.Tag = ecFileBrowser.Files(0)
            txtEC_EnvironmentConditionsFilePath.Text = fFileWoDir(ecFileBrowser.Files(0), fPATH(vectoFile))

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
        OpenFiles(fFileRepl(Me.txtEC_EnvironmentConditionsFilePath.Text, fPATH(vectoFile)))
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

    Private Sub OpenWithToolStripMenuItem_Click(sender As System.Object, e As System.EventArgs) Handles OpenWithToolStripMenuItem.Click
        If Not FileOpenAlt(cmFilesList(0)) Then MsgBox("Failed to open file!")
    End Sub
    Private Sub ShowInFolderToolStripMenuItem_Click(sender As System.Object, e As System.EventArgs) Handles ShowInFolderToolStripMenuItem.Click
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
