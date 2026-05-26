using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Main_Base.Class
{
   public class AlarmInfo
    {
        public string AlarmId { get; set; }
        public DateTime StartTime { get; set; }
        public ListViewItem Item { get; set; }
    }
}
