Imports System.IO

Namespace Hvac
    Public Class HVACMap
        'Some sort of multi-dimensional map implemented here
        'No interpolation - too expensive/complex to implement?
        'Set list of choices in each dimension of input

        'options
        '1. multi-dimension array
        '2. dictionary with a Tuple Key?
        '3. dictionary with struct as the key, struct would encapsulate the dimensions - this would map to the HVAC inputs
        'Need to test different choices for speed/ease of use

        'Initial Mock Implementation - 2 input parameters, 2 output values
        'probably easiest to implement the inputs and outputs as structs and then create a dictionary<input,output> ?

        'could define the list of inputs based on the supplied map

        ''' <summary>
        ''' Path to HVAC map csv
        ''' </summary>
        ''' <remarks></remarks>
        Private ReadOnly filePath As String

        ''' <summary>
        ''' Dictionary of values keyed by input value combinations
        ''' </summary>
        ''' <remarks></remarks>
        Private map As Dictionary(Of InputValues, OutputValues)

        ''' <summary>
        ''' Creates a new instance of and HVACMap class
        ''' </summary>
        ''' <remarks></remarks>
        Public Sub New(ByVal path As String)
            filePath = path
        End Sub

        Public Function Initialise() As Boolean
            If (File.Exists(filePath)) Then
                Using sr As StreamReader = New StreamReader(filePath)
                    'get array of lines from csv
                    Dim lines() As String = sr.ReadToEnd().Split(CType(Environment.NewLine, Char()), StringSplitOptions.RemoveEmptyEntries)

                    map = New Dictionary(Of InputValues, OutputValues)()
                    Dim firstline As Boolean = True

                    For Each line As String In lines
                        If Not firstline Then
                            'split the line
                            Dim elements() As String = line.Split(New Char() {","}, StringSplitOptions.RemoveEmptyEntries)
                            '4 entries per line required
                            If (elements.Length <> 4) Then Throw New ArgumentException("Incorrect number of values in csv file")
                            'add values to map
                            Dim region As String = elements(0)
                            Dim season As String = elements(1)
                            Dim input As New InputValues(region, season)
                            Dim electricalDemand As String = elements(2)
                            Dim mechanicalDemand As String = elements(3)
                            Dim output As New OutputValues(electricalDemand, mechanicalDemand)
                            map.Add(input, output)
                        Else
                            firstline = False
                        End If
                    Next
                End Using
                Return True
            Else
                Throw New ArgumentException("supplied input file does not exist")
            End If
        End Function


        Public Function GetMechanicalDemand(ByVal region As Integer, ByVal season As Integer) As Integer
            Dim key As InputValues = New InputValues(region, season)
            Dim val As OutputValues = map(key)
            Return val.MechanicalDemand
        End Function

        Public Function GetElectricalDemand(ByVal region As Integer, ByVal season As Integer) As Integer
            Dim key As InputValues = New InputValues(region, season)
            If (map.ContainsKey(key)) Then
                Dim val As OutputValues = map(key)
                Return val.ElectricalDemand
            Else
                Throw New ArgumentException("Key was not present")
            End If
        End Function

#Region "Nested Structures"
        Private Structure InputValues
            Private ReadOnly region As Integer
            Private ReadOnly season As Integer

            Public Sub New(ByVal region As Integer, ByVal season As Integer)
                Me.Region = region
                Me.Season = season
            End Sub
        End Structure

        Private Structure OutputValues
            Public ReadOnly MechanicalDemand As Single
            Public ReadOnly ElectricalDemand As Single

            Public Sub New(ByVal electricalDemand As Single, ByVal mechanicalDemand As Single)
                Me.MechanicalDemand = mechanicalDemand
                Me.ElectricalDemand = electricalDemand
            End Sub
        End Structure
#End Region

    End Class
End Namespace