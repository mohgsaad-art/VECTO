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

    Public Class M13
        Implements IM13

        Private m10 As IM10
        Private m11 As IM11
        Private m12 As IM12
        Private signals As ISignals

        'Internal Staging Calculations

        Private ReadOnly Property Sum1 As Single
            Get
                Return m11.TotalCycleFuelConsuptionAverageLoads * m12.StopStartCorrection
            End Get
        End Property
        Private ReadOnly Property Sum2 As Single
            Get
                Return m10.AverageLoadsFuelConsumptionInterpolatedForPneumatics * m12.StopStartCorrection
            End Get
        End Property
        Private ReadOnly Property Sum3 As Single
            Get
                Return m10.FuelConsumptionSmartPneumaticsAndAverageElectricalPowerDemand * m12.StopStartCorrection
            End Get
        End Property
        Private ReadOnly Property Sum4 As Single
            Get
                Return -m12.FuelconsumptionwithsmartElectricsandAveragePneumaticPowerDemand + Sum1
            End Get
        End Property
        Private ReadOnly Property Sum5 As Single
            Get
                Return Sum2 - Sum3
            End Get
        End Property
        Private ReadOnly Property Sum6 As Single
            Get
                Return m12.BaseFuelConsumptionWithTrueAuxiliaryLoads - Sum4
            End Get
        End Property
        Private ReadOnly Property Sum7 As Single
            Get
                Return m12.BaseFuelConsumptionWithTrueAuxiliaryLoads - Sum5
            End Get
        End Property
        Private ReadOnly Property Sum8 As Single
            Get
                Return -Sum4 + Sum7
            End Get
        End Property
        Private ReadOnly Property Sum9 As Single
            Get
                Return SW4 * SW3
            End Get
        End Property

        'Internal Staging Switches
        Private ReadOnly Property SW1 As Single
            Get
                Return If(signals.SmartPneumatics, Sum8, Sum6)
            End Get
        End Property
        Private ReadOnly Property SW2 As Single
            Get
                Return If(signals.SmartPneumatics, Sum3, m12.BaseFuelConsumptionWithTrueAuxiliaryLoads)
            End Get
        End Property
        Private ReadOnly Property SW3 As Single
            Get
                Return If(signals.SmartElectrics, SW1, SW2)
            End Get
        End Property
        Private ReadOnly Property SW4 As Single
            Get
                Return If(signals.DeclarationMode, signals.WHTC, 1)
            End Get
        End Property

        'Constructor
        Public Sub New(m10 As IM10, m11 As IM11, m12 As IM12, signals As ISignals)

            Me.m10 = m10
            Me.m11 = m11
            Me.m12 = m12
            Me.signals = signals

        End Sub

        'Public class outputs
        Public ReadOnly Property WHTCTotalCycleFuelConsumptionGrams As Single Implements IM13.WHTCTotalCycleFuelConsumptionGrams
            Get
                Return Sum9
            End Get
        End Property


    End Class

End Namespace



