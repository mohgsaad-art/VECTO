## Verification Test Mode

The purpose of the verification test is to simulate a vehicle defined in declaration mode on a measured real-driving cycle. This simulation mode uses its own [cycle format](#verification-test-cycle), requiring mainly vehicle speed, wheel speed, wheel torque, engine-fan speed, and engine speed. VECTO then calculates the appropriate gear and simulates the cycle. Auxiliary power is according to the technologies defined in the vehicle. However, the engine fan auxiliary is ignored and the power demand for the engine fan is calcuated based on the engine-fan speed. The power demand for the other auxiliaries depends on the vehicle's actual speed. The fuel consumption is calculated using the engine speed from the driving cycle and the torque demand as given in the cycle, adding the losses of all powertrain components.

<div class="engineering">
### Requirements

-   One or more checked job files in the Job List
-   Each job must include a vehicle in declaration mode (XML)
-   Each job file must include at least one driving cycle

### Results

-   Modal results (.vmod). One file for each vehicle/cycle combination. Modal results are only written if the modal output is enabled in the 'Options' tab on the [Main Window](#main-form)
-   Sum results (.vsum). One file for each invocation of VECTO.
</div>


<div class="declaration">
### Requirements

-   One or more checked job files in the Job List
-   Each job must include a vehicle in declaration mode (XML)
-   Each job must include the manufacturer report (XML) of the vehicle as generated for the vehicle declaration
-   Each job file must include exactly one driving cycle (in case multiple driving cycles are provided, only the first cycle is simulated!)

### Results

-   VTP Report (.xml). Contains a description of the vehicle and its components, the verification test analysis according to the draft legislation, and a validation of the input data digest values
-   Modal results (.vmod). One file for each vehicle/cycle combination. Modal results are only written if the modal output is enabled in the 'Options' tab on the [Main Window](#main-form)
-   Sum results (.vsum). One file for each invocation of VECTO.


### Validations

-   Before the simulation of the measured VTP cycle starts, the provided cycle data is passed through some sanity checks:
   * The cycle is provided in 2Hz
   * The ratio of wheel speeds (left/right) should be lower than 1.4 for wheel speeds above 0.1rpm
   * The absolute difference of wheel speeds (left/right) should be lower than 1rpm for wheel speeds below 0.1rpm
   * The torque ratio (left/right) should be lower than 3 and the absolute difference should be lower than 200Nm.
   * The fan speed shall be between 20 and 4000rpm, unless the vehicle is equipped with an electric fan
   * The fuel consumption within a window off 10min should be between 180 and 600 g/kWh_(PWheel_pos)

In case the provided cycle exceeds these limits an according warning message is shown in the user interface and written to the report.
</div>