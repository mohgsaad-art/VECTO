Imports System.Drawing
Imports System.Windows.Forms

Public Class frmCombinedAlternators


Protected gbColor As System.Drawing.Color = Color.LightGreen


Private Sub groupBoxUserInput_Paint(sender As Object, e As Windows.Forms.PaintEventArgs) Handles grpTable2000PRM.Paint, grpTable6000PRM.Paint, grpTable4000PRM.Paint


            Dim  p as Pen = nothing

            Dim sdr As Control = DirectCast(sender, Control)

            Select Case sdr.Name

              Case "grpTable2000PRM"
                p= new Pen(Color.LightGreen, 3)
              Case "grpTable4000PRM"
                p= new Pen(Color.Yellow, 3)
              Case "grpTable6000PRM"
                p= new Pen(Color.LightPink, 3)

                Case Else
                p= new Pen(Color.Black, 3) 

            End Select



            Dim gfx As Graphics = e.Graphics  

           

            gfx.DrawLine(p, 0, 5, 0, e.ClipRectangle.Height - 2)
            gfx.DrawLine(p, 0, 5, 10, 5)  
            gfx.DrawLine(p, 85, 5, e.ClipRectangle.Width - 2, 5)
            gfx.DrawLine(p, e.ClipRectangle.Width - 2, 5, e.ClipRectangle.Width - 2, e.ClipRectangle.Height - 2)
            gfx.DrawLine(p, e.ClipRectangle.Width - 2, e.ClipRectangle.Height - 2, 0, e.ClipRectangle.Height - 2) 

End Sub



End Class