using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;

namespace Main_Base.Class
{
    public class AutoScaleHelper
    {
        private readonly Control _rootControl;
        private readonly Size _originalSize;
        private readonly float _originalFontSize;
        private Timer _debounceTimer;

        public AutoScaleHelper(Control rootControl)
        {
            _rootControl = rootControl;
            _originalSize = rootControl.Size;
            _originalFontSize = rootControl.Font.Size;

            _debounceTimer = new Timer();
            _debounceTimer.Interval = 150; // 150ms delay
            _debounceTimer.Tick += DebounceTimer_Tick;

            // Gắn sự kiện resize
            _rootControl.Resize += RootControl_Resize;
        }

        private void RootControl_Resize(object sender, EventArgs e)
        {
            // Reset timer mỗi lần resize
            _debounceTimer.Stop();
            _debounceTimer.Start();
        }

        private void DebounceTimer_Tick(object sender, EventArgs e)
        {
            _debounceTimer.Stop();
            ApplyScale();
        }

        private void ApplyScale()
        {
            float scaleX = (float)_rootControl.Width / _originalSize.Width;
            float scaleY = (float)_rootControl.Height / _originalSize.Height;
            float scale = Math.Min(scaleX, scaleY);

            ScaleControls(_rootControl, scale);
        }

        private void ScaleControls(Control parent, float scale)
        {
            foreach (Control ctrl in parent.Controls)
            {
                // Scale kích thước control
                ctrl.Width = (int)(ctrl.Width * scale);
                ctrl.Height = (int)(ctrl.Height * scale);

                // Scale vị trí control
                ctrl.Left = (int)(ctrl.Left * scale);
                ctrl.Top = (int)(ctrl.Top * scale);

                // Scale font
                ctrl.Font = new Font(ctrl.Font.FontFamily, _originalFontSize * scale, ctrl.Font.Style);

                // Gọi đệ quy
                if (ctrl.Controls.Count > 0)
                    ScaleControls(ctrl, scale);
            }
        }
    }
}
   
