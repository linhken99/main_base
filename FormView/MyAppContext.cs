using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using Microsoft.WindowsAPICodePack.Taskbar;
using Sunny.UI;

namespace Main_Base
{
    public class MyAppContext : ApplicationContext
    {
        private Screen splash;
        private Main mainForm;
        //private StatusDisplay status_display = new StatusDisplay();
        public MyAppContext()
        {
            UILocalizeHelper.SetEN();
            splash = new Screen();
            splash.Show();
            InitAsync();
        }
        private async void InitAsync()
        {
            //splash = new Screen();
            //splash.Show();
            splash.update_label("Đang tải dữ liệu...");
            mainForm = new Main();
            await Task.Run(() =>
            {
                //load data form chính
                StatusDisplay.Instance.IsFormOpen = true;
                //TaskbarManager.Instance.SetProgressValue(100, 100);
                TaskbarManager.Instance.SetProgressState(TaskbarProgressBarState.Indeterminate);
                //mainForm.Change_Label_INPUT();
                //mainForm.Change_Label_Output();
                //mainForm.Change_Sensor();
                //mainForm.Change_TabPage_IO();
                splash.update_label("Đang tải dữ liệu...");
                mainForm.ConnectSQLite();
                mainForm.Lock_Screen();
                splash.update_label("Đang tải TCP/IP...");
                mainForm.load_IP();
                mainForm.Check_Double_Application();
                splash.update_label("Connect Robot...");
                mainForm.Connect_Robot();
                splash.update_label("Data Vision...");
                mainForm.load_data_offset_Check_Vision_Bottom();
                splash.update_label("Đang tải dữ liệu Robot...");
                mainForm.load_Offset();
                mainForm.load_Parameter();
                mainForm.Config_LoadDataRB_DataGridView();
                mainForm.load_pos_RB();
                splash.update_label("Đang tải dữ liệu chế độ chạy...");
                mainForm.load_mode();
                mainForm.load_Password();
                splash.update_label("Đang tải dữ liệu Robot...");
                mainForm.Load_Speed_Robot();
                mainForm.load_TactTime();              
                mainForm.Search_Min_Max_CorRB();
                splash.update_label("Connect PLC...");
                mainForm.Connect_PLC();
                if (Global.IsConnectPLC == true)
                {
                    string info_PLC = mainForm.Info_PLC();
                    mainForm.display_box(info_PLC);
                    mainForm.display_box("Connect PLC Succesful");
                    splash.update_label("Đang tải dữ liệu PLC...");
                    mainForm.load_data_PLC();
                    mainForm.Read_data_row_column_tray();
                }
                else
                {
                    mainForm.display_box("Disconnect PLC");
                }
                mainForm.DisConSQLite();
                mainForm.Load_data_machine_server();
            });
            splash.Close();
            //Thread monitor_ = new Thread(mainForm.monitor_all);
            //monitor_.Start();
            mainForm.Visible = false;
            mainForm.FormClosed += (s, e) => Application.Exit();
            mainForm.Show();
            Application.DoEvents();
            mainForm.Visible = true;
            TaskbarManager.Instance.SetProgressState(TaskbarProgressBarState.NoProgress);
        }
    }
}
