Namespace Electrics

    ''' <summary>
    ''' Described a consumer of Alternator electrical power
    ''' </summary>
    ''' <remarks></remarks>
    Public Class ElectricalConsumer
        Implements IElectricalConsumer

        'Calculated
        Private Property AvgConsumptionAmps As Single Implements IElectricalConsumer.AvgConsumptionAmps

        'User Input
        Public Property BaseVehicle As Boolean Implements IElectricalConsumer.BaseVehicle
        Public Property Category As String Implements IElectricalConsumer.Category
        Public Property ConsumerName As String Implements IElectricalConsumer.ConsumerName
        Public Property NominalConsumptionAmps As Single Implements IElectricalConsumer.NominalConsumptionAmps
        Public Property NumberInActualVehicle As Integer Implements IElectricalConsumer.NumberInActualVehicle
        Public Property PhaseIdle_TractionOn As Single Implements IElectricalConsumer.PhaseIdle_TractionOn
        Public Property PowerNetVoltage As Single Implements IElectricalConsumer.PowerNetVoltage

        Public Function TotalAvgConumptionAmps(Optional PhaseIdle_TractionOnBasedOnCycle As Single = 0.0) As Single Implements IElectricalConsumer.TotalAvgConumptionAmps

        'TODO'

        Throw New NotImplementedException()


        End Function


       Public Sub New(BaseVehicle As Boolean, Category As String, ConsumerName As String, NominalConsumptionAmps As Single, PhaseIdle_TractionOn As Single, PowerNetVoltageas As Single)

            'Illegal Value Check.
            If Category.Trim.Length = 0 Then Throw New ArgumentException("Category Name cannot be empty")
            If ConsumerName.Trim.Length = 0 Then Throw New ArgumentException("ConsumerName Name cannot be empty")
            If PhaseIdle_TractionOn < 0 Or PhaseIdle_TractionOn > 1 Then Throw New ArgumentException("PhaseIdle_TractionOn must have a value between 0 and 1")
            If NominalConsumptionAmps < 0 Or NominalConsumptionAmps > 1 Then Throw New ArgumentException("NominalConsumptionAmps must have a value between 0 and 1")
            If PowerNetVoltage < 6 Or PowerNetVoltage > 48 Then Throw New ArgumentException("PowerNetVoltage must have a value between 6 and 48")

            'Good, now assign.
            Me.BaseVehicle = BaseVehicle
            Me.Category = Category
            Me.ConsumerName = ConsumerName
            Me.NominalConsumptionAmps = NominalConsumptionAmps
            Me.PhaseIdle_TractionOn = PhaseIdle_TractionOn
            Me.PowerNetVoltage = PowerNetVoltage

       End Sub


    End Class
End Namespace