## Integrated Hybrid Electric Powertrain Component (IHPC)

Integrated hybrid electric powertrain component (IEPC) means a combined system of multiple electric machine systems together with the functionality of a multi-speed gearbox. An electric machine of type IHPC has to be used together with a gearbox of type IHPC.

The IEPC is modeled by the following parameters and map files:

- Maximum drive torque over rotational speed (related to the output shaft)
- Maximum generation torque over rotational speed (Related to the output shaft)
- Drag curve (shall be 0 as the drag losses are covered by the gearbox model)
- Electric power map for all mechanical gears
- Continuous torque and rotational speed (output shaft), measured with the gear with the ratio closest to 1
- Overload torque and rotational speed (output shaft), measured with the gear with the ratio closest to 1
- Maximum overload time

The first two curves are read from a .vemp file (see [Electric Motor Max Torque File (.vemp)](#electric-motor-max-torque-file-.vemp)). The drag curve(s) are provided in .viepcd file(s) (see [IEPC Drag Curve File (.viepcd)](#electric-motor-drag-curve-file-.vemd)) and the electric power maps in .viepco file(s) (see [IEPC Power Map (.viepco)](#electric-motor-power-map-.vemo)). 

In the VECTO simulation, the IHPC component is similar to an electric motor, except the electric power maps are gear dependent.

