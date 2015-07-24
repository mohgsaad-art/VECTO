Imports Omu.ValueInjecter
Imports VectoAuxiliaries.Hvac
Imports Newtonsoft.Json
Imports System.IO
Imports System.Reflection
Imports System.Text

Namespace Hvac

    'Used by frmHVACTool
    'Replaces Spreadsheet model which does the same calculation
    'Version of which appears on the form title.
    Public Class SSMTOOL
        Implements ISSMTOOL

        Private filePath As String
        Public Property GenInputs As ISSMGenInputs Implements ISSMTOOL.GenInputs
        Public Property TechList As ISSMTechList Implements ISSMTOOL.TechList
        Public Property Calculate As ISSMCalculate Implements ISSMTOOL.Calculate
        Public Property SSMDisabled As Boolean Implements ISSMTOOL.SSMDisabled
        Public Property HVACConstants As IHVACConstants Implements ISSMTOOL.HVACConstants

        'Repeat Warning Flags
        Private CompressorCapacityInsufficientWarned As Boolean
        Private FuelFiredHeaterInsufficientWarned As Boolean

        'Base Values
        Public ReadOnly Property ElectricalWBase As Single Implements ISSMTOOL.ElectricalWBase
            Get
                Return If(SSMDisabled, 0, Calculate.ElectricalWBase)
            End Get
        End Property
        Public ReadOnly Property MechanicalWBase As Single Implements ISSMTOOL.MechanicalWBase
            Get
                Return If(SSMDisabled, 0, Calculate.MechanicalWBase)
            End Get
        End Property
        Public ReadOnly Property FuelPerHBase As Single Implements ISSMTOOL.FuelPerHBase
            Get
                Return If(SSMDisabled, 0, Calculate.FuelPerHBase)
            End Get
        End Property

        'Adjusted Values
        Public ReadOnly Property ElectricalWAdjusted As Single Implements ISSMTOOL.ElectricalWAdjusted
            Get
                Return If(SSMDisabled, 0, Calculate.ElectricalWAdjusted)
            End Get
        End Property
        Public ReadOnly Property MechanicalWBaseAdjusted As Single Implements ISSMTOOL.MechanicalWBaseAdjusted
            Get
                Dim mechAdjusted As Single = If(SSMDisabled, 0, Calculate.MechanicalWBaseAdjusted)

                If CompressorCapacityInsufficientWarned = False AndAlso (mechAdjusted) / (1000 * GenInputs.AC_COP) > GenInputs.AC_CompressorCapacitykW Then

                    OnMessage(Me, "HVAC SSM :AC-Compressor Capacity unable to service cooling, run continues as if capacity was sufficient.", AdvancedAuxiliaryMessageType.Warning)
                    CompressorCapacityInsufficientWarned = True

                End If


                Return mechAdjusted

            End Get
        End Property
        Public ReadOnly Property FuelPerHBaseAdjusted As Single Implements ISSMTOOL.FuelPerHBaseAdjusted
            Get
                Return If(SSMDisabled, 0, Calculate.FuelPerHBaseAdjusted)
            End Get
        End Property

        'Constructors
        Sub New(filePath As String, hvacConstants As HVACConstants, Optional isDisabled As Boolean = False, Optional useTestValues As Boolean = False)

            Me.filePath = filePath
            Me.SSMDisabled = isDisabled
            Me.HVACConstants = hvacConstants

            GenInputs = New SSMGenInputs(useTestValues, fPATH(filePath))
            TechList = New SSMTechList(filePath, GenInputs, useTestValues)

            Calculate = New SSMCalculate(Me)

        End Sub

        'Clone values from another object of same type
        Public Sub Clone(from As ISSMTOOL) Implements ISSMTOOL.Clone

            Dim feedback As String = String.Empty

            genInputs.InjectFrom(DirectCast(from, SSMTOOL).genInputs)

            TechList.Clear()

            For Each line As TechListBenefitLine In DirectCast(from, SSMTOOL).TechList.TechLines

                Dim newLine As New TechListBenefitLine(Me.GenInputs)
                newLine.InjectFrom()
                newLine.InjectFrom(line)
                TechList.Add(newLine, feedback)

            Next

        End Sub

        'Persistance Functions
        Public Function Save(filePath As String) As Boolean Implements ISSMTOOL.Save


            Dim returnValue As Boolean = True
            Dim settings As JsonSerializerSettings = New JsonSerializerSettings()
            settings.TypeNameHandling = TypeNameHandling.Objects

            'JSON METHOD
            Try

                Dim output As String = JsonConvert.SerializeObject(Me, Formatting.Indented, settings)

                File.WriteAllText(filePath, output)

            Catch ex As Exception

                'Nothing to do except return false.
                returnValue = False

            End Try

            Return returnValue

        End Function
        Public Function Load(filePath As String) As Boolean Implements ISSMTOOL.Load

            Dim returnValue As Boolean = True
            Dim settings As JsonSerializerSettings = New JsonSerializerSettings()
            Dim tmpAux As SSMTOOL = New SSMTOOL(filePath, HVACConstants)

            settings.TypeNameHandling = TypeNameHandling.Objects

            'JSON METHOD
            Try

                Dim output As String = File.ReadAllText(filePath)


                tmpAux = JsonConvert.DeserializeObject(Of SSMTOOL)(output, settings)

                tmpAux.TechList.SetSSMGeneralInputs(tmpAux.genInputs)

                For Each tll As TechListBenefitLine In tmpAux.TechList.TechLines

                    tll.inputSheet = tmpAux.genInputs

                Next


                'This is where we Assume values of loaded( Deserialized ) object.
                Clone(tmpAux)

            Catch ex As Exception

                'Nothing to do except return false.

                returnValue = False
            End Try

            Return returnValue

        End Function

        'Comparison
        Public Function IsEqualTo(source As ISSMTOOL) As Boolean Implements ISSMTOOL.IsEqualTo

            'In this methods we only want to compare the non Static , non readonly public properties of 
            'The class's General, User Inputs and  Tech Benefit members.

            Return compareGenUserInputs(source) AndAlso compareTechListBenefitLines(source)


        End Function
        Private Function compareGenUserInputs(source As ISSMTOOL) As Boolean

            Dim src As SSMTOOL = DirectCast(source, SSMTOOL)

            Dim returnValue As Boolean = True

            Dim properties As PropertyInfo() = Me.genInputs.GetType.GetProperties

            For Each prop As PropertyInfo In properties

                'If Not prop.GetAccessors.IsReadOnly Then
                If prop.CanWrite Then
                    If prop.GetValue(Me.GenInputs, Nothing) <> prop.GetValue(src.GenInputs, Nothing) Then
                        returnValue = False
                    End If

                End If

            Next

            Return returnValue

        End Function
        Private Function compareTechListBenefitLines(source As ISSMTOOL) As Boolean


            Dim src As SSMTOOL = DirectCast(source, SSMTOOL)

            'Equal numbers of lines check
            If Me.TechList.TechLines.Count <> src.TechList.TechLines.Count Then Return False

            For Each tl As ITechListBenefitLine In Me.TechList.TechLines.OrderBy(Function(o) o.Category).ThenBy(Function(n) n.BenefitName)

                'First Check line exists in other
                If src.TechList.TechLines.Where(Function(w) w.BenefitName = tl.BenefitName AndAlso w.Category = tl.Category).Count <> 1 Then

                    Return False
                Else

                    'check are equal

                    Dim testLine As ITechListBenefitLine = src.TechList.TechLines.First(Function(w) w.BenefitName = tl.BenefitName AndAlso w.Category = tl.Category)

                    If Not testLine.IsEqualTo(tl) Then
                        Return False
                    End If

                End If


            Next

            'All Looks OK
            Return True

        End Function

        'Overrides
        Public Overrides Function ToString() As String


            Dim sb As New StringBuilder

            sb.AppendLine(Calculate.ToString())


            Return sb.ToString()

        End Function

        'Dynamicly Get Fuel having re-adjusted Engine Heat Waste, this was originally supposed to be Solid State. Late adjustment request 24/3/2015
        Public Function FuelPerHBaseAsjusted(AverageUseableEngineWasteHeatKW As Single) As Single Implements ISSMTOOL.FuelPerHBaseAsjusted

            If SSMDisabled Then
                Return 0
            End If

            'Set Engine Waste Heat
            GenInputs.AH_EngineWasteHeatkW = AverageUseableEngineWasteHeatKW
            Dim fba As Single = FuelPerHBaseAdjusted

            'Dim FuelFiredWarning As Boolean = fba * GenInputs.BC_AuxHeaterEfficiency * HVACConstants.FuelDensity * GenInputs.BC_GCVDieselOrHeatingOil * 1000 > (AverageUseableEngineWasteHeatKW + GenInputs.AH_FuelFiredHeaterkW)

            'If Not FuelFiredHeaterInsufficientWarned AndAlso FuelFiredWarning Then

            '    FuelFiredHeaterInsufficientWarned = True

            '    OnMessage(Me, " HVAC SSM : Fuel fired heater insufficient for heating requirements, run continues assuming it was sufficient.", AdvancedAuxiliaryMessageType.Warning)

            'End If

            Return fba

        End Function

        'Events
        Public Event Message(ByRef sender As Object, message As String, messageType As AdvancedAuxiliaryMessageType) Implements ISSMTOOL.Message

        'Raise Message Event.
        Private Sub OnMessage(sender As Object, message As String, messageType As AdvancedAuxiliaryMessageType)


            If Not message Is Nothing Then

                RaiseEvent Message(Me, message, messageType)

            End If

        End Sub

    End Class


End Namespace