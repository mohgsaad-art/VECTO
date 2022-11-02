
Imports System.Collections.Generic
Imports System.Drawing.Imaging
Imports System.IO
Imports System.Linq
Imports System.Windows.Forms.DataVisualization.Charting
Imports TUGraz.VectoCommon.InputData
Imports TUGraz.VectoCommon.Utils
Imports TUGraz.VectoCore.InputData.FileIO.JSON
Imports TUGraz.VectoCore.InputData.Reader.ComponentData
Imports TUGraz.VectoCore.Models.SimulationComponent.Data.Battery
Imports TUGraz.VectoCore.Utils
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
Public Class BatteryForm
    Private _batteryFile As String = ""
    Public AutoSendTo As Boolean = False
    Public JobDir As String = ""
    Private _changed As Boolean = False

    Private ressType as REESSType

    Private _contextMenuFiles As String()

    Public ReadOnly Property BatteryFile As String
        Get
            Return _batteryFile
        End Get
    End Property


    'Before closing Editor: Check if file was changed and ask to save.
    Private Sub F_ENG_FormClosing(sender As Object, e As FormClosingEventArgs) Handles Me.FormClosing
        If e.CloseReason <> CloseReason.ApplicationExitCall And e.CloseReason <> CloseReason.WindowsShutDown Then
            e.Cancel = ChangeCheckCancel()
        End If
    End Sub

    'Initialise.
    Private Sub BatteryFormLoad(sender As Object, e As EventArgs) Handles Me.Load

        ' initialize form on load - nothng to do right now

        'pnInertia.Enabled = Not Cfg.DeclMode


        _changed = False

        cbRESSType.DataSource = New List(Of object)() from{
            New With {Key .Value = REESSType.Battery, .Label = "Battery"},   
            New With {Key .Value = REESSType.SuperCap, .Label = "SuperCap"}    
        }
        cbRESSType.ValueMember = "Value"
        cbRESSType.DisplayMember = "Label"

        NewBattery()
    End Sub

    'Set generic values for Declaration mode.
    Private Sub DeclInit()

        If Not Cfg.DeclMode Then Exit Sub



    End Sub


#Region "Toolbar"

    Private Sub ToolStripBtNew_Click(sender As Object, e As EventArgs) Handles ToolStripBtNew.Click
        NewBattery()
    End Sub

    Private Sub ToolStripBtOpen_Click(sender As Object, e As EventArgs) Handles ToolStripBtOpen.Click
        If REESSFileBrowser.OpenDialog(_batteryFile) Then
            Try
                OpenBatteryFile(REESSFileBrowser.Files(0))
            Catch ex As Exception
                MsgBox(ex.Message, MsgBoxStyle.OkOnly, "Error loading Battery File")
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

        If _batteryFile = "" Then
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

        VectoJobForm.TbENG.Text = GetFilenameWithoutDirectory(_batteryFile, JobDir)
    End Sub

    Private Sub ToolStripButton1_Click(sender As Object, e As EventArgs) Handles ToolStripButton1.Click
        If File.Exists(Path.Combine(MyAppPath, "User Manual\help.html")) Then
            Dim defaultBrowserPath As String = BrowserUtils.GetDefaultBrowserPath()
            Process.Start(defaultBrowserPath,$"""file://{Path.Combine(MyAppPath, "User Manual\help.html#engine-editor")}""")
        Else
            MsgBox("User Manual not found!", MsgBoxStyle.Critical)
        End If
    End Sub

