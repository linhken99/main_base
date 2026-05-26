using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using System.IO;


namespace Main_Base
{
    public class Matrix
    {
        double[][] matrix_panel = new double[5000][];
        int row, colum;
        string _connectionString = "Data Source=|DataDirectory|/SQL_Matrix_Tool.db";       // @"Data source =C:\Users\admin\OneDrive\Desktop\11111\matrixRB\dataMatrix.db";
        SQLiteConnection _con = new SQLiteConnection();
        public bool Flag_Read_Data_Maxtrix_Tool_RB = false;
        public void Connect_SQLite(string Str_Connection)
        {

            _con.ConnectionString = _connectionString;
            _con.Open();
        }
        public void Disconnect_Sqlite()
        {

            _con.Close();
        }
        private SQLiteConnection GetConnectionSQLite()
        {
            return new SQLiteConnection(_connectionString);
        }
        //=============================================================================================
        public void PAL_PR_RB(int index, int rows, int colums, double[] PP1, double[] PP2, double[] PP3)
        {
            List<double> matrix_panel_1 = new List<double>();
            row = rows;
            colum = colums;
            int I, J, K, L, N, T, A, B, C;
            Connect_SQLite(_connectionString);
            //double[] PAL = PP1;
            double[] PAL = new double[] { 0, 0, 0, 0, 0, PP1[5] };
            N = rows * colums;
            A = index + 15;
            int[] Counter = new int[20];
            Counter[A] = N;
            L = 1;
            T = 0;
            K = 1;
            while (L < index)
            {
                B = L + 15;
                T = T + Counter[B];
                L = L + 1;

            }
            while (K <= N)
            {
                J = K / row;
                I = (K - 1) - row * J;
                if (I < 0)
                {
                    I = I + row;
                    J = J - 1;
                }
                if (row == 1 || colum == 1)
                {
                    if (row == 1)
                    {
                        PAL[0] = PP1[0] + (PP2[0] - PP1[0]) * I + (PP3[0] - PP1[0]) / (colum - 1) * J;
                        PAL[1] = PP1[1] + (PP2[1] - PP1[1]) * I + (PP3[1] - PP1[1]) / (colum - 1) * J;
                        PAL[2] = PP1[2] + (PP2[2] - PP1[2]) * I + (PP3[2] - PP1[2]) / (colum - 1) * J;

                    }
                    else
                    {
                        PAL[0] = PP1[0] + (PP2[0] - PP1[0]) / (row - 1) * I + (PP3[0] - PP1[0]) * J;
                        PAL[1] = PP1[1] + (PP2[1] - PP1[1]) / (row - 1) * I + (PP3[1] - PP1[1]) * J;
                        PAL[2] = PP1[2] + (PP2[0] - PP1[2]) / (row - 1) * I + (PP3[2] - PP1[2]) * J;
                    }
                }
                else
                {
                    PAL[0] = PP1[0] + (PP2[0] - PP1[0]) / (row - 1) * I + (PP3[0] - PP1[0]) / (colum - 1) * J;
                    PAL[1] = PP1[1] + (PP2[1] - PP1[1]) / (row - 1) * I + (PP3[1] - PP1[1]) / (colum - 1) * J;
                    PAL[2] = PP1[2] + (PP2[2] - PP1[2]) / (row - 1) * I + (PP3[2] - PP1[2]) / (colum - 1) * J;
                }
                if (index > 1)
                {
                    C = K + T + ((index - 1) * N);
                }
                else
                {
                    C = K + T;
                }
                matrix_panel[C] = PAL;
                //textBox1.Text += string.Join(";", matrix_panel[C]) + Environment.NewLine;

                string insert;
                //kiêm tra row tồn tại                            
                // insert = string.Format("UPDATE matrix_panel set  VALUE1='{0}', VALUE2='{1}', VALUE3='{2}', VALUE4='{3}' ,VALUE5='{4}',VALUE6='{5}'where ID='{6}'", PAL[0], PAL[1], PAL[2], 0, 0, PAL[5], C);
                insert = string.Format("INSERT OR REPLACE INTO Matrix_Panel_tool (ID, VALUE1, VALUE2,VALUE3,VALUE4,VALUE5,VALUE6) VALUES (@ID, @VALUE1, @VALUE2,@VALUE3,@VALUE4,@VALUE5,@VALUE6)");
                //OR REPLACE
                //Connect_SQLite(_connectionString);
                using (SQLiteCommand cmd = new SQLiteCommand(insert, _con))
                {
                    cmd.Parameters.AddWithValue("@ID", C);
                    cmd.Parameters.AddWithValue("@VALUE1", PAL[0]);
                    cmd.Parameters.AddWithValue("@VALUE2", PAL[1]);
                    cmd.Parameters.AddWithValue("@VALUE3", PAL[2]);
                    cmd.Parameters.AddWithValue("@VALUE4", 0.0);
                    cmd.Parameters.AddWithValue("@VALUE5", 0.0);
                    cmd.Parameters.AddWithValue("@VALUE6", PAL[5]);
                    cmd.ExecuteNonQuery();

                }
                // insert = string.Format("INSERT INTO matrix_panel(ID,VALUE1,VALUE2,VALUE3,VALUE4,VALUE5,VALUE6) VAlUES('{0}','{1}','{2}','{3}','{4}','{5}','{6}')", C, PAL[0], PAL[1], PAL[2], 0, 0, PAL[5]);

                K = K + 1;

            }

            Disconnect_Sqlite();
        }
        public void PAL_PR_RB1(int index, int rows, int colums, List<double>[] data_pos1, List<double>[] data_pos2, List<double>[] data_pos3, int number_tool)
        {
            List<double>[] matrix_panel_1 = new List<double>[number_tool];
            List<double>[] matrix_panel_2 = new List<double>[number_tool];
            List<double>[] matrix_panel_3 = new List<double>[number_tool];

            matrix_panel_1 = data_pos1;
            matrix_panel_2 = data_pos2;
            matrix_panel_3 = data_pos3;
            Connect_SQLite(_connectionString);
            for (int i = 0; i < number_tool; i++)
            {
                row = rows;
                colum = colums;
                int I, J, K, L, N, T, A, B, C;
                //double[] PAL = PP1;
                double[] PAL = new double[] { 0, 0, 0, 0, 0, matrix_panel_1[0][5] };
                N = rows * colums;
                A = index + i + 15;
                int[] Counter = new int[2000];
                Counter[A] = N;
                L = 1;
                T = 0;
                K = 1;
                while (L < index + i)
                {
                    B = L + 15;
                    T = T + Counter[B];
                    L = L + 1;
                }
                while (K <= N)
                {
                    J = K / row;
                    I = (K - 1) - row * J;
                    if (I < 0)
                    {
                        I = I + row;
                        J = J - 1;
                    }
                    if (row == 1 || colum == 1)
                    {
                        if (row == 1)
                        {
                            PAL[0] = matrix_panel_1[i][0] + (matrix_panel_2[i][0] - matrix_panel_1[i][0]) * I + (matrix_panel_3[i][0] - matrix_panel_1[i][0]) / (colum - 1) * J;
                            PAL[1] = matrix_panel_1[i][1] + (matrix_panel_2[i][1] - matrix_panel_1[i][1]) * I + (matrix_panel_3[i][1] - matrix_panel_1[i][1]) / (colum - 1) * J;
                            PAL[2] = matrix_panel_1[i][2] + (matrix_panel_2[i][2] - matrix_panel_1[i][2]) * I + (matrix_panel_3[i][2] - matrix_panel_1[i][2]) / (colum - 1) * J;

                        }
                        else
                        {
                            PAL[0] = matrix_panel_1[i][0] + (matrix_panel_2[i][0] - matrix_panel_1[i][0]) / (row - 1) * I + (matrix_panel_3[i][0] - matrix_panel_1[i][0]) * J;
                            PAL[1] = matrix_panel_1[i][1] + (matrix_panel_2[i][1] - matrix_panel_1[i][1]) / (row - 1) * I + (matrix_panel_3[i][1] - matrix_panel_1[i][1]) * J;
                            PAL[2] = matrix_panel_1[i][2] + (matrix_panel_2[i][0] - matrix_panel_1[i][2]) / (row - 1) * I + (matrix_panel_3[i][2] - matrix_panel_1[i][2]) * J;
                        }
                    }
                    else
                    {
                        PAL[0] = matrix_panel_1[i][0] + (matrix_panel_2[i][0] - matrix_panel_1[i][0]) / (row - 1) * I + (matrix_panel_3[i][0] - matrix_panel_1[i][0]) / (colum - 1) * J;
                        PAL[1] = matrix_panel_1[i][1] + (matrix_panel_2[i][1] - matrix_panel_1[i][1]) / (row - 1) * I + (matrix_panel_3[i][1] - matrix_panel_1[i][1]) / (colum - 1) * J;
                        PAL[2] = matrix_panel_1[i][2] + (matrix_panel_2[i][2] - matrix_panel_1[i][2]) / (row - 1) * I + (matrix_panel_3[i][2] - matrix_panel_1[i][2]) / (colum - 1) * J;
                    }
                    if (index + i > 1)
                    {
                        C = K + T + ((index + i - 1) * N);
                    }
                    else
                    {
                        C = K + T;
                    }
                    matrix_panel[C] = PAL;
                    //textBox1.Text += string.Join(";", matrix_panel[C]) + Environment.NewLine;

                    string insert;
                    //kiêm tra row tồn tại                            
                    // insert = string.Format("UPDATE matrix_panel set  VALUE1='{0}', VALUE2='{1}', VALUE3='{2}', VALUE4='{3}' ,VALUE5='{4}',VALUE6='{5}'where ID='{6}'", PAL[0], PAL[1], PAL[2], 0, 0, PAL[5], C);
                    insert = string.Format("INSERT OR REPLACE INTO Matrix_Panel_tool (ID, VALUE1, VALUE2,VALUE3,VALUE4,VALUE5,VALUE6) VALUES (@ID, @VALUE1, @VALUE2,@VALUE3,@VALUE4,@VALUE5,@VALUE6)");
                    //OR REPLACE
                    //Connect_SQLite(_connectionString);
                    using (SQLiteCommand cmd = new SQLiteCommand(insert, _con))
                    {
                        cmd.Parameters.AddWithValue("@ID", C);
                        cmd.Parameters.AddWithValue("@VALUE1", PAL[0]);
                        cmd.Parameters.AddWithValue("@VALUE2", PAL[1]);
                        cmd.Parameters.AddWithValue("@VALUE3", PAL[2]);
                        cmd.Parameters.AddWithValue("@VALUE4", 0.0);
                        cmd.Parameters.AddWithValue("@VALUE5", 0.0);
                        cmd.Parameters.AddWithValue("@VALUE6", PAL[5]);
                        cmd.ExecuteNonQuery();

                    }
                    // insert = string.Format("INSERT INTO matrix_panel(ID,VALUE1,VALUE2,VALUE3,VALUE4,VALUE5,VALUE6) VAlUES('{0}','{1}','{2}','{3}','{4}','{5}','{6}')", C, PAL[0], PAL[1], PAL[2], 0, 0, PAL[5]);

                    K = K + 1;

                }
            }
            Disconnect_Sqlite();
        }
        public double[] PAL_P_RB(int index, int N)
        {
            int i = 0;
            int I, T, A, B;
            double[] PPE = new double[6];
            I = 1;
            T = 0;
            int[] Counter = new int[2000];
            Flag_Read_Data_Maxtrix_Tool_RB = false;
            while (I < index)
            {
                A = 15 + I;
                T = T + Counter[A];
                I = I + 1;

            }
            if (index > 1)
            {
                i = N + (row * colum * (index - 1));
            }
            else
            {
                i = N;
            }
            B = N + T;
            try
            {
                //string query = "SELECT VALUE1,VALUE2,VALUE3,VALUE4,VALUE5,VALUE6 FROM matrix_panel";
                string query = string.Format("SELECT* from Matrix_Panel_tool where ID='{0}' ", i);
                Connect_SQLite(_connectionString);
                SQLiteCommand cmd = new SQLiteCommand(query, _con);
                var reader = cmd.ExecuteReader();
                var pos_list = new List<Tuple<double, double, double, double, double, double>>();
                while (reader.Read())
                {
                    var value1_pos = Math.Round(Convert.ToDouble(reader["VALUE1"]), 3);
                    var value2_pos = Math.Round(Convert.ToDouble(reader["VALUE2"]), 3);
                    var value3_pos = Math.Round(Convert.ToDouble(reader["VALUE3"]), 3);
                    var value4_pos = Math.Round(Convert.ToDouble(reader["VALUE4"]), 3);
                    var value5_pos = Math.Round(Convert.ToDouble(reader["VALUE5"]), 3);
                    var value6_pos = Math.Round(Convert.ToDouble(reader["VALUE6"]), 3);
                    pos_list.Add(new Tuple<double, double, double, double, double, double>(value1_pos, value2_pos, value3_pos, value4_pos, value5_pos, value6_pos));
                    PPE = new double[] { value1_pos, value2_pos, value3_pos, value4_pos, value5_pos, value6_pos };
                }
                Flag_Read_Data_Maxtrix_Tool_RB = true;

                Disconnect_Sqlite();
            }
            // lấy pos từ SQLite
            catch (Exception ex)
            {
                Flag_Read_Data_Maxtrix_Tool_RB = false;
                Disconnect_Sqlite();
            }

            return PPE;
        }
        public double[] PAL_P_RB_Cam_Top(int index, int N)
        {
            int i = 0;
            int I, T, A, B;
            double[] PPE = new double[6];
            I = 1;
            T = 0;
            int[] Counter = new int[2000];
            Flag_Read_Data_Maxtrix_Tool_RB = false;
            while (I < index)
            {
                A = 15 + I;
                T = T + Counter[A];
                I = I + 1;

            }
            if (index > 1)
            {
                i = N + (row * colum * (index - 1));
            }
            else
            {
                i = N;
            }
            B = N + T;
            try
            {
                //string query = "SELECT VALUE1,VALUE2,VALUE3,VALUE4,VALUE5,VALUE6 FROM matrix_panel";
                string query = string.Format("SELECT* from Matrix_Panel_Cam_Top where ID='{0}' ", i);
                Connect_SQLite(_connectionString);
                SQLiteCommand cmd = new SQLiteCommand(query, _con);
                var reader = cmd.ExecuteReader();
                var pos_list = new List<Tuple<double, double, double, double, double, double>>();
                while (reader.Read())
                {
                    var value1_pos = Math.Round(Convert.ToDouble(reader["X"]), 3);
                    var value2_pos = Math.Round(Convert.ToDouble(reader["Y"]), 3);
                    var value3_pos = Math.Round(Convert.ToDouble(reader["Z"]), 3);
                    var value4_pos = Math.Round(Convert.ToDouble(reader["A3"]), 3);
                    var value5_pos = Math.Round(Convert.ToDouble(reader["A4"]), 3);
                    var value6_pos = Math.Round(Convert.ToDouble(reader["C"]), 3);
                    pos_list.Add(new Tuple<double, double, double, double, double, double>(value1_pos, value2_pos, value3_pos, value4_pos, value5_pos, value6_pos));
                    PPE = new double[] { value1_pos, value2_pos, value3_pos, value4_pos, value5_pos, value6_pos };
                }
                Flag_Read_Data_Maxtrix_Tool_RB = true;

                Disconnect_Sqlite();
            }
            // lấy pos từ SQLite
            catch (Exception ex)
            {
                Flag_Read_Data_Maxtrix_Tool_RB = false;
                Disconnect_Sqlite();
            }

            return PPE;
        }
        //public void PAL_PR_X_PLC(int index, int rows, int colums, int X1,int X2,int X3)
        //{
        //    double[] matrix_panel_X = new double[100];
        //    row = rows;
        //    colum = colums;
        //    int I, J, K, L, N, T, A, B, C;
        //    int X;
        //    //double[] PAL = PP1;
        //   // double[] PAL = new double[] { 0, 0, 0, 0, 0, PP1[5] };
        //    N = rows * colums;
        //    A = index + 15;
        //    int[] Counter = new int[20];
        //    Counter[A] = N;
        //    L = 1;
        //    T = 0;
        //    K = 1;
        //    Connect_SQLite(_connectionString);
        //    while (L < index)
        //    {
        //        B = L + 15;
        //        T = T + Counter[B];
        //        L = L + 1;

