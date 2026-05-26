using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Sunny.UI;

namespace Main_Base.FormView
{
    public sealed partial class ScreenConnect : UIForm
    {
        public ScreenConnect()
        {
            InitializeComponent();
            _ = Task.Run(ConnectWaveEffect);
        }
        private List<UILight> lightList;
        private void LoadLights()
        {
            lightList = new List<UILight>();
            for (int i = 1; i <= 13; i++)
            {
                var l = this.Controls.Find("uiLight" + i, true).FirstOrDefault() as UILight;
                if (l != null) lightList.Add(l);
            }
        }
        private void LightOff(UILight light)
        {
            light.OnColor = light.OffColor;   // màu tắt
        }
        private void SetBrightness(UILight light, int percent)
        {
            if (percent <= 0)
            {
                LightOff(light);
                return;
            }

            // Tạo màu theo độ sáng (Green nhạt → Green đậm)
            int alpha = (int)(255 * (percent / 100.0));

            light.OnColor = Color.FromArgb(alpha, 0, 255, 0);   // xanh lá
        }
        private async Task ConnectWaveEffect()
        {
            if (lightList == null) LoadLights();

            int count = lightList.Count;

            while (true)
            {
                for (int i = 0; i < count; i++)
                {
                    foreach (var l in lightList)
                        SetBrightness(l, 0);
                    SetBrightness(lightList[i], 50);
                    if (i + 1 < count)
                        SetBrightness(lightList[i + 1], 70);
                    if (i + 2 < count)
                        SetBrightness(lightList[i + 2], 90);
                    await Task.Delay(120); // tốc độ hiệu ứng
                }
            }
        }
    }
}
