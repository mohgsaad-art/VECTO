Namespace Electrics
    Public Interface IElectricalConsumer


         Property Category As String
         Property ConsumerName As String
         Property BaseVehicle As Boolean
         Property NominalConsumptionAmps As Single
         Property PhaseIdle_TractionOn As Single
         Property NumberInActualVehicle As Integer
         Property PowerNetVoltage As Single

         Property AvgConsumptionAmps As Single

         Function TotalAvgConumptionAmps(Optional PhaseIdle_TractionOnBasedOnCycle As Single = Nothing) As Single

         Function TotalAvgConsumptionInWatts(Optional PhaseIdle_TractionOnBasedOnCycle As Single = 0.0) As Single

    End Interface
End Namespace