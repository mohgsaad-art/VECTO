
Imports VectoAuxiliaries.Electrics

module Main





Sub main()



    Dim altSignals As New CombinedAlternatorSignals()

    Dim frm As New frmHVACTool("BusDatabase.abdb", "ssm.ahsm")
   ' Dim frm As New frmCombinedAlternators("ABCDEFG.aalt",altSignals)
    frm.ShowDialog()


    

End Sub


End Module

