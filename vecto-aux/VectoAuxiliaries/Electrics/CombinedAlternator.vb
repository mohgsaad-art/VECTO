
Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Text
Imports System.IO
Imports DocumentFormat.OpenXml
Imports DocumentFormat.OpenXml.Spreadsheet
Imports SpreadsheetLight
Imports Newtonsoft.Json
Imports VectoAuxiliaries

Namespace Electrics

Public Class CombinedAlternator


  Private Alternators As New List(Of IAlternator)
  Private OriginalAlternators As New List(Of IAlternator)

  Private FilePath As String


 public sub new( filePath as String)

      Dim feedback As String = String.Empty

      If Not FilePathUtils.ValidateFilePath(filePath,".aalt", feedback) then
         Throw New ArgumentException(String.Format("Combined Alternator requires a valid .AALT filename. : {0}", feedback))
        Else
          me.filePath = filePath
      End If
      

 End Sub


 Public Sub Clone( other As CombinedAlternator) 


    For Each Alternator As IAlternator In Alternators

     Alternator.Clone( other )



    Next




 End Sub



   'Persistance Functions
 Public Function Save(filePath As String) As Boolean


   Dim returnValue As Boolean = True
   Dim settings As JsonSerializerSettings = New JsonSerializerSettings()
   settings.TypeNameHandling = TypeNameHandling.Objects

    'JSON METHOD
    Try

       Dim output As String = JsonConvert.SerializeObject(Me, Formatting.Indented, settings)

       File.WriteAllText(filePath, output)

       Catch ex As Exception

         'TODO:Do something meaningfull here perhaps logging
          returnValue = False

     End Try

   Return returnValue

End Function
 Public Function Load(filePath As String) As Boolean 

    Dim returnValue As Boolean = True
    Dim settings As JsonSerializerSettings = New JsonSerializerSettings()
    Dim tmpAux As CombinedAlternator = New CombinedAlternator(filePath)

    settings.TypeNameHandling = TypeNameHandling.Objects

     'JSON METHOD
     Try

       Dim output As String = File.ReadAllText(filePath)


       tmpAux = JsonConvert.DeserializeObject(Of CombinedAlternator)(output, settings)




       'This is where we Assume values of loaded( Deserialized ) object.
       Clone(tmpAux)

      Catch ex As Exception

        'TODO:Do something meaningfull here perhaps logging

         returnValue = False
      End Try

    Return returnValue

End Function




End Class


End Namespace



