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


Public Class VectoInputs

Implements IVectoInputs

    Public Property Cycle As String Implements IVectoInputs.Cycle

    Public Property VehicleWeightKG As Single Implements IVectoInputs.VehicleWeightKG

     Public Property PowerNetVoltage As Single Implements IVectoInputs.PowerNetVoltage

    'Public Property CycleDurationMinutes As Single Implements IVectoInputs.CycleDurationMinutes

    Public Property FuelMap As String Implements IVectoInputs.FuelMap

End Class

