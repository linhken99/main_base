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

namespace Main_Base
{
    public partial class DataCamTop : UIPage
    {
        SQLiteConnection conUse = new SQLiteConnection();
        string connectionString = "Data Source=|DataDirectory|/SQL_Matrix_Tool.db";
        public DataCamTop()
        {
            InitializeComponent();
        }
        private void connect_SQL()
        {
            conUse.ConnectionString = connectionString;
            conUse.Open();
        }
        private void Disconnect_SQL()
        {
            conUse.Close();
        }
        public void Config_LoadDataRB_DataGridView1()
        {
            connect_SQL();
            string query = string.Format("SELECT * from Matrix_Panel_Cam_Top");
            SQLiteDataAdapter adapter = new SQLiteDataAdapter(query, conUse);
            DataSet dataSet = new DataSet();
            adapter.Fill(dataSet, "Matrix_Panel_Cam_Top");
            DataGridView1.DataSource = dataSet.Tables["Matrix_Panel_Cam_Top"];
            DataGridView1.Columns["ID"].Width = 30;
            //DataGridView1.Columns["X"].Width = 80;
            //DataGridView1.Columns["Y"].Width = 80;
            //DataGridView1.Columns["Z"].Width = 80;
            DataGridView1.Columns["A3"].Width = 40;
            DataGridView1.Columns["A4"].Width = 40;
            //DataGridView1.Columns["C"].Width = 80;
            DataGridView1.Columns["ID"].ReadOnly = true;
            DataGridView1.Columns["X"].ReadOnly = true;
            DataGridView1.Columns["Y"].ReadOnly = true;
            DataGridView1.Columns["Z"].ReadOnly = true;
            DataGridView1.Columns["A3"].ReadOnly = true;
            DataGridView1.Columns["A4"].ReadOnly = true;
            DataGridView1.Columns["C"].ReadOnly = true;

            Disconnect_SQL();
        }

        private void btn_Exit_Click(object sender, EventArgs e)
        {
            this.Hide();
        }

        
    }
}
