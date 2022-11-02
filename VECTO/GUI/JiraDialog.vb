Imports System.IO

Public Class JiraDialog
	Private Sub OK_Button_Click(sender As Object, e As EventArgs) Handles OK_Button.Click
		DialogResult = DialogResult.OK
		Close()
	End Sub

	Private Sub LinkLabel1_LinkClicked(sender As Object, e As LinkLabelLinkClickedEventArgs) Handles LinkLabel1.LinkClicked
		Dim fileName = Path.Combine(MyAppPath, "User Manual\JIRA Quick Start Guide.pdf")
		If File.Exists(fileName) Then
			Process.Start(New ProcessStartInfo(fileName) With {.UseShellExecute = True})
		Else
			MsgBox("File not found!", MsgBoxStyle.Critical)
		End If
	End Sub

	Private Sub LinkLabel3_LinkClicked(sender As Object, e As LinkLabelLinkClickedEventArgs) Handles LinkLabel3.LinkClicked
		Dim bodytext = "Please provide the following information:" & "%0A" &
					"- Email" & "%0A" &
					"- Name, Surname" & "%0A" &
					"- Country of workplace" & "%0A" &
					"- Position"

		Process.Start(New ProcessStartInfo("mailto:JRC-VECTO@ec.europa.eu?subject=CITnet%20account&body=" & bodytext) With {.UseShellExecute = True})
	End Sub

	Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
		Process.Start(New ProcessStartInfo("https://citnet.tech.ec.europa.eu/CITnet/jira/browse/VECTO") With {.UseShellExecute = True})
	End Sub
End Class
