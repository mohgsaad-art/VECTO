
Namespace Hvac

Public Interface ISSMTechList

 property TechLines As List(Of ITechListBenefitLine) 

 Sub Clear()
 Function    Add( item As ITechListBenefitLine, byref feedback as String) As Boolean
 Function Delete( item As ITechListBenefitLine, byref feedback as String) As Boolean
 Function Modify( item As ITechListBenefitLine, byref feedback as String) As Boolean

 ReadOnly Property HValueVariation  As Double
 ReadOnly Property VHValueVariation As Double
 ReadOnly Property VVValueVariation As Double
 ReadOnly Property VCValueVariation As Double
 ReadOnly Property CValueVariation  As Double

 ReadOnly Property HValueVariationKW  As Double
 ReadOnly Property VHValueVariationKW As Double
 ReadOnly Property VVValueVariationKW As Double
 ReadOnly Property VCValueVariationKW As Double
 ReadOnly Property CValueVariationKW  As Double


 Function Initialise() As Boolean




End Interface



End Namespace


