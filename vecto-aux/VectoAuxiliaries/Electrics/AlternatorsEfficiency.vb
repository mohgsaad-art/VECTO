
Imports VectoAuxiliaries.Hvac
Namespace Electrics


Public Class AlternatorsEfficiency

 Private _electricalConsumersList As IElectricalConsumerList
 Private _hvacInputs As IHVACInputs

 Private _alternatorEfficiencyMap As IAlternatorMap

 Private _hvacMap As IHVACMap

 Private _powernetVoltage As Single


    Public Sub New(electricalConsumers As IElectricalConsumerList, hvacInputs As IHVACInputs, hvacMap As IHVACMap, alternatorEfficiencyMap As IAlternatorMap, powernetVoltage As Single)

    If electricalConsumers Is Nothing Then Throw New ArgumentException("No ElectricalConsumersList Supplied")
    If hvacInputs Is Nothing Then Throw New ArgumentException("No hvac inputs supplied")
    If hvacMap Is Nothing Then Throw New ArgumentException("No HVAC Map supplied")
    If alternatorEfficiencyMap Is Nothing Then Throw New ArgumentException("No Alternator Efficiency Map Supplied")
    If (powernetVoltage < ElectricConstants.PowenetVoltageMin Or powernetVoltage > ElectricConstants.PowenetVoltageMax) Then Throw New ArgumentException("Powernet Voltage out of range")

    Me._electricalConsumersList = electricalConsumers
    Me._hvacInputs = hvacInputs

    Me._alternatorEfficiencyMap = alternatorEfficiencyMap
    Me._hvacMap = hvacMap
    Me._powernetVoltage = powernetVoltage

    End Sub

    Public Function GetEfficiency(crankRPM As Integer, DoorCycleActuationPercentage As Single) As Single

          'Sanity Check.
          If crankRPM < 1 Then Throw New ArgumentException("CrankRMP must be greater than zero")
          If DoorCycleActuationPercentage < 0 Or DoorCycleActuationPercentage > 1 Then Throw New ArgumentException("DoorCyclyActuationPercentage must be between 0 and 1")

          Dim rotationalSpeed As Single = crankRPM
          Dim currentHVACDemandAmps As Single = _hvacMap.GetElectricalDemand(_hvacInputs.Region, _hvacInputs.Season)
          Dim currentElectricalConsumerDemandAmp As Single = _electricalConsumersList.GetTotalAverageDemandAmps(DoorCycleActuationPercentage, True)

          Dim totalDemandAmps As Single = currentHVACDemandAmps + currentElectricalConsumerDemandAmp

          Return _alternatorEfficiencyMap.GetEfficiency(crankRPM, totalDemandAmps).Efficiency

    End Function

   Public Function GetHVACElectricalPowerDemandAmps() As Single

        Return _hvacMap.GetElectricalDemand(_hvacInputs.Region, _hvacInputs.Season) / _powernetVoltage

  End Function


End Class


End Namespace


