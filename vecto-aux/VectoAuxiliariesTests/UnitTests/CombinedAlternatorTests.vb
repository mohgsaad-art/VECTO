Imports VectoAuxiliaries.Electrics
Imports VectoAuxiliaries.Pneumatics
Imports VectoAuxiliaries.Hvac
Imports VectoAuxiliaries.DownstreamModules
Imports NUnit.Framework
Imports VectoAuxiliaries
Imports Moq

Namespace UnitTests


<TestFixture()>
Public Class CombinedAlternatorTests

   <Test()>
   public sub One_AlternatorTestSet()

     Dim target  As New CombinedAlternator("c:\alt.xlsx",3,4,5,6,1)


     Assert.IsTrue( ValueSetsEqual( target.GetCombinedMap, OneAltValues))


   End Sub



Public   function OneAltValues() As List( OF CombinedAltEntry)

Dim results As New List( of CombinedAltEntry)


    results.Add( new CombinedAltEntry with { .Amps=10.00	,.EngineSpeed=500	,.Efficiency=0.60   })
    results.Add( new CombinedAltEntry with { .Amps=25.00	,.EngineSpeed=500	,.Efficiency=0.60   })
    results.Add( new CombinedAltEntry with { .Amps=50.00	,.EngineSpeed=500	,.Efficiency=0.60   })
    results.Add( new CombinedAltEntry with { .Amps=75.00	,.EngineSpeed=500	,.Efficiency=0.60   })
    results.Add( new CombinedAltEntry with { .Amps=100.00	,.EngineSpeed=500	,.Efficiency=0.60   })
    results.Add( new CombinedAltEntry with { .Amps=150.00	,.EngineSpeed=500	,.Efficiency=0.60   })
    results.Add( new CombinedAltEntry with { .Amps=200.00	,.EngineSpeed=500	,.Efficiency=0.60   })
    results.Add( new CombinedAltEntry with { .Amps=10.00	,.EngineSpeed=667	,.Efficiency=0.60   })
    results.Add( new CombinedAltEntry with { .Amps=25.00	,.EngineSpeed=667	,.Efficiency=0.60   })
    results.Add( new CombinedAltEntry with { .Amps=50.00	,.EngineSpeed=667	,.Efficiency=0.60   })
    results.Add( new CombinedAltEntry with { .Amps=75.00	,.EngineSpeed=667	,.Efficiency=0.60   })
    results.Add( new CombinedAltEntry with { .Amps=100.00	,.EngineSpeed=667	,.Efficiency=0.60   })
    results.Add( new CombinedAltEntry with { .Amps=150.00	,.EngineSpeed=667	,.Efficiency=0.60   })
    results.Add( new CombinedAltEntry with { .Amps=200.00	,.EngineSpeed=667	,.Efficiency=0.60   })
    results.Add( new CombinedAltEntry with { .Amps=10.00	,.EngineSpeed=1333	,.Efficiency=0.60   })
    results.Add( new CombinedAltEntry with { .Amps=25.00	,.EngineSpeed=1333	,.Efficiency=0.60   })
    results.Add( new CombinedAltEntry with { .Amps=50.00	,.EngineSpeed=1333	,.Efficiency=0.60   })
    results.Add( new CombinedAltEntry with { .Amps=75.00	,.EngineSpeed=1333	,.Efficiency=0.60   })
    results.Add( new CombinedAltEntry with { .Amps=100.00	,.EngineSpeed=1333	,.Efficiency=0.60   })
    results.Add( new CombinedAltEntry with { .Amps=150.00	,.EngineSpeed=1333	,.Efficiency=0.60   })
    results.Add( new CombinedAltEntry with { .Amps=200.00	,.EngineSpeed=1333	,.Efficiency=0.60   })
    results.Add( new CombinedAltEntry with { .Amps=10.00	,.EngineSpeed=2000	,.Efficiency=0.70   })
    results.Add( new CombinedAltEntry with { .Amps=25.00	,.EngineSpeed=2000	,.Efficiency=0.67   })
    results.Add( new CombinedAltEntry with { .Amps=50.00	,.EngineSpeed=2000	,.Efficiency=0.63   })
    results.Add( new CombinedAltEntry with { .Amps=75.00	,.EngineSpeed=2000	,.Efficiency=0.59   })
    results.Add( new CombinedAltEntry with { .Amps=100.00	,.EngineSpeed=2000	,.Efficiency=0.54   })
    results.Add( new CombinedAltEntry with { .Amps=150.00	,.EngineSpeed=2000	,.Efficiency=0.46   })
    results.Add( new CombinedAltEntry with { .Amps=200.00	,.EngineSpeed=2000	,.Efficiency=0.37   })
    results.Add( new CombinedAltEntry with { .Amps=10.00	,.EngineSpeed=2333	,.Efficiency=0.70   })
    results.Add( new CombinedAltEntry with { .Amps=25.00	,.EngineSpeed=2333	,.Efficiency=0.67   })
    results.Add( new CombinedAltEntry with { .Amps=50.00	,.EngineSpeed=2333	,.Efficiency=0.63   })
    results.Add( new CombinedAltEntry with { .Amps=75.00	,.EngineSpeed=2333	,.Efficiency=0.58   })
    results.Add( new CombinedAltEntry with { .Amps=100.00	,.EngineSpeed=2333	,.Efficiency=0.52   })
    results.Add( new CombinedAltEntry with { .Amps=150.00	,.EngineSpeed=2333	,.Efficiency=0.39   })
    results.Add( new CombinedAltEntry with { .Amps=200.00	,.EngineSpeed=2333	,.Efficiency=0.26   })


