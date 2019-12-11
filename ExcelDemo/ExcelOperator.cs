using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace ExcelManager
{
    public class ExcelOperator
    {
        /// <summary>
        /// 读取指定Excel文件，返回DataTable数据
        /// </summary>
        /// <param name="filepath">Excel路径</param>
        /// <param name="sheetname">Excel Sheet名，为空默认为第一个sheet</param>
        /// <returns>返回DataTable数据</returns>
        public static DataTable ReadExcelToCsv(string filepath,string sheetname = "") {
            DataTable table = null;
            FileInfo file = new FileInfo(filepath);
            if (!file.Exists) {
                return table;
            }
            //using 自动调用Dispose()方法
            using (ExcelPackage package = new ExcelPackage(file)) {
                ExcelWorksheet worksheet = null;
                if (sheetname == "") {
                    worksheet = package.Workbook.Worksheets[1];
                } else
                {
                    worksheet = package.Workbook.Worksheets[sheetname];
                }
                if (worksheet.Dimension == null) {
                    return table;
                }
                int rows = worksheet.Dimension.Rows;
                int cols = worksheet.Dimension.Columns;
                if (rows <= 0 || cols <= 0) {
                    return table;
                }
                table = new DataTable(worksheet.Name);
                DataRow dr;
                object objval;
                for (int j = 0; j < cols; ++j) {
                    table.Columns.Add();
                }
                for (int i = 1; i <= rows; ++i) {
                    dr = table.NewRow();
                    for (int j = 1; j <= cols; ++j) {
                        objval = worksheet.Cells[i, j].Value;
                        dr[j - 1] = objval;
                    }
                    table.Rows.Add(dr);
                }
            }
            return table;
        }
        /// <summary>
        /// 查看字符串是否为数字格式
        /// </summary>
        /// <param name="strNumber"></param>
        /// <returns></returns>
        public static bool IsNumber(String strNumber)
        {
            System.Text.RegularExpressions.Regex objNotNumberPattern = new Regex("[^0-9.-]");
            Regex objTwoDotPattern = new Regex("[0-9]*[.][0-9]*[.][0-9]*");
            Regex objTwoMinusPattern = new Regex("[0-9]*[-][0-9]*[-][0-9]*");
            String strValidRealPattern = "^([-]|[.]|[-.]|[0-9])[0-9]*[.]*[0-9]+$";
            String strValidIntegerPattern = "^([-]|[0-9])[0-9]*$";
            Regex objNumberPattern = new Regex("(" + strValidRealPattern + ")|(" + strValidIntegerPattern + ")");

            return !objNotNumberPattern.IsMatch(strNumber) &&
                   !objTwoDotPattern.IsMatch(strNumber) &&
                   !objTwoMinusPattern.IsMatch(strNumber) &&
                   objNumberPattern.IsMatch(strNumber);
        }
        /// <summary>
        /// 写Excel数据
        /// </summary>
        /// <param name="sheetname">Sheet名称</param>
        /// <param name="filepath">存储路径</param>
        /// <param name="data">数据</param>
        /// <returns></returns>
        public static bool WriteExcel(string sheetname,string filepath, string data) {
            string[] strlist = data.Split('\n');
            if (string.IsNullOrEmpty(sheetname) || string.IsNullOrEmpty(filepath) || strlist.Length == 0) {
                return false;
            }
            var existingFile = new FileInfo(filepath);
            if (existingFile.Exists)
                existingFile.Delete();
            using (ExcelPackage package = new ExcelPackage())
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add(sheetname);
                int maxcols = 0;
                for (int row = 0; row < strlist.Length; ++row)
                {
                    string[] strCols = strlist[row].Split(',');
                    maxcols = Math.Max(maxcols, strCols.Length);
                    for (int cols = 0; cols < strCols.Length; ++cols)
                    {
                        string val = strCols[cols].Trim();
                        if (string.IsNullOrEmpty(val)) {
                            continue;
                        }                        
                        
                        if (IsNumber(val))
                        {
                            double d = Convert.ToDouble(val);
                            worksheet.Cells[row + 1, cols + 1].Value = d;
                        }
                        else
                        {
                            worksheet.Cells[row + 1, cols + 1].Value = val;
                        }
                    }
                }
                worksheet.Cells.AutoFitColumns(0);
                var xlFile = new FileInfo(filepath);
                // save our new workbook in the output directory and we are done!
                package.SaveAs(xlFile);
            }
            return true;
        }
        /// <summary>
        /// 写Excel数据
        /// </summary>
        /// <param name="sheetname">Sheet名称</param>
        /// <param name="filepath">存储路径</param>
        /// <param name="data">数据</param>
        /// <returns></returns>
        public static bool WriteExcel(string sheetname, string filepath, DataTable data)
        {
            if (string.IsNullOrEmpty(sheetname) || string.IsNullOrEmpty(filepath) || data == null || data.Rows.Count == 0) {
                return false;
            }
            var existingFile = new FileInfo(filepath);
            if (existingFile.Exists)
                existingFile.Delete();
            using (ExcelPackage package = new ExcelPackage())
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add(sheetname);
                int rowcount = data.Rows.Count;
                int colcount = data.Columns.Count;
                for (int r = 0; r < rowcount; ++r)
                {
                    DataRow row = data.Rows[r];
                    for (int c = 0; c < colcount; ++c)
                    {
                        string val = row[c].ToString();
                        if (string.IsNullOrEmpty(val)) {
                            continue;
                        }
                        if (IsNumber(val))
                        {
                            double d = Convert.ToDouble(row[c]);
                            worksheet.Cells[r + 1, c + 1].Value = d;
                        }
                        else {
                            worksheet.Cells[r + 1, c + 1].Value = row[c];
                        }
                       
                    }
                }
                //worksheet.Cells["B3:D3"].Style.Numberformat.Format = "#,##0.00";
                //worksheet.Cells[1, 1, strlist.Length, maxcols].Style.Numberformat.Format = "#,##0.00";
                worksheet.Cells.AutoFitColumns(0);
                var xlFile = new FileInfo(filepath);
                // save our new workbook in the output directory and we are done!
                package.SaveAs(xlFile);
            }
            return true;
        }
    }
    
}
