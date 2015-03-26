Imports NUnit.Framework
Imports VectoAuxiliaries.Electrics
Imports VectoAuxiliariesTests.Mocks
Imports VectoAuxiliaries
Imports VectoAuxiliaries.Hvac

Namespace UnitTests

 <TestFixture()>
 Public Class CombinedAlternatorTests

  private Alt1ExpectedTable2000 As new List(Of AltUserInput)
  private Alt1ExpectedTable4000 As new List(Of AltUserInput)
  private Alt1ExpectedTable6000 As new List(Of AltUserInput)
  private Alt2ExpectedTable2000 As new List(Of AltUserInput)
  private Alt2ExpectedTable4000 As new List(Of AltUserInput)
  private Alt2ExpectedTable6000 As new List(Of AltUserInput)
  private Alt3ExpectedTable2000 As new List(Of AltUserInput)
  private Alt3ExpectedTable4000 As new List(Of AltUserInput)
  private Alt3ExpectedTable6000 As new List(Of AltUserInput)
  private Alt4ExpectedTable2000 As new List(Of AltUserInput)
  private Alt4ExpectedTable4000 As new List(Of AltUserInput)
  private Alt4ExpectedTable6000 As new List(Of AltUserInput)
  Private RangeTableExpected    As New List(Of AltUserInput)



  Private Const COMBINEDALT_GOODMAP = "testfiles\testCombinedAlternatorMap.aalt"


  Sub new ()

  Alt1ExpectedTable2000 = New List(Of AltUserInput)() from  {  New AltUserInput(  0,50), _
                                                               New AltUserInput( 10,50), _
                                                               New AltUserInput( 40,50), _
                                                               New AltUserInput( 60,50), _
                                                               New AltUserInput( 61,50), _
                                                               New AltUserInput(200,50) }

  Alt1ExpectedTable4000 = New List(Of AltUserInput)() from  {  New AltUserInput(  0,70), _
                                                               New AltUserInput( 10,70), _
                                                               New AltUserInput( 40,70), _
                                                               New AltUserInput( 60,70), _
                                                               New AltUserInput( 61,70), _
                                                               New AltUserInput(200,70) }


  Alt1ExpectedTable6000 = New List(Of AltUserInput)() from  {  New AltUserInput(  0,60), _
                                                               New AltUserInput( 10,60), _
                                                               New AltUserInput( 40,60), _
                                                               New AltUserInput( 60,60), _
                                                               New AltUserInput( 61,60), _
                                                               New AltUserInput(200,60) }

  'ALT 2
  Alt2ExpectedTable2000 = New List(Of AltUserInput)() from  {  New AltUserInput(  0,80), _
                                                               New AltUserInput( 10,80), _
                                                               New AltUserInput( 40,80), _
                                                               New AltUserInput( 60,80), _
                                                               New AltUserInput( 61,80), _
                                                               New AltUserInput(200,80) }

  Alt2ExpectedTable4000 = New List(Of AltUserInput)() from  {  New AltUserInput(  0,40), _
                                                               New AltUserInput( 10,40), _
                                                               New AltUserInput( 40,40), _
                                                               New AltUserInput( 60,40), _
                                                               New AltUserInput( 61,40), _
                                                               New AltUserInput(200,40) }


  Alt2ExpectedTable6000 = New List(Of AltUserInput)() from  {  New AltUserInput(  0,60), _
                                                               New AltUserInput( 10,60), _
                                                               New AltUserInput( 40,60), _
                                                               New AltUserInput( 60,60), _
                                                               New AltUserInput( 61,60), _
                                                               New AltUserInput(200,60) }


  'ALT 3
  Alt3ExpectedTable2000 = New List(Of AltUserInput)() from  {  New AltUserInput(  0,95), _
                                                               New AltUserInput( 10,95), _
                                                               New AltUserInput( 40,50), _
                                                               New AltUserInput( 60,90), _
                                                               New AltUserInput(62.5,95), _
                                                               New AltUserInput(200, 95) }

  Alt3ExpectedTable4000 = New List(Of AltUserInput)() from  {  New AltUserInput(  0,99), _
                                                               New AltUserInput( 10,99), _
                                                               New AltUserInput( 40, 1), _
                                                               New AltUserInput( 60,55), _
                                                               New AltUserInput( 76.2962963,99), _
                                                               New AltUserInput(200,99) }


  Alt3ExpectedTable6000 = New List(Of AltUserInput)() from  {  New AltUserInput(  0,94), _
                                                               New AltUserInput( 10,94), _
                                                               New AltUserInput( 40, 86), _
                                                               New AltUserInput( 60,13), _
                                                               New AltUserInput( 63.5616438,0), _
                                                               New AltUserInput(200,0) }


  'ALT 4
  Alt4ExpectedTable2000 = New List(Of AltUserInput)() from  {  New AltUserInput(  0,55), _
                                                               New AltUserInput( 10,55), _
                                                               New AltUserInput( 40,45), _
                                                               New AltUserInput( 60,67), _
                                                               New AltUserInput( 60,67), _
                                                               New AltUserInput(200,67) }

  Alt4ExpectedTable4000 = New List(Of AltUserInput)() from  {  New AltUserInput(  0,77), _
                                                               New AltUserInput( 10,77), _
                                                               New AltUserInput( 40,39), _
                                                               New AltUserInput( 60,23), _
                                                               New AltUserInput( 88.75,0), _
                                                               New AltUserInput(200,0) }


  Alt4ExpectedTable6000 = New List(Of AltUserInput)() from  {  New AltUserInput(  0,34), _
                                                               New AltUserInput( 10,34), _
                                                               New AltUserInput( 40, 67), _
                                                               New AltUserInput( 60,35), _
                                                               New AltUserInput( 81.875,0), _
                                                               New AltUserInput(200,0) }



  'RangeTable
  RangeTableExpected = New List(Of AltUserInput)()   from {   New AltUserInput(-3001 , 0), _
                                                              New AltUserInput(-3000 , 0), _
                                                              New AltUserInput(2000  ,50), _
                                                              New AltUserInput(4000  ,70), _
                                                              New AltUserInput(6000  ,60), _
                                                              New AltUserInput(18000 , 0), _
                                                              New AltUserInput(18001 , 0) }

    







  End Sub

  <Test()> _
  <TestCase(1,2,50f)> _
  <TestCase(1,4,70)> _
  <TestCase(1,6,60)> _
  <TestCase(2,2,80)> _
  <TestCase(2,4,40)> _
  <TestCase(2,6,60)> _
  <TestCase(3,2,55)> _
  <TestCase(3,4,7.75f)> _
  <TestCase(3,6,76.875f)> _
  <TestCase(4,2,47.75f)> _
  <TestCase(4,4,37)> _
  <TestCase(4,6,63)> 
  Public Sub Interpolate4Table4( alt As Integer, rpmK As integer, expected As single)

   Dim interpValue As Single

   Select Case alt

    Case 1
      
       Select Case rpmK
          
         Case 2
              interpValue= InterpAltUserInputs.Iterpolate( Alt1ExpectedTable2000,42.5)
         Case 4
              interpValue= InterpAltUserInputs.Iterpolate( Alt1ExpectedTable4000,42.5)
         Case 6
              interpValue= InterpAltUserInputs.Iterpolate( Alt1ExpectedTable6000,42.5)

       End Select


    Case 2

       Select Case rpmK
          
         Case 2
              interpValue= InterpAltUserInputs.Iterpolate( Alt2ExpectedTable2000,42.5)
         Case 4                                               
              interpValue= InterpAltUserInputs.Iterpolate( Alt2ExpectedTable4000,42.5)
         Case 6                                               
              interpValue= InterpAltUserInputs.Iterpolate( Alt2ExpectedTable6000,42.5)

       End Select

    Case 3

       Select Case rpmK
          
         Case 2
              interpValue= InterpAltUserInputs.Iterpolate( Alt3ExpectedTable2000,42.5)
         Case 4                                               
              interpValue= InterpAltUserInputs.Iterpolate( Alt3ExpectedTable4000,42.5)
         Case 6                                               
              interpValue= InterpAltUserInputs.Iterpolate( Alt3ExpectedTable6000,42.5)

       End Select

    Case 4

       Select Case rpmK
          
         Case 2
              interpValue= InterpAltUserInputs.Iterpolate( Alt4ExpectedTable2000,42.5)
         Case 4                                               
              interpValue= InterpAltUserInputs.Iterpolate( Alt4ExpectedTable4000,42.5)
         Case 6                                               
              interpValue= InterpAltUserInputs.Iterpolate( Alt4ExpectedTable6000,42.5)

       End Select

   End Select


       Assert.AreEqual( interpValue, expected)


  End Sub


  <Test()>
  Public Sub Alt1TableConstructTest()


  'Arrange
  Dim signals As ICombinedAlternatorSignals = New CombinedAlternatorSignals


  'Act
  Dim alt  As new CombinedAlternator(COMBINEDALT_GOODMAP, signals)



   Dim idx As integer

   For idx = 0 to alt.Alternators(0).InputTable2000.Count-1
      Assert.IsTrue( alt.Alternators(0).InputTable2000(idx).IsEqual( Alt1ExpectedTable2000(idx)))
   Next

   For idx = 0 to alt.Alternators(0).InputTable4000.Count-1
      Assert.IsTrue( alt.Alternators(0).InputTable4000(idx).IsEqual( Alt1ExpectedTable4000(idx)))
   Next

   For idx = 0 to alt.Alternators(0).InputTable6000.Count-1
      Assert.IsTrue( alt.Alternators(0).InputTable6000(idx).IsEqual( Alt1ExpectedTable6000(idx)))
   Next


  End Sub

    <Test()>
  Public Sub Alt2TableConstructTest()



  'Arrange
  Dim signals As ICombinedAlternatorSignals = New CombinedAlternatorSignals


  'Act
  Dim alt  As new CombinedAlternator(COMBINEDALT_GOODMAP, signals)
    

   Dim idx As integer

   For idx = 0 to alt.Alternators(1).InputTable2000.Count-1
      Assert.IsTrue( alt.Alternators(1).InputTable2000(idx).IsEqual( Alt2ExpectedTable2000(idx)))
   Next

   For idx = 0 to alt.Alternators(1).InputTable4000.Count-1
      Assert.IsTrue( alt.Alternators(1).InputTable4000(idx).IsEqual( Alt2ExpectedTable4000(idx)))
   Next

   For idx = 0 to alt.Alternators(1).InputTable6000.Count-1
      Assert.IsTrue( alt.Alternators(1).InputTable6000(idx).IsEqual( Alt2ExpectedTable6000(idx)))
   Next
                              

  End Sub
  <Test()>
  Public Sub Alt3TableConstructTest()


  'Arrange
  Dim signals As ICombinedAlternatorSignals = New CombinedAlternatorSignals


  'Act
  Dim alt  As new CombinedAlternator(COMBINEDALT_GOODMAP, signals)
    

   Dim idx As integer

   For idx = 0 to alt.Alternators(2).InputTable2000.Count-1
      Assert.IsTrue( alt.Alternators(2).InputTable2000(idx).IsEqual( Alt3ExpectedTable2000(idx)))
   Next

   For idx = 0 to alt.Alternators(2).InputTable4000.Count-1
      Assert.IsTrue( alt.Alternators(2).InputTable4000(idx).IsEqual( Alt3ExpectedTable4000(idx),3))
   Next

   For idx = 0 to alt.Alternators(2).InputTable6000.Count-1
      Assert.IsTrue( alt.Alternators(2).InputTable6000(idx).IsEqual( Alt3ExpectedTable6000(idx),3))
   Next
                              

  End Sub
  <Test()>
  Public Sub Alt4TableConstructTest()

  'Arrange
  Dim signals As ICombinedAlternatorSignals = New CombinedAlternatorSignals


  'Act
  Dim alt  As new CombinedAlternator(COMBINEDALT_GOODMAP, signals)
    

   Dim idx As integer

   For idx = 0 to alt.Alternators(3).InputTable2000.Count-1
      Assert.IsTrue( alt.Alternators(3).InputTable2000(idx).IsEqual( Alt4ExpectedTable2000(idx)))
   Next

   For idx = 0 to alt.Alternators(3).InputTable4000.Count-1
      Assert.IsTrue( alt.Alternators(3).InputTable4000(idx).IsEqual( Alt4ExpectedTable4000(idx),3))
   Next

   For idx = 0 to alt.Alternators(3).InputTable6000.Count-1
      Assert.IsTrue( alt.Alternators(3).InputTable6000(idx).IsEqual( Alt4ExpectedTable6000(idx),3))
   Next
                              

  End Sub


 'testCombinedAlternatorMap
   <Test()>
  Public Sub InitialiseCombinedAlternatorMapFromFile()


  'Arrange
  Dim signals As ICombinedAlternatorSignals = New CombinedAlternatorSignals


  'Act
  Dim target  As new CombinedAlternator(COMBINEDALT_GOODMAP, signals)



  'Assert

  Assert.AreEqual( target.Alternators.Count,4)

              

  End Sub

   <Test()>
  Public Sub InitialiseCombinedAlternatorMapFromDefault()


  'Arrange
  Dim signals As ICombinedAlternatorSignals = New CombinedAlternatorSignals


  'Act
  Dim target  As new CombinedAlternator("123.aalt", signals)



  'Assert

  Assert.AreEqual( target.Alternators.Count,4)

              

  End Sub



 End Class


End Namespace



    

