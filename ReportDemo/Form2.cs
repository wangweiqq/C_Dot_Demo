using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ReportDemo
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
        }
        public static bool IsNumber(String strNumber)
        {
            System.Text.RegularExpressions.Regex objNotNumberPattern = new Regex("[^0-9.-]");
            System.Text.RegularExpressions.Regex objTwoDotPattern = new Regex("[0-9]*[.][0-9]*[.][0-9]*");
            Regex objTwoMinusPattern = new Regex("[0-9]*[-][0-9]*[-][0-9]*");
            String strValidRealPattern = "^([-]|[.]|[-.]|[0-9])[0-9]*[.]*[0-9]+$";
            String strValidIntegerPattern = "^([-]|[0-9])[0-9]*$";
            Regex objNumberPattern = new Regex("(" + strValidRealPattern + ")|(" + strValidIntegerPattern + ")");

            return !objNotNumberPattern.IsMatch(strNumber) &&
                   !objTwoDotPattern.IsMatch(strNumber) &&
                   !objTwoMinusPattern.IsMatch(strNumber) &&
                   objNumberPattern.IsMatch(strNumber);
        }
        private void Form2_Load(object sender, EventArgs e)
        {
            string str = System.IO.File.ReadAllText("D:\\11111.csv");
            string[] line = str.Split('\n');
            DataTable table = new DataTable();
            for (int i = 0; i < line[0].Length; ++i)
            {
                table.Columns.Add();
            }
            int max_cols = 0;
            int empty_count = 0;
            for (int i = 0; i < line.Length; ++i)
            {
                string[] strcols = line[i].Split(',');
                DataRow dr = table.NewRow();
                if (!string.IsNullOrEmpty(strcols[0]))
                {
                    empty_count = 0;
                    for (int j = 0; j < strcols.Length; ++j)
                    {
                        if (string.IsNullOrEmpty(strcols[j]))
                        {
                            max_cols = Math.Max(max_cols, j);
                            break;
                        }
                        if (IsNumber(strcols[j])) {
                            double d = Convert.ToDouble(strcols[j]);
                            dr[j] = d.ToString("#0.0000");
                        }
                        else
                        {
                            dr[j] = strcols[j];
                        }
                    }
                }
                else {
                    empty_count++;
                }
                table.Rows.Add(dr);
                if (empty_count == 2) {
                    //删除多余行
                    for (int e1 = 0; e1 < empty_count; ++e1) {
                        table.Rows.RemoveAt(table.Rows.Count - 1);
                    }
                    break;
                }
                
            }
            //删除多余列
            for (int i = line[0].Length - 1; i >= max_cols; --i)
            {
                table.Columns.RemoveAt(i);
            }

            //this.dataGridView1.AutoGenerateColumns = false;
            //dataGridView1.RowHeadersVisible = false;
            dataGridView1.ColumnHeadersVisible = false;
            dataGridView1.DataSource = table;
            dataGridView1.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            //for (int i = 0; i < dataGridView1.Columns.Count; ++i) {
            //    dataGridView1.Columns[i].DefaultCellStyle.Format = "C3";
            //}
        }

        private void dataGridView1_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            Rectangle rect = new Rectangle(e.RowBounds.Location.X, e.RowBounds.Location.Y,
                dataGridView1.RowHeadersWidth - 4, e.RowBounds.Height);
            TextRenderer.DrawText(e.Graphics, (e.RowIndex + 1).ToString(),
                dataGridView1.RowHeadersDefaultCellStyle.Font, rect,
                dataGridView1.RowHeadersDefaultCellStyle.ForeColor,
                TextFormatFlags.VerticalCenter | TextFormatFlags.Right);
        }
    }
}
