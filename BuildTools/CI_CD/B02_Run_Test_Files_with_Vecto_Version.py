# -*- coding: utf-8 -*-
"""
Created on Mon Jan 29 10:33:00 2024

@author: alixgui
"""
# based on https://programtalk.com/python-examples/pebble.process.Pool/
# example 25


# import
import argparse
import datetime
import os
import time
# from multiprocessing import Pool ### no timeout with this
# from functools import partial
from pebble import ProcessPool
# To install pebble
    # - in Windows/anaconda / cmd line :
    #     - in cmd terminal : cd C:\Apps\anaconda3\Scripts
    #     - pip install pebble
    # - in linux / JEODPP :
    #     - in terminal : conda activate vecto_env
    #     - pip install pebble
from concurrent.futures import TimeoutError

from FUNCTIONS.VECTO import IS_CI

import subprocess

# main Vecto Run function to be run in parallel
# ==================================================
def run_individual_file(vectofile_fpath=None, vecto_cmd_path=None, vecto_cmd_option=None, log=True, overwrite=True):

    job_ext = os.path.splitext(vectofile_fpath)[-1]
    # resfile_fpath = vectofile_fpath.replace('.'+job_ext, '.vsum')

    cur_dir = os.path.dirname(vectofile_fpath)
    ls = os.listdir(cur_dir)
    resfile = [f for f in ls if f.endswith(".vsum")]

    if len(resfile)!=0 and overwrite==False:
        return vectofile_fpath, 0.0

    if os.name=='posix' :
        job_cmd = 'taskset -a -c $ML_CORES dotnet "{vectopath}" {vecto_cmd_option} "{vectojob}"'
    else:
        job_cmd = '"{vectopath}" {vecto_cmd_option} "{vectojob}"'

    job_cmd = job_cmd.format(vectopath=vecto_cmd_path,
                             vectojob=vectofile_fpath,
                             vecto_cmd_option=vecto_cmd_option)


    st = time.time()
    # txt = os.popen(job_cmd + " 2>&1").read()
    res = subprocess.run(job_cmd, stdout=subprocess.PIPE, stderr=subprocess.STDOUT, shell=True)
    et = time.time()
    txt = res.stdout.decode('utf8')
    txt = txt + '\n' + 'Running Time : \t' + str(et-st) + 's.'

    if log:
        logpath = vectofile_fpath.replace(job_ext, '.log')
        with open(logpath,'w') as file:
            file.write(job_cmd + '\n\n')
            file.write(txt)

    return vectofile_fpath, et-st

