Imports VectoAuxiliaries.Electrics

Namespace Hvac
    Public Class AverageHVACLoadDemand

        Dim map As IHVACMap
        Dim alternator As IAlternator

        Public Property Region As Integer
        Public Property Season As Integer



        Public Sub New(ByVal map As IHVACMap, ByVal alternator As IAlternator, inputs As IHVACInputs)
            Me.map = map
            Me.alternator = alternator

            Me.Region = inputs.Region
            Me.Season = inputs.Season

        End Sub

        Public Function Initialise() As Boolean
            Return alternator.Initialise() AndAlso map.Initialise()
        End Function

        Public Function AverageMechanicalPowerDemandAtCrank(engineRPM As Integer) As Single

            Return alternator.PulleyGearEfficiency

        End Function

        Function AverageElectricalPowerDemandAtAlternator() As Single


        End Function

        Function AverageElectricalPowerDemandAtCrank() As Single


        End Function



    End Class
End Namespace