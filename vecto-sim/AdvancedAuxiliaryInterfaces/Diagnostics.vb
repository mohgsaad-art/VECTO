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

Public Class Diagnostics
Implements IDiagnostics


    Public Property AA_CompressorFlag As Integer Implements IDiagnostics.AA_CompressorFlag

    Public Property AA_CompressorFlowRate_LitrePerSec As Single Implements IDiagnostics.AA_CompressorFlowRate_LitrePerSec

    Public Property AA_EngineIdleFlag As Integer Implements IDiagnostics.AA_EngineIdleFlag

    Public Property AA_NonSmartAlternatorsEfficiency As Single Implements IDiagnostics.AA_NonSmartAlternatorsEfficiency

    Public Property AA_OverrunFlag As Integer Implements IDiagnostics.AA_OverrunFlag

    Public Property AA_SmartIdleAlternatorsEfficiency As Single Implements IDiagnostics.AA_SmartIdleAlternatorsEfficiency

    Public Property AA_SmartIdleCurrent_Amps As Single Implements IDiagnostics.AA_SmartIdleCurrent_Amps

    Public Property AA_SmartOverrunAlternatorEfficiency As Single Implements IDiagnostics.AA_SmartOverrunAlternatorEfficiency

    Public Property AA_SmartOverrunCurrent_Amps As Single Implements IDiagnostics.AA_SmartOverrunCurrent_Amps

    Public Property AA_SmartTractionAlternatorEfficiency As Single Implements IDiagnostics.AA_SmartTractionAlternatorEfficiency

    Public Property AA_SmartTractionCurrent_Amps As Single Implements IDiagnostics.AA_SmartTractionCurrent_Amps

    Public Property AA_TotalCycleFC_BeforeSSandWHTCcorrection_Grams As Single Implements IDiagnostics.AA_TotalCycleFC_BeforeSSandWHTCcorrection_Grams

    Public Property AA_TotalCycleFC_BeforeSSandWHTCcorrection_Litres As Single Implements IDiagnostics.AA_TotalCycleFC_BeforeSSandWHTCcorrection_Litres


End Class

