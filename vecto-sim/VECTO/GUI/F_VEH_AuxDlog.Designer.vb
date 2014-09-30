' Copyright 2014 European Union.
' Licensed under the EUPL (the 'Licence');
'
' * You may not use this work except in compliance with the Licence.
' * You may obtain a copy of the Licence at: http://ec.europa.eu/idabc/eupl
' * Unless required by applicable law or agreed to in writing,
'   software distributed under the Licence is distributed on an "AS IS" basis,
'   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
'
' See the LICENSE.txt for the specific language governing permissions and limitations.
<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class F_VEH_AuxDlog
    Inherits System.Windows.Forms.Form

    'Das Formular überschreibt den Löschvorgang, um die Komponentenliste zu bereinigen.
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

    'Wird vom Windows Form-Designer benötigt.
    Private components As System.ComponentModel.IContainer

    'Hinweis: Die folgende Prozedur ist für den Windows Form-Designer erforderlich.
    'Das Bearbeiten ist mit dem Windows Form-Designer möglich.  
    'Das Bearbeiten mit dem Code-Editor ist nicht möglich.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.OK_Button = New System.Windows.Forms.Button()
        Me.Cancel_Button = New System.Windows.Forms.Button()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.TbID = New System.Windows.Forms.TextBox()
        Me.TbPath = New System.Windows.Forms.TextBox()
        Me.BtBrowse = New System.Windows.Forms.Button()
        Me.CbType = New System.Windows.Forms.ComboBox()
        Me.LbIDhelp = New System.Windows.Forms.Label()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.CbTech = New System.Windows.Forms.ComboBox()
        Me.PnTech = New System.Windows.Forms.Panel()
        Me.LVTech = New System.Windows.Forms.ListView()
        Me.ColumnHeader1 = CType(New System.Windows.Forms.ColumnHeader(),System.Windows.Forms.ColumnHeader)
        Me.Tabs = New System.Windows.Forms.TabControl()
        Me.tabMain = New System.Windows.Forms.TabPage()
        Me.PnFile = New System.Windows.Forms.Panel()
        Me.Label6 = New System.Windows.Forms.Label()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.txtPulleyGearRatio = New System.Windows.Forms.TextBox()
        Me.txtPulleyGearEfficiency = New System.Windows.Forms.TextBox()
        Me.tabListItems = New System.Windows.Forms.TabPage()
        Me.pnlListItems = New System.Windows.Forms.Panel()
        Me.dgvInputs = New System.Windows.Forms.DataGridView()
        Me.ItemName = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.ItemValue = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.btnConsumerRemove = New System.Windows.Forms.Button()
        Me.btnConsumerAdd = New System.Windows.Forms.Button()
        Me.tabTechnologies = New System.Windows.Forms.TabPage()
        Me.TableLayoutPanel1 = New System.Windows.Forms.TableLayoutPanel()
        Me.PnTech.SuspendLayout()
        Me.Tabs.SuspendLayout()
        Me.tabMain.SuspendLayout()
        Me.PnFile.SuspendLayout()
        Me.tabListItems.SuspendLayout()
        Me.pnlListItems.SuspendLayout()
        CType(Me.dgvInputs, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.tabTechnologies.SuspendLayout()
        Me.TableLayoutPanel1.SuspendLayout()
        Me.SuspendLayout()
        '
        'OK_Button
        '
        Me.OK_Button.Anchor = System.Windows.Forms.AnchorStyles.None
        Me.OK_Button.Location = New System.Drawing.Point(3, 3)
        Me.OK_Button.Name = "OK_Button"
        Me.OK_Button.Size = New System.Drawing.Size(67, 23)
        Me.OK_Button.TabIndex = 0
        Me.OK_Button.Text = "OK"
        '
        'Cancel_Button
        '
        Me.Cancel_Button.Anchor = System.Windows.Forms.AnchorStyles.None
        Me.Cancel_Button.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.Cancel_Button.Location = New System.Drawing.Point(76, 3)
        Me.Cancel_Button.Name = "Cancel_Button"
        Me.Cancel_Button.Size = New System.Drawing.Size(67, 23)
        Me.Cancel_Button.TabIndex = 1
        Me.Cancel_Button.Text = "Cancel"
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(171, 10)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(18, 13)
        Me.Label1.TabIndex = 1
        Me.Label1.Text = "ID"
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(9, 10)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(31, 13)
        Me.Label2.TabIndex = 1
        Me.Label2.Text = "Type"
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Location = New System.Drawing.Point(7, 39)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(50, 13)
        Me.Label3.TabIndex = 1
        Me.Label3.Text = "Input File"
        '
        'TbID
        '
        Me.TbID.Location = New System.Drawing.Point(195, 7)
        Me.TbID.Name = "TbID"
        Me.TbID.Size = New System.Drawing.Size(39, 20)
        Me.TbID.TabIndex = 5
        '
        'TbPath
        '
        Me.TbPath.Location = New System.Drawing.Point(7, 55)
        Me.TbPath.Name = "TbPath"
        Me.TbPath.Size = New System.Drawing.Size(383, 20)
        Me.TbPath.TabIndex = 10
        '
        'BtBrowse
        '
        Me.BtBrowse.Location = New System.Drawing.Point(396, 53)
        Me.BtBrowse.Name = "BtBrowse"
        Me.BtBrowse.Size = New System.Drawing.Size(32, 23)
        Me.BtBrowse.TabIndex = 15
        Me.BtBrowse.Text = "..."
        Me.BtBrowse.UseVisualStyleBackColor = True
        '
        'CbType
        '
        Me.CbType.FormattingEnabled = True
        Me.CbType.Location = New System.Drawing.Point(46, 7)
        Me.CbType.Name = "CbType"
        Me.CbType.Size = New System.Drawing.Size(109, 21)
        Me.CbType.TabIndex = 0
        '
        'LbIDhelp
        '
        Me.LbIDhelp.AutoSize = True
        Me.LbIDhelp.Location = New System.Drawing.Point(240, 10)
        Me.LbIDhelp.Name = "LbIDhelp"
        Me.LbIDhelp.Size = New System.Drawing.Size(0, 13)
        Me.LbIDhelp.TabIndex = 26
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.Location = New System.Drawing.Point(6, 9)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(63, 13)
        Me.Label4.TabIndex = 1
        Me.Label4.Text = "Technology"
        '
        'CbTech
        '
        Me.CbTech.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.CbTech.FormattingEnabled = True
        Me.CbTech.Location = New System.Drawing.Point(75, 6)
        Me.CbTech.Name = "CbTech"
        Me.CbTech.Size = New System.Drawing.Size(352, 21)
        Me.CbTech.TabIndex = 27
        '
        'PnTech
        '
        Me.PnTech.Controls.Add(Me.CbTech)
        Me.PnTech.Controls.Add(Me.Label4)
        Me.PnTech.Location = New System.Drawing.Point(31, 52)
        Me.PnTech.Name = "PnTech"
        Me.PnTech.Size = New System.Drawing.Size(435, 34)
        Me.PnTech.TabIndex = 28
        '
        'LVTech
        '
        Me.LVTech.CheckBoxes = True
        Me.LVTech.Columns.AddRange(New System.Windows.Forms.ColumnHeader() {Me.ColumnHeader1})
        Me.LVTech.FullRowSelect = True
        Me.LVTech.GridLines = True
        Me.LVTech.Location = New System.Drawing.Point(31, 92)
        Me.LVTech.Name = "LVTech"
        Me.LVTech.Size = New System.Drawing.Size(435, 202)
        Me.LVTech.TabIndex = 30
        Me.LVTech.UseCompatibleStateImageBehavior = False
        Me.LVTech.View = System.Windows.Forms.View.Details
        '
        'ColumnHeader1
        '
        Me.ColumnHeader1.Text = "Technologies"
        Me.ColumnHeader1.Width = 420
        '
        'Tabs
        '
        Me.Tabs.Controls.Add(Me.tabMain)
        Me.Tabs.Controls.Add(Me.tabListItems)
        Me.Tabs.Controls.Add(Me.tabTechnologies)
        Me.Tabs.Location = New System.Drawing.Point(12, 24)
        Me.Tabs.Name = "Tabs"
        Me.Tabs.SelectedIndex = 0
        Me.Tabs.Size = New System.Drawing.Size(505, 387)
        Me.Tabs.TabIndex = 35
        '
        'tabMain
        '
        Me.tabMain.Controls.Add(Me.PnFile)
        Me.tabMain.Location = New System.Drawing.Point(4, 22)
        Me.tabMain.Name = "tabMain"
        Me.tabMain.Padding = New System.Windows.Forms.Padding(3)
        Me.tabMain.Size = New System.Drawing.Size(497, 361)
        Me.tabMain.TabIndex = 0
        Me.tabMain.Text = "Main"
        Me.tabMain.UseVisualStyleBackColor = True
        '
        'PnFile
        '
        Me.PnFile.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.PnFile.Controls.Add(Me.Label6)
        Me.PnFile.Controls.Add(Me.Label5)
        Me.PnFile.Controls.Add(Me.txtPulleyGearRatio)
        Me.PnFile.Controls.Add(Me.txtPulleyGearEfficiency)
        Me.PnFile.Controls.Add(Me.LbIDhelp)
        Me.PnFile.Controls.Add(Me.BtBrowse)
        Me.PnFile.Controls.Add(Me.CbType)
        Me.PnFile.Controls.Add(Me.TbID)
        Me.PnFile.Controls.Add(Me.TbPath)
        Me.PnFile.Controls.Add(Me.Label2)
        Me.PnFile.Controls.Add(Me.Label3)
        Me.PnFile.Controls.Add(Me.Label1)
        Me.PnFile.Location = New System.Drawing.Point(16, 55)
        Me.PnFile.Name = "PnFile"
        Me.PnFile.Size = New System.Drawing.Size(435, 231)
        Me.PnFile.TabIndex = 29
        '
        'Label6
        '
        Me.Label6.AutoSize = True
        Me.Label6.Location = New System.Drawing.Point(242, 109)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(98, 13)
        Me.Label6.TabIndex = 34
        Me.Label6.Text = "Pulley Gear Ratio : "
        '
        'Label5
        '
        Me.Label5.AutoSize = True
        Me.Label5.Location = New System.Drawing.Point(16, 109)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(119, 13)
        Me.Label5.TabIndex = 33
        Me.Label5.Text = "Pulley Gear Efficiency : "
        '
        'txtPulleyGearRatio
        '
        Me.txtPulleyGearRatio.Location = New System.Drawing.Point(346, 105)
        Me.txtPulleyGearRatio.Name = "txtPulleyGearRatio"
        Me.txtPulleyGearRatio.Size = New System.Drawing.Size(72, 20)
        Me.txtPulleyGearRatio.TabIndex = 32
        '
        'txtPulleyGearEfficiency
        '
        Me.txtPulleyGearEfficiency.Location = New System.Drawing.Point(136, 105)
        Me.txtPulleyGearEfficiency.Name = "txtPulleyGearEfficiency"
        Me.txtPulleyGearEfficiency.Size = New System.Drawing.Size(76, 20)
        Me.txtPulleyGearEfficiency.TabIndex = 31
        '
        'tabListItems
        '
        Me.tabListItems.Controls.Add(Me.pnlListItems)
        Me.tabListItems.Location = New System.Drawing.Point(4, 22)
        Me.tabListItems.Name = "tabListItems"
        Me.tabListItems.Padding = New System.Windows.Forms.Padding(3)
        Me.tabListItems.Size = New System.Drawing.Size(497, 361)
        Me.tabListItems.TabIndex = 1
        Me.tabListItems.Text = "ListItems"
        Me.tabListItems.UseVisualStyleBackColor = True
        '
        'pnlListItems
        '
        Me.pnlListItems.Controls.Add(Me.dgvInputs)
        Me.pnlListItems.Controls.Add(Me.btnConsumerRemove)
        Me.pnlListItems.Controls.Add(Me.btnConsumerAdd)
        Me.pnlListItems.Location = New System.Drawing.Point(19, 31)
        Me.pnlListItems.Name = "pnlListItems"
        Me.pnlListItems.Size = New System.Drawing.Size(452, 324)
        Me.pnlListItems.TabIndex = 35
        '
        'dgvInputs
        '
        Me.dgvInputs.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.dgvInputs.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() {Me.ItemName, Me.ItemValue})
        Me.dgvInputs.Location = New System.Drawing.Point(3, 42)
        Me.dgvInputs.Name = "dgvInputs"
        Me.dgvInputs.Size = New System.Drawing.Size(432, 268)
        Me.dgvInputs.TabIndex = 31
        '
        'ItemName
        '
        Me.ItemName.HeaderText = "ItemName"
        Me.ItemName.Name = "ItemName"
        Me.ItemName.Width = 289
        '
        'ItemValue
        '
        Me.ItemValue.HeaderText = "ItemValue"
        Me.ItemValue.Name = "ItemValue"
        '
        'btnConsumerRemove
        '
        Me.btnConsumerRemove.Enabled = False
        Me.btnConsumerRemove.Image = Global.VECTO.My.Resources.Resources.minus_circle_icon
        Me.btnConsumerRemove.Location = New System.Drawing.Point(406, 13)
        Me.btnConsumerRemove.Name = "btnConsumerRemove"
        Me.btnConsumerRemove.Size = New System.Drawing.Size(29, 23)
        Me.btnConsumerRemove.TabIndex = 34
        Me.btnConsumerRemove.UseVisualStyleBackColor = True
        '
        'btnConsumerAdd
        '
        Me.btnConsumerAdd.Image = Global.VECTO.My.Resources.Resources.plus_circle_icon
        Me.btnConsumerAdd.Location = New System.Drawing.Point(353, 13)
        Me.btnConsumerAdd.Name = "btnConsumerAdd"
        Me.btnConsumerAdd.Size = New System.Drawing.Size(29, 23)
        Me.btnConsumerAdd.TabIndex = 33
        Me.btnConsumerAdd.UseVisualStyleBackColor = True
        '
        'tabTechnologies
        '
        Me.tabTechnologies.Controls.Add(Me.PnTech)
        Me.tabTechnologies.Controls.Add(Me.LVTech)
        Me.tabTechnologies.Location = New System.Drawing.Point(4, 22)
        Me.tabTechnologies.Name = "tabTechnologies"
        Me.tabTechnologies.Size = New System.Drawing.Size(497, 361)
        Me.tabTechnologies.TabIndex = 2
        Me.tabTechnologies.Text = "Technologies"
        Me.tabTechnologies.UseVisualStyleBackColor = True
        '
        'TableLayoutPanel1
        '
        Me.TableLayoutPanel1.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.TableLayoutPanel1.ColumnCount = 2
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.TableLayoutPanel1.Controls.Add(Me.OK_Button, 0, 0)
        Me.TableLayoutPanel1.Controls.Add(Me.Cancel_Button, 1, 0)
        Me.TableLayoutPanel1.Location = New System.Drawing.Point(385, 440)
        Me.TableLayoutPanel1.Name = "TableLayoutPanel1"
        Me.TableLayoutPanel1.RowCount = 1
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.TableLayoutPanel1.Size = New System.Drawing.Size(146, 29)
        Me.TableLayoutPanel1.TabIndex = 25
        '
        'F_VEH_AuxDlog
        '
        Me.AcceptButton = Me.OK_Button
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.CancelButton = Me.Cancel_Button
        Me.ClientSize = New System.Drawing.Size(543, 481)
        Me.Controls.Add(Me.Tabs)
        Me.Controls.Add(Me.TableLayoutPanel1)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "F_VEH_AuxDlog"
        Me.ShowInTaskbar = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.Text = "Auxiliary"
        Me.PnTech.ResumeLayout(False)
        Me.PnTech.PerformLayout()
        Me.Tabs.ResumeLayout(False)
        Me.tabMain.ResumeLayout(False)
        Me.PnFile.ResumeLayout(False)
        Me.PnFile.PerformLayout()
        Me.tabListItems.ResumeLayout(False)
        Me.pnlListItems.ResumeLayout(False)
        CType(Me.dgvInputs, System.ComponentModel.ISupportInitialize).EndInit()
        Me.tabTechnologies.ResumeLayout(False)
        Me.TableLayoutPanel1.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents OK_Button As System.Windows.Forms.Button
    Friend WithEvents Cancel_Button As System.Windows.Forms.Button
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents Label3 As System.Windows.Forms.Label
    Friend WithEvents TbID As System.Windows.Forms.TextBox
    Friend WithEvents TbPath As System.Windows.Forms.TextBox
    Friend WithEvents BtBrowse As System.Windows.Forms.Button
    Friend WithEvents CbType As System.Windows.Forms.ComboBox
    Friend WithEvents LbIDhelp As System.Windows.Forms.Label
    Friend WithEvents Label4 As System.Windows.Forms.Label
    Friend WithEvents CbTech As System.Windows.Forms.ComboBox
    Friend WithEvents PnTech As System.Windows.Forms.Panel
    Friend WithEvents LVTech As System.Windows.Forms.ListView
    Friend WithEvents ColumnHeader1 As System.Windows.Forms.ColumnHeader
    Friend WithEvents Tabs As System.Windows.Forms.TabControl
    Friend WithEvents tabMain As System.Windows.Forms.TabPage
    Friend WithEvents tabListItems As System.Windows.Forms.TabPage
    Friend WithEvents btnConsumerRemove As System.Windows.Forms.Button
    Friend WithEvents btnConsumerAdd As System.Windows.Forms.Button
    Friend WithEvents dgvInputs As System.Windows.Forms.DataGridView
    Friend WithEvents ItemName As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents ItemValue As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents tabTechnologies As System.Windows.Forms.TabPage
    Friend WithEvents TableLayoutPanel1 As System.Windows.Forms.TableLayoutPanel
    Friend WithEvents PnFile As System.Windows.Forms.Panel
    Friend WithEvents Label6 As System.Windows.Forms.Label
    Friend WithEvents Label5 As System.Windows.Forms.Label
    Friend WithEvents txtPulleyGearRatio As System.Windows.Forms.TextBox
    Friend WithEvents txtPulleyGearEfficiency As System.Windows.Forms.TextBox
    Friend WithEvents pnlListItems As System.Windows.Forms.Panel

End Class
