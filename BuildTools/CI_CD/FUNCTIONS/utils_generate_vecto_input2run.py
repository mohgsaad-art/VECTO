# -*- coding: utf-8 -*-
"""
Created on Sun Apr 12 18:50:23 2026

@author: alixgui
"""

# %%
import copy
import itertools as itt
import json
import re
from datetime import datetime
from pathlib import Path
import hashlib

import pandas as pd
import numpy as np
from lxml import etree, objectify
from ordered_set import OrderedSet as oset  # type: ignore

from .utils import (
    el_text,
    parse_xsd_type,
    get_issue_id_from_path,
    scan_issue_files,
    read_issue_files
)

# get uploads appearance order from issue description (**TO IMPROVE TO GET DATES**)
# =============================================================================

def get_gitlab_hash(fpath: Path | str) -> str:
    filep = str(fpath)

    regexp = r"[a-z0-9]{16,64}"
    hash_values = re.findall(regexp, filep)

    hash_value=None
    if hash_values:
        hash_value = hash_values[0]

    return hash_value

def read_vecto_file(fpath: Path | str) -> list:

    try:
        xml_tree = etree.parse(fpath)
    except etree.ParseError as ex:
        dicout = {"non_xml_file_errors":repr(ex)}
    else:
        try:
            VIN = el_text(xml_tree, ".//{*}Vehicle/{*}VIN")
            # if not VIN:
            #     raise ValueError(f"No <VIN> element.")

            veh_el = xml_tree.find(".//{*}Vehicle")

            OEM = el_text(veh_el, "{*}Manufacturer", "**/{*}Manufacturer")
            veh_class = el_text(veh_el, "{*}LegislativeClass","{*}LegislativeCategory")
            veh_category = el_text(veh_el, "{*}VehicleCategory","{*}ChassisConfiguration")
            OVC = el_text(veh_el, "{*}OVC", "{*}OvcHev")

            # if veh_el is None:
            #     raise ValueError("No <Vehicle> elememt.")

            ## Eventually we end up with XSD-type like:
            #   DeclarationInput:VectoInputDeclaration/VehicleDeclarationType                                2
            #
            veh_xsd_type: str = veh_el.get(
                "{http://www.w3.org/2001/XMLSchema-instance}type", ""
            )
            if not veh_xsd_type:
                # No `xsi` attribute on Vehicle, fallback to its NameSpace.
                veh_xsd_type = veh_el.xpath("namespace-uri()")
            veh_xsd_type = parse_xsd_type(veh_xsd_type)

            root = xml_tree.getroot()
            typ = root.xpath("namespace-uri()")
            tag = root.xpath("local-name()")
            veh_xsd_type = f"{parse_xsd_type(typ)}:{tag}/{veh_xsd_type}"

            is_xml_vif = bool(
                re.search("VIFType|VehicleInterimFile", veh_xsd_type)
            )
            is_xml_cif = bool(
                re.search("VectoCustomerInformation", veh_xsd_type)
            )

            is_xml_monitor = "Monitoring" in veh_xsd_type
            is_xml_output = (
                # is_xml_vif
                "Output" in veh_xsd_type
                or "output" in str(fpath).lower()
            )
            is_xml_input = (
                # is_xml_vif
                "Input" in veh_xsd_type
                or "input" in str(fpath).lower()
            )

            # is_OVC
            is_OVC = False
            if OVC is not None:
                is_OVC = OVC.lower()=="true"


            # generating data hash by removing useless data

            VIN_el = xml_tree.find(".//{*}VIN")
            VIN_el.getparent().remove(VIN_el)
            # special Mercedes, who also change the date and vehicle id to fit the VIN
            # Vehicle1 = xml_tree_1.find('.//{*}Vehicle')
            veh_el.attrib['id']=''
            Date_el = xml_tree.find('.//{*}Date')
            Date_el.getparent().remove(Date_el)

            Digests =  xml_tree.findall('.//{*}DigestValue')
            for Digest in Digests:
                Digest.text=''

            etree.indent(xml_tree,space='\t',level=0)

            xml_tree_cp = copy.deepcopy(xml_tree)
            objectify.deannotate(xml_tree_cp, cleanup_namespaces=True)
            xml_tree_str = etree.tostring(xml_tree_cp,method='xml', encoding='UTF-8')

            # hash1 = str(int(hashlib.sha1(x11).hexdigest(), 16) % (10 ** 8))
            data_hash = hashlib.sha1(xml_tree_str).hexdigest()

            dicout = {
                    "file": Path(str(fpath)),
                    "VIN": VIN,
                    "OEM": OEM,
                    "vehicle_xsd_type": veh_xsd_type,
                    "veh_class": veh_class,
                    "veh_category": veh_category,
                    "is_xml_input": is_xml_input,
                    "is_xml_output": is_xml_output,
                    "is_xml_vif": is_xml_vif,
                    "is_xml_cif": is_xml_cif,
                    "is_xml_monitor": is_xml_monitor,
                    "is_xml_primary": "Primary" in veh_xsd_type,
                    "is_xml_completed": "Completed" in veh_xsd_type,
                    "is_xml_conventional": "Conventional" in veh_xsd_type,
                    "is_xml_hev": "HEV" in veh_xsd_type,
                    "is_xml_fchv": "FC" in veh_xsd_type,
                    "is_xml_ovc": is_OVC,
                    "is_xml_pev": "PEV" in veh_xsd_type,
                    "is_xml_iepc": "IEPC" in veh_xsd_type,
                    "file_type": ("Primary" in veh_xsd_type)*1 +\
                        ("VIF" in veh_xsd_type)*2 +\
                            ("Complete" in veh_xsd_type)*3 +\
                                ("VIF" not in veh_xsd_type and "Declaration" not in veh_xsd_type)*4,
                    "data_hash": data_hash,
                    "data_hash_short":data_hash[0:5],
                    "non_xml_file_errors": None,
                }

        except Exception as ex:
            dicout = {"non_xml_file_errors":repr(ex)}

    return dicout

