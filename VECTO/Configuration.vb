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
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq
Imports TUGraz.VectoCommon.Models
Imports TUGraz.VectoCore.InputData.FileIO.JSON
Imports TUGraz.VectoCore.Models.Declaration
Imports TUGraz.VectoCore.Utils

Public Class Configuration
	Public FilePath As String
	Public ModOut As Boolean
	Public Mod1Hz As Boolean
	Public LogSize As Double
	Public AirDensity As Double
	Public OpenCmd As String
	Public OpenCmdName As String
	Public FuelDens As Double
	Public Co2PerFc As Double
	Public FirstRun As Boolean
	Public DeclMode As Boolean

	Public ValidateRunData As Boolean

    public OutputFolder As String

	Public Const DefaultFuelType As FuelType = FuelType.DieselCI

	Private Const FormatVersion As Short = 2

	Public Sub New()
		SetDefault()
	End Sub

	Public Sub DeclInit()
		AirDensity = Physics.AirDensity.Value()	' cDeclaration.AirDensity
		FuelDens = DeclarationData.FuelData.Lookup(DefaultFuelType).FuelDensity.Value()	' cDeclaration.FuelDens
		Co2PerFc = DeclarationData.FuelData.Lookup(DefaultFuelType).CO2PerFuelWeight		' cDeclaration.CO2perFC
	End Sub

	Public Sub SetDefault()
		ModOut = True
		Mod1Hz = False
		LogSize = 2
		AirDensity = 1.2
		OpenCmd = "notepad"
		OpenCmdName = "Notepad"
		FuelDens = DeclarationData.FuelData.Lookup(DefaultFuelType).FuelDensity.Value()
		Co2PerFc = DeclarationData.FuelData.Lookup(DefaultFuelType).CO2PerFuelWeight
		FirstRun = True
		DeclMode = True
		ValidateRunData = True
        OutputFolder = ""
	End Sub

	Public Sub Load()
		SetDefault()

		If Not File.Exists(FilePath) Then
			Exit Sub
		End If

		Try
			Using reader As TextReader = File.OpenText(FilePath)
				Dim content As JToken = JToken.ReadFrom(New JsonTextReader(reader))

				Dim body As JToken = content.GetEx("Body")
				Try
					Mod1Hz = body.GetEx(Of Boolean)("Mod1Hz")
				Catch
				End Try
				ModOut = body.GetEx(Of Boolean)("ModOut")
				LogSize = body.GetEx(Of Double)("LogSize")
				AirDensity = body.GetEx(Of Double)("AirDensity")
				FuelDens = body.GetEx(Of Double)("FuelDensity")
				Co2PerFc = body.GetEx(Of Double)("CO2perFC")
				OpenCmd = body.GetEx(Of String)("OpenCmd")
				OpenCmdName = body.GetEx(Of String)("OpenCmdName")
				FirstRun = body.GetEx(Of Boolean)("FirstRun")
				DeclMode = body.GetEx(Of Boolean)("DeclMode")
				ValidateRunData = IsNothing(body("ValidateRunData")) OrElse body.GetEx(Of Boolean)("ValidateRunData")
                OutputFolder = If(body("OutputFolder") Is Nothing, "", body("OutputFolder").Value(of string)())
			End Using
		Catch ex As Exception
			GUIMsg(MessageType.Err, "Error while loading settings!")
		End Try
	End Sub

	Public Sub Save()

		Dim header As Dictionary(Of String, Object) = New Dictionary(Of String, Object)
		header.Add("Date", Now.ToUniversalTime().ToString("o"))
		header.Add("AppVersion", VECTOvers)
		header.Add("FileVersion", FormatVersion)

		Dim body As Dictionary(Of String, Object) = New Dictionary(Of String, Object)
		body.Add("ModOut", ModOut)
		body.Add("Mod1Hz", Mod1Hz)
		body.Add("LogSize", LogSize)
		body.Add("AirDensity", AirDensity)
		body.Add("FuelDensity", FuelDens)
		body.Add("CO2perFC", Co2PerFc)
		body.Add("OpenCmd", OpenCmd)
		body.Add("OpenCmdName", OpenCmdName)
		body.Add("FirstRun", FirstRun)
		body.Add("DeclMode", DeclMode)
		body.Add("ValidateRunData", ValidateRunData)
        body.Add("OutputFolder", OutputFolder)

		JSONFileWriter.WriteFile(New Dictionary(Of String, Object) From {{"Header", header}, {"Body", body}}, FilePath)
	End Sub
End Class

