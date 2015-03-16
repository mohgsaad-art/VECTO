' Copyright 2015 European Union.
' Licensed under the EUPL (the 'Licence');
'
' * You may not use this work except in compliance with the Licence.
' * You may obtain a copy of the Licence at: http://ec.europa.eu/idabc/eupl
' * Unless required by applicable law or agreed to in writing,
'   software distributed under the Licence is distributed on an "AS IS" basis,
'   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
'
' See the LICENSE.txt for the specific language governing permissions and limitations.

Imports VectoAuxiliaries
Imports VectoAuxiliaries.Electrics
Imports VectoAuxiliaries.Pneumatics
Imports System.ComponentModel
Imports System.Windows.Forms
Imports System.Drawing
Imports VectoAuxiliaries.Hvac
Imports System.IO


Public Class frmAuxiliaryConfig

#Region "Fields"

Public  auxConfig      As AuxiliaryConfig
Public  originalConfig As AuxiliaryConfig ' required to test if the form is dirty
Private TabColors As Dictionary(Of TabPage, Color) = New Dictionary(Of TabPage, Color)()
Private processing As Boolean = False
Private SecondsIntoCycle As Integer = 0
Private vectoFile As String = ""
Private vectoPath As String = ""
Private auxFile As string
Private cmFilesList As String()
Private SaveClicked As Boolean

#End Region

Private Function ValidateAuxFileName( filename As String ) As Boolean

       Dim message As String = String.Empty

       If Not FilePathUtils.ValidateFilePath(filename,".aaux", message) then
         MessageBox.Show ( message )
       End If

       Return true

End Function

'Constructor
Public Sub new( byval fileName As String, byval vectoFileName As String )


        If  Not ValidateAuxFileName( fileName ) then
          Me.DialogResult=Windows.Forms.DialogResult.Abort
          Me.Close
        End If          


    Me.vectoFile = vectoFileName
    Me.vectoPath = FilePathUtils.filePathOnly( vectoFileName)

    ' This call is required by the designer.
    InitializeComponent()
    
    ' Add any initialization after the InitializeComponent() call.
    auxFile = fileName

    Try

     auxConfig = New AuxiliaryConfig( FilePathUtils.ResolveFilePath(vectoPath,auxFile) )
     originalConfig = New AuxiliaryConfig(  FilePathUtils.ResolveFilePath(vectoPath,auxFile) )

    Catch ex As Exception

     MessageBox.Show( "The filename you supplied {0} was invalid or could not be found ", fileName )
     Me.DialogResult=Windows.Forms.DialogResult.Abort
     Me.Close

    End Try


    
End Sub

'Validation
#Region "Validation Helpers"


Public Function IsPostiveNumber(ByVal test As String) As Boolean

     'Is this numeric sanity check.
     If Not IsNumeric(test) Then Return False

     Dim number As Single

     If Not Single.TryParse(test, number) Then Return False

     If number <= 0 Then Return False


     Return True

End Function

Public Function IsZeroOrPostiveNumber(ByVal test As String) As Boolean

     'Is this numeric sanity check.
     If Not IsNumeric(test) Then Return False

     Dim number As Single

     If Not Single.TryParse(test, number) Then Return False

     If number < 0 Then Return False


     Return True

End Function

Public Function IsNumberBetweenZeroandOne(test As String) As Boolean

     'Is this numeric sanity check.
     If Not IsNumeric(test) Then Return False

     Dim number As Single

     If Not Single.TryParse(test, number) Then Return False

     If number < 0 OrElse number > 1 Then Return False

     Return True

End Function

Public Function IsIntegerZeroOrPositiveNumber(test As String) As Boolean

     'Is this numeric sanity check.
     If Not IsNumeric(test) Then Return False

     'if not integer then return false

     Dim number As Integer

     If Not Integer.TryParse(test, number) Then Return False

     If number < 0 Then Return False

     Return True


End Function

#End Region
#Region "Validation Control"


'****** PNEUMATIC VALIDATION
Public Sub Validating_PneumaticHandler(sender As Object, e As CancelEventArgs) Handles txtAdBlueNIperMinute.Validating, txtBrakingWithRetarderNIperKG.Validating, txtBrakingNoRetarderNIperKG.Validating, txtAirControlledSuspensionNIperMinute.Validating, txtBreakingPerKneelingNIperKGinMM.Validating, txtSmartRegenFractionTotalAirDemand.Validating, txtPerStopBrakeActuationNIperKG.Validating, txtPerDoorOpeningNI.Validating, txtOverrunUtilisationForCompressionFraction.Validating, txtNonSmartRegenFractionTotalAirDemand.Validating, txtDeadVolumeLitres.Validating, txtDeadVolBlowOutsPerLitresperHour.Validating, txtKneelingHeightMillimeters.Validating, txtCompressorMap.Validating, txtCompressorGearRatio.Validating, txtCompressorGearEfficiency.Validating, txtActuationsMap.Validating, cboDoors.Validating, cboAirSuspensionControl.Validating, cboAdBlueDosing.Validating

    e.Cancel = Not Validate_Pneumatics()

