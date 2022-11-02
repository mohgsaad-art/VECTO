Imports System.IO

Public Class XMLImportJobDialog
	Private Sub btnBrowseJob_Click(sender As Object, e As EventArgs) Handles btnBrowseJob.Click
		Dim dialog As OpenFileDialog = New OpenFileDialog()
		If (dialog.ShowDialog() = DialogResult.OK) Then
			tbJobFile.Text = Path.GetFullPath(dialog.FileName)
		End If
	End Sub

	Private Sub btnBrowseOutput_Click(sender As Object, e As EventArgs) Handles btnBrowseOutput.Click
		If Not FolderFileBrowser.OpenDialog("") Then
			Exit Sub
		End If

		Dim filePath As String = FolderFileBrowser.Files(0)
		tbDestination.Text = Path.GetFullPath(filePath)
	End Sub

	Private Sub btnClose_Click(sender As Object, e As EventArgs) Handles btnClose.Click
		Close()
	End Sub

	Private Sub btnImport_Click(sender As Object, e As EventArgs) Handles btnImport.Click
		' TODO!
	End Sub
End Class