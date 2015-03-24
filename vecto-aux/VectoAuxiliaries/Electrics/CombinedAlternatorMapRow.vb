Namespace Electrics


    Public Class CombinedAlternatorMapRow
        Implements ICombinedAlternatorMapRow

      public property AlternatorName  As String  implements ICombinedAlternatorMapRow.AlternatorName
      public property RPM             As Single  implements ICombinedAlternatorMapRow.RPM
      public property Amps            As Single  implements ICombinedAlternatorMapRow.Amps
      public property Efficiency      As Single  implements ICombinedAlternatorMapRow.Efficiency
      public property PulleyRatio     As Single  implements ICombinedAlternatorMapRow.PulleyRatio

    Sub new ()


    End Sub

    Sub new (AlternatorName As string, RPM As single  ,Amps As single, Efficiency As single , PulleyRatio As single )

        'Sanity Check
        If AlternatorName.Trim.Length=0 then Throw New ArgumentException("Alternator name cannot be zero length")
        If Efficiency<0 or Efficiency>100 then  Throw New ArgumentException("Efficiency must be between 0 and 100")
        If PulleyRatio<=0 then  Throw New ArgumentException("Pully ratio must be a positive number")

        'Assignments
        Me.AlternatorName   =  AlternatorName  
        Me.RPM              =  RPM             
        Me.Amps             =  Amps            
        Me.Efficiency       =  Efficiency      
        Me.PulleyRatio      =  PulleyRatio     
                                                                          
    End Sub


End Class


End Namespace



