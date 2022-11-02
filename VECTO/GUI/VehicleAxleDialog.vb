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

Imports System.Collections.Generic
Imports System.ComponentModel.DataAnnotations
Imports System.Linq
Imports TUGraz.VectoCommon.InputData
Imports TUGraz.VectoCommon.Models
Imports TUGraz.VectoCommon.Utils
Imports TUGraz.VectoCore.Models.Declaration


''' <summary>
''' Axle Config Editor (Vehicle Editor sub-dialog)
''' </summary>
Public Class VehicleAxleDialog
	Public Sub New()
		InitializeComponent()

		CbWheels.Items.Add("-")
		CbWheels.Items.AddRange(DeclarationData.Wheels.GetWheelsDimensions().OrderBy(function(s) s).ToArray())

		cbAxleType.Items.Clear()
		cbAxleType.ValueMember = "Value"
		cbAxleType.DisplayMember = "Label"

		cbAxleType.DataSource = [Enum].GetValues(GetType(AxleType)) _
			.Cast(Of AxleType)() _
			.Where(Function(type) Not Cfg.DeclMode OrElse type <> AxleType.Trailer) _
			.Select(Function(type) New With {Key .Value = type, .Label = type.GetLabel()}).ToList()

	End Sub

	Public Sub Clear()
		CbTwinT.Checked = False
		TbAxleShare.Text = ""
		TbI_wheels.Text = ""
		TbRRC.Text = ""
		TbFzISO.Text = ""
		CbWheels.SelectedIndex = 0
	End Sub

	'Initialise
	Private Sub F_VEH_Axle_Load(sender As Object, e As EventArgs) Handles Me.Load
		PnAxle.Enabled = Not Cfg.DeclMode
	End Sub

	'Save and close
	Private Sub OK_Button_Click(sender As Object, e As EventArgs) Handles OK_Button.Click

		Dim axleData As Axle = New Axle With {
				.AxleWeightShare = TbAxleShare.Text.ToDouble(0),
				.RollResistanceCoefficient = TbRRC.Text.ToDouble(0),
				.TyreTestLoad = TbFzISO.Text.ToDouble(0).SI(Of Newton)(),
				.TwinTyres = CbTwinT.Checked,
				.WheelsDimension = If(IsNothing(CbWheels.SelectedItem), "", CbWheels.SelectedItem.ToString()),
				.Inertia = TbI_wheels.Text.ToDouble(0).SI(Of KilogramSquareMeter)(),
                .AxleType = CType(if(IsNothing(cbAxleType.SelectedValue), AxleType.VehicleNonDriven, cbAxleType.SelectedValue), AxleType) 
				}

		Dim results As IList(Of ValidationResult) =
				axleData.Validate(If(Cfg.DeclMode, ExecutionMode.Declaration, ExecutionMode.Engineering), VectoSimulationJobType.ConventionalVehicle, Nothing, Nothing, False)

		If results.Any() Then
			Dim messages As IEnumerable(Of String) =
					results.Select(Function(r) r.ErrorMessage + String.Join(", ", r.MemberNames.Distinct()))
			MsgBox("Invalid input:" + Environment.NewLine + String.Join(Environment.NewLine, messages), MsgBoxStyle.OkOnly,
					"Failed to save axle gear")
			Exit Sub
		End If

		DialogResult = DialogResult.OK
		Close()
	End Sub

	Private Sub CbWheels_SelectedIndexChanged(sender As Object, e As EventArgs) Handles CbWheels.SelectedIndexChanged
		'Dim inertia As Double
		If Cfg.DeclMode Then
			'inertia = DeclarationData.Wheels.Lookup(CbWheels.Text).Inertia.Value()
			If CbWheels.Text = "-" Then
				TbI_wheels.Text = "-"
			Else
				TbI_wheels.Text = DeclarationData.Wheels.Lookup(CbWheels.Text).Inertia.Value().ToString()
			End If
		End If
	End Sub

	'Cancel
	Private Sub Cancel_Button_Click(sender As Object, e As EventArgs) Handles Cancel_Button.Click
		DialogResult = DialogResult.Cancel
		Close()
	End Sub
End Class
