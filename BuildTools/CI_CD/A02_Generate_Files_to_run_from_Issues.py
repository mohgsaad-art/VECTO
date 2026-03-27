# -*- coding: utf-8 -*-
"""
Created on Thu Jan 25 16:14:36 2024

@author: alixgui
"""

import os
import sys
from lxml import etree,objectify
import shutil
import json
from datetime import datetime
import re
import copy

def create_dir_if_not_exist(dir_path):

        pathi = ''
        for pathstep in dir_path.split(os.sep):
            pathi = os.path.join(pathi,pathstep)
            if not os.path.exists(pathi):
                os.mkdir(pathi)

root_path = sys.argv[1] if len(sys.argv) > 1 else ''

dir_ALL_ISSUES = os.path.join(root_path, '11_INPUTS_ISSUE') if root_path else '11_INPUTS_ISSUE'
dir_ALL_ISSUES_sorted_renamed = os.path.join(root_path, '11_INPUTS_ISSUE_SORTED') if root_path else '11_INPUTS_ISSUE_SORTED'
dir_ALL_ISSUES_TORUN = os.path.join(root_path, '12_TO_RUN_ISSUES', 'ISSUE') if root_path else os.path.join('12_TO_RUN_ISSUES', 'ISSUE')

for dir2remove in [dir_ALL_ISSUES_sorted_renamed, dir_ALL_ISSUES_TORUN]:

    if os.path.isdir(dir2remove):

        shutil.rmtree(dir2remove)


# =========================================================================== #
# Rename files with standard names ISSUE_VIN[_COMP/PRIM]
# =========================================================================== #

print('1- Sort files and standardize file names')


if not os.path.exists(dir_ALL_ISSUES_sorted_renamed):
    os.mkdir(dir_ALL_ISSUES_sorted_renamed)

xml_file_list = [os.path.join(root,file)
                  for root,dummy,files in os.walk(dir_ALL_ISSUES)
                  for file in files
                  if file.endswith('.xml') and ('RSLT' not in file)  and ('VIF' not in file)]

for file_ref in xml_file_list:

    Issue = int(os.path.relpath(file_ref, dir_ALL_ISSUES).split(os.sep)[0][0:4])

    xml_tree = etree.parse(file_ref)

    VIN = xml_tree.find('.//{*}VIN')
    VIN_fld = VIN.text
    if VIN_fld=='':
        VIN_fld='NOVIN'

    Vehicle = xml_tree.find('.//{*}Vehicle')
    key_typeveh = [key for key in Vehicle.attrib.keys() if 'type' in key][0]

    added_label='_'
    if 'Primary' in Vehicle.attrib[key_typeveh]:
        added_label = added_label + 'PRIM'
    if 'Complete' in Vehicle.attrib[key_typeveh]:
        added_label = added_label + 'COMP'
    if added_label=='_':
        added_label=''


    ofilename = '{:04.0f}'.format(Issue) + '_' + VIN_fld + added_label + '.xml'
    ofilepath = os.path.join(dir_ALL_ISSUES_sorted_renamed,ofilename)

    shutil.copy(file_ref,ofilepath)

# =========================================================================== #
# Rename Duplicate
# =========================================================================== #

print('2- Detect and Rename Duplicates')

