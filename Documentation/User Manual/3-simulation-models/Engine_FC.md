## Engine: Fuel Consumption Calculation


The base FC value is interpolated from the stationary [FC map](#fuel-consumption-map-.vmap). If necessary the base value is corrected to compensate for unconsidered auxiliary energy consumption for vehicles with Start/Stop. In Declaration Mode [additional correction factors are applied](#engine-correction-factors).

The CO~2~ result for the actual mission profile is directly derived from the fuel consumption using a gravimetric [CO~2~/FC factor](#settings).


### Fuel Map Interpolation


The interpolation is based on [Delaunay Triangulation ![](pics/external-icon%2012x12.png)](http://en.wikipedia.org/wiki/Delaunay_triangulation) and works as follows:

1.  Triangulate the given rpm/torque/fuel points (= x,y,z)  to create a
    grid of triangles with each point of the map being part of at
    least one triangle.
2.  Find the triangle where the to-be-interpolated load point (x,y)
    is inside. If no triangle meets the criterion the calculation will
    be aborted.
3.  Calculate the z-value (= fuel) of the given x,y-point in the plane
    of the triangle

![](pics/FCmap.png)

*Delaunay Triangulation Example*



