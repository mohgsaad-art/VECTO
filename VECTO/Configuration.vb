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
Imports TUGraz.VectoCore.OutputData.FileIO
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
	Public Multithreaded As Boolean

	Public ValidateRunData As Boolean
	Public SaveVectoRunData As Boolean

    public OutputFolder As String


	Public Const DefaultFuelType As FuelType = FuelType.DieselCI

	Private Const FormatVersion As Short = 2

	'Test Settings 2nd amendment
    Private _body as String = "Body"
    Private _mod1Hz as String = "Mod1Hz"
    Private _modOut as String = "ModOut"
    Private _logSize as String = "LogSize"
    Private _airdensity as String = "AirDensity"
    Private _fueldensity as String = "FuelDensity"
    Private _co2Perfc as String = "CO2perFC"
    Private _openCmd as String = "OpenCmd"
    Private _opencmdname as String = "OpenCmdName"
    Private _firstrun as String = "FirstRun"
    Private _declmode as String = "DeclMode"
    Private _validaterundata as String = "ValidateRunData"
    Private _outputfolder as String = "OutputFolder"
    Private _saverundata as String = "SaveRunData"
    Private _overrideinitialsoc as String = "OverrideInitialSOC"
    Private _overrideinitialsocvalue as String = "OverrideInitialSOCValue"
    Private _csItActive as String = "CS_it_deactivated"

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
		Multithreaded = True
		SaveVectoRunData = False
	End Sub

	Public Sub Load()
		SetDefault()
	    If Environment.GetCommandLineArgs().Contains("-st") Then 
			Multithreaded = False
	    End If

		If Not File.Exists(FilePath) Then
			Exit Sub
		End If

		Try
			Using reader As TextReader = File.OpenText(FilePath)
				Dim content As JToken = JToken.ReadFrom(New JsonTextReader(reader))

				Dim body As JToken = content.GetEx(_body)
				Try
					Mod1Hz = body.GetEx(Of Boolean)(_mod1Hz)
				Catch
				End Try
				ModOut = body.GetEx(Of Boolean)(_modOut)
				LogSize = body.GetEx(Of Double)(_logSize)
				AirDensity = body.GetEx(Of Double)(_airdensity)
				FuelDens = body.GetEx(Of Double)(_fueldensity)
				Co2PerFc = body.GetEx(Of Double)(_co2Perfc)
				OpenCmd = body.GetEx(Of String)(_openCmd)
				OpenCmdName = body.GetEx(Of String)(_opencmdname)
				FirstRun = body.GetEx(Of Boolean)(_firstrun)
				DeclMode = body.GetEx(Of Boolean)(_declmode)
				ValidateRunData = IsNothing(body(_validaterundata)) OrElse body.GetEx(Of Boolean)(_validaterundata)
                OutputFolder = If(body(_outputfolder) Is Nothing, "", body(_outputfolder).Value(of string)())
				SaveVectoRunData = If(body(_saverundata) Is Nothing, False, body.GetEx(Of Boolean)(_saverundata))

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
		body.Add(_modOut, ModOut)
		body.Add(_mod1Hz, Mod1Hz)
		body.Add(_logSize, LogSize)
		body.Add(_airdensity, AirDensity)
		body.Add(_fueldensity, FuelDens)
		body.Add(_co2Perfc, Co2PerFc)
		body.Add(_openCMD, OpenCmd)
		body.Add(_opencmdname, OpenCmdName)
		body.Add(_firstrun, FirstRun)
		body.Add(_declMode, DeclMode)
		body.Add(_validaterundata, ValidateRunData)
        body.Add(_outputfolder, OutputFolder)
		body.Add(_saverundata, SaveVectoRunData)

		JSONFileWriter.WriteFile(New Dictionary(Of String, Object) From {{"Header", header}, {_body, body}}, FilePath)
	End Sub
End Class

