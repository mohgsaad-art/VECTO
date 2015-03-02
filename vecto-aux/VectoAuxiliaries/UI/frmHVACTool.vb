Imports System.Windows.Forms
Imports VectoAuxiliaries.Hvac
Imports System.ComponentModel
Imports System.Drawing

Public Class frmHVACTool

  'Fields
  Private busDatabasePath As String
  Private ahsmFilePath As String
  Private buses As IBusDatabase
  Private ssmTOOL As SSMTOOL
  Private TabColors As Dictionary(Of TabPage, Color) = New Dictionary(Of TabPage, Color)()



  Public Sub New(busDatabasePath As String, ahsmFilePath As String)

    ' This call is required by the designer.
    InitializeComponent()

    ' Add any initialization after the InitializeComponent() call.
    Me.busDatabasePath = busDatabasePath
    Me.ahsmFilePath = ahsmFilePath

    ssmTOOL = New SSMTOOL(ahsmFilePath)

    setupBuses()
    setupControls()
    setupBindings()

End Sub

  'Setup Methods
  Private Sub setupBuses()

    'Setup Buses
    buses = New BusDatabase()
    If Not buses.Initialise(busDatabasePath) Then
      MessageBox.Show("Problems initialising the Bus Database, some buses may not appear")
    End If

    cboBuses.DataSource = buses.GetBuses(String.Empty, True)
    cboBuses.DisplayMember = "Model"

End Sub
  Private Sub setupControls()