End Sub
Public Function Validate_Pneumatics() As Boolean

       Dim result As Boolean = True

       'PNEUMATIC AUXILLARIES PART
       '***************************


       'AdBlue NI per Minute : txtAdBlueNIperMinute
       If Not IsZeroOrPostiveNumber(txtAdBlueNIperMinute.Text) Then
         ErrorProvider.SetError(txtAdBlueNIperMinute, "Please provide a non negative number.")
         result = False
       Else
          ErrorProvider.SetError(txtAdBlueNIperMinute, String.Empty)
       End If

       'Overrun Utilisation For Compression Fraction : txtOverrunUtilisationForCompressionFraction
       If Not IsNumberBetweenZeroandOne(txtOverrunUtilisationForCompressionFraction.Text) Then
         ErrorProvider.SetError(txtOverrunUtilisationForCompressionFraction, "Please provide a non negative between 0 and 1.")
         result = False
         Else
         ErrorProvider.SetError(txtOverrunUtilisationForCompressionFraction, String.Empty)
       End If

       'Braking With Retarder NI per KG : txtBrakingWithRetarderNIperKG
       If Not IsZeroOrPostiveNumber(txtBrakingWithRetarderNIperKG.Text) Then
         ErrorProvider.SetError(txtBrakingWithRetarderNIperKG, "Please provide a non negative number.")
         result = False
        Else
         ErrorProvider.SetError(txtBrakingWithRetarderNIperKG, String.Empty)
       End If

       'Braking No Retarder NI per KG : txtBrakingNoRetarderNIperKG
       If Not IsZeroOrPostiveNumber(txtBrakingNoRetarderNIperKG.Text) Then
         ErrorProvider.SetError(txtBrakingNoRetarderNIperKG, "Please provide a non negative number.")
         result = False
       Else
         ErrorProvider.SetError(txtBrakingNoRetarderNIperKG, String.Empty)
       End If

       'Breaking Per Kneeling NI per KG in MM : txtBreakingPerKneelingNIperKGinMM
       If Not IsZeroOrPostiveNumber(txtBreakingPerKneelingNIperKGinMM.Text) Then
         ErrorProvider.SetError(txtBreakingPerKneelingNIperKGinMM, "Please provide a non negative number.")
         result = False
       Else
         ErrorProvider.SetError(txtBreakingPerKneelingNIperKGinMM, String.Empty)
       End If

       'Per Door Opening NI : txtPerDoorOpeningNI
       If Not IsZeroOrPostiveNumber(txtPerDoorOpeningNI.Text) Then
         ErrorProvider.SetError(txtPerDoorOpeningNI, "Please provide a non negative number.")
         result = False
       Else
         ErrorProvider.SetError(txtPerDoorOpeningNI, String.Empty)
       End If

       'Per Stop Brake Actuation NI per KG : txtPerStopBrakeActuationNIperKG
       If Not IsZeroOrPostiveNumber(txtPerStopBrakeActuationNIperKG.Text) Then
         ErrorProvider.SetError(txtPerStopBrakeActuationNIperKG, "Please provide a non negative number.")
         result = False
       Else
         ErrorProvider.SetError(txtPerStopBrakeActuationNIperKG, String.Empty)
       End If

       'Air Controlled Suspension NI per Minute : txtAirControlledSuspensionNIperMinute
       If Not IsZeroOrPostiveNumber(txtAirControlledSuspensionNIperMinute.Text) Then
         ErrorProvider.SetError(txtAirControlledSuspensionNIperMinute, "Please provide a non negative number.")
         result = False
       Else
         ErrorProvider.SetError(txtAirControlledSuspensionNIperMinute, String.Empty)
       End If

       'Non Smart Regen Fraction Total Air Demand : txtNonSmartRegenFractionTotalAirDemand
       If Not IsZeroOrPostiveNumber(txtNonSmartRegenFractionTotalAirDemand.Text) Then
         ErrorProvider.SetError(txtNonSmartRegenFractionTotalAirDemand, "Please provide a non negative number.")
         result = False
       Else
         ErrorProvider.SetError(txtNonSmartRegenFractionTotalAirDemand, String.Empty)
       End If

       'Smart Regen Fraction Total Air Demand : txtSmartRegenFractionTotalAirDemand
       If Not IsNumberBetweenZeroandOne(txtSmartRegenFractionTotalAirDemand.Text) Then
         ErrorProvider.SetError(txtSmartRegenFractionTotalAirDemand, "Please provide a non negative between 0 and 1.")
         result = False
       Else
         ErrorProvider.SetError(txtSmartRegenFractionTotalAirDemand, String.Empty)
       End If


       'Dead Volume Litres : txtDeadVolumeLitres
       If Not IsZeroOrPostiveNumber(txtDeadVolumeLitres.Text) Then
         ErrorProvider.SetError(txtDeadVolumeLitres, "Please provide a non negative number.")
         result = False
       Else
         ErrorProvider.SetError(txtDeadVolumeLitres, String.Empty)
       End If


       'Dead Vol BlowOuts Per Litresper Hour : txtDeadVolBlowOutsPerLitresperHour
       If Not IsZeroOrPostiveNumber(txtDeadVolBlowOutsPerLitresperHour.Text) Then
         ErrorProvider.SetError(txtDeadVolBlowOutsPerLitresperHour, "Please provide a non negative number.")
         result = False
        Else
         ErrorProvider.SetError(txtDeadVolBlowOutsPerLitresperHour, String.Empty)
       End If


       'USER CONFIG PART 
       '*****************************************************************************************
        'Compressor Map path : txtCompressorMap
        'Test for empty after trim
        If txtCompressorMap.Text.Trim.Length = 0 Then
         ErrorProvider.SetError(txtCompressorMap, "Please enter the localtion of a valid compressor map.")
         result = False
        Else
         ErrorProvider.SetError(txtCompressorMap, String.Empty)
        End If
        'Test File is valid
        Dim comp As CompressorMap
        Try

        comp = New CompressorMap(FilePathUtils.ResolveFilePath( vectoPath,txtCompressorMap.Text))
        comp.Initialise()
         ErrorProvider.SetError(txtCompressorMap, String.Empty)
        Catch ex As Exception
         ErrorProvider.SetError(txtCompressorMap, "Error : map is invalid or cannot be found, please select a Cvalid compressor map")
         result = False
        End Try

        'Compressor Gear Efficiency : txtCompressorGearEfficiency"
        If Not IsNumberBetweenZeroandOne(txtCompressorGearEfficiency.Text) Then
          ErrorProvider.SetError(txtCompressorGearEfficiency, "Please enter a number between 0 and 1")
          result = False
        Else
          ErrorProvider.SetError(txtCompressorGearEfficiency, String.Empty)
        End If

        'Compressor Gear Ratio : txtCompressorGearRatio
        If Not IsPostiveNumber(txtCompressorGearRatio.Text) Then
          ErrorProvider.SetError(txtCompressorGearRatio, "Please enter a number greater than 0.")
          result = False
        Else
          ErrorProvider.SetError(txtCompressorGearRatio, String.Empty)
        End If


        'Actuations Map : txtActuationsMap
        If txtActuationsMap.Text.Trim.Length = 0 Then
         ErrorProvider.SetError(txtActuationsMap, "Please enter the localtion of a valid Pneumatic Actuations map.")
         result = False
        Else
         ErrorProvider.SetError(txtActuationsMap, String.Empty)
        End If
        'Test File is valid
        Dim actuations As PneumaticActuationsMAP
        Try

        actuations = New PneumaticActuationsMAP(FilePathUtils.ResolveFilePath( vectoPath,txtActuationsMap.Text))
         actuations.Initialise()
         ErrorProvider.SetError(txtActuationsMap, String.Empty)
        Catch ex As Exception
         ErrorProvider.SetError(txtActuationsMap, "Error : Pneumatic Actuations map is invalid or cannot be found, please select a valid map")
         result = False
        End Try



        'NOT Required but included here so readers can see this is a positive ommission
        '******************************************************************************
        'Smart Air Compression : chkSmartAirCompression
        'Smart Regeneration : chkSmartRegeneration
        'Retarder Brake : chkRetarderBrake

        'txtKneelingHeightMillimeters : txtKneelingHeightMillimeters
        If Not IsPostiveNumber(txtKneelingHeightMillimeters.Text) Then
          ErrorProvider.SetError(txtKneelingHeightMillimeters, "Please enter a number greater than 0.")
          result = False
        Else
          ErrorProvider.SetError(txtKneelingHeightMillimeters, String.Empty)
        End If

        'cboAirSuspensionControl : cboAirSuspensionControl
        If cboAirSuspensionControl.SelectedIndex < 1 Then
          ErrorProvider.SetError(cboAirSuspensionControl, "Please make a selection.")
          result = False
        Else
          ErrorProvider.SetError(cboAirSuspensionControl, String.Empty)
        End If

        'cboAdBlueDosing : cboAdBlueDosing
        If cboAdBlueDosing.SelectedIndex < 1 Then
          ErrorProvider.SetError(cboAdBlueDosing, "Please make a selection.")
          result = False
        Else
          ErrorProvider.SetError(cboAdBlueDosing, String.Empty)
        End If

        'cboDoors : cboDoors
        If cboDoors.SelectedIndex < 1 Then
          ErrorProvider.SetError(cboDoors, "Please make a selection.")
          result = False
        Else
          ErrorProvider.SetError(cboDoors, String.Empty)
        End If


        'Set Tab Color

        UpdateTabStatus("tabPneumaticConfig", result)




