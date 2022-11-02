
# User Interface Overview
![](images/1-ui-overview/overview1-unedited.png){width=70%}\
\



The main UI consists of three tabs, which are descriped in the following chapters

- **[Jobs](#jobs)**\
    Add, simulate and edit jobs and input files.

- **[Settings](#settings)**\
    Simulation and GUI Settings.

- **About**\
    About VECTO.





## Jobs
The Jobs tab shows an overview of currently loaded jobs and allows the loading, editing and simulation of jobs.
Note: Also jobs that cannot be edited with the GUI (i.e Primary Input Files), can be loaded and simulated.



![](images/1-ui-overview/overview1-edited.png){width=100%}\
\



### Simulation and Job Commands

#### Simulation: 
Starts the simulation of all jobs that are marked for simulation. To mark a job for simulation, tick the checkbox in the left column of the job list. (see [Start Simulation](#start-simulation))


#### Stop:
Stops the current simulation, this operation can take some time and lead to error messages in the message view.

#### Up:
Moves the currently selected job up

#### Down:
Moves the currently selected job down



### Joblist
The joblist shows the name, type and file location of all currently loaded jobs and input files. 

#### Adding Jobs
New jobs from existing files can be added to this list with the buttons in the [File Section](#file-section), Drag & Drop or using **File -> Load File**. New Jobs can be created using **File -> New File** (explained in more detail in the [Create a new Job](#create-a-new-job) section).

#### Edit Jobs
A job can be edited by double clicking or using the **Edit Job** button in the [File Section](#file-section).
It is also possible to show the source file of the currently selected job in the explorer or in the default editor using **Edit -> Source File -> Show in Explorer**  or **Edit -> Source File -> Open in Editor**.

#### Start Simulation
By ticking the checkbox in the first column, a job is marked for simulation.
When a job is simulated the jobfile is loaded again from the disk, therefore only saved changes are considered in the simulation.

If no simulation is possible, a small icon is displayed instead of the checkbox (as shown in the picture below). 
There are many reasons why a job cannot be simulated, clicking the icon shows additional information.

![](images/1-ui-overview/additional-info-1-edited.png){width=70%}\
\



### File Section
- **Load File**: 
    Load a new job from a .xml or .vecto file.
- **Edit**:
    Edit the selected job.
- **Remove**: 
    Remove the selected job from the list.





### Messages


![](images/1-ui-overview/simulation-section1-edited.png){width=100%}\
\


The message section display status messages from the simulation. Warnings are displayed in yellow, and errors in red. When a simulation is completed the result files can be displayed in an editor or in explorer.

![](images/1-ui-overview/simulation_finished.png){width=100%}\
\




The progress bar on top shows the total simulation progress.
The status bar shows the progress and duration of the total simulation and the progress of the individual simulation runs. 

When running a chained simulation (Special Case I, Special Case II), it's possible that another simulation is started automatically after the first step has completed, therefore the progress is not one hundred percent accurate.

## Settings


![](images/1-ui-overview/settings-1-edited.png){width=100%}\
\


### 1 Default directories
#### Default input path
Specifies the initial directory for loading and saving files. Note: After loading or saving a file, the filedialog uses the last used directory as initial directory.

#### Default output path
Specifies the output directory for simulation results.

### 2 Simulation settings
Various simulation settings can be found here.

#### Serialize Vecto Run Data

???

#### ModalResults1Hz
If selected, the modal results (.vmod file) will be converted into 1Hz after the simulation. This may add certain artefacts in the resulting modal results file.


#### Validate
Enables or disables internal checks if the model parameters are within a reasonable range. When simulating a new vehicle model it is good to have this option enabled. If the model parameters are from certified components or the model data has been modified slightly this check may be disabled. The VECTO simulation will abort anyways if there is an error in the model parameters. Enabling this option increases the simulation time by a few seconds.


#### Actual Modal Data

????

#### Write Modal Results
Toggle output of modal results (.vmod files) in declaration mode. A Summary file (.vsum) is always created.


