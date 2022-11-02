
Imports System.Drawing.Imaging
Imports System.IO
Imports System.Linq
Imports System.Windows.Forms.DataVisualization.Charting
Imports TUGraz.VectoCommon.InputData
Imports TUGraz.VectoCommon.Models
Imports TUGraz.VectoCommon.Utils
Imports TUGraz.VectoCore.InputData.FileIO.JSON
Imports TUGraz.VectoCore.InputData.Reader.ComponentData
Imports TUGraz.VectoCore.Models.Declaration
Imports TUGraz.VectoCore.Models.SimulationComponent.Data.Engine
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
Public Class EngineForm
	Private _engFile As String = ""
	Public AutoSendTo As Boolean = False
	Public JobDir As String = ""
	Private _changed As Boolean = False

    Private _contextMenuFiles As String()
    Private SecondFuelTab As TabPage
    private MechanicalWhrTab as TabPage
    private ElectricalWhrTab as TabPage



	'Before closing Editor: Check if file was changed and ask to save.
	Private Sub F_ENG_FormClosing(sender As Object, e As FormClosingEventArgs) Handles Me.FormClosing
		If e.CloseReason <> CloseReason.ApplicationExitCall And e.CloseReason <> CloseReason.WindowsShutDown Then
			e.Cancel = ChangeCheckCancel()
		End If
	End Sub

	'Initialise.
	Private Sub EngineFormLoad(sender As Object, e As EventArgs) Handles Me.Load

		PnInertia.Enabled = Not Cfg.DeclMode
		PnWhtcDeclaration.Enabled = Cfg.DeclMode
		PnWhtcEngineering.Enabled = Not Cfg.DeclMode

		pnElWHRDeclaration.Enabled = Cfg.DeclMode
		pnElWhrEngineering.Enabled = Not Cfg.DeclMode

	    pnMechWHRDeclaration.Enabled = Cfg.DeclMode
	    pnMechWhrEngineering.Enabled = Not Cfg.DeclMode

        pnWhtcFuel2.Enabled = cfg.DeclMode
        pnEngCFFuel2.Enabled = not cfg.DeclMode

	    ElectricalWhrTab = tbWHR.TabPages(1)
	    MechanicalWhrTab = tbWHR.TabPages(2)
	    tbWHR.TabPages.Remove(ElectricalWhrTab)
	    tbWHR.TabPages.Remove(MechanicalWhrTab)

		cbFuelType.Items.Clear()
		cbFuelType.ValueMember = "Value"
		cbFuelType.DisplayMember = "Label"
		cbFuelType.DataSource =
			[Enum].GetValues(GetType(FuelType)).Cast (Of FuelType).Select(
				Function(type) New With {Key .Value = type, .Label = type.GetLabel()}).ToList()

		cbFuelType2.Items.Clear()
		cbFuelType2.ValueMember = "Value"
		cbFuelType2.DisplayMember = "Label"
		cbFuelType2.DataSource =
			[Enum].GetValues(GetType(FuelType)).Cast (Of FuelType).Select(
				Function(type) New With {Key .Value = type, .Label = type.GetLabel()}).ToList()

		_changed = False

        SecondFuelTab = tbDualFuel.TabPages(1)
        tbDualFuel.TabPages.Remove(SecondFuelTab)

        

		NewEngine()
	End Sub

	'Set generic values for Declaration mode.
	Private Sub DeclInit()

		If Not Cfg.DeclMode Then Exit Sub

		Dim gbxType as GearboxType = GearboxType.AMT

		Dim jobFile As String = VectoJobForm.VectoFile
		If Not jobFile Is Nothing AndAlso File.Exists(jobFile) Then

			Dim inputData As IEngineeringInputDataProvider = TryCast(JSONInputDataFactory.ReadJsonJob(jobFile, true), 
																	 IEngineeringInputDataProvider)
			If (not inputData Is Nothing) Then
				Dim gbx as IGearboxDeclarationInputData = inputData.JobInputData.Vehicle.Components.GearboxInputData
				gbxType = gbx.Type
			End If
		End If
		
		TbInertia.Text = DeclarationData.Engine.EngineInertia((TbDispl.Text.ToDouble(0.0)/1000.0/1000.0).SI (Of CubicMeter),
															gbxType).ToGUIFormat()
	End Sub


