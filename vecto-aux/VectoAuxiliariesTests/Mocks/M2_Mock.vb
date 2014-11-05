Imports VectoAuxiliaries.Electrics
Imports VectoAuxiliaries.Pneumatics
Imports VectoAuxiliaries.Hvac
Imports VectoAuxiliaries.DownstreamModules

Public Class M2_Mock
Implements IM2_AverageElectricalLoadDemand


Public _GetAveragePowerAtCrankFromElectrics As Single
Public _GetAveragePowerDemandAtAlternator   As Single



    Public Function GetAveragePowerAtCrankFromElectrics() As Single Implements IM2_AverageElectricalLoadDemand.GetAveragePowerAtCrankFromElectrics
       Return _GetAveragePowerAtCrankFromElectrics
    End Function

    Public Function GetAveragePowerDemandAtAlternator() As Single Implements IM2_AverageElectricalLoadDemand.GetAveragePowerDemandAtAlternator
     Return _GetAveragePowerDemandAtAlternator
    End Function



Public Sub new ( GetAveragePowerAtCrankFromElectrics As Single,GetAveragePowerDemandAtAlternator As single )


 _GetAveragePowerAtCrankFromElectrics =  GetAveragePowerAtCrankFromElectrics
 _GetAveragePowerDemandAtAlternator   =  GetAveragePowerDemandAtAlternator  

End Sub


End Class

