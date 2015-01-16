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

  Public Interface IResultCard

    ''' <summary>
    ''' Returns a List of (SmartResult )
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    ReadOnly Property Results As List(Of SmartResult)
    ''' <summary>
    ''' Returns the Smart Current (A)
    ''' </summary>
    ''' <param name="Amps"></param>
    ''' <returns></returns>
    ''' <remarks>Defaults to 10 Amps if no readings present</remarks>
    Function GetSmartCurrentResult(Amps As Single) As Single

  End Interface


End Namespace


