using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DB;
namespace PostgreSQLDemo
{
    public partial class Form1 : Form
    {
        string sql1 = "insert into \"MyTable\"(id,name) values(2,'wangwei2');";
        string sql2 = "select count(*) from \"MyTable\";";
        string sql3 = "select * from \"MyTable\";";
        [System.Runtime.InteropServices.DllImport("kernel32.dll", SetLastError = true)]
        [return: System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.Bool)]
        static extern bool AllocConsole();

        [System.Runtime.InteropServices.DllImport("Kernel32")]
        public static extern void FreeConsole();

        DB.DataStorage db = null;
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            AllocConsole();
            db = new DataStorage("Server=127.0.0.1;Port=5432;UserId=postgres;Password=123;Database=TestDb;");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            int count = db.ExecuteNonQuery(sql1);
            Console.WriteLine("button1_Click count = {0}", count);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            object obj = db.ExecuteScalar(sql2);
            Console.WriteLine("行数：{0}", Convert.ToInt32(obj));
        }

        private void button3_Click(object sender, EventArgs e)
        {
            DataSet ds = db.Query(sql3);
            DataTable tb = ds.Tables[0];
            foreach (DataRow row in tb.Rows) {
                string str = "";
                for (int i = 0; i < tb.Columns.Count; ++i) {
                    //Console.Write(row[i]);
                    str += row[i].ToString();
                    if (i + 1 != tb.Columns.Count) {
                        //Console.Write(",");
                        str += ",";
                    }
                }
                Console.WriteLine(str);
            }
        }
    }
}
