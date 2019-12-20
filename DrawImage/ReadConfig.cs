using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.IO;
using Newtonsoft.Json.Linq;

namespace DrawImage
{
    /// <summary>
    /// 文字按钮样式
    /// </summary>
    public struct ButtonStyle
    {
        public int width;//矩形框宽
        public int height;//矩形框高
        public int lineWidth;//矩形边框线宽
        public Point pos;//矩形左上角点
        public string text;//矩形文本
        public Color lineColor;//矩形边框颜色
        public Color fillColor;//矩形填充颜色
        public Color pressColor;//鼠标按下颜色
    }
    /// <summary>
    /// 字体样式
    /// </summary>
    public struct FontStyle {
        public Color fontColor;//字体颜色
        public string font;//字体
        public uint fontSize;//字体大小
    }
    public struct LineStyle {
        public int lineWidth;//连接线宽
        public Color lineColor;//连接颜色
    }
    public class SmallParts {
        public string id;//标签id号
        public ButtonStyle btnStyle;//标签矩形样式，无使用全局样式
        public FontStyle fontStyle;//同上
        public LineStyle lineStyle;//同上
        public Rectangle btnRect;//同按钮矩形，用于查询是否点击
        public Point[] linePoints;//链接线点位置
        public bool isPress = false;//是否被按下
    }
    public class ReadConfig
    {
        static ReadConfig obj = null;
        ButtonStyle btnGlobal;
        FontStyle fontGlobal;
        LineStyle lineGlobal;
        /// <summary>
        /// 按钮区域
        /// </summary>
        Region btnRegion;
        public Hashtable table;
        public static ReadConfig Instance() {
            if (obj == null) {
                obj = new ReadConfig();
            }
            return obj;
        }
        private ReadConfig() {
            table = new Hashtable();
            FileStream fd = new FileStream("Config.json",FileMode.Open);
            byte[] buff = new byte[fd.Length];
            fd.Read(buff, 0, (int)fd.Length);
            string str = Encoding.Default.GetString(buff);
            JObject jsonObject = JObject.Parse(str);
            ParseFont(ref fontGlobal, jsonObject["FontStyle"]);
            ParseButton(ref btnGlobal, jsonObject["ButtonStyle"]);
            ParseLine(ref lineGlobal, jsonObject["LineStyle"]);

            btnRegion = new Region();
            JArray list = (JArray)jsonObject["list"];
            for (int i = 0; i < list.Count; ++i) {
                SmallParts part = ParseParts(list[i]);
                table.Add(part.id, part);
                btnRegion.Union(part.btnRect);
            }
        }
        ~ReadConfig() { }
        /// <summary>
        /// 解析字体样式
        /// </summary>
        /// <param name="font"></param>
        /// <param name="json"></param>
        void ParseFont(ref FontStyle font, JToken json) {
            if (json == null) {
                return;
            }
            JToken tcolor = json["fontColor"];
            if (tcolor != null) {
                font.fontColor = ParseColor(tcolor);
            }
            JToken tfont = json["font"];
            if (tfont != null) {
                font.font = (string)tfont;
            }
            JToken tsize = json["fontSize"];
            if (tsize != null) {
                font.fontSize = (uint)tsize;
            }
        }
        /// <summary>
        /// 解析按钮样式
        /// </summary>
        /// <param name="but"></param>
        /// <param name="json"></param>
        void ParseButton(ref ButtonStyle but, JToken json) {
            if (json == null)
            {
                return;
            }
            JToken token = json["width"];
            if (token != null) {
                but.width = (int)token;
            }
            token = json["height"];
            if (token != null)
            {
                but.height = (int)token;
            }
            token = json["lineWidth"];
            if (token != null)
            {
                but.lineWidth = (int)token;
            }
            token = json["lineColor"];
            if (token != null)
            {
                but.lineColor = ParseColor(token);
            }
            token = json["fillColor"];
            if (token != null)
            {
                but.fillColor = ParseColor(token);
            }
            token = json["pressColor"];
            if (token != null)
            {
                but.pressColor = ParseColor(token);
            }
            token = json["Point"];
            if (token != null)
            {
                but.pos = ParsePoint(token);
            }
            token = json["txt"];
            if (token != null)
            {
                but.text = (string)token;
            }
        }
        /// <summary>
        /// 解析连接线样式
        /// </summary>
        /// <param name="line"></param>
        /// <param name="json"></param>
        void ParseLine(ref LineStyle line, JToken json) {
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
        SmallParts ParseParts(JToken json) {
            SmallParts part = new SmallParts();
            part.btnStyle = btnGlobal;
            part.fontStyle = fontGlobal;
            part.lineStyle = lineGlobal;
            ParseFont(ref part.fontStyle, json["FontStyle"]);
            ParseButton(ref part.btnStyle, json["ButtonStyle"]);
            ParseLine(ref part.lineStyle, json["LineStyle"]);
            part.id = (string)json["id"];
            part.btnRect = new Rectangle(part.btnStyle.pos.X, part.btnStyle.pos.Y, part.btnStyle.width, part.btnStyle.height);
            part.linePoints = ParsePoints(json["Points"]);
            return part;
        }
        Color ParseColor(JToken json) {
            if (json == null) {
                return new Color();
            }
            string[] strcolor = ((string)json).Split(',');
            return Color.FromArgb(byte.Parse(strcolor[0]), byte.Parse(strcolor[1]), byte.Parse(strcolor[2]));
        }
        Point ParsePoint(JToken json) {
            if (json == null)
            {
                return new Point();
            }
            string[] str = ((string)json).Split(',');
            return new Point(int.Parse(str[0]), int.Parse(str[1]));
        }
        Point[] ParsePoints(JToken json) {
            if (json == null) {
                return null;
            }
            string[] str = ((string)json).Split(',');
            int len = str.Length / 2;
            Point[] points = new Point[len];
            for (int i = 0; i < len; ++i) {
                points[i] = new Point(int.Parse(str[i * 2]), int.Parse(str[i * 2 + 1]));
            }
            return points;
        }
        public void DrawSmallParts(ref Graphics g) {
            foreach (SmallParts part in table.Values) {
                if (part.isPress) {
                    g.FillRectangle(new SolidBrush(part.btnStyle.pressColor), part.btnRect);
                }
                else
                {
                    g.FillRectangle(new SolidBrush(part.btnStyle.fillColor), part.btnRect);
                }
                g.DrawRectangle(new Pen(part.btnStyle.lineColor,part.btnStyle.lineWidth), part.btnRect);
                g.DrawLines(new Pen(part.lineStyle.lineColor,part.lineStyle.lineWidth), part.linePoints);
                Font f = new Font(part.fontStyle.font, part.fontStyle.fontSize);
                SizeF strSize = g.MeasureString(part.btnStyle.text, f);
                RectangleF strRect = new RectangleF(part.btnRect.X + (part.btnRect.Width - strSize.Width) / 2, part.btnRect.Y + (part.btnRect.Height - strSize.Height) / 2, strSize.Width, strSize.Height);
                g.DrawString(part.btnStyle.text, f, new SolidBrush(part.fontStyle.fontColor), strRect);
            }
        }
        public string BtnClick(Point pos) {
            if (btnRegion.IsVisible(pos)) {
                foreach (SmallParts part in table.Values) {
                    if (part.btnRect.Contains(pos)) {
                        return part.id;
                    }
                }
            }
            return "";
        }
    }
}
