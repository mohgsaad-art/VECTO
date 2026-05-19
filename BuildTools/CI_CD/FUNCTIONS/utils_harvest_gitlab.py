# -*- coding: utf-8 -*-
"""
Created on Wed Apr 15 14:02:00 2026

@author: alixgui
"""

import itertools as itt
import json
import sys
from datetime import datetime, timedelta
from pathlib import Path
from typing import TypedDict
from typing import Literal as L

import gitlab
import pandas as pd
import patoolib
from ordered_set import OrderedSet as oset  # type: ignore

try:
    from IPython.display import Markdown, display
except ImportError:
    display = print  # type: ignore
    Markdown = str  # type: ignore

from .utils import ensure_dir, next_fpath, dictify
from .utils import trim_root, locate_pswd, check_archive, extract_archive
from .utils import get_issue_id_from_path, scan_issue_files

# local function

def store_issue_data(issue_data: dict, 
                     issue_local_dir: str|Path, 
                     issue_desc_save_file_name:str) -> bool:
    
    save_dir = Path(str(issue_local_dir))
    
    outfile_fpath_template = (
        save_dir / f"{issue_desc_save_file_name}_v{{:02d}}.json"
    )
    (outfile_fpath, old_outfile_fpath) = next_fpath(
        outfile_fpath_template, start=1, overwrite=False, also_last=True
    )

    ## Download new data only if different from last time fetched.
    new_file_created = False
    if not old_outfile_fpath or json.loads(old_outfile_fpath.read_text()) != issue_data:
        print(f"  + new version of {outfile_fpath}")
        ensure_dir(outfile_fpath.parent)
        with outfile_fpath.open("wt") as f:
            json.dump(issue_data, f, indent=4)
        new_file_created = True
    return new_file_created

T_scan = TypedDict("T_scan", {"iid": int, "links": list[str], "VINs": list[str]})

def make_scan_df(
    all_scans: list[T_scan], scanfile_key: L["links"]
) -> pd.DataFrame:
    dfs = [
        pd.DataFrame({"iid": s["iid"], scanfile_key: s[scanfile_key]})
        for s in all_scans
    ]

    # `drop_duplicates()`:
    #       with multiple version of an issue,
    #       the same (issue, VIN/link) couple can appear multiple times.
    return pd.concat(dfs).drop_duplicates().set_index("iid")

def make_scan_list(
    all_scans: list[T_scan], scanfile_key: L["links"]
) -> list:
    all_fld_values = list(oset(itt.chain(*[i[scanfile_key] for i in all_scans])))
    return all_fld_values

def dl_link_gitlab(gl, dl_url_base, lnk, fpath, token):
    # gl is a fitlab session
    
    outfile = Path(str(fpath))
    
    file_url = dl_url_base + lnk
    
    response = gl.session.head(file_url, headers={"Authorizaton": token})
    status_code = response.status_code
    file_present_on_gitlab = status_code == 200
    
    if file_present_on_gitlab:
        ensure_dir(outfile.parent)
        response = gl.session.get(file_url, headers={"Authorizaton": token})
        with outfile.open("wb") as f:
            f.write(response.content)
            
    return status_code

def get_last_issues(gitlab_project, labels, updated_after: datetime = None):
    
    date_format_output=r'%Y-%m-%dT%H:%M:%S.%fZ'
    date_format_input=r'%Y-%m-%dT%H:%M:%SZ'
    
    updated_after_str = None
    if updated_after is not None:
        updated_after_str = updated_after.strftime(date_format_input)
    # expected input in ISO 8601 format (2019-03-15T08:00:00Z).
    
    issues_by_label = [gitlab_project.issues.list(labels=label,
                               order_by='updated_at', 
                               updated_after = updated_after_str,
                               sort='desc', 
                               all=True) for label in labels]
    
    issues_flatten = list(itt.chain.from_iterable(issues_by_label))
    
    issue_dates = [(i,datetime.strptime(i.updated_at,date_format_output)) for i in issues_flatten]
    
    issue_dates_sorted = sorted(
                issue_dates, 
                key=lambda x: x[1]
            )
    
    issue_list = [i[0] for i in issue_dates_sorted]
    
    return issue_list # return last update date

def get_art10_labels() -> list:
    art10_labels = [
        "art10::open",  # TODO: rename to `acknowledged` after "type:art10` in the template.
        "art10::need_info",
        "art10::approved",
        "art10::in_deployment",
        "art10::resolved",
        "art10::rejected",
    ]
    return art10_labels

