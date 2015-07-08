Imports System.IO

Namespace Hvac

'Used By SSMTOOL Class.
Public Class SSMTechList
        Implements ISSMTechList

        'Private Fields
        Private filePath As String
        Private _ssmInputs As ISSMGenInputs
        Private _dirty As Boolean

        Public Property TechLines As List(Of ITechListBenefitLine) Implements ISSMTechList.TechLines

        'Constructors
        Public Sub New(filepath As String, genInputs As ISSMGenInputs, Optional initialiseDefaults As Boolean = False)

            Me.TechLines = New List(Of ITechListBenefitLine)

            Me.filePath = filepath

            Me._ssmInputs = genInputs

            If initialiseDefaults Then SetDefaults()

        End Sub


        Public Sub SetSSMGeneralInputs(genInputs As ISSMGenInputs) Implements ISSMTechList.SetSSMGeneralInputs

            _ssmInputs = genInputs

        End Sub

        'Initialisation Methods
        Public Function Initialise(filePath As String) As Boolean Implements ISSMTechList.Initialise

            Me.filePath = filePath

            Return Initialise()

        End Function
        Public Function Initialise() As Boolean Implements ISSMTechList.Initialise



            Dim returnStatus As Boolean = True

            If File.Exists(filePath) Then
                Using sr As StreamReader = New StreamReader(filePath)
                    'get array og lines fron csv
                    Dim lines() As String = sr.ReadToEnd().Split(CType(Environment.NewLine, Char()), StringSplitOptions.RemoveEmptyEntries)

                    'Must have at least 2 entries in map to make it usable [dont forget the header row]
                    If (lines.Count() < 1) Then
                        Return False
                    End If

                    Dim firstline As Boolean = True

                    For Each line As String In lines
                        If Not firstline Then

                            'split the line
                            Dim elements() As String = line.Split(New Char() {","}, StringSplitOptions.RemoveEmptyEntries)
                            '3 entries per line required
                            If (elements.Length <> 17) Then
                                Throw New ArgumentException("Incorrect number of values in csv file")
                            End If
                            'add values to map

                            ' 00. Category,
                            ' 01. BenefitName,
                            ' 02. Units,
                            ' 03. LowH,
                            ' 04. LowV,
                            ' 05. LowC,
                            ' 06. SemiLowH,
                            ' 07. SemiLowV,
                            ' 08. SemiLowC,
                            ' 09. RaisedH,
                            ' 10. RaisedV,
                            ' 11. RaisedC,
                            ' 12. OnVehicle,
                            ' 13. LineType,
                            ' 14. AvtiveVH,
                            ' 15. ActiveVV,
                            ' 16. ActiveVC


                            'Bus
                            Try



                                Dim tbline As New TechListBenefitLine(_ssmInputs,
                                    elements(2),
                                    elements(0),
                                    elements(1),
                                    elements(3),
                                    elements(4),
                                    elements(5),
                                    elements(6),
                                    elements(7),
                                    elements(8),
                                    elements(9),
                                    elements(10),
                                    elements(11),
                                    elements(12),
                                    elements(13),
                                    elements(14),
                                    elements(15),
                                    elements(16))

                                TechLines.Add(tbline)

                            Catch ex As Exception

                                'Indicate problems
                                returnStatus = False

                            End Try





                        Else
                            firstline = False
                        End If
                    Next line
                End Using

            Else
                returnStatus = False
            End If

            Return returnStatus


        End Function

        'Public Properties - Outputs Of Class
        Public ReadOnly Property CValueVariation As Double Implements ISSMTechList.CValueVariation
            Get
                Dim a As Double

                a = TechLines.Where(Function(x) x.Units.ToLower = "fraction").Sum(Function(s) s.C)
                
                Return a

            End Get
        End Property
        Public ReadOnly Property CValueVariationKW As Double Implements ISSMTechList.CValueVariationKW
            Get

                Dim a As Double

                a = TechLines.Where(Function(x) x.Units.ToLower = "kw").Sum(Function(s) s.C)

                Return a

            End Get
        End Property
        Public ReadOnly Property HValueVariation As Double Implements ISSMTechList.HValueVariation
            Get

                'Dim a,b As double
                Return TechLines.Where(Function(x) x.Units = "fraction").Sum(Function(s) s.H)
                ' a =  
                '  b =  HValueVariationKW
                '  Return a-b
            End Get
        End Property
        Public ReadOnly Property HValueVariationKW As Double Implements ISSMTechList.HValueVariationKW
            Get
                Return TechLines.Where(Function(x) x.Units.ToLower = "kw").Sum(Function(s) s.H)
            End Get
        End Property
        Public ReadOnly Property VCValueVariation As Double Implements ISSMTechList.VCValueVariation
            Get
                Return TechLines.Where(Function(x) x.Units.ToLower = "fraction").Sum(Function(s) s.VC) '-  VCValueVariationKW
            End Get
        End Property
        Public ReadOnly Property VCValueVariationKW As Double Implements ISSMTechList.VCValueVariationKW
            Get
                Return TechLines.Where(Function(x) x.Units.ToLower = "kw").Sum(Function(s) s.VC)
            End Get
        End Property
        Public ReadOnly Property VHValueVariation As Double Implements ISSMTechList.VHValueVariation
            Get
                'Dim a,b As double

                Return TechLines.Where(Function(x) x.Units.ToLower = "fraction").Sum(Function(s) s.VH)
                'a=TechLines.Where( Function(x) x.Units="fraction").Sum( Function(s) s.VH)
                'b=VHValueVariationKW
                ' Return  a-b
            End Get
        End Property
        Public ReadOnly Property VHValueVariationKW As Double Implements ISSMTechList.VHValueVariationKW
            Get
                Return TechLines.Where(Function(x) x.Units.ToLower = "kw").Sum(Function(s) s.VH)
            End Get
        End Property
        Public ReadOnly Property VVValueVariation As Double Implements ISSMTechList.VVValueVariation
            Get
                Return TechLines.Where(Function(x) x.Units.ToLower = "fraction").Sum(Function(s) s.VV)
            End Get
        End Property
        Public ReadOnly Property VVValueVariationKW As Double Implements ISSMTechList.VVValueVariationKW
            Get
                Return TechLines.Where(Function(x) x.Units.ToLower = "kw").Sum(Function(s) s.VV) ' - VVValueVariationKW
            End Get
        End Property

        'Member Management
        Public Function Add(item As ITechListBenefitLine, ByRef feedback As String) As Boolean Implements ISSMTechList.Add

            Dim initialCount As Integer = TechLines.Count

            If TechLines.Where(Function(w) w.Category = item.Category AndAlso w.BenefitName = item.BenefitName).count() > 0 Then
                'Failure
                feedback = "Item already exists."
                Return False
            End If


            Try

                TechLines.Add(item)

                If TechLines.Count = initialCount + 1 Then

                    'Success
                    feedback = "OK"
                    _dirty = True
                    Return True

                Else

                    'Failure
                    feedback = "The system was unable to add the new tech benefit list item."
                    Return False

                End If

            Catch ex As Exception

                feedback = "The system threw an exception and was unable to add the new tech benefit list item."
                Return False

            End Try


        End Function
        Public Sub Clear() Implements ISSMTechList.Clear

            If TechLines.Count > 0 Then _dirty = True

            TechLines.Clear()



        End Sub
        Public Function Delete(item As ITechListBenefitLine, ByRef feedback As String) As Boolean Implements ISSMTechList.Delete

            Dim currentCount As Integer = TechLines.Count

            If (TechLines.Where(Function(c) c.Category = item.Category AndAlso c.BenefitName = item.BenefitName).Count = 1) Then

                Try
                    TechLines.RemoveAt(TechLines.FindIndex(Function(c) c.Category = item.Category AndAlso c.BenefitName = item.BenefitName))

                    If TechLines.Count = currentCount - 1 Then
                        'This succeeded
                        _dirty = True
                        Return True
                    Else
                        'No Exception, but this failed for some reason.
                        feedback = "The system was unable to remove the item from the list."
                        Return False

                    End If

                Catch ex As Exception

                    feedback = "An exception occured, the removal failed."
                    Return False

                End Try


            Else

                feedback = "the item was not found in the list."
                Return False

            End If

        End Function
        Public Function Modify(originalItem As ITechListBenefitLine, newItem As ITechListBenefitLine, ByRef feedback As String) As Boolean Implements ISSMTechList.Modify

            Dim fi As TechListBenefitLine = TechLines.Find(Function(f) (f.Category = originalitem.Category) AndAlso f.BenefitName = originalitem.BenefitName)
            Dim originalUnits As String = fi.Units

            If (Not fi Is Nothing) Then

                Try

                    fi.CloneFrom(newItem)

                    'The lines below are to assist in testing. The KW units are being excluded, but for benchmarking against the spreadsheet model
                    'Two KW entries are left in. There is no provision for adding KW units in so we check if the original entry was KW and 
                    'force it back to KW if it was already so. There shoud be no need to remove this as newly created lists will not match this
                    'Phenomenon.
                    If (originalUnits.ToLower = "kw") Then
                        fi.Units = originalUnits
                        newItem.Units = originalUnits
                    End If

                    If newItem = fi Then
                        'This succeeded
                        _dirty = True
                        Return True
                    Else
                        'No Exception, but this failed for some reason.
                        feedback = "The system was unable to remove the item from the list."
                        Return False

                    End If

                Catch ex As Exception

                    feedback = "An exception occured, the update failed."
                    Return False

                End Try


            Else

                feedback = "the item was not found so cannot be modified."
                Return False

            End If


        End Function

