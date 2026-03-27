# -*- coding: utf-8 -*-
"""
Created on Mon Jul  8 08:49:41 2024

@author: alixgui
"""

import os
import sys
from FUNCTIONS.VECTO import load_vecto, get_inputs_path, write_vecto,replace_string_in_filepath
from lxml import etree
import json
from datetime import datetime
import shutil

def create_dir_if_not_exist(dir_path):
    
        pathi = ''
        for pathstep in dir_path.split(os.sep):
            pathi = os.path.join(pathi,pathstep)
            if not os.path.exists(pathi):
                os.mkdir(pathi)
                
def get_dict_val_from_keypath(dico,p):
    
    dic_var=dico
    for key in p:
        dic_var = dic_var[key]
        
    return dic_var

def strip_common_str(lstring):
    
    common_start=''
    common_end=''
    
    # lstringout = lstring
    
    for idx in range(1,len(lstring[0])+1):
        common_start_test = lstring[0][0:idx]
        isOK = all([(common_start_test in string) for string in lstring])
        
        if not isOK:
            break
        
    idx = idx-1 
    common_start = lstring[0][0:idx]
    
    for idx in range(1,len(lstring[0])+1):
        common_end_test = lstring[0][-idx:]
        isOK = all([(common_end_test in string) for string in lstring])
        
        if not isOK:
            break
        
    idx = idx-1 
    common_end = lstring[0][-idx:]
    if common_end.find('.'):
        common_end = common_end[common_end.find('.'):]
    
    lstringout = [string.replace(common_start,'').replace(common_end,'') for string in lstring]
    
    return lstringout
    
def remove_str_start_length(s, start, length):
    return s[:start] + s[start + length:]

# test = ['abc.vecto','abcde.vecto','abcdef.vecto']
# testout = strip_common_str(test)

# file_vecto_fpath = os.path.join('INPUTS','Group 54','ML3van.vecto')

# inputs = load_vecto(file_vecto_fpath)    

# lpaths = get_inputs_path(inputs)

# lpath_file = [p for p in lpaths if 'file' in p]

# create_dir_if_not_exist('OUTPUTS')


# vfile = 'INPUTS2\\Generic Vehicles\\Engineering Mode\\Group5_Tractor_4x2\\Class5_Tractor_ENG.vecto'
# inputs = load_vecto(vfile) 

# loading vecto files and associated
# ----------------------------------

root_path = sys.argv[1] if len(sys.argv) > 1 else ''

Input_DIR = os.path.join(root_path, '01_INPUTS_GENERIC') if root_path else '01_INPUTS_GENERIC'
Output_DIR = os.path.join(root_path, '02_TO_RUN_GENERIC') if root_path else '02_TO_RUN_GENERIC'

subdir_mode_DECL = 'GEN__DECL_mode'
subdir_mode_ENG = 'GEN__ENG_mode'

file_list_VECTO = [os.path.join(root,file) 
              for root,dummy,files in os.walk(Input_DIR) 
              for file in files 
              if file.endswith('.vecto')]

content_from_file2remove = ['FCHV', '_invalid','P39_40']

file_list_VECTO = [f for f in file_list_VECTO if all([not (c in f) for c in content_from_file2remove])]

