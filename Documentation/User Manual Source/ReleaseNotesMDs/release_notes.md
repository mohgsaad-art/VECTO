
## VECTO v4.3.4-DEV (10-03-2025)

This development release addresses the issue discussed on previous workshops concerning the definition of Thermal Derating for electric motors and IEPC. The modifications implemented are the following: 

1. A new definition is proposed for torque limits in derating mode and transition to derating mode (through merge request !326, which describes the new equation used). 
2. A specific correction is added for IEPC recently declared overload error due to downshift with high inertia effect, allowing for those cases to run without any issue (Merge request !324). 
    
The new derating mode definition can generate changes in VECTO results for any vehicle equipped with an electric motor or IEPC, therefore stakeholders are invited to test this development release and provide feedback before introducing the modifications in a future VECTO official release.
    
Issues tentatively addressed by this version (the list could change according to discussions that will follow): #761, #767, #826, #830, #850, #867, #877, #878, #886, #888, #889, #893, #939 (and clone #940, #959), #941, #942, #943, #947, #955, #956, #957, #958, #960, #962, #963, #969, #970, #971, #974 (and clone #975), #982, #983. 

### Bug Fixes

- EM-IEPC Thermal Derating - Tq_max and Buffer Mods (vecto/vecto!326)

- Add Driving Actions for IEPC gearshift (vecto/vecto!324)

