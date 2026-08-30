universe = local

executable = D:\Apps\Python\WPy64-312101\python\python.exe
arguments = B13_Run_Single_Vecto_File.py $(vecto) $(input_dir)\input.zip $(output_file_name)_out.zip

request_cpus = 1
batch_name = vecto_batch

request_memory = ifThenElse(NumJobStarts =!= 0, 8192, 4096)

output = D:\CI\CONDOR\Z01_LOGS\$(output_file_name).out
error  = D:\CI\CONDOR\Z01_LOGS\$(output_file_name).err
log    = D:\CI\CONDOR\Z01_LOGS\$(output_file_name).log

HeldStatus = 5
RunningStatus = 2

# https://htcondor.readthedocs.io/en/latest/codes-other-values/job-status-codes.html
periodic_remove = JobStatus == $(RunningStatus) && CurrentTime-EnteredCurrentStatus > 3600*12

# Retry once with doubled memory if the job is held for exceeding request_memory (hold code 34).
# https://htcondor.readthedocs.io/en/latest/users-manual/self-checkpointing-applications.html#automatic-detection-of-jobs-that-are-killed-by-the-oom-killer
periodic_release = (JobStatus == $(HeldStatus)) && (HoldReasonCode == 34) && (NumJobStarts < 2)

queue vecto,input_dir,output_file_name from batch_arguments.txt