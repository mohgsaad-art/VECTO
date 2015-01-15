' Copyright 2015 European Union.
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
''' Global File Brower properties and cFilebrowser instances.
''' </summary>
''' <remarks></remarks>
Module FB_Global

    Public FB_FolderHistory(19) As String
    Public FB_Drives() As String
    Public FB_Init As Boolean
    Public FB_FilHisDir As String
    '-----------------------------
    Public fbFolder As cFileBrowser
    Public fbVECTO As cFileBrowser
    Public fbFileLists As cFileBrowser
    Public fbVEH As cFileBrowser
    Public fbDRI As cFileBrowser
    Public fbMAP As cFileBrowser
    Public fbFLD As cFileBrowser

    Public fbENG As cFileBrowser
    Public fbGBX As cFileBrowser
    Public fbACC As cFileBrowser
    Public fbAUX As cFileBrowser

    Public fbGBS As cFileBrowser
    Public fbTLM As cFileBrowser
    Public fbRLM As cFileBrowser
    Public fbTCC As cFileBrowser
    Public fbCDx As cFileBrowser

    Public fbVMOD As cFileBrowser

                'Paths
    private        MyAppPath  = My.Application.Info.DirectoryPath & "\"
    private        MyConfPath  = MyAppPath & "Config\"
    private        MyDeclPath = MyAppPath & "Declaration\"
    'private        FB_FilHisDir = MyConfPath & "FileHistory\"
    private        HomePath As String = "<HOME>"
    private        JobPath As String = "<JOBPATH>"
    private        DefVehPath As String = "<VEHDIR>"
    private        NoFile As String = "<NOFILE>"
    private        EmptyString As String = "<EMPTYSTRING>"
    private        Break As String = "<//>"
    
    private        Normed As String = "NORM"
   
    private        PauxSply As String = "<AUX_"
 #Region "File path functions"

    'When no path is specified, then insert either HomeDir or MainDir   Special-folders
    Public Function fFileRepl(ByVal file As String, Optional ByVal MainDir As String = "") As String

        Dim ReplPath As String

        'Trim Path
        file = Trim(file)

        'If empty file => Abort
        If file = "" Then Return ""

        'Replace sKeys
        file = Microsoft.VisualBasic.Strings.Replace(file, DefVehPath & "\", MyAppPath & "Default Vehicles\", 1, -1, CompareMethod.Text)
        file = Microsoft.VisualBasic.Strings.Replace(file, DefVehPath & "\", MyAppPath, 1, -1, CompareMethod.Text)

        'Replace - Determine folder
        If MainDir = "" Then
            ReplPath = MyAppPath
        Else
            ReplPath = MainDir
        End If

        ' "..\" => One folder-level up
        Do While ReplPath.Length > 0 AndAlso Left(file, 3) = "..\"
            ReplPath = fPathUp(ReplPath)
            file = file.Substring(3)
        Loop


        'Supplement Path, if not available
        If fPATH(file) = "" Then

            Return ReplPath & file

        Else
            Return file
        End If

    End Function

    'Path one-level-up      "C:\temp\ordner1\"  >>  "C:\temp\"
    Private Function fPathUp(ByVal Pfad As String) As String
        Dim x As Int16

        Pfad = Pfad.Substring(0, Pfad.Length - 1)

        x = Pfad.LastIndexOf("\")

        If x = -1 Then Return ""

        Return Pfad.Substring(0, x + 1)

    End Function

    'File name without the path    "C:\temp\TEST.txt"  >>  "TEST.txt" oder "TEST"
    Public Function fFILE(ByVal Pfad As String, ByVal MitEndung As Boolean) As String
        Dim x As Int16
        x = Pfad.LastIndexOf("\") + 1
        Pfad = Microsoft.VisualBasic.Right(Pfad, Microsoft.VisualBasic.Len(Pfad) - x)
        If Not MitEndung Then
            x = Pfad.LastIndexOf(".")
            If x > 0 Then Pfad = Microsoft.VisualBasic.Left(Pfad, x)
        End If
        Return Pfad
    End Function

    'Filename without extension   "C:\temp\TEST.txt" >> "C:\temp\TEST"
    Public Function fFileWoExt(ByVal Path As String) As String
        Return fPATH(Path) & fFILE(Path, False)
    End Function

    'Filename without path if Path = WorkDir or MainDir
    Public Function fFileWoDir(ByVal file As String, Optional ByVal MainDir As String = "") As String
        Dim path As String

        If MainDir = "" Then
            path = MyAppPath
        Else
            path = MainDir
        End If

        If UCase(fPATH(file)) = UCase(path) Then file = fFILE(file, True)

        Return file

    End Function

    'Path alone        "C:\temp\TEST.txt"  >>  "C:\temp\"
    '                   "TEST.txt"          >>  ""
    Public Function fPATH(ByVal Pfad As String) As String
        Dim x As Int16
        If Pfad Is Nothing OrElse Pfad.Length < 3 OrElse Pfad.Substring(1, 2) <> ":\" Then Return ""
        x = Pfad.LastIndexOf("\")
        Return Microsoft.VisualBasic.Left(Pfad, x + 1)
    End Function

    'Extension alone      "C:\temp\TEST.txt" >> ".txt"
    Public Function fEXT(ByVal Pfad As String) As String
        Dim x As Int16
        x = Pfad.LastIndexOf(".")
        If x = -1 Then
            Return ""
        Else
            Return Microsoft.VisualBasic.Right(Pfad, Microsoft.VisualBasic.Len(Pfad) - x)
        End If
    End Function


#End Region


End Module

