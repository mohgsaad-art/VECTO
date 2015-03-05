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
  Private editTechLine As ITechListBenefitLine = New TechListBenefitLine(Nothing)
  Private gvTechListBinding As BindingList(Of ITechListBenefitLine)
  Private DefaultCategories As String() = {"Cooling","Heating","Insulation","Ventiliation"}

  Public UD As String = "Hello"

  public  Sub  UpdateButtonText()


    If txtIndex.Text=String.Empty then

      btnUpdate.Text = "Add"

      Else
      
            btnUpdate.Text = "Update"

      end if


  end sub

  

  Private sub BindGrid(  )

      Dim gvTechListBinding As New BindingList(Of ITechListBenefitLine)(ssmTOOL.techList.TechLines.OrderBy( Function(o) o.Category).ThenBy( Function(t) t.BenefitName).ToList())
      Me.gvTechBenefitLines.DataSource = gvTechListBinding


  End Sub

  Private function GetCategories( ) As List(Of String)

     If Not ssmTOOL is Nothing AndAlso Not ssmTOOL.techList is Nothing AndAlso ssmTOOL.techList.TechLines.Count>0

        'Fuse Lists          
        Dim fusedList As new List(Of String )
        
        For Each s As String In ssmTOOL.techList.TechLines.Select( Function(sel) sel.Category)

          If Not fusedList.Contains(s) then
           fusedList.Add(s)
          End If

        Next
          
          Return fusedList.OrderBy( Function(o) o.ToString()).ToList()

      Else

          Return New List(Of String)(DefaultCategories)

     End If


  End Function

  Public Sub New(busDatabasePath As String, ahsmFilePath As String)

    ' This call is required by the designer.
    InitializeComponent()

    ' Add any initialization after the InitializeComponent() call.
    Me.busDatabasePath = busDatabasePath
    Me.ahsmFilePath = ahsmFilePath

    ssmTOOL = New SSMTOOL(ahsmFilePath)
    ssmTOOL.Load(ahsmFilePath)


   ' ssmTOOL.techList.in("SSMTechBenefitsALLON.csv")

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

     'gvTechBenefitLines
     gvTechBenefitLines.AutoGenerateColumns=false

     Dim cIndex As Integer

     'Column - Category
     cIndex = gvTechBenefitLines.Columns.Add("Category", "Category")
     gvTechBenefitLines.Columns(cIndex).DataPropertyName = "Category"
     gvTechBenefitLines.Columns(cIndex).Width = 70
     gvTechBenefitLines.Columns(cIndex).ReadOnly = True
     gvTechBenefitLines.Columns(cIndex).HeaderCell.Style.Alignment = DataGridViewContentAlignment.TopCenter
     gvTechBenefitLines.Columns(cIndex).HeaderCell.Style.Padding = New Padding(1, 2, 1, 1)

     'Column - BenefitName
     cIndex = gvTechBenefitLines.Columns.Add("BenefitName", "BenefitName")
     gvTechBenefitLines.Columns(cIndex).DataPropertyName = "BenefitName"
     gvTechBenefitLines.Columns(cIndex).Width = 330
     gvTechBenefitLines.Columns(cIndex).ReadOnly = True
     gvTechBenefitLines.Columns(cIndex).HeaderCell.Style.Alignment = DataGridViewContentAlignment.TopCenter
     gvTechBenefitLines.Columns(cIndex).HeaderCell.Style.Padding = New Padding(1, 2, 1, 1)

     'Column - H
     cIndex = gvTechBenefitLines.Columns.Add("H", "H")
     gvTechBenefitLines.Columns(cIndex).DataPropertyName = "H"
     gvTechBenefitLines.Columns(cIndex).Width = 60
     gvTechBenefitLines.Columns(cIndex).ReadOnly = True
     gvTechBenefitLines.Columns(cIndex).HeaderCell.Style.Alignment = DataGridViewContentAlignment.TopCenter
     gvTechBenefitLines.Columns(cIndex).HeaderCell.Style.Padding = New Padding(1, 2, 1, 1)

     'Column - VH
     cIndex = gvTechBenefitLines.Columns.Add("VH", "VH")
     gvTechBenefitLines.Columns(cIndex).DataPropertyName = "VH"
     gvTechBenefitLines.Columns(cIndex).Width = 60
     gvTechBenefitLines.Columns(cIndex).ReadOnly = True
     gvTechBenefitLines.Columns(cIndex).HeaderCell.Style.Alignment = DataGridViewContentAlignment.TopCenter
     gvTechBenefitLines.Columns(cIndex).HeaderCell.Style.Padding = New Padding(1, 2, 1, 1)


     'Column - VV
     cIndex = gvTechBenefitLines.Columns.Add("VV", "VV")
     gvTechBenefitLines.Columns(cIndex).DataPropertyName = "VV"
     gvTechBenefitLines.Columns(cIndex).Width = 60
     gvTechBenefitLines.Columns(cIndex).ReadOnly = True
     gvTechBenefitLines.Columns(cIndex).HeaderCell.Style.Alignment = DataGridViewContentAlignment.TopCenter
     gvTechBenefitLines.Columns(cIndex).HeaderCell.Style.Padding = New Padding(1, 2, 1, 1)

     'Column - VC
     cIndex = gvTechBenefitLines.Columns.Add("VC", "VC")
     gvTechBenefitLines.Columns(cIndex).DataPropertyName = "VC"
     gvTechBenefitLines.Columns(cIndex).Width = 60
     gvTechBenefitLines.Columns(cIndex).ReadOnly = True
     gvTechBenefitLines.Columns(cIndex).HeaderCell.Style.Alignment = DataGridViewContentAlignment.TopCenter
     gvTechBenefitLines.Columns(cIndex).HeaderCell.Style.Padding = New Padding(1, 2, 1, 1)

     'Column - C
     cIndex = gvTechBenefitLines.Columns.Add("C", "C")
     gvTechBenefitLines.Columns(cIndex).DataPropertyName = "C"
     gvTechBenefitLines.Columns(cIndex).Width = 60
     gvTechBenefitLines.Columns(cIndex).ReadOnly = True
     gvTechBenefitLines.Columns(cIndex).HeaderCell.Style.Alignment = DataGridViewContentAlignment.TopCenter
     gvTechBenefitLines.Columns(cIndex).HeaderCell.Style.Padding = New Padding(1, 2, 1, 1)

     'Column - OnVehicle
     cIndex = gvTechBenefitLines.Columns.Add("OnVehicle", "OnVehicle")
     gvTechBenefitLines.Columns(cIndex).DataPropertyName = "OnVehicle"
     gvTechBenefitLines.Columns(cIndex).Width = 60
     gvTechBenefitLines.Columns(cIndex).ReadOnly = True
     gvTechBenefitLines.Columns(cIndex).HeaderCell.Style.Alignment = DataGridViewContentAlignment.TopCenter
     gvTechBenefitLines.Columns(cIndex).HeaderCell.Style.Padding = New Padding(1, 2, 1, 1)


     Dim deleteColumn As New DataGridViewButtonColumn()

     With deleteColumn

       .HeaderText=""
       .ToolTipText="Delete this row"
       .Name="Delete"
       .Text="Del"
       .UseColumnTextForButtonValue=true
       .Width=55
       .DefaultCellStyle.Padding= New Padding(5,1,5,1)
       .DefaultCellStyle.Alignment= DataGridViewContentAlignment.MiddleCenter
       .DefaultCellStyle.ForeColor= Color.Red
       .AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells
       .FlatStyle = FlatStyle.Standard
       .CellTemplate.Style.BackColor = Color.Honeydew
        '.DisplayIndex = 0

     end with
     gvTechBenefitLines.Columns.Add(deleteColumn)

     'Techlist Edit Panel
     cboCategory.DataSource= GetCategories()
     cboUnits.DataSource= {"Fraction"}
     cboLineType.DataSource={"Normal","ActiveVentilation"}


  End Sub
  Private Sub setupBindings()

  UpdateButtonText()

  'TechBenefitLines
   BindGrid()

  'Bus Parameterisation
  'txtBusModel.DataBindings.Add("Text", ssmTOOL.genInputs, "BP_BusModel", False, DataSourceUpdateMode.OnPropertyChanged)
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
  Public Sub Validating_GeneralInputsOther(sender As Object, e As CancelEventArgs) Handles txtEC_Solar.Validating, txtEC_EnviromentalTemperature.Validating, txtAH_FuelFiredHeaterkW.Validating, txtAH_EngineWasteHeatkW.Validating, txtAC_CompressorCapacitykW.Validating 

    e.Cancel = Not Validate_GeneralInputsOther()

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
         IsTextBoxNumber(txtBC_GFactor,"Please enter a number ( GFactor )",result) 
        'BC_SolarClouding				      : Calculated    
        'BC_HeatPerPassengerIntoCabinW	      : Calculated             
        'txtBC_PassengerBoundaryTemperature    
         IsTextBoxNumber(txtBC_PassengerBoundaryTemperature,"Please enter a number ( Passenger Boundary Temperature )",result)
        'txtBC_PassengerDensityLowFloor   
         IsTextBoxNumber(txtBC_PassengerDensityLowFloor,"Please enter a number ( Passenger Density Low Floor )",result)
        'txtBC_PassengerDensitySemiLowFloor	 
         IsTextBoxNumber(txtBC_PassengerDensitySemiLowFloor,"Please enter a number ( Passenger Density Semi Low Floor )",result)
        'txtBC_PassengerDensityRaisedFloor	
         IsTextBoxNumber(txtBC_PassengerDensityRaisedFloor,"Please enter a number ( Passenger Density Raised Floor )",result)
        'txtBC_CalculatedPassengerNumber	: Calculated          
        'txtBC_UValues                      : Calculated                            
        'txtBC_HeatingBoundaryTemperature	
         IsTextBoxNumber(txtBC_HeatingBoundaryTemperature,"Please enter a number ( Heating Boundary Temperature )",result)             
        'txtBC_CoolingBoundaryTemperature 
         IsTextBoxNumber(txtBC_CoolingBoundaryTemperature,"Please enter a number ( Cooling Boundary Temperature )",result)                 
        'txtBC_HighVentilation    
         IsTextBoxNumber(txtBC_HighVentilation,"Please enter a number ( High Ventilation )",result)                    
        'txtBC_lowVentilation	
         IsTextBoxNumber(txtBC_lowVentilation,"Please enter a number ( Low Ventilation )",result)                  
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
         IsTextBoxNumber(txtBC_MaxTemperatureDeltaForLowFloorBusses,"Please enter a number ( Max Temp Delta For Low Floor Busses )",result)   
        'txtBC_MaxPossibleBenefitFromTechnologyList
         IsTextBoxNumber(txtBC_MaxPossibleBenefitFromTechnologyList,"Please enter a number ( Max Benefit From Technology List )",result)   

        'Set Tab Color
        UpdateTabStatus("tabGeneralInputsBC", result)

        Return result

  End Function
  Public Function Validate_GeneralInputsOther() as Boolean

      Dim result As Boolean = true

    'EnviromentalConditions				
     IsTextBoxNumber(txtEC_EnviromentalTemperature,"Please enter a number (Environmental Temperature)",result)                  
    'txtEC_Solar   	                                  
     IsTextBoxNumber(txtEC_Solar,"Please enter a number (Solar)",result) 
         					                                         
    ''AC-system				                                     
    'chkAC_InCabinRoomAC_System	     : Selection                  
    'cboAC_CompressorType			 : Selection                
    'txtAC_CompressorCapacitykW	 
     IsTextBoxNumber(txtAC_CompressorCapacitykW,"Please enter a number ( Compressor Capacity )",result)                      
    					
    ''Ventilation				
    'chkVEN_VentilationOnDuringHeating				          : Selection
    'chkVEN_VentilationWhenBothHeatingAndACInactive		      : Selection
    'chkVEN_VentilationDuringAC			                      : Selection
    'cboVEN_VentilationFlowSettingWhenHeatingAndACInactive    : Selection
    'cboVEN_VentilationDuringHeating			              : Selection
    'cboVEN_VentilationDuringCooling				          : Selection
    					
    ''Aux. Heater				
    ' txtAH_EngineWasteHeatkW	
     IsTextBoxNumber(txtAH_EngineWasteHeatkW,"Please enter a number ( Engine waste heat )",result)                                                          
    'txtAH_FuelFiredHeaterkW  
     IsTextBoxNumber(txtAH_FuelFiredHeaterkW,"Please enter a number ( Fuel fired heater )",result)
    
     'Set Tab Color
     UpdateTabStatus("tabGeneralInputsOther", result)

     Return result                             

  End Function

  Public Sub Validating_TechLineEdit(sender As Object, e As CancelEventArgs) 'Handles txtSemiLowFloorV.Validating, txtSemiLowFloorH.Validating, txtSemiLowFloorC.Validating, txtRaisedFloorV.Validating, txtRaisedFloorH.Validating, txtRaisedFloorC.Validating, txtLowFloorV.Validating, txtLowFloorH.Validating, txtLowFloorC.Validating, txtBenefitName.Validating, chkOnVehicle.Validating, chkActiveVV.Validating, chkActiveVH.Validating, chkActiveVC.Validating, cboUnits.Validating, cboLineType.Validating, cboCategory.Validating

    e.Cancel = Not Validate_TechLineEdit()

  End Sub

  Public Function Validate_TechLineEdit() As Boolean

     Dim result As Boolean = True
     
      IsEmptyString(cboCategory.Text      ,cboCategory     ,"Please enter a valid category"                       ,result)
      IsEmptyString(txtBenefitName.Text   ,txtBenefitName  ,"Please enter a valid Benefit Name"                   ,result)
      IsEmptyString(cboUnits.Text         ,cboUnits        ,"Please enter valid units"                            ,result)
      IsEmptyString(cboLineType.Text      , cboLineType    ,"Please enter a valid line type"                      ,result)
      IsTextBoxNumber(txtLowFloorH                         ,"Please enter a valid number for this floor variable" ,result)
      IsTextBoxNumber(txtLowFloorV                         ,"Please enter a valid number for this floor variable" ,result)
      IsTextBoxNumber(txtLowFloorC                         ,"Please enter a valid number for this floor variable" ,result)
      IsTextBoxNumber(txtSemiLowFloorH                     ,"Please enter a valid number for this floor variable" ,result)
      IsTextBoxNumber(txtSemiLowFloorV                     ,"Please enter a valid number for this floor variable" ,result)
      IsTextBoxNumber(txtSemiLowFloorC                     ,"Please enter a valid number for this floor variable" ,result)
      IsTextBoxNumber(txtRaisedFloorH                      ,"Please enter a valid number for this floor variable" ,result)
      IsTextBoxNumber(txtRaisedFloorV                      ,"Please enter a valid number for this floor variable" ,result)
      IsTextBoxNumber(txtRaisedFloorC                      ,"Please enter a valid number for this floor variable" ,result)

       
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
  Private Sub IsEmptyString( text as  String, control As control, errorProviderMessage As String , ByRef result As Boolean) 

      If String.IsNullOrEmpty( text ) Then
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

    gvTechBenefitLines.ClearSelection()



 
  End Sub

  
  
  
  Private Sub FillTechLineEditPanel( index As Integer)

     Dim techline As ITechListBenefitLine
     Dim benefitName , category As String
     benefitName = gvTechBenefitLines.Rows(index).Cells("BenefitName").Value
     category = gvTechBenefitLines.Rows(index).Cells("Category").Value

     techline = ssmTOOL.techList.TechLines.First( Function(f) f.BenefitName=benefitName AndAlso f.Category=category)

     txtIndex.Text=index
     cboCategory.Text= techline.Category
     txtBenefitName.Text=techline.BenefitName
     cboUnits.Text = techline.Units
     cboLineType.Text = If( techline.LineType=0, "Normal","ActiveVentilation")
     txtLowFloorH    .Text = techline.LowFloorH
     txtLowFloorV    .Text = techline.LowFloorV    
     txtLowFloorC    .Text = techline.LowFloorC    
     txtSemiLowFloorH.Text = techline.SemiLowFloorH
     txtSemiLowFloorV.Text = techline.SemiLowFloorV
     txtSemiLowFloorC.Text = techline.SemiLowFloorC
     txtRaisedFloorH .Text = techline.RaisedFloorH 
     txtRaisedFloorV .Text = techline.RaisedFloorV 
     txtRaisedFloorC .Text = techline.RaisedFloorC 
     chkActiveVH.Checked   = techline.ActiveVH
     chkActiveVV.Checked   = techline.ActiveVV
     chkActiveVC.Checked   = techline.ActiveVC
     chkOnVehicle.Checked  = techline.OnVehicle

                        

  End Sub


