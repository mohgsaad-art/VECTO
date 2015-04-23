Imports System.Windows.Forms


Namespace Electrics


Public Class ImageColumn
  Inherits DataGridViewImageColumn



   Public  Sub new()

     MyBase.New()
     Me.CellTemplate = New ImageCell()

   End Sub




End Class


End Namespace



