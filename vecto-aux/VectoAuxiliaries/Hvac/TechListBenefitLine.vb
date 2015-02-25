
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
Public Property  LowFloor As new BusFloorLow Implements ITechListBenefitLine.LowFloor
Public Property  SemiLowFloor As New BusFloorSemiLow Implements ITechListBenefitLine.SemiLowFloor
Public Property  RaisedFloor As New BusFloorRaised Implements ITechListBenefitLine.RaisedFloor

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
                 returnValue = LowFloor.H
            Case "semi low floor"
                 returnValue= SemiLowFloor.H
            Case "raised floor"
                 returnValue= RaisedFloor.H
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
              floorValue = LowFloor.V
         Case "semi low floor"
              floorValue= SemiLowFloor.V
         Case "raised floor"
              floorValue= RaisedFloor.V
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
              floorValue = LowFloor.V
         Case "semi low floor"
              floorValue= SemiLowFloor.V
         Case "raised floor"
              floorValue= RaisedFloor.V
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
              floorValue = LowFloor.V
         Case "semi low floor"
              floorValue= SemiLowFloor.V
         Case "raised floor"
              floorValue= RaisedFloor.V
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
                 returnValue = LowFloor.C
            Case "semi low floor"
                 returnValue= SemiLowFloor.C
            Case "raised floor"
                 returnValue= RaisedFloor.C
        End Select

        Return returnValue

    End Get
End Property


Sub new ( geninputs As ISSMGenInputs, 
           units As string,
           category As String, 
           benefitName As String, 
           lowFloor As BusFloorLow, 
           semiLowFloor As BusFloorSemiLow, 
           raisedFloor As BusFloorRaised, 
           onVehicle As Boolean,
           lineType As TechLineType,
           activeVH As Boolean,
           activeVV As Boolean,
           activeVC As Boolean
          )

   Me.inputSheet     = geninputs 
   Me.Units          = units
   Me.category       = category    
   Me.benefitName    = benefitName 
   Me.lowFloor       = lowFloor    
   Me.semiLowFloor   = semiLowFloor
   Me.raisedFloor    = raisedFloor 
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
             op1.H               = op2.H               AndAlso _
             op1.LineType        = op2.LineType        AndAlso _
             op1.LowFloor.V      = op2.LowFloor.V      AndAlso _
             op1.SemiLowFloor.C  = op2.SemiLowFloor.C  AndAlso _
             op1.SemiLowFloor.H  = op2.SemiLowFloor.H  AndAlso _
             op1.SemiLowFloor.V  = op2.SemiLowFloor.V  AndAlso _
             op1.RaisedFloor.C   = op2.RaisedFloor.C   AndAlso _
             op1.RaisedFloor.H   = op2.RaisedFloor.H   AndAlso _
             op1.RaisedFloor.V   = op2.RaisedFloor.V   AndAlso _
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
             op1.H               <> op2.H               OrElse _
             op1.LineType        <> op2.LineType        OrElse _
             op1.LowFloor.V      <> op2.LowFloor.V      OrElse _
             op1.SemiLowFloor.C  <> op2.SemiLowFloor.C  OrElse _
             op1.SemiLowFloor.H  <> op2.SemiLowFloor.H  OrElse _
             op1.SemiLowFloor.V  <> op2.SemiLowFloor.V  OrElse _
             op1.RaisedFloor.C   <> op2.RaisedFloor.C   OrElse _
             op1.RaisedFloor.H   <> op2.RaisedFloor.H   OrElse _
             op1.RaisedFloor.V   <> op2.RaisedFloor.V   OrElse _
             op1.Units           <> op2.Units ) then

          Return True

        Else

          Return False

        End If


End Operator


 Public Sub CloneFrom(source As ITechListBenefitLine) Implements ITechListBenefitLine.CloneFrom


     Me.Units         = source.Units        
     Me.Category      = source.Category     
     Me.BenefitName   = source.BenefitName  
     Me.LowFloor      = source.LowFloor     
     Me.SemiLowFloor  = source.SemiLowFloor 
     Me.RaisedFloor   = source.RaisedFloor  
     Me.OnVehicle     = source.OnVehicle    
     Me.ActiveVH      = source.ActiveVH     
     Me.ActiveVV      = source.ActiveVV     
     Me.ActiveVC      = source.ActiveVC     
     Me.LineType      = source.LineType     


 End Sub



End Class

End Namespace


