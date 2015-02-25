Imports NUnit.Framework
Imports VectoAuxiliaries.Pneumatics
Imports VectoAuxiliariesTests.Mocks
Imports VectoAuxiliaries
Imports VectoAuxiliaries.Hvac


Namespace UnitTests


<TestFixture()>
Public Class _SSMTOOLTests

  <Test()> _
  <TestCase("BusParameterisation")> _
  <TestCase("BoundaryConditions")> _
  <TestCase("EnvironmentalConditions")> _
  <TestCase("AC-System")> _
  <TestCase("Ventilation")> _
  <TestCase("AuxHeater")> _
  Public Sub InstantiateDefaultSSMGenInputsTest( section As string)

     Dim target As ISSMGenInputs = New SSMGenInputs(true)

     If section="BusParameterisation" then




       'BUS Parameterisation
       '********************
        Assert.AreEqual(47              ,target.BP_NumberOfPassengers                           )
        Assert.AreEqual("raised floor"  ,target.BP_BusFloorType                                 )
        Assert.AreEqual(24.1102486R     ,target.BP_BusFloorSurfaceArea,2                        )
        Assert.AreEqual(114.42325R      ,target.BP_BusSurfaceAreaM2                             )
        Assert.AreEqual(20.98R          ,Math.Round(target.BP_BusWindowSurface,2)               )
        Assert.AreEqual(61.81231875R    ,target.BP_BusVolume                                    )
        Assert.AreEqual(10.655R         ,target.BP_BusLength                                    )
        Assert.AreEqual(2.55R           ,target.BP_BusWidth                                     )

     End If

     If section = "BoundaryConditions" Then
        'BOUNDRY CONDITIONS
        '******************
         Assert.AreEqual(1.0R           ,target.BC_GFactor                               )
         Assert.AreEqual(0.8R           ,target.BC_SolarClouding                         )
         Assert.AreEqual(80             ,target.BC_HeatPerPassengerIntoCabinW            )
         Assert.AreEqual(13             ,target.BC_PassengerBoundaryTemperature          )
         Assert.AreEqual(3.0R           ,target.BC_PassengerDensityLowFloor              )
         Assert.AreEqual(2.0R           ,target.BC_PassengerDensitySemiLowFloor          )
         Assert.AreEqual(1.4R           ,target.BC_PassengerDensityRaisedFloor           )
         Assert.AreEqual(33.7543R       ,Math.Round(target.BC_CalculatedPassengerNumber,4))
         Assert.AreEqual(3.0R           ,target.BC_UValues                               )
         Assert.AreEqual(20             ,target.BC_HeatingBoundaryTemperature            )
         Assert.AreEqual(24             ,target.BC_CoolingBoundaryTemperature            )
         Assert.AreEqual(25             ,target.BC_HighVentilation                       )
         Assert.AreEqual(8              ,target.BC_lowVentilation                        )
         Assert.AreEqual(1545.30796875R ,target.BC_High                                  )
         Assert.AreEqual(494.49855000   ,target.BC_Low                                   )
         Assert.AreEqual(927.18478125   ,target.BC_HighVentPowerW                        )
         Assert.AreEqual(296.69913000R  ,target.BC_LowVentPowerW                         )
         Assert.AreEqual(0.6R           ,target.BC_SpecificVentilationPower              )
         Assert.AreEqual(4              ,target.BC_COP                                   )
         Assert.AreEqual(1              ,target.BC_AuxHeaterEfficiency                   )
         Assert.AreEqual(13             ,target.BC_GCVDieselOrHeatingOil                 )
         Assert.AreEqual(1              ,target.BC_VolumicMassDieselOrHeatingOil         )
         Assert.AreEqual(1.5R           ,target.BC_WindowAreaPerUnitBusLength            )
         Assert.AreEqual(5              ,target.BC_FrontRearWindowArea                   )
         Assert.AreEqual(4              ,target.BC_MaxTemperatureDeltaForLowFloorBusses  )
         Assert.AreEqual(0.03R          ,target.BC_MaxPossibleBenefitFromTechnologyList  )
        
     End if    
           
     
     If section="EnvironmentalConditions" Then
                                                   
       'Environmental Conditions
       '************************
        Assert.AreEqual(  25.00 ,target.EC_EnviromentalTemperature                )
        Assert.AreEqual( 400.00 ,target.EC_Solar                                  )

     End If

     If section="AC-System" Then

        'AC-SYSTEM
        '*********
        Assert.AreEqual( True         , target.AC_InCabinRoomAC_System             )            
        Assert.AreEqual( "mechanical" , target.AC_CompressorType                   )            
        Assert.AreEqual( 18           , target.AC_CompressorCapacitykW             )            

     End If

     If section = "Ventilation" Then 

        'VENTILATION
        '***********                                                                            
        Assert.Areequal(True    ,target.VEN_VentilationOnDuringHeating                       )
        Assert.Areequal(True    ,target.VEN_VentilationWhenBothHeatingAndACInactive          )
        Assert.Areequal(True    ,target.VEN_VentilationDuringAC                              )
        Assert.Areequal("high"  ,target.VEN_VentilationFlowSettingWhenHeatingAndACInactive   )
        Assert.Areequal("high"  ,target.VEN_VentilationDuringHeating                         )
        Assert.Areequal("low"   ,target.VEN_VentilationDuringCooling                         )




     End If

     If section="AuxHeater" then
        'AUX HEATER
        '**********
        Assert.AreEqual( 2.0    ,target.AH_EngineWasteHeatkW                       )           
        Assert.AreEqual(10.0    ,target.AH_FuelFiredHeaterkW                       )           

     End If

  End Sub


