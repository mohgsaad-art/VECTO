## Gearbox File (.vgbx)

File for the definition of a gearbox in Vecto. Can be created with the [Gearbox Editor](#gearbox-editor).

- File format is [JSON](#json).
- Filetype ending is ".vgbx"

Refers to other files:

* [Shift Polygon (VGBS)](#shift-polygons-input-file-.vgbs)
* [Loss Map (VTLM)](#transmission-loss-map-.vtlm)
* [Torque Converter (VTCC)](#torque-converter-characteristics-.vtcc)


**Example:**

~~~json
{
  "Header": {
    "CreatedBy": "Michael Krisper (Graz University of Technology)",
    "Date": "2016-03-18T14:37:18+01:00",
    "AppVersion": "3.0.2",
    "FileVersion": 5
  },
  "Body": {
    "SavedInDeclMode": false,
    "ModelName": "Generic 8 Gears",
    "Inertia": 0.0,
    "TracInt": 1.0,
    "Gears": [
      {
        "Ratio": 3.2,
        "LossMap": "Axle.vtlm"
      },
      {
        "Ratio": 6.4,
        "LossMap": "Indirect Gear.vtlm",
        "TCactive": false,
        "ShiftPolygon": "ShiftPolygon.vgbs",
        "FullLoadCurve": "<NOFILE>"
      },
      {
        "Ratio": 4.6,
        "LossMap": "Indirect Gear.vtlm",
        "TCactive": false,
        "ShiftPolygon": "ShiftPolygon.vgbs",
        "FullLoadCurve": "<NOFILE>"
      },
      {
        "Ratio": 3.4,
        "LossMap": "Indirect Gear.vtlm",
        "TCactive": false,
        "ShiftPolygon": "ShiftPolygon.vgbs",
        "FullLoadCurve": "<NOFILE>"
      },
      {
        "Ratio": 2.6,
        "LossMap": "Indirect Gear.vtlm",
        "TCactive": false,
        "ShiftPolygon": "ShiftPolygon.vgbs",
        "FullLoadCurve": "<NOFILE>"
      },
      {
        "Ratio": 1.9,
        "LossMap": "Indirect Gear.vtlm",
        "TCactive": false,
        "ShiftPolygon": "ShiftPolygon.vgbs",
        "FullLoadCurve": "<NOFILE>"
      },
      {
        "Ratio": 1.3,
        "LossMap": "Indirect Gear.vtlm",
        "TCactive": false,
        "ShiftPolygon": "ShiftPolygon.vgbs",
        "FullLoadCurve": "<NOFILE>"
      },
      {
        "Ratio": 1,
        "LossMap": "Direct Gear.vtlm",
        "TCactive": false,
        "ShiftPolygon": "ShiftPolygon.vgbs",
        "FullLoadCurve": "<NOFILE>"
      },
      {
        "Ratio": 0.75,
        "LossMap": "Indirect Gear.vtlm",
        "TCactive": false,
        "ShiftPolygon": "ShiftPolygon.vgbs",
        "FullLoadCurve": "<NOFILE>"
      }
    ],
    "TqReserve": 20.0,
    "SkipGears": true,
    "ShiftTime": 2,
    "EaryShiftUp": true,
    "StartTqReserve": 20.0,
    "StartSpeed": 2.0,
    "StartAcc": 0.6,
    "GearboxType": "AMT",
    "TorqueConverter": {
      "Enabled": false,
      "File": "<NOFILE>",
      "RefRPM": 0.0,
      "Inertia": 0.0
    }
    "DownshiftAferUpshiftDelay": 10.0,
    "UpshiftAfterDownshiftDelay": 10.0,
    "UpshiftMinAcceleration": 0.1
  }
}
~~~

