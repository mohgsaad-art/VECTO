
Imports TUGraz.VectoCommon.Utils
Imports TUGraz.VectoCore.Models.BusAuxiliaries.Interfaces.DownstreamModules

Public Class M2_Mock
	Implements IM2_AverageElectricalLoadDemand

	Public _GetAveragePowerAtCrankFromElectrics As Watt
	Public _GetAveragePowerDemandAtAlternator As Watt


	Public Function GetAveragePowerAtCrankFromElectrics() As Watt _
		Implements IM2_AverageElectricalLoadDemand.GetAveragePowerAtCrankFromElectrics
		Return _GetAveragePowerAtCrankFromElectrics
	End Function

    Public ReadOnly Property AveragePowerDemandAtAlternatorFromElectrics As Watt Implements IM2_AverageElectricalLoadDemand.AveragePowerDemandAtAlternatorFromElectrics

    'Public Function GetAveragePowerDemandAtAlternator() As Watt _
	'	Implements IM2_AverageElectricalLoadDemand.GetAveragePowerDemandAtAlternator
	'	Return _GetAveragePowerDemandAtAlternator
	'End Function


	Public Sub New()
	End Sub


	Public Sub New(GetAveragePowerAtCrankFromElectrics As Double, GetAveragePowerDemandAtAlternator As Double)


		_GetAveragePowerAtCrankFromElectrics = GetAveragePowerAtCrankFromElectrics.SI(Of Watt)()
		_GetAveragePowerDemandAtAlternator = GetAveragePowerDemandAtAlternator.SI(Of Watt)()
	End Sub

    Public Sub ResetCalculations() Implements IAbstractModule.ResetCalculations
        Throw New NotImplementedException
    End Sub
End Class

