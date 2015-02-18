Namespace Hvac

   Public Class Bus
    Implements IBus

   
      'Private Property Backing
      private _model                    As String
      private _floorType                As String
      private _engineType               As String
      private _lengthInMetres           As String
      private _widthInMetres            As String
      private _heightInMetres           As String
      private _registeredPassengers     As Integer
   
      'Properties Set By construction only
      Public Readonly Property Model                  As String Implements IBus.Model
       Get
        Return _model
       End Get
   End Property
      Public Readonly Property FloorType              As String Implements IBus.FloorType
       Get
        Return _floorType
       End Get
   End Property
      Public Readonly Property EngineType             As String  Implements IBus.EngineType
       Get
        Return _engineType
       End Get
   End Property
      Public Readonly Property LengthInMetres         As Single Implements IBus.LengthInMetres
       Get
        Return _lengthInMetres
       End Get
   End Property
      Public Readonly Property WidthInMetres          As Single Implements IBus.WidthInMetres
       Get
        Return _widthInMetres
       End Get
   End Property
      Public Readonly Property HeightInMetres         As Single Implements IBus.HeightInMetres
       Get
        Return _heightInMetres
       End Get
   End Property
      Public Readonly Property RegisteredPassengers   As integer Implements IBus.RegisteredPassengers
       Get
        Return _registeredPassengers
       End Get
   End Property
     
      'Caculated Property
      Public Readonly Property AreaInMetresSquared As Single Implements IBus.AreaInMetresSquared
       Get
          Return Math.Round( 2*(LengthInMetres * WidthInMetres + WidthInMetres * HeightInMetres + LengthInMetres * HeightInMetres),1)
       End Get
      End Property
      Public Readonly Property VolumneInMetresQubed As Single Implements IBus.VolumneInMetresQubed
       Get
          Return Math.Round(LengthInMetres * WidthInMetres * HeightInMetres  ,1)
       End Get
      End Property
      
      'Constructors
      Public Sub new (_model                    As String,
                      _floorType                As String,
                      _engineType               As String,
                      _lengthInMetres           As String,
                      _widthInMetres            As String,
                      _heightInMetres           As String,
                      _registeredPassengers     As Integer)


        'Validity checks.
        If NOT ModelOK( _model )                     then Throw New ArgumentException("Model argument is invalid")
        If NOT FloorTypeOK( _floorType )             then Throw New ArgumentException("Model argument is invalid")
        If NOT EngineOK( _engineType )               then Throw New ArgumentException("EngineType argument is invalid")         
        If NOT DimensionOK(_lengthInMetres)          then Throw New ArgumentException("Invalid Length") 
        If NOT DimensionOK(_widthInMetres)           then Throw New ArgumentException("Invalid Width") 
        If NOT DimensionOK( _heightInMetres)         then Throw New ArgumentException("Invalid Height") 
        If NOT PassengersOK( _registeredPassengers ) then Throw New ArgumentException("Invalid Number Of Passengers") 


        'Set Private Members
        Me._model                       = _model
        Me._floorType                   = _floorType
        Me._engineType                  = _engineType
        Me._lengthInMetres              = _lengthInMetres
        Me._widthInMetres               = _widthInMetres
        Me._heightInMetres              = _heightInMetres
        Me._registeredPassengers        =_registeredPassengers              
                                                   
                                                  
   end sub                                    
                                                  
      'Construction Validators Helpers                                     
      Private Function ModelOK( ByVal model As String ) As Boolean
       
      model= model.ToLower

      If model is Nothing orelse model.Trim.Length=0 then Return False
   
      
      Return true

   End Function 
      Private Function FloorTypeOK( ByVal floorType As String ) As Boolean
       
      floorType= floorType.ToLower

      If floorType is Nothing orelse floorType.Trim.Length=0 then Return False
   
      If floorType<> "raised floor" AndAlso floorType<> "low floor" AndAlso floorType <> "semi low floor" then Return False
      
      Return true

   End Function
      Private Function EngineOK( ByVal engine As String ) As Boolean
       
      engine= engine.ToLower

      If engine is Nothing orelse engine.Trim.Length=0 then Return False
   
      If engine<> "diesel" AndAlso engine<> "gas" AndAlso engine <> "hybrid" then Return False
      
      Return true

   End Function
      Private Function DimensionOK( byval dimension as Single ) as boolean

      Return dimension> 0.5

   End Function
      Private Function PassengersOK( ByVal registeredPassengers As integer) As Boolean

     Return registeredPassengers>1 

      
   End Function
   


   End Class


End Namespace



