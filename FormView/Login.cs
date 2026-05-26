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
using System.Data.SQLite;
using Main_Base.UseControlData;

namespace Main_Base
{
    public partial class Login : UIForm
    {
        public Login(Main MainForm)
        {
            InitializeComponent();
            _MainForm = MainForm;         
            btn_Eye.Symbol = 361552;
        }
        ChangPass changpass = new ChangPass();
        private UIPanel uiPanel2;
        private UISymbolButton btn_Eye;
        private UILine uiLine1;
        private UISymbolButton btn_Cancel;
        private UISymbolButton btn_login;
        private UIAvatar uiAvatar1;
        private UITextBox uitxt_password;
        private UITextBox uitxt_username;
        private UILabel uiLabel2;
        private UILabel uiLabel1;
        private UIScrollingText uiScrollingText1;
        private UIScrollingText uiScrollingText2;
        private UILabel ChangePass;
        private Main _MainForm;
        string _connectionString = "Data Source=|DataDirectory|/SQL_Matrix_Tool.db";
        private UILabel QuenPass;
        SQLiteConnection Conn = new SQLiteConnection();
        public void ConnectSQLite()
        {

            Conn.ConnectionString = _connectionString;
            Conn.Open();
        }
        public void DisConSQLite()
        {
            Conn.Close();
        }
        private void icobtn_Eye_Click(object sender, EventArgs e)
        {
            if (btn_Eye.Symbol == 361552)
            {
                btn_Eye.Symbol = 558391;
                btn_Eye.SymbolOffset = new System.Drawing.Point(0, 0);
                uitxt_password.PasswordChar = '\0';
            }
            else
            {
                btn_Eye.Symbol = 361552;
                btn_Eye.SymbolOffset = new System.Drawing.Point(-5, 2);
                uitxt_password.PasswordChar = '●';
            }
        }

