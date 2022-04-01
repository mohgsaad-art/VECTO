## Engineering Mode

The Engineering Mode lets the user define every aspect in the component models of the vehicle and the driving cycle. This is for experimenting and validation purposes.

In this mode the given list of job files is simulated with the respective driving cycles. Each job file defines a separate vehicle.


### Requirements

-   One or more checked job files in the Job List
-   Each job file must include at least one driving cycle

### Results

-   Modal results (.vmod). One file for each vehicle/cycle combination. Modal results are only written if the modal output is enabled in the 'Options' tab on the [Main Window](#main-form)
-   Sum results (.vsum). One file for each invocation of VECTO.


### Options
The Driving Cycle determines the simulation method in engineering mode. The option depends directly on the driving cycle input and cannot be set explicitely. For more information about the formats see [Driving Cycles](#driving-cycles-.vdri).

* [Target speed, distance-based](#engineering-mode-target-speed-distance-based-cycle)
:   This option is the a target vehicle speed distance based cycle (like in Declaration Mode). With this option experiments can be made by the manufacturer.
* [Measured speed, time-based](#engineering-mode-measured-speed-time-based-cycle)
:   Driving Mode where the actual speed from measurements is simulated.
* [Measured speed with gear, time-based](#engineering-mode-measured-speed-with-gear-time-based-cycle)
:   Driving Mode where the actual speed from measurements is simulated. Also defines the chosen gear.
* [Pwheel (SiCo) Mode, time-based](#engineering-mode-pwheel-sico-time-based)
:   In Pwheel mode the measured power at the wheels is given, and the simulation takes that as input.

**Note:** Time-based driving cycles support arbitrary time steps. However, certain actions are simulated within a single simulation interval (e.g. closing the clutch after a gear switch) and may thus result in artefacts during the simulation due to engine inertia, gearbox inertia, etc. Thus **the suggested minimum time interval for time-based cycles is 0.5s!**
