using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net.Sockets;
using NAudio.CoreAudioApi;
using NAudio.Wave;
using System.IO;
using System.Reflection;
using System.Drawing;
using System.Net;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace WinStreamer {
  public class SystemTrayForm : Form {
    private const float VolumeIncrement = 0.2f;

    private NotifyIcon trayIcon;
    private ContextMenu trayMenu;
    private Icon inactiveIcon, activeIcon;
    private MenuItem startStreaming, stopStreaming;
    private TcpClient tcpClient;
    private IWaveIn waveIn;
    private bool reset = false;
    private bool streaming = false;
    private KeyboardHook keyboardHook;
    private VolumeOverlay volumeOverlay;
    private WebClient webClient = new WebClient();

    private void UpdateTrayMenuAndIcon() {
      trayMenu.MenuItems.Remove(startStreaming);
      trayMenu.MenuItems.Remove(stopStreaming);
      if (streaming) {
        trayMenu.MenuItems.Add(0, stopStreaming);
        trayIcon.Icon = activeIcon;
      } else {
        trayMenu.MenuItems.Add(0, startStreaming);
        trayIcon.Icon = inactiveIcon;
      }
    }

    private Icon GetEmbeddedIcon(string name) {
      Assembly assembly = Assembly.GetExecutingAssembly();
      return new Icon(assembly.GetManifestResourceStream(string.Format("WinStreamer.{0}", name)));
    }

    public SystemTrayForm() {
      activeIcon = GetEmbeddedIcon("RecordEnabled.ico");
      inactiveIcon = GetEmbeddedIcon("RecordDisabled.ico");

      startStreaming = new MenuItem("Start streaming", OnStartStreaming);
      stopStreaming = new MenuItem("Stop streaming", delegate(object sender, EventArgs e) {
        EndStreaming();
      });

      trayMenu = new ContextMenu();
      trayMenu.MenuItems.Add("Exit", OnExit);

      trayIcon = new NotifyIcon();
      trayIcon.Text = "WinStreamer";

      UpdateTrayMenuAndIcon();

      trayIcon.ContextMenu = trayMenu;
      trayIcon.Visible = true;
    }

    private void OnKeyPressed(object sender, KeyPressedEventArgs e) { // XXX volume control doesn't work
      // Key presses are only for volume control
      string volumeData = Encoding.UTF8.GetString(webClient.DownloadData("http://192.168.1.112:8080/volume/get"));
      JObject volumeJson = JObject.Parse(volumeData);
      float currentVolume = (float)volumeJson["volume"];
      if (volumeOverlay == null) {
        volumeOverlay = new VolumeOverlay(currentVolume);
        volumeOverlay.FormClosed += delegate(object _sender, FormClosedEventArgs _e) {
          volumeOverlay = null;
        };
        volumeOverlay.Show();
      }

      // XXX these keys aren't right some how??
      switch (e.Key) {
      case Keys.VolumeUp:
        currentVolume += VolumeIncrement;
        break;
      case Keys.VolumeDown:
        currentVolume -= VolumeIncrement;
        break;
      }

      webClient.UploadData("http://192.168.1.112:8080/volume/set",
        Encoding.UTF8.GetBytes(string.Format("{{ \"volume\": {0} }}", currentVolume)));
      volumeOverlay.SetVolume(currentVolume);
    }

    private bool TryConnect() {
      try {
        tcpClient = new TcpClient();
        tcpClient.Connect("192.168.1.112", 3100);
        return true;
      } catch (Exception) {
        trayIcon.ShowBalloonTip(1000, "Connection Failed", "Failed to connect to remote host", ToolTipIcon.Error);
        return false;
      }
    }

    private void Disconnect() {
      if (tcpClient != null && tcpClient.Connected) {
        tcpClient.GetStream().Close();
        tcpClient.Close();
        tcpClient = null;
      }
    }

    private void RegisterHotkeys() {
      keyboardHook.RegisterHotKey(0, Keys.VolumeUp);
      keyboardHook.RegisterHotKey(0, Keys.VolumeDown);
      keyboardHook.RegisterHotKey(0, Keys.VolumeMute);
    }

    private void OnStartStreaming(object sender, EventArgs e) {
      if (TryConnect()) {
        streaming = true;
        reset = false;
        UpdateTrayMenuAndIcon();

        var deviceEnum = new MMDeviceEnumerator();
        var devices = deviceEnum.EnumerateAudioEndPoints(DataFlow.Render, DeviceState.Active).ToList();
        var selectedDevice = devices[0]; // Keep it simple and just use the first device

        waveIn = new WasapiLoopbackCapture((MMDevice)selectedDevice);
        waveIn.DataAvailable += OnDataAvailable;
        waveIn.StartRecording();

        /* XXX don't register keyboard hooks as volume control doesn't work
         * keyboardHook = new KeyboardHook();
         * keyboardHook.KeyPressed += OnKeyPressed;
         * RegisterHotkeys();
         */
      }
    }

    private void OnDataAvailable(object sender, WaveInEventArgs e) {
      if (!reset) {
        try {
          MemoryStream sendStream = new MemoryStream(e.BytesRecorded);
          for (int i = 0; i < e.BytesRecorded / 4; i++) {
            float sample = BitConverter.ToSingle(e.Buffer, i * 4);
            short sampleShort = (short)(sample * 32768);
            sendStream.Write(BitConverter.GetBytes(sampleShort), 0, 2);
          }
          tcpClient.GetStream().Write(sendStream.GetBuffer(), 0, (int)sendStream.Length);
        } catch (Exception) {
          reset = true;
          BeginInvoke((MethodInvoker)delegate {
            trayIcon.ShowBalloonTip(1000, "Disconnected", "Disconnected from remote host", ToolTipIcon.Info);
            EndStreaming();
          });
        }
      }
    }

    private void EndStreaming() {
      streaming = false;

      if (waveIn != null) {
        waveIn.StopRecording();
        waveIn.Dispose();
        waveIn = null;
      }

      if (keyboardHook != null) {
        keyboardHook.Dispose();
        keyboardHook = null;
      }

      if (tcpClient != null && tcpClient.Connected) {
        Disconnect();
      }

      UpdateTrayMenuAndIcon();
    }

    protected override void OnLoad(EventArgs e) {
      Visible = false;
      ShowInTaskbar = false;
      base.OnLoad(e);
    }

    private void OnExit(object sender, EventArgs e) {
      Application.Exit();
    }

    protected override void Dispose(bool disposing) {
      if (disposing) {
        EndStreaming();
        trayIcon.Dispose();
      }
      base.Dispose(disposing);
    }
  }
}
