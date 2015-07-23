' Copyright 2015 European Union.
' Licensed under the EUPL (the 'Licence');
'
' * You may not use this work except in compliance with the Licence.
' * You may obtain a copy of the Licence at: http://ec.europa.eu/idabc/eupl
' * Unless required by applicable law or agreed to in writing,
'   software distributed under the Licence is distributed on an "AS IS" basis,
'   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
'
' See the LICENSE.txt for the specific language governing permissions and limitations.

Imports VectoAuxiliaries.Electrics
Imports VectoAuxiliaries.Pneumatics
Imports VectoAuxiliaries.Hvac
Imports System.IO
Imports VectoAuxiliaries.DownstreamModules
Imports System.Windows.Forms
Imports Newtonsoft.Json
Imports VectoAuxiliaries

<Serializable()>
Public Class AuxiliaryConfig
    Implements IAuxiliaryConfig

    'Vecto
    Public Property VectoInputs As IVectoInputs Implements IAuxiliaryConfig.VectoInputs

    'Electrical
    Public Property ElectricalUserInputsConfig As IElectricsUserInputsConfig Implements IAuxiliaryConfig.ElectricalUserInputsConfig

    'Pneumatics
    Public Property PneumaticUserInputsConfig As IPneumaticUserInputsConfig Implements IAuxiliaryConfig.PneumaticUserInputsConfig
    Public Property PneumaticAuxillariesConfig As IPneumaticsAuxilliariesConfig Implements IAuxiliaryConfig.PneumaticAuxillariesConfig

    'Hvac
    Public Property HvacUserInputsConfig As IHVACUserInputsConfig Implements IAuxiliaryConfig.HvacUserInputsConfig

    'Vecto Signals
    Public Property Signals As ISignals

    'Constructors
    Sub New()

        Call Me.New("EMPTY")

    End Sub

    Public Sub New(auxConfigFile As String)

        'Special Condition
        If auxConfigFile = "EMPTY" Then
            ElectricalUserInputsConfig = New ElectricsUserInputsConfig() With {.PowerNetVoltage = 28.3}
            ElectricalUserInputsConfig.ElectricalConsumers = New ElectricalConsumerList(28.3, 0.096, False)
            ElectricalUserInputsConfig.ResultCardIdle = New ResultCard(New List(Of SmartResult))
            ElectricalUserInputsConfig.ResultCardOverrun = New ResultCard(New List(Of SmartResult))
            ElectricalUserInputsConfig.ResultCardTraction = New ResultCard(New List(Of SmartResult))
            PneumaticAuxillariesConfig = New PneumaticsAuxilliariesConfig(False)
            PneumaticUserInputsConfig = New PneumaticUserInputsConfig(False)
            HvacUserInputsConfig = New HVACUserInputsConfig(String.Empty, String.Empty, False)
            Exit Sub

        End If

        If auxConfigFile Is Nothing OrElse auxConfigFile.Trim().Length = 0 OrElse Not FILE.Exists(auxConfigFile) Then

            setdefaults()

        Else

            setDefaults()

            If Not Load(auxConfigFile) Then
                MessageBox.Show(String.Format("Unable to load file  {0}", auxConfigFile))
            End If

        End If

    End Sub

    'Set Default Values
    Private Sub setDefaults()

        VectoInputs = New VectoInputs With {.Cycle = "Urban", .VehicleWeightKG = 16500, .PowerNetVoltage = 28.3, .FuelMap = "testFuelGoodMap.vmap"}
        Signals = New Signals With {.EngineSpeed = 2000, .TotalCycleTimeSeconds = 3114, .ClutchEngaged = False}

        'Pneumatics set deault values
        PneumaticUserInputsConfig = New PneumaticUserInputsConfig(True)
        PneumaticAuxillariesConfig = New PneumaticsAuxilliariesConfig(True)

        'Electrical set deault values
        ElectricalUserInputsConfig = New ElectricsUserInputsConfig(True, VectoInputs)
        ElectricalUserInputsConfig.ElectricalConsumers = New ElectricalConsumerList(28.3, 0.096, True)

        'HVAC set deault values
        HvacUserInputsConfig = New HVACUserInputsConfig(String.Empty, String.Empty, False)

    End Sub

    Private Function GetDoorActuationTimeFraction() As Single

        Dim actuationsMap As PneumaticActuationsMAP = New PneumaticActuationsMAP(PneumaticUserInputsConfig.ActuationsMap)
        Dim actuationsKey As ActuationsKey = New ActuationsKey("Park brake + 2 doors", VectoInputs.Cycle)

        Dim numActuations As Single = actuationsMap.GetNumActuations(actuationsKey)
        Dim secondsPerActuation As Single = ElectricalUserInputsConfig.DoorActuationTimeSecond

        Dim doorDutyCycleFraction As Single = (numActuations * secondsPerActuation) / Signals.TotalCycleTimeSeconds

        Return doorDutyCycleFraction

    End Function

