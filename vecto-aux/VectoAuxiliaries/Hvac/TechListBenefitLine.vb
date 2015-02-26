
Namespace Hvac

Public Enum TechLineType 

  Normal
  DriverACMechanical
  DriverACElectrical
  HVCActiveSelection

End Enum

Public Enum PowerType

  Mechanical
  Electrical

End Enum

Public Class TechListBenefitLine
  Implements ITechListBenefitLine




Private _h,_vh,_vv,_vc,_c As Single
Private inputSheet As ISSMGenInputs

Public Property  Units  As string Implements ITechListBenefitLine.Units
Public Property  Category As String Implements ITechListBenefitLine.Category
Public Property  BenefitName As String Implements ITechListBenefitLine.BenefitName
Public Property  LowFloorH As new Double Implements ITechListBenefitLine.LowFloorH
Public Property  LowFloorV As new Double Implements ITechListBenefitLine.LowFloorV
Public Property  LowFloorC As new Double Implements ITechListBenefitLine.LowFloorC

Public Property  SemiLowFloorH As New Double Implements ITechListBenefitLine.SemiLowFloorH
Public Property  SemiLowFloorV As New Double Implements ITechListBenefitLine.SemiLowFloorV
Public Property  SemiLowFloorC As New Double Implements ITechListBenefitLine.SemiLowFloorC

Public Property  RaisedFloorH As New Double Implements ITechListBenefitLine.RaisedFloorH
Public Property  RaisedFloorV As New Double Implements ITechListBenefitLine.RaisedFloorV
Public Property  RaisedFloorC As New Double Implements ITechListBenefitLine.RaisedFloorC


Public Property  OnVehicle As Boolean Implements ITechListBenefitLine.OnVehicle
Public Property  ActiveVH As Boolean Implements ITechListBenefitLine.ActiveVH
Public Property  ActiveVV As Boolean Implements ITechListBenefitLine.ActiveVV
Public Property  ActiveVC As Boolean Implements ITechListBenefitLine.ActiveVC
Public Property  LineType As TechLineType Implements ITechListBenefitLine.LineType


Public ReadOnly Property  H As Single Implements ITechListBenefitLine.H
    Get

      Dim returnValue As Single =0

      '=IF($M49=0,0,IF(AND($M49=1,'INPUT & RESULTS SHEET'!$D$6="low floor"),'TECH LIST INPUT'!D49, IF(AND($M49=1,'INPUT & RESULTS SHEET'!$D$6="semi low floor"),'TECH LIST INPUT'!G49,'TECH LIST INPUT'!J49)))
        If Not OnVehicle then return returnValue

        Select Case inputSheet.BP_BusFloorType
            Case "low floor"
                 returnValue = LowFloorH
            Case "semi low floor"
                 returnValue= SemiLowFloorH
            Case "raised floor"
                 returnValue= RaisedFloorH
        End Select

        Return returnValue

    End Get
End Property
Public ReadOnly Property VH As Single Implements ITechListBenefitLine.VH
    Get

       Dim floorValue As Single =0

       If Not OnVehicle then  Return floorValue 

       'Get floor value
       Select Case inputSheet.BP_BusFloorType
         Case "low floor"
              floorValue = LowFloorV
         Case "semi low floor"
              floorValue= SemiLowFloorV
         Case "raised floor"
              floorValue= RaisedFloorV
       End Select


        'Active
        If  TechLineType.HVCActiveSelection AndAlso ActiveVH then
          Return floorValue
        else
          Return 0
       End if


    End Get

End Property
Public ReadOnly Property VV As Single Implements ITechListBenefitLine.VV
    Get

         Dim floorValue As Single =0

       If Not OnVehicle then  Return floorValue 

       'Get floor value
       Select Case inputSheet.BP_BusFloorType
         Case "low floor"
              floorValue = LowFloorV
         Case "semi low floor"
              floorValue= SemiLowFloorV
         Case "raised floor"
              floorValue= RaisedFloorV
       End Select


        'Active
        If  TechLineType.HVCActiveSelection AndAlso ActiveVV then
          Return floorValue
        else
          Return 0
       End if

    End Get
End Property
Public ReadOnly Property VC As Single Implements ITechListBenefitLine.VC
    Get

       Dim floorValue As Single =0

       If Not OnVehicle then  Return floorValue 

       'Get floor value
       Select Case inputSheet.BP_BusFloorType
         Case "low floor"
              floorValue = LowFloorV
         Case "semi low floor"
              floorValue= SemiLowFloorV
         Case "raised floor"
              floorValue= RaisedFloorV
       End Select


        'Active
        If  TechLineType.HVCActiveSelection AndAlso ActiveVC then
          Return floorValue
        else
          Return 0
       End if

    End Get
End Property
Public ReadOnly Property C As Single Implements ITechListBenefitLine.C
    Get

      Dim returnValue As Single =0


 
        If Not OnVehicle then return returnValue

        Select Case inputSheet.BP_BusFloorType
            Case "low floor"
                 returnValue = LowFloorC
            Case "semi low floor"
                 returnValue= SemiLowFloorC
            Case "raised floor"
                 returnValue= RaisedFloorC
        End Select

        Return returnValue

    End Get
End Property

Sub new ( geninputs As ISSMGenInputs )

   Me.inputSheet     =   geninputs  

End Sub

