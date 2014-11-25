
Imports System.Reflection


Public Class Form1




Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load

 Dim obj As System.Runtime.Remoting.ObjectHandle

 Dim unwrapped As IYa

Try
  obj   =  Activator.CreateInstance([Assembly].GetExecutingAssembly.FullName,"CanISeeYou.MC")


  unwrapped =  DirectCast(obj.Unwrap , IYa)

  Catch ex As Exception

  Dim ebb As Single = 0



  Finally


  End Try




End Sub



End Class

Public Class MC 
 Implements IYa

    Public Property Age As integer
    Public Property Name As String Implements IYa.Name

End Class

Public Interface IYa

  property Name as string

  Property Signals As ISignals





End Interface






