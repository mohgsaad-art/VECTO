## Command Line Arguments

![](pics/cmd3.png)

The Vecto 3.x commandline tool can be used to start simulations from the command line and  runs without  graphical user interface. If multiple job-files are specified or a job-file contains multiple simulation runs (i.e., multiple cycles and/or loadings) these simulations are executed in parallel.

### General Notes

-   The order in which the arguments are provided is arbitrary.
-   If a file path includes space characters (e.g. "C:\\VECTO Test Files\\Demo.vecto") then double quotes have to be used (as in the picture above).
-   If not the complete file path is defined (e.g. "file1.vecto" instead of "c:\\data\\file1.vecto") then VECTO expects the file in the application directory (where vectocmd.exe is located).

### Basic usage 

        vectocmd.exe [-h] [-v] FILE1.(vecto|xml) [FILE2.(vecto|xml) ...]

### List of command line arguments

- FILE1.vecto [FILE2.vecto ...]: A list of vecto-job files (with the 
       extension: .vecto). At least one file must be given. Delimited by 
       whitespace.
- -t: output information about execution times
- -mod: write mod-data in addition to sum-data
- -eng: switch to engineering mode (implies -mod)
- -v: Shows verbose information (errors and warnings will be displayed)
- -vv: Shows more verbose information (infos will be displayed)
- -vvv: Shows debug messages (slow!)
- -vvvv: Shows all verbose information (everything, slow!)
- -V: show version information
- -h: Displays this help.

