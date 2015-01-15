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

Namespace Pneumatics

Public Class PneumaticsAuxilliariesConfig
Implements IPneumaticsAuxilliariesConfig


Public Property AdBlueNIperMinute As Single Implements IPneumaticsAuxilliariesConfig.AdBlueNIperMinute
Public Property AirControlledSuspensionNIperMinute As Single Implements IPneumaticsAuxilliariesConfig.AirControlledSuspensionNIperMinute
Public Property BrakingNoRetarderNIperKG As Single Implements IPneumaticsAuxilliariesConfig.BrakingNoRetarderNIperKG
Public Property BrakingWithRetarderNIperKG As Single Implements IPneumaticsAuxilliariesConfig.BrakingWithRetarderNIperKG
Public Property BreakingPerKneelingNIperKGinMM As Single Implements IPneumaticsAuxilliariesConfig.BreakingPerKneelingNIperKGinMM
Public Property DeadVolBlowOutsPerLitresperHour As Single Implements IPneumaticsAuxilliariesConfig.DeadVolBlowOutsPerLitresperHour
Public Property DeadVolumeLitres As Single Implements IPneumaticsAuxilliariesConfig.DeadVolumeLitres
Public Property NonSmartRegenFractionTotalAirDemand As Single Implements IPneumaticsAuxilliariesConfig.NonSmartRegenFractionTotalAirDemand
Public Property OverrunUtilisationForCompressionFraction As Single Implements IPneumaticsAuxilliariesConfig.OverrunUtilisationForCompressionFraction
Public Property PerDoorOpeningNI As Single Implements IPneumaticsAuxilliariesConfig.PerDoorOpeningNI
Public Property PerStopBrakeActuationNIperKG As Single Implements IPneumaticsAuxilliariesConfig.PerStopBrakeActuationNIperKG
Public Property SmartRegenFractionTotalAirDemand As Single Implements IPneumaticsAuxilliariesConfig.SmartRegenFractionTotalAirDemand


Public Sub New(Optional setToDefaults As Boolean = False)

If setToDefaults Then SetDefaults()


End Sub

Public Sub SetDefaults()

     OverrunUtilisationForCompressionFraction = 0.97
     BrakingWithRetarderNIperKG = 0.0005
     BrakingNoRetarderNIperKG = 0.00081
     BreakingPerKneelingNIperKGinMM = 0.000066
     PerDoorOpeningNI = 12.7
     PerStopBrakeActuationNIperKG = 0.00064
     AirControlledSuspensionNIperMinute = 15
     AdBlueNIperMinute = 21.25
     NonSmartRegenFractionTotalAirDemand = 0.26
     SmartRegenFractionTotalAirDemand = 0.12
     DeadVolumeLitres = 30
     DeadVolBlowOutsPerLitresperHour = 24

End Sub


End Class



End Namespace