Return result


End Function

'*****  ELECTRICAL VALIDATION
Public Sub Validating_ElectricsHandler(sender As Object, e As CancelEventArgs) Handles txtPowernetVoltage.Validating, txtAlternatorMapPath.Validating, txtAlternatorGearEfficiency.Validating, txtDoorActuationTimeSeconds.Validating

    e.Cancel = Not Validate_Electrics()

End Sub
Public Function Validate_Electrics() As Boolean

Dim result As Boolean = True


       'Powernet Voltage : txtPowernetVoltage
       If Not IsPostiveNumber(txtPowernetVoltage.Text) Then
         ErrorProvider.SetError(txtPowernetVoltage, "Please provide a non negative number.")
         result = False
       Else
          ErrorProvider.SetError(txtPowernetVoltage, String.Empty)
       End If


        'Alternator Map  path : txtAlternatorMapPath
        'Test for empty after trim
        If txtAlternatorMapPath.Text.Trim.Length = 0 Then
         ErrorProvider.SetError(txtAlternatorMapPath, "Please enter the localtion of a valid alternator map.")
         result = False
        Else
         ErrorProvider.SetError(txtAlternatorMapPath, String.Empty)
        End If

        'Test File is valid
        Dim alt As AlternatorMap
        Try
        alt = New AlternatorMap(FilePathUtils.ResolveFilePath( vectoPath,txtAlternatorMapPath.Text))
        alt.Initialise()
         ErrorProvider.SetError(txtAlternatorMapPath, String.Empty)
        Catch ex As Exception
         ErrorProvider.SetError(txtAlternatorMapPath, "Error : map is invalid or cannot be found, please select a valid alternator map")
         result = False
        End Try


        'Alternator Gear Efficiency : txtAlternatorGearEfficiency
        If Not IsNumberBetweenZeroandOne(txtAlternatorGearEfficiency.Text) Then
         ErrorProvider.SetError(txtAlternatorGearEfficiency, "Please enter a number between 0 an 1")
         result = False
        Else
         ErrorProvider.SetError(txtAlternatorGearEfficiency, String.Empty)
        End If


       'Door Action Time : txtDoorActuationTimeSeconds
       If Not IsPostiveNumber(txtDoorActuationTimeSeconds.Text) Then
         ErrorProvider.SetError(txtDoorActuationTimeSeconds, "Please provide a non negative number.")
         result = False
       Else
          ErrorProvider.SetError(txtDoorActuationTimeSeconds, String.Empty)
       End If


               UpdateTabStatus("tabElectricalConfig", result)


       Return result


End Function

'****** HVAC VALIDATION
Public Sub Validating_HVACHandler(sender As Object, e As CancelEventArgs) Handles    txtSSMFilePath.Validating, txtBusDatabaseFilePath.Validating

    e.Cancel = Not Validate_HVAC()

End Sub
Public Function Validate_HVAC() As Boolean

Dim result As Boolean = True
Dim message As String =""


       ''HVAC Electrical Load Power Watts : txtHVACElectricalLoadPowerWatts
       'If Not IsZeroOrPostiveNumber(txtHVACElectricalLoadPowerWatts.Text) Then
       '  ErrorProvider.SetError(txtHVACElectricalLoadPowerWatts, "Please provide a non negative number.")
       '  result = False
       'Else
       '   ErrorProvider.SetError(txtHVACElectricalLoadPowerWatts, String.Empty)
       'End If

       'HVAC Mechanical Load Power Watts : txtHVACMechanicalLoadPowerWatts
       'If Not IsZeroOrPostiveNumber(txtHVACMechanicalLoadPowerWatts.Text) Then
       '  ErrorProvider.SetError(txtHVACMechanicalLoadPowerWatts, "Please provide a non negative number.")
       '  result = False
       'Else
       '   ErrorProvider.SetError(txtHVACMechanicalLoadPowerWatts, String.Empty)
       'End If


       'HVAC Fuelling Litres Per Hour : txtHVACFuellingLitresPerHour
       'If Not IsZeroOrPostiveNumber(txtHVACFuellingLitresPerHour.Text) Then
       '  ErrorProvider.SetError(txtHVACFuellingLitresPerHour, "Please provide a non negative number.")
       '  result = False
       'Else
       '   ErrorProvider.SetError(txtHVACFuellingLitresPerHour, String.Empty)
       'End If



         'Validate abdb -  Bus Database 
          Dim abdbFile As String  = FilePathUtils.ResolveFilePath(vectoPath,txtBusDatabaseFilePath.Text)
          Dim bdb As New BusDatabase()
          If   bdb.Initialise(abdbFile ) then
               ErrorProvider.SetError(txtBusDatabaseFilePath, String.Empty)
          Else
              result=false
              ErrorProvider.SetError(Me.txtBusDatabaseFilePath, "Please choose a vaid Steady State Model File (*.ABDB")
         End If


       'Try ahsm - HVac Steady State Model
       Try 

          Dim ahsmFile As String = FilePathUtils.ResolveFilePath(vectoPath,txtSSMFilePath.Text)
          Dim ssmTool As SSMTOOL = New SSMTOOL( ahsmFile,False)
          
          If ssmTool.Load( ahsmFile ) then 
                ErrorProvider.SetError(txtSSMFilePath, String.Empty)
          Else
                result=false
                ErrorProvider.SetError(txtSSMFilePath, "Please choose a vaid Steady State Model File (*.AHSM")
          End If
           
       Catch ex As Exception
           'Just in case
           ErrorProvider.SetError(txtSSMFilePath, "Please choose a vaid Steady State Model File (*.AHSM")
           result = false

       End Try


       UpdateTabStatus("tabHVACConfig", result)

   Return result


