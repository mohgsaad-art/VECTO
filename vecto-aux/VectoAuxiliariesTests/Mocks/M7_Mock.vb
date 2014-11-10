
Imports VectoAuxiliaries.Electrics
Imports VectoAuxiliaries.Pneumatics
Imports VectoAuxiliaries.Hvac
Imports VectoAuxiliaries.DownstreamModules


Public Class M7_Mock
 Implements IM7

    Private _SmartElectricalAndPneumaticAuxAirCompPowerGenAtCrank As Single
    Private _SmartElectricalAndPneumaticAuxAltPowerGenAtCrank     As Single
    Private _SmartElectricalOnlyAuxAltPowerGenAtCrank             As Single
    Private _SmartPneumaticOnlyAuxAirCompPowerGenAtCrank          As Single
 
    Public ReadOnly Property SmartElectricalAndPneumaticAuxAirCompPowerGenAtCrank As Single Implements IM7.SmartElectricalAndPneumaticAuxAirCompPowerGenAtCrank
        Get
           Return _SmartElectricalAndPneumaticAuxAirCompPowerGenAtCrank
        End Get
    End Property

    Public ReadOnly Property SmartElectricalAndPneumaticAuxAltPowerGenAtCrank As Single Implements IM7.SmartElectricalAndPneumaticAuxAltPowerGenAtCrank
        Get
        Return _SmartElectricalAndPneumaticAuxAltPowerGenAtCrank
        End Get
    End Property

    Public ReadOnly Property SmartElectricalOnlyAuxAltPowerGenAtCrank As Single Implements IM7.SmartElectricalOnlyAuxAltPowerGenAtCrank
        Get
        Return _SmartElectricalOnlyAuxAltPowerGenAtCrank
        End Get
    End Property

    Public ReadOnly Property SmartPneumaticOnlyAuxAirCompPowerGenAtCrank As Single Implements IM7.SmartPneumaticOnlyAuxAirCompPowerGenAtCrank
        Get
         Return _SmartPneumaticOnlyAuxAirCompPowerGenAtCrank
        End Get
    End Property



    'Constructors
    public Sub new()

    End Sub

    Public Sub new (SmartElectricalAndPneumaticAuxAirCompPowerGenAtCrank As Single,
                    SmartElectricalAndPneumaticAuxAltPowerGenAtCrank     As Single, 
                    SmartElectricalOnlyAuxAltPowerGenAtCrank             As Single,         
                    SmartPneumaticOnlyAuxAirCompPowerGenAtCrank          As Single)  

                    _SmartElectricalAndPneumaticAuxAirCompPowerGenAtCrank = SmartElectricalAndPneumaticAuxAirCompPowerGenAtCrank
                    _SmartElectricalAndPneumaticAuxAltPowerGenAtCrank     = SmartElectricalAndPneumaticAuxAltPowerGenAtCrank
                    _SmartElectricalOnlyAuxAltPowerGenAtCrank             = SmartElectricalOnlyAuxAltPowerGenAtCrank
                    _SmartPneumaticOnlyAuxAirCompPowerGenAtCrank          = SmartPneumaticOnlyAuxAirCompPowerGenAtCrank
                                                                    
                                                                            
    End Sub                                                              
            
                         
End Class 