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
'Option Infer On

Imports System.Collections.Generic
Imports System.Drawing.Imaging
Imports System.IO
Imports System.Linq
Imports System.Windows.Forms.DataVisualization.Charting
Imports TUGraz.VECTO.Input_Files
Imports TUGraz.VectoCommon.Exceptions
Imports TUGraz.VectoCommon.InputData
Imports TUGraz.VectoCommon.Models
Imports TUGraz.VectoCommon.Utils
Imports TUGraz.VectoCore.InputData.FileIO.JSON
Imports TUGraz.VectoCore.InputData.Reader.ComponentData
Imports TUGraz.VectoCore.InputData.Reader.DataObjectAdapter.SimulationComponents
Imports TUGraz.VectoCore.InputData.Reader.DataObjectAdapter.SimulationComponents.AuxiliaryDataAdapter
Imports TUGraz.VectoCore.Models.Declaration
Imports TUGraz.VectoCore.Models.Declaration.Auxiliaries
Imports TUGraz.VectoCore.Models.Simulation.Impl
Imports TUGraz.VectoCore.Models.SimulationComponent.Data.Engine
Imports TUGraz.VectoCore.Models.SimulationComponent.Data.Gearbox

''' <summary>
''' Job Editor. Create/Edit VECTO job files (.vecto)
''' </summary>
''' <remarks></remarks>
Public Class VectoJobForm
	Public VectoFile As String
	Private _changed As Boolean = False

	Private _pgDriver As TabPage

	Private _pgDriverOn As Boolean = True

	Private _auxDialog As VehicleAuxiliariesDialog

	Enum AuxViewColumns
		AuxID = 0
		AuxType = 1
		AuxInputOrTech = 2
	End Enum

    Public Property JobType As VectoSimulationJobType




	'Initialise form
	Private Sub F02_GEN_Load(sender As Object, e As EventArgs) Handles Me.Load
		Dim x As Integer

		_auxDialog = New VehicleAuxiliariesDialog

		_pgDriver = TabPgDriver

		For x = 0 To tcJob.TabCount - 1
			tcJob.TabPages(x).Show()
		Next

		LvAux.Columns(AuxViewColumns.AuxInputOrTech).Width = -2

		'Declaration Mode
		If Cfg.DeclMode Then
			LvAux.Columns(AuxViewColumns.AuxInputOrTech).Text = "Technology"
		Else
			LvAux.Columns(AuxViewColumns.AuxInputOrTech).Text = "Input File"
		End If
	    pnAuxEngineering.Enabled = Not Cfg.DeclMode
        pnAuxDeclarationMode.Enabled = cfg.DeclMode
        gbBusAux.Enabled = Not cfg.DeclMode

        'CbEngOnly.Enabled = Not Cfg.DeclMode
        GrCycles.Enabled = Not Cfg.DeclMode
		GrVACC.Enabled = Not Cfg.DeclMode
		RdOff.Enabled = Not Cfg.DeclMode
		GrLAC.Enabled = Not Cfg.DeclMode
		PnEcoRoll.Enabled = Not Cfg.DeclMode

		gbEcoRoll.Enabled = not Cfg.DeclMode
        gbEngineStopStart.Enabled = Not Cfg.DeclMode
        gbPCC.Enabled = Not Cfg.DeclMode

		_changed = False

        'Attempt to select that found in Config

        UpdateEnabledControls()
    End Sub

    Private Sub SetFormHeader()

        Dim prefix As String = "Job Editor - "
        Select Case JobType
            Case VectoSimulationJobType.ParallelHybridVehicle
                lblTitle.Text = prefix + "Parallel Hybrid Vehicle"
                gbElectricAux.Enabled = True
                GrAuxMech.Enabled = True
            case VectoSimulationJobType.SerialHybridVehicle
                lblTitle.Text = prefix + "Serial Hybrid Vehicle"
                gbElectricAux.Enabled = true
                GrAuxMech.Enabled = true
            Case VectoSimulationJobType.BatteryElectricVehicle
                lblTitle.Text = prefix + "Battery Electric Vehicle"
                gbElectricAux.Enabled = True
                GrAuxMech.Enabled = False
            Case VectoSimulationJobType.FCHV
                lblTitle.Text = prefix + "Fuel Cell Vehicle"
                gbElectricAux.Enabled = True
                GrAuxMech.Enabled = False
            Case VectoSimulationJobType.FCHV_IEPC
                lblTitle.Text = prefix + "Fuel Cell IEPC Vehicle"
                gbElectricAux.Enabled = True
                GrAuxMech.Enabled = Cfg.DeclMode
            Case VectoSimulationJobType.ConventionalVehicle
                lblTitle.Text = prefix + "Conventional Vehicle"
                gbElectricAux.Enabled = False
                GrAuxMech.Enabled = True
            Case VectoSimulationJobType.EngineOnlySimulation
                lblTitle.Text = prefix + "Engine Only"
            Case VectoSimulationJobType.IEPC_E
                lblTitle.Text = prefix + "IEPC-E Vehicle"
                gbElectricAux.Enabled = True
                GrAuxMech.Enabled = Cfg.DeclMode
            Case VectoSimulationJobType.IEPC_S
                lblTitle.Text = prefix + "IEPC-S Vehicle"
                gbElectricAux.Enabled = True
                GrAuxMech.Enabled = False
            Case VectoSimulationJobType.IHPC
                lblTitle.Text = prefix + "IHPC Vehicle"
                gbElectricAux.Enabled = True
                GrAuxMech.Enabled = False

        End Select
    End Sub

    'Close - Check for unsaved changes
    Private Sub F02_GEN_FormClosing(sender As Object, e As FormClosingEventArgs) Handles Me.FormClosing
        If e.CloseReason <> CloseReason.ApplicationExitCall And e.CloseReason <> CloseReason.WindowsShutDown Then
            e.Cancel = ChangeCheckCancel()
        End If
    End Sub

    'Set generic values for Declaration mode
    Private Sub DeclInit()

        If Not Cfg.DeclMode Then Exit Sub

        LvCycles.Items.Clear()
        'CbEngOnly.Checked = False
        TbDesMaxFile.Text = ""
        RdOverspeed.Checked = True
        CbLookAhead.Checked = True

        tbLacPreviewFactor.Text = DeclarationData.Driver.LookAhead.LookAheadDistanceFactor.ToGUIFormat()
        tbLacDfTargetSpeedFile.Text = ""
        tbLacDfVelocityDropFile.Text = ""

        TbOverspeed.Text = DeclarationData.Driver.OverSpeed.AllowedOverSpeed.AsKmph.ToGUIFormat()    'cDeclaration.Overspeed
        ' cDeclaration.Underspeed
        TbVmin.Text = DeclarationData.Driver.OverSpeed.MinSpeed.AsKmph.ToGUIFormat()     'cDeclaration.ECvmin
        TbAuxPAuxICEOn.Text = ""

        Dim auxList As List(Of AuxiliaryType)
        Select Case JobType
            Case VectoSimulationJobType.ConventionalVehicle, VectoSimulationJobType.ParallelHybridVehicle, VectoSimulationJobType.IHPC, VectoSimulationJobType.SerialHybridVehicle, VectoSimulationJobType.IEPC_S
                auxList = New HeavyLorryAuxiliaryDataAdapter().AuxiliaryTypes.OrderBy(Function(x) x).ToList()
            Case VectoSimulationJobType.BatteryElectricVehicle, VectoSimulationJobType.IEPC_E, VectoSimulationJobType.FCHV, VectoSimulationJobType.FCHV_IEPC
                auxList = New HeavyLorryPEVAuxiliaryDataAdapter().AuxiliaryTypes.OrderBy(Function(x) x).ToList()
            Case Else
                auxList = New List(Of AuxiliaryType)
        End Select

        Dim auxTechs = New Dictionary(Of AuxiliaryType, IDeclarationAuxiliaryTable) From {
                {AuxiliaryType.Fan, DeclarationData.Fan},
                {AuxiliaryType.SteeringPump, DeclarationData.SteeringPump},
                {AuxiliaryType.HVAC, DeclarationData.HeatingVentilationAirConditioning},
                {AuxiliaryType.ElectricSystem, DeclarationData.ElectricSystem},
                {AuxiliaryType.PneumaticSystem, DeclarationData.PneumaticSystem}
                }

        Dim toRemove As List(Of ListViewItem) = New List(Of ListViewItem)

        For Each item As ListViewItem In LvAux.Items
            If Not auxTechs.Keys.Select(Function(x) x.Key()).Contains(item.Text) Then
                toRemove.Add(item)
            End If
        Next
        For Each item As ListViewItem In toRemove
            item.Remove()
        Next

        For Each entry As AuxiliaryType In auxList
            Dim found = False
            For Each item As ListViewItem In LvAux.Items
                If item.Text.Equals(entry.Key(), StringComparison.CurrentCultureIgnoreCase) Then
                    found = True
                    Exit For
                End If
            Next
            If found Then Continue For

            LvAux.Items.Add(GetTechListForAux(entry, auxTechs(entry)))

        Next

        'For Each auxiliaryType As AuxiliaryType In auxList
        '    LvAux.Items.Add(GetTechListForAux(auxiliaryType, auxTechs(auxiliaryType)))
        'Next

    End Sub

    Protected Function GetTechListForAux(type As AuxiliaryType, aux As IDeclarationAuxiliaryTable) _
        As ListViewItem
        Dim listViewItem As ListViewItem

        listViewItem = New ListViewItem(type.Key())
        listViewItem.SubItems.Add(type.Name())
        Dim auxtech As String() = aux.GetTechnologies()
        If auxtech.Count > 1 Then
            listViewItem.SubItems.Add("")
        Else
            listViewItem.SubItems.Add(auxtech(0))
        End If
        Return listViewItem
    End Function


    'Show/Hide "Driver Assist" Tab
    Private Sub SetDrivertab(onOff As Boolean)
        If onOff Then
            If Not _pgDriverOn Then
                _pgDriverOn = True
                tcJob.TabPages.Insert(1, _pgDriver)
            End If
        Else
            If _pgDriverOn Then
                _pgDriverOn = False
                tcJob.Controls.Remove(_pgDriver)
            End If
        End If
    End Sub


