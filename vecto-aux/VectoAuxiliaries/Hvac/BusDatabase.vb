Imports System.IO

Namespace Hvac


Public Class BusDatabase
  Implements IBusDatabase

  Private buses As New Dictionary(Of String, IBus)
  Private selectListBuses As New List(Of IBus)


        Public Function GetBuses(busModel As String, optional AsSelectList As Boolean=false) As List(Of IBus) Implements IBusDatabase.GetBuses

           If AsSelectList then
            selectListBuses = New List(Of IBus)
            selectListBuses = buses.Select( Function(x) x.Value).Where( Function(v) v.Model="" OrElse v.Model.ToLower.Contains( busModel.ToLower)).ToList()
            selectListBuses.Insert(0, New Bus("<Select>","low floor","gas",1,1,1,2))
            Return selectListBuses

           Else
           
            Return buses.Select( Function(x) x.Value).Where( Function(v) v.Model="" OrElse v.Model.ToLower.Contains( busModel.ToLower)).ToList()

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

                    For Each line As String In lines
                        If Not firstline Then

                            'split the line
                            Dim elements() As String = line.Split(New Char() {","}, StringSplitOptions.RemoveEmptyEntries)
                            '3 entries per line required
                            If (elements.Length <> 7) Then
                                Throw New ArgumentException("Incorrect number of values in csv file")
                            End If
                            'add values to map

                            'Bus
                            Try
                                Dim bus As New Bus(elements(0), _
                                                   elements(1), _
                                                   elements(2), _
                                                   elements(3), _
                                                   elements(4), _
                                                   elements(5), _
                                                   elements(6))

                                buses.Add(bus.Model,bus)

                            Catch ex As Exception
                               
                               'Indicate problems
                               returnStatus=false

                            End Try

                                            
                                             


                        Else
                            firstline = False
                        End If
                    Next line
                End Using
           
            Else
               returnStatus=False
            End If

           Return returnStatus

        End Function


End Class



End Namespace



