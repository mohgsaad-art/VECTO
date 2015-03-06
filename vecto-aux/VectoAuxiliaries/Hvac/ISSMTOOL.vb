Imports VectoAuxiliaries.Hvac

Public Interface ISSMTOOL


 Property GenInputs As ISSMGenInputs
 Property TechList As ISSMTechList
 Property Calculate As ISSMCalculate



ReadOnly Property ElectricalWBase As Single
ReadOnly Property MechanicalWBase As Single
ReadOnly Property FuelLPerHBase As Single

ReadOnly Property ElectricalWAdjusted As Single
ReadOnly Property MechanicalWBaseAdjusted As Single
ReadOnly Property FuelLPerHBaseAdjusted As Single


Sub Clone(from As ISSMTOOL)


Function Load(filePath As String) As Boolean

Function Save(filePath As String) As Boolean

Function IsEqualTo(source As ISSMTOOL) As Boolean






End Interface