Private Sub gvTechBenefitLines_DoubleClick( sender As Object,  e As EventArgs) Handles gvTechBenefitLines.DoubleClick

    If  gvTechBenefitLines.SelectedCells.Count<1 then Return
    

     Dim row As Integer = gvTechBenefitLines.SelectedCells(0).OwningRow.Index

     Dim benefitName , category As String
     benefitName = gvTechBenefitLines.Rows(row).Cells("BenefitName").Value
     category = gvTechBenefitLines.Rows(row).Cells("Category").Value

     editTechLine = ssmTOOL.techList.TechLines.First( Function(f) f.BenefitName=benefitName AndAlso f.Category=category)

     FillTechLineEditPanel( row )

     UpdateButtonText()



End Sub


private function GetTechLineFromPanel() as ITechListBenefitLine

  Dim tl As ITechListBenefitLine  = New TechListBenefitLine( ssmTOOL.genInputs)
   

  tl.Category      = StrConv(cboCategory.Text, vbProperCase)
  tl.BenefitName   = txtBenefitName.Text
  tl.Units         = cboUnits.Text
  tl.LineType      = If( cboLineType.Text= "Normal",0,3)
  tl.LowFloorH     = txtLowFloorH      .Text
  tl.LowFloorV     = txtLowFloorV      .Text
  tl.LowFloorC     = txtLowFloorC      .Text
  tl.SemiLowFloorH = txtSemiLowFloorH  .Text
  tl.SemiLowFloorV = txtSemiLowFloorV  .Text
  tl.SemiLowFloorC = txtSemiLowFloorC  .Text
  tl.RaisedFloorH  = txtRaisedFloorH   .Text
  tl.RaisedFloorV  = txtRaisedFloorV   .Text
  tl.RaisedFloorC  = txtRaisedFloorC   .Text
  tl.ActiveVH      = chkActiveVH       .Checked
  tl.ActiveVV      = chkActiveVV       .Checked
  tl.ActiveVC      = chkActiveVC       .Checked
  tl.OnVehicle     = chkOnVehicle      .Checked


  Return tl

