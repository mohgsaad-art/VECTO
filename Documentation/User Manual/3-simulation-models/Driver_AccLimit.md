## Driver: Acceleration Limiting

VECTO limits the vehicle acceleration and deceleration depending on current vehicle speed, to model a realistic driver behavior. These limits are defined in the [Acceleration Limiting Input File (.vacc)](#acceleration-limiting-input-file-.vacc), which can be set in the [Job File](#job-file). In Declaration mode this is already predefined.

* If the engine can't provide the required power, the vehicle might accelerate slower than the defined driver limit.
* The minimum deceleration can always be maintained via the brakes.
* In Measured Speed Mode these limits are not used, due to the nature of this mode (speeds and accelerations are already real measured values, therefore VECTO uses them directly without limitation).

![The graph shows the acceleration and deceleration limits depending on the current vehicle speed.](pics/AccLimit.png)
