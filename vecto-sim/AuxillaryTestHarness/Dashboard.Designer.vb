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
        Me.brcElecConsumers = New System.Windows.Forms.BindingSource(Me.components)
        Me.txtDoorActuationTimeSeconds = New System.Windows.Forms.TextBox()
        Me.labelDoorActuationTimeSeconds = New System.Windows.Forms.Label()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.txtAlternatorGearEfficiency = New System.Windows.Forms.TextBox()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.txtAlternatorMapPath = New System.Windows.Forms.TextBox()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.txtPowernetVoltage = New System.Windows.Forms.TextBox()
        Me.tabPneumaticConfig = New System.Windows.Forms.TabPage()
        Me.tabHVACConfig = New System.Windows.Forms.TabPage()
        Me.tabPlayground = New System.Windows.Forms.TabPage()
        Me.Panel1 = New System.Windows.Forms.Panel()
        Me.btnCancel = New System.Windows.Forms.Button()
        Me.btnFinish = New System.Windows.Forms.Button()
        Me.btnForward = New System.Windows.Forms.Button()
        Me.btnBack = New System.Windows.Forms.Button()
        Me.btnStart = New System.Windows.Forms.Button()
        Me.resultCardContextMenu = New System.Windows.Forms.ContextMenuStrip(Me.components)
        Me.DeleteToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.tabMain.SuspendLayout
        Me.tabGeneralConfig.SuspendLayout
        Me.tabElectricalConfig.SuspendLayout
        CType(Me.gvResultsCardOverrun,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.gvResultsCardTraction,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.gvResultsCardIdle,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.gvElectricalConsumables,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.brcElecConsumers,System.ComponentModel.ISupportInitialize).BeginInit
        Me.Panel1.SuspendLayout
        Me.resultCardContextMenu.SuspendLayout
        Me.SuspendLayout
        '
        'tabMain
        '
        Me.tabMain.AccessibleDescription = ""
        Me.tabMain.Controls.Add(Me.tabGeneralConfig)
        Me.tabMain.Controls.Add(Me.tabElectricalConfig)
        Me.tabMain.Controls.Add(Me.tabPneumaticConfig)
        Me.tabMain.Controls.Add(Me.tabHVACConfig)
        Me.tabMain.Controls.Add(Me.tabPlayground)
        Me.tabMain.Location = New System.Drawing.Point(13, 14)
        Me.tabMain.Name = "tabMain"
        Me.tabMain.SelectedIndex = 0
        Me.tabMain.Size = New System.Drawing.Size(894, 636)
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
        Me.tabGeneralConfig.Size = New System.Drawing.Size(886, 610)
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
        Me.tabElectricalConfig.Controls.Add(Me.labelDoorActuationTimeSeconds)
        Me.tabElectricalConfig.Controls.Add(Me.Label5)
        Me.tabElectricalConfig.Controls.Add(Me.txtAlternatorGearEfficiency)
        Me.tabElectricalConfig.Controls.Add(Me.Label4)
        Me.tabElectricalConfig.Controls.Add(Me.txtAlternatorMapPath)
        Me.tabElectricalConfig.Controls.Add(Me.Label1)
        Me.tabElectricalConfig.Controls.Add(Me.txtPowernetVoltage)
        Me.tabElectricalConfig.Location = New System.Drawing.Point(4, 22)
        Me.tabElectricalConfig.Name = "tabElectricalConfig"
        Me.tabElectricalConfig.Padding = New System.Windows.Forms.Padding(3)
        Me.tabElectricalConfig.Size = New System.Drawing.Size(886, 610)
        Me.tabElectricalConfig.TabIndex = 1
        Me.tabElectricalConfig.Text = "ElectricalConfig"
        Me.tabElectricalConfig.UseVisualStyleBackColor = true
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
        Me.Label8.Size = New System.Drawing.Size(84, 13)
        Me.Label8.TabIndex = 14
        Me.Label8.Text = "Results Traction"
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
        DataGridViewCellStyle10.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft
        DataGridViewCellStyle10.BackColor = System.Drawing.SystemColors.Control
        DataGridViewCellStyle10.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0,Byte))
        DataGridViewCellStyle10.ForeColor = System.Drawing.SystemColors.WindowText
        DataGridViewCellStyle10.SelectionBackColor = System.Drawing.SystemColors.Highlight
        DataGridViewCellStyle10.SelectionForeColor = System.Drawing.SystemColors.HighlightText
        DataGridViewCellStyle10.WrapMode = System.Windows.Forms.DataGridViewTriState.[True]
        Me.gvElectricalConsumables.ColumnHeadersDefaultCellStyle = DataGridViewCellStyle10
        Me.gvElectricalConsumables.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.gvElectricalConsumables.DataSource = Me.brcElecConsumers
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
        Me.txtDoorActuationTimeSeconds.Location = New System.Drawing.Point(161, 101)
        Me.txtDoorActuationTimeSeconds.Name = "txtDoorActuationTimeSeconds"
        Me.txtDoorActuationTimeSeconds.Size = New System.Drawing.Size(100, 20)
        Me.txtDoorActuationTimeSeconds.TabIndex = 9
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
        'txtAlternatorGearEfficiency
        '
        Me.txtAlternatorGearEfficiency.Location = New System.Drawing.Point(161, 73)
        Me.txtAlternatorGearEfficiency.Name = "txtAlternatorGearEfficiency"
        Me.txtAlternatorGearEfficiency.Size = New System.Drawing.Size(100, 20)
        Me.txtAlternatorGearEfficiency.TabIndex = 6
        '
        'Label4
        '
        Me.Label4.AutoSize = true
        Me.Label4.Location = New System.Drawing.Point(27, 47)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(73, 13)
        Me.Label4.TabIndex = 5
        Me.Label4.Text = "AlternatorMap"
        '
        'txtAlternatorMapPath
        '
        Me.txtAlternatorMapPath.Location = New System.Drawing.Point(161, 43)
        Me.txtAlternatorMapPath.Name = "txtAlternatorMapPath"
        Me.txtAlternatorMapPath.Size = New System.Drawing.Size(319, 20)
        Me.txtAlternatorMapPath.TabIndex = 4
        '
        'Label1
        '
        Me.Label1.AutoSize = true
        Me.Label1.Location = New System.Drawing.Point(24, 18)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(88, 13)
        Me.Label1.TabIndex = 3
        Me.Label1.Text = "PowernetVoltage"
        '
        'txtPowernetVoltage
        '
        Me.txtPowernetVoltage.Location = New System.Drawing.Point(161, 16)
        Me.txtPowernetVoltage.Name = "txtPowernetVoltage"
        Me.txtPowernetVoltage.Size = New System.Drawing.Size(100, 20)
        Me.txtPowernetVoltage.TabIndex = 2
        '
        'tabPneumaticConfig
        '
        Me.tabPneumaticConfig.Location = New System.Drawing.Point(4, 22)
        Me.tabPneumaticConfig.Name = "tabPneumaticConfig"
        Me.tabPneumaticConfig.Size = New System.Drawing.Size(886, 610)
        Me.tabPneumaticConfig.TabIndex = 2
        Me.tabPneumaticConfig.Text = "PneumaticConfig"
        Me.tabPneumaticConfig.UseVisualStyleBackColor = true
        '
        'tabHVACConfig
        '
        Me.tabHVACConfig.Location = New System.Drawing.Point(4, 22)
        Me.tabHVACConfig.Name = "tabHVACConfig"
        Me.tabHVACConfig.Size = New System.Drawing.Size(886, 610)
        Me.tabHVACConfig.TabIndex = 3
        Me.tabHVACConfig.Text = "HVACConfig"
        Me.tabHVACConfig.UseVisualStyleBackColor = true
        '
        'tabPlayground
        '
        Me.tabPlayground.Location = New System.Drawing.Point(4, 22)
        Me.tabPlayground.Name = "tabPlayground"
        Me.tabPlayground.Size = New System.Drawing.Size(886, 610)
        Me.tabPlayground.TabIndex = 4
        Me.tabPlayground.Text = "Playground"
        Me.tabPlayground.UseVisualStyleBackColor = true
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
        Me.Panel1.Size = New System.Drawing.Size(937, 732)
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
        'Dashboard
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6!, 13!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(937, 732)
        Me.Controls.Add(Me.Panel1)
        Me.Name = "Dashboard"
        Me.Text = "Dashboard"
        Me.tabMain.ResumeLayout(false)
        Me.tabGeneralConfig.ResumeLayout(false)
        Me.tabGeneralConfig.PerformLayout
        Me.tabElectricalConfig.ResumeLayout(false)
        Me.tabElectricalConfig.PerformLayout
        CType(Me.gvResultsCardOverrun,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.gvResultsCardTraction,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.gvResultsCardIdle,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.gvElectricalConsumables,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.brcElecConsumers,System.ComponentModel.ISupportInitialize).EndInit
        Me.Panel1.ResumeLayout(false)
        Me.resultCardContextMenu.ResumeLayout(false)
        Me.ResumeLayout(false)

