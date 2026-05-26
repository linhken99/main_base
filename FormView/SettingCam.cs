using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.IO;
using System.Windows.Forms;
using Main_Base.Class;
using Main_Base.UseControlData;
using System.Data.SQLite;
using Sunny.UI;

namespace Main_Base.FormView
{
    public partial class SettingCam : UIForm
    {
        public SettingCam()
        {
            InitializeComponent();
        }
        public PLC_SMLP PLC2 = new PLC_SMLP();
        public bool StartedMonitor = false;
        private CancellationTokenSource _cts = new CancellationTokenSource();
        private BindingList<string> ListModelName = new BindingList<string>();
        string _connectionString = "Data Source=|DataDirectory|/SQL_Matrix_Tool.db";
        SQLiteConnection Conn1 = new SQLiteConnection();
        private bool LoadDataBaseSettingCam = false;
        private bool LoadModel = false;
        public bool IsConncetPLCSettingCam
        {
            get
            {
                if (PLC2.IsConnected2 == false)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }
        public bool IsLoadDataBaseSettingCam
        {
            get
            {
                if (LoadDataBaseSettingCam == false)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }
        public bool IsLoadModel
        {
            get
            {
                if (LoadModel == false)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }
        #region MessageBox-----------------------------------------------------------------
        private void Message_Box_Error(string content, string caption)
        {
            MessageBox.Show(content, caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        private void Message_Box_Warring(string content, string caption)
        {
            MessageBox.Show(content, caption, MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
        private void Message_Box_OK(string content, string caption)
        {
            MessageBox.Show(content, caption, MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
        }
        #endregion
        #region PLC
        private void button_Jog_Axis_MouseDown(object sender, MouseEventArgs e)
        {
            UIButton btn = sender as UIButton;
            if (btn == null) return;
            string string_type = btn.TagString;
            string slect_Axis = string_type.Substring(0, 3);
            string txtname = string_type;//.Substring(3, string_type.Length - 3);
            var matches = this.Controls.Find(txtname, true);
            if (matches.Length == 0 || !(matches[0] is UITextBox txt_cor)) return;
            int address = Convert.ToInt32(btn.Tag);

            if (slect_Axis == "A0J")
            {
                if (Convert.ToInt32(txt_cor.Text) <= 99)
                {
                    PLC2.Write_DataBit_("M" + (address + Memory_PLC.K200).ToString(), 1);
                }
                else { Message_Box_Error("Giá trị vượt quá 99", "Error Jog Axis"); }
            }
        }
        private void button_Jog_Axis_MouseUp(object sender, MouseEventArgs e)
        {
            UIButton btn = sender as UIButton;
            if (btn == null) return;
            string string_type = btn.TagString;
            string slect_Axis = string_type.Substring(0, 3);
            string txtname = string_type;//.Substring(3, string_type.Length - 3);
            var matches = this.Controls.Find(txtname, true);
            if (matches.Length == 0 || !(matches[0] is UITextBox txt_cor)) return;
            int address = Convert.ToInt32(btn.Tag);

            if (slect_Axis == "A0J")
            {
                PLC2.Write_DataBit_("M" + (address + Memory_PLC.K200).ToString(), 0);

            }
        }
        private void button_Manual_Axis_Click(object sender, EventArgs e)
        {
            UIButton btn = sender as UIButton;
            if (btn == null) return;
            int Maddress = Convert.ToInt32(btn.Tag);
            string type_btn = btn.TagString;
            string type_motion = "";
            if (type_btn.Length > 2)
            {
                type_motion = type_btn.Substring(0, 2);
            }
            int lengt = type_btn.Length;
            try
            {
                if (type_btn == "A" && lengt == 1)
                {
                    PLC2.Write_DataBit_("M" + (Maddress + Memory_PLC.K200), 1);
                }
                else if (type_btn == "A1" && lengt == 2)
                {
                    PLC2.Write_DataBit_("M" + (Maddress + Memory_PLC.K200), 1);
                }
                else if (type_motion == "A2" && lengt >= 3)
                {
                    int D_add_value = Convert.ToInt32(type_btn.Substring(2, 4));
                    string txtname = btn.TagString;
                    var matches = this.Controls.Find(txtname, true);
                    if (matches.Length == 0 || !(matches[0] is UITextBox txt_cor)) return;
                    PLC2.Write_Data_DWord_("D" + (D_add_value + Memory_PLC.K200), Convert.ToInt32(Convert.ToDouble(txt_cor.Text) * 1000));
                    PLC2.Write_DataBit_("M" + (Maddress + Memory_PLC.K200).ToString(), 1);
                }
                else if (type_btn == "M" && lengt == 1)
                {
                    PLC2.Write_DataBit_("M" + (Maddress + Memory_PLC.K1000), 1);
                }
            }
            catch { Message_Box_Error("Không tìm thấy địa chỉ", "Error"); }
        }
        private void button_Save_Axis_Click(object sender, EventArgs e)
        {
            UISymbolButton S_btn = sender as UISymbolButton;
            if (S_btn == null) return;
            int M_address = Convert.ToInt32(S_btn.Tag);
            string tag_string_add = S_btn.TagString;
            string type_btn = tag_string_add.Substring(0, 3);
            string txtName = S_btn.TagString;
            var matches = this.Controls.Find(txtName, true);
            if (matches.Length == 0 || !(matches[0] is UITextBox txt_cor)) return;
            int Add_Value_Cor = Convert.ToInt32(tag_string_add.Substring(3, tag_string_add.Length - 3));
            try
            {
                if (type_btn == "COR")
                {
                    PLC2.Write_DataBit_("M" + (M_address + Memory_PLC.K800).ToString(), 1);
                    Thread.Sleep(100);
                    StatusDisplay.Instance.Update_text3(txt_cor, PLC2.Read_Data_DWord_("D" + (Add_Value_Cor + Memory_PLC.K800).ToString()));
                }
                else if (type_btn == "CO1")
                {
                    PLC2.Write_DataBit_("M" + (M_address + Memory_PLC.K100).ToString(), 1);
                    Thread.Sleep(100);
                    StatusDisplay.Instance.Update_text3(txt_cor, PLC2.Read_Data_DWord_("D" + (Add_Value_Cor + Memory_PLC.K100).ToString()));
                }
            }
            catch { Message_Box_Error("Lưu không thành công" + '\r' + "Xem lại kết nối PLC", "Error"); }
        }
        private void button_Cylinder_Click(object sender, EventArgs e)
        {
            UIButton btn = sender as UIButton;
            if (btn == null) return;
            int address_number = Convert.ToInt32(btn.Tag);
            try
            {
                PLC2.Write_DataBit_("M" + (address_number + Memory_PLC.K500), 1);
            }
            catch { Message_Box_Error("Không tìm thấy địa chỉ", "Error"); }
        }
        private void Save_WriteAllText(string content, string file_name)
        {
            string path = AppDomain.CurrentDomain.BaseDirectory;
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            string filePath = Path.Combine(path, file_name);
            if (!File.Exists(filePath))
            {
                File.Create(filePath).Dispose();
            }
            File.WriteAllText(filePath, content);
        }
        private void btn_Connect_Click(object sender, EventArgs e)
        {
            try
            {
                Save_WriteAllText(txt_IP_PLC.Text + ";" + Port_PLC.Text, "TCPIP2.txt");
                MessageBox.Show("Lưu thành công", "Save", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
            }
            catch { MessageBox.Show("Lỗi khi lưu đị+a chỉ TCP/IP", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); }
            PLC2.Port = Convert.ToInt16(Port_PLC.Text);
            PLC2.connect2();
            if (!PLC2.IsConnected2)
            {
                Message_Box_Error("Disconnect PLC", "PLC");
            }
            else
            {
                Message_Box_OK("Connect PLC OK!", "PLC");
            }
        }
        public void ConnectPLC()
        {
            try
            {
                string Content = Read_File("TCPIP2.txt");
                string[] IP = Content.Split(';');
                StatusDisplay.Instance.Update_IPtext(txt_IP_PLC, IP[0].ToString());
                StatusDisplay.Instance.Update_text(Port_PLC, Convert.ToInt16(IP[1]));
                PLC2.Port = Convert.ToInt16(IP[1]);
                PLC2.connect2();
                if (!PLC2.IsConnected2)
                {
                    Message_Box_Error("Disconnect PLC", "PLC");
                }
                else
                {
                    Message_Box_OK("Connect PLC OK!", "PLC");
                }
            }
            catch (Exception ex) { Message_Box_Error(ex.ToString() + "\r\n" + "Systemp : Connect PLC Fail", "Connect PLC" + "\r\n" + "File TCP/IP Fail"); }
        }
        public async void start_monitor()
        {
            _cts = new CancellationTokenSource();
            await Task.Run(() => Monitor(_cts.Token));
        }
        private void stop_Monitoring()
        {
            _cts.Cancel();
        }
        private async Task Monitor(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                if (PLC2.IsConnected2 == true && Global.home_start == 0 && StartedMonitor == true)
                {
                    await Task.Delay(50);
                    //Manual
                    VaribalePLC.CamM1 = PLC2.ReadRandomBit("M" + (14400 + Memory_PLC.K1000).ToString(), 18);
                    //Teaching
                    VaribalePLC.CamWord = PLC2.Read_Word_Arr("D" + (2060 + Memory_PLC.K500).ToString(), 2);
                    VaribalePLC.CamDWord = PLC2.read_DWord_AR("D" + (2062 + Memory_PLC.K500).ToString(), 2);
                    for (int i = 0; i < 18; i++)
                    {
                        if (i < 4)
                        {
                            UIButton btn1 = this.Controls.Find("G2_T_" + i, true).FirstOrDefault() as UIButton;
                            if (btn1 != null) { StatusDisplay.Instance.STT_Button_Display_SV1(btn1, VaribalePLC.CamM1[i]); }
                        }
                        else if (i > 3 && i < 10)
                        {
                            UISymbolLabel btn1 = this.Controls.Find("G2_T_" + i, true).FirstOrDefault() as UISymbolLabel;
                            if (btn1 != null) { StatusDisplay.Instance.STT_SybolUILabel1(btn1, VaribalePLC.CamM1[i]); }
                        }
                        else if (i == 10 || i == 11)
                        {
                            UIButton sybl = this.Controls.Find("G2_T_" + i, true).FirstOrDefault() as UIButton;
                            if (sybl != null) { StatusDisplay.Instance.STT_Button_Display_SV1(sybl, VaribalePLC.CamM1[i]); }
                        }
                        else if (i == 12 || i == 13)
                        {
                            UISymbolButton sybl = this.Controls.Find("G2_T_" + i, true).FirstOrDefault() as UISymbolButton;
                            if (sybl != null) { StatusDisplay.Instance.STT_Button_Display_Control3(sybl, VaribalePLC.CamM1[i]); }
                        }
                        else if (i > 13)
                        {
                            Label lab = this.Controls.Find("G2_T_" + i, true).FirstOrDefault() as Label;
                            if (lab != null) { StatusDisplay.Instance.STT_Sensor1(lab, VaribalePLC.CamM1[i]); }
                        }

                    }
                    for (int j = 0; j < 4; j++)
                    {
                        UITextBox txt2 = this.Controls.Find("G2_Text_" + j, true).FirstOrDefault() as UITextBox;
                        if (j < 2)
                        {
                            if (txt2 != null) { StatusDisplay.Instance.Update_text(txt2, VaribalePLC.CamWord[j]); }
                        }
                        else
                        {
                            if (txt2 != null) { StatusDisplay.Instance.Update_text3(txt2, VaribalePLC.CamDWord[j - 2]); }
                        }

                        //jog
                        if (A0JA1.Text != "")
                        {
                            PLC2.Write_Data_DWord_("D" + (8108 + Memory_PLC.K200).ToString(), Convert.ToInt32(A0JA1.Text));
                        }
                        if (A0JA2.Text != "")
                        {
                            PLC2.Write_Data_DWord_("D" + (8208 + Memory_PLC.K200).ToString(), Convert.ToInt32(A0JA2.Text));
                        }
                    }
                }
            }
        }
        private void SettingCam_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            StartedMonitor = false;
            stop_Monitoring();
            this.Hide();
        }
        private string Read_File(string file_name)
        {
            string content = "";
            string path = AppDomain.CurrentDomain.BaseDirectory;
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            string filePath = Path.Combine(path, file_name);
            if (!File.Exists(filePath))
            {
                File.Create(filePath).Dispose();
            }
            content = File.ReadAllText(path + Path.DirectorySeparatorChar + file_name);
            return content;
        }
        private void GetParameter1_Click(object sender, EventArgs e)
        {
            try
            {
                PLC2.Write_Data_DWord_("D" + (8112 + Memory_PLC.K200).ToString(), (Convert.ToInt32(Creep1.Text)));
                PLC2.Write_Data_DWord_("D" + (8110 + Memory_PLC.K200).ToString(), (Convert.ToInt32(SPHome1.Text)));
                PLC2.Write_Data_DWord_("D" + (8114 + Memory_PLC.K200).ToString(), (Convert.ToInt32(SPAuto1_1.Text)));
                PLC2.Write_Data_DWord_("D" + (8116 + Memory_PLC.K200).ToString(), (Convert.ToInt32(SPAuto2_1.Text)));
                PLC2.Write_Data_DWord_("D" + (8118 + Memory_PLC.K200).ToString(), (Convert.ToInt32(SPAuto3_1.Text)));
                PLC2.Write_Data_DWord_("D" + (8174 + Memory_PLC.K200).ToString(), (Convert.ToInt32(Acc11.Text)));
                PLC2.Write_Data_DWord_("D" + (8176 + Memory_PLC.K200).ToString(), (Convert.ToInt32(Dec11.Text)));
                PLC2.Write_Data_DWord_("D" + (8178 + Memory_PLC.K200).ToString(), (Convert.ToInt32(Acc12.Text)));
                PLC2.Write_Data_DWord_("D" + (8180 + Memory_PLC.K200).ToString(), (Convert.ToInt32(Dec12.Text)));
                PLC2.Write_Data_DWord_("D" + (8182 + Memory_PLC.K200).ToString(), (Convert.ToInt32(Acc13.Text)));
                PLC2.Write_Data_DWord_("D" + (8184 + Memory_PLC.K200).ToString(), (Convert.ToInt32(Dec13.Text)));
                UIMessageBox.Show("Save thành công", "Save Parameter");
            }
            catch { }
        }
        private void GetParameter2_Click(object sender, EventArgs e)
        {
            try
            {
                PLC2.Write_Data_DWord_("D" + (8212 + Memory_PLC.K200).ToString(), (Convert.ToInt32(Creep2.Text)));
                PLC2.Write_Data_DWord_("D" + (8210 + Memory_PLC.K200).ToString(), (Convert.ToInt32(Home2.Text)));
                PLC2.Write_Data_DWord_("D" + (8214 + Memory_PLC.K200).ToString(), (Convert.ToInt32(SAuto21.Text)));
                PLC2.Write_Data_DWord_("D" + (8216 + Memory_PLC.K200).ToString(), (Convert.ToInt32(SAuto22.Text)));
                PLC2.Write_Data_DWord_("D" + (8218 + Memory_PLC.K200).ToString(), (Convert.ToInt32(SAuto23.Text)));
                PLC2.Write_Data_DWord_("D" + (8274 + Memory_PLC.K200).ToString(), (Convert.ToInt32(Acc21.Text)));
                PLC2.Write_Data_DWord_("D" + (8276 + Memory_PLC.K200).ToString(), (Convert.ToInt32(Dec21.Text)));
                PLC2.Write_Data_DWord_("D" + (8278 + Memory_PLC.K200).ToString(), (Convert.ToInt32(Acc22.Text)));
                PLC2.Write_Data_DWord_("D" + (8280 + Memory_PLC.K200).ToString(), (Convert.ToInt32(Dec22.Text)));
                PLC2.Write_Data_DWord_("D" + (8282 + Memory_PLC.K200).ToString(), (Convert.ToInt32(Acc23.Text)));
                PLC2.Write_Data_DWord_("D" + (8284 + Memory_PLC.K200).ToString(), (Convert.ToInt32(Dec23.Text)));
                UIMessageBox.Show("Save thành công", "Save Parameter");
            }
            catch { }
        }
        #endregion
        #region SQL
        public void ConnectSQLite()
        {
            if (Conn1.State == ConnectionState.Open)
                return;
            Conn1.ConnectionString = _connectionString;
            Conn1.Open();
        }
        public void DisConSQLite()
        {
            Conn1.Close();
        }
        #endregion
        #region Model
        private void btn_InsertModel_Click(object sender, EventArgs e)
        {
            Add_NewModel();
        }
        private void SettingCam_Load(object sender, EventArgs e)
        {
            //LoadData();
            Load_Model();
            //Combox_Model.DropDownStyle = UIDropDownStyle.DropDownList;           
        }

        private void Combox_Model_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Combox_Model.SelectedItem == null)
                return;
            string selected = Combox_Model.SelectedItem.ToString();
            if (selected == "➕ Add New Model...")
            {
                Add_NewModel();
            }
        }
        private void Add_NewModel()
        {
            using (NewModel newmodel = new NewModel())
            {
                if (newmodel.ShowDialog() == DialogResult.OK)
                    try
                    {
                        string NewModelName = Global.ModelName;
                        ConnectSQLite();
                        bool Check = IsModelExist(NewModelName);
                        if (Check == true) { UIMessageBox.ShowError2("Trùng tên model!", false, 0); }
                        else
                        {
                            ; string saveposSQL = string.Format("INSERT INTO Model (ID,Name) " + "VALUES (@ID,@Name)");
                            using (SQLiteCommand cmd = new SQLiteCommand(saveposSQL, Conn1))
                            {
                                cmd.Parameters.AddWithValue("@ID", ListModelName.Count);
                                cmd.Parameters.AddWithValue("@Name", NewModelName);
                                cmd.ExecuteNonQuery();
                                ListModelName.Insert(ListModelName.Count - 1, NewModelName);
                                Combox_Model.SelectedItem = NewModelName;
                                UIMessageBox.ShowSuccess2("Thêm Model thành công!", true, 0);
                            }
                            DisConSQLite();
                        }
                    }
                    catch (Exception ex) { UIMessageBox.ShowError2(ex.ToString(), false, 0); }
            }
        }
        private bool IsModelExist(string modelName)
        {
            using (var cmd = new SQLiteCommand("SELECT 1 FROM Model WHERE LOWER(Name) = LOWER(@name) LIMIT 1", Conn1))
            {
                cmd.Parameters.AddWithValue("@name", modelName.Trim());
                return cmd.ExecuteScalar() != null;
            }
        }
        #endregion
        #region Load
        public void LoadData()
        {
            //IP
            try
            {
                string content = Read_File("TCPIP2.txt");
                string[] ipp = content.Split(';');
                StatusDisplay.Instance.Update_IPtext(txt_IP_PLC, ipp[0].ToString());
                StatusDisplay.Instance.Update_text2(Port_PLC, ipp[1].ToString());
            }
            catch (Exception ex)
            {
                UIMessageBox.ShowError2(ex.ToString() + "\r\n" + "Read File TCP_2", false, 0);
            }
            //
            if (PLC2.IsConnected2 == true)
            {
                //Position A1
                StatusDisplay.Instance.Update_text3(COR8135, PLC2.Read_Data_DWord_("D" + (8135 + Memory_PLC.K200).ToString()));
                //Position A2                                   
                StatusDisplay.Instance.Update_text3(COR8235, PLC2.Read_Data_DWord_("D" + (8235 + Memory_PLC.K200).ToString()));
                //speed jog
                StatusDisplay.Instance.Update_text(A0JA1, PLC2.Read_Data_DWord_("D" + (8108 + Memory_PLC.K200).ToString()));//axis 10
                StatusDisplay.Instance.Update_text(A0JA2, PLC2.Read_Data_DWord_("D" + (8208 + Memory_PLC.K200).ToString()));//axis 11
                                                                                                                            //parameter 1
                StatusDisplay.Instance.Update_text(Creep1, PLC2.Read_Data_DWord_("D" + (8112 + Memory_PLC.K200).ToString()));
                StatusDisplay.Instance.Update_text(SPHome1, PLC2.Read_Data_DWord_("D" + (8110 + Memory_PLC.K200).ToString()));
                StatusDisplay.Instance.Update_text(SPAuto1_1, PLC2.Read_Data_DWord_("D" + (8114 + Memory_PLC.K200).ToString()));
                StatusDisplay.Instance.Update_text(SPAuto2_1, PLC2.Read_Data_DWord_("D" + (8116 + Memory_PLC.K200).ToString()));
                StatusDisplay.Instance.Update_text(SPAuto3_1, PLC2.Read_Data_DWord_("D" + (8118 + Memory_PLC.K200).ToString()));
                StatusDisplay.Instance.Update_text(Acc11, PLC2.Read_Data_DWord_("D" + (8174 + Memory_PLC.K200).ToString()));
                StatusDisplay.Instance.Update_text(Dec11, PLC2.Read_Data_DWord_("D" + (8176 + Memory_PLC.K200).ToString()));
                StatusDisplay.Instance.Update_text(Acc12, PLC2.Read_Data_DWord_("D" + (8178 + Memory_PLC.K200).ToString()));
                StatusDisplay.Instance.Update_text(Dec12, PLC2.Read_Data_DWord_("D" + (8180 + Memory_PLC.K200).ToString()));
                StatusDisplay.Instance.Update_text(Acc13, PLC2.Read_Data_DWord_("D" + (8182 + Memory_PLC.K200).ToString()));
                StatusDisplay.Instance.Update_text(Dec13, PLC2.Read_Data_DWord_("D" + (8184 + Memory_PLC.K200).ToString()));
                //parameter 2
                StatusDisplay.Instance.Update_text(Creep2, PLC2.Read_Data_DWord_("D" + (8212 + Memory_PLC.K200).ToString()));
                StatusDisplay.Instance.Update_text(Home2, PLC2.Read_Data_DWord_("D" + (8210 + Memory_PLC.K200).ToString()));
                StatusDisplay.Instance.Update_text(SAuto21, PLC2.Read_Data_DWord_("D" + (8214 + Memory_PLC.K200).ToString()));
                StatusDisplay.Instance.Update_text(SAuto22, PLC2.Read_Data_DWord_("D" + (8216 + Memory_PLC.K200).ToString()));
                StatusDisplay.Instance.Update_text(SAuto23, PLC2.Read_Data_DWord_("D" + (8218 + Memory_PLC.K200).ToString()));
                StatusDisplay.Instance.Update_text(Acc21, PLC2.Read_Data_DWord_("D" + (8274 + Memory_PLC.K200).ToString()));
                StatusDisplay.Instance.Update_text(Dec21, PLC2.Read_Data_DWord_("D" + (8276 + Memory_PLC.K200).ToString()));
                StatusDisplay.Instance.Update_text(Acc22, PLC2.Read_Data_DWord_("D" + (8278 + Memory_PLC.K200).ToString()));
                StatusDisplay.Instance.Update_text(Dec22, PLC2.Read_Data_DWord_("D" + (8280 + Memory_PLC.K200).ToString()));
                StatusDisplay.Instance.Update_text(Acc23, PLC2.Read_Data_DWord_("D" + (8282 + Memory_PLC.K200).ToString()));
                StatusDisplay.Instance.Update_text(Dec23, PLC2.Read_Data_DWord_("D" + (8284 + Memory_PLC.K200).ToString()));
                //
                LoadDataBaseSettingCam = true;
            }
            else
            {
                LoadDataBaseSettingCam = false;
            }
        }
        public void Load_Model()
        {
            ListModelName.Clear();
            try
            {
                ConnectSQLite();
                using (var cmd = new SQLiteCommand("SELECT Name FROM Model ORDER BY Name", Conn1))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        ListModelName.Add(reader.GetString(0));
                    }
                }
                ListModelName.Add("➕ Add New Model...");
                Combox_Model.DataSource = ListModelName;
                Combox_Model.DropDownStyle = UIDropDownStyle.DropDownList;
                LoadModel = true;
            }
            catch (Exception ex)
            {
                LoadModel = false;
                UIMessageBox.ShowError2(ex.ToString() + "\r\n" + "Error Load Model", false, 0);
            }
        }
        #endregion    
    }
}