for vfile in file_list_VECTO:
    
    vdir_in = os.path.dirname(vfile)
    # print(vfile)
    
    vdir_out = vdir_in.replace(Input_DIR,Output_DIR).replace(' ','_')
    
    # get extension type
    
    inputs = load_vecto(vfile)    
    lpaths = get_inputs_path(inputs)
    lpaths_file = [p for p in lpaths if 'file' in p]
    
    # print(vdir_in)
    ext = '' # no extension means a vecto file, precision added if using xmls
    # ext = '_vecto'
    # prefix = 'VECTO_'
    
    # test if Vecto file is pointing on xml files
    
    isVecto2xml=False
    for fp in lpaths_file:
        fn = get_dict_val_from_keypath(inputs,fp)
        # print('\t',fn)
        if fn.endswith('.xml'):
            ext = '_vecto2xml'
            isVecto2xml=True
    
    # print(ext)
    
    
    
    # check if multiple vecto files in directory: in that case, change name
    ltest = [f for f in file_list_VECTO if vdir_in==os.path.dirname(f)]
    ext2=''
    if len(ltest)>1:
        # print(ltest)
        lext = strip_common_str(ltest)
        # print(lext)
        ext2= lext[[i for i,x in enumerate(ltest) if x==vfile][0]]
        # print(ext2)
        vdir_out = vdir_out + ext2 + ext
        # print(vdir_out)
        # break
    else:
        vdir_out = vdir_out + ext2 + ext
        # print(vdir_out)
    
    vdir_out_elt = os.path.normpath(vdir_out).split(os.sep)
    
    
    try:
        isdecl = inputs['vecto']['data']['Body']['SavedInDeclMode']
    except:
        isdecl = None
    
    # check if file is decl or engineering mode
    if isdecl is None:
        dir_mode = 'UNKNOWN'
    else:
        if isdecl:
            dir_mode = 'GEN__DECL_mode'
        else:
            dir_mode = 'GEN__ENG_mode'  
    if isVecto2xml:
        dir_mode = 'GEN__DECL_mode'
        
    # check if vehicle is PEV, HEV or ICE
    
    # 1- Based on Path
    # if 'pev' in vdir_in.lower():
    #     dir_powertrain = 'PEV'
    # elif 'hev' in vdir_in.lower():
    #     dir_powertrain = 'HEV'
    # else:
    #     dir_powertrain = 'UNKNOWN'
    
    # 2- Based on File content
    
    # list of all the fields pointing on a file: 
    
    if not isVecto2xml:
        
        flds_file = [p[-2] for p in lpaths if 'file' in p]
        isICE = ('EngineFile' in flds_file)
        isEM = ('MotorFile' in flds_file) or ("IEPC" in flds_file) or ('MotorFile_1' in flds_file) or ("IEPC_1" in flds_file)
        
        if isICE and isEM:
            dir_powertrain = 'HEV'
        elif isICE and not isEM:
            dir_powertrain = 'ICE'
        elif not isICE and isEM:
            dir_powertrain = 'PEV'
    else:
        for fp in lpaths_file:
            fn = get_dict_val_from_keypath(inputs,fp)
            # print('\t',fn)
            if fn.endswith('.xml'):
                dp = fp[:-1]+['data']
                data = get_dict_val_from_keypath(inputs,dp) # loading a lxml tree object
                
                vehicle = data.find('.//{*}Vehicle')
                
                if len(vehicle.attrib)>0:
                    typology = vehicle.attrib[[fld for fld in vehicle.attrib if 'type' in fld][0]]
                    if 'PEV' in typology:
                        dir_powertrain='PEV'
                    elif 'HEV' in typology:
                        dir_powertrain='HEV'
                    else:
                        dir_powertrain='ICE'
                
    vdir_out_elt_simplified = [vdir_out_elt[0] , dir_mode, dir_powertrain,  vdir_out_elt[-1]]
    # vdir_out_elt_simplified = [vdir_out_elt[0] , dir_mode, dir_powertrain+'_'+ vdir_out_elt[-1]]
    
    # removing the GenericVehicle field, redundant
    vdir_out_elt_simplified[-1]=vdir_out_elt_simplified[-1].replace('GenericVehicle_','')
    vdir_out_elt_simplified[-1]=vdir_out_elt_simplified[-1].replace('GenericVehicle','')
    
    vdir_out_simplified = os.path.join(*vdir_out_elt_simplified)
    # vdir_out_simplified = os.path.join(vdir_out_elt_simplified[0],'__'.join(vdir_out_elt_simplified[1:])) # to get 
    
    # print(vdir_out_simplified)
    
    # loading vecto file and subfiles
    # --------------------------------
    
    input_i = load_vecto(vfile)   
    
    replace_string_in_filepath(input_i,'..','.')
    
    # write_vecto(input_i,vdir_out)
    write_vecto(input_i,vdir_out_simplified)
    
        
# vecto2xml files are either VTP or Buses Prim+Comp, run either in FM mode or 
# Single Run Mode --> if SR, generate the same directory in FM mode
# -----------------------------------------------------------------------------