#End Region

    'Create new empty Engine file.
    Private Sub NewBattery()

        If ChangeCheckCancel() Then Exit Sub

        tbMakeModel.Text = ""
        tbCapacity.Text = ""
        tbRiCurve.Text = ""
        tbSoCCurve.Text = ""

        DeclInit()

        _batteryFile = ""
        Text = "Electric Energy Storage Editor"
        LbStatus.Text = ""

        _changed = False

        UpdatePic()
    End Sub

    'Open VENG file
    Public Sub OpenBatteryFile(file As String)
        
        If ChangeCheckCancel() Then Exit Sub

        Dim inputData As IEngineeringInputDataProvider = TryCast(JSONInputDataFactory.ReadComponentData(file),
                                                                IEngineeringInputDataProvider)

        Dim reess as IREESSPackInputData = inputData.JobInputData.Vehicle.Components.ElectricStorage.ElectricStorageElements.FirstOrDefault().REESSPack

        If Cfg.DeclMode <> reess.SavedInDeclarationMode Then
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
        tbMakeModel.Text = reess.Model

        if (reess.StorageType = REESSType.Battery) then
            pnBattery.Visible = True
            pnSuperCap.Visible  = False
            cbRESSType.SelectedValue = REESSType.Battery
            Dim battery As IBatteryPackEngineeringInputData = ctype(reess, IBatteryPackEngineeringInputData)
            tbCapacity.Text = battery.Capacity.AsAmpHour.ToGUIFormat()

            tbSoCMin.Text = (battery.MinSOC.Value * 100).ToGUIFormat()
            tbSoCMax.Text = (battery.MaxSOC.Value * 100).ToGUIFormat()

            tbMaxCurrentMap.Text = GetRelativePath(battery.MaxCurrentMap.Source, basePath)
            tbSoCCurve.Text = GetRelativePath(battery.VoltageCurve.Source, basePath)
            tbRiCurve.Text = GetRelativePath(battery.InternalResistanceCurve.Source, basePath)

            tbSuperCapCapacity.Text = String.Empty
            tbSuperCapMaxV.Text = string.Empty
            tbSuperCapMinV.Text= string.Empty
            tbSuperCapRi.Text= string.Empty

        Elseif reess.StorageType = REESSType.SuperCap
            pnBattery.Visible = False
            pnSuperCap.Visible  = True
            cbRESSType.SelectedValue = REESSType.SuperCap
            Dim superCap As ISuperCapEngineeringInputData = ctype(reess, ISuperCapEngineeringInputData)
            tbCapacity.Text = String.Empty

            tbMaxCurrentMap.Text  = String.Empty
            tbSoCMin.Text  = String.Empty
            tbSoCMax.Text = String.Empty

            tbSoCCurve.Text =  String.Empty
            tbRiCurve.Text = String.Empty

            tbSuperCapCapacity.Text = superCap.Capacity.ToGUIFormat()
            tbSuperCapMaxV.Text = superCap.MaxVoltage.ToGUIFormat()
            tbSuperCapMinV.Text= superCap.MinVoltage.ToGUIFormat()
            tbSuperCapRi.Text= superCap.InternalResistance.ToGUIFormat()

            tbSuperCapMaxCurrentCharge.Text = superCap.MaxCurrentCharge.ToGuiFormat()
            tbSuperCapMaxCurrentDischarge.Text = superCap.MaxCurrentDischarge.ToGuiFormat()

        end if

        DeclInit()

        REESSFileBrowser.UpdateHistory(file)
        Text = GetFilenameWithoutPath(file, True)
        LbStatus.Text = ""
        _batteryFile = file
        Activate()

        _changed = False
        UpdatePic()
    End Sub

    'Save or Save As function = true if file is saved
    Private Function SaveOrSaveAs(ByVal saveAs As Boolean) As Boolean
        If _batteryFile = "" Or saveAs Then
            If REESSFileBrowser.SaveDialog(_batteryFile) Then
                _batteryFile = REESSFileBrowser.Files(0)
            Else
                Return False
            End If
        End If
        Return SaveBatteryToFile(_batteryFile)
    End Function

    'Save VENG file to given filepath. Called by SaveOrSaveAs. 
    Private Function SaveBatteryToFile(ByVal file As String) As Boolean

       

        Select Case cbRESSType.SelectedValue.ToString()
            Case "Battery"
                dim battery As Battery = FillBattery(file)
                If Not battery.SaveFile Then
                    MsgBox("Cannot save to " & file, MsgBoxStyle.Critical)
                    Return False
                End If
            Case "SuperCap"
                dim superCap As SuperCap = FillSuperCap(file)
                If Not superCap.SaveFile Then
                    MsgBox("Cannot save to " & file, MsgBoxStyle.Critical)
                    Return False
                End If
        End Select

        If AutoSendTo Then
            If REESSPAckDialog.Visible Then
                If UCase(FileRepl(REESSPAckDialog.tbBattery.Text, JobDir)) <> UCase(file) Then _
                    REESSPAckDialog.tbBattery.Text = GetFilenameWithoutDirectory(file, JobDir)
                'VectoJobForm.UpdatePic()
            End If
        End If

        REESSFileBrowser.UpdateHistory(file)
        Text = GetFilenameWithoutPath(file, True)
        LbStatus.Text = ""

        _changed = False

        Return True
    End Function

    Private Function FillSuperCap(file As String) As SuperCap
        Dim superCap As SuperCap = New SuperCap()
        superCap.FilePath = file

        superCap.ModelName = tbMakeModel.Text
        If Trim(superCap.ModelName) = "" Then superCap.ModelName = "Undefined"

        
        superCap.Cap = tbSuperCapCapacity.Text.ToDouble(0)
        superCap.Ri = _tbSuperCapRi.Text.ToDouble(0)

        superCap.MinV = _tbSuperCapMinV.Text.ToDouble(0)
        superCap.MaxV = tbSuperCapMaxV.Text.ToDouble(0)

        superCap.MaxChgCurrent = tbSuperCapMaxCurrentCharge.Text.ToDouble()
        superCap.MaxDischgCurrent = tbSuperCapMaxCurrentDischarge.Text.ToDouble()
        Return superCap
    End Function

    Private Function FillBattery(file As string) As Battery
        Dim battery As Battery = New Battery
        battery.FilePath = file

        battery.ModelName = tbMakeModel.Text
        If Trim(battery.ModelName) = "" Then battery.ModelName = "Undefined"

        battery.BatCapacity = tbCapacity.Text.ToDouble(0)

        battery.PathSoCCurve = tbSoCCurve.Text
        battery.PathRiCurve = tbRiCurve.Text

        battery.BatMinSoc = tbSoCMin.Text.ToDouble(0)
        battery.BatMaxSoc = tbSoCMax.Text.ToDouble(0)

        battery.PathMaxCurrentCurve = tbMaxCurrentMap.Text
        Return battery
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


    Private Sub TbName_TextChanged(sender As Object, e As EventArgs) Handles tbMakeModel.TextChanged
        Change()
    End Sub


    Private Sub TbInertia_TextChanged(sender As Object, e As EventArgs) Handles tbCapacity.TextChanged
        Change()
    End Sub



    Private Sub TbMAP_TextChanged(sender As Object, e As EventArgs) _
        Handles tbSoCCurve.TextChanged
        UpdatePic()
        Change()
    End Sub

   


