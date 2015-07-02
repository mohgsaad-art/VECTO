Namespace Hvac

    Public Class EnvironmentalCondition
        Implements IEnvironmentalCondition

        Private _temperature As Double
        Private _solar As Double
        Private _weight As Double

        Public Sub New(temperature As Double, solar As Double, weight As Double)

            _temperature = temperature
            _solar = solar
            _weight = weight

        End Sub

        Public Function GetTemperature() As Double Implements IEnvironmentalCondition.GetTemperature
            Return _temperature
        End Function

        Public Function GetSolar() As Double Implements IEnvironmentalCondition.GetSolar
            Return _solar
        End Function

        Public Function GetWeight() As Double Implements IEnvironmentalCondition.GetWeighting
            Return _weight
        End Function

        Public Function GetNormalisedWeight(map As List(Of IEnvironmentalCondition)) As Double Implements IEnvironmentalCondition.GetNormalisedWeighting
            Return _weight / map.Sum(Function(w) w.GetWeighting())
        End Function

    End Class

End Namespace