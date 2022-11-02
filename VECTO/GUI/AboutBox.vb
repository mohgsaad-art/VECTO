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
Imports TUGraz.VectoCore.Utils

''' <summary>
''' About Dialog. Shows Licence and contact/support information
''' </summary>
Public Class AboutBox
	Private Sub F10_AboutBox_Load(sender As Object, e As EventArgs) Handles MyBase.Load
		Text = "VECTO " & VECTOvers & " / VectoCore" & VectoSimulationCore.BranchSuffix & " " & COREvers
	End Sub

	Private Sub LinkLabel1_LinkClicked_1(sender As Object, e As LinkLabelLinkClickedEventArgs) _
		Handles LinkLabel1.LinkClicked
		Process.Start(New ProcessStartInfo("mailto:jrc-vecto@ec.europa.eu") With {.UseShellExecute = True})
	End Sub

	Private Sub PictureBoxJRC_Click(sender As Object, e As EventArgs) Handles PictureBoxJRC.Click
		Process.Start(New ProcessStartInfo("http://ec.europa.eu/dgs/jrc/index.cfm") With {.UseShellExecute = True})
	End Sub
	
	Private Sub LinkLabel2_LinkClicked(sender As Object, e As LinkLabelLinkClickedEventArgs) _
		Handles LinkLabel2.LinkClicked
		Process.Start(New ProcessStartInfo("https://joinup.ec.europa.eu/community/eupl/og_page/eupl") With {.UseShellExecute = True})
	End Sub
End Class
