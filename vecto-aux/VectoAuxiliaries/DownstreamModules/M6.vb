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

  Public Class M6
Implements IM6

   #Region "Private Field - Constructor requirements"
  private _m1      As IM1_AverageHVACLoadDemand
  private _m2      As IM2_AverageElectricalLoadDemand
  private _m3      As IM3_AveragePneumaticLoadDemand
  Private _m4      As IM4_AirCompressor
  Private _m5      As IM5_SmartAlternatorSetGeneration
  Private _signals As ISignals
 #End Region
   
   'OUT1
   Public ReadOnly Property OverrunFlag As Integer Implements IM6.OverrunFlag
        
        Get                
          Return  VC0
        End Get
    End Property
   'OUT2
   Public ReadOnly Property SmartElecAndPneumaticsCompressorFlag As integer Implements IM6.SmartElecAndPneumaticsCompressorFlag
        Get
         Return VC2
        End Get
    End Property
   'OUT3
   Public ReadOnly Property SmartElecAndPneumaticAltPowerGenAtCrank As Single Implements IM6.SmartElecAndPneumaticAltPowerGenAtCrank
        Get
        'Multiply * 1 @ Engineering Request
         Return Max1 * -1
        End Get
    End Property
   'OUT4
   Public ReadOnly Property SmartElecAndPneumaticAirCompPowerGenAtCrank As Single Implements IM6.SmartElecAndPneumaticAirCompPowerGenAtCrank
        Get
        Return Sum16
        End Get
End Property
   'OUT5
   Public ReadOnly Property SmartElecOnlyAltPowerGenAtCrank As Single Implements IM6.SmartElecOnlyAltPowerGenAtCrank
        Get
         'Multiply * -1 @ Engineering request.
         Return Max2 * -1
        End Get
    End Property
   'OUT6
   Public ReadOnly Property AveragePowerDemandAtCrankFromPneumatics As Single Implements IM6.AveragePowerDemandAtCrankFromPneumatics
        Get
         Return _m3.GetAveragePowerDemandAtCrankFromPneumatics
        End Get
    End Property
   'OUT7
   Public ReadOnly Property SmartPneumaticOnlyAirCompPowerGenAtCrank As Single Implements IM6.SmartPneumaticOnlyAirCompPowerGenAtCrank
        Get
         Return Sum19
        End Get
    End Property
   'OUT8
   Public ReadOnly Property AvgPowerDemandAtCrankFromElectricsIncHVAC As Single Implements IM6.AvgPowerDemandAtCrankFromElectricsIncHVAC
        Get
 
          Return Sum1

        End Get

    End Property
   'OUT9
   Public ReadOnly Property SmartPneumaticsOnlyCompressorFlag As Integer Implements IM6.SmartPneumaticsOnlyCompressorFlag
        Get
         Return VC4
        End Get
    End Property

   'Internal Staging Calculations
   'switches
   Private ReadOnly Property SW1 As Single
    Get

    Return If( _signals.SmartElectrics, _m5.AlternatorsGenerationPowerAtCrankTractionOnWatts,Sum1)

    End Get
End Property
   
   'Max of Sum5 vs Sum10
   Public ReadOnly Property Max1 As Single
    Get
       Return If( Sum5 > Sum10, Sum5, Sum10) 
    End Get
End Property
   'Max of Sum10 vs Sum7
   Public ReadOnly Property Max2 As Single
    Get
        Return If( Sum7 > Sum10, Sum7, Sum10) 
    End Get
End Property
   
   'Value Choices
   Public ReadOnly Property VC0 As Single
    Get
    Return  If( Sum3<=0,1,0)
    End Get
End Property
   Public ReadOnly Property VC1 As Single
    Get
     Return If( Sum12>0,1,0)
    End Get
End Property
   Public ReadOnly Property VC2 As Single
    Get
    Return If( Sum12<0 orelse Sum12=0,1,0)
    End Get
