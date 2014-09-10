Imports VectoAuxiliaries.Electrics

Namespace Mocks

    Public Class AlternatorMock
        Implements IAlternator

        Public Property PulleyGearRatio() As Single Implements IAlternator.PulleyGearRatio
            Get
                Return 1.0
            End Get
            Set(ByVal value As Single)

            End Set
        End Property

        Public Property PulleyGearEfficiency() As Single Implements IAlternator.PulleyGearEfficiency
            Get
                Return 1.0
            End Get
            Set(ByVal value As Single)

            End Set
        End Property

        Public Function Initialise() As Boolean Implements IAlternator.Initialise
            Return True
        End Function

        Public Function GetEfficiency(ByVal engineRpm As Single) As Single Implements IAlternator.GetEfficiency
            Return 0.5
        End Function

        Public Function GetMaximumRegenerationPower(ByVal engineRpm As Single) As Single Implements IAlternator.GetMaximumRegenerationPower
            Return 100
        End Function

        Public Function GetMaximumRegeneratinPowerAtCrank(ByVal engineRpm As Single) As Single Implements IAlternator.GetMaximumRegeneratinPowerAtCrank
            Return 200
        End Function
    End Class

End Namespace
