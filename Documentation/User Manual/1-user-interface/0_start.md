
Platform Requirements
=====================

Hardware Requirements

   + Microsoft Windows PC running Microsoft Windows 7 or later

Software Requirements

   + Microsoft .NET Framework 4.5

## Installation Options

VECTO is distributed as a portable application. This means you can simply unzip the archive and directly start VECTO.exe. This, however, requires write and execute permissions for the VECTO application directory.

In case you do not have execute permissions, please ask your system administrator to install VECTO into an appropriate directory (e.g. under `C:\Program Files`). Installing VECTO requires the following two steps:

  + Copy the VECTO directory and all its files and subdirectories to the appropriate location where the user has execute permissions
  + Edit the file `install.ini` and remove the comment character (#) in the line containing `ExecutionMode = install` 

If the ExecutionMode is set to `install` (this is also possible when running VECTO from an arbitrary directory), VECTO does not write its configuration files and log files to the application directory but to the directories `%APPDATA%` and `%LOCALAPPDATA%` (usually `C:\Users\<username>\AppData\Roaming` and `C:\Users\<username>\AppData\Local`).

**Important:** If VECTO is run from a directory without write permissions it is necessary that you copy the generic VECTO models distributed with VECTO to a location where you have write permissions or set the output path to a directory with write permissions (see the [Options in the main window](#main-form)).


User Manual
====================================
![](pics/VECTOlarge.png)\
\
Version: VECTO 3.3 / VectoCore 3.3.2 / VectoCmd 3.3.2

---

VECTO is a tool for the calculation of energy consumption and CO~2~ emissions of vehicles. It models the components of a heavy-duty vehicle and simulates a virtual drive on a route. The goal is to provide a standardized way of calculating the energy consumption (fuel consumption) and corresponding CO~2~ emissions.


This User Manual consists of 4 Parts:

- [Graphical User Interface](#user-interface):
    : Describes the graphical user interface of VECTO with all windows and configuration possibilities.
- [Calculation Modes](#calculation-modes):
    : Describes the calculation modes of VECTO (Declaration, Engineering, ...). 
- [Simulation Models](#simulation-models):
    : This chapter describes the used component models and formulas which are implemented in the software. 
- [Input and Output](#input-and-output):
    : Describes the input and output file formats.

This user manual describes verson 3.3.x of Vecto. 


