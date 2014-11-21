
'POCO Class to hold information required to
'1. Reference Value
'2. Create UI Controls on Form.

Namespace Hvac

    Public Class HVACMapParameter

        Public Key As String
        Public Name As String
        Public Description As String
        Public Notes As String
        Public Min As Double
        Public Max As Double
        Public SystemType As System.Type
        Public SearchControlType As System.Type
        Public ValueType As System.Type
        Public OrdinalPosition As Integer
        Public UniqueDataValues As List(Of String)
        Public IsOutput As Boolean = False

    End Class



End Namespace