Imports TUGraz.VectoCommon.Exceptions
Imports TUGraz.VectoCommon.Models

Public Class ErrorForm

    Private _exception As Exception

    Private Sub ErrorForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load

    End Sub

    Public Sub New(Ex As Exception)
        InitializeComponent()
        _exception = Ex

        Me.Text += $" - {Ex.Data(ExceptionData.RunName)}"
        _cycleIdLbl.Text = $"{Ex.Data(ExceptionData.CycleName)}    {Ex.Data(ExceptionData.RunSuffix)}"
        _codeLbl.Text = Ex.GetErrorCode().ToString()
        _messageLbl.Text = Ex.Message
        _fixLbl.Text = Ex.GetFix()
        _absTimeLbl.Text = $"{Ex.Data(ExceptionData.AbsTime)}"
        _progressLbl.Text = $"{Ex.Data(ExceptionData.Progress):P1}"
        _distanceLbl.Text = $"{Ex.Data(ExceptionData.Distance)}"
        _dtLbl.Text = $"{Ex.Data(ExceptionData.Dt)}"
        _speedLbl.Text = $"{Ex.Data(ExceptionData.Speed)}"
        _fEquivLbl.Text = $"{Ex.Data(ExceptionData.F_equiv)}"
        _stackLbl.Text = Ex.StackTrace

        Dim gearInfo = TryCast(Ex.Data(ExceptionData.GearInfo), Dictionary(Of AxleNumber, GearshiftPosition))?.Select(Function(kvp) $"Gear{kvp.Key.FormatAxleNumber()}: {kvp.Value}")
        gearInfo = If(gearInfo IsNot Nothing, gearInfo, New List(Of String))
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
        Dim gearInfo = TryCast(Ex.Data(ExceptionData.GearInfo), Dictionary(Of AxleNumber, GearshiftPosition))?.Select(Function(kvp) $"Gear{kvp.Key.FormatAxleNumber()}: {kvp.Value}")
        gearInfo = If(gearInfo IsNot Nothing, gearInfo, New List(Of String) From {"Gear: "})

        Return $"CycleRun: {Ex.Data(ExceptionData.RunName)} {Ex.Data(ExceptionData.CycleName)} {Ex.Data(ExceptionData.RunSuffix)}" +
            $"{Environment.NewLine}Code: {Ex.Data(ExceptionData.ErrorCode)}{Environment.NewLine}Error: {Ex.Message}{Environment.NewLine}{Environment.NewLine}Fix: {Ex.Data(ExceptionData.Fix)}" +
            $"{Environment.NewLine}{Environment.NewLine}Progress: {Ex.Data(ExceptionData.Progress):P1}{Environment.NewLine}AbsTime: {Ex.Data(ExceptionData.AbsTime)}{Environment.NewLine}Distance: {Ex.Data(ExceptionData.Distance)}" +
            $"{Environment.NewLine}dt: {Ex.Data(ExceptionData.Dt)}{Environment.NewLine}Speed: {Ex.Data(ExceptionData.Speed)}{Environment.NewLine}f_equiv: {Ex.Data(ExceptionData.F_equiv)}" +
            $"{Environment.NewLine}{String.Join(", ", gearInfo)}" +
            $"{Environment.NewLine}{Environment.NewLine}StackTrace:{Environment.NewLine}{Ex.StackTrace}"

    End Function
End Class