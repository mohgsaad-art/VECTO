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

        Public Function GetEfficiency(ByVal rpm As Integer, ByVal amps As Integer) As AlternatorMapValues Implements IAlternatorMap.GetEfficiency
            Return New AlternatorMapValues()
        End Function


    End Class

End Namespace
