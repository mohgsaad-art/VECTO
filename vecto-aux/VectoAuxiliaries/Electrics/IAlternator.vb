
Namespace Electrics

    'Used By Combined Alternator.
    'Model based on CombinedALTS_V02_Editable.xlsx
    Public Interface IAlternator

        'D6
        Property AlternatorName As String

        'G6
        Property PulleyRatio As Single

        'S9
        ReadOnly Property SpindleSpeed As Double

        'S10
        ReadOnly Property Efficiency As Double

        'C10-D15
        Property InputTable2000 As List(Of AltUserInput)

        'F10-G15
        Property InputTable4000 As List(Of AltUserInput)

        'I10-J15
        Property InputTable6000 As List(Of AltUserInput)

        'M10-N15
        Property RangeTable As List(Of Table4Row)

        'Test Equality
        Function IsEqualTo(other As IAlternator) As Boolean


    End Interface


End Namespace


