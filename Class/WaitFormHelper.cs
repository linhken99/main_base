using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Forms;
using Main_Base.FormView;

namespace Main_Base
{
    public static class WaitFormHelper
    {        
        private static Wait wait;
        private static ScreenConnect ScreenConnect;
        private static TransferData transferdata;
        public static async Task Show()
        {           
                if (wait == null || wait.IsDisposed)
                {
                    wait = new Wait();
                    wait.StartPosition = FormStartPosition.CenterScreen;
                    wait.TopMost = true;
                    wait.Show();
                    Application.DoEvents(); // Đảm bảo UI vẽ xong
                }
            await Task.Delay(50);
        }      
        public static void Close()
        {
            if (wait != null && !wait.IsDisposed)
            {
                wait.Invoke(new Action(() =>
                {
                    wait.Close();
                    wait = null;
                }));
            }
        }
        public static async Task ShowCam()
        {
            if (ScreenConnect == null || ScreenConnect.IsDisposed)
            {
                ScreenConnect = new ScreenConnect();
                ScreenConnect.StartPosition = FormStartPosition.CenterScreen;
                ScreenConnect.TopMost = true;
                ScreenConnect.Show();
                Application.DoEvents(); // Đảm bảo UI vẽ xong
            }
            await Task.Delay(50);
        }
        public static void CloseCam()
        {
            if (ScreenConnect != null && !ScreenConnect.IsDisposed)
            {
                ScreenConnect.Invoke(new Action(() =>
                {
                    ScreenConnect.Close();
                    ScreenConnect = null;
                }));
            }
        }
        public static async Task TransferdataRun(int value, bool FlowDirection)
        {
            try
            {
                if (transferdata == null || transferdata.IsDisposed)
                {
                    transferdata = new TransferData();
                    transferdata.Run = value == 1 ? true : false;
                    transferdata.FlowDirection = FlowDirection == true ? true : false;
                    transferdata.StartPosition = FormStartPosition.CenterScreen;
                    transferdata.TopMost = true;
                    if (FlowDirection == true)
                    {
                        transferdata.LabelTransferWrite();
                    }
                    else
                    {
                        transferdata.LabelTransferRead();
                    }
                    transferdata.Show();
                    _ = Task.Run(async () =>
                    {
                        await transferdata.Transfer();
                    });
                    //Application.DoEvents();
                }
                await Task.Delay(10);
            }
            catch { }
        }
        public static void Transferdatastop(int value)
        {
            transferdata.Run = value == 1 ? true : false;
            transferdata.StopTransfer();
            //if (transferdata != null && !transferdata.IsDisposed)
            //{
            //    transferdata.Invoke(new Action(() =>
            //    {
            //        transferdata.Close();
            //        transferdata = null;
            //    }));
            //}
        }
        public static void TransferInfo(string str)
        {
            transferdata.InfoTransfer(str);
        }
        public static void TransferLoading(int value)
        {
            transferdata.Valueload += value;
        }
    }
}
