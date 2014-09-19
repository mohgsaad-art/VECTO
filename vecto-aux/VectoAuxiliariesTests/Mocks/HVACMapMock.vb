Imports VectoAuxiliaries.Hvac

Namespace Mocks
    Public Class HVACMapMock
        Implements IHVACMap

        Public Property MapHeaders As Dictionary(Of String, HVACMapParameter) Implements IHVACMap.MapHeaders

        Public Function Initialise() As Boolean Implements IHVACMap.Initialise
            Return True
        End Function
        Public Function GetMechanicalDemand(ByVal region As Integer, ByVal season As Integer) As Integer Implements IHVACMap.GetMechanicalDemand
            Return 10
        End Function
        Public Function GetMapHeaders() As Dictionary(Of String, HVACMapParameter) Implements IHVACMap.GetMapHeaders

            Throw New NotImplementedException
        End Function

        Public Function GetMapSubSet(search() As String) As List(Of String()) Implements IHVACMap.GetMapSubSet
            Throw New NotImplementedException
        End Function

        Public Function GetUniqueValuesByOrdinal(o As Integer) As List(Of String) Implements IHVACMap.GetUniqueValuesByOrdinal
            Throw New NotImplementedException
        End Function

        Public Function GetElectricalDemand(region As Integer, season As Integer) As Integer Implements IHVACMap.GetElectricalDemand
            Return 1
        End Function

    End Class
End Namespace