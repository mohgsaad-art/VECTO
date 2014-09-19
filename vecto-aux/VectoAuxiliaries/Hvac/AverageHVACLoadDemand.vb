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

        Public Function AverageMechanicalPowerDemandAtCrank() As Single

            Dim mechD As Single = map.GetMechanicalDemand(Region, Season)

            Dim pulleyGearEfficiency As Single = alternator.PulleyGearEfficiency


            Return mechD / pulleyGearEfficiency


        End Function

        Function AverageElectricalPowerDemandAtAlternator() As Single

            Return map.GetElectricalDemand(Region, Season)


        End Function

        Function AverageElectricalPowerDemandAtCrank(engineRPM As Single) As Single

            Dim alternatorEfficiency As Single = alternator.GetEfficiency(engineRPM)
            Dim hvacElectricalPowerDemand As Single = map.GetElectricalDemand(Region, Season)

            Dim result As Single = (hvacElectricalPowerDemand / alternatorEfficiency) / alternator.PulleyGearEfficiency

            Return result


        End Function



    End Class
End Namespace