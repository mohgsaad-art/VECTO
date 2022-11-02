Imports System.IO
Imports TUGraz.VECTO.Input_Files
Imports TUGraz.VectoCommon.InputData
Imports TUGraz.VectoCommon.Utils
Imports TUGraz.VectoCore.InputData.FileIO.JSON

Public Class IHPCForm

    Public JobDir As String = ""
    Private _ihpcFilePath as String = ""
    Private _changed as Boolean
    Private _dragCurveFilePath as String
    Private _flCurveFilePath1 as String
    Private _flCurveFilePath2 as String

    Private _contextMenuFiles As String()

    Public AutoSendTo As action(of string, VehicleForm)

#Region "Set JSON Data"

    Public Sub ReadIHPCFile(file As String)
        Dim ihpcData = JSONInputDataFactory.ReadIHPCEngineeringInputData(file, True)
        _ihpcFilePath = file
        tbModel.Text = ihpcData.Model
        tbInertia.Text = ihpcData.Inertia.ToGUIFormat()
        tbThermalOverload.Text = ihpcData.OverloadRecoveryFactor.ToGUIFormat()
        tbDragCurve.Text = GetRelativePath(ihpcData.DragCurve.Source, Path.GetDirectoryName(_ihpcFilePath))

        SetVoltageLevelLow(ihpcData.VoltageLevels.First()) 
        SetVoltageLevelHigh(ihpcData.VoltageLevels.Last())

        LbStatus.Text = ""
        _changed = False
    End Sub

    Private Sub SetVoltageLevelLow(voltageLevel as IElectricMotorVoltageLevel)

        tbVoltage1.Text = voltageLevel.VoltageLevel.ToGUIFormat()
        tbContinuousTorque1.Text = voltageLevel.ContinuousTorque.ToGUIFormat()
        tbContinuousTorqueSpeed1.Text = voltageLevel.ContinuousTorqueSpeed.AsRPM.ToGUIFormat()
        tbOverloadTime1.Text = voltageLevel.OverloadTime.ToGUIFormat()
        tbOverloadTorque1.Text = voltageLevel.OverloadTorque.ToGUIFormat()
        tbOverloadTorqueSpeed1.Text = voltageLevel.OverloadTestSpeed.AsRPM.ToGUIFormat()
        tbFLCurve1.Text = GetRelativePath(voltageLevel.FullLoadCurve.Source, path.GetDirectoryName(_ihpcFilePath))
        SetPowerMapEntries(_lvPowerMap1, voltageLevel.PowerMap)

    End Sub

    Private Sub SetVoltageLevelHigh(voltageLevel as IElectricMotorVoltageLevel)

        tbVoltage2.Text = voltageLevel.VoltageLevel.ToGUIFormat()
        tbContinuousTorque2.Text = voltageLevel.ContinuousTorque.ToGUIFormat()
        tbContinuousTorqueSpeed2.Text = voltageLevel.ContinuousTorqueSpeed.AsRPM.ToGUIFormat()
        tbOverloadTime2.Text = voltageLevel.OverloadTime.ToGUIFormat()
        tbOverloadTorque2.Text = voltageLevel.OverloadTorque.ToGUIFormat()
        tbOverloadTorqueSpeed2.Text = voltageLevel.OverloadTestSpeed.AsRPM.ToGUIFormat()
        tbFLCurve2.Text = GetRelativePath(voltageLevel.FullLoadCurve.Source, Path.GetDirectoryName(_ihpcFilePath))
        SetPowerMapEntries(_lvPowerMap2, voltageLevel.PowerMap)

    End Sub

    Private Sub SetPowerMapEntries(powerMapListView As ListView, entries As IList(Of IElectricMotorPowerMap))
        For Each entry As IElectricMotorPowerMap In entries.OrderBy(Function(x) x.Gear)
            Dim listEntry = CreateListViewItem(entry.Gear, entry.PowerMap.Source)
            powerMapListView.Items.Add(listEntry)
        Next
    End Sub

    Private Function CreateListViewItem(axleNumber As Integer, filepath As String) As ListViewItem
        Dim retVal As New ListViewItem
        retVal.SubItems(0).Text = axleNumber.ToGUIFormat()
        retVal.SubItems.Add(GetRelativePath(filepath, GetPath(_ihpcFilePath)))
        Return retVal
    End Function
    
