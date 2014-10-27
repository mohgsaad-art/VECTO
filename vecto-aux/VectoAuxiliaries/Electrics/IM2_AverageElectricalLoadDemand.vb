Namespace Electrics

Public Interface IM2_AverageElectricalLoadDemand

    Function GetAveragePowerDemandAtAlternator() As Single
    Function GetAveragePowerAtCrank(ByVal engineRpm As Integer) As Single

End Interface

End Namespace