def check_xml_duplicate(file1, file2):


    xml_tree_1 = etree.parse(file1)
    xml_tree_2 = etree.parse(file2)



    # get type of xml
    # ---------------
    #Components1 = xml_tree_1.find('.//{*}Components')
    #Components2 = xml_tree_2.find('.//{*}Components')
    # key_type = [key_name for key_name in Components1.attrib.keys() if 'type' in key_name][0]
    # type1 = Components1.attrib[key_type]
    # type2 = Components2.attrib[key_type]

    Vehicle1 = xml_tree_1.find('.//{*}Vehicle')
    Vehicle2 = xml_tree_2.find('.//{*}Vehicle')
    key_type = [key_name for key_name in Vehicle1.attrib.keys() if 'type' in key_name][0]
    type1 = Vehicle1.attrib[key_type]
    type2 = Vehicle2.attrib[key_type]

    # get VINs
    # ---------------

    VIN1 = xml_tree_1.find('.//{*}VIN')
    VIN1_value = VIN1.text

    VIN2 = xml_tree_2.find('.//{*}VIN')
    VIN2_value = VIN2.text

    # fast check on xml type
    # -----------------------------------------------------------------
    is_duplicate = (type1==type2)
    if not is_duplicate:
        return is_duplicate,VIN1_value,VIN2_value

    # slow check on the entire files removing VINs, Vehicle id, Date and Digest
    # -----------------------------------------------------------------
    VIN1.getparent().remove(VIN1)
    # special Mercedes, who also change the date and vehicle id to fit the VIN
    # Vehicle1 = xml_tree_1.find('.//{*}Vehicle')
    Vehicle1.attrib['id']=''
    Date1 = xml_tree_1.find('.//{*}Date')
    Date1.getparent().remove(Date1)

    Digests1 =  xml_tree_1.findall('.//{*}DigestValue')
    for Digest in Digests1:
        Digest.text=''


    VIN2.getparent().remove(VIN2)
    # special Mercedes, who also change the date and vehicle id to fit the VIN
    # Vehicle2 = xml_tree_2.find('.//{*}Vehicle')
    Vehicle2.attrib['id']=''
    Date2 = xml_tree_2.find('.//{*}Date')
    Date2.getparent().remove(Date2)

    Digests2 =  xml_tree_2.findall('.//{*}DigestValue')
    for Digest in Digests2:
        Digest.text=''

    etree.indent(xml_tree_1,space='\t',level=0)
    etree.indent(xml_tree_2,space='\t',level=0)

    # to compare with annotation
    # x1 = etree.tostring(xml_tree_1,method='xml')
    # x2 = etree.tostring(xml_tree_2,method='xml')
    # is_duplicate = (x1==x2)

    # remove annotation before compare
    xml_tree_11 = copy.deepcopy(xml_tree_1)
    xml_tree_22 = copy.deepcopy(xml_tree_2)
    # deannotate to compare
    objectify.deannotate(xml_tree_11, cleanup_namespaces=True)
    objectify.deannotate(xml_tree_22, cleanup_namespaces=True)
    x11 = etree.tostring(xml_tree_11,method='xml')
    x22 = etree.tostring(xml_tree_22,method='xml')
    is_duplicate = (x11==x22)

    return is_duplicate,VIN1_value,VIN2_value


# ref list

xml_file_list = [os.path.join(root,file)
                  for root,dummy,files in os.walk(dir_ALL_ISSUES_sorted_renamed)
                  for file in files
                  if file.endswith('.xml')]

# check duplicates
files_duplicated={} # keys are for file duplicated, values are for ref duplicate file
duplicates_number = {}
list_file_unique = []

list_to_test = [f for f in xml_file_list]
for file_ref in xml_file_list:

    if file_ref in files_duplicated.keys():
        continue

    duplicates_number[file_ref]=0

    list_to_test = [f for f in list_to_test if f is not file_ref]

    for file_test in list_to_test:

        is_duplicate,VINref_value,VINtest_value = check_xml_duplicate(file_ref, file_test)

        if is_duplicate:

            files_duplicated[file_test]=file_ref
            duplicates_number[file_ref]+=1

    if duplicates_number[file_ref]==0:
        list_file_unique.append(file_ref)

    list_to_test = [f for f in list_to_test if f not in files_duplicated.keys()]

# getting families of duplicates
files_with_duplicated = {os.path.basename(file_ref).replace('.xml',''):[] for  file_dup,file_ref in files_duplicated.items()}

for file_dup,file_ref in files_duplicated.items():

    file_ref = os.path.basename(file_ref).replace('.xml','')
    file_dup = os.path.basename(file_dup).replace('.xml','')

    files_with_duplicated[file_ref].append(file_dup)

# rename duplicates

for filedup_fpath,fileref_fpath in files_duplicated.items():

    fileref = os.path.basename(fileref_fpath)

    filedup_fpath_new = filedup_fpath.replace('.xml', '__IDEM__' + fileref)

    os.rename(filedup_fpath, filedup_fpath_new)

# =========================================================================== #
# get files not duplicated and prepare directory to run
# =========================================================================== #

print('3- Generate Run directory')


if not os.path.exists(dir_ALL_ISSUES_TORUN):
    create_dir_if_not_exist(dir_ALL_ISSUES_TORUN)

xml_file_list_TORUN = [os.path.join(root,file)
                  for root,dummy,files in os.walk(dir_ALL_ISSUES_sorted_renamed)
                  for file in files
                  if file.endswith('.xml')]

for file in xml_file_list_TORUN:

    if 'IDEM' in file:
        continue

    filename = os.path.basename(file)

    run_subdir = filename
    run_subdir = run_subdir.replace('.xml','')
    run_subdir = run_subdir.replace('_COMP','')
    run_subdir = run_subdir.replace('_PRIM','')

    run_path = os.path.join(dir_ALL_ISSUES_TORUN, run_subdir)
    if not os.path.exists(run_path):
        os.mkdir(run_path)

    run_file_path = os.path.join(run_path, filename)

    shutil.copy(file,run_file_path)

# =========================================================================== #
# correction: some files have duplicated primary but different completed
# =========================================================================== #

dir_list = [os.path.join(dir_ALL_ISSUES_TORUN,d) for d in os.listdir(dir_ALL_ISSUES_TORUN) if os.path.isdir(os.path.join(dir_ALL_ISSUES_TORUN,d))]