#Region "Comparison"

    Private Function CompareElectricalConfiguration(other As AuxiliaryConfig) As Boolean

        'AlternatorGearEfficiency
        If Me.ElectricalUserInputsConfig.AlternatorGearEfficiency <> other.ElectricalUserInputsConfig.AlternatorGearEfficiency Then Return False

        'AlternatorMap
        If Me.ElectricalUserInputsConfig.AlternatorMap <> other.ElectricalUserInputsConfig.AlternatorMap Then Return False

        'DoorActuationTimeSecond
        If Me.ElectricalUserInputsConfig.DoorActuationTimeSecond <> other.ElectricalUserInputsConfig.DoorActuationTimeSecond Then Return False


        'Consumer list
        If Me.ElectricalUserInputsConfig.ElectricalConsumers.Items.Count <> other.ElectricalUserInputsConfig.ElectricalConsumers.Items.count Then Return False
        Dim i As Integer
        For i = 0 To Me.ElectricalUserInputsConfig.ElectricalConsumers.Items.Count - 1
            Dim thisConsumer, otherConsumer As IElectricalConsumer
            thisConsumer = Me.ElectricalUserInputsConfig.ElectricalConsumers.Items(i)
            otherConsumer = other.ElectricalUserInputsConfig.ElectricalConsumers.Items(i)

            If thisConsumer.AvgConsumptionAmps <> otherConsumer.AvgConsumptionAmps OrElse _
                thisConsumer.BaseVehicle <> otherConsumer.BaseVehicle OrElse _
                thisConsumer.Category <> otherConsumer.Category OrElse _
                thisConsumer.ConsumerName <> otherConsumer.ConsumerName OrElse _
                thisConsumer.NominalConsumptionAmps <> otherConsumer.NominalConsumptionAmps OrElse _
                thisConsumer.NumberInActualVehicle <> otherConsumer.NumberInActualVehicle OrElse _
                thisConsumer.PhaseIdle_TractionOn <> otherConsumer.PhaseIdle_TractionOn OrElse _
                thisConsumer.TotalAvgConsumptionInWatts <> otherConsumer.TotalAvgConsumptionInWatts OrElse _
                thisConsumer.TotalAvgConumptionAmps <> otherConsumer.TotalAvgConumptionAmps Then Return False

        Next

        'PowerNetVoltage
        If Me.ElectricalUserInputsConfig.PowerNetVoltage <> other.ElectricalUserInputsConfig.PowerNetVoltage Then Return False

        'ResultCardIdle
        If Me.ElectricalUserInputsConfig.ResultCardIdle.Results.count <> other.ElectricalUserInputsConfig.ResultCardIdle.Results.Count Then Return False
        For i = 0 To Me.ElectricalUserInputsConfig.ResultCardIdle.Results.Count - 1
            If Me.ElectricalUserInputsConfig.ResultCardIdle.Results(i).Amps <> other.ElectricalUserInputsConfig.ResultCardIdle.Results(i).Amps OrElse _
                Me.ElectricalUserInputsConfig.ResultCardIdle.Results(i).SmartAmps <> other.ElectricalUserInputsConfig.ResultCardIdle.Results(i).SmartAmps Then Return False
        Next

        'ResultCardOverrun
        If Me.ElectricalUserInputsConfig.ResultCardOverrun.Results.count <> other.ElectricalUserInputsConfig.ResultCardOverrun.Results.Count Then Return False
        For i = 0 To Me.ElectricalUserInputsConfig.ResultCardOverrun.Results.Count - 1
            If Me.ElectricalUserInputsConfig.ResultCardOverrun.Results(i).Amps <> other.ElectricalUserInputsConfig.ResultCardOverrun.Results(i).Amps OrElse _
                Me.ElectricalUserInputsConfig.ResultCardOverrun.Results(i).SmartAmps <> other.ElectricalUserInputsConfig.ResultCardOverrun.Results(i).SmartAmps Then Return False
        Next


        'ResultCardTraction
        If Me.ElectricalUserInputsConfig.ResultCardTraction.Results.count <> other.ElectricalUserInputsConfig.ResultCardTraction.Results.Count Then Return False
        For i = 0 To Me.ElectricalUserInputsConfig.ResultCardTraction.Results.Count - 1
            If Me.ElectricalUserInputsConfig.ResultCardTraction.Results(i).Amps <> other.ElectricalUserInputsConfig.ResultCardTraction.Results(i).Amps OrElse _
                Me.ElectricalUserInputsConfig.ResultCardTraction.Results(i).SmartAmps <> other.ElectricalUserInputsConfig.ResultCardTraction.Results(i).SmartAmps Then Return False
        Next

        'SmartElectrical
        If Me.ElectricalUserInputsConfig.SmartElectrical <> other.ElectricalUserInputsConfig.SmartElectrical Then Return False


        Return True


    End Function
    Private Function ComparePneumaticAuxiliariesConfig(other As AuxiliaryConfig) As Boolean

        If Me.PneumaticAuxillariesConfig.AdBlueNIperMinute <> other.PneumaticAuxillariesConfig.AdBlueNIperMinute Then Return False
        If Me.PneumaticAuxillariesConfig.AirControlledSuspensionNIperMinute <> other.PneumaticAuxillariesConfig.AirControlledSuspensionNIperMinute Then Return False
        If Me.PneumaticAuxillariesConfig.BrakingNoRetarderNIperKG <> other.PneumaticAuxillariesConfig.BrakingNoRetarderNIperKG Then Return False
        If Me.PneumaticAuxillariesConfig.BrakingWithRetarderNIperKG <> other.PneumaticAuxillariesConfig.BrakingWithRetarderNIperKG Then Return False
        If Me.PneumaticAuxillariesConfig.BreakingPerKneelingNIperKGinMM <> other.PneumaticAuxillariesConfig.BreakingPerKneelingNIperKGinMM Then Return False
        If Me.PneumaticAuxillariesConfig.DeadVolBlowOutsPerLitresperHour <> other.PneumaticAuxillariesConfig.DeadVolBlowOutsPerLitresperHour Then Return False
        If Me.PneumaticAuxillariesConfig.DeadVolumeLitres <> other.PneumaticAuxillariesConfig.DeadVolumeLitres Then Return False
        If Me.PneumaticAuxillariesConfig.NonSmartRegenFractionTotalAirDemand <> other.PneumaticAuxillariesConfig.NonSmartRegenFractionTotalAirDemand Then Return False
        If Me.PneumaticAuxillariesConfig.PerDoorOpeningNI <> other.PneumaticAuxillariesConfig.PerDoorOpeningNI Then Return False
        If Me.PneumaticAuxillariesConfig.PerStopBrakeActuationNIperKG <> other.PneumaticAuxillariesConfig.PerStopBrakeActuationNIperKG Then Return False
        If Me.PneumaticAuxillariesConfig.SmartRegenFractionTotalAirDemand <> other.PneumaticAuxillariesConfig.SmartRegenFractionTotalAirDemand Then Return False
        If Me.PneumaticAuxillariesConfig.OverrunUtilisationForCompressionFraction <> other.PneumaticAuxillariesConfig.OverrunUtilisationForCompressionFraction Then Return False

        Return True

    End Function
    Private Function ComparePneumaticUserConfig(other As AuxiliaryConfig) As Boolean

        If Me.PneumaticUserInputsConfig.ActuationsMap <> other.PneumaticUserInputsConfig.ActuationsMap Then Return False
        If Me.PneumaticUserInputsConfig.AdBlueDosing <> other.PneumaticUserInputsConfig.AdBlueDosing Then Return False
        If Me.PneumaticUserInputsConfig.AirSuspensionControl <> other.PneumaticUserInputsConfig.AirSuspensionControl Then Return False
        If Me.PneumaticUserInputsConfig.CompressorGearEfficiency <> other.PneumaticUserInputsConfig.CompressorGearEfficiency Then Return False
        If Me.PneumaticUserInputsConfig.CompressorGearRatio <> other.PneumaticUserInputsConfig.CompressorGearRatio Then Return False
        If Me.PneumaticUserInputsConfig.CompressorMap <> other.PneumaticUserInputsConfig.CompressorMap Then Return False
        If Me.PneumaticUserInputsConfig.Doors <> other.PneumaticUserInputsConfig.Doors Then Return False
        If Me.PneumaticUserInputsConfig.KneelingHeightMillimeters <> other.PneumaticUserInputsConfig.KneelingHeightMillimeters Then Return False
        If Me.PneumaticUserInputsConfig.RetarderBrake <> other.PneumaticUserInputsConfig.RetarderBrake Then Return False
        If Me.PneumaticUserInputsConfig.SmartAirCompression <> other.PneumaticUserInputsConfig.SmartAirCompression Then Return False
        If Me.PneumaticUserInputsConfig.SmartRegeneration <> other.PneumaticUserInputsConfig.SmartRegeneration Then Return False

        Return True

    End Function
    Private Function CompareHVACConfig(other As AuxiliaryConfig) As Boolean Implements IAuxiliaryConfig.ConfigValuesAreTheSameAs

        If Me.HvacUserInputsConfig.SSMFilePath <> other.HvacUserInputsConfig.SSMFilePath Then Return False
        If Me.HvacUserInputsConfig.BusDatabasePath <> other.HvacUserInputsConfig.BusDatabasePath Then Return False
        If Me.HvacUserInputsConfig.SSMDisabled <> other.HvacUserInputsConfig.SSMDisabled Then Return False

        Return True

    End Function

    Public Function ConfigValuesAreTheSameAs(other As AuxiliaryConfig) As Boolean

        If Not CompareElectricalConfiguration(other) Then Return False
        If Not ComparePneumaticAuxiliariesConfig(other) Then Return False
        If Not ComparePneumaticUserConfig(other) Then Return False
        If Not CompareHVACConfig(other) Then Return False

        Return True

    End Function


