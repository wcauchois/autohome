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
using System.Runtime.InteropServices;

namespace WinStreamer {
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
