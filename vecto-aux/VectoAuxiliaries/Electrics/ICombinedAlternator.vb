
Namespace Electrics


Public Interface ICombinedAlternator

   'Alternators List
   Property Alternators As List(Of IAlternator)

   'Test Equality
   Function IsEqualTo( other As ICombinedAlternator ) As Boolean



End Interface




End Namespace


