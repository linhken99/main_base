using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Main_Base
{
    public static class Global
    {
        #region Positon Robot
        public static double[] Pick_Press { get; set; } = new double[6];//1
        public static double[] Z_Pick_Press { get; set; } = new double[6]; //2
        public static double[] Check_Marking_Start { get; set; } = new double[6]; //3
        public static double[] Check_Tape_Start { get; set; } = new double[6];//4
        public static double[] Place_Tray_1 { get; set; } = new double[6];//5
        public static double[] Place_Tray_2 { get; set; } = new double[6];//6
        public static double[] Place_Tray_3 { get; set; } = new double[6];//7
        public static double[] Place_Tray_4 { get; set; } = new double[6];//8
        public static double[] Input_FPCB { get; set; } = new double[6];//9
        public static double[] Z_Input_FPCB { get; set; } = new double[6];//10
        public static double[] Pick_FPCB_Output_1 { get; set; } = new double[6];//11
        public static double[] Pick_FPCB_Output_2 { get; set; } = new double[6];//12
        public static double[] Z_Pick_FPCB_Output_1 { get; set; } = new double[6];//13
        public static double[] Z_Pick_FPCB_Output_2 { get; set; } = new double[6];//14
        public static double[] Pos_NG { get; set; } = new double[6];//15
        public static double[] Z_Pos_NG { get; set; } = new double[6];//16
        //
        public static double[] Ready_Pick_Press_1 { get; set; } = new double[6];//17
        public static double[] Ready_Pick_Press_2 { get; set; } = new double[6];//18
        //
        public static double[] Ready_Check_Camtop_1 { get; set; } = new double[6];//19
        public static double[] Ready_Check_Camtop_2 { get; set; } = new double[6];//20
        //
        public static double[] Ready_Inputput_1 { get; set; } = new double[6];//21
        public static double[] Ready_Inputput_2 { get; set; } = new double[6];//22
        //     
        public static double[] Ready_Pick_Output_1 { get; set; } = new double[6];//23
        public static double[] Ready_Pick_Output_2 { get; set; } = new double[6];//24
        //     
        public static double[] Ready_Place_Tray_1 { get; set; } = new double[6];//25
        public static double[] Ready_Place_Tray_2 { get; set; } = new double[6];//26
        public static double[] Ready_Place_Tray_3 { get; set; } = new double[6];//27             
        //    
        public static double[] Ready_Place_NG_1 { get; set; } = new double[6];//28
        public static double[] Ready_Place_NG_2 { get; set; } = new double[6];//29
        //    
        public static double[] Ready_Arc_1 { get; set; } = new double[6]; //30
        public static double[] Ready_Arc_2 { get; set; } = new double[6]; //31
        //    
        public static double[] Ready_Rotation_1 { get; set; } = new double[6];//32
        public static double[] Ready_Rotation_2 { get; set; } = new double[6];//33
        public static double[] Homee { get; set; } = new double[6];//34
        #endregion
        #region Satefy
        public static double Offset_Z { get; set; }
        public static double Offset_X { get; set; }
        public static double Offset_Y { get; set; }
        public static double Z_antoan { get; set; }
        public static double A2_antoan { get; set; }
        public static double SP_Home { get; set; }
        public static double Z_Place_Satefy { get; set; }
        public static double Y_Place_Satefy_U { get; set; }
        public static double Y_Place_Satefy_L { get; set; }
        public static double X_Place_Satefy_U { get; set; }
        public static double X_Place_Satefy_L { get; set; }
        public static double X_Camtop_Satefy_U { get; set; }
        public static double X_Camtop_Satefy_L { get; set; }
        public static double Y_Camtop_Satefy_U { get; set; }
        public static double Y_Camtop_Satefy_L { get; set; }
        public static double Z_Camtop_Satefy { get; set; }
        #endregion
        #region Setting FPCB
        public static double Number_Block { get; set; }
        public static double Offset_Block_X { get; set; }
        public static double Offset_Block_Y { get; set; }
        public static double Number_FPCB { get; set; }
        public static double Offset_FPCB_X { get; set; }
        public static double Offset_FPCB_Y { get; set; }
        public static double Offset_Block_X_marking { get; set; }
        public static double Offset_Block_Y_Marking { get; set; }
        public static double Number_FPCB_Marking { get; set; }
        public static double Offset_FPCB_X_Marking { get; set; }
        public static double Offset_FPCB_Y_Marking { get; set; }
        public static double Total_Check_Marking { get; set; }
        #endregion
        #region Setup Robot
        public static int SP_auto_RB { get; set; }
        public static int SP_snap { get; set; }
        public static int delay_hut { get; set; }
        public static int delay_tha { get; set; }
        public static int delay_up { get; set; }
        public static int delay_down { get; set; }
        public static int Number_Tool { get; set; }
        public static int Mode_run_auto { get; set; }
        public static int ACC_RB { get; set; }
        public static int SP_Wait_Pick { get; set; }
        public static int TT_CamB { get; set; }
        public static int SL_OK { get; set; }
        public static int SL_NG { get; set; }
        public static int SL_Total_Input { get; set; }
        public static int Select_row_tray_cam_ { get; set; }
        public static int Row_tray { get; set; }
        public static int Column_tray { get; set; }
        public static int Row_tray_matrix1 { get; set; }
        public static int Column_tray_matrix1 { get; set; }
        public static int Row_tray_matrix2 { get; set; }
        public static int Column_tray_matrix2 { get; set; }
        public static int Row_tray_matrix3 { get; set; }
        public static int Column_tray_matrix3 { get; set; }
        public static int combox_mode { get; set; }
        public static double[] Get_Curent_XYZ_RB { get; set; } = new double[6];
        public static double[] Get_Curent_Joint_RB { get; set; } = new double[6];
        public static bool GoX = false;
        public static bool GoY = false;
        public static bool GoZ = false;
        public static bool GoC = false;
        public static bool Security_Place = false;

        #endregion 
        #region Var Run auto
        public static int Start_Start { get; set; }
        public static bool CMD_Scan_Flag { get; set; }
        public static int[] result_Cam_Bot { get; set; } = new int[40];
        public static bool Home_All { get; set; }
        public static bool cancel_alarm { get; set; }
        public static int home_start { get; set; }
        public static int auto_ { get; set; }

        #endregion
        #region PLC Connect
        public static bool IsConnectPLC = false;
        #endregion
        #region TactTime
        public static double var_tact_time { get; set; }
        public static int Counter { get; set; }
        public static double[] CycleTime_arr { get; set; } = new double[10];
        #endregion
        #region Password
        public static string Password_New { get; set; }
        public static string Password_Old { get; set; }
        #endregion
        #region...
        public static string Chong_hang { get; set; }
        public static int[] Warring_ListView { get; set; } = new int[20];
        public static int Row_Jig_input { get; set; }
        public static int Column_Jig_input { get; set; }
        public static DateTime Timer_rsCounter { get; set; }
        public static bool HasWrittenToday = false;
        public static string ModelName { get; set; }
        public static string[] ListModelName { get; set; } = new string[1000];
        public static int Current_Lot { get; set; }
        public static int Number_Tray_output { get; set; }
        public static double Min_RB_PlaceTray_X { get; set; }
        public static double Max_RB_PlaceTray_X { get; set; }
        public static double Min_RB_PlaceTray_Y { get; set; }
        public static double Max_RB_PlaceTray_Y { get; set; }
        public static double Min_RB_PlaceTray_Z { get; set; }
        public static double Min_RB_CheckCamtop_X { get; set; }
        public static double Max_RB_CheckCamtop_X { get; set; }
        public static double Min_RB_CheckCamtop_Y { get; set; }
        public static double Max_RB_CheckCamtop_Y { get; set; }
        public static double Min_RB_CheckCamtop_Z { get; set; }
        public static string ModelName_Server { get; set; }
        public static int[] Status_Machine_Server { get; set; } = new int[5];
        public static int[] dataMonitor { get; set; } = new int[6];
        public static bool Flag_ResetTray { get; set; }

        #endregion
        #region E_MACHINE
        public static int SEQ_ID { get; set; }
        public static string MACHINE_NAME { get; set; }
        public static string GROUP_NO { get; set; }
        public static string LINE_NO { get; set; }
        public static int TIME_UPDATE { get; set; }
        public static int TIME_UPDATE_Qty { get; set; }
        public static int TIME_DAY1_hh { get; set; }
        public static int TIME_DAY1_mm { get; set; }
        public static int TIME_DAY1_ss { get; set; }
        public static int TIME_DAY2_hh { get; set; }
        public static int TIME_DAY2_mm { get; set; }
        public static int TIME_DAY2_ss { get; set; }
        public static int TIME_NIGHT1_hh { get; set; }
        public static int TIME_NIGHT1_mm { get; set; }
        public static int TIME_NIGHT1_ss { get; set; }
        public static int TIME_NIGHT2_hh { get; set; }
        public static int TIME_NIGHT2_mm { get; set; }
        public static int TIME_NIGHT2_ss { get; set; }
        public static int RUN_MES { get; set; }
        public static int UPH { get; set; }
        public static int Total_NA { get; set; }
        public static bool Flag_Run_Time = false;
        public static double Time_Pick { get; set; }
        public static double Time_Vision { get; set; }
        public static double Time_Place { get; set; }
        public static double Time_NA { get; set; }
        public static double Total_Time_RunTime { get; set; }
        public static int Production_OK { get; set; }
        public static int Production_NG { get; set; }
        public static int Production_NA { get; set; }
        public static int Select_cycle_time { get; set; }
        public static DateTime Start_Time { get; set; }
        public static DateTime End_Time { get; set; }
        #endregion
    }
}
