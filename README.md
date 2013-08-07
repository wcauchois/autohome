Components
---

- WinStreamer: Runs in the system tray and allows you to send audio output from your Windows computer to an arbitrary socket (i.e. the RasPi relay).
- control-server: Will eventually let you control things like volume via a REST API. Also hosts a mobile web interface (jQuery Mobile).
- parelay: A simple bit of C that pipes input from a stream (TCP) socket to a PulseAudio device.
