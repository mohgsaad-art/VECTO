
module Main


Sub main()

   Dim frm As New frmHVACTool("BusDatabase.abdb", "ssm.ahsm")

   frm.ShowDialog()

   frm.Dispose

End Sub


End Module

