
Namespace Electrics

    'Used By Combined Alternator.
    Public Interface IAlternator
            
     
         Property AlternatorName As String 

         Property PulleyRatio As single

         Readonly property SpindleSpeed As Double

         ReadOnly Property Efficiency  As Double

         Property InputTable2000  AS List(Of AltUserInput)
         Property InputTable4000  AS List(Of AltUserInput)
         Property InputTable6000  AS List(Of AltUserInput)
         Property RangeTable      AS List(Of Table4Row) 


         'Clone values
         Sub  Clone( other As IAlternator)

         'Test Equality
         Function IsEqualTo(other As IAlternator) As Boolean 


   end  interface


End Namespace


