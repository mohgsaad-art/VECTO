
Namespace Hvac

    Public Interface IBus

        ReadOnly Property Id As Integer

        Property Model As String
        Property FloorType As String
        Property EngineType As String
        Property LengthInMetres As Double
        Property WidthInMetres As Double
        Property HeightInMetres As Double
        Property RegisteredPassengers As Integer
        Property IsDoubleDecker As Boolean

    End Interface

End Namespace


