

Imports System.IO
Imports System.Linq
Imports TUGraz.VECTO.Input_Files
Imports TUGraz.VectoCommon.InputData
Imports TUGraz.VectoCommon.Models
Imports TUGraz.VectoCommon.Utils
Imports TUGraz.VectoCore.InputData.FileIO.JSON
' Copyright 2017 European Union.
' Licensed under the EUPL (the 'Licence');
'
' * You may not use this work except in compliance with the Licence.
' * You may obtain a copy of the Licence at: http://ec.europa.eu/idabc/eupl
' * Unless required by applicable law or agreed to in writing,
'   software distributed under the Licence is distributed on an "AS IS" basis,
'   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
'
' See the LICENSE.txt for the specific language governing permissions and limitations.

''' <summary>
''' Engine Editor. Open and save .VENG files.
''' </summary>
''' <remarks></remarks>
Public Class BusAuxiliariesEngParametersForm
    Private _busAuxParamsFile As String = ""
    Public AutoSendTo As Boolean = False
    Public JobDir As String = ""
    Private _changed As Boolean = False

    Private _contextMenuFiles As String()

    Public Property JobType As VectoSimulationJobType


    'Before closing Editor: Check if file was changed and ask to save.
    Private Sub F_BusAux_FormClosing(sender As Object, e As FormClosingEventArgs) Handles Me.FormClosing
        If e.CloseReason <> CloseReason.ApplicationExitCall And e.CloseReason <> CloseReason.WindowsShutDown Then
            e.Cancel = ChangeCheckCancel()
        End If
    End Sub

    'Initialise.
    Private Sub BusAuxFormLoad(sender As Object, e As EventArgs) Handles Me.Load

        ' initialize form on load - nothng to do right now

        'pnInertia.Enabled = Not Cfg.DeclMode

        cbAlternatorTechnology.ValueMember = "Value"
        cbAlternatorTechnology.DisplayMember = "Label"
        cbAlternatorTechnology.DataSource =
            [Enum].GetValues(GetType(AlternatorType)).cast (of AlternatorType)().select(
                Function(type) new With {Key .Value = type, .Label = type.GetLabel()}).tolist()


        _changed = False

        bgPneumaticSystem.Enabled = True
        gbHVAC.Enabled = True
        cbES_HEVREESS.Enabled = True
        pnAlternatorEfficiency.Enabled = True
        pnMaxAlternatorPower.Enabled = True
        pnSmartElectricParams.Enabled = True
        pnAlternatorTechnology.Enabled = True
        pnCurrentDemand.Enabled = True

        select case JobType
            case VectoSimulationJobType.BatteryElectricVehicle:
            case VectoSimulationJobType.IEPC_E:
                bgPneumaticSystem.Enabled = False
                gbHVAC.Enabled = False
                cbES_HEVREESS.Checked = True
                cbES_HEVREESS.Enabled = False
                pnAlternatorEfficiency.Enabled = False
                pnMaxAlternatorPower.Enabled = False
                pnSmartElectricParams.Enabled = False
                pnAlternatorTechnology.Enabled = false
                pnCurrentDemand.Enabled = False
                
        end select

        NewBusAux()
    End Sub

    'Set generic values for Declaration mode.
    Private Sub DeclInit()

        If Not Cfg.DeclMode Then Exit Sub



    End Sub


