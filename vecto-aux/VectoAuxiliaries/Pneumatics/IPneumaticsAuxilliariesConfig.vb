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

Public Interface IPneumaticsAuxilliariesConfig

Property OverrunUtilisationForCompressionFraction As Single
Property BrakingWithRetarderNIperKG As Single
Property BrakingNoRetarderNIperKG As Single
Property BreakingPerKneelingNIperKGinMM As Single
Property PerDoorOpeningNI As Single
Property PerStopBrakeActuationNIperKG As Single
Property AirControlledSuspensionNIperMinute As Single
Property AdBlueNIperMinute As Single
Property NonSmartRegenFractionTotalAirDemand As Single
Property SmartRegenFractionTotalAirDemand As Single
Property DeadVolumeLitres As Single
Property DeadVolBlowOutsPerLitresperHour As Single

End Interface
