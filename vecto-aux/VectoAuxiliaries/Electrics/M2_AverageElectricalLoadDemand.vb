' Copyright 2015 European Union.
' Licensed under the EUPL (the 'Licence');
'
' * You may not use this work except in compliance with the Licence.
' * You may obtain a copy of the Licence at: http://ec.europa.eu/idabc/eupl
' * Unless required by applicable law or agreed to in writing,
'   software distributed under the Licence is distributed on an "AS IS" basis,
'   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
'
' See the LICENSE.txt for the specific language governing permissions and limitations.

Namespace Electrics

    Public Class M2_AverageElectricalLoadDemand
    Implements IM2_AverageElectricalLoadDemand

        Public _powerNetVoltage As Single = 26.3
        Private _electricalConsumers As IElectricalConsumerList
        Private _module0 As IM0_NonSmart_AlternatorsSetEfficiency
        Private _alternatorPulleyEffiency As Single
        Private _signals As Signals


        'Constructor
        Public Sub New(ByVal electricalConsumers As IElectricalConsumerList, m0 As IM0_NonSmart_AlternatorsSetEfficiency, altPulleyEfficiency As Single, powerNetVoltage As Single, signals as ISignals )

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

        'Public class outputs (Properties)
        Public Function GetAveragePowerDemandAtAlternator() As Single Implements IM2_AverageElectricalLoadDemand.GetAveragePowerDemandAtAlternator

             Return ( _electricalConsumers.GetTotalAverageDemandAmps(False)/ElectricConstants.BatteryEfficiency) * _powerNetVoltage

        End Function
        Public Function GetAveragePowerAtCrankFromElectrics() As Single Implements IM2_AverageElectricalLoadDemand.GetAveragePowerAtCrankFromElectrics

            Dim ElectricalPowerDemandsWatts As Single = GetAveragePowerDemandAtAlternator()
            Dim alternatorsEfficiency As Single       = _module0.AlternatorsEfficiency
            Dim ElectricalPowerDemandsWattsDividedByAlternatorEfficiency as Single = ElectricalPowerDemandsWatts / alternatorsEfficiency

            Dim averagePowerDemandAtCrankFromElectricsWatts As Single
           
            averagePowerDemandAtCrankFromElectricsWatts = ElectricalPowerDemandsWattsDividedByAlternatorEfficiency / _alternatorPulleyEffiency

            Return  averagePowerDemandAtCrankFromElectricsWatts

        End Function


    End Class

End Namespace