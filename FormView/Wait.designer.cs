
namespace Main_Base
{
    partial class Wait
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.uiProgressIndicator1 = new Sunny.UI.UIProgressIndicator();
            this.uiLabel1 = new Sunny.UI.UILabel();
            this.SuspendLayout();
            // 
            // uiProgressIndicator1
            // 
            this.uiProgressIndicator1.Active = true;
            this.uiProgressIndicator1.BackColor = System.Drawing.Color.White;
            this.uiProgressIndicator1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.uiProgressIndicator1.Location = new System.Drawing.Point(93, 6);
            this.uiProgressIndicator1.MinimumSize = new System.Drawing.Size(1, 1);
            this.uiProgressIndicator1.Name = "uiProgressIndicator1";
            this.uiProgressIndicator1.Size = new System.Drawing.Size(60, 60);
            this.uiProgressIndicator1.Style = Sunny.UI.UIStyle.Custom;
            this.uiProgressIndicator1.TabIndex = 0;
            this.uiProgressIndicator1.Text = "uiProgressIndicator1";
            // 
            // uiLabel1
            // 
            this.uiLabel1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.uiLabel1.ForeColor = System.Drawing.Color.Black;
            this.uiLabel1.Location = new System.Drawing.Point(12, 63);
            this.uiLabel1.Name = "uiLabel1";
            this.uiLabel1.Size = new System.Drawing.Size(223, 20);
            this.uiLabel1.TabIndex = 1;
            this.uiLabel1.Text = "Please waiting ... ...";
            this.uiLabel1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // Wait
            // 
            this.AllowShowTitle = false;
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(247, 84);
            this.ControlBox = false;
            this.ControlBoxCloseFillHoverColor = System.Drawing.Color.White;
            this.ControlBoxFillHoverColor = System.Drawing.Color.White;
            this.Controls.Add(this.uiLabel1);
            this.Controls.Add(this.uiProgressIndicator1);
            this.ForeColor = System.Drawing.Color.White;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Wait";
            this.Padding = new System.Windows.Forms.Padding(0);
            this.RectColor = System.Drawing.Color.Black;
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.ShowRect = false;
            this.ShowShadow = false;
            this.ShowTitle = false;
            this.Text = "Pleas Wait...";
            this.ZoomScaleRect = new System.Drawing.Rectangle(15, 15, 800, 450);
            this.ResumeLayout(false);

        }

        #endregion

        private Sunny.UI.UIProgressIndicator uiProgressIndicator1;
        private Sunny.UI.UILabel uiLabel1;
    }
}