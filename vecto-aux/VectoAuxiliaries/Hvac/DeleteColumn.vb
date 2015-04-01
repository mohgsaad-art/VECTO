Imports System.Windows.Forms


Public Class DeleteColumn
  Inherits DataGridViewButtonColumn



   Public  Sub new()

     MyBase.New()
     Me.CellTemplate = New DeleteCell()

   End Sub




End Class

Public Class DeleteAlternatorColumn
  Inherits DataGridViewButtonColumn



   Public  Sub new()

     MyBase.New()
     Me.CellTemplate = New DeleteAlternatorCell()

   End Sub




End Class