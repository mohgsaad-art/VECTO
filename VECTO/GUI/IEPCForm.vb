Imports System.IO
Imports TUGraz.VECTO.Input_Files
Imports TUGraz.VectoCommon.InputData
Imports TUGraz.VectoCommon.Utils
Imports TUGraz.VectoCore.InputData.FileIO.JSON

Public Class IEPCForm

	Public JobDir As String = ""
	Private _iepcFilePath as String = ""
	Private _powerMapDlg As IEPCInputDialog
	Private _dragCurveDlg As IEPCInputDialog
	Private _gearDlg As IEPCGearInputDialog
	Private _flcFilePath1 as String
	Private _flcFilePath2 as String
	Private _changed as Boolean

    Public AutoSendTo As action(of string, VehicleForm)
	
	Private _contextMenuFiles As String()

	
	Private Sub IEPCForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load
		_powerMapDlg = New IEPCInputDialog(IEPCDialogType.PowerMapDialog)
		_dragCurveDlg = New IEPCInputDialog(IEPCDialogType.DragCurveDialog)
		_gearDlg = New IEPCGearInputDialog()
	End Sub

#Region "Set IEPC Data"

	Public Sub ReadIEPCFile(file As String)
		Dim inputData = JSONInputDataFactory.ReadIEPCEngineeringInputData(file, True)
	    _iepcFilePath = file
		tbModel.Text = inputData.Model
		tbInertia.Text = inputData.Inertia.ToGUIFormat()
		cbDifferentialIncluded.Checked = inputData.DifferentialIncluded
		cbDesignTypeWheelMotor.Checked = inputData.DesignTypeWheelMotor
		tbNumberOfDesignTypeWheelMotor.Text = inputData.NrOfDesignTypeWheelMotorMeasured.Value.ToGUIFormat()
		tbThermalOverload.Text = inputData.OverloadRecoveryFactor.ToGUIFormat()

		Dim voltageLevel = inputData.VoltageLevels.First()
		SetFirstVoltageLevel(voltageLevel)
		voltageLevel = inputData.VoltageLevels.Last()
		SetSecondaryVoltageLevel(voltageLevel)
		SetGearEntries(inputData.Gears)
		SetDragEntries(inputData.DragCurves)
		

		LbStatus.Text = ""
		_changed = False
	End Sub

	Private Sub SetFirstVoltageLevel(voltageLevel As IElectricMotorVoltageLevel)
		tbVoltage1.Text = voltageLevel.VoltageLevel.ToGUIFormat()
		tbContinousTorque1.Text = voltageLevel.ContinuousTorque.ToGUIFormat()
		tbContinousTorqueSpeed1.Text = voltageLevel.ContinuousTorqueSpeed.AsRPM.ToGUIFormat()
		tbOverloadTime1.Text = voltageLevel.OverloadTime.ToGUIFormat()
		tbOverloadTorque1.Text = voltageLevel.OverloadTorque.ToGUIFormat()
		tboverloadTorqueSpeed1.Text = voltageLevel.OverloadTestSpeed.AsRPM.ToGUIFormat()
		tbFLCurve1.Text = GetRelativePath(voltageLevel.FullLoadCurve.Source, Path.GetDirectoryName(_iepcFilePath))
		SetPowerMapEntries(_lvPowerMap1, voltageLevel.PowerMap)
	End Sub

	Private Sub SetSecondaryVoltageLevel(voltageLevel As IElectricMotorVoltageLevel)
		tbVoltage2.Text = voltageLevel.VoltageLevel.ToGUIFormat()
		tbContinousTorque2.Text = voltageLevel.ContinuousTorque.ToGUIFormat()
		tbContinousTorqueSpeed2.Text = voltageLevel.ContinuousTorqueSpeed.AsRPM.ToGUIFormat()
		tbOverloadTime2.Text = voltageLevel.OverloadTime.ToGUIFormat()
		tbOverloadTorque2.Text = voltageLevel.OverloadTorque.ToGUIFormat()
		tbOverloadTorqueSpeed2.Text = voltageLevel.OverloadTestSpeed.AsRPM.ToGUIFormat()
		tbFLCurve2.Text = GetRelativePath(voltageLevel.FullLoadCurve.Source, Path.GetDirectoryName(_iepcFilePath))
		SetPowerMapEntries(_lvPowerMap2, voltageLevel.PowerMap)
	End Sub

	Private Sub SetGearEntries(entries As IList(Of IGearEntry))
		For Each entry As IGearEntry In entries
			Dim listEntry = CreateListViewItem(entry.Ratio, entry.MaxOutputShaftTorque, entry.MaxOutputShaftSpeed)
			_lvGear.Items.Add(listEntry)
		Next
	End Sub

	Private Sub SetDragEntries(entries As IList(Of IDragCurve))
		For Each entry As IDragCurve In entries
			Dim listEntry = CreateListViewItem(entry.Gear.Value, entry.DragCurve.Source)
			_lvDragCurve.Items.Add(listEntry)
		Next
	End Sub

	Private Sub SetPowerMapEntries(powerMapListView As ListView, entries As IList(Of IElectricMotorPowerMap))
		For Each entry As IElectricMotorPowerMap In entries
			Dim listEntry = CreateListViewItem(entry.Gear, entry.PowerMap.Source)
			powerMapListView.Items.Add(listEntry)
		Next
	End Sub

	Private Function CreateListViewItem(axleNumber As Integer, filepath As String) As ListViewItem
	    Dim basePath As String = GetPath(_iepcFilePath)
	    Dim retVal As New ListViewItem
		retVal.SubItems(0).Text = axleNumber.ToGUIFormat()
        retVal.SubItems.Add(GetRelativePath(filepath, basePath))
        'retVal.SubItems.Add(filepath)
        Return retVal
	End Function

	Private Function CreateListViewItem(ratio As Double, outputShaftTorque As NewtonMeter, outputShaftSpeed As PerSecond) As ListViewItem
		Dim retVal As New ListViewItem
		retVal.SubItems(0).Text = ratio.ToGUIFormat()
		If Not outputShaftTorque = Nothing Then
			retVal.SubItems.Add(outputShaftTorque.ToGUIFormat())
		Else 
			retVal.SubItems.Add(String.Empty)
		End If

		If Not outputShaftSpeed = Nothing Then
			retVal.SubItems.Add(outputShaftSpeed.ToGUIFormat())
		Else 
			retVal.SubItems.Add(String.Empty)
		End If
		
		Return retVal
	End Function

	Private Function CreateListViewItem(ratio As Double, outputShaftTorque As Double?, outputShaftSpeed As Double?) As ListViewItem

		Dim retVal As New ListViewItem
		retVal.SubItems(0).Text = ratio.ToGUIFormat()

		If outputShaftTorque.HasValue Then
			retVal.SubItems.Add(outputShaftTorque.Value.ToGUIFormat())
		Else
			retVal.SubItems.Add(String.Empty)
		End If
		If outputShaftSpeed.HasValue Then
			retVal.SubItems.Add(outputShaftSpeed.Value.ToGUIFormat())
		Else
			retVal.SubItems.Add(String.Empty)
		End If
		Return retVal

	End Function

