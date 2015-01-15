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

  Public Interface IM6


    ''' <summary>
 ''' OverrunFlag
 ''' </summary>
 ''' <value></value>
 ''' <returns>0 = Not in overrun, 1 = In Overrun</returns>
 ''' <remarks></remarks>
    Readonly Property  OverrunFlag                                 As Integer
   
    ''' <summary>
 ''' Smart Elec And Pneumatics Compressor Flag
 ''' </summary>
 ''' <value></value>
 ''' <returns></returns>
 ''' <remarks></remarks>
    Readonly Property  SmartElecAndPneumaticsCompressorFlag        As Integer
   
    ''' <summary>
 ''' Smart Elec And Pneumatic: Alternator Power Gen At Crank (W)
 ''' </summary>
 ''' <value></value>
 ''' <returns></returns>
 ''' <remarks></remarks>
    ReadOnly Property  SmartElecAndPneumaticAltPowerGenAtCrank     As Single
   
    ''' <summary>
 ''' Smart Elec And Pneumatic: Air Compressor Power Gen At Crank (W)
 ''' </summary>
 ''' <value></value>
 ''' <returns></returns>
 ''' <remarks></remarks>
    ReadOnly Property  SmartElecAndPneumaticAirCompPowerGenAtCrank As Single
   
    ''' <summary>
 ''' Smart Electrics Only :  Alternator Power Gen At Crank (W)
 ''' </summary>
 ''' <value></value>
 ''' <returns></returns>
 ''' <remarks></remarks>
    ReadOnly Property  SmartElecOnlyAltPowerGenAtCrank             As Single
   
    ''' <summary>
''' Average Power Demand At Crank From Pneumatics (W)
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
    ReadOnly Property  AveragePowerDemandAtCrankFromPneumatics     As Single
   
    ''' <summary>
 ''' Smart Pneumatic Only Air Comp Power Gen At Crank (W)
 ''' </summary>
 ''' <value></value>
 ''' <returns></returns>
 ''' <remarks></remarks>
    ReadOnly Property  SmartPneumaticOnlyAirCompPowerGenAtCrank    As Single
                      
    ''' <summary>
 ''' Avgerage Power Demand At Crank From Electrics Including HVAC electrics (W)
 ''' </summary>
 ''' <value></value>
 ''' <returns></returns>
 ''' <remarks></remarks>
    ReadOnly Property  AvgPowerDemandAtCrankFromElectricsIncHVAC   As Single
   
    ''' <summary>
 ''' Smart Pneumatics Only CompressorFlag
 ''' </summary>
 ''' <value></value>
 ''' <returns>Less than Zero = No, Greater then Zero = Yes </returns>
 ''' <remarks></remarks>
    ReadOnly Property  SmartPneumaticsOnlyCompressorFlag          As Integer

 
End Interface

End Namespace


