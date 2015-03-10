Imports System.Text
Imports Microsoft.VisualBasic

Namespace Hvac


Public Class SSMRun
  Implements ISSMRun


  Private ssmTOOL As ISSMTOOL
  private runNumber As Integer


 Sub New(ssm As ISSMTOOL, runNumber As Integer)

    If runNumber <> 1 AndAlso runNumber <> 2 Then Throw New ArgumentException("Run number must be either 1 or 2")

    Me.runNumber = runNumber
    ssmTOOL = ssm

 End Sub


        Public ReadOnly Property HVACOperation As Double Implements ISSMRun.HVACOperation
            Get
            '=IF(C43>C25,3,IF(C43<C24,1,2))
            'C43 = EC_Enviromental Temperature
            'C25 = BC_CoolingBoundary Temperature
            'C24 = BC_HeatingBoundaryTemperature

            Dim gen As ISSMGenInputs = ssmTOOL.GenInputs

            Return If(gen.EC_EnviromentalTemperature > gen.BC_CoolingBoundaryTemperature, 3, If(gen.EC_EnviromentalTemperature < gen.BC_HeatingBoundaryTemperature, 1, 2))

            End Get

        End Property
        Public ReadOnly Property TCalc As Double Implements ISSMRun.TCalc
            Get

            'C24 = BC_HeatingBoundaryTemperature
            'C25 = BC_CoolingBoundary Temperature
            'C6  = BP_BusFloorType
            'C43 = EC_Enviromental Temperature
            'C39 = BC_FontAndRearWindowArea

            Dim gen As ISSMGenInputs = ssmTOOL.GenInputs
            Dim returnVal As Double

             If runNumber = 1 Then '=C24

               returnVal = gen.BC_HeatingBoundaryTemperature

             Else   '=IF(C6="low floor",IF((C43-C25)<C39,C25,C43-3),C25)

                returnVal = If(gen.BP_BusFloorType = "low floor", If((gen.EC_EnviromentalTemperature - gen.BC_CoolingBoundaryTemperature) < gen.BC_FrontRearWindowArea, gen.BC_CoolingBoundaryTemperature, gen.EC_EnviromentalTemperature - 3), gen.BC_CoolingBoundaryTemperature)

             End If


             Return returnVal

            End Get
        End Property
        Public ReadOnly Property TemperatureDelta As Double Implements ISSMRun.TemperatureDelta
            Get
             '=C43-F79/F80
             'C43 = EC_Enviromental Temperature
             'F79/80 = Me.TCalc

              Dim gen As ISSMGenInputs = ssmTOOL.GenInputs
              Return gen.EC_EnviromentalTemperature - TCalc

            End Get
        End Property
        Public ReadOnly Property QWall As Double Implements ISSMRun.QWall
            Get
            '=I79*D8*C23  or '=I80*D8*C23
              'Translated to
            '=I79*C8*C23  or '=I80*C8*C23

            'C23 = BC_UValues
            'C8  = BP_BusSurfaceAreaM2
            'I78/I80 = Me.TemperatureDelta

             Dim gen As ISSMGenInputs = ssmTOOL.GenInputs

             Return TemperatureDelta * gen.BP_BusSurfaceAreaM2 * gen.BC_UValues

            End Get
        End Property
        Public ReadOnly Property WattsPerPass As Double Implements ISSMRun.WattsPerPass
            Get

              '=IF(D5="",C22,IF(E5="",IF(C22<D5,C22,D5),E5))*C17
                'Translated to
              '=IF(IF(C22<C5,C22,C5))*C17
                'Simplified to
              'Max( C22,C5 )

              'C5   = BP_NumberOfPassengers
              'C22  = BC_Calculated Passenger Number
              'C17  = BC_Heat Per Passenger into cabin


              Dim gen As ISSMGenInputs = ssmTOOL.GenInputs

              Return Math.Min(gen.BP_NumberOfPassengers, gen.BC_CalculatedPassengerNumber) * gen.BC_HeatPerPassengerIntoCabinW


            End Get
        End Property
        Public ReadOnly Property Solar As Double Implements ISSMRun.Solar
            Get
              '=C44*D9*C15*C16*0.25
                'Translated to 
              '=C44*C9*C15*C16*0.25

              'C44 = EC_Solar
              'C9  = BP_BusWindowSurfaceArea
              'C15 = BC_GFactor
              'C16 = BC_SolarClouding

              Dim gen As ISSMGenInputs = ssmTOOL.GenInputs


              Return gen.EC_Solar * gen.BP_BusWindowSurface * gen.BC_GFactor * gen.BC_SolarClouding * 0.25


            End Get
        End Property
        Public ReadOnly Property TotalW As Double Implements ISSMRun.TotalW
            Get

              '=SUM(J79:L79) or =SUM(J80:L80)             
                 'Tanslated to 
              '=Sum ( Me.Qwall	,Me.WattsPerPass,Me.Solar )

              Return Me.QWall + Me.WattsPerPass + Me.Solar

            End Get
        End Property
        Public ReadOnly Property TotalKW As Double Implements ISSMRun.TotalKW
            Get
              '=M79 or =M80  / (1000)

              Return Me.TotalW / 1000

            End Get
        End Property
        Public ReadOnly Property FuelW As Double Implements ISSMRun.FuelW
            Get
              '=IF(AND(N79<0,N79<(C60*-1)),N79-(C60*-1),0)*1000

               Dim gen As ISSMGenInputs = ssmTOOL.GenInputs

              'Dim N79  as Double =  TotalKW
              'Dim C60  As Double = gen.AH_EngineWasteHeatkW

              Return IF((TotalKW<0 AndAlso TotalKW<(gen.AH_EngineWasteHeatkW *-1)), _
                              TotalKW-(gen.AH_EngineWasteHeatkW*-1), _
                              0)  _
                              *1000

            End Get
        End Property
        Public ReadOnly Property TechListAmendedFuelW As Double Implements ISSMRun.TechListAmendedFuelW
            Get
            '=IF(IF(AND((N79*(1-$J$89))<0,(N79*(1-$J$89))<(C60*-1)),(N79*(1-$J$89))-(C60*-1),0)*1000<0,IF(AND((N79*(1-$J$89))<0,(N79*(1-$J$89))<(C60*-1)),(N79*(1-$J$89))-(C60*-1),0)*1000,0)

             Dim gen As ISSMGenInputs = ssmTOOL.GenInputs
             Dim TLFFH As Double = ssmTOOL.Calculate.TechListAdjustedHeatingW_FuelFiredHeating            
            'Dim C60 As Double = gen.AH_EngineWasteHeatkW
            'Dim N79 As Double = Me.TotalKW

            Return IF( IF(((TotalKW*(1-TLFFH))<0 AndAlso (TotalKW*(1-TLFFH))<(gen.AH_EngineWasteHeatkW*-1)), _
                     (TotalKW*(1-TLFFH))-(gen.AH_EngineWasteHeatkW*-1),0)*1000<0, _ 
                     IF(((TotalKW*(1-TLFFH))<0 AndAlso (TotalKW*(1-TLFFH))<(gen.AH_EngineWasteHeatkW*-1)),(TotalKW*(1-TLFFH))-(gen.AH_EngineWasteHeatkW*-1),0)*1000,0)

            End Get

        End Property



        Public Overrides Function ToString() As String

           Dim sb As New StringBuilder()

           sb.AppendLine(String.Format("Run : {0}", runNumber))
           sb.AppendLine(String.Format("************************************"))
           sb.AppendLine(String.Format("HVAC OP         " + vbTab + ": {0}", HVACOperation))
           sb.AppendLine(String.Format("TCALC           " + vbTab + ": {0}", TCalc))
           sb.AppendLine(String.Format("Tempurature D   " + vbTab + ": {0}", TemperatureDelta))
           sb.AppendLine(String.Format("QWall           " + vbTab + ": {0}", QWall))
           sb.AppendLine(String.Format("WattsPerPass    " + vbTab + ": {0}", WattsPerPass))
           sb.AppendLine(String.Format("Solar           " + vbTab + ": {0}", Solar))
           sb.AppendLine(String.Format("TotalW          " + vbTab + ": {0}", TotalW))
           sb.AppendLine(String.Format("TotalKW         " + vbTab + ": {0}", TotalKW))
           sb.AppendLine(String.Format("Fuel W          " + vbTab + ": {0}", FuelW))
           sb.AppendLine(String.Format("Fuel Tech Adj   " + vbTab + ": {0}", TechListAmendedFuelW))

      
           Return sb.ToString()


        End Function

End Class



End Namespace




