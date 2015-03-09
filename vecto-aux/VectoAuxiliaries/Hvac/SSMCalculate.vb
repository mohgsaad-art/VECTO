Namespace Hvac


Public Class SSMCalculate
  Implements ISSMCalculate


        'Private Fields
        Private  ssmTOOL   As ISSMTOOL
        Private  Property  Run1  As ISSMRun Implements ISSMCalculate.Run1
        Private  Property  Run2  As ISSMRun Implements ISSMCalculate.Run2

        'Constructor
        Sub new( ssmTool As ISSMTOOL)

          Me.ssmTOOL = ssmTool
          Run1 = New SSMRun(Me.ssmTOOL,1)
          Run2 = New SSMRun(Me.ssmTOOL,2)

        End Sub

        #Region "Main Outputs"

        'BASE RESULTS
        Public ReadOnly Property ElectricalWBase As Single Implements ISSMCalculate.ElectricalWBase
            Get
            '=(SUM(H84)/C33)+SUM(I83:I85)

            Dim gen As ISSMGenInputs = ssmTOOL.GenInputs

            'Dim H84  As Double = BaseHeatingW_ElectricalCoolingHeating
            'Dim C33  As Double = gen.BC_COP   
            Dim BaseVentilation As Double  =  BaseHeatingW_ElectricalVentilation+ BaseCoolingW_ElectricalVentilation + BaseVentilationW_ElectricalVentilation   ' SUM(I83:I85)        

            Return  (BaseHeatingW_ElectricalCoolingHeating/gen.BC_COP)+BaseVentilation


            End Get
        End Property
        Public ReadOnly Property MechanicalWBase As Single Implements ISSMCalculate.MechanicalWBase
            Get
            '=F84/C33

             Dim gen As ISSMGenInputs = ssmTOOL.GenInputs
            
            'Dim F84 As Double = BaseCoolingW_Mechanical
            'Dim C33  As Double = gen.BC_COP 

             Return BaseCoolingW_Mechanical/gen.BC_COP 

            End Get
        End Property
        Public ReadOnly Property FuelLPerHBase As Single Implements ISSMCalculate.FuelLPerHBase
            Get

            '=ABS((J83/1000)*(1/(C36*C35))/C34)

            Dim gen As ISSMGenInputs = ssmTOOL.GenInputs
            'Dim J83 As Double = BaseCoolingW_FuelFiredHeating
            'Dim C34 As Double = gen.BC_AuxHeaterEfficiency
            'Dim C35 As Double = gen.BC_GCVDieselOrHeatingOil
            'Dim C36 As Double = gen.BC_VolumicMassDieselOrHeatingOil

            Return Math.Abs((BaseCoolingW_FuelFiredHeating/1000)*(1/(gen.BC_VolumicMassDieselOrHeatingOil*gen.BC_GCVDieselOrHeatingOil))/gen.BC_AuxHeaterEfficiency)



            End Get
        End Property


        'ADJUSTED RESULTS
        Public ReadOnly Property ElectricalWAdjusted As Single Implements ISSMCalculate.ElectricalWAdjusted
            Get
            '=((H84*(1-H90))/C33)+(I83*(1-I89))+(I84*(1-I90))+(I85*(1-I91)) + IF('TECH LIST INPUT'!D36="electrical",-'TECH LIST INPUT'!R93*1000,0)

               Dim gen As ISSMGenInputs = ssmTOOL.GenInputs
               Dim tl As ISSMTechList = ssmTOOL.TechList

               Dim DACElectrical As Boolean = tl.TechLines.Where( Function(f) f.LineType= TechLineType.DriverACElectrical).Count()=1
               Dim AdjustedAddition As Double =  If( Not DACElectrical,0, tl.CValueVariationKW*1000)

              'Dim H84  As Double = BaseHeatingW_ElectricalCoolingHeating
              'Dim H90  As Double = TechListAdjustedCoolingW_ElectricalCoolingHeating
              'Dim C33  As Double = gen.BC_COP 
              
              'Dim I83  As Double = BaseHeatingW_ElectricalVentilation
              'Dim I84  As Double = BaseCoolingW_ElectricalVentilation
              'Dim I85  As Double = BaseVentilationW_ElectricalVentilation
              'Dim I89  As Double = TechListAdjustedHeatingW_ElectricalVentilation
              'Dim I90  As Double = TechListAdjustedCoolingW_ElectricalVentilation
              'Dim I91  As Double = TechListAdjustedVentilationW_ElectricalVentilation


              Return ((BaseHeatingW_ElectricalCoolingHeating*(1-TechListAdjustedCoolingW_ElectricalCoolingHeating))/gen.BC_COP) + _
                            (BaseHeatingW_ElectricalVentilation*(1-TechListAdjustedHeatingW_ElectricalVentilation))+ _ 
                            (BaseCoolingW_ElectricalVentilation*(1-TechListAdjustedCoolingW_ElectricalVentilation))+ _ 
                            (BaseVentilationW_ElectricalVentilation*(1-TechListAdjustedVentilationW_ElectricalVentilation)) +
                            AdjustedAddition


            End Get
        End Property
        Public ReadOnly Property MechanicalWBaseAdjusted As Single Implements ISSMCalculate.MechanicalWBaseAdjusted
            Get

              '=(F84*(1-F90)/C33) + IF('TECH LIST INPUT'!D36="mechanical",-'TECH LIST INPUT'!R93*1000,0)

               Dim gen As ISSMGenInputs = ssmTOOL.GenInputs
               Dim tl As ISSMTechList = ssmTOOL.TechList

               Dim DACMechanical As Boolean = tl.TechLines.Where( Function(f) f.LineType= TechLineType.DriverACMechanical).Count()=1
               Dim AdjustedAddition As Double =  If( Not DACMechanical,0, tl.CValueVariationKW*1000)

               Dim F84 As Double  = BaseCoolingW_Mechanical
               Dim F90 As Double  = TechListAdjustedCoolingW_Mechanical
               Dim C33 As Double  = gen.BC_COP

               Return (F84*(1-F90)/C33) + AdjustedAddition

            End Get

        End Property
        Public ReadOnly Property FuelLPerHBaseAdjusted As Single Implements ISSMCalculate.FuelLPerHBaseAdjusted
            Get
            '=ABS((IF(AND(M79<0,M80<0),VLOOKUP(MAX(M79:M80),M79:P80,4),0)/1000)*(1/(C36*C35))/C34)

             Dim gen As ISSMGenInputs = ssmTOOL.GenInputs

             Dim C34 As Double = gen.BC_AuxHeaterEfficiency
             Dim C35 As Double = gen.BC_GCVDieselOrHeatingOil
             Dim C36 As Double = gen.BC_VolumicMassDieselOrHeatingOil
             Dim result As Double

             If  Run1.TotalW<0 AndAlso Run1.TotalW<0 then
                 result = If(Run1.TotalW>Run2.TotalW, Run1.TechListAmendedFuelW, Run2.TechListAmendedFuelW)/1000
                Else
                 result = 0
             End If
            
             Return Math.Abs( result * (1/(gen.BC_VolumicMassDieselOrHeatingOil*gen.BC_GCVDieselOrHeatingOil))/gen.BC_AuxHeaterEfficiency  )
            
            End Get
        End Property


        #End Region

        #Region "Staging Calculations"

        'Base Values
        Public ReadOnly Property BaseHeatingW_Mechanical As Double Implements ISSMCalculate.BaseHeatingW_Mechanical
            Get
              Return nothing
            End Get
        End Property
        Public ReadOnly Property BaseHeatingW_ElectricalCoolingHeating As Double Implements ISSMCalculate.BaseHeatingW_ElectricalCoolingHeating
            Get
               Return  nothing
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
              
              res  = IF( Run1.TotalW <0 AndAlso Run2.TotalW<0  ,  _
                      IF(gen.VEN_VentilationONDuringHeating AndAlso gen.VEN_VentilationDuringHeating="high" ,  _
                                        gen.BC_HighVentPowerW,  _
                                        IF(gen.VEN_VentilationONDuringHeating AndAlso gen.VEN_VentilationDuringHeating="low",gen.BC_LowVentPowerW,0) ),  0)


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
               If( Run1.TotalW<0 AndAlso Run2.TotalW <0) then
               
                  return  If( Run1.TotalW > Run2.TotalW, Run1.FuelW, Run2.FuelW)
               
               Else   
                    
                 return   0
               
               End If
                    
            End Get

        End Property

        Public ReadOnly Property BaseCoolingW_Mechanical As Double Implements ISSMCalculate.BaseCoolingW_Mechanical
            Get
               '=IF(C48="mechanical", IF(AND(M79>0,M80>0),MIN(M79:M80),0),0)

               Dim gen As ISSMGenInputs = ssmTOOL.GenInputs

              'Dim C48 = gen.AC_CompressorType 
              'Dim M79 = Me.Run1.TotalW
              'Dim M80 = Me.Run2.TotalW


              Return IF(gen.AC_CompressorType.ToLower ="mechanical", IF((Run1.TotalW>0 AndAlso Run2.TotalW>0),Math.Min(Run1.TotalW,Run2.TotalW),0),0)



            End Get
        End Property
        Public ReadOnly Property BaseCoolingW_ElectricalCoolingHeating As Double Implements ISSMCalculate.BaseCoolingW_ElectricalCoolingHeating
            Get
            '=IF(C48="mechanical",0,IF(AND(M79>0,M80>0),MIN(M79:M80),0))

               Dim gen As ISSMGenInputs = ssmTOOL.GenInputs

               'Dim C48 = gen.AC_CompressorType 
               'Dim M79 = Me.Run1.TotalW
               'Dim M80 = Me.Run2.TotalW


               Return IF(gen.AC_CompressorType .ToLower="mechanical",0,IF((Run1.TotalW>0 AndAlso Run2.TotalW>0),Math.Min(Run1.TotalW,Run2.TotalW),0))


            End Get
        End Property
        Public ReadOnly Property BaseCoolingW_ElectricalVentilation As Double Implements ISSMCalculate.BaseCoolingW_ElectricalVentilation
            Get
             '=IF(AND(M79>0,M80>0),IF(AND(C54="yes",C57="high"),C30,IF(AND(C54="yes",C57="low"),C31,0)),0)

               Dim gen As ISSMGenInputs = ssmTOOL.GenInputs

               'Dim C30 = gen.BC_HighVentPowerW
               'Dim C31 = gen.BC_LowVentPowerW
               'Dim C54 = gen.VEN_VentilationDuringAC
               'Dim C57 = gen.VEN_VentilationDuringCooling
               'Dim M79 = Me.Run1.TotalW
               'Dim M80 = Me.Run2.TotalW


               Return IF(Run1.TotalW>0 AndAlso Run2.TotalW>0, _
                           IF(gen.VEN_VentilationDuringAC AndAlso gen.VEN_VentilationDuringCooling.ToLower="high",gen.BC_HighVentPowerW, _
                                    IF(gen.VEN_VentilationDuringAC AndAlso gen.VEN_VentilationDuringCooling.ToLower="low",gen.BC_LowVentPowerW,0)),0)


            End Get
        End Property
        Public ReadOnly Property BaseCoolingW_FuelFiredHeating As Double Implements ISSMCalculate.BaseCoolingW_FuelFiredHeating
            Get
               Return  0
            End Get
        End Property

        Public ReadOnly Property BaseVentilationW_Mechanical As Double Implements ISSMCalculate.BaseVentilationW_Mechanical
            Get
             Return nothing
            End Get
        End Property
        Public ReadOnly Property BaseVentilationW_ElectricalCoolingHeating As Double Implements ISSMCalculate.BaseVentilationW_ElectricalCoolingHeating
            Get
              Return nothing
            End Get
        End Property
        Public ReadOnly Property BaseVentilationW_ElectricalVentilation As Double Implements ISSMCalculate.BaseVentilationW_ElectricalVentilation
            Get
             '=IF(AND(M79>0,M80<0),IF(AND(C53="yes",C55="high"),C30,IF(AND(C53="yes",C55="low"),C31,0)),0)

             Dim gen As ISSMGenInputs = ssmTOOL.GenInputs

            'Dim C30 = gen.BC_HighVentPowerW
            'Dim C31 = gen.BC_LowVentPowerW
            'Dim C53 = gen.VEN_VentilationWhenBothHeatingAndACInactive
            'Dim C55 = gen.VEN_VentilationFlowSettingWhenHeatingAndACInactive
            'Dim M79 = Me.Run1.TotalW
            'Dim M80 = Me.Run2.TotalW


             Return IF((Run1.TotalW>0 AndAlso Run2.TotalW<0), IF(gen.VEN_VentilationWhenBothHeatingAndACInactive AndAlso gen.VEN_VentilationFlowSettingWhenHeatingAndACInactive.ToLower="high",gen.BC_HighVentPowerW, _
                               IF(gen.VEN_VentilationWhenBothHeatingAndACInactive AndAlso gen.VEN_VentilationFlowSettingWhenHeatingAndACInactive.ToLower="low",gen.BC_LowVentPowerW,0)),0)

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
             Return nothing
            End Get
        End Property
        Public ReadOnly Property TechListAdjustedHeatingW_ElectricalCoolingHeating As Double Implements ISSMCalculate.TechListAdjustedHeatingW_ElectricalCoolingHeating
            Get
             Return nothing
            End Get
        End Property
        Public ReadOnly Property TechListAdjustedHeatingW_ElectricalVentilation As Double Implements ISSMCalculate.TechListAdjustedHeatingW_ElectricalVentilation
            Get
            '=IF('TECH LIST INPUT'!O92>0,MIN('TECH LIST INPUT'!O92,C40),MAX('TECH LIST INPUT'!O92,-C40))
                Dim gen As ISSMGenInputs = ssmTOOL.GenInputs
                Dim tl As ISSMTechList   = ssmTOOL.TechList

             'TECH LIST INPUT'!O92
             'Dim C40 As Double   =  gen.BC_MaxPossibleBenefitFromTechnologyList
             'Dim TLO92 As Double = tl.VHValueVariation


             Return IF(tl.VHValueVariation>0,Math.MIN(tl.VHValueVariation,gen.BC_MaxPossibleBenefitFromTechnologyList),Math.MAX(tl.VHValueVariation,-gen.BC_MaxPossibleBenefitFromTechnologyList))

            End Get

        End Property
        Public ReadOnly Property TechListAdjustedHeatingW_FuelFiredHeating As Double Implements ISSMCalculate.TechListAdjustedHeatingW_FuelFiredHeating
            Get
            '=IF('TECH LIST INPUT'!N92>0,MIN('TECH LIST INPUT'!N92,C40),MAX('TECH LIST INPUT'!N92,-C40))


              Dim gen As ISSMGenInputs = ssmTOOL.GenInputs
              Dim tl As ISSMTechList   = ssmTOOL.TechList

             'TECH LIST INPUT'!N92
             'Dim C40 As Double   =  gen.BC_MaxPossibleBenefitFromTechnologyList
             'Dim TLN92 As Double =  tl.HValueVariation


             Return IF(tl.HValueVariation>0,Math.MIN(tl.HValueVariation,gen.BC_MaxPossibleBenefitFromTechnologyList),Math.MAX(tl.HValueVariation,-gen.BC_MaxPossibleBenefitFromTechnologyList))


            End Get

        End Property

        Public ReadOnly Property TechListAdjustedCoolingW_Mechanical As Double Implements ISSMCalculate.TechListAdjustedCoolingW_Mechanical
            Get
              '=IF(IF(C48="mechanical",'TECH LIST INPUT'!R92,0)>0,MIN(IF(C48="mechanical",'TECH LIST INPUT'!R92,0),C40),MAX(IF(C48="mechanical",'TECH LIST INPUT'!R92,0),-C40))

              Dim gen As ISSMGenInputs = ssmTOOL.GenInputs
              Dim tl As ISSMTechList   = ssmTOOL.TechList

              'Dim TLR92 As Double =  tl.CValueVariation 'TECH LIST INPUT'!R92
              'Dim C40 As Double   =  gen.BC_MaxPossibleBenefitFromTechnologyList
              'Dim C48 As string   =  gen.AC_CompressorType

              Return IF(   IF(gen.AC_CompressorType.ToLower="mechanical",tl.CValueVariation,0)>0, _
                              Math.MIN(IF(gen.AC_CompressorType="mechanical",tl.CValueVariation,0),gen.BC_MaxPossibleBenefitFromTechnologyList), _
                              Math.MAX(IF(gen.AC_CompressorType="mechanical",tl.CValueVariation,0),-gen.BC_MaxPossibleBenefitFromTechnologyList))

            End Get
        End Property
        Public ReadOnly Property TechListAdjustedCoolingW_ElectricalCoolingHeating As Double Implements ISSMCalculate.TechListAdjustedCoolingW_ElectricalCoolingHeating
            Get
            '=IF(IF(C48="mechanical",0,'TECH LIST INPUT'!R92)>0,MIN(IF(C48="mechanical",0,'TECH LIST INPUT'!R92),C40),MAX(IF(C48="mechanical",0,'TECH LIST INPUT'!R92),-C40))

            
              Dim gen As ISSMGenInputs = ssmTOOL.GenInputs
              Dim tl As ISSMTechList   = ssmTOOL.TechList

              'Dim TLR92 As Double =  tl.CValueVariation 'TECH LIST INPUT'!R92
              'Dim C40 As Double   =  gen.BC_MaxPossibleBenefitFromTechnologyList
              'Dim C48 As string   =  gen.AC_CompressorType

              Return IF(IF(gen.AC_CompressorType.ToLower="mechanical",0,tl.CValueVariation)>0, _
                            Math.MIN(IF(gen.AC_CompressorType.ToLower="mechanical",0,tl.CValueVariation),gen.BC_MaxPossibleBenefitFromTechnologyList), _
                            Math.MAX(IF(gen.AC_CompressorType.ToLower="mechanical",0,tl.CValueVariation),-gen.BC_MaxPossibleBenefitFromTechnologyList))


            End Get
        End Property
        Public ReadOnly Property TechListAdjustedCoolingW_ElectricalVentilation As Double Implements ISSMCalculate.TechListAdjustedCoolingW_ElectricalVentilation
            Get

              '=IF('TECH LIST INPUT'!Q92>0,MIN('TECH LIST INPUT'!Q92,C40),MAX('TECH LIST INPUT'!Q92,-C40))
                      
              Dim gen As ISSMGenInputs = ssmTOOL.GenInputs
              Dim tl As ISSMTechList   = ssmTOOL.TechList

              'Dim TLQ92 As Double =  tl.VCValueVariation'TECH LIST INPUT'!Q92
              'Dim C40 As Double   =  gen.BC_MaxPossibleBenefitFromTechnologyList

               Return IF(tl.VCValueVariation>0, _
                      Math.MIN(tl.VCValueVariation, gen.BC_MaxPossibleBenefitFromTechnologyList), _
                      Math.MAX(tl.VCValueVariation,-gen.BC_MaxPossibleBenefitFromTechnologyList))

            End Get
        End Property
        Public ReadOnly Property TechListAdjustedCoolingW_FuelFiredHeating As Double Implements ISSMCalculate.TechListAdjustedCoolingW_FuelFiredHeating
            Get
             Return 0
            End Get
        End Property

        Public ReadOnly Property TechListAdjustedVentilationW_Mechanical As Double Implements ISSMCalculate.TechListAdjustedVentilationW_Mechanical
            Get
             Return nothing
            End Get
        End Property
        Public ReadOnly Property TechListAdjustedVentilationW_ElectricalCoolingHeating As Double Implements ISSMCalculate.TechListAdjustedVentilationW_ElectricalCoolingHeating
            Get
             Return nothing
            End Get
        End Property
        Public ReadOnly Property TechListAdjustedVentilationW_ElectricalVentilation As Double Implements ISSMCalculate.TechListAdjustedVentilationW_ElectricalVentilation
            Get
            '=IF('TECH LIST INPUT'!P92>0,MIN('TECH LIST INPUT'!P92,C40),MAX('TECH LIST INPUT'!P92,-C40))

              Dim gen As ISSMGenInputs = ssmTOOL.GenInputs
              Dim tl As ISSMTechList   = ssmTOOL.TechList

              'Dim TLP92 As Double =  tl.VVValueVariation  'TECH LIST INPUT'!P92
              'Dim C40 As Double   =  gen.BC_MaxPossibleBenefitFromTechnologyList


              Return IF(tl.VVValueVariation>0,Math.MIN(tl.VVValueVariation,gen.BC_MaxPossibleBenefitFromTechnologyList),Math.MAX(tl.VVValueVariation,-gen.BC_MaxPossibleBenefitFromTechnologyList))

            End Get
        End Property
        Public ReadOnly Property TechListAdjustedVentilationW_FuelFiredHeating As Double Implements ISSMCalculate.TechListAdjustedVentilationW_FuelFiredHeating
            Get
              Return 0
            End Get
        End Property

        #End Region 


End Class



End Namespace



