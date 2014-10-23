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
        Dim DataGridViewCellStyle13 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle14 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle15 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle16 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle17 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle18 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle19 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle20 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle21 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle22 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle23 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle24 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Me.brcElecConsumers = New System.Windows.Forms.BindingSource(Me.components)
        Me.Panel1 = New System.Windows.Forms.Panel()
        Me.btnCancel = New System.Windows.Forms.Button()
        Me.btnFinish = New System.Windows.Forms.Button()
        Me.btnForward = New System.Windows.Forms.Button()
        Me.btnBack = New System.Windows.Forms.Button()
        Me.btnStart = New System.Windows.Forms.Button()
        Me.tabMain = New System.Windows.Forms.TabControl()
        Me.tabGeneralConfig = New System.Windows.Forms.TabPage()
        Me.cboCycle = New System.Windows.Forms.ComboBox()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.txtVehicleWeightKG = New System.Windows.Forms.TextBox()
        Me.tabElectricalConfig = New System.Windows.Forms.TabPage()
        Me.gvResultsCardOverrun = New System.Windows.Forms.DataGridView()
        Me.gvResultsCardTraction = New System.Windows.Forms.DataGridView()
        Me.gvResultsCardIdle = New System.Windows.Forms.DataGridView()
        Me.Label9 = New System.Windows.Forms.Label()
        Me.Label8 = New System.Windows.Forms.Label()
        Me.Label7 = New System.Windows.Forms.Label()
        Me.chkSmartElectricals = New System.Windows.Forms.CheckBox()
        Me.Label6 = New System.Windows.Forms.Label()
        Me.gvElectricalConsumables = New System.Windows.Forms.DataGridView()
        Me.txtDoorActuationTimeSeconds = New System.Windows.Forms.TextBox()
        Me.txtAlternatorGearEfficiency = New System.Windows.Forms.TextBox()
        Me.txtAlternatorMapPath = New System.Windows.Forms.TextBox()
        Me.txtPowernetVoltage = New System.Windows.Forms.TextBox()
        Me.labelDoorActuationTimeSeconds = New System.Windows.Forms.Label()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.tabPneumaticConfig = New System.Windows.Forms.TabPage()
        Me.pnlPneumaticsUserInput = New System.Windows.Forms.Panel()
        Me.Button1 = New System.Windows.Forms.Button()
        Me.btnCompressorMap = New System.Windows.Forms.Button()
        Me.Label10 = New System.Windows.Forms.Label()
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
        Me.tabPlayground = New System.Windows.Forms.TabPage()
        Me.TabPage1 = New System.Windows.Forms.TabPage()
        Me.resultCardContextMenu = New System.Windows.Forms.ContextMenuStrip(Me.components)
        Me.DeleteToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ErrorProvider = New System.Windows.Forms.ErrorProvider(Me.components)
        CType(Me.brcElecConsumers,System.ComponentModel.ISupportInitialize).BeginInit
        Me.Panel1.SuspendLayout
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
        Me.resultCardContextMenu.SuspendLayout
        CType(Me.ErrorProvider,System.ComponentModel.ISupportInitialize).BeginInit
        Me.SuspendLayout
        '
        'Panel1
        '
        Me.Panel1.Controls.Add(Me.btnCancel)
        Me.Panel1.Controls.Add(Me.btnFinish)
        Me.Panel1.Controls.Add(Me.btnForward)
        Me.Panel1.Controls.Add(Me.btnBack)
        Me.Panel1.Controls.Add(Me.btnStart)
        Me.Panel1.Controls.Add(Me.tabMain)
        Me.Panel1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Panel1.Location = New System.Drawing.Point(0, 0)
        Me.Panel1.Name = "Panel1"
        Me.Panel1.Size = New System.Drawing.Size(945, 712)
        Me.Panel1.TabIndex = 1
        '
        'btnCancel
        '
        Me.btnCancel.Location = New System.Drawing.Point(827, 670)
        Me.btnCancel.Name = "btnCancel"
        Me.btnCancel.Size = New System.Drawing.Size(75, 23)
        Me.btnCancel.TabIndex = 5
        Me.btnCancel.Text = "Cancel"
        Me.btnCancel.UseVisualStyleBackColor = true
        '
        'btnFinish
        '
        Me.btnFinish.Location = New System.Drawing.Point(745, 671)
        Me.btnFinish.Name = "btnFinish"
        Me.btnFinish.Size = New System.Drawing.Size(75, 23)
        Me.btnFinish.TabIndex = 4
        Me.btnFinish.Text = "Finish  >|"
        Me.btnFinish.UseVisualStyleBackColor = true
        '
        'btnForward
        '
        Me.btnForward.Location = New System.Drawing.Point(621, 671)
        Me.btnForward.Name = "btnForward"
        Me.btnForward.Size = New System.Drawing.Size(75, 23)
        Me.btnForward.TabIndex = 3
        Me.btnForward.Text = "Fwd >>"
        Me.btnForward.UseVisualStyleBackColor = true
        '
        'btnBack
        '
        Me.btnBack.Location = New System.Drawing.Point(537, 671)
        Me.btnBack.Name = "btnBack"
        Me.btnBack.Size = New System.Drawing.Size(75, 23)
        Me.btnBack.TabIndex = 2
        Me.btnBack.Text = "<< Back"
        Me.btnBack.UseVisualStyleBackColor = true
        '
        'btnStart
        '
        Me.btnStart.Location = New System.Drawing.Point(420, 671)
        Me.btnStart.Name = "btnStart"
        Me.btnStart.Size = New System.Drawing.Size(75, 23)
        Me.btnStart.TabIndex = 1
        Me.btnStart.Text = "|< Start"
        Me.btnStart.UseVisualStyleBackColor = true
        '
        'tabMain
        '
        Me.tabMain.AccessibleDescription = ""
        Me.tabMain.Controls.Add(Me.tabGeneralConfig)
        Me.tabMain.Controls.Add(Me.tabElectricalConfig)
        Me.tabMain.Controls.Add(Me.tabPneumaticConfig)
        Me.tabMain.Controls.Add(Me.tabHVACConfig)
        Me.tabMain.Controls.Add(Me.tabPlayground)
        Me.tabMain.Controls.Add(Me.TabPage1)
        Me.tabMain.Location = New System.Drawing.Point(12, 12)
        Me.tabMain.Name = "tabMain"
        Me.tabMain.SelectedIndex = 0
        Me.tabMain.Size = New System.Drawing.Size(917, 636)
        Me.tabMain.TabIndex = 0
        Me.tabMain.Tag = ""
        '
        'tabGeneralConfig
        '
        Me.tabGeneralConfig.Controls.Add(Me.cboCycle)
        Me.tabGeneralConfig.Controls.Add(Me.Label3)
        Me.tabGeneralConfig.Controls.Add(Me.Label2)
        Me.tabGeneralConfig.Controls.Add(Me.txtVehicleWeightKG)
        Me.tabGeneralConfig.Location = New System.Drawing.Point(4, 22)
        Me.tabGeneralConfig.Name = "tabGeneralConfig"
        Me.tabGeneralConfig.Padding = New System.Windows.Forms.Padding(3)
        Me.tabGeneralConfig.Size = New System.Drawing.Size(909, 610)
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
        'Label3
        '
        Me.Label3.AutoSize = true
        Me.Label3.Location = New System.Drawing.Point(24, 78)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(33, 13)
        Me.Label3.TabIndex = 5
        Me.Label3.Text = "Cycle"
        '
        'Label2
        '
        Me.Label2.AutoSize = true
        Me.Label2.Location = New System.Drawing.Point(21, 38)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(97, 13)
        Me.Label2.TabIndex = 3
        Me.Label2.Text = "Vehicle Weight KG"
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
        Me.tabElectricalConfig.Controls.Add(Me.gvResultsCardOverrun)
        Me.tabElectricalConfig.Controls.Add(Me.gvResultsCardTraction)
        Me.tabElectricalConfig.Controls.Add(Me.gvResultsCardIdle)
        Me.tabElectricalConfig.Controls.Add(Me.Label9)
        Me.tabElectricalConfig.Controls.Add(Me.Label8)
        Me.tabElectricalConfig.Controls.Add(Me.Label7)
        Me.tabElectricalConfig.Controls.Add(Me.chkSmartElectricals)
        Me.tabElectricalConfig.Controls.Add(Me.Label6)
        Me.tabElectricalConfig.Controls.Add(Me.gvElectricalConsumables)
        Me.tabElectricalConfig.Controls.Add(Me.txtDoorActuationTimeSeconds)
        Me.tabElectricalConfig.Controls.Add(Me.txtAlternatorGearEfficiency)
        Me.tabElectricalConfig.Controls.Add(Me.txtAlternatorMapPath)
        Me.tabElectricalConfig.Controls.Add(Me.txtPowernetVoltage)
        Me.tabElectricalConfig.Controls.Add(Me.labelDoorActuationTimeSeconds)
        Me.tabElectricalConfig.Controls.Add(Me.Label5)
        Me.tabElectricalConfig.Controls.Add(Me.Label4)
        Me.tabElectricalConfig.Controls.Add(Me.Label1)
        Me.tabElectricalConfig.Location = New System.Drawing.Point(4, 22)
        Me.tabElectricalConfig.Name = "tabElectricalConfig"
        Me.tabElectricalConfig.Padding = New System.Windows.Forms.Padding(3)
        Me.tabElectricalConfig.Size = New System.Drawing.Size(909, 610)
        Me.tabElectricalConfig.TabIndex = 1
        Me.tabElectricalConfig.Text = "ElectricalConfig"
        Me.tabElectricalConfig.UseVisualStyleBackColor = true
        '
        'gvResultsCardOverrun
        '
        DataGridViewCellStyle13.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft
        DataGridViewCellStyle13.BackColor = System.Drawing.SystemColors.Control
        DataGridViewCellStyle13.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0,Byte))
        DataGridViewCellStyle13.ForeColor = System.Drawing.SystemColors.WindowText
        DataGridViewCellStyle13.SelectionBackColor = System.Drawing.SystemColors.Highlight
        DataGridViewCellStyle13.SelectionForeColor = System.Drawing.SystemColors.HighlightText
        DataGridViewCellStyle13.WrapMode = System.Windows.Forms.DataGridViewTriState.[True]
        Me.gvResultsCardOverrun.ColumnHeadersDefaultCellStyle = DataGridViewCellStyle13
        Me.gvResultsCardOverrun.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        DataGridViewCellStyle14.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft
        DataGridViewCellStyle14.BackColor = System.Drawing.SystemColors.Window
        DataGridViewCellStyle14.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0,Byte))
        DataGridViewCellStyle14.ForeColor = System.Drawing.SystemColors.ControlText
        DataGridViewCellStyle14.SelectionBackColor = System.Drawing.SystemColors.Highlight
        DataGridViewCellStyle14.SelectionForeColor = System.Drawing.SystemColors.HighlightText
        DataGridViewCellStyle14.WrapMode = System.Windows.Forms.DataGridViewTriState.[False]
        Me.gvResultsCardOverrun.DefaultCellStyle = DataGridViewCellStyle14
        Me.gvResultsCardOverrun.Location = New System.Drawing.Point(590, 451)
        Me.gvResultsCardOverrun.Name = "gvResultsCardOverrun"
        DataGridViewCellStyle15.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft
        DataGridViewCellStyle15.BackColor = System.Drawing.SystemColors.Control
        DataGridViewCellStyle15.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0,Byte))
        DataGridViewCellStyle15.ForeColor = System.Drawing.SystemColors.WindowText
        DataGridViewCellStyle15.SelectionBackColor = System.Drawing.SystemColors.Highlight
        DataGridViewCellStyle15.SelectionForeColor = System.Drawing.SystemColors.HighlightText
        DataGridViewCellStyle15.WrapMode = System.Windows.Forms.DataGridViewTriState.[True]
        Me.gvResultsCardOverrun.RowHeadersDefaultCellStyle = DataGridViewCellStyle15
        Me.gvResultsCardOverrun.Size = New System.Drawing.Size(246, 125)
        Me.gvResultsCardOverrun.TabIndex = 18
        '
        'gvResultsCardTraction
        '
        DataGridViewCellStyle16.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft
        DataGridViewCellStyle16.BackColor = System.Drawing.SystemColors.Control
        DataGridViewCellStyle16.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0,Byte))
        DataGridViewCellStyle16.ForeColor = System.Drawing.SystemColors.WindowText
        DataGridViewCellStyle16.SelectionBackColor = System.Drawing.SystemColors.Highlight
        DataGridViewCellStyle16.SelectionForeColor = System.Drawing.SystemColors.HighlightText
        DataGridViewCellStyle16.WrapMode = System.Windows.Forms.DataGridViewTriState.[True]
        Me.gvResultsCardTraction.ColumnHeadersDefaultCellStyle = DataGridViewCellStyle16
        Me.gvResultsCardTraction.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        DataGridViewCellStyle17.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft
        DataGridViewCellStyle17.BackColor = System.Drawing.SystemColors.Window
        DataGridViewCellStyle17.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0,Byte))
        DataGridViewCellStyle17.ForeColor = System.Drawing.SystemColors.ControlText
        DataGridViewCellStyle17.SelectionBackColor = System.Drawing.SystemColors.Highlight
        DataGridViewCellStyle17.SelectionForeColor = System.Drawing.SystemColors.HighlightText
        DataGridViewCellStyle17.WrapMode = System.Windows.Forms.DataGridViewTriState.[False]
        Me.gvResultsCardTraction.DefaultCellStyle = DataGridViewCellStyle17
        Me.gvResultsCardTraction.Location = New System.Drawing.Point(311, 451)
        Me.gvResultsCardTraction.Name = "gvResultsCardTraction"
        DataGridViewCellStyle18.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft
        DataGridViewCellStyle18.BackColor = System.Drawing.SystemColors.Control
        DataGridViewCellStyle18.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0,Byte))
        DataGridViewCellStyle18.ForeColor = System.Drawing.SystemColors.WindowText
        DataGridViewCellStyle18.SelectionBackColor = System.Drawing.SystemColors.Highlight
        DataGridViewCellStyle18.SelectionForeColor = System.Drawing.SystemColors.HighlightText
        DataGridViewCellStyle18.WrapMode = System.Windows.Forms.DataGridViewTriState.[True]
        Me.gvResultsCardTraction.RowHeadersDefaultCellStyle = DataGridViewCellStyle18
        Me.gvResultsCardTraction.Size = New System.Drawing.Size(258, 125)
        Me.gvResultsCardTraction.TabIndex = 17
        '
        'gvResultsCardIdle
        '
        DataGridViewCellStyle19.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft
        DataGridViewCellStyle19.BackColor = System.Drawing.SystemColors.Control
        DataGridViewCellStyle19.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0,Byte))
        DataGridViewCellStyle19.ForeColor = System.Drawing.SystemColors.WindowText
        DataGridViewCellStyle19.SelectionBackColor = System.Drawing.SystemColors.Highlight
        DataGridViewCellStyle19.SelectionForeColor = System.Drawing.SystemColors.HighlightText
        DataGridViewCellStyle19.WrapMode = System.Windows.Forms.DataGridViewTriState.[True]
        Me.gvResultsCardIdle.ColumnHeadersDefaultCellStyle = DataGridViewCellStyle19
        Me.gvResultsCardIdle.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        DataGridViewCellStyle20.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft
        DataGridViewCellStyle20.BackColor = System.Drawing.SystemColors.Window
        DataGridViewCellStyle20.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0,Byte))
        DataGridViewCellStyle20.ForeColor = System.Drawing.SystemColors.ControlText
        DataGridViewCellStyle20.SelectionBackColor = System.Drawing.SystemColors.Highlight
        DataGridViewCellStyle20.SelectionForeColor = System.Drawing.SystemColors.HighlightText
        DataGridViewCellStyle20.WrapMode = System.Windows.Forms.DataGridViewTriState.[False]
        Me.gvResultsCardIdle.DefaultCellStyle = DataGridViewCellStyle20
        Me.gvResultsCardIdle.Location = New System.Drawing.Point(35, 451)
        Me.gvResultsCardIdle.Name = "gvResultsCardIdle"
        DataGridViewCellStyle21.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft
        DataGridViewCellStyle21.BackColor = System.Drawing.SystemColors.Control
        DataGridViewCellStyle21.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0,Byte))
        DataGridViewCellStyle21.ForeColor = System.Drawing.SystemColors.WindowText
        DataGridViewCellStyle21.SelectionBackColor = System.Drawing.SystemColors.Highlight
        DataGridViewCellStyle21.SelectionForeColor = System.Drawing.SystemColors.HighlightText
        DataGridViewCellStyle21.WrapMode = System.Windows.Forms.DataGridViewTriState.[True]
        Me.gvResultsCardIdle.RowHeadersDefaultCellStyle = DataGridViewCellStyle21
        Me.gvResultsCardIdle.Size = New System.Drawing.Size(256, 125)
        Me.gvResultsCardIdle.TabIndex = 16
        '
        'Label9
        '
        Me.Label9.AutoSize = true
        Me.Label9.Location = New System.Drawing.Point(587, 435)
        Me.Label9.Name = "Label9"
        Me.Label9.Size = New System.Drawing.Size(83, 13)
        Me.Label9.TabIndex = 15
        Me.Label9.Text = "Results Overrun"
        '
        'Label8
        '
        Me.Label8.AutoSize = true
        Me.Label8.Location = New System.Drawing.Point(308, 435)
        Me.Label8.Name = "Label8"
        Me.Label8.Size = New System.Drawing.Size(98, 13)
        Me.Label8.TabIndex = 14
        Me.Label8.Text = "Results TractionOn"
        '
        'Label7
        '
        Me.Label7.AutoSize = true
        Me.Label7.Location = New System.Drawing.Point(38, 435)
        Me.Label7.Name = "Label7"
        Me.Label7.Size = New System.Drawing.Size(62, 13)
        Me.Label7.TabIndex = 13
        Me.Label7.Text = "Results Idle"
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
        'Label6
        '
        Me.Label6.AutoSize = true
        Me.Label6.Location = New System.Drawing.Point(34, 154)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(116, 13)
        Me.Label6.TabIndex = 11
        Me.Label6.Text = "Electrical Consumables"
        '
        'gvElectricalConsumables
        '
        Me.gvElectricalConsumables.AllowUserToOrderColumns = true
        Me.gvElectricalConsumables.AutoGenerateColumns = false
        DataGridViewCellStyle22.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft
        DataGridViewCellStyle22.BackColor = System.Drawing.SystemColors.Control
        DataGridViewCellStyle22.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0,Byte))
        DataGridViewCellStyle22.ForeColor = System.Drawing.SystemColors.WindowText
        DataGridViewCellStyle22.SelectionBackColor = System.Drawing.SystemColors.Highlight
        DataGridViewCellStyle22.SelectionForeColor = System.Drawing.SystemColors.HighlightText
        DataGridViewCellStyle22.WrapMode = System.Windows.Forms.DataGridViewTriState.[True]
        Me.gvElectricalConsumables.ColumnHeadersDefaultCellStyle = DataGridViewCellStyle22
        Me.gvElectricalConsumables.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.gvElectricalConsumables.DataSource = Me.brcElecConsumers
        DataGridViewCellStyle23.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft
        DataGridViewCellStyle23.BackColor = System.Drawing.SystemColors.Window
        DataGridViewCellStyle23.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0,Byte))
        DataGridViewCellStyle23.ForeColor = System.Drawing.SystemColors.ControlText
        DataGridViewCellStyle23.SelectionBackColor = System.Drawing.SystemColors.Highlight
        DataGridViewCellStyle23.SelectionForeColor = System.Drawing.SystemColors.HighlightText
        DataGridViewCellStyle23.WrapMode = System.Windows.Forms.DataGridViewTriState.[False]
        Me.gvElectricalConsumables.DefaultCellStyle = DataGridViewCellStyle23
        Me.gvElectricalConsumables.Location = New System.Drawing.Point(33, 170)
        Me.gvElectricalConsumables.Name = "gvElectricalConsumables"
        DataGridViewCellStyle24.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft
        DataGridViewCellStyle24.BackColor = System.Drawing.SystemColors.Control
        DataGridViewCellStyle24.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0,Byte))
        DataGridViewCellStyle24.ForeColor = System.Drawing.SystemColors.WindowText
        DataGridViewCellStyle24.SelectionBackColor = System.Drawing.SystemColors.Highlight
        DataGridViewCellStyle24.SelectionForeColor = System.Drawing.SystemColors.HighlightText
        DataGridViewCellStyle24.WrapMode = System.Windows.Forms.DataGridViewTriState.[True]
        Me.gvElectricalConsumables.RowHeadersDefaultCellStyle = DataGridViewCellStyle24
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
        'labelDoorActuationTimeSeconds
        '
        Me.labelDoorActuationTimeSeconds.AutoSize = true
        Me.labelDoorActuationTimeSeconds.Location = New System.Drawing.Point(30, 101)
        Me.labelDoorActuationTimeSeconds.Name = "labelDoorActuationTimeSeconds"
        Me.labelDoorActuationTimeSeconds.Size = New System.Drawing.Size(111, 13)
        Me.labelDoorActuationTimeSeconds.TabIndex = 8
        Me.labelDoorActuationTimeSeconds.Text = "DoorActuationTime(S)"
        '
        'Label5
        '
        Me.Label5.AutoSize = true
        Me.Label5.Location = New System.Drawing.Point(30, 73)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(121, 13)
        Me.Label5.TabIndex = 7
        Me.Label5.Text = "AlternatorGearEfficiency"
        '
        'Label4
        '
        Me.Label4.AutoSize = true
        Me.Label4.Location = New System.Drawing.Point(30, 47)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(73, 13)
        Me.Label4.TabIndex = 5
        Me.Label4.Text = "AlternatorMap"
        '
        'Label1
        '
        Me.Label1.AutoSize = true
        Me.Label1.Location = New System.Drawing.Point(30, 18)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(88, 13)
        Me.Label1.TabIndex = 3
        Me.Label1.Text = "PowernetVoltage"
        '
        'tabPneumaticConfig
        '
        Me.tabPneumaticConfig.Controls.Add(Me.pnlPneumaticsUserInput)
        Me.tabPneumaticConfig.Controls.Add(Me.pnlPneumaticAuxillaries)
        Me.tabPneumaticConfig.Location = New System.Drawing.Point(4, 22)
        Me.tabPneumaticConfig.Name = "tabPneumaticConfig"
        Me.tabPneumaticConfig.Size = New System.Drawing.Size(909, 610)
        Me.tabPneumaticConfig.TabIndex = 2
        Me.tabPneumaticConfig.Text = "PneumaticConfig"
        Me.tabPneumaticConfig.UseVisualStyleBackColor = true
        '
        'pnlPneumaticsUserInput
        '
        Me.pnlPneumaticsUserInput.Controls.Add(Me.Button1)
        Me.pnlPneumaticsUserInput.Controls.Add(Me.btnCompressorMap)
        Me.pnlPneumaticsUserInput.Controls.Add(Me.Label10)
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
        'Label10
        '
        Me.Label10.AutoSize = true
        Me.Label10.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0,Byte))
        Me.Label10.ForeColor = System.Drawing.SystemColors.MenuHighlight
        Me.Label10.Location = New System.Drawing.Point(20, 18)
        Me.Label10.Name = "Label10"
        Me.Label10.Size = New System.Drawing.Size(122, 13)
        Me.Label10.TabIndex = 52
        Me.Label10.Text = "Pneumatic Variables"
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
        Me.ErrorProvider.SetError(Me.txtKneelingHeightMillimeters, "Non negative number")
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
        Me.ErrorProvider.SetError(Me.cboCompressorType, "Select Compressor Type")
        Me.cboCompressorType.FormattingEnabled = true
        Me.cboCompressorType.Items.AddRange(New Object() {"<Select>", "CompressorType1", "CompressorType2", "CompressorType3"})
        Me.cboCompressorType.Location = New System.Drawing.Point(156, 57)
        Me.cboCompressorType.Name = "cboCompressorType"
        Me.cboCompressorType.Size = New System.Drawing.Size(121, 21)
        Me.cboCompressorType.TabIndex = 42
        '
        'txtCompressorMap
        '
        Me.ErrorProvider.SetError(Me.txtCompressorMap, "Valid File Path")
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
        Me.ErrorProvider.SetError(Me.txtCompressorGearRatio, "Non negative number")
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
        Me.ErrorProvider.SetError(Me.txtCompressorGearEfficiency, "Number between 0 and 1")
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
        Me.ErrorProvider.SetError(Me.txtActuationsMap, "Valid File Path")
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
        Me.ErrorProvider.SetError(Me.txtAdBlueNIperMinute, "Non negative number")
        Me.txtAdBlueNIperMinute.Location = New System.Drawing.Point(242, 57)
        Me.txtAdBlueNIperMinute.Name = "txtAdBlueNIperMinute"
        Me.txtAdBlueNIperMinute.Size = New System.Drawing.Size(100, 20)
        Me.txtAdBlueNIperMinute.TabIndex = 0
        '
        'txtAirControlledSuspensionNIperMinute
        '
        Me.ErrorProvider.SetError(Me.txtAirControlledSuspensionNIperMinute, "Non negative number")
        Me.txtAirControlledSuspensionNIperMinute.Location = New System.Drawing.Point(242, 98)
        Me.txtAirControlledSuspensionNIperMinute.Name = "txtAirControlledSuspensionNIperMinute"
        Me.txtAirControlledSuspensionNIperMinute.Size = New System.Drawing.Size(100, 20)
        Me.txtAirControlledSuspensionNIperMinute.TabIndex = 1
        '
        'txtBrakingNoRetarderNIperKG
        '
        Me.ErrorProvider.SetError(Me.txtBrakingNoRetarderNIperKG, "Non negative number")
        Me.txtBrakingNoRetarderNIperKG.Location = New System.Drawing.Point(242, 139)
        Me.txtBrakingNoRetarderNIperKG.Name = "txtBrakingNoRetarderNIperKG"
        Me.txtBrakingNoRetarderNIperKG.Size = New System.Drawing.Size(100, 20)
        Me.txtBrakingNoRetarderNIperKG.TabIndex = 2
        '
        'txtBrakingWithRetarderNIperKG
        '
        Me.ErrorProvider.SetError(Me.txtBrakingWithRetarderNIperKG, "Non negative number")
        Me.txtBrakingWithRetarderNIperKG.Location = New System.Drawing.Point(242, 183)
        Me.txtBrakingWithRetarderNIperKG.Name = "txtBrakingWithRetarderNIperKG"
        Me.txtBrakingWithRetarderNIperKG.Size = New System.Drawing.Size(100, 20)
        Me.txtBrakingWithRetarderNIperKG.TabIndex = 3
        '
        'txtBreakingPerKneelingNIperKGinMM
        '
        Me.ErrorProvider.SetError(Me.txtBreakingPerKneelingNIperKGinMM, "Non negative number")
        Me.txtBreakingPerKneelingNIperKGinMM.Location = New System.Drawing.Point(242, 221)
        Me.txtBreakingPerKneelingNIperKGinMM.Name = "txtBreakingPerKneelingNIperKGinMM"
        Me.txtBreakingPerKneelingNIperKGinMM.Size = New System.Drawing.Size(100, 20)
        Me.txtBreakingPerKneelingNIperKGinMM.TabIndex = 4
        '
        'txtDeadVolBlowOutsPerLitresperHour
        '
        Me.ErrorProvider.SetError(Me.txtDeadVolBlowOutsPerLitresperHour, "Non negative number")
        Me.txtDeadVolBlowOutsPerLitresperHour.Location = New System.Drawing.Point(242, 262)
        Me.txtDeadVolBlowOutsPerLitresperHour.Name = "txtDeadVolBlowOutsPerLitresperHour"
        Me.txtDeadVolBlowOutsPerLitresperHour.Size = New System.Drawing.Size(100, 20)
        Me.txtDeadVolBlowOutsPerLitresperHour.TabIndex = 5
        '
        'txtDeadVolumeLitres
        '
        Me.ErrorProvider.SetError(Me.txtDeadVolumeLitres, "Non negative number")
        Me.txtDeadVolumeLitres.Location = New System.Drawing.Point(242, 303)
        Me.txtDeadVolumeLitres.Name = "txtDeadVolumeLitres"
        Me.txtDeadVolumeLitres.Size = New System.Drawing.Size(100, 20)
        Me.txtDeadVolumeLitres.TabIndex = 6
        '
        'txtNonSmartRegenFractionTotalAirDemand
        '
        Me.ErrorProvider.SetError(Me.txtNonSmartRegenFractionTotalAirDemand, "Non negative number")
        Me.txtNonSmartRegenFractionTotalAirDemand.Location = New System.Drawing.Point(242, 344)
        Me.txtNonSmartRegenFractionTotalAirDemand.Name = "txtNonSmartRegenFractionTotalAirDemand"
        Me.txtNonSmartRegenFractionTotalAirDemand.Size = New System.Drawing.Size(100, 20)
        Me.txtNonSmartRegenFractionTotalAirDemand.TabIndex = 7
        '
        'txtOverrunUtilisationForCompressionFraction
        '
        Me.ErrorProvider.SetError(Me.txtOverrunUtilisationForCompressionFraction, "Non negative number")
        Me.txtOverrunUtilisationForCompressionFraction.Location = New System.Drawing.Point(242, 385)
        Me.txtOverrunUtilisationForCompressionFraction.Name = "txtOverrunUtilisationForCompressionFraction"
        Me.txtOverrunUtilisationForCompressionFraction.Size = New System.Drawing.Size(100, 20)
        Me.txtOverrunUtilisationForCompressionFraction.TabIndex = 8
        '
        'txtPerDoorOpeningNI
        '
        Me.ErrorProvider.SetError(Me.txtPerDoorOpeningNI, "Non negative number")
        Me.txtPerDoorOpeningNI.Location = New System.Drawing.Point(242, 426)
        Me.txtPerDoorOpeningNI.Name = "txtPerDoorOpeningNI"
        Me.txtPerDoorOpeningNI.Size = New System.Drawing.Size(100, 20)
        Me.txtPerDoorOpeningNI.TabIndex = 9
        '
        'txtPerStopBrakeActuationNIperKG
        '
        Me.ErrorProvider.SetError(Me.txtPerStopBrakeActuationNIperKG, "Non negative number")
        Me.txtPerStopBrakeActuationNIperKG.Location = New System.Drawing.Point(242, 467)
        Me.txtPerStopBrakeActuationNIperKG.Name = "txtPerStopBrakeActuationNIperKG"
        Me.txtPerStopBrakeActuationNIperKG.Size = New System.Drawing.Size(100, 20)
        Me.txtPerStopBrakeActuationNIperKG.TabIndex = 10
        '
        'txtSmartRegenFractionTotalAirDemand
        '
        Me.ErrorProvider.SetError(Me.txtSmartRegenFractionTotalAirDemand, "Non negative number")
        Me.txtSmartRegenFractionTotalAirDemand.Location = New System.Drawing.Point(242, 508)
        Me.txtSmartRegenFractionTotalAirDemand.Name = "txtSmartRegenFractionTotalAirDemand"
        Me.txtSmartRegenFractionTotalAirDemand.Size = New System.Drawing.Size(100, 20)
        Me.txtSmartRegenFractionTotalAirDemand.TabIndex = 11
        '
        'tabHVACConfig
        '
        Me.tabHVACConfig.Location = New System.Drawing.Point(4, 22)
        Me.tabHVACConfig.Name = "tabHVACConfig"
        Me.tabHVACConfig.Size = New System.Drawing.Size(909, 610)
        Me.tabHVACConfig.TabIndex = 3
        Me.tabHVACConfig.Text = "HVACConfig"
        Me.tabHVACConfig.UseVisualStyleBackColor = true
        '
        'tabPlayground
        '
        Me.tabPlayground.Location = New System.Drawing.Point(4, 22)
        Me.tabPlayground.Name = "tabPlayground"
        Me.tabPlayground.Size = New System.Drawing.Size(909, 610)
        Me.tabPlayground.TabIndex = 4
        Me.tabPlayground.Text = "Playground"
        Me.tabPlayground.UseVisualStyleBackColor = true
        '
        'TabPage1
        '
        Me.TabPage1.Location = New System.Drawing.Point(4, 22)
        Me.TabPage1.Name = "TabPage1"
        Me.TabPage1.Padding = New System.Windows.Forms.Padding(3)
        Me.TabPage1.Size = New System.Drawing.Size(909, 610)
        Me.TabPage1.TabIndex = 5
        Me.TabPage1.Text = "TabPage1"
        Me.TabPage1.UseVisualStyleBackColor = true
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
        'Dashboard
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6!, 13!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(945, 712)
        Me.Controls.Add(Me.Panel1)
        Me.Name = "Dashboard"
        Me.Text = "Dashboard"
        CType(Me.brcElecConsumers,System.ComponentModel.ISupportInitialize).EndInit
        Me.Panel1.ResumeLayout(false)
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
        Me.resultCardContextMenu.ResumeLayout(false)
        CType(Me.ErrorProvider,System.ComponentModel.ISupportInitialize).EndInit
        Me.ResumeLayout(false)

