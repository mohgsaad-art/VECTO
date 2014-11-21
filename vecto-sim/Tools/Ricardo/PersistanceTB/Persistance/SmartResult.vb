Namespace Electrics

<Serializable()>
Public Class SmartResult
Implements IComparable(Of SmartResult)


Public Property Amps As Single

Public Property SmartAmps As Single

Public Sub new ()


End Sub

Public Sub new( amps As Single , smartAmps As single)

  Me.Amps = amps
  Me.SmartAmps = smartAmps

End Sub



    Public Function CompareTo(other As SmartResult) As Integer Implements IComparable(Of SmartResult).CompareTo


    If other.Amps> Me.Amps then   return -1
    If other.Amps=Me.Amps then Return 0  
  
    Return 1


    End Function


        Public Overrides Function Equals(obj As Object) As Boolean
            
            Dim other as SmartResult = Ctype( Obj, SmartResult )

            Return Me.Amps=other.Amps

        End Function

        Public Overrides Function GetHashCode() As Integer
            Return 0
        End Function



End Class



End Namespace



