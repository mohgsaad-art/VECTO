## Declaration Mode

In Declaration Mode many input parameters are predefined for the official certification. They are locked in the user interface and will automatically be set by VECTO during calculation. Calculations will be performed for each mission profile (of the corresponding HDV class) with two different loadings: low loading and reference loading. 

Declaration Mode can be activated in the [Options Tab](#main-form).


### Requirements

-   One or more checked job files in the Job List
-   The job files don't need to include driving cycles. These are automatically assigned.

### Results

-   Modal results (.vmod). One file for each vehicle/cycle/loading combination. Modal results are only written if the modal output is enabled in the 'Options' tab on the [Main Window](#main-form)
-   Sum results (.vsum). One file for each invocation of VECTO.
-   Results (.xml). One file for each job.

