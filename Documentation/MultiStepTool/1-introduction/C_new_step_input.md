# Create a step input file
As mentioned before it is possible to load step input files when creating new jobs. 
Apart from saving the step input while creating a VIF, there is also the possibility to create the step input without a VIF. 

To create a new step input file select **File -> New File -> Create Interim/Completed Input**.


![](images/2-new-job/new_step_input_1_edited.png){width=70%}\


It is possible to create input files for 

- [Non exempted vehicles](#non-exempted-step-input)

- [Exempted vehicles](#exempted-step-input)

## Non exempted step input

When creating a new step input, it is automatically added to the job list for further editing.

![](images/2-new-job/new_step_input_2_edited.png){width=70%}\


The editing of a step input is very similar to the editing of the current step when creating a new VIF, the only difference is that there is no consolidated input data available.
### 1 Mandatory fields
These field are mandatory for every manufacturing step.
Mandatory fields don't have a checkbox and are marked red while empty.

### 2 Enable editing
To edit an optional parameter, this checkbox has to be ticked.

### 3 Parameters for step input
In this column the input data of the current step is displayed, and can be edited. The entries are checked with regard to the data type. The entered data is available and can be edited as long as VECTO wasn't closed and the job wasn't removed from the job list.
In case a mandatory input field is empty, or the value provided by the user is invalid the input element is highlighted with a red border. To remove an entry from the current step uncheck the corresponding checkbox.

### 4 Save and Close

Save the input data to .xml, the data will still be editable via the job list as long as Vecto was not closed and the file was not removed from the joblist.

## Exempted step input
The process for creating an exempted step input are the same as for [non exempted input files](#non-exempted-step-input).


# Airdrag
In the **Airdrag** section the consolidated airdrag data is shown and an airdrag component file can be loaded as input for the current step.

![](images/2-new-job/airdrag-1-edited.png){width=70%}\
\

**1 Load Airdrag Component File**
To set the airdrag component for the current step, it has to be loaded from an existing airdrag component file. To remove the data from the current step, click the trash icon.

**2 Consolidated Airdrag Data**
In this section the consolidated airdrag data from the previous manufacturing steps is displayed.

**3 Current Step Airdrag Data**
In this section the airdrag data from the current step is displayed, the values cannot be edited directly in the GUI.


Contrary to all other components, for air drag it is defined that in case standard values shall be used for a particular vehicle, no component input data shall be provided. In this case VECTO automatically allocates the standard values applicable for the particular vehicle group in the simulations. Assuming an interim manufacturer mounts the body of the complete bus and provides measured air-drag data, this is written to the VIF. In case the body is modified in a later manufacturing step in such a way that the measured air-drag data is no longer applicable but no updated measurement data is available the standard value has to be applied. (learn more in [Airdrag Modified Multistep](#airdrag-modified-multistep)).


## Airdrag Modified Multistep
When airdrag data has been provided in any previous simulation step the **Airdrag Modified Multistage** parameter can be set in the *Vehicle* section, if no airdrag data has been provided in previous steps this parameter is hidden.

All manufacturing steps after the one where the air-drag component was added need to state whether modifications have been made that affect the  air-drag. 

- In case no modifications were made, “AirdragModifiedMultistep” has to be set to false and the previous air-drag component is used.
- In case modifications were made, “AirdragModifiedMultistep” has to be set to true. The interim manufacturer can provide a new measured air-drag component to be used for the simulation. In case no updated measurement is provided VECTO applies the standard values. In the VIF this is indicated by using a special XML type for the air-drag component.


