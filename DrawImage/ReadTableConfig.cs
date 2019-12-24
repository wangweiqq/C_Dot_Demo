using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrawImage
{
    class ReadTableConfig
    {
        public struct LineStyle
        {
            public int lineWidth;//连接线宽
            public Color lineColor;//连接颜色
        }
        struct CellStyle {
            public Color fontColor;//字体颜色
            public string fontFamily;//字体类型
            public Font font;//C#字体
            public int fontSize;//字体大小
            public string text;//显示文本
            public Rectangle rect;//字体矩形外包围
            public Point offset;//字体起始点偏移
        }
        struct TableStyle {
            public Point pos;//Table位置
            public int width;//宽度
            public int height;//高度
            public Color fillColor;//table底色
            public Color lineColor;//table线颜色
            public int lineWidth;//线宽
            public int rows;//table为几行几列
            public int cols;
            public Rectangle rect;//table矩形外包围
            public float[] cellwidth;//每个单元格宽度
            public float[] cellheight;//每个单元格高度
            public CellStyle[] cells;//单元格
            public Point[] hPoints1;//水平线
            public Point[] hPoints2;//水平线
            public Point[] vPoints1;//垂直线
            public Point[] vPoints2;//垂直线
        }
        struct TableParts {
            public string id;
            public TableStyle tableSytle;
            public LineStyle lineStyle;//
            public Point[] linePoints;//链接线点位置
        }
        static ReadTableConfig obj = null;
        /// <summary>
        /// 按钮区域
        /// </summary>
        Region tableRegion;
        public Hashtable table;
        TableStyle tableGlobal;
        CellStyle cellGlobal;
        LineStyle lineGlobal;
        public static ReadTableConfig Instance()
        {
            if (obj == null)
            {
                obj = new ReadTableConfig();
            }
            return obj;
        }
        private ReadTableConfig()
        {
            table = new Hashtable();
            FileStream fd = new FileStream("ConfigTable.json", FileMode.Open);
            byte[] buff = new byte[fd.Length];
            fd.Read(buff, 0, (int)fd.Length);
            fd.Close();
            fd.Dispose();
            string str = Encoding.Default.GetString(buff);
            JObject jsonObject = JObject.Parse(str);
            ParseLine(ref lineGlobal, jsonObject["LineStyle"]);
            ParseCell(ref cellGlobal, jsonObject["CellStyle"]);
            ParseTable(ref tableGlobal, jsonObject["TableStyle"]);
            tableRegion = new Region();
            JArray list = (JArray)jsonObject["list"];
            for (int i = 0; i < list.Count; ++i)
            {
                TableParts part = ParseParts(list[i]);
                table.Add(part.id, part);
                tableRegion.Union(part.tableSytle.rect);
            }
        }
        ~ReadTableConfig() { }
        /// <summary>
        /// 解析连接线样式
        /// </summary>
        /// <param name="line"></param>
        /// <param name="json"></param>
        void ParseLine(ref LineStyle line, JToken json)
        {
            if (json == null)
            {
                return;
            }
            JToken token = json["lineWidth"];
            if (token != null)
            {
                line.lineWidth = (int)token;
            }
            token = json["lineColor"];
            if (token != null)
            {
                line.lineColor = ParseColor(token);
            }
        }
        /// <summary>
        /// 解析按钮样式
        /// </summary>
        /// <param name="but"></param>
        /// <param name="json"></param>
        void ParseTable(ref TableStyle table, JToken json)
        {
            if (json == null)
            {
                return;
            }
            JToken token = json["Point"];
            if (token != null)
            {
                table.pos = ParsePoint(token);
            }
            token = json["width"];
            if (token != null)
            {
                table.width = (int)token;
            }
            token = json["height"];
            if (token != null)
            {
                table.height = (int)token;
            }
            table.rect = new Rectangle(table.pos.X, table.pos.Y, table.width, table.height);

            token = json["fillColor"];
            if (token != null)
            {
                table.fillColor = ParseColor(token);
            }
            token = json["lineWidth"];
            if (token != null)
            {
                table.lineWidth = (int)token;
            }
            token = json["lineColor"];
            if (token != null)
            {
                table.lineColor = ParseColor(token);
            }            
            token = json["rows"];
            if (token != null)
            {
                table.rows = (int)token;
            }
            token = json["cols"];
            if (token != null)
            {
                table.cols = (int)token;
            }
            table.cellwidth = ParseCellSize(table.cols, json["cellwidth"], table.width);
            table.cellheight = ParseCellSize(table.rows, json["cellheight"], table.height);

            table.vPoints1 = new Point[table.cols + 1];//上边
            table.vPoints2 = new Point[table.cols + 1];//下边

            table.vPoints1[0] = table.pos;
            table.vPoints1[table.vPoints1.Length - 1] = new Point(table.pos.X + table.width, table.pos.Y);

            table.vPoints2[0] = new Point(table.pos.X, table.pos.Y + table.height);
            table.vPoints2[table.vPoints2.Length - 1] = new Point(table.vPoints2[0].X + table.width, table.vPoints2[0].Y);

            for (int v = 1; v < table.vPoints1.Length - 1; ++v) {
                table.vPoints1[v] = new Point((int)(table.vPoints1[v-1].X + table.cellwidth[v - 1]), table.vPoints1[0].Y);
                table.vPoints2[v] = new Point((int)(table.vPoints2[v-1].X + table.cellwidth[v - 1]), table.vPoints2[0].Y);
            }
            table.hPoints1 = new Point[table.rows + 1];//左边
            table.hPoints2 = new Point[table.rows + 1];//右边

            table.hPoints1[0] = table.pos;
            table.hPoints1[table.hPoints1.Length - 1] = new Point(table.hPoints1[0].X, table.hPoints1[0].Y + table.height);

            table.hPoints2[0] = new Point(table.pos.X + table.width, table.pos.Y);
            table.hPoints2[table.hPoints2.Length - 1] = new Point(table.hPoints2[0].X, table.hPoints2[0].Y + table.height);

            for (int r = 1; r < table.hPoints1.Length - 1; ++r) {
                table.hPoints1[r] = new Point(table.hPoints1[r - 1].X, (int)(table.hPoints1[r - 1].Y + table.cellheight[r - 1]));
                table.hPoints2[r] = new Point(table.hPoints2[r - 1].X, (int)(table.hPoints2[r - 1].Y + table.cellheight[r - 1]));
            }
            JArray list = (JArray)json["CellList"];
            if (list != null && list.Count > 0)
            {
                table.cells = new CellStyle[table.rows * table.cols];
                for (int i = 0; i < list.Count; ++i)
                {
                    CellStyle cell = cellGlobal;
                    ParseCell(ref cell, list[i]);

                    int rows = i / table.cols;
                    int cols = i % table.cols;
                    Point pos1 = table.hPoints1[rows];
                    Point pos2 = table.vPoints1[cols];
                    cell.rect = new Rectangle(pos2.X, pos1.Y, (int)table.cellwidth[cols], (int)table.cellheight[rows]);
                    table.cells[i] = cell;
                }
            }

        }
        void ParseCell(ref CellStyle cell, JToken json) {
            if (json == null)
            {
                return;
            }
            JToken token = json["font"];
            if (token != null)
            {
                cell.fontFamily = (string)token;
            }
            token = json["fontSize"];
            if (token != null)
            {
                cell.fontSize = (int)token;
            }
            cell.font = new Font(cell.fontFamily, cell.fontSize);
            token = json["fontColor"];
            if (token != null)
            {
                cell.fontColor = ParseColor(token);
            }
            token = json["txt"];
            if (token != null)
            {
                cell.text = (string)token;
            }
            token = json["offset"];
            if (token != null)
            {
                cell.offset = ParsePoint(token);
            }
        }
        TableParts ParseParts(JToken json)
        {
            TableParts part = new TableParts();
            part.tableSytle = tableGlobal;
            part.lineStyle = lineGlobal;
            ParseTable(ref part.tableSytle, json["TableStyle"]);
            part.id = (string)json["id"];
            part.linePoints = ParsePoints(json["Points"]);
            return part;
        }
        float[] ParseCellSize(int len, JToken token, int totalSize) {
            int c = 0;
            float arv = ((float)totalSize) / len;
            float sum = 0;
            float[] result = new float[len];
            if (token != null && !string.IsNullOrEmpty((string)token))
            {
                string[] list = ((string)token).Split(',');
                for (c = 0; c < len; ++c)
                {
                    if (c == list.Length)
                    {
                        break;
                    }
                    string str = list[c];
                    if (string.IsNullOrEmpty(str))
                    {
                        arv = ((float)totalSize - sum) / (len - c);
                        result[c] = arv;
                        sum += result[c];
                    }
                    else
                    {
                        result[c] = float.Parse(str);
                        sum += result[c];
                    }
                }
            }
            arv = ((float)totalSize - sum) / (len - c);
            for (; c < len; ++c)
            {
                result[c] = arv;
            }
            return result;
        }
        Color ParseColor(JToken json)
        {
            if (json == null)
            {
                return new Color();
            }
            string[] strcolor = ((string)json).Split(',');
            return Color.FromArgb(byte.Parse(strcolor[0]), byte.Parse(strcolor[1]), byte.Parse(strcolor[2]));
        }
        Point ParsePoint(JToken json)
        {
            if (json == null)
            {
                return new Point();
            }
            string[] str = ((string)json).Split(',');
            return new Point(int.Parse(str[0]), int.Parse(str[1]));
        }
        Point[] ParsePoints(JToken json)
        {
            if (json == null)
            {
                return null;
            }
            string[] str = ((string)json).Split(',');
            int len = str.Length / 2;
            Point[] points = new Point[len];
            for (int i = 0; i < len; ++i)
            {
                points[i] = new Point(int.Parse(str[i * 2]), int.Parse(str[i * 2 + 1]));
            }
            return points;
        }
        public void DrawParts(ref Graphics g)
        {
            foreach (TableParts part in table.Values)
            {
                g.FillRectangle(new SolidBrush(part.tableSytle.fillColor), part.tableSytle.rect);
                //表格外框
                g.DrawRectangle(new Pen(part.tableSytle.lineColor, part.tableSytle.lineWidth), part.tableSytle.rect);
                g.DrawLines(new Pen(part.lineStyle.lineColor, part.lineStyle.lineWidth), part.linePoints);
                //表格内竖线
                for (int v = 1; v < part.tableSytle.vPoints1.Length - 1; ++v)
                {
                    g.DrawLine(new Pen(part.tableSytle.lineColor, part.tableSytle.lineWidth), part.tableSytle.vPoints1[v], part.tableSytle.vPoints2[v]);
                }
                //表格内横线
                for (int h = 1; h < part.tableSytle.hPoints1.Length - 1; ++h)
                {
                    g.DrawLine(new Pen(part.tableSytle.lineColor, part.tableSytle.lineWidth), part.tableSytle.hPoints1[h], part.tableSytle.hPoints2[h]);
                }
                //Font f = new Font(part.fontStyle.font, part.fontStyle.fontSize);
                //SizeF strSize = g.MeasureString(part.btnStyle.text, f);
                //RectangleF strRect = new RectangleF(part.btnRect.X + (part.btnRect.Width - strSize.Width) / 2, part.btnRect.Y + (part.btnRect.Height - strSize.Height) / 2, strSize.Width, strSize.Height);
                foreach (CellStyle cell in part.tableSytle.cells)
                {
                    g.DrawString(cell.text, cell.font, new SolidBrush(cell.fontColor), new Rectangle(cell.rect.X + cell.offset.X, cell.rect.Y + cell.offset.Y, cell.rect.Width - cell.offset.X, cell.rect.Height - cell.offset.Y));
                }
            }
        }
    }
}