Private Const GOODTechList As String = "TestFiles\SSMTechBenefits.csv"
Private Const GOODTechListALLON As String = "TestFiles\SSMTechBenefitsALLON.csv"
Private Const GOODTechListALLOFF As String = "TestFiles\SSMTechBenefitsALLOFF.csv"

<Test()>
Public Sub Instantiate_TechListTest()


     Dim gen As ISSMGenInputs = New SSMGenInputs(true)

     Dim target As ISSMTechList = New SSMTechList( GOODTechList , gen)

     
     Assert.IsTrue( target.Initialise())



End Sub

<Test()>
Public Sub Instantiate_TechListTestALLON()


     Dim gen As ISSMGenInputs = New SSMGenInputs(true)

     Dim target As ISSMTechList = New SSMTechList( GOODTechListALLON , gen)

       Assert.IsTrue(target.Initialise())

       Assert.AreEqual(0.142	  ,Math.Round(target.HValueVariation,3))
       Assert.AreEqual(0.006	  ,Math.Round(target.VHValueVariation,3))
       Assert.AreEqual(0.006	  ,Math.Round(target.VVValueVariation,3))
       Assert.AreEqual(0.006	  ,Math.Round(target.VCValueVariation,3))
       Assert.AreEqual(0.259	  ,Math.Round(target.CValueVariation,3))

       Assert.AreEqual(0.000	  ,Math.Round(target.VHValueVariationKW,3))
       Assert.AreEqual(0.000	  ,Math.Round(target.VVValueVariationKW,3))
       Assert.AreEqual(0.000	  ,Math.Round(target.VCValueVariationKW,3))
       Assert.AreEqual(0.000	  ,Math.Round(target.VCValueVariationKW,3))
       Assert.AreEqual(-0.200     ,Math.Round(target.CValueVariationKW,3))





End Sub

<Test()>
Public Sub Instantiate_TechListTestALLOFF()


     Dim gen As ISSMGenInputs = New SSMGenInputs(true)

     Dim target As ISSMTechList = New SSMTechList( GOODTechListALLOFF , gen)

       Assert.IsTrue(target.Initialise())

       Assert.AreEqual(0	  ,Math.Round(target.HValueVariation,3))
       Assert.AreEqual(0	  ,Math.Round(target.VHValueVariation,3))
       Assert.AreEqual(0	  ,Math.Round(target.VVValueVariation,3))
       Assert.AreEqual(0	  ,Math.Round(target.VCValueVariation,3))
       Assert.AreEqual(0	  ,Math.Round(target.CValueVariation,3))
                       
       Assert.AreEqual(0	  ,Math.Round(target.VHValueVariationKW,3))
       Assert.AreEqual(0	  ,Math.Round(target.VVValueVariationKW,3))
       Assert.AreEqual(0	  ,Math.Round(target.VCValueVariationKW,3))
       Assert.AreEqual(0	  ,Math.Round(target.VCValueVariationKW,3))
       Assert.AreEqual(0      ,Math.Round(target.CValueVariationKW,3))


End Sub


End Class




End Namespace



