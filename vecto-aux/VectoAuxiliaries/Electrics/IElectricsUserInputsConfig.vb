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

Imports VectoAuxiliaries.Electrics

Namespace Electrics

Public Interface IElectricsUserInputsConfig

Property PowerNetVoltage As Single
Property AlternatorMap As String
Property AlternatorGearEfficiency As Single
Property ElectricalConsumers As IElectricalConsumerList
Property DoorActuationTimeSecond As Integer

Property ResultCardIdle As IResultCard
Property ResultCardTraction As IResultCard
Property ResultCardOverrun As IResultCard
Property SmartElectrical As Boolean

End Interface


End Namespace


