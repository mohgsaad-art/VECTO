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
Imports System.ComponentModel
Imports System.IO
Imports System.Reflection
Imports System.Text

Public Module VECTO_Global
    Public VECTOvers As String = FileVersionInfo.GetVersionInfo([Assembly].GetExecutingAssembly().Location).FileMajorPart.ToString()
    Public COREvers As String = "NOT FOUND"

    Public Const LicSigAppCode As String = "VECTO-Release-0093C61E0A2E4BFA9A7ED7E729C56AE4"
    Public InstallModeInstalled As Boolean = False
    Public MyAppPath As String
    Public MyConfPath As String
    Public MyLogPath As String

    Public Const CONFIG_JOBLIST_FILE As String = "joblist.txt"
    Public Const CONFIG_CYCLELIST_FILE As String = "cyclelist.txt"
    Public Const FILE_HISTORY_DIR_FILE As String = "Directories.txt"


    Public LogFile As FileLogger

    'to ensure correct format for backgroundworker thread
    Public VectoWorkerV3 As BackgroundWorker

    Public Cfg As Configuration

    Public ReadOnly FileFormat As Encoding = Encoding.UTF8


    Public ProgBarCtrl As ProgressbarControl

    Public Class FileLogger
        Private _logStream As StreamWriter

        Const LOG_FILENAME As String = "LOG.txt"
        Const BACKUP_LOG_FILENAME As String = "LOG_backup.txt"

        Private logPath As String
        Private logFile As String

        Public Sub New()
            logPath = MyAppPath
            If (InstallModeInstalled) Then
                logPath = MyLogPath
            End If
            logFile = Path.Combine(logPath, LOG_FILENAME)
        End Sub


        Public Function StartLog() As Boolean
            Try
                _logStream = My.Computer.FileSystem.OpenTextFileWriter(logFile, True, FileFormat)
                _logStream.AutoFlush = True
                WriteToLog(MessageType.Normal, "Starting Session " & Now)
                WriteToLog(MessageType.Normal, "VECTO " & VECTOvers)
            Catch ex As Exception
                Return False
            End Try

            Return True
        End Function

        Public Function SizeCheck() As Boolean

            'Start new log if file size limit reached
            If File.Exists(logFile) Then

                'File size check
                Dim logfDetail As FileInfo = My.Computer.FileSystem.GetFileInfo(logFile)

                'If Log too large: Delete
                If logfDetail.Length / (2 ^ 20) > Cfg.LogSize Then

                    WriteToLog(MessageType.Normal, "Starting new logfile")
                    _logStream.Close()

                    Dim backUpError As Boolean = False

                    Try
                        If File.Exists(Path.Combine(logPath, BACKUP_LOG_FILENAME)) Then File.Delete(Path.Combine(logPath, BACKUP_LOG_FILENAME))
                        File.Move(logFile, Path.Combine(logPath, BACKUP_LOG_FILENAME))
                    Catch ex As Exception
                        backUpError = True
                    End Try

                    If Not StartLog() Then Return False

                    If backUpError Then
                        WriteToLog(MessageType.Err, "Failed to backup logfile! (" & Path.Combine(logPath, BACKUP_LOG_FILENAME) & ")")
                    Else
                        WriteToLog(MessageType.Normal, "Logfile restarted. Old log saved to " & BACKUP_LOG_FILENAME)
                    End If

                End If

            End If

            Return True
        End Function

        Public Function CloseLog() As Boolean
            Try
                WriteToLog(MessageType.Normal, "Closing Session " & Now)
                _logStream.Close()
            Catch ex As Exception
                Return False
            End Try

            Return True
        End Function


        Public Function WriteToLog(msgType As MessageType, msg As String) As Boolean
            Dim msgTypeStr As String

            Select Case msgType
                Case MessageType.Err
                    msgTypeStr = "Error"
                Case MessageType.Warn
                    msgTypeStr = "Warning"
                Case Else
                    msgTypeStr = "-"
            End Select

            Try
                _logStream.WriteLine(Now.ToString("yyyy/MM/dd-HH:mm:ss") & vbTab & msgTypeStr & vbTab & msg)
                Return True
            Catch ex As Exception
                Return False
            End Try
        End Function
    End Class

