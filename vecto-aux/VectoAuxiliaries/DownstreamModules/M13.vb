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

   Public Class M13
      Implements IM13

     Private Const FUEL_DENSITY_L3 As Single = 835
      
     Private m1  As IM1_AverageHVACLoadDemand
     Private m10 As IM10
     Private m12 As IM12
     Private signals As ISignals
     
     'Internal Staging Calculations
     Private readonly Property Sum1 As Single
    Get
     return  - m12.FuelconsumptionwithsmartElectricsandAveragePneumaticPowerDemand + m12.BaseFuelConsumptionWithAverageAuxiliaryLoads
    End Get
End Property
     Private readonly Property Sum2 As Single
    Get
     Return m12.BaseFuelConsumptionWithAverageAuxiliaryLoads - m10.FuelConsumptionSmartPneumaticsAndAverageElectricalPowerDemand
    End Get
End Property
     Private readonly Property Sum3 As Single
    Get
     Return m12.BaseFuelConsumptionWithAverageAuxiliaryLoads-Sum2
    End Get
End Property
     Private readonly Property Sum4 As Single
    Get
     Return -Sum1+Sum3
    End Get
End Property

    'Sums 5, 6 and 7 removed during V06 implementation of the model
     Private ReadOnly Property Sum8 As Single
      Get
      Return SW4 * SW3
       End Get
     End Property

     'Internal Staging Switches
     Private readonly Property SW1 As Single
    Get
      Return If( signals.SmartPneumatics,Sum4,m12.FuelconsumptionwithsmartElectricsandAveragePneumaticPowerDemand)
    End Get
End Property
     private readonly Property SW2 as Single
    Get
     Return  If( signals.SmartPneumatics,m10.FuelConsumptionSmartPneumaticsAndAverageElectricalPowerDemand, m12.BaseFuelConsumptionWithAverageAuxiliaryLoads)
    End Get
End Property
     Private readonly Property SW3 As Single
    Get
     Return If( signals.SmartElectrics, SW1, SW2)
    End Get
End Property
     Private ReadOnly Property SW4 As single
    Get
      Return If( signals.DeclarationMode, signals.WHTC,1)
    End Get
End Property

     'Constructor
     Public Sub new ( m1 As IM1_AverageHVACLoadDemand, m10 As IM10, m12 As IM12 , signals As ISignals)

      Me.m1      =  m1
      Me.m10     =  m10
      Me.m12     =  m12 
      Me.signals =  signals
                          
End Sub

     'Public class outputs
     Public ReadOnly Property WHTCTotalCycleFuelConsumptionGrams As Single Implements IM13.WHTCTotalCycleFuelConsumptionGrams
            Get
          Return Sum8
            End Get
        End Property


   End Class

End Namespace



