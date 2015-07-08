Imports System.Text
Imports Microsoft.VisualBasic
Imports VectoAuxiliaries.Hvac

Namespace Hvac


    Public Class SSMCalculate
        Implements ISSMCalculate

        Private ssmTOOL As ISSMTOOL
        Private Property Run1 As ISSMRun Implements ISSMCalculate.Run1
        Private Property Run2 As ISSMRun Implements ISSMCalculate.Run2

        'Constructor
        Sub New(ssmTool As ISSMTOOL)

            Me.ssmTOOL = ssmTool
            Run1 = New SSMRun(Me.ssmTOOL, 1)
            Run2 = New SSMRun(Me.ssmTOOL, 2)

        End Sub

#Region "Main Outputs"

        'BASE RESULTS
        Public ReadOnly Property ElectricalWBase As Single Implements ISSMCalculate.ElectricalWBase
            Get

                Dim ElectricalWBaseWeightedAverage As Single
                Dim gen As ISSMGenInputs = ssmTOOL.GenInputs
                Dim EC_EnviromentalTemperatureBefore As Double = gen.EC_EnviromentalTemperature
                Dim EC_SolarBefore As Double = gen.EC_Solar

                If Not gen.EC_EnviromentalConditions_BatchEnabled Then
                    ElectricalWBaseWeightedAverage = CalculateElectricalWBase(gen, gen.EC_EnviromentalTemperature, gen.EC_Solar, 1)
                Else
                    For Each envCondition As IEnvironmentalCondition In gen.EC_EnvironmentalConditionsMap.GetEnvironmentalConditions()
                        ElectricalWBaseWeightedAverage += CalculateElectricalWBase(gen, envCondition.GetTemperature(), envCondition.GetSolar(), envCondition.GetNormalisedWeighting(gen.EC_EnvironmentalConditionsMap.GetEnvironmentalConditions()))
                    Next
                    gen.EC_EnviromentalTemperature = EC_EnviromentalTemperatureBefore
                    gen.EC_Solar = EC_SolarBefore
                End If

                Return ElectricalWBaseWeightedAverage

            End Get
        End Property
        Public ReadOnly Property MechanicalWBase As Single Implements ISSMCalculate.MechanicalWBase
            Get

                Dim MechanicalWBaseWeightedAverage As Single
                Dim gen As ISSMGenInputs = ssmTOOL.GenInputs
                Dim EC_EnviromentalTemperatureBefore As Double = gen.EC_EnviromentalTemperature
                Dim EC_SolarBefore As Double = gen.EC_Solar

                If Not gen.EC_EnviromentalConditions_BatchEnabled Then
                    MechanicalWBaseWeightedAverage = CalculateMechanicalWBase(gen, gen.EC_EnviromentalTemperature, gen.EC_Solar, 1)
                Else
                    For Each envCondition As IEnvironmentalCondition In gen.EC_EnvironmentalConditionsMap.GetEnvironmentalConditions()
                        MechanicalWBaseWeightedAverage += CalculateMechanicalWBase(gen, envCondition.GetTemperature(), envCondition.GetSolar(), envCondition.GetNormalisedWeighting(gen.EC_EnvironmentalConditionsMap.GetEnvironmentalConditions()))
                    Next
                    gen.EC_EnviromentalTemperature = EC_EnviromentalTemperatureBefore
                    gen.EC_Solar = EC_SolarBefore
                End If

                Return MechanicalWBaseWeightedAverage

            End Get
        End Property
        Public ReadOnly Property FuelLPerHBase As Single Implements ISSMCalculate.FuelPerHBase
            Get

                Dim FuelLPerHBaseWeightedAverage As Single
                Dim gen As ISSMGenInputs = ssmTOOL.GenInputs
                Dim EC_EnviromentalTemperatureBefore As Double = gen.EC_EnviromentalTemperature
                Dim EC_SolarBefore As Double = gen.EC_Solar

                If Not gen.EC_EnviromentalConditions_BatchEnabled Then
                    FuelLPerHBaseWeightedAverage = CalculateFuelLPerHBase(gen, gen.EC_EnviromentalTemperature, gen.EC_Solar, 1)
                Else
                    For Each envCondition As IEnvironmentalCondition In gen.EC_EnvironmentalConditionsMap.GetEnvironmentalConditions()
                        FuelLPerHBaseWeightedAverage += CalculateFuelLPerHBase(gen, envCondition.GetTemperature(), envCondition.GetSolar(), envCondition.GetNormalisedWeighting(gen.EC_EnvironmentalConditionsMap.GetEnvironmentalConditions()))
                    Next
                    gen.EC_EnviromentalTemperature = EC_EnviromentalTemperatureBefore
                    gen.EC_Solar = EC_SolarBefore
                End If

                Return FuelLPerHBaseWeightedAverage

            End Get
        End Property

        'ADJUSTED RESULTS
        Public ReadOnly Property ElectricalWAdjusted As Single Implements ISSMCalculate.ElectricalWAdjusted
            Get
                Dim ElectricalWAdjustedAverage As Single
                Dim gen As ISSMGenInputs = ssmTOOL.GenInputs
                Dim tl As ISSMTechList = ssmTOOL.TechList
                Dim EC_EnviromentalTemperatureBefore As Double = gen.EC_EnviromentalTemperature
                Dim EC_SolarBefore As Double = gen.EC_Solar

                If Not gen.EC_EnviromentalConditions_BatchEnabled Then
                    ElectricalWAdjustedAverage = CalculateElectricalWAdjusted(gen, tl, gen.EC_EnviromentalTemperature, gen.EC_Solar, 1)
                Else
                    For Each envCondition As IEnvironmentalCondition In gen.EC_EnvironmentalConditionsMap.GetEnvironmentalConditions()
                        ElectricalWAdjustedAverage += CalculateElectricalWAdjusted(gen, tl, envCondition.GetTemperature(), envCondition.GetSolar(), envCondition.GetNormalisedWeighting(gen.EC_EnvironmentalConditionsMap.GetEnvironmentalConditions()))
                    Next
                    gen.EC_EnviromentalTemperature = EC_EnviromentalTemperatureBefore
                    gen.EC_Solar = EC_SolarBefore
                End If

                Return ElectricalWAdjustedAverage

            End Get
        End Property
        Public ReadOnly Property MechanicalWBaseAdjusted As Single Implements ISSMCalculate.MechanicalWBaseAdjusted
            Get

                Dim MechanicalWBaseAdjustedAverage As Single
                Dim gen As ISSMGenInputs = ssmTOOL.GenInputs
                Dim tl As ISSMTechList = ssmTOOL.TechList
                Dim EC_EnviromentalTemperatureBefore As Double = gen.EC_EnviromentalTemperature
                Dim EC_SolarBefore As Double = gen.EC_Solar

                If Not gen.EC_EnviromentalConditions_BatchEnabled Then
                    MechanicalWBaseAdjustedAverage = CalculateMechanicalWBaseAdjusted(gen, tl, gen.EC_EnviromentalTemperature, gen.EC_Solar, 1)
                Else
                    For Each envCondition As IEnvironmentalCondition In gen.EC_EnvironmentalConditionsMap.GetEnvironmentalConditions()
                        MechanicalWBaseAdjustedAverage += CalculateMechanicalWBaseAdjusted(gen, tl, envCondition.GetTemperature(), envCondition.GetSolar(), envCondition.GetNormalisedWeighting(gen.EC_EnvironmentalConditionsMap.GetEnvironmentalConditions()))
                    Next
                    gen.EC_EnviromentalTemperature = EC_EnviromentalTemperatureBefore
                    gen.EC_Solar = EC_SolarBefore
                End If

                Return MechanicalWBaseAdjustedAverage

            End Get

        End Property
        Public ReadOnly Property FuelLPerHBaseAdjusted As Single Implements ISSMCalculate.FuelPerHBaseAdjusted
            Get
                Dim FuelLPerHBaseAdjustedAverage As Single
                Dim gen As ISSMGenInputs = ssmTOOL.GenInputs
                Dim tl As ISSMTechList = ssmTOOL.TechList
                Dim EC_EnviromentalTemperatureBefore As Double = gen.EC_EnviromentalTemperature
                Dim EC_SolarBefore As Double = gen.EC_Solar

                If Not gen.EC_EnviromentalConditions_BatchEnabled Then
                    FuelLPerHBaseAdjustedAverage = CalculateFuelLPerHBaseAdjusted(gen, tl, gen.EC_EnviromentalTemperature, gen.EC_Solar, 1)
                Else
                    For Each envCondition As IEnvironmentalCondition In gen.EC_EnvironmentalConditionsMap.GetEnvironmentalConditions()
                        FuelLPerHBaseAdjustedAverage += CalculateFuelLPerHBaseAdjusted(gen, tl, envCondition.GetTemperature(), envCondition.GetSolar(), envCondition.GetNormalisedWeighting(gen.EC_EnvironmentalConditionsMap.GetEnvironmentalConditions()))
                    Next
                    gen.EC_EnviromentalTemperature = EC_EnviromentalTemperatureBefore
                    gen.EC_Solar = EC_SolarBefore
                End If

                Return FuelLPerHBaseAdjustedAverage

            End Get
        End Property


