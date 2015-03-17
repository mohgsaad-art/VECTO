Imports System.Windows.Forms


Public Class DeleteColumn
  Inherits DataGridViewButtonColumn


  Sub new()

   Me.CellTemplate = New DeleteCell()

  End Sub


End Class