End Sub
  Private Sub setupBindings()

  'Bus Parameterisation
  txtBusModel.DataBindings.Add("Text", ssmTOOL.genInputs, "BP_BusModel", False, DataSourceUpdateMode.OnPropertyChanged)
  txtRegisteredPassengers.DataBindings.Add("Text", ssmTOOL.genInputs, "BP_NumberOfPassengers", False, DataSourceUpdateMode.OnPropertyChanged)
  txtBusFloorType.DataBindings.Add("Text", ssmTOOL.genInputs, "BP_BusFloorType", False, DataSourceUpdateMode.OnPropertyChanged)
  txtBusFloorSurfaceArea.DataBindings.Add("Text", ssmTOOL.genInputs, "BP_BusFloorSurfaceArea", False, DataSourceUpdateMode.OnPropertyChanged)
  txtBusSurfaceArea.DataBindings.Add("Text", ssmTOOL.genInputs, "BP_BusSurfaceAreaM2", False, DataSourceUpdateMode.OnPropertyChanged)
  txtBusWindowSurfaceArea.DataBindings.Add("Text", ssmTOOL.genInputs, "BP_BusWindowSurface", False, DataSourceUpdateMode.OnPropertyChanged)
  txtBusVolume.DataBindings.Add("Text", ssmTOOL.genInputs, "BP_BusVolume", False, DataSourceUpdateMode.OnPropertyChanged)
  txtBusLength.DataBindings.Add("Text", ssmTOOL.genInputs, "BP_BusLength", False, DataSourceUpdateMode.OnPropertyChanged)
  txtBusWidth.DataBindings.Add("Text", ssmTOOL.genInputs, "BP_BusWidth", False, DataSourceUpdateMode.OnPropertyChanged)

  'Boundary Conditions
  txtBC_GFactor				                .DataBindings.Add("Text",ssmTool.genInputs,"BC_GFactor"				                 ,False,DataSourceUpdateMode.OnPropertyChanged)   
  txtBC_SolarClouding				        .DataBindings.Add("Text",ssmTool.genInputs,"BC_SolarClouding"				         ,False,DataSourceUpdateMode.OnPropertyChanged) 
  txtBC_HeatPerPassengerIntoCabinW	        .DataBindings.Add("Text",ssmTool.genInputs,"BC_HeatPerPassengerIntoCabinW"	         ,False,DataSourceUpdateMode.OnPropertyChanged)
  txtBC_PassengerBoundaryTemperature        .DataBindings.Add("Text",ssmTool.genInputs,"BC_PassengerBoundaryTemperature"         ,False,DataSourceUpdateMode.OnPropertyChanged)
  txtBC_PassengerDensityLowFloor            .DataBindings.Add("Text",ssmTool.genInputs,"BC_PassengerDensityLowFloor"             ,False,DataSourceUpdateMode.OnPropertyChanged)
  txtBC_PassengerDensitySemiLowFloor	    .DataBindings.Add("Text",ssmTool.genInputs,"BC_PassengerDensitySemiLowFloor"	     ,False,DataSourceUpdateMode.OnPropertyChanged) 
  txtBC_PassengerDensityRaisedFloor	        .DataBindings.Add("Text",ssmTool.genInputs,"BC_PassengerDensityRaisedFloor"	         ,False,DataSourceUpdateMode.OnPropertyChanged)   
  txtBC_CalculatedPassengerNumber	        .DataBindings.Add("Text",ssmTool.genInputs,"BC_CalculatedPassengerNumber"	         ,False,DataSourceUpdateMode.OnPropertyChanged) 
  txtBC_UValues                             .DataBindings.Add("Text",ssmTool.genInputs,"BC_UValues"                              ,False,DataSourceUpdateMode.OnPropertyChanged)
  txtBC_HeatingBoundaryTemperature	        .DataBindings.Add("Text",ssmTool.genInputs,"BC_HeatingBoundaryTemperature"	         ,False,DataSourceUpdateMode.OnPropertyChanged)
  txtBC_CoolingBoundaryTemperature          .DataBindings.Add("Text",ssmTool.genInputs,"BC_CoolingBoundaryTemperature"           ,False,DataSourceUpdateMode.OnPropertyChanged)
  txtBC_HighVentilation                     .DataBindings.Add("Text",ssmTool.genInputs,"BC_HighVentilation"                      ,False,DataSourceUpdateMode.OnPropertyChanged)
  txtBC_lowVentilation	                    .DataBindings.Add("Text",ssmTool.genInputs,"BC_lowVentilation"	                     ,False,DataSourceUpdateMode.OnPropertyChanged)
  txtBC_High                                .DataBindings.Add("Text",ssmTool.genInputs,"BC_High"                                 ,False,DataSourceUpdateMode.OnPropertyChanged)
  txtBC_Low	                                .DataBindings.Add("Text",ssmTool.genInputs,"BC_Low"	                                 ,False,DataSourceUpdateMode.OnPropertyChanged)   
  txtBC_HighVentPowerW                      .DataBindings.Add("Text",ssmTool.genInputs,"BC_HighVentPowerW"                       ,False,DataSourceUpdateMode.OnPropertyChanged)
  txtBC_LowVentPowerW                       .DataBindings.Add("Text",ssmTool.genInputs,"BC_LowVentPowerW"                        ,False,DataSourceUpdateMode.OnPropertyChanged)
  txtBC_SpecificVentilationPower            .DataBindings.Add("Text",ssmTool.genInputs,"BC_SpecificVentilationPower"             ,False,DataSourceUpdateMode.OnPropertyChanged)
  txtBC_COP			                        .DataBindings.Add("Text",ssmTool.genInputs,"BC_COP"			                         ,False,DataSourceUpdateMode.OnPropertyChanged)   
  txtBC_AuxHeaterEfficiency		            .DataBindings.Add("Text",ssmTool.genInputs,"BC_AuxHeaterEfficiency"		             ,False,DataSourceUpdateMode.OnPropertyChanged)   
  txtBC_GCVDieselOrHeatingOil               .DataBindings.Add("Text",ssmTool.genInputs,"BC_GCVDieselOrHeatingOil"                ,False,DataSourceUpdateMode.OnPropertyChanged)
  txtBC_VolumicMassDieselOrHeatingOil	    .DataBindings.Add("Text",ssmTool.genInputs,"BC_VolumicMassDieselOrHeatingOil"	     ,False,DataSourceUpdateMode.OnPropertyChanged) 
  txtBC_WindowAreaPerUnitBusLength	        .DataBindings.Add("Text",ssmTool.genInputs,"BC_WindowAreaPerUnitBusLength"	         ,False,DataSourceUpdateMode.OnPropertyChanged)
  txtBC_FrontRearWindowArea                 .DataBindings.Add("Text",ssmTool.genInputs,"BC_FrontRearWindowArea"                  ,False,DataSourceUpdateMode.OnPropertyChanged)
  txtBC_MaxTemperatureDeltaForLowFloorBusses.DataBindings.Add("Text",ssmTool.genInputs,"BC_MaxTemperatureDeltaForLowFloorBusses" ,False,DataSourceUpdateMode.OnPropertyChanged)
  txtBC_MaxPossibleBenefitFromTechnologyList.DataBindings.Add("Text",ssmTool.genInputs,"BC_MaxPossibleBenefitFromTechnologyList" ,False,DataSourceUpdateMode.OnPropertyChanged)

  'General Inputs Other   
  'EnviromentalConditions	        		
  txtEC_EnviromentalTemperature                         .DataBindings.Add("Text",ssmTool.genInputs,"EC_EnviromentalTemperature"                         ,False,DataSourceUpdateMode.OnPropertyChanged)
  txtEC_Solar   	                                    .DataBindings.Add("Text",ssmTool.genInputs,"EC_Solar"                                           ,False,DataSourceUpdateMode.OnPropertyChanged) 	                                         			                                     

  'AC-system	
  chkAC_InCabinRoomAC_System	                        .DataBindings.Add("Checked",ssmTool.genInputs,"AC_InCabinRoomAC_System"	                        ,False,DataSourceUpdateMode.OnPropertyChanged)
  cboAC_CompressorType			                        .DataBindings.Add("Text",ssmTool.genInputs,"AC_CompressorType"		                            ,False,DataSourceUpdateMode.OnPropertyChanged)
  txtAC_CompressorCapacitykW                            .DataBindings.Add("Text",ssmTool.genInputs,"AC_CompressorCapacitykW"	                        ,False,DataSourceUpdateMode.OnPropertyChanged) 				

  'Ventilation	
  chkVEN_VentilationOnDuringHeating				        .DataBindings.Add("Checked",ssmTool.genInputs,"VEN_VentilationOnDuringHeating"				    ,False,DataSourceUpdateMode.OnPropertyChanged) 
  chkVEN_VentilationWhenBothHeatingAndACInactive		.DataBindings.Add("Checked",ssmTool.genInputs,"VEN_VentilationWhenBothHeatingAndACInactive"	    ,False,DataSourceUpdateMode.OnPropertyChanged) 
  chkVEN_VentilationDuringAC			                .DataBindings.Add("Checked",ssmTool.genInputs,"VEN_VentilationDuringAC"			                ,False,DataSourceUpdateMode.OnPropertyChanged) 
  cboVEN_VentilationFlowSettingWhenHeatingAndACInactive .DataBindings.Add("Text",ssmTool.genInputs,"VEN_VentilationFlowSettingWhenHeatingAndACInactive" ,False,DataSourceUpdateMode.OnPropertyChanged)
  cboVEN_VentilationDuringHeating			            .DataBindings.Add("Text",ssmTool.genInputs,"VEN_VentilationDuringHeating"			            ,False,DataSourceUpdateMode.OnPropertyChanged)
  cboVEN_VentilationDuringCooling				        .DataBindings.Add("Text",ssmTool.genInputs,"VEN_VentilationDuringCooling"				        ,False,DataSourceUpdateMode.OnPropertyChanged) 					

  'Aux. Heater  
  txtAH_EngineWasteHeatkW	                            .DataBindings.Add("Text",ssmTool.genInputs,"AH_EngineWasteHeatkW"	                            ,False,DataSourceUpdateMode.OnPropertyChanged)
  txtAH_FuelFiredHeaterkW                               .DataBindings.Add("Text",ssmTool.genInputs,"AH_FuelFiredHeaterkW"                               ,False,DataSourceUpdateMode.OnPropertyChanged)


