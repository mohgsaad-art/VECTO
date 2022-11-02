## Engine File (.veng)

File for the definition of an engine in Vecto. Can be created with the [Engine Editor](#engine-editor).

- File format is [JSON](#json).
- Filetype ending is ".veng"

Refers to other files:

* [Full Load And Drag Curve (VFLD)](#full-load-and-drag-curves-.vfld)
* [Fuel Consumption (VMAP)](#fuel-consumption-map-.vmap)


**Example:**

~~~json
{
  "Header": {
    "CreatedBy": "",
    "Date": "2019-12-03T16:57:31.6048929Z",
    "AppVersion": "3",
    "FileVersion": 5
  },
  "Body": {
    "SavedInDeclMode": false,
    "ModelName": "325kW 12.7l Engine",
    "Displacement": "12740",
    "IdlingSpeed": 600.0,
    "Inertia": 5.1498,
    "Fuels": [
      {
        "WHTC-Urban": 0.0,
        "WHTC-Rural": 0.0,
        "WHTC-Motorway": 0.0,
        "WHTC-Engineering": 1.0,
        "ColdHotBalancingFactor": 0.0,
        "CFRegPer": 1.0,
        "FuelMap": "325kW_WHR.vmap",
        "FuelType": "EthanolPI"
      },
      {
        "WHTC-Urban": 1.0,
        "WHTC-Rural": 1.0,
        "WHTC-Motorway": 1.0,
        "WHTC-Engineering": 1.024,
        "ColdHotBalancingFactor": 1.0,
        "CFRegPer": 1.0,
        "FuelMap": "325kW_DF.vmap",
        "FuelType": "DieselCI"
      }
    ],
    "RatedPower": 0.0,
    "RatedSpeed": 0.0,
    "MaxTorque": 0.0,
    "FullLoadCurve": "325kW.vfld",
    "WHRType": [
      "ElectricalOutput"
    ],
    "WHRCorrectionFactors": {
      "Electrical": {
        "Urban": 0.0,
        "Rural": 0.0,
        "Motorway": 0.0,
        "ColdHotBalancingFactor": 0.0,
        "CFRegPer": 0.0,
        "EngineeringCorrectionFactor": 1.02
      }
    }
  }
}
~~~

