using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Net.Sockets;
using System.Timers;
using NAudio.CoreAudioApi;
using NAudio.Wave;
using System.Threading;

namespace WinBroadcast {
  public class SystemTrayForm : Form {
    private NotifyIcon trayIcon;
    private ContextMenu trayMenu;
    private Icon inactiveIcon, activeIcon;
    private MenuItem startBroadcasting, stopBroadcasting;
    private TcpClient tcpClient;
    private IWaveIn waveIn;
    private bool reset = false;
    private bool broadcasting = false;

    private void UpdateTrayMenuAndIcon() {
      trayMenu.MenuItems.Remove(startBroadcasting);
      trayMenu.MenuItems.Remove(stopBroadcasting);
      if (broadcasting) {
        trayMenu.MenuItems.Add(0, stopBroadcasting);
        trayIcon.Icon = activeIcon;
      } else {
        trayMenu.MenuItems.Add(0, startBroadcasting);
        trayIcon.Icon = inactiveIcon;
      }
    }

    private Icon GetEmbeddedIcon(string name) {
      Assembly assembly = Assembly.GetExecutingAssembly();
      return new Icon(assembly.GetManifestResourceStream(string.Format("WinBroadcast.{0}", name)));
    }

    public SystemTrayForm() {
      activeIcon = GetEmbeddedIcon("RecordEnabled.ico");
      inactiveIcon = GetEmbeddedIcon("RecordDisabled.ico");

      startBroadcasting = new MenuItem("Start broadcasting", OnStartBroadcasting);
      stopBroadcasting = new MenuItem("Stop broadcasting", delegate(object sender, EventArgs e) {
        EndBroadcasting();
      });

      trayMenu = new ContextMenu();
      trayMenu.MenuItems.Add("Exit", OnExit);

      trayIcon = new NotifyIcon();
      trayIcon.Text = "AutoHome Broadcaster";

      UpdateTrayMenuAndIcon();

      trayIcon.ContextMenu = trayMenu;
      trayIcon.Visible = true;
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

    private void OnStartBroadcasting(object sender, EventArgs e) {
      if (TryConnect()) {
        broadcasting = true;
        reset = false;
        UpdateTrayMenuAndIcon();

        var deviceEnum = new MMDeviceEnumerator();
        var devices = deviceEnum.EnumerateAudioEndPoints(DataFlow.Render, DeviceState.Active).ToList();
        var selectedDevice = devices[0]; // Keep it simple and just use the first device

        waveIn = new WasapiLoopbackCapture((MMDevice)selectedDevice);
        waveIn.DataAvailable += OnDataAvailable;
        waveIn.StartRecording();
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
            EndBroadcasting();
          });
        }
      }
    }

    private void EndBroadcasting() {
      broadcasting = false;

      if (waveIn != null) {
        waveIn.StopRecording();
        waveIn.Dispose();
        waveIn = null;
      }

      if (tcpClient.Connected) {
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
        EndBroadcasting();
        trayIcon.Dispose();
      }
      base.Dispose(disposing);
    }
  }
  static class Program {
    /// <summary>
    /// The main entry point for the application.
    /// </summary>
    [STAThread]
    static void Main() {
      var alreadyLaunched = System.Diagnostics.Process.GetProcessesByName(
        System.IO.Path.GetFileNameWithoutExtension(
        System.Reflection.Assembly.GetEntryAssembly().Location)).Count() > 1;
      if (!alreadyLaunched) {
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);
        Application.Run(new SystemTrayForm());
      }
    }
  }
}