# Main code to define the different files to run
if __name__ ==  '__main__':
    option_runHEV = True
    mode_debug = False
    n_debug = 2
    # option_monitor=True
    option_log = True
    TIMEOUT_SECONDS = 3600*5 if option_runHEV else 3600 # 8 hours max for HEV complet RUN => 8*3600

    # define vecto standard run environment
    # -------------------------------------
    vecto_cmd_local = r'vectocmd.dll' if os.name == 'posix' else r'vectocmd.exe'

    parser = argparse.ArgumentParser(description='Run VECTO simulations on test cases in the test pool.')
    parser.add_argument('vecto_root', help='Path to the VECTO installation directory (must contain vectocmd.exe on Windows, vectocmd.dll on Linux).')
    parser.add_argument('--max-workers', type=int, default=None,
                        help='Number of parallel workers (default: number of CPU cores). Reduce on machines with limited memory.')
    parser.add_argument('--force-rerun', action='store_true', default=False,
                        help='Re-run simulations even if a result file already exists in the output directory.')
    args = parser.parse_args()

    if not os.path.exists(args.vecto_root):
        raise FileNotFoundError('VECTO path does not exist.')

    vecto_root_path = args.vecto_root
    force_rerun = args.force_rerun

    if not os.path.exists(os.path.join(vecto_root_path, vecto_cmd_local)):
        raise FileNotFoundError('There is no available VECTO version to test.')

    vecto_cmd_option = '-nv -t'

    # define result path
    # ------------------
    result_dir = os.path.join(os.getcwd(), 'XX_RESULTS')
    print(result_dir)

    max_workers = 224 if IS_CI else (args.max_workers if args.max_workers is not None else os.cpu_count())

    # Check the content of result directory :
    # - Files that can be run
    # - directories with vecto files
    # - directories with log and/or vsum files (already run, to avoid if )
    # ---------------------------------------------------------------------
    file_list_RUNABLE = [os.path.join(root,file)
                  for root,dummy,files in os.walk(result_dir)
                  for file in files
                  if ((file.endswith('.xml') and 'RSLT' not in file and 'VIF' not in file) or file.endswith('.vecto'))]

    dir_with_vecto_file = [os.path.dirname(file) for file in file_list_RUNABLE if file.endswith('.vecto')]

    dir_with_log_file = [os.path.join(root)
                  for root,dummy,files in os.walk(result_dir)
                  for file in files
                  if (file.endswith('.log'))]
    dir_with_vsum_file = [os.path.join(root)
                  for root,dummy,files in os.walk(result_dir)
                  for file in files
                  if (file.endswith('.vsum'))]

    # Define list of files to run
    # - Run prim xml when no vecto file available in subdirectory
    # - Run vecto file if there is one in subdirectory, but not the XML inside
    # ---------------------------------------------------------------------
    file_list_TORUN = [file for file in file_list_RUNABLE if ((os.path.dirname(file) not in dir_with_vecto_file) or file.endswith('.vecto'))]
    total_vehicles_to_run = file_list_TORUN
    print(total_vehicles_to_run)

    # if force_rerun is False, skip directories that already have a log file
    if not force_rerun:
        file_list_TORUN = [file for file in file_list_TORUN if (os.path.dirname(file) not in dir_with_log_file)]
        # file_list_TORUN = [file for file in file_list_TORUN if (os.path.dirname(file) not in dir_with_vsum_file)]

    if not option_runHEV:
        file_list_TORUN = [file for file in file_list_TORUN if not ('HEV' in file)]

    if mode_debug:
        file_list_TORUN = file_list_TORUN[0:n_debug]

    print(f'Run HEV vehicles has been set to: {option_runHEV}')
    print(f"Available Generic Vehicles to run (total): {len(file_list_TORUN)} ({len(total_vehicles_to_run)})")

    args = []

    for vecto_or_xml_input_file in file_list_TORUN:

        vecto_cmd = os.path.join(vecto_root_path, vecto_cmd_local)

        # arg = [vecto_or_xml_input_file, vecto_cmd, vecto_cmd_option, option_log, option_overwrite]
        if 'ENG_mode' in vecto_or_xml_input_file:
            arg = [vecto_or_xml_input_file, vecto_cmd, vecto_cmd_option + ' -eng', option_log, True] # force overwrite of vsum
        else:
            arg = [vecto_or_xml_input_file, vecto_cmd, vecto_cmd_option, option_log, True] # force overwrite of vsum

        args.append(arg)

    # run Pool computation
    # ====================
    st0 = time.time()

    print('Run Pool')
    print(20*'-')
    now = datetime.datetime.now()
    print('Start at:', now.strftime("%Y-%m-%d %H:%M:%S"))

    # with process.Pool(4) as p:
    # with ProcessPool(max_workers=4, max_tasks=8) as p:
    with ProcessPool(max_workers=max_workers) as p:
        jobs = [p.schedule(run_individual_file,args=arg,timeout=TIMEOUT_SECONDS) for arg in args]

    et0 = time.time()


    # Check results and write error log
    # =================================
    runtimes = []
    nrun_full=0
    nrun_timedout=0
    nrun_error = 0

    print('Checking results')
    print(20*'-')
    now = datetime.datetime.now()
    print('Start checking at:', now.strftime("%Y-%m-%d %H:%M:%S"))

    for arg,job in zip(args,jobs):
        try:
            runtimes.append(job.result())
            nrun_full = nrun_full+1
        except TimeoutError:
            vecto_or_xml_input_file_i, vecto_cmd_i, vecto_cmd_option_i, option_log_i, option_overwrite_i = arg

            nrun_timedout = nrun_timedout+1

            ext = "_timeout_{0}".format(TIMEOUT_SECONDS) + '.log'

            logpath = vecto_or_xml_input_file_i.replace('.xml',ext).replace('.vecto',ext)

            txt = "Function took longer than {0} seconds".format(TIMEOUT_SECONDS)

            with open(logpath,'w') as file:
                file.write(vecto_or_xml_input_file_i + '\n\n')
                file.write(txt)
        except Exception as error:

            vecto_or_xml_input_file_i, vecto_cmd_i, vecto_cmd_option_i, option_log_i, option_overwrite_i = arg

            nrun_error = nrun_error+1

            ext = '_error.log'

            logpath = vecto_or_xml_input_file_i.replace('.xml',ext).replace('.vecto',ext)

            with open(logpath,'w') as file:
                file.write(vecto_or_xml_input_file_i + '\n\n')
                file.write(str(error))


    tfulls = [runtime[1] for runtime in runtimes]
    teff = et0-st0
    ntot = nrun_full+nrun_timedout+nrun_error
    tcum = sum(tfulls) + nrun_timedout*TIMEOUT_SECONDS
    gain_factor = tcum/teff if teff > 0 else 0
    if ntot == 0:
        print('No job done')
    else:
        print(et0-st0, ' seconds', flush=True)
        print((et0-st0)/ntot, ' seconds per job', flush=True)
