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

    ''' <summary>
    ''' Described a consumer of Alternator electrical power
    ''' </summary>
    ''' <remarks></remarks>
    Public Class ElectricalConsumer
        Implements IElectricalConsumer


       'Calculated
       Private Property AvgConsumptionAmps As Single Implements IElectricalConsumer.AvgConsumptionAmps

       'Properties
       Public Property BaseVehicle As Boolean Implements IElectricalConsumer.BaseVehicle
       Public Property Category As String Implements IElectricalConsumer.Category
       Public Property ConsumerName As String Implements IElectricalConsumer.ConsumerName
       Public Property NominalConsumptionAmps As Single Implements IElectricalConsumer.NominalConsumptionAmps
       Public Property NumberInActualVehicle As Integer Implements IElectricalConsumer.NumberInActualVehicle
       Public Property PhaseIdle_TractionOn As Single Implements IElectricalConsumer.PhaseIdle_TractionOn
       Public Property PowerNetVoltage As Single Implements IElectricalConsumer.PowerNetVoltage
       Public Property Info As String Implements IElectricalConsumer.Info

       'Public class outputs
       Public Function TotalAvgConumptionAmps(Optional PhaseIdle_TractionOnBasedOnCycle As Single = 0.0) As Single Implements IElectricalConsumer.TotalAvgConumptionAmps

           If ConsumerName = "Doors per Door" Then
               Return PhaseIdle_TractionOnBasedOnCycle * NominalConsumptionAmps * NumberInActualVehicle
           Else
               Return PhaseIdle_TractionOn * NominalConsumptionAmps * NumberInActualVehicle
           End If


        End Function
       Public Function TotalAvgConsumptionInWatts(Optional PhaseIdle_TractionOnBasedOnCycle As Single = 0.0) As Single Implements Electrics.IElectricalConsumer.TotalAvgConsumptionInWatts
            Return TotalAvgConumptionAmps(PhaseIdle_TractionOnBasedOnCycle) * PowerNetVoltage
        End Function 

       'Constructor
       Public Sub New(BaseVehicle As Boolean, Category As String, ConsumerName As String, NominalConsumptionAmps As Single, PhaseIdle_TractionOn As Single, PowerNetVoltage As Single, numberInVehicle As Integer,info As string)

            'Illegal Value Check.
            If Category.Trim.Length = 0 Then Throw New ArgumentException("Category Name cannot be empty")
            If ConsumerName.Trim.Length = 0 Then Throw New ArgumentException("ConsumerName Name cannot be empty")
            If PhaseIdle_TractionOn < ElectricConstants.PhaseIdleTractionOnMin Or PhaseIdle_TractionOn > ElectricConstants.PhaseIdleTractionMax Then Throw New ArgumentException("PhaseIdle_TractionOn must have a value between 0 and 1")
            If NominalConsumptionAmps < ElectricConstants.NonminalConsumerConsumptionAmpsMin Or NominalConsumptionAmps > ElectricConstants.NominalConsumptionAmpsMax Then Throw New ArgumentException("NominalConsumptionAmps must have a value between 0 and 100")
            If PowerNetVoltage < ElectricConstants.PowenetVoltageMin Or PowerNetVoltage > ElectricConstants.PowenetVoltageMax Then Throw New ArgumentException("PowerNetVoltage must have a value between 6 and 48")
            If numberInVehicle < 0 Then Throw New ArgumentException("Cannot have less than 0 consumers in the vehicle")

            'Good, now assign.
            Me.BaseVehicle = BaseVehicle
            Me.Category = Category
            Me.ConsumerName = ConsumerName
            Me.NominalConsumptionAmps = NominalConsumptionAmps
            Me.PhaseIdle_TractionOn = PhaseIdle_TractionOn
            Me.PowerNetVoltage = PowerNetVoltage
            Me.NumberInActualVehicle = numberInVehicle
            Me.Info = info

       End Sub

       'Comparison Overrides
       Public Overrides Function Equals(obj As Object) As Boolean

         Dim other As IElectricalConsumer = CType(obj, IElectricalConsumer)
          

             Return Me.ConsumerName=other.ConsumerName


        End Function
       <System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage>
        Public Overrides Function GetHashCode() As Integer
            Return 0
        End Function
      



    End Class

End Namespace