Imports VectoAuxiliaries.Pneumatics


Namespace Mocks

    Public Class AirFlowRateMechanicalDemandMapMock
        Implements IAirFlowRateMechanicalDemandMap

        Private map = New Dictionary(Of Integer, Single)

        Public Function Initialise() As Boolean Implements IAirFlowRateMechanicalDemandMap.Initialise

            map.Add(100, 2.0)
            map.Add(200, 3.0)
            map.Add(300, 4.0)
            map.Add(500, 5.0)

            Return True

        End Function


        Public Function GetPower(flowRate As Integer) As Integer Implements IAirFlowRateMechanicalDemandMap.GetPower

            Return map(flowRate)

        End Function

    End Class


End Namespace


