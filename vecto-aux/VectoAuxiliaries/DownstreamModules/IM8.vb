
Imports VectoAuxiliaries.Electrics
Imports VectoAuxiliaries.Pneumatics
Imports VectoAuxiliaries.Hvac

Namespace DownstreamModules

Public Interface IM8


'OUT1
ReadOnly Property AuxPowerAtCrankFromElectricalHVACAndPneumaticsAncillaries As Single
'OUT2
ReadOnly Property SmartElectricalAlternatorPowerGenAtCrank As Single
'OUT3
ReadOnly Property CompressorFlag As Integer


End Interface


End Namespace