#Region "Toolbar"

	Private Sub ToolStripBtNew_Click(sender As Object, e As EventArgs) Handles ToolStripBtNew.Click
		NewEngine()
	End Sub

	Private Sub ToolStripBtOpen_Click(sender As Object, e As EventArgs) Handles ToolStripBtOpen.Click
		If EngineFileBrowser.OpenDialog(_engFile) Then
			Try
				OpenEngineFile(EngineFileBrowser.Files(0))
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

		If _engFile = "" Then
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

		VectoJobForm.TbENG.Text = GetFilenameWithoutDirectory(_engFile, JobDir)
	End Sub

	Private Sub ToolStripButton1_Click(sender As Object, e As EventArgs) Handles ToolStripButton1.Click
		If File.Exists(Path.Combine(MyAppPath, "User Manual\help.html")) Then
			Dim defaultBrowserPath As String = BrowserUtils.GetDefaultBrowserPath()
			Process.Start(defaultBrowserPath,
						$"""file://{path.Combine(MyAppPath, "User Manual\help.html#engine-editor")}""")
		Else
			MsgBox("User Manual not found!", MsgBoxStyle.Critical)
		End If
	End Sub

#End Region

	'Create new empty Engine file.
	Private Sub NewEngine()

		If ChangeCheckCancel() Then Exit Sub

		TbName.Text = ""
		TbDispl.Text = ""
		TbInertia.Text = ""
		TbIdleSpeed.Text = ""
		TbMAP.Text = ""
		TbFLD.Text = ""
		TbWHTCurban.Text = ""
		TbWHTCrural.Text = ""
		TbWHTCmw.Text = ""

		tbElWHREngineering.Text = ""
		tbElWHRColdHot.Text = ""
		tbElWHRRegPer.Text = ""
		tbElWHRRural.Text = ""
		tbElWHRUrban.Text = ""
		tbElWHRMotorway.Text = ""

	    tbMechWHREngineering.Text = ""
	    tbMechWHRBFColdHot.Text = ""
	    tbMechWHRRegPerCF.Text = ""
	    tbMechWHRRural.Text = ""
	    tbMechWHRUrban.Text = ""
	    tbMechWHRMotorway.Text = ""
		DeclInit()

		_engFile = ""
		Text = "ENG Editor"
		LbStatus.Text = ""

		_changed = False

		UpdatePic()
	End Sub

	'Open VENG file
	Public Sub OpenEngineFile(file As String)
		Dim engine As IEngineEngineeringInputData

		If ChangeCheckCancel() Then Exit Sub

		Dim inputData As IEngineeringInputDataProvider = TryCast(JSONInputDataFactory.ReadComponentData(file),
																IEngineeringInputDataProvider)

		engine = inputData.JobInputData.Vehicle.Components.EngineInputData


		If Cfg.DeclMode <> engine.SavedInDeclarationMode Then
			Select Case WrongMode()
				Case 1
					Close()
					MainForm.RbDecl.Checked = Not MainForm.RbDecl.Checked
					MainForm.OpenVectoFile(file)
				Case - 1
					Exit Sub
			End Select
		End If

		Dim basePath As String = Path.GetDirectoryName(file)
		TbName.Text = engine.Model
		TbDispl.Text = (engine.Displacement.Value() * 1000.0 * 1000).ToGUIFormat()
		TbInertia.Text = engine.Inertia.ToGUIFormat()
		

		Dim enginemode As IEngineModeEngineeringInputData = engine.engineModes.First()

		TbIdleSpeed.Text = enginemode.IdleSpeed.AsRPM.ToGUIFormat()
		TbFLD.Text = GetRelativePath(enginemode.FullLoadCurve.Source, basePath)

		tbMaxTorque.Text = engine.MaxTorqueDeclared.ToGUIFormat()
		tbRatedPower.Text = (engine.RatedPowerDeclared.Value()/1000).ToGUIFormat()
		tbRatedSpeed.Text = engine.RatedSpeedDeclared.AsRPM.ToGUIFormat()
		
		dim fuel1 As IEngineFuelEngineeringInputData = enginemode.Fuels.First()

		TbMAP.Text = GetRelativePath(fuel1.FuelConsumptionMap.Source, basePath)
		TbWHTCurban.Text = fuel1.WHTCUrban.ToGUIFormat()
		TbWHTCrural.Text = fuel1.WHTCRural.ToGUIFormat()
		TbWHTCmw.Text = fuel1.WHTCMotorway.ToGUIFormat()
		TbWHTCEngineering.Text = fuel1.WHTCEngineering.ToGUIFormat()
		TbColdHotFactor.Text = fuel1.ColdHotBalancingFactor.ToGUIFormat()
		tbRegPerCorrFactor.Text = fuel1.CorrectionFactorRegPer.ToGUIFormat()
		cbFuelType.SelectedValue = fuel1.FuelType

		If (enginemode.Fuels.Count > 1) Then
			cbDualFuel.Checked = True
            If (tbDualFuel.TabPages.Count < 2) then
			    tbDualFuel.TabPages.Add(SecondFuelTab)
            End If
			
			Dim fuel2 As IEngineFuelEngineeringInputData = enginemode.Fuels(1)

			tbMapFuel2.Text = GetRelativePath(fuel2.FuelConsumptionMap.Source, basePath)
			tbWhtcUrbanFuel2.Text = fuel2.WHTCUrban.ToGUIFormat()
			tbWhtcRuralFuel2.Text = fuel2.WHTCRural.ToGUIFormat()
			tbWhtcMotorwayFuel2.Text = fuel2.WHTCMotorway.ToGUIFormat()
			tbEngineeringCFFuel2.Text = fuel2.WHTCEngineering.ToGUIFormat()
			tbColdHotFuel2.Text = fuel2.ColdHotBalancingFactor.ToGUIFormat()
			tbRegPerFuel2.Text = fuel2.CorrectionFactorRegPer.ToGUIFormat()
			cbFuelType2.SelectedValue = fuel2.FuelType
		Else 
			cbDualFuel.Checked = False
		    If (tbDualFuel.TabPages.Count >1) then
		        tbDualFuel.TabPages.Remove(SecondFuelTab)
		    End If
		End If

        cbMechWHRInMap.Checked = (engine.WHRType and WHRType.MechanicalOutputICE) <> 0
	    cbMechWHRNotConnectedCrankshaft.Checked = (engine.WHRType and WHRType.MechanicalOutputDrivetrain) <> 0
	    cbElWHR.Checked = (engine.WHRType and WHRType.ElectricalOutput) <> 0
        
		If (engine.WHRType.IsElectrical()) Then
			Dim whr As IWHRData = enginemode.WasteHeatRecoveryDataElectrical
			If (Cfg.DeclMode) then
				tbElWHRRural.Text = whr.RuralCorrectionFactor.ToGUIFormat()
				tbElWHRUrban.Text = whr.UrbanCorrectionFactor.ToGUIFormat()
				tbElWHRMotorway.Text = whr.MotorwayCorrectionFactor.ToGUIFormat()
				tbElWHRColdHot.Text = whr.BFColdHot.ToGUIFormat()
				tbElWHRRegPer.Text = whr.CFRegPer.ToGUIFormat()
			Else
				tbElWHREngineering.Text = whr.EngineeringCorrectionFactor.ToGUIFormat() 
			end if
		End If

	    If (engine.WHRType and WHRType.MechanicalOutputDrivetrain) <> 0 Then
	        Dim whr As IWHRData = enginemode.WasteHeatRecoveryDataMechanical
	        If (Cfg.DeclMode) then
	            tbMechWHRRural.Text = whr.RuralCorrectionFactor.ToGUIFormat()
	            tbMechWHRUrban.Text = whr.UrbanCorrectionFactor.ToGUIFormat()
	            tbMechWHRMotorway.Text = whr.MotorwayCorrectionFactor.ToGUIFormat()
	            tbMechWHRBFColdHot.Text = whr.BFColdHot.ToGUIFormat()
	            tbMechWHRRegPerCF.Text = whr.CFRegPer.ToGUIFormat()
	        Else
	            tbMechWHREngineering.Text = whr.EngineeringCorrectionFactor.ToGUIFormat() 
	        end if
        Else 
            if (tbWHR.TabPages.Count > 1) then
                tbWHR.TabPages.Remove(tbMechanicalWHR)
            End If
	    End If

		DeclInit()

		EngineFileBrowser.UpdateHistory(file)
		Text = GetFilenameWithoutPath(file, True)
		LbStatus.Text = ""
		_engFile = file
		Activate()

		_changed = False
		UpdatePic()
	End Sub

	'Save or Save As function = true if file is saved
	Private Function SaveOrSaveAs(ByVal saveAs As Boolean) As Boolean
		If _engFile = "" Or saveAs Then
			If EngineFileBrowser.SaveDialog(_engFile) Then
				_engFile = EngineFileBrowser.Files(0)
			Else
				Return False
			End If
		End If
		Return SaveEngineToFile(_engFile)
	End Function

	'Save VENG file to given filepath. Called by SaveOrSaveAs. 
	Private Function SaveEngineToFile(ByVal file As String) As Boolean

		Dim engine As Engine = New Engine
		engine.FilePath = file

		engine.ModelName = TbName.Text
		If Trim(engine.ModelName) = "" Then engine.ModelName = "Undefined"
		engine.Displacement = TbDispl.Text.ToDouble(0)
		engine.EngineInertia = TbInertia.Text.ToDouble(0)
		engine.IdleSpeed = TbIdleSpeed.Text.ToDouble(0)

		engine.PathFld = TbFLD.Text

        engine.PrimaryEngineFuel.PathMap = TbMAP.Text
        engine.PrimaryEngineFuel.WHTCUrbanInput = TbWHTCurban.Text.ToDouble(0)
		engine.PrimaryEngineFuel.WHTCRuralInput = TbWHTCrural.Text.ToDouble(0)
		engine.PrimaryEngineFuel.WHTCMotorwayInput = TbWHTCmw.Text.ToDouble(0)
		engine.PrimaryEngineFuel.WHTCEngineeringInput = TbWHTCEngineering.Text.ToDouble(0)
		engine.PrimaryEngineFuel.correctionFactorRegPerInput = tbRegPerCorrFactor.Text.ToDouble(0)
        engine.PrimaryEngineFuel.ColdHotBalancingFactorInput = TbColdHotFactor.Text.ToDouble(0)
        engine.PrimaryEngineFuel.FuelTypeInput = CType(cbFuelType.SelectedValue, FuelType)

        If (cbDualFuel.Checked) Then
            engine.DualFuelInput = True

            engine.SecondaryEngineFuel.PathMap = tbMapFuel2.Text
            engine.SecondaryEngineFuel.WHTCUrbanInput = tbWhtcUrbanFuel2.Text.ToDouble(0)
            engine.SecondaryEngineFuel.WHTCRuralInput = tbWhtcRuralFuel2.Text.ToDouble(0)
            engine.SecondaryEngineFuel.WHTCMotorwayInput = tbWhtcMotorwayFuel2.Text.ToDouble(0)
            engine.SecondaryEngineFuel.WHTCEngineeringInput = tbEngineeringCFFuel2.Text.ToDouble(0)
            engine.SecondaryEngineFuel.correctionFactorRegPerInput = tbRegPerFuel2.Text.ToDouble(0)
            engine.SecondaryEngineFuel.ColdHotBalancingFactorInput = tbColdHotFuel2.Text.ToDouble(0)
            engine.SecondaryEngineFuel.FuelTypeInput = CType(cbFuelType2.SelectedValue, FuelType)
        End If

        engine.WHRTypeInput = GetWHRSelection()
        if (engine.WHRType.IsElectrical()) then
            engine.ElectricalWHRData = new WHRData(engine)
            engine.ElectricalWHRData.WHRUrbanInput = tbElWHRUrban.Text.ToDouble(0)
		    engine.ElectricalWHRData.WHRRuralInput = tbElWHRRural.Text.ToDouble(0)
		    engine.ElectricalWHRData.WHRMotorwayInput = tbElWHRMotorway.Text.ToDouble(0)
		    engine.ElectricalWHRData.WHRColdHotInput = tbElWHRColdHot.Text.ToDouble(0)
		    engine.ElectricalWHRData.WHRRegPerInput = tbElWHRRegPer.Text.ToDouble(0)
		    engine.ElectricalWHRData.WHREngineeringInput = tbElWHREngineering.Text.ToDouble(0)
        End If

        if (engine.WHRType and WHRType.MechanicalOutputDrivetrain) <> 0
            engine.MechanicalWHRData = new WHRData(engine)
            engine.MechanicalWHRData.WHRUrbanInput = tbMechWHRUrban.Text.ToDouble(0)
            engine.MechanicalWHRData.WHRRuralInput = tbMechWHRRural.Text.ToDouble(0)
            engine.MechanicalWHRData.WHRMotorwayInput = tbMechWHRMotorway.Text.ToDouble(0)
            engine.MechanicalWHRData.WHRColdHotInput = tbMechWHRBFColdHot.Text.ToDouble(0)
            engine.MechanicalWHRData.WHRRegPerInput = tbMechWHRRegPerCF.Text.ToDouble(0)
            engine.MechanicalWHRData.WHREngineeringInput = tbMechWHREngineering.Text.ToDouble(0)
        End If

		engine.ratedPowerInput = (tbRatedPower.Text.ToDouble(0)*1000).SI (Of Watt)()
		engine.ratedSpeedInput = tbRatedSpeed.Text.ToDouble(0).RPMtoRad()
		engine.maxTorqueInput = tbMaxTorque.Text.ToDouble(0).SI (Of NewtonMeter)()

		If Not engine.SaveFile Then
            MsgBox("Cannot save to " & file, MsgBoxStyle.Critical)
            Return False
		End If

		If AutoSendTo Then
			If VectoJobForm.Visible Then
				If UCase(FileRepl(VectoJobForm.TbENG.Text, JobDir)) <> UCase(file) Then _
					VectoJobForm.TbENG.Text = GetFilenameWithoutDirectory(file, JobDir)
				VectoJobForm.UpdatePic()
			End If
		End If

		EngineFileBrowser.UpdateHistory(file)
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


	Private Sub TbName_TextChanged(sender As Object, e As EventArgs) Handles TbName.TextChanged
		Change()
	End Sub

	Private Sub TbDispl_TextChanged(sender As Object, e As EventArgs) Handles TbDispl.TextChanged
		Change()
		DeclInit()
	End Sub

	Private Sub TbInertia_TextChanged(sender As Object, e As EventArgs) Handles TbInertia.TextChanged
		Change()
	End Sub

	Private Sub TbNleerl_TextChanged(sender As Object, e As EventArgs) Handles TbIdleSpeed.TextChanged
		UpdatePic()
		Change()
	End Sub

	Private Sub TbMAP_TextChanged(sender As Object, e As EventArgs) _
		Handles TbMAP.TextChanged, TbFLD.TextChanged
		UpdatePic()
		Change()
	End Sub

	Private Sub TbWHTCurban_TextChanged(sender As Object, e As EventArgs) Handles TbWHTCurban.TextChanged
		Change()
	End Sub

	Private Sub TbWHTCrural_TextChanged(sender As Object, e As EventArgs) Handles TbWHTCrural.TextChanged
		Change()
	End Sub

	Private Sub TbWHTCmw_TextChanged(sender As Object, e As EventArgs) Handles TbWHTCmw.TextChanged
		Change()
	End Sub


