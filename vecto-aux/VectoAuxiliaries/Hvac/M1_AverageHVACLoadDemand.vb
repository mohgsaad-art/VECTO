Imports VectoAuxiliaries.Electrics

Namespace Hvac

    Public Class M1_AverageHVACLoadDemand

        Dim map As IHVACMap


        Public Property Region As Integer
        Public Property Season As Integer


        Public Sub New(ByVal map As IHVACMap,  inputs As IHVACInputs)
            Me.map = map


            Me.Region = inputs.Region
            Me.Season = inputs.Season


        End Sub

        Public Function Initialise() As Boolean

        End Function

        Public Function AverageMechanicalPowerDemandAtCrank() As Single




            Return 0 'TODO FIX THIS.


        End Function

        Function AverageElectricalPowerDemandAtAlternator() As Single

            Return 0 'TODO FIX THIS


        End Function

        Function AverageElectricalPowerDemandAtCrank(engineRPM As Single) As Single


            Return 0 'TODO FIX THIS


        End Function



    End Class
End Namespace