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

Imports System.Drawing.Imaging
Imports System.IO
Imports System.Linq
Imports System.Windows.Forms.DataVisualization.Charting
Imports System.Xml
Imports Ninject
Imports TUGraz.VECTO.Input_Files
Imports TUGraz.VectoCommon.InputData
Imports TUGraz.VectoCommon.Models
Imports TUGraz.VectoCommon.Utils
Imports TUGraz.VectoCore
Imports TUGraz.VectoCore.InputData.FileIO.JSON
Imports TUGraz.VectoCore.InputData.FileIO.XML
Imports TUGraz.VectoCore.InputData.Reader.ComponentData
Imports TUGraz.VectoCore.Models.Declaration
Imports TUGraz.VectoCore.Models.SimulationComponent.Data.Engine

''' <summary>
''' Job Editor. Create/Edit VECTO job files (.vecto)
''' </summary>
''' <remarks></remarks>
Public Class VectoVTPJobForm
    Public VectoFile As String
    Private _changed As Boolean = False

    Dim _xmlInputReader as IXMLInputDataReader

    Enum AuxViewColumns
        AuxID = 0
        AuxType = 1
        AuxInputOrTech = 2
    End Enum


    'Initialise form
    Private Sub F02_GEN_Load(sender As Object, e As EventArgs) Handles Me.Load

        Dim kernel as IKernel = New StandardKernel(new VectoNinjectModule)
        _xmlInputReader = kernel.Get(Of IXMLInputDataReader)

        LvAux.Columns(AuxViewColumns.AuxInputOrTech).Width = - 2

        LvAux.Columns(AuxViewColumns.AuxInputOrTech).Text = "Technology"

        pnManufacturerRecord.Visible = cfg.DeclMode
        pnFanParameters.Enabled = Not cfg.DeclMode

        GrCycles.Enabled = True

        _changed = False
    End Sub

    'Close - Check for unsaved changes
    Private Sub F02_GEN_FormClosing(sender As Object, e As FormClosingEventArgs) Handles Me.FormClosing
        If e.CloseReason <> CloseReason.ApplicationExitCall And e.CloseReason <> CloseReason.WindowsShutDown Then
            e.Cancel = ChangeCheckCancel()
        End If
    End Sub


