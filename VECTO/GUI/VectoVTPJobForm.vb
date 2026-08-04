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
Imports System.Windows.Forms.DataVisualization.Charting
Imports Ninject
Imports TUGraz.VECTO.Input_Files
Imports TUGraz.VectoCommon.InputData
Imports TUGraz.VectoCommon.Models
Imports TUGraz.VectoCommon.Utils
Imports TUGraz.VectoCommon.BusAuxiliaries
Imports TUGraz.VectoCore
Imports TUGraz.VectoCore.InputData.FileIO.JSON
Imports TUGraz.VectoCore.InputData.FileIO.XML
Imports TUGraz.VectoCore.InputData.Reader.ComponentData
Imports TUGraz.VectoCore.Models.Declaration
Imports TUGraz.VectoCore.Models.Declaration.Auxiliaries
Imports TUGraz.VectoCore.Models.SimulationComponent.Data.Engine
Imports TUGraz.VectoCore.InputData.Impl
Imports TUGraz.VectoCore.InputData
Imports TUGraz.VectoCore.Ninject

''' <summary>
''' Job Editor. Create/Edit VECTO job files (.vecto)
''' </summary>
''' <remarks></remarks>
Public Class VectoVTPJobForm
    Public VectoFile As String
    Private _changed As Boolean = False

    Dim _xmlInputReader As IXMLInputDataReader

    Dim _vectoJob As IVTPEngineeringJobInputData

    Private Const Mega As Integer = 1000000

    Enum AuxViewColumns
        AuxID = 0
        AuxType = 1
        AuxInputOrTech = 2
    End Enum


    'Initialise form
    Private Sub F02_GEN_Load(sender As Object, e As EventArgs) Handles Me.Load

        Dim kernel As IKernel = New StandardKernel(New VectoNinjectModule)
        _xmlInputReader = kernel.Get(Of IXMLInputDataReader)

        LvAux.Columns(AuxViewColumns.AuxInputOrTech).Width = -2

        LvAux.Columns(AuxViewColumns.AuxInputOrTech).Text = "Technology"

        pnManufacturerRecord.Visible = True
        pnFanParameters.Enabled = Not Cfg.DeclMode
        _ncvGrpBox.Visible = True
        _ncvFuel1Lbl.Visible = True
        _ncvFuel1Txtbox.Visible = True
        _ncvFuel1UnitLbl.Visible = True
        _ncvFuel2Lbl.Visible = True
        _ncvFuel2Txtbox.Visible = True
        _ncvFuel2UnitLbl.Visible = True

        GrCycles.Enabled = True

        _changed = False

        HideGUIComponentsOfNCV()
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

    Private Sub UpdateCompletedVIFElements()
        Dim VIFinputData As ICompletedVIF = Nothing
        Dim PrimaryVIFInputData As IReportFile = Nothing
        Dim CustomerInfoFileInputData As IReportFile = Nothing
        Dim foundOutIfPrimary = False
        Dim isVehiclePrimaryBus = False
        Dim canAccessVIFSource = False
        Dim canAccessPrimaryVIFSource = False
        Dim vehicleFile As String =
            If _
            (Not String.IsNullOrWhiteSpace(VectoFile), Path.Combine(Path.GetDirectoryName(VectoFile), TbVEH.Text),
                TbVEH.Text)

        If File.Exists(vehicleFile) Then
            Try
                Dim input As IDeclarationInputDataProvider =
                            _xmlInputReader.CreateDeclaration(vehicleFile, True)

                isVehiclePrimaryBus = input.JobInputData.Vehicle.VehicleCategory.GetVehicleType() Is VehicleCategoryHelper.PrimaryBus

                foundOutIfPrimary = True
            Catch
            End Try
        End If

        If File.Exists(VectoFile) Then
            Try
                Dim inputData As IVTPEngineeringInputDataProvider = TryCast(JSONInputDataFactory.ReadComponentData(VectoFile),
                    IVTPEngineeringInputDataProvider)

                VIFinputData = inputData.JobInputData.CompletedVIFInputData
                PrimaryVIFInputData = inputData.JobInputData.PrimaryVIFInputData
                CustomerInfoFileInputData = inputData.JobInputData.CIFInputData

                If Not foundOutIfPrimary Then
                    isVehiclePrimaryBus = inputData.JobInputData.Vehicle.VehicleCategory.GetVehicleType() Is VehicleCategoryHelper.PrimaryBus
                End If
            Catch
            End Try
        End If

        completedVIFTxtbox.Enabled = isVehiclePrimaryBus
        primaryVIFTb.Enabled = isVehiclePrimaryBus

        If VIFinputData IsNot Nothing Then
            If VIFinputData.Source IsNot Nothing Then
                canAccessVIFSource = True
            End If
        End If

        If PrimaryVIFInputData IsNot Nothing Then
            If PrimaryVIFInputData.Source IsNot Nothing Then
                canAccessPrimaryVIFSource = True
            End If
        End If

        mrfLbl.Text = If(isVehiclePrimaryBus, "MRF (primary):", "MRF:")
        completedVIFTxtbox.Text = If(canAccessVIFSource And isVehiclePrimaryBus, VIFinputData.Source, "")
        primaryVIFTb.Text = If(canAccessPrimaryVIFSource And isVehiclePrimaryBus, PrimaryVIFInputData.Source, "")
        cifTb.Text = If(canAccessPrimaryVIFSource And isVehiclePrimaryBus, CustomerInfoFileInputData.Source, "")

        completedVIFButton.Enabled = completedVIFTxtbox.Enabled
        primaryVIFBtn.Enabled = primaryVIFTb.Enabled
    End Sub

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
            _vectoJob = vectoJob
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
                Case -1
                    Exit Sub
            End Select
        End If

        VectoFile = file
        _basePath = Path.GetDirectoryName(file)
        'Update Form


        'Files -----------------------------
        TbVEH.Text = GetRelativePath(inputData.JobInputData.Vehicle.DataSource.SourceFile, _basePath)
        tbManufacturerRecord.Text = inputData.JobInputData.ManufacturerReportInputData.Source
        UpdateCompletedVIFElements()

        Dim auxInput As IAuxiliariesDeclarationInputData = inputData.JobInputData.Vehicle.Components.AuxiliaryInputData
        Dim busAux As IBusAuxiliariesDeclarationData = inputData.JobInputData.Vehicle.Components.BusAuxiliaries

        If (auxInput Is Nothing) Then
            PopulateAuxiliaryList(busAux)
        Else
            PopulateAuxiliaryList(auxInput)
        End If

        tbMileage.Text = inputData.JobInputData.Mileage.ConvertToKiloMeter().Value.ToGUIFormat()

        Dim obfcmData = inputData.JobInputData.OBFCMDeclarationInputData
        If (obfcmData.OdometerReading IsNot Nothing) Then
            tbOdometerReading.Text = obfcmData.OdometerReading.ConvertToKiloMeter().Value.ToGUIFormat()
        End If

        If (obfcmData.LifetimeFuelConsumptionMass IsNot Nothing) Then
            tbLifetimeFCMassStart.Text = obfcmData.LifetimeFuelConsumptionMass.Item("LifetimeFuelConsumptionMassStart").Value().ToGUIFormat()
            tbLifetimeFCMassEnd.Text = obfcmData.LifetimeFuelConsumptionMass.Item("LifetimeFuelConsumptionMassEnd").Value().ToGUIFormat()
        End If

        If (obfcmData.LifetimeFuelConsumptionVolume IsNot Nothing) Then
            tbLifetimeFCVolumeStart.Text = obfcmData.LifetimeFuelConsumptionVolume.Item("LifetimeFuelConsumptionVolStart").Value().ToGUIFormat()
            tbLifetimeFCVolumeEnd.Text = obfcmData.LifetimeFuelConsumptionVolume.Item("LifetimeFuelConsumptionVolEnd").Value().ToGUIFormat()
        End If

        If Cfg.DeclMode Then
            tbC1.Text = DeclarationData.VTPMode.FanParameters(0).ToGUIFormat()
            tbC2.Text = DeclarationData.VTPMode.FanParameters(1).ToGUIFormat()
            tbC3.Text = DeclarationData.VTPMode.FanParameters(2).ToGUIFormat()
            tbC4.Text = vectoJob.ManufacturerReportInputData.CoolingFanTechCoefficient.ToString()
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
            If (coefficients.Length >= 4) Then
                tbC4.Text = coefficients(3).ToGUIFormat()
            Else
                tbC4.Text = "1"
            End If
        End If
        tbFanDiameter.Text = (vectoJob.FanDiameter.Value() * 1000).ToGUIFormat()
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

        ReadWheelTorqueDriftsFromVectoJob()

        '-------------------------------------------------------------
    End Sub

    Private Sub PopulateAuxiliaryList(auxInput As IAuxiliariesDeclarationInputData)

        LvAux.Items.Clear()
        If auxInput Is Nothing Then
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

    Private Sub PopulateAuxiliaryList(busAux As IBusAuxiliariesDeclarationData)
        LvAux.Items.Clear()
        If busAux Is Nothing Then
            Return
        End If

        Try
            LvAux.Items.Add(CreateAuxListEntry(AuxiliaryTypeHelper.GetAuxKey(AuxiliaryType.Fan),
                                               AuxiliaryTypeHelper.ToString(AuxiliaryType.Fan),
                                               String.Join("; ", busAux.FanTechnology)))

            LvAux.Items.Add(CreateAuxListEntry(AuxiliaryTypeHelper.GetAuxKey(AuxiliaryType.SteeringPump),
                                           AuxiliaryTypeHelper.ToString(AuxiliaryType.SteeringPump),
                                           String.Join("; ", busAux.SteeringPumpTechnology)))

            LvAux.Items.Add(CreateAuxListEntry(AuxiliaryTypeHelper.GetAuxKey(AuxiliaryType.ElectricSystem),
                                               AuxiliaryTypeHelper.ToString(AuxiliaryType.ElectricSystem),
                                               String.Join("; ", busAux.ElectricSupply.AlternatorTechnology)))

            LvAux.Items.Add(CreateAuxListEntry(AuxiliaryTypeHelper.GetAuxKey(AuxiliaryType.PneumaticSystem),
                                               AuxiliaryTypeHelper.ToString(AuxiliaryType.PneumaticSystem),
                                               String.Join("; ", busAux.PneumaticSupply.CompressorSize)))

            Dim hvac As List(Of String) = New List(Of String)

            If busAux.HVACAux.AdjustableCoolantThermostat Then
                hvac.Add("Adjustable Coolant Thermostat")
            End If

            If busAux.HVACAux.EngineWasteGasHeatExchanger Then
                hvac.Add("Engine Waste Gas Heat Exchanger")
            End If

            If hvac.Count() > 0 Then
                LvAux.Items.Add(CreateAuxListEntry(AuxiliaryTypeHelper.GetAuxKey(AuxiliaryType.HVAC),
                                               AuxiliaryTypeHelper.ToString(AuxiliaryType.HVAC),
                                               String.Join("; ", hvac)))
            End If

        Catch ex As Exception
        End Try

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
        vectoJob.ManufacturerRecord = tbManufacturerRecord.Text
        vectoJob.CompletedVIF = completedVIFTxtbox.Text
        vectoJob.PrimaryVIF = primaryVIFTb.Text
        vectoJob.VehicleCIF = cifTb.Text

        For Each lv0 As ListViewItem In LvCycles.Items
            Dim sb As SubPath = New SubPath
            sb.Init(GetPath(file), lv0.Text)
            vectoJob.CycleFiles.Add(sb)
        Next

        vectoJob.Mileage = tbMileage.Text.ToDouble(0).SI(Unit.SI.Kilo.Meter).Cast(Of Meter)

        vectoJob.FanCoefficients = New Double() {
                                                     tbC1.Text.ToDouble(0),
                                                     tbC2.Text.ToDouble(0),
                                                     tbC3.Text.ToDouble(0),
                                                     tbC4.Text.ToDouble(0)
                                                 }
        vectoJob.FanDiameter = (tbFanDiameter.Text.ToDouble(0) / 1000).SI(Of Meter)

        Dim odometerReadingValue = tbOdometerReading.Text.ToDouble(0).SI(Unit.SI.Kilo.Meter).Cast(Of Meter)
        Dim lifecycleFC = New Dictionary(Of String, Double) From
        {
            {"LifetimeFuelConsumptionMassStart", Val(tbLifetimeFCMassStart.Text)},
            {"LifetimeFuelConsumptionMassEnd", Val(tbLifetimeFCMassEnd.Text)},
            {"LifetimeFuelConsumptionVolStart", Val(tbLifetimeFCVolumeStart.Text)},
            {"LifetimeFuelConsumptionVolEnd", Val(tbLifetimeFCVolumeEnd.Text)}
        }

        vectoJob.OBFCMDeclarationInputData = New VTPOBFCMDeclarationData(lifecycleFC, odometerReadingValue)

        ReadFuelNCVFromGUIComponents(_ncvFuel1Lbl, _ncvFuel1Txtbox, vectoJob)
        ReadFuelNCVFromGUIComponents(_ncvFuel2Lbl, _ncvFuel2Txtbox, vectoJob)

        Dim torqueLeft As Double
        If (Not Double.TryParse(_tqDriftLeftTextbox.Text, torqueLeft)) Then
            Throw New Exception($"Torque Drift for left wheel has invalid value")
        End If
        vectoJob.TorqueDriftLeftWheel = torqueLeft.SI(Of NewtonMeter)

        Dim torqueRight As Double
        If (Not Double.TryParse(_tqDriftRightTextbox.Text, torqueRight)) Then
            Throw New Exception($"Torque Drift for right wheel has invalid value")
        End If
        vectoJob.TorqueDriftRightWheel = torqueRight.SI(Of NewtonMeter)

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

    Public Sub ReadFuelNCVFromGUIComponents(ncvLabel As Label, ncvTextBox As TextBox, vectoJob As VectoVTPJob)
        If (Not ncvLabel.Visible) Then
            Return
        End If

        Dim ncv As Double

        If (Not Double.TryParse(ncvTextBox.Text, ncv)) Then
            Throw New Exception($"Value of {ncvLabel.Text} is not a number!")
        End If

        Dim current As FuelType
        Dim ftype As FuelType

        For Each current In System.Enum.GetValues(GetType(FuelType))
            If (current.GetLabel() = ncvLabel.Text) Then
                ftype = current
            End If
        Next

        vectoJob.FuelNCVs.Add(New FuelNCVData With {.Type = ftype, .NCV = (ncv * Mega).SI(Of JoulePerKilogramm)})
    End Sub

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
        UpdateCompletedVIFElements()
        _tqDriftLeftTextbox.Text = "0"
        _tqDriftRightTextbox.Text = "0"
    End Sub

    Private Sub DeclInit()
        If Not Cfg.DeclMode Then Exit Sub

        tbC1.Text = DeclarationData.VTPMode.FanParameters(0).ToGUIFormat()
        tbC2.Text = DeclarationData.VTPMode.FanParameters(1).ToGUIFormat()
        tbC3.Text = DeclarationData.VTPMode.FanParameters(2).ToGUIFormat()
        tbC4.Text = "1"
    End Sub


