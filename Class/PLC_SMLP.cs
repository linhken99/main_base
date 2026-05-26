using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Threading;
using System.Net.Sockets;
using System.Net;
using System.Windows.Forms;
using ActUtlTypeLib;

namespace Main_Base
{
    public class PLC_SMLP
    {

        public ActUtlType PLC_MX = new ActUtlType();
        public string IP;
        public int Port;
        public int Err_message_;
        // public Socket socket;
        public bool connect = false;
        public bool connect_2 = false;
        public bool Close_Alarm = false;
        public string MessErr;
        private object lock_R = new object();
        private object lock_W = new object();
       
        public bool IsConnected
        {
            get
            {
                if (connect == false)
                {
                    return false;
                }
                else
                {
                    return connect = true;
                }

            }
        }
        public bool IsConnected2
        {
            get
            {
                if (connect_2 == false)
                {
                    return false;
                }
                else
                {
                    return connect_2 = true;
                }

            }
        }
        //public bool connect1()
        //{
        //    MessErr = string.Empty;
        //    if (socket != null)
        //    {
        //        socket.Close();
        //        socket = null;
        //    }
        //    string path = AppDomain.CurrentDomain.BaseDirectory;
        //    if (!Directory.Exists(path))
        //    {
        //        Directory.CreateDirectory(path);
        //    }
        //    string content;
        //    content = File.ReadAllText(path + Path.DirectorySeparatorChar + "TCPsave.txt");
        //    string[] ipp = content.Split(';');
        //    IP = ipp[2];
        //    Port = Convert.ToInt16(ipp[1]);
        //    //IP = "10.200.111.188";
        //    //Port = 3001;
        //    IPEndPoint ipe = new IPEndPoint(IPAddress.Parse(IP), Port);
        //    socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        //    bool ret = false;
        //    if (socket.Connected == false)
        //    {
        //        try
        //        {
        //            socket.Connect(ipe);

        //            ret = socket.Connected;
        //            ret = IsConnected;

        //        }
        //        catch (Exception ex)
        //        {
        //            MessErr = ex.ToString();
        //            ret = false;

        //        }
        //    }
        //    else
        //    {
        //        ret = true;
        //    }
        //    return ret;
        //}
        public string saveip(string ipRB, int portRB, string ipPLC, int portPLC, string ipVS1, int portVS1, string ipVS2, int portVS2)
        {
            string path = AppDomain.CurrentDomain.BaseDirectory;
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            string filePath = Path.Combine(path, "TCPsave.txt");
            string content = ipRB + ";" + portRB.ToString() + ";" + ipPLC + ";" + portPLC.ToString()
                + ";" + ipVS1 + ";" + portVS1.ToString() + ";" + ipVS2 + ";" + portVS2.ToString();
            if (!File.Exists(filePath))
            {
                File.Create(filePath).Dispose();
            }
            File.WriteAllText(path + Path.DirectorySeparatorChar + "TCPsave.txt", content);
            MessageBox.Show("Save Succesful");
            return content;
        }
        public bool connect1()
        {
            string path = AppDomain.CurrentDomain.BaseDirectory;
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            string content;
            content = File.ReadAllText(path + Path.DirectorySeparatorChar + "TCPIP.txt");
            string[] parameter_connect = content.Split(';');
            Port = Convert.ToInt16(parameter_connect[3]);
            PLC_MX.ActLogicalStationNumber = Port;
            int res = PLC_MX.Open();
            if (res == 0)
            {
                connect = true;
                Global.IsConnectPLC = true;
            }

            return connect;
        }
        public bool connect2()
        {         
            PLC_MX.ActLogicalStationNumber = Port;
            int res = PLC_MX.Open();
            if (res == 0)
            {
                connect_2 = true;
                Global.IsConnectPLC = true;
            }

            return connect_2;
        }
        public int Err_Message()
        {
            int code = 0;
            return code;
        }
        public string infor_PLC()
        {
            string info = "";
            int code;
            PLC_MX.GetCpuType(out info, out code);

            info = info + code.ToString();
            return info;
        }
        #region Write
        public bool Write_DataBit_(string addr, int data_out)
        {
            bool res = false;
            lock (lock_R)
            {
                int cc = PLC_MX.SetDevice(addr, data_out);
                if (cc == 0)
                {
                    res = true;
                }
                else
                {
                    res = false;
                }
            }

            return res;

        }
        public bool Write_Data_Word_(string addr, int data_out)
        {
            bool res = false;
            lock (lock_W)
            {
                int cc = PLC_MX.SetDevice(addr, data_out);
                if (cc == 0)
                {
                    res = true;
                }
                else
                {
                    res = false;
                }
            }

            return res;
        }
        public void Write_Data_DWord_(string Addr, int Value)
        {
            int[] Value_AR_int16 = new int[2];
            lock (lock_W)
            {
                Convert_to_32Bit(Value, ref Value_AR_int16[0], ref Value_AR_int16[1]);
                PLC_MX.WriteDeviceBlock(Addr, 2, ref Value_AR_int16[0]);
            }

        }
        public void Write_Word_Array_(string addr, int[] data_out, int number_write)
        {
            lock (lock_W)
            {
                PLC_MX.WriteDeviceBlock(addr, number_write, ref data_out[0]);
            }

        }

