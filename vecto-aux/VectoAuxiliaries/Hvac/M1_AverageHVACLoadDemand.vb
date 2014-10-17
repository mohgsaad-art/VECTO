Imports VectoAuxiliaries.Electrics

Namespace Hvac

    Public Class M1_AverageHVACLoadDemand
    Implements IM1_AverageHVACLoadDemand


    Private _m0 As IM0_NonSmart_AlternatorsSetEfficiency
    Private _alternatorGearEfficiency As Single 
    Private _hvacInputs As IHVACInputs
    Private _hvacMap As IHVACMap
    Private _signals As ISignals
    Private _powernetVoltage As single


    Public Sub new ( m0 As IM0_NonSmart_AlternatorsSetEfficiency, hvacMap As IHVACMap, hvacInputs As IHVACInputs, altGearEfficiency As Single, powernetVoltage As Single, signals As ISignals )

          'Sanity Check - Illegal operations without all params.
          If m0 is Nothing then  Throw New ArgumentException("Module0 as supplied is null")
          If hvacMap is Nothing then  Throw New ArgumentException("hvacMap as supplied is null")
          If hvacInputs is Nothing then  Throw New ArgumentException("hvacInputs as supplied is null")
          If altGearEfficiency< ElectricConstants.AlternatorPulleyEfficiencyMin orelse altGearEfficiency> ElectricConstants.AlternatorPulleyEfficiencyMax then _
              Throw New ArgumentException(String.Format("Gear efficiency must be between {0} and {1}",ElectricConstants.AlternatorPulleyEfficiencyMin,ElectricConstants.AlternatorPulleyEfficiencyMax ))

          If signals is Nothing then Throw New Exception ("Signals object as supplied is null")
          If powernetVoltage< ElectricConstants.PowenetVoltageMin orelse powernetVoltage> ElectricConstants.PowenetVoltageMax then _
          Throw New ArgumentException(String.Format("PowenetVoltage supplied must be in the range {0} to {1}",ElectricConstants.PowenetVoltageMin,ElectricConstants.PowenetVoltageMax))

          'Assign
          _m0=m0
          _hvacMap= hvacMap
          _hvacInputs = hvacInputs
          _alternatorGearEfficiency=altGearEfficiency
          _signals = _Signals
         
    End Sub


       Public Function AveragePowerDemandAtAlternatorFromHVACElectricsWatts() As Single Implements IM1_AverageHVACLoadDemand.AveragePowerDemandAtAlternatorFromHVACElectricsWatts
          Return 100'TODO FIX THIS
       End Function

        Public Function AveragePowerDemandAtCrankFromHVACElectricsWatts() As Single Implements IM1_AverageHVACLoadDemand.AveragePowerDemandAtCrankFromHVACElectricsWatts     
           Return 100'TODO FIX THIS
        End Function

        Public Function AveragePowerDemandAtCrankFromHVACMechanicalsWatts() As Single Implements IM1_AverageHVACLoadDemand.AveragePowerDemandAtCrankFromHVACMechanicalsWatts          
               Return 100'TODO FIX THIS
        End Function

        Public Function HVACFuelingLitresPerHour() As Single Implements IM1_AverageHVACLoadDemand.HVACFuelingLitresPerHour   
               Return 100'TODO FIX THI
        End Function


    End Class





End Namespace