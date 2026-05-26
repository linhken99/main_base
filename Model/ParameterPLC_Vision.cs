using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Main_Base.Model
{
   public class ParameterPLC_Vision
    {
        //=================================================View Vision PLC Control=================================================
        //* X
        public int CorX_Pick1 { get; set; }
        public int CorX_Pick2 { get; set; }
        public int CorX_Check1 { get; set; }
        public int CorX_Check2 { get; set; }
        public int CorX_Check3 { get; set; }
        public int CorX_Check4 { get; set; }
        public int CorX_Output { get; set; }
        //* Y
        public int CorY_Pick1 { get; set; }
        public int CorY_Pick2 { get; set; }
        public int CorY_Check1 { get; set; }
        public int CorY_Check2 { get; set; }
        public int CorY_Check3 { get; set; }
        public int CorY_Check4 { get; set; }
        public int CorY_Output { get; set; }
        //* Z
        public int CorZ_Pick1 { get; set; }
        public int CorZ_Pick2 { get; set; }
        public int CorZ_Check1 { get; set; }
        public int CorZ_Check2 { get; set; }
        public int CorZ_Check3 { get; set; }
        public int CorZ_Check4 { get; set; }
        public int CorZ_Output { get; set; }
        //* R
        public int CorR_Pick1 { get; set; }
        public int CorR_Pick2 { get; set; }
        public int CorR_Check1 { get; set; }
        public int CorR_Output { get; set; }
        //*CheckBox
        public int Not_Check1 { get; set; }
        public int Not_Check2 { get; set; }
        public int Not_Check3 { get; set; }
        public int Not_Check4 { get; set; }
        public int Data1_1 { get; set; }
        public int Data2_1 { get; set; }
        public int NotUseLamp { get; set; }
        public int NotUseVacuum { get; set; }
        //Parameter Vision
        public int Number_Block { get; set; }
        public double OffsetX_block { get; set; }
        public double OffsetY_block { get; set; }
        public int Number_FPCB { get; set; }
        public double OffsetX_FPCB { get; set; }
        public double OffsetY_FPCB { get; set; }
        public int Total_Check { get; set; }
        public int Number_Data { get; set; }
    }
}
