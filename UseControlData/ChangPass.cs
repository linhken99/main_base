using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SQLite;
using Sunny.UI;

namespace Main_Base.UseControlData
{
    public partial class ChangPass : UIPage
    {
        public ChangPass()
        {
            InitializeComponent();
            
        }
        string _connectionString = "Data Source=|DataDirectory|/SQL_Matrix_Tool.db";
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
        private void btn_Eye1_Click(object sender, EventArgs e)
        {
            if (btn_Eye1.Symbol == 361552)
            {
                btn_Eye1.Symbol = 558391;
                btn_Eye1.SymbolOffset = new System.Drawing.Point(0, 0);
                uiTextBox1.PasswordChar = '\0';
            }
            else
            {
                btn_Eye1.Symbol = 361552;
                btn_Eye1.SymbolOffset = new System.Drawing.Point(-5, 2);
                uiTextBox1.PasswordChar = '●';
            }
        }

        private void btn_Eye2_Click(object sender, EventArgs e)
        {
            if (btn_Eye2.Symbol == 361552)
            {
                btn_Eye2.Symbol = 558391;
                btn_Eye2.SymbolOffset = new System.Drawing.Point(0, 0);
                uiTextBox2.PasswordChar = '\0';
            }
            else
            {
                btn_Eye2.Symbol = 361552;
                btn_Eye2.SymbolOffset = new System.Drawing.Point(-5, 2);
                uiTextBox2.PasswordChar = '●';
            }
        }

        private void btn_Eye3_Click(object sender, EventArgs e)
        {
            if (btn_Eye3.Symbol == 361552)
            {
                btn_Eye3.Symbol = 558391;
                btn_Eye3.SymbolOffset = new System.Drawing.Point(0, 0);
                uiTextBox3.PasswordChar = '\0';
            }
            else
            {
                btn_Eye3.Symbol = 361552;
                btn_Eye3.SymbolOffset = new System.Drawing.Point(-5, 2);
                uiTextBox3.PasswordChar = '●';
            }
        }
        private void uiButton1_Click(object sender, EventArgs e)
        {
            try
            {
                if (uiTextBox2.Text == uiTextBox3.Text && uiTextBox2.Text != null && uiTextBox3.Text != null && uiTextBox1.Text == Global.Password_Old)
                {
                    ConnectSQLite();
                    string saveposSQL = string.Format("INSERT OR REPLACE INTO Password (STT,PassNew,PassOld) VALUES (@STT,@PassNew,@PassOld)");
                    using (SQLiteCommand cmd = new SQLiteCommand(saveposSQL, Conn))
                    {
                        cmd.Parameters.AddWithValue("@STT", 1);
                        cmd.Parameters.AddWithValue("@PassNew", uiTextBox2.Text);
                        cmd.Parameters.AddWithValue("@PassOld", uiTextBox3.Text);
                        cmd.ExecuteNonQuery();
                        Global.Password_Old = uiTextBox3.Text;
                        Global.Password_New = uiTextBox3.Text;
                    }
                    DisConSQLite();
                    MessageBox.Show("Đổi mật khẩu thành công", "Đổi mật khẩu", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.Hide();
                }
                else if (uiTextBox2.Text == null || uiTextBox3.Text == null)
                {
                    MessageBox.Show("Không được để trống", "Null", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else if (uiTextBox1.Text != Global.Password_Old)
                {
                    MessageBox.Show("Mật khẩu cũ không đúng", "Mật khẩu", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch { MessageBox.Show("Đổi mật khẩu không thành công", "Đổi mật khẩu", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }

        private void ChangPass_FormClosed(object sender, FormClosedEventArgs e)
        {
            this.Hide();
        }
    }
}
