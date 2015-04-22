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

Public Class ElectricConstants

    'Anticipated Min and Max Allowable values for Powernet, normally 26.3 volts but could be 48 in the future.
    Public Const PowenetVoltageMin As Single = 6
    Public Const PowenetVoltageMax As Single = 50

    'Duty Cycle IE Percentage of use
    Public Const PhaseIdleTractionOnMin As Single = 0
    Public Const PhaseIdleTractionMax As Single = 1

    'Max Min Expected Consumption for a Single Consumer, negative values allowed as bonuses.
    Public Const NonminalConsumerConsumptionAmpsMin As Integer = -10
    Public Const NominalConsumptionAmpsMax As Integer = 100


    'Alternator
    public const  AlternatorPulleyEfficiencyMin as single =0.1
    public const  AlternatorPulleyEfficiencyMax as single =1

    'Battery
    Public Const StoredEnergyEfficiency As Single = 1'0.935 


End Class


End Namespace









