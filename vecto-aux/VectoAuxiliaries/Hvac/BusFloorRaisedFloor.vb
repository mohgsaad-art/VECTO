
Namespace Hvac

  Public Class BusFloorRaisedFloor
   inherits BusFloorBase
  
  
    Sub new ()
    
        MyBase.FloorType= "raised floor"
    
    End Sub
    
    Sub new ( H As Double, V As Double, C As Double  ) 
    
       Me.New()
       
       MyBase.H=H
       MyBase.V=V
       MyBase.C=C
    
    
    End Sub
    
  
  End Class

End Namespace



