Imports VectoAuxiliaries.Electrics
Imports VectoAuxiliaries.Pneumatics
Imports VectoAuxiliaries.Hvac
Imports VectoAuxiliaries.DownstreamModules

Public Class M5_Mock
 Implements IM5_SmartAlternatorSetGeneration


 public property _AlternatorsGenerationPowerAtCrankIdleWatts          as single
 public property _AlternatorsGenerationPowerAtCrankOverrunWatts       as single
 public property _AlternatorsGenerationPowerAtCrankTractionOnWatts    as single


    Public Function AlternatorsGenerationPowerAtCrankIdleWatts() As Single Implements IM5_SmartAlternatorSetGeneration.AlternatorsGenerationPowerAtCrankIdleWatts
       Return _AlternatorsGenerationPowerAtCrankIdleWatts
    End Function

    Public Function AlternatorsGenerationPowerAtCrankOverrunWatts() As Single Implements IM5_SmartAlternatorSetGeneration.AlternatorsGenerationPowerAtCrankOverrunWatts
       Return _AlternatorsGenerationPowerAtCrankOverrunWatts
    End Function

    Public Function AlternatorsGenerationPowerAtCrankTractionOnWatts() As Single Implements IM5_SmartAlternatorSetGeneration.AlternatorsGenerationPowerAtCrankTractionOnWatts
     Return _AlternatorsGenerationPowerAtCrankTractionOnWatts
    End Function


 Public Sub new ()

 End Sub


   Public Sub new ( AlternatorsGenerationPowerAtCrankIdleWatts          as single,
                    AlternatorsGenerationPowerAtCrankOverrunWatts       as single,
                    AlternatorsGenerationPowerAtCrankTractionOnWatts    as single)

         _AlternatorsGenerationPowerAtCrankIdleWatts          = AlternatorsGenerationPowerAtCrankIdleWatts
         _AlternatorsGenerationPowerAtCrankOverrunWatts       = AlternatorsGenerationPowerAtCrankOverrunWatts
         _AlternatorsGenerationPowerAtCrankTractionOnWatts    = AlternatorsGenerationPowerAtCrankTractionOnWatts       
                                                    

   End Sub


End Class

