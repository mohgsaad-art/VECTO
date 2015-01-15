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

'Watts
 Readonly Property  OverrunFlag As Integer
 Readonly Property  SmartElecAndPneumaticsCompressorFlag As integer
 ReadOnly Property  SmartElecAndPneumaticAltPowerGenAtCrank As Single
 ReadOnly Property  SmartElecAndPneumaticAirCompPowerGenAtCrank As Single
 ReadOnly Property  SmartElecOnlyAltPowerGenAtCrank As Single
 ReadOnly Property  AveragePowerDemandAtCrankFromPneumatics As Single
 ReadOnly Property  SmartPneumaticOnlyAirCompPowerGenAtCrank As Single
                   
 ReadOnly Property  AvgPowerDemandAtCrankFromElectricsIncHVAC As Single
 ReadOnly Property  SmartPneumaticsOnlyCompressorFlag As Integer
 


End Interface


End Namespace


