Namespace Hvac

    Public Interface IEnvironmentalConditionsMap

        Function Initialise() As Boolean

        Function GetEnvironmentalConditions() As List(Of IEnvironmentalCondition)

    End Interface

End Namespace
