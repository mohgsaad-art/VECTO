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

Namespace Pneumatics

Public Class ActuationsKey


Private _consumerName As String
Private _cycleName As String

'Properties
Public ReadOnly Property ConsumerName As String
    Get
    Return _consumerName
    End Get
End Property

Public ReadOnly Property CycleName As String
    Get
    Return _cycleName
    End Get
End Property

'Constructor
Public Sub New(consumerName As String, cycleName As String)

If consumerName.Trim.Length = 0 Or cycleName.Trim.Length = 0 Then Throw New ArgumentException("ConsumerName and CycleName must be provided")
  _consumerName = consumerName
  _cycleName = cycleName

End Sub


 'Overrides to enable this class to be used as a dictionary key in the ActuationsMap.
 Public Overrides Function Equals(obj As Object) As Boolean

            Dim other As ActuationsKey = CType(obj, ActuationsKey)

            Return other.ConsumerName = Me.ConsumerName AndAlso other.CycleName = Me.CycleName


        End Function
 Public Overrides Function GetHashCode() As Integer
            Return 0
        End Function

End Class

End Namespace