        #endregion
        #region READ
        public bool Read_Data_Bit_(string addr)
        {
            bool res = false;
            int data;
            lock (lock_R)
            {
                PLC_MX.GetDevice(addr, out data);
                if (data == 1)
                {
                    res = true;
                }
                else
                {
                    res = false;
                }
            }

            return res;
        }
        public int[] Read_Data_Bit_Arr(string addr, int number_device_read)
        {
            string device_name = addr.Substring(0, 1);
            int device_number = Convert.ToInt16(addr.Substring(1, addr.Length - 1));
            int[] data = new int[number_device_read];
            int data_out;
            lock (lock_R)
            {
                for (int i = 0; i < data.Length; i++)
                {
                    if (Close_Alarm == false)
                    {
                        PLC_MX.GetDevice(device_name + (device_number + i).ToString(), out data_out);                           
                        data[i] = data_out;
                    }

                    else
                    {
                        i = data.Length + 1;
                    }
                }
            }
            return data;
        }
        public int[] Read_Data_Bit_Arr2(string addr, int number_device_read)
        {
            string device_name = addr.Substring(0, 1);
            int device_number = Convert.ToInt16(addr.Substring(1, addr.Length - 1));
            int[] data = new int[number_device_read];
            int data_out;
            lock (lock_R)
            {
                for (int i = 0; i < data.Length; i++)
                {                                        
                        PLC_MX.GetDevice(device_name + (device_number + i).ToString(), out data_out);
                        data[i] = data_out;                                       
                }
            }
            return data;
        }
        public int Read_Data_Word_(string addr)
        {
            int data;
            lock (lock_R)
            {
                PLC_MX.GetDevice(addr, out data);
            }
            if (data >= 32768)
            {
                data = data - 65536;
            }
            return data;
        }

