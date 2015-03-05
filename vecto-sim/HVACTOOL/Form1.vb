
Imports VectoAuxiliaries.Hvac
Imports System.ComponentModel




Public Class Form1



Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load

   Dim frm As New frmHVACTool("BusDatabase.csv", "ssmDelete.ahsm")

   frm.Show

End Sub




End Class
