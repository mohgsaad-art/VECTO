<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class Dashboard
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
        Dim DataGridViewCellStyle25 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle26 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle27 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle28 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle29 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle30 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle31 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle32 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle33 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle34 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle35 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle36 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Me.pnlMain = New System.Windows.Forms.Panel()
        Me.btnCancel = New System.Windows.Forms.Button()
        Me.btnForward = New System.Windows.Forms.Button()
        Me.btnBack = New System.Windows.Forms.Button()
        Me.tabMain = New System.Windows.Forms.TabControl()
        Me.tabGeneralConfig = New System.Windows.Forms.TabPage()
        Me.cboCycle = New System.Windows.Forms.ComboBox()
        Me.lblCycle = New System.Windows.Forms.Label()
        Me.lblVehiceWeight = New System.Windows.Forms.Label()
        Me.txtVehicleWeightKG = New System.Windows.Forms.TextBox()
        Me.tabElectricalConfig = New System.Windows.Forms.TabPage()
        Me.btnAlternatorMapPath = New System.Windows.Forms.Button()
        Me.gvResultsCardOverrun = New System.Windows.Forms.DataGridView()
        Me.gvResultsCardTraction = New System.Windows.Forms.DataGridView()
        Me.gvResultsCardIdle = New System.Windows.Forms.DataGridView()
        Me.lblResultsOverrun = New System.Windows.Forms.Label()
        Me.lblResultsTractionOn = New System.Windows.Forms.Label()
        Me.lblResultsIdle = New System.Windows.Forms.Label()
        Me.chkSmartElectricals = New System.Windows.Forms.CheckBox()
        Me.lblElectricalConsumables = New System.Windows.Forms.Label()
        Me.gvElectricalConsumables = New System.Windows.Forms.DataGridView()
        Me.txtDoorActuationTimeSeconds = New System.Windows.Forms.TextBox()
        Me.txtAlternatorGearEfficiency = New System.Windows.Forms.TextBox()
        Me.txtAlternatorMapPath = New System.Windows.Forms.TextBox()
        Me.txtPowernetVoltage = New System.Windows.Forms.TextBox()
        Me.lblDoorActuationTimeSeconds = New System.Windows.Forms.Label()
        Me.lblAlternatorGearEfficiency = New System.Windows.Forms.Label()
        Me.lblAlternatormapPath = New System.Windows.Forms.Label()
        Me.lblPowerNetVoltage = New System.Windows.Forms.Label()
        Me.tabPneumaticConfig = New System.Windows.Forms.TabPage()
        Me.pnlPneumaticsUserInput = New System.Windows.Forms.Panel()
        Me.Button1 = New System.Windows.Forms.Button()
        Me.btnCompressorMap = New System.Windows.Forms.Button()
        Me.lblPneumaticsVariablesTitle = New System.Windows.Forms.Label()
        Me.lblCompressorType = New System.Windows.Forms.Label()
        Me.lblActuationsMap = New System.Windows.Forms.Label()
        Me.chkSmartAirCompression = New System.Windows.Forms.CheckBox()
        Me.lblSmartRegeneration = New System.Windows.Forms.Label()
        Me.chkSmartRegeneration = New System.Windows.Forms.CheckBox()
        Me.lblAdBlueDosing = New System.Windows.Forms.Label()
        Me.chkRetarderBrake = New System.Windows.Forms.CheckBox()
        Me.lblSmartAirCompression = New System.Windows.Forms.Label()
        Me.txtKneelingHeightMillimeters = New System.Windows.Forms.TextBox()
        Me.lblAirSuspensionControl = New System.Windows.Forms.Label()
        Me.cboDoors = New System.Windows.Forms.ComboBox()
        Me.lblRetarderBrake = New System.Windows.Forms.Label()
        Me.cboCompressorType = New System.Windows.Forms.ComboBox()
        Me.txtCompressorMap = New System.Windows.Forms.TextBox()
        Me.lblCompressorGearEfficiency = New System.Windows.Forms.Label()
        Me.txtCompressorGearRatio = New System.Windows.Forms.TextBox()
        Me.lblCompressorGearRatio = New System.Windows.Forms.Label()
        Me.txtCompressorGearEfficiency = New System.Windows.Forms.TextBox()
        Me.lblCompressorMap = New System.Windows.Forms.Label()
        Me.cboAirSuspensionControl = New System.Windows.Forms.ComboBox()
        Me.lblDoors = New System.Windows.Forms.Label()
        Me.cboAdBlueDosing = New System.Windows.Forms.ComboBox()
        Me.lblKneelingHeightMillimeters = New System.Windows.Forms.Label()
        Me.txtActuationsMap = New System.Windows.Forms.TextBox()
        Me.pnlPneumaticAuxillaries = New System.Windows.Forms.Panel()
        Me.lblPneumaticAuxillariesTitle = New System.Windows.Forms.Label()
        Me.lblAdBlueNIperMinute = New System.Windows.Forms.Label()
        Me.lblAirControlledSuspensionNIperMinute = New System.Windows.Forms.Label()
        Me.lblBrakingNoRetarderNIperKG = New System.Windows.Forms.Label()
        Me.lblBrakingWithRetarderNIperKG = New System.Windows.Forms.Label()
        Me.lblBreakingPerKneelingNIperKGinMM = New System.Windows.Forms.Label()
        Me.lblDeadVolBlowOutsPerLitresperHour = New System.Windows.Forms.Label()
        Me.lblDeadVolumeLitres = New System.Windows.Forms.Label()
        Me.lblNonSmartRegenFractionTotalAirDemand = New System.Windows.Forms.Label()
        Me.lblOverrunUtilisationForCompressionFraction = New System.Windows.Forms.Label()
        Me.lblPerDoorOpeningNI = New System.Windows.Forms.Label()
        Me.lblPerStopBrakeActuationNIperKG = New System.Windows.Forms.Label()
        Me.lblSmartRegenFractionTotalAirDemand = New System.Windows.Forms.Label()
        Me.txtAdBlueNIperMinute = New System.Windows.Forms.TextBox()
        Me.txtAirControlledSuspensionNIperMinute = New System.Windows.Forms.TextBox()
        Me.txtBrakingNoRetarderNIperKG = New System.Windows.Forms.TextBox()
        Me.txtBrakingWithRetarderNIperKG = New System.Windows.Forms.TextBox()
        Me.txtBreakingPerKneelingNIperKGinMM = New System.Windows.Forms.TextBox()
        Me.txtDeadVolBlowOutsPerLitresperHour = New System.Windows.Forms.TextBox()
        Me.txtDeadVolumeLitres = New System.Windows.Forms.TextBox()
        Me.txtNonSmartRegenFractionTotalAirDemand = New System.Windows.Forms.TextBox()
        Me.txtOverrunUtilisationForCompressionFraction = New System.Windows.Forms.TextBox()
        Me.txtPerDoorOpeningNI = New System.Windows.Forms.TextBox()
        Me.txtPerStopBrakeActuationNIperKG = New System.Windows.Forms.TextBox()
        Me.txtSmartRegenFractionTotalAirDemand = New System.Windows.Forms.TextBox()
        Me.tabHVACConfig = New System.Windows.Forms.TabPage()
        Me.lblHVACTitle = New System.Windows.Forms.Label()
        Me.txtHVACFuellingLitresPerHour = New System.Windows.Forms.TextBox()
        Me.lblHVACFuellingLitresPerHour = New System.Windows.Forms.Label()
        Me.txtHVACMechanicalLoadPowerWatts = New System.Windows.Forms.TextBox()
        Me.lblHVACMechanicalLoadPowerWatts = New System.Windows.Forms.Label()
        Me.txtHVACElectricalLoadPowerWatts = New System.Windows.Forms.TextBox()
        Me.lblHVACElectricalLoadPowerWatts = New System.Windows.Forms.Label()
        Me.tabPlayground = New System.Windows.Forms.TabPage()
        Me.lblM4_AirCompressor = New System.Windows.Forms.Label()
        Me.Panel1 = New System.Windows.Forms.Panel()
        Me.lblM4_PowerAtCrankFromPSCompressorON = New System.Windows.Forms.Label()
        Me.lblM4_PowerAtCrankFromPSCompressorOFF = New System.Windows.Forms.Label()
        Me.lblM4_CompressorOnOffPowerDelta = New System.Windows.Forms.Label()
        Me.lblM4_CompressorFlowRate = New System.Windows.Forms.Label()
        Me.txtM4_out_PowerAtCrankFromPneumaticsCompressorON = New System.Windows.Forms.TextBox()
        Me.txtM4_out_PowerAtCrankFromPneumaticsCompressorOFF = New System.Windows.Forms.TextBox()
        Me.txtM4_out_CompresssorPwrOnMinusPwrOff = New System.Windows.Forms.TextBox()
        Me.txtM4_out_CompressorFlowRate = New System.Windows.Forms.TextBox()
        Me.lblM3_AveragePneumaticLoad = New System.Windows.Forms.Label()
        Me.pnl_M3_Displays = New System.Windows.Forms.Panel()
        Me.lblM3_TotAirConsumptionPerCycleLitres = New System.Windows.Forms.Label()
        Me.lbl_M3_AvgPowerAtCrankFromPneumatics = New System.Windows.Forms.Label()
        Me.txtM3_out_TotalAirConsumedPerCycleInLitres = New System.Windows.Forms.TextBox()
        Me.txtM3_out_AveragePowerAtCrankFromPneumatics = New System.Windows.Forms.TextBox()
        Me.lblM2AverageElectricalLoadTitle = New System.Windows.Forms.Label()
        Me.pnl_M2_Displays = New System.Windows.Forms.Panel()
        Me.lblM2_AvgPwrDmdAtCrankFromElectrics = New System.Windows.Forms.Label()
        Me.lblM2_AveragePowerDemandAtAlternatorFromElectrics = New System.Windows.Forms.Label()
        Me.txtM2_out_AvgPowerAtCrankFromElectrics = New System.Windows.Forms.TextBox()
        Me.txtM2_out_AvgPowerAtAltFromElectrics = New System.Windows.Forms.TextBox()
        Me.lblM1_HVACAverageLoad = New System.Windows.Forms.Label()
        Me.pnl_M1_Displays = New System.Windows.Forms.Panel()
        Me.lblM1_HVACFuelling = New System.Windows.Forms.Label()
        Me.lblM1_AvgPowerDemandAtCrankHVACElectrics = New System.Windows.Forms.Label()
        Me.lblM1_AveragePowerDemandAtAlternatorElectrics = New System.Windows.Forms.Label()
        Me.lblM1_AveragePowerDemandAtCrank = New System.Windows.Forms.Label()
        Me.txtM1_out_HVACFuelling = New System.Windows.Forms.TextBox()
        Me.txtM1_out_AvgPwrAtCrankFromHVACElec = New System.Windows.Forms.TextBox()
        Me.txtM1_out_AvgPowerDemandAtAlternatorHvacElectrics = New System.Windows.Forms.TextBox()
        Me.txtM1_out_AvgPowerDemandAtCrankMech = New System.Windows.Forms.TextBox()
        Me.lblM05SmartalternatorSetEfficiency = New System.Windows.Forms.Label()
        Me.pnl_M05_Displays = New System.Windows.Forms.Panel()
        Me.lblM05_AlternatorsEfficiencyOverrun = New System.Windows.Forms.Label()
        Me.lblM05SmartOverrunCurrent = New System.Windows.Forms.Label()
        Me.lblM05_AlternatorsEfficiencyTraction = New System.Windows.Forms.Label()
        Me.lblM05_SmartTractionCurrent = New System.Windows.Forms.Label()
        Me.lblM05AlternatorsEfficiencyIdle = New System.Windows.Forms.Label()
        Me.lblM05_SmartIdleCurrent = New System.Windows.Forms.Label()
        Me.txtM05_out_AlternatorsEfficiencyOverrun = New System.Windows.Forms.TextBox()
        Me.txtM05_out_SmartOverrunCurrent = New System.Windows.Forms.TextBox()
        Me.txtM05_out_AlternatorsEfficiencyTraction = New System.Windows.Forms.TextBox()
        Me.txtM05_out_SmartTractionCurrent = New System.Windows.Forms.TextBox()
        Me.txtM05_Out_AlternatorsEfficiencyIdle = New System.Windows.Forms.TextBox()
        Me.txtM05_OutSmartIdleCurrent = New System.Windows.Forms.TextBox()
        Me.lblM0Outputs = New System.Windows.Forms.Label()
        Me.pnl_M0_Displays = New System.Windows.Forms.Panel()
        Me.lblOutHVACElectricalCurrentDemand = New System.Windows.Forms.Label()
        Me.lblAlternatorsEfficiency = New System.Windows.Forms.Label()
        Me.txtM0_Out_AlternatorsEfficiency = New System.Windows.Forms.TextBox()
        Me.txtM0_Out_HVacElectricalCurrentDemand = New System.Windows.Forms.TextBox()
        Me.resultCardContextMenu = New System.Windows.Forms.ContextMenuStrip(Me.components)
        Me.DeleteToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ErrorProvider = New System.Windows.Forms.ErrorProvider(Me.components)
        Me.Timer1 = New System.Windows.Forms.Timer(Me.components)
        Me.Panel3 = New System.Windows.Forms.Panel()
        Me.lblM5_SmartAltSetGeneration = New System.Windows.Forms.Label()
        Me.txtM5_out_AltRegenPowerAtCrankIdleWatts = New System.Windows.Forms.TextBox()
        Me.txtM5_out_AltRegenPowerAtCrankTractionWatts = New System.Windows.Forms.TextBox()
        Me.lblM5_AltRegenPowerAtCrankIdleW = New System.Windows.Forms.Label()
        Me.M5_AlternatorGenerationPowerAtCrankOverrunWatts = New System.Windows.Forms.Label()
        Me.txtM5_out_AltRegenPowerAtCrankOverrunWatts = New System.Windows.Forms.TextBox()
        Me.lblM5_AlternatorRegenPowerAtCrankTractionWatts = New System.Windows.Forms.Label()
        Me.txtEngineDrivelinePower = New System.Windows.Forms.TextBox()
        Me.txtEngineDrivelineTorque = New System.Windows.Forms.TextBox()
        Me.txtEngineMotoringPower = New System.Windows.Forms.TextBox()
        Me.txtEngineSpeed = New System.Windows.Forms.TextBox()
        Me.txtTotalCycleTimeSeconds = New System.Windows.Forms.TextBox()
        Me.lblEngineDrivelinePower = New System.Windows.Forms.Label()
        Me.lblEngineDrivelineTorque = New System.Windows.Forms.Label()
        Me.lblEngineMotoringPower = New System.Windows.Forms.Label()
        Me.lblEngineSpeed = New System.Windows.Forms.Label()
        Me.lblTotalCycleTimeSeconds = New System.Windows.Forms.Label()
        Me.chkClutchEngaged = New System.Windows.Forms.CheckBox()
        Me.btnStart = New System.Windows.Forms.Button()
        Me.btnFinish = New System.Windows.Forms.Button()
        Me.PictureBox1 = New System.Windows.Forms.PictureBox()
        Me.pnlMain.SuspendLayout
        Me.tabMain.SuspendLayout
        Me.tabGeneralConfig.SuspendLayout
        Me.tabElectricalConfig.SuspendLayout
        CType(Me.gvResultsCardOverrun,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.gvResultsCardTraction,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.gvResultsCardIdle,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.gvElectricalConsumables,System.ComponentModel.ISupportInitialize).BeginInit
        Me.tabPneumaticConfig.SuspendLayout
        Me.pnlPneumaticsUserInput.SuspendLayout
        Me.pnlPneumaticAuxillaries.SuspendLayout
        Me.tabHVACConfig.SuspendLayout
        Me.tabPlayground.SuspendLayout
        Me.Panel1.SuspendLayout
        Me.pnl_M3_Displays.SuspendLayout
        Me.pnl_M2_Displays.SuspendLayout
        Me.pnl_M1_Displays.SuspendLayout
        Me.pnl_M05_Displays.SuspendLayout
        Me.pnl_M0_Displays.SuspendLayout
        Me.resultCardContextMenu.SuspendLayout
        CType(Me.ErrorProvider,System.ComponentModel.ISupportInitialize).BeginInit
        Me.Panel3.SuspendLayout
        CType(Me.PictureBox1,System.ComponentModel.ISupportInitialize).BeginInit
        Me.SuspendLayout
        '
        'pnlMain
        '
        Me.pnlMain.Controls.Add(Me.btnCancel)
        Me.pnlMain.Controls.Add(Me.btnForward)
        Me.pnlMain.Controls.Add(Me.btnBack)
        Me.pnlMain.Controls.Add(Me.tabMain)
        Me.pnlMain.Dock = System.Windows.Forms.DockStyle.Fill
        Me.pnlMain.Location = New System.Drawing.Point(0, 0)
        Me.pnlMain.Name = "pnlMain"
        Me.pnlMain.Size = New System.Drawing.Size(945, 742)
        Me.pnlMain.TabIndex = 1
        '
        'btnCancel
        '
        Me.btnCancel.Location = New System.Drawing.Point(827, 702)
        Me.btnCancel.Name = "btnCancel"
        Me.btnCancel.Size = New System.Drawing.Size(75, 23)
        Me.btnCancel.TabIndex = 5
        Me.btnCancel.Text = "Cancel"
        Me.btnCancel.UseVisualStyleBackColor = true
        '
        'btnForward
        '
        Me.btnForward.Location = New System.Drawing.Point(621, 703)
        Me.btnForward.Name = "btnForward"
        Me.btnForward.Size = New System.Drawing.Size(75, 23)
        Me.btnForward.TabIndex = 3
        Me.btnForward.Text = "Fwd >>"
        Me.btnForward.UseVisualStyleBackColor = true
        '
        'btnBack
        '
        Me.btnBack.Location = New System.Drawing.Point(537, 703)
        Me.btnBack.Name = "btnBack"
        Me.btnBack.Size = New System.Drawing.Size(75, 23)
        Me.btnBack.TabIndex = 2
        Me.btnBack.Text = "<< Back"
        Me.btnBack.UseVisualStyleBackColor = true
        '
        'tabMain
        '
        Me.tabMain.AccessibleDescription = ""
        Me.tabMain.Controls.Add(Me.tabGeneralConfig)
        Me.tabMain.Controls.Add(Me.tabElectricalConfig)
        Me.tabMain.Controls.Add(Me.tabPneumaticConfig)
        Me.tabMain.Controls.Add(Me.tabHVACConfig)
        Me.tabMain.Controls.Add(Me.tabPlayground)
        Me.tabMain.DrawMode = System.Windows.Forms.TabDrawMode.OwnerDrawFixed
        Me.tabMain.Location = New System.Drawing.Point(12, 12)
        Me.tabMain.Name = "tabMain"
        Me.tabMain.SelectedIndex = 0
        Me.tabMain.Size = New System.Drawing.Size(917, 669)
        Me.tabMain.TabIndex = 0
        Me.tabMain.Tag = ""
        '
        'tabGeneralConfig
        '
        Me.tabGeneralConfig.Controls.Add(Me.cboCycle)
        Me.tabGeneralConfig.Controls.Add(Me.lblCycle)
        Me.tabGeneralConfig.Controls.Add(Me.lblVehiceWeight)
        Me.tabGeneralConfig.Controls.Add(Me.txtVehicleWeightKG)
        Me.tabGeneralConfig.Location = New System.Drawing.Point(4, 22)
        Me.tabGeneralConfig.Name = "tabGeneralConfig"
        Me.tabGeneralConfig.Padding = New System.Windows.Forms.Padding(3)
        Me.tabGeneralConfig.Size = New System.Drawing.Size(909, 632)
        Me.tabGeneralConfig.TabIndex = 0
        Me.tabGeneralConfig.Text = "GeneralConfig"
        Me.tabGeneralConfig.UseVisualStyleBackColor = true
        '
        'cboCycle
        '
        Me.cboCycle.FormattingEnabled = true
        Me.cboCycle.Items.AddRange(New Object() {"Urban", "Heavy urban", "Suburban", "Interurban", "Coach"&Global.Microsoft.VisualBasic.ChrW(9), ""&Global.Microsoft.VisualBasic.ChrW(9)&Global.Microsoft.VisualBasic.ChrW(9)&Global.Microsoft.VisualBasic.ChrW(9)&Global.Microsoft.VisualBasic.ChrW(9)&Global.Microsoft.VisualBasic.ChrW(9)&Global.Microsoft.VisualBasic.ChrW(9)&Global.Microsoft.VisualBasic.ChrW(9)&Global.Microsoft.VisualBasic.ChrW(9)&Global.Microsoft.VisualBasic.ChrW(9)&Global.Microsoft.VisualBasic.ChrW(9)})
        Me.cboCycle.Location = New System.Drawing.Point(123, 78)
        Me.cboCycle.Name = "cboCycle"
        Me.cboCycle.Size = New System.Drawing.Size(121, 21)
        Me.cboCycle.TabIndex = 6
        '
        'lblCycle
        '
        Me.lblCycle.AutoSize = true
        Me.lblCycle.Location = New System.Drawing.Point(24, 78)
        Me.lblCycle.Name = "lblCycle"
        Me.lblCycle.Size = New System.Drawing.Size(33, 13)
        Me.lblCycle.TabIndex = 5
        Me.lblCycle.Text = "Cycle"
        '
        'lblVehiceWeight
        '
        Me.lblVehiceWeight.AutoSize = true
        Me.lblVehiceWeight.Location = New System.Drawing.Point(21, 38)
        Me.lblVehiceWeight.Name = "lblVehiceWeight"
        Me.lblVehiceWeight.Size = New System.Drawing.Size(97, 13)
        Me.lblVehiceWeight.TabIndex = 3
        Me.lblVehiceWeight.Text = "Vehicle Weight KG"
        '
        'txtVehicleWeightKG
        '
        Me.txtVehicleWeightKG.Location = New System.Drawing.Point(123, 35)
        Me.txtVehicleWeightKG.Name = "txtVehicleWeightKG"
        Me.txtVehicleWeightKG.Size = New System.Drawing.Size(100, 20)
        Me.txtVehicleWeightKG.TabIndex = 2
        '
        'tabElectricalConfig
        '
        Me.tabElectricalConfig.Controls.Add(Me.btnAlternatorMapPath)
        Me.tabElectricalConfig.Controls.Add(Me.gvResultsCardOverrun)
        Me.tabElectricalConfig.Controls.Add(Me.gvResultsCardTraction)
        Me.tabElectricalConfig.Controls.Add(Me.gvResultsCardIdle)
        Me.tabElectricalConfig.Controls.Add(Me.lblResultsOverrun)
        Me.tabElectricalConfig.Controls.Add(Me.lblResultsTractionOn)
        Me.tabElectricalConfig.Controls.Add(Me.lblResultsIdle)
        Me.tabElectricalConfig.Controls.Add(Me.chkSmartElectricals)
        Me.tabElectricalConfig.Controls.Add(Me.lblElectricalConsumables)
        Me.tabElectricalConfig.Controls.Add(Me.gvElectricalConsumables)
        Me.tabElectricalConfig.Controls.Add(Me.txtDoorActuationTimeSeconds)
        Me.tabElectricalConfig.Controls.Add(Me.txtAlternatorGearEfficiency)
        Me.tabElectricalConfig.Controls.Add(Me.txtAlternatorMapPath)
        Me.tabElectricalConfig.Controls.Add(Me.txtPowernetVoltage)
        Me.tabElectricalConfig.Controls.Add(Me.lblDoorActuationTimeSeconds)
        Me.tabElectricalConfig.Controls.Add(Me.lblAlternatorGearEfficiency)
        Me.tabElectricalConfig.Controls.Add(Me.lblAlternatormapPath)
        Me.tabElectricalConfig.Controls.Add(Me.lblPowerNetVoltage)
        Me.tabElectricalConfig.Location = New System.Drawing.Point(4, 22)
        Me.tabElectricalConfig.Name = "tabElectricalConfig"
        Me.tabElectricalConfig.Padding = New System.Windows.Forms.Padding(3)
        Me.tabElectricalConfig.Size = New System.Drawing.Size(909, 632)
        Me.tabElectricalConfig.TabIndex = 1
        Me.tabElectricalConfig.Text = "ElectricalConfig"
        Me.tabElectricalConfig.UseVisualStyleBackColor = true
        '
        'btnAlternatorMapPath
        '
        Me.btnAlternatorMapPath.Location = New System.Drawing.Point(496, 43)
        Me.btnAlternatorMapPath.Name = "btnAlternatorMapPath"
        Me.btnAlternatorMapPath.Size = New System.Drawing.Size(38, 20)
        Me.btnAlternatorMapPath.TabIndex = 19
        Me.btnAlternatorMapPath.Text = ". . ."
        Me.btnAlternatorMapPath.UseVisualStyleBackColor = true
        '
        'gvResultsCardOverrun
        '
        DataGridViewCellStyle25.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft
        DataGridViewCellStyle25.BackColor = System.Drawing.SystemColors.Control
        DataGridViewCellStyle25.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0,Byte))
        DataGridViewCellStyle25.ForeColor = System.Drawing.SystemColors.WindowText
        DataGridViewCellStyle25.SelectionBackColor = System.Drawing.SystemColors.Highlight
        DataGridViewCellStyle25.SelectionForeColor = System.Drawing.SystemColors.HighlightText
        DataGridViewCellStyle25.WrapMode = System.Windows.Forms.DataGridViewTriState.[True]
        Me.gvResultsCardOverrun.ColumnHeadersDefaultCellStyle = DataGridViewCellStyle25
        Me.gvResultsCardOverrun.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        DataGridViewCellStyle26.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft
        DataGridViewCellStyle26.BackColor = System.Drawing.SystemColors.Window
        DataGridViewCellStyle26.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0,Byte))
        DataGridViewCellStyle26.ForeColor = System.Drawing.SystemColors.ControlText
        DataGridViewCellStyle26.SelectionBackColor = System.Drawing.SystemColors.Highlight
        DataGridViewCellStyle26.SelectionForeColor = System.Drawing.SystemColors.HighlightText
        DataGridViewCellStyle26.WrapMode = System.Windows.Forms.DataGridViewTriState.[False]
        Me.gvResultsCardOverrun.DefaultCellStyle = DataGridViewCellStyle26
        Me.gvResultsCardOverrun.Location = New System.Drawing.Point(590, 451)
        Me.gvResultsCardOverrun.Name = "gvResultsCardOverrun"
        DataGridViewCellStyle27.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft
        DataGridViewCellStyle27.BackColor = System.Drawing.SystemColors.Control
        DataGridViewCellStyle27.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0,Byte))
        DataGridViewCellStyle27.ForeColor = System.Drawing.SystemColors.WindowText
        DataGridViewCellStyle27.SelectionBackColor = System.Drawing.SystemColors.Highlight
        DataGridViewCellStyle27.SelectionForeColor = System.Drawing.SystemColors.HighlightText
        DataGridViewCellStyle27.WrapMode = System.Windows.Forms.DataGridViewTriState.[True]
        Me.gvResultsCardOverrun.RowHeadersDefaultCellStyle = DataGridViewCellStyle27
        Me.gvResultsCardOverrun.Size = New System.Drawing.Size(246, 125)
        Me.gvResultsCardOverrun.TabIndex = 18
        '
        'gvResultsCardTraction
        '
        DataGridViewCellStyle28.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft
        DataGridViewCellStyle28.BackColor = System.Drawing.SystemColors.Control
        DataGridViewCellStyle28.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0,Byte))
        DataGridViewCellStyle28.ForeColor = System.Drawing.SystemColors.WindowText
        DataGridViewCellStyle28.SelectionBackColor = System.Drawing.SystemColors.Highlight
        DataGridViewCellStyle28.SelectionForeColor = System.Drawing.SystemColors.HighlightText
        DataGridViewCellStyle28.WrapMode = System.Windows.Forms.DataGridViewTriState.[True]
        Me.gvResultsCardTraction.ColumnHeadersDefaultCellStyle = DataGridViewCellStyle28
        Me.gvResultsCardTraction.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        DataGridViewCellStyle29.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft
        DataGridViewCellStyle29.BackColor = System.Drawing.SystemColors.Window
        DataGridViewCellStyle29.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0,Byte))
        DataGridViewCellStyle29.ForeColor = System.Drawing.SystemColors.ControlText
        DataGridViewCellStyle29.SelectionBackColor = System.Drawing.SystemColors.Highlight
        DataGridViewCellStyle29.SelectionForeColor = System.Drawing.SystemColors.HighlightText
        DataGridViewCellStyle29.WrapMode = System.Windows.Forms.DataGridViewTriState.[False]
        Me.gvResultsCardTraction.DefaultCellStyle = DataGridViewCellStyle29
        Me.gvResultsCardTraction.Location = New System.Drawing.Point(311, 451)
        Me.gvResultsCardTraction.Name = "gvResultsCardTraction"
        DataGridViewCellStyle30.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft
        DataGridViewCellStyle30.BackColor = System.Drawing.SystemColors.Control
        DataGridViewCellStyle30.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0,Byte))
        DataGridViewCellStyle30.ForeColor = System.Drawing.SystemColors.WindowText
        DataGridViewCellStyle30.SelectionBackColor = System.Drawing.SystemColors.Highlight
        DataGridViewCellStyle30.SelectionForeColor = System.Drawing.SystemColors.HighlightText
        DataGridViewCellStyle30.WrapMode = System.Windows.Forms.DataGridViewTriState.[True]
        Me.gvResultsCardTraction.RowHeadersDefaultCellStyle = DataGridViewCellStyle30
        Me.gvResultsCardTraction.Size = New System.Drawing.Size(258, 125)
        Me.gvResultsCardTraction.TabIndex = 17
        '
        'gvResultsCardIdle
        '
        DataGridViewCellStyle31.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft
        DataGridViewCellStyle31.BackColor = System.Drawing.SystemColors.Control
        DataGridViewCellStyle31.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0,Byte))
        DataGridViewCellStyle31.ForeColor = System.Drawing.SystemColors.WindowText
        DataGridViewCellStyle31.SelectionBackColor = System.Drawing.SystemColors.Highlight
        DataGridViewCellStyle31.SelectionForeColor = System.Drawing.SystemColors.HighlightText
        DataGridViewCellStyle31.WrapMode = System.Windows.Forms.DataGridViewTriState.[True]
        Me.gvResultsCardIdle.ColumnHeadersDefaultCellStyle = DataGridViewCellStyle31
        Me.gvResultsCardIdle.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        DataGridViewCellStyle32.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft
        DataGridViewCellStyle32.BackColor = System.Drawing.SystemColors.Window
        DataGridViewCellStyle32.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0,Byte))
        DataGridViewCellStyle32.ForeColor = System.Drawing.SystemColors.ControlText
        DataGridViewCellStyle32.SelectionBackColor = System.Drawing.SystemColors.Highlight
        DataGridViewCellStyle32.SelectionForeColor = System.Drawing.SystemColors.HighlightText
        DataGridViewCellStyle32.WrapMode = System.Windows.Forms.DataGridViewTriState.[False]
        Me.gvResultsCardIdle.DefaultCellStyle = DataGridViewCellStyle32
        Me.gvResultsCardIdle.Location = New System.Drawing.Point(35, 451)
        Me.gvResultsCardIdle.Name = "gvResultsCardIdle"
        DataGridViewCellStyle33.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft
        DataGridViewCellStyle33.BackColor = System.Drawing.SystemColors.Control
        DataGridViewCellStyle33.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0,Byte))
        DataGridViewCellStyle33.ForeColor = System.Drawing.SystemColors.WindowText
        DataGridViewCellStyle33.SelectionBackColor = System.Drawing.SystemColors.Highlight
        DataGridViewCellStyle33.SelectionForeColor = System.Drawing.SystemColors.HighlightText
        DataGridViewCellStyle33.WrapMode = System.Windows.Forms.DataGridViewTriState.[True]
        Me.gvResultsCardIdle.RowHeadersDefaultCellStyle = DataGridViewCellStyle33
        Me.gvResultsCardIdle.Size = New System.Drawing.Size(256, 125)
        Me.gvResultsCardIdle.TabIndex = 16
        '
        'lblResultsOverrun
        '
        Me.lblResultsOverrun.AutoSize = true
        Me.lblResultsOverrun.Location = New System.Drawing.Point(587, 435)
        Me.lblResultsOverrun.Name = "lblResultsOverrun"
        Me.lblResultsOverrun.Size = New System.Drawing.Size(83, 13)
        Me.lblResultsOverrun.TabIndex = 15
        Me.lblResultsOverrun.Text = "Results Overrun"
        '
        'lblResultsTractionOn
        '
        Me.lblResultsTractionOn.AutoSize = true
        Me.lblResultsTractionOn.Location = New System.Drawing.Point(308, 435)
        Me.lblResultsTractionOn.Name = "lblResultsTractionOn"
        Me.lblResultsTractionOn.Size = New System.Drawing.Size(98, 13)
        Me.lblResultsTractionOn.TabIndex = 14
        Me.lblResultsTractionOn.Text = "Results TractionOn"
        '
        'lblResultsIdle
        '
        Me.lblResultsIdle.AutoSize = true
        Me.lblResultsIdle.Location = New System.Drawing.Point(38, 435)
        Me.lblResultsIdle.Name = "lblResultsIdle"
        Me.lblResultsIdle.Size = New System.Drawing.Size(62, 13)
        Me.lblResultsIdle.TabIndex = 13
        Me.lblResultsIdle.Text = "Results Idle"
        '
        'chkSmartElectricals
        '
        Me.chkSmartElectricals.AutoSize = true
        Me.chkSmartElectricals.Location = New System.Drawing.Point(161, 132)
        Me.chkSmartElectricals.Name = "chkSmartElectricals"
        Me.chkSmartElectricals.Size = New System.Drawing.Size(101, 17)
        Me.chkSmartElectricals.TabIndex = 12
        Me.chkSmartElectricals.Text = "SmartElectricals"
        Me.chkSmartElectricals.UseVisualStyleBackColor = true
        '
        'lblElectricalConsumables
        '
        Me.lblElectricalConsumables.AutoSize = true
        Me.lblElectricalConsumables.Location = New System.Drawing.Point(34, 154)
        Me.lblElectricalConsumables.Name = "lblElectricalConsumables"
        Me.lblElectricalConsumables.Size = New System.Drawing.Size(116, 13)
        Me.lblElectricalConsumables.TabIndex = 11
        Me.lblElectricalConsumables.Text = "Electrical Consumables"
        '
        'gvElectricalConsumables
        '
        DataGridViewCellStyle34.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft
        DataGridViewCellStyle34.BackColor = System.Drawing.SystemColors.Control
        DataGridViewCellStyle34.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0,Byte))
        DataGridViewCellStyle34.ForeColor = System.Drawing.SystemColors.WindowText
        DataGridViewCellStyle34.SelectionBackColor = System.Drawing.SystemColors.Highlight
        DataGridViewCellStyle34.SelectionForeColor = System.Drawing.SystemColors.HighlightText
        DataGridViewCellStyle34.WrapMode = System.Windows.Forms.DataGridViewTriState.[True]
        Me.gvElectricalConsumables.ColumnHeadersDefaultCellStyle = DataGridViewCellStyle34
        Me.gvElectricalConsumables.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        DataGridViewCellStyle35.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft
        DataGridViewCellStyle35.BackColor = System.Drawing.SystemColors.Window
        DataGridViewCellStyle35.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0,Byte))
        DataGridViewCellStyle35.ForeColor = System.Drawing.SystemColors.ControlText
        DataGridViewCellStyle35.SelectionBackColor = System.Drawing.SystemColors.Highlight
        DataGridViewCellStyle35.SelectionForeColor = System.Drawing.SystemColors.HighlightText
        DataGridViewCellStyle35.WrapMode = System.Windows.Forms.DataGridViewTriState.[False]
        Me.gvElectricalConsumables.DefaultCellStyle = DataGridViewCellStyle35
        Me.gvElectricalConsumables.Location = New System.Drawing.Point(33, 170)
        Me.gvElectricalConsumables.Name = "gvElectricalConsumables"
        DataGridViewCellStyle36.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft
        DataGridViewCellStyle36.BackColor = System.Drawing.SystemColors.Control
        DataGridViewCellStyle36.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0,Byte))
        DataGridViewCellStyle36.ForeColor = System.Drawing.SystemColors.WindowText
        DataGridViewCellStyle36.SelectionBackColor = System.Drawing.SystemColors.Highlight
        DataGridViewCellStyle36.SelectionForeColor = System.Drawing.SystemColors.HighlightText
        DataGridViewCellStyle36.WrapMode = System.Windows.Forms.DataGridViewTriState.[True]
        Me.gvElectricalConsumables.RowHeadersDefaultCellStyle = DataGridViewCellStyle36
        Me.gvElectricalConsumables.Size = New System.Drawing.Size(803, 250)
        Me.gvElectricalConsumables.TabIndex = 10
        '
        'txtDoorActuationTimeSeconds
        '
        Me.txtDoorActuationTimeSeconds.Location = New System.Drawing.Point(161, 101)
        Me.txtDoorActuationTimeSeconds.Name = "txtDoorActuationTimeSeconds"
        Me.txtDoorActuationTimeSeconds.Size = New System.Drawing.Size(100, 20)
        Me.txtDoorActuationTimeSeconds.TabIndex = 9
        '
        'txtAlternatorGearEfficiency
        '
        Me.txtAlternatorGearEfficiency.Location = New System.Drawing.Point(161, 69)
        Me.txtAlternatorGearEfficiency.Name = "txtAlternatorGearEfficiency"
        Me.txtAlternatorGearEfficiency.Size = New System.Drawing.Size(100, 20)
        Me.txtAlternatorGearEfficiency.TabIndex = 6
        '
        'txtAlternatorMapPath
        '
        Me.txtAlternatorMapPath.Location = New System.Drawing.Point(161, 43)
        Me.txtAlternatorMapPath.Name = "txtAlternatorMapPath"
        Me.txtAlternatorMapPath.Size = New System.Drawing.Size(319, 20)
        Me.txtAlternatorMapPath.TabIndex = 4
        '
        'txtPowernetVoltage
        '
        Me.txtPowernetVoltage.Location = New System.Drawing.Point(161, 16)
        Me.txtPowernetVoltage.Name = "txtPowernetVoltage"
        Me.txtPowernetVoltage.Size = New System.Drawing.Size(100, 20)
        Me.txtPowernetVoltage.TabIndex = 2
        '
        'lblDoorActuationTimeSeconds
        '
        Me.lblDoorActuationTimeSeconds.AutoSize = true
        Me.lblDoorActuationTimeSeconds.Location = New System.Drawing.Point(30, 101)
        Me.lblDoorActuationTimeSeconds.Name = "lblDoorActuationTimeSeconds"
        Me.lblDoorActuationTimeSeconds.Size = New System.Drawing.Size(111, 13)
        Me.lblDoorActuationTimeSeconds.TabIndex = 8
        Me.lblDoorActuationTimeSeconds.Text = "DoorActuationTime(S)"
        '
        'lblAlternatorGearEfficiency
        '
        Me.lblAlternatorGearEfficiency.AutoSize = true
        Me.lblAlternatorGearEfficiency.Location = New System.Drawing.Point(30, 73)
        Me.lblAlternatorGearEfficiency.Name = "lblAlternatorGearEfficiency"
        Me.lblAlternatorGearEfficiency.Size = New System.Drawing.Size(121, 13)
        Me.lblAlternatorGearEfficiency.TabIndex = 7
        Me.lblAlternatorGearEfficiency.Text = "AlternatorGearEfficiency"
        '
        'lblAlternatormapPath
        '
        Me.lblAlternatormapPath.AutoSize = true
        Me.lblAlternatormapPath.Location = New System.Drawing.Point(30, 47)
        Me.lblAlternatormapPath.Name = "lblAlternatormapPath"
        Me.lblAlternatormapPath.Size = New System.Drawing.Size(73, 13)
        Me.lblAlternatormapPath.TabIndex = 5
        Me.lblAlternatormapPath.Text = "AlternatorMap"
        '
        'lblPowerNetVoltage
        '
        Me.lblPowerNetVoltage.AutoSize = true
        Me.lblPowerNetVoltage.Location = New System.Drawing.Point(30, 18)
        Me.lblPowerNetVoltage.Name = "lblPowerNetVoltage"
        Me.lblPowerNetVoltage.Size = New System.Drawing.Size(88, 13)
        Me.lblPowerNetVoltage.TabIndex = 3
        Me.lblPowerNetVoltage.Text = "PowernetVoltage"
        '
        'tabPneumaticConfig
        '
        Me.tabPneumaticConfig.Controls.Add(Me.pnlPneumaticsUserInput)
        Me.tabPneumaticConfig.Controls.Add(Me.pnlPneumaticAuxillaries)
        Me.tabPneumaticConfig.Location = New System.Drawing.Point(4, 22)
        Me.tabPneumaticConfig.Name = "tabPneumaticConfig"
        Me.tabPneumaticConfig.Size = New System.Drawing.Size(909, 632)
        Me.tabPneumaticConfig.TabIndex = 2
        Me.tabPneumaticConfig.Text = "PneumaticConfig"
        Me.tabPneumaticConfig.UseVisualStyleBackColor = true
        '
        'pnlPneumaticsUserInput
        '
        Me.pnlPneumaticsUserInput.Controls.Add(Me.Button1)
        Me.pnlPneumaticsUserInput.Controls.Add(Me.btnCompressorMap)
        Me.pnlPneumaticsUserInput.Controls.Add(Me.lblPneumaticsVariablesTitle)
        Me.pnlPneumaticsUserInput.Controls.Add(Me.lblCompressorType)
        Me.pnlPneumaticsUserInput.Controls.Add(Me.lblActuationsMap)
        Me.pnlPneumaticsUserInput.Controls.Add(Me.chkSmartAirCompression)
        Me.pnlPneumaticsUserInput.Controls.Add(Me.lblSmartRegeneration)
        Me.pnlPneumaticsUserInput.Controls.Add(Me.chkSmartRegeneration)
        Me.pnlPneumaticsUserInput.Controls.Add(Me.lblAdBlueDosing)
        Me.pnlPneumaticsUserInput.Controls.Add(Me.chkRetarderBrake)
        Me.pnlPneumaticsUserInput.Controls.Add(Me.lblSmartAirCompression)
        Me.pnlPneumaticsUserInput.Controls.Add(Me.txtKneelingHeightMillimeters)
        Me.pnlPneumaticsUserInput.Controls.Add(Me.lblAirSuspensionControl)
        Me.pnlPneumaticsUserInput.Controls.Add(Me.cboDoors)
        Me.pnlPneumaticsUserInput.Controls.Add(Me.lblRetarderBrake)
        Me.pnlPneumaticsUserInput.Controls.Add(Me.cboCompressorType)
        Me.pnlPneumaticsUserInput.Controls.Add(Me.txtCompressorMap)
        Me.pnlPneumaticsUserInput.Controls.Add(Me.lblCompressorGearEfficiency)
        Me.pnlPneumaticsUserInput.Controls.Add(Me.txtCompressorGearRatio)
        Me.pnlPneumaticsUserInput.Controls.Add(Me.lblCompressorGearRatio)
        Me.pnlPneumaticsUserInput.Controls.Add(Me.txtCompressorGearEfficiency)
        Me.pnlPneumaticsUserInput.Controls.Add(Me.lblCompressorMap)
        Me.pnlPneumaticsUserInput.Controls.Add(Me.cboAirSuspensionControl)
        Me.pnlPneumaticsUserInput.Controls.Add(Me.lblDoors)
        Me.pnlPneumaticsUserInput.Controls.Add(Me.cboAdBlueDosing)
        Me.pnlPneumaticsUserInput.Controls.Add(Me.lblKneelingHeightMillimeters)
        Me.pnlPneumaticsUserInput.Controls.Add(Me.txtActuationsMap)
        Me.pnlPneumaticsUserInput.Location = New System.Drawing.Point(403, 17)
        Me.pnlPneumaticsUserInput.Name = "pnlPneumaticsUserInput"
        Me.pnlPneumaticsUserInput.Size = New System.Drawing.Size(491, 576)
        Me.pnlPneumaticsUserInput.TabIndex = 53
        '
        'Button1
        '
        Me.Button1.Location = New System.Drawing.Point(449, 387)
        Me.Button1.Name = "Button1"
        Me.Button1.Size = New System.Drawing.Size(35, 23)
        Me.Button1.TabIndex = 54
        Me.Button1.Text = ". . ."
        Me.Button1.UseVisualStyleBackColor = true
        '
        'btnCompressorMap
        '
        Me.btnCompressorMap.Location = New System.Drawing.Point(450, 94)
        Me.btnCompressorMap.Name = "btnCompressorMap"
        Me.btnCompressorMap.Size = New System.Drawing.Size(35, 23)
        Me.btnCompressorMap.TabIndex = 53
        Me.btnCompressorMap.Text = ". . ."
        Me.btnCompressorMap.UseVisualStyleBackColor = true
        '
        'lblPneumaticsVariablesTitle
        '
        Me.lblPneumaticsVariablesTitle.AutoSize = true
        Me.lblPneumaticsVariablesTitle.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0,Byte))
        Me.lblPneumaticsVariablesTitle.ForeColor = System.Drawing.SystemColors.MenuHighlight
        Me.lblPneumaticsVariablesTitle.Location = New System.Drawing.Point(20, 18)
        Me.lblPneumaticsVariablesTitle.Name = "lblPneumaticsVariablesTitle"
        Me.lblPneumaticsVariablesTitle.Size = New System.Drawing.Size(122, 13)
        Me.lblPneumaticsVariablesTitle.TabIndex = 52
        Me.lblPneumaticsVariablesTitle.Text = "Pneumatic Variables"
        '
        'lblCompressorType
        '
        Me.lblCompressorType.AutoSize = true
        Me.lblCompressorType.Location = New System.Drawing.Point(13, 60)
        Me.lblCompressorType.Name = "lblCompressorType"
        Me.lblCompressorType.Size = New System.Drawing.Size(92, 13)
        Me.lblCompressorType.TabIndex = 33
        Me.lblCompressorType.Text = "Compressor Type "
        '
        'lblActuationsMap
        '
        Me.lblActuationsMap.AutoSize = true
        Me.lblActuationsMap.Location = New System.Drawing.Point(13, 392)
        Me.lblActuationsMap.Name = "lblActuationsMap"
        Me.lblActuationsMap.Size = New System.Drawing.Size(81, 13)
        Me.lblActuationsMap.TabIndex = 25
        Me.lblActuationsMap.Text = "Actuations Map"
        '
        'chkSmartAirCompression
        '
        Me.chkSmartAirCompression.AutoSize = true
        Me.chkSmartAirCompression.FlatStyle = System.Windows.Forms.FlatStyle.System
        Me.chkSmartAirCompression.Location = New System.Drawing.Point(156, 469)
        Me.chkSmartAirCompression.Name = "chkSmartAirCompression"
        Me.chkSmartAirCompression.Size = New System.Drawing.Size(35, 18)
        Me.chkSmartAirCompression.TabIndex = 48
        Me.chkSmartAirCompression.Text = " "
        Me.chkSmartAirCompression.UseVisualStyleBackColor = true
        '
        'lblSmartRegeneration
        '
        Me.lblSmartRegeneration.AutoSize = true
        Me.lblSmartRegeneration.Location = New System.Drawing.Point(13, 515)
        Me.lblSmartRegeneration.Name = "lblSmartRegeneration"
        Me.lblSmartRegeneration.Size = New System.Drawing.Size(101, 13)
        Me.lblSmartRegeneration.TabIndex = 51
        Me.lblSmartRegeneration.Text = "Smart Regeneration"
        '
        'chkSmartRegeneration
        '
        Me.chkSmartRegeneration.AutoSize = true
        Me.chkSmartRegeneration.FlatStyle = System.Windows.Forms.FlatStyle.System
        Me.chkSmartRegeneration.Location = New System.Drawing.Point(156, 510)
        Me.chkSmartRegeneration.Name = "chkSmartRegeneration"
        Me.chkSmartRegeneration.Size = New System.Drawing.Size(35, 18)
        Me.chkSmartRegeneration.TabIndex = 47
        Me.chkSmartRegeneration.Text = " "
        Me.chkSmartRegeneration.UseVisualStyleBackColor = true
        '
        'lblAdBlueDosing
        '
        Me.lblAdBlueDosing.AutoSize = true
        Me.lblAdBlueDosing.Location = New System.Drawing.Point(13, 228)
        Me.lblAdBlueDosing.Name = "lblAdBlueDosing"
        Me.lblAdBlueDosing.Size = New System.Drawing.Size(77, 13)
        Me.lblAdBlueDosing.TabIndex = 26
        Me.lblAdBlueDosing.Text = "AdBlue Dosing"
        '
        'chkRetarderBrake
        '
        Me.chkRetarderBrake.AutoSize = true
        Me.chkRetarderBrake.FlatStyle = System.Windows.Forms.FlatStyle.System
        Me.chkRetarderBrake.Location = New System.Drawing.Point(156, 428)
        Me.chkRetarderBrake.Name = "chkRetarderBrake"
        Me.chkRetarderBrake.Size = New System.Drawing.Size(38, 18)
        Me.chkRetarderBrake.TabIndex = 45
        Me.chkRetarderBrake.Text = "  "
        Me.chkRetarderBrake.UseVisualStyleBackColor = true
        '
        'lblSmartAirCompression
        '
        Me.lblSmartAirCompression.AutoSize = true
        Me.lblSmartAirCompression.Location = New System.Drawing.Point(13, 474)
        Me.lblSmartAirCompression.Name = "lblSmartAirCompression"
        Me.lblSmartAirCompression.Size = New System.Drawing.Size(112, 13)
        Me.lblSmartAirCompression.TabIndex = 50
        Me.lblSmartAirCompression.Text = "Smart Air Compression"
        '
        'txtKneelingHeightMillimeters
        '
        Me.txtKneelingHeightMillimeters.Location = New System.Drawing.Point(156, 344)
        Me.txtKneelingHeightMillimeters.Name = "txtKneelingHeightMillimeters"
        Me.txtKneelingHeightMillimeters.Size = New System.Drawing.Size(120, 20)
        Me.txtKneelingHeightMillimeters.TabIndex = 44
        '
        'lblAirSuspensionControl
        '
        Me.lblAirSuspensionControl.AutoSize = true
        Me.lblAirSuspensionControl.Location = New System.Drawing.Point(13, 270)
        Me.lblAirSuspensionControl.Name = "lblAirSuspensionControl"
        Me.lblAirSuspensionControl.Size = New System.Drawing.Size(113, 13)
        Me.lblAirSuspensionControl.TabIndex = 29
        Me.lblAirSuspensionControl.Text = "Air Suspension Control"
        '
        'cboDoors
        '
        Me.cboDoors.FormattingEnabled = true
        Me.cboDoors.Items.AddRange(New Object() {"<Select>", "Pneumatic", "Electric"})
        Me.cboDoors.Location = New System.Drawing.Point(156, 303)
        Me.cboDoors.Name = "cboDoors"
        Me.cboDoors.Size = New System.Drawing.Size(121, 21)
        Me.cboDoors.TabIndex = 43
        '
        'lblRetarderBrake
        '
        Me.lblRetarderBrake.AutoSize = true
        Me.lblRetarderBrake.Location = New System.Drawing.Point(13, 433)
        Me.lblRetarderBrake.Name = "lblRetarderBrake"
        Me.lblRetarderBrake.Size = New System.Drawing.Size(79, 13)
        Me.lblRetarderBrake.TabIndex = 49
        Me.lblRetarderBrake.Text = "Retarder Brake"
        '
        'cboCompressorType
        '
        Me.cboCompressorType.FormattingEnabled = true
        Me.cboCompressorType.Items.AddRange(New Object() {"<Select>", "CompressorType1", "CompressorType2", "CompressorType3"})
        Me.cboCompressorType.Location = New System.Drawing.Point(156, 57)
        Me.cboCompressorType.Name = "cboCompressorType"
        Me.cboCompressorType.Size = New System.Drawing.Size(121, 21)
        Me.cboCompressorType.TabIndex = 42
        '
        'txtCompressorMap
        '
        Me.txtCompressorMap.Location = New System.Drawing.Point(156, 94)
        Me.txtCompressorMap.Name = "txtCompressorMap"
        Me.txtCompressorMap.Size = New System.Drawing.Size(275, 20)
        Me.txtCompressorMap.TabIndex = 41
        '
        'lblCompressorGearEfficiency
        '
        Me.lblCompressorGearEfficiency.AutoSize = true
        Me.lblCompressorGearEfficiency.Location = New System.Drawing.Point(13, 185)
        Me.lblCompressorGearEfficiency.Name = "lblCompressorGearEfficiency"
        Me.lblCompressorGearEfficiency.Size = New System.Drawing.Size(137, 13)
        Me.lblCompressorGearEfficiency.TabIndex = 30
        Me.lblCompressorGearEfficiency.Text = "Compressor Gear Efficiency"
        '
        'txtCompressorGearRatio
        '
        Me.txtCompressorGearRatio.ForeColor = System.Drawing.Color.Black
        Me.txtCompressorGearRatio.Location = New System.Drawing.Point(156, 139)
        Me.txtCompressorGearRatio.Name = "txtCompressorGearRatio"
        Me.txtCompressorGearRatio.Size = New System.Drawing.Size(121, 20)
        Me.txtCompressorGearRatio.TabIndex = 40
        '
        'lblCompressorGearRatio
        '
        Me.lblCompressorGearRatio.AutoSize = true
        Me.lblCompressorGearRatio.Location = New System.Drawing.Point(13, 135)
        Me.lblCompressorGearRatio.Name = "lblCompressorGearRatio"
        Me.lblCompressorGearRatio.Size = New System.Drawing.Size(116, 13)
        Me.lblCompressorGearRatio.TabIndex = 31
        Me.lblCompressorGearRatio.Text = "Compressor Gear Ratio"
        '
        'txtCompressorGearEfficiency
        '
        Me.txtCompressorGearEfficiency.Location = New System.Drawing.Point(156, 183)
        Me.txtCompressorGearEfficiency.Name = "txtCompressorGearEfficiency"
        Me.txtCompressorGearEfficiency.Size = New System.Drawing.Size(121, 20)
        Me.txtCompressorGearEfficiency.TabIndex = 39
        '
        'lblCompressorMap
        '
        Me.lblCompressorMap.AutoSize = true
        Me.lblCompressorMap.Location = New System.Drawing.Point(13, 94)
        Me.lblCompressorMap.Name = "lblCompressorMap"
        Me.lblCompressorMap.Size = New System.Drawing.Size(86, 13)
        Me.lblCompressorMap.TabIndex = 32
        Me.lblCompressorMap.Text = "Compressor Map"
        '
        'cboAirSuspensionControl
        '
        Me.cboAirSuspensionControl.FormattingEnabled = true
        Me.cboAirSuspensionControl.Items.AddRange(New Object() {"<Select>", "Mechanically", "Electrically"})
        Me.cboAirSuspensionControl.Location = New System.Drawing.Point(156, 262)
        Me.cboAirSuspensionControl.Name = "cboAirSuspensionControl"
        Me.cboAirSuspensionControl.Size = New System.Drawing.Size(121, 21)
        Me.cboAirSuspensionControl.TabIndex = 28
        '
        'lblDoors
        '
        Me.lblDoors.AutoSize = true
        Me.lblDoors.Location = New System.Drawing.Point(13, 311)
        Me.lblDoors.Name = "lblDoors"
        Me.lblDoors.Size = New System.Drawing.Size(79, 13)
        Me.lblDoors.TabIndex = 34
        Me.lblDoors.Text = "Door Operation"
        '
        'cboAdBlueDosing
        '
        Me.cboAdBlueDosing.FormattingEnabled = true
        Me.cboAdBlueDosing.Items.AddRange(New Object() {"<Select>", "Pneumatic", "Electric"})
        Me.cboAdBlueDosing.Location = New System.Drawing.Point(156, 221)
        Me.cboAdBlueDosing.Name = "cboAdBlueDosing"
        Me.cboAdBlueDosing.Size = New System.Drawing.Size(121, 21)
        Me.cboAdBlueDosing.TabIndex = 27
        '
        'lblKneelingHeightMillimeters
        '
        Me.lblKneelingHeightMillimeters.AutoSize = true
        Me.lblKneelingHeightMillimeters.Location = New System.Drawing.Point(13, 351)
        Me.lblKneelingHeightMillimeters.Name = "lblKneelingHeightMillimeters"
        Me.lblKneelingHeightMillimeters.Size = New System.Drawing.Size(133, 13)
        Me.lblKneelingHeightMillimeters.TabIndex = 35
        Me.lblKneelingHeightMillimeters.Text = "Kneeling Height Millimeters"
        '
        'txtActuationsMap
        '
        Me.txtActuationsMap.Location = New System.Drawing.Point(156, 389)
        Me.txtActuationsMap.Name = "txtActuationsMap"
        Me.txtActuationsMap.Size = New System.Drawing.Size(275, 20)
        Me.txtActuationsMap.TabIndex = 24
        '
        'pnlPneumaticAuxillaries
        '
        Me.pnlPneumaticAuxillaries.Controls.Add(Me.lblPneumaticAuxillariesTitle)
        Me.pnlPneumaticAuxillaries.Controls.Add(Me.lblAdBlueNIperMinute)
        Me.pnlPneumaticAuxillaries.Controls.Add(Me.lblAirControlledSuspensionNIperMinute)
        Me.pnlPneumaticAuxillaries.Controls.Add(Me.lblBrakingNoRetarderNIperKG)
        Me.pnlPneumaticAuxillaries.Controls.Add(Me.lblBrakingWithRetarderNIperKG)
        Me.pnlPneumaticAuxillaries.Controls.Add(Me.lblBreakingPerKneelingNIperKGinMM)
        Me.pnlPneumaticAuxillaries.Controls.Add(Me.lblDeadVolBlowOutsPerLitresperHour)
        Me.pnlPneumaticAuxillaries.Controls.Add(Me.lblDeadVolumeLitres)
        Me.pnlPneumaticAuxillaries.Controls.Add(Me.lblNonSmartRegenFractionTotalAirDemand)
        Me.pnlPneumaticAuxillaries.Controls.Add(Me.lblOverrunUtilisationForCompressionFraction)
        Me.pnlPneumaticAuxillaries.Controls.Add(Me.lblPerDoorOpeningNI)
        Me.pnlPneumaticAuxillaries.Controls.Add(Me.lblPerStopBrakeActuationNIperKG)
        Me.pnlPneumaticAuxillaries.Controls.Add(Me.lblSmartRegenFractionTotalAirDemand)
        Me.pnlPneumaticAuxillaries.Controls.Add(Me.txtAdBlueNIperMinute)
        Me.pnlPneumaticAuxillaries.Controls.Add(Me.txtAirControlledSuspensionNIperMinute)
        Me.pnlPneumaticAuxillaries.Controls.Add(Me.txtBrakingNoRetarderNIperKG)
        Me.pnlPneumaticAuxillaries.Controls.Add(Me.txtBrakingWithRetarderNIperKG)
        Me.pnlPneumaticAuxillaries.Controls.Add(Me.txtBreakingPerKneelingNIperKGinMM)
        Me.pnlPneumaticAuxillaries.Controls.Add(Me.txtDeadVolBlowOutsPerLitresperHour)
        Me.pnlPneumaticAuxillaries.Controls.Add(Me.txtDeadVolumeLitres)
        Me.pnlPneumaticAuxillaries.Controls.Add(Me.txtNonSmartRegenFractionTotalAirDemand)
        Me.pnlPneumaticAuxillaries.Controls.Add(Me.txtOverrunUtilisationForCompressionFraction)
        Me.pnlPneumaticAuxillaries.Controls.Add(Me.txtPerDoorOpeningNI)
        Me.pnlPneumaticAuxillaries.Controls.Add(Me.txtPerStopBrakeActuationNIperKG)
        Me.pnlPneumaticAuxillaries.Controls.Add(Me.txtSmartRegenFractionTotalAirDemand)
        Me.pnlPneumaticAuxillaries.Location = New System.Drawing.Point(20, 17)
        Me.pnlPneumaticAuxillaries.Name = "pnlPneumaticAuxillaries"
        Me.pnlPneumaticAuxillaries.Size = New System.Drawing.Size(363, 576)
        Me.pnlPneumaticAuxillaries.TabIndex = 52
        '
        'lblPneumaticAuxillariesTitle
        '
        Me.lblPneumaticAuxillariesTitle.AutoSize = true
        Me.lblPneumaticAuxillariesTitle.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0,Byte))
        Me.lblPneumaticAuxillariesTitle.ForeColor = System.Drawing.SystemColors.MenuHighlight
        Me.lblPneumaticAuxillariesTitle.Location = New System.Drawing.Point(23, 18)
        Me.lblPneumaticAuxillariesTitle.Name = "lblPneumaticAuxillariesTitle"
        Me.lblPneumaticAuxillariesTitle.Size = New System.Drawing.Size(158, 13)
        Me.lblPneumaticAuxillariesTitle.TabIndex = 24
        Me.lblPneumaticAuxillariesTitle.Text = "Pneumatic Auxillaries Data"
        '
        'lblAdBlueNIperMinute
        '
        Me.lblAdBlueNIperMinute.AutoSize = true
        Me.lblAdBlueNIperMinute.Location = New System.Drawing.Point(20, 60)
        Me.lblAdBlueNIperMinute.MinimumSize = New System.Drawing.Size(120, 0)
        Me.lblAdBlueNIperMinute.Name = "lblAdBlueNIperMinute"
        Me.lblAdBlueNIperMinute.Size = New System.Drawing.Size(120, 13)
        Me.lblAdBlueNIperMinute.TabIndex = 12
        Me.lblAdBlueNIperMinute.Text = "AdBlue NI per Minute "
        '
        'lblAirControlledSuspensionNIperMinute
        '
        Me.lblAirControlledSuspensionNIperMinute.AutoSize = true
        Me.lblAirControlledSuspensionNIperMinute.Location = New System.Drawing.Point(20, 99)
        Me.lblAirControlledSuspensionNIperMinute.MinimumSize = New System.Drawing.Size(120, 0)
        Me.lblAirControlledSuspensionNIperMinute.Name = "lblAirControlledSuspensionNIperMinute"
        Me.lblAirControlledSuspensionNIperMinute.Size = New System.Drawing.Size(184, 13)
        Me.lblAirControlledSuspensionNIperMinute.TabIndex = 13
        Me.lblAirControlledSuspensionNIperMinute.Text = "Air Controlled Suspension NI / Minute"
        '
        'lblBrakingNoRetarderNIperKG
        '
        Me.lblBrakingNoRetarderNIperKG.AutoSize = true
        Me.lblBrakingNoRetarderNIperKG.Location = New System.Drawing.Point(20, 140)
        Me.lblBrakingNoRetarderNIperKG.MinimumSize = New System.Drawing.Size(120, 0)
        Me.lblBrakingNoRetarderNIperKG.Name = "lblBrakingNoRetarderNIperKG"
        Me.lblBrakingNoRetarderNIperKG.Size = New System.Drawing.Size(138, 13)
        Me.lblBrakingNoRetarderNIperKG.TabIndex = 14
        Me.lblBrakingNoRetarderNIperKG.Text = "Braking No Retarder NI/KG"
        '
        'lblBrakingWithRetarderNIperKG
        '
        Me.lblBrakingWithRetarderNIperKG.AutoSize = true
        Me.lblBrakingWithRetarderNIperKG.Location = New System.Drawing.Point(20, 183)
        Me.lblBrakingWithRetarderNIperKG.MinimumSize = New System.Drawing.Size(120, 0)
        Me.lblBrakingWithRetarderNIperKG.Name = "lblBrakingWithRetarderNIperKG"
        Me.lblBrakingWithRetarderNIperKG.Size = New System.Drawing.Size(146, 13)
        Me.lblBrakingWithRetarderNIperKG.TabIndex = 15
        Me.lblBrakingWithRetarderNIperKG.Text = "Braking With Retarder NI/KG"
        '
        'lblBreakingPerKneelingNIperKGinMM
        '
        Me.lblBreakingPerKneelingNIperKGinMM.AutoSize = true
        Me.lblBreakingPerKneelingNIperKGinMM.Location = New System.Drawing.Point(20, 222)
        Me.lblBreakingPerKneelingNIperKGinMM.MinimumSize = New System.Drawing.Size(120, 0)
        Me.lblBreakingPerKneelingNIperKGinMM.Name = "lblBreakingPerKneelingNIperKGinMM"
        Me.lblBreakingPerKneelingNIperKGinMM.Size = New System.Drawing.Size(178, 13)
        Me.lblBreakingPerKneelingNIperKGinMM.TabIndex = 16
        Me.lblBreakingPerKneelingNIperKGinMM.Text = "Breaking Per Kneeling NI/KG in MM"
        '
        'lblDeadVolBlowOutsPerLitresperHour
        '
        Me.lblDeadVolBlowOutsPerLitresperHour.AutoSize = true
        Me.lblDeadVolBlowOutsPerLitresperHour.Location = New System.Drawing.Point(20, 263)
        Me.lblDeadVolBlowOutsPerLitresperHour.MinimumSize = New System.Drawing.Size(120, 0)
        Me.lblDeadVolBlowOutsPerLitresperHour.Name = "lblDeadVolBlowOutsPerLitresperHour"
        Me.lblDeadVolBlowOutsPerLitresperHour.Size = New System.Drawing.Size(148, 13)
        Me.lblDeadVolBlowOutsPerLitresperHour.TabIndex = 17
        Me.lblDeadVolBlowOutsPerLitresperHour.Text = "Dead Vol Blowouts /L / Hour "
        '
        'lblDeadVolumeLitres
        '
        Me.lblDeadVolumeLitres.AutoSize = true
        Me.lblDeadVolumeLitres.Location = New System.Drawing.Point(20, 303)
        Me.lblDeadVolumeLitres.MinimumSize = New System.Drawing.Size(120, 0)
        Me.lblDeadVolumeLitres.Name = "lblDeadVolumeLitres"
        Me.lblDeadVolumeLitres.Size = New System.Drawing.Size(120, 13)
        Me.lblDeadVolumeLitres.TabIndex = 18
        Me.lblDeadVolumeLitres.Text = "Dead Volume Litres"
        '
        'lblNonSmartRegenFractionTotalAirDemand
        '
        Me.lblNonSmartRegenFractionTotalAirDemand.AutoSize = true
        Me.lblNonSmartRegenFractionTotalAirDemand.Location = New System.Drawing.Point(20, 346)
        Me.lblNonSmartRegenFractionTotalAirDemand.MinimumSize = New System.Drawing.Size(120, 0)
        Me.lblNonSmartRegenFractionTotalAirDemand.Name = "lblNonSmartRegenFractionTotalAirDemand"
        Me.lblNonSmartRegenFractionTotalAirDemand.Size = New System.Drawing.Size(218, 13)
        Me.lblNonSmartRegenFractionTotalAirDemand.TabIndex = 19
        Me.lblNonSmartRegenFractionTotalAirDemand.Text = "Non Smart Regen Fraction Total Air Demand"
        '
        'lblOverrunUtilisationForCompressionFraction
        '
        Me.lblOverrunUtilisationForCompressionFraction.AutoSize = true
        Me.lblOverrunUtilisationForCompressionFraction.Location = New System.Drawing.Point(20, 388)
        Me.lblOverrunUtilisationForCompressionFraction.MinimumSize = New System.Drawing.Size(120, 0)
        Me.lblOverrunUtilisationForCompressionFraction.Name = "lblOverrunUtilisationForCompressionFraction"
        Me.lblOverrunUtilisationForCompressionFraction.Size = New System.Drawing.Size(215, 13)
        Me.lblOverrunUtilisationForCompressionFraction.TabIndex = 20
        Me.lblOverrunUtilisationForCompressionFraction.Text = "Overrun Utilisation For Compression Fraction"
        '
        'lblPerDoorOpeningNI
        '
        Me.lblPerDoorOpeningNI.AutoSize = true
        Me.lblPerDoorOpeningNI.Location = New System.Drawing.Point(20, 427)
        Me.lblPerDoorOpeningNI.MinimumSize = New System.Drawing.Size(120, 0)
        Me.lblPerDoorOpeningNI.Name = "lblPerDoorOpeningNI"
        Me.lblPerDoorOpeningNI.Size = New System.Drawing.Size(120, 13)
        Me.lblPerDoorOpeningNI.TabIndex = 21
        Me.lblPerDoorOpeningNI.Text = "Per Door Opening NI"
        '
        'lblPerStopBrakeActuationNIperKG
        '
        Me.lblPerStopBrakeActuationNIperKG.AutoSize = true
        Me.lblPerStopBrakeActuationNIperKG.Location = New System.Drawing.Point(20, 469)
        Me.lblPerStopBrakeActuationNIperKG.MinimumSize = New System.Drawing.Size(120, 0)
        Me.lblPerStopBrakeActuationNIperKG.Name = "lblPerStopBrakeActuationNIperKG"
        Me.lblPerStopBrakeActuationNIperKG.Size = New System.Drawing.Size(161, 13)
        Me.lblPerStopBrakeActuationNIperKG.TabIndex = 22
        Me.lblPerStopBrakeActuationNIperKG.Text = "Per Stop Brake Actuation NI/KG"
        '
        'lblSmartRegenFractionTotalAirDemand
        '
        Me.lblSmartRegenFractionTotalAirDemand.AutoSize = true
        Me.lblSmartRegenFractionTotalAirDemand.Location = New System.Drawing.Point(20, 510)
        Me.lblSmartRegenFractionTotalAirDemand.MinimumSize = New System.Drawing.Size(120, 0)
        Me.lblSmartRegenFractionTotalAirDemand.Name = "lblSmartRegenFractionTotalAirDemand"
        Me.lblSmartRegenFractionTotalAirDemand.Size = New System.Drawing.Size(195, 13)
        Me.lblSmartRegenFractionTotalAirDemand.TabIndex = 23
        Me.lblSmartRegenFractionTotalAirDemand.Text = "Smart Regen Fraction Total Air Demand"
        '
        'txtAdBlueNIperMinute
        '
        Me.txtAdBlueNIperMinute.Location = New System.Drawing.Point(242, 57)
        Me.txtAdBlueNIperMinute.Name = "txtAdBlueNIperMinute"
        Me.txtAdBlueNIperMinute.Size = New System.Drawing.Size(100, 20)
        Me.txtAdBlueNIperMinute.TabIndex = 0
        '
        'txtAirControlledSuspensionNIperMinute
        '
        Me.txtAirControlledSuspensionNIperMinute.Location = New System.Drawing.Point(242, 98)
        Me.txtAirControlledSuspensionNIperMinute.Name = "txtAirControlledSuspensionNIperMinute"
        Me.txtAirControlledSuspensionNIperMinute.Size = New System.Drawing.Size(100, 20)
        Me.txtAirControlledSuspensionNIperMinute.TabIndex = 1
        '
        'txtBrakingNoRetarderNIperKG
        '
        Me.txtBrakingNoRetarderNIperKG.Location = New System.Drawing.Point(242, 139)
        Me.txtBrakingNoRetarderNIperKG.Name = "txtBrakingNoRetarderNIperKG"
        Me.txtBrakingNoRetarderNIperKG.Size = New System.Drawing.Size(100, 20)
        Me.txtBrakingNoRetarderNIperKG.TabIndex = 2
        '
        'txtBrakingWithRetarderNIperKG
        '
        Me.txtBrakingWithRetarderNIperKG.Location = New System.Drawing.Point(242, 183)
        Me.txtBrakingWithRetarderNIperKG.Name = "txtBrakingWithRetarderNIperKG"
        Me.txtBrakingWithRetarderNIperKG.Size = New System.Drawing.Size(100, 20)
        Me.txtBrakingWithRetarderNIperKG.TabIndex = 3
        '
        'txtBreakingPerKneelingNIperKGinMM
        '
        Me.txtBreakingPerKneelingNIperKGinMM.Location = New System.Drawing.Point(242, 221)
        Me.txtBreakingPerKneelingNIperKGinMM.Name = "txtBreakingPerKneelingNIperKGinMM"
        Me.txtBreakingPerKneelingNIperKGinMM.Size = New System.Drawing.Size(100, 20)
        Me.txtBreakingPerKneelingNIperKGinMM.TabIndex = 4
        '
        'txtDeadVolBlowOutsPerLitresperHour
        '
        Me.txtDeadVolBlowOutsPerLitresperHour.Location = New System.Drawing.Point(242, 262)
        Me.txtDeadVolBlowOutsPerLitresperHour.Name = "txtDeadVolBlowOutsPerLitresperHour"
        Me.txtDeadVolBlowOutsPerLitresperHour.Size = New System.Drawing.Size(100, 20)
        Me.txtDeadVolBlowOutsPerLitresperHour.TabIndex = 5
        '
        'txtDeadVolumeLitres
        '
        Me.txtDeadVolumeLitres.Location = New System.Drawing.Point(242, 303)
        Me.txtDeadVolumeLitres.Name = "txtDeadVolumeLitres"
        Me.txtDeadVolumeLitres.Size = New System.Drawing.Size(100, 20)
        Me.txtDeadVolumeLitres.TabIndex = 6
        '
        'txtNonSmartRegenFractionTotalAirDemand
        '
        Me.txtNonSmartRegenFractionTotalAirDemand.Location = New System.Drawing.Point(242, 344)
        Me.txtNonSmartRegenFractionTotalAirDemand.Name = "txtNonSmartRegenFractionTotalAirDemand"
        Me.txtNonSmartRegenFractionTotalAirDemand.Size = New System.Drawing.Size(100, 20)
        Me.txtNonSmartRegenFractionTotalAirDemand.TabIndex = 7
        '
        'txtOverrunUtilisationForCompressionFraction
        '
        Me.txtOverrunUtilisationForCompressionFraction.Location = New System.Drawing.Point(242, 385)
        Me.txtOverrunUtilisationForCompressionFraction.Name = "txtOverrunUtilisationForCompressionFraction"
        Me.txtOverrunUtilisationForCompressionFraction.Size = New System.Drawing.Size(100, 20)
        Me.txtOverrunUtilisationForCompressionFraction.TabIndex = 8
        '
        'txtPerDoorOpeningNI
        '
        Me.txtPerDoorOpeningNI.Location = New System.Drawing.Point(242, 426)
        Me.txtPerDoorOpeningNI.Name = "txtPerDoorOpeningNI"
        Me.txtPerDoorOpeningNI.Size = New System.Drawing.Size(100, 20)
        Me.txtPerDoorOpeningNI.TabIndex = 9
        '
        'txtPerStopBrakeActuationNIperKG
        '
        Me.txtPerStopBrakeActuationNIperKG.Location = New System.Drawing.Point(242, 467)
        Me.txtPerStopBrakeActuationNIperKG.Name = "txtPerStopBrakeActuationNIperKG"
        Me.txtPerStopBrakeActuationNIperKG.Size = New System.Drawing.Size(100, 20)
        Me.txtPerStopBrakeActuationNIperKG.TabIndex = 10
        '
        'txtSmartRegenFractionTotalAirDemand
        '
        Me.txtSmartRegenFractionTotalAirDemand.Location = New System.Drawing.Point(242, 508)
        Me.txtSmartRegenFractionTotalAirDemand.Name = "txtSmartRegenFractionTotalAirDemand"
        Me.txtSmartRegenFractionTotalAirDemand.Size = New System.Drawing.Size(100, 20)
        Me.txtSmartRegenFractionTotalAirDemand.TabIndex = 11
        '
        'tabHVACConfig
        '
        Me.tabHVACConfig.Controls.Add(Me.lblHVACTitle)
        Me.tabHVACConfig.Controls.Add(Me.txtHVACFuellingLitresPerHour)
        Me.tabHVACConfig.Controls.Add(Me.lblHVACFuellingLitresPerHour)
        Me.tabHVACConfig.Controls.Add(Me.txtHVACMechanicalLoadPowerWatts)
        Me.tabHVACConfig.Controls.Add(Me.lblHVACMechanicalLoadPowerWatts)
        Me.tabHVACConfig.Controls.Add(Me.txtHVACElectricalLoadPowerWatts)
        Me.tabHVACConfig.Controls.Add(Me.lblHVACElectricalLoadPowerWatts)
        Me.tabHVACConfig.Location = New System.Drawing.Point(4, 22)
        Me.tabHVACConfig.Name = "tabHVACConfig"
        Me.tabHVACConfig.Size = New System.Drawing.Size(909, 632)
        Me.tabHVACConfig.TabIndex = 3
        Me.tabHVACConfig.Text = "HVACConfig"
        Me.tabHVACConfig.UseVisualStyleBackColor = true
        '
        'lblHVACTitle
        '
        Me.lblHVACTitle.AutoSize = true
        Me.lblHVACTitle.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0,Byte))
        Me.lblHVACTitle.ForeColor = System.Drawing.SystemColors.MenuHighlight
        Me.lblHVACTitle.Location = New System.Drawing.Point(31, 37)
        Me.lblHVACTitle.Name = "lblHVACTitle"
        Me.lblHVACTitle.Size = New System.Drawing.Size(164, 13)
        Me.lblHVACTitle.TabIndex = 25
        Me.lblHVACTitle.Text = "Steady State Output Values"
        '
        'txtHVACFuellingLitresPerHour
        '
        Me.txtHVACFuellingLitresPerHour.Location = New System.Drawing.Point(192, 189)
        Me.txtHVACFuellingLitresPerHour.Name = "txtHVACFuellingLitresPerHour"
        Me.txtHVACFuellingLitresPerHour.Size = New System.Drawing.Size(100, 20)
        Me.txtHVACFuellingLitresPerHour.TabIndex = 5
        '
        'lblHVACFuellingLitresPerHour
        '
        Me.lblHVACFuellingLitresPerHour.AutoSize = true
        Me.lblHVACFuellingLitresPerHour.Location = New System.Drawing.Point(31, 193)
        Me.lblHVACFuellingLitresPerHour.Name = "lblHVACFuellingLitresPerHour"
        Me.lblHVACFuellingLitresPerHour.Size = New System.Drawing.Size(116, 13)
        Me.lblHVACFuellingLitresPerHour.TabIndex = 4
        Me.lblHVACFuellingLitresPerHour.Text = "Fuelling Litres Per Hour"
        '
        'txtHVACMechanicalLoadPowerWatts
        '
        Me.txtHVACMechanicalLoadPowerWatts.Location = New System.Drawing.Point(192, 137)
        Me.txtHVACMechanicalLoadPowerWatts.Name = "txtHVACMechanicalLoadPowerWatts"
        Me.txtHVACMechanicalLoadPowerWatts.Size = New System.Drawing.Size(100, 20)
        Me.txtHVACMechanicalLoadPowerWatts.TabIndex = 3
        '
        'lblHVACMechanicalLoadPowerWatts
        '
        Me.lblHVACMechanicalLoadPowerWatts.AutoSize = true
        Me.lblHVACMechanicalLoadPowerWatts.Location = New System.Drawing.Point(31, 141)
        Me.lblHVACMechanicalLoadPowerWatts.Name = "lblHVACMechanicalLoadPowerWatts"
        Me.lblHVACMechanicalLoadPowerWatts.Size = New System.Drawing.Size(153, 13)
        Me.lblHVACMechanicalLoadPowerWatts.TabIndex = 2
        Me.lblHVACMechanicalLoadPowerWatts.Text = "Mechanical Load Power Watts"
        '
        'txtHVACElectricalLoadPowerWatts
        '
        Me.txtHVACElectricalLoadPowerWatts.Location = New System.Drawing.Point(192, 85)
        Me.txtHVACElectricalLoadPowerWatts.Name = "txtHVACElectricalLoadPowerWatts"
        Me.txtHVACElectricalLoadPowerWatts.Size = New System.Drawing.Size(100, 20)
        Me.txtHVACElectricalLoadPowerWatts.TabIndex = 1
        '
        'lblHVACElectricalLoadPowerWatts
        '
        Me.lblHVACElectricalLoadPowerWatts.AutoSize = true
        Me.lblHVACElectricalLoadPowerWatts.Location = New System.Drawing.Point(31, 89)
        Me.lblHVACElectricalLoadPowerWatts.Name = "lblHVACElectricalLoadPowerWatts"
        Me.lblHVACElectricalLoadPowerWatts.Size = New System.Drawing.Size(141, 13)
        Me.lblHVACElectricalLoadPowerWatts.TabIndex = 0
        Me.lblHVACElectricalLoadPowerWatts.Text = "Electrical Load Power Watts"
        '
        'tabPlayground
        '
        Me.tabPlayground.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.tabPlayground.Controls.Add(Me.lblM5_SmartAltSetGeneration)
        Me.tabPlayground.Controls.Add(Me.Panel3)
        Me.tabPlayground.Controls.Add(Me.PictureBox1)
        Me.tabPlayground.Controls.Add(Me.lblM4_AirCompressor)
        Me.tabPlayground.Controls.Add(Me.btnFinish)
        Me.tabPlayground.Controls.Add(Me.Panel1)
        Me.tabPlayground.Controls.Add(Me.lblM3_AveragePneumaticLoad)
        Me.tabPlayground.Controls.Add(Me.pnl_M3_Displays)
        Me.tabPlayground.Controls.Add(Me.btnStart)
        Me.tabPlayground.Controls.Add(Me.lblM2AverageElectricalLoadTitle)
        Me.tabPlayground.Controls.Add(Me.pnl_M2_Displays)
        Me.tabPlayground.Controls.Add(Me.lblM1_HVACAverageLoad)
        Me.tabPlayground.Controls.Add(Me.pnl_M1_Displays)
        Me.tabPlayground.Controls.Add(Me.lblM05SmartalternatorSetEfficiency)
        Me.tabPlayground.Controls.Add(Me.pnl_M05_Displays)
        Me.tabPlayground.Controls.Add(Me.lblM0Outputs)
        Me.tabPlayground.Controls.Add(Me.pnl_M0_Displays)
        Me.tabPlayground.Controls.Add(Me.chkClutchEngaged)
        Me.tabPlayground.Controls.Add(Me.lblTotalCycleTimeSeconds)
        Me.tabPlayground.Controls.Add(Me.lblEngineSpeed)
        Me.tabPlayground.Controls.Add(Me.lblEngineMotoringPower)
        Me.tabPlayground.Controls.Add(Me.lblEngineDrivelineTorque)
        Me.tabPlayground.Controls.Add(Me.lblEngineDrivelinePower)
        Me.tabPlayground.Controls.Add(Me.txtTotalCycleTimeSeconds)
        Me.tabPlayground.Controls.Add(Me.txtEngineSpeed)
        Me.tabPlayground.Controls.Add(Me.txtEngineMotoringPower)
        Me.tabPlayground.Controls.Add(Me.txtEngineDrivelineTorque)
        Me.tabPlayground.Controls.Add(Me.txtEngineDrivelinePower)
        Me.tabPlayground.Location = New System.Drawing.Point(4, 22)
        Me.tabPlayground.Name = "tabPlayground"
        Me.tabPlayground.Size = New System.Drawing.Size(909, 643)
        Me.tabPlayground.TabIndex = 4
        Me.tabPlayground.Text = "Playground"
        Me.tabPlayground.UseVisualStyleBackColor = true
        '
        'lblM4_AirCompressor
        '
        Me.lblM4_AirCompressor.AutoSize = true
        Me.lblM4_AirCompressor.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0,Byte))
        Me.lblM4_AirCompressor.Location = New System.Drawing.Point(379, 265)
        Me.lblM4_AirCompressor.Name = "lblM4_AirCompressor"
        Me.lblM4_AirCompressor.Size = New System.Drawing.Size(112, 13)
        Me.lblM4_AirCompressor.TabIndex = 33
        Me.lblM4_AirCompressor.Text = "M4-Air Compressor"
        '
        'Panel1
        '
        Me.Panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.Panel1.Controls.Add(Me.lblM4_PowerAtCrankFromPSCompressorON)
        Me.Panel1.Controls.Add(Me.lblM4_PowerAtCrankFromPSCompressorOFF)
        Me.Panel1.Controls.Add(Me.lblM4_CompressorOnOffPowerDelta)
        Me.Panel1.Controls.Add(Me.lblM4_CompressorFlowRate)
        Me.Panel1.Controls.Add(Me.txtM4_out_PowerAtCrankFromPneumaticsCompressorON)
        Me.Panel1.Controls.Add(Me.txtM4_out_PowerAtCrankFromPneumaticsCompressorOFF)
        Me.Panel1.Controls.Add(Me.txtM4_out_CompresssorPwrOnMinusPwrOff)
        Me.Panel1.Controls.Add(Me.txtM4_out_CompressorFlowRate)
        Me.Panel1.Location = New System.Drawing.Point(379, 281)
        Me.Panel1.Name = "Panel1"
        Me.Panel1.Size = New System.Drawing.Size(200, 183)
        Me.Panel1.TabIndex = 32
        '
        'lblM4_PowerAtCrankFromPSCompressorON
        '
        Me.lblM4_PowerAtCrankFromPSCompressorON.AutoSize = true
        Me.lblM4_PowerAtCrankFromPSCompressorON.Location = New System.Drawing.Point(13, 129)
        Me.lblM4_PowerAtCrankFromPSCompressorON.Name = "lblM4_PowerAtCrankFromPSCompressorON"
        Me.lblM4_PowerAtCrankFromPSCompressorON.Size = New System.Drawing.Size(181, 13)
        Me.lblM4_PowerAtCrankFromPSCompressorON.TabIndex = 7
        Me.lblM4_PowerAtCrankFromPSCompressorON.Text = "Pwr@Crank From PS CompressorON"
        '
        'lblM4_PowerAtCrankFromPSCompressorOFF
        '
        Me.lblM4_PowerAtCrankFromPSCompressorOFF.AutoSize = true
        Me.lblM4_PowerAtCrankFromPSCompressorOFF.Location = New System.Drawing.Point(13, 87)
        Me.lblM4_PowerAtCrankFromPSCompressorOFF.Name = "lblM4_PowerAtCrankFromPSCompressorOFF"
        Me.lblM4_PowerAtCrankFromPSCompressorOFF.Size = New System.Drawing.Size(185, 13)
        Me.lblM4_PowerAtCrankFromPSCompressorOFF.TabIndex = 6
        Me.lblM4_PowerAtCrankFromPSCompressorOFF.Text = "Pwr@Crank From PS CompressorOFF"
        '
        'lblM4_CompressorOnOffPowerDelta
        '
        Me.lblM4_CompressorOnOffPowerDelta.AutoSize = true
        Me.lblM4_CompressorOnOffPowerDelta.Location = New System.Drawing.Point(13, 50)
        Me.lblM4_CompressorOnOffPowerDelta.Name = "lblM4_CompressorOnOffPowerDelta"
        Me.lblM4_CompressorOnOffPowerDelta.Size = New System.Drawing.Size(162, 13)
        Me.lblM4_CompressorOnOffPowerDelta.TabIndex = 5
        Me.lblM4_CompressorOnOffPowerDelta.Text = "Compressor PwrOn minus PwrOff"
        '
        'lblM4_CompressorFlowRate
        '
        Me.lblM4_CompressorFlowRate.AutoSize = true
        Me.lblM4_CompressorFlowRate.Location = New System.Drawing.Point(13, 7)
        Me.lblM4_CompressorFlowRate.Name = "lblM4_CompressorFlowRate"
        Me.lblM4_CompressorFlowRate.Size = New System.Drawing.Size(143, 13)
        Me.lblM4_CompressorFlowRate.TabIndex = 4
        Me.lblM4_CompressorFlowRate.Text = "Compressor Flow Rate L/min"
        '
        'txtM4_out_PowerAtCrankFromPneumaticsCompressorON
        '
        Me.txtM4_out_PowerAtCrankFromPneumaticsCompressorON.ForeColor = System.Drawing.Color.FromArgb(CType(CType(0,Byte),Integer), CType(CType(192,Byte),Integer), CType(CType(0,Byte),Integer))
        Me.txtM4_out_PowerAtCrankFromPneumaticsCompressorON.Location = New System.Drawing.Point(13, 144)
        Me.txtM4_out_PowerAtCrankFromPneumaticsCompressorON.Name = "txtM4_out_PowerAtCrankFromPneumaticsCompressorON"
        Me.txtM4_out_PowerAtCrankFromPneumaticsCompressorON.Size = New System.Drawing.Size(100, 20)
        Me.txtM4_out_PowerAtCrankFromPneumaticsCompressorON.TabIndex = 3
        '
        'txtM4_out_PowerAtCrankFromPneumaticsCompressorOFF
        '
        Me.txtM4_out_PowerAtCrankFromPneumaticsCompressorOFF.ForeColor = System.Drawing.Color.FromArgb(CType(CType(0,Byte),Integer), CType(CType(192,Byte),Integer), CType(CType(0,Byte),Integer))
        Me.txtM4_out_PowerAtCrankFromPneumaticsCompressorOFF.Location = New System.Drawing.Point(13, 103)
        Me.txtM4_out_PowerAtCrankFromPneumaticsCompressorOFF.Name = "txtM4_out_PowerAtCrankFromPneumaticsCompressorOFF"
        Me.txtM4_out_PowerAtCrankFromPneumaticsCompressorOFF.Size = New System.Drawing.Size(100, 20)
        Me.txtM4_out_PowerAtCrankFromPneumaticsCompressorOFF.TabIndex = 2
        '
        'txtM4_out_CompresssorPwrOnMinusPwrOff
        '
        Me.txtM4_out_CompresssorPwrOnMinusPwrOff.ForeColor = System.Drawing.Color.FromArgb(CType(CType(0,Byte),Integer), CType(CType(192,Byte),Integer), CType(CType(0,Byte),Integer))
        Me.txtM4_out_CompresssorPwrOnMinusPwrOff.Location = New System.Drawing.Point(13, 65)
        Me.txtM4_out_CompresssorPwrOnMinusPwrOff.Name = "txtM4_out_CompresssorPwrOnMinusPwrOff"
        Me.txtM4_out_CompresssorPwrOnMinusPwrOff.Size = New System.Drawing.Size(100, 20)
        Me.txtM4_out_CompresssorPwrOnMinusPwrOff.TabIndex = 1
        '
        'txtM4_out_CompressorFlowRate
        '
        Me.txtM4_out_CompressorFlowRate.ForeColor = System.Drawing.Color.FromArgb(CType(CType(0,Byte),Integer), CType(CType(192,Byte),Integer), CType(CType(0,Byte),Integer))
        Me.txtM4_out_CompressorFlowRate.Location = New System.Drawing.Point(13, 24)
        Me.txtM4_out_CompressorFlowRate.Name = "txtM4_out_CompressorFlowRate"
        Me.txtM4_out_CompressorFlowRate.Size = New System.Drawing.Size(100, 20)
        Me.txtM4_out_CompressorFlowRate.TabIndex = 0
        '
        'lblM3_AveragePneumaticLoad
        '
        Me.lblM3_AveragePneumaticLoad.AutoSize = true
        Me.lblM3_AveragePneumaticLoad.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0,Byte))
        Me.lblM3_AveragePneumaticLoad.Location = New System.Drawing.Point(376, 141)
        Me.lblM3_AveragePneumaticLoad.Name = "lblM3_AveragePneumaticLoad"
        Me.lblM3_AveragePneumaticLoad.Size = New System.Drawing.Size(195, 13)
        Me.lblM3_AveragePneumaticLoad.TabIndex = 31
        Me.lblM3_AveragePneumaticLoad.Text = "M3-Avg Pneumatic Load Demand"
        '
        'pnl_M3_Displays
        '
        Me.pnl_M3_Displays.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.pnl_M3_Displays.Controls.Add(Me.lblM3_TotAirConsumptionPerCycleLitres)
        Me.pnl_M3_Displays.Controls.Add(Me.lbl_M3_AvgPowerAtCrankFromPneumatics)
        Me.pnl_M3_Displays.Controls.Add(Me.txtM3_out_TotalAirConsumedPerCycleInLitres)
        Me.pnl_M3_Displays.Controls.Add(Me.txtM3_out_AveragePowerAtCrankFromPneumatics)
        Me.pnl_M3_Displays.Location = New System.Drawing.Point(379, 159)
        Me.pnl_M3_Displays.Name = "pnl_M3_Displays"
        Me.pnl_M3_Displays.Size = New System.Drawing.Size(200, 100)
        Me.pnl_M3_Displays.TabIndex = 30
        '
        'lblM3_TotAirConsumptionPerCycleLitres
        '
        Me.lblM3_TotAirConsumptionPerCycleLitres.AutoSize = true
        Me.lblM3_TotAirConsumptionPerCycleLitres.Location = New System.Drawing.Point(8, 49)
        Me.lblM3_TotAirConsumptionPerCycleLitres.Name = "lblM3_TotAirConsumptionPerCycleLitres"
        Me.lblM3_TotAirConsumptionPerCycleLitres.Size = New System.Drawing.Size(136, 13)
        Me.lblM3_TotAirConsumptionPerCycleLitres.TabIndex = 3
        Me.lblM3_TotAirConsumptionPerCycleLitres.Text = "Total Air Cosumed/Cycle(L)"
        '
        'lbl_M3_AvgPowerAtCrankFromPneumatics
        '
        Me.lbl_M3_AvgPowerAtCrankFromPneumatics.AutoSize = true
        Me.lbl_M3_AvgPowerAtCrankFromPneumatics.Location = New System.Drawing.Point(9, 4)
        Me.lbl_M3_AvgPowerAtCrankFromPneumatics.Name = "lbl_M3_AvgPowerAtCrankFromPneumatics"
        Me.lbl_M3_AvgPowerAtCrankFromPneumatics.Size = New System.Drawing.Size(182, 13)
        Me.lbl_M3_AvgPowerAtCrankFromPneumatics.TabIndex = 2
        Me.lbl_M3_AvgPowerAtCrankFromPneumatics.Text = "Avg Power@Crank From Pneumatics"
        '
        'txtM3_out_TotalAirConsumedPerCycleInLitres
        '
        Me.txtM3_out_TotalAirConsumedPerCycleInLitres.ForeColor = System.Drawing.Color.FromArgb(CType(CType(0,Byte),Integer), CType(CType(192,Byte),Integer), CType(CType(0,Byte),Integer))
        Me.txtM3_out_TotalAirConsumedPerCycleInLitres.Location = New System.Drawing.Point(10, 67)
        Me.txtM3_out_TotalAirConsumedPerCycleInLitres.Name = "txtM3_out_TotalAirConsumedPerCycleInLitres"
        Me.txtM3_out_TotalAirConsumedPerCycleInLitres.Size = New System.Drawing.Size(100, 20)
        Me.txtM3_out_TotalAirConsumedPerCycleInLitres.TabIndex = 1
        '
        'txtM3_out_AveragePowerAtCrankFromPneumatics
        '
        Me.txtM3_out_AveragePowerAtCrankFromPneumatics.ForeColor = System.Drawing.Color.FromArgb(CType(CType(0,Byte),Integer), CType(CType(192,Byte),Integer), CType(CType(0,Byte),Integer))
        Me.txtM3_out_AveragePowerAtCrankFromPneumatics.Location = New System.Drawing.Point(10, 22)
        Me.txtM3_out_AveragePowerAtCrankFromPneumatics.Name = "txtM3_out_AveragePowerAtCrankFromPneumatics"
        Me.txtM3_out_AveragePowerAtCrankFromPneumatics.Size = New System.Drawing.Size(100, 20)
        Me.txtM3_out_AveragePowerAtCrankFromPneumatics.TabIndex = 0
        '
        'lblM2AverageElectricalLoadTitle
        '
        Me.lblM2AverageElectricalLoadTitle.AutoSize = true
        Me.lblM2AverageElectricalLoadTitle.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0,Byte))
        Me.lblM2AverageElectricalLoadTitle.Location = New System.Drawing.Point(379, 14)
        Me.lblM2AverageElectricalLoadTitle.Name = "lblM2AverageElectricalLoadTitle"
        Me.lblM2AverageElectricalLoadTitle.Size = New System.Drawing.Size(189, 13)
        Me.lblM2AverageElectricalLoadTitle.TabIndex = 29
        Me.lblM2AverageElectricalLoadTitle.Text = "M2-Avg Electrical Load Demand"
        '
        'pnl_M2_Displays
        '
        Me.pnl_M2_Displays.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.pnl_M2_Displays.Controls.Add(Me.lblM2_AvgPwrDmdAtCrankFromElectrics)
        Me.pnl_M2_Displays.Controls.Add(Me.lblM2_AveragePowerDemandAtAlternatorFromElectrics)
        Me.pnl_M2_Displays.Controls.Add(Me.txtM2_out_AvgPowerAtCrankFromElectrics)
        Me.pnl_M2_Displays.Controls.Add(Me.txtM2_out_AvgPowerAtAltFromElectrics)
        Me.pnl_M2_Displays.Location = New System.Drawing.Point(381, 33)
        Me.pnl_M2_Displays.Name = "pnl_M2_Displays"
        Me.pnl_M2_Displays.Size = New System.Drawing.Size(200, 100)
        Me.pnl_M2_Displays.TabIndex = 28
        '
        'lblM2_AvgPwrDmdAtCrankFromElectrics
        '
        Me.lblM2_AvgPwrDmdAtCrankFromElectrics.AutoSize = true
        Me.lblM2_AvgPwrDmdAtCrankFromElectrics.Location = New System.Drawing.Point(7, 52)
        Me.lblM2_AvgPwrDmdAtCrankFromElectrics.Name = "lblM2_AvgPwrDmdAtCrankFromElectrics"
        Me.lblM2_AvgPwrDmdAtCrankFromElectrics.Size = New System.Drawing.Size(177, 13)
        Me.lblM2_AvgPwrDmdAtCrankFromElectrics.TabIndex = 3
        Me.lblM2_AvgPwrDmdAtCrankFromElectrics.Text = "Avg Pwr Dmd@Crank from Electrics"
        '
        'lblM2_AveragePowerDemandAtAlternatorFromElectrics
        '
        Me.lblM2_AveragePowerDemandAtAlternatorFromElectrics.AutoSize = true
        Me.lblM2_AveragePowerDemandAtAlternatorFromElectrics.Location = New System.Drawing.Point(8, 9)
        Me.lblM2_AveragePowerDemandAtAlternatorFromElectrics.Name = "lblM2_AveragePowerDemandAtAlternatorFromElectrics"
        Me.lblM2_AveragePowerDemandAtAlternatorFromElectrics.Size = New System.Drawing.Size(170, 13)
        Me.lblM2_AveragePowerDemandAtAlternatorFromElectrics.TabIndex = 2
        Me.lblM2_AveragePowerDemandAtAlternatorFromElectrics.Text = "Avg Pwr Dmd @ Alt From Electrics"
        '
        'txtM2_out_AvgPowerAtCrankFromElectrics
        '
        Me.txtM2_out_AvgPowerAtCrankFromElectrics.ForeColor = System.Drawing.Color.FromArgb(CType(CType(0,Byte),Integer), CType(CType(192,Byte),Integer), CType(CType(0,Byte),Integer))
        Me.txtM2_out_AvgPowerAtCrankFromElectrics.Location = New System.Drawing.Point(8, 68)
        Me.txtM2_out_AvgPowerAtCrankFromElectrics.Name = "txtM2_out_AvgPowerAtCrankFromElectrics"
        Me.txtM2_out_AvgPowerAtCrankFromElectrics.Size = New System.Drawing.Size(100, 20)
        Me.txtM2_out_AvgPowerAtCrankFromElectrics.TabIndex = 1
        '
        'txtM2_out_AvgPowerAtAltFromElectrics
        '
        Me.txtM2_out_AvgPowerAtAltFromElectrics.ForeColor = System.Drawing.Color.FromArgb(CType(CType(0,Byte),Integer), CType(CType(192,Byte),Integer), CType(CType(0,Byte),Integer))
        Me.txtM2_out_AvgPowerAtAltFromElectrics.Location = New System.Drawing.Point(9, 25)
        Me.txtM2_out_AvgPowerAtAltFromElectrics.Name = "txtM2_out_AvgPowerAtAltFromElectrics"
        Me.txtM2_out_AvgPowerAtAltFromElectrics.Size = New System.Drawing.Size(100, 20)
        Me.txtM2_out_AvgPowerAtAltFromElectrics.TabIndex = 0
        '
        'lblM1_HVACAverageLoad
        '
        Me.lblM1_HVACAverageLoad.AutoSize = true
        Me.lblM1_HVACAverageLoad.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0,Byte))
        Me.lblM1_HVACAverageLoad.Location = New System.Drawing.Point(164, 423)
        Me.lblM1_HVACAverageLoad.Name = "lblM1_HVACAverageLoad"
        Me.lblM1_HVACAverageLoad.Size = New System.Drawing.Size(143, 13)
        Me.lblM1_HVACAverageLoad.TabIndex = 27
        Me.lblM1_HVACAverageLoad.Text = "M1-HVAC_AverageLoad"
        '
        'pnl_M1_Displays
        '
        Me.pnl_M1_Displays.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.pnl_M1_Displays.Controls.Add(Me.lblM1_HVACFuelling)
        Me.pnl_M1_Displays.Controls.Add(Me.lblM1_AvgPowerDemandAtCrankHVACElectrics)
        Me.pnl_M1_Displays.Controls.Add(Me.lblM1_AveragePowerDemandAtAlternatorElectrics)
        Me.pnl_M1_Displays.Controls.Add(Me.lblM1_AveragePowerDemandAtCrank)
        Me.pnl_M1_Displays.Controls.Add(Me.txtM1_out_HVACFuelling)
        Me.pnl_M1_Displays.Controls.Add(Me.txtM1_out_AvgPwrAtCrankFromHVACElec)
        Me.pnl_M1_Displays.Controls.Add(Me.txtM1_out_AvgPowerDemandAtAlternatorHvacElectrics)
        Me.pnl_M1_Displays.Controls.Add(Me.txtM1_out_AvgPowerDemandAtCrankMech)
        Me.pnl_M1_Displays.Location = New System.Drawing.Point(166, 442)
        Me.pnl_M1_Displays.Name = "pnl_M1_Displays"
        Me.pnl_M1_Displays.Size = New System.Drawing.Size(200, 185)
        Me.pnl_M1_Displays.TabIndex = 26
        '
        'lblM1_HVACFuelling
        '
        Me.lblM1_HVACFuelling.AutoSize = true
        Me.lblM1_HVACFuelling.Location = New System.Drawing.Point(4, 133)
        Me.lblM1_HVACFuelling.Name = "lblM1_HVACFuelling"
        Me.lblM1_HVACFuelling.Size = New System.Drawing.Size(75, 13)
        Me.lblM1_HVACFuelling.TabIndex = 7
        Me.lblM1_HVACFuelling.Text = "HVAC Fuelling"
        '
        'lblM1_AvgPowerDemandAtCrankHVACElectrics
        '
        Me.lblM1_AvgPowerDemandAtCrankHVACElectrics.AutoSize = true
        Me.lblM1_AvgPowerDemandAtCrankHVACElectrics.Location = New System.Drawing.Point(4, 92)
        Me.lblM1_AvgPowerDemandAtCrankHVACElectrics.Name = "lblM1_AvgPowerDemandAtCrankHVACElectrics"
        Me.lblM1_AvgPowerDemandAtCrankHVACElectrics.Size = New System.Drawing.Size(173, 13)
        Me.lblM1_AvgPowerDemandAtCrankHVACElectrics.TabIndex = 6
        Me.lblM1_AvgPowerDemandAtCrankHVACElectrics.Text = "Avg Pwr Dmd @ Crank HVAC Elec"
        '
        'lblM1_AveragePowerDemandAtAlternatorElectrics
        '
        Me.lblM1_AveragePowerDemandAtAlternatorElectrics.AutoSize = true
        Me.lblM1_AveragePowerDemandAtAlternatorElectrics.Location = New System.Drawing.Point(5, 51)
        Me.lblM1_AveragePowerDemandAtAlternatorElectrics.Name = "lblM1_AveragePowerDemandAtAlternatorElectrics"
        Me.lblM1_AveragePowerDemandAtAlternatorElectrics.Size = New System.Drawing.Size(176, 13)
        Me.lblM1_AveragePowerDemandAtAlternatorElectrics.TabIndex = 5
        Me.lblM1_AveragePowerDemandAtAlternatorElectrics.Text = "Avg Pwr Dmd @ Alt HVAC Electrics"
        '
        'lblM1_AveragePowerDemandAtCrank
        '
        Me.lblM1_AveragePowerDemandAtCrank.AutoSize = true
        Me.lblM1_AveragePowerDemandAtCrank.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0,Byte))
        Me.lblM1_AveragePowerDemandAtCrank.Location = New System.Drawing.Point(6, 8)
        Me.lblM1_AveragePowerDemandAtCrank.Name = "lblM1_AveragePowerDemandAtCrank"
        Me.lblM1_AveragePowerDemandAtCrank.Size = New System.Drawing.Size(147, 13)
        Me.lblM1_AveragePowerDemandAtCrank.TabIndex = 4
        Me.lblM1_AveragePowerDemandAtCrank.Text = "Avg Pwr Dmd @ Crank Mech"
        '
        'txtM1_out_HVACFuelling
        '
        Me.txtM1_out_HVACFuelling.ForeColor = System.Drawing.Color.FromArgb(CType(CType(0,Byte),Integer), CType(CType(192,Byte),Integer), CType(CType(0,Byte),Integer))
        Me.txtM1_out_HVACFuelling.Location = New System.Drawing.Point(7, 149)
        Me.txtM1_out_HVACFuelling.Name = "txtM1_out_HVACFuelling"
        Me.txtM1_out_HVACFuelling.Size = New System.Drawing.Size(100, 20)
        Me.txtM1_out_HVACFuelling.TabIndex = 3
        '
        'txtM1_out_AvgPwrAtCrankFromHVACElec
        '
        Me.txtM1_out_AvgPwrAtCrankFromHVACElec.ForeColor = System.Drawing.Color.FromArgb(CType(CType(0,Byte),Integer), CType(CType(192,Byte),Integer), CType(CType(0,Byte),Integer))
        Me.txtM1_out_AvgPwrAtCrankFromHVACElec.Location = New System.Drawing.Point(7, 108)
        Me.txtM1_out_AvgPwrAtCrankFromHVACElec.Name = "txtM1_out_AvgPwrAtCrankFromHVACElec"
        Me.txtM1_out_AvgPwrAtCrankFromHVACElec.Size = New System.Drawing.Size(100, 20)
        Me.txtM1_out_AvgPwrAtCrankFromHVACElec.TabIndex = 2
        '
        'txtM1_out_AvgPowerDemandAtAlternatorHvacElectrics
        '
        Me.txtM1_out_AvgPowerDemandAtAlternatorHvacElectrics.ForeColor = System.Drawing.Color.FromArgb(CType(CType(0,Byte),Integer), CType(CType(192,Byte),Integer), CType(CType(0,Byte),Integer))
        Me.txtM1_out_AvgPowerDemandAtAlternatorHvacElectrics.Location = New System.Drawing.Point(7, 65)
        Me.txtM1_out_AvgPowerDemandAtAlternatorHvacElectrics.Name = "txtM1_out_AvgPowerDemandAtAlternatorHvacElectrics"
        Me.txtM1_out_AvgPowerDemandAtAlternatorHvacElectrics.Size = New System.Drawing.Size(100, 20)
        Me.txtM1_out_AvgPowerDemandAtAlternatorHvacElectrics.TabIndex = 1
        '
        'txtM1_out_AvgPowerDemandAtCrankMech
        '
        Me.txtM1_out_AvgPowerDemandAtCrankMech.ForeColor = System.Drawing.Color.FromArgb(CType(CType(0,Byte),Integer), CType(CType(192,Byte),Integer), CType(CType(0,Byte),Integer))
        Me.txtM1_out_AvgPowerDemandAtCrankMech.Location = New System.Drawing.Point(7, 24)
        Me.txtM1_out_AvgPowerDemandAtCrankMech.Name = "txtM1_out_AvgPowerDemandAtCrankMech"
        Me.txtM1_out_AvgPowerDemandAtCrankMech.Size = New System.Drawing.Size(100, 20)
        Me.txtM1_out_AvgPowerDemandAtCrankMech.TabIndex = 0
        '
        'lblM05SmartalternatorSetEfficiency
        '
        Me.lblM05SmartalternatorSetEfficiency.AutoSize = true
        Me.lblM05SmartalternatorSetEfficiency.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0,Byte))
        Me.lblM05SmartalternatorSetEfficiency.Location = New System.Drawing.Point(163, 141)
        Me.lblM05SmartalternatorSetEfficiency.Name = "lblM05SmartalternatorSetEfficiency"
        Me.lblM05SmartalternatorSetEfficiency.Size = New System.Drawing.Size(169, 13)
        Me.lblM05SmartalternatorSetEfficiency.TabIndex = 25
        Me.lblM05SmartalternatorSetEfficiency.Text = "M05-Smart Alt Set Efficiency"
        '
        'pnl_M05_Displays
        '
        Me.pnl_M05_Displays.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.pnl_M05_Displays.Controls.Add(Me.lblM05_AlternatorsEfficiencyOverrun)
        Me.pnl_M05_Displays.Controls.Add(Me.lblM05SmartOverrunCurrent)
        Me.pnl_M05_Displays.Controls.Add(Me.lblM05_AlternatorsEfficiencyTraction)
        Me.pnl_M05_Displays.Controls.Add(Me.lblM05_SmartTractionCurrent)
        Me.pnl_M05_Displays.Controls.Add(Me.lblM05AlternatorsEfficiencyIdle)
        Me.pnl_M05_Displays.Controls.Add(Me.lblM05_SmartIdleCurrent)
        Me.pnl_M05_Displays.Controls.Add(Me.txtM05_out_AlternatorsEfficiencyOverrun)
        Me.pnl_M05_Displays.Controls.Add(Me.txtM05_out_SmartOverrunCurrent)
        Me.pnl_M05_Displays.Controls.Add(Me.txtM05_out_AlternatorsEfficiencyTraction)
        Me.pnl_M05_Displays.Controls.Add(Me.txtM05_out_SmartTractionCurrent)
        Me.pnl_M05_Displays.Controls.Add(Me.txtM05_Out_AlternatorsEfficiencyIdle)
        Me.pnl_M05_Displays.Controls.Add(Me.txtM05_OutSmartIdleCurrent)
        Me.pnl_M05_Displays.Location = New System.Drawing.Point(164, 158)
        Me.pnl_M05_Displays.Name = "pnl_M05_Displays"
        Me.pnl_M05_Displays.Size = New System.Drawing.Size(199, 259)
        Me.pnl_M05_Displays.TabIndex = 24
        '
        'lblM05_AlternatorsEfficiencyOverrun
        '
        Me.lblM05_AlternatorsEfficiencyOverrun.AutoSize = true
        Me.lblM05_AlternatorsEfficiencyOverrun.Location = New System.Drawing.Point(11, 213)
        Me.lblM05_AlternatorsEfficiencyOverrun.Name = "lblM05_AlternatorsEfficiencyOverrun"
        Me.lblM05_AlternatorsEfficiencyOverrun.Size = New System.Drawing.Size(147, 13)
        Me.lblM05_AlternatorsEfficiencyOverrun.TabIndex = 11
        Me.lblM05_AlternatorsEfficiencyOverrun.Text = "Alternators Efficiency Overrun"
        '
        'lblM05SmartOverrunCurrent
        '
        Me.lblM05SmartOverrunCurrent.AutoSize = true
        Me.lblM05SmartOverrunCurrent.Location = New System.Drawing.Point(11, 172)
        Me.lblM05SmartOverrunCurrent.Name = "lblM05SmartOverrunCurrent"
        Me.lblM05SmartOverrunCurrent.Size = New System.Drawing.Size(112, 13)
        Me.lblM05SmartOverrunCurrent.TabIndex = 10
        Me.lblM05SmartOverrunCurrent.Text = "Smart Overrun Current"
        '
        'lblM05_AlternatorsEfficiencyTraction
        '
        Me.lblM05_AlternatorsEfficiencyTraction.AutoSize = true
        Me.lblM05_AlternatorsEfficiencyTraction.Location = New System.Drawing.Point(10, 130)
        Me.lblM05_AlternatorsEfficiencyTraction.Name = "lblM05_AlternatorsEfficiencyTraction"
        Me.lblM05_AlternatorsEfficiencyTraction.Size = New System.Drawing.Size(148, 13)
        Me.lblM05_AlternatorsEfficiencyTraction.TabIndex = 9
        Me.lblM05_AlternatorsEfficiencyTraction.Text = "Alternators Efficiency Traction"
        '
        'lblM05_SmartTractionCurrent
        '
        Me.lblM05_SmartTractionCurrent.AutoSize = true
        Me.lblM05_SmartTractionCurrent.Location = New System.Drawing.Point(10, 87)
        Me.lblM05_SmartTractionCurrent.Name = "lblM05_SmartTractionCurrent"
        Me.lblM05_SmartTractionCurrent.Size = New System.Drawing.Size(113, 13)
        Me.lblM05_SmartTractionCurrent.TabIndex = 8
        Me.lblM05_SmartTractionCurrent.Text = "Smart Traction Current"
        '
        'lblM05AlternatorsEfficiencyIdle
        '
        Me.lblM05AlternatorsEfficiencyIdle.AutoSize = true
        Me.lblM05AlternatorsEfficiencyIdle.Location = New System.Drawing.Point(11, 45)
        Me.lblM05AlternatorsEfficiencyIdle.Name = "lblM05AlternatorsEfficiencyIdle"
        Me.lblM05AlternatorsEfficiencyIdle.Size = New System.Drawing.Size(126, 13)
        Me.lblM05AlternatorsEfficiencyIdle.TabIndex = 7
        Me.lblM05AlternatorsEfficiencyIdle.Text = "Alternators Efficiency Idle"
        '
        'lblM05_SmartIdleCurrent
        '
        Me.lblM05_SmartIdleCurrent.AutoSize = true
        Me.lblM05_SmartIdleCurrent.Location = New System.Drawing.Point(11, 5)
        Me.lblM05_SmartIdleCurrent.Name = "lblM05_SmartIdleCurrent"
        Me.lblM05_SmartIdleCurrent.Size = New System.Drawing.Size(91, 13)
        Me.lblM05_SmartIdleCurrent.TabIndex = 6
        Me.lblM05_SmartIdleCurrent.Text = "Smart Idle Current"
        '
        'txtM05_out_AlternatorsEfficiencyOverrun
        '
        Me.txtM05_out_AlternatorsEfficiencyOverrun.ForeColor = System.Drawing.Color.FromArgb(CType(CType(0,Byte),Integer), CType(CType(192,Byte),Integer), CType(CType(0,Byte),Integer))
        Me.txtM05_out_AlternatorsEfficiencyOverrun.Location = New System.Drawing.Point(11, 226)
        Me.txtM05_out_AlternatorsEfficiencyOverrun.Name = "txtM05_out_AlternatorsEfficiencyOverrun"
        Me.txtM05_out_AlternatorsEfficiencyOverrun.Size = New System.Drawing.Size(100, 20)
        Me.txtM05_out_AlternatorsEfficiencyOverrun.TabIndex = 5
        '
        'txtM05_out_SmartOverrunCurrent
        '
        Me.txtM05_out_SmartOverrunCurrent.ForeColor = System.Drawing.Color.FromArgb(CType(CType(0,Byte),Integer), CType(CType(192,Byte),Integer), CType(CType(0,Byte),Integer))
        Me.txtM05_out_SmartOverrunCurrent.Location = New System.Drawing.Point(11, 187)
        Me.txtM05_out_SmartOverrunCurrent.Name = "txtM05_out_SmartOverrunCurrent"
        Me.txtM05_out_SmartOverrunCurrent.Size = New System.Drawing.Size(100, 20)
        Me.txtM05_out_SmartOverrunCurrent.TabIndex = 4
        '
        'txtM05_out_AlternatorsEfficiencyTraction
        '
        Me.txtM05_out_AlternatorsEfficiencyTraction.ForeColor = System.Drawing.Color.FromArgb(CType(CType(0,Byte),Integer), CType(CType(192,Byte),Integer), CType(CType(0,Byte),Integer))
        Me.txtM05_out_AlternatorsEfficiencyTraction.Location = New System.Drawing.Point(11, 143)
        Me.txtM05_out_AlternatorsEfficiencyTraction.Name = "txtM05_out_AlternatorsEfficiencyTraction"
        Me.txtM05_out_AlternatorsEfficiencyTraction.Size = New System.Drawing.Size(100, 20)
        Me.txtM05_out_AlternatorsEfficiencyTraction.TabIndex = 3
        '
        'txtM05_out_SmartTractionCurrent
        '
        Me.txtM05_out_SmartTractionCurrent.ForeColor = System.Drawing.Color.FromArgb(CType(CType(0,Byte),Integer), CType(CType(192,Byte),Integer), CType(CType(0,Byte),Integer))
        Me.txtM05_out_SmartTractionCurrent.Location = New System.Drawing.Point(11, 102)
        Me.txtM05_out_SmartTractionCurrent.Name = "txtM05_out_SmartTractionCurrent"
        Me.txtM05_out_SmartTractionCurrent.Size = New System.Drawing.Size(100, 20)
        Me.txtM05_out_SmartTractionCurrent.TabIndex = 2
        '
        'txtM05_Out_AlternatorsEfficiencyIdle
        '
        Me.txtM05_Out_AlternatorsEfficiencyIdle.ForeColor = System.Drawing.Color.FromArgb(CType(CType(0,Byte),Integer), CType(CType(192,Byte),Integer), CType(CType(0,Byte),Integer))
        Me.txtM05_Out_AlternatorsEfficiencyIdle.Location = New System.Drawing.Point(11, 60)
        Me.txtM05_Out_AlternatorsEfficiencyIdle.Name = "txtM05_Out_AlternatorsEfficiencyIdle"
        Me.txtM05_Out_AlternatorsEfficiencyIdle.Size = New System.Drawing.Size(100, 20)
        Me.txtM05_Out_AlternatorsEfficiencyIdle.TabIndex = 1
        '
        'txtM05_OutSmartIdleCurrent
        '
        Me.txtM05_OutSmartIdleCurrent.ForeColor = System.Drawing.Color.FromArgb(CType(CType(0,Byte),Integer), CType(CType(192,Byte),Integer), CType(CType(0,Byte),Integer))
        Me.txtM05_OutSmartIdleCurrent.Location = New System.Drawing.Point(11, 19)
        Me.txtM05_OutSmartIdleCurrent.Name = "txtM05_OutSmartIdleCurrent"
        Me.txtM05_OutSmartIdleCurrent.Size = New System.Drawing.Size(100, 20)
        Me.txtM05_OutSmartIdleCurrent.TabIndex = 0
        '
        'lblM0Outputs
        '
        Me.lblM0Outputs.AutoSize = true
        Me.lblM0Outputs.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0,Byte))
        Me.lblM0Outputs.Location = New System.Drawing.Point(164, 13)
        Me.lblM0Outputs.Name = "lblM0Outputs"
        Me.lblM0Outputs.Size = New System.Drawing.Size(189, 13)
        Me.lblM0Outputs.TabIndex = 23
        Me.lblM0Outputs.Text = "M0-Non-Smart Alt Set Efficiency"
        '
        'pnl_M0_Displays
        '
        Me.pnl_M0_Displays.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.pnl_M0_Displays.Controls.Add(Me.lblOutHVACElectricalCurrentDemand)
        Me.pnl_M0_Displays.Controls.Add(Me.lblAlternatorsEfficiency)
        Me.pnl_M0_Displays.Controls.Add(Me.txtM0_Out_AlternatorsEfficiency)
        Me.pnl_M0_Displays.Controls.Add(Me.txtM0_Out_HVacElectricalCurrentDemand)
        Me.pnl_M0_Displays.ForeColor = System.Drawing.Color.Black
        Me.pnl_M0_Displays.Location = New System.Drawing.Point(164, 33)
        Me.pnl_M0_Displays.Name = "pnl_M0_Displays"
        Me.pnl_M0_Displays.Size = New System.Drawing.Size(199, 100)
        Me.pnl_M0_Displays.TabIndex = 22
        '
        'lblOutHVACElectricalCurrentDemand
        '
        Me.lblOutHVACElectricalCurrentDemand.AutoSize = true
        Me.lblOutHVACElectricalCurrentDemand.Location = New System.Drawing.Point(9, 8)
        Me.lblOutHVACElectricalCurrentDemand.Name = "lblOutHVACElectricalCurrentDemand"
        Me.lblOutHVACElectricalCurrentDemand.Size = New System.Drawing.Size(162, 13)
        Me.lblOutHVACElectricalCurrentDemand.TabIndex = 25
        Me.lblOutHVACElectricalCurrentDemand.Text = "HVAC Electrical Current Demand"
        '
        'lblAlternatorsEfficiency
        '
        Me.lblAlternatorsEfficiency.AutoSize = true
        Me.lblAlternatorsEfficiency.Location = New System.Drawing.Point(9, 49)
        Me.lblAlternatorsEfficiency.Name = "lblAlternatorsEfficiency"
        Me.lblAlternatorsEfficiency.Size = New System.Drawing.Size(106, 13)
        Me.lblAlternatorsEfficiency.TabIndex = 24
        Me.lblAlternatorsEfficiency.Text = "Alternators Efficiency"
        '
        'txtM0_Out_AlternatorsEfficiency
        '
        Me.txtM0_Out_AlternatorsEfficiency.ForeColor = System.Drawing.Color.FromArgb(CType(CType(0,Byte),Integer), CType(CType(192,Byte),Integer), CType(CType(0,Byte),Integer))
        Me.txtM0_Out_AlternatorsEfficiency.Location = New System.Drawing.Point(12, 65)
        Me.txtM0_Out_AlternatorsEfficiency.Name = "txtM0_Out_AlternatorsEfficiency"
        Me.txtM0_Out_AlternatorsEfficiency.Size = New System.Drawing.Size(100, 20)
        Me.txtM0_Out_AlternatorsEfficiency.TabIndex = 1
        '
        'txtM0_Out_HVacElectricalCurrentDemand
        '
        Me.txtM0_Out_HVacElectricalCurrentDemand.ForeColor = System.Drawing.Color.FromArgb(CType(CType(0,Byte),Integer), CType(CType(192,Byte),Integer), CType(CType(0,Byte),Integer))
        Me.txtM0_Out_HVacElectricalCurrentDemand.Location = New System.Drawing.Point(9, 25)
        Me.txtM0_Out_HVacElectricalCurrentDemand.Name = "txtM0_Out_HVacElectricalCurrentDemand"
        Me.txtM0_Out_HVacElectricalCurrentDemand.Size = New System.Drawing.Size(100, 20)
        Me.txtM0_Out_HVacElectricalCurrentDemand.TabIndex = 0
        '
        'resultCardContextMenu
        '
        Me.resultCardContextMenu.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.DeleteToolStripMenuItem})
        Me.resultCardContextMenu.Name = "resultCardContextMenu"
        Me.resultCardContextMenu.Size = New System.Drawing.Size(108, 26)
        '
        'DeleteToolStripMenuItem
        '
        Me.DeleteToolStripMenuItem.Name = "DeleteToolStripMenuItem"
        Me.DeleteToolStripMenuItem.Size = New System.Drawing.Size(107, 22)
        Me.DeleteToolStripMenuItem.Text = "Delete"
        '
        'ErrorProvider
        '
        Me.ErrorProvider.ContainerControl = Me
        '
        'Timer1
        '
        '
        'Panel3
        '
        Me.Panel3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.Panel3.Controls.Add(Me.lblM5_AlternatorRegenPowerAtCrankTractionWatts)
        Me.Panel3.Controls.Add(Me.M5_AlternatorGenerationPowerAtCrankOverrunWatts)
        Me.Panel3.Controls.Add(Me.txtM5_out_AltRegenPowerAtCrankOverrunWatts)
        Me.Panel3.Controls.Add(Me.lblM5_AltRegenPowerAtCrankIdleW)
        Me.Panel3.Controls.Add(Me.txtM5_out_AltRegenPowerAtCrankTractionWatts)
        Me.Panel3.Controls.Add(Me.txtM5_out_AltRegenPowerAtCrankIdleWatts)
        Me.Panel3.Location = New System.Drawing.Point(380, 486)
        Me.Panel3.Name = "Panel3"
        Me.Panel3.Size = New System.Drawing.Size(197, 141)
        Me.Panel3.TabIndex = 36
        '
        'lblM5_SmartAltSetGeneration
        '
        Me.lblM5_SmartAltSetGeneration.AutoSize = true
        Me.lblM5_SmartAltSetGeneration.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0,Byte))
        Me.lblM5_SmartAltSetGeneration.Location = New System.Drawing.Point(379, 470)
        Me.lblM5_SmartAltSetGeneration.Name = "lblM5_SmartAltSetGeneration"
        Me.lblM5_SmartAltSetGeneration.Size = New System.Drawing.Size(168, 13)
        Me.lblM5_SmartAltSetGeneration.TabIndex = 37
        Me.lblM5_SmartAltSetGeneration.Text = "M5-Smart Alt Set Generation"
        '
        'txtM5_out_AltRegenPowerAtCrankIdleWatts
        '
        Me.txtM5_out_AltRegenPowerAtCrankIdleWatts.ForeColor = System.Drawing.Color.FromArgb(CType(CType(0,Byte),Integer), CType(CType(192,Byte),Integer), CType(CType(0,Byte),Integer))
        Me.txtM5_out_AltRegenPowerAtCrankIdleWatts.Location = New System.Drawing.Point(8, 21)
        Me.txtM5_out_AltRegenPowerAtCrankIdleWatts.Name = "txtM5_out_AltRegenPowerAtCrankIdleWatts"
        Me.txtM5_out_AltRegenPowerAtCrankIdleWatts.Size = New System.Drawing.Size(100, 20)
        Me.txtM5_out_AltRegenPowerAtCrankIdleWatts.TabIndex = 0
        '
        'txtM5_out_AltRegenPowerAtCrankTractionWatts
        '
        Me.txtM5_out_AltRegenPowerAtCrankTractionWatts.ForeColor = System.Drawing.Color.FromArgb(CType(CType(0,Byte),Integer), CType(CType(192,Byte),Integer), CType(CType(0,Byte),Integer))
        Me.txtM5_out_AltRegenPowerAtCrankTractionWatts.Location = New System.Drawing.Point(8, 64)
        Me.txtM5_out_AltRegenPowerAtCrankTractionWatts.Name = "txtM5_out_AltRegenPowerAtCrankTractionWatts"
        Me.txtM5_out_AltRegenPowerAtCrankTractionWatts.Size = New System.Drawing.Size(100, 20)
        Me.txtM5_out_AltRegenPowerAtCrankTractionWatts.TabIndex = 1
        '
        'lblM5_AltRegenPowerAtCrankIdleW
        '
        Me.lblM5_AltRegenPowerAtCrankIdleW.AutoSize = true
        Me.lblM5_AltRegenPowerAtCrankIdleW.Location = New System.Drawing.Point(5, 5)
        Me.lblM5_AltRegenPowerAtCrankIdleW.Name = "lblM5_AltRegenPowerAtCrankIdleW"
        Me.lblM5_AltRegenPowerAtCrankIdleW.Size = New System.Drawing.Size(165, 13)
        Me.lblM5_AltRegenPowerAtCrankIdleW.TabIndex = 2
        Me.lblM5_AltRegenPowerAtCrankIdleW.Text = "Alt Regen Pwr@tCrank IdleWatts"
        '
        'M5_AlternatorGenerationPowerAtCrankOverrunWatts
        '
        Me.M5_AlternatorGenerationPowerAtCrankOverrunWatts.AutoSize = true
        Me.M5_AlternatorGenerationPowerAtCrankOverrunWatts.Location = New System.Drawing.Point(8, 87)
        Me.M5_AlternatorGenerationPowerAtCrankOverrunWatts.Name = "M5_AlternatorGenerationPowerAtCrankOverrunWatts"
        Me.M5_AlternatorGenerationPowerAtCrankOverrunWatts.Size = New System.Drawing.Size(186, 13)
        Me.M5_AlternatorGenerationPowerAtCrankOverrunWatts.TabIndex = 5
        Me.M5_AlternatorGenerationPowerAtCrankOverrunWatts.Text = "Alt Regen Pwr@Crank Overrun Watts"
        '
        'txtM5_out_AltRegenPowerAtCrankOverrunWatts
        '
        Me.txtM5_out_AltRegenPowerAtCrankOverrunWatts.ForeColor = System.Drawing.Color.FromArgb(CType(CType(0,Byte),Integer), CType(CType(192,Byte),Integer), CType(CType(0,Byte),Integer))
        Me.txtM5_out_AltRegenPowerAtCrankOverrunWatts.Location = New System.Drawing.Point(8, 105)
        Me.txtM5_out_AltRegenPowerAtCrankOverrunWatts.Name = "txtM5_out_AltRegenPowerAtCrankOverrunWatts"
        Me.txtM5_out_AltRegenPowerAtCrankOverrunWatts.Size = New System.Drawing.Size(100, 20)
        Me.txtM5_out_AltRegenPowerAtCrankOverrunWatts.TabIndex = 4
        '
        'lblM5_AlternatorRegenPowerAtCrankTractionWatts
        '
        Me.lblM5_AlternatorRegenPowerAtCrankTractionWatts.AutoSize = true
        Me.lblM5_AlternatorRegenPowerAtCrankTractionWatts.Location = New System.Drawing.Point(5, 48)
        Me.lblM5_AlternatorRegenPowerAtCrankTractionWatts.Name = "lblM5_AlternatorRegenPowerAtCrankTractionWatts"
        Me.lblM5_AlternatorRegenPowerAtCrankTractionWatts.Size = New System.Drawing.Size(190, 13)
        Me.lblM5_AlternatorRegenPowerAtCrankTractionWatts.TabIndex = 6
        Me.lblM5_AlternatorRegenPowerAtCrankTractionWatts.Text = "Alt Regen Pwr@tCrank Traction Watts"
        '
        'txtEngineDrivelinePower
        '
        Me.txtEngineDrivelinePower.Location = New System.Drawing.Point(25, 76)
        Me.txtEngineDrivelinePower.Name = "txtEngineDrivelinePower"
        Me.txtEngineDrivelinePower.Size = New System.Drawing.Size(100, 20)
        Me.txtEngineDrivelinePower.TabIndex = 1
        '
        'txtEngineDrivelineTorque
        '
        Me.txtEngineDrivelineTorque.Location = New System.Drawing.Point(25, 120)
        Me.txtEngineDrivelineTorque.Name = "txtEngineDrivelineTorque"
        Me.txtEngineDrivelineTorque.Size = New System.Drawing.Size(100, 20)
        Me.txtEngineDrivelineTorque.TabIndex = 2
        '
        'txtEngineMotoringPower
        '
        Me.txtEngineMotoringPower.Location = New System.Drawing.Point(25, 165)
        Me.txtEngineMotoringPower.Name = "txtEngineMotoringPower"
        Me.txtEngineMotoringPower.Size = New System.Drawing.Size(100, 20)
        Me.txtEngineMotoringPower.TabIndex = 3
        '
        'txtEngineSpeed
        '
        Me.txtEngineSpeed.Location = New System.Drawing.Point(25, 205)
        Me.txtEngineSpeed.Name = "txtEngineSpeed"
        Me.txtEngineSpeed.Size = New System.Drawing.Size(100, 20)
        Me.txtEngineSpeed.TabIndex = 4
        '
        'txtTotalCycleTimeSeconds
        '
        Me.txtTotalCycleTimeSeconds.Location = New System.Drawing.Point(25, 254)
        Me.txtTotalCycleTimeSeconds.Name = "txtTotalCycleTimeSeconds"
        Me.txtTotalCycleTimeSeconds.Size = New System.Drawing.Size(100, 20)
        Me.txtTotalCycleTimeSeconds.TabIndex = 5
        '
        'lblEngineDrivelinePower
        '
        Me.lblEngineDrivelinePower.AutoSize = true
        Me.lblEngineDrivelinePower.Location = New System.Drawing.Point(22, 61)
        Me.lblEngineDrivelinePower.Name = "lblEngineDrivelinePower"
        Me.lblEngineDrivelinePower.Size = New System.Drawing.Size(120, 13)
        Me.lblEngineDrivelinePower.TabIndex = 10
        Me.lblEngineDrivelinePower.Text = "Engine Driveline Power "
        '
        'lblEngineDrivelineTorque
        '
        Me.lblEngineDrivelineTorque.AutoSize = true
        Me.lblEngineDrivelineTorque.Location = New System.Drawing.Point(24, 103)
        Me.lblEngineDrivelineTorque.Name = "lblEngineDrivelineTorque"
        Me.lblEngineDrivelineTorque.Size = New System.Drawing.Size(121, 13)
        Me.lblEngineDrivelineTorque.TabIndex = 11
        Me.lblEngineDrivelineTorque.Text = "Engine Driveline Torque"
        '
        'lblEngineMotoringPower
        '
        Me.lblEngineMotoringPower.AutoSize = true
        Me.lblEngineMotoringPower.Location = New System.Drawing.Point(24, 148)
        Me.lblEngineMotoringPower.Name = "lblEngineMotoringPower"
        Me.lblEngineMotoringPower.Size = New System.Drawing.Size(117, 13)
        Me.lblEngineMotoringPower.TabIndex = 12
        Me.lblEngineMotoringPower.Text = "Engine Motoring Power"
        '
        'lblEngineSpeed
        '
        Me.lblEngineSpeed.AutoSize = true
        Me.lblEngineSpeed.Location = New System.Drawing.Point(23, 192)
        Me.lblEngineSpeed.Name = "lblEngineSpeed"
        Me.lblEngineSpeed.Size = New System.Drawing.Size(107, 13)
        Me.lblEngineSpeed.TabIndex = 13
        Me.lblEngineSpeed.Text = "Engine Speed (RPM)"
        '
        'lblTotalCycleTimeSeconds
        '
        Me.lblTotalCycleTimeSeconds.AutoSize = true
        Me.lblTotalCycleTimeSeconds.Location = New System.Drawing.Point(25, 237)
        Me.lblTotalCycleTimeSeconds.Name = "lblTotalCycleTimeSeconds"
        Me.lblTotalCycleTimeSeconds.Size = New System.Drawing.Size(134, 13)
        Me.lblTotalCycleTimeSeconds.TabIndex = 14
        Me.lblTotalCycleTimeSeconds.Text = "Total Cycle Time Seconds "
        '
        'chkClutchEngaged
        '
        Me.chkClutchEngaged.AutoSize = true
        Me.chkClutchEngaged.Location = New System.Drawing.Point(25, 20)
        Me.chkClutchEngaged.Name = "chkClutchEngaged"
        Me.chkClutchEngaged.Size = New System.Drawing.Size(102, 17)
        Me.chkClutchEngaged.TabIndex = 20
        Me.chkClutchEngaged.Text = "Clutch Engaged"
        Me.chkClutchEngaged.UseVisualStyleBackColor = true
        '
        'btnStart
        '
        Me.btnStart.Location = New System.Drawing.Point(25, 392)
        Me.btnStart.Name = "btnStart"
        Me.btnStart.Size = New System.Drawing.Size(102, 23)
        Me.btnStart.TabIndex = 1
        Me.btnStart.Text = "Start Processing"
        Me.btnStart.UseVisualStyleBackColor = true
        '
        'btnFinish
        '
        Me.btnFinish.Location = New System.Drawing.Point(25, 448)
        Me.btnFinish.Name = "btnFinish"
        Me.btnFinish.Size = New System.Drawing.Size(100, 23)
        Me.btnFinish.TabIndex = 4
        Me.btnFinish.Text = "Stop Processing"
        Me.btnFinish.UseVisualStyleBackColor = true
        '
        'PictureBox1
        '
        Me.PictureBox1.Location = New System.Drawing.Point(28, 280)
        Me.PictureBox1.Name = "PictureBox1"
        Me.PictureBox1.Size = New System.Drawing.Size(100, 94)
        Me.PictureBox1.TabIndex = 34
        Me.PictureBox1.TabStop = false
        '
        'Dashboard
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6!, 13!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.AutoValidate = System.Windows.Forms.AutoValidate.EnableAllowFocusChange
        Me.ClientSize = New System.Drawing.Size(945, 742)
        Me.Controls.Add(Me.pnlMain)
        Me.Name = "Dashboard"
        Me.Text = "Dashboard"
        Me.pnlMain.ResumeLayout(false)
        Me.tabMain.ResumeLayout(false)
        Me.tabGeneralConfig.ResumeLayout(false)
        Me.tabGeneralConfig.PerformLayout
        Me.tabElectricalConfig.ResumeLayout(false)
        Me.tabElectricalConfig.PerformLayout
        CType(Me.gvResultsCardOverrun,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.gvResultsCardTraction,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.gvResultsCardIdle,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.gvElectricalConsumables,System.ComponentModel.ISupportInitialize).EndInit
        Me.tabPneumaticConfig.ResumeLayout(false)
        Me.pnlPneumaticsUserInput.ResumeLayout(false)
        Me.pnlPneumaticsUserInput.PerformLayout
        Me.pnlPneumaticAuxillaries.ResumeLayout(false)
        Me.pnlPneumaticAuxillaries.PerformLayout
        Me.tabHVACConfig.ResumeLayout(false)
        Me.tabHVACConfig.PerformLayout
        Me.tabPlayground.ResumeLayout(false)
        Me.tabPlayground.PerformLayout
        Me.Panel1.ResumeLayout(false)
        Me.Panel1.PerformLayout
        Me.pnl_M3_Displays.ResumeLayout(false)
        Me.pnl_M3_Displays.PerformLayout
        Me.pnl_M2_Displays.ResumeLayout(false)
        Me.pnl_M2_Displays.PerformLayout
        Me.pnl_M1_Displays.ResumeLayout(false)
        Me.pnl_M1_Displays.PerformLayout
        Me.pnl_M05_Displays.ResumeLayout(false)
        Me.pnl_M05_Displays.PerformLayout
        Me.pnl_M0_Displays.ResumeLayout(false)
        Me.pnl_M0_Displays.PerformLayout
        Me.resultCardContextMenu.ResumeLayout(false)
        CType(Me.ErrorProvider,System.ComponentModel.ISupportInitialize).EndInit
        Me.Panel3.ResumeLayout(false)
        Me.Panel3.PerformLayout
        CType(Me.PictureBox1,System.ComponentModel.ISupportInitialize).EndInit
        Me.ResumeLayout(false)

