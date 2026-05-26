using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Data.Sql;
using Main_Base.Class;

namespace Main_Base.IT
{

    public class AlarmManager
    {
        private BlockingCollection<(string alarmId, DateTime start, DateTime? end)> _queue
            = new BlockingCollection<(string, DateTime, DateTime?)>();

        private CancellationTokenSource _cts = new CancellationTokenSource();
        private Dictionary<string, DateTime> _alarmStartDict
       = new Dictionary<string, DateTime>();
        private readonly object _lock = new object();
        public AlarmManager()
        {
            Task.Run(ProcessQueue);
        }

        // gọi khi có alarm ON
        //public void AlarmOn(string alarmId)
        //{
        //    var now = DateTime.Now;

        //    lock (_lock)
        //    {
        //        if (!_alarmStartDict.ContainsKey(alarmId))
        //        {
        //            _alarmStartDict[alarmId] = now;
        //            _queue.Add((alarmId, now, null));
        //        }
        //    }
        //}
        public void AlarmOn(string alarmId)
        {
            var now = DateTime.Now;

            lock (_lock)
            {
                if (!_alarmStartDict.ContainsKey(alarmId))
                {
                    _alarmStartDict[alarmId] = now;
                }
            }
        }

        // gọi khi alarm OFF
        public void AlarmOff(string alarmId)
        {
            var now = DateTime.Now;

            lock (_lock)
            {
                if (_alarmStartDict.TryGetValue(alarmId, out DateTime start))
                {
                    _queue.Add((alarmId, start, now));
                    _alarmStartDict.Remove(alarmId);
                }
            }
        }

        // worker xử lý queue
        private async Task ProcessQueue()
        {
            foreach (var item in _queue.GetConsumingEnumerable(_cts.Token))
            {
                System.Diagnostics.Debug.WriteLine ("📥 Dequeu");
                await SendWithRetry(item.alarmId, item.start, item.end);
            }
        }

        // retry khi lỗi
        private async Task SendWithRetry(string alarmId, DateTime start, DateTime? end)
        {
            int retry = 3;

            for (int i = 0; i < retry; i++)
            {
                try
                {
                    await SendAlarmToServerAsync(alarmId, start, end);
                    break;
                }
                catch
                {
                    await Task.Delay(1000);
                }
            }
        }

        // gửi SQL
        private async Task SendAlarmToServerAsync(string alarmId, DateTime start_time, DateTime? end_time)
        {          
            try
            {
                List<E_ALARM_HISTORY> list = new List<E_ALARM_HISTORY>()
                {
                    new E_ALARM_HISTORY
                    {
                        EQUIPMENT_ID=Global.MACHINE_NAME,
                        TYPE = "Error",
                        MSG=alarmId,
                        START_TIME =start_time,
                        END_TIME = end_time ?? DateTime.Now,
                        INSERT_DATE =start_time,
                        INSERT_USER="Ai-Vision",
                        DEL_FLAG="N",
                    },
                };
               await IT_SQL_Server_Helper.Instance.InsertListIgnoreDuplicate("E_ALARM_HISTORY", list);
            }
            catch 
            {

            }
        }

        // stop nếu cần
        public void Stop()
        {
            _cts.Cancel();
            _queue.CompleteAdding();
        }
    }
}