#Region "Browse Buttons"

    Private Sub ButtonVEH_Click(sender As Object, e As EventArgs) Handles ButtonVEH.Click
        If VehicleFileBrowser.OpenDialog(FileRepl(TbVEH.Text, GetPath(VectoFile))) Then
            TbVEH.Text = GetFilenameWithoutDirectory(VehicleFileBrowser.Files(0), GetPath(VectoFile))
        End If
    End Sub

    Private Sub ButtonMAP_Click(sender As Object, e As EventArgs) Handles ButtonMAP.Click
        If EngineFileBrowser.OpenDialog(FileRepl(TbENG.Text, GetPath(VectoFile))) Then
            TbENG.Text = GetFilenameWithoutDirectory(EngineFileBrowser.Files(0), GetPath(VectoFile))
        End If
    End Sub

    Private Sub ButtonGBX_Click(sender As Object, e As EventArgs) Handles ButtonGBX.Click
        If GearboxFileBrowser.OpenDialog(FileRepl(TbGBX.Text, GetPath(VectoFile))) Then
            TbGBX.Text = GetFilenameWithoutDirectory(GearboxFileBrowser.Files(0), GetPath(VectoFile))
        End If
    End Sub

    Private Sub BtDesMaxBr_Click_1(sender As Object, e As EventArgs) Handles BtDesMaxBr.Click
        If DriverAccelerationFileBrowser.OpenDialog(FileRepl(TbDesMaxFile.Text, GetPath(VectoFile))) Then
            TbDesMaxFile.Text = GetFilenameWithoutDirectory(DriverAccelerationFileBrowser.Files(0), GetPath(VectoFile))
        End If
    End Sub

    Private Sub BtAccOpen_Click(sender As Object, e As EventArgs) Handles BtAccOpen.Click
        OpenFiles(FileRepl(TbDesMaxFile.Text, GetPath(VectoFile)))
    End Sub

#End Region

#Region "Open Buttons"

    'Open Vehicle Editor
    Private Sub ButOpenVEH_Click(sender As Object, e As EventArgs) Handles ButOpenVEH.Click
        Dim f As String
        f = FileRepl(TbVEH.Text, GetPath(VectoFile))

        'Thus Veh-file is returned
        VehicleForm.JobDir = GetPath(VectoFile)
        VehicleForm.AutoSendTo = True
        VehicleForm.VehicleType = JobType

        If Not Trim(f) = "" Then
            If Not File.Exists(f) Then
                MsgBox("File not found!")
                Exit Sub
            End If
        End If

        If Not VehicleForm.Visible Then
            VehicleForm.Show()
        Else
            If VehicleForm.WindowState = FormWindowState.Minimized Then VehicleForm.WindowState = FormWindowState.Normal
            VehicleForm.BringToFront()
        End If

        If Not Trim(f) = "" Then
            Try
                VehicleForm.OpenVehicle(f)
            Catch ex As Exception
                MsgBox(ex.Message, MsgBoxStyle.OkOnly, "Error loading Vehicle File")
            End Try
        End If
    End Sub

    'Open Engine Editor
    Private Sub ButOpenENG_Click(sender As Object, e As EventArgs) Handles ButOpenENG.Click
        Dim f As String
        f = FileRepl(TbENG.Text, GetPath(VectoFile))

        'Thus Veh-file is returned
        EngineForm.JobDir = GetPath(VectoFile)
        EngineForm.AutoSendTo = True
        EngineForm.JobType = JobType

        If Not Trim(f) = "" Then
            If Not File.Exists(f) Then
                MsgBox("File not found!")
                Exit Sub
            End If
        End If

        If Not EngineForm.Visible Then
            EngineForm.Show()
        Else
            If EngineForm.WindowState = FormWindowState.Minimized Then EngineForm.WindowState = FormWindowState.Normal
            EngineForm.BringToFront()
        End If
        Try
            If Not Trim(f) = "" Then EngineForm.OpenEngineFile(f)
        Catch ex As Exception
            MsgBox(ex.Message, MsgBoxStyle.OkOnly, "Error loading Engine File")
        End Try
    End Sub

    'Open Gearbox Editor
    Private Sub ButOpenGBX_Click(sender As Object, e As EventArgs) Handles ButOpenGBX.Click
        Dim f As String
        f = FileRepl(TbGBX.Text, GetPath(VectoFile))

        'Thus Veh-file is returned
        GearboxForm.JobDir = GetPath(VectoFile)
        GearboxForm.AutoSendTo = True

        If Not Trim(f) = "" Then
            If Not File.Exists(f) Then
                MsgBox("File not found!")
                Exit Sub
            End If
        End If

        If Not GearboxForm.Visible Then
            GearboxForm.Show()
        Else
            If GearboxForm.WindowState = FormWindowState.Minimized Then GearboxForm.WindowState = FormWindowState.Normal
            GearboxForm.BringToFront()
        End If
        Dim vehicleType As VehicleCategory
        Dim jobType As VectoSimulationJobType
        Try
            If Not Trim(f) = "" Then
                Dim vehInput As IVehicleEngineeringInputData =
                        CType(JSONInputDataFactory.ReadComponentData(FileRepl(TbVEH.Text, GetPath(VectoFile))),
                            IEngineeringInputDataProvider).JobInputData.Vehicle
                vehicleType = vehInput.VehicleCategory
                jobType = vehInput.VehicleType
            End If

        Catch ex As Exception
            vehicleType = VehicleCategory.RigidTruck
            jobType = VectoSimulationJobType.ConventionalVehicle
        End Try
        Try
            If Not Trim(f) = "" Then GearboxForm.OpenGbx(f, vehicleType, jobType)
        Catch ex As Exception
            MsgBox("Failed to open Gearbox File: " + ex.Message)
        End Try
    End Sub

#End Region

#Region "Toolbar"

    'New
    Private Sub ToolStripBtNew_Click(sender As Object, e As EventArgs) Handles ToolStripBtNew.Click
        VectoNew()
    End Sub

    'Open
    Private Sub ToolStripBtOpen_Click(sender As Object, e As EventArgs) Handles ToolStripBtOpen.Click
        If JobfileFileBrowser.OpenDialog(VectoFile, False, "vecto") Then
            Try
                VECTOload2Form(JobfileFileBrowser.Files(0))
            Catch ex As Exception
                MsgBox(ex.Message, MsgBoxStyle.OkOnly, "Error loading Vecto Job File")
            End Try

        End If
    End Sub

    'Save
    Private Sub ToolStripBtSave_Click(sender As Object, e As EventArgs) Handles ToolStripBtSave.Click
        Save()
    End Sub

    'Save As
    Private Sub ToolStripBtSaveAs_Click(sender As Object, e As EventArgs) Handles ToolStripBtSaveAs.Click
        If JobfileFileBrowser.SaveDialog(VectoFile) Then Call VECTOsave(JobfileFileBrowser.Files(0))
    End Sub

    'Send to Job file list in main form
    Private Sub ToolStripBtSendTo_Click(sender As Object, e As EventArgs) Handles ToolStripBtSendTo.Click
        If ChangeCheckCancel() Then Exit Sub
        If VectoFile = "" Then
            MsgBox("File not found!" & ChrW(10) & ChrW(10) & "Save file and try again.")
        Else
            MainForm.AddToJobListView(VectoFile)
        End If
    End Sub

    'Help
    Private Sub ToolStripButton1_Click(sender As Object, e As EventArgs) Handles ToolStripButton1.Click
        If File.Exists(Path.Combine(MyAppPath, "User Manual\help.html")) Then
            Dim defaultBrowserPath As String = BrowserUtils.GetDefaultBrowserPath()
            Process.Start(defaultBrowserPath,
                          $"""file://{Path.Combine(MyAppPath, "User Manual\help.html#job-editor")}""")
        Else
            MsgBox("User Manual not found!", MsgBoxStyle.Critical)
        End If
    End Sub


