using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using ExcelManager;
namespace ExcelDemo
{
    public partial class Form1 : Form
    {
        [System.Runtime.InteropServices.DllImport("kernel32.dll", SetLastError = true)]
        [return: System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.Bool)]
        static extern bool AllocConsole();

        [System.Runtime.InteropServices.DllImport("Kernel32")]
        public static extern void FreeConsole();

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            AllocConsole();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //读
            DataTable table = ExcelManager.ExcelOperator.ReadExcelToCsv("D:\\u5_report_v2.xlsx");
            if (table == null)
            {
                return;
            }
            StringBuilder builder = new StringBuilder();
            foreach (DataRow dr in table.Rows)
            {
                for (int c = 0; c < table.Columns.Count; ++c)
                {
                    builder.Append(dr[c]);
                    if (c + 1 == table.Columns.Count)
                    {
                        builder.Append("\n");
                    }
                    else
                    {
                        builder.Append(",");
                    }
                }
            }
            Console.Write(builder);
            FileStream fs = new FileStream("D:\\11111.csv", FileMode.Create, FileAccess.Write);
            byte[] buff = System.Text.Encoding.Default.GetBytes(builder.ToString());
            fs.Write(buff, 0, buff.Length);
            fs.Flush();
            fs.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //写
            string str = System.IO.File.ReadAllText("D:\\11111.csv");
            bool bl = ExcelManager.ExcelOperator.WriteExcel("自定义", "222.xlsx", str);
            if (bl)
            {
                DialogResult dr = MessageBox.Show("Excel保存完毕", "对话框标题", MessageBoxButtons.OK, MessageBoxIcon.Question);
                //if (dr == DialogResult.OK)
                //{
                //    //点确定的代码
                //}
                //else
                //{ //点取消的代码 }
                //}
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            string str = System.IO.File.ReadAllText("D:\\11111.csv");
            string[] line = str.Split('\n');
            DataTable table = new DataTable();
            for (int i = 0; i < line[0].Length; ++i) {
                table.Columns.Add();
            }
            for (int i = 0; i < line.Length; ++i) {
                string[] strcols = line[i].Split(',');
                DataRow dr = table.NewRow();
                for (int j = 0; j < strcols.Length; ++j) {
                    //if (ExcelManager.ExcelOperator.IsNumber(strcols[j]))
                    //{
                    //    double d = Convert.ToDouble(strcols[j]);
                    //    dr[j] = d;
                    //}
                    //else {
                        dr[j] = strcols[j];
                    //}                    
                }
                table.Rows.Add(dr);
            }
            bool bl = ExcelManager.ExcelOperator.WriteExcel("", "222.xlsx", table);
        }
    }
}
