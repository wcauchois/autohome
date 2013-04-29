using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace WinStreamer {
  public partial class SettingsForm : Form {
    public SettingsForm() {
      InitializeComponent();
    }

    private void SettingsForm_Load(object sender, EventArgs e) {
      remoteHostTextBox.Text = Properties.Settings.Default.RemoteHost;
      relayPortTextBox.Text = Properties.Settings.Default.RelayPort.ToString();
      controlPortTextBox.Text = Properties.Settings.Default.ControlPort.ToString();
    }

    private void saveButton_Click(object sender, EventArgs e) {
      try {
        Properties.Settings.Default.RemoteHost = remoteHostTextBox.Text;
        Properties.Settings.Default.RelayPort = int.Parse(relayPortTextBox.Text);
        Properties.Settings.Default.ControlPort = int.Parse(controlPortTextBox.Text);
      } catch (Exception) {
        MessageBox.Show("Couldn't save settings");
      }
    }
  }
}