#End Region

#Region "Persistance"

    'Persistance Functions
    Public Function Save(auxFile As String) As Boolean Implements IAuxiliaryConfig.Save

        Dim returnValue As Boolean = True
        Dim settings As JsonSerializerSettings = New JsonSerializerSettings()
        settings.TypeNameHandling = TypeNameHandling.Objects

        'JSON METHOD
        Try

            Dim output As String = JsonConvert.SerializeObject(Me, Formatting.Indented, settings)

            File.WriteAllText(auxFile, output)

        Catch ex As Exception

            returnValue = False

        End Try

        Return returnValue

    End Function
    Public Function Load(auxFile As String) As Boolean Implements IAuxiliaryConfig.Load

        Dim returnValue As Boolean = True
        Dim settings As JsonSerializerSettings = New JsonSerializerSettings()
        Dim tmpAux As AuxiliaryConfig

        settings.TypeNameHandling = TypeNameHandling.Objects

        'JSON METHOD
        Try

            Dim output As String = File.ReadAllText(auxFile)


            tmpAux = JsonConvert.DeserializeObject(Of AuxiliaryConfig)(output, settings)

            'This is where we Assume values of loaded( Deserialized ) object.
            AssumeValuesOfOther(tmpAux)

        Catch ex As Exception

            returnValue = False
        End Try

        Return returnValue

    End Function

    'Persistance Helpers
    Public Sub AssumeValuesOfOther(other As AuxiliaryConfig)

        CloneElectricaConfiguration(other)
        ClonePneumaticsAuxiliariesConfig(other)
        ClonePneumaticsUserInputsConfig(other)
        CloneHVAC(other)

    End Sub
    Private Sub CloneElectricaConfiguration(other As AuxiliaryConfig)

        'AlternatorGearEfficiency
        Me.ElectricalUserInputsConfig.AlternatorGearEfficiency = other.ElectricalUserInputsConfig.AlternatorGearEfficiency
        'AlternatorMap
        Me.ElectricalUserInputsConfig.AlternatorMap = other.ElectricalUserInputsConfig.AlternatorMap
        'DoorActuationTimeSecond
        Me.ElectricalUserInputsConfig.DoorActuationTimeSecond = other.ElectricalUserInputsConfig.DoorActuationTimeSecond

        'Electrical Consumer list
        Me.ElectricalUserInputsConfig.ElectricalConsumers.Items.Clear()
        For Each otherConsumer As IElectricalConsumer In other.ElectricalUserInputsConfig.ElectricalConsumers.Items

            Dim newConsumer As ElectricalConsumer = New ElectricalConsumer(otherConsumer.BaseVehicle, _
                                                                            otherConsumer.Category, _
                                                                            otherConsumer.ConsumerName, _
                                                                            otherConsumer.NominalConsumptionAmps, _
                                                                            otherConsumer.PhaseIdle_TractionOn, _
                                                                            otherConsumer.PowerNetVoltage, _
                                                                            otherConsumer.NumberInActualVehicle,
                                                                            otherConsumer.info)

            Me.ElectricalUserInputsConfig.ElectricalConsumers.Items.Add(newConsumer)

        Next

        'PowerNetVoltage
        Me.ElectricalUserInputsConfig.PowerNetVoltage = other.ElectricalUserInputsConfig.PowerNetVoltage
        'ResultCardIdle
        Me.ElectricalUserInputsConfig.ResultCardIdle.Results.Clear()
        For Each result As SmartResult In other.ElectricalUserInputsConfig.ResultCardIdle.Results
            Me.ElectricalUserInputsConfig.ResultCardIdle.Results.Add(New SmartResult(result.Amps, result.SmartAmps))
        Next
        'ResultCardOverrun
        Me.ElectricalUserInputsConfig.ResultCardOverrun.Results.Clear()
        For Each result As SmartResult In other.ElectricalUserInputsConfig.ResultCardOverrun.Results
            Me.ElectricalUserInputsConfig.ResultCardOverrun.Results.Add(New SmartResult(result.Amps, result.SmartAmps))
        Next
        'ResultCardTraction
        Me.ElectricalUserInputsConfig.ResultCardTraction.Results.Clear()
        For Each result As SmartResult In other.ElectricalUserInputsConfig.ResultCardTraction.Results
            Me.ElectricalUserInputsConfig.ResultCardTraction.Results.Add(New SmartResult(result.Amps, result.SmartAmps))
        Next
        'SmartElectrical
        Me.ElectricalUserInputsConfig.SmartElectrical = other.ElectricalUserInputsConfig.SmartElectrical

    End Sub
    Private Sub ClonePneumaticsAuxiliariesConfig(other As AuxiliaryConfig)

        Me.PneumaticAuxillariesConfig.AdBlueNIperMinute = other.PneumaticAuxillariesConfig.AdBlueNIperMinute
        Me.PneumaticAuxillariesConfig.AirControlledSuspensionNIperMinute = other.PneumaticAuxillariesConfig.AirControlledSuspensionNIperMinute
        Me.PneumaticAuxillariesConfig.BrakingNoRetarderNIperKG = other.PneumaticAuxillariesConfig.BrakingNoRetarderNIperKG
        Me.PneumaticAuxillariesConfig.BrakingWithRetarderNIperKG = other.PneumaticAuxillariesConfig.BrakingWithRetarderNIperKG
        Me.PneumaticAuxillariesConfig.BreakingPerKneelingNIperKGinMM = other.PneumaticAuxillariesConfig.BreakingPerKneelingNIperKGinMM
        Me.PneumaticAuxillariesConfig.DeadVolBlowOutsPerLitresperHour = other.PneumaticAuxillariesConfig.DeadVolBlowOutsPerLitresperHour
        Me.PneumaticAuxillariesConfig.DeadVolumeLitres = other.PneumaticAuxillariesConfig.DeadVolumeLitres
        Me.PneumaticAuxillariesConfig.NonSmartRegenFractionTotalAirDemand = other.PneumaticAuxillariesConfig.NonSmartRegenFractionTotalAirDemand
        Me.PneumaticAuxillariesConfig.PerDoorOpeningNI = other.PneumaticAuxillariesConfig.PerDoorOpeningNI
        Me.PneumaticAuxillariesConfig.PerStopBrakeActuationNIperKG = other.PneumaticAuxillariesConfig.PerStopBrakeActuationNIperKG
        Me.PneumaticAuxillariesConfig.SmartRegenFractionTotalAirDemand = other.PneumaticAuxillariesConfig.SmartRegenFractionTotalAirDemand
        Me.PneumaticAuxillariesConfig.OverrunUtilisationForCompressionFraction = other.PneumaticAuxillariesConfig.OverrunUtilisationForCompressionFraction

    End Sub
    Private Sub ClonePneumaticsUserInputsConfig(other As AuxiliaryConfig)

        Me.PneumaticUserInputsConfig.ActuationsMap = other.PneumaticUserInputsConfig.ActuationsMap
        Me.PneumaticUserInputsConfig.AdBlueDosing = other.PneumaticUserInputsConfig.AdBlueDosing
        Me.PneumaticUserInputsConfig.AirSuspensionControl = other.PneumaticUserInputsConfig.AirSuspensionControl
        Me.PneumaticUserInputsConfig.CompressorGearEfficiency = other.PneumaticUserInputsConfig.CompressorGearEfficiency
        Me.PneumaticUserInputsConfig.CompressorGearRatio = other.PneumaticUserInputsConfig.CompressorGearRatio
        Me.PneumaticUserInputsConfig.CompressorMap = other.PneumaticUserInputsConfig.CompressorMap
        Me.PneumaticUserInputsConfig.Doors = other.PneumaticUserInputsConfig.Doors
        Me.PneumaticUserInputsConfig.KneelingHeightMillimeters = other.PneumaticUserInputsConfig.KneelingHeightMillimeters
        Me.PneumaticUserInputsConfig.RetarderBrake = other.PneumaticUserInputsConfig.RetarderBrake
        Me.PneumaticUserInputsConfig.SmartAirCompression = other.PneumaticUserInputsConfig.SmartAirCompression
        Me.PneumaticUserInputsConfig.SmartRegeneration = other.PneumaticUserInputsConfig.SmartRegeneration

    End Sub
    Private Sub CloneHVAC(other As AuxiliaryConfig)

        Me.HvacUserInputsConfig.SSMFilePath = other.HvacUserInputsConfig.SSMFilePath
        Me.HvacUserInputsConfig.BusDatabasePath = other.HvacUserInputsConfig.BusDatabasePath
        Me.HvacUserInputsConfig.SSMDisabled = other.HvacUserInputsConfig.SSMDisabled

    End Sub

#End Region


End Class