#End Region

    'Save ("Save" or "Save As" when new file)
    Private Function Save() As Boolean
        If VectoFile = "" Then
            If JobfileFileBrowser.SaveDialog("") Then
                VectoFile = JobfileFileBrowser.Files(0)
            Else
                Return False
            End If
        End If
        Try
            Return VECTOsave(VectoFile)
        Catch ex As Exception
            MsgBox("Error when saving file" + Environment.NewLine + ex.Message)
            Return False
        End Try
    End Function

    'Open file
    Public Sub VECTOload2Form(file As String)

        If ChangeCheckCancel() Then Exit Sub

        VectoNew()

        'Read GEN
        Dim vectoJob As IEngineeringJobInputData = Nothing
        Dim inputData As IEngineeringInputDataProvider = Nothing
        Try
            inputData = TryCast(JSONInputDataFactory.ReadComponentData(file),
                                IEngineeringInputDataProvider)
            vectoJob = inputData.JobInputData()
        Catch ex As Exception
            MsgBox("Failed to read Job-File" + Environment.NewLine + ex.Message)
            Return
        End Try

        JobType = vectoJob.JobType

        If Cfg.DeclMode <> vectoJob.SavedInDeclarationMode Then
            Select Case WrongMode()
                Case 1
                    Close()
                    MainForm.RbDecl.Checked = Not MainForm.RbDecl.Checked
                    MainForm.OpenVectoFile(file)
                Case -1
                    Exit Sub
            End Select
        End If

        VectoFile = file
        _basePath = Path.GetDirectoryName(file)
        'Update Form

        If inputData.JobInputData().JobType = VectoSimulationJobType.EngineOnlySimulation Then
            TbENG.Text = GetRelativePath(inputData.JobInputData.EngineOnly.DataSource.SourceFile, _basePath)
            'CbEngOnly.Checked = True
            JobType = VectoSimulationJobType.EngineOnlySimulation
            Try
                Dim sb As ICycleData
                For Each sb In vectoJob.Cycles
                    Dim lv0 As ListViewItem = New ListViewItem
                    lv0.Text = GetRelativePath(sb.CycleData.Source, Path.GetDirectoryName(Path.GetFullPath(file))) 'sb.Name
                    LvCycles.Items.Add(lv0)
                Next
            Catch ex As Exception
            End Try
            UpdateEnabledControls()
            Exit Sub
        End If
        'CbEngOnly.Checked = False
        UpdateEnabledControls()
        'Files -----------------------------
        TbVEH.Text = GetRelativePath(inputData.JobInputData.Vehicle.DataSource.SourceFile, _basePath)
        If (JobType.HasEngine()) Then
            TbENG.Text = GetRelativePath(inputData.JobInputData.Vehicle.Components.EngineInputData.DataSource.SourceFile, _basePath)
        Else
            TbENG.Text = ""
        End If
        If (inputData.JobInputData.Vehicle.Components.GearboxInputData IsNot Nothing) Then
            TbGBX.Text = GetRelativePath(inputData.JobInputData.Vehicle.Components.GearboxInputData.DataSource.SourceFile, _basePath)
        Else
            TbGBX.Text = ""
        End If
        If (Cfg.DeclMode OrElse inputData.DriverInputData.GearshiftInputData Is Nothing) Then
            TbShiftStrategyParams.Text = ""
        Else
            TbShiftStrategyParams.Text = GetRelativePath(inputData.DriverInputData.GearshiftInputData.Source, _basePath)
        End If
        If (Not Cfg.DeclMode AndAlso (JobType = VectoSimulationJobType.ParallelHybridVehicle OrElse JobType = VectoSimulationJobType.SerialHybridVehicle _
                OrElse JobType = VectoSimulationJobType.IEPC_S OrElse JobType = VectoSimulationJobType.IHPC)) Then
            tbHybridStrategyParams.Text = GetRelativePath(inputData.JobInputData.HybridStrategyParameters.Source, _basePath)
        End If

        'Start/Stop
        Dim driver As IDriverEngineeringInputData = inputData.DriverInputData

        If (Cfg.DeclMode) Then
            TbDesMaxFile.Text = ""
            'AA-TB
            'Try and Select any previously selected Auxiliary Type
            Dim declarationInput As IDeclarationInputDataProvider = CType(inputData, IDeclarationInputDataProvider)
            Dim auxInput As IAuxiliariesDeclarationInputData = declarationInput.JobInputData.Vehicle.Components.AuxiliaryInputData

            LvAux.Items.Clear()
            Dim entry As IAuxiliaryDeclarationInputData
            For Each entry In auxInput.Auxiliaries
                'If entry.AuxiliaryType = AuxiliaryDemandType.Constant Then Continue For
                Try
                    LvAux.Items.Add(CreateAuxListEntry(AuxiliaryTypeHelper.GetAuxKey(entry.Type),
                                                        AuxiliaryTypeHelper.ToString(entry.Type), String.Join("; ", entry.Technology)))
                Catch ex As Exception
                End Try
            Next
        Else
            LvAux.Items.Clear()

            'VACC
            TbDesMaxFile.Text =
                If(driver.AccelerationCurve Is Nothing, "", GetRelativePath(driver.AccelerationCurve.AccelerationCurve.Source, _basePath))

            Dim auxInput As IAuxiliariesEngineeringInputData = inputData.JobInputData.Vehicle.Components.AuxiliaryInputData

            tbElectricAuxConstant.Text = auxInput.Auxiliaries.ElectricPowerDemand.ToGUIFormat()
            TbAuxPAuxICEOn.Text = auxInput.Auxiliaries.ConstantPowerDemand.ToGUIFormat()
            tbPAuxDrivingICEOff.Text = auxInput.Auxiliaries.PowerDemandICEOffDriving.ToGUIFormat()
            tbPAuxStandstillICEOff.Text = auxInput.Auxiliaries.PowerDemandICEOffStandstill.ToGUIFormat()
        End If

        BtnShiftStrategyParams.Enabled = Not Cfg.DeclMode
        TbShiftStrategyParams.Enabled = Not Cfg.DeclMode
        BtnShiftParamsForm.Enabled = Not Cfg.DeclMode

        Try
            Dim sb As ICycleData
            For Each sb In vectoJob.Cycles
                Dim lv0 As ListViewItem = New ListViewItem
                If (sb.CycleData.SourceType = DataSourceType.Embedded) Then
                    lv0.Text = sb.Name
                Else
                    lv0.Text = GetRelativePath(sb.CycleData.Source, Path.GetDirectoryName(Path.GetFullPath(file))) 'sb.Name
                End If
                LvCycles.Items.Add(lv0)
            Next
        Catch ex As Exception
        End Try


        If driver.OverSpeedData.Enabled Then
            RdOverspeed.Checked = True
        Else
            RdOff.Checked = True
        End If
        TbOverspeed.Text = driver.OverSpeedData.OverSpeed.AsKmph.ToGUIFormat()
        TbVmin.Text = driver.OverSpeedData.MinSpeed.AsKmph.ToGUIFormat()
        If Not driver.Lookahead Is Nothing Then
            CbLookAhead.Checked = driver.Lookahead.Enabled
            'TbAlookahead.Text = CStr(VEC0.ALookahead)
            tbLacMinSpeed.Text = driver.Lookahead.MinSpeed.AsKmph.ToGUIFormat()
            'TbVminLA.Text = CStr(VEC0.VMinLa)
            tbLacPreviewFactor.Text = driver.Lookahead.LookaheadDistanceFactor.ToGUIFormat()
            tbDfCoastingOffset.Text = driver.Lookahead.CoastingDecisionFactorOffset.ToGUIFormat()
            tbDfCoastingScale.Text = driver.Lookahead.CoastingDecisionFactorScaling.ToGUIFormat()

            tbLacDfTargetSpeedFile.Text = If(driver.Lookahead.CoastingDecisionFactorTargetSpeedLookup Is Nothing, "",
                                            GetRelativePath(driver.Lookahead.CoastingDecisionFactorTargetSpeedLookup.Source, _basePath))
            tbLacDfVelocityDropFile.Text = If(driver.Lookahead.CoastingDecisionFactorVelocityDropLookup Is Nothing, "",
                                            GetRelativePath(driver.Lookahead.CoastingDecisionFactorVelocityDropLookup.Source, _basePath))
        End If

        tbEngineStopStartActivationDelay.Text = If(driver.EngineStopStartData?.ActivationDelay?.ToGUIFormat(), DeclarationData.Driver.GetEngineStopStartLorry().ActivationDelay.ToGUIFormat())
        tbMaxEngineOffTimespan.Text = If(driver.EngineStopStartData?.MaxEngineOffTimespan?.ToGUIFormat(), DeclarationData.Driver.GetEngineStopStartLorry().MaxEngineOffTimespan.ToGUIFormat())
        tbEssUtility.Text = If(driver.EngineStopStartData?.UtilityFactorStandstill.ToGUIFormat(), DeclarationData.Driver.GetEngineStopStartLorry().UtilityFactor.ToGUIFormat())
        tbESSUtilityFactorDriving.Text = If(driver.EngineStopStartData?.UtilityFactorDriving.ToGUIFormat(), DeclarationData.Driver.GetEngineStopStartLorry().UtilityFactor.ToGUIFormat())

        tbEcoRollActivationDelay.Text = If(driver.EcoRollData?.ActivationDelay?.ToGUIFormat(), DeclarationData.Driver.EcoRoll.ActivationDelay.ToGUIFormat())
        tbEcoRollMinSpeed.Text = If(driver.EcoRollData?.MinSpeed?.AsKmph().ToGUIFormat(), DeclarationData.Driver.EcoRoll.MinSpeed.AsKmph().ToGUIFormat())
        tbEcoRollUnderspeed.Text = If(driver.EcoRollData?.UnderspeedThreshold?.AsKmph().ToGUIFormat(), DeclarationData.Driver.EcoRoll.UnderspeedThreshold.AsKmph().ToGUIFormat())
        tbEcoRollMaxAcc.Text = If(driver.EcoRollData?.AccelerationUpperLimit?.ToGUIFormat(), DeclarationData.Driver.EcoRoll.AccelerationUpperLimit.ToGUIFormat())

        tbPCCUnderspeed.Text = If(driver.PCCData?.Underspeed?.AsKmph().ToGUIFormat(), DeclarationData.Driver.PCC.Underspeed.AsKmph().ToGUIFormat())
        tbPCCOverspeed.Text = If(driver.PCCData?.OverspeedUseCase3?.AsKmph().ToGUIFormat(), DeclarationData.Driver.PCC.OverspeedUseCase3.AsKmph().ToGUIFormat())
        tbPCCEnableSpeed.Text = If(driver.PCCData?.PCCEnabledSpeed?.AsKmph().ToGUIFormat(), DeclarationData.Driver.PCC.PCCEnableSpeed.AsKmph().ToGUIFormat())
        tbPCCMinSpeed.Text = If(driver.PCCData?.MinSpeed?.AsKmph().ToGUIFormat(), DeclarationData.Driver.PCC.MinSpeed.AsKmph().ToGUIFormat())
        tbPCCPreviewUseCase1.Text = If(driver.PCCData?.PreviewDistanceUseCase1?.ToGUIFormat(), DeclarationData.Driver.PCC.PreviewDistanceUseCase1.ToGUIFormat())
        tbPCCPreviewUseCase2.Text = If(driver.PCCData?.PreviewDistanceUseCase2?.ToGUIFormat(), DeclarationData.Driver.PCC.PreviewDistanceUseCase2.ToGUIFormat())

        '-------------------------------------------------------------

        If (Not inputData.JobInputData.Vehicle.Components.AuxiliaryInputData.BusAuxiliariesData Is Nothing) Then
            cbEnableBusAux.Checked = True
            tbBusAuxParams.Text = GetRelativePath(inputData.JobInputData.Vehicle.Components.AuxiliaryInputData.BusAuxiliariesData.DataSource.SourceFile, _basePath)
        Else
            cbEnableBusAux.Checked = False
            tbBusAuxParams.Text = ""
        End If

        Try
            DeclInit()
        Catch ex As VectoException
            MsgBox(ex.Message, MsgBoxStyle.OkOnly, "Error loading Vecto job")
            Exit Sub
        End Try

        EngineForm.AutoSendTo = False
        GearboxForm.AutoSendTo = False
        VehicleForm.AutoSendTo = False


        Dim x As Integer = Len(file)
        While Mid(file, x, 1) <> "\" And x > 0
            x = x - 1
        End While
        Text = Mid(file, x + 1, Len(file) - x)
        _changed = False
        ToolStripStatusLabelGEN.Text = ""   'file & " opened."

        UpdatePic()

        '-------------------------------------------------------------
    End Sub


    Private Function CreateAuxListEntry(auxKey As String, type As String, technology As String) As ListViewItem
        Dim lv0 As ListViewItem = New ListViewItem
        lv0.SubItems(AuxViewColumns.AuxID).Text = auxKey
        lv0.SubItems.Add(type)
        lv0.SubItems.Add(technology)
        Return lv0
    End Function


    'Save file
    Private Function VECTOsave(file As String) As Boolean
        Dim message As String = String.Empty

        Dim vectoJob As VectoJob = New VectoJob
        vectoJob.JobType = JobType
        vectoJob.FilePath = file

        'Files ------------------------------------------------- -----------------

        vectoJob.PathVeh = TbVEH.Text
        vectoJob.PathEng = TbENG.Text

        For Each lv0 As ListViewItem In LvCycles.Items
            Dim sb As SubPath = New SubPath
            sb.Init(GetPath(file), lv0.Text)
            vectoJob.CycleFiles.Add(sb)
        Next

        vectoJob.PathGbx = TbGBX.Text
        vectoJob.PathShiftParams = TbShiftStrategyParams.Text
        vectoJob.PathHybridStrategyParams = tbHybridStrategyParams.Text
        'a_DesMax
        vectoJob.DesMaxFile = TbDesMaxFile.Text

        vectoJob.AuxEntries.Clear()
        For Each lv0 As ListViewItem In LvAux.Items
            Dim auxEntry As VectoJob.AuxEntry = New VectoJob.AuxEntry

            auxEntry.TechnologyList.Clear()
            auxEntry.TechnologyList.AddRange(
                lv0.SubItems(AuxViewColumns.AuxInputOrTech).Text.Split(";"c).Select(
                    Function(x) Trim(x)))

            auxEntry.Type = AuxiliaryTypeHelper.ParseKey(lv0.SubItems(AuxViewColumns.AuxID).Text)
            vectoJob.AuxEntries(AuxiliaryTypeHelper.GetAuxKey(auxEntry.Type)) = auxEntry
        Next
        vectoJob.AuxPwrICEOn = TbAuxPAuxICEOn.Text.ToDouble(0)
        vectoJob.AuxPwrDrivingICEOff = tbPAuxDrivingICEOff.Text.ToDouble(0)
        vectoJob.AuxPwrStandstillICEOff = _tbPAuxStandstillICEOff.Text.ToDouble(0)

        vectoJob.AuxElPadd = tbElectricAuxConstant.Text.ToDouble(0)

        If cbEnableBusAux.Checked AndAlso Not String.IsNullOrWhiteSpace(tbBusAuxParams.Text) Then
            vectoJob.UseBusAux = True
            vectoJob.PathBusAux = tbBusAuxParams.Text
        Else
            vectoJob.UseBusAux = False
        End If



        'vectoJob.EngineOnly = JobType = VectoSimulationJobType.EngineOnlySimulation

        vectoJob.OverSpeedOn = RdOverspeed.Checked
        vectoJob.OverSpeed = TbOverspeed.Text.ToDouble(0)
        vectoJob.VMin = TbVmin.Text.ToDouble(0)
        vectoJob.LookAheadOn = CbLookAhead.Checked
        'vec0.ALookahead = CSng(fTextboxToNumString(TbAlookahead.Text))
        'vec0.VMinLa = CSng(fTextboxToNumString(TbVminLA.Text))
        vectoJob.LookAheadMinSpeed = tbLacMinSpeed.Text.ToDouble(0)
        vectoJob.LacPreviewFactor = tbLacPreviewFactor.Text.ToDouble(0)
        vectoJob.LacDfOffset = tbDfCoastingOffset.Text.ToDouble(0)
        vectoJob.LacDfScale = tbDfCoastingScale.Text.ToDouble(0)
        vectoJob.LacDfTargetSpeedFile = tbLacDfTargetSpeedFile.Text
        vectoJob.LacDfVelocityDropFile = tbLacDfVelocityDropFile.Text

        vectoJob.EngineStopStartActivationThreshold = tbEngineStopStartActivationDelay.Text.ToDouble(0)
        vectoJob.EngineOffTimeLimit = tbMaxEngineOffTimespan.Text.ToDouble(0)
        vectoJob.EngineStStUtilityFactor = tbEssUtility.Text.ToDouble(0)
        vectoJob.EngineStStUtilityFactorDriving = tbESSUtilityFactorDriving.Text.ToDouble(0)

        vectoJob.EcoRollActivationDelay = tbEcoRollActivationDelay.Text.ToDouble(0)
        vectoJob.EcoRollMinSpeed = tbEcoRollMinSpeed.Text.ToDouble(0)
        vectoJob.EcoRollUnderspeedThreshold = tbEcoRollUnderspeed.Text.ToDouble(0)
        vectoJob.EcoRollMaxAcceleration = tbEcoRollMaxAcc.Text.ToDouble(0)

        vectoJob.PCCEnableSpeedVal = tbPCCEnableSpeed.Text.ToDouble(0)
        vectoJob.PCCMinSpeed = tbPCCMinSpeed.Text.ToDouble(0)
        vectoJob.PCCUnderspeed = tbPCCUnderspeed.Text.ToDouble(0)
        vectoJob.PCCOverspeedUseCase3 = tbPCCOverspeed.Text.ToDouble(0)
        vectoJob.PCCPrevewiDistance1 = tbPCCPreviewUseCase1.Text.ToDouble(0)
        vectoJob.PCCPreviewDistance2 = tbPCCPreviewUseCase2.Text.ToDouble(0)

        '------------------------------------------------------------

        'SAVE
        If Not vectoJob.SaveFile Then
            MsgBox("Cannot save to " & file, MsgBoxStyle.Critical)
            Return False
        End If

        VectoFile = file

        file = GetFilenameWithoutPath(VectoFile, True)

        Text = file
        ToolStripStatusLabelGEN.Text = ""

        MainForm.AddToJobListView(VectoFile)

        _changed = False

        Return True
    End Function

    'New file
    Public Sub VectoNew()

        If ChangeCheckCancel() Then Exit Sub

        'Files
        TbVEH.Text = ""
        TbENG.Text = ""
        LvCycles.Items.Clear()
        TbGBX.Text = ""
        TbDesMaxFile.Text = ""

        LvAux.Items.Clear()

        'CbEngOnly.Checked = False

        'RdOff.Checked = True
        RdOverspeed.Checked = True
        CbLookAhead.Checked = True
        'TbAlookahead.Text = "-0.5"
        TbOverspeed.Text = DeclarationData.Driver.OverSpeed.AllowedOverSpeed.AsKmph.ToGUIFormat()
        TbVmin.Text = DeclarationData.Driver.OverSpeed.MinSpeed.AsKmph.ToGUIFormat()

        'TbVminLA.Text = "50"
        tbLacMinSpeed.Text = DeclarationData.Driver.LookAhead.MinimumSpeed.AsKmph.ToGUIFormat()
        tbLacPreviewFactor.Text = DeclarationData.Driver.LookAhead.LookAheadDistanceFactor.ToGUIFormat()
        tbDfCoastingOffset.Text = DeclarationData.Driver.LookAhead.DecisionFactorCoastingOffset.ToGUIFormat()
        tbDfCoastingScale.Text = DeclarationData.Driver.LookAhead.DecisionFactorCoastingScaling.ToGUIFormat()
        tbLacDfTargetSpeedFile.Text = ""
        tbLacDfVelocityDropFile.Text = ""

        '---------------------------------------------------
        DeclInit()

        EngineForm.AutoSendTo = False

        VectoFile = ""
        Text = "Job Editor"
        ToolStripStatusLabelGEN.Text = ""
        _changed = False
        UpdatePic()
    End Sub