End Property
   Public ReadOnly Property VC3 As Single
    Get
    Return If( Sum13>0,1,0)
    End Get
End Property
   Public ReadOnly Property VC4 As Single
    Get
    Return If( Sum13<0 orelse Sum13=0, 1,0)
    End Get
End Property

   'Sums
   Public ReadOnly Property Sum1 As Single
    Get
      Return  _m1.AveragePowerDemandAtCrankFromHVACElectricsWatts() + _m2.GetAveragePowerAtCrankFromElectrics()
    End Get
End Property
   Public ReadOnly Property Sum2 As Single
    Get


    Dim returnValue As Single = _signals.PreExistingAuxPower +_m1.AveragePowerDemandAtCrankFromHVACMechanicalsWatts + SW1 + _m3.GetAveragePowerDemandAtCrankFromPneumatics

    Return  returnValue

           

    End Get
End Property
   Public ReadOnly Property Sum3 As Single
    Get

    Return _signals.EngineMotoringPower  + _
           _signals.EngineDrivelinePower + _
           Sum2 

    End Get
End Property
   Public ReadOnly Property Sum4 As Single
    Get

      Return Sum3 - SW1 - _m3.GetAveragePowerDemandAtCrankFromPneumatics + _m4.GetPowerCompressorOff

    End Get
End Property
   Public ReadOnly Property Sum5 As Single
    Get
      Return OverrunFlag * Sum4
    End Get
End Property
   Public ReadOnly Property Sum6 As Single 
    Get
     Return Sum4  -  _m4.GetPowerCompressorOff + _m3.GetAveragePowerDemandAtCrankFromPneumatics
    End Get
End Property
   Public ReadOnly Property Sum7 As Single
    Get
     Return VC0 * Sum6
    End Get
End Property
   Public ReadOnly Property Sum8 As Single
    Get
    Return  Sum4 + SW1
    End Get
End Property
   Public ReadOnly Property Sum9 As Single
    Get
    Return VC0 * Sum8
    End Get
End Property
   Public ReadOnly Property Sum10 As Single
    Get
    Return -1 * _m5.AlternatorsGenerationPowerAtCrankOverrunWatts
    End Get
End Property
   Public ReadOnly Property Sum11 As Single
    Get
      Return Sum5 - Max1
    End Get
End Property
   Public ReadOnly Property Sum12 As Single
    Get
    Return _m4.GetPowerDifference + Sum11
    End Get
End Property
   Public ReadOnly Property Sum13 As Single
    Get
     Return Sum9 + _m4.GetPowerDifference
    End Get
End Property
   Public ReadOnly Property Sum14 As Single
    Get
    Return VC1 * _m4.GetPowerCompressorOff
    End Get
End Property
   Public ReadOnly Property Sum15 As Single
    Get
    Return VC2 * _m4.GetPowerCompressorOn
    End Get
End Property
   Public ReadOnly property Sum16 as single
    Get
      Return Sum14 + Sum15
    End Get
End Property
   Public ReadOnly Property Sum17 As Single
    Get
    Return _m4.GetPowerCompressorOff * VC3
    End Get
End Property
   Public ReadOnly Property Sum18 As Single
    Get
    Return VC4 * _m4.GetPowerCompressorOn
    End Get
End Property
   Public ReadOnly Property Sum19 As Single
    Get
    Return Sum17 + Sum18
    end get
End Property


   'Constructor
   Public Sub new ( m1 As IM1_AverageHVACLoadDemand,
                  m2 As IM2_AverageElectricalLoadDemand,
                  m3 As IM3_AveragePneumaticLoadDemand,
                  m4 As IM4_AirCompressor,
                  m5 As IM5_SmartAlternatorSetGeneration ,
                  signals As ISignals )

      _m1=m1
      _m2=m2
      _m3=m3
      _m4=m4
      _m5=m5
      _signals= signals
                  
 End Sub
   

End Class

End Namespace