End Function

Private Sub ClearEditPanel()

  txtIndex.Text                     = String.Empty
  cboCategory.SelectedIndex=0
  txtBenefitName.Text               = String.Empty
  cboUnits.SelectedIndex=0                
  cboLineType.SelectedIndex=0
  txtLowFloorH      .Text           = String.Empty
  txtLowFloorV      .Text           = String.Empty
  txtLowFloorC      .Text           = String.Empty
  txtSemiLowFloorH  .Text           = String.Empty
  txtSemiLowFloorV  .Text           = String.Empty
  txtSemiLowFloorC  .Text           = String.Empty
  txtRaisedFloorH   .Text           = String.Empty
  txtRaisedFloorV   .Text           = String.Empty
  txtRaisedFloorC   .Text           = String.Empty
  chkActiveVH       .Checked        = False
  chkActiveVV       .Checked        = False
  chkActiveVC       .Checked        = False
  chkOnVehicle      .Checked        = False

End Sub


Private Sub btnUpdate_Click( sender As Object,  e As EventArgs) Handles btnUpdate.Click
 
  Dim feedback As String = String.Empty

  If NOT Validate_TechLineEdit() then Return
  
  If txtIndex.Text.Trim.Length=0 then 
  'This is an Add
   If Not ssmTOOL.techList.Add( GetTechLineFromPanel(), feedback) then
     MessageBox.Show( feedback )
     Else
      BindGrid()

      cboCategory.DataSource= GetCategories()

      UpdateButtonText()

   End if

  Else
  'This is an update
    If Not ssmTOOL.techList.Modify( editTechLine, GetTechLineFromPanel() , feedback) then
        MessageBox.Show( feedback )
     Else
       gvTechBenefitLines.Refresh()
       ClearEditPanel()
       UpdateButtonText()
      
    End If

  End If