        //    }
        //    while (K <= N)
        //    {
        //        J = K / row;
        //        I = (K - 1) - row * J;
        //        if (I < 0)
        //        {
        //            I = I + row;
        //            J = J - 1;
        //        }
        //        if (row == 1 || colum == 1)
        //        {
        //            if (row == 1)
        //            {
        //                X = X1 + (X2 - X1) * I + (X3 - X1) / (colum - 1) * J;


        //            }
        //            else
        //            {
        //                X =X1 + (X2 - X1) / (row - 1) * I + (X3 - X1) * J;

        //            }
        //        }
        //        else
        //        {
        //            X = X1 + (X2 - X1) / (row - 1) * I + (X3 - X1) / (colum - 1) * J;

        //        }
        //        if (index > 1)
        //        {
        //            C = K + T + ((index - 1) * N);
        //        }
        //        else
        //        {
        //            C = K + T;
        //        }
        //        matrix_panel_X[C] =Convert.ToDouble( X);
        //        //textBox1.Text += string.Join(";", matrix_panel[C]) + Environment.NewLine;

        //        //string insert;

        //        //kiêm tra row tồn tại                            
        //        // insert = string.Format("UPDATE matrix_panel set  VALUE1='{0}', VALUE2='{1}', VALUE3='{2}', VALUE4='{3}' ,VALUE5='{4}',VALUE6='{5}'where ID='{6}'", PAL[0], PAL[1], PAL[2], 0, 0, PAL[5], C);
        //        string insert = string.Format("INSERT OR REPLACE INTO Matrix_Panel_X_PLC (STT, X) VALUES (@STT, @X)");
        //       // SQLite_Connect();

