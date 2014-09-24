Public Class LaunchPad


    Private Sub btnHVAC_Click(sender As Object, e As EventArgs) Handles btnHVAC.Click

        Dim frm As New VectoAuxiliaries.UI.F_HVAC()

        frm.Show()
    End Sub

    Private Sub LaunchPad_Load(sender As Object, e As EventArgs)

    End Sub

    Private Sub LaunchPad_Activated(sender As Object, e As EventArgs)

    End Sub

    Private Sub btnCompressor_Click(sender As Object, e As EventArgs) Handles btnCompressor.Click

        Dim frm As New VectoAuxiliaries.UI.F_Compressor()

        frm.Show()

    End Sub
End Class