
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

 Private map As new List(Of ICombinedAlternatorMapRow)


  Private Alternators As New List(Of IAlternator)
  Private OriginalAlternators As New List(Of IAlternator)

  Private FilePath As String
  Private altSignals As ICombinedAlternatorSignals



 public sub new( filePath as String, altSignals As ICombinedAlternatorSignals)

      Dim feedback As String = String.Empty

      If Not FilePathUtils.ValidateFilePath(filePath,".aalt", feedback) then
         Throw New ArgumentException(String.Format("Combined Alternator requires a valid .AALT filename. : {0}", feedback))
        Else
          me.filePath = filePath
      End If


      Me.altSignals= altSignals


      'IF file exists then read it otherwise create a default.

      If File.Exists( filePath )  andAlso InitialiseMap( filePath) then

        Initialise()

        

      else



      End If

 End Sub


 private Function Initialise() As Boolean

   'From the map we construct this CombinedAlternator object and original CombinedAlternator Object

   Alternators.Clear
   OriginalAlternators.Clear

   'Set Number of alternators in AltSignals.
   altSignals.NumberOfAlternators= map.Count/9



   For Each alt As IEnumerable(Of ICombinedAlternatorMapRow)  In map.GroupBy( Function(g) g.AlternatorName)

     Dim altName As String = alt.First().AlternatorName
     Dim pulleyRatio As Single = alt.First().PulleyRatio


     Dim alternator  As IAlternator = New Alternator(altSignals, alt.ToList())

     Alternators.Add( alternator )


   Next


 End Function

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
 private Function Load() As Boolean 

      If Not InitialiseMap(filePath) then Return False
      

      Return true

 End Function

    'Initialises the map.
    Public Function InitialiseMap(filePath As string) As Boolean 

       Dim returnValue As Boolean = false

       If File.Exists(filePath) Then
                Using sr As StreamReader = New StreamReader(filePath)
                    'get array og lines fron csv
                    Dim lines() As String = sr.ReadToEnd().Split(CType(Environment.NewLine, Char()), StringSplitOptions.RemoveEmptyEntries)

                    'Must have at least 2 entries in map to make it usable [dont forget the header row]
                    If (lines.Count() < 10) Then
                        Throw New ArgumentException("Insufficient rows in csv to build a usable map")
                    End If

                    map = new  List(Of ICombinedAlternatorMapRow)

                    Dim firstline As Boolean = True

                    For Each line As String In lines
                        If Not firstline Then

                        'Advanced Alternator Source Check.
                        If line.contains("[MODELSOURCE") then Exit For

                            'split the line
                            Dim elements() As String = line.Split(New Char() {","}, StringSplitOptions.RemoveEmptyEntries)
                            '3 entries per line required
                            If (elements.Length <> 5) Then
                                Throw New ArgumentException("Incorrect number of values in csv file")
                            End If
                            'add values to map

                             map.Add( New CombinedAlternatorMapRow(elements(0),elements(1),elements(2),elements(3),elements(4)))

                        Else
                            firstline = False
                        End If
                    Next line
                End Using
                Return True
            Else
                Throw New ArgumentException("Supplied input file does not exist")
            End If

    Return returnValue


  End Function




End Class


End Namespace



