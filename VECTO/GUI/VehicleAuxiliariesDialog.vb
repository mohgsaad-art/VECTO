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

Imports System.Linq
Imports System.Windows.Forms
Imports TUGraz.VectoCommon.Models
Imports TUGraz.VectoCore.Models.Declaration


''' <summary>
''' Aux Config Editor (Job Editor sub-dialog)
''' </summary>
Public Class VehicleAuxiliariesDialog
	Public VehPath As String = ""
	Public NumAxles As Integer
	Public Const AxleNotSteered As String = "Not steered"

	Public Sub New()
		InitializeComponent()

		CbType.DisplayMember = "Caption"
		CbType.ValueMember = "Value"

		CbType.DataSource =
			[Enum].GetValues(GetType(AuxiliaryType)).Cast(Of AuxiliaryType).Select(
				Function(x) New With {Key .Caption = x.Name(), .Value = x.Key}).toarray()

		'CbType.Items.Add("Fan")
		'CbType.Items.Add("Steering pump")
		'CbType.Items.Add("HVAC")
		'CbType.Items.Add("Electric System")
		PnTech.Visible = Cfg.DeclMode

		CbTech.DisplayMember = "Caption"
		CbTech.ValueMember = "Value"
		CbTech2.DisplayMember = "Caption"
		CbTech2.ValueMember = "Value"
		CbTech3.DisplayMember = "Caption"
		CbTech3.ValueMember = "Value"
		CbTech4.DisplayMember = "Caption"
		CbTech4.ValueMember = "Value"
	End Sub

	'Initialise form
	Private Sub F_VEH_AuxDlog_Load(sender As Object, e As EventArgs) Handles Me.Load
		Text = CbType.Text
	End Sub

	'Set generic values for Declaration mode
	Private Sub DeclInit()

		CbType.Enabled = false
		CbTech2.Visible = NumAxles > 1
		CbTech3.Visible = NumAxles > 2
		CbTech4.Visible = NumAxles > 3
		lbAxl2.Visible = NumAxles > 1
		LbAxl3.Visible = NumAxles > 2
		LbAxl4.Visible = NumAxles > 3

		If CbType.SelectedItem Is Nothing Then Exit Sub

		Select Case CbType.SelectedValue.ToString()
			Case VectoCore.Configuration.Constants.Auxiliaries.IDs.Fan
				CbTech.DataSource =
					DeclarationData.Fan.GetTechnologies().Select(Function(x) New With {.Caption = x, .Value = x}).ToArray()
			Case VectoCore.Configuration.Constants.Auxiliaries.IDs.SteeringPump
				Dim notSteered = (New String() {AxleNotSteered}).Concat(DeclarationData.SteeringPump.GetTechnologies()).ToArray()
				CbTech.DataSource =
					DeclarationData.SteeringPump.GetTechnologies().Select(Function(x) New With {.Caption = x, .Value = x}).ToArray()

				CbTech2.DataSource = notSteered.Select(Function(x) New With {.Caption = x, .Value = x}).ToArray()
				CbTech3.DataSource = notSteered.Select(Function(x) New With {.Caption = x, .Value = x}).ToArray()
				CbTech4.DataSource = notSteered.Select(Function(x) New With {.Caption = x, .Value = x}).ToArray()
			Case VectoCore.Configuration.Constants.Auxiliaries.IDs.HeatingVentilationAirCondition
				CbTech.DataSource =
					DeclarationData.HeatingVentilationAirConditioning.GetTechnologies().Select(
						Function(x) New With {.Caption = x, .Value = x}).ToArray()
			Case VectoCore.Configuration.Constants.Auxiliaries.IDs.ElectricSystem
				CbTech.DataSource =
					DeclarationData.ElectricSystem.GetTechnologies().Select(Function(x) New With {.Caption = x, .Value = x}).ToArray()
			Case VectoCore.Configuration.Constants.Auxiliaries.IDs.PneumaticSystem
				CbTech.DataSource =
					DeclarationData.PneumaticSystem.GetTechnologies().Select(Function(x) New With {.Caption = x, .Value = x}).ToArray()
		End Select
		If CbTech.Items.Count > 0 Then
			'CbTech.SelectedIndex = 0
			PnTech.Enabled = True
		Else
			PnTech.Enabled = False
		End If
	End Sub

	'Close form. Check if form is complete and valid
	Private Sub F_VEH_AuxDlog_FormClosing(sender As Object, e As FormClosingEventArgs) Handles Me.FormClosing
		'If e.CloseReason <> CloseReason.WindowsShutDown And DialogResult <> DialogResult.Cancel Then

		'	If Trim(TbID.Text) = "" Or Trim(CbType.Text) = "" Then
		'		MsgBox("Form is incomplete!", MsgBoxStyle.Critical)
		'		e.Cancel = True
		'	End If

		'	If TbID.Text.Contains(",") Or CbType.Text.Contains(",") Or TbPath.Text.Contains(",") Then
		'		MsgBox("',' is no valid character!", MsgBoxStyle.Critical)
		'		e.Cancel = True
		'	End If

		'	If Cfg.DeclMode Then

		'		If CbTech.Items.Count > 0 AndAlso CbTech.Text = "" Then
		'			MsgBox("Form is incomplete!", MsgBoxStyle.Critical)
		'			e.Cancel = True
		'		End If

		'	Else

		'		If Trim(TbPath.Text) = "" Then
		'			MsgBox("Form is incomplete!", MsgBoxStyle.Critical)
		'			e.Cancel = True
		'		End If

		'	End If

		'End If
	End Sub


	'Update ID when Aux Type was changed
	Private Sub CbType_TextChanged(sender As Object, e As EventArgs) Handles CbType.TextChanged

		If CbType.Text = "" Then
			TbID.Text = ""
		Else
			TbID.Text = CbType.SelectedValue.ToString()
		End If
	End Sub

	Private Sub TbID_TextChanged(sender As Object, e As EventArgs) Handles TbID.TextChanged

		DeclInit()

	End Sub

	Private Sub CbTech_SelectedIndexChanged(sender As Object, e As EventArgs) Handles CbTech.SelectedIndexChanged
	End Sub

	Private Sub CbTech2_SelectedIndexChanged(sender As Object, e As EventArgs) Handles CbTech2.SelectedIndexChanged
		CbTech3.Enabled = Not (CbTech2.SelectedValue.ToString() = AxleNotSteered)
		If Not CbTech3.Enabled Then CbTech3.SelectedValue = AxleNotSteered
	End Sub

	Private Sub CbTech3_SelectedIndexChanged(sender As Object, e As EventArgs) Handles CbTech3.SelectedIndexChanged
		CbTech4.Enabled = Not (CbTech3.SelectedValue.ToString() = AxleNotSteered)
		If Not CbTech4.Enabled Then CbTech4.SelectedValue = AxleNotSteered
	End Sub
End Class
