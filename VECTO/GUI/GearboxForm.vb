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
Imports System.Collections.Generic
Imports System.Drawing.Imaging
Imports System.Globalization
Imports System.IO
Imports System.Linq
Imports System.Windows.Forms.DataVisualization.Charting
Imports System.Xml.Linq
Imports Ninject
Imports TUGraz.VECTO.Input_Files
Imports TUGraz.VectoCommon.InputData
Imports TUGraz.VectoCommon.Models
Imports TUGraz.VectoCommon.Utils
Imports TUGraz.VectoCore
Imports TUGraz.VectoCore.InputData.FileIO.JSON
Imports TUGraz.VectoCore.InputData.Impl
Imports TUGraz.VectoCore.InputData.Reader.ComponentData

Imports TUGraz.VectoCore.Models.Declaration
Imports TUGraz.VectoCore.Models.Simulation
Imports TUGraz.VectoCore.Models.Simulation.Data
Imports TUGraz.VectoCore.Models.Simulation.Impl
Imports TUGraz.VectoCore.Models.SimulationComponent
Imports TUGraz.VectoCore.Models.SimulationComponent.Data
Imports TUGraz.VectoCore.Models.SimulationComponent.Data.ElectricMotor
Imports TUGraz.VectoCore.Models.SimulationComponent.Data.Engine
Imports TUGraz.VectoCore.Models.SimulationComponent.Data.Gearbox
Imports TUGraz.VectoCore.Models.SimulationComponent.Impl
Imports TUGraz.VectoCore.OutputData.FileIO
Imports TUGraz.VectoCore.OutputData.XML
Imports TUGraz.VectoCore.OutputData.XML.Engineering.Interfaces


''' <summary>
''' Gearbox Editor
''' </summary>
''' <remarks></remarks>
Public Class GearboxForm
    Private Enum GearboxTbl
        GearNr = 0
        'TorqueConverter = 1
        Ratio = 1
        LossMapEfficiency = 2
        ShiftPolygons = 3
        MaxTorque = 4
        MaxSpeed = 5
    End Enum

    Private _gbxFile As String = ""
    Public AutoSendTo As Boolean = False
    Public JobDir As String = ""
    Private _gearDialog As GearboxGearDialog

    Private _changed As Boolean = False

    'Before closing Editor: Check if file was changed and ask to save.
    Private Sub F_GBX_FormClosing(sender As Object, e As FormClosingEventArgs) Handles Me.FormClosing
        If e.CloseReason <> CloseReason.ApplicationExitCall And e.CloseReason <> CloseReason.WindowsShutDown Then
            e.Cancel = ChangeCheckCancel()
        End If
    End Sub

    'Initialise.
    Private Sub F_GBX_Load(sender As Object, e As EventArgs) Handles Me.Load

        _gearDialog = New GearboxGearDialog

        PnInertiaTI.Enabled = Not Cfg.DeclMode
        GrGearShift.Enabled = Not Cfg.DeclMode
        'ChTCon.Enabled = Not Cfg.DeclMode

        CbGStype.Items.Clear()
        CbGStype.ValueMember = "Value"
        CbGStype.DisplayMember = "Label"

       ' CbGStype.DataSource = [Enum].GetValues(GetType(GearboxType)) _
       '     .Cast(Of GearboxType)() _
       '     .Where(Function(type) type.ManualTransmission() OrElse type.AutomaticTransmission() OrElse type = GearboxType.IHPC OrElse type = GearboxType.IEPC) _
       '     .Select(Function(type) New With {Key .Value = type, .Label = type.GetLabel()}).ToList()
        
        SetupTransmissionTypes()
        DeclInit()

        _changed = False
        NewGbx()
    End Sub

    private sub SetupTransmissionTypes()
        Select case VectoJobForm.JobType
            Case VectoSimulationJobType.ConventionalVehicle 
                CbGStype.DataSource = [Enum].GetValues(GetType(GearboxType)) _
                .Cast(Of GearboxType)() _
                .Where(Function(type) Not type = GearboxType.APTN And (type.ManualTransmission() OrElse type = GearboxType.ATSerial OrElse type = GearboxType.ATPowerSplit)) _
                .Select(Function(type) New With {Key .Value = type, .Label = type.GetLabel()}).ToList()

            Case VectoSimulationJobType.IHPC
                CbGStype.DataSource = [Enum].GetValues(GetType(GearboxType)) _
                    .Cast(Of GearboxType)() _
                    .Where(Function(type)  type = GearboxType.IHPC ) _
                    .Select(Function(type) New With {Key .Value = type, .Label = type.GetLabel()}).ToList()

            Case VectoSimulationJobType.IEPC_E, VectoSimulationJobType.IEPC_S, VectoSimulationJobType.FCHV_IEPC
                CbGStype.DataSource = [Enum].GetValues(GetType(GearboxType)) _
                    .Cast(Of GearboxType)() _
                    .Where(Function(type)  type = GearboxType.IEPC) _
                    .Select(Function(type) New With {Key .Value = type, .Label = type.GetLabel()}).ToList()

            Case VectoSimulationJobType.ParallelHybridVehicle
                CbGStype.DataSource = [Enum].GetValues(GetType(GearboxType)) _
                    .Cast(Of GearboxType)() _
                    .Where(Function(type) type = GearboxType.AMT OrElse (type.AutomaticTransmission() and not type = GearboxType.IHPC And Not type = GearboxType.APTN)) _
                    .Select(Function(type) New With {Key .Value = type, .Label = type.GetLabel()}).ToList()

            Case  VectoSimulationJobType.SerialHybridVehicle
                CbGStype.DataSource = [Enum].GetValues(GetType(GearboxType)) _
                    .Cast(Of GearboxType)() _
                    .Where(Function(type) type = GearboxType.AMT OrElse type.AutomaticTransmission() and not type = GearboxType.IHPC ) _
                    .Select(Function(type) New With {Key .Value = type, .Label = type.GetLabel()}).ToList()

            Case VectoSimulationJobType.BatteryElectricVehicle, VectoSimulationJobType.FCHV
                CbGStype.DataSource = [Enum].GetValues(GetType(GearboxType)) _
                    .Cast(Of GearboxType)() _
                    .Where(Function(type)  type = GearboxType.AMT OrElse type.AutomaticTransmission() and not type = GearboxType.IHPC  ) _
                    .Select(Function(type) New With {Key .Value = type, .Label = type.GetLabel()}).ToList()

            Case Else
                 CbGStype.DataSource = [Enum].GetValues(GetType(GearboxType)) _
                     .Cast(Of GearboxType)() _
                     .Where(Function(type) type.ManualTransmission() OrElse type.AutomaticTransmission() OrElse type = GearboxType.IHPC OrElse type = GearboxType.IEPC) _
                     .Select(Function(type) New With {Key .Value = type, .Label = type.GetLabel()}).ToList()

        End Select

    End sub

    'Set generic values for Declaration mode.
    Private Sub DeclInit()
        Dim gbxType As GearboxType
        Dim lv0 As ListViewItem

        If Not Cfg.DeclMode Then Exit Sub

        TBI_getr.Text = DeclarationData.Gearbox.Inertia.ToGUIFormat()	'cDeclaration.GbInertia

        gbxType = CType(CbGStype.SelectedValue, GearboxType)	'CType(Me.CbGStype.SelectedIndex, tGearbox)

        TbTracInt.Text = gbxType.TractionInterruption().ToGUIFormat()
        TbMinTimeBetweenShifts.Text = DeclarationData.Gearbox.MinTimeBetweenGearshifts.ToGUIFormat()
        'cDeclaration.MinTimeBetweenGearshift(GStype)

        TbTqResv.Text = (DeclarationData.GearboxTCU.TorqueReserve * 100).ToGUIFormat()											  ' cDeclaration.TqResv
        TbTqResvStart.Text = (DeclarationData.GearboxTCU.TorqueReserveStart * 100).ToGUIFormat() 'cDeclaration.TqResvStart
        TbStartSpeed.Text = DeclarationData.GearboxTCU.StartSpeed.ToGUIFormat()	'cDeclaration.StartSpeed
        TbStartAcc.Text = DeclarationData.GearboxTCU.StartAcceleration.ToGUIFormat()	' cDeclaration.StartAcc

        tbUpshiftMinAcceleration.Text = DeclarationData.Gearbox.UpshiftMinAcceleration.ToGUIFormat()
        tbTCCUpshiftMinAcceleration.Text = ""
        tbTCLUpshiftMinAcceleration.Text = ""

        tbDownshiftAfterUpshift.Text = DeclarationData.Gearbox.DownshiftAfterUpshiftDelay.ToGUIFormat()
        tbUpshiftAfterDownshift.Text = DeclarationData.Gearbox.UpshiftAfterDownshiftDelay.ToGUIFormat()

        'ChTCon.Checked = GStype.AutomaticTransmission()
        For Each lv0 In LvGears.Items
            lv0.SubItems(GearboxTbl.ShiftPolygons).Text = ""
        Next
    End Sub

