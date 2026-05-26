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
            DataGridView_Offset.Columns["STT"].Width = 45;
            DataGridView_Offset.Columns["Z1"].Width = 95;
            DataGridView_Offset.Columns["Z2"].Width = 95;
            DataGridView_Offset.Columns["Z3"].Width = 95;
            DataGridView_Offset.Columns["Z4"].Width = 95;
            DataGridView_Offset.Columns["STT"].ReadOnly = true;
            DataGridView_Offset.Columns["Z1"].ReadOnly = true;
            DataGridView_Offset.Columns["Z2"].ReadOnly = true;
            DataGridView_Offset.Columns["Z3"].ReadOnly = true;
            DataGridView_Offset.Columns["Z4"].ReadOnly = true;
            Disconnect_SQL();
        }
        public void Config_LoadDataRB_DataGridView_1()
        {
            connect_SQL();
            string query = string.Format("SELECT * from Matrix_Panel1_Cam_PLC");
            SQLiteDataAdapter adapter = new SQLiteDataAdapter(query, conUse);
            DataSet dataSet = new DataSet();
            adapter.Fill(dataSet, "Matrix_Panel1_Cam_PLC");
            DataGridView_Pos1.DataSource = dataSet.Tables["Matrix_Panel1_Cam_PLC"];
            DataGridView_Pos1.Columns["STT"].Width = 50;
            DataGridView_Pos1.Columns["X"].Width = 75;
            DataGridView_Pos1.Columns["Y"].Width = 75;

            DataGridView_Pos1.Columns["STT"].ReadOnly = true;
            DataGridView_Pos1.Columns["X"].ReadOnly = true;
            DataGridView_Pos1.Columns["Y"].ReadOnly = true;
            Disconnect_SQL();
        }
        public void Config_LoadDataRB_DataGridView_2()
        {
            connect_SQL();
            string query = string.Format("SELECT * from Matrix_Panel2_Cam_PLC");
            SQLiteDataAdapter adapter = new SQLiteDataAdapter(query, conUse);
            DataSet dataSet = new DataSet();
            adapter.Fill(dataSet, "Matrix_Panel2_Cam_PLC");
            DataGridView_Pos2.DataSource = dataSet.Tables["Matrix_Panel2_Cam_PLC"];
            DataGridView_Pos2.Columns["STT"].Width = 50;
            DataGridView_Pos2.Columns["X"].Width = 75;
            DataGridView_Pos2.Columns["Y"].Width = 75;

            DataGridView_Pos2.Columns["STT"].ReadOnly = true;
            DataGridView_Pos2.Columns["X"].ReadOnly = true;
            DataGridView_Pos2.Columns["Y"].ReadOnly = true;
            Disconnect_SQL();
        }
        public void Config_LoadDataRB_DataGridView_3()
        {
            connect_SQL();
            string query = string.Format("SELECT * from Matrix_Panel3_Cam_PLC");
            SQLiteDataAdapter adapter = new SQLiteDataAdapter(query, conUse);
            DataSet dataSet = new DataSet();
            adapter.Fill(dataSet, "Matrix_Panel3_Cam_PLC");
            DataGridView_Pos3.DataSource = dataSet.Tables["Matrix_Panel3_Cam_PLC"];
            DataGridView_Pos3.Columns["STT"].Width = 50;
            DataGridView_Pos3.Columns["X"].Width = 75;
            DataGridView_Pos3.Columns["Y"].Width = 75;

            DataGridView_Pos3.Columns["STT"].ReadOnly = true;
            DataGridView_Pos3.Columns["X"].ReadOnly = true;
            DataGridView_Pos3.Columns["Y"].ReadOnly = true;
            Disconnect_SQL();
        }
        public void Config_LoadDataRB_DataGridView_4()
        {
            connect_SQL();
            string query = string.Format("SELECT * from Matrix_Panel4_Cam_PLC");
            SQLiteDataAdapter adapter = new SQLiteDataAdapter(query, conUse);
            DataSet dataSet = new DataSet();
            adapter.Fill(dataSet, "Matrix_Panel4_Cam_PLC");
            DataGridView_Pos4.DataSource = dataSet.Tables["Matrix_Panel4_Cam_PLC"];
            DataGridView_Pos4.Columns["STT"].Width = 50;
            DataGridView_Pos4.Columns["X"].Width = 75;
            DataGridView_Pos4.Columns["Y"].Width = 75;

            DataGridView_Pos4.Columns["STT"].ReadOnly = true;
            DataGridView_Pos4.Columns["X"].ReadOnly = true;
            DataGridView_Pos4.Columns["Y"].ReadOnly = true;
            Disconnect_SQL();
        }
        private void btn_Exit_Click(object sender, EventArgs e)
        {
            this.Hide();
        }

        
    }
}
