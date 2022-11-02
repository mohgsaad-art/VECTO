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
Partial Class FileBrowserDialog
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
        Me.components = New System.ComponentModel.Container()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(FileBrowserDialog))
        Me.SplitContainer1 = New System.Windows.Forms.SplitContainer()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.ButtonNewDir = New System.Windows.Forms.Button()
        Me.ButtonDesktop = New System.Windows.Forms.Button()
        Me.ButtonHisFolder = New System.Windows.Forms.Button()
        Me.ButtonFolderBack = New System.Windows.Forms.Button()
        Me.TextBoxSearchFolder = New System.Windows.Forms.TextBox()
        Me.ComboBoxDrive = New System.Windows.Forms.ComboBox()
        Me.ListViewFolder = New System.Windows.Forms.ListView()
        Me.ColumnHeader3 = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.ImageList1 = New System.Windows.Forms.ImageList(Me.components)
        Me.ButtonAll = New System.Windows.Forms.Button()
        Me.ComboBoxExt = New System.Windows.Forms.ComboBox()
        Me.ButtonHisFile = New System.Windows.Forms.Button()
        Me.TextBoxSearchFile = New System.Windows.Forms.TextBox()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.ListViewFiles = New System.Windows.Forms.ListView()
        Me.ColumnFileName = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.ColumnType = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.ColumnSize = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.ColumnChangedOn = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.ContextMenuFile = New System.Windows.Forms.ContextMenuStrip(Me.components)
        Me.RenameFileToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.DeleteFileToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.TextBoxPath = New System.Windows.Forms.TextBox()
        Me.ContextMenuHisFolder = New System.Windows.Forms.ContextMenuStrip(Me.components)
        Me.ContextMenuHisFile = New System.Windows.Forms.ContextMenuStrip(Me.components)
        Me.ButtonOK = New System.Windows.Forms.Button()
        Me.ButtonCancel = New System.Windows.Forms.Button()
        Me.TextBoxCurrent = New System.Windows.Forms.TextBox()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.ToolTip1 = New System.Windows.Forms.ToolTip(Me.components)
        CType(Me.SplitContainer1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SplitContainer1.Panel1.SuspendLayout()
        Me.SplitContainer1.Panel2.SuspendLayout()
        Me.SplitContainer1.SuspendLayout()
        Me.ContextMenuFile.SuspendLayout()
        Me.SuspendLayout()
        '
        'SplitContainer1
        '
        Me.SplitContainer1.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.SplitContainer1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.SplitContainer1.Cursor = System.Windows.Forms.Cursors.VSplit
        Me.SplitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1
        Me.SplitContainer1.Location = New System.Drawing.Point(5, 44)
        Me.SplitContainer1.Margin = New System.Windows.Forms.Padding(0)
        Me.SplitContainer1.Name = "SplitContainer1"
        '
        'SplitContainer1.Panel1
        '
        Me.SplitContainer1.Panel1.Controls.Add(Me.Label1)
        Me.SplitContainer1.Panel1.Controls.Add(Me.ButtonNewDir)
        Me.SplitContainer1.Panel1.Controls.Add(Me.ButtonDesktop)
        Me.SplitContainer1.Panel1.Controls.Add(Me.ButtonHisFolder)
        Me.SplitContainer1.Panel1.Controls.Add(Me.ButtonFolderBack)
        Me.SplitContainer1.Panel1.Controls.Add(Me.TextBoxSearchFolder)
        Me.SplitContainer1.Panel1.Controls.Add(Me.ComboBoxDrive)
        Me.SplitContainer1.Panel1.Controls.Add(Me.ListViewFolder)
        Me.SplitContainer1.Panel1.Cursor = System.Windows.Forms.Cursors.Default
        Me.SplitContainer1.Panel1MinSize = 195
        '
        'SplitContainer1.Panel2
        '
        Me.SplitContainer1.Panel2.Controls.Add(Me.ButtonAll)
        Me.SplitContainer1.Panel2.Controls.Add(Me.ComboBoxExt)
        Me.SplitContainer1.Panel2.Controls.Add(Me.ButtonHisFile)
        Me.SplitContainer1.Panel2.Controls.Add(Me.TextBoxSearchFile)
        Me.SplitContainer1.Panel2.Controls.Add(Me.Label5)
        Me.SplitContainer1.Panel2.Controls.Add(Me.Label2)
        Me.SplitContainer1.Panel2.Controls.Add(Me.ListViewFiles)
        Me.SplitContainer1.Panel2.Cursor = System.Windows.Forms.Cursors.Default
        Me.SplitContainer1.Size = New System.Drawing.Size(632, 289)
        Me.SplitContainer1.SplitterDistance = 195
        Me.SplitContainer1.TabIndex = 5
        Me.SplitContainer1.TabStop = False
        '
        'Label1
        '
        Me.Label1.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(2, 272)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(32, 13)
        Me.Label1.TabIndex = 28
        Me.Label1.Text = "Filter:"
        '
        'ButtonNewDir
        '
        Me.ButtonNewDir.Image = Global.TUGraz.VECTO.My.Resources.Resources.new_dir
        Me.ButtonNewDir.Location = New System.Drawing.Point(111, 3)
        Me.ButtonNewDir.Name = "ButtonNewDir"
        Me.ButtonNewDir.Size = New System.Drawing.Size(26, 25)
        Me.ButtonNewDir.TabIndex = 21
        Me.ButtonNewDir.TabStop = False
        Me.ToolTip1.SetToolTip(Me.ButtonNewDir, "Create new Directory")
        Me.ButtonNewDir.UseVisualStyleBackColor = True
        '
        'ButtonDesktop
        '
        Me.ButtonDesktop.Image = Global.TUGraz.VECTO.My.Resources.Resources.desktop
        Me.ButtonDesktop.Location = New System.Drawing.Point(85, 3)
        Me.ButtonDesktop.Name = "ButtonDesktop"
        Me.ButtonDesktop.Size = New System.Drawing.Size(26, 25)
        Me.ButtonDesktop.TabIndex = 22
        Me.ButtonDesktop.TabStop = False
        Me.ToolTip1.SetToolTip(Me.ButtonDesktop, "Go to Desktop")
        Me.ButtonDesktop.UseVisualStyleBackColor = True
        '
        'ButtonHisFolder
        '
        Me.ButtonHisFolder.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.ButtonHisFolder.Image = Global.TUGraz.VECTO.My.Resources.Resources.file_history
        Me.ButtonHisFolder.Location = New System.Drawing.Point(164, 3)
        Me.ButtonHisFolder.Name = "ButtonHisFolder"
        Me.ButtonHisFolder.Size = New System.Drawing.Size(26, 25)
        Me.ButtonHisFolder.TabIndex = 24
        Me.ButtonHisFolder.TabStop = False
        Me.ToolTip1.SetToolTip(Me.ButtonHisFolder, "History / Favorites")
        Me.ButtonHisFolder.UseVisualStyleBackColor = True
        '
        'ButtonFolderBack
        '
        Me.ButtonFolderBack.Image = CType(resources.GetObject("ButtonFolderBack.Image"), System.Drawing.Image)
        Me.ButtonFolderBack.Location = New System.Drawing.Point(59, 3)
        Me.ButtonFolderBack.Name = "ButtonFolderBack"
        Me.ButtonFolderBack.Size = New System.Drawing.Size(26, 25)
        Me.ButtonFolderBack.TabIndex = 20
        Me.ButtonFolderBack.TabStop = False
        Me.ToolTip1.SetToolTip(Me.ButtonFolderBack, "Move up one directory")
        Me.ButtonFolderBack.UseVisualStyleBackColor = True
        '
        'TextBoxSearchFolder
        '
        Me.TextBoxSearchFolder.Anchor = CType(((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.TextBoxSearchFolder.Location = New System.Drawing.Point(35, 268)
        Me.TextBoxSearchFolder.Name = "TextBoxSearchFolder"
        Me.TextBoxSearchFolder.Size = New System.Drawing.Size(160, 20)
        Me.TextBoxSearchFolder.TabIndex = 15
        Me.ToolTip1.SetToolTip(Me.TextBoxSearchFolder, "Filter the directories")
        '
        'ComboBoxDrive
        '
        Me.ComboBoxDrive.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.ComboBoxDrive.Font = New System.Drawing.Font("Segoe UI", 11.0!)
        Me.ComboBoxDrive.FormattingEnabled = True
        Me.ComboBoxDrive.Location = New System.Drawing.Point(5, 3)
        Me.ComboBoxDrive.Name = "ComboBoxDrive"
        Me.ComboBoxDrive.Size = New System.Drawing.Size(54, 28)
        Me.ComboBoxDrive.TabIndex = 5
        '
        'ListViewFolder
        '
        Me.ListViewFolder.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.ListViewFolder.Columns.AddRange(New System.Windows.Forms.ColumnHeader() {Me.ColumnHeader3})
        Me.ListViewFolder.FullRowSelect = True
        Me.ListViewFolder.GridLines = True
        Me.ListViewFolder.HideSelection = False
        Me.ListViewFolder.Location = New System.Drawing.Point(-2, 32)
        Me.ListViewFolder.MultiSelect = False
        Me.ListViewFolder.Name = "ListViewFolder"
        Me.ListViewFolder.Size = New System.Drawing.Size(198, 237)
        Me.ListViewFolder.SmallImageList = Me.ImageList1
        Me.ListViewFolder.TabIndex = 10
        Me.ListViewFolder.UseCompatibleStateImageBehavior = False
        Me.ListViewFolder.View = System.Windows.Forms.View.Details
        '
        'ColumnHeader3
        '
        Me.ColumnHeader3.Text = "Sub-Directories:"
        Me.ColumnHeader3.Width = 368
        '
        'ImageList1
        '
        Me.ImageList1.ColorDepth = System.Windows.Forms.ColorDepth.Depth32Bit
        Me.ImageList1.ImageSize = New System.Drawing.Size(16, 16)
        Me.ImageList1.TransparentColor = System.Drawing.Color.Transparent
        '
        'ButtonAll
        '
        Me.ButtonAll.Location = New System.Drawing.Point(3, 3)
        Me.ButtonAll.Name = "ButtonAll"
        Me.ButtonAll.Size = New System.Drawing.Size(71, 25)
        Me.ButtonAll.TabIndex = 19
        Me.ButtonAll.Text = "Select All"
        Me.ToolTip1.SetToolTip(Me.ButtonAll, "Select all shown files")
        Me.ButtonAll.UseVisualStyleBackColor = True
        '
        'ComboBoxExt
        '
        Me.ComboBoxExt.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.ComboBoxExt.FormattingEnabled = True
        Me.ComboBoxExt.Location = New System.Drawing.Point(308, 268)
        Me.ComboBoxExt.Name = "ComboBoxExt"
        Me.ComboBoxExt.Size = New System.Drawing.Size(124, 21)
        Me.ComboBoxExt.TabIndex = 20
        Me.ToolTip1.SetToolTip(Me.ComboBoxExt, "Filter the file type")
        '
        'ButtonHisFile
        '
        Me.ButtonHisFile.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.ButtonHisFile.Image = Global.TUGraz.VECTO.My.Resources.Resources.file_history
        Me.ButtonHisFile.Location = New System.Drawing.Point(394, 3)
        Me.ButtonHisFile.Name = "ButtonHisFile"
        Me.ButtonHisFile.Size = New System.Drawing.Size(26, 25)
        Me.ButtonHisFile.TabIndex = 24
        Me.ButtonHisFile.TabStop = False
        Me.ToolTip1.SetToolTip(Me.ButtonHisFile, "History")
        Me.ButtonHisFile.UseVisualStyleBackColor = True
        '
        'TextBoxSearchFile
        '
        Me.TextBoxSearchFile.Anchor = CType(((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.TextBoxSearchFile.Location = New System.Drawing.Point(38, 268)
        Me.TextBoxSearchFile.Name = "TextBoxSearchFile"
        Me.TextBoxSearchFile.Size = New System.Drawing.Size(261, 20)
        Me.TextBoxSearchFile.TabIndex = 15
        Me.ToolTip1.SetToolTip(Me.TextBoxSearchFile, "Filter the files")
        '
        'Label5
        '
        Me.Label5.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.Label5.AutoSize = True
        Me.Label5.Location = New System.Drawing.Point(298, 271)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(10, 13)
        Me.Label5.TabIndex = 30
        Me.Label5.Text = "."
        '
        'Label2
        '
        Me.Label2.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(2, 272)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(32, 13)
        Me.Label2.TabIndex = 29
        Me.Label2.Text = "Filter:"
        '
        'ListViewFiles
        '
        Me.ListViewFiles.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.ListViewFiles.Columns.AddRange(New System.Windows.Forms.ColumnHeader() {Me.ColumnFileName, Me.ColumnType, Me.ColumnSize, Me.ColumnChangedOn})
        Me.ListViewFiles.ContextMenuStrip = Me.ContextMenuFile
        Me.ListViewFiles.FullRowSelect = True
        Me.ListViewFiles.GridLines = True
        Me.ListViewFiles.HideSelection = False
        Me.ListViewFiles.Location = New System.Drawing.Point(-1, 32)
        Me.ListViewFiles.Name = "ListViewFiles"
        Me.ListViewFiles.Size = New System.Drawing.Size(433, 237)
        Me.ListViewFiles.SmallImageList = Me.ImageList1
        Me.ListViewFiles.TabIndex = 10
        Me.ListViewFiles.UseCompatibleStateImageBehavior = False
        Me.ListViewFiles.View = System.Windows.Forms.View.Details
        '
        'ColumnFileName
        '
        Me.ColumnFileName.Text = "Files:"
        Me.ColumnFileName.Width = 191
        '
        'ColumnType
        '
        Me.ColumnType.Text = "Type"
        '
        'ColumnSize
        '
        Me.ColumnSize.Text = "Size"
        Me.ColumnSize.Width = 48
        '
        'ColumnChangedOn
        '
        Me.ColumnChangedOn.Text = "Changed on"
        Me.ColumnChangedOn.Width = 42
        '
        'ContextMenuFile
        '
        Me.ContextMenuFile.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.RenameFileToolStripMenuItem, Me.DeleteFileToolStripMenuItem})
        Me.ContextMenuFile.Name = "ContextMenuFile"
        Me.ContextMenuFile.Size = New System.Drawing.Size(148, 48)
        '
        'RenameFileToolStripMenuItem
        '
        Me.RenameFileToolStripMenuItem.Name = "RenameFileToolStripMenuItem"
        Me.RenameFileToolStripMenuItem.Size = New System.Drawing.Size(147, 22)
        Me.RenameFileToolStripMenuItem.Text = "Rename File..."
        '
        'DeleteFileToolStripMenuItem
        '
        Me.DeleteFileToolStripMenuItem.Name = "DeleteFileToolStripMenuItem"
        Me.DeleteFileToolStripMenuItem.Size = New System.Drawing.Size(147, 22)
        Me.DeleteFileToolStripMenuItem.Text = "Delete File..."
        '
        'TextBoxPath
        '
        Me.TextBoxPath.Anchor = CType(((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.TextBoxPath.Font = New System.Drawing.Font("Segoe UI", 11.0!)
        Me.TextBoxPath.Location = New System.Drawing.Point(41, 335)
        Me.TextBoxPath.Name = "TextBoxPath"
        Me.TextBoxPath.Size = New System.Drawing.Size(596, 27)
        Me.TextBoxPath.TabIndex = 15
        '
        'ContextMenuHisFolder
        '
        Me.ContextMenuHisFolder.Name = "ContextMenuFolderHis"
        Me.ContextMenuHisFolder.Size = New System.Drawing.Size(61, 4)
        '
        'ContextMenuHisFile
        '
        Me.ContextMenuHisFile.Name = "ContextMenuFileHis"
        Me.ContextMenuHisFile.Size = New System.Drawing.Size(61, 4)
        '
        'ButtonOK
        '
        Me.ButtonOK.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.ButtonOK.DialogResult = System.Windows.Forms.DialogResult.OK
        Me.ButtonOK.Location = New System.Drawing.Point(483, 364)
        Me.ButtonOK.Name = "ButtonOK"
        Me.ButtonOK.Size = New System.Drawing.Size(75, 23)
        Me.ButtonOK.TabIndex = 20
        Me.ButtonOK.TabStop = False
        Me.ButtonOK.Text = "OK"
        Me.ButtonOK.UseVisualStyleBackColor = True
        '
        'ButtonCancel
        '
        Me.ButtonCancel.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.ButtonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.ButtonCancel.Location = New System.Drawing.Point(561, 364)
        Me.ButtonCancel.Name = "ButtonCancel"
        Me.ButtonCancel.Size = New System.Drawing.Size(75, 23)
        Me.ButtonCancel.TabIndex = 25
        Me.ButtonCancel.TabStop = False
        Me.ButtonCancel.Text = "Cancel"
        Me.ButtonCancel.UseVisualStyleBackColor = True
        '
        'TextBoxCurrent
        '
        Me.TextBoxCurrent.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.TextBoxCurrent.Cursor = System.Windows.Forms.Cursors.Hand
        Me.TextBoxCurrent.Font = New System.Drawing.Font("Segoe UI", 11.0!)
        Me.TextBoxCurrent.Location = New System.Drawing.Point(5, 18)
        Me.TextBoxCurrent.Name = "TextBoxCurrent"
        Me.TextBoxCurrent.ReadOnly = True
        Me.TextBoxCurrent.Size = New System.Drawing.Size(632, 27)
        Me.TextBoxCurrent.TabIndex = 0
        Me.TextBoxCurrent.TabStop = False
        Me.ToolTip1.SetToolTip(Me.TextBoxCurrent, "Click for changing the directory")
        '
        'Label3
        '
        Me.Label3.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.Label3.AutoSize = True
        Me.Label3.Location = New System.Drawing.Point(5, 342)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(38, 13)
        Me.Label3.TabIndex = 29
        Me.Label3.Text = "Name:"
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.Location = New System.Drawing.Point(4, 3)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(89, 13)
        Me.Label4.TabIndex = 30
        Me.Label4.Text = "Current Directory:"
        '
        'FileBrowserDialog
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.CancelButton = Me.ButtonCancel
        Me.ClientSize = New System.Drawing.Size(645, 393)
        Me.Controls.Add(Me.Label4)
        Me.Controls.Add(Me.TextBoxCurrent)
        Me.Controls.Add(Me.ButtonCancel)
        Me.Controls.Add(Me.ButtonOK)
        Me.Controls.Add(Me.TextBoxPath)
        Me.Controls.Add(Me.SplitContainer1)
        Me.Controls.Add(Me.Label3)
        Me.MinimizeBox = False
        Me.MinimumSize = New System.Drawing.Size(341, 272)
        Me.Name = "FileBrowserDialog"
        Me.ShowIcon = False
        Me.ShowInTaskbar = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.Text = "File Browser"
        Me.SplitContainer1.Panel1.ResumeLayout(False)
        Me.SplitContainer1.Panel1.PerformLayout()
        Me.SplitContainer1.Panel2.ResumeLayout(False)
        Me.SplitContainer1.Panel2.PerformLayout()
        CType(Me.SplitContainer1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.SplitContainer1.ResumeLayout(False)
        Me.ContextMenuFile.ResumeLayout(False)
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
	Friend WithEvents SplitContainer1 As SplitContainer
	Friend WithEvents ComboBoxDrive As ComboBox
	Friend WithEvents ListViewFolder As ListView
	Friend WithEvents ListViewFiles As ListView
	Friend WithEvents ColumnFileName As ColumnHeader
	Friend WithEvents TextBoxSearchFolder As TextBox
	Friend WithEvents TextBoxPath As TextBox
	Friend WithEvents ButtonFolderBack As Button
	Friend WithEvents ContextMenuHisFolder As ContextMenuStrip
	Friend WithEvents ContextMenuHisFile As ContextMenuStrip
	Friend WithEvents ButtonHisFolder As Button
	Friend WithEvents ButtonHisFile As Button
	Friend WithEvents ButtonOK As Button
	Friend WithEvents ButtonCancel As Button
	Friend WithEvents TextBoxCurrent As TextBox
	Friend WithEvents ColumnHeader3 As ColumnHeader
	Friend WithEvents ButtonDesktop As Button
	Friend WithEvents ContextMenuFile As ContextMenuStrip
	Friend WithEvents RenameFileToolStripMenuItem As ToolStripMenuItem
	Friend WithEvents DeleteFileToolStripMenuItem As ToolStripMenuItem
	Friend WithEvents ButtonNewDir As Button
	Friend WithEvents ButtonAll As Button
	Friend WithEvents Label1 As Label
	Friend WithEvents Label3 As Label
	Friend WithEvents Label4 As Label
	Friend WithEvents ImageList1 As ImageList
	Friend WithEvents ToolTip1 As ToolTip
	Friend WithEvents Label5 As Label
	Friend WithEvents Label2 As Label
	Friend WithEvents ComboBoxExt As ComboBox
	Friend WithEvents TextBoxSearchFile As TextBox
	Friend WithEvents ColumnType As ColumnHeader
	Friend WithEvents ColumnSize As ColumnHeader
	Friend WithEvents ColumnChangedOn As ColumnHeader
End Class