#Region "Toolbar"

    Private Sub ToolStripBtNew_Click(sender As Object, e As EventArgs) Handles ToolStripBtNew.Click
        NewBusAux()
    End Sub

    Private Sub ToolStripBtOpen_Click(sender As Object, e As EventArgs) Handles ToolStripBtOpen.Click
        If EngineFileBrowser.OpenDialog(_busAuxParamsFile) Then
            Try
                OpenBusAuxParametersFile(EngineFileBrowser.Files(0))
            Catch ex As Exception
                MsgBox(ex.Message, MsgBoxStyle.OkOnly, "Error loading Engine File")
            End Try
        End If
    End Sub

    Private Sub ToolStripBtSave_Click(sender As Object, e As EventArgs) Handles ToolStripBtSave.Click
        SaveOrSaveAs(False)
    End Sub

    Private Sub ToolStripBtSaveAs_Click(sender As Object, e As EventArgs) Handles ToolStripBtSaveAs.Click
        SaveOrSaveAs(True)
    End Sub

    Private Sub ToolStripBtSendTo_Click(sender As Object, e As EventArgs) Handles ToolStripBtSendTo.Click

        If ChangeCheckCancel() Then Exit Sub

        If _busAuxParamsFile = "" Then
            If MsgBox("Save file now?", MsgBoxStyle.YesNo) = MsgBoxResult.Yes Then
                If Not SaveOrSaveAs(True) Then Exit Sub
            Else
                Exit Sub
            End If
        End If

        If Not VectoJobForm.Visible Then
            JobDir = ""
            VectoJobForm.Show()
            VectoJobForm.VectoNew()
        Else
            VectoJobForm.WindowState = FormWindowState.Normal
        End If

        VectoJobForm.TbENG.Text = GetFilenameWithoutDirectory(_busAuxParamsFile, JobDir)
    End Sub

    Private Sub ToolStripButton1_Click(sender As Object, e As EventArgs) Handles ToolStripButton1.Click
        If File.Exists(Path.Combine(MyAppPath, "User Manual\help.html")) Then
            Dim defaultBrowserPath As String = BrowserUtils.GetDefaultBrowserPath()
            Process.Start(defaultBrowserPath,
                          $"""file://{Path.Combine(MyAppPath, "User Manual\help.html#engine-editor")}""")
        Else
            MsgBox("User Manual not found!", MsgBoxStyle.Critical)
        End If
    End Sub

