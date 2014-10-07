Imports System.IO



    ''' <summary>
    ''' Alternator Efficiency Map
    ''' </summary>
    ''' <remarks></remarks>
    Public Class AlternatorMapInterpolated


        ''' <summary>
        ''' path to csv file containing map data
        ''' expects header row
        ''' Columns - [rpm - integer], [efficiency float, range 0-1], [max regen power float]
        ''' </summary>
        ''' <remarks></remarks>
        Private ReadOnly filePath As String

        Public map As List(Of AlternatorMapValues)

        ''' <summary>
        ''' Creates a new instance of AlternatorMap class
        ''' </summary>
        ''' <param name="filePath">full path to csv data</param>
        ''' <remarks></remarks>
        Public Sub New(ByVal filePath As String)
            Me.filePath = filePath
        End Sub

        ''' <summary>
        ''' Initialise the map from supplied csv data
        ''' </summary>
        ''' <returns>Boolean - true if map is created successfully</returns>
        ''' <remarks></remarks>
        Public Function Initialise() As Boolean
            If File.Exists(filePath) Then
                Using sr As StreamReader = New StreamReader(filePath)
                    'get array og lines fron csv
                    Dim lines() As String = sr.ReadToEnd().Split(CType(Environment.NewLine, Char()), StringSplitOptions.RemoveEmptyEntries)

                    'Must have at least 2 entries in map to make it usable [dont forget the header row]
                    If (lines.Count() < 3) Then
                        Throw New ArgumentException("Insufficient rows in csv to build a usable map")
                    End If

                    map = New List(Of AlternatorMapValues)()
                    Dim firstline As Boolean = True

                    For Each line As String In lines
                        If Not firstline Then
                            'split the line
                            Dim elements() As String = line.Split(New Char() {","}, StringSplitOptions.RemoveEmptyEntries)
                            '3 entries per line required
                            If (elements.Length <> 3) Then
                                Throw New ArgumentException("Incorrect number of values in csv file")
                            End If
                            'add values to map
                            map.Add(New AlternatorMapValues(elements(0), elements(2), elements(1)))
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

        ''' <summary>
        ''' Returns the alternator efficiency at given rpm
        ''' </summary>
        ''' <param name="rpm">alternator rotation speed</param>
        ''' <returns>Single</returns>
        ''' <remarks></remarks>
        Public Function GetEfficiency(ByVal rpm As Integer) As Single
            Dim tupleValue As AlternatorMapValues = GetValueOrInterpolate(rpm)
            Dim value As Single = tupleValue.Efficiency
            Return value
        End Function

        ''' <summary>
        ''' Returns the alternator Maximum Regeneration Power at given rpm
        ''' </summary>
        ''' <param name="rpm">alternator rotation speed</param>
        ''' <returns>Single</returns>
        ''' <remarks></remarks>
        Public Function GetMaximumRegenerationPower(ByVal rpm As Integer) As Single
            Dim value As AlternatorMapValues = GetValueOrInterpolate(rpm)
            Return value.Amps
        End Function

        ''' <summary>
        ''' Returns a AlternatorMapValues instance containing the entries at a given key, or new interpolated values
        ''' </summary>
        ''' <returns>AlternatorMapValues</returns>
        ''' <remarks>Throws exception if rpm are outside map</remarks>
        Private Function GetValueOrInterpolate(ByVal rpm As Integer) As AlternatorMapValues
            'check the rpm is within the map
            Dim min As Integer = map.Keys.Min()
            Dim max As Integer = map.Keys.Max()
            If rpm < min OrElse rpm > max Then
                Throw New ArgumentOutOfRangeException(String.Format("Extrapolation - rpm should be in the range {0} to {1}", min, max), rpm)
            End If

            'Check if the rpm is in the current memo
            'If supplied rpm is a key, we can just return the values
            If map.ContainsKey(rpm) Then
                Return map(rpm)
            End If

            'Not a key value, interpolate
            'get the entries before and after the supplied rpm
            Dim pre As KeyValuePair(Of Integer, AlternatorMapValues) = (From m In map Where m.Key < rpm Select m).Last()
            Dim post As KeyValuePair(Of Integer, AlternatorMapValues) = (From m In map Where m.Key > rpm Select m).First()

            'get the delta values for rpm and the values
            Dim dRpm As Integer = post.Key - pre.Key
            Dim dEfficiency As Single = post.Value.Efficiency - pre.Value.Efficiency
            Dim dPower As Single = post.Value.Amps - pre.Value.Amps

            'calculate the slopes
            Dim efficiencySlope As Single = dEfficiency / dRpm
            Dim powerSlope As Single = dPower / dRpm

            'calculate the new values
            Dim efficiency As Single = ((rpm - pre.Key) * efficiencySlope) + pre.Value.Efficiency
            Dim regenPower As Single = ((rpm - pre.Key) * powerSlope) + pre.Value.Amps

            'Build a new AlternatorMapValues instance
            Return New AlternatorMapValues(efficiency, regenPower, rpm)


        End Function

        ''' <summary>
        ''' Encapsulates Efficiency and Maximum Regeneration Power values for Alternator
        ''' </summary>
        Public Structure AlternatorMapValues



            Public ReadOnly RPM As Integer


            ''' <summary>
            ''' Efficiency of alternator at a given rotation speed
            ''' </summary>
            Public ReadOnly Efficiency As Single

            ''' <summary>
            ''' Maximum regeneration rower of alternator at a given rotation speed
            ''' </summary>
            Public ReadOnly Amps As Single

            ''' <summary>
            ''' Creates a new instance of AlternatorMapValues
            ''' </summary>
            ''' <param name="efficiency">Efficiency Value</param>
            ''' <param name="maxRegenPower">Maximum Regeneration Power value</param>
            Public Sub New(ByVal rpm As Integer, ByVal amps As Single, efficiency As Single)
                Me.Efficiency = efficiency
                Me.Amps = amps
                Me.RPM = rpm
            End Sub

        End Structure

    End Class



