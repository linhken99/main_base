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
    public partial class TransferData : UIForm
    {
        public TransferData()
        {
            InitializeComponent();
        }
        public bool Run = false;
        public bool FlowDirection = false;
        public int Valueload = 0;
        public async Task Transfer()
        {
            while (Run)
            {
                await Task.Delay(500);
                foreach (var pipe in this.GetControls<UIPipe>())
                {
                    if (FlowDirection == true)
                    {
                        pipe.FlowDirection = UIPipe.UIFlowDirection.Forward;
                        pipe.FlowColor = Color.Red;
                    }
                    else
                    {
                        pipe.FlowDirection = UIPipe.UIFlowDirection.Reverse;
                        pipe.FlowColor = Color.DarkGreen;
                    }
                    pipe.flowSize = 20;
                    pipe.flowSpeed = 15;
                    pipe.FlowInterval = 10;
                    pipe.Active = true;
                    uiProcessBar1.Value = Valueload;
                    pipe.Invalidate();
                }
            }
        }
        public void InfoTransfer(string str)
        {
            if (uiListBox1.InvokeRequired)
            {
                uiListBox1.Invoke(new Action(() =>
                {
                    uiListBox1.Items.Add(str);
                    uiListBox1.TopIndex = uiListBox1.Items.Count - 1;
                }));
            }
            else
            {
                uiListBox1.Invoke(new Action(() =>
                {
                    uiListBox1.Items.Add(str);
                    uiListBox1.TopIndex = uiListBox1.Items.Count - 1;
                }));
            }
        }
        public void StopTransfer()
        {
            uiPipe1.Active = false;
        }
        private void uiButton1_Click(object sender, EventArgs e)
        {
            Run = false;
            this.Close();
        }
        public void LabelTransferWrite()
        {
            uiScrollingText1.Text = "Write...";
            uiScrollingText1.ScrollingType = UIScrollingText.UIScrollingType.LeftToRight;
        }
        public void LabelTransferRead()
        {
            uiScrollingText1.Text = "Read...";
            uiScrollingText1.ScrollingType = UIScrollingText.UIScrollingType.RightToLeft;
        }
    }
}
