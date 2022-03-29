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
Imports System.IO
Imports System.Linq
Imports System.Text.RegularExpressions
Imports TUGraz.VectoCommon.InputData
Imports TUGraz.VectoCommon.Models
Imports TUGraz.VectoCommon.Utils
Imports TUGraz.VectoCore.InputData.FileIO.JSON
Imports TUGraz.VectoCore.InputData.Impl
Imports TUGraz.VectoCore.Models.Declaration

''' <summary>
''' Vehicle Editor.
''' </summary>
Public Class VehicleForm
	Private Enum AxleTbl
		AxleNumber = 0
		RelativeLoad = 1
		TwinTyres = 2
		RRC = 3
		FzISO = 4
		WheelsDimension = 5
		Inertia = 6
		AxleType = 7
	End Enum

	Private Enum TorqueLimitsTbl
		Gear = 0
		MaxTorque = 1
	End Enum

	Private _axlDlog As VehicleAxleDialog
	Private _hdVclass As String
	Private _vehFile As String
	Private _changed As Boolean = False
	Private _cmFiles As String()

	Public AutoSendTo As Boolean = False
	Public JobDir As String = ""
	Private _torqueLimitDlog As VehicleTorqueLimitDialog
    Public VehicleType As VectoSimulationJobType

    'Close - Check for unsaved changes
	Private Sub VehicleFormClosing(sender As Object, e As FormClosingEventArgs) Handles Me.FormClosing
		If e.CloseReason <> CloseReason.ApplicationExitCall And e.CloseReason <> CloseReason.WindowsShutDown Then
			e.Cancel = ChangeCheckCancel()
		End If
	End Sub

	'Initialise form
	Private Sub VehicleFormLoad(sender As Object, e As EventArgs) Handles MyBase.Load
		PnLoad.Enabled = Not Cfg.DeclMode
		ButAxlAdd.Enabled = Not Cfg.DeclMode
		ButAxlRem.Enabled = Not Cfg.DeclMode
		CbCdMode.Enabled = Not Cfg.DeclMode
		PnWheelDiam.Enabled = Not Cfg.DeclMode
		'gbPTO.Enabled = Not Cfg.DeclMode
		pnPTO.Enabled = Not Cfg.DeclMode
        gbPTODrive.Enabled = not Cfg.DeclMode
        tpRoadSweeper.Visible = Not cfg.DeclMode

		CbCdMode.ValueMember = "Value"
		CbCdMode.DisplayMember = "Label"
		CbCdMode.DataSource = [Enum].GetValues(GetType(CrossWindCorrectionMode)) _
			.Cast(Of CrossWindCorrectionMode) _
			.Select(Function(mode) New With {Key .Value = mode, .Label = mode.GetLabel()}).ToList()

		CbRtType.ValueMember = "Value"
		CbRtType.DisplayMember = "Label"
		CbRtType.DataSource = [Enum].GetValues(GetType(RetarderType)) _
			.Cast(Of RetarderType) _
			.Select(Function(type) New With {Key .Value = type, .Label = type.GetLabel()}).ToList()

		CbAxleConfig.ValueMember = "Value"
		CbAxleConfig.DisplayMember = "Label"
		If (cfg.DeclMode) Then
			CbAxleConfig.DataSource = DeclarationData.Segments.GetAxleConfigurations() _
				.Cast(Of AxleConfiguration) _
				.Select(Function(category) New With {Key .Value = category, .Label = category.GetName()}).ToList()
		else
				CbAxleConfig.DataSource = [Enum].GetValues(GetType(AxleConfiguration)) _
					.Cast(Of AxleConfiguration) _
					.Select(Function(category) New With {Key .Value = category, .Label = category.GetName()}).ToList()
		End If

		cbEcoRoll.ValueMember = "Value"
        cbEcoRoll.DisplayMember = "Label"
        cbEcoRoll.DataSource = [Enum].GetValues(GetType(EcoRollType)).Cast(Of EcoRollType).Select(Function(ecoRoll) new With {Key .Value = ecoRoll, .Label = ecoRoll.GetName()}).ToList()


        cbPcc.ValueMember = "Value"
        cbPcc.DisplayMember = "Label"
	    cbPcc.DataSource = [Enum].GetValues(GetType(PredictiveCruiseControlType)).Cast(Of PredictiveCruiseControlType).Select(Function(pcc) new With {Key .Value = pcc, .Label = pcc.GetName()}).ToList()

        cbTankSystem.ValueMember = "Value"
        cbTankSystem.DisplayMember = "Label"
        
        cbTankSystem.DataSource = {New With {Key .Value = CType(Nothing, TankSystem?), .Label = ""}}.Concat([Enum].GetValues(GetType(TankSystem)).Cast(Of TankSystem?).Select(Function(ts) New With {Key .Value = ts , .Label = ts.ToString()})).ToList()

        'tpADAS.Enabled = Cfg.DeclMode

		CbCat.ValueMember = "Value"
		CbCat.DisplayMember = "Label"
		CbCat.DataSource = [Enum].GetValues(GetType(VehicleCategory)) _
			.Cast(Of VehicleCategory) _
			.Select(Function(category) New With {Key .Value = category, .Label = category.GetLabel()}).ToList()

		cbAngledriveType.ValueMember = "Value"
		cbAngledriveType.DisplayMember = "Label"
		cbAngledriveType.DataSource = [Enum].GetValues(GetType(AngledriveType)) _
			.Cast(Of AngledriveType).Select(Function(type) New With {Key .Value = type, .Label = type.GetLabel()}).ToList()
		_axlDlog = New VehicleAxleDialog
		_torqueLimitDlog = New VehicleTorqueLimitDialog()

		cbPTOType.ValueMember = "Value"
		cbPTOType.DisplayMember = "Label"
		cbPTOType.DataSource = DeclarationData.PTOTransmission.GetTechnologies.Select(
			Function(technology) New With {Key .Value = technology, .Label = technology}).ToList()

		cbLegislativeClass.ValueMember = "Value"
		cbLegislativeClass.DisplayMember = "Label"
		cbLegislativeClass.DataSource = [Enum].GetValues(GetType(LegislativeClass)) _
			.Cast(Of LegislativeClass).Select(Function(x) New With {Key .Value = x, .Label = x.GetLabel()}).Tolist()
		'Items.AddRange(PtoTypeStrings.Values.Cast(Of Object).ToArray())

		_changed = False

		NewVehicle()
	End Sub

	'Set HDVclasss
	Private Sub SetHdVclass()
		If String.IsNullOrEmpty(TbMassMass.Text) OrElse Not IsNumeric(TbMassMass.Text) Then
			TbHDVclass.Text = "-"
			Exit Sub
		End If
		Dim vehC As VehicleCategory = CType(CbCat.SelectedValue, VehicleCategory)
		Dim axlC As AxleConfiguration = CType(CbAxleConfig.SelectedValue, AxleConfiguration)
		Dim maxMass As Kilogram = (TbMassMass.Text.ToDouble() * 1000).SI(Of Kilogram)()

		_hdVclass = "-"
		Dim s0 As Segment = Nothing
		Try
			s0 = DeclarationData.Segments.Lookup(vehC, axlC, maxMass, 0.SI(Of Kilogram), False)

		Catch
			' no segment found - ignore
		End Try
		If s0.Found Then
			_hdVclass = s0.VehicleClass.GetClassNumber()
		End If


		TbHDVclass.Text = _hdVclass
		PicVehicle.Image = ConvPicPath(If(Not s0.Found, -1, _hdVclass.ToInt()), False)
	End Sub


	'Set generic values for Declaration mode
	Private Sub DeclInit()
		If Not Cfg.DeclMode Then Exit Sub

		If String.IsNullOrEmpty(TbMassMass.Text) Then
			TbHDVclass.Text = "-"
			Exit Sub
		End If
		Dim vehC As VehicleCategory = CType(CbCat.SelectedValue, VehicleCategory)
		Dim axlC As AxleConfiguration = CType(CbAxleConfig.SelectedValue, AxleConfiguration)
		Dim maxMass As Kilogram = (TbMassMass.Text.ToDouble() * 1000).SI(Of Kilogram)()

		Dim s0 As Segment = Nothing
		Try
			s0 = DeclarationData.Segments.Lookup(vehC, axlC, maxMass, 0.SI(Of Kilogram), False)
		Catch
			' no segment found - ignore
		End Try
		If s0.Found Then
			_hdVclass = s0.VehicleClass.GetClassNumber()
			Dim axleCount As Integer = s0.Missions(0).AxleWeightDistribution.Count()
			Dim i0 As Integer = LvRRC.Items.Count

			Dim i As Integer
			If axleCount > i0 Then
				For i = 1 To axleCount - LvRRC.Items.Count
					LvRRC.Items.Add(CreateListViewItem(i + i0, Double.NaN, False, Double.NaN, Double.NaN, "", Double.NaN,
														AxleType.VehicleNonDriven))
				Next

			ElseIf axleCount < LvRRC.Items.Count Then
				For i = axleCount To LvRRC.Items.Count - 1
					LvRRC.Items.RemoveAt(LvRRC.Items.Count - 1)
				Next
			End If

			'PnAll.Enabled = True

		Else
			'PnAll.Enabled = False
			_hdVclass = "-"
		End If

		TbMassExtra.Text = "-"
		TbLoad.Text = "-"
		CbCdMode.SelectedValue = CrossWindCorrectionMode.DeclarationModeCorrection

		TbCdFile.Text = ""

		Dim rdyn As Double
		rdyn = -1

		If rdyn < 0 Then
			TBrdyn.Text = "-"
		Else
			TBrdyn.Text = rdyn.ToGUIFormat()
		End If
	End Sub


#Region "Toolbar"

	'New
	Private Sub ToolStripBtNew_Click(sender As Object, e As EventArgs) Handles ToolStripBtNew.Click
		NewVehicle()
	End Sub

	'Open
	Private Sub ToolStripBtOpen_Click(sender As Object, e As EventArgs) Handles ToolStripBtOpen.Click
		If VehicleFileBrowser.OpenDialog(_vehFile) Then
			Try
				OpenVehicle(VehicleFileBrowser.Files(0))
			Catch ex As Exception
				MsgBox(ex.Message, MsgBoxStyle.OkOnly, "Error loading Vehicle File")
			End Try

		End If
	End Sub

	'Save
	Private Sub ToolStripBtSave_Click(sender As Object, e As EventArgs) Handles ToolStripBtSave.Click
		SaveOrSaveAs(False)
	End Sub

	'Save As
	Private Sub ToolStripBtSaveAs_Click(sender As Object, e As EventArgs) Handles ToolStripBtSaveAs.Click
		SaveOrSaveAs(True)
	End Sub

	'Send to VECTO Editor
	Private Sub ToolStripBtSendTo_Click(sender As Object, e As EventArgs) Handles ToolStripBtSendTo.Click

		If ChangeCheckCancel() Then Exit Sub

		If _vehFile = "" Then
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

		VectoJobForm.TbVEH.Text = GetFilenameWithoutDirectory(_vehFile, JobDir)
	End Sub

	'Help
	Private Sub ToolStripButton1_Click(sender As Object, e As EventArgs) Handles ToolStripButton1.Click
		If File.Exists(Path.Combine(MyAppPath, "User Manual\help.html")) Then
			Dim defaultBrowserPath As String = BrowserUtils.GetDefaultBrowserPath()
			Process.Start(defaultBrowserPath,
						String.Format("""file://{0}""", Path.Combine(MyAppPath, "User Manual\help.html#vehicle-editor")))
		Else
			MsgBox("User Manual not found!", MsgBoxStyle.Critical)
		End If
	End Sub

#End Region

	'Save and Close
	Private Sub ButOK_Click(sender As Object, e As EventArgs) Handles ButOK.Click
		If SaveOrSaveAs(False) Then Close()
	End Sub

	'Cancel
	Private Sub ButCancel_Click(sender As Object, e As EventArgs) Handles ButCancel.Click
		Close()
	End Sub

	'Save or Save As function = true if file is saved
	Private Function SaveOrSaveAs(saveAs As Boolean) As Boolean
		If _vehFile = "" Or saveAs Then
			If VehicleFileBrowser.SaveDialog(_vehFile) Then
				_vehFile = VehicleFileBrowser.Files(0)
			Else
				Return False
			End If
		End If
		Return SaveVehicle(_vehFile)
	End Function

	'New VEH
	Private Sub NewVehicle()
		If ChangeCheckCancel() Then Exit Sub

		TbMass.Text = ""
		TbLoad.Text = ""
		TBrdyn.Text = ""
		TBcdA.Text = ""

		CbCdMode.SelectedIndex = 0
		TbCdFile.Text = ""

		CbRtType.SelectedIndex = 0
		TbRtRatio.Text = ""
		TbRtPath.Text = ""

		CbRtType.SelectedIndex = 0
		TbRtRatio.Text = ""
		TbRtPath.Text = ""

		CbCat.SelectedIndex = 0

		LvRRC.Items.Clear()

		TbMassMass.Text = ""
		TbMassExtra.Text = ""
		CbAxleConfig.SelectedIndex = 0

		cbPTOType.SelectedIndex = 0
		tbPTOLossMap.Text = ""

		DeclInit()

		_vehFile = ""
		Text = "VEH Editor"
		LbStatus.Text = ""

		_changed = False
	End Sub

	'Open VEH
	Sub OpenVehicle(file As String)

		If ChangeCheckCancel() Then Exit Sub

		Dim inputData As IEngineeringInputDataProvider = TryCast(JSONInputDataFactory.ReadComponentData(file), 
																IEngineeringInputDataProvider)
		Dim vehicle As IVehicleEngineeringInputData = inputData.JobInputData.Vehicle
		Dim airdrag As IAirdragEngineeringInputData = inputData.JobInputData.Vehicle.Components.AirdragInputData
		Dim retarder As IRetarderInputData = inputData.JobInputData.Vehicle.Components.RetarderInputData
		Dim angledrive As IAngledriveInputData = inputData.JobInputData.Vehicle.Components.AngledriveInputData
		Dim pto As IPTOTransmissionInputData = inputData.JobInputData.Vehicle.Components.PTOTransmissionInputData

		If Cfg.DeclMode <> vehicle.SavedInDeclarationMode Then
			Select Case WrongMode()
				Case 1
					Close()
					MainForm.RbDecl.Checked = Not MainForm.RbDecl.Checked
					MainForm.OpenVectoFile(file)
				Case -1
					Exit Sub
			End Select
		End If

		_vehFile = file
		Dim basePath As String = Path.GetDirectoryName(file)
		CbCat.SelectedValue = vehicle.VehicleCategory
		CbAxleConfig.SelectedValue = vehicle.AxleConfiguration
		TbMassMass.Text = (vehicle.GrossVehicleMassRating.Value() / 1000).ToGUIFormat()

		TbMass.Text = vehicle.CurbMassChassis.ToGUIFormat()
		TbMassExtra.Text = vehicle.CurbMassExtra.ToGUIFormat()
		TbLoad.Text = vehicle.Loading.ToGUIFormat()
		TBrdyn.Text = (vehicle.DynamicTyreRadius.Value() * 1000).ToGUIFormat()
		tbVehIdlingSpeed.Text = If(vehicle.EngineIdleSpeed Is Nothing, "", vehicle.EngineIdleSpeed.AsRPM.ToGUIFormat())

		CbCdMode.SelectedValue = airdrag.CrossWindCorrectionMode
		TbCdFile.Text =
			If(airdrag.CrosswindCorrectionMap Is Nothing, "", GetRelativePath(airdrag.CrosswindCorrectionMap.Source, basePath))

		cbLegislativeClass.SelectedValue = vehicle.LegislativeClass
		CbRtType.SelectedValue = retarder.Type
		TbRtRatio.Text = retarder.Ratio.ToGUIFormat()
		TbRtPath.Text = If(retarder.LossMap Is Nothing, "", GetRelativePath(retarder.LossMap.Source, basePath))

	    cbPcc.SelectedValue = vehicle.ADAS.PredictiveCruiseControl
	    cbEcoRoll.SelectedValue = vehicle.ADAS.EcoRoll
	    cbEngineStopStart.Checked = vehicle.ADAS.EngineStopStart
	    cbAtEcoRollReleaseLockupClutch.Checked = If(vehicle.ADAS.ATEcoRollReleaseLockupClutch , false)

        if (vehicle.SavedInDeclarationMode) then
            Dim declVehicle as IVehicleDeclarationInputData = vehicle
            
            if (declVehicle.TankSystem.HasValue) then
                cbTankSystem.SelectedValue = declVehicle.TankSystem.Value
            End If
        Else 
            tbPtoEngineSpeed.Text = vehicle.PTO_DriveEngineSpeed?.AsRPM.ToGUIFormat()
            tbPtoGear.Text = if (not vehicle.PTO_DriveGear is nothing, vehicle.PTO_DriveGear.Gear.ToString() ,"")
        End If

		LvRRC.Items.Clear()
		Dim i As Integer = 0
		Dim a0 As IAxleEngineeringInputData
		For Each a0 In vehicle.Components.AxleWheels.AxlesEngineering
			i += 1
			If Cfg.DeclMode Then
				Dim inertia As Double = DeclarationData.Wheels.Lookup(a0.Tyre.Dimension).Inertia.Value()
				LvRRC.Items.Add(CreateListViewItem(i, Double.NaN, a0.TwinTyres, a0.Tyre.RollResistanceCoefficient,
													a0.Tyre.TyreTestLoad.Value(), a0.Tyre.Dimension, inertia, a0.AxleType))
			Else
				LvRRC.Items.Add(CreateListViewItem(i, a0.AxleWeightShare, a0.TwinTyres, a0.Tyre.RollResistanceCoefficient,
													a0.Tyre.TyreTestLoad.Value(), a0.Tyre.Dimension, a0.Tyre.Inertia.Value(), a0.AxleType))

			End If
		Next

		lvTorqueLimits.Items.Clear()
		For Each entry As ITorqueLimitInputData In vehicle.TorqueLimits
			lvTorqueLimits.Items.Add(CreateMaxTorqueListViewItem(entry.Gear(), entry.MaxTorque.Value()))
		Next

		'TbMassExtra.Text = veh.MassExtra.ToGUIFormat()

		TBcdA.Text = If(airdrag.AirDragArea Is Nothing, "", airdrag.AirDragArea.ToGUIFormat())
		tbVehicleHeight.Text = If(vehicle.Height Is Nothing, "", vehicle.Height.ToGUIFormat())

		cbPTOType.SelectedValue = pto.PTOTransmissionType
		tbPTOLossMap.Text =
			If(Cfg.DeclMode OrElse pto.PTOLossMap Is Nothing, "", GetRelativePath(pto.PTOLossMap.Source, basePath))
		tbPTOCycle.Text = If(Cfg.DeclMode OrElse pto.PTOCycleDuringStop Is Nothing, "", GetRelativePath(pto.PTOCycleDuringStop.Source, basePath))
        tbPTODrive.Text = If (Cfg.DeclMode OrElse pto.PTOCycleWhileDriving Is Nothing, "", GetRelativePath(pto.PTOCycleWhileDriving.Source, basePath))

		cbAngledriveType.SelectedValue = angledrive.Type
		tbAngledriveRatio.Text = angledrive.Ratio.ToGUIFormat()
		tbAngledriveLossMapPath.Text =
			If(angledrive.LossMap Is Nothing, "", GetRelativePath(angledrive.LossMap.Source, basePath))

		DeclInit()

		VehicleFileBrowser.UpdateHistory(file)
		Text = GetFilenameWithoutPath(file, True)
		LbStatus.Text = ""

		Activate()

		_changed = False
	End Sub

	Private Function CreateListViewItem(axleNumber As Integer, share As Double, twinTire As Boolean, rrc As Double,
										fzIso As Double, wheels As String, inertia As Double, axletype As AxleType) As ListViewItem
		Dim retVal As New ListViewItem
		retVal.SubItems(0).Text = axleNumber.ToGUIFormat()
		FillDoubleValue(retVal, share, "-")
		retVal.SubItems.Add(If(twinTire, "yes", "no"))
		FillDoubleValue(retVal, rrc)
		FillDoubleValue(retVal, fzIso)
		retVal.SubItems.Add(wheels)
		FillDoubleValue(retVal, inertia)
		retVal.SubItems.Add(axletype.GetLabel())
		Return retVal
	End Function

	Private Sub FillDoubleValue(listViewItem As ListViewItem, share As Double, Optional defaultValue As String = "")

		If Double.IsNaN(share) Then
			listViewItem.SubItems.Add(defaultValue)
		Else
			listViewItem.SubItems.Add(share.ToGUIFormat())
		End If
	End Sub

	'Save VEH
	Private Function SaveVehicle(file As String) As Boolean

		Dim veh As Vehicle = New Vehicle
		veh.FilePath = file

		veh.Mass = TbMass.Text.ToDouble(0)
		veh.MassExtra = TbMassExtra.Text.ToDouble(0)
		veh.Loading = TbLoad.Text.ToDouble(0)
		veh.VehicleHeight = tbVehicleHeight.Text.ToDouble(0)
		veh.CdA0 = If(String.IsNullOrWhiteSpace(TBcdA.Text), Double.NaN, TBcdA.Text.ToDouble(0))
		veh.legClass = CType(cbLegislativeClass.SelectedValue, LegislativeClass)
		veh.DynamicTyreRadius = TBrdyn.Text.ToDouble(0)
		veh.CrossWindCorrectionMode = CType(CbCdMode.SelectedValue, CrossWindCorrectionMode)
		veh.CrossWindCorrectionFile.Init(GetPath(file), TbCdFile.Text)
		veh.RetarderType = CType(CbRtType.SelectedValue, RetarderType)
		veh.RetarderRatio = TbRtRatio.Text.ToDouble(0)
		veh.RetarderLossMapFile.Init(GetPath(file), TbRtPath.Text)

		veh.VehicleidlingSpeed = _tbVehIdlingSpeed.Text.ToDouble(0).RPMtoRad()

		veh.AngledriveType = CType(cbAngledriveType.SelectedValue, AngledriveType)
		veh.AngledriveRatio = tbAngledriveRatio.Text.ToDouble(0)
		veh.AngledriveLossMapFile.Init(GetPath(file), tbAngledriveLossMapPath.Text)

		veh.VehicleCategory = CType(CbCat.SelectedValue, VehicleCategory) 'CType(CbCat.SelectedIndex, tVehCat)

		For Each entry As ListViewItem In LvRRC.Items
			Dim a0 As AxleInputData = New AxleInputData()
			a0.AxleWeightShare = entry.SubItems(AxleTbl.RelativeLoad).Text.ToDouble(0)
			a0.TwinTyres = (entry.SubItems(AxleTbl.TwinTyres).Text = "yes")
			a0.AxleType = entry.SubItems(AxleTbl.AxleType).Text.ParseEnum(Of AxleType)()
			dim tyre as TyreInputData = New TyreInputData()
			tyre.RollResistanceCoefficient = entry.SubItems(AxleTbl.RRC).Text.ToDouble(0)
			tyre.TyreTestLoad = entry.SubItems(AxleTbl.FzISO).Text.ToDouble(0).SI(Of Newton)()
			tyre.Dimension = entry.SubItems(AxleTbl.WheelsDimension).Text
			tyre.Inertia = entry.SubItems(AxleTbl.Inertia).Text.ToDouble(0).SI(Of KilogramSquareMeter)()
			a0.Tyre = tyre
			veh.Axles.Add(a0)
		Next

		veh.PtoType = CType(cbPTOType.SelectedValue, String)
		veh.PtoLossMap.Init(GetPath(file), tbPTOLossMap.Text)
		veh.PtoCycleStandstill.Init(GetPath(file), tbPTOCycle.Text)
	    veh.PtoCycleDriving.Init(GetPath(file), tbPTODrive.Text)

		veh.MassMax = TbMassMass.Text.ToDouble(0)
		veh.MassExtra = TbMassExtra.Text.ToDouble(0)
		veh.AxleConfiguration = CType(CbAxleConfig.SelectedValue, AxleConfiguration)

		For Each item As ListViewItem In lvTorqueLimits.Items
			Dim tl As TorqueLimitInputData = New TorqueLimitInputData()
			tl.Gear() = item.SubItems(TorqueLimitsTbl.Gear).Text.ToInt(0)
			tl.MaxTorque = item.SubItems(TorqueLimitsTbl.MaxTorque).Text.ToDouble(0).SI(Of NewtonMeter)()
			veh.torqueLimitsList.Add(tl)
		Next

        veh.EcoRollType = CType(cbEcoRoll.SelectedValue, EcoRollType)
        veh.PCC = CType(cbPcc.SelectedValue, PredictiveCruiseControlType)
        veh.EngineStop = cbEngineStopStart.Checked
	    veh.EcoRollReleaseLockupClutch = cbAtEcoRollReleaseLockupClutch.Checked

        veh.VehicleTankSystem = CType(If(cbTankSystem.SelectedIndex > 0, cbTankSystem.SelectedValue, nothing), TankSystem?)

        veh.GearDuringPTODrive = If(string.IsNullOrWhiteSpace(tbPtoGear.Text), nothing, CType(tbPtoGear.Text.ToInt(), UInteger?))
        veh.EngineSpeedDuringPTODrive = if(string.IsNullOrWhiteSpace(tbPtoEngineSpeed.Text), Nothing, tbPtoEngineSpeed.Text.ToDouble().RPMtoRad())
		'---------------------------------------------------------------------------------
		If Not veh.SaveFile Then
			MsgBox("Cannot safe to " & file, MsgBoxStyle.Critical)
			Return False
		End If

		If AutoSendTo Then
			If VectoJobForm.Visible Then
				If UCase(FileRepl(VectoJobForm.TbVEH.Text, JobDir)) <> UCase(file) Then _
					VectoJobForm.TbVEH.Text = GetFilenameWithoutDirectory(file, JobDir)
				VectoJobForm.UpdatePic()
			End If
		End If

		VehicleFileBrowser.UpdateHistory(file)
		Text = GetFilenameWithoutPath(file, True)
		LbStatus.Text = ""

		_changed = False

		Return True
	End Function

#Region "Cd"

	'Cd Mode Change
	Private Sub CbCdMode_SelectedIndexChanged(sender As Object, e As EventArgs) _
		Handles CbCdMode.SelectedIndexChanged
		Dim bEnabled As Boolean

		Select Case CType(CbCdMode.SelectedValue, CrossWindCorrectionMode)

			Case CrossWindCorrectionMode.VAirBetaLookupTable
				bEnabled = True
				LbCdMode.Text = "Input file: Yaw Angle [°], Cd Scaling Factor [-]"

			Case CrossWindCorrectionMode.SpeedDependentCorrectionFactor
				bEnabled = True
				LbCdMode.Text = "Input file: Vehicle Speed [km/h], Cd Scaling Factor [-]"
			Case Else ' tCdMode.ConstCd0, tCdMode.CdOfVdecl
				bEnabled = False
				LbCdMode.Text = ""

		End Select

		tbVehicleHeight.Enabled = Not Cfg.DeclMode AndAlso
								CType(CbCdMode.SelectedValue, CrossWindCorrectionMode) = CrossWindCorrectionMode.DeclarationModeCorrection

		If Not Cfg.DeclMode Then
			TbCdFile.Enabled = bEnabled
			BtCdFileBrowse.Enabled = bEnabled
			BtCdFileOpen.Enabled = bEnabled
		End If

		Change()
	End Sub

	'Cd File Browse
	Private Sub BtCdFileBrowse_Click(sender As Object, e As EventArgs) Handles BtCdFileBrowse.Click
		Dim ex As String

		If CbCdMode.SelectedIndex = 1 Then
			ex = "vcdv"
		Else
			ex = "vcdb"
		End If

		If CrossWindCorrectionFileBrowser.OpenDialog(FileRepl(TbCdFile.Text, GetPath(_vehFile)), False, ex) Then _
			TbCdFile.Text = GetFilenameWithoutDirectory(CrossWindCorrectionFileBrowser.Files(0), GetPath(_vehFile))
	End Sub

	'Open Cd File
	Private Sub BtCdFileOpen_Click(sender As Object, e As EventArgs) Handles BtCdFileOpen.Click
		OpenFiles(FileRepl(TbCdFile.Text, GetPath(_vehFile)))
	End Sub

#End Region

#Region "Retarder"

	'Rt Type Change
	Private Sub CbRtType_SelectedIndexChanged(sender As Object, e As EventArgs) _
		Handles CbRtType.SelectedIndexChanged
		Select Case CbRtType.SelectedIndex
			Case 1 'Primary
				LbRtRatio.Text = "Ratio to engine speed"
				PnRt.Enabled = True
			Case 2 'Secondary
				LbRtRatio.Text = "Ratio to cardan shaft speed"
				TbRtPath.Enabled = True
				BtRtBrowse.Enabled = True
				PnRt.Enabled = True
			Case Else '0 None
				LbRtRatio.Text = "Ratio"
				PnRt.Enabled = False
		End Select

		Change()
	End Sub

	'Rt File Browse
	Private Sub BtRtBrowse_Click(sender As Object, e As EventArgs) Handles BtRtBrowse.Click
		If RetarderLossMapFileBrowser.OpenDialog(FileRepl(TbRtPath.Text, GetPath(_vehFile))) Then _
			TbRtPath.Text = GetFilenameWithoutDirectory(RetarderLossMapFileBrowser.Files(0), GetPath(_vehFile))
	End Sub

#End Region

#Region "Track changes"

	Private Sub Change()
		If Not _changed Then
			LbStatus.Text = "Unsaved changes in current file"
			_changed = True
		End If
	End Sub

	' "Save changes? "... Returns True if user aborts
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

	Private Sub TBmass_TextChanged(sender As Object, e As EventArgs) Handles TbMass.TextChanged
		Change()
	End Sub

	Private Sub TBcw_TextChanged(sender As Object, e As EventArgs) _
		Handles TbLoad.TextChanged, TBrdyn.TextChanged, TBcdA.TextChanged, TbCdFile.TextChanged, TbRtRatio.TextChanged,
				cbAngledriveType.SelectedIndexChanged, TbRtPath.TextChanged, tbAngledriveLossMapPath.TextChanged,
				tbAngledriveRatio.TextChanged
		Change()
	End Sub

	Private Sub CbCat_SelectedIndexChanged(sender As Object, e As EventArgs) Handles CbCat.SelectedIndexChanged
		Change()
		SetHdVclass()
		DeclInit()
	End Sub

	Private Sub TbMassTrailer_TextChanged(sender As Object, e As EventArgs) Handles TbMassExtra.TextChanged
		Change()
	End Sub

	Private Sub TbMassMax_TextChanged(sender As Object, e As EventArgs) Handles TbMassMass.TextChanged
		Change()
		SetHdVclass()
		DeclInit()
	End Sub

	Private Sub CbAxleConfig_SelectedIndexChanged(sender As Object, e As EventArgs) _
		Handles CbAxleConfig.SelectedIndexChanged
		Change()
		SetHdVclass()
		DeclInit()
	End Sub

#End Region

#Region "Axle Configuration"

	Private Sub ButAxlAdd_Click(sender As Object, e As EventArgs) Handles ButAxlAdd.Click
		_axlDlog.Clear()
		If _axlDlog.ShowDialog = DialogResult.OK Then
			LvRRC.Items.Add(CreateListViewItem(LvRRC.Items.Count + 1, _axlDlog.TbAxleShare.Text.ToDouble(0),
												_axlDlog.CbTwinT.Checked, _axlDlog.TbRRC.Text.ToDouble(0), _axlDlog.TbFzISO.Text.ToDouble(0),
												_axlDlog.CbWheels.Text, _axlDlog.TbI_wheels.Text.ToDouble(0), AxleType.VehicleNonDriven))
			Change()
			DeclInit()

		End If
	End Sub

	Private Sub ButAxlRem_Click(sender As Object, e As EventArgs) Handles ButAxlRem.Click
		RemoveAxleItem()
	End Sub

	Private Sub LvAxle_DoubleClick(sender As Object, e As EventArgs) Handles LvRRC.DoubleClick
		EditAxleItem()
	End Sub

	Private Sub LvAxle_KeyDown(sender As Object, e As KeyEventArgs) Handles LvRRC.KeyDown
		Select Case e.KeyCode
			Case Keys.Delete, Keys.Back
				If Not Cfg.DeclMode Then RemoveAxleItem()
			Case Keys.Enter
				EditAxleItem()
		End Select
	End Sub

	Private Sub RemoveAxleItem()
		Dim lv0 As ListViewItem
		Dim i As Integer

		If LvRRC.SelectedItems.Count = 0 Then
			If LvRRC.Items.Count = 0 Then
				Exit Sub
			Else
				LvRRC.Items(LvRRC.Items.Count - 1).Selected = True
			End If
		End If

		LvRRC.SelectedItems(0).Remove()

		If LvRRC.Items.Count > 0 Then

			i = 0
			For Each lv0 In LvRRC.Items
				i += 1
				lv0.SubItems(AxleTbl.AxleNumber).Text = i.ToString
			Next

			LvRRC.Items(LvRRC.Items.Count - 1).Selected = True
			LvRRC.Focus()
		End If

		Change()
	End Sub

	Private Sub EditAxleItem()
		If LvRRC.SelectedItems.Count = 0 Then Exit Sub

		Dim lv0 As ListViewItem = LvRRC.SelectedItems(0)

		_axlDlog.TbAxleShare.Text = lv0.SubItems(AxleTbl.RelativeLoad).Text
		_axlDlog.CbTwinT.Checked = (lv0.SubItems(AxleTbl.TwinTyres).Text = "yes")
		_axlDlog.TbRRC.Text = lv0.SubItems(AxleTbl.RRC).Text
		_axlDlog.TbFzISO.Text = lv0.SubItems(AxleTbl.FzISO).Text
		_axlDlog.TbI_wheels.Text = lv0.SubItems(AxleTbl.Inertia).Text
		_axlDlog.CbWheels.SelectedItem = if(string.IsNullOrWhiteSpace(lv0.SubItems(AxleTbl.WheelsDimension).Text), "-",lv0.SubItems(AxleTbl.WheelsDimension).Text) 
		_axlDlog.cbAxleType.SelectedValue = lv0.SubItems(AxleTbl.AxleType).Text.ParseEnum(Of AxleType)()
		If _axlDlog.ShowDialog = DialogResult.OK Then
			lv0.SubItems(AxleTbl.RelativeLoad).Text = _axlDlog.TbAxleShare.Text
			If _axlDlog.CbTwinT.Checked Then
				lv0.SubItems(AxleTbl.TwinTyres).Text = "yes"
			Else
				lv0.SubItems(AxleTbl.TwinTyres).Text = "no"
			End If
			lv0.SubItems(AxleTbl.RRC).Text = _axlDlog.TbRRC.Text
			lv0.SubItems(AxleTbl.FzISO).Text = _axlDlog.TbFzISO.Text
			lv0.SubItems(AxleTbl.WheelsDimension).Text = _axlDlog.CbWheels.Text
			lv0.SubItems(AxleTbl.Inertia).Text = _axlDlog.TbI_wheels.Text
			lv0.SubItems(AxleTbl.AxleType).Text = CType(_axlDlog.cbAxleType.SelectedValue, AxleType).GetLabel()
			Change()
			DeclInit()
		End If
	End Sub

#End Region

#Region "Open File Context Menu"


	Private Sub OpenFiles(ParamArray files() As String)
		If files.Length = 0 Then Exit Sub

		_cmFiles = files
		OpenWithToolStripMenuItem.Text = "Open with " & Cfg.OpenCmdName
		CmOpenFile.Show(Cursor.Position)
	End Sub

	Private Sub OpenWithToolStripMenuItem_Click(sender As Object, e As EventArgs) _
		Handles OpenWithToolStripMenuItem.Click
		If Not FileOpenAlt(_cmFiles(0)) Then MsgBox("Failed to open file!")
	End Sub

	Private Sub ShowInFolderToolStripMenuItem_Click(sender As Object, e As EventArgs) _
		Handles ShowInFolderToolStripMenuItem.Click
		If File.Exists(_cmFiles(0)) Then
			Try
				Process.Start("explorer", "/select,""" & _cmFiles(0) & "")
			Catch ex As Exception
				MsgBox("Failed to open file!")
			End Try
		Else
			MsgBox("File not found!")
		End If
	End Sub

#End Region


#Region "Angular Gear"

	Private Sub cbAngledriveType_SelectedIndexChanged(sender As Object, e As EventArgs) _
		Handles cbAngledriveType.SelectedIndexChanged
		Select Case CType(cbAngledriveType.SelectedValue, AngledriveType)
			Case AngledriveType.SeparateAngledrive
				pnAngledriveFields.Enabled = True
				tbAngledriveRatio.Text = "1.0"
			Case Else 'Losses included in Transmission, None
				tbAngledriveRatio.Text = ""
				tbAngledriveLossMapPath.Text = ""
				pnAngledriveFields.Enabled = False
		End Select
		Change()
	End Sub

	Private Sub btAngledriveLossMapBrowse_Click(sender As Object, e As EventArgs) Handles btAngledriveLossMapBrowse.Click
		If TransmissionLossMapFileBrowser.OpenDialog(FileRepl(TbRtPath.Text, GetPath(_vehFile))) Then _
			tbAngledriveLossMapPath.Text = GetFilenameWithoutDirectory(TransmissionLossMapFileBrowser.Files(0),
																		GetPath(_vehFile))
	End Sub

#End Region

	Private Sub cbPTOType_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cbPTOType.SelectedIndexChanged 

		If (Cfg.DeclMode) Then
			Exit Sub
		End If

		If (cbPTOType.SelectedIndex = 0) Then
			pnPTO.Enabled = False
            gbPTODrive.Enabled = false
			tbPTOLossMap.Text = ""
		Else
			pnPTO.Enabled = True
            gbPTODrive.Enabled = True
		End If

		Change()
	End Sub

	Private Sub btPTOLossMapBrowse_Click(sender As Object, e As EventArgs) Handles btPTOLossMapBrowse.Click 
		If PtoLossMapFileBrowser.OpenDialog(FileRepl(tbPTOLossMap.Text, GetPath(_vehFile))) Then
			tbPTOLossMap.Text = GetFilenameWithoutDirectory(PtoLossMapFileBrowser.Files(0), GetPath(_vehFile))
		End If
	End Sub

	Private Sub btPTOCycle_Click(sender As Object, e As EventArgs) Handles btPTOCycle.Click 
		If PTODrivingCycleStandstillFileBrowser.OpenDialog(FileRepl(tbPTOCycle.Text, GetPath(_vehFile))) Then
			tbPTOCycle.Text = GetFilenameWithoutDirectory(PTODrivingCycleStandstillFileBrowser.Files(0), GetPath(_vehFile))
		End If
	End Sub

	Private Sub btAddMaxTorqueEntry_Click(sender As Object, e As EventArgs) Handles btAddMaxTorqueEntry.Click
		_torqueLimitDlog.Clear()
		If _torqueLimitDlog.ShowDialog() = DialogResult.OK Then
			Dim gear As Integer = _torqueLimitDlog.tbGear.Text.ToInt(0)
			For Each entry As ListViewItem In lvTorqueLimits.Items
				If entry.SubItems(TorqueLimitsTbl.Gear).Text.ToInt() = gear Then
					entry.SubItems(TorqueLimitsTbl.MaxTorque).Text = _torqueLimitDlog.tbMaxTorque.Text.ToDouble(0).ToGUIFormat
					Change()
					Return
				End If
			Next

			lvTorqueLimits.Items.Add(CreateMaxTorqueListViewItem(gear, _torqueLimitDlog.tbMaxTorque.Text.ToDouble(0)))

			Change()

		End If
	End Sub

	Private Function CreateMaxTorqueListViewItem(gear As Integer, maxTorque As Double) As ListViewItem
		Dim retVal As New ListViewItem
		retVal.SubItems(0).Text = gear.ToGUIFormat()
		retVal.SubItems.Add(maxTorque.ToGUIFormat())
		Return retVal
	End Function

	Private Sub btDelMaxTorqueEntry_Click(sender As Object, e As EventArgs) Handles btDelMaxTorqueEntry.Click
		RemoveMaxTorqueItem()
	End Sub

	Private Sub RemoveMaxTorqueItem()
		If lvTorqueLimits.SelectedItems.Count = 0 Then
			If lvTorqueLimits.Items.Count = 0 Then
				Exit Sub
			Else
				lvTorqueLimits.Items(lvTorqueLimits.Items.Count - 1).Selected = True
			End If
		End If

		lvTorqueLimits.SelectedItems(0).Remove()

		Change()
	End Sub

	Private Sub lvTorqueLimits_DoubleClick(sender As Object, e As EventArgs) Handles lvTorqueLimits.DoubleClick
		EditMaxTorqueEntry()
	End Sub

	Private Sub EditMaxTorqueEntry()
		If lvTorqueLimits.SelectedItems.Count = 0 Then Exit Sub

		Dim entry As ListViewItem = lvTorqueLimits.SelectedItems(0)
		_torqueLimitDlog.tbGear.Text = entry.SubItems(TorqueLimitsTbl.Gear).Text
		_torqueLimitDlog.tbGear.ReadOnly = True
		_torqueLimitDlog.tbMaxTorque.Text = entry.SubItems(TorqueLimitsTbl.MaxTorque).Text
		_torqueLimitDlog.tbMaxTorque.Focus()
		If (_torqueLimitDlog.ShowDialog() = DialogResult.OK) Then
			entry.SubItems(TorqueLimitsTbl.MaxTorque).Text = _torqueLimitDlog.tbMaxTorque.Text
		End If
		_torqueLimitDlog.tbGear.ReadOnly = False
	End Sub

	Private Sub Label18_Click(sender As Object, e As EventArgs) Handles Label18.Click

	End Sub

	Private Sub tbVehIdlingSpeed_TextChanged(sender As Object, e As EventArgs) Handles tbVehIdlingSpeed.TextChanged

	End Sub

    Private Sub btPTOCycleDrive_Click(sender As Object, e As EventArgs) Handles btPTOCycleDrive.Click
        If PTODrivingCycleDrivingFileBrowser.OpenDialog(FileRepl(tbPTODrive.Text, GetPath(_vehFile))) Then
            tbPTODrive.Text = GetFilenameWithoutDirectory(PTODrivingCycleDrivingFileBrowser.Files(0), GetPath(_vehFile))
        End If
    End Sub
End Class

