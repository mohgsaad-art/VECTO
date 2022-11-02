<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class IEPCGearInputDialog
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
        Me.TableLayoutPanel1 = New System.Windows.Forms.TableLayoutPanel()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.Label6 = New System.Windows.Forms.Label()
        Me.tbRatio = New System.Windows.Forms.TextBox()
        Me.tbMaxOutShaftTorque = New System.Windows.Forms.TextBox()
        Me.tbMaxOutShaftSpeed = New System.Windows.Forms.TextBox()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.TableLayoutPanel5 = New System.Windows.Forms.TableLayoutPanel()
        Me.btnOk = New System.Windows.Forms.Button()
        Me.btnCancel = New System.Windows.Forms.Button()
        Me.TableLayoutPanel1.SuspendLayout
        Me.TableLayoutPanel5.SuspendLayout
        Me.SuspendLayout
        '
        'TableLayoutPanel1
        '
        Me.TableLayoutPanel1.ColumnCount = 6
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 25!))
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 120!))
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 85!))
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 38!))
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 25!))
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20!))
        Me.TableLayoutPanel1.Controls.Add(Me.Label5, 3, 0)
        Me.TableLayoutPanel1.Controls.Add(Me.Label4, 3, 1)
        Me.TableLayoutPanel1.Controls.Add(Me.Label6, 3, 2)
        Me.TableLayoutPanel1.Controls.Add(Me.tbRatio, 2, 0)
        Me.TableLayoutPanel1.Controls.Add(Me.tbMaxOutShaftTorque, 2, 1)
        Me.TableLayoutPanel1.Controls.Add(Me.tbMaxOutShaftSpeed, 2, 2)
        Me.TableLayoutPanel1.Controls.Add(Me.Label3, 1, 0)
        Me.TableLayoutPanel1.Controls.Add(Me.Label1, 1, 1)
        Me.TableLayoutPanel1.Controls.Add(Me.Label2, 1, 2)
        Me.TableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Top
        Me.TableLayoutPanel1.Location = New System.Drawing.Point(0, 0)
        Me.TableLayoutPanel1.Name = "TableLayoutPanel1"
        Me.TableLayoutPanel1.RowCount = 3
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30!))
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30!))
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30!))
        Me.TableLayoutPanel1.Size = New System.Drawing.Size(295, 91)
        Me.TableLayoutPanel1.TabIndex = 0
        '
        'Label5
        '
        Me.Label5.Anchor = System.Windows.Forms.AnchorStyles.None
        Me.Label5.AutoSize = true
        Me.Label5.Location = New System.Drawing.Point(241, 8)
        Me.Label5.Margin = New System.Windows.Forms.Padding(0)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(16, 13)
        Me.Label5.TabIndex = 64
        Me.Label5.Text = "[-]"
        '
        'Label4
        '
        Me.Label4.Anchor = System.Windows.Forms.AnchorStyles.None
        Me.Label4.AutoSize = true
        Me.Label4.Location = New System.Drawing.Point(234, 38)
        Me.Label4.Margin = New System.Windows.Forms.Padding(0)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(29, 13)
        Me.Label4.TabIndex = 64
        Me.Label4.Text = "[Nm]"
        '
        'Label6
        '
        Me.Label6.Anchor = System.Windows.Forms.AnchorStyles.None
        Me.Label6.AutoSize = true
        Me.Label6.Location = New System.Drawing.Point(234, 69)
        Me.Label6.Margin = New System.Windows.Forms.Padding(0)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(30, 13)
        Me.Label6.TabIndex = 65
        Me.Label6.Text = "[rpm]"
        '
        'tbRatio
        '
        Me.tbRatio.Anchor = System.Windows.Forms.AnchorStyles.None
        Me.tbRatio.Location = New System.Drawing.Point(149, 5)
        Me.tbRatio.Name = "tbRatio"
        Me.tbRatio.Size = New System.Drawing.Size(77, 20)
        Me.tbRatio.TabIndex = 1
        '
        'tbMaxOutShaftTorque
        '
        Me.tbMaxOutShaftTorque.Anchor = System.Windows.Forms.AnchorStyles.None
        Me.tbMaxOutShaftTorque.Location = New System.Drawing.Point(149, 35)
        Me.tbMaxOutShaftTorque.Name = "tbMaxOutShaftTorque"
        Me.tbMaxOutShaftTorque.Size = New System.Drawing.Size(77, 20)
        Me.tbMaxOutShaftTorque.TabIndex = 2
        '
        'tbMaxOutShaftSpeed
        '
        Me.tbMaxOutShaftSpeed.Anchor = System.Windows.Forms.AnchorStyles.None
        Me.tbMaxOutShaftSpeed.Location = New System.Drawing.Point(149, 65)
        Me.tbMaxOutShaftSpeed.Name = "tbMaxOutShaftSpeed"
        Me.tbMaxOutShaftSpeed.Size = New System.Drawing.Size(77, 20)
        Me.tbMaxOutShaftSpeed.TabIndex = 3
        '
        'Label3
        '
        Me.Label3.Anchor = System.Windows.Forms.AnchorStyles.Right
        Me.Label3.AutoSize = true
        Me.Label3.Location = New System.Drawing.Point(113, 8)
        Me.Label3.Margin = New System.Windows.Forms.Padding(0)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(32, 13)
        Me.Label3.TabIndex = 9
        Me.Label3.Text = "Ratio"
        '
        'Label1
        '
        Me.Label1.Anchor = System.Windows.Forms.AnchorStyles.Right
        Me.Label1.AutoSize = true
        Me.Label1.Location = New System.Drawing.Point(41, 38)
        Me.Label1.Margin = New System.Windows.Forms.Padding(0)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(104, 13)
        Me.Label1.TabIndex = 9
        Me.Label1.Text = "Max out shaft torque"
        '
        'Label2
        '
        Me.Label2.Anchor = System.Windows.Forms.AnchorStyles.Right
        Me.Label2.AutoSize = true
        Me.Label2.Location = New System.Drawing.Point(42, 69)
        Me.Label2.Margin = New System.Windows.Forms.Padding(0)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(103, 13)
        Me.Label2.TabIndex = 9
        Me.Label2.Text = "Max out shaft speed"
        '
        'TableLayoutPanel5
        '
        Me.TableLayoutPanel5.ColumnCount = 2
        Me.TableLayoutPanel5.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50!))
        Me.TableLayoutPanel5.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50!))
        Me.TableLayoutPanel5.Controls.Add(Me.btnOk, 0, 0)
        Me.TableLayoutPanel5.Controls.Add(Me.btnCancel, 1, 0)
        Me.TableLayoutPanel5.Location = New System.Drawing.Point(147, 106)
        Me.TableLayoutPanel5.Name = "TableLayoutPanel5"
        Me.TableLayoutPanel5.RowCount = 1
        Me.TableLayoutPanel5.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50!))
        Me.TableLayoutPanel5.Size = New System.Drawing.Size(148, 29)
        Me.TableLayoutPanel5.TabIndex = 5
        '
        'btnOk
        '
        Me.btnOk.Anchor = System.Windows.Forms.AnchorStyles.None
        Me.btnOk.Location = New System.Drawing.Point(3, 3)
        Me.btnOk.Name = "btnOk"
        Me.btnOk.Size = New System.Drawing.Size(67, 23)
        Me.btnOk.TabIndex = 4
        Me.btnOk.Text = "OK"
        '
        'btnCancel
        '
        Me.btnCancel.Anchor = System.Windows.Forms.AnchorStyles.None
        Me.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.btnCancel.Location = New System.Drawing.Point(77, 3)
        Me.btnCancel.Name = "btnCancel"
        Me.btnCancel.Size = New System.Drawing.Size(67, 23)
        Me.btnCancel.TabIndex = 5
        Me.btnCancel.Text = "Cancel"
        '
        'IEPCGearInputDialog
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6!, 13!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(295, 140)
        Me.Controls.Add(Me.TableLayoutPanel5)
        Me.Controls.Add(Me.TableLayoutPanel1)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow
        Me.MaximizeBox = false
        Me.MinimizeBox = false
        Me.Name = "IEPCGearInputDialog"
        Me.ShowInTaskbar = false
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "Gear"
        Me.TableLayoutPanel1.ResumeLayout(false)
        Me.TableLayoutPanel1.PerformLayout
        Me.TableLayoutPanel5.ResumeLayout(false)
        Me.ResumeLayout(false)

End Sub

	Friend WithEvents TableLayoutPanel1 As TableLayoutPanel
	Friend WithEvents Label1 As Label
	Friend WithEvents tbMaxOutShaftTorque As TextBox
	Friend WithEvents Label2 As Label
	Friend WithEvents tbMaxOutShaftSpeed As TextBox
	Friend WithEvents Label3 As Label
	Friend WithEvents tbRatio As TextBox
	Friend WithEvents Label4 As Label
	Friend WithEvents Label6 As Label
	Friend WithEvents Label5 As Label
	Friend WithEvents TableLayoutPanel5 As TableLayoutPanel
	Friend WithEvents btnOk As Button
	Friend WithEvents btnCancel As Button
End Class
