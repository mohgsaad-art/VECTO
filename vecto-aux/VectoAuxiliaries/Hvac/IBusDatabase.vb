Namespace Hvac

Public Interface IBusDatabase


     Function  GetBuses( busModel as string ) As List(of IBus )

     Function Initialise( busFileCSV As String ) As Boolean

     
End Interface


End Namespace



