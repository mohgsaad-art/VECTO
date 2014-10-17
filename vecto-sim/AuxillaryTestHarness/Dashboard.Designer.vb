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
        Me.tabMain = New System.Windows.Forms.TabControl()
        Me.tabGeneralConfig = New System.Windows.Forms.TabPage()
        Me.cboCycle = New System.Windows.Forms.ComboBox()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.txtVehicleWeightKG = New System.Windows.Forms.TextBox()
        Me.tabElectricalConfig = New System.Windows.Forms.TabPage()
        Me.DataGridView3 = New System.Windows.Forms.DataGridView()
        Me.DataGridView2 = New System.Windows.Forms.DataGridView()
        Me.DataGridView1 = New System.Windows.Forms.DataGridView()
        Me.Label9 = New System.Windows.Forms.Label()
        Me.Label8 = New System.Windows.Forms.Label()
        Me.Label7 = New System.Windows.Forms.Label()
        Me.chkSmartElectricals = New System.Windows.Forms.CheckBox()
        Me.Label6 = New System.Windows.Forms.Label()
        Me.gvElectricalConsumables = New System.Windows.Forms.DataGridView()
        Me.bsElectricalConsumables = New System.Windows.Forms.BindingSource(Me.components)
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
        Me.BindingSource1 = New System.Windows.Forms.BindingSource(Me.components)
        Me.tabMain.SuspendLayout
        Me.tabGeneralConfig.SuspendLayout
        Me.tabElectricalConfig.SuspendLayout
        CType(Me.DataGridView3,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.DataGridView2,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.DataGridView1,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.gvElectricalConsumables,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.bsElectricalConsumables,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.BindingSource1,System.ComponentModel.ISupportInitialize).BeginInit
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
        Me.tabMain.Location = New System.Drawing.Point(30, 12)
        Me.tabMain.Name = "tabMain"
        Me.tabMain.SelectedIndex = 0
        Me.tabMain.Size = New System.Drawing.Size(1116, 687)
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
        Me.tabGeneralConfig.Size = New System.Drawing.Size(1108, 661)
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
        Me.tabElectricalConfig.Controls.Add(Me.DataGridView3)
        Me.tabElectricalConfig.Controls.Add(Me.DataGridView2)
        Me.tabElectricalConfig.Controls.Add(Me.DataGridView1)
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
        Me.tabElectricalConfig.Size = New System.Drawing.Size(1108, 661)
        Me.tabElectricalConfig.TabIndex = 1
        Me.tabElectricalConfig.Text = "ElectricalConfig"
        Me.tabElectricalConfig.UseVisualStyleBackColor = true
        '
        'DataGridView3
        '
        Me.DataGridView3.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.DataGridView3.Location = New System.Drawing.Point(161, 529)
        Me.DataGridView3.Name = "DataGridView3"
        Me.DataGridView3.Size = New System.Drawing.Size(141, 81)
        Me.DataGridView3.TabIndex = 18
        '
        'DataGridView2
        '
        Me.DataGridView2.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.DataGridView2.Location = New System.Drawing.Point(161, 431)
        Me.DataGridView2.Name = "DataGridView2"
        Me.DataGridView2.Size = New System.Drawing.Size(141, 81)
        Me.DataGridView2.TabIndex = 17
        '
        'DataGridView1
        '
        Me.DataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.DataGridView1.Location = New System.Drawing.Point(161, 324)
        Me.DataGridView1.Name = "DataGridView1"
        Me.DataGridView1.Size = New System.Drawing.Size(141, 84)
        Me.DataGridView1.TabIndex = 16
        '
        'Label9
        '
        Me.Label9.AutoSize = true
        Me.Label9.Location = New System.Drawing.Point(74, 530)
        Me.Label9.Name = "Label9"
        Me.Label9.Size = New System.Drawing.Size(83, 13)
        Me.Label9.TabIndex = 15
        Me.Label9.Text = "Results Overrun"
        '
        'Label8
        '
        Me.Label8.AutoSize = true
        Me.Label8.Location = New System.Drawing.Point(71, 428)
        Me.Label8.Name = "Label8"
        Me.Label8.Size = New System.Drawing.Size(84, 13)
        Me.Label8.TabIndex = 14
        Me.Label8.Text = "Results Traction"
        '
        'Label7
        '
        Me.Label7.AutoSize = true
        Me.Label7.Location = New System.Drawing.Point(92, 325)
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
        Me.Label6.Location = New System.Drawing.Point(27, 166)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(116, 13)
        Me.Label6.TabIndex = 11
        Me.Label6.Text = "Electrical Consumables"
        '
        'gvElectricalConsumables
        '
        Me.gvElectricalConsumables.AutoGenerateColumns = false
        Me.gvElectricalConsumables.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.gvElectricalConsumables.DataSource = Me.bsElectricalConsumables
        Me.gvElectricalConsumables.Location = New System.Drawing.Point(161, 170)
        Me.gvElectricalConsumables.Name = "gvElectricalConsumables"
        Me.gvElectricalConsumables.Size = New System.Drawing.Size(877, 118)
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
        Me.tabPneumaticConfig.Size = New System.Drawing.Size(1108, 661)
        Me.tabPneumaticConfig.TabIndex = 2
        Me.tabPneumaticConfig.Text = "PneumaticConfig"
        Me.tabPneumaticConfig.UseVisualStyleBackColor = true
        '
        'tabHVACConfig
        '
        Me.tabHVACConfig.Location = New System.Drawing.Point(4, 22)
        Me.tabHVACConfig.Name = "tabHVACConfig"
        Me.tabHVACConfig.Size = New System.Drawing.Size(1108, 661)
        Me.tabHVACConfig.TabIndex = 3
        Me.tabHVACConfig.Text = "HVACConfig"
        Me.tabHVACConfig.UseVisualStyleBackColor = true
        '
        'tabPlayground
        '
        Me.tabPlayground.Location = New System.Drawing.Point(4, 22)
        Me.tabPlayground.Name = "tabPlayground"
        Me.tabPlayground.Size = New System.Drawing.Size(1108, 661)
        Me.tabPlayground.TabIndex = 4
        Me.tabPlayground.Text = "Playground"
        Me.tabPlayground.UseVisualStyleBackColor = true
        '
        'BindingSource1
        '
        Me.BindingSource1.DataSource = GetType(AuxillaryTestHarness.Dashboard)
        '
        'Dashboard
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6!, 13!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(1192, 711)
        Me.Controls.Add(Me.tabMain)
        Me.Name = "Dashboard"
        Me.Text = "Dashboard"
        Me.tabMain.ResumeLayout(false)
        Me.tabGeneralConfig.ResumeLayout(false)
        Me.tabGeneralConfig.PerformLayout
        Me.tabElectricalConfig.ResumeLayout(false)
        Me.tabElectricalConfig.PerformLayout
        CType(Me.DataGridView3,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.DataGridView2,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.DataGridView1,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.gvElectricalConsumables,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.bsElectricalConsumables,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.BindingSource1,System.ComponentModel.ISupportInitialize).EndInit
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
    Friend WithEvents DataGridView3 As System.Windows.Forms.DataGridView
    Friend WithEvents DataGridView2 As System.Windows.Forms.DataGridView
    Friend WithEvents DataGridView1 As System.Windows.Forms.DataGridView
    Friend WithEvents Label9 As System.Windows.Forms.Label
    Friend WithEvents Label8 As System.Windows.Forms.Label
    Friend WithEvents Label7 As System.Windows.Forms.Label
    Friend WithEvents chkSmartElectricals As System.Windows.Forms.CheckBox
    Friend WithEvents Label6 As System.Windows.Forms.Label
    Friend WithEvents gvElectricalConsumables As System.Windows.Forms.DataGridView
    Friend WithEvents bsElectricalConsumables As System.Windows.Forms.BindingSource
    Friend WithEvents BindingSource1 As System.Windows.Forms.BindingSource
End Class
