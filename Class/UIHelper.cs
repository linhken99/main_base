using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace Main_Base.Class
{
    public static class UIHelper
    {
        // Cache PropertyInfo để reflection không bị gọi nhiều lần
        private static readonly PropertyInfo DoubleBufferedProp =
            typeof(Control).GetProperty("DoubleBuffered",
                BindingFlags.Instance | BindingFlags.NonPublic);

        /// <summary>
        /// Bật double-buffer cho control (và toàn bộ control con nếu recursive = true).
        /// Gọi một lần sau InitializeComponent/Load để giảm nháy khi vẽ.
        /// </summary>
        public static void EnableDoubleBuffer(Control ctrl, bool recursive = true)
        {
            if (ctrl == null) return;

            try
            {
                DoubleBufferedProp?.SetValue(ctrl, true, null);
            }
            catch
            {
                // Một số control có thể không cho set - bỏ qua là được.
            }

            if (!recursive) return;

            foreach (Control child in ctrl.Controls)
                EnableDoubleBuffer(child, true);
        }
        private const int WM_SETREDRAW = 0x000B;

        [DllImport("user32.dll")]
        private static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);
        public static IDisposable PauseRedraw(Control c)
        {
            return new RedrawScope(c);
        }
        private sealed class RedrawScope : IDisposable
        {
            private readonly Control _c;

            public RedrawScope(Control c)
            {
                _c = c ?? throw new ArgumentNullException(nameof(c));
                if (!_c.IsHandleCreated) _c.CreateControl();

                // Tắt vẽ + dừng layout tạm thời
                SendMessage(_c.Handle, WM_SETREDRAW, IntPtr.Zero, IntPtr.Zero);
                _c.SuspendLayout();
            }

            public void Dispose()
            {
                // Bật lại layout + vẽ, rồi yêu cầu invalidate & refresh
                _c.ResumeLayout(true);
                SendMessage(_c.Handle, WM_SETREDRAW, (IntPtr)1, IntPtr.Zero);
                _c.Invalidate(true);
                _c.Refresh();
            }
        }
    }
}
