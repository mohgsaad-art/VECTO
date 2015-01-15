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
    Public Interface IM4_AirCompressor
        ''' <summary>
        ''' Ratio of Gear or Pulley used to drive the compressor
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Property PulleyGearRatio() As Single

        ''' <summary>
        ''' Efficiency of the Pulley or Gear used to drive the compressor
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Property PulleyGearEfficiency() As Single

        ''' <summary>
        ''' Initialises the AirCompressor Class
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Function Initialise() As Boolean

        ''' <summary>
        ''' Returns the flow rate [litres/second] of compressor for the given engine rpm
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Function GetFlowRate() As Single

        ''' <summary>
        ''' Returns the power consumed for the given engine rpm when compressor is off
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Function GetPowerCompressorOff() As Single

        ''' <summary>
        ''' Returns the power consumed for the given engine rpm when compressor is on
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Function GetPowerCompressorOn() As Single

        ''' <summary>
        ''' Returns the difference in power between compressonr on and compressor off operation at the given engine rpm
        ''' </summary>
        ''' <returns>Single / Watts</returns>
        ''' <remarks></remarks>
        Function GetPowerDifference() As Single

        ''' <summary>
        ''' Returns Average PoweDemand PeCompressor UnitFlowRate 
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Function GetAveragePowerDemandPerCompressorUnitFlowRate() As Single

    End Interface
End NameSpace