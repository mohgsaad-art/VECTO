Imports VectoAuxiliaries.Electrics
Imports VectoAuxiliaries.Pneumatics
Imports VectoAuxiliaries.Hvac
Imports VectoAuxiliaries.DownstreamModules
Imports System.IO

Imports System.Windows.Forms
Imports Newtonsoft.Json


Public Interface IAuxiliaryConfig


 'Vecto
  Property VectoInputs As IVectoInputs
  
 'Electrical
  property ElectricalUserInputsConfig As IElectricsUserInputsConfig


 'Pneumatics
  Property PneumaticUserInputsConfig As IPneumaticUserInputsConfig
  Property PneumaticAuxillariesConfig As IPneumaticsAuxilliariesConfig

 'Hvac
  Property  HvacUserInputsConfig As IHVACUserInputsConfig

  Function ConfigValuesAreTheSameAs( other As AuxiliaryConfig) As Boolean


 'Persistance Functions
  Function Save(  filePath As String ) As Boolean
  Function Load(  filePath As String  ) As Boolean



End Interface
