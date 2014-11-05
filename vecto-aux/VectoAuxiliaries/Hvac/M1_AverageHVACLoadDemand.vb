Imports VectoAuxiliaries.Electrics
Imports VectoAuxiliaries.Pneumatics

Namespace Hvac

    Public Class M1_AverageHVACLoadDemand
    Implements IM1_AverageHVACLoadDemand


    Private _m0 As IM0_NonSmart_AlternatorsSetEfficiency
    Private _alternatorGearEfficiency As Single
    Private _compressorGearEfficiency As Single
    Private _hvacInputs As IHVACInputs
    Private _hvacMap As IHVACMap
    Private _signals As ISignals
    Private _powernetVoltage As Single
    Private _steadyStateModel As IHVACSteadyStateModel


    Public Sub New(m0 As IM0_NonSmart_AlternatorsSetEfficiency, hvacMap As IHVACMap, hvacInputs As IHVACInputs, altGearEfficiency As Single, compressorGearEfficiency As Single, powernetVoltage As Single, signals As ISignals, ssm As IHVACSteadyStateModel)

          'Sanity Check - Illegal operations without all params.
          If m0 Is Nothing Then Throw New ArgumentException("Module0 as supplied is null")
          If hvacMap Is Nothing Then Throw New ArgumentException("hvacMap as supplied is null")
          If hvacInputs Is Nothing Then Throw New ArgumentException("hvacInputs as supplied is null")
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
          _hvacMap = hvacMap
          _hvacInputs = hvacInputs
          _alternatorGearEfficiency = altGearEfficiency
          _signals = signals
          _steadyStateModel = ssm
          _compressorGearEfficiency = compressorGearEfficiency
          _powernetVoltage = powernetVoltage



    End Sub


        Public Function AveragePowerDemandAtCrankFromHVACMechanicalsWatts() As Single Implements IM1_AverageHVACLoadDemand.AveragePowerDemandAtCrankFromHVACMechanicalsWatts

            Return _steadyStateModel.HVACMechanicalLoadPowerWatts / _compressorGearEfficiency


        End Function

       Public Function AveragePowerDemandAtAlternatorFromHVACElectricsWatts() As Single Implements IM1_AverageHVACLoadDemand.AveragePowerDemandAtAlternatorFromHVACElectricsWatts

       Return _steadyStateModel.HVACElectricalLoadPowerWatts

       End Function

        Public Function AveragePowerDemandAtCrankFromHVACElectricsWatts() As Single Implements IM1_AverageHVACLoadDemand.AveragePowerDemandAtCrankFromHVACElectricsWatts


       Return _steadyStateModel.HVACElectricalLoadPowerWatts / _m0.AlternatorsEfficiency()

        End Function


        Public Function HVACFuelingLitresPerHour() As Single Implements IM1_AverageHVACLoadDemand.HVACFuelingLitresPerHour


            Return _steadyStateModel.HVACFuellingLitresPerHour


        End Function


    End Class





End Namespace