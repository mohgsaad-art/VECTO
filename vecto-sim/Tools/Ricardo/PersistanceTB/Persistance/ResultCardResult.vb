
Imports VectoAuxiliaries.Electrics

Namespace Electrics

Public class ResultCardResult 


Public Property Amps As Single
Public Property SmartAmps As Single

Public Sub new ( amps As Single, smartAmps As single)

    Me.Amps = amps
    Me.SmartAmps = smartAmps

End Sub

    Public Overrides Function Equals(obj As Object) As Boolean

      Dim other As ResultCardResult = CType(obj, ResultCardResult)

        Return Me.Amps= other.Amps

    End Function

    Public Overrides Function GetHashCode() As Integer

        Return 0

    End Function



End class



End Namespace


