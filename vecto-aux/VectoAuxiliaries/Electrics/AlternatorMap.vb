
Imports System.Text
Imports System.IO

Namespace Electrics

Public Class AlternatorMap
Implements IAlternatorMap


    Private ReadOnly filePath As String

    Public map As New List(Of MapPoint)
    Public yRange As List(Of Single)
    Public xRange As List(Of Single)
    Private minX, minY, maxX, maxY As Single

    'Required Action Test or Interpolation Type
    Public Function OnBoundaryYInterpolatedX(x As Single, y As Single) As Boolean
        Return yRange.Contains(y) AndAlso Not xRange.Contains(x)
    End Function
    Public Function OnBoundaryXInterpolatedY(x As Single, y As Single) As Boolean
        Return Not yRange.Contains(y) AndAlso xRange.Contains(x)
    End Function
    Public Function ONBoundaryXY(x As Single, y As Single) As Boolean
        Return (From sector In map Where sector.Y = y AndAlso sector.x = x).Count = 1
    End Function

    'Determine Value Methods
    Private Function GetOnBoundaryXY(x As Single, y As Single) As Single
        Return (From sector In map Where sector.Y = y AndAlso sector.x = x).First().v
    End Function
    Private Function GetOnBoundaryYInterpolatedX(x As Single, y As Single) As Single

        Dim x0, x1, v0, v1, slope, dx As Single

        x0 = (From p In xRange Order By p Where p < x).Last()
        x1 = (From p In xRange Order By p Where p > x).First()
        dx = x1 - x0

        v0 = GetOnBoundaryXY(x0, y)
        v1 = GetOnBoundaryXY(x1, y)

       slope = (v1 - v0) / (x1 - x0)

        Return v0 + ((x - x0) * slope)

    End Function
    Private Function GetOnBoundaryXInterpolatedY(x As Single, y As Single) As Single

        Dim y0, y1, v0, v1, dy, v, slope As Single

        y0 = (From p In yRange Order By p Where p < y).Last()
        y1 = (From p In yRange Order By p Where p > y).First()
        dy = y1 - y0

        v0 = GetOnBoundaryXY(x, y0)
        v1 = GetOnBoundaryXY(x, y1)

        slope = (v1 - v0) / (y1 - y0)

        v = v0 + ((y - y0) * slope)

        Return v

    End Function
    Private Function GetBiLinearInterpolatedValue(x As Single, y As Single) As Single

        Dim q11, q12, q21, q22, x1, x2, y1, y2, r1, r2, p As Single

        y1 = (From mapSector As MapPoint In map Where mapSector.Y < y).Last().Y
        y2 = (From mapSector As MapPoint In map Where mapSector.Y > y).First().Y

        x1 = (From mapSector As MapPoint In map Where mapSector.x < x).Last().x
        x2 = (From mapSector As MapPoint In map Where mapSector.x > x).First().x

        q11 = GetOnBoundaryXY(x1, y1)
        q12 = GetOnBoundaryXY(x1, y2)

        q21 = GetOnBoundaryXY(x2, y1)
        q22 = GetOnBoundaryXY(x2, y2)

        r1 = ((x2 - x) / (x2 - x1)) * q11 + ((x - x1) / (x2 - x1)) * q21

        r2 = ((x2 - x) / (x2 - x1)) * q12 + ((x - x1) / (x2 - x1)) * q22


        p = ((y2 - y) / (y2 - y1)) * r1 + ((y - y1) / (y2 - y1)) * r2


        Return p

    End Function

    'Utilities
    Private Sub fillMapWithDefaults()



        map.Add(New MapPoint(10, 1500, 0.615))
        map.Add(New MapPoint(27, 1500, 0.7))
        map.Add(New MapPoint(53, 1500, 0.1947))
        map.Add(New MapPoint(63, 1500, 0.0))
        map.Add(New MapPoint(68, 1500, 0.0))
        map.Add(New MapPoint(125, 1500, 0.0))
        map.Add(New MapPoint(136, 1500, 0.0))
        map.Add(New MapPoint(10, 2000, 0.62))
        map.Add(New MapPoint(27, 2000, 0.7))
        map.Add(New MapPoint(53, 2000, 0.3))
        map.Add(New MapPoint(63, 2000, 0.1462))
        map.Add(New MapPoint(68, 2000, 0.692))
        map.Add(New MapPoint(125, 2000, 0.0))
        map.Add(New MapPoint(136, 2000, 0.0))
        map.Add(New MapPoint(10, 4000, 0.64))
        map.Add(New MapPoint(27, 4000, 0.6721))
        map.Add(New MapPoint(53, 4000, 0.7211))
        map.Add(New MapPoint(63, 4000, 0.74))
        map.Add(New MapPoint(68, 4000, 0.7352))
        map.Add(New MapPoint(125, 4000, 0.68))
        map.Add(New MapPoint(136, 4000, 0.6694))
        map.Add(New MapPoint(10, 6000, 0.53))
        map.Add(New MapPoint(27, 6000, 0.5798))
        map.Add(New MapPoint(53, 6000, 0.656))
        map.Add(New MapPoint(63, 6000, 0.6853))
        map.Add(New MapPoint(68, 6000, 0.7))
        map.Add(New MapPoint(125, 6000, 0.6329))
        map.Add(New MapPoint(136, 6000, 0.62))
        map.Add(New MapPoint(10, 7000, 0.475))
        map.Add(New MapPoint(27, 7000, 0.5337))
        map.Add(New MapPoint(53, 7000, 0.6235))
        map.Add(New MapPoint(63, 7000, 0.658))
        map.Add(New MapPoint(68, 7000, 0.6824))
        map.Add(New MapPoint(125, 7000, 0.6094))
        map.Add(New MapPoint(136, 7000, 0.5953))




    End Sub
    Private Sub getMapRanges()

        yRange = (From coords As MapPoint In map Order By coords.Y Select coords.Y Distinct).ToList()
        xRange = (From coords As MapPoint In map Order By coords.x Select coords.x Distinct).ToList()

        minX = xRange.First
        maxX = xRange.Last
        minY = yRange.First
        maxY = yRange.Last


    End Sub

    'Single entry point to determine Value on map
    Public Function GetValue(x As Single, y As Single) As Single


         If x < minX  OrElse  x > maxX OrElse  y < minY   OrElse  y > maxY  then

           'OnAuxiliaryEvent(String.Format("Alternator Map Limiting : RPM{0}, AMPS{1}",x,y),AdvancedAuxiliaryMessageType.Warning)


            'Limiting
            If x < minX Then x = minX
            If x > maxX Then x = maxX
            If y < minY Then y = minY
            If y > maxY Then y = maxY

         End If


        'Satisfies both data points - non interpolated value
        If ONBoundaryXY(x, y) Then Return GetOnBoundaryXY(x, y)

        'Satisfies only x or y - single interpolation value
        If OnBoundaryXInterpolatedY(x, y) Then Return GetOnBoundaryXInterpolatedY(x, y)
        If OnBoundaryYInterpolatedX(x, y) Then Return GetOnBoundaryYInterpolatedX(x, y)

        'satisfies no data points - Bi-Linear interpolation
        Return GetBiLinearInterpolatedValue(x, y)


    End Function
    Public Function ReturnDefaultMapValueTests() As String

        Dim sb = New StringBuilder()
        Dim x, y As Single

        'All Sector Values
        sb.AppendLine("All Values From Map")
        sb.AppendLine("-------------------")
        For Each x In xRange

            For Each y In yRange
                sb.AppendLine(String.Format("X:{0}, Y:{1}, V:{2}", x, y, GetValue(x, y)))
            Next

        Next

        sb.AppendLine("")
        sb.AppendLine("Four Corners with interpolated other")
        sb.AppendLine("-------------------")
        x = 1500 : y = 18.5
        sb.AppendLine(String.Format("X:{0}, Y:{1}, V:{2}", x, y, GetValue(x, y)))
        x = 7000 : y = 96.5
        sb.AppendLine(String.Format("X:{0}, Y:{1}, V:{2}", x, y, GetValue(x, y)))
        x = 1750 : y = 10
        sb.AppendLine(String.Format("X:{0}, Y:{1}, V:{2}", x, y, GetValue(x, y)))
        x = 6500 : y = 10
        sb.AppendLine(String.Format("X:{0}, Y:{1}, V:{2}", x, y, GetValue(x, y)))

        sb.AppendLine("")
        sb.AppendLine("Interpolated both")
        sb.AppendLine("-------------------")

        Dim mx, my As Single
        For x = 0 To xRange.Count - 2

            For y = 0 To yRange.Count - 2

                mx = xRange(x) + (xRange(x + 1) - xRange(x)) / 2
                my = yRange(y) + (yRange(y + 1) - yRange(y)) / 2

                sb.AppendLine(String.Format("X:{0}, Y:{1}, V:{2}", mx, my, GetValue(mx, my)))


            Next

        Next

        sb.AppendLine("")
        sb.AppendLine("MIKE -> 40 & 1000")
        sb.AppendLine("-------------------")
        x = 1000 : y = 40
        sb.AppendLine(String.Format("X:{0}, Y:{1}, V:{2}", x, y, GetValue(x, y)))




        Return sb.ToString()

    End Function

    'Constructors
    Public Sub New(filepath As String)

        Me.filePath = filepath

        Initialise()

        getMapRanges()

    End Sub
    Public Sub New(values As List(Of MapPoint))

        map = values
        getMapRanges()

    End Sub

    Public Class MapPoint

        Public Y As Single
        Public x As Single
        Public v As Single

        Public Sub New(y As Single, x As Single, v As Single)

            Me.Y = y
            Me.x = x
            Me.v = v

        End Sub

    End Class


        Public Function GetEfficiency(rpm As single, amps As single) As AlternatorMapValues Implements IAlternatorMap.GetEfficiency

           Return New AlternatorMapValues( GetValue(rpm,amps))

        End Function

        Public Function Initialise() As Boolean Implements IAlternatorMap.Initialise
                    If File.Exists(filePath) Then
                Using sr As StreamReader = New StreamReader(filePath)
                    'get array og lines fron csv
                    Dim lines() As String = sr.ReadToEnd().Split(CType(Environment.NewLine, Char()), StringSplitOptions.RemoveEmptyEntries)

                    'Must have at least 2 entries in map to make it usable [dont forget the header row]
                    If (lines.Count() < 3) Then
                        Throw New ArgumentException("Insufficient rows in csv to build a usable map")
                    End If

                    map = New List(Of MapPoint)
                    Dim firstline As Boolean = True

                    For Each line As String In lines
                        If Not firstline Then

                        'Advanced Alternator Source Check.
                        If line.contains("[MODELSOURCE") then Exit For

                            'split the line
                            Dim elements() As String = line.Split(New Char() {","}, StringSplitOptions.RemoveEmptyEntries)
                            '3 entries per line required
                            If (elements.Length <> 3) Then
                                Throw New ArgumentException("Incorrect number of values in csv file")
                            End If
                            'add values to map

                            'Create AlternatorKey
                            Dim newPoint as MapPoint = New  MapPoint(elements(0),elements(1),elements(2))

                            map.Add(newPoint)

                        Else
                            firstline = False
                        End If
                    Next line
                End Using
                Return True
            Else
                Throw New ArgumentException("Supplied input file does not exist")
            End If
        End Function


        Public Event AuxiliaryEvent(ByRef sender As Object, message As String, messageType As AdvancedAuxiliaryMessageType) Implements IAuxiliaryEvent.AuxiliaryEvent

        Protected sub OnAuxiliaryEvent(message As String, messageType As AdvancedAuxiliaryMessageType)

          RaiseEvent  AuxiliaryEvent(Me, message, messageType)

        End Sub

End Class


End Namespace






