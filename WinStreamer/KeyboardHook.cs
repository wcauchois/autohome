using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace WinStreamer {
  // http://www.liensberger.it/web/blog/?p=207
  public class KeyboardHook : IDisposable {
    [DllImport("user32.dll")]
    private static extern bool RegisterHotKey(IntPtr hwnd, int id, uint modifiers, uint vk);
    [DllImport("user32.dll")]
    private static extern bool UnregisterHotKey(IntPtr hwnd, int id);

    private class Window : NativeWindow, IDisposable {
      private static int WM_HOTKEY = 0x0312;
      public Window() {
        CreateHandle(new CreateParams());
      }
      protected override void WndProc(ref Message m) {
        base.WndProc(ref m);

        if (m.Msg == WM_HOTKEY) {
          Keys key = (Keys)(((int)m.LParam >> 16) & 0xFFFF);
          ModifierKeys modifier = (ModifierKeys)((int)m.LParam & 0xFFFF);
          if (KeyPressed != null) {
            KeyPressed(this, new KeyPressedEventArgs(modifier, key));
          }
        }
      }
      public event EventHandler<KeyPressedEventArgs> KeyPressed;
      public void Dispose() {
        this.DestroyHandle();
      }
    }

    private Window window = new Window();
    private int currentId;

    public KeyboardHook() {
      window.KeyPressed += delegate(object sender, KeyPressedEventArgs e) {
        if (KeyPressed != null) {
          KeyPressed(this, e);
        }
      };
    }

    public void RegisterHotKey(ModifierKeys modifier, Keys key) {
      currentId += 1;
      if (!RegisterHotKey(window.Handle, currentId, (uint)modifier, (uint)key)) {
        throw new InvalidOperationException("Couldn't register hot key");
      }
    }

    public event EventHandler<KeyPressedEventArgs> KeyPressed;

    public void Dispose() {
      for (int i = currentId; i > 0; i--) {
        UnregisterHotKey(window.Handle, i);
      }
      window.Dispose();
    }
  }

  public class KeyPressedEventArgs : EventArgs {
    private ModifierKeys modifier;
    private Keys key;
    public KeyPressedEventArgs(ModifierKeys modifier, Keys key) {
      this.modifier = modifier;
      this.key = key;
    }
    public ModifierKeys Modifier { get { return modifier; } }
    public Keys Key { get { return key; } }
  }

  [Flags]
  public enum ModifierKeys : uint {
    Alt = 1,
    Control = 2,
    Shift = 4,
    Win = 8
  }
}