        //        using (SQLiteCommand cmd = new SQLiteCommand(insert, _con))
        //        {
        //            cmd.Parameters.AddWithValue("@STT", C);
        //            cmd.Parameters.AddWithValue("@X", X);

        //            cmd.ExecuteNonQuery();

        //        }
        //        // insert = string.Format("INSERT INTO matrix_panel(ID,VALUE1,VALUE2,VALUE3,VALUE4,VALUE5,VALUE6) VAlUES('{0}','{1}','{2}','{3}','{4}','{5}','{6}')", C, PAL[0], PAL[1], PAL[2], 0, 0, PAL[5]);

        //        K = K + 1;

        //    }
        //    Disconnect_Sqlite();
        //}
        //public void PAL_PR_Y_PLC(int index, int rows, int colums, double Y1, double Y2, double Y3)
        //{
        //    List<double> matrix_panel_Y = new List<double>();
        //    row = rows;
        //    colum = colums;
        //    int I, J, K, L, N, T, A, B, C;
        //    double Y;
        //    //double[] PAL = PP1;
        //    // double[] PAL = new double[] { 0, 0, 0, 0, 0, PP1[5] };
        //    N = rows * colums;
        //    A = index + 15;
        //    int[] Counter = new int[20];
        //    Counter[A] = N;
        //    L = 1;
        //    T = 0;
        //    K = 1;
        //    while (L < index)
        //    {
        //        B = L + 15;
        //        T = T + Counter[B];
        //        L = L + 1;

        //    }
        //    while (K <= N)
        //    {
        //        J = K / row;
        //        I = (K - 1) - row * J;
        //        if (I < 0)
        //        {
        //            I = I + row;
        //            J = J - 1;
        //        }
        //        if (row == 1 || colum == 1)
        //        {
        //            if (row == 1)
        //            {
        //                Y = Y1 + (Y2 - Y1) * I + (Y3 - Y1) / (colum - 1) * J;


        //            }
        //            else
        //            {
        //                Y = Y1 + (Y2 - Y1) / (row - 1) * I + (Y3 - Y1) * J;

        //            }
        //        }
        //        else
        //        {
        //            Y = Y1 + (Y2 - Y1) / (row - 1) * I + (Y3 - Y1) / (colum - 1) * J;

        //        }
        //        if (index > 1)
        //        {
        //            C = K + T + ((index - 1) * N);
        //        }
        //        else
        //        {
        //            C = K + T;
        //        }
        //        matrix_panel_Y[C] = Y;
        //        //textBox1.Text += string.Join(";", matrix_panel[C]) + Environment.NewLine;

        //        string insert;
        //        //kiêm tra row tồn tại                            
        //        // insert = string.Format("UPDATE matrix_panel set  VALUE1='{0}', VALUE2='{1}', VALUE3='{2}', VALUE4='{3}' ,VALUE5='{4}',VALUE6='{5}'where ID='{6}'", PAL[0], PAL[1], PAL[2], 0, 0, PAL[5], C);
        //        insert = string.Format("INSERT OR REPLACE INTO Matrix_Panel_Y_PLC (STT, X) VALUES (@STT, @Y)");
        //        //SQLite_Connect();

        //        using (SQLiteCommand cmd = new SQLiteCommand(insert, _con))
        //        {
        //            cmd.Parameters.AddWithValue("@STT", C);
        //            cmd.Parameters.AddWithValue("@Y", Y);

        //            cmd.ExecuteNonQuery();

        //        }
        //        // insert = string.Format("INSERT INTO matrix_panel(ID,VALUE1,VALUE2,VALUE3,VALUE4,VALUE5,VALUE6) VAlUES('{0}','{1}','{2}','{3}','{4}','{5}','{6}')", C, PAL[0], PAL[1], PAL[2], 0, 0, PAL[5]);

        //        K = K + 1;

        //    }

