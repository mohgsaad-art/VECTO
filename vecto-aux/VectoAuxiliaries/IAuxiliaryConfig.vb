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
