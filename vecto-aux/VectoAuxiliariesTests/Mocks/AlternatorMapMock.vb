Imports VectoAuxiliaries.Electrics

Namespace Mocks

    Public Class AlternatorMapMock
        Implements IAlternatorMap

        Dim failing As Boolean

        Public Sub New(ByVal isFailing As Boolean)
            failing = isFailing
        End Sub

        Public Function Initialise() As Boolean Implements IAlternatorMap.Initialise
            If failing Then
                Throw New ArgumentException
            Else
                Return True
            End If
        End Function

        Public Function GetEfficiency(ByVal rpm As Integer) As Single Implements IAlternatorMap.GetEfficiency
            Return 0.5
        End Function

        Public Function GetMaximumRegenerationPower(ByVal rpm As Integer) As Single Implements IAlternatorMap.GetMaximumRegenerationPower
            Return 100
        End Function
    End Class

End Namespace