Return results


End Function
Public  Function TwoAltValues() As List( of CombinedAltEntry)

  Dim results As New List( Of CombinedAltEntry)

   Results.Add( new CombinedAltEntry With { .AMPS=20.00	    ,.EngineSpeed=500	,.Efficiency=0.75   })
   Results.Add( new CombinedAltEntry With { .AMPS=50.00	    ,.EngineSpeed=500	,.Efficiency=0.75   })
   Results.Add( new CombinedAltEntry With { .AMPS=100.00	,.EngineSpeed=500	,.Efficiency=0.75   })
   Results.Add( new CombinedAltEntry With { .AMPS=150.00	,.EngineSpeed=500	,.Efficiency=0.75   })
   Results.Add( new CombinedAltEntry With { .AMPS=200.00	,.EngineSpeed=500	,.Efficiency=0.75   })
   Results.Add( new CombinedAltEntry With { .AMPS=300.00	,.EngineSpeed=500	,.Efficiency=0.75   })
   Results.Add( new CombinedAltEntry With { .AMPS=400.00	,.EngineSpeed=500	,.Efficiency=0.75   })
   Results.Add( new CombinedAltEntry With { .AMPS=20.00	    ,.EngineSpeed=667	,.Efficiency=0.72   })
   Results.Add( new CombinedAltEntry With { .AMPS=50.00	    ,.EngineSpeed=667	,.Efficiency=0.72   })
   Results.Add( new CombinedAltEntry With { .AMPS=100.00	,.EngineSpeed=667	,.Efficiency=0.72   })
   Results.Add( new CombinedAltEntry With { .AMPS=150.00	,.EngineSpeed=667	,.Efficiency=0.72   })
   Results.Add( new CombinedAltEntry With { .AMPS=200.00	,.EngineSpeed=667	,.Efficiency=0.72   })
   Results.Add( new CombinedAltEntry With { .AMPS=300.00	,.EngineSpeed=667	,.Efficiency=0.72   })
   Results.Add( new CombinedAltEntry With { .AMPS=400.00	,.EngineSpeed=667	,.Efficiency=0.72   })
   Results.Add( new CombinedAltEntry With { .AMPS=20.00	    ,.EngineSpeed=1333	,.Efficiency=0.72   })
   Results.Add( new CombinedAltEntry With { .AMPS=50.00	    ,.EngineSpeed=1333	,.Efficiency=0.72   })
   Results.Add( new CombinedAltEntry With { .AMPS=100.00	,.EngineSpeed=1333	,.Efficiency=0.72   })
   Results.Add( new CombinedAltEntry With { .AMPS=150.00	,.EngineSpeed=1333	,.Efficiency=0.72   })
   Results.Add( new CombinedAltEntry With { .AMPS=200.00	,.EngineSpeed=1333	,.Efficiency=0.72   })
   Results.Add( new CombinedAltEntry With { .AMPS=300.00	,.EngineSpeed=1333	,.Efficiency=0.72   })
   Results.Add( new CombinedAltEntry With { .AMPS=400.00	,.EngineSpeed=1333	,.Efficiency=0.72   })
   Results.Add( new CombinedAltEntry With { .AMPS=20.00	    ,.EngineSpeed=2000	,.Efficiency=0.90   })
   Results.Add( new CombinedAltEntry With { .AMPS=50.00	    ,.EngineSpeed=2000	,.Efficiency=0.89   })
   Results.Add( new CombinedAltEntry With { .AMPS=100.00	,.EngineSpeed=2000	,.Efficiency=0.87   })
   Results.Add( new CombinedAltEntry With { .AMPS=150.00	,.EngineSpeed=2000	,.Efficiency=0.84   })
   Results.Add( new CombinedAltEntry With { .AMPS=200.00	,.EngineSpeed=2000	,.Efficiency=0.82   })
   Results.Add( new CombinedAltEntry With { .AMPS=300.00	,.EngineSpeed=2000	,.Efficiency=0.78   })
   Results.Add( new CombinedAltEntry With { .AMPS=400.00	,.EngineSpeed=2000	,.Efficiency=0.74   })
   Results.Add( new CombinedAltEntry With { .AMPS=20.00	    ,.EngineSpeed=2333	,.Efficiency=0.80   })
   Results.Add( new CombinedAltEntry With { .AMPS=50.00	    ,.EngineSpeed=2333	,.Efficiency=0.79   })
   Results.Add( new CombinedAltEntry With { .AMPS=100.00	,.EngineSpeed=2333	,.Efficiency=0.77   })
   Results.Add( new CombinedAltEntry With { .AMPS=150.00	,.EngineSpeed=2333	,.Efficiency=0.74   })
   Results.Add( new CombinedAltEntry With { .AMPS=200.00	,.EngineSpeed=2333	,.Efficiency=0.71   })
   Results.Add( new CombinedAltEntry With { .AMPS=300.00	,.EngineSpeed=2333	,.Efficiency=0.64   })
   Results.Add( new CombinedAltEntry With { .AMPS=400.00	,.EngineSpeed=2333	,.Efficiency=0.58   })
   

  Return results


