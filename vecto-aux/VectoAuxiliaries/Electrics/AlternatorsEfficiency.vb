
Imports VectoAuxiliaries.Hvac
Namespace Electrics


Public Class AlternatorsEfficiency

 Private _electricalConsumersList As IElectricalConsumerList
 Private _hvacInputs As IHVACInputs
 Private _pulleyGearRatio As Single
 Private _alternatorEfficiencyMap As IAlternatorMap

 Private _hvacMap As IHVACMap


    Public Sub New(electricalConsumers As IElectricalConsumerList, hvacInputs As IHVACInputs, hvacMap As IHVACMap, pullyGearRatio As Single, alternatorEfficiencyMap As IAlternatorMap)


    If electricalConsumers Is Nothing Then Throw New ArgumentException("No ElectricalConsumersList Supplied")
    If hvacInputs Is Nothing Then Throw New ArgumentException("No hvac inputs supplied")
    If hvacMap Is Nothing Then Throw New ArgumentException("No HVAC Map supplied")
    If pullyGearRatio < 0.6 Or pullyGearRatio > 1 Then Throw New ArgumentException("Pully Gear Efficiency must be between 0.6 and 1")
    If alternatorEfficiencyMap Is Nothing Then Throw New ArgumentException("No Alternator Efficiency Map Supplied")

    Me._electricalConsumersList = electricalConsumers
    Me._hvacInputs = hvacInputs
    Me._pulleyGearRatio = pullyGearRatio
    Me._alternatorEfficiencyMap = alternatorEfficiencyMap
    Me._hvacMap = hvacMap

    End Sub


    Public Function GetEfficiency(crankRPM As Integer, CycleActuationPercentage As Single) As Single

          'Sanity Check.
          If crankRPM < 1 Then Throw New ArgumentException("CrankRMP must be greater than zero")
          If CycleActuationPercentage < 0 Or CycleActuationPercentage > 1 Then Throw New ArgumentException("CyclyActuationPercentage must be between 0 and 1")

          Dim rotationalSpeed As Single = crankRPM * _pulleyGearRatio
          Dim currentHVACDemandAmps As Single = _hvacMap.GetElectricalDemand(_hvacInputs.Region, _hvacInputs.Season)
          Dim currentElectricalConsumerDemandAmp As Single = _electricalConsumersList.GetTotalAverageDemandAmps(CycleActuationPercentage)

          Dim totalDemandAmps As Single = currentHVACDemandAmps + currentElectricalConsumerDemandAmp

          Return 0 ' TODO: FIX THIS._alternatorEfficiencyMap.

    End Function


End Class


End Namespace


