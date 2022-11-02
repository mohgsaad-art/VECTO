Imports System.IO
Imports TUGraz.VECTO.Input_Files

Public Enum IEPCDialogType
	DragCurveDialog
	PowerMapDialog
End Enum

Public Class IEPCInputDialog

	Private ReadOnly _dialogType As IEPCDialogType
	Private _dragCurveFilePath As String
	Private _powerMapFilePath As String
	Public IEPCPath As String

	Public Sub New(dialogType As IEPCDialogType)

		' This call is required by the designer.
		InitializeComponent()

		' Add any initialization after the InitializeComponent() call.
		SetDialogTitle(dialogType)
		_dialogType = dialogType

	End Sub

	Public Sub Clear()
		_tbGear.Text = ""
		_tbInputFile.Text = ""
		_tbGear.Focus()
	End Sub


#Region "Button Handling"

	Private Sub btnOk_Click(sender As Object, e As EventArgs) Handles btnOk.Click
		Dim gear As Integer
		If Not Integer.TryParse(tbGear.Text, gear) Then
			MsgBox("Invalid input for Gear")
			_tbGear.Focus()
			Return
		End If

		If gear < 0 Then
			MsgBox("Invalid input for Gear")
			_tbGear.Focus()
			Return
		End If

		If tbInputFile.Text.Length = 0 Then
			MsgBox("Invalid input no file path given")
			_tbGear.Focus()
			Return
		End If

		Dim tmp As SubPath = New SubPath()
		tmp.Init(IEPCPath, tbInputFile.Text)
		If Not File.Exists(tmp.FullPath) Then
			MsgBox("Invalid input no valid file path given")
			_tbInputFile.Focus()
			Return
		End If
		
		Dim fileExtension = new FileInfo(tbInputFile.Text).Extension
		Select Case _dialogType
			Case IEPCDialogType.DragCurveDialog
				If Not $".{IEPCDragFileBrowser.Extensions.First()}"= fileExtension Then
					MsgBox($"The Selected Drag Curve file(.{IEPCDragFileBrowser.Extensions.First()}) has the wrong file extension")
					Return		
				End If
			Case IEPCDialogType.PowerMapDialog
				If Not $".{IEPCPowerMapFileBrowser.Extensions.First()}" = fileExtension Then
					MsgBox($"The Selected Power Map file(.{IEPCPowerMapFileBrowser.Extensions.First()}) has the wrong file extension")
					Return
				End If
		End Select

		DialogResult = DialogResult.OK
		Close()
	End Sub

	Private Sub btnCancel_Click(sender As Object, e As EventArgs) Handles btnCancel.Click
		DialogResult = DialogResult.Cancel
		Close()
		Clear()
	End Sub

	Private Sub btAddFilePath_Click(sender As Object, e As EventArgs) Handles btAddFilePath.Click
		Select Case _dialogType
			Case IEPCDialogType.DragCurveDialog
				SelectInputFileDialog(IEPCDragFileBrowser, _dragCurveFilePath)
			Case IEPCDialogType.PowerMapDialog
				SelectInputFileDialog(IEPCPowerMapFileBrowser, _powerMapFilePath)
		End Select
	End Sub

#End Region

	Private Sub SelectInputFileDialog(fileBrowser As FileBrowser, filePath As String )
		If fileBrowser.OpenDialog(FileRepl(tbInputFile.Text, IEPCPath)) Then
			tbInputFile.Text = GetFilenameWithoutDirectory(fileBrowser.Files(0), IEPCPath)
		End If
	End Sub
	
	Private Sub SetDialogTitle(dialogType As IEPCDialogType)
		Select Case dialogType
			Case IEPCDialogType.DragCurveDialog
				Text = "Drag Curve"
			Case IEPCDialogType.PowerMapDialog
				Text = "Power Map"
		End Select
	End Sub

	Private Sub IEPCInputDialog_Load(sender As Object, e As EventArgs) Handles MyBase.Load
		Show()
		_tbGear.Focus()
	End Sub

End Class