#Region "Track changes"

#Region "'Change' Events"

    Private Sub TextBoxVEH_TextChanged(sender As Object, e As EventArgs) _
        Handles TbVEH.TextChanged
        UpdatePic()
        Change()
    End Sub

    Private Sub TextBoxMAP_TextChanged(sender As Object, e As EventArgs) _
        Handles TbENG.TextChanged
        UpdatePic()
        Change()
    End Sub

    Private Sub TextBoxFLD_TextChanged(sender As Object, e As EventArgs) _
        Handles TbGBX.TextChanged
        UpdatePic()
        Change()
    End Sub

    Private Sub TbDesMaxFile_TextChanged_1(sender As Object, e As EventArgs) Handles TbDesMaxFile.TextChanged
        Change()
    End Sub


    Private Sub TBSSspeed_TextChanged(sender As Object, e As EventArgs)
        Change()
    End Sub

    Private Sub TBSStime_TextChanged(sender As Object, e As EventArgs)

        Change()
    End Sub

    Private Sub TbOverspeed_TextChanged(sender As Object, e As EventArgs) Handles TbOverspeed.TextChanged
        Change()
    End Sub

    Private Sub TbUnderSpeed_TextChanged(sender As Object, e As EventArgs)
        Change()
    End Sub

    Private Sub TbVmin_TextChanged(sender As Object, e As EventArgs) _
        Handles TbVmin.TextChanged
        Change()
    End Sub

    Private Sub LvCycles_AfterLabelEdit(sender As Object, e As LabelEditEventArgs) _
        Handles LvCycles.AfterLabelEdit
        Change()
    End Sub


