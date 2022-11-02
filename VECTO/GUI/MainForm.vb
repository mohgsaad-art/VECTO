'
' This file is part of VECTO.
'
' Copyright © 2012-2016 European Union
'
' Developed by Graz University of Technology,
'              Institute of Internal Combustion Engines and Thermodynamics,
'              Institute of Technical Informatics
'
' VECTO is licensed under the EUPL, Version 1.1 or - as soon they will be approved
' by the European Commission - subsequent versions of the EUPL (the "Licence");
' You may not use VECTO except in compliance with the Licence.
' You may obtain a copy of the Licence at:
'
' https://joinup.ec.europa.eu/community/eupl/og_page/eupl
'
' Unless required by applicable law or agreed to in writing, VECTO
' distributed under the Licence is distributed on an "AS IS" basis,
' WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
' See the Licence for the specific language governing permissions and
' limitations under the Licence.
'
' Authors:
'   Stefan Hausberger, hausberger@ivt.tugraz.at, IVT, Graz University of Technology
'   Christian Kreiner, christian.kreiner@tugraz.at, ITI, Graz University of Technology
'   Michael Krisper, michael.krisper@tugraz.at, ITI, Graz University of Technology
'   Raphael Luz, luz@ivt.tugraz.at, IVT, Graz University of Technology
'   Markus Quaritsch, markus.quaritsch@tugraz.at, IVT, Graz University of Technology
'   Martin Rexeis, rexeis@ivt.tugraz.at, IVT, Graz University of Technology
'

Imports System.Collections.Generic
Imports System.ComponentModel
Imports System.IO
Imports System.Linq
Imports TUGraz.VectoCore.Models.Simulation.Impl
Imports TUGraz.VectoCore.InputData.FileIO.JSON
Imports System.Text
Imports System.Threading
Imports System.Xml
Imports System.Xml.Linq
Imports Microsoft.VisualBasic.FileIO
Imports Ninject
Imports TUGraz.VectoCommon.Exceptions
Imports TUGraz.VectoCommon.InputData
Imports TUGraz.VectoCommon.Models
Imports TUGraz.VectoCommon.Resources
Imports TUGraz.VectoCommon.Utils
Imports TUGraz.VectoCore
Imports TUGraz.VectoCore.InputData.FileIO.XML
Imports TUGraz.VectoCore.Models.Simulation
Imports TUGraz.VectoCore.Models.Simulation.Impl.SimulatorFactory
Imports TUGraz.VectoCore.Models.SimulationComponent.Data
Imports TUGraz.VectoCore.OutputData
Imports TUGraz.VectoCore.OutputData.FileIO
Imports TUGraz.VectoCore.Utils

''' <summary>
''' Main application form. Loads at application start. Closing form ends application.
''' </summary>
''' <remarks></remarks>

Public Class MainForm
    Private _jobListView As FileListView
    Private _cycleListView As FileListView

    Private _lastModeName As String
    Private _conMenTarget As ListView
    Private _conMenTarJob As Boolean

    Private _guIlocked As Boolean

    Private _checkLock As Boolean
    Private _genChecked As Integer
    Private _genCheckAllLock As Boolean

    Private _cbDeclLock As Boolean = False

#Region "SLEEP Control - Prevent sleep while VECTO is running"

    Private Declare Function SetThreadExecutionState Lib "kernel32" (esFlags As Long) As Long

    Private Shared Sub AllowSleepOff()
#If Not PLATFORM = "x86" Then
        SetThreadExecutionState(EXECUTION_STATE.ES_CONTINUOUS Or EXECUTION_STATE.ES_SYSTEM_REQUIRED)
#End If
    End Sub

    Private Shared Sub AllowSleepOn()
#If Not PLATFORM = "x86" Then
        SetThreadExecutionState(EXECUTION_STATE.ES_CONTINUOUS)
