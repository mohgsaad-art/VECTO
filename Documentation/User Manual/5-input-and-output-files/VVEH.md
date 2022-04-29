## Vehicle File (.vveh)

File for the definition of a vehicle in vecto. Can be created with the [Vehicle Editor](#vehicle-editor).

- File format is [JSON](#json).
- Filetype ending is ".vveh"

Refers to other files:

* [Cross Wind Correction (VCDV, VCDB)](#vehicle-cross-wind-correction)
* [Retarder Loss Map (VRLM)](#retarder-loss-torque-input-file-.vrlm)
* [Transmission Loss Map (for Angular Gear) (VTLM)](#transmission-loss-map-.vtlm)

**Example:**

~~~json
{
  "Header": {
    "CreatedBy": "Michael Krisper (Graz University of Technology)",
    "Date": "2016-03-18T14:42:45+01:00",
    "AppVersion": "3.0.2",
    "FileVersion": 7
  },
  "Body": {
    "SavedInDeclMode": false,
    "VehCat": "RigidTruck",
    "CurbWeight": 6000.0,
    "CurbWeightExtra": 0.0,
    "Loading": 0.0,
    "MassMax": 11.9,
    "CdA": 4.5,
    "rdyn": 450,
    "Rim": "15° DC Rims",
    "CdCorrMode": "CdOfVeng",
    "CdCorrFile": "CrossWindCorrection.vcdv",
    "Retarder": {
      "Type": "Secondary",
      "Ratio": 1.0,
      "File": "Retarder.vrlm"
    },
    "AngularGear": {
      "Type" : "SeparateAngularGear",
      "Ratio": 1.0,
      "LossMap": "AngularGear.vtlm"
    },
    "AxleConfig": {
      "Type": "4x2",
      "Axles": [
        {
          "Inertia": 6.0,
          "Wheels": "245/70 R19.5",
          "AxleWeightShare": 0.0,
          "TwinTyres": false,
          "RRCISO": 0.008343465,
          "FzISO": 20800.0
        },
        {
          "Inertia": 6.0,
          "Wheels": "245/70 R19.5",
          "AxleWeightShare": 0.0,
          "TwinTyres": true,
          "RRCISO": 0.00943769,
          "FzISO": 20800.0
        }
      ]
    }
  }
}
~~~

