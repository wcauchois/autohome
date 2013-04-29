namespace WinStreamer {
  partial class SettingsForm {
    /// <summary>
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing) {
      if (disposing && (components != null)) {
        components.Dispose();
      }
      base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent() {
      this.label1 = new System.Windows.Forms.Label();
      this.remoteHostTextBox = new System.Windows.Forms.TextBox();
      this.label2 = new System.Windows.Forms.Label();
      this.relayPortTextBox = new System.Windows.Forms.TextBox();
      this.label3 = new System.Windows.Forms.Label();
      this.controlPortTextBox = new System.Windows.Forms.TextBox();
      this.saveButton = new System.Windows.Forms.Button();
      this.cancelButton = new System.Windows.Forms.Button();
      this.SuspendLayout();
      // 
      // label1
      // 
      this.label1.AutoSize = true;
      this.label1.Location = new System.Drawing.Point(12, 15);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(67, 13);
      this.label1.TabIndex = 0;
      this.label1.Text = "Remote host";
      // 
      // remoteHostTextBox
      // 
      this.remoteHostTextBox.Location = new System.Drawing.Point(85, 12);
      this.remoteHostTextBox.Name = "remoteHostTextBox";
      this.remoteHostTextBox.Size = new System.Drawing.Size(131, 20);
      this.remoteHostTextBox.TabIndex = 1;
      // 
      // label2
      // 
      this.label2.AutoSize = true;
      this.label2.Location = new System.Drawing.Point(12, 41);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(55, 13);
      this.label2.TabIndex = 2;
      this.label2.Text = "Relay port";
      // 
      // relayPortTextBox
      // 
      this.relayPortTextBox.Location = new System.Drawing.Point(85, 38);
      this.relayPortTextBox.Name = "relayPortTextBox";
      this.relayPortTextBox.Size = new System.Drawing.Size(81, 20);
      this.relayPortTextBox.TabIndex = 3;
      // 
      // label3
      // 
      this.label3.AutoSize = true;
      this.label3.Location = new System.Drawing.Point(12, 67);
      this.label3.Name = "label3";
      this.label3.Size = new System.Drawing.Size(61, 13);
      this.label3.TabIndex = 4;
      this.label3.Text = "Control port";
      // 
      // controlPortTextBox
      // 
      this.controlPortTextBox.Location = new System.Drawing.Point(85, 64);
      this.controlPortTextBox.Name = "controlPortTextBox";
      this.controlPortTextBox.Size = new System.Drawing.Size(81, 20);
      this.controlPortTextBox.TabIndex = 5;
      // 
      // saveButton
      // 
      this.saveButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.saveButton.Location = new System.Drawing.Point(60, 119);
      this.saveButton.Name = "saveButton";
      this.saveButton.Size = new System.Drawing.Size(75, 23);
      this.saveButton.TabIndex = 6;
      this.saveButton.Text = "Save";
      this.saveButton.UseVisualStyleBackColor = true;
      this.saveButton.Click += new System.EventHandler(this.saveButton_Click);
      // 
      // cancelButton
      // 
      this.cancelButton.Location = new System.Drawing.Point(141, 119);
      this.cancelButton.Name = "cancelButton";
      this.cancelButton.Size = new System.Drawing.Size(75, 23);
      this.cancelButton.TabIndex = 7;
      this.cancelButton.Text = "Cancel";
      this.cancelButton.UseVisualStyleBackColor = true;
      // 
      // SettingsForm
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(228, 154);
      this.Controls.Add(this.cancelButton);
      this.Controls.Add(this.saveButton);
      this.Controls.Add(this.controlPortTextBox);
      this.Controls.Add(this.label3);
      this.Controls.Add(this.relayPortTextBox);
      this.Controls.Add(this.label2);
      this.Controls.Add(this.remoteHostTextBox);
      this.Controls.Add(this.label1);
      this.Name = "SettingsForm";
      this.Text = "Settings";
      this.Load += new System.EventHandler(this.SettingsForm_Load);
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.TextBox remoteHostTextBox;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.TextBox relayPortTextBox;
    private System.Windows.Forms.Label label3;
    private System.Windows.Forms.TextBox controlPortTextBox;
    private System.Windows.Forms.Button saveButton;
    private System.Windows.Forms.Button cancelButton;
  }
}