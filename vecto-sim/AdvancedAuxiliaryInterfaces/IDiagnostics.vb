
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

Public Interface IDiagnostics



Property AA_NonSmartAlternatorsEfficiency As single
Property AA_SmartIdleCurrent_Amps As single
Property AA_SmartIdleAlternatorsEfficiency As single
Property AA_SmartTractionCurrent_Amps As single
Property AA_SmartTractionAlternatorEfficiency As single
Property AA_SmartOverrunCurrent_Amps As Single
Property AA_SmartOverrunAlternatorEfficiency As Single
Property AA_CompressorFlowRate_LitrePerSec As Single
Property AA_OverrunFlag As integer
Property AA_EngineIdleFlag As integer
Property AA_CompressorFlag As integer
Property AA_TotalCycleFC_BeforeSSandWHTCcorrection_Grams As single
Property AA_TotalCycleFC_BeforeSSandWHTCcorrection_Litres As single



End Interface
