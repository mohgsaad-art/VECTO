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

   Public Interface IM0_5_SmartAlternatorSetEfficiency

     ''' <summary>
     ''' Smart Idle Current (A)
     ''' </summary>
     ''' <value></value>
     ''' <returns></returns>
     ''' <remarks></remarks>
     Readonly Property SmartIdleCurrent() As single
     ''' <summary>
     ''' Alternators Efficiency In Idle ( Fraction )
     ''' </summary>
     ''' <value></value>
     ''' <returns></returns>
     ''' <remarks></remarks>
     Readonly Property AlternatorsEfficiencyIdleResultCard( ) As single
     ''' <summary>
     ''' Smart Traction Current (A)
     ''' </summary>
     ''' <value></value>
     ''' <returns></returns>
     ''' <remarks></remarks>
     Readonly Property SmartTractionCurrent As Single
     ''' <summary>
     ''' Alternators Efficiency In Traction ( Fraction )
     ''' </summary>
     ''' <value></value>
     ''' <returns></returns>
     ''' <remarks></remarks>
     Readonly Property AlternatorsEfficiencyTractionOnResultCard() As Single
     ''' <summary>
     ''' Smart Overrrun Current (A)
     ''' </summary>
     ''' <value></value>
     ''' <returns></returns>
     ''' <remarks></remarks>
     Readonly Property SmartOverrunCurrent As Single
     ''' <summary>
     ''' Alternators Efficiency In Overrun ( Fraction )
     ''' </summary>
     ''' <value></value>
     ''' <returns></returns>
     ''' <remarks></remarks>
     Readonly Property AlternatorsEfficiencyOverrunResultCard() As single

End Interface

End Namespace



