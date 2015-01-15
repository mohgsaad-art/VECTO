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

Imports System.IO

Namespace Hvac

Public Class HVACSteadyStateModel
Implements IHVACSteadyStateModel


Public Property HVACElectricalLoadPowerWatts As Single Implements IHVACSteadyStateModel.HVACElectricalLoadPowerWatts
Public Property HVACFuellingLitresPerHour As Single Implements IHVACSteadyStateModel.HVACFuellingLitresPerHour
Public Property HVACMechanicalLoadPowerWatts As Single Implements IHVACSteadyStateModel.HVACMechanicalLoadPowerWatts


Public Sub New()


End Sub


Public Sub New(elecPowerW As Single, mechPowerW As Single, fuellingLPH As Single)

     HVACElectricalLoadPowerWatts = elecPowerW
     HVACFuellingLitresPerHour = mechPowerW
     HVACMechanicalLoadPowerWatts = fuellingLPH

End Sub


Public Function SetValuesFromMap( ByVal filePath As String, byref message As String ) As Boolean Implements IHVACSteadyStateModel.SetValuesFromMap


            Dim myData As String
            Dim linesArray As String()


            'Check map file can be found.
            Try
                myData = System.IO.File.ReadAllText(filePath, System.Text.Encoding.UTF8)
            Catch ex As FileNotFoundException

                message = "HVAC Steady State Model : The map file was not found"
                Return false
            End Try


            linesArray = (From s As String In myData.Split(vbLf) Select s.Trim).ToArray

            'Check count is at least 2 rows
            If linesArray.Count<2 then 
               message="HVAC Steady State Model : Insufficient Lines in this File"
               Return False
            End If

            'validate headers
            Dim headers As String() = linesArray(0).Split(","c)
            If    headers.Count<>3 OrElse
                  headers(0).Trim<>"[Electrical Power (w)]"  OrElse _
                  headers(1).Trim<>"[Mechanical Power (w)]"  OrElse  _
                  headers(2).Trim<>"[Fuelling (L/H)]" Then
                  message = "HVAC Steady State Model : Column headers in  *.AHSM file being read are incompatable."
                  Return False

            End If
              
            'validate values
            Dim values As String()  = linesArray(1).Split(","c)
             If   headers.Count<>3             OrElse _
                  NOT IsNumeric( values(0))    OrElse _
                  NOT IsNumeric( values(1))    OrElse _
                  Not IsNumeric( values(2))    
                  message="Steady State Model : Unable to confirm numeric values in the *.AHSM file being read."
                  Return False
            End If

            'OK we have the values so lets set the  properties
             Dim out1,out2,out3 As single
             out1= HVACElectricalLoadPowerWatts 
             out2= HVACMechanicalLoadPowerWatts 
             out3= HVACFuellingLitresPerHour    
            try

              HVACElectricalLoadPowerWatts = Single.Parse(values(0))
              HVACMechanicalLoadPowerWatts = Single.Parse(values(1))
              HVACFuellingLitresPerHour    = Single.Parse(values(2))

            Catch ex As Exception

               'Restore in the event of failure to fully assign
               HVACElectricalLoadPowerWatts = out1
               HVACMechanicalLoadPowerWatts = out2
               HVACFuellingLitresPerHour    = out3

               'Return result
               message="Steady State Model : Unable to parse the values in the *.AHSM file being read no values were harmed in reading of this file."
               Return False

            End Try



            Return True


End Function



End Class


End Namespace



