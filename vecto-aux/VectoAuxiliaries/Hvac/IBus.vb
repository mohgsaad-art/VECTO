
Namespace Hvac

  Public Interface IBus
   
        Readonly Property Model                  As String
        Readonly Property FloorType              As String
        Readonly Property EngineType             As String 
        Readonly Property LengthInMetres         As Single
        Readonly Property WidthInMetres          As Single
        Readonly Property HeightInMetres         As Single
        Readonly Property RegisteredPassengers   As integer
  
        Readonly Property AreaInMetresSquared    As Single
        Readonly Property VolumneInMetresQubed   As Single
     
  
  End Interface

End Namespace


