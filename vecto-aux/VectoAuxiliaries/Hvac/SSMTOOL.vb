Imports Omu.ValueInjecter
Imports VectoAuxiliaries.Hvac
Imports Newtonsoft.Json
Imports System.IO
Imports System.Reflection
Imports System.Text

Namespace Hvac

Public Class SSMTOOL
Implements ISSMTOOL

 Private filePath As String
 Public genInputs As ISSMGenInputs
 Public techList As ISSMTechList


 'Public facing properties, final results from calculations.
 Public ReadOnly Property ElectricalWAdjusted As Single Implements ISSMTOOL.ElectricalWAdjusted
   Get

   End Get
 End Property
 Public ReadOnly Property ElectricalWBase As Single Implements ISSMTOOL.ElectricalWBase
    Get

            End Get
 End Property
 Public ReadOnly Property FuelLPerHBase As Single Implements ISSMTOOL.FuelLPerHBase
    Get

            End Get
 End Property

 Public ReadOnly Property FuelLPerHBaseAdjusted As Single Implements ISSMTOOL.FuelLPerHBaseAdjusted
    Get

            End Get
 End Property
 Public ReadOnly Property MechanicalWBase As Single Implements ISSMTOOL.MechanicalWBase
    Get

            End Get
 End Property
 Public ReadOnly Property MechanicalWBaseAdjusted As Single Implements ISSMTOOL.MechanicalWBaseAdjusted
    Get

            End Get
 End Property


 'Constructors
 Sub New()


 End Sub
 Sub New(filePath As String)

   Me.filePath = filePath
   genInputs = New SSMGenInputs()
   techList = New SSMTechList(filePath, genInputs)

 End Sub

 'Clone values from another object of same type
 Public Sub Clone(from As ISSMTOOL) Implements ISSMTOOL.Clone

     Dim feedback As String = String.Empty

     genInputs.InjectFrom(DirectCast(from, SSMTOOL).genInputs)

     techList.Clear()

     For Each line As TechListBenefitLine In DirectCast(from, SSMTOOL).techList.TechLines

         techList.Add(line, feedback)

    Next

  End Sub

 'Persistance Functions
 Public Function Save(filePath As String) As Boolean Implements ISSMTOOL.Save

   Dim returnValue As Boolean = True
   Dim settings As JsonSerializerSettings = New JsonSerializerSettings()
   settings.TypeNameHandling = TypeNameHandling.Objects

    'JSON METHOD
    Try

       Dim output As String = JsonConvert.SerializeObject(Me, Formatting.Indented, settings)

       File.WriteAllText(filePath, output)

       Catch ex As Exception

         'TODO:Do something meaningfull here perhaps logging
          returnValue = False

     End Try

   Return returnValue

End Function
 Public Function Load(filePath As String) As Boolean Implements ISSMTOOL.Load

    Dim returnValue As Boolean = True
    Dim settings As JsonSerializerSettings = New JsonSerializerSettings()
    Dim tmpAux As SSMTOOL = New SSMTOOL(filePath)

    settings.TypeNameHandling = TypeNameHandling.Objects

     'JSON METHOD
     Try

       Dim output As String = File.ReadAllText(filePath)


       tmpAux = JsonConvert.DeserializeObject(Of SSMTOOL)(output, settings)

       tmpAux.techList.SetSSMGeneralInputs(tmpAux.genInputs)

       For Each tll As TechListBenefitLine In tmpAux.techList.TechLines

         tll.inputSheet = tmpAux.genInputs

       Next



       'This is where we Assume values of loaded( Deserialized ) object.
       Clone(tmpAux)

      Catch ex As Exception

        'TODO:Do something meaningfull here perhaps logging

         returnValue = False
      End Try

    Return returnValue

End Function

 'Comparison
 Public Function IsEqualTo(source As ISSMTOOL) As Boolean Implements ISSMTOOL.IsEqualTo

  'In this methods we only want to compare the non Static , non readonly public properties of 
  'The class's General, User Inputs and  Tech Benefit members.

   Return compareGenUserInputs(source) AndAlso compareTechListBenefitLines(source)


 End Function
 Private Function compareGenUserInputs(source As ISSMTOOL) As Boolean

   Dim src As SSMTOOL = DirectCast(source, SSMTOOL)

   Dim returnValue As Boolean = True

   Dim properties As PropertyInfo() = Me.genInputs.GetType.GetProperties

     For Each prop As PropertyInfo In properties

        If Not prop.GetAccessors.IsReadOnly Then

             If prop.GetValue(Me.genInputs, Nothing) <> prop.GetValue(src.genInputs, Nothing) Then
                    returnValue = False
             End If

        End If

     Next

   Return returnValue

 End Function
 Private Function compareTechListBenefitLines(source As ISSMTOOL) As Boolean


   Dim src As SSMTOOL = DirectCast(source, SSMTOOL)

   'Equal numbers of lines check
   If Me.techList.TechLines.Count <> src.techList.TechLines.Count Then Return False

     For Each tl As ITechListBenefitLine In Me.techList.TechLines.OrderBy(Function(o) o.Category).ThenBy(Function(n) n.BenefitName)

        'First Check line exists in other
        If src.techList.TechLines.Where(Function(w) w.BenefitName = tl.BenefitName AndAlso w.Category = tl.Category).Count <> 1 Then

         Return False
        Else

         'check are equal

           If Not src.techList.TechLines.First(Function(w) w.BenefitName = tl.BenefitName AndAlso w.Category = tl.Category).IsEqualTo(tl) Then
             Return False
           End If

        End If


     Next

     'All Looks OK
     Return True

 End Function


End Class


End Namespace



