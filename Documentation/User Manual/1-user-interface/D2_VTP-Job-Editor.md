## VTP-Job Editor


![](pics/VTP-Job.png)


### Description

A VTP-Job is intended to verify the declared data of a vehicle through an on-road test. VTP-Jobs can be either simulated in engineering mode or declaration mode. For a VTP simulation the measured driving cycle along with the VECTO job-file is required. The driving cycle has to contain the vehicle's velocity, rotational speed of the driven wheels, torque of the driven wheels, and fuel consumption in a temporal resolution of 2Hz.

VECTO computes the best matching gear based on the vehicle parameters, the actual vehicle speed and the engine speed.
Next, VECTO re-computes the fuel consumption based for the given driving cycle. For a VTP-test the re-computed fuel consumption has to be within certain limits of the real fuel consumption.

The [VTP job file (.vecto)](#vtp-job-file) includes all informations to run a VECTO calculation. It defines the vehicle and the driving cycle(s) to be used for calculation. In summary it defines:

-   Filepath to the Vehicle File ([.xml](#xml-job-file-declaration-mode)) which defines all relevant parameters, including all components
-   Driving Cycles


<div class="engineering">
In engineering mode multiple driving cycles can be specified
</div>

<div class="declaration">
In declaration mode only the first given driving cycle is simulated as the results are further compared with the re-simulated results of the reference cycle. The reference cycle is the first driving cycle applicable for the actual vehicle group as listed in the Job Window and provided in the reports (i.e., LongHaul for most heavy lorries).

In declaration mode the manufacturer's record file needs to be provided. Furthermore, declaration mode simulations consider correction factors for the net calorific value of the used fuel and the vehicle's mileage. In engineering mode the according input fields are not shown.
</div>

### Relative File Paths

It is recommended to use relative filepaths. This way the Job File and all input files can be moved without having to update the paths. Example: "Vehicles\\Vehicle1.xml" points to the "Vehicles" subdirectory of the Job File's directoy.

VECTO automatically uses relative paths if the input file (e.g. Vehicle File) is in the same directory as the Job File. (*Note:* The Job File must be saved before browsing for input files.)



Cycles
:	List of cycles used for calculation. The .vdri format is described [here](#driving-cycles-.vdri).
**Double-click** an entry to open the file (see [File Open Command](#settings)).
**Click** selected items to edit file paths.

: ![addcycle](pics/plus-circle-icon.png) Add cycle (.vdri)
: ![remcycle](pics/minus-circle-icon.png) Remove the selected cycle from the list



### Chart Area

The chart area on the right shows the main vehicle parameters like HDV group and axle configuration if a valid Vehicle File is loaded into the Editor. The plot shows the full load curve(s) and sampling points of the fuel consumption map. 

### Controls

![new](pics/blue-document-icon.png) New Job File
:	Create a new empty .vecto file

![open](pics/Open-icon.png) Open existing Job File
:	Open an existing .vecto file

![save](pics/Actions-document-save-icon.png) ***Save current Job File***

![SaveAs](pics/Actions-document-save-as-icon.png) ***Save Job File as...***

![sendto](pics/export-icon.png) Send current file to Job List in [Main Form](#main-form)
:	**Note:** The file will be sent to the Job List automatically when saved.

![](pics/browse.png) ***Browse for vehicle file***

![OK](pics/OK.png) Save and close file
:	File will be added to Job List in the [Main Form](#main-form).

![Cancel](pics/Cancel.png) ***Cancel without saving***
