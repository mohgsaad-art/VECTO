
Imports System.Windows.Forms
Imports VectoAuxiliaries.Hvac



Public Class frmHVACTool

Private busDatabasePath As string
Private ahsmFilePath As String


Private buses As IBusDatabase

Private ssmTOOL As SSMTOOL


Public Sub new ( busDatabasePath As String, ahsmFilePath As String )

    ' This call is required by the designer.
    InitializeComponent()
    
    ' Add any initialization after the InitializeComponent() call.
    Me.busDatabasePath =  busDatabasePath
    Me.ahsmFilePath    = ahsmFilePath

    ssmTOOL = New SSMTOOL(ahsmFilePath)

    setupBuses()
    setupControls()
    setupBindings()
    
End Sub




Private Sub setupBuses()

    'Setup Buses
    buses = New BusDatabase()
    If Not buses.Initialise(  busDatabasePath ) then 
      MessageBox.Show("Problems initialising the Bus Database, some buses may not appear")
    End If

    cboBuses.DataSource=buses.GetBuses(String.Empty,True)
    cboBuses.DisplayMember="Model"

End Sub


private Sub setupControls()


End Sub



private Sub setupBindings()

  'BParameterisation
  txtBusFloorSurfaceArea.DataBindings.Add("Text", ssmTOOL.genInputs,"BP_BusFloorSurfaceArea",False,DataSourceUpdateMode.OnPropertyChanged)


End Sub


'GeneralInputControlEvents


Private Sub cboBuses_SelectedIndexChanged( sender As Object,  e As EventArgs) Handles cboBuses.SelectedIndexChanged

  If cboBuses.SelectedIndex>0 then


      dim bus As IBus = DirectCast(cboBuses.SelectedItem, IBus)

      ssmTOOL.genInputs.BP_BusModel= bus.Model
      ssmTOOL.genInputs.BP_NumberOfPassengers= bus.RegisteredPassengers
      ssmTOOL.genInputs.BP_BusFloorType       = bus.FloorType
      'ssmTOOL.genInputs.BP_BusFloorSurfaceArea Calculated
      ssmTOOL.genInputs.BP_BusSurfaceAreaM2 = bus.AreaInMetresSquared
      'ssmTOOL.genInputs.BP_BusWindowSurface Calculated
      ssmTOOL.genInputs.BP_BusVolume= bus.VolumneInMetresQubed
      ssmTOOL.genInputs.BP_BusLength= bus.LengthInMetres
      ssmTOOL.genInputs.BP_BusWidth= bus.WidthInMetres




  End If


End Sub








End Class