#End Region

    Private Sub Change()
        If Not _changed Then
            ToolStripStatusLabelGEN.Text = "Unsaved changes in current file"
            _changed = True
        End If
    End Sub

    ' "Save changes? "... Returns True if User aborts
    Private Function ChangeCheckCancel() As Boolean

        If _changed Then

            Select Case MsgBox("Save changes ?", MsgBoxStyle.YesNoCancel)
                Case MsgBoxResult.Yes
                    Return Not Save()
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

#Region "Aux Listview"

    Private Sub LvAux_DoubleClick(sender As Object, e As EventArgs) Handles LvAux.DoubleClick
        EditAuxItem()
    End Sub

    Private Sub LvAux_KeyDown(sender As Object, e As KeyEventArgs) Handles LvAux.KeyDown
        Select Case e.KeyCode
            Case Keys.Enter
                EditAuxItem()
        End Select
    End Sub

    Private Sub EditAuxItem()
        If LvAux.SelectedItems.Count = 0 Then
            Exit Sub
        End If

        Dim selItem As ListViewItem = LvAux.SelectedItems(0)
        _auxDialog.VehPath = GetPath(VectoFile)
        '_auxDialog.CbType.SelectedIndex = -1

        If selItem.SubItems(AuxViewColumns.AuxID).Text <> AuxiliaryTypeHelper.GetAuxKey(AuxiliaryType.SteeringPump) Then
            _auxDialog.NumAxles = 0
        Else
            _auxDialog.NumAxles =
                If(String.IsNullOrWhiteSpace(TbAxleConf.Text), 1, AxleConfigurationHelper.Parse(TbAxleConf.Text).NumAxles())

        End If

        _auxDialog.JobType = JobType
        ' clear and set selected item to force re-init of tech dropdown (apply filter for electric-only aux)
        _auxDialog.CbType.SelectedIndex = -1
        _auxDialog.CbType.SelectedValue = selItem.SubItems(AuxViewColumns.AuxID).Text   ' last call, updates GUI

        If selItem.SubItems(AuxViewColumns.AuxID).Text = AuxiliaryTypeHelper.GetAuxKey(AuxiliaryType.SteeringPump) Then
            Dim parts As String() = selItem.SubItems(AuxViewColumns.AuxInputOrTech).Text.Split(";"c)
            _auxDialog.CbTech2.SelectedItem = VehicleAuxiliariesDialog.AxleNotSteered
            _auxDialog.CbTech3.SelectedItem = VehicleAuxiliariesDialog.AxleNotSteered
            _auxDialog.CbTech4.SelectedItem = VehicleAuxiliariesDialog.AxleNotSteered
            If parts.Length > 0 Then _auxDialog.CbTech.SelectedValue = Trim(parts(0))
            If parts.Length > 1 Then _auxDialog.CbTech2.SelectedValue = Trim(parts(1))
            If parts.Length > 2 Then _auxDialog.CbTech3.SelectedValue = Trim(parts(2))
            If parts.Length > 3 Then _auxDialog.CbTech4.SelectedValue = Trim(parts(3))
        Else
            _auxDialog.CbTech.SelectedValue = selItem.SubItems(AuxViewColumns.AuxInputOrTech).Text
        End If

        '_auxDialog.TbID.Text = selItem.SubItems(AuxViewColumns.AuxID).Text	

        If _auxDialog.ShowDialog = DialogResult.OK Then
            selItem.SubItems(AuxViewColumns.AuxID).Text = _auxDialog.CbType.SelectedValue.ToString() _
            'UCase(Trim(_auxDialog.TbID.Text))
            selItem.SubItems(AuxViewColumns.AuxType).Text = _auxDialog.CbType.Text

            If _auxDialog.TbID.Text = AuxiliaryTypeHelper.GetAuxKey(AuxiliaryType.SteeringPump) Then
                Dim techlist As List(Of String) = New List(Of String)
                techlist.Add(_auxDialog.CbTech.Text)
                If _auxDialog.CbTech2.Text <> VehicleAuxiliariesDialog.AxleNotSteered Then techlist.Add(_auxDialog.CbTech2.Text)
                If _auxDialog.CbTech3.Text <> VehicleAuxiliariesDialog.AxleNotSteered Then techlist.Add(_auxDialog.CbTech3.Text)
                If _auxDialog.CbTech4.Text <> VehicleAuxiliariesDialog.AxleNotSteered Then techlist.Add(_auxDialog.CbTech4.Text)
                selItem.SubItems(AuxViewColumns.AuxInputOrTech).Text = String.Join("; ", techlist)
            Else
                selItem.SubItems(AuxViewColumns.AuxInputOrTech).Text = Trim(_auxDialog.CbTech.Text)
            End If

            Change()
        End If
    End Sub

#End Region

    'OK (Save & Close)
    Private Sub ButSave_Click(sender As Object, e As EventArgs) Handles ButOK.Click
        If Not Save() Then Exit Sub
        Close()
    End Sub

    'Cancel
    Private Sub ButCancel_Click(sender As Object, e As EventArgs) Handles ButCancel.Click
        Close()
    End Sub

#Region "Cycle list"

    Private Sub LvCycles_KeyDown(sender As Object, e As KeyEventArgs) Handles LvCycles.KeyDown
        Select Case e.KeyCode
            Case Keys.Delete, Keys.Back
                RemoveCycle()
            Case Keys.Enter
                If LvCycles.SelectedItems.Count > 0 Then LvCycles.SelectedItems(0).BeginEdit()
        End Select
    End Sub

    Private Sub BtDRIadd_Click(sender As Object, e As EventArgs) Handles BtDRIadd.Click
        Dim genDir As String = GetPath(VectoFile)

        If DrivingCycleFileBrowser.OpenDialog("", True) Then
            Dim s As String
            For Each s In DrivingCycleFileBrowser.Files
                LvCycles.Items.Add(GetFilenameWithoutDirectory(s, genDir))
            Next
            Change()
        End If
    End Sub

    Private Sub BtDRIrem_Click(sender As Object, e As EventArgs) Handles BtDRIrem.Click
        RemoveCycle()
    End Sub

    Private Sub RemoveCycle()
        Dim i As Integer

        If LvCycles.SelectedItems.Count = 0 Then
            If LvCycles.Items.Count = 0 Then
                Exit Sub
            Else
                LvCycles.Items(LvCycles.Items.Count - 1).Selected = True
            End If
        End If

        i = LvCycles.SelectedItems(0).Index

        LvCycles.SelectedItems(0).Remove()

        If LvCycles.Items.Count > 0 Then
            If i < LvCycles.Items.Count Then
                LvCycles.Items(i).Selected = True
            Else
                LvCycles.Items(LvCycles.Items.Count - 1).Selected = True
            End If

            LvCycles.Focus()
        End If

        Change()
    End Sub

#End Region

#Region "Enable/Disable GUI controls"

    'Engine only mode changed
    'Private Sub CbEngOnly_CheckedChanged(sender As Object, e As EventArgs) Handles CbEngOnly.CheckedChanged
    '	CheckEngOnly()
    '	Change()
    'End Sub

    Private Sub UpdateEnabledControls()

        SetFormHeader()

        Dim onOff As Boolean

        onOff = Not JobType = VectoSimulationJobType.EngineOnlySimulation

        SetDrivertab(onOff)

        pnVehicle.Enabled = True
        pnGearbox.Enabled = True
        pnShiftParams.Enabled = Not Cfg.DeclMode
        TabPgADAS.Enabled = True
        tpAuxiliaries.Enabled = True
        gbElectricAux.Enabled = True
        GrAuxMech.Enabled = True
        pnEngine.Enabled = True
        pnHybridStrategy.Enabled = Not Cfg.DeclMode
        gbEngineStopStart.Enabled = Not Cfg.DeclMode
        lblESSUtilityFactorDriving.Visible = True
        tbESSUtilityFactorDriving.Visible = True
        lblESSUtilityFactorDrivingUnit.Visible = True
        Select Case JobType
            Case VectoSimulationJobType.ConventionalVehicle
                gbElectricAux.Enabled = False
            Case VectoSimulationJobType.EngineOnlySimulation
                pnVehicle.Enabled = False
                pnGearbox.Enabled = False
                pnShiftParams.Enabled = False
                TabPgADAS.Enabled = False
                tpAuxiliaries.Enabled = False
                pnShiftParams.Enabled = False
                gbEngineStopStart.Enabled = False
            Case VectoSimulationJobType.ParallelHybridVehicle
                pnHybridStrategy.Enabled = Not Cfg.DeclMode
                lblESSUtilityFactorDriving.Visible = False
                tbESSUtilityFactorDriving.Visible = False
                lblESSUtilityFactorDrivingUnit.Visible = False
            Case VectoSimulationJobType.SerialHybridVehicle
                pnHybridStrategy.Enabled = Not Cfg.DeclMode
                gbEngineStopStart.Enabled = False
            Case VectoSimulationJobType.BatteryElectricVehicle, VectoSimulationJobType.FCHV
                pnEngine.Enabled = False
                pnGearbox.Enabled = True
                GrAuxMech.Enabled = Cfg.DeclMode
                pnShiftParams.Enabled = Not Cfg.DeclMode
                gbEngineStopStart.Enabled = False
                pnHybridStrategy.Enabled = False
            Case VectoSimulationJobType.IHPC
                pnEngine.Enabled = True
                pnGearbox.Enabled = True
                GrAuxMech.Enabled = True
                pnShiftParams.Enabled = Not Cfg.DeclMode
                gbEngineStopStart.Enabled = False
                pnHybridStrategy.Enabled = Not Cfg.DeclMode
            Case VectoSimulationJobType.IEPC_E, VectoSimulationJobType.FCHV_IEPC
                pnEngine.Enabled = False
                pnGearbox.Enabled = True
                GrAuxMech.Enabled = Cfg.DeclMode
                pnShiftParams.Enabled = not Cfg.DeclMode
                gbEngineStopStart.Enabled = False
            Case VectoSimulationJobType.IEPC_S
                pnEngine.Enabled = True
                pnGearbox.Enabled = True
                GrAuxMech.Enabled = Cfg.DeclMode
                pnShiftParams.Enabled = not Cfg.DeclMode
                gbEngineStopStart.Enabled = False
                pnHybridStrategy.Enabled = not Cfg.DeclMode
   
        End Select
    End Sub

    'LAC changed
    Private Sub CbLookAhead_CheckedChanged(sender As Object, e As EventArgs) _
		Handles CbLookAhead.CheckedChanged
		Change()
		pnLookAheadCoasting.Enabled = CbLookAhead.Checked
	End Sub

	'EcoRoll / Overspeed changed
	Private Sub RdOff_CheckedChanged(sender As Object, e As EventArgs) _
		Handles RdOff.CheckedChanged, RdOverspeed.CheckedChanged
		Dim ecoRoll As Boolean
		Dim overspeed As Boolean

		Change()

		ecoRoll = False	'RdEcoRoll.Checked
		overspeed = RdOverspeed.Checked

		TbOverspeed.Enabled = overspeed Or ecoRoll
		Label13.Enabled = overspeed Or ecoRoll
		Label14.Enabled = overspeed Or ecoRoll

		TbVmin.Enabled = overspeed Or ecoRoll
		Label23.Enabled = overspeed Or ecoRoll
		Label21.Enabled = overspeed Or ecoRoll
	End Sub