End Function


Public Function ValidateAll() As Boolean

  If Validate_Pneumatics() = False OrElse Validate_Electrics() = False OrElse Validate_Pneumatics() = False Then

  Return False

  End If

  Return True

End Function

'*****  IMPUTS VALIDATION

#End Region

'Form Controls & Events
Private Sub Dashboard_Load(sender As Object, e As EventArgs) Handles MyBase.Load

  'Required for OwnerDraw, this is required in order to color the tabs when a validation error occurs to draw
  'The attention of the user to the fact that attention is required on a particlar tab.
  TabColors.Add(tabGeneralConfig, Control.DefaultBackColor)
  TabColors.Add(tabElectricalConfig, Control.DefaultBackColor)
  TabColors.Add(tabPneumaticConfig, Control.DefaultBackColor)
  TabColors.Add(tabHVACConfig, Control.DefaultBackColor)


  'This is here only for testing purposes, the actual cycle will be a result of Vecto input.
  cboCycle.SelectedIndex = 0

  'General Setup of all controls 
  SetupControls()

  'Binding Values in Aux environment to the input controls on relevent tabs in the form.
  CreateBindings()

  'This function is neccesary because binding does not occur when the page is invisible, so a track across all of them
  'Is required in order to set the binding. This only needs to be done once at at setup time. after values are set in the
  'Aux environment either by setting defaults of reading and setting from the Auxillaries persistance file.
  EnsureBinding()


  'Additional atatched events
  'For Tab Coloring, this is the place where the background will get filled on the tab when attention is required.
  AddHandler tabMain.DrawItem, New System.Windows.Forms.DrawItemEventHandler(AddressOf tabMain_DrawItem)

  'Select Electric Tab
  tabMain.SelectTab(tabMain.TabPages("tabElectricalConfig"))


End Sub
Private Sub frmAuxiliaryConfig_FormClosing( sender As Object,  e As FormClosingEventArgs) Handles MyBase.FormClosing


  If Me.DialogResult=Windows.Forms.DialogResult.Cancel then return

  Dim result As DialogResult

  If  Not auxConfig.ConfigValuesAreTheSameAs( originalConfig )

             result = (MessageBox.Show("Would you like to save changes before closing?","Save Changes", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question))

            Select Case  result
            
                case DialogResult.Yes:
                    'save 
                    If NOT SaveFile()    then
                      e.Cancel=true
                    End If

                case DialogResult.No:
                    'just allow the form to close
                    'without saving
                    Me.DialogResult=Windows.Forms.DialogResult.Cancel


                case DialogResult.Cancel:
                    'cancel the close
                    e.Cancel = true
                    Me.DialogResult=Windows.Forms.DialogResult.Cancel


            end select


  End If
 

End Sub
#Region "GridHandlers"

Private Sub gvElectricalConsumables_CellValidating(sender As Object, e As DataGridViewCellValidatingEventArgs) Handles gvElectricalConsumables.CellValidating

   Dim column As DataGridViewColumn = gvElectricalConsumables.Columns(e.ColumnIndex)
   Dim s As Single


   If e.ColumnIndex = -1 Then

   e.Cancel = True
   Exit Sub

   End If



   If column.ReadOnly Then Return




    Select Case column.Name

     Case "NominalConsumptionAmps"
           If Not IsNumeric(e.FormattedValue) Then
             MessageBox.Show("This value must be numeric")
             e.Cancel = True
          End If

     Case "NumberInActualVehicle"
           If Not IsNumeric(e.FormattedValue) Then
             MessageBox.Show("This value must be numeric")
            e.Cancel = True
          Else
            s = Single.Parse(e.FormattedValue)
           End If
           If s Mod 1 > 0 OrElse s < 0 Then
              MessageBox.Show("This value must be a positive whole number ( Integer ) ")
             e.Cancel = True
           End If


     Case "PhaseIdle_TractionOn"
           If Not IsNumeric(e.FormattedValue) Then
             MessageBox.Show("This value must be numeric")
             e.Cancel = True
           Else
            s = Single.Parse(e.FormattedValue)
           End If
           If s < 0 OrElse s > 1 Then
              MessageBox.Show("This must be a value between 0 and 1 ")
              e.Cancel = True
           End If


    End Select

End Sub
Private Sub SmartResult_CellValidating(sender As Object, e As DataGridViewCellValidatingEventArgs) Handles gvResultsCardIdle.CellValidating, gvResultsCardTraction.CellValidating, gvResultsCardOverrun.CellValidating

   Dim column As DataGridViewColumn = gvElectricalConsumables.Columns(e.ColumnIndex)

   If Not IsNumeric(e.FormattedValue) Then
       MessageBox.Show("This value must be numeric")
       e.Cancel = True
   End If

End Sub
Private Sub resultCard_CellMouseUp(sender As Object, e As DataGridViewCellMouseEventArgs) Handles gvResultsCardIdle.CellMouseUp, gvResultsCardTraction.CellMouseUp, gvResultsCardOverrun.CellMouseUp

      Dim dgv As DataGridView = CType(sender, DataGridView)


        If e.Button = MouseButtons.Right Then

            resultCardContextMenu.Show(dgv, e.Location)
            resultCardContextMenu.Show(Cursor.Position)

        End If


    End Sub
Private Sub resultCardContextMenu_ItemClicked(sender As Object, e As ToolStripItemClickedEventArgs) Handles resultCardContextMenu.ItemClicked

      Dim menu As ContextMenuStrip = CType(sender, ContextMenuStrip)

      Dim grid As DataGridView = DirectCast(menu.SourceControl, DataGridView)

      Select Case e.ClickedItem.Text


      Case "Delete"

         For Each selectedRow As DataGridViewRow In grid.SelectedRows

            If Not selectedRow.IsNewRow Then

                grid.Rows.RemoveAt(selectedRow.Index)

            End If

         Next

      Case "Insert"


      End Select









End Sub
Private Sub gvElectricalConsumables_CellFormatting(sender As Object, e As DataGridViewCellFormattingEventArgs) Handles gvElectricalConsumables.CellFormatting


    If e.ColumnIndex = 4 AndAlso e.RowIndex = 0 Then

       e.CellStyle.BackColor = Color.LightGray
       e.CellStyle.ForeColor = Color.LightGray


    End If



End Sub
Private Sub gvElectricalConsumables_CellBeginEdit(sender As Object, e As DataGridViewCellCancelEventArgs) Handles gvElectricalConsumables.CellBeginEdit

    If e.ColumnIndex = 4 AndAlso e.RowIndex = 0 Then

     MessageBox.Show("This cell is calculated and cannot be edited")
     e.Cancel = True

    End If