End Sub

  'GeneralInputControlEvents
  Private Sub cboBuses_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cboBuses.SelectedIndexChanged

  If cboBuses.SelectedIndex > 0 Then


      Dim bus As IBus = DirectCast(cboBuses.SelectedItem, IBus)

      txtBusModel.Text = bus.Model
      txtRegisteredPassengers.Text = bus.RegisteredPassengers
      txtBusFloorType.Text = bus.FloorType
      'ssmTOOL.genInputs.BP_BusFloorSurfaceArea Calculated
      txtBusSurfaceArea.Text = bus.AreaInMetresSquared
      'ssmTOOL.genInputs.BP_BusWindowSurface Calculated
      txtBusVolume.Text = bus.VolumneInMetresQubed
     txtBusLength.Text = bus.LengthInMetres
     txtBusWidth.Text = bus.WidthInMetres

     txtRegisteredPassengers.Focus()

  End If


End Sub


  'Validators
  Public Sub Validating_GeneralInputsBP(sender As Object, e As CancelEventArgs) Handles txtRegisteredPassengers.Validating, txtBusWidth.Validating, txtBusVolume.Validating, txtBusSurfaceArea.Validating, txtBusModel.Validating, txtBusLength.Validating, txtBusFloorSurfaceArea.Validating, txtBC_PassengerBoundaryTemperature.Validated

    e.Cancel = Not Validate_GeneralInputsBP()

  End Sub

  Public Sub Validating_GeneralInputsBC(sender As Object, e As CancelEventArgs) Handles  txtBC_GFactor.Validating, txtBC_VolumicMassDieselOrHeatingOil.Validating, txtBC_SpecificVentilationPower.Validating, txtBC_PassengerDensitySemiLowFloor.Validating, txtBC_PassengerDensityRaisedFloor.Validating, txtBC_PassengerDensityLowFloor.Validating, txtBC_MaxTemperatureDeltaForLowFloorBusses.Validating, txtBC_MaxPossibleBenefitFromTechnologyList.Validating, txtBC_lowVentilation.Validating, txtBC_HighVentilation.Validating, txtBC_HeatingBoundaryTemperature.Validating, txtBC_GCVDieselOrHeatingOil.Validating, txtBC_COP.Validating, txtBC_CoolingBoundaryTemperature.Validating, txtBC_AuxHeaterEfficiency.Validating

    e.Cancel = Not Validate_GeneralInputsBC()

  End Sub

  Public Function Validate_GeneralInputsBP() As Boolean

       Dim result As Boolean = True

       'BUS PARAMETERISATION
       '********************

      'txtBusModel
       If txtBusModel.Text.Trim.Length = 0 Then
         ErrorProvider1.SetError(txtBusModel, "Please enter a bus model")
         result = False
       Else
          ErrorProvider1.SetError(txtBusModel, String.Empty)
       End If


       'txtRegisteredPassengers
       If Not IsPostiveInteger(txtRegisteredPassengers.Text) Then
         ErrorProvider1.SetError(txtRegisteredPassengers, "Please enter a positive integer ( Bus : Number of Passengers )")
         result = False
       Else
        ErrorProvider1.SetError(txtRegisteredPassengers, String.Empty)
       End If

      'txtBusWidth
       If Not IsPostiveNumber(txtBusWidth.Text) Then
         ErrorProvider1.SetError(txtBusWidth, "Please enter a positive number ( BusWidth : linear metres )")
         result = False
       Else
        ErrorProvider1.SetError(txtBusWidth, String.Empty)
       End If

        'txtBusVolume
        If Not IsPostiveNumber(txtBusVolume.Text) Then
         ErrorProvider1.SetError(txtBusVolume, "Please enter a positive number ( BusVolume : cubic metres )")
         result = False
        Else
         ErrorProvider1.SetError(txtBusVolume, String.Empty)
        End If

        'txtBusSurfaceArea
        If Not IsPostiveNumber(txtBusSurfaceArea.Text) Then
         ErrorProvider1.SetError(txtBusSurfaceArea, "Please enter a positive number ( BusSurfaceArea : square metres )")
         result = False
        Else
         ErrorProvider1.SetError(txtBusSurfaceArea, String.Empty)
        End If

        'txtBusLength
        If Not IsPostiveNumber(txtBusLength.Text) Then
         ErrorProvider1.SetError(txtBusLength, "Please enter a positive number ( BusLength : linear metres )")
         result = False
        Else
         ErrorProvider1.SetError(txtBusLength, String.Empty)
        End If


        'txtBusFloorSurfaceArea 
        If Not IsPostiveNumber(txtBusFloorSurfaceArea.Text) Then
         ErrorProvider1.SetError(txtBusFloorSurfaceArea, "Please enter a positive number ( BusFloorSurfaceArea : square metres )")
         result = False
        Else
         ErrorProvider1.SetError(txtBusFloorSurfaceArea, String.Empty)
        End If

        'Set Tab Color
        UpdateTabStatus("tabGeneralInputsBP", result)

        Return result

