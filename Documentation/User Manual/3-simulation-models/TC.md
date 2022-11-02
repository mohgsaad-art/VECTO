## Torque Converter Model

The torque converter is defined as (virtual) separate gear. Independent of the chosen AT gearbox type (serial or power split), Vecto uses a powertrain architecture with a serial torque converter. The mechanical gear ratios and gears with torque converter are created by Vecto depending on the gearbox type and gear configuration.

While the torque converter is active engine torque and speed are computed based on TC characteristic. 

 ![](pics/GBX-TC.svg)


### Torque converter characteristics file (.vtcc)

The file is described [here](#torque-converter-characteristics-.vtcc).

This file defines the torque converter characteristics as described in VDI 2153:

-   **Speed Ratio** ($\nu$) = Output Speed / Input Speed
-   **Torque Ratio** ($\mu$) = Output Torque / Input Torque
-   **Input Torque** ($T_{ref}(\nu)$) is the input torque (over ν) for a specific reference engine speed (see below).

The Input Torque at  reference engine speed is needed to calculate the actual engine torque using this formula:

$T_{in} = T_{ref}(\nu) \cdot ( \frac{n_{in}}{n_{ref}} )^{2}$

$\mu(\nu) = \frac{T_{out}}{T_{in}}$

with:

-   T~in~ = engine torque \[Nm\]
-   T~ref(ν)~ = reference torque at reference rpm (from .vtcc file) \[Nm\]
-   n~in~ = engine speed \[1/min\]
-   n~ref~ = reference rpm \[1/min\] (see below)

The torque converter characteristics must also be defined for speed ratios  greater than one (ν&gt;1) in order to calculate overrun conditions or engine drag (torque&lt;0).

**Note:** The torque converter characteristics must not contain parts where either the torque ratio or the input torque are constant!

<div class="declaration">
In declaration mode, the torque converter for drag points is automatically appended by VECTO. Input data with a speed ratio &geq; 1 are skipped.

For Power Split transmissions, where the torque converter characteristics already includes the gearbox losses and transmission ratio, the generic drag points are adapted according to the following equations:

$\nu_{PS} = \nu / ratio_i$

$\mu_{PS} = \mu \cdot ratio_i$
</div>

<div class="engineering">
In engineering mode the drag points for the torque converter can be specified. If so, the input data has to cover at least the speed ratio up to 2.2.

If the torque converter characteristics for drag are not specified, the generic points are appended as described above for declaration mode.
</div>

The torque converter has a separate [Shift Polygon](#shift-polygons-input-file-.vgbs) which defines the conditions for switching from torque converter gear to locked gear.


