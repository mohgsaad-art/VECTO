' Copyright 2017 European Union.
' Licensed under the EUPL (the 'Licence');
'
' * You may not use this work except in compliance with the Licence.
' * You may obtain a copy of the Licence at: http://ec.europa.eu/idabc/eupl
' * Unless required by applicable law or agreed to in writing,
'   software distributed under the Licence is distributed on an "AS IS" basis,
'   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
'
' See the LICENSE.txt for the specific language governing permissions and limitations.
Imports System.Collections.Generic
Imports System.IO
Imports TUGraz.VectoCommon.Models
Imports TUGraz.VectoCommon.Utils
Imports TUGraz.VectoCore.InputData.Reader.DataObjectAdapter
Imports TUGraz.VectoCore.Models.SimulationComponent.Data
Imports TUGraz.VectoCore.Models.SimulationComponent.Data.ElectricMotor
Imports TUGraz.VectoCore.Models.SimulationComponent.Data.Engine
Imports TUGraz.VectoCore.OutputData.FileIO

''' <summary>
''' Main calculation routines.
''' </summary>
''' <remarks></remarks>
Module MainModule
	Public JobFileList As List(Of String)

	Public Function ConvertToEngineData(ByVal fld As EngineFullLoadCurve, ByVal nIdle As PerSecond, ByVal gear As Integer,
										ByVal maxTorque As NewtonMeter) As CombustionEngineData

		Dim retVal As CombustionEngineData = New CombustionEngineData()
		Dim fullLoadCurves As Dictionary(Of UInteger, EngineFullLoadCurve) = New Dictionary(Of UInteger, EngineFullLoadCurve)
		fullLoadCurves(0) = fld
		fullLoadCurves(CType(gear, UInteger)) = AbstractSimulationDataAdapter.IntersectFullLoadCurves(fld, maxTorque)
		retVal.FullLoadCurves = fullLoadCurves
		retVal.IdleSpeed = nIdle
		Return retVal
	End Function

    public Function ConvertToElectricMotorData(emFld As ElectricMotorFullLoadCurve, gear As Integer) As ElectricMotorData
        Dim retval = new ElectricMotorData

        retval.EfficiencyData = New VoltageLevelData()
        retval.EfficiencyData.VoltageLevels = new List(Of ElectricMotorVoltageLevelData)

        Dim vl = new ElectricMotorVoltageLevelData
        retval.EfficiencyData.VoltageLevels.Add(vl)

        vl.FullLoadCurve = emFld

        Return retval
    End Function

    Public Function ConvPicPath(hdVclass As VehicleClass, isLongHaul As Boolean) As Bitmap

        Select Case hdVclass
            Case VehicleClass.Class51,
                 VehicleClass.Class53,
                 VehicleClass.Class55,
                 VehicleClass.Class1s
                Return My.Resources.Undef

            Case VehicleClass.Class1,
                 VehicleClass.Class2,
                 VehicleClass.Class3,
                VehicleClass.Class6,
                VehicleClass.Class7
                Return My.Resources._4x2r
            Case VehicleClass.Class4
                If isLongHaul Then Return My.Resources._4x2rt
                Return My.Resources._4x2r
            Case VehicleClass.Class5,
                 VehicleClass.Class8
                Return My.Resources._4x2tt
            Case VehicleClass.Class9,
                 VehicleClass.Class11,
                 VehicleClass.Class13
                If isLongHaul Then Return My.Resources._6x2rt
                Return My.Resources._6x2r
            Case VehicleClass.Class10,
                 VehicleClass.Class12,
                 VehicleClass.Class14
                Return My.Resources._6x2tt

            Case VehicleClass.Class16
                Return My.Resources.rigid8x4

            Case VehicleClass.Class52,
                 VehicleClass.Class54,
                 VehicleClass.Class56
                Return My.Resources.van

            Case VehicleClass.Class31a,
                 VehicleClass.Class31b1,
                 VehicleClass.Class31b2,
                 VehicleClass.Class31c,
                 VehicleClass.Class31d,
                 VehicleClass.Class31e,
                 VehicleClass.Class32a,
                 VehicleClass.Class32b,
                 VehicleClass.Class32c,
                 VehicleClass.Class32d,
                 VehicleClass.Class32e,
                 VehicleClass.Class32f,
                 VehicleClass.ClassP31_32
                Return My.Resources.bus4x2

            Case VehicleClass.Class33a,
                 VehicleClass.Class33b1,
                 VehicleClass.Class33b2,
                 VehicleClass.Class33c,
                 VehicleClass.Class33d,
                 VehicleClass.Class33e,
                 VehicleClass.Class34a,
                 VehicleClass.Class34b,
                 VehicleClass.Class34c,
                 VehicleClass.Class34d,
                 VehicleClass.Class34e,
                 VehicleClass.Class34f,
                 VehicleClass.ClassP33_34
                Return My.Resources.bus6x2

            Case VehicleClass.Class37a,
                 VehicleClass.Class37b1,
                 VehicleClass.Class37b2,
                 VehicleClass.Class37c,
                 VehicleClass.Class37d,
                 VehicleClass.Class37e,
                 VehicleClass.Class38a,
                 VehicleClass.Class38b,
                 VehicleClass.Class38c,
                 VehicleClass.Class38d,
                 VehicleClass.Class38e,
                 VehicleClass.Class38f,
                 VehicleClass.ClassP37_38
                Return My.Resources.bus8x2

            Case Else
                Return My.Resources.Undef
        End Select
    End Function

    Public Function GetRelativePath(filePath As String, basePath As String) As String
        Return JSONFileWriter.GetRelativePath(filePath, basePath)
		'If (String.IsNullOrEmpty(filePath)) then
		'	Return ""
		'End If
  '      If (string.isnullOrempty(basePath)) Then
  '          Return filePath
  '      End If
		'If (Path.GetDirectoryName(Path.GetFullPath(filePath)).StartsWith(basePath, StringComparison.OrdinalIgnoreCase)) Then
		'	Return Path.GetFullPath(filePath).Substring(basePath.Length + If(basePath.EndsWith("\"), 0, 1))
		'End If
		'Return filePath
	End Function
End Module
