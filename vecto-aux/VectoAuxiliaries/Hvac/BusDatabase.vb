Imports System.IO
Imports System.Text

Namespace Hvac


    Public Class BusDatabase
        Implements IBusDatabase

        Private buses As New List(Of IBus)
        Private selectListBuses As New List(Of IBus)

        Public Function AddBus(bus As IBus) As Boolean Implements IBusDatabase.AddBus

            Dim result As Boolean = True

            Try
                buses.Add(bus)
            Catch ex As Exception
                result = False
            End Try

            Return result
        End Function

        Public Function GetBuses(busModel As String, Optional AsSelectList As Boolean = False) As List(Of IBus) Implements IBusDatabase.GetBuses

            If AsSelectList Then
                selectListBuses = New List(Of IBus)
                selectListBuses = buses.Where(Function(v) v.Model = "" OrElse v.Model.ToLower.Contains(busModel.ToLower)).ToList()
                selectListBuses.Insert(0, New Bus(0, "<Select>", "low floor", "gas", 1, 1, 1, 2, False))
                Return selectListBuses

            Else

                Return buses.Where(Function(v) v.Model = "" OrElse v.Model.ToLower.Contains(busModel.ToLower)).ToList()

            End If

        End Function

        Public Function Initialise(filepath As String) As Boolean Implements IBusDatabase.Initialise

            Dim returnStatus As Boolean = True

            If File.Exists(filepath) Then
                Using sr As StreamReader = New StreamReader(filepath)
                    'get array og lines fron csv
                    Dim lines() As String = sr.ReadToEnd().Split(CType(Environment.NewLine, Char()), StringSplitOptions.RemoveEmptyEntries)

                    'Must have at least 2 entries in map to make it usable [dont forget the header row]
                    If (lines.Count() < 2) Then
                        Return False
                    End If

                    Dim firstline As Boolean = True

                    Dim id As Integer = 1

                    For Each line As String In lines
                        If Not firstline Then

                            'split the line
                            Dim elements() As String = line.Split(New Char() {","}, StringSplitOptions.RemoveEmptyEntries)
                            '7 or 8 entries per line required
                            If (elements.Length <> 7 AndAlso elements.Length <> 8) Then
                                Throw New ArgumentException("Incorrect number of values in csv file")
                            End If

                            'Bus
                            Try
                                Dim bus As New Bus(id, _
                                                   elements(0), _
                                                   elements(1), _
                                                   elements(2), _
                                                   elements(3), _
                                                   elements(4), _
                                                   elements(5), _
                                                   elements(6), _
                                                   If(elements.Length = 8, Boolean.Parse(elements(7)), False))

                                buses.Add(bus)

                            Catch ex As Exception

                                'Indicate problems
                                returnStatus = False

                            End Try

                            id = id + 1
                        Else
                            firstline = False
                        End If
                    Next line
                End Using

            Else
                returnStatus = False
            End If

            Return returnStatus

        End Function

        Public Function UpdateBus(id As Integer, bus As IBus) As Boolean Implements IBusDatabase.UpdateBus

            Dim result As Boolean = True

            Try

                Dim existingBus As IBus = buses.Single(Function(b) b.Id = id)

                existingBus.Model = bus.Model
                existingBus.RegisteredPassengers = bus.RegisteredPassengers
                existingBus.FloorType = bus.FloorType
                existingBus.LengthInMetres = bus.LengthInMetres
                existingBus.WidthInMetres = bus.WidthInMetres
                existingBus.HeightInMetres = bus.HeightInMetres
                existingBus.IsDoubleDecker = bus.IsDoubleDecker

            Catch ex As Exception
                result = False
            End Try

            Return result

        End Function

        Public Function Save(filepath As String) As Boolean Implements IBusDatabase.Save

            Dim result As Boolean = True
            Dim output As New StringBuilder

            Try
                output.AppendLine("Bus Model,Type,engine Type,length in m,wide in m,height in m,registered passengers,double decker")

                For Each bus As IBus In buses
                    output.AppendLine(bus.ToString())
                Next

                File.WriteAllText(filepath, output.ToString())
            Catch ex As Exception
                result = False
            End Try

            Return result

        End Function

    End Class



End Namespace



