Imports System.IO

Namespace Pneumatics

    ''' <summary>
    ''' Compressor Flow Rate and Power Map
    ''' </summary>
    ''' <remarks></remarks>
    Public Class CompressorMap

        Implements ICompressorMap, 
        IAuxiliaryEvent



        Private ReadOnly filePath As String

        Private _averagePowerDemandPerCompressorUnitFlowRate As Single




        ''' <summary>
        ''' Dictionary of values keyed by the rpm valaues in the csv file
        ''' Values are held as a tuple as follows
        ''' Item1 = flow rate
        ''' Item2 - power [compressor on] 
        ''' Item3 - power [compressor off]
        ''' </summary>
        ''' <remarks></remarks>
        Private map As Dictionary(Of Integer, CompressorMapValues)


        'Returns the AveragePowerDemand ( Power On ) per unit flow rate
        Public Function AveragePowerDemandPerCompressorUnitFlowRate() As Single Implements ICompressorMap.GetAveragePowerDemandPerCompressorUnitFlowRate


            Return _averagePowerDemandPerCompressorUnitFlowRate


        End Function


        ''' <summary>
        ''' Creates a new instance of the CompressorMap class
        ''' </summary>
        ''' <param name="path">full path to csv data file</param>
        ''' <remarks></remarks>
        Public Sub New(ByVal path As String)
            filePath = path
        End Sub

        ''' <summary>
        ''' Initilaises the map from the supplied csv data
        ''' </summary>
        ''' <remarks></remarks>
        Public Function Initialise() As Boolean Implements ICompressorMap.Initialise

            If File.Exists(filePath) Then
                Using sr As StreamReader = New StreamReader(filePath)
                    'get array of lines from csv
                    Dim lines() As String = sr.ReadToEnd().Split(CType(Environment.NewLine, Char()), StringSplitOptions.RemoveEmptyEntries)

                    'Must have at least 2 entries in map to make it usable [dont forget the header row]
                    If lines.Length < 3 Then Throw New ArgumentException("Insufficient rows in csv to build a usable map")

                    map = New Dictionary(Of Integer, CompressorMapValues)()
                    Dim firstline As Boolean = True

                    For Each line As String In lines
                        If Not firstline Then
                            'split the line
                            Dim elements() As String = line.Split(New Char() {","}, StringSplitOptions.RemoveEmptyEntries)
                            '4 entries per line required
                            If (elements.Length <> 4) Then Throw New ArgumentException("Incorrect number of values in csv file")
                            'add values to map
                            map.Add(elements(0), New CompressorMapValues(elements(1), elements(2), elements(3)))
                        Else
                            firstline = False
                        End If
                    Next
                End Using

                'Calculate the Average Power Demand Per Compressor Unit FlowRate
                Dim powerDividedByFlowRateSum As Single = 0
                For Each speed As KeyValuePair(Of Integer, CompressorMapValues) In map
                    powerDividedByFlowRateSum += speed.Value.PowerCompressorOn / speed.Value.FlowRate
                Next
                _averagePowerDemandPerCompressorUnitFlowRate = powerDividedByFlowRateSum / map.Count

            Else
                Throw New ArgumentException("supplied input file does not exist")
            End If

            'If we get here then all should be well and we can return a True value of success.
            Return True


        End Function

        ''' <summary>
        ''' Returns compressor flow rate at the given rotation speed
        ''' </summary>
        ''' <param name="rpm">compressor rotation speed</param>
        ''' <returns></returns>
        ''' <remarks>Single</remarks>
        Public Function GetFlowRate(ByVal rpm As Integer) As Single Implements ICompressorMap.GetFlowRate
            Dim val As CompressorMapValues = InterpolatedTuple(rpm)
            Return val.FlowRate
        End Function

        ''' <summary>
        ''' Returns mechanical power at rpm when compressor is on
        ''' </summary>
        ''' <param name="rpm">compressor rotation speed</param>
        ''' <returns></returns>
        ''' <remarks>Single</remarks>
        Public Function GetPowerCompressorOn(ByVal rpm As Integer) As Single Implements ICompressorMap.GetPowerCompressorOn
            Dim val As CompressorMapValues = InterpolatedTuple(rpm)
            Return val.PowerCompressorOn
        End Function

        ''' <summary>
        ''' Returns mechanical power at rpm when compressor is off
        ''' </summary>
        ''' <param name="rpm">compressor rotation speed</param>
        ''' <returns></returns>
        ''' <remarks>Single</remarks>
        Public Function GetPowerCompressorOff(ByVal rpm As Integer) As Single Implements ICompressorMap.GetPowerCompressorOff
            Dim val As CompressorMapValues = InterpolatedTuple(rpm)
            Return val.PowerCompressorOff
        End Function

        ''' <summary>
        ''' Returns an instance of CompressorMapValues containing the values at a key, or interpolated values
        ''' </summary>
        ''' <returns>CompressorMapValues</returns>
        ''' <remarks>Throws exception if rpm are outside map</remarks>
        Private Function InterpolatedTuple(ByVal rpm As Integer) As CompressorMapValues
            'check the rpm is within the map
            Dim min As Integer = map.Keys.Min()
            Dim max As Integer = map.Keys.Max()

            If rpm < min OrElse rpm > max Then
                OnMessage(Me,String.Format("Compresser has limited RPM of '{2}' to extent of map - rpm should be in the range {0} to {1}", min, max ,rpm), AdvancedAuxiliaryMessageType.Warning)    
               'Limiting as agreed.
               If rpm > max then rpm=max
               If rpm < min then rpm=min

            End If

            'If supplied rpm is a key, we can just return the appropriate tuple
            If map.ContainsKey(rpm) Then
                Return map(rpm)
            End If

            'Not a key value, interpolate
            'get the entries before and after the supplied rpm
            Dim pre As KeyValuePair(Of Integer, CompressorMapValues) = (From m In map Where m.Key < rpm Select m).Last()
            Dim post As KeyValuePair(Of Integer, CompressorMapValues) = (From m In map Where m.Key > rpm Select m).First()

            'get the delta values for rpm and the map values
            Dim dRpm As Integer = post.Key - pre.Key
            Dim dFlowRate As Single = post.Value.FlowRate - pre.Value.FlowRate
            Dim dPowerOn As Single = post.Value.PowerCompressorOn - pre.Value.PowerCompressorOn
            Dim dPowerOff As Single = post.Value.PowerCompressorOff - pre.Value.PowerCompressorOff

            'calculate the slopes
            Dim flowSlope As Single = dFlowRate / dRpm
            Dim powerOnSlope As Single = dPowerOn / dRpm
            Dim powerOffSlope As Single = dPowerOff / dRpm

            'calculate the new values
            Dim flowRate As Single = ((rpm - pre.Key) * flowSlope) + pre.Value.FlowRate
            Dim powerCompressorOn As Single = ((rpm - pre.Key) * powerOnSlope) + pre.Value.PowerCompressorOn
            Dim powerCompressorOff As Single = ((rpm - pre.Key) * powerOffSlope) + pre.Value.PowerCompressorOff

            'Build and return a new CompressorMapValues instance
            Return New CompressorMapValues(flowRate, powerCompressorOn, powerCompressorOff)
        End Function

        ''' <summary>
        ''' Encapsulates compressor map values
        ''' Flow Rate
        ''' Power - Compressor On
        ''' Power - Compressor Off
        ''' </summary>
        ''' <remarks></remarks>
        ''' 
        Private Structure CompressorMapValues

            ''' <summary>
            ''' Compressor flowrate
            ''' </summary>
            ''' <remarks></remarks>
            Public ReadOnly FlowRate As Single

            ''' <summary>
            ''' Power, compressor on
            ''' </summary>
            ''' <remarks></remarks>
            Public ReadOnly PowerCompressorOn As Single

            ''' <summary>
            ''' Power compressor off
            ''' </summary>
            ''' <remarks></remarks>
            Public ReadOnly PowerCompressorOff As Single

            ''' <summary>
            ''' Creates a new instance of CompressorMapValues
            ''' </summary>
            ''' <param name="flowRate">flow rate</param>
            ''' <param name="powerCompressorOn">power - compressor on</param>
            ''' <param name="powerCompressorOff">power - compressor off</param>
            ''' <remarks></remarks>
            Public Sub New(ByVal flowRate As Single, ByVal powerCompressorOn As Single, ByVal powerCompressorOff As Single)
                Me.FlowRate = flowRate
                Me.PowerCompressorOn = powerCompressorOn
                Me.PowerCompressorOff = powerCompressorOff
            End Sub

        End Structure



        Public Event Message(ByRef sender As Object, message As String, messageType As AdvancedAuxiliaryMessageType) Implements IAuxiliaryEvent.AuxiliaryEvent 

        Public Sub OnMessage(sender As Object, message As String, messageType As AdvancedAuxiliaryMessageType) 


          If Not message is Nothing then

          RaiseEvent Message( Me, message, messageType)

          End If

        End Sub
    End Class






End Namespace