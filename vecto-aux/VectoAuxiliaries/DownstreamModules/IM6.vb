
'Module 6: OVER-RUN smart and non-smart systems alternator and air compressor load calculations

Namespace DownstreamModules


Public Interface IM6

'Watts
 Readonly Property  OverrunFlag As Integer
 Readonly Property  SmartElecAndPneumaticsCompressorFlag As integer
 ReadOnly Property  SmartElecAndPneumaticAltPowerGenAtCrank As Single
 ReadOnly Property  SmartElecAndPneumaticAirCompPowerGenAtCrank As Single
 ReadOnly Property  SmartElecOnlyAltPowerGenAtCrank As Single
 ReadOnly Property  AveragePowerDemandAtCrankFromPneumatics As Single
 ReadOnly Property  SmartPneumaticOnlyAirCompPowerGenAtCrank As Single
                   
 ReadOnly Property  AvgPowerDemandAtCrankFromElectricsIncHVAC As Single
 ReadOnly Property  SmartPneumaticsOnlyCompressorFlag As Integer
 


End Interface


End Namespace


