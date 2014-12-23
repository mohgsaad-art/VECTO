

Public Interface IAdvancedAuxiliaries
 Inherits IAuxiliaryEvent


'Information
ReadOnly Property Running          As boolean

ReadOnly Property AuxiliaryName    As String
ReadOnly Property AuxiliaryVersion As String

ReadOnly Property AA_NonSmartAlternatorsEfficiency As single?
Readonly Property AA_SmartIdleCurrent_Amps As single?
Readonly Property AA_SmartIdleAlternatorsEfficiency As single?
ReadOnly Property AA_SmartTractionCurrent_Amps As single?
ReadOnly Property AA_SmartTractionAlternatorEfficiency As single?
ReadOnly Property AA_SmartOverrunCurrent_Amps As Single?
Readonly Property AA_SmartOverrunAlternatorEfficiency As Single?
Readonly Property AA_CompressorFlowRate_LitrePerSec As Single?
ReadOnly Property AA_OverrunFlag As integer?
ReadOnly Property AA_EngineIdleFlag As integer?
ReadOnly Property AA_CompressorFlag As integer?
Readonly Property AA_TotalCycleFC_BeforeSSandWHTCcorrection_Grams As single?
ReadOnly Property AA_TotalCycleFC_BeforeSSandWHTCcorrection_Litres As single?

Readonly Property TotalFuelGRAMS   As Single
Readonly Property TotalFuelLITRES  As single


'Static Values
property VectoInputs As IVectoInputs

'Running Properties
property Signals as ISignals

'Configuration
Function Configure( filePath As String, vectoFilePath As string ) As Boolean
Function ValidateAAUXFile( ByVal filePath As String , byref message As String ) As Boolean

'Command
Function  CycleStep( seconds As Integer, ByRef message As string ) As boolean
Function  RunStart( ByVal auxFilePath As String, ByRef message As string ) As boolean
Function  RunStop( ByRef message As string ) As boolean



End Interface