#End If
    End Sub

    Private Enum EXECUTION_STATE As Integer
        ''' Informs the system that the state being set should remain in effect until the next call that uses ES_CONTINUOUS and one of the other state flags is cleared.
        ES_CONTINUOUS = &H80000000
        ''' Forces the display to be on by resetting the display idle timer.
        ES_DISPLAY_REQUIRED = &H2
        ''' Forces the system to be in the working state by resetting the system idle timer.
        ES_SYSTEM_REQUIRED = &H1
    End Enum

#End Region

#Region "FileBrowser Init/Close"

    Private Sub FB_Initialize()
        FileBrowserFolderHistoryIninialized = False
        Try
            COREvers = VectoSimulationCore.VersionNumber()
        Catch ex As Exception
            LogFile.WriteToLog(MessageType.Err, ex.StackTrace)
        End Try


        FolderFileBrowser = New FileBrowser("WorkDir", True)
        TextFileBrowser = New FileBrowser("FileLists")
        JobfileFileBrowser = New FileBrowser("vecto")
        VehicleFileBrowser = New FileBrowser("vveh")
        VehicleXMLFileBrowser = New FileBrowser("vveh_xml")
        ManRXMLFileBrowser = New FileBrowser("xml")
        FuelConsumptionMapFileBrowser = New FileBrowser("vmap")
        DrivingCycleFileBrowser = New FileBrowser("vdri")
        FullLoadCurveFileBrowser = New FileBrowser("vfld")
        EngineFileBrowser = New FileBrowser("veng")
        GearboxFileBrowser = New FileBrowser("vgbx")
        TCUFileBrowser = New FileBrowser("vtcu")
        DriverAccelerationFileBrowser = New FileBrowser("vacc")
        AuxFileBrowser = New FileBrowser("vaux")
        GearboxShiftPolygonFileBrowser = New FileBrowser("vgbs")
        RetarderLossMapFileBrowser = New FileBrowser("vrlm")
        TransmissionLossMapFileBrowser = New FileBrowser("vtlm")
        PtoLossMapFileBrowser = New FileBrowser("vptol")
        PTODrivingCycleStandstillFileBrowser = New FileBrowser("vptoc")
        PTODrivingCycleDrivingFileBrowser = New FileBrowser("vptor")
        TorqueConverterFileBrowser = New FileBrowser("vtcc")
        TorqueConverterShiftPolygonFileBrowser = New FileBrowser("vgbs")
        CrossWindCorrectionFileBrowser = New FileBrowser("vcdx")
        ElectricMotorFileBrowser = New FileBrowser("vem")
        IEPCFileBrowser = New FileBrowser("viepc")
        IEPCFLCFileBrowser = New FileBrowser("viepcp")
        IEPCDragFileBrowser = new FileBrowser("viepcd")
        IEPCPowerMapFileBrowser = New FileBrowser("viepco")
        REESSFileBrowser = New FileBrowser("vreess")
        EmADCLossMapFileBrowser = New FileBrowser("vtlm")
        DriverDecisionFactorVelocityDropFileBrowser = New FileBrowser("DfVelocityDrop")
        DriverDecisionFactorTargetSpeedFileBrowser = New FileBrowser("DfTargetSpeed")
        DriverDecisionFactorVelocityDropFileBrowser.Extensions = New String() {"csv"}
        DriverDecisionFactorTargetSpeedFileBrowser.Extensions = New String() {"csv"}

        ElectricMachineDragTorqueFileBrowser = New FileBrowser("vemd")
        ElectricMachineMaxTorqueFileBrowser = New FileBrowser("vemp")
        ElectricMachineEfficiencyMapFileBrowser = New FileBrowser("vemo")
        HCUFileBrowser = New FileBrowser("vhctl")
        BusAuxFileBrowser = New FileBrowser(".vaux")
        BusAuxCompressorMapFileBrowser = New FileBrowser(".acmp")
        BatteryMaxCurrentCurveFileBrowser = New FileBrowser("vimax")
        BatteryInternalResistanceCurveFileBrowser = New FileBrowser("vbatr")
        BatterySoCCurveFileBrowser = New FileBrowser("vbatv")
        PropulsionTorqueLimitFileBrowser = New FileBrowser("vtqp")
        ModalResultsFileBrowser = New FileBrowser("vmod")

        IHPCFileBrowser = new FileBrowser("vem")
        IHPCPowerMapFileBrowser = new FileBrowser("vemo")
        IHPCFullLoadCurveFileBrowser = new FileBrowser("vemp")
        IHPCDragCurveFileBrowser = new FileBrowser("vemd")

        '-------------------------------------------------------
        TextFileBrowser.Extensions = New String() {"txt"}
        JobfileFileBrowser.Extensions = New String() {"vecto"}
        VehicleFileBrowser.Extensions = New String() {"vveh"}
        VehicleXMLFileBrowser.Extensions = New String() {"xml"}
        ManRXMLFileBrowser.Extensions = New String() {"xml"}
        FuelConsumptionMapFileBrowser.Extensions = New String() {"vmap"}
        DrivingCycleFileBrowser.Extensions = New String() {"vdri"}
        FullLoadCurveFileBrowser.Extensions = New String() {"vfld"}
        EngineFileBrowser.Extensions = New String() {"veng"}
        GearboxFileBrowser.Extensions = New String() {"vgbx"}
        TCUFileBrowser.Extensions = New String() {"vtcu", "vgbx"}
        DriverAccelerationFileBrowser.Extensions = New String() {"vacc"}
        AuxFileBrowser.Extensions = New String() {"vaux"}
        GearboxShiftPolygonFileBrowser.Extensions = New String() {"vgbs"}
        RetarderLossMapFileBrowser.Extensions = New String() {"vrlm"}
        TransmissionLossMapFileBrowser.Extensions = New String() {"vtlm"}
        PtoLossMapFileBrowser.Extensions = New String() {"vptol"}
        PTODrivingCycleStandstillFileBrowser.Extensions = New String() {"vptoc"}
        PTODrivingCycleDrivingFileBrowser.Extensions = New String() {"vptor"}
        TorqueConverterFileBrowser.Extensions = New String() {"vtcc"}
        TorqueConverterShiftPolygonFileBrowser.Extensions = New String() {"vgbs"}
        CrossWindCorrectionFileBrowser.Extensions = New String() {"vcdv", "vcdb"}
        ElectricMotorFileBrowser.Extensions = New String() {"vem"}
        REESSFileBrowser.Extensions = New String() {"vreess", "vbat"}
        EmADCLossMapFileBrowser.Extensions = New String() {"vtlm"}

        ElectricMachineDragTorqueFileBrowser.Extensions = New String() {"vemd"}
        ElectricMachineMaxTorqueFileBrowser.Extensions = New String() {"vemp"}
        ElectricMachineEfficiencyMapFileBrowser.Extensions = New String() {"vemo"}

        BatteryMaxCurrentCurveFileBrowser.Extensions = New String() {"vimax"}
        BatteryInternalResistanceCurveFileBrowser.Extensions = New String() {"vbatr"}
        BatterySoCCurveFileBrowser.Extensions = New String() {"vbatv"}
        HCUFileBrowser.Extensions = New String() {"vhctl"}
        BusAuxFileBrowser.Extensions = New String() {"vaux"}
        BusAuxCompressorMapFileBrowser.Extensions = New String() {"acmp"}
        PropulsionTorqueLimitFileBrowser.Extensions = New String() {"vtqp"}

        ModalResultsFileBrowser.Extensions = New String() {"vmod"}

        IHPCFileBrowser.Extensions = New String(){"vem"}
        IHPCPowerMapFileBrowser.Extensions = New String(){"vemo"}
        IHPCFullLoadCurveFileBrowser.Extensions = New String(){"vemp"}
        IHPCDragCurveFileBrowser.Extensions = New String(){"vemd"}
        
        IEPCFileBrowser.Extensions = New String () {"viepc"}
        IEPCFLCFileBrowser.Extensions = New String() {"viepcp"}
        IEPCDragFileBrowser.Extensions = New String() {"viepcd"}
        IEPCPowerMapFileBrowser.Extensions = New String() {"viepco"}
    End Sub

    Private Sub FB_Close()
        FolderFileBrowser.Close()
        TextFileBrowser.Close()
        JobfileFileBrowser.Close()
        VehicleFileBrowser.Close()
        VehicleXMLFileBrowser.Close()
        ManRXMLFileBrowser.Close()
        FuelConsumptionMapFileBrowser.Close()
        DrivingCycleFileBrowser.Close()
        FullLoadCurveFileBrowser.Close()
        EngineFileBrowser.Close()
        GearboxFileBrowser.Close()
        DriverAccelerationFileBrowser.Close()
        AuxFileBrowser.Close()
        GearboxShiftPolygonFileBrowser.Close()
        RetarderLossMapFileBrowser.Close()
        TransmissionLossMapFileBrowser.Close()
        PtoLossMapFileBrowser.Close()
        PTODrivingCycleStandstillFileBrowser.Close()
        PTODrivingCycleDrivingFileBrowser.Close()
        TorqueConverterFileBrowser.Close()
        TorqueConverterShiftPolygonFileBrowser.Close()
        CrossWindCorrectionFileBrowser.Close()
        ModalResultsFileBrowser.Close()
    End Sub

#End Region

    'Lock certain GUI elements while VECTO is running
    Private Sub LockGUI(lock As Boolean)
        _guIlocked = lock

        PanelOptAllg.Enabled = Not lock

        BtGENup.Enabled = Not lock
        BtGENdown.Enabled = Not lock
        ButtonGENadd.Enabled = Not lock
        ButtonGENremove.Enabled = Not lock
        LvGEN.LabelEdit = Not lock
        ChBoxAllGEN.Enabled = Not lock

        btStartV3.Enabled = Not lock
    End Sub


#Region "Form Init/Close"

    'Initialise
    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim x As Integer

        _guIlocked = False
        _checkLock = False
        _genCheckAllLock = False
        _genChecked = 0


        'Load Tabs properly (otherwise problem with ListViews)
        For x = 0 To TabControl1.TabCount - 1
            TabControl1.TabPages(x).Show()
        Next

        _lastModeName = ""

        FB_Initialize()

        Text = "VECTO" & VECTOvers & " / VectoCore" & VectoSimulationCore.BranchSuffix & " " & COREvers


        'FileLists
        _jobListView = New FileListView(Path.Combine(MyConfPath, CONFIG_JOBLIST_FILE))
        _jobListView.LVbox = LvGEN
        _cycleListView = New FileListView(Path.Combine(MyConfPath, CONFIG_CYCLELIST_FILE))

        _jobListView.LoadList()

        LoadOptions()

        'Resize columns ... after Loading the @file-lists
        LvGEN.Columns(1).Width = -2
        LvMsg.Columns(2).Width = -2

        'Initialize BackgroundWorker

        VectoWorkerV3 = New BackgroundWorker()
        AddHandler VectoWorkerV3.DoWork, AddressOf VectoWorkerV3_OnDoWork
        AddHandler VectoWorkerV3.ProgressChanged, AddressOf VectoWorkerV3_OnProgressChanged
        AddHandler VectoWorkerV3.RunWorkerCompleted, AddressOf VectoWorkerV3_OnRunWorkerCompleted

        VectoWorkerV3.WorkerReportsProgress = True
        VectoWorkerV3.WorkerSupportsCancellation = True

        'Set mode (Batch/Standard)
        ModeUpdate()

        DeclOnOff()
    End Sub

    ' ReSharper disable once UnusedMember.Global -- used via Logging Framework! 
    Public Shared Sub LogMethod(level As String, message As String)

        If VectoWorkerV3.IsBusy AndAlso Not VectoWorkerV3.CancellationPending Then
            If level = "Warn" Then
                VectoWorkerV3.ReportProgress(100,
                                             New VectoProgress With {.Target = "ListBoxWarning", .Message = message})
            ElseIf level = "Error" Or level = "Fatal" Then
                VectoWorkerV3.ReportProgress(100, New VectoProgress With {.Target = "ListBoxError", .Message = message})
            End If
        End If
    End Sub

    'Declaration mode GUI settings
    Private Sub DeclOnOff()

        If Cfg.DeclMode Then
            Text = "VECTO" & VectoSimulationCore.BranchSuffix & " " & COREvers & " - Declaration Mode"
            Cfg.DeclInit()
        Else
            Text = "VECTO" & VectoSimulationCore.BranchSuffix & " " & COREvers
        End If

        If Cfg.DeclMode Then
            _lastModeName = "Declaration"
        Else
            _lastModeName = "Engineering"
        End If

        Status(_lastModeName & " Mode")

        LoadOptions()

        LbDecl.Visible = Cfg.DeclMode
    End Sub

    'Shown Event (Form-Load finished) ... here StartUp Forms are loaded (DEV, GEN/ADV- Editor ..)
    Private Sub F01_MAINForm_Shown(sender As Object, e As EventArgs) Handles Me.Shown
        Dim fwelcome As WelcomeDialog

        If Cfg.FirstRun Then
            Cfg.FirstRun = False
            fwelcome = New WelcomeDialog
            fwelcome.ShowDialog()
        End If
    End Sub

    'Open file

    'Close
    Private Sub F01_MAINForm_FormClosing(sender As Object, e As FormClosingEventArgs) Handles Me.FormClosing

        'Save File-Lists
        SaveFileLists()

        'Close log
        LogFile.CloseLog()

        'Config save
        SetOptions()
        Cfg.Save()

        'File browser instances close
        FB_Close()
    End Sub

#End Region

    'Open file - Job, vehicle, engine, gearbox or signature file
    Public Sub OpenVectoFile(file As String)

        If Not IO.File.Exists(file) Then

            GUIMsg(MessageType.Err, "File not found! (" & file & ")")
            MsgBox("File not found! (" & file & ")", MsgBoxStyle.Critical)

        Else

            Select Case UCase(GetExtension(file))
                Case ".VGBX"
                    If Not GearboxForm.Visible Then
                        GearboxForm.Show()
                    Else
                        GearboxForm.JobDir = ""
                        If GearboxForm.WindowState = FormWindowState.Minimized Then _
                            GearboxForm.WindowState = FormWindowState.Normal
                        GearboxForm.BringToFront()
                    End If
                    Try
                        GearboxForm.OpenGbx(file, VehicleCategory.RigidTruck, VectoSimulationJobType.ConventionalVehicle)
                    Catch ex As Exception
                        MsgBox("Failed to open Gearbox File: " + ex.Message)
                    End Try
                Case ".VVEH"
                    If Not VehicleForm.Visible Then
                        VehicleForm.Show()
                    Else
                        VehicleForm.JobDir = ""
                        If VehicleForm.WindowState = FormWindowState.Minimized Then _
                            VehicleForm.WindowState = FormWindowState.Normal
                        VehicleForm.BringToFront()
                    End If
                    Try
                        VehicleForm.OpenVehicle(file)
                    Catch ex As Exception
                        MsgBox(ex.Message, MsgBoxStyle.OkOnly, "Error loading Vehicle File")
                    End Try
                Case ".VENG"
                    If Not EngineForm.Visible Then
                        EngineForm.Show()
                    Else
                        EngineForm.JobDir = ""
                        If EngineForm.WindowState = FormWindowState.Minimized Then _
                            EngineForm.WindowState = FormWindowState.Normal
                        EngineForm.BringToFront()
                    End If
                    Try
                        EngineForm.OpenEngineFile(file)
                    Catch ex As Exception
                        MsgBox(ex.Message, MsgBoxStyle.OkOnly, "Error loading Engine File")
                    End Try
                Case ".VECTO"
                    OpenVECTOeditor(file, VectoSimulationJobType.ConventionalVehicle)
                Case Else
                    MsgBox("Type '" & GetExtension(file) & "' unknown!", MsgBoxStyle.Critical)
            End Select

        End If
    End Sub


#Region "Events"

    Private Sub ButtonGENremove_Click(sender As Object, e As EventArgs) _
        Handles ButtonGENremove.Click
        RemoveJobFile()
    End Sub

    Private Sub ButtonGENadd_Click(sender As Object, e As EventArgs) _
        Handles ButtonGENadd.Click
        AddJobFile()
    End Sub

    Private Sub ListViewGEN_KeyDown(sender As Object, e As KeyEventArgs) _
        Handles LvGEN.KeyDown
        Select Case e.KeyCode
            Case Keys.Delete, Keys.Back
                If Not _guIlocked Then RemoveJobFile()
            Case Keys.Enter
                OpenJobFile()
        End Select
    End Sub

    Private Sub ListViewGEN_DoubleClick(sender As Object, e As EventArgs) Handles LvGEN.DoubleClick
        If LvGEN.SelectedItems.Count > 0 Then
            LvGEN.SelectedItems(0).Checked = Not LvGEN.SelectedItems(0).Checked
            OpenJobFile()
        End If
    End Sub

    Private Sub LvGEN_ItemChecked(sender As Object, e As ItemCheckedEventArgs) _
        Handles LvGEN.ItemChecked

        If e.Item.Checked Then
            _genChecked += 1
        Else
            _genChecked -= 1
        End If

        If _checkLock Then Exit Sub
        UpdateJobTabText()
    End Sub

    Private Sub ChBoxAllGEN_CheckedChanged(sender As Object, e As EventArgs) _
        Handles ChBoxAllGEN.CheckedChanged

        If _genCheckAllLock And ChBoxAllGEN.CheckState = CheckState.Indeterminate Then Exit Sub

        CheckAllGen(ChBoxAllGEN.Checked)
    End Sub

    Private Sub CheckAllGen(check As Boolean)
        Dim x As ListViewItem

        _checkLock = True
        LvGEN.BeginUpdate()

        For Each x In LvGEN.Items
            x.Checked = check
        Next

        LvGEN.EndUpdate()
        _checkLock = False

        _genChecked = LvGEN.CheckedItems.Count
        UpdateJobTabText()
    End Sub

    Private Sub ListGEN_DragEnter(sender As Object, e As DragEventArgs) _
        Handles LvGEN.DragEnter
        If (e.Data.GetDataPresent(DataFormats.FileDrop)) Then
            e.Effect = DragDropEffects.Copy
        End If
    End Sub

    Private Sub ListGEN_DragDrop(sender As Object, e As DragEventArgs) _
        Handles LvGEN.DragDrop
        Dim f As String()
        f = CType(e.Data.GetData(DataFormats.FileDrop), String())
        AddToJobListView(f)
    End Sub

    Private Sub BtGENup_Click(sender As Object, e As EventArgs) Handles BtGENup.Click
        MoveItem(LvGEN, True)
    End Sub

    Private Sub BtGENdown_Click(sender As Object, e As EventArgs) Handles BtGENdown.Click
        MoveItem(LvGEN, False)
    End Sub

#End Region

    'Remove selected file(s) from job list
    Private Sub RemoveJobFile()
        Dim lastindx As Integer
        Dim selIx() As Integer
        Dim i As Integer

        If LvGEN.SelectedItems.Count < 1 Then
            If LvGEN.Items.Count = 1 Then
                LvGEN.Items(0).Selected = True
            Else
                Exit Sub
            End If
        End If

        LvGEN.BeginUpdate()
        _checkLock = True

        ReDim selIx(LvGEN.SelectedItems.Count - 1)
        LvGEN.SelectedIndices.CopyTo(selIx, 0)

        lastindx = LvGEN.SelectedIndices(LvGEN.SelectedItems.Count - 1)

        For i = UBound(selIx) To 0 Step -1
            LvGEN.Items.RemoveAt(selIx(i))
        Next

        If lastindx < LvGEN.Items.Count Then
            LvGEN.Items(lastindx).Selected = True
        Else
            If LvGEN.Items.Count > 0 Then LvGEN.Items(LvGEN.Items.Count - 1).Selected = True
        End If

        LvGEN.EndUpdate()
        _checkLock = False

        _genChecked = LvGEN.CheckedItems.Count
        UpdateJobTabText()
    End Sub

    'Browse for job file(s) and add to job list with AddToJobListView
    Private Sub AddJobFile()
        Dim x As String()
        Dim chck As Boolean = False

        x = New String() {""}

        Dim extensions As String = "vecto"
        Dim inputDataExtensions As String() = New String() {"xml"}
        If (inputDataExtensions.Any()) Then _
            extensions = String.Join(",", extensions, String.Join(",", inputDataExtensions))

        'STANDARD/BATCH
        If JobfileFileBrowser.OpenDialog("", True, extensions) Then
            chck = True
            x = JobfileFileBrowser.Files
        End If

        If chck Then AddToJobListView(x)
    End Sub

    'Open file in list
    Private Sub OpenJobFile()
        Dim f As String

        If LvGEN.SelectedItems.Count < 1 Then
            If LvGEN.Items.Count = 1 Then
                LvGEN.Items(0).Selected = True
            Else
                Exit Sub
            End If
        End If

        f = LvGEN.SelectedItems(0).SubItems(0).Text
        f = FileRepl(f)
        If Path.GetExtension(f) <> VectoCore.Configuration.Constants.FileExtensions.VectoJobFile Then
            MsgBox("Job File " + f + " can not be opened in Job Editor. Try importing the file.")
            Exit Sub
        End If
        If Not File.Exists(f) Then
            MsgBox(f & " not found!")
        Else
            OpenVECTOeditor(f, VectoSimulationJobType.ConventionalVehicle)
        End If
    End Sub

    'Add File to job listview (multiple files)
    Private Sub AddToJobListView(path As String(), Optional ByVal txt As String = " ")
        Dim pDim As Integer
        Dim p As Integer
        Dim f As Integer
        Dim fList As String()
        Dim fListDim As Integer = -1
        Dim listViewItem As ListViewItem

        'If VECTO runs: Cancel operation (because Mode-change during calculation is not very clever)
        If VectoWorkerV3.IsBusy Then Exit Sub

        pDim = UBound(path)
        ReDim fList(0)     'um Nullverweisausnahme-Warnung zu verhindern

        '******************************************* Begin Update '*******************************************
        LvGEN.BeginUpdate()
        _checkLock = True

        LvGEN.SelectedIndices.Clear()

        If pDim = 0 Then
            fListDim = LvGEN.Items.Count - 1
            ReDim fList(fListDim)
            For f = 0 To fListDim
                fList(f) = FileRepl(LvGEN.Items(f).SubItems(0).Text)
            Next
        End If

        For p = 0 To pDim

            If pDim = 0 Then

                For f = 0 To fListDim

                    'If file already exists in the list: Do not append (only when a single file)
                    If UCase(path(p)) = UCase(fList(f)) Then

                        'Status reset
                        LvGEN.Items(f).SubItems(1).Text = txt
                        LvGEN.Items(f).BackColor = Color.FromKnownColor(KnownColor.Window)
                        LvGEN.Items(f).ForeColor = Color.FromKnownColor(KnownColor.WindowText)

                        'Element auswählen und anhaken |@@| Element selection and hook
                        LvGEN.Items(f).Selected = True
                        LvGEN.Items(f).Checked = True
                        LvGEN.Items(f).EnsureVisible()

                        GoTo lbFound
                    End If
                Next

            End If

            'Otherwise: Add File (without WorkDir)
            listViewItem = New ListViewItem(path(p))    'fFileWD(Path(p)))
            listViewItem.SubItems.Add(" ")
            listViewItem.Checked = True
            listViewItem.Selected = True
            LvGEN.Items.Add(listViewItem)
            listViewItem.EnsureVisible()
lbFound:
        Next

        LvGEN.EndUpdate()
        _checkLock = False
        '******************************************* End Update '*******************************************

        'Number update
        _genChecked = LvGEN.CheckedItems.Count
        UpdateJobTabText()
    End Sub

    'Add File to job listview (single file)
    Public Sub AddToJobListView(path As String, Optional ByVal txt As String = " ")
        Dim p(0) As String
        p(0) = path
        AddToJobListView(p, txt)
    End Sub

    'Update job files counter in tab titel
    Private Sub UpdateJobTabText()
        Dim count As Integer = LvGEN.Items.Count

        TabPageGEN.Text = $"Job Files ( {_genChecked} / {count} )"

        _genCheckAllLock = True

        If _genChecked = 0 Then
            ChBoxAllGEN.CheckState = CheckState.Unchecked
        ElseIf _genChecked = count Then
            ChBoxAllGEN.CheckState = CheckState.Checked
        Else
            ChBoxAllGEN.CheckState = CheckState.Indeterminate
        End If

        _genCheckAllLock = False
    End Sub


#Region "Toolstrip"

    'New Job file
    Private Sub ToolStripBtNew_Click(sender As Object, e As EventArgs) Handles ToolStripBtNew.Click
        OpenVECTOeditor("<New>", VectoSimulationJobType.ConventionalVehicle)
    End Sub

    'Open input file
    Private Sub ToolStripBtOpen_Click(sender As Object, e As EventArgs) Handles ToolStripBtOpen.Click

        If JobfileFileBrowser.OpenDialog("", False, "vecto,vveh,vgbx,veng") Then
            OpenVectoFile(JobfileFileBrowser.Files(0))
        End If
    End Sub

    Private Sub GENEditorToolStripMenuItem1_Click(sender As Object, e As EventArgs) _
        Handles GENEditorToolStripMenuItem1.Click
        OpenVECTOeditor("<New>", VectoSimulationJobType.ConventionalVehicle)
    End Sub

    Private Sub VEHEditorToolStripMenuItem_Click(sender As Object, e As EventArgs) _
        Handles VEHEditorToolStripMenuItem.Click
        If Not VehicleForm.Visible Then
            VehicleForm.Show()
        Else
            If VehicleForm.WindowState = FormWindowState.Minimized Then VehicleForm.WindowState = FormWindowState.Normal
            VehicleForm.BringToFront()
        End If
    End Sub

    Private Sub EngineEditorToolStripMenuItem_Click(sender As Object, e As EventArgs) _
        Handles EngineEditorToolStripMenuItem.Click
        If Not EngineForm.Visible Then
            EngineForm.Show()
        Else
            If EngineForm.WindowState = FormWindowState.Minimized Then EngineForm.WindowState = FormWindowState.Normal
            EngineForm.BringToFront()
        End If
    End Sub

    Private Sub GearboxEditorToolStripMenuItem_Click(sender As Object, e As EventArgs) _
        Handles GearboxEditorToolStripMenuItem.Click
        If Not GearboxForm.Visible Then
            GearboxForm.Show()
        Else
            If GearboxForm.WindowState = FormWindowState.Minimized Then GearboxForm.WindowState = FormWindowState.Normal
            GearboxForm.BringToFront()
        End If
    End Sub

    Private Sub GraphToolStripMenuItem_Click(sender As Object, e As EventArgs) _
        Handles GraphToolStripMenuItem.Click
        Dim graphForm As New GraphForm
        graphForm.Show()
    End Sub

    Private Sub OpenLogToolStripMenuItem_Click(sender As Object, e As EventArgs) _
        Handles OpenLogToolStripMenuItem.Click
        Process.Start(new ProcessStartInfo(Path.Combine(MyAppPath, "log.txt")) with {.UseShellExecute = true})
    End Sub

    Private Sub SettingsToolStripMenuItem_Click(sender As Object, e As EventArgs) _
        Handles SettingsToolStripMenuItem.Click
        Settings.ShowDialog()
    End Sub

    Private Sub UserManualToolStripMenuItem_Click(sender As Object, e As EventArgs) _
        Handles UserManualToolStripMenuItem.Click
        OpenFileExternal("User Manual\help.html")
    End Sub

    Private Sub UpdateNotesToolStripMenuItem_Click(sender As Object, e As EventArgs) _
        Handles UpdateNotesToolStripMenuItem.Click
        OpenFileExternal("User Manual\Release Notes.pdf")
    End Sub

    Private Sub OpenFileExternal(filename As String)
        Dim filepath = Path.Combine(MyAppPath, filename)
        If File.Exists(filepath) Then
            Process.Start(new ProcessStartInfo(filepath) With {.UseShellExecute = true})
        Else
            MsgBox("File not found!", MsgBoxStyle.Critical)
        End If
    End Sub

    Private Sub ReportBugViaCITnetToolStripMenuItem_Click(sender As Object, e As EventArgs) _
        Handles ReportBugViaCITnetToolStripMenuItem.Click
        JiraDialog.ShowDialog()
    End Sub

    Private Sub AboutVECTOToolStripMenuItem1_Click(sender As Object, e As EventArgs) _
        Handles AboutVECTOToolStripMenuItem1.Click
        AboutBox.ShowDialog()
    End Sub


#End Region

    'Move job/cycle file up or down in list view
    Private Sub MoveItem(ByRef listView As ListView, moveUp As Boolean)
        Dim x As Integer
        Dim y As Integer
        Dim y1 As Integer
        Dim items() As String
        Dim check() As Boolean
        Dim index() As Integer
        Dim listViewItem As ListViewItem

        If _guIlocked Then Exit Sub

        'Cache Selected Items
        y1 = listView.SelectedItems.Count - 1
        ReDim items(y1)
        ReDim check(y1)
        ReDim index(y1)
        y = 0
        For Each x In listView.SelectedIndices
            items(y) = listView.Items(x).SubItems(0).Text
            check(y) = listView.Items(x).Checked
            If moveUp Then
                If x = 0 Then Exit Sub
                index(y) = x - 1
            Else
                If x = listView.Items.Count - 1 Then Exit Sub
                index(y) = x + 1
            End If
            y += 1
        Next

        listView.BeginUpdate()

        'Delete Selected Items
        For Each listViewItem In listView.SelectedItems
            listViewItem.Remove()
        Next

        'Items select and Insert
        For y = 0 To y1
            If Not check(y) Then _genChecked += 1
            listViewItem = listView.Items.Insert(index(y), items(y))
            listViewItem.SubItems.Add(" ")
            listViewItem.Checked = check(y)
            listView.SelectedIndices.Add(index(y))
        Next

        listView.EndUpdate()
    End Sub


#Region "job/cycle file List - Context Menu"

    'Save List
    Private Sub SaveListToolStripMenuItem_Click(sender As Object, e As EventArgs) _
        Handles SaveListToolStripMenuItem.Click
        If TextFileBrowser.SaveDialog("") Then
            If _conMenTarJob Then
                _jobListView.SaveList(TextFileBrowser.Files(0))
            Else
                _cycleListView.SaveList(TextFileBrowser.Files(0))
            End If
        End If
    End Sub

    'Load List
    Private Sub LoadListToolStripMenuItem_Click(sender As Object, e As EventArgs) _
        Handles LoadListToolStripMenuItem.Click

        If _guIlocked Then Exit Sub

        If TextFileBrowser.OpenDialog("") Then

            If _conMenTarJob Then 'GEN
                _jobListView.LoadList(TextFileBrowser.Files(0))
                _genChecked = LvGEN.CheckedItems.Count
                UpdateJobTabText()
            Else 'DRI
                'Mode toggle 
                _cycleListView.LoadList(TextFileBrowser.Files(0))
            End If

        End If
    End Sub

    'Load Default List
    Private Sub LoadDefaultListToolStripMenuItem_Click(sender As Object, e As EventArgs) _
        Handles LoadDefaultListToolStripMenuItem.Click

        If _guIlocked Then Exit Sub

        If _conMenTarJob Then

            _jobListView.LoadList()

            _genChecked = LvGEN.CheckedItems.Count
            UpdateJobTabText()
        Else
            _cycleListView.LoadList()

        End If
    End Sub

    'Clear List
    Private Sub ClearListToolStripMenuItem_Click(sender As Object, e As EventArgs) _
        Handles ClearListToolStripMenuItem.Click

        If _guIlocked Then Exit Sub

        _conMenTarget.Items.Clear()
        If _conMenTarJob Then
            _genChecked = LvGEN.CheckedItems.Count
            UpdateJobTabText()
        End If
    End Sub

#End Region

    'VECTO Start button - Calls VECTO_Launcher or aborts calculation

    Private Sub btStartV3_Click(sender As Object, e As EventArgs) Handles btStartV3.Click
        If Not VectoWorkerV3.IsBusy Then
            'Save Lists for Crash
            SaveFileLists()

            LvGEN.SelectedItems.Clear()

            If LvGEN.CheckedItems.Count = 0 Then
                GUIMsg(MessageType.Err, "No job file selected!")
                Exit Sub
            End If

            Status("Launching VECTO ...")
            JobFileList.Clear()
            JobFileList.AddRange(
                From listViewItem As ListViewItem In LvGEN.CheckedItems.Cast(Of ListViewItem)()
                Select fFileRepl = FileRepl(listViewItem.SubItems(0).Text))

            SetOptions()
            Cfg.Save()
            ClearMsg()

            LockGUI(True)
            btStartV3.Enabled = True
            btStartV3.Text = "STOP"
            btStartV3.Image = My.Resources.Stop_icon

            ToolStripProgBarOverall.Value = 0
            ToolStripProgBarOverall.Style = ProgressBarStyle.Continuous
            ToolStripProgBarOverall.Visible = True

            VectoWorkerV3.RunWorkerAsync()
        Else
            btStartV3.Enabled = False
            btStartV3.Text = "Aborting..."
            btStartV3.Image = My.Resources.Play_icon_gray
            VectoWorkerV3.CancelAsync()
        End If
    End Sub


    Private Sub VectoWorkerV3_OnDoWork(theSender As Object, e As DoWorkEventArgs)
        Dim sender As BackgroundWorker = TryCast(theSender, BackgroundWorker)
        If sender Is Nothing Then Exit Sub

        AllowSleepOff()

        Dim sumFileWriter As FileOutputWriter = New FileOutputWriter(GetOutputDirectory(JobFileList(0)))
        Dim sumWriter As SummaryDataContainer = New SummaryDataContainer(sumFileWriter)
        Dim jobContainer As JobContainer = New JobContainer(sumWriter)

        Dim mode As ExecutionMode
        If Cfg.DeclMode Then
            mode = ExecutionMode.Declaration
        Else
            mode = ExecutionMode.Engineering
            Physics.AirDensity = Cfg.AirDensity.SI(Of KilogramPerCubicMeter)()
        End If

        'dictionary of run-identifiers to fileWriters (used for output directory of modfile)
        Dim fileWriters As Dictionary(Of Integer, FileOutputWriter) = New Dictionary(Of Integer, FileOutputWriter)

        'list of finished runs
        Dim finishedRuns As List(Of Integer) = New List(Of Integer)
        For Each jobFile As String In JobFileList
            Try
                sender.ReportProgress(0,
                                      New VectoProgress _
                                         With {.Target = "ListBox", .Message = "Reading File " + jobFile,
                                         .Link = jobFile})

                Dim extension As String = Path.GetExtension(jobFile)
                Dim input As IInputDataProvider = Nothing
                Select Case extension
                    Case VectoCore.Configuration.Constants.FileExtensions.VectoJobFile
                        input = JSONInputDataFactory.ReadJsonJob(jobFile)
                    Case ".xml"
                        Dim xDocument As XDocument = XDocument.Load(jobFile)
                        Dim rootNode As String = If(xDocument Is Nothing, "", xDocument.Root.Name.LocalName)
                        Dim kernel As IKernel = New StandardKernel(New VectoNinjectModule)
                        Dim xmlInputReader As IXMLInputDataReader = kernel.Get(Of IXMLInputDataReader)
                        Select Case rootNode
                            Case XMLNames.VectoInputEngineering
                                input = xmlInputReader.CreateEngineering(jobFile)
                            Case XMLNames.VectoInputDeclaration
                                Using reader As XmlReader = XmlReader.Create(jobFile)
                                    input = xmlInputReader.CreateDeclaration(reader)
                                End Using
                        End Select
                End Select

                If input Is Nothing Then
                    sender.ReportProgress(0,
                                          New VectoProgress _
                                             With {.Target = "ListBoxError",
                                             .Message = "No Input Provider for job: " + jobFile})
                    Continue For
                End If

                Dim outFile As String = GetOutputDirectory(jobFile)
                Dim fileWriter As FileOutputWriter = New FileOutputWriter(outFile)

                Dim runsFactory As ISimulatorFactory = SimulatorFactory.CreateSimulatorFactory(mode, input, fileWriter)
                runsFactory.WriteModalResults = Cfg.ModOut
                runsFactory.ModalResults1Hz = Cfg.Mod1Hz
                runsFactory.Validate = cbValidateRunData.Checked
                runsFactory.ActualModalData = cbActVmod.Checked
                runsFactory.SerializeVectoRunData = cbSaveVectoRunData.Checked

                For Each run as integer In jobContainer.AddRuns(runsFactory)
                    fileWriters.Add(run, fileWriter)
                Next

                ' TODO MQ-20200525: Remove the following loop in production (or after evaluation of LAC!!
                If not string.IsNullOrWhiteSpace(tbMinSpeedLAC.Text) then
                    'for Each run as JobContainer.RunEntry In jobContainer.Runs
                    '    dim tmpDriver as DriverData = CType(run.Run, VectoRun).GetContainer().RunData.DriverData
                    '    tmpDriver.LookAheadCoasting.Enabled = True
                    '    tmpDriver.LookAheadCoasting.MinSpeed = tbMinSpeedLAC.Text.ToDouble().KMPHtoMeterPerSecond()
                    'Next
                end if

                    
                sender.ReportProgress(0,
                                      New VectoProgress _
                                         With {.Target = "ListBox",
                                         .Message = "Finished Reading Data for job: " + jobFile})

            Catch ex As Exception
                MsgBox($"ERROR running job {jobFile}: {ex.Message}", MsgBoxStyle.Critical)
                sender.ReportProgress(0, New VectoProgress With {.Target = "ListBoxError", .Message = ex.Message})
                Return
            End Try
        Next

        'print detected cycles
        For Each cycle As JobContainer.CycleTypeDescription In jobContainer.GetCycleTypes()
            sender.ReportProgress(0,
                                  New VectoProgress _
                                     With {.Target = "ListBox",
                                     .Message = $"Detected Cycle {cycle.Name}: {cycle.CycleType}"})
        Next

        sender.ReportProgress(0, New VectoProgress With {.Target = "ListBox",
                                 .Message = _
                                 $"Starting Simulation ({JobFileList.Count} Jobs, {jobContainer.GetProgress().Count _
                                 } Runs)"})

        jobContainer.Execute(True)

        Dim start As DateTime = DateTime.Now()

        While Not jobContainer.AllCompleted
            'cancel the job if thread is interrupted (button "Stop" clicked)
            If sender.CancellationPending Then
                jobContainer.Cancel()
                Return
            End If

            Dim progress As IDictionary(Of Integer, JobContainer.ProgressEntry) = jobContainer.GetProgress()
            Dim sumProgress As Double = progress.Sum(Function(pair) pair.Value.Progress)
            Dim duration As Double = (DateTime.Now() - start).TotalSeconds

            sender.ReportProgress(Convert.ToInt32((sumProgress*100.0)/progress.Count),
                                  New VectoProgress With {.Target = "Status",
                                     .Message = $"Duration: {duration:0}s, Current Progress: {(sumProgress/progress.Count):P} ({ _
                                     String.Join(", ", progress.Select(Function(pair) $"{pair.Value.Progress,4:P}"))})"})

            Dim justFinished As Dictionary(Of Integer, JobContainer.ProgressEntry) =
                    progress.Where(Function(proc) proc.Value.Done AndAlso Not finishedRuns.Contains(proc.Key)).
                    ToDictionary(
                        Function(pair) pair.Key, Function(pair) pair.Value)
            PrintRuns(justFinished, fileWriters)
            finishedRuns.AddRange(justFinished.Select(Function(pair) pair.Key))
            Thread.Sleep(100)
        End While

        Dim remainingRuns As Dictionary(Of Integer, JobContainer.ProgressEntry) =
                jobContainer.GetProgress().Where(
                    Function(proc) proc.Value.Done AndAlso Not finishedRuns.Contains(proc.Key)).
                ToDictionary(Function(pair) pair.Key, Function(pair) pair.Value)
        PrintRuns(remainingRuns, fileWriters)

        finishedRuns.Clear()
        fileWriters.Clear()

        For Each progressEntry As KeyValuePair(Of Integer, JobContainer.ProgressEntry) In jobContainer.GetProgress()
            sender.ReportProgress(100, New VectoProgress With {.Target = "ListBox",
                                     .Message = String.Format("{0,-60} {1,8:P} {2,10:F2}s - {3}",
                                                              $"{progressEntry.Value.RunName} {progressEntry.Value.CycleName} {progressEntry.Value.RunSuffix}",
                                                              progressEntry.Value.Progress,
                                                              progressEntry.Value.ExecTime/1000.0,
                                                              IIf(progressEntry.Value.Success, "Success", "Aborted"))})
            If (Not progressEntry.Value.Success) Then
                sender.ReportProgress(100,
                                      New VectoProgress _
                                         With {.Target = "ListBox", .Message = progressEntry.Value.Error.Message})
            End If

        Next

        For Each job As String In JobFileList
            dim w as FileOutputWriter = new FileOutputWriter(GetOutputDirectory(job))
            For Each entry as KeyValuePair(Of string, string) In _
                new Dictionary(Of string, string) _
                    from {{w.XMLFullReportName, "XML Manufacturer Report"}, {w.XMLCustomerReportName, "XML Customer Report"},
                        {w.XMLVTPReportName, "VTP Report"}, {w.XMLMonitoringReportName, "XML Monitoring Report"}}
                If File.Exists(entry.Key) Then
                    sender.ReportProgress(100, New VectoProgress With {.Target = "ListBox",
                                             .Message = String.Format("{2} for '{0}' written to {1}", Path.GetFileName(job),entry.Key, entry.Value),
                                             .Link = "<XML>" + entry.Key})
                End If
            Next
        Next

        If File.Exists(sumFileWriter.SumFileName) Then
            sender.ReportProgress(100, New VectoProgress With {.Target = "ListBox",
                                     .Message = $"Sum File written to {sumFileWriter.SumFileName}",
                                     .Link = sumFileWriter.SumFileName})
        End If

        sender.ReportProgress(100, New VectoProgress With {.Target = "ListBox",
                                 .Message = $"Simulation Finished in {(DateTime.Now() - start).TotalSeconds:0}s"})

#if CERTIFICATION_RELEASE
        dim message as string = nothing
#else
#if RELEASE_CANDIDATE
        dim message as string = "RELEASE CANDIDATE - NOT FOR CERTIFICATION!"
#else
        dim message as string = "DEVELOPMENT VERSION - NOT FOR CERTIFICATION!"
#End If
#end if
        if Not string.IsNullOrWhitespace(message) then
            sender.ReportProgress(100,  New VectoProgress With {.Target = "ListBoxWarning",
                                     .Message = message})
        End If
    End Sub

    Private Function GetOutputDirectory(jobFile As String) As String

        dim outFile as String = jobfile
        If (Not string.IsNullOrWhiteSpace(tbOutputFolder.Text)) Then
            Dim outPath as string = tbOutputFolder.Text
            if (path.IsPathRooted(outPath)) Then
                outFile = Path.Combine(outPath, Path.GetFileName(jobFile))
            Else 
                outFile = Path.Combine(path.GetDirectoryName(jobFile), outPath, path.GetFileName(jobFile))
            End If
            If (Not directory.Exists(path.GetDirectoryName(outFile))) then
                Directory.CreateDirectory(path.GetDirectoryName(outFile))
            End If
        End If
        Return outFile
    End Function


    Private Shared Sub PrintRuns(progress As Dictionary(Of Integer, JobContainer.ProgressEntry),
                                 fileWriters As Dictionary(Of Integer, FileOutputWriter))
        For Each p As KeyValuePair(Of Integer, JobContainer.ProgressEntry) In progress
            Dim modFilename As String = fileWriters(p.Key).GetModDataFileName(p.Value.RunName, p.Value.CycleName,
                                                                              p.Value.RunSuffix +
                                                                              If(Cfg.Mod1Hz, "_1Hz", ""))

            Dim runName As String = $"{p.Value.RunName} {p.Value.CycleName} {p.Value.RunSuffix}"

            If Not p.Value.Error Is Nothing Then
                VectoWorkerV3.ReportProgress(0, New VectoProgress With {.Target = "ListBoxError",
                                                .Message = $"Finished Run {runName} with ERROR: {p.Value.Error.Message}",
                                                .Link = modFilename})
            Else
                VectoWorkerV3.ReportProgress(0, New VectoProgress With {.Target = "ListBox",
                                                .Message = $"Finished Run {runName} successfully."})
            End If

            If (File.Exists(modFilename)) Then
                VectoWorkerV3.ReportProgress(0, New VectoProgress With {.Target = "ListBox",
                                                .Message = $"Run {runName}: Modal Results written to {modFilename}", .Link = modFilename})
            End If
        Next
    End Sub

    Private Sub VectoWorkerV3_OnProgressChanged(sender As Object, e As ProgressChangedEventArgs)
        Dim progress As VectoProgress = TryCast(e.UserState, VectoProgress)
        If progress Is Nothing Then Exit Sub

        Select Case progress.Target
            Case "ListBox"
                If progress.Link Is Nothing Then
                    MsgToForm(MessageType.Normal, progress.Message, "", "")
                Else
                    MsgToForm(MessageType.Normal, progress.Message, "", progress.Link)
                End If
            Case "ListBoxWarning"
                MsgToForm(MessageType.Warn, progress.Message, "", "")
                Return
            Case "ListBoxError"
                MsgToForm(MessageType.Err, progress.Message, "", "")
                Return
            Case "Status"
                Status(progress.Message)
        End Select

        ToolStripProgBarOverall.Value = e.ProgressPercentage
    End Sub

    Private Sub VectoWorkerV3_OnRunWorkerCompleted(sender As Object, e As RunWorkerCompletedEventArgs)

        'Progbar reset
        ToolStripProgBarOverall.Visible = False
        ToolStripProgBarOverall.Style = ProgressBarStyle.Continuous
        ToolStripProgBarOverall.Value = 0
        ProgSecStop()

        LvGEN.SelectedIndices.Clear()

        'ShutDown when Unexpected Error
        If e.Error IsNot Nothing Then
            MsgBox("An Unexpected Error occurred!" & ChrW(10) & ChrW(10) &
                   e.Error.Message.ToString, MsgBoxStyle.Critical, "Unexpected Error")
            LogFile.WriteToLog(MessageType.Err, ">>>Unexpected Error:" & e.Error.ToString())
        End If

        'Options enable / GUI reset
        LockGUI(False)
        btStartV3.Text = "START"
        btStartV3.Image = My.Resources.Play_icon
        Status(_lastModeName & " Mode")

        'SLEEP reactivate
        AllowSleepOn()
    End Sub


    Private Sub ModeUpdate()

        'Save lists
        _jobListView.SaveList()

        'GUI changes according to current mode

        If Cfg.DeclMode Then
            _lastModeName = "Declaration"
        Else
            _lastModeName = "Engineering"
        End If

        'Update job counter
        _genChecked = LvGEN.CheckedItems.Count
        UpdateJobTabText()

        'Status label
        Status(_lastModeName & " Mode")
    End Sub

    'Class for ListView control - Job and cycle lists
    Private Class FileListView
        Private ReadOnly _filePath As String
        Private _loadedDefault As Boolean
        Public LVbox As ListView

        Public Sub New(path As String)
            _filePath = path
            _loadedDefault = False
        End Sub

        Public Sub SaveList(Optional ByVal path As String = "")
            Dim x As Integer
            If path = "" Then
                If Not _loadedDefault Then Exit Sub
                path = _filePath
            End If
            Dim file As StreamWriter = My.Computer.FileSystem.OpenTextFileWriter(path, False, Encoding.UTF8)
            For x = 1 To LVbox.Items.Count
                file.WriteLine(String.Join("?", LVbox.Items(x - 1).SubItems(0).Text,
                                           Math.Abs(CInt(LVbox.Items(x - 1).Checked))))
            Next
            file.Close()
        End Sub

        Public Sub LoadList(Optional ByVal path As String = "")
            'Dim line As String()
            Dim noCheck As Boolean
            'Dim file As CsvFile
            Dim listViewItem As ListViewItem

            If path = "" Then
                path = _filePath
                _loadedDefault = True
            End If

            'file = New CsvFile

            If Not File.Exists(path) Then
                If Not _loadedDefault Then GUIMsg(MessageType.Err, "Cannot open file (" & path & ")!")
                Exit Sub
            End If

            MainForm._checkLock = True
            LVbox.BeginUpdate()

            LVbox.Items.Clear()

            noCheck = False
            Dim reader As TextFieldParser = New TextFieldParser(path, Encoding.Default)
            reader.TextFieldType = FieldType.Delimited
            reader.Delimiters = New String() {"?"}

            Do While Not reader.EndOfData
                Dim line As String() = reader.ReadFields()
                If Strings.Left(Trim(line(0)), 1) = "#" Then Continue Do

                listViewItem = New ListViewItem(line(0))
                listViewItem.SubItems.Add(" ")

                If noCheck Then
                    listViewItem.Checked = True
                Else
                    If UBound(line) < 1 Then
                        noCheck = True
                        listViewItem.Checked = True
                    Else
                        If IsNumeric(line(1)) Then
                            listViewItem.Checked = CBool(line(1))
                        Else
                            listViewItem.Checked = True
                        End If
                    End If
                End If
                LVbox.Items.Add(listViewItem)
            Loop

            reader.Close()

            LVbox.EndUpdate()
            MainForm._checkLock = False

            If LVbox.Items.Count > 0 Then LVbox.Items(LVbox.Items.Count - 1).EnsureVisible()
        End Sub
    End Class


    'Open Job Editor and open file (or new file)
    Friend Sub OpenVECTOeditor(x As String, jobType As VectoSimulationJobType)

        If x = "<New>" Then
            ShowVectoJobForm(jobType)
            VectoJobForm.VectoNew()
        ElseIf x = "<VTP>" Then
            ShowVectoEPTPJobForm()
            VectoVTPJobForm.VectoNew()
        Else
            Try
                Dim engJob As IVTPEngineeringInputDataProvider = TryCast(JSONInputDataFactory.ReadComponentData(x),
                                                                         IVTPEngineeringInputDataProvider)
                Dim declJob As IVTPDeclarationInputDataProvider = TryCast(JSONInputDataFactory.ReadComponentData(x),
                                                                          IVTPDeclarationInputDataProvider)
                If engJob Is Nothing AndAlso declJob Is Nothing Then
                    ShowVectoJobForm(jobType)
                    VectoJobForm.VECTOload2Form(x)
                Else
                    ShowVectoEPTPJobForm()
                    VectoVTPJobForm.VECTOload2Form(x)
                End If
            Catch ex As Exception
                MsgBox(ex.Message, MsgBoxStyle.OkOnly, "Error loading Vecto Job File")
            End Try
        End If

        VectoJobForm.Activate()
    End Sub

    Private Sub ShowVectoJobForm(vectoJobType As VectoSimulationJobType)
        VectoJobForm.JobType = vectoJobType
        If Not VectoJobForm.Visible Then
            VectoJobForm.Show()
        Else
            If VectoJobForm.WindowState = FormWindowState.Minimized Then _
                VectoJobForm.WindowState = FormWindowState.Normal
            VectoJobForm.BringToFront()
        End If

    End Sub

    Private Sub ShowVectoEPTPJobForm()
        If Not VectoVTPJobForm.Visible Then
            VectoVTPJobForm.Show()
        Else
            If VectoVTPJobForm.WindowState = FormWindowState.Minimized Then _
                VectoVTPJobForm.WindowState = FormWindowState.Normal
            VectoVTPJobForm.BringToFront()
        End If
    End Sub

    'Save job and cycle file lists
    Private Sub SaveFileLists()
        _jobListView.SaveList()
        'If Cfg.BatchMode Then CycleListView.SaveList()
    End Sub


#Region "Progressbar controls"

    'Initialise progress bar (Start of next job in calculation)

    'Stop - Hide progress bar
    Private Sub ProgSecStop()
        TmProgSec.Stop()
        ToolStripProgBarJob.Visible = False
        ToolStripProgBarJob.Value = 0
    End Sub

    'Timer to update progress bar regularly
    Private Sub TmProgSec_Tick(sender As Object, e As EventArgs) Handles TmProgSec.Tick
        If _guItest.TestActive Then
            Call _guItest.TestTick()
            Exit Sub
        Else
            If Not ProgBarCtrl.ProgLock Then ProgSecUpdate()
        End If
    End Sub

    'Update progress bar (timer controlled)
    Private Sub ProgSecUpdate()

        With ProgBarCtrl

            If .ProgJobInt > 0 AndAlso ToolStripProgBarJob.Style = ProgressBarStyle.Marquee Then
                ToolStripProgBarJob.Style = ProgressBarStyle.Continuous
            End If

            If .ProgJobInt < 0 Then
                .ProgJobInt = 0
            ElseIf .ProgJobInt > 100 Then
                .ProgJobInt = 100
            End If

            ToolStripProgBarJob.Value = .ProgJobInt

            If .ProgOverallStartInt > - 1 Then
                ToolStripProgBarOverall.Value =
                    CInt(.ProgOverallStartInt + (.PgroOverallEndInt - .ProgOverallStartInt)*.ProgJobInt/100)
            End If

        End With
    End Sub


#End Region

#Region "Options Tab"

    'Load options from config class
    Public Sub LoadOptions()
        ChBoxModOut.Checked = Cfg.ModOut
        ChBoxMod1Hz.Checked = Cfg.Mod1Hz

        RbDecl.Checked = Cfg.DeclMode
        cbValidateRunData.Checked = cfg.ValidateRunData

        tbOutputFolder.Text = Cfg.OutputFolder

    End Sub

    'Update config class from options in GUI, e.g. before running calculations 
    Private Sub SetOptions()
        Cfg.ModOut = ChBoxModOut.Checked
        Cfg.Mod1Hz = ChBoxMod1Hz.Checked
        Cfg.ValidateRunData = cbValidateRunData.Checked
        Cfg.OutputFolder = tbOutputFolder.Text
    End Sub

#End Region


    'Add message to message list
    Public Sub MsgToForm(id As MessageType, msg As String, source As String, link As String)

        If (InvokeRequired) Then
            'Me.Invoke(New MsgToFormDelegate(AddressOf MSGtoForm), ID, Msg, Source, Link)
            Exit Sub
        End If
        Dim lv0 As ListViewItem

        lv0 = New ListViewItem
        lv0.Text = msg
        lv0.SubItems.Add(Now.ToString("HH:mm:ss.ff"))
        lv0.SubItems.Add(source)

        If LvMsg.Items.Count > 9999 Then LvMsg.Items.RemoveAt(0)

        LogFile.WriteToLog(id, msg & vbTab & source)

        Select Case id

            Case MessageType.Err

                lv0.BackColor = Color.Red
                lv0.ForeColor = Color.White

            Case MessageType.Warn

                lv0.BackColor = Color.Khaki				 'FromArgb(218, 125, 0) 'DarkOrange
                lv0.ForeColor = Color.Black

            Case Else

                If id = MessageType.NewJob Then
                    lv0.BackColor = Color.LightGray
                    lv0.ForeColor = Color.DarkBlue
                End If

        End Select

        If link <> "" Then
            If Not id = MessageType.Err Then lv0.ForeColor = Color.Blue
            lv0.SubItems(0).Font = New Font(LvMsg.Font, FontStyle.Underline)
            lv0.Tag = link
        End If


        LvMsg.Items.Add(lv0)

        lv0.EnsureVisible()
    End Sub


    'Open link in message list
    Private Sub LvMsg_MouseClick(sender As Object, e As MouseEventArgs) Handles LvMsg.MouseClick
        Dim txt As String
        If LvMsg.SelectedIndices.Count > 0 Then
            If Not LvMsg.SelectedItems(0).Tag Is Nothing Then
                If _
                    Len(CStr(LvMsg.SelectedItems(0).Tag)) > 4 AndAlso
                    Microsoft.VisualBasic.Left(CStr(LvMsg.SelectedItems(0).Tag), 4) = "<UM>" Then
                    txt = CStr(LvMsg.SelectedItems(0).Tag).Replace("<UM>", MyAppPath & "User Manual")
                    txt = txt.Replace(" ", "%20")
                    txt = txt.Replace("\", "/")
                    txt = "file:///" & txt
                    Try
                        Process.Start(new ProcessStartInfo(txt) With {.UseShellExecute = True})
                    Catch ex As Exception
                        MsgBox("Cannot open link!")
                    End Try
                ElseIf _
                    Len(CStr(LvMsg.SelectedItems(0).Tag)) > 5 AndAlso
                    Microsoft.VisualBasic.Left(CStr(LvMsg.SelectedItems(0).Tag), 5) = "<GUI>" Then
                    txt = CStr(LvMsg.SelectedItems(0).Tag).Replace("<GUI>", "")
                    OpenVectoFile(txt)
                ElseIf _
                    Len(CStr(LvMsg.SelectedItems(0).Tag)) > 5 AndAlso
                    Microsoft.VisualBasic.Left(CStr(LvMsg.SelectedItems(0).Tag), 5) = "<RUN>" Then
                    txt = CStr(LvMsg.SelectedItems(0).Tag).Replace("<RUN>", "")
                    Try
                        Process.Start(new ProcessStartInfo(txt) With {.UseShellExecute = true})
                    Catch ex As Exception
                        GUIMsg(MessageType.Err, "Could not run '" & txt & "'!")
                    End Try
                ElseIf _
                    Len(CStr(LvMsg.SelectedItems(0).Tag)) > 5 AndAlso
                    Microsoft.VisualBasic.Left(CStr(LvMsg.SelectedItems(0).Tag), 5) = "<XML>" Then
                    txt = CStr(LvMsg.SelectedItems(0).Tag).Replace("<XML>", "")
                    OpenFiles(txt)
                Else
                    OpenFiles(CStr(LvMsg.SelectedItems(0).Tag))
                End If
            End If
        End If
    End Sub

    'Link-cursor (Hand) for links
    Private Sub LvMsg_MouseMove(sender As Object, e As MouseEventArgs) Handles LvMsg.MouseMove
        Dim lv0 As ListViewItem
        lv0 = LvMsg.GetItemAt(e.Location.X, e.Location.Y)
        If lv0 Is Nothing OrElse lv0.Tag Is Nothing Then
            LvMsg.Cursor = Cursors.Arrow
        Else
            LvMsg.Cursor = Cursors.Hand
        End If
        If _mouseDownOnListView Then
            Try
                LvMsg.HitTest(e.Location).Item.Selected = True
            Catch
            End Try
        End If
    End Sub

#Region "Open File Context Menu"

    Private _contextMenuFiles As String()

    'Initialise and open context menu
    Private Sub OpenFiles(ParamArray files() As String)

        If files.Length = 0 Then Exit Sub

        _contextMenuFiles = files

        OpenInGraphWindowToolStripMenuItem.Enabled = (UCase(GetExtension(_contextMenuFiles(0))) = ".VMOD")

        OpenWithToolStripMenuItem.Text = "Open with " & Cfg.OpenCmdName

        CmOpenFile.Show(Cursor.Position)
    End Sub

    'Open with tool defined in Settings
    Private Sub OpenWithToolStripMenuItem_Click(sender As Object, e As EventArgs) _
        Handles OpenWithToolStripMenuItem.Click
        If Not FileOpenAlt(_contextMenuFiles(0)) Then MsgBox("Failed to open file!")
    End Sub

    Private Sub OpenInGraphWindowToolStripMenuItem_Click(sender As Object, e As EventArgs) _
        Handles OpenInGraphWindowToolStripMenuItem.Click
        Dim graphForm As New GraphForm
        graphForm.Show()
        graphForm.LoadNewFile(_contextMenuFiles(0))
    End Sub

    'Show in folder
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

    'Change Declaraion Mode
    Private Sub RbDecl_CheckedChanged(sender As Object, e As EventArgs) Handles RbDecl.CheckedChanged
        If _cbDeclLock Then Exit Sub

        If VectoJobForm.Visible Or VehicleForm.Visible Or GearboxForm.Visible Or EngineForm.Visible Then
            _cbDeclLock = True
            RbDecl.Checked = Not RbDecl.Checked
            _cbDeclLock = False
            MsgBox("Please close all dialog windows (e.g. Job Editor) before changing mode!")
        Else
            Cfg.DeclMode = RbDecl.Checked
            RbDev.Checked = Not RbDecl.Checked
            DeclOnOff()
        End If
        JobEditorBatteryElectricVehicleToolStripMenuItem.Enabled = Not Cfg.DeclMode
        JobEditorParallelHybridVehicleToolStripMenuItem.Enabled = Not Cfg.DeclMode
        JobEditorEngineOnlyModeToolStripMenuItem.Enabled = Not Cfg.DeclMode
        JobEditorSerialHybridVehicleToolStripMenuItem.Enabled = Not Cfg.DeclMode
    End Sub


#Region "GUI Tests"

    Private ReadOnly _guItest As New GUItest(Me)
    Private _mouseDownOnListView As Boolean

    Private Class GUItest
        Private Const RowLim As Integer = 9
        Private Const ColLim As Integer = 45
        Public TestActive As Boolean = False
        Private _testAborted As Boolean
        Private _xCtrl As Integer
        Private _xPanel As Integer
        Private _scr As Integer
        Private _pRbAlt As Boolean
        Private ReadOnly _ctrls(RowLim + 1) As Integer
        Private ReadOnly _pnls(RowLim + 1) As Integer
        Private _ctrlC As Integer
        Private _ctrlCl As Integer
        Private _pnDir As Integer
        Private _pnDirC As Integer
        Private _pnDirCl As Integer
        Private _pnDirRnd As Integer
        Private _ctrlRnd As Integer
        Private _diffC As Integer
        Private _diffLvl As Integer
        Private _bInit As Integer
        Private ReadOnly _mainForm As MainForm
        Private ReadOnly _keyCode As List(Of Integer)

        Private Sub TestRun()

            Dim z As Integer

            _xPanel = ColLim - 10
            _xCtrl = ColLim - 10
            _pRbAlt = False
            _scr = 0
            _pnDir = 0
            _pnDirCl = 10
            _pnDirC = 0	' StrDirCL
            _ctrlCl = 5
            _ctrlC = _ctrlCl
            _pnDirRnd = 5
            _ctrlRnd = 8
            _diffC = 0
            _diffLvl = 1
            _bInit = 0
            _testAborted = False
            Randomize()


            _mainForm.LvMsg.Items.Clear()
            _mainForm.ToolStripLbStatus.Text = "Score: 0000             Press <Esc> to Quit"

            For z = 1 To RowLim - 6
                _pRbAlt = Not _pRbAlt
                If Not _pRbAlt Then
                    _mainForm.LvMsg.Items.Add(Space(ColLim - 11) & "*|       |*")
                Else
                    _mainForm.LvMsg.Items.Add(Space(ColLim - 11) & "*|   |   |*")
                End If
            Next

            _pRbAlt = False

            _mainForm.LvMsg.Items.Add("  VECTO Interactive Mode" & Space(ColLim - 35) & "*|       |*")
            _mainForm.LvMsg.Items.Add(Space(ColLim - 11) & "*|   |   |*")
            _mainForm.LvMsg.Items.Add(Space(ColLim - 11) & "*|       |*")
            _mainForm.LvMsg.Items.Add(Space(ColLim - 11) & "*|   |   |*")
            _mainForm.LvMsg.Items.Add(Space(ColLim - 11) & "*|       |*")
            _mainForm.LvMsg.Items.Add(Space(ColLim - 11) & "*|   ∆   |*")

            For z = 1 To RowLim + 1
                _pnls(z) = ColLim - 10
                _ctrls(z) = 0
            Next

            _mainForm.TmProgSec.Interval = 200

            _mainForm.LvMsg.Focus()

            _mainForm.TmProgSec.Start()
        End Sub

        Public Sub TestStop()
            _mainForm.TmProgSec.Stop()
            TestActive = False
            _mainForm.LvMsg.Items.Clear()
            _ctrlC = 0
            _mainForm.ToolStripLbStatus.Text = _mainForm._lastModeName & " Mode"
        End Sub

        Public Sub TestTick()

            If _bInit = 24 Then GoTo LbRace
            _bInit += 1

            Select Case _bInit
                Case 10
                    _mainForm.LvMsg.Items.RemoveAt(RowLim - 6)
                    _mainForm.LvMsg.Items.RemoveAt(RowLim - 5)
                    _mainForm.LvMsg.Items.Insert(RowLim - 6, Space(ColLim - 11) & "*|       |*")
                    _mainForm.LvMsg.Items.Insert(RowLim - 4,
                                                 Space(ColLim - 30) & "  3      " & Space(10) & "*|       |*")
                Case 14
                    _mainForm.LvMsg.Items.RemoveAt(RowLim - 4)
                    _mainForm.LvMsg.Items.Insert(RowLim - 4,
                                                 Space(ColLim - 30) & "  2      " & Space(10) & "*|       |*")
                Case 18
                    _mainForm.LvMsg.Items.RemoveAt(RowLim - 4)
                    _mainForm.LvMsg.Items.Insert(RowLim - 4,
                                                 Space(ColLim - 30) & "  1      " & Space(10) & "*|       |*")
                Case 22
                    _mainForm.LvMsg.Items.RemoveAt(RowLim - 4)
                    _mainForm.LvMsg.Items.Insert(RowLim - 4,
                                                 Space(ColLim - 30) & " Go!     " & Space(10) & "*|       |*")
                Case 24
                    _mainForm.LvMsg.Items.RemoveAt(RowLim - 4)
                    _mainForm.LvMsg.Items.Insert(RowLim - 4,
                                                 Space(ColLim - 30) & "         " & Space(10) & "*|       |*")
            End Select
            Exit Sub
            LbRace:

            _pRbAlt = Not _pRbAlt

            _mainForm.LvMsg.BeginUpdate()

            Lists()

            Align()

            SetCtrl()

            SetPanel()

            _mainForm.LvMsg.Items.RemoveAt(RowLim)

            UpdateCtrl()

            _mainForm.LvMsg.EndUpdate()

            If Math.Abs(_xCtrl - _pnls(2)) > 4 Then
                Abort()
                Exit Sub
            ElseIf _ctrls(2) <> 0 Then
                If _xCtrl = _pnls(2) + _ctrls(2) - 4 Then
                    Abort()
                    Exit Sub
                End If
                _scr += 5*_diffLvl
            End If

            _scr += _diffLvl
            _diffC += 1

            'Erhöhe Schwierigkeitsgrad
            If _diffC = (_diffLvl + 3)*4 Then
                _diffC = 0
                _diffLvl += 1
                If _diffLvl > 2 And _diffLvl < 7 Then _mainForm.TmProgSec.Interval = 300 - (_diffLvl)*30
                _scr += 100
                Select Case _diffLvl
                    Case 3
                        _pnDirCl = 3
                        _ctrlCl = 4
                        _ctrlRnd = 6
                    Case 5
                        _pnDirCl = 2
                        _pnDirRnd = 4
                    Case 8
                        _ctrlCl = 2
                    Case 10
                        _ctrlRnd = 4
                        _pnDirRnd = 3
                End Select
            End If
        End Sub

        Public Sub TestKey(key As Integer)

            If TestActive Then
                Select Case key
                    Case Keys.Left
                        _xCtrl -= 1
                        UpdateCtrl()
                    Case Keys.Right
                        _xCtrl += 1
                        UpdateCtrl()
                    Case Keys.Escape
                        TestStop()
                End Select
            Else

                If _keyCode(_ctrlC) = key Then
                    _ctrlC += 1
                    If _ctrlC = _keyCode.Count Then
                        TestActive = True
                        TestRun()
                    End If
                Else
                    _ctrlC = 0
                End If

            End If
        End Sub

        Private Sub Abort()

            Dim s As String, s1 As String

            If _testAborted Then Exit Sub

            _testAborted = True

            _mainForm.TmProgSec.Stop()

            _mainForm.LvMsg.BeginUpdate()

            s = _mainForm.LvMsg.Items(0).Text
            _mainForm.LvMsg.Items.RemoveAt(0)
            _mainForm.LvMsg.Items.Insert(0, "You crashed!" & Microsoft.VisualBasic.Right(s, Len(s) - 12))

            s = _mainForm.LvMsg.Items(1).Text
            s1 = "Score: " & _scr & " "
            _mainForm.LvMsg.Items.RemoveAt(1)
            _mainForm.LvMsg.Items.Insert(1, s1 & Microsoft.VisualBasic.Right(s, Len(s) - Len(s1)))

            _mainForm.LvMsg.EndUpdate()

            LogFile.WriteToLog(MessageType.Normal, "*** Race Score: " & _scr.ToString("0000") & " ***")

            _ctrlC = 0
            TestActive = False

            _mainForm.ToolStripLbStatus.Text = _mainForm._lastModeName & " Mode"
        End Sub

        Private Sub SetCtrl()
            Dim x As Integer
            If _scr < 10 Then Exit Sub
            _ctrls(RowLim + 1) = 0
            _ctrlC += 1
            If _ctrlC < _ctrlCl Then Exit Sub
            Select Case CInt(Int((_ctrlRnd*Rnd()) + 1))
                Case 1, 2
                    _ctrlC = 0
                    x = CInt(Int((7*Rnd()) + 1))
                    _ctrls(RowLim + 1) = x
            End Select
        End Sub

        Private Sub UpdateCtrl()
            Dim s As String
            If _bInit < 21 Then
                _xCtrl = ColLim - 10
                Exit Sub
            End If
            If Math.Abs(_xCtrl - _pnls(1)) > 5 Then
                Abort()
                Exit Sub
            End If
            s = Replace(_mainForm.LvMsg.Items(RowLim - 1).Text.ToString, "∆", " ") & "   "
            s = Microsoft.VisualBasic.Left(s, ColLim + 15)
            's = s.Remove(0, 20)
            's = "Press <Esc> to Quit " & s
            If Mid(s, _xCtrl + 5, 1) = "X" Then
                Abort()
                Exit Sub
            End If
            s = s.Remove(_xCtrl + 4, 1)
            's = Trim(s.Insert(xCar + 4, "∆")) & Space(ColLim + 5 - Streets(2)) & "Pts: " & Pts & " Lv: " & DiffLvl
            s = Space(_pnls(2) - 1) & Trim(s.Insert(_xCtrl + 4, "∆"))
            _mainForm.LvMsg.Items.RemoveAt(RowLim - 1)
            _mainForm.LvMsg.Items.Insert(RowLim - 1, s)
            _mainForm.ToolStripLbStatus.Text = "Score: " & _scr.ToString("0000") & "             Press <Esc> to Quit"
        End Sub

        Private Sub SetPanel()
            Dim s As String
            s = "*|   |   |*"
            If _pRbAlt Then
                s = s.Remove(5, 1)
                s = s.Insert(5, " ")
            End If
            If _ctrls(RowLim + 1) <> 0 Then
                s = s.Remove(_ctrls(RowLim + 1) + 1, 1)
                s = s.Insert(_ctrls(RowLim + 1) + 1, "X")
            End If
            Select Case _xPanel - _pnls(RowLim)
                Case - 1
                    s = Replace(s, "|", "\")
                Case 1
                    s = Replace(s, "|", "/")
            End Select
            _mainForm.LvMsg.Items.Insert(0, Space(_xPanel - 1) & s)
        End Sub

        Private Sub Align()
            _pnDirC += 1
            If _pnDirC < _pnDirCl Then GoTo Lb1
            _pnDirC = 0
            Select Case CInt(Int((_pnDirRnd*Rnd()) + 1))
                Case 1
                    _pnDir = 1
                Case 2
                    _pnDir = - 1
                Case Else
                    _pnDir = 0
            End Select
            Lb1:
            _xPanel += _pnDir
            If _xPanel > ColLim Then
                _xPanel = ColLim
            ElseIf _xPanel < 22 Then
                _xPanel = 22
            End If
            _pnls(RowLim + 1) = _xPanel
        End Sub

        Private Sub Lists()
            Dim x As Integer
            For x = 2 To RowLim + 1
                _ctrls(x - 1) = _ctrls(x)
                _pnls(x - 1) = _pnls(x)
            Next
        End Sub

        Public Sub New(form As MainForm)
            _mainForm = form
            _keyCode = New List(Of Integer)
            _keyCode.Add(Keys.Up)
            _keyCode.Add(Keys.Up)
            _keyCode.Add(Keys.Down)
            _keyCode.Add(Keys.Down)
            _keyCode.Add(Keys.Left)
            _keyCode.Add(Keys.Right)
            _keyCode.Add(Keys.Left)
            _keyCode.Add(Keys.Right)
            _keyCode.Add(Keys.B)
            _keyCode.Add(Keys.A)
            _ctrlC = 0
        End Sub
    End Class

    Private Sub LvMsg_KeyDown(sender As Object, e As KeyEventArgs) Handles LvMsg.KeyDown
        _guItest.TestKey(e.KeyValue)
        If _guItest.TestActive Then e.SuppressKeyPress = True
    End Sub

    Private Sub LvMsg_LostFocus(sender As Object, e As EventArgs) Handles LvMsg.LostFocus
        If _guItest.TestActive Then _guItest.TestStop()
    End Sub

#End Region

    Private Sub LvMsg_KeyUp(sender As Object, e As KeyEventArgs) Handles LvMsg.KeyUp
        If (e.Control And e.KeyCode = Keys.C) Then
            Dim builder As StringBuilder = New StringBuilder()
            For Each selectedItem As ListViewItem In LvMsg.SelectedItems
                builder.AppendLine(String.Join(", ",
                                               selectedItem.SubItems.Cast (Of ListViewItem.ListViewSubItem).Select(
                                                   Function(item) item.Text)))
            Next
            Clipboard.SetText(builder.ToString())
        End If
    End Sub

    Private Sub LvMsg_MouseDown(sender As Object, e As MouseEventArgs) Handles LvMsg.MouseDown
        _mouseDownOnListView = True
    End Sub

    Private Sub LvMsg_MouseUp(sender As Object, e As MouseEventArgs) Handles LvMsg.MouseUp
        _mouseDownOnListView = False
    End Sub

    Private Sub RbDev_CheckedChanged(sender As Object, e As EventArgs) Handles RbDev.CheckedChanged
    End Sub


    Private Class VectoProgress
        Public Target As String
        Public Message As String
        Public Link As String
    End Class

    Private Sub btnExportXML_Click(sender As Object, e As EventArgs) Handles btnExportXML.Click

        If LvGEN.SelectedItems.Count < 1 Then
            If LvGEN.Items.Count = 1 Then
                LvGEN.Items(0).Selected = True
            Else
                Exit Sub
            End If
        End If

        Dim f As String = LvGEN.SelectedItems(0).SubItems(0).Text
        f = FileRepl(f)
        If Not File.Exists(f) Then
            MsgBox(f & " not found!")
            Return
        End If
        Try
            Dim input As IInputDataProvider = Nothing
            Dim extension As String = Path.GetExtension(f)
            Select Case extension
                Case ".vecto"
                    input = JSONInputDataFactory.ReadJsonJob(f)
                Case ".xml"
                    Dim xDocument As XDocument = xDocument.Load(f)
                    Dim rootNode As String = If(xDocument Is Nothing, "", xDocument.Root.Name.LocalName)
                    Dim kernel as IKernel = New StandardKernel(new VectoNinjectModule)
                    Dim xmlInputReader as IXMLInputDataReader = kernel.Get(Of IXMLInputDataReader)
                    Select Case rootNode
                        Case XMLNames.VectoInputEngineering
                            input = xmlInputReader.CreateEngineering(f)
                        Case XMLNames.VectoInputDeclaration
                            input = xmlInputReader.CreateDeclaration(XmlReader.Create(f))
                    End Select
            End Select

            If input Is Nothing Then Throw New VectoException("No InputDataProvider for file {0} found!", f)

            XMLExportJobDialog.Initialize(input)
            XMLExportJobDialog.ShowDialog()
        Catch ex As Exception
            MsgBox("Exporting job failed: " + ex.Message)
        End Try
    End Sub

    Private Sub LvGEN_SelectedIndexChanged(sender As Object, e As EventArgs) Handles LvGEN.SelectedIndexChanged
        btnExportXML.Enabled = (LvGEN.SelectedItems.Count = 1)
    End Sub

    Private Sub btnImportXML_Click(sender As Object, e As EventArgs) Handles btnImportXML.Click
        'Try
        '	Dim jobFile As String = PluginRegistry.Instance.GetImportPlugin("TUG.IVT.Vecto.XMLImport").ImportJob()
        '	AddToJobListView(jobFile)
        'Catch ex As Exception
        '	MsgBox("Importing job failed: " + ex.Message)
        'End Try
    End Sub

    Private Sub LvGEN_MouseClick(sender As Object, e As MouseEventArgs) Handles  LvGEN.MouseDown
        If e.Button = MouseButtons.Right Then
            _conMenTarget = LvGEN
            _conMenTarJob = True

            'Locked functions show/hide
            LoadListToolStripMenuItem.Enabled = Not _guIlocked
            LoadDefaultListToolStripMenuItem.Enabled = Not _guIlocked
            ClearListToolStripMenuItem.Enabled = Not _guIlocked

            ConMenFilelist.Show(MousePosition)
        End If
    End Sub

    Private Sub ShowInFolderMenuItem_Click(sender As Object, e As EventArgs) Handles ShowInFolderMenuItem.Click

        For Each item As ListViewItem In LvGEN.SelectedItems
            Dim fileName As String = FileRepl(item.SubItems(0).Text)
            If File.Exists(fileName) Then
                Try
                    Process.Start("explorer", "/select,""" & fileName & "")
                Catch ex As Exception
                    MsgBox("Failed to open file!")
                End Try
            Else
                MsgBox("File not found: " & fileName)
            End If
        Next
    End Sub

    Private Sub EPTPJobEditorToolStripMenuItem_Click(sender As Object, e As EventArgs) _
        Handles EPTPJobEditorToolStripMenuItem.Click
        OpenVECTOeditor("<VTP>", VectoSimulationJobType.ConventionalVehicle)
    End Sub

    Private Sub BtTCfileBrowse_Click(sender As Object, e As EventArgs) Handles BtTCfileBrowse.Click
        If Not FolderFileBrowser.OpenDialog("") Then
            Exit Sub
        End If

        Dim filePath As String = FolderFileBrowser.Files(0)
        tbOutputFolder.Text = Path.GetFullPath(filePath)
    End Sub

    Private Sub JobEditorParallelHybridVehicleToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles JobEditorParallelHybridVehicleToolStripMenuItem.Click
        OpenVECTOeditor("<New>", VectoSimulationJobType.ParallelHybridVehicle)
    End Sub

    Private Sub JobEditorBatteryElectricVehicleToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles JobEditorBatteryElectricVehicleToolStripMenuItem.Click
        OpenVECTOeditor("<New>", VectoSimulationJobType.BatteryElectricVehicle)
    End Sub

    Private Sub JobEditorEngineOnlyModeToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles JobEditorEngineOnlyModeToolStripMenuItem.Click
        OpenVECTOeditor("<New>", VectoSimulationJobType.EngineOnlySimulation)
    End Sub

    Private Sub JobEditorSerialHybridVehicleToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles JobEditorSerialHybridVehicleToolStripMenuItem.Click
        OpenVECTOeditor("<New>", VectoSimulationJobType.SerialHybridVehicle)
    End Sub
    Private Sub JobEditorIEPC_E_VehicleToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles JobEditorIEPC_E_VehicleToolStripMenuItem.Click
        OpenVECTOeditor("<New>", VectoSimulationJobType.IEPC_E)
    End Sub

    Private Sub JobEditorIHPCVehicleToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles JobEditorIHPCVehicleToolStripMenuItem.Click 
        OpenVECTOeditor("<New>", VectoSimulationJobType.IHPC)
    End Sub

    Private Sub ToolStripMenuItem1_Click(sender As Object, e As EventArgs) Handles JobEditorIEPC_S_VehicleToolStripMenuItem.Click
        OpenVECTOeditor("<New>", VectoSimulationJobType.IEPC_S)
    End Sub
End Class
