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
        Me.TabControl1 = New System.Windows.Forms.TabControl()
        Me.tabBusParameters = New System.Windows.Forms.TabPage()
        Me.GroupBox1 = New System.Windows.Forms.GroupBox()
        Me.txtBusModel = New System.Windows.Forms.TextBox()
        Me.lblBusModel = New System.Windows.Forms.Label()
        Me.txtBusFloorType = New System.Windows.Forms.TextBox()
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
        Me.tabTechListInput = New System.Windows.Forms.TabPage()
        Me.ToolTip1 = New System.Windows.Forms.ToolTip(Me.components)
        Me.TabControl1.SuspendLayout
        Me.tabBusParameters.SuspendLayout
        Me.GroupBox1.SuspendLayout
        Me.SuspendLayout
        '
        'TabControl1
        '
        Me.TabControl1.Controls.Add(Me.tabBusParameters)
        Me.TabControl1.Controls.Add(Me.tabTechListInput)
        Me.TabControl1.Location = New System.Drawing.Point(8, 33)
        Me.TabControl1.Name = "TabControl1"
        Me.TabControl1.SelectedIndex = 0
        Me.TabControl1.Size = New System.Drawing.Size(945, 623)
        Me.TabControl1.TabIndex = 0
        '
        'tabBusParameters
        '
        Me.tabBusParameters.Controls.Add(Me.GroupBox1)
        Me.tabBusParameters.Controls.Add(Me.cboBuses)
        Me.tabBusParameters.Font = New System.Drawing.Font("Microsoft Sans Serif", 9!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0,Byte))
        Me.tabBusParameters.Location = New System.Drawing.Point(4, 22)
        Me.tabBusParameters.Name = "tabBusParameters"
        Me.tabBusParameters.Padding = New System.Windows.Forms.Padding(3)
        Me.tabBusParameters.Size = New System.Drawing.Size(937, 597)
        Me.tabBusParameters.TabIndex = 0
        Me.tabBusParameters.Text = "INP BusParameters"
        Me.tabBusParameters.UseVisualStyleBackColor = true
        '
        'GroupBox1
        '
        Me.GroupBox1.BackColor = System.Drawing.Color.Transparent
        Me.GroupBox1.Controls.Add(Me.txtBusModel)
        Me.GroupBox1.Controls.Add(Me.lblBusModel)
        Me.GroupBox1.Controls.Add(Me.txtBusFloorType)
        Me.GroupBox1.Controls.Add(Me.lblBusFloorType)
        Me.GroupBox1.Controls.Add(Me.lblUnitsBW)
        Me.GroupBox1.Controls.Add(Me.lblUnitsBSA)
        Me.GroupBox1.Controls.Add(Me.lblUnitsBWSA)
        Me.GroupBox1.Controls.Add(Me.lblUnitsBV)
        Me.GroupBox1.Controls.Add(Me.lblUnitsBL)
        Me.GroupBox1.Controls.Add(Me.lblUnitsBFSA)
        Me.GroupBox1.Controls.Add(Me.txtBusWidth)
        Me.GroupBox1.Controls.Add(Me.txtBusLength)
        Me.GroupBox1.Controls.Add(Me.txtBusVolume)
        Me.GroupBox1.Controls.Add(Me.lblBusVolume)
        Me.GroupBox1.Controls.Add(Me.lblBusLength)
        Me.GroupBox1.Controls.Add(Me.lblBusWidth)
        Me.GroupBox1.Controls.Add(Me.txtBusWindowSurfaceArea)
        Me.GroupBox1.Controls.Add(Me.lblBusWindowSurfaceArea)
        Me.GroupBox1.Controls.Add(Me.txtBusSurfaceArea)
        Me.GroupBox1.Controls.Add(Me.lblBusSurfaceArea)
        Me.GroupBox1.Controls.Add(Me.txtBusFloorSurfaceArea)
        Me.GroupBox1.Controls.Add(Me.lblBusFloorSurfaceArea)
        Me.GroupBox1.Controls.Add(Me.txtRegisteredPassengers)
        Me.GroupBox1.Controls.Add(Me.lblRegisteredPassengers)
        Me.GroupBox1.FlatStyle = System.Windows.Forms.FlatStyle.Popup
        Me.GroupBox1.Font = New System.Drawing.Font("Microsoft Sans Serif", 10!)
        Me.GroupBox1.ForeColor = System.Drawing.Color.Green
        Me.GroupBox1.Location = New System.Drawing.Point(34, 80)
        Me.GroupBox1.Name = "GroupBox1"
        Me.GroupBox1.Size = New System.Drawing.Size(416, 294)
        Me.GroupBox1.TabIndex = 1
        Me.GroupBox1.TabStop = false
        Me.GroupBox1.Text = "Bus Parameterisation"
        '
        'txtBusModel
        '
        Me.txtBusModel.Font = New System.Drawing.Font("Microsoft Sans Serif", 9!)
        Me.txtBusModel.Location = New System.Drawing.Point(179, 30)
        Me.txtBusModel.Name = "txtBusModel"
        Me.txtBusModel.Size = New System.Drawing.Size(208, 21)
        Me.txtBusModel.TabIndex = 25
        '
        'lblBusModel
        '
        Me.lblBusModel.AutoSize = true
        Me.lblBusModel.Font = New System.Drawing.Font("Microsoft Sans Serif", 9!)
        Me.lblBusModel.ForeColor = System.Drawing.Color.Black
        Me.lblBusModel.Location = New System.Drawing.Point(14, 33)
        Me.lblBusModel.Name = "lblBusModel"
        Me.lblBusModel.Size = New System.Drawing.Size(138, 15)
        Me.lblBusModel.TabIndex = 24
        Me.lblBusModel.Text = "Registered Passengers "
        '
        'txtBusFloorType
        '
        Me.txtBusFloorType.Font = New System.Drawing.Font("Microsoft Sans Serif", 9!)
        Me.txtBusFloorType.Location = New System.Drawing.Point(179, 90)
        Me.txtBusFloorType.Name = "txtBusFloorType"
        Me.txtBusFloorType.ReadOnly = true
        Me.txtBusFloorType.Size = New System.Drawing.Size(97, 21)
        Me.txtBusFloorType.TabIndex = 23
        '
        'lblBusFloorType
        '
        Me.lblBusFloorType.AutoSize = true
        Me.lblBusFloorType.Font = New System.Drawing.Font("Microsoft Sans Serif", 9!)
        Me.lblBusFloorType.ForeColor = System.Drawing.Color.Black
        Me.lblBusFloorType.Location = New System.Drawing.Point(14, 88)
        Me.lblBusFloorType.Name = "lblBusFloorType"
        Me.lblBusFloorType.Size = New System.Drawing.Size(88, 15)
        Me.lblBusFloorType.TabIndex = 22
        Me.lblBusFloorType.Text = "Bus Floor Type"
        '
        'lblUnitsBW
        '
        Me.lblUnitsBW.AutoSize = true
        Me.lblUnitsBW.Font = New System.Drawing.Font("Microsoft Sans Serif", 9!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0,Byte))
        Me.lblUnitsBW.Location = New System.Drawing.Point(283, 261)
        Me.lblUnitsBW.Name = "lblUnitsBW"
        Me.lblUnitsBW.Size = New System.Drawing.Size(18, 15)
        Me.lblUnitsBW.TabIndex = 21
        Me.lblUnitsBW.Text = "m"
        Me.ToolTip1.SetToolTip(Me.lblUnitsBW, "Linear Metres")
        '
        'lblUnitsBSA
        '
        Me.lblUnitsBSA.AutoSize = true
        Me.lblUnitsBSA.Font = New System.Drawing.Font("Microsoft Sans Serif", 9!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0,Byte))
        Me.lblUnitsBSA.Location = New System.Drawing.Point(283, 146)
        Me.lblUnitsBSA.Name = "lblUnitsBSA"
        Me.lblUnitsBSA.Size = New System.Drawing.Size(31, 15)
        Me.lblUnitsBSA.TabIndex = 20
        Me.lblUnitsBSA.Text = "m^2"
        Me.ToolTip1.SetToolTip(Me.lblUnitsBSA, "Metres Squared")
        '
        'lblUnitsBWSA
        '
        Me.lblUnitsBWSA.AutoSize = true
        Me.lblUnitsBWSA.Font = New System.Drawing.Font("Microsoft Sans Serif", 9!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0,Byte))
        Me.lblUnitsBWSA.Location = New System.Drawing.Point(283, 175)
        Me.lblUnitsBWSA.Name = "lblUnitsBWSA"
        Me.lblUnitsBWSA.Size = New System.Drawing.Size(31, 15)
        Me.lblUnitsBWSA.TabIndex = 19
        Me.lblUnitsBWSA.Text = "m^2"
        Me.ToolTip1.SetToolTip(Me.lblUnitsBWSA, "Metres Squared")
        '
        'lblUnitsBV
        '
        Me.lblUnitsBV.AutoSize = true
        Me.lblUnitsBV.Font = New System.Drawing.Font("Microsoft Sans Serif", 9!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0,Byte))
        Me.lblUnitsBV.Location = New System.Drawing.Point(283, 202)
        Me.lblUnitsBV.Name = "lblUnitsBV"
        Me.lblUnitsBV.Size = New System.Drawing.Size(31, 15)
        Me.lblUnitsBV.TabIndex = 18
        Me.lblUnitsBV.Text = "m^3"
        Me.ToolTip1.SetToolTip(Me.lblUnitsBV, "Metres Cubed")
        '
        'lblUnitsBL
        '
        Me.lblUnitsBL.AutoSize = true
        Me.lblUnitsBL.Font = New System.Drawing.Font("Microsoft Sans Serif", 9!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0,Byte))
        Me.lblUnitsBL.Location = New System.Drawing.Point(283, 230)
        Me.lblUnitsBL.Name = "lblUnitsBL"
        Me.lblUnitsBL.Size = New System.Drawing.Size(18, 15)
        Me.lblUnitsBL.TabIndex = 17
        Me.lblUnitsBL.Text = "m"
        Me.ToolTip1.SetToolTip(Me.lblUnitsBL, "Linear Metres")
        '
        'lblUnitsBFSA
        '
        Me.lblUnitsBFSA.AutoSize = true
        Me.lblUnitsBFSA.Font = New System.Drawing.Font("Microsoft Sans Serif", 9!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0,Byte))
        Me.lblUnitsBFSA.Location = New System.Drawing.Point(283, 120)
        Me.lblUnitsBFSA.Name = "lblUnitsBFSA"
        Me.lblUnitsBFSA.Size = New System.Drawing.Size(31, 15)
        Me.lblUnitsBFSA.TabIndex = 16
        Me.lblUnitsBFSA.Text = "m^2"
        Me.ToolTip1.SetToolTip(Me.lblUnitsBFSA, "Metres Squared")
        '
        'txtBusWidth
        '
        Me.txtBusWidth.Font = New System.Drawing.Font("Microsoft Sans Serif", 9!)
        Me.txtBusWidth.Location = New System.Drawing.Point(179, 258)
        Me.txtBusWidth.Name = "txtBusWidth"
        Me.txtBusWidth.Size = New System.Drawing.Size(97, 21)
        Me.txtBusWidth.TabIndex = 15
        '
        'txtBusLength
        '
        Me.txtBusLength.Font = New System.Drawing.Font("Microsoft Sans Serif", 9!)
        Me.txtBusLength.Location = New System.Drawing.Point(179, 229)
        Me.txtBusLength.Name = "txtBusLength"
        Me.txtBusLength.Size = New System.Drawing.Size(97, 21)
        Me.txtBusLength.TabIndex = 14
        '
        'txtBusVolume
        '
        Me.txtBusVolume.Font = New System.Drawing.Font("Microsoft Sans Serif", 9!)
        Me.txtBusVolume.Location = New System.Drawing.Point(179, 202)
        Me.txtBusVolume.Name = "txtBusVolume"
        Me.txtBusVolume.Size = New System.Drawing.Size(97, 21)
        Me.txtBusVolume.TabIndex = 13
        '
        'lblBusVolume
        '
        Me.lblBusVolume.AutoSize = true
        Me.lblBusVolume.Font = New System.Drawing.Font("Microsoft Sans Serif", 9!)
        Me.lblBusVolume.ForeColor = System.Drawing.Color.Black
        Me.lblBusVolume.Location = New System.Drawing.Point(14, 205)
        Me.lblBusVolume.Name = "lblBusVolume"
        Me.lblBusVolume.Size = New System.Drawing.Size(73, 15)
        Me.lblBusVolume.TabIndex = 12
        Me.lblBusVolume.Text = "Bus Volume"
        '
        'lblBusLength
        '
        Me.lblBusLength.AutoSize = true
        Me.lblBusLength.Font = New System.Drawing.Font("Microsoft Sans Serif", 9!)
        Me.lblBusLength.ForeColor = System.Drawing.Color.Black
        Me.lblBusLength.Location = New System.Drawing.Point(14, 232)
        Me.lblBusLength.Name = "lblBusLength"
        Me.lblBusLength.Size = New System.Drawing.Size(72, 15)
        Me.lblBusLength.TabIndex = 11
        Me.lblBusLength.Text = "Bus  Length"
        '
        'lblBusWidth
        '
        Me.lblBusWidth.AutoSize = true
        Me.lblBusWidth.Font = New System.Drawing.Font("Microsoft Sans Serif", 9!)
        Me.lblBusWidth.ForeColor = System.Drawing.Color.Black
        Me.lblBusWidth.Location = New System.Drawing.Point(14, 261)
        Me.lblBusWidth.Name = "lblBusWidth"
        Me.lblBusWidth.Size = New System.Drawing.Size(65, 15)
        Me.lblBusWidth.TabIndex = 10
        Me.lblBusWidth.Text = "Bus  Width"
        '
        'txtBusWindowSurfaceArea
        '
        Me.txtBusWindowSurfaceArea.Font = New System.Drawing.Font("Microsoft Sans Serif", 9!)
        Me.txtBusWindowSurfaceArea.Location = New System.Drawing.Point(179, 173)
        Me.txtBusWindowSurfaceArea.Name = "txtBusWindowSurfaceArea"
        Me.txtBusWindowSurfaceArea.ReadOnly = true
        Me.txtBusWindowSurfaceArea.Size = New System.Drawing.Size(97, 21)
        Me.txtBusWindowSurfaceArea.TabIndex = 9
        '
        'lblBusWindowSurfaceArea
        '
        Me.lblBusWindowSurfaceArea.AutoSize = true
        Me.lblBusWindowSurfaceArea.Font = New System.Drawing.Font("Microsoft Sans Serif", 9!)
        Me.lblBusWindowSurfaceArea.ForeColor = System.Drawing.Color.Black
        Me.lblBusWindowSurfaceArea.Location = New System.Drawing.Point(14, 176)
        Me.lblBusWindowSurfaceArea.Name = "lblBusWindowSurfaceArea"
        Me.lblBusWindowSurfaceArea.Size = New System.Drawing.Size(151, 15)
        Me.lblBusWindowSurfaceArea.TabIndex = 8
        Me.lblBusWindowSurfaceArea.Text = "Bus  Window Surface Area"
        '
        'txtBusSurfaceArea
        '
        Me.txtBusSurfaceArea.Font = New System.Drawing.Font("Microsoft Sans Serif", 9!)
        Me.txtBusSurfaceArea.Location = New System.Drawing.Point(179, 145)
        Me.txtBusSurfaceArea.Name = "txtBusSurfaceArea"
        Me.txtBusSurfaceArea.Size = New System.Drawing.Size(97, 21)
        Me.txtBusSurfaceArea.TabIndex = 7
        '
        'lblBusSurfaceArea
        '
        Me.lblBusSurfaceArea.AutoSize = true
        Me.lblBusSurfaceArea.Font = New System.Drawing.Font("Microsoft Sans Serif", 9!)
        Me.lblBusSurfaceArea.ForeColor = System.Drawing.Color.Black
        Me.lblBusSurfaceArea.Location = New System.Drawing.Point(14, 148)
        Me.lblBusSurfaceArea.Name = "lblBusSurfaceArea"
        Me.lblBusSurfaceArea.Size = New System.Drawing.Size(104, 15)
        Me.lblBusSurfaceArea.TabIndex = 6
        Me.lblBusSurfaceArea.Text = "Bus  Surface Area"
        '
        'txtBusFloorSurfaceArea
        '
        Me.txtBusFloorSurfaceArea.Font = New System.Drawing.Font("Microsoft Sans Serif", 9!)
        Me.txtBusFloorSurfaceArea.Location = New System.Drawing.Point(179, 117)
        Me.txtBusFloorSurfaceArea.Name = "txtBusFloorSurfaceArea"
        Me.txtBusFloorSurfaceArea.Size = New System.Drawing.Size(97, 21)
        Me.txtBusFloorSurfaceArea.TabIndex = 5
        '
        'lblBusFloorSurfaceArea
        '
        Me.lblBusFloorSurfaceArea.AutoSize = true
        Me.lblBusFloorSurfaceArea.Font = New System.Drawing.Font("Microsoft Sans Serif", 9!)
        Me.lblBusFloorSurfaceArea.ForeColor = System.Drawing.Color.Black
        Me.lblBusFloorSurfaceArea.Location = New System.Drawing.Point(14, 120)
        Me.lblBusFloorSurfaceArea.Name = "lblBusFloorSurfaceArea"
        Me.lblBusFloorSurfaceArea.Size = New System.Drawing.Size(132, 15)
        Me.lblBusFloorSurfaceArea.TabIndex = 4
        Me.lblBusFloorSurfaceArea.Text = "Bus Floor Surface Area"
        '
        'txtRegisteredPassengers
        '
        Me.txtRegisteredPassengers.Font = New System.Drawing.Font("Microsoft Sans Serif", 9!)
        Me.txtRegisteredPassengers.Location = New System.Drawing.Point(179, 59)
        Me.txtRegisteredPassengers.Name = "txtRegisteredPassengers"
        Me.txtRegisteredPassengers.Size = New System.Drawing.Size(97, 21)
        Me.txtRegisteredPassengers.TabIndex = 1
        '
        'lblRegisteredPassengers
        '
        Me.lblRegisteredPassengers.AutoSize = true
        Me.lblRegisteredPassengers.Font = New System.Drawing.Font("Microsoft Sans Serif", 9!)
        Me.lblRegisteredPassengers.ForeColor = System.Drawing.Color.Black
        Me.lblRegisteredPassengers.Location = New System.Drawing.Point(14, 62)
        Me.lblRegisteredPassengers.Name = "lblRegisteredPassengers"
        Me.lblRegisteredPassengers.Size = New System.Drawing.Size(138, 15)
        Me.lblRegisteredPassengers.TabIndex = 0
        Me.lblRegisteredPassengers.Text = "Registered Passengers "
        '
        'cboBuses
        '
        Me.cboBuses.FormattingEnabled = true
        Me.cboBuses.Location = New System.Drawing.Point(34, 30)
        Me.cboBuses.Name = "cboBuses"
        Me.cboBuses.Size = New System.Drawing.Size(361, 23)
        Me.cboBuses.TabIndex = 0
        '
        'tabTechListInput
        '
        Me.tabTechListInput.Location = New System.Drawing.Point(4, 22)
        Me.tabTechListInput.Name = "tabTechListInput"
        Me.tabTechListInput.Padding = New System.Windows.Forms.Padding(3)
        Me.tabTechListInput.Size = New System.Drawing.Size(937, 597)
        Me.tabTechListInput.TabIndex = 1
        Me.tabTechListInput.Text = "Tech List Input"
        Me.tabTechListInput.UseVisualStyleBackColor = true
        '
        'frmHVACTool
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6!, 13!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(965, 712)
        Me.Controls.Add(Me.TabControl1)
        Me.Name = "frmHVACTool"
        Me.Text = "frmHVACTool"
        Me.TabControl1.ResumeLayout(false)
        Me.tabBusParameters.ResumeLayout(false)
        Me.GroupBox1.ResumeLayout(false)
        Me.GroupBox1.PerformLayout
        Me.ResumeLayout(false)

