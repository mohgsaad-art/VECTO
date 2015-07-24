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

Namespace DownstreamModules

    Public Class M11
        Implements IM11

        Private Const RPM_to_RadiansPerSecond As Single = 9.55

#Region "Private Aggregates"
        'Private Aggregations
        Private AG1 As Single
        Private AG2 As Single
        Private AG3 As Single
        Private AG4 As Single
        Private AG5 As Single
        Private AG6 As Single
        Private AG7 As Single
#End Region

#Region "Private Fields Assigned by Constructor."

        Private M1 As IM1_AverageHVACLoadDemand
        Private M3 As IM3_AveragePneumaticLoadDemand
        Private M6 As IM6
        Private M8 As IM8
        Private fmap As IFUELMAP
        Private signals As ISignals

#End Region

        'Staging Calculations
        Private Function Sum0(ByVal rpm As Single) As Single

            If rpm < 1 Then rpm = 1

            Return rpm / RPM_to_RadiansPerSecond


        End Function
        Private ReadOnly Property Sum1 As Single
            Get
                Return m6.OverrunFlag * m8.SmartElectricalAlternatorPowerGenAtCrank
            End Get
        End Property
        Private ReadOnly Property Sum2 As Single
            Get
                Return m3.GetAveragePowerDemandAtCrankFromPneumatics + m1.AveragePowerDemandAtCrankFromHVACMechanicalsWatts
            End Get
        End Property
        Private ReadOnly Property Sum3 As Single
            Get
                Return m8.SmartElectricalAlternatorPowerGenAtCrank / Sum0(signals.EngineSpeed)
            End Get
        End Property
        Private ReadOnly Property Sum4 As Single
            Get
                Return Sum2 / Sum0(signals.EngineSpeed)
            End Get
        End Property
        Private ReadOnly Property Sum5 As Single
            Get
                Return Sum4 + Sum9
            End Get
        End Property
        Private ReadOnly Property Sum6 As Single
            Get
                Return Sum3 + Sum5
            End Get
        End Property
        Private ReadOnly Property Sum7 As Single
            Get

                'SCM 3_02
                Dim intrp1 As Single = fmap.fFCdelaunay_Intp(signals.EngineSpeed, sum6)
                intrp1 = If(Not Single.IsNaN(intrp1) AndAlso intrp1 > 0, intrp1, 0)
                Return intrp1

            End Get
        End Property
        Private ReadOnly Property Sum8 As Single
            Get

                'SCHM 3_2
                Dim intrp2 As Single = fmap.fFCdelaunay_Intp(signals.EngineSpeed, sum5)
                intrp2 = If(Not Single.IsNaN(intrp2) AndAlso intrp2 > 0, intrp2, 0)
                Return intrp2

            End Get
        End Property
        Private ReadOnly Property Sum9 As Single
            Get

                Return signals.EngineDrivelineTorque + ((signals.PreExistingAuxPower * 1000) / Sum0(signals.EngineSpeed))

            End Get
        End Property
        Private ReadOnly Property Sum10 As Single
            Get

                Return M6.AvgPowerDemandAtCrankFromElectricsIncHVAC \ Sum0(signals.EngineSpeed)

            End Get
        End Property
        Private ReadOnly Property Sum11 As Single
            Get

                Return Sum5 + Sum10

            End Get
        End Property
        Private ReadOnly Property Sum12 As Single
            Get

                'SCHM 3_2
                Dim intrp3 As Single = fmap.fFCdelaunay_Intp(signals.EngineSpeed, Sum11)
                intrp3 = If(Not Single.IsNaN(intrp3) AndAlso intrp3 > 0, intrp3, 0)
                Return intrp3

            End Get
        End Property

        'OUT1
        Public ReadOnly Property SmartElectricalTotalCycleElectricalEnergyGeneratedDuringOverrunOnly As Single Implements IM11.SmartElectricalTotalCycleElectricalEnergyGeneratedDuringOverrunOnly
            Get
                Return AG1
            End Get
        End Property
        'OUT2
        Public ReadOnly Property SmartElectricalTotalCycleEletricalEnergyGenerated As Single Implements IM11.SmartElectricalTotalCycleEletricalEnergyGenerated
            Get
                Return AG2
            End Get
        End Property
        'OUT3
        Public ReadOnly Property TotalCycleElectricalDemand As Single Implements IM11.TotalCycleElectricalDemand
            Get
                Return AG3
            End Get
        End Property
        'OUT4
        Public ReadOnly Property TotalCycleFuelConsumptionSmartElectricalLoad As Single Implements IM11.TotalCycleFuelConsumptionSmartElectricalLoad
            Get
                Return AG4
            End Get
        End Property
        'OUT5
        Public ReadOnly Property TotalCycleFuelConsumptionZeroElectricalLoad As Single Implements IM11.TotalCycleFuelConsumptionZeroElectricalLoad
            Get
                Return AG5
            End Get
        End Property
        'OUT6
        Public ReadOnly Property StopStartSensitiveTotalCycleElectricalDemand As Single Implements IM11.StopStartSensitiveTotalCycleElectricalDemand
            Get
                Return AG6
            End Get
        End Property
        'OUT7
        Public ReadOnly Property TotalCycleFuelConsuptionAverageLoads As Single Implements IM11.TotalCycleFuelConsuptionAverageLoads
            Get
                Return AG7
            End Get
        End Property

        Private ReadOnly Property SW1 As Single
            Get
                Return If(signals.EngineStopped, 0, 1)
            End Get
        End Property

        'Clear at the beginning of cycle
        Sub ClearAggregates() Implements IM11.ClearAggregates

            AG1 = 0
            AG2 = 0
            AG3 = 0
            AG4 = 0
            AG5 = 0
            AG6 = 0
            AG7 = 0

        End Sub

        'Add to Aggregates dependent on cycle step time.
        Sub CycleStep(Optional stepTimeInSeconds As Single = 0.0) Implements IM11.CycleStep

            'S/S Insensitive
            AG3 += (stepTimeInSeconds * m6.AvgPowerDemandAtCrankFromElectricsIncHVAC)


            If Signals.EngineStopped Then Return

            'S/S Sensitive
            AG1 += (stepTimeInSeconds * sum1 * SW1)
            AG2 += (stepTimeInSeconds * M8.SmartElectricalAlternatorPowerGenAtCrank * SW1)

            AG6 += (stepTimeInSeconds * m6.AvgPowerDemandAtCrankFromElectricsIncHVAC * SW1)

            'These need to be divided by 3600 as the Fuel Map output is in Grams/Second.
            AG4 += (stepTimeInSeconds * sum7 / 3600 * SW1)
            AG5 += (stepTimeInSeconds * Sum8 / 3600 * SW1)
            AG7 += (stepTimeInSeconds * Sum12 / 3600 * SW1)

        End Sub

        'Constructor
        Public Sub New(m1 As IM1_AverageHVACLoadDemand, m3 As IM3_AveragePneumaticLoadDemand, m6 As IM6, m8 As IM8, fmap As IFUELMAP, signals As ISignals)

            Me.M1 = m1
            Me.M3 = m3
            Me.M6 = m6
            Me.M8 = m8
            Me.fmap = fmap
            Me.signals = signals

        End Sub

    End Class

End Namespace