End Function
Public  Function ThreeAltValues()    As List( of CombinedAltEntry)

  Dim results As New List( Of CombinedAltEntry)

  results.Add( new CombinedAltEntry With { .AMPS=30	    ,.EngineSpeed = 500	    ,.Efficiency = 0.71    })
  results.Add( new CombinedAltEntry With { .AMPS=75	    ,.EngineSpeed = 500	    ,.Efficiency = 0.73    })
  results.Add( new CombinedAltEntry With { .AMPS=150	,.EngineSpeed = 500	    ,.Efficiency = 0.77    })
  results.Add( new CombinedAltEntry With { .AMPS=225	,.EngineSpeed = 500	    ,.Efficiency = 0.00    })
  results.Add( new CombinedAltEntry With { .AMPS=300	,.EngineSpeed = 500	    ,.Efficiency = 0.00    })
  results.Add( new CombinedAltEntry With { .AMPS=450	,.EngineSpeed = 500	    ,.Efficiency = 0.00    })
  results.Add( new CombinedAltEntry With { .AMPS=600	,.EngineSpeed = 500	    ,.Efficiency = 0.00    })
  results.Add( new CombinedAltEntry With { .AMPS=30	    ,.EngineSpeed = 667	    ,.Efficiency = 0.69    })
  results.Add( new CombinedAltEntry With { .AMPS=75	    ,.EngineSpeed = 667	    ,.Efficiency = 0.70    })
  results.Add( new CombinedAltEntry With { .AMPS=150	,.EngineSpeed = 667	    ,.Efficiency = 0.73    })
  results.Add( new CombinedAltEntry With { .AMPS=225	,.EngineSpeed = 667	    ,.Efficiency = 0.22    })
  results.Add( new CombinedAltEntry With { .AMPS=300	,.EngineSpeed = 667	    ,.Efficiency = 0.00    })
  results.Add( new CombinedAltEntry With { .AMPS=450	,.EngineSpeed = 667	    ,.Efficiency = 0.00    })
  results.Add( new CombinedAltEntry With { .AMPS=600	,.EngineSpeed = 667	    ,.Efficiency = 0.00    })
  results.Add( new CombinedAltEntry With { .AMPS=30	    ,.EngineSpeed = 1333	,.Efficiency = 0.64    })
  results.Add( new CombinedAltEntry With { .AMPS=75	    ,.EngineSpeed = 1333	,.Efficiency = 0.66    })
  results.Add( new CombinedAltEntry With { .AMPS=150	,.EngineSpeed = 1333	,.Efficiency = 0.69    })
  results.Add( new CombinedAltEntry With { .AMPS=225	,.EngineSpeed = 1333	,.Efficiency = 0.70    })
  results.Add( new CombinedAltEntry With { .AMPS=300	,.EngineSpeed = 1333	,.Efficiency = 0.69    })
  results.Add( new CombinedAltEntry With { .AMPS=450	,.EngineSpeed = 1333	,.Efficiency = 0.67    })
  results.Add( new CombinedAltEntry With { .AMPS=600	,.EngineSpeed = 1333	,.Efficiency = 0.65    })
  results.Add( new CombinedAltEntry With { .AMPS=30	    ,.EngineSpeed = 2000	,.Efficiency = 0.70    })
  results.Add( new CombinedAltEntry With { .AMPS=75	    ,.EngineSpeed = 2000	,.Efficiency = 0.72    })
  results.Add( new CombinedAltEntry With { .AMPS=150	,.EngineSpeed = 2000	,.Efficiency = 0.75    })
  results.Add( new CombinedAltEntry With { .AMPS=225	,.EngineSpeed = 2000	,.Efficiency = 0.77    })
  results.Add( new CombinedAltEntry With { .AMPS=300	,.EngineSpeed = 2000	,.Efficiency = 0.74    })
  results.Add( new CombinedAltEntry With { .AMPS=450	,.EngineSpeed = 2000	,.Efficiency = 0.69    })
  results.Add( new CombinedAltEntry With { .AMPS=600	,.EngineSpeed = 2000	,.Efficiency = 0.63    })
  results.Add( new CombinedAltEntry With { .AMPS=30	    ,.EngineSpeed = 2333	,.Efficiency = 0.61    })
  results.Add( new CombinedAltEntry With { .AMPS=75	    ,.EngineSpeed = 2333	,.Efficiency = 0.63    })
  results.Add( new CombinedAltEntry With { .AMPS=150	,.EngineSpeed = 2333	,.Efficiency = 0.66    })
  results.Add( new CombinedAltEntry With { .AMPS=225	,.EngineSpeed = 2333	,.Efficiency = 0.69    })
  results.Add( new CombinedAltEntry With { .AMPS=300	,.EngineSpeed = 2333	,.Efficiency = 0.65    })
  results.Add( new CombinedAltEntry With { .AMPS=450	,.EngineSpeed = 2333	,.Efficiency = 0.58    })
  results.Add( new CombinedAltEntry With { .AMPS=600	,.EngineSpeed = 2333	,.Efficiency = 0.51    })



  Return results
  