End Sub




Private Sub btnSave_Click( sender As Object,  e As EventArgs) Handles btnSave.Click


     ssmTOOL.Save( ahsmFilePath )


End Sub



Private Sub gvTechBenefitLines_CellClick( sender As Object,  e As DataGridViewCellEventArgs) Handles gvTechBenefitLines.CellClick

   If e.ColumnIndex<0 OrElse e.RowIndex<0 then Return


   If gvTechBenefitLines.Columns( e.ColumnIndex).Name="Delete" then

      Dim benefit As String = gvTechBenefitLines.Rows( e.RowIndex).Cells(1).Value
      Dim category As String = gvTechBenefitLines.Rows( e.RowIndex).Cells(0).Value
      Dim feedback As String = String.Empty

      Dim dr As DialogResult = MessageBox.Show(String.Format("Do you want to delete benefit '{0}' ?", benefit),"", MessageBoxButtons.YesNo)

      If dr= Windows.Forms.DialogResult.Yes then
      
       If  ssmTOOL.techList.Delete( New TechListBenefitLine With {.BenefitName= benefit, .Category=category}, feedback) then

         BindGrid

       End If



      End If


   End If

End Sub



Private Sub btnClearForm_Click( sender As Object,  e As EventArgs) Handles btnClearForm.Click

  ClearEditPanel()
  UpdateButtonText()


End Sub



End Class
