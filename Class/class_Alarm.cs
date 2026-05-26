using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Data.SqlClient; // Thư viện SQL

namespace Main_Base
{
    public class class_Alarm
    {
        // ===================Khai báo mảng Alarm===============================
        public static DateTime ngay_thang_nam;
        public static DateTime gio_phut;
        public static DateTime Time_xx;
        public static string[] Alarm_ID = new string[100];
        public static string[] Warring_ID = new string[100];
        public static string NameTableAlarm = "Alarm_Display";
        public static string TableAlarmAddress = "Data Source=|DataDirectory|/SQL_Matrix_Tool.db";
        public static bool Cancel_Alarm = true;
        public static Dictionary<string, DateTime> alarmStartTime = new Dictionary<string, DateTime>();
        // ===================Hàm gọi Alarm mới xuát hiện=================================
        public static void cmd_NewAlarm(int ID)
        {
            #region Alarm List
            // ===================Điền danh sách tên các Alarm ở đây===============================
            Alarm_ID[1] = "EMG";
            Alarm_ID[2] = "Warring: Mở Cửa";
            Alarm_ID[3] = "Axis Input Tray power OFF";
            Alarm_ID[4] = "Axis Output Tray power OFF";
            Alarm_ID[5] = "Axis X Vision power OFF";
            Alarm_ID[6] = "Axis Y Vision power OFF";
            Alarm_ID[7] = "Axis Z Vision power OFF";
            Alarm_ID[8] = "Axis R Vision power OFF";
            Alarm_ID[9] = "Axis X Remov Cover Tape power OFF";
            Alarm_ID[10] = "Axis Z Remov Cover Tape power OFF";
            Alarm_ID[11] = "Axis Loading input power OFF";
            Alarm_ID[12] = "Axis Input Tray Error";
            Alarm_ID[13] = "Axis Output Tray Error";
            Alarm_ID[14] = "Axis X Vision Error";
            Alarm_ID[15] = "Axis Y Vision Error";
            Alarm_ID[16] = "Axis Z Vision Error";
            Alarm_ID[17] = "Axis R Vision Error";
            Alarm_ID[18] = "Axis X Remov Cover Tape Error";
            Alarm_ID[19] = "Axis Z Remov Cover Tape Error";
            Alarm_ID[20] = "Axis R Loading input Error";
            Alarm_ID[21] = "Warring: Cylinder Input : X100B";
            Alarm_ID[22] = "Warring: Cylinder Input : X100C";
            Alarm_ID[23] = "Warring: Cylinder Output : X100D";
            Alarm_ID[24] = "Warring: Cylinder Output : X100E";
            Alarm_ID[25] = "Warring: Cylinder Transfer Tray : X1008";
            Alarm_ID[26] = "Warring: Cylinder Transfer Tray : X1007";
            Alarm_ID[27] = "Warring: Cylinder Align Tray Input : X100F";
            Alarm_ID[28] = "Warring: Cylinder Align Tray Output : X1010";
            Alarm_ID[29] = "Warring: Cylinder Pick Tray : X100A";
            Alarm_ID[30] = "Warring: Cylinder Pick Tray : X1009";
            Alarm_ID[31] = "Robot Hiwin Error";
            Alarm_ID[32] = "Servo Input Tray tray quá giới hạn trên ";
            Alarm_ID[33] = "Servo Input Tray tray quá giới hạn dưới ";
            Alarm_ID[34] = "Servo Output Tray tray quá giới hạn trên ";
            Alarm_ID[35] = "Servo Output Tray tray quá giới hạn dưới ";
            Alarm_ID[36] = "Servo X Vision quá giới hạn trên ";
            Alarm_ID[37] = "Servo X Vision quá giới hạn dưới ";
            Alarm_ID[38] = "Servo Y Vision quá giới hạn trên ";
            Alarm_ID[39] = "Servo Y Vision quá giới hạn dưới ";
            Alarm_ID[40] = "Servo Z Vision quá giới hạn trên ";
            Alarm_ID[41] = "Servo Z Vision quá giới hạn dưới ";
            Alarm_ID[42] = "Servo R Vision quá giới hạn trên ";
            Alarm_ID[43] = "Servo R Vision quá giới hạn dưới ";
            Alarm_ID[44] = "Servo X Remov Cover Tape quá giới hạn trên ";
            Alarm_ID[45] = "Servo X Remov Cover Tape quá giới hạn dưới ";
            Alarm_ID[46] = "Servo Z Remov Cover Tape quá giới hạn trên ";
            Alarm_ID[47] = "Servo Z Remov Cover Tape quá giới hạn dưới ";
            Alarm_ID[48] = "Servo R Loading input quá giới hạn trên ";
            Alarm_ID[49] = "Servo R Loading input quá giới hạn dưới ";
            Alarm_ID[50] = "Warring: Cylinder Grip 1 : X1011";
            Alarm_ID[51] = "Warring: Cylinder Grip 2 : X1012";
            Alarm_ID[52] = "Warring: Cylinder Grip 3 : X1013";
            Alarm_ID[53] = "Warring: Cylinder Jig input FPCB : X1014";
            Alarm_ID[54] = "Warring: Cylinder Jig input FPCB : X1015";
            Alarm_ID[55] = "Warring: Cylinder transfer đèn camera : X1016";
            Alarm_ID[56] = "Warring: Cylinder transfer đèn camera : X1017";
            Alarm_ID[57] = "Warring: Cylinder transfer group camera : X1018";
            Alarm_ID[58] = "Warring: Cylinder transfer group camera : X1019";
            Alarm_ID[59] = "Vaccum transfer tray lower";
            Alarm_ID[60] = " ";
            Alarm_ID[61] = " ";
            Alarm_ID[62] = " ";
            Alarm_ID[63] = " ";
            Alarm_ID[64] = " ";
            Alarm_ID[65] = " ";
            Alarm_ID[66] = " ";
            Alarm_ID[67] = " ";
            Alarm_ID[68] = " ";
            Alarm_ID[69] = " ";
            Alarm_ID[70] = " ";
            Alarm_ID[71] = " ";
            Alarm_ID[72] = " ";
            Alarm_ID[73] = " ";
            Alarm_ID[74] = " ";
            Alarm_ID[75] = "";
            Alarm_ID[76] = " ";
            Alarm_ID[77] = " ";
            Alarm_ID[78] = " ";
            Alarm_ID[79] = " ";
            Alarm_ID[80] = " ";
            Alarm_ID[81] = " ";
            Alarm_ID[82] = " ";
            Alarm_ID[83] = " ";
            Alarm_ID[84] = " ";
            Alarm_ID[85] = " ";
            Alarm_ID[86] = " ";
            Alarm_ID[87] = " ";
            Alarm_ID[88] = " ";
            Alarm_ID[89] = " ";
            Alarm_ID[90] = " ";
            //===============================================================================
            #endregion       
        }

