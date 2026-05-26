using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using System.Diagnostics;
using System.Data.SQLite;
using System.IO;
using SimpleTCP;
using System.Net;
using Sunny.UI;
using System.Globalization;
using ClosedXML.Excel;
using Microsoft.WindowsAPICodePack.Taskbar;
using Main_Base.Class;
using Main_Base.FormView;
using Main_Base.IT;
using Main_Base.Model;
using Main_Base.UseControlData;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;


namespace Main_Base
{
    public partial class Main : UIForm
    {
        public Main()
        {
            InitializeComponent();
            UIHelper.EnableDoubleBuffer(this);
            UIStyles.GlobalFont = false;
            UIStyles.MultiLanguageSupport = true;
            UIStyles.CultureInfo = _cultureInfo;
            UIStyles.Translate();
            Config_Chart();
            getValueOffsetTool.Click += GetValueOffsetTool_Click;
            getValueRunAuto.Click += GetValueRunAuto_Click;
            getValueSatefy.Click += GetValueSatefy_Click;
            getValueDataSheetFPCB.Click += GetValueDataSheetFPCB_Click;
            getValueCheckMarking.Click += GetValueCheckMarking_Click;
            DataGridView_Production.SelectIndexChange += DataGridView_Production_SelectIndexChange;
            client_cam1 = new SimpleTcpClient();
            client_cam1.StringEncoder = System.Text.Encoding.UTF8;
            client_cam1.DataReceived += Client_Datareceived_Cam1;
            client_cam2 = new SimpleTcpClient();
            client_cam2.StringEncoder = System.Text.Encoding.UTF8;
            client_cam2.DataReceived += Client_Datareceived_Cam2;
            Images_Project();
            //          
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            //
            UIHelper.EnableDoubleBuffer(TabControl_All);
            UIHelper.EnableDoubleBuffer(tabControl_PLC);
        }
        //
        private readonly HashSet<TabPage> _initedPages = new HashSet<TabPage>();
        private IDisposable _tabRedrawGuard;
        //
        private static CultureInfo _cultureInfo = CultureInfos.en_US;
        //class
        public PLC_SMLP PLC1 = new PLC_SMLP();
        public Function_Robot FuncRobot = new Function_Robot();
        public Matrix matrix = new Matrix();
        //private Screen Screen_splash;
        //User Control
        //       
        DataCamTop show_data_cam_top = new DataCamTop();
        SettingCam SettingCamera = new SettingCam();
        DataOffsetVisionBottom Show_data_offset_ZVisionBottom = new DataOffsetVisionBottom();
        ////
        string _connectionString = "Data Source=|DataDirectory|/SQL_Matrix_Tool.db";
        SQLiteConnection Conn = new SQLiteConnection();
        //*
        SimpleTcpClient client_cam1 = new SimpleTcpClient();
        SimpleTcpClient client_cam2 = new SimpleTcpClient();
        //Thread
        Thread Start, Home;
        int delaypage = 100;
        //
        private string file_process_auto = "ProcessAuto.txt";
        private bool bool_Check_tool_after_place = false;
        private bool Mov_Pick_bool = false;
        private bool OK_NG_NA = false;
        private Stopwatch timer_all = new Stopwatch();
        private Stopwatch Cycle_time_pick = new Stopwatch();
        private Stopwatch Cycle_time_vision = new Stopwatch();
        private Stopwatch Cycle_time_place = new Stopwatch();
        private Stopwatch Cycle_time_NA = new Stopwatch();
        private bool _isClosing = false;
        //
        #region Variable-------------------------------------------------------------------
        string IP_Robot = "";
        int handle = -1, click_DO;
        int[] count_item = new int[4];
        int[] Vaccum_Tool1 = new int[10];
        int[] Vaccum_Tool2 = new int[10];
        string data_read_sever1 = "";
        string data_read_sever2 = "";
        int stop_change = 0;
        int pause_rb = 0;
        int loading_TF_tray = 0;
        int stop_index = 0;
        int start_home = 0;
        int n = 0, ind_n1 = 0, ind_n2 = 0, ind_n3 = 0;
        int flag_switch, click_Vaccum, click_blow, select_air1, ss_satefy;
        bool text_curr_FPCB = false;
        double[] current_satefy = new double[6];
        double[] check_Pos_curr = new double[6];
        double[] check_pos = new double[6];
        List<double>[] dataList = new List<double>[50];
        List<double>[] dataList_E_MACHINE = new List<double>[50];
        List<Record> allData = new List<Record>();
        int Scan_Position = 0;
        int T_P1_T1, T_P1_T2, T_P1_T3, T_P1_T4, T_P2_T1, T_P2_T2, T_P2_T3, T_P2_T4, T_P3_T1, T_P3_T2, T_P3_T3, T_P3_T4, T_P4_T1, T_P4_T2, T_P4_T3, T_P4_T4, T_lamp_White, T_Lamp_Blue, T_Tha_NG;
        double Z1_1, Z1_2, Z1_3, Z1_4, Z1_5, Z1_6, Z1_7, Z1_8, Z1_9, Z1_10, Z1_11, Z1_12, Z1_13, Z1_14, Z1_15, Z1_16, Z1_17, Z1_18, Z1_19, Z1_20;
        double Z2_1, Z2_2, Z2_3, Z2_4, Z2_5, Z2_6, Z2_7, Z2_8, Z2_9, Z2_10, Z2_11, Z2_12, Z2_13, Z2_14, Z2_15, Z2_16, Z2_17, Z2_18, Z2_19, Z2_20;
        double Z3_1, Z3_2, Z3_3, Z3_4, Z3_5, Z3_6, Z3_7, Z3_8, Z3_9, Z3_10, Z3_11, Z3_12, Z3_13, Z3_14, Z3_15, Z3_16, Z3_17, Z3_18, Z3_19, Z3_20;
        double Z4_1, Z4_2, Z4_3, Z4_4, Z4_5, Z4_6, Z4_7, Z4_8, Z4_9, Z4_10, Z4_11, Z4_12, Z4_13, Z4_14, Z4_15, Z4_16, Z4_17, Z4_18, Z4_19, Z4_20;
        #endregion
        #region Form-----------------------------------------------------------------------      
        private void Main_Load(object sender, EventArgs e)
        {
            //originalFormSize = this.Size;           
        }
        private void Main_FormClosed(object sender, FormClosedEventArgs e)
        {
            //StatusDisplay.Instance.IsFormOpen = false;
            //Application.Exit();
        }
        private async void Main_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_isClosing) return;
            e.Cancel = true;
            _isClosing = true;
            StatusDisplay.Instance.IsFormOpen = false;
            var task = UpdateCloseServer();
            var timeout = Task.Delay(1000);
            var completed = await Task.WhenAny(task, timeout);
            Application.Exit();
        }
        private void Matrix_Display_Result_CamBot()
        {
            //int rowD = Global.RowDisplay;
            //int columnD = Global.ColumnDisplay/ Global.RowDisplay;
            int rowD = Convert.ToInt16(Row_input.Text);
            int columnD = Convert.ToInt16(Column_input.Text);
            Global.Row_Jig_input = rowD;
            Global.Column_Jig_input = columnD;
            if (rowD == 0 || columnD == 0) return;
            int startX = 3;
            int startY = 4;
            Panel_Result1.Controls.Clear();
            for (int i = 0; i < columnD; i++)
            {
                for (int j = 0; j < rowD; j++)
                {
                    UILabel lbl = new UILabel();

                    if (i == 0)
                    {
                        lbl.Name = $"lbl_B{j + 1}";
                        lbl.Text = $"{j + 1}";
                    }
                    else
                    {
                        lbl.Name = $"lbl_B{j + rowD + 1}";
                        lbl.Text = $"{j + rowD + 1}";
                    }
                    lbl.Size = new Size((Panel_Result1.Width - 10) / rowD, (Panel_Result1.Height - 10) / columnD);
                    if (i == 0)
                    {
                        lbl.Location = new Point(startX + j * (Panel_Result1.Width - 5) / rowD, startY);
                    }
                    else
                    {
                        lbl.Location = new Point(startX + j * (Panel_Result1.Width - 5) / rowD, startY + (Panel_Result1.Height - 5) / columnD);
                    }
                    lbl.Font = new Font("Segoe UI", 8, FontStyle.Bold);
                    lbl.BorderStyle = BorderStyle.FixedSingle;
                    lbl.TextAlign = ContentAlignment.MiddleCenter;
                    lbl.ForeColor = Color.Black;
                    lbl.BackColor = Color.WhiteSmoke;
                    lbl.AutoSize = false;
                    lbl.Margin = new Padding(5);
                    Panel_Result1.Controls.Add(lbl);
                }
            }
        }
        private void Matrix_Display_Result_CamTop()
        {
            int rowD = Convert.ToInt16(Row_input.Text);
            int columnD = Convert.ToInt16(Column_input.Text);
            Global.Row_Jig_input = rowD;
            Global.Column_Jig_input = columnD;
            int startX = 3;
            int startY = 4;
            Panel_Result2.Controls.Clear();
            for (int i = 0; i < columnD; i++)
            {
                for (int j = 0; j < rowD; j++)
                {
                    UILabel lbl = new UILabel();

                    if (i == 0)
                    {
                        lbl.Name = $"lbl_T{j + 1}";
                        lbl.Text = $"{j + 1}";
                    }
                    else
                    {
                        lbl.Name = $"lbl_T{j + rowD + 1}";
                        lbl.Text = $"{j + rowD + 1}";
                    }
                    lbl.Size = new Size((Panel_Result2.Width - 10) / rowD, (Panel_Result2.Height - 10) / columnD);
                    if (i == 0)
                    {
                        lbl.Location = new Point(startX + j * (Panel_Result2.Width - 6) / rowD, startY);
                    }
                    else
                    {
                        lbl.Location = new Point(startX + j * (Panel_Result2.Width - 6) / rowD, startY + (Panel_Result2.Height - 6) / columnD);
                    }
                    lbl.Font = new Font("Segoe UI", 8, FontStyle.Bold);
                    lbl.BorderStyle = BorderStyle.FixedSingle;
                    lbl.TextAlign = ContentAlignment.MiddleCenter;
                    lbl.ForeColor = Color.Black;
                    lbl.BackColor = Color.WhiteSmoke;
                    lbl.AutoSize = false;
                    lbl.Margin = new System.Windows.Forms.Padding(3, 3, 3, 3);
                    Panel_Result2.Controls.Add(lbl);
                }
            }
        }
        private void Save_Info_jigInput(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                UITextBox txt = sender as UITextBox;
                if (txt == null) return;
                try
                {
                    int add = Convert.ToInt32(txt.Tag.ToString());
                    int value = Convert.ToInt32(txt.Text);

                    PLC1.Write_Data_DWord_("D" + (add + Memory_PLC.K1000), value);
                    Matrix_Display_Result_CamBot();
                    Matrix_Display_Result_CamTop();
                    Message_Box_OK("OK", "Transfer data");
                    this.ActiveControl = null;
                }
                catch (Exception ex)
                {
                    Message_Box_Error(ex.ToString(), "Data Write PLC");
                }
            }
        }
        private void Main_Shown(object sender, EventArgs e)
        {
            comboBox_Mode.SelectedIndex = Global.combox_mode;
            load_option_camera();
            Connect_sever_Cam1();
            Connect_sever_Cam2();
            Matrix_Display_Result_CamBot();
            Matrix_Display_Result_CamTop();
            InitAlarmEvent1();
            Load_Name_Model();
            //Thread monitor_ = new Thread(monitor_all);
            //monitor_.Start();
            _ = Task.Run(monitor_all);
            _ = Task.Run(UpdateStatusServer);
            _ = Task.Run(MES);
        }
        public void Check_Double_Application()
        {
            int totalApp = 0;
            string[] appsToCheck = { "Final" };
            appsToCheck[0] = Process.GetCurrentProcess().ProcessName;
            foreach (var appName in appsToCheck)
            {
                // Tìm tiến trình có tên ứng dụng trong danh sách
                Process[] processes = Process.GetProcessesByName(appName);
                if (processes.Length > 0)
                {
                    totalApp = processes.Length;
                }
                if (totalApp > 1)
                {
                    Message_Box_Error("Bật quá nhiều App điều khiển" + "\r" + "Sau đây sẽ là tiến trình tự động tắt hết app trên Windown" + "\r" + "Sau khi App được đóng xin vui lòng kiểm tra lại trong hộp Task Manager!", "Application");

                    foreach (var process in processes)
                    {
                        try
                        {
                            process.Kill();
                            process.WaitForExit();
                        }
                        catch
                        {
                            Message_Box_Error("Lỗi tắt App tự động bị gián đoạn.Xin vui lòng tắt thủ công trong hộp thoại Task Manager", "Application");
                        }
                    }
                }
            }
        }
        private void Images_Project()
        {
            string path_image = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Image");
            if (!File.Exists(path_image))
            {
                //logo
                if (pictureBox_cong_ty.Image != null) { pictureBox_cong_ty.Image.Dispose(); }
                pictureBox_cong_ty.Image = Image.FromFile(path_image + "\\synopex_xrotate.gif");
                if (pictureBox_bo_phan.Image != null) { pictureBox_bo_phan.Image.Dispose(); }
                pictureBox_bo_phan.Image = Image.FromFile(path_image + "\\auto2.gif");

                //Main
                if (pictureBox_Machine.Image != null) { pictureBox_Machine.Image.Dispose(); }
                pictureBox_Machine.Image = Image.FromFile(path_image + "\\creo3.JPG");
                //G1
                //if (pictureBox6.Image != null) { pictureBox2.Image.Dispose(); }
                //pictureBox6.Image = Image.FromFile(@path_image + "\\remov.JPG");
                if (pictureBox2.Image != null) { pictureBox5.Image.Dispose(); }
                pictureBox2.Image = Image.FromFile(path_image + "\\remov.JPG");
                //G2
                if (pictureBox7.Image != null) { pictureBox7.Image.Dispose(); }
                pictureBox7.Image = Image.FromFile(path_image + "\\axisintray.JPG");
                if (pictureBox5.Image != null) { pictureBox5.Image.Dispose(); }
                pictureBox5.Image = Image.FromFile(path_image + "\\IN_OUT_Tray.JPG");
                if (pictureBox3.Image != null) { pictureBox3.Image.Dispose(); }
                pictureBox3.Image = Image.FromFile(path_image + "\\axisintray.JPG");
                if (pictureBox4.Image != null) { pictureBox4.Image.Dispose(); }
                pictureBox4.Image = Image.FromFile(path_image + "\\zout.JPG");
                //G3
                //if (pictureBox8.Image != null) { pictureBox8.Image.Dispose(); }
                //pictureBox8.Image = Image.FromFile(path_image + "\\XYZR.JPG");
                if (pictureBox1.Image != null) { pictureBox1.Image.Dispose(); }
                pictureBox1.Image = Image.FromFile(path_image + "\\XYZR.JPG");
            }
        }
        #endregion
        #region Main-----------------------------------------------------------------------
        private void btn_Save_IP_Click(object sender, EventArgs e)
        {
            try
            {
                Save_WriteAllText(txt_IP_Robot.Text + ";" + Port_Robot.Text + ";" + txt_IP_PLC.Text + ";" + Port_PLC.Text + ";" + txt_ip_vision1.Text + ";" + txt_port_vision1.Text + ";" + txt_ip_vision2.Text + ";" + txt_port_vision2.Text, "TCPIP.txt");
                MessageBox.Show("Lưu thành công", "Save", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
            }
            catch { MessageBox.Show("Lỗi khi lưu địa chỉ TCP/IP", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); }

        }
        private void button_Set_Monitor_Process_Click(object sender, EventArgs e)
        {
            UISymbolButton btn = sender as UISymbolButton;
            if (btn == null) return;
            string add_type = btn.Tag.ToString();
            int address_number = Convert.ToInt32(btn.TagString);
            try
            {
                if (add_type == "M")
                {
                    PLC1.Write_DataBit_(add_type + (address_number + Memory_PLC.K1000), 1);
                }
                else if (add_type == "L")
                {
                    PLC1.Write_DataBit_(add_type + (address_number + Memory_PLC.K100), 1);
                }
                else if (add_type == "DO")
                {
                    SDKHrobot.HRobot.set_digital_output(handle, address_number, true);
                }
            }
            catch { Message_Box_Error("Không tìm thấy địa chỉ", "Error"); }
        }
        private void button_Reset_Monitor_Process_Click(object sender, EventArgs e)
        {
            UISymbolButton btn = sender as UISymbolButton;
            if (btn == null) return;
            string add_type = btn.Tag.ToString();
            int address_number = Convert.ToInt32(btn.TagString);
            try
            {
                if (add_type == "M")
                {
                    PLC1.Write_DataBit_(add_type + (address_number + Memory_PLC.K1000), 1);
                }
                else if (add_type == "L")
                {
                    PLC1.Write_DataBit_(add_type + (address_number + Memory_PLC.K100), 0);
                }
                else if (add_type == "DO")
                {
                    SDKHrobot.HRobot.set_digital_output(handle, address_number, false);
                    PLC1.Write_DataBit_("L" + (95 + Memory_PLC.K100), 0);
                }
            }
            catch { Message_Box_Error("Không tìm thấy địa chỉ", "Error"); }
        }
        private void button_Reset_Monitor_Process1_Click(object sender, EventArgs e)
        {
            string message_ = "Off Vaccum Tool Robot để ý hàng còn trên tool không? " + "\r\n" + " Còn hàng trên tool Robot off vaccum hứng hàng không để hàng rơi lên tray";
            const string caption_ = "Vaccum Robot Robot";
            var result = MessageBox.Show(message_, caption_, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                SDKHrobot.HRobot.set_digital_output(handle, 51, false);
                PLC1.Write_DataBit_("M" + (9081 + Memory_PLC.K1000).ToString(), 1);
            }
        }
        private async void btn_Auto_Click(object sender, EventArgs e)
        {
            if (SDKHrobot.HRobot.get_digital_input(handle, 53) == 1)
            {
                comboBox_Mode.Enabled = false;
                if (checkBox_camtop.Checked == false)
                {
                    try
                    {
                        Connect_sever_Cam1();
                        Connect_sever_Cam2();

                        if (client_cam1.TcpClient.Connected == false && client_cam2.TcpClient.Connected == false)
                        {
                            Message_Box_Error("Kết nối không thành công Camera", "Connect Camera");
                        }

                    }
                    catch { }
                }
                if (Scan_Position == 2 && Global.Home_All == true && ((client_cam1.TcpClient.Connected == true && client_cam2.TcpClient.Connected == true) || checkBox_camtop.Checked == true))
                {
                    #region auto
                    StatusDisplay.Instance.Update_process(txt_process, "Auto Mode");
                    if (SDKHrobot.HRobot.get_digital_input(handle, 5) == 1)
                    {
                        SDKHrobot.HRobot.set_digital_output(handle, 3, false);
                        Mov_Pick_bool = false;
                        FuncRobot.CMD_Pick_Output = false;
                        SDKHrobot.HRobot.set_digital_output(handle, 3, false);
                        PLC1.Write_DataBit_("L" + (95 + Memory_PLC.K100).ToString(), 0);
                        FuncRobot.CMD_Check_Marking = true;
                    }
                    else
                    {
                        FuncRobot.CMD_Check_Marking = false;
                    }
                    if (SDKHrobot.HRobot.get_digital_output(handle, 3) == 1)
                    {
                        Mov_Pick_bool = true;
                        FuncRobot.CMD_Check_Marking = false;
                    }
                    else
                    {
                        Mov_Pick_bool = false;
                    }
                    PLC1.Write_DataBit_("M" + (5000 + Memory_PLC.K40), 1);
                    Global.home_start = 0;
                    Global.auto_ = 1;
                    text_curr_FPCB = false;
                    //btn_Login.Enabled = false;
                    if (SDKHrobot.HRobot.get_digital_output(handle, 51) == 1 && SDKHrobot.HRobot.get_digital_output(handle, 8) == 0 && SDKHrobot.HRobot.get_digital_output(handle, 3) == 1)
                    {
                        PLC1.Write_DataBit_("L" + (94 + Memory_PLC.K100), 1);
                        SDKHrobot.HRobot.set_digital_output(handle, 3, false);
                        PLC1.Write_DataBit_("M" + (9440 + Memory_PLC.K1000).ToString(), 1);//rst buffer output
                    }
                    else if (SDKHrobot.HRobot.get_digital_output(handle, 51) == 1 && SDKHrobot.HRobot.get_digital_output(handle, 3) == 0)
                    {
                        Global.result_Cam_Bot = PLC1.Read_Word_Arr("D" + (9250 + Memory_PLC.K2000), Global.Number_Tool);
                    }
                    //Global.Row_tray = PLC1.Read_Data_Word_("D" + (3002 + Memory_PLC.K1000).ToString());
                    //Global.Column_tray = PLC1.Read_Data_Word_("D" + (3003 + Memory_PLC.K1000).ToString());
                    //Global.Column_tray = Convert.ToInt16(txt_Column_tray.Text);
                    #endregion
                    await Task.Run(() =>
                     {
                         Status_btn_Login(61475);
                     });
                    // btn_Start.Enabled = true;
                }
                else if (client_cam1.TcpClient.Connected == false || client_cam2.TcpClient.Connected == false)
                {
                    Message_Box_Warring("Kiểm tra lại Connect Vision Top", "Vision");
                }
                else if (Scan_Position != 2)
                {
                    Message_Box_Warring("Kiểm tra lại tọa độ Position Place_Tray_Start trong Teaching 1 Robot", "Position");
                }
            }
            else { Message_Box_Warring("Origin Machine Fail", "OPR Machine"); }
        }
        private void btn_Start_Click(object sender, EventArgs e)
        {
            if (Global.Start_Start == 0)
            {
                btn_Clear.Enabled = false;
                btn_Origin.Enabled = false;
                if (SDKHrobot.HRobot.get_digital_input(handle, 49) == 1 && (SDKHrobot.HRobot.get_digital_input(handle, 55) == 1 || checkBox_SatefyBehind.Checked == true))
                {
                    if (SDKHrobot.HRobot.get_digital_output(handle, 51) == 1)
                    {
                        StatusDisplay.Instance.Update_process(txt_process, "Start Mode");
                        Global.CycleTime_arr[0] = 0;
                        int check_vaccum = 0;
                        int[] arr_vaccum = PLC1.Read_Word_Arr("D" + (7031 + Memory_PLC.K100).ToString(), Global.Number_Tool);
                        for (int i = 0; i < Global.Number_Tool; i++)
                        {
                            if (arr_vaccum[i] == 1)
                            {
                                check_vaccum++;
                            }
                        }
                        if (check_vaccum == 0)
                        {
                            SDKHrobot.HRobot.set_digital_output(handle, 51, false);
                        }
                    }
                    PLC1.Write_DataBit_("M" + (5003 + Memory_PLC.K40).ToString(), 1);
                    Global.Start_Start = 1;
                    Global.home_start = 0;
                    stop_change = 0;
                    start_home = 0;
                    flag_switch = 0;
                    SDKHrobot.HRobot.set_digital_output(handle, 1, false);
                    if (Global.Home_All == true && SDKHrobot.HRobot.get_digital_input(handle, 49) == 1)
                    {
                        Start = new Thread(Start_On2);
                        Start.Start();
                        DateTime date = DateTime.Now;
                        string[] content = { "Start On", date.ToString("dd/MM/yyyy"), date.ToString("HH:mm:ss") };
                        write_alarm_database(content);
                    }
                }
                else if (SDKHrobot.HRobot.get_digital_input(handle, 55) == 0 && checkBox_SatefyBehind.Checked == false)
                {
                    Message_Box_Warring("Kiem tra sensor satefy Machine", "Satefy");
                }
                Lock_UI_Run();
            }
        }
        private void btn_Pause_Click(object sender, EventArgs e)
        {
            //SDKHrobot.HRobot.motion_hold(handle);
            if (Global.home_start == 1)
            {
                stop_change = 2;
                PLC1.Write_DataBit_("M" + (5004 + Memory_PLC.K40).ToString(), 1);

            }
            if (Global.Start_Start == 1)
            {
                stop_change = 2;
                PLC1.Write_DataBit_("M" + (5004 + Memory_PLC.K40).ToString(), 1);
            }
        }
        private void btn_Stop_Click(object sender, EventArgs e)
        {
            // btn_Auto.Enabled = true;
            btn_Origin.Enabled = true;
            btn_Clear.Enabled = true;
            GB_Monitor_Process.Enabled = true;
            Enable_GB_info_tray_Stop();
            //SDKHrobot.HRobot.motion_abort(handle);
            bool W_ = PLC1.Write_DataBit_("M" + (5006 + Memory_PLC.K40).ToString(), 1);
            Global.home_start = 0;
            Global.Start_Start = 0;
            stop_change = 1;
            FuncRobot.Flag_Cam1 = 0;
            FuncRobot.Flag_Cam2 = 0;
            Global.auto_ = 0;
            start_home = 0;
            flag_switch = 0;
            TrackBar_Jog_Speed_Robot.Value = 10;
            Global.Home_All = false;
            Global.Security_Place = false;
            FuncRobot.Security_Check_Camtop = false;
            FuncRobot.Brake_While = false;
            btn_Login.Enabled = true;
            int spp = SDKHrobot.HRobot.set_override_ratio(handle, 10);
            int DO2 = SDKHrobot.HRobot.set_digital_output(handle, 2, false);
            timer_all = new Stopwatch();
            timer_all.Reset();
            txt_total.Text = "0";
            txt_speed_Jog.Text = "10";
            if (W_ == false)
            {
                Message_Box_Error("Disconnect PLC", "PLC");
            }
            if (spp != 0 || DO2 != 0)
            {
                Message_Box_Error("Disconnect Robot", "Robot");
            }

        }
        private void btn_Stop_MouseDown(object sender, MouseEventArgs e)
        {
            //try
            //{
            //    SDKHrobot.HRobot.motion_hold(handle);
            //}
            //catch { SDKHrobot.HRobot.motion_hold(handle); }
        }
        private void btn_Origin_Click(object sender, EventArgs e)
        {
            if (SDKHrobot.HRobot.get_digital_input(handle, 49) == 0 && SDKHrobot.HRobot.get_digital_input(handle, 50) == 0)
            {
                int check_rb = SDKHrobot.HRobot.set_digital_output(handle, 1, false); //flag origin home rb           
                Global.home_start = 1;
                Global.Start_Start = 0;
                Home = new Thread(home2);
                Home.Start();
                bool check_plc = PLC1.Write_DataBit_("M" + (5005 + Memory_PLC.K40).ToString(), 1);
                //home();
                if (check_plc == false)
                {
                    Message_Box_Error("Disconnect PLC", "PLC");
                }
                if (check_rb != 0)
                {
                    Message_Box_Error("Disconnect Robot", "Robot");
                }
            }
            else
            {
                Message_Box_Warring("Machine Runing", "Warring");
            }
        }
        private void btn_Reset_Click(object sender, EventArgs e)
        {
            if (SDKHrobot.HRobot.get_digital_input(handle, 55) == 1 || checkBox_SatefyBehind.Checked == true)
            {
                PLC1.Write_DataBit_("M" + (5007 + Memory_PLC.K40).ToString(), 1);
                //PLC1.Write_DataBit_("M" + (9016 + Memory_PLC.K1000).ToString(), 0);
                stop_change = 0;
                flag_switch = 0;
                pause_rb = 0;
                ss_satefy = 0;
                SDKHrobot.HRobot.clear_alarm(handle);

                if (Global.home_start == 1)
                {
                    SDKHrobot.HRobot.motion_continue(handle);
                }
                if (Global.Start_Start == 1)
                {
                    SDKHrobot.HRobot.motion_continue(handle);
                }
            }
            else if (SDKHrobot.HRobot.get_digital_input(handle, 55) == 0 && checkBox_SatefyBehind.Checked == false)
            {
                Message_Box_Warring("Check sensor Satefy", "Satefy");
            }
        }
        private void btn_Clear_Click(object sender, EventArgs e)
        {
            StatusDisplay.Instance.Update_process(txt_process, "");
            SDKHrobot.HRobot.set_override_ratio(handle, 10);
            stop_change = 0;
            PLC1.Write_DataBit_("M" + (5030 + Memory_PLC.K1), 1);
            SDKHrobot.HRobot.clear_alarm(handle);
            if (SDKHrobot.HRobot.get_motor_state(handle) == 0)
            {
                SDKHrobot.HRobot.set_motor_state(handle, 1);   // Servo on
            }
            Buzz.Image = Main_Base.Properties.Resources.icons8_sound_50;
            Buzz.Text = "Buzz";
        }
        private void btn_Save_Data_tray_Click(object sender, EventArgs e)
        {
            PLC1.Write_Data_Word_("D" + (3000 + Memory_PLC.K1000).ToString(), Convert.ToInt16(txt_number_tray_curr.Text));
            PLC1.Write_Data_Word_("D" + (3001 + Memory_PLC.K1000).ToString(), Convert.ToInt16(txt_number_tray_output.Text));
            PLC1.Write_Data_Word_("D" + (3002 + Memory_PLC.K1000).ToString(), Convert.ToInt16(txt_Row_tray.Text));
            PLC1.Write_Data_Word_("D" + (3003 + Memory_PLC.K1000).ToString(), Convert.ToInt16(txt_Column_tray.Text));
            Global.Row_tray = Convert.ToInt16(txt_Row_tray.Text);
            Global.Column_tray = Convert.ToInt16(txt_Column_tray.Text);
            Global.Number_Tray_output = Convert.ToInt16(txt_number_tray_curr.Text);
            Message_Box_OK("Lưu thành công", "Save");
        }
        private void btn_Set_Number_FPCB_Tray_Click(object sender, EventArgs e)
        {
            SDKHrobot.HRobot.set_counter(handle, 2, Convert.ToInt16(txt_set_number_fpcb.Text));
            int[] dataset = case_set_number_current_fpcb_tray(Convert.ToInt16(txt_set_number_fpcb.Text));
            SDKHrobot.HRobot.set_counter(handle, 3, dataset[0]);
            SDKHrobot.HRobot.set_counter(handle, 4, dataset[1]);
            SDKHrobot.HRobot.set_counter(handle, 5, dataset[2]);
            SDKHrobot.HRobot.set_counter(handle, 6, 1);
            n = Convert.ToInt16(txt_set_number_fpcb.Text);
            ind_n1 = dataset[0];
            ind_n2 = dataset[1];
            ind_n3 = dataset[2];
            PLC1.Write_Data_Word_("D" + (3010 + Memory_PLC.K1000).ToString(), Convert.ToInt16(txt_set_number_fpcb.Text));
            text_curr_FPCB = false;

        }
        private void btn_reset_tray_Click(object sender, EventArgs e)
        {
            StatusDisplay.Instance.Update_text_toText(txt_Lot_Old, txt_lot);
            PLC1.Write_Data_DWord_("D" + (9022 + Memory_PLC.K2000).ToString(), Convert.ToInt32(txt_Lot_Old.Text));
            PLC1.Write_DataBit_("M" + (9007 + Memory_PLC.K1000).ToString(), 1);
            SDKHrobot.HRobot.set_counter(handle, 2, 1);
            SDKHrobot.HRobot.set_counter(handle, 3, 1);
            SDKHrobot.HRobot.set_counter(handle, 4, 1);
            SDKHrobot.HRobot.set_counter(handle, 5, 1);
            SDKHrobot.HRobot.set_counter(handle, 6, 1);
        }
        private void Btn_resset_count_Click(object sender, EventArgs e)
        {
            string shift = GetShift(DateTime.Now);
            if (Convert.ToInt32(txt_total_input.Text) > 0)
            {
                write_Production_database(txt_total_input.Text, txt_FPCB_OK.Text, txt_FPCB_NG.Text);
                //List<E_MACHINE_RESUTL> _list = new List<E_MACHINE_RESUTL>
                //   {
                //       new E_MACHINE_RESUTL
                //       {
                //           SEQ_ID=Global.SEQ_ID,
                //           MACHINE_NAME=Global.MACHINE_NAME,
                //           GROUP_NO=Global.GROUP_NO,
                //           LINE_NO=Global.LINE_NO,
                //           MODEL=Global.ModelName_Server,
                //           SHIFT=shift,
                //           QTY= Global.SL_Total_Input,
                //           UPH=Global.UPH,
                //           RESET_FLAG="Y",
                //           RESUTL_DT=Global.Timer_rsCounter,
                //           INSRT_DT=Global.Timer_rsCounter,
                //       },
                //   };
                //await IT_SQL_Server_Helper.Instance.InsertListIgnoreDuplicate("E_MACHINE_RESUTL", _list);
                PLC1.Write_DataBit_("L" + (80 + Memory_PLC.K100).ToString(), 1);
            }
        }
        private void btn_Input_Tray_Click(object sender, EventArgs e)
        {
            PLC1.Write_DataBit_("M" + (9003 + Memory_PLC.K1000).ToString(), 1);
        }
        private void Btn_CMD_Pick_Click(object sender, EventArgs e)
        {
            if (Global.Start_Start == 1 && Global.combox_mode == 0)
            {
                //FuncRobot.CMD_Check_Marking = true;
                ////SDKHrobot.HRobot.set_digital_output(handle, 51, true);
                //PLC1.Write_DataBit_("M" + (9085 + Memory_PLC.K1000).ToString(), 1);
            }
            else
            {
                FuncRobot.CMD_Pick = false;
            }
            btn_CMD_Pick.Enabled = false;
        }
        private void ComboBox_Mode_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (InvokeRequired)
            {
                Invoke(new MethodInvoker(() => ComboBox_Mode_SelectedIndexChanged(sender, e)));
                return;
            }

            Global.combox_mode = comboBox_Mode.SelectedIndex;

            try
            {
                ConnectSQLite();
                string saveposSQL = string.Format("INSERT OR REPLACE INTO Mode (STT,Mode) VALUES (@STT,@Mode)");
                using (SQLiteCommand cmd = new SQLiteCommand(saveposSQL, Conn))
                {
                    cmd.Parameters.AddWithValue("@STT", 1);
                    cmd.Parameters.AddWithValue("@Mode", Global.combox_mode);

                    cmd.ExecuteNonQuery();
                }
                DisConSQLite();
            }
            catch { Message_Box_Error("Data Locked" + "?" + "Data Mode Fail", "Mode Run"); }
        }
        private void txt_model_input_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                Save_WriteAllText(txt_model_input.Text, "Model.txt");
                uiLabel_Name_Machine.Text = txt_model_input.Text;
                Global.ModelName_Server = txt_model_input.Text;
                this.ActiveControl = null;
                UITextBox txt = sender as UITextBox;
                if (txt == null) return;
                txt.ForeColor = Color.Navy;
                txt.TextAlignment = System.Drawing.ContentAlignment.MiddleRight;
                txt.Font = new Font(txt.Font.FontFamily, 14, FontStyle.Bold);
            }
        }
        private void Buzz_Click(object sender, EventArgs e)
        {
            Task.Run(() =>
            {
                if (PLC1.Read_Data_Bit_("M" + (9084 + Memory_PLC.K1000).ToString()) == false)
                {
                    PLC1.Write_DataBit_("M" + (9084 + Memory_PLC.K1000).ToString(), 1);
                    Buzz.Image = Main_Base.Properties.Resources.icons8_sound_511;
                    StatusDisplay.Instance.Update_Button_text2(Buzz, "Mute");
                }
                else
                {
                    PLC1.Write_DataBit_("M" + (9084 + Memory_PLC.K1000).ToString(), 0);
                    Buzz.Image = Main_Base.Properties.Resources.icons8_sound_50;
                    StatusDisplay.Instance.Update_Button_text2(Buzz, "Buzz");
                }
            });

        }
        public void Status_btn_Login(int valueSymbol)
        {
            StatusDisplay.Instance.Get_Symbol(btn_Login, valueSymbol);
        }
        public void Enable_GB_info_tray_Stop()
        {
            GB_info_tray.Enabled = true;
            txt_Row_tray.Enabled = false;
            txt_Column_tray.Enabled = false;
        }
        #endregion
        #region Login----------------------------------------------------------------------
        private void Login_Click(object sender, EventArgs e)
        {
            Login login = new Login(this);
            login.ShowDialog();
        }
        public void Lock_Screen()
        {
            Task.Run(() =>
            {
                StatusDisplay.Instance.Enable_Tabcontrol(tabControl_Robot, 0);
                StatusDisplay.Instance.Enable_Tabcontrol(tabControl_PLC, 0);
                StatusDisplay.Instance.Enable_Group_Box(GB_Info_TCP, 0);
                StatusDisplay.Instance.Enable_Group_Box(GB_info_tray, 0);
                StatusDisplay.Instance.Enable_Group_Box(GB_Monitor_Process, 0);
                StatusDisplay.Instance.Enable_Group_Box(GB_Option_Main, 0);
                StatusDisplay.Instance.Enable_Group_Box(uiGroupBox5, 0);
                StatusDisplay.Instance.Enable_Group_Box(uiGroupBox1, 0);
                StatusDisplay.Instance.Enable_Panel(uiPanel20, 0);
            });
        }
        public void Unlock_Action()
        {
            if (Global.Start_Start == 0)
            {
                StatusDisplay.Instance.Enable_Tabcontrol(tabControl_Robot, 1);
                StatusDisplay.Instance.Enable_Group_Box(GB_Info_TCP, 1);
                StatusDisplay.Instance.Enable_Group_Box(GB_info_tray, 1);
                StatusDisplay.Instance.Enable_Uitextbox(txt_Column_tray, 1);
                StatusDisplay.Instance.Enable_Uitextbox(txt_Row_tray, 1);
                StatusDisplay.Instance.Enable_Group_Box(GB_Monitor_Process, 1);
                StatusDisplay.Instance.Enable_Group_Box(GB_Option_Main, 1);
                StatusDisplay.Instance.Enable_Group_Box(uiGroupBox5, 1);
                StatusDisplay.Instance.Enable_Group_Box(uiGroupBox1, 1);
                StatusDisplay.Instance.Enable_Panel(uiPanel20, 1);
                StatusDisplay.Instance.Enable_Combox(comboBox_Mode, 1);
            }
            StatusDisplay.Instance.Enable_Tabcontrol(tabControl_PLC, 1);
            StatusDisplay.Instance.Get_Symbol(btn_Login, 61596);
            Message_Box_OK("Đăng nhập thành công", "Login");
        }
        private void Lock_UI_Run()
        {
            Task.Run(() =>
            {
                StatusDisplay.Instance.Enable_Tabcontrol(tabControl_Robot, 0);
                StatusDisplay.Instance.Enable_Tabcontrol(tabControl_PLC, 0);
                StatusDisplay.Instance.Enable_Group_Box(GB_Info_TCP, 0);
                StatusDisplay.Instance.Enable_Group_Box(GB_info_tray, 0);
                StatusDisplay.Instance.Enable_Group_Box(GB_Monitor_Process, 0);
                StatusDisplay.Instance.Enable_Group_Box(GB_Option_Main, 0);
                StatusDisplay.Instance.Enable_Group_Box(uiGroupBox5, 0);
                StatusDisplay.Instance.Enable_Panel(uiPanel20, 0);
                StatusDisplay.Instance.Get_Symbol(btn_Login, 61475);

            });
        }
        private void Unlock_UI_Stop()
        {
            StatusDisplay.Instance.Enable_UiSymbolButton(btn_Login, 1);
        }
        #endregion
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
        #region SQL------------------------------------------------------------------------
        public void ConnectSQLite()
        {
            Conn.ConnectionString = _connectionString;
            Conn.Open();
        }
        public void DisConSQLite()
        {
            Conn.Close();
        }
        #endregion
        #region File-----------------------------------------------------------------------
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
        private void Save_AppendAllLines(string[] content, string file_name)
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
            File.AppendAllLines(filePath, content);
        }
        private void ExportDataGridViewToExcelWithFolder(DataGridView dgv)
        {
            using (FolderBrowserDialog fbd = new FolderBrowserDialog())
            {
                fbd.Description = "Chọn thư mục để lưu file Excel";

                if (fbd.ShowDialog() == DialogResult.OK)
                {
                    string folderPath = fbd.SelectedPath;
                    //
                    string fileName = "DataProduction_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".xlsx";
                    string filePath = Path.Combine(folderPath, fileName);

                    var workbook = new XLWorkbook();
                    var worksheet = workbook.Worksheets.Add("Data");

                    // Ghi tiêu đề cột
                    for (int i = 0; i < dgv.Columns.Count; i++)
                    {
                        worksheet.Cell(1, i + 1).Value = dgv.Columns[i].HeaderText;
                    }

                    // Ghi dữ liệu dòng
                    for (int i = 0; i < dgv.Rows.Count; i++)
                    {
                        for (int j = 0; j < dgv.Columns.Count; j++)
                        {
                            var value = dgv.Rows[i].Cells[j].Value;
                            worksheet.Cell(i + 2, j + 1).Value = value?.ToString();
                        }
                    }
                    // Lưu
                    workbook.SaveAs(filePath);
                    MessageBox.Show("Đã lưu Excel tại:\n" + filePath, "Xuất thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }
        private void ExportDataGridViewToExcel_SaveAs(string Name)
        {
            using (SaveFileDialog sfd = new SaveFileDialog())
            {
                sfd.Title = "Lưu file Excel";
                sfd.Filter = "Excel Workbook|*.xlsx";
                sfd.FileName = Name + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".xlsx";

                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    ConnectSQLite();
                    string query = string.Format("SELECT * from " + Name);
                    SQLiteDataAdapter adapter = new SQLiteDataAdapter(query, Conn);
                    DataSet dataSet = new DataSet();
                    adapter.Fill(dataSet, Name);
                    DisConSQLite();
                    DataTable dt = dataSet.Tables[Name];
                    string filePath = sfd.FileName;
                    var workbook = new XLWorkbook();
                    var worksheet = workbook.Worksheets.Add("Data");
                    worksheet.Cell(1, 1).InsertTable(dt, true);
                    worksheet.Columns().AdjustToContents();
                    ////cột
                    //for (int i = 0; i < dgv.Columns.Count; i++)
                    //{
                    //    worksheet.Cell(1, i + 1).Value = dgv.Columns[i].HeaderText;
                    //}
                    ////dữ liệu
                    //for (int i = 0; i < dgv.Rows.Count; i++)
                    //{
                    //    for (int j = 0; j < dgv.Columns.Count; j++)
                    //    {
                    //        var value = dgv.Rows[i].Cells[j].Value;
                    //        worksheet.Cell(i + 2, j + 1).Value = value?.ToString();
                    //    }
                    //}
                    // Lưu file
                    workbook.SaveAs(filePath);
                    WaitFormHelper.Close();
                    MessageBox.Show("Xuất file thành công:\n" + filePath, "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }
        private async void File_Production_Excel_Click(object sender, EventArgs e)
        {
            await WaitFormHelper.Show();
            ExportDataGridViewToExcel_SaveAs("Production");
        }
        private async void File_Alarm_Excel_Click(object sender, EventArgs e)
        {
            await WaitFormHelper.Show();
            ExportDataGridViewToExcel_SaveAs("History_Alarm");
        }
        #endregion
        #region Robot Connect--------------------------------------------------------------
        private static SDKHrobot.HRobot.CallBackFun callback;
        public static void EventFun(UInt16 cmd, UInt16 rlt, ref UInt16 Msg, int len)
        {
            Console.WriteLine("Command: " + cmd + " Resault: " + rlt);

            switch (cmd)
            {
                case 4011:
                    if (rlt != 0)
                    {
                        MessageBox.Show("Update fail. " + rlt, "HRSS update callback", MessageBoxButtons.OK, MessageBoxIcon.Warning,
                            MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                    }
                    break;
            }
        }
        public void display_box(string item)
        {
            info_device.Invoke(new MethodInvoker(() =>
            {
                if (item == "Connect Vision Cam1 error" || item == "Connect Vision Cam2 error" || item == "Connect Vision Cam1 Succesful" || item == "Connect Vision Cam2 Succesful")
                {
                    string[] item_fres = new string[4] { "Connect Vision Cam1 error", "Connect Vision Cam2 error", "Connect Vision Cam1 Succesful", "Connect Vision Cam2 Succesful" };
                    int[] index_fres = new int[4];
                    index_fres[0] = info_device.Items.IndexOf(item_fres[0]);
                    index_fres[1] = info_device.Items.IndexOf(item_fres[1]);
                    index_fres[2] = info_device.Items.IndexOf(item_fres[2]);
                    index_fres[3] = info_device.Items.IndexOf(item_fres[3]);

                    for (int id = 0; id < index_fres.Length; id++)
                    {
                        if (index_fres[id] != -1)
                        {
                            count_item[id]++;
                            if (count_item[id] > 1 && count_item[id] < 3)
                            {
                                info_device.Items.RemoveAt(index_fres[id]);
                                count_item[id] = 0;
                            }
                        }
                    }

                }
                //if (item == "Connect Vision Cam1 Succesful" || item == "Connect Vision Cam2 Succesful")
                //{
                //    int index1 = info_device.Items.IndexOf("Connect Vision Cam1 error");
                //    int index2 = info_device.Items.IndexOf("Connect Vision Cam2 error");
                //    if(index1 != -1)
                //    {
                //        info_device.Items.RemoveAt(index1);
                //    }
                //    if (index2 != -1)
                //    {
                //        info_device.Items.RemoveAt(index2);
                //    }
                //    int index_ok = info_device.Items.IndexOf(item);
                //    int count_listbox_ok = 0;
                //    if (index1 != -1 && index2 != -1)
                //    {
                //        for (int i = 0; i < info_device.Items.Count - 1; i++)
                //        {
                //            if (info_device.Items[index_ok].ToString() == info_device.Items[i].ToString())
                //            {
                //                count_listbox_ok++;
                //                if (count_listbox_ok > 0)
                //                {
                //                    info_device.Items.RemoveAt(i);
                //                }
                //            }
                //        }
                //    }
                //}
                info_device.Items.Add(item);
            }));
        }
        public void Connect_Robot()
        {
            try
            {
                // lấy địa chỉ ip
                string path = AppDomain.CurrentDomain.BaseDirectory;
                string content;
                content = File.ReadAllText(path + Path.DirectorySeparatorChar + "TCPIP.txt");
                string[] ipp = content.Split(';');
                IP_Robot = ipp[0];
                //
                callback = new SDKHrobot.HRobot.CallBackFun(EventFun);
                IPAddress ip_rb;
                if (IPAddress.TryParse(IP_Robot, out ip_rb))
                {
                    handle = SDKHrobot.HRobot.open_connection(ip_rb.ToString(), 1, callback);
                    if (handle < 0)
                    {
                        callback = new SDKHrobot.HRobot.CallBackFun(EventFun);
                        handle = SDKHrobot.HRobot.open_connection(ip_rb.ToString(), 1, callback);
                        if (handle < 0)
                        {
                            MessageBox.Show("Cannot connect to Robot, Please use Caterpillar to try open Robot!", "Connect Robot Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            display_box("Connect Robot Fail");
                            TaskbarManager.Instance.SetProgressValue(100, 100);
                            TaskbarManager.Instance.SetProgressState(TaskbarProgressBarState.Error);
                            return;
                        }
                    }
                    if (handle > -1)
                    {
                        StringBuilder HRSSverStr = new StringBuilder();
                        SDKHrobot.HRobot.get_hrss_version(handle, HRSSverStr);

                        display_box("ID:" + HRSSverStr.ToString());

                        StringBuilder RobotTypeStr = new StringBuilder();
                        SDKHrobot.HRobot.get_robot_type(handle, RobotTypeStr);

                        display_box("RBType:" + RobotTypeStr.ToString());
                        StringBuilder RobotID = new StringBuilder();
                        SDKHrobot.HRobot.get_robot_id(handle, RobotID);
                        display_box("Controller:" + RobotID.ToString());
                        TaskbarManager.Instance.SetProgressValue(100, 100);
                        TaskbarManager.Instance.SetProgressState(TaskbarProgressBarState.Normal);
                        display_box("Connect Robot Succesful");
                    }

                    UInt64[] alarm_code = new UInt64[100];
                    int count = 0;
                    SDKHrobot.HRobot.get_alarm_code(handle, ref count, alarm_code);

                    if (count > 0)
                    {
                        SDKHrobot.HRobot.clear_alarm(handle);
                    }
                    else
                    {
                        // lbAlram.BackColor = Color.DarkGray;
                    }
                    String tmp = "";
                    for (int a = 0; a < count; a++)
                    {
                        tmp = tmp + String.Format("{0:x16}\n", alarm_code[a]);
                    }
                    if (tmp == "")
                    {
                        //btn_main_signal_Robot.Enabled = true;
                        // btn_main_signal_Robot.BackColor = Color.GreenYellow;
                    }
                    else
                    {
                        //ListAlarmAddT(tmp);
                        //btn_main_signal_Robot.BackColor = Color.Red;
                    }
                }
                else
                {
                    //  btn_main_signal_Robot.BackColor = Color.Red;
                    MessageBox.Show("The IP you entered is: '" + IP_Robot + "'. Please enter the correct format of IP to connect Robot!", "Connect Robot Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (DllNotFoundException e)
            {
                //btn_main_signal_Robot.BackColor = Color.Red;
                MessageBox.Show(e.Message, "Connect Robot Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }
        #endregion
        #region Clent TCP/IP---------------------------------------------------------------
        private void Connect_sever_Cam1()
        {
            string ip; int port;
            // lấy địa chỉ ip
            try
            {
                string path = AppDomain.CurrentDomain.BaseDirectory;
                string content;
                content = File.ReadAllText(path + Path.DirectorySeparatorChar + "TCPIP.txt");
                string[] ipp = content.Split(';');
                ip = ipp[4];
                port = Convert.ToInt32(ipp[5]);
                client_cam1?.Disconnect();
                client_cam1.Connect(ip, port);
                TaskbarManager.Instance.SetProgressValue(100, 100);
                TaskbarManager.Instance.SetProgressState(TaskbarProgressBarState.Indeterminate);
                display_box("Connect Vision Cam1 Succesful");
            }
            catch
            {
                TaskbarManager.Instance.SetProgressValue(100, 100);
                TaskbarManager.Instance.SetProgressState(TaskbarProgressBarState.Error);
                display_box("Connect Vision Cam1 error");
                // MessageBox.Show("Connect Vision Cam1 error");

            }
        }
        private void Send_data_Cam1(string data)
        {
            FuncRobot.Flag_Cam1 = 0;
            try
            {
                if (client_cam1.TcpClient.Connected == true)
                {
                    client_cam1.Write(data);
                }
                else
                {
                    stop_change = 1;
                    stopOn();
                    //MessageBox.Show("Connect Vision Cam1 error");
                }
            }
            catch
            {
                stop_change = 1;
                stopOn();
                MessageBox.Show("Connect Vision Cam1 error");
            }
        }
        private void Client_Datareceived_Cam1(object sender, SimpleTCP.Message e)
        {
            //FuncRobot.Flag_Cam1 = Convert.ToInt16(e.MessageString);

            txt_DataReceived1.Invoke((MethodInvoker)delegate ()
            {
                txt_DataReceived1.Text = e.MessageString;
                if (txt_DataReceived1 != null)
                {
                    FuncRobot.Flag_Cam1 = Convert.ToInt16(txt_DataReceived1.Text);
                }

                txt_DataReceived1.Text = string.Empty;
            });

        }
        void Connect_sever_Cam2()
        {
            string ip; int port;
            // lấy địa chỉ ip
            try
            {
                string path = AppDomain.CurrentDomain.BaseDirectory;
                string content;
                content = File.ReadAllText(path + Path.DirectorySeparatorChar + "TCPIP.txt");
                string[] ipp = content.Split(';');
                ip = ipp[6];
                port = Convert.ToInt32(ipp[7]);
                client_cam2?.Disconnect();
                client_cam2.Connect(ip, port);
                TaskbarManager.Instance.SetProgressValue(100, 100);
                TaskbarManager.Instance.SetProgressState(TaskbarProgressBarState.Indeterminate);
                display_box("Connect Vision Cam2 Succesful");
            }
            catch
            {
                TaskbarManager.Instance.SetProgressValue(100, 100);
                TaskbarManager.Instance.SetProgressState(TaskbarProgressBarState.Error);
                display_box("Connect Vision Cam2 error");
                //MessageBox.Show("Connect Vision Cam2 error");
            }
        }
        void Send_data_Cam2(string data)
        {
            FuncRobot.Flag_Cam2 = 0;
            try
            {
                if (client_cam2.TcpClient.Connected == true)
                {
                    client_cam2.Write(data);
                }
                else
                {
                    stop_change = 1;
                    stopOn();
                    //MessageBox.Show("Connect Vision Cam2 error");
                }

            }
            catch
            {
                stop_change = 1;
                stopOn();
                MessageBox.Show("Connect Vision Cam2 error");
            }

        }
        private void Client_Datareceived_Cam2(object sender, SimpleTCP.Message e)
        {
            //FuncRobot.Flag_Cam2 = Convert.ToInt16(e.MessageString);
            txt_DataReceived2.Invoke((MethodInvoker)delegate ()
            {
                txt_DataReceived2.Text = e.MessageString;
                if (txt_DataReceived2 != null)
                {
                    FuncRobot.Flag_Cam2 = Convert.ToInt16(txt_DataReceived2.Text);
                    //txt_data_read2.Text = "0";
                }
                txt_DataReceived2.Text = string.Empty;
            });
        }
        #endregion
        #region Function Machine-----------------------------------------------------------
        private async Task Alarm()
        {
            await Task.Delay(30);
            if (InvokeRequired)
            {
                //listView1.Invoke(new MethodInvoker(() =>
                // {
                if (class_Alarm.Cancel_Alarm == true)
                {
                    if (PLC1.Read_Data_Bit_("M" + (8199 + Memory_PLC.K200).ToString()) == true)
                    {
                        int[] check_alarm = PLC1.ReadRandomBit("M" + (8000 + Memory_PLC.K200).ToString(), 60);
                        int inc = 1;
                        for (int i = 0; i < 58; i++)
                        {
                            listView_Alarm.Invoke(new MethodInvoker(() =>
                            {
                                if (check_alarm[i] == 1)
                                // if(res==true)
                                {
                                    class_Alarm.cmd_NewAlarm(inc);

                                    string alarm_new = class_Alarm.Alarm_ID[inc];
                                    //listView1.Invoke(new MethodInvoker(() =>
                                    // {

                                    ListViewItem item = listView_Alarm.FindItemWithText(alarm_new);
                                    ListViewItem itemdata = new ListViewItem();
                                    if (item == null)
                                    {
                                        //data_error(alarm_new, 1);
                                        class_Alarm.ngay_thang_nam = DateTime.Now;
                                        class_Alarm.gio_phut = DateTime.Now;
                                        string data_ngay_thang_nam = class_Alarm.ngay_thang_nam.ToString("dd/MM/yyyy");
                                        string data_gio = class_Alarm.gio_phut.ToString("HH:mm:ss");
                                        ListViewItem item1 = new ListViewItem();
                                        string CodeNumber = ErrorCodeNumber(inc);
                                        item1.Text = alarm_new + CodeNumber;
                                        item1.ForeColor = Color.Red;
                                        item1.SubItems.Add(new ListViewItem.ListViewSubItem() { Text = data_gio });
                                        item1.SubItems.Add(new ListViewItem.ListViewSubItem() { Text = data_ngay_thang_nam });
                                        listView_Alarm.Invoke(new MethodInvoker(() =>
                                        {
                                            listView_Alarm.Items.Add(item1);
                                        }));
                                        string savehistory = item1.Text;
                                        string[] History_ = { savehistory, data_ngay_thang_nam, data_gio };
                                        write_alarm_database(History_);
                                        if (class_Alarm.Cancel_Alarm == false)
                                        {
                                            i = 100;
                                        }
                                    }
                                }

                                else
                                {
                                    try
                                    {
                                        class_Alarm.cmd_NewAlarm(inc);
                                        string alarm_new = class_Alarm.Alarm_ID[inc];
                                        ListViewItem item = listView_Alarm.FindItemWithText(alarm_new);

                                        if (item != null)
                                        {
                                            int a = item.Index;
                                            listView_Alarm.Items[a].Remove();
                                        }
                                    }
                                    catch { }

                                }
                                inc++;
                            }));
                        }

                    }
                    else
                    {
                        listView_Alarm.Invoke(new MethodInvoker(() =>
                        {
                            listView_Alarm.Items.Clear();
                        }));
                    }

                }
                //}));

            }
        }
        private async Task Alarm1()
        {
            await Task.Delay(30);
            if (InvokeRequired)
            {
                //listView1.Invoke(new MethodInvoker(() =>
                // {
                if (class_Alarm.Cancel_Alarm == true)
                {
                    if (PLC1.Read_Data_Bit_("M" + (8199 + Memory_PLC.K200).ToString()) == true)
                    {
                        int[] check_alarm = PLC1.ReadRandomBit("M" + (8000 + Memory_PLC.K200).ToString(), 60);
                        int inc = 1;
                        for (int i = 0; i < 58; i++)
                        {
                            listView_Alarm.Invoke(new MethodInvoker(() =>
                            {
                                if (check_alarm[i] == 1)
                                // if(res==true)
                                {
                                    class_Alarm.cmd_NewAlarm(inc);

                                    string alarm_new = class_Alarm.Alarm_ID[inc];
                                    if (!class_Alarm.alarmStartTime.ContainsKey(alarm_new))
                                    {
                                        class_Alarm.alarmStartTime[alarm_new] = DateTime.Now; // thời điểm bắt đầu lỗi
                                    }
                                    //listView1.Invoke(new MethodInvoker(() =>
                                    // {

                                    ListViewItem item = listView_Alarm.FindItemWithText(alarm_new);
                                    ListViewItem itemdata = new ListViewItem();
                                    if (item == null)
                                    {
                                        //data_error(alarm_new, 1);
                                        class_Alarm.ngay_thang_nam = DateTime.Now;
                                        class_Alarm.gio_phut = DateTime.Now;
                                        string data_ngay_thang_nam = class_Alarm.ngay_thang_nam.ToString("dd/MM/yyyy");
                                        string data_gio = class_Alarm.gio_phut.ToString("HH:mm:ss");
                                        ListViewItem item1 = new ListViewItem();
                                        string CodeNumber = ErrorCodeNumber(inc);
                                        item1.Text = alarm_new + CodeNumber;
                                        item1.ForeColor = Color.Red;
                                        item1.SubItems.Add(new ListViewItem.ListViewSubItem() { Text = data_gio });
                                        item1.SubItems.Add(new ListViewItem.ListViewSubItem() { Text = data_ngay_thang_nam });
                                        listView_Alarm.Invoke(new MethodInvoker(() =>
                                        {
                                            listView_Alarm.Items.Add(item1);
                                        }));
                                        string savehistory = item1.Text;
                                        string[] History_ = { savehistory, data_ngay_thang_nam, data_gio };
                                        write_alarm_database(History_);
                                        if (class_Alarm.Cancel_Alarm == false)
                                        {
                                            i = 100;
                                        }
                                    }
                                }

                                else
                                {
                                    try
                                    {
                                        class_Alarm.cmd_NewAlarm(inc);
                                        string alarm_new = class_Alarm.Alarm_ID[inc];
                                        ListViewItem item = listView_Alarm.FindItemWithText(alarm_new);

                                        if (item != null)
                                        {
                                            int a = item.Index;
                                            listView_Alarm.Items[a].Remove();
                                        }
                                    }
                                    catch { }

                                }
                                inc++;
                            }));
                        }

                    }
                    else
                    {
                        listView_Alarm.Invoke(new MethodInvoker(() =>
                        {
                            listView_Alarm.Items.Clear();
                        }));
                    }

                }
                //}));

            }
        }
        // Biến global     
        Dictionary<string, AlarmInfo> alarmDict1 = new Dictionary<string, AlarmInfo>();
        public event Action<AlarmInfo, bool, DateTime> OnAlarmChanged1;
        private async Task Alarm2()
        {
            while (Global.IsConnectPLC == true)
            {
                await Task.Delay(300);

                if (Global.cancel_alarm == true)
                {
                    bool masterAlarm = PLC1.Read_Data_Bit_("M" + (8199 + Memory_PLC.K200));
                    //Nếu không còn alarm tổng → clear hết
                    if (!masterAlarm)
                    {
                        listView_Alarm.Invoke(new MethodInvoker(() =>
                        {
                            listView_Alarm.Items.Clear();
                        }));

                        alarmDict1.Clear();
                        return;
                    }
                    try
                    {
                        int[] check_alarm = PLC1.ReadRandomBit("M" + (8000 + Memory_PLC.K200), 60);

                        List<Action> uiActions = new List<Action>();

                        int inc = 1;

                        for (int i = 0; i < 58; i++)
                        {
                            class_Alarm.cmd_NewAlarm(inc);
                            string alarm_new = class_Alarm.Alarm_ID[inc];

                            // =========================
                            // ALARM ON
                            // =========================
                            if (check_alarm[i] == 1)
                            {
                                if (!alarmDict1.ContainsKey(alarm_new))
                                {
                                    DateTime start = DateTime.Now;

                                    AlarmInfo info = new AlarmInfo();
                                    info.StartTime = start;
                                    string startStr = start.ToString("HH:mm:ss");
                                    DateTime dt = DateTime.Now;
                                    string date = dt.ToString("dd/MM/yyyy");
                                    string[] History_ = { alarm_new, date, startStr };
                                    write_alarm_database(History_);
                                    uiActions.Add(() =>
                                    {
                                        ListViewItem item = new ListViewItem(alarm_new);
                                        item.ForeColor = Color.Red;

                                        item.SubItems.Add(startStr); // Start
                                        item.SubItems.Add("");       // End
                                        item.SubItems.Add("");       // Lost
                                        item.SubItems.Add(date);       // date
                                        listView_Alarm.Items.Insert(0, item);

                                        info.Item = item;
                                    });

                                    alarmDict1[alarm_new] = info;
                                }
                            }

                            // =========================
                            // ALARM OFF
                            // =========================
                            else
                            {
                                if (alarmDict1.ContainsKey(alarm_new))
                                {
                                    var info = alarmDict1[alarm_new];

                                    DateTime end = DateTime.Now;
                                    TimeSpan lost = end - info.StartTime;

                                    string endStr = end.ToString("HH:mm:ss");
                                    string lostStr = lost.ToString(@"hh\:mm\:ss");
                                    string date = end.ToString("dd/MM/yyyy");
                                    // GHI DB 1 DÒNG DUY NHẤT
                                    //save_alarm_full(alarm_new, info.StartTime, end, lost);

                                    uiActions.Add(() =>
                                    {
                                        if (info.Item != null)
                                        {
                                            info.Item.SubItems[2].Text = endStr;   // End
                                            info.Item.SubItems[3].Text = lostStr;  // Lost
                                                                                   //info.Item.SubItems[4].Text = date;
                                            info.Item.ForeColor = Color.Black;     // optional: đổi màu
                                        }
                                    });
                                    alarmDict1.Remove(alarm_new);

                                }
                            }

                            inc++;
                        }
                        // Update UI 1 lần duy nhất
                        if (uiActions.Count > 0)
                        {
                            listView_Alarm.Invoke(new MethodInvoker(() =>
                            {
                                foreach (var action in uiActions)
                                    action();
                            }));
                        }
                    }
                    catch { }
                }
            }
        }
        private async Task Alarm3()
        {
            //while (Global.IsConnectPLC)
            //{
            await Task.Delay(50); // nhanh hơn 300ms cho đỡ miss

            //if (Global.dataMonitor[0] == 0) continue;

            try
            {
                //bool masterAlarm = PLC1.Read_Data_Bit_("M" + (6199 + Memory_PLC.K200));

                // clear all khi hết alarm tổng
                if (Global.dataMonitor[0] == 0)
                {
                    foreach (var kv in alarmDict1.ToList())
                    {
                        OnAlarmChanged1?.Invoke(kv.Value, false, DateTime.Now);
                    }

                    alarmDict1.Clear();
                    //continue;
                    return;
                }
                int[] bits = PLC1.ReadRandomBit("M" + (8000 + Memory_PLC.K200), 60);
                int inc = 1;

                for (int i = 0; i < 58; i++)
                {
                    class_Alarm.cmd_NewAlarm(inc);
                    string alarmId = class_Alarm.Alarm_ID[inc];

                    bool isOn = bits[i] == 1;

                    // ================= ON =================
                    if (isOn)
                    {
                        if (!alarmDict1.ContainsKey(alarmId))
                        {
                            string CodeAlarm = ErrorCodeNumber(inc);
                            var info = new AlarmInfo
                            {
                                AlarmId = alarmId + ": " + CodeAlarm,
                                StartTime = DateTime.Now
                            };

                            alarmDict1[alarmId] = info;
                            OnAlarmChanged1?.Invoke(info, true, info.StartTime);
                        }
                    }
                    // ================= OFF =================
                    else
                    {
                        if (alarmDict1.ContainsKey(alarmId))
                        {
                            var info = alarmDict1[alarmId];

                            OnAlarmChanged1?.Invoke(info, false, DateTime.Now);

                            alarmDict1.Remove(alarmId);
                        }
                    }

                    inc++;
                }
            }
            catch
            {
                // nên log ra nếu cần
            }
            //}
        }
        private void InitAlarmEvent()
        {
            OnAlarmChanged += (info, isOn, time) =>
            {
                listView_Alarm.Invoke(new MethodInvoker(() =>
                {
                    if (isOn)
                    {
                        string start = time.ToString("HH:mm:ss");
                        string date = time.ToString("dd/MM/yyyy");

                        ListViewItem item = new ListViewItem(info.AlarmId);
                        item.ForeColor = Color.Red;

                        item.SubItems.Add(start);
                        item.SubItems.Add("");
                        item.SubItems.Add("");
                        item.SubItems.Add(date);

                        listView_Alarm.Items.Insert(0, item);

                        info.Item = item;

                        // Mes
                        string[] History_ = { info.AlarmId, date, start };
                        write_alarm_database(History_);
                    }
                    else
                    {
                        DateTime end = time;
                        TimeSpan lost = end - info.StartTime;

                        if (info.Item != null)
                        {
                            info.Item.SubItems[2].Text = end.ToString("HH:mm:ss");
                            info.Item.SubItems[3].Text = lost.ToString(@"hh\:mm\:ss");
                            info.Item.ForeColor = Color.Black;
                        }

                        // nếu cần lưu full
                        // save_alarm_full(info.AlarmId, info.StartTime, end, lost);
                    }
                }));
            };
        }
        private void InitAlarmEvent1()
        {
            OnAlarmChanged1 += (info, isOn, time) =>
             {
                 listView_Alarm.Invoke(new MethodInvoker(() =>
                 {
                     if (isOn)
                     {
                         string start = time.ToString("HH:mm:ss");
                         string date = time.ToString("dd/MM/yyyy");
                         ListViewItem item = new ListViewItem(info.AlarmId);
                         item.ForeColor = Color.Red;

                         item.SubItems.Add(start);
                         item.SubItems.Add("");
                         item.SubItems.Add("");
                         item.SubItems.Add(date);

                         listView_Alarm.Items.Insert(0, item);

                         info.Item = item;

                         //  DB
                         string[] History_ = { info.AlarmId, date, start };
                         write_alarm_database(History_);
                     }
                     else
                     {
                         DateTime end = time;
                         TimeSpan lost = end - info.StartTime;

                         if (info.Item != null)
                         {
                             info.Item.SubItems[2].Text = end.ToString("HH:mm:ss");
                             info.Item.SubItems[3].Text = lost.ToString(@"hh\:mm\:ss");
                             info.Item.ForeColor = Color.Black;
                         }

                         // nếu cần lưu full
                         // save_alarm_full(info.AlarmId, info.StartTime, end, lost);
                     }
                 }));
             };
        }
        private async Task SendAlarmToServerAsync(string alarmId, DateTime start_time, DateTime end_time)
        {
            if (end_time == null)
            {
                end_time = DateTime.Now;
            }
            try
            {
                List<E_ALARM_HISTORY> list = new List<E_ALARM_HISTORY>()
                {
                    new E_ALARM_HISTORY
                    {
                        EQUIPMENT_ID=Global.MACHINE_NAME,
                        TYPE = "Error",
                        MSG=alarmId,
                        START_TIME =start_time,
                        END_TIME=end_time,
                        INSERT_DATE =start_time,
                        INSERT_USER="Ai-Vision",
                        DEL_FLAG="N",
                    },
                };
                await IT_SQL_Server_Helper.Instance.InsertListIgnoreDuplicate("E_ALARM_HISTORY", list);
            }
            catch
            {

            }
        }
        Dictionary<string, bool> _alarmState = new Dictionary<string, bool>();
        private string ErrorCodeNumber(int Code)
        {
            string codeNumber = "---Code: ";
            switch (Code)
            {
                case 12:
                    codeNumber = codeNumber + PLC1.Read_Data_Word_("D" + (2020 + Memory_PLC.K500).ToString()).ToString();
                    break;
                case 13:
                    codeNumber = codeNumber + PLC1.Read_Data_Word_("D" + (2021 + Memory_PLC.K500).ToString()).ToString();
                    break;
                case 14:
                    codeNumber = codeNumber + PLC1.Read_Data_Word_("D" + (2040 + Memory_PLC.K500).ToString()).ToString();
                    break;
                case 15:
                    codeNumber = codeNumber + PLC1.Read_Data_Word_("D" + (2041 + Memory_PLC.K500).ToString()).ToString();
                    break;
                case 16:
                    codeNumber = codeNumber + PLC1.Read_Data_Word_("D" + (2042 + Memory_PLC.K500).ToString()).ToString();
                    break;
                case 17:
                    codeNumber = codeNumber + PLC1.Read_Data_Word_("D" + (2043 + Memory_PLC.K500).ToString()).ToString();
                    break;
                case 18:
                    codeNumber = codeNumber + PLC1.Read_Data_Word_("D" + (2000 + Memory_PLC.K500).ToString()).ToString();
                    break;
                case 19:
                    codeNumber = codeNumber + PLC1.Read_Data_Word_("D" + (2001 + Memory_PLC.K500).ToString()).ToString();
                    break;
                case 20:
                    codeNumber = codeNumber + PLC1.Read_Data_Word_("D" + (2002 + Memory_PLC.K500).ToString()).ToString();
                    break;
                default:
                    codeNumber = " ";
                    break;
            }
            return codeNumber;
        }
        private async void Warring()
        {
            await Task.Delay(10);
            if (InvokeRequired)
            {
                try
                {
                    for (int i = 0; i < 17; i++)
                    {
                        Warring_List.Invoke(new MethodInvoker(() =>
                        {
                            if (Global.Warring_ListView[i] == 1)
                            {
                                class_Alarm.cmd_NewWarring(i);
                                string warring_new = class_Alarm.Warring_ID[i];
                                ListViewItem item = Warring_List.FindItemWithText(warring_new);
                                if (item == null)
                                {
                                    // class_Alarm.ngay_thang_nam = DateTime.Now;
                                    // class_Alarm.gio_phut = DateTime.Now;
                                    // string data_ngay_thang_nam = class_Alarm.ngay_thang_nam.ToString("dd/MM/yyyy");
                                    // string data_gio = class_Alarm.gio_phut.ToString("HH:mm:ss");
                                    ListViewItem item1 = new ListViewItem();
                                    item1.Text = warring_new;
                                    item1.ForeColor = Color.Red;
                                    // item1.SubItems.Add(new ListViewItem.ListViewSubItem() { Text = data_ngay_thang_nam });
                                    // item1.SubItems.Add(new ListViewItem.ListViewSubItem() { Text = data_gio });
                                    Warring_List.Invoke(new MethodInvoker(() =>
                                    {
                                        Warring_List.Items.Add(item1);
                                    }));
                                }
                            }
                            else
                            {
                                try
                                {
                                    class_Alarm.cmd_NewWarring(i);
                                    string warring_new1 = class_Alarm.Warring_ID[i];
                                    ListViewItem item2 = Warring_List.FindItemWithText(warring_new1);
                                    if (item2 != null)
                                    {
                                        int a = item2.Index;
                                        Warring_List.Items[a].Remove();
                                    }
                                }
                                catch { }

                            }
                        }));
                    }
                }
                catch (Exception ex) { Message_Box_Error(ex.ToString(), "Warring ListView"); }
            }
        }
        private void write_alarm_database(string[] data)
        {
            try
            {
                using (var conn_ = GetConnectionSQLite())
                {
                    string SQL_data = string.Format("INSERT INTO History_Alarm (Mess,Time,Date)" + "VALUES (@Mess,@Time,@Date)");
                    using (SQLiteCommand cmd = new SQLiteCommand(SQL_data, conn_))
                    {
                        cmd.Parameters.AddWithValue("@Mess", data[0]);
                        cmd.Parameters.AddWithValue("@Time", data[2]);
                        cmd.Parameters.AddWithValue("@Date", data[1]);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch { }
        }
        private void write_Production_database(string data, string OK, string NG)
        {
            DateTime date;
            date = DateTime.Now;
            string data1 = date.ToString("dd/MM/yyyy");
            string data2 = date.ToString("HH: mm:ss");
            data = (data == "") ? "0" : data;
            using (var con = GetConnectionSQLite())
            {
                con.Open();
                string SQL_data = string.Format("INSERT INTO Production (Qty,OK,NG,Time,Date)" + "VALUES (@Qty,@OK,@NG,@Time,@Date)");
                using (SQLiteCommand cmd = new SQLiteCommand(SQL_data, con))
                {
                    cmd.Parameters.AddWithValue("@Qty", data);
                    cmd.Parameters.AddWithValue("@OK", Convert.ToInt32(OK));
                    cmd.Parameters.AddWithValue("@NG", Convert.ToInt32(NG));
                    cmd.Parameters.AddWithValue("@Time", data2);
                    cmd.Parameters.AddWithValue("@Date", data1);
                    cmd.ExecuteNonQuery();
                }
            }
        }
        #endregion
        #region tabcontrol-----------------------------------------------------------------
        int tab_select_all, tabControl_PLC_, tabremov_PLC, tabtray_PLC, tabvision_PLC, tabIO_input_PLC, tabIO_output_PLC, tabControl_Robot_, tabControl_Jog_RB_, tabControl_InfoMachine;
        #endregion
        #region Tabcontrol select----------------------------------------------------------
        private void tabControl_Jog_RB_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (InvokeRequired)
            {
                Invoke(new MethodInvoker(() => tabControl_Jog_RB_SelectedIndexChanged(sender, e)));
                return;
            }
            tabControl_Jog_RB_ = tabControl_Jog_RB.SelectedIndex;

            if (tabControl_Jog_RB_ == 0)
            {
                CurSelJogType = (int)JogType.Cart;
            }
            else
            {
                CurSelJogType = (int)JogType.Joint;
            }
        }
        private void tabControl_All_SelectedIndexChanged(object sender, EventArgs e)
        {
            //if (InvokeRequired)
            //{
            //    Invoke(new MethodInvoker(() => tabControl_All_SelectedIndexChanged(sender, e)));
            //    return;
            //}
            //tab_select_all = TabControl_All.SelectedIndex;
            //if (tab_select_all == 5)
            //{
            //    int[] data1 = PLC1.Read_Word_Arr("D" + (10901 + Memory_PLC.K2000), 9);
            //    int[] data2 = PLC1.Read_Word_Arr("D" + (10921 + Memory_PLC.K2000), 9);
            //    double[] ValueTimeRobot = new double[10];
            //    ConnectSQLite();
            //    for (int i = 1; i < 10; i++)
            //    {
            //        string query = string.Format("SELECT* from TactTime where STT='{0}' ", i);
            //        SQLiteCommand cmd = new SQLiteCommand(query, Conn);
            //        var reader = cmd.ExecuteReader();
            //        while (reader.Read())
            //        {
            //            var value = Math.Round(Convert.ToDouble(reader["VALUE"]), 3);
            //            ValueTimeRobot[i - 1] = value;
            //        }
            //    }
            //    DisConSQLite();
            //    for (int i = 1; i < 9; i++)
            //    {
            //        UILabel lab0 = this.Controls.Find("Tact" + i, true).FirstOrDefault() as UILabel;
            //        UILabel lab1 = this.Controls.Find("Tact1" + i, true).FirstOrDefault() as UILabel;
            //        UILabel lab2 = this.Controls.Find("Tact2" + i, true).FirstOrDefault() as UILabel;
            //        UILabel lab3 = this.Controls.Find("Tact3" + i, true).FirstOrDefault() as UILabel;
            //        if (lab0 != null) { StatusDisplay.Instance.Update_Label(lab0, ValueTimeRobot[i - 1]); }
            //        if (lab1 != null) { StatusDisplay.Instance.Update_Label(lab1, Convert.ToDouble(data1[i - 1]) / 100); }
            //        if (lab2 != null) { StatusDisplay.Instance.Update_Label(lab2, Convert.ToDouble(data2[i - 1]) / 100); }
            //        if (lab0 != null && lab1 != null && lab2 != null && lab3 != null)
            //        {
            //            lab3.Text = (Convert.ToDouble(lab0.Text) + Convert.ToDouble(lab1.Text) + Convert.ToDouble(lab2.Text)).ToString();
            //        }
            //    }
            //    Config_Chart();
            //}
        }

        private void tabControl_Robot_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (InvokeRequired)
            {
                Invoke(new MethodInvoker(() => tabControl_Robot_SelectedIndexChanged(sender, e)));
                return;
            }
            //tabControl_Robot_ = tabControl_Robot.SelectedIndex;
        }

        private void tabControl_PLC_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (InvokeRequired)
            {
                Invoke(new MethodInvoker(() => tabControl_PLC_SelectedIndexChanged(sender, e)));
                return;
            }
            //tabControl_PLC_ = tabControl_PLC.SelectedIndex;
        }

        private void tabControl_Group_1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (InvokeRequired)
            {
                Invoke(new MethodInvoker(() => tabControl_Group_1_SelectedIndexChanged(sender, e)));
                return;
            }
            //tabremov_PLC = tabControl_Group_1.SelectedIndex;
        }

        private void tabControl_Group_2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (InvokeRequired)
            {
                Invoke(new MethodInvoker(() => tabControl_Group_2_SelectedIndexChanged(sender, e)));
                return;
            }
            //tabtray_PLC = tabControl_Group_2.SelectedIndex;
        }

        private void tabControl_Group_3_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (InvokeRequired)
            {
                Invoke(new MethodInvoker(() => tabControl_Group_3_SelectedIndexChanged(sender, e)));
                return;
            }
            //tabvision_PLC = tabControl_Group_3.SelectedIndex;
        }

        private void tabControl_input12_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (InvokeRequired)
            {
                Invoke(new MethodInvoker(() => tabControl_input12_SelectedIndexChanged(sender, e)));
                return;
            }
            //tabIO_input_PLC = tabControl_input12.SelectedIndex;
        }

        private void tabControl_Output123_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (InvokeRequired)
            {
                Invoke(new MethodInvoker(() => tabControl_Output123_SelectedIndexChanged(sender, e)));
                return;
            }
            //tabIO_output_PLC = tabControl_Output123.SelectedIndex;
        }
        //
        private void LightRefresh(TabPage page)
        {

        }
        private void HeavyInit(TabPage page)
        {


        }
        private void tabcontrol_All_Selecting(object sender, TabControlCancelEventArgs e)
        {
            //_tabRedrawGuard = UIHelper.PauseRedraw(TabControl_All);
        }
        private void TabControl_All_Selected(object sender, TabControlEventArgs e)
        {
            if (InvokeRequired)
            {
                Invoke(new MethodInvoker(() => tabControl_All_SelectedIndexChanged(sender, e)));
                return;
            }
            tab_select_all = TabControl_All.SelectedIndex;
            uiCalendar1.Date = DateTime.Today;
            if (tab_select_all == 5)
            {
                int[] data1 = PLC1.Read_Word_Arr("D" + (10901 + Memory_PLC.K2000), 9);
                int[] data2 = PLC1.Read_Word_Arr("D" + (10921 + Memory_PLC.K2000), 9);
                double[] ValueTimeRobot = new double[10];

                using (var con = GetConnectionSQLite())
                {
                    con.Open();
                    for (int i = 1; i < 10; i++)
                    {
                        string query = string.Format("SELECT* from TactTime where STT='{0}' ", i);
                        SQLiteCommand cmd = new SQLiteCommand(query, con);
                        var reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            var value = Math.Round(Convert.ToDouble(reader["VALUE"]), 3);
                            ValueTimeRobot[i - 1] = value;
                        }
                    }

                }
                for (int i = 1; i < 9; i++)
                {
                    UILabel lab0 = this.Controls.Find("Tact" + i, true).FirstOrDefault() as UILabel;
                    UILabel lab1 = this.Controls.Find("Tact1" + i, true).FirstOrDefault() as UILabel;
                    UILabel lab2 = this.Controls.Find("Tact2" + i, true).FirstOrDefault() as UILabel;
                    UILabel lab3 = this.Controls.Find("Tact3" + i, true).FirstOrDefault() as UILabel;
                    if (lab0 != null) { StatusDisplay.Instance.Update_Label(lab0, ValueTimeRobot[i - 1]); }
                    if (lab1 != null) { StatusDisplay.Instance.Update_Label(lab1, Convert.ToDouble(data1[i - 1]) / 100); }
                    if (lab2 != null) { StatusDisplay.Instance.Update_Label(lab2, Convert.ToDouble(data2[i - 1]) / 100); }
                    if (lab0 != null && lab1 != null && lab2 != null && lab3 != null)
                    {
                        lab3.Text = (Convert.ToDouble(lab0.Text) + Convert.ToDouble(lab1.Text) + Convert.ToDouble(lab2.Text)).ToString();
                    }
                }
                Config_Chart();
            }

        }
        //
        private void tabControl_Robot_Selecting(object sender, TabControlCancelEventArgs e)
        {
            // _tabRedrawGuard = UIHelper.PauseRedraw(tabControl_Robot);
        }
        private void tabControl_PLC_Selecting(object sender, TabControlCancelEventArgs e)
        {
            // _tabRedrawGuard = UIHelper.PauseRedraw(tabControl_PLC);
        }

        private void tabControl_Robot_Selected(object sender, TabControlEventArgs e)
        {
            tabControl_Robot_ = tabControl_Robot.SelectedIndex;
        }

        private void tabControl_PLC_Selected(object sender, TabControlEventArgs e)
        {
            tabControl_PLC_ = tabControl_PLC.SelectedIndex;
        }
        private void tabControl_Group_1_Selecting(object sender, TabControlCancelEventArgs e)
        {
            //_tabRedrawGuard = UIHelper.PauseRedraw(tabControl_Group_1);
        }
        private void tabControl_Group_2_Selecting(object sender, TabControlCancelEventArgs e)
        {
            //_tabRedrawGuard = UIHelper.PauseRedraw(tabControl_Group_2);
        }
        private void tabControl_Group_3_Selecting(object sender, TabControlCancelEventArgs e)
        {
            // _tabRedrawGuard = UIHelper.PauseRedraw(tabControl_Group_3);
        }
        private void tabControl_input12_Selecting(object sender, TabControlCancelEventArgs e)
        {
            //_tabRedrawGuard = UIHelper.PauseRedraw(tabControl_input12);
        }
        private void tabControl_Output123_Selecting(object sender, TabControlCancelEventArgs e)
        {
            //_tabRedrawGuard = UIHelper.PauseRedraw(tabControl_Output123);
        }

        private void tabControl_Group_1_Selected(object sender, TabControlEventArgs e)
        {
            tabremov_PLC = tabControl_Group_1.SelectedIndex;
        }

        private void tabControl_Group_2_Selected(object sender, TabControlEventArgs e)
        {
            tabtray_PLC = tabControl_Group_2.SelectedIndex;
        }

        private void tabControl_Group_3_Selected(object sender, TabControlEventArgs e)
        {
            tabvision_PLC = tabControl_Group_3.SelectedIndex;
        }

        private void tabControl_input12_Selected(object sender, TabControlEventArgs e)
        {
            tabIO_input_PLC = tabControl_input12.SelectedIndex;
        }

        private void tabControl_Output123_Selected(object sender, TabControlEventArgs e)
        {
            tabIO_output_PLC = tabControl_Output123.SelectedIndex;
        }
        private void tabControl_InfoMachine_Selected(object sender, TabControlEventArgs e)
        {
            tabControl_InfoMachine = TabControl_InfoMachine.SelectedIndex;
            if (tabControl_InfoMachine == 2)
            {
                Load_data_machine_server();
            }
        }

        #endregion
        #region Button Robot---------------------------------------------------------------
        //Config datagridview pos robot
        public void Config_LoadDataRB_DataGridView()
        {
            string query = string.Format("SELECT * from Data_Pos_RB");
            SQLiteDataAdapter adapter = new SQLiteDataAdapter(query, Conn);
            DataSet dataSet = new DataSet();
            adapter.Fill(dataSet, "Data_Pos_RB");
            if (uiDataGridView1.InvokeRequired)
            {
                uiDataGridView1.Invoke(new MethodInvoker(() =>
                {
                    uiDataGridView1.DataSource = dataSet.Tables["Data_Pos_RB"];
                    uiDataGridView1.Columns["STT"].Width = 50;
                    uiDataGridView1.Columns["POSITION"].Width = 280;
                    uiDataGridView1.Columns["A4"].Width = 70;
                    uiDataGridView1.Columns["A5"].Width = 70;
                    uiDataGridView1.Columns["STT"].ReadOnly = true;
                    uiDataGridView1.Columns["POSITION"].ReadOnly = true;
                    uiDataGridView1.Columns["X"].ReadOnly = true;
                    uiDataGridView1.Columns["Y"].ReadOnly = true;
                    uiDataGridView1.Columns["Z"].ReadOnly = true;
                    uiDataGridView1.Columns["C"].ReadOnly = true;
                    uiDataGridView1.Columns["A4"].ReadOnly = true;
                    uiDataGridView1.Columns["A5"].ReadOnly = true;
                    uiDataGridView1.CellFormatting += (s, e) =>
                    {
                        if (e.ColumnIndex == uiDataGridView1.Columns["POSITION"].Index) // Xác định cột cần căn chỉnh
                        {
                            e.CellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
                        }
                    };
                    uiDataGridView1.CellFormatting += (s, e) =>
                    {
                        if (e.ColumnIndex == uiDataGridView1.Columns["X"].Index) // Xác định cột cần căn chỉnh
                        {
                            e.CellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                        }
                    };
                    uiDataGridView1.CellFormatting += (s, e) =>
                    {
                        if (e.ColumnIndex == uiDataGridView1.Columns["Y"].Index) // Xác định cột cần căn chỉnh
                        {
                            e.CellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                        }
                    };
                    uiDataGridView1.CellFormatting += (s, e) =>
                    {
                        if (e.ColumnIndex == uiDataGridView1.Columns["A4"].Index) // Xác định cột cần căn chỉnh
                        {
                            e.CellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                        }
                    };
                    uiDataGridView1.CellFormatting += (s, e) =>
                    {
                        if (e.ColumnIndex == uiDataGridView1.Columns["A5"].Index) // Xác định cột cần căn chỉnh
                        {
                            e.CellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                        }
                    };
                    uiDataGridView1.CellFormatting += (s, e) =>
                    {
                        if (e.ColumnIndex == uiDataGridView1.Columns["Z"].Index) // Xác định cột cần căn chỉnh
                        {
                            e.CellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                        }
                    };
                    uiDataGridView1.CellFormatting += (s, e) =>
                    {
                        if (e.ColumnIndex == uiDataGridView1.Columns["C"].Index) // Xác định cột cần căn chỉnh
                        {
                            e.CellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                        }
                    };
                }));
            }
        }

        public void Read_data_row_column_tray()
        {
            //StatusDisplay.Instance.Update_text(txt_number_tray_curr, PLC1.Read_Data_Word_("D" + (3000 + Memory_PLC.K1000).ToString()));
            //StatusDisplay.Instance.Update_text(txt_number_tray_output, PLC1.Read_Data_Word_("D" + (3001 + Memory_PLC.K1000).ToString()));
            //StatusDisplay.Instance.Update_text(txt_Row_tray, PLC1.Read_Data_Word_("D" + (3002 + Memory_PLC.K1000).ToString()));
            //StatusDisplay.Instance.Update_text(txt_Column_tray, PLC1.Read_Data_Word_("D" + (3003 + Memory_PLC.K1000).ToString()));


            //if (Convert.ToInt16(txt_Row_tray.Text) % 2 == 0)
            //{
            //    StatusDisplay.Instance.Update_text(txt_row_Cam, (Convert.ToInt16(txt_Row_tray.Text) / 2));
            //    StatusDisplay.Instance.Update_text(txt_Column_Cam, (Convert.ToInt16(txt_Column_tray.Text)));
            //    Select_row_tray_cam_ = Convert.ToInt16(txt_Row_tray.Text);
            //}
            //else
            //{
            //    StatusDisplay.Instance.Update_text(txt_row_Cam, (Convert.ToInt16(txt_Row_tray.Text) / 2 + 1));
            //    StatusDisplay.Instance.Update_text(txt_Column_Cam, (Convert.ToInt16(txt_Column_tray.Text)));
            //    Select_row_tray_cam_ = Convert.ToInt16(txt_Row_tray.Text);
            //}

        }
        private void savePos_RB()
        {
            string[] save = new string[8];
            if (uiDataGridView1.SelectedRows.Count > 0)
            {
                int read_curr = SDKHrobot.HRobot.get_current_position(handle, Global.Get_Curent_XYZ_RB);
                DataGridViewRow row = uiDataGridView1.SelectedRows[0];
                save[1] = row.Cells[1].Value.ToString();
                string message_ = "Bạn Có muốn lưu vào " + save[1].ToString() + " không?";
                const string caption_ = "Save Position Robot";
                var result = MessageBox.Show(message_, caption_, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    if (read_curr == 0)
                    {
                        if (row.Index > 3 && row.Index < 8)
                        {
                            if (Global.Get_Curent_XYZ_RB[2] > Global.Z_Place_Satefy && Global.Get_Curent_XYZ_RB[1] < Global.Y_Place_Satefy_U && Global.Get_Curent_XYZ_RB[1] > Global.Y_Place_Satefy_L
                                && Global.Get_Curent_XYZ_RB[0] < Global.X_Place_Satefy_U && Global.Get_Curent_XYZ_RB[0] > Global.X_Place_Satefy_L)
                            {
                                for (int i = 0; i < 1; i++)
                                {
                                    row.Cells[i + 2].Value = Global.Get_Curent_XYZ_RB[0].ToString();
                                    row.Cells[i + 3].Value = Global.Get_Curent_XYZ_RB[1].ToString();
                                    row.Cells[i + 4].Value = Global.Get_Curent_XYZ_RB[2].ToString();
                                    row.Cells[i + 7].Value = Global.Get_Curent_XYZ_RB[5].ToString();
                                }
                                for (int j = 0; j < row.Cells.Count; j++)
                                {
                                    save[j] = row.Cells[j].Value.ToString();
                                }
                                ConnectSQLite();
                                string saveposSQL = string.Format("INSERT OR REPLACE INTO Data_Pos_RB (STT, POSITION, X,Y,Z,A4,A5,C) VALUES (@STT, @POSITION, @X,@Y,@Z,@A4,@A5,@C)");
                                using (SQLiteCommand cmd = new SQLiteCommand(saveposSQL, Conn))
                                {
                                    cmd.Parameters.AddWithValue("@STT", save[0]);
                                    cmd.Parameters.AddWithValue("@POSITION", save[1]);
                                    cmd.Parameters.AddWithValue("@X", Convert.ToDouble(save[2]));
                                    cmd.Parameters.AddWithValue("@Y", Convert.ToDouble(save[3]));
                                    cmd.Parameters.AddWithValue("@Z", Convert.ToDouble(save[4]));
                                    cmd.Parameters.AddWithValue("@A4", 0.00);
                                    cmd.Parameters.AddWithValue("@A5", 0.00);
                                    cmd.Parameters.AddWithValue("@C", Convert.ToDouble(save[7]));
                                    cmd.ExecuteNonQuery();
                                    Message_Box_OK("Lưu thành công", "Save");
                                }
                                DisConSQLite();
                            }
                            else
                            {
                                Message_Box_Error("Chiều Cao Z or Y or X ngoài phạm vi cài đặt", "Alarm");
                            }
                        }
                        else if (row.Index == 2 || row.Index == 3)
                        {
                            if (Global.Get_Curent_XYZ_RB[2] > Global.Z_Camtop_Satefy && Global.Get_Curent_XYZ_RB[1] < Global.Y_Camtop_Satefy_U && Global.Get_Curent_XYZ_RB[1] > Global.Y_Camtop_Satefy_L
                                && Global.Get_Curent_XYZ_RB[0] < Global.X_Camtop_Satefy_U && Global.Get_Curent_XYZ_RB[0] > Global.X_Camtop_Satefy_L)
                            {
                                for (int i = 0; i < 1; i++)
                                {
                                    row.Cells[i + 2].Value = Global.Get_Curent_XYZ_RB[0].ToString();
                                    row.Cells[i + 3].Value = Global.Get_Curent_XYZ_RB[1].ToString();
                                    row.Cells[i + 4].Value = Global.Get_Curent_XYZ_RB[2].ToString();
                                    row.Cells[i + 7].Value = Global.Get_Curent_XYZ_RB[5].ToString();
                                }
                                for (int j = 0; j < row.Cells.Count; j++)
                                {
                                    save[j] = row.Cells[j].Value.ToString();
                                }
                                ConnectSQLite();
                                string saveposSQL = string.Format("INSERT OR REPLACE INTO Data_Pos_RB (STT, POSITION, X,Y,Z,A4,A5,C) VALUES (@STT, @POSITION, @X,@Y,@Z,@A4,@A5,@C)");
                                using (SQLiteCommand cmd = new SQLiteCommand(saveposSQL, Conn))
                                {
                                    cmd.Parameters.AddWithValue("@STT", save[0]);
                                    cmd.Parameters.AddWithValue("@POSITION", save[1]);
                                    cmd.Parameters.AddWithValue("@X", Convert.ToDouble(save[2]));
                                    cmd.Parameters.AddWithValue("@Y", Convert.ToDouble(save[3]));
                                    cmd.Parameters.AddWithValue("@Z", Convert.ToDouble(save[4]));
                                    cmd.Parameters.AddWithValue("@A4", 0.00);
                                    cmd.Parameters.AddWithValue("@A5", 0.00);
                                    cmd.Parameters.AddWithValue("@C", Convert.ToDouble(save[7]));
                                    cmd.ExecuteNonQuery();
                                    Message_Box_OK("Lưu thành công", "Save");
                                }
                                DisConSQLite();
                            }
                            else
                            {
                                MessageBox.Show("Chiều Cao Z or Y or X ngoài phạm vi cài đặt");
                            }
                        }
                        else if (row.Index == 0 || row.Index == 1 || row.Index > 6)
                        {
                            for (int i = 0; i < 1; i++)
                            {
                                row.Cells[i + 2].Value = Global.Get_Curent_XYZ_RB[0].ToString();
                                row.Cells[i + 3].Value = Global.Get_Curent_XYZ_RB[1].ToString();
                                row.Cells[i + 4].Value = Global.Get_Curent_XYZ_RB[2].ToString();
                                row.Cells[i + 7].Value = Global.Get_Curent_XYZ_RB[5].ToString();
                            }
                            for (int j = 0; j < row.Cells.Count; j++)
                            {
                                save[j] = row.Cells[j].Value.ToString();
                            }
                            ConnectSQLite();
                            string saveposSQL = string.Format("INSERT OR REPLACE INTO Data_Pos_RB (STT, POSITION, X,Y,Z,A4,A5,C) VALUES (@STT, @POSITION, @X,@Y,@Z,@A4,@A5,@C)");
                            using (SQLiteCommand cmd = new SQLiteCommand(saveposSQL, Conn))
                            {
                                cmd.Parameters.AddWithValue("@STT", save[0]);
                                cmd.Parameters.AddWithValue("@POSITION", save[1]);
                                cmd.Parameters.AddWithValue("@X", Convert.ToDouble(save[2]));
                                cmd.Parameters.AddWithValue("@Y", Convert.ToDouble(save[3]));
                                cmd.Parameters.AddWithValue("@Z", Convert.ToDouble(save[4]));
                                cmd.Parameters.AddWithValue("@A4", 0.00);
                                cmd.Parameters.AddWithValue("@A5", 0.00);
                                cmd.Parameters.AddWithValue("@C", Convert.ToDouble(save[7]));
                                cmd.ExecuteNonQuery();
                                Message_Box_OK("Lưu thành công", "Save");
                            }
                            DisConSQLite();
                        }
                    }
                    else
                    {
                        Message_Box_Error("Disconnect Robot", "Robot");
                    }
                }
            }
        }
        private void save_Offset_RB(int ID, string[] Offset)
        {
            ConnectSQLite();
            string saveposSQL = string.Format("INSERT OR REPLACE INTO OffsetRB (STT,X,Y,Z,Z_antoan,A2_antoan,SP_Home,Z_Place_Satefy,Y_Place_Satefy_L,X_Place_Satefy_U,X_Place_Satefy_L,Number_Block,Offset_Block_X,Offset_Block_Y,Number_FPCB,Offset_FPCB_X,Offset_FPCB_Y,X_Camtop_Satefy_U,X_Camtop_Satefy_L,Y_Camtop_Satefy_U,Y_Camtop_Satefy_L,Z_Camtop_Satefy,M_Total_Check,M_Offset_X_Block,M_Offset_Y_Block,M_Offset_X_FPCB,M_Offset_Y_FPCB) " +
                "VALUES (@STT,@X,@Y,@Z,@Z_antoan,@A2_antoan,@SP_Home,@Z_Place_Satefy,@Y_Place_Satefy_L,@X_Place_Satefy_U,@X_Place_Satefy_L,@Number_Block,@Offset_Block_X,@Offset_Block_Y,@Number_FPCB,@Offset_FPCB_X,@Offset_FPCB_Y,@X_Camtop_Satefy_U,@X_Camtop_Satefy_L,@Y_Camtop_Satefy_U,@Y_Camtop_Satefy_L,@Z_Camtop_Satefy,@M_Total_Check,@M_Offset_X_Block,@M_Offset_Y_Block,@M_Offset_X_FPCB,@M_Offset_Y_FPCB)");
            using (SQLiteCommand cmd = new SQLiteCommand(saveposSQL, Conn))
            {
                cmd.Parameters.AddWithValue("@STT", ID);
                cmd.Parameters.AddWithValue("@X", Convert.ToDouble(Offset[0]));
                cmd.Parameters.AddWithValue("@Y", Convert.ToDouble(Offset[1]));
                cmd.Parameters.AddWithValue("@Z", Convert.ToDouble(Offset[2]));
                cmd.Parameters.AddWithValue("@Z_antoan", Convert.ToDouble(Offset[3]));
                cmd.Parameters.AddWithValue("@A2_antoan", Convert.ToDouble(Offset[4]));
                cmd.Parameters.AddWithValue("@SP_Home", Convert.ToDouble(Offset[5]));
                cmd.Parameters.AddWithValue("@Z_Place_Satefy", Convert.ToDouble(Offset[6]));
                cmd.Parameters.AddWithValue("@Y_Place_Satefy_L", Convert.ToDouble(Offset[7]));
                cmd.Parameters.AddWithValue("@X_Place_Satefy_U", Convert.ToDouble(Offset[8]));
                cmd.Parameters.AddWithValue("@X_Place_Satefy_L", Convert.ToDouble(Offset[9]));
                //
                cmd.Parameters.AddWithValue("@Number_Block", Convert.ToDouble(Offset[10]));
                cmd.Parameters.AddWithValue("@Offset_Block_X", Convert.ToDouble(Offset[11]));
                cmd.Parameters.AddWithValue("@Offset_Block_Y", Convert.ToDouble(Offset[12]));
                cmd.Parameters.AddWithValue("@Number_FPCB", Convert.ToDouble(Offset[13]));
                cmd.Parameters.AddWithValue("@Offset_FPCB_X", Convert.ToDouble(Offset[14]));
                cmd.Parameters.AddWithValue("@Offset_FPCB_Y", Convert.ToDouble(Offset[15]));
                //
                cmd.Parameters.AddWithValue("@X_Camtop_Satefy_U", Convert.ToDouble(Offset[16]));
                cmd.Parameters.AddWithValue("@X_Camtop_Satefy_L", Convert.ToDouble(Offset[17]));
                cmd.Parameters.AddWithValue("@Y_Camtop_Satefy_U", Convert.ToDouble(Offset[18]));
                cmd.Parameters.AddWithValue("@Y_Camtop_Satefy_L", Convert.ToDouble(Offset[19]));
                cmd.Parameters.AddWithValue("@Z_Camtop_Satefy", Convert.ToDouble(Offset[20]));
                //
                cmd.Parameters.AddWithValue("@M_Total_Check", Convert.ToDouble(Offset[21]));
                cmd.Parameters.AddWithValue("@M_Offset_X_Block", Convert.ToDouble(Offset[22]));
                cmd.Parameters.AddWithValue("@M_Offset_Y_Block", Convert.ToDouble(Offset[23]));
                cmd.Parameters.AddWithValue("@M_Offset_X_FPCB", Convert.ToDouble(Offset[24]));
                cmd.Parameters.AddWithValue("@M_Offset_Y_FPCB", Convert.ToDouble(Offset[25]));
                cmd.ExecuteNonQuery();
                Message_Box_OK("Lưu thành công", "Save");
            }
            DisConSQLite();

        }
        private void save_Parameter_RB(int ID, string[] Offset)
        {
            ConnectSQLite();
            string saveposSQL = string.Format("INSERT OR REPLACE INTO Thong_so_RB (STT,SpeedAuto,Delay1,Delay2,Delay3,Delay4,Number_check,Mode_run_RB,ACC_RB,SP_Wait_Pick,Delay_Trigger_CamB, Snap) " +
                "VALUES (@STT,@SpeedAuto,@Delay1,@Delay2,@Delay3,@Delay4,@Number_check,@Mode_run_RB,@ACC_RB,@SP_Wait_Pick,@Delay_Trigger_CamB, @Snap)");
            using (SQLiteCommand cmd = new SQLiteCommand(saveposSQL, Conn))
            {
                cmd.Parameters.AddWithValue("@STT", ID);
                cmd.Parameters.AddWithValue("@SpeedAuto", Convert.ToInt16(Offset[0]));
                cmd.Parameters.AddWithValue("@Delay1", Convert.ToInt16(Offset[1]));
                cmd.Parameters.AddWithValue("@Delay2", Convert.ToInt16(Offset[2]));
                cmd.Parameters.AddWithValue("@Delay3", Convert.ToInt16(Offset[3]));
                cmd.Parameters.AddWithValue("@Delay4", Convert.ToInt16(Offset[4]));
                cmd.Parameters.AddWithValue("@Number_check", Convert.ToInt16(Offset[5]));
                cmd.Parameters.AddWithValue("@Mode_run_RB", Convert.ToInt16(Offset[6]));
                cmd.Parameters.AddWithValue("@ACC_RB", Convert.ToInt16(Offset[7]));
                cmd.Parameters.AddWithValue("@SP_Wait_Pick", Convert.ToInt16(Offset[8]));
                cmd.Parameters.AddWithValue("@Delay_Trigger_CamB", Convert.ToInt16(Offset[9]));
                cmd.Parameters.AddWithValue("@Snap", Convert.ToInt16(Offset[10]));
                cmd.ExecuteNonQuery();
                Message_Box_OK("Lưu thành công", "Save");
            }
            DisConSQLite();

        }
        private void Great_value_setup_robot()
        {
            string[] data_Pos = { txt_OffsetRB_X.Text,txt_Limit_Y_Place_U.Text, txt_offsetRB_Z.Text, txt_Z_antoan.Text, txt_A2_antoan.Text, txt_Speed_Home_RB.Text, txt_Z_Place_Satefy.Text, txt_Limit_Y_Place_L.Text, txt_Limit_X_Place_U.Text, txt_Limit_X_Place_L.Text,
            txt_number_Block_Fpcb_RB.Text, Offset_X_BlockFPCB_Tool_tray.Text, Offset_Y_BlockFPCB_Tool_tray.Text, txt_number_Fpcb_Block_RB.Text, Offset_X_FPCB_Tool_tray.Text, Offset_Y_FPCB_Tool_tray.Text,
            txt_Limit_X_Camtop_U.Text,txt_Limit_X_Camtop_L.Text,txt_Limit_Y_Camtop_U.Text,txt_Limit_Y_Camtop_L.Text,txt_Z_Camtop_Satefy.Text,
           txt_Total_Check_Marking.Text,Offset_X_BlockFPCB_Tool_Marking.Text,Offset_Y_BlockFPCB_Tool_Marking.Text,Offset_X_FPCB_Tool_Marking.Text, Offset_Y_FPCB_Tool_Marking.Text};
            int i = 1;
            Global.Offset_X = Convert.ToDouble(txt_OffsetRB_X.Text);
            Global.Y_Place_Satefy_U = Convert.ToDouble(txt_Limit_Y_Place_U.Text);
            Global.Offset_Z = Convert.ToDouble(txt_offsetRB_Z.Text);
            Global.Z_antoan = Convert.ToDouble(txt_Z_antoan.Text);
            Global.A2_antoan = Convert.ToDouble(txt_A2_antoan.Text);
            Global.SP_Home = Convert.ToDouble(txt_Speed_Home_RB.Text);
            //
            Global.Z_Place_Satefy = Convert.ToDouble(txt_Z_Place_Satefy.Text);
            Global.Y_Place_Satefy_L = Convert.ToDouble(txt_Limit_Y_Place_L.Text);
            Global.X_Place_Satefy_U = Convert.ToDouble(txt_Limit_X_Place_U.Text);
            Global.X_Place_Satefy_L = Convert.ToDouble(txt_Limit_X_Place_L.Text);
            //
            Global.X_Camtop_Satefy_U = Convert.ToDouble(txt_Limit_X_Camtop_U.Text);
            Global.X_Camtop_Satefy_L = Convert.ToDouble(txt_Limit_X_Camtop_L.Text);
            Global.Y_Camtop_Satefy_U = Convert.ToDouble(txt_Limit_Y_Camtop_U.Text);
            Global.Y_Camtop_Satefy_L = Convert.ToDouble(txt_Limit_Y_Camtop_L.Text);
            Global.Z_Camtop_Satefy = Convert.ToDouble(txt_Z_Camtop_Satefy.Text);
            //
            Global.Number_Block = Convert.ToDouble(txt_number_Block_Fpcb_RB.Text);
            Global.Offset_Block_X = Convert.ToDouble(Offset_X_BlockFPCB_Tool_tray.Text);
            Global.Offset_Block_Y = Convert.ToDouble(Offset_Y_BlockFPCB_Tool_tray.Text);
            Global.Number_FPCB = Convert.ToDouble(txt_number_Fpcb_Block_RB.Text);
            Global.Offset_FPCB_X = Convert.ToDouble(Offset_X_FPCB_Tool_tray.Text);
            Global.Offset_FPCB_Y = Convert.ToDouble(Offset_Y_FPCB_Tool_tray.Text);
            //
            Global.Offset_Block_X_marking = Convert.ToDouble(Offset_X_BlockFPCB_Tool_Marking.Text);
            Global.Offset_Block_Y_Marking = Convert.ToDouble(Offset_Y_BlockFPCB_Tool_Marking.Text);
            Global.Offset_FPCB_X_Marking = Convert.ToDouble(Offset_X_FPCB_Tool_Marking.Text);
            Global.Offset_FPCB_Y_Marking = Convert.ToDouble(Offset_Y_FPCB_Tool_Marking.Text);
            Global.Total_Check_Marking = Convert.ToDouble(txt_Total_Check_Marking.Text);
            save_Offset_RB(i, data_Pos);
        }
        private void Great_Value_Place_tray()
        {
            try
            {
                ConnectSQLite();
                using (var cmd = new SQLiteCommand("DELETE FROM Matrix_Panel_Tool_1_1;", Conn))
                {
                    cmd.ExecuteNonQuery();
                }
                DisConSQLite();
                PLC1.Write_Data_Word_("D" + (9006 + Memory_PLC.K2000).ToString(), Convert.ToInt16(txt_Row1_x10.Text));
                PLC1.Write_Data_Word_("D" + (9007 + Memory_PLC.K2000).ToString(), Convert.ToInt16(txt_Column1_x10.Text));
                PLC1.Write_Data_Word_("D" + (9008 + Memory_PLC.K2000).ToString(), Convert.ToInt16(txt_Row2_x10.Text));
                PLC1.Write_Data_Word_("D" + (9009 + Memory_PLC.K2000).ToString(), Convert.ToInt16(txt_Column2_x10.Text));
                PLC1.Write_Data_Word_("D" + (9010 + Memory_PLC.K2000).ToString(), Convert.ToInt16(txt_Row3_x8.Text));
                PLC1.Write_Data_Word_("D" + (9011 + Memory_PLC.K2000).ToString(), Convert.ToInt16(txt_Column3_x8.Text));
                PLC1.Write_Data_DWord_("D" + (9014 + Memory_PLC.K2000).ToString(), Convert.ToInt32(Convert.ToDouble(txt_distance_row.Text) * 1000));
                PLC1.Write_Data_DWord_("D" + (9016 + Memory_PLC.K2000).ToString(), Convert.ToInt32(Convert.ToDouble(txt_distance_column.Text) * 1000));
                //
                int row_matrix_1 = Convert.ToInt16(txt_Row1_x10.Text);
                int column_matrix_1 = Convert.ToInt16(txt_Column1_x10.Text);
                int row_matrix_2 = Convert.ToInt16(txt_Row2_x10.Text);
                int column_matrix_2 = Convert.ToInt16(txt_Column2_x10.Text);
                int row_matrix_3 = Convert.ToInt16(txt_Row3_x8.Text);
                int column_matrix_3 = Convert.ToInt16(txt_Column3_x8.Text);
                Global.Row_tray_matrix1 = row_matrix_1;
                Global.Column_tray_matrix1 = column_matrix_1;
                Global.Row_tray_matrix2 = row_matrix_1;
                Global.Column_tray_matrix2 = column_matrix_1;
                Global.Row_tray_matrix3 = row_matrix_3;
                Global.Column_tray_matrix3 = column_matrix_3;
                //
                double distance_tray_X = 0;
                double distance_tray_Y = 0;
                double distance_alpha = Convert.ToDouble(Offset_Y_BlockFPCB_Tool_tray.Text);
                if (RadioButton_Chieu_Y.Checked == true)
                {
                    distance_tray_X = Convert.ToDouble(txt_distance_column.Text);
                    distance_tray_Y = Convert.ToDouble(txt_distance_row.Text);
                }
                else if (RadioButton_Chieu_X.Checked == true)
                {
                    distance_tray_X = Convert.ToDouble(txt_distance_row.Text);
                    distance_tray_Y = Convert.ToDouble(txt_distance_column.Text);
                }
                double offset_X_Fpcb = Global.Offset_FPCB_X;
                double offset_Y_Fpcb = Global.Offset_FPCB_Y;
                if (CheckBox_Use_Matrix1.Checked == true)
                {
                    #region Maxtrix 1
                    //1
                    double[] data_pos_Matrix1_1 = new double[6];
                    double[] data_pos_Matrix1_2 = new double[6];
                    double[] data_pos_Matrix1_3 = new double[6];
                    if (RadioButton_Chieu_Y.Checked == true)
                    {
                        data_pos_Matrix1_1 = Global.Place_Tray_1;
                        data_pos_Matrix1_2 = new double[6] { Global.Place_Tray_1[0] - distance_alpha, Global.Place_Tray_1[1] + distance_tray_Y * (row_matrix_1 - 1), Global.Place_Tray_1[2], Global.Place_Tray_1[3], Global.Place_Tray_1[4], Global.Place_Tray_1[5] };
                        data_pos_Matrix1_3 = new double[6] { Global.Place_Tray_1[0] - distance_tray_X * (column_matrix_1 - 1), Global.Place_Tray_1[1], Global.Place_Tray_1[2], Global.Place_Tray_1[3], Global.Place_Tray_1[4], Global.Place_Tray_1[5] };
                    }
                    else if (RadioButton_Chieu_X.Checked == true)
                    {
                        data_pos_Matrix1_1 = Global.Place_Tray_1;
                        data_pos_Matrix1_2 = new double[6] { Global.Place_Tray_1[0] + distance_tray_X * (row_matrix_1 - 1), Global.Place_Tray_1[1], Global.Place_Tray_1[2], Global.Place_Tray_1[3], Global.Place_Tray_1[4], Global.Place_Tray_1[5] };
                        data_pos_Matrix1_3 = new double[6] { Global.Place_Tray_1[0] - distance_alpha, Global.Place_Tray_1[1] - distance_tray_Y * (column_matrix_1 - 1), Global.Place_Tray_1[2], Global.Place_Tray_1[3], Global.Place_Tray_1[4], Global.Place_Tray_1[5] };
                    }
                    //1-10
                    List<double>[] Matrix_tray1x10_1 = new List<double>[100];
                    List<double>[] Matrix_tray1x10_2 = new List<double>[100];
                    List<double>[] Matrix_tray1x10_3 = new List<double>[100];
                    double[] Array_X_x10_1 = new double[Convert.ToInt16(Global.Number_FPCB)];//array 1 x10                                                     
                    double[] Array_Y_x10_1 = new double[Convert.ToInt16(Global.Number_FPCB)];//array 1 x10  
                    double[] Array_X_x10_2 = new double[Convert.ToInt16(Global.Number_FPCB)];//array 1 x10                                                     
                    double[] Array_Y_x10_2 = new double[Convert.ToInt16(Global.Number_FPCB)];//array 1 x10  
                    double[] Array_X_x10_3 = new double[Convert.ToInt16(Global.Number_FPCB)];//array 1 x10                                                     
                    double[] Array_Y_x10_3 = new double[Convert.ToInt16(Global.Number_FPCB)];//array 1 x10  

                    // điểm 1                                                       
                    for (int i = 0; i < Global.Number_FPCB; i++)
                    {
                        Array_X_x10_1[i] = data_pos_Matrix1_1[0] - offset_X_Fpcb * i;
                        Array_Y_x10_1[i] = data_pos_Matrix1_1[1] + offset_Y_Fpcb * i;
                    }
                    //điểm 2
                    for (int i = 0; i < Global.Number_FPCB; i++)
                    {
                        Array_X_x10_2[i] = data_pos_Matrix1_2[0] - offset_X_Fpcb * i;
                        Array_Y_x10_2[i] = data_pos_Matrix1_2[1] + offset_Y_Fpcb * i;
                    }
                    //điểm 3
                    for (int i = 0; i < Global.Number_FPCB; i++)
                    {
                        Array_X_x10_3[i] = data_pos_Matrix1_3[0] - offset_X_Fpcb * i;
                        Array_Y_x10_3[i] = data_pos_Matrix1_3[1] + offset_Y_Fpcb * i;
                    }
                    for (int i = 0; i < Global.Number_FPCB; i++)
                    {
                        Matrix_tray1x10_1[i] = new List<double>();
                        Matrix_tray1x10_1[i].Add(Array_X_x10_1[i]);
                        Matrix_tray1x10_1[i].Add(Array_Y_x10_1[i]);
                        Matrix_tray1x10_1[i].Add(Global.Place_Tray_1[2]);
                        Matrix_tray1x10_1[i].Add(Global.Place_Tray_1[3]);
                        Matrix_tray1x10_1[i].Add(Global.Place_Tray_1[4]);
                        Matrix_tray1x10_1[i].Add(Global.Place_Tray_1[5]);
                    }
                    for (int i = 0; i < Global.Number_FPCB; i++)
                    {
                        Matrix_tray1x10_2[i] = new List<double>();
                        Matrix_tray1x10_2[i].Add(Array_X_x10_2[i]);
                        Matrix_tray1x10_2[i].Add(Array_Y_x10_2[i]);
                        Matrix_tray1x10_2[i].Add(Global.Place_Tray_1[2]);
                        Matrix_tray1x10_2[i].Add(Global.Place_Tray_1[3]);
                        Matrix_tray1x10_2[i].Add(Global.Place_Tray_1[4]);
                        Matrix_tray1x10_2[i].Add(Global.Place_Tray_1[5]);
                    }
                    for (int i = 0; i < Global.Number_FPCB; i++)
                    {
                        Matrix_tray1x10_3[i] = new List<double>();
                        Matrix_tray1x10_3[i].Add(Array_X_x10_3[i]);
                        Matrix_tray1x10_3[i].Add(Array_Y_x10_3[i]);
                        Matrix_tray1x10_3[i].Add(Global.Place_Tray_1[2]);
                        Matrix_tray1x10_3[i].Add(Global.Place_Tray_1[3]);
                        Matrix_tray1x10_3[i].Add(Global.Place_Tray_1[4]);
                        Matrix_tray1x10_3[i].Add(Global.Place_Tray_1[5]);
                    }
                    List<double>[] matrix_panel_1 = new List<double>[10];
                    List<double>[] matrix_panel_2 = new List<double>[10];
                    List<double>[] matrix_panel_3 = new List<double>[10];
                    matrix_panel_1 = Matrix_tray1x10_1;
                    matrix_panel_2 = Matrix_tray1x10_2;
                    matrix_panel_3 = Matrix_tray1x10_3;
                    for (int i = 0; i < Global.Number_FPCB; i++)
                    {
                        matrix.PAL_PR_RB_1_1(i + 1, row_matrix_1, column_matrix_1, Matrix_tray1x10_1, Matrix_tray1x10_2, Matrix_tray1x10_3, i);
                    }
                    #endregion
                }
                if (CheckBox_Use_Matrix2.Checked == true)
                {
                    #region Matrix 2
                    //1
                    double[] data_pos_Matrix2_1 = new double[6];
                    double[] data_pos_Matrix2_2 = new double[6];
                    double[] data_pos_Matrix2_3 = new double[6];
                    if (RadioButton_Chieu_Y.Checked == true)
                    {
                        data_pos_Matrix2_1 = Global.Place_Tray_2;
                        data_pos_Matrix2_2 = new double[6] { Global.Place_Tray_2[0] - distance_alpha, Global.Place_Tray_2[1] + distance_tray_Y * (row_matrix_2 - 1), Global.Place_Tray_2[2], Global.Place_Tray_2[3], Global.Place_Tray_2[4], Global.Place_Tray_2[5] };
                        data_pos_Matrix2_3 = new double[6] { Global.Place_Tray_2[0] - distance_tray_X * (column_matrix_2 - 1), Global.Place_Tray_2[1], Global.Place_Tray_2[2], Global.Place_Tray_2[3], Global.Place_Tray_2[4], Global.Place_Tray_2[5] };
                    }
                    else if (RadioButton_Chieu_X.Checked == true)
                    {
                        data_pos_Matrix2_1 = Global.Place_Tray_2;
                        data_pos_Matrix2_2 = new double[6] { Global.Place_Tray_2[0] + distance_tray_X * (row_matrix_2 - 1), Global.Place_Tray_2[1], Global.Place_Tray_2[2], Global.Place_Tray_2[3], Global.Place_Tray_2[4], Global.Place_Tray_2[5] };
                        data_pos_Matrix2_3 = new double[6] { Global.Place_Tray_2[0] - distance_alpha, Global.Place_Tray_2[1] - distance_tray_Y * (column_matrix_2 - 1), Global.Place_Tray_2[2], Global.Place_Tray_2[3], Global.Place_Tray_2[4], Global.Place_Tray_2[5] };
                    }
                    //1-10
                    List<double>[] Matrix_tray1x10_2_1 = new List<double>[100];
                    List<double>[] Matrix_tray1x10_2_2 = new List<double>[100];
                    List<double>[] Matrix_tray1x10_2_3 = new List<double>[100];
                    double[] Array_X_x10_2_1 = new double[Convert.ToInt16(Global.Number_FPCB)];//array 1 x10                                                     //
                    double[] Array_Y_x10_2_1 = new double[Convert.ToInt16(Global.Number_FPCB)];//array 1 x10  
                    double[] Array_X_x10_2_2 = new double[Convert.ToInt16(Global.Number_FPCB)];//array 1 x10                                                     //
                    double[] Array_Y_x10_2_2 = new double[Convert.ToInt16(Global.Number_FPCB)];//array 1 x10  
                    double[] Array_X_x10_2_3 = new double[Convert.ToInt16(Global.Number_FPCB)];//array 1 x10                                                     //
                    double[] Array_Y_x10_2_3 = new double[Convert.ToInt16(Global.Number_FPCB)];//array 1 x10  
                                                                                               // điểm 1
                    for (int i = 0; i < Global.Number_FPCB; i++)
                    {
                        Array_X_x10_2_1[i] = data_pos_Matrix2_1[0] - offset_X_Fpcb * i;
                        Array_Y_x10_2_1[i] = data_pos_Matrix2_1[1] + offset_Y_Fpcb * i;
                    }
                    //điểm 2
                    for (int i = 0; i < Global.Number_FPCB; i++)
                    {
                        Array_X_x10_2_2[i] = data_pos_Matrix2_2[0] - offset_X_Fpcb * i;
                        Array_Y_x10_2_2[i] = data_pos_Matrix2_2[1] + offset_Y_Fpcb * i;
                    }
                    //điểm 3
                    for (int i = 0; i < Global.Number_FPCB; i++)
                    {
                        Array_X_x10_2_3[i] = data_pos_Matrix2_3[0] - offset_X_Fpcb * i;
                        Array_Y_x10_2_3[i] = data_pos_Matrix2_3[1] + offset_Y_Fpcb * i;
                    }
                    for (int i = 0; i < Global.Number_FPCB; i++)
                    {
                        Matrix_tray1x10_2_1[i] = new List<double>();
                        Matrix_tray1x10_2_1[i].Add(Array_X_x10_2_1[i]);
                        Matrix_tray1x10_2_1[i].Add(Array_Y_x10_2_1[i]);
                        Matrix_tray1x10_2_1[i].Add(Global.Place_Tray_2[2]);
                        Matrix_tray1x10_2_1[i].Add(Global.Place_Tray_2[3]);
                        Matrix_tray1x10_2_1[i].Add(Global.Place_Tray_2[4]);
                        Matrix_tray1x10_2_1[i].Add(Global.Place_Tray_2[5]);
                    }
                    for (int i = 0; i < Global.Number_FPCB; i++)
                    {
                        Matrix_tray1x10_2_2[i] = new List<double>();
                        Matrix_tray1x10_2_2[i].Add(Array_X_x10_2_2[i]);
                        Matrix_tray1x10_2_2[i].Add(Array_Y_x10_2_2[i]);
                        Matrix_tray1x10_2_2[i].Add(Global.Place_Tray_2[2]);
                        Matrix_tray1x10_2_2[i].Add(Global.Place_Tray_2[3]);
                        Matrix_tray1x10_2_2[i].Add(Global.Place_Tray_2[4]);
                        Matrix_tray1x10_2_2[i].Add(Global.Place_Tray_2[5]);
                    }
                    for (int i = 0; i < Global.Number_FPCB; i++)
                    {
                        Matrix_tray1x10_2_3[i] = new List<double>();
                        Matrix_tray1x10_2_3[i].Add(Array_X_x10_2_3[i]);
                        Matrix_tray1x10_2_3[i].Add(Array_Y_x10_2_3[i]);
                        Matrix_tray1x10_2_3[i].Add(Global.Place_Tray_2[2]);
                        Matrix_tray1x10_2_3[i].Add(Global.Place_Tray_2[3]);
                        Matrix_tray1x10_2_3[i].Add(Global.Place_Tray_2[4]);
                        Matrix_tray1x10_2_3[i].Add(Global.Place_Tray_2[5]);
                    }
                    for (int i = 0; i < Global.Number_FPCB; i++)
                    {
                        matrix.PAL_PR_RB_1_1(Convert.ToInt32(Global.Number_FPCB) + 1 + i, row_matrix_2, column_matrix_2, Matrix_tray1x10_2_1, Matrix_tray1x10_2_2, Matrix_tray1x10_2_3, i);
                    }
                    #endregion
                }
                //#region Matrix 3
                ////1
                //double[] data_pos_Matrix3_1 = Global.Place_Tray_3;
                //double[] data_pos_Matrix3_2 = { Global.Place_Tray_3[0], Global.Place_Tray_3[1] + distance_tray_Y * 13, Global.Place_Tray_3[2], Global.Place_Tray_3[3], Global.Place_Tray_3[4], Global.Place_Tray_3[5] };
                //double[] data_pos_Matrix3_3 = { Global.Place_Tray_3[0] - distance_tray_X, Global.Place_Tray_3[1], Global.Place_Tray_3[2], Global.Place_Tray_3[3], Global.Place_Tray_3[4], Global.Place_Tray_3[5] };
                ////1-10
                //List<double>[] Matrix_tray1x10_3_1 = new List<double>[100];
                //List<double>[] Matrix_tray1x10_3_2 = new List<double>[100];
                //List<double>[] Matrix_tray1x10_3_3 = new List<double>[100];
                //double[] Array_X_x10_3_1 = new double[Convert.ToInt16(10)];//array 1 x10                                                 
                //double[] Array_Y_x10_3_1 = new double[Convert.ToInt16(10)];//array 1 x10 
                //double[] Array_X_x10_3_2 = new double[Convert.ToInt16(10)];//array 1 x10  
                //double[] Array_Y_x10_3_2 = new double[Convert.ToInt16(10)];//array 1 x10  
                //double[] Array_X_x10_3_3 = new double[Convert.ToInt16(10)];//array 1 x10 
                //double[] Array_Y_x10_3_3 = new double[Convert.ToInt16(10)];//array 1 x10  
                //                                                           // điểm 1
                //for (int i = 0; i < 10; i++)
                //{
                //    Array_X_x10_3_1[i] = data_pos_Matrix3_1[0] - offset_X_Fpcb * i;
                //    Array_Y_x10_3_1[i] = data_pos_Matrix3_1[1] - offset_Y_Fpcb * i;
                //}
                ////điểm 2
                //for (int i = 0; i < 10; i++)
                //{
                //    Array_X_x10_3_2[i] = data_pos_Matrix3_2[0] - offset_X_Fpcb * i;
                //    Array_Y_x10_3_2[i] = data_pos_Matrix3_2[1] - offset_Y_Fpcb * i;
                //}
                ////điểm 3
                //for (int i = 0; i < 10; i++)
                //{
                //    Array_X_x10_3_3[i] = data_pos_Matrix3_3[0] - offset_X_Fpcb * i;
                //    Array_Y_x10_3_3[i] = data_pos_Matrix3_3[1] - offset_Y_Fpcb * i;
                //}
                //for (int i = 0; i < 10; i++)
                //{
                //    Matrix_tray1x10_3_1[i] = new List<double>();
                //    Matrix_tray1x10_3_1[i].Add(Array_X_x10_3_1[i]);
                //    Matrix_tray1x10_3_1[i].Add(Array_Y_x10_3_1[i]);
                //    Matrix_tray1x10_3_1[i].Add(Global.Place_Tray_3[2]);
                //    Matrix_tray1x10_3_1[i].Add(Global.Place_Tray_3[3]);
                //    Matrix_tray1x10_3_1[i].Add(Global.Place_Tray_3[4]);
                //    Matrix_tray1x10_3_1[i].Add(Global.Place_Tray_3[5]);
                //}
                //for (int i = 0; i < 10; i++)
                //{
                //    Matrix_tray1x10_3_2[i] = new List<double>();
                //    Matrix_tray1x10_3_2[i].Add(Array_X_x10_3_2[i]);
                //    Matrix_tray1x10_3_2[i].Add(Array_Y_x10_3_2[i]);
                //    Matrix_tray1x10_3_2[i].Add(Global.Place_Tray_3[2]);
                //    Matrix_tray1x10_3_2[i].Add(Global.Place_Tray_3[3]);
                //    Matrix_tray1x10_3_2[i].Add(Global.Place_Tray_3[4]);
                //    Matrix_tray1x10_3_2[i].Add(Global.Place_Tray_3[5]);
                //}
                //for (int i = 0; i < 10; i++)
                //{
                //    Matrix_tray1x10_3_3[i] = new List<double>();
                //    Matrix_tray1x10_3_3[i].Add(Array_X_x10_3_3[i]);
                //    Matrix_tray1x10_3_3[i].Add(Array_Y_x10_3_3[i]);
                //    Matrix_tray1x10_3_3[i].Add(Global.Place_Tray_3[2]);
                //    Matrix_tray1x10_3_3[i].Add(Global.Place_Tray_3[3]);
                //    Matrix_tray1x10_3_3[i].Add(Global.Place_Tray_3[4]);
                //    Matrix_tray1x10_3_3[i].Add(Global.Place_Tray_3[5]);
                //}
                //for (int i = 0; i < 10; i++)
                //{
                //    matrix.PAL_PR_RB_1_3(i + 1, row_matrix_3, column_matrix_3, Matrix_tray1x10_3_1, Matrix_tray1x10_3_2, Matrix_tray1x10_3_3, i);
                //}
                //#endregion          
                //Scan Điều kiện Pos thả tray theo parameter Satefy 
                if (Scan_Position == 2 && Global.Home_All == false)
                {
                    if (Global.Place_Tray_1[0] > Global.X_Place_Satefy_L && Global.Place_Tray_1[0] < Global.X_Place_Satefy_U && Global.Place_Tray_1[1] < Global.Y_Place_Satefy_U && Global.Place_Tray_1[1] > Global.Y_Place_Satefy_L && Global.Place_Tray_1[2] > Global.Z_Place_Satefy
                       && Global.Place_Tray_2[0] > Global.X_Place_Satefy_L && Global.Place_Tray_2[0] < Global.X_Place_Satefy_U && Global.Place_Tray_2[1] < Global.Y_Place_Satefy_U && Global.Place_Tray_2[1] > Global.Y_Place_Satefy_L && Global.Place_Tray_2[2] > Global.Z_Place_Satefy
                      && Global.Place_Tray_3[0] > Global.X_Place_Satefy_L && Global.Place_Tray_3[0] < Global.X_Place_Satefy_U && Global.Place_Tray_3[1] < Global.Y_Place_Satefy_U && Global.Place_Tray_3[1] > Global.Y_Place_Satefy_L && Global.Place_Tray_3[2] > Global.Z_Place_Satefy)
                    {
                        //code
                    }
                    else
                    {
                        WaitFormHelper.Close();
                        MessageBox.Show("Kiêm tra lại tọa độ Maxtrix trong Teaching 1 Robot");
                    }
                }
                else
                {
                    WaitFormHelper.Close();
                    MessageBox.Show("Kiểm tra lại tọa độ Z của các Position Matrix trong Teaching 1 Robot");
                }
                WaitFormHelper.Close();
                Message_Box_OK("Write data position Place tray thành công", "Writing...");
            }
            catch (Exception e)
            {
                WaitFormHelper.Close();
                Message_Box_Error(e.ToString(), "Great_Value_Place_tray");
            }
        }
        private void Great_Value_Place_tray1()
        {
            try
            {
                ConnectSQLite();
                using (var cmd = new SQLiteCommand("DELETE FROM Matrix_Panel_Tool_1_1;", Conn))
                {
                    cmd.ExecuteNonQuery();
                }
                DisConSQLite();
                PLC1.Write_Data_Word_("D" + (9006 + Memory_PLC.K2000).ToString(), Convert.ToInt16(txt_Row1_x10.Text));
                PLC1.Write_Data_Word_("D" + (9007 + Memory_PLC.K2000).ToString(), Convert.ToInt16(txt_Column1_x10.Text));
                PLC1.Write_Data_Word_("D" + (9008 + Memory_PLC.K2000).ToString(), Convert.ToInt16(txt_Row2_x10.Text));
                PLC1.Write_Data_Word_("D" + (9009 + Memory_PLC.K2000).ToString(), Convert.ToInt16(txt_Column2_x10.Text));
                PLC1.Write_Data_Word_("D" + (9010 + Memory_PLC.K2000).ToString(), Convert.ToInt16(txt_Row3_x8.Text));
                PLC1.Write_Data_Word_("D" + (9011 + Memory_PLC.K2000).ToString(), Convert.ToInt16(txt_Column3_x8.Text));
                PLC1.Write_Data_DWord_("D" + (9014 + Memory_PLC.K2000).ToString(), Convert.ToInt32(Convert.ToDouble(txt_distance_row.Text) * 1000));
                PLC1.Write_Data_DWord_("D" + (9016 + Memory_PLC.K2000).ToString(), Convert.ToInt32(Convert.ToDouble(txt_distance_column.Text) * 1000));
                //
                int row_matrix_1 = Convert.ToInt16(txt_Row1_x10.Text);
                int column_matrix_1 = Convert.ToInt16(txt_Column1_x10.Text);
                int row_matrix_2 = Convert.ToInt16(txt_Row2_x10.Text);
                int column_matrix_2 = Convert.ToInt16(txt_Column2_x10.Text);
                int row_matrix_3 = Convert.ToInt16(txt_Row3_x8.Text);
                int column_matrix_3 = Convert.ToInt16(txt_Column3_x8.Text);
                Global.Row_tray_matrix1 = row_matrix_1;
                Global.Column_tray_matrix1 = column_matrix_1;
                Global.Row_tray_matrix2 = row_matrix_1;
                Global.Column_tray_matrix2 = column_matrix_1;
                Global.Row_tray_matrix3 = row_matrix_3;
                Global.Column_tray_matrix3 = column_matrix_3;
                //
                double distance_tray_X = 0;
                double distance_tray_Y = 0;
                double distance_alpha = Convert.ToDouble(Offset_Y_BlockFPCB_Tool_tray.Text);
                if (RadioButton_Chieu_Y.Checked == true)
                {
                    distance_tray_X = Convert.ToDouble(txt_distance_column.Text);
                    distance_tray_Y = Convert.ToDouble(txt_distance_row.Text);
                }
                else if (RadioButton_Chieu_X.Checked == true)
                {
                    distance_tray_X = Convert.ToDouble(txt_distance_row.Text);
                    distance_tray_Y = Convert.ToDouble(txt_distance_column.Text);
                }
                double offset_X_Fpcb = Global.Offset_FPCB_X;
                double offset_Y_Fpcb = Global.Offset_FPCB_Y;
                if (CheckBox_Use_Matrix1.Checked == true)
                {
                    #region Maxtrix 1
                    //1
                    double[] data_pos_Matrix1_1 = new double[6];
                    double[] data_pos_Matrix1_2 = new double[6];
                    double[] data_pos_Matrix1_3 = new double[6];
                    if (RadioButton_Chieu_Y.Checked == true)
                    {
                        data_pos_Matrix1_1 = Global.Place_Tray_1;
                        data_pos_Matrix1_2 = new double[6] { Global.Place_Tray_1[0], Global.Place_Tray_1[1] + distance_tray_Y * (row_matrix_1 - 1), Global.Place_Tray_1[2], Global.Place_Tray_1[3], Global.Place_Tray_1[4], Global.Place_Tray_1[5] };
                        data_pos_Matrix1_3 = new double[6] { Global.Place_Tray_1[0] - distance_tray_X * (column_matrix_1 - 1), Global.Place_Tray_1[1], Global.Place_Tray_1[2], Global.Place_Tray_1[3], Global.Place_Tray_1[4], Global.Place_Tray_1[5] };
                    }
                    else if (RadioButton_Chieu_X.Checked == true)
                    {
                        data_pos_Matrix1_1 = Global.Place_Tray_1;
                        data_pos_Matrix1_2 = new double[6] { Global.Place_Tray_1[0] + distance_tray_X * (row_matrix_1 - 1), Global.Place_Tray_1[1], Global.Place_Tray_1[2], Global.Place_Tray_1[3], Global.Place_Tray_1[4], Global.Place_Tray_1[5] };
                        data_pos_Matrix1_3 = new double[6] { Global.Place_Tray_1[0], Global.Place_Tray_1[1] - distance_tray_Y * (column_matrix_1 - 1), Global.Place_Tray_1[2], Global.Place_Tray_1[3], Global.Place_Tray_1[4], Global.Place_Tray_1[5] };
                    }
                    //1-10
                    List<double>[] Matrix_tray1x10_1 = new List<double>[100];
                    List<double>[] Matrix_tray1x10_2 = new List<double>[100];
                    List<double>[] Matrix_tray1x10_3 = new List<double>[100];
                    double[] Array_X_x10_1 = new double[Convert.ToInt16(Global.Number_FPCB)];//array 1 x10                                                     
                    double[] Array_Y_x10_1 = new double[Convert.ToInt16(Global.Number_FPCB)];//array 1 x10  
                    double[] Array_X_x10_2 = new double[Convert.ToInt16(Global.Number_FPCB)];//array 1 x10                                                     
                    double[] Array_Y_x10_2 = new double[Convert.ToInt16(Global.Number_FPCB)];//array 1 x10  
                    double[] Array_X_x10_3 = new double[Convert.ToInt16(Global.Number_FPCB)];//array 1 x10                                                     
                    double[] Array_Y_x10_3 = new double[Convert.ToInt16(Global.Number_FPCB)];//array 1 x10  

                    // điểm 1                                                       
                    for (int i = 0; i < Global.Number_FPCB; i++)
                    {
                        Array_X_x10_1[i] = data_pos_Matrix1_1[0] - offset_X_Fpcb * i;
                        Array_Y_x10_1[i] = data_pos_Matrix1_1[1] + offset_Y_Fpcb * i;
                    }
                    //điểm 2
                    for (int i = 0; i < Global.Number_FPCB; i++)
                    {
                        Array_X_x10_2[i] = data_pos_Matrix1_2[0] - offset_X_Fpcb * i;
                        Array_Y_x10_2[i] = data_pos_Matrix1_2[1] + offset_Y_Fpcb * i;
                    }
                    //điểm 3
                    for (int i = 0; i < Global.Number_FPCB; i++)
                    {
                        Array_X_x10_3[i] = data_pos_Matrix1_3[0] - offset_X_Fpcb * i;
                        Array_Y_x10_3[i] = data_pos_Matrix1_3[1] + offset_Y_Fpcb * i;
                    }
                    for (int i = 0; i < Global.Number_FPCB; i++)
                    {
                        Matrix_tray1x10_1[i] = new List<double>();
                        Matrix_tray1x10_1[i].Add(Array_X_x10_1[i]);
                        Matrix_tray1x10_1[i].Add(Array_Y_x10_1[i]);
                        Matrix_tray1x10_1[i].Add(Global.Place_Tray_1[2]);
                        Matrix_tray1x10_1[i].Add(Global.Place_Tray_1[3]);
                        Matrix_tray1x10_1[i].Add(Global.Place_Tray_1[4]);
                        Matrix_tray1x10_1[i].Add(Global.Place_Tray_1[5]);
                    }
                    for (int i = 0; i < Global.Number_FPCB; i++)
                    {
                        Matrix_tray1x10_2[i] = new List<double>();
                        Matrix_tray1x10_2[i].Add(Array_X_x10_2[i]);
                        Matrix_tray1x10_2[i].Add(Array_Y_x10_2[i]);
                        Matrix_tray1x10_2[i].Add(Global.Place_Tray_1[2]);
                        Matrix_tray1x10_2[i].Add(Global.Place_Tray_1[3]);
                        Matrix_tray1x10_2[i].Add(Global.Place_Tray_1[4]);
                        Matrix_tray1x10_2[i].Add(Global.Place_Tray_1[5]);
                    }
                    for (int i = 0; i < Global.Number_FPCB; i++)
                    {
                        Matrix_tray1x10_3[i] = new List<double>();
                        Matrix_tray1x10_3[i].Add(Array_X_x10_3[i]);
                        Matrix_tray1x10_3[i].Add(Array_Y_x10_3[i]);
                        Matrix_tray1x10_3[i].Add(Global.Place_Tray_1[2]);
                        Matrix_tray1x10_3[i].Add(Global.Place_Tray_1[3]);
                        Matrix_tray1x10_3[i].Add(Global.Place_Tray_1[4]);
                        Matrix_tray1x10_3[i].Add(Global.Place_Tray_1[5]);
                    }
                    List<double>[] matrix_panel_1 = new List<double>[10];
                    List<double>[] matrix_panel_2 = new List<double>[10];
                    List<double>[] matrix_panel_3 = new List<double>[10];
                    matrix_panel_1 = Matrix_tray1x10_1;
                    matrix_panel_2 = Matrix_tray1x10_2;
                    matrix_panel_3 = Matrix_tray1x10_3;
                    for (int i = 0; i < Global.Number_FPCB; i++)
                    {
                        matrix.PAL_PR_RB_1_1(i + 1, row_matrix_1, column_matrix_1, Matrix_tray1x10_1, Matrix_tray1x10_2, Matrix_tray1x10_3, i);
                    }
                    #endregion
                }
                if (CheckBox_Use_Matrix2.Checked == true)
                {
                    #region Matrix 2
                    //1
                    double[] data_pos_Matrix2_1 = new double[6];
                    double[] data_pos_Matrix2_2 = new double[6];
                    double[] data_pos_Matrix2_3 = new double[6];
                    if (RadioButton_Chieu_Y.Checked == true)
                    {
                        data_pos_Matrix2_1 = Global.Place_Tray_2;
                        data_pos_Matrix2_2 = new double[6] { Global.Place_Tray_2[0] - distance_alpha, Global.Place_Tray_2[1] + distance_tray_Y * (row_matrix_2 - 1), Global.Place_Tray_2[2], Global.Place_Tray_2[3], Global.Place_Tray_2[4], Global.Place_Tray_2[5] };
                        data_pos_Matrix2_3 = new double[6] { Global.Place_Tray_2[0] - distance_tray_X * (column_matrix_2 - 1), Global.Place_Tray_2[1], Global.Place_Tray_2[2], Global.Place_Tray_2[3], Global.Place_Tray_2[4], Global.Place_Tray_2[5] };
                    }
                    else if (RadioButton_Chieu_X.Checked == true)
                    {
                        data_pos_Matrix2_1 = Global.Place_Tray_2;
                        data_pos_Matrix2_2 = new double[6] { Global.Place_Tray_2[0] + distance_tray_X * (row_matrix_2 - 1), Global.Place_Tray_2[1], Global.Place_Tray_2[2], Global.Place_Tray_2[3], Global.Place_Tray_2[4], Global.Place_Tray_2[5] };
                        data_pos_Matrix2_3 = new double[6] { Global.Place_Tray_2[0] - distance_alpha, Global.Place_Tray_2[1] - distance_tray_Y * (column_matrix_2 - 1), Global.Place_Tray_2[2], Global.Place_Tray_2[3], Global.Place_Tray_2[4], Global.Place_Tray_2[5] };
                    }
                    //1-10
                    List<double>[] Matrix_tray1x10_2_1 = new List<double>[100];
                    List<double>[] Matrix_tray1x10_2_2 = new List<double>[100];
                    List<double>[] Matrix_tray1x10_2_3 = new List<double>[100];
                    double[] Array_X_x10_2_1 = new double[Convert.ToInt16(Global.Number_FPCB)];//array 1 x10                                                     //
                    double[] Array_Y_x10_2_1 = new double[Convert.ToInt16(Global.Number_FPCB)];//array 1 x10  
                    double[] Array_X_x10_2_2 = new double[Convert.ToInt16(Global.Number_FPCB)];//array 1 x10                                                     //
                    double[] Array_Y_x10_2_2 = new double[Convert.ToInt16(Global.Number_FPCB)];//array 1 x10  
                    double[] Array_X_x10_2_3 = new double[Convert.ToInt16(Global.Number_FPCB)];//array 1 x10                                                     //
                    double[] Array_Y_x10_2_3 = new double[Convert.ToInt16(Global.Number_FPCB)];//array 1 x10  
                                                                                               // điểm 1
                    for (int i = 0; i < Global.Number_FPCB; i++)
                    {
                        Array_X_x10_2_1[i] = data_pos_Matrix2_1[0] - offset_X_Fpcb * i;
                        Array_Y_x10_2_1[i] = data_pos_Matrix2_1[1] + offset_Y_Fpcb * i;
                    }
                    //điểm 2
                    for (int i = 0; i < Global.Number_FPCB; i++)
                    {
                        Array_X_x10_2_2[i] = data_pos_Matrix2_2[0] - offset_X_Fpcb * i;
                        Array_Y_x10_2_2[i] = data_pos_Matrix2_2[1] + offset_Y_Fpcb * i;
                    }
                    //điểm 3
                    for (int i = 0; i < Global.Number_FPCB; i++)
                    {
                        Array_X_x10_2_3[i] = data_pos_Matrix2_3[0] - offset_X_Fpcb * i;
                        Array_Y_x10_2_3[i] = data_pos_Matrix2_3[1] + offset_Y_Fpcb * i;
                    }
                    for (int i = 0; i < Global.Number_FPCB; i++)
                    {
                        Matrix_tray1x10_2_1[i] = new List<double>();
                        Matrix_tray1x10_2_1[i].Add(Array_X_x10_2_1[i]);
                        Matrix_tray1x10_2_1[i].Add(Array_Y_x10_2_1[i]);
                        Matrix_tray1x10_2_1[i].Add(Global.Place_Tray_2[2]);
                        Matrix_tray1x10_2_1[i].Add(Global.Place_Tray_2[3]);
                        Matrix_tray1x10_2_1[i].Add(Global.Place_Tray_2[4]);
                        Matrix_tray1x10_2_1[i].Add(Global.Place_Tray_2[5]);
                    }
                    for (int i = 0; i < Global.Number_FPCB; i++)
                    {
                        Matrix_tray1x10_2_2[i] = new List<double>();
                        Matrix_tray1x10_2_2[i].Add(Array_X_x10_2_2[i]);
                        Matrix_tray1x10_2_2[i].Add(Array_Y_x10_2_2[i]);
                        Matrix_tray1x10_2_2[i].Add(Global.Place_Tray_2[2]);
                        Matrix_tray1x10_2_2[i].Add(Global.Place_Tray_2[3]);
                        Matrix_tray1x10_2_2[i].Add(Global.Place_Tray_2[4]);
                        Matrix_tray1x10_2_2[i].Add(Global.Place_Tray_2[5]);
                    }
                    for (int i = 0; i < Global.Number_FPCB; i++)
                    {
                        Matrix_tray1x10_2_3[i] = new List<double>();
                        Matrix_tray1x10_2_3[i].Add(Array_X_x10_2_3[i]);
                        Matrix_tray1x10_2_3[i].Add(Array_Y_x10_2_3[i]);
                        Matrix_tray1x10_2_3[i].Add(Global.Place_Tray_2[2]);
                        Matrix_tray1x10_2_3[i].Add(Global.Place_Tray_2[3]);
                        Matrix_tray1x10_2_3[i].Add(Global.Place_Tray_2[4]);
                        Matrix_tray1x10_2_3[i].Add(Global.Place_Tray_2[5]);
                    }
                    for (int i = 0; i < Global.Number_FPCB; i++)
                    {
                        matrix.PAL_PR_RB_1_1(Convert.ToInt32(Global.Number_FPCB) + 1 + i, row_matrix_2, column_matrix_2, Matrix_tray1x10_2_1, Matrix_tray1x10_2_2, Matrix_tray1x10_2_3, i);
                    }
                    #endregion
                }

                //Scan Điều kiện Pos thả tray theo parameter Satefy 
                if (Scan_Position == 2 && Global.Home_All == false)
                {
                    if (Global.Place_Tray_1[0] > Global.X_Place_Satefy_L && Global.Place_Tray_1[0] < Global.X_Place_Satefy_U && Global.Place_Tray_1[1] < Global.Y_Place_Satefy_U && Global.Place_Tray_1[1] > Global.Y_Place_Satefy_L && Global.Place_Tray_1[2] > Global.Z_Place_Satefy
                       && Global.Place_Tray_2[0] > Global.X_Place_Satefy_L && Global.Place_Tray_2[0] < Global.X_Place_Satefy_U && Global.Place_Tray_2[1] < Global.Y_Place_Satefy_U && Global.Place_Tray_2[1] > Global.Y_Place_Satefy_L && Global.Place_Tray_2[2] > Global.Z_Place_Satefy
                      && Global.Place_Tray_3[0] > Global.X_Place_Satefy_L && Global.Place_Tray_3[0] < Global.X_Place_Satefy_U && Global.Place_Tray_3[1] < Global.Y_Place_Satefy_U && Global.Place_Tray_3[1] > Global.Y_Place_Satefy_L && Global.Place_Tray_3[2] > Global.Z_Place_Satefy)
                    {
                        //code
                    }
                    else
                    {
                        WaitFormHelper.Close();
                        MessageBox.Show("Kiêm tra lại tọa độ Maxtrix trong Teaching 1 Robot");
                    }
                }
                else
                {
                    WaitFormHelper.Close();
                    MessageBox.Show("Kiểm tra lại tọa độ Z của các Position Matrix trong Teaching 1 Robot");
                }
                WaitFormHelper.Close();
                Message_Box_OK("Write data position Place tray thành công", "Writing...");
            }
            catch (Exception e)
            {
                WaitFormHelper.Close();
                Message_Box_Error(e.ToString(), "Great_Value_Place_tray");
            }
        }
        private void Great_Value_Check_Marking()
        {
            try
            {
                double[] data_pos_X = new double[Global.Number_Tool];
                double[] data_pos_Y = new double[Global.Number_Tool];
                double[] data_pos_Z = new double[Global.Number_Tool];
                double[] data_pos_A3 = new double[Global.Number_Tool];
                double[] data_pos_A4 = new double[Global.Number_Tool];
                double[] data_pos_C = new double[Global.Number_Tool];
                int number_block = Convert.ToInt32(Global.Number_Block);
                int number_Fpcb_block = Convert.ToInt32(Global.Number_FPCB);
                int Total_Check_Cam_Marking = Convert.ToInt32(Global.Total_Check_Marking);
                double offset_X_Fpcb = Global.Offset_FPCB_X_Marking;
                double offset_Y_Fpcb = Global.Offset_FPCB_Y_Marking;
                double offset_X_Block = Global.Offset_Block_X_marking;
                double offset_Y_Block = Global.Offset_Block_Y_Marking;
                int k = 0;
                //if (number_block > Total_Check_Cam_Marking)
                //{
                //    for (int i = 0; i < number_block; i++)
                //    {
                //        for (int j = 0; j < number_Fpcb_block; j++)
                //        {
                //            if (i == 0)
                //            {
                //                data_pos_X[k] = Global.Check_Marking_Start[0] + offset_X_Fpcb * j;
                //            }
                //            else if (i > 0 && k != number_Fpcb_block * i)
                //            {
                //                data_pos_X[k] = data_pos_X[k - 1 * j] + offset_X_Fpcb * j;
                //            }
                //            else if (k == i * number_Fpcb_block)
                //            {
                //                data_pos_X[k] = data_pos_X[k - 1] + offset_X_Fpcb * j + offset_X_Block;
                //            }
                //            data_pos_Y[k] = Global.Check_Marking_Start[1];
                //            data_pos_Z[k] = Global.Check_Marking_Start[2];
                //            data_pos_A3[k] = Global.Check_Marking_Start[3];
                //            data_pos_A4[k] = Global.Check_Marking_Start[4];
                //            data_pos_C[k] = Global.Check_Marking_Start[5];
                //            k++;
                //        }
                //    }
                //    matrix.Write_SQL2(data_pos_X, data_pos_Y, data_pos_Z, data_pos_A3, data_pos_A4, data_pos_C, Global.Number_Tool);
                //}
                //else if (number_block == Total_Check_Cam_Marking)
                //{
                //    for (int i = 0; i < number_block; i++)
                //    {
                //        data_pos_X[i] = Global.Check_Marking_Start[0] + offset_X_Block * i;
                //        data_pos_Y[i] = Global.Check_Marking_Start[1];
                //        data_pos_Z[i] = Global.Check_Marking_Start[2];
                //        data_pos_A3[i] = Global.Check_Marking_Start[3];
                //        data_pos_A4[i] = Global.Check_Marking_Start[4];
                //        data_pos_C[i] = Global.Check_Marking_Start[5];
                //        matrix.Write_SQL2(data_pos_X, data_pos_Y, data_pos_Z, data_pos_A3, data_pos_A4, data_pos_C, Total_Check_Cam_Marking);
                //    }
                //}
                ConnectSQLite();
                using (var cmd = new SQLiteCommand("DELETE FROM Matrix_Panel_Cam_Top;", Conn))
                {
                    cmd.ExecuteNonQuery();
                }
                DisConSQLite();
                if (number_block != 0)
                {
                    for (int i = 0; i < Total_Check_Cam_Marking; i++)
                    {
                        data_pos_X[i] = Global.Check_Marking_Start[0] + offset_X_Fpcb * i;
                        data_pos_Y[i] = Global.Check_Marking_Start[1] + offset_Y_Fpcb * i;
                        data_pos_Z[i] = Global.Check_Marking_Start[2];
                        data_pos_A3[i] = Global.Check_Marking_Start[3];
                        data_pos_A4[i] = Global.Check_Marking_Start[4];
                        data_pos_C[i] = Global.Check_Marking_Start[5];
                        matrix.Write_SQL2(data_pos_X, data_pos_Y, data_pos_Z, data_pos_A3, data_pos_A4, data_pos_C, Total_Check_Cam_Marking);
                    }
                }
                else
                {
                    WaitFormHelper.Close();
                    Message_Box_Error("Number Block null", "Marking Position");
                }
                WaitFormHelper.Close();
                Message_Box_OK("Write data position Check Marking thành công", "Writing...");
            }
            catch (Exception e)
            {
                WaitFormHelper.Close();
                Message_Box_Error(e.ToString(), "Great_Value_Check_Marking");
            }
        }
        private void Show_Array_Pos_Tray()
        {
            string query = string.Format("SELECT * from Matrix_Panel_Tool_1_1");
            SQLiteDataAdapter adapter = new SQLiteDataAdapter(query, Conn);
            DataSet dataSet = new DataSet();
            adapter.Fill(dataSet, "Matrix_Panel_Tool_1_1");
            DataGridView_Matrix1.DataSource = dataSet.Tables["Matrix_Panel_Tool_1_1"];
            DataGridView_Matrix1.Columns["ID"].Width = 50;
            DataGridView_Matrix1.Columns["X"].Width = 80;
            DataGridView_Matrix1.Columns["Y"].Width = 80;
            DataGridView_Matrix1.Columns["Z"].Width = 80;
            DataGridView_Matrix1.Columns["A4"].Width = 50;
            DataGridView_Matrix1.Columns["A5"].Width = 50;
            DataGridView_Matrix1.Columns["C"].Width = 80;
            DataGridView_Matrix1.Columns["X"].ReadOnly = true;
            DataGridView_Matrix1.Columns["Y"].ReadOnly = true;
            DataGridView_Matrix1.Columns["Z"].ReadOnly = true;
            DataGridView_Matrix1.Columns["A4"].ReadOnly = true;
            DataGridView_Matrix1.Columns["A5"].ReadOnly = true;
            DataGridView_Matrix1.Columns["C"].ReadOnly = true;
            //
            String query1 = string.Format("SELECT * from Matrix_Panel_Tool_1_2");
            SQLiteDataAdapter adapter1 = new SQLiteDataAdapter(query1, Conn);
            DataSet dataSet1 = new DataSet();
            adapter1.Fill(dataSet1, "Matrix_Panel_Tool_1_2");
            DataGridView_Matrix2.DataSource = dataSet1.Tables["Matrix_Panel_Tool_1_2"];
            DataGridView_Matrix2.Columns["ID"].Width = 50;
            DataGridView_Matrix2.Columns["X"].Width = 65;
            DataGridView_Matrix2.Columns["Y"].Width = 65;
            DataGridView_Matrix2.Columns["Z"].Width = 65;
            DataGridView_Matrix2.Columns["A4"].Width = 30;
            DataGridView_Matrix2.Columns["A5"].Width = 30;
            DataGridView_Matrix2.Columns["C"].Width = 60;
            DataGridView_Matrix2.Columns["X"].ReadOnly = true;
            DataGridView_Matrix2.Columns["Y"].ReadOnly = true;
            DataGridView_Matrix2.Columns["Z"].ReadOnly = true;
            DataGridView_Matrix2.Columns["A4"].ReadOnly = true;
            DataGridView_Matrix2.Columns["A5"].ReadOnly = true;
            DataGridView_Matrix2.Columns["C"].ReadOnly = true;
            //
            String query2 = string.Format("SELECT * from Matrix_Panel_Tool_1_3");
            SQLiteDataAdapter adapter2 = new SQLiteDataAdapter(query2, Conn);
            DataSet dataSet2 = new DataSet();
            adapter2.Fill(dataSet2, "Matrix_Panel_Tool_1_3");
            DataGridView_Matrix3.DataSource = dataSet2.Tables["Matrix_Panel_Tool_1_3"];
            DataGridView_Matrix3.Columns["ID"].Width = 50;
            DataGridView_Matrix3.Columns["X"].Width = 65;
            DataGridView_Matrix3.Columns["Y"].Width = 65;
            DataGridView_Matrix3.Columns["Z"].Width = 65;
            DataGridView_Matrix3.Columns["A4"].Width = 30;
            DataGridView_Matrix3.Columns["A5"].Width = 30;
            DataGridView_Matrix3.Columns["C"].Width = 60;
            DataGridView_Matrix3.Columns["X"].ReadOnly = true;
            DataGridView_Matrix3.Columns["Y"].ReadOnly = true;
            DataGridView_Matrix3.Columns["Z"].ReadOnly = true;
            DataGridView_Matrix3.Columns["A4"].ReadOnly = true;
            DataGridView_Matrix3.Columns["A5"].ReadOnly = true;
            DataGridView_Matrix3.Columns["C"].ReadOnly = true;
            //
            //String query3 = string.Format("SELECT * from Matrix_Panel_Tool_1_4");
            //SQLiteDataAdapter adapter3 = new SQLiteDataAdapter(query3, Conn);
            //DataSet dataSet3 = new DataSet();
            //adapter3.Fill(dataSet3, "Matrix_Panel_Tool_1_4");
            //dataGridView_Arr_4.DataSource = dataSet3.Tables["Matrix_Panel_Tool_1_4"];
            //dataGridView_Arr_4.Columns["ID"].Width = 20;
            //dataGridView_Arr_4.Columns["X"].Width = 53;
            //dataGridView_Arr_4.Columns["Y"].Width = 53;
            //dataGridView_Arr_4.Columns["Z"].Width = 53;
            //dataGridView_Arr_4.Columns["A4"].Width = 15;
            //dataGridView_Arr_4.Columns["A5"].Width = 15;
            //dataGridView_Arr_4.Columns["C"].Width = 53;
            //dataGridView_Arr_4.Columns["X"].ReadOnly = true;
            //dataGridView_Arr_4.Columns["Y"].ReadOnly = true;
            //dataGridView_Arr_4.Columns["Z"].ReadOnly = true;
            //dataGridView_Arr_4.Columns["A4"].ReadOnly = true;
            //dataGridView_Arr_4.Columns["A5"].ReadOnly = true;
            //dataGridView_Arr_4.Columns["C"].ReadOnly = true;
        }
        private object lock_DO = new object();
        private void Change_DO(int robot, int id)
        {
            click_DO = 1;
            lock (lock_DO)
            {
                if (stop_change == 1)
                {
                    if (SDKHrobot.HRobot.get_digital_output(robot, id) == 0)
                    {
                        SDKHrobot.HRobot.set_digital_output(robot, id, true);
                    }
                    else
                    {
                        SDKHrobot.HRobot.set_digital_output(robot, id, false);
                    }
                }
                else { MessageBox.Show("Press Stop"); }
            }
            click_DO = 0;
        }
        private void DO8_Click(object sender, EventArgs e)
        {
            Change_DO(handle, 8);
        }
        private void btn_stop_rb_Click(object sender, EventArgs e)
        {
            SDKHrobot.HRobot.task_abort(handle);
            Global.GoX = false;
            Global.GoY = false;
            Global.GoZ = false;
            Global.GoC = false;
        }
        private void btn_Show_Pos_Marking_Click(object sender, EventArgs e)
        {
            show_data_cam_top.Config_LoadDataRB_DataGridView1();
            this.Controls.Add(show_data_cam_top);
            show_data_cam_top.Location = new Point(0, 290);
            show_data_cam_top.Size = new Size(570, 720);
            show_data_cam_top.BringToFront();
            show_data_cam_top.Show();
        }
        private void btnMov_Lin_Click(object sender, EventArgs e)
        {
            SDKHrobot.HRobot.set_operation_mode(handle, 0);
            double[] Pos_home_Joint = new double[6];
            if (stop_change == 0 && SDKHrobot.HRobot.get_digital_input(handle, 50) == 0)
            {
                //int res1 = SDKHrobot.HRobot.get_current_joint(handle, Pos_home_Joint);
                //if (res1 == 0)
                //{
                //    Pos_home_Joint = new double[6] { Pos_home_Joint[0], Pos_home_Joint[1], Z_antoan[3], Pos_home_Joint[3], Pos_home_Joint[4], Pos_home_Joint[5] };
                //    SDKHrobot.HRobot.set_override_ratio(handle, 10);
                //    SDKHrobot.HRobot.ptp_axis(handle, 0, Pos_home_Joint);
                //}
                //FuncRobot.Wait_for_stop_motion_2(handle);
                SDKHrobot.HRobot.set_override_ratio(handle, Convert.ToInt16(txt_speed_Jog.Text));
                string[] save = new string[8];
                double[] movpos = new double[6];
                Global.GoX = false;
                Global.GoY = false;
                Global.GoZ = false;
                Global.GoC = false;
                if (uiDataGridView1.SelectedRows.Count > 0)
                {

                    DataGridViewRow row = uiDataGridView1.SelectedRows[0];
                    for (int i = 0; i < 1; i++)
                    {
                        movpos[0] = Convert.ToDouble(row.Cells[i + 2].Value);
                        movpos[1] = Convert.ToDouble(row.Cells[i + 3].Value);
                        movpos[2] = Convert.ToDouble(row.Cells[i + 4].Value);
                        movpos[3] = 0.000;
                        movpos[4] = 0.000;
                        movpos[5] = Convert.ToDouble(row.Cells[i + 7].Value);
                    }

                    SDKHrobot.HRobot.lin_pos(handle, 0, 0, movpos);

                }
            }
            else
            {
                Message_Box_Error("Machine Stop!", "Error");
            }
        }
        private void btnOverWrite_Click(object sender, EventArgs e)
        {
            savePos_RB();
            load_pos_RB();
        }
        private void btn_Get_value_optoin_Robot_Click(object sender, EventArgs e)
        {
            Get_Para_Robot.Show(btn_Get_value_optoin_Robot, 0, 0);
        }
        private void GetValueOffsetTool_Click(object sender, EventArgs e)
        {
            Great_value_setup_robot();
        }
        private void GetValueRunAuto_Click(object sender, EventArgs e)
        {
            string[] data_RB = { txt_sp_auto_RB.Text, txt_delayhut.Text, txt_delaytha.Text, txt_delay_up_cylinder.Text, txt_delay_down_cylinder.Text,
                txt_number_tool.Text, txt_mode_run.Text, txt_ACC_RB.Text, txt_SP_Wait_Pick.Text,txt_trigger_camB.Text,txt_speed_snap.Text };
            int i = 1;
            save_Parameter_RB(i, data_RB);
            Global.SP_auto_RB = Convert.ToInt16(txt_sp_auto_RB.Text);
            Global.delay_hut = Convert.ToInt16(txt_delayhut.Text);
            Global.delay_tha = Convert.ToInt16(txt_delaytha.Text);
            Global.delay_up = Convert.ToInt16(txt_delay_up_cylinder.Text);
            Global.delay_down = Convert.ToInt16(txt_delay_down_cylinder.Text);
            Global.Number_Tool = Convert.ToInt16(txt_number_tool.Text);// D9100
            Global.Mode_run_auto = Convert.ToInt16(txt_mode_run.Text);
            Global.ACC_RB = Convert.ToInt16(txt_ACC_RB.Text);
            Global.SP_Wait_Pick = Convert.ToInt16(txt_SP_Wait_Pick.Text);
            Global.TT_CamB = Convert.ToInt16(txt_trigger_camB.Text);
            Global.SP_snap = Convert.ToInt16(txt_speed_snap.Text);
            WaitFormHelper.Close();
        }
        private void GetValueSatefy_Click(object sender, EventArgs e)
        {
            Great_value_setup_robot();
        }
        private void GetValueDataSheetFPCB_Click(object sender, EventArgs e)
        {
            Great_value_setup_robot();
            // Great_Value_Place_tray();                
        }
        private async void GetValueCheckMarking_Click(object sender, EventArgs e)
        {
            Great_value_setup_robot();
            await WaitFormHelper.Show();
            await Task.Run(() =>
            {
                Great_Value_Check_Marking();
            });

        }
        private void btn_Cylinder_Camera_Top_Click(object sender, EventArgs e)
        {
            if (SDKHrobot.HRobot.get_digital_output(handle, 50) == 0)// cylinder cam top
            {
                SDKHrobot.HRobot.set_digital_output(handle, 50, true);
                btn_Cylinder_Camera_Top.Text = "Cam Down";
                btn_Cylinder_Camera_Top.BackColor = Control.DefaultBackColor;
            }
            else
            {
                SDKHrobot.HRobot.set_digital_output(handle, 50, false);
                btn_Cylinder_Camera_Top.Text = "Cam Up";
                btn_Cylinder_Camera_Top.BackColor = Color.GreenYellow;
            }
        }
        private void btn_Vaccum_All_Click(object sender, EventArgs e)
        {
            if (SDKHrobot.HRobot.get_digital_output(handle, 51) == 0)
            {
                SDKHrobot.HRobot.set_digital_output(handle, 52, false);
                SDKHrobot.HRobot.set_digital_output(handle, 51, true);
                PLC1.Write_DataBit_("M" + (9080 + Memory_PLC.K1000).ToString(), 1);
            }
            else
            {
                SDKHrobot.HRobot.set_digital_output(handle, 52, false);
                SDKHrobot.HRobot.set_digital_output(handle, 51, false);
                PLC1.Write_DataBit_("M" + (9081 + Memory_PLC.K1000).ToString(), 1);
                click_Vaccum = 0;
            }
        }
        private void btn_Blow_All_Click(object sender, EventArgs e)
        {
            click_blow++;
            if (click_blow == 1)
            {
                SDKHrobot.HRobot.set_digital_output(handle, 51, false);
                SDKHrobot.HRobot.set_digital_output(handle, 52, true);
                PLC1.Write_DataBit_("M" + (9081 + Memory_PLC.K1000).ToString(), 1);
            }
            else
            {
                SDKHrobot.HRobot.set_digital_output(handle, 51, false);
                SDKHrobot.HRobot.set_digital_output(handle, 52, false);
                PLC1.Write_DataBit_("M" + (9081 + Memory_PLC.K1000).ToString(), 1);
                click_blow = 0;
            }
        }
        private void btn_Vaccum_Select_Click(object sender, EventArgs e)
        {
            select_air1++;
            SDKHrobot.HRobot.set_digital_output(handle, 51, true);
            if (select_air1 == 1)
            {
                try
                {
                    if (txt_Number_Tool_Vaccum.Text != null)
                    {
                        PLC1.Write_Data_Word_("D" + (7030 + Memory_PLC.K100 + Convert.ToInt16(txt_Number_Tool_Vaccum.Text)), 1);
                    }
                    else
                    {
                        Message_Box_Error("Number Tool select Null", "Select tool vaccum");
                    }
                }
                catch { Message_Box_Error("Number Select vaccum null", "Exception"); }
            }

            else if (select_air1 == 2)
            {
                SDKHrobot.HRobot.set_digital_output(handle, 51, false);
                PLC1.Write_Data_Word_("D" + (7030 + Memory_PLC.K100 + Convert.ToInt16(txt_Number_Tool_Vaccum.Text)), 0);
                select_air1 = 0;
            }
        }
        private void btn_Blow_Select_Click(object sender, EventArgs e)
        {
            if (txt_Number_Tool_Vaccum.Text != null)
            {
                SDKHrobot.HRobot.set_digital_output(handle, 52, true);
                PLC1.Write_Data_Word_("D" + (7030 + Memory_PLC.K100 + Convert.ToInt16(txt_Number_Tool_Vaccum.Text)), 1);
                Thread.Sleep(500);
                PLC1.Write_Data_Word_("D" + (7030 + Memory_PLC.K100 + Convert.ToInt16(txt_Number_Tool_Vaccum.Text)), 0);
            }
            else
            {
                Message_Box_Error("Number Tool select Null", "Select tool vaccum");
            }
        }
        private async void btn_calculator_Click(object sender, EventArgs e)
        {
            await WaitFormHelper.Show();
            await Task.Run(() =>
            {
                if (Convert.ToDouble(Offset_Y_BlockFPCB_Tool_tray.Text) == 0)
                {
                    Great_Value_Place_tray();
                }
                else
                {
                    Great_Value_Place_tray1();
                }
            });
        }
        private void btn_Show_Data_Matrix_Click(object sender, EventArgs e)
        {
            Show_Array_Pos_Tray();
        }
        private void btn_boxNG_Click(object sender, EventArgs e)
        {
            UIButton btn = sender as UIButton;
            if (btn == null) return;
            int address = Convert.ToInt16(btn.Tag);
            bool get_mode = Convert.ToBoolean(btn.TagString);
            SDKHrobot.HRobot.set_digital_output(handle, address, get_mode);
        }
        private void btn_EMG_Click(object sender, EventArgs e)
        {
            UIButton btn = sender as UIButton;
            if (btn == null) return;
            int address = Convert.ToInt16(btn.Tag);
            bool get_mode = Convert.ToBoolean(btn.TagString);
            PLC1.Write_DataBit_("M" + (address + Memory_PLC.K100).ToString(), 1);
        }
        private void btn_Textbox_MouseDown(object sender, MouseEventArgs e)
        {
            UITextBox txt = sender as UITextBox;
            if (txt == null) return;
            string axis = txt.Tag.ToString();
            if (axis == "X")
            {
                Global.GoX = true;
            }
            else if (axis == "Y")
            {
                Global.GoY = true;
            }
            else if (axis == "Z")
            {
                Global.GoZ = true;
            }
            else if (axis == "C")
            {
                Global.GoC = true;
            }
        }
        private void btn_Go_Click(object sender, EventArgs e)
        {
            UISymbolButton btnSy = sender as UISymbolButton;
            if (btnSy == null) return;
            string txtName = btnSy.Tag.ToString();
            string Name_Axis = btnSy.TagString;
            var matches = this.Controls.Find(txtName, true);
            if (matches.Length == 0 || !(matches[0] is UITextBox textbox)) return;
            double[] go = new double[6];
            SDKHrobot.HRobot.get_current_position(handle, go);
            double move_axis = Convert.ToDouble(textbox.Text);
            try
            {
                if (Name_Axis == "X")
                {
                    double[] Go_Mov = new double[6] { move_axis, go[1], go[2], go[3], go[4], go[5] };
                    SDKHrobot.HRobot.lin_pos(handle, 0, 0, Go_Mov);
                    Global.GoX = false;
                }
                else if (Name_Axis == "Y")
                {
                    double[] Go_Mov = new double[6] { go[0], move_axis, go[2], go[3], go[4], go[5] };
                    SDKHrobot.HRobot.lin_pos(handle, 0, 0, Go_Mov);
                    Global.GoY = false;
                }
                else if (Name_Axis == "Z")
                {
                    double[] Go_Mov = new double[6] { go[0], go[1], move_axis, go[3], go[4], go[5] };
                    SDKHrobot.HRobot.lin_pos(handle, 0, 0, Go_Mov);
                    Global.GoZ = false;
                }
                else if (Name_Axis == "C")
                {
                    double[] Go_Mov = new double[6] { go[0], go[1], go[2], go[3], go[4], move_axis };
                    SDKHrobot.HRobot.lin_pos(handle, 0, 0, Go_Mov);
                    Global.GoC = false;
                }

            }
            catch { Message_Box_Error("Move Erro", "Mov"); }
        }
        #endregion
        #region Button Jog Robot-----------------------------------------------------------
        enum JogType
        {
            Cart = 0,
            Joint = 1,
        };
        public int CurSelJogType;
        private void btn_jog_X_tru_MouseDown(object sender, MouseEventArgs e)
        {
            Global.GoX = false;
            SDKHrobot.HRobot.jog(handle, CurSelJogType, 0, -1);
            SDKHrobot.HRobot.set_digital_output(handle, 1, false);
        }
        private void btn_jog_X_tru_MouseUp(object sender, MouseEventArgs e)
        {
            SDKHrobot.HRobot.jog_stop(handle);
        }
        private void btn_jog_Y_tru_MouseDown(object sender, MouseEventArgs e)
        {
            Global.GoY = false;
            SDKHrobot.HRobot.jog(handle, CurSelJogType, 1, -1);
            SDKHrobot.HRobot.set_digital_output(handle, 1, false);
        }

        private void btn_jog_Y_tru_MouseUp(object sender, MouseEventArgs e)
        {
            SDKHrobot.HRobot.jog_stop(handle);
        }

        private void btn_Jog_Z_tru_MouseDown(object sender, MouseEventArgs e)
        {
            Global.GoZ = false;
            SDKHrobot.HRobot.jog(handle, CurSelJogType, 2, -1);
            SDKHrobot.HRobot.set_digital_output(handle, 1, false);
        }

        private void btn_Jog_Z_tru_MouseUp(object sender, MouseEventArgs e)
        {
            SDKHrobot.HRobot.jog_stop(handle);
        }

        private void btn_Jog_A4_tru_MouseDown(object sender, MouseEventArgs e)
        {
            Global.GoC = false;
            SDKHrobot.HRobot.jog(handle, CurSelJogType, 3, -1);
            SDKHrobot.HRobot.set_digital_output(handle, 1, false);
        }

        private void btn_Jog_A4_tru_MouseUp(object sender, MouseEventArgs e)
        {
            SDKHrobot.HRobot.jog_stop(handle);
        }

        private void btn_Jog_X_Cong_MouseDown(object sender, MouseEventArgs e)
        {
            Global.GoX = false;
            SDKHrobot.HRobot.jog(handle, CurSelJogType, 0, 1);
            SDKHrobot.HRobot.set_digital_output(handle, 1, false);
        }

        private void btn_Jog_X_Cong_MouseUp(object sender, MouseEventArgs e)
        {
            SDKHrobot.HRobot.jog_stop(handle);
        }

        private void btn_Jog_Y_Cong_MouseDown(object sender, MouseEventArgs e)
        {
            Global.GoY = false;
            SDKHrobot.HRobot.jog(handle, CurSelJogType, 1, 1);
            SDKHrobot.HRobot.set_digital_output(handle, 1, false);
        }

        private void btn_Jog_Y_Cong_MouseUp(object sender, MouseEventArgs e)
        {
            SDKHrobot.HRobot.jog_stop(handle);
        }

        private void btn_Jog_Z_Cong_MouseDown(object sender, MouseEventArgs e)
        {
            Global.GoZ = false;
            SDKHrobot.HRobot.jog(handle, CurSelJogType, 2, 1);
            SDKHrobot.HRobot.set_digital_output(handle, 1, false);
        }

        private void btn_Jog_Z_Cong_MouseUp(object sender, MouseEventArgs e)
        {
            SDKHrobot.HRobot.jog_stop(handle);
        }

        private void btn_Jog_A4_Cong_MouseDown(object sender, MouseEventArgs e)
        {
            Global.GoC = false;
            SDKHrobot.HRobot.jog(handle, CurSelJogType, 3, 1);
            SDKHrobot.HRobot.set_digital_output(handle, 1, false);
        }

        private void btn_Jog_A4_Cong_MouseUp(object sender, MouseEventArgs e)
        {
            SDKHrobot.HRobot.jog_stop(handle);
        }
        private void btn_Jog_A1_Tru_MouseDown(object sender, MouseEventArgs e)
        {
            SDKHrobot.HRobot.jog(handle, CurSelJogType, 0, -1);
            SDKHrobot.HRobot.set_digital_output(handle, 1, false);
        }

        private void btn_Jog_A1_Tru_MouseUp(object sender, MouseEventArgs e)
        {
            SDKHrobot.HRobot.jog_stop(handle);
        }

        private void btn_Jog_A2_Tru_MouseDown(object sender, MouseEventArgs e)
        {
            SDKHrobot.HRobot.jog(handle, CurSelJogType, 1, -1);
            SDKHrobot.HRobot.set_digital_output(handle, 1, false);
        }

        private void btn_Jog_A2_Tru_MouseUp(object sender, MouseEventArgs e)
        {
            SDKHrobot.HRobot.jog_stop(handle);
        }

        private void btn_Jog_A3_Tru_MouseDown(object sender, MouseEventArgs e)
        {
            SDKHrobot.HRobot.jog(handle, CurSelJogType, 2, -1);
            SDKHrobot.HRobot.set_digital_output(handle, 1, false);
        }

        private void btn_Jog_A3_Tru_MouseUp(object sender, MouseEventArgs e)
        {
            SDKHrobot.HRobot.jog_stop(handle);
        }

        private void btn_Jog_Joint_A4_Tru_MouseDown(object sender, MouseEventArgs e)
        {
            SDKHrobot.HRobot.jog(handle, CurSelJogType, 3, -1);
            SDKHrobot.HRobot.set_digital_output(handle, 1, false);
        }

        private void btn_Jog_Joint_A4_Tru_MouseUp(object sender, MouseEventArgs e)
        {
            SDKHrobot.HRobot.jog_stop(handle);
        }

        private void btn_Jog_A1_Cong_MouseDown(object sender, MouseEventArgs e)
        {
            SDKHrobot.HRobot.jog(handle, CurSelJogType, 0, 1);
            SDKHrobot.HRobot.set_digital_output(handle, 1, false);
        }
        private void btn_Jog_A1_Cong_MouseUp(object sender, MouseEventArgs e)
        {
            SDKHrobot.HRobot.jog_stop(handle);
        }

        private void btn_Jog_A2_Cong_MouseDown(object sender, MouseEventArgs e)
        {
            SDKHrobot.HRobot.jog(handle, CurSelJogType, 1, 1);
            SDKHrobot.HRobot.set_digital_output(handle, 1, false);
        }

        private void btn_Jog_A2_Cong_MouseUp(object sender, MouseEventArgs e)
        {
            SDKHrobot.HRobot.jog_stop(handle);
        }

        private void btn_Jog_A3_Cong_MouseDown(object sender, MouseEventArgs e)
        {
            SDKHrobot.HRobot.jog(handle, CurSelJogType, 2, 1);
            SDKHrobot.HRobot.set_digital_output(handle, 1, false);
        }

        private void btn_Jog_A3_Cong_MouseUp(object sender, MouseEventArgs e)
        {
            SDKHrobot.HRobot.jog_stop(handle);
        }

        private void btn_Jog_Joint_A4_Cong_MouseDown(object sender, MouseEventArgs e)
        {
            SDKHrobot.HRobot.jog(handle, CurSelJogType, 3, 1);
            SDKHrobot.HRobot.set_digital_output(handle, 1, false);
        }

        private void btn_Jog_Joint_A4_Cong_MouseUp(object sender, MouseEventArgs e)
        {
            SDKHrobot.HRobot.jog_stop(handle);
        }
        private void Value_ChangeSpeed(object sender, EventArgs e)
        {
            SDKHrobot.HRobot.set_override_ratio(handle, TrackBar_Jog_Speed_Robot.Value);
            txt_speed_Jog.Text = TrackBar_Jog_Speed_Robot.Value.ToString();
        }
        #endregion
        #region PLC Common-----------------------------------------------------------------
        public bool Connect_PLC()
        {
            try
            {
                PLC1.connect1();
                TaskbarManager.Instance.SetProgressValue(100, 100);
                TaskbarManager.Instance.SetProgressState(TaskbarProgressBarState.Normal);
            }
            catch
            {
                Message_Box_Error("Disconnect PLC", "Connect");
                TaskbarManager.Instance.SetProgressValue(100, 100);
                TaskbarManager.Instance.SetProgressState(TaskbarProgressBarState.Error);
            }
            return Global.IsConnectPLC;
        }
        public string Info_PLC()
        {
            string info_ = PLC1.infor_PLC();
            return info_;
        }
        private void Update_Axis_Menu_Click(object sender, EventArgs e)
        {
            Menu_Save_Speed_Axis.Show(Update_Axis_Menu, 0, 0);
        }
        private void Update_Speed_Axis_Menu(int index_, string content_update)
        {
            if (PLC1.IsConnected == true)
            {
                UITextBox txt_name1 = this.Controls.Find("Creep" + index_.ToString(), true).FirstOrDefault() as UITextBox;
                UITextBox txt_name2 = this.Controls.Find("SPHome" + index_.ToString(), true).FirstOrDefault() as UITextBox;
                UITextBox txt_name3 = this.Controls.Find("SPAuto1_" + index_.ToString(), true).FirstOrDefault() as UITextBox;
                UITextBox txt_name4 = this.Controls.Find("SPAuto2_" + index_.ToString(), true).FirstOrDefault() as UITextBox;
                UITextBox txt_name5 = this.Controls.Find("SPAuto3_" + index_.ToString(), true).FirstOrDefault() as UITextBox;
                UITextBox txt_name6 = this.Controls.Find("ACC1_" + index_.ToString(), true).FirstOrDefault() as UITextBox;
                UITextBox txt_name7 = this.Controls.Find("DEC1_" + index_.ToString(), true).FirstOrDefault() as UITextBox;
                UITextBox txt_name8 = this.Controls.Find("ACC2_" + index_.ToString(), true).FirstOrDefault() as UITextBox;
                UITextBox txt_name9 = this.Controls.Find("DEC2_" + index_.ToString(), true).FirstOrDefault() as UITextBox;
                UITextBox txt_name10 = this.Controls.Find("ACC3_" + index_.ToString(), true).FirstOrDefault() as UITextBox;
                UITextBox txt_name11 = this.Controls.Find("DEC3_" + index_.ToString(), true).FirstOrDefault() as UITextBox;
                if (index_ < 9)
                {
                    try
                    {
                        PLC1.Write_Data_DWord_("D" + (5112 + 100 * (index_ - 1) + Memory_PLC.K800).ToString(), (Convert.ToInt32(txt_name1.Text)));
                        PLC1.Write_Data_DWord_("D" + (5110 + 100 * (index_ - 1) + Memory_PLC.K800).ToString(), (Convert.ToInt32(txt_name2.Text)));
                        PLC1.Write_Data_DWord_("D" + (5114 + 100 * (index_ - 1) + Memory_PLC.K800).ToString(), (Convert.ToInt32(txt_name3.Text)));
                        PLC1.Write_Data_DWord_("D" + (5116 + 100 * (index_ - 1) + Memory_PLC.K800).ToString(), (Convert.ToInt32(txt_name4.Text)));
                        PLC1.Write_Data_DWord_("D" + (5118 + 100 * (index_ - 1) + Memory_PLC.K800).ToString(), (Convert.ToInt32(txt_name5.Text)));
                        PLC1.Write_Data_DWord_("D" + (5174 + 100 * (index_ - 1) + Memory_PLC.K800).ToString(), (Convert.ToInt32(txt_name6.Text)));
                        PLC1.Write_Data_DWord_("D" + (5176 + 100 * (index_ - 1) + Memory_PLC.K800).ToString(), (Convert.ToInt32(txt_name7.Text)));
                        PLC1.Write_Data_DWord_("D" + (5178 + 100 * (index_ - 1) + Memory_PLC.K800).ToString(), (Convert.ToInt32(txt_name8.Text)));
                        PLC1.Write_Data_DWord_("D" + (5180 + 100 * (index_ - 1) + Memory_PLC.K800).ToString(), (Convert.ToInt32(txt_name9.Text)));
                        PLC1.Write_Data_DWord_("D" + (5182 + 100 * (index_ - 1) + Memory_PLC.K800).ToString(), (Convert.ToInt32(txt_name10.Text)));
                        PLC1.Write_Data_DWord_("D" + (5184 + 100 * (index_ - 1) + Memory_PLC.K800).ToString(), (Convert.ToInt32(txt_name11.Text)));
                        Message_Box_OK(content_update + " thành công", "Update Speed Axis");
                    }
                    catch { Message_Box_Error(content_update + " không thành công", "Update Speed Axis"); }
                }
                else
                {
                    try
                    {
                        PLC1.Write_Data_DWord_("D" + (6712 + Memory_PLC.K100).ToString(), (Convert.ToInt32(txt_name1.Text)));
                        PLC1.Write_Data_DWord_("D" + (6710 + Memory_PLC.K100).ToString(), (Convert.ToInt32(txt_name2.Text)));
                        PLC1.Write_Data_DWord_("D" + (6714 + Memory_PLC.K100).ToString(), (Convert.ToInt32(txt_name3.Text)));
                        PLC1.Write_Data_DWord_("D" + (6716 + Memory_PLC.K100).ToString(), (Convert.ToInt32(txt_name4.Text)));
                        PLC1.Write_Data_DWord_("D" + (6718 + Memory_PLC.K100).ToString(), (Convert.ToInt32(txt_name5.Text)));
                        PLC1.Write_Data_DWord_("D" + (6774 + Memory_PLC.K100).ToString(), (Convert.ToInt32(txt_name6.Text)));
                        PLC1.Write_Data_DWord_("D" + (6776 + Memory_PLC.K100).ToString(), (Convert.ToInt32(txt_name7.Text)));
                        PLC1.Write_Data_DWord_("D" + (6778 + Memory_PLC.K100).ToString(), (Convert.ToInt32(txt_name8.Text)));
                        PLC1.Write_Data_DWord_("D" + (6780 + Memory_PLC.K100).ToString(), (Convert.ToInt32(txt_name9.Text)));
                        PLC1.Write_Data_DWord_("D" + (6782 + Memory_PLC.K100).ToString(), (Convert.ToInt32(txt_name10.Text)));
                        PLC1.Write_Data_DWord_("D" + (6784 + Memory_PLC.K100).ToString(), (Convert.ToInt32(txt_name11.Text)));
                        Message_Box_OK(content_update + " thành công", "Update Speed Axis");
                    }
                    catch { Message_Box_Error(content_update + " không thành công", "Update Speed Axis"); }
                }
            }
        }
        private void Menu_Save_Speed_Axis_Click(object sender, ToolStripItemClickedEventArgs e)
        {
            ToolStripItem clickedItem = e.ClickedItem;
            int value = Menu_Save_Speed_Axis.Items.IndexOf(clickedItem);
            Update_Speed_Axis_Menu(value + 1, clickedItem.ToString()); ;
        }
        private void Get_Offset_Motion_Check_Vision_Bottom()
        {
            try
            {
                PLC1.Write_Data_DWord_("D" + (9050 + Memory_PLC.K2000).ToString(), Convert.ToInt32(txt_number_block_FPCB.Text));
                PLC1.Write_Data_DWord_("D" + (9052 + Memory_PLC.K2000).ToString(), Convert.ToInt32(txt_number_FPCB_Block.Text));
                PLC1.Write_Data_DWord_("D" + (9054 + Memory_PLC.K2000).ToString(), Convert.ToInt32(Convert.ToDouble(txt_Offset_X_FPCB.Text) * 1000));
                PLC1.Write_Data_DWord_("D" + (9056 + Memory_PLC.K2000).ToString(), Convert.ToInt32(Convert.ToDouble(txt_Offset_Y_FPCB.Text) * 1000));
                PLC1.Write_Data_DWord_("D" + (9058 + Memory_PLC.K2000).ToString(), Convert.ToInt32(Convert.ToDouble(txt_Offset_X_Block.Text) * 1000));
                PLC1.Write_Data_DWord_("D" + (9060 + Memory_PLC.K2000).ToString(), Convert.ToInt32(Convert.ToDouble(txt_Offset_Y_Block.Text) * 1000));
                PLC1.Write_Data_DWord_("D" + (9062 + Memory_PLC.K2000).ToString(), Convert.ToInt32(txt_number_data.Text));
                PLC1.Write_Data_DWord_("D" + (9100 + Memory_PLC.K2000).ToString(), Convert.ToInt32(Convert.ToDouble(txt_Total_Check.Text)));
                int Number_Tool_Check_VisionBot = Convert.ToInt16(txt_Total_Check.Text);
                int[] data_pos_X1 = new int[Number_Tool_Check_VisionBot];
                int[] data_pos_Y1 = new int[Number_Tool_Check_VisionBot];
                //                          
                int[] data_pos_X2 = new int[Number_Tool_Check_VisionBot];
                int[] data_pos_Y2 = new int[Number_Tool_Check_VisionBot];
                //                          
                int[] data_pos_X3 = new int[Number_Tool_Check_VisionBot];
                int[] data_pos_Y3 = new int[Number_Tool_Check_VisionBot];
                //                          
                int[] data_pos_X4 = new int[Number_Tool_Check_VisionBot];
                int[] data_pos_Y4 = new int[Number_Tool_Check_VisionBot];
                //
                int number_block = Convert.ToInt16(txt_number_block_FPCB.Text);
                int number_Fpcb_block = Convert.ToInt16(txt_number_FPCB_Block.Text);
                int offset_X_Fpcb = Convert.ToInt32(Convert.ToDouble(txt_Offset_X_FPCB.Text) * 1000);
                int offset_Y_Fpcb = Convert.ToInt32(Convert.ToDouble(txt_Offset_Y_FPCB.Text) * 1000);
                int offset_X_Block = Convert.ToInt32(Convert.ToDouble(txt_Offset_X_Block.Text) * 1000);
                int offset_Y_Block = Convert.ToInt32(Convert.ToDouble(txt_Offset_Y_Block.Text) * 1000);
                int k = 0;
                if (number_block > 0)
                {
                    //POS CHUP 1
                    for (int i = 0; i < number_block; i++)
                    {
                        for (int j = 0; j < number_Fpcb_block; j++)
                        {
                            if (i == 0)
                            {
                                data_pos_X1[k] = Convert.ToInt32(Convert.ToDouble(COR5341.Text) * 1000) + offset_X_Fpcb * j;
                                data_pos_Y1[k] = Convert.ToInt32(Convert.ToDouble(COR5441.Text) * 1000 + offset_Y_Fpcb * j);
                            }
                            else if (i > 0 && k != number_Fpcb_block * i)
                            {
                                data_pos_X1[k] = data_pos_X1[k - 1 * j] + offset_X_Fpcb * j;
                                //data_pos_Y[k] = data_pos_Y[k - 1 * j] + offset_Y_Fpcb * j;
                                data_pos_Y1[k] = Convert.ToInt32(Convert.ToDouble(data_pos_Y1[k - 1 * j] + offset_Y_Fpcb * j));
                            }
                            else if (k == i * number_Fpcb_block)
                            {
                                data_pos_X1[k] = data_pos_X1[k - 1] + offset_X_Fpcb * j + offset_X_Block;
                                //data_pos_Y[k] = data_pos_Y[k - 1] + offset_Y_Fpcb * j + offset_Y_Block;
                                data_pos_Y1[k] = Convert.ToInt32(Convert.ToDouble(data_pos_Y1[k - 1] + offset_Y_Fpcb * j + offset_Y_Block));
                            }
                            //data_pos_Y1[k] = Convert.ToInt32(Convert.ToDouble(COR5441.Text) * 1000);
                            k++;
                        }
                    }
                    //POS CHUP 2
                    k = 0;
                    for (int i = 0; i < number_block; i++)
                    {
                        for (int j = 0; j < number_Fpcb_block; j++)
                        {
                            if (i == 0)
                            {
                                data_pos_X2[k] = Convert.ToInt32(Convert.ToDouble(COR5343.Text) * 1000) + offset_X_Fpcb * j;
                                //data_pos_Y[k] = Convert.ToInt32(txt_D5441.Text) + offset_Y_Fpcb * j;
                                data_pos_Y2[k] = Convert.ToInt32(Convert.ToDouble(COR5443.Text) * 1000 + offset_Y_Fpcb * j);
                            }
                            else if (i > 0 && k != number_Fpcb_block * i)
                            {
                                data_pos_X2[k] = data_pos_X2[k - 1 * j] + offset_X_Fpcb * j;
                                //data_pos_Y[k] = data_pos_Y[k - 1 * j] + offset_Y_Fpcb * j;
                                data_pos_Y2[k] = Convert.ToInt32(Convert.ToDouble(data_pos_Y2[k - 1 * j] + offset_Y_Fpcb * j));
                            }
                            else if (k == i * number_Fpcb_block)
                            {
                                data_pos_X2[k] = data_pos_X2[k - 1] + offset_X_Fpcb * j + offset_X_Block;
                                //data_pos_Y[k] = data_pos_Y[k - 1] + offset_Y_Fpcb * j + offset_Y_Block;
                                data_pos_Y2[k] = Convert.ToInt32(Convert.ToDouble(data_pos_Y2[k - 1] + offset_Y_Fpcb * j + offset_Y_Block));
                            }
                            //data_pos_Y2[k] = Convert.ToInt32(Convert.ToDouble(COR5443.Text) * 1000);
                            k++;
                        }
                    }
                    //POS CHUP 3
                    k = 0;
                    for (int i = 0; i < number_block; i++)
                    {
                        for (int j = 0; j < number_Fpcb_block; j++)
                        {
                            if (i == 0)
                            {
                                data_pos_X3[k] = Convert.ToInt32(Convert.ToDouble(COR5345.Text) * 1000) + offset_X_Fpcb * j;
                                //data_pos_Y[k] = Convert.ToInt32(txt_D5441.Text) + offset_Y_Fpcb * j;
                                data_pos_Y3[k] = Convert.ToInt32(Convert.ToDouble(COR5445.Text) * 1000 + offset_Y_Fpcb * j);
                            }
                            else if (i > 0 && k != number_Fpcb_block * i)
                            {
                                data_pos_X3[k] = data_pos_X3[k - 1 * j] + offset_X_Fpcb * j;
                                //data_pos_Y[k] = data_pos_Y[k - 1 * j] + offset_Y_Fpcb * j;
                                data_pos_Y3[k] = Convert.ToInt32(Convert.ToDouble(data_pos_Y3[k - 1 * j] + offset_Y_Fpcb * j));
                            }
                            else if (k == i * number_Fpcb_block)
                            {
                                data_pos_X3[k] = data_pos_X3[k - 1] + offset_X_Fpcb * j + offset_X_Block;
                                //data_pos_Y[k] = data_pos_Y[k - 1] + offset_Y_Fpcb * j + offset_Y_Block;
                                data_pos_Y3[k] = Convert.ToInt32(Convert.ToDouble(data_pos_Y3[k - 1] + offset_Y_Fpcb * j + offset_Y_Block));
                            }
                            // data_pos_Y3[k] = Convert.ToInt32(Convert.ToDouble(COR5445.Text) * 1000);
                            k++;
                        }
                    }
                    //         
                    //POS CHUP 4
                    k = 0;
                    for (int i = 0; i < number_block; i++)
                    {
                        for (int j = 0; j < number_Fpcb_block; j++)
                        {
                            if (i == 0)
                            {
                                data_pos_X4[k] = Convert.ToInt32(Convert.ToDouble(COR5347.Text) * 1000) + offset_X_Fpcb * j;
                                //data_pos_Y[k] = Convert.ToInt32(txt_D5441.Text) + offset_Y_Fpcb * j;
                                data_pos_Y4[k] = Convert.ToInt32(Convert.ToDouble(COR5447.Text) * 1000 + offset_Y_Fpcb * j);
                            }
                            else if (i > 0 && k != number_Fpcb_block * i)
                            {
                                data_pos_X4[k] = data_pos_X4[k - 1 * j] + offset_X_Fpcb * j;
                                //data_pos_Y[k] = data_pos_Y[k - 1 * j] + offset_Y_Fpcb * j;
                                data_pos_Y4[k] = Convert.ToInt32(Convert.ToDouble(data_pos_Y4[k - 1 * j] + offset_Y_Fpcb * j));
                            }
                            else if (k == i * number_Fpcb_block)
                            {
                                data_pos_X4[k] = data_pos_X4[k - 1] + offset_X_Fpcb * j + offset_X_Block;
                                //data_pos_Y[k] = data_pos_Y[k - 1] + offset_Y_Fpcb * j + offset_Y_Block;
                                data_pos_Y4[k] = Convert.ToInt32(Convert.ToDouble(data_pos_Y4[k - 1] + offset_Y_Fpcb * j + offset_Y_Block));
                            }
                            //data_pos_Y4[k] = Convert.ToInt32(Convert.ToDouble(COR5447.Text) * 1000);
                            k++;
                        }
                    }
                    //                  
                    ConnectSQLite();
                    using (var cmd = new SQLiteCommand("DELETE FROM Matrix_Panel1_Cam_PLC;", Conn))
                    {
                        cmd.ExecuteNonQuery();
                    }
                    using (var cmd = new SQLiteCommand("DELETE FROM Matrix_Panel2_Cam_PLC;", Conn))
                    {
                        cmd.ExecuteNonQuery();
                    }
                    using (var cmd = new SQLiteCommand("DELETE FROM Matrix_Panel3_Cam_PLC;", Conn))
                    {
                        cmd.ExecuteNonQuery();
                    }
                    using (var cmd = new SQLiteCommand("DELETE FROM Matrix_Panel4_Cam_PLC;", Conn))
                    {
                        cmd.ExecuteNonQuery();
                    }
                    DisConSQLite();
                    matrix.Write_SQL(data_pos_X1, data_pos_Y1, Number_Tool_Check_VisionBot, "INSERT OR REPLACE INTO Matrix_Panel1_Cam_PLC (STT, X, Y) VALUES (@STT, @X, @Y)");
                    matrix.Write_SQL(data_pos_X2, data_pos_Y2, Number_Tool_Check_VisionBot, "INSERT OR REPLACE INTO Matrix_Panel2_Cam_PLC (STT, X, Y) VALUES (@STT, @X, @Y)");
                    matrix.Write_SQL(data_pos_X3, data_pos_Y3, Number_Tool_Check_VisionBot, "INSERT OR REPLACE INTO Matrix_Panel3_Cam_PLC (STT, X, Y) VALUES (@STT, @X, @Y)");
                    matrix.Write_SQL(data_pos_X4, data_pos_Y4, Number_Tool_Check_VisionBot, "INSERT OR REPLACE INTO Matrix_Panel4_Cam_PLC (STT, X, Y) VALUES (@STT, @X, @Y)");
                    for (int i = 0; i < Number_Tool_Check_VisionBot; i++)
                    {
                        PLC1.Write_Data_DWord_("D" + (10001 + Memory_PLC.K2000 + i * 2).ToString(), data_pos_X1[i]);// X POS CHECK 1
                        PLC1.Write_Data_DWord_("D" + (10101 + Memory_PLC.K2000 + i * 2).ToString(), data_pos_Y1[i]);// Y POS CHECK 1
                        PLC1.Write_Data_DWord_("D" + (10201 + Memory_PLC.K2000 + i * 2).ToString(), data_pos_X2[i]);// X POS CHECK 2
                        PLC1.Write_Data_DWord_("D" + (10301 + Memory_PLC.K2000 + i * 2).ToString(), data_pos_Y2[i]);// Y POS CHECK 2
                        PLC1.Write_Data_DWord_("D" + (10401 + Memory_PLC.K2000 + i * 2).ToString(), data_pos_X3[i]);// X POS CHECK 3
                        PLC1.Write_Data_DWord_("D" + (10501 + Memory_PLC.K2000 + i * 2).ToString(), data_pos_Y3[i]);// Y POS CHECK 3
                        PLC1.Write_Data_DWord_("D" + (13001 + Memory_PLC.K2000 + i * 2).ToString(), data_pos_X4[i]);// X POS CHECK 4
                        PLC1.Write_Data_DWord_("D" + (13101 + Memory_PLC.K2000 + i * 2).ToString(), data_pos_Y4[i]);// Y POS CHECK 4
                    }
                    WaitFormHelper.Close();
                    Message_Box_OK("Ghi PLC thành công", "Write PLC");
                }
                else
                {
                    WaitFormHelper.Close();
                    Message_Box_Warring("Min Block = 1", "Warring");
                }
            }
            catch (Exception e)
            {
                WaitFormHelper.Close();
                Message_Box_Error(e.ToString(), "Get_Offset_Height_Vision_Bottom");
            }

        }
        private void Get_Offset_Height_Vision_Bottom()
        {
            string[] Array_Offset_Z1 = new string[20];
            string[] Array_Offset_Z2 = new string[20];
            string[] Array_Offset_Z3 = new string[20];
            string[] Array_Offset_Z4 = new string[20];
            try
            {
                for (int i = 1; i <= 20; i++)
                {
                    UITextBox txt_name1 = this.Controls.Find("txt_Offset_Z_T" + i, true).FirstOrDefault() as UITextBox;
                    UITextBox txt_name2 = this.Controls.Find("txt_Offset_Z2_T" + i, true).FirstOrDefault() as UITextBox;
                    UITextBox txt_name3 = this.Controls.Find("txt_Offset_Z3_T" + i, true).FirstOrDefault() as UITextBox;
                    UITextBox txt_name4 = this.Controls.Find("txt_Offset_Z4_T" + i, true).FirstOrDefault() as UITextBox;
                    Array_Offset_Z1[i - 1] = txt_name1.Text;
                    Array_Offset_Z2[i - 1] = txt_name2.Text;
                    Array_Offset_Z3[i - 1] = txt_name3.Text;
                    Array_Offset_Z4[i - 1] = txt_name4.Text;
                }
                int total_check_FPCB = PLC1.Read_Data_DWord_("D" + (9100 + Memory_PLC.K2000).ToString());
                int[] IsUpdate = new int[total_check_FPCB];
                for (int j = 0; j < total_check_FPCB; j++)
                {
                    if (Convert.ToDouble(Array_Offset_Z1[j]) < 2 && Convert.ToDouble(Array_Offset_Z2[j]) < 2 && Convert.ToDouble(Array_Offset_Z3[j]) < 2 && Convert.ToDouble(Array_Offset_Z4[j]) < 2)
                    {
                        IsUpdate[j] = 1;
                    }
                    else
                    {
                        IsUpdate[j] = 0;
                        WaitFormHelper.Close();
                        Message_Box_Error("Max Offset < 2 mm", "Error");
                    }
                }
                bool Is_Update = IsUpdate.Contains(0);
                if (Is_Update == false)
                {
                    ConnectSQLite();
                    using (var cmd = new SQLiteCommand("DELETE FROM Offset_Z_Check_Connector;", Conn))
                    {
                        cmd.ExecuteNonQuery();
                    }
                    using (var cmd = new SQLiteCommand("DELETE FROM Offset_Z_Check_FPCB;", Conn))
                    {
                        cmd.ExecuteNonQuery();
                    }
                    using (var cmd = new SQLiteCommand("DELETE FROM Offset_Z_Check_Bien_Dang;", Conn))
                    {
                        cmd.ExecuteNonQuery();
                    }
                    using (var cmd = new SQLiteCommand("DELETE FROM Offset_Z_Check_4;", Conn))
                    {
                        cmd.ExecuteNonQuery();
                    }
                    using (var cmd = new SQLiteCommand("DELETE FROM TABLE_OFFSET_Z_CHECK_VISION;", Conn))
                    {
                        cmd.ExecuteNonQuery();
                    }
                    #region Check 1
                    string saveposSQL1 = string.Format("INSERT OR REPLACE INTO Offset_Z_Check_Connector (STT,Z1,Z2,Z3,Z4,Z5,Z6,Z7,Z8,Z9,Z10, Z11,Z12,Z13,Z14,Z15,Z16,Z17,Z18,Z19,Z20) " +
                         "VALUES (@STT,@Z1,@Z2,@Z3,@Z4,@Z5,@Z6,@Z7,@Z8,@Z9,@Z10, @Z11,@Z12,@Z13,@Z14,@Z15,@Z16,@Z17,@Z18,@Z19,@Z20)");
                    using (SQLiteCommand cmd = new SQLiteCommand(saveposSQL1, Conn))
                    {
                        cmd.Parameters.AddWithValue("@STT", 1);
                        cmd.Parameters.AddWithValue("@Z1", Convert.ToDouble(Array_Offset_Z1[0]));
                        cmd.Parameters.AddWithValue("@Z2", Convert.ToDouble(Array_Offset_Z1[1]));
                        cmd.Parameters.AddWithValue("@Z3", Convert.ToDouble(Array_Offset_Z1[2]));
                        cmd.Parameters.AddWithValue("@Z4", Convert.ToDouble(Array_Offset_Z1[3]));
                        cmd.Parameters.AddWithValue("@Z5", Convert.ToDouble(Array_Offset_Z1[4]));
                        cmd.Parameters.AddWithValue("@Z6", Convert.ToDouble(Array_Offset_Z1[5]));
                        cmd.Parameters.AddWithValue("@Z7", Convert.ToDouble(Array_Offset_Z1[6]));
                        cmd.Parameters.AddWithValue("@Z8", Convert.ToDouble(Array_Offset_Z1[7]));
                        cmd.Parameters.AddWithValue("@Z9", Convert.ToDouble(Array_Offset_Z1[8]));
                        cmd.Parameters.AddWithValue("@Z10", Convert.ToDouble(Array_Offset_Z1[9]));
                        cmd.Parameters.AddWithValue("@Z11", Convert.ToDouble(Array_Offset_Z1[10]));
                        cmd.Parameters.AddWithValue("@Z12", Convert.ToDouble(Array_Offset_Z1[11]));
                        cmd.Parameters.AddWithValue("@Z13", Convert.ToDouble(Array_Offset_Z1[12]));
                        cmd.Parameters.AddWithValue("@Z14", Convert.ToDouble(Array_Offset_Z1[13]));
                        cmd.Parameters.AddWithValue("@Z15", Convert.ToDouble(Array_Offset_Z1[14]));
                        cmd.Parameters.AddWithValue("@Z16", Convert.ToDouble(Array_Offset_Z1[15]));
                        cmd.Parameters.AddWithValue("@Z17", Convert.ToDouble(Array_Offset_Z1[16]));
                        cmd.Parameters.AddWithValue("@Z18", Convert.ToDouble(Array_Offset_Z1[17]));
                        cmd.Parameters.AddWithValue("@Z19", Convert.ToDouble(Array_Offset_Z1[18]));
                        cmd.Parameters.AddWithValue("@Z20", Convert.ToDouble(Array_Offset_Z1[19]));
                        cmd.ExecuteNonQuery();
                    }
                    Z1_1 = Convert.ToDouble(txt_Offset_Z_T1.Text);
                    Z1_2 = Convert.ToDouble(txt_Offset_Z_T2.Text);
                    Z1_3 = Convert.ToDouble(txt_Offset_Z_T3.Text);
                    Z1_4 = Convert.ToDouble(txt_Offset_Z_T4.Text);
                    Z1_5 = Convert.ToDouble(txt_Offset_Z_T5.Text);
                    Z1_6 = Convert.ToDouble(txt_Offset_Z_T6.Text);
                    Z1_7 = Convert.ToDouble(txt_Offset_Z_T7.Text);
                    Z1_8 = Convert.ToDouble(txt_Offset_Z_T8.Text);
                    Z1_9 = Convert.ToDouble(txt_Offset_Z_T9.Text);
                    Z1_10 = Convert.ToDouble(txt_Offset_Z_T10.Text);
                    Z1_11 = Convert.ToDouble(txt_Offset_Z_T11.Text);
                    Z1_12 = Convert.ToDouble(txt_Offset_Z_T12.Text);
                    Z1_13 = Convert.ToDouble(txt_Offset_Z_T13.Text);
                    Z1_14 = Convert.ToDouble(txt_Offset_Z_T14.Text);
                    Z1_15 = Convert.ToDouble(txt_Offset_Z_T15.Text);
                    Z1_16 = Convert.ToDouble(txt_Offset_Z_T16.Text);
                    Z1_17 = Convert.ToDouble(txt_Offset_Z_T17.Text);
                    Z1_18 = Convert.ToDouble(txt_Offset_Z_T18.Text);
                    Z1_19 = Convert.ToDouble(txt_Offset_Z_T19.Text);
                    Z1_20 = Convert.ToDouble(txt_Offset_Z_T20.Text);
                    #endregion
                    #region Check 2
                    string saveposSQL2 = string.Format("INSERT OR REPLACE INTO Offset_Z_Check_FPCB (STT,Z1,Z2,Z3,Z4,Z5,Z6,Z7,Z8,Z9,Z10, Z11,Z12,Z13,Z14,Z15,Z16,Z17,Z18,Z19,Z20) " +
                        "VALUES (@STT,@Z1,@Z2,@Z3,@Z4,@Z5,@Z6,@Z7,@Z8,@Z9,@Z10, @Z11,@Z12,@Z13,@Z14,@Z15,@Z16,@Z17,@Z18,@Z19,@Z20)");
                    using (SQLiteCommand cmd = new SQLiteCommand(saveposSQL2, Conn))
                    {
                        cmd.Parameters.AddWithValue("@STT", 1);
                        cmd.Parameters.AddWithValue("@Z1", Convert.ToDouble(Array_Offset_Z2[0]));
                        cmd.Parameters.AddWithValue("@Z2", Convert.ToDouble(Array_Offset_Z2[1]));
                        cmd.Parameters.AddWithValue("@Z3", Convert.ToDouble(Array_Offset_Z2[2]));
                        cmd.Parameters.AddWithValue("@Z4", Convert.ToDouble(Array_Offset_Z2[3]));
                        cmd.Parameters.AddWithValue("@Z5", Convert.ToDouble(Array_Offset_Z2[4]));
                        cmd.Parameters.AddWithValue("@Z6", Convert.ToDouble(Array_Offset_Z2[5]));
                        cmd.Parameters.AddWithValue("@Z7", Convert.ToDouble(Array_Offset_Z2[6]));
                        cmd.Parameters.AddWithValue("@Z8", Convert.ToDouble(Array_Offset_Z2[7]));
                        cmd.Parameters.AddWithValue("@Z9", Convert.ToDouble(Array_Offset_Z2[8]));
                        cmd.Parameters.AddWithValue("@Z10", Convert.ToDouble(Array_Offset_Z2[9]));
                        cmd.Parameters.AddWithValue("@Z11", Convert.ToDouble(Array_Offset_Z2[10]));
                        cmd.Parameters.AddWithValue("@Z12", Convert.ToDouble(Array_Offset_Z2[11]));
                        cmd.Parameters.AddWithValue("@Z13", Convert.ToDouble(Array_Offset_Z2[12]));
                        cmd.Parameters.AddWithValue("@Z14", Convert.ToDouble(Array_Offset_Z2[13]));
                        cmd.Parameters.AddWithValue("@Z15", Convert.ToDouble(Array_Offset_Z2[14]));
                        cmd.Parameters.AddWithValue("@Z16", Convert.ToDouble(Array_Offset_Z2[15]));
                        cmd.Parameters.AddWithValue("@Z17", Convert.ToDouble(Array_Offset_Z2[16]));
                        cmd.Parameters.AddWithValue("@Z18", Convert.ToDouble(Array_Offset_Z2[17]));
                        cmd.Parameters.AddWithValue("@Z19", Convert.ToDouble(Array_Offset_Z2[18]));
                        cmd.Parameters.AddWithValue("@Z20", Convert.ToDouble(Array_Offset_Z2[19]));
                        cmd.ExecuteNonQuery();
                    }

                    Z2_1 = Convert.ToDouble(txt_Offset_Z2_T1.Text);
                    Z2_2 = Convert.ToDouble(txt_Offset_Z2_T2.Text);
                    Z2_3 = Convert.ToDouble(txt_Offset_Z2_T3.Text);
                    Z2_4 = Convert.ToDouble(txt_Offset_Z2_T4.Text);
                    Z2_5 = Convert.ToDouble(txt_Offset_Z2_T5.Text);
                    Z2_6 = Convert.ToDouble(txt_Offset_Z2_T6.Text);
                    Z2_7 = Convert.ToDouble(txt_Offset_Z2_T7.Text);
                    Z2_8 = Convert.ToDouble(txt_Offset_Z2_T8.Text);
                    Z2_9 = Convert.ToDouble(txt_Offset_Z2_T9.Text);
                    Z2_10 = Convert.ToDouble(txt_Offset_Z2_T10.Text);
                    Z2_11 = Convert.ToDouble(txt_Offset_Z2_T11.Text);
                    Z2_12 = Convert.ToDouble(txt_Offset_Z2_T12.Text);
                    Z2_13 = Convert.ToDouble(txt_Offset_Z2_T13.Text);
                    Z2_14 = Convert.ToDouble(txt_Offset_Z2_T14.Text);
                    Z2_15 = Convert.ToDouble(txt_Offset_Z2_T15.Text);
                    Z2_16 = Convert.ToDouble(txt_Offset_Z2_T16.Text);
                    Z2_17 = Convert.ToDouble(txt_Offset_Z2_T17.Text);
                    Z2_18 = Convert.ToDouble(txt_Offset_Z2_T18.Text);
                    Z2_19 = Convert.ToDouble(txt_Offset_Z2_T19.Text);
                    Z2_20 = Convert.ToDouble(txt_Offset_Z2_T20.Text);
                    #endregion
                    #region Check 3
                    string saveposSQL3 = string.Format("INSERT OR REPLACE INTO Offset_Z_Check_Bien_Dang (STT,Z1,Z2,Z3,Z4,Z5,Z6,Z7,Z8,Z9,Z10, Z11,Z12,Z13,Z14,Z15,Z16,Z17,Z18,Z19,Z20) " +
                        "VALUES (@STT,@Z1,@Z2,@Z3,@Z4,@Z5,@Z6,@Z7,@Z8,@Z9,@Z10, @Z11,@Z12,@Z13,@Z14,@Z15,@Z16,@Z17,@Z18,@Z19,@Z20)");
                    using (SQLiteCommand cmd = new SQLiteCommand(saveposSQL3, Conn))
                    {
                        cmd.Parameters.AddWithValue("@STT", 1);
                        cmd.Parameters.AddWithValue("@Z1", Convert.ToDouble(Array_Offset_Z3[0]));
                        cmd.Parameters.AddWithValue("@Z2", Convert.ToDouble(Array_Offset_Z3[1]));
                        cmd.Parameters.AddWithValue("@Z3", Convert.ToDouble(Array_Offset_Z3[2]));
                        cmd.Parameters.AddWithValue("@Z4", Convert.ToDouble(Array_Offset_Z3[3]));
                        cmd.Parameters.AddWithValue("@Z5", Convert.ToDouble(Array_Offset_Z3[4]));
                        cmd.Parameters.AddWithValue("@Z6", Convert.ToDouble(Array_Offset_Z3[5]));
                        cmd.Parameters.AddWithValue("@Z7", Convert.ToDouble(Array_Offset_Z3[6]));
                        cmd.Parameters.AddWithValue("@Z8", Convert.ToDouble(Array_Offset_Z3[7]));
                        cmd.Parameters.AddWithValue("@Z9", Convert.ToDouble(Array_Offset_Z3[8]));
                        cmd.Parameters.AddWithValue("@Z10", Convert.ToDouble(Array_Offset_Z3[9]));
                        cmd.Parameters.AddWithValue("@Z11", Convert.ToDouble(Array_Offset_Z3[10]));
                        cmd.Parameters.AddWithValue("@Z12", Convert.ToDouble(Array_Offset_Z3[11]));
                        cmd.Parameters.AddWithValue("@Z13", Convert.ToDouble(Array_Offset_Z3[12]));
                        cmd.Parameters.AddWithValue("@Z14", Convert.ToDouble(Array_Offset_Z3[13]));
                        cmd.Parameters.AddWithValue("@Z15", Convert.ToDouble(Array_Offset_Z3[14]));
                        cmd.Parameters.AddWithValue("@Z16", Convert.ToDouble(Array_Offset_Z3[15]));
                        cmd.Parameters.AddWithValue("@Z17", Convert.ToDouble(Array_Offset_Z3[16]));
                        cmd.Parameters.AddWithValue("@Z18", Convert.ToDouble(Array_Offset_Z3[17]));
                        cmd.Parameters.AddWithValue("@Z19", Convert.ToDouble(Array_Offset_Z3[18]));
                        cmd.Parameters.AddWithValue("@Z20", Convert.ToDouble(Array_Offset_Z3[19]));
                        cmd.ExecuteNonQuery();
                    }

                    Z3_1 = Convert.ToDouble(txt_Offset_Z3_T1.Text);
                    Z3_2 = Convert.ToDouble(txt_Offset_Z3_T2.Text);
                    Z3_3 = Convert.ToDouble(txt_Offset_Z3_T3.Text);
                    Z3_4 = Convert.ToDouble(txt_Offset_Z3_T4.Text);
                    Z3_5 = Convert.ToDouble(txt_Offset_Z3_T5.Text);
                    Z3_6 = Convert.ToDouble(txt_Offset_Z3_T6.Text);
                    Z3_7 = Convert.ToDouble(txt_Offset_Z3_T7.Text);
                    Z3_8 = Convert.ToDouble(txt_Offset_Z3_T8.Text);
                    Z3_9 = Convert.ToDouble(txt_Offset_Z3_T9.Text);
                    Z3_10 = Convert.ToDouble(txt_Offset_Z3_T10.Text);
                    Z3_11 = Convert.ToDouble(txt_Offset_Z3_T11.Text);
                    Z3_12 = Convert.ToDouble(txt_Offset_Z3_T12.Text);
                    Z3_13 = Convert.ToDouble(txt_Offset_Z3_T13.Text);
                    Z3_14 = Convert.ToDouble(txt_Offset_Z3_T14.Text);
                    Z3_15 = Convert.ToDouble(txt_Offset_Z3_T15.Text);
                    Z3_16 = Convert.ToDouble(txt_Offset_Z3_T16.Text);
                    Z3_17 = Convert.ToDouble(txt_Offset_Z3_T17.Text);
                    Z3_18 = Convert.ToDouble(txt_Offset_Z3_T18.Text);
                    Z3_19 = Convert.ToDouble(txt_Offset_Z3_T19.Text);
                    Z3_20 = Convert.ToDouble(txt_Offset_Z3_T20.Text);
                    #endregion
                    #region Check 4
                    string saveposSQL4 = string.Format("INSERT OR REPLACE INTO Offset_Z_Check_4 (STT,Z1,Z2,Z3,Z4,Z5,Z6,Z7,Z8,Z9,Z10, Z11,Z12,Z13,Z14,Z15,Z16,Z17,Z18,Z19,Z20) " +
                        "VALUES (@STT,@Z1,@Z2,@Z3,@Z4,@Z5,@Z6,@Z7,@Z8,@Z9,@Z10, @Z11,@Z12,@Z13,@Z14,@Z15,@Z16,@Z17,@Z18,@Z19,@Z20)");
                    using (SQLiteCommand cmd = new SQLiteCommand(saveposSQL4, Conn))
                    {
                        cmd.Parameters.AddWithValue("@STT", 1);
                        cmd.Parameters.AddWithValue("@Z1", Convert.ToDouble(Array_Offset_Z4[0]));
                        cmd.Parameters.AddWithValue("@Z2", Convert.ToDouble(Array_Offset_Z4[1]));
                        cmd.Parameters.AddWithValue("@Z3", Convert.ToDouble(Array_Offset_Z4[2]));
                        cmd.Parameters.AddWithValue("@Z4", Convert.ToDouble(Array_Offset_Z4[3]));
                        cmd.Parameters.AddWithValue("@Z5", Convert.ToDouble(Array_Offset_Z4[4]));
                        cmd.Parameters.AddWithValue("@Z6", Convert.ToDouble(Array_Offset_Z4[5]));
                        cmd.Parameters.AddWithValue("@Z7", Convert.ToDouble(Array_Offset_Z4[6]));
                        cmd.Parameters.AddWithValue("@Z8", Convert.ToDouble(Array_Offset_Z4[7]));
                        cmd.Parameters.AddWithValue("@Z9", Convert.ToDouble(Array_Offset_Z4[8]));
                        cmd.Parameters.AddWithValue("@Z10", Convert.ToDouble(Array_Offset_Z4[9]));
                        cmd.Parameters.AddWithValue("@Z11", Convert.ToDouble(Array_Offset_Z4[10]));
                        cmd.Parameters.AddWithValue("@Z12", Convert.ToDouble(Array_Offset_Z4[11]));
                        cmd.Parameters.AddWithValue("@Z13", Convert.ToDouble(Array_Offset_Z4[12]));
                        cmd.Parameters.AddWithValue("@Z14", Convert.ToDouble(Array_Offset_Z4[13]));
                        cmd.Parameters.AddWithValue("@Z15", Convert.ToDouble(Array_Offset_Z4[14]));
                        cmd.Parameters.AddWithValue("@Z16", Convert.ToDouble(Array_Offset_Z4[15]));
                        cmd.Parameters.AddWithValue("@Z17", Convert.ToDouble(Array_Offset_Z4[16]));
                        cmd.Parameters.AddWithValue("@Z18", Convert.ToDouble(Array_Offset_Z4[17]));
                        cmd.Parameters.AddWithValue("@Z19", Convert.ToDouble(Array_Offset_Z4[18]));
                        cmd.Parameters.AddWithValue("@Z20", Convert.ToDouble(Array_Offset_Z4[19]));
                        cmd.ExecuteNonQuery();
                    }
                    Z4_1 = Convert.ToDouble(txt_Offset_Z4_T1.Text);
                    Z4_2 = Convert.ToDouble(txt_Offset_Z4_T2.Text);
                    Z4_3 = Convert.ToDouble(txt_Offset_Z4_T3.Text);
                    Z4_4 = Convert.ToDouble(txt_Offset_Z4_T4.Text);
                    Z4_5 = Convert.ToDouble(txt_Offset_Z4_T5.Text);
                    Z4_6 = Convert.ToDouble(txt_Offset_Z4_T6.Text);
                    Z4_7 = Convert.ToDouble(txt_Offset_Z4_T7.Text);
                    Z4_8 = Convert.ToDouble(txt_Offset_Z4_T8.Text);
                    Z4_9 = Convert.ToDouble(txt_Offset_Z4_T9.Text);
                    Z4_10 = Convert.ToDouble(txt_Offset_Z4_T10.Text);
                    Z4_11 = Convert.ToDouble(txt_Offset_Z4_T11.Text);
                    Z4_12 = Convert.ToDouble(txt_Offset_Z4_T12.Text);
                    Z4_13 = Convert.ToDouble(txt_Offset_Z4_T13.Text);
                    Z4_14 = Convert.ToDouble(txt_Offset_Z4_T14.Text);
                    Z4_15 = Convert.ToDouble(txt_Offset_Z4_T15.Text);
                    Z4_16 = Convert.ToDouble(txt_Offset_Z4_T16.Text);
                    Z4_17 = Convert.ToDouble(txt_Offset_Z4_T17.Text);
                    Z4_18 = Convert.ToDouble(txt_Offset_Z4_T18.Text);
                    Z4_19 = Convert.ToDouble(txt_Offset_Z4_T19.Text);
                    Z4_20 = Convert.ToDouble(txt_Offset_Z4_T20.Text);
                    #endregion
                    #region write PLC
                    int[] data_pos_Z1 = new int[total_check_FPCB];
                    int[] data_pos_Z2 = new int[total_check_FPCB];
                    int[] data_pos_Z3 = new int[total_check_FPCB];
                    int[] data_pos_Z4 = new int[total_check_FPCB];
                    double[] Z_Check1 = { Z1_1, Z1_2, Z1_3, Z1_4, Z1_5, Z1_6, Z1_7, Z1_8, Z1_9, Z1_10, Z1_11, Z1_12, Z1_13, Z1_14, Z1_15, Z1_16, Z1_17, Z1_18, Z1_19, Z1_20 };
                    double[] Z_Check2 = { Z2_1, Z2_2, Z2_3, Z2_4, Z2_5, Z2_6, Z2_7, Z2_8, Z2_9, Z2_10, Z2_11, Z2_12, Z2_13, Z2_14, Z2_15, Z2_16, Z2_17, Z2_18, Z2_19, Z2_20 };
                    double[] Z_Check3 = { Z3_1, Z3_2, Z3_3, Z3_4, Z3_5, Z3_6, Z3_7, Z3_8, Z3_9, Z3_10, Z3_11, Z3_12, Z3_13, Z3_14, Z3_15, Z3_16, Z3_17, Z3_18, Z3_19, Z3_20 };
                    double[] Z_Check4 = { Z4_1, Z4_2, Z4_3, Z4_4, Z4_5, Z4_6, Z4_7, Z4_8, Z4_9, Z4_10, Z4_11, Z4_12, Z4_13, Z4_14, Z4_15, Z4_16, Z4_17, Z4_18, Z4_19, Z4_20 };
                    //POS CHUP 1-2-3
                    if (total_check_FPCB > 0)
                    {
                        for (int i = 0; i < total_check_FPCB; i++)
                        {
                            data_pos_Z1[i] = Convert.ToInt32(Convert.ToDouble(COR5541.Text) * 1000 + Z_Check1[i] * 1000);
                            PLC1.Write_Data_DWord_("D" + (10601 + i * 2 + +Memory_PLC.K2000).ToString(), data_pos_Z1[i]);//Z1
                            data_pos_Z2[i] = Convert.ToInt32(Convert.ToDouble(COR5543.Text) * 1000 + Z_Check2[i] * 1000);
                            PLC1.Write_Data_DWord_("D" + (10701 + i * 2 + Memory_PLC.K2000).ToString(), data_pos_Z2[i]);//z2
                            data_pos_Z3[i] = Convert.ToInt32(Convert.ToDouble(COR5545.Text) * 1000 + Z_Check3[i] * 1000);
                            PLC1.Write_Data_DWord_("D" + (10801 + i * 2 + Memory_PLC.K2000).ToString(), data_pos_Z3[i]);//z3
                            data_pos_Z4[i] = Convert.ToInt32(Convert.ToDouble(COR5547.Text) * 1000 + Z_Check4[i] * 1000);
                            PLC1.Write_Data_DWord_("D" + (13201 + i * 2 + Memory_PLC.K2000).ToString(), data_pos_Z4[i]);//z4
                        }
                        string DIR_UPDATE = string.Format("INSERT OR REPLACE INTO TABLE_OFFSET_Z_CHECK_VISION (STT, Z1,Z2,Z3,Z4) VALUES (@STT, @Z1, @Z2,@Z3,@Z4)");
                        for (int i = 0; i < total_check_FPCB; i++)
                        {
                            using (SQLiteCommand cmd = new SQLiteCommand(DIR_UPDATE, Conn))
                            {
                                cmd.Parameters.AddWithValue("@STT", i + 1);
                                cmd.Parameters.AddWithValue("@Z1", Convert.ToDouble(data_pos_Z1[i]) / 1000);
                                cmd.Parameters.AddWithValue("@Z2", Convert.ToDouble(data_pos_Z2[i]) / 1000);
                                cmd.Parameters.AddWithValue("@Z3", Convert.ToDouble(data_pos_Z3[i]) / 1000);
                                cmd.Parameters.AddWithValue("@Z4", Convert.ToDouble(data_pos_Z4[i]) / 1000);
                                cmd.ExecuteNonQuery();
                            }
                        }
                        DisConSQLite();
                        WaitFormHelper.Close();
                        Message_Box_OK("Ghi dữ liệu thành công", "SQL-PLC");
                    }
                    else
                    {
                        Message_Box_Error("Số lần check = 0 -Fail", "Offset Check");
                    }
                    #endregion
                }
                else
                {
                    WaitFormHelper.Close();
                    Message_Box_Error("Kiểm tra lại giá trị Offset (< 2mm)", "Error");
                }
            }
            catch (Exception e)
            {
                WaitFormHelper.Close();
                Message_Box_Error(e.ToString(), "Get_Offset_Height_Vision_Bottom");
            }
        }
        private async void Get_Offset_Vision_Bottom_Click(object sender, ToolStripItemClickedEventArgs e)
        {
            ToolStripItem clickedItem = e.ClickedItem;
            int value = Get_Offset_Vision_Bottom.Items.IndexOf(clickedItem);
            await WaitFormHelper.Show();
            await Task.Run(() =>
            {
                if (value == 0)
                {
                    Get_Offset_Motion_Check_Vision_Bottom();
                }
                else
                {
                    Get_Offset_Height_Vision_Bottom();
                }
            });

        }
        private void Show_data_offset_Z_VisionBottom_Click(object sender, EventArgs e)
        {
            Show_data_offset_ZVisionBottom.Config_LoadDataRB_DataGridView_Z();
            Show_data_offset_ZVisionBottom.Config_LoadDataRB_DataGridView_1();
            Show_data_offset_ZVisionBottom.Config_LoadDataRB_DataGridView_2();
            Show_data_offset_ZVisionBottom.Config_LoadDataRB_DataGridView_3();
            Show_data_offset_ZVisionBottom.Config_LoadDataRB_DataGridView_4();
            this.Controls.Add(Show_data_offset_ZVisionBottom);
            Show_data_offset_ZVisionBottom.Location = new Point(130, 290);
            Show_data_offset_ZVisionBottom.BringToFront();
            Show_data_offset_ZVisionBottom.Show();
        }

        private void uiMenuBtn_get_Offset_Vision_Click(object sender, EventArgs e)
        {
            Get_Offset_Vision_Bottom.Show(uiMenuBtn_get_Offset_Vision, 0, 0);
        }
        private void Edit_coordinate(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                UITextBox txt = sender as UITextBox;
                if (txt == null) return;
                try
                {
                    int add = Convert.ToInt32(txt.Tag.ToString());
                    int value = Convert.ToInt32(Convert.ToDouble(txt.Text) * 1000);
                    if (txt.TagString == "OK")
                    {
                        PLC1.Write_Data_DWord_("D" + (add + Memory_PLC.K800), value);
                        Message_Box_OK("Transfer data " + txt.Name + " => D" + add.ToString() + " Value New = " + value.ToString() + "\n" + "\n" + "Lưu ý Coor địa chỉ gửi trùng địa chỉ nhận!", "Transfer data");
                        this.ActiveControl = null;
                    }
                    else if (txt.TagString == "OK1")
                    {
                        PLC1.Write_Data_DWord_("D" + (add + Memory_PLC.K100), value);
                        Message_Box_OK("Transfer data " + txt.Name + " => D" + add.ToString() + " Value New = " + value.ToString() + "\n" + "\n" + "Lưu ý Coor địa chỉ gửi trùng địa chỉ nhận!", "Transfer data");
                        this.ActiveControl = null;
                    }
                }
                catch (Exception ex)
                {
                    Message_Box_Error(ex.ToString(), "Data Write PLC");
                }
            }
        }
        #endregion
        #region Button PLC-----------------------------------------------------------------
        // BUTTON MANUAL
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
                    PLC1.Write_DataBit_("M" + (Maddress + Memory_PLC.K800), 1);
                }
                else if (type_btn == "A1" && lengt == 2)
                {
                    PLC1.Write_DataBit_("M" + (Maddress + Memory_PLC.K100), 1);
                }
                else if (type_motion == "A2" && lengt >= 3)
                {
                    int D_add_value = Convert.ToInt32(type_btn.Substring(2, 4));
                    string txtname = btn.TagString;
                    var matches = this.Controls.Find(txtname, true);
                    if (matches.Length == 0 || !(matches[0] is UITextBox txt_cor)) return;
                    PLC1.Write_Data_DWord_("D" + (D_add_value + Memory_PLC.K800), Convert.ToInt32(Convert.ToDouble(txt_cor.Text) * 1000));
                    PLC1.Write_DataBit_("M" + (Maddress + Memory_PLC.K800).ToString(), 1);
                }
                else if (type_motion == "A3" && lengt >= 3)
                {
                    int D_add_value = Convert.ToInt32(type_btn.Substring(2, 4));
                    string txtname = btn.TagString;
                    var matches = this.Controls.Find(txtname, true);
                    if (matches.Length == 0 || !(matches[0] is UITextBox txt_cor)) return;
                    PLC1.Write_Data_DWord_("D" + (D_add_value + Memory_PLC.K100), Convert.ToInt32(Convert.ToDouble(txt_cor.Text) * 1000));
                    PLC1.Write_DataBit_("M" + (Maddress + Memory_PLC.K100).ToString(), 1);
                }
                else if (type_btn == "M" && lengt == 1)
                {
                    PLC1.Write_DataBit_("M" + (Maddress + Memory_PLC.K1000), 1);
                }
            }
            catch { Message_Box_Error("Không tìm thấy địa chỉ", "Error"); }
        }
        //BUTTON SAVE TEACH
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
                    PLC1.Write_DataBit_("M" + (M_address + Memory_PLC.K800).ToString(), 1);
                    Thread.Sleep(100);
                    StatusDisplay.Instance.Update_text3(txt_cor, PLC1.Read_Data_DWord_("D" + (Add_Value_Cor + Memory_PLC.K800).ToString()));
                }
                else if (type_btn == "CO1")
                {
                    PLC1.Write_DataBit_("M" + (M_address + Memory_PLC.K100).ToString(), 1);
                    Thread.Sleep(100);
                    StatusDisplay.Instance.Update_text3(txt_cor, PLC1.Read_Data_DWord_("D" + (Add_Value_Cor + Memory_PLC.K100).ToString()));
                }
            }
            catch { Message_Box_Error("Lưu không thành công" + '\r' + "Xem lại kết nối PLC", "Error"); }
        }
        // BUTTON GET LIMIT
        private void button_Get_Limit_click(object sender, EventArgs e)
        {
            UISymbolButton S_btn = sender as UISymbolButton;
            if (S_btn == null) return;
            string tag_string_add = S_btn.TagString;
            string type_btn = tag_string_add.Substring(0, 3);
            try
            {
                if (type_btn == "DOG")
                {
                    int M_add = Convert.ToInt32(S_btn.Tag);
                    int D_Key = Convert.ToInt32(tag_string_add.Substring(3, tag_string_add.Length - 4));
                    string txtName = tag_string_add;
                    var matches = this.Controls.Find(txtName, true);
                    if (matches.Length == 0 || !(matches[0] is UITextBox txt_cor)) return;
                    PLC1.Write_Data_DWord_("D" + (D_Key).ToString(), Convert.ToInt32(txt_cor.Text));
                    if (PLC1.Read_Data_DWord_("D" + D_Key.ToString()) == 111111)
                    {
                        PLC1.Write_DataBit_("M" + (M_add + Memory_PLC.k60).ToString(), 1);
                        txt_cor.Text = "";
                        txt_cor.Watermark = "Type your password";

                    }
                    else
                    {
                        Message_Box_Error("Key Home Fail", "Key");
                    }
                }
                else if (type_btn == "LIM")
                {
                    int D_Key = Convert.ToInt32(tag_string_add.Substring(3, tag_string_add.Length - 3));
                    string txtName = tag_string_add;
                    var matches = this.Controls.Find(txtName, true);
                    if (matches.Length == 0 || !(matches[0] is UITextBox txt_cor)) return;
                    PLC1.Write_Data_DWord_("D" + (Memory_PLC.K30 + D_Key).ToString(), Convert.ToInt32(Convert.ToDouble(txt_cor.Text) * 1000));
                }
            }
            catch { Message_Box_Error("Key null", "Error"); }
        }
        // BUTTON SAVE OFFSET 
        private void button_save_offset_Click(object sender, EventArgs e)
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
            int value_Write = Convert.ToInt32(Convert.ToDouble(txt_cor.Text) * 1000);
            try
            {
                if (type_btn == "OFF")
                {
                    PLC1.Write_Data_DWord_("D" + (M_address + Memory_PLC.K800).ToString(), value_Write);
                }
                else if (type_btn == "AUT")
                {
                    PLC1.Write_Data_DWord_("D" + (M_address + Memory_PLC.K2000).ToString(), value_Write);
                }

            }
            catch { Message_Box_Error("Không tìm thấy địa chỉ", "Error"); }
        }
        private void button_save_Timer_Click(object sender, EventArgs e)
        {
            UISymbolButton S_btn = sender as UISymbolButton;
            if (S_btn == null) return;
            int D_address = Convert.ToInt32(S_btn.Tag);
            string tag_string_add = S_btn.TagString;
            string txtName = S_btn.TagString;
            var matches = this.Controls.Find(txtName, true);
            if (matches.Length == 0 || !(matches[0] is UITextBox txt_cor)) return;
            int Value_Write = Convert.ToInt32(Convert.ToDouble(txt_cor.Text) * 10);
            try
            {
                PLC1.Write_Data_Word_("D" + (D_address + Memory_PLC.K2000).ToString(), Value_Write);
            }
            catch { Message_Box_Error("Không tìm thấy địa chỉ", "Error"); }
        }
        //BUTTON JOG AXIS
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
                    PLC1.Write_DataBit_("M" + (address + Memory_PLC.K800).ToString(), 1);
                }
                else { Message_Box_Error("Giá trị vượt quá 99", "Error Jog Axis"); }
            }
            else if (slect_Axis == "A1J")
            {
                if (Convert.ToInt32(txt_cor.Text) <= 99)
                {
                    PLC1.Write_DataBit_("M" + (address + Memory_PLC.K100).ToString(), 1);
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
                PLC1.Write_DataBit_("M" + (address + Memory_PLC.K800).ToString(), 0);

            }
            else if (slect_Axis == "A1J")
            {
                PLC1.Write_DataBit_("M" + (address + Memory_PLC.K100).ToString(), 0);
            }
        }
        //BUTTON CYLINDER
        private void button_Cylinder_Click(object sender, EventArgs e)
        {
            UIButton btn = sender as UIButton;
            if (btn == null) return;
            int address_number = Convert.ToInt32(btn.Tag);
            //int type_cylinder = Convert.ToInt16(btn.TagString);
            try
            {
                PLC1.Write_DataBit_("M" + (address_number + Memory_PLC.K500), 1);
            }
            catch { Message_Box_Error("Không tìm thấy địa chỉ", "Error"); }
        }
        private void Change_CurrentFPCBTray_MouseDown(object sender, MouseEventArgs e)
        {
            text_curr_FPCB = true;
        }
        private void Brake_Servo_Click(object sender, EventArgs e)
        {
            UIButton btn = sender as UIButton;
            if (btn == null) return;
            int add = Convert.ToInt32(btn.Tag);
            PLC1.Write_DataBit_("M" + (add + Memory_PLC.K800).ToString(), 1);
        }
        #endregion
        #region Button Alarm---------------------------------------------------------------
        private void btn_Refesh_Alarm_Click(object sender, EventArgs e)
        {
            string query = string.Format("SELECT * from History_Alarm");
            SQLiteDataAdapter adapter = new SQLiteDataAdapter(query, Conn);
            DataSet dataSet = new DataSet();
            adapter.Fill(dataSet, "History_Alarm");
            uiDataGridView_Alarm.DataSource = dataSet.Tables["History_Alarm"];
            uiDataGridView_Alarm.Columns["Mess"].Width = 940;
            uiDataGridView_Alarm.Columns["Time"].Width = 250;
            uiDataGridView_Alarm.Columns["Date"].Width = 250;
            uiDataGridView_Alarm.Columns["Mess"].ReadOnly = true;
            uiDataGridView_Alarm.Columns["Time"].ReadOnly = true;
            uiDataGridView_Alarm.Columns["Date"].ReadOnly = true;
        }
        private void btn_Clear_History_Alarm_Click(object sender, EventArgs e)
        {
            ConnectSQLite();
            string query = string.Format("DELETE from History_Alarm");
            using (var command = new SQLiteCommand(query, Conn))
            {
                command.ExecuteNonQuery();
            }
            query = string.Format("SELECT * from History_Alarm");
            SQLiteDataAdapter adapter = new SQLiteDataAdapter(query, Conn);
            DataSet dataSet = new DataSet();
            adapter.Fill(dataSet, "History_Alarm");
            uiDataGridView_Alarm.DataSource = dataSet.Tables["History_Alarm"];
            uiDataGridView_Alarm.Columns["Mess"].Width = 940;
            uiDataGridView_Alarm.Columns["Time"].Width = 250;
            uiDataGridView_Alarm.Columns["Date"].Width = 250;
            uiDataGridView_Alarm.Columns["Mess"].ReadOnly = true;
            uiDataGridView_Alarm.Columns["Time"].ReadOnly = true;
            uiDataGridView_Alarm.Columns["Date"].ReadOnly = true;
            DisConSQLite();
        }
        #endregion
        #region Button Group Vision--------------------------------------------------------
        private async void Connect_Cam_Click(object sender, EventArgs e)
        {
            try
            {
                Save_WriteAllText(txt_IP_Robot.Text + ";" + Port_Robot.Text + ";" + txt_IP_PLC.Text + ";" + Port_PLC.Text + ";" + txt_ip_vision1.Text + ";" + txt_port_vision1.Text + ";" + txt_ip_vision2.Text + ";" + txt_port_vision2.Text, "TCPIP.txt");
            }
            catch { }
            await WaitFormHelper.ShowCam();
            await Task.Delay(2000);
            Connect_sever_Cam1();
            Connect_sever_Cam2();
            try
            {
                if (client_cam1.TcpClient.Connected == true && client_cam2.TcpClient.Connected == true)
                {
                    TaskbarManager.Instance.SetProgressValue(100, 100);
                    TaskbarManager.Instance.SetProgressState(TaskbarProgressBarState.Normal);
                    WaitFormHelper.CloseCam();
                    Message_Box_OK("Kết nối thành công Camera", "Connect Camera");
                }
                else
                {
                    TaskbarManager.Instance.SetProgressValue(100, 100);
                    TaskbarManager.Instance.SetProgressState(TaskbarProgressBarState.Error);
                    WaitFormHelper.CloseCam();
                    Message_Box_Error("Kết nối không thành công Camera", "Connect Camera");
                }
            }
            catch
            {
                WaitFormHelper.CloseCam();
                Message_Box_Error("Client Null", "TCP/IP");
            }
        }
        private void btn_Sendata1_Click(object sender, EventArgs e)
        {
            Send_data_Cam1(txt_data_send_cam1.Text);
        }
        private void btn_Sendata2_Click(object sender, EventArgs e)
        {
            Send_data_Cam2(txt_data_send_cam2.Text);
        }
        private void btn_get_data_trigger_Click(object sender, EventArgs e)
        {
            try
            {
                string add_data_trigger = txt_Add_Data_Start.Text;
                string add_data_select_1 = add_data_trigger.Substring(0, 1);
                string add_data_select_2 = add_data_trigger.Substring(1, add_data_trigger.Length - 1);
                for (int i = 0; i < Convert.ToInt16(txt_Number_Add.Text); i++)
                {
                    if (i == 0)
                    {
                        PLC1.Write_Data_Word_(add_data_select_1 + (Convert.ToInt16(add_data_select_2) + i).ToString(), Convert.ToInt16(txt_number_trigger_start.Text));
                    }
                    else
                    {
                        PLC1.Write_Data_Word_(add_data_select_1 + (Convert.ToInt16(add_data_select_2) + i).ToString(), Convert.ToInt16(txt_number_trigger_start.Text) + i * Convert.ToInt16(txt_number_index_data_trigger.Text));
                    }
                    //else if (i > 1)
                    //{
                    //    PLC1.Write_Data_Word_(add_data_select_1 + (Convert.ToInt16(add_data_select_2) + i).ToString(), Convert.ToInt16(txt_number_trigger_start.Text) + i * Convert.ToInt16(txt_number_index_data_trigger.Text));
                    //}
                }
                Message_Box_OK("Lưu thành công", "Data trigger Camera Bottom");
            }
            catch { Message_Box_Error("Lưu không thành công", "Data trigger Camera Bottom"); }
        }
        private void btn_Check_data_trigger_Click(object sender, EventArgs e)
        {
            try
            {
                txt_DataTriggerCheck.Text = string.Empty;
                string add_data_trigger = txt_Add_Data_Start.Text;
                string add_data_select_1 = add_data_trigger.Substring(0, 1);
                string add_data_select_2 = add_data_trigger.Substring(1, add_data_trigger.Length - 1);
                int[] Data_Check = PLC1.Read_Word_Arr(add_data_trigger, Convert.ToInt16(txt_Number_Add.Text));
                for (int i = 0; i < Data_Check.Length; i++)
                {
                    txt_DataTriggerCheck.Text = txt_DataTriggerCheck.Text + add_data_select_1 + (Convert.ToInt16(add_data_select_2) + i).ToString() + ": " + Data_Check[i].ToString() + "\r\n";
                }
            }
            catch (Exception ex)
            {
                Message_Box_Error(ex.ToString(), "Data Trigger Vision");
            }
        }
        private void btn_OpenSettingCamera_Click(object sender, EventArgs e)
        {

        }
        #endregion
        #region Button Info Machine--------------------------------------------------------
        public class Record
        {
            public int Qty { get; set; }
            public int OK { get; set; }
            public int NG { get; set; }
            public string Time { get; set; }
            public string Date { get; set; }
        }
        private void Config_datagridview_production()
        {
            DataGridView_Production.Columns["STT"].Width = 50;
            DataGridView_Production.Columns["Qty"].Width = 200;
            DataGridView_Production.Columns["OK"].Width = 140;
            DataGridView_Production.Columns["NG"].Width = 140;
            DataGridView_Production.Columns["Time"].Width = 216;
            DataGridView_Production.Columns["Date"].Width = 216;
            DataGridView_Production.Columns["STT"].ReadOnly = true;
            DataGridView_Production.Columns["Qty"].ReadOnly = true;
            DataGridView_Production.Columns["OK"].ReadOnly = true;
            DataGridView_Production.Columns["NG"].ReadOnly = true;
            DataGridView_Production.Columns["Time"].ReadOnly = true;
            DataGridView_Production.Columns["Date"].ReadOnly = true;
            DataGridView_Production.CellFormatting += (s, r) =>
            {
                if (r.ColumnIndex == DataGridView_Production.Columns["Qty"].Index)
                {
                    r.CellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                }
            };
            DataGridView_Production.CellFormatting += (s, r) =>
            {
                if (r.ColumnIndex == DataGridView_Production.Columns["Qty"].Index)
                {
                    r.CellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                }
            };
            DataGridView_Production.CellFormatting += (s, r) =>
            {
                if (r.ColumnIndex == DataGridView_Production.Columns["OK"].Index)
                {
                    r.CellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                }
            };
            DataGridView_Production.CellFormatting += (s, r) =>
            {
                if (r.ColumnIndex == DataGridView_Production.Columns["NG"].Index)
                {
                    r.CellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                }
            };
            DataGridView_Production.CellFormatting += (s, r) =>
            {
                if (r.ColumnIndex == DataGridView_Production.Columns["Time"].Index)
                {
                    r.CellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                }
            };
            DataGridView_Production.CellFormatting += (s, r) =>
            {
                if (r.ColumnIndex == DataGridView_Production.Columns["Date"].Index) // Xác định cột cần căn chỉnh
                {
                    r.CellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                }
            };
        }
        private DataTable allProductionData;
        private void LoadProductionPage(int pageIndex)
        {

            if (allProductionData == null || allProductionData.Rows.Count == 0)
                return;

            int pageSize = uiPagination1.PageSize;
            int skip = (pageIndex - 1) * pageSize;

            // Clone schema + copy trang dữ liệu
            DataTable pageTable = allProductionData.Clone();

            for (int i = skip; i < skip + pageSize && i < allProductionData.Rows.Count; i++)
            {
                pageTable.ImportRow(allProductionData.Rows[i]);
            }

            DataGridView_Production.DataSource = pageTable;
            Config_datagridview_production();
            int totalQty = 0, totalOK = 0, totalNG = 0; int k = 0;
            foreach (DataRow row in pageTable.Rows)
            {
                int.TryParse(row["Qty"]?.ToString(), out int qty);
                int.TryParse(row["OK"]?.ToString(), out int ok);
                int.TryParse(row["NG"]?.ToString(), out int ng);
                k++;
                totalQty += qty;
                totalOK += ok;
                totalNG += ng;
            }
            uiDataGridViewFooter1.Text = $"Tổng: → Qty: {totalQty}, OK: {totalOK}, NG: {totalNG}";
            uiDataGridViewFooter1["Qty"] = totalQty.ToString();
            uiDataGridViewFooter1["OK"] = totalOK.ToString();
            uiDataGridViewFooter1["NG"] = totalNG.ToString();
            totalQty = 0;
            totalOK = 0;
            totalNG = 0;
        }
        private void btn_Refesh_Production_Click(object sender, EventArgs e)
        {
            ConnectSQLite();
            string query = string.Format("SELECT * from Production");
            SQLiteDataAdapter adapter = new SQLiteDataAdapter(query, Conn);
            DataSet dataSet = new DataSet();
            adapter.Fill(dataSet, "Production");
            DataGridView_Production.DataSource = dataSet.Tables["Production"];
            Config_datagridview_production();
            allProductionData = dataSet.Tables["Production"];
            uiPagination1.TotalCount = allProductionData.Rows.Count;
            uiPagination1.ActivePage = 1;
            LoadProductionPage(uiPagination1.ActivePage);
            DisConSQLite();
        }
        private void LanguageCalender()
        {
            uiCalendar1.MultiLanguageSupport = true;
        }
        private void uiCalendar1_OnDateTimeChanged(object sender, UIDateTimeArgs e)
        {
            string date = uiCalendar1.Date.ToString("dd/MM/yyyy");
            string query = string.Format("SELECT * FROM Production WHERE Date = @Date");
            using (SQLiteCommand cmd = new SQLiteCommand(query, Conn))
            {
                cmd.Parameters.AddWithValue("@Date", date);
                SQLiteDataAdapter da = new SQLiteDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);
                DataGridView_Production.DataSource = dt;
                Config_datagridview_production();
                allProductionData = dt;
                uiPagination1.TotalCount = allProductionData.Rows.Count;
                uiPagination1.ActivePage = 1;
                LoadProductionPage(uiPagination1.ActivePage);
            }
        }
        private void DataGridView_Production_SelectIndexChange(object sender, int index)
        {
            index.WriteConsole("SelectedIndex");
        }
        private void uiPagination1_PageChanged(object sender, object pagingSource, int pageIndex, int count)
        {
            LoadProductionPage(uiPagination1.ActivePage);
            //LoadPage(pageIndex);
        }
        private void Config_Chart()
        {
            var option = new UIPieOption();
            option.Title.Text = "Tact Time";
            option.Title.Left = UILeftAlignment.Center;
            //toolTip
            option.ToolTip.Visible = true;
            //Legend
            option.Legend = new UILegend();
            option.Legend.Orient = UIOrient.Horizontal;
            option.Legend.Top = UITopAlignment.Bottom;
            option.Legend.Left = UILeftAlignment.Left;

            option.Legend.AddData("1");
            option.Legend.AddData("2");
            option.Legend.AddData("3");
            option.Legend.AddData("4");
            option.Legend.AddData("5");
            option.Legend.AddData("6");
            option.Legend.AddData("7");
            option.Legend.AddData("8");
            option.Legend.AddData("9");
            option.Legend.AddData("10");

            //Series
            var series = new UIPieSeries();
            series.Name = "StarCount";
            series.Center = new UICenter(50, 55);
            series.Radius = 70;
            series.Label.Show = true;
            //
            series.AddData("Time 1", Convert.ToDouble(Tact31.Text));
            series.AddData("Time 2", Convert.ToDouble(Tact32.Text));
            series.AddData("Time 3", Convert.ToDouble(Tact33.Text));
            series.AddData("Time 4", Convert.ToDouble(Tact34.Text));
            series.AddData("Time 5", Convert.ToDouble(Tact35.Text));
            series.AddData("Time 6", Convert.ToDouble(Tact36.Text));
            series.AddData("Time 7", Convert.ToDouble(Tact37.Text));
            series.AddData("Time 8", Convert.ToDouble(Tact38.Text));
            series.AddData("Time 9", Convert.ToDouble(Tact39.Text));
            series.AddData("Time 10", Convert.ToDouble(Tact40.Text));

            //Series
            option.Series.Clear();
            option.Series.Add(series);
            //Option
            Chart_Tact_Time.SetOption(option);
        }
        private void Click_click(object sender, EventArgs e)
        {
            Config_Chart();
        }
        #endregion
        #region Load data------------------------------------------------------------------
        public void load_IP()
        {
            try
            {
                string content = Read_File("TCPIP.txt");
                string content_model = Read_File("Model.txt");
                var prop = txt_IP_Robot.GetType().GetProperty("Value");
                string model = content_model;
                string[] ipp = content.Split(';');
                StatusDisplay.Instance.Update_IPtext(txt_IP_Robot, ipp[0].ToString());
                StatusDisplay.Instance.Update_text(Port_Robot, Convert.ToInt16(ipp[1]));
                StatusDisplay.Instance.Update_IPtext(txt_IP_PLC, ipp[2].ToString());
                StatusDisplay.Instance.Update_text(Port_PLC, Convert.ToInt16(ipp[3]));
                StatusDisplay.Instance.Update_IPtext(txt_ip_vision1, ipp[4].ToString());
                StatusDisplay.Instance.Update_text(txt_port_vision1, Convert.ToInt16(ipp[5]));
                StatusDisplay.Instance.Update_IPtext(txt_ip_vision2, ipp[6].ToString());
                StatusDisplay.Instance.Update_text(txt_port_vision2, Convert.ToInt16(ipp[7]));
                StatusDisplay.Instance.Update_text2(txt_model_input, model);
                StatusDisplay.Instance.Update_UILabel2(uiLabel_Name_Machine, model);
                Global.ModelName_Server = model;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
                Message_Box_Error("Lỗi đọc địa chỉ IP, file thiếu địa chỉ IP", "Error read file");
            }
        }
        public void load_pos_RB()
        {
            // Khởi tạo danh sách để lưu trữ dữ liệu
            //List<string>[] dataList = new List<string>[50];
            int i = 0;
            foreach (DataGridViewRow row in uiDataGridView1.Rows) // Duyệt qua từng dòng trong DataGridView
            {
                dataList[i] = new List<double>();
                // Truy cập vào giá trị của các ô dữ liệu trong dòng
                // cellValue1 = row.Cells["POSITION"].Value.ToString();
                string cellValue2 = row.Cells["X"].Value.ToString();
                string cellValue3 = row.Cells["Y"].Value.ToString();
                string cellValue4 = row.Cells["Z"].Value.ToString();
                string cellValue5 = row.Cells["A4"].Value.ToString();
                string cellValue6 = row.Cells["A5"].Value.ToString();
                string cellValue7 = row.Cells["C"].Value.ToString();
                // Thêm dữ liệu vào danh sách
                dataList[i].Add(Convert.ToDouble(cellValue2));
                dataList[i].Add(Convert.ToDouble(cellValue3));
                dataList[i].Add(Convert.ToDouble(cellValue4));
                dataList[i].Add(Convert.ToDouble(cellValue5));
                dataList[i].Add(Convert.ToDouble(cellValue6));
                dataList[i].Add(Convert.ToDouble(cellValue7));// + ";" + cellValue3 + ";" + cellValue4 + ";" + cellValue5 + ";" + cellValue6 + ";" + cellValue7);
                i++;
            }
            Global.Pick_Press = dataList[0].ToArray();//1
            Global.Z_Pick_Press = dataList[1].ToArray();//2
            Global.Check_Marking_Start = dataList[2].ToArray();//3
            Global.Check_Tape_Start = dataList[3].ToArray();//4
            Global.Place_Tray_1 = dataList[4].ToArray();//5
            Global.Place_Tray_2 = dataList[5].ToArray();//6
            Global.Place_Tray_3 = dataList[6].ToArray();//7
            Global.Place_Tray_4 = dataList[7].ToArray();//8
            Global.Input_FPCB = dataList[8].ToArray();//9
            Global.Z_Input_FPCB = dataList[9].ToArray();//10
            Global.Pick_FPCB_Output_1 = dataList[10].ToArray();//11
            Global.Pick_FPCB_Output_2 = dataList[11].ToArray();//12
            Global.Z_Pick_FPCB_Output_1 = dataList[12].ToArray();//13
            Global.Z_Pick_FPCB_Output_2 = dataList[13].ToArray();//13
            Global.Pos_NG = dataList[14].ToArray();//14
            Global.Z_Pos_NG = dataList[15].ToArray();//15
                                                     //
            Global.Ready_Pick_Press_1 = dataList[16].ToArray();//16
            Global.Ready_Pick_Press_2 = dataList[17].ToArray();//17
                                                               //
            Global.Ready_Check_Camtop_1 = dataList[18].ToArray();//18
            Global.Ready_Check_Camtop_2 = dataList[19].ToArray();//19
                                                                 //
            Global.Ready_Inputput_1 = dataList[20].ToArray();//20
            Global.Ready_Inputput_2 = dataList[21].ToArray();//21
                                                             //
            Global.Ready_Pick_Output_1 = dataList[22].ToArray();//22
            Global.Ready_Pick_Output_2 = dataList[23].ToArray();//23
                                                                //
            Global.Ready_Place_Tray_1 = dataList[24].ToArray();//24
            Global.Ready_Place_Tray_2 = dataList[25].ToArray();//25
            Global.Ready_Place_Tray_3 = dataList[26].ToArray();//26
                                                               //                                           
            Global.Ready_Place_NG_1 = dataList[27].ToArray();//27
            Global.Ready_Place_NG_2 = dataList[28].ToArray();//28
                                                             //
            Global.Ready_Arc_1 = dataList[29].ToArray();//29
            Global.Ready_Arc_2 = dataList[30].ToArray();//30
                                                        //
            Global.Ready_Rotation_1 = dataList[31].ToArray();//31
            Global.Ready_Rotation_2 = dataList[32].ToArray();//32
                                                             //
            Global.Homee = dataList[33].ToArray();//33
            string message_s = "";
            int Check_Z, Check_X, Check_Y;
            //ZZZZZ
            if (Global.Place_Tray_1[2] < Global.Z_Place_Satefy || Global.Place_Tray_2[2] < Global.Z_Place_Satefy || Global.Place_Tray_3[2] < Global.Z_Place_Satefy || Global.Place_Tray_4[2] < Global.Z_Place_Satefy)
            {
                Scan_Position = 1;
                message_s = "Tọa độ trục Z trong các Position Maxtrix ngoài phạm vi trục Z" + "\r";
                Check_Z = 1;
            }
            else
            {
                Scan_Position = 2;
                Check_Z = 2;
            }
            //XXXXXXX
            if (Global.Place_Tray_1[0] < Global.X_Place_Satefy_L || Global.Place_Tray_1[0] > Global.X_Place_Satefy_U
                || Global.Place_Tray_2[0] < Global.X_Place_Satefy_L || Global.Place_Tray_2[0] > Global.X_Place_Satefy_U
                || Global.Place_Tray_3[0] < Global.X_Place_Satefy_L || Global.Place_Tray_3[0] > Global.X_Place_Satefy_U
                || Global.Place_Tray_4[0] < Global.X_Place_Satefy_L || Global.Place_Tray_4[0] > Global.X_Place_Satefy_U)
            {
                Scan_Position = 1;
                message_s = message_s + "Tọa độ trục X trong các Position Maxtrix ngoài phạm vi trục X" + "\r";
                Check_X = 1;
            }
            else
            {
                Scan_Position = 2;
                Check_X = 2;
            }
            //YYYYYYYYYYYYYY
            if (Global.Place_Tray_1[1] > Global.Y_Place_Satefy_U || Global.Place_Tray_1[1] < Global.Y_Place_Satefy_L
                || Global.Place_Tray_2[1] > Global.Y_Place_Satefy_U || Global.Place_Tray_2[1] < Global.Y_Place_Satefy_L
                || Global.Place_Tray_3[1] > Global.Y_Place_Satefy_U || Global.Place_Tray_3[1] < Global.Y_Place_Satefy_L
                 || Global.Place_Tray_4[1] > Global.Y_Place_Satefy_U || Global.Place_Tray_4[1] < Global.Y_Place_Satefy_L)
            {
                Scan_Position = 1;
                message_s = message_s + "Tọa độ trục Y trong các Position Maxtrix ngoài phạm vi trục Y" + "\r";
                Check_Y = 1;
            }
            else
            {
                Scan_Position = 2;
                Check_Y = 2;
            }
            if (Scan_Position == 1 || Check_Z == 1 || Check_X == 1 || Check_Y == 1)
            {
                Message_Box_Error(message_s + "Kiểm tra lại tọa độ!", "Error Position Robot");
                Scan_Position = 1;
            }
            if (handle == 0)
            {
                StatusDisplay.Instance.Update_text(txt_set_number_fpcb, SDKHrobot.HRobot.get_counter(handle, 2));
            }
        }
        public void load_data_PLC()
        {
            if (PLC1.IsConnected == true)
            {
                // Load Speed Jog Axis 1-8
                for (int i = 1; i < 9; i++)
                {
                    UITextBox txt1 = this.Controls.Find("A0JA" + i, true).FirstOrDefault() as UITextBox;
                    if (txt1 != null) { StatusDisplay.Instance.Update_text(txt1, PLC1.Read_Data_DWord_("D" + (5108 + ((i - 1) * 100) + Memory_PLC.K800).ToString())); }
                }
                StatusDisplay.Instance.Update_text(A1JA9, PLC1.Read_Data_DWord_("D" + (6708 + Memory_PLC.K100).ToString()));//axis 9
                //Position A1
                StatusDisplay.Instance.Update_text3(COR5135, PLC1.Read_Data_DWord_("D" + (5135 + Memory_PLC.K800).ToString()));
                StatusDisplay.Instance.Update_text3(OFF5132, PLC1.Read_Data_DWord_("D" + (5132 + Memory_PLC.K800).ToString()));
                StatusDisplay.Instance.Update_text3(LIM4100, PLC1.Read_Data_DWord_("D" + (4100 + Memory_PLC.K30).ToString()));
                StatusDisplay.Instance.Update_text3(LIM4102, PLC1.Read_Data_DWord_("D" + (4102 + Memory_PLC.K30).ToString()));
                //Position A2
                StatusDisplay.Instance.Update_text3(COR5235, PLC1.Read_Data_DWord_("D" + (5235 + Memory_PLC.K800).ToString()));
                StatusDisplay.Instance.Update_text3(OFF5232, PLC1.Read_Data_DWord_("D" + (5232 + Memory_PLC.K800).ToString()));
                StatusDisplay.Instance.Update_text3(AUT9070, PLC1.Read_Data_DWord_("D" + (9070 + Memory_PLC.K2000).ToString()));
                StatusDisplay.Instance.Update_text3(OFF5294, PLC1.Read_Data_DWord_("D" + (5294 + Memory_PLC.K800).ToString()));
                StatusDisplay.Instance.Update_text3(LIM4104, PLC1.Read_Data_DWord_("D" + (4104 + Memory_PLC.K30).ToString()));
                StatusDisplay.Instance.Update_text3(LIM4106, PLC1.Read_Data_DWord_("D" + (4106 + Memory_PLC.K30).ToString()));
                //Position A3
                StatusDisplay.Instance.Update_text3(COR5335, PLC1.Read_Data_DWord_("D" + (5335 + Memory_PLC.K800).ToString()));
                StatusDisplay.Instance.Update_text3(COR5337, PLC1.Read_Data_DWord_("D" + (5337 + Memory_PLC.K800).ToString()));
                StatusDisplay.Instance.Update_text3(COR5341, PLC1.Read_Data_DWord_("D" + (5341 + Memory_PLC.K800).ToString()));
                StatusDisplay.Instance.Update_text3(COR5343, PLC1.Read_Data_DWord_("D" + (5343 + Memory_PLC.K800).ToString()));
                StatusDisplay.Instance.Update_text3(COR5345, PLC1.Read_Data_DWord_("D" + (5345 + Memory_PLC.K800).ToString()));
                StatusDisplay.Instance.Update_text3(COR5347, PLC1.Read_Data_DWord_("D" + (5347 + Memory_PLC.K800).ToString()));
                StatusDisplay.Instance.Update_text3(COR5353, PLC1.Read_Data_DWord_("D" + (5353 + Memory_PLC.K800).ToString()));
                StatusDisplay.Instance.Update_text3(COR5368, PLC1.Read_Data_DWord_("D" + (5368 + Memory_PLC.K800).ToString()));
                //Position A4
                StatusDisplay.Instance.Update_text3(COR5435, PLC1.Read_Data_DWord_("D" + (5435 + Memory_PLC.K800).ToString()));
                StatusDisplay.Instance.Update_text3(COR5437, PLC1.Read_Data_DWord_("D" + (5437 + Memory_PLC.K800).ToString()));
                StatusDisplay.Instance.Update_text3(COR5441, PLC1.Read_Data_DWord_("D" + (5441 + Memory_PLC.K800).ToString()));
                StatusDisplay.Instance.Update_text3(COR5443, PLC1.Read_Data_DWord_("D" + (5443 + Memory_PLC.K800).ToString()));
                StatusDisplay.Instance.Update_text3(COR5445, PLC1.Read_Data_DWord_("D" + (5445 + Memory_PLC.K800).ToString()));
                StatusDisplay.Instance.Update_text3(COR5447, PLC1.Read_Data_DWord_("D" + (5447 + Memory_PLC.K800).ToString()));
                StatusDisplay.Instance.Update_text3(COR5453, PLC1.Read_Data_DWord_("D" + (5453 + Memory_PLC.K800).ToString()));
                StatusDisplay.Instance.Update_text3(COR5468, PLC1.Read_Data_DWord_("D" + (5468 + Memory_PLC.K800).ToString()));
                //Position A5
                StatusDisplay.Instance.Update_text3(COR5535, PLC1.Read_Data_DWord_("D" + (5535 + Memory_PLC.K800).ToString()));
                StatusDisplay.Instance.Update_text3(COR5537, PLC1.Read_Data_DWord_("D" + (5537 + Memory_PLC.K800).ToString()));
                StatusDisplay.Instance.Update_text3(COR5541, PLC1.Read_Data_DWord_("D" + (5541 + Memory_PLC.K800).ToString()));
                StatusDisplay.Instance.Update_text3(COR5543, PLC1.Read_Data_DWord_("D" + (5543 + Memory_PLC.K800).ToString()));
                StatusDisplay.Instance.Update_text3(COR5545, PLC1.Read_Data_DWord_("D" + (5545 + Memory_PLC.K800).ToString()));
                StatusDisplay.Instance.Update_text3(COR5547, PLC1.Read_Data_DWord_("D" + (5547 + Memory_PLC.K800).ToString()));
                StatusDisplay.Instance.Update_text3(COR5553, PLC1.Read_Data_DWord_("D" + (5553 + Memory_PLC.K800).ToString()));
                StatusDisplay.Instance.Update_text3(LIM4108, PLC1.Read_Data_DWord_("D" + (4108 + Memory_PLC.K30).ToString()));
                StatusDisplay.Instance.Update_text3(LIM4110, PLC1.Read_Data_DWord_("D" + (4110 + Memory_PLC.K30).ToString()));
                StatusDisplay.Instance.Update_text3(COR5568, PLC1.Read_Data_DWord_("D" + (5568 + Memory_PLC.K800).ToString()));
                //Position A6
                StatusDisplay.Instance.Update_text3(COR5635, PLC1.Read_Data_DWord_("D" + (5635 + Memory_PLC.K800).ToString()));
                StatusDisplay.Instance.Update_text3(COR5637, PLC1.Read_Data_DWord_("D" + (5637 + Memory_PLC.K800).ToString()));
                StatusDisplay.Instance.Update_text3(COR5641, PLC1.Read_Data_DWord_("D" + (5641 + Memory_PLC.K800).ToString()));
                StatusDisplay.Instance.Update_text3(COR5653, PLC1.Read_Data_DWord_("D" + (5653 + Memory_PLC.K800).ToString()));
                StatusDisplay.Instance.Update_text3(LIM4112, PLC1.Read_Data_DWord_("D" + (4112 + Memory_PLC.K30).ToString()));
                StatusDisplay.Instance.Update_text3(LIM4114, PLC1.Read_Data_DWord_("D" + (4114 + Memory_PLC.K30).ToString()));
                StatusDisplay.Instance.Update_text3(COR5668, PLC1.Read_Data_DWord_("D" + (5668 + Memory_PLC.K800).ToString()));
                //Position A7
                StatusDisplay.Instance.Update_text3(COR5735, PLC1.Read_Data_DWord_("D" + (5735 + Memory_PLC.K800).ToString()));
                StatusDisplay.Instance.Update_text3(COR5737, PLC1.Read_Data_DWord_("D" + (5737 + Memory_PLC.K800).ToString()));
                StatusDisplay.Instance.Update_text3(COR5739, PLC1.Read_Data_DWord_("D" + (5739 + Memory_PLC.K800).ToString()));
                StatusDisplay.Instance.Update_text3(COR5741, PLC1.Read_Data_DWord_("D" + (5741 + Memory_PLC.K800).ToString()));
                StatusDisplay.Instance.Update_text3(COR5743, PLC1.Read_Data_DWord_("D" + (5743 + Memory_PLC.K800).ToString()));
                StatusDisplay.Instance.Update_text3(COR5745, PLC1.Read_Data_DWord_("D" + (5745 + Memory_PLC.K800).ToString()));
                //Position A8
                StatusDisplay.Instance.Update_text3(COR5835, PLC1.Read_Data_DWord_("D" + (5835 + Memory_PLC.K800).ToString()));
                StatusDisplay.Instance.Update_text3(COR5837, PLC1.Read_Data_DWord_("D" + (5837 + Memory_PLC.K800).ToString()));
                StatusDisplay.Instance.Update_text3(COR5839, PLC1.Read_Data_DWord_("D" + (5839 + Memory_PLC.K800).ToString()));
                StatusDisplay.Instance.Update_text3(COR5841, PLC1.Read_Data_DWord_("D" + (5841 + Memory_PLC.K800).ToString()));
                StatusDisplay.Instance.Update_text3(COR5843, PLC1.Read_Data_DWord_("D" + (5843 + Memory_PLC.K800).ToString()));
                StatusDisplay.Instance.Update_text3(COR5845, PLC1.Read_Data_DWord_("D" + (5845 + Memory_PLC.K800).ToString()));
                StatusDisplay.Instance.Update_text3(LIM4116, PLC1.Read_Data_DWord_("D" + (4116 + Memory_PLC.K30).ToString()));
                StatusDisplay.Instance.Update_text3(LIM4118, PLC1.Read_Data_DWord_("D" + (4118 + Memory_PLC.K30).ToString()));
                //Position A9
                StatusDisplay.Instance.Update_text3(CO16735, PLC1.Read_Data_DWord_("D" + (6735 + Memory_PLC.K100).ToString()));
                StatusDisplay.Instance.Update_text3(CO16737, PLC1.Read_Data_DWord_("D" + (6737 + Memory_PLC.K100).ToString()));
                StatusDisplay.Instance.Update_text3(LIM4120, PLC1.Read_Data_DWord_("D" + (4120 + Memory_PLC.K30).ToString()));
                StatusDisplay.Instance.Update_text3(LIM4122, PLC1.Read_Data_DWord_("D" + (4122 + Memory_PLC.K30).ToString()));
                //parameter 1-9
                for (int i = 1; i < 10; i++)
                {
                    UITextBox txt1 = this.Controls.Find("Creep" + i, true).FirstOrDefault() as UITextBox;
                    UITextBox txt2 = this.Controls.Find("SPHome" + i, true).FirstOrDefault() as UITextBox;
                    UITextBox txt3 = this.Controls.Find("SPAuto1_" + i, true).FirstOrDefault() as UITextBox;
                    UITextBox txt4 = this.Controls.Find("SPAuto2_" + i, true).FirstOrDefault() as UITextBox;
                    UITextBox txt5 = this.Controls.Find("SPAuto3_" + i, true).FirstOrDefault() as UITextBox;
                    UITextBox txt6 = this.Controls.Find("ACC1_" + i, true).FirstOrDefault() as UITextBox;
                    UITextBox txt7 = this.Controls.Find("DEC1_" + i, true).FirstOrDefault() as UITextBox;
                    UITextBox txt8 = this.Controls.Find("ACC2_" + i, true).FirstOrDefault() as UITextBox;
                    UITextBox txt9 = this.Controls.Find("DEC2_" + i, true).FirstOrDefault() as UITextBox;
                    UITextBox txt10 = this.Controls.Find("ACC3_" + i, true).FirstOrDefault() as UITextBox;
                    UITextBox txt11 = this.Controls.Find("DEC3_" + i, true).FirstOrDefault() as UITextBox;
                    if (i < 9)
                    {
                        if (txt1 != null) { StatusDisplay.Instance.Update_text(txt1, PLC1.Read_Data_DWord_("D" + (5112 + ((i - 1) * 100) + Memory_PLC.K800).ToString())); }
                        if (txt2 != null) { StatusDisplay.Instance.Update_text(txt2, PLC1.Read_Data_DWord_("D" + (5110 + ((i - 1) * 100) + Memory_PLC.K800).ToString())); }
                        if (txt3 != null) { StatusDisplay.Instance.Update_text(txt3, PLC1.Read_Data_DWord_("D" + (5114 + ((i - 1) * 100) + Memory_PLC.K800).ToString())); }
                        if (txt4 != null) { StatusDisplay.Instance.Update_text(txt4, PLC1.Read_Data_DWord_("D" + (5116 + ((i - 1) * 100) + Memory_PLC.K800).ToString())); }
                        if (txt5 != null) { StatusDisplay.Instance.Update_text(txt5, PLC1.Read_Data_DWord_("D" + (5118 + ((i - 1) * 100) + Memory_PLC.K800).ToString())); }
                        if (txt6 != null) { StatusDisplay.Instance.Update_text(txt6, PLC1.Read_Data_DWord_("D" + (5174 + ((i - 1) * 100) + Memory_PLC.K800).ToString())); }
                        if (txt7 != null) { StatusDisplay.Instance.Update_text(txt7, PLC1.Read_Data_DWord_("D" + (5176 + ((i - 1) * 100) + Memory_PLC.K800).ToString())); }
                        if (txt8 != null) { StatusDisplay.Instance.Update_text(txt8, PLC1.Read_Data_DWord_("D" + (5178 + ((i - 1) * 100) + Memory_PLC.K800).ToString())); }
                        if (txt9 != null) { StatusDisplay.Instance.Update_text(txt9, PLC1.Read_Data_DWord_("D" + (5180 + ((i - 1) * 100) + Memory_PLC.K800).ToString())); }
                        if (txt10 != null) { StatusDisplay.Instance.Update_text(txt10, PLC1.Read_Data_DWord_("D" + (5182 + ((i - 1) * 100) + Memory_PLC.K800).ToString())); }
                        if (txt11 != null) { StatusDisplay.Instance.Update_text(txt11, PLC1.Read_Data_DWord_("D" + (5184 + ((i - 1) * 100) + Memory_PLC.K800).ToString())); }
                    }
                    else
                    {
                        if (txt1 != null) { StatusDisplay.Instance.Update_text(txt1, PLC1.Read_Data_DWord_("D" + (6712 + Memory_PLC.K100).ToString())); }
                        if (txt2 != null) { StatusDisplay.Instance.Update_text(txt2, PLC1.Read_Data_DWord_("D" + (6710 + Memory_PLC.K100).ToString())); }
                        if (txt3 != null) { StatusDisplay.Instance.Update_text(txt3, PLC1.Read_Data_DWord_("D" + (6714 + Memory_PLC.K100).ToString())); }
                        if (txt4 != null) { StatusDisplay.Instance.Update_text(txt4, PLC1.Read_Data_DWord_("D" + (6716 + Memory_PLC.K100).ToString())); }
                        if (txt5 != null) { StatusDisplay.Instance.Update_text(txt5, PLC1.Read_Data_DWord_("D" + (6718 + Memory_PLC.K100).ToString())); }
                        if (txt6 != null) { StatusDisplay.Instance.Update_text(txt6, PLC1.Read_Data_DWord_("D" + (6774 + Memory_PLC.K100).ToString())); }
                        if (txt7 != null) { StatusDisplay.Instance.Update_text(txt7, PLC1.Read_Data_DWord_("D" + (6776 + Memory_PLC.K100).ToString())); }
                        if (txt8 != null) { StatusDisplay.Instance.Update_text(txt8, PLC1.Read_Data_DWord_("D" + (6778 + Memory_PLC.K100).ToString())); }
                        if (txt9 != null) { StatusDisplay.Instance.Update_text(txt9, PLC1.Read_Data_DWord_("D" + (6780 + Memory_PLC.K100).ToString())); }
                        if (txt10 != null) { StatusDisplay.Instance.Update_text(txt10, PLC1.Read_Data_DWord_("D" + (6782 + Memory_PLC.K100).ToString())); }
                        if (txt11 != null) { StatusDisplay.Instance.Update_text(txt11, PLC1.Read_Data_DWord_("D" + (6784 + Memory_PLC.K100).ToString())); }
                    }
                }
                //Delay
                for (int i = 1; i < 6; i++)
                {
                    UITextBox txt = this.Controls.Find("txt_delay_" + i, true).FirstOrDefault() as UITextBox;

                    if (txt != null) { StatusDisplay.Instance.Update_text_Double(txt, Convert.ToDouble(PLC1.Read_Data_Word_("D" + (9001 + i - 1 + Memory_PLC.K2000).ToString())) / 10); }
                }

                //
                checkBox_SatefyBehind.Checked = PLC1.Read_Data_Bit_("L" + (3 + Memory_PLC.K100).ToString());
                checkBox_door1.Checked = PLC1.Read_Data_Bit_("L" + (10 + Memory_PLC.K100).ToString());
                //======================use disable camtop ===================
                checkBox_camtop.Checked = PLC1.Read_Data_Bit_("L" + (5 + Memory_PLC.K100).ToString());
                CheckBox_NotUse_SS_InsideLamp1.Checked = PLC1.Read_Data_Bit_("L" + (1 + Memory_PLC.K100).ToString());
                CheckBox_NotUse_SS_InsideLamp2.Checked = PLC1.Read_Data_Bit_("L" + (2 + Memory_PLC.K100).ToString());
                CheckBox_NotUse_SS_OutsideLamp1.Checked = PLC1.Read_Data_Bit_("L" + (6 + Memory_PLC.K100).ToString());
                CheckBox_NotUse_SS_OutsideLamp2.Checked = PLC1.Read_Data_Bit_("L" + (7 + Memory_PLC.K100).ToString());
                //
                Checkbox_Connector.Checked = PLC1.Read_Data_Bit_("L" + (11 + Memory_PLC.K100).ToString());
                Checkbox_FPCB.Checked = PLC1.Read_Data_Bit_("L" + (12 + Memory_PLC.K100).ToString());
                Checkbox_BienDang.Checked = PLC1.Read_Data_Bit_("L" + (13 + Memory_PLC.K100).ToString());
                CheckBox_use_transfer_lampVision.Checked = PLC1.Read_Data_Bit_("L" + (14 + Memory_PLC.K100).ToString());
                Checkbox_check4.Checked = PLC1.Read_Data_Bit_("L" + (15 + Memory_PLC.K100).ToString());
                checkBox_ResultCamBot.Checked = PLC1.Read_Data_Bit_("L" + (93 + Memory_PLC.K100).ToString());
                checkBox_ResultCamtop.Checked = PLC1.Read_Data_Bit_("L" + (92 + Memory_PLC.K100).ToString());
                CheckBox_NotUseJig1.Checked = PLC1.Read_Data_Bit_("L" + (8 + Memory_PLC.K100).ToString());
                CheckBox_NotUseJig2.Checked = PLC1.Read_Data_Bit_("L" + (9 + Memory_PLC.K100).ToString());
                CheckBox_Use_Matrix1.Checked = PLC1.Read_Data_Bit_("L" + (51 + Memory_PLC.K100).ToString());
                CheckBox_Use_Matrix2.Checked = PLC1.Read_Data_Bit_("L" + (52 + Memory_PLC.K100).ToString());
                CheckBox_Use_Matrix3.Checked = PLC1.Read_Data_Bit_("L" + (53 + Memory_PLC.K100).ToString());
                CheckBox_MultiBlow.Checked = PLC1.Read_Data_Bit_("L" + (54 + Memory_PLC.K100).ToString());
                CheckBox_SingleBlow.Checked = PLC1.Read_Data_Bit_("L" + (55 + Memory_PLC.K100).ToString());
                CheckBox_UseCylinderCamtop.Checked = PLC1.Read_Data_Bit_("L" + (56 + Memory_PLC.K100).ToString());
                CheckBox_NotUseVaccumVisionBot.Checked = PLC1.Read_Data_Bit_("L" + (49 + Memory_PLC.K100).ToString());
                uiRadioButton_ViewMuti.Checked = PLC1.Read_Data_Bit_("L" + (18 + Memory_PLC.K100).ToString());
                uiRadioButton_ViewSign.Checked = PLC1.Read_Data_Bit_("L" + (19 + Memory_PLC.K100).ToString());
                uiRadioButton_PowerBlow1.Checked = PLC1.Read_Data_Bit_("L" + (21 + Memory_PLC.K100).ToString());
                uiRadioButton_PowerBlow2.Checked = PLC1.Read_Data_Bit_("L" + (22 + Memory_PLC.K100).ToString());
                CheckBox_DataVisionBotMode11.Checked = PLC1.Read_Data_Bit_("L" + (23 + Memory_PLC.K100).ToString());
                CheckBox_DataVisionBotMode21.Checked = PLC1.Read_Data_Bit_("L" + (24 + Memory_PLC.K100).ToString());
                CheckBox_Cylinder_BoxNG.Checked = PLC1.Read_Data_Bit_("L" + (45 + Memory_PLC.K100).ToString());
                RadioButton_Chieu_X.Checked = PLC1.Read_Data_Bit_("L" + (46 + Memory_PLC.K100).ToString());
                RadioButton_Chieu_Y.Checked = PLC1.Read_Data_Bit_("L" + (47 + Memory_PLC.K100).ToString());
                CheckBox_MasterCheck.Checked = PLC1.Read_Data_Bit_("L" + (48 + Memory_PLC.K100).ToString());
                //======================data tray=============================
                StatusDisplay.Instance.Update_text(txt_number_tray_output, PLC1.Read_Data_Word_("D" + (3001 + Memory_PLC.K1000).ToString()));
                StatusDisplay.Instance.Update_text(txt_number_tray_curr, PLC1.Read_Data_Word_("D" + (3000 + Memory_PLC.K1000).ToString()));
                StatusDisplay.Instance.Update_text(txt_Row_tray, PLC1.Read_Data_Word_("D" + (3002 + Memory_PLC.K1000).ToString()));
                StatusDisplay.Instance.Update_text(txt_Column_tray, PLC1.Read_Data_Word_("D" + (3003 + Memory_PLC.K1000).ToString()));
                StatusDisplay.Instance.Update_text(txt_Lot_Old, PLC1.Read_Data_Word_("D" + (9022 + Memory_PLC.K2000).ToString()));
                Global.Row_tray = Convert.ToInt32(txt_Row_tray.Text);
                Global.Column_tray = Convert.ToInt32(txt_Column_tray.Text);
                Global.Number_Tray_output = Convert.ToInt32(txt_number_tray_output.Text);
                //data offset vision inspection

                StatusDisplay.Instance.Update_text(txt_number_block_FPCB, PLC1.Read_Data_DWord_("D" + (9050 + Memory_PLC.K2000).ToString()));
                StatusDisplay.Instance.Update_text(txt_number_FPCB_Block, PLC1.Read_Data_DWord_("D" + (9052 + Memory_PLC.K2000).ToString()));
                StatusDisplay.Instance.Update_text3(txt_Offset_X_FPCB, PLC1.Read_Data_DWord_("D" + (9054 + Memory_PLC.K2000).ToString()));
                StatusDisplay.Instance.Update_text3(txt_Offset_Y_FPCB, PLC1.Read_Data_DWord_("D" + (9056 + Memory_PLC.K2000).ToString()));
                StatusDisplay.Instance.Update_text3(txt_Offset_X_Block, PLC1.Read_Data_DWord_("D" + (9058 + Memory_PLC.K2000).ToString()));
                StatusDisplay.Instance.Update_text3(txt_Offset_Y_Block, PLC1.Read_Data_DWord_("D" + (9060 + Memory_PLC.K2000).ToString()));
                StatusDisplay.Instance.Update_text(txt_number_data, PLC1.Read_Data_DWord_("D" + (9062 + Memory_PLC.K2000).ToString()));
                StatusDisplay.Instance.Update_text(txt_Total_Check, PLC1.Read_Data_DWord_("D" + (9100 + Memory_PLC.K2000).ToString()));
                //ARRAY TRAY OUTPUT
                StatusDisplay.Instance.Update_text(txt_Row1_x10, PLC1.Read_Data_Word_("D" + (9006 + Memory_PLC.K2000).ToString()));
                StatusDisplay.Instance.Update_text(txt_Column1_x10, PLC1.Read_Data_Word_("D" + (9007 + Memory_PLC.K2000).ToString()));
                StatusDisplay.Instance.Update_text(txt_Row2_x10, PLC1.Read_Data_Word_("D" + (9008 + Memory_PLC.K2000).ToString()));
                StatusDisplay.Instance.Update_text(txt_Column2_x10, PLC1.Read_Data_Word_("D" + (9009 + Memory_PLC.K2000).ToString()));
                StatusDisplay.Instance.Update_text(txt_Row3_x8, PLC1.Read_Data_Word_("D" + (9010 + Memory_PLC.K2000).ToString()));
                StatusDisplay.Instance.Update_text(txt_Column3_x8, PLC1.Read_Data_Word_("D" + (9011 + Memory_PLC.K2000).ToString()));
                StatusDisplay.Instance.Update_text_Double(txt_distance_row, Convert.ToDouble(PLC1.Read_Data_DWord_("D" + (9014 + Memory_PLC.K2000).ToString())) / 1000);
                StatusDisplay.Instance.Update_text_Double(txt_distance_column, Convert.ToDouble(PLC1.Read_Data_DWord_("D" + (9016 + Memory_PLC.K2000).ToString())) / 1000);
                //

                Global.Row_tray_matrix1 = StatusDisplay.Instance.ConvertInt(txt_Row1_x10);
                Global.Column_tray_matrix1 = StatusDisplay.Instance.ConvertInt(txt_Column1_x10);
                Global.Row_tray_matrix2 = StatusDisplay.Instance.ConvertInt(txt_Row2_x10);
                Global.Column_tray_matrix2 = StatusDisplay.Instance.ConvertInt(txt_Column2_x10);
                Global.Row_tray_matrix3 = StatusDisplay.Instance.ConvertInt(txt_Row3_x8);
                Global.Column_tray_matrix3 = StatusDisplay.Instance.ConvertInt(txt_Column3_x8);
                //
                StatusDisplay.Instance.Update_text(Row_input, PLC1.Read_Data_Word_("D" + (3014 + Memory_PLC.K1000).ToString()));
                StatusDisplay.Instance.Update_text(Column_input, PLC1.Read_Data_Word_("D" + (3015 + Memory_PLC.K1000).ToString()));
                Global.Row_Jig_input = StatusDisplay.Instance.ConvertInt(Row_input);
                Global.Column_Jig_input = StatusDisplay.Instance.ConvertInt(Column_input);

            }
        }
        public void load_mode()
        {
            List<int> data_parameter = new List<int>();
            string query = string.Format("SELECT* from Mode where STT='{0}' ", 1);
            SQLiteCommand cmd = new SQLiteCommand(query, Conn);
            var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                var Mode_load = Convert.ToInt16(reader["Mode"]);
                data_parameter.Add(Mode_load);
            }
            Global.combox_mode = Convert.ToInt16(data_parameter[0]);

        }
        public void load_Password()
        {
            List<string> data_parameter = new List<string>();
            string query = string.Format("SELECT* from Password where STT='{0}' ", 1);
            SQLiteCommand cmd = new SQLiteCommand(query, Conn);
            var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                var passNew = Convert.ToString(reader["PassNew"]);
                var passOld = Convert.ToString(reader["PassOld"]);
                data_parameter.Add(passNew);
                data_parameter.Add(passOld);
            }
            Global.Password_New = data_parameter[0];
            Global.Password_Old = data_parameter[1];
        }
        public void load_data_offset_Check_Vision_Bottom()
        {
            //int i = 1;
            //var data_offset = new List<Tuple<double, double, double>>();

            List<double>[] data_parameter = new List<double>[40];
            // load Z connector
            data_parameter[0] = new List<double>();
            string query = string.Format("SELECT* from Offset_Z_Check_Connector where STT='{0}' ", 1);
            SQLiteCommand cmd = new SQLiteCommand(query, Conn);
            var reader = cmd.ExecuteReader();
            bool reader_ = false;
            if (reader.Read())
            {
                reader_ = true;
            }
            for (int i = 1; i <= 20; i++)
            {
                if (reader_ == true)
                {
                    var Z_connector = Convert.ToDouble(reader["Z" + i.ToString()]);
                    data_parameter[0].Add(Z_connector);
                    UITextBox UI_textbox_Z_Connector = this.Controls.Find("txt_Offset_Z_T" + i, true).FirstOrDefault() as UITextBox;
                    if (UI_textbox_Z_Connector != null)
                    {
                        StatusDisplay.Instance.Update_text_Double(UI_textbox_Z_Connector, Convert.ToDouble(data_parameter[0][i - 1].ToString()));
                    }
                }
            }
            // load Z FPCB
            data_parameter[1] = new List<double>();
            query = string.Format("SELECT* from Offset_Z_Check_FPCB where STT='{0}' ", 1);
            cmd = new SQLiteCommand(query, Conn);
            reader = cmd.ExecuteReader();
            reader_ = false;
            if (reader.Read())
            {
                reader_ = true;
            }
            for (int i = 1; i <= 20; i++)
            {
                if (reader_ == true)
                {
                    var Z_FPCB = Convert.ToDouble(reader["Z" + i.ToString()]);
                    data_parameter[1].Add(Z_FPCB);
                    UITextBox UI_textbox_Z_FPCB = this.Controls.Find("txt_Offset_Z2_T" + i, true).FirstOrDefault() as UITextBox;
                    if (UI_textbox_Z_FPCB != null)
                    {
                        StatusDisplay.Instance.Update_text_Double(UI_textbox_Z_FPCB, Convert.ToDouble(data_parameter[1][i - 1].ToString()));
                    }
                }
            }
            //load Z Biến dạng
            data_parameter[2] = new List<double>();
            query = string.Format("SELECT* from Offset_Z_Check_Bien_Dang where STT='{0}' ", 1);
            cmd = new SQLiteCommand(query, Conn);
            reader = cmd.ExecuteReader();
            reader_ = false;
            if (reader.Read())
            {
                reader_ = true;
            }
            for (int i = 1; i <= 20; i++)
            {
                if (reader_ == true)
                {
                    var Z_Bien_dang = Convert.ToDouble(reader["Z" + i.ToString()]);
                    data_parameter[2].Add(Z_Bien_dang);
                    UITextBox UI_textbox_Z_Bien_dang = this.Controls.Find("txt_Offset_Z3_T" + i, true).FirstOrDefault() as UITextBox;
                    if (UI_textbox_Z_Bien_dang != null)
                    {
                        StatusDisplay.Instance.Update_text_Double(UI_textbox_Z_Bien_dang, Convert.ToDouble(data_parameter[2][i - 1].ToString()));
                    }
                }
            }
            //load Z4
            data_parameter[3] = new List<double>();
            query = string.Format("SELECT* from Offset_Z_Check_4 where STT='{0}' ", 1);
            cmd = new SQLiteCommand(query, Conn);
            reader = cmd.ExecuteReader();
            reader_ = false;
            if (reader.Read())
            {
                reader_ = true;
            }
            for (int i = 1; i <= 20; i++)
            {
                if (reader_ == true)
                {
                    var Z_Bien_dang = Convert.ToDouble(reader["Z" + i.ToString()]);
                    data_parameter[3].Add(Z_Bien_dang);
                    UITextBox UI_textbox_Z_Bien_dang = this.Controls.Find("txt_Offset_Z4_T" + i, true).FirstOrDefault() as UITextBox;
                    if (UI_textbox_Z_Bien_dang != null)
                    {
                        StatusDisplay.Instance.Update_text_Double(UI_textbox_Z_Bien_dang, Convert.ToDouble(data_parameter[3][i - 1].ToString()));
                    }
                }
            }
        }
        public void load_data_timer_Lamp_tool()
        {
            List<double>[] data_parameter = new List<double>[19];
            data_parameter[0] = new List<double>();
            data_parameter[1] = new List<double>();
            data_parameter[2] = new List<double>();
            data_parameter[3] = new List<double>();
            data_parameter[4] = new List<double>();
            string query = string.Format("SELECT* from Delay_Lamp_Tool_RB where STT='{0}' ", 1);
            SQLiteCommand cmd = new SQLiteCommand(query, Conn);
            var reader = cmd.ExecuteReader();
            bool reader_ = false;
            reader_ = reader.Read() ? true : false;
            for (int i = 1; i <= 4; i++)
            {
                if (reader_ == true)
                {
                    var data_timer0 = Convert.ToInt16(reader["P1_T" + i.ToString()]);
                    data_parameter[0].Add(data_timer0);
                    UITextBox UI_txt_timer0 = this.Controls.Find("txt_Delay_P1_T" + i, true).FirstOrDefault() as UITextBox;
                    if (UI_txt_timer0 != null)
                    {
                        StatusDisplay.Instance.Update_text(UI_txt_timer0, Convert.ToInt16(data_parameter[0][i - 1].ToString()));
                    }
                    //
                    var data_timer1 = Convert.ToInt16(reader["P2_T" + i.ToString()]);
                    data_parameter[1].Add(data_timer1);
                    UITextBox UI_txt_timer1 = this.Controls.Find(" txt_Delay_P2_T" + i, true).FirstOrDefault() as UITextBox;
                    if (UI_txt_timer1 != null)
                    {
                        StatusDisplay.Instance.Update_text(UI_txt_timer1, Convert.ToInt16(data_parameter[1][i - 1].ToString()));
                    }
                    //
                    var data_timer2 = Convert.ToInt16(reader["P3_T" + i.ToString()]);
                    data_parameter[2].Add(data_timer2);
                    UITextBox UI_txt_timer2 = this.Controls.Find(" txt_Delay_P3_T" + i, true).FirstOrDefault() as UITextBox;
                    if (UI_txt_timer2 != null)
                    {
                        StatusDisplay.Instance.Update_text(UI_txt_timer2, Convert.ToInt16(data_parameter[2][i - 1].ToString()));
                    }
                    //
                    var data_timer3 = Convert.ToInt16(reader["P4_T" + i.ToString()]);
                    data_parameter[3].Add(data_timer3);
                    UITextBox UI_txt_timer3 = this.Controls.Find("txt_Delay_P4_T" + i, true).FirstOrDefault() as UITextBox;
                    if (UI_txt_timer3 != null)
                    {
                        StatusDisplay.Instance.Update_text(UI_txt_timer3, Convert.ToInt16(data_parameter[3][i - 1].ToString()));
                    }
                }
            }
            var T_Lamp_White = Convert.ToInt16(reader["Lamp_White"]);
            var T_Lamp_Blue = Convert.ToInt16(reader["Lamp_Blue"]);
            var T_Tha_NG = Convert.ToInt16(reader["Tha_NG"]);
            data_parameter[4].Add(T_Lamp_White);
            data_parameter[4].Add(T_Lamp_Blue);
            data_parameter[4].Add(T_Tha_NG);
            //txt_Delay_Lamp_White.Text = data_parameter[1][16].ToString();
            //txt_Delay_Lamp_Blue.Text = data_parameter[1][17].ToString();
            //txt_Delay_Tha_NG.Text = data_parameter[1][18].ToString();
            //convert toint
            T_P1_T1 = Convert.ToInt16(data_parameter[0][0]);
            T_P1_T2 = Convert.ToInt16(data_parameter[0][1]);
            T_P1_T3 = Convert.ToInt16(data_parameter[0][2]);
            T_P1_T4 = Convert.ToInt16(data_parameter[0][3]);
            //
            T_P2_T1 = Convert.ToInt16(data_parameter[1][0]);
            T_P2_T2 = Convert.ToInt16(data_parameter[1][1]);
            T_P2_T3 = Convert.ToInt16(data_parameter[1][2]);
            T_P2_T4 = Convert.ToInt16(data_parameter[1][3]);
            //
            T_P3_T1 = Convert.ToInt16(data_parameter[2][0]);
            T_P3_T2 = Convert.ToInt16(data_parameter[2][1]);
            T_P3_T3 = Convert.ToInt16(data_parameter[2][2]);
            T_P3_T4 = Convert.ToInt16(data_parameter[2][3]);
            //
            T_P4_T1 = Convert.ToInt16(data_parameter[3][0]);
            T_P4_T2 = Convert.ToInt16(data_parameter[3][1]);
            T_P4_T3 = Convert.ToInt16(data_parameter[3][2]);
            T_P4_T4 = Convert.ToInt16(data_parameter[3][3]);
            //
            T_lamp_White = Convert.ToInt16(data_parameter[4][0]);
            T_Lamp_Blue = Convert.ToInt16(data_parameter[4][1]);
            T_Tha_NG = Convert.ToInt16(data_parameter[4][2]);
        }
        public void load_option_camera()
        {
            #region tam thoi bo qua
            //checkBox_Marking.Checked = PLC1.Read_Data_Bit_("L5");
            //checkBox_Tape.Checked = PLC1.Read_Data_Bit_("L4");
            ////Option Check Vision Bottom
            //checkBox_Connector.Checked = PLC1.Read_Data_Bit_("L" + (11 + Memory_PLC.K100).ToString());
            //checkBox_FPCB.Checked = PLC1.Read_Data_Bit_("L" + (12 + Memory_PLC.K100).ToString());
            //checkBox_Bien_Dang.Checked = PLC1.Read_Data_Bit_("L" + (13 + Memory_PLC.K100).ToString());
            //checkBox_pass_result_cambot_top.Checked = PLC1.Read_Data_Bit_("L" + (93 + Memory_PLC.K100).ToString());
            #endregion
            //checkBox_Use_Cylinder_Tool.Checked = PLC1.Read_Data_Bit_("L460");
            //checkBox_Disable_SS_Up_T1.Checked = PLC1.Read_Data_Bit_("L461");
            //checkBox_Disable_SS_Up_T2.Checked = PLC1.Read_Data_Bit_("L462");
            //checkBox_Disable_SS_Up_T3.Checked = PLC1.Read_Data_Bit_("L463");
            //checkBox_Disable_SS_Up_T4.Checked = PLC1.Read_Data_Bit_("L464");
            //if (checkBox_Use_Cylinder_Tool.Checked == true)
            //{
            //    checkBox_Disable_SS_Up_T1.Enabled = true;
            //    checkBox_Disable_SS_Up_T2.Enabled = true;
            //    checkBox_Disable_SS_Up_T3.Enabled = true;
            //    checkBox_Disable_SS_Up_T4.Enabled = true;
            //}
            //else
            //{
            //    checkBox_Disable_SS_Up_T1.Enabled = false;
            //    checkBox_Disable_SS_Up_T2.Enabled = false;
            //    checkBox_Disable_SS_Up_T3.Enabled = false;
            //    checkBox_Disable_SS_Up_T4.Enabled = false;
            //}
        }
        public void load_Parameter()
        {
            //int i = 1;
            //var data_offset = new List<Tuple<double, double, double>>();
            List<double>[] data_parameter = new List<double>[10];
            for (int i = 1; i < 10; i++)
            {
                data_parameter[i] = new List<double>();

                string query = string.Format("SELECT* from Thong_so_RB where STT='{0}' ", i);

                SQLiteCommand cmd = new SQLiteCommand(query, Conn);
                var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    var SP_auto_rb = Convert.ToInt16(reader["SpeedAuto"]);
                    var delay1 = Convert.ToInt16(reader["Delay1"]);
                    var delay2 = Convert.ToInt16(reader["Delay2"]);
                    var delay3 = Convert.ToInt16(reader["Delay3"]);
                    var delay4 = Convert.ToInt16(reader["Delay4"]);
                    var number_check = Convert.ToInt16(reader["Number_check"]);
                    var Mode_Run_Rb = Convert.ToInt16(reader["Mode_run_RB"]);
                    var Acc_Rb = Convert.ToInt16(reader["ACC_RB"]);
                    var SP_wait_pick = Convert.ToInt16(reader["SP_Wait_Pick"]);
                    var delay_trigger_camB = Convert.ToInt16(reader["Delay_Trigger_CamB"]);
                    var Snap_ = Convert.ToInt16(reader["Snap"]);
                    //
                    data_parameter[i].Add(SP_auto_rb);
                    data_parameter[i].Add(delay1);
                    data_parameter[i].Add(delay2);
                    data_parameter[i].Add(delay3);
                    data_parameter[i].Add(delay4);
                    data_parameter[i].Add(number_check);
                    data_parameter[i].Add(Mode_Run_Rb);
                    data_parameter[i].Add(Acc_Rb);
                    data_parameter[i].Add(SP_wait_pick);
                    data_parameter[i].Add(delay_trigger_camB);
                    data_parameter[i].Add(Snap_);

                }
            }
            StatusDisplay.Instance.Update_text(txt_sp_auto_RB, Convert.ToInt16(data_parameter[1][0]));
            StatusDisplay.Instance.Update_text(txt_delayhut, Convert.ToInt16(data_parameter[1][1]));
            StatusDisplay.Instance.Update_text(txt_delaytha, Convert.ToInt16(data_parameter[1][2]));
            StatusDisplay.Instance.Update_text(txt_delay_up_cylinder, Convert.ToInt16(data_parameter[1][3]));
            StatusDisplay.Instance.Update_text(txt_delay_down_cylinder, Convert.ToInt16(data_parameter[1][4]));
            StatusDisplay.Instance.Update_text(txt_number_tool, Convert.ToInt16(data_parameter[1][5]));
            StatusDisplay.Instance.Update_text(txt_mode_run, Convert.ToInt16(data_parameter[1][6]));
            StatusDisplay.Instance.Update_text(txt_ACC_RB, Convert.ToInt16(data_parameter[1][7]));
            StatusDisplay.Instance.Update_text(txt_SP_Wait_Pick, Convert.ToInt16(data_parameter[1][8]));
            StatusDisplay.Instance.Update_text(txt_trigger_camB, Convert.ToInt16(data_parameter[1][9]));
            StatusDisplay.Instance.Update_text(txt_speed_snap, Convert.ToInt16(data_parameter[1][10]));
            //
            Global.SP_auto_RB = Convert.ToInt16(data_parameter[1][0]);
            Global.delay_hut = Convert.ToInt16(data_parameter[1][1]);
            Global.delay_tha = Convert.ToInt16(data_parameter[1][2]);
            Global.delay_up = Convert.ToInt16(data_parameter[1][3]);
            Global.delay_down = Convert.ToInt16(data_parameter[1][4]);
            Global.Number_Tool = Convert.ToInt16(data_parameter[1][5]);
            Global.Mode_run_auto = Convert.ToInt16(data_parameter[1][6]);
            Global.ACC_RB = Convert.ToInt16(data_parameter[1][7]);
            Global.SP_Wait_Pick = Convert.ToInt16(data_parameter[1][8]);
            Global.TT_CamB = Convert.ToInt16(data_parameter[1][9]);
            Global.SP_snap = Convert.ToInt16(data_parameter[1][10]);
        }
        public void load_Offset()
        {
            //int i = 1;
            //var data_offset = new List<Tuple<double, double, double>>();
            List<double>[] data_offset = new List<double>[50];
            for (int i = 1; i < 2; i++)
            {
                data_offset[i] = new List<double>();

                string query = string.Format("SELECT* from OffsetRB where STT='{0}' ", i);

                SQLiteCommand cmd = new SQLiteCommand(query, Conn);
                var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    var valueX_pos = Math.Round(Convert.ToDouble(reader["X"]), 3);
                    var valueY_pos = Math.Round(Convert.ToDouble(reader["Y"]), 3);
                    var valueZ_pos = Math.Round(Convert.ToDouble(reader["Z"]), 3);
                    var valueZ_antoan = Math.Round(Convert.ToDouble(reader["Z_antoan"]), 3);
                    var valueA2_antoan = Math.Round(Convert.ToDouble(reader["A2_antoan"]), 3);
                    var SP_home = Math.Round(Convert.ToDouble(reader["SP_Home"]), 3);
                    var Z_place_Satefy_ = Math.Round(Convert.ToDouble(reader["Z_Place_Satefy"]), 3);
                    var Y_place_Satefy_L = Math.Round(Convert.ToDouble(reader["Y_Place_Satefy_L"]), 3);
                    var X_place_Satefy_U = Math.Round(Convert.ToDouble(reader["X_Place_Satefy_U"]), 3);
                    var X_place_Satefy_L = Math.Round(Convert.ToDouble(reader["X_Place_Satefy_L"]), 3);
                    var Block_ = Math.Round(Convert.ToDouble(reader["Number_Block"]), 3);
                    var Block_X = Math.Round(Convert.ToDouble(reader["Offset_Block_X"]), 3);
                    var Block_Y = Math.Round(Convert.ToDouble(reader["Offset_Block_Y"]), 3);
                    var FPCB_ = Math.Round(Convert.ToDouble(reader["Number_FPCB"]), 3);
                    var FPCB_X = Math.Round(Convert.ToDouble(reader["Offset_FPCB_X"]), 3);
                    var FPCB_Y = Math.Round(Convert.ToDouble(reader["Offset_FPCB_Y"]), 3);
                    var X_Camtop_Satefy_U = Math.Round(Convert.ToDouble(reader["X_Camtop_Satefy_U"]), 3);
                    var X_Camtop_Satefy_L = Math.Round(Convert.ToDouble(reader["X_Camtop_Satefy_L"]), 3);
                    var Y_Camtop_Satefy_U = Math.Round(Convert.ToDouble(reader["Y_Camtop_Satefy_U"]), 3);
                    var Y_Camtop_Satefy_L = Math.Round(Convert.ToDouble(reader["Y_Camtop_Satefy_L"]), 3);
                    var Z_Camtop_Satefy = Math.Round(Convert.ToDouble(reader["Z_Camtop_Satefy"]), 3);
                    //
                    var Total_Check_Marking = Math.Round(Convert.ToDouble(reader["M_Total_Check"]), 3);
                    var Block_X_Marking = Math.Round(Convert.ToDouble(reader["M_Offset_X_Block"]), 3);
                    var Block_Y_Marking = Math.Round(Convert.ToDouble(reader["M_Offset_Y_Block"]), 3);
                    var FPCB_X_Marking = Math.Round(Convert.ToDouble(reader["M_Offset_X_FPCB"]), 3);
                    var FPCB_Y_Marking = Math.Round(Convert.ToDouble(reader["M_Offset_Y_FPCB"]), 3);


                    data_offset[i].Add(valueX_pos);
                    data_offset[i].Add(valueY_pos);
                    data_offset[i].Add(valueZ_pos);
                    data_offset[i].Add(valueZ_antoan);
                    data_offset[i].Add(valueA2_antoan);
                    data_offset[i].Add(SP_home);
                    data_offset[i].Add(Z_place_Satefy_);
                    data_offset[i].Add(Y_place_Satefy_L);
                    data_offset[i].Add(X_place_Satefy_U);
                    data_offset[i].Add(X_place_Satefy_L);
                    //

                    data_offset[i].Add(Block_);
                    data_offset[i].Add(Block_X);
                    data_offset[i].Add(Block_Y);
                    data_offset[i].Add(FPCB_);
                    data_offset[i].Add(FPCB_X);
                    data_offset[i].Add(FPCB_Y);
                    //
                    data_offset[i].Add(X_Camtop_Satefy_U);
                    data_offset[i].Add(X_Camtop_Satefy_L);
                    data_offset[i].Add(Y_Camtop_Satefy_U);
                    data_offset[i].Add(Y_Camtop_Satefy_L);
                    data_offset[i].Add(Z_Camtop_Satefy);
                    //
                    data_offset[i].Add(Total_Check_Marking);
                    data_offset[i].Add(Block_X_Marking);
                    data_offset[i].Add(Block_Y_Marking);
                    data_offset[i].Add(FPCB_X_Marking);
                    data_offset[i].Add(FPCB_Y_Marking);

                }
            }

            txt_OffsetRB_X.Text = data_offset[1][0].ToString();
            txt_Limit_Y_Place_U.Text = data_offset[1][1].ToString();
            txt_offsetRB_Z.Text = data_offset[1][2].ToString();
            txt_Z_antoan.Text = data_offset[1][3].ToString();
            txt_A2_antoan.Text = data_offset[1][4].ToString();
            txt_Speed_Home_RB.Text = data_offset[1][5].ToString();
            txt_Z_Place_Satefy.Text = data_offset[1][6].ToString();
            txt_Limit_Y_Place_L.Text = data_offset[1][7].ToString();
            txt_Limit_X_Place_U.Text = data_offset[1][8].ToString();
            txt_Limit_X_Place_L.Text = data_offset[1][9].ToString();
            //
            txt_number_Block_Fpcb_RB.Text = data_offset[1][10].ToString();
            Offset_X_BlockFPCB_Tool_tray.Text = data_offset[1][11].ToString();
            Offset_Y_BlockFPCB_Tool_tray.Text = data_offset[1][12].ToString();
            //
            txt_number_Fpcb_Block_RB.Text = data_offset[1][13].ToString();
            Offset_X_FPCB_Tool_tray.Text = data_offset[1][14].ToString();
            Offset_Y_FPCB_Tool_tray.Text = data_offset[1][15].ToString();
            //
            txt_Limit_X_Camtop_U.Text = data_offset[1][16].ToString();
            txt_Limit_X_Camtop_L.Text = data_offset[1][17].ToString();
            txt_Limit_Y_Camtop_U.Text = data_offset[1][18].ToString();
            txt_Limit_Y_Camtop_L.Text = data_offset[1][19].ToString();
            txt_Z_Camtop_Satefy.Text = data_offset[1][20].ToString();
            //
            txt_Total_Check_Marking.Text = data_offset[1][21].ToString();
            Offset_X_BlockFPCB_Tool_Marking.Text = data_offset[1][22].ToString();
            Offset_Y_BlockFPCB_Tool_Marking.Text = data_offset[1][23].ToString();
            Offset_X_FPCB_Tool_Marking.Text = data_offset[1][24].ToString();
            Offset_Y_FPCB_Tool_Marking.Text = data_offset[1][25].ToString();

            //
            Global.Offset_X = data_offset[1][0];
            Global.Y_Place_Satefy_U = data_offset[1][1];
            Global.Offset_Z = data_offset[1][2];
            Global.Z_antoan = data_offset[1][3];
            Global.A2_antoan = data_offset[1][4];
            Global.SP_Home = data_offset[1][5];
            Global.Z_Place_Satefy = data_offset[1][6];
            Global.Y_Place_Satefy_L = data_offset[1][7];
            Global.X_Place_Satefy_U = data_offset[1][8];
            Global.X_Place_Satefy_L = data_offset[1][9];
            //
            Global.Number_Block = data_offset[1][10];
            Global.Offset_Block_X = data_offset[1][11];
            Global.Offset_Block_Y = data_offset[1][12];
            Global.Number_FPCB = data_offset[1][13];
            Global.Offset_FPCB_X = data_offset[1][14];
            Global.Offset_FPCB_Y = data_offset[1][15];
            //
            Global.X_Camtop_Satefy_U = data_offset[1][16];
            Global.X_Camtop_Satefy_L = data_offset[1][17];
            Global.Y_Camtop_Satefy_U = data_offset[1][18];
            Global.Y_Camtop_Satefy_L = data_offset[1][19];
            Global.Z_Camtop_Satefy = data_offset[1][20];
            //          
            Global.Total_Check_Marking = data_offset[1][21];
            Global.Offset_Block_X_marking = data_offset[1][22];
            Global.Offset_Block_Y_Marking = data_offset[1][23];
            Global.Offset_FPCB_X_Marking = data_offset[1][24];
            Global.Offset_FPCB_Y_Marking = data_offset[1][25];

        }
        public void Load_Speed_Robot()
        {
            StatusDisplay.Instance.Get_speed_Robot(TrackBar_Jog_Speed_Robot, 10);
            StatusDisplay.Instance.Update_text(txt_speed_Jog, TrackBar_Jog_Speed_Robot.Value);
        }
        public void load_TactTime()
        {
            List<double>[] data_TactTime = new List<double>[50];
            for (int i = 1; i < 10; i++)
            {
                data_TactTime[i] = new List<double>();

                string query = string.Format("SELECT* from TactTime where STT='{0}' ", i);

                SQLiteCommand cmd = new SQLiteCommand(query, Conn);
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    var valueX_pos = Math.Round(Convert.ToDouble(reader["VALUE"]), 3);
                    data_TactTime[i].Add(valueX_pos);
                }
                UILabel Lab = this.Controls.Find("Tact" + i, true).FirstOrDefault() as UILabel;
                if (Lab != null) { StatusDisplay.Instance.Update_Label(Lab, data_TactTime[i][0]); }
            }
        }
        private void Search_Min_Max_CorRB_PlaceTray()
        {
            #region
            string Max_X = string.Format("SELECT MAX (X) from Matrix_Panel_Tool_1_1  ");
            string Min_X = string.Format("SELECT MIN (X) from Matrix_Panel_Tool_1_1  ");
            string Max_Y = string.Format("SELECT MAX (Y) from Matrix_Panel_Tool_1_1  ");
            string Min_Y = string.Format("SELECT MIN (Y) from Matrix_Panel_Tool_1_1  ");
            string Min_Z = string.Format("SELECT MIN (Z) from Matrix_Panel_Tool_1_1  ");
            //X
            using (SQLiteCommand cmd = new SQLiteCommand(Max_X, Conn))
            {
                object max_X = cmd.ExecuteScalar();
                double maxValue_X = max_X != DBNull.Value ? Convert.ToDouble(max_X) : 0;
                StatusDisplay.Instance.Update_text2(txt_Max_RB_PlaceTray_X, maxValue_X.ToString("F3"));
                Global.Max_RB_PlaceTray_X = maxValue_X;
            }
            using (SQLiteCommand cmd = new SQLiteCommand(Min_X, Conn))
            {
                object min_X = cmd.ExecuteScalar();
                double minValue_X = min_X != DBNull.Value ? Convert.ToDouble(min_X) : 0;
                StatusDisplay.Instance.Update_text2(txt_Min_RB_PlaceTray_X, minValue_X.ToString("F3"));
                Global.Min_RB_PlaceTray_X = minValue_X;
            }
            //Y
            using (SQLiteCommand cmd = new SQLiteCommand(Max_Y, Conn))
            {
                object max_Y = cmd.ExecuteScalar();
                double maxValue_Y = max_Y != DBNull.Value ? Convert.ToDouble(max_Y) : 0;
                StatusDisplay.Instance.Update_text2(txt_Max_RB_PlaceTray_Y, maxValue_Y.ToString("F3"));
                Global.Max_RB_PlaceTray_Y = maxValue_Y;
            }
            using (SQLiteCommand cmd = new SQLiteCommand(Min_Y, Conn))
            {
                object min_Y = cmd.ExecuteScalar();
                double minValue_Y = min_Y != DBNull.Value ? Convert.ToDouble(min_Y) : 0;
                StatusDisplay.Instance.Update_text2(txt_Min_RB_PlaceTray_Y, minValue_Y.ToString("F3"));
                Global.Min_RB_PlaceTray_Y = minValue_Y;
            }
            //Z
            using (SQLiteCommand cmd = new SQLiteCommand(Min_Z, Conn))
            {
                object min_Z = cmd.ExecuteScalar();
                double minValue_Z = min_Z != DBNull.Value ? Convert.ToDouble(min_Z) : 0;
                StatusDisplay.Instance.Update_text2(txt_Min_RB_PlaceTray_Z, minValue_Z.ToString("F3"));
                Global.Min_RB_PlaceTray_Z = minValue_Z;
            }
            #endregion
        }
        private void Search_Min_Max_CorRB_CheckCamTop()
        {
            #region
            string Max_X = string.Format("SELECT MAX (X) from Matrix_Panel_Cam_Top");
            string Min_X = string.Format("SELECT MIN (X) from Matrix_Panel_Cam_Top");
            string Max_Y = string.Format("SELECT MAX (Y) from Matrix_Panel_Cam_Top");
            string Min_Y = string.Format("SELECT MIN (Y) from Matrix_Panel_Cam_Top");
            string Min_Z = string.Format("SELECT MIN (Z) from Matrix_Panel_Cam_Top");
            //X
            using (SQLiteCommand cmd = new SQLiteCommand(Max_X, Conn))
            {
                object max_X = cmd.ExecuteScalar();
                double maxValue_X = max_X != DBNull.Value ? Convert.ToDouble(max_X) : 0;
                StatusDisplay.Instance.Update_text2(txt_Max_RB_CheckCamtop_X, maxValue_X.ToString("F3"));
                Global.Max_RB_CheckCamtop_X = maxValue_X;
            }
            using (SQLiteCommand cmd = new SQLiteCommand(Min_X, Conn))
            {
                object min_X = cmd.ExecuteScalar();
                double minValue_X = min_X != DBNull.Value ? Convert.ToDouble(min_X) : 0;
                StatusDisplay.Instance.Update_text2(txt_Min_RB_CheckCamtop_X, minValue_X.ToString("F3"));
                Global.Min_RB_CheckCamtop_X = minValue_X;
            }
            //Y
            using (SQLiteCommand cmd = new SQLiteCommand(Max_Y, Conn))
            {
                object max_Y = cmd.ExecuteScalar();
                double maxValue_Y = max_Y != DBNull.Value ? Convert.ToDouble(max_Y) : 0;
                StatusDisplay.Instance.Update_text2(txt_Max_RB_CheckCamtop_Y, maxValue_Y.ToString("F3"));
                Global.Max_RB_CheckCamtop_Y = maxValue_Y;

            }
            using (SQLiteCommand cmd = new SQLiteCommand(Min_Y, Conn))
            {
                object min_Y = cmd.ExecuteScalar();
                double minValue_Y = min_Y != DBNull.Value ? Convert.ToDouble(min_Y) : 0;
                StatusDisplay.Instance.Update_text2(txt_Min_RB_CheckCamtop_Y, minValue_Y.ToString("F3"));
                Global.Min_RB_CheckCamtop_Y = minValue_Y;
            }
            //Z
            using (SQLiteCommand cmd = new SQLiteCommand(Min_Z, Conn))
            {
                object min_Z = cmd.ExecuteScalar();
                double minValue_Z = min_Z != DBNull.Value ? Convert.ToDouble(min_Z) : 0;
                StatusDisplay.Instance.Update_text2(txt_Min_RB_CheckCamtop_Z, minValue_Z.ToString("F3"));
                Global.Min_RB_CheckCamtop_Z = minValue_Z;
            }
            #endregion
        }
        public void Search_Min_Max_CorRB()
        {
            Search_Min_Max_CorRB_PlaceTray();
            Search_Min_Max_CorRB_CheckCamTop();
        }
        #endregion
        #region Monitor--------------------------------------------------------------------
        private void Monitor_Current_PLC()
        {
            try
            {
                if (tab_select_all == 2)
                {
                    if (tabControl_PLC_ == 0)
                    {
                        if (tabremov_PLC == 0)
                        {
                            // manual Remov Tape A7
                            for (int i = 1; i <= 6; i++)
                            {
                                UIButton ICO_btn1 = this.Controls.Find("G1_A7_btn_P" + i, true).FirstOrDefault() as UIButton;
                                UIButton ICO_btn2 = this.Controls.Find("G1_A8_btn_P" + i, true).FirstOrDefault() as UIButton;
                                if (ICO_btn1 != null) { StatusDisplay.Instance.STT_Button_Display_SV(ICO_btn1, PLC1.Read_Data_Bit_("M" + (5717 + Memory_PLC.K800 + ((i - 1) * 5)).ToString())); }
                                if (ICO_btn2 != null) { StatusDisplay.Instance.STT_Button_Display_SV(ICO_btn2, PLC1.Read_Data_Bit_("M" + (5817 + Memory_PLC.K800 + ((i - 1) * 5)).ToString())); }
                            }
                            StatusDisplay.Instance.STT_Button_Display_SV(G1_M_1, PLC1.Read_Data_Bit_("M" + (6717 + Memory_PLC.K100).ToString()));
                            StatusDisplay.Instance.STT_Button_Display_SV(G1_M_2, PLC1.Read_Data_Bit_("M" + (6722 + Memory_PLC.K100).ToString()));
                            //cylinder
                            StatusDisplay.Instance.STT_Button_Display_Cylinder2(G1_C2, G1_C1, PLC1.Read_Data_Word_("D" + (7006 + Memory_PLC.K100).ToString()));
                            StatusDisplay.Instance.STT_Button_Display_Cylinder2(G1_C4, G1_C3, PLC1.Read_Data_Word_("D" + (7007 + Memory_PLC.K100).ToString()));
                            StatusDisplay.Instance.STT_Button_Display_Cylinder2(G1_C7, G1_C8, PLC1.Read_Data_Word_("D" + (7009 + Memory_PLC.K100).ToString()));
                            StatusDisplay.Instance.STT_Button_Display_Cylinder2(G1_C9, G1_C10, PLC1.Read_Data_Word_("D" + (7017 + Memory_PLC.K100).ToString()));
                            StatusDisplay.Instance.STT_Button_Display_Cylinder1(G1_M_4, G1_M_3, PLC1.Read_Data_Word_("D" + (7015 + Memory_PLC.K100).ToString()));
                            //ss
                            for (int j = 1; j < 6; j++)
                            {
                                Label lab = this.Controls.Find("G1_SS_" + j, true).FirstOrDefault() as Label;
                                if (lab != null) { StatusDisplay.Instance.STT_Sensor(lab, PLC1.Read_Data_Bit_("M" + (8517 + Memory_PLC.K300 + j - 1).ToString())); }
                            }
                        }
                        else if (tabremov_PLC == 1)
                        {
                            for (int i = 7; i < 10; i++)
                            {
                                UIButton btn1 = this.Controls.Find("btn_T_1_A" + i, true).FirstOrDefault() as UIButton;
                                UIButton btn2 = this.Controls.Find("btn_T_2_A" + i, true).FirstOrDefault() as UIButton;
                                UITextBox txt2 = this.Controls.Find("txt_T_1_A" + i, true).FirstOrDefault() as UITextBox;
                                UITextBox txt3 = this.Controls.Find("txt_T2_A" + i, true).FirstOrDefault() as UITextBox;

                                if (i < 9)
                                {
                                    if (btn1 != null) { StatusDisplay.Instance.STT_Button_Display_SV(btn1, PLC1.Read_Data_Bit_("M" + (5704 + Memory_PLC.K800 + ((i - 7) * 100)).ToString())); }
                                    if (btn2 != null) { StatusDisplay.Instance.STT_Button_Display_SV(btn2, PLC1.Read_Data_Bit_("M" + (5709 + Memory_PLC.K800 + ((i - 7) * 100)).ToString())); }
                                    if (txt2 != null) { StatusDisplay.Instance.Update_text(txt2, PLC1.Read_Data_Word_("D" + (5722 + Memory_PLC.K800 + ((i - 7) * 100)).ToString())); }
                                    if (txt3 != null) { StatusDisplay.Instance.Update_text(txt3, PLC1.Read_Data_DWord_("D" + (5730 + Memory_PLC.K800 + ((i - 7) * 100)).ToString())); }

                                }
                                else
                                {
                                    if (btn1 != null) { StatusDisplay.Instance.STT_Button_Display_SV(btn1, PLC1.Read_Data_Bit_("M" + (6704 + Memory_PLC.K100).ToString())); }
                                    if (btn2 != null) { StatusDisplay.Instance.STT_Button_Display_SV(btn2, PLC1.Read_Data_Bit_("M" + (6700 + Memory_PLC.K100).ToString())); }
                                    if (txt2 != null) { StatusDisplay.Instance.Update_text(txt2, PLC1.Read_Data_Word_("D" + (6722 + Memory_PLC.K100).ToString())); }
                                    if (txt3 != null) { StatusDisplay.Instance.Update_text(txt3, PLC1.Read_Data_DWord_("D" + (6730 + Memory_PLC.K100).ToString())); }
                                }
                            }
                            // speed jog
                            if (A0JA7.Text != "")
                            {
                                PLC1.Write_Data_DWord_("D" + (5708 + Memory_PLC.K800).ToString(), Convert.ToInt32(A0JA7.Text));
                            }
                            if (A0JA8.Text != "")
                            {
                                PLC1.Write_Data_DWord_("D" + (5808 + Memory_PLC.K800).ToString(), Convert.ToInt32(A0JA8.Text));
                            }
                            if (A1JA9.Text != "")
                            {
                                PLC1.Write_Data_DWord_("D" + (6708 + Memory_PLC.K100).ToString(), Convert.ToInt32(A1JA9.Text));
                            }
                            //limit A7
                            StatusDisplay.Instance.STT_Sensor2(G1_T_7, PLC1.Read_Data_Bit_("M" + (8538 + Memory_PLC.K300).ToString()));
                            StatusDisplay.Instance.STT_Sensor2(G1_T_8, PLC1.Read_Data_Bit_("M" + (8539 + Memory_PLC.K300).ToString()));
                            StatusDisplay.Instance.STT_Sensor2(G1_T_9, PLC1.Read_Data_Bit_("M" + (8540 + Memory_PLC.K300).ToString()));
                            //limit A8
                            StatusDisplay.Instance.STT_Sensor2(G1_T_10, PLC1.Read_Data_Bit_("M" + (5807 + Memory_PLC.K800).ToString()));
                            StatusDisplay.Instance.STT_Sensor2(G1_T_11, PLC1.Read_Data_Bit_("M" + (5808 + Memory_PLC.K800).ToString()));
                            StatusDisplay.Instance.STT_Sensor2(G1_T_12, PLC1.Read_Data_Bit_("M" + (5806 + Memory_PLC.K800).ToString()));
                            //limit A9                                                                                     
                            StatusDisplay.Instance.STT_Sensor2(G1_T_13, PLC1.Read_Data_Bit_("M" + (6707 + Memory_PLC.K100).ToString()));
                            StatusDisplay.Instance.STT_Sensor2(G1_T_14, PLC1.Read_Data_Bit_("M" + (6708 + Memory_PLC.K100).ToString()));
                            StatusDisplay.Instance.STT_Sensor2(G1_T_15, PLC1.Read_Data_Bit_("M" + (6706 + Memory_PLC.K100).ToString()));
                        }
                    }
                    else if (tabControl_PLC_ == 1)
                    {
                        if (tabtray_PLC == 0)
                        {
                            //Manual A1
                            StatusDisplay.Instance.STT_Button_Display_SV(G2_M_0, PLC1.Read_Data_Bit_("M" + (5117 + Memory_PLC.K800).ToString()));
                            StatusDisplay.Instance.STT_Button_Display_SV(G2_M_1, PLC1.Read_Data_Bit_("M" + (5122 + Memory_PLC.K800).ToString()));
                            //Manual A2
                            StatusDisplay.Instance.STT_Button_Display_SV(G2_M_2, PLC1.Read_Data_Bit_("M" + (5217 + Memory_PLC.K800).ToString()));
                            StatusDisplay.Instance.STT_Button_Display_SV(G2_M_3, PLC1.Read_Data_Bit_("M" + (5222 + Memory_PLC.K800).ToString()));
                            // Manual Input tray
                            StatusDisplay.Instance.STT_Button_Display_Cylinder1(G2_M_4, G2_M_5, PLC1.Read_Data_Word_("D" + (7000 + Memory_PLC.K100).ToString()));
                            // Manual Output tray
                            StatusDisplay.Instance.STT_Button_Display_Cylinder1(G2_M_8, G2_M_9, PLC1.Read_Data_Word_("D" + (7001 + Memory_PLC.K100).ToString()));
                            // Manual Transfer tray
                            StatusDisplay.Instance.STT_Button_Display_Cylinder1(G2_M_12, G2_M_13, PLC1.Read_Data_Word_("D" + (7002 + Memory_PLC.K100).ToString()));
                            // Manual Pick tray
                            StatusDisplay.Instance.STT_Button_Display_Cylinder1(G2_M_15, G2_M_14, PLC1.Read_Data_Word_("D" + (7005 + Memory_PLC.K100).ToString()));
                            // Manual Align tray
                            StatusDisplay.Instance.STT_Button_Display_Cylinder1(G2_M_7, G2_M_6, PLC1.Read_Data_Word_("D" + (7003 + Memory_PLC.K100).ToString()));
                            StatusDisplay.Instance.STT_Button_Display_Cylinder1(G2_M_11, G2_M_10, PLC1.Read_Data_Word_("D" + (7004 + Memory_PLC.K100).ToString()));
                            // Manual Vaccum tray
                            StatusDisplay.Instance.STT_Button_Display_Cylinder1(G2_M_17, G2_M_16, PLC1.Read_Data_Word_("D" + (7013 + Memory_PLC.K100).ToString()));
                            // Manual Box NG
                            StatusDisplay.Instance.STT_Button_Display_Cylinder1(BoxNG_For, BoxNG_Back, SDKHrobot.HRobot.get_digital_output(handle, 56));
                            //sensor cylinder
                            for (int i = 1; i < 11; i++)
                            {
                                Label lab = this.Controls.Find("G2_SS_" + i, true).FirstOrDefault() as Label;
                                if (lab != null) { StatusDisplay.Instance.STT_Sensor(lab, PLC1.Read_Data_Bit_("M" + (8507 + Memory_PLC.K300 + i - 1).ToString())); }
                            }
                            //ss vaccum
                            StatusDisplay.Instance.STT_Sensor(G2_M_28, PLC1.Read_Data_Bit_("M" + (8526 + Memory_PLC.K300).ToString()));
                            //ss box NG
                            StatusDisplay.Instance.STT_Sensor1(SS_DI12, SDKHrobot.HRobot.get_digital_input(handle, 12));
                            StatusDisplay.Instance.STT_Sensor1(SS_DI13, SDKHrobot.HRobot.get_digital_input(handle, 13));
                        }
                        else if (tabtray_PLC == 1)
                        {
                            //current axis 1-2
                            StatusDisplay.Instance.Update_text(G2_Text_2, PLC1.Read_Data_DWord_("D" + (5130 + Memory_PLC.K800).ToString()));
                            StatusDisplay.Instance.Update_text(G2_Text_3, PLC1.Read_Data_DWord_("D" + (5230 + Memory_PLC.K800).ToString()));
                            //Alarm Code Axis 1-2
                            StatusDisplay.Instance.Update_text(G2_Text_0, PLC1.Read_Data_Word_("D" + (5122 + Memory_PLC.K800).ToString()));
                            StatusDisplay.Instance.Update_text(G2_Text_1, PLC1.Read_Data_Word_("D" + (5222 + Memory_PLC.K800).ToString()));
                            //Status Servo On
                            StatusDisplay.Instance.STT_Button_Display_SV(G2_T_0, PLC1.Read_Data_Bit_("M" + (5104 + Memory_PLC.K800).ToString()));
                            StatusDisplay.Instance.STT_Button_Display_SV(G2_T_2, PLC1.Read_Data_Bit_("M" + (5204 + Memory_PLC.K800).ToString()));
                            //Status Home
                            StatusDisplay.Instance.STT_Button_Display_SV(G2_T_1, PLC1.Read_Data_Bit_("M" + (5109 + Memory_PLC.K800).ToString()));
                            StatusDisplay.Instance.STT_Button_Display_SV(G2_T_3, PLC1.Read_Data_Bit_("M" + (5209 + Memory_PLC.K800).ToString()));
                            //Speed Jog Axis 1-2
                            if (A0JA1.Text != "")
                            {
                                PLC1.Write_Data_DWord_("D" + (5108 + Memory_PLC.K800).ToString(), Convert.ToInt32(A0JA1.Text));
                            }
                            if (A0JA2.Text != "")
                            {
                                PLC1.Write_Data_DWord_("D" + (5208 + Memory_PLC.K800).ToString(), Convert.ToInt32(A0JA2.Text));
                            }
                            //limit A7
                            StatusDisplay.Instance.STT_Sensor2(G2_T_4, PLC1.Read_Data_Bit_("M" + (5107 + Memory_PLC.K800).ToString()));
                            StatusDisplay.Instance.STT_Sensor2(G2_T_5, PLC1.Read_Data_Bit_("M" + (5108 + Memory_PLC.K800).ToString()));
                            StatusDisplay.Instance.STT_Sensor2(G2_T_6, PLC1.Read_Data_Bit_("M" + (5106 + Memory_PLC.K800).ToString()));
                            //limit A8
                            StatusDisplay.Instance.STT_Sensor2(G2_T_7, PLC1.Read_Data_Bit_("M" + (5207 + Memory_PLC.K800).ToString()));
                            StatusDisplay.Instance.STT_Sensor2(G2_T_8, PLC1.Read_Data_Bit_("M" + (5208 + Memory_PLC.K800).ToString()));
                            StatusDisplay.Instance.STT_Sensor2(G2_T_9, PLC1.Read_Data_Bit_("M" + (5206 + Memory_PLC.K800).ToString()));
                            //current                          
                        }
                    }
                    else if (tabControl_PLC_ == 2)
                    {
                        if (tabvision_PLC == 0)
                        {
                            for (int i = 1; i < 3; i++)
                            {
                                UIButton btn1 = this.Controls.Find("G3_A3_btn_P" + i, true).FirstOrDefault() as UIButton;
                                UIButton btn2 = this.Controls.Find("G3_A4_btn_P" + i, true).FirstOrDefault() as UIButton;
                                UIButton btn3 = this.Controls.Find("G3_A5_btn_P" + i, true).FirstOrDefault() as UIButton;
                                UIButton btn4 = this.Controls.Find("G3_A6_btn_P" + i, true).FirstOrDefault() as UIButton;

                                if (btn1 != null) { StatusDisplay.Instance.STT_Button_Display_SV(btn1, PLC1.Read_Data_Bit_("M" + (5317 + ((i - 1) * 5) + Memory_PLC.K800).ToString())); }
                                if (btn2 != null) { StatusDisplay.Instance.STT_Button_Display_SV(btn2, PLC1.Read_Data_Bit_("M" + (5417 + ((i - 1) * 5) + Memory_PLC.K800).ToString())); }
                                if (btn3 != null) { StatusDisplay.Instance.STT_Button_Display_SV(btn3, PLC1.Read_Data_Bit_("M" + (5517 + ((i - 1) * 5) + Memory_PLC.K800).ToString())); }
                                if (btn4 != null) { StatusDisplay.Instance.STT_Button_Display_SV(btn4, PLC1.Read_Data_Bit_("M" + (5617 + ((i - 1) * 5) + Memory_PLC.K800).ToString())); }
                            }
                            for (int i = 3; i < 7; i++)
                            {
                                UIButton btn1 = this.Controls.Find("G3_A3_btn_P" + i, true).FirstOrDefault() as UIButton;
                                UIButton btn2 = this.Controls.Find("G3_A4_btn_P" + i, true).FirstOrDefault() as UIButton;
                                UIButton btn3 = this.Controls.Find("G3_A5_btn_P" + i, true).FirstOrDefault() as UIButton;
                                UIButton btn4 = this.Controls.Find("G3_A6_btn_P" + i, true).FirstOrDefault() as UIButton;

                                if (btn1 != null) { StatusDisplay.Instance.STT_Button_Display_SV(btn1, PLC1.Read_Data_Bit_("M" + (5332 + ((i - 3) * 5) + Memory_PLC.K800).ToString())); }
                                if (btn2 != null) { StatusDisplay.Instance.STT_Button_Display_SV(btn2, PLC1.Read_Data_Bit_("M" + (5432 + ((i - 3) * 5) + Memory_PLC.K800).ToString())); }
                                if (btn3 != null) { StatusDisplay.Instance.STT_Button_Display_SV(btn3, PLC1.Read_Data_Bit_("M" + (5532 + ((i - 3) * 5) + Memory_PLC.K800).ToString())); }
                                if (btn4 != null) { StatusDisplay.Instance.STT_Button_Display_SV(btn4, PLC1.Read_Data_Bit_("M" + (5632 + ((i - 3) * 5) + Memory_PLC.K800).ToString())); }
                            }
                            //cylinder lamp
                            StatusDisplay.Instance.STT_Button_Display_Cylinder1(G3_M_29, G3_M_30, PLC1.Read_Data_Word_("D" + (7010 + Memory_PLC.K100).ToString()));
                            //Group camera
                            StatusDisplay.Instance.STT_Button_Display_Cylinder1(G3_C5, G3_C6, PLC1.Read_Data_Word_("D" + (7011 + Memory_PLC.K100).ToString()));
                            // vaccum
                            StatusDisplay.Instance.STT_Button_Display_Cylinder1(G3_M_27, G3_M_26, PLC1.Read_Data_Word_("D" + (7014 + Memory_PLC.K100).ToString()));
                            //sensor cylinder
                            for (int i = 1; i < 5; i++)
                            {
                                Label lab = this.Controls.Find("G3_SS" + i, true).FirstOrDefault() as Label;
                                if (lab != null) { StatusDisplay.Instance.STT_Sensor(lab, PLC1.Read_Data_Bit_("M" + (8522 + Memory_PLC.K300 + i - 1).ToString())); }
                            }
                            StatusDisplay.Instance.STT_Sensor(G3_M_28, PLC1.Read_Data_Bit_("M" + (8528 + Memory_PLC.K300).ToString()));
                        }
                        else if (tabvision_PLC == 1)
                        {
                            for (int i = 3; i < 7; i++)
                            {
                                UIButton btn1 = this.Controls.Find("G3_T1_A" + i, true).FirstOrDefault() as UIButton;
                                UIButton btn2 = this.Controls.Find("G3_T2_A" + i, true).FirstOrDefault() as UIButton;
                                UITextBox txt1 = this.Controls.Find("txt_T1_A" + i, true).FirstOrDefault() as UITextBox;
                                UITextBox txt2 = this.Controls.Find("txt_T2_A" + i, true).FirstOrDefault() as UITextBox;
                                UITextBox txt3 = this.Controls.Find("A0JA" + i, true).FirstOrDefault() as UITextBox;
                                if (btn1 != null) { StatusDisplay.Instance.STT_Button_Display_SV(btn1, PLC1.Read_Data_Bit_("M" + (5304 + ((i - 3) * 100) + Memory_PLC.K800).ToString())); }
                                if (btn2 != null) { StatusDisplay.Instance.STT_Button_Display_SV(btn2, PLC1.Read_Data_Bit_("M" + (5309 + ((i - 3) * 100) + Memory_PLC.K800).ToString())); }
                                if (txt1 != null) { StatusDisplay.Instance.Update_text(txt1, PLC1.Read_Data_Word_("D" + (5322 + ((i - 3) * 100) + Memory_PLC.K800).ToString())); }
                                if (txt2 != null) { StatusDisplay.Instance.Update_text(txt2, PLC1.Read_Data_DWord_("D" + (5330 + ((i - 3) * 100) + Memory_PLC.K800).ToString())); }
                                if (txt3 != null && txt3.Text != "") { PLC1.Write_Data_DWord_("D" + (5308 + ((i - 3) * 100) + Memory_PLC.K800).ToString(), Convert.ToInt32(txt3.Text)); }
                            }
                            //limit A3
                            StatusDisplay.Instance.STT_Sensor2(G3_T_8, PLC1.Read_Data_Bit_("M" + (5307 + Memory_PLC.K800).ToString()));
                            StatusDisplay.Instance.STT_Sensor2(G3_T_9, PLC1.Read_Data_Bit_("M" + (5308 + Memory_PLC.K800).ToString()));
                            StatusDisplay.Instance.STT_Sensor2(G3_T_10, PLC1.Read_Data_Bit_("M" + (5306 + Memory_PLC.K800).ToString()));
                            //limit A4                                                                                        
                            StatusDisplay.Instance.STT_Sensor2(G3_T_11, PLC1.Read_Data_Bit_("M" + (5407 + Memory_PLC.K800).ToString()));
                            StatusDisplay.Instance.STT_Sensor2(G3_T_12, PLC1.Read_Data_Bit_("M" + (5408 + Memory_PLC.K800).ToString()));
                            StatusDisplay.Instance.STT_Sensor2(G3_T_13, PLC1.Read_Data_Bit_("M" + (5406 + Memory_PLC.K800).ToString()));
                            //limit A5                                                                                        
                            StatusDisplay.Instance.STT_Sensor2(G3_T_14, PLC1.Read_Data_Bit_("M" + (5507 + Memory_PLC.K800).ToString()));
                            StatusDisplay.Instance.STT_Sensor2(G3_T_15, PLC1.Read_Data_Bit_("M" + (5508 + Memory_PLC.K800).ToString()));
                            StatusDisplay.Instance.STT_Sensor2(G3_T_16, PLC1.Read_Data_Bit_("M" + (5506 + Memory_PLC.K800).ToString()));
                            //limit A6                                                                                        
                            StatusDisplay.Instance.STT_Sensor2(G3_T_17, PLC1.Read_Data_Bit_("M" + (5607 + Memory_PLC.K800).ToString()));
                            StatusDisplay.Instance.STT_Sensor2(G3_T_18, PLC1.Read_Data_Bit_("M" + (5608 + Memory_PLC.K800).ToString()));
                            StatusDisplay.Instance.STT_Sensor2(G3_T_19, PLC1.Read_Data_Bit_("M" + (5606 + Memory_PLC.K800).ToString()));
                            //current
                            //StatusDisplay.Instance.Update_text(txt_T2_A3, PLC1.Read_Data_DWord_("D" + (5330 + Memory_PLC.K800).ToString()));
                            //StatusDisplay.Instance.Update_text(txt_T2_A4, PLC1.Read_Data_DWord_("D" + (5430 + Memory_PLC.K800).ToString()));
                            //StatusDisplay.Instance.Update_text(txt_T2_A5, PLC1.Read_Data_DWord_("D" + (5530 + Memory_PLC.K800).ToString()));
                            //StatusDisplay.Instance.Update_text(txt_T2_A6, PLC1.Read_Data_DWord_("D" + (5630 + Memory_PLC.K800).ToString()));
                        }
                    }
                    else if (tabControl_PLC_ == 5)
                    {
                        //io 1
                        if (tabIO_input_PLC == 0)
                        {
                            for (int i = 0; i < 32; i++)
                            {
                                UILight lamp_input = this.Controls.Find("I" + i, true).FirstOrDefault() as UILight;
                                if (lamp_input != null) { StatusDisplay.Instance.IO_light(lamp_input, PLC1.Read_Data_Bit_("M" + (8500 + i + Memory_PLC.K300).ToString())); }
                            }
                        }
                        //io2
                        else if (tabIO_input_PLC == 1)
                        {
                            for (int j = 32; j < 64; j++)
                            {
                                UILight lamp_input = this.Controls.Find("I" + j, true).FirstOrDefault() as UILight;
                                if (lamp_input != null) { StatusDisplay.Instance.IO_light(lamp_input, PLC1.Read_Data_Bit_("M" + (8500 + j + Memory_PLC.K300).ToString())); }
                            }
                        }
                    }
                    else if (tabControl_PLC_ == 6)
                    {
                        //io 1
                        if (tabIO_input_PLC == 0)
                        {
                            for (int i = 0; i < 32; i++)
                            {
                                UILight lamp_Output1 = this.Controls.Find("Out" + i, true).FirstOrDefault() as UILight;
                                if (lamp_Output1 != null) { StatusDisplay.Instance.IO_light(lamp_Output1, PLC1.Read_Data_Bit_("M" + (8564 + i + Memory_PLC.K300).ToString())); }
                            }
                        }
                        //io2
                        else if (tabIO_input_PLC == 1)
                        {
                            for (int j = 32; j < 64; j++)
                            {
                                UILight lamp_Output2 = this.Controls.Find("Out" + j, true).FirstOrDefault() as UILight;
                                if (lamp_Output2 != null) { StatusDisplay.Instance.IO_light(lamp_Output2, PLC1.Read_Data_Bit_("M" + (8564 + j + Memory_PLC.K300).ToString())); }
                            }
                        }
                        //io2
                        else if (tabIO_input_PLC == 2)
                        {
                            for (int k = 64; k < 96; k++)
                            {
                                UILight lamp_Output3 = this.Controls.Find("Out" + k, true).FirstOrDefault() as UILight;
                                if (lamp_Output3 != null) { StatusDisplay.Instance.IO_light(lamp_Output3, PLC1.Read_Data_Bit_("M" + (8564 + k + Memory_PLC.K300).ToString())); }
                            }
                        }
                    }
                }
            }
            catch { };
        }
        private async Task Read_PLC()
        {
            await Task.Delay(30);
            try
            {
                if (tab_select_all == 2)
                {
                    if (tabControl_PLC_ == 0)
                    {
                        if (tabremov_PLC == 0)
                        {
                            // Manual G1
                            Thread.Sleep(delaypage);
                            VaribalePLC.Group1 = PLC1.ReadRandomBit("M" + (14000 + Memory_PLC.K1000).ToString(), 6);
                        }
                        else if (tabremov_PLC == 1)
                        {
                            Thread.Sleep(delaypage);
                            VaribalePLC.Group12 = PLC1.ReadRandomBit("M" + (14050 + Memory_PLC.K1000).ToString(), 15);
                            VaribalePLC.Group1_text = PLC1.Read_Word_Arr("D" + (2000 + Memory_PLC.K500).ToString(), 3);
                            VaribalePLC.Group1_Dtext = PLC1.read_DWord_AR("D" + (2004 + Memory_PLC.K500).ToString(), 3);
                        }
                    }
                    else if (tabControl_PLC_ == 1)
                    {
                        if (tabtray_PLC == 0)
                        {
                            Thread.Sleep(delaypage);
                            //Manual
                            VaribalePLC.Group2 = PLC1.ReadRandomBit("M" + (14100 + Memory_PLC.K1000).ToString(), 29);

                        }
                        else if (tabtray_PLC == 1)
                        {
                            Thread.Sleep(delaypage);
                            VaribalePLC.Group22 = PLC1.ReadRandomBit("M" + (14150 + Memory_PLC.K1000).ToString(), 10);
                            VaribalePLC.Group2_text = PLC1.Read_Word_Arr("D" + (2020 + Memory_PLC.K500).ToString(), 2);
                            VaribalePLC.Group2_Dtext = PLC1.read_DWord_AR("D" + (2022 + Memory_PLC.K500).ToString(), 2);
                        }
                    }
                    else if (tabControl_PLC_ == 2)
                    {
                        if (tabvision_PLC == 0)
                        {
                            Thread.Sleep(delaypage);
                            //Manual
                            VaribalePLC.Group3 = PLC1.ReadRandomBit("M" + (14200 + Memory_PLC.K1000).ToString(), 38);
                        }
                        else if (tabvision_PLC == 1)
                        {
                            Thread.Sleep(delaypage);
                            VaribalePLC.Group32 = PLC1.ReadRandomBit("M" + (14250 + Memory_PLC.K1000).ToString(), 20);
                            VaribalePLC.Group3_text = PLC1.Read_Word_Arr("D" + (2040 + Memory_PLC.K500).ToString(), 4);
                            VaribalePLC.Group3_Dtext = PLC1.read_DWord_AR("D" + (2044 + Memory_PLC.K500).ToString(), 4);
                        }
                    }
                    else if (tabControl_PLC_ == 5)
                    {
                        //io 1
                        if (tabIO_input_PLC == 0)
                        {
                            Thread.Sleep(delaypage);
                            VaribalePLC.M_Input1 = PLC1.ReadRandomBit("M" + (8500 + Memory_PLC.K300).ToString(), 32);
                        }
                        //io2
                        else if (tabIO_input_PLC == 1)
                        {
                            Thread.Sleep(delaypage);
                            VaribalePLC.M_Input2 = PLC1.ReadRandomBit("M" + (8532 + Memory_PLC.K300).ToString(), 32);
                        }
                        else if (tabIO_input_PLC == 2)
                        {
                            Thread.Sleep(delaypage);
                            VaribalePLC.M_Input3 = PLC1.ReadRandomBit("M" + (8660 + Memory_PLC.K300).ToString(), 10);
                        }
                    }
                    else if (tabControl_PLC_ == 6)
                    {
                        //io 1
                        if (tabIO_output_PLC == 0)
                        {
                            Thread.Sleep(delaypage);
                            VaribalePLC.M_Output1 = PLC1.ReadRandomBit("M" + (8564 + Memory_PLC.K300).ToString(), 32);
                        }
                        //io2
                        else if (tabIO_output_PLC == 1)
                        {
                            Thread.Sleep(delaypage);
                            VaribalePLC.M_Output2 = PLC1.ReadRandomBit("M" + (8564 + 32 + Memory_PLC.K300).ToString(), 32);
                        }
                        //io2
                        else if (tabIO_output_PLC == 2)
                        {
                            Thread.Sleep(delaypage);
                            VaribalePLC.M_Output3 = PLC1.ReadRandomBit("M" + (8564 + 64 + Memory_PLC.K300).ToString(), 32);
                        }
                    }
                }
            }
            catch { };
        }
        private async Task Monitor_Current_PLC2()
        {
            await Task.Delay(30);
            try
            {
                if (tab_select_all == 2)
                {
                    if (tabControl_PLC_ == 0)
                    {
                        if (tabremov_PLC == 0)
                        {
                            // manual Remov Tape A7
                            for (int ni = 1; ni < 5; ni++)
                            {
                                UIButton ICO_btn1 = this.Controls.Find("G1_M_" + ni, true).FirstOrDefault() as UIButton;
                                if (ICO_btn1 != null) { StatusDisplay.Instance.STT_Button_Display_SV1(ICO_btn1, VaribalePLC.Group1[ni - 1]); }
                            }
                        }
                        else if (tabremov_PLC == 1)
                        {
                            for (int i = 1; i < 16; i++)
                            {
                                if (i < 7)
                                {
                                    UIButton btn1 = this.Controls.Find("G1_T_" + i, true).FirstOrDefault() as UIButton;
                                    if (btn1 != null) { StatusDisplay.Instance.STT_Button_Display_SV1(btn1, VaribalePLC.Group12[i - 1]); }
                                }
                                else
                                {
                                    UISymbolLabel sybl = this.Controls.Find("G1_T_" + i, true).FirstOrDefault() as UISymbolLabel;
                                    if (sybl != null) { StatusDisplay.Instance.STT_SybolUILabel1(sybl, VaribalePLC.Group12[i - 1]); }
                                }
                            }
                            for (int j = 1; j < 7; j++)
                            {
                                UITextBox txt2 = this.Controls.Find("G1_Text_" + j, true).FirstOrDefault() as UITextBox;
                                if (j < 4)
                                {
                                    if (txt2 != null) { StatusDisplay.Instance.Update_text(txt2, VaribalePLC.Group1_text[j - 1]); }
                                }
                                else
                                {
                                    if (txt2 != null) { StatusDisplay.Instance.Update_text3(txt2, VaribalePLC.Group1_Dtext[j - 4]); }
                                }
                            }
                            // speed jog
                            if (A0JA7.Text != "")
                            {
                                PLC1.Write_Data_DWord_("D" + (5708 + Memory_PLC.K800).ToString(), Convert.ToInt32(A0JA7.Text));
                            }
                            if (A0JA8.Text != "")
                            {
                                PLC1.Write_Data_DWord_("D" + (5808 + Memory_PLC.K800).ToString(), Convert.ToInt32(A0JA8.Text));
                            }
                            if (A1JA9.Text != "")
                            {
                                PLC1.Write_Data_DWord_("D" + (6708 + Memory_PLC.K100).ToString(), Convert.ToInt32(A1JA9.Text));
                            }
                        }
                    }
                    else if (tabControl_PLC_ == 1)
                    {
                        if (tabtray_PLC == 0)
                        {
                            for (int i = 0; i < 29; i++)
                            {
                                if (i < 18)
                                {
                                    UIButton btn = this.Controls.Find("G2_M_" + i, true).FirstOrDefault() as UIButton;
                                    if (btn != null) { StatusDisplay.Instance.STT_Button_Display_SV1(btn, VaribalePLC.Group2[i]); }
                                }
                                else
                                {
                                    Label lab = this.Controls.Find("G2_M_" + i, true).FirstOrDefault() as Label;
                                    if (lab != null) { StatusDisplay.Instance.STT_Sensor1(lab, VaribalePLC.Group2[i]); }
                                }
                            }
                            //ss box NG
                            StatusDisplay.Instance.STT_Button_Display_Cylinder2(BoxNG_For, BoxNG_Back, SDKHrobot.HRobot.get_digital_output(handle, 56));
                            StatusDisplay.Instance.STT_Sensor1(SS_DI12, SDKHrobot.HRobot.get_digital_input(handle, 12));
                            StatusDisplay.Instance.STT_Sensor1(SS_DI13, SDKHrobot.HRobot.get_digital_input(handle, 13));
                        }
                        else if (tabtray_PLC == 1)
                        {
                            for (int i = 0; i < 10; i++)
                            {
                                if (i < 4)
                                {
                                    UIButton btn1 = this.Controls.Find("G2_T_" + i, true).FirstOrDefault() as UIButton;
                                    if (btn1 != null) { StatusDisplay.Instance.STT_Button_Display_SV1(btn1, VaribalePLC.Group22[i]); }
                                }
                                else
                                {
                                    UISymbolLabel sybl = this.Controls.Find("G2_T_" + i, true).FirstOrDefault() as UISymbolLabel;
                                    if (sybl != null) { StatusDisplay.Instance.STT_SybolUILabel1(sybl, VaribalePLC.Group22[i]); }
                                }

                            }
                            for (int j = 0; j < 4; j++)
                            {
                                UITextBox txt2 = this.Controls.Find("G2_Text_" + j, true).FirstOrDefault() as UITextBox;
                                if (j < 2)
                                {
                                    if (txt2 != null) { StatusDisplay.Instance.Update_text(txt2, VaribalePLC.Group2_text[j]); }
                                }
                                else
                                {
                                    if (txt2 != null) { StatusDisplay.Instance.Update_text3(txt2, VaribalePLC.Group2_Dtext[j - 2]); }
                                }
                            }
                            //jog
                            if (A0JA1.Text != "")
                            {
                                PLC1.Write_Data_DWord_("D" + (5108 + Memory_PLC.K800).ToString(), Convert.ToInt32(A0JA1.Text));
                            }
                            if (A0JA2.Text != "")
                            {
                                PLC1.Write_Data_DWord_("D" + (5208 + Memory_PLC.K800).ToString(), Convert.ToInt32(A0JA2.Text));
                            }
                        }
                    }
                    else if (tabControl_PLC_ == 2)
                    {
                        if (tabvision_PLC == 0)
                        {
                            for (int i = 0; i < 36; i++)
                            {
                                if (i < 28)
                                {
                                    UIButton btn = this.Controls.Find("G3_M_" + i, true).FirstOrDefault() as UIButton;
                                    if (btn != null) { StatusDisplay.Instance.STT_Button_Display_SV1(btn, VaribalePLC.Group3[i]); }
                                }
                                else if (i > 28 && i < 31)
                                {
                                    UIButton btn = this.Controls.Find("G3_M_" + i, true).FirstOrDefault() as UIButton;
                                    if (btn != null) { StatusDisplay.Instance.STT_Button_Display_SV1(btn, VaribalePLC.Group3[i]); }
                                }
                                else if (i > 32 && i < 36)
                                {
                                    UIButton btn = this.Controls.Find("G3_M_" + i, true).FirstOrDefault() as UIButton;
                                    if (btn != null) { StatusDisplay.Instance.STT_Button_Display_SV1(btn, VaribalePLC.Group3[i]); }
                                }
                            }
                            StatusDisplay.Instance.STT_Sensor1(G3_M_28, VaribalePLC.Group3[28]);
                            StatusDisplay.Instance.STT_Sensor1(G3_M_31, VaribalePLC.Group3[31]);
                            StatusDisplay.Instance.STT_Sensor1(G3_M_32, VaribalePLC.Group3[32]);
                            StatusDisplay.Instance.STT_Sensor1(G3_M_36, VaribalePLC.Group3[36]);
                            StatusDisplay.Instance.STT_Sensor1(G3_M_37, VaribalePLC.Group3[37]);
                        }
                        else if (tabvision_PLC == 1)
                        {
                            for (int ii = 0; ii < 20; ii++)
                            {
                                if (ii < 8)
                                {
                                    UIButton btn1 = this.Controls.Find("G3_T_" + ii, true).FirstOrDefault() as UIButton;
                                    if (btn1 != null) { StatusDisplay.Instance.STT_Button_Display_SV1(btn1, VaribalePLC.Group32[ii]); }
                                }
                                else
                                {
                                    UISymbolLabel sybl = this.Controls.Find("G3_T_" + ii, true).FirstOrDefault() as UISymbolLabel;
                                    if (sybl != null) { StatusDisplay.Instance.STT_SybolUILabel1(sybl, VaribalePLC.Group32[ii]); }
                                }
                            }
                            for (int k = 0; k < 8; k++)
                            {
                                UITextBox txt2 = this.Controls.Find("G3_Text_" + k, true).FirstOrDefault() as UITextBox;
                                if (k < 4)
                                {
                                    if (txt2 != null) { StatusDisplay.Instance.Update_text(txt2, VaribalePLC.Group3_text[k]); }
                                }
                                else
                                {
                                    if (txt2 != null) { StatusDisplay.Instance.Update_text3(txt2, VaribalePLC.Group3_Dtext[k - 4]); }
                                }
                            }
                            //jog
                            if (A0JA3.Text != "")
                            {
                                PLC1.Write_Data_DWord_("D" + (5308 + Memory_PLC.K800).ToString(), Convert.ToInt32(A0JA3.Text));
                            }
                            if (A0JA4.Text != "")
                            {
                                PLC1.Write_Data_DWord_("D" + (5408 + Memory_PLC.K800).ToString(), Convert.ToInt32(A0JA4.Text));
                            }
                            if (A0JA5.Text != "")
                            {
                                PLC1.Write_Data_DWord_("D" + (5508 + Memory_PLC.K800).ToString(), Convert.ToInt32(A0JA5.Text));
                            }
                            if (A0JA6.Text != "")
                            {
                                PLC1.Write_Data_DWord_("D" + (5608 + Memory_PLC.K800).ToString(), Convert.ToInt32(A0JA6.Text));
                            }
                        }
                    }
                    else if (tabControl_PLC_ == 5)
                    {
                        //io 1
                        if (tabIO_input_PLC == 0)
                        {
                            for (int i = 0; i < 32; i++)
                            {
                                UILight lamp_input = this.Controls.Find("I" + i, true).FirstOrDefault() as UILight;
                                if (lamp_input != null) { StatusDisplay.Instance.IO_light1(lamp_input, VaribalePLC.M_Input1[i]); }
                            }
                        }
                        //io2
                        else if (tabIO_input_PLC == 1)
                        {
                            for (int j = 32; j < 64; j++)
                            {
                                UILight lamp_input = this.Controls.Find("I" + j, true).FirstOrDefault() as UILight;
                                if (lamp_input != null) { StatusDisplay.Instance.IO_light1(lamp_input, VaribalePLC.M_Input2[j - 32]); }
                            }
                        }
                        //io3
                        else if (tabIO_input_PLC == 2)
                        {
                            for (int zj = 64; zj < 70; zj++)
                            {
                                UILight lamp_input = this.Controls.Find("I" + zj, true).FirstOrDefault() as UILight;
                                if (lamp_input != null) { StatusDisplay.Instance.IO_light1(lamp_input, VaribalePLC.M_Input3[zj - 64]); }
                            }
                        }
                    }
                    else if (tabControl_PLC_ == 6)
                    {
                        //io 1
                        if (tabIO_output_PLC == 0)
                        {
                            for (int i = 0; i < 32; i++)
                            {
                                UILight lamp_Output1 = this.Controls.Find("Out" + i, true).FirstOrDefault() as UILight;
                                if (lamp_Output1 != null) { StatusDisplay.Instance.IO_light1(lamp_Output1, VaribalePLC.M_Output1[i]); }
                            }
                        }
                        //io2
                        else if (tabIO_output_PLC == 1)
                        {
                            for (int j = 32; j < 64; j++)
                            {
                                UILight lamp_Output2 = this.Controls.Find("Out" + j, true).FirstOrDefault() as UILight;
                                if (lamp_Output2 != null) { StatusDisplay.Instance.IO_light1(lamp_Output2, VaribalePLC.M_Output2[j - 32]); }
                            }
                        }
                        //io2
                        else if (tabIO_output_PLC == 2)
                        {
                            for (int k = 64; k < 96; k++)
                            {
                                UILight lamp_Output3 = this.Controls.Find("Out" + k, true).FirstOrDefault() as UILight;
                                if (lamp_Output3 != null) { StatusDisplay.Instance.IO_light1(lamp_Output3, VaribalePLC.M_Output3[k - 64]); }
                            }
                        }
                    }
                }
            }
            catch { };
        }
        private async Task PLC_Display()
        {
            await Task.Delay(30);
            if (Global.Start_Start == 0)
            {
                await Read_PLC();
                await Monitor_Current_PLC2();
            }
        }
        private async Task Monitor_Main()
        {
            await Task.Delay(30);
            SDKHrobot.HRobot.get_current_position(handle, current_satefy);
            if (Global.Security_Place == true)
            {
                if (current_satefy[0] > Global.X_Place_Satefy_U || current_satefy[0] < Global.X_Place_Satefy_L ||
                    current_satefy[1] > Global.Y_Place_Satefy_U || current_satefy[1] < Global.Y_Place_Satefy_L || current_satefy[2] < Global.Z_Place_Satefy)
                {
                    SDKHrobot.HRobot.motion_abort(handle);
                    Process_Auto("Place Error Security", file_process_auto);
                    stop_change = 1;
                    stopOn();
                }
            }
            if (FuncRobot.Security_Check_Camtop == true)
            {
                if (current_satefy[0] > Global.X_Camtop_Satefy_U || current_satefy[0] < Global.X_Camtop_Satefy_L ||
                    current_satefy[1] > Global.Y_Camtop_Satefy_U || current_satefy[1] < Global.Y_Camtop_Satefy_L || current_satefy[2] < Global.Z_Camtop_Satefy)
                {
                    SDKHrobot.HRobot.motion_abort(handle);
                    Process_Auto("Check Marking Error Security", file_process_auto);
                    stop_change = 1;
                    stopOn();
                }
            }
            if (Global.Start_Start == 1)
            {
                if (Global.CMD_Scan_Flag == true)
                {
                    Global.result_Cam_Bot = PLC1.Read_Word_Arr("D" + (9250 + Memory_PLC.K2000).ToString(), Global.Number_Tool);
                    bool check_data_vision = Global.result_Cam_Bot.Contains(0);
                    if (check_data_vision == false)
                    {
                        Global.CMD_Scan_Flag = false;
                    }
                }
                if (SDKHrobot.HRobot.get_digital_input(handle, 55) == 0 && checkBox_SatefyBehind.Checked == false)
                {
                    stop_change = 2;
                    ss_satefy = 1;
                }
                if (SDKHrobot.HRobot.get_digital_input(handle, 55) == 1 && checkBox_SatefyBehind.Checked == false &&
                    SDKHrobot.HRobot.get_digital_input(handle, 59) == 1 && SDKHrobot.HRobot.get_digital_input(handle, 51) == 0)
                {
                    stop_change = 0;
                    ss_satefy = 0;
                    SDKHrobot.HRobot.motion_continue(handle);
                }
                TimeSpan timer = timer_all.Elapsed;
                StatusDisplay.Instance.Update_process(txt_total, timer.TotalSeconds.ToString("F3"));
                //if ((SDKHrobot.HRobot.get_digital_input(handle, 15) == 0) && current_satefy[0] < Global.Homee[0] && pause_rb == 0)//satefy
                //{
                //    pause_rb = 1;
                //    SDKHrobot.HRobot.motion_hold(handle);
                //}
                //StatusDisplay.Instance.Update_text(txt_time_hut, FuncRobot.Flag_Cam1);
                //StatusDisplay.Instance.Update_text(txt_time_tha, FuncRobot.Flag_Cam2);
                StatusDisplay.Instance.STT_SybolUILabel1(btn_pause_RB, ss_satefy);
                //statusDisplay.stt_Lamp(sysmbol_alarm_vaccum_pick_NG, PLC1.Read_Data_Bit_("M1400"));
            }
            if (SDKHrobot.HRobot.get_digital_input(handle, 51) == 1 || stop_change == 2) // pause
            {
                stop_change = 2;
                stopOn();
            }
            if (SDKHrobot.HRobot.get_digital_input(handle, 52) == 1 || stop_change == 1)//stop
            {
                SDKHrobot.HRobot.motion_abort(handle);
                stop_change = 1;
                stopOn();
                //StatusDisplay.Instance.Enable_Button(btn_Reset_, 1);
            }
            if (stop_change == 2 && SDKHrobot.HRobot.get_digital_input(handle, 51) == 0 && Global.Start_Start == 1)
            {
                //PLC1.Write_DataBit_("M" + (9016 + Memory_PLC.K1000).ToString(), 0);
                stop_change = 0;
                flag_switch = 0;
                SDKHrobot.HRobot.motion_continue(handle);
            }
            if (text_curr_FPCB == false)
            {
                if (StatusDisplay.Instance.IsFormOpen == true)
                {
                    if (handle == 0)
                    {
                        StatusDisplay.Instance.Update_text(txt_set_number_fpcb, SDKHrobot.HRobot.get_counter(handle, 2));
                        PLC1.Write_Data_Word_("D" + (3010 + Memory_PLC.K1000).ToString(), Convert.ToInt16(txt_set_number_fpcb.Text)); // number FPCB current in tray
                    }
                    else
                    {
                        StatusDisplay.Instance.Update_process(txt_process, "Disconnect Robot");
                        SDKHrobot.HRobot.motion_abort(handle);
                        stop_change = 1;
                        stopOn();
                    }
                }
            }
            Global.dataMonitor = PLC1.ReadRandomBit("M" + (14300 + Memory_PLC.K1000).ToString(), 6);
            if (tab_select_all == 0)
            {
                int[] value_button = new int[5];
                SDKHrobot.HRobot.get_DI_range(handle, 49, 53, value_button);
                Global.Status_Machine_Server = value_button;
                StatusDisplay.Instance.STT_Button_Display_Control2(btn_Auto, value_button[0]);
                StatusDisplay.Instance.STT_Button_Display_Control2(btn_Start, value_button[1]);
                StatusDisplay.Instance.STT_Button_Display_Control2(btn_Pause, value_button[2]);
                StatusDisplay.Instance.STT_Button_Display_Control2(btn_Stop, value_button[3]);
                StatusDisplay.Instance.STT_Button_Display_Control2(btn_Origin, value_button[4]);

                Global.Current_Lot = PLC1.Read_Data_Word_("D" + (3000 + Memory_PLC.K1000).ToString());
                StatusDisplay.Instance.Update_text(txt_number_tray_curr, Global.Current_Lot);
                StatusDisplay.Instance.STT_Int_UILight1(uiLight_Alarm, Global.dataMonitor[0]);
                StatusDisplay.Instance.STT_Int_UILight1(uiLight_trayinput, Global.dataMonitor[1]);
                StatusDisplay.Instance.STT_Int_UILight1(uiLight_trayoutput, Global.dataMonitor[2]);
                //
                Global.SL_OK = PLC1.Read_Data_DWord_("D" + (9392 + Memory_PLC.K2000).ToString());
                Global.SL_NG = PLC1.Read_Data_DWord_("D" + (9394 + Memory_PLC.K2000).ToString());
                Global.SL_Total_Input = Global.SL_OK + Global.SL_NG;
                StatusDisplay.Instance.Update_text(txt_FPCB_OK, Global.SL_OK);
                StatusDisplay.Instance.Update_text(txt_FPCB_NG, Global.SL_NG);
                StatusDisplay.Instance.Update_text(txt_total_input, Global.SL_Total_Input);
                int curr_fpcbtray = SDKHrobot.HRobot.get_counter(handle, 2);
                if (Global.Current_Lot > 0 && curr_fpcbtray > 0)
                {
                    StatusDisplay.Instance.Update_text(txt_lot, (Global.Current_Lot - 1) * (Global.Row_tray * Global.Column_tray) + (curr_fpcbtray - 1));
                }
                else { StatusDisplay.Instance.Update_text(txt_lot, 0); }
                if (Global.Current_Lot == Global.Number_Tray_output && Global.Flag_ResetTray == false)
                {
                    StatusDisplay.Instance.Update_text_toText(txt_Lot_Old, txt_lot);
                    Global.Flag_ResetTray = true;
                    PLC1.Write_Data_DWord_("D" + (9022 + Memory_PLC.K2000).ToString(), Convert.ToInt32(txt_Lot_Old.Text));
                }
                if (Global.Current_Lot != Global.Number_Tray_output) { Global.Flag_ResetTray = false; }
                if (Global.SL_Total_Input != 0)
                {
                    StatusDisplay.Instance.Update_text_rate(txt_rate, Convert.ToDouble((double)Global.SL_OK / (double)Global.SL_Total_Input));
                }
                else
                {
                    StatusDisplay.Instance.Update_text_rate(txt_rate, 0 / 1);
                }
                StatusDisplay.Instance.STT_Int_UILight(Sysmbol_Flag_Buffer1, Global.dataMonitor[3]);
                StatusDisplay.Instance.STT_Int_UILight(Sysmbol_Flag_Buffer2, Global.dataMonitor[4]);
                StatusDisplay.Instance.STT_Int_UILight(Sysmbol_Flag_Pick, Global.dataMonitor[5]);
                StatusDisplay.Instance.STT_Int_UILight(Sysmbol_Flag_Inspec, SDKHrobot.HRobot.get_digital_input(handle, 5));
                StatusDisplay.Instance.STT_Int_UILight(Sysmbol_Flag_Place_tray, SDKHrobot.HRobot.get_digital_output(handle, 51));
                //StatusDisplay.Instance.Update_text(txt_matrix_1, ind_n1);
                //StatusDisplay.Instance.Update_text(txt_matrix_2, ind_n2);
                //StatusDisplay.Instance.Update_text(txt_matrix_3, ind_n3);
            }
            if (Global.Start_Start == 1 || Global.home_start == 1)
            {
                Global.Warring_ListView = PLC1.ReadRandomBit("M" + (8150 + Memory_PLC.K200).ToString(), 20);
                Warring();
            }
            //Global.Timer_rsCounter = DateTime.Now;
            //bool IsTime1 = Global.Timer_rsCounter.Hour == 7 && Global.Timer_rsCounter.Minute == 55 && Global.Timer_rsCounter.Second >= 30 && Global.Timer_rsCounter.Second < 31;
            //bool IsTime2 = Global.Timer_rsCounter.Hour == 19 && Global.Timer_rsCounter.Minute == 55 && Global.Timer_rsCounter.Second >= 30 && Global.Timer_rsCounter.Second < 31;
            //if ((IsTime1 || IsTime2) && !Global.HasWrittenToday)
            //{
            //    write_Production_database(txt_total_input.Text, txt_FPCB_OK.Text, txt_FPCB_NG.Text);
            //    PLC1.Write_DataBit_("L" + (80 + Memory_PLC.K100).ToString(), 1);
            //    Global.HasWrittenToday = true;
            //}
            //if (Global.Timer_rsCounter.Minute != 55)
            //{
            //    Global.HasWrittenToday = false;
            //}
        }
        private async Task Monitor_Result_Cam()
        {
            await Task.Delay(10);
            try
            {
                VaribalePLC.Result_Cam_Bot_Check = PLC1.Read_Word_Arr("D" + (9110 + Memory_PLC.K2000), Global.Row_Jig_input * Global.Column_Jig_input);
                VaribalePLC.Result_Cam_Top_Check = PLC1.Read_Word_Arr("D" + (1600 + Memory_PLC.K100), Global.Row_Jig_input * Global.Column_Jig_input);
                //VaribalePLC.Result_Cam_Bot_Check = new int[10] { 1, 1, 3, 1, 1, 2, 1, 1, 2, 1 };
                //VaribalePLC.Result_Cam_Top_Check = new int[10] { 1, 1, 3, 1, 1, 2, 1, 2, 2, 1 };
                for (int i = 1; i <= Global.Row_Jig_input * Global.Column_Jig_input; i++)
                {
                    UILabel lab1 = this.Controls.Find("lbl_B" + i, true).FirstOrDefault() as UILabel;
                    UILabel lab2 = this.Controls.Find("lbl_T" + i, true).FirstOrDefault() as UILabel;
                    if (lab1 != null) { StatusDisplay.Instance.STT_Label(lab1, VaribalePLC.Result_Cam_Bot_Check[i - 1]); }
                    if (lab2 != null) { StatusDisplay.Instance.STT_Label(lab2, VaribalePLC.Result_Cam_Top_Check[i - 1]); }
                }
            }
            catch { }
        }
        public async Task monitor_all()
        {
            await Task.Delay(30);
            Stopwatch Sw = new Stopwatch();
            Sw.Start();
            while (StatusDisplay.Instance.IsFormOpen == true)
            {
                StatusDisplay.Instance.STT_Bool_UILight(LightPing, true);
                switch (tab_select_all)
                {
                    case 1:
                        Global.cancel_alarm = false;
                        PLC1.Close_Alarm = true;
                        await Monitor_Current_Robot();
                        break;
                    case 2:
                        Global.cancel_alarm = false;
                        PLC1.Close_Alarm = false;
                        await PLC_Display();
                        break;
                    case 3:
                        Global.cancel_alarm = true;
                        PLC1.Close_Alarm = false;
                        await Alarm3();
                        break;
                    case 4: await Task.Delay(10); break;
                    case 5: await Task.Delay(10); break;
                }
                if (tab_select_all == 0 || Global.Start_Start == 1)
                {
                    Global.cancel_alarm = false;
                    PLC1.Close_Alarm = true;
                    await Monitor_Main();
                    if (tab_select_all == 0)
                    {
                        await Monitor_Result_Cam();
                    }
                }

                //await ResetCounterQty();
                Sw.Stop();
                StatusDisplay.Instance.Update_UILabel3(PingMain, Sw.ElapsedMilliseconds.ToString());
                Sw.Restart();
            }
            StatusDisplay.Instance.STT_Bool_UILight(LightPing, false);
        }
        private async Task Monitor_Current_Robot()
        {
            await Task.Delay(30);
            if (tab_select_all == 1 && Global.Start_Start == 0)
            {
                if (tabControl_Robot_ == 0)
                {
                    if (tabControl_Jog_RB_ == 0)
                    {
                        SDKHrobot.HRobot.get_current_position(handle, Global.Get_Curent_XYZ_RB);
                        if (Global.GoX == false)
                        {
                            StatusDisplay.Instance.Update_text_Double(txt_Curr_X, Global.Get_Curent_XYZ_RB[0]);
                        }
                        if (Global.GoY == false)
                        {
                            StatusDisplay.Instance.Update_text_Double(txt_Curr_Y, Global.Get_Curent_XYZ_RB[1]);
                        }
                        if (Global.GoZ == false)
                        {
                            StatusDisplay.Instance.Update_text_Double(txt_Curr_Z, Global.Get_Curent_XYZ_RB[2]);
                        }
                        if (Global.GoC == false)
                        {
                            StatusDisplay.Instance.Update_text_Double(txt_Curr_A4, Global.Get_Curent_XYZ_RB[5]);
                        }
                    }
                    else if (tabControl_Jog_RB_ == 1)
                    {
                        SDKHrobot.HRobot.get_current_joint(handle, Global.Get_Curent_Joint_RB);
                        StatusDisplay.Instance.Update_text_Double(txt_Curr_A1, Global.Get_Curent_Joint_RB[0]);
                        StatusDisplay.Instance.Update_text_Double(txt_Curr_A2, Global.Get_Curent_Joint_RB[1]);
                        StatusDisplay.Instance.Update_text_Double(txt_Curr_A3, Global.Get_Curent_Joint_RB[2]);
                        StatusDisplay.Instance.Update_text_Double(txt_Curr_A4_Joint, Global.Get_Curent_Joint_RB[3]);

                    }
                }
                else if (tabControl_Robot_ == 4)
                {
                    for (int i = 1; i <= 16; i++)
                    {
                        UILight Light = this.Controls.Find("DI" + i, true).FirstOrDefault() as UILight;
                        StatusDisplay.Instance.IO_light(Light, FuncRobot.Monitor_DI(handle, i));
                    }
                    for (int i = 49; i <= 64; i++)
                    {
                        UILight Light = this.Controls.Find("DI" + i, true).FirstOrDefault() as UILight;
                        StatusDisplay.Instance.IO_light(Light, FuncRobot.Monitor_DI(handle, i));
                    }
                }
                else if (tabControl_Robot_ == 5 && click_DO == 0)
                {
                    for (int i = 49; i <= 64; i++)
                    {
                        UILight Light = this.Controls.Find("DO" + i, true).FirstOrDefault() as UILight;
                        StatusDisplay.Instance.IO_light(Light, FuncRobot.Monitor_DO(handle, i));
                    }
                    for (int i = 1; i <= 8; i++)
                    {
                        UILight Light = this.Controls.Find("DO" + i, true).FirstOrDefault() as UILight;
                        StatusDisplay.Instance.IO_light(Light, FuncRobot.Monitor_DO(handle, i));
                    }
                }
                //string path = AppDomain.CurrentDomain.BaseDirectory;
                //if (!Directory.Exists(path))
                //{
                //    Directory.CreateDirectory(path);
                //}
                //string filePath = Path.Combine(path, "DataPos.txt");
                //DateTime date = DateTime.Now;
                //string[] content = {"1= " +Global.Pick_Press[0].ToString() +'\r'+ "2= "+Global.Ready_Pick_Output_1[0].ToString()+'\r'+"3= " + Global.Ready_Pick_Output_2[0].ToString() + '\r' + "4= "+ Global.Place_Tray_1[0].ToString()  + "---" + date.ToString() };
                //if (!File.Exists(filePath))
                //{
                //    File.Create(filePath).Dispose();
                //}
                //File.AppendAllLines(filePath, content);
            }
        }

        #endregion
        #region Program Auto---------------------------------------------------------------
        private void Start_On()
        {
            Load_Process_Old();
            while ((n < stop_index && SDKHrobot.HRobot.get_digital_input(handle, 50) == 1) && stop_change != 1)
            {
                if (SDKHrobot.HRobot.get_digital_output(handle, 2) == 1)
                {
                    CMD_Transfer_Tray();
                }
                while (stop_change != 1 && SDKHrobot.HRobot.get_digital_input(handle, 50) == 1)
                {
                    Check_Status_MC();
                    while (FuncRobot.CMD_Pick == false && PLC1.Read_Data_Bit_("L" + (91 + Memory_PLC.K100).ToString()) == false
                       && SDKHrobot.HRobot.get_digital_input(handle, 5) == 0 && SDKHrobot.HRobot.get_digital_input(handle, 1) == 0 && FuncRobot.CMD_Pick_Output == false && Global.combox_mode == 0)
                    {
                        Thread.Sleep(50);
                        StatusDisplay.Instance.Update_process(txt_process, "Wait Pick Fpcb Press");
                    }
                    timer_all = new Stopwatch();
                    timer_all.Start();
                    Motion_Pick_Press();
                    if (SDKHrobot.HRobot.get_digital_input(handle, 8) == 1 || (SDKHrobot.HRobot.get_digital_input(handle, 1) == 0 && SDKHrobot.HRobot.get_digital_input(handle, 2) == 0 && SDKHrobot.HRobot.get_digital_input(handle, 8) == 0)) // flag fpcb jig remov
                    {
                        FuncRobot.Wait_For_Digital_Input_On(handle, 16);
                        Motion_Input_FPCB();
                        FuncRobot.Wait_for_stop_motion_2(handle);
                        Motion_Check_Marking();
                    }
                    Motion_Pick_Output();
                    Execution_Transfer_Tray();
                    Motion_Place_Tray();
                    timer_all.Stop();
                    TimeSpan tt = timer_all.Elapsed;
                    string path = AppDomain.CurrentDomain.BaseDirectory;
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }
                    string[] content = { "cycle =" + tt.TotalSeconds.ToString("F3") };
                    File.AppendAllLines(path + Path.DirectorySeparatorChar + "Cycle.txt", content);
                }
            }
        }
        private void Machine_Pause_(int Change)
        {
            while (stop_change == 2)
            {
                Thread.Sleep(50);
                StatusDisplay.Instance.Update_process(txt_process, "Machine Pause");
            }
        }
        private void stopOn()
        {
            flag_switch++;
            if (stop_change == 1)
            {
                PLC1.Write_DataBit_("M" + (5006 + Memory_PLC.K40).ToString(), 1);
                SDKHrobot.HRobot.motion_abort(handle);
                try
                {

                    if (Start.ThreadState != System.Threading.ThreadState.Stopped)
                    {
                        Start.Abort();
                    }
                    if (Home.ThreadState != System.Threading.ThreadState.Stopped)
                    {
                        Home.Abort();
                    }
                }
                catch { SDKHrobot.HRobot.motion_abort(handle); }
                PLC1.Write_DataBit_("M" + (5006 + Memory_PLC.K40).ToString(), 0);
                if (handle == 0)
                {
                    SDKHrobot.HRobot.set_operation_mode(handle, 0);
                }

                Global.CMD_Scan_Flag = false;

                if (flag_switch == 1)
                {
                    string pathst = AppDomain.CurrentDomain.BaseDirectory;

                    if (!Directory.Exists(pathst))
                    {
                        Directory.CreateDirectory(pathst);
                    }
                    string[] History1 = { "Machine Stop " + DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss") };
                    File.AppendAllLines(pathst + Path.DirectorySeparatorChar + "History Alamr.txt", History1);
                }

            }
            else if (stop_change == 2)
            {

                SDKHrobot.HRobot.motion_hold(handle);
                if (flag_switch == 1)
                {
                    string pathst = AppDomain.CurrentDomain.BaseDirectory;

                    if (!Directory.Exists(pathst))
                    {
                        Directory.CreateDirectory(pathst);
                    }
                    string[] History2 = { "Machine Pause " + DateTime.Now.ToString("dd / MM / yyyy hh: mm:ss") };
                    File.AppendAllLines(pathst + Path.DirectorySeparatorChar + "History Alamr.txt", History2);
                }
            }
        }
        private void Process_Auto(string data_, string file_name)
        {
            string path = AppDomain.CurrentDomain.BaseDirectory;

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            string filePath = Path.Combine(path, file_name);
            DateTime date = DateTime.Now;
            string[] content = { data_ + "---" + date.ToString() };
            if (!File.Exists(filePath))
            {
                File.Create(filePath).Dispose();
            }
            File.AppendAllLines(filePath, content);
        }
        private void home()
        {
            start_home++;
            int res1 = -1;
            if (start_home == 1)
            {
                StatusDisplay.Instance.Update_process(txt_process, "Origin Waiting ...");
                int sp_home = Convert.ToInt16(Global.SP_Home);
                Thread.Sleep(50);
                SDKHrobot.HRobot.set_operation_mode(handle, 0);
                SDKHrobot.HRobot.set_override_ratio(handle, sp_home);
                SDKHrobot.HRobot.set_digital_output(handle, 50, false);
                FuncRobot.Flag_Wait_Pick_Press = false;
                Thread.Sleep(50);
                // PLC1.Write_DataBit_("m5005", 1);

                for (int i = 0; i < 4; i++)
                {
                    SDKHrobot.HRobot.set_digital_output(handle, i + 1, false);
                }
                int auto = SDKHrobot.HRobot.get_digital_input(handle, 49);
                int start = SDKHrobot.HRobot.get_digital_input(handle, 50);
                int pause = SDKHrobot.HRobot.get_digital_input(handle, 51);
                int stop = SDKHrobot.HRobot.get_digital_input(handle, 52);
                if (start == 0 & auto == 0 & pause == 0 & stop == 0)

                {
                    SDKHrobot.HRobot.clear_alarm(handle);
                    if (handle == 0)
                    {
                        SDKHrobot.HRobot.set_connection_level(handle, 1);

                        if (SDKHrobot.HRobot.get_motor_state(handle) == 0)
                        {
                            SDKHrobot.HRobot.set_motor_state(handle, 1);   // Servo on
                        }
                        double[] Pos_current1 = new double[6];
                        double[] Pos_current2 = new double[6];
                        double[] Pos_home_2 = new double[6];
                        double[] Pos_home_Joint1 = new double[6];
                        double[] Pos_home = new double[6];
                        double[] Pos_home_Mov1 = new double[6];
                        res1 = SDKHrobot.HRobot.get_current_position(handle, Pos_current1);
                        if (Pos_current1[1] == Global.Ready_Arc_1[1] && Pos_current1[0] < Global.Ready_Arc_2[0] && Pos_current1[0] > Global.Ready_Pick_Press_1[0])
                        {
                            SDKHrobot.HRobot.lin_pos(handle, 1, 30, Global.Ready_Arc_1);
                            SDKHrobot.HRobot.lin_pos(handle, 1, 30, Global.Homee);
                            FuncRobot.Wait_for_stop_motion_2(handle);
                            SDKHrobot.HRobot.get_current_position(handle, Pos_home);
                            if (Pos_home[0] == Global.Homee[0] && Pos_home[1] == Global.Homee[1] && Pos_home[2] == Global.Homee[2] && Pos_home[3] == Global.Homee[3] && Pos_home[4] == Global.Homee[4] && Pos_home[5] == Global.Homee[5])
                            {
                                SDKHrobot.HRobot.set_digital_output(handle, 1, true);// rb origin ok
                                SDKHrobot.HRobot.set_digital_output(handle, 49, false);//ROBOT 1 BUSY PICK FPCB PRESS
                            }
                            FuncRobot.Wait_For_Digital_Input_On(handle, 53);//flag home

                            PLC1.Write_DataBit_("L" + (31 + Memory_PLC.K100).ToString(), 1); //diem an toan
                            PLC1.Write_DataBit_("L" + (39 + Memory_PLC.K100).ToString(), 1); //diem an toan
                            PLC1.Write_DataBit_("L" + (40 + Memory_PLC.K100).ToString(), 1); //diem an toan
                            SDKHrobot.HRobot.set_digital_output(handle, 5, false);
                            FuncRobot.Flag_Wait_Pick_Press = false;
                            SDKHrobot.HRobot.set_digital_output(handle, 8, false);
                            if (SDKHrobot.HRobot.get_digital_output(handle, 51) == 1)
                            {
                                PLC1.Write_DataBit_("L" + (301 + Memory_PLC.K1).ToString(), 0);
                            }
                        }//1
                        else if (Pos_current1[0] < Global.Ready_Arc_2[0] && (Pos_current1[0] > Global.Homee[0] || Pos_current1[0] == Global.Homee[0]) && Pos_current1[2] > Global.Homee[2] - 15)
                        {
                            //SDKHrobot.HRobot.lin_pos(handle, 1, 30, Ready_Arc_2);
                            FuncRobot.Wait_for_stop_motion(handle);
                            SDKHrobot.HRobot.lin_pos(handle, 1, 30, Global.Ready_Arc_1);
                            SDKHrobot.HRobot.lin_pos(handle, 1, 30, Global.Homee);
                            FuncRobot.Wait_for_stop_motion_2(handle);
                            SDKHrobot.HRobot.get_current_position(handle, Pos_home);
                            if (Pos_home[0] == Global.Homee[0] && Pos_home[1] == Global.Homee[1] && Pos_home[2] == Global.Homee[2] && Pos_home[3] == Global.Homee[3] && Pos_home[4] == Global.Homee[4] && Pos_home[5] == Global.Homee[5])
                            {
                                SDKHrobot.HRobot.set_digital_output(handle, 1, true);// rb origin ok
                                SDKHrobot.HRobot.set_digital_output(handle, 49, false);//ROBOT 1 BUSY PICK FPCB PRESS
                                SDKHrobot.HRobot.set_digital_output(handle, 8, false);// compt pick press
                            }
                            FuncRobot.Wait_For_Digital_Input_On(handle, 53);//flag home
                            PLC1.Write_DataBit_("L" + (31 + Memory_PLC.K100).ToString(), 1); //diem an toan
                            PLC1.Write_DataBit_("L" + (39 + Memory_PLC.K100).ToString(), 1); //diem an toan
                            PLC1.Write_DataBit_("L" + (40 + Memory_PLC.K100).ToString(), 1); //diem an toan
                            SDKHrobot.HRobot.set_digital_output(handle, 5, false);
                            FuncRobot.Flag_Wait_Pick_Press = false;
                            if (SDKHrobot.HRobot.get_digital_output(handle, 51) == 1)
                            {
                                PLC1.Write_DataBit_("L" + (301 + Memory_PLC.K1).ToString(), 0);
                            }
                        }//2
                        else if (Pos_current1[0] == Global.Ready_Pick_Press_1[1] || Pos_current1[0] == Global.Ready_Pick_Press_2[0] || Pos_current1[0] == Global.Pick_Press[0] || Pos_current1[0] < Global.Homee[0])
                        {
                            if (Pos_current1[0] < Global.Homee[0] && (Pos_current1[0] > Global.Ready_Pick_Press_1[0] || Pos_current1[0] == Global.Ready_Pick_Press_1[0]) && Pos_current1[2] == Global.Ready_Pick_Press_1[2] && Pos_current1[1] == Global.Ready_Pick_Press_1[1])
                            {
                                SDKHrobot.HRobot.lin_pos(handle, 1, 10, Global.Homee);
                                FuncRobot.Wait_for_stop_motion_2(handle);
                                SDKHrobot.HRobot.get_current_position(handle, Pos_home);
                                if (Pos_home[0] == Global.Homee[0] && Pos_home[1] == Global.Homee[1] && Pos_home[2] == Global.Homee[2] && Pos_home[3] == Global.Homee[3] && Pos_home[4] == Global.Homee[4] && Pos_home[5] == Global.Homee[5])
                                {
                                    SDKHrobot.HRobot.set_digital_output(handle, 1, true);// rb origin ok
                                    SDKHrobot.HRobot.set_digital_output(handle, 49, false);//ROBOT 1 BUSY PICK FPCB PRESS
                                }
                                FuncRobot.Wait_For_Digital_Input_On(handle, 53);//flag home
                                FuncRobot.Wait_for_stop_motion_2(handle);
                                PLC1.Write_DataBit_("L" + (31 + Memory_PLC.K100).ToString(), 1); //diem an toan
                                PLC1.Write_DataBit_("L" + (39 + Memory_PLC.K100).ToString(), 1); //diem an toan
                                PLC1.Write_DataBit_("L" + (40 + Memory_PLC.K100).ToString(), 1); //diem an toan
                                SDKHrobot.HRobot.set_digital_output(handle, 5, false);
                                FuncRobot.Flag_Wait_Pick_Press = false;
                                SDKHrobot.HRobot.set_digital_output(handle, 8, false);
                                if (SDKHrobot.HRobot.get_digital_output(handle, 51) == 1)
                                {
                                    PLC1.Write_DataBit_("L" + (301 + Memory_PLC.K1).ToString(), 0);
                                }
                            }//3-1
                            else if (Pos_current1[0] < Global.Homee[0] && Pos_current1[0] > Global.Ready_Pick_Press_1[0] && (Pos_current1[2] > Global.Ready_Pick_Press_1[2] || Pos_current1[2] == Global.Ready_Pick_Press_1[2]) || Pos_current1[2] > Global.Ready_Pick_Press_1[2] - 10)
                            {
                                SDKHrobot.HRobot.lin_pos(handle, 1, 10, Global.Homee);
                                FuncRobot.Wait_for_stop_motion_2(handle);
                                SDKHrobot.HRobot.get_current_position(handle, Pos_home);
                                if (Pos_home[0] == Global.Homee[0] && Pos_home[1] == Global.Homee[1] && Pos_home[2] == Global.Homee[2] && Pos_home[3] == Global.Homee[3] && Pos_home[4] == Global.Homee[4] && Pos_home[5] == Global.Homee[5])
                                {
                                    SDKHrobot.HRobot.set_digital_output(handle, 1, true);// rb origin ok
                                    SDKHrobot.HRobot.set_digital_output(handle, 49, false);//ROBOT 1 BUSY PICK FPCB PRESS
                                }
                                FuncRobot.Wait_For_Digital_Input_On(handle, 53);//flag home
                                FuncRobot.Wait_for_stop_motion_2(handle);
                                PLC1.Write_DataBit_("L" + (31 + Memory_PLC.K100).ToString(), 1); //diem an toan
                                PLC1.Write_DataBit_("L" + (39 + Memory_PLC.K100).ToString(), 1); //diem an toan
                                PLC1.Write_DataBit_("L" + (40 + Memory_PLC.K100).ToString(), 1); //diem an toan
                                SDKHrobot.HRobot.set_digital_output(handle, 5, false);
                                FuncRobot.Flag_Wait_Pick_Press = false;
                                SDKHrobot.HRobot.set_digital_output(handle, 8, false);
                                if (SDKHrobot.HRobot.get_digital_output(handle, 51) == 1)
                                {
                                    PLC1.Write_DataBit_("L" + (301 + Memory_PLC.K1).ToString(), 0);
                                }
                            }//3-2
                            else if (Pos_current1[0] < Global.Homee[0] && (Pos_current1[0] < Global.Ready_Pick_Press_1[0] || Pos_current1[0] == Global.Ready_Pick_Press_1[0]) && (Pos_current1[2] == Global.Ready_Pick_Press_2[2] || Pos_current1[2] < Global.Ready_Pick_Press_1[2]))
                            {
                                SDKHrobot.HRobot.lin_pos(handle, 1, 10, Global.Ready_Pick_Press_1);
                                FuncRobot.Wait_for_stop_motion_2(handle);
                                SDKHrobot.HRobot.lin_pos(handle, 1, 10, Global.Homee);
                                FuncRobot.Wait_for_stop_motion_2(handle);
                                SDKHrobot.HRobot.get_current_position(handle, Pos_home);
                                if (Pos_home[0] == Global.Homee[0] && Pos_home[1] == Global.Homee[1] && Pos_home[2] == Global.Homee[2] && Pos_home[3] == Global.Homee[3] && Pos_home[4] == Global.Homee[4] && Pos_home[5] == Global.Homee[5])
                                {
                                    SDKHrobot.HRobot.set_digital_output(handle, 1, true);// rb origin ok
                                    SDKHrobot.HRobot.set_digital_output(handle, 49, false);//ROBOT 1 BUSY PICK FPCB PRESS
                                }
                                FuncRobot.Wait_For_Digital_Input_On(handle, 53);//flag home
                                PLC1.Write_DataBit_("L" + (31 + Memory_PLC.K100).ToString(), 1); //diem an toan
                                PLC1.Write_DataBit_("L" + (39 + Memory_PLC.K100).ToString(), 1); //diem an toan
                                PLC1.Write_DataBit_("L" + (40 + Memory_PLC.K100).ToString(), 1); //diem an toan
                                SDKHrobot.HRobot.set_digital_output(handle, 5, false);
                                FuncRobot.Flag_Wait_Pick_Press = false;
                                SDKHrobot.HRobot.set_digital_output(handle, 8, false);
                                if (SDKHrobot.HRobot.get_digital_output(handle, 51) == 1)
                                {
                                    PLC1.Write_DataBit_("L" + (301 + Memory_PLC.K1).ToString(), 0);
                                }
                            }//3-3
                            else if (Pos_current1[0] == Global.Ready_Pick_Press_1[0] && Pos_current1[2] == Global.Ready_Pick_Press_2[2] && Pos_current1[1] == Global.Ready_Pick_Press_1[1] && Pos_current1[5] == Global.Ready_Rotation_1[5])
                            {
                                SDKHrobot.HRobot.lin_pos(handle, 1, 10, Global.Ready_Pick_Press_2);
                                SDKHrobot.HRobot.lin_pos(handle, 1, 10, Global.Ready_Pick_Press_1);
                                FuncRobot.Wait_for_stop_motion_2(handle);
                                SDKHrobot.HRobot.get_current_position(handle, Pos_current2);
                                if (Pos_current2[0] == Global.Ready_Pick_Press_1[0] && Pos_current2[1] == Global.Ready_Pick_Press_1[1] && Pos_current2[2] == Global.Ready_Pick_Press_1[2] && Pos_current2[5] == Global.Ready_Pick_Press_1[5])
                                {
                                    SDKHrobot.HRobot.lin_pos(handle, 1, 10, Global.Homee);
                                    FuncRobot.Wait_for_stop_motion_2(handle);
                                }
                                else
                                {
                                    stop_change = 1;
                                    stopOn();
                                }
                                SDKHrobot.HRobot.get_current_position(handle, Pos_home);
                                if (Pos_home[0] == Global.Homee[0] && Pos_home[1] == Global.Homee[1] && Pos_home[2] == Global.Homee[2] && Pos_home[3] == Global.Homee[3] && Pos_home[4] == Global.Homee[4] && Pos_home[5] == Global.Homee[5])
                                {
                                    SDKHrobot.HRobot.set_digital_output(handle, 1, true);// rb origin ok
                                    SDKHrobot.HRobot.set_digital_output(handle, 49, false);//ROBOT 1 BUSY PICK FPCB PRESS
                                }
                                FuncRobot.Wait_For_Digital_Input_On(handle, 53);//flag home
                                FuncRobot.Wait_for_stop_motion_2(handle);
                                PLC1.Write_DataBit_("L" + (31 + Memory_PLC.K100).ToString(), 1); //diem an toan
                                PLC1.Write_DataBit_("L" + (39 + Memory_PLC.K100).ToString(), 1); //diem an toan
                                PLC1.Write_DataBit_("L" + (40 + Memory_PLC.K100).ToString(), 1); //diem an toan
                                SDKHrobot.HRobot.set_digital_output(handle, 5, false);
                                FuncRobot.Flag_Wait_Pick_Press = false;
                                SDKHrobot.HRobot.set_digital_output(handle, 8, false);
                                if (SDKHrobot.HRobot.get_digital_output(handle, 51) == 1)
                                {
                                    PLC1.Write_DataBit_("L" + (301 + Memory_PLC.K1).ToString(), 0);
                                }
                            }//3-4
                            else if (Pos_current1[0] == Global.Ready_Pick_Press_2[0] && Pos_current1[1] == Global.Ready_Pick_Press_2[1] && Pos_current1[2] == Global.Ready_Pick_Press_2[2])
                            {
                                SDKHrobot.HRobot.lin_pos(handle, 1, 10, Global.Ready_Pick_Press_2);
                                SDKHrobot.HRobot.lin_pos(handle, 1, 10, Global.Ready_Pick_Press_1);
                                FuncRobot.Wait_for_stop_motion_2(handle);
                                SDKHrobot.HRobot.get_current_position(handle, Pos_current2);
                                if (Pos_current2[0] == Global.Ready_Pick_Press_1[0] && Pos_current2[1] == Global.Ready_Pick_Press_1[1] && Pos_current2[2] == Global.Ready_Pick_Press_1[2] && Pos_current2[5] == Global.Ready_Pick_Press_1[5])
                                {
                                    SDKHrobot.HRobot.lin_pos(handle, 1, 10, Global.Homee);
                                    FuncRobot.Wait_for_stop_motion_2(handle);
                                }
                                else
                                {
                                    stop_change = 1;
                                    stopOn();
                                }
                                SDKHrobot.HRobot.get_current_position(handle, Pos_home);
                                if (Pos_home[0] == Global.Homee[0] && Pos_home[1] == Global.Homee[1] && Pos_home[2] == Global.Homee[2] && Pos_home[3] == Global.Homee[3] && Pos_home[4] == Global.Homee[4] && Pos_home[5] == Global.Homee[5])
                                {
                                    SDKHrobot.HRobot.set_digital_output(handle, 1, true);// rb origin ok
                                    SDKHrobot.HRobot.set_digital_output(handle, 49, false);//ROBOT 1 BUSY PICK FPCB PRESS
                                }
                                FuncRobot.Wait_For_Digital_Input_On(handle, 53);//flag home
                                FuncRobot.Wait_for_stop_motion_2(handle);
                                PLC1.Write_DataBit_("L" + (31 + Memory_PLC.K100).ToString(), 1); //diem an toan
                                PLC1.Write_DataBit_("L" + (39 + Memory_PLC.K100).ToString(), 1); //diem an toan
                                PLC1.Write_DataBit_("L" + (40 + Memory_PLC.K100).ToString(), 1); //diem an toan
                                SDKHrobot.HRobot.set_digital_output(handle, 5, false);
                                FuncRobot.Flag_Wait_Pick_Press = false;
                                SDKHrobot.HRobot.set_digital_output(handle, 8, false);
                                if (SDKHrobot.HRobot.get_digital_output(handle, 51) == 1)
                                {
                                    PLC1.Write_DataBit_("L" + (301 + Memory_PLC.K1).ToString(), 0);
                                }
                            }//3-5
                            else if ((Pos_current1[0] < Global.Ready_Rotation_1[0] || Pos_current1[0] == Global.Pick_Press[0] || Pos_current1[2] == Global.Z_Pick_Press[2]) && (Pos_current1[5] == Global.Ready_Rotation_1[5] || Pos_current1[5] == Global.Pick_Press[5]))
                            {
                                SDKHrobot.HRobot.lin_pos(handle, 1, 10, Global.Pick_Press);
                                SDKHrobot.HRobot.lin_pos(handle, 1, 10, Global.Ready_Rotation_1);
                                FuncRobot.Wait_for_stop_motion_2(handle);
                                SDKHrobot.HRobot.lin_pos(handle, 1, 10, Global.Ready_Pick_Press_2);
                                SDKHrobot.HRobot.lin_pos(handle, 1, 10, Global.Ready_Pick_Press_1);
                                FuncRobot.Wait_for_stop_motion_2(handle);
                                SDKHrobot.HRobot.get_current_position(handle, Pos_current2);
                                if (Pos_current2[0] == Global.Ready_Pick_Press_1[0] && Pos_current2[1] == Global.Ready_Pick_Press_1[1] && Pos_current2[2] == Global.Ready_Pick_Press_1[2] && Pos_current2[5] == Global.Ready_Pick_Press_1[5])
                                {
                                    SDKHrobot.HRobot.lin_pos(handle, 1, 10, Global.Homee);
                                    FuncRobot.Wait_for_stop_motion_2(handle);
                                }
                                else
                                {
                                    stop_change = 1;
                                    stopOn();
                                }
                                SDKHrobot.HRobot.get_current_position(handle, Pos_home);
                                if (Pos_home[0] == Global.Homee[0] && Pos_home[1] == Global.Homee[1] && Pos_home[2] == Global.Homee[2] && Pos_home[3] == Global.Homee[3] && Pos_home[4] == Global.Homee[4] && Pos_home[5] == Global.Homee[5])
                                {
                                    SDKHrobot.HRobot.set_digital_output(handle, 1, true);// rb origin ok
                                    SDKHrobot.HRobot.set_digital_output(handle, 49, false);//ROBOT 1 BUSY PICK FPCB PRESS
                                }
                                FuncRobot.Wait_For_Digital_Input_On(handle, 53);//flag home
                                FuncRobot.Wait_for_stop_motion_2(handle);
                                PLC1.Write_DataBit_("L" + (31 + Memory_PLC.K100).ToString(), 1); //diem an toan
                                PLC1.Write_DataBit_("L" + (39 + Memory_PLC.K100).ToString(), 1); //diem an toan
                                PLC1.Write_DataBit_("L" + (40 + Memory_PLC.K100).ToString(), 1); //diem an toan
                                SDKHrobot.HRobot.set_digital_output(handle, 5, false);
                                FuncRobot.Flag_Wait_Pick_Press = false;
                                SDKHrobot.HRobot.set_digital_output(handle, 8, false);
                                if (SDKHrobot.HRobot.get_digital_output(handle, 51) == 1)
                                {
                                    PLC1.Write_DataBit_("L" + (301 + Memory_PLC.K1).ToString(), 0);
                                }
                            }//3-6
                        }//3
                        else if (Pos_current1[1] > Global.Z_Input_FPCB[1] || Pos_current1[1] == Global.Z_Input_FPCB[1])
                        {
                            if (Pos_current1[2] == Global.Z_Input_FPCB[2] || Pos_current1[0] == Global.Input_FPCB[0])
                            {
                                double[] Z_antoan_1 = { Pos_current1[0], Pos_current1[1], Global.Z_antoan, Pos_current1[3], Pos_current1[4], Pos_current1[5] };
                                SDKHrobot.HRobot.lin_pos(handle, 1, 30, Z_antoan_1);
                                double[] mov_1 = { Global.Ready_Inputput_1[0], Global.Ready_Inputput_1[1], Global.Z_antoan, Pos_current1[3], Pos_current1[4], Pos_current1[5] };
                                SDKHrobot.HRobot.lin_pos(handle, 1, 10, mov_1);
                                double[] ready_rotation = { Global.Ready_Inputput_1[0], Global.Ready_Inputput_1[1], Global.Ready_Inputput_1[2], Global.Ready_Inputput_1[3], Global.Ready_Inputput_1[4], Global.Ready_Inputput_1[5] };
                                SDKHrobot.HRobot.lin_pos(handle, 1, 20, ready_rotation);
                                SDKHrobot.HRobot.lin_pos(handle, 1, 30, Global.Ready_Check_Camtop_1);
                                SDKHrobot.HRobot.lin_pos(handle, 0, 0, Global.Ready_Arc_2);
                                SDKHrobot.HRobot.lin_pos(handle, 0, 0, Global.Ready_Arc_1);
                                FuncRobot.Wait_for_stop_motion_2(handle);
                                SDKHrobot.HRobot.lin_pos(handle, 1, 30, Global.Homee);
                                FuncRobot.Wait_for_stop_motion_2(handle);
                                SDKHrobot.HRobot.get_current_position(handle, Pos_home);
                                if (Pos_home[0] == Global.Homee[0] && Pos_home[1] == Global.Homee[1] && Pos_home[2] == Global.Homee[2] && Pos_home[3] == Global.Homee[3] && Pos_home[4] == Global.Homee[4] && Pos_home[5] == Global.Homee[5])
                                {
                                    SDKHrobot.HRobot.set_digital_output(handle, 1, true);// rb origin ok
                                    SDKHrobot.HRobot.set_digital_output(handle, 49, false);//ROBOT 1 BUSY PICK FPCB PRESS
                                }
                                FuncRobot.Wait_For_Digital_Input_On(handle, 53);//flag home
                                FuncRobot.Wait_for_stop_motion_2(handle);
                                PLC1.Write_DataBit_("L" + (31 + Memory_PLC.K100).ToString(), 1); //diem an toan
                                PLC1.Write_DataBit_("L" + (39 + Memory_PLC.K100).ToString(), 1); //diem an toan
                                PLC1.Write_DataBit_("L" + (40 + Memory_PLC.K100).ToString(), 1); //diem an toan
                                SDKHrobot.HRobot.set_digital_output(handle, 5, false);
                                FuncRobot.Flag_Wait_Pick_Press = false;
                                SDKHrobot.HRobot.set_digital_output(handle, 8, false);
                                if (SDKHrobot.HRobot.get_digital_output(handle, 51) == 1)
                                {
                                    PLC1.Write_DataBit_("L" + (301 + Memory_PLC.K1).ToString(), 0);
                                }
                            }
                            else
                            {
                                if (Pos_current1[5] != Global.Ready_Check_Camtop_1[5])
                                {
                                    SDKHrobot.HRobot.lin_pos(handle, 1, 20, Global.Ready_Inputput_1);
                                    SDKHrobot.HRobot.lin_pos(handle, 1, 30, Global.Ready_Check_Camtop_1);
                                    SDKHrobot.HRobot.lin_pos(handle, 0, 0, Global.Ready_Arc_2);
                                    SDKHrobot.HRobot.lin_pos(handle, 0, 0, Global.Ready_Arc_1);
                                    FuncRobot.Wait_for_stop_motion_2(handle);
                                    SDKHrobot.HRobot.lin_pos(handle, 1, 30, Global.Homee);
                                    FuncRobot.Wait_for_stop_motion_2(handle);
                                    SDKHrobot.HRobot.get_current_position(handle, Pos_home);

                                    if (Pos_home[0] == Global.Homee[0] && Pos_home[1] == Global.Homee[1] && Pos_home[2] == Global.Homee[2] && Pos_home[3] == Global.Homee[3] && Pos_home[4] == Global.Homee[4] && Pos_home[5] == Global.Homee[5])
                                    {
                                        SDKHrobot.HRobot.set_digital_output(handle, 1, true);// rb origin ok
                                        SDKHrobot.HRobot.set_digital_output(handle, 49, false);//ROBOT 1 BUSY PICK FPCB PRESS
                                    }
                                    FuncRobot.Wait_For_Digital_Input_On(handle, 53);//flag home
                                    FuncRobot.Wait_for_stop_motion_2(handle);
                                    PLC1.Write_DataBit_("L" + (31 + Memory_PLC.K100).ToString(), 1); //diem an toan
                                    PLC1.Write_DataBit_("L" + (39 + Memory_PLC.K100).ToString(), 1); //diem an toan
                                    PLC1.Write_DataBit_("L" + (40 + Memory_PLC.K100).ToString(), 1); //diem an toan
                                    SDKHrobot.HRobot.set_digital_output(handle, 5, false);
                                    FuncRobot.Flag_Wait_Pick_Press = false;
                                    SDKHrobot.HRobot.set_digital_output(handle, 8, false);
                                    if (SDKHrobot.HRobot.get_digital_output(handle, 51) == 1)
                                    {
                                        PLC1.Write_DataBit_("L" + (301 + Memory_PLC.K1).ToString(), 0);
                                    }
                                }
                                else
                                {
                                    SDKHrobot.HRobot.lin_pos(handle, 1, 30, Global.Ready_Check_Camtop_1);
                                    SDKHrobot.HRobot.lin_pos(handle, 0, 0, Global.Ready_Arc_2);
                                    SDKHrobot.HRobot.lin_pos(handle, 0, 0, Global.Ready_Arc_1);
                                    FuncRobot.Wait_for_stop_motion_2(handle);
                                    SDKHrobot.HRobot.lin_pos(handle, 1, 30, Global.Homee);
                                    FuncRobot.Wait_for_stop_motion_2(handle);
                                    SDKHrobot.HRobot.get_current_position(handle, Pos_home);

                                    if (Pos_home[0] == Global.Homee[0] && Pos_home[1] == Global.Homee[1] && Pos_home[2] == Global.Homee[2] && Pos_home[3] == Global.Homee[3] && Pos_home[4] == Global.Homee[4] && Pos_home[5] == Global.Homee[5])
                                    {
                                        SDKHrobot.HRobot.set_digital_output(handle, 1, true);// rb origin ok
                                        SDKHrobot.HRobot.set_digital_output(handle, 49, false);//ROBOT 1 BUSY PICK FPCB PRESS
                                    }
                                    FuncRobot.Wait_For_Digital_Input_On(handle, 53);//flag home
                                    FuncRobot.Wait_for_stop_motion_2(handle);
                                    PLC1.Write_DataBit_("L" + (31 + Memory_PLC.K100).ToString(), 1); //diem an toan
                                    PLC1.Write_DataBit_("L" + (39 + Memory_PLC.K100).ToString(), 1); //diem an toan
                                    PLC1.Write_DataBit_("L" + (40 + Memory_PLC.K100).ToString(), 1); //diem an toan
                                    SDKHrobot.HRobot.set_digital_output(handle, 5, false);
                                    FuncRobot.Flag_Wait_Pick_Press = false;
                                    SDKHrobot.HRobot.set_digital_output(handle, 8, false);
                                    if (SDKHrobot.HRobot.get_digital_output(handle, 51) == 1)
                                    {
                                        PLC1.Write_DataBit_("L" + (301 + Memory_PLC.K1).ToString(), 0);
                                    }
                                }
                            }
                        }//4
                        else if (Pos_current1[1] < 225)
                        {
                            double[] antoan_1 = { Global.Ready_Place_Tray_3[0], Global.Ready_Place_Tray_3[1], Pos_current1[2], Pos_current1[3], Pos_current1[4], Pos_current1[5] };
                            SDKHrobot.HRobot.lin_pos(handle, 1, 30, antoan_1);
                            SDKHrobot.HRobot.lin_pos(handle, 1, 30, Global.Ready_Place_Tray_2);
                            SDKHrobot.HRobot.lin_pos(handle, 1, 10, Global.Ready_Check_Camtop_1);
                            SDKHrobot.HRobot.lin_pos(handle, 1, 10, Global.Ready_Arc_2);
                            FuncRobot.Wait_for_stop_motion_2(handle);
                            SDKHrobot.HRobot.lin_pos(handle, 1, 10, Global.Ready_Arc_1);
                            SDKHrobot.HRobot.lin_pos(handle, 1, 10, Global.Homee);
                            FuncRobot.Wait_for_stop_motion_2(handle);
                            SDKHrobot.HRobot.get_current_position(handle, Pos_home);
                            if (Pos_home[0] == Global.Homee[0] && Pos_home[1] == Global.Homee[1] && Pos_home[2] == Global.Homee[2] && Pos_home[3] == Global.Homee[3] && Pos_home[4] == Global.Homee[4] && Pos_home[5] == Global.Homee[5])
                            {
                                SDKHrobot.HRobot.set_digital_output(handle, 1, true);// rb origin ok
                                SDKHrobot.HRobot.set_digital_output(handle, 49, false);//ROBOT 1 BUSY PICK FPCB PRESS
                            }
                            FuncRobot.Wait_For_Digital_Input_On(handle, 53);//flag home
                            FuncRobot.Wait_for_stop_motion_2(handle);
                            PLC1.Write_DataBit_("L" + (31 + Memory_PLC.K100).ToString(), 1); //diem an toan
                            PLC1.Write_DataBit_("L" + (39 + Memory_PLC.K100).ToString(), 1); //diem an toan
                            PLC1.Write_DataBit_("L" + (40 + Memory_PLC.K100).ToString(), 1); //diem an toan
                            SDKHrobot.HRobot.set_digital_output(handle, 5, false);
                            FuncRobot.Flag_Wait_Pick_Press = false;
                            SDKHrobot.HRobot.set_digital_output(handle, 8, false);
                            if (SDKHrobot.HRobot.get_digital_output(handle, 51) == 1)
                            {
                                PLC1.Write_DataBit_("L" + (301 + Memory_PLC.K1).ToString(), 0);
                            }
                        }//5
                        UInt64[] alarm_code = new UInt64[100];
                        int count = 0;
                        SDKHrobot.HRobot.get_alarm_code(handle, ref count, alarm_code);
                        if (count == 0)
                        {
                            Thread.Sleep(100);
                            // Wait_Bit_PLC("m5015", true);

                            int ss_vac_check = SDKHrobot.HRobot.get_digital_output(handle, 51);
                            if (ss_vac_check == 1)
                            {
                                if (PLC1.Read_Data_Bit_("L" + (90 + Memory_PLC.K100).ToString()) == true)//flag pick fpcb press
                                {
                                    if (SDKHrobot.HRobot.get_digital_input(handle, 1) == 0 && SDKHrobot.HRobot.get_digital_input(handle, 2) == 0 && SDKHrobot.HRobot.get_digital_input(handle, 8) == 0)
                                    {
                                        FuncRobot.CMD_Check_Marking = false;
                                        FuncRobot.CMD_Pick_Press = false;
                                        FuncRobot.CMD_Pick_Output = false;
                                        FuncRobot.CMD_Input = true;
                                        FuncRobot.CMD_Place_Tray = false;
                                    }
                                }
                            }
                            // StatusDisplay.Instance.Enable_Button(btn_Auto, 1);
                            Global.Home_All = true;
                            start_home = 0;
                            SDKHrobot.HRobot.set_override_ratio(handle, 10);
                            StatusDisplay.Instance.Update_process(txt_process, "Origin hoàn thành");
                        }

                    }
                }
            }
        }
        private void home2()
        {
            start_home++;
            int res1 = -1;
            if (start_home == 1)
            {
                StatusDisplay.Instance.Update_process(txt_process, "Origin Waiting ...");
                int sp_home = Convert.ToInt16(Global.SP_Home);
                Thread.Sleep(50);
                SDKHrobot.HRobot.set_operation_mode(handle, 0);
                SDKHrobot.HRobot.set_override_ratio(handle, sp_home);
                SDKHrobot.HRobot.set_digital_output(handle, 50, false);
                SDKHrobot.HRobot.set_digital_output(handle, 56, false);
                FuncRobot.Flag_Wait_Pick_Press = false;
                Thread.Sleep(50);
                // PLC1.Write_DataBit_("m5005", 1);
                int auto = SDKHrobot.HRobot.get_digital_input(handle, 49);
                int start = SDKHrobot.HRobot.get_digital_input(handle, 50);
                int pause = SDKHrobot.HRobot.get_digital_input(handle, 51);
                int stop = SDKHrobot.HRobot.get_digital_input(handle, 52);
                if (start == 0 & auto == 0 & pause == 0 & stop == 0)

                {
                    SDKHrobot.HRobot.clear_alarm(handle);
                    if (handle == 0)
                    {
                        SDKHrobot.HRobot.set_connection_level(handle, 1);

                        if (SDKHrobot.HRobot.get_motor_state(handle) == 0)
                        {
                            SDKHrobot.HRobot.set_motor_state(handle, 1);   // Servo on
                        }
                        double[] Pos_current1 = new double[6];
                        double[] Pos_current2 = new double[6];
                        double[] Pos_home_2 = new double[6];
                        double[] Pos_home_Joint1 = new double[6];
                        double[] Pos_home = new double[6];
                        double[] Pos_home_Mov1 = new double[6];
                        res1 = SDKHrobot.HRobot.get_current_position(handle, Pos_current1);
                        UInt64[] alarm_code = new UInt64[100];
                        int count = 0;
                        SDKHrobot.HRobot.get_alarm_code(handle, ref count, alarm_code);
                        if (count == 0 && res1 == 0)
                        {
                            if (Memory_PLC.Group == "A")
                            {
                                if (Pos_current1[0] < Global.Ready_Rotation_1[0] && Pos_current1[1] < Global.Max_RB_PlaceTray_Y + 10 && Pos_current1[5] < Global.Pick_Press[5])
                                {
                                    Pos_current2 = new double[6] { Global.Ready_Place_Tray_1[0], Pos_current1[1], Pos_current1[2], Pos_current1[3], Pos_current1[4], Pos_current1[5] };
                                    SDKHrobot.HRobot.lin_pos(handle, 0, 0, Pos_current2);
                                    Pos_home = new double[6] { Pos_current2[0], Pos_current2[1], Global.Z_antoan, Pos_current2[3], Pos_current2[4], Pos_current2[5] };
                                    SDKHrobot.HRobot.lin_pos(handle, 0, 0, Pos_home);
                                    FuncRobot.Wait_for_stop_motion(handle);
                                    SDKHrobot.HRobot.lin_pos(handle, 0, 0, Global.Homee);
                                    FuncRobot.Wait_for_stop_motion(handle);
                                    PLC1.Write_DataBit_("L" + (31 + Memory_PLC.K100), 1); //diem an toan
                                    PLC1.Write_DataBit_("L" + (39 + Memory_PLC.K100), 1); //diem an toan
                                    SDKHrobot.HRobot.set_digital_output(handle, 1, true);
                                }
                                else
                                {
                                    if (Pos_current1[5] > Global.Pick_Press[5] && Pos_current1[5] > 0)
                                    {
                                        Pos_home = new double[6] { Pos_current1[0], Pos_current1[1], Global.Z_antoan, Pos_current1[3], Pos_current1[4], Pos_current1[5] };
                                        SDKHrobot.HRobot.lin_pos(handle, 0, 0, Pos_home);
                                        Pos_home_Mov1 = new double[6] { Pos_current1[0], Pos_current1[1], Global.Z_antoan, Pos_current1[3], Pos_current1[4], Global.Pick_Press[5] };
                                        SDKHrobot.HRobot.lin_pos(handle, 0, 0, Pos_home_Mov1);
                                        FuncRobot.Wait_for_stop_motion(handle);
                                        SDKHrobot.HRobot.lin_pos(handle, 0, 0, Global.Homee);
                                        FuncRobot.Wait_for_stop_motion(handle);
                                        PLC1.Write_DataBit_("L" + (31 + Memory_PLC.K100), 1); //diem an toan
                                        PLC1.Write_DataBit_("L" + (39 + Memory_PLC.K100), 1); //diem an toan
                                        SDKHrobot.HRobot.set_digital_output(handle, 1, true);
                                    }
                                    else
                                    {
                                        Pos_home = new double[6] { Pos_current1[0], Pos_current1[1], Global.Z_antoan, Pos_current1[3], Pos_current1[4], Pos_current1[5] };
                                        SDKHrobot.HRobot.lin_pos(handle, 0, 0, Pos_home);
                                        FuncRobot.Wait_for_stop_motion(handle);
                                        SDKHrobot.HRobot.lin_pos(handle, 0, 0, Global.Homee);
                                        FuncRobot.Wait_for_stop_motion(handle);
                                        PLC1.Write_DataBit_("L" + (31 + Memory_PLC.K100), 1); //diem an toan
                                        PLC1.Write_DataBit_("L" + (39 + Memory_PLC.K100), 1); //diem an toan
                                        SDKHrobot.HRobot.set_digital_output(handle, 1, true);
                                    }
                                }
                            }
                            else
                            {
                                if (Pos_current1[0] > Global.Ready_Rotation_1[0] && Pos_current1[1] < Global.Max_RB_PlaceTray_Y + 10 && Pos_current1[5] < Global.Pick_Press[5] && Pos_current1[5] > 0)
                                {
                                    Pos_current2 = new double[6] { Global.Ready_Place_Tray_1[0], Pos_current1[1], Pos_current1[2], Pos_current1[3], Pos_current1[4], Pos_current1[5] };
                                    SDKHrobot.HRobot.lin_pos(handle, 0, 0, Pos_current2);
                                    Pos_home = new double[6] { Pos_current2[0], Pos_current2[1], Global.Z_antoan, Pos_current2[3], Pos_current2[4], Pos_current2[5] };
                                    SDKHrobot.HRobot.lin_pos(handle, 0, 0, Pos_home);
                                    FuncRobot.Wait_for_stop_motion(handle);
                                    SDKHrobot.HRobot.lin_pos(handle, 0, 0, Global.Homee);
                                    FuncRobot.Wait_for_stop_motion(handle);
                                    PLC1.Write_DataBit_("L" + (31 + Memory_PLC.K100), 1); //diem an toan
                                    PLC1.Write_DataBit_("L" + (39 + Memory_PLC.K100), 1); //diem an toan
                                    SDKHrobot.HRobot.set_digital_output(handle, 1, true);
                                }
                                else
                                {
                                    if (Pos_current1[5] < Global.Pick_Press[5] && Pos_current1[5] > 0)
                                    {
                                        Pos_home = new double[6] { Pos_current1[0], Pos_current1[1], Global.Z_antoan, Pos_current1[3], Pos_current1[4], Pos_current1[5] };
                                        SDKHrobot.HRobot.lin_pos(handle, 0, 0, Pos_home);

                                        Pos_home_Mov1 = new double[6] { Pos_current1[0], Pos_current1[1], Global.Z_antoan, Pos_current1[3], Pos_current1[4], Global.Pick_Press[5] };
                                        SDKHrobot.HRobot.lin_pos(handle, 0, 0, Pos_home_Mov1);
                                        SDKHrobot.HRobot.lin_pos(handle, 0, 0, Global.Homee);
                                        FuncRobot.Wait_for_stop_motion(handle);
                                        PLC1.Write_DataBit_("L" + (31 + Memory_PLC.K100), 1); //diem an toan
                                        PLC1.Write_DataBit_("L" + (39 + Memory_PLC.K100), 1); //diem an toan
                                        SDKHrobot.HRobot.set_digital_output(handle, 1, true);
                                    }
                                    else
                                    {
                                        Pos_home = new double[6] { Pos_current1[0], Pos_current1[1], Global.Z_antoan, Pos_current1[3], Pos_current1[4], Pos_current1[5] };
                                        SDKHrobot.HRobot.lin_pos(handle, 0, 0, Pos_home);
                                        SDKHrobot.HRobot.lin_pos(handle, 0, 0, Global.Homee);
                                        FuncRobot.Wait_for_stop_motion(handle);
                                        PLC1.Write_DataBit_("L" + (31 + Memory_PLC.K100), 1); //diem an toan
                                        PLC1.Write_DataBit_("L" + (39 + Memory_PLC.K100), 1); //diem an toan
                                        SDKHrobot.HRobot.set_digital_output(handle, 1, true);
                                    }

                                }
                            }
                        }
                        else
                        {
                            stop_change = 1;
                            stopOn();
                        }
                        if (count == 0)
                        {
                            int ss_vac_check = SDKHrobot.HRobot.get_digital_output(handle, 51);
                            if (ss_vac_check == 1)
                            {
                                FuncRobot.CMD_Check_Marking = false;
                                FuncRobot.CMD_Pick_Press = false;
                                FuncRobot.CMD_Pick_Output = false;
                                FuncRobot.CMD_Input = false;
                                FuncRobot.CMD_Place_Tray = false;
                            }
                            else
                            {
                                FuncRobot.CMD_Pick_Press = true;
                            }
                            FuncRobot.Wait_For_Digital_Input_On(handle, 53);//flag home
                                                                            // StatusDisplay.Instance.Enable_Button(btn_Auto, 1);
                            Global.Home_All = true;
                            start_home = 0;
                            SDKHrobot.HRobot.set_override_ratio(handle, 10);
                            StatusDisplay.Instance.Update_process(txt_process, "Origin hoàn thành");
                        }

                    }
                }
            }
        }
        private int[] case_set_number_current_fpcb_tray(int data)
        {
            int[] data_output = new int[3];
            switch (data)
            {
                case 1:
                    data_output[0] = data;
                    data_output[1] = data;
                    data_output[2] = data;
                    break;
                case 2:
                    data_output[0] = data;
                    data_output[1] = 1;
                    data_output[2] = data;
                    break;
                case 3:
                    data_output[0] = data;
                    data_output[1] = 1;
                    data_output[2] = data;
                    break;
                case 4:
                    data_output[0] = data;
                    data_output[1] = 1;
                    data_output[2] = data;
                    break;
                case 5:
                    data_output[0] = data;
                    data_output[1] = 1;
                    data_output[2] = data;
                    break;
                case 6:
                    data_output[0] = data;
                    data_output[1] = 1;
                    data_output[2] = data;
                    break;
                case 7:
                    data_output[0] = data;
                    data_output[1] = 1;
                    data_output[2] = data;
                    break;
                case 8:
                    data_output[0] = data;
                    data_output[1] = 1;
                    data_output[2] = data;
                    break;
                case 9:
                    data_output[0] = data;
                    data_output[1] = 1;
                    data_output[2] = data;
                    break;
                case 10:
                    data_output[0] = data;
                    data_output[1] = 1;
                    data_output[2] = data;
                    break;
                case 11:
                    data_output[0] = 11;
                    data_output[1] = 1;
                    data_output[2] = 6;
                    break;
                case 12:
                    data_output[0] = 11;
                    data_output[1] = 2;
                    data_output[2] = 7;
                    break;
                case 13:
                    data_output[0] = 11;
                    data_output[1] = 3;
                    data_output[2] = 8;
                    break;
                case 14:
                    data_output[0] = 11;
                    data_output[1] = 4;
                    data_output[2] = 9;
                    break;
                case 15:
                    data_output[0] = 11;
                    data_output[1] = 5;
                    data_output[2] = 10;
                    break;
                case 16:
                    data_output[0] = 11;
                    data_output[1] = 6;
                    data_output[2] = 11;
                    break;
                case 17:
                    data_output[0] = 11;
                    data_output[1] = 7;
                    data_output[2] = 12;
                    break;
                case 18:
                    data_output[0] = 11;
                    data_output[1] = 8;
                    data_output[2] = 13;
                    break;
                case 19:
                    data_output[0] = 11;
                    data_output[1] = 9;
                    data_output[2] = 14;
                    break;
                case 20:
                    data_output[0] = 11;
                    data_output[1] = 10;
                    data_output[2] = 20;
                    break;
                case 21:
                    data_output[0] = 11;
                    data_output[1] = 11;
                    data_output[2] = 21;
                    break;
                case 22:
                    data_output[0] = 11;
                    data_output[1] = 12;
                    data_output[2] = 22;
                    break;
                case 23:
                    data_output[0] = 11;
                    data_output[1] = 13;
                    data_output[2] = 23;
                    break;
                case 24:
                    data_output[0] = 11;
                    data_output[1] = 14;
                    data_output[2] = 24;
                    break;
                case 25:
                    data_output[0] = 11;
                    data_output[1] = 15;
                    data_output[2] = 25;
                    break;
                case 26:
                    data_output[0] = 11;
                    data_output[1] = 16;
                    data_output[2] = 26;
                    break;
                case 27:
                    data_output[0] = 11;
                    data_output[1] = 17;
                    data_output[2] = 27;
                    break;
                case 28:
                    data_output[0] = 11;
                    data_output[1] = 18;
                    data_output[2] = 28;
                    break;

            }
            return data_output;
        }
        private void Place_NG()
        {
            SDKHrobot.HRobot.get_current_position(handle, check_Pos_curr);
            if (check_Pos_curr[0] == Global.Ready_Place_Tray_1[0] && check_Pos_curr[1] == Global.Ready_Place_Tray_1[1] &&
                check_Pos_curr[2] == Global.Ready_Place_Tray_1[2] && check_Pos_curr[5] == Global.Ready_Place_Tray_1[5])
            {
                SDKHrobot.HRobot.lin_pos(handle, 1, 20, Global.Pos_NG);
                if (CheckBox_Cylinder_BoxNG.Checked == true)
                {
                    SDKHrobot.HRobot.set_digital_output(handle, 56, true);// O.Cylinder Box NG
                    StatusDisplay.Instance.Update_process(txt_process, "Wait SS For Box NG");
                    FuncRobot.Wait_For_Digital_Input_On(handle, 13);// ss forward box NG
                }
                FuncRobot.Wait_for_stop_motion_2(handle);
                SDKHrobot.HRobot.set_digital_output(handle, 51, false);// Vaccum tool robot
                if (uiRadioButton_PowerBlow1.Checked == true)
                {
                    SDKHrobot.HRobot.set_digital_output(handle, 52, true);
                }
                else if (uiRadioButton_PowerBlow2.Checked == true)
                {
                    SDKHrobot.HRobot.set_digital_output(handle, 53, true);
                }
                else if (uiRadioButton_PowerBlow1.Checked == false && uiRadioButton_PowerBlow2.Checked == false)
                {
                    SDKHrobot.HRobot.set_digital_output(handle, 52, true);
                }
                PLC1.Write_DataBit_("M" + (9081 + Memory_PLC.K1000).ToString(), 1);
                //PLC1.Write_DataBit_("L" + (91 + Memory_PLC.K100).ToString(), 0);
                FuncRobot.Flag_FPCB_All_NG = false;
                Thread.Sleep(Global.delay_up);
                if (uiRadioButton_PowerBlow1.Checked == true)
                {
                    SDKHrobot.HRobot.set_digital_output(handle, 52, false);
                }
                else if (uiRadioButton_PowerBlow2.Checked == true)
                {
                    SDKHrobot.HRobot.set_digital_output(handle, 53, false);
                }
                else if (uiRadioButton_PowerBlow1.Checked == false && uiRadioButton_PowerBlow2.Checked == false)
                {
                    SDKHrobot.HRobot.set_digital_output(handle, 52, false);
                }
                Global.result_Cam_Bot = new int[Global.Number_Tool];
                PLC1.Write_DataBit_("M" + (9083 + Memory_PLC.K1000).ToString(), 1);
                if (CheckBox_Cylinder_BoxNG.Checked == true)
                {
                    SDKHrobot.HRobot.set_digital_output(handle, 56, false);
                }
                SDKHrobot.HRobot.lin_pos(handle, 1, 20, Global.Ready_Place_Tray_1);
                FuncRobot.Wait_for_stop_motion_2(handle);
            }
        }
        private void Place_NG_2()
        {
            SDKHrobot.HRobot.get_current_position(handle, check_Pos_curr);
            if (check_Pos_curr[0] == Global.Ready_Place_Tray_1[0] && check_Pos_curr[1] == Global.Ready_Place_Tray_1[1] &&
                check_Pos_curr[2] == Global.Ready_Place_Tray_1[2] && check_Pos_curr[5] == Global.Ready_Place_Tray_1[5])
            {
                SDKHrobot.HRobot.lin_pos(handle, 1, 20, Global.Pos_NG);
                if (CheckBox_Cylinder_BoxNG.Checked == true)
                {
                    SDKHrobot.HRobot.set_digital_output(handle, 56, true);// O.Cylinder Box NG                 
                    StatusDisplay.Instance.Update_process(txt_process, "Wait SS For Box NG");
                    FuncRobot.Wait_For_Digital_Input_On(handle, 13);
                }
                FuncRobot.Wait_for_stop_motion_2(handle);
                if (uiRadioButton_PowerBlow1.Checked == true)
                {
                    SDKHrobot.HRobot.set_digital_output(handle, 52, true);
                }
                else if (uiRadioButton_PowerBlow2.Checked == true)
                {
                    SDKHrobot.HRobot.set_digital_output(handle, 53, true);
                }
                else if (uiRadioButton_PowerBlow1.Checked == false && uiRadioButton_PowerBlow2.Checked == false)
                {
                    SDKHrobot.HRobot.set_digital_output(handle, 52, true);
                }
                PLC1.Write_DataBit_("M" + (9068 + Memory_PLC.K1000).ToString(), 1);
                Thread.Sleep(Global.delay_up);
                if (uiRadioButton_PowerBlow1.Checked == true)
                {
                    SDKHrobot.HRobot.set_digital_output(handle, 52, false);
                }
                else if (uiRadioButton_PowerBlow2.Checked == true)
                {
                    SDKHrobot.HRobot.set_digital_output(handle, 53, false);
                }
                else if (uiRadioButton_PowerBlow1.Checked == false && uiRadioButton_PowerBlow2.Checked == false)
                {
                    SDKHrobot.HRobot.set_digital_output(handle, 52, false);
                }
                FuncRobot.Flag_FPCB_All_NG = false;
                for (int zi = 0; zi < Global.Number_Tool; zi++)
                {
                    if (Global.result_Cam_Bot[zi] == 1)
                    {
                        Global.result_Cam_Bot[zi] = 0;
                    }
                }
                SDKHrobot.HRobot.lin_pos(handle, 1, 20, Global.Ready_Place_Tray_1);
                if (CheckBox_Cylinder_BoxNG.Checked == true)
                {
                    SDKHrobot.HRobot.set_digital_output(handle, 56, false);
                }
                FuncRobot.Wait_for_stop_motion_2(handle);
            }
        }
        private void Load_Process_Old()
        {
            //StatusDisplay.Instance.Enable_Button(btn_Start, 0);
            //StatusDisplay.Instance.Enable_Button(btn_Auto, 0);
            SDKHrobot.HRobot.set_operation_mode(handle, Global.Mode_run_auto);
            SDKHrobot.HRobot.set_acc_dec_ratio(handle, 50);//set acc dec cho lenh ptp or lin
            double ACC_RB_0 = Convert.ToDouble(Global.ACC_RB);
            SDKHrobot.HRobot.set_acc_time(handle, ACC_RB_0);
            Thread.Sleep(50);
            SDKHrobot.HRobot.set_override_ratio(handle, Global.SP_auto_RB);
            int row = PLC1.Read_Data_Word_("D" + (3002 + Memory_PLC.K1000).ToString());
            int column = PLC1.Read_Data_Word_("D" + (3003 + Memory_PLC.K1000).ToString());
            int number_fpcb = row * column;
            stop_index = number_fpcb + 1;
            int[] counter = new int[20];
            SDKHrobot.HRobot.get_counter_value_all(handle, counter);

            if (counter[1] == 0)
            {
                SDKHrobot.HRobot.set_counter(handle, 2, 1);
            }
            else
            {
                SDKHrobot.HRobot.set_digital_output(handle, 57, false);// power transfer tray
            }
            if (counter[3] == 0)
            {
                SDKHrobot.HRobot.set_counter(handle, 3, 1);
            }
            if (counter[4] == 0)
            {
                SDKHrobot.HRobot.set_counter(handle, 4, 1);
            }
            if (counter[5] == 0)
            {
                SDKHrobot.HRobot.set_counter(handle, 5, 1);
            }
            if (counter[6] == 0)
            {
                SDKHrobot.HRobot.set_counter(handle, 6, 1);
            }

            n = SDKHrobot.HRobot.get_counter(handle, 2);
            ind_n1 = SDKHrobot.HRobot.get_counter(handle, 3);
            ind_n2 = SDKHrobot.HRobot.get_counter(handle, 4);
            ind_n3 = SDKHrobot.HRobot.get_counter(handle, 5);
            SDKHrobot.HRobot.set_counter(handle, 2, n);
            SDKHrobot.HRobot.set_counter(handle, 3, ind_n1);
            SDKHrobot.HRobot.set_counter(handle, 4, ind_n2);
            SDKHrobot.HRobot.set_counter(handle, 5, ind_n3);

            if (counter[1] >= stop_index)
            {
                if (SDKHrobot.HRobot.get_digital_output(handle, 2) == 1 || PLC1.Read_Data_Bit_("L" + (72 + Memory_PLC.K100).ToString()) == true
                    || SDKHrobot.HRobot.get_digital_input(handle, 4) == 1)// DO2 -L72 -RB CALL PLC TRANSFER TRAY - DI16 cho phep thả hàng vào tray
                {
                    if (PLC1.Read_Data_Bit_("M" + (8526 + Memory_PLC.K300).ToString()) == false && SDKHrobot.HRobot.get_digital_input(handle, 4) == 0) // di4 flag transfer tray, Bit M check ss Vaccum Pick tray
                    {
                        StatusDisplay.Instance.Update_process(txt_process, "Yêu cầu Pick tray");
                        PLC1.Write_DataBit_("M" + (9020 + Memory_PLC.K1000).ToString(), 1);//pick tray
                        StatusDisplay.Instance.Update_process(txt_process, "Wait transfer tray...");
                        FuncRobot.Wait_For_Digital_Input_On(handle, 4);// flag transfer tray hoàn thành                      
                        SDKHrobot.HRobot.set_digital_output(handle, 2, false);//Call trans tray
                        n = 1;
                        ind_n1 = 1;
                        ind_n2 = 1;
                        ind_n3 = 1;
                        SDKHrobot.HRobot.set_counter(handle, 2, n);
                        SDKHrobot.HRobot.set_counter(handle, 3, ind_n1);
                        SDKHrobot.HRobot.set_counter(handle, 4, ind_n2);
                        SDKHrobot.HRobot.set_counter(handle, 5, ind_n3);
                        StatusDisplay.Instance.Update_process(txt_process, "Transfer tray hoàn thành");
                    }
                    else if (SDKHrobot.HRobot.get_digital_input(handle, 4) == 1)// TF tray ok
                    {
                        n = 1;
                        ind_n1 = 1;
                        ind_n2 = 1;
                        ind_n3 = 1;
                        SDKHrobot.HRobot.set_counter(handle, 2, n);
                        SDKHrobot.HRobot.set_counter(handle, 3, ind_n1);
                        SDKHrobot.HRobot.set_counter(handle, 4, ind_n2);
                        SDKHrobot.HRobot.set_counter(handle, 5, ind_n3);
                        StatusDisplay.Instance.Update_process(txt_process, "Wait transfer tray...");
                        FuncRobot.Wait_For_Digital_Input_On(handle, 4);// flag transfer tray hoàn thành
                        StatusDisplay.Instance.Update_process(txt_process, "Transfer tray hoàn thành");
                        PLC1.Write_DataBit_("L" + (30 + Memory_PLC.K100).ToString(), 0);
                    }

                }
                else if (SDKHrobot.HRobot.get_digital_output(handle, 2) == 0 || PLC1.Read_Data_Bit_("L" + (72 + Memory_PLC.K100).ToString()) == false || SDKHrobot.HRobot.get_digital_input(handle, 4) == 0)   // DO2-L72 call trans tray , DI4 Flag trans tray           
                {
                    StatusDisplay.Instance.Update_process(txt_process, "Wait transfer tray...");
                    PLC1.Write_DataBit_("L" + (74 + Memory_PLC.K100).ToString(), 1);
                    PLC1.Write_DataBit_("L" + (72 + Memory_PLC.K100).ToString(), 1);
                    FuncRobot.Wait_For_Digital_Input_On(handle, 4);
                    PLC1.Write_DataBit_("L" + (74 + Memory_PLC.K100).ToString(), 0);
                    StatusDisplay.Instance.Update_process(txt_process, "Transfer tray hoàn thành");
                    n = 1;
                    ind_n1 = 1;
                    ind_n2 = 1;
                    ind_n3 = 1;
                    SDKHrobot.HRobot.set_counter(handle, 2, n);
                    SDKHrobot.HRobot.set_counter(handle, 3, ind_n1);
                    SDKHrobot.HRobot.set_counter(handle, 4, ind_n2);
                    SDKHrobot.HRobot.set_counter(handle, 5, ind_n3);
                }
            }
        }
        private void Check_Status_MC()
        {
            //MC1
            //DI1 FLAG FPCB BUFFER INPUT
            //DI2 FLAG FPCB TOOL CHECK VISION CAM BOT
            //DI3 FLAG FPCB  REMOV TAPE 
            //DI5 SMEMA CALL CHECK MARKING AFTER REMOV COVER TAPE
            //DI6 FLAG FPCB REMOV TAPE INPUT
            //DI7 SMEMA CALL ROBOT 1 PICK FPCB PRESS
            //DI9 SMEMA CALL INPUT FPCB
            //DI16 CMD-RB pick/insert
            //DO51 power vaccum tool rb
            if (Global.combox_mode == 0)
            {
                StatusDisplay.Instance.Enable_Button(btn_CMD_Pick, 1);
            }
            StatusDisplay.Instance.Update_process(txt_process, "Search Status Machine");
            while (stop_change != 1 && SDKHrobot.HRobot.get_digital_input(handle, 50) == 1 && FuncRobot.Brake_While == false && PLC1.Read_Data_Bit_("L" + (91 + Memory_PLC.K100).ToString()) == false)
            {
                if (SDKHrobot.HRobot.get_digital_input(handle, 7) == 1 && SDKHrobot.HRobot.get_digital_output(handle, 51) == 0 && SDKHrobot.HRobot.get_digital_input(handle, 1) == 0 &&
                    SDKHrobot.HRobot.get_digital_output(handle, 6) == 0 && FuncRobot.CMD_Pick_Press == false && FuncRobot.CMD_Pick_Output == false && Global.combox_mode == 1)
                {
                    //PICK FPCB PRESS
                    StatusDisplay.Instance.Update_process(txt_process, "CMD Pick Press");
                    FuncRobot.CMD_Pick_Press = true;
                    FuncRobot.Brake_While = true;
                }
                else if (SDKHrobot.HRobot.get_digital_input(handle, 6) == 1 && SDKHrobot.HRobot.get_digital_output(handle, 51) == 0 && SDKHrobot.HRobot.get_digital_input(handle, 5) == 0)
                {
                    StatusDisplay.Instance.Update_process(txt_process, "CMD Pick Output ");
                    FuncRobot.CMD_Pick_Output = true;
                    FuncRobot.CMD_Pick_Press = false;
                    FuncRobot.CMD_Check_Marking = false;
                    FuncRobot.Brake_While = true;
                    SDKHrobot.HRobot.set_digital_output(handle, 5, false);//call press
                    FuncRobot.Flag_Wait_Pick_Press = false;
                    PLC1.Write_DataBit_("L" + (301 + Memory_PLC.K1).ToString(), 0);
                }
                else if ((FuncRobot.CMD_Pick_Output == false && SDKHrobot.HRobot.get_digital_input(handle, 5) == 1 && (SDKHrobot.HRobot.get_digital_input(handle, 1) == 1 || SDKHrobot.HRobot.get_digital_input(handle, 2) == 1) && SDKHrobot.HRobot.get_digital_output(handle, 51) == 0)
                   || (SDKHrobot.HRobot.get_digital_input(handle, 5) == 1 && SDKHrobot.HRobot.get_digital_output(handle, 51) == 1 && SDKHrobot.HRobot.get_digital_input(handle, 1) == 0))
                {
                    StatusDisplay.Instance.Update_process(txt_process, "CMD Check Marking");
                    FuncRobot.CMD_Check_Marking = true;
                    FuncRobot.Brake_While = true;
                }
                else if (FuncRobot.CMD_Pick_Press == false && FuncRobot.CMD_Check_Marking == false && FuncRobot.CMD_Pick_Output == false
                    && SDKHrobot.HRobot.get_digital_output(handle, 51) == 0 && Global.combox_mode == 1 && FuncRobot.Flag_Wait_Pick_Press == false)
                {
                    StatusDisplay.Instance.Update_process(txt_process, "Wait Pick Press");
                    SDKHrobot.HRobot.set_digital_output(handle, 5, true);//call press
                    FuncRobot.Flag_Wait_Pick_Press = true;
                }
                else if (FuncRobot.CMD_Pick == true && FuncRobot.CMD_Pick_Output == false && Global.combox_mode == 0)
                {
                    FuncRobot.Brake_While = true;
                }
                Machine_Pause_(stop_change);
            }
            //else if (SDKHrobot.HRobot.get_digital_output(handle, 51) == 0 && SDKHrobot.HRobot.get_digital_output(handle, 2) == 0 && SDKHrobot.HRobot.get_digital_output(handle, 1) == 0 && FuncRobot.CMD_Pick_Press == false)
            //{
            //    FuncRobot.CMD_Pick_Press = true;
            //}
            //else if (SDKHrobot.HRobot.get_digital_output(handle, 51) == 1 && SDKHrobot.HRobot.get_digital_output(handle, 9) == 1 && SDKHrobot.HRobot.get_digital_output(handle, 2) == 0
            //    && SDKHrobot.HRobot.get_digital_output(handle, 1) == 0)
            //{
            //    if (SDKHrobot.HRobot.get_digital_output(handle, 6) == 1)
            //    {
            //        FuncRobot.Wait_For_Digital_Input_On(handle, 3);
            //        FuncRobot.CMD_Input = true;
            //        FuncRobot.CMD_Check_Marking = true;
            //    }
            //    else
            //    {
            //        FuncRobot.CMD_Input = true;
            //        FuncRobot.CMD_Check_Marking = false;
            //    }
            //    //Input FPCB Jig Input

            //}           
        }
        private void Mov_pick_press()
        {
            SDKHrobot.HRobot.set_digital_output(handle, 49, true);//ROBOT 1 BUSY PICK FPCB PRESS
            double[] current_pick_press = new double[6];
            SDKHrobot.HRobot.get_current_position(handle, current_pick_press);
            if (current_pick_press[0] == Global.Homee[0] && current_pick_press[1] == Global.Homee[1] && current_pick_press[2] == Global.Homee[2] && current_pick_press[5] == Global.Homee[5] && stop_change != 1)
            {
                StatusDisplay.Instance.Update_process(txt_process, "Pick Fpcb Press Start");
                StatusDisplay.Instance.Update_process(txt_process, "SS Up Cam Top ");
                FuncRobot.Wait_For_Digital_Input_On(handle, 56);
                SDKHrobot.HRobot.lin_pos(handle, 1, 20, Global.Ready_Pick_Press_1);
                FuncRobot.Wait_for_stop_motion_2(handle);
                Machine_Pause_(stop_change);

                //SDKHrobot.HRobot.lin_pos(handle, 1, 20, Ready_Pick_Press_2);
                //SDKHrobot.HRobot.lin_pos(handle, 1, 20, Ready_Rotation_1);
                double[] Z_Wait_pick_press = { Global.Z_Pick_Press[0], Global.Z_Pick_Press[1], Global.Z_Pick_Press[2] + Global.Offset_Z, Global.Z_Pick_Press[3], Global.Z_Pick_Press[4], Global.Z_Pick_Press[5] };
                //FuncRobot.Wait_for_stop_motion_2(handle);
                SDKHrobot.HRobot.lin_pos(handle, 1, 30, Global.Pick_Press);
                FuncRobot.Wait_for_stop_motion_2(handle);
                SDKHrobot.HRobot.ptp_pos(handle, 1, Z_Wait_pick_press);
                SDKHrobot.HRobot.set_override_ratio(handle, Global.SP_Wait_Pick);
                FuncRobot.Wait_for_stop_motion_2(handle);
                SDKHrobot.HRobot.ptp_pos(handle, 1, Global.Z_Pick_Press);
                FuncRobot.Wait_for_stop_motion_2(handle);
                SDKHrobot.HRobot.set_digital_output(handle, 51, true);
                PLC1.Write_DataBit_("M" + (9080 + Memory_PLC.K1000).ToString(), 1);
                Thread.Sleep(Global.delay_hut);
                SDKHrobot.HRobot.lin_pos(handle, 1, 10, Z_Wait_pick_press);
                FuncRobot.Wait_for_stop_motion_2(handle);
                SDKHrobot.HRobot.set_override_ratio(handle, Global.SP_auto_RB);
                PLC1.Write_DataBit_("L" + (90 + Memory_PLC.K100).ToString(), 1);//flag fpcb pick press
                FuncRobot.Flag_FPCB_After_Pick_Press_RB = true;
                FuncRobot.CMD_Pick_Press = false;
                SDKHrobot.HRobot.lin_pos(handle, 1, 10, Global.Pick_Press);
                FuncRobot.Wait_for_stop_motion(handle);
                //SDKHrobot.HRobot.lin_pos(handle, 1, 10, Ready_Rotation_1);
                //SDKHrobot.HRobot.lin_pos(handle, 1, 10, Ready_Pick_Press_2);
                //FuncRobot.Wait_for_stop_motion_2(handle);
                SDKHrobot.HRobot.lin_pos(handle, 1, 10, Global.Ready_Pick_Press_1);
                FuncRobot.Wait_for_stop_motion(handle);
                SDKHrobot.HRobot.get_current_position(handle, check_pos);
                if (check_pos[0] == Global.Ready_Pick_Press_1[0] && check_pos[1] == Global.Ready_Pick_Press_1[1] && check_pos[2] == Global.Ready_Pick_Press_1[2] && check_pos[5] == Global.Ready_Pick_Press_1[5])
                {
                    SDKHrobot.HRobot.lin_pos(handle, 1, 10, Global.Homee);
                }
                else
                {
                    Process_Auto("Error position Ready_Pick_press 1", file_process_auto);
                    stop_change = 1;
                    stopOn();
                }
                FuncRobot.Wait_for_stop_motion(handle);
                SDKHrobot.HRobot.get_current_position(handle, check_pos);
                if (check_pos[0] == Global.Homee[0] && check_pos[1] == Global.Homee[1] && check_pos[2] == Global.Homee[2] && check_pos[5] == Global.Homee[5])
                {
                    SDKHrobot.HRobot.set_digital_output(handle, 8, true);//SMEMA COMPLATE PICK FPCB PRESS
                    Thread.Sleep(150);
                    SDKHrobot.HRobot.set_digital_output(handle, 49, false);//ROBOT 1 BUSY PICK FPCB PRESS
                    SDKHrobot.HRobot.set_digital_output(handle, 8, false);//SMEMA COMPLATE PICK FPCB PRESS
                    SDKHrobot.HRobot.set_digital_output(handle, 5, false);//call pick press
                    FuncRobot.Flag_Wait_Pick_Press = false;
                    PLC1.Write_DataBit_("L" + (301 + Memory_PLC.K1).ToString(), 0);
                    StatusDisplay.Instance.Update_process(txt_process, "Pick Fpcb Press End");
                }
            }
            else
            {
                Process_Auto("Error position Home", file_process_auto);
                stop_change = 1;
                stopOn();
            }
        }
        private void Mov_Input_FBCP()
        {
            if (SDKHrobot.HRobot.get_digital_input(handle, 9) == 1 && stop_change != 1 && SDKHrobot.HRobot.get_digital_output(handle, 51) == 1)
            {
                StatusDisplay.Instance.Update_process(txt_process, "Mov Input FPCB");
                StatusDisplay.Instance.Update_process(txt_process, "SS Up Cam Top ");
                FuncRobot.Wait_For_Digital_Input_On(handle, 56);
                FuncRobot.Wait_For_Digital_Input_On(handle, 16);
                PLC1.Write_DataBit_("L" + (39 + Memory_PLC.K100).ToString(), 0);
                PLC1.Write_DataBit_("L" + (40 + Memory_PLC.K100).ToString(), 0);//satefy Group remov
                SDKHrobot.HRobot.lin_pos(handle, 1, 10, Global.Ready_Arc_1);
                SDKHrobot.HRobot.lin_pos(handle, 1, 10, Global.Ready_Arc_2);
                FuncRobot.Wait_for_stop_motion_2(handle);
                FuncRobot.Wait_For_Digital_Input_On(handle, 9);//SMEMA CALL INPUT FPCB TO JIG REMOV 
                SDKHrobot.HRobot.lin_pos(handle, 1, 20, Global.Ready_Inputput_1);
                FuncRobot.Wait_for_stop_motion_2(handle);
                SDKHrobot.HRobot.lin_pos(handle, 1, 20, Global.Ready_Inputput_2);
                FuncRobot.Wait_for_stop_motion_2(handle);
                SDKHrobot.HRobot.lin_pos(handle, 1, 30, Global.Input_FPCB);
                FuncRobot.Wait_for_stop_motion_2(handle);
                double[] Z_Wait_Input_Fpcb = { Global.Z_Input_FPCB[0], Global.Z_Input_FPCB[1], Global.Z_Input_FPCB[2] + Global.Offset_Z, Global.Z_Input_FPCB[3], Global.Z_Input_FPCB[4], Global.Z_Input_FPCB[5] };
                SDKHrobot.HRobot.lin_pos(handle, 1, 10, Z_Wait_Input_Fpcb);
                FuncRobot.Wait_for_stop_motion_2(handle);
                SDKHrobot.HRobot.set_override_ratio(handle, Global.SP_Wait_Pick);
                SDKHrobot.HRobot.lin_pos(handle, 1, 30, Global.Z_Input_FPCB);
                FuncRobot.Wait_for_stop_motion_2(handle);
                SDKHrobot.HRobot.set_override_ratio(handle, Global.SP_auto_RB);
                SDKHrobot.HRobot.set_digital_output(handle, 51, false); //VACCUM ALL TOOL ROBOT 1 -40
                PLC1.Write_DataBit_("M" + (9081 + Memory_PLC.K1000).ToString(), 1);// select blow all
                SDKHrobot.HRobot.set_digital_output(handle, 52, true);//blow ALL TOOL ROBOT 1 -40
                SDKHrobot.HRobot.set_digital_output(handle, 4, true);//call pick FPCB check vision
                PLC1.Write_DataBit_("M" + (7073 + Memory_PLC.K500).ToString(), 1);//Vaccum jig input
                PLC1.Write_Data_Word_("D" + (7052 + Memory_PLC.K100).ToString(), 1);
                PLC1.Write_DataBit_("L" + (90 + Memory_PLC.K100).ToString(), 0);// Flag FPCB Pick Press
                FuncRobot.CMD_Pick = false;
                Thread.Sleep(Global.delay_tha);
                SDKHrobot.HRobot.set_digital_output(handle, 52, false);
                PLC1.Write_Data_Word_("D" + (7052 + Memory_PLC.K100).ToString(), 0);
                SDKHrobot.HRobot.lin_pos(handle, 1, 30, Z_Wait_Input_Fpcb);
                FuncRobot.Wait_for_stop_motion_2(handle);
                SDKHrobot.HRobot.lin_pos(handle, 1, 30, Global.Input_FPCB);
                FuncRobot.Wait_for_stop_motion_2(handle);
                SDKHrobot.HRobot.lin_pos(handle, 1, 20, Global.Ready_Inputput_2);
                SDKHrobot.HRobot.lin_pos(handle, 1, 20, Global.Ready_Inputput_1);
                FuncRobot.Wait_for_stop_motion_2(handle);
                Thread.Sleep(100);
                SDKHrobot.HRobot.set_digital_output(handle, 4, false);
                StatusDisplay.Instance.Update_process(txt_process, "Input Complt");
            }

        }
        private void Mov_Check_Maring_Tape()
        {
            if (SDKHrobot.HRobot.get_digital_input(handle, 5) == 1 && stop_change != 1)
            {
                FuncRobot.Wait_For_Digital_Input_On(handle, 5);//SMEMA CALL CHECK MARKING
                FuncRobot.Wait_For_Digital_Input_On(handle, 16);                            //
                double[] current_Check_Marking = new double[6];
                SDKHrobot.HRobot.get_current_position(handle, current_Check_Marking);
                if (((current_Check_Marking[0] == Global.Ready_Check_Camtop_1[0] && current_Check_Marking[1] == Global.Ready_Check_Camtop_1[1]
                    && current_Check_Marking[2] == Global.Ready_Check_Camtop_1[2] && current_Check_Marking[5] == Global.Ready_Check_Camtop_1[5])
                  || (current_Check_Marking[0] == Global.Ready_Inputput_1[0] && current_Check_Marking[1] == Global.Ready_Inputput_1[1]
                    && current_Check_Marking[2] == Global.Ready_Inputput_1[2] && current_Check_Marking[5] == Global.Ready_Inputput_1[5])) && stop_change != 1)
                {
                    StatusDisplay.Instance.Update_process(txt_process, "Check Marking Start");
                    PLC1.Write_DataBit_("L" + (39 + Memory_PLC.K100).ToString(), 0);
                    PLC1.Write_DataBit_("L" + (40 + Memory_PLC.K100).ToString(), 0); //diem an toan
                    SDKHrobot.HRobot.lin_pos(handle, 1, 20, Global.Ready_Check_Camtop_2);
                    SDKHrobot.HRobot.set_digital_output(handle, 50, true);
                    PLC1.Write_DataBit_("M" + (7039 + Memory_PLC.K500).ToString(), 1);
                    FuncRobot.Wait_for_stop_motion_2(handle);
                    for (int i = 0; i < Global.Total_Check_Marking; i++)
                    {
                        double[] pos_camtop_n = new double[6];
                        pos_camtop_n = matrix.PAL_P_RB_Cam_Top(1, i + 1);
                        if (pos_camtop_n[1] == Global.Check_Marking_Start[1] && pos_camtop_n[2] == Global.Check_Marking_Start[2] && matrix.Flag_Read_Data_Maxtrix_Tool_RB == true && stop_change != 1)
                        {
                            if (pos_camtop_n[0] < Global.X_Camtop_Satefy_U && pos_camtop_n[0] > Global.X_Camtop_Satefy_L && pos_camtop_n[1] < Global.Y_Camtop_Satefy_U && pos_camtop_n[1] > Global.Y_Camtop_Satefy_L
                                && pos_camtop_n[3] > Global.Z_Camtop_Satefy)
                            {
                                SDKHrobot.HRobot.lin_pos(handle, 1, 50, pos_camtop_n);
                                FuncRobot.Wait_for_stop_motion_2(handle);
                                if (i == 0)
                                {
                                    StatusDisplay.Instance.Update_process(txt_process, "SS Down Cam Top");
                                    FuncRobot.Wait_For_Digital_Input_On(handle, 57);
                                    StatusDisplay.Instance.Update_process(txt_process, "SS Up Jig Remov");
                                    FuncRobot.Wait_For_Digital_Input_On(handle, 60);
                                    Thread.Sleep(300);
                                    Send_data_Cam1("100");// lan 1 cam 1
                                    Send_data_Cam2("200");// lan 1 cam 2
                                }
                                else if (i == 1)
                                {
                                    StatusDisplay.Instance.Update_process(txt_process, "SS Down Cam Top");
                                    FuncRobot.Wait_For_Digital_Input_On(handle, 57);
                                    StatusDisplay.Instance.Update_process(txt_process, "SS Up Jig Remov");
                                    FuncRobot.Wait_For_Digital_Input_On(handle, 60);
                                    Thread.Sleep(300);
                                    Send_data_Cam1("110");
                                    Send_data_Cam2("210");
                                }
                                else if (i == 2)
                                {
                                    StatusDisplay.Instance.Update_process(txt_process, "SS Down Cam Top");
                                    FuncRobot.Wait_For_Digital_Input_On(handle, 57);
                                    StatusDisplay.Instance.Update_process(txt_process, "SS Up Jig Remov");
                                    FuncRobot.Wait_For_Digital_Input_On(handle, 60);
                                    Thread.Sleep(300);
                                    Send_data_Cam1("120");
                                    Send_data_Cam2("220");
                                }
                                else if (i == 3)
                                {
                                    StatusDisplay.Instance.Update_process(txt_process, "SS Down Cam Top");
                                    FuncRobot.Wait_For_Digital_Input_On(handle, 57);
                                    StatusDisplay.Instance.Update_process(txt_process, "SS Up Jig Remov");
                                    FuncRobot.Wait_For_Digital_Input_On(handle, 60);
                                    Thread.Sleep(300);
                                    Send_data_Cam1("130");
                                    Send_data_Cam2("230");
                                }
                                StatusDisplay.Instance.Update_process(txt_process, "Trigger Camera Tool: " + (i + 1).ToString());
                                FuncRobot.Wait_For_Flag_Cam();
                                FuncRobot.Flag_Cam1 = 0;
                                FuncRobot.Flag_Cam2 = 0;
                            }
                            else
                            {
                                Process_Auto("Error position Check_Marking_Start", file_process_auto);
                                stop_change = 1;
                                stopOn();
                            }
                        }
                        else
                        {
                            Process_Auto("Error position Check_Marking_Start", file_process_auto);
                            stop_change = 1;
                            stopOn();
                        }
                    }
                    PLC1.Write_DataBit_("L" + (95 + Memory_PLC.K100).ToString(), 1);// check marking complt > pick output
                    PLC1.Write_DataBit_("L" + (32 + Memory_PLC.K100).ToString(), 0);// rst check marking 
                    StatusDisplay.Instance.Update_process(txt_process, "Check Marking and Tape Complete");
                    FuncRobot.CMD_Pick_Output = true;
                }
                else
                {
                    Process_Auto(" position Ready_Check_Marking đang ở home", file_process_auto);
                    //stop_change = 1;
                    //stopOn();
                }
                if (current_Check_Marking[0] == Global.Homee[0] && current_Check_Marking[1] == Global.Homee[1] && current_Check_Marking[2] == Global.Homee[2] && current_Check_Marking[5] == Global.Homee[5])
                {
                    StatusDisplay.Instance.Update_process(txt_process, "Check Marking Start");
                    PLC1.Write_DataBit_("L" + (39 + Memory_PLC.K100).ToString(), 0);
                    PLC1.Write_DataBit_("L" + (40 + Memory_PLC.K100).ToString(), 0); //diem an toan
                    SDKHrobot.HRobot.lin_pos(handle, 1, 20, Global.Ready_Arc_1);
                    SDKHrobot.HRobot.lin_pos(handle, 1, 20, Global.Ready_Arc_2);
                    FuncRobot.Wait_for_stop_motion(handle);
                    SDKHrobot.HRobot.lin_pos(handle, 1, 20, Global.Ready_Check_Camtop_1);
                    SDKHrobot.HRobot.lin_pos(handle, 1, 20, Global.Ready_Check_Camtop_2);
                    SDKHrobot.HRobot.set_digital_output(handle, 50, true);
                    FuncRobot.Wait_for_stop_motion_2(handle);
                    PLC1.Write_DataBit_("M" + (7039 + Memory_PLC.K500).ToString(), 1);
                    for (int i = 0; i < Global.Total_Check_Marking; i++)
                    {
                        double[] pos_camtop_n = new double[6];
                        pos_camtop_n = matrix.PAL_P_RB_Cam_Top(1, i + 1);
                        if (pos_camtop_n[1] == Global.Check_Marking_Start[1] && pos_camtop_n[2] == Global.Check_Marking_Start[2] && matrix.Flag_Read_Data_Maxtrix_Tool_RB == true && stop_change != 1)
                        {
                            if (pos_camtop_n[0] < Global.X_Camtop_Satefy_U && pos_camtop_n[0] > Global.X_Camtop_Satefy_L && pos_camtop_n[1] < Global.Y_Camtop_Satefy_U && pos_camtop_n[1] > Global.Y_Camtop_Satefy_L
                                && pos_camtop_n[3] > Global.Z_Camtop_Satefy)
                            {
                                SDKHrobot.HRobot.lin_pos(handle, 1, 50, pos_camtop_n);
                                FuncRobot.Wait_for_stop_motion_2(handle);
                                if (i == 0)
                                {
                                    StatusDisplay.Instance.Update_process(txt_process, "SS Down Cam Top");
                                    FuncRobot.Wait_For_Digital_Input_On(handle, 57);
                                    Send_data_Cam1("100");// lan 1 cam 1
                                    Send_data_Cam2("200");// lan 1 cam 2
                                }
                                else if (i == 1)
                                {
                                    StatusDisplay.Instance.Update_process(txt_process, "SS Down Cam Top");
                                    FuncRobot.Wait_For_Digital_Input_On(handle, 57);
                                    Send_data_Cam1("110");
                                    Send_data_Cam2("210");
                                }
                                else if (i == 2)
                                {
                                    StatusDisplay.Instance.Update_process(txt_process, "SS Down Cam Top");
                                    FuncRobot.Wait_For_Digital_Input_On(handle, 57);
                                    Send_data_Cam1("120");
                                    Send_data_Cam2("220");
                                }
                                else if (i == 3)
                                {
                                    StatusDisplay.Instance.Update_process(txt_process, "SS Down Cam Top");
                                    FuncRobot.Wait_For_Digital_Input_On(handle, 57);
                                    Send_data_Cam1("130");
                                    Send_data_Cam2("230");
                                }
                                StatusDisplay.Instance.Update_process(txt_process, "Trigger Camera Tool: " + i + 1.ToString());
                                StatusDisplay.Instance.Update_process(txt_process, "Cam Top Wait...");
                                FuncRobot.Wait_For_Flag_Cam();
                                FuncRobot.Flag_Cam1 = 0;
                                FuncRobot.Flag_Cam2 = 0;
                            }
                            else
                            {
                                Process_Auto("Error position Check_Marking_Start", file_process_auto);
                                stop_change = 1;
                                stopOn();
                            }
                        }
                        else
                        {
                            Process_Auto("Error position Check_Marking_Start", file_process_auto);
                            stop_change = 1;
                            stopOn();
                        }
                    }
                    PLC1.Write_DataBit_("L" + (95 + Memory_PLC.K100).ToString(), 1);// check marking complt > pick output
                    PLC1.Write_DataBit_("L" + (32 + Memory_PLC.K100).ToString(), 0);// rst check marking 
                    FuncRobot.CMD_Pick_Output = true;
                }
                FuncRobot.CMD_Check_Marking = false;
                SDKHrobot.HRobot.set_digital_output(handle, 50, false);
                SDKHrobot.HRobot.lin_pos(handle, 1, 30, Global.Ready_Check_Camtop_2);
                SDKHrobot.HRobot.lin_pos(handle, 1, 30, Global.Ready_Check_Camtop_1);
                FuncRobot.Wait_for_stop_motion_2(handle);
                StatusDisplay.Instance.Update_process(txt_process, "Check Marking and Tape Complete");
            }
            else
            {
                FuncRobot.CMD_Check_Marking = false;
            }
        }
        private void Mov_Pick_FPCB_Output()
        {
            Process_Auto("Pick FPCB Output Start", file_process_auto);
            double[] current_Check_Pick_FPCB_output = new double[6];
            SDKHrobot.HRobot.get_current_position(handle, current_Check_Pick_FPCB_output);
            if (SDKHrobot.HRobot.get_digital_input(handle, 8) == 1 && stop_change != 1)
            {
                if (PLC1.Read_Data_Word_("D" + (9018 + Memory_PLC.K2000).ToString()) == 2 && stop_change != 1)
                {
                    if ((current_Check_Pick_FPCB_output[0] == Global.Ready_Check_Camtop_1[0] && current_Check_Pick_FPCB_output[1] == Global.Ready_Check_Camtop_1[1] &&
                        current_Check_Pick_FPCB_output[2] == Global.Ready_Check_Camtop_1[2] && current_Check_Pick_FPCB_output[5] == Global.Ready_Check_Camtop_1[5]))
                    {
                        Global.result_Cam_Bot = new int[40];
                        Global.CMD_Scan_Flag = true;
                        StatusDisplay.Instance.Update_process(txt_process, "Pick Buffer out 1");
                        if (n >= stop_index)
                        {
                            PLC1.Write_DataBit_("L" + (31 + Memory_PLC.K100).ToString(), 1);
                        }
                        StatusDisplay.Instance.Update_process(txt_process, "SS Up Cam Top ");
                        FuncRobot.Wait_For_Digital_Input_On(handle, 56);
                        SDKHrobot.HRobot.lin_pos(handle, 1, 30, Global.Ready_Pick_Output_1);
                        FuncRobot.Wait_for_stop_motion_2(handle);
                        SDKHrobot.HRobot.lin_pos(handle, 1, 10, Global.Ready_Pick_Output_2);
                        SDKHrobot.HRobot.lin_pos(handle, 1, 10, Global.Pick_FPCB_Output_1);
                        StatusDisplay.Instance.Update_process(txt_process, "SS Up Jig Remov");
                        FuncRobot.Wait_For_Digital_Input_On(handle, 60); //ss up jig remove
                        FuncRobot.Wait_for_stop_motion_2(handle);
                        double[] Z_Wait_Pick_Output = { Global.Z_Pick_FPCB_Output_1[0], Global.Z_Pick_FPCB_Output_1[1], Global.Z_Pick_FPCB_Output_1[2] + Global.Offset_Z, Global.Z_Pick_FPCB_Output_1[3], Global.Z_Pick_FPCB_Output_1[4], Global.Z_Pick_FPCB_Output_1[5] };
                        SDKHrobot.HRobot.lin_pos(handle, 1, 10, Z_Wait_Pick_Output);
                        FuncRobot.Wait_for_stop_motion_2(handle);
                        SDKHrobot.HRobot.set_override_ratio(handle, Global.SP_Wait_Pick);
                        SDKHrobot.HRobot.lin_pos(handle, 1, 30, Global.Z_Pick_FPCB_Output_1);
                        FuncRobot.Wait_for_stop_motion_2(handle);
                        SDKHrobot.HRobot.set_digital_output(handle, 52, false);
                        SDKHrobot.HRobot.set_digital_output(handle, 51, true);
                        PLC1.Write_DataBit_("M" + (9410 + Memory_PLC.K1000).ToString(), 1);//set
                                                                                           //PLC1.Write_DataBit_("M" + (9080 + Memory_PLC.K1000).ToString(), 1);
                                                                                           //PLC1.Write_DataBit_("M" + (7071 + Memory_PLC.K500).ToString(), 1);//below jig remov
                                                                                           //PLC1.Write_DataBit_("L" + (94 + Memory_PLC.K100).ToString(), 1);//read data Vision
                                                                                           //PLC1.Write_DataBit_("L" + (91 + Memory_PLC.K100).ToString(), 1);//Flag FPCB Place
                        Thread.Sleep(Global.delay_hut);
                        FuncRobot.Flag_FPCB_After_Pick_Output_RB = true;
                        PLC1.Write_Data_Word_("D" + (9018 + Memory_PLC.K2000).ToString(), 1);
                        SDKHrobot.HRobot.set_override_ratio(handle, Global.SP_auto_RB);
                        SDKHrobot.HRobot.lin_pos(handle, 1, 30, Global.Pick_FPCB_Output_1);
                        FuncRobot.Wait_for_stop_motion_2(handle);
                        PLC1.Write_DataBit_("M" + (9411 + Memory_PLC.K1000).ToString(), 1);//reset
                                                                                           //PLC1.Write_DataBit_("M" + (7067 + Memory_PLC.K500).ToString(), 1);//Vaccum jig remov 
                                                                                           //PLC1.Write_DataBit_("M" + (7037 + Memory_PLC.K500).ToString(), 1);//down jig remove
                        SDKHrobot.HRobot.lin_pos(handle, 1, 30, Global.Ready_Pick_Output_2);
                        SDKHrobot.HRobot.lin_pos(handle, 1, 30, Global.Ready_Pick_Output_1);
                        FuncRobot.Wait_for_stop_motion_2(handle);
                        StatusDisplay.Instance.Update_process(txt_process, "Wait...");
                        FuncRobot.Wait_For_Digital_Input_On(handle, 16);//CMD-RB pick/insert 
                        Process_Auto("Pick FPCB Output 1 End", file_process_auto);
                    }
                    else
                    {
                        Process_Auto("Error position Ready pick output", file_process_auto);
                        stop_change = 1;
                        stopOn();
                    }
                    if (current_Check_Pick_FPCB_output[0] == Global.Homee[0] && current_Check_Pick_FPCB_output[1] == Global.Homee[1] &&
                       current_Check_Pick_FPCB_output[2] == Global.Homee[2] && current_Check_Pick_FPCB_output[5] == Global.Homee[5])
                    {
                        Global.result_Cam_Bot = new int[40];
                        Global.CMD_Scan_Flag = true;
                        StatusDisplay.Instance.Update_process(txt_process, "SS Up Cam Top ");
                        FuncRobot.Wait_For_Digital_Input_On(handle, 56);
                        SDKHrobot.HRobot.lin_pos(handle, 1, 20, Global.Ready_Arc_1);
                        SDKHrobot.HRobot.lin_pos(handle, 1, 20, Global.Ready_Arc_2);
                        FuncRobot.Wait_for_stop_motion_2(handle);
                        SDKHrobot.HRobot.lin_pos(handle, 1, 20, Global.Ready_Pick_Output_1);
                        SDKHrobot.HRobot.lin_pos(handle, 1, 20, Global.Ready_Pick_Output_2);
                        FuncRobot.Wait_for_stop_motion_2(handle);
                        SDKHrobot.HRobot.lin_pos(handle, 1, 10, Global.Pick_FPCB_Output_1);
                        if (SDKHrobot.HRobot.get_digital_input(handle, 60) == 0)
                        {
                            PLC1.Write_DataBit_("M" + (7039 + Memory_PLC.K500), 1);
                        }
                        FuncRobot.Wait_For_Digital_Input_On(handle, 60); //ss up jig remove
                        FuncRobot.Wait_for_stop_motion_2(handle);
                        double[] Z_Wait_Pick_Output = { Global.Z_Pick_FPCB_Output_1[0], Global.Z_Pick_FPCB_Output_1[1], Global.Z_Pick_FPCB_Output_1[2] + Global.Offset_Z, Global.Z_Pick_FPCB_Output_1[3], Global.Z_Pick_FPCB_Output_1[4], Global.Z_Pick_FPCB_Output_1[5] };
                        SDKHrobot.HRobot.lin_pos(handle, 1, 10, Z_Wait_Pick_Output);
                        FuncRobot.Wait_for_stop_motion_2(handle);
                        SDKHrobot.HRobot.set_override_ratio(handle, Global.SP_Wait_Pick);
                        SDKHrobot.HRobot.lin_pos(handle, 1, 30, Global.Z_Pick_FPCB_Output_1);
                        FuncRobot.Wait_for_stop_motion_2(handle);
                        SDKHrobot.HRobot.set_digital_output(handle, 52, false);
                        SDKHrobot.HRobot.set_digital_output(handle, 51, true);
                        PLC1.Write_DataBit_("M" + (9410 + Memory_PLC.K1000).ToString(), 1);//set
                                                                                           //PLC1.Write_DataBit_("M" + (9080 + Memory_PLC.K1000).ToString(), 1);
                                                                                           //PLC1.Write_DataBit_("M" + (7071 + Memory_PLC.K500).ToString(), 1);//blow jig remove
                                                                                           //PLC1.Write_DataBit_("L" + (94 + Memory_PLC.K100).ToString(), 1);//read data Vision
                                                                                           //PLC1.Write_DataBit_("L" + (91 + Memory_PLC.K100).ToString(), 1);//Flag FPCB Place
                        Thread.Sleep(Global.delay_hut);
                        FuncRobot.Flag_FPCB_After_Pick_Output_RB = true;
                        PLC1.Write_Data_Word_("D" + (9018 + Memory_PLC.K2000).ToString(), 1);
                        SDKHrobot.HRobot.set_override_ratio(handle, Global.SP_auto_RB);
                        SDKHrobot.HRobot.lin_pos(handle, 1, 30, Global.Pick_FPCB_Output_1);
                        FuncRobot.Wait_for_stop_motion_2(handle);
                        PLC1.Write_DataBit_("M" + (9411 + Memory_PLC.K1000).ToString(), 1);//reset
                                                                                           //PLC1.Write_DataBit_("M" + (7067 + Memory_PLC.K500).ToString(), 1);//Vaccum jig remov 
                                                                                           //PLC1.Write_DataBit_("M" + (7037 + Memory_PLC.K500).ToString(), 1);//down jig remove
                        SDKHrobot.HRobot.lin_pos(handle, 1, 30, Global.Ready_Pick_Output_2);
                        SDKHrobot.HRobot.lin_pos(handle, 1, 30, Global.Ready_Pick_Output_1);
                        FuncRobot.Wait_for_stop_motion_2(handle);
                        Process_Auto("Pick FPCB Output 1 End", file_process_auto);
                    }
                    else
                    {
                        Process_Auto("Error position Ready pick output", file_process_auto);
                        //stop_change = 1;
                        //stopOn();
                    }
                }
                else if (PLC1.Read_Data_Word_("D" + (9018 + Memory_PLC.K2000).ToString()) == 1 && stop_change != 1)
                {
                    if ((current_Check_Pick_FPCB_output[0] == Global.Ready_Check_Camtop_1[0] && current_Check_Pick_FPCB_output[1] == Global.Ready_Check_Camtop_1[1] && current_Check_Pick_FPCB_output[2] == Global.Ready_Check_Camtop_1[2] && current_Check_Pick_FPCB_output[5] == Global.Ready_Check_Camtop_1[5]) ||
                        (current_Check_Pick_FPCB_output[0] == Global.Ready_Pick_Output_2[0] && current_Check_Pick_FPCB_output[1] == Global.Ready_Pick_Output_2[1] && current_Check_Pick_FPCB_output[2] == Global.Ready_Pick_Output_2[2] && current_Check_Pick_FPCB_output[5] == Global.Ready_Pick_Output_2[5]))
                    {
                        if ((current_Check_Pick_FPCB_output[0] == Global.Ready_Check_Camtop_1[0] && current_Check_Pick_FPCB_output[1] == Global.Ready_Check_Camtop_1[1] && current_Check_Pick_FPCB_output[2] == Global.Ready_Check_Camtop_1[2] && current_Check_Pick_FPCB_output[5] == Global.Ready_Check_Camtop_1[5]) ||
                        (current_Check_Pick_FPCB_output[0] == Global.Ready_Pick_Output_2[0] && current_Check_Pick_FPCB_output[1] == Global.Ready_Pick_Output_2[1] && current_Check_Pick_FPCB_output[2] == Global.Ready_Pick_Output_2[2] && current_Check_Pick_FPCB_output[5] == Global.Ready_Pick_Output_2[5]))
                        {
                            Global.result_Cam_Bot = new int[40];
                            // FuncRobot.Wait_For_Digital_Input_On(handle, 16);//CMD-RB pick/insert 
                            Global.CMD_Scan_Flag = true;
                            StatusDisplay.Instance.Update_process(txt_process, "Pick Buffer out 2");
                            if (current_Check_Pick_FPCB_output[0] != Global.Ready_Pick_Output_2[0] && current_Check_Pick_FPCB_output[1] != Global.Ready_Pick_Output_2[1] && current_Check_Pick_FPCB_output[2] != Global.Ready_Pick_Output_2[2] && current_Check_Pick_FPCB_output[5] != Global.Ready_Pick_Output_2[5])
                            {
                                SDKHrobot.HRobot.lin_pos(handle, 1, 30, Global.Ready_Pick_Output_1);
                                FuncRobot.Wait_for_stop_motion_2(handle);
                                SDKHrobot.HRobot.lin_pos(handle, 1, 10, Global.Ready_Pick_Output_2);
                                FuncRobot.Wait_for_stop_motion_2(handle);
                            }
                            if (n >= stop_index)
                            {
                                PLC1.Write_DataBit_("L" + (31 + Memory_PLC.K100).ToString(), 1);
                            }
                            FuncRobot.Wait_For_Digital_Input_On(handle, 16);//CMD-RB pick/insert 
                            StatusDisplay.Instance.Update_process(txt_process, "SS Up Cam Top ");
                            FuncRobot.Wait_For_Digital_Input_On(handle, 56);
                            SDKHrobot.HRobot.lin_pos(handle, 1, 10, Global.Pick_FPCB_Output_2);
                            PLC1.Write_DataBit_("M" + (7039 + Memory_PLC.K500).ToString(), 1);// up jig remov
                            FuncRobot.Wait_For_Digital_Input_On(handle, 60); //ss up jig remove
                            double[] Z_Wait_Pick_Output = { Global.Z_Pick_FPCB_Output_2[0], Global.Z_Pick_FPCB_Output_2[1], Global.Z_Pick_FPCB_Output_2[2] + Global.Offset_Z, Global.Z_Pick_FPCB_Output_2[3], Global.Z_Pick_FPCB_Output_2[4], Global.Z_Pick_FPCB_Output_2[5] };
                            SDKHrobot.HRobot.lin_pos(handle, 1, 10, Z_Wait_Pick_Output);
                            FuncRobot.Wait_for_stop_motion_2(handle);
                            SDKHrobot.HRobot.set_override_ratio(handle, Global.SP_Wait_Pick);
                            SDKHrobot.HRobot.lin_pos(handle, 1, 30, Global.Z_Pick_FPCB_Output_2);
                            FuncRobot.Wait_for_stop_motion_2(handle);
                            SDKHrobot.HRobot.set_digital_output(handle, 52, false);
                            SDKHrobot.HRobot.set_digital_output(handle, 51, true);
                            PLC1.Write_DataBit_("M" + (9410 + Memory_PLC.K1000).ToString(), 1);//set
                                                                                               //PLC1.Write_DataBit_("M" + (9080 + Memory_PLC.K1000).ToString(), 1);
                                                                                               //PLC1.Write_DataBit_("M" + (7071 + Memory_PLC.K500).ToString(), 1);//blow jig remove
                                                                                               //PLC1.Write_DataBit_("L" + (94 + Memory_PLC.K100).ToString(), 1);//read data Vision
                                                                                               //PLC1.Write_DataBit_("L" + (91 + Memory_PLC.K100).ToString(), 1);//Flag FPCB Place
                            Thread.Sleep(Global.delay_hut);
                            FuncRobot.Flag_FPCB_After_Pick_Output_RB = true;
                            PLC1.Write_Data_Word_("D" + (9018 + Memory_PLC.K2000).ToString(), 0);
                            SDKHrobot.HRobot.set_override_ratio(handle, Global.SP_auto_RB);
                            SDKHrobot.HRobot.lin_pos(handle, 1, 30, Global.Pick_FPCB_Output_2);
                            FuncRobot.Wait_for_stop_motion_2(handle);
                            StatusDisplay.Instance.Update_process(txt_process, "Wait...");
                            FuncRobot.Wait_For_Digital_Input_On(handle, 16);//CMD-RB pick/insert 
                            PLC1.Write_DataBit_("M" + (9412 + Memory_PLC.K1000).ToString(), 1);//reset2
                                                                                               //PLC1.Write_DataBit_("M" + (7039 + Memory_PLC.K500).ToString(), 0);// up jig remov
                                                                                               //PLC1.Write_DataBit_("M" + (7071 + Memory_PLC.K500).ToString(), 0);//blow jig remove
                                                                                               //PLC1.Write_Data_Word_("D" + (7015 + Memory_PLC.K100).ToString(), 0);
                                                                                               // PLC1.Write_DataBit_("M" + (7037 + Memory_PLC.K500).ToString(), 1);//down jig remove
                            SDKHrobot.HRobot.lin_pos(handle, 1, 30, Global.Ready_Pick_Output_2);
                            SDKHrobot.HRobot.lin_pos(handle, 1, 30, Global.Ready_Pick_Output_1);
                            FuncRobot.Wait_for_stop_motion_2(handle);
                            Process_Auto("Pick FPCB Output 2 End", file_process_auto);
                        }
                        else
                        {
                            Process_Auto("Error position Ready pick output", file_process_auto);
                            stop_change = 1;
                            stopOn();
                        }
                    }

                    else if (current_Check_Pick_FPCB_output[0] == Global.Homee[0] && current_Check_Pick_FPCB_output[1] == Global.Homee[1] &&
                       current_Check_Pick_FPCB_output[2] == Global.Homee[2] && current_Check_Pick_FPCB_output[5] == Global.Homee[5])
                    {
                        Global.result_Cam_Bot = new int[40];
                        Global.CMD_Scan_Flag = true;
                        StatusDisplay.Instance.Update_process(txt_process, "SS Up Cam Top ");
                        FuncRobot.Wait_For_Digital_Input_On(handle, 56);
                        SDKHrobot.HRobot.lin_pos(handle, 1, 20, Global.Ready_Arc_1);
                        SDKHrobot.HRobot.lin_pos(handle, 1, 20, Global.Ready_Arc_2);
                        SDKHrobot.HRobot.lin_pos(handle, 1, 20, Global.Ready_Pick_Output_1);
                        SDKHrobot.HRobot.lin_pos(handle, 1, 20, Global.Ready_Pick_Output_2);
                        FuncRobot.Wait_for_stop_motion_2(handle);
                        if (SDKHrobot.HRobot.get_digital_input(handle, 60) == 0)
                        {
                            PLC1.Write_DataBit_("M" + (7039 + Memory_PLC.K500), 1);
                        }
                        FuncRobot.Wait_For_Digital_Input_On(handle, 60); //ss up jig remove
                        SDKHrobot.HRobot.lin_pos(handle, 1, 10, Global.Pick_FPCB_Output_2);
                        double[] Z_Wait_Pick_Output = { Global.Z_Pick_FPCB_Output_2[0], Global.Z_Pick_FPCB_Output_2[1], Global.Z_Pick_FPCB_Output_2[2] + Global.Offset_Z, Global.Z_Pick_FPCB_Output_2[3], Global.Z_Pick_FPCB_Output_2[4], Global.Z_Pick_FPCB_Output_2[5] };
                        SDKHrobot.HRobot.lin_pos(handle, 1, 10, Z_Wait_Pick_Output);
                        FuncRobot.Wait_for_stop_motion_2(handle);
                        SDKHrobot.HRobot.set_override_ratio(handle, Global.SP_Wait_Pick);
                        SDKHrobot.HRobot.lin_pos(handle, 1, 30, Global.Z_Pick_FPCB_Output_2);
                        FuncRobot.Wait_for_stop_motion_2(handle);
                        SDKHrobot.HRobot.set_digital_output(handle, 52, false);
                        SDKHrobot.HRobot.set_digital_output(handle, 51, true);
                        PLC1.Write_DataBit_("M" + (9410 + Memory_PLC.K1000).ToString(), 1);//set
                                                                                           //PLC1.Write_DataBit_("M" + (9080 + Memory_PLC.K1000).ToString(), 1);
                                                                                           //PLC1.Write_DataBit_("M" + (7071 + Memory_PLC.K500).ToString(), 1);//blow jig remove
                                                                                           //PLC1.Write_DataBit_("L" + (94 + Memory_PLC.K100).ToString(), 1);//read data Vision
                                                                                           //PLC1.Write_DataBit_("L" + (91 + Memory_PLC.K100).ToString(), 1);//Flag FPCB Place
                        Thread.Sleep(Global.delay_hut);
                        FuncRobot.Flag_FPCB_After_Pick_Output_RB = true;
                        PLC1.Write_Data_Word_("D" + (9018 + Memory_PLC.K2000).ToString(), 0);
                        SDKHrobot.HRobot.set_override_ratio(handle, Global.SP_auto_RB);
                        SDKHrobot.HRobot.lin_pos(handle, 1, 30, Global.Pick_FPCB_Output_1);
                        FuncRobot.Wait_for_stop_motion_2(handle);
                        PLC1.Write_DataBit_("M" + (9412 + Memory_PLC.K1000).ToString(), 1);//reset2
                                                                                           //PLC1.Write_DataBit_("M" + (7039 + Memory_PLC.K500).ToString(), 0);// up jig remov
                                                                                           //PLC1.Write_Data_Word_("D" + (7015 + Memory_PLC.K100).ToString(), 0);
                        SDKHrobot.HRobot.lin_pos(handle, 1, 30, Global.Ready_Pick_Output_2);
                        //PLC1.Write_DataBit_("M" + (7071 + Memory_PLC.K500).ToString(), 0);//blow jig remove
                        //PLC1.Write_DataBit_("M" + (7037 + Memory_PLC.K500).ToString(), 1);//down jig remove
                        SDKHrobot.HRobot.lin_pos(handle, 1, 30, Global.Ready_Pick_Output_1);
                        FuncRobot.Wait_for_stop_motion_2(handle);
                        Process_Auto("Pick FPCB Output 1 End", file_process_auto);
                    }
                    //else
                    //{
                    //    Process_Auto("Error position Ready pick output", file_process_auto);
                    //    stop_change = 1;
                    //    stopOn();
                    //}
                }
            }
            else
            {
                FuncRobot.CMD_Pick_Output = false;
            }
        }
        private void Execution_Transfer_Tray()
        {
            Process_Auto(" Execution_Transfer_Tray Start", file_process_auto);
            if (loading_TF_tray == 1 && SDKHrobot.HRobot.get_digital_input(handle, 4) == 0)//di 11 flag TF tray
            {
                StatusDisplay.Instance.Update_process(txt_process, "Chờ insert FPCB");
                int check2 = SDKHrobot.HRobot.get_digital_output(handle, 57); // flag hold transfer tray
                if (check2 == 1)
                {
                    if (SDKHrobot.HRobot.get_digital_input(handle, 4) == 0)// flag tf tray
                    {
                        StatusDisplay.Instance.Update_process(txt_process, "Wait Transfer tray ...");
                        SDKHrobot.HRobot.set_digital_output(handle, 2, true);//RB CALL PLC TRANSFER TRAY -X1031
                        FuncRobot.Wait_For_Digital_Input_On(handle, 4);// flag transfer tray hoàn thành
                        PLC1.Write_DataBit_("M" + (9020 + Memory_PLC.K1000).ToString(), 0);//off rb call pick tray chờ transfer
                        SDKHrobot.HRobot.set_digital_output(handle, 2, false);//off
                        FuncRobot.Wait_For_Digital_Input_On(handle, 16);//free robot
                        n = 1;
                        ind_n1 = 1;
                        ind_n2 = 1;
                        ind_n3 = 1;
                        //ind_n4 = 1;
                        SDKHrobot.HRobot.set_counter(handle, 2, n);
                        SDKHrobot.HRobot.set_counter(handle, 3, ind_n1);
                        SDKHrobot.HRobot.set_counter(handle, 4, ind_n2);
                        SDKHrobot.HRobot.set_counter(handle, 5, ind_n3);
                        // SDKHrobot.HRobot.set_counter(handle, 6, ind_n4);
                        loading_TF_tray = 0;
                        PLC1.Write_DataBit_("L" + (30 + Memory_PLC.K100).ToString(), 0);
                        SDKHrobot.HRobot.set_digital_output(handle, 57, false);//flag đầy tray check cam top NG ko?
                    }
                    else if (SDKHrobot.HRobot.get_digital_input(handle, 4) == 1) //flag transfer tray complete
                    {
                        StatusDisplay.Instance.Update_process(txt_process, "Transfer tray hoàn thành - reset FPCB trên tray");
                        PLC1.Write_DataBit_("M" + (9020 + Memory_PLC.K1000).ToString(), 0);//off rb call pick tray chờ transfer
                        SDKHrobot.HRobot.set_digital_output(handle, 2, false);//off
                        n = 1;
                        ind_n1 = 1;
                        ind_n2 = 1;
                        ind_n3 = 1;
                        //ind_n4 = 1;
                        SDKHrobot.HRobot.set_counter(handle, 2, n);
                        SDKHrobot.HRobot.set_counter(handle, 3, ind_n1);
                        SDKHrobot.HRobot.set_counter(handle, 4, ind_n2);
                        SDKHrobot.HRobot.set_counter(handle, 5, ind_n3);
                        //SDKHrobot.HRobot.set_counter(handle, 6, ind_n4);
                        loading_TF_tray = 0;
                        PLC1.Write_DataBit_("L" + (30 + Memory_PLC.K100).ToString(), 0);
                        SDKHrobot.HRobot.set_digital_output(handle, 57, false);//flag đầy tray check cam top NG ko?
                    }
                }
            }
            else if (loading_TF_tray == 1 & SDKHrobot.HRobot.get_digital_input(handle, 4) == 1)
            {
                n = 1;
                ind_n1 = 1;
                ind_n2 = 1;
                ind_n3 = 1;
                //ind_n4 = 1;
                SDKHrobot.HRobot.set_counter(handle, 2, n);
                SDKHrobot.HRobot.set_counter(handle, 3, ind_n1);
                SDKHrobot.HRobot.set_counter(handle, 4, ind_n2);
                SDKHrobot.HRobot.set_counter(handle, 5, ind_n3);
                //SDKHrobot.HRobot.set_counter(handle, 6, ind_n4);
                PLC1.Write_DataBit_("L" + (20 + Memory_PLC.K100).ToString(), 1);//off              
                loading_TF_tray = 0;
                SDKHrobot.HRobot.set_digital_output(handle, 57, false);//flag đầy tray check cam top NG ko?                            
                StatusDisplay.Instance.Update_process(txt_process, "Transfer tray hoàn thành - reset FPCB trên tray");
                FuncRobot.Wait_For_Digital_Input_On(handle, 16);//free robot
            }

            Process_Auto(" Execution_Transfer_Tray End", file_process_auto);
        }
        private void Check_FPCB_Place()
        {
            Process_Auto("Check_FPCB_Place Start", file_process_auto);
            if (n >= stop_index - 10)
            {
                bool ss_vaccum_pick_tray1 = PLC1.Read_Data_Bit_("M" + (8526 + Memory_PLC.K300).ToString());
                if (ss_vaccum_pick_tray1 == false)
                {
                    StatusDisplay.Instance.Update_process(txt_process, "Yêu cầu Hút tray");
                    PLC1.Write_DataBit_("M" + (9020 + Memory_PLC.K1000).ToString(), 1);// rb call hút tray chờ transfer
                }

            }

            if (n >= stop_index)
            {
                StatusDisplay.Instance.Update_process(txt_process, "Tray đã đủ số lường FPCB");
                loading_TF_tray = 1;
                SDKHrobot.HRobot.set_digital_output(handle, 57, true);//flag đầy tray?                                                                                                                          //{
                Mov_Pick_bool = false;
                Execution_Transfer_Tray2();
                FuncRobot.Wait_For_Digital_Input_On(handle, 16);
                FuncRobot.Wait_For_Digital_Input_On(handle, 3);
            }
            Process_Auto("Check_FPCB_Place End", file_process_auto);
        }
        private void Mov_Place_Tray()
        {
            int so_lan_tha_hang = 0;
            StatusDisplay.Instance.Update_process(txt_process, "Wait PLC(Check Tray Output)");
            PLC1.Write_DataBit_("L" + (31 + Memory_PLC.K100).ToString(), 0);// nguy hiem tray                                         
            FuncRobot.Wait_For_Digital_Input_On(handle, 16); //smema plc call insert tray
            FuncRobot.Wait_for_stop_motion_2(handle);
            FuncRobot.Wait_For_Digital_Input_On(handle, 61); //ss down jig remove
            bool check_data_vision_ = Global.result_Cam_Bot.Contains(0);
            if (check_data_vision_ == true)
            {
                Global.CMD_Scan_Flag = true;
            }
            //SDKHrobot.HRobot.lin_pos(handle, 1, 30, Ready_Place_Tray_1);
            //SDKHrobot.HRobot.lin_pos(handle, 1, 30, Ready_Place_Tray_2);
            FuncRobot.Wait_for_stop_motion_2(handle);
            double[] current_check = new double[6];
            int check_curr = SDKHrobot.HRobot.get_current_position(handle, current_check);
            if (check_curr != 0)
            {
                SDKHrobot.HRobot.motion_abort(handle);
                stop_change = 1;
                stopOn();
            }

            if (current_check[0] == Global.Ready_Place_Tray_2[0] && current_check[1] == Global.Ready_Place_Tray_2[1] && current_check[2] == Global.Ready_Place_Tray_2[2] && current_check[5] == Global.Ready_Place_Tray_2[5] && stop_change != 1)
            {
                Process_Auto("Place Tray", file_process_auto);
                //Vaccum_Tool3 = { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 };
                //Vaccum_Tool4 = { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 };
                //Vaccum_Tool = PLC1.Read_Word_Arr("D" + (7031 + Memory_PLC.K100).ToString(), Number_Tool);
                int vaccum = SDKHrobot.HRobot.get_digital_output(handle, 51);
                int check1 = SDKHrobot.HRobot.get_digital_output(handle, 57);//flag đầy tray
                if (check1 == 0 && n < stop_index && vaccum == 1)
                {
                    FuncRobot.Wait_For_Digital_Input_On(handle, 16); //smema plc call insert tray
                    SDKHrobot.HRobot.lin_pos(handle, 1, 30, Global.Ready_Place_Tray_3);
                    PLC1.Write_DataBit_("M" + (7041 + Memory_PLC.K500).ToString(), 1);
                    StatusDisplay.Instance.Update_process(txt_process, "SS OutSide Lamp CamBot ");
                    FuncRobot.Wait_For_Digital_Input_On(handle, 62); //ss outside lamp cambottom                       
                    FuncRobot.Wait_for_stop_motion_2(handle);
                    check_data_vision_ = Global.result_Cam_Bot.Contains(1);
                    if (check_data_vision_ == true)
                    {
                        Place_NG_2();
                    }
                    Global.Security_Place = true;
                }
                Vaccum_Tool1 = PLC1.Read_Word_Arr("D" + (7031 + Memory_PLC.K100).ToString(), 10);
                Vaccum_Tool2 = PLC1.Read_Word_Arr("D" + (7041 + Memory_PLC.K100).ToString(), 10);
                int select_tool_flag = 0;
                int[] bool_flag_scan = new int[10];
                //
                int select_tool_flag2 = 0;
                int[] bool_flag_scan2 = new int[10];
                //
                int select_tool_flag3 = 0;
                int[] bool_flag_scan3 = new int[10];
                //
                int select_tool_flag4 = 0;
                int[] bool_flag_scan4 = new int[10];
                Global.CMD_Scan_Flag = true;
                if (n < stop_index && stop_change != 1)
                {
                    #region MAXTRIX 1
                    while (select_tool_flag < 10 && stop_change != 1)
                    {
                        for (int i = 0; i < 10; i++)
                        {
                            if (n < stop_index && stop_change != 1)
                            {
                                if (ind_n1 < 11)
                                {
                                    if (Global.result_Cam_Bot[i] == 2 && Vaccum_Tool1[i] == 1)
                                    {
                                        bool_flag_scan[i] = 1;
                                        double[] pos_tool1 = new double[6];
                                        pos_tool1 = matrix.PAL_P_RB_1_1(1 + i, ind_n1, Global.Row_tray_matrix1, Global.Column_tray_matrix1);
                                        StatusDisplay.Instance.Update_process(txt_process, "SS Up Cam Top ");
                                        FuncRobot.Wait_For_Digital_Input_On(handle, 56);
                                        if (pos_tool1[2] == Global.Place_Tray_1[2])
                                        {
                                            if (matrix.Flag_Read_Data_Maxtrix_Tool_RB == true && pos_tool1[0] > Global.X_Place_Satefy_L && pos_tool1[0] < Global.X_Place_Satefy_U && pos_tool1[1] < Global.Y_Place_Satefy_U && pos_tool1[1] > Global.Y_Place_Satefy_L && pos_tool1[2] > Global.Z_Place_Satefy)
                                            {
                                                SDKHrobot.HRobot.lin_pos(handle, 1, 30, pos_tool1);
                                                FuncRobot.Wait_for_stop_motion_2(handle);
                                                PLC1.Write_DataBit_("M" + (9066 + Memory_PLC.K1000).ToString(), 1);
                                                Thread.Sleep(100);
                                                SDKHrobot.HRobot.set_digital_output(handle, 52, true);//power blow                                              
                                                PLC1.Write_Data_Word_("D" + (7031 + i + Memory_PLC.K100).ToString(), 0);//blow index tool
                                                Thread.Sleep(Global.delay_tha);
                                                SDKHrobot.HRobot.set_digital_output(handle, 52, false);//power blow 
                                                Vaccum_Tool1[i] = 0;
                                                Global.result_Cam_Bot[i] = 0;
                                                PLC1.Write_Data_Word_("D" + (9200 + i + Memory_PLC.K2000).ToString(), 0);
                                                PLC1.Write_DataBit_("M" + (9067 + Memory_PLC.K1000).ToString(), 1);
                                                ind_n1 = ind_n1 + 1;
                                                n = n + 1;
                                                SDKHrobot.HRobot.set_counter(handle, 2, n);
                                                SDKHrobot.HRobot.set_counter(handle, 3, ind_n1);
                                                select_tool_flag = select_tool_flag + 1;
                                                so_lan_tha_hang++;
                                            }
                                            else
                                            {
                                                Process_Auto("Error position Place tray", file_process_auto);
                                                SDKHrobot.HRobot.motion_abort(handle);
                                            }
                                        }
                                        else
                                        {
                                            StatusDisplay.Instance.Update_process(txt_process, "Read data position SQL Matrix error");
                                            SDKHrobot.HRobot.motion_abort(handle);
                                            stop_change = 1;
                                            stopOn();
                                        }
                                    }
                                    else if (Global.result_Cam_Bot[i] == 1 && Vaccum_Tool1[i] == 1 && bool_flag_scan[i] == 0)
                                    {
                                        bool_flag_scan[i] = 1;
                                        select_tool_flag = select_tool_flag + 1;
                                    }
                                    else if (Global.result_Cam_Bot[i] == 3 && Vaccum_Tool1[i] == 1 && bool_flag_scan[i] == 0)
                                    {
                                        bool_flag_scan[i] = 1;
                                        select_tool_flag = select_tool_flag + 1;
                                    }
                                    else if (Global.result_Cam_Bot[i] == 0 && Vaccum_Tool1[i] == 0 && bool_flag_scan[i] == 0)
                                    {
                                        bool_flag_scan[i] = 1;
                                        select_tool_flag = select_tool_flag + 1;
                                    }
                                    else if (Global.result_Cam_Bot[i] != 0 && Vaccum_Tool1[i] == 0 && bool_flag_scan[i] == 0)
                                    {
                                        bool_flag_scan[i] = 1;
                                        select_tool_flag = select_tool_flag + 1;
                                    }
                                    else if (Global.result_Cam_Bot[i] == 0 && Vaccum_Tool1[i] == 1)
                                    {
                                        bool_flag_scan[i] = 1;
                                        select_tool_flag = select_tool_flag + 1;
                                        //StatusDisplay.Instance.Update_process(txt_process, "Không có data tool " + (i + 1).ToString());
                                    }
                                }
                                else
                                {
                                    i = i + 10;
                                    select_tool_flag = select_tool_flag + Global.Number_Tool;
                                }
                                StatusDisplay.Instance.Update_process(txt_process, select_tool_flag.ToString());
                                Machine_Pause_(stop_change);
                            }
                            else
                            {
                                select_tool_flag = select_tool_flag + 10;
                                i = i + 10;
                            }
                        }
                    } // MATRIX 1
                    #endregion
                    #region MAXTRIX 2
                    while (select_tool_flag2 < 10 && stop_change != 1)
                    {
                        for (int i = 0; i < 10; i++)
                        {
                            if (n < stop_index && stop_change != 1)
                            {
                                if (ind_n2 < 19)
                                {
                                    if (Global.result_Cam_Bot[i + 10] == 2 && Vaccum_Tool2[i] == 1)
                                    {
                                        bool_flag_scan2[i] = 1;
                                        double[] pos_tool1 = new double[6];
                                        pos_tool1 = matrix.PAL_P_RB_1_2(1 + i, ind_n2, Global.Row_tray_matrix2, Global.Column_tray_matrix2);
                                        StatusDisplay.Instance.Update_process(txt_process, "SS Up Cam Top ");
                                        FuncRobot.Wait_For_Digital_Input_On(handle, 56);
                                        if (pos_tool1[2] == Global.Place_Tray_2[2])
                                        {
                                            if (matrix.Flag_Read_Data_Maxtrix_Tool_RB == true && pos_tool1[0] > Global.X_Place_Satefy_L && pos_tool1[0] < Global.X_Place_Satefy_U && pos_tool1[1] < Global.Y_Place_Satefy_U && pos_tool1[1] > Global.Y_Place_Satefy_L && pos_tool1[2] > Global.Z_Place_Satefy)
                                            {
                                                SDKHrobot.HRobot.lin_pos(handle, 1, 30, pos_tool1);
                                                FuncRobot.Wait_for_stop_motion_2(handle);
                                                PLC1.Write_DataBit_("M" + (9066 + Memory_PLC.K1000).ToString(), 1);//on vaccum
                                                Thread.Sleep(100);
                                                SDKHrobot.HRobot.set_digital_output(handle, 52, true);//power blow 
                                                PLC1.Write_Data_Word_("D" + (7031 + i + 10 + Memory_PLC.K100).ToString(), 0);//blow index tool
                                                Thread.Sleep(Global.delay_tha);
                                                SDKHrobot.HRobot.set_digital_output(handle, 52, false);//power blow 
                                                Vaccum_Tool2[i] = 0;
                                                Global.result_Cam_Bot[i + 10] = 0;
                                                PLC1.Write_Data_Word_("D" + (9200 + i + 10 + Memory_PLC.K2000).ToString(), 0);
                                                PLC1.Write_DataBit_("M" + (9067 + Memory_PLC.K1000).ToString(), 1);//off vaccum
                                                ind_n2 = ind_n2 + 1;
                                                n = n + 1;
                                                SDKHrobot.HRobot.set_counter(handle, 2, n);
                                                SDKHrobot.HRobot.set_counter(handle, 4, ind_n2);
                                                select_tool_flag2 = select_tool_flag2 + 1;
                                                so_lan_tha_hang++;
                                            }
                                            else
                                            {
                                                Process_Auto("Error position Place tray", file_process_auto);
                                                SDKHrobot.HRobot.motion_abort(handle);
                                            }
                                        }
                                        else
                                        {
                                            StatusDisplay.Instance.Update_process(txt_process, "Read data position SQL Matrix error");
                                            SDKHrobot.HRobot.motion_abort(handle);
                                            stop_change = 1;
                                            stopOn();
                                        }
                                    }
                                    else if (Global.result_Cam_Bot[i + 10] == 1 && Vaccum_Tool2[i] == 1 && bool_flag_scan2[i] == 0)
                                    {
                                        bool_flag_scan2[i] = 1;
                                        select_tool_flag2 = select_tool_flag2 + 1;
                                    }
                                    else if (Global.result_Cam_Bot[i + 10] == 3 && Vaccum_Tool2[i] == 1 && bool_flag_scan2[i] == 0)
                                    {
                                        bool_flag_scan2[i] = 1;
                                        select_tool_flag2 = select_tool_flag2 + 1;
                                    }
                                    else if (Global.result_Cam_Bot[i + 10] == 0 && Vaccum_Tool2[i] == 0 && bool_flag_scan2[i] == 0)
                                    {
                                        bool_flag_scan2[i] = 1;
                                        select_tool_flag2 = select_tool_flag2 + 1;
                                    }
                                    else if (Global.result_Cam_Bot[i + 10] != 0 && Vaccum_Tool2[i] == 0 && bool_flag_scan2[i] == 0)
                                    {
                                        bool_flag_scan2[i] = 1;
                                        select_tool_flag2 = select_tool_flag2 + 1;
                                    }
                                    else if (Global.result_Cam_Bot[i + 10] == 0 && Vaccum_Tool2[i] == 1)
                                    {
                                        bool_flag_scan2[i] = 1;
                                        select_tool_flag2 = select_tool_flag2 + 1;
                                    }
                                }
                                else
                                {
                                    select_tool_flag2 = select_tool_flag2 + 10;
                                    i = i + 10;
                                }
                                StatusDisplay.Instance.Update_process(txt_process, select_tool_flag2.ToString());
                                Machine_Pause_(stop_change);
                            }
                            else
                            {
                                select_tool_flag2 = select_tool_flag2 + 10;
                                i = i + 10;
                            }
                        }
                    } // MATRIX 2
                    #endregion
                    #region MAXTRIX 3
                    while (select_tool_flag3 < 10 && stop_change != 1)
                    {
                        for (int i = 0; i < 10; i++)
                        {
                            if (n < stop_index && stop_change != 1)
                            {
                                if (ind_n2 < 19)
                                {
                                    if (Global.result_Cam_Bot[i] == 2 && Vaccum_Tool1[i] == 1)
                                    {
                                        bool_flag_scan3[i] = 1;
                                        double[] pos_tool1 = new double[6];
                                        ind_n3 = Case_input_tray_matrix3_2(ind_n2);
                                        pos_tool1 = matrix.PAL_P_RB_1_3(1 + i, ind_n3, Global.Row_tray_matrix3, Global.Column_tray_matrix3);
                                        StatusDisplay.Instance.Update_process(txt_process, "SS Up Cam Top ");
                                        FuncRobot.Wait_For_Digital_Input_On(handle, 56);
                                        if (pos_tool1[2] == Global.Place_Tray_3[2] && ind_n3 < stop_index)
                                        {
                                            if (matrix.Flag_Read_Data_Maxtrix_Tool_RB == true && pos_tool1[0] > Global.X_Place_Satefy_L && pos_tool1[0] < Global.X_Place_Satefy_U && pos_tool1[1] < Global.Y_Place_Satefy_U && pos_tool1[1] > Global.Y_Place_Satefy_L && pos_tool1[2] > Global.Z_Place_Satefy)
                                            {
                                                SDKHrobot.HRobot.lin_pos(handle, 1, 30, pos_tool1);
                                                FuncRobot.Wait_for_stop_motion_2(handle);
                                                PLC1.Write_DataBit_("M" + (9066 + Memory_PLC.K1000).ToString(), 1);//on vaccum
                                                Thread.Sleep(100);
                                                SDKHrobot.HRobot.set_digital_output(handle, 52, true);//power blow 
                                                PLC1.Write_Data_Word_("D" + (7031 + i + Memory_PLC.K100).ToString(), 0);//blow index tool
                                                Thread.Sleep(Global.delay_tha);
                                                SDKHrobot.HRobot.set_digital_output(handle, 52, false);//power blow 
                                                                                                       //PLC1.Write_Data_Word_("D" + (7031 + i + Memory_PLC.K100).ToString(), 1);//blow index tool
                                                Vaccum_Tool1[i] = 0;
                                                Global.result_Cam_Bot[i] = 0;
                                                PLC1.Write_Data_Word_("D" + (9200 + i + Memory_PLC.K2000).ToString(), 0);
                                                PLC1.Write_DataBit_("M" + (9067 + Memory_PLC.K1000).ToString(), 1);//off vaccum
                                                ind_n3 = ind_n3 + 1;
                                                ind_n2 = ind_n2 + 1;
                                                n = n + 1;
                                                SDKHrobot.HRobot.set_counter(handle, 2, n);
                                                SDKHrobot.HRobot.set_counter(handle, 4, ind_n2);
                                                SDKHrobot.HRobot.set_counter(handle, 5, ind_n3);
                                                select_tool_flag3 = select_tool_flag3 + 1;
                                                so_lan_tha_hang++;
                                            }
                                            else
                                            {
                                                Process_Auto("Error position Place tray", file_process_auto);
                                                SDKHrobot.HRobot.motion_abort(handle);
                                            }
                                        }
                                        else
                                        {
                                            StatusDisplay.Instance.Update_process(txt_process, "Read data position SQL Matrix3 error");
                                            SDKHrobot.HRobot.motion_abort(handle);
                                            stop_change = 1;
                                            stopOn();
                                        }
                                    }
                                    else if (Global.result_Cam_Bot[i] == 1 && Vaccum_Tool1[i] == 1 && bool_flag_scan3[i] == 0)
                                    {
                                        bool_flag_scan3[i] = 1;
                                        select_tool_flag3 = select_tool_flag3 + 1;
                                    }
                                    else if (Global.result_Cam_Bot[i] == 3 && Vaccum_Tool1[i] == 1 && bool_flag_scan3[i] == 0)
                                    {
                                        bool_flag_scan3[i] = 1;
                                        select_tool_flag3 = select_tool_flag3 + 1;
                                    }
                                    else if (Global.result_Cam_Bot[i] == 0 && Vaccum_Tool1[i] == 0 && bool_flag_scan3[i] == 0)
                                    {
                                        bool_flag_scan3[i] = 1;
                                        select_tool_flag3 = select_tool_flag3 + 1;
                                    }
                                    else if (Global.result_Cam_Bot[i] != 0 && Vaccum_Tool1[i] == 0 && bool_flag_scan3[i] == 0)
                                    {
                                        bool_flag_scan3[i] = 1;
                                        select_tool_flag3 = select_tool_flag3 + 1;
                                    }
                                    else if (Global.result_Cam_Bot[i] == 0 && Vaccum_Tool1[i] == 1)
                                    {
                                        bool_flag_scan3[i] = 1;
                                        select_tool_flag3 = select_tool_flag3 + 1;
                                        //StatusDisplay.Instance.Update_process(txt_process, "Không có data tool " + (i + 1).ToString());
                                    }
                                }
                                else
                                {
                                    i = i + 10;
                                    select_tool_flag3 = select_tool_flag3 + 10;
                                }
                                StatusDisplay.Instance.Update_process(txt_process, select_tool_flag3.ToString());
                                Machine_Pause_(stop_change);
                            }
                            else
                            {
                                select_tool_flag3 = select_tool_flag3 + 10;
                                i = i + 10;
                            }
                        }
                    } // MATRIX 3
                    #endregion
                    #region MAXTRIX 4
                    //while (select_tool_flag4 < 10 && stop_change != 1)
                    //{
                    //    for (int i = 0; i < 10; i++)
                    //    {
                    //        if (n < stop_index && stop_change != 1)
                    //        {
                    //            if (ind_n3 < 9)
                    //            {
                    //                if (result_Cam_Bot[i + 10] == 2 && Vaccum_Tool4[i] == 1)
                    //                {
                    //                    bool_flag_scan4[i] = 1;
                    //                    double[] pos_tool1 = new double[6];
                    //                    pos_tool1 = matrix.PAL_P_RB_1_4(1 + i, ind_n4);
                    //                    if ((pos_tool1[0] == Place_Tray_4[0]) && pos_tool1[2] == Place_Tray_4[2])
                    //                    {
                    //                        if (matrix.Flag_Read_Data_Maxtrix_Tool_RB == true && pos_tool1[0] > X_Place_Satefy_L && pos_tool1[0] < X_Place_Satefy_U && pos_tool1[1] < Offset_Y[1] && pos_tool1[1] > Y_Place_Satefy_L && pos_tool1[2] > Z_Place_Satefy)
                    //                        {
                    //                            SDKHrobot.HRobot.lin_pos(handle, 1, 30, pos_tool1);
                    //                            FuncRobot.Wait_for_stop_motion_2(handle);
                    //                            SDKHrobot.HRobot.set_digital_output(handle, 52, true);//power blow 
                    //                            PLC1.Write_Data_Word_("D" + (7031 + i + 10 + Memory_PLC.K100).ToString(), 0);//blow index tool
                    //                            Thread.Sleep(delay_tha);
                    //                            SDKHrobot.HRobot.set_digital_output(handle, 52, false);//power blow 
                    //                            Vaccum_Tool4[i] = 0;
                    //                            result_Cam_Bot[i + 10] = 0;
                    //                            PLC1.Write_Data_Word_("D" + (9200 + i + 10 + Memory_PLC.K2000).ToString(), 0);
                    //                            ind_n3 = ind_n3 + 1;
                    //                            n = n + 1;
                    //                            SDKHrobot.HRobot.set_counter(handle, 2, n);
                    //                            SDKHrobot.HRobot.set_counter(handle, 5, ind_n3);
                    //                            select_tool_flag4 = select_tool_flag4 + 1;
                    //                            so_lan_tha_hang++;
                    //                        }
                    //                        else
                    //                        {
                    //                            Process_Auto("Error position Place tray4", file_process_auto);
                    //                            SDKHrobot.HRobot.motion_abort(handle);
                    //                        }
                    //                    }
                    //                    else
                    //                    {
                    //                        StatusDisplay.Instance.Update_process(txt_process, "Read data position SQL Matrix error4");
                    //                        SDKHrobot.HRobot.motion_abort(handle);
                    //                        stop_change = 1;
                    //                        stopOn();
                    //                    }
                    //                }
                    //                else if (result_Cam_Bot[i + 10] == 1 && Vaccum_Tool4[i] == 1 && bool_flag_scan4[i] == 0)
                    //                {
                    //                    bool_flag_scan4[i] = 1;
                    //                    select_tool_flag4 = select_tool_flag4 + 1;
                    //                }
                    //                else if (result_Cam_Bot[i + 10] == 3 && Vaccum_Tool4[i] == 1 && bool_flag_scan4[i] == 0)
                    //                {
                    //                    bool_flag_scan4[i + 10] = 1;
                    //                    select_tool_flag4 = select_tool_flag4 + 1;
                    //                }
                    //                else if (result_Cam_Bot[i + 10] == 0 && Vaccum_Tool4[i] == 0 && bool_flag_scan4[i] == 0)
                    //                {
                    //                    bool_flag_scan4[i] = 1;
                    //                    select_tool_flag4 = select_tool_flag4 + 1;
                    //                }
                    //                else if (result_Cam_Bot[i + 10] != 0 && Vaccum_Tool4[i] == 0 && bool_flag_scan4[i] == 0)
                    //                {
                    //                    bool_flag_scan4[i] = 1;
                    //                    select_tool_flag4 = select_tool_flag4 + 1;
                    //                }
                    //                else if (result_Cam_Bot[i + 10] == 0 && Vaccum_Tool4[i] == 1)
                    //                {
                    //                    bool_flag_scan4[i] = 1;
                    //                    select_tool_flag4 = select_tool_flag4 + 1;
                    //                    //StatusDisplay.Instance.Update_process(txt_process, "Không có data tool " + (i + 1).ToString());
                    //                }
                    //            }
                    //            else
                    //            {
                    //                i = i + 10;
                    //                select_tool_flag4 = select_tool_flag4 + 10;
                    //            }
                    //            StatusDisplay.Instance.Update_process(txt_process, select_tool_flag.ToString());
                    //            Machine_Pause_(stop_change);
                    //        }
                    //        else
                    //        {
                    //            select_tool_flag4 = select_tool_flag4 + 10;
                    //            i = i + 10;
                    //        }
                    //    }
                    //} // MATRIX 4
                    #endregion
                }
                if (n > 1 && SDKHrobot.HRobot.get_digital_input(handle, 4) == 1)//flag transfer tray
                {
                    PLC1.Write_DataBit_("L" + (30 + Memory_PLC.K100).ToString(), 0);// tín hiệu transfer tray
                }
                Global.Security_Place = false;
                Machine_Pause_(stop_change);
                Process_Auto("Ready_Place_3 Start", file_process_auto);
                int ready_place_3 = -1;
                ready_place_3 = SDKHrobot.HRobot.lin_pos(handle, 0, 0, Global.Ready_Place_Tray_3);
                if (ready_place_3 != 0)
                {
                    SDKHrobot.HRobot.motion_abort(handle);
                    Process_Auto("Ready_Place_3 Error", file_process_auto);
                    stop_change = 1;
                    stopOn();
                }
                if (ready_place_3 == 0)
                {
                    int[] Check_Vaccum_After_Place = new int[2];
                    for (int i = 0; i < 20; i++)
                    {
                        if (SDKHrobot.HRobot.get_digital_output(handle, 51) == 1 && Global.result_Cam_Bot[i] == 1)// hang NG
                        {
                            Check_Vaccum_After_Place[0]++;
                        }
                        else if (SDKHrobot.HRobot.get_digital_output(handle, 51) == 1 && Global.result_Cam_Bot[i] == 2)//hang ok
                        {
                            Check_Vaccum_After_Place[1]++;
                        }
                    }
                    Machine_Pause_(stop_change);
                    if (Check_Vaccum_After_Place[1] > 0 && Check_Vaccum_After_Place[0] > 0 && n >= stop_index)
                    {
                        //đợi thả hết hàng ok vào tray
                        bool_Check_tool_after_place = true;
                        SDKHrobot.HRobot.lin_pos(handle, 1, 30, Global.Ready_Place_Tray_2);
                        SDKHrobot.HRobot.lin_pos(handle, 1, 30, Global.Ready_Pick_Output_1);
                        FuncRobot.Wait_for_stop_motion_2(handle);
                        double[] check_pos = new double[6];
                        SDKHrobot.HRobot.get_current_position(handle, check_pos);
                        if (n >= stop_index && check_pos[0] == Global.Ready_Pick_Output_1[0] && check_pos[1] == Global.Ready_Pick_Output_1[1] && check_pos[2] == Global.Ready_Pick_Output_1[2] && check_pos[5] == Global.Ready_Pick_Output_1[5])
                        {
                            PLC1.Write_DataBit_("L" + (31 + Memory_PLC.K100).ToString(), 1);
                        }
                        Check_FPCB_Place();
                        //Execution_Transfer_Tray();
                        Motion_Place_Tray();
                    }
                    else if (Check_Vaccum_After_Place[0] > 0 && Check_Vaccum_After_Place[1] == 0)
                    {
                        // thả hàng NG
                        StatusDisplay.Instance.Update_process(txt_process, "Thả hàng NG");
                        bool_Check_tool_after_place = false;
                        FuncRobot.Flag_FPCB_After_Pick_Output_RB = false;
                        FuncRobot.Flag_FPCB_All_NG = true;
                        SDKHrobot.HRobot.lin_pos(handle, 1, 20, Global.Ready_Place_Tray_3);
                        FuncRobot.Wait_for_stop_motion(handle);
                        Place_NG();
                    }
                    else if (Check_Vaccum_After_Place[0] == 0 && Check_Vaccum_After_Place[1] == 0 && PLC1.Read_Data_Word_("D" + (9018 + Memory_PLC.K2000).ToString()) == 0)
                    {
                        //thả hết hàng trên tool
                        StatusDisplay.Instance.Update_process(txt_process, "Hết FPCB");
                        bool_Check_tool_after_place = false;
                        FuncRobot.Flag_FPCB_After_Pick_Output_RB = false;
                        SDKHrobot.HRobot.set_digital_output(handle, 51, false);
                        SDKHrobot.HRobot.set_digital_output(handle, 52, false);
                        PLC1.Write_DataBit_("M" + (9081 + Memory_PLC.K1000).ToString(), 1);
                        SDKHrobot.HRobot.lin_pos(handle, 1, 20, Global.Ready_Place_Tray_3);
                        SDKHrobot.HRobot.lin_pos(handle, 1, 20, Global.Ready_Place_Tray_2);
                        FuncRobot.Wait_for_stop_motion_2(handle);
                        //SDKHrobot.HRobot.lin_pos(handle, 1, 20, Ready_Place_Tray_1);
                        SDKHrobot.HRobot.lin_pos(handle, 1, 20, Global.Ready_Arc_2);
                        FuncRobot.Wait_for_stop_motion_2(handle);
                        if (n > stop_index - 4)
                        {
                            PLC1.Write_DataBit_("M" + (9020 + Memory_PLC.K1000).ToString(), 1);
                        }
                        if (n > stop_index || n == stop_index)
                        {
                            SDKHrobot.HRobot.set_digital_output(handle, 2, true);
                            Thread.Sleep(150);
                            SDKHrobot.HRobot.set_digital_output(handle, 2, false);
                        }
                        SDKHrobot.HRobot.lin_pos(handle, 1, 20, Global.Ready_Arc_1);
                        SDKHrobot.HRobot.lin_pos(handle, 1, 20, Global.Homee);
                        FuncRobot.Wait_for_stop_motion(handle);
                        double[] check_pos = new double[6];
                        SDKHrobot.HRobot.get_current_position(handle, check_pos);
                        if (check_pos[0] == Global.Homee[0] && check_pos[1] == Global.Homee[1] && check_pos[2] == Global.Homee[2] && check_pos[5] == Global.Homee[5])
                        {
                            PLC1.Write_DataBit_("L" + (40 + Memory_PLC.K100).ToString(), 1);
                            PLC1.Write_DataBit_("L" + (39 + Memory_PLC.K100).ToString(), 1);
                            PLC1.Write_DataBit_("L" + (31 + Memory_PLC.K100).ToString(), 1);// nguy hiem
                            PLC1.Write_DataBit_("L" + (91 + Memory_PLC.K100).ToString(), 0);//flag fpcb place
                        }
                    }
                    else if (Check_Vaccum_After_Place[0] == 0 && Check_Vaccum_After_Place[1] == 0 && PLC1.Read_Data_Word_("D" + (9018 + Memory_PLC.K2000).ToString()) == 1)
                    {
                        FuncRobot.Flag_FPCB_After_Pick_Output_RB = false;
                        SDKHrobot.HRobot.set_digital_output(handle, 51, false);
                        SDKHrobot.HRobot.set_digital_output(handle, 52, false);
                        PLC1.Write_DataBit_("M" + (9081 + Memory_PLC.K1000).ToString(), 1);
                        SDKHrobot.HRobot.lin_pos(handle, 1, 20, Global.Ready_Place_Tray_3);
                        FuncRobot.Wait_for_stop_motion_2(handle);
                        SDKHrobot.HRobot.lin_pos(handle, 1, 30, Global.Ready_Place_Tray_2);
                        SDKHrobot.HRobot.lin_pos(handle, 1, 30, Global.Ready_Pick_Output_2);
                        FuncRobot.Wait_for_stop_motion_2(handle);
                        if (n > stop_index - 4)
                        {
                            PLC1.Write_DataBit_("M" + (9020 + Memory_PLC.K1000).ToString(), 1);
                        }
                        if (n >= stop_index)
                        {
                            SDKHrobot.HRobot.set_digital_output(handle, 2, true);
                            Thread.Sleep(150);
                            //DKHrobot.HRobot.set_digital_output(handle, 2, false);
                            //PLC1.Write_DataBit_("L" + (31 + Memory_PLC.K100).ToString(), 1);// nguy hiem
                        }
                        Mov_Pick_FPCB_Output();
                        double[] check_pos = new double[6];
                        SDKHrobot.HRobot.get_current_position(handle, check_pos);
                        if ((n == stop_index || n > stop_index) && check_pos[0] == Global.Ready_Pick_Output_1[0] && check_pos[1] == Global.Ready_Pick_Output_1[1] && check_pos[2] == Global.Ready_Pick_Output_1[2] && check_pos[5] == Global.Ready_Pick_Output_1[5])
                        {
                            PLC1.Write_DataBit_("L" + (31 + Memory_PLC.K100).ToString(), 1);
                        }
                        Check_FPCB_Place();
                        FuncRobot.CMD_Pick_Output = false;
                        //Execution_Transfer_Tray();
                        Motion_Place_Tray();
                    }
                    else if (Check_Vaccum_After_Place[0] == 0 && Check_Vaccum_After_Place[1] > 0 && n >= stop_index)
                    {
                        SDKHrobot.HRobot.lin_pos(handle, 1, 30, Global.Ready_Place_Tray_2);
                        SDKHrobot.HRobot.lin_pos(handle, 1, 30, Global.Ready_Pick_Output_1);
                        FuncRobot.Wait_for_stop_motion_2(handle);
                        double[] check_pos = new double[6];
                        SDKHrobot.HRobot.get_current_position(handle, check_pos);
                        if ((n == stop_index || n > stop_index) && check_pos[0] == Global.Ready_Pick_Output_1[0] && check_pos[1] == Global.Ready_Pick_Output_1[1] && check_pos[2] == Global.Ready_Pick_Output_1[2] && check_pos[5] == Global.Ready_Pick_Output_1[5])
                        {
                            PLC1.Write_DataBit_("L" + (31 + Memory_PLC.K100).ToString(), 1);
                        }
                        Check_FPCB_Place();
                        //Execution_Transfer_Tray();
                        Motion_Place_Tray();
                    }
                    else if (Check_Vaccum_After_Place[0] == 0 && Check_Vaccum_After_Place[1] > 0 && n < stop_index)
                    {
                        Process_Auto("Index Special", file_process_auto);
                        Place_NG();
                        //Process_Auto("Index number tray current error", file_process_auto);
                        //StatusDisplay.Instance.Update_process(txt_process, "Index number tray current error");
                        //stop_change = 1;
                        //stopOn();
                    }
                }
                else
                {
                    SDKHrobot.HRobot.motion_abort(handle);
                    stop_change = 1;
                    stopOn();
                }
                Machine_Pause_(stop_change);
                FuncRobot.Wait_for_stop_motion_2(handle);
                //Process_Auto("Place tray 2 End", file_process_auto);
                //double[] check_Curr_ = new double[6];
                //SDKHrobot.HRobot.get_current_position(handle, check_Curr_);
                //int success3 = -1;
                //if (check_Curr_[0] == Global.Ready_Place_Tray_3[0] && check_Curr_[1] == Global.Ready_Place_Tray_3[1] && check_Curr_[2] == Global.Ready_Place_Tray_3[2] && check_Curr_[5] == Global.Ready_Place_Tray_3[5] && stop_change != 1)
                //{
                //    Process_Auto("ready place tray 2 OK", file_process_auto);
                //    StatusDisplay.Instance.Update_process(txt_process, "Wait 1");
                //    success3 = SDKHrobot.HRobot.lin_pos(handle, 1, 5, Ready_Place_Tray_2);
                //    if(success3 == 0)
                //    {
                //        SDKHrobot.HRobot.lin_pos(handle, 1, 5, Ready_Check_Camtop_1);
                //        FuncRobot.Wait_for_stop_motion_2(handle);
                //        SDKHrobot.HRobot.lin_pos(handle, 1, 5, Ready_Arc_2);
                //        FuncRobot.Wait_for_stop_motion_2(handle);
                //        SDKHrobot.HRobot.lin_pos(handle, 1, 5, Ready_Arc_1);
                //        FuncRobot.Wait_for_stop_motion_2(handle);
                //        SDKHrobot.HRobot.lin_pos(handle, 1, 5, Homee);
                //        FuncRobot.Wait_for_stop_motion_2(handle);
                //    }
                //}               
                //else
                //{
                //    SDKHrobot.HRobot.motion_abort(handle);
                //    Process_Auto("ready place tray 2 Error", file_process_auto);
                //    stop_change = 1;
                //    stopOn();
                //}

            }

        }
        private void CMD_Transfer_Tray()
        {
            Process_Auto("CMD_Transfer_Tray start", file_process_auto);
            if (SDKHrobot.HRobot.get_digital_output(handle, 2) == 1) // call transfer tray
            {
                bool ss_vaccum_pick_tray1 = PLC1.Read_Data_Bit_("M" + (8526 + Memory_PLC.K300).ToString());
                if (ss_vaccum_pick_tray1 == false)
                {
                    StatusDisplay.Instance.Update_process(txt_process, "Hút tray chờ transfer tray");
                    PLC1.Write_DataBit_("M" + (9020 + Memory_PLC.K1000).ToString(), 1);// rb call hút tray chờ transfer                                                 
                }
                loading_TF_tray = 1;
            }
            Process_Auto("CMD_Transfer_Tray End", file_process_auto);
        }
        private void Motion_Pick_Press()
        {
            if (FuncRobot.CMD_Pick_Press == true && SDKHrobot.HRobot.get_digital_output(handle, 51) == 0 && stop_change != 1)
            {
                if (Global.combox_mode == 1)
                {
                    Mov_pick_press();
                    FuncRobot.CMD_Input = true;
                }
            }
            else if (PLC1.Read_Data_Bit_("L" + (90 + Memory_PLC.K100).ToString()) == true)
            {
                FuncRobot.CMD_Input = true;
            }
            FuncRobot.CMD_Input = true;
        }
        private void Motion_Input_FPCB()
        {
            if (FuncRobot.CMD_Input == true && stop_change != 1)
            {
                FuncRobot.Wait_For_Digital_Input_On(handle, 16);// remov complete
                Mov_Input_FBCP();
                FuncRobot.CMD_Input = false;
                FuncRobot.CMD_Check_Marking = true;
            }

        }
        private void Motion_Check_Marking()
        {

            if (FuncRobot.CMD_Check_Marking == true && stop_change != 1)
            {
                if (SDKHrobot.HRobot.get_digital_input(handle, 8) == 1 && SDKHrobot.HRobot.get_digital_input(handle, 5) == 1)
                {
                    FuncRobot.Wait_For_Digital_Input_On(handle, 5);// remov complete
                    Mov_Check_Maring_Tape();
                    FuncRobot.CMD_Check_Marking = false;
                    FuncRobot.CMD_Pick_Output = true;
                }
                else
                {
                    double[] current_ = new double[6];
                    SDKHrobot.HRobot.get_current_position(handle, current_);
                    if (current_[0] == Global.Ready_Inputput_1[0] && current_[1] == Global.Ready_Inputput_1[1] && current_[2] == Global.Ready_Inputput_1[2] && current_[5] == Global.Ready_Inputput_1[5])
                    {
                        SDKHrobot.HRobot.lin_pos(handle, 1, 10, Global.Ready_Check_Camtop_1);
                        SDKHrobot.HRobot.lin_pos(handle, 1, 10, Global.Ready_Arc_2);
                        FuncRobot.Wait_for_stop_motion(handle);
                        SDKHrobot.HRobot.lin_pos(handle, 1, 10, Global.Ready_Arc_1);
                        SDKHrobot.HRobot.lin_pos(handle, 1, 10, Global.Homee);
                        FuncRobot.Wait_for_stop_motion(handle);
                        PLC1.Write_DataBit_("L" + (39 + Memory_PLC.K100).ToString(), 1);
                        PLC1.Write_DataBit_("L" + (40 + Memory_PLC.K100).ToString(), 1);
                        PLC1.Write_DataBit_("L" + (31 + Memory_PLC.K100).ToString(), 1);
                    }
                }
            }
        }
        private void Motion_Pick_Output()
        {
            if (FuncRobot.CMD_Pick_Output == true && stop_change != 1)
            {
                Mov_Pick_FPCB_Output();
                FuncRobot.CMD_Pick_Output = false;
            }
        }
        private void Motion_Place_Tray()
        {
            int check_vaccum = SDKHrobot.HRobot.get_digital_output(handle, 51);
            if (check_vaccum == 1 && PLC1.Read_Data_Bit_("L" + (91 + Memory_PLC.K100).ToString()) == true)
            {
                SDKHrobot.HRobot.get_current_position(handle, check_Pos_curr);
                if (check_Pos_curr[0] == Global.Ready_Pick_Output_1[0] && check_Pos_curr[1] == Global.Ready_Pick_Output_1[1] && check_Pos_curr[2] == Global.Ready_Pick_Output_1[2] && check_Pos_curr[5] == Global.Ready_Pick_Output_1[5] && stop_change != 1)
                {
                    //SDKHrobot.HRobot.lin_pos(handle, 1, 10, Ready_Place_Tray_1);
                    SDKHrobot.HRobot.lin_pos(handle, 1, 10, Global.Ready_Place_Tray_2);
                    FuncRobot.Wait_for_stop_motion(handle);
                    Mov_Place_Tray();

                }
                else if (check_Pos_curr[0] == Global.Homee[0] && check_Pos_curr[1] == Global.Homee[1] && check_Pos_curr[2] == Global.Homee[2] && check_Pos_curr[5] == Global.Homee[5] && stop_change != 1)
                {
                    FuncRobot.Wait_For_Digital_Input_On(handle, 16);
                    PLC1.Write_DataBit_("L" + (39 + Memory_PLC.K100).ToString(), 0);
                    PLC1.Write_DataBit_("L" + (40 + Memory_PLC.K100).ToString(), 0);
                    SDKHrobot.HRobot.lin_pos(handle, 1, 10, Global.Ready_Arc_1);
                    SDKHrobot.HRobot.lin_pos(handle, 1, 10, Global.Ready_Arc_2);
                    FuncRobot.Wait_for_stop_motion(handle);
                    SDKHrobot.HRobot.lin_pos(handle, 1, 10, Global.Ready_Check_Camtop_1);
                    //SDKHrobot.HRobot.lin_pos(handle, 1, 10, Ready_Place_Tray_1);
                    SDKHrobot.HRobot.lin_pos(handle, 1, 10, Global.Ready_Place_Tray_2);
                    FuncRobot.Wait_for_stop_motion(handle);
                    Mov_Place_Tray();
                }
            }
        }
        private int Case_input_tray_matrix3_2(int data)
        {
            int data_output = 0;
            switch (data)
            {
                case 1:
                    data_output = 6;
                    break;
                case 2:
                    data_output = 7;
                    break;
                case 3:
                    data_output = 8;
                    break;
                case 4:
                    data_output = 9;
                    break;
                case 5:
                    data_output = 10;
                    break;
                case 6:
                    data_output = 11;
                    break;
                case 7:
                    data_output = 12;
                    break;
                case 8:
                    data_output = 13;
                    break;
                case 9:
                    data_output = 14;
                    break;
                case 10:
                    data_output = 20;
                    break;
                case 11:
                    data_output = 21;
                    break;
                case 12:
                    data_output = 22;
                    break;
                case 13:
                    data_output = 23;
                    break;
                case 14:
                    data_output = 24;
                    break;
                case 15:
                    data_output = 25;
                    break;
                case 16:
                    data_output = 26;
                    break;
                case 17:
                    data_output = 27;
                    break;
                case 18:
                    data_output = 28;
                    break;
                default:
                    data_output = 29;
                    break;
            }
            return data_output;
        }
        //program 2
        private void Start_On2()
        {
            StatusDisplay.Instance.Update_process(txt_process, "Load process start");
            Load_Process_Old();
            while ((n < stop_index && SDKHrobot.HRobot.get_digital_input(handle, 50) == 1) && stop_change != 1)
            {
                if (SDKHrobot.HRobot.get_digital_output(handle, 2) == 1)
                {
                    CMD_Transfer_Tray();
                }
                while (stop_change != 1 && SDKHrobot.HRobot.get_digital_input(handle, 50) == 1)
                {
                    StatusDisplay.Instance.Update_process(txt_process, "process start");
                    Check_Status_MC3();
                    timer_all = new Stopwatch();
                    timer_all.Start();
                    Motion_Check_Inspection2();
                    Motion_Pick_Output2();
                    Execution_Transfer_Tray2();
                    Motion_Place_Tray2();
                    OK_NG_NA = false;
                    timer_all.Stop();

                    Global.CycleTime_arr[0] = Math.Round(timer_all.Elapsed.TotalSeconds, 3);
                    if (Global.CycleTime_arr[0] != 0)
                    {
                        ConnectSQLite();
                        Array.Copy(Global.CycleTime_arr, 0, Global.CycleTime_arr, 1, 9);
                        Write_TactTime(Global.CycleTime_arr);
                        DisConSQLite();
                    }
                }
            }
        }
        private void Status_MC2()
        {
            if (Global.combox_mode == 0)
            {
                StatusDisplay.Instance.Enable_Button(btn_CMD_Pick, 1);
            }
            Mov_Pick_bool = false;
            while (stop_change != 1 && FuncRobot.Brake_While == false)
            {
                if (SDKHrobot.HRobot.get_digital_output(handle, 5) == 1)//call check vision top

                {
                    StatusDisplay.Instance.Update_process(txt_process, "CMD Check Top");
                    FuncRobot.CMD_Check_Marking = true;
                    FuncRobot.Brake_While = true;
                }
                else if (FuncRobot.CMD_Check_Marking == true && Global.combox_mode == 0)
                {
                    FuncRobot.Brake_While = true;
                }
                Machine_Pause_(stop_change);
            }
        }
        private void Check_Status_MC3()
        {
            //MC1
            //DI1 FLAG FPCB BUFFER INPUT-Y1066
            //DI2 FLAG FPCB TOOL CHECK VISION CAM BOT-Y1067
            //DI3 -Y1068
            //DI 4 PLAG TRANSFER TRAY OK -Y1069
            //DI5 SMEMA CALL CHECK MARKING-Y106A
            //DI6 CHECK MARKING COMPLETE -Y106B
            //DI7 SMEMA CALL ROBOT 1 PICK FPCB PRESS-Y106C
            //DI 8 Flag jig Remov-Y106D
            //DI9 SMEMA CALL INPUT FPCB-Y106E
            //DI16 CMD-RB pick/insert-Y106F
            //DO51 power vaccum tool rb
            if (Global.combox_mode == 0)
            {
                StatusDisplay.Instance.Enable_Button(btn_CMD_Pick, 1);
            }
            StatusDisplay.Instance.Update_process(txt_process, "Search Status Machine");
            while (stop_change != 1 && SDKHrobot.HRobot.get_digital_input(handle, 50) == 1 && FuncRobot.Brake_While == false)
            {
                Thread.Sleep(70);
                if ((SDKHrobot.HRobot.get_digital_input(handle, 6) == 1 || SDKHrobot.HRobot.get_digital_output(handle, 3) == 1) &&
                    SDKHrobot.HRobot.get_digital_output(handle, 51) == 0 && SDKHrobot.HRobot.get_digital_input(handle, 5) == 0)
                {
                    StatusDisplay.Instance.Update_process(txt_process, "CMD Pick Output ");
                    FuncRobot.CMD_Pick_Output = true;
                    Mov_Pick_bool = true;
                    FuncRobot.CMD_Pick_Press = false;
                    FuncRobot.CMD_Check_Marking = false;
                    FuncRobot.Brake_While = true;
                }
                else if (FuncRobot.CMD_Pick_Output == false && SDKHrobot.HRobot.get_digital_input(handle, 5) == 1 && SDKHrobot.HRobot.get_digital_output(handle, 51) == 0)
                {
                    StatusDisplay.Instance.Update_process(txt_process, "CMD Check Marking");
                    FuncRobot.CMD_Check_Marking = true;
                    FuncRobot.Brake_While = true;
                }
                else if (SDKHrobot.HRobot.get_digital_output(handle, 51) == 1 && stop_change != 1)
                {
                    FuncRobot.CMD_Pick_Output = false;
                    Mov_Pick_bool = false;
                    FuncRobot.CMD_Check_Marking = false;
                    FuncRobot.Brake_While = true;
                }
                //else if (FuncRobot.CMD_Pick_Press == false && FuncRobot.CMD_Check_Marking == false && FuncRobot.CMD_Pick_Output == false
                //    && SDKHrobot.HRobot.get_digital_output(handle, 51) == 0 && Global.combox_mode == 1 && FuncRobot.Flag_Wait_Pick_Press == false)
                //{
                //    StatusDisplay.Instance.Update_process(txt_process, "Wait Pick Press");
                //    SDKHrobot.HRobot.set_digital_output(handle, 5, true);//call press
                //    FuncRobot.Flag_Wait_Pick_Press = true;
                //}
                else if (FuncRobot.CMD_Pick == true && FuncRobot.CMD_Pick_Output == false && Global.combox_mode == 0)
                {
                    FuncRobot.Brake_While = true;
                }
                Machine_Pause_(stop_change);
            }
            FuncRobot.Brake_While = false;
            //else if (SDKHrobot.HRobot.get_digital_output(handle, 51) == 0 && SDKHrobot.HRobot.get_digital_output(handle, 2) == 0 && SDKHrobot.HRobot.get_digital_output(handle, 1) == 0 && FuncRobot.CMD_Pick_Press == false)
            //{
            //    FuncRobot.CMD_Pick_Press = true;
            //}
            //else if (SDKHrobot.HRobot.get_digital_output(handle, 51) == 1 && SDKHrobot.HRobot.get_digital_output(handle, 9) == 1 && SDKHrobot.HRobot.get_digital_output(handle, 2) == 0
            //    && SDKHrobot.HRobot.get_digital_output(handle, 1) == 0)
            //{
            //    if (SDKHrobot.HRobot.get_digital_output(handle, 6) == 1)
            //    {
            //        FuncRobot.Wait_For_Digital_Input_On(handle, 3);
            //        FuncRobot.CMD_Input = true;
            //        FuncRobot.CMD_Check_Marking = true;
            //    }
            //    else
            //    {
            //        FuncRobot.CMD_Input = true;
            //        FuncRobot.CMD_Check_Marking = false;
            //    }
            //    //Input FPCB Jig Input

            //}           
        }
        private void Execution_Transfer_Tray2()
        {
            Process_Auto(" Execution_Transfer_Tray Start", file_process_auto);
            if (loading_TF_tray == 1 && SDKHrobot.HRobot.get_digital_input(handle, 4) == 0)//di 11 flag TF tray
            {
                StatusDisplay.Instance.Update_process(txt_process, "Chờ insert FPCB");
                int check2 = SDKHrobot.HRobot.get_digital_output(handle, 57); // flag hold transfer tray
                if (check2 == 1)
                {
                    if (SDKHrobot.HRobot.get_digital_input(handle, 4) == 0)// flag tf tray
                    {
                        StatusDisplay.Instance.Update_process(txt_process, "Wait Transfer tray...");
                        SDKHrobot.HRobot.set_digital_output(handle, 2, true);//RB CALL PLC TRANSFER TRAY -X1031
                        FuncRobot.Wait_For_Digital_Input_On(handle, 4);// flag transfer tray hoàn thành
                        PLC1.Write_DataBit_("M" + (9020 + Memory_PLC.K1000).ToString(), 0);//off rb call pick tray chờ transfer
                        SDKHrobot.HRobot.set_digital_output(handle, 2, false);//off
                        FuncRobot.Wait_For_Digital_Input_On(handle, 3);//free robot
                        n = 1;
                        ind_n1 = 1;
                        ind_n2 = 1;
                        ind_n3 = 1;
                        //ind_n4 = 1;
                        SDKHrobot.HRobot.set_counter(handle, 2, n);
                        SDKHrobot.HRobot.set_counter(handle, 3, ind_n1);
                        SDKHrobot.HRobot.set_counter(handle, 4, ind_n2);
                        SDKHrobot.HRobot.set_counter(handle, 5, ind_n3);
                        // SDKHrobot.HRobot.set_counter(handle, 6, ind_n4);
                        loading_TF_tray = 0;
                        PLC1.Write_DataBit_("L" + (30 + Memory_PLC.K100).ToString(), 0);
                        SDKHrobot.HRobot.set_digital_output(handle, 57, false);//flag đầy tray check cam top NG ko?
                    }
                    else if (SDKHrobot.HRobot.get_digital_input(handle, 4) == 1) //flag transfer tray complete
                    {
                        StatusDisplay.Instance.Update_process(txt_process, "Transfer tray hoàn thành - reset FPCB trên tray");
                        PLC1.Write_DataBit_("M" + (9020 + Memory_PLC.K1000).ToString(), 0);//off rb call pick tray chờ transfer
                        SDKHrobot.HRobot.set_digital_output(handle, 2, false);//off
                        n = 1;
                        ind_n1 = 1;
                        ind_n2 = 1;
                        ind_n3 = 1;
                        //ind_n4 = 1;
                        SDKHrobot.HRobot.set_counter(handle, 2, n);
                        SDKHrobot.HRobot.set_counter(handle, 3, ind_n1);
                        SDKHrobot.HRobot.set_counter(handle, 4, ind_n2);
                        SDKHrobot.HRobot.set_counter(handle, 5, ind_n3);
                        //SDKHrobot.HRobot.set_counter(handle, 6, ind_n4);
                        loading_TF_tray = 0;
                        PLC1.Write_DataBit_("L" + (30 + Memory_PLC.K100).ToString(), 0);
                        SDKHrobot.HRobot.set_digital_output(handle, 57, false);//flag đầy tray check cam top NG ko?
                    }
                }
            }
            else if (loading_TF_tray == 1 & SDKHrobot.HRobot.get_digital_input(handle, 4) == 1)
            {
                n = 1;
                ind_n1 = 1;
                ind_n2 = 1;
                ind_n3 = 1;
                //ind_n4 = 1;
                SDKHrobot.HRobot.set_counter(handle, 2, n);
                SDKHrobot.HRobot.set_counter(handle, 3, ind_n1);
                SDKHrobot.HRobot.set_counter(handle, 4, ind_n2);
                SDKHrobot.HRobot.set_counter(handle, 5, ind_n3);
                //SDKHrobot.HRobot.set_counter(handle, 6, ind_n4);
                PLC1.Write_DataBit_("L" + (20 + Memory_PLC.K100).ToString(), 1);//off

                loading_TF_tray = 0;
                SDKHrobot.HRobot.set_digital_output(handle, 57, false);//flag đầy tray check cam top NG ko?                            
                StatusDisplay.Instance.Update_process(txt_process, "Transfer tray hoàn thành - reset FPCB trên tray");
                FuncRobot.Wait_For_Digital_Input_On(handle, 3);//free robot
                FuncRobot.Wait_For_Digital_Input_On(handle, 16);//free robot
            }

            Process_Auto(" Execution_Transfer_Tray End", file_process_auto);
        }
        private void Check_Inspection2()
        {
            if (Global.combox_mode == 0 && FuncRobot.CMD_Check_Marking == true && SDKHrobot.HRobot.get_digital_output(handle, 51) == 0)
            {
                PLC1.Write_DataBit_("M" + (9085 + Memory_PLC.K1000).ToString(), 0);// manual check inspection
                FuncRobot.CMD_Check_Marking = false;
            }
            else if (Global.combox_mode == 1)
            {
                FuncRobot.CMD_Pick = false;
                StatusDisplay.Instance.Update_process(txt_process, "Check Inspection Start");
                Process_Auto("Check Inspection Start", file_process_auto);
                FuncRobot.Wait_For_Digital_Input_On(handle, 5);// Call check inspection
                FuncRobot.Wait_For_Digital_Input_On(handle, 16);
                PLC1.Write_DataBit_("L" + (39 + Memory_PLC.K100).ToString(), 0);// stop output fpcb
                Thread.Sleep(20);
            }
            if (SDKHrobot.HRobot.get_digital_input(handle, 5) == 1 && checkBox_camtop.Checked == false)
            {
                Cycle_time_vision = new Stopwatch();
                Cycle_time_vision.Start();

                SDKHrobot.HRobot.get_current_position(handle, check_Pos_curr);
                if (check_Pos_curr[0] == Global.Ready_Place_Tray_1[0] && check_Pos_curr[1] == Global.Ready_Place_Tray_1[1] &&
                    check_Pos_curr[2] == Global.Ready_Place_Tray_1[2] && check_Pos_curr[5] == Global.Ready_Place_Tray_1[5] ||
                    (check_Pos_curr[0] == Global.Homee[0] && check_Pos_curr[1] == Global.Homee[1] &&
                    check_Pos_curr[2] == Global.Homee[2] && check_Pos_curr[5] == Global.Homee[5]))
                {
                    StatusDisplay.Instance.Update_process(txt_process, "Check Marking Start");
                    PLC1.Write_DataBit_("L" + (39 + Memory_PLC.K100).ToString(), 0);// stop output fpcb
                    SDKHrobot.HRobot.lin_pos(handle, 1, 20, Global.Ready_Check_Camtop_1);
                    SDKHrobot.HRobot.lin_pos(handle, 1, 20, Global.Check_Marking_Start);
                    FuncRobot.Wait_for_stop_motion_2(handle);
                    if (CheckBox_UseCylinderCamtop.Checked == true)
                    {
                        SDKHrobot.HRobot.set_digital_output(handle, 50, true);//down cylinder camtop
                    }
                    for (int i = 0; i < Global.Total_Check_Marking; i++)
                    {
                        double[] pos_camtop_n = new double[6];
                        pos_camtop_n = matrix.PAL_P_RB_Cam_Top(1, i + 1);
                        if (pos_camtop_n[2] == Global.Check_Marking_Start[2] && matrix.Flag_Read_Data_Maxtrix_Tool_RB == true && stop_change != 1)
                        {
                            if (pos_camtop_n[0] < Global.X_Camtop_Satefy_U && pos_camtop_n[0] > Global.X_Camtop_Satefy_L && pos_camtop_n[1] < Global.Y_Camtop_Satefy_U && pos_camtop_n[1] > Global.Y_Camtop_Satefy_L
                                && pos_camtop_n[2] > Global.Z_Camtop_Satefy)
                            {
                                SDKHrobot.HRobot.lin_pos(handle, 1, 50, pos_camtop_n);
                                FuncRobot.Wait_for_stop_motion_2(handle);
                                if (CheckBox_UseCylinderCamtop.Checked == true)
                                {
                                    StatusDisplay.Instance.Update_process(txt_process, "SS Down Cam Top ...");
                                    FuncRobot.Wait_For_Digital_Input_On(handle, 57);
                                }
                                else
                                {
                                    StatusDisplay.Instance.Update_process(txt_process, "SS Up Cam Top ...");
                                    FuncRobot.Wait_For_Digital_Input_On(handle, 56);
                                }
                                Thread.Sleep(300);
                                if (uiRadioButton_ViewMuti.Checked == true)
                                {
                                    Send_data_Cam1("110" + (1 + i * 2).ToString());// lan 1 cam 1 -101
                                    Send_data_Cam2("210" + (1 + i * 2).ToString());// lan 1 cam 2 -201                                   
                                }
                                else if (uiRadioButton_ViewSign.Checked == true)
                                {
                                    Send_data_Cam1("110" + (1 + i).ToString());// lan 1 cam 1 -101
                                    Send_data_Cam2("210" + (1 + i).ToString());// lan 1 cam 2 -201
                                }
                                else if (uiRadioButton_ViewMuti.Checked == false && uiRadioButton_ViewSign.Checked == false)
                                {
                                    Message_Box_Error("Chưa select view check cam top", "Camera Top");
                                    stop_change = 1;
                                    stopOn();
                                }
                                StatusDisplay.Instance.Update_process(txt_process, "Trigger Camera Tool: " + (i + 1).ToString());
                                //FuncRobot.Flag_Cam1 = 1;
                                //FuncRobot.Flag_Cam2 = 1;
                                FuncRobot.Wait_For_Flag_Cam();
                                FuncRobot.Flag_Cam1 = 0;
                                FuncRobot.Flag_Cam2 = 0;
                                if (CheckBox_MasterCheck.Checked == true)
                                {
                                    StatusDisplay.Instance.Update_process(txt_process, "Check master : nhấn continue để tiếp tục");
                                    FuncRobot.Wait_For_CheckMaster();
                                    FuncRobot.Check_Master = 0;
                                }
                            }
                            else
                            {
                                Process_Auto("Error position Check_Marking_Start", file_process_auto);
                                stop_change = 1;
                                stopOn();
                            }
                        }
                        else
                        {
                            Process_Auto("Error position Check_Marking_Start", file_process_auto);
                            stop_change = 1;
                            stopOn();
                        }
                    }
                    SDKHrobot.HRobot.set_digital_output(handle, 50, false);
                    SDKHrobot.HRobot.lin_pos(handle, 1, 30, Global.Pick_Press);
                }
                Process_Auto("Check Inspection End", file_process_auto);
                FuncRobot.Wait_for_stop_motion(handle);
                Mov_Pick_bool = true;
                Global.CMD_Scan_Flag = true;
                Cycle_time_vision.Stop();
                Global.Time_Vision = Math.Round(Cycle_time_vision.Elapsed.TotalSeconds, 3);
            }
            else if (SDKHrobot.HRobot.get_digital_input(handle, 5) == 1 && checkBox_camtop.Checked == true)
            {
                Cycle_time_vision = new Stopwatch();
                Cycle_time_vision.Start();

                SDKHrobot.HRobot.get_current_position(handle, check_Pos_curr);
                if (check_Pos_curr[0] == Global.Ready_Place_Tray_1[0] && check_Pos_curr[1] == Global.Ready_Place_Tray_1[1] &&
                    check_Pos_curr[2] == Global.Ready_Place_Tray_1[2] && check_Pos_curr[5] == Global.Ready_Place_Tray_1[5] ||
                    (check_Pos_curr[0] == Global.Homee[0] && check_Pos_curr[1] == Global.Homee[1] &&
                    check_Pos_curr[2] == Global.Homee[2] && check_Pos_curr[5] == Global.Homee[5]))
                {
                    StatusDisplay.Instance.Update_process(txt_process, "Pick");
                    PLC1.Write_DataBit_("L" + (39 + Memory_PLC.K100).ToString(), 0);// stop output fpcb
                    SDKHrobot.HRobot.lin_pos(handle, 1, 20, Global.Ready_Check_Camtop_1);
                    SDKHrobot.HRobot.set_digital_output(handle, 50, false);
                    SDKHrobot.HRobot.lin_pos(handle, 1, 30, Global.Pick_Press);
                    FuncRobot.Wait_for_stop_motion(handle);
                    Mov_Pick_bool = true;
                    Global.CMD_Scan_Flag = true;
                    Cycle_time_vision.Stop();
                    Global.Time_Vision = Math.Round(Cycle_time_vision.Elapsed.TotalSeconds, 3);
                }
            }
        }
        private void Pick_FPCB2()
        {
            if (Mov_Pick_bool == true && stop_change != 1 && SDKHrobot.HRobot.get_digital_output(handle, 51) == 0)
            {
                SDKHrobot.HRobot.get_current_position(handle, check_Pos_curr);
                if (check_Pos_curr[0] == Global.Pick_Press[0] && check_Pos_curr[1] == Global.Pick_Press[1] &&
                    check_Pos_curr[2] == Global.Pick_Press[2] && check_Pos_curr[5] == Global.Pick_Press[5])
                {
                    if (stop_change != 1)
                    {
                        StatusDisplay.Instance.Update_process(txt_process, "Pick FPCB");
                        StatusDisplay.Instance.Update_process(txt_process, "Sensor Cam Top UP ...");
                        FuncRobot.Wait_For_Digital_Input_On(handle, 56);
                        SDKHrobot.HRobot.lin_pos(handle, 0, 0, Global.Pick_Press);
                        FuncRobot.Wait_for_stop_motion_2(handle);
                        //down tool                 
                        double[] wait_pick1 = new double[6] { Global.Z_Pick_Press[0], Global.Z_Pick_Press[1], Global.Z_Pick_Press[2] + Global.Offset_Z, Global.Z_Pick_Press[3], Global.Z_Pick_Press[4], Global.Z_Pick_Press[5] };
                        SDKHrobot.HRobot.ptp_pos(handle, 1, wait_pick1);
                        FuncRobot.Wait_for_stop_motion_2(handle);
                        SDKHrobot.HRobot.set_override_ratio(handle, Global.SP_Wait_Pick);
                        SDKHrobot.HRobot.ptp_pos(handle, 1, Global.Z_Pick_Press);
                        FuncRobot.Wait_for_stop_motion_2(handle);
                        SDKHrobot.HRobot.set_operation_mode(handle, 0);
                        SDKHrobot.HRobot.set_override_ratio(handle, Global.SP_Wait_Pick);
                        //vaccum                  
                        PLC1.Write_DataBit_("M" + (9080 + Memory_PLC.K1000).ToString(), 1);
                        SDKHrobot.HRobot.set_digital_output(handle, 51, true);//vaccum
                        PLC1.Write_DataBit_("M" + (9440 + Memory_PLC.K1000).ToString(), 1);//rst buffer output
                        Thread.Sleep(Global.delay_hut);
                        PLC1.Write_DataBit_("L" + (94 + Memory_PLC.K100), 1);
                        OK_NG_NA = true;
                        SDKHrobot.HRobot.ptp_pos(handle, 1, wait_pick1);
                        FuncRobot.Wait_for_stop_motion_2(handle);
                        SDKHrobot.HRobot.set_operation_mode(handle, Global.Mode_run_auto);
                        SDKHrobot.HRobot.set_override_ratio(handle, Global.SP_auto_RB);
                        int check = SDKHrobot.HRobot.get_digital_output(handle, 51);
                        if (check == 1)
                        {
                            SDKHrobot.HRobot.ptp_pos(handle, 1, Global.Pick_Press);
                            FuncRobot.Wait_for_stop_motion(handle);
                            int success = SDKHrobot.HRobot.lin_pos(handle, 1, 20, Global.Ready_Place_Tray_1);
                            FuncRobot.Wait_for_stop_motion_2(handle);
                            //PLC1.Write_DataBit_("L" + (94 + Memory_PLC.K100), 1);
                            SDKHrobot.HRobot.set_digital_output(handle, 3, false);
                            PLC1.Write_DataBit_("L" + (95 + Memory_PLC.K100).ToString(), 0);
                            if (success == 0)
                            {
                                //reset bit call pick
                            }
                            else
                            {
                                stop_change = 1;
                                stopOn();
                                MessageBox.Show("Disconnect Robot");
                            }
                        }
                    }
                }
                Mov_Pick_bool = false;
            }
            else
            {
                StatusDisplay.Instance.Update_process(txt_process, "mov pick = false");
                stop_change = 1;
                stopOn();
            }
            Process_Auto("Pick End", file_process_auto);
        }
        private void Place2()
        {
            Cycle_time_place = new Stopwatch();
            Cycle_time_place.Start();
            StatusDisplay.Instance.Update_process(txt_process, "Wait PLC(Check Tray Output)");
            PLC1.Write_DataBit_("L" + (31 + Memory_PLC.K100).ToString(), 0);// nguy hiem tray                                         
            FuncRobot.Wait_For_Digital_Input_On(handle, 3); //free place
            FuncRobot.Wait_For_Digital_Input_On(handle, 16); //free place
            FuncRobot.Wait_for_stop_motion_2(handle);
            StatusDisplay.Instance.Update_process(txt_process, "Wait SS UP Camera Top ");
            FuncRobot.Wait_For_Digital_Input_On(handle, 56); //sS UP camtop
            if (CheckBox_Cylinder_BoxNG.Checked == true)
            {
                StatusDisplay.Instance.Update_process(txt_process, "Wait SS back Box NG ");
                FuncRobot.Wait_For_Digital_Input_On(handle, 12); //sS back BoxNG
            }
            bool check_data_vision_ = Global.result_Cam_Bot.Contains(0);
            if (check_data_vision_ == true)
            {
                Global.CMD_Scan_Flag = true;
            }
            //SDKHrobot.HRobot.lin_pos(handle, 1, 30, Global.Ready_Place_Tray_1);
            //SDKHrobot.HRobot.lin_pos(handle, 1, 30, Global.Ready_Place_Tray_2);
            if (OK_NG_NA == true)
            {
                Search_OK_NG_NA();
                OK_NG_NA = false;
            }
            double[] current_check = new double[6];
            int check_curr = SDKHrobot.HRobot.get_current_position(handle, current_check);
            if (check_curr != 0)
            {
                SDKHrobot.HRobot.motion_abort(handle);
                stop_change = 1;
                stopOn();
            }

            if ((check_Pos_curr[0] == Global.Homee[0] && check_Pos_curr[1] == Global.Homee[1] && check_Pos_curr[2] == Global.Homee[2] && check_Pos_curr[5] == Global.Homee[5]) ||
                (current_check[0] == Global.Ready_Place_Tray_1[0] && current_check[1] == Global.Ready_Place_Tray_1[1] &&
                current_check[2] == Global.Ready_Place_Tray_1[2] && current_check[5] == Global.Ready_Place_Tray_1[5]) && stop_change != 1)
            {
                Process_Auto("Place Tray", file_process_auto);
                int vaccum = SDKHrobot.HRobot.get_digital_output(handle, 51);
                int check1 = SDKHrobot.HRobot.get_digital_output(handle, 57);//flag đầy tray
                if (check1 == 0 && n < stop_index && vaccum == 1)
                {
                    FuncRobot.Wait_For_Digital_Input_On(handle, 3); //smema plc call insert tray                                                                                 
                    FuncRobot.Wait_for_stop_motion_2(handle);
                    check_data_vision_ = Global.result_Cam_Bot.Contains(1);
                    // bool check_no_data = Global.result_Cam_Bot.Contains(0);
                    if (check_data_vision_ == true)//|| check_no_data == true)
                    {
                        Place_NG_2();
                    }
                    Global.Security_Place = true;
                }
                Vaccum_Tool1 = PLC1.Read_Word_Arr("D" + (7031 + Memory_PLC.K100).ToString(), Global.Number_Tool);
                int select_tool_flag = 0;
                int[] bool_flag_scan = new int[Global.Number_Tool];
                Global.CMD_Scan_Flag = true;
                if (n < stop_index && stop_change != 1)
                {
                    #region MAXTRIX 1
                    while (select_tool_flag < Global.Number_Tool && stop_change != 1)
                    {
                        for (int i = 0; i < Global.Number_Tool; i++)
                        {
                            if (n < stop_index && stop_change != 1)
                            {
                                if (Global.result_Cam_Bot[i] == 2 && Vaccum_Tool1[i] == 1)
                                {
                                    bool_flag_scan[i] = 1;
                                    double[] pos_tool1 = new double[6];
                                    if (Global.Row_tray_matrix1 > 0 && Global.Column_tray_matrix1 > 0)
                                    {
                                        pos_tool1 = matrix.PAL_P_RB_1_1(1 + i, n, Global.Row_tray_matrix1, Global.Column_tray_matrix1);
                                    }
                                    else
                                    {
                                        StatusDisplay.Instance.Update_process(txt_process, "Row tray và Column tray = 0");
                                        SDKHrobot.HRobot.motion_abort(handle);
                                        stop_change = 1;
                                        stopOn();
                                    }
                                    StatusDisplay.Instance.Update_process(txt_process, "SS Up Cam Top ... ");
                                    FuncRobot.Wait_For_Digital_Input_On(handle, 56);
                                    if (pos_tool1[2] == Global.Place_Tray_1[2])
                                    {
                                        if (matrix.Flag_Read_Data_Maxtrix_Tool_RB == true && pos_tool1[0] > Global.X_Place_Satefy_L && pos_tool1[0] < Global.X_Place_Satefy_U && pos_tool1[1] < Global.Y_Place_Satefy_U && pos_tool1[1] > Global.Y_Place_Satefy_L && pos_tool1[2] > Global.Z_Place_Satefy)
                                        {
                                            SDKHrobot.HRobot.lin_pos(handle, 1, 30, pos_tool1);
                                            FuncRobot.Wait_for_stop_motion_2(handle);
                                            if (CheckBox_SingleBlow.Checked == true)
                                            {
                                                PLC1.Write_DataBit_("M" + (9066 + Memory_PLC.K1000).ToString(), 1);//single blow                                                
                                                Thread.Sleep(100);
                                                SDKHrobot.HRobot.set_digital_output(handle, 52, true);//power blow
                                            }
                                            if (CheckBox_MultiBlow.Checked == true)
                                            {
                                                SDKHrobot.HRobot.set_digital_output(handle, 52, true);//power blow
                                            }
                                            PLC1.Write_Data_Word_("D" + (7031 + i + Memory_PLC.K100).ToString(), 0);//blow index tool
                                            Thread.Sleep(Global.delay_tha);
                                            if (CheckBox_MultiBlow.Checked == true)
                                            {
                                                SDKHrobot.HRobot.set_digital_output(handle, 52, false);//power blow 
                                            }
                                            Vaccum_Tool1[i] = 0;
                                            Global.result_Cam_Bot[i] = 0;
                                            PLC1.Write_Data_Word_("D" + (9250 + i + Memory_PLC.K2000).ToString(), 0);
                                            if (CheckBox_SingleBlow.Checked == true)
                                            {
                                                SDKHrobot.HRobot.set_digital_output(handle, 52, false);//power blow
                                                PLC1.Write_DataBit_("M" + (9067 + Memory_PLC.K1000).ToString(), 1);
                                            }
                                            n = n + 1;
                                            SDKHrobot.HRobot.set_counter(handle, 2, n);
                                            select_tool_flag = select_tool_flag + 1;
                                        }
                                        else
                                        {
                                            Process_Auto("Error position Place tray,Over Limit", file_process_auto);
                                            SDKHrobot.HRobot.motion_abort(handle);
                                            stop_change = 1;
                                            stopOn();
                                        }
                                    }
                                    else
                                    {
                                        StatusDisplay.Instance.Update_process(txt_process, "Read data position SQL Matrix error");
                                        SDKHrobot.HRobot.motion_abort(handle);
                                        stop_change = 1;
                                        stopOn();
                                    }
                                }
                                else if (Global.result_Cam_Bot[i] == 1 && Vaccum_Tool1[i] == 1 && bool_flag_scan[i] == 0)
                                {
                                    bool_flag_scan[i] = 1;
                                    select_tool_flag = select_tool_flag + 1;
                                }
                                else if (Global.result_Cam_Bot[i] == 3 && Vaccum_Tool1[i] == 1 && bool_flag_scan[i] == 0)
                                {
                                    bool_flag_scan[i] = 1;
                                    select_tool_flag = select_tool_flag + 1;
                                }
                                else if (Global.result_Cam_Bot[i] == 0 && Vaccum_Tool1[i] == 0 && bool_flag_scan[i] == 0)
                                {
                                    bool_flag_scan[i] = 1;
                                    select_tool_flag = select_tool_flag + 1;
                                }
                                else if (Global.result_Cam_Bot[i] != 0 && Vaccum_Tool1[i] == 0 && bool_flag_scan[i] == 0)
                                {
                                    bool_flag_scan[i] = 1;
                                    select_tool_flag = select_tool_flag + 1;
                                }
                                else if (Global.result_Cam_Bot[i] == 0 && Vaccum_Tool1[i] == 1)
                                {
                                    bool_flag_scan[i] = 1;
                                    select_tool_flag = select_tool_flag + 1;
                                    //StatusDisplay.Instance.Update_process(txt_process, "Không có data tool " + (i + 1).ToString());
                                }
                                StatusDisplay.Instance.Update_process(txt_process, select_tool_flag.ToString());
                                Machine_Pause_(stop_change);
                            }
                            else
                            {
                                select_tool_flag = select_tool_flag + Global.Number_Tool;
                                i = i + Global.Number_Tool;
                            }
                        }
                    } // MATRIX 1
                    #endregion
                }
                if (n > 1 && SDKHrobot.HRobot.get_digital_input(handle, 4) == 1)//flag transfer tray
                {
                    PLC1.Write_DataBit_("L" + (30 + Memory_PLC.K100).ToString(), 0);// tín hiệu transfer tray
                }
                Global.Security_Place = false;
                Machine_Pause_(stop_change);
                Process_Auto("Ready_Place_1 Start", file_process_auto);
                int ready_place_1 = -1;
                ready_place_1 = SDKHrobot.HRobot.lin_pos(handle, 0, 0, Global.Ready_Place_Tray_1);
                FuncRobot.Wait_for_stop_motion_2(handle);
                if (ready_place_1 != 0)
                {
                    SDKHrobot.HRobot.motion_abort(handle);
                    Process_Auto("Ready_Place_1 Error", file_process_auto);
                    stop_change = 1;
                    stopOn();
                }
                if (ready_place_1 == 0)
                {
                    int[] Check_Vaccum_After_Place = new int[5];
                    int[] check_power_vaccum = new int[Global.Number_Tool];
                    check_power_vaccum = PLC1.Read_Word_Arr("D" + (7031 + Memory_PLC.K100).ToString(), Global.Number_Tool);
                    for (int i = 0; i < Global.Number_Tool; i++)
                    {
                        if (SDKHrobot.HRobot.get_digital_output(handle, 51) == 1 && (Global.result_Cam_Bot[i] == 1 || Global.result_Cam_Bot[i] == 3 || Global.result_Cam_Bot[i] == 0)
                            && check_power_vaccum[i] == 1)// hang NG
                        {
                            Check_Vaccum_After_Place[0]++;
                        }
                        else if (SDKHrobot.HRobot.get_digital_output(handle, 51) == 1 && Global.result_Cam_Bot[i] == 2 && check_power_vaccum[i] == 1)//hang ok
                        {
                            Check_Vaccum_After_Place[1]++;
                        }
                        else if (SDKHrobot.HRobot.get_digital_output(handle, 51) == 1 && (Global.result_Cam_Bot[i] == 0 || Global.result_Cam_Bot[i] == 3))
                        {
                            Check_Vaccum_After_Place[2]++;
                        }
                        else if (check_power_vaccum[i] == 0)
                        {
                            Check_Vaccum_After_Place[3]++;
                        }
                        if (SDKHrobot.HRobot.get_digital_output(handle, 51) == 1 && check_power_vaccum[i] == 0)
                        {
                            Check_Vaccum_After_Place[4]++;
                        }
                    }
                    if (Check_Vaccum_After_Place[2] == Global.Number_Tool || Check_Vaccum_After_Place[3] == Global.Number_Tool || Check_Vaccum_After_Place[4] == Global.Number_Tool)
                    {
                        SDKHrobot.HRobot.set_digital_output(handle, 51, false);
                    }
                    if (Check_Vaccum_After_Place[0] > 0)
                    {
                        // thả hàng NG
                        StatusDisplay.Instance.Update_process(txt_process, "Thả hàng NG");
                        bool_Check_tool_after_place = false;
                        FuncRobot.Flag_FPCB_After_Pick_Output_RB = false;
                        FuncRobot.Flag_FPCB_All_NG = true;
                        SDKHrobot.HRobot.lin_pos(handle, 1, 20, Global.Ready_Place_Tray_1);
                        FuncRobot.Wait_for_stop_motion(handle);
                        Place_NG();
                    }
                    if (Check_Vaccum_After_Place[1] > 0 && Check_Vaccum_After_Place[0] > 0 && n >= stop_index)
                    {
                        //đợi thả hết hàng ok vào tray
                        bool_Check_tool_after_place = true;
                        SDKHrobot.HRobot.lin_pos(handle, 1, 30, Global.Ready_Place_Tray_1);
                        FuncRobot.Wait_for_stop_motion_2(handle);
                        double[] check_pos1 = new double[6];
                        SDKHrobot.HRobot.get_current_position(handle, check_pos1);
                        if (n >= stop_index && check_pos1[0] == Global.Ready_Place_Tray_1[0] && check_pos1[1] == Global.Ready_Place_Tray_1[1] &&
                            check_pos1[2] == Global.Ready_Place_Tray_1[2] && check_pos1[5] == Global.Ready_Place_Tray_1[5])
                        {
                            PLC1.Write_DataBit_("L" + (31 + Memory_PLC.K100).ToString(), 1);
                        }
                        Check_FPCB_Place();
                        //Execution_Transfer_Tray();
                        Motion_Place_Tray2();
                    }
                    Machine_Pause_(stop_change);
                    //đợi thả hết hàng ok vào tray                 
                    double[] check_pos = new double[6];
                    SDKHrobot.HRobot.get_current_position(handle, check_pos);
                    if (n >= stop_index && check_pos[0] == Global.Ready_Place_Tray_1[0] && check_pos[1] == Global.Ready_Place_Tray_1[1] &&
                        check_pos[2] == Global.Ready_Place_Tray_1[2] && check_pos[5] == Global.Ready_Place_Tray_1[5])
                    {
                        PLC1.Write_DataBit_("L" + (31 + Memory_PLC.K100).ToString(), 1);
                    }
                    Check_FPCB_Place();
                    Motion_Place_Tray2();
                }
                else
                {
                    SDKHrobot.HRobot.motion_abort(handle);
                    stop_change = 1;
                    stopOn();
                }
                Machine_Pause_(stop_change);
                FuncRobot.Wait_for_stop_motion_2(handle);
                if (SDKHrobot.HRobot.get_digital_output(handle, 51) == 0)
                {
                    PLC1.Write_DataBit_("L" + (39 + Memory_PLC.K100).ToString(), 1);
                    SDKHrobot.HRobot.set_digital_output(handle, 52, false);
                }
            }
            Cycle_time_place.Stop();
            Global.Time_Place = Math.Round(Cycle_time_place.Elapsed.TotalSeconds, 3);

        }
        private void Motion_Check_Inspection2()
        {
            StatusDisplay.Instance.Update_process(txt_process, "Wait Check Top");
            if (FuncRobot.CMD_Check_Marking == true && stop_change != 1)
            {
                if (SDKHrobot.HRobot.get_digital_input(handle, 8) == 1 && SDKHrobot.HRobot.get_digital_input(handle, 5) == 1 &&
                    SDKHrobot.HRobot.get_digital_output(handle, 51) == 0)
                {
                    double[] current_ = new double[6];
                    SDKHrobot.HRobot.get_current_position(handle, current_);
                    if ((current_[0] == Global.Ready_Place_Tray_1[0] && current_[1] == Global.Ready_Place_Tray_1[1] &&
                        current_[2] == Global.Ready_Place_Tray_1[2] && current_[5] == Global.Ready_Place_Tray_1[5]) ||
                        (current_[0] == Global.Homee[0] && current_[1] == Global.Homee[1] &&
                        current_[2] == Global.Homee[2] && current_[5] == Global.Homee[5]))
                    {
                        FuncRobot.Wait_For_Digital_Input_On(handle, 5);// remov complete
                        Global.Total_Time_RunTime = Convert.ToDouble(PLC1.Read_Data_Word_("D" + (10934 + Memory_PLC.K2000).ToString())) / 100;
                        Global.Start_Time = DateTime.Now;
                        Global.Flag_Run_Time = true;
                        Check_Inspection2();
                        FuncRobot.CMD_Check_Marking = false;
                        FuncRobot.CMD_Pick_Output = true;
                        PLC1.Write_DataBit_("L" + (95 + Memory_PLC.K100).ToString(), 1);// check marking complt > pick output//check cam top complt
                    }
                    else
                    {
                        Process_Auto("Ready_Place_1 Fail", file_process_auto);
                        stop_change = 1;
                        stopOn();
                    }
                }

            }
        }
        private void Motion_Pick_Output2()
        {
            StatusDisplay.Instance.Update_process(txt_process, "Wait Pick");
            if (FuncRobot.CMD_Pick_Output == true && stop_change != 1 && SDKHrobot.HRobot.get_digital_output(handle, 51) == 0)
            {
                SDKHrobot.HRobot.set_digital_output(handle, 3, true);
                FuncRobot.Wait_For_Digital_Input_On(handle, 16);
                PLC1.Write_DataBit_("L" + (39 + Memory_PLC.K100).ToString(), 0);// stop output fpcb             
                SDKHrobot.HRobot.get_current_position(handle, check_Pos_curr);
                if (check_Pos_curr[0] == Global.Pick_Press[0] && check_Pos_curr[1] == Global.Pick_Press[1] &&
                    check_Pos_curr[2] == Global.Pick_Press[2] && check_Pos_curr[5] == Global.Pick_Press[5])
                {
                    Cycle_time_pick = new Stopwatch();
                    Cycle_time_pick.Start();
                    Pick_FPCB2();
                    Cycle_time_pick.Stop();
                    Global.Time_Pick = Math.Round(Cycle_time_pick.Elapsed.TotalSeconds, 3);
                }
                else if (check_Pos_curr[0] == Global.Homee[0] && check_Pos_curr[1] == Global.Homee[1] &&
                    check_Pos_curr[2] == Global.Homee[2] && check_Pos_curr[5] == Global.Homee[5])
                {
                    Cycle_time_pick = new Stopwatch();
                    Cycle_time_pick.Start();
                    FuncRobot.Wait_For_Digital_Input_On(handle, 16);
                    SDKHrobot.HRobot.set_digital_output(handle, 3, true);
                    SDKHrobot.HRobot.lin_pos(handle, 1, 20, Global.Ready_Check_Camtop_1);
                    SDKHrobot.HRobot.lin_pos(handle, 1, 20, Global.Pick_Press);
                    FuncRobot.Wait_for_stop_motion_2(handle);
                    PLC1.Write_DataBit_("L" + (39 + Memory_PLC.K100).ToString(), 0);// stop output fpcb                 
                    Pick_FPCB2();
                    Cycle_time_pick.Stop();
                    Global.Time_Pick = Math.Round(Cycle_time_pick.Elapsed.TotalSeconds, 3);
                }
                FuncRobot.CMD_Pick_Output = false;
                Mov_Pick_bool = false;
            }
        }
        private void Motion_Place_Tray2()
        {
            StatusDisplay.Instance.Update_process(txt_process, "Wait Place");
            int check_vaccum = SDKHrobot.HRobot.get_digital_output(handle, 51);
            if (check_vaccum == 1)
            {
                SDKHrobot.HRobot.get_current_position(handle, check_Pos_curr);
                if ((check_Pos_curr[0] == Global.Homee[0] && check_Pos_curr[1] == Global.Homee[1] && check_Pos_curr[2] == Global.Homee[2] && check_Pos_curr[5] == Global.Homee[5]) ||
                   (check_Pos_curr[0] == Global.Ready_Place_Tray_1[0] && check_Pos_curr[1] == Global.Ready_Place_Tray_1[1] && check_Pos_curr[2] == Global.Ready_Place_Tray_1[2] && check_Pos_curr[5] == Global.Ready_Place_Tray_1[5])
                   && stop_change != 1)
                {
                    FuncRobot.Wait_For_Digital_Input_On(handle, 16);
                    FuncRobot.Wait_For_Digital_Input_On(handle, 3);
                    PLC1.Write_DataBit_("L" + (39 + Memory_PLC.K100).ToString(), 0);
                    PLC1.Write_DataBit_("L" + (31 + Memory_PLC.K100).ToString(), 0);
                    FuncRobot.Wait_for_stop_motion(handle);
                    Place2();
                }
            }
        }
        #endregion
        #region Change_UI_Project----------------------------------------------------------
        public void Change_Label_INPUT()
        {
            //
            for (int i = 0; i < 80; i++) // 0..15
            {
                if (i < 16)
                {
                    string hex = i.ToString("X"); // 0 -> F
                    string name = $"I{i}";
                    string textValue = $"X10A{hex}";
                    var label = this.Controls.Find(name, true).FirstOrDefault() as UILight;
                    if (label != null)
                        label.Text = textValue;
                }
                else if (i >= 16 && i < 32)
                {
                    string hex = (i - 16).ToString("X"); // 0 -> F
                    string name = $"I{i}";
                    string textValue = $"X10B{hex}";
                    var label = this.Controls.Find(name, true).FirstOrDefault() as UILight;
                    if (label != null)
                        label.Text = textValue;
                }
                else if (i >= 32 && i < 48)
                {
                    string hex = (i - 32).ToString("X"); // 0 -> F
                    string name = $"I{i}";
                    string textValue = $"X10C{hex}";
                    var label = this.Controls.Find(name, true).FirstOrDefault() as UILight;
                    if (label != null)
                        label.Text = textValue;
                }
                else if (i >= 48 && i < 64)
                {
                    string hex = (i - 48).ToString("X"); // 0 -> F
                    string name = $"I{i}";
                    string textValue = $"X10D{hex}";
                    var label = this.Controls.Find(name, true).FirstOrDefault() as UILight;
                    if (label != null)
                        label.Text = textValue;
                }
                else if (i >= 64 && i < 80)
                {
                    string hex = (i - 64).ToString("X"); // 0 -> F
                    string name = $"I{i}";
                    string textValue = $"X115{hex}";
                    var label = this.Controls.Find(name, true).FirstOrDefault() as UILight;
                    if (label != null)
                        label.Text = textValue;
                }
            }
        }
        public void Change_Label_Output()
        {
            for (int i = 0; i < 96; i++) // 0..15
            {
                if (i < 16)
                {
                    string hex = i.ToString("X"); // 0 -> F
                    string name = $"Out{i}";
                    string textValue = $"Y10E{hex}";
                    var label = this.Controls.Find(name, true).FirstOrDefault() as UILight;
                    if (label != null)
                        label.Text = textValue;
                }
                else if (i >= 16 && i < 32)
                {
                    string hex = (i - 16).ToString("X"); // 0 -> F
                    string name = $"Out{i}";
                    string textValue = $"Y10F{hex}";
                    var label = this.Controls.Find(name, true).FirstOrDefault() as UILight;
                    if (label != null)
                        label.Text = textValue;
                }
                else if (i >= 32 && i < 48)
                {
                    string hex = (i - 32).ToString("X"); // 0 -> F
                    string name = $"Out{i}";
                    string textValue = $"Y110{hex}";
                    var label = this.Controls.Find(name, true).FirstOrDefault() as UILight;
                    if (label != null)
                        label.Text = textValue;
                }
                else if (i >= 48 && i < 64)
                {
                    string hex = (i - 48).ToString("X"); // 0 -> F
                    string name = $"Out{i}";
                    string textValue = $"Y111{hex}";
                    var label = this.Controls.Find(name, true).FirstOrDefault() as UILight;
                    if (label != null)
                        label.Text = textValue;
                }
                else if (i >= 64 && i < 80)
                {
                    string hex = (i - 64).ToString("X"); // 0 -> F
                    string name = $"Out{i}";
                    string textValue = $"Y112{hex}";
                    var label = this.Controls.Find(name, true).FirstOrDefault() as UILight;
                    if (label != null)
                        label.Text = textValue;
                }
                else if (i >= 80 && i < 96)
                {
                    string hex = (i - 80).ToString("X"); // 0 -> F
                    string name = $"Out{i}";
                    string textValue = $"Y113{hex}";
                    var label = this.Controls.Find(name, true).FirstOrDefault() as UILight;
                    if (label != null)
                        label.Text = textValue;
                }
            }
        }
        public void Change_Sensor()
        {
            //group remov
            X1011.Text = "X10B1";
            X1012.Text = "X10B2";
            X1013.Text = "X10B3";
            X1014.Text = "X10B4";
            X1015.Text = "X10B5";
            //group tray
            X1007.Text = "X10A7";
            X1008.Text = "X10A8";
            X1009.Text = "X10A9";
            X100A.Text = "X10AA";
            X100B.Text = "X10AB";
            X100C.Text = "X10AC";
            X100D.Text = "X10AD";
            X100E.Text = "X10AE";
            X100F.Text = "X10AF";
            X1010.Text = "X10B0";
            X101A.Text = "X10BA";
            //GROUP VISION
            X1016.Text = "X10B6";
            X1017.Text = "X10B7";
            X1018.Text = "X10B8";
            X1019.Text = "X10B9";
            X101C.Text = "X10BC";
        }
        public void Change_TabPage_IO()
        {
            tabPage_Input1.Text = "X10A0 - X10BF";
            tabPage_Input2.Text = "X10C0 - X10DF";
            tabPage_Input3.Text = "X1150 - X115F";
            tabPage_Output1.Text = "Y10E0 - X10FF";
            tabPage_Output2.Text = "Y1100 - X111F";
            tabPage_Output3.Text = "Y1120 - X113F";
            string text_ = "Cam 1:" + "\r\n" +
                                         "- POS 1 : D11601 ~D11640 : 1101 ~1140" + "\r\n" +
                                         "- POS 2 : D11641 ~D11680 : 1201 ~1240" + "\r\n" +
                                         "- POS 3 : D11681 ~D11720 : 1301 ~1340" + "\r\n" +
                                         "- POS 4 : D11721 ~D11760 : 1401 ~1440" + "\r\n" +
                                         "\r\n" +
                                         "\r\n" +
                                         "Cam 2:" + "\r\n" +
                                         " -POS 1 : D11801 ~D11840 : 2101 ~2140" + "\r\n" +
                                         "- POS 2 : D11841 ~D11880 : 2201 ~2240" + "\r\n" +
                                         "- POS 3 : D11881 ~D11920 : 2301 ~2340" + "\r\n" +
                                         "- POS 4 : D11921 ~D11960 : 2401 ~2440";
            StatusDisplay.Instance.Update_text2(txt_data_trigger_Bot, text_);
        }
        #endregion
        #region CheckBoxChange-------------------------------------------------------------                        
        private void uiRadioButton_ViewMuti_CheckedChanged(object sender, EventArgs e)
        {
            if (uiRadioButton_ViewMuti.Checked == true)
            {
                PLC1.Write_DataBit_("L" + (18 + Memory_PLC.K100).ToString(), 1);
            }
            else
            {
                PLC1.Write_DataBit_("L" + (18 + Memory_PLC.K100).ToString(), 0);
            }
        }
        private void uiRadioButton_ViewSign_CheckedChanged(object sender, EventArgs e)
        {
            if (uiRadioButton_ViewSign.Checked == true)
            {
                PLC1.Write_DataBit_("L" + (19 + Memory_PLC.K100).ToString(), 1);
            }
            else
            {
                PLC1.Write_DataBit_("L" + (19 + Memory_PLC.K100).ToString(), 0);
            }
        }
        private void uiRadioButton_PowerBlow1_CheckedChanged(object sender, EventArgs e)
        {
            if (uiRadioButton_PowerBlow1.Checked == true)
            {
                PLC1.Write_DataBit_("L" + (21 + Memory_PLC.K100).ToString(), 1);
            }
            else
            {
                PLC1.Write_DataBit_("L" + (21 + Memory_PLC.K100).ToString(), 0);
            }
        }
        private void uiRadioButton_PowerBlow2_CheckedChanged(object sender, EventArgs e)
        {
            if (uiRadioButton_PowerBlow2.Checked == true)
            {
                PLC1.Write_DataBit_("L" + (22 + Memory_PLC.K100).ToString(), 1);
            }
            else
            {
                PLC1.Write_DataBit_("L" + (22 + Memory_PLC.K100).ToString(), 0);
            }
        }
        private void RadioButton_Chieu_X_CheckedChanged(object sender, EventArgs e)
        {
            if (RadioButton_Chieu_X.Checked == true)
            {
                PLC1.Write_DataBit_("L" + (46 + Memory_PLC.K100).ToString(), 1);
            }
            else
            {
                PLC1.Write_DataBit_("L" + (46 + Memory_PLC.K100).ToString(), 0);
            }
        }
        private void RadioButton_Chieu_Y_CheckedChanged(object sender, EventArgs e)
        {
            if (RadioButton_Chieu_Y.Checked == true)
            {
                PLC1.Write_DataBit_("L" + (47 + Memory_PLC.K100).ToString(), 1);
            }
            else
            {
                PLC1.Write_DataBit_("L" + (47 + Memory_PLC.K100).ToString(), 0);
            }
        }
        private void CheckBoxAll_CheckedChanged(object sender, EventArgs e)
        {
            UICheckBox CheckBox_ = sender as UICheckBox;
            if (CheckBox_ == null) return;
            try
            {
                int addr = Convert.ToInt32(CheckBox_.Tag);
                if (CheckBox_.Checked == true)
                {
                    PLC1.Write_DataBit_("L" + (addr + Memory_PLC.K100).ToString(), 1);
                    if (addr == 10)
                    {
                        string[] data = new string[3];
                        DateTime _datetime = DateTime.Now;
                        data[0] = "Sử dụng interlock cửa";
                        data[1] = _datetime.ToString("HH:mm:ss");
                        data[2] = _datetime.ToString("dd:MM:yyyy");
                        write_alarm_database(data);
                    }
                    else if (addr == 3)
                    {
                        string[] data = new string[3];
                        DateTime _datetime = DateTime.Now;
                        data[0] = "Sử dụng sensor an toàn phía sau máy";
                        data[1] = _datetime.ToString("HH:mm:ss");
                        data[2] = _datetime.ToString("dd:MM:yyyy");
                        write_alarm_database(data);
                    }
                }
                else
                {
                    PLC1.Write_DataBit_("L" + (addr + Memory_PLC.K100).ToString(), 0);
                    if (addr == 10)
                    {
                        string[] data = new string[3];
                        DateTime _datetime = DateTime.Now;
                        data[0] = "Không sử dụng interlock cửa";
                        data[1] = _datetime.ToString("dd/MM/yyyy");
                        data[2] = _datetime.ToString("HH:mm:ss");
                        write_alarm_database(data);
                    }
                    else if (addr == 3)
                    {
                        string[] data = new string[3];
                        DateTime _datetime = DateTime.Now;
                        data[0] = "Không sử dụng sensor an toàn phía sau máy";
                        data[1] = _datetime.ToString("dd/MM/yyyy");
                        data[2] = _datetime.ToString("HH:mm:ss");
                        write_alarm_database(data);
                    }
                }
            }
            catch { Message_Box_Error(CheckBox_.Tag.ToString(), "CheckBox"); }
        }
        #endregion
        #region Lamp Switch----------------------------------------------------------------
        private void SwitchLampVision_CLick(object sender, EventArgs e)
        {
            UIButton btn = sender as UIButton;
            if (btn == null) return;
            int type_lamp = Convert.ToInt16(btn.TagString);
            try
            {
                PLC1.Write_Data_Word_("D" + (1501 + Memory_PLC.K1).ToString(), type_lamp);
            }
            catch { Message_Box_Error("Không tìm thấy địa chỉ", "Error"); }
        }
        #endregion
        #region Tact time
        private void Write_TactTime(double[] value)
        {
            string lineSql = string.Format("INSERT OR REPLACE INTO TactTime (STT,VALUE)" + "VALUES (@STT,@VALUE)");
            for (int i = 1; i < 10; i++)
            {
                using (SQLiteCommand cmd = new SQLiteCommand(lineSql, Conn))
                {
                    cmd.Parameters.AddWithValue("STT", i);
                    cmd.Parameters.AddWithValue("VALUE", value[i - 1]);
                    cmd.ExecuteNonQuery();
                }
            }
        }
        #endregion
        #region Setting Camera----------------------------------------------------------------
        private void btn_Show_SettingCamera_Click(object sender, EventArgs e)
        {
            if (Global.Start_Start == 0)
            {
                if (SettingCamera.IsConncetPLCSettingCam == false)
                {
                    SettingCamera.ConnectPLC();
                }
                if (SettingCamera.IsLoadDataBaseSettingCam == false)
                {
                    SettingCamera.LoadData();
                }
                SettingCamera.StartedMonitor = true;
                SettingCamera.Show();
                if (SettingCamera.IsLoadModel == false)
                {
                    SettingCamera.Load_Model();
                }
                if (SettingCamera.StartedMonitor == true)
                {
                    SettingCamera.start_monitor();
                }
            }
            else
            {
                Message_Box_Warring("Machine đang Run...", "Open Setting Camera");
            }
        }
        #endregion
        #region Check Master----------------------------------------------------------
        private void uiSymbolButton15_Click(object sender, EventArgs e)
        {
            FuncRobot.Check_Master = 1;
        }
        private void uiSymbolButton16_Click(object sender, EventArgs e)
        {
            PLC1.Write_DataBit_("M" + (9053 + Memory_PLC.K1000).ToString(), 1);
        }
        private void CheckBox_MasterCheck_CheckedChanged(object sender, EventArgs e)
        {
            if (CheckBox_MasterCheck.Checked == true)
            {
                PLC1.Write_DataBit_("L" + (48 + Memory_PLC.K100).ToString(), 1);
            }
            else
            {
                PLC1.Write_DataBit_("L" + (48 + Memory_PLC.K100).ToString(), 0);
            }
        }
        #endregion
        #region SearchMinMAx-----------------------------------------------------------
        private void SearchMinMaxPlaceTray_Click(object sender, EventArgs e)
        {
            ConnectSQLite();
            Search_Min_Max_CorRB_PlaceTray();
            DisConSQLite();
        }
        private void SearchMinMaxCheckCamTop_Click(object sender, EventArgs e)
        {
            ConnectSQLite();
            Search_Min_Max_CorRB_CheckCamTop();
            DisConSQLite();
        }
        #endregion
        #region SQLServer
        private async Task ResetCounterQty()
        {
            try
            {
                await Task.Delay(10);
                Global.Timer_rsCounter = DateTime.Now;
                DateTime ABCD = DateTime.Now;
                bool IsTime1 = ABCD.Hour == Global.TIME_DAY1_hh && ABCD.Minute == Global.TIME_DAY1_mm && ABCD.Second >= Global.TIME_DAY1_ss && ABCD.Second < Global.TIME_DAY1_ss + 2;
                //bool IsTime1 = Global.Timer_rsCounter.Hour == Global.TIME_DAY1_hh && Global.Timer_rsCounter.Minute == Global.TIME_DAY1_mm && Global.Timer_rsCounter.Second >= Global.TIME_DAY1_ss && Global.Timer_rsCounter.Second < Global.TIME_DAY1_ss + 2;
                bool IsTime2 = Global.Timer_rsCounter.Hour == Global.TIME_NIGHT1_hh && Global.Timer_rsCounter.Minute == Global.TIME_NIGHT1_mm && Global.Timer_rsCounter.Second >= Global.TIME_NIGHT1_ss && Global.Timer_rsCounter.Second < Global.TIME_NIGHT1_ss + 2;

                if (Global.Flag_Run_Time == true)
                {
                    Global.End_Time = Global.Start_Time.AddSeconds(Global.Total_Time_RunTime);
                    Global.Flag_Run_Time = false;
                    List<E_RUN_TIME> _list = new List<E_RUN_TIME>
                   {
                       new E_RUN_TIME
                       {
                           EQUIPMENT_ID=Global.MACHINE_NAME,
                           START_TIME=Global.Start_Time,
                           END_TIME=Global.End_Time,
                           RUN_SECONDS=Convert.ToSingle(Global.Total_Time_RunTime),
                           PRODUCTION_OK=Global.Production_OK,
                           PRODUCTION_NG=Global.Production_NG,
                           PRODUCTION_NA=Global.Production_NA,
                           TOTAL_PRODUCTION=Global.SL_Total_Input,
                           PRODUCTION = Global.Number_Tool,
                           INSERT_DATE=Global.Timer_rsCounter,
                           MODEL_NAME=Global.ModelName_Server,
                           DEL_FLAG="N",
                           INSERT_USER="Ai-Vision",
                       },
                   };
                    await IT_SQL_Server_Helper.Instance.InsertListIgnoreDuplicate("E_RUNTIME", _list);
                    UpdateQtyServerTime.Stop();
                    UpdateQtyServerTime.Restart();
                }
                if ((IsTime1 || IsTime2) && !Global.HasWrittenToday)
                {
                    if (Convert.ToInt32(txt_total_input.Text) > 0)
                    {
                        write_Production_database(txt_total_input.Text, txt_FPCB_OK.Text, txt_FPCB_NG.Text);
                        PLC1.Write_DataBit_("L" + (80 + Memory_PLC.K100).ToString(), 1);
                        Global.HasWrittenToday = true;
                    }
                }
                if (Global.Timer_rsCounter.Minute != Global.TIME_DAY1_mm || Global.Timer_rsCounter.Minute != Global.TIME_NIGHT1_mm)
                {
                    Global.HasWrittenToday = false;
                }
            }
            catch { }
        }
        Stopwatch UpdateQtyServerTime = new Stopwatch();
        private AlarmManager Alarm_Manager = new AlarmManager();
        private SQLiteConnection GetConnectionSQLite()
        {
            return new SQLiteConnection(_connectionString);
        }
        private async Task MES()
        {
            //UpdateQtyServerTime = new Stopwatch();
            //UpdateQtyServerTime.Start();
            while (Global.RUN_MES == 1)
            {
                await ResetCounterQty();
                await AlarmMes();
            }
        }
        Dictionary<string, AlarmInfo> alarmDict = new Dictionary<string, AlarmInfo>();
        public event Action<AlarmInfo, bool, DateTime> OnAlarmChanged;
        private async Task AlarmMes()
        {
            if (Global.RUN_MES == 1)
            {
                await Task.Delay(300);
                bool masterAlarm = PLC1.Read_Data_Bit_("M" + (8199 + Memory_PLC.K200));
                //Nếu không còn alarm tổng → clear hết              
                try
                {
                    if (masterAlarm == true)
                    {
                        int[] check_alarm = PLC1.ReadRandomBit("M" + (8000 + Memory_PLC.K200), 60);
                        List<Action> uiActions = new List<Action>();
                        int inc = 1;
                        for (int i = 0; i < 58; i++)
                        {
                            class_Alarm.cmd_NewAlarm(inc);
                            string alarm_new = class_Alarm.Alarm_ID[inc];

                            // =========================
                            // ALARM ON
                            // =========================
                            if (check_alarm[i] == 1)
                            {
                                if (!alarmDict.ContainsKey(alarm_new))
                                {
                                    DateTime start = DateTime.Now;
                                    Alarm_Manager.AlarmOn(alarm_new);
                                    alarmDict[alarm_new] = new AlarmInfo
                                    {
                                        StartTime = start
                                    };
                                }
                            }
                            // =========================
                            // ALARM OFF
                            // =========================
                            else
                            {
                                if (alarmDict.ContainsKey(alarm_new))
                                {
                                    //var info = alarmDict[alarm_new];
                                    Alarm_Manager.AlarmOff(alarm_new);
                                    alarmDict.Remove(alarm_new);
                                }
                            }

                            inc++;
                        }
                    }
                }
                catch { }
            }
        }
        private async Task UpdateStatusServer()
        {
            Stopwatch UpdateStatusServerTime = new Stopwatch();
            UpdateStatusServerTime.Start();
            while (StatusDisplay.Instance.IsFormOpen == true)
            {
                if (Global.RUN_MES == 1)
                {
                    await Task.Delay(300);
                    try
                    {
                        if (UpdateStatusServerTime.Elapsed.TotalSeconds > Global.TIME_UPDATE)
                        {
                            string MC_STATUS_ = "N/A";
                            if (Global.Status_Machine_Server[1] == 1)
                            {
                                MC_STATUS_ = "Run";
                            }
                            else if (Global.Status_Machine_Server[3] == 1)
                            {
                                MC_STATUS_ = "Stop";
                            }
                            else if (Global.Status_Machine_Server[2] == 1)
                            {
                                MC_STATUS_ = "Pause";
                            }
                            else if (Global.Status_Machine_Server[1] == 0 && Global.Status_Machine_Server[3] == 0)
                            {
                                MC_STATUS_ = "Stop";
                            }
                            DateTime dt = DateTime.Now;
                            List<E_RUN_STATUS> list = new List<E_RUN_STATUS>
                   {
                    new E_RUN_STATUS
                    {
                        EQUIPMENT_ID=Global.MACHINE_NAME,
                        RUNNING=MC_STATUS_,
                        MODEL=Global.ModelName_Server,
                        TOTAL_OK=Convert.ToInt32(txt_FPCB_OK.Text),
                        TOTAL_NG=Convert.ToInt32(txt_FPCB_NG.Text),
                        TOTAL_NA=Global.Total_NA,
                        QTY=Convert.ToInt32(txt_total_input.Text),
                        UPDATE_DATE=dt,
                        UPDATE_USER="Ai-Vision",
                       },
                   };

                            await IT_SQL_Server_Helper.Instance.MergeList("E_RUN_STATUS", list, "EQUIPMENT_ID");
                            UpdateStatusServerTime.Stop();
                            UpdateStatusServerTime.Restart();
                        }
                    }
                    catch { }
                }
            }
        }
        private async Task UpdateCloseServer()
        {

            if (Global.RUN_MES == 1)
            {
                await Task.Delay(5);

                DateTime dt = DateTime.Now;
                List<E_RUN_STATUS> list = new List<E_RUN_STATUS>
                   {
                    new E_RUN_STATUS
                    {
                        EQUIPMENT_ID=Global.MACHINE_NAME,
                        RUNNING="Stop",
                        MODEL=Global.ModelName_Server,
                        TOTAL_OK=Convert.ToInt32(txt_FPCB_OK.Text),
                        TOTAL_NG=Convert.ToInt32(txt_FPCB_NG.Text),
                        TOTAL_NA=Global.Total_NA,
                        QTY=Convert.ToInt32(txt_total_input.Text),
                        UPDATE_DATE=dt,
                        UPDATE_USER="Ai-Vision",
                       },
                   };

                await IT_SQL_Server_Helper.Instance.MergeList("E_RUN_STATUS", list, "EQUIPMENT_ID");
            }
        }
        public void Load_data_machine_server()
        {
            try
            {
                using (var connn = GetConnectionSQLite())
                {
                    connn.Open();
                    string query = string.Format("SELECT * from E_MACHINE");
                    SQLiteDataAdapter adapter = new SQLiteDataAdapter(query, connn);
                    DataSet dataSet = new DataSet();
                    adapter.Fill(dataSet, "E_MACHINE");

                    MACHINE_INFO.Invoke(new MethodInvoker(() =>
                    {
                        MACHINE_INFO.DataSource = dataSet.Tables["E_MACHINE"];
                        MACHINE_INFO.Columns["ID"].Width = 100;
                        MACHINE_INFO.Columns["Name"].Width = 250;
                        MACHINE_INFO.Columns["Data"].Width = 200;

                        MACHINE_INFO.Columns["ID"].ReadOnly = false;
                        MACHINE_INFO.Columns["Name"].ReadOnly = false;
                        MACHINE_INFO.Columns["Data"].ReadOnly = false;
                        MACHINE_INFO.CellFormatting += (s, e) =>
                        {
                            if (e.ColumnIndex == MACHINE_INFO.Columns["ID"].Index) // Xác định cột cần căn chỉnh
                            {
                                e.CellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                            }
                        };
                        MACHINE_INFO.CellFormatting += (s, e) =>
                        {
                            if (e.ColumnIndex == MACHINE_INFO.Columns["Name"].Index) // Xác định cột cần căn chỉnh
                            {
                                e.CellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
                            }
                        };
                        MACHINE_INFO.CellFormatting += (s, e) =>
                        {
                            if (e.ColumnIndex == MACHINE_INFO.Columns["Data"].Index) // Xác định cột cần căn chỉnh
                            {
                                e.CellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
                            }
                        };
                    }));
                }
                //
                List<string>[] dataSQL = new List<string>[50];
                int i = 0;
                foreach (DataGridViewRow row in MACHINE_INFO.Rows) // Duyệt qua từng dòng trong DataGridView
                {
                    dataSQL[i] = new List<string>();
                    // Truy cập vào giá trị của các ô dữ liệu trong dòng
                    string cellValue2 = row.Cells["Data"].Value.ToString();
                    // Thêm dữ liệu vào danh sách
                    dataSQL[i].Add(cellValue2);
                    i++;
                }
                Global.MACHINE_NAME = dataSQL[0][0];
                Global.LINE_NO = dataSQL[1][0];
                Global.TIME_UPDATE = Convert.ToInt32(dataSQL[2][0]);
                Global.RUN_MES = Convert.ToInt32(dataSQL[3][0]);
                Global.TIME_DAY1_hh = Convert.ToInt32(dataSQL[4][0]);
                Global.TIME_DAY1_mm = Convert.ToInt32(dataSQL[5][0]);
                Global.TIME_DAY1_ss = Convert.ToInt32(dataSQL[6][0]);
                Global.TIME_NIGHT1_hh = Convert.ToInt32(dataSQL[7][0]);
                Global.TIME_NIGHT1_mm = Convert.ToInt32(dataSQL[8][0]);
                Global.TIME_NIGHT1_ss = Convert.ToInt32(dataSQL[9][0]);
                Global.Select_cycle_time = Convert.ToInt32(dataSQL[10][0]);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "SQL");
            }
        }
        private void MACHINE_INFO_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                UPDATE_E_MACHINE();
                Load_data_machine_server();
            }
        }
        private void UPDATE_E_MACHINE()
        {
            string[] UPDATE = new string[3];
            if (MACHINE_INFO.SelectedRows.Count > 0)
            {
                DataGridViewRow row = MACHINE_INFO.SelectedRows[0];
                UPDATE[1] = row.Cells[1].Value.ToString();
                string message_ = "Bạn Có muốn lưu vào " + UPDATE[1].ToString() + " không?";
                const string caption_ = "UPDATE E_MACHINE";
                var result = MessageBox.Show(message_, caption_, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    try
                    {
                        if (row.Index != null)
                        {
                            for (int j = 0; j < row.Cells.Count; j++)
                            {
                                UPDATE[j] = row.Cells[j].Value.ToString();
                            }
                            using (var con = GetConnectionSQLite())
                            {
                                con.Open();
                                string saveposSQL = string.Format("INSERT OR REPLACE INTO E_MACHINE (ID,Name,Data) " +
                                    "VALUES (@ID,@Name,@Data)");
                                using (SQLiteCommand cmd = new SQLiteCommand(saveposSQL, con))
                                {
                                    cmd.Parameters.AddWithValue("@ID", UPDATE[0]);
                                    cmd.Parameters.AddWithValue("@Name", UPDATE[1]);
                                    cmd.Parameters.AddWithValue("@Data", UPDATE[2]);

                                    cmd.ExecuteNonQuery();
                                    MessageBox.Show("Lưu thành công", "Save");
                                }
                            }

                        }
                        else
                        {
                            MessageBox.Show("Lưu không thành công", "SQL");
                        }
                    }
                    catch
                    {
                        MessageBox.Show("Lưu không thành công", "SQL");
                    }
                }
            }
        }
        private string GetShift(DateTime time)
        {
            TimeSpan t = time.TimeOfDay;

            if (t >= new TimeSpan(8, 0, 0) && t < new TimeSpan(20, 0, 0))
                return "Day"; // Ca ngày
            else
                return "Night"; // Ca đêm
        }
        private void Search_OK_NG_NA()
        {
            try
            {
                Global.Production_OK = Global.result_Cam_Bot.Count(x => x == 2);
                Global.Production_NG = Global.result_Cam_Bot.Count(x => x == 1);
                Global.Production_NA = Global.result_Cam_Bot.Count(x => x == 3);
                if (Global.Production_OK == 0 && Global.Production_NG == 0 && Global.Production_NA == 0)
                {
                    Global.Production_NA = Global.Number_Tool;
                }
            }
            catch
            {
                Global.Production_OK = 0;
                Global.Production_NG = 0;
                Global.Production_NA = 0;
            }
        }
        #endregion
        #region Model
        private BindingList<string> ListModelName = new BindingList<string>();
        private static List<ParameterModel> _data = new List<ParameterModel>();
        private ParameterModel parameterModel = new ParameterModel();
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
                        bool Check = IsModelExist(NewModelName);
                        if (Check == true) { UIMessageBox.ShowError2("Trùng tên model!", false, 0); }
                        else
                        {
                            using (var conn_ = GetConnectionSQLite())
                            {
                                conn_.Open();
                                string saveposSQL = string.Format("INSERT INTO Model (ID,Name) " + "VALUES (@ID,@Name)");
                                using (SQLiteCommand cmd = new SQLiteCommand(saveposSQL, conn_))
                                {
                                    cmd.Parameters.AddWithValue("@ID", ListModelName.Count);
                                    cmd.Parameters.AddWithValue("@Name", NewModelName);
                                    cmd.ExecuteNonQuery();
                                    ListModelName.Insert(ListModelName.Count - 1, NewModelName);
                                    Combox_Model.SelectedItem = NewModelName;
                                    UIMessageBox.ShowSuccess2("Thêm Model thành công!", true, 0);
                                }
                            }
                        }
                    }
                    catch (Exception ex) { UIMessageBox.ShowError2(ex.ToString(), false, 0); }
            }
        }
        private bool IsModelExist(string modelName)
        {
            using (var conn_ = GetConnectionSQLite())
            {
                conn_.Open();
                using (var cmd = new SQLiteCommand("SELECT 1 FROM Model WHERE LOWER(Name) = LOWER(@name) LIMIT 1", conn_))
                {
                    cmd.Parameters.AddWithValue("@name", modelName.Trim());
                    return cmd.ExecuteScalar() != null;
                }
            }
        }
        public void Load_Name_Model()
        {
            ListModelName.Clear();
            try
            {
                using (var Conn_ = GetConnectionSQLite())
                {
                    Conn_.Open();
                    using (var cmd = new SQLiteCommand("SELECT Name FROM Model ORDER BY Name", Conn_))
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            ListModelName.Add(reader.GetString(0));
                        }
                    }
                }
                ListModelName.Add("➕ Add New Model...");
                Combox_Model.DataSource = ListModelName;
                Combox_Model.DropDownStyle = UIDropDownStyle.DropDownList;
                Combox_Model.SelectedItem = Global.ModelName_Server;
                //LoadModel = true;
            }
            catch (Exception ex)
            {
                //LoadModel = false;
                UIMessageBox.ShowError2(ex.ToString() + "\r\n" + "Error Load Model", false, 0);
            }
        }
        //Import
        private void import_paraMain()
        {
            parameterModel.parameterMain.RowTray = IntValue_para(txt_Row_tray);
            parameterModel.parameterMain.CloumnTray = IntValue_para(txt_Column_tray);
            parameterModel.parameterMain.RowJigInput = IntValue_para(Row_input);
            parameterModel.parameterMain.ColumnJigInput = IntValue_para(Column_input);
            WaitFormHelper.TransferInfo("Import :Data setting Main complete...");
        }
        private void import_paraRobotTeaching()
        {
            List<double>[] points = new List<double>[34];
            int i = 0;
            foreach (DataGridViewRow row in uiDataGridView1.Rows)
            {
                if (row.IsNewRow) continue;
                points[i] = new List<double>();
                double x = Convert.ToDouble(row.Cells["X"].Value);
                double y = Convert.ToDouble(row.Cells["Y"].Value);
                double z = Convert.ToDouble(row.Cells["Z"].Value);
                double a4 = Convert.ToDouble(row.Cells["A4"].Value);
                double a5 = Convert.ToDouble(row.Cells["A5"].Value);
                double c = Convert.ToDouble(row.Cells["C"].Value);

                points[i].Add(x);
                points[i].Add(y);
                points[i].Add(z);
                points[i].Add(a4);
                points[i].Add(a5);
                points[i].Add(c);
                i++;
            }
            parameterModel.parameterRobot_Teaching.point = points;
            WaitFormHelper.TransferInfo("Import :Data Position Robot complete...");
        }
        private void import_paraRobotSetup()
        {
            //OffsetTool
            parameterModel.parameterRobot_Setup.Offset_X = DoubleValue_para(txt_OffsetRB_X);
            //RunAuto
            parameterModel.parameterRobot_Setup.Number_Tool = IntValue_para(txt_number_tool);
            //*Place Tray satefy
            parameterModel.parameterRobot_Setup.X_Upper1 = DoubleValue_para(txt_Limit_X_Place_U);
            parameterModel.parameterRobot_Setup.X_Lower1 = DoubleValue_para(txt_Limit_X_Place_L);
            parameterModel.parameterRobot_Setup.Y_Upper1 = DoubleValue_para(txt_Limit_Y_Place_U);
            parameterModel.parameterRobot_Setup.Y_Lower1 = DoubleValue_para(txt_Limit_Y_Place_L);
            parameterModel.parameterRobot_Setup.Z_Satefy1 = DoubleValue_para(txt_Z_Place_Satefy);
            //*Check camtop satefy
            parameterModel.parameterRobot_Setup.X_Upper2 = DoubleValue_para(txt_Limit_X_Camtop_U);
            parameterModel.parameterRobot_Setup.X_Lower2 = DoubleValue_para(txt_Limit_X_Camtop_L);
            parameterModel.parameterRobot_Setup.Y_Upper2 = DoubleValue_para(txt_Limit_Y_Camtop_U);
            parameterModel.parameterRobot_Setup.Y_Lower2 = DoubleValue_para(txt_Limit_Y_Camtop_L);
            parameterModel.parameterRobot_Setup.Z_Satefy2 = DoubleValue_para(txt_Z_Camtop_Satefy);
            //data sheet FPCB    
            parameterModel.parameterRobot_Setup.NumberFPCB = IntValue_para(txt_number_Fpcb_Block_RB);
            parameterModel.parameterRobot_Setup.OffsetX_FPCB1 = DoubleValue_para(Offset_X_FPCB_Tool_tray);
            parameterModel.parameterRobot_Setup.OffsetY_FPCB1 = DoubleValue_para(Offset_Y_FPCB_Tool_tray);
            parameterModel.parameterRobot_Setup.OffsetX_BlockOrTray = DoubleValue_para(Offset_X_BlockFPCB_Tool_tray);
            parameterModel.parameterRobot_Setup.OffsetY_Tray = DoubleValue_para(Offset_Y_BlockFPCB_Tool_tray);
            //data check Camtop
            parameterModel.parameterRobot_Setup.NumberTool_Checkk = IntValue_para(txt_Total_Check_Marking);
            parameterModel.parameterRobot_Setup.OffsetX_FPCB2 = DoubleValue_para(Offset_X_FPCB_Tool_Marking);
            parameterModel.parameterRobot_Setup.OffsetY_FPCB2 = DoubleValue_para(Offset_Y_FPCB_Tool_Marking);
            //checkbox
            parameterModel.parameterRobot_Setup.MutiBlow = CheckBox_Value(CheckBox_MultiBlow);
            parameterModel.parameterRobot_Setup.SingleBlow = CheckBox_Value(CheckBox_SingleBlow);
            parameterModel.parameterRobot_Setup.MutiView = RadioButton_Value(uiRadioButton_ViewMuti);
            parameterModel.parameterRobot_Setup.SingleView = RadioButton_Value(uiRadioButton_ViewSign);
            parameterModel.parameterRobot_Setup.PowerBlow1 = RadioButton_Value(uiRadioButton_PowerBlow1);
            parameterModel.parameterRobot_Setup.PowerBlow2 = RadioButton_Value(uiRadioButton_PowerBlow2);
            WaitFormHelper.TransferInfo("Import :Data Robot Setup complete...");
        }
        private void import_paraRobotSetupTray()
        {
            parameterModel.parameterRobot_SetupTray.Distance_Row = DoubleValue_para(txt_distance_row);
            parameterModel.parameterRobot_SetupTray.Distance_Column = DoubleValue_para(txt_distance_column);
            parameterModel.parameterRobot_SetupTray.Chieu_thaX = RadioButton_Value(RadioButton_Chieu_X);
            parameterModel.parameterRobot_SetupTray.Chieu_thaY = RadioButton_Value(RadioButton_Chieu_Y);
            parameterModel.parameterRobot_SetupTray.RowTray1 = IntValue_para(txt_Row1_x10);
            parameterModel.parameterRobot_SetupTray.ColumnTray1 = IntValue_para(txt_Column1_x10);
            parameterModel.parameterRobot_SetupTray.RowTray2 = IntValue_para(txt_Row2_x10);
            parameterModel.parameterRobot_SetupTray.ColumnTray2 = IntValue_para(txt_Column2_x10);
            parameterModel.parameterRobot_SetupTray.Checkbox_Array1 = CheckBox_Value(CheckBox_Use_Matrix1);
            parameterModel.parameterRobot_SetupTray.Checkbox_Array2 = CheckBox_Value(CheckBox_Use_Matrix2);
            WaitFormHelper.TransferInfo("Import :Data Robot Setup Tray complete...");
        }
        private void import_paraPLC_Loading()
        {
            parameterModel.parameterPLC_Loading.Cor_Input = IntValue_paraCor(CO16735);
            parameterModel.parameterPLC_Loading.Cor_Output = IntValue_paraCor(CO16737);
            WaitFormHelper.TransferInfo("Import :Data position Group Loading complete...");
        }
        private void import_paraPLC_Vision()
        {
            //X
            parameterModel.parameterPLC_Vision.CorX_Pick1 = IntValue_paraCor(COR5337);
            parameterModel.parameterPLC_Vision.CorX_Pick2 = IntValue_paraCor(COR5368);
            parameterModel.parameterPLC_Vision.CorX_Check1 = IntValue_paraCor(COR5341);
            parameterModel.parameterPLC_Vision.CorX_Check2 = IntValue_paraCor(COR5343);
            parameterModel.parameterPLC_Vision.CorX_Check3 = IntValue_paraCor(COR5345);
            parameterModel.parameterPLC_Vision.CorX_Check4 = IntValue_paraCor(COR5347);
            parameterModel.parameterPLC_Vision.CorX_Output = IntValue_paraCor(COR5353);
            //Y
            parameterModel.parameterPLC_Vision.CorY_Pick1 = IntValue_paraCor(COR5437);
            parameterModel.parameterPLC_Vision.CorY_Pick2 = IntValue_paraCor(COR5468);
            parameterModel.parameterPLC_Vision.CorY_Check1 = IntValue_paraCor(COR5441);
            parameterModel.parameterPLC_Vision.CorY_Check2 = IntValue_paraCor(COR5443);
            parameterModel.parameterPLC_Vision.CorY_Check3 = IntValue_paraCor(COR5445);
            parameterModel.parameterPLC_Vision.CorY_Check4 = IntValue_paraCor(COR5447);
            parameterModel.parameterPLC_Vision.CorY_Output = IntValue_paraCor(COR5453);
            //Z
            parameterModel.parameterPLC_Vision.CorZ_Pick1 = IntValue_paraCor(COR5537);
            parameterModel.parameterPLC_Vision.CorZ_Pick2 = IntValue_paraCor(COR5568);
            parameterModel.parameterPLC_Vision.CorZ_Check1 = IntValue_paraCor(COR5541);
            parameterModel.parameterPLC_Vision.CorZ_Check2 = IntValue_paraCor(COR5543);
            parameterModel.parameterPLC_Vision.CorZ_Check3 = IntValue_paraCor(COR5545);
            parameterModel.parameterPLC_Vision.CorZ_Check4 = IntValue_paraCor(COR5547);
            parameterModel.parameterPLC_Vision.CorZ_Output = IntValue_paraCor(COR5553);
            //R
            parameterModel.parameterPLC_Vision.CorR_Pick1 = IntValue_paraCor(COR5637);
            parameterModel.parameterPLC_Vision.CorR_Pick2 = IntValue_paraCor(COR5668);
            parameterModel.parameterPLC_Vision.CorR_Check1 = IntValue_paraCor(COR5641);
            parameterModel.parameterPLC_Vision.CorR_Output = IntValue_paraCor(COR5653);
            //*CheckBox
            parameterModel.parameterPLC_Vision.Not_Check1 = CheckBox_Value(Checkbox_Connector);
            parameterModel.parameterPLC_Vision.Not_Check2 = CheckBox_Value(Checkbox_FPCB);
            parameterModel.parameterPLC_Vision.Not_Check3 = CheckBox_Value(Checkbox_BienDang);
            parameterModel.parameterPLC_Vision.Not_Check4 = CheckBox_Value(Checkbox_check4);
            parameterModel.parameterPLC_Vision.Data1_1 = CheckBox_Value(CheckBox_DataVisionBotMode11);
            parameterModel.parameterPLC_Vision.Data2_1 = CheckBox_Value(CheckBox_DataVisionBotMode21);
            parameterModel.parameterPLC_Vision.NotUseLamp = CheckBox_Value(CheckBox_use_transfer_lampVision);
            parameterModel.parameterPLC_Vision.NotUseVacuum = CheckBox_Value(CheckBox_NotUseVaccumVisionBot);
            //Parameter Vision
            parameterModel.parameterPLC_Vision.Number_Block = IntValue_para(txt_number_block_FPCB);
            parameterModel.parameterPLC_Vision.OffsetX_block = DoubleValue_para(txt_Offset_X_Block);
            parameterModel.parameterPLC_Vision.OffsetY_block = DoubleValue_para(txt_Offset_Y_Block);
            parameterModel.parameterPLC_Vision.Number_FPCB = IntValue_para(txt_number_FPCB_Block);
            parameterModel.parameterPLC_Vision.OffsetX_FPCB = DoubleValue_para(txt_Offset_X_FPCB);
            parameterModel.parameterPLC_Vision.OffsetY_FPCB = DoubleValue_para(txt_Offset_Y_FPCB);
            parameterModel.parameterPLC_Vision.Total_Check = IntValue_para(txt_Total_Check);
            parameterModel.parameterPLC_Vision.Number_Data = IntValue_para(txt_number_data);
            WaitFormHelper.TransferInfo("Import :Data position Group Vision complete...");
        }
        //Export
        private void Export_paraMain()
        {
            StatusDisplay.Instance.Update_text2(txt_Row_tray, parameterModel.parameterMain.RowTray.ToString());
            StatusDisplay.Instance.Update_text2(txt_Column_tray, parameterModel.parameterMain.CloumnTray.ToString());
            StatusDisplay.Instance.Update_text2(Row_input, parameterModel.parameterMain.RowJigInput.ToString());
            StatusDisplay.Instance.Update_text2(Column_input, parameterModel.parameterMain.ColumnJigInput.ToString());
        }
        private void Export_paraRobotTeaching()
        {
            List<double>[] Position = parameterModel.parameterRobot_Teaching.point;
            if (Position == null) return;
            foreach (DataGridViewRow row in uiDataGridView1.Rows)
            {
                if (row.IsNewRow) continue;

                int i = row.Index;

                if (i < Position.Length)
                {
                    row.Cells["X"].Value = Position[i][0];
                    row.Cells["Y"].Value = Position[i][1];
                    row.Cells["Z"].Value = Position[i][2];
                    row.Cells["A4"].Value = Position[i][3];
                    row.Cells["A5"].Value = Position[i][4];
                    row.Cells["C"].Value = Position[i][5];
                }
            }
            Global.Pick_Press = Position[0].ToArray();//1
            Global.Z_Pick_Press = Position[1].ToArray();//2
            Global.Check_Marking_Start = Position[2].ToArray();//3
            Global.Check_Tape_Start = Position[3].ToArray();//4
            Global.Place_Tray_1 = Position[4].ToArray();//5
            Global.Place_Tray_2 = Position[5].ToArray();//6
            Global.Place_Tray_3 = Position[6].ToArray();//7
            Global.Place_Tray_4 = Position[7].ToArray();//8
            Global.Input_FPCB = Position[8].ToArray();//9
            Global.Z_Input_FPCB = Position[9].ToArray();//10
            Global.Pick_FPCB_Output_1 = Position[10].ToArray();//11
            Global.Pick_FPCB_Output_2 = Position[11].ToArray();//12
            Global.Z_Pick_FPCB_Output_1 = Position[12].ToArray();//13
            Global.Z_Pick_FPCB_Output_2 = Position[13].ToArray();//13
            Global.Pos_NG = Position[14].ToArray();//14
            Global.Z_Pos_NG = Position[15].ToArray();//15
            Global.Ready_Pick_Press_1 = Position[16].ToArray();//16
            Global.Ready_Pick_Press_2 = Position[17].ToArray();//17
            Global.Ready_Check_Camtop_1 = Position[18].ToArray();//18
            Global.Ready_Check_Camtop_2 = Position[19].ToArray();//19
            Global.Ready_Inputput_1 = Position[20].ToArray();//20
            Global.Ready_Inputput_2 = Position[21].ToArray();//21
            Global.Ready_Pick_Output_1 = Position[22].ToArray();//22
            Global.Ready_Pick_Output_2 = Position[23].ToArray();//23
                                                                //
            Global.Ready_Place_Tray_1 = Position[24].ToArray();//24
            Global.Ready_Place_Tray_2 = Position[25].ToArray();//25
            Global.Ready_Place_Tray_3 = Position[26].ToArray();//26
                                                               //                                           
            Global.Ready_Place_NG_1 = Position[27].ToArray();//27
            Global.Ready_Place_NG_2 = Position[28].ToArray();//28
                                                             //
            Global.Ready_Arc_1 = Position[29].ToArray();//29
            Global.Ready_Arc_2 = Position[30].ToArray();//30
                                                        //
            Global.Ready_Rotation_1 = Position[31].ToArray();//31
            Global.Ready_Rotation_2 = Position[32].ToArray();//32
            Global.Homee = Position[33].ToArray();//33
        }
        private void Export_paraRobotSetup()
        {
            //OffsetTool
            StatusDisplay.Instance.Update_text2(txt_OffsetRB_X, parameterModel.parameterRobot_Setup.Offset_X.ToString());
            //RunAuto
            StatusDisplay.Instance.Update_text2(txt_number_tool, parameterModel.parameterRobot_Setup.Number_Tool.ToString());
            //*Place Tray satefy
            StatusDisplay.Instance.Update_text2(txt_Limit_X_Place_U, parameterModel.parameterRobot_Setup.X_Upper1.ToString());
            StatusDisplay.Instance.Update_text2(txt_Limit_X_Place_L, parameterModel.parameterRobot_Setup.X_Lower1.ToString());
            StatusDisplay.Instance.Update_text2(txt_Limit_Y_Place_U, parameterModel.parameterRobot_Setup.Y_Upper1.ToString());
            StatusDisplay.Instance.Update_text2(txt_Limit_Y_Place_L, parameterModel.parameterRobot_Setup.Y_Lower1.ToString());
            StatusDisplay.Instance.Update_text2(txt_Z_Place_Satefy, parameterModel.parameterRobot_Setup.Z_Satefy1.ToString());
            //*Check camtop satefy
            StatusDisplay.Instance.Update_text2(txt_Limit_X_Camtop_U, parameterModel.parameterRobot_Setup.X_Upper2.ToString());
            StatusDisplay.Instance.Update_text2(txt_Limit_X_Camtop_L, parameterModel.parameterRobot_Setup.X_Lower2.ToString());
            StatusDisplay.Instance.Update_text2(txt_Limit_Y_Camtop_U, parameterModel.parameterRobot_Setup.Y_Upper2.ToString());
            StatusDisplay.Instance.Update_text2(txt_Limit_Y_Camtop_L, parameterModel.parameterRobot_Setup.Y_Lower2.ToString());
            StatusDisplay.Instance.Update_text2(txt_Z_Camtop_Satefy, parameterModel.parameterRobot_Setup.Z_Satefy2.ToString());
            //data sheet FPCB    
            StatusDisplay.Instance.Update_text2(txt_number_Fpcb_Block_RB, parameterModel.parameterRobot_Setup.NumberFPCB.ToString());
            StatusDisplay.Instance.Update_text2(Offset_X_FPCB_Tool_tray, parameterModel.parameterRobot_Setup.OffsetX_FPCB1.ToString());
            StatusDisplay.Instance.Update_text2(Offset_Y_FPCB_Tool_tray, parameterModel.parameterRobot_Setup.OffsetY_FPCB1.ToString());
            StatusDisplay.Instance.Update_text2(Offset_X_BlockFPCB_Tool_tray, parameterModel.parameterRobot_Setup.OffsetX_BlockOrTray.ToString());
            StatusDisplay.Instance.Update_text2(Offset_Y_BlockFPCB_Tool_tray, parameterModel.parameterRobot_Setup.OffsetY_Tray.ToString());
            //data check Camtop
            StatusDisplay.Instance.Update_text2(txt_Total_Check_Marking, parameterModel.parameterRobot_Setup.NumberTool_Checkk.ToString());
            StatusDisplay.Instance.Update_text2(Offset_X_FPCB_Tool_Marking, parameterModel.parameterRobot_Setup.OffsetX_FPCB2.ToString());
            StatusDisplay.Instance.Update_text2(Offset_Y_FPCB_Tool_Marking, parameterModel.parameterRobot_Setup.OffsetY_FPCB2.ToString());
            //checkbox
            CheckBox_MultiBlow.Checked = BoolValue(parameterModel.parameterRobot_Setup.MutiBlow);
            CheckBox_SingleBlow.Checked = BoolValue(parameterModel.parameterRobot_Setup.SingleBlow);
            uiRadioButton_ViewMuti.Checked = BoolValue(parameterModel.parameterRobot_Setup.MutiView);
            uiRadioButton_ViewSign.Checked = BoolValue(parameterModel.parameterRobot_Setup.SingleView);
            uiRadioButton_PowerBlow2.Checked = BoolValue(parameterModel.parameterRobot_Setup.PowerBlow2);
            uiRadioButton_PowerBlow1.Checked = BoolValue(parameterModel.parameterRobot_Setup.PowerBlow1);
        }
        private void Export_paraRobotSetupTray()
        {
            StatusDisplay.Instance.Update_text2(txt_distance_row, parameterModel.parameterRobot_SetupTray.Distance_Row.ToString());
            StatusDisplay.Instance.Update_text2(txt_distance_column, parameterModel.parameterRobot_SetupTray.Distance_Column.ToString());
            RadioButton_Chieu_X.Checked = BoolValue(parameterModel.parameterRobot_SetupTray.Chieu_thaX);
            RadioButton_Chieu_Y.Checked = BoolValue(parameterModel.parameterRobot_SetupTray.Chieu_thaY);
            StatusDisplay.Instance.Update_text2(txt_Row1_x10, parameterModel.parameterRobot_SetupTray.RowTray1.ToString());
            StatusDisplay.Instance.Update_text2(txt_Column1_x10, parameterModel.parameterRobot_SetupTray.ColumnTray1.ToString());
            StatusDisplay.Instance.Update_text2(txt_Row2_x10, parameterModel.parameterRobot_SetupTray.RowTray2.ToString());
            StatusDisplay.Instance.Update_text2(txt_Column2_x10, parameterModel.parameterRobot_SetupTray.ColumnTray2.ToString());
            CheckBox_Use_Matrix1.Checked = BoolValue(parameterModel.parameterRobot_SetupTray.Checkbox_Array1);
            CheckBox_Use_Matrix2.Checked = BoolValue(parameterModel.parameterRobot_SetupTray.Checkbox_Array2);
        }
        private void Export_paraPLC_Loading()
        {
            StatusDisplay.Instance.Update_text2(CO16735, StringValue_paraCor(parameterModel.parameterPLC_Loading.Cor_Input));
            StatusDisplay.Instance.Update_text2(CO16737, StringValue_paraCor(parameterModel.parameterPLC_Loading.Cor_Output));
        }
        private void Export_paraPLC_Vision()
        {
            //X
            StatusDisplay.Instance.Update_text2(COR5337, StringValue_paraCor(parameterModel.parameterPLC_Vision.CorX_Pick1));
            StatusDisplay.Instance.Update_text2(COR5368, StringValue_paraCor(parameterModel.parameterPLC_Vision.CorX_Pick2));
            StatusDisplay.Instance.Update_text2(COR5341, StringValue_paraCor(parameterModel.parameterPLC_Vision.CorX_Check1));
            StatusDisplay.Instance.Update_text2(COR5343, StringValue_paraCor(parameterModel.parameterPLC_Vision.CorX_Check2));
            StatusDisplay.Instance.Update_text2(COR5345, StringValue_paraCor(parameterModel.parameterPLC_Vision.CorX_Check3));
            StatusDisplay.Instance.Update_text2(COR5347, StringValue_paraCor(parameterModel.parameterPLC_Vision.CorX_Check4));
            StatusDisplay.Instance.Update_text2(COR5353, StringValue_paraCor(parameterModel.parameterPLC_Vision.CorX_Output));
            //Y
            StatusDisplay.Instance.Update_text2(COR5437, StringValue_paraCor(parameterModel.parameterPLC_Vision.CorY_Pick1));
            StatusDisplay.Instance.Update_text2(COR5468, StringValue_paraCor(parameterModel.parameterPLC_Vision.CorY_Pick2));
            StatusDisplay.Instance.Update_text2(COR5441, StringValue_paraCor(parameterModel.parameterPLC_Vision.CorY_Check1));
            StatusDisplay.Instance.Update_text2(COR5443, StringValue_paraCor(parameterModel.parameterPLC_Vision.CorY_Check2));
            StatusDisplay.Instance.Update_text2(COR5445, StringValue_paraCor(parameterModel.parameterPLC_Vision.CorY_Check3));
            StatusDisplay.Instance.Update_text2(COR5447, StringValue_paraCor(parameterModel.parameterPLC_Vision.CorY_Check4));
            StatusDisplay.Instance.Update_text2(COR5453, StringValue_paraCor(parameterModel.parameterPLC_Vision.CorY_Output));
            //Z
            StatusDisplay.Instance.Update_text2(COR5537, StringValue_paraCor(parameterModel.parameterPLC_Vision.CorZ_Pick1));
            StatusDisplay.Instance.Update_text2(COR5568, StringValue_paraCor(parameterModel.parameterPLC_Vision.CorZ_Pick2));
            StatusDisplay.Instance.Update_text2(COR5541, StringValue_paraCor(parameterModel.parameterPLC_Vision.CorZ_Check1));
            StatusDisplay.Instance.Update_text2(COR5543, StringValue_paraCor(parameterModel.parameterPLC_Vision.CorZ_Check2));
            StatusDisplay.Instance.Update_text2(COR5545, StringValue_paraCor(parameterModel.parameterPLC_Vision.CorZ_Check3));
            StatusDisplay.Instance.Update_text2(COR5547, StringValue_paraCor(parameterModel.parameterPLC_Vision.CorZ_Check4));
            StatusDisplay.Instance.Update_text2(COR5553, StringValue_paraCor(parameterModel.parameterPLC_Vision.CorZ_Output));
            //R
            StatusDisplay.Instance.Update_text2(COR5637, StringValue_paraCor(parameterModel.parameterPLC_Vision.CorR_Pick1));
            StatusDisplay.Instance.Update_text2(COR5668, StringValue_paraCor(parameterModel.parameterPLC_Vision.CorR_Pick2));
            StatusDisplay.Instance.Update_text2(COR5641, StringValue_paraCor(parameterModel.parameterPLC_Vision.CorR_Check1));
            StatusDisplay.Instance.Update_text2(COR5653, StringValue_paraCor(parameterModel.parameterPLC_Vision.CorR_Output));
            //*CheckBox
            Checkbox_Connector.Checked = BoolValue(parameterModel.parameterPLC_Vision.Not_Check1);
            Checkbox_FPCB.Checked = BoolValue(parameterModel.parameterPLC_Vision.Not_Check2);
            Checkbox_BienDang.Checked = BoolValue(parameterModel.parameterPLC_Vision.Not_Check3);
            Checkbox_check4.Checked = BoolValue(parameterModel.parameterPLC_Vision.Not_Check4);
            CheckBox_DataVisionBotMode11.Checked = BoolValue(parameterModel.parameterPLC_Vision.Data1_1);
            CheckBox_DataVisionBotMode21.Checked = BoolValue(parameterModel.parameterPLC_Vision.Data2_1);
            CheckBox_use_transfer_lampVision.Checked = BoolValue(parameterModel.parameterPLC_Vision.NotUseLamp);
            CheckBox_NotUseVaccumVisionBot.Checked = BoolValue(parameterModel.parameterPLC_Vision.NotUseVacuum);
            //Parameter Vision
            StatusDisplay.Instance.Update_text2(txt_number_block_FPCB, parameterModel.parameterPLC_Vision.Number_Block.ToString());
            StatusDisplay.Instance.Update_text2(txt_Offset_X_Block, parameterModel.parameterPLC_Vision.OffsetX_block.ToString());
            StatusDisplay.Instance.Update_text2(txt_Offset_Y_Block, parameterModel.parameterPLC_Vision.OffsetY_block.ToString());
            StatusDisplay.Instance.Update_text2(txt_number_FPCB_Block, parameterModel.parameterPLC_Vision.Number_FPCB.ToString());
            StatusDisplay.Instance.Update_text2(txt_Offset_X_FPCB, parameterModel.parameterPLC_Vision.OffsetX_FPCB.ToString());
            StatusDisplay.Instance.Update_text2(txt_Offset_Y_FPCB, parameterModel.parameterPLC_Vision.OffsetY_FPCB.ToString());
            StatusDisplay.Instance.Update_text2(txt_Total_Check, parameterModel.parameterPLC_Vision.Total_Check.ToString());
            StatusDisplay.Instance.Update_text2(txt_number_data, parameterModel.parameterPLC_Vision.Number_Data.ToString());
        }

        //helper convert
        private int IntValue_para(UITextBox txt)
        {
            if (txt.Text == "" || txt.Text == null) return 0;
            int value = Convert.ToInt32(txt.Text);
            return value;

        }
        private double DoubleValue_para(UITextBox txt)
        {
            if (txt.Text == "" || txt.Text == null) return 0;
            double value = Convert.ToDouble(txt.Text);
            return value;
        }
        private int CheckBox_Value(UICheckBox checkbox)
        {
            int value = -1;
            if (checkbox.Checked == true)
            {
                value = 1;
            }
            else { value = 0; }
            return value;
        }
        private int RadioButton_Value(UIRadioButton radiobutton)
        {
            int value = -1;
            if (radiobutton.Checked == true)
            {
                value = 1;
            }
            else { value = 0; }
            return value;
        }
        private int IntValue_paraCor(UITextBox txt)
        {
            if (txt.Text == "" || txt.Text == null) return 0;
            int value = Convert.ToInt32(Convert.ToDouble(txt.Text) * 1000);
            return value;

        }
        private string StringValue_paraCor(int value)
        {
            string data = (Convert.ToDouble(value) / 1000).ToString();
            return data;

        }
        private bool BoolValue(int value)
        {
            bool check = value == 1 ? true : false;
            return check;
        }
        //Transfer data > database + MomoryPLC
        private void TransferMain()
        {
            try
            {
                PLC1.Write_Data_Word_("D" + (3000 + Memory_PLC.K1000).ToString(), Convert.ToInt16(txt_number_tray_curr.Text));
                PLC1.Write_Data_Word_("D" + (3002 + Memory_PLC.K1000).ToString(), Convert.ToInt16(txt_Row_tray.Text));
                PLC1.Write_Data_Word_("D" + (3003 + Memory_PLC.K1000).ToString(), Convert.ToInt16(txt_Column_tray.Text));
                PLC1.Write_Data_Word_("D" + (3014 + Memory_PLC.K1000).ToString(), Convert.ToInt16(Row_input.Text));
                PLC1.Write_Data_Word_("D" + (3015 + Memory_PLC.K1000).ToString(), Convert.ToInt16(Column_input.Text));
                Global.Row_tray = Convert.ToInt16(txt_Row_tray.Text);
                Global.Column_tray = Convert.ToInt16(txt_Column_tray.Text);
                Global.Number_Tray_output = Convert.ToInt16(txt_number_tray_curr.Text);
                WaitFormHelper.TransferInfo("Transfer :Data Main complete...");
            }
            catch { }
        }
        private void TransferRobotTeaching()
        {
            using (var conn_ = GetConnectionSQLite())
            {
                conn_.Open();
                foreach (DataGridViewRow row in uiDataGridView1.Rows)
                {
                    if (row.IsNewRow)
                        continue;
                    string saveposSQL = string.Format("INSERT OR REPLACE INTO Data_Pos_RB (STT, POSITION, X,Y,Z,A4,A5,C) VALUES (@STT, @POSITION, @X,@Y,@Z,@A4,@A5,@C)");
                    using (SQLiteCommand cmd = new SQLiteCommand(saveposSQL, conn_))
                    {
                        cmd.Parameters.AddWithValue("@STT", row.Cells[0].Value);
                        cmd.Parameters.AddWithValue("@POSITION", row.Cells[1].Value);
                        cmd.Parameters.AddWithValue("@X", Convert.ToDouble(row.Cells[2].Value));
                        cmd.Parameters.AddWithValue("@Y", Convert.ToDouble(row.Cells[3].Value));
                        cmd.Parameters.AddWithValue("@Z", Convert.ToDouble(row.Cells[4].Value));
                        cmd.Parameters.AddWithValue("@A4", 0.00);
                        cmd.Parameters.AddWithValue("@A5", 0.00);
                        cmd.Parameters.AddWithValue("@C", Convert.ToDouble(row.Cells[7].Value));
                        cmd.ExecuteNonQuery();
                    }
                }
                WaitFormHelper.TransferInfo("Transfer :Position Robot complete...");
            }
        }
        private void TransferRobotSetup()
        {
            Sub_GreatValue_SetupRobot();
            Sub_TransferValueRunAuto();
            Sub_GreatValue_CheckMarking();
        }
        private void TransferRobotSetupTray()
        {
            Sub_GreatValue_PlaceTray();
        }
        private void TransferPLCLoading()
        {
            Sub_TransferValueLoading();

        }
        private void TransferPLCVision()
        {
            Sub_TransferValueVision();
            Sub_TransferParameterVision();
        }
        //Sub transfer data robot
        private void Sub_TransferValueRunAuto()
        {
            string[] data_RB = { txt_sp_auto_RB.Text, txt_delayhut.Text, txt_delaytha.Text, txt_delay_up_cylinder.Text, txt_delay_down_cylinder.Text,
                txt_number_tool.Text, txt_mode_run.Text, txt_ACC_RB.Text, txt_SP_Wait_Pick.Text,txt_trigger_camB.Text,txt_speed_snap.Text };
            using (var conn_ = GetConnectionSQLite())
            {
                conn_.Open();
                string saveposSQL = string.Format("INSERT OR REPLACE INTO Thong_so_RB (STT,SpeedAuto,Delay1,Delay2,Delay3,Delay4,Number_check,Mode_run_RB,ACC_RB,SP_Wait_Pick,Delay_Trigger_CamB, Snap) " +
                "VALUES (@STT,@SpeedAuto,@Delay1,@Delay2,@Delay3,@Delay4,@Number_check,@Mode_run_RB,@ACC_RB,@SP_Wait_Pick,@Delay_Trigger_CamB, @Snap)");
                using (SQLiteCommand cmd = new SQLiteCommand(saveposSQL, conn_))
                {
                    cmd.Parameters.AddWithValue("@STT", 1);
                    cmd.Parameters.AddWithValue("@SpeedAuto", Convert.ToInt16(data_RB[0]));
                    cmd.Parameters.AddWithValue("@Delay1", Convert.ToInt16(data_RB[1]));
                    cmd.Parameters.AddWithValue("@Delay2", Convert.ToInt16(data_RB[2]));
                    cmd.Parameters.AddWithValue("@Delay3", Convert.ToInt16(data_RB[3]));
                    cmd.Parameters.AddWithValue("@Delay4", Convert.ToInt16(data_RB[4]));
                    cmd.Parameters.AddWithValue("@Number_check", Convert.ToInt16(data_RB[5]));
                    cmd.Parameters.AddWithValue("@Mode_run_RB", Convert.ToInt16(data_RB[6]));
                    cmd.Parameters.AddWithValue("@ACC_RB", Convert.ToInt16(data_RB[7]));
                    cmd.Parameters.AddWithValue("@SP_Wait_Pick", Convert.ToInt16(data_RB[8]));
                    cmd.Parameters.AddWithValue("@Delay_Trigger_CamB", Convert.ToInt16(data_RB[9]));
                    cmd.Parameters.AddWithValue("@Snap", Convert.ToInt16(data_RB[10]));
                    cmd.ExecuteNonQuery();
                    WaitFormHelper.TransferInfo("Transfer :Data Run Auto complete...");
                }
            }
            Global.SP_auto_RB = Convert.ToInt16(txt_sp_auto_RB.Text);
            Global.delay_hut = Convert.ToInt16(txt_delayhut.Text);
            Global.delay_tha = Convert.ToInt16(txt_delaytha.Text);
            Global.delay_up = Convert.ToInt16(txt_delay_up_cylinder.Text);
            Global.delay_down = Convert.ToInt16(txt_delay_down_cylinder.Text);
            Global.Number_Tool = Convert.ToInt16(txt_number_tool.Text);// D9100
            Global.Mode_run_auto = Convert.ToInt16(txt_mode_run.Text);
            Global.ACC_RB = Convert.ToInt16(txt_ACC_RB.Text);
            Global.SP_Wait_Pick = Convert.ToInt16(txt_SP_Wait_Pick.Text);
            Global.TT_CamB = Convert.ToInt16(txt_trigger_camB.Text);
            Global.SP_snap = Convert.ToInt16(txt_speed_snap.Text);
        }
        private void Sub_GreatValue_SetupRobot()
        {
            string[] data_Pos = { txt_OffsetRB_X.Text,txt_Limit_Y_Place_U.Text, txt_offsetRB_Z.Text, txt_Z_antoan.Text, txt_A2_antoan.Text, txt_Speed_Home_RB.Text, txt_Z_Place_Satefy.Text, txt_Limit_Y_Place_L.Text, txt_Limit_X_Place_U.Text, txt_Limit_X_Place_L.Text,
            txt_number_Block_Fpcb_RB.Text, Offset_X_BlockFPCB_Tool_tray.Text, Offset_Y_BlockFPCB_Tool_tray.Text, txt_number_Fpcb_Block_RB.Text, Offset_X_FPCB_Tool_tray.Text, Offset_Y_FPCB_Tool_tray.Text,
            txt_Limit_X_Camtop_U.Text,txt_Limit_X_Camtop_L.Text,txt_Limit_Y_Camtop_U.Text,txt_Limit_Y_Camtop_L.Text,txt_Z_Camtop_Satefy.Text,
           txt_Total_Check_Marking.Text,Offset_X_BlockFPCB_Tool_Marking.Text,Offset_Y_BlockFPCB_Tool_Marking.Text,Offset_X_FPCB_Tool_Marking.Text, Offset_Y_FPCB_Tool_Marking.Text};

            Global.Offset_X = Convert.ToDouble(txt_OffsetRB_X.Text);
            Global.Y_Place_Satefy_U = Convert.ToDouble(txt_Limit_Y_Place_U.Text);
            Global.Offset_Z = Convert.ToDouble(txt_offsetRB_Z.Text);
            Global.Z_antoan = Convert.ToDouble(txt_Z_antoan.Text);
            Global.A2_antoan = Convert.ToDouble(txt_A2_antoan.Text);
            Global.SP_Home = Convert.ToDouble(txt_Speed_Home_RB.Text);
            //
            Global.Z_Place_Satefy = Convert.ToDouble(txt_Z_Place_Satefy.Text);
            Global.Y_Place_Satefy_L = Convert.ToDouble(txt_Limit_Y_Place_L.Text);
            Global.X_Place_Satefy_U = Convert.ToDouble(txt_Limit_X_Place_U.Text);
            Global.X_Place_Satefy_L = Convert.ToDouble(txt_Limit_X_Place_L.Text);
            //
            Global.X_Camtop_Satefy_U = Convert.ToDouble(txt_Limit_X_Camtop_U.Text);
            Global.X_Camtop_Satefy_L = Convert.ToDouble(txt_Limit_X_Camtop_L.Text);
            Global.Y_Camtop_Satefy_U = Convert.ToDouble(txt_Limit_Y_Camtop_U.Text);
            Global.Y_Camtop_Satefy_L = Convert.ToDouble(txt_Limit_Y_Camtop_L.Text);
            Global.Z_Camtop_Satefy = Convert.ToDouble(txt_Z_Camtop_Satefy.Text);
            //
            Global.Number_Block = Convert.ToDouble(txt_number_Block_Fpcb_RB.Text);
            Global.Offset_Block_X = Convert.ToDouble(Offset_X_BlockFPCB_Tool_tray.Text);
            Global.Offset_Block_Y = Convert.ToDouble(Offset_Y_BlockFPCB_Tool_tray.Text);
            Global.Number_FPCB = Convert.ToDouble(txt_number_Fpcb_Block_RB.Text);
            Global.Offset_FPCB_X = Convert.ToDouble(Offset_X_FPCB_Tool_tray.Text);
            Global.Offset_FPCB_Y = Convert.ToDouble(Offset_Y_FPCB_Tool_tray.Text);
            //
            Global.Offset_Block_X_marking = Convert.ToDouble(Offset_X_BlockFPCB_Tool_Marking.Text);
            Global.Offset_Block_Y_Marking = Convert.ToDouble(Offset_Y_BlockFPCB_Tool_Marking.Text);
            Global.Offset_FPCB_X_Marking = Convert.ToDouble(Offset_X_FPCB_Tool_Marking.Text);
            Global.Offset_FPCB_Y_Marking = Convert.ToDouble(Offset_Y_FPCB_Tool_Marking.Text);
            Global.Total_Check_Marking = Convert.ToDouble(txt_Total_Check_Marking.Text);
            //

            string saveposSQL = string.Format("INSERT OR REPLACE INTO OffsetRB (STT,X,Y,Z,Z_antoan,A2_antoan,SP_Home,Z_Place_Satefy,Y_Place_Satefy_L,X_Place_Satefy_U,X_Place_Satefy_L,Number_Block,Offset_Block_X,Offset_Block_Y,Number_FPCB,Offset_FPCB_X,Offset_FPCB_Y,X_Camtop_Satefy_U,X_Camtop_Satefy_L,Y_Camtop_Satefy_U,Y_Camtop_Satefy_L,Z_Camtop_Satefy,M_Total_Check,M_Offset_X_Block,M_Offset_Y_Block,M_Offset_X_FPCB,M_Offset_Y_FPCB) " +
                "VALUES (@STT,@X,@Y,@Z,@Z_antoan,@A2_antoan,@SP_Home,@Z_Place_Satefy,@Y_Place_Satefy_L,@X_Place_Satefy_U,@X_Place_Satefy_L,@Number_Block,@Offset_Block_X,@Offset_Block_Y,@Number_FPCB,@Offset_FPCB_X,@Offset_FPCB_Y,@X_Camtop_Satefy_U,@X_Camtop_Satefy_L,@Y_Camtop_Satefy_U,@Y_Camtop_Satefy_L,@Z_Camtop_Satefy,@M_Total_Check,@M_Offset_X_Block,@M_Offset_Y_Block,@M_Offset_X_FPCB,@M_Offset_Y_FPCB)");
            using (var conn_ = GetConnectionSQLite())
            {
                conn_.Open();
                using (SQLiteCommand cmd = new SQLiteCommand(saveposSQL, conn_))
                {
                    cmd.Parameters.AddWithValue("@STT", 1);
                    cmd.Parameters.AddWithValue("@X", Convert.ToDouble(data_Pos[0]));
                    cmd.Parameters.AddWithValue("@Y", Convert.ToDouble(data_Pos[1]));
                    cmd.Parameters.AddWithValue("@Z", Convert.ToDouble(data_Pos[2]));
                    cmd.Parameters.AddWithValue("@Z_antoan", Convert.ToDouble(data_Pos[3]));
                    cmd.Parameters.AddWithValue("@A2_antoan", Convert.ToDouble(data_Pos[4]));
                    cmd.Parameters.AddWithValue("@SP_Home", Convert.ToDouble(data_Pos[5]));
                    cmd.Parameters.AddWithValue("@Z_Place_Satefy", Convert.ToDouble(data_Pos[6]));
                    cmd.Parameters.AddWithValue("@Y_Place_Satefy_L", Convert.ToDouble(data_Pos[7]));
                    cmd.Parameters.AddWithValue("@X_Place_Satefy_U", Convert.ToDouble(data_Pos[8]));
                    cmd.Parameters.AddWithValue("@X_Place_Satefy_L", Convert.ToDouble(data_Pos[9]));
                    //
                    cmd.Parameters.AddWithValue("@Number_Block", Convert.ToDouble(data_Pos[10]));
                    cmd.Parameters.AddWithValue("@Offset_Block_X", Convert.ToDouble(data_Pos[11]));
                    cmd.Parameters.AddWithValue("@Offset_Block_Y", Convert.ToDouble(data_Pos[12]));
                    cmd.Parameters.AddWithValue("@Number_FPCB", Convert.ToDouble(data_Pos[13]));
                    cmd.Parameters.AddWithValue("@Offset_FPCB_X", Convert.ToDouble(data_Pos[14]));
                    cmd.Parameters.AddWithValue("@Offset_FPCB_Y", Convert.ToDouble(data_Pos[15]));
                    //
                    cmd.Parameters.AddWithValue("@X_Camtop_Satefy_U", Convert.ToDouble(data_Pos[16]));
                    cmd.Parameters.AddWithValue("@X_Camtop_Satefy_L", Convert.ToDouble(data_Pos[17]));
                    cmd.Parameters.AddWithValue("@Y_Camtop_Satefy_U", Convert.ToDouble(data_Pos[18]));
                    cmd.Parameters.AddWithValue("@Y_Camtop_Satefy_L", Convert.ToDouble(data_Pos[19]));
                    cmd.Parameters.AddWithValue("@Z_Camtop_Satefy", Convert.ToDouble(data_Pos[20]));
                    //
                    cmd.Parameters.AddWithValue("@M_Total_Check", Convert.ToDouble(data_Pos[21]));
                    cmd.Parameters.AddWithValue("@M_Offset_X_Block", Convert.ToDouble(data_Pos[22]));
                    cmd.Parameters.AddWithValue("@M_Offset_Y_Block", Convert.ToDouble(data_Pos[23]));
                    cmd.Parameters.AddWithValue("@M_Offset_X_FPCB", Convert.ToDouble(data_Pos[24]));
                    cmd.Parameters.AddWithValue("@M_Offset_Y_FPCB", Convert.ToDouble(data_Pos[25]));
                    cmd.ExecuteNonQuery();
                    WaitFormHelper.TransferInfo("Transfer :Data Setup Robot complete...");
                }
            }
        }
        private void Sub_GreatValue_CheckMarking()
        {
            try
            {
                double[] data_pos_X = new double[Global.Number_Tool];
                double[] data_pos_Y = new double[Global.Number_Tool];
                double[] data_pos_Z = new double[Global.Number_Tool];
                double[] data_pos_A3 = new double[Global.Number_Tool];
                double[] data_pos_A4 = new double[Global.Number_Tool];
                double[] data_pos_C = new double[Global.Number_Tool];
                int number_block = Convert.ToInt32(Global.Number_Block);
                int number_Fpcb_block = Convert.ToInt32(Global.Number_FPCB);
                int Total_Check_Cam_Marking = Convert.ToInt32(Global.Total_Check_Marking);
                double offset_X_Fpcb = Global.Offset_FPCB_X_Marking;
                double offset_Y_Fpcb = Global.Offset_FPCB_Y_Marking;
                double offset_X_Block = Global.Offset_Block_X_marking;
                double offset_Y_Block = Global.Offset_Block_Y_Marking;
                int k = 0;
                using (var conn_ = GetConnectionSQLite())
                {
                    conn_.Open();
                    using (var cmd = new SQLiteCommand("DELETE FROM Matrix_Panel_Cam_Top;", conn_))
                    {
                        cmd.ExecuteNonQuery();
                    }
                }
                if (number_block != 0)
                {
                    for (int i = 0; i < Total_Check_Cam_Marking; i++)
                    {
                        data_pos_X[i] = Global.Check_Marking_Start[0] + offset_X_Fpcb * i;
                        data_pos_Y[i] = Global.Check_Marking_Start[1] + offset_Y_Fpcb * i;
                        data_pos_Z[i] = Global.Check_Marking_Start[2];
                        data_pos_A3[i] = Global.Check_Marking_Start[3];
                        data_pos_A4[i] = Global.Check_Marking_Start[4];
                        data_pos_C[i] = Global.Check_Marking_Start[5];
                        matrix.Write_SQL2(data_pos_X, data_pos_Y, data_pos_Z, data_pos_A3, data_pos_A4, data_pos_C, Total_Check_Cam_Marking);
                    }
                    WaitFormHelper.TransferInfo("Transfer :Data Check Marking complete...");
                }

            }
            catch { WaitFormHelper.TransferInfo("Transfer :Data Check Marking Error..."); }
        }
        private void Sub_GreatValue_PlaceTray()
        {
            try
            {
                using (var conn_ = GetConnectionSQLite())
                {
                    conn_.Open();
                    using (var cmd = new SQLiteCommand("DELETE FROM Matrix_Panel_Tool_1_1;", conn_))
                    {
                        cmd.ExecuteNonQuery();
                    }
                }
                PLC1.Write_Data_Word_("D" + (9006 + Memory_PLC.K2000).ToString(), Convert.ToInt16(txt_Row1_x10.Text));
                PLC1.Write_Data_Word_("D" + (9007 + Memory_PLC.K2000).ToString(), Convert.ToInt16(txt_Column1_x10.Text));
                PLC1.Write_Data_Word_("D" + (9008 + Memory_PLC.K2000).ToString(), Convert.ToInt16(txt_Row2_x10.Text));
                PLC1.Write_Data_Word_("D" + (9009 + Memory_PLC.K2000).ToString(), Convert.ToInt16(txt_Column2_x10.Text));
                PLC1.Write_Data_Word_("D" + (9010 + Memory_PLC.K2000).ToString(), Convert.ToInt16(txt_Row3_x8.Text));
                PLC1.Write_Data_Word_("D" + (9011 + Memory_PLC.K2000).ToString(), Convert.ToInt16(txt_Column3_x8.Text));
                PLC1.Write_Data_DWord_("D" + (9014 + Memory_PLC.K2000).ToString(), Convert.ToInt32(Convert.ToDouble(txt_distance_row.Text) * 1000));
                PLC1.Write_Data_DWord_("D" + (9016 + Memory_PLC.K2000).ToString(), Convert.ToInt32(Convert.ToDouble(txt_distance_column.Text) * 1000));
                //
                int row_matrix_1 = Convert.ToInt16(txt_Row1_x10.Text);
                int column_matrix_1 = Convert.ToInt16(txt_Column1_x10.Text);
                int row_matrix_2 = Convert.ToInt16(txt_Row2_x10.Text);
                int column_matrix_2 = Convert.ToInt16(txt_Column2_x10.Text);
                int row_matrix_3 = Convert.ToInt16(txt_Row3_x8.Text);
                int column_matrix_3 = Convert.ToInt16(txt_Column3_x8.Text);
                Global.Row_tray_matrix1 = row_matrix_1;
                Global.Column_tray_matrix1 = column_matrix_1;
                Global.Row_tray_matrix2 = row_matrix_1;
                Global.Column_tray_matrix2 = column_matrix_1;
                Global.Row_tray_matrix3 = row_matrix_3;
                Global.Column_tray_matrix3 = column_matrix_3;
                //
                double distance_tray_X = 0;
                double distance_tray_Y = 0;
                double distance_alpha = Convert.ToDouble(Offset_Y_BlockFPCB_Tool_tray.Text);
                if (RadioButton_Chieu_Y.Checked == true)
                {
                    distance_tray_X = Convert.ToDouble(txt_distance_column.Text);
                    distance_tray_Y = Convert.ToDouble(txt_distance_row.Text);
                }
                else if (RadioButton_Chieu_X.Checked == true)
                {
                    distance_tray_X = Convert.ToDouble(txt_distance_row.Text);
                    distance_tray_Y = Convert.ToDouble(txt_distance_column.Text);
                }
                double offset_X_Fpcb = Global.Offset_FPCB_X;
                double offset_Y_Fpcb = Global.Offset_FPCB_Y;
                if (CheckBox_Use_Matrix1.Checked == true)
                {
                    #region Maxtrix 1
                    //1
                    double[] data_pos_Matrix1_1 = new double[6];
                    double[] data_pos_Matrix1_2 = new double[6];
                    double[] data_pos_Matrix1_3 = new double[6];
                    if (RadioButton_Chieu_Y.Checked == true)
                    {
                        data_pos_Matrix1_1 = Global.Place_Tray_1;
                        data_pos_Matrix1_2 = new double[6] { Global.Place_Tray_1[0] - distance_alpha, Global.Place_Tray_1[1] + distance_tray_Y * (row_matrix_1 - 1), Global.Place_Tray_1[2], Global.Place_Tray_1[3], Global.Place_Tray_1[4], Global.Place_Tray_1[5] };
                        data_pos_Matrix1_3 = new double[6] { Global.Place_Tray_1[0] - distance_tray_X * (column_matrix_1 - 1), Global.Place_Tray_1[1], Global.Place_Tray_1[2], Global.Place_Tray_1[3], Global.Place_Tray_1[4], Global.Place_Tray_1[5] };
                    }
                    else if (RadioButton_Chieu_X.Checked == true)
                    {
                        data_pos_Matrix1_1 = Global.Place_Tray_1;
                        data_pos_Matrix1_2 = new double[6] { Global.Place_Tray_1[0] + distance_tray_X * (row_matrix_1 - 1), Global.Place_Tray_1[1], Global.Place_Tray_1[2], Global.Place_Tray_1[3], Global.Place_Tray_1[4], Global.Place_Tray_1[5] };
                        data_pos_Matrix1_3 = new double[6] { Global.Place_Tray_1[0] - distance_alpha, Global.Place_Tray_1[1] - distance_tray_Y * (column_matrix_1 - 1), Global.Place_Tray_1[2], Global.Place_Tray_1[3], Global.Place_Tray_1[4], Global.Place_Tray_1[5] };
                    }
                    //1-10
                    List<double>[] Matrix_tray1x10_1 = new List<double>[100];
                    List<double>[] Matrix_tray1x10_2 = new List<double>[100];
                    List<double>[] Matrix_tray1x10_3 = new List<double>[100];
                    double[] Array_X_x10_1 = new double[Convert.ToInt16(Global.Number_FPCB)];//array 1 x10                                                     
                    double[] Array_Y_x10_1 = new double[Convert.ToInt16(Global.Number_FPCB)];//array 1 x10  
                    double[] Array_X_x10_2 = new double[Convert.ToInt16(Global.Number_FPCB)];//array 1 x10                                                     
                    double[] Array_Y_x10_2 = new double[Convert.ToInt16(Global.Number_FPCB)];//array 1 x10  
                    double[] Array_X_x10_3 = new double[Convert.ToInt16(Global.Number_FPCB)];//array 1 x10                                                     
                    double[] Array_Y_x10_3 = new double[Convert.ToInt16(Global.Number_FPCB)];//array 1 x10  

                    // điểm 1                                                       
                    for (int i = 0; i < Global.Number_FPCB; i++)
                    {
                        Array_X_x10_1[i] = data_pos_Matrix1_1[0] - offset_X_Fpcb * i;
                        Array_Y_x10_1[i] = data_pos_Matrix1_1[1] + offset_Y_Fpcb * i;
                    }
                    //điểm 2
                    for (int i = 0; i < Global.Number_FPCB; i++)
                    {
                        Array_X_x10_2[i] = data_pos_Matrix1_2[0] - offset_X_Fpcb * i;
                        Array_Y_x10_2[i] = data_pos_Matrix1_2[1] + offset_Y_Fpcb * i;
                    }
                    //điểm 3
                    for (int i = 0; i < Global.Number_FPCB; i++)
                    {
                        Array_X_x10_3[i] = data_pos_Matrix1_3[0] - offset_X_Fpcb * i;
                        Array_Y_x10_3[i] = data_pos_Matrix1_3[1] + offset_Y_Fpcb * i;
                    }
                    for (int i = 0; i < Global.Number_FPCB; i++)
                    {
                        Matrix_tray1x10_1[i] = new List<double>();
                        Matrix_tray1x10_1[i].Add(Array_X_x10_1[i]);
                        Matrix_tray1x10_1[i].Add(Array_Y_x10_1[i]);
                        Matrix_tray1x10_1[i].Add(Global.Place_Tray_1[2]);
                        Matrix_tray1x10_1[i].Add(Global.Place_Tray_1[3]);
                        Matrix_tray1x10_1[i].Add(Global.Place_Tray_1[4]);
                        Matrix_tray1x10_1[i].Add(Global.Place_Tray_1[5]);
                    }
                    for (int i = 0; i < Global.Number_FPCB; i++)
                    {
                        Matrix_tray1x10_2[i] = new List<double>();
                        Matrix_tray1x10_2[i].Add(Array_X_x10_2[i]);
                        Matrix_tray1x10_2[i].Add(Array_Y_x10_2[i]);
                        Matrix_tray1x10_2[i].Add(Global.Place_Tray_1[2]);
                        Matrix_tray1x10_2[i].Add(Global.Place_Tray_1[3]);
                        Matrix_tray1x10_2[i].Add(Global.Place_Tray_1[4]);
                        Matrix_tray1x10_2[i].Add(Global.Place_Tray_1[5]);
                    }
                    for (int i = 0; i < Global.Number_FPCB; i++)
                    {
                        Matrix_tray1x10_3[i] = new List<double>();
                        Matrix_tray1x10_3[i].Add(Array_X_x10_3[i]);
                        Matrix_tray1x10_3[i].Add(Array_Y_x10_3[i]);
                        Matrix_tray1x10_3[i].Add(Global.Place_Tray_1[2]);
                        Matrix_tray1x10_3[i].Add(Global.Place_Tray_1[3]);
                        Matrix_tray1x10_3[i].Add(Global.Place_Tray_1[4]);
                        Matrix_tray1x10_3[i].Add(Global.Place_Tray_1[5]);
                    }
                    List<double>[] matrix_panel_1 = new List<double>[10];
                    List<double>[] matrix_panel_2 = new List<double>[10];
                    List<double>[] matrix_panel_3 = new List<double>[10];
                    matrix_panel_1 = Matrix_tray1x10_1;
                    matrix_panel_2 = Matrix_tray1x10_2;
                    matrix_panel_3 = Matrix_tray1x10_3;
                    for (int i = 0; i < Global.Number_FPCB; i++)
                    {
                        matrix.PAL_PR_RB_1_1(i + 1, row_matrix_1, column_matrix_1, Matrix_tray1x10_1, Matrix_tray1x10_2, Matrix_tray1x10_3, i);
                    }
                    #endregion
                }
                if (CheckBox_Use_Matrix2.Checked == true)
                {
                    #region Matrix 2
                    //1
                    double[] data_pos_Matrix2_1 = new double[6];
                    double[] data_pos_Matrix2_2 = new double[6];
                    double[] data_pos_Matrix2_3 = new double[6];
                    if (RadioButton_Chieu_Y.Checked == true)
                    {
                        data_pos_Matrix2_1 = Global.Place_Tray_2;
                        data_pos_Matrix2_2 = new double[6] { Global.Place_Tray_2[0] - distance_alpha, Global.Place_Tray_2[1] + distance_tray_Y * (row_matrix_2 - 1), Global.Place_Tray_2[2], Global.Place_Tray_2[3], Global.Place_Tray_2[4], Global.Place_Tray_2[5] };
                        data_pos_Matrix2_3 = new double[6] { Global.Place_Tray_2[0] - distance_tray_X * (column_matrix_2 - 1), Global.Place_Tray_2[1], Global.Place_Tray_2[2], Global.Place_Tray_2[3], Global.Place_Tray_2[4], Global.Place_Tray_2[5] };
                    }
                    else if (RadioButton_Chieu_X.Checked == true)
                    {
                        data_pos_Matrix2_1 = Global.Place_Tray_2;
                        data_pos_Matrix2_2 = new double[6] { Global.Place_Tray_2[0] + distance_tray_X * (row_matrix_2 - 1), Global.Place_Tray_2[1], Global.Place_Tray_2[2], Global.Place_Tray_2[3], Global.Place_Tray_2[4], Global.Place_Tray_2[5] };
                        data_pos_Matrix2_3 = new double[6] { Global.Place_Tray_2[0] - distance_alpha, Global.Place_Tray_2[1] - distance_tray_Y * (column_matrix_2 - 1), Global.Place_Tray_2[2], Global.Place_Tray_2[3], Global.Place_Tray_2[4], Global.Place_Tray_2[5] };
                    }
                    //1-10
                    List<double>[] Matrix_tray1x10_2_1 = new List<double>[100];
                    List<double>[] Matrix_tray1x10_2_2 = new List<double>[100];
                    List<double>[] Matrix_tray1x10_2_3 = new List<double>[100];
                    double[] Array_X_x10_2_1 = new double[Convert.ToInt16(Global.Number_FPCB)];//array 1 x10                                                     //
                    double[] Array_Y_x10_2_1 = new double[Convert.ToInt16(Global.Number_FPCB)];//array 1 x10  
                    double[] Array_X_x10_2_2 = new double[Convert.ToInt16(Global.Number_FPCB)];//array 1 x10                                                     //
                    double[] Array_Y_x10_2_2 = new double[Convert.ToInt16(Global.Number_FPCB)];//array 1 x10  
                    double[] Array_X_x10_2_3 = new double[Convert.ToInt16(Global.Number_FPCB)];//array 1 x10                                                     //
                    double[] Array_Y_x10_2_3 = new double[Convert.ToInt16(Global.Number_FPCB)];//array 1 x10  
                                                                                               // điểm 1
                    for (int i = 0; i < Global.Number_FPCB; i++)
                    {
                        Array_X_x10_2_1[i] = data_pos_Matrix2_1[0] - offset_X_Fpcb * i;
                        Array_Y_x10_2_1[i] = data_pos_Matrix2_1[1] + offset_Y_Fpcb * i;
                    }
                    //điểm 2
                    for (int i = 0; i < Global.Number_FPCB; i++)
                    {
                        Array_X_x10_2_2[i] = data_pos_Matrix2_2[0] - offset_X_Fpcb * i;
                        Array_Y_x10_2_2[i] = data_pos_Matrix2_2[1] + offset_Y_Fpcb * i;
                    }
                    //điểm 3
                    for (int i = 0; i < Global.Number_FPCB; i++)
                    {
                        Array_X_x10_2_3[i] = data_pos_Matrix2_3[0] - offset_X_Fpcb * i;
                        Array_Y_x10_2_3[i] = data_pos_Matrix2_3[1] + offset_Y_Fpcb * i;
                    }
                    for (int i = 0; i < Global.Number_FPCB; i++)
                    {
                        Matrix_tray1x10_2_1[i] = new List<double>();
                        Matrix_tray1x10_2_1[i].Add(Array_X_x10_2_1[i]);
                        Matrix_tray1x10_2_1[i].Add(Array_Y_x10_2_1[i]);
                        Matrix_tray1x10_2_1[i].Add(Global.Place_Tray_2[2]);
                        Matrix_tray1x10_2_1[i].Add(Global.Place_Tray_2[3]);
                        Matrix_tray1x10_2_1[i].Add(Global.Place_Tray_2[4]);
                        Matrix_tray1x10_2_1[i].Add(Global.Place_Tray_2[5]);
                    }
                    for (int i = 0; i < Global.Number_FPCB; i++)
                    {
                        Matrix_tray1x10_2_2[i] = new List<double>();
                        Matrix_tray1x10_2_2[i].Add(Array_X_x10_2_2[i]);
                        Matrix_tray1x10_2_2[i].Add(Array_Y_x10_2_2[i]);
                        Matrix_tray1x10_2_2[i].Add(Global.Place_Tray_2[2]);
                        Matrix_tray1x10_2_2[i].Add(Global.Place_Tray_2[3]);
                        Matrix_tray1x10_2_2[i].Add(Global.Place_Tray_2[4]);
                        Matrix_tray1x10_2_2[i].Add(Global.Place_Tray_2[5]);
                    }
                    for (int i = 0; i < Global.Number_FPCB; i++)
                    {
                        Matrix_tray1x10_2_3[i] = new List<double>();
                        Matrix_tray1x10_2_3[i].Add(Array_X_x10_2_3[i]);
                        Matrix_tray1x10_2_3[i].Add(Array_Y_x10_2_3[i]);
                        Matrix_tray1x10_2_3[i].Add(Global.Place_Tray_2[2]);
                        Matrix_tray1x10_2_3[i].Add(Global.Place_Tray_2[3]);
                        Matrix_tray1x10_2_3[i].Add(Global.Place_Tray_2[4]);
                        Matrix_tray1x10_2_3[i].Add(Global.Place_Tray_2[5]);
                    }
                    for (int i = 0; i < Global.Number_FPCB; i++)
                    {
                        matrix.PAL_PR_RB_1_1(Convert.ToInt32(Global.Number_FPCB) + 1 + i, row_matrix_2, column_matrix_2, Matrix_tray1x10_2_1, Matrix_tray1x10_2_2, Matrix_tray1x10_2_3, i);
                    }
                    #endregion
                }
                //Scan Điều kiện Pos thả tray theo parameter Satefy 
                if (Scan_Position == 2 && Global.Home_All == false)
                {
                    if (Global.Place_Tray_1[0] > Global.X_Place_Satefy_L && Global.Place_Tray_1[0] < Global.X_Place_Satefy_U && Global.Place_Tray_1[1] < Global.Y_Place_Satefy_U && Global.Place_Tray_1[1] > Global.Y_Place_Satefy_L && Global.Place_Tray_1[2] > Global.Z_Place_Satefy
                       && Global.Place_Tray_2[0] > Global.X_Place_Satefy_L && Global.Place_Tray_2[0] < Global.X_Place_Satefy_U && Global.Place_Tray_2[1] < Global.Y_Place_Satefy_U && Global.Place_Tray_2[1] > Global.Y_Place_Satefy_L && Global.Place_Tray_2[2] > Global.Z_Place_Satefy
                      && Global.Place_Tray_3[0] > Global.X_Place_Satefy_L && Global.Place_Tray_3[0] < Global.X_Place_Satefy_U && Global.Place_Tray_3[1] < Global.Y_Place_Satefy_U && Global.Place_Tray_3[1] > Global.Y_Place_Satefy_L && Global.Place_Tray_3[2] > Global.Z_Place_Satefy)
                    {
                        //code
                    }
                    else
                    {
                        WaitFormHelper.TransferInfo("Transfer : Data Place tray Error Satefy...");
                    }
                }
                else
                {
                    WaitFormHelper.TransferInfo("Transfer : Data Place tray Error Satefy...");
                }
                WaitFormHelper.TransferInfo("Transfer : Data Place tray complete...");
            }
            catch
            {
                WaitFormHelper.TransferInfo("Transfer : Data Place tray Error...");
            }
        }
        //Sub transfer data plc
        private void Sub_TransferValueLoading()
        {
            //Loading
            PLC1.Write_Data_DWord_("D" + (6735 + Memory_PLC.K100).ToString(), parameterModel.parameterPLC_Loading.Cor_Input);
            PLC1.Write_Data_DWord_("D" + (6737 + Memory_PLC.K100).ToString(), parameterModel.parameterPLC_Loading.Cor_Output);
        }
        private void Sub_TransferValueVision()
        {
            //Vision X
            PLC1.Write_Data_DWord_("D" + (5337 + Memory_PLC.K800).ToString(), parameterModel.parameterPLC_Vision.CorX_Pick1);
            PLC1.Write_Data_DWord_("D" + (5368 + Memory_PLC.K800).ToString(), parameterModel.parameterPLC_Vision.CorX_Pick2);
            PLC1.Write_Data_DWord_("D" + (5341 + Memory_PLC.K800).ToString(), parameterModel.parameterPLC_Vision.CorX_Check1);
            PLC1.Write_Data_DWord_("D" + (5343 + Memory_PLC.K800).ToString(), parameterModel.parameterPLC_Vision.CorX_Check2);
            PLC1.Write_Data_DWord_("D" + (5345 + Memory_PLC.K800).ToString(), parameterModel.parameterPLC_Vision.CorX_Check3);
            PLC1.Write_Data_DWord_("D" + (5347 + Memory_PLC.K800).ToString(), parameterModel.parameterPLC_Vision.CorX_Check4);
            PLC1.Write_Data_DWord_("D" + (5353 + Memory_PLC.K800).ToString(), parameterModel.parameterPLC_Vision.CorX_Output);
            //Vision Y
            PLC1.Write_Data_DWord_("D" + (5437 + Memory_PLC.K800).ToString(), parameterModel.parameterPLC_Vision.CorY_Pick1);
            PLC1.Write_Data_DWord_("D" + (5468 + Memory_PLC.K800).ToString(), parameterModel.parameterPLC_Vision.CorY_Pick2);
            PLC1.Write_Data_DWord_("D" + (5441 + Memory_PLC.K800).ToString(), parameterModel.parameterPLC_Vision.CorY_Check1);
            PLC1.Write_Data_DWord_("D" + (5443 + Memory_PLC.K800).ToString(), parameterModel.parameterPLC_Vision.CorY_Check2);
            PLC1.Write_Data_DWord_("D" + (5445 + Memory_PLC.K800).ToString(), parameterModel.parameterPLC_Vision.CorY_Check3);
            PLC1.Write_Data_DWord_("D" + (5447 + Memory_PLC.K800).ToString(), parameterModel.parameterPLC_Vision.CorY_Check4);
            PLC1.Write_Data_DWord_("D" + (5453 + Memory_PLC.K800).ToString(), parameterModel.parameterPLC_Vision.CorY_Output);
            //Vision Z
            PLC1.Write_Data_DWord_("D" + (5537 + Memory_PLC.K800).ToString(), parameterModel.parameterPLC_Vision.CorZ_Pick1);
            PLC1.Write_Data_DWord_("D" + (5568 + Memory_PLC.K800).ToString(), parameterModel.parameterPLC_Vision.CorZ_Pick2);
            PLC1.Write_Data_DWord_("D" + (5541 + Memory_PLC.K800).ToString(), parameterModel.parameterPLC_Vision.CorZ_Check1);
            PLC1.Write_Data_DWord_("D" + (5543 + Memory_PLC.K800).ToString(), parameterModel.parameterPLC_Vision.CorZ_Check2);
            PLC1.Write_Data_DWord_("D" + (5545 + Memory_PLC.K800).ToString(), parameterModel.parameterPLC_Vision.CorZ_Check3);
            PLC1.Write_Data_DWord_("D" + (5547 + Memory_PLC.K800).ToString(), parameterModel.parameterPLC_Vision.CorZ_Check4);
            PLC1.Write_Data_DWord_("D" + (5553 + Memory_PLC.K800).ToString(), parameterModel.parameterPLC_Vision.CorZ_Output);
            //Vision R
            PLC1.Write_Data_DWord_("D" + (5637 + Memory_PLC.K800).ToString(), parameterModel.parameterPLC_Vision.CorR_Pick1);
            PLC1.Write_Data_DWord_("D" + (5668 + Memory_PLC.K800).ToString(), parameterModel.parameterPLC_Vision.CorR_Pick2);
            PLC1.Write_Data_DWord_("D" + (5641 + Memory_PLC.K800).ToString(), parameterModel.parameterPLC_Vision.CorR_Check1);
            PLC1.Write_Data_DWord_("D" + (5653 + Memory_PLC.K800).ToString(), parameterModel.parameterPLC_Vision.CorR_Output);
        }
        private void Sub_TransferParameterVision()
        {
            try
            {
                PLC1.Write_Data_DWord_("D" + (9050 + Memory_PLC.K2000).ToString(), Convert.ToInt32(txt_number_block_FPCB.Text));
                PLC1.Write_Data_DWord_("D" + (9052 + Memory_PLC.K2000).ToString(), Convert.ToInt32(txt_number_FPCB_Block.Text));
                PLC1.Write_Data_DWord_("D" + (9054 + Memory_PLC.K2000).ToString(), Convert.ToInt32(Convert.ToDouble(txt_Offset_X_FPCB.Text) * 1000));
                PLC1.Write_Data_DWord_("D" + (9056 + Memory_PLC.K2000).ToString(), Convert.ToInt32(Convert.ToDouble(txt_Offset_Y_FPCB.Text) * 1000));
                PLC1.Write_Data_DWord_("D" + (9058 + Memory_PLC.K2000).ToString(), Convert.ToInt32(Convert.ToDouble(txt_Offset_X_Block.Text) * 1000));
                PLC1.Write_Data_DWord_("D" + (9060 + Memory_PLC.K2000).ToString(), Convert.ToInt32(Convert.ToDouble(txt_Offset_Y_Block.Text) * 1000));
                PLC1.Write_Data_DWord_("D" + (9062 + Memory_PLC.K2000).ToString(), Convert.ToInt32(txt_number_data.Text));
                PLC1.Write_Data_DWord_("D" + (9100 + Memory_PLC.K2000).ToString(), Convert.ToInt32(Convert.ToDouble(txt_Total_Check.Text)));
                int Number_Tool_Check_VisionBot = Convert.ToInt16(txt_Total_Check.Text);
                int[] data_pos_X1 = new int[Number_Tool_Check_VisionBot];
                int[] data_pos_Y1 = new int[Number_Tool_Check_VisionBot];
                //                          
                int[] data_pos_X2 = new int[Number_Tool_Check_VisionBot];
                int[] data_pos_Y2 = new int[Number_Tool_Check_VisionBot];
                //                          
                int[] data_pos_X3 = new int[Number_Tool_Check_VisionBot];
                int[] data_pos_Y3 = new int[Number_Tool_Check_VisionBot];
                //                          
                int[] data_pos_X4 = new int[Number_Tool_Check_VisionBot];
                int[] data_pos_Y4 = new int[Number_Tool_Check_VisionBot];
                //
                int number_block = Convert.ToInt16(txt_number_block_FPCB.Text);
                int number_Fpcb_block = Convert.ToInt16(txt_number_FPCB_Block.Text);
                int offset_X_Fpcb = Convert.ToInt32(Convert.ToDouble(txt_Offset_X_FPCB.Text) * 1000);
                int offset_Y_Fpcb = Convert.ToInt32(Convert.ToDouble(txt_Offset_Y_FPCB.Text) * 1000);
                int offset_X_Block = Convert.ToInt32(Convert.ToDouble(txt_Offset_X_Block.Text) * 1000);
                int offset_Y_Block = Convert.ToInt32(Convert.ToDouble(txt_Offset_Y_Block.Text) * 1000);
                int k = 0;
                if (number_block > 0)
                {
                    //POS CHUP 1
                    for (int i = 0; i < number_block; i++)
                    {
                        for (int j = 0; j < number_Fpcb_block; j++)
                        {
                            if (i == 0)
                            {
                                data_pos_X1[k] = Convert.ToInt32(Convert.ToDouble(COR5341.Text) * 1000) + offset_X_Fpcb * j;
                                data_pos_Y1[k] = Convert.ToInt32(Convert.ToDouble(COR5441.Text) * 1000 + offset_Y_Fpcb * j);
                            }
                            else if (i > 0 && k != number_Fpcb_block * i)
                            {
                                data_pos_X1[k] = data_pos_X1[k - 1 * j] + offset_X_Fpcb * j;
                                //data_pos_Y[k] = data_pos_Y[k - 1 * j] + offset_Y_Fpcb * j;
                                data_pos_Y1[k] = Convert.ToInt32(Convert.ToDouble(data_pos_Y1[k - 1 * j] + offset_Y_Fpcb * j));
                            }
                            else if (k == i * number_Fpcb_block)
                            {
                                data_pos_X1[k] = data_pos_X1[k - 1] + offset_X_Fpcb * j + offset_X_Block;
                                //data_pos_Y[k] = data_pos_Y[k - 1] + offset_Y_Fpcb * j + offset_Y_Block;
                                data_pos_Y1[k] = Convert.ToInt32(Convert.ToDouble(data_pos_Y1[k - 1] + offset_Y_Fpcb * j + offset_Y_Block));
                            }
                            //data_pos_Y1[k] = Convert.ToInt32(Convert.ToDouble(COR5441.Text) * 1000);
                            k++;
                        }
                    }
                    //POS CHUP 2
                    k = 0;
                    for (int i = 0; i < number_block; i++)
                    {
                        for (int j = 0; j < number_Fpcb_block; j++)
                        {
                            if (i == 0)
                            {
                                data_pos_X2[k] = Convert.ToInt32(Convert.ToDouble(COR5343.Text) * 1000) + offset_X_Fpcb * j;
                                //data_pos_Y[k] = Convert.ToInt32(txt_D5441.Text) + offset_Y_Fpcb * j;
                                data_pos_Y2[k] = Convert.ToInt32(Convert.ToDouble(COR5443.Text) * 1000 + offset_Y_Fpcb * j);
                            }
                            else if (i > 0 && k != number_Fpcb_block * i)
                            {
                                data_pos_X2[k] = data_pos_X2[k - 1 * j] + offset_X_Fpcb * j;
                                //data_pos_Y[k] = data_pos_Y[k - 1 * j] + offset_Y_Fpcb * j;
                                data_pos_Y2[k] = Convert.ToInt32(Convert.ToDouble(data_pos_Y2[k - 1 * j] + offset_Y_Fpcb * j));
                            }
                            else if (k == i * number_Fpcb_block)
                            {
                                data_pos_X2[k] = data_pos_X2[k - 1] + offset_X_Fpcb * j + offset_X_Block;
                                //data_pos_Y[k] = data_pos_Y[k - 1] + offset_Y_Fpcb * j + offset_Y_Block;
                                data_pos_Y2[k] = Convert.ToInt32(Convert.ToDouble(data_pos_Y2[k - 1] + offset_Y_Fpcb * j + offset_Y_Block));
                            }
                            //data_pos_Y2[k] = Convert.ToInt32(Convert.ToDouble(COR5443.Text) * 1000);
                            k++;
                        }
                    }
                    //POS CHUP 3
                    k = 0;
                    for (int i = 0; i < number_block; i++)
                    {
                        for (int j = 0; j < number_Fpcb_block; j++)
                        {
                            if (i == 0)
                            {
                                data_pos_X3[k] = Convert.ToInt32(Convert.ToDouble(COR5345.Text) * 1000) + offset_X_Fpcb * j;
                                //data_pos_Y[k] = Convert.ToInt32(txt_D5441.Text) + offset_Y_Fpcb * j;
                                data_pos_Y3[k] = Convert.ToInt32(Convert.ToDouble(COR5445.Text) * 1000 + offset_Y_Fpcb * j);
                            }
                            else if (i > 0 && k != number_Fpcb_block * i)
                            {
                                data_pos_X3[k] = data_pos_X3[k - 1 * j] + offset_X_Fpcb * j;
                                //data_pos_Y[k] = data_pos_Y[k - 1 * j] + offset_Y_Fpcb * j;
                                data_pos_Y3[k] = Convert.ToInt32(Convert.ToDouble(data_pos_Y3[k - 1 * j] + offset_Y_Fpcb * j));
                            }
                            else if (k == i * number_Fpcb_block)
                            {
                                data_pos_X3[k] = data_pos_X3[k - 1] + offset_X_Fpcb * j + offset_X_Block;
                                //data_pos_Y[k] = data_pos_Y[k - 1] + offset_Y_Fpcb * j + offset_Y_Block;
                                data_pos_Y3[k] = Convert.ToInt32(Convert.ToDouble(data_pos_Y3[k - 1] + offset_Y_Fpcb * j + offset_Y_Block));
                            }
                            // data_pos_Y3[k] = Convert.ToInt32(Convert.ToDouble(COR5445.Text) * 1000);
                            k++;
                        }
                    }
                    //         
                    //POS CHUP 4
                    k = 0;
                    for (int i = 0; i < number_block; i++)
                    {
                        for (int j = 0; j < number_Fpcb_block; j++)
                        {
                            if (i == 0)
                            {
                                data_pos_X4[k] = Convert.ToInt32(Convert.ToDouble(COR5347.Text) * 1000) + offset_X_Fpcb * j;
                                //data_pos_Y[k] = Convert.ToInt32(txt_D5441.Text) + offset_Y_Fpcb * j;
                                data_pos_Y4[k] = Convert.ToInt32(Convert.ToDouble(COR5447.Text) * 1000 + offset_Y_Fpcb * j);
                            }
                            else if (i > 0 && k != number_Fpcb_block * i)
                            {
                                data_pos_X4[k] = data_pos_X4[k - 1 * j] + offset_X_Fpcb * j;
                                //data_pos_Y[k] = data_pos_Y[k - 1 * j] + offset_Y_Fpcb * j;
                                data_pos_Y4[k] = Convert.ToInt32(Convert.ToDouble(data_pos_Y4[k - 1 * j] + offset_Y_Fpcb * j));
                            }
                            else if (k == i * number_Fpcb_block)
                            {
                                data_pos_X4[k] = data_pos_X4[k - 1] + offset_X_Fpcb * j + offset_X_Block;
                                //data_pos_Y[k] = data_pos_Y[k - 1] + offset_Y_Fpcb * j + offset_Y_Block;
                                data_pos_Y4[k] = Convert.ToInt32(Convert.ToDouble(data_pos_Y4[k - 1] + offset_Y_Fpcb * j + offset_Y_Block));
                            }
                            //data_pos_Y4[k] = Convert.ToInt32(Convert.ToDouble(COR5447.Text) * 1000);
                            k++;
                        }
                    }
                    //
                    //
                    using (var conn_ = GetConnectionSQLite())
                    {
                        conn_.Open();
                        using (var cmd = new SQLiteCommand("DELETE FROM Matrix_Panel1_Cam_PLC;", conn_))
                        {
                            cmd.ExecuteNonQuery();
                        }
                        using (var cmd = new SQLiteCommand("DELETE FROM Matrix_Panel2_Cam_PLC;", conn_))
                        {
                            cmd.ExecuteNonQuery();
                        }
                        using (var cmd = new SQLiteCommand("DELETE FROM Matrix_Panel3_Cam_PLC;", conn_))
                        {
                            cmd.ExecuteNonQuery();
                        }
                        using (var cmd = new SQLiteCommand("DELETE FROM Matrix_Panel4_Cam_PLC;", conn_))
                        {
                            cmd.ExecuteNonQuery();
                        }
                    }
                    matrix.Write_SQL(data_pos_X1, data_pos_Y1, Number_Tool_Check_VisionBot, "INSERT OR REPLACE INTO Matrix_Panel1_Cam_PLC (STT, X, Y) VALUES (@STT, @X, @Y)");
                    matrix.Write_SQL(data_pos_X2, data_pos_Y2, Number_Tool_Check_VisionBot, "INSERT OR REPLACE INTO Matrix_Panel2_Cam_PLC (STT, X, Y) VALUES (@STT, @X, @Y)");
                    matrix.Write_SQL(data_pos_X3, data_pos_Y3, Number_Tool_Check_VisionBot, "INSERT OR REPLACE INTO Matrix_Panel3_Cam_PLC (STT, X, Y) VALUES (@STT, @X, @Y)");
                    matrix.Write_SQL(data_pos_X4, data_pos_Y4, Number_Tool_Check_VisionBot, "INSERT OR REPLACE INTO Matrix_Panel4_Cam_PLC (STT, X, Y) VALUES (@STT, @X, @Y)");
                    for (int i = 0; i < Number_Tool_Check_VisionBot; i++)
                    {
                        PLC1.Write_Data_DWord_("D" + (10001 + Memory_PLC.K2000 + i * 2).ToString(), data_pos_X1[i]);// X POS CHECK 1
                        PLC1.Write_Data_DWord_("D" + (10101 + Memory_PLC.K2000 + i * 2).ToString(), data_pos_Y1[i]);// Y POS CHECK 1
                        PLC1.Write_Data_DWord_("D" + (10201 + Memory_PLC.K2000 + i * 2).ToString(), data_pos_X2[i]);// X POS CHECK 2
                        PLC1.Write_Data_DWord_("D" + (10301 + Memory_PLC.K2000 + i * 2).ToString(), data_pos_Y2[i]);// Y POS CHECK 2
                        PLC1.Write_Data_DWord_("D" + (10401 + Memory_PLC.K2000 + i * 2).ToString(), data_pos_X3[i]);// X POS CHECK 3
                        PLC1.Write_Data_DWord_("D" + (10501 + Memory_PLC.K2000 + i * 2).ToString(), data_pos_Y3[i]);// Y POS CHECK 3
                        PLC1.Write_Data_DWord_("D" + (13001 + Memory_PLC.K2000 + i * 2).ToString(), data_pos_X4[i]);// X POS CHECK 4
                        PLC1.Write_Data_DWord_("D" + (13101 + Memory_PLC.K2000 + i * 2).ToString(), data_pos_Y4[i]);// Y POS CHECK 4
                    }
                }
                else
                {
                    WaitFormHelper.TransferInfo("Transfer : Data Parameter Vision Error...");
                }
            }
            catch { }
            string[] Array_Offset_Z1 = new string[20];
            string[] Array_Offset_Z2 = new string[20];
            string[] Array_Offset_Z3 = new string[20];
            string[] Array_Offset_Z4 = new string[20];
            try
            {
                for (int i = 1; i <= 20; i++)
                {
                    UITextBox txt_name1 = this.Controls.Find("txt_Offset_Z_T" + i, true).FirstOrDefault() as UITextBox;
                    UITextBox txt_name2 = this.Controls.Find("txt_Offset_Z2_T" + i, true).FirstOrDefault() as UITextBox;
                    UITextBox txt_name3 = this.Controls.Find("txt_Offset_Z3_T" + i, true).FirstOrDefault() as UITextBox;
                    UITextBox txt_name4 = this.Controls.Find("txt_Offset_Z4_T" + i, true).FirstOrDefault() as UITextBox;
                    Array_Offset_Z1[i - 1] = txt_name1.Text;
                    Array_Offset_Z2[i - 1] = txt_name2.Text;
                    Array_Offset_Z3[i - 1] = txt_name3.Text;
                    Array_Offset_Z4[i - 1] = txt_name4.Text;
                }
                int total_check_FPCB = PLC1.Read_Data_DWord_("D" + (9100 + Memory_PLC.K2000).ToString());
                int[] IsUpdate = new int[total_check_FPCB];
                for (int j = 0; j < total_check_FPCB; j++)
                {
                    if (Convert.ToDouble(Array_Offset_Z1[j]) < 2 && Convert.ToDouble(Array_Offset_Z2[j]) < 2 && Convert.ToDouble(Array_Offset_Z3[j]) < 2 && Convert.ToDouble(Array_Offset_Z4[j]) < 2)
                    {
                        IsUpdate[j] = 1;
                    }
                    else
                    {
                        IsUpdate[j] = 0;
                    }
                }
                bool Is_Update = IsUpdate.Contains(0);
                if (Is_Update == false)
                {
                    using (var conn_ = GetConnectionSQLite())
                    {
                        conn_.Open();
                        using (var cmd = new SQLiteCommand("DELETE FROM Offset_Z_Check_Connector;", conn_))
                        {
                            cmd.ExecuteNonQuery();
                        }
                        using (var cmd = new SQLiteCommand("DELETE FROM Offset_Z_Check_FPCB;", conn_))
                        {
                            cmd.ExecuteNonQuery();
                        }
                        using (var cmd = new SQLiteCommand("DELETE FROM Offset_Z_Check_Bien_Dang;", conn_))
                        {
                            cmd.ExecuteNonQuery();
                        }
                        using (var cmd = new SQLiteCommand("DELETE FROM Offset_Z_Check_4;", conn_))
                        {
                            cmd.ExecuteNonQuery();
                        }
                        using (var cmd = new SQLiteCommand("DELETE FROM TABLE_OFFSET_Z_CHECK_VISION;", conn_))
                        {
                            cmd.ExecuteNonQuery();
                        }
                    }
                    #region Check 1
                    using (var conn_ = GetConnectionSQLite())
                    {
                        conn_.Open();
                        string saveposSQL1 = string.Format("INSERT OR REPLACE INTO Offset_Z_Check_Connector (STT,Z1,Z2,Z3,Z4,Z5,Z6,Z7,Z8,Z9,Z10, Z11,Z12,Z13,Z14,Z15,Z16,Z17,Z18,Z19,Z20) " +
                             "VALUES (@STT,@Z1,@Z2,@Z3,@Z4,@Z5,@Z6,@Z7,@Z8,@Z9,@Z10, @Z11,@Z12,@Z13,@Z14,@Z15,@Z16,@Z17,@Z18,@Z19,@Z20)");
                        using (SQLiteCommand cmd = new SQLiteCommand(saveposSQL1, conn_))
                        {
                            cmd.Parameters.AddWithValue("@STT", 1);
                            cmd.Parameters.AddWithValue("@Z1", Convert.ToDouble(Array_Offset_Z1[0]));
                            cmd.Parameters.AddWithValue("@Z2", Convert.ToDouble(Array_Offset_Z1[1]));
                            cmd.Parameters.AddWithValue("@Z3", Convert.ToDouble(Array_Offset_Z1[2]));
                            cmd.Parameters.AddWithValue("@Z4", Convert.ToDouble(Array_Offset_Z1[3]));
                            cmd.Parameters.AddWithValue("@Z5", Convert.ToDouble(Array_Offset_Z1[4]));
                            cmd.Parameters.AddWithValue("@Z6", Convert.ToDouble(Array_Offset_Z1[5]));
                            cmd.Parameters.AddWithValue("@Z7", Convert.ToDouble(Array_Offset_Z1[6]));
                            cmd.Parameters.AddWithValue("@Z8", Convert.ToDouble(Array_Offset_Z1[7]));
                            cmd.Parameters.AddWithValue("@Z9", Convert.ToDouble(Array_Offset_Z1[8]));
                            cmd.Parameters.AddWithValue("@Z10", Convert.ToDouble(Array_Offset_Z1[9]));
                            cmd.Parameters.AddWithValue("@Z11", Convert.ToDouble(Array_Offset_Z1[10]));
                            cmd.Parameters.AddWithValue("@Z12", Convert.ToDouble(Array_Offset_Z1[11]));
                            cmd.Parameters.AddWithValue("@Z13", Convert.ToDouble(Array_Offset_Z1[12]));
                            cmd.Parameters.AddWithValue("@Z14", Convert.ToDouble(Array_Offset_Z1[13]));
                            cmd.Parameters.AddWithValue("@Z15", Convert.ToDouble(Array_Offset_Z1[14]));
                            cmd.Parameters.AddWithValue("@Z16", Convert.ToDouble(Array_Offset_Z1[15]));
                            cmd.Parameters.AddWithValue("@Z17", Convert.ToDouble(Array_Offset_Z1[16]));
                            cmd.Parameters.AddWithValue("@Z18", Convert.ToDouble(Array_Offset_Z1[17]));
                            cmd.Parameters.AddWithValue("@Z19", Convert.ToDouble(Array_Offset_Z1[18]));
                            cmd.Parameters.AddWithValue("@Z20", Convert.ToDouble(Array_Offset_Z1[19]));
                            cmd.ExecuteNonQuery();
                        }
                    }
                    Z1_1 = Convert.ToDouble(txt_Offset_Z_T1.Text);
                    Z1_2 = Convert.ToDouble(txt_Offset_Z_T2.Text);
                    Z1_3 = Convert.ToDouble(txt_Offset_Z_T3.Text);
                    Z1_4 = Convert.ToDouble(txt_Offset_Z_T4.Text);
                    Z1_5 = Convert.ToDouble(txt_Offset_Z_T5.Text);
                    Z1_6 = Convert.ToDouble(txt_Offset_Z_T6.Text);
                    Z1_7 = Convert.ToDouble(txt_Offset_Z_T7.Text);
                    Z1_8 = Convert.ToDouble(txt_Offset_Z_T8.Text);
                    Z1_9 = Convert.ToDouble(txt_Offset_Z_T9.Text);
                    Z1_10 = Convert.ToDouble(txt_Offset_Z_T10.Text);
                    Z1_11 = Convert.ToDouble(txt_Offset_Z_T11.Text);
                    Z1_12 = Convert.ToDouble(txt_Offset_Z_T12.Text);
                    Z1_13 = Convert.ToDouble(txt_Offset_Z_T13.Text);
                    Z1_14 = Convert.ToDouble(txt_Offset_Z_T14.Text);
                    Z1_15 = Convert.ToDouble(txt_Offset_Z_T15.Text);
                    Z1_16 = Convert.ToDouble(txt_Offset_Z_T16.Text);
                    Z1_17 = Convert.ToDouble(txt_Offset_Z_T17.Text);
                    Z1_18 = Convert.ToDouble(txt_Offset_Z_T18.Text);
                    Z1_19 = Convert.ToDouble(txt_Offset_Z_T19.Text);
                    Z1_20 = Convert.ToDouble(txt_Offset_Z_T20.Text);
                    #endregion
                    #region Check 2
                    using (var conn_ = GetConnectionSQLite())
                    {
                        conn_.Open();
                        string saveposSQL2 = string.Format("INSERT OR REPLACE INTO Offset_Z_Check_FPCB (STT,Z1,Z2,Z3,Z4,Z5,Z6,Z7,Z8,Z9,Z10, Z11,Z12,Z13,Z14,Z15,Z16,Z17,Z18,Z19,Z20) " +
                        "VALUES (@STT,@Z1,@Z2,@Z3,@Z4,@Z5,@Z6,@Z7,@Z8,@Z9,@Z10, @Z11,@Z12,@Z13,@Z14,@Z15,@Z16,@Z17,@Z18,@Z19,@Z20)");
                        using (SQLiteCommand cmd = new SQLiteCommand(saveposSQL2, conn_))
                        {
                            cmd.Parameters.AddWithValue("@STT", 1);
                            cmd.Parameters.AddWithValue("@Z1", Convert.ToDouble(Array_Offset_Z2[0]));
                            cmd.Parameters.AddWithValue("@Z2", Convert.ToDouble(Array_Offset_Z2[1]));
                            cmd.Parameters.AddWithValue("@Z3", Convert.ToDouble(Array_Offset_Z2[2]));
                            cmd.Parameters.AddWithValue("@Z4", Convert.ToDouble(Array_Offset_Z2[3]));
                            cmd.Parameters.AddWithValue("@Z5", Convert.ToDouble(Array_Offset_Z2[4]));
                            cmd.Parameters.AddWithValue("@Z6", Convert.ToDouble(Array_Offset_Z2[5]));
                            cmd.Parameters.AddWithValue("@Z7", Convert.ToDouble(Array_Offset_Z2[6]));
                            cmd.Parameters.AddWithValue("@Z8", Convert.ToDouble(Array_Offset_Z2[7]));
                            cmd.Parameters.AddWithValue("@Z9", Convert.ToDouble(Array_Offset_Z2[8]));
                            cmd.Parameters.AddWithValue("@Z10", Convert.ToDouble(Array_Offset_Z2[9]));
                            cmd.Parameters.AddWithValue("@Z11", Convert.ToDouble(Array_Offset_Z2[10]));
                            cmd.Parameters.AddWithValue("@Z12", Convert.ToDouble(Array_Offset_Z2[11]));
                            cmd.Parameters.AddWithValue("@Z13", Convert.ToDouble(Array_Offset_Z2[12]));
                            cmd.Parameters.AddWithValue("@Z14", Convert.ToDouble(Array_Offset_Z2[13]));
                            cmd.Parameters.AddWithValue("@Z15", Convert.ToDouble(Array_Offset_Z2[14]));
                            cmd.Parameters.AddWithValue("@Z16", Convert.ToDouble(Array_Offset_Z2[15]));
                            cmd.Parameters.AddWithValue("@Z17", Convert.ToDouble(Array_Offset_Z2[16]));
                            cmd.Parameters.AddWithValue("@Z18", Convert.ToDouble(Array_Offset_Z2[17]));
                            cmd.Parameters.AddWithValue("@Z19", Convert.ToDouble(Array_Offset_Z2[18]));
                            cmd.Parameters.AddWithValue("@Z20", Convert.ToDouble(Array_Offset_Z2[19]));
                            cmd.ExecuteNonQuery();
                        }
                    }
                    Z2_1 = Convert.ToDouble(txt_Offset_Z2_T1.Text);
                    Z2_2 = Convert.ToDouble(txt_Offset_Z2_T2.Text);
                    Z2_3 = Convert.ToDouble(txt_Offset_Z2_T3.Text);
                    Z2_4 = Convert.ToDouble(txt_Offset_Z2_T4.Text);
                    Z2_5 = Convert.ToDouble(txt_Offset_Z2_T5.Text);
                    Z2_6 = Convert.ToDouble(txt_Offset_Z2_T6.Text);
                    Z2_7 = Convert.ToDouble(txt_Offset_Z2_T7.Text);
                    Z2_8 = Convert.ToDouble(txt_Offset_Z2_T8.Text);
                    Z2_9 = Convert.ToDouble(txt_Offset_Z2_T9.Text);
                    Z2_10 = Convert.ToDouble(txt_Offset_Z2_T10.Text);
                    Z2_11 = Convert.ToDouble(txt_Offset_Z2_T11.Text);
                    Z2_12 = Convert.ToDouble(txt_Offset_Z2_T12.Text);
                    Z2_13 = Convert.ToDouble(txt_Offset_Z2_T13.Text);
                    Z2_14 = Convert.ToDouble(txt_Offset_Z2_T14.Text);
                    Z2_15 = Convert.ToDouble(txt_Offset_Z2_T15.Text);
                    Z2_16 = Convert.ToDouble(txt_Offset_Z2_T16.Text);
                    Z2_17 = Convert.ToDouble(txt_Offset_Z2_T17.Text);
                    Z2_18 = Convert.ToDouble(txt_Offset_Z2_T18.Text);
                    Z2_19 = Convert.ToDouble(txt_Offset_Z2_T19.Text);
                    Z2_20 = Convert.ToDouble(txt_Offset_Z2_T20.Text);
                    #endregion
                    #region Check 3
                    using (var conn_ = GetConnectionSQLite())
                    {
                        conn_.Open();
                        string saveposSQL3 = string.Format("INSERT OR REPLACE INTO Offset_Z_Check_Bien_Dang (STT,Z1,Z2,Z3,Z4,Z5,Z6,Z7,Z8,Z9,Z10, Z11,Z12,Z13,Z14,Z15,Z16,Z17,Z18,Z19,Z20) " +
                        "VALUES (@STT,@Z1,@Z2,@Z3,@Z4,@Z5,@Z6,@Z7,@Z8,@Z9,@Z10, @Z11,@Z12,@Z13,@Z14,@Z15,@Z16,@Z17,@Z18,@Z19,@Z20)");
                        using (SQLiteCommand cmd = new SQLiteCommand(saveposSQL3, conn_))
                        {
                            cmd.Parameters.AddWithValue("@STT", 1);
                            cmd.Parameters.AddWithValue("@Z1", Convert.ToDouble(Array_Offset_Z3[0]));
                            cmd.Parameters.AddWithValue("@Z2", Convert.ToDouble(Array_Offset_Z3[1]));
                            cmd.Parameters.AddWithValue("@Z3", Convert.ToDouble(Array_Offset_Z3[2]));
                            cmd.Parameters.AddWithValue("@Z4", Convert.ToDouble(Array_Offset_Z3[3]));
                            cmd.Parameters.AddWithValue("@Z5", Convert.ToDouble(Array_Offset_Z3[4]));
                            cmd.Parameters.AddWithValue("@Z6", Convert.ToDouble(Array_Offset_Z3[5]));
                            cmd.Parameters.AddWithValue("@Z7", Convert.ToDouble(Array_Offset_Z3[6]));
                            cmd.Parameters.AddWithValue("@Z8", Convert.ToDouble(Array_Offset_Z3[7]));
                            cmd.Parameters.AddWithValue("@Z9", Convert.ToDouble(Array_Offset_Z3[8]));
                            cmd.Parameters.AddWithValue("@Z10", Convert.ToDouble(Array_Offset_Z3[9]));
                            cmd.Parameters.AddWithValue("@Z11", Convert.ToDouble(Array_Offset_Z3[10]));
                            cmd.Parameters.AddWithValue("@Z12", Convert.ToDouble(Array_Offset_Z3[11]));
                            cmd.Parameters.AddWithValue("@Z13", Convert.ToDouble(Array_Offset_Z3[12]));
                            cmd.Parameters.AddWithValue("@Z14", Convert.ToDouble(Array_Offset_Z3[13]));
                            cmd.Parameters.AddWithValue("@Z15", Convert.ToDouble(Array_Offset_Z3[14]));
                            cmd.Parameters.AddWithValue("@Z16", Convert.ToDouble(Array_Offset_Z3[15]));
                            cmd.Parameters.AddWithValue("@Z17", Convert.ToDouble(Array_Offset_Z3[16]));
                            cmd.Parameters.AddWithValue("@Z18", Convert.ToDouble(Array_Offset_Z3[17]));
                            cmd.Parameters.AddWithValue("@Z19", Convert.ToDouble(Array_Offset_Z3[18]));
                            cmd.Parameters.AddWithValue("@Z20", Convert.ToDouble(Array_Offset_Z3[19]));
                            cmd.ExecuteNonQuery();
                        }
                    }
                    Z3_1 = Convert.ToDouble(txt_Offset_Z3_T1.Text);
                    Z3_2 = Convert.ToDouble(txt_Offset_Z3_T2.Text);
                    Z3_3 = Convert.ToDouble(txt_Offset_Z3_T3.Text);
                    Z3_4 = Convert.ToDouble(txt_Offset_Z3_T4.Text);
                    Z3_5 = Convert.ToDouble(txt_Offset_Z3_T5.Text);
                    Z3_6 = Convert.ToDouble(txt_Offset_Z3_T6.Text);
                    Z3_7 = Convert.ToDouble(txt_Offset_Z3_T7.Text);
                    Z3_8 = Convert.ToDouble(txt_Offset_Z3_T8.Text);
                    Z3_9 = Convert.ToDouble(txt_Offset_Z3_T9.Text);
                    Z3_10 = Convert.ToDouble(txt_Offset_Z3_T10.Text);
                    Z3_11 = Convert.ToDouble(txt_Offset_Z3_T11.Text);
                    Z3_12 = Convert.ToDouble(txt_Offset_Z3_T12.Text);
                    Z3_13 = Convert.ToDouble(txt_Offset_Z3_T13.Text);
                    Z3_14 = Convert.ToDouble(txt_Offset_Z3_T14.Text);
                    Z3_15 = Convert.ToDouble(txt_Offset_Z3_T15.Text);
                    Z3_16 = Convert.ToDouble(txt_Offset_Z3_T16.Text);
                    Z3_17 = Convert.ToDouble(txt_Offset_Z3_T17.Text);
                    Z3_18 = Convert.ToDouble(txt_Offset_Z3_T18.Text);
                    Z3_19 = Convert.ToDouble(txt_Offset_Z3_T19.Text);
                    Z3_20 = Convert.ToDouble(txt_Offset_Z3_T20.Text);
                    #endregion
                    #region Check 4
                    using (var conn_ = GetConnectionSQLite())
                    {
                        conn_.Open();
                        string saveposSQL4 = string.Format("INSERT OR REPLACE INTO Offset_Z_Check_4 (STT,Z1,Z2,Z3,Z4,Z5,Z6,Z7,Z8,Z9,Z10, Z11,Z12,Z13,Z14,Z15,Z16,Z17,Z18,Z19,Z20) " +
                        "VALUES (@STT,@Z1,@Z2,@Z3,@Z4,@Z5,@Z6,@Z7,@Z8,@Z9,@Z10, @Z11,@Z12,@Z13,@Z14,@Z15,@Z16,@Z17,@Z18,@Z19,@Z20)");
                        using (SQLiteCommand cmd = new SQLiteCommand(saveposSQL4, conn_))
                        {
                            cmd.Parameters.AddWithValue("@STT", 1);
                            cmd.Parameters.AddWithValue("@Z1", Convert.ToDouble(Array_Offset_Z4[0]));
                            cmd.Parameters.AddWithValue("@Z2", Convert.ToDouble(Array_Offset_Z4[1]));
                            cmd.Parameters.AddWithValue("@Z3", Convert.ToDouble(Array_Offset_Z4[2]));
                            cmd.Parameters.AddWithValue("@Z4", Convert.ToDouble(Array_Offset_Z4[3]));
                            cmd.Parameters.AddWithValue("@Z5", Convert.ToDouble(Array_Offset_Z4[4]));
                            cmd.Parameters.AddWithValue("@Z6", Convert.ToDouble(Array_Offset_Z4[5]));
                            cmd.Parameters.AddWithValue("@Z7", Convert.ToDouble(Array_Offset_Z4[6]));
                            cmd.Parameters.AddWithValue("@Z8", Convert.ToDouble(Array_Offset_Z4[7]));
                            cmd.Parameters.AddWithValue("@Z9", Convert.ToDouble(Array_Offset_Z4[8]));
                            cmd.Parameters.AddWithValue("@Z10", Convert.ToDouble(Array_Offset_Z4[9]));
                            cmd.Parameters.AddWithValue("@Z11", Convert.ToDouble(Array_Offset_Z4[10]));
                            cmd.Parameters.AddWithValue("@Z12", Convert.ToDouble(Array_Offset_Z4[11]));
                            cmd.Parameters.AddWithValue("@Z13", Convert.ToDouble(Array_Offset_Z4[12]));
                            cmd.Parameters.AddWithValue("@Z14", Convert.ToDouble(Array_Offset_Z4[13]));
                            cmd.Parameters.AddWithValue("@Z15", Convert.ToDouble(Array_Offset_Z4[14]));
                            cmd.Parameters.AddWithValue("@Z16", Convert.ToDouble(Array_Offset_Z4[15]));
                            cmd.Parameters.AddWithValue("@Z17", Convert.ToDouble(Array_Offset_Z4[16]));
                            cmd.Parameters.AddWithValue("@Z18", Convert.ToDouble(Array_Offset_Z4[17]));
                            cmd.Parameters.AddWithValue("@Z19", Convert.ToDouble(Array_Offset_Z4[18]));
                            cmd.Parameters.AddWithValue("@Z20", Convert.ToDouble(Array_Offset_Z4[19]));
                            cmd.ExecuteNonQuery();
                        }
                    }
                    Z4_1 = Convert.ToDouble(txt_Offset_Z4_T1.Text);
                    Z4_2 = Convert.ToDouble(txt_Offset_Z4_T2.Text);
                    Z4_3 = Convert.ToDouble(txt_Offset_Z4_T3.Text);
                    Z4_4 = Convert.ToDouble(txt_Offset_Z4_T4.Text);
                    Z4_5 = Convert.ToDouble(txt_Offset_Z4_T5.Text);
                    Z4_6 = Convert.ToDouble(txt_Offset_Z4_T6.Text);
                    Z4_7 = Convert.ToDouble(txt_Offset_Z4_T7.Text);
                    Z4_8 = Convert.ToDouble(txt_Offset_Z4_T8.Text);
                    Z4_9 = Convert.ToDouble(txt_Offset_Z4_T9.Text);
                    Z4_10 = Convert.ToDouble(txt_Offset_Z4_T10.Text);
                    Z4_11 = Convert.ToDouble(txt_Offset_Z4_T11.Text);
                    Z4_12 = Convert.ToDouble(txt_Offset_Z4_T12.Text);
                    Z4_13 = Convert.ToDouble(txt_Offset_Z4_T13.Text);
                    Z4_14 = Convert.ToDouble(txt_Offset_Z4_T14.Text);
                    Z4_15 = Convert.ToDouble(txt_Offset_Z4_T15.Text);
                    Z4_16 = Convert.ToDouble(txt_Offset_Z4_T16.Text);
                    Z4_17 = Convert.ToDouble(txt_Offset_Z4_T17.Text);
                    Z4_18 = Convert.ToDouble(txt_Offset_Z4_T18.Text);
                    Z4_19 = Convert.ToDouble(txt_Offset_Z4_T19.Text);
                    Z4_20 = Convert.ToDouble(txt_Offset_Z4_T20.Text);
                    #endregion
                    #region write PLC
                    int[] data_pos_Z1 = new int[total_check_FPCB];
                    int[] data_pos_Z2 = new int[total_check_FPCB];
                    int[] data_pos_Z3 = new int[total_check_FPCB];
                    int[] data_pos_Z4 = new int[total_check_FPCB];
                    double[] Z_Check1 = { Z1_1, Z1_2, Z1_3, Z1_4, Z1_5, Z1_6, Z1_7, Z1_8, Z1_9, Z1_10, Z1_11, Z1_12, Z1_13, Z1_14, Z1_15, Z1_16, Z1_17, Z1_18, Z1_19, Z1_20 };
                    double[] Z_Check2 = { Z2_1, Z2_2, Z2_3, Z2_4, Z2_5, Z2_6, Z2_7, Z2_8, Z2_9, Z2_10, Z2_11, Z2_12, Z2_13, Z2_14, Z2_15, Z2_16, Z2_17, Z2_18, Z2_19, Z2_20 };
                    double[] Z_Check3 = { Z3_1, Z3_2, Z3_3, Z3_4, Z3_5, Z3_6, Z3_7, Z3_8, Z3_9, Z3_10, Z3_11, Z3_12, Z3_13, Z3_14, Z3_15, Z3_16, Z3_17, Z3_18, Z3_19, Z3_20 };
                    double[] Z_Check4 = { Z4_1, Z4_2, Z4_3, Z4_4, Z4_5, Z4_6, Z4_7, Z4_8, Z4_9, Z4_10, Z4_11, Z4_12, Z4_13, Z4_14, Z4_15, Z4_16, Z4_17, Z4_18, Z4_19, Z4_20 };
                    //POS CHUP 1-2-3
                    if (total_check_FPCB > 0)
                    {
                        for (int i = 0; i < total_check_FPCB; i++)
                        {
                            data_pos_Z1[i] = Convert.ToInt32(Convert.ToDouble(COR5541.Text) * 1000 + Z_Check1[i] * 1000);
                            PLC1.Write_Data_DWord_("D" + (10601 + i * 2 + +Memory_PLC.K2000).ToString(), data_pos_Z1[i]);//Z1
                            data_pos_Z2[i] = Convert.ToInt32(Convert.ToDouble(COR5543.Text) * 1000 + Z_Check2[i] * 1000);
                            PLC1.Write_Data_DWord_("D" + (10701 + i * 2 + Memory_PLC.K2000).ToString(), data_pos_Z2[i]);//z2
                            data_pos_Z3[i] = Convert.ToInt32(Convert.ToDouble(COR5545.Text) * 1000 + Z_Check3[i] * 1000);
                            PLC1.Write_Data_DWord_("D" + (10801 + i * 2 + Memory_PLC.K2000).ToString(), data_pos_Z3[i]);//z3
                            data_pos_Z4[i] = Convert.ToInt32(Convert.ToDouble(COR5547.Text) * 1000 + Z_Check4[i] * 1000);
                            PLC1.Write_Data_DWord_("D" + (13201 + i * 2 + Memory_PLC.K2000).ToString(), data_pos_Z4[i]);//z4
                        }
                        using (var conn_ = GetConnectionSQLite())
                        {
                            conn_.Open();
                            string DIR_UPDATE = string.Format("INSERT OR REPLACE INTO TABLE_OFFSET_Z_CHECK_VISION (STT, Z1,Z2,Z3,Z4) VALUES (@STT, @Z1, @Z2,@Z3,@Z4)");
                            for (int i = 0; i < total_check_FPCB; i++)
                            {
                                using (SQLiteCommand cmd = new SQLiteCommand(DIR_UPDATE, conn_))
                                {
                                    cmd.Parameters.AddWithValue("@STT", i + 1);
                                    cmd.Parameters.AddWithValue("@Z1", Convert.ToDouble(data_pos_Z1[i]) / 1000);
                                    cmd.Parameters.AddWithValue("@Z2", Convert.ToDouble(data_pos_Z2[i]) / 1000);
                                    cmd.Parameters.AddWithValue("@Z3", Convert.ToDouble(data_pos_Z3[i]) / 1000);
                                    cmd.Parameters.AddWithValue("@Z4", Convert.ToDouble(data_pos_Z4[i]) / 1000);
                                    cmd.ExecuteNonQuery();
                                    WaitFormHelper.TransferInfo("Transfer : Data Parameter Vision complete...");
                                }
                            }
                        }
                    }

                    #endregion
                }

            }
            catch { WaitFormHelper.TransferInfo("Transfer : Data Parameter Vision Error..."); }

        }
        //***************************************
        private void DeleteModel(string modelName)
        {
            try
            {
                using (var Conn_ = GetConnectionSQLite())
                {
                    Conn_.Open();

                    string sql = "DELETE FROM Model WHERE Name = @Name";

                    using (var cmd = new SQLiteCommand(sql, Conn_))
                    {
                        cmd.Parameters.AddWithValue("@Name", modelName);

                        cmd.ExecuteNonQuery();
                    }
                }
                // 2. DELETE JSON
                if (File.Exists("ModelManager.json"))
                {
                    var json = File.ReadAllText("ModelManager.json");

                    var dict = JsonConvert.DeserializeObject<Dictionary<string, JObject>>(json)
                               ?? new Dictionary<string, JObject>();

                    if (dict.ContainsKey(modelName))
                    {
                        dict.Remove(modelName);
                    }
                    File.WriteAllText(
                        "ModelManager.json",
                        JsonConvert.SerializeObject(dict, Formatting.Indented));
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void ReloadModel()
        {
            ListModelName.Clear();

            using (var Conn_ = GetConnectionSQLite())
            {
                Conn_.Open();

                using (var cmd = new SQLiteCommand(
                    "SELECT Name FROM Model ORDER BY Name", Conn_))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        ListModelName.Add(reader.GetString(0));
                    }
                }
            }

            ListModelName.Add("➕ Add New Model...");
            Combox_Model.DataSource = null;
            Combox_Model.DataSource = ListModelName;
            Combox_Model.SelectedItem = Global.ModelName_Server;
        }

        private async void btnSaveModel_Click(object sender, EventArgs e)
        {
            if (Global.Start_Start == 1) return;
            DialogResult rs = MessageBox.Show($"Save model [{Combox_Model.Text}] ?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (rs != DialogResult.Yes)
                return;
            ModelHelper modelhelper = new ModelHelper();
            parameterModel.Name = Combox_Model.Text;
            await WaitFormHelper.TransferdataRun(1, true);
            await Task.Run(() =>
            {
                import_paraMain();
                WaitFormHelper.TransferLoading(10);
                import_paraRobotTeaching();
                WaitFormHelper.TransferLoading(20);
                import_paraRobotSetup();
                WaitFormHelper.TransferLoading(20);
                import_paraRobotSetupTray();
                WaitFormHelper.TransferLoading(20);
                import_paraPLC_Loading();
                WaitFormHelper.TransferLoading(10);
                import_paraPLC_Vision();
                WaitFormHelper.TransferLoading(20);
            });

            if (parameterModel.Name == "" || parameterModel.Name == null) return;
            modelhelper.Save(parameterModel.Name, parameterModel);
            // Message_Box_OK("Save parameter model complete!", "Model Save");
            WaitFormHelper.TransferInfo("*********");
            WaitFormHelper.TransferInfo("Finish");
            WaitFormHelper.Transferdatastop(0);
        }
        private async void btnLoadModel_Click(object sender, EventArgs e)
        {
            if (Global.Start_Start == 1)
            {
                Message_Box_Error("Máy đang Run không được load model", "Load model");
                return;
            }
            ModelHelper modelhelper = new ModelHelper();
            parameterModel.Name = Combox_Model.Text;
            DialogResult rs = MessageBox.Show($"Load model [{Combox_Model.Text}] ?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (rs != DialogResult.Yes)
                return;
            if (parameterModel.Name == "" || parameterModel.Name == null) return;
            Global.ModelName = parameterModel.Name;
            Save_WriteAllText(Global.ModelName, "Model.txt");
            uiLabel_Name_Machine.Text = Global.ModelName;
            txt_model_input.Text = Global.ModelName;
            Global.ModelName_Server = Global.ModelName;
            parameterModel = modelhelper.Load(parameterModel.Name);
            if (parameterModel == null)
            {
                Message_Box_OK("Không tìm thấy model", "Load Model");
                return;
            }
            await WaitFormHelper.TransferdataRun(1, false);
            await Task.Run(() =>
            {
                WaitFormHelper.TransferLoading(0);
                Export_paraMain();
                WaitFormHelper.TransferLoading(5);
                Export_paraRobotTeaching();
                WaitFormHelper.TransferLoading(5);
                Export_paraRobotSetup();
                WaitFormHelper.TransferLoading(5);
                Export_paraRobotSetupTray();
                WaitFormHelper.TransferLoading(5);
                Export_paraPLC_Loading();
                WaitFormHelper.TransferLoading(5);
                Export_paraPLC_Vision();
                WaitFormHelper.TransferLoading(5);
                TransferMain();
                WaitFormHelper.TransferLoading(5);
                TransferRobotTeaching();
                WaitFormHelper.TransferLoading(5);
                TransferRobotSetup();
                WaitFormHelper.TransferLoading(5);
                TransferRobotSetupTray();
                WaitFormHelper.TransferLoading(30);
                TransferPLCLoading();
                WaitFormHelper.TransferLoading(15);
                TransferPLCVision();
                WaitFormHelper.TransferLoading(20);
            });
            WaitFormHelper.TransferInfo("***********");
            WaitFormHelper.TransferInfo("Finish");
            WaitFormHelper.Transferdatastop(0);
        }
        private void btnDeleteModel_Click(object sender, EventArgs e)
        {
            string modelName = Combox_Model.SelectedItem?.ToString();

            if (string.IsNullOrEmpty(modelName))
                return;

            if (modelName == "➕ Add New Model...")
                return;
            if (modelName == Global.ModelName_Server)
            {
                MessageBox.Show("Model is running!");
                return;
            }
            DialogResult rs = MessageBox.Show(
                $"Delete model [{modelName}] ?",
                "Confirm",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (rs != DialogResult.Yes)
                return;

            DeleteModel(modelName);

            ReloadModel();
            Message_Box_OK("Delete thành công", "Delete Model");
        }

        #endregion
    }
}