def get_art10_labels_valid()-> list:
    art10_labels_valid = [l for l in get_art10_labels() if not 'reject' in l]
    return art10_labels_valid

def get_issue_data(gitlab_issue):
    
    issue_data = {
        "iid": gitlab_issue.iid,
        "issue": gitlab_issue.asdict(),
        # "discussions": dictify(issue.discussions.list(order_by="updated_at")),
        "notes": dictify(gitlab_issue.notes.list(order_by="updated_at",order='asc',get_all=True)), # used to get link, must be in chronological order
        "status_events": dictify(gitlab_issue.resourcestateevents.list()),
        "label_events": dictify(gitlab_issue.resourcelabelevents.list()),
        "milestone_events": dictify(gitlab_issue.resourcemilestoneevents.list()),
    }
    
    return issue_data

def get_issue_local_path(save_dir:str|Path, iid)-> Path:
    
    save_dir_path = Path(str(save_dir))
    save_dir_i = save_dir_path / f"{iid:04d}"
    return save_dir_i

def get_link_local_path(save_dir:str|Path, iid, link:str|Path)-> Path:
    
    link_path = trim_root(link)
    save_dir_i = get_issue_local_path(save_dir,iid)
    save_dir_l = save_dir_i / link_path
    
    return save_dir_l

def update_password(file="credentials", path=None):
    """Reload archive passwords from credentials.

    Args:
        file: ignored (kept for backward compatibility).
        path: optional path to a credentials.py file. When given, the module is loaded
              from that path rather than from sys.path.
    """
    import importlib
    import importlib.util

    sys.modules.pop("credentials", None)
    globals().pop("credentials", None)

    if path:
        spec = importlib.util.spec_from_file_location("credentials", path)
        if spec is None:
            raise FileNotFoundError(f"credentials module not found at: {path}")
        ms = importlib.util.module_from_spec(spec)
        spec.loader.exec_module(ms)
    else:
        ms = importlib.import_module("credentials")

    archive_passwords = ms.ARCHIVE_PASSWORDS
    df_passwords = (
        pd.DataFrame(archive_passwords, columns=["iid", "filename", "Password"])
        .drop_duplicates()
        .set_index(["iid", "filename"], drop=True)
        .sort_index()
    )
    return df_passwords

