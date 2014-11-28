
Imports AdvancedAuxiliaryInterfaces.Electrics
Imports AdvancedAuxiliaryInterfaces.Pneumatics
Imports AdvancedAuxiliaryInterfaces.Hvac

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


