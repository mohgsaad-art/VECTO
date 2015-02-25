Namespace Hvac


Public Interface ITechListBenefitLine

 Property  Units  As string
 Property  Category As String
 Property  BenefitName As String

 Property  LowFloorH As double
 Property  LowFloorV As double
 Property  LowFloorC As double

 Property  SemiLowFloorH As double
 Property  SemiLowFloorV As double
 Property  SemiLowFloorC As double

 Property  RaisedFloorH  As double
 Property  RaisedFloorV  As double
 Property  RaisedFloorC  As double

 Property  OnVehicle As Boolean
 Property  ActiveVH As Boolean
 Property  ActiveVV As Boolean
 Property  ActiveVC As Boolean
 Property  LineType As TechLineType

 ReadOnly Property  H As Single
 ReadOnly Property VH As Single
 ReadOnly Property VV As Single
 ReadOnly Property VC As Single
 ReadOnly Property C  As Single

 Sub CloneFrom( source As ITechListBenefitLine)
 
 Function IsEqualTo(source As ITechListBenefitLine ) As Boolean
 


End Interface


End Namespace



