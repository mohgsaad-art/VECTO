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


    Public Interface IAlternatorMap
     Inherits IAuxiliaryEvent
        ''' <summary>
        ''' Initialise the map from supplied csv data
        ''' </summary>
        ''' <returns>Boolean - true if map is created successfully</returns>
        ''' <remarks></remarks>
        Function Initialise() As Boolean

        ''' <summary>
        ''' Returns the alternator efficiency at given rpm
        ''' </summary>
        ''' <param name="rpm">alternator rotation speed</param>
        ''' <returns>Single</returns>
        ''' <remarks></remarks>
        Function GetEfficiency(ByVal rpm As single, ByVal amps As single) As AlternatorMapValues

    End Interface


End Namespace