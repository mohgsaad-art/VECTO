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

Public Interface ISignals

Property PreExistingAuxPower As single
Property EngineMotoringPower As single
property EngineDrivelinePower as single
property SmartElectrics As Boolean
Property ClutchEngaged As Boolean
Property EngineSpeed as integer
Property SmartPneumatics As Boolean
Property TotalCycleTimeSeconds As Integer
Property CurrentCycleTimeInSeconds As Integer
property EngineDrivelineTorque as single
Property Idle As Boolean
Property InNeutral As Boolean
Property AuxiliaryEventReportingLevel As AdvancedAuxiliaryMessageType
Property EngineStopped As Boolean
Property WHTC As Single
Property DeclarationMode As Boolean


End Interface