#End Region

#Region "Staging Calculations"

        'Base Values
        Public ReadOnly Property BaseHeatingW_Mechanical As Double Implements ISSMCalculate.BaseHeatingW_Mechanical
            Get
                Return Nothing
            End Get
        End Property
        Public ReadOnly Property BaseHeatingW_ElectricalCoolingHeating As Double Implements ISSMCalculate.BaseHeatingW_ElectricalCoolingHeating
            Get
                Return Nothing
            End Get
        End Property
        Public ReadOnly Property BaseHeatingW_ElectricalVentilation As Double Implements ISSMCalculate.BaseHeatingW_ElectricalVentilation
            Get
                '=IF(AND(M79<0,M80<0),IF(AND(C52="yes",C56="high"),C30,IF(AND(C52="yes",C56="low"),C31,0)),0)

                Dim gen As ISSMGenInputs = ssmTOOL.GenInputs

                'Dim C30 = gen.BC_HighVentPowerW
                'Dim C31 = gen.BC_LowVentPowerW
                'Dim C52 = gen.VEN_VentilationONDuringHeating
                'Dim C56 = gen.VEN_VentilationDuringHeating
                'Dim M79 = Me.Run1.TotalW
                'Dim M80 = Me.Run2.TotalW

                Dim res As Double

                res = If(Run1.TotalW < 0 AndAlso Run2.TotalW < 0, _
                        If(gen.VEN_VentilationOnDuringHeating AndAlso gen.VEN_VentilationDuringHeating.ToLower() = "high", _
                                        gen.BC_HighVentPowerW, _
                                        If(gen.VEN_VentilationOnDuringHeating AndAlso gen.VEN_VentilationDuringHeating.ToLower() = "low", gen.BC_LowVentPowerW, 0)), 0)


                Return res

            End Get

        End Property
        Public ReadOnly Property BaseHeatingW_FuelFiredHeating As Double Implements ISSMCalculate.BaseHeatingW_FuelFiredHeating

            Get

                '=IF(AND(M79<0,M80<0),VLOOKUP(MAX(M79:M80),M79:O80,3),0)

                'Dim M79 = Me.Run1.TotalW
                'Dim M80 = Me.Run2.TotalW
                'VLOOKUP(MAX(M79:M80),M79:O80  => VLOOKUP ( lookupValue, tableArray, colIndex, rangeLookup )

                'If both Run TotalW values are >=0 then return FuelW from Run with largest TotalW value, else return 0
                If (Run1.TotalW < 0 AndAlso Run2.TotalW < 0) Then

                    Return If(Run1.TotalW > Run2.TotalW, Run1.FuelW, Run2.FuelW)

                Else

                    Return 0

                End If

            End Get

        End Property
        Public ReadOnly Property BaseCoolingW_Mechanical As Double Implements ISSMCalculate.BaseCoolingW_Mechanical
            Get
                '=IF(C46<C28,0,IF(C53="electrical", 0, IF(AND(M89>0,M90>0),MIN(M89:M90),0)))

                Dim gen As ISSMGenInputs = ssmTOOL.GenInputs

                'Dim C46 = gen.EC_EnviromentalTemperature
                'Dim C28 = gen.BC_TemperatureCoolingTurnsOff
                'Dim C53 = gen.AC_CompressorTypeDerived
                'Dim M89 = Run1.TotalW
                'Dim M90 = Run2.TotalW

                Return If(gen.EC_EnviromentalTemperature < gen.BC_TemperatureCoolingTurnsOff, 0, If(gen.AC_CompressorTypeDerived.ToLower() = "electrical", 0, If(Run1.TotalW > 0 AndAlso Run2.TotalW > 0, Math.Min(Run1.TotalW, Run2.TotalW), 0)))

           End Get
        End Property
        Public ReadOnly Property BaseCoolingW_ElectricalCoolingHeating As Double Implements ISSMCalculate.BaseCoolingW_ElectricalCoolingHeating
            Get
                '=IF(C46<C28,0,IF(C53="electrical",IF(AND(M89>0,M90>0),MIN(M89:M90),0),0))

                Dim gen As ISSMGenInputs = ssmTOOL.GenInputs

                'Dim C46 = gen.EC_EnviromentalTemperature
                'Dim C28 = gen.BC_TemperatureCoolingTurnsOff
                'Dim C53 = gen.AC_CompressorTypeDerived
                'Dim M89 = Run1.TotalW
                'Dim M90 = Run2.TotalW

                Return If(gen.EC_EnviromentalTemperature < gen.BC_TemperatureCoolingTurnsOff, 0, If(gen.AC_CompressorTypeDerived.ToLower() = "electrical", If(Run1.TotalW > 0 AndAlso Run2.TotalW > 0, Math.Min(Run1.TotalW, Run2.TotalW), 0), 0))

            End Get
        End Property
        Public ReadOnly Property BaseCoolingW_ElectricalVentilation As Double Implements ISSMCalculate.BaseCoolingW_ElectricalVentilation
            Get
                '=IF(AND(C46>=C28,M89>0,M90>0),IF(AND(C64="yes",C67="high"),C33,IF(AND(C64="yes",C67="low"),C34,0)),0)

                Dim gen As ISSMGenInputs = ssmTOOL.GenInputs

                'Dim C46 = gen.EC_EnviromentalTemperature
                'Dim C28 = gen.BC_TemperatureCoolingTurnsOff
                'Dim M89 = Run1.TotalW
                'Dim M90 = Run2.TotalW
                'Dim C64 = gen.VEN_VentilationDuringAC
                'Dim C67 = gen.VEN_VentilationDuringCooling
                'Dim C33 = gen.BC_HighVentPowerW
                'Dim C34 = gen.BC_LowVentPowerW

                Return If(gen.EC_EnviromentalTemperature >= gen.BC_TemperatureCoolingTurnsOff AndAlso Run1.TotalW > 0 AndAlso Run2.TotalW > 0, _
                          If(gen.VEN_VentilationDuringAC AndAlso gen.VEN_VentilationDuringCooling.ToLower() = "high", gen.BC_HighVentPowerW, If(gen.VEN_VentilationDuringAC AndAlso gen.VEN_VentilationDuringCooling.ToLower() = "low", gen.BC_LowVentPowerW, 0)) _
                          , 0)


            End Get
        End Property
        Public ReadOnly Property BaseCoolingW_FuelFiredHeating As Double Implements ISSMCalculate.BaseCoolingW_FuelFiredHeating
            Get
                Return 0
            End Get
        End Property

        Public ReadOnly Property BaseVentilationW_Mechanical As Double Implements ISSMCalculate.BaseVentilationW_Mechanical
            Get
                Return Nothing
            End Get
        End Property
        Public ReadOnly Property BaseVentilationW_ElectricalCoolingHeating As Double Implements ISSMCalculate.BaseVentilationW_ElectricalCoolingHeating
            Get
                Return Nothing
            End Get
        End Property
        Public ReadOnly Property BaseVentilationW_ElectricalVentilation As Double Implements ISSMCalculate.BaseVentilationW_ElectricalVentilation
            Get
                '=IF(OR(AND(C46<C28,M89>0,M90>0),AND(M89>0,M90<0)),IF(AND(C63="yes",C65="high"),C33,IF(AND(C63="yes",C65="low"),C34,0)),0)

                Dim gen As ISSMGenInputs = ssmTOOL.GenInputs

                'Dim C46 = gen.EC_EnviromentalTemperature
                'Dim C28 = gen.BC_TemperatureCoolingTurnsOff
                'Dim M89 = Run1.TotalW
                'Dim M90 = Run2.TotalW
                'Dim C63 = gen.VEN_VentilationWhenBothHeatingAndACInactive
                'Dim C65 = gen.VEN_VentilationFlowSettingWhenHeatingAndACInactive
                'Dim C33 = gen.BC_HighVentPowerW
                'Dim C34 = gen.BC_LowVentPowerW

                Return If((gen.EC_EnviromentalTemperature < gen.BC_TemperatureCoolingTurnsOff AndAlso Run1.TotalW > 0 AndAlso Run2.TotalW > 0) OrElse (Run1.TotalW > 0 AndAlso Run2.TotalW < 0), _
                          If(gen.VEN_VentilationWhenBothHeatingAndACInactive AndAlso gen.VEN_VentilationFlowSettingWhenHeatingAndACInactive.ToLower() = "high", gen.BC_HighVentPowerW, If(gen.VEN_VentilationWhenBothHeatingAndACInactive AndAlso gen.VEN_VentilationFlowSettingWhenHeatingAndACInactive.ToLower() = "low", gen.BC_LowVentPowerW, 0)), 0)

            End Get
        End Property
        Public ReadOnly Property BaseVentilationW_FuelFiredHeating As Double Implements ISSMCalculate.BaseVentilationW_FuelFiredHeating
            Get
                Return 0
            End Get
        End Property

        'Adjusted Values
        Public ReadOnly Property TechListAdjustedHeatingW_Mechanical As Double Implements ISSMCalculate.TechListAdjustedHeatingW_Mechanical
            Get
                Return Nothing
            End Get
        End Property
        Public ReadOnly Property TechListAdjustedHeatingW_ElectricalCoolingHeating As Double Implements ISSMCalculate.TechListAdjustedHeatingW_ElectricalCoolingHeating
            Get
                Return Nothing
            End Get
        End Property
        Public ReadOnly Property TechListAdjustedHeatingW_ElectricalVentilation As Double Implements ISSMCalculate.TechListAdjustedHeatingW_ElectricalVentilation
            Get
                '=IF('TECH LIST INPUT'!O92>0,MIN('TECH LIST INPUT'!O92,C40),MAX('TECH LIST INPUT'!O92,-C40))
                Dim gen As ISSMGenInputs = ssmTOOL.GenInputs
                Dim tl As ISSMTechList = ssmTOOL.TechList

                'TECH LIST INPUT'!O92
                'Dim C40 As Double   =  gen.BC_MaxPossibleBenefitFromTechnologyList
                'Dim TLO92 As Double = tl.VHValueVariation


                Return If(tl.VHValueVariation > 0, Math.Min(tl.VHValueVariation, gen.BC_MaxPossibleBenefitFromTechnologyList), Math.Max(tl.VHValueVariation, -gen.BC_MaxPossibleBenefitFromTechnologyList))

            End Get

        End Property
        Public ReadOnly Property TechListAdjustedHeatingW_FuelFiredHeating As Double Implements ISSMCalculate.TechListAdjustedHeatingW_FuelFiredHeating
            Get
                '=IF('TECH LIST INPUT'!N92>0,MIN('TECH LIST INPUT'!N92,C40),MAX('TECH LIST INPUT'!N92,-C40))


                Dim gen As ISSMGenInputs = ssmTOOL.GenInputs
                Dim tl As ISSMTechList = ssmTOOL.TechList

                'TECH LIST INPUT'!N92
                'Dim C40 As Double   =  gen.BC_MaxPossibleBenefitFromTechnologyList
                'Dim TLN92 As Double =  tl.HValueVariation


                Return If(tl.HValueVariation > 0, Math.Min(tl.HValueVariation, gen.BC_MaxPossibleBenefitFromTechnologyList), Math.Max(tl.HValueVariation, -gen.BC_MaxPossibleBenefitFromTechnologyList))


            End Get

        End Property

        Public ReadOnly Property TechListAdjustedCoolingW_Mechanical As Double Implements ISSMCalculate.TechListAdjustedCoolingW_Mechanical
            Get
                '=IF(IF(C48="mechanical",'TECH LIST INPUT'!R92,0)>0,MIN(IF(C48="mechanical",'TECH LIST INPUT'!R92,0),C40),MAX(IF(C48="mechanical",'TECH LIST INPUT'!R92,0),-C40))

                Dim gen As ISSMGenInputs = ssmTOOL.GenInputs
                Dim tl As ISSMTechList = ssmTOOL.TechList
                Dim result As Double
                'Dim TLR92 As Double =  tl.CValueVariation 'TECH LIST INPUT'!R92
                'Dim C40 As Double   =  gen.BC_MaxPossibleBenefitFromTechnologyList
                'Dim C48 As string   =  gen.AC_CompressorType

                result = If(If(gen.AC_CompressorType.ToLower() = "mechanical", tl.CValueVariation, 0) > 0, _
                                Math.Min(If(gen.AC_CompressorType.ToLower() = "mechanical", tl.CValueVariation, 0), gen.BC_MaxPossibleBenefitFromTechnologyList), _
                                Math.Max(If(gen.AC_CompressorType.ToLower() = "mechanical", tl.CValueVariation, 0), -gen.BC_MaxPossibleBenefitFromTechnologyList))

                Return result

            End Get
        End Property
        Public ReadOnly Property TechListAdjustedCoolingW_ElectricalCoolingHeating As Double Implements ISSMCalculate.TechListAdjustedCoolingW_ElectricalCoolingHeating
            Get
                '=IF(IF(C48="mechanical",0,'TECH LIST INPUT'!R92)>0,MIN(IF(C48="mechanical",0,'TECH LIST INPUT'!R92),C40),MAX(IF(C48="mechanical",0,'TECH LIST INPUT'!R92),-C40))


                Dim gen As ISSMGenInputs = ssmTOOL.GenInputs
                Dim tl As ISSMTechList = ssmTOOL.TechList
                Dim result As Double

                'Dim TLR92 As Double =  tl.CValueVariation 'TECH LIST INPUT'!R92
                'Dim C40 As Double   =  gen.BC_MaxPossibleBenefitFromTechnologyList
                'Dim C48 As string   =  gen.AC_CompressorType

                result = If(If(gen.AC_CompressorType.ToLower() = "mechanical", 0, tl.CValueVariation) > 0, _
                            Math.Min(If(gen.AC_CompressorType.ToLower() = "mechanical", 0, tl.CValueVariation), gen.BC_MaxPossibleBenefitFromTechnologyList), _
                            Math.Max(If(gen.AC_CompressorType.ToLower() = "mechanical", 0, tl.CValueVariation), -gen.BC_MaxPossibleBenefitFromTechnologyList))

                Return result

            End Get
        End Property
        Public ReadOnly Property TechListAdjustedCoolingW_ElectricalVentilation As Double Implements ISSMCalculate.TechListAdjustedCoolingW_ElectricalVentilation
            Get

                '=IF('TECH LIST INPUT'!Q92>0,MIN('TECH LIST INPUT'!Q92,C40),MAX('TECH LIST INPUT'!Q92,-C40))

                Dim gen As ISSMGenInputs = ssmTOOL.GenInputs
                Dim tl As ISSMTechList = ssmTOOL.TechList

                'Dim TLQ92 As Double =  tl.VCValueVariation'TECH LIST INPUT'!Q92
                'Dim C40 As Double   =  gen.BC_MaxPossibleBenefitFromTechnologyList

                Return If(tl.VCValueVariation > 0, _
                        Math.Min(tl.VCValueVariation, gen.BC_MaxPossibleBenefitFromTechnologyList), _
                        Math.Max(tl.VCValueVariation, -gen.BC_MaxPossibleBenefitFromTechnologyList))

            End Get
        End Property
        Public ReadOnly Property TechListAdjustedCoolingW_FuelFiredHeating As Double Implements ISSMCalculate.TechListAdjustedCoolingW_FuelFiredHeating
            Get
                Return 0
            End Get
        End Property

        Public ReadOnly Property TechListAdjustedVentilationW_Mechanical As Double Implements ISSMCalculate.TechListAdjustedVentilationW_Mechanical
            Get
                Return Nothing
            End Get
        End Property
        Public ReadOnly Property TechListAdjustedVentilationW_ElectricalCoolingHeating As Double Implements ISSMCalculate.TechListAdjustedVentilationW_ElectricalCoolingHeating
            Get
                Return Nothing
            End Get
        End Property
        Public ReadOnly Property TechListAdjustedVentilationW_ElectricalVentilation As Double Implements ISSMCalculate.TechListAdjustedVentilationW_ElectricalVentilation
            Get
                '=IF('TECH LIST INPUT'!P92>0,MIN('TECH LIST INPUT'!P92,C40),MAX('TECH LIST INPUT'!P92,-C40))

                Dim gen As ISSMGenInputs = ssmTOOL.GenInputs
                Dim tl As ISSMTechList = ssmTOOL.TechList

                'Dim TLP92 As Double =  tl.VVValueVariation  'TECH LIST INPUT'!P92
                'Dim C40 As Double   =  gen.BC_MaxPossibleBenefitFromTechnologyList


                Return If(tl.VVValueVariation > 0, Math.Min(tl.VVValueVariation, gen.BC_MaxPossibleBenefitFromTechnologyList), Math.Max(tl.VVValueVariation, -gen.BC_MaxPossibleBenefitFromTechnologyList))

            End Get
        End Property
        Public ReadOnly Property TechListAdjustedVentilationW_FuelFiredHeating As Double Implements ISSMCalculate.TechListAdjustedVentilationW_FuelFiredHeating
            Get
                Return 0
            End Get
        End Property