#Region "Toolbar"

    Private Sub ToolStripBtNew_Click(sender As Object, e As EventArgs) Handles ToolStripBtNew.Click
        NewGbx()
    End Sub

    Private Sub ToolStripBtOpen_Click(sender As Object, e As EventArgs) Handles ToolStripBtOpen.Click
        If GearboxFileBrowser.OpenDialog(_gbxFile) Then
            Try
                OpenGbx(GearboxFileBrowser.Files(0), VehicleCategory.RigidTruck, VectoSimulationJobType.ConventionalVehicle)
            Catch ex As Exception
                MsgBox("Failed to open Gearbox File: " + ex.Message)
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

        If _gbxFile = "" Then
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

        VectoJobForm.TbGBX.Text = GetFilenameWithoutDirectory(_gbxFile, JobDir)
    End Sub

    'Help
    Private Sub ToolStripButton1_Click(sender As Object, e As EventArgs) Handles ToolStripButton1.Click
		If File.Exists(Path.Combine(MyAppPath,"User Manual\help.html")) Then
            Dim defaultBrowserPath As String = BrowserUtils.GetDefaultBrowserPath()
            Process.Start(defaultBrowserPath,
                          $"""file://{Path.Combine(MyAppPath, "User Manual\help.html#gearbox-editor")}""")
        Else
            MsgBox("User Manual not found!", MsgBoxStyle.Critical)
        End If
    End Sub

