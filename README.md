#RaspberryPiWakeUp

C# Tool to make a wake up (dimm LED and play music) with a Raspberry Pi

##Needed Tools
* WiringPI (https://projects.drogon.net/raspberry-pi/wiringpi/)
* mono-runtime (apt-get install mono-runtime)

##Config

* actions.xml

##Setting up USB-Audio

```scss
vi /etc/mpd.conf
```
uncomment

\#mixer_type                      "software"

```scss
vi /etc/modprobe.d/alsa-base.conf
```

set the index of "snd-usb-audio" to 0

options snd-usb-audio index=0
