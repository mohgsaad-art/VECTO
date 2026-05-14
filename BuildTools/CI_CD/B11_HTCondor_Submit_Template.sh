universe = local

## initialdir = D:\Apps\Python\alixgui\test_case_VECTOCI_HTCONDOR
executable = D:\Apps\Python\WPy64-312101\python\python.exe

## transfer_input_files = run_single_vecto_file.py, D:\Apps\Python\alixgui\test_case_VECTOCI_HTCONDOR\A00_VECTO\$(vecto), D:\Apps\Python\alixgui\test_case_VECTOCI_HTCONDOR\$(input_dir)\input.zip
arguments = B13_Run_Single_Vecto_File.py $(vecto) $(input_dir)\input.zip $(output_file_name)_out.zip

##should_transfer_files = YES
##when_to_transfer_output = ON_EXIT

request_cpus = 1
batch_name = LEGENT_pyjeo_vecto

output = D:\Apps\Python\alixgui\test_case_VECTOCI_HTCONDOR\Z01_LOGS\$(output_file_name).out
error  = D:\Apps\Python\alixgui\test_case_VECTOCI_HTCONDOR\Z01_LOGS\$(output_file_name).err
log    = D:\Apps\Python\alixgui\test_case_VECTOCI_HTCONDOR\Z01_LOGS\$(output_file_name).log

queue vecto,input_dir,output_file_name from batch_arguments.txt