def scan_pool(save_dir_RAW: Path | str, save_dir_EXTRACT: Path | str, issue_desc_save_file_name):

    raw_issue_jsons = list(save_dir_RAW.glob(f"*/{issue_desc_save_file_name}*.json"))
    all_scans_sorted = scan_issue_files(raw_issue_jsons)
    all_links_sorted = list(oset(itt.chain(*[i["links"] for i in all_scans_sorted]))) # should be sorted in order of appearance

    all_IssueVINs = [(i["iid"],i_vin) for i in all_scans_sorted for i_vin in i["VINs"]]
    all_IssueVINs_sorted = list(oset(all_IssueVINs))

    # all_IssueCurrentLabel = [(i["iid"],i["label"]) for i in all_scans_sorted]
    # df_IssueLabel = pd.DataFrame(data=all_IssueCurrentLabel,columns=['Issue','Label'])
    # all_IssueLabel = [(i["iid"],i[]["created_at"],i["closed_at"],i["label"],i["label_events"]) for i in all_scans_sorted]
    # df_IssueLabel = pd.DataFrame(data=all_IssueLabel,columns=['Issue',"created_at","closed_at",'Label','LabelEvents'])
    # df_IssueLabel_sorted = df_IssueLabel.groupby('Issue').tail(1)

    df_IssueVINs_declared = pd.DataFrame(data=all_IssueVINs_sorted,columns=['Issue','VIN'])
    df_IssueVINs_declared['IssueVIN']=df_IssueVINs_declared['Issue'].apply(lambda x: f"{x:04d}") + '_' + df_IssueVINs_declared['VIN']

    all_path_sorted = [Path(i) for i in all_links_sorted]
    all_hash_sorted = [get_gitlab_hash(path_i) for path_i in all_path_sorted]
    df_hash_sorted = pd.DataFrame(data = { 'gitlab_upload_hash' : all_hash_sorted})

    return df_IssueVINs_declared, df_hash_sorted

def read_pool_stats(save_dir_RAW: Path | str, issue_desc_save_file_name):
    raw_issue_jsons = list(save_dir_RAW.glob(f"*/{issue_desc_save_file_name}*.json"))

    df_issues = read_issue_files(raw_issue_jsons)

    return df_issues

def generate_df_path(local_files: list) -> pd.DataFrame:
    df_local_path = pd.DataFrame(data = { 'local_path' : local_files})

    # join by local hash
    df_local_path['gitlab_upload_hash'] = df_local_path['local_path'].apply(get_gitlab_hash)
    df_local_path['ext'] = df_local_path['local_path'].apply(lambda x: x.suffix.lower())
    df_local_path['iid'] = df_local_path['local_path'].apply(get_issue_id_from_path)
    return df_local_path

