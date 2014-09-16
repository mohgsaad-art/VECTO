Imports VectoAuxiliaries.Hvac

Namespace Mocks
    Public Class HVACMapMock
        Implements IHVACMap

        Public Function Initialise() As Boolean Implements IHVACMap.Initialise
            Return True
        End Function

        Public Function GetMechanicalDemand(ByVal region As Integer, ByVal season As Integer) As Integer Implements IHVACMap.GetMechanicalDemand
            Return 10
        End Function

        Public Function GetElectricalDemand(ByVal region As Integer, ByVal season As Integer) As Integer Implements IHVACMap.GetElectricalDemand
            Return 10
        End Function
    End Class
End Namespace