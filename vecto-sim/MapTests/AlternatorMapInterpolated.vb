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

        Public map As Dictionary(Of AlternatorMapKey, AlternatorMapValues)

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

                    map = New Dictionary(Of AlternatorMapKey, AlternatorMapValues)()
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

                            'Create AlternatorKey
                            Dim aKey As AlternatorMapKey = New AlternatorMapKey(elements(0), elements(1))
                            Dim aValue As AlternatorMapValues = New AlternatorMapValues()

                            'Add Efficiency Value to Key.
                            map.Add(aKey, New AlternatorMapValues(elements(2)))

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
            'Dim tupleValue As AlternatorMapValues = GetValueOrInterpolate(rpm)
            'Dim value As Single = tupleValue.Efficiency
            'Return value
            Return 0 'TODO:FIX THIS
        End Function

        ''' <summary>
        ''' Returns the alternator Maximum Regeneration Power at given rpm
        ''' </summary>
        ''' <param name="rpm">alternator rotation speed</param>
        ''' <returns>Single</returns>
        ''' <remarks></remarks>
        Public Function GetMaximumRegenerationPower(ByVal rpm As Integer) As Single
            'Dim value As AlternatorMapValues = GetValueOrInterpolate(rpm)
            'Return value.Amps
            Return 0 'TODO:FIX THIS
        End Function

        ''' <summary>
        ''' Returns a AlternatorMapValues instance containing the entries at a given key, or new interpolated values
        ''' </summary>
        ''' <returns>AlternatorMapValues</returns>
        ''' <remarks>Throws exception if rpm are outside map</remarks>
        Public Function GetValueOrInterpolate(mapKey As AlternatorMapKey) As AlternatorMapValues
            'check the rpm is within the map


            'Dim min As AlternatorMapKey = map.Keys.Min()
            'Dim max As AlternatorMapKey = map.Keys.Max()

            'If mapKey.amps < 0 OrElse mapKey.amps > max.amps OrElse mapKey.rpm < 0 OrElse mapKey.rpm > max.rpm Then
            '    Throw New ArgumentOutOfRangeException(String.Format("Extrapolation - Amp/Rpm Values should should be in the range {0} to {1}", min.ToString(), max.ToString()))
            'End If

            'Check if the rpm is in the current memo
            'If supplied present key, we can just return the values
            If map.ContainsKey(mapKey) Then
                Return map(mapKey)
            End If


            'Get Pre and Post Keys.
            Dim rpmEfficiency As Single
            Dim ampsEfficiency As Single
            Dim rpmPre As AlternatorMapValues
            Dim rpmPost As AlternatorMapValues
            Dim ampsPre As AlternatorMapValues
            Dim ampsPost As AlternatorMapValues

            Dim intRpmPre As Integer
            Dim intRpmPost As Integer
            Dim intAmpsPre As Integer
            Dim intAmpsPost As Integer

            intRpmPre = (From m In map Where m.Key.rpm < mapKey.rpm Select m.Key.rpm).Last()
            intRpmPost = (From m In map Where m.Key.rpm > mapKey.rpm Select m.Key.rpm).First()
            intAmpsPre = (From m In map Where m.Key.amps < mapKey.amps Select m.Key.amps).Last()
            intAmpsPost = (From m In map Where m.Key.amps > mapKey.amps Select m.Key.amps).First()

            rpmPre = map(New AlternatorMapKey(intAmpsPre, intRpmPre))
            rpmPost = map(New AlternatorMapKey(intAmpsPre, intRpmPost))

            ampsPre = map(New AlternatorMapKey(intAmpsPost, intRpmPre))
            ampsPost = map(New AlternatorMapKey(intAmpsPost, intRpmPost))


            '**********     A-B  Efficiency  ( Lower Amps )  ************
            'get the delta values for rpm and the values
             Dim dRpm As Integer = intRpmPost - intRpmPre
             Dim dRpmEfficiency As Single = rpmPost.Efficiency - rpmPre.Efficiency

            'calculate the slopes
             Dim rpmEfficiencySlope As Single = dRpmEfficiency / dRpm

            'calculate the new values
             Dim AB_Efficiency As Single = ((mapKey.rpm - intRpmPre) * rpmEfficiencySlope) + rpmPre.Efficiency

             '**********     C-D Efficiency  ( Higher Amps )  ************
            'get the delta values for rpm and the values
             dRpm = intRpmPost - intRpmPre
             dRpmEfficiency = ampsPost.Efficiency - ampsPre.Efficiency

            'calculate the slopes
             rpmEfficiencySlope = dRpmEfficiency / dRpm

            'calculate the new values
             Dim CD_Efficiency As Single = ((mapKey.rpm - intRpmPre) * rpmEfficiencySlope) + ampsPre.Efficiency


             '(C-D) - (A-B) Efficiency
             'Deltas
             Dim dAmps = intAmpsPost - intAmpsPre
             Dim dAmpEfficiency = CD_Efficiency - AB_Efficiency

             'slopes
             Dim ampsEfficiencySlope As Single = dAmpEfficiency / dAmps

             'calculate final Values
             Dim ABCDEfficiency = ((mapKey.amps - intAmpsPre) * ampsEfficiencySlope) + AB_Efficiency


            Return New AlternatorMapValues(ABCDEfficiency)



        End Function

        ''' <summary>
        ''' Encapsulates Efficiency and Maximum Regeneration Power values for Alternator
        ''' </summary>
        Public Structure AlternatorMapValues


            Public ReadOnly Efficiency As Single



            Public Sub New(ByVal efficiency As Single)
                Me.Efficiency = efficiency
            End Sub

        End Structure


       Public Structure AlternatorMapKey


           Public amps As Integer
           Public rpm As Integer



        Public Sub New(ByVal amps As Integer, ByVal rpm As Integer)

        Me.amps = amps
        Me.rpm = rpm


        End Sub

        Public Overrides Function ToString() As String

          Return "Amps:" & amps & " / " & "Rpm:" & rpm

        End Function

       End Structure


    End Class