def get_vecto_file_data(df_local_path: pd.DataFrame) -> pd.DataFrame:
    df_readable = df_local_path[df_local_path['ext'].isin([".xml", ".txt", ""])].reset_index()
    # scan for VECTO file content
    df_readable.loc[:,'xml_scan_result']=df_readable.loc[:,'local_path'].apply(read_vecto_file)
    df_readable = df_readable.join(pd.json_normalize(df_readable['xml_scan_result']))

    # order by VIN defined in file, can be made only on readable files
    df_vecto = df_readable.loc[df_readable['VIN'].notna()].copy()
    df_vecto['IssueVIN']=df_vecto['iid'].apply(lambda x: f"{x:04d}") + '_' + df_vecto['VIN']

    df_vecto = df_vecto.drop(columns=["ext", "xml_scan_result","file"])
    df_vecto["vehicle_type"] = df_vecto["vehicle_xsd_type"].str.split('/').str[-1]

    return df_vecto

def select_vecto_inputs(file_type_list: list) -> list:
    x = np.array(file_type_list)

    x_arg = []
    if 0 in file_type_list:
        x_arg.append(int(np.argwhere(x==0).flatten()[0]))
        return x_arg

    # possible combination = [1], [1,3], [2,3]
    if 1 in file_type_list: # prim
        x_arg.append(int(np.argwhere(x==1).flatten()[0]))
        if 3 in file_type_list: # complete
            x_arg.append(int(np.argwhere(x==3).flatten()[0]))
        return x_arg
    if 2 in file_type_list and 3 in file_type_list: # VIF + COMP
        x_arg.append(int(np.argwhere(x==2).flatten()[0]))
        x_arg.append(int(np.argwhere(x==3).flatten()[0]))
        return x_arg

    return x_arg  # empty list — no recognised file type combination

def get_vehicles(df_vecto_ord_filt: pd.DataFrame):

    # for each issue, VIN and xml type, get the last uploaded file
    # --> Group by VIN and type of file / group by output filename if it contains VIN and type of file
    df_vecto_ord_lastFver = df_vecto_ord_filt.groupby(["IssueVIN","vehicle_xsd_type"]).tail(1).copy()

    # important for next step: sort by file type to get them in always same order (used for naming)
    df_vecto_ord_lastFver= df_vecto_ord_lastFver.sort_values(["IssueVIN", 'file_type'])

    # group all files for each issue and VIN to get files associated to a vehicle
    df_vehicles = df_vecto_ord_lastFver.groupby(['IssueVIN']).agg(list)


    # =========================================================================== #
    # Sort files: keep only relevant, remove duplicates
    # =========================================================================== #
    # TO DO
    # define and sort ref inputs (0 or 1,2 (PRIM/COMP) or 2,3 (COMP/VIF)) for each vehicle
    # define vehicle hash
    # group by vehicle_hash to avoid duplicate
    # define ref IssueVIN for a given vehicle hash : first vehicle in list grouped by hash that is declared / first vehicle in list grouped by hash otherwise
    # var needed to define vehicle :
    #   - data_hash_short,
    #   - local_path,
    #   - isDeclared
    #   - file_type,
    #   - iid,
    #   - VIN,
    #   - is_xml_HEV, is_XML_iepc, is_xml_pev, is_xml_ovc

    # REMOVE VIF if there is a primary or a main
    col2sort = [c for c in df_vehicles.columns if c!='file_type']

    df_vehicles['selection'] = df_vehicles['file_type'].apply(select_vecto_inputs)

    no_valid_input = df_vehicles['selection'].apply(len) == 0
    if no_valid_input.any():
        print(f'WARNING: {no_valid_input.sum()} vehicle(s) skipped - no recognized file-type combination.')
        for file_types in df_vehicles.loc[no_valid_input, 'file_type']:
            print(f'  file_type values found: {file_types}')
        df_vehicles = df_vehicles[~no_valid_input]

    for col in col2sort:
        df_vehicles[col] = df_vehicles.apply(lambda x: [x[col][i] for i in x['selection']], axis=1)

    df_vehicles.drop(columns=["selection"], axis=1, inplace=True)

    df_vehicles['Issue_VIN'] = df_vehicles.index.values # as it is the result of a grouby on Issue and VIN, we created for each line a list with the same values multiple times

    for c in ['is_xml_conventional','is_xml_fchv','is_xml_hev', 'is_xml_iepc', 'is_xml_pev', 'is_xml_ovc','veh_class','veh_category','OEM','VIN','iid','art10_label']:
        df_vehicles[c] = df_vehicles[c].str[0] # take first elt, meaning main or primary or VIF

    df_vehicles['veh_hash'] = df_vehicles['data_hash'].apply(lambda x: '_'.join(x))
    df_vehicles['veh_hash_short'] = df_vehicles['data_hash_short'].apply(lambda x: '_'.join(x))
    df_vehicles['veh_Declared'] = df_vehicles['isDeclared'].apply(lambda x: all(x))

    df_vehicles['veh_pwt']='CONV'
    df_vehicles.loc[df_vehicles['is_xml_iepc']==True,'veh_pwt']='IEPC'
    df_vehicles.loc[df_vehicles['is_xml_pev']==True,'veh_pwt']='PEV'
    df_vehicles.loc[df_vehicles['is_xml_hev']==True,'veh_pwt']='HEV'
    df_vehicles.loc[df_vehicles['is_xml_fchv']==True,'veh_pwt']='FCHV'
    # df_vehicles.loc[df_vehicles['is_xml_ovc']==True,'veh_pwt']+='ovc' # there are some PEVovc and IEPCovc ???
    sel_ovc = (df_vehicles['is_xml_ovc']==True) & \
        ((df_vehicles['is_xml_hev']==True) | \
         (df_vehicles['is_xml_fchv']==True))
    df_vehicles.loc[sel_ovc,'veh_pwt']+='ovc' # there are some PEVovc and IEPCovc ???

    return df_vehicles