#End Region

	Public Sub UpdatePic()


		TbHVCclass.Text = ""
		TbVehCat.Text = ""
		TbMass.Text = ""
		TbAxleConf.Text = ""
		TbEngTxt.Text = ""
		TbGbxTxt.Text = ""
		PicVehicle.Image = Nothing
		PicBox.Image = Nothing

		Try
			UpdateVehiclePic()

			Dim chart As Chart = Nothing
			UpdateEnginePic(chart)


			UpdateGearboxPic(chart)

			If chart Is Nothing Then Return

			Dim chartArea As ChartArea = New ChartArea()
			chartArea.Name = "main"

			chartArea.AxisX.Title = "engine speed [1/min]"
			chartArea.AxisX.TitleFont = New Font("Helvetica", 10)
			chartArea.AxisX.LabelStyle.Font = New Font("Helvetica", 8)
			chartArea.AxisX.LabelAutoFitStyle = LabelAutoFitStyles.None
			chartArea.AxisX.MajorGrid.LineDashStyle = ChartDashStyle.Dot

			chartArea.AxisY.Title = "engine torque [Nm]"
			chartArea.AxisY.TitleFont = New Font("Helvetica", 10)
			chartArea.AxisY.LabelStyle.Font = New Font("Helvetica", 8)
			chartArea.AxisY.LabelAutoFitStyle = LabelAutoFitStyles.None
			chartArea.AxisY.MajorGrid.LineDashStyle = ChartDashStyle.Dot

			chartArea.AxisX.Minimum = 300
			chartArea.BorderDashStyle = ChartDashStyle.Solid
			chartArea.BorderWidth = 1

			chartArea.BackColor = Color.GhostWhite

			chart.ChartAreas.Add(chartArea)
			chart.Update()

			Dim img As Bitmap = New Bitmap(chart.Width, chart.Height, PixelFormat.Format32bppArgb)
			chart.DrawToBitmap(img, New Rectangle(0, 0, PicBox.Width, PicBox.Height))

			PicBox.Image = img
		Catch
		End Try
	End Sub

	Private Sub UpdateGearboxPic(ByRef chartArea As Chart)
		Dim s As Series
		Dim i As Integer

		Dim gearbox As IGearboxEngineeringInputData = Nothing
		Dim gearboxFile As String =
				If(Not String.IsNullOrWhiteSpace(VectoFile), Path.Combine(Path.GetDirectoryName(VectoFile), TbGBX.Text), TbGBX.Text)
		If File.Exists(gearboxFile) Then
			Try
				Dim inputData As IEngineeringInputDataProvider = TryCast(JSONInputDataFactory.ReadComponentData(gearboxFile), 
																		IEngineeringInputDataProvider)
				gearbox = inputData.JobInputData.Vehicle.Components.GearboxInputData
			Catch
			End Try
		End If

		If gearbox Is Nothing Then Return

        if (JobType = VectoSimulationJobType.IEPC_E OrElse  JobType = VectoSimulationJobType.IEPC_S) Then
            TbGbxTxt.Text = $"IEPC {gearbox.Model}"
        else
            TbGbxTxt.Text = $"{gearbox.Gears.Count}-Speed {gearbox.Type.ShortName()} {gearbox.Model}"
        End If

	    If Cfg.DeclMode Then
			For i = 1 To gearbox.Gears.Count
				'If FLD0.Init(ENG0.Nidle) Then '' use engine from below...

				'Dim engine As CombustionEngineData = ConvertToEngineData(FLD0, F_VECTO.n_idle)
				'Dim shiftLines As ShiftPolygon = DeclarationData.Gearbox.ComputeShiftPolygon(Gear - 1,
				'																			engine.FullLoadCurve, gears,
				'																			engine,
				'																			Double.Parse(LvGears.Items(0).SubItems(F_GBX.GearboxTbl.Ratio).Text,
				'																						CultureInfo.InvariantCulture),
				'																			(.rdyn / 1000.0).SI(Of Meter))

				's = New Series
				's.Points.DataBindXY(shiftLines.Upshift.Select(Function(pt) pt.AngularSpeed.Value() / Constants.RPMToRad).ToList(),
				'					shiftLines.Upshift.Select(Function(pt) pt.Torque.Value()).ToList())
				's.ChartType = SeriesChartType.FastLine
				's.BorderWidth = 2
				's.Color = Color.DarkRed
				's.Name = "Upshift curve (" & i & ")"
				'MyChart.Series.Add(s)

				's = New Series
				's.Points.DataBindXY(
				'	shiftLines.Downshift.Select(Function(pt) pt.AngularSpeed.Value() / Constants.RPMToRad).ToList(),
				'	shiftLines.Downshift.Select(Function(pt) pt.Torque.Value()).ToList())
				's.ChartType = SeriesChartType.FastLine
				's.BorderWidth = 2
				's.Color = Color.DarkRed
				's.Name = "Downshift curve (" & i & ")"
				'MyChart.Series.Add(s)
				'End If

				'	OkCount += 1

				'	pmax = FLD0.Pfull(FLD0.EngineRatedSpeed)

				'End If
			Next
		Else
			For Each gear As ITransmissionInputData In gearbox.Gears
				If gear.ShiftPolygon Is Nothing OrElse gear.ShiftPolygon.Rows.Count = 0 Then Continue For
				Dim shiftPolygon As ShiftPolygon = ShiftPolygonReader.Create(gear.ShiftPolygon)
				s = New Series
				s.Points.DataBindXY(shiftPolygon.Upshift.Select(Function(x) x.AngularSpeed.AsRPM).ToArray(),
									shiftPolygon.Upshift.Select(Function(x) x.Torque.Value()).ToArray())
				s.ChartType = SeriesChartType.FastLine
				s.BorderWidth = 2
				s.Color = Color.DarkRed
				s.Name = "Upshift curve"
				' MyChart.Series.Add(s) 'MQ 2016-06-20: do not plot shift lines in engine dialog

				s = New Series
				s.Points.DataBindXY(shiftPolygon.Downshift.Select(Function(x) x.AngularSpeed.AsRPM).ToArray(),
									shiftPolygon.Downshift.Select(Function(x) x.Torque.Value()).ToArray())
				s.ChartType = SeriesChartType.FastLine
				s.BorderWidth = 2
				s.Color = Color.DarkRed
				s.Name = "Downshift curve"
				'MyChart.Series.Add(s) 'MQ 2016-06-20:do not plot shift lines in engine dialog
			Next
		End If
	End Sub

	Private Sub UpdateEnginePic(ByRef chart As Chart)
		Dim s As Series
		Dim pmax As Double

		Dim engine As IEngineEngineeringInputData = Nothing
		lblEngineCharacteristics.Text = ""
		Dim engineFile As String =
				If(Not String.IsNullOrWhiteSpace(VectoFile), Path.Combine(Path.GetDirectoryName(VectoFile), TbENG.Text), TbENG.Text)
		If File.Exists(engineFile) Then
			Try
				Dim inputData As IEngineeringInputDataProvider = TryCast(JSONInputDataFactory.ReadComponentData(engineFile), 
																		IEngineeringInputDataProvider)
				engine = inputData.JobInputData.Vehicle.Components.EngineInputData
			Catch
				Return
			End Try
		End If

		'engine.FilePath = fFileRepl(TbENG.Text, GetPath(VECTOfile))

		'Create plot
		chart = New Chart
		chart.Width = PicBox.Width
		chart.Height = PicBox.Height


		'Dim FLD0 As EngineFullLoadCurve = New EngineFullLoadCurve

		If engine Is Nothing Then Return


		'engine.IdleSpeed.Value()

		Dim fullLoadCurve As EngineFullLoadCurve = FullLoadCurveReader.Create(engine.EngineModes.First().FullLoadCurve)

		s = New Series
		s.Points.DataBindXY(fullLoadCurve.FullLoadEntries.Select(Function(x) x.EngineSpeed.AsRPM).ToArray(),
							fullLoadCurve.FullLoadEntries.Select(Function(x) x.TorqueFullLoad.Value()).ToArray())
		s.ChartType = SeriesChartType.FastLine
		s.BorderWidth = 2
		s.Color = Color.DarkBlue
		s.Name = "Full load"
		chart.Series.Add(s)

		s = New Series
		s.Points.DataBindXY(fullLoadCurve.FullLoadEntries.Select(Function(x) x.EngineSpeed.AsRPM).ToArray(),
							fullLoadCurve.FullLoadEntries.Select(Function(x) x.TorqueDrag.Value()).ToArray())
		s.ChartType = SeriesChartType.FastLine
		s.BorderWidth = 2
		s.Color = Color.Blue
		s.Name = "Motoring"
		chart.Series.Add(s)

		pmax = fullLoadCurve.MaxPower.Value() / 1000 'FLD0.Pfull(FLD0.EngineRatedSpeed)


		TbEngTxt.Text = $"{(engine.Displacement.Value()*1000).ToString("0.0")} l {pmax.ToString("#")} kw {engine.Model}"

	    Dim fuelConsumptionMap As FuelConsumptionMap = FuelConsumptionMapReader.Create(engine.EngineModes.First().Fuels.First().FuelConsumptionMap)

		s = New Series
		s.Points.DataBindXY(fuelConsumptionMap.Entries.Select(Function(x) x.EngineSpeed.AsRPM).ToArray(),
							fuelConsumptionMap.Entries.Select(Function(x) x.Torque.Value()).ToArray())
		s.ChartType = SeriesChartType.Point
		s.MarkerSize = 3
		s.Color = Color.Red
		s.Name = "Map"
		

		If (engine.EngineModes.First().Fuels.Count > 1) then
			Dim fcMap2 As FuelConsumptionMap = FuelConsumptionMapReader.Create(engine.EngineModes.First().Fuels(1).FuelConsumptionMap)

			Dim s2 As Series = New Series
			s2.Points.DataBindXY(fcMap2.Entries.Select(Function(x) x.EngineSpeed.AsRPM).ToArray(),
								fcMap2.Entries.Select(Function(x) x.Torque.Value()).ToArray())
			s2.ChartType = SeriesChartType.Point
			s2.MarkerSize = 3
			s2.Color = Color.Green
			s2.Name = "Map 2"
			chart.Series.Add(s2)
		End If

		chart.Series.Add(s)

		Dim engineCharacteristics As String =
                $"Max. Torque: {fullLoadCurve.MaxTorque.Value():F0} Nm; Max. Power: { _
                (fullLoadCurve.MaxPower.Value()/1000):F1} kW; n_rated: {fullLoadCurve.RatedSpeed.AsRPM:F0} rpm; n_95h: { _
                fullLoadCurve.N95hSpeed.AsRPM:F0} rpm"
	    lblEngineCharacteristics.Text = engineCharacteristics
	End Sub

	Private Sub UpdateVehiclePic()
		Dim HDVclass As VehicleClass = VehicleClass.Unknown

		Dim vehicle As IVehicleEngineeringInputData = Nothing

		Dim vehicleFile As String =
				If(Not String.IsNullOrWhiteSpace(VectoFile), Path.Combine(Path.GetDirectoryName(VectoFile), TbVEH.Text), TbVEH.Text)
		If File.Exists(vehicleFile) Then
			Try
				Dim inputData As IEngineeringInputDataProvider = TryCast(JSONInputDataFactory.ReadComponentData(vehicleFile), 
																		IEngineeringInputDataProvider)
				vehicle = inputData.JobInputData.Vehicle
			Catch
			End Try
		End If

		If vehicle Is Nothing Then Return

		Dim maxMass As Kilogram = vehicle.GrossVehicleMassRating					'CSng(fTextboxToNumString(TbMassMass.Text))

		Dim s0 As Segment = Nothing
		Try
			s0 = DeclarationData.TruckSegments.Lookup(vehicle.VehicleCategory, vehicle.AxleConfiguration, maxMass, 0.SI(Of Kilogram),
												False)
		Catch
		End Try
		If s0.Found Then
			HDVclass = s0.VehicleClass

			If Cfg.DeclMode Then
				LvCycles.Items.Clear()
				Dim m0 As Mission
				For Each m0 In s0.Missions
					LvCycles.Items.Add(m0.MissionType.ToString())
				Next
			End If

		End If

		PicVehicle.Image = ConvPicPath(HDVclass, False) _
        'Image.FromFile(cDeclaration.ConvPicPath(HDVclass, False))

        TbHVCclass.Text = $"{HDVclass}"
        TbVehCat.Text = vehicle.VehicleCategory.GetCategoryName()	'ConvVehCat(VEH0.VehCat, True)
		TbMass.Text = (vehicle.GrossVehicleMassRating.Value() / 1000) & " t"
		TbAxleConf.Text = vehicle.AxleConfiguration.GetName()	'ConvAxleConf(VEH0.AxleConf)
	End Sub