#End Region


    'Open VMAP file
    Private Sub BtMAPopen_Click(sender As Object, e As EventArgs)
        Dim fldfile As String

        fldfile = FileRepl(tbSoCCurve.Text, GetPath(_batteryFile))

        If fldfile <> NoFile AndAlso File.Exists(fldfile) Then
            OpenFiles(FileRepl(tbRiCurve.Text, GetPath(_batteryFile)), fldfile)
        Else
            OpenFiles(FileRepl(tbRiCurve.Text, GetPath(_batteryFile)))
        End If
    End Sub


    'Save and close
    Private Sub ButOK_Click(sender As Object, e As EventArgs) Handles ButOK.Click
        If SaveOrSaveAs(False) Then
            DialogResult = DialogResult.OK
            Close()
        End If
    End Sub

    'Close without saving (see FormClosing Event)
    Private Sub ButCancel_Click(sender As Object, e As EventArgs) Handles ButCancel.Click
        DialogResult = DialogResult.Cancel
        Close()
    End Sub

    Private Sub UpdatePic()
        Dim socCurve As SOCMap = Nothing
        Dim riCurve As InternalResistanceMap = Nothing

        'Dim engineCharacteristics As String = ""

        PicBox.Image = Nothing

        'If Not File.Exists(_engFile) Then Exit Sub

        if (ressType <> REESSType.Battery) then
            Return
        end if

        Try
            Dim socFile As String =
                    If(Not String.IsNullOrWhiteSpace(_batteryFile), Path.Combine(Path.GetDirectoryName(_batteryFile), tbSoCCurve.Text), tbSoCCurve.Text)
            If File.Exists(socFile) Then _
                socCurve = BatterySOCReader.Create(VectoCSVFile.Read(socFile))
        Catch ex As Exception
        End Try

        Try
            Dim riFile As String =
                    If(Not String.IsNullOrWhiteSpace(_batteryFile), Path.Combine(Path.GetDirectoryName(_batteryFile), tbRiCurve.Text), tbRiCurve.Text)
            If File.Exists(riFile) Then riCurve = BatteryInternalResistanceReader.Create(VectoCSVFile.Read(riFile), false)
        Catch ex As Exception
        End Try

        If socCurve Is Nothing AndAlso riCurve Is Nothing Then Exit Sub

        'Create plot
        Dim chart As Chart = New Chart
        chart.Width = PicBox.Width
        chart.Height = PicBox.Height

        Dim chartArea As ChartArea = New ChartArea

        If Not socCurve Is Nothing Then
            Dim series As Series = New Series
            series.Points.DataBindXY(socCurve.Entries.Select(Function(x) x.SOC * 100).ToArray(),
                                    socCurve.Entries.Select(Function(x) x.BatteryVolts.Value()).ToArray())
            series.ChartType = SeriesChartType.FastLine
            series.BorderWidth = 2
            series.Color = Color.DarkBlue
            series.Name = "Battery Voltage (" & tbSoCCurve.Text & ")"
            chart.Series.Add(series)
        End If

        If Not riCurve Is Nothing Then
            Dim series1 As Series = New Series
            series1.Points.DataBindXY(riCurve.Entries.Select(Function(x) x.SoC * 100).ToArray(),
                                     riCurve.Entries.Select(Function(x) x.Resistance.First.Item2.Value()).ToArray())
            series1.ChartType = SeriesChartType.FastLine
            series1.MarkerSize = 3
            series1.Color = Color.MediumVioletRed
            series1.Name = "Internal Resistance t_min"
            series1.YAxisType = AxisType.Secondary
            chart.Series.Add(series1)
            Dim series2 As Series = New Series
            series2.Points.DataBindXY(riCurve.Entries.Select(Function(x) x.SoC * 100).ToArray(),
                                     riCurve.Entries.Select(Function(x) x.Resistance.Last().Item2.Value()).ToArray())
            series2.ChartType = SeriesChartType.FastLine
            series2.MarkerSize = 3
            series2.Color = Color.PaleVioletRed
            series2.Name = "Internal Resistance t_max"
            series2.YAxisType = AxisType.Secondary
            chart.Series.Add(series2)
        End If


        chartArea.Name = "main"

        chartArea.AxisX.Title = "SoC [%]"
        chartArea.AxisX.TitleFont = New Font("Helvetica", 10)
        chartArea.AxisX.LabelStyle.Font = New Font("Helvetica", 8)
        chartArea.AxisX.LabelAutoFitStyle = LabelAutoFitStyles.None
        chartArea.AxisX.MajorGrid.LineDashStyle = ChartDashStyle.Dot

        chartArea.AxisY.Title = "Voltage [V]"
        chartArea.AxisY.TitleFont = New Font("Helvetica", 10)
        chartArea.AxisY.LabelStyle.Font = New Font("Helvetica", 8)
        chartArea.AxisY.LabelAutoFitStyle = LabelAutoFitStyles.None
        chartArea.AxisY.MajorGrid.LineDashStyle = ChartDashStyle.Dot

        chartArea.AxisY2.Title = "Internal Resistance [Ω]"
        chartArea.AxisY2.TitleFont = New Font("Helvetica", 10)
        chartArea.AxisY2.LabelStyle.Font = New Font("Helvetica", 8)
        chartArea.AxisY2.LabelAutoFitStyle = LabelAutoFitStyles.None
        chartArea.AxisY2.MajorGrid.LineDashStyle = ChartDashStyle.Dot

        chartArea.AxisX.Minimum = 0
        chartArea.BorderDashStyle = ChartDashStyle.Solid
        chartArea.BorderWidth = 1

        chartArea.BackColor = Color.GhostWhite

        chart.ChartAreas.Add(chartArea)

        chart.Update()

        Dim img As Bitmap = New Bitmap(chart.Width, chart.Height, PixelFormat.Format32bppArgb)
        chart.DrawToBitmap(img, New Rectangle(0, 0, PicBox.Width, PicBox.Height))


        PicBox.Image = img
        'lblEngineCharacteristics.Text = engineCharacteristics
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

