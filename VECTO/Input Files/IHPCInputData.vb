Imports System.IO
Imports TUGraz.VECTO.Input_Files
Imports TUGraz.VectoCommon.Exceptions
Imports TUGraz.VectoCommon.InputData
Imports TUGraz.VectoCommon.Models
Imports TUGraz.VectoCommon.Utils
Imports TUGraz.VectoCore.InputData.FileIO.JSON
Imports TUGraz.VectoCore.InputData.Impl
Imports TUGraz.VectoCore.OutputData.FileIO
Imports TUGraz.VectoCore.Utils

Public Class IHPCInputData
    Implements IElectricMotorEngineeringInputData

    Private _model As String
    Private _inertia As KilogramSquareMeter
    Private _voltageLevels As IList(Of IElectricMotorVoltageLevel)
    Private _dragCurve As TableData
    Private _overloadRecoveryFactor As Double
    private _filePath As String

    Public Sub New (file As String)
        _voltageLevels = New List(Of IElectricMotorVoltageLevel)()
        _filePath = file
    End Sub

    Public Function SaveFile() As Boolean
        Try
            Dim writer = New JSONFileWriter()
            writer.SaveIHPC(Me, _filePath, Cfg.DeclMode)
        Catch ex As Exception
            MsgBox("Failed to write IHPC file: " + ex.Message)
            Return False
        End Try
        Return True
    End Function


    Public Sub SetCommonEntries(model As String, inertia As String, dragCurveFilePath As string, 
                                thermalOverloadRecoverFactor As String )
        _model = model
        _inertia = inertia.ToDouble().SI(Of KilogramSquareMeter)
        Dim tmp as SubPath = new SubPath()
        tmp.Init(GetPath(_filePath), dragCurveFilePath)
        If Not File.Exists(tmp.FullPath) Then 
            Throw New VectoException("Drag Curve is missing or invalid")
        Else
            _dragCurve = VectoCSVFile.Read(tmp.FullPath)
        End If

        _overloadRecoveryFactor = thermalOverloadRecoverFactor.ToDouble()
    End Sub

    Public Sub SetVoltageLevelEntries(voltage As String, continuousTorque As String, continuousTorqueSpeed As String,
                                      overloadTime As String, overloadTorque As String, overloadTorqueSpeed As String, 
                                      fullLoadCurve As string, powerMap As ListView)


        Dim level = New ElectricMotorVoltageLevel()
        level.VoltageLevel = voltage.ToDouble().SI(Of Volt)
        level.ContinuousTorque = continuousTorque.ToDouble().SI(Of NewtonMeter)
        level.ContinuousTorqueSpeed = continuousTorqueSpeed.ToDouble().RPMtoRad()
        level.OverloadTime = overloadTime.ToDouble().SI(Of Second)
        level.OverloadTorque = overloadTorque.ToDouble().SI(Of NewtonMeter)
        level.OverloadTestSpeed = overloadTorqueSpeed.ToDouble().RPMtoRad()
        Dim tmp as SubPath = new SubPath()
        tmp.Init(GetPath(_filePath), fullLoadCurve)
        If Not File.Exists(tmp.FullPath) Then 
            Throw New VectoException("Full-Load Curve is missing or invalid")
        Else
            level.FullLoadCurve = New List(Of IElectricMotorLoadCurve) From {New ElectricMotorLoadCurve With {.LoadCurve = VectoCSVFile.Read(tmp.FullPath), .Gear = 0}}
        End If
        level.PowerMap = GetPowerMap(powerMap)
        
        _voltageLevels.Add(level)

    End Sub

    Private Function GetPowerMap(powerMap As ListView) As IList(of IElectricMotorPowerMap)
        Dim powerMaps = new List(Of IElectricMotorPowerMap)
        
        For Each entry As ListViewItem In powerMap.Items
            Dim currentEntry = New JSONElectricMotorPowerMap
            currentEntry.Gear = entry.SubItems(0).Text.ToInt()
            Dim tmp as SubPath = new SubPath()
            tmp.Init(GetPath(_filePath), entry.SubItems(1).Text)
            If Not File.Exists(tmp.FullPath) Then
                Throw New VectoException("Power Map is missing or invalid")
            Else 
                currentEntry.PowerMap = VectoCSVFile.Read(tmp.FullPath)
            End If
            powerMaps.Add(currentEntry)
        Next
        
        Return powerMaps
    End Function


    Public ReadOnly Property DataSource As DataSource Implements IComponentInputData.DataSource
        Get
            Dim retVal = New DataSource()
            retVal.SourceType = DataSourceType.JSONFile
            retVal.SourceFile = _filePath
            Return retVal
        End Get
    End Property

    Public ReadOnly Property SavedInDeclarationMode As Boolean Implements IComponentInputData.SavedInDeclarationMode
        Get
            Return Cfg.DeclMode
        End Get
    End Property

    Public ReadOnly Property Manufacturer As String Implements IComponentInputData.Manufacturer
        Get
            Return Nothing
        End Get
    End Property

    Public ReadOnly Property Model As String Implements IComponentInputData.Model
        Get
            Return _model
        End Get
    End Property

    Public ReadOnly Property [Date] As Date Implements IComponentInputData.[Date]
        Get
            Return Now.ToUniversalTime()
        End Get
    End Property

    Public ReadOnly Property AppVersion As String Implements IComponentInputData.AppVersion
        Get
            Return "VECTO-GUI"
        End Get
    End Property

    Public ReadOnly Property CertificationMethod As CertificationMethod Implements IComponentInputData.CertificationMethod
        Get
            Return CertificationMethod.NotCertified
        End Get
    End Property

    Public ReadOnly Property CertificationNumber As String Implements IComponentInputData.CertificationNumber
        Get
            Return VectoCore.Configuration.Constants.NOT_AVAILABLE
        End Get
    End Property

    Public ReadOnly Property DigestValue As DigestData Implements IComponentInputData.DigestValue
        Get
            Return Nothing
        End Get
    End Property

    Public ReadOnly Property ElectricMachineType As ElectricMachineType Implements IElectricMotorDeclarationInputData.ElectricMachineType
        Get
            Return Nothing
        End Get
    End Property

    Public Property R85RatedPower As Watt Implements IElectricMotorDeclarationInputData.R85RatedPower

    Public ReadOnly Property Inertia As KilogramSquareMeter Implements IElectricMotorDeclarationInputData.Inertia
        Get
            Return _inertia
        End Get
    End Property

    Public ReadOnly Property DcDcConverterIncluded As Boolean Implements IElectricMotorDeclarationInputData.DcDcConverterIncluded

    Public ReadOnly Property IHPCType As String Implements IElectricMotorDeclarationInputData.IHPCType

    Public ReadOnly Property VoltageLevels As IList(Of IElectricMotorVoltageLevel) Implements IElectricMotorDeclarationInputData.VoltageLevels
        Get
            Return _voltageLevels
        End Get
    End Property

    Public ReadOnly Property DragCurve As TableData Implements IElectricMotorDeclarationInputData.DragCurve
        Get
            Return _dragCurve
        End Get
    End Property

    Public ReadOnly Property Conditioning As TableData Implements IElectricMotorDeclarationInputData.Conditioning
        Get
            Return Nothing
        End Get
    End Property

    Public ReadOnly Property OverloadRecoveryFactor As Double Implements IElectricMotorEngineeringInputData.OverloadRecoveryFactor
        Get
            Return _overloadRecoveryFactor
        End Get
    End Property
End Class
