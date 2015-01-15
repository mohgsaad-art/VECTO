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


'POCO Class to hold information required to
'1. Reference Value
'2. Create UI Controls on Form.

Namespace Hvac

    Public Class HVACMapParameter

        Public Key As String
        Public Name As String
        Public Description As String
        Public Notes As String
        Public Min As Double
        Public Max As Double
        Public SystemType As System.Type
        Public SearchControlType As System.Type
        Public ValueType As System.Type
        Public OrdinalPosition As Integer
        Public UniqueDataValues As List(Of String)
        Public IsOutput As Boolean = False

    End Class



End Namespace