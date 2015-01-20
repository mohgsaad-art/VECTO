<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmAuxiliaryConfig
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmAuxiliaryConfig))
        Dim DataGridViewCellStyle1 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle2 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle3 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle4 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle5 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle6 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle7 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle8 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle9 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle10 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle11 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle12 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Me.pnlMain = New System.Windows.Forms.Panel()
        Me.btnCancel = New System.Windows.Forms.Button()
        Me.btnSave = New System.Windows.Forms.Button()
        Me.tabMain = New System.Windows.Forms.TabControl()
        Me.tabGeneralConfig = New System.Windows.Forms.TabPage()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.btnFuelMap = New System.Windows.Forms.Button()
        Me.lblFuelMap = New System.Windows.Forms.Label()
        Me.txtFuelMap = New System.Windows.Forms.TextBox()
        Me.cboCycle = New System.Windows.Forms.ComboBox()
        Me.txtVehicleWeightKG = New System.Windows.Forms.TextBox()
        Me.tabElectricalConfig = New System.Windows.Forms.TabPage()
        Me.btnAALTOpen = New System.Windows.Forms.Button()
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
        Me.btnOpenAPAC = New System.Windows.Forms.Button()
        Me.btnOpenACMP = New System.Windows.Forms.Button()
        Me.btnActuationsMap = New System.Windows.Forms.Button()
        Me.btnCompressorMap = New System.Windows.Forms.Button()
        Me.lblPneumaticsVariablesTitle = New System.Windows.Forms.Label()
        Me.lblActuationsMap = New System.Windows.Forms.Label()
        Me.chkSmartAirCompression = New System.Windows.Forms.CheckBox()
        Me.chkSmartRegeneration = New System.Windows.Forms.CheckBox()
        Me.lblAdBlueDosing = New System.Windows.Forms.Label()
        Me.chkRetarderBrake = New System.Windows.Forms.CheckBox()
        Me.txtKneelingHeightMillimeters = New System.Windows.Forms.TextBox()
        Me.lblAirSuspensionControl = New System.Windows.Forms.Label()
        Me.cboDoors = New System.Windows.Forms.ComboBox()
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
        Me.btnOpenAHSM = New System.Windows.Forms.Button()
        Me.btnSSMBSource = New System.Windows.Forms.Button()
        Me.lblSSMFilePath = New System.Windows.Forms.Label()
        Me.txtSSMFilePath = New System.Windows.Forms.TextBox()
        Me.lblHVACTitle = New System.Windows.Forms.Label()
        Me.txtHVACFuellingLitresPerHour = New System.Windows.Forms.TextBox()
        Me.lblHVACFuellingLitresPerHour = New System.Windows.Forms.Label()
        Me.txtHVACMechanicalLoadPowerWatts = New System.Windows.Forms.TextBox()
        Me.lblHVACMechanicalLoadPowerWatts = New System.Windows.Forms.Label()
        Me.txtHVACElectricalLoadPowerWatts = New System.Windows.Forms.TextBox()
        Me.lblHVACElectricalLoadPowerWatts = New System.Windows.Forms.Label()
        Me.resultCardContextMenu = New System.Windows.Forms.ContextMenuStrip(Me.components)
        Me.DeleteToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ErrorProvider = New System.Windows.Forms.ErrorProvider(Me.components)
        Me.Timer1 = New System.Windows.Forms.Timer(Me.components)
        Me.CmFiles = New System.Windows.Forms.ContextMenuStrip(Me.components)
        Me.OpenWithToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ShowInFolderToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
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
        Me.resultCardContextMenu.SuspendLayout
        CType(Me.ErrorProvider,System.ComponentModel.ISupportInitialize).BeginInit
        Me.CmFiles.SuspendLayout
        Me.SuspendLayout
        '
        'pnlMain
        '
        Me.pnlMain.Controls.Add(Me.btnCancel)
        Me.pnlMain.Controls.Add(Me.btnSave)
        Me.pnlMain.Controls.Add(Me.tabMain)
        Me.pnlMain.Dock = System.Windows.Forms.DockStyle.Fill
        Me.pnlMain.Location = New System.Drawing.Point(0, 0)
        Me.pnlMain.Name = "pnlMain"
        Me.pnlMain.Size = New System.Drawing.Size(933, 683)
        Me.pnlMain.TabIndex = 1
        '
        'btnCancel
        '
        Me.btnCancel.Location = New System.Drawing.Point(843, 645)
        Me.btnCancel.Name = "btnCancel"
        Me.btnCancel.Size = New System.Drawing.Size(75, 23)
        Me.btnCancel.TabIndex = 5
        Me.btnCancel.Text = "Cancel"
        Me.btnCancel.UseVisualStyleBackColor = true
        '
        'btnSave
        '
        Me.btnSave.Location = New System.Drawing.Point(745, 645)
        Me.btnSave.Name = "btnSave"
        Me.btnSave.Size = New System.Drawing.Size(75, 23)
        Me.btnSave.TabIndex = 10
        Me.btnSave.Text = "Save"
        Me.btnSave.UseVisualStyleBackColor = true
        '
        'tabMain
        '
        Me.tabMain.AccessibleDescription = ""
        Me.tabMain.Controls.Add(Me.tabGeneralConfig)
        Me.tabMain.Controls.Add(Me.tabElectricalConfig)
        Me.tabMain.Controls.Add(Me.tabPneumaticConfig)
        Me.tabMain.Controls.Add(Me.tabHVACConfig)
        Me.tabMain.DrawMode = System.Windows.Forms.TabDrawMode.OwnerDrawFixed
        Me.tabMain.Location = New System.Drawing.Point(12, 12)
        Me.tabMain.Name = "tabMain"
        Me.tabMain.SelectedIndex = 0
        Me.tabMain.Size = New System.Drawing.Size(909, 613)
        Me.tabMain.TabIndex = 0
        Me.tabMain.Tag = ""
        '
        'tabGeneralConfig
        '
        Me.tabGeneralConfig.Controls.Add(Me.Label1)
        Me.tabGeneralConfig.Controls.Add(Me.btnFuelMap)
        Me.tabGeneralConfig.Controls.Add(Me.lblFuelMap)
        Me.tabGeneralConfig.Controls.Add(Me.txtFuelMap)
        Me.tabGeneralConfig.Controls.Add(Me.cboCycle)
        Me.tabGeneralConfig.Controls.Add(Me.txtVehicleWeightKG)
        Me.tabGeneralConfig.Location = New System.Drawing.Point(4, 22)
        Me.tabGeneralConfig.Name = "tabGeneralConfig"
        Me.tabGeneralConfig.Padding = New System.Windows.Forms.Padding(3)
        Me.tabGeneralConfig.Size = New System.Drawing.Size(901, 704)
        Me.tabGeneralConfig.TabIndex = 0
        Me.tabGeneralConfig.Text = "General"
        Me.tabGeneralConfig.UseVisualStyleBackColor = true
        '
        'Label1
        '
        Me.Label1.AutoSize = true
        Me.Label1.Font = New System.Drawing.Font("Microsoft Sans Serif", 14!, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, CType(0,Byte))
        Me.Label1.ForeColor = System.Drawing.Color.Lime
        Me.Label1.Location = New System.Drawing.Point(82, 59)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(364, 24)
        Me.Label1.TabIndex = 10
        Me.Label1.Text = "This Area Is reserved for future expansion."
        '
        'btnFuelMap
        '
        Me.btnFuelMap.Location = New System.Drawing.Point(791, 204)
        Me.btnFuelMap.Name = "btnFuelMap"
        Me.btnFuelMap.Size = New System.Drawing.Size(38, 23)
        Me.btnFuelMap.TabIndex = 9
        Me.btnFuelMap.Text = ". . . "
        Me.btnFuelMap.UseVisualStyleBackColor = true
        Me.btnFuelMap.Visible = false
        '
        'lblFuelMap
        '
        Me.lblFuelMap.AutoSize = true
        Me.lblFuelMap.Location = New System.Drawing.Point(79, 205)
        Me.lblFuelMap.Name = "lblFuelMap"
        Me.lblFuelMap.Size = New System.Drawing.Size(51, 13)
        Me.lblFuelMap.TabIndex = 8
        Me.lblFuelMap.Text = "Fuel Map"
        Me.lblFuelMap.Visible = false
        '
        'txtFuelMap
        '
        Me.txtFuelMap.Location = New System.Drawing.Point(136, 204)
        Me.txtFuelMap.Name = "txtFuelMap"
        Me.txtFuelMap.Size = New System.Drawing.Size(649, 20)
        Me.txtFuelMap.TabIndex = 7
        Me.txtFuelMap.Visible = false
        '
        'cboCycle
        '
        Me.cboCycle.FormattingEnabled = true
        Me.cboCycle.Items.AddRange(New Object() {"Urban", "Heavy urban", "Suburban", "Interurban", "Coach"})
        Me.cboCycle.Location = New System.Drawing.Point(136, 304)
        Me.cboCycle.Name = "cboCycle"
        Me.cboCycle.Size = New System.Drawing.Size(121, 21)
        Me.cboCycle.TabIndex = 6
        Me.cboCycle.Visible = false
        '
        'txtVehicleWeightKG
        '
        Me.txtVehicleWeightKG.Location = New System.Drawing.Point(136, 244)
        Me.txtVehicleWeightKG.Name = "txtVehicleWeightKG"
        Me.txtVehicleWeightKG.Size = New System.Drawing.Size(100, 20)
        Me.txtVehicleWeightKG.TabIndex = 2
        Me.txtVehicleWeightKG.Visible = false
        '
        'tabElectricalConfig
        '
        Me.tabElectricalConfig.Controls.Add(Me.btnAALTOpen)
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
        Me.tabElectricalConfig.Size = New System.Drawing.Size(901, 587)
        Me.tabElectricalConfig.TabIndex = 1
        Me.tabElectricalConfig.Text = "Electrics"
        Me.tabElectricalConfig.UseVisualStyleBackColor = true
        '
        'btnAALTOpen
        '
        Me.btnAALTOpen.Image = CType(resources.GetObject("btnAALTOpen.Image"),System.Drawing.Image)
        Me.btnAALTOpen.Location = New System.Drawing.Point(541, 43)
        Me.btnAALTOpen.Name = "btnAALTOpen"
        Me.btnAALTOpen.Size = New System.Drawing.Size(28, 20)
        Me.btnAALTOpen.TabIndex = 20
        Me.btnAALTOpen.UseVisualStyleBackColor = true
        '
        'btnAlternatorMapPath
        '
        Me.btnAlternatorMapPath.Location = New System.Drawing.Point(497, 43)
        Me.btnAlternatorMapPath.Name = "btnAlternatorMapPath"
        Me.btnAlternatorMapPath.Size = New System.Drawing.Size(38, 20)
        Me.btnAlternatorMapPath.TabIndex = 19
        Me.btnAlternatorMapPath.Text = ". . ."
        Me.btnAlternatorMapPath.UseVisualStyleBackColor = true
        '
        'gvResultsCardOverrun
        '
        DataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft
        DataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control
        DataGridViewCellStyle1.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0,Byte))
        DataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText
        DataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight
        DataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText
        DataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.[True]
        Me.gvResultsCardOverrun.ColumnHeadersDefaultCellStyle = DataGridViewCellStyle1
        Me.gvResultsCardOverrun.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        DataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft
        DataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window
        DataGridViewCellStyle2.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0,Byte))
        DataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText
        DataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight
        DataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText
        DataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.[False]
        Me.gvResultsCardOverrun.DefaultCellStyle = DataGridViewCellStyle2
        Me.gvResultsCardOverrun.Location = New System.Drawing.Point(590, 451)
        Me.gvResultsCardOverrun.Name = "gvResultsCardOverrun"
        DataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft
        DataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Control
        DataGridViewCellStyle3.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0,Byte))
        DataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.WindowText
        DataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight
        DataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText
        DataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.[True]
        Me.gvResultsCardOverrun.RowHeadersDefaultCellStyle = DataGridViewCellStyle3
        Me.gvResultsCardOverrun.Size = New System.Drawing.Size(246, 125)
        Me.gvResultsCardOverrun.TabIndex = 18
        '
        'gvResultsCardTraction
        '
        DataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft
        DataGridViewCellStyle4.BackColor = System.Drawing.SystemColors.Control
        DataGridViewCellStyle4.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0,Byte))
        DataGridViewCellStyle4.ForeColor = System.Drawing.SystemColors.WindowText
        DataGridViewCellStyle4.SelectionBackColor = System.Drawing.SystemColors.Highlight
        DataGridViewCellStyle4.SelectionForeColor = System.Drawing.SystemColors.HighlightText
        DataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.[True]
        Me.gvResultsCardTraction.ColumnHeadersDefaultCellStyle = DataGridViewCellStyle4
        Me.gvResultsCardTraction.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        DataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft
        DataGridViewCellStyle5.BackColor = System.Drawing.SystemColors.Window
        DataGridViewCellStyle5.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0,Byte))
        DataGridViewCellStyle5.ForeColor = System.Drawing.SystemColors.ControlText
        DataGridViewCellStyle5.SelectionBackColor = System.Drawing.SystemColors.Highlight
        DataGridViewCellStyle5.SelectionForeColor = System.Drawing.SystemColors.HighlightText
        DataGridViewCellStyle5.WrapMode = System.Windows.Forms.DataGridViewTriState.[False]
        Me.gvResultsCardTraction.DefaultCellStyle = DataGridViewCellStyle5
        Me.gvResultsCardTraction.Location = New System.Drawing.Point(311, 451)
        Me.gvResultsCardTraction.Name = "gvResultsCardTraction"
        DataGridViewCellStyle6.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft
        DataGridViewCellStyle6.BackColor = System.Drawing.SystemColors.Control
        DataGridViewCellStyle6.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0,Byte))
        DataGridViewCellStyle6.ForeColor = System.Drawing.SystemColors.WindowText
        DataGridViewCellStyle6.SelectionBackColor = System.Drawing.SystemColors.Highlight
        DataGridViewCellStyle6.SelectionForeColor = System.Drawing.SystemColors.HighlightText
        DataGridViewCellStyle6.WrapMode = System.Windows.Forms.DataGridViewTriState.[True]
        Me.gvResultsCardTraction.RowHeadersDefaultCellStyle = DataGridViewCellStyle6
        Me.gvResultsCardTraction.Size = New System.Drawing.Size(258, 125)
        Me.gvResultsCardTraction.TabIndex = 17
        '
        'gvResultsCardIdle
        '
        DataGridViewCellStyle7.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft
        DataGridViewCellStyle7.BackColor = System.Drawing.SystemColors.Control
        DataGridViewCellStyle7.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0,Byte))
        DataGridViewCellStyle7.ForeColor = System.Drawing.SystemColors.WindowText
        DataGridViewCellStyle7.SelectionBackColor = System.Drawing.SystemColors.Highlight
        DataGridViewCellStyle7.SelectionForeColor = System.Drawing.SystemColors.HighlightText
        DataGridViewCellStyle7.WrapMode = System.Windows.Forms.DataGridViewTriState.[True]
        Me.gvResultsCardIdle.ColumnHeadersDefaultCellStyle = DataGridViewCellStyle7
        Me.gvResultsCardIdle.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        DataGridViewCellStyle8.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft
        DataGridViewCellStyle8.BackColor = System.Drawing.SystemColors.Window
        DataGridViewCellStyle8.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0,Byte))
        DataGridViewCellStyle8.ForeColor = System.Drawing.SystemColors.ControlText
        DataGridViewCellStyle8.SelectionBackColor = System.Drawing.SystemColors.Highlight
        DataGridViewCellStyle8.SelectionForeColor = System.Drawing.SystemColors.HighlightText
        DataGridViewCellStyle8.WrapMode = System.Windows.Forms.DataGridViewTriState.[False]
        Me.gvResultsCardIdle.DefaultCellStyle = DataGridViewCellStyle8
        Me.gvResultsCardIdle.Location = New System.Drawing.Point(35, 451)
        Me.gvResultsCardIdle.Name = "gvResultsCardIdle"
        DataGridViewCellStyle9.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft
        DataGridViewCellStyle9.BackColor = System.Drawing.SystemColors.Control
        DataGridViewCellStyle9.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0,Byte))
        DataGridViewCellStyle9.ForeColor = System.Drawing.SystemColors.WindowText
        DataGridViewCellStyle9.SelectionBackColor = System.Drawing.SystemColors.Highlight
        DataGridViewCellStyle9.SelectionForeColor = System.Drawing.SystemColors.HighlightText
        DataGridViewCellStyle9.WrapMode = System.Windows.Forms.DataGridViewTriState.[True]
        Me.gvResultsCardIdle.RowHeadersDefaultCellStyle = DataGridViewCellStyle9
        Me.gvResultsCardIdle.Size = New System.Drawing.Size(256, 125)
        Me.gvResultsCardIdle.TabIndex = 16
        '
        'lblResultsOverrun
        '
        Me.lblResultsOverrun.AutoSize = true
        Me.lblResultsOverrun.Location = New System.Drawing.Point(587, 435)
        Me.lblResultsOverrun.Name = "lblResultsOverrun"
        Me.lblResultsOverrun.Size = New System.Drawing.Size(109, 13)
        Me.lblResultsOverrun.TabIndex = 15
        Me.lblResultsOverrun.Text = "Result Card : Overrun"
        '
        'lblResultsTractionOn
        '
        Me.lblResultsTractionOn.AutoSize = true
        Me.lblResultsTractionOn.Location = New System.Drawing.Point(308, 435)
        Me.lblResultsTractionOn.Name = "lblResultsTractionOn"
        Me.lblResultsTractionOn.Size = New System.Drawing.Size(124, 13)
        Me.lblResultsTractionOn.TabIndex = 14
        Me.lblResultsTractionOn.Text = "Result Card : TractionOn"
        '
        'lblResultsIdle
        '
        Me.lblResultsIdle.AutoSize = true
        Me.lblResultsIdle.Location = New System.Drawing.Point(33, 435)
        Me.lblResultsIdle.Name = "lblResultsIdle"
        Me.lblResultsIdle.Size = New System.Drawing.Size(91, 13)
        Me.lblResultsIdle.TabIndex = 13
        Me.lblResultsIdle.Text = "Result Card :  Idle"
        '
        'chkSmartElectricals
        '
        Me.chkSmartElectricals.AutoSize = true
        Me.chkSmartElectricals.Location = New System.Drawing.Point(162, 132)
        Me.chkSmartElectricals.Name = "chkSmartElectricals"
        Me.chkSmartElectricals.Size = New System.Drawing.Size(96, 17)
        Me.chkSmartElectricals.TabIndex = 12
        Me.chkSmartElectricals.Text = "Smart Electrics"
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
        DataGridViewCellStyle10.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft
        DataGridViewCellStyle10.BackColor = System.Drawing.SystemColors.Control
        DataGridViewCellStyle10.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0,Byte))
        DataGridViewCellStyle10.ForeColor = System.Drawing.SystemColors.WindowText
        DataGridViewCellStyle10.SelectionBackColor = System.Drawing.SystemColors.Highlight
        DataGridViewCellStyle10.SelectionForeColor = System.Drawing.SystemColors.HighlightText
        DataGridViewCellStyle10.WrapMode = System.Windows.Forms.DataGridViewTriState.[True]
        Me.gvElectricalConsumables.ColumnHeadersDefaultCellStyle = DataGridViewCellStyle10
        Me.gvElectricalConsumables.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        DataGridViewCellStyle11.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft
        DataGridViewCellStyle11.BackColor = System.Drawing.SystemColors.Window
        DataGridViewCellStyle11.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0,Byte))
        DataGridViewCellStyle11.ForeColor = System.Drawing.SystemColors.ControlText
        DataGridViewCellStyle11.SelectionBackColor = System.Drawing.SystemColors.Highlight
        DataGridViewCellStyle11.SelectionForeColor = System.Drawing.SystemColors.HighlightText
        DataGridViewCellStyle11.WrapMode = System.Windows.Forms.DataGridViewTriState.[False]
        Me.gvElectricalConsumables.DefaultCellStyle = DataGridViewCellStyle11
        Me.gvElectricalConsumables.Location = New System.Drawing.Point(33, 170)
        Me.gvElectricalConsumables.Name = "gvElectricalConsumables"
        DataGridViewCellStyle12.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft
        DataGridViewCellStyle12.BackColor = System.Drawing.SystemColors.Control
        DataGridViewCellStyle12.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0,Byte))
        DataGridViewCellStyle12.ForeColor = System.Drawing.SystemColors.WindowText
        DataGridViewCellStyle12.SelectionBackColor = System.Drawing.SystemColors.Highlight
        DataGridViewCellStyle12.SelectionForeColor = System.Drawing.SystemColors.HighlightText
        DataGridViewCellStyle12.WrapMode = System.Windows.Forms.DataGridViewTriState.[True]
        Me.gvElectricalConsumables.RowHeadersDefaultCellStyle = DataGridViewCellStyle12
        Me.gvElectricalConsumables.Size = New System.Drawing.Size(803, 250)
        Me.gvElectricalConsumables.TabIndex = 10
        '
        'txtDoorActuationTimeSeconds
        '
        Me.txtDoorActuationTimeSeconds.Location = New System.Drawing.Point(162, 101)
        Me.txtDoorActuationTimeSeconds.Name = "txtDoorActuationTimeSeconds"
        Me.txtDoorActuationTimeSeconds.Size = New System.Drawing.Size(100, 20)
        Me.txtDoorActuationTimeSeconds.TabIndex = 9
        '
        'txtAlternatorGearEfficiency
        '
        Me.txtAlternatorGearEfficiency.Location = New System.Drawing.Point(162, 71)
        Me.txtAlternatorGearEfficiency.Name = "txtAlternatorGearEfficiency"
        Me.txtAlternatorGearEfficiency.Size = New System.Drawing.Size(100, 20)
        Me.txtAlternatorGearEfficiency.TabIndex = 6
        '
        'txtAlternatorMapPath
        '
        Me.txtAlternatorMapPath.Location = New System.Drawing.Point(162, 43)
        Me.txtAlternatorMapPath.Name = "txtAlternatorMapPath"
        Me.txtAlternatorMapPath.Size = New System.Drawing.Size(319, 20)
        Me.txtAlternatorMapPath.TabIndex = 4
        '
        'txtPowernetVoltage
        '
        Me.txtPowernetVoltage.Location = New System.Drawing.Point(162, 16)
        Me.txtPowernetVoltage.Name = "txtPowernetVoltage"
        Me.txtPowernetVoltage.Size = New System.Drawing.Size(100, 20)
        Me.txtPowernetVoltage.TabIndex = 2
        '
        'lblDoorActuationTimeSeconds
        '
        Me.lblDoorActuationTimeSeconds.AutoSize = true
        Me.lblDoorActuationTimeSeconds.Location = New System.Drawing.Point(30, 105)
        Me.lblDoorActuationTimeSeconds.Name = "lblDoorActuationTimeSeconds"
        Me.lblDoorActuationTimeSeconds.Size = New System.Drawing.Size(114, 13)
        Me.lblDoorActuationTimeSeconds.TabIndex = 8
        Me.lblDoorActuationTimeSeconds.Text = "Door ActuationTime(S)"
        '
        'lblAlternatorGearEfficiency
        '
        Me.lblAlternatorGearEfficiency.AutoSize = true
        Me.lblAlternatorGearEfficiency.Location = New System.Drawing.Point(30, 75)
        Me.lblAlternatorGearEfficiency.Name = "lblAlternatorGearEfficiency"
        Me.lblAlternatorGearEfficiency.Size = New System.Drawing.Size(132, 13)
        Me.lblAlternatorGearEfficiency.TabIndex = 7
        Me.lblAlternatorGearEfficiency.Text = "Alternator Pulley Efficiency"
        '
        'lblAlternatormapPath
        '
        Me.lblAlternatormapPath.AutoSize = true
        Me.lblAlternatormapPath.Location = New System.Drawing.Point(30, 47)
        Me.lblAlternatormapPath.Name = "lblAlternatormapPath"
        Me.lblAlternatormapPath.Size = New System.Drawing.Size(76, 13)
        Me.lblAlternatormapPath.TabIndex = 5
        Me.lblAlternatormapPath.Text = "Alternator Map"
        '
        'lblPowerNetVoltage
        '
        Me.lblPowerNetVoltage.AutoSize = true
        Me.lblPowerNetVoltage.Location = New System.Drawing.Point(30, 20)
        Me.lblPowerNetVoltage.Name = "lblPowerNetVoltage"
        Me.lblPowerNetVoltage.Size = New System.Drawing.Size(91, 13)
        Me.lblPowerNetVoltage.TabIndex = 3
        Me.lblPowerNetVoltage.Text = "Powernet Voltage"
        '
        'tabPneumaticConfig
        '
        Me.tabPneumaticConfig.Controls.Add(Me.pnlPneumaticsUserInput)
        Me.tabPneumaticConfig.Controls.Add(Me.pnlPneumaticAuxillaries)
        Me.tabPneumaticConfig.Location = New System.Drawing.Point(4, 22)
        Me.tabPneumaticConfig.Name = "tabPneumaticConfig"
        Me.tabPneumaticConfig.Size = New System.Drawing.Size(901, 704)
        Me.tabPneumaticConfig.TabIndex = 2
        Me.tabPneumaticConfig.Text = "Pneumatics"
        Me.tabPneumaticConfig.UseVisualStyleBackColor = true
        '
        'pnlPneumaticsUserInput
        '
        Me.pnlPneumaticsUserInput.Controls.Add(Me.btnOpenAPAC)
        Me.pnlPneumaticsUserInput.Controls.Add(Me.btnOpenACMP)
        Me.pnlPneumaticsUserInput.Controls.Add(Me.btnActuationsMap)
        Me.pnlPneumaticsUserInput.Controls.Add(Me.btnCompressorMap)
        Me.pnlPneumaticsUserInput.Controls.Add(Me.lblPneumaticsVariablesTitle)
        Me.pnlPneumaticsUserInput.Controls.Add(Me.lblActuationsMap)
        Me.pnlPneumaticsUserInput.Controls.Add(Me.chkSmartAirCompression)
        Me.pnlPneumaticsUserInput.Controls.Add(Me.chkSmartRegeneration)
        Me.pnlPneumaticsUserInput.Controls.Add(Me.lblAdBlueDosing)
        Me.pnlPneumaticsUserInput.Controls.Add(Me.chkRetarderBrake)
        Me.pnlPneumaticsUserInput.Controls.Add(Me.txtKneelingHeightMillimeters)
        Me.pnlPneumaticsUserInput.Controls.Add(Me.lblAirSuspensionControl)
        Me.pnlPneumaticsUserInput.Controls.Add(Me.cboDoors)
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
        'btnOpenAPAC
        '
        Me.btnOpenAPAC.Image = CType(resources.GetObject("btnOpenAPAC.Image"),System.Drawing.Image)
        Me.btnOpenAPAC.Location = New System.Drawing.Point(456, 345)
        Me.btnOpenAPAC.Name = "btnOpenAPAC"
        Me.btnOpenAPAC.Size = New System.Drawing.Size(28, 20)
        Me.btnOpenAPAC.TabIndex = 54
        Me.btnOpenAPAC.UseVisualStyleBackColor = true
        '
        'btnOpenACMP
        '
        Me.btnOpenACMP.Image = CType(resources.GetObject("btnOpenACMP.Image"),System.Drawing.Image)
        Me.btnOpenACMP.Location = New System.Drawing.Point(456, 60)
        Me.btnOpenACMP.Name = "btnOpenACMP"
        Me.btnOpenACMP.Size = New System.Drawing.Size(28, 20)
        Me.btnOpenACMP.TabIndex = 53
        Me.btnOpenACMP.UseVisualStyleBackColor = true
        '
        'btnActuationsMap
        '
        Me.btnActuationsMap.Location = New System.Drawing.Point(412, 343)
        Me.btnActuationsMap.Name = "btnActuationsMap"
        Me.btnActuationsMap.Size = New System.Drawing.Size(35, 23)
        Me.btnActuationsMap.TabIndex = 21
        Me.btnActuationsMap.Text = ". . ."
        Me.btnActuationsMap.UseVisualStyleBackColor = true
        '
        'btnCompressorMap
        '
        Me.btnCompressorMap.Location = New System.Drawing.Point(412, 58)
        Me.btnCompressorMap.Name = "btnCompressorMap"
        Me.btnCompressorMap.Size = New System.Drawing.Size(35, 23)
        Me.btnCompressorMap.TabIndex = 13
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
        'lblActuationsMap
        '
        Me.lblActuationsMap.AutoSize = true
        Me.lblActuationsMap.Location = New System.Drawing.Point(13, 348)
        Me.lblActuationsMap.Name = "lblActuationsMap"
        Me.lblActuationsMap.Size = New System.Drawing.Size(81, 13)
        Me.lblActuationsMap.TabIndex = 25
        Me.lblActuationsMap.Text = "Actuations Map"
        '
        'chkSmartAirCompression
        '
        Me.chkSmartAirCompression.CheckAlign = System.Drawing.ContentAlignment.MiddleRight
        Me.chkSmartAirCompression.FlatStyle = System.Windows.Forms.FlatStyle.System
        Me.chkSmartAirCompression.Location = New System.Drawing.Point(16, 430)
        Me.chkSmartAirCompression.MinimumSize = New System.Drawing.Size(150, 0)
        Me.chkSmartAirCompression.Name = "chkSmartAirCompression"
        Me.chkSmartAirCompression.Size = New System.Drawing.Size(153, 22)
        Me.chkSmartAirCompression.TabIndex = 23
        Me.chkSmartAirCompression.Text = "Smart Pneumatics"
        Me.chkSmartAirCompression.UseVisualStyleBackColor = true
        '
        'chkSmartRegeneration
        '
        Me.chkSmartRegeneration.CheckAlign = System.Drawing.ContentAlignment.BottomRight
        Me.chkSmartRegeneration.FlatStyle = System.Windows.Forms.FlatStyle.System
        Me.chkSmartRegeneration.Location = New System.Drawing.Point(16, 469)
        Me.chkSmartRegeneration.MinimumSize = New System.Drawing.Size(150, 0)
        Me.chkSmartRegeneration.Name = "chkSmartRegeneration"
        Me.chkSmartRegeneration.Size = New System.Drawing.Size(153, 22)
        Me.chkSmartRegeneration.TabIndex = 24
        Me.chkSmartRegeneration.Text = "Smart Regeneration"
        Me.chkSmartRegeneration.UseVisualStyleBackColor = true
        '
        'lblAdBlueDosing
        '
        Me.lblAdBlueDosing.AutoSize = true
        Me.lblAdBlueDosing.Location = New System.Drawing.Point(13, 184)
        Me.lblAdBlueDosing.Name = "lblAdBlueDosing"
        Me.lblAdBlueDosing.Size = New System.Drawing.Size(77, 13)
        Me.lblAdBlueDosing.TabIndex = 26
        Me.lblAdBlueDosing.Text = "AdBlue Dosing"
        '
        'chkRetarderBrake
        '
        Me.chkRetarderBrake.CheckAlign = System.Drawing.ContentAlignment.MiddleRight
        Me.chkRetarderBrake.FlatStyle = System.Windows.Forms.FlatStyle.System
        Me.chkRetarderBrake.Location = New System.Drawing.Point(16, 389)
        Me.chkRetarderBrake.MinimumSize = New System.Drawing.Size(150, 0)
        Me.chkRetarderBrake.Name = "chkRetarderBrake"
        Me.chkRetarderBrake.Size = New System.Drawing.Size(153, 22)
        Me.chkRetarderBrake.TabIndex = 22
        Me.chkRetarderBrake.TabStop = false
        Me.chkRetarderBrake.Text = "Retarder Brake"
        Me.chkRetarderBrake.UseVisualStyleBackColor = true
        '
        'txtKneelingHeightMillimeters
        '
        Me.txtKneelingHeightMillimeters.Location = New System.Drawing.Point(156, 304)
        Me.txtKneelingHeightMillimeters.Name = "txtKneelingHeightMillimeters"
        Me.txtKneelingHeightMillimeters.Size = New System.Drawing.Size(120, 20)
        Me.txtKneelingHeightMillimeters.TabIndex = 19
        '
        'lblAirSuspensionControl
        '
        Me.lblAirSuspensionControl.AutoSize = true
        Me.lblAirSuspensionControl.Location = New System.Drawing.Point(13, 225)
        Me.lblAirSuspensionControl.Name = "lblAirSuspensionControl"
        Me.lblAirSuspensionControl.Size = New System.Drawing.Size(113, 13)
        Me.lblAirSuspensionControl.TabIndex = 29
        Me.lblAirSuspensionControl.Text = "Air Suspension Control"
        '
        'cboDoors
        '
        Me.cboDoors.FormattingEnabled = true
        Me.cboDoors.Items.AddRange(New Object() {"<Select>", "Pneumatic", "Electric"})
        Me.cboDoors.Location = New System.Drawing.Point(156, 262)
        Me.cboDoors.Name = "cboDoors"
        Me.cboDoors.Size = New System.Drawing.Size(121, 21)
        Me.cboDoors.TabIndex = 18
        '
        'txtCompressorMap
        '
        Me.txtCompressorMap.Location = New System.Drawing.Point(156, 60)
        Me.txtCompressorMap.Name = "txtCompressorMap"
        Me.txtCompressorMap.Size = New System.Drawing.Size(246, 20)
        Me.txtCompressorMap.TabIndex = 12
        '
        'lblCompressorGearEfficiency
        '
        Me.lblCompressorGearEfficiency.AutoSize = true
        Me.lblCompressorGearEfficiency.Location = New System.Drawing.Point(13, 142)
        Me.lblCompressorGearEfficiency.Name = "lblCompressorGearEfficiency"
        Me.lblCompressorGearEfficiency.Size = New System.Drawing.Size(137, 13)
        Me.lblCompressorGearEfficiency.TabIndex = 30
        Me.lblCompressorGearEfficiency.Text = "Compressor Gear Efficiency"
        '
        'txtCompressorGearRatio
        '
        Me.txtCompressorGearRatio.ForeColor = System.Drawing.Color.Black
        Me.txtCompressorGearRatio.Location = New System.Drawing.Point(156, 98)
        Me.txtCompressorGearRatio.Name = "txtCompressorGearRatio"
        Me.txtCompressorGearRatio.Size = New System.Drawing.Size(121, 20)
        Me.txtCompressorGearRatio.TabIndex = 14
        '
        'lblCompressorGearRatio
        '
        Me.lblCompressorGearRatio.AutoSize = true
        Me.lblCompressorGearRatio.Location = New System.Drawing.Point(13, 99)
        Me.lblCompressorGearRatio.Name = "lblCompressorGearRatio"
        Me.lblCompressorGearRatio.Size = New System.Drawing.Size(116, 13)
        Me.lblCompressorGearRatio.TabIndex = 14
        Me.lblCompressorGearRatio.Text = "Compressor Gear Ratio"
        '
        'txtCompressorGearEfficiency
        '
        Me.txtCompressorGearEfficiency.Location = New System.Drawing.Point(156, 139)
        Me.txtCompressorGearEfficiency.Name = "txtCompressorGearEfficiency"
        Me.txtCompressorGearEfficiency.Size = New System.Drawing.Size(121, 20)
        Me.txtCompressorGearEfficiency.TabIndex = 15
        '
        'lblCompressorMap
        '
        Me.lblCompressorMap.AutoSize = true
        Me.lblCompressorMap.Location = New System.Drawing.Point(13, 60)
        Me.lblCompressorMap.Name = "lblCompressorMap"
        Me.lblCompressorMap.Size = New System.Drawing.Size(86, 13)
        Me.lblCompressorMap.TabIndex = 32
        Me.lblCompressorMap.Text = "Compressor Map"
        '
        'cboAirSuspensionControl
        '
        Me.cboAirSuspensionControl.FormattingEnabled = true
        Me.cboAirSuspensionControl.Items.AddRange(New Object() {"<Select>", "Mechanically", "Electrically"})
        Me.cboAirSuspensionControl.Location = New System.Drawing.Point(156, 220)
        Me.cboAirSuspensionControl.Name = "cboAirSuspensionControl"
        Me.cboAirSuspensionControl.Size = New System.Drawing.Size(121, 21)
        Me.cboAirSuspensionControl.TabIndex = 17
        '
        'lblDoors
        '
        Me.lblDoors.AutoSize = true
        Me.lblDoors.Location = New System.Drawing.Point(13, 266)
        Me.lblDoors.Name = "lblDoors"
        Me.lblDoors.Size = New System.Drawing.Size(79, 13)
        Me.lblDoors.TabIndex = 34
        Me.lblDoors.Text = "Door Operation"
        '
        'cboAdBlueDosing
        '
        Me.cboAdBlueDosing.FormattingEnabled = true
        Me.cboAdBlueDosing.Items.AddRange(New Object() {"<Select>", "Pneumatic", "Electric"})
        Me.cboAdBlueDosing.Location = New System.Drawing.Point(156, 180)
        Me.cboAdBlueDosing.Name = "cboAdBlueDosing"
        Me.cboAdBlueDosing.Size = New System.Drawing.Size(121, 21)
        Me.cboAdBlueDosing.TabIndex = 16
        '
        'lblKneelingHeightMillimeters
        '
        Me.lblKneelingHeightMillimeters.AutoSize = true
        Me.lblKneelingHeightMillimeters.Location = New System.Drawing.Point(13, 308)
        Me.lblKneelingHeightMillimeters.Name = "lblKneelingHeightMillimeters"
        Me.lblKneelingHeightMillimeters.Size = New System.Drawing.Size(133, 13)
        Me.lblKneelingHeightMillimeters.TabIndex = 35
        Me.lblKneelingHeightMillimeters.Text = "Kneeling Height Millimeters"
        '
        'txtActuationsMap
        '
        Me.txtActuationsMap.Location = New System.Drawing.Point(156, 345)
        Me.txtActuationsMap.Name = "txtActuationsMap"
        Me.txtActuationsMap.Size = New System.Drawing.Size(246, 20)
        Me.txtActuationsMap.TabIndex = 20
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
        Me.lblPneumaticAuxillariesTitle.Text = "Pneumatic Auxiliaries Data"
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
        Me.txtAdBlueNIperMinute.Location = New System.Drawing.Point(242, 60)
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
        Me.txtBrakingWithRetarderNIperKG.Location = New System.Drawing.Point(242, 180)
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
        Me.tabHVACConfig.Controls.Add(Me.btnOpenAHSM)
        Me.tabHVACConfig.Controls.Add(Me.btnSSMBSource)
        Me.tabHVACConfig.Controls.Add(Me.lblSSMFilePath)
        Me.tabHVACConfig.Controls.Add(Me.txtSSMFilePath)
        Me.tabHVACConfig.Controls.Add(Me.lblHVACTitle)
        Me.tabHVACConfig.Controls.Add(Me.txtHVACFuellingLitresPerHour)
        Me.tabHVACConfig.Controls.Add(Me.lblHVACFuellingLitresPerHour)
        Me.tabHVACConfig.Controls.Add(Me.txtHVACMechanicalLoadPowerWatts)
        Me.tabHVACConfig.Controls.Add(Me.lblHVACMechanicalLoadPowerWatts)
        Me.tabHVACConfig.Controls.Add(Me.txtHVACElectricalLoadPowerWatts)
        Me.tabHVACConfig.Controls.Add(Me.lblHVACElectricalLoadPowerWatts)
        Me.tabHVACConfig.Location = New System.Drawing.Point(4, 22)
        Me.tabHVACConfig.Name = "tabHVACConfig"
        Me.tabHVACConfig.Size = New System.Drawing.Size(901, 704)
        Me.tabHVACConfig.TabIndex = 3
        Me.tabHVACConfig.Text = "HVAC"
        Me.tabHVACConfig.UseVisualStyleBackColor = true
        '
        'btnOpenAHSM
        '
        Me.btnOpenAHSM.Image = CType(resources.GetObject("btnOpenAHSM.Image"),System.Drawing.Image)
        Me.btnOpenAHSM.Location = New System.Drawing.Point(777, 83)
        Me.btnOpenAHSM.Name = "btnOpenAHSM"
        Me.btnOpenAHSM.Size = New System.Drawing.Size(28, 20)
        Me.btnOpenAHSM.TabIndex = 55
        Me.btnOpenAHSM.UseVisualStyleBackColor = true
        '
        'btnSSMBSource
        '
        Me.btnSSMBSource.Location = New System.Drawing.Point(712, 82)
        Me.btnSSMBSource.Name = "btnSSMBSource"
        Me.btnSSMBSource.Size = New System.Drawing.Size(45, 23)
        Me.btnSSMBSource.TabIndex = 28
        Me.btnSSMBSource.Text = ". . ."
        Me.btnSSMBSource.UseVisualStyleBackColor = true
        '
        'lblSSMFilePath
        '
        Me.lblSSMFilePath.AutoSize = true
        Me.lblSSMFilePath.Location = New System.Drawing.Point(31, 88)
        Me.lblSSMFilePath.Name = "lblSSMFilePath"
        Me.lblSSMFilePath.Size = New System.Drawing.Size(171, 13)
        Me.lblSSMFilePath.TabIndex = 27
        Me.lblSSMFilePath.Text = "Steady State Model File ( .AHSM  )"
        '
        'txtSSMFilePath
        '
        Me.txtSSMFilePath.Location = New System.Drawing.Point(210, 85)
        Me.txtSSMFilePath.Name = "txtSSMFilePath"
        Me.txtSSMFilePath.Size = New System.Drawing.Size(485, 20)
        Me.txtSSMFilePath.TabIndex = 26
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
        Me.txtHVACFuellingLitresPerHour.Location = New System.Drawing.Point(208, 237)
        Me.txtHVACFuellingLitresPerHour.Name = "txtHVACFuellingLitresPerHour"
        Me.txtHVACFuellingLitresPerHour.Size = New System.Drawing.Size(100, 20)
        Me.txtHVACFuellingLitresPerHour.TabIndex = 5
        '
        'lblHVACFuellingLitresPerHour
        '
        Me.lblHVACFuellingLitresPerHour.AutoSize = true
        Me.lblHVACFuellingLitresPerHour.Location = New System.Drawing.Point(31, 241)
        Me.lblHVACFuellingLitresPerHour.Name = "lblHVACFuellingLitresPerHour"
        Me.lblHVACFuellingLitresPerHour.Size = New System.Drawing.Size(116, 13)
        Me.lblHVACFuellingLitresPerHour.TabIndex = 4
        Me.lblHVACFuellingLitresPerHour.Text = "Fuelling Litres Per Hour"
        '
        'txtHVACMechanicalLoadPowerWatts
        '
        Me.txtHVACMechanicalLoadPowerWatts.Location = New System.Drawing.Point(208, 185)
        Me.txtHVACMechanicalLoadPowerWatts.Name = "txtHVACMechanicalLoadPowerWatts"
        Me.txtHVACMechanicalLoadPowerWatts.Size = New System.Drawing.Size(100, 20)
        Me.txtHVACMechanicalLoadPowerWatts.TabIndex = 3
        '
        'lblHVACMechanicalLoadPowerWatts
        '
        Me.lblHVACMechanicalLoadPowerWatts.AutoSize = true
        Me.lblHVACMechanicalLoadPowerWatts.Location = New System.Drawing.Point(31, 189)
        Me.lblHVACMechanicalLoadPowerWatts.Name = "lblHVACMechanicalLoadPowerWatts"
        Me.lblHVACMechanicalLoadPowerWatts.Size = New System.Drawing.Size(153, 13)
        Me.lblHVACMechanicalLoadPowerWatts.TabIndex = 2
        Me.lblHVACMechanicalLoadPowerWatts.Text = "Mechanical Load Power Watts"
        '
        'txtHVACElectricalLoadPowerWatts
        '
        Me.txtHVACElectricalLoadPowerWatts.Location = New System.Drawing.Point(208, 133)
        Me.txtHVACElectricalLoadPowerWatts.Name = "txtHVACElectricalLoadPowerWatts"
        Me.txtHVACElectricalLoadPowerWatts.Size = New System.Drawing.Size(100, 20)
        Me.txtHVACElectricalLoadPowerWatts.TabIndex = 1
        '
        'lblHVACElectricalLoadPowerWatts
        '
        Me.lblHVACElectricalLoadPowerWatts.AutoSize = true
        Me.lblHVACElectricalLoadPowerWatts.Location = New System.Drawing.Point(31, 137)
        Me.lblHVACElectricalLoadPowerWatts.Name = "lblHVACElectricalLoadPowerWatts"
        Me.lblHVACElectricalLoadPowerWatts.Size = New System.Drawing.Size(141, 13)
        Me.lblHVACElectricalLoadPowerWatts.TabIndex = 0
        Me.lblHVACElectricalLoadPowerWatts.Text = "Electrical Load Power Watts"
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
        Me.Timer1.Interval = 1000
        '
        'CmFiles
        '
        Me.CmFiles.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.OpenWithToolStripMenuItem, Me.ShowInFolderToolStripMenuItem})
        Me.CmFiles.Name = "CmOpenFile"
        Me.CmFiles.Size = New System.Drawing.Size(153, 48)
        '
        'OpenWithToolStripMenuItem
        '
        Me.OpenWithToolStripMenuItem.Name = "OpenWithToolStripMenuItem"
        Me.OpenWithToolStripMenuItem.Size = New System.Drawing.Size(152, 22)
        Me.OpenWithToolStripMenuItem.Text = "Open with ..."
        '
        'ShowInFolderToolStripMenuItem
        '
        Me.ShowInFolderToolStripMenuItem.Name = "ShowInFolderToolStripMenuItem"
        Me.ShowInFolderToolStripMenuItem.Size = New System.Drawing.Size(152, 22)
        Me.ShowInFolderToolStripMenuItem.Text = "Open In Folder"
        '
        'frmAuxiliaryConfig
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6!, 13!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.AutoValidate = System.Windows.Forms.AutoValidate.EnableAllowFocusChange
        Me.ClientSize = New System.Drawing.Size(933, 683)
        Me.Controls.Add(Me.pnlMain)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D
        Me.MaximizeBox = false
        Me.MinimizeBox = false
        Me.Name = "frmAuxiliaryConfig"
        Me.Text = "Auxiliaries Configuration"
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
        Me.resultCardContextMenu.ResumeLayout(false)
        CType(Me.ErrorProvider,System.ComponentModel.ISupportInitialize).EndInit
        Me.CmFiles.ResumeLayout(false)
        Me.ResumeLayout(false)

