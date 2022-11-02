Imports System.ComponentModel
Imports Microsoft.VisualBasic.CompilerServices



<DesignerGenerated()> _
Partial Class GearboxGearDialog
	Inherits Form

	'Das Formular überschreibt den Löschvorgang, um die Komponentenliste zu bereinigen.
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

	'Wird vom Windows Form-Designer benötigt.
	Private components As IContainer

	'Hinweis: Die folgende Prozedur ist für den Windows Form-Designer erforderlich.
	'Das Bearbeiten ist mit dem Windows Form-Designer möglich.  
	'Das Bearbeiten mit dem Code-Editor ist nicht möglich.
	<DebuggerStepThrough()> _
	Private Sub InitializeComponent()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(GearboxGearDialog))
        Me.TableLayoutPanel1 = New System.Windows.Forms.TableLayoutPanel()
        Me.OK_Button = New System.Windows.Forms.Button()
        Me.Cancel_Button = New System.Windows.Forms.Button()
        Me.TbGear = New System.Windows.Forms.TextBox()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.TbRatio = New System.Windows.Forms.TextBox()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.TbMapPath = New System.Windows.Forms.TextBox()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.BtBrowse = New System.Windows.Forms.Button()
        Me.BtNext = New System.Windows.Forms.Button()
        Me.TbShiftPolyFile = New System.Windows.Forms.TextBox()
        Me.BtShiftPolyBrowse = New System.Windows.Forms.Button()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.PnShiftPoly = New System.Windows.Forms.Panel()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.TbMaxTorque = New System.Windows.Forms.TextBox()
        Me.PnFld = New System.Windows.Forms.Panel()
        Me.Label7 = New System.Windows.Forms.Label()
        Me.tbMaxSpeed = New System.Windows.Forms.TextBox()
        Me.Label8 = New System.Windows.Forms.Label()
        Me.Label6 = New System.Windows.Forms.Label()
        Me.BtPrevious = New System.Windows.Forms.Button()
        Me.TableLayoutPanel1.SuspendLayout
        Me.PnShiftPoly.SuspendLayout
        Me.PnFld.SuspendLayout
        Me.SuspendLayout
        '
        'TableLayoutPanel1
        '
        Me.TableLayoutPanel1.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
        Me.TableLayoutPanel1.ColumnCount = 2
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50!))
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50!))
        Me.TableLayoutPanel1.Controls.Add(Me.OK_Button, 0, 0)
        Me.TableLayoutPanel1.Controls.Add(Me.Cancel_Button, 1, 0)
        Me.TableLayoutPanel1.Location = New System.Drawing.Point(370, 177)
        Me.TableLayoutPanel1.Name = "TableLayoutPanel1"
        Me.TableLayoutPanel1.RowCount = 1
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50!))
        Me.TableLayoutPanel1.Size = New System.Drawing.Size(146, 29)
        Me.TableLayoutPanel1.TabIndex = 12
        '
        'OK_Button
        '
        Me.OK_Button.Anchor = System.Windows.Forms.AnchorStyles.None
        Me.OK_Button.Location = New System.Drawing.Point(3, 3)
        Me.OK_Button.Name = "OK_Button"
        Me.OK_Button.Size = New System.Drawing.Size(67, 23)
        Me.OK_Button.TabIndex = 6
        Me.OK_Button.Text = "OK"
        '
        'Cancel_Button
        '
        Me.Cancel_Button.Anchor = System.Windows.Forms.AnchorStyles.None
        Me.Cancel_Button.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.Cancel_Button.Location = New System.Drawing.Point(76, 3)
        Me.Cancel_Button.Name = "Cancel_Button"
        Me.Cancel_Button.Size = New System.Drawing.Size(67, 23)
        Me.Cancel_Button.TabIndex = 7
        Me.Cancel_Button.Text = "Cancel"
        '
        'TbGear
        '
        Me.TbGear.Location = New System.Drawing.Point(48, 6)
        Me.TbGear.Name = "TbGear"
        Me.TbGear.ReadOnly = true
        Me.TbGear.Size = New System.Drawing.Size(37, 20)
        Me.TbGear.TabIndex = 0
        Me.TbGear.TabStop = false
        '
        'Label1
        '
        Me.Label1.AutoSize = true
        Me.Label1.Location = New System.Drawing.Point(12, 9)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(30, 13)
        Me.Label1.TabIndex = 0
        Me.Label1.Text = "Gear"
        '
        'TbRatio
        '
        Me.TbRatio.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
        Me.TbRatio.Location = New System.Drawing.Point(139, 6)
        Me.TbRatio.Name = "TbRatio"
        Me.TbRatio.Size = New System.Drawing.Size(66, 20)
        Me.TbRatio.TabIndex = 1
        '
        'Label2
        '
        Me.Label2.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
        Me.Label2.AutoSize = true
        Me.Label2.Location = New System.Drawing.Point(101, 9)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(32, 13)
        Me.Label2.TabIndex = 2
        Me.Label2.Text = "Ratio"
        '
        'TbMapPath
        '
        Me.TbMapPath.Location = New System.Drawing.Point(12, 51)
        Me.TbMapPath.Name = "TbMapPath"
        Me.TbMapPath.Size = New System.Drawing.Size(476, 20)
        Me.TbMapPath.TabIndex = 2
        '
        'Label3
        '
        Me.Label3.AutoSize = true
        Me.Label3.Location = New System.Drawing.Point(11, 37)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(187, 13)
        Me.Label3.TabIndex = 5
        Me.Label3.Text = "Loss Map file or Efficiency Value [0..1]"
        '
        'BtBrowse
        '
        Me.BtBrowse.Image = CType(resources.GetObject("BtBrowse.Image"),System.Drawing.Image)
        Me.BtBrowse.Location = New System.Drawing.Point(489, 49)
        Me.BtBrowse.Name = "BtBrowse"
        Me.BtBrowse.Size = New System.Drawing.Size(24, 24)
        Me.BtBrowse.TabIndex = 3
        Me.BtBrowse.UseVisualStyleBackColor = false
        '
        'BtNext
        '
        Me.BtNext.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left),System.Windows.Forms.AnchorStyles)
        Me.BtNext.Location = New System.Drawing.Point(196, 180)
        Me.BtNext.Name = "BtNext"
        Me.BtNext.Size = New System.Drawing.Size(67, 23)
        Me.BtNext.TabIndex = 9
        Me.BtNext.Text = "&Next >"
        Me.BtNext.UseVisualStyleBackColor = true
        '
        'TbShiftPolyFile
        '
        Me.TbShiftPolyFile.Location = New System.Drawing.Point(2, 17)
        Me.TbShiftPolyFile.Name = "TbShiftPolyFile"
        Me.TbShiftPolyFile.Size = New System.Drawing.Size(476, 20)
        Me.TbShiftPolyFile.TabIndex = 0
        '
        'BtShiftPolyBrowse
        '
        Me.BtShiftPolyBrowse.Image = CType(resources.GetObject("BtShiftPolyBrowse.Image"),System.Drawing.Image)
        Me.BtShiftPolyBrowse.Location = New System.Drawing.Point(479, 16)
        Me.BtShiftPolyBrowse.Name = "BtShiftPolyBrowse"
        Me.BtShiftPolyBrowse.Size = New System.Drawing.Size(24, 24)
        Me.BtShiftPolyBrowse.TabIndex = 1
        Me.BtShiftPolyBrowse.UseVisualStyleBackColor = false
        '
        'Label4
        '
        Me.Label4.AutoSize = true
        Me.Label4.Location = New System.Drawing.Point(1, 3)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(89, 13)
        Me.Label4.TabIndex = 0
        Me.Label4.Text = "Shift polygons file"
        '
        'PnShiftPoly
        '
        Me.PnShiftPoly.Controls.Add(Me.TbShiftPolyFile)
        Me.PnShiftPoly.Controls.Add(Me.BtShiftPolyBrowse)
        Me.PnShiftPoly.Controls.Add(Me.Label4)
        Me.PnShiftPoly.Location = New System.Drawing.Point(10, 79)
        Me.PnShiftPoly.Name = "PnShiftPoly"
        Me.PnShiftPoly.Size = New System.Drawing.Size(513, 41)
        Me.PnShiftPoly.TabIndex = 4
        '
        'Label5
        '
        Me.Label5.AutoSize = true
        Me.Label5.Location = New System.Drawing.Point(1, 5)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(123, 13)
        Me.Label5.TabIndex = 0
        Me.Label5.Text = "Maximum allowed torque"
        '
        'TbMaxTorque
        '
        Me.TbMaxTorque.Location = New System.Drawing.Point(2, 19)
        Me.TbMaxTorque.Name = "TbMaxTorque"
        Me.TbMaxTorque.Size = New System.Drawing.Size(88, 20)
        Me.TbMaxTorque.TabIndex = 0
        '
        'PnFld
        '
        Me.PnFld.Controls.Add(Me.Label7)
        Me.PnFld.Controls.Add(Me.tbMaxSpeed)
        Me.PnFld.Controls.Add(Me.Label8)
        Me.PnFld.Controls.Add(Me.Label6)
        Me.PnFld.Controls.Add(Me.TbMaxTorque)
        Me.PnFld.Controls.Add(Me.Label5)
        Me.PnFld.Location = New System.Drawing.Point(10, 126)
        Me.PnFld.Name = "PnFld"
        Me.PnFld.Size = New System.Drawing.Size(513, 46)
        Me.PnFld.TabIndex = 5
        '
        'Label7
        '
        Me.Label7.AutoSize = true
        Me.Label7.Location = New System.Drawing.Point(360, 22)
        Me.Label7.Name = "Label7"
        Me.Label7.Size = New System.Drawing.Size(30, 13)
        Me.Label7.TabIndex = 5
        Me.Label7.Text = "[rpm]"
        '
        'tbMaxSpeed
        '
        Me.tbMaxSpeed.Location = New System.Drawing.Point(266, 19)
        Me.tbMaxSpeed.Name = "tbMaxSpeed"
        Me.tbMaxSpeed.Size = New System.Drawing.Size(88, 20)
        Me.tbMaxSpeed.TabIndex = 1
        '
        'Label8
        '
        Me.Label8.AutoSize = true
        Me.Label8.Location = New System.Drawing.Point(265, 5)
        Me.Label8.Name = "Label8"
        Me.Label8.Size = New System.Drawing.Size(122, 13)
        Me.Label8.TabIndex = 3
        Me.Label8.Text = "Maximum allowed speed"
        '
        'Label6
        '
        Me.Label6.AutoSize = true
        Me.Label6.Location = New System.Drawing.Point(96, 22)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(29, 13)
        Me.Label6.TabIndex = 2
        Me.Label6.Text = "[Nm]"
        '
        'BtPrevious
        '
        Me.BtPrevious.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left),System.Windows.Forms.AnchorStyles)
        Me.BtPrevious.Location = New System.Drawing.Point(129, 180)
        Me.BtPrevious.Name = "BtPrevious"
        Me.BtPrevious.Size = New System.Drawing.Size(67, 23)
        Me.BtPrevious.TabIndex = 8
        Me.BtPrevious.Text = "< &Previous"
        Me.BtPrevious.UseVisualStyleBackColor = true
        '
        'GearboxGearDialog
        '
        Me.AcceptButton = Me.OK_Button
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6!, 13!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.CancelButton = Me.Cancel_Button
        Me.ClientSize = New System.Drawing.Size(528, 218)
        Me.Controls.Add(Me.BtPrevious)
        Me.Controls.Add(Me.PnFld)
        Me.Controls.Add(Me.PnShiftPoly)
        Me.Controls.Add(Me.BtNext)
        Me.Controls.Add(Me.BtBrowse)
        Me.Controls.Add(Me.Label3)
        Me.Controls.Add(Me.TbMapPath)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.TbRatio)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.TbGear)
        Me.Controls.Add(Me.TableLayoutPanel1)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow
        Me.MaximizeBox = false
        Me.MinimizeBox = false
        Me.Name = "GearboxGearDialog"
        Me.ShowInTaskbar = false
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "Edit Gears"
        Me.TableLayoutPanel1.ResumeLayout(false)
        Me.PnShiftPoly.ResumeLayout(false)
        Me.PnShiftPoly.PerformLayout
        Me.PnFld.ResumeLayout(false)
        Me.PnFld.PerformLayout
        Me.ResumeLayout(false)
        Me.PerformLayout

End Sub
	Friend WithEvents TableLayoutPanel1 As TableLayoutPanel
	Friend WithEvents OK_Button As Button
	Friend WithEvents Cancel_Button As Button
	Friend WithEvents TbGear As TextBox
	Friend WithEvents Label1 As Label
	Friend WithEvents TbRatio As TextBox
	Friend WithEvents Label2 As Label
	Friend WithEvents TbMapPath As TextBox
	Friend WithEvents Label3 As Label
	Friend WithEvents BtBrowse As Button
	Friend WithEvents BtNext As Button
	Friend WithEvents TbShiftPolyFile As TextBox
	Friend WithEvents BtShiftPolyBrowse As Button
	Friend WithEvents Label4 As Label
	Friend WithEvents PnShiftPoly As Panel
	Friend WithEvents Label5 As Label
	Friend WithEvents TbMaxTorque As TextBox
	Friend WithEvents PnFld As Panel
	Friend WithEvents BtPrevious As Button
	Friend WithEvents Label6 As Label
	Friend WithEvents Label7 As System.Windows.Forms.Label
	Friend WithEvents tbMaxSpeed As System.Windows.Forms.TextBox
	Friend WithEvents Label8 As System.Windows.Forms.Label

End Class
