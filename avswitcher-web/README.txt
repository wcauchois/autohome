Flask app to control an audio and HDMI switch using a USB-UIRT. Runs on a Raspberry Pi.

Adding it as a service:

 - Copy scripts/avswitcher into /etc/init.d
 - Run `sudo update-rc.d avswitcher defaults`
 - Run `sudo update-rc.d avswitcher enable`

To generate an IR config: irrecord -d /dev/ttyUSB0 -H uirt2_raw /tmp/my_remote

