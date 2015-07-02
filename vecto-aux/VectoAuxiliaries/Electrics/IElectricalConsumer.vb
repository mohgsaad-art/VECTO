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

Imports System.ComponentModel

Namespace Electrics

    Public Interface IElectricalConsumer
        Inherits INotifyPropertyChanged

        Property Category As String
        Property ConsumerName As String
        Property BaseVehicle As Boolean
        Property NominalConsumptionAmps As Single
        Property PhaseIdle_TractionOn As Single
        Property NumberInActualVehicle As Integer
        Property PowerNetVoltage As Single
        Property AvgConsumptionAmps As Single
        Property Info As String
        Function TotalAvgConumptionAmps(Optional PhaseIdle_TractionOnBasedOnCycle As Single = Nothing) As Single
        Function TotalAvgConsumptionInWatts(Optional PhaseIdle_TractionOnBasedOnCycle As Single = 0.0) As Single

    End Interface

End Namespace