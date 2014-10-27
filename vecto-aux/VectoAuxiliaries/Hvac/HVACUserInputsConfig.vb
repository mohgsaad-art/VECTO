
Imports VectoAuxiliaries.Hvac
Imports VectoAuxiliaries.Electrics
Imports VectoAuxiliaries.Pneumatics

Namespace Hvac

Public Class HVACUserInputsConfig
Implements IHVACUserInputsConfig


        Public Property _compressorGearEfficiency As Single Implements IHVACUserInputsConfig._compressorGearEfficiency
        Public Property _compressorGearRatio As Single Implements IHVACUserInputsConfig._compressorGearRatio
        Public Property _hvacInputs As IHVACInputs Implements IHVACUserInputsConfig._hvacInputs
        Public Property _hvacMapPath As String Implements IHVACUserInputsConfig._hvacMapPath
        Public Property _steadyStateModel As IHVACSteadyStateModel Implements IHVACUserInputsConfig._steadyStateModel

        Public Sub new (compressorGearEfficiency As single,compressorGearRatio As Single ,hvacInputs as IHVACInputs  ,hvacMapPath As string, ssm As IHVACSteadyStateModel)

        'Sanity Check
        if hvacMapPath.Trim().Length=0 then Throw New ArgumentException("hvacMap Path must have a string to represent the csv file location")

        'Assign
        _compressorGearEfficiency   =  compressorGearEfficiency
        _compressorGearRatio        =  compressorGearRatio
        _hvacInputs                 =  hvacInputs
        _hvacMapPath                =  hvacMapPath
        _steadyStateModel           = ssm

        End Sub


End Class

End Namespace



