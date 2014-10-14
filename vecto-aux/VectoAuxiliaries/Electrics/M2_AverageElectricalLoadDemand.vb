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



        ''' <summary>
        ''' Gets the total average power at the alternator for all electrical consumers
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetAveragePowerDemandAtAlternator() As Single

           '  Return _electricalConsumers.GetTotalAverageDemandAmps()

            Return 5000 ' TODO:FIX THIS
        End Function


        Public Function GetAveragePowerAtCrank(ByVal engineRpm As Integer) As Single
            Dim elecPower As Single = GetAveragePowerDemandAtAlternator()
            Dim alternatorEfficiency As Single = 0 '_alternator.GetEfficiency(engineRpm) TODO: Fix THis.
            Dim demandFromAlternator As Single = elecPower / alternatorEfficiency
            Dim powerAtCrank As Single = 0 ' TODO : FIX THIS demandFromAlternator / _alternator.PulleyGearEfficiency
            Return powerAtCrank
        End Function


    End Class
End Namespace