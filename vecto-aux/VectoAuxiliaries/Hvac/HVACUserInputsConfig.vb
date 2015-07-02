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
        Public Sub New(ssmFilePath As String, busDatabasePath As String, isDisabled As Boolean)

            Me.SSMFilePath = ssmFilePath
            Me.BusDatabasePath = busDatabasePath
            Me.SSMDisabled = isDisabled

        End Sub

        Public Property SSMFilePath As String Implements IHVACUserInputsConfig.SSMFilePath

        Public Property BusDatabasePath As String Implements IHVACUserInputsConfig.BusDatabasePath

        Public Property SSMDisabled As Boolean Implements IHVACUserInputsConfig.SSMDisabled

    End Class

End Namespace



