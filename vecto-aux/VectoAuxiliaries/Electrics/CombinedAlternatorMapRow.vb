Namespace Electrics

'This class is reflective of the stored entries for the combined alternator
'And is used by the Combined Alternator Form and any related classes.

    Public Class CombinedAlternatorMapRow
        Implements ICombinedAlternatorMapRow

      Public Property AlternatorName  As String  implements ICombinedAlternatorMapRow.AlternatorName
      Public Property RPM             As Single  implements ICombinedAlternatorMapRow.RPM
      Public Property Amps            As Single  implements ICombinedAlternatorMapRow.Amps
      Public Property Efficiency      As Single  implements ICombinedAlternatorMapRow.Efficiency
      Public Property PulleyRatio     As Single  implements ICombinedAlternatorMapRow.PulleyRatio

    'Constructors
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



