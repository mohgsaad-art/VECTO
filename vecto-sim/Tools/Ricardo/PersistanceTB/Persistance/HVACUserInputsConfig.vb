
Imports VectoAuxiliaries.Hvac
Imports VectoAuxiliaries.Electrics
Imports VectoAuxiliaries.Pneumatics

Namespace Hvac

<Serializable()>
Public Class HVACUserInputsConfig
Implements IHVACUserInputsConfig


        Public Property  SteadyStateModel As IHVACSteadyStateModel Implements IHVACUserInputsConfig.SteadyStateModel

        Public Sub new (ssm As IHVACSteadyStateModel)


        SteadyStateModel           = ssm

        End Sub

        Public Sub new()

        End Sub



End Class

End Namespace



