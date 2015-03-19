
module Main


Sub main()

   Dim frm As New frmHVACTool("BusDatabase.abdb", "ssm.ahsm")
   Dim frmAlt As New frmCombinedAlternators()



   'frm.ShowDialog()
   frmAlt.ShowDialog()

   frm.Dispose

End Sub


End Module

