## Gearbox: AT Gearbox Model

Vecto supports both, AT gearboxes with serial torque converter and AT gearboxes with power split. Internally, both gearbox types are simulated using a power train architecture with the torque converter in series.

![Automatic transmission with torque converter in series](pics/AT-S.svg)

![Automatic transmission with parallel torque converter](pics/AT-P.svg)

In the input data [Gearbox File](#gearbox-file-.vgbx) **only the mechanical gears need to be specified**. Depending on the gearbox type (AT-S or AT-P) Vecto adds the correct virtual 'torque converter gear'.

For AT gearbox with serial torque converter, the torque converter uses the same ratio and mechanical losses as the first gear (and second, depending on the gear ratios), and adds the torque converter.

For AT gearboxes using power split the torque converter characteristics already takes the transmission ratio and mechanical losses into account. Hence, Vecto sets the ratio for the mechanical gear to 1 without additional losses.

The .vmod file for vehicles with AT gearboxes contains an additional column that indicates if the torque converter is locked or not.

### Gearshift losses for AT Gearboxes

For AT gearboxes the losses during a power-shift are modeled according to the following equations

#### Basic assumptions

+ Only power-shifts with positive power at gearbox output side are considered. 
+ Both upshifts and downshifts with positive power at gearbox output side have to be considered.
+ The power at gearbox output side is assumed to be constant during a power-shift


#### Power-shift loss computation

Model parameters: shift time ($t_s$), inertia factor ($f_I$)

![Engine speed, clutch speed during power-shift](pics/powershiftlosses.png)

$T_{PS,loss} = |T_{GBX,in}  * \Delta\omega_F| * t_s / dt$

$\Delta\omega_I = \omega_{engine,1} - \omega_{engine,2}$

$\Delta\omega_F = (\omega_{engine,1} - \omega_{engine,1^*}) / 2$
