
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;


namespace Main_Base
{
    public class Function_Robot
    {
        public bool CMD_Pick = false;
        public bool CMD_Pick_Press = false;
        public bool CMD_Input = false;
        public bool CMD_Check_Marking = false;
        public bool CMD_Pick_Output = false;
        public bool CMD_Place_Tray = false;
        public bool Flag_FPCB_After_Pick_Press_RB = false;
        public bool Flag_FPCB_After_Pick_Output_RB = false;
        public bool Flag_FPCB_All_NG = false;
        public bool Brake_While = false;
        public bool Security_Check_Camtop = false;
        public int Flag_Cam1, Flag_Cam2,Check_Master;
        public bool Flag_Wait_Pick_Press = false;
        ////Wait Robot
        ////
        public void Wait_for_stop_motion(int robot)
        {
            while (SDKHrobot.HRobot.get_motion_state(robot) != 1)
            {
                Thread.Sleep(50);
            }
            //Thread.Sleep(100);
        }
        public void Wait_for_stop_motion_2(int robot)
        {
            while (SDKHrobot.HRobot.get_motion_state(robot) != 1)
            {
                Thread.Sleep(50);
            }
            //Thread.Sleep(50);
        }
        public void Wait_For_Digital_Input_On(int robot, int DI_id)
        {

            while (SDKHrobot.HRobot.get_digital_input(robot, DI_id) != 1)
            {
                Thread.Sleep(50);
            }
            Thread.Sleep(80);
        }
        public void Wait_For_Digital_Input_Off(int robot, int DI_id)
        {
            while (SDKHrobot.HRobot.get_digital_input(robot, DI_id) != 0)
            {
                Thread.Sleep(20);
            }
            Thread.Sleep(120);
        }
        public void Wait_For_Digital_Output_On(int robot, int DO_id)
        {
            while (SDKHrobot.HRobot.get_digital_output(robot, DO_id) == 1)
            {
                Thread.Sleep(20);
            }
            //Thread.Sleep(80);
        }
        public void Wait_For_Digital_Output_Off(int robot, int DO_id)
        {
            while (SDKHrobot.HRobot.get_digital_output(robot, DO_id) != 1)
            {
                Thread.Sleep(20);
            }
            Thread.Sleep(120);
        }
        // Monitor DI Robot
        public bool Monitor_DI(int robot, int DI_id)
        {
            bool digital_Di = false;
            int result = SDKHrobot.HRobot.get_digital_input(robot, DI_id);
            if (result == 1)
            {
                digital_Di = true;
            }
            else
            {
                digital_Di = false;
            }
            return digital_Di;
        }
        public bool Monitor_DO(int robot, int Do_id)
        {
            bool digital_Do = false;
            try
            {
                int result = SDKHrobot.HRobot.get_digital_output(robot, Do_id);
                if (result == 1)
                {
                    digital_Do = true;
                }
                else
                {
                    digital_Do = false;
                }
            }
            catch { }
            return digital_Do;
        }
        public void CMD_pick()
        {
            while (CMD_Pick != true)
            {
                Thread.Sleep(50);
            }
            //  Thread.Sleep(100);
        }
        public void Wait_For_Flag_Cam()
        {
            while (Flag_Cam1 != 1 || Flag_Cam2 != 1)
            {
                Thread.Sleep(10);
            }
            //  Thread.Sleep(100);
        }
        public void Wait_For_CheckMaster()
        {
            while (Check_Master != 1)
            {
                Thread.Sleep(50);
            }
            //  Thread.Sleep(100);
        }
        public void Wait_Cylinder_Up(int[] DI_Id)
        {
            while (DI_Id[0] != 1 || DI_Id[1] != 1 || DI_Id[2] != 1 || DI_Id[3] != 1)
            {
                Thread.Sleep(30);
            }
            Thread.Sleep(20);
        }
    }
}
