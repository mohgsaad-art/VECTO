# -*- coding: utf-8 -*-
"""
Created on Tue Jun 29 09:36:30 2021

@author: bitsaev
"""

import pandas as pd
import matplotlib.pyplot as plt
import os
cwd = os.getcwd()


#one test is
#time_run_path = r'..\..\Generic Vehicles\Engineering Mode\GenericVehicle_Group2_P2_EM\Class2_RigidTruck_ParHyb_ENG_01Cycle_time_no_grad_exact_160s.vmod'
#distance_run_path = r'..\..\Generic Vehicles\Engineering Mode\GenericVehicle_Group2_P2_EM\Class2_RigidTruck_ParHyb_ENG_LongHaulNoGrad.vmod'

#time_run_path = r'..\..\Generic Vehicles\Engineering Mode\GenericVehicle_Group2_P2_EM\Class2_RigidTruck_ParHyb_ENG_UrbanDelivery.vmod'
#time_run_path = r'..\..\Generic Vehicles\Engineering Mode\GenericVehicle_Group2_P2_EM\Class2_RigidTruck_ParHyb_ENG_Test_cycle_time.vmod'
#time_run_path = r'..\..\Generic Vehicles\Engineering Mode\GenericVehicle_Group2_P2_EM\Class2_RigidTruck_ParHyb_ENG_RegionalDelivery.vmod'
#time_run_path = r'..\..\Generic Vehicles\Engineering Mode\GenericVehicle_Group2_P2_EM\Class2_RigidTruck_ParHyb_ENG_LongHaulNoGrad.vmod'
#time_run_path = r'..\..\Generic Vehicles\Engineering Mode\GenericVehicle_Group2_P2_EM\Class2_RigidTruck_ParHyb_ENG_LongHaul.vmod'
#time_run_path = r'..\..\Generic Vehicles\Engineering Mode\GenericVehicle_Group2_P2_EM\Class2_RigidTruck_ParHyb_ENG_Cycle_time_no_grad_exact_gear.vmod'
#time_run_path = r'..\..\Generic Vehicles\Engineering Mode\GenericVehicle_Group2_P2_EM\Class2_RigidTruck_ParHyb_ENG_Cycle_time_no_grad.vmod'
#time_run_path = r'..\..\Generic Vehicles\Engineering Mode\GenericVehicle_Group2_P2_EM\Class2_RigidTruck_ParHyb_ENG_Cycle_time.vmod'
#time_run_path = r'..\..\Generic Vehicles\Engineering Mode\GenericVehicle_Group2_P2_EM\Class2_RigidTruck_ParHyb_ENG_03Cycle_time_no_grad_exact_gear_small_org.vmod'
#time_run_path = r'..\..\Generic Vehicles\Engineering Mode\GenericVehicle_Group2_P2_EM\Class2_RigidTruck_ParHyb_ENG_02Cycle_time_no_grad_no_gear_small_org.vmod'
#time_run_path = r'..\..\Generic Vehicles\Engineering Mode\GenericVehicle_Group2_P2_EM\Class2_RigidTruck_ParHyb_ENG_01Cycle_time_no_grad_exact_gear_small_org.vmod'

time_run_path = r'..\..\Generic Vehicles\Engineering Mode\GenericVehicle_Group2_P2_EM\Class2_RigidTruck_ParHyb_ENG_Cycle_time_no_grad_exact_gear.vmod'
distance_run_path = r'..\..\Generic Vehicles\Engineering Mode\GenericVehicle_Group2_P2_EM\Class2_RigidTruck_ParHyb_ENG_LongHaulNoGrad.vmod'


df_time = pd.read_csv(time_run_path, sep=',' , encoding='latin-1',skiprows=1)
sd_time = dict(df_time)

df_distance_raw = pd.read_csv(distance_run_path, sep=',' , encoding='latin-1',skiprows=1)
df_distance = df_distance_raw.iloc[0:len(df_time)]
df_distance.reset_index(inplace=True)
sd_distance = dict(df_distance)
 
fig, ((ax1, ax2), (ax3, ax4)) = plt.subplots(2, 2)
#fig.suptitle('Sharing x per column, y per row')
ax1.plot(sd_distance['time [s]'], sd_distance['v_act [km/h]'],sd_time['time [s]'], sd_time['v_act [km/h]'])
#ax1.plot(sd_distance['time [s]'], sd_distance['v_act [km/h]'],sd_time['time [s]'], sd_time['v_act [km/h]'])
#ax2.plot(sd_distance['time [s]'], sd_distance['Gear [-]'],sd_time['time [s]'], sd_time['Gear [-]'])
ax2.plot(sd_distance['time [s]'], sd_distance['P_trac [kW]'],sd_time['time [s]'], sd_time['P_trac [kW]'])
ax3.plot(sd_distance['time [s]'], sd_distance['P_ice_fcmap [kW]'],sd_time['time [s]'], sd_time['P_ice_fcmap [kW]'])
ax4.plot(sd_distance['time [s]'], sd_distance['P_P2_mech [kW]'],sd_time['time [s]'], sd_time['P_P2_mech [kW]'])
plt.setp(ax1,ylabel='v_act [km/h]')
#plt.setp(ax2,ylabel='Gear [-]')
plt.setp(ax2,ylabel='P_trac [kW]')
plt.setp(ax3,ylabel='P_ice_fcmap [kW]')
plt.setp(ax4,ylabel='P_P2_mech [kW]')
plt.show()
#fig1 = plt.plot(sd_distance['time [s]'], sd_distance['P_trac [kW]'],sd_time['time [s]'], sd_time['P_trac [kW]'])