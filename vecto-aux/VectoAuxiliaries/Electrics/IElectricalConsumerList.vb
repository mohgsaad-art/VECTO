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


Namespace Electrics

Public Interface IElectricalConsumerList

    ''' <summary>
    ''' List of Electrical Consumers
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    ReadOnly Property Items As List(Of  IElectricalConsumer)

    ''' <summary>
    ''' Add New Electrical Consumer
    ''' </summary>
    ''' <param name="consumer"></param>
    ''' <remarks></remarks>
    Sub AddConsumer(consumer As IElectricalConsumer)

    ''' <summary>
    ''' Remove Electrical Consumer
    ''' </summary>
    ''' <param name="consumer"></param>
    ''' <remarks></remarks>
    Sub RemoveConsumer(consumer As IElectricalConsumer)

    ''' <summary>
    ''' Get Total Average Demand In Amps 
    ''' </summary>
    ''' <param name="excludeOnBase">Exclude those on base vehicle</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Function GetTotalAverageDemandAmps(excludeOnBase As Boolean) As Single

    ''' <summary>
    ''' Door Actuation Time Fraction ( Total Time Spent Operational during cycle )
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Property DoorDutyCycleFraction As single

End Interface


End Namespace


