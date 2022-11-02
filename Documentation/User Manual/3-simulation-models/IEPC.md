## Integrated Electric Powertrain Component (IEPC)

Integrated electric powertrain component (IEPC) means a combined system of an electric machine system together with the funcitonality of either a single- or multi-speed gearbox or a differential or both. 

An IEPC can be of design-type wheel motor which means that the output shaft (or two output shafts) are directly connected to the wheel hub(s).

The IEPC is modeled by the following parameters and map files:

- Maximum drive torque over rotational speed (related to the output shaft)
- Maximum generation torque over rotational speed (Related to the output shaft)
- Gear ratios of all gears (electric motor to out shaft)
- Drag curve. either for a single gear (measured with the gear with the ratio closest to 1) or all gears
- Electric power map for all mechanical gears
- Continuous torque and rotational speed (output shaft), measured with the gear with the ratio closest to 1
- Overload torque and rotational speed (output shaft), measured with the gear with the ratio closest to 1
- Maximum overload time

The first two curves are read from a .viepcp file (see [IEPC Max Torque File (.viepcp)](#iepc-max-torque-file-.viepcp)). The drag curve(s) are provided in .viepcd file(s) (see [IEPC Drag Curve File (.viepcd)](#iepc-drag-curve-file-.viepcd)) and the electric power maps in .viepco file(s) (see [IEPC Power Map (.viepco)](#iepc-power-map-.viepco)). It is important to note that for the IEPC all maps are related to the output shaft speed (including all integrated components of the IEPC).

In the VECTO simulation, the IEPC component is virtually split up into the electric machine (with gear-dependent electric power maps), an APT-N gearbox in case of a multi-speed gearbox or a single-speed gearbox in case the IEPC has only a single fixed transmission ratio, and optionally an axle gear. All virtual powertrain components (gearbox, axlegear) are modeled as loss-less components. Thus, the simulation of an IEPC is similar to E2 vehicles in case of a multi-speed gearbox or an E3 vehicle in case of a single-speed gearbox.

![](pics/Structure_IEPC.png)

All signals with the suffix "_int" refer to the electric motor, while signals without this suffix refer to the whole component.