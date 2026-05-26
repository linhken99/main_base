using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Windows;
using System.IO;

namespace Main_Base.IT
{
    public class IT_SQL_Server_Helper
    {
        public static IT_SQL_Server_Helper Instance { get; } = new IT_SQL_Server_Helper();
        public static SqlConnection _connection;
        private static readonly string ConnectionString = "Server=192.168.0.250,1433;Database=SYNOVN_MC;User Id=mc_if;Password=Synovina!@90;Encrypt=True;TrustServerCertificate=True;";
        private static SqlConnection GetConnection()
        {
            return new SqlConnection(ConnectionString);
        }
        private DataTable ToDataTable<T>(IEnumerable<T> data)
        {
            var table = new DataTable();
            var props = typeof(T).GetProperties();

            foreach (var prop in props)
            {
                table.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
            }

            foreach (var item in data)
            {
                var values = props.Select(p => p.GetValue(item) ?? DBNull.Value).ToArray();
                table.Rows.Add(values);
            }

            return table;
        }
        public void Import_SQL<T>(string file_name, List<T> list)
        {
            try
            {
                using (var conn = GetConnection())
                {
                    using (var transaction = conn.BeginTransaction())
                    {
                        foreach (var obj in list)
                        {
                            var props = typeof(T).GetProperties();
                            var columns = string.Join(", ", props.Select(p => p.Name));
                            var parameters = string.Join(", ", props.Select(p => "@" + p.Name));
                            string sql = $"INSERT OR REPLACE INTO {file_name} ({columns}) VALUES ({parameters})";
                            using (var cmd = new SqlCommand(sql, conn, transaction))
                            {
                                foreach (var prop in props)
                                {
                                    var value = prop.GetValue(obj) ?? DBNull.Value;
                                    cmd.Parameters.AddWithValue("@" + prop.Name, value);
                                }
                                cmd.ExecuteNonQuery();
                            }
                        }
                        transaction.Commit();
                    }
                }
            }
            catch { }

        }
        public async Task MergeList<T>(string tableName, IEnumerable<T> list, string keyColumn)
        {
            using (var conn = GetConnection())
            {
                await conn.OpenAsync();

                using (var tran = conn.BeginTransaction())
                {
                    try
                    {
                        foreach (var item in list)
                        {
                            await MergeAsyncInternal(conn, tran, tableName, item, keyColumn);
                        }

                        tran.Commit();
                    }
                    catch
                    {
                        tran.Rollback();
                        throw;
                    }
                }
            }
        }
        private async Task MergeAsyncInternal<T>(SqlConnection conn, SqlTransaction tran, string tableName, T data, string keyColumn)
        {
            var props = typeof(T).GetProperties().Where(p=>p.Name!= "ID").ToList();
            //
            // var columns = props.Select(p => p.Name).ToList();
            var columns = props
                      .Where(p => p.Name != "ID") // fix cứng trước
                      .Select(p => p.Name)
                      .ToList();

            var source = string.Join(", ", columns.Select(c => $"@{c} AS {c}"));

            var update = string.Join(", ", columns
                .Where(c => c != keyColumn)
                .Select(c => $"target.{c} = source.{c}"));

            var insertColumns = string.Join(", ", columns);
            var insertValues = string.Join(", ", columns.Select(c => $"source.{c}"));

            string sql = $@"
                        MERGE INTO {tableName} WITH (HOLDLOCK) AS target
                        USING (SELECT {source}) AS source
                        ON target.{keyColumn} = source.{keyColumn}
                        
                        WHEN MATCHED THEN
                            UPDATE SET {update}
                        
                        WHEN NOT MATCHED THEN
                            INSERT ({insertColumns})
                            VALUES ({insertValues});";
            try
            {
                using (var cmd = new SqlCommand(sql, conn, tran))
                {
                    foreach (var prop in props)
                    {
                        var value = prop.GetValue(data) ?? DBNull.Value;
                        cmd.Parameters.AddWithValue("@" + prop.Name, value);
                    }

                    await cmd.ExecuteNonQueryAsync();
                }
            }
            catch { }
        }
        // insert
        public async Task InsertListIgnoreDuplicate<T>(string tableName, IEnumerable<T> list)
        {
            using (var conn = GetConnection())
            {
                await conn.OpenAsync();

                using (var tran = conn.BeginTransaction())
                {
                    try
                    {
                        foreach (var item in list)
                        {
                            await InsertSingleIgnoreDuplicate(conn, tran, tableName, item);
                        }

                        tran.Commit();
                    }
                    catch
                    {
                        tran.Rollback();
                        throw;
                    }
                }
            }
        }
        private async Task InsertSingleIgnoreDuplicate<T>(SqlConnection conn, SqlTransaction tran, string tableName, T data)
        {
            var props = typeof(T).GetProperties()
                                 .Where(p => p.Name != "ID") // bỏ identity
                                 .ToList();

            var columns = string.Join(", ", props.Select(p => p.Name));
            var values = string.Join(", ", props.Select(p => "@" + p.Name));

            string sql = $@"
        INSERT INTO {tableName} ({columns})
        VALUES ({values});";

            try
            {
                using (var cmd = new SqlCommand(sql, conn, tran))
                {
                    foreach (var prop in props)
                    {
                        var value = prop.GetValue(data) ?? DBNull.Value;
                        cmd.Parameters.AddWithValue("@" + prop.Name, value);
                    }

                    await cmd.ExecuteNonQueryAsync();
                }
            }
            catch (SqlException ex) when (ex.Number == 2627 || ex.Number == 2601)
            {
                //  Duplicate key → bỏ qua
                // 2627: PK
                // 2601: UNIQUE
            }
        }
        //1000+
        public async Task BulkInsertIgnoreDuplicate<T>(string tableName, IEnumerable<T> list, string keyColumn)
        {
            using (var conn = GetConnection())
            {
                await conn.OpenAsync();

                using (var tran = conn.BeginTransaction())
                {
                    try
                    {
                        var props = typeof(T).GetProperties()
                                             .Where(p => p.Name != "ID")
                                             .ToList();

                        // 1. Tạo DataTable
                        var dt = new DataTable();
                        foreach (var prop in props)
                        {
                            dt.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
                        }

                        foreach (var item in list)
                        {
                            var row = dt.NewRow();
                            foreach (var prop in props)
                            {
                                row[prop.Name] = prop.GetValue(item) ?? DBNull.Value;
                            }
                            dt.Rows.Add(row);
                        }

                        // 2. Tạo temp table
                        string tempTable = "#Temp_" + Guid.NewGuid().ToString("N");

                        var createTableSql = $@"
                    CREATE TABLE {tempTable} (
                        {string.Join(", ", props.Select(p => $"{p.Name} NVARCHAR(MAX)"))}
                    );";

                        using (var cmd = new SqlCommand(createTableSql, conn, tran))
                        {
                            await cmd.ExecuteNonQueryAsync();
                        }

                        // 3. Bulk copy vào temp
                        using (var bulk = new SqlBulkCopy(conn, SqlBulkCopyOptions.Default, tran))
                        {
                            bulk.DestinationTableName = tempTable;

                            foreach (var prop in props)
                            {
                                bulk.ColumnMappings.Add(prop.Name, prop.Name);
                            }

                            await bulk.WriteToServerAsync(dt);
                        }

                        // 4. Insert vào bảng chính (lọc duplicate)
                        string insertSql = $@"
                    INSERT INTO {tableName} ({string.Join(", ", props.Select(p => p.Name))})
                    SELECT {string.Join(", ", props.Select(p => "t." + p.Name))}
                    FROM {tempTable} t
                    LEFT JOIN {tableName} main
                        ON t.{keyColumn} = main.{keyColumn}
                    WHERE main.{keyColumn} IS NULL;";

                        using (var cmd = new SqlCommand(insertSql, conn, tran))
                        {
                            await cmd.ExecuteNonQueryAsync();
                        }

                        tran.Commit();
                    }
                    catch
                    {
                        tran.Rollback();
                        throw;
                    }
                }
            }
        }
        //log
        public void Log_Error(string str)
        {
            try
            {
                string path = AppDomain.CurrentDomain.BaseDirectory;
                DateTime dt = DateTime.Now;
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                string namedate = dt.ToString("dd/MM/yyyy");
                namedate = namedate.Replace('/', '_');
                string filePath = Path.Combine(path, "LogError" + namedate + ".txt");
                string[] content = { str };
                if (!File.Exists(filePath))
                {
                    File.Create(filePath).Dispose();
                }
                File.AppendAllLines(filePath, content);
            }
            catch { }
        }
    }
}
