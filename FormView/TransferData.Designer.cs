
namespace Main_Base.FormView
{
    partial class TransferData
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.uiLabel1 = new Sunny.UI.UILabel();
            this.uiProcessBar1 = new Sunny.UI.UIProcessBar();
            this.uiButton1 = new Sunny.UI.UIButton();
            this.uiListBox1 = new Sunny.UI.UIListBox();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.uiPipe1 = new Sunny.UI.UIPipe();
            this.uiScrollingText1 = new Sunny.UI.UIScrollingText();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // uiLabel1
            // 
            this.uiLabel1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))));
            this.uiLabel1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(192)))));
            this.uiLabel1.Location = new System.Drawing.Point(2, 347);
            this.uiLabel1.Name = "uiLabel1";
            this.uiLabel1.Size = new System.Drawing.Size(421, 30);
            this.uiLabel1.TabIndex = 15;
            this.uiLabel1.Text = "Automation";
            this.uiLabel1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // uiProcessBar1
            // 
            this.uiProcessBar1.BackColor = System.Drawing.Color.LightGray;
            this.uiProcessBar1.FillColor = System.Drawing.Color.LightGray;
            this.uiProcessBar1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.uiProcessBar1.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.uiProcessBar1.Location = new System.Drawing.Point(3, 154);
            this.uiProcessBar1.MinimumSize = new System.Drawing.Size(508, 29);
            this.uiProcessBar1.Name = "uiProcessBar1";
            this.uiProcessBar1.RectColor = System.Drawing.Color.FromArgb(((int)(((byte)(80)))), ((int)(((byte)(145)))), ((int)(((byte)(255)))));
            this.uiProcessBar1.Size = new System.Drawing.Size(508, 29);
            this.uiProcessBar1.Style = Sunny.UI.UIStyle.Custom;
            this.uiProcessBar1.TabIndex = 14;
            this.uiProcessBar1.Value = 10;
            // 
            // uiButton1
            // 
            this.uiButton1.Cursor = System.Windows.Forms.Cursors.Hand;
            this.uiButton1.FillColor = System.Drawing.Color.DarkGray;
            this.uiButton1.FillColor2 = System.Drawing.Color.DarkGray;
            this.uiButton1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.uiButton1.ForeColor = System.Drawing.Color.Black;
            this.uiButton1.Location = new System.Drawing.Point(429, 347);
            this.uiButton1.MinimumSize = new System.Drawing.Size(1, 1);
            this.uiButton1.Name = "uiButton1";
            this.uiButton1.RectColor = System.Drawing.Color.Gray;
            this.uiButton1.RectDisableColor = System.Drawing.Color.Gray;
            this.uiButton1.RectHoverColor = System.Drawing.Color.Gray;
            this.uiButton1.RectPressColor = System.Drawing.Color.Gray;
            this.uiButton1.RectSelectedColor = System.Drawing.Color.Gray;
            this.uiButton1.Size = new System.Drawing.Size(82, 30);
            this.uiButton1.TabIndex = 13;
            this.uiButton1.Text = "Close";
            this.uiButton1.TipsFont = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.uiButton1.Click += new System.EventHandler(this.uiButton1_Click);
            // 
            // uiListBox1
            // 
            this.uiListBox1.FillColor = System.Drawing.Color.LightGray;
            this.uiListBox1.FillColor2 = System.Drawing.Color.LightGray;
            this.uiListBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.uiListBox1.HoverColor = System.Drawing.Color.LightGray;
            this.uiListBox1.ItemSelectForeColor = System.Drawing.Color.White;
            this.uiListBox1.Location = new System.Drawing.Point(3, 191);
            this.uiListBox1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.uiListBox1.MinimumSize = new System.Drawing.Size(1, 1);
            this.uiListBox1.Name = "uiListBox1";
            this.uiListBox1.Padding = new System.Windows.Forms.Padding(2);
            this.uiListBox1.RectColor = System.Drawing.Color.Gray;
            this.uiListBox1.RectDisableColor = System.Drawing.Color.Gray;
            this.uiListBox1.ScrollBarBackColor = System.Drawing.Color.Gray;
            this.uiListBox1.ScrollBarColor = System.Drawing.Color.Gray;
            this.uiListBox1.ScrollBarStyleInherited = false;
            this.uiListBox1.ShowText = false;
            this.uiListBox1.Size = new System.Drawing.Size(508, 148);
            this.uiListBox1.TabIndex = 12;
            this.uiListBox1.Text = "uiListBox1";
            // 
            // pictureBox2
            // 
            this.pictureBox2.Image = global::Main_Base.Properties.Resources.server;
            this.pictureBox2.Location = new System.Drawing.Point(383, 54);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(100, 78);
            this.pictureBox2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox2.TabIndex = 11;
            this.pictureBox2.TabStop = false;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::Main_Base.Properties.Resources.pc;
            this.pictureBox1.Location = new System.Drawing.Point(32, 54);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(100, 78);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 10;
            this.pictureBox1.TabStop = false;
            // 
            // uiPipe1
            // 
            this.uiPipe1.Active = true;
            this.uiPipe1.BackColor = System.Drawing.Color.LightGray;
            this.uiPipe1.FillColor = System.Drawing.Color.LightGray;
            this.uiPipe1.FlowColor = System.Drawing.Color.Green;
            this.uiPipe1.FlowDirection = Sunny.UI.UIPipe.UIFlowDirection.Reverse;
            this.uiPipe1.FlowInterval = 10;
            this.uiPipe1.FlowSize = 24;
            this.uiPipe1.FlowSpeed = 12;
            this.uiPipe1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.uiPipe1.Location = new System.Drawing.Point(134, 83);
            this.uiPipe1.MinimumSize = new System.Drawing.Size(1, 1);
            this.uiPipe1.Name = "uiPipe1";
            this.uiPipe1.Radius = 16;
            this.uiPipe1.RadiusSides = Sunny.UI.UICornerRadiusSides.None;
            this.uiPipe1.RectColor = System.Drawing.Color.LightGray;
            this.uiPipe1.Size = new System.Drawing.Size(243, 16);
            this.uiPipe1.Style = Sunny.UI.UIStyle.Custom;
            this.uiPipe1.StyleCustomMode = true;
            this.uiPipe1.TabIndex = 9;
            this.uiPipe1.Text = "uiPipe1";
            this.uiPipe1.ZoomScaleDisabled = true;
            // 
            // uiScrollingText1
            // 
            this.uiScrollingText1.Active = true;
            this.uiScrollingText1.FillColor = System.Drawing.Color.LightGray;
            this.uiScrollingText1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.uiScrollingText1.ForeColor = System.Drawing.Color.Coral;
            this.uiScrollingText1.Location = new System.Drawing.Point(134, 106);
            this.uiScrollingText1.MinimumSize = new System.Drawing.Size(1, 1);
            this.uiScrollingText1.Name = "uiScrollingText1";
            this.uiScrollingText1.RectColor = System.Drawing.Color.LightGray;
            this.uiScrollingText1.ScrollingType = Sunny.UI.UIScrollingText.UIScrollingType.LeftToRight;
            this.uiScrollingText1.Size = new System.Drawing.Size(243, 19);
            this.uiScrollingText1.TabIndex = 16;
            this.uiScrollingText1.Text = "Write";
            // 
            // TransferData
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.LightGray;
            this.ClientSize = new System.Drawing.Size(516, 382);
            this.ControlBox = false;
            this.Controls.Add(this.uiScrollingText1);
            this.Controls.Add(this.uiLabel1);
            this.Controls.Add(this.uiProcessBar1);
            this.Controls.Add(this.uiButton1);
            this.Controls.Add(this.uiListBox1);
            this.Controls.Add(this.pictureBox2);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.uiPipe1);
            this.IconImage = global::Main_Base.Properties.Resources.icons8_paper_plane_30;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "TransferData";
            this.Text = "Transfer Data";
            this.ZoomScaleRect = new System.Drawing.Rectangle(15, 15, 800, 480);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Sunny.UI.UILabel uiLabel1;
        private Sunny.UI.UIProcessBar uiProcessBar1;
        private Sunny.UI.UIButton uiButton1;
        private Sunny.UI.UIListBox uiListBox1;
        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.PictureBox pictureBox1;
        private Sunny.UI.UIPipe uiPipe1;
        private Sunny.UI.UIScrollingText uiScrollingText1;
    }
}
