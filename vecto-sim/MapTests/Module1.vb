

Imports System.IO
Imports System.Text

Module Module1

    Sub Main()


       'CreateBigFile()




        'Dim NonInterpolated As New AlternatorMapNonInterpolated("nonInterpolated.csv")

        'Dim nonIntReadStart = New TimeSpan(DateTime.Now.Ticks)
        'NonInterpolated.Initialise()
        'Dim nonIntReadStop = New TimeSpan(DateTime.Now.Ticks)

        'Console.WriteLine("NONE INTERP 56*200")
        'Console.WriteLine("___________________")
        'Console.WriteLine("Started Initialise {0}", nonIntReadStart.ToString("fff"))
        'Console.WriteLine("Stopped Initialise {0}", nonIntReadStop.ToString("fff"))
        'Console.WriteLine("Delta ( ms ) {0}", (nonIntReadStop - nonIntReadStart).ToString("fff"))


        Dim map As AlternatorMapInterpolated = New AlternatorMapInterpolated("interpolated.csv")

        Dim efficiency As AlternatorMapInterpolated.AlternatorMapValues

        map.Initialise()

         Dim nonIntReadStart = New TimeSpan(DateTime.Now.Ticks)

        For rpm As Integer = 2100 To 3099 Step 1

              efficiency = map.GetValueOrInterpolate(New AlternatorMapInterpolated.AlternatorMapKey(15, rpm))
        Next

        Dim nonIntReadStop = New TimeSpan(DateTime.Now.Ticks)

        Console.WriteLine("Interpolated Values - 1000 Interpolations between 2100 and 3099")
        Console.WriteLine("___________________")
        Console.WriteLine("Started  {0}", nonIntReadStart.ToString("fff"))
        Console.WriteLine("Stopped  {0}", nonIntReadStop.ToString("fff"))
        Console.WriteLine("Delta ( ms ) {0}", (nonIntReadStop - nonIntReadStart).ToString("fff"))

        Console.ReadLine()

    End Sub





     'Function getNearestValues(ByVal rpm As Integer, ByVal amps As Integer, ByRef map As AlternatorMapNonInterpolated) As Single

     '   Dim efficiency As Single

     '     If (rpm Mod 100 = 0 AndAlso Math.Floor(amps) = amps) Then

     '        efficiency = (From values In map.map Where values.RPM = rpm And values.Efficiency = amps).First.Amps

     '        Return efficiency / 100

     '     End If


     '   Dim positionRpm As Integer = rpm Mod 100

     '   If positionRpm <= 50 Then
     '       positionRpm = rpm - positionRpm
     '       Else
     '       positionRpm = rpm - positionRpm + 100
     '   End If

     '   Dim positionAmps As Single

     '   If amps - Math.Floor(amps) <= 0.5 Then

     '   positionAmps = Math.Floor(amps)

     '   Else
     '      positionAmps = Math.Ceiling(amps)
     '   End If


     '    efficiency = (From values In map.map Where values.RPM = positionRpm And values.Efficiency = positionAmps).First.Amps

     '   Return efficiency / 100

     'End Function




End Module
