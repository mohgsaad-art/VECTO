## Engine: Transient Full Load

The engine implements a PT1 behaviour to model transient torque build up:

$P_{fld\ dyn_{i}} = \frac{1}{T(n_{i})+1} \cdot \left(P_{fld\ stat}(n_{i})+T(n_{i}) \cdot P_{act_{i-1}}\right)$

with:

* n~i~ ... current engine speed
* T(n~i~) ... PT1 time constant at engine speed n~i~ (col. 4 in [.vfld file](#full-load-and-drag-curves-.vfld))
* P~fld\ stat~(n~i~) ... Static full load at engine speed n~i~ (col. 2 in [.vfld file](#full-load-and-drag-curves-.vfld))
* P~act\ i-1~ ... Engine power in previous time step


Vecto 3.x uses basically the same PT1 behavior to model transient torque build up. However, due to the dynamic time steps the formula is implemented as follows:

$P_{fld\ dyn_{i}} = P_{fld\ stat}(n_i) \cdot \left(1 - e^{-\frac{t_i^*}{\mathit{PT1}}}\right)$

where t* is computed from the dynamic full-load power in the previous simulation interval:

$t_i^* = t_{i-1}^* + dt$

$t_{i-1}^* = \mathit{PT1} \cdot ln\left(\frac{1}{1 - \frac{P_{eng_{i - 1}}}{P_{fld\ stat}(n_i)}}\right)$



