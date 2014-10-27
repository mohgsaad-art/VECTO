Namespace Electrics

Public Interface IResultCard

 ReadOnly Property Results As List(Of SmartResult)
Function GetSmartCurrentResult(Amps As Single) As Single

End Interface


End Namespace


