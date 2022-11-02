## Gearshift Parameters File (.vtcu)


**Empty .vtcu File - default values are used**

~~~
{
  "Header": {
    "CreatedBy": " ()",
    "Date": "2016-10-13T15:52:04.0766564Z",
    "AppVersion": "3",
    "FileVersion": 1
  },
  "Body": {
	 
      }
}
~~~


**Example .vtcu file**

~~~
{
  "Header": {
    "CreatedBy": " ()",
    "Date": "2016-10-13T15:52:04.0766564Z",
    "AppVersion": "3",
    "FileVersion": 1
  },
  "Body": {
	 
    "Rating_current_gear": 0.97,
	
	"RatioEarlyUpshiftFC": 24,
	"RatioEarlyDownshiftFC": 24,    
	"VelocityDropFactor": 1.0,
	
	"ShiftTime": 0.7,
	"AccelerationFactorNP98h":0.8,
	"VelocityDropFactor": 1,
	"ATLookAheadTime": 0.8,
	
	"LoadStageThresoldsUp": "19.7;36.34;53.01;69.68;86.35",
    "LoadStageThresoldsDown": "13.7;30.34;47.01;63.68;80.35",
	"ShiftSpeedsTCLockup" : [
		[ 50, 80, 125, 50, 80, 125 ],
		[ 50, 80, 125, 50, 80, 125 ],
		[ 50, 80, 125, 50, 80, 125 ],
		[ 50, 80, 125, 70, 100, 145 ],
		[ 60, 90, 135, 80, 110, 155 ],
		[ 70, 100, 145, 90, 120, 155 ]
	]
  }
}
~~~