#End Region

	Private Sub btAddDragCurve_Click(sender As Object, e As EventArgs) Handles btAddDragCurve.Click
		AddListViewItem(_dragCurveDlg, _lvDragCurve)
	End Sub

	Private Sub AddListViewItem(dialog As IEPCInputDialog, listView As ListView)
		dialog.Clear()
		If dialog.ShowDialog() = DialogResult.OK Then
			Dim gear = Convert.ToInt32(dialog.tbGear.Text)
			Dim filePath = dialog.tbInputFile.Text
			listView.Items.Add(CreateListViewItem(gear, filePath))
			Change()
		End If
	End Sub

	Private Sub btRemoveDragCurve_Click(sender As Object, e As EventArgs) Handles btRemoveDragCurve.Click
		RemoveListEntry(_lvDragCurve)
	End Sub
	
	Private Sub btRemoveGear_Click(sender As Object, e As EventArgs) Handles btRemoveGear.Click
		RemoveListEntry(_lvGear)
		RemoveListEntry(_lvPowerMap1)
		RemoveListEntry(_lvPowerMap2)
	End Sub

	Private Sub RemoveListEntry(listView As ListView)
		If listView.Items.Count = 0 Then
			Exit Sub
		Else
			listView.Items(listView.Items.Count - 1).Remove()
			Change()
		End If
	End Sub

	Private Sub btAddGear_Click(sender As Object, e As EventArgs) Handles btAddGear.Click
		_gearDlg.Clear()

		If _gearDlg.ShowDialog() = DialogResult.OK Then
			Dim ratio = Convert.ToDouble(_gearDlg.tbRatio.Text)
			Dim outputShaftTorque As Double?
			Dim outputShaftSpeed As Double?
			If _gearDlg.tbMaxOutShaftTorque.Text.Length > 0 Then
				outputShaftTorque = _gearDlg.tbMaxOutShaftTorque.Text.ToDouble(0)
			End If
			If _gearDlg.tbMaxOutShaftSpeed.Text.Length > 0 Then
				outputShaftSpeed = _gearDlg.tbMaxOutShaftSpeed.Text.ToDouble(0)
			End If

			Dim entry = CreateListViewItem(ratio, outputShaftTorque, outputShaftSpeed)
			_lvGear.Items.Add(entry)
			AddPowerMapEntry(lvPowerMap1, _lvGear.Items.Count)
			AddPowerMapEntry(lvPowerMap2, _lvGear.Items.Count)
		
			Change()
		End If
	End Sub

	Private Sub AddPowerMapEntry(powerMapListView As ListView, gearIndex As Integer)
		Dim retVal As New ListViewItem
		retVal.SubItems(0).Text = gearIndex.ToString()
		retVal.SubItems.Add(String.Empty)
		powerMapListView.Items.Add(retVal)
	End Sub
	
	Private Sub lvGear_DoubleClick(sender As Object, e As EventArgs) Handles lvGear.DoubleClick

		If lvGear.SelectedItems.Count = 0 Then Exit Sub

		Dim entry As ListViewItem = lvGear.SelectedItems(0)

		_gearDlg.tbRatio.Text = entry.SubItems(0).Text
		 _gearDlg.tbMaxOutShaftTorque.Text = entry.SubItems(1).Text
		 _gearDlg.tbMaxOutShaftSpeed.Text = entry.SubItems(2).Text
		_gearDlg.tbRatio.Focus()

		If _gearDlg.ShowDialog() = DialogResult.OK Then
			entry.SubItems(0).Text = _gearDlg.tbRatio.Text
			entry.SubItems(1).Text = _gearDlg.tbMaxOutShaftTorque.Text
			entry.SubItems(2).Text = _gearDlg.tbMaxOutShaftSpeed.Text
			Change()
		End If

	End Sub
	
	Private Sub lvDragCurve_DoubleClick(sender As Object, e As EventArgs) Handles lvDragCurve.DoubleClick 
		EditEntry(_dragCurveDlg, lvDragCurve)
	End Sub

	Private Sub lvPowerMap1_DoubleClick(sender As Object, e As EventArgs) Handles lvPowerMap1.DoubleClick
	    _powerMapDlg.tbGear.Enabled = False
		EditEntry(_powerMapDlg, lvPowerMap1)
	End Sub

	Private Sub lvPowerMap2_DoubleClick(sender As Object, e As EventArgs) Handles lvPowerMap2.DoubleClick 
		_powerMapDlg.tbGear.Enabled = False
		EditEntry(_powerMapDlg, lvPowerMap2)
	End Sub

	Private Sub EditEntry(dialog As IEPCInputDialog, listView As ListView)
		
		If listView.SelectedItems.Count = 0 Then Exit Sub
		dialog.Clear()

		Dim entry As ListViewItem = listView.SelectedItems(0)
		dialog.tbGear.Text = entry.SubItems(0).Text
		dialog.tbInputFile.Text = entry.SubItems(1).Text
		dialog.tbGear.Focus()
		dialog.IEPCPath = GetPath(_iepcFilePath)

		If dialog.ShowDialog() = DialogResult.OK Then
			entry.SubItems(0).Text = dialog.tbGear.Text
		    entry.SubItems(1).Text = dialog.tbInputFile.Text
		End If

	End Sub

	Private Sub btFLCurveFile1_Click(sender As Object, e As EventArgs) Handles btFLCurveFile1.Click
		If IEPCFLCFileBrowser.OpenDialog(FileRepl(tbFLCurve1.Text, GetPath(_iepcFilePath))) Then
			tbFLCurve1.Text = GetFilenameWithoutDirectory(IEPCFLCFileBrowser.Files(0), GetPath(_iepcFilePath))
		End If
	End Sub

	Private Sub btFLCurveFile2_Click(sender As Object, e As EventArgs) Handles btFLCurveFile2.Click
		If IEPCFLCFileBrowser.OpenDialog(FileRepl(tbFLCurve2.Text, GetPath(_iepcFilePath))) Then
			tbFLCurve2.Text = GetFilenameWithoutDirectory(IEPCFLCFileBrowser.Files(0), GetPath(_iepcFilePath))
		End If
	End Sub

	Private Sub ToolStripBtNew_Click(sender As Object, e As EventArgs) Handles ToolStripBtNew.Click
		NewIEPC()
	End Sub

	Private Sub ToolStripBtOpen_Click(sender As Object, e As EventArgs) Handles ToolStripBtOpen.Click
		If IEPCFileBrowser.OpenDialog(_iepcFilePath) Then
			Try
				ReadIEPCFile(IEPCFileBrowser.Files(0))
			Catch ex As Exception
				MsgBox(ex.Message, MsgBoxStyle.OkOnly, $"Error loading IEPC(.{IEPCFileBrowser.Extensions}) File")
			End Try
		End If
	End Sub
	
	#Region "Toolbar"

	Public Sub NewIEPC()
		tbModel.Text = ""
		tbInertia.Text = ""
		cbDifferentialIncluded.Checked = False
		cbDesignTypeWheelMotor.Checked = False
		tbNumberOfDesignTypeWheelMotor.Text = ""
		tbThermalOverload.Text = ""

		tbVoltage1.Text = ""
		tbContinousTorque1.Text = ""
		tbContinousTorqueSpeed1.Text = ""
		tbOverloadTime1.Text = ""
		tbOverloadTorque1.Text = ""
		tboverloadTorqueSpeed1.Text = ""
		tbFLCurve1.Text = ""
		_flcFilePath1 = ""
		RemoveAllListViewItems(lvPowerMap1)
		
		tbVoltage2.Text = ""
		tbContinousTorque2.Text = ""
		tbContinousTorqueSpeed2.Text = ""
		tbOverloadTime2.Text = ""
		tbOverloadTorque2.Text = ""
		tboverloadTorqueSpeed2.Text = ""
		tbFLCurve2.Text = ""
		_flcFilePath2 = ""
		RemoveAllListViewItems(lvPowerMap2)
		
		RemoveAllListViewItems(lvDragCurve)
		RemoveAllListViewItems(lvGear)
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

	Private Sub ToolStripBtSave_Click(sender As Object, e As EventArgs) Handles ToolStripBtSave.Click
		SaveOrSaveAs(False)
	End Sub

	Private Sub ToolStripBtSaveAs_Click(sender As Object, e As EventArgs) Handles ToolStripBtSaveAs.Click
		SaveOrSaveAs(True)
	End Sub

	Private Sub ToolStripBtSendTo_Click(sender As Object, e As EventArgs) Handles ToolStripBtSendTo.Click
		If ChangeCheckCancel() Then Exit Sub

		If _iepcFilePath = "" Then
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

		VehicleForm.tbIEPCFilePath.Text = GetFilenameWithoutDirectory(_iepcFilePath, JobDir)
	End Sub

	Private Sub ToolStripButton1_Click(sender As Object, e As EventArgs) Handles ToolStripButton1.Click
		If File.Exists(Path.Combine(MyAppPath, "User Manual\help.html")) Then
			Dim defaultBrowserPath As String = BrowserUtils.GetDefaultBrowserPath()
			Process.Start(defaultBrowserPath,
						  $"""file://{Path.Combine(MyAppPath, "User Manual\help.html#iepc-editor")}""")
		Else
			MsgBox("User Manual not found!", MsgBoxStyle.Critical)
		End If
	End Sub
	
	#End Region
	
	#Region "Save Methods"
	
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

		If _iepcFilePath = "" Or saveAs Then
			If IEPCFileBrowser.SaveDialog(_iepcFilePath) Then
				_iepcFilePath = IEPCFileBrowser.Files(0)
			Else
				Return False
			End If
		End If
		Return SaveIEPCToFile(_iepcFilePath)
	End Function
	
	Private Function SaveIEPCToFile(ByVal file As String) As Boolean
		Dim iepc = New IEPCInputData(file)

		iepc.SetCommonEntries(tbModel.Text, tbInertia.Text, cbDesignTypeWheelMotor.Checked, 
							  tbNumberOfDesignTypeWheelMotor.Text, cbDifferentialIncluded.Checked,
							  tbThermalOverload.Text)
		
		iepc.SetVoltageLevelEntries(tbVoltage1.Text, tbContinousTorque1.Text, tbContinousTorqueSpeed1.Text,
									tbOverloadTime1.Text, tbOverloadTorque1.Text, tboverloadTorqueSpeed1.Text, 
									tbFLCurve1.Text, lvPowerMap1)

		iepc.SetVoltageLevelEntries(tbVoltage2.Text, tbContinousTorque2.Text, tbContinousTorqueSpeed2.Text,
									tbOverloadTime2.Text, tbOverloadTorque2.Text, tboverloadTorqueSpeed2.Text, 
									tbFLCurve2.Text, lvPowerMap2)

		iepc.SetGearsEntries(lvGear)
		iepc.SetDragCurveEntries(lvDragCurve)


		If Not iepc.SaveFile Then
			MsgBox("Cannot save to " & file, MsgBoxStyle.Critical)
			Return False
		End If

	    If not AutoSendTo is nothing Then
	        If VehicleForm.Visible Then
	            AutoSendTo(file, VehicleForm)
	        End If
	    End If

		_changed = False
		LbStatus.Text = ""

		Return True
	End Function


	#End Region



	Private Sub ButOK_Click(sender As Object, e As EventArgs) Handles ButOK.Click
		If SaveOrSaveAs(False) Then Close()
	End Sub

	Private Sub ButCancel_Click(sender As Object, e As EventArgs) Handles ButCancel.Click
		Close()
	End Sub