#Region "Browse Buttons"

    Private Sub ButtonVEH_Click(sender As Object, e As EventArgs) Handles ButtonVEH.Click
        If VehicleXMLFileBrowser.OpenDialog(FileRepl(TbVEH.Text, GetPath(VectoFile))) Then
            TbVEH.Text = GetFilenameWithoutDirectory(VehicleXMLFileBrowser.Files(0), GetPath(VectoFile))
        End If
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
        Dim vectoJob As IVTPEngineeringJobInputData = Nothing
        Dim inputData As IVTPEngineeringInputDataProvider = Nothing
        Try
            inputData = TryCast(JSONInputDataFactory.ReadComponentData(file),
                                IVTPEngineeringInputDataProvider)
            vectoJob = inputData.JobInputData()
        Catch ex As Exception
            MsgBox("Failed to read Job-File" + Environment.NewLine + ex.Message)
            Return
        End Try


        If Cfg.DeclMode <> vectoJob.SavedInDeclarationMode Then
            Select Case WrongMode()
                Case 1
                    Close()
                    MainForm.RbDecl.Checked = Not MainForm.RbDecl.Checked
                    MainForm.OpenVectoFile(file)
                Case - 1
                    Exit Sub
            End Select
        End If

        VectoFile = file
        _basePath = Path.GetDirectoryName(file)
        'Update Form


        'Files -----------------------------
        TbVEH.Text = GetRelativePath(inputData.JobInputData.Vehicle.DataSource.SourceFile, _basePath)
        tbManufacturerRecord.Text =
            If (cfg.DeclMode, GetRelativePath(inputData.JobInputData.ManufacturerReportInputData.Source, _basePath), "")

        Dim auxInput As IAuxiliariesDeclarationInputData = inputData.JobInputData.Vehicle.Components.AuxiliaryInputData

        PopulateAuxiliaryList(auxInput)

        tbMileage.Text = if(Cfg.DeclMode, inputData.JobInputData.Mileage.ConvertToKiloMeter().Value.ToGUIFormat(), "")
        
        If cfg.DeclMode Then
            tbC1.Text = DeclarationData.VTPMode.FanParameters(0).ToGUIFormat()
            tbC2.Text = DeclarationData.VTPMode.FanParameters(1).ToGUIFormat()
            tbC3.Text = DeclarationData.VTPMode.FanParameters(2).ToGUIFormat()
            Dim coefficientsD As Double() = vectoJob.FanPowerCoefficents.ToArray()
            If (coefficientsD.Length >= 4) then
                tbC4.Text = coefficientsD(3).ToGUIFormat()
            Else 
                tbC4.Text = "1"
            End If
        Else
            Dim coefficients As Double() = vectoJob.FanPowerCoefficents.ToArray()
            If (coefficients.Length >= 1) Then
                tbC1.Text = coefficients(0).ToGUIFormat()
            End If
            If (coefficients.Length >= 2) Then
                tbC2.Text = coefficients(1).ToGUIFormat()
            End If
            If (coefficients.Length >= 3) Then
                tbC3.Text = coefficients(2).ToGUIFormat()
            End If
            If (coefficients.Length >= 4) then
                tbC4.Text = coefficients(3).ToGUIFormat()
            Else 
                tbC4.Text = "1"
            End If
        End If
        tbFanDiameter.Text = (vectoJob.FanDiameter.Value()*1000).ToGUIFormat()
        Try
            Dim sb As ICycleData
            For Each sb In vectoJob.Cycles
                Dim lv0 As ListViewItem = New ListViewItem
                lv0.Text = GetRelativePath(sb.CycleData.Source, Path.GetDirectoryName(Path.GetFullPath(file))) 'sb.Name
                LvCycles.Items.Add(lv0)
            Next
        Catch ex As Exception
        End Try

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

    Private Sub PopulateAuxiliaryList(auxInput As IAuxiliariesDeclarationInputData)

        LvAux.Items.Clear()
        If auxInput is Nothing Then
            Return
        End If
        
        Dim entry As IAuxiliaryDeclarationInputData
        For Each entry In auxInput.Auxiliaries
            'If entry.AuxiliaryType = AuxiliaryDemandType.Constant Then Continue For
            Try
                LvAux.Items.Add(CreateAuxListEntry(AuxiliaryTypeHelper.GetAuxKey(entry.Type),
                                                   AuxiliaryTypeHelper.ToString(entry.Type),
                                                   String.Join("; ", entry.Technology)))
            Catch ex As Exception
            End Try
        Next
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


        Dim vectoJob As VectoVTPJob = New VectoVTPJob
        vectoJob.FilePath = file

        'Files ------------------------------------------------- -----------------

        vectoJob.PathVeh = TbVEH.Text
        vectoJob.ManufacturerRecord = If(cfg.DeclMode, tbManufacturerRecord.Text, "")


        For Each lv0 As ListViewItem In LvCycles.Items
            Dim sb As SubPath = New SubPath
            sb.Init(GetPath(file), lv0.Text)
            vectoJob.CycleFiles.Add(sb)
        Next

        vectoJob.Mileage = tbMileage.Text.ToDouble(0).SI(Unit.SI.Kilo.Meter).Cast (of Meter)

       vectoJob.FanCoefficients = New Double() { _
                                                    tbC1.Text.ToDouble(0),
                                                    tbC2.Text.ToDouble(0),
                                                    tbC3.Text.ToDouble(0),
                                                    tbC4.Text.ToDouble(0)
                                                }
        vectoJob.FanDiameter = (tbFanDiameter.Text.ToDouble(0)/1000).SI (of Meter)

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
        LvCycles.Items.Clear()


        LvAux.Items.Clear()

        DeclInit()

        EngineForm.AutoSendTo = False

        VectoFile = ""
        Text = "Job Editor"
        ToolStripStatusLabelGEN.Text = ""
        _changed = False
        UpdatePic()
    End Sub

    Private Sub DeclInit()
        If not Cfg.DeclMode then exit Sub

        tbC1.Text = DeclarationData.VTPMode.FanParameters(0).ToGUIFormat()
        tbc2.Text = DeclarationData.VTPMode.FanParameters(1).ToGUIFormat()
        tbC3.Text = DeclarationData.VTPMode.FanParameters(2).ToGUIFormat()
        tbC4.Text = "1"
    End Sub


#Region "Track changes"

