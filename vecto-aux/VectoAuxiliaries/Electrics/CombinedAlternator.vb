Option Strict On

Imports VectoAuxiliaries.Electrics
Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Text
Imports System.IO
Imports DocumentFormat.OpenXml
Imports DocumentFormat.OpenXml.Spreadsheet
Imports Newtonsoft.Json
Imports VectoAuxiliaries
Imports System.Globalization


Namespace Electrics

Public Class CombinedAlternator
        Implements IAlternatorMap, ICombinedAlternator

        Private map As New List(Of ICombinedAlternatorMapRow)
        Public Property Alternators As New List(Of IAlternator) Implements ICombinedAlternator.Alternators
        Private OriginalAlternators As New List(Of IAlternator)
        Private FilePath As String
        Private altSignals As ICombinedAlternatorSignals
        Private Signals As ISignals
        Private AverageAlternatorsEfficiency As AlternatorMapValues

        'Interface Implementation
        Public Function GetEfficiency(ByVal CrankRPM As Single, ByVal Amps As Single) As AlternatorMapValues Implements IAlternatorMap.GetEfficiency

            altSignals.CrankRPM = CrankRPM
            altSignals.CurrentDemandAmps = Amps / Alternators.Count

            Dim alternatorMapValues As AlternatorMapValues = Nothing

            If Signals Is Nothing OrElse Signals.RunningCalc Then
                'If running calc cycle get efficiency from interpolation function
                alternatorMapValues = New AlternatorMapValues(Convert.ToSingle(Alternators.Average(Function(a) a.Efficiency) / 100))
            Else
                'If running Pre calc cycle get an average of inputs
                alternatorMapValues = AverageAlternatorsEfficiency
            End If

            If alternatorMapValues.Efficiency <= 0 Then
                alternatorMapValues = New AlternatorMapValues(0.01)
            End If

            Return alternatorMapValues

        End Function
        Public Function Initialise() As Boolean Implements IAlternatorMap.Initialise

            'From the map we construct this CombinedAlternator object and original CombinedAlternator Object

            Alternators.Clear()
            OriginalAlternators.Clear()


            For Each alt As IEnumerable(Of ICombinedAlternatorMapRow) In map.GroupBy(Function(g) g.AlternatorName)

                Dim altName As String = alt.First().AlternatorName
                Dim pulleyRatio As Single = alt.First().PulleyRatio


                Dim alternator As IAlternator = New Alternator(altSignals, alt.ToList())

                Alternators.Add(alternator)


            Next

            Return True

        End Function

        'Constructors
        Public Sub New(filePath As String, Optional signals As ISignals = Nothing)

            Dim feedback As String = String.Empty
            Me.Signals = signals

            If Not FilePathUtils.ValidateFilePath(filePath, ".aalt", feedback) Then
                Throw New ArgumentException(String.Format("Combined Alternator requires a valid .AALT filename. : {0}", feedback))
            Else
                Me.FilePath = filePath
            End If


            Me.altSignals = New CombinedAlternatorSignals()


            'IF file exists then read it otherwise create a default.

            If File.Exists(filePath) AndAlso InitialiseMap(filePath) Then
                Initialise()
            Else
                'Create Default Map
                CreateDefaultMap()
                Initialise()
            End If

            ' Calculate alternators average which is used only in the pre-run
            Dim efficiencySum As Single
            Dim efficiencyAverage As Single

            For Each alt As IAlternator In Alternators
                efficiencySum += alt.InputTable2000.ElementAt(1).Eff
                efficiencySum += alt.InputTable2000.ElementAt(2).Eff
                efficiencySum += alt.InputTable2000.ElementAt(3).Eff

                efficiencySum += alt.InputTable4000.ElementAt(1).Eff
                efficiencySum += alt.InputTable4000.ElementAt(2).Eff
                efficiencySum += alt.InputTable4000.ElementAt(3).Eff

                efficiencySum += alt.InputTable6000.ElementAt(1).Eff
                efficiencySum += alt.InputTable6000.ElementAt(2).Eff
                efficiencySum += alt.InputTable6000.ElementAt(3).Eff
            Next

            efficiencyAverage = efficiencySum / (Alternators.Count * 9)
            AverageAlternatorsEfficiency = New AlternatorMapValues(efficiencyAverage / 100)

        End Sub

        'Helpers
        Private Sub CreateDefaultMap()

            map.Clear()

            map.Add(New CombinedAlternatorMapRow("Alt1", 2000, 10, 62, 3.6))
            map.Add(New CombinedAlternatorMapRow("Alt1", 2000, 27, 70, 3.6))
            map.Add(New CombinedAlternatorMapRow("Alt1", 2000, 53, 30, 3.6))

            map.Add(New CombinedAlternatorMapRow("Alt1", 4000, 10, 64, 3.6))
            map.Add(New CombinedAlternatorMapRow("Alt1", 4000, 63, 74, 3.6))
            map.Add(New CombinedAlternatorMapRow("Alt1", 4000, 125, 68, 3.6))

            map.Add(New CombinedAlternatorMapRow("Alt1", 6000, 10, 53, 3.6))
            map.Add(New CombinedAlternatorMapRow("Alt1", 6000, 68, 70, 3.6))
            map.Add(New CombinedAlternatorMapRow("Alt1", 6000, 136, 62, 3.6))

            map.Add(New CombinedAlternatorMapRow("Alt2", 2000, 10, 62, 3))
            map.Add(New CombinedAlternatorMapRow("Alt2", 2000, 27, 70, 3))
            map.Add(New CombinedAlternatorMapRow("Alt2", 2000, 53, 30, 3))

            map.Add(New CombinedAlternatorMapRow("Alt2", 4000, 10, 64, 3))
            map.Add(New CombinedAlternatorMapRow("Alt2", 4000, 63, 74, 3))
            map.Add(New CombinedAlternatorMapRow("Alt2", 4000, 125, 68, 3))

            map.Add(New CombinedAlternatorMapRow("Alt2", 6000, 10, 53, 3))
            map.Add(New CombinedAlternatorMapRow("Alt2", 6000, 68, 70, 3))
            map.Add(New CombinedAlternatorMapRow("Alt2", 6000, 136, 62, 3))


        End Sub

        'Grid Management
        Private Function AddNewAlternator(list As List(Of ICombinedAlternatorMapRow), ByRef feeback As String) As Boolean

            Dim returnValue As Boolean = True

            Dim altName As String = list.First().AlternatorName
            Dim pulleyRatio As Single = list.First().PulleyRatio

            'Check alt does not already exist in list
            If Alternators.Where(Function(w) w.AlternatorName = altName).Count > 0 Then
                feeback = "This alternator already exists in in the list, operation not completed."
                Return False
            End If

            Dim alternator As IAlternator = New Alternator(altSignals, list.ToList())

            Alternators.Add(alternator)


            Return returnValue

        End Function
        Public Function AddAlternator(rows As List(Of ICombinedAlternatorMapRow), ByRef feedback As String) As Boolean

            If Not AddNewAlternator(rows, feedback) Then
                feedback = String.Format("Unable to add new alternator : {0}", feedback)
                Return False
            End If

            Return True

        End Function
        Public Function DeleteAlternator(alternatorName As String, ByRef feedback As String, CountValidation As Boolean) As Boolean

            'Is this the last alternator, if so deny the user the right to remove it.
            If CountValidation AndAlso Alternators.Count < 2 Then
                feedback = "There must be at least one alternator remaining, operation aborted."
                Return False
            End If

            If Alternators.Where(Function(w) w.AlternatorName = alternatorName).Count = 0 Then
                feedback = "This alternator does not exist"
                Return False
            End If

            Dim altToRemove As IAlternator = Alternators.First(Function(w) w.AlternatorName = alternatorName)
            Dim numAlternators As Integer = Alternators.Count

            Alternators.Remove(altToRemove)

            If Alternators.Count = numAlternators - 1 Then
                Return True
            Else
                feedback = String.Format("The alternator {0} could not be removed : {1}", alternatorName, feedback)
                Return False
            End If

        End Function
        'Public Function UpdateAlternator( gridIndex As Integer, rows As List(Of ICombinedAlternatorMapRow),  ByRef feedback As String) As Boolean

        '      Dim altName As String = rows.First.AlternatorName
        '      Dim altToUpd As IAlternator = Alternators.First(Function(w) w.AlternatorName = altName)

        '      If Not DeleteAlternator(altName, feedback) Then
        '         feedback = feedback
        '         Return False

        '      End If

        '      'Re.create alternator.

        '      Dim replacementAlt As New Alternator(altSignals, rows)
        '      Alternators.Add(replacementAlt)

        '      Return True


        'End Function

        'Persistance Functions
        Public Function Save(aaltPath As String) As Boolean


            Dim returnValue As Boolean = True
            Dim sb As New StringBuilder()
            Dim row As Integer = 0
            Dim amps, eff As Single

            'write headers  
            sb.AppendLine("[AlternatorName],[RPM],[Amps],[Efficiency],[PulleyRatio]")

            'write details
            For Each alt As IAlternator In Alternators.OrderBy(Function(o) o.AlternatorName)


                '2000 - IE Alt1,2000,10,50,3
                For row = 1 To 3
                    amps = alt.InputTable2000(row).Amps : eff = alt.InputTable2000(row).Eff
                    sb.Append(alt.AlternatorName + ",2000," + amps.ToString("0.000") + "," + eff.ToString("0.000") + "," + alt.PulleyRatio.ToString("0.000"))
                    sb.AppendLine("")
                Next

                '4000 - IE Alt1,2000,10,50,3
                For row = 1 To 3
                    amps = alt.InputTable4000(row).Amps : eff = alt.InputTable4000(row).Eff
                    sb.Append(alt.AlternatorName + ",4000," + amps.ToString("0.000") + "," + eff.ToString("0.000") + "," + alt.PulleyRatio.ToString("0.000"))
                    sb.AppendLine("")
                Next

                '6000 - IE Alt1,2000,10,50,3
                For row = 1 To 3
                    amps = alt.InputTable6000(row).Amps : eff = alt.InputTable6000(row).Eff
                    sb.Append(alt.AlternatorName + ",6000," + amps.ToString("0.000") + "," + eff.ToString("0.000") + "," + alt.PulleyRatio.ToString("0.000"))
                    sb.AppendLine("")
                Next


            Next

            'Add Model Source
            sb.AppendLine("[MODELSOURCE]")
            sb.Append(Me.ToString())

            ' Write the stream cotnents to a new file named "AllTxtFiles.txt" 
            Using outfile As New StreamWriter(aaltPath)
                outfile.Write(sb.ToString())
            End Using

            Return returnValue

        End Function
        Private Function Load() As Boolean

            If Not InitialiseMap(FilePath) Then Return False


            Return True

        End Function

        'Initialises the map, only valid when loadingUI for first time in edit mode or always in operational mode.
        Private Function InitialiseMap(filePath As String) As Boolean

            Dim returnValue As Boolean = False
            Dim elements As String()

            If File.Exists(filePath) Then
                Using sr As StreamReader = New StreamReader(filePath)
                    'get array og lines fron csv
                    Dim lines() As String = sr.ReadToEnd().Split(CType(Environment.NewLine, Char()), StringSplitOptions.RemoveEmptyEntries)

                    'Must have at least 2 entries in map to make it usable [dont forget the header row]
                    If (lines.Count() < 10) Then
                        Throw New ArgumentException("Insufficient rows in csv to build a usable map")
                    End If

                    map = New List(Of ICombinedAlternatorMapRow)

                    Dim firstline As Boolean = True

                    For Each line As String In lines
                        If Not firstline Then

                            'Advanced Alternator Source Check.
                            If line.Contains("[MODELSOURCE") Then Exit For

                            'split the line
                            elements = line.Split(New Char() {","c}, StringSplitOptions.RemoveEmptyEntries)
                            '3 entries per line required
                            If (elements.Length <> 5) Then
                                Throw New ArgumentException("Incorrect number of values in csv file")
                            End If
                            'add values to map

                            map.Add(New CombinedAlternatorMapRow(elements(0), CType(elements(1), Single), CType(elements(2), Single), CType(elements(3), Single), CType(elements(4), Single)))

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

        'This is used to generate a diagnostics output which enables the user to 
        'Determine if they beleive the resulting map is what is expected
        'Basically it is a check against the model/Spreadsheet
        Public Overrides Function ToString() As String

            Dim sb As New StringBuilder()
            Dim a1, a2, a3, e1, e2, e3 As String


            For Each alt As Alternator In Alternators.OrderBy(Function(o) o.AlternatorName)
                sb.AppendLine("")
                sb.AppendFormat("** {0} ** , PulleyRatio {1}", alt.AlternatorName, alt.PulleyRatio)
                sb.AppendLine("")
                sb.AppendLine("******************************************************************")
                sb.AppendLine("")

                Dim i As Integer = 1
                sb.AppendLine("Table 1 (2000)" + vbTab + "Table 2 (4000)" + vbTab + "Table 3 (6000)")
                sb.AppendLine("Amps" + vbTab + "Eff" + vbTab + "Amps" + vbTab + "Eff" + vbTab + "Amps" + vbTab + "Eff" + vbTab)
                sb.AppendLine("")
                For i = 1 To 3

                    a1 = alt.InputTable2000(i).Amps.ToString("0")
                    e1 = alt.InputTable2000(i).Eff.ToString("0.000")
                    a2 = alt.InputTable4000(i).Amps.ToString("0")
                    e2 = alt.InputTable4000(i).Eff.ToString("0.000")
                    a3 = alt.InputTable6000(i).Amps.ToString("0")
                    e3 = alt.InputTable6000(i).Eff.ToString("0.000")
                    sb.AppendLine(a1 + vbTab + e1 + vbTab + a2 + vbTab + e2 + vbTab + a3 + vbTab + e3 + vbTab)

                Next


            Next

            'sb.AppendLine("")
            'sb.AppendLine("********* COMBINED EFFICIENCY VALUES **************")
            'sb.AppendLine("")
            'sb.AppendLine(vbTab + "RPM VALUES")
            'sb.AppendLine("AMPS" + vbTab + "500" + vbTab + "1500" + vbTab + "2500" + vbTab + "3500" + vbTab + "4500" + vbTab + "5500" + vbTab + "6500" + vbTab + "7500")
            'For a As Single = 1 To Alternators.Count * 50

            '    sb.Append(a.ToString("0") + vbTab)
            '    For Each r As Single In {500, 1500, 2500, 3500, 4500, 5500, 6500, 7500}

            '        Dim eff As Single = GetEfficiency(r, a).Efficiency

            '        sb.Append(eff.ToString("0.000") + vbTab)

            '    Next
            '    sb.AppendLine("")

            'Next




            Return sb.ToString()

        End Function


        'Equality
        Public Function IsEqualTo(other As ICombinedAlternator) As Boolean Implements ICombinedAlternator.IsEqualTo

            'Count Check.
            If Me.Alternators.Count <> other.Alternators.Count Then Return False

            For Each alt As IAlternator In Me.Alternators

                'Can we find the same alternatorName in other
                If other.Alternators.Where(Function(f) f.AlternatorName = alt.AlternatorName).Count() <> 1 Then Return False

                'get the alternator to compare and compare it.
                If Not alt.IsEqualTo(other.Alternators.first(Function(f) f.AlternatorName = alt.AlternatorName)) Then Return False

            Next

            Return True

        End Function



    End Class


End Namespace



