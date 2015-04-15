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

Public Interface IAdvancedAuxiliaries
' Inherits IAuxiliaryEvent



  Event AuxiliaryEvent( ByRef sender As Object, byval message As String, ByVal messageType As AdvancedAuxiliaryMessageType )


  'Information
  ReadOnly Property Running          As boolean
  
  ReadOnly Property AuxiliaryName    As String
  ReadOnly Property AuxiliaryVersion As String
  

  'Diagnostic Only - Remove when beta over.
  Readonly Property AA_D_M12_P1X as single
  Readonly Property AA_D_M12_P1Y as single
  Readonly Property AA_D_M12_P2X as single
  Readonly Property AA_D_M12_P2Y as single
  Readonly Property AA_D_M12_P3X as single
  Readonly Property AA_D_M12_P3Y as single
  Readonly Property AA_D_M12_XTAIN as single
  Readonly Property AA_D_M12_INTERP1 as single
  Readonly Property AA_D_M12_INTERP2 as single

  'Additional Permenent Monitoring Signals - Required by engineering
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
  Readonly Property AA_TotalCycleFC_Grams As single?
  ReadOnly Property AA_TotalCycleFC_Litres As single?
  
  ''' <summary>
  ''' Total Cycle Fuel In Grams
  ''' </summary>
  ''' <value></value>
  ''' <returns></returns>
  ''' <remarks></remarks>
  Readonly Property TotalFuelGRAMS   As Single
  ''' <summary>
  ''' Total Cycle Fuel in Litres
  ''' </summary>
  ''' <value></value>
  ''' <returns></returns>
  ''' <remarks></remarks>
  Readonly Property TotalFuelLITRES  As single
  ''' <summary>
  ''' Total Power Demans At Crank From Auxuliaries (W)
  ''' </summary>
  ''' <value></value>
  ''' <returns></returns>
  ''' <remarks></remarks>
  ReadOnly Property AuxiliaryPowerAtCrankWatts As single



  ''' <summary>
  ''' Vecto Inputs
  ''' </summary>
  ''' <value></value>
  ''' <returns></returns>
  ''' <remarks></remarks>
  property VectoInputs As IVectoInputs
  
  ''' <summary>
  ''' Signals From Vecto 
  ''' </summary>
  ''' <value></value>
  ''' <returns></returns>
  ''' <remarks></remarks>
  property Signals as ISignals
  
  ''' <summary>
  ''' Configure Auxuliaries ( Launches Config Form )
  ''' </summary>
  ''' <param name="filePath"></param>
  ''' <param name="vectoFilePath"></param>
  ''' <returns></returns>
  ''' <remarks></remarks>
  Function Configure( filePath As String, vectoFilePath As string ) As Boolean
  ''' <summary>
  ''' Validate AAUX file path supplied.
  ''' </summary>
  ''' <param name="filePath"></param>
  ''' <param name="message"></param>
  ''' <returns></returns>
  ''' <remarks></remarks>
  Function ValidateAAUXFile( ByVal filePath As String , byref message As String ) As Boolean
  
  'Command
  ''' <summary>
  ''' Cycle Step - Used to calculate fuelling
  ''' </summary>
  ''' <param name="seconds"></param>
  ''' <param name="message"></param>
  ''' <returns></returns>
  ''' <remarks></remarks>
  Function  CycleStep( seconds As Integer, ByRef message As string ) As boolean
  ''' <summary>
  ''' Initialises AAUX Environment ( Begin Processs )
  ''' </summary>
  ''' <param name="auxFilePath"></param>
  ''' <param name="vectoFilePath"></param>
  ''' <param name="message"></param>
  ''' <returns></returns>
  ''' <remarks></remarks>
  Function  RunStart( ByVal auxFilePath As String,byval vectoFilePath as string ,ByRef message As string ) As boolean
  ''' <summary>
  ''' Any Termination Which Needs to be done ( Model depenent )
  ''' </summary>
  ''' <param name="message"></param>
  ''' <returns></returns>
  ''' <remarks></remarks>
  Function  RunStop( ByRef message As string ) As boolean



End Interface


