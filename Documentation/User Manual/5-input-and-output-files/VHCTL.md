## Hybrid Strategy Parameters File (.vhctl)

File for the definition of the hybrid control strategy parameters in VECTO. Can be created with the [Hybrid Strategy Parameters Editor](#hybrid-strategy-parameters-editor).

- File format is [JSON](#json).
- Filetype ending is ".vhctl"

**Example:**

~~~
{
  "Header": {
    "CreatedBy": "",
    "Date": "2020-09-07T15:28:08.3781385Z",
    "AppVersion": "3",
    "FileVersion": 1
  },
  "Body": {
    "EquivalenceFactor": 2.0,
    "MinSoC": 20.0,
    "MaxSoC": 80.0,
    "TargetSoC": 50.0,
    "AuxBufferTime": 5.0,
    "AuxBufferChgTime": 5.0,
    "MinICEOnTime": 10.0
  }
}
~~~
