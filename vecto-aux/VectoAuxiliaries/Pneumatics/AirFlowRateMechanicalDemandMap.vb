Imports System.IO

Namespace Pneumatics

    Public Class AirFlowRateMechanicalDemandMap
        Implements IAirFlowRateMechanicalDemandMap

        Private _filePath As String

        Private _map As New Dictionary(Of Integer, Single)


        Public Sub New(iFilePath As String)

            _filePath = iFilePath

        End Sub


        Public Function Initialise() As Boolean Implements IAirFlowRateMechanicalDemandMap.Initialise

            If File.Exists(_filePath) Then
                Using sr As StreamReader = New StreamReader(_filePath)
                    'get array of lines from csv
                    Dim lines() As String = sr.ReadToEnd().Split(CType(Environment.NewLine, Char()), StringSplitOptions.RemoveEmptyEntries)

                    'Must have at least 2 entries in map to make it usable [dont forget the header row]
                    If lines.Length < 3 Then Throw New ArgumentException("Insufficient rows in csv to build a usable map")

                    _map = New Dictionary(Of Integer, Single)
                    Dim firstline As Boolean = True

                    For Each line As String In lines
                        If Not firstline Then
                            'split the line
                            Dim elements() As String = line.Split(New Char() {","}, StringSplitOptions.RemoveEmptyEntries)
                            '2 entries per line required
                            If (elements.Length <> 2) Then Throw New ArgumentException("Incorrect number of values in csv file")
                            'add values to map
                            _map.Add(elements(0), elements(1))


                            'Test For Non Numeric Data,
                            If (Not firstline AndAlso (Not IsNumeric(elements(0)) OrElse Not IsNumeric(elements(1)))) Then
                                Throw (New ArgumentException("SomeValues were not numeric"))
                            End If

                        Else
                            firstline = False
                        End If
                    Next
                End Using
            Else
                Throw New ArgumentException("supplied input file does not exist")
            End If

            Return True
        End Function

        Public Function GetPower(flowRate As Integer) As Integer Implements IAirFlowRateMechanicalDemandMap.GetPower

            If _map.ContainsKey(flowRate) = False Then

                Throw New ArgumentException("Flow rate was not a key in the FlowRate Mechanical power map")


            End If

            Return _map(flowRate)

        End Function



    End Class

End Namespace