End Sub
    Friend WithEvents pnlMain As System.Windows.Forms.Panel
    Friend WithEvents btnCancel As System.Windows.Forms.Button
    Friend WithEvents btnForward As System.Windows.Forms.Button
    Friend WithEvents btnBack As System.Windows.Forms.Button
    Friend WithEvents resultCardContextMenu As System.Windows.Forms.ContextMenuStrip
    Friend WithEvents DeleteToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ErrorProvider As System.Windows.Forms.ErrorProvider
    Friend WithEvents tabMain As System.Windows.Forms.TabControl
    Friend WithEvents tabGeneralConfig As System.Windows.Forms.TabPage
    Friend WithEvents cboCycle As System.Windows.Forms.ComboBox
    Friend WithEvents lblCycle As System.Windows.Forms.Label
    Friend WithEvents lblVehiceWeight As System.Windows.Forms.Label
    Friend WithEvents txtVehicleWeightKG As System.Windows.Forms.TextBox
    Friend WithEvents tabElectricalConfig As System.Windows.Forms.TabPage
    Friend WithEvents gvResultsCardOverrun As System.Windows.Forms.DataGridView
    Friend WithEvents gvResultsCardTraction As System.Windows.Forms.DataGridView
    Friend WithEvents lblResultsOverrun As System.Windows.Forms.Label
    Friend WithEvents lblResultsTractionOn As System.Windows.Forms.Label
    Friend WithEvents lblResultsIdle As System.Windows.Forms.Label
    Friend WithEvents chkSmartElectricals As System.Windows.Forms.CheckBox
    Friend WithEvents lblElectricalConsumables As System.Windows.Forms.Label
    Friend WithEvents gvElectricalConsumables As System.Windows.Forms.DataGridView
    Friend WithEvents txtDoorActuationTimeSeconds As System.Windows.Forms.TextBox
    Friend WithEvents txtAlternatorGearEfficiency As System.Windows.Forms.TextBox
    Friend WithEvents txtAlternatorMapPath As System.Windows.Forms.TextBox
    Friend WithEvents txtPowernetVoltage As System.Windows.Forms.TextBox
    Friend WithEvents lblDoorActuationTimeSeconds As System.Windows.Forms.Label
    Friend WithEvents lblAlternatorGearEfficiency As System.Windows.Forms.Label
    Friend WithEvents lblAlternatormapPath As System.Windows.Forms.Label
    Friend WithEvents lblPowerNetVoltage As System.Windows.Forms.Label
    Friend WithEvents tabPneumaticConfig As System.Windows.Forms.TabPage
    Friend WithEvents pnlPneumaticsUserInput As System.Windows.Forms.Panel
    Friend WithEvents Button1 As System.Windows.Forms.Button
    Friend WithEvents btnCompressorMap As System.Windows.Forms.Button
    Friend WithEvents lblPneumaticsVariablesTitle As System.Windows.Forms.Label
    Friend WithEvents lblCompressorType As System.Windows.Forms.Label
    Friend WithEvents lblActuationsMap As System.Windows.Forms.Label
    Friend WithEvents chkSmartAirCompression As System.Windows.Forms.CheckBox
    Friend WithEvents lblSmartRegeneration As System.Windows.Forms.Label
    Friend WithEvents chkSmartRegeneration As System.Windows.Forms.CheckBox
    Friend WithEvents lblAdBlueDosing As System.Windows.Forms.Label
    Friend WithEvents chkRetarderBrake As System.Windows.Forms.CheckBox
    Friend WithEvents lblSmartAirCompression As System.Windows.Forms.Label
    Friend WithEvents txtKneelingHeightMillimeters As System.Windows.Forms.TextBox
    Friend WithEvents lblAirSuspensionControl As System.Windows.Forms.Label
    Friend WithEvents cboDoors As System.Windows.Forms.ComboBox
    Friend WithEvents lblRetarderBrake As System.Windows.Forms.Label
    Friend WithEvents cboCompressorType As System.Windows.Forms.ComboBox
    Friend WithEvents txtCompressorMap As System.Windows.Forms.TextBox
    Friend WithEvents lblCompressorGearEfficiency As System.Windows.Forms.Label
    Friend WithEvents txtCompressorGearRatio As System.Windows.Forms.TextBox
    Friend WithEvents lblCompressorGearRatio As System.Windows.Forms.Label
    Friend WithEvents txtCompressorGearEfficiency As System.Windows.Forms.TextBox
    Friend WithEvents lblCompressorMap As System.Windows.Forms.Label
    Friend WithEvents cboAirSuspensionControl As System.Windows.Forms.ComboBox
    Friend WithEvents lblDoors As System.Windows.Forms.Label
    Friend WithEvents cboAdBlueDosing As System.Windows.Forms.ComboBox
    Friend WithEvents lblKneelingHeightMillimeters As System.Windows.Forms.Label
    Friend WithEvents txtActuationsMap As System.Windows.Forms.TextBox
    Friend WithEvents pnlPneumaticAuxillaries As System.Windows.Forms.Panel
    Friend WithEvents lblPneumaticAuxillariesTitle As System.Windows.Forms.Label
    Friend WithEvents lblAdBlueNIperMinute As System.Windows.Forms.Label
    Friend WithEvents lblAirControlledSuspensionNIperMinute As System.Windows.Forms.Label
    Friend WithEvents lblBrakingNoRetarderNIperKG As System.Windows.Forms.Label
    Friend WithEvents lblBrakingWithRetarderNIperKG As System.Windows.Forms.Label
    Friend WithEvents lblBreakingPerKneelingNIperKGinMM As System.Windows.Forms.Label
    Friend WithEvents lblDeadVolBlowOutsPerLitresperHour As System.Windows.Forms.Label
    Friend WithEvents lblDeadVolumeLitres As System.Windows.Forms.Label
    Friend WithEvents lblNonSmartRegenFractionTotalAirDemand As System.Windows.Forms.Label
    Friend WithEvents lblOverrunUtilisationForCompressionFraction As System.Windows.Forms.Label
    Friend WithEvents lblPerDoorOpeningNI As System.Windows.Forms.Label
    Friend WithEvents lblPerStopBrakeActuationNIperKG As System.Windows.Forms.Label
    Friend WithEvents lblSmartRegenFractionTotalAirDemand As System.Windows.Forms.Label
    Friend WithEvents txtAdBlueNIperMinute As System.Windows.Forms.TextBox
    Friend WithEvents txtAirControlledSuspensionNIperMinute As System.Windows.Forms.TextBox
    Friend WithEvents txtBrakingNoRetarderNIperKG As System.Windows.Forms.TextBox
    Friend WithEvents txtBrakingWithRetarderNIperKG As System.Windows.Forms.TextBox
    Friend WithEvents txtBreakingPerKneelingNIperKGinMM As System.Windows.Forms.TextBox
    Friend WithEvents txtDeadVolBlowOutsPerLitresperHour As System.Windows.Forms.TextBox
    Friend WithEvents txtDeadVolumeLitres As System.Windows.Forms.TextBox
    Friend WithEvents txtNonSmartRegenFractionTotalAirDemand As System.Windows.Forms.TextBox
    Friend WithEvents txtOverrunUtilisationForCompressionFraction As System.Windows.Forms.TextBox
    Friend WithEvents txtPerDoorOpeningNI As System.Windows.Forms.TextBox
    Friend WithEvents txtPerStopBrakeActuationNIperKG As System.Windows.Forms.TextBox
    Friend WithEvents txtSmartRegenFractionTotalAirDemand As System.Windows.Forms.TextBox
    Friend WithEvents tabHVACConfig As System.Windows.Forms.TabPage
    Friend WithEvents tabPlayground As System.Windows.Forms.TabPage
    Friend WithEvents btnAlternatorMapPath As System.Windows.Forms.Button
    Friend WithEvents gvResultsCardIdle As System.Windows.Forms.DataGridView
    Friend WithEvents txtHVACFuellingLitresPerHour As System.Windows.Forms.TextBox
    Friend WithEvents lblHVACFuellingLitresPerHour As System.Windows.Forms.Label
    Friend WithEvents txtHVACMechanicalLoadPowerWatts As System.Windows.Forms.TextBox
    Friend WithEvents lblHVACMechanicalLoadPowerWatts As System.Windows.Forms.Label
    Friend WithEvents txtHVACElectricalLoadPowerWatts As System.Windows.Forms.TextBox
    Friend WithEvents lblHVACElectricalLoadPowerWatts As System.Windows.Forms.Label
    Friend WithEvents lblHVACTitle As System.Windows.Forms.Label
    Friend WithEvents lblM0Outputs As System.Windows.Forms.Label
    Friend WithEvents pnl_M0_Displays As System.Windows.Forms.Panel
    Friend WithEvents lblAlternatorsEfficiency As System.Windows.Forms.Label
    Friend WithEvents txtM0_Out_AlternatorsEfficiency As System.Windows.Forms.TextBox
    Friend WithEvents txtM0_Out_HVacElectricalCurrentDemand As System.Windows.Forms.TextBox
    Friend WithEvents lblOutHVACElectricalCurrentDemand As System.Windows.Forms.Label
    Friend WithEvents Timer1 As System.Windows.Forms.Timer
    Friend WithEvents pnl_M05_Displays As System.Windows.Forms.Panel
    Friend WithEvents lblM05SmartalternatorSetEfficiency As System.Windows.Forms.Label
    Friend WithEvents lblM05_AlternatorsEfficiencyOverrun As System.Windows.Forms.Label
    Friend WithEvents lblM05SmartOverrunCurrent As System.Windows.Forms.Label
    Friend WithEvents lblM05_AlternatorsEfficiencyTraction As System.Windows.Forms.Label
    Friend WithEvents lblM05_SmartTractionCurrent As System.Windows.Forms.Label
    Friend WithEvents lblM05AlternatorsEfficiencyIdle As System.Windows.Forms.Label
    Friend WithEvents lblM05_SmartIdleCurrent As System.Windows.Forms.Label
    Friend WithEvents txtM05_out_AlternatorsEfficiencyOverrun As System.Windows.Forms.TextBox
    Friend WithEvents txtM05_out_SmartOverrunCurrent As System.Windows.Forms.TextBox
    Friend WithEvents txtM05_out_AlternatorsEfficiencyTraction As System.Windows.Forms.TextBox
    Friend WithEvents txtM05_out_SmartTractionCurrent As System.Windows.Forms.TextBox
    Friend WithEvents txtM05_Out_AlternatorsEfficiencyIdle As System.Windows.Forms.TextBox
    Friend WithEvents txtM05_OutSmartIdleCurrent As System.Windows.Forms.TextBox
    Friend WithEvents lblM1_HVACAverageLoad As System.Windows.Forms.Label
    Friend WithEvents pnl_M1_Displays As System.Windows.Forms.Panel
    Friend WithEvents lblM1_HVACFuelling As System.Windows.Forms.Label
    Friend WithEvents lblM1_AvgPowerDemandAtCrankHVACElectrics As System.Windows.Forms.Label
    Friend WithEvents lblM1_AveragePowerDemandAtAlternatorElectrics As System.Windows.Forms.Label
    Friend WithEvents lblM1_AveragePowerDemandAtCrank As System.Windows.Forms.Label
    Friend WithEvents txtM1_out_HVACFuelling As System.Windows.Forms.TextBox
    Friend WithEvents txtM1_out_AvgPwrAtCrankFromHVACElec As System.Windows.Forms.TextBox
    Friend WithEvents txtM1_out_AvgPowerDemandAtAlternatorHvacElectrics As System.Windows.Forms.TextBox
    Friend WithEvents txtM1_out_AvgPowerDemandAtCrankMech As System.Windows.Forms.TextBox
    Friend WithEvents lblM2AverageElectricalLoadTitle As System.Windows.Forms.Label
    Friend WithEvents pnl_M2_Displays As System.Windows.Forms.Panel
    Friend WithEvents lblM2_AvgPwrDmdAtCrankFromElectrics As System.Windows.Forms.Label
    Friend WithEvents lblM2_AveragePowerDemandAtAlternatorFromElectrics As System.Windows.Forms.Label
    Friend WithEvents txtM2_out_AvgPowerAtCrankFromElectrics As System.Windows.Forms.TextBox
    Friend WithEvents txtM2_out_AvgPowerAtAltFromElectrics As System.Windows.Forms.TextBox
    Friend WithEvents lblM3_AveragePneumaticLoad As System.Windows.Forms.Label
    Friend WithEvents pnl_M3_Displays As System.Windows.Forms.Panel
    Friend WithEvents lblM3_TotAirConsumptionPerCycleLitres As System.Windows.Forms.Label
    Friend WithEvents lbl_M3_AvgPowerAtCrankFromPneumatics As System.Windows.Forms.Label
    Friend WithEvents txtM3_out_TotalAirConsumedPerCycleInLitres As System.Windows.Forms.TextBox
    Friend WithEvents txtM3_out_AveragePowerAtCrankFromPneumatics As System.Windows.Forms.TextBox
    Friend WithEvents lblM4_AirCompressor As System.Windows.Forms.Label
    Friend WithEvents Panel1 As System.Windows.Forms.Panel
    Friend WithEvents lblM4_PowerAtCrankFromPSCompressorON As System.Windows.Forms.Label
    Friend WithEvents lblM4_PowerAtCrankFromPSCompressorOFF As System.Windows.Forms.Label
    Friend WithEvents lblM4_CompressorOnOffPowerDelta As System.Windows.Forms.Label
    Friend WithEvents lblM4_CompressorFlowRate As System.Windows.Forms.Label
    Friend WithEvents txtM4_out_PowerAtCrankFromPneumaticsCompressorON As System.Windows.Forms.TextBox
    Friend WithEvents txtM4_out_PowerAtCrankFromPneumaticsCompressorOFF As System.Windows.Forms.TextBox
    Friend WithEvents txtM4_out_CompresssorPwrOnMinusPwrOff As System.Windows.Forms.TextBox
    Friend WithEvents txtM4_out_CompressorFlowRate As System.Windows.Forms.TextBox
    Friend WithEvents lblM5_SmartAltSetGeneration As System.Windows.Forms.Label
    Friend WithEvents Panel3 As System.Windows.Forms.Panel
    Friend WithEvents M5_AlternatorGenerationPowerAtCrankOverrunWatts As System.Windows.Forms.Label
    Friend WithEvents txtM5_out_AltRegenPowerAtCrankOverrunWatts As System.Windows.Forms.TextBox
    Friend WithEvents lblM5_AltRegenPowerAtCrankIdleW As System.Windows.Forms.Label
    Friend WithEvents txtM5_out_AltRegenPowerAtCrankTractionWatts As System.Windows.Forms.TextBox
    Friend WithEvents txtM5_out_AltRegenPowerAtCrankIdleWatts As System.Windows.Forms.TextBox
    Friend WithEvents lblM5_AlternatorRegenPowerAtCrankTractionWatts As System.Windows.Forms.Label
    Friend WithEvents PictureBox1 As System.Windows.Forms.PictureBox
    Friend WithEvents btnFinish As System.Windows.Forms.Button
    Friend WithEvents btnStart As System.Windows.Forms.Button
    Friend WithEvents chkClutchEngaged As System.Windows.Forms.CheckBox
    Friend WithEvents lblTotalCycleTimeSeconds As System.Windows.Forms.Label
    Friend WithEvents lblEngineSpeed As System.Windows.Forms.Label
    Friend WithEvents lblEngineMotoringPower As System.Windows.Forms.Label
    Friend WithEvents lblEngineDrivelineTorque As System.Windows.Forms.Label
    Friend WithEvents lblEngineDrivelinePower As System.Windows.Forms.Label
    Friend WithEvents txtTotalCycleTimeSeconds As System.Windows.Forms.TextBox
    Friend WithEvents txtEngineSpeed As System.Windows.Forms.TextBox
    Friend WithEvents txtEngineMotoringPower As System.Windows.Forms.TextBox
    Friend WithEvents txtEngineDrivelineTorque As System.Windows.Forms.TextBox
    Friend WithEvents txtEngineDrivelinePower As System.Windows.Forms.TextBox

End Class
