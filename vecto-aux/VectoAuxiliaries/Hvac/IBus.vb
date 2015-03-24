
Namespace Hvac

  Public Interface IBus
   
        Readonly Property Model                  As String
        Readonly Property FloorType              As String
        Readonly Property EngineType             As String 
        Readonly Property LengthInMetres         As Double
        Readonly Property WidthInMetres          As Double
        Readonly Property HeightInMetres         As Double
        Readonly Property RegisteredPassengers   As integer
  
        Readonly Property AreaInMetresSquared    As Double
        Readonly Property VolumneInMetresQubed   As Double
     
  
  End Interface

End Namespace


