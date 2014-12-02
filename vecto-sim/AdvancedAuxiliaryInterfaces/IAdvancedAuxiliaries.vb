

Public Interface IAdvancedAuxiliaries
 Inherits IAuxiliaryEvent


'Information
ReadOnly Property Running          As boolean
Readonly Property TotalFuelGRAMS   As Single
Readonly Property TotalFuelLITRES  As single
ReadOnly Property AuxiliaryName    As String
ReadOnly Property AuxiliaryVersion As String


'Static Values
property VectoInputs As IVectoInputs

'Running Properties
property Signals as ISignals

'Configuration
Function Configure( filePath As String, vectoFilePath As string ) As Boolean

'Command
Function  CycleStep( seconds As Integer, ByRef message As string ) As boolean
Function  RunStart( ByVal auxFilePath As String, ByRef message As string ) As boolean
Function  RunStop( ByRef message As string ) As boolean



End Interface


