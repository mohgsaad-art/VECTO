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

Namespace Hvac

    Public Class M1_AverageHVACLoadDemand
      Implements IM1_AverageHVACLoadDemand

      
       Private _m0 As IM0_NonSmart_AlternatorsSetEfficiency
       Private _alternatorGearEfficiency As Single
       Private _compressorGearEfficiency As Single    
       Private _signals As ISignals
       Private _powernetVoltage As Single
       Private _steadyStateModel As ISSMTOOL

       Private _ElectricalPowerW As Single
       Private _MechanicalPowerW As Single
       Private _FuelingLPerH As Single
       

      
       'Constructor
       Public Sub New(m0 As IM0_NonSmart_AlternatorsSetEfficiency, altGearEfficiency As Single, compressorGearEfficiency As Single, powernetVoltage As Single, signals As ISignals, ssm As ISSMTOOL)

          'Sanity Check - Illegal operations without all params.
          If m0 Is Nothing Then Throw New ArgumentException("Module0 as supplied is null")

          If altGearEfficiency < ElectricConstants.AlternatorPulleyEfficiencyMin OrElse altGearEfficiency > ElectricConstants.AlternatorPulleyEfficiencyMax Then _
              Throw New ArgumentException(String.Format("Gear efficiency must be between {0} and {1}", ElectricConstants.AlternatorPulleyEfficiencyMin, ElectricConstants.AlternatorPulleyEfficiencyMax))

          If signals Is Nothing Then Throw New Exception("Signals object as supplied is null")
          If powernetVoltage < ElectricConstants.PowenetVoltageMin OrElse powernetVoltage > ElectricConstants.PowenetVoltageMax Then _
          Throw New ArgumentException(String.Format("PowenetVoltage supplied must be in the range {0} to {1}", ElectricConstants.PowenetVoltageMin, ElectricConstants.PowenetVoltageMax))
          If ssm Is Nothing Then Throw New ArgumentException("Steady State model was not supplied")
          If compressorGearEfficiency < 0 OrElse altGearEfficiency > 1 Then _
              Throw New ArgumentException(String.Format("Compressor Gear efficiency must be between {0} and {1}", 0, 1))


          'Assign
          _m0 = m0
          _alternatorGearEfficiency = altGearEfficiency
          _signals = signals
        
          _compressorGearEfficiency = compressorGearEfficiency
          _powernetVoltage = powernetVoltage


          _steadyStateModel = ssm

          _ElectricalPowerW = ssm.ElectricalWAdjusted
          _MechanicalPowerW = ssm.MechanicalWBaseAdjusted
          _FuelingLPerH     = ssm.FuelPerHBaseAdjusted

    End Sub  
       
       'Public Methods - Implementation
       Public Function AveragePowerDemandAtCrankFromHVACMechanicalsWatts() As Single Implements IM1_AverageHVACLoadDemand.AveragePowerDemandAtCrankFromHVACMechanicalsWatts

            Return _MechanicalPowerW/ _compressorGearEfficiency

        End Function      
       Public Function AveragePowerDemandAtAlternatorFromHVACElectricsWatts() As Single Implements IM1_AverageHVACLoadDemand.AveragePowerDemandAtAlternatorFromHVACElectricsWatts

             Return _ElectricalPowerW

       End Function     
       Public Function AveragePowerDemandAtCrankFromHVACElectricsWatts() As Single Implements IM1_AverageHVACLoadDemand.AveragePowerDemandAtCrankFromHVACElectricsWatts

            Return _ElectricalPowerW / _m0.AlternatorsEfficiency() / _alternatorGearEfficiency

        End Function  
       Public Function HVACFuelingLitresPerHour() As Single Implements IM1_AverageHVACLoadDemand.HVACFuelingLitresPerHour

            Return _FuelingLPerH

        End Function
      

    End Class

End Namespace