#End Region

    Public Sub ClearIHPC()

        tbModel.Text = ""
        tbInertia.Text = ""
        tbThermalOverload.Text = ""
        tbDragCurve.Text = ""

        tbVoltage1.Text = ""
        tbContinuousTorque1.Text = ""
        tbContinuousTorqueSpeed1.Text = ""
        tbOverloadTime1.Text = ""
        tbOverloadTorque1.Text = ""
        tbOverloadTorqueSpeed1.Text = ""
        tbFLCurve1.Text = ""
        RemoveAllListViewItems(_lvPowerMap1)

        tbVoltage2.Text = ""
        tbContinuousTorque2.Text = ""
        tbContinuousTorqueSpeed2.Text = ""
        tbOverloadTime2.Text = ""
        tbOverloadTorque2.Text = ""
        tbOverloadTorqueSpeed2.Text = ""
        tbFLCurve2.Text = ""
        RemoveAllListViewItems(_lvPowerMap2)
        
        LbStatus.Text = ""
        _changed = False

    End Sub

    Private Sub RemoveAllListViewItems(listView As ListView)
        If listView.Items.Count = 0 Then
            Exit Sub
        Else
            For Each listItem As ListViewItem In listView.Items
                listItem.Remove()
            Next
        End If
    End Sub