#End Region

    'New file
    Private Sub NewGbx()
        'Dim lvi As ListViewItem

        If ChangeCheckCancel() Then Exit Sub

        'CbGStype.SelectedIndex = 0

        TbName.Text = ""
        TbTracInt.Text = ""
        TBI_getr.Text = ""

        LvGears.Items.Clear()

        LvGears.Items.Add(CreateListviewItem("Axle", 1, "1", "", "", ""))

        'Me.ChSkipGears.Checked = False         'set by CbGStype.SelectedIndexChanged
        'Me.ChShiftInside.Checked = False       'set by CbGStype.SelectedIndexChanged
        TbTqResv.Text = (DeclarationData.GearboxTCU.TorqueReserve * 100).ToGUIFormat()
        TbMinTimeBetweenShifts.Text = DeclarationData.Gearbox.MinTimeBetweenGearshifts.ToGUIFormat()
        TbTqResvStart.Text = (DeclarationData.GearboxTCU.TorqueReserveStart * 100).ToGUIFormat()
        TbStartSpeed.Text = DeclarationData.GearboxTCU.StartSpeed.ToGUIFormat() ' in m/s!
        TbStartAcc.Text = DeclarationData.GearboxTCU.StartAcceleration.ToGUIFormat()

        tbUpshiftMinAcceleration.Text = DeclarationData.Gearbox.UpshiftMinAcceleration.ToGUIFormat()
        tbTCLUpshiftMinAcceleration.Text = ""
        tbTCCUpshiftMinAcceleration.Text = ""

        tbDownshiftAfterUpshift.Text = DeclarationData.Gearbox.DownshiftAfterUpshiftDelay.ToGUIFormat()
        tbUpshiftAfterDownshift.Text = DeclarationData.Gearbox.UpshiftAfterDownshiftDelay.ToGUIFormat()

        'ChTCon.Checked = False				'set by CbGStype.SelectedIndexChanged
        TbTCfile.Text = ""
        TbTCrefrpm.Text = ""
        TbTCinertia.Text = ""

        DeclInit()

        _gbxFile = ""
        Text = "GBX Editor"
        LbStatus.Text = ""


        _changed = False
        Try
            UpdatePic()
        Catch
        End Try
    End Sub

    'Open file
    Public Sub OpenGbx(file As String, vehicleCategory As VehicleCategory, vehicleJobType As VectoSimulationJobType)

        If ChangeCheckCancel() Then Exit Sub

        Dim inputData As IEngineeringInputDataProvider = TryCast(JSONInputDataFactory.ReadComponentData(file), 
                                                                IEngineeringInputDataProvider)
        Dim vehicle As IVehicleEngineeringInputData = inputData.JobInputData.Vehicle
		Dim gearbox As IGearboxEngineeringInputData = vehicle.Components.GearboxInputData
	    Dim shiftParams As IGearshiftEngineeringInputData = TryCast(gearbox, IGearshiftEngineeringInputData)
		Dim axlegear As IAxleGearInputData = vehicle.Components.AxleGearInputData

        _vehicleCategory = vehicleCategory
        _vehicleJobType = vehicleJobType

        If Cfg.DeclMode <> gearbox.SavedInDeclarationMode Then
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
        TbName.Text = gearbox.Model
        TbTracInt.Text = gearbox.TractionInterruption.ToGUIFormat()
        TBI_getr.Text = gearbox.Inertia.ToGUIFormat()

        LvGears.Items.Clear()

        Dim lossmap As String = ""
        Try
            lossmap = If(axlegear.LossMap Is Nothing, axlegear.Efficiency.ToGUIFormat(),
                        GetRelativePath(axlegear.LossMap.Source, basePath))
        Catch ex As Exception
        End Try

        LvGears.Items.Add(CreateListviewItem("Axle", axlegear.Ratio, lossmap, "", "", ""))

        For Each gear As ITransmissionInputData In gearbox.Gears
            lossmap = ""
            Try
                lossmap = If(gear.LossMap Is Nothing, gear.Efficiency.ToGUIFormat(), GetRelativePath(gear.LossMap.Source, basePath))
            Catch ex As Exception

            End Try
            LvGears.Items.Add(CreateListviewItem(gear.Gear.ToString("00"), gear.Ratio,
                                                lossmap,
                                                If(gear.ShiftPolygon Is Nothing, "", GetRelativePath(gear.ShiftPolygon.Source, basePath)),
                                                If(gear.MaxTorque Is Nothing, "", gear.MaxTorque.ToGUIFormat()),
                                                If(gear.MaxInputSpeed Is Nothing, "", gear.MaxInputSpeed.AsRPM.ToGUIFormat())))
        Next

		TbTqResv.Text = (shiftParams.TorqueReserve * 100).ToGUIFormat()
		TbMinTimeBetweenShifts.Text = shiftParams.MinTimeBetweenGearshift.ToGUIFormat()
		TbTqResvStart.Text = (shiftParams.StartTorqueReserve * 100).ToGUIFormat()
		TbStartSpeed.Text = shiftParams.StartSpeed.ToGUIFormat()
		TbStartAcc.Text = shiftParams.StartAcceleration.ToGUIFormat()

		Dim torqueConverter As ITorqueConverterEngineeringInputData = vehicle.Components.TorqueConverterInputData
        If torqueConverter Is Nothing OrElse gearbox.Type.ManualTransmission() Then
            TbTCfile.Text = ""
            TbTCrefrpm.Text = ""
            TbTCinertia.Text = ""
            TBTCShiftPolygon.Text = ""
            tbTCmaxSpeed.Text = ""
            tbTCLUpshiftMinAcceleration.Text = ""
            tbTCCUpshiftMinAcceleration.Text = ""

        Else
            TbTCfile.Text = If(torqueConverter.TCData Is Nothing, "", GetRelativePath(torqueConverter.TCData.Source, basePath))
            TbTCrefrpm.Text = If(torqueConverter.ReferenceRPM Is Nothing, "", torqueConverter.ReferenceRPM.AsRPM.ToGUIFormat())
            TbTCinertia.Text = If(torqueConverter.Inertia Is Nothing, "", torqueConverter.Inertia.ToGUIFormat())
            TBTCShiftPolygon.Text =
                If(torqueConverter.ShiftPolygon Is Nothing, "", GetRelativePath(torqueConverter.ShiftPolygon.Source, basePath))
            tbTCmaxSpeed.Text =
                If(torqueConverter.MaxInputSpeed Is Nothing, "", torqueConverter.MaxInputSpeed.AsRPM.ToGUIFormat())
			tbTCLUpshiftMinAcceleration.Text = shiftParams.CLUpshiftMinAcceleration.ToGUIFormat()
			tbTCCUpshiftMinAcceleration.Text = shiftParams.CCUpshiftMinAcceleration.ToGUIFormat()
        End If

		tbUpshiftMinAcceleration.Text = shiftParams.UpshiftMinAcceleration.ToGUIFormat()
		tbDownshiftAfterUpshift.Text = shiftParams.DownshiftAfterUpshiftDelay.ToGUIFormat()
		tbUpshiftAfterDownshift.Text = shiftParams.UpshiftAfterDownshiftDelay.ToGUIFormat()

        tbATShiftTime.Text = gearbox.PowershiftShiftTime.ToGUIFormat()

        CbGStype.SelectedValue = gearbox.Type


        DeclInit()


        GearboxFileBrowser.UpdateHistory(file)
        Text = GetFilenameWithoutPath(file, True)
        LbStatus.Text = ""
        UpdateGearboxInfoText()
        _gbxFile = file
        Activate()

        _changed = False
        Try
            UpdatePic()
        Catch
        End Try
    End Sub

    Private Function CreateListviewItem(gear As String, ratio As Double, getrMap As String,
                                        shiftPolygon As String, maxTorque As String, maxSpeed As String) As ListViewItem
        Dim retVal As ListViewItem = New ListViewItem(gear)
        'retVal.SubItems.Add(tc)
        retVal.SubItems.Add(ratio.ToGUIFormat())
        retVal.SubItems.Add(getrMap)
        retVal.SubItems.Add(shiftPolygon)
        retVal.SubItems.Add(maxTorque)
        retVal.SubItems.Add(maxSpeed)
        Return retVal
    End Function

    'Save or Save As function = true if file is saved
    Private Function SaveOrSaveAs(saveAs As Boolean) As Boolean
        If _gbxFile = "" Or saveAs Then
            If GearboxFileBrowser.SaveDialog(_gbxFile) Then
                _gbxFile = GearboxFileBrowser.Files(0)
            Else
                Return False
            End If
        End If
        Return SaveGbx(_gbxFile)
    End Function

    'Save file
    Private Function SaveGbx(file As String) As Boolean

        Dim gearbox As Gearbox = FillGearboxData(file)


        If Not gearbox.SaveFile Then
            MsgBox("Cannot save to " & file, MsgBoxStyle.Critical)
            Return False
        End If

        If AutoSendTo Then
            If VectoJobForm.Visible Then
                If UCase(FileRepl(VectoJobForm.TbGBX.Text, JobDir)) <> UCase(file) Then _
                    VectoJobForm.TbGBX.Text = GetFilenameWithoutDirectory(file, JobDir)
                VectoJobForm.UpdatePic()
            End If
        End If

        GearboxFileBrowser.UpdateHistory(file)
        Text = GetFilenameWithoutPath(file, True)
        LbStatus.Text = ""

        _changed = False

        Return True
    End Function

    Private Function FillGearboxData(file As String) As Gearbox
        Dim gearbox As Gearbox
        Dim i As Integer

        gearbox = New Gearbox
        gearbox.FilePath = file

        gearbox.ModelName = TbName.Text
        If Trim(gearbox.ModelName) = "" Then gearbox.ModelName = "Undefined"

        gearbox.TracIntrSi = TbTracInt.Text.ToDouble(0)
        gearbox.GbxInertia = TBI_getr.Text.ToDouble(0)

        For i = 0 To LvGears.Items.Count - 1
            'GBX0.IsTCgear.Add(Me.LvGears.Items(i).SubItems(GearboxTbl.TorqueConverter).Text = "on" And i > 0)
            gearbox.GearRatios.Add(LvGears.Items(i).SubItems(GearboxTbl.Ratio).Text.ToDouble(0))
            gearbox.GearLossmaps.Add(New SubPath)
            gearbox.GearLossMap(i) = LvGears.Items(i).SubItems(GearboxTbl.LossMapEfficiency).Text
            gearbox.GearshiftFiles.Add(New SubPath)
            gearbox.ShiftPolygonFile(i) = LvGears.Items(i).SubItems(GearboxTbl.ShiftPolygons).Text
            'GBX0.FldFiles.Add(New cSubPath)
            'GBX0.FldFile(i) = Me.LvGears.Items(i).SubItems(GearboxTbl.MaxTorque).Text
            gearbox.MaxTorque.Add(LvGears.Items(i).SubItems(GearboxTbl.MaxTorque).Text)
            gearbox.MaxSpeed.Add(LvGears.Items(i).SubItems(GearboxTbl.MaxSpeed).Text)
        Next

        gearbox.TorqueResv = TbTqResv.Text.ToDouble(0)
        gearbox.ShiftTime = TbMinTimeBetweenShifts.Text.ToDouble(0)
        gearbox.TorqueResvStart = TbTqResvStart.Text.ToDouble(0)
        gearbox.StartSpeed = TbStartSpeed.Text.ToDouble(0)
        gearbox.StartAcc = TbStartAcc.Text.ToDouble(0)

        gearbox.Type = CType(CbGStype.SelectedValue, GearboxType)

        gearbox.Type.AutomaticTransmission()
        gearbox.TorqueConverterFile = TbTCfile.Text
        gearbox.TorqueConverterReferenceRpm = TbTCrefrpm.Text.ToDouble(0)
        gearbox.TorqueConverterInertia = TbTCinertia.Text.ToDouble(0)
        gearbox.TorqueConverterShiftPolygonFile = TBTCShiftPolygon.Text
        gearbox.TorqueConverterMaxSpeed = tbTCmaxSpeed.Text.ToDouble(0)

        gearbox.DownshiftAfterUpshift = tbDownshiftAfterUpshift.Text.ToDouble(0)
        gearbox.UpshiftAfterDownshift = tbUpshiftAfterDownshift.Text.ToDouble(0)

        gearbox.UpshiftMinAcceleration = tbUpshiftMinAcceleration.Text.ToDouble(0)
        gearbox.TCLUpshiftMinAcceleration = tbTCLUpshiftMinAcceleration.Text.ToDouble(0)
        gearbox.TCCUpshiftMinAcceleration = tbTCCUpshiftMinAcceleration.Text.ToDouble(0)

        gearbox.PSShiftTime = tbATShiftTime.Text.ToDouble(0)
        Return gearbox
    End Function

