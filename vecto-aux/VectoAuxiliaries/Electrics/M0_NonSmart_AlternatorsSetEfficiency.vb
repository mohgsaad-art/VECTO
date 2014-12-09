
Imports VectoAuxiliaries.Hvac
Namespace Electrics


Public Class M0_NonSmart_AlternatorsSetEfficiency
 Implements IM0_NonSmart_AlternatorsSetEfficiency



 Private _electricalConsumersList As IElectricalConsumerList
' Private _hvacInputs As IHVACInputs
 Private _alternatorEfficiencyMap As IAlternatorMap

 Private _powernetVoltage As Single
 Private _signals As ISignals
 Private _steadyStateModelHVAC As IHVACSteadyStateModel


    Public Sub New(electricalConsumers As IElectricalConsumerList,  alternatorEfficiencyMap As IAlternatorMap, powernetVoltage As Single, signals As ISignals, ssmHvac As IHVACSteadyStateModel)

    If electricalConsumers Is Nothing Then Throw New ArgumentException("No ElectricalConsumersList Supplied")
    'If hvacInputs Is Nothing Then Throw New ArgumentException("No hvac inputs supplied")

    If alternatorEfficiencyMap Is Nothing Then Throw New ArgumentException("No Alternator Efficiency Map Supplied")
    If (powernetVoltage < ElectricConstants.PowenetVoltageMin Or powernetVoltage > ElectricConstants.PowenetVoltageMax) Then Throw New ArgumentException("Powernet Voltage out of range")
    If signals is Nothing then Throw New ArgumentException("No Signals reference was supplied.")

    Me._electricalConsumersList = electricalConsumers
    'Me._hvacInputs = hvacInputs

    Me._alternatorEfficiencyMap = alternatorEfficiencyMap
    Me._powernetVoltage = powernetVoltage
    Me._signals = signals
    Me._steadyStateModelHVAC = ssmHvac


    End Sub


    Public ReadOnly Property  AlternatorsEfficiency As Single Implements IM0_NonSmart_AlternatorsSetEfficiency.AlternatorsEfficiency

    Get
          'Sanity Check.
          If _signals.EngineSpeed < 1 Then Return 0

          Dim baseCurrentDemandAmps As Single = _electricalConsumersList.GetTotalAverageDemandAmps(True)

          Dim totalDemandAmps As Single = baseCurrentDemandAmps + GetHVACElectricalPowerDemandAmps

          Return _alternatorEfficiencyMap.GetEfficiency(_signals.EngineSpeed, totalDemandAmps).Efficiency
    End Get


    end property


    Public readonly property GetHVACElectricalPowerDemandAmps As Single Implements IM0_NonSmart_AlternatorsSetEfficiency.GetHVACElectricalPowerDemandAmps
        Get
          Return _steadyStateModelHVAC.HVACElectricalLoadPowerWatts / _powernetVoltage
        End Get
    End Property

 






End Class


End Namespace


