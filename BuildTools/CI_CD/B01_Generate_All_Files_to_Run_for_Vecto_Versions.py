# -*- coding: utf-8 -*-
"""
Created on Mon Jan 29 10:33:00 2024

@author: alixgui
"""

# import
import argparse
import os
import shutil

parser = argparse.ArgumentParser(description='Prepare XX_RESULTS run directory by copying test cases from the test pool.')
parser.add_argument('vecto_root', help='Path to the VECTO installation directory.')
parser.add_argument('test_pool_path', help='Directory containing 02_TO_RUN_GENERIC and 12_TO_RUN_ISSUES. XX_RESULTS will be created here.')
parser.add_argument('--force-rerun', action='store_true', default=False,
                    help='Re-copy input files into XX_RESULTS even if they already exist, overwriting previously copied files. Use this to reset the run directory after changes to the input test pool.')
args = parser.parse_args()

if not os.path.exists(args.vecto_root):
    raise FileNotFoundError('VECTO path does not exist.')
if not os.path.exists(args.test_pool_path):
    raise FileNotFoundError('Test pool path does not exist.')

vecto_root = args.vecto_root
test_pool_path = args.test_pool_path
force_rerun = args.force_rerun


# define path to get ready-to-run input files
# -------------------------------------------
input_dirs = [r'02_TO_RUN_GENERIC', r'12_TO_RUN_ISSUES']

input_dirs_to_copy = []

for input_dir in input_dirs:

    input_dir_fullpath = os.path.join(test_pool_path,input_dir)

    # Fetch files to run by filtering out RSLT and VIF files and keeping only .xml and .vecto input files.
    file_list_RUNABLE = [os.path.join(root,file)
                  for root,dummy,files in os.walk(input_dir_fullpath)
                  for file in files
                  if ((file.endswith('.xml') and 'RSLT' not in file and 'VIF' not in file) or file.endswith('.vecto'))]

    dir_with_vecto_file = [os.path.dirname(file) for file in file_list_RUNABLE if file.endswith('.vecto')]

    file_list_TORUN = [file for file in file_list_RUNABLE if ((os.path.dirname(file) not in dir_with_vecto_file) or file.endswith('.vecto'))]

    input_dirs_to_copy_i = [os.path.dirname(f) for f in file_list_TORUN]
    input_dirs_to_copy = input_dirs_to_copy + input_dirs_to_copy_i

# define result path
# ------------------
result_dir = os.path.join(os.getcwd(), 'XX_RESULTS')
os.makedirs(result_dir, exist_ok=True)

for input_dir_to_copy in input_dirs_to_copy:

        dirs_to_copy=os.path.normpath(input_dir_to_copy)
        for path2remove in input_dirs:
            dirs_to_copy = dirs_to_copy.replace(os.path.normpath(path2remove)+os.sep,'')

        dir_name = os.path.normpath(os.path.relpath(dirs_to_copy, test_pool_path))
        output_dir = os.path.join(result_dir, dir_name)

        # if output_dir doesn't exist, copy directories
        if (not os.path.isdir(output_dir)):
            shutil.copytree(input_dir_to_copy, output_dir) # only for stand-alone directory
        else:
            if (force_rerun):
                l_content2copy=[p for p in os.listdir(input_dir_to_copy)]
                for f_or_d in l_content2copy:
                    fpath_in = os.path.join(input_dir_to_copy,f_or_d)
                    fpath_out = os.path.join(output_dir,f_or_d)

                    if os.path.isdir(fpath_in):
                        if os.path.isdir(fpath_out):
                            shutil.rmtree(fpath_out)
                        shutil.copytree(fpath_in, fpath_out)


                    if os.path.isfile(fpath_in):
                        if os.path.isfile(fpath_out):
                            os.remove(fpath_out)

                        shutil.copyfile(fpath_in, fpath_out)