#Region "Events"

    Private Sub btDragCurve_Click(sender As Object, e As EventArgs) Handles btDragCurve.Click
        If IHPCDragCurveFileBrowser.OpenDialog(FileRepl(tbDragCurve.Text, GetPath(_ihpcFilePath)))
            _tbDragCurve.Text = GetFilenameWithoutDirectory(IHPCDragCurveFileBrowser.Files(0), GetPath(_ihpcFilePath))
        End If
    End Sub
    
    Private Sub btFLCurveFile1_Click(sender As Object, e As EventArgs) Handles btFLCurveFile1.Click
        If IHPCFullLoadCurveFileBrowser.OpenDialog(FileRepl(tbFLCurve1.Text, GetPath(_ihpcFilePath)))
            _tbFLCurve1.Text = GetFilenameWithoutDirectory(IHPCFullLoadCurveFileBrowser.Files(0), GetPath(_ihpcFilePath))
        End If
    End Sub
    
    Private Sub btFLCurveFile2_Click(sender As Object, e As EventArgs) Handles btFLCurveFile2.Click
        If IHPCFullLoadCurveFileBrowser.OpenDialog(FileRepl(tbFLCurve2.Text, GetPath(_ihpcFilePath)))
            _tbFLCurve2.Text = GetFilenameWithoutDirectory(IHPCFullLoadCurveFileBrowser.Files(0), GetPath(_ihpcFilePath))
        End If
    End Sub

    Private Sub btAddPowerMap1_Click(sender As Object, e As EventArgs) Handles btAddPowerMap1.Click
        AddListViewItem(lvPowerMap1)
    End Sub
    
    Private Sub btAddPowerMap2_Click(sender As Object, e As EventArgs) Handles btAddPowerMap2.Click
        AddListViewItem(lvPowerMap2)
    End Sub
    
    Private Sub AddListViewItem(listView As ListView)
        IHPCPowerMapInputDialog.Clear()
        dim gear = listView.Items.Count + 1
        IHPCPowerMapInputDialog.tbGear.Text = gear.ToString()

        If IHPCPowerMapInputDialog.ShowDialog() = DialogResult.OK Then
            Dim filePath = IHPCPowerMapInputDialog.tbInputFile.Text
            listView.Items.Add(CreateListViewItem(gear, filePath))
            if (listView.Equals(lvPowerMap1)) Then
                lvPowerMap2.Items.Add(CreateListViewItem(gear, ""))
            End If
            if (listView.Equals(_lvPowerMap2)) Then
                lvPowerMap1.Items.Add(CreateListViewItem(gear, ""))
            End If
            Change()
        End If
    End Sub

    Private Sub btRemovePowerMap1_Click(sender As Object, e As EventArgs) Handles btRemovePowerMap1.Click
        RemoveListEntry(lvPowerMap1)
        RemoveListEntry(lvPowerMap2)
    End Sub
    
    Private Sub btRemovePowerMap2_Click(sender As Object, e As EventArgs) Handles btRemovePowerMap2.Click
        RemoveListEntry(lvPowerMap2)
        RemoveListEntry(lvPowerMap1)
    End Sub

    Private Sub RemoveListEntry(listView As ListView)
        If listView.Items.Count = 0 Then
            Exit Sub
        Else
            listView.Items(listView.Items.Count - 1).Remove()
            Change()
        End If
    End Sub

    Private Sub lvPowerMap1_DoubleClick(sender As Object, e As EventArgs) Handles lvPowerMap1.DoubleClick
        EditEntry(lvPowerMap1)
    End Sub

    Private Sub lvPowerMap2_DoubleClick(sender As Object, e As EventArgs) Handles lvPowerMap2.DoubleClick
        EditEntry(lvPowerMap2)
    End Sub
    
    Private Sub EditEntry( listView As ListView)
        If listView.SelectedItems.Count = 0 Then Exit Sub
        Dim entry As ListViewItem = listView.SelectedItems(0)
        
        IHPCPowerMapInputDialog.tbGear.Text = entry.SubItems(0).Text
        IHPCPowerMapInputDialog.tbInputFile.Text = entry.SubItems(1).Text
        IHPCPowerMapInputDialog.tbGear.Focus()
        IHPCPowerMapInputDialog.IHPCPath = GetPath(_ihpcFilePath)

        If IHPCPowerMapInputDialog.ShowDialog() = DialogResult.OK Then
            entry.SubItems(0).Text = IHPCPowerMapInputDialog.tbGear.Text
            entry.SubItems(1).Text = IHPCPowerMapInputDialog.tbInputFile.Text
        End If

    End Sub
    
    Private Sub btSave_Click(sender As Object, e As EventArgs) Handles btSave.Click
        If SaveOrSaveAs(False) Then Close()
    End Sub

    Private Sub btCancel_Click(sender As Object, e As EventArgs) Handles btCancel.Click
        Close()
    End Sub

    Private Sub ToolStripBtNew_Click(sender As Object, e As EventArgs) Handles ToolStripBtNew.Click
        ClearIHPC()
    End Sub
    
    Private Sub ToolStripBtOpen_Click(sender As Object, e As EventArgs) Handles ToolStripBtOpen.Click
        If IHPCFileBrowser.OpenDialog(_ihpcFilePath) Then
            Try
                ReadIHPCFile(IHPCFileBrowser.Files(0))
            Catch ex As Exception
                MsgBox(ex.Message, MsgBoxStyle.OkOnly, $"Error loading IHPC(.{IHPCFileBrowser.Extensions}) File")
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

        If _ihpcFilePath= "" Then
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

        VehicleForm.tbIHPCFilePath.Text = GetFilenameWithoutDirectory(_ihpcFilePath, JobDir)
    End Sub


    Private Function ChangeCheckCancel() As Boolean
        If _changed Then
            Select Case MsgBox("Save changes ?", MsgBoxStyle.YesNoCancel)
                Case MsgBoxResult.Yes
                    Return Not SaveOrSaveAs(False)
                Case MsgBoxResult.Cancel
                    Return True
                Case Else
                    _changed = False
                    Return False
            End Select
        Else
            Return False
        End If
    End Function


    Private Function SaveOrSaveAs(ByVal saveAs As Boolean) As Boolean
        If ValidateData() = False Then _
            Return False

        If _ihpcFilePath = "" Or saveAs Then
            If IHPCFileBrowser.SaveDialog(_ihpcFilePath) Then
                _ihpcFilePath = IHPCFileBrowser.Files(0)
            Else
                Return False
            End If
        End If
        Return SaveIHPCToFile(_ihpcFilePath)
    End Function

    Private Function SaveIHPCToFile(ByVal ihpcFilePath As String) As Boolean
        Dim ihpcInputData = New IHPCInputData(ihpcFilePath)

        ihpcInputData.SetCommonEntries(tbModel.Text, tbInertia.Text, tbDragCurve.Text, tbThermalOverload.Text)
        ihpcInputData.SetVoltageLevelEntries(tbVoltage1.Text, tbContinuousTorque1.Text, tbContinuousTorqueSpeed1.Text,
                                             tbOverloadTime1.Text, tbOverloadTorque1.Text, tbOverloadTorqueSpeed1.Text,
                                             tbFLCurve1.Text, lvPowerMap1)
        ihpcInputData.SetVoltageLevelEntries(tbVoltage2.Text, tbContinuousTorque2.Text, tbContinuousTorqueSpeed2.Text,
                                             tbOverloadTime2.Text, tbOverloadTorque2.Text, tbOverloadTorqueSpeed2.Text,
                                             tbFLCurve2.Text, lvPowerMap2)


        If Not ihpcInputData.SaveFile() Then
            MsgBox("Cannot save to " & ihpcFilePath, MsgBoxStyle.Critical)
            Return False
        End If

        If not AutoSendTo is nothing Then
            If VehicleForm.Visible Then
                AutoSendTo(ihpcFilePath, VehicleForm)
            End If
        End If

        _changed = False
        LbStatus.Text = ""

        Return True
    End Function