#Region "Change Events"

    'Change Status ändern |@@| Change Status change
    Private Sub Change()
        If Not _changed Then
            LbStatus.Text = "Unsaved changes in current file"
            _changed = True
        End If
    End Sub

    ' "Save changes ?" ...liefert True wenn User Vorgang abbricht |@@| Save changes? "... Returns True if user aborts
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

    Private Sub TbName_TextChanged(sender As Object, e As EventArgs) _
        Handles TbName.TextChanged, TBI_getr.TextChanged, TbTracInt.TextChanged, TbTqResv.TextChanged,
                TbMinTimeBetweenShifts.TextChanged, TbTqResvStart.TextChanged, TbStartSpeed.TextChanged, TbStartAcc.TextChanged,
                TbTCfile.TextChanged,
                tbTCCUpshiftMinAcceleration.TextChanged, tbTCLUpshiftMinAcceleration.TextChanged
        Change()
    End Sub

#End Region

    'Save and close
    Private Sub ButOK_Click(sender As Object, e As EventArgs) Handles ButOK.Click
        If SaveOrSaveAs(False) Then Close()
    End Sub

    'Cancel
    Private Sub ButCancel_Click(sender As Object, e As EventArgs) Handles ButCancel.Click
        Close()
    End Sub

    'Enable/Disable settings for specific transmission types
    Private Sub CbGStype_SelectedIndexChanged(sender As Object, e As EventArgs) _
        Handles CbGStype.SelectedIndexChanged
        Dim gStype As GearboxType = CType(CbGStype.SelectedValue, GearboxType)

        Change()

        Dim hasTorqueConverter = gStype.AutomaticTransmission() AndAlso gStype <> GearboxType.APTN AndAlso gStype <> GearboxType.IHPC
        'ChTCon.Enabled = (GStype.AutomaticTransmission())
        gbTC.Enabled = hasTorqueConverter
        pnTcEngineering.Enabled = Not Cfg.DeclMode AndAlso hasTorqueConverter
        gbTCAccMin.Enabled = Not Cfg.DeclMode AndAlso hasTorqueConverter
        gbPowershiftLosses.Enabled = Not Cfg.DeclMode AndAlso hasTorqueConverter
        TbStartAcc.Enabled = Not hasTorqueConverter
        TbStartSpeed.Enabled = Not hasTorqueConverter
        TbTqResv.Enabled = Not hasTorqueConverter
        GroupBox2.Enabled = Not hasTorqueConverter
        TBI_getr.Enabled = Not hasTorqueConverter
        TbTracInt.Enabled = Not hasTorqueConverter
        tbDownshiftAfterUpshift.Enabled = Not hasTorqueConverter
        tbUpshiftAfterDownshift.Enabled = Not hasTorqueConverter
        UpdateGearboxInfoText()
    End Sub

    Private Sub UpdateGearboxInfoText()
        Dim gStype As GearboxType = CType(CbGStype.SelectedValue, GearboxType)

        Dim text As String = ""
        If (gStype = GearboxType.ATSerial) Then
            If LvGears.Items.Count > 2 Then
                Dim ratio1 As Double = LvGears.Items.Item(1).SubItems(GearboxTbl.Ratio).Text.ToDouble(0)
                Dim ratio2 As Double = LvGears.Items.Item(2).SubItems(GearboxTbl.Ratio).Text.ToDouble(0)

                If ratio1 / ratio2 >= DeclarationData.Gearbox.TorqueConverterSecondGearThreshold(_vehicleCategory) Then
                    text = "Torque converter is used in 1st and 2nd gear"
                Else
                    text = "Torque converter is used in 1st gear only"
                End If
            End If
        End If
        lblGbxInfo.Text = text
    End Sub


    Private Sub LvGears_SelectedIndexChanged(sender As Object, e As EventArgs) _
        Handles LvGears.SelectedIndexChanged
        Try
            UpdatePic()
            btExportVGBS.Enabled = True
        Catch
            btExportVGBS.Enabled = False
        End Try
    End Sub