End Sub
    Friend WithEvents pnlMain As System.Windows.Forms.Panel
    Friend WithEvents btnCancel As System.Windows.Forms.Button
    Friend WithEvents resultCardContextMenu As System.Windows.Forms.ContextMenuStrip
    Friend WithEvents DeleteToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ErrorProvider As System.Windows.Forms.ErrorProvider
    Friend WithEvents tabMain As System.Windows.Forms.TabControl
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
    Friend WithEvents btnActuationsMap As System.Windows.Forms.Button
    Friend WithEvents btnCompressorMap As System.Windows.Forms.Button
    Friend WithEvents lblPneumaticsVariablesTitle As System.Windows.Forms.Label
    Friend WithEvents lblActuationsMap As System.Windows.Forms.Label
    Friend WithEvents chkSmartAirCompression As System.Windows.Forms.CheckBox
    Friend WithEvents chkSmartRegeneration As System.Windows.Forms.CheckBox
    Friend WithEvents lblAdBlueDosing As System.Windows.Forms.Label
    Friend WithEvents chkRetarderBrake As System.Windows.Forms.CheckBox
    Friend WithEvents txtKneelingHeightMillimeters As System.Windows.Forms.TextBox
    Friend WithEvents lblAirSuspensionControl As System.Windows.Forms.Label
    Friend WithEvents cboDoors As System.Windows.Forms.ComboBox
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
    Friend WithEvents btnAlternatorMapPath As System.Windows.Forms.Button
    Friend WithEvents gvResultsCardIdle As System.Windows.Forms.DataGridView
    Friend WithEvents txtHVACFuellingLitresPerHour As System.Windows.Forms.TextBox
    Friend WithEvents lblHVACFuellingLitresPerHour As System.Windows.Forms.Label
    Friend WithEvents txtHVACMechanicalLoadPowerWatts As System.Windows.Forms.TextBox
    Friend WithEvents lblHVACMechanicalLoadPowerWatts As System.Windows.Forms.Label
    Friend WithEvents txtHVACElectricalLoadPowerWatts As System.Windows.Forms.TextBox
    Friend WithEvents lblHVACElectricalLoadPowerWatts As System.Windows.Forms.Label
    Friend WithEvents lblHVACTitle As System.Windows.Forms.Label
    Friend WithEvents Timer1 As System.Windows.Forms.Timer
    Friend WithEvents btnSave As System.Windows.Forms.Button
    Friend WithEvents tabGeneralConfig As System.Windows.Forms.TabPage
    Friend WithEvents btnFuelMap As System.Windows.Forms.Button
    Friend WithEvents lblFuelMap As System.Windows.Forms.Label
    Friend WithEvents txtFuelMap As System.Windows.Forms.TextBox
    Friend WithEvents cboCycle As System.Windows.Forms.ComboBox
    Friend WithEvents txtVehicleWeightKG As System.Windows.Forms.TextBox
    Friend WithEvents btnSSMBSource As System.Windows.Forms.Button
    Friend WithEvents lblSSMFilePath As System.Windows.Forms.Label
    Friend WithEvents txtSSMFilePath As System.Windows.Forms.TextBox
    Friend WithEvents btnAALTOpen As System.Windows.Forms.Button
    Friend WithEvents CmFiles As System.Windows.Forms.ContextMenuStrip
    Friend WithEvents OpenWithToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ShowInFolderToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents btnOpenACMP As System.Windows.Forms.Button
    Friend WithEvents btnOpenAPAC As System.Windows.Forms.Button
    Friend WithEvents btnOpenAHSM As System.Windows.Forms.Button
    Friend WithEvents Label1 As System.Windows.Forms.Label

End Class