#End Region


    Private Sub btnBrowseSoCCurve_Click(sender As Object, e As EventArgs) Handles btnBrowseSoCCurve.Click
        If BatterySoCCurveFileBrowser.OpenDialog(FileRepl(tbSoCCurve.Text, GetPath(_batteryFile))) Then _
            tbSoCCurve.Text = GetFilenameWithoutDirectory(BatterySoCCurveFileBrowser.Files(0), GetPath(_batteryFile))
    End Sub

    Private Sub btnBrowseRiMap_Click(sender As Object, e As EventArgs) Handles btnBrowseRiMap.Click
        If BatteryInternalResistanceCurveFileBrowser.OpenDialog(FileRepl(tbRiCurve.Text, GetPath(_batteryFile))) Then _
            tbRiCurve.Text = GetFilenameWithoutDirectory(BatteryInternalResistanceCurveFileBrowser.Files(0), GetPath(_batteryFile))
    End Sub

    Private Sub btnSoCCurveOpen_Click(sender As Object, e As EventArgs) Handles btnSoCCurveOpen.Click
        Dim theFile As String

        theFile = FileRepl(tbSoCCurve.Text, GetPath(_batteryFile))

        If theFile <> NoFile AndAlso File.Exists(theFile) Then
            OpenFiles(FileRepl(tbSoCCurve.Text, GetPath(_batteryFile)), theFile)
        Else
            OpenFiles(FileRepl(tbSoCCurve.Text, GetPath(_batteryFile)))
        End If
    End Sub

    Private Sub btnRiMapOpen_Click(sender As Object, e As EventArgs) Handles btnRiMapOpen.Click
        Dim theFile As String

        theFile = FileRepl(tbRiCurve.Text, GetPath(_batteryFile))

        If theFile <> NoFile AndAlso File.Exists(theFile) Then
            OpenFiles(FileRepl(tbRiCurve.Text, GetPath(_batteryFile)), theFile)
        Else
            OpenFiles(FileRepl(tbRiCurve.Text, GetPath(_batteryFile)))
        End If
    End Sub

    Private Sub tbRiCurve_TextChanged(sender As Object, e As EventArgs) Handles tbRiCurve.TextChanged
        UpdatePic()
        Change()
    End Sub

    Private Sub lblRessType_Click(sender As Object, e As EventArgs) Handles lblRessType.Click

    End Sub

    Private Sub cbRESSType_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cbRESSType.SelectedIndexChanged
        If (cbRESSType.SelectedValue.Equals(REESSType.Battery)) Then
            pnBattery.Visible = True
            pnSuperCap.Visible = False
            ressType = REESSType.Battery
        Else 
            pnBattery.Visible = False
            pnSuperCap.Visible = True
            ressType = REESSType.SuperCap 
        End If
    End Sub

    Private Sub btnBrowseMaxCurrentMap_Click(sender As Object, e As EventArgs) Handles btnBrowseMaxCurrentMap.Click
        If BatteryMaxCurrentCurveFileBrowser.OpenDialog(FileRepl(tbMaxCurrentMap.Text, GetPath(_batteryFile))) Then _
            tbMaxCurrentMap.Text = GetFilenameWithoutDirectory(BatteryMaxCurrentCurveFileBrowser.Files(0), GetPath(_batteryFile))
    End Sub

    Private Sub btnMaxCurrentMapOpen_Click(sender As Object, e As EventArgs) Handles btnMaxCurrentMapOpen.Click
        Dim theFile As String

        theFile = FileRepl(tbMaxCurrentMap.Text, GetPath(_batteryFile))

        If theFile <> NoFile AndAlso File.Exists(theFile) Then
            OpenFiles(FileRepl(tbMaxCurrentMap.Text, GetPath(_batteryFile)), theFile)
        Else
            OpenFiles(FileRepl(tbMaxCurrentMap.Text, GetPath(_batteryFile)))
        End If
    End Sub
End Class
