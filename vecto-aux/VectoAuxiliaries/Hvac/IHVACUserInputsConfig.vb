
Imports VectoAuxiliaries.Pneumatics
Imports VectoAuxiliaries.Electrics
Imports VectoAuxiliaries.Hvac

Namespace Hvac

Public Interface IHVACUserInputsConfig


  property  _hvacInputs as IHVACInputs
  Property  _hvacMapPath As string
  Property  _compressorGearRatio As Single
  Property  _compressorGearEfficiency As Single
  Property  _steadyStateModel As IHVACSteadyStateModel



End Interface




End Namespace


