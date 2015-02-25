Namespace Hvac


Public Interface ITechListBenefitLine

 Property  Units  As string
 Property  Category As String
 Property  BenefitName As String
 Property  LowFloor As BusFloorLow
 Property  SemiLowFloor As BusFloorSemiLow
 Property  RaisedFloor  As BusFloorRaised

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


End Interface


End Namespace



