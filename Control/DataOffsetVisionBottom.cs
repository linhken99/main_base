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

namespace Main_Base
{
    public partial class DataOffsetVisionBottom : UIPage
    {
        public DataOffsetVisionBottom()
        {
            InitializeComponent();           
        }
        SQLiteConnection conUse = new SQLiteConnection();
        string connectionString = "Data Source=|DataDirectory|/SQL_Matrix_Tool.db";
        private void connect_SQL()
        {
            conUse.ConnectionString = connectionString;
            conUse.Open();
        }
        private void Disconnect_SQL()
        {
            conUse.Close();
        }
        public void Config_LoadDataRB_DataGridView_Z()
        {
            connect_SQL();
            string query = string.Format("SELECT * from TABLE_OFFSET_Z_CHECK_VISION");
            SQLiteDataAdapter adapter = new SQLiteDataAdapter(query, conUse);
            DataSet dataSet = new DataSet();
            adapter.Fill(dataSet, "TABLE_OFFSET_Z_CHECK_VISION");
            DataGridView_Offset.DataSource = dataSet.Tables["TABLE_OFFSET_Z_CHECK_VISION"];
            DataGridView_Offset.Columns["STT"].Width = 70;
            DataGridView_Offset.Columns["Z1"].Width = 140;
            DataGridView_Offset.Columns["Z2"].Width = 140;
            DataGridView_Offset.Columns["Z3"].Width = 140;

            DataGridView_Offset.Columns["STT"].ReadOnly = true;
            DataGridView_Offset.Columns["Z1"].ReadOnly = true;
            DataGridView_Offset.Columns["Z2"].ReadOnly = true;
            DataGridView_Offset.Columns["Z3"].ReadOnly = true;          
            Disconnect_SQL();
        }

        private void btn_Exit_Click(object sender, EventArgs e)
        {
            this.Hide();
        }
    }
}
