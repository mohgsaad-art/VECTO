## Gearbox Editor

![](pics/GearboxForm.png)

### Description

The [Gearbox File (.vgbx)](#gearbox-file-.vgbx) defines all gearbox-related input parameters like gear ratios and transmission loss maps. 
Furthermore, certain parameters for the gearshift strategy such as the gearshift lines can be provided (see [Gear Shift Model](#gear-shift-model) for details).


### Relative File Paths

It is recommended to use relative filepaths. This way the Job File and all input files can be moved without having to update the paths. \
Example: "Gears\\Gear1.vtlm" points to the "Gears" subdirectory of the Gearbox File's directoy.

VECTO automatically uses relative paths if the input file (e.g. Shift Polygons File) is in the same directory as the Gearbox File. (The Gearbox File must be saved before browsing for input files.)


### Main Gearbox Parameters

Make and Model
:   Free text defining the gearbox model, type, etc.


Transmission Type
:   Depending on the transmission type some options below are not available. The following types are available:
:   -   **MT**: Manual Transmission
-   **AMT**: Automated Manual Transmission
-   **APT-S**: Automatic Transmission with torque converter - Serial configuration
-   **APT-P**: Automatic Transmission with torque converter - Power Split configuration
-   **APT-N**: Automatic Transmission without torque converter, only applicable for pure electric vehicles

For more details on the automatic transmission please see the [APT-Model](#gearbox-at-gearbox-model)

Inertia \[kgm²\]
:   Rotational inertia of the gearbox (constant for all gears). (Engineering mode only)


Traction Interruption \[s\]
:   Interruption during gear shift event. (Engineering mode only)


### Gears

Use the ![add](pics/plus-circle-icon.png) and ![remove](pics/minus-circle-icon.png) buttons to add or remove gears from the vehicle. Doubleclick entries to edit existing gears.

-   Gear **"Axle"** defines the ratio of the axle transmission / differential.
-    **"Ratio"** defines the ratio between the input speed and output speed for the current gear. Must be greater than 0.
-    **"Loss Map or Efficiency"** allows to define either a constant efficiency value or a [loss map (.vtlm)](#transmission-loss-map-.vtlm). <span class="engineering">Note: efficiency values are only allowed in engineering mode</span>
-    **"Shift polygons"** defines the [Shift Polygons InputFile (.vgbs)](#shift-polygons-input-file-.vgbs) for each gear. Not allowed in [Declaration Mode](#declaration-mode). See [GearShift Model](#gear-shift-model) for details.
-	 **"Max Torque"** defines the maximum allowed torque (if applicable) for a gear. It is used for limiting the engine's torque in certain gears. Note: in Declaration mode the [generic shift polygons](#gear-shift-model) are computed from the engine's full-load curve. If the maximum torque is limited by the gearbox, the minimum of the gearbox and engine maximum torque will be used to compute the [generic shift polygons](#gear-shift-model)!


### Gear shift strategy parameters

![](pics/Vecto_ShiftStrategyParameters.svg)

Some parameters influencing the gearshift behavior can be defined in the gearbox file. Therefore, the gearbox file has to be provided as input for the shift strategy parameters as well. See [Gearbox-TCU](#gearshift-parameters-file-.vtcu) for more details.

In addition, the gearshift polygon affects the gearshift behavior to a certain degree. The gearshift polygon can be defined individually for each gear. If no shift polygon is provided the declaration mode shift polygons for the selected transmission type are used.

The gearshift strategy depends on the transmission type:

Manual Transmission
:   Shiftline based approach. The calculation of gearshift lines and the gearshift rules are [described here](#shift-strategy-mt-gearshift-rules)

Automated Manual Transmission - Conventional vehicle
:   Efficiency shift. The calculation of gearshift lines and the gearshift rules are [described here](#shift-strategy-amt-gearshift-rules)

Automated Manual Transmission - Hybrid Electric vehicle
:   Gearshift is handled by the hybrid controller. Shift lines (calculated in the same way as for conventional vehicles) are used as upper and lower boundary for allowed ICE operating points.

Automated Manual Transmission - Pure Electric vehicle
:   Efficiency shift based strategy. The calculation of gearshift lines and the gearshift rules are [described here](#pev-gear-shift-model)

Automatic Transmission - Conventional vehicle
:   Efficiency shift. The calculation of gearshift lines and the gearshift rules are [described here](#shift-strategy-apt-gearshift-rules)

Automatic Transmission - Hybrid Electric vehicle
:   Gearshift is handled by the hybrid controller. Shift lines (calculated in the same way as for conventional vehicles) are used as upper and lower boundary for allowed ICE operating points.

Automatic Transmission (APT-N) - Pure Electric vehicle
:    Efficiency shift based strategy. The calculation of gearshift lines and the gearshift rules are [described here](#pev-gear-shift-model)

<div class="engineering">

#### Gearshift Parameters

Torque reserve
:   The minimal torque reserve which has to be provided after a gearshift. Only used for MT transmissions.

Minimum time between gearshifts
:   Defines the time interval between two consecutive gearshifts. Has to be greater than 0. This time interval is ignored if the engine speed gets too high or too low.

#### Shift Strategy Parameters

The user interface contains input fields for the following parameters:
: - **Downshift after upshift delay**: to prevent frequent (oscilating) up-/down shifts this parameter blocks downshifts for a certain period after an upshift
- **Upshift after downshift delay**: to prevent frequent (oscilating) up-/down shifts this parameter blocks upshifts for a certain period after a downshift
- **Min acceleration after upshift**: after an upshift the vehicle must be able to accelerate with at least the given acceleration. The achievable acceleration after an upshift is estimated on the current driving condition and powertrain state.

#### Start Gear

In order to calculate an appropriate gear for vehicle start (first gear after vehicle standstill) a fictional load case is calculated using a specified **reference vehicle speed** and **reference acceleration** together with the actual road gradient, transmission losses and auxiliary power demand. This way the start gear is independent from the target speed. VECTO uses the highest possible gear which provides the defined **torque reserve**.


Reference vehicle speed at clutch-in
:   The reference vehicle speed

Reference acceleration at clutch-in
:   The reference acceleration

</div>

### Torque Converter

Torque converter characteristics file
:   Defines the [Torque converter characteristics file](#torque-converter-characteristics-.vtcc) containing the torque ratio and reference torque over the speed ratio.

Inertia \[kgm²\]
:   Rotational inertia of the engine-side part of the torque converter.
(Gearbox-side inertia is not considered in VECTO.)

Reference RPM
:   Defines the reference speed at which the torque converter characteristics file was measured.

Max. Speed
:   Defines the maximum input speed the torque converter can handle.

Torque converter shift polygon
:   Defines the [Shift Polygons InputFile (.vgbs)](#shift-polygons-input-file-.vgbs) separately for the torque converter. For details on shifting from/to the torque converter gear please see [AT Gear Shift Strategy](#shift-strategy-apt-gearshift-rules).


### Torque Converter: Minimal acceleration after upshift

Here the minimal achievable accelerations before upshifts can be defined.

Acc. for C->L \[m/s²\]
:   The minimal achievable acceleration for shifts from torque converter gear to locked gear.

Acc. for C->C \[m/s²\]
:   The minimal achievable acceleration for shifts from first torque converter gear to second torque converter gear (1C->2C)


### Power shift losses

Shift time \[s\]
:   The shift time for powershift losses.

### Chart Area

The Chart Area displays the [Shift Polygons Input File(.vgbs)](#shift-polygons-input-file-.vgbs) as well as the declaration mode shift polygons (dashed lines) for the selected gear together with the engine's full-load curve.


### Controls



![](pics/blue-document-icon.png) New file
:   Create a new empty .vgbx file

![open](pics/Open-icon.png)Open existing file
:   Open an existing .vgbx file


![save](pics/Actions-document-save-icon.png) ***Save current file***

![SaveAs](pics/Actions-document-save-as-icon.png) ***Save file as...***

![sendto](pics/export-icon.png) Send current file to the [VECTO Editor](#job-editor)
:   **Note:** If the current file was opened via the [VECTO Editor](#job-editor) the file will be sent automatically when saved.


![](pics/browse.png) ***Open file browser***

![](pics/OpenFile.PNG) ***Open file*** (see [File Open Command)](#settings).

![OK](pics/OK.png) Save and close file
:   If necessary the file path in the [VECTO Editor](#job-editor) will
be updated.


![Cancel](pics/Cancel.png) ***Cancel without saving***
