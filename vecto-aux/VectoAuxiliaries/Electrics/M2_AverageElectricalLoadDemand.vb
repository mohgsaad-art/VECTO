Namespace Electrics

    Public Class M2_AverageElectricalLoadDemand

        Public _powerNetVoltage As Single = 26.3
        Private _electricalConsumers As IElectricalConsumerList
        Private _module0 As IM0_NonSmart_AlternatorsSetEfficiency
        Private _alternatorPulleyEffiency As Single



        Public Sub New(ByVal electricalConsumers As IElectricalConsumerList, m0 As IM0_NonSmart_AlternatorsSetEfficiency, altPulleyEfficiency As Single, powerNetVoltage As Single)

        If electricalConsumers Is Nothing Then Throw New ArgumentException("Electrical Consumer List must be supplied")
        If m0 Is Nothing Then Throw New ArgumentException("Must supply module 0")
        If altPulleyEfficiency = 0 OrElse altPulleyEfficiency > 1 Then Throw New ArgumentException("Alternator Gear efficiency out of range.")
        If powerNetVoltage < ElectricConstants.PowenetVoltageMin OrElse powerNetVoltage > ElectricConstants.PowenetVoltageMax Then
        Throw New ArgumentException("Powernet Voltage out of known range.")
        End If


            _powerNetVoltage = powerNetVoltage
            _electricalConsumers = electricalConsumers
            _module0 = m0
            _alternatorPulleyEffiency = altPulleyEfficiency


        End Sub



        Public Function GetAveragePowerDemandAtAlternator() As Single

             Return _electricalConsumers.GetTotalAverageDemandAmps(False)

        End Function


        Public Function GetAveragePowerAtCrank(ByVal engineRpm As Integer, doorDutyCycleZeroToOne As Single) As Single

            Dim ElectricalPowerDemandsWatts As Single = GetAveragePowerDemandAtAlternator() * _powerNetVoltage
            Dim alternatorsEfficiency As Single       = _module0.GetEfficiency(engineRpm)
            Dim ElectricalPowerDemandsWattsDividedByAlternatorEfficiency as Single = ElectricalPowerDemandsWatts / alternatorsEfficiency

            Dim averagePowerDemandAtCrankFromElectricsWatts As Single
           
            averagePowerDemandAtCrankFromElectricsWatts = ElectricalPowerDemandsWattsDividedByAlternatorEfficiency / _alternatorPulleyEffiency

            Return  averagePowerDemandAtCrankFromElectricsWatts

        End Function


    End Class
End Namespace