#End Region

	'Browse for VMAP file
	Private Sub BtMAP_Click(sender As Object, e As EventArgs) Handles BtMAP.Click
		If FuelConsumptionMapFileBrowser.OpenDialog(FileRepl(TbMAP.Text, GetPath(_engFile))) Then _
			TbMAP.Text = GetFilenameWithoutDirectory(FuelConsumptionMapFileBrowser.Files(0), GetPath(_engFile))
	End Sub


	'Open VMAP file
	Private Sub BtMAPopen_Click(sender As Object, e As EventArgs) Handles BtMAPopen.Click
		Dim fldfile As String

		fldfile = FileRepl(TbFLD.Text, GetPath(_engFile))

		If fldfile <> NoFile AndAlso File.Exists(fldfile) Then
			OpenFiles(FileRepl(TbMAP.Text, GetPath(_engFile)), fldfile)
		Else
			OpenFiles(FileRepl(TbMAP.Text, GetPath(_engFile)))
		End If
	End Sub


	'Save and close
	Private Sub ButOK_Click(sender As Object, e As EventArgs) Handles ButOK.Click
		If SaveOrSaveAs(False) Then Close()
	End Sub

	'Close without saving (see FormClosing Event)
	Private Sub ButCancel_Click(sender As Object, e As EventArgs) Handles ButCancel.Click
		Close()
	End Sub

	Private Sub UpdatePic()
		Dim fullLoadCurve As EngineFullLoadCurve = Nothing
		Dim fcMap As FuelConsumptionMap = Nothing
        Dim fcMap2 As FuelConsumptionMap = Nothing

		Dim engineCharacteristics As String = ""

		PicBox.Image = Nothing

		'If Not File.Exists(_engFile) Then Exit Sub

		Try
			Dim fldFile As String =
					If(Not String.IsNullOrWhiteSpace(_engFile), Path.Combine(Path.GetDirectoryName(_engFile), TbFLD.Text), TbFLD.Text)
			If File.Exists(fldFile) Then _
				fullLoadCurve = FullLoadCurveReader.Create(VectoCSVFile.Read(fldFile))
		Catch ex As Exception
		End Try

		Try
			Dim fcFile As String =
					If(Not String.IsNullOrWhiteSpace(_engFile), Path.Combine(Path.GetDirectoryName(_engFile), TbMAP.Text), TbMAP.Text)
			If File.Exists(fcFile) Then fcMap = FuelConsumptionMapReader.Create(VectoCSVFile.Read(fcFile))
		Catch ex As Exception
		End Try

        Try 
            If (cbDualFuel.Checked) then
                Dim fcFile As String =
                        If(Not String.IsNullOrWhiteSpace(_engFile), Path.Combine(Path.GetDirectoryName(_engFile), tbMapFuel2.Text), tbMapFuel2.Text)
                If File.Exists(fcFile) Then fcMap2 = FuelConsumptionMapReader.Create(VectoCSVFile.Read(fcFile))
            End if
        Catch ex As Exception

        End Try

		If fullLoadCurve Is Nothing AndAlso fcMap Is Nothing AndAlso fcmap2 Is nothing Then Exit Sub


		'Create plot
		Dim chart As Chart = New Chart
		chart.Width = PicBox.Width
		chart.Height = PicBox.Height

		Dim chartArea As ChartArea = New ChartArea

		If Not fullLoadCurve Is Nothing Then
			Dim series As Series = New Series
			series.Points.DataBindXY(fullLoadCurve.FullLoadEntries.Select(Function(x) x.EngineSpeed.AsRPM).ToArray(),
									fullLoadCurve.FullLoadEntries.Select(Function(x) x.TorqueFullLoad.Value()).ToArray())
			series.ChartType = SeriesChartType.FastLine
			series.BorderWidth = 2
			series.Color = Color.DarkBlue
			series.Name = "Full load (" & TbFLD.Text & ")"
			chart.Series.Add(series)

			series = New Series
			series.Points.DataBindXY(fullLoadCurve.FullLoadEntries.Select(Function(x) x.EngineSpeed.AsRPM).ToArray(),
									fullLoadCurve.FullLoadEntries.Select(Function(x) x.TorqueDrag.Value()).ToArray())
			series.ChartType = SeriesChartType.FastLine
			series.BorderWidth = 2
			series.Color = Color.Blue
			series.Name = "Motoring (" & Path.GetFileNameWithoutExtension(TbMAP.Text) & ")"
			chart.Series.Add(series)

			engineCharacteristics += $"Max. Torque: {fullLoadCurve.MaxTorque.Value():F0} Nm; Max. Power: {(fullLoadCurve.MaxPower.Value()/1000):F1} kW; n_rated: {fullLoadCurve.RatedSpeed.AsRPM:F0} rpm; n_95h: {fullLoadCurve.N95hSpeed.AsRPM:F0} rpm"
		End If

	    If Not fcMap2 Is Nothing Then
	        Dim series As Series = New Series
	        series.Points.DataBindXY(fcMap2.Entries.Select(Function(x) x.EngineSpeed.AsRPM).ToArray(),
                                     fcMap2.Entries.Select(Function(x) x.Torque.Value()).ToArray())
	        series.ChartType = SeriesChartType.Point
	        series.MarkerSize = 3
	        series.Color = Color.Green
	        series.Name = "Map 2"
	        chart.Series.Add(series)
	    End If

		If Not fcMap Is Nothing Then
			Dim series As Series = New Series
			series.Points.DataBindXY(fcMap.Entries.Select(Function(x) x.EngineSpeed.AsRPM).ToArray(),
									fcMap.Entries.Select(Function(x) x.Torque.Value()).ToArray())
			series.ChartType = SeriesChartType.Point
			series.MarkerSize = 3
			series.Color = Color.Red
			series.Name = "Map"
			chart.Series.Add(series)
		End If

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
		lblEngineCharacteristics.Text = engineCharacteristics
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


	Private Sub BtFLD_Click(sender As Object, e As EventArgs) Handles BtFLD.Click
		If FullLoadCurveFileBrowser.OpenDialog(FileRepl(TbFLD.Text, GetPath(_engFile))) Then _
			TbFLD.Text = GetFilenameWithoutDirectory(FullLoadCurveFileBrowser.Files(0), GetPath(_engFile))
	End Sub

	Private Sub BtFLDopen_Click(sender As Object, e As EventArgs) Handles BtFLDopen.Click
		Dim fldfile As String

		fldfile = FileRepl(TbFLD.Text, GetPath(_engFile))

		If fldfile <> NoFile AndAlso File.Exists(fldfile) Then
			OpenFiles(fldfile)
		End If
	End Sub

	Private Sub btMapFuel2_Click(sender As Object, e As EventArgs) Handles btMapFuel2.Click
		If FuelConsumptionMapFileBrowser.OpenDialog(FileRepl(tbMapFuel2.Text, GetPath(_engFile))) Then _
			tbMapFuel2.Text = GetFilenameWithoutDirectory(FuelConsumptionMapFileBrowser.Files(0), GetPath(_engFile))

	End Sub

	Private Sub btMapOpenFuel2_Click(sender As Object, e As EventArgs) Handles btMapOpenFuel2.Click
		Dim fldfile As String

		fldfile = FileRepl(TbFLD.Text, GetPath(_engFile))

		If fldfile <> NoFile AndAlso File.Exists(fldfile) Then
			OpenFiles(FileRepl(tbMapFuel2.Text, GetPath(_engFile)), fldfile)
		Else
			OpenFiles(FileRepl(tbMapFuel2.Text, GetPath(_engFile)))
		End If
	End Sub

    Private Sub cbDuaFuel_CheckedChanged(sender As Object, e As EventArgs) Handles cbDualFuel.CheckedChanged
        If (cbDualFuel.Checked) Then
            If (not tbDualFuel.TabPages.Contains(SecondFuelTab)) then
                tbDualFuel.TabPages.Add(SecondFuelTab)
            End If
        Else 
            If (tbDualFuel.TabPages.Contains(SecondFuelTab)) then
                tbDualFuel.TabPages.Remove(SecondFuelTab)
            End If
        End If
    End Sub

    Private Sub cbMechWHRNotConnectedCrankshaft_CheckedChanged(sender As Object, e As EventArgs) Handles cbMechWHRNotConnectedCrankshaft.CheckedChanged
       UpdateWHRTabs()
    End Sub

    Private Sub cbElWHR_CheckedChanged(sender As Object, e As EventArgs) Handles cbElWHR.CheckedChanged
        UpdateWHRTabs()
    End Sub

    Private Sub UpdateWHRTabs()
        dim whrType as WHRType = GetWHRSelection()
       

        tbWHR.TabPages.Remove(tbElectricalWHR)
        tbWHR.TabPages.Remove(tbMechanicalWHR)
        if ( whrtype and WHRType.MechanicalOutputDrivetrain) <> 0 Then
            if (not tbWHR.TabPages.Contains(tbMechanicalWHR)) then
                tbWHR.TabPages.Add(MechanicalWhrTab)
            End If
        End If
        If (whrtype and WHRType.ElectricalOutput) <> 0 Then
            if (Not tbWHR.TabPages.Contains(tbElectricalWHR)) then
                tbWHR.TabPages.Add(ElectricalWhrTab)
            End If
        End If
    End Sub

    private Function GetWHRSelection() As WHRType
        dim whrType as WHRType = WHRType.None
        if (cbMechWHRInMap.Checked)
            whrType = whrType or whrType.MechanicalOutputICE
        End If
        if (cbMechWHRNotConnectedCrankshaft.Checked)
            whrType = whrType or whrType.MechanicalOutputDrivetrain
        End If
        if (cbElWHR.Checked)
            whrType = whrType or whrType.ElectricalOutput
        End If
        return whrType
    End Function
End Class