        //====================== Hàm xóa Alarm đã hết=============================	
        public static void cmd_AckAlarm(int ID)
        {
            //string Table_Name = "Alarm";
            string collum2 = "ID";
            string collum3 = "Status";

            //Khởi tạo Database và mở kết nối
            SqlConnection sql_conn = new SqlConnection();
            sql_conn.ConnectionString = TableAlarmAddress;
            try
            {
                sql_conn.Open();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            //Ghi trạng thái OFF (IO) của các Alarm
            string sql = " UPDATE " + NameTableAlarm + " SET "
                + collum3 + "= 'IO'"
                + "WHERE "
                + collum2 + "= " + ID;
            using (SqlCommand cmd = new SqlCommand(sql, sql_conn))
            {
                cmd.ExecuteNonQuery();
            }
            sql_conn.Close();
        }
        public static void cmd_NewWarring(int ID)
        {
            #region Warring List
            // ===================Điền danh sách tên các Alarm ở đây===============================
            Warring_ID[0] = "Warring : Vaccum Pick Tray yếu";
            Warring_ID[1] = "Warring : Cửa máy mở";
            Warring_ID[2] = "Warring : Cửa input tray mở";
            Warring_ID[3] = "Warring : Sensor Satefy Off";
            Warring_ID[4] = "Check marking không thể output";
            Warring_ID[5] = "Pick FPCB không thể output";
            Warring_ID[6] = "Warring : NG 100% Cam bottom";
            Warring_ID[7] = "Warring : NG 100% Cam top";
            Warring_ID[8] = "Warring : Robot double call check + pick";
            Warring_ID[9] = "Warring : SS back Box NG Off";
            Warring_ID[10] = "Warring : SS outside xylanh input Off";
            Warring_ID[11] = "Warring : SS inside xylanh output Off";
            Warring_ID[12] = "Warring : SS back align tray input Off";
            Warring_ID[13] = "Warring : SS back align tray output Off";
            Warring_ID[14] = "Warring : Robot Chưa về home";
            Warring_ID[15] = "Warring : SS Lamp Inside Off";
            Warring_ID[16] = "No";
            Warring_ID[17] = "No";
            Warring_ID[18] = "No";
            Warring_ID[29] = "No";
            #endregion
        }

        //=====================Hàm đếm xem số lần Alarm đó xuất hiện trong Database=============
        private static int Count_Alarm(int ID_Count)
        {
            //Khởi tạo Database và mở kết nối
            SqlConnection sql_conn = new SqlConnection();
            sql_conn.ConnectionString = TableAlarmAddress;
            try
            {
                sql_conn.Open();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            //Đếm số lần Alarm có trạng thái ON (I) có ID cần đếm
            string sqlSelect = "SELECT *FROM " + NameTableAlarm + " WHERE Status = 'I' AND ID=" + ID_Count;
            SqlCommand CMD = new SqlCommand(sqlSelect, sql_conn);
            SqlDataReader dr = CMD.ExecuteReader();
            DataTable dt = new DataTable();
            dt.Load(dr);
            sql_conn.Close();
            return dt.Rows.Count;
        }

        // =====================Hiển thị lên data grid view=======================================
        public static void Display(string sqlSelect, DataGridView dtgr)
        {
            //Khởi tạo Database và mở kết nối
            SqlConnection sql_conn = new SqlConnection();
            sql_conn.ConnectionString = TableAlarmAddress;
            try
            {
                sql_conn.Open();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            SqlCommand cmd = new SqlCommand(sqlSelect, sql_conn);
            SqlDataReader dr = cmd.ExecuteReader();
            DataTable dt = new DataTable();
            dt.Load(dr);
            dtgr.DataSource = dt;
            //dtgr.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            sql_conn.Close();
        }
    }
}
