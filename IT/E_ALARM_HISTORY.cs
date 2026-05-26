using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Main_Base.IT
{
    public class E_ALARM_HISTORY
    {
        public string EQUIPMENT_ID { get; set; }
        public string TYPE { get; set; }
        public DateTime START_TIME { get; set; }
        public DateTime END_TIME { get; set; }
        public string MSG { get; set; }
        public DateTime INSERT_DATE { get; set; }
        public string INSERT_USER { get; set; }
        public string DEL_FLAG { get; set; }
    }
}
