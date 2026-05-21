universe = local

executable = D:\Apps\Python\WPy64-312101\python\python.exe
arguments = B13_Run_Single_Vecto_File.py $(vecto) $(input_dir)\input.zip $(output_file_name)_out.zip

request_cpus = 1
batch_name = vecto_batch

output = D:\CI\CONDOR\Z01_LOGS\$(output_file_name).out
error  = D:\CI\CONDOR\Z01_LOGS\$(output_file_name).err
log    = D:\CI\CONDOR\Z01_LOGS\$(output_file_name).log

# https://htcondor.readthedocs.io/en/latest/codes-other-values/job-status-codes.html
periodic_remove = JobStatus == 2 && CurrentTime-EnteredCurrentStatus > 3600*12

queue vecto,input_dir,output_file_name from batch_arguments.txt