#Region "Open File Context Menu"

	Private _contextMenuFiles As String()
	Private _basePath As String = ""

	Private Sub OpenFiles(ParamArray files() As String)
		If files.Length = 0 Then Exit Sub

		_contextMenuFiles = files
		OpenWithToolStripMenuItem.Text = "Open with " & Cfg.OpenCmdName
		CmOpenFile.Show(System.Windows.Forms.Cursor.Position)
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


#Region "TextBox Validation OnLeave"

    Private Function TextboxValidation(text As String) As Boolean
        If text = "" Then
            Return True
        End If
        If Not IsNumeric(text) Then
            Return False
        End If

        If Not 0 < Convert.ToDouble(text) Then
            Return False
        End If

        Return True
    End Function


    Private Sub tbEngineStopStartActivationDelay_Leave(sender As Object, e As System.EventArgs) Handles tbEngineStopStartActivationDelay.Leave
        If Not TextboxValidation(tbEngineStopStartActivationDelay.Text) Then
            MsgBox("Invalid input value for 'Engine-off'!", vbExclamation, "Warning")
            tbEngineStopStartActivationDelay.Focus()
            Return
        End If
    End Sub

    Private Sub tbMaxEngineOffTimespan_Leave(sender As Object, e As System.EventArgs) Handles tbMaxEngineOffTimespan.Leave
        If Not TextboxValidation(tbMaxEngineOffTimespan.Text) Then
            MsgBox("Invalid input value for 'Max. engine-off time:'!", vbExclamation, "Warning")
            tbMaxEngineOffTimespan.Focus()
            Return
        End If
    End Sub

    Private Sub tbEcoRollMinSpeed_Leave(sender As Object, e As System.EventArgs) Handles tbEcoRollMinSpeed.Leave
        If Not TextboxValidation(tbEcoRollMinSpeed.Text) Then
            MsgBox("Invalid input value for 'Eco Roll - Minimum Speed'!", vbExclamation, "Warning")
            tbEcoRollMinSpeed.Focus()
            Return
        End If
    End Sub

    Private Sub tbEcoRollActivationDelay_Leave(sender As Object, e As System.EventArgs) Handles tbEcoRollActivationDelay.Leave
        If Not TextboxValidation(tbEcoRollActivationDelay.Text) Then
            MsgBox("Invalid input value for 'Eco Roll - Activation Delay'!", vbExclamation, "Warning")
            tbEcoRollActivationDelay.Focus()
            Return
        End If
    End Sub

    Private Sub tbEcoRollMaxAcc_Leave(sender As Object, e As System.EventArgs) Handles tbEcoRollMaxAcc.Leave
        If Not TextboxValidation(tbEcoRollMaxAcc.Text) Then
            MsgBox("Invalid input value for 'Eco Roll - Upper Accelaration Limit'!", vbExclamation, "Warning")
            tbEcoRollActivationDelay.Focus()
            Return
        End If
    End Sub

    Private Sub tbEcoRollUnderspeed_Leave(sender As Object, e As System.EventArgs) Handles tbEcoRollUnderspeed.Leave
        If Not TextboxValidation(tbEcoRollUnderspeed.Text) Then
            MsgBox("Invalid input value for 'Eco Roll - Underspeed threshold'!", vbExclamation, "Warning")
            tbEcoRollUnderspeed.Focus()
            Return
        End If
    End Sub

    Private Sub tbPCCUnderspeed_Leave(sender As Object, e As System.EventArgs) Handles tbPCCUnderspeed.Leave
        If Not TextboxValidation(tbPCCUnderspeed.Text) Then
            MsgBox("Invalid input value for 'Predictive Cruise Control -  Allowed Underspeed'!", vbExclamation, "Warning")
            tbPCCUnderspeed.Focus()
            Return
        End If
    End Sub

    Private Sub tbPCCOverspeed_Leave(sender As Object, e As System.EventArgs) Handles tbPCCOverspeed.Leave
        If Not TextboxValidation(tbPCCOverspeed.Text) Then
            MsgBox("Invalid input value for 'Predictive Cruise Control - Allowed Overspeed'!", vbExclamation, "Warning")
            tbPCCOverspeed.Focus()
            Return
        End If
    End Sub

    Private Sub tbPCCEnableSpeed_Leave(sender As Object, e As System.EventArgs) Handles tbPCCEnableSpeed.Leave
        If Not TextboxValidation(tbPCCEnableSpeed.Text) Then
            MsgBox("Invalid input value for 'Predictive Cruise Control - PCC enabling velocity'!", vbExclamation, "Warning")
            tbPCCEnableSpeed.Focus()
            Return
        End If
    End Sub

    Private Sub tbPCCMinSpeed_Leave(sender As Object, e As System.EventArgs) Handles tbPCCMinSpeed.Leave
        If Not TextboxValidation(tbPCCMinSpeed.Text) Then
            MsgBox("Invalid input value for 'Predictive Cruise Control - Minimum Speed'!", vbExclamation, "Warning")
            tbPCCMinSpeed.Focus()
            Return
        End If
    End Sub

    Private Sub tbPCCPreviewUseCase1_Leave(sender As Object, e As System.EventArgs) Handles tbPCCPreviewUseCase1.Leave
        If Not TextboxValidation(tbPCCPreviewUseCase1.Text) Then
            MsgBox("Invalid input value for 'Predictive Cruise Control - Preview Distance use case 1'!", vbExclamation, "Warning")
            tbPCCPreviewUseCase1.Focus()
            Return
        End If
    End Sub

    Private Sub tbtbPCCPreviewUseCase2_Leave(sender As Object, e As System.EventArgs) Handles tbPCCPreviewUseCase2.Leave
        If Not TextboxValidation(tbPCCPreviewUseCase2.Text) Then
            MsgBox("Invalid input value for 'Predictive Cruise Control - Preview Distance use case 2'!", vbExclamation, "Warning")
            tbPCCPreviewUseCase2.Focus()
            Return
        End If
    End Sub

    Private Sub TbAuxPAuxICEOn_Leave(sender As Object, e As System.EventArgs) Handles TbAuxPAuxICEOn.Leave
        If Not TextboxValidation(TbAuxPAuxICEOn.Text) Then
            MsgBox("Invalid input value for 'Auxiliaries - Aux Load (ICE On)'!", vbExclamation, "Warning")
            TbAuxPAuxICEOn.Focus()
            Return
        End If
    End Sub

    Private Sub tbPAuxDrivingICEOff_Leave(sender As Object, e As System.EventArgs) Handles tbPAuxDrivingICEOff.Leave
        If Not TextboxValidation(tbPAuxDrivingICEOff.Text) Then
            MsgBox("Invalid input value for 'Auxiliaries - Aux Load (Driving, ICE Off)'!", vbExclamation, "Warning")
            tbPAuxDrivingICEOff.Focus()
            Return
        End If
    End Sub

    Private Sub tbPAuxStandstillICEOff_Leave(sender As Object, e As System.EventArgs) Handles tbPAuxStandstillICEOff.Leave
        If Not TextboxValidation(tbPAuxStandstillICEOff.Text) Then
            MsgBox("Invalid input value for 'Auxiliaries - Aux Load (Standstill, ICE Off)'!", vbExclamation, "Warning")
            tbPAuxStandstillICEOff.Focus()
            Return
        End If
    End Sub

    Private Sub TbOverspeed_Leave(sender As Object, e As System.EventArgs) Handles TbOverspeed.Leave
        If Not TextboxValidation(TbOverspeed.Text) Then
            MsgBox("Invalid input value for 'Driver Model - 'Maximum Overspeed'!", vbExclamation, "Warning")
            TbOverspeed.Focus()
            Return
        End If
    End Sub

    Private Sub TbVmin_Leave(sender As Object, e As System.EventArgs) Handles TbVmin.Leave
        If Not TextboxValidation(TbVmin.Text) Then
            MsgBox("Invalid input value for 'Driver Model - 'Minimum Speed'!", vbExclamation, "Warning")
            TbVmin.Focus()
            Return
        End If
    End Sub

    Private Sub tbLacMinSpeed_Leave(sender As Object, e As System.EventArgs) Handles tbLacMinSpeed.Leave
        If Not TextboxValidation(tbLacMinSpeed.Text) Then
            MsgBox("Invalid input value for 'Look-Ahead Coasting - 'Min. Velocity'!", vbExclamation, "Warning")
            tbLacMinSpeed.Focus()
            Return
        End If
    End Sub

    Private Sub tbLacPreviewFactor_Leave(sender As Object, e As System.EventArgs) Handles tbLacPreviewFactor.Leave
        If Not TextboxValidation(tbLacPreviewFactor.Text) Then
            MsgBox("Invalid input value for 'Look-Ahead Coasting - 'Preview distance factor'!", vbExclamation, "Warning")
            tbLacPreviewFactor.Focus()
            Return
        End If
    End Sub


