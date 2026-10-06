universe = local

executable = D:\Apps\Python\WPy64-312101\python\python.exe
arguments = B13_Run_Single_Vecto_File.py $(vecto) $(input_dir)\input.zip $(output_file_name)_out.zip

# Forward the output-archive password (read by B13) from the submitting environment.
environment = "zipmep=$ENV(zipmep)"

request_cpus = 1
batch_name = vecto_batch

max_materialize = 100
request_memory = 4096
retry_request_memory = 8192

output = D:\CI\CONDOR\Z01_LOGS\$(output_file_name).out
error  = D:\CI\CONDOR\Z01_LOGS\$(output_file_name).err
log    = D:\CI\CONDOR\Z01_LOGS\$(output_file_name).log

RunningStatus = 2

# https://htcondor.readthedocs.io/en/latest/codes-other-values/job-status-codes.html
periodic_remove = JobStatus == $(RunningStatus) && CurrentTime-EnteredCurrentStatus > 3600*12


queue vecto,input_dir,output_file_name from batch_arguments.txt