End Sub

#End Region
#Region "Button Handlers"

Private Sub btnSave_Click(sender As Object, e As EventArgs) Handles btnSave.Click


   If  SaveFile() then

      originalConfig.AssumeValuesOfOther( auxConfig)
      Me.Close


   End If



End Sub
Private function SaveFile() As Boolean

   Dim result As Boolean


   If Not ValidateAll then Return false


   result = auxConfig.Save(FilePathUtils.ResolveFilePath(vectoPath,auxFile))

   If Not result then  MessageBox.Show(String.Format("Unable to Save the file '{0}'",auxFile) )

   Return result

End Function
Private Function  LoadFile() As boolean

  'JSON METHOD
   Dim result As boolean

  'Release existing databindings
  UnbindAllControls(Me)

  result = auxConfig.Load( FilePathUtils.ResolveFilePath( vectoPath,auxFile))

  If Not result then     
     MessageBox.Show(String.Format("Unable to load the file '{0}'",auxFile) )
   Else  
    CreateBindings()
  End If

  Return result

End function
Private Sub btnFuelMap_Click(sender As Object, e As EventArgs) Handles btnFuelMap.Click

               Dim fbAux As New cFileBrowser(True, False)



              ' Dim vectoFile As String = "C:\Users\tb28\Source\Workspaces\VECTO\vecto-sim-ricardoaeaTB\AuxillaryTestHarness\bin\Debug\vectopath.vecto"
               Dim fname As String = fFILE(vectoFile, True)

                fbAux.Extensions = New String() {"vmap"}
              '  If fbAux.OpenDialog(fFileRepl(fname, fPATH(VECTOfile))) Then 
               If fbAux.OpenDialog(fPATH(vectoFile)) Then

                 txtFuelMap.Text = fFileWoDir(fbAux.Files(0), fPATH(vectoFile))

                End If


End Sub
Private Sub btnAlternatorMapPath_Click(sender As Object, e As EventArgs) Handles btnAlternatorMapPath.Click



               Dim fbAux As New cFileBrowser(True, False)



              ' Dim vectoFile As String = "C:\Users\tb28\Source\Workspaces\VECTO\vecto-sim-ricardoaeaTB\AuxillaryTestHarness\bin\Debug\vectopath.vecto"
               Dim fname As String = fFILE(vectoFile, True)

                fbAux.Extensions = New String() {"AALT"}
                If fbAux.OpenDialog(fPATH(vectoFile)) Then

                 txtAlternatorMapPath.Text = fFileWoDir(fbAux.Files(0), fPATH(vectoFile))

               End If

               Validate_Electrics()

               'Causes Binding to fire
               txtAlternatorMapPath.Focus()

End Sub
Private Sub btnCompressorMap_Click(sender As Object, e As EventArgs) Handles btnCompressorMap.Click


               Dim fbAux As New cFileBrowser(True, False)



              ' Dim vectoFile As String = "C:\Users\tb28\Source\Workspaces\VECTO\vecto-sim-ricardoaeaTB\AuxillaryTestHarness\bin\Debug\vectopath.vecto"
               Dim fname As String = fFILE(vectoFile, True)

                fbAux.Extensions = New String() {"ACMP"}
                If fbAux.OpenDialog(fPATH(vectoFile)) Then

                 txtCompressorMap.Text = fFileWoDir(fbAux.Files(0), fPATH(vectoFile))

               End If

               Validate_Pneumatics()

               'Causes binding to fire
               txtCompressorMap.Focus()


End Sub
Private Sub btnActuationsMap_Click(sender As Object, e As EventArgs) Handles btnActuationsMap.Click

               Dim fbAux As New cFileBrowser(True, False)

              ' Dim vectoFile As String = "C:\Users\tb28\Source\Workspaces\VECTO\vecto-sim-ricardoaeaTB\AuxillaryTestHarness\bin\Debug\vectopath.vecto"
               Dim fname As String = fFILE(vectoFile, True)

                fbAux.Extensions = New String() {"APAC"}
                If fbAux.OpenDialog(fPATH(vectoFile)) Then

                 txtActuationsMap.Text = fFileWoDir(fbAux.Files(0), fPATH(vectoFile))

                End If

                Validate_Pneumatics()

                'Causes Binding to fire.
                txtActuationsMap.Focus()

End Sub
Private Sub btnCancel_Click( sender As Object,  e As EventArgs) Handles btnCancel.Click


  Me.DialogResult = Windows.Forms.DialogResult.Cancel
  Me.Close()


End Sub

Private Sub btnBusDatabaseSource_Click( sender As Object,  e As EventArgs) Handles btnBusDatabaseSource.Click

               Dim fbAux As New cFileBrowser(True, False)
               Dim ssmMap As Hvac.HVACSteadyStateModel
               Dim message As String = String.Empty


               fbAux.Extensions = New String() {"abdb"}

               If fbAux.OpenDialog(fPATH(vectoFile)) Then

                 txtBusDatabaseFilePath.Focus()
                 txtBusDatabaseFilePath.Text = fFileWoDir(fbAux.Files(0), fPATH(vectoFile))

                 Dim busDB As New BusDatabase( )
                 
                 If  Not busDB.Initialise( FilePathUtils.ResolveFilePath(vectoPath,txtBusDatabaseFilePath.Text) )

                      messagebox.Show("Unable to load")


                 End If


               End If


