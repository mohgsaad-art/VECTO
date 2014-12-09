
Imports VectoAuxiliaries.Hvac
Imports VectoAuxiliaries.Electrics
Imports VectoAuxiliaries.Pneumatics

Namespace Hvac

Public Class HVACUserInputsConfig
Implements IHVACUserInputsConfig



        Public Property  SteadyStateModel As IHVACSteadyStateModel Implements IHVACUserInputsConfig.SteadyStateModel
        Public Property SSMFilePath As String Implements IHVACUserInputsConfig.SSMFilePath

        Public Sub new (ssm As IHVACSteadyStateModel, ssmFilePath As string)

        SteadyStateModel           = ssm
        Me.SSMFilePath             = ssmFilePath

        End Sub

End Class

End Namespace



