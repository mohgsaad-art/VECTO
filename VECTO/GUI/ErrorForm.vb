Imports TUGraz.VectoCommon.Exceptions
Imports TUGraz.VectoCommon.Models

Public Class ErrorForm

    Private _exception As Exception

    Private Sub ErrorForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load

    End Sub

    Public Sub New(Ex As Exception)
        InitializeComponent()
        _exception = Ex

        _cycleIdLbl.Text = $"{Ex.Data(RunInfo.RunIdentifier)} {Ex.Data(RunInfo.RunName)} {Ex.Data(RunInfo.CycleName)} {Ex.Data(RunInfo.RunSuffix)}"
        _codeLbl.Text = Ex.GetErrorCode().ToString()
        _messageLbl.Text = Ex.Message
        _fixLbl.Text = Ex.GetFix()
        _absTimeLbl.Text = $"AbsTime: {Ex.Data(RunInfo.AbsTime)}"
        _progressLbl.Text = $"Progress: {Ex.Data(RunInfo.Progress):P1}"
        _distanceLbl.Text = $"Distance: {Ex.Data(RunInfo.Distance)}"
        _dtLbl.Text = $"dt: {Ex.Data(RunInfo.Dt)}"
        _speedLbl.Text = $"Speed: {Ex.Data(RunInfo.Speed)}"
        _fEquivLbl.Text = $"f_equiv: {Ex.Data(RunInfo.F_equiv)}"
        _stackLbl.Text = Ex.StackTrace

        Dim gearInfo = TryCast(Ex.Data(RunInfo.GearInfo), Dictionary(Of AxleNumber, GearshiftPosition))?.Select(Function(kvp) $"Gear{kvp.Key.FormatAxleNumber()}: {kvp.Value}")
        gearInfo = If(gearInfo IsNot Nothing, gearInfo, New List(Of String) From {"Gear: "})
        _gearLbl.Text = String.Join(", ", gearInfo)

    End Sub

    Private Sub _okBtn_Click(sender As Object, e As EventArgs) Handles _okBtn.Click
        Me.DialogResult = DialogResult.OK
        Me.Close()
    End Sub

    Private Sub _clipboardBtn_Click(sender As Object, e As EventArgs) Handles _clipboardBtn.Click
        Clipboard.SetText(GetClipboardMessage(_exception))
    End Sub

    Private Shared Function GetClipboardMessage(Ex As Exception) As String
        Dim gearInfo = TryCast(Ex.Data(RunInfo.GearInfo), Dictionary(Of AxleNumber, GearshiftPosition))?.Select(Function(kvp) $"Gear{kvp.Key.FormatAxleNumber()}: {kvp.Value}")
        gearInfo = If(gearInfo IsNot Nothing, gearInfo, New List(Of String) From {"Gear: "})

        Return $"CycleRun: {Ex.Data(RunInfo.RunName)} {Ex.Data(RunInfo.CycleName)} {Ex.Data(RunInfo.RunSuffix)}" +
            $"{Environment.NewLine}Code: {Ex.Data(RunInfo.ErrorCode)}{Environment.NewLine}Error: {Ex.Message}{Environment.NewLine}{Environment.NewLine}Fix: {Ex.Data(RunInfo.Fix)}" +
            $"{Environment.NewLine}{Environment.NewLine}Progress: {Ex.Data(RunInfo.Progress):P1}{Environment.NewLine}AbsTime: {Ex.Data(RunInfo.AbsTime)}{Environment.NewLine}Distance: {Ex.Data(RunInfo.Distance)}" +
            $"{Environment.NewLine}dt: {Ex.Data(RunInfo.Dt)}{Environment.NewLine}Speed: {Ex.Data(RunInfo.Speed)}{Environment.NewLine}f_equiv: {Ex.Data(RunInfo.F_equiv)}" +
            $"{Environment.NewLine}{String.Join(", ", gearInfo)}" +
            $"{Environment.NewLine}{Environment.NewLine}StackTrace:{Environment.NewLine}{Ex.StackTrace}"

    End Function
End Class