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

Imports System.IO
Imports System.Linq
Imports TUGraz.VectoCommon.InputData
Imports TUGraz.VectoCommon.Models
Imports TUGraz.VectoCommon.Utils
Imports TUGraz.VectoCore.InputData.FileIO.JSON
Imports TUGraz.VectoCore.InputData.Impl
Imports TUGraz.VectoCore.Models.Declaration
Imports TUGraz.VectoCore.Utils

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

	Private Enum RatiosPerGearTbl
		Gear = 0
		Ratio = 1
	End Enum

	Private Enum REESPackTbl
		ReessFile = 0
		Count = 1
		StringId = 2
	End Enum


	Private _axlDlog As VehicleAxleDialog
	Private _hdVclass As VehicleClass
	Private _vehFile As String
	Private _changed As Boolean = False
	Private _cmFiles As String()

	Public AutoSendTo As Boolean = False
	Public JobDir As String = ""
	Private _torqueLimitDlog As VehicleTorqueLimitDialog
	Private _emRatioPerGearDlog As EMGearRatioDialog
	Private _reessPackDlg As REESSPackDialog
	Friend VehicleType As VectoSimulationJobType

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
		pnPTO.Enabled = Not Cfg.DeclMode
		gbPTODrive.Enabled = Not Cfg.DeclMode
		tpRoadSweeper.Visible = Not Cfg.DeclMode

		CbCdMode.DataSource = EnumHelper.GetKeyValuePairs(Of CrossWindCorrectionMode)(Function(t) t.GetLabel())

		CbRtType.DataSource = EnumHelper.GetKeyValuePairs(Of RetarderType)(Function(t) t.GetLabel()).ToDataView()

		If (Cfg.DeclMode) Then
			CbAxleConfig.DataSource = DeclarationData.TruckSegments.GetAxleConfigurations() _
				.Cast(Of AxleConfiguration) _
				.Select(Function(category) New With {.Key = category, .Value = category.GetName()}).ToList()
		Else
			CbAxleConfig.DataSource = EnumHelper.GetKeyValuePairs(Of AxleConfiguration)(Function(t) t.GetName())
		End If

		cbEcoRoll.DataSource = EnumHelper.GetKeyValuePairs(Of EcoRollType)(Function(t) t.GetName())

		cbPcc.DataSource = EnumHelper.GetKeyValuePairs(Of PredictiveCruiseControlType)(Function(t) t.GetName())

		cbTankSystem.DataSource = EnumHelper.GetKeyValuePairs(Of TankSystem)()

		'tpADAS.Enabled = Cfg.DeclMode

		CbCat.DataSource = EnumHelper.GetKeyValuePairs(Of VehicleCategory)(Function(t) t.GetLabel())

		cbAngledriveType.DataSource = EnumHelper.GetKeyValuePairs(Of AngledriveType)(Function(t) t.GetLabel())

		_axlDlog = New VehicleAxleDialog
		_torqueLimitDlog = New VehicleTorqueLimitDialog()
		_emRatioPerGearDlog = New EMGearRatioDialog()
		_reessPackDlg = New REESSPackDialog()

		cbPTOType.DataSource = DeclarationData.PTOTransmission.GetTechnologies.Select(
			Function(technology) New With {.Key = technology, .Value = technology}).ToList()

		cbLegislativeClass.DataSource = EnumHelper.GetKeyValuePairs(Of LegislativeClass)(Function(t) t.GetLabel())
		'cbLegislativeClass.DataSource = EnumHelper.GetValues(Of LegislativeClass).Cast(Of LegislativeClass?).Select( _
		'	Function(x) New With {.Key = x, .Value = x.GetLabel()}).ToList()

		_changed = False

		cbEmPos.DataSource = EnumHelper.GetKeyValuePairs(Of PowertrainPosition)(Function(t) t.GetLabel(),
																				Function(x) x <> PowertrainPosition.GEN _
																					AndAlso x <> PowertrainPosition.HybridP0)

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

		_hdVclass = VehicleClass.Unknown
		Dim s0 As Segment = Nothing
		Try
			s0 = DeclarationData.TruckSegments.Lookup(vehC, axlC, maxMass, 0.SI(Of Kilogram), False)

		Catch
			' no segment found - ignore
		End Try
		If s0.Found Then
			_hdVclass = s0.VehicleClass
		End If


		TbHDVclass.Text = _hdVclass.GetClassNumber()
		PicVehicle.Image = ConvPicPath(_hdVclass, False)
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
			s0 = DeclarationData.TruckSegments.Lookup(vehC, axlC, maxMass, 0.SI(Of Kilogram), False)
		Catch
			' no segment found - ignore
		End Try
		If s0.Found Then
			_hdVclass = s0.VehicleClass
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
			_hdVclass = VehicleClass.Unknown
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
						$"""file://{Path.Combine(MyAppPath, "User Manual\help.html#vehicle-editor")}""")
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

		CbCat.SelectedIndex = 0

		LvRRC.Items.Clear()

		TbMassMass.Text = ""
		TbMassExtra.Text = ""
		CbAxleConfig.SelectedIndex = 0

		cbPcc.SelectedIndex = 0

		cbTankSystem.SelectedIndex = 0

		cbAngledriveType.SelectedIndex = 0

		cbPTOType.SelectedIndex = 0
		tbPTOLossMap.Text = ""

		DeclInit()

		_vehFile = ""
		Text = "VEH Editor"
		LbStatus.Text = ""

		UpdateForm(VehicleType)

		_changed = False
	End Sub

	'Open VEH
	Sub OpenVehicle(file As String)

		If ChangeCheckCancel() Then Exit Sub

		Dim inputData As IEngineeringInputDataProvider = TryCast(JSONInputDataFactory.ReadComponentData(file),
																IEngineeringInputDataProvider)
		UpdateForm(inputData.JobInputData.Vehicle.VehicleType)

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
		cbAtEcoRollReleaseLockupClutch.Checked = If(vehicle.ADAS.ATEcoRollReleaseLockupClutch, False)

		If (vehicle.SavedInDeclarationMode) Then
			Dim declVehicle As IVehicleDeclarationInputData = vehicle

			If (declVehicle.TankSystem.HasValue) Then
				cbTankSystem.SelectedValue = declVehicle.TankSystem.Value
			End If
		Else
			tbPtoEngineSpeed.Text = vehicle.PTO_DriveEngineSpeed?.AsRPM.ToGUIFormat()
			tbPtoGear.Text = If(Not vehicle.PTO_DriveGear Is Nothing, vehicle.PTO_DriveGear.Gear.ToString(), "")
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
		tbPTODrive.Text = If(Cfg.DeclMode OrElse pto.PTOCycleWhileDriving Is Nothing, "", GetRelativePath(pto.PTOCycleWhileDriving.Source, basePath))

		cbAngledriveType.SelectedValue = angledrive.Type
		tbAngledriveRatio.Text = angledrive.Ratio.ToGUIFormat()
		tbAngledriveLossMapPath.Text =
			If(angledrive.LossMap Is Nothing, "", GetRelativePath(angledrive.LossMap.Source, basePath))

		If (vehicle.VehicleType <> VectoSimulationJobType.ConventionalVehicle AndAlso vehicle.VehicleType <> VectoSimulationJobType.EngineOnlySimulation) Then
			lvREESSPacks.Items.Clear()
			For Each entry As IElectricStorageEngineeringInputData In vehicle.Components.ElectricStorage.ElectricStorageElements.OrderBy(Function(x) x.StringId)
				lvREESSPacks.Items.Add(CreateREESSPackListViewItem(GetRelativePath(entry.REESSPack.DataSource.SourceFile, basePath), entry.Count, entry.StringId))
			Next
			tbInitialSoC.Text = (vehicle.InitialSOC * 100).ToGUIFormat()

			if (vehicle.VehicleType = VectoSimulationJobType.ParallelHybridVehicle OrElse vehicle.VehicleType = VectoSimulationJobType.BatteryElectricVehicle OrElse vehicle.VehicleType = VectoSimulationJobType.SerialHybridVehicle) then
			Dim em As ElectricMachineEntry(Of IElectricMotorEngineeringInputData) = vehicle.Components.ElectricMachines.Entries.First(Function(x) x.Position <> PowertrainPosition.GEN)
			tbElectricMotor.Text = GetRelativePath(em.ElectricMachine.DataSource.SourceFile, basePath)
			tbEmCount.Text = em.Count.ToGUIFormat()
			tbEmADCLossMap.Text = If(em.MechanicalTransmissionLossMap Is Nothing, em.MechanicalTransmissionEfficiency.ToGUIFormat(),
									 GetRelativePath(em.MechanicalTransmissionLossMap.Source, basePath))
			tbRatioEm.Text = em.RatioADC.ToGUIFormat()

			cbEmPos.SelectedValue = em.Position

			If (em.Position = PowertrainPosition.HybridP2_5) AndAlso Not em.RatioPerGear Is Nothing Then
				lvRatioPerGear.Items.Clear()
				Dim gear As Integer = 1
				For Each entry As Double In em.RatioPerGear
					lvRatioPerGear.Items.Add(CreateRatioPerGearListViewItem(gear, entry))
					gear += 1
				Next
			End If
            end If
			if vehicle.VehicleType = VectoSimulationJobType.IEPC_E OrElse vehicle.VehicleType = VectoSimulationJobType.IEPC_S Then
				Dim iepc = vehicle.Components.IEPCEngineeringInputData
				tbIEPCFilePath.Text = GetRelativePath(iepc.DataSource.SourceFile, basePath)
			End If

			if vehicle.VehicleType = VectoSimulationJobType.IHPC Then
				Dim ihpc = vehicle.Components.ElectricMachines.Entries.First(Function(x) x.Position = PowertrainPosition.IHPC)
				tbIHPCFilePath.Text = GetRelativePath(ihpc.ElectricMachine.DataSource.SourceFile, basePath)
			End If
		End If

		If (vehicle.VehicleType = VectoSimulationJobType.SerialHybridVehicle OrElse vehicle.VehicleType = VectoSimulationJobType.IEPC_S) Then
			Dim gen As ElectricMachineEntry(Of IElectricMotorEngineeringInputData) = vehicle.Components.ElectricMachines.Entries.First(Function(x) x.Position = PowertrainPosition.GEN)
			tbGenSetEM.Text = GetRelativePath(gen.ElectricMachine.DataSource.SourceFile, basePath)
			tbGenSetCount.Text = gen.Count.ToGUIFormat()
			tbGenSetADC.Text = If(gen.MechanicalTransmissionLossMap Is Nothing, gen.MechanicalTransmissionEfficiency.ToGUIFormat(),
									 GetRelativePath(gen.MechanicalTransmissionLossMap.Source, basePath))
			tbGenSetRatio.Text = gen.RatioADC.ToGUIFormat()
		End If

		If (vehicle.VehicleType = VectoSimulationJobType.ParallelHybridVehicle OrElse vehicle.VehicleType = VectoSimulationJobType.IHPC) Then
			'tbMaxDrivetrainPwr.Text = vehicle.MaxDrivetrainPower.ConvertToKiloWatt().Value.ToXMLFormat(2)
			'ToDo ElectricMotorTorqueLimits changed
			'tbEmTorqueLimits.Text = if (Not vehicle.ElectricMotorTorqueLimits Is Nothing, GetRelativePath(vehicle.ElectricMotorTorqueLimits.Source, basePath), "")
			tbPropulsionTorqueLimit.Text = If(Not vehicle.BoostingLimitations Is Nothing, GetRelativePath(vehicle.BoostingLimitations.Source, basePath), "")
		End If

		DeclInit()

		VehicleFileBrowser.UpdateHistory(file)
		Text = GetFilenameWithoutPath(file, True)
		LbStatus.Text = ""

		Activate()

		_changed = False
	End Sub

	Private Function CreateRatioPerGearListViewItem(gear As Integer, ratio As Double) As ListViewItem
		Dim retval As New ListViewItem
		retval.SubItems(0).Text = gear.ToGUIFormat()
		retval.SubItems.Add(ratio.ToGUIFormat())
		Return retval
	End Function

	Private Function CreateREESSPackListViewItem(batFile As String, count As Integer, stringid As Integer) As ListViewItem
		Dim retval As New ListViewItem
		retval.SubItems(0).Text = If(File.Exists(_vehFile), GetRelativePath(batFile, Path.GetDirectoryName(_vehFile)), batFile)
		retval.SubItems.Add(count.ToGUIFormat())
		retval.SubItems.Add(stringid.ToGUIFormat())
		Return retval
	End Function

	Private Sub UpdateForm(vehType As VectoSimulationJobType)
		VehicleType = vehType

		If Not tcVehicleComponents.TabPages.Contains(tpElectricMachine) Then
			tcVehicleComponents.TabPages.Insert(2, tpElectricMachine)
			tpElectricMachine.BindingContext = BindingContext
		End If

		If Not tcVehicleComponents.TabPages.Contains(tpReess) Then
			tcVehicleComponents.TabPages.Insert(3, tpReess)
			tpReess.BindingContext = BindingContext
		End If

		If Not tcVehicleComponents.TabPages.Contains(tpGensetComponents) Then
			tcVehicleComponents.TabPages.Insert(4, tpGensetComponents)
			tpGensetComponents.BindingContext = BindingContext
		End If
		If Not tcVehicleComponents.TabPages.Contains(tpTorqueLimits) Then
			tcVehicleComponents.TabPages.Insert(5, tpTorqueLimits)
			tpTorqueLimits.BindingContext = BindingContext
		End If

		lblNotePtoPEV_HEVS.Visible = False
		Select Case vehType
			Case VectoSimulationJobType.ConventionalVehicle
				lblTitle.Text = "Conventional Vehicle"

				'Powertrain ---------------------------------------------------------------
				gbVehicleIdlingSpeed.Enabled = True
				gbTankSystem.Enabled = True
				gbRetarderLosses.Enabled = True
				gbAngledrive.Enabled = True

				'Electric Powertrain Components -------------------------------------------
				tcVehicleComponents.TabPages.Remove(tpElectricMachine)
				tcVehicleComponents.TabPages.Remove(tpReess)

				'GenSet Components --------------------------------------------------------
				tcVehicleComponents.TabPages.Remove(tpGensetComponents)

				'Torque Limits ------------------------------------------------------------
				gbEMTorqueLimits.Enabled = False

				'ADAS ---------------------------------------------------------------------
				cbEngineStopStart.Visible = True
				cbAtEcoRollReleaseLockupClutch.Visible = True
				pnEcoRoll.Visible = True

				'IEPC
				tcVehicleComponents.TabPages.Remove(tpIEPC)

				'IHPC 
				tcVehicleComponents.TabPages.Remove(tbIHPC)

			Case VectoSimulationJobType.ParallelHybridVehicle
				lblTitle.Text = "Parallel Hybrid Vehicle"

				'Powertrain ---------------------------------------------------------------
				gbVehicleIdlingSpeed.Enabled = True
				gbTankSystem.Enabled = True
				gbRetarderLosses.Enabled = True
				gbAngledrive.Enabled = True

				'Electric Powertrain Components -------------------------------------------
				cbEmPos.DataSource = EnumHelper.GetKeyValuePairs(Of PowertrainPosition) _
					(Function(t) t.GetLabel(), Function(x) x.IsParallelHybrid())

				'GenSet Components --------------------------------------------------------
				tcVehicleComponents.TabPages.Remove(tpGensetComponents)

				'Torque Limits ------------------------------------------------------------
				gbEMTorqueLimits.Enabled = True

				'ADAS ---------------------------------------------------------------------
				cbEngineStopStart.Visible = True
				cbAtEcoRollReleaseLockupClutch.Visible = False
				pnEcoRoll.Visible = False
				cbEcoRoll.SelectedIndex = 0

				'IEPC
				tcVehicleComponents.TabPages.Remove(tpIEPC)

				'IHPC 
				tcVehicleComponents.TabPages.Remove(tbIHPC)

			Case VectoSimulationJobType.SerialHybridVehicle
				lblTitle.Text = "Serial Hybrid Vehicle"

				'Powertrain ---------------------------------------------------------------
				gbVehicleIdlingSpeed.Enabled = True
				gbTankSystem.Enabled = True
				gbRetarderLosses.Enabled = False
				gbAngledrive.Enabled = True

				'Electric Powertrain Components -------------------------------------------
				cbEmPos.DataSource = EnumHelper.GetKeyValuePairs(Of PowertrainPosition) _
					(Function(t) t.GetLabel(), Function(x) x.IsSerialHybrid())

				'GenSet Components --------------------------------------------------------
				'-

				'Torque Limits ------------------------------------------------------------
				gbEMTorqueLimits.Enabled = False
				tpTorqueLimits.Enabled = False

				'ADAS ---------------------------------------------------------------------
				cbEngineStopStart.Visible = False
				cbAtEcoRollReleaseLockupClutch.Visible = False
				pnEcoRoll.Visible = False
				cbEcoRoll.SelectedIndex = 0

				'IEPC
				tcVehicleComponents.TabPages.Remove(tpIEPC)

				'IHPC 
				tcVehicleComponents.TabPages.Remove(tbIHPC)

				'PTO
				gbPTODrive.Enabled = False
				pnPtoMode1.Enabled = false
				pnPtoMode3.Enabled = False
				lblNotePtoPEV_HEVS.Visible = True
			Case VectoSimulationJobType.BatteryElectricVehicle
				lblTitle.Text = "Battery Electric Vehicle"

				'Powertrain ---------------------------------------------------------------
				gbVehicleIdlingSpeed.Enabled = False
				gbTankSystem.Enabled = False
				gbRetarderLosses.Enabled = False
				gbAngledrive.Enabled = False

				'Electric Powertrain Components -------------------------------------------
				cbEmPos.DataSource = EnumHelper.GetKeyValuePairs(Of PowertrainPosition) _
					(Function(t) t.GetLabel(), Function(x) x.IsBatteryElectric())

				'GenSet Components --------------------------------------------------------
				tcVehicleComponents.TabPages.Remove(tpGensetComponents)

				'Torque Limits ------------------------------------------------------------
				gbEMTorqueLimits.Enabled = False
				tcVehicleComponents.TabPages.Remove(tpTorqueLimits)

				'ADAS ---------------------------------------------------------------------
				cbEngineStopStart.Visible = False
				cbAtEcoRollReleaseLockupClutch.Visible = False
				pnEcoRoll.Visible = False
				cbEcoRoll.SelectedIndex = 0

				'IEPC
				tcVehicleComponents.TabPages.Remove(tpIEPC)

				'IHPC 
				tcVehicleComponents.TabPages.Remove(tbIHPC)

			    'PTO
			    gbPTODrive.Enabled = False
			    pnPtoMode1.Enabled = false
			    pnPtoMode3.Enabled = False
				lblNotePtoPEV_HEVS.Visible = true
			Case VectoSimulationJobType.IEPC_E
				lblTitle.Text = "IEPC-E Vehicle"
				
				tcVehicleComponents.TabPages.Remove(tpElectricMachine)
				tcVehicleComponents.TabPages.Remove(tpGensetComponents)
				tcVehicleComponents.TabPages.Remove(tbIHPC)

			    'Torque Limits ------------------------------------------------------------
			    gbEMTorqueLimits.Enabled = False
			    tcVehicleComponents.TabPages.Remove(tpTorqueLimits)

				'ADAS ---------------------------------------------------------------------
				cbEngineStopStart.Visible = False
				cbAtEcoRollReleaseLockupClutch.Visible = False
				pnEcoRoll.Visible = False
				cbEcoRoll.SelectedIndex = 0

				'PTO
				gbPTO.Enabled = False
				pnPTO.Enabled = false
			Case VectoSimulationJobType.IEPC_S
				lblTitle.Text = "IEPC-S Vehicle"
				tcVehicleComponents.TabPages.Remove(tpElectricMachine)
				tcVehicleComponents.TabPages.Remove(tbIHPC)

			    'Torque Limits ------------------------------------------------------------
			    gbEMTorqueLimits.Enabled = False
			    tcVehicleComponents.TabPages.Remove(tpTorqueLimits)
				
				'ADAS ---------------------------------------------------------------------
				cbEngineStopStart.Visible = False
				cbAtEcoRollReleaseLockupClutch.Visible = False
				pnEcoRoll.Visible = False
				cbEcoRoll.SelectedIndex = 0

			    'PTO
				gbPTO.Enabled = False
				pnPTO.Enabled = false

			Case VectoSimulationJobType.IHPC
				lblTitle.Text = "IHPC Vehicle"

				tcVehicleComponents.TabPages.Remove(tpElectricMachine)
				tcVehicleComponents.TabPages.Remove(tpGensetComponents)
				tcVehicleComponents.TabPages.Remove(tpIEPC)

				gbEMTorqueLimits.Enabled = False
				gbPropulsionTorque.Enabled = True

			    'PTO
			    gbPTO.Enabled = False
			    pnPTO.Enabled = false

			Case Else
				If Not tcVehicleComponents.TabPages.Contains(tpElectricMachine) Then
					tcVehicleComponents.TabPages.Insert(2, tpElectricMachine)
					tpElectricMachine.BindingContext = BindingContext
				End If

				If Not tcVehicleComponents.TabPages.Contains(tpReess) Then
					tcVehicleComponents.TabPages.Insert(3, tpReess)
					tpReess.BindingContext = BindingContext
				End If

				If Not tcVehicleComponents.TabPages.Contains(tpGensetComponents) Then
					tcVehicleComponents.TabPages.Insert(4, tpGensetComponents)
					tpGensetComponents.BindingContext = BindingContext
				End If
				pnEcoRoll.Visible = True
				cbAtEcoRollReleaseLockupClutch.Visible = True
				cbEngineStopStart.Visible = True
		End Select

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

		veh.VehicleType = VehicleType

		veh.VehicleCategory = CType(CbCat.SelectedValue, VehicleCategory) 'CType(CbCat.SelectedIndex, tVehCat)
		veh.Mass = TbMass.Text.ToDouble(0)
		veh.MassExtra = TbMassExtra.Text.ToDouble(0)
		veh.Loading = TbLoad.Text.ToDouble(0)
		veh.VehicleHeight = tbVehicleHeight.Text.ToDouble(0)
		veh.CdA0 = If(String.IsNullOrWhiteSpace(TBcdA.Text), Double.NaN, TBcdA.Text.ToDouble(0))
		veh.legClass = CType(cbLegislativeClass.SelectedValue, LegislativeClass)
		veh.DynamicTyreRadius = TBrdyn.Text.ToDouble(0)
		veh.CrossWindCorrectionMode = CType(CbCdMode.SelectedValue, CrossWindCorrectionMode)
		veh.CrossWindCorrectionFile.Init(GetPath(file), TbCdFile.Text)

		veh.MassMax = TbMassMass.Text.ToDouble(0)
		veh.MassExtra = TbMassExtra.Text.ToDouble(0)
		veh.AxleConfiguration = CType(CbAxleConfig.SelectedValue, AxleConfiguration)

		For Each entry As ListViewItem In LvRRC.Items
			Dim a0 As AxleInputData = New AxleInputData()
			a0.AxleWeightShare = entry.SubItems(AxleTbl.RelativeLoad).Text.ToDouble(0)
			a0.TwinTyres = (entry.SubItems(AxleTbl.TwinTyres).Text = "yes")
			a0.AxleType = entry.SubItems(AxleTbl.AxleType).Text.ParseEnum(Of AxleType)()
			Dim tyre As TyreInputData = New TyreInputData()
			tyre.RollResistanceCoefficient = entry.SubItems(AxleTbl.RRC).Text.ToDouble(0)
			tyre.TyreTestLoad = entry.SubItems(AxleTbl.FzISO).Text.ToDouble(0).SI(Of Newton)()
			tyre.Dimension = entry.SubItems(AxleTbl.WheelsDimension).Text
			tyre.Inertia = entry.SubItems(AxleTbl.Inertia).Text.ToDouble(0).SI(Of KilogramSquareMeter)()
			a0.Tyre = tyre
			veh.Axles.Add(a0)
		Next

		veh.RetarderType = CType(CbRtType.SelectedValue, RetarderType)
		veh.RetarderRatio = TbRtRatio.Text.ToDouble(0)
		veh.RetarderLossMapFile.Init(GetPath(file), TbRtPath.Text)

		If (VehicleType = VectoSimulationJobType.ConventionalVehicle OrElse VehicleType = VectoSimulationJobType.ParallelHybridVehicle _
            OrElse VehicleType = VectoSimulationJobType.SerialHybridVehicle OrElse VehicleType = VectoSimulationJobType.IHPC) Then
			veh.VehicleidlingSpeed = _tbVehIdlingSpeed.Text.ToDouble(0).RPMtoRad()

			veh.AngledriveType = CType(cbAngledriveType.SelectedValue, AngledriveType)
			veh.AngledriveRatio = tbAngledriveRatio.Text.ToDouble(0)
			veh.AngledriveLossMapFile.Init(GetPath(file), tbAngledriveLossMapPath.Text)

			veh.PtoType = CType(cbPTOType.SelectedValue, String)
			veh.PtoLossMap.Init(GetPath(file), tbPTOLossMap.Text)
			veh.PtoCycleStandstill.Init(GetPath(file), tbPTOCycle.Text)
			veh.PtoCycleDriving.Init(GetPath(file), tbPTODrive.Text)

			For Each item As ListViewItem In lvTorqueLimits.Items
				Dim tl As TorqueLimitInputData = New TorqueLimitInputData()
				tl.Gear() = item.SubItems(TorqueLimitsTbl.Gear).Text.ToInt(0)
				tl.MaxTorque = item.SubItems(TorqueLimitsTbl.MaxTorque).Text.ToDouble(0).SI(Of NewtonMeter)()
				veh.torqueLimitsList.Add(tl)
			Next

			veh.VehicleTankSystem = CType(If(cbTankSystem.SelectedIndex > 0, cbTankSystem.SelectedValue, Nothing), TankSystem?)
		End If

		if (VehicleType = VectoSimulationJobType.BatteryElectricVehicle) Then
		    veh.PtoType = CType(cbPTOType.SelectedValue, String)
		    veh.PtoLossMap.Init(GetPath(file), tbPTOLossMap.Text)
		End If

		'If (VehicleType = VectoSimulationJobType.ParallelHybridVehicle OrElse VehicleType = VectoSimulationJobType.BatteryElectricVehicle OrElse VehicleType = VectoSimulationJobType.SerialHybridVehicle) Then
		If (VehicleType <> VectoSimulationJobType.ConventionalVehicle andalso VehicleType <> VectoSimulationJobType.EngineOnlySimulation)
			For Each reess As ListViewItem In lvREESSPacks.Items
				veh.ReessPacks.Add(Tuple.Create(reess.SubItems(REESPackTbl.ReessFile).Text, reess.SubItems(REESPackTbl.Count).Text.ToInt(), reess.SubItems(REESPackTbl.StringId).Text.ToInt()))
			Next
			veh.InitialSOC = tbInitialSoC.Text.ToDouble(80) / 100.0

			

		    If (VehicleType = VectoSimulationJobType.ParallelHybridVehicle OrElse 
                VehicleType = VectoSimulationJobType.BatteryElectricVehicle OrElse 
                VehicleType = VectoSimulationJobType.SerialHybridVehicle) Then
			    
		        If tbElectricMotor.Text = "" Then
		            MsgBox("Electric Motor File is required.")
		            tcVehicleComponents.SelectedTab = tpElectricMachine
		            tbElectricMotor.Focus()
		            Return False
		        End If
		        veh.ElectricMotorFile.Init(GetPath(file), tbElectricMotor.Text)
			    veh.ElectricMotorPosition = CType(cbEmPos.SelectedValue, PowertrainPosition)
			    veh.ElectricMotorCount = tbEmCount.Text.ToInt(1)
			    veh.ElectricMotorRatio = tbRatioEm.Text.ToDouble(1)
			    'veh.ElectricMotorMechEff = tbEmADCLossMap.Text.ToDouble()
			    If tbEmADCLossMap.Text = "" Then
				    MsgBox("Loss Map EM ADC is required.")
				    tcVehicleComponents.SelectedTab = tpElectricMachine
				    tbEmADCLossMap.Focus()
				    Return False
			    End If

			    veh.ElectricMotorMechLossMap.Init(GetPath(file), tbEmADCLossMap.Text)
			    If (veh.ElectricMotorPosition = PowertrainPosition.HybridP2_5) Then
				    veh.ElectricMotorPerGearRatios = lvRatioPerGear.Items.Cast(Of ListViewItem).Select(Function(item) item.SubItems(RatiosPerGearTbl.Ratio).Text.ToDouble(0)).ToArray()
			    End If
				
		    end if
			if (VehicleType = VectoSimulationJobType.IEPC_S OrElse VehicleType = VectoSimulationJobType.IEPC_E) Then
				veh.IEPCFile.Init(GetPath(file), tbIEPCFilePath.Text)
			End If
		    if (VehicleType = VectoSimulationJobType.IHPC) Then
		        If (tbIHPCFilePath.Text = "") Then
		            MsgBox("IHPC File is required.")
		            tcVehicleComponents.SelectedTab = tbIHPC
		            tbIHPCFilePath.Focus()
		        End If
		        veh.ElectricMotorFile.Init(GetPath(file), tbIHPCFilePath.Text)
		        veh.ElectricMotorPosition = PowertrainPosition.IHPC
		        veh.ElectricMotorCount = 1
		        veh.ElectricMotorRatio = 1
		    End If
		End If

		If (VehicleType = VectoSimulationJobType.SerialHybridVehicle OrElse VehicleType = VectoSimulationJobType.IEPC_S) Then
			If tbGenSetEM.Text = "" Then
				MsgBox("Generator File is required.")
				tcVehicleComponents.SelectedTab = tpGensetComponents
				tbGenSetEM.Focus()
				Return False
			End If
			veh.GenSetEMFile.Init(GetPath(file), tbGenSetEM.Text)
			veh.GenSetPosition = PowertrainPosition.GEN
			veh.GenSetCount = tbGenSetCount.Text.ToInt(1)
			veh.GenSetRatio = tbGenSetRatio.Text.ToDouble(1)
			'veh.ElectricMotorMechEff = tbEmADCLossMap.Text.ToDouble()
			If tbGenSetADC.Text = "" Then
				MsgBox("Loss Map GenSet ADC is required.")
				tcVehicleComponents.SelectedTab = tpGensetComponents
				tbGenSetADC.Focus()
				Return False
			End If
			veh.GenSetMechLossMap.Init(GetPath(file), tbGenSetADC.Text)
		End If


		If (VehicleType = VectoSimulationJobType.ParallelHybridVehicle) AndAlso Not String.IsNullOrWhiteSpace(tbEmTorqueLimits.Text) Then
			veh.EmTorqueLimitsFile.Init(GetPath(file), tbEmTorqueLimits.Text)
		End If

		If ((VehicleType = VectoSimulationJobType.ParallelHybridVehicle OrElse VehicleType = VectoSimulationJobType.IHPC) AndAlso Not String.IsNullOrEmpty(tbPropulsionTorqueLimit.Text)) Then
			veh.PropulsionTorqueFile.Init(GetPath(file), tbPropulsionTorqueLimit.Text)
		End If

		veh.EcoRolltype = CType(cbEcoRoll.SelectedValue, EcoRollType)
		veh.PCC = CType(cbPcc.SelectedValue, PredictiveCruiseControlType)
		veh.EngineStop = cbEngineStopStart.Checked
		veh.EcoRollReleaseLockupClutch = cbAtEcoRollReleaseLockupClutch.Checked


		veh.GearDuringPTODrive = If(String.IsNullOrWhiteSpace(tbPtoGear.Text), Nothing, CType(tbPtoGear.Text.ToInt(), UInteger?))
		veh.EngineSpeedDuringPTODrive = If(String.IsNullOrWhiteSpace(tbPtoEngineSpeed.Text), Nothing, tbPtoEngineSpeed.Text.ToDouble().RPMtoRad())
		'---------------------------------------------------------------------------------
		If Not veh.SaveFile Then
			MsgBox("Cannot save to " & file, MsgBoxStyle.Critical)
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
	Private Sub CbRtType_SelectedIndexChanged(sender As Object, e As EventArgs) Handles CbRtType.SelectedIndexChanged
		Select Case CType(CbRtType.SelectedValue, RetarderType)
			Case RetarderType.TransmissionInputRetarder
				LbRtRatio.Text = "Ratio to engine speed"
				PnRt.Enabled = True
			Case RetarderType.TransmissionOutputRetarder
				LbRtRatio.Text = "Ratio to cardan shaft speed"
				PnRt.Enabled = True
			Case RetarderType.AxlegearInputRetarder
				LbRtRatio.Text = "Ratio to axle shaft speed"
				PnRt.Enabled = True
			Case Else
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
												_axlDlog.CbWheels.Text, _axlDlog.TbI_wheels.Text.ToDouble(0), CType(_axlDlog.cbAxleType.SelectedValue, AxleType)))
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
		_axlDlog.CbWheels.SelectedItem = If(String.IsNullOrWhiteSpace(lv0.SubItems(AxleTbl.WheelsDimension).Text), "-", lv0.SubItems(AxleTbl.WheelsDimension).Text)
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
			gbPTODrive.Enabled = False
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

	Private Sub tbVehIdlingSpeed_TextChanged(sender As Object, e As EventArgs) Handles tbVehIdlingSpeed.TextChanged
		Change()
	End Sub

	Private Sub CheckBox1_CheckedChanged(sender As Object, e As EventArgs) Handles cbAtEcoRollReleaseLockupClutch.CheckedChanged
		Change()
	End Sub

	Private Sub tbVehicleHeight_TextChanged(sender As Object, e As EventArgs) Handles tbVehicleHeight.TextChanged

	End Sub

	Private Sub btnBrowseElectricMotor_Click(sender As Object, e As EventArgs) Handles btnBrowseElectricMotor.Click
		If ElectricMotorFileBrowser.OpenDialog(FileRepl(tbElectricMotor.Text, GetPath(_vehFile))) Then
			tbElectricMotor.Text = GetFilenameWithoutDirectory(ElectricMotorFileBrowser.Files(0), GetPath(_vehFile))
		End If
	End Sub


	Private Sub btnOpenElectricMotor_Click(sender As Object, e As EventArgs) Handles btnOpenElectricMotor.Click
		Dim f As String
		f = FileRepl(tbElectricMotor.Text, GetPath(_vehFile))

		'Thus Veh-file is returned
		ElectricMotorForm.JobDir = GetPath(_vehFile)
		ElectricMotorForm.AutoSendTo = Sub(file, vehicleForm)
										   If UCase(FileRepl(vehicleForm.tbElectricMotor.Text, JobDir)) <> UCase(file) Then _
											   vehicleForm.tbElectricMotor.Text = GetFilenameWithoutDirectory(file, JobDir)
										   VectoJobForm.UpdatePic()
									   End Sub

		If Not Trim(f) = "" Then
			If Not File.Exists(f) Then
				MsgBox("File not found!")
				Exit Sub
			End If
		End If

		If Not ElectricMotorForm.Visible Then
			ElectricMotorForm.Show()
		Else
			If ElectricMotorForm.WindowState = FormWindowState.Minimized Then ElectricMotorForm.WindowState = FormWindowState.Normal
			ElectricMotorForm.BringToFront()
		End If

		If Not Trim(f) = "" Then
			Try
				ElectricMotorForm.OpenElectricMachineFile(f)
			Catch ex As Exception
				MsgBox(ex.Message, MsgBoxStyle.OkOnly, "Error loading Vehicle File")
			End Try
		End If
	End Sub

	Private Sub btnEmTorqueLimits_Click(sender As Object, e As EventArgs) Handles btnEmTorqueLimits.Click
		If ElectricMachineMaxTorqueFileBrowser.OpenDialog(FileRepl(tbEmTorqueLimits.Text, GetPath(_vehFile))) Then _
			tbEmTorqueLimits.Text = GetFilenameWithoutDirectory(ElectricMachineMaxTorqueFileBrowser.Files(0), GetPath(_vehFile))
	End Sub

	Private Sub btnPropulsionTorqueLimit_Click(sender As Object, e As EventArgs) Handles btnPropulsionTorqueLimit.Click
		If PropulsionTorqueLimitFileBrowser.OpenDialog(FileRepl(tbPropulsionTorqueLimit.Text, GetPath(_vehFile))) Then _
			tbPropulsionTorqueLimit.Text = GetFilenameWithoutDirectory(PropulsionTorqueLimitFileBrowser.Files(0), GetPath(_vehFile))

	End Sub

	Private Sub btnEmADCLossMap_Click(sender As Object, e As EventArgs) Handles btnEmADCLossMap.Click
		If EmADCLossMapFileBrowser.OpenDialog(FileRepl(tbEmADCLossMap.Text, GetPath(_vehFile))) Then _
			tbEmADCLossMap.Text = GetFilenameWithoutDirectory(EmADCLossMapFileBrowser.Files(0), GetPath(_vehFile))
	End Sub

	Private Sub btPTOCycleDrive_Click(sender As Object, e As EventArgs) Handles btPTOCycleDrive.Click
		If PTODrivingCycleDrivingFileBrowser.OpenDialog(FileRepl(tbPTODrive.Text, GetPath(_vehFile))) Then
			tbPTODrive.Text = GetFilenameWithoutDirectory(PTODrivingCycleDrivingFileBrowser.Files(0), GetPath(_vehFile))
		End If
	End Sub

	Private Sub cbEmPos_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cbEmPos.SelectedIndexChanged
		gbRatiosPerGear.Enabled = PowertrainPosition.HybridP2_5.Equals(cbEmPos.SelectedValue)
		Dim selectedValue = CbRtType.SelectedValue

		If PowertrainPosition.BatteryElectricE4.Equals(cbEmPos.SelectedValue) Then
			gbRetarderLosses.Enabled = False
			TbRtRatio.Text = ""
			TbRtPath.Text = ""
			CbRtType.SelectedIndex = -1
			CType(CbRtType.DataSource, DataView).RowFilter = $"Key <> {CInt(RetarderType.AxlegearInputRetarder)}"
		ElseIf PowertrainPosition.BatteryElectricE3.Equals(cbEmPos.SelectedValue) Then
			gbRetarderLosses.Enabled = True
			CType(CbRtType.DataSource, DataView).RowFilter = $"Key in ({CInt(RetarderType.None)}, {CInt(RetarderType.AxlegearInputRetarder)})"
		Else
			gbRetarderLosses.Enabled = True
			CType(CbRtType.DataSource, DataView).RowFilter = $"Key <> {CInt(RetarderType.AxlegearInputRetarder)}"
		End If

		If (selectedValue IsNot Nothing) Then
			If Not selectedValue.Equals(CbRtType.SelectedValue) Then
				MsgBox("Retarder has changed due to change of electric motor position. Please check.", MsgBoxStyle.Information)
				CbRtType.SelectedIndex = 0
			End If
		End If
	End Sub

	Private Sub Button1_Click(sender As Object, e As EventArgs) Handles btnAddEMRatio.Click
		_emRatioPerGearDlog.Clear()
		If _emRatioPerGearDlog.ShowDialog() = DialogResult.OK Then
			Dim gear As Integer = _emRatioPerGearDlog.tbGear.Text.ToInt(0)
			For Each entry As ListViewItem In lvRatioPerGear.Items
				If entry.SubItems(TorqueLimitsTbl.Gear).Text.ToInt() = gear Then
					entry.SubItems(TorqueLimitsTbl.MaxTorque).Text = _emRatioPerGearDlog.tbGearRatio.Text.ToDouble(0).ToGUIFormat
					Change()
					Return
				End If
			Next

			lvRatioPerGear.Items.Add(CreateRatioPerGearListViewItem(gear, _emRatioPerGearDlog.tbGearRatio.Text.ToDouble(0)))

			Change()

		End If
	End Sub

	Private Sub Button2_Click(sender As Object, e As EventArgs) Handles btnRemoveEMRatio.Click
		If lvRatioPerGear.SelectedItems.Count = 0 Then
			If lvRatioPerGear.Items.Count = 0 Then
				Exit Sub
			Else
				lvRatioPerGear.Items(lvRatioPerGear.Items.Count - 1).Selected = True
			End If
		End If

		lvRatioPerGear.SelectedItems(0).Remove()
	End Sub

	Private Sub lvRatioPerGear_DoubleClick(sender As Object, e As EventArgs) Handles lvRatioPerGear.DoubleClick
		If lvRatioPerGear.SelectedItems.Count = 0 Then Exit Sub

		Dim entry As ListViewItem = lvRatioPerGear.SelectedItems(0)
		_emRatioPerGearDlog.tbGear.Text = entry.SubItems(RatiosPerGearTbl.Gear).Text
		_emRatioPerGearDlog.tbGear.ReadOnly = True
		_emRatioPerGearDlog.tbGearRatio.Text = entry.SubItems(RatiosPerGearTbl.Ratio).Text
		_emRatioPerGearDlog.tbGearRatio.Focus()
		If (_emRatioPerGearDlog.ShowDialog() = DialogResult.OK) Then
			entry.SubItems(RatiosPerGearTbl.Ratio).Text = _emRatioPerGearDlog.tbGearRatio.Text
		End If
		_emRatioPerGearDlog.tbGear.ReadOnly = False
	End Sub

	Private Sub lvREESSPacks_DoubleClick(sender As Object, e As EventArgs) Handles lvREESSPacks.DoubleClick
		If lvREESSPacks.SelectedItems.Count = 0 Then Exit Sub

		Dim entry As ListViewItem = lvREESSPacks.SelectedItems(0)
		_reessPackDlg._vehFile = _vehFile
		_reessPackDlg.tbBattery.Text = entry.SubItems(REESPackTbl.ReessFile).Text
		_reessPackDlg.tbBatteryPackCnt.Text = entry.SubItems(REESPackTbl.Count).Text
		_reessPackDlg.tbStreamId.Text = entry.SubItems(REESPackTbl.StringId).Text
		_reessPackDlg.tbBattery.Focus()
		If (_reessPackDlg.ShowDialog() = DialogResult.OK) Then
			entry.SubItems(REESPackTbl.ReessFile).Text = _reessPackDlg.tbBattery.Text
			entry.SubItems(REESPackTbl.Count).Text = _reessPackDlg.tbBatteryPackCnt.Text
			entry.SubItems(REESPackTbl.StringId).Text = _reessPackDlg.tbStreamId.Text
		End If
	End Sub

	Private Sub btnAddReessPack_Click(sender As Object, e As EventArgs) Handles btnAddReessPack.Click
		_reessPackDlg.Clear()
		_reessPackDlg._vehFile = _vehFile
		If _reessPackDlg.ShowDialog() = DialogResult.OK Then

			lvREESSPacks.Items.Add(CreateREESSPackListViewItem(_reessPackDlg.tbBattery.Text, _reessPackDlg.tbBatteryPackCnt.Text.ToInt(0), _reessPackDlg.tbStreamId.Text.ToInt(0)))

			Change()

		End If
	End Sub

	Private Sub btnRemoveReessPack_Click(sender As Object, e As EventArgs) Handles btnRemoveReessPack.Click
		If lvREESSPacks.SelectedItems.Count = 0 Then
			If lvREESSPacks.Items.Count = 0 Then
				Exit Sub
			Else
				lvREESSPacks.Items(lvREESSPacks.Items.Count - 1).Selected = True
			End If
		End If

		lvREESSPacks.SelectedItems(0).Remove()
	End Sub

	Private Sub btnOpenGenSetEM_Click(sender As Object, e As EventArgs) Handles btnOpenGenSetEM.Click
		Dim f As String
		f = FileRepl(tbGenSetEM.Text, GetPath(_vehFile))

		'Thus Veh-file is returned
		ElectricMotorForm.JobDir = GetPath(_vehFile)
		ElectricMotorForm.AutoSendTo = Sub(file, vehicleForm)
										   If UCase(FileRepl(vehicleForm.tbGenSetEM.Text, JobDir)) <> UCase(file) Then _
											   vehicleForm.tbGenSetEM.Text = GetFilenameWithoutDirectory(file, JobDir)
										   VectoJobForm.UpdatePic()
									   End Sub

		If Not Trim(f) = "" Then
			If Not File.Exists(f) Then
				MsgBox("File not found!")
				Exit Sub
			End If
		End If

		If Not ElectricMotorForm.Visible Then
			ElectricMotorForm.Show()
		Else
			If ElectricMotorForm.WindowState = FormWindowState.Minimized Then ElectricMotorForm.WindowState = FormWindowState.Normal
			ElectricMotorForm.BringToFront()
		End If

		If Not Trim(f) = "" Then
			Try
				ElectricMotorForm.OpenElectricMachineFile(f)
			Catch ex As Exception
				MsgBox(ex.Message, MsgBoxStyle.OkOnly, "Error loading Vehicle File")
			End Try
		End If
	End Sub

	Private Sub btnBrowseGenSetEM_Click(sender As Object, e As EventArgs) Handles btnBrowseGenSetEM.Click
		If ElectricMotorFileBrowser.OpenDialog(FileRepl(tbGenSetEM.Text, GetPath(_vehFile))) Then
			tbGenSetEM.Text = GetFilenameWithoutDirectory(ElectricMotorFileBrowser.Files(0), GetPath(_vehFile))
		End If
	End Sub

	Private Sub btnGenSetLossMap_Click(sender As Object, e As EventArgs) Handles btnGenSetLossMap.Click
		If EmADCLossMapFileBrowser.OpenDialog(FileRepl(tbGenSetADC.Text, GetPath(_vehFile))) Then _
			tbGenSetADC.Text = GetFilenameWithoutDirectory(EmADCLossMapFileBrowser.Files(0), GetPath(_vehFile))
	End Sub

	Private Sub cbTankSystem_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cbTankSystem.SelectedIndexChanged
		Change()
	End Sub

	Private Sub cbEcoRoll_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cbEcoRoll.SelectedIndexChanged
		Change()
	End Sub

	Private Sub cbPcc_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cbPcc.SelectedIndexChanged
		Change()
	End Sub

	Private Sub cbEngineStopStart_CheckedChanged(sender As Object, e As EventArgs) Handles cbEngineStopStart.CheckedChanged
		Change()
	End Sub

	Private Sub btIEPCFilePath_Click(sender As Object, e As EventArgs) Handles btIEPCFilePath.Click
		If IEPCFileBrowser.OpenDialog(FileRepl(tbIEPCFilePath.Text, GetPath(_vehFile))) Then
			tbIEPCFilePath.Text = GetFilenameWithoutDirectory(IEPCFileBrowser.Files(0), GetPath(_vehFile))
		End If
	End Sub

	Private Sub btnIEPC_Click(sender As Object, e As EventArgs) Handles btnIEPC.Click
		Dim f = FileRepl(tbIEPCFilePath.Text, GetPath(_vehFile))

		IEPCForm.JobDir = GetPath(_vehFile)
	    IEPCForm.AutoSendTo = Sub(file, vehicleForm)
	        If UCase(FileRepl(vehicleForm.tbIEPCFilePath.Text, JobDir)) <> UCase(file) Then _
	            vehicleForm.tbIEPCFilePath.Text = GetFilenameWithoutDirectory(file, JobDir)
	        VectoJobForm.UpdatePic()
	    End Sub

		If Not Trim(f) = "" Then
			If Not File.Exists(f) Then
				MsgBox("File not found!")
				Exit Sub
			End If
		End If

		If Not IEPCForm.Visible Then
			IEPCForm.NewIEPC()
			IEPCForm.Show()
		Else
			If IEPCForm.WindowState = FormWindowState.Minimized Then IEPCForm.WindowState = FormWindowState.Normal
			IEPCForm.BringToFront()
		End If

		If Not Trim(f) = "" Then
			Try
				IEPCForm.ReadIEPCFile(f)
			Catch ex As Exception
				MsgBox(ex.Message, MsgBoxStyle.OkOnly, "Error loading IEPC File")
			End Try
		End If
	End Sub

	Private Sub btIHPCFile_Click(sender As Object, e As EventArgs) Handles btIHPCFile.Click
		If IHPCFileBrowser.OpenDialog(FileRepl(tbIHPCFilePath.Text, GetPath(_vehFile))) Then
			tbIHPCFilePath.Text = GetFilenameWithoutDirectory(IHPCFileBrowser.Files(0), GetPath(_vehFile))
		End If
	End Sub

	Private Sub btIHPC_Click(sender As Object, e As EventArgs) Handles btIHPC.Click
		Dim f = FileRepl(tbIHPCFilePath.Text, GetPath(_vehFile))

		IHPCForm.JobDir = GetPath(_vehFile)
	    IHPCForm.AutoSendTo = Sub(file, vehicleForm)
	        If UCase(FileRepl(vehicleForm.tbIHPCFilePath.Text, JobDir)) <> UCase(file) Then _
	            vehicleForm.tbIHPCFilePath.Text = GetFilenameWithoutDirectory(file, JobDir)
	        VectoJobForm.UpdatePic()
	    End Sub

	    If Not Trim(f) = "" Then
	        If Not File.Exists(f) Then
	            MsgBox("File not found!")
	            Exit Sub
	        End If
	    End If

		If Not IHPCForm.Visible Then
			IHPCForm.ClearIHPC()
			IHPCForm.Show()
		Else
			If IHPCForm.WindowState = FormWindowState.Minimized Then IHPCForm.WindowState = FormWindowState.Normal
			IHPCForm.BringToFront()
		End If

		If Not Trim(f) = "" Then
			Try
				IHPCForm.ReadIHPCFile(f)
			Catch ex As Exception
				MsgBox(ex.Message, MsgBoxStyle.OkOnly, "Error loading IHPC File")
			End Try
		End If
	End Sub
End Class