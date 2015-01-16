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

Namespace Electrics

Public Interface IElectricsUserInputsConfig

    ''' <summary>
    ''' Power Net Voltage - The supply voltage used on the vehilce.
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Property PowerNetVoltage As Single
    ''' <summary>
    ''' The Path for the Alternator map
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Property AlternatorMap As String
    ''' <summary>
    ''' Alternator Gear Efficiency
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Property AlternatorGearEfficiency As Single
    ''' <summary>
    ''' List of Electrical Consumers
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Property ElectricalConsumers As IElectricalConsumerList
    ''' <summary>
    ''' Door Actuation Time In Seconds ( Time Taken to Open/Close the door )
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Property DoorActuationTimeSecond As Integer
    ''' <summary>
    ''' Result Card Taken During Idle.
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Property ResultCardIdle As IResultCard
    ''' <summary>
    ''' Result Card Taken During Traction
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Property ResultCardTraction As IResultCard
    ''' <summary>
    ''' Result Card Taken During Overrun
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Property ResultCardOverrun As IResultCard
    ''' <summary>
    ''' Smart Electrical System
    ''' </summary>
    ''' <value></value>
    ''' <returns>True For Smart Electrical Systems/ False For non Smart.</returns>
    ''' <remarks></remarks>
    Property SmartElectrical As Boolean

End Interface


End Namespace


