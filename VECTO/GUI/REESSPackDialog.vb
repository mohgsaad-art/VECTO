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
Option Strict On
Option Explicit On

Imports System.IO


''' <summary>
''' Axle Config Editor (Vehicle Editor sub-dialog)
''' </summary>
Public Class REESSPackDialog
    
    Public _vehFile As String

    Public Sub New()
		InitializeComponent()
	End Sub

	Public Sub Clear()

		tbBattery.Text = ""
		tbBatteryPackCnt.Text = ""
        tbStreamId.Text = ""
		tbBattery.Focus()
	End Sub

	'Initialise
	Private Sub F_VEH_Axle_Load(sender As Object, e As EventArgs) Handles Me.Load
	End Sub

	'Save and close
	Private Sub OK_Button_Click(sender As Object, e As EventArgs) Handles OK_Button.Click

		If Not IsNumeric(tbStreamId.Text) Then
			MsgBox("Invalid input for Stream #")
			tbStreamId.Focus()
			Return
		End If
		If Not IsNumeric(tbBatteryPackCnt.Text) Then
			MsgBox("Invalid REESS Count")
			tbBatteryPackCnt.Focus()
			Return
		End If

		DialogResult = DialogResult.OK
		Close()
	End Sub

	'Cancel
	Private Sub Cancel_Button_Click(sender As Object, e As EventArgs) Handles Cancel_Button.Click
		DialogResult = DialogResult.Cancel
		Close()
	End Sub

    Private Sub btnBrowseBattery_Click(sender As Object, e As EventArgs) Handles btnBrowseBattery.Click
        If REESSFileBrowser.OpenDialog(FileRepl(tbBattery.Text, GetPath(_vehFile))) Then
            tbBattery.Text = GetFilenameWithoutDirectory(REESSFileBrowser.Files(0), GetPath(_vehFile))
        End If
    End Sub

    Private Sub btnOpenBattery_Click(sender As Object, e As EventArgs) Handles btnOpenBattery.Click
        Dim f As String
        f = FileRepl(tbBattery.Text, GetPath(_vehFile))

        'Thus Veh-file is returned
        BatteryForm.JobDir = GetPath(_vehFile)
        BatteryForm.AutoSendTo = True

        If Not Trim(f) = "" Then
            If Not File.Exists(f) Then
                MsgBox("File not found!")
                Exit Sub
            End If
        End If

        if Not BatteryForm.Visible Then
            BatteryForm.Show()
        Else 
            If BatteryForm.WindowState = FormWindowState.Minimized Then BatteryForm.WindowState = FormWindowState.Normal
            BatteryForm.BringToFront()
        End If

        If Not Trim(f) = "" Then
            Try
                BatteryForm.OpenBatteryFile(f)
            Catch ex As Exception
                MsgBox(ex.Message, MsgBoxStyle.OkOnly, "Error loading Vehicle File")
            End Try
        End If
    End Sub
End Class
