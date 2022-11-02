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
Imports System.IO
Imports TUGraz.VectoCommon.InputData
Imports TUGraz.VectoCommon.Utils
Imports TUGraz.VectoCore.InputData.FileIO.JSON

''' <summary>
''' HybridStrategyParams Editor. Open and save .VENG files.
''' </summary>
Public Class HybridStrategyParamsForm

    Private _strategyParamsFile As String = ""

    Private _changed As Boolean = False

    Public Property AutoSendTo As Boolean = False

    Public Property JobDir As String = ""

    Public Property JobType As VectoSimulationJobType

    Private Sub F_ENG_FormClosing(sender As Object, e As FormClosingEventArgs) Handles Me.FormClosing
        If e.CloseReason <> CloseReason.ApplicationExitCall AndAlso e.CloseReason <> CloseReason.WindowsShutDown Then
            e.Cancel = ChangeCheckCancel()
        End If
    End Sub

    Private Sub HybridStrategyParamsFormLoad(sender As Object, e As EventArgs) Handles Me.Load
        NewHybStrategyParams()
    End Sub


#Region "Toolbar"

    Private Sub ToolStripBtNew_Click(sender As Object, e As EventArgs) Handles ToolStripBtNew.Click
        NewHybStrategyParams()
    End Sub

    Private Sub ToolStripBtOpen_Click(sender As Object, e As EventArgs) Handles ToolStripBtOpen.Click
        If HCUFileBrowser.OpenDialog(_strategyParamsFile) Then
            Try
                OpenHybridStrategyParametersFile(HCUFileBrowser.Files(0))
            Catch ex As Exception
                MsgBox(ex.Message, MsgBoxStyle.OkOnly, "Error loading HybridStrategyParams File")
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

        If _strategyParamsFile = "" Then
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

        VectoJobForm.TbENG.Text = GetFilenameWithoutDirectory(_strategyParamsFile, JobDir)
    End Sub

    Private Sub ToolStripButton1_Click(sender As Object, e As EventArgs) Handles ToolStripButton1.Click
        If File.Exists(Path.Combine(MyAppPath, "User Manual\help.html")) Then
            Dim defaultBrowserPath As String = BrowserUtils.GetDefaultBrowserPath()
            Process.Start(defaultBrowserPath, $"""file://{Path.Combine(MyAppPath, "User Manual\help.html#hybrid-strategy-parameters-editor")}""")
        Else
            MsgBox("User Manual not found!", MsgBoxStyle.Critical)
        End If
    End Sub

#End Region

    Private Sub NewHybStrategyParams()
        If ChangeCheckCancel() Then Exit Sub
        UpdateForm(JobType)
        _changed = False
    End Sub

    Private Sub UpdateForm(vectoSimulationJobType As VectoSimulationJobType)
        _strategyParamsFile = ""
        Text = "Hybrid Strategy Parameters Editor"
        LbStatus.Text = ""

        tbEquivalenceFactorDischarge.ResetText()
        tbEquivalenceFactorCharge.ResetText()
        tbMinSoC.ResetText()
        tbMaxSoC.ResetText()
        tbTargetSoC.ResetText()
        tbMinICEOnTime.ResetText()
        tbauxBufferTime.ResetText()
        tbAuxBufferChargeTime.ResetText()
        tbICEStartPenaltyFactor.ResetText()
        tbCostFactorSoCExponent.ResetText()
        tbGensetMinOptPowerFactor.ResetText()

        Select Case vectoSimulationJobType
            Case VectoSimulationJobType.ParallelHybridVehicle
                pnEquivFactor.Enabled = True
                pnEquivFactorCharge.Enabled = True
                pnMinSoC.Enabled = True
                pnMaxSoC.Enabled = True
                pnTargetSoC.Enabled = True
                pnICEOnTime.Enabled = True
                pnAuxBufferTime.Enabled = True
                pnAuxBufferChgTime.Enabled = True
                pnICEStartPenaltyFactor.Enabled = True
                pnCostFactorSoCExponent.Enabled = True
                pnGenset.Enabled = False
            Case VectoSimulationJobType.SerialHybridVehicle
            case VectoSimulationJobType.IEPC_S
                pnEquivFactor.Enabled = False
                pnEquivFactorCharge.Enabled = False
                pnMinSoC.Enabled = True
                pnMaxSoC.Enabled = False
                pnTargetSoC.Enabled = True
                pnICEOnTime.Enabled = False
                pnAuxBufferTime.Enabled = False
                pnAuxBufferChgTime.Enabled = False
                pnICEStartPenaltyFactor.Enabled = False
                pnCostFactorSoCExponent.Enabled = False
                pnGenset.Enabled = False
            Case Else
                pnEquivFactor.Enabled = True
                pnEquivFactorCharge.Enabled = True
                pnMinSoC.Enabled = True
                pnMaxSoC.Enabled = True
                pnTargetSoC.Enabled = True
                pnICEOnTime.Enabled = True
                pnAuxBufferTime.Enabled = True
                pnAuxBufferChgTime.Enabled = True
                pnICEStartPenaltyFactor.Enabled = True
                pnCostFactorSoCExponent.Enabled = True
                pnGenset.Enabled = True
        End Select
    End Sub

    Public Sub OpenHybridStrategyParametersFile(file As String)
        If ChangeCheckCancel() Then Exit Sub

        If Cfg.DeclMode Then
            Select Case WrongMode()
                Case 1
                    Close()
                    MainForm.RbDecl.Checked = Not MainForm.RbDecl.Checked
                    MainForm.OpenVectoFile(file)
                Case -1
                    Exit Sub
            End Select
        End If

        UpdateForm(JobType)
        Dim inputData = TryCast(JSONInputDataFactory.ReadComponentData(file), IEngineeringInputDataProvider)
        Dim strategyParams As IHybridStrategyParameters = inputData.JobInputData.HybridStrategyParameters

        Select Case JobType
            Case VectoSimulationJobType.ParallelHybridVehicle
                tbEquivalenceFactorDischarge.Text = strategyParams.EquivalenceFactorDischarge.ToGUIFormat()
                tbEquivalenceFactorCharge.Text = strategyParams.EquivalenceFactorCharge.ToGUIFormat()
                tbMinSoC.Text = (strategyParams.MinSoC * 100).ToGUIFormat()
                tbMaxSoC.Text = (strategyParams.MaxSoC * 100).ToGUIFormat()
                tbTargetSoC.Text = (strategyParams.TargetSoC * 100).ToGUIFormat()

                tbMinICEOnTime.Text = strategyParams.MinimumICEOnTime.ToGUIFormat()
                tbauxBufferTime.Text = strategyParams.AuxBufferTime.ToGUIFormat()
                tbAuxBufferChargeTime.Text = strategyParams.AuxBufferChargeTime.ToGUIFormat()

                tbICEStartPenaltyFactor.Text = strategyParams.ICEStartPenaltyFactor.ToGUIFormat()
                tbCostFactorSoCExponent.Text = If(Double.IsNaN(strategyParams.CostFactorSOCExpponent), 5, strategyParams.CostFactorSOCExpponent).ToGUIFormat()

            Case VectoSimulationJobType.SerialHybridVehicle
            Case VectoSimulationJobType.IEPC_S
                tbMinSoC.Text = (strategyParams.MinSoC * 100).ToGUIFormat()
                tbTargetSoC.Text = (strategyParams.TargetSoC * 100).ToGUIFormat()

            Case Else
                tbEquivalenceFactorDischarge.Text = strategyParams.EquivalenceFactorDischarge.ToGUIFormat()
                tbEquivalenceFactorCharge.Text = strategyParams.EquivalenceFactorCharge.ToGUIFormat()
                tbMinSoC.Text = (strategyParams.MinSoC * 100).ToGUIFormat()
                tbMaxSoC.Text = (strategyParams.MaxSoC * 100).ToGUIFormat()
                tbTargetSoC.Text = (strategyParams.TargetSoC * 100).ToGUIFormat()

                tbMinICEOnTime.Text = strategyParams.MinimumICEOnTime.ToGUIFormat()
                tbauxBufferTime.Text = strategyParams.AuxBufferTime.ToGUIFormat()
                tbAuxBufferChargeTime.Text = strategyParams.AuxBufferChargeTime.ToGUIFormat()

                tbICEStartPenaltyFactor.Text = strategyParams.ICEStartPenaltyFactor.ToGUIFormat()
                tbCostFactorSoCExponent.Text = If(Double.IsNaN(strategyParams.CostFactorSOCExpponent), 5, strategyParams.CostFactorSOCExpponent).ToGUIFormat()
                tbGensetMinOptPowerFactor.Text = If(Double.IsNaN(strategyParams.GensetMinOptPowerFactor), "", strategyParams.GensetMinOptPowerFactor.ToGUIFormat())
        End Select

        REESSFileBrowser.UpdateHistory(file)
        Text = GetFilenameWithoutPath(file, True)
        LbStatus.Text = ""
        _strategyParamsFile = file

        Activate()

        _changed = False
    End Sub

    'Save or Save As function = true if file is saved
    Private Function SaveOrSaveAs(saveAs As Boolean) As Boolean
        If _strategyParamsFile = "" Or saveAs Then
            If HCUFileBrowser.SaveDialog(_strategyParamsFile) Then
                _strategyParamsFile = HCUFileBrowser.Files(0)
            Else
                Return False
            End If
        End If
        Return SaveParamsToFile(_strategyParamsFile)
    End Function

    'Save VENG file to given filepath.
    Private Function SaveParamsToFile(file As String) As Boolean
        Dim strategyParams = New HybridStrategyParams With {
                .FilePath = file,
                .EquivalenceFactorDischarge = tbEquivalenceFactorDischarge.Text.ToDouble(0),
                .EquivalenceFactorCharge = tbEquivalenceFactorCharge.Text.ToDouble(0),
                .MinSoC = tbMinSoC.Text.ToDouble(0) / 100,
                .MaxSoC = tbMaxSoC.Text.ToDouble(0) / 100,
                .TargetSoC = tbTargetSoC.Text.ToDouble(0) / 100,
                .MinimumIceOnTime = tbMinICEOnTime.Text.ToDouble(0),
                .AuxiliaryBufferTime = tbauxBufferTime.Text.ToDouble(0),
                .AuxiliaryBufferChgTime = tbAuxBufferChargeTime.Text.ToDouble(0),
                .ICEStartPenaltyFactor = tbICEStartPenaltyFactor.Text.ToDouble(0),
                .CostFactorSOCExpponent = tbCostFactorSoCExponent.Text.ToDouble(0),
                .GensetMinOptPowerFactor = tbGensetMinOptPowerFactor.Text.ToDouble(0)
                }

        If Not strategyParams.SaveFile Then
            MsgBox("Cannot save to " & file, MsgBoxStyle.Critical)
            Return False
        End If

        If AutoSendTo Then
            If VectoJobForm.Visible Then
                If UCase(FileRepl(VectoJobForm.tbHybridStrategyParams.Text, JobDir)) <> UCase(file) Then _
                    VectoJobForm.tbHybridStrategyParams.Text = GetFilenameWithoutDirectory(file, JobDir)
                VectoJobForm.UpdatePic()
            End If
        End If

        HCUFileBrowser.UpdateHistory(file)
        Text = GetFilenameWithoutPath(file, True)
        LbStatus.Text = ""

        _changed = False

        Return True
    End Function

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

    Private Sub ButOK_Click(sender As Object, e As EventArgs) Handles ButOK.Click
        If SaveOrSaveAs(False) Then Close()
    End Sub

    Private Sub ButCancel_Click(sender As Object, e As EventArgs) Handles ButCancel.Click
        Close()
    End Sub

    Private Sub tbMinSoC_TextChanged(sender As Object, e As EventArgs) Handles tbEquivalenceFactorDischarge.TextChanged,
                                                                               tbEquivalenceFactorCharge.TextChanged,
                                                                               tbMinSoC.TextChanged,
                                                                               tbMaxSoC.TextChanged,
                                                                               tbTargetSoC.TextChanged,
                                                                               tbMinICEOnTime.TextChanged,
                                                                               tbauxBufferTime.TextChanged,
                                                                               tbAuxBufferChargeTime.TextChanged,
                                                                               tbICEStartPenaltyFactor.TextChanged,
                                                                               tbCostFactorSoCExponent.TextChanged,
                                                                               tbGensetMinOptPowerFactor.TextChanged

        LbStatus.Text = "Unsaved changes in current file"
        _changed = True
    End Sub
End Class