#End Region

#Region "Validation"

    Private Function ValidateData() As Boolean

        If ValidateModel() = False Then Return False
        If ValidateInertia() = False Then Return False
        If ValidateThermalOverloadFactor() = False Then Return False
        If ValidateDragCurve() = False Then Return False

        If ValidateVoltage(tbVoltage1) = False Then Return False
        If ValidateContinuousTorque(tbContinuousTorque1) = False Then Return False
        If ValidateContinuousTorqueSpeed(tbContinuousTorqueSpeed1) = False Then Return False
        If ValidateOverloadTime(tbOverloadTime1) = False Then Return False
        If ValidateOverloadTorque(tbOverloadTime1) = False Then Return False
        If ValidateOverloadTorqueSpeed(tbOverloadTorqueSpeed1) = False Then Return False
        If ValidateFullLoadCurve(tbFLCurve1) = False Then Return False
        If ValidatePowerMapEntry(lvPowerMap1) = False Then Return False

        If ValidateVoltage(tbVoltage2) = False Then Return False
        If ValidateContinuousTorque(tbContinuousTorque2) = False Then Return False
        If ValidateContinuousTorqueSpeed(tbContinuousTorqueSpeed2) = False Then Return False
        If ValidateOverloadTime(tbOverloadTime2) = False Then Return False
        If ValidateOverloadTorque(tbOverloadTime2) = False Then Return False
        If ValidateOverloadTorqueSpeed(tbOverloadTorqueSpeed2) = False Then Return False
        If ValidateFullLoadCurve(tbFLCurve2) = False Then Return False
        If ValidatePowerMapEntry(lvPowerMap2) = False Then Return False

        Return True
    End Function

    Private Function ValidateModel() As Boolean
        If String.IsNullOrEmpty(tbModel.Text) Then
            ShowErrorMessageBox("Model", tbModel)
            Return False
        End If
        Return True
    End Function

    Private Function ValidateInertia() As Boolean
        If Not ValidDoubleValue(tbInertia.Text) Then
            ShowErrorMessageBox("Inertia", tbInertia)
            Return False
        End If
        Return True
    End Function

    Private Function ValidateThermalOverloadFactor() As Boolean
        If Not ValidDoubleValue(tbThermalOverload.Text) Then
            ShowErrorMessageBox("Thermal Overload Recovery Factor", tbThermalOverload)
            Return False
        End If
        Return True
    End Function

    Private Function ValidateDragCurve() As Boolean
        dim tmp = new SubPath()
		tmp.Init(GetPath(_ihpcFilePath), tbDragCurve.Text)
        If Not File.Exists(tmp.FullPath) Then
            ShowErrorMessageBox("No valid Drag Curve file path given", tbDragCurve, False)
            Return False
        End If

        Dim fileExtension = new FileInfo(tbDragCurve.Text).Extension
        If Not $".{IHPCDragCurveFileBrowser.Extensions.First()}" = fileExtension Then
            ShowErrorMessageBox($"The selected Drag Curve file(.{IHPCDragCurveFileBrowser.Extensions.First()}) has the wrong extension",
                                tbDragCurve, False)
            Return False		
        End If
        Return True
    End Function

    Private Function ValidateVoltage(tb As TextBox) As Boolean
        If Not ValidDoubleValue(tb.Text) Then
            ShowErrorMessageBox("Voltage", tb)
            Return False
        End If
        Return True
    End Function

    Private Function ValidateContinuousTorque(tb As TextBox) as Boolean
        If Not ValidDoubleValue(tb.Text) Then
            ShowErrorMessageBox("Continuous Torque", tb)
            Return False
        End If
        Return True
    End Function

    Private Function ValidateContinuousTorqueSpeed(tb As TextBox) As Boolean
        If Not ValidDoubleValue(tb.Text) Then
            ShowErrorMessageBox("Continuous Torque Speed", tb)
            Return False
        End If
        Return True
    End Function

    Private Function ValidateOverloadTime(tb As TextBox) As Boolean
        If Not ValidDoubleValue(tb.Text) Then
            ShowErrorMessageBox("Overload Time", tb)
            Return False
        End If
        Return True
    End Function

    Private Function ValidateOverloadTorque(tb As TextBox) As Boolean
        If Not ValidDoubleValue(tb.Text) Then
            ShowErrorMessageBox("Overload Torque", tb)
            Return False
        End If
        Return True
    End Function

    Private Function ValidateOverloadTorqueSpeed(tb As TextBox) As Boolean
        If Not ValidDoubleValue(tb.Text) Then
            ShowErrorMessageBox("Overload Torque Speed", tb)
            Return False
        End If
        Return True
    End Function
    
    Private Function ValidateFullLoadCurve(tb As TextBox) As Boolean
        dim tmp = new SubPath()
        tmp.Init(GetPath(_ihpcFilePath), tb.Text)
        If Not File.Exists(tmp.FullPath) Then
            ShowErrorMessageBox("No valid Full Load Curve file path given", tb, False)
            Return False
        End If

        Dim fileExtension = new FileInfo(tb.Text).Extension
        If Not $".{IHPCFullLoadCurveFileBrowser.Extensions.First()}" = fileExtension Then
            ShowErrorMessageBox($"The selected Full Load file(.{IHPCFullLoadCurveFileBrowser.Extensions.First()}) has the wrong extension",
                                tb, False)
            Return False		
        End If
        Return True
    End Function

    Private Function ValidatePowerMapEntry(tb As ListView) As Boolean
        If tb.Items.Count = 0 Then
            ShowErrorMessageBox("Invalid input missing Power Map files")
            Return False
        End If
        Return True
    End Function

    Private Function ValidDoubleValue(value As string) As Boolean
        If String.IsNullOrEmpty(value)
            Return false
        End If
        Return IsNumeric(value)
    End Function


    Private Sub ShowErrorMessageBox(message As string, textbox As TextBox, Optional defaultMessage As Boolean = True)
        If Not message = Nothing And defaultMessage Then
            MsgBox($"Invalid input for {message}")
            textbox.Focus()
            Return
        Else 
            MsgBox($"{message}")
            textbox.Focus()
            Return
        End If
    End Sub

    Private Sub ShowErrorMessageBox(message As string)
        If Not message = Nothing Then
            MsgBox($"{message}")
            Return
        End If
    End Sub
    
