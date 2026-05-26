using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Main_Base.Class
{
    public static class VaribalePLC
    {
        // G1
        public static int[] Group1 = new int[100];
        public static int[] Group12 = new int[100];
        public static int[] Group1_Dtext = new int[100];
        public static int[] Group1_text = new int[100];
        //              
        // G2
        public static int[] Group2 = new int[100];
        public static int[] Group22 = new int[100];
        public static int[] Group2_Dtext = new int[100];
        public static int[] Group2_text = new int[100];
        // G3
        public static int[] Group3 = new int[100];
        public static int[] Group32 = new int[100];
        public static int[] Group3_Dtext = new int[100];
        public static int[] Group3_text = new int[100];          
        //G5
        public static int[] M_Input1 = new int[500];
        public static int[] M_Input2 = new int[500];
        public static int[] M_Input3 = new int[500];
        //G6
        public static int[] M_Output1 = new int[500];
        public static int[] M_Output2 = new int[500];
        public static int[] M_Output3 = new int[500];
        //
        public static int[] Result_Cam_Bot_Check = new int[500];
        public static int[] Result_Cam_Top_Check = new int[500];
        //Setting Camera
        public static int[] CamM1 = new int[100];
        public static int[] CamM2 = new int[100];
        public static int[] CamWord = new int[100];
        public static int[] CamDWord = new int[100];
    }
}