End Sub
Private Sub btnSSMBSource_Click( sender As Object,  e As EventArgs) Handles btnSSMBSource.Click


               Dim aauxFileValidated As Boolean = False
               Dim fbAux As New cFileBrowser(True, False)
              fbAux.Extensions = New String() {"AHSM"}
               Dim frm As frmHVACTool  

               Dim suppliedSSMPath  As string = String.Empty
               Dim absoluteSSMPath  As String = String.Empty
               Dim absoluteBusDatabasePath As string = String.Empty
               Dim message As String = String.Empty
               Dim newFile As Boolean = false

              
               'Trim ssmPath
               txtSSMFilePath.Text.Trim


                'Is Filename NOT supplied, try and obtain  it, still not supplied, then bail.
                 If( txtSSMFilePath.Text.Length=0) then
               
                      newFile = true
               
                      If fbAux.CustomDialog(vectoPath,False,False, tFbExtMode.ForceExt,False,"") then
                         If fbAux.Files.Count=0 then
                               Return
                          Else                     
                               suppliedSSMPath = fbAux.Files(0)
                         End If
                      Else        
                           'No file given in text box, not given in browser, so bail         
                           return
                      End If 
               
               Else                 
                     suppliedSSMPath = txtSSMFilePath.Text
               
               End If

               'Set Absolutes.
               absoluteSSMPath = FilePathUtils.ResolveFilePath(fPATH(VECTOfile), suppliedSSMPath )
               absoluteBusDatabasePath = FilePathUtils.ResolveFilePath(fPATH(VECTOfile),Me.txtBusDatabaseFilePath.Text )


               'Is supplied filename NOT valid. bail
               if Not FilePathUtils.ValidateFilePath(absoluteSSMPath,".ahsm",Message)  then                 
                  MessageBox.Show( message)
                  return
               end if


               'If file Exists, Check validity, else fire up a default SSM Config.
               If  File.Exists( absoluteSSMPath ) then
                     'is file valid Try ahsm - HVac Steady State Model
                      try
                          Dim ahsmFile As String = FilePathUtils.ResolveFilePath(vectoPath, absoluteBusDatabasePath)
                          Dim ssmTool As SSMTOOL = New SSMTOOL( ahsmFile,False)                        
                          ssmTool.Load( ahsmFile )                   
                       Catch ex As Exception         
                            MessageBox.Show("The supplied AHSM File was invalid, aborting.")
                            return
                       End Try
                Else              
                 newFile=true

                End If


                'If newFile then use Defaults
                If newFile then
                   frm   = New frmHVACTool(absoluteBusDatabasePath, absoluteSSMPath,True )
                               
                Else               
                   frm   = New frmHVACTool(absoluteBusDatabasePath, absoluteSSMPath )

                End If




               'If Dialog result is OK, then take action else bail
               If frm.ShowDialog()   = Windows.Forms.DialogResult.OK then

                 If suppliedSSMPath.Contains(":\")  then

                    txtSSMFilePath.Text= suppliedSSMPath   

                 else
                    
                    txtSSMFilePath.Text = fFileWoDir( suppliedSSMPath )

                 End If

               Else          
                   return
               End If









End Sub


Private function launchSSMConfig( absoluteBusPath as String, absoluteSSMPath As String )  As DialogResult



    


End Function


#End Region

Private Function ValidateSSMFile( filepath As String ) As Boolean

   Dim ssmTool As ISSMTOOL = New SSMTOOL(filepath,False)

   Return  ssmTool.Load( filepath)

End Function

#Region "File Viewer Button Events"

Private Sub btnAALTOpen_Click( sender As Object,  e As EventArgs) Handles btnAALTOpen.Click

        OpenFiles(fFileRepl(Me.txtAlternatorMapPath.Text, fPATH(VECTOfile)))    
    
End Sub
Private Sub btnOpenACMP_Click( sender As Object,  e As EventArgs) Handles btnOpenACMP.Click


         OpenFiles(fFileRepl(Me.txtCompressorMap.Text, fPATH(VECTOfile)))

End Sub
Private Sub btnOpenAPAC_Click( sender As Object,  e As EventArgs) Handles btnOpenAPAC.Click

          OpenFiles(fFileRepl(Me.txtActuationsMap.Text, fPATH(VECTOfile)))

End Sub
Private Sub btnOpenAHSM_Click( sender As Object,  e As EventArgs) Handles btnOpenAHSM.Click

          OpenFiles(fFileRepl(Me.txtSSMFilePath.Text, fPATH(VECTOfile)))


End Sub
Private Sub btnOpenABDB_Click( sender As Object,  e As EventArgs) Handles btnOpenABDB.Click


          OpenFiles(fFileRepl(Me.txtBusDatabaseFilePath.Text, fPATH(VECTOfile)))

End Sub


#end region

'Overrides
Protected Overrides Function ProcessCmdKey(ByRef msg As Message, keyData As Keys) As Boolean

        If keyData = Keys.Enter AndAlso Me.AcceptButton Is Nothing Then
        Dim box As TextBoxBase = CType(Me.ActiveControl, TextBoxBase)

        If box Is Nothing OrElse Not box.Multiline Then

          Me.SelectNextControl(Me.ActiveControl, True, True, True, True)
          Return True

       End If

       End If


       Return MyBase.ProcessCmdKey(msg, keyData)



    End Function

'Helpers
Private Sub OpenFiles(ParamArray files() As String)

        If files.Length = 0 Then Exit Sub

        CmFilesList = files

        OpenWithToolStripMenuItem.Text = "Open with notepad"

        CmFiles.Show(Cursor.Position)

End Sub
Private Sub OpenWithToolStripMenuItem_Click(sender As System.Object, e As System.EventArgs) Handles OpenWithToolStripMenuItem.Click
    If Not FileOpenAlt(CmFilesList(0)) Then MsgBox("Failed to open file!")
End Sub
Private Sub ShowInFolderToolStripMenuItem_Click(sender As System.Object, e As System.EventArgs) Handles ShowInFolderToolStripMenuItem.Click
    If IO.File.Exists(CmFilesList(0)) Then
        Try
            System.Diagnostics.Process.Start("explorer", "/select,""" & CmFilesList(0) & "")
        Catch ex As Exception
            MsgBox("Failed to open file!")
        End Try
    Else
        MsgBox("File not found!")
    End If
End Sub
#Region "Tab Header Color Change"

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
       ' e.Graphics.DrawRectangle(Pens.DarkGray, rect)
        'e.DrawFocusRectangle()

    End Using




End Sub


#End Region
Public Sub UnbindAllControls(ByRef container As Control)
  'Clear all of the controls within the container object
  'If "Recurse" is true, then also clear controls within any sub-containers
  Dim ctrl As Control = Nothing

  For Each ctrl In container.Controls

           ctrl.DataBindings.Clear()

           If ctrl.HasChildren Then
              UnbindAllControls(ctrl)
           End If

  Next

End Sub
Private Function GetSSMMAP( ByVal filePath As String , byref message As string) As Hvac.IHVACSteadyStateModel

      Dim ssmMap As New Hvac.HVACSteadyStateModel()


      Try

       If  ssmMap.SetValuesFromMap(FilePathUtils.ResolveFilePath(vectoPath,txtSSMFilePath.Text), message) then

         Return ssmMap  

       End If

       catch ex As Exception

       MessageBox.Show("Unable to retreive values from map")
        
      End Try

        Return Nothing

End Function
Private Sub SetupControls()


      Dim cIndex As Integer = 0

      gvElectricalConsumables.AutoGenerateColumns = False

     'ElectricalConsumerGrid 
     'Columns
     cIndex = gvElectricalConsumables.Columns.Add("Category", "Category")
     gvElectricalConsumables.Columns(cIndex).DataPropertyName = "Category"
     gvElectricalConsumables.Columns(cIndex).MinimumWidth = 150
     gvElectricalConsumables.Columns(cIndex).ReadOnly = True
     gvElectricalConsumables.Columns(cIndex).HeaderCell.Style.Alignment = DataGridViewContentAlignment.TopCenter
     gvElectricalConsumables.Columns(cIndex).HeaderCell.Style.Padding = New Padding(1, 2, 1, 1)

     cIndex = gvElectricalConsumables.Columns.Add("ConsumerName", "Name")

     gvElectricalConsumables.Columns(cIndex).DataPropertyName = "ConsumerName"
     gvElectricalConsumables.Columns(cIndex).MinimumWidth = 308
     gvElectricalConsumables.Columns(cIndex).ReadOnly = True
     gvElectricalConsumables.Columns(cIndex).HeaderCell.Style.Alignment = DataGridViewContentAlignment.TopCenter
     gvElectricalConsumables.Columns(cIndex).HeaderCell.Style.Padding = New Padding(1, 2, 1, 1)

     Dim baseVehicle As New DataGridViewCheckBoxColumn(False)
     baseVehicle.HeaderText = "Base Vehicle"
     cIndex = gvElectricalConsumables.Columns.Add(baseVehicle)
     gvElectricalConsumables.Columns(cIndex).DataPropertyName = "BaseVehicle"
     gvElectricalConsumables.Columns(cIndex).Width = 75
     gvElectricalConsumables.Columns(cIndex).HeaderCell.Style.Alignment = DataGridViewContentAlignment.TopCenter
     gvElectricalConsumables.Columns(cIndex).HeaderCell.Style.Padding = New Padding(1, 2, 1, 1)
     gvElectricalConsumables.Columns(cIndex).HeaderCell.ToolTipText = "Energy included in the calculations of base vehicle"

     cIndex = gvElectricalConsumables.Columns.Add("NominalConsumptionAmps", "Nominal Amps")
     gvElectricalConsumables.Columns(cIndex).DataPropertyName = "NominalConsumptionAmps"
     gvElectricalConsumables.Columns(cIndex).Width = 70
     gvElectricalConsumables.Columns(cIndex).HeaderCell.Style.Alignment = DataGridViewContentAlignment.TopCenter
     gvElectricalConsumables.Columns(cIndex).HeaderCell.Style.Padding = New Padding(1, 2, 1, 1)
     gvElectricalConsumables.Columns(cIndex).HeaderCell.ToolTipText = "Nominal consumption in AMPS"

     cIndex = gvElectricalConsumables.Columns.Add("PhaseIdle_TractionOn", "PhaseIdle/ TractionOn")
     gvElectricalConsumables.Columns(cIndex).DataPropertyName = "PhaseIdle_TractionOn"
     gvElectricalConsumables.Columns(cIndex).Width = 70
     gvElectricalConsumables.Columns(cIndex).HeaderCell.Style.Alignment = DataGridViewContentAlignment.TopCenter
     gvElectricalConsumables.Columns(cIndex).HeaderCell.Style.Padding = New Padding(1, 2, 1, 1)
     gvElectricalConsumables.Columns(cIndex).HeaderCell.ToolTipText = "Represents the amount of time (during engine fueling) as " & vbCrLf & "percentage that the consumer is active during the cycle."

     cIndex = gvElectricalConsumables.Columns.Add("NumberInActualVehicle", "Num in Vehicle")
     gvElectricalConsumables.Columns(cIndex).DataPropertyName = "NumberInActualVehicle"
     gvElectricalConsumables.Columns(cIndex).Width = 70
     gvElectricalConsumables.Columns(cIndex).HeaderCell.Style.Alignment = DataGridViewContentAlignment.TopCenter
     gvElectricalConsumables.Columns(cIndex).HeaderCell.Style.Padding = New Padding(1, 2, 1, 1)
     gvElectricalConsumables.Columns(cIndex).HeaderCell.ToolTipText = "Number of consumables of this" & vbCrLf & "type installed on the vehicle."

     'ResultCard Grids

     'Handler for deleting rows.


     'IDLE

     cIndex = gvResultsCardIdle.Columns.Add("Amps", "Amps")
     gvResultsCardIdle.Columns(cIndex).DataPropertyName = "Amps"
     gvResultsCardIdle.Columns(cIndex).Width = 65

     cIndex = gvResultsCardIdle.Columns.Add("SmartAmps", "SmartAmps")
     gvResultsCardIdle.Columns(cIndex).DataPropertyName = "SmartAmps"
     gvResultsCardIdle.Columns(cIndex).Width = 65

     'TRACTION
     cIndex = gvResultsCardTraction.Columns.Add("Amps", "Amps")
     gvResultsCardTraction.Columns(cIndex).DataPropertyName = "Amps"
     gvResultsCardTraction.Columns(cIndex).Width = 65

     cIndex = gvResultsCardTraction.Columns.Add("SmartAmps", "SmartAmps")
     gvResultsCardTraction.Columns(cIndex).DataPropertyName = "SmartAmps"
     gvResultsCardTraction.Columns(cIndex).Width = 65

     'OVERRUN
     cIndex = gvResultsCardOverrun.Columns.Add("Amps", "Amps")
     gvResultsCardOverrun.Columns(cIndex).DataPropertyName = "Amps"
     gvResultsCardOverrun.Columns(cIndex).Width = 65

     cIndex = gvResultsCardOverrun.Columns.Add("SmartAmps", "SmartAmps")
     gvResultsCardOverrun.Columns(cIndex).DataPropertyName = "SmartAmps"
     gvResultsCardOverrun.Columns(cIndex).Width = 65




End Sub
#Region "Binding Control"


Private Sub CreateBindings()

     'auxConfig.Vecto Bindings
     txtPowernetVoltage.DataBindings.Add("Text", auxConfig.ElectricalUserInputsConfig, "PowerNetVoltage")
     'txtVehicleWeightKG.DataBindings.Add("Text", auxConfig.VectoInputs, "VehicleWeightKG")
     'cboCycle.DataBindings.Add("Text", auxConfig.VectoInputs, "Cycle")
     txtFuelMap.DataBindings.Add("Text", auxConfig.VectoInputs, "FuelMap")

     'Electricals General
     txtAlternatorMapPath.DataBindings.Add("Text", auxConfig.ElectricalUserInputsConfig, "AlternatorMap")
     txtAlternatorGearEfficiency.DataBindings.Add("Text", auxConfig.ElectricalUserInputsConfig, "AlternatorGearEfficiency")
     txtDoorActuationTimeSeconds.DataBindings.Add("Text", auxConfig.ElectricalUserInputsConfig, "DoorActuationTimeSecond")
     chkSmartElectricals.DataBindings.Add("Checked", auxConfig.ElectricalUserInputsConfig, "SmartElectrical", False, DataSourceUpdateMode.OnPropertyChanged)


     'Electrical ConsumablesGrid
     Dim electricalConsumerBinding As New BindingList(Of IElectricalConsumer)(auxConfig.ElectricalUserInputsConfig.ElectricalConsumers.Items)
     gvElectricalConsumables.DataSource = electricalConsumerBinding



     'ResultCards

         'IDLE
         Dim idleBinding as BindingList(Of SmartResult)
         idleBinding = New BindingList(Of SmartResult)(auxConfig.ElectricalUserInputsConfig.ResultCardIdle.Results)
         idleBinding.AllowNew = True
         idleBinding.AllowRemove = True
         gvResultsCardIdle.DataSource = idleBinding

         'TRACTION
         Dim tractionBinding As BindingList(Of SmartResult)
         tractionBinding = New BindingList(Of SmartResult)(auxConfig.ElectricalUserInputsConfig.ResultCardTraction.Results)
         tractionBinding.AllowNew = True
         tractionBinding.AllowRemove = True
         gvResultsCardTraction.DataSource = tractionBinding

         'OVERRUN
         Dim overrunBinding As BindingList(Of SmartResult)
         overrunBinding = New BindingList(Of SmartResult)(auxConfig.ElectricalUserInputsConfig.ResultCardOverrun.Results)
         overrunBinding.AllowNew = True
         overrunBinding.AllowRemove = True
         gvResultsCardOverrun.DataSource = overrunBinding


        'Pneumatic Auxillaries Binding
        txtAdBlueNIperMinute.DataBindings.Add("Text", auxConfig.PneumaticAuxillariesConfig, "AdBlueNIperMinute")

        txtOverrunUtilisationForCompressionFraction.DataBindings.Add("Text", auxConfig.PneumaticAuxillariesConfig, "OverrunUtilisationForCompressionFraction")
        txtBrakingWithRetarderNIperKG.DataBindings.Add("Text", auxConfig.PneumaticAuxillariesConfig, "BrakingWithRetarderNIperKG")
        txtBrakingNoRetarderNIperKG.DataBindings.Add("Text", auxConfig.PneumaticAuxillariesConfig, "BrakingNoRetarderNIperKG")
        txtBreakingPerKneelingNIperKGinMM.DataBindings.Add("Text", auxConfig.PneumaticAuxillariesConfig, "BreakingPerKneelingNIperKGinMM", True, DataSourceUpdateMode.OnPropertyChanged, Nothing, "0.########")
        txtPerDoorOpeningNI.DataBindings.Add("Text", auxConfig.PneumaticAuxillariesConfig, "PerDoorOpeningNI")
        txtPerStopBrakeActuationNIperKG.DataBindings.Add("Text", auxConfig.PneumaticAuxillariesConfig, "PerStopBrakeActuationNIperKG")
        txtAirControlledSuspensionNIperMinute.DataBindings.Add("Text", auxConfig.PneumaticAuxillariesConfig, "AirControlledSuspensionNIperMinute")
        txtNonSmartRegenFractionTotalAirDemand.DataBindings.Add("Text", auxConfig.PneumaticAuxillariesConfig, "NonSmartRegenFractionTotalAirDemand")
        txtSmartRegenFractionTotalAirDemand.DataBindings.Add("Text", auxConfig.PneumaticAuxillariesConfig, "SmartRegenFractionTotalAirDemand")
        txtDeadVolumeLitres.DataBindings.Add("Text", auxConfig.PneumaticAuxillariesConfig, "DeadVolumeLitres")
        txtDeadVolBlowOutsPerLitresperHour.DataBindings.Add("Text", auxConfig.PneumaticAuxillariesConfig, "DeadVolBlowOutsPerLitresperHour")

        'Pneumatic UserInputsConfig Binding    
        txtCompressorMap.DataBindings.Add("Text", auxConfig.PneumaticUserInputsConfig, "CompressorMap")
        txtCompressorGearEfficiency.DataBindings.Add("Text", auxConfig.PneumaticUserInputsConfig, "CompressorGearEfficiency")
        txtCompressorGearRatio.DataBindings.Add("Text", auxConfig.PneumaticUserInputsConfig, "CompressorGearRatio")
        txtActuationsMap.DataBindings.Add("Text", auxConfig.PneumaticUserInputsConfig, "ActuationsMap")
        chkSmartAirCompression.DataBindings.Add("Checked", auxConfig.PneumaticUserInputsConfig, "SmartAirCompression", False, DataSourceUpdateMode.OnPropertyChanged)




        chkSmartRegeneration.DataBindings.Add("Checked", auxConfig.PneumaticUserInputsConfig, "SmartRegeneration", False, DataSourceUpdateMode.OnPropertyChanged)
        chkRetarderBrake.DataBindings.Add("Checked", auxConfig.PneumaticUserInputsConfig, "RetarderBrake")
        txtKneelingHeightMillimeters.DataBindings.Add("Text", auxConfig.PneumaticUserInputsConfig, "KneelingHeightMillimeters")
        cboAirSuspensionControl.DataBindings.Add("Text", auxConfig.PneumaticUserInputsConfig, "AirSuspensionControl", False)
        cboAdBlueDosing.DataBindings.Add("Text", auxConfig.PneumaticUserInputsConfig, "AdBlueDosing")
        cboDoors.DataBindings.Add("Text", auxConfig.PneumaticUserInputsConfig, "Doors")

        'HVAC Bindings     
        'txtHVACElectricalLoadPowerWatts.DataBindings.Add("Text", auxConfig.HvacUserInputsConfig.SteadyStateModel, "HVACElectricalLoadPowerWatts",False,DataSourceUpdateMode.OnPropertyChanged)
        'txtHVACFuellingLitresPerHour.DataBindings.Add("Text", auxConfig.HvacUserInputsConfig.SteadyStateModel, "HVACFuellingLitresPerHour",False,DataSourceUpdateMode.OnPropertyChanged)
        'txtHVACMechanicalLoadPowerWatts.DataBindings.Add("Text", auxConfig.HvacUserInputsConfig.SteadyStateModel, "HVACMechanicalLoadPowerWatts",False,DataSourceUpdateMode.OnPropertyChanged)

        txtSSMFilePath.DataBindings.Add( "Text", auxConfig.HvacUserInputsConfig,"SSMFilePath")
        txtBusDatabaseFilePath.DataBindings.Add("Text",auxConfig.HvacUserInputsConfig,"BusDatabasePath")

        'Signals



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


#End Region


'Open File with software defined in Config
Public Function FileOpenAlt(ByVal file As String) As Boolean
        Dim PSI As New ProcessStartInfo

        If Not IO.File.Exists(file) Then Return False

        PSI.FileName = "notepad.exe"
        PSI.Arguments = ChrW(34) & file & ChrW(34)
        Try
            Process.Start(PSI)
            Return True
        Catch ex As Exception
            Return False
        End Try

    End Function



End Class