#Region "Gears"

    'Gear-DoubleClick
    Private Sub LvGears_MouseDoubleClick(ByVal sender As Object, ByVal e As MouseEventArgs) _
        Handles LvGears.MouseDoubleClick
        EditGear()
    End Sub

    'Gear-KeyDown
    Private Sub LvGears_KeyDown(sender As Object, e As KeyEventArgs) Handles LvGears.KeyDown
        Select Case e.KeyCode
            Case Keys.Delete, Keys.Back
                RemoveGear(False)
            Case Keys.Enter
                EditGear()
        End Select
    End Sub

    'Remove Gear Button
    Private Sub BtClearGear_Click(sender As Object, e As EventArgs) Handles BtRemGear.Click
        RemoveGear(False)
    End Sub

    'Add Gear button
    Private Sub BtAddGear_Click(sender As Object, e As EventArgs) Handles BtAddGear.Click
        AddGear()
        LvGears.Items(LvGears.Items.Count - 1).Selected = True
        EditGear()
    End Sub

    'Edit Gear
    Private Sub EditGear()

        Do

            'GearDia.ChIsTCgear.Enabled = (Me.ChTCon.Checked And Me.LvGears.SelectedIndices(0) > 0)
            _gearDialog.GearboxType = CType(CbGStype.SelectedValue, GearboxType)
            _gearDialog.PnShiftPoly.Enabled = (Not Cfg.DeclMode And LvGears.SelectedIndices(0) > 0)
            _gearDialog.PnFld.Enabled = (LvGears.SelectedIndices(0) > 0)
            _gearDialog.GbxPath = GetPath(_gbxFile)
            _gearDialog.TbGear.Text = LvGears.SelectedItems(0).SubItems(GearboxTbl.GearNr).Text
            _gearDialog.TbRatio.Text = LvGears.SelectedItems(0).SubItems(GearboxTbl.Ratio).Text
            _gearDialog.TbMapPath.Text = LvGears.SelectedItems(0).SubItems(GearboxTbl.LossMapEfficiency).Text
            If LvGears.SelectedIndices(0) > 0 Then
                _gearDialog.TbShiftPolyFile.Text = LvGears.SelectedItems(0).SubItems(GearboxTbl.ShiftPolygons).Text
                _gearDialog.TbMaxTorque.Text = LvGears.SelectedItems(0).SubItems(GearboxTbl.MaxTorque).Text
                _gearDialog.tbMaxSpeed.Text = LvGears.SelectedItems(0).SubItems(GearboxTbl.MaxSpeed).Text
            Else
                _gearDialog.TbShiftPolyFile.Text = ""
                _gearDialog.TbMaxTorque.Text = ""
                _gearDialog.tbMaxSpeed.Text = ""
            End If

            If LvGears.SelectedItems(0).Index = 0 Then
                _gearDialog.BtPrevious.Enabled = False
            Else
                _gearDialog.BtPrevious.Enabled = True
            End If

            If _gearDialog.ShowDialog = DialogResult.OK Then
                'Me.LvGears.SelectedItems(0).SubItems(GearboxTbl.TorqueConverter).Text = "-"
                LvGears.SelectedItems(0).SubItems(GearboxTbl.Ratio).Text = _gearDialog.TbRatio.Text
                LvGears.SelectedItems(0).SubItems(GearboxTbl.LossMapEfficiency).Text = _gearDialog.TbMapPath.Text
                LvGears.SelectedItems(0).SubItems(GearboxTbl.ShiftPolygons).Text = _gearDialog.TbShiftPolyFile.Text
                LvGears.SelectedItems(0).SubItems(GearboxTbl.MaxTorque).Text = _gearDialog.TbMaxTorque.Text
                LvGears.SelectedItems(0).SubItems(GearboxTbl.MaxSpeed).Text = _gearDialog.tbMaxSpeed.Text

                UpdateGearboxInfoText()
                Try
                    UpdatePic()
                Catch
                End Try

                Change()

            Else

                If LvGears.SelectedItems(0).SubItems(GearboxTbl.Ratio).Text = "" Then RemoveGear(True)

            End If

            If _gearDialog.NextGear Then
                If LvGears.Items.Count - 1 = LvGears.SelectedIndices(0) Then AddGear()

                LvGears.Items(LvGears.SelectedIndices(0) + 1).Selected = True
            End If

            If _gearDialog.PreviousGear AndAlso LvGears.SelectedIndices(0) > 0 Then
                LvGears.Items(LvGears.SelectedIndices(0) - 1).Selected = True
            End If

        Loop Until Not (_gearDialog.NextGear OrElse _gearDialog.PreviousGear)
    End Sub

    'Add Gear
    Private Sub AddGear()
        Dim lvi As ListViewItem

        lvi = CreateListviewItem(LvGears.Items.Count.ToString("00"), 1, "", "", "", "")

        LvGears.Items.Add(lvi)

        lvi.EnsureVisible()
        UpdateGearboxInfoText()
        LvGears.Focus()

        'Change() => NO! Change() is already handled by EditGear
    End Sub

    'Remove Gear
    Private Sub RemoveGear(noChange As Boolean)
        Dim i0 As Integer
        Dim i As Integer
        Dim lv0 As ListViewItem

        If LvGears.Items.Count < 2 Then Exit Sub

        If LvGears.SelectedItems.Count = 0 Then LvGears.Items(LvGears.Items.Count - 1).Selected = True

        i0 = LvGears.SelectedItems(0).Index

        If i0 = 0 Then Exit Sub 'Must not remove axle

        LvGears.SelectedItems(0).Remove()

        i = 0
        For Each lv0 In LvGears.Items
            If lv0.SubItems(GearboxTbl.GearNr).Text = "Axle" Then Continue For
            i += 1
            lv0.SubItems(GearboxTbl.GearNr).Text = i.ToString("00")
        Next

        If i0 < LvGears.Items.Count Then
            LvGears.Items(i0).Selected = True
            LvGears.Items(i0).EnsureVisible()
        End If
        UpdateGearboxInfoText()
        LvGears.Focus()
        Try
            UpdatePic()
        Catch
        End Try

        If Not noChange Then Change()
    End Sub


#End Region


#Region "Open File Context Menu"

    Private _contextMenuFiles As String()
    Private _vehicleCategory As VehicleCategory
    private _vehicleJobType as VectoSimulationJobType

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