End Sub
    Friend WithEvents tabMain As System.Windows.Forms.TabControl
    Friend WithEvents tabGeneralConfig As System.Windows.Forms.TabPage
    Friend WithEvents tabElectricalConfig As System.Windows.Forms.TabPage
    Friend WithEvents tabPneumaticConfig As System.Windows.Forms.TabPage
    Friend WithEvents tabHVACConfig As System.Windows.Forms.TabPage
    Friend WithEvents tabPlayground As System.Windows.Forms.TabPage
    Friend WithEvents cboCycle As System.Windows.Forms.ComboBox
    Friend WithEvents Label3 As System.Windows.Forms.Label
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents txtVehicleWeightKG As System.Windows.Forms.TextBox
    Friend WithEvents txtDoorActuationTimeSeconds As System.Windows.Forms.TextBox
    Friend WithEvents labelDoorActuationTimeSeconds As System.Windows.Forms.Label
    Friend WithEvents Label5 As System.Windows.Forms.Label
    Friend WithEvents txtAlternatorGearEfficiency As System.Windows.Forms.TextBox
    Friend WithEvents Label4 As System.Windows.Forms.Label
    Friend WithEvents txtAlternatorMapPath As System.Windows.Forms.TextBox
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents txtPowernetVoltage As System.Windows.Forms.TextBox
    Friend WithEvents gvResultsCardOverrun As System.Windows.Forms.DataGridView
    Friend WithEvents gvResultsCardTraction As System.Windows.Forms.DataGridView
    Friend WithEvents Label9 As System.Windows.Forms.Label
    Friend WithEvents Label8 As System.Windows.Forms.Label
    Friend WithEvents Label7 As System.Windows.Forms.Label
    Friend WithEvents chkSmartElectricals As System.Windows.Forms.CheckBox
    Friend WithEvents Label6 As System.Windows.Forms.Label
    Friend WithEvents gvElectricalConsumables As System.Windows.Forms.DataGridView
    Friend WithEvents Panel1 As System.Windows.Forms.Panel
    Friend WithEvents btnCancel As System.Windows.Forms.Button
    Friend WithEvents btnFinish As System.Windows.Forms.Button
    Friend WithEvents btnForward As System.Windows.Forms.Button
    Friend WithEvents btnBack As System.Windows.Forms.Button
    Friend WithEvents btnStart As System.Windows.Forms.Button
    Friend WithEvents brcElecConsumers As System.Windows.Forms.BindingSource
    Public WithEvents gvResultsCardIdle As System.Windows.Forms.DataGridView
    Friend WithEvents resultCardContextMenu As System.Windows.Forms.ContextMenuStrip
    Friend WithEvents DeleteToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
End Class
