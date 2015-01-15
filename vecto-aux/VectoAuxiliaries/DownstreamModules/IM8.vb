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


Imports VectoAuxiliaries.Electrics
Imports VectoAuxiliaries.Pneumatics
Imports VectoAuxiliaries.Hvac

Namespace DownstreamModules

Public Interface IM8


'OUT1
''' <summary>
''' Aux Power At Crank From Electrical HVAC And Pneumatics Ancilaries (W)
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
ReadOnly Property AuxPowerAtCrankFromElectricalHVACAndPneumaticsAncillaries As Single
'OUT2
''' <summary>
''' Smart Electrical Alternator Power Gen At Crank (W)
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
ReadOnly Property SmartElectricalAlternatorPowerGenAtCrank As Single
'OUT3
''' <summary>
''' Compressor Flag
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
ReadOnly Property CompressorFlag As Integer


End Interface


End Namespace