#Region "'Change' Events"

    Private Sub TextBoxVEH_TextChanged(sender As Object, e As EventArgs) _
        Handles TbVEH.TextChanged
        UpdateAuxList()
        UpdatePic()
        Change()
    End Sub

    Private Sub UpdateAuxList()
        Dim vehicleFile As String =
                If _
                (Not String.IsNullOrWhiteSpace(VectoFile), Path.Combine(Path.GetDirectoryName(VectoFile), TbVEH.Text),
                 TbVEH.Text)
        If File.Exists(vehicleFile) Then
            Try
                Dim inputData As IDeclarationInputDataProvider =
                        _xmlInputReader.CreateDeclaration(XmlReader.Create(vehicleFile))
                Dim auxInput As IAuxiliariesDeclarationInputData = inputData.JobInputData.Vehicle.Components.AuxiliaryInputData
                PopulateAuxiliaryList(auxInput)
            Catch
            End Try
        End If
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

        Dim gearbox As IGearboxDeclarationInputData = Nothing
        Dim vehicleFile As String =
                If _
                (Not String.IsNullOrWhiteSpace(VectoFile), Path.Combine(Path.GetDirectoryName(VectoFile), TbVEH.Text),
                 TbVEH.Text)
        If File.Exists(vehicleFile) Then
            Try
                Dim inputData As IDeclarationInputDataProvider =
                        _xmlInputReader.CreateDeclaration(XmlReader.Create(vehicleFile))
                gearbox = inputData.JobInputData.Vehicle.Components.GearboxInputData
            Catch
            End Try
        End If

        If gearbox Is Nothing Then Return

        TbGbxTxt.Text = $"{gearbox.Gears.Count}-Speed {gearbox.Type.ShortName()} {gearbox.Model}"
    End Sub

    Private Sub UpdateEnginePic(ByRef chart As Chart)
        Dim s As Series
        Dim pmax As Double

        Dim engine As IEngineDeclarationInputData = Nothing
        lblEngineCharacteristics.Text = ""
        Dim vehicleFile As String =
                If _
                (Not String.IsNullOrWhiteSpace(VectoFile), Path.Combine(Path.GetDirectoryName(VectoFile), TbVEH.Text),
                 TbVEH.Text)
        If File.Exists(vehicleFile) Then
            Try
                Dim inputData As IDeclarationInputDataProvider =
                        _xmlInputReader.CreateDeclaration(XmlReader.Create(vehicleFile))
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

        pmax = fullLoadCurve.MaxPower.Value()/1000 'FLD0.Pfull(FLD0.EngineRatedSpeed)


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

        Dim vehicle As IVehicleDeclarationInputData = Nothing

        Dim vehicleFile As String =
                If _
                (Not String.IsNullOrWhiteSpace(VectoFile), Path.Combine(Path.GetDirectoryName(VectoFile), TbVEH.Text),
                 TbVEH.Text)
        If File.Exists(vehicleFile) Then
            Try
                Dim inputData As IDeclarationInputDataProvider =
                        _xmlInputReader.CreateDeclaration(XmlReader.Create(vehicleFile))
                vehicle = inputData.JobInputData.Vehicle
            Catch
            End Try
        End If

        If vehicle Is Nothing Then Return

        Dim maxMass As Kilogram = vehicle.GrossVehicleMassRating _
        'CSng(fTextboxToNumString(TbMassMass.Text))

        Dim s0 As Segment = Nothing
        Try
            s0 = DeclarationData.TruckSegments.Lookup(vehicle.VehicleCategory, vehicle.AxleConfiguration, maxMass,
                                                 0.SI (Of Kilogram),
                                                 False)
        Catch
        End Try
        If s0.Found Then
           HDVclass = s0.VehicleClass
        End If

        PicVehicle.Image = ConvPicPath(HDVclass, False) _
        'Image.FromFile(cDeclaration.ConvPicPath(HDVclass, False))

        TbHVCclass.Text = $"HDV Group {HDVclass}"
        TbVehCat.Text = vehicle.VehicleCategory.GetCategoryName()   'ConvVehCat(VEH0.VehCat, True)
        TbMass.Text = (vehicle.GrossVehicleMassRating.Value()/1000) & " t"
        TbAxleConf.Text = vehicle.AxleConfiguration.GetName()   'ConvAxleConf(VEH0.AxleConf)
    End Sub


#Region "Open File Context Menu"

    Private _contextMenuFiles As String()
    Private _basePath As String = ""

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


    Private Sub LvCycles_MouseClick(sender As Object, e As MouseEventArgs) Handles LvCycles.MouseClick
        If e.Button = MouseButtons.Right AndAlso LvCycles.SelectedItems.Count > 0 Then
            OpenFiles(FileRepl(LvCycles.SelectedItems(0).SubItems(0).Text, GetPath(VectoFile)))
        End If
    End Sub

    Private Sub LvAux_SelectedIndexChanged(sender As Object, e As EventArgs) Handles LvAux.SelectedIndexChanged
    End Sub

    Private Sub ButtonManR_Click(sender As Object, e As EventArgs) Handles ButtonManR.Click
        If ManRXMLFileBrowser.OpenDialog(FileRepl(tbManufacturerRecord.Text, GetPath(VectoFile))) Then
            tbManufacturerRecord.Text = GetFilenameWithoutDirectory(ManRXMLFileBrowser.Files(0), GetPath(VectoFile))
        End If
    End Sub
End Class


