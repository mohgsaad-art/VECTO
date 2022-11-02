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
Imports System.Collections.Generic
Imports System.IO
Imports System.Linq
Imports System.Text
Imports NLog
Imports NLog.Targets
Imports NLog.Targets.Wrappers
Imports TUGraz.VectoCommon.Utils
Imports TUGraz.VectoCore.Utils


Namespace My
	' The following events are available for MyApplication:
	' 
	' Startup: Raised when the application starts even before the creation of the Startup-forms.
	' Shutdown: Raised after closing all the application forms. This event is not raised if the application terminates abnormally.
	' UnhandledException: Raised if the application encounters an unhandled exception.
	' StartupNextInstance: Raised when launching a single-instance application, and one is already active.
	' NetworkAvailabilityChanged: Occurs when connecting or disconnecting to the network.
	' ReSharper disable once ClassNeverInstantiated.Global
	Partial Friend Class MyApplication

		Const INSTALL_SETTINGS_FILE As String = "install.ini"
		Const CONFIG_FOLDER As string = "Config"
		Const CONFIG_FILE_HISTORY_FOLDER As string = "FileHistory"
		

		Const APP_DATA_VENDOR_PATH As String = "VECTO"

		'Initialization
		Private Sub MyApplication_Startup(sender As Object, e As Microsoft.VisualBasic.ApplicationServices.StartupEventArgs) _
			Handles Me.Startup

			Dim s As String
			Dim i As Integer

			'Paths
			MyAppPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..")

			ReadInstallMode()

			MyConfPath = path.Combine(MyAppPath, CONFIG_FOLDER)
			MyLogPath = ""
			if (InstallModeInstalled) Then
				MyConfPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), APP_DATA_VENDOR_PATH, VectoSimulationCore.VersionNumber, CONFIG_FOLDER)
				MyLogPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), APP_DATA_VENDOR_PATH, VectoSimulationCore.VersionNumber)

				Dim logTarget As FileTarget = TryCast(LogManager.Configuration.FindTargetByName("LogFile"), FileTarget)
				if (logTarget is Nothing) Then
					logTarget = CType(TryCast(LogManager.Configuration.FindTargetByName("LogFile"), WrapperTargetBase).WrappedTarget, FileTarget)
				End If

				if (Not (logTarget Is Nothing)) then
					logTarget.FileName = Path.Combine(MyLogPath, "logs", "log.txt")
					LogManager.ReconfigExistingLoggers()
				End If
			End If

			FileHistoryPath = path.Combine(MyConfPath, CONFIG_FILE_HISTORY_FOLDER)

			

			'If folder does not exist: Create!
			If Not String.IsNullOrWhiteSpace(MyLogPath) AndAlso Not Directory.Exists(MyLogPath) Then
				Try
					Directory.CreateDirectory(MyLogPath)
				Catch ex As Exception
					MsgBox("Failed to create directory '" & MyLogPath & "'!", MsgBoxStyle.Critical)
					'LogFile.WriteToLog(MessageType.Err, "Failed to create directory '" & MyLogPath & "'!")
					e.Cancel = True
				End Try
			End If

			'Log
			LogFile = New FileLogger
			If Not LogFile.StartLog() Then
				MsgBox("Error! Can't access log file. Application folder needs read/write permissions!")
				e.Cancel = True
			End If

			If Not Directory.Exists(MyConfPath) Then
				Try
					Directory.CreateDirectory(MyConfPath)
				Catch ex As Exception
					MsgBox("Failed to create directory '" & MyConfPath & "'!", MsgBoxStyle.Critical)
					LogFile.WriteToLog(MessageType.Err, "Failed to create directory '" & MyConfPath & "'!")
					e.Cancel = True
				End Try
				File.Create(path.Combine(MyConfPath, CONFIG_JOBLIST_FILE)).Close()
				File.Create(path.Combine(MyConfPath, CONFIG_CYCLELIST_FILE)).Close()
			End If
			
			If Not Directory.Exists(FileHistoryPath) Then
				Try
					Directory.CreateDirectory(FileHistoryPath)

					'Preconfigure Directories.txt
					Try
						s = Directory.GetParent(Application.Info.DirectoryPath).ToString & "\"
					Catch ex As Exception
						s = MyAppPath
					End Try
					Try

						Dim file As StreamWriter = Computer.FileSystem.OpenTextFileWriter(Path.Combine(FileHistoryPath, FILE_HISTORY_DIR_FILE), True,
																						Encoding.UTF8)
						file.WriteLine(s)
						For i = 2 To 20
							file.WriteLine(" ")
						Next
						file.Close()
					Catch ex As Exception

					End Try

				Catch ex As Exception
					MsgBox("Failed to create directory '" & FileHistoryPath & "'!", MsgBoxStyle.Critical)
					LogFile.WriteToLog(MessageType.Err, "Failed to create directory '" & FileHistoryPath & "'!")
					e.Cancel = True
				End Try
			End If

			'Separator!
			If Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator <> "." Then
				Try
					Threading.Thread.CurrentThread.CurrentCulture = New Globalization.CultureInfo("en-US")
					Threading.Thread.CurrentThread.CurrentUICulture = New Globalization.CultureInfo("en-US")
					'MSGtoForm(8, "Set CurrentCulture to 'en-US'", True)
				Catch ex As Exception
					GUIMsg(MessageType.Err,
							"Failed to set Application Regional Settings to 'en-US'! Check system decimal- and group- separators!")
				End Try
			End If

			'Initialise Classes
			JobFileList = New List(Of String)

			'DEV = New cDEV

			Cfg = New Configuration _
			'ACHTUNG: Configuration.New löst Configuration.SetDefault aus welches sKey benötigt dehalb muss sKey schon vorher initialisiert werden!!
			Cfg.FilePath = Path.Combine(MyConfPath, "settings.json")

			ProgBarCtrl = New ProgressbarControl

			'Config
			Cfg.Load()

			'Restart log if log file too large
			LogFile.SizeCheck()
		End Sub

		Private Sub ReadInstallMode()

			if (file.Exists(path.Combine(MyAppPath, INSTALL_SETTINGS_FILE))) Then
				Try 
					Dim lines As List(Of String) = file.readlines(path.Combine(MyAppPath, INSTALL_SETTINGS_FILE)).Where(function(s1) Not(s1.RemoveWhitespace().StartsWith("#"))).toList()
					Dim installSetting as String = lines.LastOrDefault(function(s1) s1.StartsWith("ExecutionMode", StringComparison.InvariantCultureIgnoreCase))
					if (Not(string.IsNullOrWhiteSpace(installSetting))) then
						Dim parts As String() = installSetting.split("="c)
						if (parts.Length > 1 AndAlso "install".Equals(parts(1).RemoveWhitespace(), StringComparison.InvariantCultureIgnoreCase)) then
							InstallModeInstalled = True
						End If
					End If
				Catch ex As Exception
					'do nothing...
				End Try
			End If
		End Sub

		Private Sub MyApplication_UnhandledException(ByVal sender As Object,
													ByVal e As Microsoft.VisualBasic.ApplicationServices.UnhandledExceptionEventArgs) _
			Handles Me.UnhandledException
			e.ExitApplication = True
			MsgBox("ERROR!" & ChrW(10) & ChrW(10) & e.Exception.Message.ToString, MsgBoxStyle.Critical, "Unexpected Error")
			LogFile.WriteToLog(MessageType.Err, ">>>Unexpected Error:" & e.Exception.ToString())
		End Sub
	End Class
End Namespace

