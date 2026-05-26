using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Main_Base.Model
{
    public class ParameterRobot_Setup
    {
        //OffsetTool
        public double Offset_X { get; set; }
        //RunAuto
        public double Number_Tool { get; set; }
        //*Place Tray satefy
        public double X_Upper1 { get; set; }
        public double X_Lower1 { get; set; }
        public double Y_Upper1 { get; set; }
        public double Y_Lower1 { get; set; }
        public double Z_Satefy1 { get; set; }
        //*Check camtop satefy
        public double X_Upper2 { get; set; }
        public double X_Lower2 { get; set; }
        public double Y_Upper2 { get; set; }
        public double Y_Lower2 { get; set; }
        public double Z_Satefy2 { get; set; }
        //data sheet FPCB     
        public int NumberFPCB { get; set; }
        public double OffsetX_FPCB1 { get; set; }
        public double OffsetY_FPCB1 { get; set; }
        public double OffsetX_BlockOrTray { get; set; }
        public double OffsetY_Tray { get; set; }
        //data check Camtop
        public int NumberTool_Checkk { get; set; }
        public double OffsetX_FPCB2 { get; set; }
        public double OffsetY_FPCB2 { get; set; }
        //checkbox
        public int MutiBlow { get; set; }
        public int SingleBlow { get; set; }
        public int MutiView { get; set; }
        public int SingleView { get; set; }
        public int PowerBlow1 { get; set; }
        public int PowerBlow2 { get; set; }
    }
}
