Imports System.IO
Imports TUGraz.VECTO.Input_Files

Public Class IHPCPowerMapInputDialog
    
    Private _inputFilePath As String
    Public IHPCPath As String

    public Sub Clear()
        _tbGear.Text = ""
        _tbInputFile.Text = ""
        _tbGear.Focus()
    End Sub



#Region "Events"

    Private Sub btAddInput_Click(sender As Object, e As EventArgs) Handles btAddInput.Click
        Dim gear As Integer
        If  Not Integer.TryParse(tbGear.Text, gear) Then
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
        tmp.Init(IHPCPath, tbInputFile.Text)
        If Not File.Exists(tmp.FullPath) Then
            MsgBox("Invalid input no valid file path given")
            _tbInputFile.Focus()
            Return
        End If
        
        Dim fileExtension = new FileInfo(tbInputFile.Text).Extension
        If Not $".{IHPCPowerMapFileBrowser.Extensions.First()}" = fileExtension Then
            MsgBox($"The selected Power Map file(.{IHPCPowerMapFileBrowser.Extensions.First()}) has the wrong file extension")
            Return
        End If

        DialogResult = DialogResult.OK
        Close()
    End Sub

    Private Sub btCancel_Click(sender As Object, e As EventArgs) Handles btCancel.Click
        DialogResult = DialogResult.Cancel
        Close()
        Clear()
    End Sub

    Private Sub btAddFilePath_Click(sender As Object, e As EventArgs) Handles btAddFilePath.Click
        If IHPCPowerMapFileBrowser.OpenDialog(FileRepl(tbInputFile.Text, IHPCPath))
            tbInputFile.Text = GetFilenameWithoutDirectory(IHPCPowerMapFileBrowser.Files(0),IHPCPath)
        End If
    End Sub

    Private Sub IHPCPowerMapInputDialog_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Show()
        _tbGear.Focus()
    End Sub

#End Region

End Class