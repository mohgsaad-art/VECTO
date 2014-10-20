imports VectoAuxiliaries.Electrics
Public Class ConsumerList



Friend ConsumerList As List(Of IElectricalConsumer)




Private Sub ConsumerList_Load( sender As Object,  e As EventArgs) Handles MyBase.Load


ConsumerList = New List(Of IElectricalConsumer)





End Sub

Private sub AddConsumer( consumer as IElectricalConsumer)

If Not ConsumerList.Contains( consumer ) then
 

ConsumerList.Add(consumer)

Else 

MessageBox.Show("Already Exists")


End If



End Sub


Private Sub Button1_Click( sender As Object,  e As EventArgs) Handles Button1.Click

        AddConsumer( New ElectricalConsumer(False,"Test","Test1",2.2,.5,26.3,1))

End Sub
End Class