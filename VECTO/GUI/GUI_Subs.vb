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


''' <summary>
''' Methods for GUI interaction
''' </summary>
''' <remarks></remarks>
Public Module GUI_Subs

#Region "GUI control via background worker"

	'Status Message => Msg-Listview
	Public Sub WorkerMsg(ByVal id As MessageType, ByVal msg As String, ByVal source As String,
						Optional ByVal link As String = "")
		Dim workProg As New BackgroundWorkerMessage(WorkerMessageType.StatusListBox)
		workProg.ID = id
		Select Case id
			Case MessageType.Err
			Case MessageType.Warn
		End Select
		workProg.Msg = msg
		workProg.Source = source
		Try
			'VECTOworker.ReportProgress(0, WorkProg)
		Catch ex As Exception
			GUIMsg(id, msg)
		End Try
	End Sub

#End Region

#Region "Direct GUI control - Cannot be called by background worker!"

	'Status message
	' ReSharper disable once InconsistentNaming
	Public Sub GUIMsg(ByVal id As MessageType, ByVal msg As String)
		MainForm.MsgToForm(id, msg, "", Nothing)
	End Sub

	'Statusbar
	Public Sub Status(ByVal txt As String)
		MainForm.ToolStripLbStatus.Text = txt
	End Sub

	'Status form reset
	Public Sub ClearMsg()
		MainForm.LvMsg.Items.Clear()
	End Sub

#End Region

	'Class used to pass Messages from BackgroundWorker to GUI
	Private Class BackgroundWorkerMessage
		Public Sub New(msgTarget As WorkerMessageType)
			Source = ""
		End Sub


		Public Property Source As String

		Public Property ID As MessageType

		Public Property Msg As String
	End Class

	'Progress bar control
	Public Class ProgressbarControl
		Public ProgOverallStartInt As Integer = -1
		Public PgroOverallEndInt As Integer = -1
		Public ProgJobInt As Integer = -1
		Public ProgLock As Boolean = False
	End Class

#Region "Textbox text conversion for file open/save operations"

	'Text-to-number
	'Public Function ParseNumber(txt As String) As Double
	'	If Not IsNumeric(txt) Then
	'		Return 0
	'	End If
	'	Return Double.Parse(txt, CultureInfo.InvariantCulture)
	'End Function


#End Region

	'Open File with software defined in Config
	Public Function FileOpenAlt(ByVal file As String) As Boolean
		Dim psi As New ProcessStartInfo

		If Not IO.File.Exists(file) Then Return False

		psi.FileName = Cfg.OpenCmd
		psi.Arguments = ChrW(34) & file & ChrW(34)
		Try
			Process.Start(psi)
			Return True
		Catch ex As Exception
			Return False
		End Try
	End Function

	Public Function WrongMode() As Integer

		If Cfg.DeclMode Then

			Select Case _
				MsgBox(
					"This file was created in Engineering Mode! Opening in Declaration Mode will overwrite some parameters with generic values." &
					vbCrLf & vbCrLf & "Do you want to switch to Engineering Mode?" & vbCrLf & vbCrLf &
					"[Yes] Switch mode and open file" & vbCrLf & "[No] Open file without changing mode" & vbCrLf &
					"[Cancel] Abort opening file", MsgBoxStyle.YesNoCancel, "Warning")
				Case MsgBoxResult.Yes
					Return 1

				Case (MsgBoxResult.No)
					Return 0

				Case Else
					Return -1

			End Select

		Else

			Select Case _
				MsgBox(
					"This file was created in Declaration Mode! For use in Engineering Mode missing parameters must be defined." &
					vbCrLf & vbCrLf & "Do you want to switch to Declaration Mode?" & vbCrLf & vbCrLf &
					"[Yes] Switch mode and open file" & vbCrLf & "[No] Open file without changing mode" & vbCrLf &
					"[Cancel] Abort opening file", MsgBoxStyle.YesNoCancel, "Warning")
				Case MsgBoxResult.Yes
					Return 1

				Case (MsgBoxResult.No)
					Return 0

				Case Else
					Return -1

			End Select

		End If
	End Function
End Module
