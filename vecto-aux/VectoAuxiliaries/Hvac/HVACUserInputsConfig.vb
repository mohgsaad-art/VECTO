
Imports AdvancedAuxiliaryInterfaces.Hvac
Imports AdvancedAuxiliaryInterfaces.Electrics
Imports AdvancedAuxiliaryInterfaces.Pneumatics

Namespace Hvac

Public Class HVACUserInputsConfig
Implements IHVACUserInputsConfig


        Public Property  SteadyStateModel As IHVACSteadyStateModel Implements IHVACUserInputsConfig.SteadyStateModel

        Public Sub new (ssm As IHVACSteadyStateModel)


        SteadyStateModel           = ssm

        End Sub


End Class

End Namespace



