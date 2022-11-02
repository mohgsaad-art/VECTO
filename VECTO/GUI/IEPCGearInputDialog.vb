Public Class IEPCGearInputDialog
	Public Sub Clear()
		_tbRatio.Text = ""
		_tbMaxOutShaftSpeed.Text = ""
		_tbMaxOutShaftTorque.Text = ""
		_tbRatio.Focus()
	End Sub

	Private Sub btnOk_Click(sender As Object, e As EventArgs) Handles btnOk.Click
		If Not IsNumeric(_tbRatio.Text) Then
			MsgBox("Invalid input for Ratio")
			_tbRatio.Focus()
			Return
		End If

		If tbMaxOutShaftTorque.Text.Length > 0 And Not IsNumeric(_tbMaxOutShaftTorque.Text) Then
			MsgBox("Invalid input for Max Out Shaft Torque")
			_tbMaxOutShaftTorque.Focus()
			Return
		End If

		If _tbMaxOutShaftSpeed.Text.Length > 0 And Not IsNumeric(_tbMaxOutShaftSpeed.Text) Then
			MsgBox("Invalid input for Max Out Shaft Speed")
			_tbMaxOutShaftSpeed.Focus()
			Return
		End If

		DialogResult = DialogResult.OK
		Close()
	End Sub

	Private Sub btnCancel_Click(sender As Object, e As EventArgs) Handles btnCancel.Click
		DialogResult = DialogResult.Cancel
		Close()
		Clear()
	End Sub

	Private Sub IEPCGearInputDialog_Load(sender As Object, e As EventArgs) Handles MyBase.Load
		Show()
		_tbRatio.Focus()
	End Sub
End Class