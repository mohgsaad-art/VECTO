Imports VectoAuxiliaries.Hvac

Public Interface ISSMTOOL


 Property GenInputs As ISSMGenInputs
 Property TechList As ISSMTechList
 Property Calculate As ISSMCalculate



ReadOnly Property ElectricalWBase As Single
ReadOnly Property MechanicalWBase As Single
ReadOnly Property FuelPerHBase As Single

ReadOnly Property ElectricalWAdjusted As Single
ReadOnly Property MechanicalWBaseAdjusted As Single
ReadOnly Property FuelPerHBaseAdjusted As Single


Sub Clone(from As ISSMTOOL)


Function Load(filePath As String) As Boolean

Function Save(filePath As String) As Boolean

Function IsEqualTo(source As ISSMTOOL) As Boolean

''' <summary>
''' This alters the waste heat and returns an adjusted fueling value
''' </summary>
''' <param name="AverageUseableEngineWasteHeatKW"></param>
''' <returns></returns>
''' <remarks></remarks>
Function FuelPerHBaseAsjusted( AverageUseableEngineWasteHeatKW As Single ) As Single


Event Message(ByRef sender As Object, message As String, messageType As AdvancedAuxiliaryMessageType)



End Interface
