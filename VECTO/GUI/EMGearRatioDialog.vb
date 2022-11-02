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




''' <summary>
''' Axle Config Editor (Vehicle Editor sub-dialog)
''' </summary>
Public Class EMGearRatioDialog
    
	Public Sub New()
		InitializeComponent()
	End Sub

	Public Sub Clear()

		tbGear.Text = ""
		tbGearRatio.Text = ""
		tbGear.Focus()
	End Sub

	'Initialise
	Private Sub F_VEH_Axle_Load(sender As Object, e As EventArgs) Handles Me.Load
	End Sub

	'Save and close
	Private Sub OK_Button_Click(sender As Object, e As EventArgs) Handles OK_Button.Click

		If Not IsNumeric(tbGear.Text) Then
			MsgBox("Invalid input for Gear")
			tbGear.Focus()
			Return
		End If
		If Not IsNumeric(tbGearRatio.Text) Then
			MsgBox("Invalid max. torque")
			tbGearRatio.Focus()
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
End Class