#Region "Validate Input"

	Private Function ValidateData() As Boolean

		If ValidateModel() = False Then Return False
		If ValidateInertia() = False Then Return False
		If ValidateNrDesignTypeWheelMotorMeasured() = False Then Return False
		If ValidateOverloadRecoveryFactor() = False Then Return False

		If ValidateVoltage(tbVoltage1) = False Then Return False
		If ValidateContinuousTorque(tbContinousTorque1) = False Then Return False
		If ValidateContinuousTorqueSpeed(tbContinousTorqueSpeed1) = False Then Return False
		If ValidateOverloadTime(tbOverloadTime1) = False Then Return False
		If ValidateOverloadTorque(tbOverloadTorque1) = False Then Return False
		If ValidateOverloadTorqueSpeed(tboverloadTorqueSpeed1) = False Then Return False
		
		If ValidateVoltage(tbVoltage2) = False Then Return False
		If ValidateContinuousTorque(tbContinousTorque2) = False Then Return False
		If ValidateContinuousTorqueSpeed(tbContinousTorqueSpeed2) = False Then Return False
		If ValidateOverloadTime(tbOverloadTime2) = False Then Return False
		If ValidateOverloadTorque(tbOverloadTorque2) = False Then Return False
		If ValidateOverloadTorqueSpeed(tboverloadTorqueSpeed2) = False Then Return False

		If ValidateAmountOfEntries() = False Then Return False
		If ValidateFullLoadCurve1() = False Then Return False
		If ValidateFullLoadCurve2() = False Then Return False

		If ValidatePowerMapEntries(_lvPowerMap1) = False Then Return False
		If ValidatePowerMapEntries(_lvPowerMap2) = False Then Return False

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

	Private Function ValidateNrDesignTypeWheelMotorMeasured() As Boolean 
		If Not cbDesignTypeWheelMotor.Checked AND Not ValidDoubleValue(tbNumberOfDesignTypeWheelMotor.Text) Then
			ShowErrorMessageBox("Nr of Design Type Wheel Motor Measured", tbNumberOfDesignTypeWheelMotor)
			Return False
		End If
		Return True
	End Function

	Private Function ValidateOverloadRecoveryFactor() As Boolean
		If Not ValidDoubleValue(tbThermalOverload.Text) Then
			ShowErrorMessageBox("Thermal Overload Recovery Factor", tbThermalOverload)
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

	Private Function ValidateContinuousTorque(tb As TextBox) As Boolean
		If Not ValidDoubleValue(tb.Text) Then
			ShowErrorMessageBox("Continuous Torque", tb)
			Return False
		End If
		Return True
	End Function

	Private Function ValidateContinuousTorqueSpeed(tb As TextBox) As Boolean
		If Not ValidDoubleValue(tb.Text) Then
			ShowErrorMessageBox("Continuous Torque", tb)
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
	
	Private Function ValidateAmountOfEntries() As Boolean
		If _lvGear.Items.Count = 0 Then
			ShowErrorMessageBox("Invalid input no Gear given")
			Return False
		End If

		If Not _lvPowerMap1.Items.Count = _lvGear.Items.Count Then
			ShowErrorMessageBox("Invalid number of Power Map entries at Voltage Level Low")
			Return False
		End If
		
		If Not _lvPowerMap2.Items.Count = _lvGear.Items.Count Then
			ShowErrorMessageBox("Invalid number of Power Map entries at Voltage Level High")
			Return False
		End If

		If _lvDragCurve.Items.Count = 0 
			ShowErrorMessageBox("Invalid input no Drag Curve given")
			Return False
		End If

		If Not _lvDragCurve.Items.Count = _lvGear.Items.Count And _lvDragCurve.Items.Count > 1
			ShowErrorMessageBox("Invalid numbers of Drag Curves given")
			Return False
		End If

		Return True
	End Function

	Private Function ValidateFullLoadCurve1() As Boolean
		dim tmp = new SubPath()
		tmp.Init(GetPath(_iepcFilePath), tbFLCurve1.Text)
		If Not File.Exists(tmp.FullPath) Then
			ShowErrorMessageBox("No valid file path given", tbFLCurve1, False)
			Return False
		End If
		
		Dim fileExtension = new FileInfo(tbFLCurve1.Text).Extension
		If Not $".{IEPCFLCFileBrowser.Extensions.First()}" = fileExtension Then
			ShowErrorMessageBox($"The selected Full Load Curve file(.{IEPCFLCFileBrowser.Extensions.First()}) has the wrong extension",
								tbFLCurve1, False)
			Return False		
		End If
		Return True
	End Function

	Private Function ValidateFullLoadCurve2() As Boolean
	    dim tmp = new SubPath()
	    tmp.Init(GetPath(_iepcFilePath), tbFLCurve2.Text)
		If Not File.Exists(tmp.FullPath) Then
			ShowErrorMessageBox("Invalid input no valid file path given", tbFLCurve2, False)
			Return False
		End If
		
		Dim fileExtension = new FileInfo(tbFLCurve2.Text).Extension
		If Not $".{IEPCFLCFileBrowser.Extensions.First()}" = fileExtension Then
			ShowErrorMessageBox($"The selected Full Load Curve file(.{IEPCFLCFileBrowser.Extensions.First()}) has the wrong file extension",
								tbFLCurve2, False)
			Return False		
		End If
		Return True
	End Function

	Private Function ValidatePowerMapEntries(powerMap as ListView) As Boolean

		For Each entry As ListViewItem In powerMap.Items
			If entry.SubItems.Count = 1 Then
				ShowErrorMessageBox("Invalid input missing Power Map files")
				Return False
			End If

			If entry.SubItems.Count = 2 Then
				If  String.IsNullOrEmpty(entry.SubItems(1).Text)
				    ShowErrorMessageBox($"Missing Power Map file entry at Gear {entry.SubItems(0).Text}")
					Return False
				End If
			    
			    Dim fileExtension = new FileInfo(entry.SubItems(1).Text).Extension
				If Not $".{IEPCPowerMapFileBrowser.Extensions.First()}" = fileExtension
					ShowErrorMessageBox($"The selected Power Map file(.{IEPCPowerMapFileBrowser.Extensions.First()}) has the wrong file extension")
					Return False
				End If
			End If
		Next
		Return True

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
	
	Private Function ValidDoubleValue(value As string) As Boolean
		If String.IsNullOrEmpty(value)
			Return false
		End If
		Return IsNumeric(value)
	End Function
   
