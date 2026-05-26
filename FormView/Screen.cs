using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Forms;
using Sunny.UI;

namespace Main_Base
{
    public partial class Screen : UIForm
    {
        public Screen()
        {
            InitializeComponent();
            uiProgressIndicator1.Start();           
        }
        private PictureBox pictureBox1;
        private System.Windows.Forms.Timer timer1;
        private IContainer components;
        public void update_label( string str)
        {
            if (uiLabel1.InvokeRequired)
            {
                uiLabel1.Invoke(new MethodInvoker(() =>
                {
                    uiLabel1.Text = str;                   
                }));
            }          
        }
        private void timer1_Tick(object sender, EventArgs e)
        { 

        }        
        public bool start_Screen = false;
        public void process()
        {
            //try
            //{
            //    uiProcessBar1.Invoke(new Action(() =>
            //    {
            //        if (uiProcessBar1.Value <= uiProcessBar1.Maximum && change ==false)
            //        {
            //            inc += 10;
            //            uiProcessBar1.Value = inc;                       
            //            if ( uiProcessBar1.Value >= uiProcessBar1.Maximum)
            //            {
            //                change = true;
            //            } 

            //        }
            //        else
            //        {
            //            inc -= 5;
            //            uiProcessBar1.Value = inc;
            //        }
            //        if(uiProcessBar1.Value <= 0)
            //        {
            //            inc = 0;
            //            change = false;
            //        }
            //    }));              
            //}
            //catch { }

        }
        private UILabel uiLabel1;
        private UIProgressIndicator uiProgressIndicator1;

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.uiLabel1 = new Sunny.UI.UILabel();
            this.uiProgressIndicator1 = new Sunny.UI.UIProgressIndicator();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // timer1
            // 
            this.timer1.Interval = 200;
            // 
            // uiLabel1
            // 
            this.uiLabel1.BackColor = System.Drawing.Color.Black;
            this.uiLabel1.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.uiLabel1.ForeColor = System.Drawing.Color.White;
            this.uiLabel1.Location = new System.Drawing.Point(20, 380);
            this.uiLabel1.Name = "uiLabel1";
            this.uiLabel1.Size = new System.Drawing.Size(780, 20);
            this.uiLabel1.TabIndex = 6;
            // 
            // uiProgressIndicator1
            // 
            this.uiProgressIndicator1.BackColor = System.Drawing.Color.Black;
            this.uiProgressIndicator1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.uiProgressIndicator1.Location = new System.Drawing.Point(0, 380);
            this.uiProgressIndicator1.MinimumSize = new System.Drawing.Size(1, 1);
            this.uiProgressIndicator1.Name = "uiProgressIndicator1";
            this.uiProgressIndicator1.Size = new System.Drawing.Size(20, 20);
            this.uiProgressIndicator1.Style = Sunny.UI.UIStyle.Custom;
            this.uiProgressIndicator1.TabIndex = 7;
            this.uiProgressIndicator1.Text = "uiProgressIndicator1";
            // 
            // pictureBox1
            // 
            this.pictureBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureBox1.Image = global::Main_Base.Properties.Resources.Video1_1;
            this.pictureBox1.Location = new System.Drawing.Point(0, 0);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(800, 400);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 4;
            this.pictureBox1.TabStop = false;
            // 
            // Screen
            // 
            this.AllowShowTitle = false;
            this.ClientSize = new System.Drawing.Size(800, 400);
            this.Controls.Add(this.uiProgressIndicator1);
            this.Controls.Add(this.uiLabel1);
            this.Controls.Add(this.pictureBox1);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Screen";
            this.Padding = new System.Windows.Forms.Padding(0);
            this.ShowIcon = false;
            this.ShowTitle = false;
            this.Text = "Falsh";
            this.TitleHeight = 29;
            this.ZoomScaleRect = new System.Drawing.Rectangle(15, 15, 800, 400);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);

        }
        //public void text_control( int value)
        //{
        //    if (uiTextBox1.InvokeRequired)
        //    {
        //        uiTextBox1.Invoke(new MethodInvoker(() =>
        //        {
        //            uiTextBox1.Text = value.ToString();
        //        }));
        //    }
        //}

    }
}

