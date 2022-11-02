# Create a new Job

New jobs can be created via the file menu.

![](images/2-new-job/new_job_1_edited.png){width=70%}\
\

Three different types of jobs can be created with the multistep tool:

- **[Interim/Completed Job](#new-interimcompleted-job---general-case)**
- **[Primary Job with Interim Input](#new-primary-job-with-interim-input---special-case-i)**
- **[Complete Job](#new-complete-job---special-case-ii)**

They are described in more detail in the following chapters.


## New Interim/Completed Job - General Case
The general case is applicable to interim manufacturing steps and completed manufacturing steps. Required inputs in these steps are the VIF from the previous step and the input parameters of the current step. The latter can be entered via the graphical user interface. 

![](images/2-new-job/general-case.png){width=70%}\
\


![](images/2-new-job/new_vif_edited.png){width=100%}\
\


### 1 Load VIF from previous step
To create a VIF for the current step, the VIF from the previous step has to be loaded.
After the VIF is loaded, the consolidated data from the previous steps is displayed and the input data for the current step can be added.

Note: It is also possible to directly add the VIF from the previous step to the job list.

### 2 Load input for current step (optional)
Instead of editing the input data for the current step manually, it is also possible to import previously saved input parameters. The input parameters can be saved during the creation of a VIF (see [Save and Close](#save-and-close)) or can be created using **File -> New File -> Create Interim/Completed Input** (see [Create a step input file](#create-a-step-input-file)).

### 3 The input data is split into three components.
- **Vehicle**
    Contains the main vehicle parameters (manufacturer, model, masses, seats, dimensions, ADAS etc.)

- **Airdrag**
    Allows the loading of an airdrag component file, and displays the consolidated airdrag data from the previous steps. For more information see [Airdrag](#airdrag)

- **Auxiliaries**
    Contains the paremeters concering auxiliaries (HVAC, LED-Lights etc.).

### 4 Mandatory input fields for every simulation step.
At the interim and completed step most input parameters can be provided individually. Four input parameters are mandatory (Manufacturer, Manufacturer Address, and VIN).

### 5 Consolidated Data
The third column is readonly and shows the consolidated data from the previous steps. These are currently applicable values for the simulation. 
In case of the first interim step of course no previous values are available and therefore nothing is shown.

### 6 Enable editing
To edit a parameter in the current step, the editing has to be enabled by checking the corresponding checkbox.
A special case is editing the input parameters that have to be provided as a group (i.e., dimensions, passenger count, ADAS). If the editing for one parameter of this group is enabled, all parameters from the group need to be provided.


### 7 Current step input Data
In this column the input data of the current step is displayed, and can be edited. The entries are checked with regard to the data type. The entered data is available and can be edited as long as VECTO wasn't closed and the job wasn't removed from the job list.
In case a mandatory input field is empty, or the value provided by the user is invalid the input element is highlighted with a red border. To remove an entry from the current step uncheck the corresponding checkbox.

### 8 Save and Close
- **Save Input As ...**:\
    Saves the input of the current step to a new file.
- **Save Input**: \
    Saves the input of the current step to the loaded input file.
- **Save as new VIF**: \
    creates a new VIF and adds it to the job list. If 'Vehicle Declaration Type' is set to 'Final' and all required parameters are present, the file can directly be simulated.
- **Close**:\
    Closes the window. Note: As long as VECTO is open and the job is not removed from the job list, unsaved changes remain in the job for further editing.


## New Primary Job with Interim Input - Special Case I
If the manufacturer of the primary vehicle also adds certain components of the completed step (e.g., HVAC compressor), it is required that the VIF created at the primary step already contains the provided input parameters of the completed step. Therefore, at the primary step the input consists on the one hand of the input XML for the primary vehicle and on the other hand the XML with parts of the completed vehicle.

![](images/2-new-job/special-case-1.png){width=70%}\
\



To create primary job with interim input create new job with **File -> New File -> New Primary Job with Interim Input**
The job is automatically added to the job list for further editing, but simulation is only possible when the job is saved.

![](images/2-new-job/special-case-1-edited.png){width=70%}\
\



### 1 Load primary input file
In the first simulation step the primary input is simulated and an intermediate VIF and a MRF are created. The step input is then added and the resulting VIF is written to disk.



### 2 Select step input file
Select the step input that should be applied in the first manufacturing step.

Note: If an exempted primary input is loaded, the step input has also be for an exempted vehicle.

### 3 Save and Close
After saving the file the job can be simulated.

Note: If the applied step input is completed and marked as "final", VECTO automatically starts the simulation (as in Special Case II).


## New Complete Job - Special Case II
If a bus is produced and homologated in a single step (“complete”) it is required that the simulation of the whole vehicle is done in one VECTO invocation, applying the factor method. VECTO in this case creates the MRF of the primary step, the MRF of the completed step, the VIF of the first manufacturing step with the complete vehicle information, and the CIF. Thus, the input consists of both, the input XML for the primary vehicle and the XML with all parameters of the completed vehicle. 

![](images/2-new-job/special-case-2.png){width=35%}\
\


To create a job for Special Case II, select **File -> New File -> New Complete Job**, the further steps are identical to [Special Case I](#new-primary-job-with-interim-input---special-case-i).

Note: If the VIF resulting from the primary input and the step input, cannot be simulated, add the resulting VIF to the job list and click the info icon to get additional information.


