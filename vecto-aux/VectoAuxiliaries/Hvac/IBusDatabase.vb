Namespace Hvac

    Public Interface IBusDatabase

        Function AddBus(bus As IBus) As Boolean

        Function GetBuses(busModel As String, Optional AsSelectList As Boolean = False) As List(Of IBus)

        Function Initialise(busFileCSV As String) As Boolean

        Function UpdateBus(id As Integer, bus As IBus) As Boolean

        Function Save(filepath As String) As Boolean

    End Interface

End Namespace