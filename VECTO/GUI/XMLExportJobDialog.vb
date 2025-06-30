Imports System.IO
Imports System.Xml.Linq
Imports Ninject
Imports TUGraz.VectoCommon.Exceptions
Imports TUGraz.VectoCommon.InputData
Imports TUGraz.VectoCommon.Models
Imports TUGraz.VectoCore
Imports TUGraz.VectoCore.OutputData.XML
Imports TUGraz.VectoCore.OutputData.XML.Engineering.Interfaces

Public Class XMLExportJobDialog
	Private _mode As ExecutionMode
	Private _data As IInputDataProvider

	Public Sub Initialize(data As IInputDataProvider)
		Dim source As String
		Dim allowSingleFile As Boolean
		Dim eng As IEngineeringInputDataProvider = TryCast(data, IEngineeringInputDataProvider)
		If (Not eng Is Nothing AndAlso Not eng.JobInputData().SavedInDeclarationMode) Then
			source = eng.JobInputData().JobName
			_mode = ExecutionMode.Engineering
			allowSingleFile = True
		Else
			Dim decl As IDeclarationInputDataProvider = TryCast(data, IDeclarationInputDataProvider)
			If (Not decl Is Nothing AndAlso decl.JobInputData().SavedInDeclarationMode) Then
				source = decl.JobInputData().JobName
				_mode = ExecutionMode.Declaration
				allowSingleFile = False
			Else
				Throw New VectoException("Input data is neither declaration nor engineering mode!")
			End If
		End If
		_data = data
		tbJobfile.Text = source
		tbMode.Text = If(_mode = ExecutionMode.Engineering, "Engineering Mode", "Declaration Mode")
		cbSingleFile.Checked = True
		cbSingleFile.Enabled = allowSingleFile
	End Sub

	Private Sub btnBrowseOutputDir_Click(sender As Object, e As EventArgs) Handles BtTCfileBrowse.Click
		If Not FolderFileBrowser.OpenDialog("") Then
			Exit Sub
		End If

		Dim filePath As String = FolderFileBrowser.Files(0)
		tbDestination.Text = Path.GetFullPath(filePath)
	End Sub

	Private Sub btnCancel_Click(sender As Object, e As EventArgs) Handles btnCancel.Click
		Close()
	End Sub

	Private Sub btnExport_Click(sender As Object, e As EventArgs) Handles btnExport.Click
	    MsgBox("XML Export is lo nonger supported")
	    Close()
		
	End Sub
End Class