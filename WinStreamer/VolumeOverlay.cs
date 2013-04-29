using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Timers;

namespace WinStreamer {
  public class VolumeOverlay : Form {
    [DllImport("user32.dll")]
    private static extern uint SendMessage(IntPtr hwnd, uint msg, uint wParam, uint lParam);
    [DllImport("user32.dll")]
    private static extern bool SetWindowPos(IntPtr hwnd, IntPtr hwndInsertAfter, int x, int y, int cs, int cy, uint uFlags);

    private const int HWND_TOPMOST = -1;

    private int ExpirationTime = 1000;
    private ProgressBar progressBar;
    private System.Timers.Timer expirationTimer;

    public VolumeOverlay(float volume) {
      Width = 800;
      Height = 30;
      FormBorderStyle = FormBorderStyle.None;
      CenterToScreen();
      SetWindowPos(Handle,
        (IntPtr)(-1), // HWND_TOPMOST
        0, 0, 0, 0,
        0x0002 /* SWP_NOMOVE */ | 0x0001 /* SWP_NOSIZE */);

      progressBar = new ProgressBar();
      progressBar.Dock = DockStyle.Fill;
      SetVolume(volume);
      Controls.Add(progressBar);
    }

    private void ResetExpirationTimer() {
      if (expirationTimer != null) {
        expirationTimer.Enabled = false;
        expirationTimer.Close();
      }
      expirationTimer = new System.Timers.Timer() {
        Interval = ExpirationTime,
        AutoReset = false
      };
      expirationTimer.Elapsed += delegate(object sender, ElapsedEventArgs e) {
        BeginInvoke((MethodInvoker)delegate() {
          Close();
        });
      };
      expirationTimer.Enabled = true;
    }

    public void SetVolume(float volume) {
      // We have to set the progress bar to PAUSED and then NORMAL so
      // we don't get a progress bar animation.
      SendMessage(progressBar.Handle,
        0x400 + 16, // WM_USER + PBM_SETSTATE
        0x0002, // PBST_PAUSED
        0);
      progressBar.Value = (int)(volume * 100.0f);
      SendMessage(progressBar.Handle,
        0x400 + 16, // WM_USER + PBM_SETSTATE
        0x0001, // PBST_NORMAL
        0);

      ResetExpirationTimer();
    }
  }
}