        public int Read_Data_DWord_(string addr)
        {
            int data;
            int[] buffer_R_DW = new int[2];
            lock (lock_R)
            {
                PLC_MX.ReadDeviceBlock(addr, 2, out buffer_R_DW[0]);
                data = buffer_R_DW[1] << 16 | buffer_R_DW[0];
            }

            return data;
        }
        public int[] read_DWord_AR(string Addr, int number_Value)
        {
            int[] result_R_DW_AR = new int[number_Value];
            int number_Resgiter = 2 * number_Value;
            int[] buffer1 = new int[number_Resgiter];
            lock (lock_R)
            {
                PLC_MX.ReadDeviceBlock(Addr, number_Resgiter, out buffer1[0]);
            }

            for (int i = 0; i < number_Value; i++)
            {
                int j = i * 2;
                result_R_DW_AR[i] = buffer1[j + 1] << 16 | buffer1[j];
            }

            return result_R_DW_AR;
        }
        public int[] Read_Word_Arr(string addr, int number_read)
        {
            int[] data = new int[number_read];
            lock (lock_R)
            {
                PLC_MX.ReadDeviceBlock(addr, number_read, out data[0]);
            }
            for (int i = 0; i < number_read; i++)
            {
                if (data[i] > 32768)
                    data[i] = data[i] - 65536;
            }
            return data;

        }
        public int []ReadRandomBit(string addr, int number_read)
        {
            int[] data = new int[number_read];
            string DeviceNane = "";
            int add_number = Convert.ToInt32(addr.Substring(1, addr.Length - 1));
            string TypeName = addr.Substring(0, 1);
            for(int i=0; i < number_read; i++)
            {
                DeviceNane += TypeName + (add_number + i).ToString() + "\n";
            }
            lock (lock_R)
            {
                PLC_MX.ReadDeviceRandom(DeviceNane, number_read, out data[0]);
            }
            return data;
        }
        #endregion
        #region Convert
        private short[] ConvertIntTOShort(int inData)
        {
            short[] outData = new short[2];
            outData[0] = (short)inData;
            outData[1] = (short)(inData >> 16);
            return outData;
        }
        public void Convert_to_32Bit(int ValueConvert, ref int Val1, ref int Val2)
        {
            byte[] ValueByte = BitConverter.GetBytes(ValueConvert);

            short Val_Buffer1 = BitConverter.ToInt16(ValueByte, 0);
            short Val_Buffer2 = BitConverter.ToInt16(ValueByte, 2);
            Val1 = Val_Buffer1;
            Val2 = Val_Buffer2;

        }
        private float ToFloat_word(int[] data)
        {
            if (data == null)
            {
                return -1;
            }
            try
            {
                byte[] byarrBufferByte = new byte[4];
                byte[] byarrTemp;
                int iNumber;
                for (iNumber = 0; iNumber <= 1; iNumber++)
                {
                    byarrTemp = BitConverter.GetBytes(data[iNumber]);
                    byarrBufferByte[iNumber * 2] = byarrTemp[0];
                    byarrBufferByte[iNumber * 2 + 1] = byarrTemp[1];
                }
                float output = BitConverter.ToSingle(byarrBufferByte, 0);
                return output;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                return 0;
            }
        }
        private int Converint32(int[] data)
        {
            int outdata;

            outdata = ((int)data[1] << 16) | (ushort)data[0];
            return outdata;
        }
        private int[] Convert16_to_32(int[] data)
        {
            int s = (data.Length - 1) / 2;
            int[] outdata = new int[s];
            for (int i = 0; i < s; i++)
            {
                outdata[i] = ((int)data[i * 2 + 1] << 16) | (ushort)data[i * 2];
            }

            return outdata;
        }
        bool IsChanLe = false;
        private string Convert_CharTo_Hex(string data)
        {
            char[] arrhex = new char[100];
            string[] data_out = new string[100];
            string data_set = "";

            string hex = data;
            int d = hex.Length;
            if (d > 1)
            {
                if (hex.Length % 2 == 0)
                {
                    IsChanLe = true;
                }
                if (IsChanLe == false)
                {
                    arrhex = new char[hex.Length + 1];
                    data_out = new string[hex.Length + 1];
                    d = hex.Length + 1;
                }
                else
                {
                    arrhex = new char[hex.Length];
                    data_out = new string[hex.Length];
                    d = hex.Length;
                }

                for (int i = 0; i < d; i++)
                {
                    try { arrhex[i] = Convert.ToChar(hex.Substring(i, 1)); }
                    catch { }

                    if (arrhex[i] != null)
                    {
                        int value = Convert.ToInt16(arrhex[i]);
                        data_out[i] = value.ToString("x").PadLeft(2, '0');
                    }
                    else
                    {
                        data_out[i] = "00";
                    }

                }

                for (int j = 0; j < d / 2; j++)
                {
                    if (data_out[j * 2 + 1] != null && data_out[j * 2] != null)
                    {
                        data_set += data_out[j * 2 + 1] + data_out[j * 2];
                    }


                }
            }
            else
            {
                data_set = "0";
            }
            return data_set;
        }
        #endregion


    }

}