#End Region

    'Create new empty Engine file.
    Private Sub NewBusAux()

        If ChangeCheckCancel() Then Exit Sub


        'tbEquivalenceFactorDischarge.Text = ""
        'tbMinSoC.Text = ""
        'tbMaxSoC.Text = ""
        'tbTargetSoC.Text = ""
        'tbauxBufferTime.Text = ""
        'tbAuxBufferChargeTime.Text = ""

        DeclInit()

        _busAuxParamsFile = ""
        Text = "Bus Auxiliaries Parameters Editor"
        LbStatus.Text = ""
        pnSmartElectricParams.Enabled = false
        _changed = False
    End Sub

    'Open VENG file
    Public Sub OpenBusAuxParametersFile(file As String)

        If ChangeCheckCancel() Then Exit Sub

        Dim inputData As IBusAuxiliariesEngineeringData = JSONInputDataFactory.ReadEngineeringBusAuxiliaries(file)

        If Cfg.DeclMode Then
            Select Case WrongMode()
                Case 1
                    Close()
                    MainForm.RbDecl.Checked = Not MainForm.RbDecl.Checked
                    MainForm.OpenVectoFile(file)
                Case -1
                    Exit Sub
            End Select
        End If

        Dim basePath As String = Path.GetDirectoryName(file)


        tbCurrentDemand.Text = inputData.ElectricSystem.CurrentDemand.ToGUIFormat()
        tbCurrentDemandEngineOffDriving.Text = inputData.ElectricSystem.CurrentDemandEngineOffDriving.ToGUIFormat()
        tbCurrentDemandEngineOffStandstill.Text = inputData.ElectricSystem.CurrentDemandEngineOffStandstill.ToGUIFormat()
        tbDCDCEff.Text = inputData.ElectricSystem.DCDCConverterEfficiency.ToGUIFormat()
        if (JobType <> VectoSimulationJobType.BatteryElectricVehicle AndAlso JobType <> VectoSimulationJobType.IEPC_E) Then
            tbAlternatorEfficiency.Text = inputData.ElectricSystem.AlternatorEfficiency.ToGUIFormat()
            cbAlternatorTechnology.SelectedValue  = inputData.ElectricSystem.AlternatorType
            tbMaxAlternatorPower.Text = inputData.ElectricSystem.MaxAlternatorPower.ToGUIFormat()
            tbElectricStorageCapacity.Text = inputData.ElectricSystem.ElectricStorageCapacity.ConvertToWattHour().Value.ToGUIFormat()
            tbBatEfficiency.Text = inputData.ElectricSystem.ElectricStorageEfficiency.ToGuiFormat()

            tbCompressorMap.Text = GetRelativePath(inputData.PneumaticSystem.CompressorMap.Source, basePath)
            tbAverageAirDemand.Text = inputData.PneumaticSystem.AverageAirConsumed.ToGUIFormat()
            tbCompressorRatio.Text = inputData.PneumaticSystem.GearRatio.ToGUIFormat()
            cbSmartCompressor.Checked = inputData.PneumaticSystem.SmartAirCompression

            tbHvacElectricPowerDemand.Text = inputData.HVACData.ElectricalPowerDemand.ToGUIFormat()
            tbHvacMechPowerDemand.Text = inputData.HVACData.MechanicalPowerDemand.ToGUIFormat()
            tbHvacAuxHeaterPwr.Text = inputData.HVACData.AuxHeaterPower.ToGUIFormat()
            tbHvacHeatingDemand.Text = (inputData.HVACData.AverageHeatingDemand.Value() / 1e6).ToGUIFormat()

            pnSmartElectricParams.Enabled = inputData.ElectricSystem.AlternatorType = AlternatorType.Smart

            cbES_HEVREESS.Checked = inputData.ElectricSystem.ESSupplyFromHEVREESS
            pnDCDCEff.Enabled = cbES_HEVREESS.Checked
        End If
        

        DeclInit()

       
        Text = GetFilenameWithoutPath(file, True)
        LbStatus.Text = ""
        _busAuxParamsFile = file
        Activate()

        _changed = False
    End Sub

    'Save or Save As function = true if file is saved
    Private Function SaveOrSaveAs(ByVal saveAs As Boolean) As Boolean
        If _busAuxParamsFile = "" Or saveAs Then
            If BusAuxFileBrowser.SaveDialog(_busAuxParamsFile) Then
                _busAuxParamsFile = BusAuxFileBrowser.Files(0)
            Else
                Return False
            End If
        End If
        Return SaveParamsToFile(_busAuxParamsFile)
    End Function

    'Save VENG file to given filepath. Called by SaveOrSaveAs. 
    Private Function SaveParamsToFile(ByVal file As String) As Boolean

        Dim busAuxParams As BusAuxEngineeringParams = New BusAuxEngineeringParams
        busAuxParams.FilePath = file


        busAuxParams.AlternatorEfficiency = tbAlternatorEfficiency.Text.ToDouble(0)
        busAuxParams.CurrentDemandEngineOn = tbCurrentDemand.Text.ToDouble(0)
        busAuxParams.CurrentDemandEngineOffDriving = tbCurrentDemandEngineOffDriving.Text.ToDouble(0)
        busAuxParams.CurrentDemandEngineOffStandstill = tbCurrentDemandEngineOffStandstill.Text.ToDouble(0)
        busAuxParams.AlternatorType = CType(cbAlternatorTechnology.SelectedValue, AlternatorType)
        busAuxParams.MaxAlternatorPower = tbMaxAlternatorPower.Text.ToDouble(0)
        busAuxParams.ElectricStorageCapacity = tbElectricStorageCapacity.Text.ToDouble(0)
        busAuxParams.ElectricStorageEfficiency = tbBatEfficiency.Text.ToDouble(1)
        busAuxParams.DCDCEfficiency = tbDCDCEff.Text.ToDouble(0)
        busAuxParams.SupplyESFromHEVREESS = cbES_HEVREESS.Checked

        if (JobType = VectoSimulationJobType.IEPC_E OrElse JobType = VectoSimulationJobType.BatteryElectricVehicle) then
            busAuxParams.CompressorMap = Nothing
        Else 
            busAuxParams.CompressorMap = new SubPath()
            busAuxParams.PathCompressorMap = tbCompressorMap.Text
        End If
        busAuxParams.AverageAirDemand = tbAverageAirDemand.Text.ToDouble(0)
        busAuxParams.GearRatio = tbCompressorRatio.Text.ToDouble(0)
        busAuxParams.SmartCompression = cbSmartCompressor.Checked

        busAuxParams.ElectricPowerDemand = tbHvacElectricPowerDemand.Text.ToDouble(0)
        busAuxParams.MechanicalPowerDemand = tbHvacMechPowerDemand.Text.ToDouble(0)
        busAuxParams.AuxHeaterPower = tbHvacAuxHeaterPwr.Text.ToDouble(0)
        busAuxParams.AverageHeatingDemand = tbHvacHeatingDemand.Text.ToDouble(0)

        busAuxParams.JobType = JobType

        If Not busAuxParams.SaveFile Then
            MsgBox("Cannot save to " & file, MsgBoxStyle.Critical)
            Return False
        End If

        If AutoSendTo Then
            If VectoJobForm.Visible Then
                If UCase(FileRepl(VectoJobForm.tbBusAuxParams.Text, JobDir)) <> UCase(file) Then _
                    VectoJobForm.tbBusAuxParams.Text = GetFilenameWithoutDirectory(file, JobDir)
                VectoJobForm.UpdatePic()
            End If
        End If

        BusAuxFileBrowser.UpdateHistory(file)
        Text = GetFilenameWithoutPath(file, True)
        LbStatus.Text = ""

        _changed = False

        Return True
    End Function