#End Region
	
#Region "Track changes"

	Private Sub Change()
		If Not _changed Then
			LbStatus.Text = "Unsaved changes in current file"
			_changed = True
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

	Private Sub cbDifferentialIncluded_CheckedChanged(sender As Object, e As EventArgs) Handles cbDifferentialIncluded.CheckedChanged
		Change()
	End Sub

	Private Sub cbDesignTypeWheelMotor_CheckedChanged(sender As Object, e As EventArgs) Handles cbDesignTypeWheelMotor.CheckedChanged
		tbNumberOfDesignTypeWheelMotor.Enabled = cbDesignTypeWheelMotor.Checked
		cbDifferentialIncluded.Enabled = Not cbDesignTypeWheelMotor.Checked
		if (cbDesignTypeWheelMotor.Checked) then
		    cbDifferentialIncluded.Checked = False
		end if
		If tbNumberOfDesignTypeWheelMotor.Enabled = False Then _
			tbNumberOfDesignTypeWheelMotor.Text = "0"
		Change()
	End Sub

	Private Sub tbNumberOfDesignTypeWheelMotor_TextChanged(sender As Object, e As EventArgs) Handles tbNumberOfDesignTypeWheelMotor.TextChanged
		Change()
	End Sub

	Private Sub tbVoltage1_TextChanged(sender As Object, e As EventArgs) Handles tbVoltage1.TextChanged
		Change()
	End Sub

	Private Sub tbContinousTorque1_TextChanged(sender As Object, e As EventArgs) Handles tbContinousTorque1.TextChanged
		Change()
	End Sub

	Private Sub tbContinousTorqueSpeed1_TextChanged(sender As Object, e As EventArgs) Handles tbContinousTorqueSpeed1.TextChanged
		Change()
	End Sub

	Private Sub tbOverloadTime1_TextChanged(sender As Object, e As EventArgs) Handles tbOverloadTime1.TextChanged
		Change()
	End Sub

	Private Sub tbOverloadTorque1_TextChanged(sender As Object, e As EventArgs) Handles tbOverloadTorque1.TextChanged
		Change()
	End Sub
	
	Private Sub tboverloadTorqueSpeed1_TextChanged(sender As Object, e As EventArgs) Handles tboverloadTorqueSpeed1.TextChanged
		Change()
	End Sub

	Private Sub tbFLCurve1_TextChanged(sender As Object, e As EventArgs) Handles tbFLCurve1.TextChanged
		Change()
	End Sub

	Private Sub tbVoltage2_TextChanged(sender As Object, e As EventArgs) Handles tbVoltage2.TextChanged
		Change()
	End Sub

	Private Sub tbContinousTorque2_TextChanged(sender As Object, e As EventArgs) Handles tbContinousTorque2.TextChanged
		Change()
	End Sub

	Private Sub tbContinousTorqueSpeed2_TextChanged(sender As Object, e As EventArgs) Handles tbContinousTorqueSpeed2.TextChanged
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

	Private Sub tbFLCurve2_TextChanged(sender As Object, e As EventArgs) Handles tbFLCurve2.TextChanged
		Change()
	End Sub

	Private Sub IEPCForm_FormClosing(sender As Object, e As FormClosingEventArgs) Handles MyBase.FormClosing
		If e.CloseReason <> CloseReason.ApplicationExitCall And e.CloseReason <> CloseReason.WindowsShutDown Then
			e.Cancel = ChangeCheckCancel()
		End If
	End Sub

 

#End Region

	Private Sub btShowFLCurve1_Click(sender As Object, e As EventArgs) Handles btShowFLCurve1.Click
		Dim theFile = FileRepl(tbFLCurve1.Text, GetPath(_iepcFilePath))

		If theFile <> NoFile AndAlso File.Exists(theFile) Then
			OpenFiles(theFile)
		End If
	End Sub
	
	Private Sub btShowFLCurve2_Click(sender As Object, e As EventArgs) Handles btShowFLCurve2.Click
		Dim theFile = FileRepl(tbFLCurve2.Text, GetPath(_iepcFilePath))

		If theFile <> NoFile AndAlso File.Exists(theFile) Then
			OpenFiles(theFile)
		End If
	End Sub
	
	Private Sub OpenFiles(ParamArray files() As String)

		If files.Length = 0 Then Exit Sub

		_contextMenuFiles = files

		OpenWithToolStripMenuItem.Text = "Open with " & Cfg.OpenCmdName

		CmOpenFile.Show(Cursor.Position)
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

End Class