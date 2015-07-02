Imports System.IO

Namespace Hvac

    Public Class EnvironmentalConditionsMap
        Implements IEnvironmentalConditionsMap

        Private ReadOnly filePath As String

        Private _map As New List(Of IEnvironmentalCondition)

        Public Sub New(filepath As String)

            Me.filePath = filepath

            Initialise()

        End Sub

        Public Sub Initialise() Implements IEnvironmentalConditionsMap.Initialise

            If (Not String.IsNullOrWhiteSpace(filePath)) Then
                If File.Exists(filePath) Then
                    Using sr As StreamReader = New StreamReader(filePath)

                        'get array og lines fron csv
                        Dim lines() As String = sr.ReadToEnd().Split(CType(Environment.NewLine, Char()), StringSplitOptions.RemoveEmptyEntries)

                        'Must have at least 1 entries to make it usable [dont forget the header row]
                        If (lines.Count() < 2) Then
                            Throw New ArgumentException("Insufficient rows to build conditions")
                        End If

                        Dim firstline As Boolean = True

                        For Each line As String In lines
                            If Not firstline Then

                                'split the line
                                Dim elements() As String = line.Split(New Char() {","}, StringSplitOptions.RemoveEmptyEntries)

                                '3 entries per line required
                                If (elements.Length <> 4) Then
                                    Throw New ArgumentException("Incorrect number of values in file")
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
                    Throw New ArgumentException("File not found")
                End If
            End If

        End Sub

        Public Function GetEnvironmentalConditions() As List(Of IEnvironmentalCondition) Implements IEnvironmentalConditionsMap.GetEnvironmentalConditions

            Return _map

        End Function

    End Class

End Namespace
