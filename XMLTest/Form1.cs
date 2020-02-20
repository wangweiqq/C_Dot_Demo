using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;

namespace XMLTest
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            //dataGridView1.EditMode= DataGridViewEditMode.
        }
        private void btnReadExcel_Click(object sender, EventArgs e)
        {
            OpenFileDialog readpath = new OpenFileDialog();
            readpath.Filter = "EXCEL文件|*.xlsx";
            if (readpath.ShowDialog() == DialogResult.OK) {
                xmlroot = ExcelManager.ExcelOperator.ReadExcelToXml(readpath.FileName);
                var tableid = from tb in xmlroot.Elements("table")
                              select tb.Attribute("id").Value;
                IList<string> list = new List<string>();
                foreach (string s in tableid) {
                    list.Add(s);
                }
                cbotableids.DataSource = list;
                cbotableids.SelectedIndex = 0;
            }
        }
        private void cbotableids_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selid = ((ComboBox)sender).SelectedItem.ToString();
            //object sss = ((ComboBox)sender).SelectedItem;
            if (string.IsNullOrEmpty(selid)) {
                return;
            }
            xmltable = from tb in xmlroot.Elements("table")
                       where tb.Attribute("id").Value == selid
                       select tb;
            UpdateDataGrid();
        }
        DataTable GetItemDataTable(XElement item) {
            string tableid = item.Attribute("id").Value;
            DataTable data = new DataTable(tableid);
            IEnumerable<XElement> head = item.Element("row").Elements();
            for (int i = 0; i < head.Count(); ++i)
            {
                XElement cols = head.ElementAt(i);
                data.Columns.Add(cols.Value, typeof(XElement));
            }
            var rows = from row in item.Elements("row")
                       select row;
            for (int i = 1; i < rows.Count(); ++i)
            {
                IEnumerable<XElement> colsdata = rows.ElementAt(i).Elements("column");
                DataRow rdata = data.NewRow();
                for (int j = 0; j < colsdata.Count(); ++j)
                {
                    rdata[j] = colsdata.ElementAt(j);
                }
                data.Rows.Add(rdata);
            }
            XElement cell = (XElement)data.Rows[0][0];
            return data;
        }
        private void dataGridView1_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            XElement cell = (XElement)e.Value;
            if (cell == null) {
                return;
            }
            XAttribute formula = cell.Attribute("Formula");
            if (formula != null && !string.IsNullOrEmpty(formula.Value))
            {
                e.Value = formula.Value;
                e.CellStyle.BackColor = Color.Yellow;
                //((DataGridView)sender).Rows[e.RowIndex][e.]
                //dataGridView1.Rows[rindex].Cells[j].Style.BackColor = Color.Yellow;
                //dataGridView1.Rows[rindex].Cells[j].Value = formula.ToString();
            }
            else
            {
                //dataGridView1.Rows[rindex].Cells[j].Value = colsdata.ElementAt(j).Value;
                e.Value = cell.Value;
            }
        }
        int editrow = 0;
        int editcols = 0;
        XElement xmlroot = null;
        IEnumerable<XElement> xmltable = null;
        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            DataGridView obj = (DataGridView)sender;
            DataGridViewSelectedCellCollection cells = obj.SelectedCells;
            if (cells.Count == 1) {
                if (string.IsNullOrEmpty(cells[0].Value.ToString())) { return; }
                //string val = cells[0].Value.ToString();
                XElement val2 = (XElement)cells[0].Value;
                int row = int.Parse( val2.Attribute("row").Value);
                int cols = int.Parse(val2.Attribute("cols").Value);
                XAttribute formula = val2.Attribute("Formula");
                string address = ExcelManager.ExcelOperator.GetExcelAddress(row, cols);
                if (chklockaddree.Checked)
                {
                    txtcellval.Text = string.Format("{0}{1}", txtcellval.Text, address);
                }
                else {
                    lbl_addr.Text = address;
                    editrow = row;
                    editcols = cols;
                    if (formula == null || string.IsNullOrEmpty(formula.Value)) {
                        cbocelltype.SelectedIndex = 0;
                        txtcellval.Text = val2.Value;
                    } else {
                        cbocelltype.SelectedIndex = 1;
                        txtcellval.Text = formula.Value;
                    }
                }
            }
        }
        XElement GetXmlTable(XElement root, string tableid) {
            var table = from tb in root.Elements("table")
                        where tb.Attribute("id").Value == tableid
                        select tb;
            return table.ElementAt(0);
        }
        void SetXmlCellValue(XElement root,string tableid, string rowtitle,string colstitle,string val) {
            SetXmlCellValue(GetXmlTable(root,tableid), rowtitle, colstitle, val);
        }
        void SetXmlCellValue(XElement table, string rowtitle, string colstitle, string val) {
            int cols = int.Parse((from column in table.Elements("row").ElementAt(0).Elements("column")
                                  where column.Value == colstitle
                                  select column.Attribute("cols").Value).ElementAt(0));
            int row = int.Parse((from rowxml in table.Elements("row")
                                 let column = rowxml.Elements("column").ElementAt(0)
                                 where column.Value == rowtitle
                                 select column.Attribute("row").Value).ElementAt(0));
            SetXmlCellValue(table, row, cols, val);
        }
        void SetXmlCellValue(XElement table,int row, int cols, string val) {
            var search = from rows in table.Elements("row")
                         from column in rows.Elements("column")
                         where column.Attribute("row").Value == row.ToString() && column.Attribute("cols").Value == cols.ToString()
                         select column;
            XElement xmlcolumn = search.ElementAt(0);
            xmlcolumn.Value = val;
        }
        private void button4_Click(object sender, EventArgs e)
        {
            //修改dataGridview内容
            if (xmltable == null) {
                return;
            }
            var search = from rows in xmltable.Elements("row")
                         from column in rows.Elements("column")
                         where column.Attribute("row").Value == editrow.ToString() && column.Attribute("cols").Value == editcols.ToString()
                         select column;
            XElement xmlcolumn = search.ElementAt(0);
            switch (cbocelltype.SelectedIndex) {
                case 0:
                    if (xmlcolumn.Attribute("Formula") != null) { xmlcolumn.Attribute("Formula").Remove(); }
                    xmlcolumn.Value = txtcellval.Text.Trim();
                    break;
                case 1:
                    if (xmlcolumn.Attribute("Formula") == null) {
                        xmlcolumn.SetAttributeValue("Formula", txtcellval.Text.Trim());
                    } else {
                        xmlcolumn.Attribute("Formula").Value = txtcellval.Text.Trim();
                    }
                    break;
            }
            UpdateDataGrid();
            xmlroot.Save("ceshi2.xml");
        }
        void UpdateDataGrid() {
            chklockaddree.Checked = false;
            DataTable data = GetItemDataTable(xmltable.ElementAt(0));
            dataGridView1.DataSource = data;
        }
        private void button1_Click(object sender, EventArgs e)
        {
            string tableid = "FAI-6-1";
            xmlroot = XElement.Load("ceshi.xml");
            xmltable = from tb in xmlroot.Elements("table")
                        where tb.Attribute("id").Value == tableid
                        select tb;
            UpdateDataGrid();
            //var rows = from tb in root.Elements("table")
            //           where tb.Attribute("id").Value == tableid
            //           from row in tb.Elements("row")
            //           select row;
            //IEnumerable<XElement> head = xmltable.ElementAt(0).Element("row").Elements();
            //DataTable data = GetItemDataTable(xmltable.ElementAt(0));
            //dataGridView1.DataSource = data;
            //for (int i = 0; i < head.Count(); ++i)
            //{
            //    XElement cols = head.ElementAt(i);
            //    //DataGridViewColumn column = new DataGridViewColumn();
            //    //column.HeaderText = cols.Value;
            //    //column.
            //    dataGridView1.Columns.Add(i.ToString(), cols.Value);
            //}
            ////IEnumerable<XElement> rowdata = rows.Elements("row");
            //for (int i = 1; i < rows.Count(); ++i) {
            //    IEnumerable<XElement> colsdata = rows.ElementAt(i).Elements("column");
            //    int rindex =dataGridView1.Rows.Add();
            //    for (int j = 0; j < colsdata.Count(); ++j) {                    
            //        object formula = colsdata.ElementAt(j).Attribute("Formula");
            //        if (formula != null && !string.IsNullOrEmpty(formula.ToString()))
            //        {
            //            dataGridView1.Rows[rindex].Cells[j].Style.BackColor = Color.Yellow;
            //            dataGridView1.Rows[rindex].Cells[j].Value = formula.ToString();
            //        }
            //        else {
            //            dataGridView1.Rows[rindex].Cells[j].Value = colsdata.ElementAt(j).Value;
            //        }
            //    }
            //}
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            //IEnumerable<XElement> tables = from tb in root.Elements("table") select tb;

            //foreach (XElement t in tables) {
            //    string id = (string)t.Attribute("id");
            //    Console.WriteLine(id);
            //    var row = from r in t.Elements("row") where r.Element("column").Value == "TP" select r;
            //    foreach (var r in row) {
            //        Console.WriteLine(r);
            //        var ccccccc = r.Element("column");
            //        XAttribute aaat = ccccccc.Attribute("type");
            //        var column = from c in row.Elements("column") where c.Attribute("type") != null && c.Attribute("type").Value == "Out" select c;
            //        foreach (var c in column) {
            //            Console.WriteLine(c.Value);
            //        }
            //    }
            //}

            //var cc = from c in (from r in 
            //             (from tb in root.Elements("table") select tb).Elements("row")
            //         where r.Element("column").Value == "TP"
            //         select r).Elements("column")
            //         where c.Attribute("type") != null && c.Attribute("type").Value == "Out"
            //         select c;
            //foreach (var cb in cc) {
            //    Console.WriteLine(cb);
            //}

            //var c1 = from tb in root.Elements("table")
            //         from r in tb.Elements("row")
            //         where r.Element("column").Value == "TP"
            //         from c in r.Elements("column")
            //         where c.Attribute("type") != null && c.Attribute("type").Value == "Out"
            //         group c by tb.Attribute("id").Value;
            //foreach (var cb in c1)
            //{
            //    Console.WriteLine("key = " + cb.Key + ",value = ");
            //    foreach (var e2 in cb)
            //    {
            //        Console.WriteLine(e2.Value);
            //    }
            //    //Console.WriteLine(cb);
            //}
            //IEnumerable formula = from tb in root.Elements("table") where (string)tb.Attribute("id") == "FAI-6-1" select from row in tb where tb.Element("");
            //IEnumerable<XElement> tttt = from tb in root.Elements("table") where (string)tb.Attribute("id") == "FAI-6-1" select tb;
            //XElement xxx = from row in tttt.Elements("row") where row;
            //XElement xxxx = tttt[0];
            //IEnumerable column = from row in (from tb in root.Elements("table") where (string)tb.Attribute("id") == "FAI-6-1" select tb.Element("row")) where row.FirstNode.ToString() == "TP" select row.LastNode;
            //XNode node = (XNode)column;
            //XElement cols = node.Parent;
            //Console.WriteLine(cols.Attribute("type"));
            //Console.WriteLine(cols.ToString());
            //Console.WriteLine(cols.Value);
        }
        private void button2_Click(object sender, EventArgs e)
        {
            string[] numbers = {"0042","010","9","27" };
            //int[] nums = (from s in numbers select int.Parse(s)).OrderBy(s=>s).ToArray();
            int[] nums = numbers.Select(s => int.Parse(s)).OrderBy(s => s).ToArray();
            foreach (int n in nums) {
                Console.WriteLine(n);
            }
            //string[] greetings = { "hello world","hello LINQ","hello Apress","my LINQ","out linq"};
            //var items = from s in greetings where s.EndsWith("LINQ") select s;
            //for (int i = 0; i < items.Count(); ++i) {
            //    Console.WriteLine(items.ElementAt(i));
            //}
            //foreach (var s in items) {
            //    Console.WriteLine(s);
            //}
        }
        void SetHoldData(XElement root, string tableid, string x, string y, string z) {
            var rows = from tb in root.Elements("table")
                       where tb.Attribute("id").Value == tableid
                       let meanid = (from head in tb.Element("row").Elements("column") where head.Value == "MEAS" select head.Attribute("cols").Value).ElementAt(0)
                       from row in tb.Elements("row")
                       let param = row.Element("column").Value
                       where param == "X" || param == "Y" || param == "DF"
                       from cols in row.Elements("column")
                       where cols.Attribute("cols").Value == meanid
                       group cols by param;
            foreach (var c in rows)
            {
                switch (c.Key)
                {
                    case "X":
                        c.ElementAt(0).Value = x;
                        break;
                    case "Y":
                        c.ElementAt(0).Value = y;
                        break;
                    case "DF":
                        c.ElementAt(0).Value = z;
                        break;
                }
            }
        }
        private void button3_Click(object sender, EventArgs e)
        {
            XElement root = ExcelManager.ExcelOperator.ReadExcelToXml("D:\\u5报表.xlsx");
            SetXmlCellValue(root, "FAI-6-1", "X", "MEAS", "3.5291");
            SetXmlCellValue(root, "FAI-6-1", "Y", "MEAS", "812.6247");
            SetXmlCellValue(root, "FAI-6-1", "DF", "MEAS", "17.0521");
            root.Save("ceshi.xml");
            //string id = "FAI-6-1";
            //double x = 2.5291;
            //double y = 812.6247;
            //double z = 17.0521;
            //SetHoldData(root, id, x.ToString(), y.ToString(), z.ToString());
            //bool bl = ExcelManager.ExcelOperator.WriteXmlToExcel("测试u5", "ceshi.xlsx", root.Elements("table"));
            //bool zz = bl == true;
        }
    }
}
