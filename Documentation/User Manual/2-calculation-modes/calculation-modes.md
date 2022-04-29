# Calculation Modes

VECTO supports different calculation modes for declaring a vehicle, validation of test-results, or experimenting with different parameters and components. These modes are described here.

- [**Declaration Mode**](#declaration-mode)
    : In this mode a vehicle can be declared. Many simulation parameters are predefined to provide a generic way of comparing the emissions.

- [**Engineering Mode**](#engineering-mode)
    : This mode is for experimenting and validation of a vehicle. There exist several options how the driving cycle may be defined (Target speed, Measured Speed, Pwheel).

- [**Verification Test Mode**](#verification-test-mode)
    : This mode is for simulating a vehicle defined in declaration mode for the purpose of verification tests. This mode is similar to the PWheel simulation in engineering mode.

- [**Engine Only Mode**](#engine-only-mode)
    : This mode is for validation of a measured engine component. Only the engine is simulated in this mode.


In the GUI the Calculation Mode can be changed via the Options Tab of the [Main Form](#main-form).

In the Command Line the default Calculation Mode is Declaration, but can be changed to Engineering with the "-eng" flag.