for dirpath in dir_list:

    list_xml = [f for f in os.listdir(dirpath) if f.endswith('.xml')]

    comp_file_list = [f for f in list_xml if 'COMP' in f]
    prim_file_list = [f for f in list_xml if 'PRIM' in f]

    if len(comp_file_list)>0 and len(prim_file_list)==0:
        # locate if a PRIM file exist

        comp_file = comp_file_list[0]
        prim_file_pattern = comp_file.replace('COMP','PRIM').replace('.xml','')
        xml_primfile_corresponding = [os.path.join(root,file)
                          for root,dummy,files in os.walk(dir_ALL_ISSUES_sorted_renamed)
                          for file in files
                          if (file.endswith('.xml') and prim_file_pattern in file)]
        if len(xml_primfile_corresponding)>0:

            xml_primfile = os.path.basename(xml_primfile_corresponding[0])
            xml_primfile_changedname = re.sub('__IDEM__.*', '.xml', xml_primfile, count=0, flags=0)
            shutil.copy(xml_primfile_corresponding[0], os.path.join(dirpath,xml_primfile_changedname))

        else:
            print('no primary file for directory :', dirpath)

# leave tracking of file not run in the directory to run
with open(os.path.join(dir_ALL_ISSUES_TORUN,'duplicates_not_run.json'), 'w') as fp:
    json.dump(files_with_duplicated, fp, indent=4)

# =========================================================================== #
# generate vecto files for buses (PRIM + COMP)
# =========================================================================== #

dir_list = [os.path.join(dir_ALL_ISSUES_TORUN,d) for d in os.listdir(dir_ALL_ISSUES_TORUN) if os.path.isdir(os.path.join(dir_ALL_ISSUES_TORUN,d))]

for dirpath in dir_list:

    # if dirpath == '12_TO_RUN_ISSUES\\302_NLRTMF100RA028935':
    #     print('')

    list_xml = [f for f in os.listdir(dirpath) if f.endswith('.xml')]

    comp_file_list = [f for f in list_xml if 'COMP' in f]
    prim_file_list = [f for f in list_xml if 'PRIM' in f]

    if len(comp_file_list)>0 and len(prim_file_list)>0:

        # generate VECTO file
        # FM from prim VIF and COMP
        # --------------------------
        # job_dict = {
        #       "Header": {
        #     	"CreatedBy": "Guillaume Alix, JRC, Ispra",
        #     	"Date": datetime.now().isoformat()+'Z',
        #     	"AppVersion": "2.2",
        #     	"FileVersion": 7
        #       },
        #       "Body": {
        #     	"PrimaryVehicleResults":  prim_file_list[0],
        #     	"CompletedVehicle": comp_file_list[0]
        #       }
        #     }

        # FM from prim and comp
        # --------------------------
        job_dict = {
           "Header": {
             "CreatedBy": "Guillaume Alix, JRC, Ispra",
             "Date": datetime.now().isoformat()+'Z',
             "AppVersion": "2.2",
             "FileVersion": 10
           },
           "Body": {
             "PrimaryVehicle": prim_file_list[0],
             "InterimStep": comp_file_list[0],
             "Completed": 'true',
             "RunSimulation": 'true'
           }
         }


        jobfilename = prim_file_list[0].replace('_PRIM','')
        jobfilename = jobfilename.replace('.xml','.vecto') # no need for os.name as it is only filename and not complicated path

        fjob_fpath = os.path.join(dirpath, jobfilename)

        with open(fjob_fpath, "w") as file:
            json.dump(job_dict, file, indent=2)

# rename to_RUN directories to identify HEV

for dirpath in dir_list:

    list_xml = [file for file in os.listdir(dirpath) if (file.endswith('.xml') and 'RSLT' not in file and 'VIF' not in file)]

    if len(list_xml)>0:
        prim_file_list = [f for f in list_xml if 'PRIM' in f]
        if len(prim_file_list)>0:
            fxml=os.path.join(dirpath,prim_file_list[0])
        else:
            fxml=os.path.join(dirpath,list_xml[0])
    else:
        continue

    xml_tree =  etree.parse(fxml)
    vehicle = xml_tree.find('.//{*}Vehicle')

    dir_powertrain=''
    if len(vehicle.attrib)>0:
        typology = vehicle.attrib[[fld for fld in vehicle.attrib if 'type' in fld][0]]
        # print(typology)
        if 'PEV' in typology:
            dir_powertrain='PEV'
        elif 'HEV' in typology:
            dir_powertrain='HEV'
        elif 'IEPC' in typology: # note: HEV_IEPC are considered HEV this way
            dir_powertrain='IEPC'
        else:
            dir_powertrain='ICE'

    if dir_powertrain!='ICE' and dir_powertrain!='':

        shutil.move(dirpath, dirpath+'_'+dir_powertrain)


