Imports VectoAuxiliaries.Electrics

Namespace Hvac
    Public Class AverageHVACLoadDemand
        Dim map As IHVACMap
        Dim alternator As IAlternator

        Public Sub New(ByVal map As IHVACMap, ByVal alternator As IAlternator)
            Me.map = map
            Me.alternator = alternator
        End Sub

        Public Function Initialise() As Boolean
            Return alternator.Initialise() AndAlso map.Initialise()
        End Function

        Public Function AverageMechanicalPowerAtCrank() As Single
            Throw New NotImplementedException
        End Function

        Function AverageElectricalPowerAtAlternator() As Single
            Throw New NotImplementedException
        End Function

        Function AverageElectricalPowerAtCrank() As Single
            Throw New NotImplementedException
        End Function
    End Class
End Namespace