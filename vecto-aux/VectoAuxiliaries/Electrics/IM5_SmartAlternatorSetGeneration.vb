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

  Public Interface IM5_SmartAlternatorSetGeneration

    ''' <summary>
  ''' Alternators Generation Power At Crank Idle (W)
  ''' </summary>
  ''' <returns></returns>
  ''' <remarks></remarks>
    Function  AlternatorsGenerationPowerAtCrankIdleWatts() As Single
    ''' <summary>
  ''' Alternators Generation Power At Crank Traction On  (W)
  ''' </summary>
  ''' <returns></returns>
  ''' <remarks></remarks>
    Function  AlternatorsGenerationPowerAtCrankTractionOnWatts() As Single
    ''' <summary>
  ''' Alternators Generation Power At Crank Overrun  (W)
  ''' </summary>
  ''' <returns></returns>
  ''' <remarks></remarks>
    Function  AlternatorsGenerationPowerAtCrankOverrunWatts() As Single 

End Interface

End Namespace


