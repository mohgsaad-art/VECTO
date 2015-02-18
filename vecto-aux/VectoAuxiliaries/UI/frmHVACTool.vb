
Imports System.Windows.Forms
Imports VectoAuxiliaries.Hvac



Public Class frmHVACTool

Private filePath As string

Private buses As IBusDatabase



Public Sub new ( filePath As String )

    ' This call is required by the designer.
    InitializeComponent()
    
    ' Add any initialization after the InitializeComponent() call.

    Me.filePath = filePath


    Initlialise()

    
End Sub


Private Sub Initlialise()

  'Instantial Buses
  buses = New BusDatabase()
  If Not buses.Initialise( filePath ) then 
    MessageBox.Show("Problems initialising the Bus Database, some buses may not appear")
  End If

   


  cboBuses.DataSource=buses.GetBuses(String.Empty,True)
  cboBuses.DisplayMember="Model"


End Sub






End Class



