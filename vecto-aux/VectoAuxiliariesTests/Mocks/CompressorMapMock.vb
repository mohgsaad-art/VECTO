Imports VectoAuxiliaries.Pneumatics

Namespace Mocks

    Public Class CompressorMapMock
        Implements ICompressorMap

        Dim failing As Boolean

        Public Sub New(ByVal isFailing As Boolean)
            failing = isFailing
        End Sub

        Public Function Initialise() As Boolean Implements ICompressorMap.Initialise
            If failing Then
                Throw New System.ArgumentException
            Else
                Return True
            End If
        End Function

        Public Function GetFlowRate(ByVal rpm As Integer) As Single Implements ICompressorMap.GetFlowRate
            Return 2.0
        End Function

        Public Function GetPowerCompressorOn(ByVal rpm As Integer) As Single Implements ICompressorMap.GetPowerCompressorOn
            Return 8.0
        End Function

        Public Function GetPowerCompressorOff(ByVal rpm As Integer) As Single Implements ICompressorMap.GetPowerCompressorOff
            Return 5.0
        End Function


        Public Function GetAveragePowerDemandPerCompressorUnitFlowRate() As Single Implements ICompressorMap.GetAveragePowerDemandPerCompressorUnitFlowRate

            Return 0.01

        End Function


    End Class
End Namespace