#End Region
    
#Region "Track Changes"

    Private Sub Change()
        If Not _changed Then
            LbStatus.Text = "Unsaved changes in current file"
            _changed  = True
        End If
    End Sub

    Private Sub tbModel_TextChanged(sender As Object, e As EventArgs) Handles tbModel.TextChanged
        Change()
    End Sub

    Private Sub tbInertia_TextChanged(sender As Object, e As EventArgs) Handles tbInertia.TextChanged
        Change()
    End Sub
    Private Sub tbThermalOverload_TextChanged(sender As Object, e As EventArgs) Handles tbThermalOverload.TextChanged
        Change()
    End Sub

    Private Sub tbDragCurve_TextChanged(sender As Object, e As EventArgs) Handles tbDragCurve.TextChanged
        Change()
    End Sub

    Private Sub tbVoltage1_TextChanged(sender As Object, e As EventArgs) Handles tbVoltage1.TextChanged
        Change()
    End Sub

    Private Sub tbContinuousTorque1_TextChanged(sender As Object, e As EventArgs) Handles tbContinuousTorque1.TextChanged
        Change()
    End Sub

    Private Sub tbContinuousTorqueSpeed1_TextChanged(sender As Object, e As EventArgs) Handles tbContinuousTorqueSpeed1.TextChanged
        Change()
    End Sub
    
    Private Sub tbOverloadTime1_TextChanged(sender As Object, e As EventArgs) Handles tbOverloadTime1.TextChanged
        Change()
    End Sub

    Private Sub tbOverloadTorque1_TextChanged(sender As Object, e As EventArgs) Handles tbOverloadTorque1.TextChanged
        Change()
    End Sub

    Private Sub tbOverloadTorqueSpeed1_TextChanged(sender As Object, e As EventArgs) Handles tbOverloadTorqueSpeed1.TextChanged
        Change()
    End Sub
    
    Private Sub tbFLCurve1_TextChanged(sender As Object, e As EventArgs) Handles tbFLCurve1.TextChanged
        Change()
    End Sub
    
    Private Sub tbVoltage2_TextChanged(sender As Object, e As EventArgs) Handles tbVoltage2.TextChanged
        Change()
    End Sub

    Private Sub tbContinuousTorque2_TextChanged(sender As Object, e As EventArgs) Handles tbContinuousTorque2.TextChanged
        Change()
    End Sub

    Private Sub tbContinuousTorqueSpeed2_TextChanged(sender As Object, e As EventArgs) Handles tbContinuousTorqueSpeed2.TextChanged
        Change()
    End Sub
    
    Private Sub tbFLCurve2_TextChanged(sender As Object, e As EventArgs) Handles tbFLCurve2.TextChanged
        Change()
    End Sub

    Private Sub tbOverloadTime2_TextChanged(sender As Object, e As EventArgs) Handles tbOverloadTime2.TextChanged
        Change()
    End Sub

    Private Sub tbOverloadTorque2_TextChanged(sender As Object, e As EventArgs) Handles tbOverloadTorque2.TextChanged
        Change()
    End Sub

    Private Sub tbOverloadTorqueSpeed2_TextChanged(sender As Object, e As EventArgs) Handles tbOverloadTorqueSpeed2.TextChanged
        Change()
    End Sub

    Private Sub IHPCForm_FormClosing(sender As Object, e As FormClosingEventArgs) Handles MyBase.FormClosing
        If e.CloseReason <> CloseReason.ApplicationExitCall And e.CloseReason <> CloseReason.WindowsShutDown Then
            e.Cancel = ChangeCheckCancel()
        End If
    End Sub

    Private Sub ToolStripButton1_Click(sender As Object, e As EventArgs) Handles ToolStripButton1.Click
        If File.Exists(Path.Combine(MyAppPath, "User Manual\help.html")) Then
            Dim defaultBrowserPath As String = BrowserUtils.GetDefaultBrowserPath()
            Process.Start(defaultBrowserPath,
                          $"""file://{Path.Combine(MyAppPath, "User Manual\help.html#ihpc-editor")}""")
        Else
            MsgBox("User Manual not found!", MsgBoxStyle.Critical)
        End If
    End Sub
    Private Sub btDragCurveOpen_Click(sender As Object, e As EventArgs) Handles btDragCurveOpen.Click
        Dim theFile = FileRepl(tbDragCurve.Text, GetPath(_ihpcFilePath))

        If theFile <> NoFile AndAlso File.Exists(theFile) Then
            OpenFiles(theFile)
        End If
    End Sub

    Private Sub btFLCurve1_Click(sender As Object, e As EventArgs) Handles btFLCurve1.Click
        Dim theFile = FileRepl(tbFLCurve1.Text, GetPath(_ihpcFilePath))

        If theFile <> NoFile AndAlso File.Exists(theFile) Then
            OpenFiles(theFile)
        End If
    End Sub
    
    Private Sub btFLCurve2_Click(sender As Object, e As EventArgs) Handles btFLCurve2.Click
        Dim theFile = FileRepl(tbFLCurve1.Text, GetPath(_ihpcFilePath))

        If theFile <> NoFile AndAlso File.Exists(theFile) Then
            OpenFiles(theFile)
        End If
    End Sub
    

    Private Sub OpenFiles(ParamArray files() As String)

        If files.Length = 0 Then Exit Sub

        _contextMenuFiles = files

        OpenWithToolStripMenuItem.Text = "Open with " & Cfg.OpenCmdName

        CmOpenFile.Show(Windows.Forms.Cursor.Position)
    End Sub

    Private Sub OpenWithToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles OpenWithToolStripMenuItem.Click
        If Not FileOpenAlt(_contextMenuFiles(0)) Then MsgBox("Failed to open file!")
    End Sub

    Private Sub ShowInFolderToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ShowInFolderToolStripMenuItem.Click

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
End Class