#Region "Track changes"

    'Flags current file as modified.
    Private Sub Change()
        If Not _changed Then
            LbStatus.Text = "Unsaved changes in current file"
            _changed = True
        End If
    End Sub

    ' "Save changes ?" .... Returns True if User aborts
    Private Function ChangeCheckCancel() As Boolean

        If _changed Then
            Select Case MsgBox("Save changes ?", MsgBoxStyle.YesNoCancel)
                Case MsgBoxResult.Yes
                    Return Not SaveOrSaveAs(False)
                Case MsgBoxResult.Cancel
                    Return True
                Case Else 'MsgBoxResult.No
                    _changed = False
                    Return False
            End Select

        Else

            Return False

        End If
    End Function






#End Region



    'Save and close
    Private Sub ButOK_Click(sender As Object, e As EventArgs) Handles ButOK.Click
        If SaveOrSaveAs(False) Then Close()
    End Sub

    'Close without saving (see FormClosing Event)
    Private Sub ButCancel_Click(sender As Object, e As EventArgs) Handles ButCancel.Click
        Close()
    End Sub


#Region "Open File Context Menu"


    Private Sub OpenFiles(ParamArray files() As String)

        If files.Length = 0 Then Exit Sub

        _contextMenuFiles = files

        OpenWithToolStripMenuItem.Text = "Open with " & Cfg.OpenCmdName

        CmOpenFile.Show(Windows.Forms.Cursor.Position)
    End Sub

    Private Sub OpenWithToolStripMenuItem_Click(sender As Object, e As EventArgs) _
        Handles OpenWithToolStripMenuItem.Click
        If Not FileOpenAlt(_contextMenuFiles(0)) Then MsgBox("Failed to open file!")
    End Sub

    Private Sub ShowInFolderToolStripMenuItem_Click(sender As Object, e As EventArgs) _
        Handles ShowInFolderToolStripMenuItem.Click
        If File.Exists(_contextMenuFiles(0)) Then
            Try
                Process.Start("explorer", "/select,""" & _contextMenuFiles(0) & "")
            Catch ex As Exception
                MsgBox("Failed to open file!")
            End Try
        Else
            MsgBox("File not found!")
        End If
    End Sub


    Private Sub cbAlternatorTechnology_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cbAlternatorTechnology.SelectedIndexChanged

        Select CType(cbAlternatorTechnology.SelectedValue, AlternatorType)
            Case AlternatorType.Conventional:
                pnSmartElectricParams.Enabled = false  
                
            Case AlternatorType.Smart:
                pnSmartElectricParams.Enabled = true    
            
            Case AlternatorType.None:
                pnSmartElectricParams.Enabled = false    
        End Select

    End Sub

    Private Sub cbES_HEVREESS_CheckedChanged(sender As Object, e As EventArgs) Handles cbES_HEVREESS.CheckedChanged
        pnDCDCEff.Enabled = cbES_HEVREESS.Checked
    End Sub

    Private Sub btnBrowseCompressorMap_Click(sender As Object, e As EventArgs) Handles btnBrowseCompressorMap.Click
        If BusAuxCompressorMapFileBrowser.OpenDialog(FileRepl(tbCompressorMap.Text, GetPath(_busAuxParamsFile))) Then _
            tbCompressorMap.Text = GetFilenameWithoutDirectory(BusAuxCompressorMapFileBrowser.Files(0), GetPath(_busAuxParamsFile))

    End Sub

#End Region





End Class
