
Imports VectoAuxiliaries.Hvac
Namespace Electrics


Public Class M0_NonSmart_AlternatorsSetEfficiency
 Implements IM0_NonSmart_AlternatorsSetEfficiency

 Private _electricalConsumersList As IElectricalConsumerList
 Private _hvacInputs As IHVACInputs
 Private _alternatorEfficiencyMap As IAlternatorMap
 Private _hvacMap As IHVACMap
 Private _powernetVoltage As Single
 Private _signals As ISignals


    Public Sub New(electricalConsumers As IElectricalConsumerList, hvacInputs As IHVACInputs, hvacMap As IHVACMap, alternatorEfficiencyMap As IAlternatorMap, powernetVoltage As Single, signals As ISignals)

    If electricalConsumers Is Nothing Then Throw New ArgumentException("No ElectricalConsumersList Supplied")
    If hvacInputs Is Nothing Then Throw New ArgumentException("No hvac inputs supplied")
    If hvacMap Is Nothing Then Throw New ArgumentException("No HVAC Map supplied")
    If alternatorEfficiencyMap Is Nothing Then Throw New ArgumentException("No Alternator Efficiency Map Supplied")
    If (powernetVoltage < ElectricConstants.PowenetVoltageMin Or powernetVoltage > ElectricConstants.PowenetVoltageMax) Then Throw New ArgumentException("Powernet Voltage out of range")
    If signals is Nothing then Throw New ArgumentException("No Signals reference was supplied.")

    Me._electricalConsumersList = electricalConsumers
    Me._hvacInputs = hvacInputs

    Me._alternatorEfficiencyMap = alternatorEfficiencyMap
    Me._hvacMap = hvacMap
    Me._powernetVoltage = powernetVoltage
    Me._signals = signals

    End Sub



    Public Function GetEfficiency() As Single Implements IM0_NonSmart_AlternatorsSetEfficiency.GetEfficiency

          'Sanity Check.
          If _signals.EngineSpeed < 1 Then Throw New ArgumentException("CrankRPM must be greater than zero")

          Dim rotationalSpeed As Single = _signals.EngineSpeed
          Dim currentHVACDemandAmps As Single = _hvacMap.GetElectricalDemand(_hvacInputs.Region, _hvacInputs.Season)
          Dim currentElectricalConsumerDemandAmp As Single = _electricalConsumersList.GetTotalAverageDemandAmps(True)

          Dim totalDemandAmps As Single = currentHVACDemandAmps + currentElectricalConsumerDemandAmp

          Return _alternatorEfficiencyMap.GetEfficiency(_signals.EngineSpeed, totalDemandAmps).Efficiency

    End Function

    Public Function GetHVACElectricalPowerDemandAmps() As Single Implements IM0_NonSmart_AlternatorsSetEfficiency.GetHVACElectricalPowerDemandAmps

        Return _hvacMap.GetElectricalDemand(_hvacInputs.Region, _hvacInputs.Season) / _powernetVoltage

  End Function


End Class


End Namespace


