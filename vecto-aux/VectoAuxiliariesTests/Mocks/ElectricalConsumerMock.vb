Imports VectoAuxiliaries.Electrics
Imports System.ComponentModel

Namespace Mocks

    Public Class ElectricalConsumerMock
        Implements IElectricalConsumer

        Public Property AvgConsumptionAmps As Single Implements IElectricalConsumer.AvgConsumptionAmps

        Public Property BaseVehicle As Boolean Implements IElectricalConsumer.BaseVehicle

        Public Property Category As String Implements IElectricalConsumer.Category

        Public Property ConsumerName As String Implements IElectricalConsumer.ConsumerName

        Public Property NominalConsumptionAmps As Single Implements IElectricalConsumer.NominalConsumptionAmps

        Public Property NumberInActualVehicle As Integer Implements IElectricalConsumer.NumberInActualVehicle

        Public Property PhaseIdle_TractionOn As Single Implements IElectricalConsumer.PhaseIdle_TractionOn

        Public Property PowerNetVoltage As Single Implements IElectricalConsumer.PowerNetVoltage

        Public Function TotalAvgConumptionAmps(Optional PhaseIdle_TractionOnBasedOnCycle As Single = 0.0) As Single Implements IElectricalConsumer.TotalAvgConumptionAmps
            Return 9
        End Function


        Public Function TotalAvgConsumptionInWatts(Optional PhaseIdle_TractionOnBasedOnCycle As Single = 0.0) As Single Implements IElectricalConsumer.TotalAvgConsumptionInWatts

            Return 9 * 26.3

        End Function


        Public Property Info As String Implements IElectricalConsumer.Info

        Public Event PropertyChanged As PropertyChangedEventHandler _
            Implements INotifyPropertyChanged.PropertyChanged

        Private Sub NotifyPropertyChanged(p As String)
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(p))
        End Sub

    End Class

End Namespace
