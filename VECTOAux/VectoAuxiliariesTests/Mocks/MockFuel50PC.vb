
Imports TUGraz.VectoCommon.BusAuxiliaries
Imports TUGraz.VectoCommon.Models
Imports TUGraz.VectoCommon.Utils

Public Class MockFuel50PC
	Implements IFuelConsumptionMap

	

	Public Function fFCdelaunay_Intp(nU As Double, Tq As Double) As Double

		Return (nU + Tq) * 0.5
	End Function


	Public Function GetFuelConsumption(torque As NewtonMeter, angularVelocity As PerSecond) As KilogramPerSecond _
		Implements IFuelConsumptionMap.GetFuelConsumptionValue
		Return (fFCdelaunay_Intp(angularVelocity.AsRPM, torque.Value()) / 3600 / 1000).SI(Of KilogramPerSecond)()
	End Function


    Public Function IFuelConsumptionMap_GetFuelConsumption(currentStateEngineTorque As NewtonMeter, avgEngineSpeed As PerSecond, Optional allowExtrapolation As Boolean = False) As FuelConsumptionResult Implements IFuelConsumptionMap.GetFuelConsumption
        Throw New NotImplementedException
    End Function
End Class

