Namespace Hvac
    Public Interface IEnvironmentalCondition

        Function GetTemperature() As Double
        Function GetSolar() As Double
        Function GetWeighting() As Double
        Function GetNormalisedWeighting(map As List(Of IEnvironmentalCondition)) As Double

    End Interface
End Namespace