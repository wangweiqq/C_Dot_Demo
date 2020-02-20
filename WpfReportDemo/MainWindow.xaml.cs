using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Linq;

namespace WpfReportDemo
{
    public class TableIdConvert : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return "Name: " + value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    public class TableAddrsConvert : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return "Address: " + value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    public class CellXMLToString : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            XElement cell = (XElement)value;
            XAttribute formula = cell.Attribute("Formula");
            if (formula != null && !string.IsNullOrEmpty(formula.Value))
            {
                return formula.Value;
                //e.Value = formula.Value;
                //e.CellStyle.BackColor = Color.Yellow;
            }
            else
            {
                return cell.Value;
                //dataGridView1.Rows[rindex].Cells[j].Value = colsdata.ElementAt(j).Value;
                //e.Value = cell.Value;
            }
            //throw new NotImplementedException();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        XElement xmlroot = null;
        XElement xmlonetable = null;
        List<dynamic> xmlTables = null;
        int editrow = 0;
        int editcols = 0;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            
        }
        void UpdateDataGrid()
        {
            int colscount = dataGrid.Columns.Count;
            for (int i = colscount - 1; i >= 0; --i) {
                dataGrid.Columns.RemoveAt(i);
            }
            List<string> heads = new List<string>();
            List<dynamic> rows = new List<dynamic>();
            GetItemData(xmlonetable, heads, rows);
            foreach (string head in heads) {
                DataGridTextColumn cols = new DataGridTextColumn();
                cols.Header = head;
                Binding bind = new Binding();
                bind.Path = new PropertyPath(head);
                bind.Converter = new CellXMLToString();
                cols.Binding = bind;
                dataGrid.Columns.Add(cols);
            }
            dataGrid.ItemsSource = rows;
        }
        void GetItemData(XElement item,List<string> heads, List<dynamic> list) {
            IEnumerable<XElement> head = item.Element("row").Elements();
            for (int i = 0; i < head.Count(); ++i)
            {
                XElement cols = head.ElementAt(i);
                heads.Add(cols.Value);
            }
            var rows = from row in item.Elements("row")
                       select row;
            for (int i = 1; i < rows.Count(); ++i)
            {
                IEnumerable<XElement> colsdata = rows.ElementAt(i).Elements("column");
                List<XElement> rowlist = new List<XElement>();
                dynamic obj1 = new System.Dynamic.ExpandoObject();
                IDictionary<string, object> dic = (IDictionary<string, object>)obj1;
                for (int j = 0; j < colsdata.Count(); ++j)
                {
                    dic.Add(heads[j], colsdata.ElementAt(j));
                }
                list.Add(obj1);
            }
        }
        DataTable GetItemDataTable(XElement item)
        {
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

        private void dataGrid_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            DataGrid obj = (DataGrid)sender;
            var cells = obj.SelectedCells;
            var obj2 = obj.SelectedItem;
            //obj.SelectedItems;
            if (cells.Count == 1)
            {
                DataGridColumn column = cells[0].Column;
                IDictionary<string, object> item = (IDictionary<string, object>)cells[0].Item;
                XElement val2 = (XElement)item[column.Header.ToString()];
                int row = int.Parse(val2.Attribute("row").Value);
                int cols = int.Parse(val2.Attribute("cols").Value);
                XAttribute formula = val2.Attribute("Formula");
                string address = ExcelManager.ExcelOperator.GetExcelAddress(row, cols);
                if ((bool)chkLockCell.IsChecked)
                {
                    txtDisplay.Text = string.Format("{0}{1}", txtDisplay.Text, address);
                }
                else
                {
                    lblCell.Text = "当前单元格:" + address;
                    editrow = row;
                    editcols = cols;
                    if (formula == null || string.IsNullOrEmpty(formula.Value))
                    {
                        chkFormula.IsChecked = false;
                        //cbocelltype.SelectedIndex = 0;
                        txtDisplay.Text = val2.Value;
                    }
                    else
                    {
                        chkFormula.IsChecked = true;
                        //cbocelltype.SelectedIndex = 1;
                        txtDisplay.Text = formula.Value;
                    }
                }
                
            }
        }

        private void btnLoadExcel_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog readpath = new OpenFileDialog();
            readpath.Filter = "EXCEL文件|*.xlsx";
            if (readpath.ShowDialog() == true)
            {
                xmlroot = ExcelManager.ExcelOperator.ReadExcelToXml(readpath.FileName);
                //xmlroot = XElement.Load("ceshi.xml");
                IEnumerable<XElement> tables = from tb in xmlroot.Elements("table")
                                               select tb;
                xmlTables = new List<dynamic>();
                foreach (XElement tb in tables)
                {
                    IDictionary<string, object> obj1 = (IDictionary<string, object>)new System.Dynamic.ExpandoObject();
                    obj1.Add("Id", tb.Attribute("id").Value);
                    obj1.Add("Addrs", tb.Attribute("cell").Value);
                    obj1.Add("Table", tb);
                    xmlTables.Add(obj1);
                }
                listTable.ItemsSource = xmlTables;
                //string tableid = "FAI-6-1";
                //xmltable = from tb in xmlroot.Elements("table")
                //           where tb.Attribute("id").Value == tableid
                //           select tb;
                //UpdateDataGrid();
            }
        }

        private void listTable_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListBox obj = (ListBox)sender;
            dynamic item = obj.SelectedItem;
            xmlonetable = item.Table;

            UpdateDataGrid();
        }

        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            //修改dataGridview内容
            if (xmlonetable == null)
            {
                return;
            }
            var search = from rows in xmlonetable.Elements("row")
                         from column in rows.Elements("column")
                         where column.Attribute("row").Value == editrow.ToString() && column.Attribute("cols").Value == editcols.ToString()
                         select column;
            XElement xmlcolumn = search.ElementAt(0);
            switch (chkFormula.IsChecked)
            {
                case false:
                    if (xmlcolumn.Attribute("Formula") != null) { xmlcolumn.Attribute("Formula").Remove(); }
                    xmlcolumn.Value = txtDisplay.Text.Trim();
                    break;
                case true:
                    if (xmlcolumn.Attribute("Formula") == null)
                    {
                        xmlcolumn.SetAttributeValue("Formula", txtDisplay.Text.Trim());
                    }
                    else
                    {
                        xmlcolumn.Attribute("Formula").Value = txtDisplay.Text.Trim();
                    }
                    break;
            }
            UpdateDataGrid();
            xmlroot.Save("ceshi2.xml");
        }
    }
}