#End Region


    Private Sub UpdatePic()

        Dim path As String

        Dim chart As Chart
        Dim s As Series
        Dim a As ChartArea
        Dim img As Bitmap
        Dim gear As Integer
        'Dim fullLoadCurve As EngineFullLoadCurve = Nothing
        'Dim shiftOk As Boolean


        PicBox.Image = Nothing

        Dim shiftPolygon As ShiftPolygon = Nothing
        'Dim engineFld As FullLoadCurve

        If LvGears.Items.Count <= 1 Then Exit Sub

        dim lossmap As TransmissionLossMap = Nothing
        Try
            If LvGears.SelectedItems.Count > 0 AndAlso LvGears.SelectedIndices(0) > 0 Then
                path = FileRepl(LvGears.SelectedItems(0).SubItems(GearboxTbl.ShiftPolygons).Text, GetPath(_gbxFile))
                gear = LvGears.SelectedIndices(0)
            Else
                path = FileRepl(LvGears.Items(1).SubItems(GearboxTbl.ShiftPolygons).Text, GetPath(_gbxFile))
                gear = 1
            End If

            If File.Exists(path) Then shiftPolygon = ShiftPolygonReader.ReadFromFile(path)
            
            if LvGears.SelectedItems.Count > 0 AndAlso LvGears.SelectedIndices(0) > 0 Then
                lossmap = TransmissionLossMapReader.ReadFromFile(FileRepl(LvGears.SelectedItems(0).SubItems(GearboxTbl.LossMapEfficiency).Text, GetPath(_gbxFile)), 
                                                                          LvGears.SelectedItems(0).SubItems(GearboxTbl.Ratio).Text.ToDouble(),"gear plot")
            End If

        Catch ex As Exception

        End Try

        chart = New Chart
        chart.Width = PicBox.Width
        chart.Height = PicBox.Height

        a = New ChartArea

        if Not lossmap Is Nothing Then
            s = new Series()
            s.Points.DataBindXY(lossmap._entries.Select(Function(x) x.InputSpeed.AsRPM).ToArray(),
                                lossmap._entries.Select(function(y) y.InputTorque.Value()).ToArray())
            s.ChartType = SeriesChartType.FastPoint
            s.Color = color.ForestGreen
            s.Name = "LossMap Entries"
            chart.Series.Add(s)
        End If

        'Shiftpolygons from file

        If Not shiftPolygon Is Nothing Then
            s = New Series
            s.Points.DataBindXY(shiftPolygon.Upshift.Select(Function(x) x.AngularSpeed.AsRPM).ToArray(),
                                shiftPolygon.Upshift.Select(Function(x) x.Torque.Value()).ToArray())
            s.ChartType = SeriesChartType.FastLine
            s.BorderWidth = 2
            s.Color = Color.DarkRed
            s.Name = "Upshift curve"
            chart.Series.Add(s)

            s = New Series
            s.Points.DataBindXY(shiftPolygon.Downshift.Select(Function(x) x.AngularSpeed.AsRPM).ToArray(),
                                shiftPolygon.Downshift.Select(Function(x) x.Torque.Value()).ToArray())
            s.ChartType = SeriesChartType.FastLine
            s.BorderWidth = 2
            s.Color = Color.DarkRed
            s.Name = "Downshift curve"
            chart.Series.Add(s)
        End If

        'Dim vectoJob As VectoJob = New VectoJob() With {.FilePath = VectoJobForm.VECTOfile}
        'Dim vectoOk As Boolean = vectoJob.ReadFile()
        dim hasICE = false
        Dim jobFile As String = VectoJobForm.VectoFile
        If Not jobFile Is Nothing AndAlso File.Exists(jobFile) Then

            Dim inputData As IEngineeringInputDataProvider = TryCast(JSONInputDataFactory.ReadJsonJob(jobFile), 
                                                                    IEngineeringInputDataProvider)
            If (inputData Is Nothing) Then
                Exit Sub
            End If
            Dim vehicle As IVehicleEngineeringInputData = inputData.JobInputData.Vehicle
            'inputData = TryCast(JSONInputDataFactory.ReadComponentData(vectoJob.PathEng(False)), IEngineeringInputDataProvider)
			Dim engine As IEngineEngineeringInputData = inputData.JobInputData.Vehicle.Components.EngineInputData
            Dim engineFld As EngineFullLoadCurve = Nothing
            if (not engine is nothing) then
                engineFld = FullLoadCurveReader.Create(engine.EngineModes.First().FullLoadCurve)
                DrawEngineFld(engine, chart)
                DrawShiftLineICE(engine, vehicle, gear, chart)
                hasICE = true
            End If

            dim em = inputData.JobInputData.Vehicle.Components.ElectricMachines?.Entries.FirstOrDefault(function(x) x.Position <> PowertrainPosition.GEN)
            if (engine is nothing andalso not em is nothing) then 
                DrawEMFld(em, chart)
                DrawShiftLineEM(em, vehicle, gear, chart)
            end if
        End If

        a.Name = "main"

        a.AxisX.Title = "engine speed [1/min]"
        a.AxisX.TitleFont = New Font("Helvetica", 10)
        a.AxisX.LabelStyle.Font = New Font("Helvetica", 8)
        a.AxisX.LabelAutoFitStyle = LabelAutoFitStyles.None
        a.AxisX.MajorGrid.LineDashStyle = ChartDashStyle.Dot

        a.AxisY.Title = "engine torque [Nm]"
        a.AxisY.TitleFont = New Font("Helvetica", 10)
        a.AxisY.LabelStyle.Font = New Font("Helvetica", 8)
        a.AxisY.LabelAutoFitStyle = LabelAutoFitStyles.None
        a.AxisY.MajorGrid.LineDashStyle = ChartDashStyle.Dot

        a.AxisX.Minimum = If(hasICE, 300,0)
        a.BorderDashStyle = ChartDashStyle.Solid
        a.BorderWidth = 1

        a.BackColor = Color.GhostWhite

        chart.ChartAreas.Add(a)

        chart.Titles.Add("Gear " & gear & " shift polygons")
        chart.Titles(0).Font = New Font("Helvetica", 12)

        chart.Update()

        img = New Bitmap(chart.Width, chart.Height, PixelFormat.Format32bppArgb)
        chart.DrawToBitmap(img, New Rectangle(0, 0, PicBox.Width, PicBox.Height))

        PicBox.Image = img
    End Sub

    
    Private sub DrawEngineFld(engine As IEngineEngineeringInputData, chart As chart)
        
        Dim s As Series
        Dim engineFld As EngineFullLoadCurve = FullLoadCurveReader.Create(engine.EngineModes.First().FullLoadCurve)


        s = New Series
        s.Points.DataBindXY(engineFld.FullLoadEntries.Select(Function(x) x.EngineSpeed.AsRPM).ToArray(),
                            engineFld.FullLoadEntries.Select(Function(x) x.TorqueFullLoad.Value()).ToArray())
        s.ChartType = SeriesChartType.FastLine
        s.BorderWidth = 2
        s.Color = Color.DarkBlue
        s.Name = "Full load"
        chart.Series.Add(s)
    End sub

    Private Sub DrawShiftLineICE(engine As IEngineEngineeringInputData, vehicle As IVehicleEngineeringInputData, gear As Integer, chart As Chart)
        if (engine Is nothing) then 
            return
        End If
        
        Dim engineFld As EngineFullLoadCurve = FullLoadCurveReader.Create(engine.EngineModes.First().FullLoadCurve)
        If VectoJobForm.Visible AndAlso engine.EngineModes.First().IdleSpeed > 0 Then
            'If FLD0.Init(VectoJobForm.n_idle) Then

            'Dim fullLoadCurve As FullLoadCurve = ConvertToFullLoadCurve(FLD0.LnU, FLD0.LTq)
            Dim gears As IList(Of ITransmissionInputData) = ConvertToGears(LvGears.Items)
            Dim shiftLines As ShiftPolygon = GetShiftLines(engine.EngineModes.First().IdleSpeed, engineFld, vehicle, gears, gear)
            If (Not IsNothing(shiftLines)) Then


                dim s = New Series

                's.Points.DataBindXY(Shiftpoly.gs_nUup, Shiftpoly.gs_TqUp)
                s.Points.DataBindXY(
                    shiftLines.Upshift.Select(Function(pt) pt.AngularSpeed.AsRPM).
                                       ToArray(),
                    shiftLines.Upshift.Select(Function(pt) pt.Torque.Value()).ToArray())
                s.ChartType = SeriesChartType.FastLine
                s.BorderWidth = 2
                s.Color = Color.DarkRed
                s.BorderDashStyle = ChartDashStyle.Dash
                s.Name = "Upshift curve (generic)"
                chart.Series.Add(s)

                s = New Series
                's.Points.DataBindXY(Shiftpoly.gs_nUdown, Shiftpoly.gs_TqDown)
                s.Points.DataBindXY(
                    shiftLines.Downshift.Select(Function(pt) pt.AngularSpeed.AsRPM) _
                                       .ToArray(),
                    shiftLines.Downshift.Select(Function(pt) pt.Torque.Value()).ToArray())
                s.ChartType = SeriesChartType.FastLine
                s.BorderWidth = 2
                s.Color = Color.DarkRed
                s.BorderDashStyle = ChartDashStyle.Dash
                s.Name = "Downshift curve (generic)"
                chart.Series.Add(s)
            End If
            'End If
        End If
    End Sub


    Private sub DrawEmFld(em As ElectricMachineEntry(Of IElectricMotorEngineeringInputData), chart As chart)
        
        Dim s As Series
        Dim emFld = ElectricFullLoadCurveReader.Create(em.ElectricMachine.VoltageLevels.First().FullLoadCurve.First().LoadCurve, em.Count)


        s = New Series
        s.Points.DataBindXY(emFld.FullLoadEntries.Select(Function(x) x.MotorSpeed.AsRPM).ToArray(),
                            emFld.FullLoadEntries.Select(Function(x) x.FullDriveTorque.Value()).ToArray())
        s.ChartType = SeriesChartType.FastLine
        s.BorderWidth = 2
        s.Color = Color.DarkBlue
        s.Name = "Dirve Torque"
        
        chart.Series.Add(s)

        s = New Series
        s.Points.DataBindXY(emFld.FullLoadEntries.Select(Function(x) x.MotorSpeed.AsRPM).ToArray(),
                            emFld.FullLoadEntries.Select(Function(x) x.FullGenerationTorque.Value()).ToArray())
        s.ChartType = SeriesChartType.FastLine
        s.BorderWidth = 2
        s.Color = Color.DarkBlue
        s.Name = "Generation Torque"
        
        chart.Series.Add(s)
    End sub

     Private Sub DrawShiftLineEM(em As ElectricMachineEntry(Of IElectricMotorEngineeringInputData), vehicle As IVehicleEngineeringInputData, gear As Integer, chart As Chart)
        if (em Is nothing) then 
            return
        End If

        Dim emFld = ElectricFullLoadCurveReader.Create(em.ElectricMachine.VoltageLevels.First().FullLoadCurve.First().LoadCurve, em.Count)
        If VectoJobForm.Visible Then
            'If FLD0.Init(VectoJobForm.n_idle) Then

            'Dim fullLoadCurve As FullLoadCurve = ConvertToFullLoadCurve(FLD0.LnU, FLD0.LTq)
            Dim gears As IList(Of ITransmissionInputData) = ConvertToGears(LvGears.Items)
            Dim shiftLines As ShiftPolygon = GetShiftLines(emFld, vehicle, gears, gear)
            If (Not IsNothing(shiftLines)) Then


                dim s = New Series

                's.Points.DataBindXY(Shiftpoly.gs_nUup, Shiftpoly.gs_TqUp)
                if (shiftLines.Upshift.Any()) then
                s.Points.DataBindXY(
                    shiftLines.Upshift.Select(Function(pt) pt.AngularSpeed.AsRPM).
                                       ToArray(),
                    shiftLines.Upshift.Select(Function(pt) pt.Torque.Value()).ToArray())
                end if
                s.ChartType = SeriesChartType.FastLine
                s.BorderWidth = 2
                s.Color = Color.DarkRed
                s.BorderDashStyle = ChartDashStyle.Dash
                s.Name = "Upshift curve (generic)"
                chart.Series.Add(s)

                s = New Series
                's.Points.DataBindXY(Shiftpoly.gs_nUdown, Shiftpoly.gs_TqDown)
                if (shiftLines.Downshift.Any()) then
                s.Points.DataBindXY(
                    shiftLines.Downshift.Select(Function(pt) pt.AngularSpeed.AsRPM) _
                                       .ToArray(),
                    shiftLines.Downshift.Select(Function(pt) pt.Torque.Value()).ToArray())
                end If
                s.ChartType = SeriesChartType.FastLine
                s.BorderWidth = 2
                s.Color = Color.DarkRed
                s.BorderDashStyle = ChartDashStyle.Dash
                s.Name = "Downshift curve (generic)"
                chart.Series.Add(s)
            End If
            'End If
        End If
    End Sub

    Private Function GetShiftLines(emFld As ElectricMotorFullLoadCurve, vehicle As IVehicleEngineeringInputData, gears As IList(Of ITransmissionInputData), gear As Integer) As ShiftPolygon
        
        Dim maxTqStr As String = LvGears.Items(gear).SubItems(GearboxTbl.MaxTorque).Text
        
        If gears.Count <= 1 Then
            Return Nothing
        End If
        Dim rDyn As Meter = vehicle.DynamicTyreRadius
        If rDyn.IsEqual(0) Then
            If (vehicle.Components.AxleWheels.AxlesEngineering.Count < 2) Then
                Return Nothing
            End If
            rdyn = vehicle.Components.AxleWheels.AxlesEngineering.Where(Function(axle)  axle.AxleType = AxleType.VehicleDriven) _
                .Select(Function(da) DeclarationData.Wheels.Lookup(da.Tyre.Dimension).DynamicTyreRadius) _
                .Average()
        End If
        If (rDyn.IsEqual(0)) Then
            Return Nothing
        End If

        Dim kernel As IKernel = new StandardKernel(new VectoNinjectModule)
        dim shiftStrategyFactory as IShiftStrategyFactory = kernel.Get(of IShiftStrategyFactory)()
        Dim tmpStrategyName as String = shiftStrategyFactory.GetShiftStrategyName( CType(CbGStype.SelectedValue, GearboxType), VectoSimulationJobType.BatteryElectricVehicle)
        Dim tmpStrategy As IShiftPolygonCalculator = shiftStrategyFactory.CreateShiftPolygonCalculator(tmpStrategyName, Nothing)

        dim em as ElectricMotorData = ConvertToElectricMotorData(emFld, gear)

        Dim shiftLines As ShiftPolygon = tmpStrategy.ComputeDeclarationShiftPolygon(
            CType(CbGStype.SelectedValue, GearboxType), gear - 1,
            Nothing, gears, nothing,
            Double.Parse(LvGears.Items(0).SubItems(GearboxTbl.Ratio).Text, CultureInfo.InvariantCulture),
            (rDyn), em)
        Return shiftLines
    End Function



    Private Function GetShiftLines(idleSpeed As PerSecond, engineFullLoadCurve As EngineFullLoadCurve, vehicle As IVehicleEngineeringInputData, gears As IList(Of ITransmissionInputData), gear As Integer) _
        As ShiftPolygon
        Dim maxTqStr As String = LvGears.Items(gear).SubItems(GearboxTbl.MaxTorque).Text
        Dim engine As CombustionEngineData = ConvertToEngineData(engineFullLoadCurve, idleSpeed, gear,
                                                                If(String.IsNullOrWhiteSpace(maxTqStr), Nothing, maxTqStr.ToDouble(0).SI(Of NewtonMeter)))
        If gears.Count <= 1 Then
            Return Nothing
        End If
        Dim rDyn As Meter = vehicle.DynamicTyreRadius
        If rDyn.IsEqual(0) Then
			If (vehicle.Components.AxleWheels.AxlesEngineering.Count < 2) Then
                Return Nothing
            End If
			rdyn = vehicle.Components.AxleWheels.AxlesEngineering.Where(Function(axle)  axle.AxleType = AxleType.VehicleDriven) _
				.Select(Function(da) DeclarationData.Wheels.Lookup(da.Tyre.Dimension).DynamicTyreRadius) _
				.Average()
        End If
        If (rDyn.IsEqual(0)) Then
            Return Nothing
        End If

        Dim kernel As IKernel = new StandardKernel(new VectoNinjectModule)
        dim shiftStrategyFactory as IShiftStrategyFactory = kernel.Get(of IShiftStrategyFactory)()
        Dim tmpStrategyName as String = shiftStrategyFactory.GetShiftStrategyName( CType(CbGStype.SelectedValue, GearboxType), _vehicleJobType)
        Dim tmpStrategy As IShiftPolygonCalculator = shiftStrategyFactory.CreateShiftPolygonCalculator(tmpStrategyName, Nothing)

        Dim shiftLines As ShiftPolygon = tmpStrategy.ComputeDeclarationShiftPolygon(
            CType(CbGStype.SelectedValue, GearboxType), gear - 1,
            engine.FullLoadCurves(CType(gear, UInteger)), gears, engine,
            Double.Parse(LvGears.Items(0).SubItems(GearboxTbl.Ratio).Text, CultureInfo.InvariantCulture),
            (rDyn))
        Return shiftLines
    End Function

    Private Function ConvertToGears(gbx As ListView.ListViewItemCollection) As IList(Of ITransmissionInputData)
        Dim retVal As List(Of ITransmissionInputData) = New List(Of ITransmissionInputData)
        Dim value As Double

        For i As Integer = 1 To gbx.Count - 1
            If _
                gbx(i).SubItems(GearboxTbl.Ratio).Text <> "" AndAlso Double.TryParse(gbx(i).SubItems(GearboxTbl.Ratio).Text, value) _
                Then
                Dim maxSpeed As PerSecond =
                        If _
                        (String.IsNullOrWhiteSpace(gbx(i).SubItems(GearboxTbl.MaxSpeed).Text), Nothing,
                        gbx(i).SubItems(GearboxTbl.MaxSpeed).Text.ToDouble().RPMtoRad())
                retVal.Add(
                    New TransmissionInputData() _
                            With {.Ratio = value, .MaxInputSpeed = maxSpeed})

            End If
        Next
        Return retVal
    End Function


