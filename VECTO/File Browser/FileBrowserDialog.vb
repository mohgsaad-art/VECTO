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
Option Infer On

Imports System.IO
Imports System.Text
Imports Microsoft.VisualBasic.FileIO
Imports System.Runtime.InteropServices


''' <summary>
''' File Browser dialog. Entirely controlled by cFilebrowser class.
''' </summary>
Public Class FileBrowserDialog
	Private _myFolder As String
	Private _myFiles() As String
	Private _myDrive As String
	Private _updateLock As Boolean
	Private _initialized As Boolean
	Private _myId As String
	Private _myExt() As String
	Private _lastFile As String
	Private _bFileMustExist As Boolean
	Private _bOverwriteCheck As Boolean
	Private _bMultiFiles As Boolean
	Private _noExt As Boolean
	Private _bBrowseFolder As Boolean
	Private _bForceExt As Boolean
	Private _extListSingle As ArrayList
	Private _extListMulti As ArrayList
	Private _lastExt As String
	Private ReadOnly _bLightMode As Boolean

	Private Const FavText As String = "Edit Favorites..."
	Private Const EmptyText As String = " "
	Private Const NoFavString As String = "<undefined>"

	'New
	Public Sub New(lightMode As Boolean)
		' This call is required by the Windows Form Designer.
		InitializeComponent()
		' Append any initialization after the InitializeComponent() call.
		_myId = "Default"
		_updateLock = False
		_initialized = False
		_myFolder = "."
		_myDrive = ""
		_lastFile = ""
		_bOverwriteCheck = False
		_bFileMustExist = False
		_bMultiFiles = False
		_noExt = True
		_bBrowseFolder = False
		_bLightMode = lightMode
		ButtonHisFile.Enabled = Not _bLightMode
	End Sub

	'Resize
	Private Sub FB_Dialog_Resize(sender As Object, e As EventArgs) Handles Me.ResizeEnd
		Resized()
	End Sub

	'Shown
	Private Sub FileBrowser_Shown(sender As Object, e As EventArgs) Handles Me.Shown
		Resized()
		ListViewFiles.ListViewItemSorter = currentSorter
		TextBoxPath.Focus()
		TextBoxPath.SelectAll()
	End Sub

	'Resized ListView Format
	Private Sub Resized()
		
		ListViewFolder.Columns(0).Width = ListViewFolder.Width - 4
		ListViewFiles.AutoResizeColumn(1, ColumnHeaderAutoResizeStyle.ColumnContent)
		ListViewFiles.AutoResizeColumn(2, ColumnHeaderAutoResizeStyle.ColumnContent)
		ListViewFiles.AutoResizeColumn(3, ColumnHeaderAutoResizeStyle.ColumnContent)
		
		if ColumnType.Width < 41 Then ColumnType.Width = 41
		if ColumnSize.Width < 33 Then ColumnSize.Width = 33
		if ColumnChangedOn.Width < 73 Then ColumnChangedOn.Width = 73

		ColumnFileName.Width = ListViewFiles.Width - ColumnType.Width - ColumnSize.Width - ColumnChangedOn.Width - 21
	End Sub

	'SplitterMoved
	Private Sub SplitContainer1_SplitterMoved(sender As Object, e As SplitterEventArgs) _
		Handles SplitContainer1.SplitterMoved
		If _initialized Then Resized()
	End Sub

	'Closing (Overwrite-Check etc)
	Private Sub FileBrowser_FormClosing(sender As Object, e As FormClosingEventArgs) Handles Me.FormClosing
		Dim x As Int32
		Dim path As String
		Dim ext As String
		Dim hasExt As Boolean
		If DialogResult = DialogResult.OK Then
			If _bBrowseFolder Then
				path = Trim(TextBoxPath.Text)
				'If empty path: use the Current-folder(MyFolder)
				If path = "" Then
					path = _myFolder
				Else
					If Mid(path, 2, 1) <> ":" Then path = _myFolder & path
				End If
				If Not Directory.Exists(path) Then
					MsgBox("Directory " & path & " does not exist!", MsgBoxStyle.Critical)
					e.Cancel = True
					Exit Sub
				End If
				If Microsoft.VisualBasic.Right(path, 1) <> "\" Then path &= "\"
				ReDim _myFiles(0)
				_myFiles(0) = path
			Else
				'Stop if empty path
				If Trim(TextBoxPath.Text) = "" Then
					e.Cancel = True
					Exit Sub
				End If
				_lastExt = Trim(ComboBoxExt.Text)
				'Assume Files in array
				If Microsoft.VisualBasic.Left(TextBoxPath.Text, 1) = "<" And ListViewFiles.SelectedItems.Count > 0 Then
					'Multiple files selected
					ReDim _myFiles(ListViewFiles.SelectedItems.Count - 1)
					x = -1
					For Each lv0 As ListViewItem In ListViewFiles.Items
						If lv0.Selected Then
							x += 1
							_myFiles(x) = _myFolder & lv0.SubItems(0).Text
						End If
					Next
					_bMultiFiles = True
				Else
					'Single File
					path = Trim(TextBoxPath.Text)
					'Primary extension (eg for bForceExt)
					ext = Trim(ComboBoxExt.Text.Split(","c)(0))
					'If file without path then append path
					If Mid(path, 2, 1) <> ":" Then path = _myFolder & path
					'If instead of File a Folder is entered: Switch to Folder and Abort
					If Directory.Exists(path) Then
						SetFolder(path)
						e.Cancel = True
						Exit Sub
					End If
					'Force Extension
					If _bForceExt Then
						If UCase(IO.Path.GetExtension(path)) <> "." & UCase(ext) Then path &= "." & ext
						hasExt = True
					Else
						'Check whether specified a File with Ext
						hasExt = (Microsoft.VisualBasic.Len(IO.Path.GetExtension(path)) > 1)
					End If
					'If File without Extension (after bForceExt question) and it does not exist, then add primary Extension
					If Not hasExt Then
						If ext <> "*" And ext <> "" Then
							If Not File.Exists(path) Then path &= "." & ext
						End If
					End If
					'Check that File exists
					If File.Exists(path) Then
						'Yes: when bOverwriteCheck, check for Overwrite
						If _bOverwriteCheck Then
							If MsgBox("Overwrite " & path & " ?", MsgBoxStyle.YesNo) = MsgBoxResult.No Then
								e.Cancel = True
								Exit Sub
							End If
						End If
					Else
						'No: abort if bFileMustExist
						If _bFileMustExist Then
							MsgBox("The file " & path & " does not exist!", MsgBoxStyle.Critical)
							e.Cancel = True
							Exit Sub
						End If
					End If
					'Define MyFiles
					ReDim _myFiles(0)
					_myFiles(0) = path
					_bMultiFiles = False
				End If
			End If
		End If
	End Sub

	'Browse - Custom Dialog
	Public Function Browse(path As String, fileMustExist As Boolean, overwriteCheck As Boolean,
							extMode As FileBrowserFileExtensionMode,
							multiFile As Boolean, ext As String, caption As String) As Boolean
		If Not _initialized Then Init()

		'Load Folder History ContextMenu
		For x = 0 To 9
			ContextMenuHisFolder.Items(x).Text = FileBrowserFolderHistory(x)
		Next
		For x = 10 To 19
			ContextMenuHisFolder.Items(x + 1).Text = FileBrowserFolderHistory(x)
		Next

		'Options
		_bOverwriteCheck = overwriteCheck
		_bFileMustExist = fileMustExist
		_bForceExt = (extMode = FileBrowserFileExtensionMode.ForceExt)

		'Form Config
		ListViewFiles.MultiSelect = multiFile
		ButtonAll.Visible = multiFile
		_title = caption
		Text = caption

		'Ext-Combobox
		ComboBoxExt.Items.Clear()
		If _noExt Then
			ComboBoxExt.Items.Add("*")
			ComboBoxExt.SelectedIndex = 0
		Else
			Select Case extMode
				Case FileBrowserFileExtensionMode.ForceExt
					If ext = "" Then ext = _extListSingle(0).ToString
					ComboBoxExt.Items.AddRange(_extListSingle.ToArray)
					ComboBoxExt.Text = ext
					ComboBoxExt.Enabled = False
				Case FileBrowserFileExtensionMode.MultiExt, FileBrowserFileExtensionMode.SingleExt
					If extMode = FileBrowserFileExtensionMode.MultiExt Then
						ComboBoxExt.Items.AddRange(_extListMulti.ToArray)
					Else
						ComboBoxExt.Items.AddRange(_extListSingle.ToArray)
					End If
					If ext <> "" Then
						ComboBoxExt.Text = ext
					Else
						ComboBoxExt.Text = _lastExt
					End If
					ComboBoxExt.Enabled = True
			End Select
		End If


		'Define Path
		'   If no path is specified: Last folder, no file name
		If path = "" Then path = FileBrowserFolderHistory(0)

		'   If path-length too small  (Path is invalid): Last File
		If path.Length < 2 Then path = _lastFile

		'Open Folder - If no folder in the path: Last folder
		If fPATH(path) = "" Then
			'If given a file without path
			If Trim(FileBrowserFolderHistory(0)) = "" Then
				SetFolder("C:\")
			Else
				SetFolder(FileBrowserFolderHistory(0))
			End If
		Else
			'...Otherwise:
			SetFolder(fPATH(path))
		End If
		If _bBrowseFolder Then
			FolderUp()
			TextBoxPath.Text = path
		Else
			TextBoxPath.Text = IO.Path.GetFileName(path)
		End If

		'Show form ------------------------------------------------ ----
		ShowDialog()
		If DialogResult = DialogResult.OK Then
			'File / Folder History
			If _bMultiFiles Then
				_lastFile = _myFolder
				UpdateHisFolder(_myFolder)
			Else
				_lastFile = _myFiles(0)
				UpdateHisFolder(fPATH(_lastFile))
				If Not _bBrowseFolder Then UpdateHisFile(_lastFile)
			End If
			'Update Global History Folder
			For x = 0 To 9
				FileBrowserFolderHistory(x) = ContextMenuHisFolder.Items(x).Text
			Next
			For x = 10 To 19
				FileBrowserFolderHistory(x) = ContextMenuHisFolder.Items(x + 1).Text
			Next
			Return True
		Else
			Return False
		End If
	End Function

	Private _title As String

	Private _filesSortColumn As Integer
	Private _directorySortColumn As Integer

	'Close and save File / Folder History
	Public Sub SaveAndClose()
		'Folder History
		If FileBrowserFolderHistoryIninialized Then
			Try
				Dim f = My.Computer.FileSystem.OpenTextFileWriter(path.Combine(FileHistoryPath, FILE_HISTORY_DIR_FILE), False, Encoding.UTF8)
				For x = 0 To 19
					f.WriteLine(FileBrowserFolderHistory(x))
				Next
				f.Close()
				f.Dispose()
			Catch ex As Exception
			End Try
			FileBrowserFolderHistoryIninialized = False
		End If
		'File History
		If _initialized And Not _bLightMode Then
			If Not _bBrowseFolder Then
				Try
					Dim f = My.Computer.FileSystem.OpenTextFileWriter(Path.Combine(FileHistoryPath,  _myId & ".txt"), False, Encoding.UTF8)
					For x = 0 To 9
						f.WriteLine(ContextMenuHisFile.Items(x).Text)
					Next
					f.Close()
					f.Dispose()
				Catch ex As Exception
				End Try
			End If
			_initialized = False
		End If

		'Close
		Close()
	End Sub

	'Switching to FolderBrowser
	Public Sub SetFolderBrowser()
		If _initialized Then Exit Sub

		_bBrowseFolder = True
		Width = 500
		ListViewFiles.Enabled = False
		ButtonHisFile.Enabled = False
		TextBoxSearchFile.Enabled = False
		SplitContainer1.Panel2Collapsed = True
		Text = "Directory Browser"
	End Sub

	'Initialize
	Private Sub Init()
		_updateLock = True

		'Initialization for Global File Browser
		If Not FileBrowserFolderHistoryIninialized Then GlobalInit()

		'Load Drive ComboBox
		For x = 0 To UBound(Drives)
			ComboBoxDrive.Items.Add(Drives(x))
		Next

		'FolderHistory ContextMenu
		ContextMenuHisFolder.Items.Clear()
		For x = 0 To 9
			ContextMenuHisFolder.Items.Add("")
		Next
		ContextMenuHisFolder.Items.Add("-")
		For x = 10 To 19
			ContextMenuHisFolder.Items.Add("")
		Next
		ContextMenuHisFolder.Items.Add("-")
		ContextMenuHisFolder.Items.Add(FavText)

		'FileHistory ContextMenu
		If _bBrowseFolder Then
			_lastFile = FileBrowserFolderHistory(0)
		ElseIf Not _bLightMode Then
			For x = 0 To 9
				ContextMenuHisFile.Items.Add("")
			Next
			If File.Exists(path.Combine(FileHistoryPath, _myId & ".txt")) Then
				Dim f = New StreamReader(path.Combine(FileHistoryPath, _myId & ".txt"))
				Dim x = -1
				Do While Not f.EndOfStream And x < 9
					x += 1
					Dim line = f.ReadLine
					ContextMenuHisFile.Items(x).Text = line
					If x = 0 Then _lastFile = line
				Loop
				f.Close()
				f.Dispose()
			End If
		End If

		'Extension-ComboBox
		If Not _noExt Then
			_extListSingle = New ArrayList
			_extListMulti = New ArrayList
			For x = 0 To UBound(_myExt)
				_extListMulti.Add(_myExt(x))
				For Each line In _myExt(x).Split(","c)
					_extListSingle.Add(Trim(line))
				Next
			Next
			_extListMulti.Add("*")
			_extListSingle.Add("*")
		End If

		_initialized = True
		_updateLock = False
	End Sub

	Private Sub GlobalInit()

		'Create Drive List
		ReDim Drives(UBound(Directory.GetLogicalDrives()))
		Dim x = -1
		For Each drive In Directory.GetLogicalDrives()
			x += 1
			Drives(x) = Microsoft.VisualBasic.Left(drive, 2)
		Next

		'Read Folder History
		For x = 0 To 19
			FileBrowserFolderHistory(x) = EmptyText
		Next
		If File.Exists(path.Combine(FileHistoryPath, FILE_HISTORY_DIR_FILE)) Then
			Dim f = New StreamReader(path.Combine(FileHistoryPath, FILE_HISTORY_DIR_FILE))
			x = -1
			Do While Not f.EndOfStream And x < 19
				x += 1
				FileBrowserFolderHistory(x) = f.ReadLine()
			Loop
			f.Dispose()
			f.Close()
		End If

		FileBrowserFolderHistoryIninialized = True
	End Sub

	'ComboBoxDrive_SelectedIndexChanged
	Private Sub ComboBoxDrive_SelectedIndexChanged(sender As Object, e As EventArgs) _
		Handles ComboBoxDrive.SelectedIndexChanged
		If Not _updateLock Then SetFolder(ComboBoxDrive.SelectedItem.ToString)
	End Sub


	'ButtonFolderBack_Click
	Private Sub ButtonFolderBack_Click(sender As Object, e As EventArgs) Handles ButtonFolderBack.Click
		FolderUp()
	End Sub

	'TextBoxPath_KeyDown (ENTER)
	Private Sub TextBoxPath_KeyDown(sender As Object, e As KeyEventArgs) Handles TextBoxPath.KeyDown
		If e.KeyCode = Keys.Enter Then
			Dim path = TextBoxPath.Text
			If Directory.Exists(path) Then
				If _bBrowseFolder Then
					DialogResult = DialogResult.OK
					Close()
				Else
					SetFolder(path)
				End If
			Else
				DialogResult = DialogResult.OK
				Close()
			End If
		End If
	End Sub

	'ListViewFolder_SelectedIndexChanged
	Private Sub ListViewFolder_SelectedIndexChanged(sender As Object, e As EventArgs) _
		Handles ListViewFolder.SelectedIndexChanged
		If _bBrowseFolder Then
			_updateLock = True
			If ListViewFolder.SelectedItems.Count > 0 Then
				TextBoxPath.Text = ListViewFolder.SelectedItems.Item(0).Text & "\"
			End If
			_updateLock = False
		End If
	End Sub

	'ListViewFolder_MouseDoubleClick
	Private Sub ListViewFolder_MouseDoubleClick(sender As Object, e As MouseEventArgs) _
		Handles ListViewFolder.MouseDoubleClick
		If ListViewFolder.SelectedItems.Count = 0 Then Exit Sub
		SetFolder(_myFolder & ListViewFolder.SelectedItems.Item(0).Text)
	End Sub

	'ListViewFolder_KeyDown
	Private Sub ListViewFolder_KeyDown(sender As Object, e As KeyEventArgs) Handles ListViewFolder.KeyDown
		If e.KeyCode = Keys.Enter Then
			If ListViewFolder.SelectedItems.Count = 0 Then Exit Sub
			SetFolder(_myFolder & ListViewFolder.SelectedItems.Item(0).Text)
		End If
	End Sub

	'ListViewFiles_SelectedIndexChanged
	Private Sub ListViewFiles_SelectedIndexChanged(sender As Object, e As EventArgs) _
		Handles ListViewFiles.SelectedIndexChanged
		_updateLock = True
		If ListViewFiles.SelectedItems.Count = 0 Then
			TextBoxPath.Text = ""
		Else
			If ListViewFiles.SelectedItems.Count > 1 Then
				TextBoxPath.Text = $"<{ListViewFiles.SelectedItems.Count} Files selected>"
			Else
				TextBoxPath.Text = ListViewFiles.SelectedItems.Item(0).Text
				TextBoxPath.SelectionStart = TextBoxPath.Text.Length
			End If
		End If
		_updateLock = False
	End Sub

	'ListViewFiles_MouseDoubleClick
	Private Sub ListViewFiles_MouseDoubleClick(sender As Object, e As MouseEventArgs) _
		Handles ListViewFiles.MouseDoubleClick
		If ListViewFiles.SelectedItems.Count = 0 Then Exit Sub

		DialogResult = DialogResult.OK
		Close()
	End Sub

	'ListViewFiles_KeyDown
	Private Sub ListViewFiles_KeyDown(sender As Object, e As KeyEventArgs) Handles ListViewFiles.KeyDown
		If e.KeyCode = Keys.Enter Then
			If ListViewFiles.SelectedItems.Count = 0 Then Exit Sub
			DialogResult = DialogResult.OK
			Close()
		End If
	End Sub

	'TextBoxSearchFolder_KeyDown
	Private Sub TextBoxSearchFolder_KeyDown(sender As Object, e As KeyEventArgs) Handles TextBoxSearchFolder.KeyDown
		Dim itemCount = ListViewFolder.Items.Count
		Dim noItem = (itemCount = 0)

		Dim selIndex As Integer
		If Not noItem Then
			If ListViewFolder.SelectedItems.Count = 0 Then
				selIndex = -1
			Else
				selIndex = ListViewFolder.SelectedIndices(0)
			End If
		End If
		Select Case e.KeyCode
			Case Keys.Enter
				If noItem Then Exit Sub
				If ListViewFolder.SelectedItems.Count = 0 Then ListViewFolder.SelectedIndices.Add(0)
				SetFolder(_myFolder & ListViewFolder.SelectedItems.Item(0).Text)
			Case Keys.Up
				If Not noItem Then
					If selIndex < 1 Then
						selIndex = 1
					Else
						ListViewFolder.Items(selIndex).Selected = False
					End If
					ListViewFolder.Items(selIndex - 1).Selected = True
					ListViewFolder.Items(selIndex - 1).EnsureVisible()
				End If
			Case Keys.Down
				If Not noItem And selIndex < itemCount - 1 Then
					If Not selIndex = -1 Then ListViewFolder.Items(selIndex).Selected = False
					ListViewFolder.Items(selIndex + 1).Selected = True
					ListViewFolder.Items(selIndex + 1).EnsureVisible()
				End If
			Case Keys.Back
				If TextBoxSearchFolder.Text = "" Then FolderUp()
		End Select
	End Sub

	'TextBoxSearchFolder_TextChanged
	Private Sub TextBoxSearchFolder_TextChanged(sender As Object, e As EventArgs) Handles TextBoxSearchFolder.TextChanged
		If Not _updateLock Then LoadListFolder()
	End Sub

	'TextBoxSearchFile_KeyDown
	Private Sub TextBoxSearchFile_KeyDown(sender As Object, e As KeyEventArgs) Handles TextBoxSearchFile.KeyDown
		Dim selIndex As Integer

		Dim itemCount = ListViewFiles.Items.Count
		Dim noItem = (itemCount = 0)
		If Not noItem Then
			If ListViewFiles.SelectedItems.Count = 0 Then
				selIndex = -1
			Else
				selIndex = ListViewFiles.SelectedIndices(0)
			End If
		End If
		Select Case e.KeyCode
			Case Keys.Enter
				If noItem Then Exit Sub
				If ListViewFiles.SelectedItems.Count = 0 Then ListViewFiles.SelectedIndices.Add(0)
				DialogResult = DialogResult.OK
				Close()
			Case Keys.Up
				If Not noItem Then
					If selIndex < 1 Then
						selIndex = 1
					Else
						ListViewFiles.Items(selIndex).Selected = False
					End If
					ListViewFiles.Items(selIndex - 1).Selected = True
					ListViewFiles.Items(selIndex - 1).EnsureVisible()
				End If
			Case Keys.Down
				If Not noItem And selIndex < itemCount - 1 Then
					If Not selIndex = -1 Then ListViewFiles.Items(selIndex).Selected = False
					ListViewFiles.Items(selIndex + 1).Selected = True
					ListViewFiles.Items(selIndex + 1).EnsureVisible()
				End If
		End Select
	End Sub

	Private Sub TextBoxSearchFile_TextChanged(sender As Object, e As EventArgs) _
		Handles TextBoxSearchFile.TextChanged, ComboBoxExt.TextChanged
		If Not _updateLock Then LoadListFiles()
	End Sub

	Private Sub ButtonHisFolder_Click(sender As Object, e As EventArgs) Handles ButtonHisFolder.Click
		ContextMenuHisFolder.Show(MousePosition)
	End Sub


	Private Sub ButtonHisFile_Click(sender As Object, e As EventArgs) Handles ButtonHisFile.Click
		ContextMenuHisFile.Show(MousePosition)
	End Sub

	Private Sub ButtonAll_Click(sender As Object, e As EventArgs) Handles ButtonAll.Click
		ListViewFiles.BeginUpdate()
		For i = 0 To ListViewFiles.Items.Count - 1
			ListViewFiles.Items(i).Selected = True
		Next
		ListViewFiles.EndUpdate()
	End Sub

	Private Sub ContextMenuHisFile_ItemClicked(sender As Object, e As ToolStripItemClickedEventArgs) _
		Handles ContextMenuHisFile.ItemClicked
		Dim path = e.ClickedItem.Text.ToString
		If path = EmptyText Then Exit Sub
		SetFolder(fPATH(path))
		TextBoxPath.Text = IO.Path.GetFileName(path)
	End Sub

	Private Sub ContextMenuHisFolder_ItemClicked(sender As Object, e As ToolStripItemClickedEventArgs) _
		Handles ContextMenuHisFolder.ItemClicked

		Dim path = e.ClickedItem.Text.ToString

		If path = EmptyText Then Exit Sub

		If path = FavText Then
			Dim favdlog = New FileBrowserFavoritesDialog
			If favdlog.ShowDialog(Me) = DialogResult.OK Then
				For x As Integer = 10 To 19
					path = favdlog.ListBox1.Items(x - 10).ToString()
					If path = NoFavString Then
						FileBrowserFolderHistory(x) = EmptyText
					Else
						FileBrowserFolderHistory(x) = path
					End If
					ContextMenuHisFolder.Items(x + 1).Text = FileBrowserFolderHistory(x)
				Next
			End If
		Else
			SetFolder(path)
		End If
	End Sub

	Private Sub TextBoxCurrent_MouseClick(sender As Object, e As MouseEventArgs) Handles TextBoxCurrent.MouseClick
		Dim newpath = ""
		Dim x = TextBoxCurrent.SelectionStart
		Dim path = TextBoxCurrent.Text
		Dim x1 = path.Length
		If x = x1 Then Exit Sub
		If x < 4 Then
			SetFolder(Microsoft.VisualBasic.Left(path, 2))
			Exit Sub
		End If
		Do While x1 > x
			newpath = path
			'path = Microsoft.VisualBasic.Left(path, x1 - 1)
			path = Microsoft.VisualBasic.Left(path, path.LastIndexOf("\", StringComparison.Ordinal))
			x1 = path.Length
		Loop
		SetFolder(newpath)
	End Sub

	Private Sub ButtonDesktop_Click(sender As Object, e As EventArgs) Handles ButtonDesktop.Click
		SetFolder(SpecialDirectories.Desktop.ToString)
	End Sub

	Private Sub UpdateHisFile(path As String)
		If _bLightMode Then Exit Sub

		Dim x As Integer
		For x = 0 To 8
			If UCase(ContextMenuHisFile.Items(x).Text.ToString) = UCase(path) Then Exit For
		Next
		For y = x To 1 Step -1
			ContextMenuHisFile.Items(y).Text = ContextMenuHisFile.Items(y - 1).Text
		Next
		ContextMenuHisFile.Items(0).Text = path
	End Sub

	Private Sub UpdateHisFolder(path As String)
		Dim x As Integer
		For x = 0 To 8
			If UCase(ContextMenuHisFolder.Items(x).Text.ToString) = UCase(path) Then Exit For
		Next
		For y = x To 1 Step -1
			ContextMenuHisFolder.Items(y).Text = ContextMenuHisFolder.Items(y - 1).Text
		Next
		ContextMenuHisFolder.Items(0).Text = path
	End Sub

	Public Sub UpdateHistory(path As String)
		If Not _initialized Then Init()
		UpdateHisFile(path)
		path = fPATH(path)
		Dim x As Integer
		For x = 0 To 8
			If UCase(FileBrowserFolderHistory(x)) = UCase(path) Then Exit For
		Next
		For y = x To 1 Step -1
			FileBrowserFolderHistory(y) = FileBrowserFolderHistory(y - 1)
		Next
		FileBrowserFolderHistory(0) = path
	End Sub

	'Change folder
	Private Sub SetFolder(path As String)

		'Abort if no drive specified
		If Mid(path, 2, 1) <> ":" Then Exit Sub

		_updateLock = True

		'Delete Search-fields
		TextBoxSearchFile.Text = ""
		TextBoxSearchFolder.Text = ""

		'Set Drive
		If _myDrive <> Microsoft.VisualBasic.Left(path, 2) Then
			_myDrive = UCase(Microsoft.VisualBasic.Left(path, 2))
			ComboBoxDrive.SelectedItem = _myDrive
		End If

		'Set Folder
		_myFolder = path
		If Microsoft.VisualBasic.Right(_myFolder, 1) <> "\" Then _myFolder &= "\"

		Text = _title & " " & _myFolder

		LoadListFolder()
		LoadListFiles()

		If _bBrowseFolder Then TextBoxPath.Text = ""

		TextBoxCurrent.Text = _myFolder

		'TextBoxPath.SelectionStart = TextBoxPath.Text.Length
		_updateLock = False
	End Sub

	'Folder one level up
	Private Sub FolderUp()
		If _myFolder <> "" Then
			Dim path = Microsoft.VisualBasic.Left(_myFolder, _myFolder.Length - 1)
			Dim x = path.LastIndexOf("\", StringComparison.Ordinal)
			If x > 0 Then SetFolder(Microsoft.VisualBasic.Left(path, x))
		End If
	End Sub


	Private Structure SHFILEINFO
		Public hIcon As IntPtr ' : icon
		Public iIcon As Integer	' : icondex
		Public dwAttributes As Integer ' : SFGAO_ flags
		<MarshalAs(UnmanagedType.ByValTStr, SizeConst:=260)> Public szDisplayName As String
		<MarshalAs(UnmanagedType.ByValTStr, SizeConst:=80)> Public szTypeName As String
	End Structure

	Private Const SHGFI_ICON = &H100
	Private Const SHGFI_SMALLICON = &H1
	Private Declare Ansi Function SHGetFileInfo Lib "shell32.dll" (pszPath As String, dwFileAttributes As Integer,
																ByRef psfi As SHFILEINFO, cbFileInfo As Integer, uFlags As Integer) As IntPtr

	'Load Folder-List
	Private Sub LoadListFolder()
		'Delete Folder-List
		ListViewFolder.Items.Clear()
		Dim searchPat = "*" & TextBoxSearchFolder.Text & "*"
		Try
			'Add Folder
			Dim di As New DirectoryInfo(_myFolder)
			Dim aryFi = di.GetDirectories(searchPat)
			ImageList1.Images.Clear()
			Dim x = ImageList1.Images.Count - 1
			For Each fi In aryFi
				x += 1
				Dim shinfo = New SHFILEINFO()
				shinfo.szDisplayName = New String(Chr(0), 260)
				shinfo.szTypeName = New String(Chr(0), 80)
				SHGetFileInfo(Path.Combine(_myFolder, fi.ToString()), 0, shinfo, Marshal.SizeOf(shinfo), SHGFI_ICON Or SHGFI_SMALLICON)
				Dim myIcon = Icon.FromHandle(shinfo.hIcon)
				ImageList1.Images.Add(myIcon)
				ListViewFolder.Items.Add(fi.Name, x)
			Next
		Catch ex As Exception
			ListViewFolder.Items.Add("<ERROR: " & ex.Message.ToString & ">")
		End Try
		_directorySortColumn = 0
	    ListViewFolder.Sorting = SortOrder.Ascending
	    ListViewFolder.ListViewItemSorter = new ListViewFilesItemComparer(_directorySortColumn, ListViewFolder.Sorting, CompareType.AsString)

	End Sub

	'Load File-list
	Private Sub LoadListFiles()
		'Abort if bBrowseFolder
		If _bBrowseFolder Then Exit Sub

		'Define Extension-filter
		Dim extStr As String()
		If Trim(ComboBoxExt.Text.ToString) = "" Then
			extStr = New String() {"*"}
		Else
			extStr = ComboBoxExt.Text.ToString.Split(","c)
		End If

		'Delete File-List
		ListViewFiles.Items.Clear()

		Dim searchFile = TextBoxSearchFile.Text

		ListViewFiles.BeginUpdate()
		Try
			'Add Folder
			Dim di As New DirectoryInfo(_myFolder)
			'Dim aryFi As FileInfo()
			Dim fi As FileInfo
			Dim x = ImageList1.Images.Count - 1
			For Each SearchExt In extStr
				Dim searchPat = "*" & Trim(searchFile) & "*." & Trim(SearchExt)
				Dim aryFi = di.GetFiles(searchPat)
				For Each fi In aryFi
					x += 1
					Dim shinfo = New SHFILEINFO()
					shinfo.szDisplayName = New String(Chr(0), 260)
					shinfo.szTypeName = New String(Chr(0), 80)
					SHGetFileInfo(Path.Combine(_myFolder, fi.ToString), 0, shinfo, Marshal.SizeOf(shinfo),
								SHGFI_ICON Or SHGFI_SMALLICON)
					Dim myIcon = Icon.FromHandle(shinfo.hIcon)
					ImageList1.Images.Add(myIcon)

					Dim item = New ListViewItem(fi.Name, fi.Extension)
					item.SubItems.Add(fi.Extension.Substring(1))
					item.SubItems.Add(fnFileSize(fi.Length)).Tag = fi.Length
					item.SubItems.Add(fi.LastWriteTime.ToString()).Tag = fi.LastWriteTime
					item.ImageIndex = x
					ListViewFiles.Items.Add(item)
				Next
			Next
		Catch ex As Exception
			ListViewFiles.Items.Add("<ERROR: " & ex.Message.ToString & ">")
		End Try
		Resized()

		ListViewFiles.EndUpdate()
	End Sub

	<DllImport("shlwapi", CharSet:=CharSet.Auto)>
	Private Shared Function StrFormatByteSize(fileSize As Long, buffer As StringBuilder, bufferSize As Integer) As long
	End Function

	Private Function fnFileSize(size1 As Long) As String
		Dim sb As New StringBuilder(20)
		StrFormatByteSize(size1, sb, sb.Capacity)
		Return sb.ToString()
	End Function

	'Rename File
	Private Sub RenameFileToolStripMenuItem_Click(sender As Object, e As EventArgs) _
		Handles RenameFileToolStripMenuItem.Click

		If ListViewFiles.SelectedItems.Count = 1 Then
			Dim file0 = ListViewFiles.SelectedItems(0).Text
			Dim file = file0
lb1:
			file = InputBox("New Filename", "Rename " & file0, file)
			If file <> "" Then
				If IO.File.Exists(_myFolder & file) Then
					MsgBox("File " & file & " already exists!")
					GoTo lb1
				End If
				Try
					My.Computer.FileSystem.RenameFile(_myFolder & file0, file)
					LoadListFiles()
				Catch ex As Exception
					MsgBox("Cannot rename " & file0 & "!")
				End Try
			End If
		End If
	End Sub

	'Delete File
	Private Sub DeleteFileToolStripMenuItem_Click(sender As Object, e As EventArgs) _
		Handles DeleteFileToolStripMenuItem.Click
		Dim c = ListViewFiles.SelectedItems.Count
		If c > 0 Then
			If c = 1 Then
				If MsgBox("Delete " & _myFolder & ListViewFiles.SelectedItems(0).Text & "?", MsgBoxStyle.YesNo) = MsgBoxResult.No _
					Then Exit Sub
			Else
				If MsgBox("Delete " & c & " files?", MsgBoxStyle.YesNo) = MsgBoxResult.No Then Exit Sub
			End If
			For x = 0 To c - 1
				Try
					File.Delete(_myFolder & ListViewFiles.SelectedItems(x).Text)
				Catch ex As Exception
				End Try
			Next
			LoadListFiles()
		End If
	End Sub

	'Neuer Ordner
	Private Sub ButtonNewDir_Click(sender As Object, e As EventArgs) Handles ButtonNewDir.Click
		Dim f = "New Folder"
lb10:
		f = InputBox("Create New Folder", "New Folder", f)
		If f <> "" Then
			If Directory.Exists(_myFolder & f) Then
				MsgBox("Folder " & _myFolder & f & " already exists!")
				GoTo lb10
			End If
			Try
				Directory.CreateDirectory(_myFolder & f)
				SetFolder(_myFolder & f)
			Catch ex As Exception
				MsgBox("Cannot create " & f & "!")
			End Try
		End If
	End Sub

	Private Shared Function fPATH(path As String) As String
		Dim x = path.LastIndexOf("\", StringComparison.Ordinal)
		If x = -1 Then
			Return Microsoft.VisualBasic.Left(path, 0)
		Else
			Return Microsoft.VisualBasic.Left(path, x + 1)
		End If
	End Function

	Public ReadOnly Property Files As String()
		Get
			Return _myFiles
		End Get
	End Property

	Public Property ID As String
		Get
			Return _myId
		End Get
		Set(value As String)
			_myId = value
		End Set
	End Property

	Public Property Extensions As String()
		Get
			Return _myExt
		End Get
		Set(value As String())
			_myExt = value
			_lastExt = _myExt(0)
			_noExt = False
		End Set
	End Property

	ReadOnly currentSorter As New ListViewFilesItemComparer(0, SortOrder.Ascending, CompareType.AsString)

	Private Sub ListViewFiles_ColumnClick(sender As Object, e As ColumnClickEventArgs) Handles ListViewFiles.ColumnClick
		If currentSorter.col = e.Column Then
			If currentSorter.sortOrder = SortOrder.Ascending Then
				currentSorter.sortOrder = SortOrder.Descending
			Else
				currentSorter.sortOrder = SortOrder.Ascending
			End If
		Else
			currentSorter.col = e.Column
			currentSorter.sortOrder = SortOrder.Ascending
			If e.Column = 0 Or e.Column = 1 Then
				currentSorter.compareType = CompareType.AsString
			ElseIf e.Column = 3 Then
				currentSorter.compareType = CompareType.AsDate
			Else
				currentSorter.compareType = CompareType.AsNumber
			End If
		End If
		ListViewFiles.Sort()
	End Sub

	Public Enum CompareType
		AsString
		AsNumber
		AsDate
	End Enum

	Private Class ListViewFilesItemComparer 
		Implements IComparer

		public col As Integer
		public compareType As CompareType
		public sortOrder As SortOrder

		Sub New(column As Integer, sortOrder As SortOrder, compareType As CompareType)
			col = column
			Me.sortOrder = sortOrder
			Me.compareType = compareType
		End Sub

		Function Compare(x1 As Object, y1 As Object) As Integer Implements IComparer.Compare
			Dim x = CType(x1, ListViewItem)
			Dim y = CType(y1, ListViewItem)
			If compareType = CompareType.AsString Then
				Return String.Compare(x.SubItems(col).Text, y.SubItems(col).Text) * (sortOrder * -2 + 3)
			ElseIf compareType = CompareType.AsDate Then
				Return CType(x.SubItems(col).Tag, Date).CompareTo(CType(y.SubItems(col).Tag, Date)) * (sortOrder * -2 + 3)
			Else
				Return CType(x.SubItems(col).Tag, integer).CompareTo(CType(y.SubItems(col).Tag, integer)) * (sortOrder * -2 + 3)
			End If
		End Function
	End Class
End Class


