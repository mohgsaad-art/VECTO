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

    Public Class M12
        Implements IM12


        Private Class Point

            Public X As Single
            Public Y As Single

        End Class

        Private M11 As IM11
        Private M10 As IM10
        Private Signals As ISignals
        Private _P1X, _P1Y, _P2X, _P2Y, _P3X, _P3Y, _XT, _INTERP1, _INTERP2 As Single

        Private Sub setPoints()

            _P1X = 0
            _P1Y = M11.TotalCycleFuelConsumptionZeroElectricalLoad
            _P2X = M11.SmartElectricalTotalCycleEletricalEnergyGenerated
            _P2Y = M11.TotalCycleFuelConsumptionSmartElectricalLoad
            _P3X = M11.StopStartSensitiveTotalCycleElectricalDemand
            _P3Y = M10.AverageLoadsFuelConsumptionInterpolatedForPneumatics
            _XT = M11.TotalCycleElectricalDemand




        End Sub

        'Interpolation 
        Private Function Sum1() As Single

            Dim P1 As Point = New Point With {.X = 0, .Y = M11.TotalCycleFuelConsumptionZeroElectricalLoad}
            Dim P2 As Point = New Point With {.X = M11.SmartElectricalTotalCycleEletricalEnergyGenerated * Signals.StoredEnergyEfficiency, .Y = M11.TotalCycleFuelConsumptionSmartElectricalLoad}

            Dim IP5x As Single = M11.TotalCycleElectricalDemand
            Dim IP5y As Single = 0

            Dim TanTeta As Single = (P2.Y - P1.Y) / (P2.X - P1.X)

            IP5y = P1.Y + (TanTeta * IP5x)

            _INTERP1 = IP5Y

            setPoints()

            Return If(Single.IsNaN(IP5Y), 0, IP5y)

        End Function
        Private Function Sum2() As Single

            Dim P1 As Point = New Point With {.X = 0, .Y = M11.TotalCycleFuelConsumptionZeroElectricalLoad}
            Dim P3 As Point = New Point With {.X = M11.StopStartSensitiveTotalCycleElectricalDemand, .Y = M10.AverageLoadsFuelConsumptionInterpolatedForPneumatics}

            Dim IP5x As Single = M11.TotalCycleElectricalDemand
            Dim IP5y As Single = 0

            Dim TanTeta As Single = (P3.Y - P1.Y) / (P3.X - P1.X)

            IP5y = P1.Y + (TanTeta * IP5x)

            _INTERP2 = IP5y

            Return If(Single.IsNaN(IP5Y), 0, IP5y)

        End Function

        'Constructor
        Public Sub New(m10 As IM10, m11 As IM11, signals As ISignals)

            Me.M10 = m10
            Me.M11 = m11
            Me.Signals = signals

        End Sub

        'Main Class Outputs
        Public ReadOnly Property FuelconsumptionwithsmartElectricsandAveragePneumaticPowerDemand As Single Implements IM12.FuelconsumptionwithsmartElectricsandAveragePneumaticPowerDemand
            Get

                'SCHM 3_2
                Dim interp1 As Single = Sum1

                interp1 = If(Not Single.IsNaN(interp1) AndAlso M11.StopStartSensitiveTotalCycleElectricalDemand > 0, interp1, M11.TotalCycleFuelConsumptionZeroElectricalLoad)
                Return interp1

            End Get

        End Property
        Public ReadOnly Property BaseFuelConsumptionWithTrueAuxiliaryLoads As Single Implements IM12.BaseFuelConsumptionWithTrueAuxiliaryLoads
            Get

                'SCM 3_02
                Dim interp2 As Single = Sum2()

                interp2 = If(Not Single.IsNaN(interp2) AndAlso M11.StopStartSensitiveTotalCycleElectricalDemand > 0, interp2, M10.AverageLoadsFuelConsumptionInterpolatedForPneumatics)
                Return interp2

            End Get
        End Property
        Public ReadOnly Property StopStartCorrection As Single Implements IM12.StopStartCorrection
            Get

                Return BaseFuelConsumptionWithTrueAuxiliaryLoads \ M10.AverageLoadsFuelConsumptionInterpolatedForPneumatics

            End Get
        End Property

        'Diagnostics Signal Exposure only. Does not materially affect class operation.
        Public ReadOnly Property INTRP1 As Single Implements IM12.INTRP1
            Get
                Return _INTERP1
            End Get
        End Property
        Public ReadOnly Property INTRP2 As Single Implements IM12.INTRP2
            Get
                Return _INTERP2
            End Get
        End Property
        Public ReadOnly Property P1X As Single Implements IM12.P1X
            Get
                Return _P1X
            End Get
        End Property
        Public ReadOnly Property P1Y As Single Implements IM12.P1Y
            Get
                Return _p1Y
            End Get
        End Property
        Public ReadOnly Property P2X As Single Implements IM12.P2X
            Get
                Return _P2X
            End Get
        End Property
        Public ReadOnly Property P2Y As Single Implements IM12.P2Y
            Get
                Return _P2Y
            End Get
        End Property
        Public ReadOnly Property P3X As Single Implements IM12.P3X
            Get
                Return _P3X
            End Get
        End Property
        Public ReadOnly Property P3Y As Single Implements IM12.P3Y
            Get
                Return _P3Y
            End Get
        End Property
        Public ReadOnly Property XTAIN As Single Implements IM12.XTAIN
            Get
                Return _XT
            End Get
        End Property

    End Class

End Namespace



