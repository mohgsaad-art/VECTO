Namespace Hvac


Public Class SSMCalculate
  Implements ISSMCalculate


         Private ssmTOOL As ISSMTOOL


         private Property Run1 As ISSMRun Implements ISSMCalculate.Run1


        Public  Property Run2 As ISSMRun Implements ISSMCalculate.Run2


        Sub new( ssmTool As ISSMTOOL)

          Me.ssmTOOL = ssmTool
          Run1 = New SSMRun(Me.ssmTOOL,1)
          Run2 = New SSMRun(Me.ssmTOOL,2)

        End Sub



        'Base Values
        Public ReadOnly Property BaseHeatingW_Mechanical As Double Implements ISSMCalculate.BaseHeatingW_Mechanical
            Get
              Return nothing
            End Get
        End Property
        Public ReadOnly Property BaseHeatingW_ElectricalCoolingHeating As Double Implements ISSMCalculate.BaseHeatingW_ElectricalCoolingHeating
            Get

            End Get
        End Property
        Public ReadOnly Property BaseHeatingW_ElectricalVentilation As Double Implements ISSMCalculate.BaseHeatingW_ElectricalVentilation
            Get
             Return nothing
            End Get
        End Property
        Public ReadOnly Property BaseHeatingW_FuelFiredHeating As Double Implements ISSMCalculate.BaseHeatingW_FuelFiredHeating
            Get
            '=IF(AND(M79<0,M80<0),IF(AND(C52="yes",C56="high"),C30,IF(AND(C52="yes",C56="low"),C31,0)),0)

            Dim gen As ISSMGenInputs = ssmTOOL.GenInputs






            End Get
        End Property

        Public ReadOnly Property BaseCoolingW_Mechanical As Double Implements ISSMCalculate.BaseCoolingW_Mechanical
            Get

            End Get
        End Property
        Public ReadOnly Property BaseCoolingW_ElectricalCoolingHeating As Double Implements ISSMCalculate.BaseCoolingW_ElectricalCoolingHeating
            Get

            End Get
        End Property
        Public ReadOnly Property BaseCoolingW_ElectricalVentilation As Double Implements ISSMCalculate.BaseCoolingW_ElectricalVentilation
            Get

            End Get
        End Property
        Public ReadOnly Property BaseCoolingW_FuelFiredHeating As Double Implements ISSMCalculate.BaseCoolingW_FuelFiredHeating
            Get

            End Get
        End Property

        Public ReadOnly Property BaseVentilationW_Mechanical As Double Implements ISSMCalculate.BaseVentilationW_Mechanical
            Get

            End Get
        End Property
        Public ReadOnly Property BaseVentilationW_ElectricalCoolingHeating As Double Implements ISSMCalculate.BaseVentilationW_ElectricalCoolingHeating
            Get

            End Get
        End Property
        Public ReadOnly Property BaseVentilationW_ElectricalVentilation As Double Implements ISSMCalculate.BaseVentilationW_ElectricalVentilation
            Get

            End Get
        End Property
        Public ReadOnly Property BaseVentilationW_FuelFiredHeating As Double Implements ISSMCalculate.BaseVentilationW_FuelFiredHeating
            Get

            End Get
        End Property


        'Adjusted Values
        Public ReadOnly Property TechListAdjustedHeatingW_Mechanical As Double Implements ISSMCalculate.TechListAdjustedHeatingW_Mechanical
            Get

            End Get
        End Property
        Public ReadOnly Property TechListAdjustedHeatingW_ElectricalCoolingHeating As Double Implements ISSMCalculate.TechListAdjustedHeatingW_ElectricalCoolingHeating
            Get

            End Get
        End Property
        Public ReadOnly Property TechListAdjustedHeatingW_ElectricalVentilation As Double Implements ISSMCalculate.TechListAdjustedHeatingW_ElectricalVentilation
            Get

            End Get
        End Property
        Public ReadOnly Property TechListAdjustedHeatingW_FuelFiredHeating As Double Implements ISSMCalculate.TechListAdjustedHeatingW_FuelFiredHeating
            Get

            End Get
        End Property

        Public ReadOnly Property TechListAdjustedCoolingW_Mechanical As Double Implements ISSMCalculate.TechListAdjustedCoolingW_Mechanical
            Get

            End Get
        End Property
        Public ReadOnly Property TechListAdjustedCoolingW_ElectricalCoolingHeating As Double Implements ISSMCalculate.TechListAdjustedCoolingW_ElectricalCoolingHeating
            Get

            End Get
        End Property
        Public ReadOnly Property TechListAdjustedCoolingW_ElectricalVentilation As Double Implements ISSMCalculate.TechListAdjustedCoolingW_ElectricalVentilation
            Get

            End Get
        End Property
        Public ReadOnly Property TechListAdjustedCoolingW_FuelFiredHeating As Double Implements ISSMCalculate.TechListAdjustedCoolingW_FuelFiredHeating
            Get

            End Get
        End Property

        Public ReadOnly Property TechListAdjustedVentilationW_Mechanical As Double Implements ISSMCalculate.TechListAdjustedVentilationW_Mechanical
            Get

            End Get
        End Property
        Public ReadOnly Property TechListAdjustedVentilationW_ElectricalCoolingHeating As Double Implements ISSMCalculate.TechListAdjustedVentilationW_ElectricalCoolingHeating
            Get

            End Get
        End Property
        Public ReadOnly Property TechListAdjustedVentilationW_ElectricalVentilation As Double Implements ISSMCalculate.TechListAdjustedVentilationW_ElectricalVentilation
            Get

            End Get
        End Property
        Public ReadOnly Property TechListAdjustedVentilationW_FuelFiredHeating As Double Implements ISSMCalculate.TechListAdjustedVentilationW_FuelFiredHeating
            Get

            End Get
        End Property



End Class



End Namespace



