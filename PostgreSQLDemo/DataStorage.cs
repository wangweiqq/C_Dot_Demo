using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Npgsql;
using System.Data;
namespace DB
{
    public class DataStorage
    {
        Npgsql.NpgsqlConnection conn = null;
        public DataStorage(string connectString) {
            conn = new Npgsql.NpgsqlConnection(connectString);            
            conn.Open();            
        }
        ~DataStorage() {
            conn.Close();
            conn.Dispose();
        }
        /// <summary>
        /// 执行sql update，insert等语句，返回影响行数
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public int ExecuteNonQuery(string sql) {
            
            int rowsaffected = -1;
            try
            {
                using (Npgsql.NpgsqlCommand command = new Npgsql.NpgsqlCommand(sql, conn))
                {
                    rowsaffected = command.ExecuteNonQuery();
                }
                Console.WriteLine("影响行数：{0}", rowsaffected);
            }
            catch (Exception ex)
            {
                Console.WriteLine("插入数据库失败" + ex.Message);
            }
            return rowsaffected;
        }
        /// <summary>
        /// 查询一个字段
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public object ExecuteScalar(string sql) {
            object obj = null;            
            try
            {
                using (Npgsql.NpgsqlCommand command = new Npgsql.NpgsqlCommand(sql, conn))
                {
                    obj = command.ExecuteScalar();
                }
            }
            catch {
                Console.WriteLine("查询数据失败");
            }
            return obj;
        }
        public DataSet Query(string sql) {
            DataSet ds = new DataSet();            
            try
            {
                using (NpgsqlDataAdapter adapter = new NpgsqlDataAdapter(sql, conn))
                {
                    adapter.Fill(ds);
                }
            }
            catch {
                ds = null;
            }
            return ds;
        }
    }
}
