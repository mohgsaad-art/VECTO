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

Public Class ElectricsUserInputsConfig
Implements IElectricsUserInputsConfig


Public Property PowerNetVoltage As Single Implements IElectricsUserInputsConfig.PowerNetVoltage
Public Property AlternatorMap As String Implements IElectricsUserInputsConfig.AlternatorMap
Public Property AlternatorGearEfficiency As Single Implements IElectricsUserInputsConfig.AlternatorGearEfficiency
Public Property ElectricalConsumers As IElectricalConsumerList Implements IElectricsUserInputsConfig.ElectricalConsumers
Public Property DoorActuationTimeSecond As Integer Implements IElectricsUserInputsConfig.DoorActuationTimeSecond


Public Property ResultCardIdle As IResultCard Implements IElectricsUserInputsConfig.ResultCardIdle
Public Property ResultCardTraction As IResultCard Implements IElectricsUserInputsConfig.ResultCardTraction
Public Property ResultCardOverrun As IResultCard Implements IElectricsUserInputsConfig.ResultCardOverrun

Public Property SmartElectrical As Boolean Implements IElectricsUserInputsConfig.SmartElectrical


End Class

End Namespace



