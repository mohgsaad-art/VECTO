

Public Class FilePathUtils


Public shared Function ValidateFilePath( byval filePath As String , byval expectedExtension As String, ByRef message As string ) As Boolean


  Dim  illegalFileNameCharacters As Char()  = {"<",">",":","""","/","\","|","?","*","~"}
  Dim  detectedExtention As String          =  fileExtentionOnly( filePath)
  Dim  pathOnly as string                   =  filePathOnly( filePath)
  Dim  fileNameOnlyWithExtension As String  =  fileNameOnly(filePath,True)
  Dim  fileNameOnlyNoExtension As String    =  fileNameOnly(filePath,false)

  'Is this filePath empty
  If filePath.trim.Length=0 then
      message = "A filename cannot be whitespace"
     Return false
  End If
  
  
  'Extension Expected, but not match
  If expectedExtension.Trim.Length>0 then
     If  String.Compare(expectedExtension,detectedExtention,true)<>0
       message = String.Format("The file extension type does not match the expected type of {0}", expectedExtension)
       Return false
     End If
  End If
  
  'Extension Not Expected, but was supplied
  If expectedExtension.Trim.Length>0  then
    If detectedExtention.Length= 0 then
    message=String.Format("No Extension was supplied, but an extension of {0}, this is not required",detectedExtention)
    Return false
    End If 
  End If
  
  
  'Illegal characters
  If Not fileNameLegal(fileNameOnlyWithExtension )
       message= String.Format("The filenames have one or more illegal characters")
       Return false
  End If
  
  

  message="OK"
  Return true

End Function


Public Shared function fileNameLegal( fileName as string) As Boolean

  Dim  illegalFileNameCharacters As Char()  = {"<",">",":","""","/","\","|","?","*","~"}


    'Illegal characters
  For Each ch As Char In illegalFileNameCharacters
  
    If fileName.Contains( ch ) then

       Return false

    End If
  Next
  Return true

End Function




    ''' <summary>
    ''' File name without the path    "C:\temp\TEST.txt"  >>  "TEST.txt" oder "TEST"
    ''' </summary>
    ''' <param name="filePath"></param>
    ''' <param name="WithExtention"></param>
    ''' <returns>Return file portion of the path, with or without the extension</returns>
    ''' <remarks></remarks>
    Public shared Function fileNameOnly(ByVal filePath As String, ByVal WithExtention As Boolean) As String
        Dim x As Int16
        x = filePath.LastIndexOf("\") + 1
        filePath = Microsoft.VisualBasic.Right(filePath, Microsoft.VisualBasic.Len(filePath) - x)
        If Not WithExtention Then
            x = filePath.LastIndexOf(".")
            If x > 0 Then filePath = Microsoft.VisualBasic.Left(filePath, x)
        End If
        Return filePath
    End Function


    ''' <summary>
    ''' Extension alone      "C:\temp\TEST.txt" >> ".txt"
    ''' </summary>
    ''' <param name="filePath"></param>
    ''' <returns>Extension alone Including the dot IE  .EXT</returns>
    ''' <remarks></remarks>
    Public shared Function fileExtentionOnly(ByVal filePath As String) As String
        Dim x As Int16
        x = filePath.LastIndexOf(".")
        If x = -1 Then
            Return ""
        Else
            Return Microsoft.VisualBasic.Right(filePath, Microsoft.VisualBasic.Len(filePath) - x)
        End If
    End Function

    ''' <summary>
    ''' File Path alone   "C:\temp\TEST.txt"  >>  "C:\temp\"
    '''                   "TEST.txt"          >>  ""
    ''' </summary>
    ''' <param name="filePath"></param>
    ''' <returns>Filepath without the extension</returns>
    ''' <remarks></remarks>
    Public  Shared Function filePathOnly(ByVal filePath As String) As String
        Dim x As Int16
        If filePath Is Nothing OrElse filePath.Length < 3 OrElse filePath.Substring(1, 2) <> ":\" Then Return ""
        x = filePath.LastIndexOf("\")
        Return Microsoft.VisualBasic.Left(filePath, x + 1)
    End Function


End Class

