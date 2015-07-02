Imports System.ComponentModel

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

        'Fields
        Private _BaseVehicle As Boolean
        Private _Category As String
        Private _ConsumerName As String
        Private _NominalConsumptionAmps As Single
        Private _NumberInActualVehicle As Integer
        Private _PhaseIdle_TractionOn As Single
        Private _PowerNetVoltage As Single
        Private _Info As String

        'Calculated
        Private Property AvgConsumptionAmps As Single Implements IElectricalConsumer.AvgConsumptionAmps

        'Properties
        Public Property BaseVehicle As Boolean Implements IElectricalConsumer.BaseVehicle
            Get
                Return _BaseVehicle
            End Get
            Set(value As Boolean)
                _BaseVehicle = value
                NotifyPropertyChanged("BaseVehicle")
            End Set
        End Property

        Public Property Category As String Implements IElectricalConsumer.Category
            Get
                Return _Category
            End Get
            Set(value As String)
                _Category = value
                NotifyPropertyChanged("Category")
            End Set
        End Property

        Public Property ConsumerName As String Implements IElectricalConsumer.ConsumerName
            Get
                Return _ConsumerName
            End Get
            Set(value As String)
                _ConsumerName = value
                NotifyPropertyChanged("ConsumerName")
            End Set
        End Property

        Public Property NominalConsumptionAmps As Single Implements IElectricalConsumer.NominalConsumptionAmps
            Get
                Return _NominalConsumptionAmps
            End Get
            Set(value As Single)
                _NominalConsumptionAmps = value
                NotifyPropertyChanged("NominalConsumptionAmps")
            End Set
        End Property

        Public Property NumberInActualVehicle As Integer Implements IElectricalConsumer.NumberInActualVehicle
            Get
                Return _NumberInActualVehicle
            End Get
            Set(value As Integer)
                _NumberInActualVehicle = value
                NotifyPropertyChanged("NumberInActualVehicle")
            End Set
        End Property

        Public Property PhaseIdle_TractionOn As Single Implements IElectricalConsumer.PhaseIdle_TractionOn
            Get
                Return _PhaseIdle_TractionOn
            End Get
            Set(value As Single)
                _PhaseIdle_TractionOn = value
                NotifyPropertyChanged("PhaseIdle_TractionOn")
            End Set
        End Property

        Public Property PowerNetVoltage As Single Implements IElectricalConsumer.PowerNetVoltage
            Get
                Return _PowerNetVoltage
            End Get
            Set(value As Single)
                _PowerNetVoltage = value
                NotifyPropertyChanged("PowerNetVoltage")
            End Set
        End Property

        Public Property Info As String Implements IElectricalConsumer.Info
            Get
                Return _Info
            End Get
            Set(value As String)
                _Info = value
                NotifyPropertyChanged("Info")
            End Set
        End Property


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
        Public Sub New(BaseVehicle As Boolean, Category As String, ConsumerName As String, NominalConsumptionAmps As Single, PhaseIdle_TractionOn As Single, PowerNetVoltage As Single, numberInVehicle As Integer, info As String)

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


            Return Me.ConsumerName = other.ConsumerName


        End Function
        <System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage>
        Public Overrides Function GetHashCode() As Integer
            Return 0
        End Function


        Public Event PropertyChanged As PropertyChangedEventHandler _
            Implements INotifyPropertyChanged.PropertyChanged

        Private Sub NotifyPropertyChanged(p As String)
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(p))
        End Sub


    End Class

End Namespace