#Region "File path functions"

    'When no path is specified, then insert either HomeDir or MainDir   Special-folders
    Public Function FileRepl(file As String, Optional ByVal mainDir As String = "") As String

        Dim replPath As String

        'Trim Path
        file = Trim(file)

        'If empty file => Abort
        If file = "" Then Return ""

        'Replace sKeys
        file = Replace(file, DefVehPath & "\", MyAppPath & "Default Vehicles\", 1, -1,
                        CompareMethod.Text)
        file = Replace(file, HomePath & "\", MyAppPath, 1, -1, CompareMethod.Text)

        'Replace - Determine folder
        If mainDir = "" Then
            replPath = MyAppPath
        Else
            replPath = mainDir
        End If

        ' "..\" => One folder-level up
        Do While replPath.Length > 0 AndAlso Left(file, 3) = "..\"
            replPath = PathUp(replPath)
            file = file.Substring(3)
        Loop


        'Supplement Path, if not available
        If GetPath(file) = "" Then

            Return replPath & file

        Else
            Return file
        End If
    End Function

    'Path one-level-up      "C:\temp\ordner1\"  >>  "C:\temp\"
    Private Function PathUp(pfad As String) As String
		Dim x As Integer

        pfad = pfad.Substring(0, pfad.Length - 1)

        x = pfad.LastIndexOf("\", StringComparison.Ordinal)

        If x = -1 Then Return ""

        Return pfad.Substring(0, x + 1)
    End Function

    'File name without the path    "C:\temp\TEST.txt"  >>  "TEST.txt" oder "TEST"
    Public Function GetFilenameWithoutPath(file As String, includeFileExtension As Boolean) As String _
'GetFilenameWithoutPath
		Dim x As Integer
		x = file.LastIndexOf("\", StringComparison.Ordinal) + 1
		file = Right(file, Len(file) - x)
		If Not includeFileExtension Then
			x = file.LastIndexOf(".", StringComparison.Ordinal)
			If x > 0 Then file = Left(file, x)
		End If
		Return file
	End Function

	'Filename without extension   "C:\temp\TEST.txt" >> "C:\temp\TEST"

	'Filename without path if Path = WorkDir or MainDir
	Public Function GetFilenameWithoutDirectory(file As String, Optional ByVal mainDir As String = "") As String _
'GetFilenameWithoutDirectory
		Dim path As String

		If mainDir = "" Then
			path = MyAppPath
		Else
			path = mainDir
		End If

		If UCase(GetPath(file)) = UCase(path) Then file = GetFilenameWithoutPath(file, True)

		Return file
	End Function

	'Path alone        "C:\temp\TEST.txt"  >>  "C:\temp\"
	'                   "TEST.txt"          >>  ""
	Public Function GetPath(file As String) As String 'GetPath
		Dim x As Integer
		If file Is Nothing OrElse file.Length < 3 OrElse file.Substring(1, 2) <> ":\" Then Return ""
		x = file.LastIndexOf("\", StringComparison.Ordinal)
		Return Left(file, x + 1)
	End Function

	'Extension alone      "C:\temp\TEST.txt" >> ".txt"
	Public Function GetExtension(file As String) As String 'GetExtension
		Dim x As Integer
		x = file.LastIndexOf(".", StringComparison.Ordinal)
		If x = -1 Then
			Return ""
		Else
			Return Right(file, Len(file) - x)
		End If
	End Function


#End Region
End Module


Module Constants
	'Public ReadOnly AUX As AuxiliaryKey

	Public Const HomePath As String = "<HOME>"
	Public Const DefVehPath As String = "<VEHDIR>"
	Public Const NoFile As String = "<NOFILE>"

	'Public Sub New()

	'	AUX = New AuxiliaryKey
	'End Sub


	' ReSharper disable once ClassNeverInstantiated.Global
End Module


