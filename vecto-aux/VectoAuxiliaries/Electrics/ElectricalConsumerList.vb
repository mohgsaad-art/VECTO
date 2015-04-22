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


Imports System.Collections.Generic

Namespace Electrics

  Public Class ElectricalConsumerList
    Implements IElectricalConsumerList

   Private _items As New List(Of IElectricalConsumer)
   Private _powernetVoltage As Single
   Private _doorDutyCycleZeroToOne As single


   'Constructor
   Public Sub New(powernetVoltage As Single,doorDutyCycle_ZeroToOne As single, Optional createDefaultList As Boolean = False)

   _powernetVoltage = powernetVoltage
   
   If createDefaultList Then SetDefaultConsumerList()
   
   _doorDutyCycleZeroToOne = doorDutyCycle_ZeroToOne

End Sub
   'Initialise default set of consumers
   Public Sub SetDefaultConsumerList()

     'This populates the default settings as per engineering spreadsheet.
     'Vehicle Basic Equipment' category can be added or remove by customers.
     'At some time in the future, this may be removed and replace with file based consumer lists.
    
     _items.Clear()
    
     Dim c1, c2, c3, c4, c5, c6, c7, c8, c9, c10, c11, c12, c13, c14, c15, c16, c17, c18, c19, c20 As IElectricalConsumer
    
     c1 = CType(New ElectricalConsumer(False, "Doors", "Doors per Door",                                                                        3.00, 0.096339, _powernetVoltage, 3), IElectricalConsumer)
     c2 = CType(New ElectricalConsumer(True, "Veh Electronics &Engine", "Controllers,Valves etc",                                               25.00, 1.00, _powernetVoltage, 1), IElectricalConsumer)
     c3 = CType(New ElectricalConsumer(False, "Vehicle basic equipment", "Radio City",                                                           2.00, 0.80, _powernetVoltage, 1), IElectricalConsumer)
     c4 = CType(New ElectricalConsumer(False, "Vehicle basic equipment", "Radio Intercity",                                                      5.00, 0.80, _powernetVoltage, 0), IElectricalConsumer)
     c5 = CType(New ElectricalConsumer(False, "Vehicle basic equipment", "Radio/Audio Tourism",                                                  9.00, 0.80, _powernetVoltage, 0), IElectricalConsumer)
     c6 = CType(New ElectricalConsumer(False, "Vehicle basic equipment", "Fridge",                                                               4.00, 0.50, _powernetVoltage, 0), IElectricalConsumer)
     c7 = CType(New ElectricalConsumer(False, "Vehicle basic equipment", "Kitchen Standard",                                                    67.00, 0.05, _powernetVoltage, 0), IElectricalConsumer)
     c8 = CType(New ElectricalConsumer(False, "Vehicle basic equipment", "Interior lights City/ Intercity + Doorlights [1/m]",                   1.00, 0.70, _powernetVoltage, 12), IElectricalConsumer)
     c9 = CType(New ElectricalConsumer(False, "Vehicle basic equipment", "LED Interior lights ceiling city/ontercity + door [1/m]",              0.60, 0.70, _powernetVoltage, 0), IElectricalConsumer)
     c10 = CType(New ElectricalConsumer(False, "Vehicle basic equipment", "Interior lights Tourism + reading [1/m]",                             1.10, 0.70, _powernetVoltage, 0), IElectricalConsumer)
     c11 = CType(New ElectricalConsumer(False, "Vehicle basic equipment", "LED Interior lights ceiling Tourism + LED reading [1/m]",             0.66, 0.70, _powernetVoltage, 0), IElectricalConsumer)
     c12 = CType(New ElectricalConsumer(False, "Customer Specific Equipment", "External Displays Font/Side/Rear",                                2.65017667844523, 1.00, _powernetVoltage, 4), IElectricalConsumer)
     c13 = CType(New ElectricalConsumer(False, "Customer Specific Equipment", "Internal display per unit ( front side rear)",                    1.06007067137809, 1.00, _powernetVoltage, 1), IElectricalConsumer)
     c14 = CType(New ElectricalConsumer(False, "Customer Specific Equipment", "CityBus Ref EBSF Table4 Devices ITS No Displays",                 9.30, 1.00, _powernetVoltage, 1), IElectricalConsumer)
     c15 = CType(New ElectricalConsumer(False, "Lights", "Exterior Lights BULB",                                                                 7.40, 1.00, _powernetVoltage, 1), IElectricalConsumer)
     c16 = CType(New ElectricalConsumer(False, "Lights", "Day running lights LED bonus",                                                        -0.723, 1.00, _powernetVoltage, 1), IElectricalConsumer)
     c17 = CType(New ElectricalConsumer(False, "Lights", "Antifog rear lights LED bonus",                                                       -0.17, 1.00, _powernetVoltage, 1), IElectricalConsumer)
     c18 = CType(New ElectricalConsumer(False, "Lights", "Position lights LED bonus",                                                           -1.20, 1.00, _powernetVoltage, 1), IElectricalConsumer)
     c19 = CType(New ElectricalConsumer(False, "Lights", "Direction lights LED bonus",                                                          -0.30, 1.00, _powernetVoltage, 1), IElectricalConsumer)
     c20 = CType(New ElectricalConsumer(False, "Lights", "Brake Lights",                                                                        -1.20, 1.00, _powernetVoltage, 1), IElectricalConsumer)
    
    _items.Add(c1)
    _items.Add(c2)
    _items.Add(c3)
    _items.Add(c4)
    _items.Add(c5)
    _items.Add(c6)
    _items.Add(c7)
    _items.Add(c8)
    _items.Add(c9)
    _items.Add(c10)
    _items.Add(c11)
    _items.Add(c12)
    _items.Add(c13)
    _items.Add(c14)
    _items.Add(c15)
    _items.Add(c16)
    _items.Add(c17)
    _items.Add(c18)
    _items.Add(c19)
    _items.Add(c20)
    
End Sub    
  

   'Interface implementation
   Public Property DoorDutyCycleFraction As single Implements IElectricalConsumerList.DoorDutyCycleFraction

 Get
  Return _doorDutyCycleZeroToOne
 End Get
 Set(value As Single)
  _doorDutyCycleZeroToOne=value
 End Set

end property  
   Public ReadOnly Property Items As List(Of  IElectricalConsumer) Implements IElectricalConsumerList.Items
    Get
     Return _items
    End Get
End Property
   Public Sub AddConsumer(consumer As IElectricalConsumer) Implements Electrics.IElectricalConsumerList.AddConsumer

       If Not _items.Contains(consumer) Then
         _items.Add(consumer)
       Else
      
       Throw New ArgumentException("Consumer Already Present in the list")
      
       End If

   End Sub
   Public Sub RemoveConsumer(consumer As IElectricalConsumer) Implements Electrics.IElectricalConsumerList.RemoveConsumer

     If _items.Contains(consumer) Then
    
     _items.Remove(consumer)
    
     Else
    
       Throw New ArgumentException("Consumer Not In List")
    
     End If

   End Sub
   Public Function GetTotalAverageDemandAmps(excludeOnBase As Boolean) As Single Implements Electrics.IElectricalConsumerList.GetTotalAverageDemandAmps

     Dim Amps As Single

     If excludeOnBase Then
       Amps = Aggregate item In Items Where item.BaseVehicle = False Into Sum(item.TotalAvgConumptionAmps(DoorDutyCycleFraction))
     Else
       Amps = Aggregate item In Items Into Sum(item.TotalAvgConumptionAmps(DoorDutyCycleFraction))
     End If


    Return Amps

   End Function


End Class

End Namespace


