Imports System.IO

Namespace Hvac

    Public Class EnvironmentalConditionsMap
        Implements IEnvironmentalConditionsMap

        Private filePath As String
        Private vectoDirectory As String

        Private _map As New List(Of IEnvironmentalCondition)

        Public Sub New(filepath As String, vectoDirectory As String)

            Me.filePath = filepath
            Me.vectoDirectory = vectoDirectory

            Initialise()

        End Sub

        Public Function Initialise() As Boolean Implements IEnvironmentalConditionsMap.Initialise

            If (Not String.IsNullOrWhiteSpace(filePath)) Then

                filePath = FilePathUtils.ResolveFilePath(vectoDirectory, filePath)

                If File.Exists(filePath) Then
                    Using sr As StreamReader = New StreamReader(filePath)

                        'get array og lines fron csv
                        Dim lines() As String = sr.ReadToEnd().Split(CType(Environment.NewLine, Char()), StringSplitOptions.RemoveEmptyEntries)

                        'Must have at least 1 entries to make it usable [dont forget the header row]
                        If (lines.Count() < 2) Then
                            Return False
                        End If

                        Dim firstline As Boolean = True

                        For Each line As String In lines
                            If Not firstline Then

                                'split the line
                                Dim elements() As String = line.Split(New Char() {","}, StringSplitOptions.RemoveEmptyEntries)

                                '3 entries per line required
                                If (elements.Length <> 4) Then
                                    Return False
                                End If

                                'Add environment condition
                                Dim newCondition As EnvironmentalCondition = New EnvironmentalCondition(elements(1), elements(2), elements(3))

                                _map.Add(newCondition)

                            Else
                                firstline = False
                            End If
                        Next line
                    End Using

                Else
                    Return False
                End If
            End If

            Return True

        End Function

        Public Function GetEnvironmentalConditions() As List(Of IEnvironmentalCondition) Implements IEnvironmentalConditionsMap.GetEnvironmentalConditions

            Return _map

        End Function

    End Class

End Namespace