if __name__ == '__main__':


    # =========================================================================
    # SET LOCAL ENV
    # =========================================================================

    import time

    # get local / personal stuff
    # -------------------------------------------------------------------------
    sys.modules.pop("credentials", None)
    globals().pop("credentials", None)
    import credentials

    # %%
    wsite = "https://code.europa.eu"

    project_path = getattr(credentials, "PROJECT_PATH", "vecto/vecto")
    # Issue-ids to process;  search by Art10 labels if empty.
    issues_to_process = getattr(credentials, "ISSUES_TO_PROCESS", None)
    token = credentials.GITLAB_TOKEN
    archive_passwords = credentials.ARCHIVE_PASSWORDS
    df_passwords = (
        pd.DataFrame(archive_passwords, columns=["iid", "filename", "Password"])
        .drop_duplicates()
        .set_index(["iid", "filename"], drop=True)
        .sort_index()
    )
    defaultpassword = getattr(credentials, "DEFAULT_ARCHIVE_PASSWORD", None)
    
    # saving paths
    # -------------------------------------------------------------------------
    save_dir = Path("A01_GITLAB_UTILS") # / Path(f"{project.path}")
    save_dir_RAW = save_dir / Path("A00_RAW")
    issue_desc_save_file_name = "issue_txt_data"
    save_dir_EXTRACT = save_dir / Path("A01_EXTRACT")
     
    # everything else
    # -------------------------------------------------------------------------
    extract_verbosity=-1 # do not display patool results
    count=1
    
    # =========================================================================
    # PROCESSING
    # =========================================================================
    
    print('start processing')
    start_time = time.time()
    
    # CONNECT TO GITLAB
    # -------------------------------------------------------------------------
    gl = gitlab.Gitlab(url=wsite, private_token=token)

    # Make an API request to validate authentication token (not really necessary).
    gl.auth()
    
    project = gl.projects.get(project_path)
    
    dl_url_base = project._links["self"].replace("api/v4/projects/", "-/project/")
    
    print(count,' - ','Gitlab Connection:', time.time() - start_time, 'sec')
    start_time = time.time()
    count += 1
    
    # LIST ISSUES
    # -------------------------------------------------------------------------
    date_ref = datetime.now() - timedelta(days=365*5)
    # date_ref = datetime.now() - timedelta(days=5)
    # date_ref = datetime.now() - timedelta(days=10)
    
    art10_labels_valid = get_art10_labels()
    issues = get_last_issues(gitlab_project=project, labels=art10_labels_valid, updated_after = date_ref)
    
    print(count,' - ','List Issues:', time.time() - start_time, 'sec')
    start_time = time.time()
    count += 1

    if not issues:
        print('X  -  Stop - So new issue')
    else:
        date_string = date_ref.strftime(r'%Y-%m-%dT%H:%M:%SZ')
        print(f"   *  Issue(s) Update since {date_string}:" , [i.iid for i in issues])
        # STORE ISSUES DATA
        # ---------------------------------------------------------------------
    
        is_issue_data_updated = False
        
        save_dirs_updated = []
    
        for issue in issues:
            
            issue_data=get_issue_data(issue)
            
            save_dir_RAW_i = get_issue_local_path(save_dir_RAW, issue.iid)
            
            is_file_updated = store_issue_data(issue_data,save_dir_RAW_i,issue_desc_save_file_name)
            
            is_issue_data_updated = is_issue_data_updated | is_file_updated
            
            save_dirs_updated.append(save_dir_RAW_i)
            
        print(count,' - ','Store data:', time.time() - start_time, 'sec')
        start_time = time.time()
        count += 1
        
        # SCAN FOR LINKS IN ENTIRE HISTORY AND DOWNLOAD
        # ---------------------------------------------------------------------
        
        issue_data_files = list(save_dir_RAW.glob(f"*/{issue_desc_save_file_name}*.json"))
        
        # issue_data_files_select = issue_data_files
        issue_data_files_select = [f for f in issue_data_files if (f.parent in save_dirs_updated)]
        
        # not, we could take only the last file, but we take all the files to make sure nothing has been forgotten
        # in any case, if we do not overwrite something already downloaded
        
        
        all_scans = scan_issue_files(issue_data_files_select)
        
        df_issue_links = make_scan_df(all_scans,"links")
        
        print(count,' - ','Scan data:', time.time() - start_time, 'sec')
        start_time = time.time()
        count += 1
        
        # DOWNLOAD LINKS
        # ---------------------------------------------------------------------
        
        for iid,line in df_issue_links.iterrows():
            
            link = line["links"]
            dl_path = get_link_local_path(save_dir_RAW,iid,link)
            
            if not dl_path.exists():
            
                status_code = dl_link_gitlab(gl, dl_url_base, link, dl_path, token)
            
                file_present_on_gitlab = status_code == 200
            
                if not file_present_on_gitlab:
                    print(f"   *  WARNING: file not present on gitlab for issue {iid}: " , link)
                    
        print(count,' - ','Download files uploaded on gitlab:', time.time() - start_time, 'sec')
        start_time = time.time()
        count += 1
        
        # EXTRACT LINKS DOWNLOADED
        # ---------------------------------------------------------------------
        
        downloaded_files = [i for i in save_dir_RAW.glob("*/uploads/**/*") if i.is_file()]
        
        df_passwords = update_password(file="credentials")
        
        for outfile in downloaded_files:
            
            is_archive = patoolib.is_archive(outfile)
            
            if not is_archive:
                continue
            
            extract_path = Path(str(outfile).replace(str(save_dir_RAW),str(save_dir_EXTRACT))).parent
            
            extract_path_already_used = (extract_path.exists() and any(extract_path.iterdir()))
            
            if extract_path_already_used:
                continue
                
            iid = get_issue_id_from_path(outfile)
            password = locate_pswd(df_passwords, outfile.name, iid)
            archive_err = check_archive(outfile, password=password, defaultpassword=defaultpassword, verbosity=extract_verbosity)
        
            if (archive_err is None):
                archive_err = extract_archive(outfile, extract_path, password=password, defaultpassword=defaultpassword, verbosity=extract_verbosity)
            else:
                print(10*' ' + f"* extracting {outfile} failed")
                
        print(count,' - ','Extract archives downloaded from gitlab:', time.time() - start_time, 'sec')
        start_time = time.time()
        count += 1