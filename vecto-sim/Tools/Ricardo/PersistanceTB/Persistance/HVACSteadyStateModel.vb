
Namespace Hvac

<Serializable()>
Public Class HVACSteadyStateModel
implements IHVACSteadyStateModel

Public Property HVACElectricalLoadPowerWatts As Single Implements IHVACSteadyStateModel.HVACElectricalLoadPowerWatts
Public Property HVACFuellingLitresPerHour As Single Implements IHVACSteadyStateModel.HVACFuellingLitresPerHour
Public Property HVACMechanicalLoadPowerWatts As Single Implements IHVACSteadyStateModel.HVACMechanicalLoadPowerWatts


Public Sub new ()


End Sub


Public Sub new( elecPowerW As Single, mechPowerW As Single, fuellingLPH As single)

     HVACElectricalLoadPowerWatts=elecPowerW   
     HVACFuellingLitresPerHour =mechPowerW
     HVACMechanicalLoadPowerWatts=fuellingLPH

End Sub


End Class


End Namespace