#Region "Default Values"

        Private Sub SetDefaults()

            Dim techLine1 As ITechListBenefitLine = New TechListBenefitLine()
            With techLine1
                .Category = "Cooling"
                .BenefitName = "Separate air distribution ducts"
                .LowFloorH = 0
                .LowFloorC = 0.04
                .LowFloorV = 0
                .SemiLowFloorH = 0
                .SemiLowFloorC = 0.04
                .SemiLowFloorV = 0
                .RaisedFloorH = 0
                .RaisedFloorC = 0.04
                .RaisedFloorV = 0
                .ActiveVH = 0
                .ActiveVV = 0
                .ActiveVC = 0
                .OnVehicle = False
                .Units = "fraction"
            End With

            Dim techLine2 As ITechListBenefitLine = New TechListBenefitLine()
            With techLine2
                .Category = "Heating"
                .BenefitName = "Adjustable auxiliary heater"
                .LowFloorH = 0.02
                .LowFloorC = 0
                .LowFloorV = 0
                .SemiLowFloorH = 0.02
                .SemiLowFloorC = 0
                .SemiLowFloorV = 0
                .RaisedFloorH = 0.02
                .RaisedFloorC = 0
                .RaisedFloorV = 0
                .ActiveVH = 0
                .ActiveVV = 0
                .ActiveVC = 0
                .OnVehicle = False
                .Units = "fraction"
            End With

            Dim techLine3 As ITechListBenefitLine = New TechListBenefitLine()
            With techLine3
                .Category = "Heating"
                .BenefitName = "Adjustable coolant thermostat"
                .LowFloorH = 0.02
                .LowFloorC = 0
                .LowFloorV = 0
                .SemiLowFloorH = 0.02
                .SemiLowFloorC = 0
                .SemiLowFloorV = 0
                .RaisedFloorH = 0.02
                .RaisedFloorC = 0
                .RaisedFloorV = 0
                .ActiveVH = 0
                .ActiveVV = 0
                .ActiveVC = 0
                .OnVehicle = False
                .Units = "fraction"
            End With

            Dim techLine4 As ITechListBenefitLine = New TechListBenefitLine()
            With techLine4
                .Category = "Heating"
                .BenefitName = "Engine waste gas heat exchanger"
                .LowFloorH = 0.04
                .LowFloorC = 0
                .LowFloorV = 0
                .SemiLowFloorH = 0
                .SemiLowFloorC = 0
                .SemiLowFloorV = 0
                .RaisedFloorH = 0
                .RaisedFloorC = 0
                .RaisedFloorV = 0
                .ActiveVH = 0
                .ActiveVV = 0
                .ActiveVC = 0
                .OnVehicle = False
                .Units = "fraction"
            End With

            Dim techLine5 As ITechListBenefitLine = New TechListBenefitLine()
            With techLine5
                .Category = "Heating"
                .BenefitName = "Heat pump systems"
                .LowFloorH = 0.06
                .LowFloorC = 0
                .LowFloorV = 0
                .SemiLowFloorH = 0.04
                .SemiLowFloorC = 0
                .SemiLowFloorV = 0
                .RaisedFloorH = 0.04
                .RaisedFloorC = 0
                .RaisedFloorV = 0
                .ActiveVH = 0
                .ActiveVV = 0
                .ActiveVC = 0
                .OnVehicle = False
                .Units = "fraction"
            End With

            Dim techLine6 As ITechListBenefitLine = New TechListBenefitLine()
            With techLine6
                .Category = "Insulation"
                .BenefitName = "Double-glazing"
                .LowFloorH = 0.04
                .LowFloorC = 0.04
                .LowFloorV = 0
                .SemiLowFloorH = 0.04
                .SemiLowFloorC = 0.04
                .SemiLowFloorV = 0
                .RaisedFloorH = 0.04
                .RaisedFloorC = 0.04
                .RaisedFloorV = 0
                .ActiveVH = 0
                .ActiveVV = 0
                .ActiveVC = 0
                .OnVehicle = False
                .Units = "fraction"
            End With

            Dim techLine7 As ITechListBenefitLine = New TechListBenefitLine()
            With techLine7
                .Category = "Insulation"
                .BenefitName = "Tinted windows"
                .LowFloorH = 0
                .LowFloorC = 0
                .LowFloorV = 0
                .SemiLowFloorH = 0
                .SemiLowFloorC = 0
                .SemiLowFloorV = 0
                .RaisedFloorH = 0
                .RaisedFloorC = 0
                .RaisedFloorV = 0
                .ActiveVH = 0
                .ActiveVV = 0
                .ActiveVC = 0
                .OnVehicle = False
                .Units = "fraction"
            End With

            Dim techLine8 As ITechListBenefitLine = New TechListBenefitLine()
            With techLine8
                .Category = "Ventilation"
                .BenefitName = "Fan control strategy (serial/parallel)"
                .LowFloorH = 0
                .LowFloorC = 0
                .LowFloorV = 0.02
                .SemiLowFloorH = 0
                .SemiLowFloorC = 0
                .SemiLowFloorV = 0.02
                .RaisedFloorH = 0
                .RaisedFloorC = 0
                .RaisedFloorV = 0.02
                .ActiveVH = 0.02
                .ActiveVV = 0.02
                .ActiveVC = 0.02
                .OnVehicle = False
                .Units = "fraction"
                .LineType = TechLineType.HVCActiveSelection
            End With

            Dim feedback As String = String.Empty
            Add(techLine1, feedback)
            Add(techLine2, feedback)
            Add(techLine3, feedback)
            Add(techLine4, feedback)
            Add(techLine5, feedback)
            Add(techLine6, feedback)
            Add(techLine7, feedback)
            Add(techLine8, feedback)

        End Sub

#End Region

End Class

End Namespace