End Function
Public  Function FourAltValues()  As List( of CombinedAltEntry)

  Dim results As New List( Of CombinedAltEntry)


   results.Add( new CombinedAltEntry With {.AMPS=40	    ,.EngineSpeed=500	    ,.Efficiency=0.61    })
   results.Add( new CombinedAltEntry With {.AMPS=100.00	,.EngineSpeed=500	    ,.Efficiency=0.68    })
   results.Add( new CombinedAltEntry With {.AMPS=200.00	,.EngineSpeed=500	    ,.Efficiency=0.78    })
   results.Add( new CombinedAltEntry With {.AMPS=300.00	,.EngineSpeed=500	    ,.Efficiency=0.00    })
   results.Add( new CombinedAltEntry With {.AMPS=400.00	,.EngineSpeed=500	    ,.Efficiency=0.00    })
   results.Add( new CombinedAltEntry With {.AMPS=600.00	,.EngineSpeed=500	    ,.Efficiency=0.00    })
   results.Add( new CombinedAltEntry With {.AMPS=800.00	,.EngineSpeed=500	    ,.Efficiency=0.00    })
   results.Add( new CombinedAltEntry With {.AMPS=40.00	,.EngineSpeed=667	    ,.Efficiency=0.68    })
   results.Add( new CombinedAltEntry With {.AMPS=100.00	,.EngineSpeed=667	    ,.Efficiency=0.70    })
   results.Add( new CombinedAltEntry With {.AMPS=200.00	,.EngineSpeed=667	    ,.Efficiency=0.73    })
   results.Add( new CombinedAltEntry With {.AMPS=300.00	,.EngineSpeed=667	    ,.Efficiency=0.35    })
   results.Add( new CombinedAltEntry With {.AMPS=400.00	,.EngineSpeed=667	    ,.Efficiency=0.00    })
   results.Add( new CombinedAltEntry With {.AMPS=600.00	,.EngineSpeed=667	    ,.Efficiency=0.00    })
   results.Add( new CombinedAltEntry With {.AMPS=800.00	,.EngineSpeed=667	    ,.Efficiency=0.00    })
   results.Add( new CombinedAltEntry With {.AMPS=40.00	,.EngineSpeed=1333	    ,.Efficiency=0.59    })
   results.Add( new CombinedAltEntry With {.AMPS=100.00	,.EngineSpeed=1333	    ,.Efficiency=0.61    })
   results.Add( new CombinedAltEntry With {.AMPS=200.00	,.EngineSpeed=1333	    ,.Efficiency=0.66    })
   results.Add( new CombinedAltEntry With {.AMPS=300.00	,.EngineSpeed=1333	    ,.Efficiency=0.69    })
   results.Add( new CombinedAltEntry With {.AMPS=400.00	,.EngineSpeed=1333	    ,.Efficiency=0.68    })
   results.Add( new CombinedAltEntry With {.AMPS=600.00	,.EngineSpeed=1333	    ,.Efficiency=0.64    })
   results.Add( new CombinedAltEntry With {.AMPS=800.00	,.EngineSpeed=1333	    ,.Efficiency=0.61    })
   results.Add( new CombinedAltEntry With {.AMPS=40.00	,.EngineSpeed=2000	    ,.Efficiency=0.58    })
   results.Add( new CombinedAltEntry With {.AMPS=100.00	,.EngineSpeed=2000	    ,.Efficiency=0.61    })
   results.Add( new CombinedAltEntry With {.AMPS=200.00	,.EngineSpeed=2000	    ,.Efficiency=0.67    })
   results.Add( new CombinedAltEntry With {.AMPS=300.00	,.EngineSpeed=2000	    ,.Efficiency=0.72    })
   results.Add( new CombinedAltEntry With {.AMPS=400.00	,.EngineSpeed=2000	    ,.Efficiency=0.69    })
   results.Add( new CombinedAltEntry With {.AMPS=600.00	,.EngineSpeed=2000	    ,.Efficiency=0.63    })
   results.Add( new CombinedAltEntry With {.AMPS=800.00	,.EngineSpeed=2000	    ,.Efficiency=0.56    })
   results.Add( new CombinedAltEntry With {.AMPS=40.00	,.EngineSpeed=2333	    ,.Efficiency=0.48    })
   results.Add( new CombinedAltEntry With {.AMPS=100.00	,.EngineSpeed=2333	    ,.Efficiency=0.52    })
   results.Add( new CombinedAltEntry With {.AMPS=200.00	,.EngineSpeed=2333	    ,.Efficiency=0.59    })
   results.Add( new CombinedAltEntry With {.AMPS=300.00	,.EngineSpeed=2333	    ,.Efficiency=0.65    })
   results.Add( new CombinedAltEntry With {.AMPS=400.00	,.EngineSpeed=2333	    ,.Efficiency=0.61    })
   results.Add( new CombinedAltEntry With {.AMPS=600.00	,.EngineSpeed=2333	    ,.Efficiency=0.53    })
   results.Add( new CombinedAltEntry With {.AMPS=800.00	,.EngineSpeed=2333	    ,.Efficiency=0.45    })


  Return results

End Function


Public Function ValueSetsEqual( set1 As List(Of CombinedAltEntry),set2 As List(Of CombinedAltEntry)) As Boolean

   'Bsic Count Check
   If set1.Count<> set2.Count then Return False
   
   'Values Check, position for position
   For i As Integer = 0 to set1.Count-1

     If set1(i).Amps<> set2(i).Amps OrElse set1(i).EngineSpeed<> set2(i).EngineSpeed OrElse set1(i).Efficiency <> set2(i).Efficiency then
       Return False    
     End If

   Next

   Return true

End Function





End Class





End Namespace



