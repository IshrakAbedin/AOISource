# AOI
AOI is an application that auto calculates a daily routine given a set of tasks or works and recreational activities. Having MiniZinc solver in heart, the main idea is that works will have priority and consume energy while recreational activities restores energy. Given a starting energy. the schedular will make a schedule in such a way that at any moment of time, energy never goes below zero. It will always fit all the works and every recreational activities at least once or it will fail at making the schedule. Python helps to format the input of MiniZinc and auto formats every input.

USING PYTHON 'bridge.py'
--------------------------------------------------------------------
bridge.py serves as the driver for Routine.mzn, which is the real solver script. Bridge converts all the inputs to integers and the passes it to Routine.mzn which returns the routine as an array of integers. Time is divided into quanta. Quanta is in minutes and taking smaller quanta will make the data more granular while making the computation more expense. So choosing a proper quanta is necessary for maintaining balance between time and granularity. Python auto formats input times and energies based on quanta.

**MiniZinc with CoinBC or CBC solver is required and must be added to path. For *Windows* users, the main minizinc.exe must be run once for the application to work even after adding to path. I do not know why this happens. ;p**

**PYTHON version used**: 3.7

**REQUIRED PYTHON LIBRARIES**:
pymzn, prettytable

**HOW TO USE**:
There are two ways of running the Python bridge. First is running bridge.py directly and then entering command. Second is writing all the commands in a text file and then passing that text file as command line argument to bridge.py. For example,

`$python bridge.py input.txt`

**COMMAND LIST**:

For adding work, `work/wrk workName totalWorkEnergy workMinimumDuration workMaximumDuration priority`

For adding recreational activity, `recr/rec recreationName perQuantaEnergyRestoration`

For adding starting energy, `nrg/ener/energy/enrg energyAmount`

For adding quanta, `quanta/qnta quantaAmount`

For adding starting time, `stime tt:ttam/pm`

For adding ending time, `etime tt:ttam/pm`

To start the calculation, `run`

To reset every input, `clear`

**EXAMPLE COMMAND**
```
energy 80
stime 06:00pm
etime 12:00am
quanta 15
work cleaning 48 60 75 4
work study 25 75 105 5
work repair 51 45 45 2
work cooking 20 30 30 3
work project 30 15 45 2
recr singing 30
recr procastinating 45
run
```
*Do not use space between task names or other input*

USING DOTNET GUI APPLICATION
----------------------------------------------------------------------
The DOTNET GUI application takes the same input but through a GUI form and then passes it to bridge.py. However, the application can format the inputs itself, show the minimum required time and let review and modify tasks. For running in *Windows*, a precompiled version of the application can be found at https://github.com/IshrakAbedin/AOIArtifact.

**GUI is not up to date with the latest python code and solver with Min/Max work duration**