End Sub
    Friend WithEvents TabControl1 As System.Windows.Forms.TabControl
    Friend WithEvents tabBusParameters As System.Windows.Forms.TabPage
    Friend WithEvents tabTechListInput As System.Windows.Forms.TabPage
    Friend WithEvents cboBuses As System.Windows.Forms.ComboBox
    Friend WithEvents GroupBox1 As System.Windows.Forms.GroupBox
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
    Friend WithEvents txtBusVolume As System.Windows.Forms.TextBox
    Friend WithEvents txtBusWidth As System.Windows.Forms.TextBox
    Friend WithEvents txtBusLength As System.Windows.Forms.TextBox
    Friend WithEvents lblUnitsBW As System.Windows.Forms.Label
    Friend WithEvents lblUnitsBSA As System.Windows.Forms.Label
    Friend WithEvents lblUnitsBWSA As System.Windows.Forms.Label
    Friend WithEvents lblUnitsBV As System.Windows.Forms.Label
    Friend WithEvents lblUnitsBL As System.Windows.Forms.Label
    Friend WithEvents lblUnitsBFSA As System.Windows.Forms.Label
    Friend WithEvents ToolTip1 As System.Windows.Forms.ToolTip
    Friend WithEvents txtBusFloorType As System.Windows.Forms.TextBox
    Friend WithEvents lblBusFloorType As System.Windows.Forms.Label
    Friend WithEvents txtBusModel As System.Windows.Forms.TextBox
    Friend WithEvents lblBusModel As System.Windows.Forms.Label
End Class
