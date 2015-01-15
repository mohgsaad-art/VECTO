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

Public Class M8
Implements IM8

   #Region "Fields"

   private _m1 As IM1_AverageHVACLoadDemand
   private _m6 As IM6
   private _m7 As IM7
   private _signals As ISignals

#End Region
 
   #Region "Internal Sums and Switches"   

   'Internal Staged Sums and Switches
   private ReadOnly Property Sum1 As Single
    Get
      Return _m7.SmartElectricalAndPneumaticAuxAltPowerGenAtCrank + _m7.SmartElectricalAndPneumaticAuxAirCompPowerGenAtCrank
    End Get
End Property
   private ReadOnly Property Sum2 As Single
    Get
     Return _m7.SmartElectricalOnlyAuxAltPowerGenAtCrank +_m6.AveragePowerDemandAtCrankFromPneumatics
    End Get
End Property
   private ReadOnly Property Sum3 As Single
    Get
    Return _m7.SmartPneumaticOnlyAuxAirCompPowerGenAtCrank + _m6.AvgPowerDemandAtCrankFromElectricsIncHVAC
    End Get
End Property
   private ReadOnly Property Sum4 As Single
    Get
     Return _m6.AvgPowerDemandAtCrankFromElectricsIncHVAC + _m6.AveragePowerDemandAtCrankFromPneumatics
    End Get
End Property
   private ReadOnly Property Sum5 As Single
    Get
    Return _m1.AveragePowerDemandAtCrankFromHVACMechanicalsWatts + SW5
    End Get
End Property
   
   Private ReadOnly Property SW1 As Single 
       Get
       Return If( _signals.SmartPneumatics, Sum1, Sum2)
       End Get
   End Property
   Private ReadOnly Property SW2 As Single
    Get
    Return If( _signals.SmartPneumatics,Sum3,Sum4)
    End Get
End Property
   Private ReadOnly Property SW3 As Single
    Get
    Return If(_signals.SmartPneumatics,_m7.SmartElectricalAndPneumaticAuxAltPowerGenAtCrank,_m7.SmartElectricalOnlyAuxAltPowerGenAtCrank)
    End Get
End Property
   Private ReadOnly Property SW4 As integer
    Get
    Return If( _signals.SmartElectrics,_m6.SmartElecAndPneumaticsCompressorFlag,_m6.SmartPneumaticsOnlyCompressorFlag)
    End Get
End Property
   Private ReadOnly Property SW5 As Single
       Get
       Return If( _signals.SmartElectrics,SW1,SW2)
       End Get
   End Property

#End Region

   'Class Outputs
   'OUT1
   Public ReadOnly Property AuxPowerAtCrankFromElectricalHVACAndPneumaticsAncillaries As Single Implements IM8.AuxPowerAtCrankFromElectricalHVACAndPneumaticsAncillaries
            Get
            Return Sum5
            End Get
        End Property

   'OUT2
   Public ReadOnly Property SmartElectricalAlternatorPowerGenAtCrank As Single Implements IM8.SmartElectricalAlternatorPowerGenAtCrank
            Get
            Return SW3
            End Get
        End Property

   'OUT3
   Public ReadOnly Property CompressorFlag As Integer Implements IM8.CompressorFlag
            Get
            Return SW4
            End Get
        End Property


   'Constructor
   Public Sub new( m1 As IM1_AverageHVACLoadDemand, m6 As IM6, m7 As IM7, signals As ISignals)

             _m1=m1
             _m6=m6
             _m7=m7
             _signals=signals

        End Sub


End Class

End Namespace



