using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace MTTApp
{
    internal class WideButtonComboBox : ComboBox
    {
        [DllImport("user32.dll")] static extern IntPtr GetWindow(IntPtr hWnd, uint uCmd);
        [DllImport("user32.dll")] static extern bool MoveWindow(IntPtr hWnd, int X, int Y, int nWidth, int nHeight, bool bRepaint);
        [DllImport("user32.dll")] static extern bool GetWindowRect(IntPtr hWnd, out RECT r);

        [StructLayout(LayoutKind.Sequential)]
        struct RECT { public int Left, Top, Right, Bottom; }

        private const uint GW_CHILD = 5;
        private const int WM_PAINT = 0x000F;
        private const int WM_SIZE  = 0x0005;

        public int ButtonWidth { get; set; } = 50;

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            AdjustEditWidth();
        }

        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);
            if (m.Msg == WM_SIZE)
                AdjustEditWidth();
            if (m.Msg == WM_PAINT)
            {
                AdjustEditWidth();
                using (var g = Graphics.FromHwnd(Handle))
                {
                    var r = new Rectangle(Width - ButtonWidth, 1, ButtonWidth - 1, Height - 2);
                    ControlPaint.DrawComboButton(g, r, DroppedDown ? ButtonState.Pushed : ButtonState.Normal);
                }
            }
        }

        private void AdjustEditWidth()
        {
            IntPtr edit = GetWindow(Handle, GW_CHILD);
            if (edit == IntPtr.Zero) return;
            RECT er, cr;
            GetWindowRect(edit, out er);
            GetWindowRect(Handle, out cr);
            int x = er.Left - cr.Left;
            int y = er.Top - cr.Top;
            int h = er.Bottom - er.Top;
            int w = Width - ButtonWidth - x;
            if (w > 0)
                MoveWindow(edit, x, y, w, h, false);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            int native = SystemInformation.VerticalScrollBarWidth;
            if (e.X >= Width - ButtonWidth && e.X < Width - native)
            {
                DroppedDown = true;
                return;
            }
            base.OnMouseDown(e);
        }
    }

    internal class WhitespaceTextBox : RichTextBox
    {
        private bool _formatting;

        public WhitespaceTextBox()
        {
            ScrollBars = RichTextBoxScrollBars.None;
            WordWrap = false;
        }

        public override string Text
        {
            get { return base.Text.Replace("\r\n", "").Replace("\r", "").Replace("\n", ""); }
            set { base.Text = value; }
        }

        protected override bool IsInputKey(Keys keyData)
        {
            if ((keyData & Keys.KeyCode) == Keys.Tab) return false;
            return base.IsInputKey(keyData);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) { e.SuppressKeyPress = true; return; }
            base.OnKeyDown(e);
        }

        protected override void OnTextChanged(EventArgs e)
        {
            if (!_formatting) HighlightSpaces();
            base.OnTextChanged(e);
        }

        private void HighlightSpaces()
        {
            if (!IsHandleCreated) return;
            _formatting = true;
            int sel = SelectionStart;

            SelectAll();
            SelectionBackColor = SystemColors.Window;

            string t = Text;
            for (int i = 0; i < t.Length; i++)
            {
                if (t[i] != ' ') continue;
                SelectionStart = i;
                SelectionLength = 1;
                SelectionBackColor = Color.LightSkyBlue;
            }

            SelectionStart = Math.Min(sel, t.Length);
            SelectionLength = 0;
            _formatting = false;
        }
    }
}
