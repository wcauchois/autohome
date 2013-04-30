autohome
===

So I bought a Raspberry Pi... and I'm moving into a new apartment. I thought I might as well make some aspects of my life easier.

Currently, this software can stream music from my Windows computer to my Raspberry Pi (and thus my speaker system).

I also bought a USB-UIRT (universal IR transmitter/receiver), which I've managed to get to control my Apple TV from the command-line, so that might get incorporated some how.

Components
---

- WinStreamer: Runs in the system tray and allows you to send audio output from your Windows computer to an arbitrary socket (i.e. the RasPi relay).
- control-server: Will eventually let you control things like volume via a REST API. Also hosts a mobile web interface (jQuery Mobile).
- parelay: A simple bit of C that pipes input from a stream (TCP) socket to a PulseAudio device.
