using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Main_Base.IT
{
    public class E_RUN_TIME
    {
        public string EQUIPMENT_ID { get; set; }
        public DateTime START_TIME { get; set; }
        public DateTime END_TIME { get; set; }
        public float RUN_SECONDS { get; set; }
        public int PRODUCTION_OK { get; set; }
        public int PRODUCTION_NG { get; set; }
        public int PRODUCTION_NA { get; set; }
        public int TOTAL_PRODUCTION { get; set; }
        public int PRODUCTION { get; set; }
        public string INSERT_USER { get; set; }
        public string DEL_FLAG { get; set; }
        public string MODEL_NAME { get; set; }
        public DateTime INSERT_DATE { get; set; }

    }
}
