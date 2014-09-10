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

        Private map As Dictionary(Of InputValues, OutputValues)

        Public Sub New()
            map = New Dictionary(Of InputValues, OutputValues)()

            For i As Integer = 25 To 50
                For j As Integer = 25 To 50
                    Dim input As New InputValues(i, j)
                    Dim output As New OutputValues(i * i * j, i * i * j * 5)
                    map.Add(input, output)
                Next
            Next
        End Sub

        Private Structure InputValues
            Public ReadOnly Region As Integer
            Public ReadOnly Season As Integer

            Public Sub New(ByVal region As Integer, ByVal season As Integer)
                Me.Region = region
                Me.Season = season
            End Sub
        End Structure

        Private Structure OutputValues
            Public ReadOnly MechanicalDemand As Integer
            Public ReadOnly ElectricalDemand As Integer

            Public Sub New(ByVal mechanicalDemand As Integer, ByVal electricalDemand As Integer)
                Me.MechanicalDemand = mechanicalDemand
                Me.ElectricalDemand = electricalDemand
            End Sub
        End Structure

        Public Function GetMechanicalDemand(ByVal region As Integer, ByVal season As Integer) As Integer
            Dim key As InputValues = New InputValues(region, season)
            Dim val As OutputValues = map(key)
            Return val.MechanicalDemand
        End Function



        Public Function GetElectricalDemand(ByVal region As Integer, ByVal season As Integer) As Integer
            Dim key As InputValues = New InputValues(region, season)
            Dim val As OutputValues = map(key)
            Return val.ElectricalDemand
        End Function
    End Class
End Namespace