#End Region

        'Provides Diagnostic Information for the user which can be displayed on the form.
        'Based on the inputs generated, can be used to cross reference the Excel Model with the
        'Outputs generated here.
        Public Overrides Function ToString() As String

            Dim sb As New StringBuilder()

            sb.AppendLine("")
            sb.AppendLine("TechList Detail")
            sb.AppendLine("***********************")

            Dim nameLength As Integer = 40
            Dim catLength As Integer = 15
            Dim unitLength As Integer = 15
            Dim firstValuePos As Integer = nameLength + catLength + unitLength + 2
            Dim cat As String
            Dim name As String
            Dim units As String

            sb.AppendLine(String.Format(Space(firstValuePos) + "H{0}VH{0}VV{0}VC{0}C{0}", vbtab))


            For Each line As ITechListBenefitLine In ssmTOOL.TechList.TechLines

                With line

                    Dim extraNameSpaces, extraCatSpaces, extraUnitSpaces As Integer

                    extraNameSpaces = nameLength - .BenefitName.Length
                    extraCatSpaces = catLength - .Category.Length
                    extraUnitSpaces = unitLength - .Units.Length

                    cat = line.Category.Substring(0, Math.Min(line.Category.Length, catLength)) + Space(If(extraCatSpaces < 0, 0, extraCatSpaces)).Replace(" ", ".")
                    name = line.BenefitName.Substring(0, Math.Min(line.BenefitName.Length, nameLength)) + Space(If(extraNameSpaces < 0, 0, extraNameSpaces)).Replace(" ", ".")
                    units = line.Units.Substring(0, Math.Min(line.Units.Length, unitLength)) + Space(If(extraUnitSpaces < 0, 0, extraUnitSpaces)).Replace(" ", ".")

                    sb.AppendLine(String.Format(Units + cat + name + " {0}{1}{0}{2}{0}{3}{0}{4}{0}{5}", vbtab, .H.ToString("0.000"), .VH.ToString("0.000"), .VV.ToString("0.000"), .VC.ToString("0.000"), .C.ToString("0.000")))

                End With

            Next

            sb.AppendLine("")
            sb.AppendLine("TechList Totals")
            sb.AppendLine("***********************")

            With ssmTOOL.TechList

                sb.AppendLine(vbTab + vbTab + "H" + vbTab + "VH" + vbTab + "VV" + vbTab + "VC" + vbTab + "C")
                sb.AppendLine(String.Format("Base Var %   {0}{1}{0}{2}{0}{3}{0}{4}{0}{5}", vbtab, .HValueVariation.ToString("0.000"), .VHValueVariation.ToString("0.000"), .VVValueVariation.ToString("0.000"), .VCValueVariation.ToString("0.000"), .CValueVariation.ToString("0.000")))
                sb.AppendLine(String.Format("Base Var KW  {0}{1}{0}{2}{0}{3}{0}{4}{0}{5}", vbtab, .HValueVariationKW.ToString("0.000"), .VHValueVariationKW.ToString("0.000"), .VVValueVariationKW.ToString("0.000"), .VCValueVariationKW.ToString("0.000"), .CValueVariationKW.ToString("0.000")))

            End With



            'Runs
            sb.AppendLine(Run1.ToString())
            sb.AppendLine(Run2.ToString())

            'Staging Calcs
            sb.AppendLine("Staging Base Values")
            sb.AppendLine("*******************")
            sb.AppendLine(vbTab + vbTab + vbTab + "Mechanical" + vbTab + "Elec Cool/Heat" + vbTab + "Elec Vent" + vbTab + "Fuel Fired Heating")

            sb.AppendLine(String.Format("Heating   {0}{1}{0}{2}{0}{3}{0}{4}", vbTab + vbtab, BaseHeatingW_Mechanical.ToString("0.00"), BaseHeatingW_ElectricalCoolingHeating.ToString("0.00"), BaseHeatingW_ElectricalVentilation.ToString("0.00"), BaseHeatingW_FuelFiredHeating.ToString("0.00")))
            sb.AppendLine(String.Format("Cooling   {0}{1}{0}{2}{0}{3}{0}{4}", vbTab + vbtab, BaseCoolingW_Mechanical.ToString("0.00"), BaseCoolingW_ElectricalCoolingHeating.ToString("0.00"), BaseCoolingW_ElectricalVentilation.ToString("0.00"), BaseCoolingW_FuelFiredHeating.ToString("0.00")))
            sb.AppendLine(String.Format("Ventilate {0}{1}{0}{2}{0}{3}{0}{4}", vbTab + vbtab, BaseVentilationW_Mechanical.ToString("0.00"), BaseVentilationW_ElectricalCoolingHeating.ToString("0.00"), BaseVentilationW_ElectricalVentilation.ToString("0.00"), BaseVentilationW_FuelFiredHeating.ToString("0.00")))

            sb.AppendLine("")
            sb.AppendLine("Staging Adjusted Values")
            sb.AppendLine("***********************")

            sb.AppendLine(String.Format("Heating   {0}{1}{0}{2}{0}{3}{0}{4}", vbTab + vbtab, TechListAdjustedHeatingW_Mechanical.ToString("0.00"), TechListAdjustedHeatingW_ElectricalCoolingHeating.ToString("0.00"), TechListAdjustedHeatingW_ElectricalVentilation.ToString("0.00"), TechListAdjustedHeatingW_FuelFiredHeating.ToString("0.00")))
            sb.AppendLine(String.Format("Cooling   {0}{1}{0}{2}{0}{3}{0}{4}", vbTab + vbtab, TechListAdjustedCoolingW_Mechanical.ToString("0.00"), TechListAdjustedCoolingW_ElectricalCoolingHeating.ToString("0.00"), TechListAdjustedCoolingW_ElectricalVentilation.ToString("0.00"), TechListAdjustedCoolingW_FuelFiredHeating.ToString("0.00")))
            sb.AppendLine(String.Format("Ventilate {0}{1}{0}{2}{0}{3}{0}{4}", vbTab + vbtab, TechListAdjustedVentilationW_Mechanical.ToString("0.00"), TechListAdjustedVentilationW_ElectricalCoolingHeating.ToString("0.00"), TechListAdjustedVentilationW_ElectricalVentilation.ToString("0.00"), TechListAdjustedVentilationW_FuelFiredHeating.ToString("0.00")))



            Return sb.ToString()

        End Function

        Private Function CalculateElectricalWBase(genInputs As ISSMGenInputs, EnviromentalTemperature As Double, Solar As Double, Weight As Single) As Single

            'MIN(SUM(H94),C54*1000)/C59+SUM(I93:I95)

            genInputs.EC_EnviromentalTemperature = EnviromentalTemperature
            genInputs.EC_Solar = Solar

            'Dim H94 = BaseCoolingW_ElectricalCoolingHeating
            'Dim C54 = genInputs.AC_CompressorCapacitykW
            'Dim C59 = genInputs.AC_COP
            'Dim I93 = BaseHeatingW_ElectricalVentilation
            'Dim I94 = BaseCoolingW_ElectricalVentilation
            'Dim I95 = BaseVentilationW_ElectricalVentilation

            Dim ElectricalWBaseCurrentResult As Double = Math.Min(BaseCoolingW_ElectricalCoolingHeating, genInputs.AC_CompressorCapacitykW * 1000) / genInputs.AC_COP + BaseHeatingW_ElectricalVentilation + BaseCoolingW_ElectricalVentilation + BaseVentilationW_ElectricalVentilation

            Return ElectricalWBaseCurrentResult * Weight

        End Function

        Private Function CalculateMechanicalWBase(genInputs As ISSMGenInputs, EnviromentalTemperature As Double, Solar As Double, Weight As Single) As Single

            '=MIN(F94,C54*1000)/C59

            genInputs.EC_EnviromentalTemperature = EnviromentalTemperature
            genInputs.EC_Solar = Solar

            'Dim F94 = BaseCoolingW_Mechanical
            'Dim C54 = genInputs.AC_CompressorCapacitykW
            'Dim C59 = genInputs.AC_COP 

            Dim MechanicalWBaseCurrentResult As Double = Math.Min(BaseCoolingW_Mechanical, genInputs.AC_CompressorCapacitykW * 1000) / genInputs.AC_COP

            Return MechanicalWBaseCurrentResult * Weight

        End Function

        Private Function CalculateFuelLPerHBase(genInputs As ISSMGenInputs, EnviromentalTemperature As Double, Solar As Double, Weight As Single) As Single

            '=(MIN(ABS(J93/1000),C71)/C37)*(1/(C39*C38))

            genInputs.EC_EnviromentalTemperature = EnviromentalTemperature
            genInputs.EC_Solar = Solar

            'Dim J93 = BaseHeatingW_FuelFiredHeating
            'Dim C71 = genInputs.AH_FuelFiredHeaterkW
            'Dim C37 = genInputs.BC_AuxHeaterEfficiency
            'Dim C39 = ssmTOOL.HVACConstants.FuelDensity
            'Dim C38 = genInputs.BC_GCVDieselOrHeatingOil

            Dim FuelLPerHBaseCurrentResult As Double = (Math.Min(Math.Abs(BaseHeatingW_FuelFiredHeating / 1000), genInputs.AH_FuelFiredHeaterkW) / genInputs.BC_AuxHeaterEfficiency) * (1 / (genInputs.BC_GCVDieselOrHeatingOil * ssmTOOL.HVACConstants.FuelDensity))

            Return FuelLPerHBaseCurrentResult * Weight

        End Function

        Private Function CalculateElectricalWAdjusted(genInputs As ISSMGenInputs, tecList As ISSMTechList, EnviromentalTemperature As Double, Solar As Double, Weight As Single) As Single

            '=(MIN((H94*(1-H100)),C54*1000)/C59)+(I93*(1-I99))+(I94*(1-I100))+(I95*(1-I101))
            
            genInputs.EC_EnviromentalTemperature = EnviromentalTemperature
            genInputs.EC_Solar = Solar

            Dim H94 As Double = BaseCoolingW_ElectricalCoolingHeating
            Dim H100 As Double = TechListAdjustedCoolingW_ElectricalCoolingHeating
            Dim C54 As Double = genInputs.AC_CompressorCapacitykW
            Dim C59 As Double = genInputs.AC_COP

            Dim I93 As Double = BaseHeatingW_ElectricalVentilation
            Dim I94 As Double = BaseCoolingW_ElectricalVentilation
            Dim I95 As Double = BaseVentilationW_ElectricalVentilation
            Dim I99 As Double = TechListAdjustedHeatingW_ElectricalVentilation
            Dim I100 As Double = TechListAdjustedCoolingW_ElectricalVentilation
            Dim I101 As Double = TechListAdjustedVentilationW_ElectricalVentilation

            Dim ElectricalWAdjusted As Single = (Math.Min((H94 * (1 - H100)), C54 * 1000) / C59) + (I93 * (1 - I99)) + (I94 * (1 - I100)) + (I95 * (1 - I101))

            Return ElectricalWAdjusted * Weight

        End Function

        Private Function CalculateMechanicalWBaseAdjusted(genInputs As ISSMGenInputs, tecList As ISSMTechList, EnviromentalTemperature As Double, Solar As Double, Weight As Single) As Single

            '=(MIN((F94*(1-F100)),C54*1000)/C59)
            
            genInputs.EC_EnviromentalTemperature = EnviromentalTemperature
            genInputs.EC_Solar = Solar

            Dim F94 As Double = BaseCoolingW_Mechanical
            Dim F100 As Double = TechListAdjustedCoolingW_Mechanical
            Dim C54 As Double = genInputs.AC_CompressorCapacitykW
            Dim C59 As Double = genInputs.AC_COP

            Dim MechanicalWBaseAdjusted As Single = (Math.Min((F94 * (1 - F100)), C54 * 1000) / C59)

            Return MechanicalWBaseAdjusted * Weight

        End Function

        Private Function CalculateFuelLPerHBaseAdjusted(genInputs As ISSMGenInputs, tecList As ISSMTechList, EnviromentalTemperature As Double, Solar As Double, Weight As Single) As Single

            '=MIN(ABS(IF(AND(M89<0,M90<0),VLOOKUP(MAX(M89:M90),M89:P90,4),0)/1000),C71)/C37*(1/(C39*C38))

            genInputs.EC_EnviromentalTemperature = EnviromentalTemperature
            genInputs.EC_Solar = Solar

            'Dim M89 = Run1.TotalW
            'Dim M90 = genInputs.BC_GCVDieselOrHeatingOil
            'Dim C71 = genInputs.AH_FuelFiredHeaterkW
            'Dim C37 = genInputs.BC_AuxHeaterEfficiency
            'Dim C38 = genInputs.BC_GCVDieselOrHeatingOil
            'Dim C39 = ssmTOOL.HVACConstants.FuelDensity

            Dim result As Double = 0

            If Run1.TotalW < 0 AndAlso Run2.TotalW < 0 Then
                result = Math.Abs(If(Run1.TotalW > Run2.TotalW, Run1.TechListAmendedFuelW, Run2.TechListAmendedFuelW) / 1000)
            End If

            Dim FuelLPerHBaseAdjusted As Single = Math.Min(result, genInputs.AH_FuelFiredHeaterkW) / genInputs.BC_AuxHeaterEfficiency * (1 / (genInputs.BC_GCVDieselOrHeatingOil * ssmTOOL.HVACConstants.FuelDensity))

            Return FuelLPerHBaseAdjusted * Weight

        End Function
    End Class



End Namespace



