
Imports TUGraz.VectoCommon.Utils
Imports TUGraz.VectoCore.Models.BusAuxiliaries.Interfaces.DownstreamModules

Public Class M5_Mock
	Implements IM5_SmartAlternatorSetGeneration

	Public Property _AlternatorsGenerationPowerAtCrankIdleWatts As Watt
	Public Property _AlternatorsGenerationPowerAtCrankOverrunWatts As Watt
	Public Property _AlternatorsGenerationPowerAtCrankTractionOnWatts As Watt


	Public Function AlternatorsGenerationPowerAtCrankIdle() As Watt _
		Implements IM5_SmartAlternatorSetGeneration.AlternatorsGenerationPowerAtCrankIdle
		Return _AlternatorsGenerationPowerAtCrankIdleWatts
	End Function

	Public Function AlternatorsGenerationPowerAtCrankOverrun() As Watt _
		Implements IM5_SmartAlternatorSetGeneration.AlternatorsGenerationPowerAtCrankOverrun
		Return _AlternatorsGenerationPowerAtCrankOverrunWatts
	End Function

	Public Function AlternatorsGenerationPowerAtCrankTractionOn() As Watt _
		Implements IM5_SmartAlternatorSetGeneration.AlternatorsGenerationPowerAtCrankTractionOn
		Return _AlternatorsGenerationPowerAtCrankTractionOnWatts
	End Function


	Public Sub New()
	End Sub


	Public Sub New(AlternatorsGenerationPowerAtCrankIdleWatts As Double,
					AlternatorsGenerationPowerAtCrankOverrunWatts As Double,
					AlternatorsGenerationPowerAtCrankTractionOnWatts As Double)

		_AlternatorsGenerationPowerAtCrankIdleWatts = AlternatorsGenerationPowerAtCrankIdleWatts.SI(Of Watt)()
		_AlternatorsGenerationPowerAtCrankOverrunWatts = AlternatorsGenerationPowerAtCrankOverrunWatts.SI(Of Watt)()
		_AlternatorsGenerationPowerAtCrankTractionOnWatts = AlternatorsGenerationPowerAtCrankTractionOnWatts.SI(Of Watt)()
	End Sub

    Public Sub ResetCalculations() Implements IAbstractModule.ResetCalculations
        Throw New NotImplementedException
    End Sub
End Class

