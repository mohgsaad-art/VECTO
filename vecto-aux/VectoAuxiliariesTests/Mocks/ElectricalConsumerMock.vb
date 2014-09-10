Imports VectoAuxiliaries.Electrics

Namespace Mocks

    Public Class ElectricalConsumerMock
        Implements IElectricalConsumer

        Public ReadOnly Property Name() As String Implements IElectricalConsumer.Name
            Get
                Return "TestName"
            End Get
        End Property

        Public ReadOnly Property Power() As Single Implements IElectricalConsumer.Power
            Get
                Return 100.0
            End Get
        End Property

    End Class

End Namespace