#End Region

    Private Sub btnDfTargetSpeed_Click(sender As Object, e As EventArgs) Handles btnDfTargetSpeed.Click
        If DriverDecisionFactorTargetSpeedFileBrowser.OpenDialog(FileRepl(tbLacDfTargetSpeedFile.Text, GetPath(VectoFile))) _
            Then _
            tbLacDfTargetSpeedFile.Text = GetFilenameWithoutDirectory(DriverDecisionFactorTargetSpeedFileBrowser.Files(0),
                                                                    GetPath(VectoFile))
    End Sub

    Private Sub btnDfVelocityDrop_Click_1(sender As Object, e As EventArgs) Handles btnDfVelocityDrop.Click
		If DriverDecisionFactorVelocityDropFileBrowser.OpenDialog(FileRepl(tbLacDfVelocityDropFile.Text, GetPath(VectoFile))) _
			Then _
			tbLacDfVelocityDropFile.Text = GetFilenameWithoutDirectory(DriverDecisionFactorVelocityDropFileBrowser.Files(0),
																		GetPath(VectoFile))
	End Sub

	Private Sub LvCycles_MouseClick(sender As Object, e As MouseEventArgs) Handles LvCycles.MouseClick
		If e.Button = MouseButtons.Right AndAlso LvCycles.SelectedItems.Count > 0 Then
			OpenFiles(FileRepl(LvCycles.SelectedItems(0).SubItems(0).Text, GetPath(VectoFile)))
		End If
	End Sub


	Private Sub BtnShiftStrategyParams_Click(sender As Object, e As EventArgs) Handles BtnShiftStrategyParams.Click
		If TCUFileBrowser.OpenDialog(FileRepl(TbShiftStrategyParams.Text, GetPath(VectoFile))) Then
			TbShiftStrategyParams.Text = GetFilenameWithoutDirectory(TCUFileBrowser.Files(0), GetPath(VectoFile))
		End If
	End Sub

	
	Private Sub Label44_Click(sender As Object, e As EventArgs) Handles Label44.Click

	End Sub

    Private Sub VectoJobForm_HandleDestroyed(sender As Object, e As EventArgs) Handles Me.HandleDestroyed

    End Sub

    Private Sub btnBrowseHybridStrategyParams_Click(sender As Object, e As EventArgs) Handles btnBrowseHybridStrategyParams.Click
        If HCUFileBrowser.OpenDialog(FileRepl(tbHybridStrategyParams.Text, GetPath(VectoFile))) Then
            tbHybridStrategyParams.Text = GetFilenameWithoutDirectory(HCUFileBrowser.Files(0), GetPath(VectoFile))
        End If
    End Sub

    Private Sub btnOpenHybridStrategyParameters_Click(sender As Object, e As EventArgs) Handles btnOpenHybridStrategyParameters.Click
        Dim f As String
        f = FileRepl(tbHybridStrategyParams.Text, GetPath(VectoFile))

        'Thus Veh-file is returned
        HybridStrategyParamsForm.JobDir = GetPath(VectoFile)
        HybridStrategyParamsForm.JobType = JobType
        HybridStrategyParamsForm.AutoSendTo = True

        If Not Trim(f) = "" Then
            If Not File.Exists(f) Then
                MsgBox("File not found!")
                Exit Sub
            End If
        End If

        If Not HybridStrategyParamsForm.Visible Then
            HybridStrategyParamsForm.Show()
        Else
            If HybridStrategyParamsForm.WindowState = FormWindowState.Minimized Then HybridStrategyParamsForm.WindowState = FormWindowState.Normal
            HybridStrategyParamsForm.BringToFront()
        End If
        Dim vehicleType As VehicleCategory
        Try
            If Not Trim(f) = "" Then
                Dim vehInput As IVehicleDeclarationInputData =
                        CType(JSONInputDataFactory.ReadComponentData(FileRepl(TbVEH.Text, GetPath(VectoFile))),
                              IEngineeringInputDataProvider).JobInputData.Vehicle
                vehicleType = vehInput.VehicleCategory
            End If

        Catch ex As Exception
            vehicleType = VehicleCategory.RigidTruck
        End Try
        Try
            If Not Trim(f) = "" Then HybridStrategyParamsForm.OpenHybridStrategyParametersFile(f)
        Catch ex As Exception
            MsgBox("Failed to open Hybrid strategy parameters File: " + ex.Message)
        End Try

    End Sub

    Private Sub cbEnableBusAux_CheckedChanged(sender As Object, e As EventArgs) Handles cbEnableBusAux.CheckedChanged
        pnBusAux.Enabled = not Cfg.DeclMode AndAlso cbEnableBusAux.Checked
    End Sub

    Private Sub btnBusAuxP_Click(sender As Object, e As EventArgs) Handles btnBusAuxP.Click 
        Dim f As String
        f = FileRepl(tbBusAuxParams.Text, GetPath(VectoFile))

        'Thus Veh-file is returned
        BusAuxiliariesEngParametersForm.JobDir = GetPath(VectoFile)
        BusAuxiliariesEngParametersForm.AutoSendTo = True
        BusAuxiliariesEngParametersForm.JobType = JobType
        If Not Trim(f) = "" Then
            If Not File.Exists(f) Then
                MsgBox("File not found!")
                Exit Sub
            End If
        End If

        If Not BusAuxiliariesEngParametersForm.Visible Then
            BusAuxiliariesEngParametersForm.Show()
        Else
            If BusAuxiliariesEngParametersForm.WindowState = FormWindowState.Minimized Then BusAuxiliariesEngParametersForm.WindowState = FormWindowState.Normal
            BusAuxiliariesEngParametersForm.BringToFront()
        End If
        'Dim vehicleType As VehicleCategory
        'Try
        '    If Not Trim(f) = "" Then
        '        Dim vehInput As IVehicleDeclarationInputData =
        '                CType(JSONInputDataFactory.ReadComponentData(FileRepl(TbVEH.Text, GetPath(VectoFile))),
        '                      IEngineeringInputDataProvider).JobInputData.Vehicle
        '        vehicleType = vehInput.VehicleCategory
        '    End If

        'Catch ex As Exception
        '    vehicleType = VehicleCategory.RigidTruck
        'End Try
        
        Try
            If Not Trim(f) = "" Then BusAuxiliariesEngParametersForm.OpenBusAuxParametersFile(f)
        Catch ex As Exception
            MsgBox("Failed to open Gearbox File: " + ex.Message)
        End Try
    End Sub

    Private Sub btnBrowsBusAuxParams_Click(sender As Object, e As EventArgs) Handles btnBrowsBusAuxParams.Click
        If BusAuxFileBrowser.OpenDialog(FileRepl(tbBusAuxParams.Text, GetPath(VectoFile))) Then
            tbBusAuxParams.Text = GetFilenameWithoutDirectory(BusAuxFileBrowser.Files(0), GetPath(VectoFile))
        End If
    End Sub

End Class