#Region "Torque Converter"


    'Browse TC file
    Private Sub BtTCfileBrowse_Click(sender As Object, e As EventArgs) Handles BtTCfileBrowse.Click
        If TorqueConverterFileBrowser.OpenDialog(FileRepl(TbTCfile.Text, GetPath(_gbxFile))) Then
            TbTCfile.Text = GetFilenameWithoutDirectory(TorqueConverterFileBrowser.Files(0), GetPath(_gbxFile))
        End If
    End Sub

    'Open TC file
    Private Sub BtTCfileOpen_Click(sender As Object, e As EventArgs) Handles BtTCfileOpen.Click
        OpenFiles(FileRepl(TbTCfile.Text, GetPath(_gbxFile)))
    End Sub


#End Region

    Private Sub GroupBox1_Enter(sender As Object, e As EventArgs) Handles GroupBox1.Enter
    End Sub

    Public Sub New()

        ' Dieser Aufruf ist für den Designer erforderlich.
        InitializeComponent()

        ' Fügen Sie Initialisierungen nach dem InitializeComponent()-Aufruf hinzu.
    End Sub

    Private Sub BtTCShiftFileBrowse_Click(sender As Object, e As EventArgs) Handles BtTCShiftFileBrowse.Click
        If TorqueConverterShiftPolygonFileBrowser.OpenDialog(FileRepl(TBTCShiftPolygon.Text, GetPath(_gbxFile))) Then
            TBTCShiftPolygon.Text = GetFilenameWithoutDirectory(TorqueConverterShiftPolygonFileBrowser.Files(0),
                                                                GetPath(_gbxFile))
        End If
    End Sub

    Private Sub btnExportXML_Click(sender As Object, e As EventArgs) Handles btnExportXML.Click
        MsgBox("XML Export is lo nonger supported")
        
    End Sub

    Private Sub btnExportAxlGearXML_Click(sender As Object, e As EventArgs) Handles btnExportAxlGearXML.Click
        MsgBox("XML Export is lo nonger supported")

    End Sub

    Private Sub btExportVGBS_Click(sender As Object, e As EventArgs) Handles btExportVGBS.Click
        If LvGears.Items.Count <= 1 Then Exit Sub

        Dim shiftPolygon As ShiftPolygon = Nothing
        Dim path As String
        Dim gear As Integer
        Try
            If LvGears.SelectedItems.Count > 0 AndAlso LvGears.SelectedIndices(0) > 0 Then
                path = FileRepl(LvGears.SelectedItems(0).SubItems(GearboxTbl.ShiftPolygons).Text, GetPath(_gbxFile))
                gear = LvGears.SelectedIndices(0)
            Else
                path = FileRepl(LvGears.Items(1).SubItems(GearboxTbl.ShiftPolygons).Text, GetPath(_gbxFile))
                gear = 1
            End If

            If File.Exists(path) Then shiftPolygon = ShiftPolygonReader.ReadFromFile(path)

            If Not shiftPolygon Is Nothing Then
                ShiftPolygonExport.WriteShiftPolygon(shiftPolygon, path & ".vgbs")
            End If
        Catch ex As Exception
        End Try

        Dim jobFile As String = VectoJobForm.VectoFile
        If Not jobFile Is Nothing AndAlso File.Exists(jobFile) Then
            Dim inputData As IEngineeringInputDataProvider = TryCast(JSONInputDataFactory.ReadJsonJob(jobFile), 
                                                                    IEngineeringInputDataProvider)
            If (inputData Is Nothing) Then
                Exit Sub
            End If

            Dim vehicle As IVehicleEngineeringInputData = inputData.JobInputData.Vehicle
			Dim engine As IEngineEngineeringInputData = vehicle.Components.EngineInputData
			Dim engineFld As EngineFullLoadCurve = FullLoadCurveReader.Create(engine.EngineModes.First().FullLoadCurve)

			If VectoJobForm.Visible AndAlso engine.EngineModes.First().IdleSpeed > 0 Then
                Dim gears As IList(Of ITransmissionInputData) = ConvertToGears(LvGears.Items)
				Dim shiftLines As ShiftPolygon = GetShiftLines(engine.EngineModes.First().IdleSpeed, engineFld, vehicle, gears, gear)
                If (Not IsNothing(shiftLines)) Then
                    ShiftPolygonExport.WriteShiftPolygon(shiftLines, jobFile & "_Gear " & gear & ".vgbs")
                End If
            End If
        End If
    End Sub

    Private Class DummyVehicleContainer
        Inherits VehicleContainer
        Implements IVehicleContainer

        Public Sub New(vectoRunData As VectoRunData) 
            MyBase.New(vectoRunData, Nothing, Nothing, Nothing)
            Throw New NotImplementedException
        End Sub
    End Class
End Class


