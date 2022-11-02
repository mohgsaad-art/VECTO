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
Imports System.IO
Imports TUGraz.VectoCommon.Utils

''' <summary>
''' Settings form
''' </summary>
''' <remarks></remarks>
Public Class Settings
	'Initialize - load config
	Private Sub F03_Options_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load

		LoadSettings()
	End Sub

	Private Sub LoadSettings()

		TextBoxLogSize.Text = Cfg.LogSize.ToGUIFormat()
		TbAirDensity.Text = CStr(Cfg.AirDensity)
		TbOpenCmd.Text = Cfg.OpenCmd
		TbOpenCmdName.Text = Cfg.OpenCmdName
		TbFuelDens.Text = Cfg.FuelDens.ToString
		TbCO2toFC.Text = Cfg.CO2perFC.ToString

		'GrCalc.Enabled = Not Cfg.DeclMode
	End Sub


	'Reset Button
	Private Sub ButReset_Click(sender As Object, e As EventArgs) Handles ButReset.Click
		If _
			MsgBox(
				"This will reset all application settings including the Options Tab. Filehistory will not be deleted." & vbCrLf &
				vbCrLf & "Continue ?", MsgBoxStyle.YesNo, "Reset Application Settings") = MsgBoxResult.Yes Then
			Cfg.SetDefault()
			If Cfg.DeclMode Then Cfg.DeclInit()
			MainForm.LoadOptions()
			LoadSettings()
			Close()
		End If
	End Sub

	'Save and close
	Private Sub ButtonOK_Click(ByVal sender As Object, ByVal e As EventArgs) Handles ButtonOK.Click
		Cfg.LogSize = CSng(TextBoxLogSize.Text)
		Cfg.AirDensity = CSng(TbAirDensity.Text)
		Cfg.OpenCmd = TbOpenCmd.Text
		Cfg.OpenCmdName = TbOpenCmdName.Text
		Cfg.FuelDens = CSng(TbFuelDens.Text)
		Cfg.CO2perFC = CSng(TbCO2toFC.Text)
		'----------------------------------------------------

		Cfg.Save()

		Close()
	End Sub

	'Cancel
	Private Sub ButtonCancel_Click(ByVal sender As Object, ByVal e As EventArgs) Handles ButtonCancel.Click
		Close()
	End Sub

	'Help button
	Private Sub BtHelp_Click(sender As Object, e As EventArgs) Handles BtHelp.Click
		If File.Exists(Path.Combine(MyAppPath, "User Manual\help.html")) Then
			Dim defaultBrowserPath As String = BrowserUtils.GetDefaultBrowserPath()
			Process.Start(defaultBrowserPath,
						$"""file://{Path.Combine(MyAppPath, "User Manual\help.html#settings")}""")
		Else
			MsgBox("User Manual not found!", MsgBoxStyle.Critical)
		End If
	End Sub
End Class
