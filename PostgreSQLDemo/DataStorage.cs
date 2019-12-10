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
        public DataStorage() {
            conn = new Npgsql.NpgsqlConnection("Server=127.0.0.1;Port=5432;UserId=postgres;Password=123;Database=TestDb;");            
            conn.Open();
            
        }
        ~DataStorage() {
            conn.Close();
        }
        /// <summary>
        /// 执行sql update，insert等语句，返回影响行数
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public int ExecuteNonQuery(string sql) {
            Npgsql.NpgsqlCommand command = new Npgsql.NpgsqlCommand(sql, conn);
            int rowsaffected = -1;
            try
            {
                rowsaffected = command.ExecuteNonQuery();
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
            Npgsql.NpgsqlCommand command = new Npgsql.NpgsqlCommand(sql, conn);
            try
            {
                obj = command.ExecuteScalar();
            }
            catch {
                Console.WriteLine("查询数据失败");
            }
            return obj;
        }
        public DataSet Query(string sql) {
            DataSet ds = new DataSet();
            NpgsqlDataAdapter adapter = new NpgsqlDataAdapter(sql, conn);
            try
            {
                adapter.Fill(ds);
            }
            catch {
                ds = null;
            }
            return ds;
        }
    }
}
