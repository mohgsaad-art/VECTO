Namespace Hvac

    Public Interface IEnvironmentalConditionsMap

        Sub Initialise()

        Function GetEnvironmentalConditions() As List(Of IEnvironmentalCondition)

    End Interface

End Namespace
