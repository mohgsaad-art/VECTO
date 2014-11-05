Imports VectoAuxiliaries.Electrics
Imports VectoAuxiliaries.Pneumatics
Imports VectoAuxiliaries.Hvac
Imports VectoAuxiliaries.DownstreamModules
Imports NUnit.Framework
Imports VectoAuxiliaries

Namespace UnitTests

<TestFixture()> _
Public Class M7Test

private M5 As M5_Mock
Private M6 As M6_MOCK
Private Signals As ISignals

Public Sub new ()

M5 = New M5_Mock( 100,110,120)
M6 = New M6_Mock(100,0,0,110,120,0,130,140,150)
Signals = New Signals()

End Sub

<Test()>
Public Sub CreateNew_M7InstanceTest()
 Dim target As IM7 = New M7(M5,M6,Signals)
 Assert.IsNotNull(target)
End Sub

'IP1  M5 : Alternators Generation Power At Crank (Traction ) (Single )
'IP2  M5 : Alternators Generation Power At Crank (Idle )     (Single )
'IP3  Signals : IDLE            (Boolean )
'IP4  M6      :Overrun Flag     (Integer )
'IP5  Signals : Clutch engaged  (Boolean )
'IP6  Signals : InNuetral       (Boolean )
'IP7  M6      : SmartElectricalAndPneumatic:AlternatorPowerGen@Crank
'IP8  M6      : SmartElectricalAndPneumatic:AirCompPowerGen@Crank
'IP9  M6      : SmartElectricalOnly:AlternatorPowerGen@Crank
'IP10 M6      : AveragePowerDemand@CrankFromPneumatics
'IP11 M6      : SmartPneumaticsOnly:AirComprPowerGen@Crank
'OP1  Op1     :Smart Electrical & Pneumatic Aux : Alternator power gen @ Crank
'OP2  OP2     :Smart Electrical & Pneumatic Aux : Air comp   power gen @ Crank
'OP3  OP3     :Smart Electrical Aux : Alternator             Power Gen @ Crank
'OP4  OP4     :Smart Electrical Aux : Ait Compressor         Power Gen @ Crank 
<Test()>
Public Sub InputOutputTests(IP1 As Single,  _  
                            IP2 As Single,  _ 
                            IP3 As Boolean, _
                            IP4 As Single,  _
                            IP5 As Boolean, _
                            IP6 As Boolean, _
                            IP7 As Single,  _
                            IP8 As Single,  _
                            IP9 As single,  _
                            IP10 As Single, _
                            IP11 As Single, _
                            IP12 As Single, _
                            IP13 As Single, _
                            OP1  As Single, _
                            OP2  As Single,  _
                            OP3  As Single,  _
                            OP4  As Single  )

                            'Instantiate new mocks.
                            M5 = New M5_Mock()
                            M6 = New M6_Mock()                         
                            Signals = New Signals

                            'Assign from TestCaseValues
                            M5._AlternatorsGenerationPowerAtCrankTractionOnWatts=IP1
                            M5._AlternatorsGenerationPowerAtCrankIdleWatts=IP2
                            Signals.Idle=IP3
                            M6._OverrunFlag=IP4
                            Signals.ClutchEngaged=IP5 
                            Signals.InNeutral=IP6 
                            M6._SmartElecAndPneumaticAltPowerGenAtCrank=IP7
                            M6._SmartElecAndPneumaticAirCompPowerGenAtCrank=IP8
                            M6._SmartElecOnlyAltPowerGenAtCrank=IP9
                            M6._AveragePowerDemandAtCrankFromPneumatics=IP10
                            M6._SmartPneumaticOnlyAirCompPowerGenAtCrank=IP11


            'Create Instance of M7 from 
            Dim target As IM7 = New M7(M5,M6,Signals)



            Assert.AreEqual(OP1, target.SmartElectricalAndPneumaticAuxAltPowerGenAtCrank)
            Assert.AreEqual(OP2, target.SmartElectricalAndPneumaticAuxAirCompPowerGenAtCrank)
            Assert.AreEqual(OP3, target.SmartElectricalOnlyAuxAltPowerGenAtCrank)
            Assert.AreEqual(OP4, target.SmartPneumaticOnlyAuxAirCompPowerGenAtCrank)
             

 Assert.IsNotNull(target)

End Sub


End Class



End Namespace



