Imports VectoAuxiliaries.Electrics
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
   Implements IAlternatorMap


  Private map As new List(Of ICombinedAlternatorMapRow)


  Public Property Alternators As New List(Of IAlternator)
  Private OriginalAlternators As New List(Of IAlternator)

  Private FilePath As String
  Private altSignals As ICombinedAlternatorSignals


 'Interface Implementation
 Public Function GetEfficiency( ByVal CrankRPM As Single , ByVal  Amps  As Single ) As AlternatorMapValues Implements IAlternatorMap.GetEfficiency

    altSignals.CrankRPM = CrankRPM
    altSignals.CurrentDemandAmps = Amps / Alternators.Count

    Return  New AlternatorMapValues( Alternators.Average( Function(a) a.Efficiency)/100)


 End Function
 public Function Initialise() As Boolean Implements IAlternatorMap.Initialise

   'From the map we construct this CombinedAlternator object and original CombinedAlternator Object

   Alternators.Clear
   OriginalAlternators.Clear


   For Each alt As IEnumerable(Of ICombinedAlternatorMapRow)  In map.GroupBy( Function(g) g.AlternatorName)

     Dim altName As String = alt.First().AlternatorName
     Dim pulleyRatio As Single = alt.First().PulleyRatio


     Dim alternator  As IAlternator = New Alternator(altSignals, alt.ToList())

     Alternators.Add( alternator )


   Next

   Return true

 End Function

 'Constructors
 public sub new( filePath as String)

      Dim feedback As String = String.Empty

      If Not FilePathUtils.ValidateFilePath(filePath,".aalt", feedback) then
         Throw New ArgumentException(String.Format("Combined Alternator requires a valid .AALT filename. : {0}", feedback))
        Else
          me.filePath = filePath
      End If


      Me.altSignals= New CombinedAlternatorSignals()


      'IF file exists then read it otherwise create a default.

      If File.Exists( filePath )  andAlso InitialiseMap( filePath) then
        Initialise()
      else

       'Create Default Map
        CreateDefaultMap()
        Initialise

      End If

 End Sub

 'Helpers
 Private sub CreateDefaultMap()

    map.Clear

    map.Add( new CombinedAlternatorMapRow("Alt1",2000,10,50,3   ))
    map.Add( new CombinedAlternatorMapRow("Alt1",2000,40,50,3   ))
    map.Add( new CombinedAlternatorMapRow("Alt1",2000,60,50,3   ))
    map.Add( new CombinedAlternatorMapRow("Alt1",4000,10,70,3   ))
    map.Add( new CombinedAlternatorMapRow("Alt1",4000,40,70,3   ))
    map.Add( new CombinedAlternatorMapRow("Alt1",4000,60,70,3   ))
    map.Add( new CombinedAlternatorMapRow("Alt1",6000,10,60,3   ))
    map.Add( new CombinedAlternatorMapRow("Alt1",6000,40,60,3   ))
    map.Add( new CombinedAlternatorMapRow("Alt1",6000,60,60,3   ))
    map.Add( new CombinedAlternatorMapRow("Alt2",2000,10,80,2.5 ))
    map.Add( new CombinedAlternatorMapRow("Alt2",2000,40,80,2.5 ))
    map.Add( new CombinedAlternatorMapRow("Alt2",2000,60,80,2.5 ))
    map.Add( new CombinedAlternatorMapRow("Alt2",4000,10,40,2.5 ))
    map.Add( new CombinedAlternatorMapRow("Alt2",4000,40,40,2.5 ))
    map.Add( new CombinedAlternatorMapRow("Alt2",4000,60,40,2.5 ))
    map.Add( new CombinedAlternatorMapRow("Alt2",6000,10,60,2.5 ))
    map.Add( new CombinedAlternatorMapRow("Alt2",6000,40,60,2.5 ))
    map.Add( new CombinedAlternatorMapRow("Alt2",6000,60,60,2.5 ))
    map.Add( new CombinedAlternatorMapRow("Alt3",2000,10,95,3.5 ))
    map.Add( new CombinedAlternatorMapRow("Alt3",2000,40,50,3.5 ))
    map.Add( new CombinedAlternatorMapRow("Alt3",2000,60,90,3.5 ))
    map.Add( new CombinedAlternatorMapRow("Alt3",4000,10,99,3.5 ))
    map.Add( new CombinedAlternatorMapRow("Alt3",4000,40, 1,3.5 ))
    map.Add( new CombinedAlternatorMapRow("Alt3",4000,60,55,3.5 ))
    map.Add( new CombinedAlternatorMapRow("Alt3",6000,10,94,3.5 ))
    map.Add( new CombinedAlternatorMapRow("Alt3",6000,40,86,3.5 ))
    map.Add( new CombinedAlternatorMapRow("Alt3",6000,60,13,3.5 ))
    map.Add( new CombinedAlternatorMapRow("Alt4",2000,10,55,2   ))
    map.Add( new CombinedAlternatorMapRow("Alt4",2000,40,45,2   ))
    map.Add( new CombinedAlternatorMapRow("Alt4",2000,60,67,2   ))
    map.Add( new CombinedAlternatorMapRow("Alt4",4000,10,77,2   ))
    map.Add( new CombinedAlternatorMapRow("Alt4",4000,40,39,2   ))
    map.Add( new CombinedAlternatorMapRow("Alt4",4000,60,23,2   ))
    map.Add( new CombinedAlternatorMapRow("Alt4",6000,10,34,2   ))
    map.Add( new CombinedAlternatorMapRow("Alt4",6000,40,67,2   ))
    map.Add( new CombinedAlternatorMapRow("Alt4",6000,60,35,2   ))


 End Sub

 'Grid Management
 private Function AddNewAlternator( list As List(Of ICombinedAlternatorMapRow), ByRef feeback As string) As Boolean

     Dim returnValue As Boolean = true
    
     Dim altName As String = list.First().AlternatorName
     Dim pulleyRatio As Single = list.First().PulleyRatio

     'Check alt does not already exist in list
     If Alternators.where( Function(w) w.AlternatorName=altName).Count>0 then
       feeback="This alternator already exists in in the list, operation not completed."
       Return False
     End If

     Dim alternator  As IAlternator = New Alternator(altSignals, list.ToList())

     Alternators.Add( alternator )


   Return returnValue

 End Function
 Public Function AddAlternator( rows As List( Of ICombinedAlternatorMapRow)  , byref feedback as string) As Boolean

       If Not   AddNewAlternator( rows, feedback )
         feedback=String.Format("Unable to add new alternator : {0}", feedback)
         Return false
       End If

       Return true

 End Function
 Public Function DeleteAlternator( alternatorName As string, byref feedback as string ) As Boolean

     If Alternators.Where( Function(w) w.AlternatorName = alternatorName).Count=0 then
       feedback="This alternator does not exist"
       Return false
     End if


     Dim altToRemove As IAlternator = Alternators.First( Function(w) w.AlternatorName= alternatorName)
     Dim numAlternators = Alternators.Count
   
     Alternators.Remove( altToRemove)

     If Alternators.Count = numAlternators-1 then
      Return True
     Else
      feedback= String.Format("The alternator {0} could not be removed : {1}", alternatorName, feedback)
      Return false
     End If

 End Function
 Public Function UpdateAlternator(  rows As List( Of ICombinedAlternatorMapRow) , byref feedback as string ) As Boolean

       Dim altName As String = rows.First.AlternatorName
       Dim altToUpd As IAlternator = Alternators.First( Function(w) w.AlternatorName = altName)

       If Not DeleteAlternator(altName, feedback) then
          feedback= feedback
          Return false

       End If

       'Re.create alternator.

       Dim replacementAlt As New Alternator( altSignals, rows )
       Alternators.Add( replacementAlt)

       Return true


 End Function

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

 'Initialises the map, only valid when loadingUI for first time in edit mode or always in operational mode.
 Private Function InitialiseMap(filePath As string) As Boolean 

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

 'Can be used to send messages to Vecto.
 Public Event AuxiliaryEvent(ByRef sender As Object, message As String, messageType As AdvancedAuxiliaryMessageType) Implements IAuxiliaryEvent.AuxiliaryEvent


 Public Overrides Function ToString() As String

  Dim sb As New StringBuilder()
  Dim a1,a2,a3,e1,e2,e3 As String


  For Each alt As Alternator In Alternators
    sb.AppendLine("")
    sb.AppendFormat("** {0} ** , PulleyRatio {1}",alt.AlternatorName,alt.PulleyRatio)
    sb.AppendLine("")
    sb.AppendLine  ("******************************************************************")
    sb.AppendLine("")

    Dim i As Integer=1
    sb.AppendLine("Table 1 (2000)" + vbTab + "Table 2 (4000)" + vbTab + "Table 3 (6000)")
    sb.AppendLine("Amps" + vbTab + "Eff" + vbTab + "Amps" + vbTab + "Eff" + vbTab + "Amps" + vbTab + "Eff" + vbTab )
    sb.AppendLine("")
    For  i = 0 to 5 

    a1= alt.InputTable2000(i).Amps.ToString("0.###")
    e1 =alt.InputTable2000(i).Eff .ToString("0.###")
    a2= alt.InputTable4000(i).Amps.ToString("0.###")
    e2 =alt.InputTable4000(i).Eff .ToString("0.###")
    a3= alt.InputTable6000(i).Amps.ToString("0.###")
    e3 =alt.InputTable6000(i).Eff .ToString("0.###")
    sb.AppendLine(a1 + vbTab + e1 + vbTab + a2 + vbTab + e2 + vbTab + a3 + vbTab + e3 + vbTab )

    Next


  Next

    sb.AppendLine("")
    sb.AppendLine("********* COMBINED EFFICIENCY VALUES **************")
    sb.AppendLine("")
    sb.AppendLine( vbTab +"RPM VALUES")
    sb.AppendLine("AMPS" + vbTab + "500" + vbTab + "1500" + vbtab + "2500" + vbtab + "3500" + vbtab + "4500" + vbtab + "5500" + vbtab + "6500" + vbTab  + "7500")
    For a As Single = 1 to alternators.Count * 50

       sb.Append(a.ToString("0") + vbTab)
       For Each  r As Single in {500,1500,2500,3500,4500,5500,6500,7500}
        
          Dim eff as Single = GetEfficiency( r ,a).Efficiency
          
          sb.Append( eff.ToString("0.###") + vbTab)
          
       Next
       sb.AppendLine("")

    Next




  Return sb.ToString()

 End Function



 End Class


End Namespace



