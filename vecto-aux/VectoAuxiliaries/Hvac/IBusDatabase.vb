Namespace Hvac

Public Interface IBusDatabase


     Function  GetBuses( busModel as string, Optional AsSelectList As Boolean=false ) As List(of IBus )

     Function Initialise( busFileCSV As String ) As Boolean

     
End Interface


End Namespace



