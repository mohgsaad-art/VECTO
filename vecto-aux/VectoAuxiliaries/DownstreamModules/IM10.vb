Imports VectoAuxiliaries.Electrics
Imports VectoAuxiliaries.Pneumatics
Imports VectoAuxiliaries.Hvac
Imports VectoAuxiliaries.DownstreamModules

Namespace DownstreamModules



Public Interface IM10

'Interpolated FC between points 2-1 Representing non-smart Pneumatics = BaseFuel Consumption with average auxillary loads
ReadOnly Property InterFCNonSmartPneumatics As Single

'Interpolated FC between points 2-3-1 Representing smart Pneumatics = Fuel consumption with smart Pneumatics and average electrical  power demand
ReadOnly Property InterFCSmartPneumatics As Single

End Interface



End Namespace