def get_REF_vehicles(df_vehicles: pd.DataFrame):

    # group by vehicle_hash to avoid duplicate
    df_vehicles_groupby_vhash = df_vehicles.groupby('veh_hash',sort=False).agg(list)

    # select first vehicle that is not undeclared as reference, or the first found if they are all undeclared (it can be found undeclared by mistake)
    df_vehicles_groupby_vhash['REF_idx'] = df_vehicles_groupby_vhash['veh_Declared'].apply(lambda x: x.index(max(x)))

    # generate the REF data to prepare copy:
    for col2sel in ['local_path', 'file_type', 'Issue_VIN',
           'vehicle_type', 'veh_category',  'veh_pwt',
           'veh_hash_short', 'veh_Declared','art10_label']:
        df_vehicles_groupby_vhash['REF_' + col2sel] = df_vehicles_groupby_vhash.apply(lambda x: x[col2sel][x['REF_idx']], axis=1)

    df_vehicles_groupby_vhash['REF_save_dirname']=df_vehicles_groupby_vhash['REF_veh_category'].str.replace(' ','') \
        + '_' + df_vehicles_groupby_vhash['REF_veh_pwt'] \
        + '_' + df_vehicles_groupby_vhash['REF_veh_hash_short']

    # reduce dataframe to ref files to prepqre copy
    col_sel = [c for c in df_vehicles_groupby_vhash.columns if 'REF' in c and c!='REF_idx']

    col_sel = col_sel + ['Issue_VIN']

    df_vehicles_REF = df_vehicles_groupby_vhash[col_sel].dropna()

    return df_vehicles_REF

def generate_PRIM_COMP_job(PRIM,COMP,fjob_fpath):
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
         "PrimaryVehicle": PRIM,
         "InterimStep": COMP,
         "Completed": 'true',
         "RunSimulation": 'true'
       }
     }


    # jobfilename = 'job.vecto'
    # fjob_fpath = os.path.join(dirpath, jobfilename)

    with open(fjob_fpath, "w") as file:
        json.dump(job_dict, file, indent=2)

def generate_PRIMVIF_COMP_job(PRIMVIF,COMP,fjob_fpath):

    # FM from prim VIF and COMP
    # --------------------------
    job_dict = {
          "Header": {
        	"CreatedBy": "Guillaume Alix, JRC, Ispra",
        	"Date": datetime.now().isoformat()+'Z',
        	"AppVersion": "2.2",
        	"FileVersion": 7
          },
          "Body": {
        	"PrimaryVehicleResults":  PRIMVIF,
        	"CompletedVehicle": COMP
          }
        }

    # jobfilename = 'job.vecto'
    # fjob_fpath = os.path.join(dirpath, jobfilename)

    with open(fjob_fpath, "w") as file:
        json.dump(job_dict, file, indent=2)

