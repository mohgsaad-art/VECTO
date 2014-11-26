
Imports System.IO

Namespace Pneumatics

Public Class PneumaticActuationsMAP
Implements IPneumaticActuationsMAP


Private map As Dictionary(Of ActuationsKey, Integer)
Private filePath As String


Public Function GetNumActuations(key As ActuationsKey) As Integer Implements IPneumaticActuationsMAP.GetNumActuations

   If map Is Nothing OrElse Not map.ContainsKey(key) Then
    Throw New ArgumentException(String.Format("Pneumatic Actuations map does not contain the key '{0}'.", key.CycleName & ":" & key.ConsumerName))
   End If

   Return map(key)


End Function


Public Sub New(filePath As String)

   Me.filePath = filePath

   If filePath.Trim.Length=0 then Throw New ArgumentException("A filename for the Pneumatic Actuations Map has not been supplied")

   Initialise()



End Sub

 Public Function Initialise() As Boolean Implements IPneumaticActuationsMAP.Initialise

            Dim newKey As ActuationsKey
            Dim numActuations As Single

            If File.Exists(filePath) Then
                Using sr As StreamReader = New StreamReader(filePath)
                    'get array of lines from csv
                    Dim lines() As String = sr.ReadToEnd().Split(CType(Environment.NewLine, Char()), StringSplitOptions.RemoveEmptyEntries)

                    'Must have at least 2 entries in map to make it usable [dont forget the header row]
                    If lines.Length < 3 Then Throw New ArgumentException("Pneumatic Actuations Map does not have sufficient rows in file to build a usable map")

                    map = New Dictionary(Of ActuationsKey, Integer)()
                    Dim firstline As Boolean = True

                    For Each line As String In lines
                        If Not firstline Then
                            'split the line
                            Dim elements() As String = line.Split(New Char() {","}, StringSplitOptions.RemoveEmptyEntries)
                            '3 entries per line required
                            If (elements.Length <> 3) Then Throw New ArgumentException("Pneumatic Actuations Map has Incorrect number of values in file")

                            'add values to map



                            If Not Integer.TryParse(elements(2), numActuations) Then
                            Throw New ArgumentException("Pneumatic Actuations Map Contains Non Integer values in actuations column")
                            End If

                            'Should throw exception if ConsumerName or CycleName are empty.
                            newKey = New ActuationsKey(elements(0).ToString(), elements(1).ToString())

                            map.Add(newKey, CType(elements(2), Single))

                        Else
                            firstline = False
                        End If
                    Next
                End Using


            Else
                Throw New ArgumentException(String.Format(" Pneumatic Acutations map '{0}' supplied  does not exist", filePath))
                Return False
            End If

            'If we get here then all should be well and we can return a True value of success.
            Return True


        End Function

End Class


End Namespace