file_list_VECTO2XML = [os.path.join(root,file) 
              for root,dummy,files in os.walk(Output_DIR) 
              for file in files 
              if file.endswith('.vecto') and '_vecto2xml' in root and '_FM_vecto2xml' not in root]

for fpath in file_list_VECTO2XML:
    
    data={}
    try:
        with open(fpath, 'r',encoding='utf-8-sig') as file:
            data = json.load(file)
    except: 
        with open(fpath, 'r',encoding='utf-8') as file:
            data = json.load(file)
    
    if data['Header']['FileVersion']!=6: 
        continue
        
    dir_src = os.path.dirname(fpath)
    dir_dest = os.path.dirname(fpath).replace('_vecto2xml','_FM_vecto2xml')
    
    fpath_dest = fpath.replace(dir_src,dir_dest)
    
    files2copy = os.listdir(dir_src)
    path2copy = [os.path.join(dir_src,f) for f in files2copy]
    
    job_dict = {
       "Header": {
         "CreatedBy": "Guillaume Alix, JRC, Ispra",
         "Date": datetime.now().isoformat()+'Z',
         "AppVersion": "Vecto3GUI2020",
         "FileVersion": 10
       },
       "Body": {
         "PrimaryVehicle": data['Body']['PrimaryVehicle'],
         "InterimStep": data['Body']['CompletedVehicle'],
         "Completed": 'true',
         "RunSimulation": 'true'
       }
     }
    
    if not os.path.exists(dir_dest):
        os.mkdir(dir_dest)
    
    with open(fpath_dest, "w") as file:
        json.dump(job_dict, file, indent=2)
    
    for filepath in path2copy:
        if filepath==fpath:
            continue
        shutil.copy(filepath,dir_dest)

# adding xml files that are alone in their directory
# --------------------------------------------------
file_list_XML = [os.path.join(root,file) 
              for root,dummy,files in os.walk(Input_DIR) 
              for file in files 
              if file.endswith('.xml')]

dir_list_XML = [os.path.dirname(f) for f in file_list_XML]

file_list_XMLalone = [f for f in file_list_XML if dir_list_XML.count(os.path.dirname(f))==1]

file_list_XMLalone = [f for f in file_list_XMLalone if all([not (c in f) for c in content_from_file2remove])]

ext='_xml'


for fxml in file_list_XMLalone:
    vdir_in = os.path.dirname(fxml)
    vdir_out = vdir_in.replace(Input_DIR,Output_DIR).replace(' ','_')
    # remove ext if already here
    pos_ext = vdir_out.lower().rfind(ext.lower())
    if pos_ext>-1:
        vdir_out=remove_str_start_length(vdir_out, pos_ext, len(ext))
    
    
    vdir_out = vdir_out + ext
    vdir_out_elt = os.path.normpath(vdir_out).split(os.sep)
    
    # xml file are in decl mode
    
    dir_mode = subdir_mode_DECL
    
    # powertrain recognized throuh xlm file
    
    xml_tree = etree.parse(fxml)
    vehicle = xml_tree.find('.//{*}Vehicle')
    
    # is_primary = False
    if len(vehicle.attrib)>0:
        typology = vehicle.attrib[[fld for fld in vehicle.attrib if 'type' in fld][0]]
        # print(typology)
        if 'PEV' in typology:
            dir_powertrain='PEV'
        elif 'HEV' in typology:
            dir_powertrain='HEV'
        else:
            dir_powertrain='ICE'
    
    vdir_out_elt_simplified = [vdir_out_elt[0] , dir_mode, dir_powertrain,  vdir_out_elt[-1]]
    vdir_out_simplified = os.path.join(*vdir_out_elt_simplified)
    
    # print(vdir_out_simplified)
    create_dir_if_not_exist(vdir_out_simplified)
    outpath = os.path.join(vdir_out_simplified,os.path.basename(fxml).replace(' ','_'))
    # print(outpath)
    
    xml_tree.write(outpath, encoding='utf-8', xml_declaration=True, pretty_print=False)