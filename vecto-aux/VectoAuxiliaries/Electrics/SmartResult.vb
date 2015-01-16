' Copyright 2015 European Union.
' Licensed under the EUPL (the 'Licence');
'
' * You may not use this work except in compliance with the Licence.
' * You may obtain a copy of the Licence at: http://ec.europa.eu/idabc/eupl
' * Unless required by applicable law or agreed to in writing,
'   software distributed under the Licence is distributed on an "AS IS" basis,
'   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
'
' See the LICENSE.txt for the specific language governing permissions and limitations.

Namespace Electrics

   Public Class SmartResult
Implements IComparable(Of SmartResult)

   
   Public Property Amps As Single
   Public Property SmartAmps As Single
   
   'Constructors
   Public Sub new ()
     'An empty constructor is requried. Do not remove.
   End Sub 
   Public Sub new( amps As Single , smartAmps As single)

      Me.Amps = amps
      Me.SmartAmps = smartAmps

    End Sub
   
   'Comparison
   Public Function CompareTo(other As SmartResult) As Integer Implements IComparable(Of SmartResult).CompareTo

      If other.Amps> Me.Amps then   return -1
      If other.Amps=Me.Amps then Return 0  
      
      Return 1

    End Function

   'Comparison Overrides
   Public Overrides Function Equals(obj As Object) As Boolean
            
     Dim other as SmartResult = Ctype( Obj, SmartResult )

     Return Me.Amps=other.Amps

    End Function
   Public Overrides Function GetHashCode() As Integer
     Return 0
   End Function


End Class

End Namespace