End Sub
    Friend WithEvents Panel1 As System.Windows.Forms.Panel
    Friend WithEvents btnCancel As System.Windows.Forms.Button
    Friend WithEvents btnFinish As System.Windows.Forms.Button
    Friend WithEvents btnForward As System.Windows.Forms.Button
    Friend WithEvents btnBack As System.Windows.Forms.Button
    Friend WithEvents btnStart As System.Windows.Forms.Button
    Friend WithEvents brcElecConsumers As System.Windows.Forms.BindingSource
    Friend WithEvents resultCardContextMenu As System.Windows.Forms.ContextMenuStrip
    Friend WithEvents DeleteToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ErrorProvider As System.Windows.Forms.ErrorProvider
    Friend WithEvents tabMain As System.Windows.Forms.TabControl
    Friend WithEvents tabGeneralConfig As System.Windows.Forms.TabPage
    Friend WithEvents cboCycle As System.Windows.Forms.ComboBox
    Friend WithEvents Label3 As System.Windows.Forms.Label
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents txtVehicleWeightKG As System.Windows.Forms.TextBox
    Friend WithEvents tabElectricalConfig As System.Windows.Forms.TabPage
    Friend WithEvents gvResultsCardOverrun As System.Windows.Forms.DataGridView
    Friend WithEvents gvResultsCardTraction As System.Windows.Forms.DataGridView
    Public WithEvents gvResultsCardIdle As System.Windows.Forms.DataGridView
    Friend WithEvents Label9 As System.Windows.Forms.Label
    Friend WithEvents Label8 As System.Windows.Forms.Label
    Friend WithEvents Label7 As System.Windows.Forms.Label
    Friend WithEvents chkSmartElectricals As System.Windows.Forms.CheckBox
    Friend WithEvents Label6 As System.Windows.Forms.Label
    Friend WithEvents gvElectricalConsumables As System.Windows.Forms.DataGridView
    Friend WithEvents txtDoorActuationTimeSeconds As System.Windows.Forms.TextBox
    Friend WithEvents txtAlternatorGearEfficiency As System.Windows.Forms.TextBox
    Friend WithEvents txtAlternatorMapPath As System.Windows.Forms.TextBox
    Friend WithEvents txtPowernetVoltage As System.Windows.Forms.TextBox
    Friend WithEvents labelDoorActuationTimeSeconds As System.Windows.Forms.Label
    Friend WithEvents Label5 As System.Windows.Forms.Label
    Friend WithEvents Label4 As System.Windows.Forms.Label
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents tabPneumaticConfig As System.Windows.Forms.TabPage
    Friend WithEvents pnlPneumaticsUserInput As System.Windows.Forms.Panel
    Friend WithEvents Button1 As System.Windows.Forms.Button
    Friend WithEvents btnCompressorMap As System.Windows.Forms.Button
    Friend WithEvents Label10 As System.Windows.Forms.Label
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
    Friend WithEvents TabPage1 As System.Windows.Forms.TabPage
End Class