End Function
  Public Function Validate_GeneralInputsBC() As Boolean

         Dim result As Boolean = True

        'BOUNDARY CONDITIONS
        '*******************

        'txtBC_GFactor		
        If Not IsNumeric(txtBC_GFactor.Text) Then
         ErrorProvider1.SetError(txtBC_GFactor, "Please enter a number ( GFactor )")
         result = False
        Else
         ErrorProvider1.SetError(txtBC_GFactor, String.Empty)
        End If  
          		              
        'BC_SolarClouding				      : Calculated    
        'BC_HeatPerPassengerIntoCabinW	      : Calculated
             
        'txtBC_PassengerBoundaryTemperature    
        If Not IsNumeric(txtBC_PassengerBoundaryTemperature.Text) Then
         ErrorProvider1.SetError(txtBC_PassengerBoundaryTemperature, "Please enter a number ( Passenger Boundary Temperature )")
         result = False
        Else
         ErrorProvider1.SetError(txtBC_PassengerBoundaryTemperature, String.Empty)
        End If  
               
        'txtBC_PassengerDensityLowFloor   
        If Not IsNumeric(txtBC_PassengerDensityLowFloor.Text) Then
         ErrorProvider1.SetError(txtBC_PassengerDensityLowFloor, "Please enter a number ( Passenger Density Low Floor )")
         result = False
        Else
         ErrorProvider1.SetError(txtBC_PassengerDensityLowFloor, String.Empty)
        End If
        
                         
        'txtBC_PassengerDensitySemiLowFloor	 
        If Not IsNumeric(txtBC_PassengerDensitySemiLowFloor.Text) Then
         ErrorProvider1.SetError(txtBC_PassengerDensitySemiLowFloor, "Please enter a number ( Passenger Density Semi Low Floor )")
         result = False
        Else
         ErrorProvider1.SetError(txtBC_PassengerDensitySemiLowFloor, String.Empty)
        End If

             
        'txtBC_PassengerDensityRaisedFloor	
        If Not IsNumeric(txtBC_PassengerDensityRaisedFloor.Text) Then
         ErrorProvider1.SetError(txtBC_PassengerDensityRaisedFloor, "Please enter a number ( Passenger Density Raised Floor )")
         result = False
        Else
         ErrorProvider1.SetError(txtBC_PassengerDensityRaisedFloor, String.Empty)
        End If
               
              
        'txtBC_CalculatedPassengerNumber	: Calculated          
        'txtBC_UValues                      : Calculated
        
                                 
        'txtBC_HeatingBoundaryTemperature	
        If Not IsNumeric(txtBC_HeatingBoundaryTemperature.Text) Then
         ErrorProvider1.SetError(txtBC_HeatingBoundaryTemperature, "Please enter a number ( Heating Boundary Temperature )")
         result = False
        Else
         ErrorProvider1.SetError(txtBC_HeatingBoundaryTemperature, String.Empty)
        End If        
        
              
        'txtBC_CoolingBoundaryTemperature 
        If Not IsNumeric(txtBC_CoolingBoundaryTemperature.Text) Then
         ErrorProvider1.SetError(txtBC_CoolingBoundaryTemperature, "Please enter a number ( Cooling Boundary Temperature )")
         result = False
        Else
         ErrorProvider1.SetError(txtBC_CoolingBoundaryTemperature, String.Empty)
        End If          
        
                 
        'txtBC_HighVentilation    
        If Not IsNumeric(txtBC_HighVentilation.Text) Then
         ErrorProvider1.SetError(txtBC_HighVentilation, "Please enter a number ( High Ventilation )")
         result = False
        Else
         ErrorProvider1.SetError(txtBC_HighVentilation, String.Empty)
        End If      
        
                         
        'txtBC_lowVentilation	
        If Not IsNumeric(txtBC_lowVentilation.Text) Then
         ErrorProvider1.SetError(txtBC_lowVentilation, "Please enter a number ( Low Ventilation )")
         result = False
        Else
         ErrorProvider1.SetError(txtBC_lowVentilation, String.Empty)
        End If         
                          
        'txtBC_High             : Calculated                                     
        'txtBC_Low	            : Calculated                                
        'txtBC_HighVentPowerW   : Calculated                  
        'txtBC_LowVentPowerW    : Calculated                           
        'txtBC_SpecificVentilationPower 
         IsTextBoxNumber(txtBC_SpecificVentilationPower,"Please enter a number ( Specific Ventilation Power )",result)                       
        'txtBC_COP	
         IsTextBoxNumber(txtBC_COP,"Please enter a number ( COP )",result)       		                      
        'txtBC_AuxHeaterEfficiency		
         IsTextBoxNumber(txtBC_AuxHeaterEfficiency,"Please enter a number ( Aux Heater Efficiency )",result)                    
        'txtBC_GCVDieselOrHeatingOil   
         IsTextBoxNumber(txtBC_GCVDieselOrHeatingOil,"Please enter a number ( GCV Diesel Or Heating Oil )",result)                      
        'txtBC_VolumicMassDieselOrHeatingOil	
         IsTextBoxNumber(txtBC_VolumicMassDieselOrHeatingOil,"Please enter a number ( Volumic Mass Diesel Or Heating Oil )",result)                   
        'txtBC_WindowAreaPerUnitBusLength	     : Calculated 
        'txtBC_FrontRearWindowArea               : Calculated                      
        'txtBC_MaxTemperatureDeltaForLowFloorBusses
         IsTextBoxNumber(txtBC_MaxTemperatureDeltaForLowFloorBusses,"Please enter a number ( tMax Temp Delta For Low Floor Busses )",result)   
        'txtBC_MaxPossibleBenefitFromTechnologyList
         IsTextBoxNumber(txtBC_MaxPossibleBenefitFromTechnologyList,"Please enter a number ( Max Benefit From Technology List )",result)   

        'Set Tab Color

        UpdateTabStatus("tabGeneralInputsBC", result)

        Return result

  End Function
  Public Function Validate_GeneralInputsOther() as Boolean

      Dim result As Boolean = true

    ' 'EnviromentalConditions				
    'txtEC_EnviromentalTemperature                      
    'txtEC_Solar   	                                  
    					                                         
    ''AC-system				                                     
    'chkAC_InCabinRoomAC_System	                       
    'txtAC_CompressorType			                   
    'cboAC_CompressorCapacitykW	                       
    					
    ''Ventilation				
    'chkVEN_VentilationOnDuringHeating	
			          
    'chkVEN_VentilationWhenBothHeatingAndACInactive		 
    'chkVEN_VentilationDuringAC			                 
    'cboVEN_VentilationFlowSettingWhenHeatingAndACInactive
    'cboVEN_VentilationDuringHeating			          
    'cboVEN_VentilationDuringCooling				      
    					
    ''Aux. Heater				
    'txtAH_EngineWasteHeatkW	                            
    'txtAH_FuelFiredHeaterkW  
    
    
     Return result                             

  End Function



  'Validation Helpers
  Private Sub IsTextBoxNumber( control As TextBox, errorProviderMessage As String , ByRef result As Boolean) 

      If Not IsNumeric(control.Text) Then
         ErrorProvider1.SetError(control, errorProviderMessage)
         result =  False
        Else
         ErrorProvider1.SetError(control, String.Empty)

        End If 

  End sub

  Private Function IsPostiveInteger(ByVal test As String) As Boolean

     'Is this numeric sanity check.
     If Not IsNumeric(test) Then Return False

     Dim number As Single

     If Not Integer.TryParse(test, number) Then Return False

     If number <= 0 Then Return False


     Return True

