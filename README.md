# VECTO: Vehicle Energy Consumption Calculation TOol

|                 |         |
| ---:            | :---    |
|       **home:** | https://climate.ec.europa.eu/eu-action/transport-emissions/road-transport-reducing-co2-emissions-vehicles/vehicle-energy-consumption-calculation-tool-vecto_en |
| **repository:** | https://code.europa.eu/vecto/vecto |
|   **keywords:** | CO2, fuelconsumption, energy, vehicle, automotive, HDV, simulation, engineering, EU, DG_CLIMA, JRC, IET, STU, policy |
|      **email**: | JRC-VECTO@ec.europa.eu |
|  **copyright:** | [2012-2022 European Commission, DG_CLIMA](https://climate.ec.europa.eu/index_en) <br> consider also other copyright mentions present in sub-components. |
|    **license:** | [EUPL 1.2+](https://joinup.ec.europa.eu/software/page/eupl) |

<img src="Documentation/User Manual/pics/VECTOlarge.png" alt="Vecto banner" height="120"/> <img src="Documentation/User Manual/pics/JRClogo.jpg" alt="JRC logo" height="120"/>

## What is VECTO?

VECTO is the offical vehicle simulator developed by the European Commission
to certify & monitoring energy demands, fuel consumption & CO<sub>2</sub> emissions
from Heavy Duty Vehicles (HDVs).

VECTO is a downloadable executable program written for .Net (C# & VBasic for the GUI)
designed to operate on a single computer.

The purpose of [VECTO's repository at code.europe.eu](https://code.europa.eu/vecto/vecto)
is to facilitate the communication between the Commission and VECTO's development team
with its users, to streamline support related issues contributing to the development,
review, and dissemination of VECTO. 

### EU Policy

From the 1st of January 2019, it is mandatory to certify CO<sub>2</sub> emissions
for all new truck vehicles of categories 4, 5, 9 and 10, as foreseen by regulation 2017/2400/EU
and [related legislation](http://ec.europa.eu/growth/sectors/automotive/environment-protection/emissions_en).  Data determined with VECTO along with other related parameters,
are [monitored and reported](https://climate.ec.europa.eu/eu-action/transport-emissions/road-transport-reducing-co2-emissions-vehicles/vehicle-energy-consumption-calculation-tool-vecto_en)
to the European Commission and made publicly available for each of those new trucks.


## Installation

VECTO is distributed as a portable application, downloaded from 
[**"Deployments" | "Releases releases"**](https://code.europa.eu/vecto/vecto/-/releases)
(or from the latest release-badges at the top of the landing page).
This means you can simply unzip the distributed archive and directly execute it.

### Requirements

- Microsoft Windows PC running Microsoft Windows 7 or later
- Microsoft .NET Framework 4.5

### Procedure

Installing VECTO requires the following two steps:

1. Copy the VECTO directory and all its files and subdirectories to the appropriate location
  where the user has *execute* permissions
  (e.g. inside `C:\Program Files` or ask your system administrator).

2. Edit the file `install.ini` and remove the comment character (`#`) in the line containing:

      `ExecutionMode = install`

  **Note:** If the `ExecutionMode` is set to `install`,
  it is necessary to copy the generic VECTO models distributed with VECTO
  to a location where you have *write* permissions, as VECTO writes results
  into the same directory of the input job file.


## Usage

Five different mission profiles for trucks and five different mission profiles for buses and coaches have been developed and implemented in the tool to better reflect the current European fleet.

The inputs for VECTO are characteristic parameters to determine the power consumption of every relevant vehicle component. Amongst others, the parameters for rolling resistance, air drag, masses and inertias, gearbox friction, auxiliary power and engine performance are input values to simulate energy consumption and CO2 emissions on standardised driving cycles.

Launch the `./User Manual/help.html` file in your browser (included in the distributed archive)
and read it for further usage instructions.

<img src="Documentation/User Manual/OriginalPictures/VECTO_MainForm.jpg" alt="Vecto main form" width="480"/>


## The [`trailer` branch](https://code.europa.eu/vecto/vecto/-/tree/trailer)

For a later regulation, the VECTO GUI [has been extended](https://ec.europa.eu/clima/system/files/2020-01/report_bodies_trailers_en.pdf) 
for bodies and trailers, where only the relevant data has to be entered
(total mass, RRC of the tyres and air drag changes compared to the standard body or trailer).
Also vehicle VIN, some technology information and certainly the manufacturer,
production date etc. can be entered to produce a VECTO result file complete for certification.
For bodies from rigid lorries, also the _Primary vehicle Information File_ (PIF)
has to be loaded to VECTO, which has to be provided by the manufacturer of the primary vehicle.
For trailers and semi-trailers, VECTO allocates the generic data automatically.

You may download the Generic Truck/Trailer 3D shapes for CFD simulations from
the [ECoGeT 3D shapes](https://code.europa.eu/vecto/vecto-cfd) project.


## Related vecto software

In the [`vecto` group](https://code.europa.eu/vecto) you may find additionally:

- **Vecto Airdrag:** certify $C_d·A$ values from constant speed tests
- **Vecto Engine:** certify engine declaration XML files as VECTO input
- **Vecto EEA hashing-tool:** (archived) monitoring-aid for European Environment Agency 
- **Vecto Git:** (unused) cirtify any pre-processing file