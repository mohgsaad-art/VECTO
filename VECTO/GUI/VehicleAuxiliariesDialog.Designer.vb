' Copyright 2017 European Union.
' Licensed under the EUPL (the 'Licence');
'
' * You may not use this work except in compliance with the Licence.
' * You may obtain a copy of the Licence at: http://ec.europa.eu/idabc/eupl
' * Unless required by applicable law or agreed to in writing,
'   software distributed under the Licence is distributed on an "AS IS" basis,
'   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
'
' See the LICENSE.txt for the specific language governing permissions and limitations.
Imports System.ComponentModel
Imports Microsoft.VisualBasic.CompilerServices

<DesignerGenerated()> _
Partial Class VehicleAuxiliariesDialog
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
        Me.TableLayoutPanel1 = New System.Windows.Forms.TableLayoutPanel()
        Me.OK_Button = New System.Windows.Forms.Button()
        Me.Cancel_Button = New System.Windows.Forms.Button()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.TbID = New System.Windows.Forms.TextBox()
        Me.CbType = New System.Windows.Forms.ComboBox()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.CbTech = New System.Windows.Forms.ComboBox()
        Me.PnTech = New System.Windows.Forms.Panel()
        Me.LbAxl4 = New System.Windows.Forms.Label()
        Me.LbAxl3 = New System.Windows.Forms.Label()
        Me.lbAxl2 = New System.Windows.Forms.Label()
        Me.CbTech4 = New System.Windows.Forms.ComboBox()
        Me.CbTech3 = New System.Windows.Forms.ComboBox()
        Me.CbTech2 = New System.Windows.Forms.ComboBox()
        Me.TableLayoutPanel1.SuspendLayout
        Me.PnTech.SuspendLayout
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
        Me.TableLayoutPanel1.Location = New System.Drawing.Point(301, 173)
        Me.TableLayoutPanel1.Name = "TableLayoutPanel1"
        Me.TableLayoutPanel1.RowCount = 1
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50!))
        Me.TableLayoutPanel1.Size = New System.Drawing.Size(146, 29)
        Me.TableLayoutPanel1.TabIndex = 25
        '
        'OK_Button
        '
        Me.OK_Button.Anchor = System.Windows.Forms.AnchorStyles.None
        Me.OK_Button.DialogResult = System.Windows.Forms.DialogResult.OK
        Me.OK_Button.Location = New System.Drawing.Point(3, 3)
        Me.OK_Button.Name = "OK_Button"
        Me.OK_Button.Size = New System.Drawing.Size(67, 23)
        Me.OK_Button.TabIndex = 5
        Me.OK_Button.Text = "OK"
        '
        'Cancel_Button
        '
        Me.Cancel_Button.Anchor = System.Windows.Forms.AnchorStyles.None
        Me.Cancel_Button.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.Cancel_Button.Location = New System.Drawing.Point(76, 3)
        Me.Cancel_Button.Name = "Cancel_Button"
        Me.Cancel_Button.Size = New System.Drawing.Size(67, 23)
        Me.Cancel_Button.TabIndex = 6
        Me.Cancel_Button.Text = "Cancel"
        '
        'Label1
        '
        Me.Label1.AutoSize = true
        Me.Label1.Location = New System.Drawing.Point(174, 15)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(18, 13)
        Me.Label1.TabIndex = 1
        Me.Label1.Text = "ID"
        '
        'Label2
        '
        Me.Label2.AutoSize = true
        Me.Label2.Location = New System.Drawing.Point(12, 15)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(31, 13)
        Me.Label2.TabIndex = 1
        Me.Label2.Text = "Type"
        '
        'TbID
        '
        Me.TbID.Location = New System.Drawing.Point(198, 12)
        Me.TbID.Name = "TbID"
        Me.TbID.ReadOnly = true
        Me.TbID.Size = New System.Drawing.Size(39, 20)
        Me.TbID.TabIndex = 5
        '
        'CbType
        '
        Me.CbType.FormattingEnabled = true
        Me.CbType.Location = New System.Drawing.Point(49, 12)
        Me.CbType.Name = "CbType"
        Me.CbType.Size = New System.Drawing.Size(109, 21)
        Me.CbType.TabIndex = 0
        '
        'Label4
        '
        Me.Label4.AutoSize = true
        Me.Label4.Location = New System.Drawing.Point(6, 9)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(63, 13)
        Me.Label4.TabIndex = 1
        Me.Label4.Text = "Technology"
        '
        'CbTech
        '
        Me.CbTech.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.CbTech.FormattingEnabled = true
        Me.CbTech.Location = New System.Drawing.Point(75, 6)
        Me.CbTech.Name = "CbTech"
        Me.CbTech.Size = New System.Drawing.Size(352, 21)
        Me.CbTech.TabIndex = 1
        '
        'PnTech
        '
        Me.PnTech.Controls.Add(Me.LbAxl4)
        Me.PnTech.Controls.Add(Me.LbAxl3)
        Me.PnTech.Controls.Add(Me.lbAxl2)
        Me.PnTech.Controls.Add(Me.CbTech4)
        Me.PnTech.Controls.Add(Me.CbTech3)
        Me.PnTech.Controls.Add(Me.CbTech2)
        Me.PnTech.Controls.Add(Me.CbTech)
        Me.PnTech.Controls.Add(Me.Label4)
        Me.PnTech.Location = New System.Drawing.Point(9, 44)
        Me.PnTech.Name = "PnTech"
        Me.PnTech.Size = New System.Drawing.Size(435, 119)
        Me.PnTech.TabIndex = 28
        '
        'LbAxl4
        '
        Me.LbAxl4.AutoSize = true
        Me.LbAxl4.Location = New System.Drawing.Point(30, 90)
        Me.LbAxl4.Name = "LbAxl4"
        Me.LbAxl4.Size = New System.Drawing.Size(39, 13)
        Me.LbAxl4.TabIndex = 33
        Me.LbAxl4.Text = "4. Axle"
        '
        'LbAxl3
        '
        Me.LbAxl3.AutoSize = true
        Me.LbAxl3.Location = New System.Drawing.Point(30, 63)
        Me.LbAxl3.Name = "LbAxl3"
        Me.LbAxl3.Size = New System.Drawing.Size(39, 13)
        Me.LbAxl3.TabIndex = 32
        Me.LbAxl3.Text = "3. Axle"
        '
        'lbAxl2
        '
        Me.lbAxl2.AutoSize = true
        Me.lbAxl2.Location = New System.Drawing.Point(30, 36)
        Me.lbAxl2.Name = "lbAxl2"
        Me.lbAxl2.Size = New System.Drawing.Size(39, 13)
        Me.lbAxl2.TabIndex = 31
        Me.lbAxl2.Text = "2. Axle"
        '
        'CbTech4
        '
        Me.CbTech4.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.CbTech4.FormattingEnabled = true
        Me.CbTech4.Location = New System.Drawing.Point(75, 87)
        Me.CbTech4.Name = "CbTech4"
        Me.CbTech4.Size = New System.Drawing.Size(352, 21)
        Me.CbTech4.TabIndex = 4
        '
        'CbTech3
        '
        Me.CbTech3.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.CbTech3.FormattingEnabled = true
        Me.CbTech3.Location = New System.Drawing.Point(75, 60)
        Me.CbTech3.Name = "CbTech3"
        Me.CbTech3.Size = New System.Drawing.Size(352, 21)
        Me.CbTech3.TabIndex = 3
        '
        'CbTech2
        '
        Me.CbTech2.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.CbTech2.FormattingEnabled = true
        Me.CbTech2.Location = New System.Drawing.Point(75, 33)
        Me.CbTech2.Name = "CbTech2"
        Me.CbTech2.Size = New System.Drawing.Size(352, 21)
        Me.CbTech2.TabIndex = 2
        '
        'VehicleAuxiliariesDialog
        '
        Me.AcceptButton = Me.OK_Button
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6!, 13!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.CancelButton = Me.Cancel_Button
        Me.ClientSize = New System.Drawing.Size(459, 214)
        Me.Controls.Add(Me.PnTech)
        Me.Controls.Add(Me.CbType)
        Me.Controls.Add(Me.TableLayoutPanel1)
        Me.Controls.Add(Me.TbID)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.Label2)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow
        Me.MaximizeBox = false
        Me.MinimizeBox = false
        Me.Name = "VehicleAuxiliariesDialog"
        Me.ShowInTaskbar = false
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "Auxiliary"
        Me.TableLayoutPanel1.ResumeLayout(false)
        Me.PnTech.ResumeLayout(false)
        Me.PnTech.PerformLayout
        Me.ResumeLayout(false)
        Me.PerformLayout

End Sub
	Friend WithEvents TableLayoutPanel1 As TableLayoutPanel
	Friend WithEvents OK_Button As Button
	Friend WithEvents Cancel_Button As Button
	Friend WithEvents Label1 As Label
	Friend WithEvents Label2 As Label
	Friend WithEvents TbID As TextBox
	Friend WithEvents CbType As ComboBox
	Friend WithEvents Label4 As Label
	Friend WithEvents CbTech As ComboBox
	Friend WithEvents PnTech As Panel
	Friend WithEvents CbTech4 As System.Windows.Forms.ComboBox
	Friend WithEvents CbTech3 As System.Windows.Forms.ComboBox
	Friend WithEvents CbTech2 As System.Windows.Forms.ComboBox
	Friend WithEvents LbAxl4 As System.Windows.Forms.Label
	Friend WithEvents LbAxl3 As System.Windows.Forms.Label
	Friend WithEvents lbAxl2 As System.Windows.Forms.Label

End Class