        private void btn_login_Click(object sender, EventArgs e)
        {
            if (uitxt_password.Text == Global.Password_New)
            {
                Task.Run(() =>
                {
                    _MainForm.Unlock_Action();
                    //_MainForm.Status_btn_Login(61596);//61475//61596
                });
                this.Close();
            }
            else
            {
                MessageBox.Show("Sai mật khẩu", "Mật khẩu", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void btn_Cancel_Click(object sender, EventArgs e)
        {                     
                this.Close();           
        }

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Login));
            this.uiPanel2 = new Sunny.UI.UIPanel();
            this.QuenPass = new Sunny.UI.UILabel();
            this.ChangePass = new Sunny.UI.UILabel();
            this.btn_Eye = new Sunny.UI.UISymbolButton();
            this.uiLine1 = new Sunny.UI.UILine();
            this.btn_Cancel = new Sunny.UI.UISymbolButton();
            this.btn_login = new Sunny.UI.UISymbolButton();
            this.uiAvatar1 = new Sunny.UI.UIAvatar();
            this.uitxt_password = new Sunny.UI.UITextBox();
            this.uitxt_username = new Sunny.UI.UITextBox();
            this.uiLabel2 = new Sunny.UI.UILabel();
            this.uiLabel1 = new Sunny.UI.UILabel();
            this.uiScrollingText1 = new Sunny.UI.UIScrollingText();
            this.uiScrollingText2 = new Sunny.UI.UIScrollingText();
            this.uiPanel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // uiPanel2
            // 
            this.uiPanel2.Controls.Add(this.QuenPass);
            this.uiPanel2.Controls.Add(this.ChangePass);
            this.uiPanel2.Controls.Add(this.btn_Eye);
            this.uiPanel2.Controls.Add(this.uiLine1);
            this.uiPanel2.Controls.Add(this.btn_Cancel);
            this.uiPanel2.Controls.Add(this.btn_login);
            this.uiPanel2.Controls.Add(this.uiAvatar1);
            this.uiPanel2.Controls.Add(this.uitxt_password);
            this.uiPanel2.Controls.Add(this.uitxt_username);
            this.uiPanel2.Controls.Add(this.uiLabel2);
            this.uiPanel2.Controls.Add(this.uiLabel1);
            this.uiPanel2.FillColor = System.Drawing.Color.White;
            this.uiPanel2.FillColor2 = System.Drawing.Color.White;
            this.uiPanel2.FillDisableColor = System.Drawing.Color.White;
            this.uiPanel2.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.uiPanel2.ForeColor = System.Drawing.Color.Black;
            this.uiPanel2.ForeDisableColor = System.Drawing.Color.Black;
            this.uiPanel2.Location = new System.Drawing.Point(396, 106);
            this.uiPanel2.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.uiPanel2.MinimumSize = new System.Drawing.Size(1, 1);
            this.uiPanel2.Name = "uiPanel2";
            this.uiPanel2.RectColor = System.Drawing.Color.White;
            this.uiPanel2.RectDisableColor = System.Drawing.Color.White;
            this.uiPanel2.Size = new System.Drawing.Size(249, 260);
            this.uiPanel2.TabIndex = 5;
            this.uiPanel2.Text = null;
            this.uiPanel2.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // QuenPass
            // 
            this.QuenPass.AutoSize = true;
            this.QuenPass.Cursor = System.Windows.Forms.Cursors.Hand;
            this.QuenPass.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F);
            this.QuenPass.ForeColor = System.Drawing.Color.Blue;
            this.QuenPass.Location = new System.Drawing.Point(127, 242);
            this.QuenPass.Name = "QuenPass";
            this.QuenPass.Size = new System.Drawing.Size(86, 13);
            this.QuenPass.TabIndex = 16;
            this.QuenPass.Text = "Quên mật khẩu?";
            this.QuenPass.Click += new System.EventHandler(this.QuenPass_Click);
            // 
            // ChangePass
            // 
            this.ChangePass.AutoSize = true;
            this.ChangePass.Cursor = System.Windows.Forms.Cursors.Hand;
            this.ChangePass.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F);
            this.ChangePass.ForeColor = System.Drawing.Color.Blue;
            this.ChangePass.Location = new System.Drawing.Point(16, 242);
            this.ChangePass.Name = "ChangePass";
            this.ChangePass.Size = new System.Drawing.Size(70, 13);
            this.ChangePass.TabIndex = 15;
            this.ChangePass.Text = "Đổi mật khẩu";
            this.ChangePass.Click += new System.EventHandler(this.ChangePass_Click);
            // 
            // btn_Eye
            // 
            this.btn_Eye.BackColor = System.Drawing.Color.Transparent;
            this.btn_Eye.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btn_Eye.FillColor = System.Drawing.Color.Transparent;
            this.btn_Eye.FillColor2 = System.Drawing.Color.Transparent;
            this.btn_Eye.FillDisableColor = System.Drawing.Color.Transparent;
            this.btn_Eye.FillHoverColor = System.Drawing.Color.Transparent;
            this.btn_Eye.FillPressColor = System.Drawing.Color.Transparent;
            this.btn_Eye.FillSelectedColor = System.Drawing.Color.Transparent;
            this.btn_Eye.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.btn_Eye.ForeDisableColor = System.Drawing.Color.Transparent;
            this.btn_Eye.Location = new System.Drawing.Point(216, 178);
            this.btn_Eye.MinimumSize = new System.Drawing.Size(1, 1);
            this.btn_Eye.Name = "btn_Eye";
            this.btn_Eye.RectColor = System.Drawing.Color.Transparent;
            this.btn_Eye.RectDisableColor = System.Drawing.Color.Transparent;
            this.btn_Eye.RectHoverColor = System.Drawing.Color.Transparent;
            this.btn_Eye.RectPressColor = System.Drawing.Color.Transparent;
            this.btn_Eye.RectSelectedColor = System.Drawing.Color.Transparent;
            this.btn_Eye.Size = new System.Drawing.Size(25, 25);
            this.btn_Eye.Symbol = 361552;
            this.btn_Eye.SymbolColor = System.Drawing.Color.DarkGray;
            this.btn_Eye.SymbolHoverColor = System.Drawing.Color.DarkGray;
            this.btn_Eye.SymbolOffset = new System.Drawing.Point(1, 0);
            this.btn_Eye.SymbolPressColor = System.Drawing.Color.DimGray;
            this.btn_Eye.SymbolSelectedColor = System.Drawing.Color.Gray;
            this.btn_Eye.TabIndex = 14;
            this.btn_Eye.TipsFont = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.btn_Eye.Click += new System.EventHandler(this.icobtn_Eye_Click);
            // 
            // uiLine1
            // 
            this.uiLine1.BackColor = System.Drawing.Color.Transparent;
            this.uiLine1.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.uiLine1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(48)))), ((int)(((byte)(48)))));
            this.uiLine1.Location = new System.Drawing.Point(37, 69);
            this.uiLine1.MinimumSize = new System.Drawing.Size(1, 1);
            this.uiLine1.Name = "uiLine1";
            this.uiLine1.RectSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.None;
            this.uiLine1.Size = new System.Drawing.Size(175, 19);
            this.uiLine1.TabIndex = 13;
            this.uiLine1.Text = "Đăng nhập";
            // 
            // btn_Cancel
            // 
            this.btn_Cancel.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btn_Cancel.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(230)))), ((int)(((byte)(80)))), ((int)(((byte)(80)))));
            this.btn_Cancel.FillColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(230)))), ((int)(((byte)(80)))), ((int)(((byte)(80)))));
            this.btn_Cancel.FillHoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(235)))), ((int)(((byte)(115)))), ((int)(((byte)(115)))));
            this.btn_Cancel.FillPressColor = System.Drawing.Color.FromArgb(((int)(((byte)(184)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.btn_Cancel.FillSelectedColor = System.Drawing.Color.FromArgb(((int)(((byte)(184)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.btn_Cancel.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.btn_Cancel.LightColor = System.Drawing.Color.FromArgb(((int)(((byte)(253)))), ((int)(((byte)(243)))), ((int)(((byte)(243)))));
            this.btn_Cancel.Location = new System.Drawing.Point(130, 208);
            this.btn_Cancel.MinimumSize = new System.Drawing.Size(1, 1);
            this.btn_Cancel.Name = "btn_Cancel";
            this.btn_Cancel.Padding = new System.Windows.Forms.Padding(28, 0, 0, 0);
            this.btn_Cancel.RectColor = System.Drawing.Color.FromArgb(((int)(((byte)(230)))), ((int)(((byte)(80)))), ((int)(((byte)(80)))));
            this.btn_Cancel.RectHoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(235)))), ((int)(((byte)(115)))), ((int)(((byte)(115)))));
            this.btn_Cancel.RectPressColor = System.Drawing.Color.FromArgb(((int)(((byte)(184)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.btn_Cancel.RectSelectedColor = System.Drawing.Color.FromArgb(((int)(((byte)(184)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.btn_Cancel.Size = new System.Drawing.Size(106, 29);
            this.btn_Cancel.Style = Sunny.UI.UIStyle.Custom;
            this.btn_Cancel.Symbol = 61453;
            this.btn_Cancel.TabIndex = 12;
            this.btn_Cancel.Text = "Thoát";
            this.btn_Cancel.TipsFont = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.btn_Cancel.Click += new System.EventHandler(this.btn_Cancel_Click);
            // 
            // btn_login
            // 
            this.btn_login.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btn_login.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.btn_login.Location = new System.Drawing.Point(16, 208);
            this.btn_login.MinimumSize = new System.Drawing.Size(1, 1);
            this.btn_login.Name = "btn_login";
            this.btn_login.Size = new System.Drawing.Size(106, 29);
            this.btn_login.Symbol = 61452;
            this.btn_login.TabIndex = 11;
            this.btn_login.Text = "Đăng nhập";
            this.btn_login.TipsFont = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.btn_login.Click += new System.EventHandler(this.btn_login_Click);
            // 
            // uiAvatar1
            // 
            this.uiAvatar1.BackColor = System.Drawing.Color.Transparent;
            this.uiAvatar1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.uiAvatar1.Location = new System.Drawing.Point(94, 3);
            this.uiAvatar1.MinimumSize = new System.Drawing.Size(1, 1);
            this.uiAvatar1.Name = "uiAvatar1";
            this.uiAvatar1.Size = new System.Drawing.Size(60, 60);
            this.uiAvatar1.TabIndex = 10;
            // 
            // uitxt_password
            // 
            this.uitxt_password.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.uitxt_password.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.uitxt_password.Location = new System.Drawing.Point(37, 175);
            this.uitxt_password.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.uitxt_password.MinimumSize = new System.Drawing.Size(1, 16);
            this.uitxt_password.Name = "uitxt_password";
            this.uitxt_password.Padding = new System.Windows.Forms.Padding(5);
            this.uitxt_password.PasswordChar = '•';
            this.uitxt_password.ShowText = false;
            this.uitxt_password.Size = new System.Drawing.Size(175, 25);
            this.uitxt_password.Symbol = 61475;
            this.uitxt_password.TabIndex = 9;
            this.uitxt_password.TextAlignment = System.Drawing.ContentAlignment.MiddleRight;
            this.uitxt_password.Watermark = "Type your password";
            this.uitxt_password.KeyDown += new System.Windows.Forms.KeyEventHandler(this.uitxt_password_KeyDown);
            // 
            // uitxt_username
            // 
            this.uitxt_username.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.uitxt_username.FillColor2 = System.Drawing.Color.White;
            this.uitxt_username.FillDisableColor = System.Drawing.Color.White;
            this.uitxt_username.FillReadOnlyColor = System.Drawing.Color.White;
            this.uitxt_username.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.uitxt_username.ForeColor = System.Drawing.Color.Black;
            this.uitxt_username.ForeDisableColor = System.Drawing.Color.Black;
            this.uitxt_username.ForeReadOnlyColor = System.Drawing.Color.Black;
            this.uitxt_username.Location = new System.Drawing.Point(37, 119);
            this.uitxt_username.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.uitxt_username.MinimumSize = new System.Drawing.Size(1, 16);
            this.uitxt_username.Name = "uitxt_username";
            this.uitxt_username.Padding = new System.Windows.Forms.Padding(5);
            this.uitxt_username.RectDisableColor = System.Drawing.Color.White;
            this.uitxt_username.RectReadOnlyColor = System.Drawing.Color.White;
            this.uitxt_username.ScrollBarColor = System.Drawing.Color.White;
            this.uitxt_username.ScrollBarStyleInherited = false;
            this.uitxt_username.ShowText = false;
            this.uitxt_username.Size = new System.Drawing.Size(175, 25);
            this.uitxt_username.Symbol = 61447;
            this.uitxt_username.TabIndex = 4;
            this.uitxt_username.Text = "Admin";
            this.uitxt_username.TextAlignment = System.Drawing.ContentAlignment.MiddleRight;
            this.uitxt_username.Watermark = "Type your username";
            // 
            // uiLabel2
            // 
            this.uiLabel2.AutoSize = true;
            this.uiLabel2.BackColor = System.Drawing.Color.Transparent;
            this.uiLabel2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.uiLabel2.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.uiLabel2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(48)))), ((int)(((byte)(48)))));
            this.uiLabel2.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.uiLabel2.ImageIndex = 1;
            this.uiLabel2.Location = new System.Drawing.Point(37, 150);
            this.uiLabel2.Name = "uiLabel2";
            this.uiLabel2.Size = new System.Drawing.Size(85, 17);
            this.uiLabel2.TabIndex = 3;
            this.uiLabel2.Text = "    Password";
            this.uiLabel2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // uiLabel1
            // 
            this.uiLabel1.AutoSize = true;
            this.uiLabel1.BackColor = System.Drawing.Color.Transparent;
            this.uiLabel1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.uiLabel1.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.uiLabel1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(48)))), ((int)(((byte)(48)))));
            this.uiLabel1.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.uiLabel1.ImageIndex = 0;
            this.uiLabel1.Location = new System.Drawing.Point(37, 94);
            this.uiLabel1.Name = "uiLabel1";
            this.uiLabel1.Size = new System.Drawing.Size(89, 17);
            this.uiLabel1.TabIndex = 2;
            this.uiLabel1.Text = "    Username";
            this.uiLabel1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // uiScrollingText1
            // 
            this.uiScrollingText1.Active = true;
            this.uiScrollingText1.BackColor = System.Drawing.Color.Transparent;
            this.uiScrollingText1.Dock = System.Windows.Forms.DockStyle.Top;
            this.uiScrollingText1.FillColor = System.Drawing.Color.White;
            this.uiScrollingText1.Font = new System.Drawing.Font("Microsoft Sans Serif", 20F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))));
            this.uiScrollingText1.ForeColor = System.Drawing.Color.Navy;
            this.uiScrollingText1.Location = new System.Drawing.Point(0, 35);
            this.uiScrollingText1.MinimumSize = new System.Drawing.Size(1, 1);
            this.uiScrollingText1.Name = "uiScrollingText1";
            this.uiScrollingText1.ScrollingType = Sunny.UI.UIScrollingText.UIScrollingType.LeftToRight;
            this.uiScrollingText1.Size = new System.Drawing.Size(734, 62);
            this.uiScrollingText1.TabIndex = 6;
            this.uiScrollingText1.Text = "AI VISION AFTER FRESS";
            this.uiScrollingText1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // uiScrollingText2
            // 
            this.uiScrollingText2.Active = true;
            this.uiScrollingText2.BackColor = System.Drawing.Color.Transparent;
            this.uiScrollingText2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.uiScrollingText2.FillColor = System.Drawing.Color.Transparent;
            this.uiScrollingText2.FillDisableColor = System.Drawing.Color.Transparent;
            this.uiScrollingText2.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))));
            this.uiScrollingText2.ForeColor = System.Drawing.Color.Navy;
            this.uiScrollingText2.Location = new System.Drawing.Point(0, 385);
            this.uiScrollingText2.MinimumSize = new System.Drawing.Size(1, 1);
            this.uiScrollingText2.Name = "uiScrollingText2";
            this.uiScrollingText2.RectColor = System.Drawing.Color.Transparent;
            this.uiScrollingText2.Size = new System.Drawing.Size(734, 26);
            this.uiScrollingText2.TabIndex = 7;
            this.uiScrollingText2.Text = "AUTOMATION.LT";
            this.uiScrollingText2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // Login
            // 
            this.BackgroundImage = global::Main_Base.Properties.Resources.Login6;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(734, 411);
            this.Controls.Add(this.uiScrollingText2);
            this.Controls.Add(this.uiScrollingText1);
            this.Controls.Add(this.uiPanel2);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Login";
            this.Text = "Login";
            this.TitleColor = System.Drawing.Color.DimGray;
            this.ZoomScaleRect = new System.Drawing.Rectangle(15, 15, 734, 411);
            this.uiPanel2.ResumeLayout(false);
            this.uiPanel2.PerformLayout();
            this.ResumeLayout(false);

        }

        private void ChangePass_Click(object sender, EventArgs e)
        {
            changpass = new ChangPass();
            changpass.Location = new Point(280, 160);
            // changpass.Size = new Size(570, 720);
            this.Controls.Add(changpass);
            changpass.BringToFront();
            changpass.Show();
        }

        private void QuenPass_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Liên hệ với phòng Automation để cấp lại mật khẩu", "Quên mật khẩu", MessageBoxButtons.OK, MessageBoxIcon.Question);
        }
     
        private void uitxt_password_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (uitxt_password.Text == Global.Password_New)
                {
                    Task.Run(() =>
                    {
                        _MainForm.Unlock_Action();
                        //_MainForm.Status_btn_Login(61596);//61475//61596
                    });
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Sai mật khẩu", "Mật khẩu", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}
