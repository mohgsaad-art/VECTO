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

Imports VectoAuxiliaries.Electrics
Imports VectoAuxiliaries.Pneumatics
Imports VectoAuxiliaries.Hvac

Namespace DownstreamModules

Public Class M7
     implements IM7

      Private _m5 As IM5_SmartAlternatorSetGeneration
      Private _m6 As IM6
      Private _signals As ISignals

      'Boolean  Conditions
      Private Readonly Property C1 As Boolean
           Get
             Return If(_m6.OverrunFlag=1,True,False) Andalso _signals.ClutchEngaged Andalso _signals.InNeutral=false
           End Get
       End Property

      'Internal Switched Outputs 
      Private Readonly Property SW1 As Single
           Get
             
              Dim idle As Boolean = _signals.EngineSpeed <=_signals.EngineIdleSpeed ANDAlso  ( NOT _signals.ClutchEngaged OrElse _signals.InNeutral)

              Return If ( _signals.Idle, _m5.AlternatorsGenerationPowerAtCrankIdleWatts, _m5.AlternatorsGenerationPowerAtCrankTractionOnWatts)
           End Get
       End Property
      Private Readonly Property SW2 As Single
           Get
             Return  If( C1, _m6.SmartElecAndPneumaticAltPowerGenAtCrank, SW1)
           End Get
       End Property
      Private Readonly Property SW3 As Single
           Get
              Return If( C1,_m6.SmartElecAndPneumaticAirCompPowerGenAtCrank,_m6.AveragePowerDemandAtCrankFromPneumatics)
           End Get
       End Property
      Private Readonly Property SW4 As Single
           Get
             Return If( C1,_m6.SmartElecOnlyAltPowerGenAtCrank,SW1)
           End Get
       End Property
      Private Readonly Property SW5 As Single
           Get
             Return If( C1, _m6.SmartPneumaticOnlyAirCompPowerGenAtCrank,_m6.AveragePowerDemandAtCrankFromPneumatics)
           End Get
       End Property
      
      'Public readonly properties  ( Outputs )
      Public ReadOnly Property SmartElectricalAndPneumaticAuxAltPowerGenAtCrank As Single Implements IM7.SmartElectricalAndPneumaticAuxAltPowerGenAtCrank
            Get
               Return SW2
            End Get
        End Property
      Public ReadOnly Property SmartElectricalAndPneumaticAuxAirCompPowerGenAtCrank As Single Implements IM7.SmartElectricalAndPneumaticAuxAirCompPowerGenAtCrank
            Get
              Return SW3
            End Get
        End Property
      Public ReadOnly Property SmartElectricalOnlyAuxAltPowerGenAtCrank As Single Implements IM7.SmartElectricalOnlyAuxAltPowerGenAtCrank
            Get
              Return SW4
            End Get
        End Property
      Public ReadOnly Property SmartPneumaticOnlyAuxAirCompPowerGenAtCrank As Single Implements IM7.SmartPneumaticOnlyAuxAirCompPowerGenAtCrank
            Get
              Return SW5
            End Get
        End Property

      'Constructor
      Public sub new ( m5 as IM5_SmartAlternatorSetGeneration, _
                         m6 As IM6, _
                         signals As ISignals)
            
          _m5  = m5
          _m6  = m6
          _signals = signals

        End Sub


End Class


End Namespace