End Function
  Private Function IsPostiveNumber(ByVal test As String) As Boolean

     'Is this numeric sanity check.
     If Not IsNumeric(test) Then Return False

     Dim number As Single

     If Not Double.TryParse(test, number) Then Return False

     If number <= 0 Then Return False


     Return True

End Function
  Private Function IsZeroOrPostiveNumber(ByVal test As String) As Boolean

     'Is this numeric sanity check.
     If Not IsNumeric(test) Then Return False

     Dim number As Single

     If Not Double.TryParse(test, number) Then Return False

     If number < 0 Then Return False


     Return True

End Function
  Private Function IsNumberBetweenZeroandOne(test As String) As Boolean

     'Is this numeric sanity check.
     If Not IsNumeric(test) Then Return False

     Dim number As Single

     If Not Double.TryParse(test, number) Then Return False

     If number < 0 OrElse number > 1 Then Return False

     Return True

End Function
  Private Function IsIntegerZeroOrPositiveNumber(test As String) As Boolean

     'Is this numeric sanity check.
     If Not IsNumeric(test) Then Return False

     'if not integer then return false

     Dim number As Integer

     If Not Integer.TryParse(test, number) Then Return False

     If number < 0 Then Return False

     Return True


End Function

  'Tab Colors
  Private Sub UpdateTabStatus(pageName As String, resultGood As Boolean)
  
  
         Dim page As TabPage = tabMain.TabPages(pageName)
  
             If Not resultGood Then
  
                SetTabHeader(page, Color.Red)
            
     
         Else
                SetTabHeader(page, Control.DefaultBackColor)
  
         End If
  
  
  
  
  End Sub
  Private Sub SetTabHeader(page As TabPage, color As Color)
  
      TabColors(page) = color
      tabMain.Invalidate()
  
  End Sub
  Private Sub tabMain_DrawItem(sender As Object, e As DrawItemEventArgs)
  
      Dim br As Brush = New SolidBrush(TabColors(tabMain.TabPages(e.Index)))
    
      Using (br)
  
          e.Graphics.FillRectangle(br, e.Bounds)
          Dim sz As SizeF = e.Graphics.MeasureString(tabMain.TabPages(e.Index).Text, e.Font)
          e.Graphics.DrawString(tabMain.TabPages(e.Index).Text, e.Font, Brushes.Black, e.Bounds.Left + (e.Bounds.Width - sz.Width) / 2, e.Bounds.Top + (e.Bounds.Height - sz.Height) / 2 + 1)
  
          Dim rect As Rectangle = e.Bounds
          rect.Offset(-1, -1)
          rect.Inflate(1, 1)
  
  
      End Using
  
  End Sub

  Private Sub EnsureBinding()
        With tabMain
            Dim lastSelectedTabIndex As Integer = .SelectedIndex
            If lastSelectedTabIndex < 0 OrElse lastSelectedTabIndex > .TabCount Then lastSelectedTabIndex = 0
            For currentTab As Integer = 0 To .TabCount - 1
                .SelectedIndex = currentTab
            Next
            .SelectedIndex = 0
        End With
    End Sub


  Private Sub frmHVACTool_Load( sender As Object,  e As EventArgs) Handles MyBase.Load

    'Required for OwnerDraw, this is required in order to color the tabs when a validation error occurs to draw
    'The attention of the user to the fact that attention is required on a particlar tab.
    TabColors.Add(tabGeneralInputsBP, Control.DefaultBackColor)
    TabColors.Add(tabGeneralInputsBC, Control.DefaultBackColor)
    TabColors.Add(tabGeneralInputsOther, Control.DefaultBackColor)
    TabColors.Add(tabTechBenefits, Control.DefaultBackColor)
  
    EnsureBinding()

    'Additional atatched events
    'For Tab Coloring, this is the place where the background will get filled on the tab when attention is required.
    AddHandler tabMain.DrawItem, New System.Windows.Forms.DrawItemEventHandler(AddressOf tabMain_DrawItem)


 
  End Sub


Private Sub Validating_GeneralInputs( sender As Object,  e As EventArgs)

End Sub

Private Sub Validating_GeneralInputsBP( sender As Object,  e As EventArgs)

End Sub
End Class
