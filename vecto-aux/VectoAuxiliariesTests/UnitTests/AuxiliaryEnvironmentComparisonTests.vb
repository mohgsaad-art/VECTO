Imports System.Text
Imports NUnit.Framework
Imports NUnit
Imports VectoAuxiliaries
Imports VectoAuxiliaries.Electrics
Imports VectoAuxiliaries.Pneumatics
Imports VectoAuxiliaries.Hvac

Namespace UnitTests

<TestFixture()> 
Public Class AuxiliaryComparisonTests


    <Test()> 
    Public Sub BasicEqualCompareTest()

    'Arrange
    Dim auxFresh = New AuxillaryEnvironment("")
    Dim auxNow   = New AuxillaryEnvironment("")'auxFresh.ShallowCopy()
    Dim compareResult As Boolean
    
    'Act
    compareResult =  auxFresh.ConfigValuesAreTheSameAs( auxNow)
    
    Assert.AreEqual( True,compareResult )

    End Sub

    <Test()> 
    Public Sub BasicUnequalTest()

    'Arrange
    Dim auxFresh = New AuxillaryEnvironment("")
    Dim auxNow   = New AuxillaryEnvironment("")
    Dim compareResult As Boolean
    
    'Act
    auxNow.PneumaticUserInputsConfig.SmartRegeneration= Not auxNow.PneumaticUserInputsConfig.SmartRegeneration
    compareResult =  auxFresh.ConfigValuesAreTheSameAs( auxNow)
 
    Assert.AreEqual( false,compareResult )

    End Sub

    #Region "ElectricalUserConfig"

    <Test()> 
    <Category("ElectricalUserConfig")> _
    Public Sub ElectricalUserConfig_AlternatorGearEfficiency_UnequalTest()

    'Arrange
    Dim auxFresh = New AuxillaryEnvironment("")
    Dim auxNow   = New AuxillaryEnvironment("")
    Dim compareResult As Boolean
    auxNow.ElectricalUserInputsConfig.AlternatorGearEfficiency= If( auxNow.ElectricalUserInputsConfig.AlternatorGearEfficiency+0.1>1, 1, auxNow.ElectricalUserInputsConfig.AlternatorGearEfficiency+0.1)

    'Act
    compareResult = auxFresh.ConfigValuesAreTheSameAs( auxNow )

    Assert.AreEqual( false,compareResult )

    End Sub
    <Test()> 
    <Category("ElectricalUserConfig")> _
    Public Sub ElectricalUserConfig_AlternatorMap_UnequalTest()

    'Arrange
    Dim auxFresh = New AuxillaryEnvironment("")
    Dim auxNow   = New AuxillaryEnvironment("")
    Dim compareResult As Boolean
    auxNow.ElectricalUserInputsConfig.AlternatorMap = auxNow.ElectricalUserInputsConfig.AlternatorMap & "X"

    'Act
    compareResult = auxFresh.ConfigValuesAreTheSameAs( auxNow )

    Assert.AreEqual( false,compareResult )

    End Sub
    <Test()> 
    <Category("ElectricalUserConfig")> _
    Public Sub ElectricalUserConfig_DoorActuationTimeSecond_UnequalTest()

    'Arrange
    Dim auxFresh = New AuxillaryEnvironment("")
    Dim auxNow   = New AuxillaryEnvironment("")
    Dim compareResult As Boolean
    auxNow.ElectricalUserInputsConfig.DoorActuationTimeSecond+=1

    'Act
    compareResult = auxFresh.ConfigValuesAreTheSameAs( auxNow )

    Assert.AreEqual( false,compareResult )

    End Sub
    <Test()>
    <Category("ElectricalUserConfig")> _
    Public Sub ElectricalUserConfig_Consumers_Unequal_Count_UnequalTest()

    'Arrange
    Dim auxFresh = New AuxillaryEnvironment("")
    Dim auxNow   = New AuxillaryEnvironment("")
    Dim compareResult As Boolean
    auxNow.ElectricalUserInputsConfig.ElectricalConsumers.Items.RemoveAt(0)

    'Act
    compareResult = auxFresh.ConfigValuesAreTheSameAs( auxNow )

    Assert.AreEqual( false,compareResult )

    End Sub
    <Test()>
    <Category("ElectricalUserConfig")> _
    Public Sub ElectricalUserConfig_Unequal_Consumers_AvgConsumptionAmps_UnequalTest()

    'Arrange
    Dim auxFresh = New AuxillaryEnvironment("")
    Dim auxNow   = New AuxillaryEnvironment("")
    Dim compareResult As Boolean
    auxNow.ElectricalUserInputsConfig.ElectricalConsumers.Items(0).AvgConsumptionAmps+=1

    'Act
    compareResult = auxFresh.ConfigValuesAreTheSameAs( auxNow )

    Assert.AreEqual( false,compareResult )

    End Sub
    <Test()>
    <Category("ElectricalUserConfig")> _
    Public Sub ElectricalUserConfig_Unequal_Consumers_BaseVehicle_UnequalTest()

    'Arrange
    Dim auxFresh = New AuxillaryEnvironment("")
    Dim auxNow   = New AuxillaryEnvironment("")
    Dim compareResult As Boolean
    auxNow.ElectricalUserInputsConfig.ElectricalConsumers.Items(0).BaseVehicle= NOT auxNow.ElectricalUserInputsConfig.ElectricalConsumers.Items(0).BaseVehicle

    'Act
    compareResult = auxFresh.ConfigValuesAreTheSameAs( auxNow )

    Assert.AreEqual( false,compareResult )

    End Sub
    <Test()>
    <Category("ElectricalUserConfig")> _
    Public Sub ElectricalUserConfig_Unequal_Consumers_Category_UnequalTest()

    'Arrange
    Dim auxFresh = New AuxillaryEnvironment("")
    Dim auxNow   = New AuxillaryEnvironment("")
    Dim compareResult As Boolean
    Dim cat As String = auxNow.ElectricalUserInputsConfig.ElectricalConsumers.Items(0).Category
    auxNow.ElectricalUserInputsConfig.ElectricalConsumers.Items(0).Category= cat & "x"

    'Act
    compareResult = auxFresh.ConfigValuesAreTheSameAs( auxNow )

    Assert.AreEqual( false,compareResult )

    End Sub
    <Test()>
    <Category("ElectricalUserConfig")> _
    Public Sub ElectricalUserConfig_Unequal_Consumers_ConsumerName_UnequalTest()

    'Arrange
    Dim auxFresh = New AuxillaryEnvironment("")
    Dim auxNow   = New AuxillaryEnvironment("")
    Dim compareResult As Boolean
    Dim cname As String = auxNow.ElectricalUserInputsConfig.ElectricalConsumers.Items(0).ConsumerName
    auxNow.ElectricalUserInputsConfig.ElectricalConsumers.Items(0).ConsumerName= cName & "x"

    'Act
    compareResult = auxFresh.ConfigValuesAreTheSameAs( auxNow )

    Assert.AreEqual( false,compareResult )

    End Sub
    <Test()>
    <Category("ElectricalUserConfig")> _
    Public Sub ElectricalUserConfig_Unequal_Consumers_NominalConsumptionAmps_UnequalTest()

    'Arrange
    Dim auxFresh = New AuxillaryEnvironment("")
    Dim auxNow   = New AuxillaryEnvironment("")
    Dim compareResult As Boolean
    Dim cname As single = auxNow.ElectricalUserInputsConfig.ElectricalConsumers.Items(0).NominalConsumptionAmps
    auxNow.ElectricalUserInputsConfig.ElectricalConsumers.Items(0).NominalConsumptionAmps+=1

    'Act
    compareResult = auxFresh.ConfigValuesAreTheSameAs( auxNow )

    Assert.AreEqual( false,compareResult )

    End Sub
    <Test()>
    <Category("ElectricalUserConfig")> _
    Public Sub ElectricalUserConfig_Unequal_Consumers_NumberInActualVehicle_UnequalTest()

    'Arrange
    Dim auxFresh = New AuxillaryEnvironment("")
    Dim auxNow   = New AuxillaryEnvironment("")
    Dim compareResult As Boolean
    Dim cname As single = auxNow.ElectricalUserInputsConfig.ElectricalConsumers.Items(0).NumberInActualVehicle
    auxNow.ElectricalUserInputsConfig.ElectricalConsumers.Items(0).NumberInActualVehicle+=1

    'Act
    compareResult = auxFresh.ConfigValuesAreTheSameAs( auxNow )

    Assert.AreEqual( false,compareResult )

    End Sub
    <Test()>
    <Category("ElectricalUserConfig")> _
    Public Sub ElectricalUserConfig_Unequal_Consumers_PhaseIdle_TractionOn_UnequalTest()

    'Arrange
    Dim auxFresh = New AuxillaryEnvironment("")
    Dim auxNow   = New AuxillaryEnvironment("")
    Dim compareResult As Boolean
    Dim cname As single = auxNow.ElectricalUserInputsConfig.ElectricalConsumers.Items(0).PhaseIdle_TractionOn
    auxNow.ElectricalUserInputsConfig.ElectricalConsumers.Items(0).PhaseIdle_TractionOn+=1

    'Act
    compareResult = auxFresh.ConfigValuesAreTheSameAs( auxNow )

    Assert.AreEqual( false,compareResult )

    End Sub
    <Test()>
    <Category("ElectricalUserConfig")> _
    Public Sub ElectricalUserConfig_Unequal_ResultCardIdle_UnequalCount_UnequalTest()

    'Arrange
    Dim auxFresh = New AuxillaryEnvironment("")
    Dim auxNow   = New AuxillaryEnvironment("")
    Dim compareResult As Boolean
    auxNow.ElectricalUserInputsConfig.ResultCardIdle.Results.Add( New SmartResult(50,49))

    'Act
    compareResult = auxFresh.ConfigValuesAreTheSameAs( auxNow )

    Assert.AreEqual( false,compareResult )

    End Sub
    <Test()>
    <Category("ElectricalUserConfig")> _
    Public Sub ElectricalUserConfig_Unequal_ResultCardTraction_UnequalCount_UnequalTest()

    'Arrange
    Dim auxFresh = New AuxillaryEnvironment("")
    Dim auxNow   = New AuxillaryEnvironment("")
    Dim compareResult As Boolean
    auxNow.ElectricalUserInputsConfig.ResultCardTraction.Results.Add( New SmartResult(50,49))

    'Act
    compareResult = auxFresh.ConfigValuesAreTheSameAs( auxNow )

    Assert.AreEqual( false,compareResult )

    End Sub
    <Test()>
    <Category("ElectricalUserConfig")> _
    Public Sub ElectricalUserConfig_Unequal_ResultCardOverrun_UnequalCount_UnequalTest()

    'Arrange
    Dim auxFresh = New AuxillaryEnvironment("")
    Dim auxNow   = New AuxillaryEnvironment("")
    Dim compareResult As Boolean
    auxNow.ElectricalUserInputsConfig.ResultCardOverrun.Results.Add( New SmartResult(50,49))

    'Act
    compareResult = auxFresh.ConfigValuesAreTheSameAs( auxNow )

    Assert.AreEqual( false,compareResult )

    End Sub
    <Test()>
    <Category("ElectricalUserConfig")> _
    Public Sub ElectricalUserConfig_Unequal_ResultCardIdle_AMPS_UnequalTest()

    'Arrange
    Dim auxFresh = New AuxillaryEnvironment("")
    Dim auxNow   = New AuxillaryEnvironment("")
    Dim compareResult As Boolean
    auxFresh.ElectricalUserInputsConfig.ResultCardIdle.Results.Add( New SmartResult(50,49))
    auxNow.ElectricalUserInputsConfig.ResultCardIdle.Results.Add( New SmartResult(49,49))

    'Act
    compareResult = auxFresh.ConfigValuesAreTheSameAs( auxNow )

    Assert.AreEqual( false,compareResult )

    End Sub
    <Test()>
    <Category("ElectricalUserConfig")> _
    Public Sub ElectricalUserConfig_Unequal_ResultCardIdle_UnEqualAMPS_UnequalTest()

    'Arrange
    Dim auxFresh = New AuxillaryEnvironment("")
    Dim auxNow   = New AuxillaryEnvironment("")
    Dim compareResult As Boolean
    auxFresh.ElectricalUserInputsConfig.ResultCardIdle.Results.Add( New SmartResult(49,49))
    auxNow.ElectricalUserInputsConfig.ResultCardIdle.Results.Add( New SmartResult(50,49))

    'Act
    compareResult = auxFresh.ConfigValuesAreTheSameAs( auxNow )

    Assert.AreEqual( false,compareResult )

    End Sub
    <Test()>
    <Category("ElectricalUserConfig")> _
    Public Sub ElectricalUserConfig_Unequal_ResultCardIdle_UnEqualSMARTAMPS_UnequalTest()

    'Arrange
    Dim auxFresh = New AuxillaryEnvironment("")
    Dim auxNow   = New AuxillaryEnvironment("")
    Dim compareResult As Boolean
    auxFresh.ElectricalUserInputsConfig.ResultCardIdle.Results.Add( New SmartResult(49,51))
    auxNow.ElectricalUserInputsConfig.ResultCardIdle.Results.Add( New SmartResult(49,50))

    'Act
    compareResult = auxFresh.ConfigValuesAreTheSameAs( auxNow )

    Assert.AreEqual( false,compareResult )

    End Sub
    <Test()>
    <Category("ElectricalUserConfig")> _
    Public Sub ElectricalUserConfig_Unequal_ResultCardTraction_AMPS_UnequalTest()

    'Arrange
    Dim auxFresh = New AuxillaryEnvironment("")
    Dim auxNow   = New AuxillaryEnvironment("")
    Dim compareResult As Boolean
    auxFresh.ElectricalUserInputsConfig.ResultCardTraction.Results.Add( New SmartResult(50,49))
    auxNow.ElectricalUserInputsConfig.ResultCardTraction.Results.Add( New SmartResult(51,49))

    'Act
    compareResult = auxFresh.ConfigValuesAreTheSameAs( auxNow )

    Assert.AreEqual( false,compareResult )

    End Sub
    <Test()>
    <Category("ElectricalUserConfig")> _
    Public Sub ElectricalUserConfig_Unequal_ResultCardTraction_UnEqualAMPS_UnequalTest()

    'Arrange
    Dim auxFresh = New AuxillaryEnvironment("")
    Dim auxNow   = New AuxillaryEnvironment("")
    Dim compareResult As Boolean
    auxFresh.ElectricalUserInputsConfig.ResultCardIdle.Results.Add( New SmartResult(49,49))
    auxNow.ElectricalUserInputsConfig.ResultCardIdle.Results.Add( New SmartResult(50,49))

    'Act
    compareResult = auxFresh.ConfigValuesAreTheSameAs( auxNow )

    Assert.AreEqual( false,compareResult )

    End Sub
    <Test()>
    <Category("ElectricalUserConfig")> _
    Public Sub ElectricalUserConfig_Unequal_ResultCardTraction_UnEqualSMARTAMPS_UnequalTest()

    'Arrange
    Dim auxFresh = New AuxillaryEnvironment("")
    Dim auxNow   = New AuxillaryEnvironment("")
    Dim compareResult As Boolean
    auxFresh.ElectricalUserInputsConfig.ResultCardIdle.Results.Add( New SmartResult(49,49))
    auxNow.ElectricalUserInputsConfig.ResultCardIdle.Results.Add( New SmartResult(49,50))

    'Act
    compareResult = auxFresh.ConfigValuesAreTheSameAs( auxNow )

    Assert.AreEqual( false,compareResult )

    End Sub  
    <Test()>
    <Category("ElectricalUserConfig")> _
    Public Sub ElectricalUserConfig_Unequal_ResultCardOverrun_AMPS_UnequalTest()

    'Arrange
    Dim auxFresh = New AuxillaryEnvironment("")
    Dim auxNow   = New AuxillaryEnvironment("")
    Dim compareResult As Boolean
    auxFresh.ElectricalUserInputsConfig.ResultCardOverrun.Results.Add( New SmartResult(50,49))
    auxNow.ElectricalUserInputsConfig.ResultCardOverrun.Results.Add( New SmartResult(50,48))

    'Act
    compareResult = auxFresh.ConfigValuesAreTheSameAs( auxNow )

    Assert.AreEqual( false,compareResult )

    End Sub
    <Test()>
    <Category("ElectricalUserConfig")> _
    Public Sub ElectricalUserConfig_Unequal_ResultCardOverrun_UnEqualAMPS_UnequalTest()

    'Arrange
    Dim auxFresh = New AuxillaryEnvironment("")
    Dim auxNow   = New AuxillaryEnvironment("")
    Dim compareResult As Boolean
    auxFresh.ElectricalUserInputsConfig.ResultCardOverrun.Results.Add( New SmartResult(49,49))
    auxNow.ElectricalUserInputsConfig.ResultCardOverrun.Results.Add( New SmartResult(50,49))

    'Act
    compareResult = auxFresh.ConfigValuesAreTheSameAs( auxNow )

    Assert.AreEqual( false,compareResult )

    End Sub
    <Test()>
    <Category("ElectricalUserConfig")> _
    Public Sub ElectricalUserConfig_Unequal_ResultCardOverrun_UnEqualSMARTAMPS_UnequalTest()

    'Arrange
    Dim auxFresh = New AuxillaryEnvironment("")
    Dim auxNow   = New AuxillaryEnvironment("")
    Dim compareResult As Boolean
    auxFresh.ElectricalUserInputsConfig.ResultCardOverrun.Results.Add( New SmartResult(49,49))
    auxNow.ElectricalUserInputsConfig.ResultCardOverrun.Results.Add( New SmartResult(49,50))

    'Act
    compareResult = auxFresh.ConfigValuesAreTheSameAs( auxNow )

    Assert.AreEqual( false,compareResult )

    End Sub
    <Test()>
    <Category("ElectricalUserConfig")> _
    Public Sub ElectricalUserConfig_Unequal_PowernetVoltage_UnEqualSMARTAMPS_UnequalTest()

    'Arrange
    Dim auxFresh = New AuxillaryEnvironment("")
    Dim auxNow   = New AuxillaryEnvironment("")
    Dim compareResult As Boolean
    auxNow.ElectricalUserInputsConfig.PowerNetVoltage+=1
   
    'Act
    compareResult = auxFresh.ConfigValuesAreTheSameAs( auxNow )

    Assert.AreEqual( false,compareResult )

    End Sub
    <Test()>
    <Category("ElectricalUserConfig")> _
    Public Sub ElectricalUserConfig_Unequal_SmarElectrics_UnEqualSMARTAMPS_UnequalTest()

    'Arrange
    Dim auxFresh = New AuxillaryEnvironment("")
    Dim auxNow   = New AuxillaryEnvironment("")
    Dim compareResult As Boolean
    auxNow.ElectricalUserInputsConfig.SmartElectrical= Not auxNow.ElectricalUserInputsConfig.SmartElectrical
    
    'Act
    compareResult = auxFresh.ConfigValuesAreTheSameAs( auxNow )

    Assert.AreEqual( false,compareResult )

    End Sub

    #End Region

    #Region "PneumaticAuxiliariesConfig"

    <Test()>
    <Category("PneumaticAuxiliariesConfig")> _
    Public Sub PneumaticsAuxuiliaryConfig_AdBlueNIperMinute_Enequal()

    'Arrange
    Dim auxFresh = New AuxillaryEnvironment("")
    Dim auxNow   = New AuxillaryEnvironment("")
    Dim compareResult As Boolean
    auxNow.PneumaticAuxillariesConfig.AdBlueNIperMinute+=1
  
    'Act
    compareResult = auxFresh.ConfigValuesAreTheSameAs( auxNow )

    Assert.AreEqual( false,compareResult )

    End Sub
    <Test()>
    <Category("PneumaticAuxiliariesConfig")> _
    Public Sub PneumaticsAuxuiliaryConfig_AirControlledSuspensionNIperMinute_Enequal()

    'Arrange
    Dim auxFresh = New AuxillaryEnvironment("")
    Dim auxNow   = New AuxillaryEnvironment("")
    Dim compareResult As Boolean
    auxNow.PneumaticAuxillariesConfig.AirControlledSuspensionNIperMinute+=1

    'Act
    compareResult = auxFresh.ConfigValuesAreTheSameAs( auxNow )

    Assert.AreEqual( false,compareResult )

    End Sub
    <Test()>
    <Category("PneumaticAuxiliariesConfig")> _
    Public Sub PneumaticsAuxuiliaryConfig_BrakingNoRetarderNIperKG_Enequal()

    'Arrange
    Dim auxFresh = New AuxillaryEnvironment("")
    Dim auxNow   = New AuxillaryEnvironment("")
    Dim compareResult As Boolean
    auxNow.PneumaticAuxillariesConfig.BrakingNoRetarderNIperKG+=1

    'Act
    compareResult = auxFresh.ConfigValuesAreTheSameAs( auxNow )

    Assert.AreEqual( false,compareResult )

    End Sub
    <Test()>
    <Category("PneumaticAuxiliariesConfig")> _
    Public Sub PneumaticsAuxuiliaryConfig_BrakingWithRetarderNIperKG_Enequal()

    'Arrange
    Dim auxFresh = New AuxillaryEnvironment("")
    Dim auxNow   = New AuxillaryEnvironment("")
    Dim compareResult As Boolean
    auxNow.PneumaticAuxillariesConfig.BrakingWithRetarderNIperKG+=1

    'Act
    compareResult = auxFresh.ConfigValuesAreTheSameAs( auxNow )

    Assert.AreEqual( false,compareResult )

    End Sub
    <Test()>
    <Category("PneumaticAuxiliariesConfig")> _
    Public Sub PneumaticsAuxuiliaryConfig_BreakingPerKneelingNIperKGinMM_Enequal()

    'Arrange
    Dim auxFresh = New AuxillaryEnvironment("")
    Dim auxNow   = New AuxillaryEnvironment("")
    Dim compareResult As Boolean
    auxNow.PneumaticAuxillariesConfig.BreakingPerKneelingNIperKGinMM+=1

    'Act
    compareResult = auxFresh.ConfigValuesAreTheSameAs( auxNow )

    Assert.AreEqual( false,compareResult )

    End Sub
    <Test()>
    <Category("PneumaticAuxiliariesConfig")> _
    Public Sub PneumaticsAuxuiliaryConfig_DeadVolBlowOutsPerLitresperHour_Enequal()

    'Arrange
    Dim auxFresh = New AuxillaryEnvironment("")
    Dim auxNow   = New AuxillaryEnvironment("")
    Dim compareResult As Boolean
    auxNow.PneumaticAuxillariesConfig.DeadVolBlowOutsPerLitresperHour+=1   

    'Act
    compareResult = auxFresh.ConfigValuesAreTheSameAs( auxNow )

    Assert.AreEqual( false,compareResult )

    End Sub
    <Test()>
    <Category("PneumaticAuxiliariesConfig")> _
    Public Sub PneumaticsAuxuiliaryConfig_DeadVolumeLitres_Enequal()

    'Arrange
    Dim auxFresh = New AuxillaryEnvironment("")
    Dim auxNow   = New AuxillaryEnvironment("")
    Dim compareResult As Boolean
    auxNow.PneumaticAuxillariesConfig.DeadVolumeLitres+=1    

    'Act
    compareResult = auxFresh.ConfigValuesAreTheSameAs( auxNow )

    Assert.AreEqual( false,compareResult )

    End Sub
    <Test()>
    <Category("PneumaticAuxiliariesConfig")> _
    Public Sub PneumaticsAuxuiliaryConfig_NonSmartRegenFractionTotalAirDemand_Enequal()

    'Arrange
    Dim auxFresh = New AuxillaryEnvironment("")
    Dim auxNow   = New AuxillaryEnvironment("")
    Dim compareResult As Boolean
    auxNow.PneumaticAuxillariesConfig.NonSmartRegenFractionTotalAirDemand+=1    

    'Act
    compareResult = auxFresh.ConfigValuesAreTheSameAs( auxNow )

    Assert.AreEqual( false,compareResult )

    End Sub
    <Test()>
    <Category("PneumaticAuxiliariesConfig")> _
    Public Sub PneumaticsAuxuiliaryConfig_OverrunUtilisationForCompressionFraction_Enequal()

    'Arrange
    Dim auxFresh = New AuxillaryEnvironment("")
    Dim auxNow   = New AuxillaryEnvironment("")
    Dim compareResult As Boolean
    auxNow.PneumaticAuxillariesConfig.OverrunUtilisationForCompressionFraction+=1    

    'Act
    compareResult = auxFresh.ConfigValuesAreTheSameAs( auxNow )

    Assert.AreEqual( false,compareResult )

    End Sub
    <Test()>
    <Category("PneumaticAuxiliariesConfig")> _
    Public Sub PneumaticsAuxuiliaryConfig_PerDoorOpeningNI_Enequal()

    'Arrange
    Dim auxFresh = New AuxillaryEnvironment("")
    Dim auxNow   = New AuxillaryEnvironment("")
    Dim compareResult As Boolean
    auxNow.PneumaticAuxillariesConfig.PerDoorOpeningNI+=1    

    'Act
    compareResult = auxFresh.ConfigValuesAreTheSameAs( auxNow)

    Assert.AreEqual( false,compareResult )

    End Sub
    <Test()>
    <Category("PneumaticAuxiliariesConfig")> _
    Public Sub PneumaticsAuxuiliaryConfig_PerStopBrakeActuationNIperKG_Enequal()

    'Arrange
    Dim auxFresh = New AuxillaryEnvironment("")
    Dim auxNow   = New AuxillaryEnvironment("")
    Dim compareResult As Boolean
    auxNow.PneumaticAuxillariesConfig.PerStopBrakeActuationNIperKG+=1    

    'Act
    compareResult = auxFresh.ConfigValuesAreTheSameAs( auxNow)

    Assert.AreEqual( false,compareResult )

    End Sub
    <Test()>
    <Category("PneumaticAuxiliariesConfig")> _
    Public Sub PneumaticsUserInputsConfig_SmartRegenFractionTotalAirDemand_Enequal()

    'Arrange
    Dim auxFresh = New AuxillaryEnvironment("")
    Dim auxNow   = New AuxillaryEnvironment("")
    Dim compareResult As Boolean
    auxNow.PneumaticAuxillariesConfig.SmartRegenFractionTotalAirDemand+=1    

    'Act
    compareResult = auxFresh.ConfigValuesAreTheSameAs( auxNow)

    Assert.AreEqual( false,compareResult )

    End Sub

    #End Region

    #Region "PneumaticUserInputsConfig"

    <Test()>
    <Category("PneumaticUserInputsConfig")> _
    Public Sub PneumaticUserInputsConfig_ActuationsMap_Enequal()

    'Arrange
    Dim auxFresh = New AuxillaryEnvironment("")
    Dim auxNow   = New AuxillaryEnvironment("")
    Dim compareResult As Boolean
    auxNow.PneumaticUserInputsConfig.ActuationsMap = auxNow.PneumaticUserInputsConfig.ActuationsMap & "x"    

    'Act
    compareResult = auxFresh.ConfigValuesAreTheSameAs( auxNow)

    Assert.AreEqual( false,compareResult )

    End Sub
    <Test()>
    <Category("PneumaticUserInputsConfig")> _
    Public Sub PneumaticUserInputsConfig_AdBlueDosing_Enequal()

    'Arrange
    Dim auxFresh = New AuxillaryEnvironment("")
    Dim auxNow   = New AuxillaryEnvironment("")
    Dim compareResult As Boolean
    auxNow.PneumaticUserInputsConfig.AdBlueDosing = auxNow.PneumaticUserInputsConfig.AdBlueDosing & "x"   
     
    'Act
    compareResult = auxFresh.ConfigValuesAreTheSameAs( auxNow)

    Assert.AreEqual( false,compareResult )

    End Sub
    <Test()>
    <Category("PneumaticUserInputsConfig")> _
    Public Sub PneumaticUserInputsConfig_AirSuspensionControl_Enequal()

    'Arrange
    Dim auxFresh = New AuxillaryEnvironment("")
    Dim auxNow   = New AuxillaryEnvironment("")
    Dim compareResult As Boolean
    auxNow.PneumaticUserInputsConfig.AirSuspensionControl = auxNow.PneumaticUserInputsConfig.AirSuspensionControl & "x"    

    'Act
    compareResult = auxFresh.ConfigValuesAreTheSameAs( auxNow)

    Assert.AreEqual( false,compareResult )

    End Sub
    <Test()>
    <Category("PneumaticUserInputsConfig")> _
    Public Sub PneumaticUserInputsConfig_CompressorGearEfficiency_Enequal()

    'Arrange
    Dim auxFresh = New AuxillaryEnvironment("")
    Dim auxNow   = New AuxillaryEnvironment("")
    Dim compareResult As Boolean
    auxNow.PneumaticUserInputsConfig.CompressorGearEfficiency = If( auxNow.PneumaticUserInputsConfig.CompressorGearEfficiency-0.1<0,0,auxNow.PneumaticUserInputsConfig.CompressorGearEfficiency-0.1)
        
    'Act
    compareResult = auxFresh.ConfigValuesAreTheSameAs( auxNow)


    Assert.AreEqual( false,compareResult )

    End Sub
    <Test()>
    <Category("PneumaticUserInputsConfig")> _
    Public Sub PneumaticUserInputsConfig_CompressorGearRatio_Enequal()

    'Arrange
    Dim auxFresh = New AuxillaryEnvironment("")
    Dim auxNow   = New AuxillaryEnvironment("")
    Dim compareResult As Boolean 
    auxNow.PneumaticUserInputsConfig.CompressorGearRatio +=1

    'Act
    compareResult = auxFresh.ConfigValuesAreTheSameAs( auxNow)
    Assert.AreEqual( false,compareResult )

    End Sub
    <Test()>
    <Category("PneumaticUserInputsConfig")> _
    Public Sub PneumaticUserInputsConfig_CompressorMap_Enequal()

    'Arrange
    Dim auxFresh = New AuxillaryEnvironment("")
    Dim auxNow   = New AuxillaryEnvironment("")
    Dim compareResult As Boolean
    

    auxNow.PneumaticUserInputsConfig.CompressorMap = auxNow.PneumaticUserInputsConfig.CompressorMap & "x"
    'Act
    compareResult = auxFresh.ConfigValuesAreTheSameAs( auxNow)



    Assert.AreEqual( false,compareResult )

    End Sub
    <Test()>
    <Category("PneumaticUserInputsConfig")> _
    Public Sub PneumaticUserInputsConfig_Doors_Enequal()

    'Arrange
    Dim auxFresh = New AuxillaryEnvironment("")
    Dim auxNow   = New AuxillaryEnvironment("")
    Dim compareResult As Boolean
    auxNow.PneumaticUserInputsConfig.Doors = auxNow.PneumaticUserInputsConfig.Doors & "x"

    'Act
    compareResult = auxFresh.ConfigValuesAreTheSameAs( auxNow)


    Assert.AreEqual( false,compareResult )

    End Sub
    <Test()>
    <Category("PneumaticUserInputsConfig")> _
    Public Sub PneumaticUserInputsConfig_KneelingHeightMillimeters_Enequal()

    'Arrange
    Dim auxFresh = New AuxillaryEnvironment("")
    Dim auxNow   = New AuxillaryEnvironment("")
    Dim compareResult As Boolean
    auxNow.PneumaticUserInputsConfig.KneelingHeightMillimeters +=1   
     
    'Act
    compareResult = auxFresh.ConfigValuesAreTheSameAs( auxNow)
    Assert.AreEqual( false,compareResult )

    End Sub
    <Test()>
    <Category("PneumaticUserInputsConfig")> _
    Public Sub PneumaticUserInputsConfig_RetarderBrake_Enequal()

    'Arrange
    Dim auxFresh = New AuxillaryEnvironment("")
    Dim auxNow   = New AuxillaryEnvironment("")
    Dim compareResult As Boolean    
    auxNow.PneumaticUserInputsConfig.RetarderBrake = Not auxNow.PneumaticUserInputsConfig.RetarderBrake 

    'Act
    compareResult = auxFresh.ConfigValuesAreTheSameAs( auxNow)
    Assert.AreEqual( false,compareResult )

    End Sub
    <Test()>
    <Category("PneumaticUserInputsConfig")> _
    Public Sub PneumaticUserInputsConfig_SmartAirCompression_Enequal()

    'Arrange
    Dim auxFresh = New AuxillaryEnvironment("")
    Dim auxNow   = New AuxillaryEnvironment("")
    Dim compareResult As Boolean
    auxNow.PneumaticUserInputsConfig.SmartAirCompression = Not auxNow.PneumaticUserInputsConfig.SmartAirCompression 

    'Act
    compareResult = auxFresh.ConfigValuesAreTheSameAs( auxNow)


    Assert.AreEqual( false,compareResult )

    End Sub
    <Test()>
    <Category("PneumaticUserInputsConfig")> _
    Public Sub PneumaticUserInputsConfig_SmartRegeneration_Enequal()

    'Arrange
    Dim auxFresh = New AuxillaryEnvironment("")
    Dim auxNow   = New AuxillaryEnvironment("")
    Dim compareResult As Boolean
    auxNow.PneumaticUserInputsConfig.SmartRegeneration = Not auxNow.PneumaticUserInputsConfig.SmartRegeneration 
   
    'Act
    compareResult = auxFresh.ConfigValuesAreTheSameAs( auxNow)

    Assert.AreEqual( false,compareResult )

    End Sub

    #End Region






End Class


End Namespace


