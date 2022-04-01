## Auxiliary Dialog



<div class="declaration">
![Auxiliary Dialog (Declaration Mode)](pics/VECTO-Editor_Aux_DECL.jpg)
</div>



<div class="engineering">
![Auxiliary Dialog (Engineering Mode)](pics/VECTO-Editor_Aux_ENG.jpg)
</div>

### Description


The Auxiliary Dialog is used to configure auxiliaries. In [Declaration Mode](#declaration-mode) the set of auxiliaries and their power demand is pre-defined. For every auxiliary the user has to select the technology from a given list. In [Engineering Mode](#engineering-mode) the set of auxiliaries can be specified by the user. Auxiliary efficieny is defined using an [Auxiliary Input File (.vaux)](#auxiliary-input-file-.vaux). See [Auxiliaries](#auxiliaries) for details on how the power demand for each auxiliary is calculated.

### Settings

<div class="declaration">
Technology
:   List of available technology for the auxiliary type
For the  steering pump multiple technologies can be defined, one for each steered axle.
</div>

<div class="engineering">
Type
:	String defining type of auxiliary. Click the arrow to load from a predefined list, however It is not required to use a type from the list.

ID
:	The ID string is required to link the auxiliary to the corresponding supply power definition in the driving cycle. The ID must contain characters  and numbers only (A-Z, a-z, 0-9). The ID is not case sensitive (e.g. "ALT" will link to "Alt" or "alt", etc.)
***Example*** *: Auxiliary "ALT" is linked to the column "&lt;Aux\_ALT&gt;" in the driving cylce.*
See [Auxiliaries](#auxiliaries) for details.

Input File
:	Path to the [Auxiliary File (.vaux)](#auxiliary-input-file-.vaux).
</div>



### Controls


![ok](pics/OK.png) ***Save and close***

![cancel](pics/Cancel.png) ***Close without saving***
