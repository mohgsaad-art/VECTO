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

Namespace Hvac

   Public Class HVACConstants
    Implements  IHVACConstants

        Private _fuelDensity As Single

        Public Sub New()
            _fuelDensity = 0.832
        End Sub

        Public Sub New(fuelDensitySingle As Single)
            _fuelDensity = fuelDensitySingle
        End Sub

        Public ReadOnly Property DieselGCVJperGram As Single Implements IHVACConstants.DieselGCVJperGram
            Get
                Return 44800
            End Get
        End Property

        Public ReadOnly Property FuelDensity As Single Implements IHVACConstants.FuelDensity
            Get
                Return _fuelDensity
            End Get
        End Property

    End Class

End Namespace



