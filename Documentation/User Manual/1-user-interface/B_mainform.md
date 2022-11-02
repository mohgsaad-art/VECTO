## Main Form



![](pics/mainform.svg)


### Description


The Main Form is loaded when starting VECTO. Closing this form will close VECTO even if other dialogs are still open. In this form all global settings can be controlled and all other application dialogs can be opened.

In order to start a simulation the [Calculation Mode](#calculation-modes) must be set and at least one [Job File (.vecto)](#job-editor) must added to the Job List. After clicking START all checked files in the Job List will be calculated. 

The Main Form includes two tabs as described below:

* Job Files Tab
* Options Tab


### Job Files Tab


#### Job Files List#

Job files (.vecto) listed here will be used for calculation. Unchecked files will be ignored!
Doubleclick entries to edit job files with the [VECTO Editor](#job-editor).

![cb](pics/checkbox.png) All
:   (Un-)Check all files in Job List. Only checked files are calculated when clicking START.

![add](pics/plus-circle-icon.png) ***Add files to Job List***

![remove](pics/minus-circle-icon.png) ***Remove selected files from List***

![up](pics/Actions-arrow-up-icon.png)![down](pics/Actions-arrow-down-icon.png) ***Move selected files up or down in list***

##### List Options#

- **Save/Load List**
    - Save or load Job List to text file
- **Load Autosave-List**
    - The Autosave-List is saved automatically on application exit and calculation start
- **Clear List**
    - Remove all files from Job List
- **Remove Paths**
    - Remove paths, i.e. only file names remain using the Working Directory as source path.





#### ![START](pics/Play-icon.png) ***START Button***

Start VECTO in the selected mode (see [Options](#options-tab)).



### Options Tab

![](pics/VECTO_OptionsTab.png)

In this tab the global calculation settings can be changed.

**Mode**

:   Select either [Declaration Mode](#declaration-mode) or [Engineering Mode](#engineering-mode)

**Output Directory**

:   This input can be used to write all simulation result files to a certain directory. This can be either an absolute path or a relative path. If an absolute path is provided, all result files are written to this directory. If a relative path is provided the .vmod and XML reports are written into the corresponding subdirectory of the job file and the .vsum file is written to the corresponding subdirectory of the first selected job file.

**Output**

![cb](pics/checkbox.png) Write modal results
:   Toggle output of modal results (.vmod files) in declaration mode. A Summary file (.vsum) is always created.

![cb](pics/checkbox.png) Modal results in 1Hz
:   If selected, the modal results (.vmod file) will be converted into 1Hz after the simulation. This may add certain artefacts in the resulting modal results file.

**MISC**

Validate Data
:    Enables or disables internal checks if the model parameters are within a reasonable range. When simulating a new vehicle model it is good to have this option enabled. If the model parameters are from certified components or the model data has only been modified slightly this check may be disabled. The VECTO simulation will abort anyways if there is an error in the model parameters. Enabling this option increases the simulation time by a few seconds.

Output values in vmod at beginning and end of simulation iterval
:    By default VECTO writes the simulation results at the middle of every simulation interval. If this option is enabled, the .vmod file will contain two entries for every simulation interval, one at the beginning and one at the end of the simulation interval. Enabling this option may be helpful for analysing the trace of certain signals but can not be used for quantitative analyses of the fuel consumption, average power losses, etc. The generated modal result file has the suffix '_sim'. The picture below shows the difference in the output (top: conventional, bottom: if this option is checked)

![Regular VECTO .vmod output (top) vs. beginning and end of simulation interval (bottom)](pics/VECTO_vmod_vgl.png)


### Controls

![new](pics/blue-document-icon.png) New Job File
: Create a new .vecto file using the [VECTO Editor](#job-editor)


![open](pics/Open-icon.png) Open existing Job or Input File
: Open an existing input file (Job, Engine, etc.)


![tools](pics/Misc-Tools-icon.png) ***Tools***

- **[Job](#job-editor), [Vehicle](#vehicle-editor-general-tab), [Engine](#engine-editor), [Gearbox](#gearbox-editor) Editor**
    - Opens the respective Editor
- **Graph**
    -   Open a new [Graph Window](#graph-window)
- **Open Log**
    -   Opens the [Log File](#application-files) in the system's default text editor
- **Settings**
    -   Opens the [Settings](#settings) dialog.


![info](pics/Help-icon.png) ***Help***

- **User Manual**
    - Opens this User Manual
- **Release Notes**
    - Open the Release Notes (pdf)
- **Report Bug via CITnet / JIRA**
    - Open the CITnet/JIRA website for reporting bug
- **Create Activation File**
    - Create an Activation File used for Licensing
- **About VECTO**
    - Information about the software, license and support contact


Message List
: All messages, warnings and errors are displayed here and written to the log file LOG.txt in the VECTO application folder.
Depending on the colour the following message types are displayed:

-   <span style="font-family: Courier New;">Status Messages</span>
-   <span style="font-family: Courier New; background-color: rgb(255, 204, 102);">Warnings</span>
-   <span style="font-family: Courier New; background-color: red; color: white;">Errors</span>
-   <span style="font-family: Courier New; text-decoration: underline; color: rgb(51, 51, 255);">Links</span> - click to open file/user manual/etc.

Note that the [message log](#application-files) can be opened in the ![](pics/Misc-Tools-icon.png) Tools menu with **Open Log**.

In addition to the log messages shown in the message list, Vecto writes more elaborate messages in the subdirectory logs. If multiple simulations are run in parallel (e.g., in declartion mode a vehicle is simulated on different cycles with different loadings) a separate log-file is created for every simulation run.

Statusbar
: Displays current status and progress of active simulations. When no simulation is executed the current mode is displayed (Engineering or Declaration Mode).