#Region "Track changes"

#Region "'Change' Events"

    Private Sub TextBoxVEH_TextChanged(sender As Object, e As EventArgs) _
        Handles TbVEH.TextChanged
        UpdateAuxList()
        UpdatePic()
        UpdateNCVs()
        UpdateFanDiameterGUIComponent()
        UpdateCompletedVIFElements()
        Change()
    End Sub

    Private Sub UpdateFanDiameterGUIComponent()
        Dim vehicleFile As String =
            If _
            (Not String.IsNullOrWhiteSpace(VectoFile), Path.Combine(Path.GetDirectoryName(VectoFile), TbVEH.Text),
                TbVEH.Text)

        Label6.Enabled = True
        Label7.Enabled = True
        tbFanDiameter.Enabled = True

        If File.Exists(vehicleFile) Then
            Try
                Dim inputData As IDeclarationInputDataProvider =
                        _xmlInputReader.CreateDeclaration(vehicleFile, True)

                Dim fanTech = ""

                If inputData.JobInputData.Vehicle.Components.AuxiliaryInputData Is Nothing Then
                    fanTech = inputData.JobInputData.Vehicle.Components.BusAuxiliaries.FanTechnology
                Else
                    Dim auxiliaries As IList(Of IAuxiliaryDeclarationInputData) = inputData.JobInputData.Vehicle.Components.AuxiliaryInputData.Auxiliaries

                    Dim fan As IAuxiliaryDeclarationInputData = auxiliaries.First(Function(aux) aux.Type = AuxiliaryType.Fan)
                    fanTech = fan.Technology.First
                End If

                Dim fullyElectric As Boolean = DeclarationData.Fan.FullyElectricTechnologies().Contains(fanTech)

                Label6.Enabled = Not fullyElectric
                Label7.Enabled = Not fullyElectric
                tbFanDiameter.Enabled = Not fullyElectric
            Catch
            End Try
        End If
    End Sub

    Private Sub ReadWheelTorqueDriftsFromVectoJob()
        Dim leftInput As NewtonMeter = _vectoJob.TorqueDriftLeftWheel
        Dim rightInput As NewtonMeter = _vectoJob.TorqueDriftRightWheel

        If (Not (leftInput Is Nothing)) Then
            _tqDriftLeftTextbox.Text = leftInput.Value.ToString()
        End If

        If (Not (rightInput Is Nothing)) Then
            _tqDriftRightTextbox.Text = rightInput.Value.ToString()
        End If
    End Sub

    Private Sub HideGUIComponentsOfNCV()
        _ncvFuel1Lbl.Visible = False
        _ncvFuel1Txtbox.Visible = False
        _ncvFuel1UnitLbl.Visible = False
        _ncvFuel2Lbl.Visible = False
        _ncvFuel2Txtbox.Visible = False
        _ncvFuel2UnitLbl.Visible = False
    End Sub

    Private Sub UpdateNCVs()
        HideGUIComponentsOfNCV()

        Dim vehicleFile As String =
                If _
                (Not String.IsNullOrWhiteSpace(VectoFile), Path.Combine(Path.GetDirectoryName(VectoFile), TbVEH.Text),
                 TbVEH.Text)

        If File.Exists(vehicleFile) Then
            Try
                Dim inputData As IDeclarationInputDataProvider =
                        _xmlInputReader.CreateDeclaration(vehicleFile, True)

                Dim fuels As IList(Of IEngineFuelDeclarationInputData) =
                    inputData.JobInputData.Vehicle.Components.EngineInputData.EngineModes.First().Fuels

                Dim entry As IEngineFuelDeclarationInputData

                For Each entry In fuels
                    Dim fueldata As IFuelProperties =
                            DeclarationData.FuelData.Lookup(entry.FuelType, inputData.JobInputData.Vehicle.TankSystem)

                    If (Not _ncvFuel1Lbl.Visible) Then
                        _ncvFuel1Lbl.Text = entry.FuelType.GetLabel()
                        _ncvFuel1Lbl.Visible = True

                        _ncvFuel1Txtbox.Text = (fueldata.LowerHeatingValueVecto.Value / Mega).ToString()
                        _ncvFuel1Txtbox.Visible = True
                        _ncvFuel1UnitLbl.Visible = True
                    ElseIf (Not _ncvFuel2Lbl.Visible) Then
                        _ncvFuel2Lbl.Text = entry.FuelType.GetLabel()
                        _ncvFuel2Lbl.Visible = True

                        _ncvFuel2Txtbox.Text = (fueldata.LowerHeatingValueVecto.Value / Mega).ToString()
                        _ncvFuel2Txtbox.Visible = True
                        _ncvFuel2UnitLbl.Visible = True
                    End If
                Next

            Catch
            End Try
        End If

        Dim jobFuelNCV As IFuelNCVData

        If (_vectoJob IsNot Nothing) Then
            For Each jobFuelNCV In _vectoJob.FuelNCVs
                If (_ncvFuel1Lbl.Visible And (_ncvFuel1Lbl.Text.Equals(jobFuelNCV.Type.GetLabel()))) Then
                    _ncvFuel1Txtbox.Text = jobFuelNCV.NCV.ConvertToMegaJoulePerKilogram().Value.ToString()
                End If
                If (_ncvFuel2Lbl.Visible And (_ncvFuel2Lbl.Text.Equals(jobFuelNCV.Type.GetLabel()))) Then
                    _ncvFuel2Txtbox.Text = jobFuelNCV.NCV.ConvertToMegaJoulePerKilogram().Value.ToString()
                End If
            Next
        End If

    End Sub

    Private Sub UpdateAuxList()
        Dim vehicleFile As String =
                If _
                (Not String.IsNullOrWhiteSpace(VectoFile), Path.Combine(Path.GetDirectoryName(VectoFile), TbVEH.Text),
                 TbVEH.Text)
        If File.Exists(vehicleFile) Then
            Try
                Dim inputData As IDeclarationInputDataProvider =
                        _xmlInputReader.CreateDeclaration(vehicleFile, True)
                Dim auxInput As IAuxiliariesDeclarationInputData = inputData.JobInputData.Vehicle.Components.AuxiliaryInputData
                Dim busAux As IBusAuxiliariesDeclarationData = inputData.JobInputData.Vehicle.Components.BusAuxiliaries

                If (auxInput Is Nothing) Then
                    PopulateAuxiliaryList(busAux)
                Else
                    PopulateAuxiliaryList(auxInput)
                End If
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
                        _xmlInputReader.CreateDeclaration(vehicleFile, True)
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
                        _xmlInputReader.CreateDeclaration(vehicleFile, True)
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


        TbEngTxt.Text = $"{(engine.Displacement.Value() * 1000).ToString("0.0")} l {pmax.ToString("#")} kw {engine.Model}"

        Dim fuelConsumptionMap As FuelConsumptionMap = FuelConsumptionMapReader.Create(engine.EngineModes.First().Fuels.First().FuelConsumptionMap)

        s = New Series
        s.Points.DataBindXY(fuelConsumptionMap.Entries.Select(Function(x) x.EngineSpeed.AsRPM).ToArray(),
                            fuelConsumptionMap.Entries.Select(Function(x) x.Torque.Value()).ToArray())
        s.ChartType = SeriesChartType.Point
        s.MarkerSize = 3
        s.Color = Color.Red
        s.Name = "Map"

        If (engine.EngineModes.First().Fuels.Count > 1) Then
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
                (fullLoadCurve.MaxPower.Value() / 1000):F1} kW; n_rated: {fullLoadCurve.RatedSpeed.AsRPM:F0} rpm; n_95h: { _
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
                        _xmlInputReader.CreateDeclaration(vehicleFile, True)
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
                                                 0.SI(Of Kilogram),
                                                 False)
        Catch
            Try
                s0 = DeclarationData.PrimaryBusSegments.Lookup(vehicle.VehicleCategory, vehicle.AxleConfiguration, vehicle.Articulated)
            Catch
            End Try
        End Try
        If s0.Found Then
            HDVclass = s0.VehicleClass
        End If

        PicVehicle.Image = ConvPicPath(HDVclass, False) _
        'Image.FromFile(cDeclaration.ConvPicPath(HDVclass, False))

        TbHVCclass.Text = $"{HDVclass}"
        TbVehCat.Text = vehicle.VehicleCategory.GetCategoryName()   'ConvVehCat(VEH0.VehCat, True)
        TbMass.Text = (vehicle.GrossVehicleMassRating.Value() / 1000) & " t"
        TbAxleConf.Text = vehicle.AxleConfiguration.GetName()   'ConvAxleConf(VEH0.AxleConf)
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

    Private Sub completedVIFButton_Click(sender As Object, e As EventArgs) Handles completedVIFButton.Click
        If CompletedVIFFileBrowser.OpenDialog(FileRepl(completedVIFTxtbox.Text, GetPath(VectoFile))) Then
            completedVIFTxtbox.Text = GetFilenameWithoutDirectory(CompletedVIFFileBrowser.Files(0), GetPath(VectoFile))
        End If
    End Sub

    Private Sub cifBtn_Click(sender As Object, e As EventArgs) Handles cifBtn.Click
        If CompletedVIFFileBrowser.OpenDialog(FileRepl(cifTb.Text, GetPath(VectoFile))) Then
            cifTb.Text = GetFilenameWithoutDirectory(CompletedVIFFileBrowser.Files(0), GetPath(VectoFile))
        End If
    End Sub

    Private Sub primaryVIFBtn_Click(sender As Object, e As EventArgs) Handles primaryVIFBtn.Click
        If CompletedVIFFileBrowser.OpenDialog(FileRepl(primaryVIFTb.Text, GetPath(VectoFile))) Then
            primaryVIFTb.Text = GetFilenameWithoutDirectory(CompletedVIFFileBrowser.Files(0), GetPath(VectoFile))
        End If
    End Sub

End Class


