## VTP-Job File

File for the definition of a verification test job in vecto. A job contains everything what is needed to run a simulation. Can be created with the [Verifcation Test Job Editor](#vtp-job-editor).

- File format is [JSON](#json).
- Filetype ending is ".vecto"

Refers to other files:

* [Vehicle (XML)](#vehicle-file-.vveh)
* [Gearbox (VGBX)](#gearbox-file-.vgbx)
* [Driving Cycle (VDRI)](#driving-cycles-.vdri)



**Example:**

~~~json
{
  "Header": {
    "CreatedBy": "VECTO 3.3.11",
    "Date": "2022-09-12T13:30:50.8149043Z",
    "AppVersion": "3",
    "FileVersion": 4
  },
  "Body": {
    "SavedInDeclMode": true,
    "DeclarationVehicle": "vehicle_sampleSingleModeDualFuel.xml",
    "ManufacturerRecord": "vehicle_sampleSingleModeDualFuel.RSLT_MANUFACTURER.xml",
    "Mileage": 30000.0,
    "FanPowerCoefficients": [
      7.32,
      1200.0,
      810.0
    ],
    "FanDiameter": 0.3,
    "FuelNCVs": [
      {
        "Type": "Diesel CI",
        "NCV": 42.7
      },
      {
        "Type": "NG CI",
        "NCV": 48.0
      }
    ],
    "TorqueDriftLeftWheel": 0.0,
    "TorqueDriftRightWheel": 0.0,
    "Cycles": [
      "vtp_cycle_2Hz.vdri"
    ]
  }
}
~~~

