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

        Public Function GetEfficiency(ByVal rpm As single, ByVal amps As single) As AlternatorMapValues Implements IAlternatorMap.GetEfficiency
            Return New AlternatorMapValues()
        End Function


            Public Event AuxiliaryEvent(ByRef sender As Object, message As String, messageType As VectoAuxiliaries.AdvancedAuxiliaryMessageType) Implements VectoAuxiliaries.IAuxiliaryEvent.AuxiliaryEvent
    End Class

End Namespace
