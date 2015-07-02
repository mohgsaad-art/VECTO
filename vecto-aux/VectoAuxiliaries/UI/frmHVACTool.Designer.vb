<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmHVACTool
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmHVACTool))
        Me.tabMain = New System.Windows.Forms.TabControl()
        Me.tabGeneralInputsBP = New System.Windows.Forms.TabPage()
        Me.btnCancelBus = New System.Windows.Forms.Button()
        Me.BusParamGroupEdit = New System.Windows.Forms.GroupBox()
        Me.Label19 = New System.Windows.Forms.Label()
        Me.chkEditIsDoubleDecker = New System.Windows.Forms.CheckBox()
        Me.cmbEditEngineType = New System.Windows.Forms.ComboBox()
        Me.cmbEditFloorType = New System.Windows.Forms.ComboBox()
        Me.txtEditBusModel = New System.Windows.Forms.TextBox()
        Me.Label7 = New System.Windows.Forms.Label()
        Me.Label8 = New System.Windows.Forms.Label()
        Me.Label9 = New System.Windows.Forms.Label()
        Me.Label12 = New System.Windows.Forms.Label()
        Me.Label14 = New System.Windows.Forms.Label()
        Me.Label16 = New System.Windows.Forms.Label()
        Me.txtEditBusHeight = New System.Windows.Forms.TextBox()
        Me.txtEditBusWidth = New System.Windows.Forms.TextBox()
        Me.txtEditBusLength = New System.Windows.Forms.TextBox()
        Me.Label17 = New System.Windows.Forms.Label()
        Me.Label18 = New System.Windows.Forms.Label()
        Me.Label21 = New System.Windows.Forms.Label()
        Me.txtEditBusPassengers = New System.Windows.Forms.TextBox()
        Me.Label22 = New System.Windows.Forms.Label()
        Me.btnEditBus = New System.Windows.Forms.Button()
        Me.btnUpdateBusDatabase = New System.Windows.Forms.Button()
        Me.btnNewBus = New System.Windows.Forms.Button()
        Me.BusParamGroupModel = New System.Windows.Forms.GroupBox()
        Me.Label15 = New System.Windows.Forms.Label()
        Me.chkIsDoubleDecker = New System.Windows.Forms.CheckBox()
        Me.cmbBusFloorType = New System.Windows.Forms.ComboBox()
        Me.Label10 = New System.Windows.Forms.Label()
        Me.txtBusHeight = New System.Windows.Forms.TextBox()
        Me.Label11 = New System.Windows.Forms.Label()
        Me.txtBusModel = New System.Windows.Forms.TextBox()
        Me.lblBusModel = New System.Windows.Forms.Label()
        Me.lblBusFloorType = New System.Windows.Forms.Label()
        Me.lblUnitsBW = New System.Windows.Forms.Label()
        Me.lblUnitsBSA = New System.Windows.Forms.Label()
        Me.lblUnitsBWSA = New System.Windows.Forms.Label()
        Me.lblUnitsBV = New System.Windows.Forms.Label()
        Me.lblUnitsBL = New System.Windows.Forms.Label()
        Me.lblUnitsBFSA = New System.Windows.Forms.Label()
        Me.txtBusWidth = New System.Windows.Forms.TextBox()
        Me.txtBusLength = New System.Windows.Forms.TextBox()
        Me.txtBusVolume = New System.Windows.Forms.TextBox()
        Me.lblBusVolume = New System.Windows.Forms.Label()
        Me.lblBusLength = New System.Windows.Forms.Label()
        Me.lblBusWidth = New System.Windows.Forms.Label()
        Me.txtBusWindowSurfaceArea = New System.Windows.Forms.TextBox()
        Me.lblBusWindowSurfaceArea = New System.Windows.Forms.Label()
        Me.txtBusSurfaceArea = New System.Windows.Forms.TextBox()
        Me.lblBusSurfaceArea = New System.Windows.Forms.Label()
        Me.txtBusFloorSurfaceArea = New System.Windows.Forms.TextBox()
        Me.lblBusFloorSurfaceArea = New System.Windows.Forms.Label()
        Me.txtRegisteredPassengers = New System.Windows.Forms.TextBox()
        Me.lblRegisteredPassengers = New System.Windows.Forms.Label()
        Me.cboBuses = New System.Windows.Forms.ComboBox()
        Me.tabGeneralInputsBC = New System.Windows.Forms.TabPage()
        Me.GroupBox2 = New System.Windows.Forms.GroupBox()
        Me.lblUnitsBC_MaxPossibleBenefitFromTechnologyList = New System.Windows.Forms.Label()
        Me.txtBC_MaxPossibleBenefitFromTechnologyList = New System.Windows.Forms.TextBox()
        Me.lblBC_MaxPossibleBenefitFromTechnologyList = New System.Windows.Forms.Label()
        Me.lblUnitsBC_MaxTemperatureDeltaForLowFloorBusses = New System.Windows.Forms.Label()
        Me.txtBC_MaxTemperatureDeltaForLowFloorBusses = New System.Windows.Forms.TextBox()
        Me.lblBC_MaxTemperatureDeltaForLowFloorBusses = New System.Windows.Forms.Label()
        Me.lblUnitsBC_FrontRearWindowArea = New System.Windows.Forms.Label()
        Me.txtBC_FrontRearWindowArea = New System.Windows.Forms.TextBox()
        Me.lblBC_FrontRearWindowArea = New System.Windows.Forms.Label()
        Me.lblUnitsBC_WindowAreaPerUnitBusLength = New System.Windows.Forms.Label()
        Me.txtBC_WindowAreaPerUnitBusLength = New System.Windows.Forms.TextBox()
        Me.lblBC_WindowAreaPerUnitBusLength = New System.Windows.Forms.Label()
        Me.lblUnitsBC_VolumicMassDieselOrHeatingOil = New System.Windows.Forms.Label()
        Me.txtBC_VolumicMassDieselOrHeatingOil = New System.Windows.Forms.TextBox()
        Me.lblBC_VolumicMassDieselOrHeatingOil = New System.Windows.Forms.Label()
        Me.lblUnitsBC_GCVDieselOrHeatingOil = New System.Windows.Forms.Label()
        Me.txtBC_GCVDieselOrHeatingOil = New System.Windows.Forms.TextBox()
        Me.lblBC_GCVDieselOrHeatingOil = New System.Windows.Forms.Label()
        Me.txtBC_AuxHeaterEfficiency = New System.Windows.Forms.TextBox()
        Me.lblBC_AuxHeaterEfficiency = New System.Windows.Forms.Label()
        Me.lblUnitsBC_COP = New System.Windows.Forms.Label()
        Me.txtBC_COP = New System.Windows.Forms.TextBox()
        Me.lblBC_COP = New System.Windows.Forms.Label()
        Me.lblUnitsBC_SpecificVentilationPower = New System.Windows.Forms.Label()
        Me.txtBC_SpecificVentilationPower = New System.Windows.Forms.TextBox()
        Me.lvlBC_SpecificVentilationPower = New System.Windows.Forms.Label()
        Me.lblUnitsBC_LowVentPowerW = New System.Windows.Forms.Label()
        Me.txtBC_LowVentPowerW = New System.Windows.Forms.TextBox()
        Me.lblBC_LowVentPowerW = New System.Windows.Forms.Label()
        Me.lblUnitsBC_HighVentPowerW = New System.Windows.Forms.Label()
        Me.txtBC_HighVentPowerW = New System.Windows.Forms.TextBox()
        Me.lblBC_HighVentPowerW = New System.Windows.Forms.Label()
        Me.lblUnitsBC_Low = New System.Windows.Forms.Label()
        Me.txtBC_Low = New System.Windows.Forms.TextBox()
        Me.lblBC_Low = New System.Windows.Forms.Label()
        Me.lblUnitsBC_High = New System.Windows.Forms.Label()
        Me.txtBC_High = New System.Windows.Forms.TextBox()
        Me.lblBC_High = New System.Windows.Forms.Label()
        Me.lblUnitsBC_lowVentilation = New System.Windows.Forms.Label()
        Me.txtBC_lowVentilation = New System.Windows.Forms.TextBox()
        Me.lblBC_lowVentilation = New System.Windows.Forms.Label()
        Me.lblUnitsBC_HighVentilation = New System.Windows.Forms.Label()
        Me.txtBC_HighVentilation = New System.Windows.Forms.TextBox()
        Me.lblBC_HighVentilation = New System.Windows.Forms.Label()
        Me.lblUnitsBC_CoolingBoundaryTemperature = New System.Windows.Forms.Label()
        Me.txtBC_CoolingBoundaryTemperature = New System.Windows.Forms.TextBox()
        Me.lblBC_CoolingBoundaryTemperature = New System.Windows.Forms.Label()
        Me.lblUnitsBC_HeatingBoundaryTemperature = New System.Windows.Forms.Label()
        Me.txtBC_HeatingBoundaryTemperature = New System.Windows.Forms.TextBox()
        Me.lblBC_HeatingBoundaryTemperature = New System.Windows.Forms.Label()
        Me.txtBC_GFactor = New System.Windows.Forms.TextBox()
        Me.lblGFactor = New System.Windows.Forms.Label()
        Me.txtBC_HeatPerPassengerIntoCabinW = New System.Windows.Forms.TextBox()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.lblUnitsUValues = New System.Windows.Forms.Label()
        Me.lblUnitsPassengerBoundaryTemp = New System.Windows.Forms.Label()
        Me.lblUnitsPGRDensitySemiLowFloor = New System.Windows.Forms.Label()
        Me.lblUnitsPassenderDensityLowFloor = New System.Windows.Forms.Label()
        Me.lblUnitsBC_PassengerDensityRaisedFloor = New System.Windows.Forms.Label()
        Me.lblHeatPerPassengerIntoCabinW = New System.Windows.Forms.Label()
        Me.txtBC_UValues = New System.Windows.Forms.TextBox()
        Me.txtBC_CalculatedPassengerNumber = New System.Windows.Forms.TextBox()
        Me.txtBC_PassengerDensityRaisedFloor = New System.Windows.Forms.TextBox()
        Me.lblBC_PassengerDensityRaisedFloor = New System.Windows.Forms.Label()
        Me.lblBC_CalculatedPassengerNumber = New System.Windows.Forms.Label()
        Me.lblBC_UValues = New System.Windows.Forms.Label()
        Me.txtBC_PassengerDensitySemiLowFloor = New System.Windows.Forms.TextBox()
        Me.lblBC_PassengerDensitySemiLowFloor = New System.Windows.Forms.Label()
        Me.txtBC_PassengerDensityLowFloor = New System.Windows.Forms.TextBox()
        Me.Label13 = New System.Windows.Forms.Label()
        Me.txtBC_PassengerBoundaryTemperature = New System.Windows.Forms.TextBox()
        Me.lblPassengerBoundaryTemp = New System.Windows.Forms.Label()
        Me.txtBC_SolarClouding = New System.Windows.Forms.TextBox()
        Me.lblSolarClouding = New System.Windows.Forms.Label()
        Me.tabGeneralInputsOther = New System.Windows.Forms.TabPage()
        Me.grpEnvironmentConditions = New System.Windows.Forms.GroupBox()
        Me.btnOpenAenv = New System.Windows.Forms.Button()
        Me.btnEnvironmentConditionsSource = New System.Windows.Forms.Button()
        Me.txtEC_EnvironmentConditionsFilePath = New System.Windows.Forms.TextBox()
        Me.Label20 = New System.Windows.Forms.Label()
        Me.chkEC_BatchMode = New System.Windows.Forms.CheckBox()
        Me.Label23 = New System.Windows.Forms.Label()
        Me.txtEC_EnviromentalTemperature = New System.Windows.Forms.TextBox()
        Me.lbltxtEC_EnviromentalTemperature = New System.Windows.Forms.Label()
        Me.txtEC_Solar = New System.Windows.Forms.TextBox()
        Me.lbltxtEC_Solar = New System.Windows.Forms.Label()
        Me.lblUnitstxtEC_EnviromentalTemperature = New System.Windows.Forms.Label()
        Me.lblUnitstxtEC_Solar = New System.Windows.Forms.Label()
        Me.grpAuxHeater = New System.Windows.Forms.GroupBox()
        Me.lblUnitsAH_FuelFiredHeater = New System.Windows.Forms.Label()
        Me.txtAH_FuelFiredHeaterkW = New System.Windows.Forms.TextBox()
        Me.lbltxtAH_FuelFiredHeaterkW = New System.Windows.Forms.Label()
        Me.grpVentilation = New System.Windows.Forms.GroupBox()
        Me.cboVEN_VentilationDuringCooling = New System.Windows.Forms.ComboBox()
        Me.cboVEN_VentilationDuringHeating = New System.Windows.Forms.ComboBox()
        Me.cboVEN_VentilationFlowSettingWhenHeatingAndACInactive = New System.Windows.Forms.ComboBox()
        Me.chkVEN_VentilationDuringAC = New System.Windows.Forms.CheckBox()
        Me.chkVEN_VentilationWhenBothHeatingAndACInactive = New System.Windows.Forms.CheckBox()
        Me.chkVEN_VentilationOnDuringHeating = New System.Windows.Forms.CheckBox()
        Me.lblcboVEN_VentilationDuringCooling = New System.Windows.Forms.Label()
        Me.lblcboVEN_VentilationDuringHeating = New System.Windows.Forms.Label()
        Me.lblcboVEN_VentilationFlowSettingWhenHeatingAndACInactive = New System.Windows.Forms.Label()
        Me.lblchkVEN_VentilationOnDuringHeating = New System.Windows.Forms.Label()
        Me.lblchkVEN_VentilationDuringAC = New System.Windows.Forms.Label()
        Me.lblchkVEN_VentilationWhenBothHeatingAndACInactive = New System.Windows.Forms.Label()
        Me.grpACSystem = New System.Windows.Forms.GroupBox()
        Me.cboAC_CompressorType = New System.Windows.Forms.ComboBox()
        Me.chkAC_InCabinRoomAC_System = New System.Windows.Forms.CheckBox()
        Me.lblChkAC_InCabinRoomAC_System = New System.Windows.Forms.Label()
        Me.txtAC_CompressorCapacitykW = New System.Windows.Forms.TextBox()
        Me.lbltxtAC_CompressorCapacitykW = New System.Windows.Forms.Label()
        Me.lblUnitstxtAC_CompressorCapacitykW = New System.Windows.Forms.Label()
        Me.lblcboAC_CompressorType = New System.Windows.Forms.Label()
        Me.tabTechBenefits = New System.Windows.Forms.TabPage()
        Me.btnClearForm = New System.Windows.Forms.Button()
        Me.lblIndex = New System.Windows.Forms.Label()
        Me.txtIndex = New System.Windows.Forms.TextBox()
        Me.btnUpdate = New System.Windows.Forms.Button()
        Me.lblLineType = New System.Windows.Forms.Label()
        Me.chkActiveVC = New System.Windows.Forms.CheckBox()
        Me.lblCoolingColumn = New System.Windows.Forms.Label()
        Me.chkActiveVV = New System.Windows.Forms.CheckBox()
        Me.txtBenefitName = New System.Windows.Forms.TextBox()
        Me.chkActiveVH = New System.Windows.Forms.CheckBox()
        Me.lblBenefitName = New System.Windows.Forms.Label()
        Me.chkOnVehicle = New System.Windows.Forms.CheckBox()
        Me.lblVentelationColumn = New System.Windows.Forms.Label()
        Me.lblHeatingColumn = New System.Windows.Forms.Label()
        Me.cboLineType = New System.Windows.Forms.ComboBox()
        Me.pnlRaisedFloorRow = New System.Windows.Forms.Panel()
        Me.txtRaisedFloorH = New System.Windows.Forms.TextBox()
        Me.txtRaisedFloorC = New System.Windows.Forms.TextBox()
        Me.txtRaisedFloorV = New System.Windows.Forms.TextBox()
        Me.lblRaisedFloorRow = New System.Windows.Forms.Label()
        Me.pnlSemiLowFloorRow = New System.Windows.Forms.Panel()
        Me.txtSemiLowFloorH = New System.Windows.Forms.TextBox()
        Me.txtSemiLowFloorC = New System.Windows.Forms.TextBox()
        Me.txtSemiLowFloorV = New System.Windows.Forms.TextBox()
        Me.lblSemiLowFloorRow = New System.Windows.Forms.Label()
        Me.pnlLowFloorRow = New System.Windows.Forms.Panel()
        Me.txtLowFloorH = New System.Windows.Forms.TextBox()
        Me.txtLowFloorC = New System.Windows.Forms.TextBox()
        Me.txtLowFloorV = New System.Windows.Forms.TextBox()
        Me.lblLowFloorRow = New System.Windows.Forms.Label()
        Me.lblCategory = New System.Windows.Forms.Label()
        Me.cboCategory = New System.Windows.Forms.ComboBox()
        Me.gvTechBenefitLines = New System.Windows.Forms.DataGridView()
        Me.lblUnits = New System.Windows.Forms.Label()
        Me.cboUnits = New System.Windows.Forms.ComboBox()
        Me.tabDiagnostics = New System.Windows.Forms.TabPage()
        Me.txtDiagnostics = New System.Windows.Forms.TextBox()
        Me.ToolTip1 = New System.Windows.Forms.ToolTip(Me.components)
        Me.Label1 = New System.Windows.Forms.Label()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.ErrorProvider1 = New System.Windows.Forms.ErrorProvider(Me.components)
        Me.btnSave = New System.Windows.Forms.Button()
        Me.btnCancel = New System.Windows.Forms.Button()
        Me.txtBasElectrical = New System.Windows.Forms.TextBox()
        Me.txtBaseMechanical = New System.Windows.Forms.TextBox()
        Me.txtAdjMechanical = New System.Windows.Forms.TextBox()
        Me.txtAdjElectrical = New System.Windows.Forms.TextBox()
        Me.Timer1 = New System.Windows.Forms.Timer(Me.components)
        Me.lblElectricalBaseW = New System.Windows.Forms.Label()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.CMFiles = New System.Windows.Forms.ContextMenuStrip(Me.components)
        Me.OpenWithToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ShowInFolderToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.tabMain.SuspendLayout()
        Me.tabGeneralInputsBP.SuspendLayout()
        Me.BusParamGroupEdit.SuspendLayout()
        Me.BusParamGroupModel.SuspendLayout()
        Me.tabGeneralInputsBC.SuspendLayout()
        Me.GroupBox2.SuspendLayout()
        Me.tabGeneralInputsOther.SuspendLayout()
        Me.grpEnvironmentConditions.SuspendLayout()
        Me.grpAuxHeater.SuspendLayout()
        Me.grpVentilation.SuspendLayout()
        Me.grpACSystem.SuspendLayout()
        Me.tabTechBenefits.SuspendLayout()
        Me.pnlRaisedFloorRow.SuspendLayout()
        Me.pnlSemiLowFloorRow.SuspendLayout()
        Me.pnlLowFloorRow.SuspendLayout()
        CType(Me.gvTechBenefitLines, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.tabDiagnostics.SuspendLayout()
        CType(Me.ErrorProvider1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.CMFiles.SuspendLayout()
        Me.SuspendLayout()
        '
        'tabMain
        '
        Me.tabMain.Controls.Add(Me.tabGeneralInputsBP)
        Me.tabMain.Controls.Add(Me.tabGeneralInputsBC)
        Me.tabMain.Controls.Add(Me.tabGeneralInputsOther)
        Me.tabMain.Controls.Add(Me.tabTechBenefits)
        Me.tabMain.Controls.Add(Me.tabDiagnostics)
        Me.tabMain.DrawMode = System.Windows.Forms.TabDrawMode.OwnerDrawFixed
        Me.tabMain.Location = New System.Drawing.Point(8, 88)
        Me.tabMain.Name = "tabMain"
        Me.tabMain.SelectedIndex = 0
        Me.tabMain.Size = New System.Drawing.Size(945, 637)
        Me.tabMain.TabIndex = 0
        '
        'tabGeneralInputsBP
        '
        Me.tabGeneralInputsBP.Controls.Add(Me.btnCancelBus)
        Me.tabGeneralInputsBP.Controls.Add(Me.BusParamGroupEdit)
        Me.tabGeneralInputsBP.Controls.Add(Me.btnEditBus)
        Me.tabGeneralInputsBP.Controls.Add(Me.btnUpdateBusDatabase)
        Me.tabGeneralInputsBP.Controls.Add(Me.btnNewBus)
        Me.tabGeneralInputsBP.Controls.Add(Me.BusParamGroupModel)
        Me.tabGeneralInputsBP.Controls.Add(Me.cboBuses)
        Me.tabGeneralInputsBP.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.tabGeneralInputsBP.Location = New System.Drawing.Point(4, 22)
        Me.tabGeneralInputsBP.Name = "tabGeneralInputsBP"
        Me.tabGeneralInputsBP.Padding = New System.Windows.Forms.Padding(3)
        Me.tabGeneralInputsBP.Size = New System.Drawing.Size(937, 611)
        Me.tabGeneralInputsBP.TabIndex = 0
        Me.tabGeneralInputsBP.Text = " INP - BusParameters "
        Me.tabGeneralInputsBP.UseVisualStyleBackColor = True
        '
        'btnCancelBus
        '
        Me.btnCancelBus.Enabled = False
        Me.btnCancelBus.Location = New System.Drawing.Point(581, 22)
        Me.btnCancelBus.Name = "btnCancelBus"
        Me.btnCancelBus.Size = New System.Drawing.Size(74, 25)
        Me.btnCancelBus.TabIndex = 13
        Me.btnCancelBus.Text = "Cancel"
        Me.btnCancelBus.UseVisualStyleBackColor = True
        '
        'BusParamGroupEdit
        '
        Me.BusParamGroupEdit.BackColor = System.Drawing.Color.Transparent
        Me.BusParamGroupEdit.Controls.Add(Me.Label19)
        Me.BusParamGroupEdit.Controls.Add(Me.chkEditIsDoubleDecker)
        Me.BusParamGroupEdit.Controls.Add(Me.cmbEditEngineType)
        Me.BusParamGroupEdit.Controls.Add(Me.cmbEditFloorType)
        Me.BusParamGroupEdit.Controls.Add(Me.txtEditBusModel)
        Me.BusParamGroupEdit.Controls.Add(Me.Label7)
        Me.BusParamGroupEdit.Controls.Add(Me.Label8)
        Me.BusParamGroupEdit.Controls.Add(Me.Label9)
        Me.BusParamGroupEdit.Controls.Add(Me.Label12)
        Me.BusParamGroupEdit.Controls.Add(Me.Label14)
        Me.BusParamGroupEdit.Controls.Add(Me.Label16)
        Me.BusParamGroupEdit.Controls.Add(Me.txtEditBusHeight)
        Me.BusParamGroupEdit.Controls.Add(Me.txtEditBusWidth)
        Me.BusParamGroupEdit.Controls.Add(Me.txtEditBusLength)
        Me.BusParamGroupEdit.Controls.Add(Me.Label17)
        Me.BusParamGroupEdit.Controls.Add(Me.Label18)
        Me.BusParamGroupEdit.Controls.Add(Me.Label21)
        Me.BusParamGroupEdit.Controls.Add(Me.txtEditBusPassengers)
        Me.BusParamGroupEdit.Controls.Add(Me.Label22)
        Me.BusParamGroupEdit.FlatStyle = System.Windows.Forms.FlatStyle.Popup
        Me.BusParamGroupEdit.Font = New System.Drawing.Font("Microsoft Sans Serif", 10.0!)
        Me.BusParamGroupEdit.ForeColor = System.Drawing.SystemColors.MenuHighlight
        Me.BusParamGroupEdit.Location = New System.Drawing.Point(30, 409)
        Me.BusParamGroupEdit.Name = "BusParamGroupEdit"
        Me.BusParamGroupEdit.Size = New System.Drawing.Size(625, 352)
        Me.BusParamGroupEdit.TabIndex = 12
        Me.BusParamGroupEdit.TabStop = False
        Me.BusParamGroupEdit.Text = "Bus Parameterisation"
        Me.BusParamGroupEdit.Visible = False
        '
        'Label19
        '
        Me.Label19.AutoSize = True
        Me.Label19.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!)
        Me.Label19.ForeColor = System.Drawing.Color.Black
        Me.Label19.Location = New System.Drawing.Point(14, 151)
        Me.Label19.Name = "Label19"
        Me.Label19.Size = New System.Drawing.Size(101, 15)
        Me.Label19.TabIndex = 33
        Me.Label19.Text = "Is Double Decker"
        '
        'chkEditIsDoubleDecker
        '
        Me.chkEditIsDoubleDecker.AutoSize = True
        Me.chkEditIsDoubleDecker.Location = New System.Drawing.Point(179, 151)
        Me.chkEditIsDoubleDecker.Name = "chkEditIsDoubleDecker"
        Me.chkEditIsDoubleDecker.Size = New System.Drawing.Size(15, 14)
        Me.chkEditIsDoubleDecker.TabIndex = 32
        Me.chkEditIsDoubleDecker.UseVisualStyleBackColor = True
        '
        'cmbEditEngineType
        '
        Me.cmbEditEngineType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cmbEditEngineType.FormattingEnabled = True
        Me.cmbEditEngineType.Items.AddRange(New Object() {"diesel", "gas", "hybrid"})
        Me.cmbEditEngineType.Location = New System.Drawing.Point(179, 118)
        Me.cmbEditEngineType.Name = "cmbEditEngineType"
        Me.cmbEditEngineType.Size = New System.Drawing.Size(208, 24)
        Me.cmbEditEngineType.TabIndex = 28
        '
        'cmbEditFloorType
        '
        Me.cmbEditFloorType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cmbEditFloorType.FormattingEnabled = True
        Me.cmbEditFloorType.Items.AddRange(New Object() {"raised floor", "low floor", "semi low floor"})
        Me.cmbEditFloorType.Location = New System.Drawing.Point(179, 88)
        Me.cmbEditFloorType.Name = "cmbEditFloorType"
        Me.cmbEditFloorType.Size = New System.Drawing.Size(208, 24)
        Me.cmbEditFloorType.TabIndex = 27
        '
        'txtEditBusModel
        '
        Me.txtEditBusModel.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!)
        Me.txtEditBusModel.Location = New System.Drawing.Point(179, 30)
        Me.txtEditBusModel.Name = "txtEditBusModel"
        Me.txtEditBusModel.Size = New System.Drawing.Size(208, 21)
        Me.txtEditBusModel.TabIndex = 25
        '
        'Label7
        '
        Me.Label7.AutoSize = True
        Me.Label7.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!)
        Me.Label7.ForeColor = System.Drawing.Color.Black
        Me.Label7.Location = New System.Drawing.Point(14, 33)
        Me.Label7.Name = "Label7"
        Me.Label7.Size = New System.Drawing.Size(66, 15)
        Me.Label7.TabIndex = 24
        Me.Label7.Text = "Bus Model"
        '
        'Label8
        '
        Me.Label8.AutoSize = True
        Me.Label8.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!)
        Me.Label8.ForeColor = System.Drawing.Color.Black
        Me.Label8.Location = New System.Drawing.Point(14, 88)
        Me.Label8.Name = "Label8"
        Me.Label8.Size = New System.Drawing.Size(88, 15)
        Me.Label8.TabIndex = 22
        Me.Label8.Text = "Bus Floor Type"
        '
        'Label9
        '
        Me.Label9.AutoSize = True
        Me.Label9.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label9.Location = New System.Drawing.Point(296, 211)
        Me.Label9.Name = "Label9"
        Me.Label9.Size = New System.Drawing.Size(18, 15)
        Me.Label9.TabIndex = 21
        Me.Label9.Text = "m"
        Me.ToolTip1.SetToolTip(Me.Label9, "Linear Metres")
        '
        'Label12
        '
        Me.Label12.AutoSize = True
        Me.Label12.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label12.Location = New System.Drawing.Point(296, 235)
        Me.Label12.Name = "Label12"
        Me.Label12.Size = New System.Drawing.Size(18, 15)
        Me.Label12.TabIndex = 18
        Me.Label12.Text = "m"
        Me.ToolTip1.SetToolTip(Me.Label12, "Metres Cubed")
        '
        'Label14
        '
        Me.Label14.AutoSize = True
        Me.Label14.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label14.Location = New System.Drawing.Point(296, 180)
        Me.Label14.Name = "Label14"
        Me.Label14.Size = New System.Drawing.Size(18, 15)
        Me.Label14.TabIndex = 17
        Me.Label14.Text = "m"
        Me.ToolTip1.SetToolTip(Me.Label14, "Linear Metres")
        '
        'Label16
        '
        Me.Label16.AutoSize = True
        Me.Label16.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!)
        Me.Label16.ForeColor = System.Drawing.Color.Black
        Me.Label16.Location = New System.Drawing.Point(14, 238)
        Me.Label16.Name = "Label16"
        Me.Label16.Size = New System.Drawing.Size(67, 15)
        Me.Label16.TabIndex = 12
        Me.Label16.Text = "Bus Height"
        '
        'txtEditBusHeight
        '
        Me.txtEditBusHeight.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!)
        Me.txtEditBusHeight.Location = New System.Drawing.Point(179, 235)
        Me.txtEditBusHeight.Name = "txtEditBusHeight"
        Me.txtEditBusHeight.Size = New System.Drawing.Size(97, 21)
        Me.txtEditBusHeight.TabIndex = 31
        '
        'txtEditBusWidth
        '
        Me.txtEditBusWidth.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!)
        Me.txtEditBusWidth.Location = New System.Drawing.Point(179, 208)
        Me.txtEditBusWidth.Name = "txtEditBusWidth"
        Me.txtEditBusWidth.Size = New System.Drawing.Size(97, 21)
        Me.txtEditBusWidth.TabIndex = 30
        '
        'txtEditBusLength
        '
        Me.txtEditBusLength.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!)
        Me.txtEditBusLength.Location = New System.Drawing.Point(179, 179)
        Me.txtEditBusLength.Name = "txtEditBusLength"
        Me.txtEditBusLength.Size = New System.Drawing.Size(97, 21)
        Me.txtEditBusLength.TabIndex = 29
        '
        'Label17
        '
        Me.Label17.AutoSize = True
        Me.Label17.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!)
        Me.Label17.ForeColor = System.Drawing.Color.Black
        Me.Label17.Location = New System.Drawing.Point(14, 182)
        Me.Label17.Name = "Label17"
        Me.Label17.Size = New System.Drawing.Size(72, 15)
        Me.Label17.TabIndex = 11
        Me.Label17.Text = "Bus  Length"
        '
        'Label18
        '
        Me.Label18.AutoSize = True
        Me.Label18.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!)
        Me.Label18.ForeColor = System.Drawing.Color.Black
        Me.Label18.Location = New System.Drawing.Point(14, 211)
        Me.Label18.Name = "Label18"
        Me.Label18.Size = New System.Drawing.Size(65, 15)
        Me.Label18.TabIndex = 10
        Me.Label18.Text = "Bus  Width"
        '
        'Label21
        '
        Me.Label21.AutoSize = True
        Me.Label21.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!)
        Me.Label21.ForeColor = System.Drawing.Color.Black
        Me.Label21.Location = New System.Drawing.Point(14, 120)
        Me.Label21.Name = "Label21"
        Me.Label21.Size = New System.Drawing.Size(99, 15)
        Me.Label21.TabIndex = 4
        Me.Label21.Text = "Bus Engine Type"
        '
        'txtEditBusPassengers
        '
        Me.txtEditBusPassengers.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!)
        Me.txtEditBusPassengers.Location = New System.Drawing.Point(179, 59)
        Me.txtEditBusPassengers.Name = "txtEditBusPassengers"
        Me.txtEditBusPassengers.Size = New System.Drawing.Size(97, 21)
        Me.txtEditBusPassengers.TabIndex = 26
        '
        'Label22
        '
        Me.Label22.AutoSize = True
        Me.Label22.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!)
        Me.Label22.ForeColor = System.Drawing.Color.Black
        Me.Label22.Location = New System.Drawing.Point(14, 62)
        Me.Label22.Name = "Label22"
        Me.Label22.Size = New System.Drawing.Size(138, 15)
        Me.Label22.TabIndex = 0
        Me.Label22.Text = "Registered Passengers "
        '
        'btnEditBus
        '
        Me.btnEditBus.Enabled = False
        Me.btnEditBus.Location = New System.Drawing.Point(421, 22)
        Me.btnEditBus.Name = "btnEditBus"
        Me.btnEditBus.Size = New System.Drawing.Size(74, 25)
        Me.btnEditBus.TabIndex = 11
        Me.btnEditBus.Text = "Edit"
        Me.btnEditBus.UseVisualStyleBackColor = True
        '
        'btnUpdateBusDatabase
        '
        Me.btnUpdateBusDatabase.Enabled = False
        Me.btnUpdateBusDatabase.Location = New System.Drawing.Point(294, 22)
        Me.btnUpdateBusDatabase.Name = "btnUpdateBusDatabase"
        Me.btnUpdateBusDatabase.Size = New System.Drawing.Size(123, 25)
        Me.btnUpdateBusDatabase.TabIndex = 10
        Me.btnUpdateBusDatabase.Text = "Save Bus Database"
        Me.btnUpdateBusDatabase.UseVisualStyleBackColor = True
        '
        'btnNewBus
        '
        Me.btnNewBus.Location = New System.Drawing.Point(501, 22)
        Me.btnNewBus.Name = "btnNewBus"
        Me.btnNewBus.Size = New System.Drawing.Size(74, 25)
        Me.btnNewBus.TabIndex = 9
        Me.btnNewBus.Text = "New"
        Me.btnNewBus.UseVisualStyleBackColor = True
        '
        'BusParamGroupModel
        '
        Me.BusParamGroupModel.BackColor = System.Drawing.Color.Transparent
        Me.BusParamGroupModel.Controls.Add(Me.Label15)
        Me.BusParamGroupModel.Controls.Add(Me.chkIsDoubleDecker)
        Me.BusParamGroupModel.Controls.Add(Me.cmbBusFloorType)
        Me.BusParamGroupModel.Controls.Add(Me.Label10)
        Me.BusParamGroupModel.Controls.Add(Me.txtBusHeight)
        Me.BusParamGroupModel.Controls.Add(Me.Label11)
        Me.BusParamGroupModel.Controls.Add(Me.txtBusModel)
        Me.BusParamGroupModel.Controls.Add(Me.lblBusModel)
        Me.BusParamGroupModel.Controls.Add(Me.lblBusFloorType)
        Me.BusParamGroupModel.Controls.Add(Me.lblUnitsBW)
        Me.BusParamGroupModel.Controls.Add(Me.lblUnitsBSA)
        Me.BusParamGroupModel.Controls.Add(Me.lblUnitsBWSA)
        Me.BusParamGroupModel.Controls.Add(Me.lblUnitsBV)
        Me.BusParamGroupModel.Controls.Add(Me.lblUnitsBL)
        Me.BusParamGroupModel.Controls.Add(Me.lblUnitsBFSA)
        Me.BusParamGroupModel.Controls.Add(Me.txtBusWidth)
        Me.BusParamGroupModel.Controls.Add(Me.txtBusLength)
        Me.BusParamGroupModel.Controls.Add(Me.txtBusVolume)
        Me.BusParamGroupModel.Controls.Add(Me.lblBusVolume)
        Me.BusParamGroupModel.Controls.Add(Me.lblBusLength)
        Me.BusParamGroupModel.Controls.Add(Me.lblBusWidth)
        Me.BusParamGroupModel.Controls.Add(Me.txtBusWindowSurfaceArea)
        Me.BusParamGroupModel.Controls.Add(Me.lblBusWindowSurfaceArea)
        Me.BusParamGroupModel.Controls.Add(Me.txtBusSurfaceArea)
        Me.BusParamGroupModel.Controls.Add(Me.lblBusSurfaceArea)
        Me.BusParamGroupModel.Controls.Add(Me.txtBusFloorSurfaceArea)
        Me.BusParamGroupModel.Controls.Add(Me.lblBusFloorSurfaceArea)
        Me.BusParamGroupModel.Controls.Add(Me.txtRegisteredPassengers)
        Me.BusParamGroupModel.Controls.Add(Me.lblRegisteredPassengers)
        Me.BusParamGroupModel.FlatStyle = System.Windows.Forms.FlatStyle.Popup
        Me.BusParamGroupModel.Font = New System.Drawing.Font("Microsoft Sans Serif", 10.0!)
        Me.BusParamGroupModel.ForeColor = System.Drawing.SystemColors.MenuHighlight
        Me.BusParamGroupModel.Location = New System.Drawing.Point(30, 51)
        Me.BusParamGroupModel.Name = "BusParamGroupModel"
        Me.BusParamGroupModel.Size = New System.Drawing.Size(625, 352)
        Me.BusParamGroupModel.TabIndex = 8
        Me.BusParamGroupModel.TabStop = False
        Me.BusParamGroupModel.Text = "Bus Parameterisation"
        '
        'Label15
        '
        Me.Label15.AutoSize = True
        Me.Label15.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!)
        Me.Label15.ForeColor = System.Drawing.Color.Black
        Me.Label15.Location = New System.Drawing.Point(14, 116)
        Me.Label15.Name = "Label15"
        Me.Label15.Size = New System.Drawing.Size(101, 15)
        Me.Label15.TabIndex = 31
        Me.Label15.Text = "Is Double Decker"
        '
        'chkIsDoubleDecker
        '
        Me.chkIsDoubleDecker.AutoSize = True
        Me.chkIsDoubleDecker.Location = New System.Drawing.Point(179, 116)
        Me.chkIsDoubleDecker.Name = "chkIsDoubleDecker"
        Me.chkIsDoubleDecker.Size = New System.Drawing.Size(15, 14)
        Me.chkIsDoubleDecker.TabIndex = 30
        Me.chkIsDoubleDecker.UseVisualStyleBackColor = True
        '
        'cmbBusFloorType
        '
        Me.cmbBusFloorType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cmbBusFloorType.FormattingEnabled = True
        Me.cmbBusFloorType.Items.AddRange(New Object() {"raised floor", "low floor", "semi low floor"})
        Me.cmbBusFloorType.Location = New System.Drawing.Point(179, 85)
        Me.cmbBusFloorType.Name = "cmbBusFloorType"
        Me.cmbBusFloorType.Size = New System.Drawing.Size(208, 24)
        Me.cmbBusFloorType.TabIndex = 29
        '
        'Label10
        '
        Me.Label10.AutoSize = True
        Me.Label10.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label10.Location = New System.Drawing.Point(296, 201)
        Me.Label10.Name = "Label10"
        Me.Label10.Size = New System.Drawing.Size(18, 15)
        Me.Label10.TabIndex = 28
        Me.Label10.Text = "m"
        Me.ToolTip1.SetToolTip(Me.Label10, "Linear Metres")
        '
        'txtBusHeight
        '
        Me.txtBusHeight.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!)
        Me.txtBusHeight.Location = New System.Drawing.Point(179, 198)
        Me.txtBusHeight.Name = "txtBusHeight"
        Me.txtBusHeight.Size = New System.Drawing.Size(97, 21)
        Me.txtBusHeight.TabIndex = 27
        '
        'Label11
        '
        Me.Label11.AutoSize = True
        Me.Label11.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!)
        Me.Label11.ForeColor = System.Drawing.Color.Black
        Me.Label11.Location = New System.Drawing.Point(14, 201)
        Me.Label11.Name = "Label11"
        Me.Label11.Size = New System.Drawing.Size(70, 15)
        Me.Label11.TabIndex = 26
        Me.Label11.Text = "Bus  Height"
        '
        'txtBusModel
        '
        Me.txtBusModel.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!)
        Me.txtBusModel.Location = New System.Drawing.Point(179, 30)
        Me.txtBusModel.Name = "txtBusModel"
        Me.txtBusModel.Size = New System.Drawing.Size(208, 21)
        Me.txtBusModel.TabIndex = 25
        '
        'lblBusModel
        '
        Me.lblBusModel.AutoSize = True
        Me.lblBusModel.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!)
        Me.lblBusModel.ForeColor = System.Drawing.Color.Black
        Me.lblBusModel.Location = New System.Drawing.Point(14, 33)
        Me.lblBusModel.Name = "lblBusModel"
        Me.lblBusModel.Size = New System.Drawing.Size(66, 15)
        Me.lblBusModel.TabIndex = 24
        Me.lblBusModel.Text = "Bus Model"
        '
        'lblBusFloorType
        '
        Me.lblBusFloorType.AutoSize = True
        Me.lblBusFloorType.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!)
        Me.lblBusFloorType.ForeColor = System.Drawing.Color.Black
        Me.lblBusFloorType.Location = New System.Drawing.Point(14, 88)
        Me.lblBusFloorType.Name = "lblBusFloorType"
        Me.lblBusFloorType.Size = New System.Drawing.Size(88, 15)
        Me.lblBusFloorType.TabIndex = 22
        Me.lblBusFloorType.Text = "Bus Floor Type"
        '
        'lblUnitsBW
        '
        Me.lblUnitsBW.AutoSize = True
        Me.lblUnitsBW.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblUnitsBW.Location = New System.Drawing.Point(296, 171)
        Me.lblUnitsBW.Name = "lblUnitsBW"
        Me.lblUnitsBW.Size = New System.Drawing.Size(18, 15)
        Me.lblUnitsBW.TabIndex = 21
        Me.lblUnitsBW.Text = "m"
        Me.ToolTip1.SetToolTip(Me.lblUnitsBW, "Linear Metres")
        '
        'lblUnitsBSA
        '
        Me.lblUnitsBSA.AutoSize = True
        Me.lblUnitsBSA.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblUnitsBSA.Location = New System.Drawing.Point(296, 287)
        Me.lblUnitsBSA.Name = "lblUnitsBSA"
        Me.lblUnitsBSA.Size = New System.Drawing.Size(31, 15)
        Me.lblUnitsBSA.TabIndex = 20
        Me.lblUnitsBSA.Text = "m^2"
        Me.ToolTip1.SetToolTip(Me.lblUnitsBSA, "Metres Squared")
        '
        'lblUnitsBWSA
        '
        Me.lblUnitsBWSA.AutoSize = True
        Me.lblUnitsBWSA.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblUnitsBWSA.Location = New System.Drawing.Point(296, 257)
        Me.lblUnitsBWSA.Name = "lblUnitsBWSA"
        Me.lblUnitsBWSA.Size = New System.Drawing.Size(31, 15)
        Me.lblUnitsBWSA.TabIndex = 19
        Me.lblUnitsBWSA.Text = "m^2"
        Me.ToolTip1.SetToolTip(Me.lblUnitsBWSA, "Metres Squared")
        '
        'lblUnitsBV
        '
        Me.lblUnitsBV.AutoSize = True
        Me.lblUnitsBV.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblUnitsBV.Location = New System.Drawing.Point(296, 313)
        Me.lblUnitsBV.Name = "lblUnitsBV"
        Me.lblUnitsBV.Size = New System.Drawing.Size(31, 15)
        Me.lblUnitsBV.TabIndex = 18
        Me.lblUnitsBV.Text = "m^3"
        Me.ToolTip1.SetToolTip(Me.lblUnitsBV, "Metres Cubed")
        '
        'lblUnitsBL
        '
        Me.lblUnitsBL.AutoSize = True
        Me.lblUnitsBL.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblUnitsBL.Location = New System.Drawing.Point(296, 140)
        Me.lblUnitsBL.Name = "lblUnitsBL"
        Me.lblUnitsBL.Size = New System.Drawing.Size(18, 15)
        Me.lblUnitsBL.TabIndex = 17
        Me.lblUnitsBL.Text = "m"
        Me.ToolTip1.SetToolTip(Me.lblUnitsBL, "Linear Metres")
        '
        'lblUnitsBFSA
        '
        Me.lblUnitsBFSA.AutoSize = True
        Me.lblUnitsBFSA.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblUnitsBFSA.Location = New System.Drawing.Point(296, 230)
        Me.lblUnitsBFSA.Name = "lblUnitsBFSA"
        Me.lblUnitsBFSA.Size = New System.Drawing.Size(31, 15)
        Me.lblUnitsBFSA.TabIndex = 16
        Me.lblUnitsBFSA.Text = "m^2"
        Me.ToolTip1.SetToolTip(Me.lblUnitsBFSA, "Metres Squared")
        '
        'txtBusWidth
        '
        Me.txtBusWidth.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!)
        Me.txtBusWidth.Location = New System.Drawing.Point(179, 168)
        Me.txtBusWidth.Name = "txtBusWidth"
        Me.txtBusWidth.Size = New System.Drawing.Size(97, 21)
        Me.txtBusWidth.TabIndex = 15
        '
        'txtBusLength
        '
        Me.txtBusLength.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!)
        Me.txtBusLength.Location = New System.Drawing.Point(179, 139)
        Me.txtBusLength.Name = "txtBusLength"
        Me.txtBusLength.Size = New System.Drawing.Size(97, 21)
        Me.txtBusLength.TabIndex = 14
        '
        'txtBusVolume
        '
        Me.txtBusVolume.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!)
        Me.txtBusVolume.Location = New System.Drawing.Point(179, 313)
        Me.txtBusVolume.Name = "txtBusVolume"
        Me.txtBusVolume.ReadOnly = True
        Me.txtBusVolume.Size = New System.Drawing.Size(97, 21)
        Me.txtBusVolume.TabIndex = 13
        '
        'lblBusVolume
        '
        Me.lblBusVolume.AutoSize = True
        Me.lblBusVolume.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!)
        Me.lblBusVolume.ForeColor = System.Drawing.Color.Black
        Me.lblBusVolume.Location = New System.Drawing.Point(14, 316)
        Me.lblBusVolume.Name = "lblBusVolume"
        Me.lblBusVolume.Size = New System.Drawing.Size(73, 15)
        Me.lblBusVolume.TabIndex = 12
        Me.lblBusVolume.Text = "Bus Volume"
        '
        'lblBusLength
        '
        Me.lblBusLength.AutoSize = True
        Me.lblBusLength.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!)
        Me.lblBusLength.ForeColor = System.Drawing.Color.Black
        Me.lblBusLength.Location = New System.Drawing.Point(14, 142)
        Me.lblBusLength.Name = "lblBusLength"
        Me.lblBusLength.Size = New System.Drawing.Size(72, 15)
        Me.lblBusLength.TabIndex = 11
        Me.lblBusLength.Text = "Bus  Length"
        '
        'lblBusWidth
        '
        Me.lblBusWidth.AutoSize = True
        Me.lblBusWidth.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!)
        Me.lblBusWidth.ForeColor = System.Drawing.Color.Black
        Me.lblBusWidth.Location = New System.Drawing.Point(14, 171)
        Me.lblBusWidth.Name = "lblBusWidth"
        Me.lblBusWidth.Size = New System.Drawing.Size(65, 15)
        Me.lblBusWidth.TabIndex = 10
        Me.lblBusWidth.Text = "Bus  Width"
        '
        'txtBusWindowSurfaceArea
        '
        Me.txtBusWindowSurfaceArea.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!)
        Me.txtBusWindowSurfaceArea.Location = New System.Drawing.Point(179, 255)
        Me.txtBusWindowSurfaceArea.Name = "txtBusWindowSurfaceArea"
        Me.txtBusWindowSurfaceArea.ReadOnly = True
        Me.txtBusWindowSurfaceArea.Size = New System.Drawing.Size(97, 21)
        Me.txtBusWindowSurfaceArea.TabIndex = 9
        '
        'lblBusWindowSurfaceArea
        '
        Me.lblBusWindowSurfaceArea.AutoSize = True
        Me.lblBusWindowSurfaceArea.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!)
        Me.lblBusWindowSurfaceArea.ForeColor = System.Drawing.Color.Black
        Me.lblBusWindowSurfaceArea.Location = New System.Drawing.Point(14, 258)
        Me.lblBusWindowSurfaceArea.Name = "lblBusWindowSurfaceArea"
        Me.lblBusWindowSurfaceArea.Size = New System.Drawing.Size(151, 15)
        Me.lblBusWindowSurfaceArea.TabIndex = 8
        Me.lblBusWindowSurfaceArea.Text = "Bus  Window Surface Area"
        '
        'txtBusSurfaceArea
        '
        Me.txtBusSurfaceArea.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!)
        Me.txtBusSurfaceArea.Location = New System.Drawing.Point(179, 286)
        Me.txtBusSurfaceArea.Name = "txtBusSurfaceArea"
        Me.txtBusSurfaceArea.ReadOnly = True
        Me.txtBusSurfaceArea.Size = New System.Drawing.Size(97, 21)
        Me.txtBusSurfaceArea.TabIndex = 7
        '
        'lblBusSurfaceArea
        '
        Me.lblBusSurfaceArea.AutoSize = True
        Me.lblBusSurfaceArea.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!)
        Me.lblBusSurfaceArea.ForeColor = System.Drawing.Color.Black
        Me.lblBusSurfaceArea.Location = New System.Drawing.Point(14, 289)
        Me.lblBusSurfaceArea.Name = "lblBusSurfaceArea"
        Me.lblBusSurfaceArea.Size = New System.Drawing.Size(104, 15)
        Me.lblBusSurfaceArea.TabIndex = 6
        Me.lblBusSurfaceArea.Text = "Bus  Surface Area"
        '
        'txtBusFloorSurfaceArea
        '
        Me.txtBusFloorSurfaceArea.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!)
        Me.txtBusFloorSurfaceArea.Location = New System.Drawing.Point(179, 227)
        Me.txtBusFloorSurfaceArea.Name = "txtBusFloorSurfaceArea"
        Me.txtBusFloorSurfaceArea.ReadOnly = True
        Me.txtBusFloorSurfaceArea.Size = New System.Drawing.Size(97, 21)
        Me.txtBusFloorSurfaceArea.TabIndex = 5
        '
        'lblBusFloorSurfaceArea
        '
        Me.lblBusFloorSurfaceArea.AutoSize = True
        Me.lblBusFloorSurfaceArea.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!)
        Me.lblBusFloorSurfaceArea.ForeColor = System.Drawing.Color.Black
        Me.lblBusFloorSurfaceArea.Location = New System.Drawing.Point(14, 230)
        Me.lblBusFloorSurfaceArea.Name = "lblBusFloorSurfaceArea"
        Me.lblBusFloorSurfaceArea.Size = New System.Drawing.Size(132, 15)
        Me.lblBusFloorSurfaceArea.TabIndex = 4
        Me.lblBusFloorSurfaceArea.Text = "Bus Floor Surface Area"
        '
        'txtRegisteredPassengers
        '
        Me.txtRegisteredPassengers.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!)
        Me.txtRegisteredPassengers.Location = New System.Drawing.Point(179, 59)
        Me.txtRegisteredPassengers.Name = "txtRegisteredPassengers"
        Me.txtRegisteredPassengers.Size = New System.Drawing.Size(97, 21)
        Me.txtRegisteredPassengers.TabIndex = 1
        '
        'lblRegisteredPassengers
        '
        Me.lblRegisteredPassengers.AutoSize = True
        Me.lblRegisteredPassengers.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!)
        Me.lblRegisteredPassengers.ForeColor = System.Drawing.Color.Black
        Me.lblRegisteredPassengers.Location = New System.Drawing.Point(14, 62)
        Me.lblRegisteredPassengers.Name = "lblRegisteredPassengers"
        Me.lblRegisteredPassengers.Size = New System.Drawing.Size(138, 15)
        Me.lblRegisteredPassengers.TabIndex = 0
        Me.lblRegisteredPassengers.Text = "Registered Passengers "
        '
        'cboBuses
        '
        Me.cboBuses.FormattingEnabled = True
        Me.cboBuses.Location = New System.Drawing.Point(30, 22)
        Me.cboBuses.Name = "cboBuses"
        Me.cboBuses.Size = New System.Drawing.Size(251, 23)
        Me.cboBuses.TabIndex = 7
        '
        'tabGeneralInputsBC
        '
        Me.tabGeneralInputsBC.Controls.Add(Me.GroupBox2)
        Me.tabGeneralInputsBC.Location = New System.Drawing.Point(4, 22)
        Me.tabGeneralInputsBC.Name = "tabGeneralInputsBC"
        Me.tabGeneralInputsBC.Size = New System.Drawing.Size(937, 611)
        Me.tabGeneralInputsBC.TabIndex = 2
        Me.tabGeneralInputsBC.Text = " INP - Boundary Conditions "
        Me.tabGeneralInputsBC.UseVisualStyleBackColor = True
        '
        'GroupBox2
        '
        Me.GroupBox2.BackColor = System.Drawing.Color.Transparent
        Me.GroupBox2.Controls.Add(Me.lblUnitsBC_MaxPossibleBenefitFromTechnologyList)
        Me.GroupBox2.Controls.Add(Me.txtBC_MaxPossibleBenefitFromTechnologyList)
        Me.GroupBox2.Controls.Add(Me.lblBC_MaxPossibleBenefitFromTechnologyList)
        Me.GroupBox2.Controls.Add(Me.lblUnitsBC_MaxTemperatureDeltaForLowFloorBusses)
        Me.GroupBox2.Controls.Add(Me.txtBC_MaxTemperatureDeltaForLowFloorBusses)
        Me.GroupBox2.Controls.Add(Me.lblBC_MaxTemperatureDeltaForLowFloorBusses)
        Me.GroupBox2.Controls.Add(Me.lblUnitsBC_FrontRearWindowArea)
        Me.GroupBox2.Controls.Add(Me.txtBC_FrontRearWindowArea)
        Me.GroupBox2.Controls.Add(Me.lblBC_FrontRearWindowArea)
        Me.GroupBox2.Controls.Add(Me.lblUnitsBC_WindowAreaPerUnitBusLength)
        Me.GroupBox2.Controls.Add(Me.txtBC_WindowAreaPerUnitBusLength)
        Me.GroupBox2.Controls.Add(Me.lblBC_WindowAreaPerUnitBusLength)
        Me.GroupBox2.Controls.Add(Me.lblUnitsBC_VolumicMassDieselOrHeatingOil)
        Me.GroupBox2.Controls.Add(Me.txtBC_VolumicMassDieselOrHeatingOil)
        Me.GroupBox2.Controls.Add(Me.lblBC_VolumicMassDieselOrHeatingOil)
        Me.GroupBox2.Controls.Add(Me.lblUnitsBC_GCVDieselOrHeatingOil)
        Me.GroupBox2.Controls.Add(Me.txtBC_GCVDieselOrHeatingOil)
        Me.GroupBox2.Controls.Add(Me.lblBC_GCVDieselOrHeatingOil)
        Me.GroupBox2.Controls.Add(Me.txtBC_AuxHeaterEfficiency)
        Me.GroupBox2.Controls.Add(Me.lblBC_AuxHeaterEfficiency)
        Me.GroupBox2.Controls.Add(Me.lblUnitsBC_COP)
        Me.GroupBox2.Controls.Add(Me.txtBC_COP)
        Me.GroupBox2.Controls.Add(Me.lblBC_COP)
        Me.GroupBox2.Controls.Add(Me.lblUnitsBC_SpecificVentilationPower)
        Me.GroupBox2.Controls.Add(Me.txtBC_SpecificVentilationPower)
        Me.GroupBox2.Controls.Add(Me.lvlBC_SpecificVentilationPower)
        Me.GroupBox2.Controls.Add(Me.lblUnitsBC_LowVentPowerW)
        Me.GroupBox2.Controls.Add(Me.txtBC_LowVentPowerW)
        Me.GroupBox2.Controls.Add(Me.lblBC_LowVentPowerW)
        Me.GroupBox2.Controls.Add(Me.lblUnitsBC_HighVentPowerW)
        Me.GroupBox2.Controls.Add(Me.txtBC_HighVentPowerW)
        Me.GroupBox2.Controls.Add(Me.lblBC_HighVentPowerW)
        Me.GroupBox2.Controls.Add(Me.lblUnitsBC_Low)
        Me.GroupBox2.Controls.Add(Me.txtBC_Low)
        Me.GroupBox2.Controls.Add(Me.lblBC_Low)
        Me.GroupBox2.Controls.Add(Me.lblUnitsBC_High)
        Me.GroupBox2.Controls.Add(Me.txtBC_High)
        Me.GroupBox2.Controls.Add(Me.lblBC_High)
        Me.GroupBox2.Controls.Add(Me.lblUnitsBC_lowVentilation)
        Me.GroupBox2.Controls.Add(Me.txtBC_lowVentilation)
        Me.GroupBox2.Controls.Add(Me.lblBC_lowVentilation)
        Me.GroupBox2.Controls.Add(Me.lblUnitsBC_HighVentilation)
        Me.GroupBox2.Controls.Add(Me.txtBC_HighVentilation)
        Me.GroupBox2.Controls.Add(Me.lblBC_HighVentilation)
        Me.GroupBox2.Controls.Add(Me.lblUnitsBC_CoolingBoundaryTemperature)
        Me.GroupBox2.Controls.Add(Me.txtBC_CoolingBoundaryTemperature)
        Me.GroupBox2.Controls.Add(Me.lblBC_CoolingBoundaryTemperature)
        Me.GroupBox2.Controls.Add(Me.lblUnitsBC_HeatingBoundaryTemperature)
        Me.GroupBox2.Controls.Add(Me.txtBC_HeatingBoundaryTemperature)
        Me.GroupBox2.Controls.Add(Me.lblBC_HeatingBoundaryTemperature)
        Me.GroupBox2.Controls.Add(Me.txtBC_GFactor)
        Me.GroupBox2.Controls.Add(Me.lblGFactor)
        Me.GroupBox2.Controls.Add(Me.txtBC_HeatPerPassengerIntoCabinW)
        Me.GroupBox2.Controls.Add(Me.Label2)
        Me.GroupBox2.Controls.Add(Me.lblUnitsUValues)
        Me.GroupBox2.Controls.Add(Me.lblUnitsPassengerBoundaryTemp)
        Me.GroupBox2.Controls.Add(Me.lblUnitsPGRDensitySemiLowFloor)
        Me.GroupBox2.Controls.Add(Me.lblUnitsPassenderDensityLowFloor)
        Me.GroupBox2.Controls.Add(Me.lblUnitsBC_PassengerDensityRaisedFloor)
        Me.GroupBox2.Controls.Add(Me.lblHeatPerPassengerIntoCabinW)
        Me.GroupBox2.Controls.Add(Me.txtBC_UValues)
        Me.GroupBox2.Controls.Add(Me.txtBC_CalculatedPassengerNumber)
        Me.GroupBox2.Controls.Add(Me.txtBC_PassengerDensityRaisedFloor)
        Me.GroupBox2.Controls.Add(Me.lblBC_PassengerDensityRaisedFloor)
        Me.GroupBox2.Controls.Add(Me.lblBC_CalculatedPassengerNumber)
        Me.GroupBox2.Controls.Add(Me.lblBC_UValues)
        Me.GroupBox2.Controls.Add(Me.txtBC_PassengerDensitySemiLowFloor)
        Me.GroupBox2.Controls.Add(Me.lblBC_PassengerDensitySemiLowFloor)
        Me.GroupBox2.Controls.Add(Me.txtBC_PassengerDensityLowFloor)
        Me.GroupBox2.Controls.Add(Me.Label13)
        Me.GroupBox2.Controls.Add(Me.txtBC_PassengerBoundaryTemperature)
        Me.GroupBox2.Controls.Add(Me.lblPassengerBoundaryTemp)
        Me.GroupBox2.Controls.Add(Me.txtBC_SolarClouding)
        Me.GroupBox2.Controls.Add(Me.lblSolarClouding)
        Me.GroupBox2.FlatStyle = System.Windows.Forms.FlatStyle.Popup
        Me.GroupBox2.Font = New System.Drawing.Font("Microsoft Sans Serif", 10.0!)
        Me.GroupBox2.ForeColor = System.Drawing.SystemColors.MenuHighlight
        Me.GroupBox2.Location = New System.Drawing.Point(34, 47)
        Me.GroupBox2.Name = "GroupBox2"
        Me.GroupBox2.Size = New System.Drawing.Size(890, 534)
        Me.GroupBox2.TabIndex = 27
        Me.GroupBox2.TabStop = False
        Me.GroupBox2.Text = "Boundary Conditions"
        '
        'lblUnitsBC_MaxPossibleBenefitFromTechnologyList
        '
        Me.lblUnitsBC_MaxPossibleBenefitFromTechnologyList.AutoSize = True
        Me.lblUnitsBC_MaxPossibleBenefitFromTechnologyList.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblUnitsBC_MaxPossibleBenefitFromTechnologyList.Location = New System.Drawing.Point(783, 264)
        Me.lblUnitsBC_MaxPossibleBenefitFromTechnologyList.Name = "lblUnitsBC_MaxPossibleBenefitFromTechnologyList"
        Me.lblUnitsBC_MaxPossibleBenefitFromTechnologyList.Size = New System.Drawing.Size(51, 15)
        Me.lblUnitsBC_MaxPossibleBenefitFromTechnologyList.TabIndex = 75
        Me.lblUnitsBC_MaxPossibleBenefitFromTechnologyList.Text = "Fraction"
        Me.ToolTip1.SetToolTip(Me.lblUnitsBC_MaxPossibleBenefitFromTechnologyList, "Fraction")
        '
        'txtBC_MaxPossibleBenefitFromTechnologyList
        '
        Me.txtBC_MaxPossibleBenefitFromTechnologyList.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!)
        Me.txtBC_MaxPossibleBenefitFromTechnologyList.Location = New System.Drawing.Point(655, 261)
        Me.txtBC_MaxPossibleBenefitFromTechnologyList.Name = "txtBC_MaxPossibleBenefitFromTechnologyList"
        Me.txtBC_MaxPossibleBenefitFromTechnologyList.Size = New System.Drawing.Size(97, 21)
        Me.txtBC_MaxPossibleBenefitFromTechnologyList.TabIndex = 74
        '
        'lblBC_MaxPossibleBenefitFromTechnologyList
        '
        Me.lblBC_MaxPossibleBenefitFromTechnologyList.AutoSize = True
        Me.lblBC_MaxPossibleBenefitFromTechnologyList.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!)
        Me.lblBC_MaxPossibleBenefitFromTechnologyList.ForeColor = System.Drawing.Color.Black
        Me.lblBC_MaxPossibleBenefitFromTechnologyList.Location = New System.Drawing.Point(448, 264)
        Me.lblBC_MaxPossibleBenefitFromTechnologyList.Name = "lblBC_MaxPossibleBenefitFromTechnologyList"
        Me.lblBC_MaxPossibleBenefitFromTechnologyList.Size = New System.Drawing.Size(159, 15)
        Me.lblBC_MaxPossibleBenefitFromTechnologyList.TabIndex = 73
        Me.lblBC_MaxPossibleBenefitFromTechnologyList.Text = "Max  Benefit From Tech List"
        '
        'lblUnitsBC_MaxTemperatureDeltaForLowFloorBusses
        '
        Me.lblUnitsBC_MaxTemperatureDeltaForLowFloorBusses.AutoSize = True
        Me.lblUnitsBC_MaxTemperatureDeltaForLowFloorBusses.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblUnitsBC_MaxTemperatureDeltaForLowFloorBusses.Location = New System.Drawing.Point(783, 235)
        Me.lblUnitsBC_MaxTemperatureDeltaForLowFloorBusses.Name = "lblUnitsBC_MaxTemperatureDeltaForLowFloorBusses"
        Me.lblUnitsBC_MaxTemperatureDeltaForLowFloorBusses.Size = New System.Drawing.Size(15, 15)
        Me.lblUnitsBC_MaxTemperatureDeltaForLowFloorBusses.TabIndex = 72
        Me.lblUnitsBC_MaxTemperatureDeltaForLowFloorBusses.Text = "K"
        Me.ToolTip1.SetToolTip(Me.lblUnitsBC_MaxTemperatureDeltaForLowFloorBusses, "K")
        '
        'txtBC_MaxTemperatureDeltaForLowFloorBusses
        '
        Me.txtBC_MaxTemperatureDeltaForLowFloorBusses.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!)
        Me.txtBC_MaxTemperatureDeltaForLowFloorBusses.Location = New System.Drawing.Point(655, 232)
        Me.txtBC_MaxTemperatureDeltaForLowFloorBusses.Name = "txtBC_MaxTemperatureDeltaForLowFloorBusses"
        Me.txtBC_MaxTemperatureDeltaForLowFloorBusses.Size = New System.Drawing.Size(97, 21)
        Me.txtBC_MaxTemperatureDeltaForLowFloorBusses.TabIndex = 71
        '
        'lblBC_MaxTemperatureDeltaForLowFloorBusses
        '
        Me.lblBC_MaxTemperatureDeltaForLowFloorBusses.AutoSize = True
        Me.lblBC_MaxTemperatureDeltaForLowFloorBusses.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!)
        Me.lblBC_MaxTemperatureDeltaForLowFloorBusses.ForeColor = System.Drawing.Color.Black
        Me.lblBC_MaxTemperatureDeltaForLowFloorBusses.Location = New System.Drawing.Point(448, 235)
        Me.lblBC_MaxTemperatureDeltaForLowFloorBusses.Name = "lblBC_MaxTemperatureDeltaForLowFloorBusses"
        Me.lblBC_MaxTemperatureDeltaForLowFloorBusses.Size = New System.Drawing.Size(205, 15)
        Me.lblBC_MaxTemperatureDeltaForLowFloorBusses.TabIndex = 70
        Me.lblBC_MaxTemperatureDeltaForLowFloorBusses.Text = "Max Temp Delta - Low Floor Busses"
        '
        'lblUnitsBC_FrontRearWindowArea
        '
        Me.lblUnitsBC_FrontRearWindowArea.AutoSize = True
        Me.lblUnitsBC_FrontRearWindowArea.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblUnitsBC_FrontRearWindowArea.Location = New System.Drawing.Point(783, 208)
        Me.lblUnitsBC_FrontRearWindowArea.Name = "lblUnitsBC_FrontRearWindowArea"
        Me.lblUnitsBC_FrontRearWindowArea.Size = New System.Drawing.Size(31, 15)
        Me.lblUnitsBC_FrontRearWindowArea.TabIndex = 69
        Me.lblUnitsBC_FrontRearWindowArea.Text = "m^2"
        Me.ToolTip1.SetToolTip(Me.lblUnitsBC_FrontRearWindowArea, "Metres Squared")
        '
        'txtBC_FrontRearWindowArea
        '
        Me.txtBC_FrontRearWindowArea.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!)
        Me.txtBC_FrontRearWindowArea.Location = New System.Drawing.Point(655, 205)
        Me.txtBC_FrontRearWindowArea.Name = "txtBC_FrontRearWindowArea"
        Me.txtBC_FrontRearWindowArea.ReadOnly = True
        Me.txtBC_FrontRearWindowArea.Size = New System.Drawing.Size(97, 21)
        Me.txtBC_FrontRearWindowArea.TabIndex = 68
        '
        'lblBC_FrontRearWindowArea
        '
        Me.lblBC_FrontRearWindowArea.AutoSize = True
        Me.lblBC_FrontRearWindowArea.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!)
        Me.lblBC_FrontRearWindowArea.ForeColor = System.Drawing.Color.Black
        Me.lblBC_FrontRearWindowArea.Location = New System.Drawing.Point(448, 208)
        Me.lblBC_FrontRearWindowArea.Name = "lblBC_FrontRearWindowArea"
        Me.lblBC_FrontRearWindowArea.Size = New System.Drawing.Size(140, 15)
        Me.lblBC_FrontRearWindowArea.TabIndex = 67
        Me.lblBC_FrontRearWindowArea.Text = "Front Rear Window Area"
        '
        'lblUnitsBC_WindowAreaPerUnitBusLength
        '
        Me.lblUnitsBC_WindowAreaPerUnitBusLength.AutoSize = True
        Me.lblUnitsBC_WindowAreaPerUnitBusLength.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblUnitsBC_WindowAreaPerUnitBusLength.Location = New System.Drawing.Point(783, 179)
        Me.lblUnitsBC_WindowAreaPerUnitBusLength.Name = "lblUnitsBC_WindowAreaPerUnitBusLength"
        Me.lblUnitsBC_WindowAreaPerUnitBusLength.Size = New System.Drawing.Size(38, 15)
        Me.lblUnitsBC_WindowAreaPerUnitBusLength.TabIndex = 66
        Me.lblUnitsBC_WindowAreaPerUnitBusLength.Text = "m^/m"
        Me.ToolTip1.SetToolTip(Me.lblUnitsBC_WindowAreaPerUnitBusLength, "m^2/m")
        '
        'txtBC_WindowAreaPerUnitBusLength
        '
        Me.txtBC_WindowAreaPerUnitBusLength.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!)
        Me.txtBC_WindowAreaPerUnitBusLength.Location = New System.Drawing.Point(655, 176)
        Me.txtBC_WindowAreaPerUnitBusLength.Name = "txtBC_WindowAreaPerUnitBusLength"
        Me.txtBC_WindowAreaPerUnitBusLength.ReadOnly = True
        Me.txtBC_WindowAreaPerUnitBusLength.Size = New System.Drawing.Size(97, 21)
        Me.txtBC_WindowAreaPerUnitBusLength.TabIndex = 65
        '
        'lblBC_WindowAreaPerUnitBusLength
        '
        Me.lblBC_WindowAreaPerUnitBusLength.AutoSize = True
        Me.lblBC_WindowAreaPerUnitBusLength.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!)
        Me.lblBC_WindowAreaPerUnitBusLength.ForeColor = System.Drawing.Color.Black
        Me.lblBC_WindowAreaPerUnitBusLength.Location = New System.Drawing.Point(448, 179)
        Me.lblBC_WindowAreaPerUnitBusLength.Name = "lblBC_WindowAreaPerUnitBusLength"
        Me.lblBC_WindowAreaPerUnitBusLength.Size = New System.Drawing.Size(188, 15)
        Me.lblBC_WindowAreaPerUnitBusLength.TabIndex = 64
        Me.lblBC_WindowAreaPerUnitBusLength.Text = "WindowArea Per Unit Bus Length"
        '
        'lblUnitsBC_VolumicMassDieselOrHeatingOil
        '
        Me.lblUnitsBC_VolumicMassDieselOrHeatingOil.AutoSize = True
        Me.lblUnitsBC_VolumicMassDieselOrHeatingOil.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblUnitsBC_VolumicMassDieselOrHeatingOil.Location = New System.Drawing.Point(783, 151)
        Me.lblUnitsBC_VolumicMassDieselOrHeatingOil.Name = "lblUnitsBC_VolumicMassDieselOrHeatingOil"
        Me.lblUnitsBC_VolumicMassDieselOrHeatingOil.Size = New System.Drawing.Size(28, 15)
        Me.lblUnitsBC_VolumicMassDieselOrHeatingOil.TabIndex = 63
        Me.lblUnitsBC_VolumicMassDieselOrHeatingOil.Text = "Kg/l"
        Me.ToolTip1.SetToolTip(Me.lblUnitsBC_VolumicMassDieselOrHeatingOil, "Kg/Litre")
        '
        'txtBC_VolumicMassDieselOrHeatingOil
        '
        Me.txtBC_VolumicMassDieselOrHeatingOil.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!)
        Me.txtBC_VolumicMassDieselOrHeatingOil.Location = New System.Drawing.Point(655, 148)
        Me.txtBC_VolumicMassDieselOrHeatingOil.Name = "txtBC_VolumicMassDieselOrHeatingOil"
        Me.txtBC_VolumicMassDieselOrHeatingOil.Size = New System.Drawing.Size(97, 21)
        Me.txtBC_VolumicMassDieselOrHeatingOil.TabIndex = 62
        '
        'lblBC_VolumicMassDieselOrHeatingOil
        '
        Me.lblBC_VolumicMassDieselOrHeatingOil.AutoSize = True
        Me.lblBC_VolumicMassDieselOrHeatingOil.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!)
        Me.lblBC_VolumicMassDieselOrHeatingOil.ForeColor = System.Drawing.Color.Black
        Me.lblBC_VolumicMassDieselOrHeatingOil.Location = New System.Drawing.Point(448, 151)
        Me.lblBC_VolumicMassDieselOrHeatingOil.Name = "lblBC_VolumicMassDieselOrHeatingOil"
        Me.lblBC_VolumicMassDieselOrHeatingOil.Size = New System.Drawing.Size(202, 15)
        Me.lblBC_VolumicMassDieselOrHeatingOil.TabIndex = 61
        Me.lblBC_VolumicMassDieselOrHeatingOil.Text = "Volumic Mass Diesel Or Heating Oil"
        '
        'lblUnitsBC_GCVDieselOrHeatingOil
        '
        Me.lblUnitsBC_GCVDieselOrHeatingOil.AutoSize = True
        Me.lblUnitsBC_GCVDieselOrHeatingOil.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblUnitsBC_GCVDieselOrHeatingOil.Location = New System.Drawing.Point(783, 123)
        Me.lblUnitsBC_GCVDieselOrHeatingOil.Name = "lblUnitsBC_GCVDieselOrHeatingOil"
        Me.lblUnitsBC_GCVDieselOrHeatingOil.Size = New System.Drawing.Size(50, 15)
        Me.lblUnitsBC_GCVDieselOrHeatingOil.TabIndex = 60
        Me.lblUnitsBC_GCVDieselOrHeatingOil.Text = "Kw/h/kg"
        Me.ToolTip1.SetToolTip(Me.lblUnitsBC_GCVDieselOrHeatingOil, "Kw/h/kg")
        '
        'txtBC_GCVDieselOrHeatingOil
        '
        Me.txtBC_GCVDieselOrHeatingOil.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!)
        Me.txtBC_GCVDieselOrHeatingOil.Location = New System.Drawing.Point(655, 120)
        Me.txtBC_GCVDieselOrHeatingOil.Name = "txtBC_GCVDieselOrHeatingOil"
        Me.txtBC_GCVDieselOrHeatingOil.Size = New System.Drawing.Size(97, 21)
        Me.txtBC_GCVDieselOrHeatingOil.TabIndex = 59
        '
        'lblBC_GCVDieselOrHeatingOil
        '
        Me.lblBC_GCVDieselOrHeatingOil.AutoSize = True
        Me.lblBC_GCVDieselOrHeatingOil.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!)
        Me.lblBC_GCVDieselOrHeatingOil.ForeColor = System.Drawing.Color.Black
        Me.lblBC_GCVDieselOrHeatingOil.Location = New System.Drawing.Point(448, 123)
        Me.lblBC_GCVDieselOrHeatingOil.Name = "lblBC_GCVDieselOrHeatingOil"
        Me.lblBC_GCVDieselOrHeatingOil.Size = New System.Drawing.Size(149, 15)
        Me.lblBC_GCVDieselOrHeatingOil.TabIndex = 58
        Me.lblBC_GCVDieselOrHeatingOil.Text = "GCV Diesel Or Heating Oil"
        '
        'txtBC_AuxHeaterEfficiency
        '
        Me.txtBC_AuxHeaterEfficiency.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!)
        Me.txtBC_AuxHeaterEfficiency.Location = New System.Drawing.Point(655, 90)
        Me.txtBC_AuxHeaterEfficiency.Name = "txtBC_AuxHeaterEfficiency"
        Me.txtBC_AuxHeaterEfficiency.Size = New System.Drawing.Size(97, 21)
        Me.txtBC_AuxHeaterEfficiency.TabIndex = 57
        '
        'lblBC_AuxHeaterEfficiency
        '
        Me.lblBC_AuxHeaterEfficiency.AutoSize = True
        Me.lblBC_AuxHeaterEfficiency.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!)
        Me.lblBC_AuxHeaterEfficiency.ForeColor = System.Drawing.Color.Black
        Me.lblBC_AuxHeaterEfficiency.Location = New System.Drawing.Point(448, 93)
        Me.lblBC_AuxHeaterEfficiency.Name = "lblBC_AuxHeaterEfficiency"
        Me.lblBC_AuxHeaterEfficiency.Size = New System.Drawing.Size(121, 15)
        Me.lblBC_AuxHeaterEfficiency.TabIndex = 56
        Me.lblBC_AuxHeaterEfficiency.Text = "Aux Heater Efficiency"
        '
        'lblUnitsBC_COP
        '
        Me.lblUnitsBC_COP.AutoSize = True
        Me.lblUnitsBC_COP.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblUnitsBC_COP.Location = New System.Drawing.Point(783, 65)
        Me.lblUnitsBC_COP.Name = "lblUnitsBC_COP"
        Me.lblUnitsBC_COP.Size = New System.Drawing.Size(0, 15)
        Me.lblUnitsBC_COP.TabIndex = 55
        '
        'txtBC_COP
        '
        Me.txtBC_COP.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!)
        Me.txtBC_COP.Location = New System.Drawing.Point(655, 62)
        Me.txtBC_COP.Name = "txtBC_COP"
        Me.txtBC_COP.Size = New System.Drawing.Size(97, 21)
        Me.txtBC_COP.TabIndex = 54
        '
        'lblBC_COP
        '
        Me.lblBC_COP.AutoSize = True
        Me.lblBC_COP.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!)
        Me.lblBC_COP.ForeColor = System.Drawing.Color.Black
        Me.lblBC_COP.Location = New System.Drawing.Point(448, 65)
        Me.lblBC_COP.Name = "lblBC_COP"
        Me.lblBC_COP.Size = New System.Drawing.Size(32, 15)
        Me.lblBC_COP.TabIndex = 53
        Me.lblBC_COP.Text = "COP"
        '
        'lblUnitsBC_SpecificVentilationPower
        '
        Me.lblUnitsBC_SpecificVentilationPower.AutoSize = True
        Me.lblUnitsBC_SpecificVentilationPower.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblUnitsBC_SpecificVentilationPower.Location = New System.Drawing.Point(783, 36)
        Me.lblUnitsBC_SpecificVentilationPower.Name = "lblUnitsBC_SpecificVentilationPower"
        Me.lblUnitsBC_SpecificVentilationPower.Size = New System.Drawing.Size(52, 15)
        Me.lblUnitsBC_SpecificVentilationPower.TabIndex = 52
        Me.lblUnitsBC_SpecificVentilationPower.Text = "Wh/m^3"
        Me.ToolTip1.SetToolTip(Me.lblUnitsBC_SpecificVentilationPower, "Watts per Hour / Metres Cubed")
        '
        'txtBC_SpecificVentilationPower
        '
        Me.txtBC_SpecificVentilationPower.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!)
        Me.txtBC_SpecificVentilationPower.Location = New System.Drawing.Point(655, 33)
        Me.txtBC_SpecificVentilationPower.Name = "txtBC_SpecificVentilationPower"
        Me.txtBC_SpecificVentilationPower.Size = New System.Drawing.Size(97, 21)
        Me.txtBC_SpecificVentilationPower.TabIndex = 51
        '
        'lvlBC_SpecificVentilationPower
        '
        Me.lvlBC_SpecificVentilationPower.AutoSize = True
        Me.lvlBC_SpecificVentilationPower.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!)
        Me.lvlBC_SpecificVentilationPower.ForeColor = System.Drawing.Color.Black
        Me.lvlBC_SpecificVentilationPower.Location = New System.Drawing.Point(448, 36)
        Me.lvlBC_SpecificVentilationPower.Name = "lvlBC_SpecificVentilationPower"
        Me.lvlBC_SpecificVentilationPower.Size = New System.Drawing.Size(148, 15)
        Me.lvlBC_SpecificVentilationPower.TabIndex = 50
        Me.lvlBC_SpecificVentilationPower.Text = "Specific Ventilation Power"
        '
        'lblUnitsBC_LowVentPowerW
        '
        Me.lblUnitsBC_LowVentPowerW.AutoSize = True
        Me.lblUnitsBC_LowVentPowerW.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblUnitsBC_LowVentPowerW.Location = New System.Drawing.Point(349, 487)
        Me.lblUnitsBC_LowVentPowerW.Name = "lblUnitsBC_LowVentPowerW"
        Me.lblUnitsBC_LowVentPowerW.Size = New System.Drawing.Size(18, 15)
        Me.lblUnitsBC_LowVentPowerW.TabIndex = 49
        Me.lblUnitsBC_LowVentPowerW.Text = "W"
        Me.ToolTip1.SetToolTip(Me.lblUnitsBC_LowVentPowerW, "Watts")
        '
        'txtBC_LowVentPowerW
        '
        Me.txtBC_LowVentPowerW.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!)
        Me.txtBC_LowVentPowerW.Location = New System.Drawing.Point(218, 484)
        Me.txtBC_LowVentPowerW.Name = "txtBC_LowVentPowerW"
        Me.txtBC_LowVentPowerW.ReadOnly = True
        Me.txtBC_LowVentPowerW.Size = New System.Drawing.Size(97, 21)
        Me.txtBC_LowVentPowerW.TabIndex = 48
        '
        'lblBC_LowVentPowerW
        '
        Me.lblBC_LowVentPowerW.AutoSize = True
        Me.lblBC_LowVentPowerW.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!)
        Me.lblBC_LowVentPowerW.ForeColor = System.Drawing.Color.Black
        Me.lblBC_LowVentPowerW.Location = New System.Drawing.Point(14, 487)
        Me.lblBC_LowVentPowerW.Name = "lblBC_LowVentPowerW"
        Me.lblBC_LowVentPowerW.Size = New System.Drawing.Size(95, 15)
        Me.lblBC_LowVentPowerW.TabIndex = 47
        Me.lblBC_LowVentPowerW.Text = "Low Vent Power"
        '
        'lblUnitsBC_HighVentPowerW
        '
        Me.lblUnitsBC_HighVentPowerW.AutoSize = True
        Me.lblUnitsBC_HighVentPowerW.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblUnitsBC_HighVentPowerW.Location = New System.Drawing.Point(349, 459)
        Me.lblUnitsBC_HighVentPowerW.Name = "lblUnitsBC_HighVentPowerW"
        Me.lblUnitsBC_HighVentPowerW.Size = New System.Drawing.Size(18, 15)
        Me.lblUnitsBC_HighVentPowerW.TabIndex = 46
        Me.lblUnitsBC_HighVentPowerW.Text = "W"
        Me.ToolTip1.SetToolTip(Me.lblUnitsBC_HighVentPowerW, "Watts")
        '
        'txtBC_HighVentPowerW
        '
        Me.txtBC_HighVentPowerW.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!)
        Me.txtBC_HighVentPowerW.Location = New System.Drawing.Point(218, 456)
        Me.txtBC_HighVentPowerW.Name = "txtBC_HighVentPowerW"
        Me.txtBC_HighVentPowerW.ReadOnly = True
        Me.txtBC_HighVentPowerW.Size = New System.Drawing.Size(97, 21)
        Me.txtBC_HighVentPowerW.TabIndex = 45
        '
        'lblBC_HighVentPowerW
        '
        Me.lblBC_HighVentPowerW.AutoSize = True
        Me.lblBC_HighVentPowerW.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!)
        Me.lblBC_HighVentPowerW.ForeColor = System.Drawing.Color.Black
        Me.lblBC_HighVentPowerW.Location = New System.Drawing.Point(14, 459)
        Me.lblBC_HighVentPowerW.Name = "lblBC_HighVentPowerW"
        Me.lblBC_HighVentPowerW.Size = New System.Drawing.Size(98, 15)
        Me.lblBC_HighVentPowerW.TabIndex = 44
        Me.lblBC_HighVentPowerW.Text = "High Vent Power"
        '
        'lblUnitsBC_Low
        '
        Me.lblUnitsBC_Low.AutoSize = True
        Me.lblUnitsBC_Low.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblUnitsBC_Low.Location = New System.Drawing.Point(349, 430)
        Me.lblUnitsBC_Low.Name = "lblUnitsBC_Low"
        Me.lblUnitsBC_Low.Size = New System.Drawing.Size(43, 15)
        Me.lblUnitsBC_Low.TabIndex = 43
        Me.lblUnitsBC_Low.Text = "m^3/H"
        Me.ToolTip1.SetToolTip(Me.lblUnitsBC_Low, "Metres ^3 / Hour")
        '
        'txtBC_Low
        '
        Me.txtBC_Low.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!)
        Me.txtBC_Low.Location = New System.Drawing.Point(218, 427)
        Me.txtBC_Low.Name = "txtBC_Low"
        Me.txtBC_Low.ReadOnly = True
        Me.txtBC_Low.Size = New System.Drawing.Size(97, 21)
        Me.txtBC_Low.TabIndex = 42
        '
        'lblBC_Low
        '
        Me.lblBC_Low.AutoSize = True
        Me.lblBC_Low.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!)
        Me.lblBC_Low.ForeColor = System.Drawing.Color.Black
        Me.lblBC_Low.Location = New System.Drawing.Point(14, 430)
        Me.lblBC_Low.Name = "lblBC_Low"
        Me.lblBC_Low.Size = New System.Drawing.Size(30, 15)
        Me.lblBC_Low.TabIndex = 41
        Me.lblBC_Low.Text = "Low"
        '
        'lblUnitsBC_High
        '
        Me.lblUnitsBC_High.AutoSize = True
        Me.lblUnitsBC_High.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblUnitsBC_High.Location = New System.Drawing.Point(349, 401)
        Me.lblUnitsBC_High.Name = "lblUnitsBC_High"
        Me.lblUnitsBC_High.Size = New System.Drawing.Size(43, 15)
        Me.lblUnitsBC_High.TabIndex = 40
        Me.lblUnitsBC_High.Text = "m^3/H"
        Me.ToolTip1.SetToolTip(Me.lblUnitsBC_High, "Metres ^3 / Hour")
        '
        'txtBC_High
        '
        Me.txtBC_High.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!)
        Me.txtBC_High.Location = New System.Drawing.Point(218, 398)
        Me.txtBC_High.Name = "txtBC_High"
        Me.txtBC_High.ReadOnly = True
        Me.txtBC_High.Size = New System.Drawing.Size(97, 21)
        Me.txtBC_High.TabIndex = 39
        '
        'lblBC_High
        '
        Me.lblBC_High.AutoSize = True
        Me.lblBC_High.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!)
        Me.lblBC_High.ForeColor = System.Drawing.Color.Black
        Me.lblBC_High.Location = New System.Drawing.Point(14, 401)
        Me.lblBC_High.Name = "lblBC_High"
        Me.lblBC_High.Size = New System.Drawing.Size(33, 15)
        Me.lblBC_High.TabIndex = 38
        Me.lblBC_High.Text = "High"
        '
        'lblUnitsBC_lowVentilation
        '
        Me.lblUnitsBC_lowVentilation.AutoSize = True
        Me.lblUnitsBC_lowVentilation.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblUnitsBC_lowVentilation.Location = New System.Drawing.Point(349, 373)
        Me.lblUnitsBC_lowVentilation.Name = "lblUnitsBC_lowVentilation"
        Me.lblUnitsBC_lowVentilation.Size = New System.Drawing.Size(22, 15)
        Me.lblUnitsBC_lowVentilation.TabIndex = 37
        Me.lblUnitsBC_lowVentilation.Text = "l/H"
        Me.ToolTip1.SetToolTip(Me.lblUnitsBC_lowVentilation, "Litres / Hour")
        '
        'txtBC_lowVentilation
        '
        Me.txtBC_lowVentilation.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!)
        Me.txtBC_lowVentilation.Location = New System.Drawing.Point(218, 370)
        Me.txtBC_lowVentilation.Name = "txtBC_lowVentilation"
        Me.txtBC_lowVentilation.Size = New System.Drawing.Size(97, 21)
        Me.txtBC_lowVentilation.TabIndex = 36
        '
        'lblBC_lowVentilation
        '
        Me.lblBC_lowVentilation.AutoSize = True
        Me.lblBC_lowVentilation.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!)
        Me.lblBC_lowVentilation.ForeColor = System.Drawing.Color.Black
        Me.lblBC_lowVentilation.Location = New System.Drawing.Point(14, 373)
        Me.lblBC_lowVentilation.Name = "lblBC_lowVentilation"
        Me.lblBC_lowVentilation.Size = New System.Drawing.Size(90, 15)
        Me.lblBC_lowVentilation.TabIndex = 35
        Me.lblBC_lowVentilation.Text = "Low Ventilation"
        '
        'lblUnitsBC_HighVentilation
        '
        Me.lblUnitsBC_HighVentilation.AutoSize = True
        Me.lblUnitsBC_HighVentilation.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblUnitsBC_HighVentilation.Location = New System.Drawing.Point(349, 346)
        Me.lblUnitsBC_HighVentilation.Name = "lblUnitsBC_HighVentilation"
        Me.lblUnitsBC_HighVentilation.Size = New System.Drawing.Size(22, 15)
        Me.lblUnitsBC_HighVentilation.TabIndex = 34
        Me.lblUnitsBC_HighVentilation.Text = "l/H"
        Me.ToolTip1.SetToolTip(Me.lblUnitsBC_HighVentilation, "Litres / Hour")
        '
        'txtBC_HighVentilation
        '
        Me.txtBC_HighVentilation.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!)
        Me.txtBC_HighVentilation.Location = New System.Drawing.Point(218, 343)
        Me.txtBC_HighVentilation.Name = "txtBC_HighVentilation"
        Me.txtBC_HighVentilation.Size = New System.Drawing.Size(97, 21)
        Me.txtBC_HighVentilation.TabIndex = 33
        '
        'lblBC_HighVentilation
        '
        Me.lblBC_HighVentilation.AutoSize = True
        Me.lblBC_HighVentilation.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!)
        Me.lblBC_HighVentilation.ForeColor = System.Drawing.Color.Black
        Me.lblBC_HighVentilation.Location = New System.Drawing.Point(14, 346)
        Me.lblBC_HighVentilation.Name = "lblBC_HighVentilation"
        Me.lblBC_HighVentilation.Size = New System.Drawing.Size(93, 15)
        Me.lblBC_HighVentilation.TabIndex = 32
        Me.lblBC_HighVentilation.Text = "High Ventilation"
        '
        'lblUnitsBC_CoolingBoundaryTemperature
        '
        Me.lblUnitsBC_CoolingBoundaryTemperature.AutoSize = True
        Me.lblUnitsBC_CoolingBoundaryTemperature.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblUnitsBC_CoolingBoundaryTemperature.Location = New System.Drawing.Point(349, 317)
        Me.lblUnitsBC_CoolingBoundaryTemperature.Name = "lblUnitsBC_CoolingBoundaryTemperature"
        Me.lblUnitsBC_CoolingBoundaryTemperature.Size = New System.Drawing.Size(22, 15)
        Me.lblUnitsBC_CoolingBoundaryTemperature.TabIndex = 31
        Me.lblUnitsBC_CoolingBoundaryTemperature.Text = "oC"
        Me.ToolTip1.SetToolTip(Me.lblUnitsBC_CoolingBoundaryTemperature, "Degrees Centigrade")
        '
        'txtBC_CoolingBoundaryTemperature
        '
        Me.txtBC_CoolingBoundaryTemperature.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!)
        Me.txtBC_CoolingBoundaryTemperature.Location = New System.Drawing.Point(218, 314)
        Me.txtBC_CoolingBoundaryTemperature.Name = "txtBC_CoolingBoundaryTemperature"
        Me.txtBC_CoolingBoundaryTemperature.Size = New System.Drawing.Size(97, 21)
        Me.txtBC_CoolingBoundaryTemperature.TabIndex = 30
        '
        'lblBC_CoolingBoundaryTemperature
        '
        Me.lblBC_CoolingBoundaryTemperature.AutoSize = True
        Me.lblBC_CoolingBoundaryTemperature.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!)
        Me.lblBC_CoolingBoundaryTemperature.ForeColor = System.Drawing.Color.Black
        Me.lblBC_CoolingBoundaryTemperature.Location = New System.Drawing.Point(14, 317)
        Me.lblBC_CoolingBoundaryTemperature.Name = "lblBC_CoolingBoundaryTemperature"
        Me.lblBC_CoolingBoundaryTemperature.Size = New System.Drawing.Size(178, 15)
        Me.lblBC_CoolingBoundaryTemperature.TabIndex = 29
        Me.lblBC_CoolingBoundaryTemperature.Text = "Cooling Boundary Temperature"
        '
        'lblUnitsBC_HeatingBoundaryTemperature
        '
        Me.lblUnitsBC_HeatingBoundaryTemperature.AutoSize = True
        Me.lblUnitsBC_HeatingBoundaryTemperature.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblUnitsBC_HeatingBoundaryTemperature.Location = New System.Drawing.Point(349, 289)
        Me.lblUnitsBC_HeatingBoundaryTemperature.Name = "lblUnitsBC_HeatingBoundaryTemperature"
        Me.lblUnitsBC_HeatingBoundaryTemperature.Size = New System.Drawing.Size(22, 15)
        Me.lblUnitsBC_HeatingBoundaryTemperature.TabIndex = 28
        Me.lblUnitsBC_HeatingBoundaryTemperature.Text = "oC"
        Me.ToolTip1.SetToolTip(Me.lblUnitsBC_HeatingBoundaryTemperature, "Degrees Centigrade")
        '
        'txtBC_HeatingBoundaryTemperature
        '
        Me.txtBC_HeatingBoundaryTemperature.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!)
        Me.txtBC_HeatingBoundaryTemperature.Location = New System.Drawing.Point(218, 286)
        Me.txtBC_HeatingBoundaryTemperature.Name = "txtBC_HeatingBoundaryTemperature"
        Me.txtBC_HeatingBoundaryTemperature.Size = New System.Drawing.Size(97, 21)
        Me.txtBC_HeatingBoundaryTemperature.TabIndex = 27
        '
        'lblBC_HeatingBoundaryTemperature
        '
        Me.lblBC_HeatingBoundaryTemperature.AutoSize = True
        Me.lblBC_HeatingBoundaryTemperature.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!)
        Me.lblBC_HeatingBoundaryTemperature.ForeColor = System.Drawing.Color.Black
        Me.lblBC_HeatingBoundaryTemperature.Location = New System.Drawing.Point(14, 289)
        Me.lblBC_HeatingBoundaryTemperature.Name = "lblBC_HeatingBoundaryTemperature"
        Me.lblBC_HeatingBoundaryTemperature.Size = New System.Drawing.Size(179, 15)
        Me.lblBC_HeatingBoundaryTemperature.TabIndex = 26
        Me.lblBC_HeatingBoundaryTemperature.Text = "Heating Boundary Temperature"
        '
        'txtBC_GFactor
        '
        Me.txtBC_GFactor.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!)
        Me.txtBC_GFactor.Location = New System.Drawing.Point(220, 30)
        Me.txtBC_GFactor.Name = "txtBC_GFactor"
        Me.txtBC_GFactor.Size = New System.Drawing.Size(97, 21)
        Me.txtBC_GFactor.TabIndex = 25
        '
        'lblGFactor
        '
        Me.lblGFactor.AutoSize = True
        Me.lblGFactor.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!)
        Me.lblGFactor.ForeColor = System.Drawing.Color.Black
        Me.lblGFactor.Location = New System.Drawing.Point(14, 33)
        Me.lblGFactor.Name = "lblGFactor"
        Me.lblGFactor.Size = New System.Drawing.Size(54, 15)
        Me.lblGFactor.TabIndex = 24
        Me.lblGFactor.Text = "G-Factor"
        '
        'txtBC_HeatPerPassengerIntoCabinW
        '
        Me.txtBC_HeatPerPassengerIntoCabinW.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!)
        Me.txtBC_HeatPerPassengerIntoCabinW.Location = New System.Drawing.Point(218, 90)
        Me.txtBC_HeatPerPassengerIntoCabinW.Name = "txtBC_HeatPerPassengerIntoCabinW"
        Me.txtBC_HeatPerPassengerIntoCabinW.ReadOnly = True
        Me.txtBC_HeatPerPassengerIntoCabinW.Size = New System.Drawing.Size(97, 21)
        Me.txtBC_HeatPerPassengerIntoCabinW.TabIndex = 23
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!)
        Me.Label2.ForeColor = System.Drawing.Color.Black
        Me.Label2.Location = New System.Drawing.Point(14, 88)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(153, 15)
        Me.Label2.TabIndex = 22
        Me.Label2.Text = "Heat/Passenger Into Cabin"
        '
        'lblUnitsUValues
        '
        Me.lblUnitsUValues.AutoSize = True
        Me.lblUnitsUValues.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblUnitsUValues.Location = New System.Drawing.Point(349, 261)
        Me.lblUnitsUValues.Name = "lblUnitsUValues"
        Me.lblUnitsUValues.Size = New System.Drawing.Size(66, 15)
        Me.lblUnitsUValues.TabIndex = 21
        Me.lblUnitsUValues.Text = "W/(K*m^3)"
        Me.ToolTip1.SetToolTip(Me.lblUnitsUValues, "W/(K*m^3)")
        '
        'lblUnitsPassengerBoundaryTemp
        '
        Me.lblUnitsPassengerBoundaryTemp.AutoSize = True
        Me.lblUnitsPassengerBoundaryTemp.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblUnitsPassengerBoundaryTemp.Location = New System.Drawing.Point(349, 123)
        Me.lblUnitsPassengerBoundaryTemp.Name = "lblUnitsPassengerBoundaryTemp"
        Me.lblUnitsPassengerBoundaryTemp.Size = New System.Drawing.Size(22, 15)
        Me.lblUnitsPassengerBoundaryTemp.TabIndex = 20
        Me.lblUnitsPassengerBoundaryTemp.Text = "oC"
        Me.ToolTip1.SetToolTip(Me.lblUnitsPassengerBoundaryTemp, "Degrees Centigrade")
        '
        'lblUnitsPGRDensitySemiLowFloor
        '
        Me.lblUnitsPGRDensitySemiLowFloor.AutoSize = True
        Me.lblUnitsPGRDensitySemiLowFloor.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblUnitsPGRDensitySemiLowFloor.Location = New System.Drawing.Point(349, 175)
        Me.lblUnitsPGRDensitySemiLowFloor.Name = "lblUnitsPGRDensitySemiLowFloor"
        Me.lblUnitsPGRDensitySemiLowFloor.Size = New System.Drawing.Size(61, 15)
        Me.lblUnitsPGRDensitySemiLowFloor.TabIndex = 19
        Me.lblUnitsPGRDensitySemiLowFloor.Text = "Pass/m^2"
        Me.ToolTip1.SetToolTip(Me.lblUnitsPGRDensitySemiLowFloor, "Passenger/Metres Squared")
        '
        'lblUnitsPassenderDensityLowFloor
        '
        Me.lblUnitsPassenderDensityLowFloor.AutoSize = True
        Me.lblUnitsPassenderDensityLowFloor.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblUnitsPassenderDensityLowFloor.Location = New System.Drawing.Point(349, 148)
        Me.lblUnitsPassenderDensityLowFloor.Name = "lblUnitsPassenderDensityLowFloor"
        Me.lblUnitsPassenderDensityLowFloor.Size = New System.Drawing.Size(61, 15)
        Me.lblUnitsPassenderDensityLowFloor.TabIndex = 18
        Me.lblUnitsPassenderDensityLowFloor.Text = "Pass/m^2"
        Me.ToolTip1.SetToolTip(Me.lblUnitsPassenderDensityLowFloor, "Passengers/MetreSquared")
        '
        'lblUnitsBC_PassengerDensityRaisedFloor
        '
        Me.lblUnitsBC_PassengerDensityRaisedFloor.AutoSize = True
        Me.lblUnitsBC_PassengerDensityRaisedFloor.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblUnitsBC_PassengerDensityRaisedFloor.Location = New System.Drawing.Point(349, 206)
        Me.lblUnitsBC_PassengerDensityRaisedFloor.Name = "lblUnitsBC_PassengerDensityRaisedFloor"
        Me.lblUnitsBC_PassengerDensityRaisedFloor.Size = New System.Drawing.Size(61, 15)
        Me.lblUnitsBC_PassengerDensityRaisedFloor.TabIndex = 17
        Me.lblUnitsBC_PassengerDensityRaisedFloor.Text = "Pass/m^2"
        Me.ToolTip1.SetToolTip(Me.lblUnitsBC_PassengerDensityRaisedFloor, "Passengers/Metre Squared")
        '
        'lblHeatPerPassengerIntoCabinW
        '
        Me.lblHeatPerPassengerIntoCabinW.AutoSize = True
        Me.lblHeatPerPassengerIntoCabinW.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblHeatPerPassengerIntoCabinW.Location = New System.Drawing.Point(349, 93)
        Me.lblHeatPerPassengerIntoCabinW.Name = "lblHeatPerPassengerIntoCabinW"
        Me.lblHeatPerPassengerIntoCabinW.Size = New System.Drawing.Size(18, 15)
        Me.lblHeatPerPassengerIntoCabinW.TabIndex = 16
        Me.lblHeatPerPassengerIntoCabinW.Text = "W"
        Me.ToolTip1.SetToolTip(Me.lblHeatPerPassengerIntoCabinW, "Watts")
        '
        'txtBC_UValues
        '
        Me.txtBC_UValues.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!)
        Me.txtBC_UValues.Location = New System.Drawing.Point(218, 258)
        Me.txtBC_UValues.Name = "txtBC_UValues"
        Me.txtBC_UValues.ReadOnly = True
        Me.txtBC_UValues.Size = New System.Drawing.Size(97, 21)
        Me.txtBC_UValues.TabIndex = 15
        '
        'txtBC_CalculatedPassengerNumber
        '
        Me.txtBC_CalculatedPassengerNumber.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!)
        Me.txtBC_CalculatedPassengerNumber.Location = New System.Drawing.Point(218, 229)
        Me.txtBC_CalculatedPassengerNumber.Name = "txtBC_CalculatedPassengerNumber"
        Me.txtBC_CalculatedPassengerNumber.ReadOnly = True
        Me.txtBC_CalculatedPassengerNumber.Size = New System.Drawing.Size(97, 21)
        Me.txtBC_CalculatedPassengerNumber.TabIndex = 14
        '
        'txtBC_PassengerDensityRaisedFloor
        '
        Me.txtBC_PassengerDensityRaisedFloor.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!)
        Me.txtBC_PassengerDensityRaisedFloor.Location = New System.Drawing.Point(218, 202)
        Me.txtBC_PassengerDensityRaisedFloor.Name = "txtBC_PassengerDensityRaisedFloor"
        Me.txtBC_PassengerDensityRaisedFloor.Size = New System.Drawing.Size(97, 21)
        Me.txtBC_PassengerDensityRaisedFloor.TabIndex = 13
        '
        'lblBC_PassengerDensityRaisedFloor
        '
        Me.lblBC_PassengerDensityRaisedFloor.AutoSize = True
        Me.lblBC_PassengerDensityRaisedFloor.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!)
        Me.lblBC_PassengerDensityRaisedFloor.ForeColor = System.Drawing.Color.Black
        Me.lblBC_PassengerDensityRaisedFloor.Location = New System.Drawing.Point(14, 205)
        Me.lblBC_PassengerDensityRaisedFloor.Name = "lblBC_PassengerDensityRaisedFloor"
        Me.lblBC_PassengerDensityRaisedFloor.Size = New System.Drawing.Size(182, 15)
        Me.lblBC_PassengerDensityRaisedFloor.TabIndex = 12
        Me.lblBC_PassengerDensityRaisedFloor.Text = "Passenger Density Raised Floor"
        '
        'lblBC_CalculatedPassengerNumber
        '
        Me.lblBC_CalculatedPassengerNumber.AutoSize = True
        Me.lblBC_CalculatedPassengerNumber.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!)
        Me.lblBC_CalculatedPassengerNumber.ForeColor = System.Drawing.Color.Black
        Me.lblBC_CalculatedPassengerNumber.Location = New System.Drawing.Point(14, 232)
        Me.lblBC_CalculatedPassengerNumber.Name = "lblBC_CalculatedPassengerNumber"
        Me.lblBC_CalculatedPassengerNumber.Size = New System.Drawing.Size(175, 15)
        Me.lblBC_CalculatedPassengerNumber.TabIndex = 11
        Me.lblBC_CalculatedPassengerNumber.Text = "Calculated Passenger Number"
        '
        'lblBC_UValues
        '
        Me.lblBC_UValues.AutoSize = True
        Me.lblBC_UValues.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!)
        Me.lblBC_UValues.ForeColor = System.Drawing.Color.Black
        Me.lblBC_UValues.Location = New System.Drawing.Point(14, 261)
        Me.lblBC_UValues.Name = "lblBC_UValues"
        Me.lblBC_UValues.Size = New System.Drawing.Size(57, 15)
        Me.lblBC_UValues.TabIndex = 10
        Me.lblBC_UValues.Text = "U-Values"
        '
        'txtBC_PassengerDensitySemiLowFloor
        '
        Me.txtBC_PassengerDensitySemiLowFloor.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!)
        Me.txtBC_PassengerDensitySemiLowFloor.Location = New System.Drawing.Point(218, 173)
        Me.txtBC_PassengerDensitySemiLowFloor.Name = "txtBC_PassengerDensitySemiLowFloor"
        Me.txtBC_PassengerDensitySemiLowFloor.Size = New System.Drawing.Size(97, 21)
        Me.txtBC_PassengerDensitySemiLowFloor.TabIndex = 9
        '
        'lblBC_PassengerDensitySemiLowFloor
        '
        Me.lblBC_PassengerDensitySemiLowFloor.AutoSize = True
        Me.lblBC_PassengerDensitySemiLowFloor.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!)
        Me.lblBC_PassengerDensitySemiLowFloor.ForeColor = System.Drawing.Color.Black
        Me.lblBC_PassengerDensitySemiLowFloor.Location = New System.Drawing.Point(14, 176)
        Me.lblBC_PassengerDensitySemiLowFloor.Name = "lblBC_PassengerDensitySemiLowFloor"
        Me.lblBC_PassengerDensitySemiLowFloor.Size = New System.Drawing.Size(198, 15)
        Me.lblBC_PassengerDensitySemiLowFloor.TabIndex = 8
        Me.lblBC_PassengerDensitySemiLowFloor.Text = "Passenger Density Semi Low Floor"
        '
        'txtBC_PassengerDensityLowFloor
        '
        Me.txtBC_PassengerDensityLowFloor.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!)
        Me.txtBC_PassengerDensityLowFloor.Location = New System.Drawing.Point(218, 145)
        Me.txtBC_PassengerDensityLowFloor.Name = "txtBC_PassengerDensityLowFloor"
        Me.txtBC_PassengerDensityLowFloor.Size = New System.Drawing.Size(97, 21)
        Me.txtBC_PassengerDensityLowFloor.TabIndex = 7
        '
        'Label13
        '
        Me.Label13.AutoSize = True
        Me.Label13.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!)
        Me.Label13.ForeColor = System.Drawing.Color.Black
        Me.Label13.Location = New System.Drawing.Point(14, 148)
        Me.Label13.Name = "Label13"
        Me.Label13.Size = New System.Drawing.Size(166, 15)
        Me.Label13.TabIndex = 6
        Me.Label13.Text = "Passenger Density Low Floor"
        '
        'txtBC_PassengerBoundaryTemperature
        '
        Me.txtBC_PassengerBoundaryTemperature.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!)
        Me.txtBC_PassengerBoundaryTemperature.Location = New System.Drawing.Point(218, 117)
        Me.txtBC_PassengerBoundaryTemperature.Name = "txtBC_PassengerBoundaryTemperature"
        Me.txtBC_PassengerBoundaryTemperature.Size = New System.Drawing.Size(97, 21)
        Me.txtBC_PassengerBoundaryTemperature.TabIndex = 5
        '
        'lblPassengerBoundaryTemp
        '
        Me.lblPassengerBoundaryTemp.AutoSize = True
        Me.lblPassengerBoundaryTemp.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!)
        Me.lblPassengerBoundaryTemp.ForeColor = System.Drawing.Color.Black
        Me.lblPassengerBoundaryTemp.Location = New System.Drawing.Point(14, 120)
        Me.lblPassengerBoundaryTemp.Name = "lblPassengerBoundaryTemp"
        Me.lblPassengerBoundaryTemp.Size = New System.Drawing.Size(156, 15)
        Me.lblPassengerBoundaryTemp.TabIndex = 4
        Me.lblPassengerBoundaryTemp.Text = "Passenger Boundary Temp"
        '
        'txtBC_SolarClouding
        '
        Me.txtBC_SolarClouding.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!)
        Me.txtBC_SolarClouding.Location = New System.Drawing.Point(218, 59)
        Me.txtBC_SolarClouding.Name = "txtBC_SolarClouding"
        Me.txtBC_SolarClouding.ReadOnly = True
        Me.txtBC_SolarClouding.Size = New System.Drawing.Size(97, 21)
        Me.txtBC_SolarClouding.TabIndex = 1
        '
        'lblSolarClouding
        '
        Me.lblSolarClouding.AutoSize = True
        Me.lblSolarClouding.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!)
        Me.lblSolarClouding.ForeColor = System.Drawing.Color.Black
        Me.lblSolarClouding.Location = New System.Drawing.Point(14, 62)
        Me.lblSolarClouding.Name = "lblSolarClouding"
        Me.lblSolarClouding.Size = New System.Drawing.Size(88, 15)
        Me.lblSolarClouding.TabIndex = 0
        Me.lblSolarClouding.Text = "Solar Clouding"
        '
        'tabGeneralInputsOther
        '
        Me.tabGeneralInputsOther.Controls.Add(Me.grpEnvironmentConditions)
        Me.tabGeneralInputsOther.Controls.Add(Me.grpAuxHeater)
        Me.tabGeneralInputsOther.Controls.Add(Me.grpVentilation)
        Me.tabGeneralInputsOther.Controls.Add(Me.grpACSystem)
        Me.tabGeneralInputsOther.Location = New System.Drawing.Point(4, 22)
        Me.tabGeneralInputsOther.Name = "tabGeneralInputsOther"
        Me.tabGeneralInputsOther.Size = New System.Drawing.Size(937, 611)
        Me.tabGeneralInputsOther.TabIndex = 3
        Me.tabGeneralInputsOther.Text = " INP - Other "
        Me.tabGeneralInputsOther.UseVisualStyleBackColor = True
        '
        'grpEnvironmentConditions
        '
        Me.grpEnvironmentConditions.BackColor = System.Drawing.Color.Transparent
        Me.grpEnvironmentConditions.Controls.Add(Me.btnOpenAenv)
        Me.grpEnvironmentConditions.Controls.Add(Me.btnEnvironmentConditionsSource)
        Me.grpEnvironmentConditions.Controls.Add(Me.txtEC_EnvironmentConditionsFilePath)
        Me.grpEnvironmentConditions.Controls.Add(Me.Label20)
        Me.grpEnvironmentConditions.Controls.Add(Me.chkEC_BatchMode)
        Me.grpEnvironmentConditions.Controls.Add(Me.Label23)
        Me.grpEnvironmentConditions.Controls.Add(Me.txtEC_EnviromentalTemperature)
        Me.grpEnvironmentConditions.Controls.Add(Me.lbltxtEC_EnviromentalTemperature)
        Me.grpEnvironmentConditions.Controls.Add(Me.txtEC_Solar)
        Me.grpEnvironmentConditions.Controls.Add(Me.lbltxtEC_Solar)
        Me.grpEnvironmentConditions.Controls.Add(Me.lblUnitstxtEC_EnviromentalTemperature)
        Me.grpEnvironmentConditions.Controls.Add(Me.lblUnitstxtEC_Solar)
        Me.grpEnvironmentConditions.FlatStyle = System.Windows.Forms.FlatStyle.Popup
        Me.grpEnvironmentConditions.Font = New System.Drawing.Font("Microsoft Sans Serif", 10.0!)
        Me.grpEnvironmentConditions.ForeColor = System.Drawing.SystemColors.MenuHighlight
        Me.grpEnvironmentConditions.Location = New System.Drawing.Point(34, 36)
        Me.grpEnvironmentConditions.Name = "grpEnvironmentConditions"
        Me.grpEnvironmentConditions.Size = New System.Drawing.Size(868, 158)
        Me.grpEnvironmentConditions.TabIndex = 33
        Me.grpEnvironmentConditions.TabStop = False
        Me.grpEnvironmentConditions.Text = "Environmental Conditions"
        '
        'btnOpenAenv
        '
        Me.btnOpenAenv.Image = CType(resources.GetObject("btnOpenAenv.Image"), System.Drawing.Image)
        Me.btnOpenAenv.Location = New System.Drawing.Point(783, 126)
        Me.btnOpenAenv.Name = "btnOpenAenv"
        Me.btnOpenAenv.Size = New System.Drawing.Size(28, 20)
        Me.btnOpenAenv.TabIndex = 62
        Me.btnOpenAenv.UseVisualStyleBackColor = True
        '
        'btnEnvironmentConditionsSource
        '
        Me.btnEnvironmentConditionsSource.Location = New System.Drawing.Point(721, 123)
        Me.btnEnvironmentConditionsSource.Name = "btnEnvironmentConditionsSource"
        Me.btnEnvironmentConditionsSource.Size = New System.Drawing.Size(45, 23)
        Me.btnEnvironmentConditionsSource.TabIndex = 61
        Me.btnEnvironmentConditionsSource.Text = ". . ."
        Me.btnEnvironmentConditionsSource.UseVisualStyleBackColor = True
        '
        'txtEC_EnvironmentConditionsFilePath
        '
        Me.txtEC_EnvironmentConditionsFilePath.Location = New System.Drawing.Point(216, 123)
        Me.txtEC_EnvironmentConditionsFilePath.Name = "txtEC_EnvironmentConditionsFilePath"
        Me.txtEC_EnvironmentConditionsFilePath.Size = New System.Drawing.Size(487, 23)
        Me.txtEC_EnvironmentConditionsFilePath.TabIndex = 60
        '
        'Label20
        '
        Me.Label20.AutoSize = True
        Me.Label20.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!)
        Me.Label20.ForeColor = System.Drawing.Color.Black
        Me.Label20.Location = New System.Drawing.Point(14, 126)
        Me.Label20.Name = "Label20"
        Me.Label20.Size = New System.Drawing.Size(174, 15)
        Me.Label20.TabIndex = 28
        Me.Label20.Text = "Enviromental Batch Conditions"
        '
        'chkEC_BatchMode
        '
        Me.chkEC_BatchMode.AutoSize = True
        Me.chkEC_BatchMode.Location = New System.Drawing.Point(216, 91)
        Me.chkEC_BatchMode.Name = "chkEC_BatchMode"
        Me.chkEC_BatchMode.Size = New System.Drawing.Size(31, 21)
        Me.chkEC_BatchMode.TabIndex = 27
        Me.chkEC_BatchMode.Text = " "
        Me.chkEC_BatchMode.UseVisualStyleBackColor = True
        '
        'Label23
        '
        Me.Label23.AutoSize = True
        Me.Label23.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!)
        Me.Label23.ForeColor = System.Drawing.Color.Black
        Me.Label23.Location = New System.Drawing.Point(14, 97)
        Me.Label23.Name = "Label23"
        Me.Label23.Size = New System.Drawing.Size(73, 15)
        Me.Label23.TabIndex = 27
        Me.Label23.Text = "Batch Mode"
        '
        'txtEC_EnviromentalTemperature
        '
        Me.txtEC_EnviromentalTemperature.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!)
        Me.txtEC_EnviromentalTemperature.Location = New System.Drawing.Point(215, 34)
        Me.txtEC_EnviromentalTemperature.Name = "txtEC_EnviromentalTemperature"
        Me.txtEC_EnviromentalTemperature.Size = New System.Drawing.Size(101, 21)
        Me.txtEC_EnviromentalTemperature.TabIndex = 25
        '
        'lbltxtEC_EnviromentalTemperature
        '
        Me.lbltxtEC_EnviromentalTemperature.AutoSize = True
        Me.lbltxtEC_EnviromentalTemperature.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!)
        Me.lbltxtEC_EnviromentalTemperature.ForeColor = System.Drawing.Color.Black
        Me.lbltxtEC_EnviromentalTemperature.Location = New System.Drawing.Point(13, 37)
        Me.lbltxtEC_EnviromentalTemperature.Name = "lbltxtEC_EnviromentalTemperature"
        Me.lbltxtEC_EnviromentalTemperature.Size = New System.Drawing.Size(153, 15)
        Me.lbltxtEC_EnviromentalTemperature.TabIndex = 24
        Me.lbltxtEC_EnviromentalTemperature.Text = "Enviromental Temperature"
        '
        'txtEC_Solar
        '
        Me.txtEC_Solar.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!)
        Me.txtEC_Solar.Location = New System.Drawing.Point(215, 63)
        Me.txtEC_Solar.Name = "txtEC_Solar"
        Me.txtEC_Solar.Size = New System.Drawing.Size(101, 21)
        Me.txtEC_Solar.TabIndex = 1
        '
        'lbltxtEC_Solar
        '
        Me.lbltxtEC_Solar.AutoSize = True
        Me.lbltxtEC_Solar.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!)
        Me.lbltxtEC_Solar.ForeColor = System.Drawing.Color.Black
        Me.lbltxtEC_Solar.Location = New System.Drawing.Point(13, 66)
        Me.lbltxtEC_Solar.Name = "lbltxtEC_Solar"
        Me.lbltxtEC_Solar.Size = New System.Drawing.Size(36, 15)
        Me.lbltxtEC_Solar.TabIndex = 0
        Me.lbltxtEC_Solar.Text = "Solar"
        '
        'lblUnitstxtEC_EnviromentalTemperature
        '
        Me.lblUnitstxtEC_EnviromentalTemperature.AutoSize = True
        Me.lblUnitstxtEC_EnviromentalTemperature.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblUnitstxtEC_EnviromentalTemperature.Location = New System.Drawing.Point(347, 37)
        Me.lblUnitstxtEC_EnviromentalTemperature.Name = "lblUnitstxtEC_EnviromentalTemperature"
        Me.lblUnitstxtEC_EnviromentalTemperature.Size = New System.Drawing.Size(22, 15)
        Me.lblUnitstxtEC_EnviromentalTemperature.TabIndex = 16
        Me.lblUnitstxtEC_EnviromentalTemperature.Text = "oC"
        Me.ToolTip1.SetToolTip(Me.lblUnitstxtEC_EnviromentalTemperature, "Degrees Centigrade")
        '
        'lblUnitstxtEC_Solar
        '
        Me.lblUnitstxtEC_Solar.AutoSize = True
        Me.lblUnitstxtEC_Solar.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblUnitstxtEC_Solar.Location = New System.Drawing.Point(347, 63)
        Me.lblUnitstxtEC_Solar.Name = "lblUnitstxtEC_Solar"
        Me.lblUnitstxtEC_Solar.Size = New System.Drawing.Size(45, 15)
        Me.lblUnitstxtEC_Solar.TabIndex = 26
        Me.lblUnitstxtEC_Solar.Text = "W/m^3"
        Me.ToolTip1.SetToolTip(Me.lblUnitstxtEC_Solar, "Watts/Metre Cubed")
        '
        'grpAuxHeater
        '
        Me.grpAuxHeater.BackColor = System.Drawing.Color.Transparent
        Me.grpAuxHeater.Controls.Add(Me.lblUnitsAH_FuelFiredHeater)
        Me.grpAuxHeater.Controls.Add(Me.txtAH_FuelFiredHeaterkW)
        Me.grpAuxHeater.Controls.Add(Me.lbltxtAH_FuelFiredHeaterkW)
        Me.grpAuxHeater.FlatStyle = System.Windows.Forms.FlatStyle.Popup
        Me.grpAuxHeater.Font = New System.Drawing.Font("Microsoft Sans Serif", 10.0!)
        Me.grpAuxHeater.ForeColor = System.Drawing.SystemColors.MenuHighlight
        Me.grpAuxHeater.Location = New System.Drawing.Point(474, 209)
        Me.grpAuxHeater.Name = "grpAuxHeater"
        Me.grpAuxHeater.Size = New System.Drawing.Size(409, 77)
        Me.grpAuxHeater.TabIndex = 32
        Me.grpAuxHeater.TabStop = False
        Me.grpAuxHeater.Text = "Aux Heater"
        '
        'lblUnitsAH_FuelFiredHeater
        '
        Me.lblUnitsAH_FuelFiredHeater.AutoSize = True
        Me.lblUnitsAH_FuelFiredHeater.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblUnitsAH_FuelFiredHeater.Location = New System.Drawing.Point(360, 36)
        Me.lblUnitsAH_FuelFiredHeater.Name = "lblUnitsAH_FuelFiredHeater"
        Me.lblUnitsAH_FuelFiredHeater.Size = New System.Drawing.Size(24, 15)
        Me.lblUnitsAH_FuelFiredHeater.TabIndex = 27
        Me.lblUnitsAH_FuelFiredHeater.Text = "Kw"
        Me.ToolTip1.SetToolTip(Me.lblUnitsAH_FuelFiredHeater, "Kilo Watts")
        '
        'txtAH_FuelFiredHeaterkW
        '
        Me.txtAH_FuelFiredHeaterkW.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!)
        Me.txtAH_FuelFiredHeaterkW.Location = New System.Drawing.Point(226, 33)
        Me.txtAH_FuelFiredHeaterkW.Name = "txtAH_FuelFiredHeaterkW"
        Me.txtAH_FuelFiredHeaterkW.Size = New System.Drawing.Size(97, 21)
        Me.txtAH_FuelFiredHeaterkW.TabIndex = 1
        '
        'lbltxtAH_FuelFiredHeaterkW
        '
        Me.lbltxtAH_FuelFiredHeaterkW.AutoSize = True
        Me.lbltxtAH_FuelFiredHeaterkW.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!)
        Me.lbltxtAH_FuelFiredHeaterkW.ForeColor = System.Drawing.Color.Black
        Me.lbltxtAH_FuelFiredHeaterkW.Location = New System.Drawing.Point(22, 36)
        Me.lbltxtAH_FuelFiredHeaterkW.Name = "lbltxtAH_FuelFiredHeaterkW"
        Me.lbltxtAH_FuelFiredHeaterkW.Size = New System.Drawing.Size(102, 15)
        Me.lbltxtAH_FuelFiredHeaterkW.TabIndex = 0
        Me.lbltxtAH_FuelFiredHeaterkW.Text = "Fuel Fired Heater"
        '
        'grpVentilation
        '
        Me.grpVentilation.BackColor = System.Drawing.Color.Transparent
        Me.grpVentilation.Controls.Add(Me.cboVEN_VentilationDuringCooling)
        Me.grpVentilation.Controls.Add(Me.cboVEN_VentilationDuringHeating)
        Me.grpVentilation.Controls.Add(Me.cboVEN_VentilationFlowSettingWhenHeatingAndACInactive)
        Me.grpVentilation.Controls.Add(Me.chkVEN_VentilationDuringAC)
        Me.grpVentilation.Controls.Add(Me.chkVEN_VentilationWhenBothHeatingAndACInactive)
        Me.grpVentilation.Controls.Add(Me.chkVEN_VentilationOnDuringHeating)
        Me.grpVentilation.Controls.Add(Me.lblcboVEN_VentilationDuringCooling)
        Me.grpVentilation.Controls.Add(Me.lblcboVEN_VentilationDuringHeating)
        Me.grpVentilation.Controls.Add(Me.lblcboVEN_VentilationFlowSettingWhenHeatingAndACInactive)
        Me.grpVentilation.Controls.Add(Me.lblchkVEN_VentilationOnDuringHeating)
        Me.grpVentilation.Controls.Add(Me.lblchkVEN_VentilationDuringAC)
        Me.grpVentilation.Controls.Add(Me.lblchkVEN_VentilationWhenBothHeatingAndACInactive)
        Me.grpVentilation.FlatStyle = System.Windows.Forms.FlatStyle.Popup
        Me.grpVentilation.Font = New System.Drawing.Font("Microsoft Sans Serif", 10.0!)
        Me.grpVentilation.ForeColor = System.Drawing.SystemColors.MenuHighlight
        Me.grpVentilation.Location = New System.Drawing.Point(34, 350)
        Me.grpVentilation.Name = "grpVentilation"
        Me.grpVentilation.Size = New System.Drawing.Size(409, 216)
        Me.grpVentilation.TabIndex = 31
        Me.grpVentilation.TabStop = False
        Me.grpVentilation.Text = "Ventilation"
        '
        'cboVEN_VentilationDuringCooling
        '
        Me.cboVEN_VentilationDuringCooling.FormattingEnabled = True
        Me.cboVEN_VentilationDuringCooling.Items.AddRange(New Object() {"High", "Low"})
        Me.cboVEN_VentilationDuringCooling.Location = New System.Drawing.Point(216, 182)
        Me.cboVEN_VentilationDuringCooling.Name = "cboVEN_VentilationDuringCooling"
        Me.cboVEN_VentilationDuringCooling.Size = New System.Drawing.Size(75, 24)
        Me.cboVEN_VentilationDuringCooling.TabIndex = 39
        '
        'cboVEN_VentilationDuringHeating
        '
        Me.cboVEN_VentilationDuringHeating.FormattingEnabled = True
        Me.cboVEN_VentilationDuringHeating.Items.AddRange(New Object() {"High", "Low"})
        Me.cboVEN_VentilationDuringHeating.Location = New System.Drawing.Point(216, 150)
        Me.cboVEN_VentilationDuringHeating.Name = "cboVEN_VentilationDuringHeating"
        Me.cboVEN_VentilationDuringHeating.Size = New System.Drawing.Size(75, 24)
        Me.cboVEN_VentilationDuringHeating.TabIndex = 38
        '
        'cboVEN_VentilationFlowSettingWhenHeatingAndACInactive
        '
        Me.cboVEN_VentilationFlowSettingWhenHeatingAndACInactive.FormattingEnabled = True
        Me.cboVEN_VentilationFlowSettingWhenHeatingAndACInactive.Items.AddRange(New Object() {"High", "Low"})
        Me.cboVEN_VentilationFlowSettingWhenHeatingAndACInactive.Location = New System.Drawing.Point(216, 117)
        Me.cboVEN_VentilationFlowSettingWhenHeatingAndACInactive.Name = "cboVEN_VentilationFlowSettingWhenHeatingAndACInactive"
        Me.cboVEN_VentilationFlowSettingWhenHeatingAndACInactive.Size = New System.Drawing.Size(75, 24)
        Me.cboVEN_VentilationFlowSettingWhenHeatingAndACInactive.TabIndex = 37
        '
        'chkVEN_VentilationDuringAC
        '
        Me.chkVEN_VentilationDuringAC.AutoSize = True
        Me.chkVEN_VentilationDuringAC.Location = New System.Drawing.Point(278, 85)
        Me.chkVEN_VentilationDuringAC.Name = "chkVEN_VentilationDuringAC"
        Me.chkVEN_VentilationDuringAC.Size = New System.Drawing.Size(31, 21)
        Me.chkVEN_VentilationDuringAC.TabIndex = 36
        Me.chkVEN_VentilationDuringAC.Text = " "
        Me.chkVEN_VentilationDuringAC.UseVisualStyleBackColor = True
        '
        'chkVEN_VentilationWhenBothHeatingAndACInactive
        '
        Me.chkVEN_VentilationWhenBothHeatingAndACInactive.AutoSize = True
        Me.chkVEN_VentilationWhenBothHeatingAndACInactive.Location = New System.Drawing.Point(278, 61)
        Me.chkVEN_VentilationWhenBothHeatingAndACInactive.Name = "chkVEN_VentilationWhenBothHeatingAndACInactive"
        Me.chkVEN_VentilationWhenBothHeatingAndACInactive.Size = New System.Drawing.Size(31, 21)
        Me.chkVEN_VentilationWhenBothHeatingAndACInactive.TabIndex = 35
        Me.chkVEN_VentilationWhenBothHeatingAndACInactive.Text = " "
        Me.chkVEN_VentilationWhenBothHeatingAndACInactive.UseVisualStyleBackColor = True
        '
        'chkVEN_VentilationOnDuringHeating
        '
        Me.chkVEN_VentilationOnDuringHeating.AutoSize = True
        Me.chkVEN_VentilationOnDuringHeating.Location = New System.Drawing.Point(279, 33)
        Me.chkVEN_VentilationOnDuringHeating.Name = "chkVEN_VentilationOnDuringHeating"
        Me.chkVEN_VentilationOnDuringHeating.Size = New System.Drawing.Size(31, 21)
        Me.chkVEN_VentilationOnDuringHeating.TabIndex = 34
        Me.chkVEN_VentilationOnDuringHeating.Text = " "
        Me.chkVEN_VentilationOnDuringHeating.UseVisualStyleBackColor = True
        '
        'lblcboVEN_VentilationDuringCooling
        '
        Me.lblcboVEN_VentilationDuringCooling.AutoSize = True
        Me.lblcboVEN_VentilationDuringCooling.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!)
        Me.lblcboVEN_VentilationDuringCooling.ForeColor = System.Drawing.Color.Black
        Me.lblcboVEN_VentilationDuringCooling.Location = New System.Drawing.Point(13, 184)
        Me.lblcboVEN_VentilationDuringCooling.Name = "lblcboVEN_VentilationDuringCooling"
        Me.lblcboVEN_VentilationDuringCooling.Size = New System.Drawing.Size(149, 15)
        Me.lblcboVEN_VentilationDuringCooling.TabIndex = 33
        Me.lblcboVEN_VentilationDuringCooling.Text = "Ventilation During Cooling"
        '
        'lblcboVEN_VentilationDuringHeating
        '
        Me.lblcboVEN_VentilationDuringHeating.AutoSize = True
        Me.lblcboVEN_VentilationDuringHeating.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!)
        Me.lblcboVEN_VentilationDuringHeating.ForeColor = System.Drawing.Color.Black
        Me.lblcboVEN_VentilationDuringHeating.Location = New System.Drawing.Point(14, 152)
        Me.lblcboVEN_VentilationDuringHeating.Name = "lblcboVEN_VentilationDuringHeating"
        Me.lblcboVEN_VentilationDuringHeating.Size = New System.Drawing.Size(150, 15)
        Me.lblcboVEN_VentilationDuringHeating.TabIndex = 30
        Me.lblcboVEN_VentilationDuringHeating.Text = "Ventilation During Heating"
        '
        'lblcboVEN_VentilationFlowSettingWhenHeatingAndACInactive
        '
        Me.lblcboVEN_VentilationFlowSettingWhenHeatingAndACInactive.AutoSize = True
        Me.lblcboVEN_VentilationFlowSettingWhenHeatingAndACInactive.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!)
        Me.lblcboVEN_VentilationFlowSettingWhenHeatingAndACInactive.ForeColor = System.Drawing.Color.Black
        Me.lblcboVEN_VentilationFlowSettingWhenHeatingAndACInactive.Location = New System.Drawing.Point(14, 115)
        Me.lblcboVEN_VentilationFlowSettingWhenHeatingAndACInactive.MaximumSize = New System.Drawing.Size(210, 0)
        Me.lblcboVEN_VentilationFlowSettingWhenHeatingAndACInactive.Name = "lblcboVEN_VentilationFlowSettingWhenHeatingAndACInactive"
        Me.lblcboVEN_VentilationFlowSettingWhenHeatingAndACInactive.Size = New System.Drawing.Size(169, 30)
        Me.lblcboVEN_VentilationFlowSettingWhenHeatingAndACInactive.TabIndex = 27
        Me.lblcboVEN_VentilationFlowSettingWhenHeatingAndACInactive.Text = "Ventilation FlowSetting When Heating And AC Inactive"
        '
        'lblchkVEN_VentilationOnDuringHeating
        '
        Me.lblchkVEN_VentilationOnDuringHeating.AutoSize = True
        Me.lblchkVEN_VentilationOnDuringHeating.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!)
        Me.lblchkVEN_VentilationOnDuringHeating.ForeColor = System.Drawing.Color.Black
        Me.lblchkVEN_VentilationOnDuringHeating.Location = New System.Drawing.Point(14, 33)
        Me.lblchkVEN_VentilationOnDuringHeating.Name = "lblchkVEN_VentilationOnDuringHeating"
        Me.lblchkVEN_VentilationOnDuringHeating.Size = New System.Drawing.Size(169, 15)
        Me.lblchkVEN_VentilationOnDuringHeating.TabIndex = 24
        Me.lblchkVEN_VentilationOnDuringHeating.Text = "Ventilation On During Heating"
        '
        'lblchkVEN_VentilationDuringAC
        '
        Me.lblchkVEN_VentilationDuringAC.AutoSize = True
        Me.lblchkVEN_VentilationDuringAC.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!)
        Me.lblchkVEN_VentilationDuringAC.ForeColor = System.Drawing.Color.Black
        Me.lblchkVEN_VentilationDuringAC.Location = New System.Drawing.Point(14, 88)
        Me.lblchkVEN_VentilationDuringAC.Name = "lblchkVEN_VentilationDuringAC"
        Me.lblchkVEN_VentilationDuringAC.Size = New System.Drawing.Size(122, 15)
        Me.lblchkVEN_VentilationDuringAC.TabIndex = 22
        Me.lblchkVEN_VentilationDuringAC.Text = "Ventilation During AC"
        '
        'lblchkVEN_VentilationWhenBothHeatingAndACInactive
        '
        Me.lblchkVEN_VentilationWhenBothHeatingAndACInactive.AutoSize = True
        Me.lblchkVEN_VentilationWhenBothHeatingAndACInactive.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!)
        Me.lblchkVEN_VentilationWhenBothHeatingAndACInactive.ForeColor = System.Drawing.Color.Black
        Me.lblchkVEN_VentilationWhenBothHeatingAndACInactive.Location = New System.Drawing.Point(14, 62)
        Me.lblchkVEN_VentilationWhenBothHeatingAndACInactive.Name = "lblchkVEN_VentilationWhenBothHeatingAndACInactive"
        Me.lblchkVEN_VentilationWhenBothHeatingAndACInactive.Size = New System.Drawing.Size(256, 15)
        Me.lblchkVEN_VentilationWhenBothHeatingAndACInactive.TabIndex = 0
        Me.lblchkVEN_VentilationWhenBothHeatingAndACInactive.Text = "Ventilation When Both Heating And ACInactive"
        '
        'grpACSystem
        '
        Me.grpACSystem.BackColor = System.Drawing.Color.Transparent
        Me.grpACSystem.Controls.Add(Me.cboAC_CompressorType)
        Me.grpACSystem.Controls.Add(Me.chkAC_InCabinRoomAC_System)
        Me.grpACSystem.Controls.Add(Me.lblChkAC_InCabinRoomAC_System)
        Me.grpACSystem.Controls.Add(Me.txtAC_CompressorCapacitykW)
        Me.grpACSystem.Controls.Add(Me.lbltxtAC_CompressorCapacitykW)
        Me.grpACSystem.Controls.Add(Me.lblUnitstxtAC_CompressorCapacitykW)
        Me.grpACSystem.Controls.Add(Me.lblcboAC_CompressorType)
        Me.grpACSystem.FlatStyle = System.Windows.Forms.FlatStyle.Popup
        Me.grpACSystem.Font = New System.Drawing.Font("Microsoft Sans Serif", 10.0!)
        Me.grpACSystem.ForeColor = System.Drawing.SystemColors.MenuHighlight
        Me.grpACSystem.Location = New System.Drawing.Point(34, 209)
        Me.grpACSystem.Name = "grpACSystem"
        Me.grpACSystem.Size = New System.Drawing.Size(409, 135)
        Me.grpACSystem.TabIndex = 30
        Me.grpACSystem.TabStop = False
        Me.grpACSystem.Text = "AC-System"
        '
        'cboAC_CompressorType
        '
        Me.cboAC_CompressorType.FormattingEnabled = True
        Me.cboAC_CompressorType.Items.AddRange(New Object() {"Mechanical", "Electrical"})
        Me.cboAC_CompressorType.Location = New System.Drawing.Point(217, 62)
        Me.cboAC_CompressorType.Name = "cboAC_CompressorType"
        Me.cboAC_CompressorType.Size = New System.Drawing.Size(100, 24)
        Me.cboAC_CompressorType.TabIndex = 26
        '
        'chkAC_InCabinRoomAC_System
        '
        Me.chkAC_InCabinRoomAC_System.AutoSize = True
        Me.chkAC_InCabinRoomAC_System.Location = New System.Drawing.Point(220, 33)
        Me.chkAC_InCabinRoomAC_System.Name = "chkAC_InCabinRoomAC_System"
        Me.chkAC_InCabinRoomAC_System.Size = New System.Drawing.Size(31, 21)
        Me.chkAC_InCabinRoomAC_System.TabIndex = 25
        Me.chkAC_InCabinRoomAC_System.Text = " "
        Me.chkAC_InCabinRoomAC_System.UseVisualStyleBackColor = True
        '
        'lblChkAC_InCabinRoomAC_System
        '
        Me.lblChkAC_InCabinRoomAC_System.AutoSize = True
        Me.lblChkAC_InCabinRoomAC_System.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!)
        Me.lblChkAC_InCabinRoomAC_System.ForeColor = System.Drawing.Color.Black
        Me.lblChkAC_InCabinRoomAC_System.Location = New System.Drawing.Point(14, 33)
        Me.lblChkAC_InCabinRoomAC_System.Name = "lblChkAC_InCabinRoomAC_System"
        Me.lblChkAC_InCabinRoomAC_System.Size = New System.Drawing.Size(154, 15)
        Me.lblChkAC_InCabinRoomAC_System.TabIndex = 24
        Me.lblChkAC_InCabinRoomAC_System.Text = "In Cabin Room AC_System"
        '
        'txtAC_CompressorCapacitykW
        '
        Me.txtAC_CompressorCapacitykW.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!)
        Me.txtAC_CompressorCapacitykW.Location = New System.Drawing.Point(218, 92)
        Me.txtAC_CompressorCapacitykW.Name = "txtAC_CompressorCapacitykW"
        Me.txtAC_CompressorCapacitykW.Size = New System.Drawing.Size(99, 21)
        Me.txtAC_CompressorCapacitykW.TabIndex = 23
        '
        'lbltxtAC_CompressorCapacitykW
        '
        Me.lbltxtAC_CompressorCapacitykW.AutoSize = True
        Me.lbltxtAC_CompressorCapacitykW.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!)
        Me.lbltxtAC_CompressorCapacitykW.ForeColor = System.Drawing.Color.Black
        Me.lbltxtAC_CompressorCapacitykW.Location = New System.Drawing.Point(14, 91)
        Me.lbltxtAC_CompressorCapacitykW.Name = "lbltxtAC_CompressorCapacitykW"
        Me.lbltxtAC_CompressorCapacitykW.Size = New System.Drawing.Size(140, 15)
        Me.lbltxtAC_CompressorCapacitykW.TabIndex = 22
        Me.lbltxtAC_CompressorCapacitykW.Text = "AC-Compressor capacity"
        '
        'lblUnitstxtAC_CompressorCapacitykW
        '
        Me.lblUnitstxtAC_CompressorCapacitykW.AutoSize = True
        Me.lblUnitstxtAC_CompressorCapacitykW.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblUnitstxtAC_CompressorCapacitykW.Location = New System.Drawing.Point(352, 99)
        Me.lblUnitstxtAC_CompressorCapacitykW.Name = "lblUnitstxtAC_CompressorCapacitykW"
        Me.lblUnitstxtAC_CompressorCapacitykW.Size = New System.Drawing.Size(24, 15)
        Me.lblUnitstxtAC_CompressorCapacitykW.TabIndex = 16
        Me.lblUnitstxtAC_CompressorCapacitykW.Text = "Kw"
        Me.ToolTip1.SetToolTip(Me.lblUnitstxtAC_CompressorCapacitykW, "Kilo Watts")
        '
        'lblcboAC_CompressorType
        '
        Me.lblcboAC_CompressorType.AutoSize = True
        Me.lblcboAC_CompressorType.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!)
        Me.lblcboAC_CompressorType.ForeColor = System.Drawing.Color.Black
        Me.lblcboAC_CompressorType.Location = New System.Drawing.Point(14, 62)
        Me.lblcboAC_CompressorType.Name = "lblcboAC_CompressorType"
        Me.lblcboAC_CompressorType.Size = New System.Drawing.Size(122, 15)
        Me.lblcboAC_CompressorType.TabIndex = 0
        Me.lblcboAC_CompressorType.Text = "AC-Compressor Type"
        '
        'tabTechBenefits
        '
        Me.tabTechBenefits.CausesValidation = False
        Me.tabTechBenefits.Controls.Add(Me.btnClearForm)
        Me.tabTechBenefits.Controls.Add(Me.lblIndex)
        Me.tabTechBenefits.Controls.Add(Me.txtIndex)
        Me.tabTechBenefits.Controls.Add(Me.btnUpdate)
        Me.tabTechBenefits.Controls.Add(Me.lblLineType)
        Me.tabTechBenefits.Controls.Add(Me.chkActiveVC)
        Me.tabTechBenefits.Controls.Add(Me.lblCoolingColumn)
        Me.tabTechBenefits.Controls.Add(Me.chkActiveVV)
        Me.tabTechBenefits.Controls.Add(Me.txtBenefitName)
        Me.tabTechBenefits.Controls.Add(Me.chkActiveVH)
        Me.tabTechBenefits.Controls.Add(Me.lblBenefitName)
        Me.tabTechBenefits.Controls.Add(Me.chkOnVehicle)
        Me.tabTechBenefits.Controls.Add(Me.lblVentelationColumn)
        Me.tabTechBenefits.Controls.Add(Me.lblHeatingColumn)
        Me.tabTechBenefits.Controls.Add(Me.cboLineType)
        Me.tabTechBenefits.Controls.Add(Me.pnlRaisedFloorRow)
        Me.tabTechBenefits.Controls.Add(Me.pnlSemiLowFloorRow)
        Me.tabTechBenefits.Controls.Add(Me.pnlLowFloorRow)
        Me.tabTechBenefits.Controls.Add(Me.lblCategory)
        Me.tabTechBenefits.Controls.Add(Me.cboCategory)
        Me.tabTechBenefits.Controls.Add(Me.gvTechBenefitLines)
        Me.tabTechBenefits.Controls.Add(Me.lblUnits)
        Me.tabTechBenefits.Controls.Add(Me.cboUnits)
        Me.tabTechBenefits.Location = New System.Drawing.Point(4, 22)
        Me.tabTechBenefits.Name = "tabTechBenefits"
        Me.tabTechBenefits.Padding = New System.Windows.Forms.Padding(3)
        Me.tabTechBenefits.Size = New System.Drawing.Size(937, 611)
        Me.tabTechBenefits.TabIndex = 5
        Me.tabTechBenefits.Text = " Tech List Input "
        Me.tabTechBenefits.UseVisualStyleBackColor = True
        '
        'btnClearForm
        '
        Me.btnClearForm.Location = New System.Drawing.Point(814, 100)
        Me.btnClearForm.Name = "btnClearForm"
        Me.btnClearForm.Size = New System.Drawing.Size(75, 23)
        Me.btnClearForm.TabIndex = 34
        Me.btnClearForm.Text = "Clear Form"
        Me.btnClearForm.UseVisualStyleBackColor = True
        '
        'lblIndex
        '
        Me.lblIndex.AutoSize = True
        Me.lblIndex.Location = New System.Drawing.Point(29, 18)
        Me.lblIndex.Name = "lblIndex"
        Me.lblIndex.Size = New System.Drawing.Size(33, 13)
        Me.lblIndex.TabIndex = 33
        Me.lblIndex.Text = "Index"
        '
        'txtIndex
        '
        Me.txtIndex.Location = New System.Drawing.Point(81, 15)
        Me.txtIndex.Name = "txtIndex"
        Me.txtIndex.ReadOnly = True
        Me.txtIndex.Size = New System.Drawing.Size(58, 20)
        Me.txtIndex.TabIndex = 32
        '
        'btnUpdate
        '
        Me.btnUpdate.Location = New System.Drawing.Point(814, 18)
        Me.btnUpdate.Name = "btnUpdate"
        Me.btnUpdate.Size = New System.Drawing.Size(75, 23)
        Me.btnUpdate.TabIndex = 25
        Me.btnUpdate.Text = "Update/Add"
        Me.btnUpdate.UseVisualStyleBackColor = True
        '
        'lblLineType
        '
        Me.lblLineType.AutoSize = True
        Me.lblLineType.Location = New System.Drawing.Point(415, 74)
        Me.lblLineType.Name = "lblLineType"
        Me.lblLineType.Size = New System.Drawing.Size(51, 13)
        Me.lblLineType.TabIndex = 31
        Me.lblLineType.Text = "LineType"
        '
        'chkActiveVC
        '
        Me.chkActiveVC.AutoSize = True
        Me.chkActiveVC.Location = New System.Drawing.Point(490, 173)
        Me.chkActiveVC.Name = "chkActiveVC"
        Me.chkActiveVC.Size = New System.Drawing.Size(73, 17)
        Me.chkActiveVC.TabIndex = 16
        Me.chkActiveVC.Text = "Active VC"
        Me.chkActiveVC.UseVisualStyleBackColor = True
        '
        'lblCoolingColumn
        '
        Me.lblCoolingColumn.AutoSize = True
        Me.lblCoolingColumn.Location = New System.Drawing.Point(316, 100)
        Me.lblCoolingColumn.Name = "lblCoolingColumn"
        Me.lblCoolingColumn.Size = New System.Drawing.Size(42, 13)
        Me.lblCoolingColumn.TabIndex = 30
        Me.lblCoolingColumn.Text = "Cooling"
        '
        'chkActiveVV
        '
        Me.chkActiveVV.AutoSize = True
        Me.chkActiveVV.Location = New System.Drawing.Point(490, 145)
        Me.chkActiveVV.Name = "chkActiveVV"
        Me.chkActiveVV.Size = New System.Drawing.Size(73, 17)
        Me.chkActiveVV.TabIndex = 15
        Me.chkActiveVV.Text = "Active VV"
        Me.chkActiveVV.UseVisualStyleBackColor = True
        '
        'txtBenefitName
        '
        Me.txtBenefitName.Location = New System.Drawing.Point(488, 41)
        Me.txtBenefitName.Name = "txtBenefitName"
        Me.txtBenefitName.Size = New System.Drawing.Size(270, 20)
        Me.txtBenefitName.TabIndex = 2
        '
        'chkActiveVH
        '
        Me.chkActiveVH.AutoSize = True
        Me.chkActiveVH.Location = New System.Drawing.Point(490, 117)
        Me.chkActiveVH.Name = "chkActiveVH"
        Me.chkActiveVH.Size = New System.Drawing.Size(74, 17)
        Me.chkActiveVH.TabIndex = 14
        Me.chkActiveVH.Text = "Active VH"
        Me.chkActiveVH.UseVisualStyleBackColor = True
        '
        'lblBenefitName
        '
        Me.lblBenefitName.AutoSize = True
        Me.lblBenefitName.Location = New System.Drawing.Point(411, 44)
        Me.lblBenefitName.Name = "lblBenefitName"
        Me.lblBenefitName.Size = New System.Drawing.Size(71, 13)
        Me.lblBenefitName.TabIndex = 18
        Me.lblBenefitName.Text = "Benefit Name"
        '
        'chkOnVehicle
        '
        Me.chkOnVehicle.AutoSize = True
        Me.chkOnVehicle.Location = New System.Drawing.Point(490, 202)
        Me.chkOnVehicle.Name = "chkOnVehicle"
        Me.chkOnVehicle.Size = New System.Drawing.Size(78, 17)
        Me.chkOnVehicle.TabIndex = 17
        Me.chkOnVehicle.Text = "On Vehicle"
        Me.chkOnVehicle.UseVisualStyleBackColor = True
        '
        'lblVentelationColumn
        '
        Me.lblVentelationColumn.AutoSize = True
        Me.lblVentelationColumn.Location = New System.Drawing.Point(215, 100)
        Me.lblVentelationColumn.Name = "lblVentelationColumn"
        Me.lblVentelationColumn.Size = New System.Drawing.Size(56, 13)
        Me.lblVentelationColumn.TabIndex = 29
        Me.lblVentelationColumn.Text = "Ventilation"
        '
        'lblHeatingColumn
        '
        Me.lblHeatingColumn.AutoSize = True
        Me.lblHeatingColumn.Location = New System.Drawing.Point(122, 100)
        Me.lblHeatingColumn.Name = "lblHeatingColumn"
        Me.lblHeatingColumn.Size = New System.Drawing.Size(44, 13)
        Me.lblHeatingColumn.TabIndex = 28
        Me.lblHeatingColumn.Text = "Heating"
        '
        'cboLineType
        '
        Me.cboLineType.AutoCompleteCustomSource.AddRange(New String() {"Normal", "Active Ventelation"})
        Me.cboLineType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cboLineType.FormattingEnabled = True
        Me.cboLineType.Items.AddRange(New Object() {"Normal", "ActiveVentilation"})
        Me.cboLineType.Location = New System.Drawing.Point(488, 71)
        Me.cboLineType.Name = "cboLineType"
        Me.cboLineType.Size = New System.Drawing.Size(270, 21)
        Me.cboLineType.TabIndex = 4
        '
        'pnlRaisedFloorRow
        '
        Me.pnlRaisedFloorRow.BackColor = System.Drawing.Color.Lavender
        Me.pnlRaisedFloorRow.Controls.Add(Me.txtRaisedFloorH)
        Me.pnlRaisedFloorRow.Controls.Add(Me.txtRaisedFloorC)
        Me.pnlRaisedFloorRow.Controls.Add(Me.txtRaisedFloorV)
        Me.pnlRaisedFloorRow.Controls.Add(Me.lblRaisedFloorRow)
        Me.pnlRaisedFloorRow.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!)
        Me.pnlRaisedFloorRow.ForeColor = System.Drawing.Color.Black
        Me.pnlRaisedFloorRow.Location = New System.Drawing.Point(26, 193)
        Me.pnlRaisedFloorRow.Name = "pnlRaisedFloorRow"
        Me.pnlRaisedFloorRow.Size = New System.Drawing.Size(379, 33)
        Me.pnlRaisedFloorRow.TabIndex = 9
        '
        'txtRaisedFloorH
        '
        Me.txtRaisedFloorH.Location = New System.Drawing.Point(84, 6)
        Me.txtRaisedFloorH.Name = "txtRaisedFloorH"
        Me.txtRaisedFloorH.Size = New System.Drawing.Size(70, 20)
        Me.txtRaisedFloorH.TabIndex = 11
        '
        'txtRaisedFloorC
        '
        Me.txtRaisedFloorC.Location = New System.Drawing.Point(281, 6)
        Me.txtRaisedFloorC.Name = "txtRaisedFloorC"
        Me.txtRaisedFloorC.Size = New System.Drawing.Size(70, 20)
        Me.txtRaisedFloorC.TabIndex = 13
        '
        'txtRaisedFloorV
        '
        Me.txtRaisedFloorV.Location = New System.Drawing.Point(184, 6)
        Me.txtRaisedFloorV.Name = "txtRaisedFloorV"
        Me.txtRaisedFloorV.Size = New System.Drawing.Size(70, 20)
        Me.txtRaisedFloorV.TabIndex = 12
        '
        'lblRaisedFloorRow
        '
        Me.lblRaisedFloorRow.AutoSize = True
        Me.lblRaisedFloorRow.Location = New System.Drawing.Point(3, 9)
        Me.lblRaisedFloorRow.Name = "lblRaisedFloorRow"
        Me.lblRaisedFloorRow.Size = New System.Drawing.Size(66, 13)
        Me.lblRaisedFloorRow.TabIndex = 23
        Me.lblRaisedFloorRow.Text = "Raised Floor"
        '
        'pnlSemiLowFloorRow
        '
        Me.pnlSemiLowFloorRow.BackColor = System.Drawing.Color.Lavender
        Me.pnlSemiLowFloorRow.Controls.Add(Me.txtSemiLowFloorH)
        Me.pnlSemiLowFloorRow.Controls.Add(Me.txtSemiLowFloorC)
        Me.pnlSemiLowFloorRow.Controls.Add(Me.txtSemiLowFloorV)
        Me.pnlSemiLowFloorRow.Controls.Add(Me.lblSemiLowFloorRow)
        Me.pnlSemiLowFloorRow.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!)
        Me.pnlSemiLowFloorRow.ForeColor = System.Drawing.Color.Black
        Me.pnlSemiLowFloorRow.Location = New System.Drawing.Point(26, 155)
        Me.pnlSemiLowFloorRow.Name = "pnlSemiLowFloorRow"
        Me.pnlSemiLowFloorRow.Size = New System.Drawing.Size(379, 33)
        Me.pnlSemiLowFloorRow.TabIndex = 8
        '
        'txtSemiLowFloorH
        '
        Me.txtSemiLowFloorH.Location = New System.Drawing.Point(84, 6)
        Me.txtSemiLowFloorH.Name = "txtSemiLowFloorH"
        Me.txtSemiLowFloorH.Size = New System.Drawing.Size(70, 20)
        Me.txtSemiLowFloorH.TabIndex = 8
        '
        'txtSemiLowFloorC
        '
        Me.txtSemiLowFloorC.Location = New System.Drawing.Point(281, 6)
        Me.txtSemiLowFloorC.Name = "txtSemiLowFloorC"
        Me.txtSemiLowFloorC.Size = New System.Drawing.Size(70, 20)
        Me.txtSemiLowFloorC.TabIndex = 10
        '
        'txtSemiLowFloorV
        '
        Me.txtSemiLowFloorV.Location = New System.Drawing.Point(184, 6)
        Me.txtSemiLowFloorV.Name = "txtSemiLowFloorV"
        Me.txtSemiLowFloorV.Size = New System.Drawing.Size(70, 20)
        Me.txtSemiLowFloorV.TabIndex = 9
        '
        'lblSemiLowFloorRow
        '
        Me.lblSemiLowFloorRow.AutoSize = True
        Me.lblSemiLowFloorRow.Location = New System.Drawing.Point(3, 9)
        Me.lblSemiLowFloorRow.Name = "lblSemiLowFloorRow"
        Me.lblSemiLowFloorRow.Size = New System.Drawing.Size(79, 13)
        Me.lblSemiLowFloorRow.TabIndex = 22
        Me.lblSemiLowFloorRow.Text = "Semi Low Floor"
        '
        'pnlLowFloorRow
        '
        Me.pnlLowFloorRow.BackColor = System.Drawing.Color.Lavender
        Me.pnlLowFloorRow.Controls.Add(Me.txtLowFloorH)
        Me.pnlLowFloorRow.Controls.Add(Me.txtLowFloorC)
        Me.pnlLowFloorRow.Controls.Add(Me.txtLowFloorV)
        Me.pnlLowFloorRow.Controls.Add(Me.lblLowFloorRow)
        Me.pnlLowFloorRow.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!)
        Me.pnlLowFloorRow.ForeColor = System.Drawing.Color.Black
        Me.pnlLowFloorRow.Location = New System.Drawing.Point(26, 116)
        Me.pnlLowFloorRow.Name = "pnlLowFloorRow"
        Me.pnlLowFloorRow.Size = New System.Drawing.Size(379, 33)
        Me.pnlLowFloorRow.TabIndex = 5
        '
        'txtLowFloorH
        '
        Me.txtLowFloorH.Location = New System.Drawing.Point(84, 6)
        Me.txtLowFloorH.Name = "txtLowFloorH"
        Me.txtLowFloorH.Size = New System.Drawing.Size(70, 20)
        Me.txtLowFloorH.TabIndex = 5
        '
        'txtLowFloorC
        '
        Me.txtLowFloorC.Location = New System.Drawing.Point(280, 6)
        Me.txtLowFloorC.Name = "txtLowFloorC"
        Me.txtLowFloorC.Size = New System.Drawing.Size(70, 20)
        Me.txtLowFloorC.TabIndex = 7
        '
        'txtLowFloorV
        '
        Me.txtLowFloorV.Location = New System.Drawing.Point(184, 6)
        Me.txtLowFloorV.Name = "txtLowFloorV"
        Me.txtLowFloorV.Size = New System.Drawing.Size(70, 20)
        Me.txtLowFloorV.TabIndex = 6
        '
        'lblLowFloorRow
        '
        Me.lblLowFloorRow.AutoSize = True
        Me.lblLowFloorRow.Location = New System.Drawing.Point(7, 9)
        Me.lblLowFloorRow.Name = "lblLowFloorRow"
        Me.lblLowFloorRow.Size = New System.Drawing.Size(53, 13)
        Me.lblLowFloorRow.TabIndex = 21
        Me.lblLowFloorRow.Text = "Low Floor"
        '
        'lblCategory
        '
        Me.lblCategory.AutoSize = True
        Me.lblCategory.Location = New System.Drawing.Point(26, 71)
        Me.lblCategory.Name = "lblCategory"
        Me.lblCategory.Size = New System.Drawing.Size(49, 13)
        Me.lblCategory.TabIndex = 19
        Me.lblCategory.Text = "Category"
        '
        'cboCategory
        '
        Me.cboCategory.FormattingEnabled = True
        Me.cboCategory.Location = New System.Drawing.Point(81, 71)
        Me.cboCategory.Name = "cboCategory"
        Me.cboCategory.Size = New System.Drawing.Size(324, 21)
        Me.cboCategory.TabIndex = 3
        '
        'gvTechBenefitLines
        '
        Me.gvTechBenefitLines.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.gvTechBenefitLines.Location = New System.Drawing.Point(21, 238)
        Me.gvTechBenefitLines.Name = "gvTechBenefitLines"
        Me.gvTechBenefitLines.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect
        Me.gvTechBenefitLines.Size = New System.Drawing.Size(885, 356)
        Me.gvTechBenefitLines.TabIndex = 30
        '
        'lblUnits
        '
        Me.lblUnits.AutoSize = True
        Me.lblUnits.Location = New System.Drawing.Point(26, 44)
        Me.lblUnits.Name = "lblUnits"
        Me.lblUnits.Size = New System.Drawing.Size(31, 13)
        Me.lblUnits.TabIndex = 20
        Me.lblUnits.Text = "Units"
        '
        'cboUnits
        '
        Me.cboUnits.AutoCompleteCustomSource.AddRange(New String() {"Fraction"})
        Me.cboUnits.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cboUnits.FormattingEnabled = True
        Me.cboUnits.Items.AddRange(New Object() {"Fraction"})
        Me.cboUnits.Location = New System.Drawing.Point(81, 44)
        Me.cboUnits.Name = "cboUnits"
        Me.cboUnits.Size = New System.Drawing.Size(324, 21)
        Me.cboUnits.TabIndex = 1
        '
        'tabDiagnostics
        '
        Me.tabDiagnostics.Controls.Add(Me.txtDiagnostics)
        Me.tabDiagnostics.Location = New System.Drawing.Point(4, 22)
        Me.tabDiagnostics.Name = "tabDiagnostics"
        Me.tabDiagnostics.Size = New System.Drawing.Size(937, 611)
        Me.tabDiagnostics.TabIndex = 6
        Me.tabDiagnostics.Text = "Diagnostics"
        Me.tabDiagnostics.UseVisualStyleBackColor = True
        '
        'txtDiagnostics
        '
        Me.txtDiagnostics.Font = New System.Drawing.Font("Courier New", 8.0!)
        Me.txtDiagnostics.Location = New System.Drawing.Point(22, 17)
        Me.txtDiagnostics.Multiline = True
        Me.txtDiagnostics.Name = "txtDiagnostics"
        Me.txtDiagnostics.ScrollBars = System.Windows.Forms.ScrollBars.Both
        Me.txtDiagnostics.Size = New System.Drawing.Size(877, 584)
        Me.txtDiagnostics.TabIndex = 0
        Me.txtDiagnostics.WordWrap = False
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.ForeColor = System.Drawing.SystemColors.MenuHighlight
        Me.Label1.Location = New System.Drawing.Point(746, 30)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(82, 13)
        Me.Label1.TabIndex = 10
        Me.Label1.Text = "Electrical Adj W"
        Me.ToolTip1.SetToolTip(Me.Label1, "Electrical W - Tech List Adjusted")
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.ForeColor = System.Drawing.SystemColors.MenuHighlight
        Me.Label4.Location = New System.Drawing.Point(746, 57)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(94, 13)
        Me.Label4.TabIndex = 12
        Me.Label4.Text = "Mechanical Adj W"
        Me.ToolTip1.SetToolTip(Me.Label4, "Mechanical W - Tech List Adjusted")
        '
        'ErrorProvider1
        '
        Me.ErrorProvider1.ContainerControl = Me
        '
        'btnSave
        '
        Me.btnSave.Location = New System.Drawing.Point(767, 734)
        Me.btnSave.Name = "btnSave"
        Me.btnSave.Size = New System.Drawing.Size(75, 23)
        Me.btnSave.TabIndex = 1
        Me.btnSave.Text = "Save"
        Me.btnSave.UseVisualStyleBackColor = True
        '
        'btnCancel
        '
        Me.btnCancel.Location = New System.Drawing.Point(872, 734)
        Me.btnCancel.Name = "btnCancel"
        Me.btnCancel.Size = New System.Drawing.Size(75, 23)
        Me.btnCancel.TabIndex = 2
        Me.btnCancel.Text = "Cancel"
        Me.btnCancel.UseVisualStyleBackColor = True
        '
        'txtBasElectrical
        '
        Me.txtBasElectrical.Location = New System.Drawing.Point(634, 26)
        Me.txtBasElectrical.Name = "txtBasElectrical"
        Me.txtBasElectrical.Size = New System.Drawing.Size(100, 20)
        Me.txtBasElectrical.TabIndex = 3
        '
        'txtBaseMechanical
        '
        Me.txtBaseMechanical.Location = New System.Drawing.Point(634, 53)
        Me.txtBaseMechanical.Name = "txtBaseMechanical"
        Me.txtBaseMechanical.Size = New System.Drawing.Size(100, 20)
        Me.txtBaseMechanical.TabIndex = 4
        '
        'txtAdjMechanical
        '
        Me.txtAdjMechanical.Location = New System.Drawing.Point(847, 53)
        Me.txtAdjMechanical.Name = "txtAdjMechanical"
        Me.txtAdjMechanical.Size = New System.Drawing.Size(100, 20)
        Me.txtAdjMechanical.TabIndex = 7
        '
        'txtAdjElectrical
        '
        Me.txtAdjElectrical.Location = New System.Drawing.Point(847, 26)
        Me.txtAdjElectrical.Name = "txtAdjElectrical"
        Me.txtAdjElectrical.Size = New System.Drawing.Size(100, 20)
        Me.txtAdjElectrical.TabIndex = 6
        '
        'Timer1
        '
        Me.Timer1.Interval = 1000
        '
        'lblElectricalBaseW
        '
        Me.lblElectricalBaseW.AutoSize = True
        Me.lblElectricalBaseW.ForeColor = System.Drawing.SystemColors.MenuHighlight
        Me.lblElectricalBaseW.Location = New System.Drawing.Point(528, 30)
        Me.lblElectricalBaseW.Name = "lblElectricalBaseW"
        Me.lblElectricalBaseW.Size = New System.Drawing.Size(91, 13)
        Me.lblElectricalBaseW.TabIndex = 9
        Me.lblElectricalBaseW.Text = "Electrical Base W"
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.ForeColor = System.Drawing.SystemColors.MenuHighlight
        Me.Label3.Location = New System.Drawing.Point(528, 57)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(103, 13)
        Me.Label3.TabIndex = 11
        Me.Label3.Text = "Mechanical Base W"
        '
        'CMFiles
        '
        Me.CMFiles.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.OpenWithToolStripMenuItem, Me.ShowInFolderToolStripMenuItem})
        Me.CMFiles.Name = "CMFiles"
        Me.CMFiles.Size = New System.Drawing.Size(177, 48)
        '
        'OpenWithToolStripMenuItem
        '
        Me.OpenWithToolStripMenuItem.Name = "OpenWithToolStripMenuItem"
        Me.OpenWithToolStripMenuItem.Size = New System.Drawing.Size(176, 22)
        Me.OpenWithToolStripMenuItem.Text = "Open with notepad"
        '
        'ShowInFolderToolStripMenuItem
        '
        Me.ShowInFolderToolStripMenuItem.Name = "ShowInFolderToolStripMenuItem"
        Me.ShowInFolderToolStripMenuItem.Size = New System.Drawing.Size(176, 22)
        Me.ShowInFolderToolStripMenuItem.Text = "Open in Folder"
        '
        'frmHVACTool
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.AutoValidate = System.Windows.Forms.AutoValidate.EnableAllowFocusChange
        Me.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.ClientSize = New System.Drawing.Size(965, 766)
        Me.Controls.Add(Me.Label4)
        Me.Controls.Add(Me.Label3)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.lblElectricalBaseW)
        Me.Controls.Add(Me.txtAdjMechanical)
        Me.Controls.Add(Me.txtAdjElectrical)
        Me.Controls.Add(Me.txtBaseMechanical)
        Me.Controls.Add(Me.txtBasElectrical)
        Me.Controls.Add(Me.btnCancel)
        Me.Controls.Add(Me.btnSave)
        Me.Controls.Add(Me.tabMain)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle
        Me.MaximizeBox = False
        Me.Name = "frmHVACTool"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.Text = "SSM HVAC Model V01 ( Excel Model V04 )"
        Me.tabMain.ResumeLayout(False)
        Me.tabGeneralInputsBP.ResumeLayout(False)
        Me.BusParamGroupEdit.ResumeLayout(False)
        Me.BusParamGroupEdit.PerformLayout()
        Me.BusParamGroupModel.ResumeLayout(False)
        Me.BusParamGroupModel.PerformLayout()
        Me.tabGeneralInputsBC.ResumeLayout(False)
        Me.GroupBox2.ResumeLayout(False)
        Me.GroupBox2.PerformLayout()
        Me.tabGeneralInputsOther.ResumeLayout(False)
        Me.grpEnvironmentConditions.ResumeLayout(False)
        Me.grpEnvironmentConditions.PerformLayout()
        Me.grpAuxHeater.ResumeLayout(False)
        Me.grpAuxHeater.PerformLayout()
        Me.grpVentilation.ResumeLayout(False)
        Me.grpVentilation.PerformLayout()
        Me.grpACSystem.ResumeLayout(False)
        Me.grpACSystem.PerformLayout()
        Me.tabTechBenefits.ResumeLayout(False)
        Me.tabTechBenefits.PerformLayout()
        Me.pnlRaisedFloorRow.ResumeLayout(False)
        Me.pnlRaisedFloorRow.PerformLayout()
        Me.pnlSemiLowFloorRow.ResumeLayout(False)
        Me.pnlSemiLowFloorRow.PerformLayout()
        Me.pnlLowFloorRow.ResumeLayout(False)
        Me.pnlLowFloorRow.PerformLayout()
        CType(Me.gvTechBenefitLines, System.ComponentModel.ISupportInitialize).EndInit()
        Me.tabDiagnostics.ResumeLayout(False)
        Me.tabDiagnostics.PerformLayout()
        CType(Me.ErrorProvider1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.CMFiles.ResumeLayout(False)
        Me.ResumeLayout(False)
        Me.PerformLayout

End Sub
    Friend WithEvents tabMain As System.Windows.Forms.TabControl
    Friend WithEvents tabGeneralInputsBP As System.Windows.Forms.TabPage
    Friend WithEvents ToolTip1 As System.Windows.Forms.ToolTip
    Friend WithEvents ErrorProvider1 As System.Windows.Forms.ErrorProvider
    Friend WithEvents tabGeneralInputsBC As System.Windows.Forms.TabPage
    Friend WithEvents GroupBox2 As System.Windows.Forms.GroupBox
    Friend WithEvents txtBC_GFactor As System.Windows.Forms.TextBox
    Friend WithEvents lblGFactor As System.Windows.Forms.Label
    Friend WithEvents txtBC_HeatPerPassengerIntoCabinW As System.Windows.Forms.TextBox
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents lblUnitsUValues As System.Windows.Forms.Label
    Friend WithEvents lblUnitsPassengerBoundaryTemp As System.Windows.Forms.Label
    Friend WithEvents lblUnitsPGRDensitySemiLowFloor As System.Windows.Forms.Label
    Friend WithEvents lblUnitsPassenderDensityLowFloor As System.Windows.Forms.Label
    Friend WithEvents lblUnitsBC_PassengerDensityRaisedFloor As System.Windows.Forms.Label
    Friend WithEvents lblHeatPerPassengerIntoCabinW As System.Windows.Forms.Label
    Friend WithEvents txtBC_UValues As System.Windows.Forms.TextBox
    Friend WithEvents txtBC_CalculatedPassengerNumber As System.Windows.Forms.TextBox
    Friend WithEvents txtBC_PassengerDensityRaisedFloor As System.Windows.Forms.TextBox
    Friend WithEvents lblBC_PassengerDensityRaisedFloor As System.Windows.Forms.Label
    Friend WithEvents lblBC_CalculatedPassengerNumber As System.Windows.Forms.Label
    Friend WithEvents lblBC_UValues As System.Windows.Forms.Label
    Friend WithEvents txtBC_PassengerDensitySemiLowFloor As System.Windows.Forms.TextBox
    Friend WithEvents lblBC_PassengerDensitySemiLowFloor As System.Windows.Forms.Label
    Friend WithEvents txtBC_PassengerDensityLowFloor As System.Windows.Forms.TextBox
    Friend WithEvents Label13 As System.Windows.Forms.Label
    Friend WithEvents txtBC_PassengerBoundaryTemperature As System.Windows.Forms.TextBox
    Friend WithEvents lblPassengerBoundaryTemp As System.Windows.Forms.Label
    Friend WithEvents txtBC_SolarClouding As System.Windows.Forms.TextBox
    Friend WithEvents lblSolarClouding As System.Windows.Forms.Label
    Friend WithEvents lblUnitsBC_lowVentilation As System.Windows.Forms.Label
    Friend WithEvents txtBC_lowVentilation As System.Windows.Forms.TextBox
    Friend WithEvents lblBC_lowVentilation As System.Windows.Forms.Label
    Friend WithEvents lblUnitsBC_HighVentilation As System.Windows.Forms.Label
    Friend WithEvents txtBC_HighVentilation As System.Windows.Forms.TextBox
    Friend WithEvents lblBC_HighVentilation As System.Windows.Forms.Label
    Friend WithEvents lblUnitsBC_CoolingBoundaryTemperature As System.Windows.Forms.Label
    Friend WithEvents txtBC_CoolingBoundaryTemperature As System.Windows.Forms.TextBox
    Friend WithEvents lblBC_CoolingBoundaryTemperature As System.Windows.Forms.Label
    Friend WithEvents lblUnitsBC_HeatingBoundaryTemperature As System.Windows.Forms.Label
    Friend WithEvents txtBC_HeatingBoundaryTemperature As System.Windows.Forms.TextBox
    Friend WithEvents lblBC_HeatingBoundaryTemperature As System.Windows.Forms.Label
    Friend WithEvents lblUnitsBC_SpecificVentilationPower As System.Windows.Forms.Label
    Friend WithEvents txtBC_SpecificVentilationPower As System.Windows.Forms.TextBox
    Friend WithEvents lvlBC_SpecificVentilationPower As System.Windows.Forms.Label
    Friend WithEvents lblUnitsBC_LowVentPowerW As System.Windows.Forms.Label
    Friend WithEvents txtBC_LowVentPowerW As System.Windows.Forms.TextBox
    Friend WithEvents lblBC_LowVentPowerW As System.Windows.Forms.Label
    Friend WithEvents lblUnitsBC_HighVentPowerW As System.Windows.Forms.Label
    Friend WithEvents txtBC_HighVentPowerW As System.Windows.Forms.TextBox
    Friend WithEvents lblBC_HighVentPowerW As System.Windows.Forms.Label
    Friend WithEvents lblUnitsBC_Low As System.Windows.Forms.Label
    Friend WithEvents txtBC_Low As System.Windows.Forms.TextBox
    Friend WithEvents lblBC_Low As System.Windows.Forms.Label
    Friend WithEvents lblUnitsBC_High As System.Windows.Forms.Label
    Friend WithEvents txtBC_High As System.Windows.Forms.TextBox
    Friend WithEvents lblBC_High As System.Windows.Forms.Label
    Friend WithEvents lblUnitsBC_VolumicMassDieselOrHeatingOil As System.Windows.Forms.Label
    Friend WithEvents txtBC_VolumicMassDieselOrHeatingOil As System.Windows.Forms.TextBox
    Friend WithEvents lblBC_VolumicMassDieselOrHeatingOil As System.Windows.Forms.Label
    Friend WithEvents lblUnitsBC_GCVDieselOrHeatingOil As System.Windows.Forms.Label
    Friend WithEvents txtBC_GCVDieselOrHeatingOil As System.Windows.Forms.TextBox
    Friend WithEvents lblBC_GCVDieselOrHeatingOil As System.Windows.Forms.Label
    Friend WithEvents txtBC_AuxHeaterEfficiency As System.Windows.Forms.TextBox
    Friend WithEvents lblBC_AuxHeaterEfficiency As System.Windows.Forms.Label
    Friend WithEvents lblUnitsBC_COP As System.Windows.Forms.Label
    Friend WithEvents txtBC_COP As System.Windows.Forms.TextBox
    Friend WithEvents lblBC_COP As System.Windows.Forms.Label
    Friend WithEvents lblUnitsBC_MaxPossibleBenefitFromTechnologyList As System.Windows.Forms.Label
    Friend WithEvents txtBC_MaxPossibleBenefitFromTechnologyList As System.Windows.Forms.TextBox
    Friend WithEvents lblBC_MaxPossibleBenefitFromTechnologyList As System.Windows.Forms.Label
    Friend WithEvents lblUnitsBC_MaxTemperatureDeltaForLowFloorBusses As System.Windows.Forms.Label
    Friend WithEvents txtBC_MaxTemperatureDeltaForLowFloorBusses As System.Windows.Forms.TextBox
    Friend WithEvents lblBC_MaxTemperatureDeltaForLowFloorBusses As System.Windows.Forms.Label
    Friend WithEvents lblUnitsBC_FrontRearWindowArea As System.Windows.Forms.Label
    Friend WithEvents txtBC_FrontRearWindowArea As System.Windows.Forms.TextBox
    Friend WithEvents lblBC_FrontRearWindowArea As System.Windows.Forms.Label
    Friend WithEvents lblUnitsBC_WindowAreaPerUnitBusLength As System.Windows.Forms.Label
    Friend WithEvents txtBC_WindowAreaPerUnitBusLength As System.Windows.Forms.TextBox
    Friend WithEvents lblBC_WindowAreaPerUnitBusLength As System.Windows.Forms.Label
    Friend WithEvents tabGeneralInputsOther As System.Windows.Forms.TabPage
    Friend WithEvents grpAuxHeater As System.Windows.Forms.GroupBox
    Friend WithEvents txtAH_FuelFiredHeaterkW As System.Windows.Forms.TextBox
    Friend WithEvents lbltxtAH_FuelFiredHeaterkW As System.Windows.Forms.Label
    Friend WithEvents grpVentilation As System.Windows.Forms.GroupBox
    Friend WithEvents cboVEN_VentilationDuringCooling As System.Windows.Forms.ComboBox
    Friend WithEvents cboVEN_VentilationDuringHeating As System.Windows.Forms.ComboBox
    Friend WithEvents cboVEN_VentilationFlowSettingWhenHeatingAndACInactive As System.Windows.Forms.ComboBox
    Friend WithEvents chkVEN_VentilationDuringAC As System.Windows.Forms.CheckBox
    Friend WithEvents chkVEN_VentilationWhenBothHeatingAndACInactive As System.Windows.Forms.CheckBox
    Friend WithEvents chkVEN_VentilationOnDuringHeating As System.Windows.Forms.CheckBox
    Friend WithEvents lblcboVEN_VentilationDuringCooling As System.Windows.Forms.Label
    Friend WithEvents lblcboVEN_VentilationDuringHeating As System.Windows.Forms.Label
    Friend WithEvents lblcboVEN_VentilationFlowSettingWhenHeatingAndACInactive As System.Windows.Forms.Label
    Friend WithEvents lblchkVEN_VentilationOnDuringHeating As System.Windows.Forms.Label
    Friend WithEvents lblchkVEN_VentilationDuringAC As System.Windows.Forms.Label
    Friend WithEvents lblchkVEN_VentilationWhenBothHeatingAndACInactive As System.Windows.Forms.Label
    Friend WithEvents grpACSystem As System.Windows.Forms.GroupBox
    Friend WithEvents cboAC_CompressorType As System.Windows.Forms.ComboBox
    Friend WithEvents chkAC_InCabinRoomAC_System As System.Windows.Forms.CheckBox
    Friend WithEvents lblChkAC_InCabinRoomAC_System As System.Windows.Forms.Label
    Friend WithEvents txtAC_CompressorCapacitykW As System.Windows.Forms.TextBox
    Friend WithEvents lbltxtAC_CompressorCapacitykW As System.Windows.Forms.Label
    Friend WithEvents lblUnitstxtAC_CompressorCapacitykW As System.Windows.Forms.Label
    Friend WithEvents lblcboAC_CompressorType As System.Windows.Forms.Label
    Friend WithEvents tabTechBenefits As System.Windows.Forms.TabPage
    Friend WithEvents gvTechBenefitLines As System.Windows.Forms.DataGridView
    Friend WithEvents lblCategory As System.Windows.Forms.Label
    Friend WithEvents lblRaisedFloorRow As System.Windows.Forms.Label
    Friend WithEvents chkActiveVC As System.Windows.Forms.CheckBox
    Friend WithEvents cboCategory As System.Windows.Forms.ComboBox
    Friend WithEvents chkActiveVV As System.Windows.Forms.CheckBox
    Friend WithEvents lblSemiLowFloorRow As System.Windows.Forms.Label
    Friend WithEvents chkActiveVH As System.Windows.Forms.CheckBox
    Friend WithEvents lblUnits As System.Windows.Forms.Label
    Friend WithEvents chkOnVehicle As System.Windows.Forms.CheckBox
    Friend WithEvents lblLowFloorRow As System.Windows.Forms.Label
    Friend WithEvents cboUnits As System.Windows.Forms.ComboBox
    Friend WithEvents txtRaisedFloorC As System.Windows.Forms.TextBox
    Friend WithEvents cboLineType As System.Windows.Forms.ComboBox
    Friend WithEvents txtLowFloorH As System.Windows.Forms.TextBox
    Friend WithEvents txtRaisedFloorV As System.Windows.Forms.TextBox
    Friend WithEvents txtLowFloorV As System.Windows.Forms.TextBox
    Friend WithEvents txtRaisedFloorH As System.Windows.Forms.TextBox
    Friend WithEvents txtLowFloorC As System.Windows.Forms.TextBox
    Friend WithEvents txtSemiLowFloorC As System.Windows.Forms.TextBox
    Friend WithEvents txtSemiLowFloorH As System.Windows.Forms.TextBox
    Friend WithEvents txtSemiLowFloorV As System.Windows.Forms.TextBox
    Friend WithEvents lblBenefitName As System.Windows.Forms.Label
    Friend WithEvents txtBenefitName As System.Windows.Forms.TextBox
    Friend WithEvents lblCoolingColumn As System.Windows.Forms.Label
    Friend WithEvents lblVentelationColumn As System.Windows.Forms.Label
    Friend WithEvents lblHeatingColumn As System.Windows.Forms.Label
    Friend WithEvents pnlRaisedFloorRow As System.Windows.Forms.Panel
    Friend WithEvents pnlSemiLowFloorRow As System.Windows.Forms.Panel
    Friend WithEvents pnlLowFloorRow As System.Windows.Forms.Panel
    Friend WithEvents btnUpdate As System.Windows.Forms.Button
    Friend WithEvents lblLineType As System.Windows.Forms.Label
    Friend WithEvents lblIndex As System.Windows.Forms.Label
    Friend WithEvents txtIndex As System.Windows.Forms.TextBox
    Friend WithEvents btnCancel As System.Windows.Forms.Button
    Friend WithEvents btnSave As System.Windows.Forms.Button
    Friend WithEvents btnClearForm As System.Windows.Forms.Button
    Friend WithEvents txtAdjMechanical As System.Windows.Forms.TextBox
    Friend WithEvents txtAdjElectrical As System.Windows.Forms.TextBox
    Friend WithEvents txtBaseMechanical As System.Windows.Forms.TextBox
    Friend WithEvents txtBasElectrical As System.Windows.Forms.TextBox
    Friend WithEvents Timer1 As System.Windows.Forms.Timer
    Friend WithEvents tabDiagnostics As System.Windows.Forms.TabPage
    Friend WithEvents txtDiagnostics As System.Windows.Forms.TextBox
    Friend WithEvents lblUnitsAH_FuelFiredHeater As System.Windows.Forms.Label
    Friend WithEvents Label4 As System.Windows.Forms.Label
    Friend WithEvents Label3 As System.Windows.Forms.Label
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents lblElectricalBaseW As System.Windows.Forms.Label
    Friend WithEvents btnCancelBus As System.Windows.Forms.Button
    Friend WithEvents BusParamGroupEdit As System.Windows.Forms.GroupBox
    Friend WithEvents Label19 As System.Windows.Forms.Label
    Friend WithEvents chkEditIsDoubleDecker As System.Windows.Forms.CheckBox
    Friend WithEvents cmbEditEngineType As System.Windows.Forms.ComboBox
    Friend WithEvents cmbEditFloorType As System.Windows.Forms.ComboBox
    Friend WithEvents txtEditBusModel As System.Windows.Forms.TextBox
    Friend WithEvents Label7 As System.Windows.Forms.Label
    Friend WithEvents Label8 As System.Windows.Forms.Label
    Friend WithEvents Label9 As System.Windows.Forms.Label
    Friend WithEvents Label12 As System.Windows.Forms.Label
    Friend WithEvents Label14 As System.Windows.Forms.Label
    Friend WithEvents Label16 As System.Windows.Forms.Label
    Friend WithEvents txtEditBusHeight As System.Windows.Forms.TextBox
    Friend WithEvents txtEditBusWidth As System.Windows.Forms.TextBox
    Friend WithEvents txtEditBusLength As System.Windows.Forms.TextBox
    Friend WithEvents Label17 As System.Windows.Forms.Label
    Friend WithEvents Label18 As System.Windows.Forms.Label
    Friend WithEvents Label21 As System.Windows.Forms.Label
    Friend WithEvents txtEditBusPassengers As System.Windows.Forms.TextBox
    Friend WithEvents Label22 As System.Windows.Forms.Label
    Friend WithEvents btnEditBus As System.Windows.Forms.Button
    Friend WithEvents btnUpdateBusDatabase As System.Windows.Forms.Button
    Friend WithEvents btnNewBus As System.Windows.Forms.Button
    Friend WithEvents BusParamGroupModel As System.Windows.Forms.GroupBox
    Friend WithEvents Label15 As System.Windows.Forms.Label
    Friend WithEvents chkIsDoubleDecker As System.Windows.Forms.CheckBox
    Friend WithEvents cmbBusFloorType As System.Windows.Forms.ComboBox
    Friend WithEvents Label10 As System.Windows.Forms.Label
    Friend WithEvents txtBusHeight As System.Windows.Forms.TextBox
    Friend WithEvents Label11 As System.Windows.Forms.Label
    Friend WithEvents txtBusModel As System.Windows.Forms.TextBox
    Friend WithEvents lblBusModel As System.Windows.Forms.Label
    Friend WithEvents lblBusFloorType As System.Windows.Forms.Label
    Friend WithEvents lblUnitsBW As System.Windows.Forms.Label
    Friend WithEvents lblUnitsBSA As System.Windows.Forms.Label
    Friend WithEvents lblUnitsBWSA As System.Windows.Forms.Label
    Friend WithEvents lblUnitsBV As System.Windows.Forms.Label
    Friend WithEvents lblUnitsBL As System.Windows.Forms.Label
    Friend WithEvents lblUnitsBFSA As System.Windows.Forms.Label
    Friend WithEvents txtBusWidth As System.Windows.Forms.TextBox
    Friend WithEvents txtBusLength As System.Windows.Forms.TextBox
    Friend WithEvents txtBusVolume As System.Windows.Forms.TextBox
    Friend WithEvents lblBusVolume As System.Windows.Forms.Label
    Friend WithEvents lblBusLength As System.Windows.Forms.Label
    Friend WithEvents lblBusWidth As System.Windows.Forms.Label
    Friend WithEvents txtBusWindowSurfaceArea As System.Windows.Forms.TextBox
    Friend WithEvents lblBusWindowSurfaceArea As System.Windows.Forms.Label
    Friend WithEvents txtBusSurfaceArea As System.Windows.Forms.TextBox
    Friend WithEvents lblBusSurfaceArea As System.Windows.Forms.Label
    Friend WithEvents txtBusFloorSurfaceArea As System.Windows.Forms.TextBox
    Friend WithEvents lblBusFloorSurfaceArea As System.Windows.Forms.Label
    Friend WithEvents txtRegisteredPassengers As System.Windows.Forms.TextBox
    Friend WithEvents lblRegisteredPassengers As System.Windows.Forms.Label
    Friend WithEvents cboBuses As System.Windows.Forms.ComboBox
    Friend WithEvents grpEnvironmentConditions As System.Windows.Forms.GroupBox
    Friend WithEvents btnOpenAenv As System.Windows.Forms.Button
    Friend WithEvents btnEnvironmentConditionsSource As System.Windows.Forms.Button
    Friend WithEvents txtEC_EnvironmentConditionsFilePath As System.Windows.Forms.TextBox
    Friend WithEvents Label20 As System.Windows.Forms.Label
    Friend WithEvents chkEC_BatchMode As System.Windows.Forms.CheckBox
    Friend WithEvents Label23 As System.Windows.Forms.Label
    Friend WithEvents txtEC_EnviromentalTemperature As System.Windows.Forms.TextBox
    Friend WithEvents lbltxtEC_EnviromentalTemperature As System.Windows.Forms.Label
    Friend WithEvents txtEC_Solar As System.Windows.Forms.TextBox
    Friend WithEvents lbltxtEC_Solar As System.Windows.Forms.Label
    Friend WithEvents lblUnitstxtEC_EnviromentalTemperature As System.Windows.Forms.Label
    Friend WithEvents lblUnitstxtEC_Solar As System.Windows.Forms.Label
    Friend WithEvents CMFiles As System.Windows.Forms.ContextMenuStrip
    Friend WithEvents OpenWithToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ShowInFolderToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
End Class
