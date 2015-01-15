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


Public Interface IM0_5_SmartAlternatorSetEfficiency


readonly property SmartIdleCurrent() As single
readonly property AlternatorsEfficiencyIdleResultCard( ) As single
readonly property SmartTractionCurrent As Single
readonly property AlternatorsEfficiencyTractionOnResultCard() As Single
readonly property SmartOverrunCurrent As Single
readonly property AlternatorsEfficiencyOverrunResultCard() As single



End Interface



End Namespace



