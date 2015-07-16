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

Namespace DownstreamModules

    Public Interface IM12

        ''' <summary>
        ''' Fuel consumption with smart Electrics and Average Pneumatic Power Demand
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ReadOnly Property FuelconsumptionwithsmartElectricsandAveragePneumaticPowerDemand As Single

        ''' <summary>
        ''' Base Fuel Consumption With Average Auxiliary Loads
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ReadOnly Property BaseFuelConsumptionWithTrueAuxiliaryLoads As Single
        ''' <summary>
        ''' Stop Start Correction
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ReadOnly Property StopStartCorrection As Single


        'Diagnostic Signals Only For Testing - No Material interference with operation of class.
        ReadOnly Property P1X As Single
        ReadOnly Property P1Y As Single
        ReadOnly Property P2X As Single
        ReadOnly Property P2Y As Single
        ReadOnly Property P3X As Single
        ReadOnly Property P3Y As Single
        ReadOnly Property XTAIN As Single
        ReadOnly Property INTRP1 As Single
        ReadOnly Property INTRP2 As Single


    End Interface


End Namespace


