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

Imports VectoAuxiliaries.Hvac
Imports VectoAuxiliaries.Electrics
Imports VectoAuxiliaries.Pneumatics

Namespace Hvac

Public Class HVACUserInputsConfig
Implements IHVACUserInputsConfig

    'Constructor
    Public Sub new (ssm As IHVACSteadyStateModel, ssmFilePath As string)

           SteadyStateModel           = ssm
           Me.SSMFilePath             = ssmFilePath

        End Sub

    'Implementation
    Public Property SteadyStateModel As IHVACSteadyStateModel Implements IHVACUserInputsConfig.SteadyStateModel
    Public Property SSMFilePath As String Implements IHVACUserInputsConfig.SSMFilePath

End Class

End Namespace



