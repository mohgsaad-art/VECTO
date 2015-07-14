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

Imports System.Diagnostics.CodeAnalysis

Namespace Electrics

    Public Class M0_5_SmartAlternatorSetEfficiency
        Implements IM0_5_SmartAlternatorSetEfficiency

        'Fields
        Private _m0 As IM0_NonSmart_AlternatorsSetEfficiency
        Private _electricalConsumables As IElectricalConsumerList
        Private _alternatorMap As IAlternatorMap
        Private _resultCardIdle As IResultCard
        Private _resultCardTraction As IResultCard
        Private _resultCardOverrun As IResultCard
        Private _signals As ISignals

        'Constructor
        Public Sub New(m0 As IM0_NonSmart_AlternatorsSetEfficiency, _
                   electricalConsumables As IElectricalConsumerList, _
                   alternatorMap As IAlternatorMap, _
                   resultCardIdle As IResultCard, _
                   resultCardTraction As IResultCard, _
                   resultCardOverrun As IResultCard,
                   signals As ISignals)

            'Sanity Check on supplied arguments, throw an argument exception
            If m0 Is Nothing Then Throw New ArgumentException("Module 0 must be supplied")
            If electricalConsumables Is Nothing Then Throw New ArgumentException("ElectricalConsumablesList must be supplied even if empty")
            If alternatorMap Is Nothing Then Throw New ArgumentException("Must supply a valid alternator map")
            If resultCardIdle Is Nothing Then Throw New ArgumentException("Result Card 'IDLE' must be supplied even if it has no contents")
            If resultCardTraction Is Nothing Then Throw New ArgumentException("Result Card 'TRACTION' must be supplied even if it has no contents")
            If resultCardOverrun Is Nothing Then Throw New ArgumentException("Result Card 'OVERRUN' must be supplied even if it has no contents")
            If signals Is Nothing Then Throw New ArgumentException("No Signals Reference object was provided ")

            'Assignments to private variables.
            _m0 = m0
            _electricalConsumables = electricalConsumables
            _alternatorMap = alternatorMap
            _resultCardIdle = resultCardIdle
            _resultCardTraction = resultCardTraction
            _resultCardOverrun = resultCardOverrun
            _signals = signals


        End Sub


        'Public Class Outputs (Properties)
        Public ReadOnly Property SmartIdleCurrent() As Single Implements IM0_5_SmartAlternatorSetEfficiency.SmartIdleCurrent
            Get
                Dim hvac_Plus_None_Base As Single = HvacPlusNonBaseCurrents()
                Dim smart_idle_current As Single = _resultCardIdle.GetSmartCurrentResult(hvac_Plus_None_Base)

                Return smart_idle_current
            End Get
        End Property
        Public ReadOnly Property AlternatorsEfficiencyIdleResultCard() As Single Implements IM0_5_SmartAlternatorSetEfficiency.AlternatorsEfficiencyIdleResultCard
            Get
                Return _alternatorMap.GetEfficiency(_signals.EngineSpeed, SmartIdleCurrent()).Efficiency
            End Get
        End Property
        Public ReadOnly Property SmartTractionCurrent As Single Implements IM0_5_SmartAlternatorSetEfficiency.SmartTractionCurrent
            Get
                Return _resultCardTraction.GetSmartCurrentResult(HvacPlusNonBaseCurrents())
            End Get
        End Property
        Public ReadOnly Property AlternatorsEfficiencyTractionOnResultCard() As Single Implements IM0_5_SmartAlternatorSetEfficiency.AlternatorsEfficiencyTractionOnResultCard
            Get
                Return _alternatorMap.GetEfficiency(_signals.EngineSpeed, SmartTractionCurrent()).Efficiency
            End Get
        End Property
        Public ReadOnly Property SmartOverrunCurrent As Single Implements IM0_5_SmartAlternatorSetEfficiency.SmartOverrunCurrent
            Get
                Return _resultCardOverrun.GetSmartCurrentResult(HvacPlusNonBaseCurrents())
            End Get
        End Property
        Public ReadOnly Property AlternatorsEfficiencyOverrunResultCard() As Single Implements IM0_5_SmartAlternatorSetEfficiency.AlternatorsEfficiencyOverrunResultCard
            Get
                Return _alternatorMap.GetEfficiency(_signals.EngineSpeed, SmartOverrunCurrent()).Efficiency
            End Get
        End Property

        'Helpers
        ''' <summary>
        ''' Returns Non Base Currents (A)
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <ExcludeFromCodeCoverage>
        Private Function HvacPlusNonBaseCurrents() As Single
            'Stored Energy Efficience removed from V8.0 21/4/15 by Mike Preston  //tb
            Return _m0.GetHVACElectricalPowerDemandAmps() + _electricalConsumables.GetTotalAverageDemandAmps(True)  '/ElectricConstants.StoredEnergyEfficiency)

        End Function


    End Class

End Namespace


