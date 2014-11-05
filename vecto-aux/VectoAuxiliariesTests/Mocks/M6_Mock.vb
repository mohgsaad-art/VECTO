
Imports VectoAuxiliaries.Electrics
Imports VectoAuxiliaries.Pneumatics
Imports VectoAuxiliaries.Hvac
Imports VectoAuxiliaries.DownstreamModules


Public Class M6_Mock
 Implements IM6


Public Property _AveragePowerDemandAtCrankFromPneumatics       As Single
public property _AvgPowerDemandAtCrankFromElectricsIncHVAC     As single
public property _OverrunFlag                                   As integer
public property _SmartElecAndPneumaticAirCompPowerGenAtCrank   As single
public property _SmartElecAndPneumaticAltPowerGenAtCrank       As single
public property _SmartElecAndPneumaticsCompressorFlag          As integer
public property _SmartElecOnlyAltPowerGenAtCrank               As single
public property _SmartPneumaticOnlyAirCompPowerGenAtCrank      As single
public property _SmartPneumaticsOnlyCompressorFlag             As integer


    Public ReadOnly Property AveragePowerDemandAtCrankFromPneumatics As Single Implements IM6.AveragePowerDemandAtCrankFromPneumatics
        Get
        Return _AveragePowerDemandAtCrankFromPneumatics
        End Get

    End Property
    Public ReadOnly Property AvgPowerDemandAtCrankFromElectricsIncHVAC As Single Implements IM6.AvgPowerDemandAtCrankFromElectricsIncHVAC
        Get
        Return _AvgPowerDemandAtCrankFromElectricsIncHVAC
        End Get
    End Property
    Public ReadOnly Property OverrunFlag As Integer Implements IM6.OverrunFlag
        Get
        Return OverrunFlag
        End Get
    End Property
    Public ReadOnly Property SmartElecAndPneumaticAirCompPowerGenAtCrank As Single Implements IM6.SmartElecAndPneumaticAirCompPowerGenAtCrank
        Get
        Return _SmartElecAndPneumaticAirCompPowerGenAtCrank
        End Get
    End Property
    Public ReadOnly Property SmartElecAndPneumaticAltPowerGenAtCrank As Single Implements IM6.SmartElecAndPneumaticAltPowerGenAtCrank
        Get
         Return _SmartElecAndPneumaticAltPowerGenAtCrank
        End Get
    End Property
    Public ReadOnly Property SmartElecAndPneumaticsCompressorFlag As Integer Implements IM6.SmartElecAndPneumaticsCompressorFlag
        Get
          Return _SmartElecAndPneumaticsCompressorFlag
        End Get
    End Property
    Public ReadOnly Property SmartElecOnlyAltPowerGenAtCrank As Single Implements IM6.SmartElecOnlyAltPowerGenAtCrank
        Get
        Return _SmartElecOnlyAltPowerGenAtCrank
        End Get
    End Property
    Public ReadOnly Property SmartPneumaticOnlyAirCompPowerGenAtCrank As Single Implements IM6.SmartPneumaticOnlyAirCompPowerGenAtCrank
        Get
          return _SmartPneumaticOnlyAirCompPowerGenAtCrank
        End Get
    End Property
    Public ReadOnly Property SmartPneumaticsOnlyCompressorFlag As Integer Implements IM6.SmartPneumaticsOnlyCompressorFlag
        Get
         Return _SmartPneumaticsOnlyCompressorFlag
        End Get
    End Property


    Public Sub new()

    End Sub

    Public Sub new(_AveragePowerDemandAtCrankFromPneumatics       As Single , _
                   _AvgPowerDemandAtCrankFromElectricsIncHVAC     As single , _
                   _OverrunFlag                                   As integer, _
                   _SmartElecAndPneumaticAirCompPowerGenAtCrank   As single , _
                   _SmartElecAndPneumaticAltPowerGenAtCrank       As single , _
                   _SmartElecAndPneumaticsCompressorFlag          As integer, _
                   _SmartElecOnlyAltPowerGenAtCrank               As single , _
                   _SmartPneumaticOnlyAirCompPowerGenAtCrank      As single , _
                   _SmartPneumaticsOnlyCompressorFlag             As integer)


       _AveragePowerDemandAtCrankFromPneumatics     = AveragePowerDemandAtCrankFromPneumatics
       _AvgPowerDemandAtCrankFromElectricsIncHVAC   = AvgPowerDemandAtCrankFromElectricsIncHVAC
       _OverrunFlag                                 = OverrunFlag
       _SmartElecAndPneumaticAirCompPowerGenAtCrank = SmartElecAndPneumaticAirCompPowerGenAtCrank
       _SmartElecAndPneumaticAltPowerGenAtCrank     = SmartElecAndPneumaticAltPowerGenAtCrank
       _SmartElecAndPneumaticsCompressorFlag        = SmartElecAndPneumaticsCompressorFlag
       _SmartElecOnlyAltPowerGenAtCrank             = SmartElecOnlyAltPowerGenAtCrank
       _SmartPneumaticOnlyAirCompPowerGenAtCrank    = SmartPneumaticOnlyAirCompPowerGenAtCrank
       _SmartPneumaticsOnlyCompressorFlag           = SmartPneumaticsOnlyCompressorFlag
                                                         
                                                           
                                                                   
    End Sub                                               
                                                                 

End Class

