Imports System.ComponentModel
Imports Microsoft.VisualBasic.CompilerServices

<DesignerGenerated()> _
Partial Class Settings
	Inherits Form

	'Das Formular Ã¼berschreibt den LÃ¶schvorgang, um die Komponentenliste zu bereinigen.
	<DebuggerNonUserCode()> _
	Protected Overrides Sub Dispose(ByVal disposing As Boolean)
		Try
			If disposing AndAlso components IsNot Nothing Then
				components.Dispose()
			End If
		Finally
			MyBase.Dispose(disposing)
		End Try
	End Sub

	'Wird vom Windows Form-Designer benÃ¶tigt.
	Private components As IContainer

	'Hinweis: Die folgende Prozedur ist fÃ¼r den Windows Form-Designer erforderlich.
	'Das Bearbeiten ist mit dem Windows Form-Designer mÃ¶glich.  
	'Das Bearbeiten mit dem Code-Editor ist nicht mÃ¶glich.
	<DebuggerStepThrough()> _
	Private Sub InitializeComponent()
		Me.ButtonOK = New System.Windows.Forms.Button()
		Me.ButtonCancel = New System.Windows.Forms.Button()
		Me.GroupBox3 = New System.Windows.Forms.GroupBox()
		Me.GroupBox5 = New System.Windows.Forms.GroupBox()
		Me.TbOpenCmdName = New System.Windows.Forms.TextBox()
		Me.Label7 = New System.Windows.Forms.Label()
		Me.TbOpenCmd = New System.Windows.Forms.TextBox()
		Me.Label12 = New System.Windows.Forms.Label()
		Me.TextBoxLogSize = New System.Windows.Forms.TextBox()
		Me.Label16 = New System.Windows.Forms.Label()
		Me.TabControl1 = New System.Windows.Forms.TabControl()
		Me.TabPage2 = New System.Windows.Forms.TabPage()
		Me.GrCalc = New System.Windows.Forms.GroupBox()
		Me.Label11 = New System.Windows.Forms.Label()
		Me.Label9 = New System.Windows.Forms.Label()
		Me.Label3 = New System.Windows.Forms.Label()
		Me.TbCO2toFC = New System.Windows.Forms.TextBox()
		Me.Label10 = New System.Windows.Forms.Label()
		Me.TbFuelDens = New System.Windows.Forms.TextBox()
		Me.Label8 = New System.Windows.Forms.Label()
		Me.TbAirDensity = New System.Windows.Forms.TextBox()
		Me.Label2 = New System.Windows.Forms.Label()
		Me.ButReset = New System.Windows.Forms.Button()
		Me.BtHelp = New System.Windows.Forms.Button()
		Me.Label1 = New System.Windows.Forms.Label()
		Me.GroupBox3.SuspendLayout()
		Me.GroupBox5.SuspendLayout()
		Me.TabControl1.SuspendLayout()
		Me.TabPage2.SuspendLayout()
		Me.GrCalc.SuspendLayout()
		Me.SuspendLayout()
		'
		'ButtonOK
		'
		Me.ButtonOK.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
		Me.ButtonOK.Location = New System.Drawing.Point(145, 288)
		Me.ButtonOK.Name = "ButtonOK"
		Me.ButtonOK.Size = New System.Drawing.Size(75, 24)
		Me.ButtonOK.TabIndex = 2
		Me.ButtonOK.Text = "OK"
		Me.ButtonOK.UseVisualStyleBackColor = True
		'
		'ButtonCancel
		'
		Me.ButtonCancel.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
		Me.ButtonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel
		Me.ButtonCancel.Location = New System.Drawing.Point(226, 288)
		Me.ButtonCancel.Name = "ButtonCancel"
		Me.ButtonCancel.Size = New System.Drawing.Size(75, 24)
		Me.ButtonCancel.TabIndex = 3
		Me.ButtonCancel.Text = "Cancel"
		Me.ButtonCancel.UseVisualStyleBackColor = True
		'
		'GroupBox3
		'
		Me.GroupBox3.Controls.Add(Me.GroupBox5)
		Me.GroupBox3.Controls.Add(Me.TextBoxLogSize)
		Me.GroupBox3.Controls.Add(Me.Label16)
		Me.GroupBox3.Location = New System.Drawing.Point(5, 6)
		Me.GroupBox3.Name = "GroupBox3"
		Me.GroupBox3.Size = New System.Drawing.Size(287, 129)
		Me.GroupBox3.TabIndex = 0
		Me.GroupBox3.TabStop = False
		Me.GroupBox3.Text = "Interface"
		'
		'GroupBox5
		'
		Me.GroupBox5.Controls.Add(Me.TbOpenCmdName)
		Me.GroupBox5.Controls.Add(Me.Label7)
		Me.GroupBox5.Controls.Add(Me.TbOpenCmd)
		Me.GroupBox5.Controls.Add(Me.Label12)
		Me.GroupBox5.Location = New System.Drawing.Point(7, 45)
		Me.GroupBox5.Name = "GroupBox5"
		Me.GroupBox5.Size = New System.Drawing.Size(274, 76)
		Me.GroupBox5.TabIndex = 1
		Me.GroupBox5.TabStop = False
		Me.GroupBox5.Text = "File Open Command"
		'
		'TbOpenCmdName
		'
		Me.TbOpenCmdName.Location = New System.Drawing.Point(66, 19)
		Me.TbOpenCmdName.Name = "TbOpenCmdName"
		Me.TbOpenCmdName.Size = New System.Drawing.Size(202, 20)
		Me.TbOpenCmdName.TabIndex = 0
		'
		'Label7
		'
		Me.Label7.AutoSize = True
		Me.Label7.Location = New System.Drawing.Point(6, 48)
		Me.Label7.Name = "Label7"
		Me.Label7.Size = New System.Drawing.Size(54, 13)
		Me.Label7.TabIndex = 12
		Me.Label7.Text = "Command"
		'
		'TbOpenCmd
		'
		Me.TbOpenCmd.Location = New System.Drawing.Point(66, 45)
		Me.TbOpenCmd.Name = "TbOpenCmd"
		Me.TbOpenCmd.Size = New System.Drawing.Size(202, 20)
		Me.TbOpenCmd.TabIndex = 1
		'
		'Label12
		'
		Me.Label12.AutoSize = True
		Me.Label12.Location = New System.Drawing.Point(6, 22)
		Me.Label12.Name = "Label12"
		Me.Label12.Size = New System.Drawing.Size(35, 13)
		Me.Label12.TabIndex = 12
		Me.Label12.Text = "Name"
		'
		'TextBoxLogSize
		'
		Me.TextBoxLogSize.Location = New System.Drawing.Point(123, 19)
		Me.TextBoxLogSize.Name = "TextBoxLogSize"
		Me.TextBoxLogSize.Size = New System.Drawing.Size(36, 20)
		Me.TextBoxLogSize.TabIndex = 0
		'
		'Label16
		'
		Me.Label16.AutoSize = True
		Me.Label16.Location = New System.Drawing.Point(6, 22)
		Me.Label16.Name = "Label16"
		Me.Label16.Size = New System.Drawing.Size(110, 13)
		Me.Label16.TabIndex = 10
		Me.Label16.Text = "Logfile Size Limit [MB]"
		'
		'TabControl1
		'
		Me.TabControl1.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
			Or System.Windows.Forms.AnchorStyles.Left) _
			Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
		Me.TabControl1.Controls.Add(Me.TabPage2)
		Me.TabControl1.Location = New System.Drawing.Point(3, 3)
		Me.TabControl1.Name = "TabControl1"
		Me.TabControl1.SelectedIndex = 0
		Me.TabControl1.Size = New System.Drawing.Size(306, 279)
		Me.TabControl1.TabIndex = 12
		'
		'TabPage2
		'
		Me.TabPage2.Controls.Add(Me.GrCalc)
		Me.TabPage2.Controls.Add(Me.GroupBox3)
		Me.TabPage2.Location = New System.Drawing.Point(4, 22)
		Me.TabPage2.Name = "TabPage2"
		Me.TabPage2.Padding = New System.Windows.Forms.Padding(3)
		Me.TabPage2.Size = New System.Drawing.Size(298, 253)
		Me.TabPage2.TabIndex = 0
		Me.TabPage2.Text = "VECTO"
		Me.TabPage2.UseVisualStyleBackColor = True
		'
		'GrCalc
		'
		Me.GrCalc.Controls.Add(Me.Label1)
		Me.GrCalc.Controls.Add(Me.Label11)
		Me.GrCalc.Controls.Add(Me.Label9)
		Me.GrCalc.Controls.Add(Me.Label3)
		Me.GrCalc.Controls.Add(Me.TbCO2toFC)
		Me.GrCalc.Controls.Add(Me.Label10)
		Me.GrCalc.Controls.Add(Me.TbFuelDens)
		Me.GrCalc.Controls.Add(Me.Label8)
		Me.GrCalc.Controls.Add(Me.TbAirDensity)
		Me.GrCalc.Controls.Add(Me.Label2)
		Me.GrCalc.Location = New System.Drawing.Point(6, 141)
		Me.GrCalc.Name = "GrCalc"
		Me.GrCalc.Size = New System.Drawing.Size(286, 107)
		Me.GrCalc.TabIndex = 1
		Me.GrCalc.TabStop = False
		Me.GrCalc.Text = "Calculation"
		'
		'Label11
		'
		Me.Label11.AutoSize = True
		Me.Label11.Location = New System.Drawing.Point(158, 77)
		Me.Label11.Name = "Label11"
		Me.Label11.Size = New System.Drawing.Size(77, 13)
		Me.Label11.TabIndex = 16
		Me.Label11.Text = "[kgCO2/KgFC]"
		Me.Label11.Visible = False
		'
		'Label9
		'
		Me.Label9.AutoSize = True
		Me.Label9.Location = New System.Drawing.Point(158, 51)
		Me.Label9.Name = "Label9"
		Me.Label9.Size = New System.Drawing.Size(41, 13)
		Me.Label9.TabIndex = 16
		Me.Label9.Text = "[kg/m³]"
		Me.Label9.Visible = False
		'
		'Label3
		'
		Me.Label3.AutoSize = True
		Me.Label3.Location = New System.Drawing.Point(158, 25)
		Me.Label3.Name = "Label3"
		Me.Label3.Size = New System.Drawing.Size(41, 13)
		Me.Label3.TabIndex = 16
		Me.Label3.Text = "[kg/m³]"
		'
		'TbCO2toFC
		'
		Me.TbCO2toFC.Location = New System.Drawing.Point(102, 74)
		Me.TbCO2toFC.Name = "TbCO2toFC"
		Me.TbCO2toFC.Size = New System.Drawing.Size(50, 20)
		Me.TbCO2toFC.TabIndex = 2
		Me.TbCO2toFC.Visible = False
		'
		'Label10
		'
		Me.Label10.AutoSize = True
		Me.Label10.Location = New System.Drawing.Point(5, 77)
		Me.Label10.Name = "Label10"
		Me.Label10.Size = New System.Drawing.Size(91, 13)
		Me.Label10.TabIndex = 14
		Me.Label10.Text = "CO2-to-Fuel Ratio"
		Me.Label10.Visible = False
		'
		'TbFuelDens
		'
		Me.TbFuelDens.Location = New System.Drawing.Point(102, 48)
		Me.TbFuelDens.Name = "TbFuelDens"
		Me.TbFuelDens.Size = New System.Drawing.Size(50, 20)
		Me.TbFuelDens.TabIndex = 1
		Me.TbFuelDens.Visible = False
		'
		'Label8
		'
		Me.Label8.AutoSize = True
		Me.Label8.Location = New System.Drawing.Point(5, 51)
		Me.Label8.Name = "Label8"
		Me.Label8.Size = New System.Drawing.Size(63, 13)
		Me.Label8.TabIndex = 14
		Me.Label8.Text = "Fuel density"
		Me.Label8.Visible = False
		'
		'TbAirDensity
		'
		Me.TbAirDensity.Location = New System.Drawing.Point(102, 22)
		Me.TbAirDensity.Name = "TbAirDensity"
		Me.TbAirDensity.Size = New System.Drawing.Size(50, 20)
		Me.TbAirDensity.TabIndex = 0
		'
		'Label2
		'
		Me.Label2.AutoSize = True
		Me.Label2.Location = New System.Drawing.Point(5, 25)
		Me.Label2.Name = "Label2"
		Me.Label2.Size = New System.Drawing.Size(57, 13)
		Me.Label2.TabIndex = 14
		Me.Label2.Text = "Air Density"
		'
		'ButReset
		'
		Me.ButReset.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
		Me.ButReset.Location = New System.Drawing.Point(31, 288)
		Me.ButReset.Name = "ButReset"
		Me.ButReset.Size = New System.Drawing.Size(108, 24)
		Me.ButReset.TabIndex = 1
		Me.ButReset.Text = "Reset All Settings"
		Me.ButReset.UseVisualStyleBackColor = True
		'
		'BtHelp
		'
		Me.BtHelp.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
		Me.BtHelp.Image = Global.TUGraz.VECTO.My.Resources.Resources.Help_icon
		Me.BtHelp.Location = New System.Drawing.Point(4, 288)
		Me.BtHelp.Name = "BtHelp"
		Me.BtHelp.Size = New System.Drawing.Size(24, 24)
		Me.BtHelp.TabIndex = 0
		Me.BtHelp.UseVisualStyleBackColor = True
		'
		'Label1
		'
		Me.Label1.AutoSize = True
		Me.Label1.Location = New System.Drawing.Point(206, 25)
		Me.Label1.Name = "Label1"
		Me.Label1.Size = New System.Drawing.Size(65, 13)
		Me.Label1.TabIndex = 17
		Me.Label1.Text = "(Eng, Mode)"
		'
		'Settings
		'
		Me.AcceptButton = Me.ButtonOK
		Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
		Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
		Me.CancelButton = Me.ButtonCancel
		Me.ClientSize = New System.Drawing.Size(313, 320)
		Me.Controls.Add(Me.BtHelp)
		Me.Controls.Add(Me.ButReset)
		Me.Controls.Add(Me.TabControl1)
		Me.Controls.Add(Me.ButtonCancel)
		Me.Controls.Add(Me.ButtonOK)
		Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow
		Me.MaximizeBox = False
		Me.MinimizeBox = False
		Me.Name = "Settings"
		Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
		Me.Text = "Settings"
		Me.GroupBox3.ResumeLayout(False)
		Me.GroupBox3.PerformLayout()
		Me.GroupBox5.ResumeLayout(False)
		Me.GroupBox5.PerformLayout()
		Me.TabControl1.ResumeLayout(False)
		Me.TabPage2.ResumeLayout(False)
		Me.GrCalc.ResumeLayout(False)
		Me.GrCalc.PerformLayout()
		Me.ResumeLayout(False)

	End Sub
	Friend WithEvents ButtonOK As Button
	Friend WithEvents ButtonCancel As Button
	Friend WithEvents GroupBox3 As GroupBox
	Friend WithEvents TabControl1 As TabControl
	Friend WithEvents TabPage2 As TabPage
	Friend WithEvents TextBoxLogSize As TextBox
	Friend WithEvents Label16 As Label
	Friend WithEvents ButReset As Button
	Friend WithEvents TbOpenCmd As TextBox
	Friend WithEvents Label7 As Label
	Friend WithEvents GrCalc As GroupBox
	Friend WithEvents Label3 As Label
	Friend WithEvents TbAirDensity As TextBox
	Friend WithEvents Label2 As Label
	Friend WithEvents Label9 As Label
	Friend WithEvents TbFuelDens As TextBox
	Friend WithEvents Label8 As Label
	Friend WithEvents Label11 As Label
	Friend WithEvents TbCO2toFC As TextBox
	Friend WithEvents Label10 As Label
	Friend WithEvents BtHelp As Button
	Friend WithEvents GroupBox5 As GroupBox
	Friend WithEvents TbOpenCmdName As TextBox
	Friend WithEvents Label12 As Label
	Friend WithEvents Label1 As System.Windows.Forms.Label
End Class