Sub new ( geninputs As ISSMGenInputs, 
           units As string,
           category As String, 
           benefitName As String, 
           lowFloorH As double, 
           lowFloorV As double, 
           lowFloorC As double, 
           semiLowFloorH As Double, 
           semiLowFloorV As Double, 
           semiLowFloorC As Double, 
           raisedFloorH As Double, 
           raisedFloorV As Double, 
           raisedFloorC As Double, 
           onVehicle As Boolean,
           lineType As TechLineType,
           activeVH As Boolean,
           activeVV As Boolean,
           activeVC As Boolean
          )

   Me.inputSheet     =   geninputs  
   Me.Units          =   Units       
   Me.category       =   category    
   Me.benefitName    =   benefitName 
   Me.lowFloorH      =   lowFloorH   
   Me.lowFloorV      =   lowFloorV   
   Me.lowFloorC      =   lowFloorC   
   Me.semiLowFloorH  =   semiLowFloorH
   Me.semiLowFloorV  =   semiLowFloorV
   Me.semiLowFloorC  =   semiLowFloorC
   Me.raisedFloorH   =   raisedFloorH
   Me.raisedFloorV   =   raisedFloorV
   Me.raisedFloorC   =   raisedFloorC
   Me.OnVehicle      = onVehicle
   Me.lineType       = lineType    
   Me.ActiveVH       = activeVH 
   Me.ActiveVV       = activeVV 
   Me.ActiveVC       = activeVC 
      
      
End Sub

'Operator Overloads
 Public Shared Operator = (ByVal op1 as TechListBenefitLine, ByVal op2 as TechListBenefitLine) As Boolean

        If ( op1.Category        = op2.Category        AndAlso _
             op1.BenefitName     = op2.BenefitName     AndAlso _
             op1.ActiveVC        = op2.ActiveVC        AndAlso _
             op1.ActiveVH        = op2.ActiveVH        AndAlso _
             op1.ActiveVV        = op2.ActiveVV        AndAlso _
             op1.LineType        = op2.LineType        AndAlso _
             op1.LowFloorC       = op2.LowFloorC       AndAlso _
             op1.LowFloorV       = op2.LowFloorV       AndAlso _
             op1.LowFloorH       = op2.LowFloorH       AndAlso _
             op1.SemiLowFloorC   = op2.SemiLowFloorC   AndAlso _
             op1.SemiLowFloorH   = op2.SemiLowFloorH   AndAlso _
             op1.SemiLowFloorV   = op2.SemiLowFloorV   AndAlso _
             op1.RaisedFloorC    = op2.RaisedFloorC    AndAlso _
             op1.RaisedFloorH    = op2.RaisedFloorH    AndAlso _
             op1.RaisedFloorV    = op2.RaisedFloorV    AndAlso _
             op1.OnVehicle       = op2.OnVehicle       AndAlso _
             op1.Units           = op2.Units ) then

          Return True

        Else

          Return False

        End If


End Operator
 Public Shared Operator <> (ByVal op1 as TechListBenefitLine, ByVal op2 as TechListBenefitLine) As Boolean

        If ( op1.Category        <> op2.Category        OrElse _
             op1.BenefitName     <> op2.BenefitName     OrElse _
             op1.ActiveVC        <> op2.ActiveVC        OrElse _
             op1.ActiveVH        <> op2.ActiveVH        OrElse _
             op1.ActiveVV        <> op2.ActiveVV        OrElse _           
             op1.LineType        <> op2.LineType        OrElse _
             op1.LowFloorC       <> op2.LowFloorC       OrElse _
             op1.LowFloorV       <> op2.LowFloorV       OrElse _
             op1.LowFloorH       <> op2.LowFloorH       OrElse _
             op1.SemiLowFloorC   <> op2.SemiLowFloorC   OrElse _
             op1.SemiLowFloorH   <> op2.SemiLowFloorH   OrElse _
             op1.SemiLowFloorV   <> op2.SemiLowFloorV   OrElse _
             op1.RaisedFloorC    <> op2.RaisedFloorC    OrElse _
             op1.RaisedFloorH    <> op2.RaisedFloorH    OrElse _
             op1.RaisedFloorV    <> op2.RaisedFloorV    OrElse _
             op1.OnVehicle       <> op2.OnVehicle       OrElse _
             op1.Units           <> op2.Units ) then

          Return True

        Else

          Return False

        End If


End Operator

 
 Public Sub CloneFrom(source As ITechListBenefitLine) Implements ITechListBenefitLine.CloneFrom


     Me.Units          = source.Units        
     Me.Category       = source.Category     
     Me.BenefitName    = source.BenefitName  
     Me.LowFloorH      = source.LowFloorH 
     Me.LowFloorV      = source.LowFloorV
     Me.LowFloorC      = source.LowFloorC      
         
     Me.SemiLowFloorH  = source.SemiLowFloorH 
     Me.SemiLowFloorV  = source.SemiLowFloorV 
     Me.SemiLowFloorC  = source.SemiLowFloorC 

     Me.RaisedFloorH   = source.RaisedFloorH  
     Me.RaisedFloorV   = source.RaisedFloorV  
     Me.RaisedFloorC   = source.RaisedFloorC  

     Me.OnVehicle      = source.OnVehicle    
     Me.ActiveVH       = source.ActiveVH     
     Me.ActiveVV       = source.ActiveVV     
     Me.ActiveVC       = source.ActiveVC     
     Me.LineType       = source.LineType     


 End Sub
 Public Function IsEqualTo(source As ITechListBenefitLine) As boolean Implements ITechListBenefitLine.IsEqualTo

      Return If(Me=source,True,False)
      

    End Function



End Class

End Namespace