        //}
        //=============================================================================================
        public void PAL_PR_XY_PLC(int index, int rows, int colums, int[] PP1, int[] PP2, int[] PP3)
        {

            row = rows;
            colum = colums;
            int I, J, K, L, N, T, A, B, C;

            //double[] PAL = PP1;
            double[] PAL = new double[2];
            N = rows * colums;
            A = index + 15;
            int[] Counter = new int[20];
            Counter[A] = N;
            L = 1;
            T = 0;
            K = 1;
            Connect_SQLite(_connectionString);
            while (L < index)
            {
                B = L + 15;
                T = T + Counter[B];
                L = L + 1;

            }
            while (K <= N)
            {
                J = K / row;
                I = (K - 1) - row * J;
                if (I < 0)
                {
                    I = I + row;
                    J = J - 1;
                }
                if (row == 1 || colum == 1)
                {
                    if (row == 1)
                    {
                        PAL[0] = PP1[0] + (PP2[0] - PP1[0]) * I + (PP3[0] - PP1[0]) / (colum - 1) * J;
                        PAL[1] = PP1[1] + (PP2[1] - PP1[1]) * I + (PP3[1] - PP1[1]) / (colum - 1) * J;


                    }
                    else
                    {
                        PAL[0] = PP1[0] + (PP2[0] - PP1[0]) / (row - 1) * I + (PP3[0] - PP1[0]) * J;
                        PAL[1] = PP1[1] + (PP2[1] - PP1[1]) / (row - 1) * I + (PP3[1] - PP1[1]) * J;

                    }
                }
                else
                {
                    PAL[0] = PP1[0] + (PP2[0] - PP1[0]) / (row - 1) * I + (PP3[0] - PP1[0]) / (colum - 1) * J;
                    PAL[1] = PP1[1] + (PP2[1] - PP1[1]) / (row - 1) * I + (PP3[1] - PP1[1]) / (colum - 1) * J;

                }
                if (index > 1)
                {
                    C = K + T + ((index - 1) * N);
                }
                else
                {
                    C = K + T;
                }

                //textBox1.Text += string.Join(";", matrix_panel[C]) + Environment.NewLine;

                string insert;
                //kiêm tra row tồn tại                            
                // insert = string.Format("UPDATE matrix_panel set  VALUE1='{0}', VALUE2='{1}', VALUE3='{2}', VALUE4='{3}' ,VALUE5='{4}',VALUE6='{5}'where ID='{6}'", PAL[0], PAL[1], PAL[2], 0, 0, PAL[5], C);
                insert = string.Format("INSERT OR REPLACE INTO Matrix_Panel_XY_PLC (STT, X, Y) VALUES (@STT, @X, @Y)");
                //SQLite_Connect();

                using (SQLiteCommand cmd = new SQLiteCommand(insert, _con))
                {
                    cmd.Parameters.AddWithValue("@STT", C);
                    cmd.Parameters.AddWithValue("@X", PAL[0]);
                    cmd.Parameters.AddWithValue("@Y", PAL[1]);

                    cmd.ExecuteNonQuery();

                }
                // insert = string.Format("INSERT INTO matrix_panel(ID,VALUE1,VALUE2,VALUE3,VALUE4,VALUE5,VALUE6) VAlUES('{0}','{1}','{2}','{3}','{4}','{5}','{6}')", C, PAL[0], PAL[1], PAL[2], 0, 0, PAL[5]);

                K = K + 1;

            }
            Disconnect_Sqlite();

        }
        public int[] PAL_P_XY_PLC(int index, int N, int ind)
        {
            int i = 0;
            int I, T, A, B;
            int[] PPE = new int[2];
            string Zindex = ind.ToString();
            int[] valueX_pos = new int[5000];
            I = 1;
            T = 0;
            int[] Counter = new int[2000];
            Connect_SQLite(_connectionString);
            while (I < index)
            {
                A = 15 + I;
                T = T + Counter[A];
                I = I + 1;

            }
            if (index > 1)
            {
                i = N + (row * colum * (index - 1));
            }
            else
            {
                i = N;
            }
            B = N + T;
            try
            {

                //string query = "SELECT VALUE1,VALUE2,VALUE3,VALUE4,VALUE5,VALUE6 FROM matrix_panel";
                string query = string.Format("SELECT* from Matrix_Panel_XY_PLC where STT='{0}' ", i);

                SQLiteCommand cmd = new SQLiteCommand(query, _con);
                var reader = cmd.ExecuteReader();
                double Data;
                while (reader.Read())
                {
                    var ValueX_posX = (reader["X"]);
                    var ValueX_posY = (reader["Y"]);
                    //PLC3E.Write_Data_DWord_("D250" + Zindex, Convert.ToInt32(ValueX_posX));
                    //PLC3E.Write_Data_DWord_("D270" + Zindex, Convert.ToInt32(ValueX_posY));
                    PPE = new int[2] { Convert.ToInt32(ValueX_posX), Convert.ToInt32(ValueX_posY) };
                }



            }

            // lấy pos từ SQLite
            catch (Exception ex) { }
            Disconnect_Sqlite();
            return PPE;
        }
        //=============================================================================================
        public void PAL_PR_CAM_PLC(int index, int rows, int colums, int[] PP1, int[] PP2, int[] PP3)
        {

            row = rows;
            colum = colums;
            int I, J, K, L, N, T, A, B, C;

            //double[] PAL = PP1;
            double[] PAL = new double[2];
            N = rows * colums;
            A = index + 15;
            int[] Counter = new int[20];
            Counter[A] = N;
            L = 1;
            T = 0;
            K = 1;
            Connect_SQLite(_connectionString);
            while (L < index)
            {
                B = L + 15;
                T = T + Counter[B];
                L = L + 1;

            }
            while (K <= N)
            {
                J = K / row;
                I = (K - 1) - row * J;
                if (I < 0)
                {
                    I = I + row;
                    J = J - 1;
                }
                if (row == 1 || colum == 1)
                {
                    if (row == 1)
                    {
                        PAL[0] = PP1[0] + (PP2[0] - PP1[0]) * I + (PP3[0] - PP1[0]) / (colum - 1) * J;
                        PAL[1] = PP1[1] + (PP2[1] - PP1[1]) * I + (PP3[1] - PP1[1]) / (colum - 1) * J;


                    }
                    else
                    {
                        PAL[0] = PP1[0] + (PP2[0] - PP1[0]) / (row - 1) * I + (PP3[0] - PP1[0]) * J;
                        PAL[1] = PP1[1] + (PP2[1] - PP1[1]) / (row - 1) * I + (PP3[1] - PP1[1]) * J;

                    }
                }
                else
                {
                    PAL[0] = PP1[0] + (PP2[0] - PP1[0]) / (row - 1) * I + (PP3[0] - PP1[0]) / (colum - 1) * J;
                    PAL[1] = PP1[1] + (PP2[1] - PP1[1]) / (row - 1) * I + (PP3[1] - PP1[1]) / (colum - 1) * J;

                }
                if (index > 1)
                {
                    C = K + T + ((index - 1) * N);
                }
                else
                {
                    C = K + T;
                }

                //textBox1.Text += string.Join(";", matrix_panel[C]) + Environment.NewLine;

                string insert;
                //kiêm tra row tồn tại                            
                // insert = string.Format("UPDATE matrix_panel set  VALUE1='{0}', VALUE2='{1}', VALUE3='{2}', VALUE4='{3}' ,VALUE5='{4}',VALUE6='{5}'where ID='{6}'", PAL[0], PAL[1], PAL[2], 0, 0, PAL[5], C);
                insert = string.Format("INSERT OR REPLACE INTO Matrix_Panel_Cam_PLC (STT, X, Y) VALUES (@STT, @X, @Y)");
                //SQLite_Connect();

                using (SQLiteCommand cmd = new SQLiteCommand(insert, _con))
                {
                    cmd.Parameters.AddWithValue("@STT", C);
                    cmd.Parameters.AddWithValue("@X", PAL[0]);
                    cmd.Parameters.AddWithValue("@Y", PAL[1]);

                    cmd.ExecuteNonQuery();

                }
                // insert = string.Format("INSERT INTO matrix_panel(ID,VALUE1,VALUE2,VALUE3,VALUE4,VALUE5,VALUE6) VAlUES('{0}','{1}','{2}','{3}','{4}','{5}','{6}')", C, PAL[0], PAL[1], PAL[2], 0, 0, PAL[5]);

                K = K + 1;

            }
            Disconnect_Sqlite();

        }
        public int[] PAL_P_CAM_PLC(int index, int N, int ind)
        {
            int i = 0;
            int I, T, A, B;
            int[] PPE = new int[2];
            string Zindex = ind.ToString();
            int[] valueX_pos = new int[100];
            I = 1;
            T = 0;
            int[] Counter = new int[20];
            Connect_SQLite(_connectionString);
            while (I < index)
            {
                A = 15 + I;
                T = T + Counter[A];
                I = I + 1;

            }
            if (index > 1)
            {
                i = N + (row * colum * (index - 1));
            }
            else
            {
                i = N;
            }
            B = N + T;
            try
            {

                //string query = "SELECT VALUE1,VALUE2,VALUE3,VALUE4,VALUE5,VALUE6 FROM matrix_panel";
                string query = string.Format("SELECT* from Matrix_Panel_Cam_PLC where STT='{0}' ", i);

                SQLiteCommand cmd = new SQLiteCommand(query, _con);
                var reader = cmd.ExecuteReader();
                double Data;
                while (reader.Read())
                {
                    var ValueX_posX = (reader["X"]);
                    var ValueX_posY = (reader["Y"]);
                    //PLC3E.Write_Data_DWord_("D200" + Zindex, Convert.ToInt32(ValueX_posX));
                    //PLC3E.Write_Data_DWord_("D220" + Zindex, Convert.ToInt32(ValueX_posY));
                    PPE = new int[2] { Convert.ToInt32(ValueX_posX), Convert.ToInt32(ValueX_posY) };
                }



            }

            // lấy pos từ SQLite
            catch (Exception ex) { }
            Disconnect_Sqlite();
            return PPE;
        }
        public void Write_SQL(int[] dataX, int[] dataY, int Number_Tool, string file_name)
        {
            Connect_SQLite(_connectionString);
            for (int i = 1; i < Number_Tool + 1; i++)
            {
                //string insert = string.Format("INSERT OR REPLACE INTO Matrix_Panel1_Cam_PLC (STT, X, Y) VALUES (@STT, @X, @Y)");
                string insert = string.Format(file_name);
                using (SQLiteCommand cmd = new SQLiteCommand(insert, _con))
                {
                    cmd.Parameters.AddWithValue("@STT", i);
                    cmd.Parameters.AddWithValue("@X",Convert.ToDouble( dataX[i - 1])/1000);
                    cmd.Parameters.AddWithValue("@Y", Convert.ToDouble(dataY[i - 1])/1000);
                    cmd.ExecuteNonQuery();
                }
            }
            Disconnect_Sqlite();
        }
        public void Write_SQL2(double[] dataX, double[] dataY, double[] dataZ, double[] dataA3, double[] dataA4, double[] dataC, int Number_Tool)
        {
            Connect_SQLite(_connectionString);
            for (int i = 1; i < Number_Tool + 1; i++)
            {
                string insert = string.Format("INSERT OR REPLACE INTO Matrix_Panel_Cam_Top (ID, X, Y, Z, A3, A4, C) VALUES (@ID, @X, @Y, @Z, @A3, @A4, @C)");
                using (SQLiteCommand cmd = new SQLiteCommand(insert, _con))
                {
                    cmd.Parameters.AddWithValue("@ID", i);
                    cmd.Parameters.AddWithValue("@X", dataX[i - 1]);
                    cmd.Parameters.AddWithValue("@Y", dataY[i - 1]);
                    cmd.Parameters.AddWithValue("@Z", dataZ[i - 1]);
                    cmd.Parameters.AddWithValue("@A3", dataA3[i - 1]);
                    cmd.Parameters.AddWithValue("@A4", dataA4[i - 1]);
                    cmd.Parameters.AddWithValue("@C", dataC[i - 1]);
                    cmd.ExecuteNonQuery();
                }
            }
            Disconnect_Sqlite();
        }
        public void PAL_PR_RB_1_1(int index, int rows, int colums, List<double>[] PP1_1, List<double>[] PP2_1, List<double>[] PP3_1, int Z)
        {
            List<double>[] matrix_panel_1 = new List<double>[10];
            List<double>[] matrix_panel_2 = new List<double>[10];
            List<double>[] matrix_panel_3 = new List<double>[10];

            matrix_panel_1 = PP1_1;
            matrix_panel_2 = PP2_1;
            matrix_panel_3 = PP3_1;
            row = rows;
            colum = colums;
            int I, J, K, L, N, T, A, B, C;
            Connect_SQLite(_connectionString);
            //double[] PAL = PP1;
            double[] PAL = new double[] { 0, 0, 0, 0, 0, matrix_panel_1[0][5] };
            N = rows * colums;
            A = index + 15;
            int[] Counter = new int[200];
            Counter[A] = N;
            L = 1;
            T = 0;
            K = 1;
            while (L < index)
            {
                B = L + 15;
                T = T + Counter[B];
                L = L + 1;

            }
            while (K <= N)
            {
                J = K / row;
                I = (K - 1) - row * J;
                if (I < 0)
                {
                    I = I + row;
                    J = J - 1;
                }
                if (row == 1 || colum == 1)
                {
                    if (row == 1)
                    {
                        PAL[0] = matrix_panel_1[Z][0] + (matrix_panel_2[Z][0] - matrix_panel_1[Z][0]) * I + (matrix_panel_3[Z][0] - matrix_panel_1[Z][0]) / (colum - 1) * J;
                        PAL[1] = matrix_panel_1[Z][1] + (matrix_panel_2[Z][1] - matrix_panel_1[Z][1]) * I + (matrix_panel_3[Z][1] - matrix_panel_1[Z][1]) / (colum - 1) * J;
                        PAL[2] = matrix_panel_1[Z][2] + (matrix_panel_2[Z][2] - matrix_panel_1[Z][2]) * I + (matrix_panel_3[Z][2] - matrix_panel_1[Z][2]) / (colum - 1) * J;

                    }
                    else
                    {
                        PAL[0] = matrix_panel_1[Z][0] + (matrix_panel_2[Z][0] - matrix_panel_1[Z][0]) / (row - 1) * I + (matrix_panel_3[Z][0] - matrix_panel_1[Z][0]) * J;
                        PAL[1] = matrix_panel_1[Z][1] + (matrix_panel_2[Z][1] - matrix_panel_1[Z][1]) / (row - 1) * I + (matrix_panel_3[Z][1] - matrix_panel_1[Z][1]) * J;
                        PAL[2] = matrix_panel_1[Z][2] + (matrix_panel_2[Z][2] - matrix_panel_1[Z][2]) / (row - 1) * I + (matrix_panel_3[Z][2] - matrix_panel_1[Z][2]) * J;
                    }
                }
                else
                {
                    PAL[0] = matrix_panel_1[Z][0] + (matrix_panel_2[Z][0] - matrix_panel_1[Z][0]) / (row - 1) * I + (matrix_panel_3[Z][0] - matrix_panel_1[Z][0]) / (colum - 1) * J;
                    PAL[1] = matrix_panel_1[Z][1] + (matrix_panel_2[Z][1] - matrix_panel_1[Z][1]) / (row - 1) * I + (matrix_panel_3[Z][1] - matrix_panel_1[Z][1]) / (colum - 1) * J;
                    PAL[2] = matrix_panel_1[Z][2] + (matrix_panel_2[Z][2] - matrix_panel_1[Z][2]) / (row - 1) * I + (matrix_panel_3[Z][2] - matrix_panel_1[Z][2]) / (colum - 1) * J;
                }
                if (index > 1)
                {
                    C = K + T + ((index - 1) * N);
                }
                else
                {
                    C = K + T;
                }
                matrix_panel[C] = PAL;
                //textBox1.Text += string.Join(";", matrix_panel[C]) + Environment.NewLine;

                string insert;
                //kiêm tra row tồn tại                            
                // insert = string.Format("UPDATE matrix_panel set  VALUE1='{0}', VALUE2='{1}', VALUE3='{2}', VALUE4='{3}' ,VALUE5='{4}',VALUE6='{5}'where ID='{6}'", PAL[0], PAL[1], PAL[2], 0, 0, PAL[5], C);
                insert = string.Format("INSERT OR REPLACE INTO Matrix_Panel_Tool_1_1 (ID, X, Y,Z,A4,A5,C) VALUES (@ID, @X, @Y,@Z,@A4,@A5,@C)");
                //OR REPLACE
                //Connect_SQLite(_connectionString);
                using (SQLiteCommand cmd = new SQLiteCommand(insert, _con))
                {
                    cmd.Parameters.AddWithValue("@ID", C);
                    cmd.Parameters.AddWithValue("@X", PAL[0]);
                    cmd.Parameters.AddWithValue("@Y", PAL[1]);
                    cmd.Parameters.AddWithValue("@Z", PAL[2]);
                    cmd.Parameters.AddWithValue("@A4", 0.0);
                    cmd.Parameters.AddWithValue("@A5", 0.0);
                    cmd.Parameters.AddWithValue("@C", PAL[5]);
                    cmd.ExecuteNonQuery();

                }
                // insert = string.Format("INSERT INTO matrix_panel(ID,VALUE1,VALUE2,VALUE3,VALUE4,VALUE5,VALUE6) VAlUES('{0}','{1}','{2}','{3}','{4}','{5}','{6}')", C, PAL[0], PAL[1], PAL[2], 0, 0, PAL[5]);

                K = K + 1;

            }

            Disconnect_Sqlite();
        }    
        public void PAL_PR_RB_1_2(int index, int rows, int colums, List<double>[] PP1_1, List<double>[] PP2_1, List<double>[] PP3_1, int Z)
        {
            List<double>[] matrix_panel_1 = new List<double>[10];
            List<double>[] matrix_panel_2 = new List<double>[10];
            List<double>[] matrix_panel_3 = new List<double>[10];

            matrix_panel_1 = PP1_1;
            matrix_panel_2 = PP2_1;
            matrix_panel_3 = PP3_1;
            row = rows;
            colum = colums;
            int I, J, K, L, N, T, A, B, C;
            Connect_SQLite(_connectionString);
            //double[] PAL = PP1;
            double[] PAL = new double[] { 0, 0, 0, 0, 0, matrix_panel_1[0][5] };
            N = rows * colums;
            A = index + 15;
            int[] Counter = new int[200];
            Counter[A] = N;
            L = 1;
            T = 0;
            K = 1;
            while (L < index)
            {
                B = L + 15;
                T = T + Counter[B];
                L = L + 1;

            }
            while (K <= N)
            {
                J = K / row;
                I = (K - 1) - row * J;
                if (I < 0)
                {
                    I = I + row;
                    J = J - 1;
                }
                if (row == 1 || colum == 1)
                {
                    if (row == 1)
                    {
                        PAL[0] = matrix_panel_1[Z][0] + (matrix_panel_2[Z][0] - matrix_panel_1[Z][0]) * I + (matrix_panel_3[Z][0] - matrix_panel_1[Z][0]) / (colum - 1) * J;
                        PAL[1] = matrix_panel_1[Z][1] + (matrix_panel_2[Z][1] - matrix_panel_1[Z][1]) * I + (matrix_panel_3[Z][1] - matrix_panel_1[Z][1]) / (colum - 1) * J;
                        PAL[2] = matrix_panel_1[Z][2] + (matrix_panel_2[Z][2] - matrix_panel_1[Z][2]) * I + (matrix_panel_3[Z][2] - matrix_panel_1[Z][2]) / (colum - 1) * J;

                    }
                    else
                    {
                        PAL[0] = matrix_panel_1[Z][0] + (matrix_panel_2[Z][0] - matrix_panel_1[Z][0]) / (row - 1) * I + (matrix_panel_3[Z][0] - matrix_panel_1[Z][0]) * J;
                        PAL[1] = matrix_panel_1[Z][1] + (matrix_panel_2[Z][1] - matrix_panel_1[Z][1]) / (row - 1) * I + (matrix_panel_3[Z][1] - matrix_panel_1[Z][1]) * J;
                        PAL[2] = matrix_panel_1[Z][2] + (matrix_panel_2[Z][2] - matrix_panel_1[Z][2]) / (row - 1) * I + (matrix_panel_3[Z][2] - matrix_panel_1[Z][2]) * J;
                    }
                }
                else
                {
                    PAL[0] = matrix_panel_1[Z][0] + (matrix_panel_2[Z][0] - matrix_panel_1[Z][0]) / (row - 1) * I + (matrix_panel_3[Z][0] - matrix_panel_1[Z][0]) / (colum - 1) * J;
                    PAL[1] = matrix_panel_1[Z][1] + (matrix_panel_2[Z][1] - matrix_panel_1[Z][1]) / (row - 1) * I + (matrix_panel_3[Z][1] - matrix_panel_1[Z][1]) / (colum - 1) * J;
                    PAL[2] = matrix_panel_1[Z][2] + (matrix_panel_2[Z][2] - matrix_panel_1[Z][2]) / (row - 1) * I + (matrix_panel_3[Z][2] - matrix_panel_1[Z][2]) / (colum - 1) * J;
                }
                if (index > 1)
                {
                    C = K + T + ((index - 1) * N);
                }
                else
                {
                    C = K + T;
                }
                matrix_panel[C] = PAL;
                //textBox1.Text += string.Join(";", matrix_panel[C]) + Environment.NewLine;

                string insert;
                //kiêm tra row tồn tại                            
                // insert = string.Format("UPDATE matrix_panel set  VALUE1='{0}', VALUE2='{1}', VALUE3='{2}', VALUE4='{3}' ,VALUE5='{4}',VALUE6='{5}'where ID='{6}'", PAL[0], PAL[1], PAL[2], 0, 0, PAL[5], C);
                insert = string.Format("INSERT OR REPLACE INTO Matrix_Panel_Tool_1_2 (ID, X, Y,Z,A4,A5,C) VALUES (@ID, @X, @Y,@Z,@A4,@A5,@C)");
                //OR REPLACE
                //Connect_SQLite(_connectionString);
                using (SQLiteCommand cmd = new SQLiteCommand(insert, _con))
                {
                    cmd.Parameters.AddWithValue("@ID", C);
                    cmd.Parameters.AddWithValue("@X", PAL[0]);
                    cmd.Parameters.AddWithValue("@Y", PAL[1]);
                    cmd.Parameters.AddWithValue("@Z", PAL[2]);
                    cmd.Parameters.AddWithValue("@A4", 0.0);
                    cmd.Parameters.AddWithValue("@A5", 0.0);
                    cmd.Parameters.AddWithValue("@C", PAL[5]);
                    cmd.ExecuteNonQuery();

                }
                // insert = string.Format("INSERT INTO matrix_panel(ID,VALUE1,VALUE2,VALUE3,VALUE4,VALUE5,VALUE6) VAlUES('{0}','{1}','{2}','{3}','{4}','{5}','{6}')", C, PAL[0], PAL[1], PAL[2], 0, 0, PAL[5]);

                K = K + 1;

            }

            Disconnect_Sqlite();
        }
        public void PAL_PR_RB_1_3(int index, int rows, int colums, List<double>[] PP1_1, List<double>[] PP2_1, List<double>[] PP3_1, int Z)
        {
            List<double>[] matrix_panel_1 = new List<double>[10];
            List<double>[] matrix_panel_2 = new List<double>[10];
            List<double>[] matrix_panel_3 = new List<double>[10];

            matrix_panel_1 = PP1_1;
            matrix_panel_2 = PP2_1;
            matrix_panel_3 = PP3_1;
            row = rows;
            colum = colums;
            int I, J, K, L, N, T, A, B, C;
            Connect_SQLite(_connectionString);
            //double[] PAL = PP1;
            double[] PAL = new double[] { 0, 0, 0, 0, 0, matrix_panel_1[0][5] };
            N = rows * colums;
            A = index + 15;
            int[] Counter = new int[200];
            Counter[A] = N;
            L = 1;
            T = 0;
            K = 1;
            while (L < index)
            {
                B = L + 15;
                T = T + Counter[B];
                L = L + 1;

            }
            while (K <= N)
            {
                J = K / row;
                I = (K - 1) - row * J;
                if (I < 0)
                {
                    I = I + row;
                    J = J - 1;
                }
                if (row == 1 || colum == 1)
                {
                    if (row == 1)
                    {
                        PAL[0] = matrix_panel_1[Z][0] + (matrix_panel_2[Z][0] - matrix_panel_1[Z][0]) * I + (matrix_panel_3[Z][0] - matrix_panel_1[Z][0]) / (colum - 1) * J;
                        PAL[1] = matrix_panel_1[Z][1] + (matrix_panel_2[Z][1] - matrix_panel_1[Z][1]) * I + (matrix_panel_3[Z][1] - matrix_panel_1[Z][1]) / (colum - 1) * J;
                        PAL[2] = matrix_panel_1[Z][2] + (matrix_panel_2[Z][2] - matrix_panel_1[Z][2]) * I + (matrix_panel_3[Z][2] - matrix_panel_1[Z][2]) / (colum - 1) * J;

                    }
                    else
                    {
                        PAL[0] = matrix_panel_1[Z][0] + (matrix_panel_2[Z][0] - matrix_panel_1[Z][0]) / (row - 1) * I + (matrix_panel_3[Z][0] - matrix_panel_1[Z][0]) * J;
                        PAL[1] = matrix_panel_1[Z][1] + (matrix_panel_2[Z][1] - matrix_panel_1[Z][1]) / (row - 1) * I + (matrix_panel_3[Z][1] - matrix_panel_1[Z][1]) * J;
                        PAL[2] = matrix_panel_1[Z][2] + (matrix_panel_2[Z][2] - matrix_panel_1[Z][2]) / (row - 1) * I + (matrix_panel_3[Z][2] - matrix_panel_1[Z][2]) * J;
                    }
                }
                else
                {
                    PAL[0] = matrix_panel_1[Z][0] + (matrix_panel_2[Z][0] - matrix_panel_1[Z][0]) / (row - 1) * I + (matrix_panel_3[Z][0] - matrix_panel_1[Z][0]) / (colum - 1) * J;
                    PAL[1] = matrix_panel_1[Z][1] + (matrix_panel_2[Z][1] - matrix_panel_1[Z][1]) / (row - 1) * I + (matrix_panel_3[Z][1] - matrix_panel_1[Z][1]) / (colum - 1) * J;
                    PAL[2] = matrix_panel_1[Z][2] + (matrix_panel_2[Z][2] - matrix_panel_1[Z][2]) / (row - 1) * I + (matrix_panel_3[Z][2] - matrix_panel_1[Z][2]) / (colum - 1) * J;
                }
                if (index > 1)
                {
                    C = K + T + ((index - 1) * N);
                }
                else
                {
                    C = K + T;
                }
                matrix_panel[C] = PAL;
                //textBox1.Text += string.Join(";", matrix_panel[C]) + Environment.NewLine;

                string insert;
                //kiêm tra row tồn tại                            
                // insert = string.Format("UPDATE matrix_panel set  VALUE1='{0}', VALUE2='{1}', VALUE3='{2}', VALUE4='{3}' ,VALUE5='{4}',VALUE6='{5}'where ID='{6}'", PAL[0], PAL[1], PAL[2], 0, 0, PAL[5], C);
                insert = string.Format("INSERT OR REPLACE INTO Matrix_Panel_Tool_1_3 (ID, X, Y, Z, A4, A5, C) VALUES (@ID, @X, @Y,@Z,@A4,@A5,@C)");
                //OR REPLACE
                //Connect_SQLite(_connectionString);
                using (SQLiteCommand cmd = new SQLiteCommand(insert, _con))
                {
                    cmd.Parameters.AddWithValue("@ID", C);
                    cmd.Parameters.AddWithValue("@X", PAL[0]);
                    cmd.Parameters.AddWithValue("@Y", PAL[1]);
                    cmd.Parameters.AddWithValue("@Z", PAL[2]);
                    cmd.Parameters.AddWithValue("@A4", 0.0);
                    cmd.Parameters.AddWithValue("@A5", 0.0);
                    cmd.Parameters.AddWithValue("@C", PAL[5]);
                    cmd.ExecuteNonQuery();

                }
                // insert = string.Format("INSERT INTO matrix_panel(ID,VALUE1,VALUE2,VALUE3,VALUE4,VALUE5,VALUE6) VAlUES('{0}','{1}','{2}','{3}','{4}','{5}','{6}')", C, PAL[0], PAL[1], PAL[2], 0, 0, PAL[5]);

                K = K + 1;

            }

            Disconnect_Sqlite();
        }
        public void PAL_PR_RB_1_4(int index, int rows, int colums, List<double>[] PP1_1, List<double>[] PP2_1, List<double>[] PP3_1, int Z)
        {
            List<double>[] matrix_panel_1 = new List<double>[10];
            List<double>[] matrix_panel_2 = new List<double>[10];
            List<double>[] matrix_panel_3 = new List<double>[10];

            matrix_panel_1 = PP1_1;
            matrix_panel_2 = PP2_1;
            matrix_panel_3 = PP3_1;
            row = rows;
            colum = colums;
            int I, J, K, L, N, T, A, B, C;
            Connect_SQLite(_connectionString);
            //double[] PAL = PP1;
            double[] PAL = new double[] { 0, 0, 0, 0, 0, matrix_panel_1[0][5] };
            N = rows * colums;
            A = index + 15;
            int[] Counter = new int[200];
            Counter[A] = N;
            L = 1;
            T = 0;
            K = 1;
            while (L < index)
            {
                B = L + 15;
                T = T + Counter[B];
                L = L + 1;

            }
            while (K <= N)
            {
                J = K / row;
                I = (K - 1) - row * J;
                if (I < 0)
                {
                    I = I + row;
                    J = J - 1;
                }
                if (row == 1 || colum == 1)
                {
                    if (row == 1)
                    {
                        PAL[0] = matrix_panel_1[Z][0] + (matrix_panel_2[Z][0] - matrix_panel_1[Z][0]) * I + (matrix_panel_3[Z][0] - matrix_panel_1[Z][0]) / (colum - 1) * J;
                        PAL[1] = matrix_panel_1[Z][1] + (matrix_panel_2[Z][1] - matrix_panel_1[Z][1]) * I + (matrix_panel_3[Z][1] - matrix_panel_1[Z][1]) / (colum - 1) * J;
                        PAL[2] = matrix_panel_1[Z][2] + (matrix_panel_2[Z][2] - matrix_panel_1[Z][2]) * I + (matrix_panel_3[Z][2] - matrix_panel_1[Z][2]) / (colum - 1) * J;

                    }
                    else
                    {
                        PAL[0] = matrix_panel_1[Z][0] + (matrix_panel_2[Z][0] - matrix_panel_1[Z][0]) / (row - 1) * I + (matrix_panel_3[Z][0] - matrix_panel_1[Z][0]) * J;
                        PAL[1] = matrix_panel_1[Z][1] + (matrix_panel_2[Z][1] - matrix_panel_1[Z][1]) / (row - 1) * I + (matrix_panel_3[Z][1] - matrix_panel_1[Z][1]) * J;
                        PAL[2] = matrix_panel_1[Z][2] + (matrix_panel_2[Z][2] - matrix_panel_1[Z][2]) / (row - 1) * I + (matrix_panel_3[Z][2] - matrix_panel_1[Z][2]) * J;
                    }
                }
                else
                {
                    PAL[0] = matrix_panel_1[Z][0] + (matrix_panel_2[Z][0] - matrix_panel_1[Z][0]) / (row - 1) * I + (matrix_panel_3[Z][0] - matrix_panel_1[Z][0]) / (colum - 1) * J;
                    PAL[1] = matrix_panel_1[Z][1] + (matrix_panel_2[Z][1] - matrix_panel_1[Z][1]) / (row - 1) * I + (matrix_panel_3[Z][1] - matrix_panel_1[Z][1]) / (colum - 1) * J;
                    PAL[2] = matrix_panel_1[Z][2] + (matrix_panel_2[Z][2] - matrix_panel_1[Z][2]) / (row - 1) * I + (matrix_panel_3[Z][2] - matrix_panel_1[Z][2]) / (colum - 1) * J;
                }
                if (index > 1)
                {
                    C = K + T + ((index - 1) * N);
                }
                else
                {
                    C = K + T;
                }
                matrix_panel[C] = PAL;
                //textBox1.Text += string.Join(";", matrix_panel[C]) + Environment.NewLine;

                string insert;
                //kiêm tra row tồn tại                            
                // insert = string.Format("UPDATE matrix_panel set  VALUE1='{0}', VALUE2='{1}', VALUE3='{2}', VALUE4='{3}' ,VALUE5='{4}',VALUE6='{5}'where ID='{6}'", PAL[0], PAL[1], PAL[2], 0, 0, PAL[5], C);
                insert = string.Format("INSERT OR REPLACE INTO Matrix_Panel_Tool_1_4 (ID, X, Y,Z,A4,A5,C) VALUES (@ID, @X, @Y,@Z,@A4,@A5,@C)");
                //OR REPLACE
                //Connect_SQLite(_connectionString);
                using (SQLiteCommand cmd = new SQLiteCommand(insert, _con))
                {
                    cmd.Parameters.AddWithValue("@ID", C);
                    cmd.Parameters.AddWithValue("@X", PAL[0]);
                    cmd.Parameters.AddWithValue("@Y", PAL[1]);
                    cmd.Parameters.AddWithValue("@Z", PAL[2]);
                    cmd.Parameters.AddWithValue("@A4", 0.0);
                    cmd.Parameters.AddWithValue("@A5", 0.0);
                    cmd.Parameters.AddWithValue("@C", PAL[5]);
                    cmd.ExecuteNonQuery();

                }
                // insert = string.Format("INSERT INTO matrix_panel(ID,VALUE1,VALUE2,VALUE3,VALUE4,VALUE5,VALUE6) VAlUES('{0}','{1}','{2}','{3}','{4}','{5}','{6}')", C, PAL[0], PAL[1], PAL[2], 0, 0, PAL[5]);

                K = K + 1;

            }

            Disconnect_Sqlite();
        }
        public double[] PAL_P_RB_1_1(int index, int N, int row, int column)
        {
            int i = 0;
            int I, T, A, B;
            double[] PPE = new double[6];
            I = 1;
            T = 0;
            int[] Counter = new int[2000];
            Flag_Read_Data_Maxtrix_Tool_RB = false;
            while (I < index)
            {
                A = 15 + I;
                T = T + Counter[A];
                I = I + 1;

            }
            if (index > 1)
            {
                i = N + (row * column * (index - 1));
            }
            else
            {
                i = N;
            }
            B = N + T;
            try
            {
                //string query = "SELECT VALUE1,VALUE2,VALUE3,VALUE4,VALUE5,VALUE6 FROM matrix_panel";
                string query = string.Format("SELECT* from Matrix_Panel_Tool_1_1 where ID='{0}' ", i);
                Connect_SQLite(_connectionString);
                SQLiteCommand cmd = new SQLiteCommand(query, _con);
                var reader = cmd.ExecuteReader();
                var pos_list = new List<Tuple<double, double, double, double, double, double>>();
                while (reader.Read())
                {
                    var value1_pos = Math.Round(Convert.ToDouble(reader["X"]), 3);
                    var value2_pos = Math.Round(Convert.ToDouble(reader["Y"]), 3);
                    var value3_pos = Math.Round(Convert.ToDouble(reader["Z"]), 3);
                    var value4_pos = Math.Round(Convert.ToDouble(reader["A4"]), 3);
                    var value5_pos = Math.Round(Convert.ToDouble(reader["A5"]), 3);
                    var value6_pos = Math.Round(Convert.ToDouble(reader["C"]), 3);
                    pos_list.Add(new Tuple<double, double, double, double, double, double>(value1_pos, value2_pos, value3_pos, value4_pos, value5_pos, value6_pos));
                    PPE = new double[] { value1_pos, value2_pos, value3_pos, value4_pos, value5_pos, value6_pos };
                }
                Flag_Read_Data_Maxtrix_Tool_RB = true;

                Disconnect_Sqlite();
            }
            // lấy pos từ SQLite
            catch (Exception ex)
            {
                Flag_Read_Data_Maxtrix_Tool_RB = false;
                Disconnect_Sqlite();
            }

            return PPE;
        }
        public double[] PAL_P_RB_1_2(int index, int N, int row, int column)
        {
            int i = 0;
            int I, T, A, B;
            double[] PPE = new double[6];
            I = 1;
            T = 0;
            int[] Counter = new int[2000];
            Flag_Read_Data_Maxtrix_Tool_RB = false;
            while (I < index)
            {
                A = 15 + I;
                T = T + Counter[A];
                I = I + 1;

            }
            if (index > 1)
            {
                i = N + (row * column * (index - 1));
            }
            else
            {
                i = N;
            }
            B = N + T;
            try
            {
                //string query = "SELECT VALUE1,VALUE2,VALUE3,VALUE4,VALUE5,VALUE6 FROM matrix_panel";
                string query = string.Format("SELECT* from Matrix_Panel_Tool_1_2 where ID='{0}' ", i);
                Connect_SQLite(_connectionString);
                SQLiteCommand cmd = new SQLiteCommand(query, _con);
                var reader = cmd.ExecuteReader();
                var pos_list = new List<Tuple<double, double, double, double, double, double>>();
                while (reader.Read())
                {
                    var value1_pos = Math.Round(Convert.ToDouble(reader["X"]), 3);
                    var value2_pos = Math.Round(Convert.ToDouble(reader["Y"]), 3);
                    var value3_pos = Math.Round(Convert.ToDouble(reader["Z"]), 3);
                    var value4_pos = Math.Round(Convert.ToDouble(reader["A4"]), 3);
                    var value5_pos = Math.Round(Convert.ToDouble(reader["A5"]), 3);
                    var value6_pos = Math.Round(Convert.ToDouble(reader["C"]), 3);
                    pos_list.Add(new Tuple<double, double, double, double, double, double>(value1_pos, value2_pos, value3_pos, value4_pos, value5_pos, value6_pos));
                    PPE = new double[] { value1_pos, value2_pos, value3_pos, value4_pos, value5_pos, value6_pos };
                }
                Flag_Read_Data_Maxtrix_Tool_RB = true;

                Disconnect_Sqlite();
            }
            // lấy pos từ SQLite
            catch (Exception ex)
            {
                Flag_Read_Data_Maxtrix_Tool_RB = false;
                Disconnect_Sqlite();
            }

            return PPE;
        }
        public double[] PAL_P_RB_1_3(int index, int N, int row, int column)
        {
            int i = 0;
            int I, T, A, B;
            double[] PPE = new double[6];
            I = 1;
            T = 0;
            int[] Counter = new int[2000];
            Flag_Read_Data_Maxtrix_Tool_RB = false;
            while (I < index)
            {
                A = 15 + I;
                T = T + Counter[A];
                I = I + 1;

            }
            if (index > 1)
            {
                i = N + (row * column * (index - 1));
            }
            else
            {
                i = N;
            }
            B = N + T;
            try
            {
                //string query = "SELECT VALUE1,VALUE2,VALUE3,VALUE4,VALUE5,VALUE6 FROM matrix_panel";
                string query = string.Format("SELECT* from Matrix_Panel_Tool_1_3 where ID='{0}' ", i);
                Connect_SQLite(_connectionString);
                SQLiteCommand cmd = new SQLiteCommand(query, _con);
                var reader = cmd.ExecuteReader();
                var pos_list = new List<Tuple<double, double, double, double, double, double>>();
                while (reader.Read())
                {
                    var value1_pos = Math.Round(Convert.ToDouble(reader["X"]), 3);
                    var value2_pos = Math.Round(Convert.ToDouble(reader["Y"]), 3);
                    var value3_pos = Math.Round(Convert.ToDouble(reader["Z"]), 3);
                    var value4_pos = Math.Round(Convert.ToDouble(reader["A4"]), 3);
                    var value5_pos = Math.Round(Convert.ToDouble(reader["A5"]), 3);
                    var value6_pos = Math.Round(Convert.ToDouble(reader["C"]), 3);
                    pos_list.Add(new Tuple<double, double, double, double, double, double>(value1_pos, value2_pos, value3_pos, value4_pos, value5_pos, value6_pos));
                    PPE = new double[] { value1_pos, value2_pos, value3_pos, value4_pos, value5_pos, value6_pos };
                }
                Flag_Read_Data_Maxtrix_Tool_RB = true;

                Disconnect_Sqlite();
            }
            // lấy pos từ SQLite
            catch (Exception ex)
            {
                Flag_Read_Data_Maxtrix_Tool_RB = false;
                Disconnect_Sqlite();
            }

            return PPE;
        }
        public double[] PAL_P_RB_1_4(int index, int N)
        {
            int i = 0;
            int I, T, A, B;
            double[] PPE = new double[6];
            I = 1;
            T = 0;
            int[] Counter = new int[2000];
            Flag_Read_Data_Maxtrix_Tool_RB = false;
            while (I < index)
            {
                A = 15 + I;
                T = T + Counter[A];
                I = I + 1;
            }
            if (index > 1)
            {
                i = N + (9 * 2 * (index - 1));
            }
            else
            {
                i = N;
            }
            B = N + T;
            try
            {
                //string query = "SELECT VALUE1,VALUE2,VALUE3,VALUE4,VALUE5,VALUE6 FROM matrix_panel";
                string query = string.Format("SELECT* from Matrix_Panel_Tool_1_4 where ID='{0}' ", i);
                Connect_SQLite(_connectionString);
                SQLiteCommand cmd = new SQLiteCommand(query, _con);
                var reader = cmd.ExecuteReader();
                var pos_list = new List<Tuple<double, double, double, double, double, double>>();
                while (reader.Read())
                {
                    var value1_pos = Math.Round(Convert.ToDouble(reader["VALUE1"]), 3);
                    var value2_pos = Math.Round(Convert.ToDouble(reader["VALUE2"]), 3);
                    var value3_pos = Math.Round(Convert.ToDouble(reader["VALUE3"]), 3);
                    var value4_pos = Math.Round(Convert.ToDouble(reader["VALUE4"]), 3);
                    var value5_pos = Math.Round(Convert.ToDouble(reader["VALUE5"]), 3);
                    var value6_pos = Math.Round(Convert.ToDouble(reader["VALUE6"]), 3);
                    pos_list.Add(new Tuple<double, double, double, double, double, double>(value1_pos, value2_pos, value3_pos, value4_pos, value5_pos, value6_pos));
                    PPE = new double[] { value1_pos, value2_pos, value3_pos, value4_pos, value5_pos, value6_pos };
                }
                Flag_Read_Data_Maxtrix_Tool_RB = true;
                Disconnect_Sqlite();
            }
            // lấy pos từ SQLite
            catch (Exception ex)
            {
                Flag_Read_Data_Maxtrix_Tool_RB = false;
                Disconnect_Sqlite();
            }
            return PPE;
        }
    }
}
