Imports VectoAuxiliaries.Electrics

Namespace Mocks

    Public Class AlternatorMock
        Implements IAlternator

        Private _gearPullyRatio As Single = 1
        Private _gearPullyEfficiency As Single = 1


        Public Property PulleyGearRatio() As Single Implements IAlternator.PulleyGearRatio
            Get
                Return _gearPullyRatio
            End Get
            Set(ByVal value As Single)
                _gearPullyRatio = value
            End Set
        End Property

        Public Property PulleyGearEfficiency() As Single Implements IAlternator.PulleyGearEfficiency
            Get
                Return _gearPullyEfficiency
            End Get
            Set(ByVal value As Single)
                _gearPullyEfficiency = value
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
