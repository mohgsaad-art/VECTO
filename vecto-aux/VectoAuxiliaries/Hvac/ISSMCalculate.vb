Namespace Hvac

   Public Interface ISSMCalculate

       Property Run1 As ISSMRun
       Property Run2 As ISSMRun



       'BaseValues
       '- Heating
       ReadOnly Property BaseHeatingW_Mechanical                   As Double 
       ReadOnly Property BaseHeatingW_ElectricalCoolingHeating     As Double 
       ReadOnly Property BaseHeatingW_ElectricalVentilation        As Double 
       ReadOnly Property BaseHeatingW_FuelFiredHeating             As Double
                                                                   
       'Cooling                                                    
       ReadOnly Property BaseCoolingW_Mechanical                   As Double 
       ReadOnly Property BaseCoolingW_ElectricalCoolingHeating     As Double 
       ReadOnly Property BaseCoolingW_ElectricalVentilation        As Double 
       ReadOnly Property BaseCoolingW_FuelFiredHeating            As Double
       
       'Cooling
       ReadOnly Property BaseVentilationW_Mechanical               As Double 
       ReadOnly Property BaseVentilationW_ElectricalCoolingHeating As Double 
       ReadOnly Property BaseVentilationW_ElectricalVentilation    As Double 
       ReadOnly Property BaseVentilationW_FuelFiredHeating        As Double
  
       
       'TechListBenefits
       '- Heating
       ReadOnly Property TechListAdjustedHeatingW_Mechanical                   As Double 
       ReadOnly Property TechListAdjustedHeatingW_ElectricalCoolingHeating     As Double 
       ReadOnly Property TechListAdjustedHeatingW_ElectricalVentilation        As Double 
       ReadOnly Property TechListAdjustedHeatingW_FuelFiredHeating            As Double
                                                                               
       'Cooling          TechListAdjusted                                      
       ReadOnly Property TechListAdjustedCoolingW_Mechanical                   As Double 
       ReadOnly Property TechListAdjustedCoolingW_ElectricalCoolingHeating     As Double 
       ReadOnly Property TechListAdjustedCoolingW_ElectricalVentilation        As Double 
       ReadOnly Property TechListAdjustedCoolingW_FuelFiredHeating            As Double
                        
       'Cooling          TechListAdjusted
       ReadOnly Property TechListAdjustedVentilationW_Mechanical               As Double 
       ReadOnly Property TechListAdjustedVentilationW_ElectricalCoolingHeating As Double 
       ReadOnly Property TechListAdjustedVentilationW_ElectricalVentilation    As Double 
       ReadOnly Property TechListAdjustedVentilationW_FuelFiredHeating        As Double
   
   
   End interface

   

End Namespace



