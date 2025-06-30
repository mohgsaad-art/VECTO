Imports System.Collections.Generic
Imports System.ComponentModel.DataAnnotations
Imports System.IO
Imports System.Linq
Imports TUGraz.VectoCommon.InputData
Imports TUGraz.VectoCommon.Models
Imports TUGraz.VectoCommon.Utils
Imports TUGraz.VectoCore.OutputData.FileIO

Public Class HybridStrategyParams
    Implements IHybridStrategyParameters

    Private _filePath As String
    Private _myPath As String
    Public MinimumIceOnTime As Double

    Public Sub New()
        _myPath = ""
        _filePath = ""




        SetDefault()
    End Sub

    Private Sub SetDefault()

    End Sub

    Public Function SaveFile() As Boolean

        Dim validationResults As IList(Of ValidationResult) =
                Validate(If(Cfg.DeclMode, ExecutionMode.Declaration, ExecutionMode.Engineering), VectoSimulationJobType.ParallelHybridVehicle, Nothing, Nothing, False)

        If validationResults.Count > 0 Then
            Dim messages As IEnumerable(Of String) =
                    validationResults.Select(Function(r) r.ErrorMessage + String.Join(", ", r.MemberNames.Distinct()))
            MsgBox("Invalid input." + Environment.NewLine + String.Join(Environment.NewLine, messages), MsgBoxStyle.OkOnly,
                   "Failed to save strategy parameters")
            Return False
        End If

        Try
            Dim writer = New JSONFileWriter()
            writer.SaveStrategyParameters(Me, _filePath, Cfg.DeclMode)

        Catch ex As Exception
            MsgBox("Failed to write Strategy Parameters file: " + ex.Message)
            Return False
        End Try
        Return True
    End Function

    Public Property FilePath() As String
        Get
            Return _filePath
        End Get
        Set(ByVal value As String)
            _filePath = value
            If _filePath = "" Then
                _myPath = ""
            Else
                _myPath = Path.GetDirectoryName(_filePath) & "\"
            End If
        End Set
    End Property


    Public Property EquivalenceFactorCharge As Double Implements IHybridStrategyParameters.EquivalenceFactorCharge
    

    Public  Property EquivalenceFactorDischarge As Double Implements IHybridStrategyParameters.EquivalenceFactorDischarge
        

    Public Property MinSoC As Double

    Public Property MaxSoC As Double

    Public Property TargetSoC As Double

    Public ReadOnly Property Source As String Implements IHybridStrategyParameters.Source
        Get
            Return FilePath
        End Get
    End Property

    Public ReadOnly Property MinICEOnTime As Second Implements IHybridStrategyParameters.MinimumICEOnTime
        Get
            Return MinimumICEOnTime.SI(of Second)
        End Get
    End Property

    Public ReadOnly Property AuxBufferTime As Second Implements IHybridStrategyParameters.AuxBufferTime
        Get
            Return AuxiliaryBufferTime.SI(Of Second)
        End Get
    End Property

    Public ReadOnly Property AuxBufferChargeTime As Second Implements IHybridStrategyParameters.AuxBufferChargeTime
        Get
            Return AuxiliaryBufferChgTime.SI(of Second)
        End Get
    End Property

    Public Property ICEStartPenaltyFactor As Double Implements IHybridStrategyParameters.ICEStartPenaltyFactor
    Public Property CostFactorSOCExpponent As Double Implements IHybridStrategyParameters.CostFactorSOCExpponent
    Public Property GensetMinOptPowerFactor As Double Implements IHybridStrategyParameters.GensetMinOptPowerFactor
       

    Public Property AuxiliaryBufferTime As Double

    Public Property AuxiliaryBufferChgTime As Double



    Private ReadOnly Property IHybridStrategyParameters_MinSoC As Double Implements IHybridStrategyParameters.MinSoC
        Get
            Return MinSoC
        End Get
    End Property

    Private ReadOnly Property IHybridStrategyParameters_MaxSoC As Double Implements IHybridStrategyParameters.MaxSoC
        Get
            Return MaxSoC
        End Get
    End Property

    Private ReadOnly Property IHybridStrategyParameters_TargetSoC As Double Implements IHybridStrategyParameters.TargetSoC
        Get
            Return TargetSoC
        End Get
    End Property
End Class