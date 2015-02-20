
Namespace Hvac

Public Class BusFloorLow
 Inherits BusFloorBase

Sub new ()

    MyBase.FloorType= "low floor"

End Sub

Sub new ( H As Double, V As Double, C As Double  ) 

   Me.New()
   
   MyBase.H=H
   MyBase.V=V
   MyBase.C=C


End Sub


End Class

End Namespace



