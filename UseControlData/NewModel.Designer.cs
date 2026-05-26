
namespace Main_Base.UseControlData
{
    partial class NewModel
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
            this.uiTextBox1 = new Sunny.UI.UITextBox();
            this.uiLabel1 = new Sunny.UI.UILabel();
            this.uiButton1 = new Sunny.UI.UIButton();
            this.uiButton2 = new Sunny.UI.UIButton();
            this.SuspendLayout();
            // 
            // uiTextBox1
            // 
            this.uiTextBox1.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.uiTextBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.uiTextBox1.Location = new System.Drawing.Point(132, 58);
            this.uiTextBox1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.uiTextBox1.MinimumSize = new System.Drawing.Size(1, 16);
            this.uiTextBox1.Name = "uiTextBox1";
            this.uiTextBox1.Padding = new System.Windows.Forms.Padding(5);
            this.uiTextBox1.ShowText = false;
            this.uiTextBox1.Size = new System.Drawing.Size(259, 29);
            this.uiTextBox1.TabIndex = 0;
            this.uiTextBox1.TextAlignment = System.Drawing.ContentAlignment.MiddleRight;
            this.uiTextBox1.Watermark = "";
            // 
            // uiLabel1
            // 
            this.uiLabel1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.uiLabel1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(48)))), ((int)(((byte)(48)))));
            this.uiLabel1.Location = new System.Drawing.Point(25, 61);
            this.uiLabel1.Name = "uiLabel1";
            this.uiLabel1.Size = new System.Drawing.Size(100, 23);
            this.uiLabel1.TabIndex = 1;
            this.uiLabel1.Text = "Model Name";
            // 
            // uiButton1
            // 
            this.uiButton1.Cursor = System.Windows.Forms.Cursors.Hand;
            this.uiButton1.FillColor = System.Drawing.Color.Gray;
            this.uiButton1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.uiButton1.Location = new System.Drawing.Point(175, 95);
            this.uiButton1.MinimumSize = new System.Drawing.Size(1, 1);
            this.uiButton1.Name = "uiButton1";
            this.uiButton1.Size = new System.Drawing.Size(80, 27);
            this.uiButton1.TabIndex = 2;
            this.uiButton1.Text = "Yes";
            this.uiButton1.TipsFont = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.uiButton1.Click += new System.EventHandler(this.uiButton1_Click);
            // 
            // uiButton2
            // 
            this.uiButton2.Cursor = System.Windows.Forms.Cursors.Hand;
            this.uiButton2.FillColor = System.Drawing.Color.Gray;
            this.uiButton2.FillColor2 = System.Drawing.Color.Gray;
            this.uiButton2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.uiButton2.Location = new System.Drawing.Point(264, 95);
            this.uiButton2.MinimumSize = new System.Drawing.Size(1, 1);
            this.uiButton2.Name = "uiButton2";
            this.uiButton2.Size = new System.Drawing.Size(80, 27);
            this.uiButton2.TabIndex = 3;
            this.uiButton2.Text = "No";
            this.uiButton2.TipsColor = System.Drawing.Color.RosyBrown;
            this.uiButton2.TipsFont = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.uiButton2.Click += new System.EventHandler(this.uiButton2_Click);
            // 
            // NewModel
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.LightGray;
            this.ClientSize = new System.Drawing.Size(402, 132);
            this.ControlBox = false;
            this.Controls.Add(this.uiButton2);
            this.Controls.Add(this.uiButton1);
            this.Controls.Add(this.uiLabel1);
            this.Controls.Add(this.uiTextBox1);
            this.IconImage = global::Main_Base.Properties.Resources.icons8_activity_history_64;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "NewModel";
            this.Text = "Add New Model";
            this.TitleColor = System.Drawing.Color.DimGray;
            this.ZoomScaleRect = new System.Drawing.Rectangle(15, 15, 816, 489);
            this.ResumeLayout(false);

        }

        #endregion

        private Sunny.UI.UITextBox uiTextBox1;
        private Sunny.UI.UILabel uiLabel1;
        private Sunny.UI.UIButton uiButton1;
        private Sunny.UI.UIButton uiButton2;
    }
}
