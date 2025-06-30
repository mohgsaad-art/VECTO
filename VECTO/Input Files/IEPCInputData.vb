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

Public Class IEPCInputData 
    Implements IIEPCEngineeringInputData
    
    Private _model As String
    Private _inertia As KilogramSquareMeter
    Private _wheelMotorMeasured As Boolean
    Private _disengagementClutch As Boolean
    Private _nrDesignTypeWheelMotor As Integer?
    Private _differentialIncluded As Boolean
    Private _overloadRecoverFactor As Double
    Private _gears As IList(Of IGearEntry)
    Private _voltageLevels As IList(Of IElectricMotorVoltageLevel)
    Private _dragCurves As  IList(Of IDragCurve)
    private _filePath As String

    
    public Sub New(file As String)
        _voltageLevels = New List(Of IElectricMotorVoltageLevel)
        _filePath = file
    End Sub
    
    Public Function SaveFile As Boolean
        Try
            Dim writer = New JSONFileWriter()
            writer.SaveIEPC(Me, _filePath, Cfg.DeclMode)
        Catch ex As Exception
            MsgBox("Failed to write IEPC file: " + ex.Message)
            Return False
        End Try

        Return True
    End Function

    Public Sub SetCommonEntries(model As String, inertia As String, designTypeWheelMotorMeasured As Boolean, 
                                nrOfDesignTypeWheelMotorMeasured As Integer, differentialIncluded as Boolean,
                                thermalOverloadRecoverFactor As String)

        _model = model
        _inertia = inertia.ToDouble().SI(Of KilogramSquareMeter)
        _wheelMotorMeasured = designTypeWheelMotorMeasured
        _nrDesignTypeWheelMotor = nrOfDesignTypeWheelMotorMeasured
        _differentialIncluded = differentialIncluded
        _overloadRecoverFactor = thermalOverloadRecoverFactor.ToDouble(0)

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

    Public Sub SetGearsEntries(gearsListView As ListView)
        _gears = New List(Of IGearEntry)

        Dim gearNumber = 1
        For Each entry As  ListViewItem In gearsListView.Items

            Dim currentEntry = new GearEntry
            currentEntry.GearNumber = gearNumber
            gearNumber += 1
            
            currentEntry.Ratio = entry.SubItems(0).Text.ToDouble()
            If Not entry.SubItems(1).Text = Nothing Then _
                currentEntry.MaxOutputShaftTorque = entry.SubItems(1).Text.ToDouble().SI(Of NewtonMeter)
            If Not entry.SubItems(2).Text = Nothing Then _
                currentEntry.MaxOutputShaftSpeed = entry.SubItems(2).Text.ToDouble().RPMtoRad()
            
            _gears.Add(currentEntry)
        Next
    End Sub
    
    Public Sub SetDragCurveEntries(dragCurveListView As ListView)
        _dragCurves = New List(Of IDragCurve)

        For Each entry As ListViewItem In dragCurveListView.Items
            Dim currentEntry = New DragCurveEntry
            currentEntry.Gear = entry.SubItems(0).Text.ToInt()
            Dim tmp as SubPath = new SubPath()
            tmp.Init(GetPath(_filePath), entry.SubItems(1).Text)
            If Not File.Exists(tmp.FullPath) Then
                Throw New VectoException("Drag Curve is missing or invalid")
            Else 
                currentEntry.DragCurve = VectoCSVFile.Read(tmp.FullPath)
            End If
            _dragCurves.Add(currentEntry)
        Next
    End Sub
    
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

    Public Property ElectricMachineType As ElectricMachineType Implements IIEPCDeclarationInputData.ElectricMachineType

    Public Property R85RatedPower As Watt Implements IIEPCDeclarationInputData.R85RatedPower
    Public ReadOnly Property TotalRatedPowerCalculated As Watt Implements IIEPCDeclarationInputData.TotalRatedPowerCalculated


    Public ReadOnly Property Inertia As KilogramSquareMeter Implements IIEPCDeclarationInputData.Inertia
        Get
            Return _inertia
        End Get
    End Property

    Public ReadOnly Property DifferentialIncluded As Boolean Implements IIEPCDeclarationInputData.DifferentialIncluded
        Get
            Return _differentialIncluded
        End Get
    End Property

    Public ReadOnly Property DesignTypeWheelMotor As Boolean Implements IIEPCDeclarationInputData.DesignTypeWheelMotor
        Get
            Return _wheelMotorMeasured
        End Get
    End Property

    Public ReadOnly Property DisengagementClutch As Boolean Implements IIEPCDeclarationInputData.DisengagementClutch
        Get
            Return _disengagementClutch
        End Get
    End Property

    Public ReadOnly Property NrOfDesignTypeWheelMotorMeasured As Integer? Implements IIEPCDeclarationInputData.NrOfDesignTypeWheelMotorMeasured
        Get
            Return _nrDesignTypeWheelMotor
        End Get
    End Property

    Public ReadOnly Property Gears As IList(Of IGearEntry) Implements IIEPCDeclarationInputData.Gears
        Get
            Return _gears
        End Get
    End Property

    Public ReadOnly Property VoltageLevels As IList(Of IElectricMotorVoltageLevel) Implements IIEPCDeclarationInputData.VoltageLevels
        Get
            Return _voltageLevels
        End Get
    End Property

    Public ReadOnly Property DragCurves As IList(Of IDragCurve) Implements IIEPCDeclarationInputData.DragCurves
        Get
            Return _dragCurves
        End Get
    End Property

    Public ReadOnly Property Conditioning As TableData Implements IIEPCDeclarationInputData.Conditioning
        Get
            Return Nothing
        End Get
    End Property

    Public ReadOnly Property OverloadRecoveryFactor As Double Implements IIEPCEngineeringInputData.OverloadRecoveryFactor
        Get
            Return _overloadRecoverFactor
        End Get
    End Property

End Class
