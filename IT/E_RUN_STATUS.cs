using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Main_Base.IT
{
    public class E_RUN_STATUS
    {
        public string EQUIPMENT_ID { get; set; }
        public string RUNNING { get; set; } // trạng thái hoạt động của máy
        public string MODEL { get; set; }
        public int QTY { get; set; }
        public int TOTAL_OK { get; set; }
        public int TOTAL_NG { get; set; }
        public int TOTAL_NA { get; set; }
        public DateTime UPDATE_DATE { get; set; }
        public string UPDATE_USER { get; set; }

    }
}
