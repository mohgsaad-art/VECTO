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
   
    ''' <summary>
    ''' Name of the Cycle ( Urban, Interurban etc )
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property Cycle As String Implements IVectoInputs.Cycle
    ''' <summary>
    ''' Vehicle Mass (KG)
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property VehicleWeightKG As Single Implements IVectoInputs.VehicleWeightKG
    ''' <summary>
    ''' Powernet Voltage (V)
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks>This is the power voltage available in the bus - usually 26.3 Volts</remarks>
    Public Property PowerNetVoltage As Single Implements IVectoInputs.PowerNetVoltage
    ''' <summary>
    ''' Fuel Map Same One as used in Vecto.
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property FuelMap As String Implements IVectoInputs.FuelMap
    ''' <summary>
    ''' Fuel Density as used in Vecto.
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property FuelDensity As String Implements IVectoInputs.FuelDensity

End Class

